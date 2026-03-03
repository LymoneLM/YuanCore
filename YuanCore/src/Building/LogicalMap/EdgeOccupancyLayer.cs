using System;

namespace YuanCore.Building;

[Flags]
public enum EdgeOccupancyLayer
{
    None = 0,
    Wall = 1 << 0,
    Gate = 1 << 1,
    Building = 1 << 2,
    NoBuild = 1 << 3
}
