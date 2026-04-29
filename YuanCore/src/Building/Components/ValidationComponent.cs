using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext)), Event(EventTarget.Self)]
public sealed class ValidationComponent : IComponent
{
    public (Vector2Int, bool)[] Flags;
    public bool CanPlace
    {
        get
        {
            var result = true;
            foreach (var (_, flag) in Flags)
                result &= flag;
            return result;
        }
    }
}
