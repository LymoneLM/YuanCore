namespace YuanCore.Building;

public readonly record struct CellOccupant(int EntityId, CellOccupancyLayer CellOccupancyLayer);

public struct CellData
{
    public CellOccupancyLayer CellOccupancyLayer { get; private set; }

    private int _count;
    private CellOccupant _occupant1;
    private CellOccupant _occupant2;

    public void Add(CellOccupant occupant)
    {
        switch (_count)
        {
            case 2:
                throw new System.InvalidOperationException("CellData already contains two occupants.");
            case 1:
                _occupant2 = occupant;
                break;
            case 0:
                _occupant1 = occupant;
                break;
        }
        ++_count;
        CellOccupancyLayer |= occupant.CellOccupancyLayer;
    }

    public void Remove(CellOccupant occupant)
    {
        switch (_count)
        {
            case 2:
                if (_occupant2.Equals(occupant))
                {
                    _occupant2 = default;
                    break;
                }
                if (_occupant1.Equals(occupant))
                {
                    _occupant1 = _occupant2;
                    _occupant2 = default;
                    break;
                }
                return;
            case 1:
                if (_occupant1.Equals(occupant))
                {
                    _occupant1 = default;
                    break;
                }
                return;
            case 0:
                return;
        }
        --_count;
        RebuildLayer();
    }

    public CellOccupant[] Get()
    {
        return _count switch
        {
            0 => [],
            1 => [_occupant1],
            2 => [_occupant1, _occupant2],
            _ => throw new System.InvalidOperationException("Invalid CellData count.")
        };
    }

    private void RebuildLayer()
    {
        CellOccupancyLayer = CellOccupancyLayer.None;

        switch (_count)
        {
            case 2:
                CellOccupancyLayer |= _occupant2.CellOccupancyLayer;
                goto case 1;
            case 1:
                CellOccupancyLayer |= _occupant1.CellOccupancyLayer;
                break;
            default:
                throw new System.InvalidOperationException("Invalid CellData count.");
        }
    }
}
