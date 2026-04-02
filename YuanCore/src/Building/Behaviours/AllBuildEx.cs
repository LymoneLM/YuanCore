using UnityEngine;

namespace YuanCore.Building;

public class AllBuildEx : MonoBehaviour
{
    private string _sceneID;
    private Transform _backMap;

    private void Awake()
    {
        _backMap = transform.Find("BackMap");
    }

    private void OnEnable()
    {
        BuildingSignals.OnMapChanged += SwitchScene;
    }

    private void OnDisable()
    {
        BuildingSignals.OnMapChanged -= SwitchScene;
    }

    private void SwitchScene(string sceneID)
    {
        _sceneID = sceneID;
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
        var parts = _sceneID.Split('|');
        var sceneClass = parts[0];
        var sceneIndex = int.Parse(parts[1]);
        var sceneIndex2 = parts.Length > 2 ? int.Parse(parts[2]) : 0;

        switch (sceneClass)
        {
            case "M":
                return Resources.Load<GameObject>(
                    "AllBackMap/M/" + Mainload.Fudi_now[sceneIndex][37]);
            case "Z":
                FormulaData.SetNeiGameGuide(1);
                return Resources.Load<GameObject>(
                    "AllBackMap/Z/" + Mainload.NongZ_now[sceneIndex][sceneIndex2][5]);
            case "S":
                return Resources.Load<GameObject>("AllBackMap/S/" + sceneIndex);
            case "F":
                return Resources.Load<GameObject>("PerFengdiScene");
            case "H":
                return Resources.Load<GameObject>("AllBackMap/H/" + sceneIndex);
            case "L":
                return Resources.Load<GameObject>(
                    "AllBackMap/L/" + Mainload.Mudi_now[sceneIndex][sceneIndex2][2]);
            default:
                return null;
        }
    }
}
