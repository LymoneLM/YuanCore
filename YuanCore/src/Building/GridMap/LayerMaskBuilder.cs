using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace YuanCore.Building;

public static class LayerMaskBuilder
{
    public static (CellOccupancyLayer[], EdgeOccupancyLayer[]) LoadFromJson(string json)
    {
        var config = JsonConvert.DeserializeObject<LayerCollision>(json)
                     ?? throw new InvalidOperationException("Failed to deserialize collision config.");

        return (
            BuildMasks<CellOccupancyLayer>(config.Cells),
            BuildMasks<EdgeOccupancyLayer>(config.Edges)
        );
    }

    private static TEnum[] BuildMasks<TEnum>(List<List<string>> rules)
        where TEnum : struct, Enum
    {
        var result = new TEnum[GetMaskArrayLength<TEnum>()];

        if (rules == null)
            return result;

        foreach (var rule in rules)
        {
            if (rule is not { Count: 2 })
                throw new JsonSerializationException(
                    $"{typeof(TEnum).Name} collision rule must contain exactly 2 layer names.");

            var left = ParseLayer<TEnum>(rule[0]);
            var right = ParseLayer<TEnum>(rule[1]);

            if (IsNone(left) || IsNone(right))
                throw new JsonSerializationException(
                    $"{typeof(TEnum).Name} collision rule cannot use None.");

            AddCollision(result, left, right);
            AddCollision(result, right, left);
        }

        return result;
    }

    private static void AddCollision<TEnum>(TEnum[] masks, TEnum source, TEnum target)
        where TEnum : struct, Enum
    {
        var index = GetLayerIndex(source);
        var existing = ToUInt64(masks[index]);
        var updated = existing | ToUInt64(target);
        masks[index] = FromUInt64<TEnum>(updated);
    }

    private static TEnum ParseLayer<TEnum>(string name)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new JsonSerializationException($"Empty {typeof(TEnum).Name} name.");

        if (!Enum.TryParse<TEnum>(name, ignoreCase: false, out var value))
            throw new JsonSerializationException($"Unknown {typeof(TEnum).Name}: '{name}'");

        return value;
    }

    private static int GetMaskArrayLength<TEnum>()
        where TEnum : struct, Enum
    {
        return GetLayerIndex(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Max()) + 1;
    }

    private static int GetLayerIndex<TEnum>(TEnum layer)
        where TEnum : struct, Enum
    {
        return BitOperations.TrailingZeroCount64(ToUInt64(layer));
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
