using System;

namespace HyperV
{
    ///<summary>Defines a Network Adapter.</summary>
    public class NetworkAdapter
    {
        ///<summary>Propagate the name of the network adapter into the guest operating system.</summary>
        public bool DeviceNaming { get; set; }

        ///<summary>Drop DHCP server messages from unauthorized virtual machines pretending to be DHCP servers.</summary>
        public bool DhcpGuard { get; set; }

        ///<summary>Enable IPsec task offloading.</summary>
        public bool IpsecOffloading { get; set; }

        private UInt16 ipsecSecurityAssociations;

        ///<summary>The maximum number of offloaded security associations.</summary>
        ///<value>The number must be between 1 and 4094.</value>
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

        ///<summary>The static MAC address.</summary>
        public MacAddress MacAddress { get; set; }

        ///<summary>Enable MAC address spoofing.</summary>
        public bool MacAddressSpoofing { get; set; }

        private UInt64 maximumBandwidth;

        ///<summary>The maximum amount of network bandwidth in Mbps.</summary>
        ///<value>The amount must be between 0 and 999999999.</value>
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

        ///<summary>The minimum amount of network bandwidth in Mbps.</summary>
        ///<value>The amount must be between 0 and 999999999.</value>
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

        ///<summary>Enable this network adapter to be part of a team in the guest operating system.</summary>
        public bool NicTeaming { get; set; }

        /// <summary>The port mirroring mode for this network adapter.</summary>
        public PortMirroringMode PortMirroringMode { get; set; }

        ///<summary>Move this virtual machine to another cluster node if a network disconnection is detected.</summary>
        public bool ProtectedNetwork { get; set; }

        ///<summary>Drops router advertisement and redirection messages from unauthorized virtual machines pretending to be routers.</summary>
        public bool RouterGuard { get; set; }

        ///<summary>Enable SR-IOV.</summary>
        public bool SrIov { get; set; }

        ///<summary>The virtual switch to attach.</summary>
        public string VirtualSwitch { get; set; }

        ///<summary>Enable virtual LAN identification.</summary>
        public bool Vlan { get; set; }

        private UInt16 vlanId;

        ///<summary>The VLAN identifier.</summary>
        ///<value>The identifier must be between 1 and 4094.</value>
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

        ///<summary>Enable virtual machine queue.</summary>
        public bool Vmq { get; set; }

        ///<summary>Initializes a new instance of the <see cref="NetworkAdapter"/> class.</summary>
        public NetworkAdapter()
        {
            IpsecOffloading = true;
            IpsecSecurityAssociations = 512;
            PortMirroringMode = PortMirroringMode.None;
            ProtectedNetwork = true;
            Vmq = true;
        }

        ///<summary>Initializes a new instance of the <see cref="NetworkAdapter"/> class with the specified switch already attached.</summary>
        ///<param name="virtualSwitch">The virtual switch to attach.</param>
        public NetworkAdapter(string virtualSwitch) : this()
        {
            VirtualSwitch = virtualSwitch;
        }
    }
}