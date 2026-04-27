using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// EditSelect 模式下，当 Cursor Grid 变化时刷新候选建筑列表。
/// 候选来源：当前格 Cell 建筑 + 当前格四条边上方向来源匹配的 Edge 建筑。
/// 列表以稳定排序呈现。
/// </summary>
public sealed class EditCandidateRefreshSystem : IExecuteSystem
{
    private readonly MapContext _context;
    private Vector2Int _lastGrid;
    private readonly HashSet<string> _seen = new();
    private readonly List<(string uid, int sortKey)> _candidates = new();

    public EditCandidateRefreshSystem(MapContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.EditSelect)
        {
            if (_context.HasEditCandidates())
                _context.RemoveEditCandidates();
            return;
        }

        if (!CursorState.Active) return;

        if (CursorState.GridPosition == _lastGrid && _context.HasEditCandidates()) return;
        _lastGrid = CursorState.GridPosition;

        RefreshCandidates(CursorState.GridPosition);
    }

    private void RefreshCandidates(Vector2Int gridPos)
    {
        _seen.Clear();
        _candidates.Clear();

        // Cell 建筑
        var cellUids = BuildingStates.Instance.GetCellBuildingsUid(gridPos);
        foreach (var uid in cellUids)
        {
            if (_seen.Add(uid))
            {
                var sortKey = GetSortKey(uid);
                _candidates.Add((uid, sortKey));
            }
        }

        // Edge 建筑——只纳入"其边占用定义确实是从当前格发出的"建筑
        var directions = new[]
        {
            BuildingDirection.North,
            BuildingDirection.West,
            BuildingDirection.South,
            BuildingDirection.East
        };

        foreach (var dir in directions)
        {
            var edgeUids = BuildingStates.Instance.GetEdgeBuildingsUid(gridPos, dir);
            foreach (var uid in edgeUids)
            {
                if (!_seen.Add(uid)) continue;

                // 验证此边建筑的边占用定义确实从当前格发出
                if (IsEdgeFromThisCell(uid, gridPos, dir))
                {
                    var sortKey = GetSortKey(uid);
                    _candidates.Add((uid, sortKey));
                }
                else
                {
                    _seen.Remove(uid); // 不合格，允许后续方向再检查
                }
            }
        }

        // 稳定排序：sortKey 升序（sortingOrder 基于 gridPos）
        _candidates.Sort((a, b) => a.sortKey.CompareTo(b.sortKey));

        var uidArray = new string[_candidates.Count];
        for (var i = 0; i < _candidates.Count; i++)
            uidArray[i] = _candidates[i].uid;

        // 保持 currentIndex 有效
        var oldIndex = 0;
        if (_context.HasEditCandidates())
        {
            var old = _context.GetEditCandidates();
            // 尝试保持同一 uid 的选中
            if (old.Candidates != null && old.CurrentIndex < old.Candidates.Length)
            {
                var oldUid = old.Candidates[old.CurrentIndex];
                for (var i = 0; i < uidArray.Length; i++)
                {
                    if (uidArray[i] == oldUid)
                    {
                        oldIndex = i;
                        break;
                    }
                }
            }
        }

        if (oldIndex >= uidArray.Length)
            oldIndex = uidArray.Length > 0 ? 0 : 0;

        _context.ReplaceEditCandidates(uidArray, oldIndex);
    }

    private bool IsEdgeFromThisCell(string uid, Vector2Int gridPos, BuildingDirection dir)
    {
        // 查找此建筑的 shape 定义，确认其边占用确实从 gridPos 出发
        var entity = _context.GetBuildingByUid(uid);
        if (entity == null) return false;

        var building = entity.GetBuilding();
        var state = entity.GetBuildingState();
        var origin = entity.GetGridPosition().Value;

        if (!BuildingShapeRegistry.TryGet(building.BuildingID, state.Rotation, out var shape))
            return false;

        foreach (var edge in shape.Edges)
        {
            var absPos = origin + edge.Position;
            if (absPos == gridPos && edge.Direction == dir)
                return true;
        }
        return false;
    }

    private int GetSortKey(string uid)
    {
        var entity = _context.GetBuildingByUid(uid);
        if (entity == null) return 0;
        if (!entity.HasGridPosition()) return 0;
        var gp = entity.GetGridPosition().Value;
        // 使用与 BuildingView 相同的 sortingOrder 算法
        return (gp.x + gp.y) * 10;
    }
}
