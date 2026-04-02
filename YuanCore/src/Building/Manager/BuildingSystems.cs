using Entitas.Unity;

namespace YuanCore.Building;

public sealed class BuildingSystems : Feature
{
    public BuildingSystems(MapContext mapContext)
    {
        Add(new ConvertGridPositionSystem(mapContext));


    }
}
