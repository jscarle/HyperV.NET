using System;
using System.Management;
using System.Threading;
using HyperV.Extensions;

namespace HyperV
{
    public partial class VMMS : IDisposable
    {
        private readonly string host;
        private readonly ManagementScope virtualizationScope;
        private readonly ManagementScope hgsScope;
        private readonly ManagementObject ss;       // Security Service
        private readonly ManagementObject ims;      // Image Management Service
        private readonly ManagementObject vmms;     // Virtual Machine Management Service
        private bool disposed = false;

        internal void AddFeatureSettings(ManagementObject ethernetPortAllocationSettings, ManagementObject[] featureSettings, out ManagementObject[] resultingFeatureSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("AddFeatureSettings"))
            {
                inputParameters["AffectedConfiguration"] = ethernetPortAllocationSettings.Path.Path;
                inputParameters["FeatureSettings"] = featureSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("AddFeatureSettings", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingFeatureSettings = ((string[])outputParameters["ResultingFeatureSettings"]).ToObjectArray();
                }
            }
        }

        internal void AddResourceSettings(ManagementObject systemSettings, ManagementObject[] resourceSettings, out ManagementObject[] resultingResourceSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("AddResourceSettings"))
            {
                inputParameters["AffectedConfiguration"] = systemSettings.Path.Path;
                inputParameters["ResourceSettings"] = resourceSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("AddResourceSettings", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingResourceSettings = ((string[])outputParameters["ResultingResourceSettings"]).ToObjectArray();
                }
            }
        }

        internal ManagementObject CreateFeatureSettings(Features feature)
        {
            string featureGuid = "";
            switch (feature)
            {
                case Features.Bandwidth: featureGuid = "24AD3CE1-69BD-4978-B2AC-DAAD389D699C"; break;
                case Features.Offload: featureGuid = "C885BFD1-ABB7-418F-8163-9F379C9F7166"; break;
                case Features.Security: featureGuid = "776E0BA7-94A1-41C8-8F28-951F524251B5"; break;
                case Features.Vlan: featureGuid = "952C5004-4465-451C-8CB8-FA9AB382B773"; break;
            }

            string defaultFeatureSettingPath = null;
            using (ManagementClass featureCapabilitiesClass = new ManagementClass("Msvm_EthernetSwitchFeatureCapabilities"))
            {
                featureCapabilitiesClass.Scope = virtualizationScope;
                using (ManagementObjectCollection featureCapabilitiesCollection = featureCapabilitiesClass.GetInstances())
                    foreach (ManagementObject featureCapabilities in featureCapabilitiesCollection)
                        using (featureCapabilities)
                            if (String.Equals((string)featureCapabilities["FeatureId"], featureGuid, StringComparison.OrdinalIgnoreCase))
                            {
                                using (ManagementObjectCollection featureSettingAssociationCollection = featureCapabilities.GetRelationships("Msvm_FeatureSettingsDefineCapabilities"))
                                    foreach (ManagementObject featureSettingAssociation in featureSettingAssociationCollection)
                                        using (featureSettingAssociation)
                                        {
                                            if ((ushort)featureSettingAssociation["ValueRole"] == 0)
                                            {
                                                defaultFeatureSettingPath = (string)featureSettingAssociation["PartComponent"];
                                                break;
                                            }
                                        }
                                break;
                            }
            }

            if (defaultFeatureSettingPath == null)
                throw new ManagementException("Unable to find the Default Feature Settings.");

            using (ManagementObject defaultFeatureSetting = new ManagementObject(defaultFeatureSettingPath))
            {
                defaultFeatureSetting.Scope = virtualizationScope;
                defaultFeatureSetting.Get();
                return defaultFeatureSetting;
            }
        }

        internal ManagementObject CreateKeyProtector()
        {
            using (ManagementClass managementClass = new ManagementClass("MSFT_HgsKeyProtector"))
            {
                managementClass.Scope = hgsScope;
                return managementClass;
            }
        }

        internal ManagementObject CreateResource(Resources resource)
        {
            string resourcePoolClass = "Msvm_ResourcePool";
            if (resource == Resources.Processor)
                resourcePoolClass = "Msvm_ProcessorPool";

            string resourceSubType = "";
            switch (resource)
            {
                case Resources.Processor: resourceSubType = "Microsoft:Hyper-V:Processor"; break;
                case Resources.Memory: resourceSubType = "Microsoft:Hyper-V:Memory"; break;
                case Resources.SCSIController: resourceSubType = "Microsoft:Hyper-V:Synthetic SCSI Controller"; break;
                case Resources.VirtualHardDrive: resourceSubType = "Microsoft:Hyper-V:Synthetic Disk Drive"; break;
                case Resources.VirtualHardDisk: resourceSubType = "Microsoft:Hyper-V:Virtual Hard Disk"; break;
                case Resources.VirtualDvdDrive: resourceSubType = "Microsoft:Hyper-V:Synthetic DVD Drive"; break;
                case Resources.VirtualDvdDisk: resourceSubType = "Microsoft:Hyper-V:Virtual CD/DVD Disk"; break;
                case Resources.NetworkAdapter: resourceSubType = "Microsoft:Hyper-V:Synthetic Ethernet Port"; break;
                case Resources.SwitchPort: resourceSubType = "Microsoft:Hyper-V:Ethernet Connection"; break;
            }

            string defaultSettingPath = null;
            ObjectQuery query = new ObjectQuery($"SELECT * FROM {resourcePoolClass} WHERE ResourceSubType = \"{resourceSubType}\" AND Primordial = True");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(virtualizationScope, query))
            using (ManagementObject resourcePool = searcher.Get().First())
            using (ManagementObjectCollection capabilitiesCollection = resourcePool.GetRelated("Msvm_AllocationCapabilities", "Msvm_ElementCapabilities", null, null, null, null, false, null))
            using (ManagementObject capabilities = capabilitiesCollection.First())
                foreach (ManagementObject settingAssociation in capabilities.GetRelationships("Msvm_SettingsDefineCapabilities"))
                    if ((ushort)settingAssociation["ValueRole"] == 0)
                    {
                        defaultSettingPath = (string)settingAssociation["PartComponent"];
                        break;
                    }

            if (defaultSettingPath == null)
                throw new ManagementException("Unable to find the Default Resource Settings.");

            using (ManagementObject defaultSetting = new ManagementObject(defaultSettingPath))
            {
                defaultSetting.Scope = virtualizationScope;
                defaultSetting.Get();
                return defaultSetting;
            }
        }

        internal ManagementObject CreateSettings(Settings settings)
        {
            using (ManagementClass managementClass = new ManagementClass(SettingsClass(settings)))
            {
                managementClass.Scope = virtualizationScope;
                return managementClass.CreateInstance();
            }
        }

        internal void CreateVirtualHardDisk(ManagementObject virtualHardDiskSettings)
        {
            using (ManagementBaseObject inputParameters = ims.GetMethodParameters("CreateVirtualHardDisk"))
            {
                inputParameters["VirtualDiskSettingData"] = virtualHardDiskSettings.GetText(TextFormat.WmiDtd20);
                using (ManagementBaseObject outputParameters = ims.InvokeMethod("CreateVirtualHardDisk", inputParameters, null))
                    ValidateOutput(outputParameters);
            }
        }

        internal void DefineSystem(ManagementObject systemSettings, ManagementObject[] resourceSettings, out ManagementObject resultingSystem)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("DefineSystem"))
            {
                inputParameters["SystemSettings"] = systemSettings.GetText(TextFormat.WmiDtd20);
                inputParameters["ResourceSettings"] = resourceSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("DefineSystem", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingSystem = new ManagementObject((string)outputParameters["ResultingSystem"]);
                }
            }
        }

        internal static string ErrorCodeMeaning(uint returnValue)
        {
            switch (returnValue)
            {
                case 0: return "Completed with No Error.";
                case 1: return "Not Supported.";
                case 2: return "Failed.";
                case 3: return "Timeout.";
                case 4: return "Invalid Parameter.";
                case 5: return "Invalid State.";
                case 6: return "Invalid Type.";
                case 4096: return "Method Parameters Checked - Job Started.";
                case 32768: return "Failed.";
                case 32769: return "Access Denied.";
                case 32770: return "Not Supported.";
                case 32771: return "Status is Unknown.";
                case 32772: return "Timeout.";
                case 32773: return "Invalid Parameter.";
                case 32774: return "System is In Use.";
                case 32775: return "Invalid State for this Operation.";
                case 32776: return "Incorrect Data Type.";
                case 32777: return "System is Not Available.";
                case 32778: return "Out of Memory.";
                default: return "The Method Failed. The Reason is Unknown.";
            }
        }

        internal bool ExistsVirtualMachine(string name)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Msvm_ComputerSystem WHERE Caption = \"Virtual Machine\" AND ElementName = \"{name}\"");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(virtualizationScope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                if (collection.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        internal void DestroyVirtualMachine(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (ManagementObject virtualMachine = GetVirtualMachine(name))
            {
                using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("DestroySystem"))
                {
                    inputParameters["AffectedSystem"] = virtualMachine.Path.Path;
                    using (ManagementBaseObject outputParameters = vmms.InvokeMethod("DestroySystem", inputParameters, null))
                    {
                        ValidateOutput(outputParameters);
                    }
                }
            }
        }

        internal bool ExistsVirtualSwitch(string name)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Msvm_VirtualEthernetSwitch WHERE Caption = \"Virtual Switch\" AND ElementName = \"{name}\"");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(virtualizationScope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                if (collection.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        internal ManagementObject GetImageManagementService()
        {
            using (ManagementClass managementClass = new ManagementClass("Msvm_ImageManagementService"))
            {
                managementClass.Scope = virtualizationScope;
                return managementClass.GetInstances().First();
            }
        }

        internal static ManagementObject GetRelatedSettings(ManagementObject instance, Settings settings)
        {
            return instance.GetRelated(SettingsClass(settings)).First();
        }

        internal ManagementObject GetSecurityService()
        {
            using (ManagementClass managementClass = new ManagementClass("Msvm_SecurityService"))
            {
                managementClass.Scope = virtualizationScope;
                return managementClass.GetInstances().First();
            }
        }

        internal ManagementObject GetUntrustedGuardian()
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM MSFT_HgsGuardian WHERE Name = \"UntrustedGuardian\"");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(hgsScope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                if (collection.Count == 0)
                    return null;
                else
                    return collection.First();
            }
        }

        internal ManagementObject GetVirtualMachine(string name)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Msvm_ComputerSystem WHERE Caption = \"Virtual Machine\" AND ElementName = \"{name}\"");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(virtualizationScope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                if (collection.Count == 0)
                    throw new ManagementException("Unable to find the Virtual Machine.");

                return collection.First();
            }
        }

        internal ManagementObject GetVirtualMachineManagementService()
        {
            using (ManagementClass managementClass = new ManagementClass("Msvm_VirtualSystemManagementService"))
            {
                managementClass.Scope = virtualizationScope;
                return managementClass.GetInstances().First();
            }
        }

        internal ManagementObject GetVirtualSwitch(string name)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Msvm_VirtualEthernetSwitch WHERE Caption = \"Virtual Switch\" AND ElementName = \"{name}\"");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(virtualizationScope, query))
            using (ManagementObjectCollection collection = searcher.Get())
            {
                if (collection.Count == 0)
                    throw new ManagementException("Unable to find the Virtual Switch.");

                return collection.First();
            }
        }

        internal static bool IsJobComplete(object jobStateObj)
        {
            JobState jobState = (JobState)(ushort)jobStateObj;

            return
                (jobState == JobState.Completed) ||
                (jobState == JobState.CompletedWithWarnings) ||
                (jobState == JobState.Terminated) ||
                (jobState == JobState.Exception) ||
                (jobState == JobState.Killed);
        }

        internal static bool IsJobSuccessful(object jobStateObj)
        {
            JobState jobState = (JobState)(ushort)jobStateObj;

            return
                (jobState == JobState.Completed) ||
                (jobState == JobState.CompletedWithWarnings);
        }

        internal void ModifyFeatureSettings(ManagementObject[] featureSettings, out ManagementObject[] resultingFeatureSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("ModifyFeatureSettings"))
            {
                inputParameters["FeatureSettings"] = featureSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("ModifyFeatureSettings", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingFeatureSettings = ((string[])outputParameters["ResultingFeatureSettings"]).ToObjectArray();
                }
            }
        }

        internal void ModifyGuestServiceSettings(ManagementObject[] guestServiceSettings, out ManagementObject[] resultingGuestServiceSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("ModifyGuestServiceSettings"))
            {
                inputParameters["GuestServiceSettings"] = guestServiceSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("ModifyGuestServiceSettings", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingGuestServiceSettings = ((string[])outputParameters["ResultingGuestServiceSettings"]).ToObjectArray();
                }
            }
        }

        internal void ModifyResourceSettings(ManagementObject[] resourceSettings, out ManagementObject[] resultingResourceSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("ModifyResourceSettings"))
            {
                inputParameters["ResourceSettings"] = resourceSettings.ToStringArray();
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("ModifyResourceSettings", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    resultingResourceSettings = ((string[])outputParameters["ResultingResourceSettings"]).ToObjectArray();
                }
            }
        }

        internal void ModifySecuritySettings(ManagementObject securitySettings)
        {
            using (ManagementBaseObject inputParameters = ss.GetMethodParameters("ModifySecuritySettings"))
            {
                inputParameters["SecuritySettingData"] = securitySettings.GetText(TextFormat.WmiDtd20);
                using (ManagementBaseObject outputParameters = ss.InvokeMethod("ModifySecuritySettings", inputParameters, null))
                    ValidateOutput(outputParameters);
            }
        }

        internal void ModifySystemSettings(ManagementObject systemSettings)
        {
            using (ManagementBaseObject inputParameters = vmms.GetMethodParameters("ModifySystemSettings"))
            {
                inputParameters["SystemSettings"] = systemSettings.GetText(TextFormat.WmiDtd20);
                using (ManagementBaseObject outputParameters = vmms.InvokeMethod("ModifySystemSettings", inputParameters, null))
                    ValidateOutput(outputParameters);
            }
        }

        internal void NewByGenerateCertificates(out ManagementObject hgsGuardian)
        {
            using (ManagementClass hgsGuardianClass = new ManagementClass("MSFT_HgsGuardian"))
            {
                hgsGuardianClass.Scope = hgsScope;
                using (ManagementBaseObject inputParameters = hgsGuardianClass.GetMethodParameters("NewByGenerateCertificates"))
                {
                    inputParameters["Name"] = "UntrustedGuardian";
                    inputParameters["GenerateCertificates"] = true;
                    using (ManagementBaseObject outputParameters = hgsGuardianClass.InvokeMethod("NewByGenerateCertificates", inputParameters, null))
                    {
                        ValidateOutput(outputParameters);
                        hgsGuardian = (ManagementObject)outputParameters["cmdletOutput"];
                    }
                }
            }
        }

        internal byte[] NewByGuardians()
        {
            ManagementObject defaultGuardian = GetUntrustedGuardian();
            if (defaultGuardian == null)
                NewByGenerateCertificates(out defaultGuardian);
            using (defaultGuardian)
            using (ManagementObject keyProtector = CreateKeyProtector())
            using (ManagementBaseObject inputParameters = keyProtector.GetMethodParameters("NewByGuardians"))
            {
                inputParameters["Owner"] = defaultGuardian;
                inputParameters["AllowUntrustedRoot"] = true;
                using (ManagementBaseObject outputParameters = keyProtector.InvokeMethod("NewByGuardians", inputParameters, null))
                {
                    ValidateOutput(outputParameters);
                    using (ManagementBaseObject hgsKeyProtector = (ManagementBaseObject)outputParameters["cmdletOutput"])
                        return (byte[])hgsKeyProtector["RawData"];
                }
            }
        }

        internal static void RequestStateChange(ManagementObject virtualMachine, RequestedState requestedState)
        {
            using (ManagementBaseObject inputParameters = virtualMachine.GetMethodParameters("RequestStateChange"))
            {
                inputParameters["RequestedState"] = requestedState;
                using (ManagementBaseObject outputParameters = virtualMachine.InvokeMethod("RequestStateChange", inputParameters, null))
                    ValidateOutput(outputParameters);
            }
        }

        internal void SetKeyProtector(ManagementObject securitySettings, byte[] keyProtector)
        {
            using (ManagementBaseObject inputParameters = ss.GetMethodParameters("SetKeyProtector"))
            {
                inputParameters["SecuritySettingData"] = securitySettings.GetText(TextFormat.WmiDtd20);
                inputParameters["KeyProtector"] = keyProtector;
                using (ManagementBaseObject outputParameters = ss.InvokeMethod("SetKeyProtector", inputParameters, null))
                    ValidateOutput(outputParameters);
            }
        }

        internal static string SettingsClass(Settings settings)
        {
            switch (settings)
            {
                case Settings.System: return "Msvm_VirtualSystemSettingData";
                case Settings.Security: return "Msvm_SecuritySettingData";
                case Settings.Resource: return "Msvm_ResourceAllocationSettingData";
                case Settings.Memory: return "Msvm_MemorySettingData";
                case Settings.Processor: return "Msvm_ProcessorSettingData";
                case Settings.Storage: return "Msvm_StorageAllocationSettingData";
                case Settings.VirtualHardDisk: return "Msvm_VirtualHardDiskSettingData";
                case Settings.NetworkAdapter: return "Msvm_SyntheticEthernetPortSettingData";
                case Settings.SwitchPort: return "Msvm_EthernetPortAllocationSettingData";
                case Settings.SwitchPortOffload: return "Msvm_EthernetSwitchPortOffloadSettingData";
                case Settings.Shutdown: return "Msvm_ShutdownComponentSettingData";
                case Settings.TimeSynchronization: return "Msvm_TimeSyncComponentSettingData";
                case Settings.DataExchange: return "Msvm_KvpExchangeComponentSettingData";
                case Settings.Heartbeat: return "Msvm_HeartbeatComponentSettingData";
                case Settings.VolumeShadowCopy: return "Msvm_VssComponentSettingData";
                case Settings.GuestServices: return "Msvm_GuestServiceInterfaceComponentSettingData";
            }

            return null;
        }

        internal static void ValidateOutput(ManagementBaseObject outputParameters)
        {
            uint errorCode = (uint)outputParameters["ReturnValue"];
            if (errorCode == 4096)
            {
                using (ManagementObject job = new ManagementObject((string)outputParameters["Job"]))
                {
                    while (!IsJobComplete(job["JobState"]))
                    {
                        Thread.Sleep(500);
                        job.Get();
                    }

                    if (!IsJobSuccessful(job["JobState"]))
                    {
                        string errorMessage = "The method failed.";
                        if (!String.IsNullOrWhiteSpace((string)job["ErrorDescription"]))
                            errorMessage = (string)job["ErrorDescription"];

                        throw new ManagementException(errorMessage);
                    }
                }
            }
            else if (errorCode != 0)
            {
                throw new ManagementException(ErrorCodeMeaning(errorCode));
            }
        }

        internal enum JobState
        {
            New = 2,
            Starting = 3,
            Running = 4,
            Suspended = 5,
            ShuttingDown = 6,
            Completed = 7,
            Terminated = 8,
            Killed = 9,
            Exception = 10,
            CompletedWithWarnings = 32768
        }

        internal enum RequestedState : ushort
        {
            Other = 1,
            Enabled = 2,
            Disabled = 3,
            ShutDown = 4,
            Offline = 6,
            Test = 7,
            Defer = 8,
            Quiesce = 9,
            Reboot = 10,
            Reset = 11,
            Saving = 32773,
            Pausing = 32776,
            Resuming = 32777,
            FastSaved = 32779,
            FastSaving = 32780,
            RunningCritical = 32781,
            OffCritical = 32782,
            StoppingCritical = 32783,
            SavedCritical = 32784,
            PausedCritical = 32785,
            StartingCritical = 32786,
            ResetCritical = 32787,
            SavingCritical = 32788,
            PausingCritical = 32789,
            ResumingCritical = 32790,
            FastSavedCritical = 32791,
            FastSavingCritical = 32792
        }

        internal enum Resources
        {
            Processor,
            Memory,
            SCSIController,
            VirtualHardDrive,
            VirtualHardDisk,
            VirtualDvdDrive,
            VirtualDvdDisk,
            NetworkAdapter,
            SwitchPort
        }

        internal enum Settings
        {
            System,
            Security,
            Resource,
            Memory,
            Processor,
            Storage,
            VirtualHardDisk,
            NetworkAdapter,
            SwitchPort,
            SwitchPortOffload,
            Shutdown,
            TimeSynchronization,
            DataExchange,
            Heartbeat,
            VolumeShadowCopy,
            GuestServices
        }

        internal enum Features
        {
            Bandwidth,
            Offload,
            Security,
            Vlan
        }
    }
}