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

namespace Syncfusion.XlsIO.Parser.Biff_Records.Charts
{
    /// <summary>
    /// This record specifies the layout information for attached label
    /// </summary>
    [Biff(TBIFFRecord.ChartAttachedLabelLayout)]
    [Syncfusion.Documentation.DocumentationExclude()]
    [CLSCompliant(false)]    
    public class ChartAttachedLabelLayoutRecord
        : BiffRecordRaw
    {
        #region Class constants
        /// <summary>
        /// Correct record size.
        /// </summary>
        private const int DEF_RECORD_SIZE = 60;
        /// <summary>
        /// Specifies the future record type header
        /// </summary>
        private byte[] DEF_HEADER = new byte[] { 
                                                    0x09D, 0x008, 0x000, 0x000, 0x000, 0x000, 0x000, 0x000,
                                                    0x0000, 0x000, 0x000, 0x000
                                               };
        private byte[] m_frtHeader;
        private int m_dwCheckSum;
        private int m_info;
        private bool m_unUsed;
        private int m_autoLayoutType;
        private byte[] m_reserved1;
        private LayoutModes m_wXMode;
        private LayoutModes m_wYMode;
        private LayoutModes m_wWidthMode;
        private LayoutModes m_wHeightMode;
        private double m_x;
        private double m_y;
        private double m_dx;
        private double m_dy;
        private byte[] m_reserved2 = new byte[2];
        #endregion

        #region Class properties
        private byte[] FrtHeader
        {
            get
            {
                return m_frtHeader;
            }
        }
        private int dwCheckSum
        {
            get
            {
                return m_dwCheckSum;
            }
            set
            {
                m_dwCheckSum = value;
            }
        }
        public LayoutModes WXMode
        {
            get
            {
                return m_wXMode;
            }
            set
            {
                m_wXMode = value;
            }
        }
        public LayoutModes WYMode
        {
            get
            {
                return m_wYMode;
            }
            set
            {
                m_wYMode = value;
            }
        }
        public LayoutModes WWidthMode
        {
            get
            {
                return m_wWidthMode;
            }
            set
            {
                m_wWidthMode = value;
            }
        }
        public LayoutModes WHeightMode
        {
            get
            {
                return m_wHeightMode;
            }
            set
            {
                m_wHeightMode = value;
            }
        }
        public double X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }
        public double Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }
        public double Dx
        {
            get
            {
                return m_dx;
            }
            set
            {
                m_dx = value;
            }
        }
        public double Dy
        {
            get
            {
                return m_dy;
            }
            set
            {
                m_dy = value;
            }
        }
        #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartAttachedLabelLayoutRecord()
      : base()
    {
        m_frtHeader = DEF_HEADER;
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  ChartAttachedLabelLayoutRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
        m_frtHeader = DEF_HEADER;
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public ChartAttachedLabelLayoutRecord(int iReserve)
      : base( iReserve )
    {
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
        provider.ReadArray(iOffset, m_frtHeader);
        m_dwCheckSum = provider.ReadInt32(iOffset + 12);

        m_info = provider.ReadInt32(iOffset + 16);
        //byte[] bytes = new byte[16];
        //provider.ReadArray(iOffset + 14, bytes);

        //m_unUsed = GetBit(bytes, 0, 1);
        //m_autoLayoutType = provider.ReadArray(70, bytes, 4);
        //m_reserved1 = GetBytes(bytes, 5, 11);

        //m_unUsed = Convert.ToBoolean(m_info & 32768);

        m_wXMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), provider.ReadInt16(iOffset + 18).ToString(), true);
        m_wYMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), provider.ReadInt16(iOffset + 20).ToString(), true);
        m_wWidthMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), provider.ReadInt16(iOffset + 22).ToString(), true);
        m_wHeightMode = (LayoutModes)Enum.Parse(typeof(LayoutModes), provider.ReadInt16(iOffset + 24).ToString(), true);

        m_x = provider.ReadDouble(iOffset + 26);
        m_y = provider.ReadDouble(iOffset + 34);
        m_dx = provider.ReadDouble(iOffset + 42);
        m_dy = provider.ReadDouble(iOffset + 50);

        provider.ReadArray(iOffset + 58, m_reserved2);
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
        m_iLength = GetStoreSize(version);
        provider.WriteBytes(iOffset + 0, m_frtHeader);
        provider.WriteInt32(iOffset + 12, m_dwCheckSum);
        provider.WriteInt32(iOffset + 16, m_info);
        provider.WriteInt32(iOffset + 18, (int)Enum.Parse(typeof(LayoutModes), m_wXMode.ToString(), true));
        provider.WriteInt32(iOffset + 20, (int)Enum.Parse(typeof(LayoutModes), m_wYMode.ToString(), true));
        provider.WriteInt32(iOffset + 22, (int)Enum.Parse(typeof(LayoutModes), m_wWidthMode.ToString(), true));
        provider.WriteInt32(iOffset + 24, (int)Enum.Parse(typeof(LayoutModes), m_wHeightMode.ToString(), true));
        provider.WriteDouble(iOffset + 26, m_x);
        provider.WriteDouble(iOffset + 34, m_y);
        provider.WriteDouble(iOffset + 42, m_dx);
        provider.WriteDouble(iOffset + 50, m_dy);
        provider.WriteBytes(iOffset + 58, m_reserved2);
    }
    /// <summary>
    /// 
    /// </summary>
    public override int GetStoreSize(ExcelVersion version)
    {
        return DEF_RECORD_SIZE;
    }
    #endregion

    #region Class Methods
    /// <summary>
    /// If the checksum is incorrect, the layout information specified in this record must be ignored
    /// </summary>
    /// <returns></returns>
    private int CheckSum()
    {

        return 0;
    }
    #endregion
    }
}
