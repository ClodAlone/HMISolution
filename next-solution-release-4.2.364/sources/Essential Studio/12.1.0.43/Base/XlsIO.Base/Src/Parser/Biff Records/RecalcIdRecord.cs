#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// This record stores RecalcId identifiers.    
    /// </summary>
    [Biff(TBIFFRecord.RecalcId)]
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]
    public class RecalcIdRecord : BiffRecordRaw
    {
        #region Class members
        /// <summary>
        /// Represents the Record Type
        /// </summary>
        [BiffRecordPos(0, 4)]
        private uint m_record = 449;
        /// <summary>
        /// Represents Calc Identifier.
        /// </summary>
        [BiffRecordPos(4, 8)]
        private uint m_dwBuild;// = 145621;
        #endregion

        #region Class properties

        /// <summary>
        /// Represents the user interface language of the Excel version
        /// that saved this file.
        /// </summary>
        public uint RecordId
        {
            get
            {
                return m_record;
            }
            set
            {
                m_record = value;
            }
        }

        /// <summary>
        /// Represents the system regional settings
        /// at the time the file was saved.
        /// </summary>
        public uint CalcIdentifier
        {
            get
            {
                return m_dwBuild;
            }
            set
            {
                m_dwBuild = value;
            }
        }

        /// <summary>
        /// Read-only. Returns minimum possible size of record's
        /// internal data array.
        /// </summary>
        override public int MinimumRecordSize
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        /// Read-only. Returns maximum possible size of record's
        /// internal data array.
        /// </summary>
        override public int MaximumRecordSize
        {
            get
            {
                return 8;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Default constructor
        /// </summary>
        public RecalcIdRecord()
            : base()
        {
        }

        /// <summary>
        /// Read/Initialize constructor
        /// </summary>
        /// <param name="stream">Stream from which record data should be read.</param>
        /// <param name="itemSize">Size of read item.</param>
        /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
        /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
        public RecalcIdRecord(Stream stream, out int itemSize)
            : base(stream, out itemSize)
        {
        }

        /// <summary>
        /// Reserved for record's internal data array.
        /// </summary>
        /// <param name="iReserve">Amount of bytes for data array.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
        public RecalcIdRecord(int iReserve)
            : base(iReserve)
        {
        }

        #endregion

        #region Record Serialization
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
        {
            m_record = provider.ReadUInt16(iOffset + 0);
            m_dwBuild = provider.ReadUInt32(iOffset + 4);
        }
        /// <summary>
        /// In this method, class must pack all of its properties into
        /// an internal data array, m_data. This method is called by
        /// FillStream, when the record must be serialized into a stream.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset in the buffer.</param>
        /// <param name="version">Excel version used for infill.</param>
        public override void InfillInternalData(DataProvider provider, int iOffset, ExcelVersion version)
        {
            provider.WriteUInt32(iOffset + 0, m_record);            
            provider.WriteUInt32(iOffset + 4, m_dwBuild);
        }
        #endregion
    }
}
