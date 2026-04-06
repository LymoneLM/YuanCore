using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using YuanCore.Core;

namespace YuanCore.Building;

public static class LinkMaterialUpdaterFactory
{
    public delegate void LinkMaterialUpdater(Transform transform, Map.Entity entity, int diffuseLevel);

    private enum GroupID
    {
        Road,
        Pond,
        DeepPond,
        RiverWay, // TODO
        Wall,
        Gate,
        CityWall, // TODO
    }

    private static readonly Dictionary<int, GroupID> BuildingGroupMap = new()
    {
        {82, GroupID.Road}, {83, GroupID.Road}, {84, GroupID.Road},
        {26, GroupID.Pond},
        {90, GroupID.DeepPond},
        {18, GroupID.Wall}, {87, GroupID.Wall}, {88, GroupID.Wall}, {89, GroupID.Wall},
        {174, GroupID.Wall}, {175, GroupID.Wall}, {176, GroupID.Wall}, {177, GroupID.Wall}, {217, GroupID.Wall},
        {85, GroupID.Gate}, {178, GroupID.Gate}
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
            GroupID.Wall => WallUpdater(GroupID.Wall),
            GroupID.Gate => WallUpdater(GroupID.Gate),
            _ => null
        };
    }

    private static readonly Vector2Int[] RoadTraverseOffset = [
        new(0,-1), new(-1,0), new(0, 1), new(1, 0)
    ];
    private static readonly Vector2Int[] PondTraverseOffset = [
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
            var offset = groupID == GroupID.Road ? RoadTraverseOffset : PondTraverseOffset;
            for (var i = 0; i < 4; ++i)
            {
                var uids = BuildingStates.Instance.GetCellBuildingsUid(posi + offset[i]);
                var disable = false;
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group == groupID;
                    disable |= flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                }
                sprites[i].enabled = !disable;
            }
        };
    }

    private static readonly Vector2Int[] DeepPondTraverseOffset = [
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
                var uids = BuildingStates.Instance.GetCellBuildingsUid(posi + DeepPondTraverseOffset[i]);
                var disable = false;
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group == GroupID.DeepPond;
                    disable |= flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                }
                sprites[i].enabled = !disable;
            }
        };
    }

    public static readonly (Vector2Int offset, BuildingDirection dir)[][] WallFrontTraverseOffset =
    [
        [
            (new Vector2Int( 0,  0), BuildingDirection.West),
            (new Vector2Int( 0, -1), BuildingDirection.North),
            (new Vector2Int(-1,  0), BuildingDirection.West)
        ],
        [
            (new Vector2Int( 0,  0), BuildingDirection.North),
            (new Vector2Int(-1,  0), BuildingDirection.West),
            (new Vector2Int( 0, -1), BuildingDirection.North)
        ],
        [
            (new Vector2Int( 1,  0), BuildingDirection.West),
            (new Vector2Int( 0, -1), BuildingDirection.South),
            (new Vector2Int( 0,  0), BuildingDirection.West)
        ],
        [
            (new Vector2Int( 0,  1), BuildingDirection.North),
            (new Vector2Int(-1,  0), BuildingDirection.East),
            (new Vector2Int( 0,  0), BuildingDirection.North)
        ]
    ];

    public static readonly (Vector2Int offset, BuildingDirection dir)[][] WallBackTraverseOffset =
    [
        [
            (new Vector2Int(-1,  0), BuildingDirection.East),
            (new Vector2Int( 0,  1), BuildingDirection.North),
            (new Vector2Int( 0,  0), BuildingDirection.East)
        ],
        [
            (new Vector2Int( 0, -1), BuildingDirection.South),
            (new Vector2Int( 1,  0), BuildingDirection.West),
            (new Vector2Int( 0,  0), BuildingDirection.South)
        ],
        [
            (new Vector2Int( 0,  0), BuildingDirection.East),
            (new Vector2Int( 0,  1), BuildingDirection.South),
            (new Vector2Int( 1,  0), BuildingDirection.East)
        ],
        [
            (new Vector2Int( 0,  0), BuildingDirection.South),
            (new Vector2Int( 1,  0), BuildingDirection.East),
            (new Vector2Int( 0,  1), BuildingDirection.South)
        ]
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetWallTraverseOffset(
        BuildingDirection direction,
        out (Vector2Int offset, BuildingDirection dir)[] front,
        out (Vector2Int offset, BuildingDirection dir)[] back)
    {
        var index = (int)direction;
        front = WallFrontTraverseOffset[index];
        back = WallBackTraverseOffset[index];
    }

    private static LinkMaterialUpdater WallUpdater(GroupID groupID)
    {
        SpriteRenderer[] sprites = null;
        Vector2Int frontOffset = Vector2Int.zero, backOffset = Vector2Int.zero;
        var direction = BuildingDirection.North;
        return (transform, entity, diffuseLevel) =>
        {
            var building = entity.GetBuilding();
            var state = entity.GetBuildingState();
            if (sprites == null)
            {
                sprites = new SpriteRenderer[3];
                transform = transform.Find("Build");
                for (var i = 0; i < transform.childCount; ++i)
                {
                    var ts = transform.GetChild(i);
                    ts.gameObject.SetActive(true);
                    var parts = ts.name.Split('|');
                    if (parts.Length >= 3)
                        sprites[int.Parse(parts[2])] = ts.GetComponent<SpriteRenderer>();
                }

                var edges = BuildingShapeRegistry.Get(building.BuildingID, state.Rotation).Edges;
                direction = edges[0].Direction;
                if (groupID == GroupID.Gate)
                {
                    frontOffset = backOffset = edges[0].Position;
                    for (var i = 1; i < edges.Length; ++i)
                    {
                        var p = edges[i].Position;
                        if (p.x < frontOffset.x || (p.x == frontOffset.x && p.y < frontOffset.y))
                            frontOffset = p;
                        if (p.x > backOffset.x || (p.x == backOffset.x && p.y > backOffset.y))
                            backOffset = p;
                    }
                }
            }

            var map = MapContext.Instance;
            var diffuse = diffuseLevel > 0;
            var posi = entity.GetGridPosition().Value;
            GetWallTraverseOffset(direction, out var front, out var back);
            var pos = posi + frontOffset;
            for (var i = 0; i < 3; ++i)
            {
                var uids = BuildingStates.Instance.GetEdgeBuildingsUid(pos + front[i].offset, front[i].dir);
                var enable = false;
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group is GroupID.Wall or GroupID.Gate;
                    enable |= flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                }
                if (i < 2)
                    sprites[i].enabled = enable;
            }

            pos = posi + backOffset;
            for (var i = 0; i < 3; ++i)
            {
                var uids = BuildingStates.Instance.GetEdgeBuildingsUid(pos + back[i].offset, back[i].dir);
                var enable = false;
                foreach (var uid in uids)
                {
                    var ett = map.GetBuildingByUid(uid);
                    if(!BuildingGroupMap.TryGetValue(ett.GetBuilding().BuildingID, out var group))
                        continue;
                    var flag = group is GroupID.Wall or GroupID.Gate;
                    enable |= flag;
                    if (flag && diffuse && !ett.HasLinkMaterialUpdate())
                        ett.AddLinkMaterialUpdate(diffuseLevel - 1);
                }
                if (i == 0)
                    sprites[2].enabled = enable;
            }
        };
    }
}

