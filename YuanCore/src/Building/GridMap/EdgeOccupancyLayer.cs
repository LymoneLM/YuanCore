using System;

namespace YuanCore.Building;

[Flags]
public enum EdgeOccupancyLayer
{
    None = 0,
    EdgeBuilding = 1 << 0,
}
