using UnityEngine;

namespace YuanCore.Building;

public static class TransformExtensions
{
    /// <summary>
    /// 删除所有子对象
    /// </summary>
    public static void DestroyAllChildren(this Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(parent.GetChild(i).gameObject);
        }
    }
}
