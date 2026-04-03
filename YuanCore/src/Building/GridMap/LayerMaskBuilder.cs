using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;

namespace YuanCore.Building;

public static class LayerMaskBuilder
{
    public static CollisionMasks LoadFromJson(string json)
    {
        var obj = JsonConvert.DeserializeObject<LayerCollision>(json);
        if (obj == null)
            throw new InvalidOperationException("Failed to deserialize collision config.");

        return new CollisionMasks
        {
            CellLayerMask = BuildLayerMask<CellOccupancyLayer>(obj.Cells),
            EdgeLayerMask = BuildLayerMask<EdgeOccupancyLayer>(obj.Edges)
        };
    }

    private static TEnum[] BuildLayerMask<TEnum>(List<List<string>> groups)
        where TEnum : struct, Enum
    {
        var allLayers = GetAllLayersExceptNone<TEnum>();
        var maxIndex = GetLayerIndex(allLayers.Max());
        var result = new TEnum[maxIndex+1];

        if (groups == null)
            return result;

        foreach (var group in groups)
        {
            if (group == null || group.Count == 0)
                continue;

            var flagsInGroup = new HashSet<TEnum>();

            foreach (var name in group)
            {
                var parsed = ParseLayer<TEnum>(name);

                foreach (var flag in SplitFlags(parsed, allLayers))
                {
                    if (!IsNone(flag))
                        flagsInGroup.Add(flag);
                }
            }

            var groupFlags = flagsInGroup.ToArray();

            foreach (var current in groupFlags)
            {
                ulong conflictMaskValue = 0;

                foreach (var t in groupFlags)
                {
                    conflictMaskValue |= ToUInt64(t);
                }

                var index = GetLayerIndex(current);
                var existing = ToUInt64(result[index]);
                result[index] = FromUInt64<TEnum>(existing | conflictMaskValue);
            }
        }

        return result;
    }

    private static TEnum ParseLayer<TEnum>(string name)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(name))
            return default;

        if (!Enum.TryParse<TEnum>(name, ignoreCase: false, out var value))
            throw new JsonSerializationException($"Unknown {typeof(TEnum).Name}: '{name}'");

        return value;
    }

    private static TEnum[] GetAllLayersExceptNone<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Where(x => !IsNone(x))
            .ToArray();
    }

    private static IEnumerable<TEnum> SplitFlags<TEnum>(TEnum value, TEnum[] allLayers)
        where TEnum : struct, Enum
    {
        var rawValue = ToUInt64(value);

        foreach (var layer in allLayers)
        {
            var rawLayer = ToUInt64(layer);
            if ((rawValue & rawLayer) != 0)
                yield return layer;
        }
    }

    private static int GetLayerIndex<TEnum>(TEnum layer)
        where TEnum : struct, Enum
    {
        var raw = ToUInt64(layer);
        return BitOperations.TrailingZeroCount(raw);
    }

    private static bool IsNone<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return ToUInt64(value) == 0;
    }

    private static ulong ToUInt64<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return Convert.ToUInt64(value);
    }

    private static TEnum FromUInt64<TEnum>(ulong value)
        where TEnum : struct, Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), value);
    }
}

public sealed class LayerCollision
{
    [JsonProperty("Cells")]
    public List<List<string>> Cells { get; set; } = [];

    [JsonProperty("Edges")]
    public List<List<string>> Edges { get; set; } = [];
}

public sealed class CollisionMasks
{
    public CellOccupancyLayer[] CellLayerMask { get; init; } = [];
    public EdgeOccupancyLayer[] EdgeLayerMask { get; init; } = [];
}
