using System;

namespace HyperV.Definitions
{
    public class ProcessorDefinition
    {
        private UInt64 limit;

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

        public bool LimitFeatures;
        private UInt64 numaMemoryPerNode;

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

        public ProcessorDefinition()
        {
            Quantity = 1;
            Limit = 100;
            Weight = 100;
            NumaMemoryPerNode = 131072;
            NumaNodesPerSocket = 1;
            NumaProcessorsPerNode = 8;
        }
    }
}