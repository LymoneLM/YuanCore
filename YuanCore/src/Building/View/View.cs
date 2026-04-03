using Entitas;
using Entitas.Unity;
using UnityEngine;

namespace YuanCore.Building;

public class View : MonoBehaviour, IView, IYuanCoreBuildingMapWorldPositionAddedListener
{
    protected Map.Entity LinkedEntity;

    public virtual void Link(Entity entity)
    {
        gameObject.Link(entity);
        LinkedEntity = (Map.Entity)entity;

        OnWorldPositionAdded(LinkedEntity, LinkedEntity.GetWorldPosition().Value);
    }

    public void OnWorldPositionAdded(Map.Entity entity, Vector2 value)
    {
        transform.position = value;
    }

    protected virtual void OnDestroy()
    {
        gameObject.Unlink();
    }
}
