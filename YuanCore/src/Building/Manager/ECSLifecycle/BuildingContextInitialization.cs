using Entitas.Generators.Attributes;

namespace YuanCore.Building;

public static partial class BuildingContextInitialization
{
    public static void Initialize()
    {
        InitializeMapContext();
    }

    [ContextInitialization(typeof(MapContext))]
    public static partial void InitializeMapContext();
}
