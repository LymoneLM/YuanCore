using Entitas;
using lmm = YuanCore.Building.YuanCoreBuildingMapLinkMaterialUpdateMatcher;
using vm = YuanCore.Building.YuanCoreBuildingMapViewMatcher;

namespace YuanCore.Building;

public sealed class LinkMaterialUpdateSystem : IExecuteSystem
{
    public void Execute()
    {
        var entities = MapContext.Instance.GetGroup(Matcher<Map.Entity>
            .AllOf(lmm.LinkMaterialUpdate, vm.View));
        foreach (var entity in entities)
        {
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
