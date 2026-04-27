using Entitas.Unity;

namespace YuanCore.Building;

public sealed class BuildingSystems : Feature
{
    public BuildingSystems(MapContext mapContext)
    {
        // ── 输入 & 模式同步 ──
        Add(new ModeSyncSystem());
        Add(new CursorUpdateSystem());
        Add(new BuildingInputSystem(mapContext));

        // ── 编辑候选 ──
        Add(new EditCandidateRefreshSystem(mapContext));

        // ── Placement 跟随 & 检测 ──
        Add(new PlacementFollowSystem(mapContext));
        Add(new PlacementValidationSystem(mapContext));

        // ── 视图 ──
        Add(new ConvertGridPositionSystem(mapContext));
        Add(new CreateViewSystem(mapContext));
        Add(new LinkMaterialUpdateSystem());

        // ── 点击业务消费 ──
        Add(new ClickProcessSystem(mapContext));
    }
}
