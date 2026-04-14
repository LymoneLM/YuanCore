using Entitas;
using UnityEngine;

namespace YuanCore.Building;

public class BuildingView : View , IYuanCoreBuildingMapGridPositionAddedListener
{
    protected (int, SpriteRenderer)[] Sprites;

    protected virtual void Awake()
    {
        var ts = transform.Find("Build");
        Sprites = new (int, SpriteRenderer)[ts.childCount];
        for (var i = 0; i < ts.childCount; i++)
        {
            var obj = ts.GetChild(i).gameObject;
            var parts = obj.name.Split('|');
            Sprites[i] = (
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
        foreach (var (delta, sprite) in Sprites)
            sprite.sortingOrder = order + delta;
    }
}
