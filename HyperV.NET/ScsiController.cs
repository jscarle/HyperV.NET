using System.Diagnostics.CodeAnalysis;

namespace HyperV
{
    ///<summary>Defines a SCSI Controller.</summary>
    public class ScsiController
    {
        ///<summary>Represents the drives attached to this SCSI Controller.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public IScsiDrive[] Drives { get; set; }

        ///<summary>Initializes a new instance of the <see cref="ScsiController"/> class.</summary>
        public ScsiController()
        {
            Drives = new IScsiDrive[64];
        }
    }
}