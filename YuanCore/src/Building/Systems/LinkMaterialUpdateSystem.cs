using System.Collections.Generic;
using Entitas;
using lmm = YuanCore.Building.YuanCoreBuildingMapLinkMaterialUpdateMatcher;

namespace YuanCore.Building;

public sealed class LinkMaterialUpdateSystem : IExecuteSystem
{
    private readonly IGroup<Map.Entity> _group;
    private readonly List<Map.Entity> _buffer = [];

    public LinkMaterialUpdateSystem()
    {
        _group = MapContext.Instance.GetGroup(Matcher<Map.Entity>.AllOf(lmm.LinkMaterialUpdate));
    }

    public void Execute()
    {
        _buffer.Clear();
        _buffer.AddRange(_group.GetEntities());
        var len = _buffer.Count;
        for (var i = 0; i < len; ++i)
        {
            var entity = _buffer[i];
            if (!entity.HasLinkMaterialUpdate())
                continue;
            if (!entity.HasView() || entity.GetView().View is not BuildingShowView view)
            {
                entity.RemoveLinkMaterialUpdate();
                continue;
            }
            view.UpdateLinkMaterial(entity.GetLinkMaterialUpdate().DiffuseLevel);
            entity.RemoveLinkMaterialUpdate();
        }
    }
}
