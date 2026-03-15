#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

namespace Syncfusion.XlsIO.Parser.Biff_Records.PivotTable
{
  /// <summary>
  /// This record stores PivotTable rule data.
  /// </summary>
  [ Biff( TBIFFRecord.RuleData ) ]
  [ CLSCompliant( false ) ]
  public class RuleDataRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    /// <summary>
    /// Bit mask for RuleType property.
    /// </summary>
    private const ushort DEF_BITMASK_RULETYPE = 0x00F0;
    /// <summary>
    /// First bit for RuleType property.
    /// </summary>
    private const ushort DEF_BIT_RULETYPE = 4;
    #endregion

    #region Class members
    /// <summary>
    /// Position of current field in axis.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_btDim;
    /// <summary>
    /// Current field.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_btCurrentField;
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// Indicates whether current field is in row area.
    /// </summary>
    [ BiffRecordPos( 2, 0, TFieldType.Bit ) ]
    private bool m_bRowArea;
    /// <summary>
    /// Indicates whether current field is in column area.
    /// </summary>
    [ BiffRecordPos( 2, 1, TFieldType.Bit ) ]
    private bool m_bColumnArea;
    /// <summary>
    /// Indicates whether current field is in page area.
    /// </summary>
    [ BiffRecordPos( 2, 2, TFieldType.Bit ) ]
    private bool m_bPageArea;
    /// <summary>
    /// Indicates whether current field is in data area.
    /// </summary>
    [ BiffRecordPos( 2, 3, TFieldType.Bit ) ]
    private bool m_bDataArea;

    /// <summary>
    /// Indicates whether header is not selected.
    /// </summary>
    [ BiffRecordPos( 3, 1, TFieldType.Bit ) ]
    private bool m_bNoHeader;
    /// <summary>
    /// Indicates whether data is not selected.
    /// </summary>
    [ BiffRecordPos( 3, 2, TFieldType.Bit ) ]
    private bool m_bNoData;
    /// <summary>
    /// Indicates whether row grand total is selected.
    /// </summary>
    [ BiffRecordPos( 3, 3, TFieldType.Bit ) ]
    private bool m_bGrandRow;
    /// <summary>
    /// Indicates whether column grand total is selected.
    /// </summary>
    [ BiffRecordPos( 3, 4, TFieldType.Bit ) ]
    private bool m_bGrandColumn;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 3, 5, TFieldType.Bit ) ]
    private bool m_bGrandRowSav;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 3, 6, TFieldType.Bit ) ]
    private bool m_bCacheBased;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 3, 7, TFieldType.Bit ) ]
    private bool m_bGrandColSav;
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usReserved;
    /// <summary>
    /// Number of SXFILT records following this record.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usFiltersCount;
    private int? m_iReserved;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  RuleDataRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">When stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">When stream does not support read or seek operations.</exception>
    public  RuleDataRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  RuleDataRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion
    
    #region Class properties
    /// <summary>
    /// Position of current field in axis.
    /// </summary>
    public byte Dim
    {
      get
      {
        return m_btDim;
      }
      set
      {
        m_btDim = value;
      }
    }
    /// <summary>
    /// Current field.
    /// </summary>
    public byte CurrentField
    {
      get
      {
        return m_btCurrentField;
      }
      set
      {
        m_btCurrentField = value;
      }
    }
    /// <summary>
    /// Option flags. Read-only.
    /// </summary>
    public ushort Options
    {
      get
      {
        return m_usOptions;
      }
#if DEBUG
      set
      {
        m_usOptions = value;
      }
#endif
    }
    /// <summary>
    /// Indicates whether current field is in row area.
    /// </summary>
    public bool IsRowArea
    {
      get
      {
        return m_bRowArea;
      }
      set
      {
        m_bRowArea = value;
      }
    }
    /// <summary>
    /// Indicates whether current field is in column area.
    /// </summary>
    public bool IsColumnArea
    {
      get
      {
        return m_bColumnArea;
      }
      set
      {
        m_bColumnArea = value;
      }
    }
    /// <summary>
    /// Indicates whether current field is in page area.
    /// </summary>
    public bool IsPageArea
    {
      get
      {
        return m_bPageArea;
      }
      set
      {
        m_bPageArea = value;
      }
    }
    /// <summary>
    /// Indicates whether current field is in data area.
    /// </summary>
    public bool IsDataArea
    {
      get
      {
        return m_bDataArea;
      }
      set
      {
        m_bDataArea = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ushort RuleType
    {
      get
      {
        return ( ushort )( GetUInt16BitsByMask( m_usOptions, DEF_BITMASK_RULETYPE ) >> DEF_BIT_RULETYPE );
      }
      set
      {
        ushort valueToSet = ( ushort )( value << DEF_BIT_RULETYPE );
        SetUInt16BitsByMask( ref m_usOptions, DEF_BITMASK_RULETYPE, valueToSet );
      }
    }
    /// <summary>
    /// Indicates whether header is not selected.
    /// </summary>
    public bool IsNoHeader
    {
      get
      {
        return m_bNoHeader;
      }
      set
      {
        m_bNoHeader = value;
      }
    }
    /// <summary>
    /// Indicates whether data is not selected.
    /// </summary>
    public bool IsNoData
    {
      get
      {
        return m_bNoData;
      }
      set
      {
        m_bNoData = value;
      }
    }
    /// <summary>
    /// Indicates whether row grand total is selected.
    /// </summary>
    public bool IsGrandRow
    {
      get
      {
        return m_bGrandRow;
      }
      set
      {
        m_bGrandRow = value;
      }
    }
    /// <summary>
    /// Indicates whether column grand total is selected.
    /// </summary>
    public bool IsGrandColumn
    {
      get
      {
        return m_bGrandColumn;
      }
      set
      {
        m_bGrandColumn = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsGrandRowSav
    {
      get
      {
        return m_bGrandRowSav;
      }
      set
      {
        m_bGrandRowSav = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsCacheBased
    {
      get
      {
        return m_bCacheBased;
      }
      set
      {
        m_bCacheBased = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsGrandColSav
    {
      get
      {
        return m_bGrandColSav;
      }
      set
      {
        m_bGrandColSav = value;
      }
    }
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    public ushort Reserved
    {
      get
      {
        return m_usReserved;
      }
      set
      {
        m_usReserved = value;
      }
    }
    /// <summary>
    /// Number of SXFILT records following this record.
    /// </summary>
    public ushort FiltersCount
    {
      get
      {
        return m_usFiltersCount;
      }
      set
      {
        m_usFiltersCount = value;
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
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_btDim = provider.ReadByte( iOffset + 0 );
      m_btCurrentField = provider.ReadByte( iOffset + 1 );
      m_usOptions = provider.ReadUInt16( iOffset + 2 );
      m_bRowArea = provider.ReadBit( iOffset + 2, 0 );
      m_bColumnArea = provider.ReadBit( iOffset + 2, 1 );
      m_bPageArea = provider.ReadBit( iOffset + 2, 2 );
      m_bDataArea = provider.ReadBit( iOffset + 2, 3 );
      m_bNoHeader = provider.ReadBit( iOffset + 3, 1 );
      m_bNoData = provider.ReadBit( iOffset + 3, 2 );
      m_bGrandRow = provider.ReadBit( iOffset + 3, 3 );
      m_bGrandColumn = provider.ReadBit( iOffset + 3, 4 );
      m_bGrandRowSav = provider.ReadBit( iOffset + 3, 5 );
      m_bCacheBased = provider.ReadBit( iOffset + 3, 6 );
      m_bGrandColSav = provider.ReadBit( iOffset + 3, 7 );
      m_usReserved = provider.ReadUInt16( iOffset + 4 );
      m_usFiltersCount = provider.ReadUInt16( iOffset + 6 );

      if( iLength > 8 )
        m_iReserved = provider.ReadInt32( iOffset + 8 );
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
      provider.WriteByte( iOffset + 0, m_btDim );
      provider.WriteByte( iOffset + 1, m_btCurrentField );
      provider.WriteUInt16( iOffset + 2, m_usOptions );
      provider.WriteBit( iOffset + 2, m_bRowArea, 0 );
      provider.WriteBit( iOffset + 2, m_bColumnArea, 1 );
      provider.WriteBit( iOffset + 2, m_bPageArea, 2 );
      provider.WriteBit( iOffset + 2, m_bDataArea, 3 );
      provider.WriteBit( iOffset + 3, m_bNoHeader, 1 );
      provider.WriteBit( iOffset + 3, m_bNoData, 2 );
      provider.WriteBit( iOffset + 3, m_bGrandRow, 3 );
      provider.WriteBit( iOffset + 3, m_bGrandColumn, 4 );
      provider.WriteBit( iOffset + 3, m_bGrandRowSav, 5 );
      provider.WriteBit( iOffset + 3, m_bCacheBased, 6 );
      provider.WriteBit( iOffset + 3, m_bGrandColSav, 7 );
      provider.WriteUInt16( iOffset + 4, m_usReserved );
      provider.WriteUInt16( iOffset + 6, m_usFiltersCount );
      m_iLength = DefaultRecordSize;

      if( m_iReserved != null )
      {
        provider.WriteInt32( iOffset + 8, ( int )m_iReserved );
        m_iLength += ExcelConstants.IntSize;
      }
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int result = DefaultRecordSize;

      if( m_iReserved != null )
        result += ExcelConstants.IntSize;

      return result;
    }
    #endregion
  }
}
