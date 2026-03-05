namespace YuanCore.Building;

public record struct EdgeOccupant(int EntityId, Direction OccupiedAs, EdgeOccupancyLayer EdgeOccupancyLayer);

public struct EdgeData
{
    public EdgeOccupancyLayer EdgeOccupancyLayer { get; private set; }

    private bool _hasOccupant;
    private EdgeOccupant _occupant;

    public void Add(EdgeOccupant occupant)
    {
        if (_hasOccupant)
            throw new System.InvalidOperationException("EdgeData already contains one occupant.");

        _occupant = occupant;
        _hasOccupant = true;
        EdgeOccupancyLayer = occupant.EdgeOccupancyLayer;
    }

    public void Remove(EdgeOccupant occupant)
    {
        if (!_hasOccupant)
            return;

        if (!_occupant.Equals(occupant))
            return;

        _occupant = default;
        _hasOccupant = false;
        EdgeOccupancyLayer = EdgeOccupancyLayer.None;
    }

    public EdgeOccupant[] Get()
    {
        return _hasOccupant ? [_occupant] : [];
    }
}
