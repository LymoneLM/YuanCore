using System;
using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance;
    public Systems Systems;
    private BuildingInputManager _inputManager;

    private void Awake()
    {
        if (Instance != null)
            throw new InvalidOperationException("BuildingController is already instantiated!");
        Instance = this;

        BuildingContextInitialization.Initialize();
        var mapContext = MapContext.Instance.AddCustomEntityIndexes();

        mapContext.CreateContextObserver();

        _inputManager = new BuildingInputManager(mapContext);
        Systems = new BuildingSystems(mapContext);
    }

    public void Start()
    {
        _inputManager.Initialize();
        Systems.Initialize();
    }

    public void Update()
    {
        _inputManager.Update();
        Systems.Execute();
        Systems.Cleanup();
    }
}
