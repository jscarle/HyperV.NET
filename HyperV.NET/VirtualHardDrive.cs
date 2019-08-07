using System;

namespace HyperV
{
    ///<summary>Defines a virtual hard drive.</summary>
    public class VirtualHardDrive : IScsiDrive
    {
        private UInt64 maximumIOPS;

        ///<summary>The maximum number of 8 KB IOPS.</summary>
        ///<value>The number must be between 0 and 1000000000.</value>
        public UInt64 MaximumIOPS
        {
            get { return maximumIOPS; }
            set
            {
                if (value < 0 || value > 1000000000)
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumIOPS)} must be between 0 and 1000000000.");
                maximumIOPS = value;
            }
        }

        private UInt64 minimumIOPS;

        ///<summary>The minimum number of 8 KB IOPS.</summary>
        ///<value>The number must be between 0 and 1000000000.</value>
        public UInt64 MinimumIOPS
        {
            get { return minimumIOPS; }
            set
            {
                if (value < 0 || value > 1000000000)
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumIOPS)} must be between 0 and 1000000000.");
                minimumIOPS = value;
            }
        }

        ///<summary>The attached virtual hard disk.</summary>
        public VirtualHardDisk VirtualHardDisk { get; set; }

        ///<summary>Initializes a new instance of the <see cref="VirtualHardDrive"/> class.</summary>
        public VirtualHardDrive()
        {
        }

        ///<summary>Initializes a new instance of the <see cref="VirtualHardDrive"/> class with the specified disk already attached.</summary>
        ///<param name="virtualHardDisk">The virtual hard disk to attach.</param>
        public VirtualHardDrive(VirtualHardDisk virtualHardDisk)
        {
            VirtualHardDisk = virtualHardDisk;
        }

        ///<summary>Initializes a new instance of the <see cref="VirtualHardDrive"/> class using the specified quality of service settings with the specified disk already attached.</summary>
        ///<param name="virtualHardDisk">The virtual hard disk to attach.</param>
        ///<param name="minimumIOPS">The minimum number of 8 KB IOPS.</param>
        ///<param name="maximumIOPS">The maximum number of 8 KB IOPS.</param>
        public VirtualHardDrive(VirtualHardDisk virtualHardDisk, UInt64 minimumIOPS, UInt64 maximumIOPS)
        {
            VirtualHardDisk = virtualHardDisk;
            MinimumIOPS = minimumIOPS;
            MaximumIOPS = maximumIOPS;
        }
    }
}