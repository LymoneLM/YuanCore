using Entitas;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace YuanCore.Building;

public class BuildingPlacementView : BuildingView, IYuanCoreBuildingMapPlacementAddedListener
{
    private static readonly Color Green = new(0f, 1f, 0f, 0.2f);
    private static readonly Color Red = new(1f, 0f, 0f, 0.2f);

    private readonly Dictionary<Vector2Int, Image> _imageMap = new();
    private readonly Dictionary<Vector2Int, bool> _flagMap = new();

    protected override void Awake()
    {
        base.Awake();

        var ts = transform.Find("UI/AllDrag");
        for (var i = 0; i < ts.childCount; ++i)
        {
            var obj = ts.GetChild(i).gameObject;
            var parts = obj.name.Split('|');
            _imageMap[new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]))] =
                obj.GetComponent<Image>();
        }
    }

    public override void Link(Entity entity)
    {
        base.Link(entity);
        LinkedEntity.AddPlacementAddedListener(this);

        var cmp = LinkedEntity.GetPlacement();
        OnPlacementAdded(LinkedEntity, cmp.Offset, cmp.Flags);
    }

    public void OnPlacementAdded(Map.Entity entity, Vector2Int _, (Vector2Int, bool)[] flags)
    {
        _flagMap.Clear();

        foreach (var (pos, canPlace) in flags)
        {
            if (_flagMap.TryGetValue(pos, out var current))
                _flagMap[pos] = current && canPlace;
            else
                _flagMap[pos] = canPlace;
        }

        foreach (var flag in _flagMap)
        {
            if (_imageMap.TryGetValue(flag.Key, out var image))
            {
                image.color = flag.Value ? Green : Red;
            }
        }
    }
}
