using System;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

public static class BuildingModeManager
{
    private static BuildingInteractionMode _currentMode = BuildingInteractionMode.Normal;
    private static int _nextSessionId = 1;

    public static BuildingInteractionMode CurrentMode => _currentMode;
    public static int CurrentSessionId { get; private set; }

    public static event Action<BuildingInteractionMode, BuildingInteractionMode> OnModeChanged;

    public static void SetMode(BuildingInteractionMode newMode)
    {
        if (_currentMode == newMode) return;
        var oldMode = _currentMode;
        _currentMode = newMode;

        // 同步兼容层
        MainloadCompatibility.SyncModeToVanilla(newMode);

        OnModeChanged?.Invoke(oldMode, newMode);

        YuanCorePlugin.Logger.LogDebug($"[BuildingMode] {oldMode} -> {newMode}");
    }

    public static int AllocateSession()
    {
        CurrentSessionId = _nextSessionId++;
        return CurrentSessionId;
    }

    /// <summary>
    /// 由兼容层同步系统每帧调用，检测原版是否从外部触发了模式变更。
    /// </summary>
    public static void PollVanillaMode()
    {
        // 仅在 Normal 模式下检测外部触发
        if (_currentMode != BuildingInteractionMode.Normal) return;

        if (MainloadCompatibility.IsBuildMode &&
            MainloadCompatibility.BuildIDCreatNow != "null")
        {
            SetMode(BuildingInteractionMode.Build);
            return;
        }

        if (MainloadCompatibility.IsEditMode)
        {
            SetMode(BuildingInteractionMode.EditSelect);
        }
    }

    /// <summary>
    /// 重置到 Normal（场景切换时调用）。
    /// </summary>
    public static void Reset()
    {
        _currentMode = BuildingInteractionMode.Normal;
        CurrentSessionId = 0;
    }
}

public enum BuildingInteractionMode
{
    Normal = 0,
    Build = 1,
    EditSelect = 2,
    EditMove = 3,
}
