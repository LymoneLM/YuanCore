using Entitas;
using Entitas.Unity;

namespace YuanCore.Building;

// ECS挂载点
public class BuildingController
{
    public static BuildingController Instance => field ??= new BuildingController();
    private readonly Systems _systems;

    public BuildingController()
    {
        BuildingContextInitialization.Initialize();
        var mapContext = MapContext.Instance;

        mapContext.CreateContextObserver();

        _systems = new BuildingSystems(mapContext);
    }

    public void Initialize()
    {
        _systems.Initialize();
    }

    public void Execute()
    {
        _systems.Execute();
        _systems.Cleanup();
    }
}
