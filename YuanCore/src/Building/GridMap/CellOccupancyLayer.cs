using System;

namespace YuanCore.Building;

[Flags]
public enum CellOccupancyLayer
{
    None = 0,
    MainBuilding = 1 << 0,
    Water = 1 << 1,
    WaterDecoration = 1 << 2,
}
