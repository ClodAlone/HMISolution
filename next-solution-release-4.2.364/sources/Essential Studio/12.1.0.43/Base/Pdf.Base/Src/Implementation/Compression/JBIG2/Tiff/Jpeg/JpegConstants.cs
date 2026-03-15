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

namespace Syncfusion.Pdf.Compression.JBIG2
{
    /// <summary>
    /// Defines some JPEG constants.
    /// </summary>
    static class JpegConstants
    {
        /// <summary>
        /// The basic DCT block is 8x8 samples
        /// </summary>
        public const int DCTSIZE = 8;

        /// <summary>
        /// DCTSIZE squared; the number of elements in a block. 
        /// </summary>
        public const int DCTSIZE2 = DCTSIZE * DCTSIZE;

        /// <summary>
        /// Quantization tables are numbered 0..3 
        /// </summary>
        public const int NUM_QUANT_TBLS = 4;

        /// <summary>
        /// Huffman tables are numbered 0..3
        /// </summary>
        public const int NUM_HUFF_TBLS = 4;

        /// <summary>
        /// JPEG limit on the number of components in one scan.
        /// </summary>
        public const int MAX_COMPS_IN_SCAN = 4;

        /// <summary>
        /// Compressor's limit on blocks per MCU.
        /// </summary>
        public const int C_MAX_BLOCKS_IN_MCU = 10;

        /// <summary>
        /// Decompressor's limit on blocks per MCU.
        /// </summary>
        public const int D_MAX_BLOCKS_IN_MCU = 10;
        
        /// <summary>
        /// JPEG limit on sampling factors.
        /// </summary>
        public const int MAX_SAMP_FACTOR = 4;

        /// <summary>
        /// Maximum number of color channels allowed in JPEG image.
        /// </summary>
        public const int MAX_COMPONENTS = 10;

        /// <summary>
        /// The size of sample.
        /// </summary>
        public const int BITS_IN_JSAMPLE = 8;

        /// <summary>
        /// DCT method used by default.
        /// </summary>
        public const J_DCT_METHOD JDCT_DEFAULT = J_DCT_METHOD.JDCT_ISLOW;

        /// <summary>
        /// Fastest DCT method.
        /// </summary>
        public const J_DCT_METHOD JDCT_FASTEST = J_DCT_METHOD.JDCT_IFAST;

        /// <summary>
        /// A tad under 64K to prevent overflows. 
        /// </summary>
        public const int JPEG_MAX_DIMENSION = 65500;

        /// <summary>
        /// The maximum sample value.
        /// </summary>
        public const int MAXJSAMPLE = 255;

        /// <summary>
        /// The medium sample value.
        /// </summary>
        public const int CENTERJSAMPLE = 128;

        /// <summary>
        /// Offset of Red in an RGB scanline element. 
        /// </summary>
        public const int RGB_RED = 0;

        /// <summary>
        /// Offset of Green in an RGB scanline element. 
        /// </summary>
        public const int RGB_GREEN = 1;

        /// <summary>
        /// Offset of Blue in an RGB scanline element. 
        /// </summary>
        public const int RGB_BLUE = 2;

        /// <summary>
        /// Bytes per RGB scanline element.
        /// </summary>
        public const int RGB_PIXELSIZE = 3;

        /// <summary>
        /// The number of bits of lookahead.
        /// </summary>
        public const int HUFF_LOOKAHEAD = 8;
    }

    /// <summary>
    /// DCT coefficient quantization tables.
    /// </summary>
    class JQUANT_TBL
    {
        private bool m_sent_table;
        internal readonly short[] quantval = new short[JpegConstants.DCTSIZE2];

        internal JQUANT_TBL()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the table has been output to file.
        /// </summary>
        public bool Sent_table
        {
            get { return m_sent_table; }
            set { m_sent_table = value; }
        }
    }

    /// <summary>
    /// JPEG virtual array.
    /// </summary>
    class jvirt_array<T>
    {
        internal delegate T[][] Allocator(int width, int height);

        private CommonStruct m_cinfo;

        private T[][] m_buffer;

        /// <summary>
        /// Request a virtual 2-D array
        /// </summary>
        internal jvirt_array(int width, int height, Allocator allocator)
        {
            m_cinfo = null;
            m_buffer = allocator(width, height);
        }

        /// <summary>
        /// Gets or sets the error processor.
        /// </summary>
        public CommonStruct ErrorProcessor
        {
            get { return m_cinfo; }
            set { m_cinfo = value; }
        }

        /// <summary>
        /// Access the part of a virtual array.
        /// </summary>
        public T[][] Access(int startRow, int numberOfRows)
        {
            if (startRow + numberOfRows > m_buffer.Length)
            {
                if (m_cinfo != null)
                { }//    m_cinfo.ERREXIT(J_MESSAGE_CODE.JERR_BAD_VIRTUAL_ACCESS);
                else
                    throw new InvalidOperationException("Bogus virtual array access");
            }

            T[][] ret = new T[numberOfRows][];
            for (int i = 0; i < numberOfRows; i++)
                ret[i] = m_buffer[startRow + i];

            return ret;
        }
    }

    /// <summary>
    /// One block of coefficients.
    /// </summary>
    class JBLOCK
    {
        internal short[] data = new short[JpegConstants.DCTSIZE2];

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        public short this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }
    }

    /// <summary>
    /// Huffman coding table.
    /// </summary>
    class JHUFF_TBL
    {
        private readonly byte[] m_bits = new byte[17];
        private readonly byte[] m_huffval = new byte[256];
        private bool m_sent_table;


        internal JHUFF_TBL()
        {
        }

        internal byte[] Bits
        {
            get { return m_bits; }
        }

        internal byte[] Huffval
        {
            get { return m_huffval; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the table has been output to file.
        /// </summary>
        public bool Sent_table
        {
            get { return m_sent_table; }
            set { m_sent_table = value; }
        }
    }
}
