using System;

namespace Utilities.Enums
{
    /// <summary>
    /// Enumetor who define the possible alarm states
    /// </summary>
    [Flags]
    public enum AlarmMask : uint
    {
        None = 0,
        Default = 0x000F,
        On = 0x1,
        OnAck = 0x2,
        Off = 0x4,
        OffAck = 0x8
    }
}
