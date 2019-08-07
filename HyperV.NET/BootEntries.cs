using System.Collections.Generic;

namespace HyperV
{
    ///<summary>Defines a list of boot devices.</summary>
    public class BootEntries : List<BootDevice>
    {
        ///<summary>Move a boot device from one index to another.</summary>
        ///<param name="index">The current index of the boot device.</param>
        ///<param name="newIndex">The new index for the boot device.</param>
        public void Move(int index, int newIndex)
        {
            BootDevice item = base[index];
            base.RemoveAt(index);
            base.Insert(newIndex, item);
        }
    }
}