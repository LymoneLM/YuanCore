using System;
using UnityEngine;

namespace YuanCore.Building;

public sealed class GridMap<T1, T2>
{
    private T1[,] _cells;
    private T2[,] _horizontalEdges;
    private T2[,] _verticalEdges;

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
        _cells = new T1[width, height];
        _horizontalEdges = new T2[width + 1, height];
        _verticalEdges = new T2[width, height + 1];
    }

    public bool Contains(int x, int y) => x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
    public bool Contains(Vector2Int girdPosition) => Contains(girdPosition.x, girdPosition.y);

    public ref T1 Get(int x, int y) => ref _cells[x - MinX, y - MinY];
    public ref T1 Get(Vector2Int gridPosition) => ref Get(gridPosition.x, gridPosition.y);

    public void Set(int x, int y, T1 value) => _cells[x - MinX, y - MinY] = value;
    public void Set(Vector2Int gridPosition, T1 value) => Set(gridPosition.x, gridPosition.y, value);
}
