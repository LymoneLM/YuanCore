using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 某个建筑在某个固定旋转下的逻辑占位定义
/// Position 均为相对建筑原点的逻辑坐标
/// </summary>
public readonly struct BuildingShape
{
    public readonly BuildingCellShape[] Cells;
    public readonly BuildingEdgeShape[] Edges;

    public BuildingShape(BuildingCellShape[] cells, BuildingEdgeShape[] edges)
    {
        Cells = cells ?? [];
        Edges = edges ?? [];
    }

    public bool IsEmpty => Cells.Length == 0 && Edges.Length == 0;
}

public readonly record struct BuildingCellShape(
    Vector2Int Position,
    CellOccupancyLayer Layer);

public readonly record struct BuildingEdgeShape(
    Vector2Int Position,
    BuildingDirection Direction,
    EdgeOccupancyLayer Layer);
