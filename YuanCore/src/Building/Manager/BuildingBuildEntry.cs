using YuanCore.Core;

namespace YuanCore.Building;

/// <summary>
/// 为原版 UI / 面板提供的建造入口 API。
/// 原版 BuildPanel 等脚本通过此类进入建造模式，
/// </summary>
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
        MainloadCompatibility.SyncBuildPanelOpen(false);
    }
}
