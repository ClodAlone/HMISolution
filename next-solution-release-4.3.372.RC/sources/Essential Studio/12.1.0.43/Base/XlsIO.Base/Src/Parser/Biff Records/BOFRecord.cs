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

using System;
using System.IO;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Summary description for BOFRecord.
  /// It is used for the beginning of a set of
  /// records that have a particular purpose or subject.
  /// Used in sheets and workbooks.
  /// </summary>
  [ Biff( TBIFFRecord.BOF ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BOFRecord
#if DEBUG
    : BiffRecordRaw
#else
    : BiffRecordWithStreamPos
#endif
  {
    #region Class constants
    /// <summary>
    /// Represents the record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 16;
    /// <summary>
    /// This enum that describes all possible data after this BOF record.
    /// </summary>
    public enum TType : int
    {
      /// <summary>
      /// Represents the workbook data.
      /// </summary>
      TYPE_WORKBOOK       = 0x05,
      /// <summary>
      /// Represents the vb_module data.
      /// </summary>
      TYPE_VB_MODULE      = 0x06,
      /// <summary>
      /// Represents the worksheet data.
      /// </summary>
      TYPE_WORKSHEET      = 0x10,
      /// <summary>
      /// Represents the chart data.
      /// </summary>
      TYPE_CHART          = 0x20,
      /// <summary>
      /// Represents the excel_4_macro data.
      /// </summary>
      TYPE_EXCEL_4_MACRO  = 0x40,
      /// <summary>
      /// Represents the workspace file data.
      /// </summary>
      TYPE_WORKSPACE_FILE = 0x100,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Version:
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usVersion = 1536;

    /// <summary>
    /// Type of the following data:
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usType = (ushort) TType.TYPE_WORKBOOK;

    /// <summary>
    /// Build identifier:
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usBuild = 6214;

    /// <summary>
    /// Build year:
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usYear = 1997;

    /// <summary>
    /// File history flag:
    /// </summary>
    [ BiffRecordPos( 8, 4, true ) ]
    private int m_iHistory = 98497;

    /// <summary>
    /// Lowest Excel version that can read all records in this file:
    /// </summary>
    [ BiffRecordPos( 12, 4, true ) ]
    private int m_iRVersion = 1542;

    /// <summary>
    /// 
    /// </summary>
    private bool m_bIsNested = false;
    #endregion

    #region Class properties
    /// <summary>
    /// Version:
    /// </summary>
    public ushort Version
    {
      get
      {
        return m_usVersion;
      }
      set
      {
        m_usVersion = value;
      }
    }

    /// <summary>
    /// Type of the following data:
    /// </summary>
    public TType Type
    {
      get
      {
        return (TType)m_usType;
      }
      set
      {
        m_usType = (ushort)value;
      }
    }

    /// <summary>
    /// Build identifier:
    /// </summary>
    public ushort Build
    {
      get
      {
        return m_usBuild;
      }
      set
      {
        m_usBuild = value;
      }
    }

    /// <summary>
    /// Build year:
    /// </summary>
    public ushort Year
    {
      get
      {
        return m_usYear;
      }
      set
      {
        m_usYear = value;
      }
    }

    /// <summary>
    /// File history flag:
    /// </summary>
    public int History
    {
      get
      {
        return m_iHistory;
      }
      set
      {
        m_iHistory = value;
      }
    }

    /// <summary>
    /// Lowest Excel version that can read all records in this file.
    /// </summary>
    public int RequeredVersion
    {
      get
      {
        return m_iRVersion;
      }
      set
      {
        m_iRVersion = value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsNested
    {
      get
      {
        return m_bIsNested;
      }
      set
      {
        m_bIsNested = value;
      }
    }
    /// <summary>
    /// Indicates whether record allows shorter data. Read-only.
    /// </summary>
    public override bool IsAllowShortData
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Indicates whether record need decoding when file is encoded or not. Read-only.
    /// </summary>
    public override bool NeedDecoding
    {
      get
      {
        return false;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default Constructor
    /// </summary>
    public  BOFRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize Constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  BOFRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for the record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  BOFRecord( int iReserve )
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
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usVersion = provider.ReadUInt16( iOffset );
      m_usType = provider.ReadUInt16( iOffset + 2 );
      m_usBuild = provider.ReadUInt16( iOffset + 4 );
      m_usYear = provider.ReadUInt16( iOffset + 6 );
      m_iHistory = provider.ReadInt32( iOffset + 8 );
      m_iRVersion = provider.ReadInt32( iOffset + 12 );
    }

    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_iLength = DEF_RECORD_SIZE;

      provider.WriteUInt16( iOffset, m_usVersion );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usType );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usBuild );
      iOffset += 2;

      provider.WriteUInt16( iOffset, m_usYear );
      iOffset += 2;

      provider.WriteInt32( iOffset, m_iHistory );
      iOffset += 4;

      provider.WriteInt32( iOffset, m_iRVersion );
      iOffset += 4;
    }

    #endregion
  }
}
