using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext)), Event(EventTarget.Self)]
public sealed class GridPositionComponent : IComponent
{
    public Vector2Int Value;
}
