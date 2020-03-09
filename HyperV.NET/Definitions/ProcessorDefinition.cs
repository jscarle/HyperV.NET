using System;

namespace HyperV.Definitions
{
    ///<summary>Defines the Processor settings.</summary>
    public class ProcessorDefinition
    {
        private UInt64 hardwareThreadsPerCore;

        ///<summary>The number of hardware threads per core.</summary>
        ///<value>The limit must be between 0 and 2.</value>
        public UInt64 HardwareThreadsPerCore
        {
            get { return hardwareThreadsPerCore; }
            set
            {
                if (value > 2)
                    throw new ArgumentOutOfRangeException($"{nameof(HardwareThreadsPerCore)} must be between 0 and 2.");
                hardwareThreadsPerCore = value;
            }
        }

        private UInt64 limit;

        ///<summary>The virtual machine limit (percentage).</summary>
        ///<value>The limit must be between 0 and 100.</value>
        public UInt64 Limit
        {
            get { return limit; }
            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException($"{nameof(Limit)} must be between 0 and 100.");
                limit = value;
            }
        }

        ///<summary>Migrate to a physical computer with a different processor version.</summary>
        public bool LimitFeatures;

        private UInt64 numaMemoryPerNode;

        ///<summary>The maximum amount of memory (MB) per NUMA node.</summary>
        ///<value>The amount must be between 32 and 12582912.</value>
        public UInt64 NumaMemoryPerNode
        {
            get { return numaMemoryPerNode; }
            set
            {
                if (value < 32 || value > 12582912)
                    throw new ArgumentOutOfRangeException($"{nameof(NumaMemoryPerNode)} must be between 32 and 12582912.");
                numaMemoryPerNode = value;
            }
        }

        private UInt64 numaNodesPerSocket;

        ///<summary>The maximum number of NUMA nodes allowed on a single socket.</summary>
        ///<value>The number must be between 1 and 64.</value>
        public UInt64 NumaNodesPerSocket
        {
            get { return numaNodesPerSocket; }
            set
            {
                if (value < 1 || value > 64)
                    throw new ArgumentOutOfRangeException($"{nameof(NumaNodesPerSocket)} must be between 1 and 64.");
                numaNodesPerSocket = value;
            }
        }

        private UInt64 numaProcessorsPerNode;

        ///<summary>The maximum number of processors per NUMA node.</summary>
        ///<value>The number must be between 1 and 64.</value>
        public UInt64 NumaProcessorsPerNode
        {
            get { return numaProcessorsPerNode; }
            set
            {
                if (value < 1 || value > 64)
                    throw new ArgumentOutOfRangeException($"{nameof(NumaProcessorsPerNode)} must be between 1 and 64.");
                numaProcessorsPerNode = value;
            }
        }

        private UInt64 quantity;

        ///<summary>The number of virtual processors.</summary>
        ///<value>The number must be between 1 and 240.</value>
        public UInt64 Quantity
        {
            get { return quantity; }
            set
            {
                if (value < 1 || value > 240)
                    throw new ArgumentOutOfRangeException($"{nameof(Quantity)} must be between 1 and 240.");
                quantity = value;
            }
        }

        private UInt64 reservation;

        ///<summary>The virtual machine reserve (percentage).</summary>
        ///<value>The reserve must be between 0 and 100.</value>
        public UInt64 Reservation
        {
            get { return reservation; }
            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException($"{nameof(Reservation)} must be between 0 and 100.");
                reservation = value;
            }
        }

        private UInt32 weight;

        ///<summary>The priority to use when balancing processor resources compared to other virtual machines.</summary>
        ///<value>The weight must be between 0 and 10000.</value>
        public UInt32 Weight
        {
            get { return weight; }
            set
            {
                if (value > 10000)
                    throw new ArgumentOutOfRangeException($"{nameof(Weight)} must be between 0 and 10000.");
                weight = value;
            }
        }

        ///<summary>Initializes a new instance of the <see cref="ProcessorDefinition"/> class.</summary>
        public ProcessorDefinition()
        {
            Quantity = 1;
            Limit = 100;
            Weight = 100;
            NumaMemoryPerNode = 131072;
            NumaNodesPerSocket = 1;
            NumaProcessorsPerNode = 8;
            HardwareThreadsPerCore = 0;
        }
    }
}