using Entitas;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingView : View , IYuanCoreBuildingMapGridPositionAddedListener
{
    private (int, SpriteRenderer)[] _sprites;

    protected virtual void Awake()
    {
        var ts = transform.Find("Build");
        _sprites = new (int, SpriteRenderer)[ts.childCount];
        for (var i = 0; i < ts.childCount; i++)
        {
            var obj = ts.GetChild(i).gameObject;
            var parts = obj.name.Split('|');
            _sprites[i] = (
                Mathf.RoundToInt((float.Parse(parts[0]) + float.Parse(parts[1])) * 10),
                obj.GetComponent<SpriteRenderer>());
        }
    }

    public override void Link(Entity entity)
    {
        base.Link(entity);
        LinkedEntity.AddGridPositionAddedListener(this);

        if(LinkedEntity.HasGridPosition())
            OnGridPositionAdded(LinkedEntity, LinkedEntity.GetGridPosition().Value);
    }

    public void OnGridPositionAdded(Map.Entity entity, Vector2Int value)
    {
        var order = (value.x + value.y) * 10;
        foreach (var (delta, sprite) in _sprites)
            sprite.sortingOrder = order + delta;
    }
}
