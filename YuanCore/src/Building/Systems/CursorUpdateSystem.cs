using Entitas;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 在 Build / EditSelect / EditMove 模式下，每帧读取鼠标位置，
/// 更新 CursorState 的网格坐标。
/// </summary>
public sealed class CursorUpdateSystem : IExecuteSystem
{
    private Camera _camera;

    public CursorUpdateSystem()
    {
        _camera = Camera.main;
    }

    public void Execute()
    {
        if (!CursorState.Active) return;

        var cam = _camera;
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
            _camera = cam;
        }

        var mouseScreen = Input.mousePosition;
        var worldPos = (Vector2)cam.ScreenToWorldPoint(mouseScreen);
        var gridPos = PositionConvertor.WorldToGrid(worldPos);

        CursorState.SetGridPosition(gridPos);
    }
}
