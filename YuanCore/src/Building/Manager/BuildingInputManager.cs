using System.Collections.Generic;
using Entitas;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// 非 ECS 输入管理器 —— 整合模式同步、光标更新、输入采集、Placement 跟随。
/// BuildingController.Update() 在 ECS Systems 之前调用，确保输入状态在响应式系统执行前已稳定。
/// </summary>
public sealed class BuildingInputManager
{
    private readonly MapContext _context;
    private readonly IGroup<Map.Entity> _placementGroup;
    private readonly List<Map.Entity> _placementBuffer = [];
    private Camera _camera;
    private Vector2Int _lastCursorGrid;

    public BuildingInputManager(MapContext context)
    {
        _context = context;
        _placementGroup = context.GetGroup(
            Matcher<Map.Entity>.AllOf(
                YuanCoreBuildingMapPlacementMatcher.Placement));
        _camera = Camera.main;
    }

    /// <summary>Startup: 重置 CursorState。</summary>
    public void Initialize()
    {
        CursorState.Reset();
    }

    /// <summary>每帧更新入口：模式同步 → 光标 → 输入 → Placement 跟随。</summary>
    public void Update()
    {
        SyncMode();

        if (!CursorState.Active)
            return;

        UpdateCursor();
        HandleInput();
        FollowPlacements();
    }

    // ─── Mode Sync ───

    private void SyncMode()
    {
        BuildingModeManager.PollVanillaMode();

        var mode = BuildingModeManager.CurrentMode;
        CursorState.Active = mode == BuildingInteractionMode.Build ||
                             mode == BuildingInteractionMode.EditSelect ||
                             mode == BuildingInteractionMode.EditMove;
    }

    // ─── Cursor Update ───

    private void UpdateCursor()
    {
        var cam = _camera;
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
            _camera = cam;
        }

        var mouseScreen = Input.mousePosition;
        var worldPos = (Vector2)cam.ScreenToWorldPoint(mouseScreen);
        var gridPos = PositionConvertor.WorldToGrid(worldPos);

        CursorState.SetGridPosition(gridPos);
    }

    // ─── Input Handling ───

    private void HandleInput()
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
            HandleRotation(mode);

        // 滚轮
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            HandleScroll(mode, scroll);

        // 左键点击
        if (Input.GetMouseButtonDown(0))
            HandleLeftClick(mode);
    }

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

    private void HandleRotation(BuildingInteractionMode mode)
    {
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        PlacementLifecycle.RotateSessionPlacements(_context);
    }

    private void HandleScroll(BuildingInteractionMode mode, float scroll)
    {
        if (PointChecker.IsPointerOverUI()) return;

        if (mode == BuildingInteractionMode.EditSelect)
        {
            EditCandidateManager.CycleSelection(_context, scroll > 0 ? -1 : 1);
        }
    }

    private void HandleLeftClick(BuildingInteractionMode mode)
    {
        if (PointChecker.IsPointerOverUI()) return;

        switch (mode)
        {
            case BuildingInteractionMode.Build:
                PlacementLifecycle.TrySubmitBuild(_context);
                break;

            case BuildingInteractionMode.EditSelect:
                TrySelectCandidate();
                break;

            case BuildingInteractionMode.EditMove:
                PlacementLifecycle.TrySubmitEditMove(_context);
                break;
        }
    }

    private void TrySelectCandidate()
    {
        if (!EditCandidateManager.TryGetCurrentUid(out var uid)) return;
        PlacementLifecycle.BeginEditMove(_context, uid);
    }

    // ─── Placement Follow ───

    private void FollowPlacements()
    {
        var mode = BuildingModeManager.CurrentMode;
        if (mode != BuildingInteractionMode.Build && mode != BuildingInteractionMode.EditMove)
            return;

        if (CursorState.GridPosition == _lastCursorGrid) return;
        _lastCursorGrid = CursorState.GridPosition;

        _placementBuffer.Clear();
        _placementBuffer.AddRange(_placementGroup.GetEntities());

        foreach (var entity in _placementBuffer)
        {
            var offset = entity.GetPlacement().Offset;
            entity.ReplaceGridPosition(CursorState.GridPosition + offset);
        }
    }
}
