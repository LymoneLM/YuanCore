using System;
using System.Collections.Generic;
using UnityEngine;
using YuanCore.Core;
using Object = UnityEngine.Object;

namespace YuanCore.Building;

public static class PrefabFactory
{
    private static readonly Dictionary<string, GameObject> BackMapCache = new();
    private static readonly Dictionary<string, GameObject> BuildingShowCache = new();
    private static readonly Dictionary<string, GameObject> BuildingPlacementCache = new();

    private static GameObject _templateHolder;

    private static Transform GetTemplateHolder()
    {
        if (_templateHolder != null) return _templateHolder.transform;
        _templateHolder = new GameObject("[PrefabFactory_Templates]");
        _templateHolder.SetActive(false);
        Object.DontDestroyOnLoad(_templateHolder);
        return _templateHolder.transform;
    }

    public static T LoadAsBackMap<T>(string path) where T : Object
    {
        if (BackMapCache.TryGetValue(path, out var cached))
            return cached as T;

        var raw = Resources.Load<GameObject>(path);
        if (raw == null)
        {
            YuanCorePlugin.Logger.LogError($"PrefabLoader: Load failed, path = {path}");
            return null as T;
        }

        var template = Object.Instantiate(raw, GetTemplateHolder());

        if (path.IndexOf("/s/", StringComparison.OrdinalIgnoreCase) != -1)
        {
            template.transform.Find("BuildShop").gameObject.name = "BuildShow";
            Object.DestroyImmediate(template.transform.Find("BuildCity").gameObject);
        }

        if (path.IndexOf("/l/", StringComparison.OrdinalIgnoreCase) != -1 ||
            path.IndexOf("/z/", StringComparison.OrdinalIgnoreCase) != -1)
        {
            var ts = template.transform.Find("AddClickBack");
            for (var i = 0; i < ts.childCount; ++i)
            {
                var obj = ts.GetChild(i).GetComponent<AddClickBack>();
                if (obj != null) Object.DestroyImmediate(obj);
            }
            ts.gameObject.name = "AllBack";
            ts = ts.Find("AllKuai");
            for (var i = 0; i < ts.childCount; ++i)
            {
                var obj = ts.GetChild(i).GetComponent<PerFeiWoRange>();
                if (obj != null) Object.DestroyImmediate(obj);
                RemoveColliders(ts.GetChild(i).gameObject);
                RemoveRigidbodies(ts.GetChild(i).gameObject);
            }
        }
        else
        {
            var obj = template.transform.Find("AddClickBack")?.gameObject;
            if (obj != null) Object.DestroyImmediate(obj);
        }

        foreach (var name in new[] { "BuildPosiGet", "AllZuDang", "BuildTip" })
        {
            var obj = template.transform.Find(name)?.gameObject;
            if (obj != null) Object.DestroyImmediate(obj);
        }

        RemoveAllScripts(template);

        BackMapCache[path] = template;
        return template as T;
    }

    public static T LoadAsBuildingShow<T>(string path) where T : Object
    {
        if (BuildingShowCache.TryGetValue(path, out var cached))
            return cached as T;

        var raw = Resources.Load<GameObject>(path);
        if (raw == null)
        {
            YuanCorePlugin.Logger.LogError($"PrefabLoader: Load failed, path = {path}");
            return null as T;
        }

        var template = Object.Instantiate(raw, GetTemplateHolder());

        foreach (var name in new[] {
            "UI/TouMing", "UI/AllDrag", "UIBT",
            "LineCollider", "RangeColliderA", "RangeColliderB", "NPC", "OutQMember" })
        {
            var obj = template.transform.Find(name)?.gameObject;
            if (obj != null) Object.DestroyImmediate(obj);
        }

        var go = template.transform.Find("UI/OpenBT")?.gameObject;
        if (go != null)
        {
            RemoveAllScripts(go);
            go.AddComponent<BuildingOpenBT>();
        }
        RemoveAllScripts(template);
        template.AddComponent<BuildingShowView>();

        BuildingShowCache[path] = template;
        return template as T;
    }

    public static T LoadAsBuildingPlacement<T>(string path) where T : Object
    {
        if (BuildingPlacementCache.TryGetValue(path, out var cached))
            return cached as T;

        var raw = Resources.Load<GameObject>(path);
        if (raw == null)
        {
            YuanCorePlugin.Logger.LogError($"PrefabLoader: Load failed, path = {path}");
            return null as T;
        }

        var template = Object.Instantiate(raw, GetTemplateHolder());

        var allDrag = template.transform.Find("UI/AllDrag");
        if (allDrag == null)
            throw new NullReferenceException(
                $"PrefabLoader: prefab '{raw.name}' does not contain path 'UI/AllDrag'");

        for (var i = 0; i < allDrag.childCount; i++)
        {
            var go = allDrag.GetChild(i).gameObject;
            RemoveRigidbodies(go);
            RemoveColliders(go);
            RemoveAllScripts(go);
        }

        RemoveAllScripts(template);
        template.AddComponent<BuildingPlacementView>();

        BuildingPlacementCache[path] = template;
        return template as T;
    }

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
