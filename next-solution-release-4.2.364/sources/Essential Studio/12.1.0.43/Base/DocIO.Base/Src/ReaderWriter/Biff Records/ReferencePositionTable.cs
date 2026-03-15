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
    /// Summary description for ReferencePositionTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class ReferencePositionTable : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Locations of footnote references within the main text address space.
        /// </summary>
        private int[] m_arrPositions;
        /// <summary>
        /// Reference numbers.
        /// </summary>
        private ushort[] m_arrNumbers;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal ReferencePositionTable()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal ReferencePositionTable(byte[] data)
            : base(data)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal ReferencePositionTable(byte[] arrData, int iOffset)
            : base(arrData, iOffset)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="iCount">Number of bytes for the new record.</param>
        internal ReferencePositionTable(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal ReferencePositionTable(Stream stream, int iCount)
            : base(stream, iCount)
        {
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
                throw new ArgumentOutOfRangeException("iOffset", "Value can not be less than 0 and greater than arrData.Length - 1");

            if (iCount < 0 || iOffset + iCount > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            if (iCount == 0)
            {
                m_arrNumbers = null;
                m_arrPositions = null;
                return;
            }

            int iElements = (iCount - Constants.BytesInInt)
              / (Constants.BytesInInt + Constants.BytesInWord);

            m_arrPositions = new int[iElements + 1];
            m_arrNumbers = new ushort[iElements];

            int iSize = (iElements + 1) * Constants.BytesInInt;
            //      API.CopyMemory( m_arrPositions, ref arrData[ iOffset ], iSize );
            Buffer.BlockCopy(arrData, iOffset, m_arrPositions, 0, iSize);
            iOffset += iSize;

            //      API.CopyMemory( m_arrNumbers, ref arrData[ iOffset ],
            //        iElements * Constants.BytesInWord );
            Buffer.BlockCopy(arrData, iOffset, m_arrNumbers, 0, iElements * Constants.BytesInWord);
        }

        #endregion

        #region Class Properties
        /// <summary>
        /// Gets locations of footnote references within the main text
        /// address space. Read-only.
        /// </summary>
        internal int[] Positions
        {
            get
            {
                return m_arrPositions;
            }
        }

        /// <summary>
        /// Gets reference numbers. Read-only.
        /// </summary>
        internal ushort[] Numbers
        {
            get
            {
                return m_arrNumbers;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                return m_arrNumbers.Length * Constants.BytesInWord
                  + m_arrPositions.Length * Constants.BytesInInt;
            }
        }
        #endregion
    }
}
