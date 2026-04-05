namespace YuanCore.Building;

public static class BuildingSceneBootstrap
{
    public static void Bootstrap(string sceneID)
    {
        var mapContext = MapContext.Instance;
        mapContext.DestroyAllEntities();

        Mainload.TempMemberIndex_now = 0;
        Mainload.BuildPosiID_Now = "0|0";
        Mainload.BuildID_CreatNow = "null";

        var dtoList = BuildingDataAdapter.Load(sceneID);

        foreach (var dto in dtoList)
        {
            var entity = mapContext.CreateEntity();
            entity.AddBuilding(dto.Uid, dto.BuildingID);
            entity.AddBuildingState(dto.TaoZhuangID, dto.Rotation, dto.IsRuined);
            entity.AddGridPosition(dto.GridPosition);
            entity.AddWorldPosition(PositionConvertor.GridToWorld(dto.GridPosition));

            BuildingStates.Instance.AddBuilding(
                dto.BuildingID,
                dto.Rotation,
                dto.GridPosition,
                dto.Uid
            );
        }

        Mainload.isCreatSceneFinish = true;
        Mainload.isSwichPanelOpen = false;
    }
}
