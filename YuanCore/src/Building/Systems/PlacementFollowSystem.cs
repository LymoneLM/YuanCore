using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 当 Cursor Grid 坐标变化时，遍历当前会话中的全部 Placement，
/// 更新其逻辑坐标为 CursorGrid + Offset。
/// </summary>
public sealed class PlacementFollowSystem : IExecuteSystem
{
    private readonly MapContext _context;
    private readonly IGroup<Map.Entity> _placementGroup;
    private readonly List<Map.Entity> _buffer = new();
    private Vector2Int _lastCursorGrid;

    public PlacementFollowSystem(MapContext context)
    {
        _context = context;
        _placementGroup = context.GetGroup(
            Matcher<Map.Entity>.AllOf(
                YuanCoreBuildingMapPlacementMatcher.Placement));
    }

    public void Execute()
    {
        if (!CursorState.Active) return;

        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        if (CursorState.GridPosition == _lastCursorGrid) return;
        _lastCursorGrid = CursorState.GridPosition;

        _buffer.Clear();
        _buffer.AddRange(_placementGroup.GetEntities());

        foreach (var entity in _buffer)
        {
            var offset = entity.GetPlacement().Offset;
            entity.ReplaceGridPosition(CursorState.GridPosition + offset);
        }
    }
}
