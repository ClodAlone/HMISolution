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

namespace Syncfusion.Pdf.Compression.JBIG2.Internal
{
    /// <summary>
    /// Bitreading state saved across MCUs
    /// </summary>
    struct BitReadPermState
    {
        public int get_buffer;
        public int bits_left;
    }

    /// <summary>
    /// Bitreading working state within an MCU
    /// </summary>
    struct BitReadWorkingState
    {
        public int get_buffer;
        public int bits_left;
        public DecompressStruct cinfo;
    }

    /// <summary>
    /// Encapsulates buffer of image samples for one color component
    /// </summary>
    class ComponentBuffer
    {
        private byte[][] m_buffer;
        private int[] m_funnyIndices;
        private int m_funnyOffset;

        public ComponentBuffer()
        {
        }

        public ComponentBuffer(byte[][] buf, int[] funnyIndices, int funnyOffset)
        {
            SetBuffer(buf, funnyIndices, funnyOffset);
        }

        public void SetBuffer(byte[][] buf, int[] funnyIndices, int funnyOffset)
        {
            m_buffer = buf;
            m_funnyIndices = funnyIndices;
            m_funnyOffset = funnyOffset;
        }

        public byte[] this[int i]
        {
            get
            {
                if (m_funnyIndices == null)
                    return m_buffer[i];

                return m_buffer[m_funnyIndices[i + m_funnyOffset]];
            }
        }
    }

    /// <summary>
    /// Derived data constructed for each Huffman table
    /// </summary>
    class DDerivedTbl
    {
        public int[] maxcode = new int[18];
        public int[] valoffset = new int[17];
        public JHUFF_TBL pub;
        public int[] look_nbits = new int[1 << JpegConstants.HUFF_LOOKAHEAD];
        public byte[] look_sym = new byte[1 << JpegConstants.HUFF_LOOKAHEAD];
    }
}
