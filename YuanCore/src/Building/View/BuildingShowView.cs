using Entitas;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YuanCore.Building;

public class BuildingShowView : BuildingView
{
    private LinkMaterialUpdaterFactory.LinkMaterialUpdater _updater;
    private bool _isHovered;
    private static readonly Color ColorHovered = new(0.6f, 0.6f, 0.6f);
    private static readonly Color ColorNormal = new(1f, 1f, 1f);

    public override void Link(Entity entity)
    {
        base.Link(entity);
        _updater = LinkMaterialUpdaterFactory.GetUpdater(LinkedEntity.GetBuilding().BuildingID);
    }

    public void UpdateLinkMaterial(int diffuseLevel)
    {
        _updater?.Invoke(transform, LinkedEntity, diffuseLevel);
    }

    public void OnPointerEnterForwarded()
    {
        if (PointChecker.IsPointerOverUI()) return;

        if (LinkedEntity != null && LinkedEntity.IsEnabled)
            BuildingSignals.InvokePointerEnterBuilding(
                LinkedEntity.GetBuilding().Uid);
    }

    public void OnPointerExitForwarded()
    {
        if (LinkedEntity != null && LinkedEntity.IsEnabled)
            BuildingSignals.InvokePointerExitBuilding(
                LinkedEntity.GetBuilding().Uid);
    }

    public void OnClickForwarded(PointerEventData.InputButton button)
    {
        if (PointChecker.IsPointerOverUI()) return;
        if (button != PointerEventData.InputButton.Left) return;
        if (MainloadCompat.CurrentMode != BuildMode.Normal) return;

        if (LinkedEntity != null && LinkedEntity.IsEnabled)
            if (!LinkedEntity.HasClicked())
                LinkedEntity.AddClicked();
    }

    public void SetHoverVisual(bool hovered)
    {
        if (_isHovered == hovered) return;
        _isHovered = hovered;

        foreach (var (_, sprite) in Sprites)
        {
            sprite.color = hovered ? ColorHovered : ColorNormal;
        }
    }
}
