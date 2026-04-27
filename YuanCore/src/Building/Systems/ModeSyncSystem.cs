using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 每帧检测原版 Mainload 是否从外部触发了模式变更，
/// 并维护 Cursor 实体的 Active 状态。
/// </summary>
public sealed class ModeSyncSystem : IInitializeSystem, IExecuteSystem
{
    private readonly MapContext _context;

    public ModeSyncSystem(MapContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        if (!_context.HasCursor())
            _context.SetCursor(Vector2.zero, Vector2Int.zero, false);
    }

    public void Execute()
    {
        // 让兼容层检测原版是否发起了模式切换
        BuildingModeManager.PollVanillaMode();

        // 同步 Cursor Active 状态
        var mode = BuildingModeManager.CurrentMode;
        var shouldBeActive = mode == BuildingInteractionMode.Build ||
                             mode == BuildingInteractionMode.EditSelect ||
                             mode == BuildingInteractionMode.EditMove;

        if (_context.HasCursor())
        {
            var cursor = _context.GetCursor();
            if (cursor.Active != shouldBeActive)
                _context.ReplaceCursor(cursor.WorldPosition, cursor.GridPosition, shouldBeActive);
        }
    }
}
