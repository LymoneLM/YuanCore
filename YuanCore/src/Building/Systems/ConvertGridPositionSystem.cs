using System.Collections.Generic;
using Entitas;
using static YuanCore.Building.YuanCoreBuildingMapGridPositionMatcher;

namespace YuanCore.Building;

public sealed class ConvertGridPositionSystem : ReactiveSystem<Map.Entity>
{
    public ConvertGridPositionSystem(MapContext context) : base(context) { }

    protected override ICollector<Map.Entity> GetTrigger(IContext<Map.Entity> context)
        => context.CreateCollector(GridPosition);

    protected override bool Filter(Map.Entity entity)
        => entity.HasGridPosition() && entity.HasWorldPosition();

    protected override void Execute(List<Map.Entity> entities)
    {
        foreach (var entity in entities)
        {
            entity.ReplaceWorldPosition(PositionConvertor.GridToWorld(entity.GetGridPosition().Value));
        }
    }
}
