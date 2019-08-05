using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using HyperV.Extensions;

namespace HyperV
{
    public partial class VMMS : IDisposable
    {
        public VMMS()
        {
            host = Environment.MachineName;
            virtualizationScope = new ManagementScope(@"\ROOT\virtualization\v2");
            hgsScope = new ManagementScope(@"\ROOT\Microsoft\Windows\Hgs");
            vmms = GetVirtualMachineManagementService();
            ss = GetSecurityService();
            ims = GetImageManagementService();
        }

        public VMMS(string host)
        {
            this.host = host.ToUpper();
            virtualizationScope = new ManagementScope($@"\\{host}\ROOT\virtualization\v2");
            hgsScope = new ManagementScope($@"\\{host}\ROOT\Microsoft\Windows\Hgs");
            vmms = GetVirtualMachineManagementService();
            ss = GetSecurityService();
            ims = GetImageManagementService();
        }

        public VMMS(string host, ConnectionOptions connectionOptions)
        {
            this.host = host.ToUpper();
            virtualizationScope = new ManagementScope($@"\\{host}\ROOT\virtualization\v2", connectionOptions);
            hgsScope = new ManagementScope($@"\\{host}\ROOT\Microsoft\Windows\Hgs", connectionOptions);
            vmms = GetVirtualMachineManagementService();
            ss = GetSecurityService();
            ims = GetImageManagementService();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    vmms.Dispose();
                    ss.Dispose();
                    ims.Dispose();
                }

                disposed = true;
            }
        }

        ~VMMS()
        {
            Dispose(false);
        }

        public void CreateVirtualMachine(VirtualMachineDefinition virtualMachineDefinition)
        {
            //==================================================================================
            // Duplication Checking
            //==================================================================================

            if (ExistsVirtualMachine(virtualMachineDefinition.Name))
                throw new ArgumentException("Virtual Machine already exists.");

            foreach (ScsiController scsiController in virtualMachineDefinition.ScsiControllers)
                foreach (IScsiDrive drive in scsiController.Drives)
                    if (drive?.GetType() == typeof(VirtualHardDrive))
                        if (!String.IsNullOrWhiteSpace(((VirtualHardDrive)drive).VirtualHardDisk.Path))
                            if (File.Exists($@"\\{host}\{((VirtualHardDrive)drive).VirtualHardDisk.Path.Replace(':', '$')}"))
                                throw new ArgumentException("Virtual Hard Disk already exists.");

            foreach (NetworkAdapter networkAdapter in virtualMachineDefinition.NetworkAdapters)
                if (!String.IsNullOrWhiteSpace(networkAdapter.VirtualSwitch))
                    if (!ExistsVirtualSwitch(networkAdapter.VirtualSwitch))
                        throw new ArgumentException("Virtual Switch does not exist.");

            //==================================================================================
            // General Configuration
            //==================================================================================

            ManagementObject systemSettings = CreateSettings(Settings.System);
            ManagementObject memoryResource = CreateResource(Resources.Memory);
            ManagementObject processorResource = CreateResource(Resources.Processor);

            //----------------------------------------------------------------------------------

            // Virtual Machine Name
            systemSettings["ElementName"] = virtualMachineDefinition.Name;

            // Virtual Machine Location
            systemSettings["ConfigurationDataRoot"] = virtualMachineDefinition.Path;

            // Virtual Machine Generation
            systemSettings["VirtualSystemSubtype"] = "Microsoft:Hyper-V:SubType:2"; // Generation 2

            //==================================================================================
            // Secure Boot Configuration
            //==================================================================================

            //----------------------------------------------------------------------------------
            // Secure Boot
            //----------------------------------------------------------------------------------

            // Enable Secure Boot
            systemSettings["SecureBootEnabled"] = virtualMachineDefinition.Security.SecureBoot;

            if (virtualMachineDefinition.Security.SecureBoot)
            {
                // Template
                systemSettings["SecureBootTemplateId"] = virtualMachineDefinition.Security.SecureBootTemplate;
            }

            //==================================================================================
            // Memory Configuration
            //==================================================================================

            // RAM (MB)
            memoryResource["VirtualQuantity"] = virtualMachineDefinition.Memory.Startup;

            //----------------------------------------------------------------------------------
            // Dynamic Memory
            //----------------------------------------------------------------------------------

            if (virtualMachineDefinition.Memory.Minimum > 0 &&
                virtualMachineDefinition.Memory.Maximum > 0 &&
                virtualMachineDefinition.Memory.Minimum <= virtualMachineDefinition.Memory.Startup &&
                virtualMachineDefinition.Memory.Maximum >= virtualMachineDefinition.Memory.Startup)
            {
                // Enable Dynamic Memory
                memoryResource["DynamicMemoryEnabled"] = true;

                // Disable Virtual Non-Uniform Memory Access (NUMA) Nodes
                systemSettings["VirtualNumaEnabled"] = false;

                // Minimum RAM (MB)
                memoryResource["Reservation"] = virtualMachineDefinition.Memory.Minimum;

                // Maximum RAM (MB)
                memoryResource["Limit"] = virtualMachineDefinition.Memory.Maximum;

                // Memory Buffer (Percent)
                memoryResource["TargetMemoryBuffer"] = virtualMachineDefinition.Memory.Buffer;
            }
            else
            {
                // Disable Dynamic Memory
                memoryResource["DynamicMemoryEnabled"] = false;

                // Enable Virtual Non-Uniform Memory Access (NUMA) Nodes
                systemSettings["VirtualNumaEnabled"] = true;
            }

            //----------------------------------------------------------------------------------
            // Memory Weight
            //----------------------------------------------------------------------------------

            // Memory Weight (0, 1250, 2500, 3750, 5000, 6250, 7500, 8750, 10000)
            memoryResource["Weight"] = virtualMachineDefinition.Memory.Weight;

            //==================================================================================
            // Processor Configuration
            //==================================================================================

            // Number Of Virtual Processors
            processorResource["VirtualQuantity"] = virtualMachineDefinition.Processor.Quantity;

            //----------------------------------------------------------------------------------
            // Resource Control
            //----------------------------------------------------------------------------------

            // Virtual Machine Reserve (Percentage) * 1000 [0 - 100000]
            processorResource["Reservation"] = (UInt64)(virtualMachineDefinition.Processor.Reservation * 1000);

            // Virtual Machine Limit (Percentage) * 1000 [0 - 100000]
            processorResource["Limit"] = (UInt64)(virtualMachineDefinition.Processor.Limit * 1000);

            // Relative Weight
            processorResource["Weight"] = virtualMachineDefinition.Processor.Weight;

            //----------------------------------------------------------------------------------
            // Processor Comptability
            //----------------------------------------------------------------------------------

            // Migrate To A Physical Computer With A Different Processor Version
            processorResource["LimitProcessorFeatures"] = virtualMachineDefinition.Processor.LimitFeatures;

            //----------------------------------------------------------------------------------
            // NUMA Topology
            //----------------------------------------------------------------------------------

            // Maximum Number of Processors
            processorResource["MaxProcessorsPerNumaNode"] = virtualMachineDefinition.Processor.NumaProcessorsPerNode;

            // Maximum Amount of Memory (MB)
            memoryResource["MaxMemoryBlocksPerNumaNode"] = virtualMachineDefinition.Processor.NumaMemoryPerNode;

            // Maximum NUMA Nodes Allowed On A Socket
            processorResource["MaxNumaNodesPerSocket"] = virtualMachineDefinition.Processor.NumaNodesPerSocket;

            //==================================================================================
            // Name Configuration
            //==================================================================================

            // Notes
            systemSettings["Notes"] = new string[] { virtualMachineDefinition.Notes };

            //==================================================================================
            // Checkpoints Configuration
            //==================================================================================

            // Checkpoint Type
            switch (virtualMachineDefinition.Checkpoints.Type)
            {
                case CheckpointType.None:
                    systemSettings["UserSnapshotType"] = (UInt16)2; // Disabled
                    break;

                case CheckpointType.Production:
                    if (virtualMachineDefinition.Checkpoints.Fallback)
                        systemSettings["UserSnapshotType"] = (UInt16)3; // ProductionFallbackToTest
                    else
                        systemSettings["UserSnapshotType"] = (UInt16)4; // ProductionNoFallback
                    break;

                case CheckpointType.Standard:
                    systemSettings["UserSnapshotType"] = (UInt16)5; // Test
                    break;
            }

            // Checkpoint File Location
            systemSettings["SnapshotDataRoot"] = virtualMachineDefinition.Checkpoints.Path;

            //==================================================================================
            // Smart Paging File Location Configuration
            //==================================================================================

            // Smart Paging File Location
            systemSettings["SwapFileDataRoot"] = virtualMachineDefinition.SmartPaging.Path;

            //==================================================================================
            // Automatic Start Action Configuration
            //==================================================================================

            // Automatic Start Action
            systemSettings["AutomaticStartupAction"] = virtualMachineDefinition.AutomaticStart.Action;

            if (virtualMachineDefinition.AutomaticStart.Action != AutomaticStartAction.Nothing)
            {
                // Startup Delay
                systemSettings["AutomaticStartupActionDelay"] = ManagementDateTimeConverter.ToDmtfTimeInterval(TimeSpan.FromSeconds(virtualMachineDefinition.AutomaticStart.Delay));
            }

            //==================================================================================
            // Automatic Stop Action Configuration
            //==================================================================================

            // Automatic Stop Action
            systemSettings["AutomaticShutdownAction"] = virtualMachineDefinition.AutomaticStop.Action;

            //==================================================================================
            // Create Virtual Machine
            //==================================================================================

            DefineSystem(systemSettings, new ManagementObject[] { processorResource, memoryResource }, out ManagementObject virtualMachine);

            //==================================================================================
            // Cleanup
            //==================================================================================

            systemSettings.Dispose();
            memoryResource.Dispose();
            processorResource.Dispose();

            //==================================================================================
            // Get Virtual Machine Settings
            //==================================================================================

            systemSettings = GetRelatedSettings(virtualMachine, Settings.System);

            //==================================================================================
            // Configure SCSI Controllers
            //==================================================================================

            foreach (ScsiController scsiControllerDefinition in virtualMachineDefinition.ScsiControllers)
            {
                ManagementObject scsiControllerResource = CreateResource(Resources.SCSIController);
                AddResourceSettings(systemSettings, new ManagementObject[] { scsiControllerResource }, out ManagementObject[] scsiControllers);
                ManagementObject scsiController = scsiControllers[0];

                //----------------------------------------------------------------------------------

                for (UInt16 address = 0; address < scsiControllerDefinition.Drives.Length; address++)
                {
                    switch (scsiControllerDefinition.Drives[address])
                    {
                        case VirtualHardDrive virtualHardDriveDefinition:
                            //==================================================================================
                            // Create Hard Drive
                            //==================================================================================

                            ManagementObject virtualHardDriveResource = CreateResource(Resources.VirtualHardDrive);
                            virtualHardDriveResource["Parent"] = scsiController.Path.Path; // Scsi Controller
                            virtualHardDriveResource["AddressOnParent"] = address; // Port
                            AddResourceSettings(systemSettings, new ManagementObject[] { virtualHardDriveResource }, out ManagementObject[] virtualHardDrives);
                            ManagementObject virtualHardDrive = virtualHardDrives[0];

                            //----------------------------------------------------------------------------------

                            if (virtualHardDriveDefinition.VirtualHardDisk != null)
                            {
                                //==================================================================================
                                // Create Virtual Hard Disk
                                //==================================================================================

                                ManagementObject virtualHardDiskSettings = CreateSettings(Settings.VirtualHardDisk);
                                virtualHardDiskSettings["Type"] = virtualHardDriveDefinition.VirtualHardDisk.Type;
                                virtualHardDiskSettings["Format"] = virtualHardDriveDefinition.VirtualHardDisk.Format;
                                virtualHardDiskSettings["MaxInternalSize"] = (UInt64)(virtualHardDriveDefinition.VirtualHardDisk.Size * 1073741824); // Bytes
                                virtualHardDiskSettings["Path"] = virtualHardDriveDefinition.VirtualHardDisk.Path;
                                CreateVirtualHardDisk(virtualHardDiskSettings);
                                virtualHardDiskSettings.Dispose();

                                //==================================================================================
                                // Attach Virtual Hard Disk
                                //==================================================================================

                                ManagementObject virtualHardDiskResource = CreateResource(Resources.VirtualHardDisk);
                                virtualHardDiskResource["Parent"] = virtualHardDrive.Path.Path;
                                virtualHardDiskResource["HostResource"] = new string[] { virtualHardDriveDefinition.VirtualHardDisk.Path };

                                //----------------------------------------------------------------------------------
                                // Configure Quality of Service
                                //----------------------------------------------------------------------------------

                                if ((virtualHardDriveDefinition.MinimumIOPS > 0 || virtualHardDriveDefinition.MaximumIOPS > 0) &&
                                    virtualHardDriveDefinition.MaximumIOPS >= virtualHardDriveDefinition.MinimumIOPS)
                                {
                                    // Minimum IOPS
                                    virtualHardDiskResource["IOPSReservation"] = virtualHardDriveDefinition.MinimumIOPS;

                                    // Maximum IOPS
                                    virtualHardDiskResource["IOPSLimit"] = virtualHardDriveDefinition.MaximumIOPS;
                                }

                                //----------------------------------------------------------------------------------

                                AddResourceSettings(systemSettings, new ManagementObject[] { virtualHardDiskResource }, out _);

                                //----------------------------------------------------------------------------------

                                virtualHardDiskResource.Dispose();
                                virtualHardDrives.Dispose();
                                virtualHardDrive.Dispose();
                            }

                            //----------------------------------------------------------------------------------

                            virtualHardDriveResource.Dispose();
                            break;

                        case VirtualDvdDrive virtualDvdDriveDefinition:
                            //==================================================================================
                            // Create DVD Drive
                            //==================================================================================

                            ManagementObject virtualDvdDriveResource = CreateResource(Resources.VirtualDvdDrive);
                            virtualDvdDriveResource["Parent"] = scsiController.Path.Path; // Scsi Controller
                            virtualDvdDriveResource["AddressOnParent"] = address; // Port
                            AddResourceSettings(systemSettings, new ManagementObject[] { virtualDvdDriveResource }, out ManagementObject[] virtualDvdDrives);
                            ManagementObject virtualDvdDrive = virtualDvdDrives[0];

                            //----------------------------------------------------------------------------------

                            if (virtualDvdDriveDefinition.VirtualDvdDisk != null)
                            {
                                //==================================================================================
                                // Attach Virtual DVD Disk
                                //==================================================================================

                                ManagementObject virtualDvdDiskResource = CreateResource(Resources.VirtualDvdDisk);
                                virtualDvdDiskResource["Parent"] = virtualDvdDrive.Path.Path;
                                virtualDvdDiskResource["HostResource"] = new string[] { virtualDvdDriveDefinition.VirtualDvdDisk.Path };
                                AddResourceSettings(systemSettings, new ManagementObject[] { virtualDvdDiskResource }, out _);

                                //----------------------------------------------------------------------------------

                                virtualDvdDiskResource.Dispose();
                                virtualDvdDrives.Dispose();
                                virtualDvdDrive.Dispose();
                            }

                            //----------------------------------------------------------------------------------

                            virtualDvdDriveResource.Dispose();
                            break;
                    }
                }

                //----------------------------------------------------------------------------------

                scsiControllerResource.Dispose();
                scsiControllers.Dispose();
                scsiController.Dispose();
            }

            //==================================================================================
            // Configure Network Adapters
            //==================================================================================

            foreach (NetworkAdapter networkAdapterDefinition in virtualMachineDefinition.NetworkAdapters)
            {
                //==================================================================================
                // Create Network Adapter
                //==================================================================================

                ManagementObject networkAdapterResource = CreateResource(Resources.NetworkAdapter);
                networkAdapterResource["ElementName"] = "Network Adapter";

                //----------------------------------------------------------------------------------
                // Configure MAC Address
                //----------------------------------------------------------------------------------

                if (networkAdapterDefinition.MacAddress != null)
                {
                    networkAdapterResource["StaticMacAddress"] = true;
                    networkAdapterResource["Address"] = networkAdapterDefinition.MacAddress.ToString();
                }

                //----------------------------------------------------------------------------------
                // Configure Protected Network
                //----------------------------------------------------------------------------------

                networkAdapterResource["ClusterMonitored"] = networkAdapterDefinition.ProtectedNetwork;

                //----------------------------------------------------------------------------------
                // Configure Device Naming
                //----------------------------------------------------------------------------------

                networkAdapterResource["DeviceNamingEnabled"] = networkAdapterDefinition.DeviceNaming;

                //----------------------------------------------------------------------------------

                AddResourceSettings(systemSettings, new ManagementObject[] { networkAdapterResource }, out ManagementObject[] networkAdapters);
                ManagementObject networkAdapter = networkAdapters[0];

                //==================================================================================
                // Configure Ethernet Port
                //==================================================================================

                ManagementObject switchPortResource = CreateResource(Resources.SwitchPort);
                switchPortResource["ElementName"] = "Dynamic Ethernet Switch Port";
                switchPortResource["Parent"] = networkAdapter.Path.Path; // Network Adapter

                //----------------------------------------------------------------------------------
                // Connect Virtual Switch
                //----------------------------------------------------------------------------------

                if (!String.IsNullOrWhiteSpace(networkAdapterDefinition.VirtualSwitch))
                {
                    ManagementObject virtualSwitch = GetVirtualSwitch(networkAdapterDefinition.VirtualSwitch);
                    switchPortResource["HostResource"] = new string[] { virtualSwitch.Path.Path }; // Virtual Switch
                    virtualSwitch.Dispose();
                }
                else
                {
                    switchPortResource["EnabledState"] = 3; // Disabled
                }

                //----------------------------------------------------------------------------------

                AddResourceSettings(systemSettings, new ManagementObject[] { switchPortResource }, out ManagementObject[] switchPorts);
                ManagementObject switchPort = switchPorts[0];

                //==================================================================================
                // Configure VLAN
                //==================================================================================

                if (networkAdapterDefinition.VlanId > 0)
                {
                    ManagementObject portVlanSettings = CreateFeatureSettings(Features.Vlan);
                    portVlanSettings["OperationMode"] = 1; // Access
                    portVlanSettings["AccessVlanId"] = networkAdapterDefinition.VlanId;
                    AddFeatureSettings(switchPort, new ManagementObject[] { portVlanSettings }, out _);
                    portVlanSettings.Dispose();
                }

                //==================================================================================
                // Configure Quality of Service
                //==================================================================================

                if ((networkAdapterDefinition.MinimumBandwidth > 0 || networkAdapterDefinition.MaximumBandwidth > 0) &&
                    networkAdapterDefinition.MaximumBandwidth >= networkAdapterDefinition.MinimumBandwidth)
                {
                    ManagementObject portBandwidthSettings = CreateFeatureSettings(Features.Bandwidth);
                    portBandwidthSettings["Reservation"] = networkAdapterDefinition.MinimumBandwidth * 1000000;
                    portBandwidthSettings["Limit"] = networkAdapterDefinition.MaximumBandwidth * 1000000;
                    AddFeatureSettings(switchPort, new ManagementObject[] { portBandwidthSettings }, out _);
                    portBandwidthSettings.Dispose();
                }

                //==================================================================================
                // Configure Hardware Acceleration
                //==================================================================================

                ManagementObject portOffloadSettings = GetRelatedSettings(switchPort, Settings.SwitchPortOffload);

                //----------------------------------------------------------------------------------
                // Configure Virtual Machine Queue
                //----------------------------------------------------------------------------------

                if (networkAdapterDefinition.Vmq)
                    portOffloadSettings["VMQOffloadWeight"] = 100;
                else
                    portOffloadSettings["VMQOffloadWeight"] = 0;

                //----------------------------------------------------------------------------------
                // Configure IPsec Task Offloading
                //----------------------------------------------------------------------------------

                if (networkAdapterDefinition.IpsecOffloading)
                    portOffloadSettings["IPSecOffloadLimit"] = networkAdapterDefinition.IpsecSecurityAssociations;
                else
                    portOffloadSettings["IPSecOffloadLimit"] = 0;

                //----------------------------------------------------------------------------------
                // Configure SR-IOV
                //----------------------------------------------------------------------------------

                if (networkAdapterDefinition.SrIov)
                    portOffloadSettings["IOVOffloadWeight"] = 100;
                else
                    portOffloadSettings["IOVOffloadWeight"] = 0;

                //----------------------------------------------------------------------------------

                ModifyFeatureSettings(new ManagementObject[] { portOffloadSettings }, out _);
                portOffloadSettings.Dispose();

                //==================================================================================
                // Configure Advanced Features
                //==================================================================================

                ManagementObject portSecuritySettings = CreateFeatureSettings(Features.Security);

                //----------------------------------------------------------------------------------
                // Configure MAC Address Spoofing
                //----------------------------------------------------------------------------------

                portSecuritySettings["AllowMacSpoofing"] = networkAdapterDefinition.MacAddressSpoofing;

                //----------------------------------------------------------------------------------
                // Configure DHCP Guard
                //----------------------------------------------------------------------------------

                portSecuritySettings["EnableDhcpGuard"] = networkAdapterDefinition.DhcpGuard;

                //----------------------------------------------------------------------------------
                // Configure Router Advertisement Guard
                //----------------------------------------------------------------------------------

                portSecuritySettings["EnableRouterGuard"] = networkAdapterDefinition.RouterGuard;

                //----------------------------------------------------------------------------------
                // Configure Port Mirroring
                //----------------------------------------------------------------------------------

                switch (networkAdapterDefinition.PortMirroringMode)
                {
                    case PortMirroringMode.None:
                        portSecuritySettings["MonitorMode"] = 0; // None
                        break;

                    case PortMirroringMode.Destination:
                        portSecuritySettings["MonitorMode"] = 1; // Destination
                        break;

                    case PortMirroringMode.Source:
                        portSecuritySettings["MonitorMode"] = 2; // Source
                        break;
                }

                //----------------------------------------------------------------------------------
                // Configure NIC Teaming
                //----------------------------------------------------------------------------------

                portSecuritySettings["AllowTeaming"] = networkAdapterDefinition.NicTeaming;

                //----------------------------------------------------------------------------------

                AddFeatureSettings(switchPort, new ManagementObject[] { portSecuritySettings }, out _);
                portSecuritySettings.Dispose();

                //----------------------------------------------------------------------------------

                networkAdapterResource.Dispose();
                networkAdapters.Dispose();
                networkAdapter.Dispose();
                switchPortResource.Dispose();
                switchPorts.Dispose();
                switchPort.Dispose();
            }

            //==================================================================================
            // Configure Security Settings
            //==================================================================================

            ManagementObject securitySettings = GetRelatedSettings(systemSettings, Settings.Security);

            //----------------------------------------------------------------------------------
            // Encryption Support
            //----------------------------------------------------------------------------------

            // Enable Trusted Platform Module
            securitySettings["TpmEnabled"] = virtualMachineDefinition.Security.TrustedPlatformModule;

            if (virtualMachineDefinition.Security.TrustedPlatformModule)
            {
                // Encrypt state and virtual machine migration traffic
                securitySettings["EncryptStateAndVmMigrationTraffic"] = virtualMachineDefinition.Security.EncryptTraffic;
            }

            //----------------------------------------------------------------------------------
            // Security Policy
            //----------------------------------------------------------------------------------

            // Enable Shielding
            securitySettings["ShieldingRequested"] = virtualMachineDefinition.Security.Shielding;

            if (virtualMachineDefinition.Security.Shielding)
            {
                // Enable Trusted Platform Module
                securitySettings["TpmEnabled"] = virtualMachineDefinition.Security.TrustedPlatformModule;

                // Encrypt State And Virtual Machine Migration Traffic
                securitySettings["EncryptStateAndVmMigrationTraffic"] = virtualMachineDefinition.Security.EncryptTraffic;
            }

            //----------------------------------------------------------------------------------

            byte[] localKeyProtector = NewByGuardians();
            SetKeyProtector(securitySettings, localKeyProtector);
            ModifySecuritySettings(securitySettings);

            //----------------------------------------------------------------------------------

            securitySettings.Dispose();

            //==================================================================================
            // Integration Services Configuration
            //==================================================================================

            //----------------------------------------------------------------------------------
            // Configure Operating System Shutdown
            //----------------------------------------------------------------------------------

            ManagementObject shutdownSettings = GetRelatedSettings(systemSettings, Settings.Shutdown);
            if (virtualMachineDefinition.IntegrationServices.Shutdown)
                shutdownSettings["EnabledState"] = 2; // Enabled
            else
                shutdownSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { shutdownSettings }, out _);
            shutdownSettings.Dispose();

            //----------------------------------------------------------------------------------
            // Configure Time Synchronization
            //----------------------------------------------------------------------------------

            ManagementObject timeSynchronizationSettings = GetRelatedSettings(systemSettings, Settings.TimeSynchronization);
            if (virtualMachineDefinition.IntegrationServices.TimeSynchronisation)
                timeSynchronizationSettings["EnabledState"] = 2; // Enabled
            else
                timeSynchronizationSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { timeSynchronizationSettings }, out _);
            timeSynchronizationSettings.Dispose();

            //----------------------------------------------------------------------------------
            // Configure Data Exchange
            //----------------------------------------------------------------------------------

            ManagementObject dataExchangeSettings = GetRelatedSettings(systemSettings, Settings.DataExchange);
            if (virtualMachineDefinition.IntegrationServices.DataExchange)
                dataExchangeSettings["EnabledState"] = 2; // Enabled
            else
                dataExchangeSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { dataExchangeSettings }, out _);
            dataExchangeSettings.Dispose();

            //----------------------------------------------------------------------------------
            // Configure Heartbeat
            //----------------------------------------------------------------------------------

            ManagementObject heartbeatSettings = GetRelatedSettings(systemSettings, Settings.Heartbeat);
            if (virtualMachineDefinition.IntegrationServices.Heartbeat)
                heartbeatSettings["EnabledState"] = 2; // Enabled
            else
                heartbeatSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { heartbeatSettings }, out _);
            heartbeatSettings.Dispose();

            //----------------------------------------------------------------------------------
            // Configure Backup (Volume Shadow Copy)
            //----------------------------------------------------------------------------------

            ManagementObject backupSettings = GetRelatedSettings(systemSettings, Settings.VolumeShadowCopy);
            if (virtualMachineDefinition.IntegrationServices.VolumeShadowCopy)
                backupSettings["EnabledState"] = 2; // Enabled
            else
                backupSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { backupSettings }, out _);
            backupSettings.Dispose();

            //----------------------------------------------------------------------------------
            // Configure Guest Services
            //----------------------------------------------------------------------------------

            ManagementObject guestServicesSettings = GetRelatedSettings(systemSettings, Settings.GuestServices);
            if (virtualMachineDefinition.IntegrationServices.GuestServices)
                guestServicesSettings["EnabledState"] = 2; // Enabled
            else
                guestServicesSettings["EnabledState"] = 3; // Disabled
            ModifyGuestServiceSettings(new ManagementObject[] { guestServicesSettings }, out _);
            guestServicesSettings.Dispose();

            //==================================================================================
            // Cleanup
            //==================================================================================

            virtualMachine.Dispose();
            systemSettings.Dispose();
        }

        public BootEntries GetVirtualMachineBootOrder(string name)
        {
            using (ManagementObject virtualMachine = GetVirtualMachine(name))
            using (ManagementObject systemSettings = GetRelatedSettings(virtualMachine, Settings.System))
            {
                BootEntries bootEntries = new BootEntries();
                foreach (string bootEntry in (string[])systemSettings["BootSourceOrder"])
                    bootEntries.Add(new BootEntry(bootEntry));
                return bootEntries;
            }
        }

        public void SetVirtualMachineBootOrder(string name, BootEntries bootEntries)
        {
            using (ManagementObject virtualMachine = GetVirtualMachine(name))
            using (ManagementObject systemSettings = GetRelatedSettings(virtualMachine, Settings.System))
            {
                List<string> bootOrder = new List<string>();
                foreach (BootEntry bootEntry in bootEntries)
                    bootOrder.Add(bootEntry.ToString());
                systemSettings["BootSourceOrder"] = bootOrder.ToArray();

                ModifySystemSettings(systemSettings);
            }
        }

        public void StartVirtualMachine(string name)
        {
            using (ManagementObject virtualMachine = GetVirtualMachine(name))
                RequestStateChange(virtualMachine, RequestedState.Enabled);
        }
    }
}