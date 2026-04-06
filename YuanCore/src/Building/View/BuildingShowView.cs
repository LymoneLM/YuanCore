using Entitas;

namespace YuanCore.Building;

public class BuildingShowView : BuildingView
{
    private LinkMaterialUpdaterFactory.LinkMaterialUpdater _updater;

    public override void Link(Entity entity)
    {
        base.Link(entity);
        _updater = LinkMaterialUpdaterFactory.GetUpdater(LinkedEntity.GetBuilding().BuildingID);
    }

    public void UpdateLinkMaterial(int diffuseLevel)
    {
        _updater?.Invoke(transform, LinkedEntity, diffuseLevel);
    }
}
