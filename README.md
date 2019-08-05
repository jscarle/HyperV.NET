# HyperV.NET - Simple Hyper-V Virtual Machine Creation
The primary design goal of the HyperV.NET library is to abstract Hyper-V's WMI provider and present a simplified and strongly typed .NET implementation for deploying Hyper-V Virtual Machines. Much of the underlying mechanics have therefore purposefully been abstracted behind private or internal modifiers.

## References
This library targets .NET Framework 4.7.2 and references the System and System.Management namespaces only.

## Supported Generations
Only Generation **2** virtual machines can be created.

## Compatiblity Testing
This has only been tested on Windows Server 2016 at the moment.

## Configurable Settings
Nearly all of the configurable settings found in the standard Hyper-V management interface are available when creating a new virtual machine using this library. The following table describes which settings from that dialog can be configured.

| Setting | Available | Notes |
| --- | --- | --- |
| | | |
| **General** | | |
| Name | :heavy_check_mark: Yes | |
| Notes | :heavy_check_mark: Yes | |
| Configuration Path | :heavy_check_mark: Yes | |
| | | |
| **Add Hardware** | | |
| SCSI Controller | :heavy_check_mark: Yes | :information_source: Up to 4 SCSI Controllers can be added. |
| Network Adapter | :heavy_check_mark: Yes | :information_source: Up to 8 Network Adapters can be added. |
| RemoteFX 3D Video Adapter | :x: No | |
| Fibre Channel Adapter | :x: No | |
| | | |
| **Firmware** | | |
| Boot Order | :heavy_check_mark: Yes | :information_source: Can be configured after the virtual machine is created. |
| | | |
| **Security** | | |
| Secure Boot | :heavy_check_mark: Yes | |
| Secure Boot Template | :heavy_check_mark: Yes | |
| Trusted Platform Module | :heavy_check_mark: Yes | |
| Encrypt Traffic | :heavy_check_mark: Yes | |
| Shielding | :heavy_check_mark: Yes | |
| | | |
| **Memory** | | |
| Startup RAM | :heavy_check_mark: Yes | |
| Dynamic Memory | :heavy_check_mark: Yes | |
| Minimum RAM | :heavy_check_mark: Yes | |
| Maximum RAM | :heavy_check_mark: Yes | |
| Memory Buffer | :heavy_check_mark: Yes | |
| Memory Weight | :heavy_check_mark: Yes | |
| | | |
| **Processor** | | |
| Number of Processors | :heavy_check_mark: Yes | |
| Processor Reservation | :heavy_check_mark: Yes | |
| Processor Limit | :heavy_check_mark: Yes | |
| Processor Weight | :heavy_check_mark: Yes | |
| Processor Compatibility | :heavy_check_mark: Yes | |
| NUMA Topology: Processors | :heavy_check_mark: Yes | |
| NUMA Topology: Memory | :heavy_check_mark: Yes | |
| NUMA Topology: Nodes | :heavy_check_mark: Yes | |
| | | |
| **SCSI Controller** | | :information_source: Up to 64 devices per SCSI Controller can be added. |
| | | |
| **Hard Drive** | | |
| Controller | :heavy_check_mark: Yes | |
| Location | :heavy_check_mark: Yes | |
| Media: Virtual Hard Disk | :heavy_check_mark: Yes | |
| Media: Physical Hard Disk | :x: No | |
| Quality of Service | :heavy_check_mark: Yes | |
| Minimum IOPS | :heavy_check_mark: Yes | |
| Maximum IOPS | :heavy_check_mark: Yes | |
| Policy | :x: No | |
| | | |
| **Virtual Hard Disk** | | |
| Format | :heavy_check_mark: Yes | |
| Type | :warning: Partial | :information_source: Differencing Disks are not supported. |
| Size | :heavy_check_mark: Yes | |
| Path | :heavy_check_mark: Yes | |
| | | |
| **DVD Drive** | | |
| Controller | :heavy_check_mark: Yes | |
| Location | :heavy_check_mark: Yes | |
| Media: Path | :heavy_check_mark: Yes | |
| | | |
| **Network Adapter** | | |
| Virtual Switch | :heavy_check_mark: Yes | |
| VLAN | :heavy_check_mark: Yes | |
| VLAN ID | :heavy_check_mark: Yes | |
| Bandwidth Management | :heavy_check_mark: Yes | |
| Minimum Bandwidth | :heavy_check_mark: Yes | |
| Maximum Bandwidth | :heavy_check_mark: Yes | |
| Virtual Machine Queue | :heavy_check_mark: Yes | |
| IPsec Task Offloading | :heavy_check_mark: Yes | |
| Maximum Offloaded SA | :heavy_check_mark: Yes | |
| SR-IOV | :heavy_check_mark: Yes | |
| Dynamic MAC Address | :heavy_check_mark: Yes | |
| Static MAC Address | :heavy_check_mark: Yes | |
| MAC Address Spoofing | :heavy_check_mark: Yes | |
| DHCP Guard | :heavy_check_mark: Yes | |
| Router Guard | :heavy_check_mark: Yes | |
| Protected Network | :heavy_check_mark: Yes | |
| Port Mirroring | :heavy_check_mark: Yes | |
| NIC Teaming | :heavy_check_mark: Yes | |
| Device Naming | :heavy_check_mark: Yes | |
| | | |
| **Integration Services** | | |
| Operating System Shutdown | :heavy_check_mark: Yes | |
| Time Synchronization | :heavy_check_mark: Yes | |
| Data Exchange | :heavy_check_mark: Yes | |
| Heartbeat | :heavy_check_mark: Yes | |
| Backup (Volume Shadow Copy) | :heavy_check_mark: Yes | |
| Guest Services | :heavy_check_mark: Yes | |
| | | |
| **Checkpoints** | | |
| Enable | :heavy_check_mark: Yes | |
| Production | :heavy_check_mark: Yes | |
| Fallback | :heavy_check_mark: Yes | |
| Standard | :heavy_check_mark: Yes | |
| Path | :heavy_check_mark: Yes | |
| | | |
| **Smart Paging** | | |
| Path | :heavy_check_mark: Yes | |
| | | |
| **Automatic Start** | | |
| Action | :heavy_check_mark: Yes | |
| Delay | :heavy_check_mark: Yes | |
| | | |
| **Automatic Stop** | | |
| Action | :heavy_check_mark: Yes | |























