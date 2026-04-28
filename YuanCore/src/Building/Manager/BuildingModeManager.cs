using YuanCore.Core;

namespace YuanCore.Building;

public static class BuildingModeManager
{
    private static BuildingInteractionMode _currentMode = BuildingInteractionMode.Normal;

    public static BuildingInteractionMode CurrentMode => _currentMode;

    public static void SetMode(BuildingInteractionMode newMode)
    {
        if (_currentMode == newMode) return;
        var oldMode = _currentMode;
        _currentMode = newMode;

        // 同步兼容层
        MainloadCompat.SyncModeToVanilla(newMode);

        YuanCorePlugin.Logger.LogDebug($"[BuildingMode] {oldMode} -> {newMode}");
    }

    /// <summary>
    /// 由兼容层同步系统每帧调用，检测原版是否从外部触发了模式变更。
    /// </summary>
    public static void PollVanillaMode()
    {
        // 仅在 Normal 模式下检测外部触发
        if (_currentMode != BuildingInteractionMode.Normal) return;

        if (MainloadCompat.IsBuildMode &&
            MainloadCompat.BuildIDCreatNow != "null")
        {
            // TODO: 需要从原版字段获取 buildingID/taoZhuangID/rotation，
            //       并调用 PlacementLifecycle.BeginNewPlacement() 创建 Placement 实体。
            //       当前只切换模式，无 Placement 实体，FollowPlacements 空转。
            //       理想情况：此路径应改为由原版面板按钮直接调用 BuildingBuildEntry.EnterBuildMode()。
            SetMode(BuildingInteractionMode.Build);
            return;
        }

        if (MainloadCompat.IsEditMode)
        {
            SetMode(BuildingInteractionMode.EditSelect);
        }
    }

    /// <summary>
    /// 重置到 Normal（场景切换时调用）。
    /// </summary>
    public static void Reset()
    {
        _currentMode = BuildingInteractionMode.Normal;
    }
}

public enum BuildingInteractionMode
{
    Normal = 0,
    Build = 1,
    EditSelect = 2,
    EditMove = 3,
}
