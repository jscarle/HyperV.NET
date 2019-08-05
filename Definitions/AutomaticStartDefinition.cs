using System;

namespace HyperV.Definitions
{
    public class AutomaticStartDefinition
    {
        public AutomaticStartAction Action { get; set; }
        private UInt64 delay;

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

        public AutomaticStartDefinition()
        {
            Action = AutomaticStartAction.Nothing;
        }
    }
}