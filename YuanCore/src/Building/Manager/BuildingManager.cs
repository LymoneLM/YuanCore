using System;
using UnityEngine;

namespace YuanCore.Building;

// 全局唯一
// 面相对象业务逻辑/建筑场景逻辑总入口：
// 处理场景变更
// 处理存档加载/存储

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public BuildingStates States;

    private string _sceneIDLast;
    private const string _defaultSceneID = "null|0";

    private GameObject _buildShow;

    private void Awake()
    {
        Instance = this;
        States = BuildingStates.Instance;

        // Current Class
        _sceneIDLast = _defaultSceneID;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Start()
    {
        _controller.Initialize();
    }

    private void Update()
    {
        CheckUpdateScene();
    }

    private void CheckUpdateScene()
    {
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
            ChangeScene();
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

    private void ChangeScene()
    {
        MapContext.Instance.DestroyAllEntities();
        var (sceneClass, sceneIndex)= ParseSceneID(_sceneIDLast);
        BuildingSignals.InvokeSceneChanged(sceneClass, sceneIndex);
        BuildingState.Instance.InitializeMap(sceneClass, sceneIndex);
    }

    private static (string SceneClass, int SceneIndex) ParseSceneID(string sceneID)
    {
        var parts = sceneID.Split('|');
        var sceneClass = parts[0];
        var index = int.Parse(parts[1]);
        var index2 = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        var sceneIndex = sceneClass switch
        {
            "M" => int.Parse(Mainload.Fudi_now[index][37]),
            "Z" => int.Parse(Mainload.NongZ_now[index][index2][5]),
            "S" => index,
            "F" => -1,
            "H" => index,
            "L" => int.Parse(Mainload.Mudi_now[index][index2][2]),
            _ => throw new ArgumentOutOfRangeException()
        };
        return (sceneClass, sceneIndex);
    }
}
