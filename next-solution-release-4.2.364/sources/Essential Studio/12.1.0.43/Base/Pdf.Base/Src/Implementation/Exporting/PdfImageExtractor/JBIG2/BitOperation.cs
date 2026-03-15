#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Pdf
{
    class BitOperation
    {
        public const int LEFT_SHIFT = 0;
        public const int RIGHT_SHIFT = 1;

        public int GetInt32(short[] number)
        {
            return (number[0] << 24) | (number[1] << 16) | (number[2] << 8) | number[3];
        }

        public int GetInt16(short[] number)
        {
            return (number[0] << 8) | number[1];
        }

        public long Bit32Shift(long number, int shift, int direction)
        {
            if (direction == LEFT_SHIFT)
            {
                number <<= shift;
            }
            else
            {
                number >>= shift;
            }

            long mask = 0xffffffffL; // 1111 1111 1111 1111 1111 1111 1111 1111
            return (number & mask);
        }

        public int Bit8Shift(int number, int shift, int direction)
        {
            if (direction == LEFT_SHIFT)
            {
                number <<= shift;
            }
            else
            {
                number >>= shift;
            }

            int mask = 0xff; // 1111 1111
            return (number & mask);
        }
    }
}
