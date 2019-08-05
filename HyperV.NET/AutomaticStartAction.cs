using System;

namespace HyperV
{
    public enum AutomaticStartAction : UInt16
    {
        Nothing = 2,
        StartIfRunning = 3,
        AlwaysStart = 4
    }
}