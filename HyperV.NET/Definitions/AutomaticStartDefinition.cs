using System;

namespace HyperV.Definitions
{
    ///<summary>Defines the Automatic Start Action settings.</summary>
    public class AutomaticStartDefinition
    {
        ///<summary>The action when the physical computer starts.</summary>
        public AutomaticStartAction Action { get; set; }

        private UInt64 delay;

        ///<summary>The startup delay when the the virtual machine is configured to start automatically.</summary>
        ///<value>The delay must be between 0 and 999999999.</value>
        public UInt64 Delay
        {
            get { return delay; }
            set
            {
                if (value > 999999999)
                    throw new ArgumentOutOfRangeException($"{nameof(Delay)} must be between 0 and 999999999.");
                Action = AutomaticStartAction.AlwaysStart;
                delay = value;
            }
        }

        ///<summary>Initializes a new instance of the <see cref="AutomaticStartDefinition"/> class.</summary>
        public AutomaticStartDefinition()
        {
            Action = AutomaticStartAction.Nothing;
        }
    }
}