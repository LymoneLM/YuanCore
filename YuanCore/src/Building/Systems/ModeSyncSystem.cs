using Entitas;

namespace YuanCore.Building;

/// <summary>
/// 每帧检测原版 Mainload 是否从外部触发了模式变更，
/// 并维护 CursorState 的 Active 状态。
/// </summary>
public sealed class ModeSyncSystem : IInitializeSystem, IExecuteSystem
{
    public void Initialize()
    {
        CursorState.Reset();
    }

    public void Execute()
    {
        BuildingModeManager.PollVanillaMode();

        var mode = BuildingModeManager.CurrentMode;
        CursorState.Active = mode == BuildingInteractionMode.Build ||
                             mode == BuildingInteractionMode.EditSelect ||
                             mode == BuildingInteractionMode.EditMove;
    }
}
