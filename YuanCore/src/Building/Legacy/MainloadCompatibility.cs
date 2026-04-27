using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

public class MainloadCompatibility : MonoBehaviour
{
    public static string SceneID => Mainload.SceneID;
    public static bool IsSceneCreated => Mainload.isCreatSceneFinish;
    public static bool IsBuildPanelOpen => Mainload.isBuildPanelOpen;
    public static bool IsBuildMode => Mainload.isBuildMode;
    public static bool IsEditMode => Mainload.isBuildEdit;
    public static bool IsFertilizationMode => Mainload.isShiFeiMode;

    /// 当前待建建筑 ID（原版面板选中的建筑）。
    public static string BuildIDCreatNow => Mainload.BuildID_CreatNow;

    /// 当前建筑位置 ID。
    public static string BuildPosiIDNow => Mainload.BuildPosiID_Now;

    /// 原版编辑模式下选中的建筑实例 ID。
    public static string EditBuildShiliID => Mainload.EditBuildShiliID;

    private void Update()
    {

    }

    public static void SyncModeToVanilla(BuildingInteractionMode mode)
    {
        switch (mode)
        {
            case BuildingInteractionMode.Normal:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = false;
                Mainload.EditBuildShiliID = "null";
                break;

            case BuildingInteractionMode.Build:
                Mainload.isBuildMode = true;
                Mainload.isBuildEdit = false;
                Mainload.EditBuildShiliID = "null";
                break;

            case BuildingInteractionMode.EditSelect:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = true;
                break;

            case BuildingInteractionMode.EditMove:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = true;
                break;
        }
    }

    /// 回写当前 hover 建筑信息，供原版 BuildInfoTip 等 UI 使用。
    public static void SyncHoverBuilding(int buildingID, string uid)
    {
        if (buildingID >= 0)
        {
            Mainload.BuildID_IsYour_Enter[0] = buildingID;
            // Mainload.BuildID_IsYour_Enter 可能有更多字段，这里只写必要的
        }
        else
        {
            Mainload.BuildID_IsYour_Enter[0] = -1;
        }
    }

    /// 回写编辑选中的建筑实例 ID。
    public static void SyncEditTarget(string uid)
        => Mainload.EditBuildShiliID = uid ?? "null";

    /// 回写是非模式标志。
    public static void SyncShiFeiMode(bool value)
        => Mainload.isShiFeiMode = value;

    /// 回写建造面板打开状态。
    public static void SyncBuildPanelOpen(bool value)
        => Mainload.isBuildPanelOpen = value;
}
