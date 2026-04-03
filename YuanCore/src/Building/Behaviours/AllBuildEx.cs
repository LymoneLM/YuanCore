using System;
using UnityEngine;

namespace YuanCore.Building;

public class AllBuildEx : MonoBehaviour
{
    private string _sceneClass;
    private int _sceneIndex;
    private Transform _backMap;

    private void Awake()
    {
        _backMap = transform.Find("BackMap");
    }

    private void OnEnable()
    {
        BuildingSignals.OnSceneChanged += SwitchScene;
    }

    private void OnDisable()
    {
        BuildingSignals.OnSceneChanged -= SwitchScene;
    }

    private void SwitchScene(string sceneClass, int sceneIndex)
    {
        _sceneClass = sceneClass;
        _sceneIndex = sceneIndex;
        _backMap.transform.DestroyAllChildren();
        this.DelayInvoke(LoadScene, 0.4f);
    }

    private void LoadScene()
    {
        var prefab = GetScenePrefab();
        var instance = Instantiate(prefab, _backMap);
        instance.transform.localScale = Vector3.one;
        instance.transform.localPosition = Vector3.zero;
    }

    private GameObject GetScenePrefab()
    {
        switch (_sceneClass)
        {
            case "M":
                return Resources.Load<GameObject>("AllBackMap/M/" + _sceneIndex);
            case "Z":
                FormulaData.SetNeiGameGuide(1);
                return Resources.Load<GameObject>("AllBackMap/Z/" + _sceneIndex);
            case "S":
                return Resources.Load<GameObject>("AllBackMap/S/" + _sceneIndex);
            case "F":
                return Resources.Load<GameObject>("PerFengdiScene");
            case "H":
                return Resources.Load<GameObject>("AllBackMap/H/" + _sceneIndex);
            case "L":
                return Resources.Load<GameObject>("AllBackMap/L/" + _sceneIndex);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
