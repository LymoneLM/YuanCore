using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingStateComponent : IComponent
{
    public int ID;
    public BuildingRotation Rotation;
}
