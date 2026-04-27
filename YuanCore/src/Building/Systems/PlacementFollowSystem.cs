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
                YuanCoreBuildingMapPlacementMatcher.Placement,
                YuanCoreBuildingMapPlacementSessionMatcher.PlacementSession));
    }

    public void Execute()
    {
        if (!_context.HasCursor()) return;
        var cursor = _context.GetCursor();
        if (!cursor.Active) return;

        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        if (cursor.GridPosition == _lastCursorGrid) return;
        _lastCursorGrid = cursor.GridPosition;

        _buffer.Clear();
        _buffer.AddRange(_placementGroup.GetEntities());

        var sessionId = BuildingModeManager.CurrentSessionId;

        foreach (var entity in _buffer)
        {
            if (!entity.HasPlacementSession()) continue;
            if (entity.GetPlacementSession().SessionId != sessionId) continue;

            var offset = entity.GetPlacement().Offset;
            var newGrid = cursor.GridPosition + offset;

            entity.ReplaceGridPosition(newGrid);
        }
    }
}
