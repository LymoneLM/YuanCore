using System;
using UnityEngine;

namespace YuanCore.Building;

public static class BuildingSignals
{
    public static event Action<string> OnSceneChanged;
    public static void InvokeSceneChanged(string sceneID)
        => OnSceneChanged?.Invoke(sceneID);

    /// TODO: 此信号目前无人订阅。ClickProcessSystem 在 Normal 模式下点击建筑时触发，
    ///       但全代码库无任何监听者。需要接入原版建筑信息面板。
    public static event Action<string, int> OnBuildingClicked;
    public static void InvokeBuildingClicked(string uid, int buildingID)
        => OnBuildingClicked?.Invoke(uid, buildingID);
}
