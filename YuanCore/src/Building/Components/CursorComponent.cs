using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 常驻光标实体，标记 + 数据。
/// 仅在 Build / EditSelect / EditMove 模式下持续更新。
/// </summary>
[Context(typeof(MapContext)), Unique]
public sealed class CursorComponent : IComponent
{
    public Vector2 WorldPosition;
    public Vector2Int GridPosition;
    public bool Active;
}
