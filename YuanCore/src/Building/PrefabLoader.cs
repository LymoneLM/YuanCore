// 这是一个兼容性工具，加载原版预制体并修改为YuanCore需求的结构

using UnityEngine;

namespace YuanCore.Building;

public static class PrefabLoader
{
    /// <summary>
    /// 代理 Resources.Load,对 GameObject 预制体进行预处理：<br/>
    /// 根节点挂载 A.cs <br/>
    /// 清理 UI/AllDrag 下所有节点的刚体、碰撞体、脚本 <br/>
    /// 仅推荐用于加载 AllBuild/x/BuildTip/x/x
    /// </summary>
    public static T Load<T>(string path) where T : Object
    {
        var asset = Resources.Load<T>(path);
        if (asset == null)
        {
            Debug.LogWarning($"ResourcesProxy.Load failed, path = {path}");
            return null;
        }

        if (asset is GameObject prefab)
        {
            ProcessPrefabInPlace(prefab);
        }

        return asset;
    }

    public static Object Load(string path, System.Type systemTypeInstance)
    {
        var asset = Resources.Load(path, systemTypeInstance);
        if (asset == null)
        {
            Debug.LogWarning($"ResourcesProxy.Load failed, path = {path}, type = {systemTypeInstance}");
            return null;
        }

        if (asset is GameObject prefab)
        {
            ProcessPrefabInPlace(prefab);
        }

        return asset;
    }

    private static void ProcessPrefabInPlace(GameObject prefab)
    {
        if (prefab == null)
            return;

        if (prefab.GetComponent<BuildingPlacementView>() != null)
            return;

        prefab.AddComponent<BuildingPlacementView>();

        // UI/AllDrag
        var allDrag = prefab.transform.Find("UI/AllDrag");
        if (allDrag == null)
        {
            Debug.LogWarning($"ResourcesProxy: prefab '{prefab.name}' does not contain path 'UI/AllDrag'");
            return;
        }

        for (var i = 0; i < allDrag.childCount; i++)
        {
            var go = allDrag.GetChild(i).gameObject;
            RemoveRigidbodies(go);
            RemoveColliders(go);
            RemoveAllScripts(go);
        }
    }

    /// <summary>
    /// 删除 2D 刚体
    /// </summary>
    private static void RemoveRigidbodies(GameObject go)
    {
        if (go == null)
            return;

        var rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            Object.DestroyImmediate(rb2d);
        }
    }

    /// <summary>
    /// 删除所有 2D 碰撞体
    /// 包括 PolygonCollider2D、CircleCollider2D
    /// </summary>
    private static void RemoveColliders(GameObject go)
    {
        if (go == null)
            return;

        var colliders2D = go.GetComponents<Collider2D>();
        foreach (var collider in colliders2D)
        {
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
        }
    }

    /// <summary>
    /// 删除节点上所有 MonoBehaviour 脚本
    /// </summary>
    private static void RemoveAllScripts(GameObject go)
    {
        if (go == null)
            return;

        var scripts = go.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != null)
            {
                Object.DestroyImmediate(script);
            }
        }
    }
}
