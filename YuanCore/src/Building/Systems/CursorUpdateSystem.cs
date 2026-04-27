using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 在 Build / EditSelect / EditMove 模式下，每帧更新 Cursor 的
/// World 坐标和 Grid 逻辑坐标。
/// </summary>
public sealed class CursorUpdateSystem : IExecuteSystem
{
    private readonly MapContext _context;

    public CursorUpdateSystem(MapContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        if (!_context.HasCursor()) return;
        var cursor = _context.GetCursor();
        if (!cursor.Active) return;

        var cam = Camera.main;
        if (cam == null) return;

        var mouseScreen = Input.mousePosition;
        var worldPos = (Vector2)cam.ScreenToWorldPoint(mouseScreen);
        var gridPos = PositionConvertor.WorldToGrid(worldPos);

        if (gridPos != cursor.GridPosition || worldPos != cursor.WorldPosition)
        {
            _context.ReplaceCursor(worldPos, gridPos, true);
        }
    }
}
