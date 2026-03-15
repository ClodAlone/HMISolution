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
    class Jbig2StreamReader
    {
        private byte[] m_data;
        private int m_bitPointer = 7;

        internal int bytePointer = 0;

        internal Jbig2StreamReader(byte[] data)
        {
            this.m_data = data;
        }

        internal short ReadByte()
        {
            short bite = (short)(m_data[bytePointer++] & 255);

            return bite;
        }

        internal void ReadByte(short[] buf)
        {
            for (int i = 0; i < buf.Length; i++)
            {
                if (bytePointer < m_data.Length)
                    buf[i] = (short)(m_data[bytePointer++] & 255);
            }
        }

        internal int ReadBit()
        {
            short buf = ReadByte();
            short mask = (short)(1 << m_bitPointer);

            int bit = (buf & mask) >> m_bitPointer;

            m_bitPointer--;
            if (m_bitPointer == -1)
            {
                m_bitPointer = 7;
            }
            else
            {
                MovePointer(-1);
            }

            return bit;
        }

        internal int ReadBits(int num)
        {
            int result = 0;

            for (int i = 0; i < num; i++)
            {
                result = (result << 1) | ReadBit();
            }

            return result;
        }

        internal void MovePointer(int ammount)
        {
            bytePointer += ammount;
        }

        internal void ConsumeRemainingBits()
        {
            if (m_bitPointer != 7)
            {
                ReadBits(m_bitPointer + 1);
            }
        }

        internal bool Getfinished()
        {
            return bytePointer == m_data.Length;
        }
    }
}
