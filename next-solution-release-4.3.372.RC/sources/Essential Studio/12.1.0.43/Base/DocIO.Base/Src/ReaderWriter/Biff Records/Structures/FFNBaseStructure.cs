#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Runtime.InteropServices;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures
{
    /// <summary>
    /// Base structure for FontFamilyNameRecord.
    /// </summary>
    [CLSCompliant(false)]
#if AllowUnsafeCode && !SILVERLIGHT && !WP
    [StructLayout(LayoutKind.Sequential)]
#endif
    internal class FFNBaseStructure : DataStructure
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_RECORD_LENGTH = 40;
        private const int DEF_PANOSE_SIZE = 10;
        private const int DEF_FONTSIGNATURE_SIZE = 24;
        #endregion

        #region Class members
        /// <summary>
        /// Total length of FFN - 1.
        /// </summary>
        ///[ FieldOffset( 0 ) ]
        private byte m_btTotalLength;

        /// <summary>
        /// Option flags.
        /// <list>
        ///  2bits - pitch request 
        ///  1bit  - when 1, font is a TrueType font 
        ///  1bit  - reserved 
        ///  3bits - font family id 
        ///  1bit  - reserved 
        /// </list>
        /// </summary>
        ///[ FieldOffset( 1 ) ]
        private byte m_btOptions;

        /// <summary>
        /// Base weight of font.
        /// </summary>
        ///[ FieldOffset( 2 ) ]
        private short m_wWeight;

        /// <summary>
        /// Character set identifier.
        /// </summary>
        ///[ FieldOffset( 4 ) ]
        private byte m_btCharacterSetId;

        /// <summary>
        /// Index into ffn.szFfn to the name of the alternate font.
        /// </summary>
        ///[ FieldOffset( 5 ) ]
        private byte m_btAlternateFontIndex;

        /// <summary>
        /// "Magic" data.
        /// <sample>
        /// { 0x2, 0x2, 0x6, 0x3, 0x5, 0x4, 0x5, 0x2, 0x3, 0x4 }
        /// </sample>
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DEF_PANOSE_SIZE)]
#endif
        private byte[] m_PANOSE = new byte[DEF_PANOSE_SIZE];

        /// <summary>
        /// "Magic" data.
        /// <sample>
        /// m_FONTSIGNATURE[ 0 ] = 0x87;
        /// m_FONTSIGNATURE[ 1 ] = 0x7a;
        /// ...
        /// m_FONTSIGNATURE[ 3 ] = 0x20;
        /// ...
        /// m_FONTSIGNATURE[ 7 ] = 0x80;
        /// m_FONTSIGNATURE[ 8 ] = 0x8;
        /// ...
        /// m_FONTSIGNATURE[ 16 ] = 0xff;
        /// m_FONTSIGNATURE[ 17 ] = 0x1;
        /// </sample>
        /// </summary>
#if AllowUnsafeCode && !SILVERLIGHT && !WP
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = DEF_FONTSIGNATURE_SIZE)]
#endif
        internal byte[] m_FONTSIGNATURE = new byte[DEF_FONTSIGNATURE_SIZE];
        #endregion

        #region Class properties
        /// <summary>
        /// Total length of FFN - 1.
        /// </summary>
        internal byte TotalLengthM1
        {
            get
            {
                return m_btTotalLength;
            }
            set
            {
                if (value != m_btTotalLength)
                {
                    m_btTotalLength = value;
                }
            }
        }

        /// <summary>
        /// Option flags.
        /// <list>
        ///  2bits - pitch request 
        ///  1bit  - when 1, font is a TrueType font 
        ///  1bit  - reserved 
        ///  3bits - font family id 
        ///  1bit  - reserved 
        /// </list>
        /// </summary>
        internal byte Options
        {
            get
            {
                return m_btOptions;
            }
            set
            {
                if (value != m_btOptions)
                {
                    m_btOptions = value;
                }
            }
        }

        /// <summary>
        /// Base weight of font.
        /// </summary>
        internal short Weight
        {
            get
            {
                return m_wWeight;
            }

            set
            {
                if (value != m_wWeight)
                {
                    m_wWeight = value;
                }
            }
        }

        /// <summary>
        /// Character set identifier.
        /// </summary>
        internal byte CharacterSetId
        {
            get
            {
                return m_btCharacterSetId;
            }
            set
            {
                if (value != m_btCharacterSetId)
                {
                    m_btCharacterSetId = value;
                }
            }
        }

        /// <summary>
        /// Index into ffn.szFfn to the name of the alternate font.
        /// </summary>
        internal byte AlternateFontIndex
        {
            get
            {
                return m_btAlternateFontIndex;
            }
            set
            {
                if (value != m_btAlternateFontIndex)
                {
                    m_btAlternateFontIndex = value;
                }
            }
        }

        /// <summary>
        /// Length of record.
        /// </summary>
        /// <value>The length.</value>
        internal override int Length
        {
            get
            {
                return DEF_RECORD_LENGTH;
            }
        }
        #endregion

        #region Implementation / override
        /// <summary>
        /// Parse the data strucure
        /// </summary>
        /// <param name="arrData">Bytes with data</param>
        /// <param name="iOffset">Offset</param>
        internal override void Parse(byte[] arrData, int iOffset)
        {
            m_btTotalLength = arrData[iOffset];
            iOffset += 1;
            m_btOptions = arrData[iOffset];
            iOffset += 1;

            m_wWeight = ReadInt16(arrData, ref iOffset);

            m_btCharacterSetId = arrData[iOffset];
            iOffset += 1;
            m_btAlternateFontIndex = arrData[iOffset];
            iOffset += 1;

            m_PANOSE = ReadBytes(arrData, DEF_PANOSE_SIZE, ref iOffset);
            m_FONTSIGNATURE = ReadBytes(arrData, DEF_FONTSIGNATURE_SIZE, ref iOffset);
        }

        /// <summary>
        /// Saves the data structure.
        /// </summary>
        /// <param name="arrData">The destination array.</param>
        /// <param name="iOffset">The offset.</param>
        internal override int Save(byte[] arrData, int iOffset)
        {
            arrData[iOffset] = m_btTotalLength;
            iOffset += 1;
            arrData[iOffset] = m_btOptions;
            iOffset += 1;

            WriteInt16(arrData, ref iOffset, m_wWeight);

            arrData[iOffset] = m_btCharacterSetId;
            iOffset += 1;
            arrData[iOffset] = m_btAlternateFontIndex;
            iOffset += 1;

            WriteBytes(arrData, ref iOffset, m_PANOSE);
            WriteBytes(arrData, ref iOffset, m_FONTSIGNATURE);

            return DEF_RECORD_LENGTH;
        }
        #endregion
    }
}