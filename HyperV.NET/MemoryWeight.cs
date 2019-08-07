using System;

namespace HyperV
{
    ///<summary>Defines the priority when balancing memory availability compared to other virtual machines.</summary>
    public enum MemoryWeight : UInt32
    {
        ///<summary>The lowest priority possible.</summary>
        Lowest = 0,

        ///<summary>The priority is lower than low.</summary>
        Lower = 1250,

        ///<summary>The priority is low.</summary>
        Low = 2500,

        ///<summary>The priority is lower than balanced.</summary>
        BalancedLow = 3750,

        ///<summary>The priority is balanced.</summary>
        Balanced = 5000,

        ///<summary>The priority is higher than balanced.</summary>
        BalancedHigh = 6250,

        ///<summary>The priority is high.</summary>
        High = 7500,

        ///<summary>The priority is higher than high.</summary>
        Higher = 8750,

        ///<summary>The highest priority possible.</summary>
        Highest = 10000
    }
}