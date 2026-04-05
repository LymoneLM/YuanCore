using System;
using UnityEngine;

namespace YuanCore.Building;

public static class BuildingSignals
{
    public static event Action<string, int> OnSceneChanged;
    public static void InvokeSceneChanged(string sceneClass, int sceneIndex)
        => OnSceneChanged?.Invoke(sceneClass, sceneIndex);

    public static event Action<GameObject> OnSceneCreated;
    public static void InvokeSceneCreated(GameObject buildShow)
        => OnSceneCreated?.Invoke(buildShow);
}
