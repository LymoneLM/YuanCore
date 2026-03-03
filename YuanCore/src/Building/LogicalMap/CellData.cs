using System.Collections.Generic;

namespace YuanCore.Building;

public record struct CellOccupant(int EntityId, CellOccupancyLayer CellOccupancyLayer);

public struct CellData()
{
    public CellOccupancyLayer CellOccupancyLayer { get; private set; }

    public int Count { get; private set; } = 0;

    // 使用List会导致创建万数List对象，有分配和GC问题风险
    // 后续应该使用更优的策略或者数据结构
    private List<CellOccupant> _list = [];

    public void Add(CellOccupant cellOccupant)
    {
        ++Count;
        _list.Add(cellOccupant);
        CellOccupancyLayer |= cellOccupant.CellOccupancyLayer;
    }

    public void Remove(CellOccupant cellOccupant)
    {
        --Count;
        CellOccupancyLayer = CellOccupancyLayer.None;
        _list.Remove(cellOccupant);
        foreach (var o in _list)
            CellOccupancyLayer |= o.CellOccupancyLayer;
    }

}
