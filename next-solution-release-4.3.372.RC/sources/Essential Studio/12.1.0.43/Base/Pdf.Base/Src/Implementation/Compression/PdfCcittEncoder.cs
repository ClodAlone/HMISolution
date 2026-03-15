#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.Pdf.Compression
{
    /// <summary>
    /// Encodes data to CCITT format.
    /// </summary>
    internal class PdfCcittEncoder
    {
        #region Constants
        /// <summary>
        /// End of line.
        /// </summary>
        private const int c_G3code_Eol = -1;

        /// <summary>
        /// Invalide data.
        /// </summary>
        private const int c_G3code_Invalid = -2;

        /// <summary>
        /// End of input data.
        /// </summary>
        private const int c_G3code_Eof = -3;

        /// <summary>
        /// Incomplete run code.
        /// </summary>
        private const int c_G3code_Incomp = -4;

        /// <summary>
        /// Bit length of g3 code.
        /// </summary>
        private const int c_Length = 0;

        /// <summary>
        /// G3 code.
        /// </summary>
        private const int c_Code = 1;

        /// <summary>
        /// Run length in bits.
        /// </summary>
        private const int c_Runlen = 2;

        /// <summary>
        /// EOL code value - 0000 0000 0000 1.
        /// </summary>
        private const int c_Eol = 0x001;
        #endregion

        #region Fields
        /// <summary>
        /// Holds table zero span.
        /// </summary>
        private static byte[] s_tableZeroSpan;

        /// <summary>
        /// Holds table one span.
        /// </summary>
        private static byte[] s_tableOneSpan;

        /// <summary>
        /// Holds terminating white codes.
        /// </summary>
        private static int[][] s_terminatingWhiteCodes;

        /// <summary>
        /// Holds terminating black codes.
        /// </summary>
        private static int[][] s_terminatingBlackCodes;

        /// <summary>
        /// 001
        /// </summary>
        private static int[] s_horizontalTabel = { 3, 0x1, 0 };

        /// <summary>
        /// 0001
        /// </summary>
        private static int[] s_passcode = { 4, 0x1, 0 };

        /// <summary>
        /// holds mask table.
        /// </summary>
        private static int[] s_maskTabel = { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f, 0xff };

        /// <summary>
        /// Holds vertical table.
        /// </summary>
        private static int[][] s_verticalTable;

        /// <summary>
        /// Holds row bytes.
        /// </summary>
        private int m_rowbytes;

        /// <summary>
        /// Holds row pixels.
        /// </summary>
        private int m_rowPixels;

        /// <summary>
        /// Holds bit count.
        /// </summary>
        private int m_countBit = 8;

        /// <summary>
        /// Holds data.
        /// </summary>
        private int m_data;

        /// <summary>
        /// Holds reference line.
        /// </summary>
        private byte[] m_refline;

        /// <summary>
        /// Holds out buffer array list.
        /// </summary>
        private List<byte> m_outBuf = new List<byte>();

        /// <summary>
        /// Holds image data.
        /// </summary>
        private byte[] m_imageData;

        /// <summary>
        /// Holds offset data.
        /// </summary>
        private int m_offsetData;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfCcittEncoder"/> class.
        /// </summary>
        public PdfCcittEncoder()
        {
        }

        /// <summary>
        /// Initializes the <see cref="PdfCcittEncoder"/> class.
        /// </summary>
        static PdfCcittEncoder()
        {
            CreteTableZeroSpan();
            CreteTableOneSpan();
            CreateTerminatingWhiteCodes();
            CreateTerminatingBlackCodes();
            CreateVerticalTable();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Encodes the image.
        /// </summary>
        /// <param name="data">The image data.</param>
        /// <param name="width">The image width.</param>
        /// <param name="height">The image height.</param>
        /// <returns></returns>
        public byte[] EncodeData(byte[] data, int width, int height)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            m_rowPixels = width;
            m_rowbytes = (int)Math.Ceiling((float)m_rowPixels / 8f);
            m_refline = new byte[m_rowbytes];

            m_imageData = data;
            m_offsetData = 0;
            int sizeImageData = m_rowbytes * height;

            while (sizeImageData > 0)
            {
                Fax3Encode();
                Array.Copy(m_imageData, m_offsetData, m_refline, 0, m_rowbytes);
                m_offsetData += m_rowbytes;
                sizeImageData -= m_rowbytes;
            }

            Fax4Encode();
            byte[] result = new byte[m_outBuf.Count];
            for (int i = 0, len = m_outBuf.Count; i < len; i++)
            {
                result[i] = (byte)m_outBuf[i];
            }

            return result;
        }

        /// <summary>
        /// Creates ccitt vertical table.
        /// </summary>
        private static void CreateVerticalTable()
        {
            s_verticalTable = new int[][] { new int[]{ 7, 0x03, 0 }, // 0000011
																			new int[]{ 6, 0x03, 0 }, // 000011
																			new int[]{ 3, 0x03, 0 }, // 011
																			new int[]{ 1, 0x1, 0 },  // 1
																			new int[]{ 3, 0x2, 0 },  // 010
																			new int[]{ 6, 0x02, 0 }, // 000010
																			new int[]{ 7, 0x02, 0 }  // 0000010
																		};
        }

        /// <summary>
        /// Creates ccitt zero span.
        /// </summary>
        private static void CreteTableZeroSpan()
        {
            s_tableZeroSpan = new byte[]{ 8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, // 0x00 - 0x0f
																		3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, // 0x10 - 0x1f 
																		2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, // 0x20 - 0x2f 
																		2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, // 0x30 - 0x3f 
																		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x40 - 0x4f 
																		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x50 - 0x5f 
																		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x60 - 0x6f 
																		1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x70 - 0x7f 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x80 - 0x8f 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x90 - 0x9f 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0xa0 - 0xaf 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0xb0 - 0xbf 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0xc0 - 0xcf 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0xd0 - 0xdf 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0xe0 - 0xef 
																		0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0  // 0xf0 - 0xff 
																	};
        }

        /// <summary>
        /// Creates citt one span.
        /// </summary>
        private static void CreteTableOneSpan()
        {
            s_tableOneSpan = new byte[]{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x00 - 0x0f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x10 - 0x1f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x20 - 0x2f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x30 - 0x3f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x40 - 0x4f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x50 - 0x5f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x60 - 0x6f 
																	 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 0x70 - 0x7f 
																	 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x80 - 0x8f 
																	 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0x90 - 0x9f 
																	 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0xa0 - 0xaf 
																	 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, // 0xb0 - 0xbf 
																	 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, // 0xc0 - 0xcf 
																	 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, // 0xd0 - 0xdf 
																	 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, // 0xe0 - 0xef 
																	 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8  // 0xf0 - 0xff 
																 };
        }

        /// <summary>
        /// Creates terminating white codes.
        /// </summary>
        private static void CreateTerminatingWhiteCodes()
        {
            s_terminatingWhiteCodes = new int[][]{ new int[]{ 8, 0x35, 0 },             // 00110101
																						 new int[]{ 6, 0x7, 1 },              // 000111
																						 new int[]{ 4, 0x7, 2 },              // 0111 
																						 new int[]{ 4, 0x8, 3 },              // 1000
																						 new int[]{ 4, 0xB, 4 },              // 1011
																						 new int[]{ 4, 0xC, 5 },              // 1100
																						 new int[]{ 4, 0xE, 6 },              // 1110
																						 new int[]{ 4, 0xF, 7 },              // 1111
																						 new int[]{ 5, 0x13, 8 },             // 10011
																						 new int[]{ 5, 0x14, 9 },             // 10100
																						 new int[]{ 5, 0x7, 10 },             // 00111
																						 new int[]{ 5, 0x8, 11 },             // 01000
																						 new int[]{ 6, 0x8, 12 },             // 001000
																						 new int[]{ 6, 0x3, 13 },             // 000011
																						 new int[]{ 6, 0x34, 14 },            // 110100
																						 new int[]{ 6, 0x35, 15 },            // 110101
																						 new int[]{ 6, 0x2A, 16 },            // 101010
																						 new int[]{ 6, 0x2B, 17 },            // 101011
																						 new int[]{ 7, 0x27, 18 },            // 0100111
																						 new int[]{ 7, 0xC, 19 },             // 0001100
																						 new int[]{ 7, 0x8, 20 },             // 0001000
																						 new int[]{ 7, 0x17, 21 },            // 0010111
																						 new int[]{ 7, 0x3, 22 },             // 0000011
																						 new int[]{ 7, 0x4, 23 },             // 0000100
																						 new int[]{ 7, 0x28, 24 },            // 0101000
																						 new int[]{ 7, 0x2B, 25 },            // 0101011
																						 new int[]{ 7, 0x13, 26 },            // 0010011
																						 new int[]{ 7, 0x24, 27 },            // 0100100
																						 new int[]{ 7, 0x18, 28 },            // 0011000
																						 new int[]{ 8, 0x2, 29 },             // 00000010
																						 new int[]{ 8, 0x3, 30 },             // 00000011
																						 new int[]{ 8, 0x1A, 31 },            // 00011010
																						 new int[]{ 8, 0x1B, 32 },            // 00011011
																						 new int[]{ 8, 0x12, 33 },            // 00010010
																						 new int[]{ 8, 0x13, 34 },            // 00010011
																						 new int[]{ 8, 0x14, 35 },            // 00010100
																						 new int[]{ 8, 0x15, 36 },            // 00010101
																						 new int[]{ 8, 0x16, 37 },            // 00010110
																						 new int[]{ 8, 0x17, 38 },            // 00010111
																						 new int[]{ 8, 0x28, 39 },            // 00101000
																						 new int[]{ 8, 0x29, 40 },            // 00101001
																						 new int[]{ 8, 0x2A, 41 },            // 00101010
																						 new int[]{ 8, 0x2B, 42 },            // 00101011
																						 new int[]{ 8, 0x2C, 43 },            // 00101100
																						 new int[]{ 8, 0x2D, 44 },            // 00101101
																						 new int[]{ 8, 0x4, 45 },             // 00000100
																						 new int[]{ 8, 0x5, 46 },             // 00000101
																						 new int[]{ 8, 0xA, 47 },             // 00001010
																						 new int[]{ 8, 0xB, 48 },             // 00001011
																						 new int[]{ 8, 0x52, 49 },            // 01010010
																						 new int[]{ 8, 0x53, 50 },            // 01010011
																						 new int[]{ 8, 0x54, 51 },            // 01010100
																						 new int[]{ 8, 0x55, 52 },            // 01010101
																						 new int[]{ 8, 0x24, 53 },            // 00100100
																						 new int[]{ 8, 0x25, 54 },            // 00100101
																						 new int[]{ 8, 0x58, 55 },            // 01011000
																						 new int[]{ 8, 0x59, 56 },            // 01011001
																						 new int[]{ 8, 0x5A, 57 },            // 01011010
																						 new int[]{ 8, 0x5B, 58 },            // 01011011
																						 new int[]{ 8, 0x4A, 59 },            // 01001010
																						 new int[]{ 8, 0x4B, 60 },            // 01001011
																						 new int[]{ 8, 0x32, 61 },            // 00110010
																						 new int[]{ 8, 0x33, 62 },            // 00110011
																						 new int[]{ 8, 0x34, 63 },            // 00110100
																						 new int[]{ 5, 0x1B, 64 },            // 11011
																						 new int[]{ 5, 0x12, 128 },           // 10010
																						 new int[]{ 6, 0x17, 192 },           // 010111
																						 new int[]{ 7, 0x37, 256 },           // 0110111
																						 new int[]{ 8, 0x36, 320 },           // 00110110
																						 new int[]{ 8, 0x37, 384 },           // 00110111
																						 new int[]{ 8, 0x64, 448 },           // 01100100
																						 new int[]{ 8, 0x65, 512 },           // 01100101
																						 new int[]{ 8, 0x68, 576 },           // 01101000
																						 new int[]{ 8, 0x67, 640 },           // 01100111
																						 new int[]{ 9, 0xCC, 704 },           // 011001100
																						 new int[]{ 9, 0xCD, 768 },           // 011001101
																						 new int[]{ 9, 0xD2, 832 },           // 011010010
																						 new int[]{ 9, 0xD3, 896 },           // 011010011
																						 new int[]{ 9, 0xD4, 960 },           // 011010100
																						 new int[]{ 9, 0xD5, 1024 },          // 011010101
																						 new int[]{ 9, 0xD6, 1088 },          // 011010110
																						 new int[]{ 9, 0xD7, 1152 },          // 011010111
																						 new int[]{ 9, 0xD8, 1216 },          // 011011000
																						 new int[]{ 9, 0xD9, 1280 },          // 011011001
																						 new int[]{ 9, 0xDA, 1344 },          // 011011010
																						 new int[]{ 9, 0xDB, 1408 },          // 011011011
																						 new int[]{ 9, 0x98, 1472 },          // 010011000
																						 new int[]{ 9, 0x99, 1536 },          // 010011001
																						 new int[]{ 9, 0x9A, 1600 },          // 010011010
																						 new int[]{ 6, 0x18, 1664 },          // 011000
																						 new int[]{ 9, 0x9B, 1728 },          // 010011011
																						 new int[]{ 11, 0x8, 1792 },          // 00000001000
																						 new int[]{ 11, 0xC, 1856 },          // 00000001100
																						 new int[]{ 11, 0xD, 1920 },          // 00000001101
																						 new int[]{ 12, 0x12, 1984 },         // 000000010010
																						 new int[]{ 12, 0x13, 2048 },         // 000000010011
																						 new int[]{ 12, 0x14, 2112 },         // 000000010100
																						 new int[]{ 12, 0x15, 2176 },         // 000000010101
																						 new int[]{ 12, 0x16, 2240 },         // 000000010110
																						 new int[]{ 12, 0x17, 2304 },         // 000000010111
																						 new int[]{ 12, 0x1C, 2368 },         // 000000011100
																						 new int[]{ 12, 0x1D, 2432 },         // 000000011101
																						 new int[]{ 12, 0x1E, 2496 },         // 000000011110
																						 new int[]{ 12, 0x1F, 2560 },         // 000000011111
																						 new int[]{ 12, 0x1, c_G3code_Eol },    // 000000000001
																						 new int[]{ 9, 0x1, c_G3code_Invalid }, // 000000001
																						 new int[]{ 10, 0x1, c_G3code_Invalid },// 0000000001
																						 new int[]{ 11, 0x1, c_G3code_Invalid },// 00000000001
																						 new int[]{ 12, 0x0, c_G3code_Invalid } // 000000000000
																					 };
        }

        /// <summary>
        /// Creates terminating black codes.
        /// </summary>
        private static void CreateTerminatingBlackCodes()
        {
            s_terminatingBlackCodes = new int[][]{ new int[]{ 10, 0x37, 0 },             // 0000110111
																						 new int[]{ 3, 0x2, 1 },               // 010
																						 new int[]{ 2, 0x3, 2 },               // 11
																						 new int[]{ 2, 0x2, 3 },               // 10
																						 new int[]{ 3, 0x3, 4 },               // 011
																						 new int[]{ 4, 0x3, 5 },               // 0011
																						 new int[]{ 4, 0x2, 6 },               // 0010
																						 new int[]{ 5, 0x3, 7 },               // 00011
																						 new int[]{ 6, 0x5, 8 },               // 000101
																						 new int[]{ 6, 0x4, 9 },               // 000100
																						 new int[]{ 7, 0x4, 10 },              // 0000100
																						 new int[]{ 7, 0x5, 11 },              // 0000101
																						 new int[]{ 7, 0x7, 12 },              // 0000111
																						 new int[]{ 8, 0x4, 13 },              // 00000100
																						 new int[]{ 8, 0x7, 14 },              // 00000111
																						 new int[]{ 9, 0x18, 15 },             // 000011000
																						 new int[]{ 10, 0x17, 16 },            // 0000010111
																						 new int[]{ 10, 0x18, 17 },            // 0000011000
																						 new int[]{ 10, 0x8, 18 },             // 0000001000
																						 new int[]{ 11, 0x67, 19 },            // 00001100111
																						 new int[]{ 11, 0x68, 20 },            // 00001101000
																						 new int[]{ 11, 0x6C, 21 },            // 00001101100
																						 new int[]{ 11, 0x37, 22 },            // 00000110111
																						 new int[]{ 11, 0x28, 23 },            // 00000101000
																						 new int[]{ 11, 0x17, 24 },            // 00000010111
																						 new int[]{ 11, 0x18, 25 },            // 00000011000
																						 new int[]{ 12, 0xCA, 26 },            // 000011001010
																						 new int[]{ 12, 0xCB, 27 },            // 000011001011
																						 new int[]{ 12, 0xCC, 28 },            // 000011001100
																						 new int[]{ 12, 0xCD, 29 },            // 000011001101
																						 new int[]{ 12, 0x68, 30 },            // 000001101000
																						 new int[]{ 12, 0x69, 31 },            // 000001101001
																						 new int[]{ 12, 0x6A, 32 },            // 000001101010
																						 new int[]{ 12, 0x6B, 33 },            // 000001101011
																						 new int[]{ 12, 0xD2, 34 },            // 000011010010
																						 new int[]{ 12, 0xD3, 35 },            // 000011010011
																						 new int[]{ 12, 0xD4, 36 },            // 000011010100
																						 new int[]{ 12, 0xD5, 37 },            // 000011010101
																						 new int[]{ 12, 0xD6, 38 },            // 000011010110
																						 new int[]{ 12, 0xD7, 39 },            // 000011010111
																						 new int[]{ 12, 0x6C, 40 },            // 000001101100
																						 new int[]{ 12, 0x6D, 41 },            // 000001101101
																						 new int[]{ 12, 0xDA, 42 },            // 000011011010
																						 new int[]{ 12, 0xDB, 43 },            // 000011011011
																						 new int[]{ 12, 0x54, 44 },            // 000001010100
																						 new int[]{ 12, 0x55, 45 },            // 000001010101
																						 new int[]{ 12, 0x56, 46 },            // 000001010110
																						 new int[]{ 12, 0x57, 47 },            // 000001010111
																						 new int[]{ 12, 0x64, 48 },            // 000001100100
																						 new int[]{ 12, 0x65, 49 },            // 000001100101
																						 new int[]{ 12, 0x52, 50 },            // 000001010010
																						 new int[]{ 12, 0x53, 51 },            // 000001010011
																						 new int[]{ 12, 0x24, 52 },            // 000000100100
																						 new int[]{ 12, 0x37, 53 },            // 000000110111
																						 new int[]{ 12, 0x38, 54 },            // 000000111000
																						 new int[]{ 12, 0x27, 55 },            // 000000100111
																						 new int[]{ 12, 0x28, 56 },            // 000000101000
																						 new int[]{ 12, 0x58, 57 },            // 000001011000
																						 new int[]{ 12, 0x59, 58 },            // 000001011001
																						 new int[]{ 12, 0x2B, 59 },            // 000000101011
																						 new int[]{ 12, 0x2C, 60 },            // 000000101100
																						 new int[]{ 12, 0x5A, 61 },            // 000001011010
																						 new int[]{ 12, 0x66, 62 },            // 000001100110
																						 new int[]{ 12, 0x67, 63 },            // 000001100111
																						 new int[]{ 10, 0xF, 64 },             // 0000001111
																						 new int[]{ 12, 0xC8, 128 },           // 000011001000
																						 new int[]{ 12, 0xC9, 192 },           // 000011001001
																						 new int[]{ 12, 0x5B, 256 },           // 000001011011
																						 new int[]{ 12, 0x33, 320 },           // 000000110011
																						 new int[]{ 12, 0x34, 384 },           // 000000110100
																						 new int[]{ 12, 0x35, 448 },           // 000000110101
																						 new int[]{ 13, 0x6C, 512 },           // 0000001101100
																						 new int[]{ 13, 0x6D, 576 },           // 0000001101101
																						 new int[]{ 13, 0x4A, 640 },           // 0000001001010
																						 new int[]{ 13, 0x4B, 704 },           // 0000001001011
																						 new int[]{ 13, 0x4C, 768 },           // 0000001001100
																						 new int[]{ 13, 0x4D, 832 },           // 0000001001101
																						 new int[]{ 13, 0x72, 896 },           // 0000001110010
																						 new int[]{ 13, 0x73, 960 },           // 0000001110011
																						 new int[]{ 13, 0x74, 1024 },          // 0000001110100
																						 new int[]{ 13, 0x75, 1088 },          // 0000001110101
																						 new int[]{ 13, 0x76, 1152 },          // 0000001110110
																						 new int[]{ 13, 0x77, 1216 },          // 0000001110111
																						 new int[]{ 13, 0x52, 1280 },          // 0000001010010
																						 new int[]{ 13, 0x53, 1344 },          // 0000001010011
																						 new int[]{ 13, 0x54, 1408 },          // 0000001010100
																						 new int[]{ 13, 0x55, 1472 },          // 0000001010101
																						 new int[]{ 13, 0x5A, 1536 },          // 0000001011010
																						 new int[]{ 13, 0x5B, 1600 },          // 0000001011011
																						 new int[]{ 13, 0x64, 1664 },          // 0000001100100
																						 new int[]{ 13, 0x65, 1728 },          // 0000001100101
																						 new int[]{ 11, 0x8, 1792 },           // 00000001000
																						 new int[]{ 11, 0xC, 1856 },           // 00000001100
																						 new int[]{ 11, 0xD, 1920 },           // 00000001101
																						 new int[]{ 12, 0x12, 1984 },          // 000000010010
																						 new int[]{ 12, 0x13, 2048 },          // 000000010011
																						 new int[]{ 12, 0x14, 2112 },          // 000000010100
																						 new int[]{ 12, 0x15, 2176 },          // 000000010101
																						 new int[]{ 12, 0x16, 2240 },          // 000000010110
																						 new int[]{ 12, 0x17, 2304 },          // 000000010111
																						 new int[]{ 12, 0x1C, 2368 },          // 000000011100
																						 new int[]{ 12, 0x1D, 2432 },          // 000000011101
																						 new int[]{ 12, 0x1E, 2496 },          // 000000011110
																						 new int[]{ 12, 0x1F, 2560 },          // 000000011111
																						 new int[]{ 12, 0x1, c_G3code_Eol },     // 000000000001
																						 new int[]{ 9, 0x1, c_G3code_Invalid },  // 000000001
																						 new int[]{ 10, 0x1, c_G3code_Invalid }, // 0000000001
																						 new int[]{ 11, 0x1, c_G3code_Invalid }, // 00000000001
																						 new int[]{ 12, 0x0, c_G3code_Invalid }  // 000000000000
																					 };
        }

        /// <summary>
        /// Putcodes the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        private void Putcode(int[] table)
        {
            PutBits(table[c_Code], table[c_Length]);
        }

        /// <summary>
        /// Putspans the specified span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <param name="tab">The tab.</param>
        private void PutSpan(int span, int[][] tab)
        {
            int code, length;

            while (span >= 2624)
            {
                int[] te = tab[63 + (2560 >> 6)];
                code = te[c_Code];
                length = te[c_Length];
                PutBits(code, length);
                span -= te[c_Runlen];
            }

            if (span >= 64)
            {
                int[] te = tab[63 + (span >> 6)];
                code = te[c_Code];
                length = te[c_Length];
                PutBits(code, length);
                span -= te[c_Runlen];
            }

            code = tab[span][c_Code];
            length = tab[span][c_Length];
            PutBits(code, length);
        }

        /// <summary>
        /// Puts the bits.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="length">The length.</param>
        private void PutBits(int bits, int length)
        {
            while (length > m_countBit)
            {
                m_data |= bits >> (length - m_countBit);
                length -= m_countBit;
                m_outBuf.Add((byte)m_data);
                m_data = 0;
                m_countBit = 8;
            }

            m_data |= (bits & s_maskTabel[length]) << (m_countBit - length);
            m_countBit -= length;

            if (m_countBit == 0)
            {
                m_outBuf.Add((byte)m_data);
                m_data = 0;
                m_countBit = 8;
            }
        }

        /// <summary>
        /// Implements Fax3Encode.
        /// </summary>
        private void Fax3Encode()
        {
            int diff0 = 0;
            int diff1 = (Pixel(m_imageData, m_offsetData, 0) != 0 ? 0 : Finddiff(m_imageData, m_offsetData, 0, m_rowPixels, 0));
            int diff3 = (Pixel(m_refline, 0, 0) != 0 ? 0 : Finddiff(m_refline, 0, 0, m_rowPixels, 0));
            int diff2;
            int diff4;

            while (true)
            {
                diff4 = Finddiff2(m_refline, 0, diff3, m_rowPixels, Pixel(m_refline, 0, diff3));
                if (diff4 >= diff1)
                {
                    int d = diff3 - diff1;

                    if (!(-3 <= d && d <= 3))
                    {
                        diff2 = Finddiff2(m_imageData, m_offsetData, diff1, m_rowPixels, Pixel(m_imageData, m_offsetData, diff1));
                        Putcode(s_horizontalTabel);

                        if (diff0 + diff1 == 0 || Pixel(m_imageData, m_offsetData, diff0) == 0)
                        {
                            PutSpan(diff1 - diff0, s_terminatingWhiteCodes);
                            PutSpan(diff2 - diff1, s_terminatingBlackCodes);
                        }
                        else
                        {
                            PutSpan(diff1 - diff0, s_terminatingBlackCodes);
                            PutSpan(diff2 - diff1, s_terminatingWhiteCodes);
                        }

                        diff0 = diff2;
                    }
                    else
                    {
                        Putcode(s_verticalTable[d + 3]);
                        diff0 = diff1;
                    }
                }
                else
                {
                    Putcode(s_passcode);
                    diff0 = diff4;
                }

                if (diff0 >= m_rowPixels)
                    break;

                diff1 = Finddiff(m_imageData, m_offsetData, diff0, m_rowPixels, Pixel(m_imageData, m_offsetData, diff0));
                diff3 = Finddiff(m_refline, 0, diff0, m_rowPixels, Pixel(m_imageData, m_offsetData, diff0) ^ 1);
                diff3 = Finddiff(m_refline, 0, diff3, m_rowPixels, Pixel(m_imageData, m_offsetData, diff0));
            }
        }

        /// <summary>
        /// Implements Fax4Encode.
        /// </summary>
        private void Fax4Encode()
        {
            PutBits(c_Eol, 12);
            PutBits(c_Eol, 12);
            if (m_countBit != 8)
            {
                m_outBuf.Add((byte)m_data);
                m_data = 0;
                m_countBit = 8;
            }
        }

        /// <summary>
        /// Pixels the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bit">The bit.</param>
        /// <returns></returns>
        private int Pixel(byte[] data, int offset, int bit)
        {
            int result = 0;

            if (bit < m_rowPixels)
            {
                result = ((data[offset + (bit >> 3)] & 0xff) >> (7 - ((bit) & 7))) & 1;
            }

            return result;
        }

        /// <summary>
        /// Finds the first span.
        /// </summary>
        /// <param name="bp">The bp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bs">The bs.</param>
        /// <param name="be">The be.</param>
        /// <returns></returns>
        private int FindFirstSpan(byte[] bp, int offset, int bs, int be)
        {
            int bits = be - bs;
            int n, span;

            int pos = offset + (bs >> 3);

            if (bits > 0 && (n = (bs & 7)) != 0)
            {
                span = s_tableOneSpan[((int)bp[pos] << n) & 0xff];

                if (span > 8 - n)
                    span = 8 - n;

                if (span > bits)
                    span = bits;

                if (n + span < 8)
                    return (span);

                bits -= span;
                pos++;
            }
            else
            {
                span = 0;
            }

            while (bits >= 8)
            {
                if (bp[pos] != 0xff)
                    return (span + s_tableOneSpan[bp[pos] & 0xff]);

                span += 8;
                bits -= 8;
                pos++;
            }

            if (bits > 0)
            {
                n = s_tableOneSpan[bp[pos] & 0xff];
                span += (n > bits ? bits : n);
            }

            return span;
        }

        /// <summary>
        /// Finds the zero span.
        /// </summary>
        /// <param name="bp">The bp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bs">The bs.</param>
        /// <param name="be">The be.</param>
        /// <returns></returns>
        private int FindZeroSpan(byte[] bp, int offset, int bs, int be)
        {
            int bits = be - bs;
            int n, span;

            int pos = offset + (bs >> 3);

            if (bits > 0 && (n = (bs & 7)) != 0)
            {
                span = s_tableZeroSpan[((int)bp[pos] << n) & 0xff];

                if (span > 8 - n)
                {
                    span = 8 - n;
                }

                if (span > bits)
                {
                    span = bits;
                }

                if (n + span < 8)
                {
                    return (span);
                }

                bits -= span;
                pos++;
            }
            else
            {
                span = 0;
            }

            while (bits >= 8)
            {
                if (bp[pos] != 0)
                {
                    return (span + s_tableZeroSpan[bp[pos] & 0xff]);
                }

                span += 8;
                bits -= 8;
                pos++;
            }

            if (bits > 0)
            {
                n = s_tableZeroSpan[bp[pos] & 0xff];
                span += (n > bits ? bits : n);
            }

            return span;
        }

        /// <summary>
        /// Finddiffs the specified bp.
        /// </summary>
        /// <param name="bp">The bp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bs">The bs.</param>
        /// <param name="be">The be.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private int Finddiff(byte[] bp, int offset, int bs, int be, int color)
        {
            return bs + (color != 0 ? FindFirstSpan(bp, offset, bs, be) : FindZeroSpan(bp, offset, bs, be));
        }

        /// <summary>
        /// Finddiff2s the specified bp.
        /// </summary>
        /// <param name="bp">The bp.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="bs">The bs.</param>
        /// <param name="be">The be.</param>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        private int Finddiff2(byte[] bp, int offset, int bs, int be, int color)
        {
            return bs < be ? Finddiff(bp, offset, bs, be, color) : be;
        }
        #endregion
    }
}
