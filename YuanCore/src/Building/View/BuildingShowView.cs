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
        if (InputBlocker.IsPointerOverUI()) return;
        if (BuildingModeManager.CurrentMode != BuildingInteractionMode.Normal &&
            BuildingModeManager.CurrentMode != BuildingInteractionMode.EditSelect)
            return;

        SetHoverVisual(true);
    }

    public void OnPointerExitForwarded()
    {
        SetHoverVisual(false);
    }

    public void OnClickForwarded(PointerEventData.InputButton button)
    {
        if (button != PointerEventData.InputButton.Left) return;

        if (InputBlocker.IsPointerOverUI()) return;
        if (BuildingModeManager.CurrentMode != BuildingInteractionMode.Normal) return;

        if (LinkedEntity != null && LinkedEntity.IsEnabled)
        {
            if (!LinkedEntity.HasClicked())
                LinkedEntity.AddClicked();
        }
    }

    public void SetEditHighlight(bool on)
    {
        SetHoverVisual(on);
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
