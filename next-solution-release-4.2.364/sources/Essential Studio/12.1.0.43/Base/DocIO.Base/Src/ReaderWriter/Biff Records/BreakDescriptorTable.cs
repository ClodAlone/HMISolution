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

using TableEntry = Syncfusion.DocIO.ReaderWriter.Biff_Records.BreakDescriptorRecord;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BinaryTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BreakDescriptorTable : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Array of file positions (n+1).
        /// </summary>
        private uint[] m_arrFC;
        /// <summary>
        /// Array of talbe entries (n).
        /// </summary>
        private TableEntry[] m_arrEntry;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BreakDescriptorTable()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="data">Data to parse.</param>
        internal BreakDescriptorTable(byte[] data)
            : base(data)
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="iOffset">Offset to the class data.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorTable(byte[] arrData, int iOffset)
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
        internal BreakDescriptorTable(byte[] arrData, int iOffset, int iCount)
            : base(arrData, iOffset, iCount)
        {
        }
        /// <summary>
        /// Creates new record from stream.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal BreakDescriptorTable(Stream stream, int iCount)
            : base(stream, iCount)
        {
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

            if (iOffset < 0 || iOffset > arrData.Length - 1)
                throw new ArgumentOutOfRangeException("iOffset", "Value can not be less than 0 and greater than arrData.Length - 1");

            if (iCount < 0 || iCount + iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iCount");

            m_arrEntry = null;
            m_arrFC = null;

            if (iCount == 0) return;

            int iElementsCount = (iCount - Constants.FileCharPosSize)
              / (Constants.FileCharPosSize + TableEntry.DEF_RECORD_SIZE);

            m_arrFC = new uint[iElementsCount + 1];
            m_arrEntry = new TableEntry[iElementsCount];


            int iFCPartSize = (iElementsCount + 1) * Constants.FileCharPosSize;
            Buffer.BlockCopy(arrData, 0, m_arrFC, 0, iFCPartSize);
            //API.CopyMemory( m_arrFC, arrData, iFCPartSize );
            iOffset = iOffset + iFCPartSize;

            for (int i = 0; i < iElementsCount; i++, iOffset += TableEntry.DEF_RECORD_SIZE)
            {
                m_arrEntry[i] = new TableEntry(arrData, iOffset);
            }
        }
        /// <summary>
        /// Parses record.
        /// </summary>
        /// <param name="stream">Stream with record's data.</param>
        /// <param name="iCount">Number of bytes to parse.</param>
        internal override void Parse(Stream stream, int iCount)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (iCount < 0)
                throw new ArgumentOutOfRangeException("iCount");

            m_arrEntry = null;
            m_arrFC = null;

            if (iCount == 0) return;

            int iElementsCount = (iCount - Constants.FileCharPosSize)
              / (Constants.FileCharPosSize + TableEntry.DEF_RECORD_SIZE);

            m_arrFC = new uint[iElementsCount + 1];
            m_arrEntry = new TableEntry[iElementsCount];

            int iFCPartSize = (iElementsCount + 1) * Constants.FileCharPosSize;
            byte[] arrBuffer = new byte[iFCPartSize];

            stream.Read(arrBuffer, 0, iFCPartSize);
            Buffer.BlockCopy(arrBuffer, 0, m_arrFC, 0, iFCPartSize);
            //API.CopyMemory( m_arrFC, arrBuffer, iFCPartSize );

            if (iFCPartSize < TableEntry.DEF_RECORD_SIZE)
            {
                arrBuffer = new byte[TableEntry.DEF_RECORD_SIZE];
            }

            for (int i = 0; i < iElementsCount; i++)
            {
                stream.Read(arrBuffer, 0, TableEntry.DEF_RECORD_SIZE);
                m_arrEntry[i] = new TableEntry(arrBuffer, 0, TableEntry.DEF_RECORD_SIZE);
            }
        }

        #endregion

        #region Class Properties
        /// <summary>
        /// Returns array of character positions. Read-only.
        /// </summary>
        internal uint[] FileCharacterPos
        {
            get
            {
                return m_arrFC;
            }
        }
        /// <summary>
        /// Returns array of binary table entries. Read-only.
        /// </summary>
        internal TableEntry[] Entries
        {
            get
            {
                return m_arrEntry;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                int iResult = 0;

                if (m_arrFC != null)
                {
                    iResult += m_arrFC.Length + Constants.FileCharPosSize;
                }

                if (m_arrEntry != null)
                {
                    iResult += TableEntry.DEF_RECORD_SIZE * m_arrEntry.Length;
                }

                return iResult;
            }
        }

        #endregion
    }
}
