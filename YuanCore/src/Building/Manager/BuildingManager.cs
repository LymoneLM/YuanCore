using System;
using UnityEngine;

namespace YuanCore.Building;

/// 建筑场景逻辑总入口
public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    internal Transform BuildingViewRoot;
    internal AllBuildEx SceneRoot;

    private void Awake()
    {
        Instance = this;
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

        if(!MainloadCompat.IsFirstGame)
            // TODO: 切换为Core接管的存档管理
            SaveData.SaveGameData(sceneID);

        MainloadCompat.ResetSceneState();
        BuildingModeManager.Reset();

        var (sceneClass, sceneIndex) = SceneIDResolver.GetSceneType(sceneID);
        BuildingStates.Instance.InitializeMap(sceneClass, sceneIndex);
        SceneRoot.SwitchScene(sceneID, (buildingViewRoot) =>
        {
            BuildingViewRoot = buildingViewRoot;

            MapContext.Instance.DestroyAllEntities();
            BuildingSceneBootstrap.Run(sceneID);

            MainloadCompat.FinishSceneLoad();
        });
    }

    private void Update()
    {
        MainloadCompat.Update();
    }
}
