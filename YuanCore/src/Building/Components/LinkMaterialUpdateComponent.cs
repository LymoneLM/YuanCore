using Entitas;
using Entitas.Generators.Attributes;

namespace YuanCore.Building;

[Context(typeof(MapContext)), Event(EventTarget.Self)]
public sealed class LinkMaterialUpdateComponent : IComponent
{
    public int DiffuseLevel;
}
