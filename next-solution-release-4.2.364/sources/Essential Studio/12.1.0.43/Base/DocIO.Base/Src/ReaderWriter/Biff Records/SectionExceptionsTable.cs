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
    /// Summary description for SectionExceptionsTable.
    /// </summary>
    [CLSCompliant(false)]
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class SectionExceptionsTable : BaseWordRecord
    {
        #region Class members
        /// <summary>
        /// The boundaries (character positions) of sections in the Word document.
        /// </summary>
        private int[] m_arrPositions;
        /// <summary>
        /// 1-to-1 correspondence to the array of CPs.
        /// Each SED stores the beginning FC of the SEPX that records the properties
        /// for a section. If the FC stored in a SED is -1, the section properties of
        /// the section are exactly equal to the standard section properties.
        /// </summary>
        private SectionDescriptor[] m_arrDescriptors;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal SectionExceptionsTable()
        {
        }
        /// <summary>
        /// Creates new record from array of bytes, using specified memory provider.
        /// </summary>
        /// <param name="arrData">Data to parse.</param>
        /// <param name="converter">MemoryConverter to convert array of bytes into structure.</param>
        internal SectionExceptionsTable(byte[] arrData)
            : base(arrData)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        internal SectionExceptionsTable(Stream stream, int iCount)
            : base(stream, iCount)
        { }
        #endregion

        #region Class Overrides
        /// <summary>
        /// Parses the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="iOffset">The offset.</param>
        /// <param name="iCount">The count.</param>
        internal override void Parse(byte[] data, int iOffset, int iCount)
        {
            if (data == null)
                throw new ArgumentNullException("arrData");

            if (iOffset != 0)
                throw new ArgumentOutOfRangeException("iOffset");

            if (iCount < 0)
                throw new ArgumentOutOfRangeException("iCount");

            if (iOffset + iCount > data.Length)
                throw new ArgumentOutOfRangeException("iOffset + iCount");

            int iLength = data.Length;
            int iElementSize = Constants.BytesInInt + SectionDescriptor.DEF_RECORD_SIZE;
            int iArraySize = iLength / iElementSize;

            m_arrPositions = new int[iArraySize + 1];
            m_arrDescriptors = new SectionDescriptor[iArraySize];

            iOffset = (iArraySize + 1) * Constants.BytesInInt;
            Buffer.BlockCopy(data, 0, m_arrPositions, 0, iOffset);
            //API.CopyMemory( m_arrPositions, data, iOffset );

            for (int i = 0; i < iArraySize; i++, iOffset += SectionDescriptor.DEF_RECORD_SIZE)
            {
                m_arrDescriptors[i] = new SectionDescriptor(data, iOffset);
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

            int iLength = Length;

            if (iOffset < 0 || iOffset + iLength > arrData.Length)
                throw new ArgumentOutOfRangeException("iOffset");

            int iStartOffset = iOffset;
            int iPosSize = m_arrPositions.Length * Constants.FileCharPosSize;

            //      API.CopyMemory( ref arrData[ iOffset ], m_arrPositions, iPosSize );
            Buffer.BlockCopy(m_arrPositions, 0, arrData, iOffset, iPosSize);
            iOffset += iPosSize;

            for (int i = 0, len = m_arrDescriptors.Length; i < len; i++)
            {
                m_arrDescriptors[i].Save(arrData, iOffset);
                iOffset += m_arrDescriptors[i].Length;
            }

            return iOffset - iStartOffset;
        }

        #endregion

        #region Class Properties
        /// <summary>
        /// The boundaries (character positions) of sections in the Word document.
        /// </summary>
        internal int[] Positions
        {
            get
            {
                return m_arrPositions;
            }
        }
        /// <summary>
        /// 1-to-1 correspondence to the array of CPs.
        /// Each SED stores the beginning FC of the SEPX that records the properties
        /// for a section. If the FC stored in a SED is -1, the section properties of
        /// the section are exactly equal to the standard section properties.
        /// </summary>
        internal SectionDescriptor[] Descriptors
        {
            get
            {
                return m_arrDescriptors;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal override int Length
        {
            get
            {
                int iResult = m_arrPositions.Length * Constants.BytesInInt;

                for (int i = 0, len = m_arrDescriptors.Length; i < len; i++)
                {
                    iResult += m_arrDescriptors[i].Length;
                }

                return iResult;
            }
        }

        /// <summary>
        /// Number of elements in the collection.
        /// </summary>
        internal int EntriesCount
        {
            get
            {
                return (m_arrDescriptors == null) ? 0 : m_arrDescriptors.Length;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("EntriesCount");

                if (value != EntriesCount)
                {
                    m_arrDescriptors = new SectionDescriptor[value];
                    m_arrPositions = new int[value + 1];

                    for (int i = 0; i < value; i++)
                    {
                        m_arrDescriptors[i] = new SectionDescriptor();
                    }
                }
            }
        }
        #endregion
    }
}
