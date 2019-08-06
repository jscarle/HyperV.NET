using System;
using System.Collections.Generic;

namespace HyperV.Definitions
{
    ///<summary>Defines the settings for up to 4 SCSI Controllers.</summary>
    public class ScsiControllersDefinition : List<ScsiController>
    {
        ///<summary>Add settings for a single SCSI Controller.</summary>
        ///<remarks>A maximum of 4 SCSI Controllers can be added.</remarks>
        public new void Add(ScsiController scsiControllerSettings)
        {
            if (this.Count > 4)
                throw new ArgumentOutOfRangeException($"Cannot add more than 4 {nameof(ScsiController)}.");
            base.Add(scsiControllerSettings);
        }
    }
}