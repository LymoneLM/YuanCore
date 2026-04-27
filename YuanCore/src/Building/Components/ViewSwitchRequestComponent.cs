using Entitas;
using Entitas.Generators.Attributes;

namespace YuanCore.Building;

[Context(typeof(MapContext))]
public sealed class ViewSwitchRequestComponent : IComponent
{
    public bool ToPlacement;
}
