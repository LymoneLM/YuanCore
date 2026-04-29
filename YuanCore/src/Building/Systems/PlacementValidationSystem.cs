using System.Collections.Generic;
using Entitas;

namespace YuanCore.Building;

/// <summary>
/// 当 Placement 的 GridPosition 或 BuildingState 变化时，重新执行占位检测。
/// 检测结果写入 PlacementComponent.Flags 供 View 做可视反馈。
/// </summary>
public sealed class PlacementValidationSystem : ReactiveSystem<Map.Entity>
{
    private readonly MapContext _context;

    public PlacementValidationSystem(MapContext context) : base(context)
    {
        _context = context;
    }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(
            new TriggerOnEvent<Map.Entity>(
                YuanCoreBuildingMapGridPositionMatcher.GridPosition, GroupEvent.Added),
            new TriggerOnEvent<Map.Entity>(
                YuanCoreBuildingMapBuildingStateMatcher.BuildingState, GroupEvent.Added));

    protected override bool Filter(Map.Entity entity)
        => entity.HasPlacement() && entity.HasBuilding() &&
           entity.HasBuildingState() && entity.HasGridPosition();

    protected override void Execute(List<Map.Entity> entities)
    {
        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        foreach (var entity in entities)
        {
            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;
            var offset = entity.GetPlacement().Offset;

            BuildingManager.States.CheckCanBuild(
                building.BuildingID, state.Rotation, gridPos, out var flags);

            entity.ReplacePlacement(offset, flags);
        }
    }
}
