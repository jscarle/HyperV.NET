using System.Diagnostics.CodeAnalysis;

namespace HyperV
{
    public class ScsiController
    {
        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public IScsiDrive[] Drives { get; set; }

        public ScsiController()
        {
            Drives = new IScsiDrive[64];
        }
    }
}