#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Pdf.JPEG2000.entropy.decoder
{
    internal class ByteToBitInput
    {
        internal ByteInputBuffer in_Renamed;
        internal int bbuf;
        internal int bpos = -1;
        public ByteToBitInput(ByteInputBuffer in_Renamed)
        {
            this.in_Renamed = in_Renamed;
        }
        public int readBit()
        {
            if (bpos < 0)
            {
                if ((bbuf & 0xFF) != 0xFF)
                {
                    bbuf = in_Renamed.read();
                    bpos = 7;
                }
                else
                {
                    bbuf = in_Renamed.read();
                    bpos = 6;
                }
            }
            return (bbuf >> bpos--) & 0x01;
        }
        public virtual bool checkBytePadding()
        {
            int seq;
            if (bpos < 0 && (bbuf & 0xFF) == 0xFF)
            {
                bbuf = in_Renamed.read();
                bpos = 6;
            }
            if (bpos >= 0)
            {
                seq = bbuf & ((1 << (bpos + 1)) - 1);
                if (seq != (0x55 >> (7 - bpos)))
                    return true;
            }
            if (bbuf != -1)
            {
                if (bbuf == 0xFF && bpos == 0)
                {
                    if ((in_Renamed.read() & 0xFF) >= 0x80)
                        return true;
                }
                else
                {
                    if (in_Renamed.read() != -1)
                        return true;
                }
            }
            return false;
        }
        internal void flush()
        {
            bbuf = 0;
            bpos = -1;
        }
        internal void setByteArray(byte[] buf, int off, int len)
        {
            in_Renamed.setByteArray(buf, off, len);
            bbuf = 0;
            bpos = -1;
        }
    }
}