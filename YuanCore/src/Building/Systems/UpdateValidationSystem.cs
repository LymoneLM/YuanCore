using System.Collections.Generic;
using Entitas;
using static YuanCore.Building.YuanCoreBuildingMapGridPositionMatcher;
using static YuanCore.Building.YuanCoreBuildingMapBuildingStateMatcher;
using static YuanCore.Building.YuanCoreBuildingMapPlacementMatcher;

namespace YuanCore.Building;

/// <summary>
/// 当 Placement 的 GridPosition / BuildingState / Placement 变化时，重新执行占位检测。
/// 自动挂载 ValidationComponent，检测结果写入其中供 View 做可视反馈。
/// </summary>
public sealed class UpdateValidationSystem : ReactiveSystem<Map.Entity>
{
    private readonly MapContext _context;

    public UpdateValidationSystem(MapContext context) : base(context)
    {
        _context = context;
    }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(
            new TriggerOnEvent<Map.Entity>(GridPosition, GroupEvent.Added),
            new TriggerOnEvent<Map.Entity>(BuildingState, GroupEvent.Added),
            new TriggerOnEvent<Map.Entity>(Placement, GroupEvent.Added));

    protected override bool Filter(Map.Entity entity)
        => entity.HasPlacement() && entity.HasBuilding() &&
           entity.HasBuildingState() && entity.HasGridPosition();

    protected override void Execute(List<Map.Entity> entities)
    {
        foreach (var entity in entities)
        {
            if (!entity.HasValidation())
                entity.AddValidation([]);

            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;

            BuildingManager.States.CheckCanBuild(
                building.BuildingID, state.Rotation, gridPos, out var flags);

            entity.ReplaceValidation(flags);
        }
    }
}
