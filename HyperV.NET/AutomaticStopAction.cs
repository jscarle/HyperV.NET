using System;

namespace HyperV
{
    ///<summary>Defines the automatic stop action.</summary>
    public enum AutomaticStopAction : UInt16
    {
        ///<summary>Save the virtual machine state.</summary>
        Save = 3,

        ///<summary>Turn off the virtual machine.</summary>
        TurnOff = 2,

        ///<summary>Shutdown the guest operating system.</summary>
        Shutdown = 4
    }
}