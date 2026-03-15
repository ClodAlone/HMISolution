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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for ParagraphPropertyException.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ParagraphPropertyException : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Style id.
        /// </summary>
        protected ushort m_usStyleId;
        /// <summary>
        /// </summary>
        protected SinglePropertyModifierArray m_arrSprms = new SinglePropertyModifierArray();
        /// <summary>
        /// </summary>
        private bool m_isHugePapx = false;
        //    /// <summary>
        //    /// </summary>
        //    protected byte[] m_sprmBytes;
        //    protected bool m_saveByteArray = true; 
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ParagraphPropertyException()
        { }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal ParagraphPropertyException(byte[] data)
        {
            Parse(data);
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal ParagraphPropertyException(byte[] arrData, int iOffset)
        {
            Parse(arrData, iOffset);
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        internal ParagraphPropertyException(byte[] arrData, int iOffset, int iCount)
        {
            Parse(arrData, iOffset, iCount);
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="isHugePapx">if set to <c>true</c> [is huge papx].</param>
        internal ParagraphPropertyException(Stream stream, int iCount, bool isHugePapx)
        {
            m_isHugePapx = isHugePapx;
            Parse(stream, iCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="converter"></param>
        internal ParagraphPropertyException(UniversalPropertyException property)
        {
            byte[] arrData = property.Data;
            Parse(arrData, 0, arrData.Length);
        }
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
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            if (iCount < Constants.BytesInWord)
                throw new ArgumentOutOfRangeException("iCount");

            if (iCount + iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iCount + iOffset");

            if (!m_isHugePapx)
            {
                m_usStyleId = BitConverter.ToUInt16(arrData, 0);
                iOffset += Constants.BytesInWord;
                iCount -= Constants.BytesInWord;
            }

            //base.Parse( arrData, iOffset, iCount, converter );
            m_arrSprms.Parse(arrData, iOffset, iCount);
        }
        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(Stream stream)
        {
            int iLength = 2;
            stream.Write(BitConverter.GetBytes(m_usStyleId), 0, Constants.BytesInWord);

            if (m_arrSprms != null)
            {
                iLength += m_arrSprms.Save(stream);
            }

            return iLength;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        internal int SaveHugePapx(Stream stream)
        {
            int iLength = 0;

            if (m_arrSprms != null)
            {
                iLength += m_arrSprms.Save(stream);
            }

            return iLength;
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets / sets StyleId.
        /// </summary>
        internal ushort StyleIndex
        {
            get
            {
                return m_usStyleId;
            }
            set
            {
                m_usStyleId = value;
            }
        }
        /// <summary>
        /// A list of the sprms (Single PRoperty modifier) that encode
        /// the differences between CHP (CHaracter property) for a run
        /// of text and the CHP generated by the paragraph and character
        /// styles that tag the run.
        /// </summary>
        internal SinglePropertyModifierArray PropertyModifiers
        {
            get
            {
                return m_arrSprms;
            }
            set
            {
                m_arrSprms = value;
            }

        }
        /// <summary>
        /// Returns number of property modifiers. Read-only.
        /// </summary>
        internal int ModifiersCount
        {
            get
            {
                return m_arrSprms.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return 2 + m_arrSprms.Length;
            }
        }
        #endregion

        #region Class Modifier Special properties
        /// <summary>
        /// 
        /// </summary>
        internal ushort ParagraphStyleId
        {
            get
            {
                return m_arrSprms.GetUShort(WordSprmOptions.sprmPIstd, 0);
            }
            set
            {
                if (ParagraphStyleId != value)
                {
                    //TODO: validates is stid is correct!
                    m_arrSprms.SetValue(WordSprmOptions.sprmPIstd, value);
                }
            }
        }
        #endregion

        #region Class Modifier properties
        /// <summary>
        /// 
        /// </summary>
        internal ParagraphJustify Justification
        {
            get
            {
                return (ParagraphJustify)m_arrSprms.GetByte(WordSprmOptions.sprmPJc, 0);
            }
            set
            {
                m_arrSprms.SetValue(WordSprmOptions.sprmPJc, (byte)value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool Keep
        {
            get
            {
                return m_arrSprms.GetBoolean(WordSprmOptions.sprmPFKeep, false);
            }
            set
            {
                m_arrSprms.SetValue(WordSprmOptions.sprmPFKeep, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool KeepFollow
        {
            get
            {
                return m_arrSprms.GetBoolean(WordSprmOptions.sprmPFKeepFollow, false);
            }
            set
            {
                m_arrSprms.SetValue(WordSprmOptions.sprmPFKeepFollow, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool PageBreakBefore
        {
            get
            {
                return m_arrSprms.GetBoolean(WordSprmOptions.sprmPFKeepFollow, false);
            }
            set
            {
                m_arrSprms.SetValue(WordSprmOptions.sprmPFKeepFollow, value);
            }
        }
        #endregion
    }
}