using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance;
    public Systems Systems;

    private void Awake()
    {
        if (Instance != null)
            throw new InvalidOperationException("BuildingController is already instantiated!");
        Instance = this;

        BuildingContextInitialization.Initialize();
        var mapContext = MapContext.Instance;

        mapContext.CreateContextObserver();

        Systems = new BuildingSystems(mapContext);
    }

    public void Start()
    {
        Systems.Initialize();
    }

    public void Update()
    {
        Systems.Execute();
        Systems.Cleanup();
    }
}
