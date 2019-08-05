using System;
using System.Collections.Generic;

namespace HyperV.Definitions
{
    public class ScsiControllersDefinition : List<ScsiController>
    {
        public new void Add(ScsiController scsiControllerSettings)
        {
            if (this.Count > 4)
                throw new ArgumentOutOfRangeException($"Cannot add more than 4 {nameof(ScsiController)}.");
            base.Add(scsiControllerSettings);
        }
    }
}