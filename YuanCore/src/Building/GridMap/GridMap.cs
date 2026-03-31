using System;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 逻辑网格封装
/// x 轴正方向 = South
/// y 轴正方向 = East
/// </summary>
public sealed class GridMap<TCell, TEdge>
    where TCell : struct
    where TEdge : struct
{
    private readonly TCell[,] _cells;

    // 以 x 为索引的边
    // 尺寸 [width + 1, height]
    // cell 的 North / South 边
    private readonly TEdge[,] _xEdges;

    // 以 y 为索引的边
    // 尺寸 [width, height + 1]
    // cell 的 West / East 边
    private readonly TEdge[,] _yEdges;

    public readonly int MinX, MinY, Width, Height;

    public int MaxX => MinX + Width - 1;
    public int MaxY => MinY + Height - 1;

    public GridMap(int minX, int minY, int width, int height)
    {
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        MinX = minX;
        MinY = minY;
        Width = width;
        Height = height;

        _cells = new TCell[width, height];
        _xEdges = new TEdge[width + 1, height];
        _yEdges = new TEdge[width, height + 1];
    }


    public bool Contains(int x, int y) => x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    public bool Contains(Vector2Int gridPosition) => Contains(gridPosition.x, gridPosition.y);


    /// <summary>
    /// 获取格数据引用
    /// </summary>
    public ref TCell GetCell(int x, int y) => ref _cells[x - MinX, y - MinY];
    public ref TCell GetCell(Vector2Int gridPosition) => ref GetCell(gridPosition.x, gridPosition.y);

    /// <summary>
    /// 获取边数据引用
    /// </summary>
    public ref TEdge GetEdge(int x, int y, BuildingDirection buildingDirection)
    {
        var localX = x - MinX;
        var localY = y - MinY;

        switch (buildingDirection)
        {
            case BuildingDirection.North:
                return ref _xEdges[localX, localY];
            case BuildingDirection.South:
                return ref _xEdges[localX + 1, localY];
            case BuildingDirection.West:
                return ref _yEdges[localX, localY];
            case BuildingDirection.East:
                return ref _yEdges[localX, localY + 1];
            default:
                throw new ArgumentOutOfRangeException(nameof(buildingDirection), buildingDirection, null);
        }
    }

    public ref TEdge GetEdge(Vector2Int gridPosition, BuildingDirection buildingDirection)
        => ref GetEdge(gridPosition.x, gridPosition.y, buildingDirection);
}
