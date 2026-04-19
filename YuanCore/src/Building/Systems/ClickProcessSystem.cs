using System.Collections.Generic;
using Entitas;
using YuanCore.Core;
using static YuanCore.Building.YuanCoreBuildingMapClickedMatcher;

namespace YuanCore.Building;

public sealed class ClickProcessSystem : ReactiveSystem<Map.Entity>
{
    public ClickProcessSystem(MapContext context) : base(context) { }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(Clicked);

    protected override bool Filter(Map.Entity entity)
        => entity.HasClicked() && entity.HasBuilding();

    protected override void Execute(List<Map.Entity> entities)
    {
        foreach (var entity in entities)
        {
            entity.RemoveClicked();

            if (BuildingModeManager.CurrentMode != BuildingInteractionMode.Normal)
                continue;

            var building = entity.GetBuilding();

            BuildingSignals.InvokeBuildingClicked(building.Uid, building.BuildingID);

            YuanCorePlugin.Logger.LogDebug(
                $"[ClickProcess] Building clicked: {building.Uid} ({building.BuildingID})");
        }
    }
}
