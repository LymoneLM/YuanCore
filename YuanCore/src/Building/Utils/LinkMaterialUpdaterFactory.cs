using System.Collections.Generic;
using UnityEngine;

namespace YuanCore.Building;

public static class LinkMaterialUpdaterFactory
{
    public delegate void LinkMaterialUpdater(Transform transform, Map.Entity entity, int diffuseLevel);

    private enum GroupID
    {
        Road,
        Pond,
        DeepPond,
        RiverWay,
        SingleWall,
        MultiWall,
    }

    private static readonly Dictionary<int, GroupID> BuildingGroupMap = new()
    {
        {82, GroupID.Road}, {83, GroupID.Road}, {84, GroupID.Road},
        {26, GroupID.Pond},
        {90, GroupID.DeepPond},
    };

    public static LinkMaterialUpdater GetUpdater(int buildingID)
    {
        if (!BuildingGroupMap.TryGetValue(buildingID, out var group))
            return null;
        return group switch
        {
            GroupID.Road => FlatQuadUpdater(GroupID.Road),
            GroupID.Pond => FlatQuadUpdater(GroupID.Pond),
            GroupID.DeepPond => DeepPondUpdater(),
            _ => null
        };
    }

    private static readonly Vector2Int[] RoadCheckGridOffset = [
        new(0,-1), new(-1,0), new(0, 1), new(1, 0)
    ];
    private static readonly Vector2Int[] PondCheckGridOffset = [
        new(1,0), new(0,1), new(0, -1), new(-1, 0)
    ];

    private static LinkMaterialUpdater FlatQuadUpdater(GroupID groupID)
    {
        SpriteRenderer[] sprites = null;
        return (transform, entity, diffuseLevel) =>
        {
            if (sprites == null)
            {
                sprites = new SpriteRenderer[4];
                transform = transform.Find("Build");
                for (var i = 0; i < transform.childCount; ++i)
                {
                    var ts = transform.GetChild(i);
                    var parts = ts.name.Split('|');
                    if (parts.Length >= 3 && parts[2] != "4")
                        sprites[int.Parse(parts[2])] = ts.GetComponent<SpriteRenderer>();
                }
            }

            var map = MapContext.Instance;
            var diffuse = diffuseLevel > 0;
            var posi = entity.GetGridPosition().Value;
            var offset = groupID == GroupID.Road ? RoadCheckGridOffset : PondCheckGridOffset;
            for (var i = 0; i < 4; ++i)
            {
                var uids = BuildingStates.Instance.GetCellBuildingsUid(posi + offset[i]);
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group == groupID;
                    sprites[i].enabled = !flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                    break;
                }
            }
        };
    }

    private static readonly Vector2Int[] DeepPondCheckGridOffset = [
        new(1,0), new(0,1), new(0, -1), new(-1, 0), new(-1, -1), new(-1, -1)
    ];

    private static LinkMaterialUpdater DeepPondUpdater()
    {
        SpriteRenderer[] sprites = null;
        return (transform, entity, diffuseLevel) =>
        {
            if (sprites == null)
            {
                sprites = new SpriteRenderer[6];
                transform = transform.Find("Build");
                for (var i = 0; i < transform.childCount; ++i)
                {
                    var ts = transform.GetChild(i);
                    var parts = ts.name.Split('|');
                    if (parts.Length < 3 || parts[2] == "4")
                        continue;
                    var num = int.Parse(parts[2]);
                    sprites[num > 4 ? num - 1 : num] = ts.GetComponent<SpriteRenderer>();
                }
            }

            var map = MapContext.Instance;
            var diffuse = diffuseLevel > 0;
            var posi = entity.GetGridPosition().Value;
            for (var i = 0; i < 6; ++i)
            {
                var uids = BuildingStates.Instance.GetCellBuildingsUid(posi + DeepPondCheckGridOffset[i]);
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group == GroupID.DeepPond;
                    sprites[i].enabled = !flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                    break;
                }
            }
        };
    }
}
