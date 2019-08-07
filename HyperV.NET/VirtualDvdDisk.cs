namespace HyperV
{
    ///<summary>Defines virtual DVD media.</summary>
    public class VirtualDvdDisk
    {
        ///<summary>The path for the virtual DVD media file.</summary>
        ///<remarks>The virtual DVD media file must be in the ISO image format.</remarks>
        public string Path { get; set; }

        ///<summary>Initializes a new instance of the <see cref="VirtualDvdDisk"/> class for the specified media.</summary>
        ///<param name="path">The path for the virtual DVD media file.</param>
        public VirtualDvdDisk(string path)
        {
            Path = path;
        }
    }
}