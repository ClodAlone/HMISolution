using System;
using Microsoft.SPOT;

namespace MFRuntime
{
    public static class Utils
    {
        #region diagnostics helpers
        public static string ByteToHex(byte b)
        {
            const string hex = "0123456789ABCDEF";
            int lowNibble = b & 0x0F;
            int highNibble = (b & 0xF0) >> 4;
            string s = new string(new char[] { hex[highNibble], hex[lowNibble] });
            return s;
        }

        public static string BufferToString(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            string s = string.Empty;
            for (int i = 0; i < buffer.Length; i++)
            {
                s += ByteToHex(buffer[i]) + " ";
                if (i > 0 && i % 16 == 0)
                    s += "\r\n";
            }
            return s;
        }
        #endregion
    }
}
