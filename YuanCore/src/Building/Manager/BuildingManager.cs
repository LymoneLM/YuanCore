using System;
using UnityEngine;

namespace YuanCore.Building;

/// 建筑场景逻辑总入口
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    public static BuildingStates States;

    internal Transform BuildingViewRoot;
    internal AllBuildEx SceneRoot;
    internal string CurrentSceneID;

    private void Awake()
    {
        Instance = this;
        States = new BuildingStates();
    }

    private void OnEnable()
    {
        BuildingSignals.OnSceneChanged += OnSceneChanged;
    }

    private void OnDisable()
    {
        BuildingSignals.OnSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(string sceneID)
    {
        MainloadCompat.StartSceneLoad();
        CurrentSceneID = sceneID;

        if(!MainloadCompat.IsFirstGame)
            // TODO: 切换为Core接管的存档管理
            SaveData.SaveGameData(sceneID);

        MainloadCompat.ResetSceneState();
        MainloadCompat.ResetMode();

        States.InitializeMap(sceneID);
        SceneRoot.SwitchScene(sceneID, (buildingViewRoot) =>
        {
            BuildingViewRoot = buildingViewRoot;

            MapContext.Instance.DestroyAllEntities();
            BuildingSceneBootstrap.Run(sceneID);

            MainloadCompat.FinishSceneLoad();
        });
    }
}
