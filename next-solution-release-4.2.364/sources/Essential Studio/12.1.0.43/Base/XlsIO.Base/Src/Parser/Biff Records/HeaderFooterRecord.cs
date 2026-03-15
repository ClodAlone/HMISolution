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
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    [Biff(TBIFFRecord.HeaderFooter)]
    public class HeaderAndFooterRecord: BiffRecordRaw
    {
        #region Class members
        /// <summary>
        /// Type of data in categories.
        /// </summary>
        [BiffRecordPos(28, 0, TFieldType.Bit)]
        private bool m_bfHFDiffOddEven;
        /// <summary>
        /// Type of data in categories.
        /// </summary>
        [BiffRecordPos(28, 1, TFieldType.Bit)]
        private bool m_bfHFDiffFirst;
        /// <summary>
        /// Type of data in categories.
        /// </summary>
        [BiffRecordPos(28, 2, TFieldType.Bit)]
        private bool m_bfHFScaleWithDoc;
        /// <summary>
        /// Type of data in categories.
        /// </summary>
        [BiffRecordPos(28, 3, TFieldType.Bit)]
        private bool m_bfHFAlignMargins;
        /// <summary>
        /// Record databytes.
        /// </summary>
        private byte[] m_arrBytes = new byte[38];
        /// <summary>
        /// HeaderFooter Record Code
        /// </summary>
        private int recordCode = 2204;
        /// <summary>
        /// Excel 2003 record length
        /// </summary>
        private const int Record2003Length = 22;
        /// <summary>
        /// Excel 2010 record length
        /// </summary>
        private const int Record2010Length = 38;
        #endregion

        #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  HeaderAndFooterRecord()
      : base()
    {      
        m_bfHFScaleWithDoc = true;
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  HeaderAndFooterRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public HeaderAndFooterRecord(int iReserve)
      : base( iReserve )
    {
    }
    #endregion

        #region Class properties
    public bool AlignHFWithPageMargins
    {
        get
        {
            return m_bfHFAlignMargins;
        }
        set
        {
            m_bfHFAlignMargins = value;
        }
    }
    public bool DifferentOddAndEvenPagesHF
    {
        get
        {
            return m_bfHFDiffOddEven;
        }
        set
        {
            m_bfHFDiffOddEven = value;
        }
    }
    public bool HFScaleWithDoc
    {
        get
        {
            return m_bfHFScaleWithDoc;
        }
        set
        {
            m_bfHFScaleWithDoc = value;
        }
    }
    public bool DifferentFirstPageHF
    {
        get
        {
            return m_bfHFDiffFirst;
        }
        set
        {
            m_bfHFDiffFirst = value;
        }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record's
    /// internal data array.
    /// </summary>
    public override int MinimumRecordSize
    {
        get
        {
            return 0;
        }
    }
       #endregion

        #region Record Serialization
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="iLength">Length of the record's data.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void ParseStructure(DataProvider provider, int iOffset, int iLength, ExcelVersion version)
    {
        if (m_iLength > 0)
        {
            if (m_iLength > Record2003Length)
            {
                iOffset += 28;
            }
            else
            {
                iOffset += 12;
            }
            m_bfHFDiffOddEven = provider.ReadBit(iOffset, 0);
            m_bfHFDiffFirst = provider.ReadBit(iOffset, 1);
            m_bfHFScaleWithDoc = provider.ReadBit(iOffset, 2);
            m_bfHFAlignMargins = provider.ReadBit(iOffset, 3);
        }
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
        provider.WriteBytes(4, m_arrBytes);
        provider.WriteInt32(4, recordCode);
        //if (m_iLength > Record2003Length)
        //{
        //    iOffset = 28;
        //}
        //else
        //{
        //    iOffset = 12;
        //}
        iOffset = 28;
        provider.WriteBit(iOffset, m_bfHFDiffOddEven, 0);
        provider.WriteBit(iOffset, m_bfHFDiffFirst, 1);
        provider.WriteBit(iOffset, m_bfHFScaleWithDoc, 2);
        provider.WriteBit(iOffset, m_bfHFAlignMargins, 3);
    }
    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize(ExcelVersion version)
    {
        return Record2010Length+4;
    }
    #endregion
    }
}
