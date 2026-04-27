using Entitas;

namespace YuanCore.Building;

/// <summary>
/// 监听 EditCandidates 的 CurrentIndex 变化（由滚轮触发），
/// 更新候选建筑的高亮显示。
/// </summary>
public sealed class EditCandidateHighlightSystem : IExecuteSystem
{
    private readonly MapContext _context;
    private int _lastIndex = -1;
    private string _lastHighlightUid;

    public EditCandidateHighlightSystem(MapContext context)
    {
        _context = context;
    }

    public void Execute()
    {
        if (!_context.HasEditCandidates())
        {
            ClearHighlight();
            _lastIndex = -1;
            return;
        }

        var ec = _context.GetEditCandidates();
        if (ec.Candidates == null || ec.Candidates.Length == 0)
        {
            ClearHighlight();
            _lastIndex = -1;
            return;
        }

        if (ec.CurrentIndex == _lastIndex) return;
        _lastIndex = ec.CurrentIndex;

        ClearHighlight();

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
