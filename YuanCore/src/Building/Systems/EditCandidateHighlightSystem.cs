using System.Collections.Generic;
using Entitas;

namespace YuanCore.Building;

/// <summary>
/// 响应式监听 EditCandidates 的 Added/Removed 事件，
/// 更新候选建筑的高亮显示。
/// </summary>
public sealed class EditCandidateHighlightSystem : ReactiveSystem<Map.Entity>
{
    private readonly MapContext _context;
    private string _lastHighlightUid;

    public EditCandidateHighlightSystem(MapContext context) : base(context)
    {
        _context = context;
    }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(
            new TriggerOnEvent<Map.Entity>(
                Matcher<Map.Entity>.AllOf(
                    YuanCoreBuildingMapEditCandidatesMatcher.EditCandidates),
                GroupEvent.AddedOrRemoved));

    protected override bool Filter(Map.Entity entity)
        => true;

    protected override void Execute(List<Map.Entity> entities)
    {
        ClearHighlight();

        if (!_context.HasEditCandidates()) return;

        var ec = _context.GetEditCandidates();
        if (ec.Candidates == null || ec.Candidates.Length == 0) return;
        if (ec.CurrentIndex < 0 || ec.CurrentIndex >= ec.Candidates.Length) return;

        var uid = ec.Candidates[ec.CurrentIndex];
        var entity = _context.GetBuildingByUid(uid);
        if (entity == null || !entity.HasView()) return;

        if (entity.GetView().View is BuildingShowView showView)
        {
            showView.SetEditHighlight(true);
            _lastHighlightUid = uid;
        }
    }

    private void ClearHighlight()
    {
        if (_lastHighlightUid == null) return;

        var entity = _context.GetBuildingByUid(_lastHighlightUid);
        if (entity != null && entity.HasView() && entity.GetView().View is BuildingShowView sv)
            sv.SetEditHighlight(false);

        _lastHighlightUid = null;
    }
}
