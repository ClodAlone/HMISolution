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
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using System.IO;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ParagraphExceptionInDiskPage : ParagraphPropertyException
    {
        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return (IsPad ? 2 : 1) + Constants.BytesInWord + m_arrSprms.Length;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected bool IsPad
        {
            get
            {
                return (m_arrSprms.Length % 2 == 0);
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ParagraphExceptionInDiskPage()
        { }
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ParagraphExceptionInDiskPage(ParagraphPropertyException papx)
        {
            m_usStyleId = papx.StyleIndex;
            m_arrSprms = papx.PropertyModifiers;
        }
        /// <summary>
        /// Creates new instance of Paragraph Property Exception in Formatted Disk Page.
        /// </summary>
        /// <param name="arrData">Array that contains class data.</param>
        /// <param name="iOffset">Data offset in the data array.</param>
        /// <param name="converter"></param>
        internal ParagraphExceptionInDiskPage(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        { }
        #endregion

        #region Class methods
        /// <summary>
        /// Extracts structure from the array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes with structure's data.</param>
        /// <param name="iOffset">Offset to the structure's data start.</param>
        /// <param name="converter"></param>
        /// <returns>Offset after structure's data end.</returns>
        new internal int Parse(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            int iLength = arrData.Length;

            if (iOffset < 0 || iOffset >= iLength)
                throw new ArgumentOutOfRangeException("iOffset");

            byte btWordsCount = arrData[iOffset];
            iOffset++;

            if (btWordsCount == 0)
            {
                if (iOffset >= iLength)
                    throw new ArgumentOutOfRangeException("iOffset");

                btWordsCount = arrData[iOffset++];
            }

            int iDataLength = btWordsCount * Constants.BytesInWord;

            if (iOffset + iDataLength > iLength)
                throw new ArgumentOutOfRangeException("Data array is too short");

            //base.Parse( arrData, iOffset, iDataLength, converter );
            m_usStyleId = BitConverter.ToUInt16(arrData, iOffset);
            iOffset += Constants.BytesInWord;
            iDataLength -= Constants.BytesInWord;

            m_arrSprms.Parse(arrData, iOffset, iDataLength);
            return iOffset;
        }
        /// <summary>
        /// Saves record into array of bytes.
        /// </summary>
        /// <param name="arrData">Array of bytes to save record into.</param>
        /// <param name="iOffset">Offset in the array.</param>
        /// <returns>Number of bytes in the written data.</returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            int iStartOffset = iOffset;
            int iLength = Length - (IsPad ? 2 : 1);

            /*
            if( iOffset % 2 != 0 )
            {
              iStartOffset++;
              arrData[ iOffset++ ] = 0;
            }

            if( iLength % 2 != 0 ) iLength++;
            */
            if (IsPad)
            {
                arrData[iOffset++] = 0;
            }

            byte btWordsCount = (byte)(iLength / 2);

            arrData[iOffset++] = btWordsCount;

            BitConverter.GetBytes(m_usStyleId).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInWord;

            iOffset += m_arrSprms.Save(arrData, iOffset);

            return iOffset - iStartOffset;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal void Save(BinaryWriter writer, Stream stream)
        {
            int iLength = Length - (IsPad ? 2 : 1);

            if (IsPad)
            {
                writer.Write((byte)0);
            }

            byte btWordsCount = (byte)(iLength / 2);
            writer.Write(btWordsCount);

            writer.Write(m_usStyleId);

            if (m_arrSprms != null)
            {
                m_arrSprms.Save(writer, stream);
            }
        }
        #endregion
    }
}