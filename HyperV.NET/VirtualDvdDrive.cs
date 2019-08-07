namespace HyperV
{
    ///<summary>Defines a virtual DVD drive.</summary>
    public class VirtualDvdDrive : IScsiDrive
    {
        ///<summary>The attached virtual DVD media.</summary>
        public VirtualDvdDisk VirtualDvdDisk { get; set; }

        ///<summary>Initializes a new instance of the <see cref="VirtualDvdDrive"/> class.</summary>
        public VirtualDvdDrive()
        {
        }

        ///<summary>Initializes a new instance of the <see cref="VirtualDvdDrive"/> class with the specified disk already attached.</summary>
        ///<param name="virtualDvdDisk">The virtual DVD media to attach.</param>
        public VirtualDvdDrive(VirtualDvdDisk virtualDvdDisk)
        {
            VirtualDvdDisk = virtualDvdDisk;
        }
    }
}