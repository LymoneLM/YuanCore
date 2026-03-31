using UnityEngine;

namespace YuanCore.Building;

// 世界坐标与逻辑坐标变换，热路径优化
public static class PositionConvertor
{
    private const float INV082 = 1f / 0.82f;
    private const float INV164 = 1f / 1.64f;
    private const float HALF_WIDTH = 0.82f;
    private const float HALF_HEIGHT = 0.41f;
    private const float WORLD_Y_OFFSET = 5.38f;

    public static Vector2Int WorldToGrid(Vector2 worldPos)
    {
        var a = (WORLD_Y_OFFSET - worldPos.y) * INV082;
        var b = worldPos.x * INV164;

        return new Vector2Int(
            Mathf.RoundToInt(a - b),
            Mathf.RoundToInt(a + b)
        );
    }

    public static Vector2 GridToWorld(Vector2Int gridPos)
    {
        var sum = gridPos.x + gridPos.y;
        var diff = gridPos.y - gridPos.x;

        return new Vector2(
            diff * HALF_WIDTH,
            WORLD_Y_OFFSET - sum * HALF_HEIGHT
        );
    }
}
