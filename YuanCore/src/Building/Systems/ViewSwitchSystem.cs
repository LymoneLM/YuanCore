using System.Collections.Generic;
using Entitas;
using UnityEngine;
using YuanCore.Core;
using static YuanCore.Building.YuanCoreBuildingMapViewSwitchRequestMatcher;

namespace YuanCore.Building;

public sealed class ViewSwitchSystem : ReactiveSystem<Map.Entity>
{
    public ViewSwitchSystem(MapContext context) : base(context) { }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(ViewSwitchRequest);

    protected override bool Filter(Map.Entity entity)
        => entity.HasViewSwitchRequest() && entity.HasBuilding() && entity.HasBuildingState();

    protected override void Execute(List<Map.Entity> entities)
    {
        foreach (var entity in entities)
        {
            var request = entity.GetViewSwitchRequest();
            entity.RemoveViewSwitchRequest();

            // 销毁旧 View
            if (entity.HasView())
            {
                var oldView = entity.GetView().View;
                if (oldView is MonoBehaviour mb && mb != null)
                    Object.Destroy(mb.gameObject);
                entity.RemoveView();
            }

            // 实例化新 View
            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();

            string path;
            GameObject prefab;

            if (request.ToPlacement)
            {
                path = $"AllBuild/{state.TaoZhuangID}/BuildTip/{building.BuildingID}/{state.VanillaStateID}";
                prefab = PrefabFactory.LoadAsBuildingPlacement<GameObject>(path);
            }
            else
            {
                path = $"AllBuild/{state.TaoZhuangID}/Scene/{building.BuildingID}/{state.VanillaStateID}";
                prefab = PrefabFactory.LoadAsBuildingShow<GameObject>(path);
            }

            if (prefab == null)
            {
                YuanCorePlugin.Logger.LogWarning($"[ViewSwitchSystem] Prefab not found: {path}");
                continue;
            }

            var root = BuildingManager.Instance?.BuildViewRoot;
            if (root == null)
            {
                YuanCorePlugin.Logger.LogWarning("[ViewSwitchSystem] BuildViewRoot is null");
                continue;
            }

            var instance = Object.Instantiate(prefab, root);
            var view = instance.GetComponent<IView>();
            if (view == null)
            {
                YuanCorePlugin.Logger.LogWarning($"[ViewSwitchSystem] No IView on {path}");
                Object.Destroy(instance);
                continue;
            }

            view.Link(entity);
            entity.AddView(view);
        }
    }
}
