using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext)), Event(EventTarget.Self)]
public sealed class WorldPositionComponent : IComponent
{
    public Vector2 Value;
}
