// 这是一个兼容性工具，加载原版预制体并修改为YuanCore需求的结构

using System;
using UnityEngine;
using YuanCore.Core;
using Object = UnityEngine.Object;

namespace YuanCore.Building;

public static class PrefabLoader
{
    public static T LoadAsBackMap<T>(string path) where T : Object
    {
        var asset = Resources.Load<T>(path);
        if (asset == null)
            throw new NullReferenceException($"PrefabLoader: Load failed, path = {path}");

        if (asset is not GameObject prefab)
            return asset;

        if (path.IndexOf("/l/", StringComparison.Ordinal) != -1 || path.IndexOf("/z/", StringComparison.Ordinal) != -1)
        {
            var ts = prefab.transform.Find("AddClickBack");
            for (var i = 0; i < ts.childCount; ++i)
            {
                RemoveAllScripts(ts.GetChild(i).gameObject);
            }

            ts.gameObject.name = "AllBack";
            ts = ts.Find("AllKuai");
            for (var i = 0; i < ts.childCount; ++i)
            {
                RemoveAllScripts(ts.GetChild(i).gameObject);
            }
        }
        else
        {
            var obj = prefab.transform.Find("AddClickBack").gameObject;
            if (obj != null)
                Object.DestroyImmediate(obj);
        }

        string[] arr = ["BuildPosiGet", "AllZuDang", "BuildTip"];
        foreach (var name in arr)
        {
            var obj = prefab.transform.Find(name).gameObject;
            if (obj != null)
                Object.DestroyImmediate(obj);
        }

        RemoveAllScripts(prefab);

        return asset;
    }

    public static T LoadAsBuilding<T>(string path) where T : Object
    {
        var asset = Resources.Load<T>(path);
        if (asset == null)
            throw new NullReferenceException($"PrefabLoader: Load failed, path = {path}");

        if (asset is not GameObject prefab || prefab.GetComponent<BuildingView>() != null)
            return asset;

        // UI/AllDrag
        var allDrag = prefab.transform.Find("UI/AllDrag");
        if (allDrag == null)
            throw new NullReferenceException($"PrefabLoader: prefab '{prefab.name}' does not contain path 'UI/AllDrag'");

        for (var i = 0; i < allDrag.childCount; i++)
        {
            var go = allDrag.GetChild(i).gameObject;
            RemoveRigidbodies(go);
            RemoveColliders(go);
            RemoveAllScripts(go);
        }

        string[] arr = ["UI/TouMing", "UIBT", "LineCollider", "RangeColliderA", "RangeColliderB", "NPC", "OutQMember"];
        foreach (var name in arr)
        {
            var obj = prefab.transform.Find(name).gameObject;
            if (obj != null)
                Object.DestroyImmediate(obj);
        }

        RemoveAllScripts(prefab);
        prefab.AddComponent<BuildingView>();

        return asset;
    }

    /// <summary>
    /// 代理 Resources.Load,对 GameObject 预制体进行预处理：<br/>
    /// 根节点挂载 BuildingPlacementView.cs <br/>
    /// 清理 UI/AllDrag 下所有节点的刚体、碰撞体、脚本 <br/>
    /// 仅推荐用于加载 AllBuild/x/BuildTip/x/x
    /// </summary>
    public static T LoadAsBuildingPlacement<T>(string path) where T : Object
    {
        var asset = Resources.Load<T>(path);
        if (asset == null)
            throw new NullReferenceException($"PrefabLoader: Load failed, path = {path}");

        if (asset is not GameObject prefab || prefab.GetComponent<BuildingPlacementView>() != null)
            return asset;

        // UI/AllDrag
        var allDrag = prefab.transform.Find("UI/AllDrag");
        if (allDrag == null)
            throw new NullReferenceException($"PrefabLoader: prefab '{prefab.name}' does not contain path 'UI/AllDrag'");

        for (var i = 0; i < allDrag.childCount; i++)
        {
            var go = allDrag.GetChild(i).gameObject;
            RemoveRigidbodies(go);
            RemoveColliders(go);
            RemoveAllScripts(go);
        }

        RemoveAllScripts(prefab);
        prefab.AddComponent<BuildingPlacementView>();

        return asset;
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
