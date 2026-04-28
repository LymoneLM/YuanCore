using System;
using UnityEngine;

namespace YuanCore.Building;

public class AllBuildEx : MonoBehaviour
{
    private string _sceneType;
    private int _sceneSize;
    private Transform _backMap;

    private void Awake()
    {
        _backMap = transform.Find("BackMap");
    }

    public void SwitchScene(string sceneID, Action<Transform> onComplete = null)
    {
        (_sceneType, _sceneSize) = SceneIDResolver.GetSceneType(sceneID);
        _backMap.transform.DestroyAllChildren();
        this.DelayInvoke(() => LoadScene(onComplete), 0.4f);
    }

    private void LoadScene(Action<Transform> onComplete)
    {
        var prefab = GetScenePrefab();
        var instance = Instantiate(prefab, _backMap).transform;
        instance.localScale = Vector3.one;
        instance.localPosition = Vector3.zero;
        var ts = instance.Find("BuildShow");
        onComplete?.Invoke(ts);
    }

    private GameObject GetScenePrefab()
    {
        switch (_sceneType)
        {
            case "M":
                return PrefabFactory.LoadAsBackMap<GameObject>("AllBackMap/M/" + _sceneSize);
            case "Z":
                FormulaData.SetNeiGameGuide(1);
                return PrefabFactory.LoadAsBackMap<GameObject>("AllBackMap/Z/" + _sceneSize);
            case "S":
                return PrefabFactory.LoadAsBackMap<GameObject>("AllBackMap/S/" + _sceneSize);
            case "F":
                return PrefabFactory.LoadAsBackMap<GameObject>("PerFengdiScene");
            case "H":
                return PrefabFactory.LoadAsBackMap<GameObject>("AllBackMap/H/" + _sceneSize);
            case "L":
                return PrefabFactory.LoadAsBackMap<GameObject>("AllBackMap/L/" + _sceneSize);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
