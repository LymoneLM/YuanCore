using UnityEngine;
using UnityEngine.EventSystems;

namespace YuanCore.Building;

public static class PointChecker
{
    public static bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}
