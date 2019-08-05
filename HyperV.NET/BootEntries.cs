using System.Collections.Generic;

namespace HyperV
{
    public class BootEntries : List<BootEntry>
    {
        public void Move(int oldIndex, int newIndex)
        {
            BootEntry item = base[oldIndex];
            base.RemoveAt(oldIndex);
            base.Insert(newIndex, item);
        }
    }
}