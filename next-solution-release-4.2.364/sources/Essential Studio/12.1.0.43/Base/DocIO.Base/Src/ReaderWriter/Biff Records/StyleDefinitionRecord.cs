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
using System.IO;
using System.Text;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Base part of StyleDefinition.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class StyleDefinitionRecord : BaseWordRecord
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_ID = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_ID = 0x0FFF;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_SCRATCH = 12;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_INVALID_HEIGHT = 13;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_HAS_UPE = 14;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_MASS_COPY = 15;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_TYPE_CODE = 0x0F;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_TYPE_CODE = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_BASE_STYLE = 0xFFF0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_BASE_STYLE = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_UPX_NUMBER = 0x0F;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_UPX_NUMBER = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_MASK_NEXT_STYLE = 0xFFF0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_START_NEXT_STYLE = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_AUTO_REDEFINE = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_BIT_HIDDEN = 1;
        #endregion

        #region Class members
        /// <summary>
        /// Base part of the style definition.
        /// </summary>
        private StyleDefinitionBase m_basePart = new StyleDefinitionBase();
        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    private byte[] m_arrVariable;
        /// <summary>
        /// Names of the styles (comma separated).
        /// </summary>
        private string m_strStyleName;
        /// <summary>
        /// 
        /// </summary>
        private UniversalPropertyException[] m_arrUpx;
        /// <summary>
        /// 
        /// </summary>
        private CharacterPropertyException m_chpx;
        /// <summary>
        /// 
        /// </summary>
        private ParagraphPropertyException m_papx;
        private byte[] m_data = null;
        private byte[] m_data1 = null;
        private byte[] m_data2 = null;
        private byte[] m_data3 = null;
        private byte[] m_tapx = null;
        private StyleSheetInfoRecord m_shInfo;
        //    // Variable length part of STD:
        //    private XCHAR    xstzName[2];        /* sub-names are separated by chDelimStyle */
        //    /* char  grupx[]; */
        //    /* the UPEs are not stored on the file; they are a cache of the based-on
        //               chain */
        //    /* char  grupe[]; */
        #endregion

        #region Class Properties
        /// <summary>
        /// 
        /// </summary>
        internal byte[] Tapx
        {
            get
            {
                return m_tapx;
            }
            set
            {
                m_tapx = value;
            }
        }
        /// <summary>
        /// Invariant style identifier.
        /// </summary>
        internal ushort StyleId
        {
            get
            {
                return m_basePart.StyleId;
            }
            set
            {
                m_basePart.StyleId = value;
            }
        }
        /// <summary>
        /// Spare field for any temporary use, always reset back to zero!
        /// </summary>
        internal bool IsScratch
        {
            get
            {
                return m_basePart.IsScratch;
            }
            set
            {
                m_basePart.IsScratch = value;
            }
        }
        /// <summary>
        /// PHEs of all text with this style are wrong.
        /// </summary>
        internal bool IsInvalidHeight
        {
            get
            {
                return m_basePart.IsInvalidHeight;
            }
            set
            {
                m_basePart.IsInvalidHeight = value;
            }
        }
        /// <summary>
        /// Indicates whether UPEs have been generated.
        /// </summary>
        internal bool HasUpe
        {
            get
            {
                return m_basePart.HasUpe;
            }
            set
            {
                m_basePart.HasUpe = value;
            }
        }
        /// <summary>
        /// std has been mass-copied; if unused at save time, style should be deleted.
        /// </summary>
        internal bool IsMassCopy
        {
            get
            {
                return m_basePart.IsMassCopy;
            }
            set
            {
                m_basePart.IsMassCopy = value;
            }
        }
        /// <summary>
        /// Style type code.
        /// </summary>
        internal WordStyleType TypeCode
        {
            get
            {
                return (WordStyleType)m_basePart.TypeCode;
            }
            set
            {
                if (value == WordStyleType.ParagraphStyle)
                {
                    m_papx = new ParagraphPropertyException();
                }
                else if (m_basePart.TypeCode == (ushort)WordStyleType.ParagraphStyle)
                {
                    m_papx = null;
                }

                m_basePart.TypeCode = (ushort)value;
            }
        }
        /// <summary>
        /// Base style.
        /// </summary>
        internal ushort BaseStyle
        {
            get
            {
                return m_basePart.BaseStyle;
            }
            set
            {
                m_basePart.BaseStyle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort UPEOffset
        {
            get
            {
                return m_basePart.UPEOffset;
            }
            set
            {
                m_basePart.UPEOffset = value;
            }
        }
        /// <summary>
        /// Number of UPXs (and UPEs).
        /// </summary>
        internal ushort UpxNumber
        {
            get
            {
                return m_basePart.UpxNumber;
            }
            set
            {
                m_basePart.UpxNumber = value;
            }
        }
        /// <summary>
        /// Next style.
        /// </summary>
        internal ushort NextStyleId
        {
            get
            {
                return m_basePart.NextStyleId;
            }
            set
            {
                m_basePart.NextStyleId = value;
            }
        }
        /// <summary>
        /// Auto redefine style when appropriate
        /// </summary>
        internal bool IsAutoRedefine
        {
            get
            {
                return m_basePart.IsAutoRedefine;
            }
            set
            {
                m_basePart.IsAutoRedefine = value;
            }
        }
        /// <summary>
        /// Indicates whether style is hidden from UI.
        /// </summary>
        internal bool IsHidden
        {
            get
            {
                return m_basePart.IsHidden;
            }
            set
            {
                m_basePart.IsHidden = value;
            }
        }
        //    /// <summary>
        //    /// Variable part of the style definition. Read-only.
        //    /// </summary>
        //    internal byte[] VariablePart
        //    {
        //      get
        //      {
        //        return m_arrVariable;
        //      }
        //    }
        /// <summary>
        /// Names of the style (comma separated).
        /// </summary>
        internal string StyleName
        {
            get
            {
                return m_strStyleName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Length == 0)
                    throw new ArgumentException("value - string can not be empty");

                m_strStyleName = value;
            }
        }
        /// <summary>
        /// Returns array of Universal Property Exceptions. Read-only.
        /// </summary>
        internal UniversalPropertyException[] PropertyExceptions
        {
            get
            {
                return m_arrUpx;
            }
        }
        /// <summary>
        /// Returns array of Property Exceptions. Read-only.
        /// </summary>
        internal CharacterPropertyException CharacterProperty
        {
            get
            {
                return m_chpx;
            }
            set
            {
                m_chpx = value;
            }
        }
        /// <summary>
        /// Returns array of Property Exceptions. Read-only.
        /// </summary>
        internal ParagraphPropertyException ParagraphProperty
        {
            get
            {
                return m_papx;
            }
            set
            {
                m_papx = value;
            }
        }
        /// <summary>
        /// Underlying structure.
        /// </summary>
        protected override DataStructure UnderlyingStructure
        {
            get
            {
                return m_basePart;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                // Length of STD base part
                int iResult = Math.Max(m_shInfo.STDBaseLength, m_basePart.Length);

                if (iResult % 2 != 0) ++iResult; // Make even end

                // Length of STD style name
                if (m_strStyleName != null)
                {
                    iResult += Encoding.Unicode.GetByteCount(m_strStyleName);
                }

                iResult += 4; // add zero termination ( [Nazar]: and add first 2 bytes )

                // Length of UPX part
                if (m_tapx != null && UpxNumber == 3)
                {
                    iResult += m_tapx.Length;
                }
                else
                {
                    if (m_arrUpx != null && m_arrUpx.Length > 0)
                    {
                        for (int i = 0, len = m_arrUpx.Length; i < len; i++)
                        {
#if DEBUG
                            if (m_arrUpx[i] == null) return -1;
#endif
                            iResult += m_arrUpx[i].Length;
                        }
                    }

                    // if arrUpx == null -> count papx & chpx
                    if (m_arrUpx == null)
                    {
                        iResult += UpxLength;
                    }
                }

                return iResult;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DBG_data
        {
            get
            {
                return m_data;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DBG_data1
        {
            get
            {
                return m_data1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DBG_data2
        {
            get
            {
                return m_data2;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] DBG_data3
        {
            get
            {
                return m_data3;
            }
        }
        /// <summary>
        /// Gets or sets the link style id.
        /// </summary>
        /// <value>The link style id.</value>
        internal ushort LinkStyleId
        {
            get
            {
                return m_basePart.LinkStyleId;
            }
            set
            {
                m_basePart.LinkStyleId = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is primary style; otherwise, <c>false</c>.
        /// </value>
        internal bool IsQFormat
        {
            get
            {
                return m_basePart.IsQFormat;
            }
            set
            {
                m_basePart.IsQFormat = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is priority.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is priority; otherwise, <c>false</c>.
        /// </value>
        internal bool UnhideWhenUsed
        {
            get
            {
                return m_basePart.UnhideWhenUsed;
            }
            set
            {
                m_basePart.UnhideWhenUsed = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is semi hidden.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is semi hidden; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSemiHidden
        {
            get
            {
                return m_basePart.IsSemiHidden;
            }
            set
            {
                m_basePart.IsSemiHidden = value;
            }
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Extract record's fields from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes that belongs the record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        /// <param name="info">Style sheet info.</param>
        internal void Parse(Stream stream, int iCount, StyleSheetInfoRecord info)
        {
            Clear();

            if (iCount == 0) return;

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (info == null)
                throw new ArgumentNullException("info");

            byte[] arrBuffer = new byte[iCount];
            stream.Read(arrBuffer, 0, iCount);
            stream.Position -= iCount;

#if DEBUG
            m_data = arrBuffer;
#endif
            int iSTDLength = info.STDBaseLength;

            int iArraySize = Math.Max(iSTDLength, StyleDefinitionBase.DEF_RECORD_SIZE);
            arrBuffer = new byte[iArraySize];
            stream.Read(arrBuffer, 0, iSTDLength);

#if DEBUG
            m_data1 = arrBuffer;
#endif

            Parse(arrBuffer, 0, iArraySize);

            int iVarSize = iCount - iSTDLength;

            // Each part of style definition record starts on even-byte offset
            // from the beginning of the record.
            // That's why we have to make position even.
            if (iSTDLength % 2 != 0)
            {
                stream.Position++;
                iVarSize--;
            }

            byte[] arrVariable = new byte[iVarSize];
            stream.Read(arrVariable, 0, iVarSize);
#if DEBUG
            m_data2 = arrVariable;
#endif

            int iEndPos;
            m_strStyleName = GetZeroTerminatedString(arrVariable, 0, out iEndPos);

#if DEBUG_BIFFRECORDS
      Debug.WriteLine( m_strStyleName, "Names of the style" );
#endif

#if DEBUG
            m_data3 = new byte[arrVariable.Length - iEndPos];
            Array.Copy(m_data2, iEndPos, m_data3, 0, m_data3.Length);
#endif
            ParseUpxPart(arrVariable, iEndPos);
        }
        /// <summary>
        /// 
        /// </summary>
        internal void Clear()
        {
            m_basePart.Clear();
            //m_arrVariable = null;
            m_arrUpx = null;
            m_strStyleName = null;
        }
        /// <summary>
        /// Saves record into stream.
        /// </summary>
        /// <param name="stream">Stream to save record into.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(Stream stream)
        {
            long iOffset = stream.Position;

            // Calculates UPX number / UPE offset
            if (m_tapx != null && TypeCode == WordStyleType.TableStyle)
            {
                m_basePart.UpxNumber = 3;
            }
            else
            {
                m_basePart.UpxNumber = (ushort)(m_chpx != null ? 1 : 0);
            }
            if (m_strStyleName == "No List")
            {
                //special case for "No List" style to avoid corruption. Need to implement chpx and papx based on the latest specification.
                TypeCode = WordStyleType.ListStyle;
                m_basePart.UPEOffset = 40;
            }
            else
            {
                m_basePart.UPEOffset = (ushort)(Length);
            }
            if (m_basePart.UpxNumber > 0)
            {
                m_basePart.UpxNumber += (ushort)(m_papx != null ? 1 : 0);
            }

            // Saves STD Base part
            byte[] partBuff = new byte[m_basePart.Length];
            base.Save(partBuff, 0);
            stream.Write(partBuff, 0, partBuff.Length);

            if (m_shInfo.STDBaseLength > partBuff.Length)
            {
                int emtyBuffLen = m_shInfo.STDBaseLength - partBuff.Length;
                stream.Write(new byte[emtyBuffLen], 0, emtyBuffLen);
            }

            // Saves STD style name
            byte[] arrStyleName = ToZeroTerminatedArray(m_strStyleName);
            stream.Write(arrStyleName, 0, arrStyleName.Length);

            // Saves UPX part
            if (m_basePart.UpxNumber > 0)
            {
                SaveUpxPart(stream);
            }

            return (int)(stream.Position - iOffset);
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal StyleDefinitionRecord(string styleName, ushort styleId, StyleSheetInfoRecord info)
        {
            m_strStyleName = styleName;
            m_basePart.BaseStyle = 0xfff;
            m_basePart.HasUpe = false;
            m_basePart.NextStyleId = 0;
            m_basePart.StyleId = styleId;

            TypeCode = WordStyleType.CharacterStyle;
            m_chpx = new CharacterPropertyException();

            m_shInfo = info;
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        /// <param name="info"></param>
        internal StyleDefinitionRecord(byte[] arrData, int iOffset, int iCount, StyleSheetInfoRecord info)
            : base(arrData, iOffset, iCount)
        {
            m_shInfo = info;
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes that belongs the record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        /// <param name="info">Style sheet info.</param>
        internal StyleDefinitionRecord(Stream stream, int iCount, StyleSheetInfoRecord info)
        {
            m_shInfo = info;
            Parse(stream, iCount, info);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        private int UpxLength
        {
            get
            {
                if (m_strStyleName == "No List")
                {
                    return 4;
                }
                if (UpxNumber == 3 && m_tapx != null)
                {
                    return m_tapx.Length;
                }

                int iLength = 2;

                if (m_papx != null)
                {
                    iLength += 2 + m_papx.Length;

                    if (iLength % 2 != 0) ++iLength;
                }

                if (m_chpx != null)
                    iLength += m_chpx.PropertyModifiers.Length;

                if (iLength % 2 != 0) ++iLength;

                return iLength;
            }
        }
        /// <summary>
        /// Parses UPX part of the variable part.
        /// </summary>
        /// <param name="arrVariable"></param>
        /// <param name="iStartPos"></param>
        /// <param name="converter"></param>
        private void ParseUpxPart(byte[] arrVariable, int iStartPos)
        {
            m_arrUpx = new UniversalPropertyException[m_basePart.UpxNumber];

            if (UpxNumber == 3)
            {
                m_tapx = new byte[arrVariable.Length - iStartPos];
                TypeCode = WordStyleType.TableStyle;
                Buffer.BlockCopy(arrVariable, iStartPos, m_tapx, 0, arrVariable.Length - iStartPos);
                return;
            }
            for (int i = 0; i < m_basePart.UpxNumber; i++)
            {
                iStartPos = MakeEven(iStartPos);
                ushort usSize = BitConverter.ToUInt16(arrVariable, iStartPos);
                iStartPos += Constants.BytesInWord;

                if (usSize == 0) continue;

                // TODO: Create UPX on bytes.
                m_arrUpx[i] = new UniversalPropertyException(arrVariable, iStartPos, usSize);

                // Extract concrete properties modificators
                if (m_basePart.UpxNumber == 1 || (UpxNumber == 2 && i == 1))
                {
                    m_chpx = new CharacterPropertyException(m_arrUpx[i]);
                }
                else if (m_basePart.UpxNumber == 2 && i == 0)
                {
                    m_papx = new ParagraphPropertyException(m_arrUpx[i]);
                }

                iStartPos += usSize;
                iStartPos = MakeEven(iStartPos);
            }
        }
        /// <summary>
        /// Returns even number based on specified number.
        /// </summary>
        /// <param name="iStartPos"></param>
        /// <returns></returns>
        private int MakeEven(int iStartPos)
        {
            if (iStartPos % 2 == 0) return iStartPos;

            return ++iStartPos;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="converter"></param>
        private void SaveUpxPart(Stream stream)
        {
            long iUpxStart = stream.Position;
            if (UpxNumber == 3 && m_tapx != null)
            {
                stream.Write(m_tapx, 0, m_tapx.Length);
            }
            else if (m_strStyleName == "No List")
            {
                //special case for "No List" style to avoid corruption. Need to implement chpx and papx based on the latest specification.
                stream.Write(new byte[2] { 2, 0 }, 0, 2);
                stream.Write(BitConverter.GetBytes(StyleId), 0, 2);
            }
            else
            {
                if (m_papx != null)
                {
                    ushort usSize = (ushort)m_papx.Length;
                    byte[] usSizeArr = BitConverter.GetBytes(usSize);
                    stream.Write(usSizeArr, 0, usSizeArr.Length);
                    int pLen = m_papx.Save(stream);

                    if (usSize != pLen)
                    {
                        throw new StreamWriteException("Incorrect writing UPX(pap) to file");
                    }

                    if (usSize % 2 != 0)
                    {
                        stream.WriteByte(0);
                    }
                }

                ushort usSize1 = (ushort)m_chpx.PropertyModifiers.Length;
                byte[] usSizeArr1 = BitConverter.GetBytes(usSize1);
                stream.Write(usSizeArr1, 0, usSizeArr1.Length);
                int cLen = m_chpx.PropertyModifiers.Save(stream);
                //      BinaryWriter writer = new BinaryWriter( stream );
                //      int cLen = m_chpx.PropertyModifiers.Save( writer, stream );
                //m_chpx.Save( stream, converter );

                if (usSize1 != cLen)
                {
                    throw new StreamWriteException("Incorrect writing UPX(chp) to file");
                }

                if (usSize1 % 2 != 0)
                {
                    stream.WriteByte(0);
                }
            }
            if (stream.Position - iUpxStart != UpxLength)
            {
                throw new StreamWriteException("Incorrect writing UPX to file, invalid UPX Length");
            }
        }
        #endregion
    }
}