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
using System.Runtime.InteropServices;

using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;

using TableEntry = Syncfusion.DocIO.ReaderWriter.Biff_Records.PieceDescriptorRecord;
#endregion

namespace Syncfusion.DocIO.ReaderWriter.Biff_Records
{
    /// <summary>
    /// Summary description for BinaryTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class PieceTable : BaseWordRecord
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
        internal PieceTable()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="converter"></param>
        internal PieceTable(byte[] arrData)
            : base(arrData)
        {
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="converter"></param>
        internal override void Parse(byte[] arrData)
        {
            int iLength = arrData.Length;
            int iCount = (iLength - Constants.FileCharPosSize)
              / (Constants.FileCharPosSize + TableEntry.RECORD_SIZE);

            m_arrFC = new uint[iCount + 1];
            m_arrEntry = new TableEntry[iCount];


            int iFCPartSize = (iCount + 1) * Constants.FileCharPosSize;
            Buffer.BlockCopy(arrData, 0, m_arrFC, 0, iFCPartSize);
            //API.CopyMemory( m_arrFC, arrData, iFCPartSize );
            int iOffset = iFCPartSize;

            for (int i = 0; i < iCount; i++, iOffset += TableEntry.RECORD_SIZE)
            {
                m_arrEntry[i] = new TableEntry();
                m_arrEntry[i].Parse(arrData, iOffset);
            }
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

            int iLength = Length;

            if (iOffset < 0 || iOffset + iLength > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iStartOffset = iOffset;

            arrData[iOffset++] = (byte)WordComplexBlockType.PieceTable;

            BitConverter.GetBytes(iLength - 1 - Constants.BytesInInt).CopyTo(arrData, iOffset);
            iOffset += Constants.BytesInInt;

            if (EntriesCount > 0)
            {

                int iFCSize = m_arrFC.Length * Constants.FileCharPosSize;

                //        API.CopyMemory( ref arrData[ iOffset ], m_arrFC, iFCSize );
                Buffer.BlockCopy(m_arrFC, 0, arrData, iOffset, iFCSize);
                iOffset += iFCSize;

                for (int i = 0, len = EntriesCount; i < len; i++)
                {
                    iOffset += m_arrEntry[i].Save(arrData, iOffset);
                }
            }

            return iOffset - iStartOffset;
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
                int iResult = m_arrFC.Length * Constants.FileCharPosSize + 1 + Constants.BytesInInt;

                for (int i = 0, len = m_arrEntry.Length; i < len; i++)
                {
                    iResult += m_arrEntry[i].Length;
                }

                return iResult;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal int EntriesCount
        {
            get
            {
                if (m_arrEntry == null) return 0;

                return m_arrEntry.Length;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("EntriesCount");

                if (value != EntriesCount)
                {
                    m_arrEntry = new TableEntry[value];
                    m_arrFC = new uint[value + 1];

                    for (int i = 0; i < value; i++)
                    {
                        m_arrEntry[i] = new TableEntry();
                    }
                }
            }
        }
        #endregion
    }
}
