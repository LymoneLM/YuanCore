using Entitas;

namespace YuanCore.Building;

public static class MapContextExtensions
{
    public const string YUAN_CORE_BUILDING_UID = "YuanCoreBuildingUID";

    extension(MapContext mapContext)
    {
        public MapContext AddCustomEntityIndexes()
        {
            mapContext.AddEntityIndex(new PrimaryEntityIndex<Map.Entity, string>(
                YUAN_CORE_BUILDING_UID,
                mapContext.GetGroup(YuanCoreBuildingMapBuildingMatcher.Building),
                (entity, component) => (component as BuildingComponent)?.Uid ?? entity.GetBuilding().Uid));
            return mapContext;
        }

        public Map.Entity GetBuildingByUid(string uid)
        {
            return ((PrimaryEntityIndex<Map.Entity, string>)mapContext
                    .GetEntityIndex(YUAN_CORE_BUILDING_UID))
                .GetEntity(uid);
        }
    }
}
