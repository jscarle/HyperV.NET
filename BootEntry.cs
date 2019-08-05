using System;
using System.IO;
using System.Management;

namespace HyperV
{
    public class BootEntry
    {
        public string Description { get; }
        public BootDeviceType DeviceType { get; }
        public string Value { get; }
        private readonly string bootEntry;

        public BootEntry(string bootEntry)
        {
            this.bootEntry = bootEntry;
            DeviceType = BootDeviceType.Unknown;
            Value = "";

            using (ManagementObject bootSource = new ManagementObject(bootEntry))
            {
                Description = (string)bootSource["BootSourceDescription"];
                switch ((UInt32)bootSource["BootSourceType"])
                {
                    case 1:
                        using (ManagementObject resource = VMMS.GetRelatedSettings(bootSource, VMMS.Settings.Resource))
                        {
                            switch ((UInt16)resource["ResourceType"])
                            {
                                case 16: DeviceType = BootDeviceType.DvdDrive; break;
                                case 17: DeviceType = BootDeviceType.HardDrive; break;
                            }
                            using (ManagementObject storage = VMMS.GetRelatedSettings(resource, VMMS.Settings.Storage))
                                if (storage != null)
                                    Value = Path.GetFileName(((string[])storage["HostResource"])[0]);
                        }
                        break;

                    case 2:
                        using (ManagementObject networkAdapter = VMMS.GetRelatedSettings(bootSource, VMMS.Settings.NetworkAdapter))
                        {
                            DeviceType = BootDeviceType.NetworkAdapter;
                            using (ManagementObject switchPort = VMMS.GetRelatedSettings(networkAdapter, VMMS.Settings.SwitchPort))
                                if (switchPort != null)
                                    if ((UInt16)switchPort["EnabledState"] == 2)
                                        Value = (string)switchPort["LastKnownSwitchName"];
                                    else
                                        Value = "None";
                        }
                        break;

                    case 3:
                        DeviceType = BootDeviceType.File;
                        Value = Path.GetFileName(((string)bootSource["FirmwareDevicePath"]).Split('/')[1]);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return bootEntry;
        }
    }
}