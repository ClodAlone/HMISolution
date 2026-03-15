using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class BitOperations
    {
        #region Methods
        public static byte SetBitValue(byte mask, byte value, bool set)
        {
            if (set)
                value |= mask;
            else
                value &= (byte)(~mask);
            return value;
        }
        public static bool CheckBitValue(byte mask, byte value)
        {
            return (value & mask) > 0;
        }
        #endregion
    }
}
