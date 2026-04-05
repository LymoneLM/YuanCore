using System.Collections.Generic;
using Entitas;
using UnityEngine;
using static YuanCore.Building.YuanCoreBuildingMapBuildingStateMatcher;

namespace YuanCore.Building;

public sealed class CreateViewSystem : ReactiveSystem<Map.Entity>
{
    public CreateViewSystem(MapContext context) : base(context) { }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(BuildingState);

    protected override bool Filter(Map.Entity entity)
        => entity.HasBuilding() && entity.HasBuildingState() && !entity.HasView();

    protected override void Execute(List<Map.Entity> entities)
    {
        foreach (var entity in entities)
        {
            entity.AddView(InstantiateView(entity));
        }
    }

    IView InstantiateView(Map.Entity entity)
    {
        var building = entity.GetBuilding();
        var state = entity.GetBuildingState();
        var prefab = entity.HasPlacement() ?
            PrefabLoader.LoadAsBuildingPlacement<GameObject>(
                $"AllBuild/{state.TaoZhuangID}/BuildTip/{building.BuildingID}/{state.VanillaStateID}") :
            PrefabLoader.LoadAsBuildingShow<GameObject>(
                $"AllBuild/{state.TaoZhuangID}/Scene/{building.BuildingID}/{state.VanillaStateID}");
        var view = Object.Instantiate(prefab, BuildingManager.Instance.BuildViewRoot).GetComponent<IView>();
        view.Link(entity);
        return view;
    }
}
