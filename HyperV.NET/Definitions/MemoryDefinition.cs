using System;

namespace HyperV.Definitions
{
    ///<summary>Defines the Memory settings.</summary>
    public class MemoryDefinition
    {
        private UInt32 buffer;

        ///<summary>The percentage of memory to try to reserve as a buffer.</summary>
        ///<value>The buffer must be between 0 and 100.</value>
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

        ///<summary>The maximum RAM (MB) allowed when Dynamic Memory is enabled.</summary>
        ///<value>The maximum must be between 32 and 12582912.</value>
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

        ///<summary>The minimum RAM (MB) allowed when Dynamic Memory is enabled.</summary>
        ///<value>The minimum must be between 32 and 12582912.</value>
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

        ///<summary>The amount of RAM (MB) to use at startup.</summary>
        ///<value>The startup must be between 32 and 12582912.</value>
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

        ///<summary>The priority when balancing memory availability compared to other virtual machines.</summary>
        public MemoryWeight Weight { get; set; }

        ///<summary>Initializes a new instance of the <see cref="MemoryDefinition"/> class.</summary>
        public MemoryDefinition()
        {
            Buffer = 20;
            Minimum = 512;
            Maximum = 1048576;
            Startup = 1024;
            Weight = MemoryWeight.Balanced;
        }
    }
}
