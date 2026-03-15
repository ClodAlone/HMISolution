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

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for CharPosTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class CharPosTableRecord : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private int[] m_arrPositions;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal CharPosTableRecord()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        internal CharPosTableRecord(byte[] data)
            : base(data)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        internal CharPosTableRecord(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal CharPosTableRecord(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal CharPosTableRecord(Stream stream, int iCount)
            : base(stream, iCount)
        {
        }
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets text positions. Read-only.
        /// </summary>
        internal int[] Positions
        {
            get
            {
                return m_arrPositions;
            }
            set
            {
                m_arrPositions = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_arrPositions.Length * Constants.BytesInInt;
            }
        }

        #endregion

        #region Class Public methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        internal string GetTextChunk(string text, int position)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (text.Length == 0)
                throw new ArgumentException("text - string can not be empty");

            int posMax = Positions.Length - 1;

            if (position < 0 || position > posMax)
                throw new ArgumentOutOfRangeException("position",
                  "Value can not be less 0 and greater " + posMax.ToString());

            int start = Positions[position];
            int length = 0;

            if (position + 1 < Positions.Length)
            {
                length = Positions[position + 1] - start;
            }
            else
            {
                length = text.Length - start;
            }

            return text.Substring(start, length);
        }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset in the data array to the records data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iCount)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length - 1)
                throw new ArgumentOutOfRangeException("iOffset", 
                  "Value can not be less 0 and greater arrData.Length - 1");

            if (iCount < 0 || iOffset + iCount > arrData.Length)
                throw new ArgumentOutOfRangeException("iCount");

            int iElements = iCount / Constants.BytesInInt;

            m_arrPositions = new int[iElements];

            Buffer.BlockCopy(arrData, 0, m_arrPositions, 0, iElements * Constants.BytesInInt);
            //API.CopyMemory( m_arrPositions, arrData, iElements * Constants.BytesInInt );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iOffset"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        internal override int Save(byte[] arrData, int iOffset)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length - 1)
                throw new ArgumentOutOfRangeException("iOffset", 
                  "Value can not be less 0 and greater arrData.Length - 1");

            Buffer.BlockCopy(m_arrPositions, 0, arrData, 0, Length);
            //API.CopyMemory( arrData, m_arrPositions, Length );

            return arrData.Length;
        }
        #endregion
    }
}