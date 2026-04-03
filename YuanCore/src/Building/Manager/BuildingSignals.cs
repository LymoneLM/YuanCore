using System;

namespace YuanCore.Building;

public static class BuildingSignals
{
    public static event Action<string, int> OnSceneChanged;
    public static void InvokeSceneChanged(string sceneClass, int sceneIndex)
        => OnSceneChanged?.Invoke(sceneClass, sceneIndex);
}
