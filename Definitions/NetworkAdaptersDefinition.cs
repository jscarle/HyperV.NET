using System;
using System.Collections.Generic;

namespace HyperV.Definitions
{
    public class NetworkAdaptersDefinition : List<NetworkAdapter>
    {
        public new void Add(NetworkAdapter networkAdapter)
        {
            if (this.Count > 8)
                throw new ArgumentOutOfRangeException($"Cannot add more than 8 {nameof(NetworkAdapter)}.");
            base.Add(networkAdapter);
        }
    }
}