using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YuanCore.Building;

public class BuildingOpenBT : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private BuildingShowView _showView;

    private void Awake()
    {
        _showView = GetComponentInParent<BuildingShowView>();
        transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.005f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_showView != null)
            _showView.OnPointerEnterForwarded();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_showView != null)
            _showView.OnPointerExitForwarded();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_showView != null)
            _showView.OnClickForwarded(eventData.button);
    }
}
