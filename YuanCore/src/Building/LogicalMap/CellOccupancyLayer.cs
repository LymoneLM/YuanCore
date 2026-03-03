using System;

namespace YuanCore.Building;

[Flags]
public enum CellOccupancyLayer
{
    None = 0,
    MainBuilding = 1 << 0,
    Water = 1 << 1,
    WaterBuilding = 1 << 2,
    Road = 1 << 3,
}
