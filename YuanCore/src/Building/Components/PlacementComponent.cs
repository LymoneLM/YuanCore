using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext)), Event(EventTarget.Self)]
public sealed class PlacementComponent : IComponent
{
    public Vector2Int Offset;
    public (Vector2Int, bool)[] Flags;
}
