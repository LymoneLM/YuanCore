using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 当 Placement 的 GridPosition 变化时，重新执行占位检测。
/// 检测结果写入 PlacementComponent.Flags 供 View 做可视反馈。
/// </summary>
public sealed class PlacementValidationSystem : IExecuteSystem
{
    private readonly MapContext _context;
    private readonly IGroup<Map.Entity> _group;
    private readonly List<Map.Entity> _buffer = new();

    public PlacementValidationSystem(MapContext context)
    {
        _context = context;
        _group = context.GetGroup(
            Matcher<Map.Entity>.AllOf(
                YuanCoreBuildingMapPlacementMatcher.Placement,
                YuanCoreBuildingMapBuildingMatcher.Building,
                YuanCoreBuildingMapBuildingStateMatcher.BuildingState,
                YuanCoreBuildingMapGridPositionMatcher.GridPosition));
    }

    public void Execute()
    {
        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        _buffer.Clear();
        _buffer.AddRange(_group.GetEntities());

        foreach (var entity in _buffer)
        {
            if (!entity.HasPlacement()) continue;

            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            var gridPos = entity.GetGridPosition().Value;
            var offset = entity.GetPlacement().Offset;

            BuildingStates.Instance.CheckCanBuild(
                building.BuildingID, state.Rotation, gridPos, out var flags);

            entity.ReplacePlacement(offset, flags);
        }
    }
}
