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
using System.Diagnostics;
using System.Reflection;
using System.Text;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// <para>Font family name record.
    /// Note that just as for a pascal-style string, the first byte in the 
    /// FFN records the total number of bytes not counting the count byte 
    /// itself.</para>
    /// <para>The names of the fonts correspond to the ftc codes in the 
    /// CHP structure. For example, the first font name listed corresponds 
    /// is the name for ftc = 0</para>
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FontFamilyNameRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// Maximal length of string that represent font names.
        /// </summary>
        private const int DEF_MAX_LENGTH = 130;
        /// <summary>
        /// Bits masks and offsets for FFNBaseStructure.Options property.
        /// </summary>
        private const int DEF_PITCHREQUEST_MASK = 0x03;
        private const int DEF_ISTRUETYPE_MASK = 0x04;
        private const int DEF_ISTRUETYPE_OFFSET = 2;
        private const int DEF_FONTFAMILYID_MASK = 0x70;
        private const int DEF_FONTFAMILYID_OFFSET = 4;
        #endregion

        #region Class members
        /// <summary>
        /// FFN base structure.
        /// </summary>
        private FFNBaseStructure m_ffnBase = new FFNBaseStructure();
        /// <summary>
        /// The font name.
        /// </summary>
        private string m_strFontName = string.Empty;
        /// <summary>
        /// The alternative font name.
        /// </summary>
        private string m_strAltFontName = string.Empty;
        private byte[] m_dbgFontName;
        #endregion

        #region Class properties
        /// <summary>
        /// Underlying structure ( used in BaseWordRecord for fast copy data
        /// to special structure )
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_ffnBase;
            }
        }
        /// <summary>
        /// Length of ffn minus 1.
        /// </summary>
        private byte TotalLengthM1
        {
            get
            {
                return m_ffnBase.TotalLengthM1;
            }
        }
        /// <summary>
        /// Length of record.
        /// </summary>
        internal override int Length
        {
            get
            {
                return TotalLengthM1 + 1;
            }
        }
        /// <summary>
        /// Gets or sets pitch request.
        /// </summary>
        internal byte PitchRequest
        {
            get
            {
                return (byte)GetBitsByMask(m_ffnBase.Options, DEF_PITCHREQUEST_MASK, 0);
            }
            set
            {
                if (PitchRequest != value)
                {
                    m_ffnBase.Options = (byte)SetBitsByMask(m_ffnBase.Options, DEF_PITCHREQUEST_MASK, value);
                }
            }
        }
        /// <summary>
        /// Gets or sets TrueType flag for font.
        /// </summary>
        internal bool TrueType
        {
            get
            {
                return GetBit(m_ffnBase.Options, DEF_ISTRUETYPE_MASK);
            }
            set
            {
                if (TrueType != value)
                {
                    m_ffnBase.Options = (byte)SetBit(m_ffnBase.Options, DEF_ISTRUETYPE_OFFSET, value);
                }
            }
        }
        ///<summary>
        /// Gets or sets font family id.
        ///</summary>
        internal byte FontFamilyID
        {
            get
            {
                return (byte)GetBitsByMask(m_ffnBase.Options, DEF_FONTFAMILYID_MASK,
                  DEF_FONTFAMILYID_OFFSET);
            }
            set
            {
                if (FontFamilyID != value)
                {
                    m_ffnBase.Options = (byte)SetBitsByMask(m_ffnBase.Options, (byte)DEF_FONTFAMILYID_MASK, value << DEF_FONTFAMILYID_OFFSET);
                }
            }
        }
        /// <summary>
        /// Gets or sets base weight of font.
        /// </summary>
        internal short Weight
        {
            get
            {
                return m_ffnBase.Weight;
            }
            set
            {
                if (m_ffnBase.Weight != value)
                {
                    m_ffnBase.Weight = value;
                }
            }
        }
        /// <summary>
        /// Get/set font signature of Unicode Subset Bitfields 0
        /// </summary>
        internal byte[] SigUsb0
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] =m_ffnBase.m_FONTSIGNATURE[0 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[0 + i] = value[i];
                }
            }
        }
        /// <summary>
        ///  Get/set font signature of Unicode Subset Bitfields 1
        /// </summary>
        internal byte[] SigUsb1
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = m_ffnBase.m_FONTSIGNATURE[4 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[4 + i] = value[i];
                }
            }
        }
        /// <summary>
        ///  Get/set font signature of Unicode Subset Bitfields 2
        /// </summary>
        internal byte[] SigUsb2
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = m_ffnBase.m_FONTSIGNATURE[8 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[8 + i] = value[i];
                }
            }
        }
        /// <summary>
        ///  Get/set font signature of Unicode Subset Bitfields 3
        /// </summary>
        internal byte[] SigUsb3
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = m_ffnBase.m_FONTSIGNATURE[12 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[12 + i] = value[i];
                }
            }
        }
        /// <summary>
        ///  Get/set font signature of Code Page Bitfields 0
        /// </summary>
        internal byte[] SigCsb0
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = m_ffnBase.m_FONTSIGNATURE[16 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[16 + i] = value[i];
                }
            }
        }
        /// <summary>
        /// Get/set font signature of Code Page Bitfields 1
        /// </summary>
        internal byte[] SigCsb1
        {
            get
            {
                byte[] bytes = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    bytes[i] = m_ffnBase.m_FONTSIGNATURE[20 + i];
                }
                return bytes;
            }
            set
            {
                for (int i = 0; i < 4; i++)
                {
                    m_ffnBase.m_FONTSIGNATURE[20 + i] = value[i];
                }
            }
        }
        /// <summary>
        /// Gets or sets character set identifier.
        /// </summary>
        internal byte CharacterSetId
        {
            get
            {
                return m_ffnBase.CharacterSetId;
            }
            set
            {
                if (m_ffnBase.CharacterSetId != value)
                {
                    m_ffnBase.CharacterSetId = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets font name.
        /// Summary font name and alternate font name length can not be large 65 bytes.
        /// </summary>
        internal string FontName
        {
            get
            {
                return m_strFontName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("FontName is empty or nullable!");


                if (value.Length + m_strAltFontName.Length > DEF_MAX_LENGTH)
                    throw new ArgumentOutOfRangeException("FontName can not be large ( 65 - " +
                      m_strAltFontName.ToString() + " ) symbols");

                // Sets new font name
                if (m_strFontName != value || value==string.Empty)
                {
                    Encoding encoding = Encoding.Unicode;
                    m_strFontName = value == string.Empty ? "\0" : value;
                    int iFontLength = encoding.GetByteCount(m_strFontName);
                    int iAltFontLength = encoding.GetByteCount(m_strAltFontName);

                    int totalLength = (m_ffnBase.Length + iFontLength + 1) + ((iAltFontLength > 0) ? (iAltFontLength + 2) : 0);

                    m_ffnBase.TotalLengthM1 = (byte)totalLength;
                }
            }
        }
        /// <summary>
        /// Gets/Sets alternative font name.
        /// </summary>
        internal string AlternativeFontName
        {
            get
            {
                return m_strAltFontName;
            }
            set
            {
                m_strAltFontName = value;
            }
        }
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    internal byte[] DBG_FontName
        //    {
        //      get
        //      {
        //        return m_dbgFontName;
        //      }
        //    }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal FontFamilyNameRecord()
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// Exstract record data from specified byte array.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            // Gets FFN base structure data.
            base.Parse(arrData, iOffset, iCount);

            int iBaseLength = m_ffnBase.Length;
            iOffset += iBaseLength;
            int iLength = Length - iBaseLength;

            if (iLength > DEF_MAX_LENGTH)
            {
#if DEBUG_BIFFRECORD
        Debug.WriteLine( "(!)string length(" + iLength.ToString() +
          ") trucated to " + DEF_MAX_LENGTH.ToString() );
#endif
                iLength = DEF_MAX_LENGTH;
            }

            m_dbgFontName = new byte[iLength];
            Array.Copy(arrData, iOffset, m_dbgFontName, 0, iLength);
            // Gets font name string.
            m_strFontName = ReadString(arrData, iOffset, (ushort)(iLength - 2));
            
            if (m_ffnBase.AlternateFontIndex != 0 && m_ffnBase.AlternateFontIndex < m_strFontName.Length)
            {
                m_strAltFontName = m_strFontName.Substring(m_ffnBase.AlternateFontIndex);
                m_strFontName = m_strFontName.Substring(0, m_ffnBase.AlternateFontIndex - 1);
                if (m_strFontName == string.Empty)
                    m_strFontName = m_strAltFontName;
            }
            else
            {
                char[] split = { '\0' };
                string[] fontname = m_strFontName.Split(split);
                m_strFontName = fontname[0];
            }
        }
        /// <summary>
        /// Save record data in array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            int iStartOffset = iOffset;
            
            if (AlternativeFontName != string.Empty)
            {
                m_ffnBase.AlternateFontIndex = (byte)(m_strFontName.Length + 1);
                m_strFontName += '\0'.ToString() + AlternativeFontName;
            }

            // Save part of data from FFN base structure.
            base.Save(arrData, iOffset);
            iOffset += m_ffnBase.Length;

            // Save font name.
            WriteString(arrData, m_strFontName, ref iOffset);
            arrData[iOffset++] = 0;

            if (Length % 2 == 0)
            {
                arrData[iOffset++] = 0;
            }
            if (iOffset - iStartOffset != Length)
                throw new Exception("Length of FFN record data is incorrect!");

            return iOffset;
        }
        #endregion
    }
}