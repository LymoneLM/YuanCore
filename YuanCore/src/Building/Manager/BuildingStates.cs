using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using YuanCore.Core;

namespace YuanCore.Building;

// 地图建筑全局状态，全局可读有限写入
// 本质上是被 Manager 控制的，不隶属只是为了便于访问
public class BuildingStates
{
    public static BuildingStates Instance { get; } = new();

    private GridMap<CellData, EdgeData>  _gridMap;
    private Dictionary<string, (int, BuildingRotation, Vector2Int)> _buildings;

    private CellOccupancyLayer[] _cellOccupancyMask;
    private EdgeOccupancyLayer[] _edgeOccupancyMask;

    private Dictionary<(string, int),(int, int, int, int)> _mapShape;

    private BuildingStates()
    {
        // BuildingShapeRegistry 初始化
        var executingAssembly = Assembly.GetExecutingAssembly();
        var modPath = Path.GetDirectoryName(executingAssembly.Location);
        var json = File.ReadAllText(modPath + "/BuildingShape.json");
        BuildingShapeRegistry.RegisterFromJson(json);

        // 构建碰撞位遮罩
        var json2 = File.ReadAllText(modPath + "/LayerCollision.json");
        (_cellOccupancyMask, _edgeOccupancyMask) = LayerMaskBuilder.LoadFromJson(json2);

        // 读取地图范围
        var json3 = File.ReadAllText(modPath + "/MapShape.json");
        _mapShape = JsonConvert.DeserializeObject<List<MapShapeDefinition>>(json3)
            .ToDictionary(
                def => (def.Class, def.Index),
                def => (def.MinX, def.MinY, def.Width, def.Height)
            );
    }

    public void InitializeMap(int minX, int minY, int width, int height)
    {
        _gridMap = new GridMap<CellData, EdgeData>(minX, minY, width, height);
        _buildings = [];
    }

    public void InitializeMap(string sceneClass, int sceneIndex)
    {
        var (x, y, w, h) = _mapShape[(sceneClass, sceneIndex)];
        YuanCorePlugin.Logger.LogDebug($"InitializeMap[{sceneClass}|{sceneIndex}]:({x}, {y}, {w}, {h})");
        InitializeMap(x, y, w, h);
    }

    public void AddBuilding(int buildingID, BuildingRotation rotation, Vector2Int posi, string uID)
    {
        if(_gridMap == null)
            throw new InvalidOperationException("GridMap has not been initialized.");

        if(_buildings.ContainsKey(uID))
            YuanCorePlugin.Logger.LogWarning($"Can't add registered building {uID}");

        _buildings[uID] = (buildingID, rotation, posi);
        var shape = BuildingShapeRegistry.Get(buildingID, rotation);
        foreach (var cell in shape.Cells)
        {
            var pos = posi + cell.Position;
            _gridMap.GetCell(pos).Add(new CellOccupant(uID, cell.Layer));
        }
        foreach (var edge in shape.Edges)
        {
            var pos = posi + edge.Position;
            _gridMap.GetEdge(pos, edge.Direction).Add(new EdgeOccupant(uID, edge.Direction, edge.Layer));
        }
    }

    public void RemoveBuilding(string uID)
    {
        if(_gridMap == null)
            throw new InvalidOperationException("GridMap has not been initialized.");

        if(!_buildings.ContainsKey(uID))
            YuanCorePlugin.Logger.LogWarning($"Can't remove unregistered building {uID}");

        var (buildingID, rotation, posi) = _buildings[uID];
        var shape = BuildingShapeRegistry.Get(buildingID, rotation);
        foreach (var cell in shape.Cells)
        {
            var pos = posi + cell.Position;
            _gridMap.GetCell(pos).Remove(new CellOccupant(uID, cell.Layer));
        }
        foreach (var edge in shape.Edges)
        {
            var pos = posi + edge.Position;
            _gridMap.GetEdge(pos, edge.Direction).Remove(new EdgeOccupant(uID, edge.Direction, edge.Layer));
        }

        _buildings.Remove(uID);
    }

    public bool CheckCanBuild(int buildingID, BuildingRotation rotation, Vector2Int posi, out (Vector2Int, bool)[] result)
    {
        if(_gridMap == null)
            throw new InvalidOperationException("GridMap has not been initialized.");

        var shape = BuildingShapeRegistry.Get(buildingID, rotation);
        result = new (Vector2Int, bool)[shape.Cells.Length+shape.Edges.Length];
        var idx = 0;
        var flag = true;

        foreach (var cell in shape.Cells)
        {
            var pos = posi + cell.Position;
            var res = _gridMap.Contains(pos) && (_gridMap.GetCell(pos).CellOccupancyLayer &
                _cellOccupancyMask[BitOperations.TrailingZeroCount64((ulong)cell.Layer)]) == CellOccupancyLayer.None;
            flag &= res;
            result[idx++] = (cell.Position, res);
        }

        foreach (var edge in shape.Edges)
        {
            var pos = posi + edge.Position;
            var res = _gridMap.Contains(pos) && (_gridMap.GetEdge(pos, edge.Direction).EdgeOccupancyLayer &
                _edgeOccupancyMask[BitOperations.TrailingZeroCount64((ulong)edge.Layer)]) == EdgeOccupancyLayer.None;
            flag &= res;
            result[idx++] = (edge.Position, res);
        }

        return flag;
    }

    public string[] GetCellBuildingsUid(Vector2Int posi)
    {
        if(_gridMap == null)
            throw new InvalidOperationException("GridMap has not been initialized.");

        return _gridMap.Contains(posi) ? _gridMap.GetCell(posi).GetUid() : [];
    }

    public string[] GetEdgeBuildingsUid(Vector2Int posi, BuildingDirection direction)
    {
        if(_gridMap == null)
            throw new InvalidOperationException("GridMap has not been initialized.");

        return _gridMap.Contains(posi) ? _gridMap.GetEdge(posi, direction).GetUid() : [];
    }
}

public class MapShapeDefinition
{
    [JsonProperty("Class")]
    public string Class { get; set; }

    [JsonProperty("Index")]
    public int Index { get; set; }

    [JsonProperty("MinX")]
    public int MinX { get; set; }

    [JsonProperty("MinY")]
    public int MinY { get; set; }

    [JsonProperty("Width")]
    public int Width { get; set; }

    [JsonProperty("Height")]
    public int Height { get; set; }
}
