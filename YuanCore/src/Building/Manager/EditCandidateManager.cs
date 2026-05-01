using System;
using System.Collections.Generic;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 编辑候选建筑管理器。由 BuildingInputManager 持有并管理生命周期。
/// 订阅 CursorState.OnGridChanged 自动刷新候选列表。
/// </summary>
public sealed class EditCandidateManager
{
    private readonly MapContext _context;
    private string[] _candidates = [];
    private int _currentIndex = -1;
    public bool HasCandidates => _candidates is { Length: > 0 };

    public string CurrentUid
    {
        get
        {
            if (!HasCandidates) return null;
            if (_currentIndex < 0 || _currentIndex >= _candidates.Length) return null;
            return _candidates[_currentIndex];
        }
    }

    private readonly HashSet<string> _seen = new();
    private readonly List<(string uid, int sortKey)> _buffer = new();

    public EditCandidateManager(MapContext context)
    {
        _context = context;
        CursorState.OnGridChanged += OnGridChanged;
    }

    public void Dispose()
    {
        CursorState.OnGridChanged -= OnGridChanged;
    }

    private void OnGridChanged(Vector2Int gridPos)
    {
        if (!CursorState.Active) return;
        if (MainloadCompat.CurrentMode != BuildMode.EditSelect) return;

        Refresh(gridPos);
    }

    /// <summary>
    /// 刷新格子位置上的建筑候选列表。
    /// 候选来源：当前格 Cell 建筑 + 当前格四条边上方向来源匹配的 Edge 建筑。
    /// </summary>
    private void Refresh(Vector2Int gridPos)
    {
        _seen.Clear();
        _buffer.Clear();

        // Cell 建筑
        var cellUids = BuildingManager.States.GetCellBuildingsUid(gridPos);
        foreach (var uid in cellUids)
        {
            if (_seen.Add(uid))
                _buffer.Add((uid, GetSortKey(uid)));
        }

        // Edge 建筑
        var directions = new[]
        {
            BuildingDirection.North,
            BuildingDirection.West,
            BuildingDirection.South,
            BuildingDirection.East
        };

        foreach (var dir in directions)
        {
            var edgeUids = BuildingManager.States.GetEdgeBuildingsUid(gridPos, dir);
            foreach (var uid in edgeUids)
            {
                if (!_seen.Add(uid)) continue;

                if (IsEdgeFromThisCell(uid, gridPos, dir))
                {
                    _buffer.Add((uid, GetSortKey(uid)));
                }
                else
                {
                    _seen.Remove(uid);
                }
            }
        }

        _buffer.Sort((a, b) => a.sortKey.CompareTo(b.sortKey));

        var uidArray = new string[_buffer.Count];
        for (var i = 0; i < _buffer.Count; i++)
            uidArray[i] = _buffer[i].uid;

        SetCandidates(uidArray);
    }

    /// <summary>
    /// 设置候选列表。自动保持已选中建筑（通过 UID 匹配）。
    /// </summary>
    private void SetCandidates(string[] candidates)
    {
        if (candidates == null)
            candidates = Array.Empty<string>();

        var newIndex = 0;
        var oldUid = CurrentUid;
        if (oldUid != null)
        {
            for (var i = 0; i < candidates.Length; i++)
            {
                if (candidates[i] == oldUid)
                {
                    newIndex = i;
                    break;
                }
            }
        }

        _candidates = candidates;
        _currentIndex = candidates.Length > 0 ? Mathf.Clamp(newIndex, 0, candidates.Length - 1) : 0;
    }

    /// <summary>
    /// 循环切换选中项。direction > 0 为下一项，< 0 为上一项。
    /// </summary>
    public void CycleSelection(int direction)
    {
        if (!HasCandidates) return;
        if (_candidates.Length <= 1) return;

        var delta = direction > 0 ? 1 : -1;
        _currentIndex = (_currentIndex + delta + _candidates.Length) % _candidates.Length;
    }

    /// <summary>
    /// 尝试获取当前选中的建筑 UID。
    /// </summary>
    public bool TryGetCurrentUid(out string uid)
    {
        uid = CurrentUid;
        return uid != null;
    }

    /// <summary>
    /// 清除全部候选（离开 EditSelect 模式时调用）。
    /// </summary>
    public void Clear()
    {
        _candidates = Array.Empty<string>();
        _currentIndex = 0;
    }

    /// <summary>
    /// 重置所有状态（场景切换时调用），不依赖 MapContext。
    /// </summary>
    public void Reset()
    {
        _candidates = Array.Empty<string>();
        _currentIndex = 0;
    }

    private bool IsEdgeFromThisCell(string uid, Vector2Int gridPos, BuildingDirection dir)
    {
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
        return (gp.x + gp.y) * 10;
    }
}
