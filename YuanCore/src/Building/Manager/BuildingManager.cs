using UnityEngine;

namespace YuanCore.Building;

// 全局唯一
// 面相对象业务逻辑/建筑场景逻辑总入口：
// 处理场景变更
// 处理存档加载/存储

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    private BuildingController _buildingController;

    private string _sceneIDLast;
    private const string _defaultSceneID = "null|0";

    private void Awake()
    {
        Instance = this;
        _buildingController = BuildingController.Instance;

        _sceneIDLast = _defaultSceneID;
    }

    private void Start()
    {
        _buildingController.Initialize();
    }

    private void Update()
    {
        _buildingController.Execute();

        CheckUpdateScene();
    }

    private void CheckUpdateScene()
    {
        // 可能为冗余代码
        // if (Mainload.isClearNongZBuild)
        // {
        //     Mainload.isClearNongZBuild = false;
        //     DesOldScene();
        // }
        if (_sceneIDLast != Mainload.SceneID || Mainload.isUpdateScene)
        {
            Mainload.isCreatSceneFinish = false;
            Mainload.isSwichPanelOpen = true;

            if (!Mainload.isFirstGame)
            {
                Mainload.isSaveFinish = false;
                SaveData.SaveGameData(_sceneIDLast);
            }
            else
            {
                Mainload.isSaveFinish = true;
            }
            ResetSceneState();

            _sceneIDLast = Mainload.SceneID;
            Mainload.isUpdateScene = false;
        }
        if (Mainload.isSaveFinish)
        {
            Mainload.isSaveFinish = false;
            BuildingSignals.InvokeMapChanged(_sceneIDLast);
        }
    }


    private static void ResetSceneState()
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
    }
}
