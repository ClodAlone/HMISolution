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
  /// This record stores PivotTable Rule Filter options.
  /// </summary>
  [ Biff( TBIFFRecord.RuleFilter ) ]
  [ CLSCompliant( false ) ]
  public class RuleFilterRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    /// <summary>
    /// BitMask for Dim field.
    /// </summary>
    private const ushort DEF_DIM_BITMASK = 0xFFC0;
    /// <summary>
    /// Start bit for Dim field.
    /// </summary>
    private const ushort DEF_DIM_START_BIT = 6;
    /// <summary>
    /// Bit mask for SXVD field.
    /// </summary>
    private const ushort DEF_SXVD_BITMASK = 0x3ff;
    /// <summary>
    /// Represents the Function types.
    /// </summary>
    public enum FunctionType
    {
      /// <summary>
      /// Represents the Data type.
      /// </summary>
      Data = 0x01,
      /// <summary>
      /// Represents the Default type.
      /// </summary>
      Default = 0x02,
      /// <summary>
      /// Represents the Sum type.
      /// </summary>
      Sum = 0x04,
      /// <summary>
      /// Represents the CountA type.
      /// </summary>
      CountA = 0x08,
      /// <summary>
      /// Represents the Count type.
      /// </summary>
      Count = 0x10,
      /// <summary>
      /// Represents the Average type.
      /// </summary>
      Average = 0x20,
      /// <summary>
      /// Represents the Max type.
      /// </summary>
      Max = 0x40,
      /// <summary>
      /// Represents the Min type.
      /// </summary>
      Min = 0x80,
      /// <summary>
      /// Represents the Product type.
      /// </summary>
      Product = 0x100,
      /// <summary>
      /// Represents the Stdev type.
      /// </summary>
      Stdev = 0x200,
      /// <summary>
      /// Represents the Stdevp type.
      /// </summary>
      Stdevp = 0x400,
      /// <summary>
      /// Represents the Var type.
      /// </summary>
      Var = 0x800,
      /// <summary>
      /// Represents the Varp type.
      /// </summary>
      Varp = 0x1000,
    }
    #endregion

    #region Class members
    /// <summary>
    /// Options flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions1;
    /// <summary>
    /// Indicates whether field is in row area.
    /// </summary>
    [ BiffRecordPos( 0, 0, TFieldType.Bit ) ]
    private bool m_bRowField;
    /// <summary>
    /// Indicates whether field is in column area.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bColumnField;
    /// <summary>
    /// Indicates whether field is in page area.
    /// </summary>
    [ BiffRecordPos( 0, 2, TFieldType.Bit ) ]
    private bool m_bPageField;
    /// <summary>
    /// Indicates whether field is in data area.
    /// </summary>
    [ BiffRecordPos( 0, 3, TFieldType.Bit ) ]
    private bool m_bDataField;
    /// <summary>
    /// Options flags.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usOptions2;
    /// <summary>
    /// 
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usFunction;
    /// <summary>
    /// Number of ViewItemRecord records.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usViewItemCount;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  RuleFilterRecord()
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
    public  RuleFilterRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  RuleFilterRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Options flags.
    /// </summary>
    public ushort Options1
    {
      get
      {
        return m_usOptions1;
      }
#if DEBUG
      set
      {
        m_usOptions1 = value;
      }
#endif
    }
    /// <summary>
    /// Indicates whether field is in row area.
    /// </summary>
    public bool IsRowField
    {
      get
      {
        return m_bRowField;
      }
      set
      {
        m_bRowField = value;
      }
    }
    /// <summary>
    /// Indicates whether field is in column area.
    /// </summary>
    public bool IsColumnField
    {
      get
      {
        return m_bColumnField;
      }
      set
      {
        m_bColumnField = value;
      }
    }
    /// <summary>
    /// Indicates whether field is in page area.
    /// </summary>
    public bool IsPageField
    {
      get
      {
        return m_bPageField;
      }
      set
      {
        m_bPageField = value;
      }
    }
    /// <summary>
    /// Indicates whether field is in data area.
    /// </summary>
    public bool IsDataField
    {
      get
      {
        return m_bDataField;
      }
      set
      {
        m_bDataField = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ushort Dim
    {
      get
      {
        return ( ushort )( GetUInt16BitsByMask( m_usOptions1, DEF_DIM_BITMASK ) >> DEF_DIM_START_BIT );
      }
      set
      {
        ushort usToSet = ( ushort )( value << DEF_DIM_START_BIT );
        SetUInt16BitsByMask( ref m_usOptions1, DEF_DIM_BITMASK, usToSet );
      }
    }
    /// <summary>
    /// Options flags. Read-only.
    /// </summary>
    public ushort Options2
    {
      get
      {
        return m_usOptions2;
      }
#if DEBUG
      set
      {
        m_usOptions2 = value;
      }
#endif
    }
    /// <summary>
    /// 
    /// </summary>
    public ushort SXVD
    {
      get
      {
        return GetUInt16BitsByMask( m_usOptions2, DEF_SXVD_BITMASK );
      }
      set
      {
        if( value > DEF_SXVD_BITMASK )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be greater than " + DEF_SXVD_BITMASK.ToString() );

        SetUInt16BitsByMask( ref m_usOptions2, DEF_SXVD_BITMASK, value );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public FunctionType Function
    {
      get
      {
        return ( FunctionType )m_usFunction;
      }
      set
      {
        m_usFunction = ( ushort )value;
      }
    }
    /// <summary>
    /// Number of ViewItemRecord records.
    /// </summary>
    public ushort ViewItemCount
    {
      get
      {
        return m_usViewItemCount;
      }
      set
      {
        m_usViewItemCount = value;
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
      m_usOptions1 = provider.ReadUInt16( iOffset + 0 );
      m_bRowField = provider.ReadBit( iOffset + 0, 0 );
      m_bColumnField = provider.ReadBit( iOffset + 0, 1 );
      m_bPageField = provider.ReadBit( iOffset + 0, 2 );
      m_bDataField = provider.ReadBit( iOffset + 0, 3 );
      m_usOptions2 = provider.ReadUInt16( iOffset + 2 );
      m_usFunction = provider.ReadUInt16( iOffset + 4 );
      m_usViewItemCount = provider.ReadUInt16( iOffset + 6 );
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
      provider.WriteUInt16( iOffset + 0, m_usOptions1 );
      provider.WriteBit( iOffset + 0, m_bRowField, 0 );
      provider.WriteBit( iOffset + 0, m_bColumnField, 1 );
      provider.WriteBit( iOffset + 0, m_bPageField, 2 );
      provider.WriteBit( iOffset + 0, m_bDataField, 3 );
      provider.WriteUInt16( iOffset + 2, m_usOptions2 );
      provider.WriteUInt16( iOffset + 4, m_usFunction );
      provider.WriteUInt16( iOffset + 6, m_usViewItemCount );
      m_iLength = DefaultRecordSize;
    }
    /// <summary>
    /// Evaluates size of the required storage space.
    /// </summary>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DefaultRecordSize;
    }
    #endregion
  }
}
