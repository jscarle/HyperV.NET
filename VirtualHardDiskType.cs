using System;

namespace HyperV
{
    public enum VirtualHardDiskType : UInt16
    {
        FixedSize = 2,
        DynamicallyExpanding = 3,
        Differencing = 4
    }
}