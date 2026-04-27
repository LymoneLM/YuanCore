using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 编辑模式下当前候选建筑列表（挂在 Unique 编辑状态实体上）。
/// </summary>
[Context(typeof(MapContext)), Unique]
public sealed class EditCandidatesComponent : IComponent
{
    /// <summary>已按稳定顺序排列的候选建筑 UID 列表。</summary>
    public string[] Candidates;

    /// <summary>当前选中的候选索引。</summary>
    public int CurrentIndex;
}

/// <summary>
/// 编辑移动会话——记录被编辑建筑的原始状态，用于取消恢复。
/// </summary>
[Context(typeof(MapContext))]
public sealed class EditMoveSessionComponent : IComponent
{
    public string Uid;
    public Vector2Int OriginalGridPosition;
    public BuildingRotation OriginalRotation;
    public bool OccupancyRemoved;
}

/// <summary>
/// 建造会话组件——标记一个 Placement 属于哪个建造/编辑会话。
/// </summary>
[Context(typeof(MapContext))]
public sealed class PlacementSessionComponent : IComponent
{
    /// <summary>会话 ID，同一次建造/编辑操作共享同一个 SessionId。</summary>
    public int SessionId;
}


