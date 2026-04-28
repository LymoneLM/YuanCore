using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// 为原版 UI / 面板提供的建造入口 API。
/// 原版 BuildPanel 等脚本通过此类进入建造模式。
/// </summary>
/// TODO: 整个类未被调用——需要接入原版面板按钮回调。
///       原版建造按钮 → EnterBuildMode(buildingID, taoZhuangID, rotation)
///       原版编辑按钮 → EnterEditMode()
///       原版退出按钮 / Escape → ExitToNormal()
///       当前实际入口是 BuildingModeManager.PollVanillaMode() 轮询，
///       但该路径缺少 BeginNewPlacement 调用（见 PollVanillaMode）
public static class BuildingBuildEntry
{
    public static void EnterBuildMode(int buildingID, int taoZhuangID,
        BuildingRotation rotation = BuildingRotation.R0)
    {
        if (BuildingModeManager.CurrentMode != BuildingInteractionMode.Normal)
        {
            YuanCorePlugin.Logger.LogWarning(
                $"[BuildEntry] Cannot enter Build mode from {BuildingModeManager.CurrentMode}");
            return;
        }

        BuildingModeManager.SetMode(BuildingInteractionMode.Build);

        // 创建 Placement 实体
        PlacementLifecycle.BeginNewPlacement(
            MapContext.Instance, buildingID, taoZhuangID, rotation);
    }

    /// <summary>
    /// 从原版面板进入编辑模式。
    /// </summary>
    public static void EnterEditMode()
    {
        if (BuildingModeManager.CurrentMode != BuildingInteractionMode.Normal)
        {
            YuanCorePlugin.Logger.LogWarning(
                $"[BuildEntry] Cannot enter Edit mode from {BuildingModeManager.CurrentMode}");
            return;
        }

        BuildingModeManager.SetMode(BuildingInteractionMode.EditSelect);
    }

    /// <summary>
    /// 退出当前建筑交互模式，回到 Normal。
    /// 自动处理 Placement 取消逻辑。
    /// </summary>
    public static void ExitToNormal()
    {
        var mode = BuildingModeManager.CurrentMode;
        if (mode == BuildingInteractionMode.Normal) return;

        if (mode == BuildingInteractionMode.Build || mode == BuildingInteractionMode.EditMove)
        {
            PlacementLifecycle.CancelAllSessionPlacements(MapContext.Instance);
        }

        BuildingModeManager.SetMode(BuildingInteractionMode.Normal);
        MainloadCompat.SyncBuildPanelOpen(false);
    }
}
