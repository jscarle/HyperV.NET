using System;

namespace HyperV
{
    public class NetworkAdapter
    {
        public bool DeviceNaming { get; set; }
        public bool DhcpGuard { get; set; }
        public bool IpsecOffloading { get; set; }
        private UInt16 ipsecSecurityAssociations;

        public UInt16 IpsecSecurityAssociations
        {
            get { return ipsecSecurityAssociations; }
            set
            {
                if (value < 1 || value > 4094)
                    throw new ArgumentOutOfRangeException($"{nameof(IpsecSecurityAssociations)} must be between 1 and 4094.");
                IpsecOffloading = true;
                ipsecSecurityAssociations = value;
            }
        }

        public MacAddress MacAddress { get; set; }
        public bool MacAddressSpoofing { get; set; }
        private UInt64 maximumBandwidth;

        public UInt64 MaximumBandwidth
        {
            get { return maximumBandwidth; }
            set
            {
                if (value > 999999999)
                    throw new ArgumentOutOfRangeException($"{nameof(MaximumBandwidth)} must be between 0 and 999999999.");
                maximumBandwidth = value;
            }
        }

        private UInt64 minimumBandwidth;

        public UInt64 MinimumBandwidth
        {
            get { return minimumBandwidth; }
            set
            {
                if (value > 999999999)
                    throw new ArgumentOutOfRangeException($"{nameof(MinimumBandwidth)} must be between 0 and 999999999.");
                minimumBandwidth = value;
            }
        }

        public bool NicTeaming { get; set; }
        public PortMirroringMode PortMirroringMode { get; set; }
        public bool ProtectedNetwork { get; set; }
        public bool RouterGuard { get; set; }
        public bool SrIov { get; set; }
        public string VirtualSwitch { get; set; }
        private UInt16 vlanId;

        public UInt16 VlanId
        {
            get { return vlanId; }
            set
            {
                if (value < 1 || value > 4094)
                    throw new ArgumentOutOfRangeException($"{nameof(VlanId)} must be between 1 and 4094.");
                vlanId = value;
            }
        }

        public bool Vmq { get; set; }

        public NetworkAdapter()
        {
            IpsecOffloading = true;
            IpsecSecurityAssociations = 512;
            PortMirroringMode = PortMirroringMode.None;
            ProtectedNetwork = true;
            Vmq = true;
        }

        public NetworkAdapter(string virtualSwitch) : this()
        {
            VirtualSwitch = virtualSwitch;
        }
    }
}