using System;

namespace HyperV
{
    public class VirtualHardDrive : IScsiDrive
    {
        private UInt64 maximumIOPS;

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

        public VirtualHardDisk VirtualHardDisk { get; set; }

        public VirtualHardDrive()
        {
        }

        public VirtualHardDrive(VirtualHardDisk virtualHardDisk)
        {
            VirtualHardDisk = virtualHardDisk;
        }

        public VirtualHardDrive(VirtualHardDisk virtualHardDisk, UInt64 minimumIOPS, UInt64 maximumIOPS)
        {
            VirtualHardDisk = virtualHardDisk;
            MinimumIOPS = minimumIOPS;
            MaximumIOPS = maximumIOPS;
        }
    }
}