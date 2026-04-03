using System.Collections.Generic;
using Newtonsoft.Json;

namespace YuanCore.Building;

/// <summary>
/// 建筑逻辑占位注册器
/// </summary>
public static class BuildingShapeRegistry
{
    private static Dictionary<int, BuildingShape> _shapeMap = new();

    public static void Register(int buildingID, BuildingRotation rotation, BuildingShape shape)
    {
        _shapeMap[MakeKey(buildingID, rotation)] = shape;
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

    public static BuildingShape Get(int buildingID, BuildingRotation rotation)
    {
        return _shapeMap[MakeKey(buildingID, rotation)];
    }

    public static bool TryGet(int buildingID, BuildingRotation rotation, out BuildingShape shape)
    {
        return _shapeMap.TryGetValue(MakeKey(buildingID, rotation), out shape);
    }

    public static bool Contains(int buildingID, BuildingRotation rotation)
    {
        return _shapeMap.ContainsKey(MakeKey(buildingID, rotation));
    }

    public static int MakeKey(int buildingID, BuildingRotation rotation)
    {
        return (buildingID << 2) | (int)rotation;
    }
}
