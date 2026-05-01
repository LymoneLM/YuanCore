using System.Collections.Generic;
using Entitas;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// 非 ECS 输入管理器 —— 整合模式同步、光标更新、输入采集、Hover 决策、
/// EditCandidate 管理、Placement 跟随。
/// BuildingController.Update() 在 ECS Systems 之前调用，确保输入状态在响应式系统执行前已稳定。
/// </summary>
public sealed class BuildingInputManager
{
    private readonly MapContext _context;
    private readonly IGroup<Map.Entity> _placementGroup;
    private readonly List<Map.Entity> _placementBuffer = [];
    private readonly EditCandidateManager _editCandidateManager;
    private Camera _camera;
    private Vector2Int _lastCursorGrid;

    // ─── Hover ───
    private string _hoveredUid;

    public BuildingInputManager(MapContext context)
    {
        _context = context;
        _placementGroup = context.GetGroup(
            Matcher<Map.Entity>.AllOf(
                YuanCoreBuildingMapPlacementMatcher.Placement));
        _camera = Camera.main;

        _editCandidateManager = new EditCandidateManager(context);

        BuildingSignals.OnPointerEnterBuilding += OnPointerEnterBuilding;
        BuildingSignals.OnPointerExitBuilding += OnPointerExitBuilding;
    }

    /// <summary>Startup: 重置 CursorState。</summary>
    public void Initialize()
    {
        CursorState.Reset();
    }

    /// <summary>每帧更新入口：模式同步 → 光标 → 输入 → HoverInfo → Placement 跟随。</summary>
    public void Update()
    {
        SyncMode();

        if (!CursorState.Active)
            return;

        UpdateCursor();
        HandleInput();
        FollowPlacements();
    }

    public void Dispose()
    {
        BuildingSignals.OnPointerEnterBuilding -= OnPointerEnterBuilding;
        BuildingSignals.OnPointerExitBuilding -= OnPointerExitBuilding;
        _editCandidateManager.Dispose();
    }

    // ─── Mode Sync ───

    private void SyncMode()
    {
        MainloadCompat.PollVanillaMode();

        var mode = MainloadCompat.CurrentMode;
        CursorState.Active = mode == BuildMode.Build ||
                             mode == BuildMode.EditSelect ||
                             mode == BuildMode.EditMove;
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
        var mode = MainloadCompat.CurrentMode;

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

    private void HandleEscape(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Build:
                PlacementLifecycle.CancelAllSessionPlacements(_context);
                MainloadCompat.SetMode(BuildMode.Normal);
                MainloadCompat.SyncBuildPanelOpen(false);
                break;

            case BuildMode.EditMove:
                PlacementLifecycle.CancelAllSessionPlacements(_context);
                MainloadCompat.SetMode(BuildMode.EditSelect);
                break;

            case BuildMode.EditSelect:
                _editCandidateManager.Clear();
                MainloadCompat.SetMode(BuildMode.Normal);
                break;
        }
    }

    private void HandleRotation(BuildMode mode)
    {
        if (mode != BuildMode.Build && mode != BuildMode.EditMove)
            return;

        PlacementLifecycle.RotateSessionPlacements(_context);
    }

    private void HandleScroll(BuildMode mode, float scroll)
    {
        if (PointChecker.IsPointerOverUI()) return;

        if (mode == BuildMode.EditSelect)
        {
            _editCandidateManager.CycleSelection(scroll > 0 ? -1 : 1);
        }
    }

    private void HandleLeftClick(BuildMode mode)
    {
        if (PointChecker.IsPointerOverUI()) return;

        switch (mode)
        {
            case BuildMode.Build:
                PlacementLifecycle.TrySubmitBuild(_context);
                break;

            case BuildMode.EditSelect:
                TrySelectCandidate();
                break;

            case BuildMode.EditMove:
                PlacementLifecycle.TrySubmitEditMove(_context);
                break;
        }
    }

    private void TrySelectCandidate()
    {
        if (!_editCandidateManager.TryGetCurrentUid(out var uid)) return;
        PlacementLifecycle.BeginEditMove(_context, uid);
    }

    // ─── Placement Follow ───

    private void FollowPlacements()
    {
        var mode = MainloadCompat.CurrentMode;
        if (mode != BuildMode.Build && mode != BuildMode.EditMove)
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

    // ─── Hover ───

    private void OnPointerEnterBuilding(string uid)
    {
        if (MainloadCompat.CurrentMode != BuildMode.Normal)
            return;

        ClearCurrentHover();

        _hoveredUid = uid;
        SetBuildingHoverVisual(uid, true);
    }

    private void OnPointerExitBuilding(string uid)
    {
        if (_hoveredUid != uid) return;

        ClearCurrentHover();
    }

    private void ClearCurrentHover()
    {
        if (_hoveredUid == null) return;

        SetBuildingHoverVisual(_hoveredUid, false);
        _hoveredUid = null;
    }

    private void SetBuildingHoverVisual(string uid, bool on)
    {
        var entity = _context.GetBuildingByUid(uid);
        if (entity == null || !entity.HasView()) return;
        if (entity.GetView().View is BuildingShowView sv)
            sv.SetHoverVisual(on);
    }
}
