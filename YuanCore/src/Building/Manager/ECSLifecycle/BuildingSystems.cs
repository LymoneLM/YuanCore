using Entitas.Unity;

namespace YuanCore.Building;

public sealed class BuildingSystems : Feature
{
    public BuildingSystems(MapContext mapContext)
    {
        // ── 编辑候选 ──
        Add(new EditCandidateRefreshSystem(mapContext));

        // ── Placement 检测 ──
        Add(new UpdateValidationSystem(mapContext));

        // ── 视图 ──
        Add(new ConvertGridPositionSystem(mapContext));
        Add(new CreateViewSystem(mapContext));
        Add(new UpdateLinkMaterialSystem(mapContext));

        // ── 点击业务消费 ──
        Add(new ProcessClickedSystem(mapContext));
    }
}
