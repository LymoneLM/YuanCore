using YuanCore.Core;

namespace YuanCore.Building;

public static class MainloadCompat
{
    // ── 模式管理 ──

    public static BuildMode CurrentMode { get; private set; } = BuildMode.Normal;

    public static void SetMode(BuildMode newMode)
    {
        if (CurrentMode == newMode) return;
        var oldMode = CurrentMode;
        CurrentMode = newMode;
        SyncModeToVanilla(newMode);
        YuanCorePlugin.Logger.LogDebug($"[BuildingMode] {oldMode} -> {newMode}");
    }

    /// <summary>
    /// 每帧检测原版是否从外部触发了模式变更（仅在 Normal 模式下）。
    /// </summary>
    public static void PollVanillaMode()
    {
        if (CurrentMode != BuildMode.Normal) return;

        if (IsBuildMode && BuildIDCreatNow != "null")
        {
            // TODO: 需要从原版字段获取 buildingID/taoZhuangID/rotation，
            //       并调用 PlacementLifecycle.BeginNewPlacement() 创建 Placement 实体。
            SetMode(BuildMode.Build);
            return;
        }

        if (IsEditMode)
        {
            SetMode(BuildMode.EditSelect);
        }
    }

    public static void ResetMode()
    {
        CurrentMode = BuildMode.Normal;
    }

    // ── 原版状态桥接 ──

    public static bool IsFirstGame => Mainload.isFirstGame;
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

    public static void SyncModeToVanilla(BuildMode mode)
    {
        switch (mode)
        {
            case BuildMode.Normal:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = false;
                Mainload.EditBuildShiliID = "null";
                break;

            case BuildMode.Build:
                Mainload.isBuildMode = true;
                Mainload.isBuildEdit = false;
                Mainload.EditBuildShiliID = "null";
                break;

            case BuildMode.EditSelect:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = true;
                break;

            case BuildMode.EditMove:
                Mainload.isBuildMode = false;
                Mainload.isBuildEdit = true;
                break;
        }
    }

    /// 回写当前 hover 建筑信息，供原版 BuildInfoTip 等 UI 使用。
    /// TODO: 此方法从未被调用。需要接入 EditCandidateManager 的悬停回调。
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

    /// 回写施肥模式标志。
    /// TODO: 此方法从未被调用。需要接入施肥模式切换。
    public static void SyncShiFeiMode(bool value)
        => Mainload.isShiFeiMode = value;

    /// 回写建造面板打开状态。
    public static void SyncBuildPanelOpen(bool value)
        => Mainload.isBuildPanelOpen = value;

    internal static void Update()
    {
        CheckSceneChange();
    }

    private static string _sceneIDLast = "null|0";

    public static void CheckSceneChange()
    {
        if (_sceneIDLast == Mainload.SceneID && !Mainload.isUpdateScene)
            return;
        _sceneIDLast = Mainload.SceneID;
        Mainload.isUpdateScene = false;

        BuildingSignals.InvokeSceneChanged(_sceneIDLast);
    }

    public static void ResetSceneState()
    {
        Mainload.MemberData_Enter = "null";
        Mainload.BuildID_IsYour_Enter[0] = -1;
        Mainload.TradeSR_index = 0;
        Mainload.TradeData_now = "null";
        Mainload.isShiFeiMode = false;
        Mainload.isBuildPanelOpen = false;
        Mainload.isBuildMode = false;
        Mainload.isBuildEdit = false;
        Mainload.EditBuildShiliID = "null";
        Mainload.KingMemberID_OutBuild = [];
        Mainload.HanMen_City = [];
        Mainload.ClanMember_City = [];

        // 来自建筑加载头部
        Mainload.TempMemberIndex_now = 0;
        Mainload.BuildPosiID_Now = "0|0";
        Mainload.BuildID_CreatNow = "null";
    }

    public static void StartSceneLoad()
    {
        Mainload.isCreatSceneFinish = false;
        Mainload.isSwichPanelOpen = true;
    }

    public static void FinishSceneLoad()
    {
        Mainload.isCreatSceneFinish = true;
        Mainload.isSwichPanelOpen = false;
    }
}

public enum BuildMode
{
    Normal = 0,
    Build = 1,
    EditSelect = 2,
    EditMove = 3,
}
