using YuanCore.Core;

namespace YuanCore.Building;

public static class BuildingSceneBootstrap
{
    public static void Run(string sceneID)
    {
        var dtoList = BuildingDataAdapter.Load(sceneID);

        foreach (var dto in dtoList)
        {
            if (!BuildingManager.States.CheckCanBuild(
                    dto.BuildingID, dto.Rotation, dto.GridPosition, out _))
            {
                YuanCorePlugin.Logger.LogWarning($"[SceneBootstrap] Can't load building " +
                                               $"{dto.Uid}({dto.BuildingID}|{dto.Rotation}) in {dto.GridPosition}");
                continue;
            }

            var entity = MapContext.Instance.CreateEntity();
            entity.AddBuilding(dto.Uid, dto.BuildingID);
            entity.AddBuildingState(dto.TaoZhuangID, dto.Rotation, dto.IsRuined, false);
            entity.AddGridPosition(dto.GridPosition);
            entity.AddLinkMaterialUpdate(1);

            BuildingManager.States.AddBuilding(dto.BuildingID, dto.Rotation, dto.GridPosition, dto.Uid);
        }
    }
}
