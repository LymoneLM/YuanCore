using System;

namespace YuanCore.Building;

public static class BuildingSignals
{
    public static event Action<string> OnMapChanged;
    public static void InvokeMapChanged(string sceneID)
        => OnMapChanged?.Invoke(sceneID);
}
