using System;

namespace HyperV
{
    ///<summary>Defines the type of virtual hard disk.</summary>
    public enum VirtualHardDiskType : UInt16
    {
        ///<summary>Space for fixed virtual hard disks is first allocated when the file is created.</summary>
        FixedSize = 2,

        ///<summary>Space for dynamically expanding virtual hard disks is allocated on demand.</summary>
        DynamicallyExpanding = 3

        // Differencing disks are not supported by this library.
        // Differencing = 4
    }
}