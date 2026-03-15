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
using System.Collections;
using System.Diagnostics;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for FontFamilyNameTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class FontFamilyNameStringTable : BaseWordRecord
    {
        #region Class constants
        private ushort DEF_EXTENDED = 0xffff;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private ushort m_extendedFlag;
        /// <summary>
        /// 
        /// </summary>
        private ushort m_noStrings = 0;
        /// <summary>
        /// 
        /// </summary>
        private ushort m_extraDataLen;
        /// <summary>
        /// 
        /// </summary>
        private FontFamilyNameRecord[] m_ffnRecords = null;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                int length = 4;

                if (m_ffnRecords == null) return length;

                for (int i = 0; i < m_ffnRecords.Length; i++)
                {
                    if (m_ffnRecords[i] != null)
                    {
                        length += m_ffnRecords[i].Length;
                    }
                }

                return length;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int RecordsCount
        {
            get
            {
                if (m_ffnRecords != null)
                    return m_ffnRecords.Length;
                else
                    return 0;
            }
            set
            {
                m_ffnRecords = new FontFamilyNameRecord[value];
            }
        }
        /// <summary>
        /// Array of FontFamilyRecord objects.
        /// </summary>
        internal FontFamilyNameRecord[] FontFamilyNameRecords
        {
            get
            {
                return m_ffnRecords;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal FontFamilyNameStringTable()
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            if (arrData.Length < 2) return;
#if DEBUG_BIFFRECORD
      Debug.WriteLine( "reading fonts..." );
#endif
            m_noStrings = m_extendedFlag = ReadUInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            // if extendedFlag == 0xffff, reads noStrings from next word.
            if (m_extendedFlag == DEF_EXTENDED)
            {
                m_noStrings = ReadUInt16(arrData, ref iOffset);
            }

            // gets extradata length.
            m_extraDataLen = ReadUInt16(arrData, ref iOffset);
            iCount = arrData.Length - iOffset;
            m_ffnRecords = new FontFamilyNameRecord[m_noStrings];

            for (int i = 0; i < m_noStrings; i++)
            {
                FontFamilyNameRecord ffnRecord = m_ffnRecords[i] = new FontFamilyNameRecord();

                ffnRecord.Parse(arrData, iOffset, iCount);
                iOffset += ffnRecord.Length;
                iCount -= ffnRecord.Length;

#if DEBUG_BIFFRECORD
        Debug.WriteLine( "font " + ffnRecord.FontName );
#endif
            }
#if DEBUG_BIFFRECORD
      Debug.WriteLine( "done reading fonts." );
#endif
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
#if DEBUG_BIFFRECORD
      Debug.WriteLine( "saving fonts..." );
#endif
            int iLength = 0;
            m_noStrings = (ushort)m_ffnRecords.Length;
            WriteUInt16(arrData, m_noStrings, ref iOffset);
            WriteUInt16(arrData, m_extraDataLen, ref iOffset);

            for (int i = 0; i < m_ffnRecords.Length; i++)
            {
                iOffset = m_ffnRecords[i].Save(arrData, iOffset);
            }

#if DEBUG_BIFFRECORD
      Debug.WriteLine( "done saving fonts" );
#endif
            return iLength;
        }
        #endregion
    }
}