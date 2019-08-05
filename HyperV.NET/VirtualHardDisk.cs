using System;

namespace HyperV
{
    public class VirtualHardDisk
    {
        public VirtualHardDiskFormat Format { get; set; }
        public string Path { get; set; }
        private UInt64 size;

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

        public VirtualHardDiskType Type { get; set; }

        public VirtualHardDisk(VirtualHardDiskFormat format, VirtualHardDiskType type, UInt64 size, string path)
        {
            Format = format;
            Path = path;
            Size = size;
            Type = type;
        }
    }
}