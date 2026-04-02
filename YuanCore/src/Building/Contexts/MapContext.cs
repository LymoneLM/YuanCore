using Entitas;

namespace YuanCore.Building;

partial class MapContext : IContext
{
    public static MapContext Instance
    {
        get => field ??= new MapContext();
        set;
    }
}
