using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 建筑逻辑占位 JSON 反序列化模型
/// </summary>
public sealed class BuildingShapeDefinition
{
    [JsonProperty("ID")]
    public int ID { get; set; }

    [JsonProperty("Rotations")]
    public BuildingRotation[] Rotations { get; set; } = [];

    [JsonProperty("Footprints")]
    public BuildingFootprintDefinition[] Footprints { get; set; } = [];

    [JsonProperty("Cells")]
    public BuildingCellDefinition[] Cells { get; set; } = [];

    [JsonProperty("Edges")]
    public BuildingEdgeDefinition[] Edges { get; set; } = [];

    /// <summary>
    /// 构建 Rotations[0] 对应的基础形状
    /// 1. Footprints 按顺序写入，后读覆盖
    /// 2. Cells 后读覆盖 Footprints
    /// 3. Edges 自己内部后读覆盖
    /// </summary>
    public BuildingShape BuildBaseShape()
    {
        var cellMap = new Dictionary<Vector2Int, CellOccupancyLayer>();
        var edgeMap = new Dictionary<EdgeKey, EdgeOccupancyLayer>();

        if (Footprints != null)
        {
            foreach (var footprint in Footprints)
            {
                var layer = ParseCellLayer(footprint.Layer);

                for (var x = footprint.XMin; x <= footprint.XMax; ++x)
                {
                    for (var y = footprint.YMin; y <= footprint.YMax; ++y)
                    {
                        cellMap[new Vector2Int(x, y)] = layer;
                    }
                }
            }
        }

        if (Cells != null)
        {
            foreach (var cell in Cells)
            {
                cellMap[new Vector2Int(cell.X, cell.Y)] = ParseCellLayer(cell.Layer);
            }
        }

        if (Edges != null)
        {
            foreach (var edge in Edges)
            {
                edgeMap[new EdgeKey(edge.X, edge.Y, ParseDirection(edge.Direction))] =
                    ParseEdgeLayer(edge.Layer);
            }
        }

        var cells = new BuildingCellShape[cellMap.Count];
        var cellIndex = 0;
        foreach (var pair in cellMap)
        {
            cells[cellIndex++] = new BuildingCellShape(pair.Key, pair.Value);
        }

        var edges = new BuildingEdgeShape[edgeMap.Count];
        var edgeIndex = 0;
        foreach (var pair in edgeMap)
        {
            edges[edgeIndex++] = new BuildingEdgeShape(
                new Vector2Int(pair.Key.X, pair.Key.Y),
                pair.Key.Direction,
                pair.Value);
        }

        return new BuildingShape(cells, edges);
    }

    /// <summary>
    /// 根据 Rotations[0] 定义展开所有需要注册的旋转形态
    /// </summary>
    public IEnumerable<(BuildingRotation Rotation, BuildingShape Shape)> ExpandShapes()
    {
        if (Rotations == null || Rotations.Length == 0)
            yield break;

        var baseShape = BuildBaseShape();
        var baseRotation = Rotations[0];

        yield return (baseRotation, baseShape);

        for (var i = 1; i < Rotations.Length; ++i)
        {
            var targetRotation = Rotations[i];
            yield return (targetRotation, DeriveShape(baseShape, baseRotation, targetRotation));
        }
    }

    /// <summary>
    /// 旋转推导函数
    /// </summary>
    private static BuildingShape DeriveShape(
        BuildingShape baseShape,
        BuildingRotation baseRotation,
        BuildingRotation targetRotation)
    {
        return baseShape;
    }

    private static CellOccupancyLayer ParseCellLayer(string value)
    {
        if (!Enum.TryParse<CellOccupancyLayer>(value, true, out var layer))
            throw new InvalidOperationException($"Invalid CellOccupancyLayer: {value}");

        return layer;
    }

    private static EdgeOccupancyLayer ParseEdgeLayer(string value)
    {
        if (!Enum.TryParse<EdgeOccupancyLayer>(value, true, out var layer))
            throw new InvalidOperationException($"Invalid EdgeOccupancyLayer: {value}");

        return layer;
    }

    private static BuildingDirection ParseDirection(string value)
    {
        if (!Enum.TryParse<BuildingDirection>(value, true, out var direction))
            throw new InvalidOperationException($"Invalid Direction: {value}");

        return direction;
    }

    private readonly record struct EdgeKey(int X, int Y, BuildingDirection Direction);
}

public sealed class BuildingFootprintDefinition
{
    [JsonProperty("XMin")]
    public int XMin { get; set; }

    [JsonProperty("YMin")]
    public int YMin { get; set; }

    [JsonProperty("XMax")]
    public int XMax { get; set; }

    [JsonProperty("YMax")]
    public int YMax { get; set; }

    [JsonProperty("Layer")]
    public string Layer { get; set; } = string.Empty;
}

public sealed class BuildingCellDefinition
{
    [JsonProperty("X")]
    public int X { get; set; }

    [JsonProperty("Y")]
    public int Y { get; set; }

    [JsonProperty("Layer")]
    public string Layer { get; set; } = string.Empty;
}

public sealed class BuildingEdgeDefinition
{
    [JsonProperty("X")]
    public int X { get; set; }

    [JsonProperty("Y")]
    public int Y { get; set; }

    [JsonProperty("Direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonProperty("Layer")]
    public string Layer { get; set; } = string.Empty;
}
