using System;
using UnityEngine;

namespace YuanCore.Building;

public sealed class GridMap
{
    private CellData[,] _cells;
    private EdgeData[,] _horizontalEdges;
    private EdgeData[,] _verticalEdges;

    public int MinX { get; }
    public int MinY { get; }
    public int Width { get; }
    public int Height { get; }
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
        _cells = new CellData[width, height];
        _horizontalEdges = new EdgeData[width + 1, height];
        _verticalEdges = new EdgeData[width, height + 1];
    }

    public bool Contains(int x, int y) => x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    public bool Contains(Vector2Int girdPosition) => Contains(girdPosition.x, girdPosition.y);

    public ref CellData Get(int x, int y) => ref _cells[x - MinX, y - MinY];
    public ref CellData Get(Vector2Int gridPosition) => ref Get(gridPosition.x, gridPosition.y);

    public void Set(int x, int y, CellData value) => _cells[x - MinX, y - MinY] = value;
    public void Set(Vector2Int gridPosition, CellData value) => Set(gridPosition.x, gridPosition.y, value);
}
