using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

[Context(typeof(MapContext))]
public sealed class BuildingStateComponent : IComponent
{
    public int TaoZhuangID;
    public BuildingRotation Rotation;
    public bool IsRuined;

    public int VanillaStateID => IsRuined ? -(int)Rotation-1 : (int)Rotation;
}
