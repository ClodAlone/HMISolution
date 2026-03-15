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
  /// This record stores a PivotTable name.
  /// </summary>
  [ Biff( TBIFFRecord.PivotName ) ]
  [ CLSCompliant( false ) ]
  public class PivotNameRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Option flags.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usOptions;
    /// <summary>
    /// If it is set to true then the name is invalid and should
    /// be displayed and evaluated as #NAME.
    /// </summary>
    [ BiffRecordPos( 0, 1, TFieldType.Bit ) ]
    private bool m_bErrorName;
    /// <summary>
    /// Field to aggregate in calculated field formulas.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usAggregateField;
    /// <summary>
    /// Function to use for aggregation in calculated field formulas.
    /// </summary>
    [ BiffRecordPos( 4, 2 ) ]
    private ushort m_usAggregateFunction;
    /// <summary>
    /// Number of SXPAIR records to follow this record.
    /// </summary>
    [ BiffRecordPos( 6, 2 ) ]
    private ushort m_usPairCount;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotNameRecord()
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
    public  PivotNameRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotNameRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
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
    /// If it is set to true then the name is invalid and should
    /// be displayed and evaluated as #NAME.
    /// </summary>
    public bool IsErrorName
    {
      get
      {
        return m_bErrorName;
      }
      set
      {
        m_bErrorName = value;
      }
    }
    /// <summary>
    /// Field to aggregate in calculated field formulas.
    /// </summary>
    public ushort AggregateField
    {
      get
      {
        return m_usAggregateField;
      }
      set
      {
        m_usAggregateField = value;
      }
    }
    /// <summary>
    /// Function to use for aggregation in calculated field formulas.
    /// </summary>
    public ushort AggregateFunction
    {
      get
      {
        return m_usAggregateFunction;
      }
      set
      {
        m_usAggregateFunction = value;
      }
    }
    /// <summary>
    /// Number of SXPAIR records to follow this record.
    /// </summary>
    public ushort PairCount
    {
      get
      {
        return m_usPairCount;
      }
      set
      {
        m_usPairCount = value;
      }
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
    public override void ParseStructure( DataProvider provider, int iOffset, int iLength, ExcelVersion version )
    {
      m_usOptions = provider.ReadUInt16( iOffset + 0 );
      m_bErrorName = provider.ReadBit( iOffset + 0, 1 );
      m_usAggregateField = provider.ReadUInt16( iOffset + 2 );
      m_usAggregateFunction = provider.ReadUInt16( iOffset + 4 );
      m_usPairCount = provider.ReadUInt16( iOffset + 6 );
    }
    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset in the buffer.</param>
    /// <param name="version">Excel version used for infill.</param>
    /// <returns>Size of the record data.</returns>
    public override void InfillInternalData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteUInt16( iOffset + 0, m_usOptions );
      provider.WriteBit( iOffset + 0, m_bErrorName, 1 );
      provider.WriteUInt16( iOffset + 2, m_usAggregateField );
      provider.WriteUInt16( iOffset + 4, m_usAggregateFunction );
      provider.WriteUInt16( iOffset + 6, m_usPairCount );
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
