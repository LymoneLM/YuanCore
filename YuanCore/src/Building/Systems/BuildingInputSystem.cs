using Entitas;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// 统一的建筑输入采集系统。
/// 根据当前模式处理滚轮、左键、旋转键、ESC。
/// </summary>
public sealed class BuildingInputSystem : IExecuteSystem
{
    private readonly MapContext _context;

    public BuildingInputSystem(MapContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        var mode = BuildingModeManager.CurrentMode;

        // ESC 退出
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape(mode);
            return;
        }

        // 旋转 (R 键)
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleRotation(mode);
        }

        // 滚轮
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            HandleScroll(mode, scroll);
        }

        // 左键点击
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick(mode);
        }
    }

    // ─── ESC ───

    private void HandleEscape(BuildingInteractionMode mode)
    {
        switch (mode)
        {
            case BuildingInteractionMode.Build:
                PlacementLifecycle.CancelAllSessionPlacements(_context);
                BuildingModeManager.SetMode(BuildingInteractionMode.Normal);
                MainloadCompatibility.SyncBuildPanelOpen(false);
                break;

            case BuildingInteractionMode.EditMove:
                PlacementLifecycle.CancelAllSessionPlacements(_context);
                BuildingModeManager.SetMode(BuildingInteractionMode.EditSelect);
                break;

            case BuildingInteractionMode.EditSelect:
                BuildingModeManager.SetMode(BuildingInteractionMode.Normal);
                break;
        }
    }

    // ─── 旋转 ───

    private void HandleRotation(BuildingInteractionMode mode)
    {
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        PlacementLifecycle.RotateSessionPlacements(_context);
    }

    // ─── 滚轮 ───

    private void HandleScroll(BuildingInteractionMode mode, float scroll)
    {
        if (PointChecker.IsPointerOverUI()) return;

        if (mode == BuildingInteractionMode.EditSelect)
        {
            // 切换候选建筑
            if (!_context.HasEditCandidates()) return;
            var ec = _context.GetEditCandidates();
            if (ec.Candidates == null || ec.Candidates.Length <= 1) return;

            var delta = scroll > 0 ? -1 : 1;
            var newIndex = (ec.CurrentIndex + delta + ec.Candidates.Length) % ec.Candidates.Length;
            _context.ReplaceEditCandidates(ec.Candidates, newIndex);
        }
        // 其它模式下滚轮行为由相机系统处理，不在此拦截
    }

    // ─── 左键 ───

    private void HandleLeftClick(BuildingInteractionMode mode)
    {
        if (PointChecker.IsPointerOverUI()) return;

        switch (mode)
        {
            case BuildingInteractionMode.Build:
                // 确认 Placement 落位
                PlacementLifecycle.TrySubmitBuild(_context);
                break;

            case BuildingInteractionMode.EditSelect:
                // 选中当前候选建筑，进入 EditMove
                TrySelectCandidate();
                break;

            case BuildingInteractionMode.EditMove:
                // 确认编辑移动落位
                PlacementLifecycle.TrySubmitEditMove(_context);
                break;

            // Normal 模式的点击由 OpenBT -> BuildingShowView 处理，
            // 挂 Clicked 组件后交给 ClickProcessSystem 消费
        }
    }

    private void TrySelectCandidate()
    {
        if (!_context.HasEditCandidates()) return;
        var ec = _context.GetEditCandidates();
        if (ec.Candidates == null || ec.Candidates.Length == 0) return;
        if (ec.CurrentIndex < 0 || ec.CurrentIndex >= ec.Candidates.Length) return;

        var uid = ec.Candidates[ec.CurrentIndex];
        PlacementLifecycle.BeginEditMove(_context, uid);
    }
}
