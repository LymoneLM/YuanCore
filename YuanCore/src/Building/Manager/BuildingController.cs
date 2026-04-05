using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance;
    private Systems _systems;

    private void Awake()
    {
        if (Instance != null)
            throw new InvalidOperationException("BuildingController is already instantiated!");
        Instance = this;

        BuildingContextInitialization.Initialize();
        var mapContext = MapContext.Instance;

        mapContext.CreateContextObserver();

        _systems = new BuildingSystems(mapContext);
    }

    public void Start()
    {
        _systems.Initialize();
    }

    public void Update()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
