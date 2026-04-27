using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 建筑回溯组件——记录被编辑建筑的原始状态，用于取消恢复。
/// </summary>
[Context(typeof(MapContext))]
public sealed class BuildingRollbackComponent : IComponent
{
    public Vector2Int OriginalGridPosition;
    public BuildingRotation OriginalRotation;
}
