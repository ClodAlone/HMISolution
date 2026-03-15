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
using System.IO;
using System.Collections;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for SectionPropertyException.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SectionPropertyException : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// List of sprms that encodes the differences between the properties of
        /// a section and Word's default section properties.
        /// </summary>
        //private ArrayList m_arrSprms = new ArrayList();
        private SinglePropertyModifierArray m_arrSprms = new SinglePropertyModifierArray();
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal SinglePropertyModifierArray Properties
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
        /// Number of property modifiers.
        /// </summary>
        internal int Count
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
                return Constants.BytesInWord + m_arrSprms.Length;
            }
        }
        #endregion

        #region Class modifier properties
        /// <summary>
        /// 
        /// </summary>
        internal ushort HeaderHeight
        {
            get
            {
                return m_arrSprms.GetUShort(WordSprmOptions.sprmSDyaHdrTop, 0);
            }
            set
            {
                if (value != 0)
                {
                    m_arrSprms.SetValue(WordSprmOptions.sprmSDyaHdrTop, value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ushort FooterHeight
        {
            get
            {
                return m_arrSprms.GetUShort(WordSprmOptions.sprmSDyaHdrBottom, 0);
            }
            set
            {
                if (value != 0)
                {
                    m_arrSprms.SetValue(WordSprmOptions.sprmSDyaHdrBottom, value);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal bool IsTitlePage
        {
            get
            {
                return m_arrSprms.GetBoolean(WordSprmOptions.sprmSFTitlePage, false);
            }
            set
            {
                if (value != false)
                {
                    m_arrSprms.SetValue(WordSprmOptions.sprmSFTitlePage, value);
                }
            }
        }
        /// <summary>
        /// Gets / sets break code
        /// </summary>
        internal byte BreakCode
        {
            get
            {
                return m_arrSprms.GetByte(WordSprmOptions.sprmSBkc, 2);
            }
            set
            {
                if (value != 2)
                {
                    m_arrSprms.SetValue(WordSprmOptions.sprmSBkc, value);
                }
            }
        }
        /// <summary>
        /// Gets / sets columns count
        /// </summary>
        internal int ColumnsCount
        {
            get
            {
                return m_arrSprms.GetUShort(WordSprmOptions.sprmSCcolumns, 0) + 1;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException();

                m_arrSprms.SetValue(WordSprmOptions.sprmSCcolumns, (ushort)(value - 1));
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SectionPropertyException()
        {
            
        }
        /// <summary>
        /// Creates instance and sets the default section properties
        /// </summary>
        /// <param name="isDefaultSEP">If true, loads the default section properties.</param>
        internal SectionPropertyException(bool isDefaultSEP)
        {
            if (isDefaultSEP)
            {
                m_arrSprms.SetValue(WordSprmOptions.sprmSDyaPgn, (ushort)720);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDxaPgn, (ushort)720);
                m_arrSprms.SetValue(WordSprmOptions.sprmSFEndnote, true);
                m_arrSprms.SetValue(WordSprmOptions.sprmSFEvenlySpaced, true);

                m_arrSprms.SetValue(WordSprmOptions.sprmSXaPage, (ushort)12240);
                m_arrSprms.SetValue(WordSprmOptions.sprmSYaPage, (ushort)15840);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDyaHdrTop, (ushort)720);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDyaHdrBottom, (ushort)720);

                m_arrSprms.SetValue(WordSprmOptions.sprmSBOrientation, true);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDxaColumns, (ushort)720);

                m_arrSprms.SetValue(WordSprmOptions.sprmSDyaTop, (ushort)1440);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDxaLeft, (ushort)1440);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDyaBottom, (ushort)1440);
                m_arrSprms.SetValue(WordSprmOptions.sprmSDxaRight, (ushort)1440);
                m_arrSprms.SetValue(WordSprmOptions.sprmSPgnStart, (ushort)1);
            }
        }
        /// <summary>
        /// Creates instance and gets data from the stream.
        /// </summary>
        /// <param name="stream">Stream with new instance data.</param>
        /// <param name="provider">MemoryConverter for structures convertions.</param>
        internal SectionPropertyException(Stream stream)
        {
            Parse(stream);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Extracts instance from the stream.
        /// </summary>
        /// <param name="stream">Stream with data.</param>
        /// <param name="provider">MemoryConverter for structures convertions.</param>
        private void Parse(Stream stream)
        {
            byte[] arrBuffer = new byte[2];
            int iReadCount = stream.Read(arrBuffer, 0, 2);

            if (iReadCount != 2)
                throw new Exception("Was unable to read required bytes from the stream");

            ushort usCount = BitConverter.ToUInt16(arrBuffer, 0);
            int iEndPos = (int)(usCount + stream.Position);

            while (stream.Position < iEndPos)
            {
                SinglePropertyModifierRecord sprm = new SinglePropertyModifierRecord(stream);
                m_arrSprms.Add(sprm);
            }
        }

        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            ushort usLength = (ushort)m_arrSprms.Length;

            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset + usLength > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iStartOffset = iOffset;
            //BitConverter.GetBytes( usLength ).CopyTo( arrData, iOffset );
            //iOffset += Constants.FileCharPosSize;
            BitConverter.GetBytes(usLength).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            iOffset += m_arrSprms.Save(arrData, iOffset);

            return iOffset - iStartOffset;
        }

        #endregion
    }
}
