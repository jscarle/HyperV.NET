using System;

namespace HyperV
{
    public enum MemoryWeight : UInt32
    {
        Lowest = 0,
        Lower = 1250,
        Low = 2500,
        BalancedLow = 3750,
        Balanced = 5000,
        BalancedHigh = 6250,
        High = 7500,
        Higher = 8750,
        Highest = 10000
    }
}