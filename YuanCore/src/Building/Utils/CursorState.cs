using System;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 全局光标状态——纯静态方案，替代 ECS 组件。
/// 网格坐标由 CursorUpdateSystem 每帧更新，Active 由 ModeSyncSystem 控制。
/// </summary>
public static class CursorState
{
    public static Vector2Int GridPosition { get; private set; }
    public static bool Active { get; set; }

    /// <summary>网格坐标变化时触发。</summary>
    public static event Action<Vector2Int> OnGridChanged;

    public static void SetGridPosition(Vector2Int gridPos)
    {
        if (gridPos == GridPosition) return;
        GridPosition = gridPos;
        OnGridChanged?.Invoke(gridPos);
    }

    public static void Reset()
    {
        GridPosition = Vector2Int.zero;
        Active = false;
    }
}
