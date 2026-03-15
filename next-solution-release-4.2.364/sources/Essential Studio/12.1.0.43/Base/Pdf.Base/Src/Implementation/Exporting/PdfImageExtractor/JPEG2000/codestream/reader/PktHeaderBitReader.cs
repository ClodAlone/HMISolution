#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Pdf.JPEG2000.io;
namespace Syncfusion.Pdf.JPEG2000.codestream.reader
{
    internal class PktHeaderBitReader
    {
        internal JPXRandomAccessStream in_Renamed;
        internal System.IO.MemoryStream bais;
        internal bool usebais;
        internal int bbuf;
        internal int bpos;
        internal int nextbbuf;
        internal PktHeaderBitReader(JPXRandomAccessStream in_Renamed)
        {
            this.in_Renamed = in_Renamed;
            usebais = false;
        }
        internal PktHeaderBitReader(System.IO.MemoryStream bais)
        {
            this.bais = bais;
            usebais = true;
        }
        internal int readBit()
        {
            if (bpos == 0)
            {
                if (bbuf != 0xFF)
                {
                    if (usebais)
                    {
                        bbuf = bais.ReadByte();
                    }
                    else
                    {
                        bbuf = in_Renamed.read();
                    }
                    bpos = 8;
                    if (bbuf == 0xFF)
                    {
                        if (usebais)
                        {
                            nextbbuf = bais.ReadByte();
                        }
                        else
                        {
                            nextbbuf = in_Renamed.read();
                        }
                    }
                }
                else
                {
                    bbuf = nextbbuf;
                    bpos = 7;
                }
            }
            return (bbuf >> --bpos) & 0x01;
        }
        internal int readBits(int n)
        {
            int bits;
            if (n <= bpos)
            {
                return (bbuf >> (bpos -= n)) & ((1 << n) - 1);
            }
            else
            {
                bits = 0;
                do
                {
                    bits <<= bpos;
                    n -= bpos;
                    bits |= readBits(bpos);
                    if (bbuf != 0xFF)
                    {
                        if (usebais)
                        {
                            bbuf = bais.ReadByte();
                        }
                        else
                        {
                            bbuf = in_Renamed.read();
                        }
                        bpos = 8;
                        if (bbuf == 0xFF)
                        {
                            if (usebais)
                            {
                                nextbbuf = bais.ReadByte();
                            }
                            else
                            {
                                nextbbuf = in_Renamed.read();
                            }
                        }
                    }
                    else
                    {
                        bbuf = nextbbuf;
                        bpos = 7;
                    }
                }
                while (n > bpos);
                bits <<= n;
                bits |= (bbuf >> (bpos -= n)) & ((1 << n) - 1);
                return bits;
            }
        }
        internal virtual void sync()
        {
            bbuf = 0;
            bpos = 0;
        }
        internal virtual void setInput(JPXRandomAccessStream in_Renamed)
        {
            this.in_Renamed = in_Renamed;
            bbuf = 0;
            bpos = 0;
        }
        internal virtual void setInput(System.IO.MemoryStream bais)
        {
            this.bais = bais;
            bbuf = 0;
            bpos = 0;
        }
    }
}