using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext))]
public sealed class PlacementComponent : IComponent
{
    public Vector2Int Offset;
}
