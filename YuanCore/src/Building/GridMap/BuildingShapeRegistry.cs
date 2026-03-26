using System.Collections.Generic;
using Newtonsoft.Json;

namespace YuanCore.Building;

/// <summary>
/// 建筑逻辑占位注册器
/// </summary>
public static class BuildingShapeRegistry
{
    private static Dictionary<int, BuildingShape> _shapeMap = new();

    public static void Register(int buildingId, BuildingRotation rotation, BuildingShape shape)
    {
        _shapeMap[MakeKey(buildingId, rotation)] = shape;
    }

    public static void Register(BuildingShapeDefinition definition)
    {
        foreach (var (rotation, shape) in definition.ExpandShapes())
        {
            Register(definition.ID, rotation, shape);
        }
    }

    public static void Register(IEnumerable<BuildingShapeDefinition> definitions)
    {
        foreach (var definition in definitions)
        {
            Register(definition);
        }
    }

    public static void RegisterFromJson(string json)
    {
        Register(JsonConvert.DeserializeObject<BuildingShapeDefinition[]>(json) ?? []);
    }

    public static BuildingShape Get(int buildingId, BuildingRotation rotation)
    {
        return _shapeMap[MakeKey(buildingId, rotation)];
    }

    public static bool TryGet(int buildingId, BuildingRotation rotation, out BuildingShape shape)
    {
        return _shapeMap.TryGetValue(MakeKey(buildingId, rotation), out shape);
    }

    public static bool Contains(int buildingId, BuildingRotation rotation)
    {
        return _shapeMap.ContainsKey(MakeKey(buildingId, rotation));
    }

    public static int MakeKey(int buildingId, BuildingRotation rotation)
    {
        return (buildingId << 2) | (int)rotation;
    }
}
