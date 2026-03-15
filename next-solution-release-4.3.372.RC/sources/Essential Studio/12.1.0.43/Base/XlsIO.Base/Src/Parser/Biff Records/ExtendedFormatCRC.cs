#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// This record specifies the number of XF records contained in this file and that contains a checksum of the data in those records. 
    /// This record MUST exist if and only if there are XFExt records in the file.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    [Biff(TBIFFRecord.ExtendedFormatCRC)]
    [CLSCompliant(false)]
    public class ExtendedFormatCRC : BiffRecordRaw
    {
        #region Start
        /// <summary>
        /// Header of this record
        /// </summary>
        private FutureHeader m_header;
        /// <summary>
        /// Specifies the number of XF records
        /// </summary>
        private ushort m_usXFCount;
        /// <summary>
        /// Specifies the Checksum of the record
        /// This checksum is used to detect whether the XF records in the file were modified by an 
        /// application that does not support the formatting feature extensions in XFExt records.
        /// </summary>
        private uint m_uiCRC;
        #endregion

        #region Member
        /// <summary>
        /// current parent workbook
        /// </summary>
        private WorkbookImpl m_book;
        #endregion

        #region Initialization
        /// <summary>
        /// Default constructor
        /// </summary>
        public ExtendedFormatCRC()
            : base()
        {
            m_header = new FutureHeader();
            m_header.Type = (ushort)TBIFFRecord.ExtendedFormatCRC;
            m_usXFCount = 16;
            //m_uiCRC = 1886378647;
        }  
        #endregion

        #region Properties
        /// <summary>
        /// Specifies the number of XF records
        /// </summary>
        public ushort XFCount
        {
            get
            {
                return m_usXFCount;
            }
            set
            {
                m_usXFCount = value;
            }
        }
        /// <summary>
        /// CRC checksum
        /// </summary>
        public uint CRCChecksum
        {
            get
            {
                return m_uiCRC;
            }
            set
            {
                m_uiCRC = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parse structure of record. Converts data buffer to special
        /// values according to record specification.
        /// </summary>
        /// <param name="provider">Object that provides access to the data.</param>
        /// <param name="iOffset">Offset to the record's data.</param>
        /// <param name="iLength">Length of the record's data.</param>
        /// <param name="version">Excel version used for infill.</param>
        /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
        /// If there is any internal error.
        /// </exception>
        public override void ParseStructure(DataProvider arrData, int iOffset, int iLength, ExcelVersion version)
        {
            ushort reserved = 0;
            m_header.Type = arrData.ReadUInt16(iOffset);
            iOffset += 2; ;
            m_header.Attributes = arrData.ReadByte(iOffset);
            iOffset += 2;
            reserved = arrData.ReadByte(iOffset);
            iOffset += 8;

            reserved = arrData.ReadByte(iOffset);
            iOffset += 2;

            m_usXFCount = arrData.ReadUInt16(iOffset);
            iOffset += 2;

            m_uiCRC = arrData.ReadUInt32(iOffset);
            iOffset += 4;
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
            ushort reserved = 0;
            provider.WriteUInt16(iOffset, m_header.Type);

            provider.WriteUInt16(iOffset + 2, m_header.Attributes);

            provider.WriteInt64(iOffset + 4, reserved); //Reserved

            provider.WriteUInt16(iOffset + 12, reserved);

            //Current formats count
            //m_usXFCount = (ushort)m_book.InnerExtFormats.Count;
            provider.WriteUInt16(iOffset + 14, m_usXFCount);
            
            provider.WriteUInt32(iOffset + 16, m_uiCRC);
        }
        /// <summary>
        /// Get the size
        /// </summary>
        public override int GetStoreSize(ExcelVersion version)
        {
            return 20;
        }        
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current Object.</returns>
        public override int GetHashCode()
        {
            int i_hashCode = m_header.Type.GetHashCode()
                ^ m_header.Attributes.GetHashCode()
                ^ m_usXFCount.GetHashCode()
                ^ m_uiCRC.GetHashCode();

            return i_hashCode;
        }
        #endregion

        #region ICloneable Members
        /// <summary>
        /// Clone current Record.
        /// </summary>
        /// <returns>Returns memberwise clone on current object.</returns>
        public override object Clone()
        {
            return new ExtendedFormatCRC();
        }
        #endregion
    }
}
