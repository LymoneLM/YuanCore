using Entitas;
using Entitas.Generators.Attributes;

namespace YuanCore.Building;

[Context(typeof(MapContext))]
public sealed class BuildingComponent : IComponent
{
    public string Uid;
}
