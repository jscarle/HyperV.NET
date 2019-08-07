using System;

namespace HyperV
{
    ///<summary>Defines a virtual hard disk.</summary>
    public class VirtualHardDisk
    {
        ///<summary>The file format of the virtual hard disk.</summary>
        public VirtualHardDiskFormat Format { get; set; }

        /// <summary>The path for the virtual hard disk file.</summary>
        public string Path { get; set; }

        private UInt64 size;

        /// <summary>The size in GB of the virtual hard disk.</summary>
        ///<value>The size must be between 1 and 2040 for the VHD format and between 1 and 65536 for the VHDX format.</value>
        public UInt64 Size
        {
            get { return size; }
            set
            {
                switch (Format)
                {
                    case VirtualHardDiskFormat.Vhd:
                        if (value < 1 || value > 2040)
                            throw new ArgumentOutOfRangeException($"{nameof(Size)} must be between 1 and 2040.");
                        break;

                    case VirtualHardDiskFormat.Vhdx:
                        if (value < 1 || value > 65536)
                            throw new ArgumentOutOfRangeException($"{nameof(Size)} must be between 1 and 65536.");
                        break;
                }
                size = value;
            }
        }

        /// <summary>The type of virtual hard disk.</summary>
        public VirtualHardDiskType Type { get; set; }

        ///<summary>Initializes a new instance of the <see cref="VirtualHardDisk"/> class of the specified format, type, size, and path.</summary>
        ///<param name="format">The file format of the virtual hard disk file.</param>
        ///<param name="type">The type of virtual hard disk.</param>
        ///<param name="size">The size in GB of the virtual hard disk.</param>
        ///<param name="path">The path for the virtual hard disk file.</param>
        public VirtualHardDisk(VirtualHardDiskFormat format, VirtualHardDiskType type, UInt64 size, string path)
        {
            Format = format;
            Path = path;
            Size = size;
            Type = type;
        }
    }
}