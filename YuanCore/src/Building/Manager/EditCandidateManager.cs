using System;
using UnityEngine;

namespace YuanCore.Building;

/// <summary>
/// 编辑候选建筑管理器——纯静态方案，替代 EditCandidatesComponent。
/// 候选列表由 EditCandidateRefreshSystem 计算并通过 SetCandidates 注入，
/// 高亮作为同步副作用自动应用。
/// </summary>
public static class EditCandidateManager
{
    // ── State ──
    public static string[] Candidates { get; private set; } = Array.Empty<string>();
    public static int CurrentIndex { get; private set; }

    // ── Computed ──
    public static bool HasCandidates => Candidates is { Length: > 0 };

    public static string CurrentUid
    {
        get
        {
            if (!HasCandidates) return null;
            if (CurrentIndex < 0 || CurrentIndex >= Candidates.Length) return null;
            return Candidates[CurrentIndex];
        }
    }

    // ── Highlight tracking ──
    private static string _lastHighlightUid;

    // ── Public Operations ──

    /// <summary>
    /// 设置候选列表。自动保持已选中建筑（通过 UID 匹配）。
    /// 若之前选中的 UID 不在新列表中，回退到索引 0。
    /// </summary>
    public static void SetCandidates(MapContext ctx, string[] candidates)
    {
        if (candidates == null)
            candidates = Array.Empty<string>();

        // 尽量保持选中
        var newIndex = 0;
        var oldUid = CurrentUid;
        if (oldUid != null)
        {
            for (var i = 0; i < candidates.Length; i++)
            {
                if (candidates[i] == oldUid)
                {
                    newIndex = i;
                    break;
                }
            }
        }

        Candidates = candidates;
        CurrentIndex = candidates.Length > 0 ? Mathf.Clamp(newIndex, 0, candidates.Length - 1) : 0;

        ApplyHighlight(ctx);
    }

    /// <summary>
    /// 循环切换选中项。direction > 0 为下一项，< 0 为上一项。
    /// </summary>
    public static void CycleSelection(MapContext ctx, int direction)
    {
        if (!HasCandidates) return;
        if (Candidates.Length <= 1) return;

        var delta = direction > 0 ? 1 : -1;
        CurrentIndex = (CurrentIndex + delta + Candidates.Length) % Candidates.Length;

        ApplyHighlight(ctx);
    }

    /// <summary>
    /// 尝试获取当前选中的建筑 UID。
    /// </summary>
    public static bool TryGetCurrentUid(out string uid)
    {
        uid = CurrentUid;
        return uid != null;
    }

    /// <summary>
    /// 清除全部候选和高亮（离开 EditSelect 模式时调用）。
    /// </summary>
    public static void Clear(MapContext ctx)
    {
        ClearHighlight(ctx);
        Candidates = Array.Empty<string>();
        CurrentIndex = 0;
    }

    /// <summary>
    /// 重置所有状态（场景切换时调用），不依赖 MapContext。
    /// </summary>
    public static void Reset()
    {
        _lastHighlightUid = null;
        Candidates = Array.Empty<string>();
        CurrentIndex = 0;
    }

    // ── Internal Highlight ──

    private static void ApplyHighlight(MapContext ctx)
    {
        ClearHighlight(ctx);

        var uid = CurrentUid;
        if (uid == null) return;

        var entity = ctx.GetBuildingByUid(uid);
        if (entity == null || !entity.HasView()) return;
        if (entity.GetView().View is not BuildingShowView showView) return;

        showView.SetEditHighlight(true);
        _lastHighlightUid = uid;
    }

    private static void ClearHighlight(MapContext ctx)
    {
        if (_lastHighlightUid == null) return;

        var entity = ctx.GetBuildingByUid(_lastHighlightUid);
        if (entity != null && entity.HasView() &&
            entity.GetView().View is BuildingShowView sv)
        {
            sv.SetEditHighlight(false);
        }

        _lastHighlightUid = null;
    }
}
