namespace HyperV
{
    public class VirtualDvdDrive : IScsiDrive
    {
        public VirtualDvdDisk VirtualDvdDisk { get; set; }

        public VirtualDvdDrive()
        {
        }

        public VirtualDvdDrive(VirtualDvdDisk virtualDvdDisk)
        {
            VirtualDvdDisk = virtualDvdDisk;
        }
    }
}