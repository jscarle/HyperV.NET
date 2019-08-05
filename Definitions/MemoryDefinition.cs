using System;

namespace HyperV.Definitions
{
    public class MemoryDefinition
    {
        private UInt32 buffer;

        public UInt32 Buffer
        {
            get { return buffer; }
            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException($"{nameof(Buffer)} must be between 0 and 100.");
                buffer = value;
            }
        }

        private UInt64 maximum;

        public UInt64 Maximum
        {
            get { return maximum; }
            set
            {
                if (value < 32 || value > 12582912)
                    throw new ArgumentOutOfRangeException($"{nameof(Maximum)} must be between 32 and 12582912.");
                maximum = value;
            }
        }

        private UInt64 minimum;

        public UInt64 Minimum
        {
            get { return minimum; }
            set
            {
                if (value < 32 || value > 12582912)
                    throw new ArgumentOutOfRangeException($"{nameof(Minimum)} must be between 32 and 12582912.");
                minimum = value;
            }
        }

        private UInt64 startup;

        public UInt64 Startup
        {
            get { return startup; }
            set
            {
                if (value < 32 || value > 12582912)
                    throw new ArgumentOutOfRangeException($"{nameof(Startup)} must be between 32 and 12582912.");
                startup = value;
            }
        }

        public MemoryWeight Weight { get; set; }

        public MemoryDefinition()
        {
            Buffer = 20;
            Startup = 1024;
            Weight = MemoryWeight.Balanced;
        }
    }
}