namespace HyperV
{
    ///<summary>Defines the type of boot device.</summary>
    public enum BootDeviceType
    {
        ///<summary>The type of boot device could not be determined.</summary>
        Unknown,

        ///<summary>Boot device is a DVD drive.</summary>
        DvdDrive,

        ///<summary>Boot device is a hard drive.</summary>
        HardDrive,

        ///<summary>Boot device is a network adapter.</summary>
        NetworkAdapter,

        ///<summary>Boot device is a file.</summary>
        File
    }
}