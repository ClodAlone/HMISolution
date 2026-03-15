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
    /// Summary description for BinaryTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class BinaryTable : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// Array of file positions (n+1).
        /// </summary>
        private uint[] m_arrFC;
        /// <summary>
        /// Array of talbe entries (n).
        /// </summary>
        private BinTableEntry[] m_arrEntry;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal BinaryTable()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="converter"></param>
        internal BinaryTable(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="iCount"></param>
        /// <param name="converter"></param>
        internal BinaryTable(Stream stream, int iCount)
            : base(stream, iCount)
        { }
        #endregion

        #region Class overrides
        /// <summary>
        /// Parses the specified data.
        /// </summary>
        /// <param name="arrData">The data.</param>
        /// <param name="iOffset">The offset.</param>
        /// <param name="iLength">Length.</param>
        internal override void Parse(byte[] arrData, int iOffset, int iLength)
        {
            if (arrData == null)
                throw new ArgumentNullException("arrData");
            if (iOffset != 0)
                throw new ArgumentOutOfRangeException("iOffset", "Value cannot be less  and greater ");
            if (iLength != arrData.Length)
                throw new ArgumentOutOfRangeException("iLength", "Value cannot be less  and greater ");

            int iCount = (iLength - Constants.FileCharPosSize)
              / (Constants.FileCharPosSize + BinTableEntry.RECORD_SIZE);

            m_arrFC = new uint[iCount + 1];
            m_arrEntry = new BinTableEntry[iCount];


            int iFCPartSize = (iCount + 1) * Constants.FileCharPosSize;
            Buffer.BlockCopy(arrData, 0, m_arrFC, 0, iFCPartSize);
            //API.CopyMemory( m_arrFC, arrData, iFCPartSize );
            iOffset = iFCPartSize;

            for (int i = 0; i < iCount; i++)
            {
                m_arrEntry[i] = new BinTableEntry();
                iOffset = m_arrEntry[i].Parse(arrData, iOffset);
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
            if (arrData == null)
                throw new ArgumentNullException("arrData");

            if (iOffset < 0 || iOffset > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iFCLength = Constants.FileCharPosSize * m_arrFC.Length;
            int iEntryLength = m_arrEntry.Length * BinTableEntry.RECORD_SIZE;
            int iFullLength = iFCLength + iEntryLength;

            if (iOffset + iFullLength > arrData.Length)
                throw new ArgumentOutOfRangeException("arrData.Length");

            //      API.CopyMemory( ref arrData[ iOffset ], m_arrFC, iFCLength );
            Buffer.BlockCopy(m_arrFC, 0, arrData, iOffset, iFCLength);
            iOffset += iFCLength;

            for (int i = 0, len = m_arrEntry.Length; i < len; i++)
            {
                m_arrEntry[i].Save(arrData, iOffset);
                iOffset += BinTableEntry.RECORD_SIZE;
            }

            return iFullLength;
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
        internal BinTableEntry[] Entries
        {
            get
            {
                return m_arrEntry;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal int EntriesCount
        {
            get
            {
                return m_arrEntry.Length;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Length");

                m_arrFC = new uint[value + 1];
                m_arrEntry = new BinTableEntry[value];
            }
        }
        /// <summary>
        /// Returns number of bytes needed to store record in a stream or in an array.
        /// Read-only.
        /// </summary>
        internal override int Length
        {
            get
            {
                int iFCLength = Constants.FileCharPosSize * m_arrFC.Length;
                int iEntryLength = m_arrEntry.Length * BinTableEntry.RECORD_SIZE;

                return iFCLength + iEntryLength;
            }
        }

        #endregion
    }
}