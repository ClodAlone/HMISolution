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
  /// This record is an extension of the CacheDataRecord.
  /// Both records contain PivotTable cache data.
  /// Corresponds to SXDBEX in Excel format specification.
  /// </summary>
  [ Biff( TBIFFRecord.CacheDataEx ) ]
  [ CLSCompliant( false ) ]
  public class CacheDataExRecord : BiffRecordRaw
  {
    #region Constants
    /// <summary>
    /// Default record size.
    /// </summary>
    private const int DefaultRecordSize = 12;
    #endregion

    #region Class members
    /// <summary>
    /// The date that the PivotTable cache was created or was last refreshed.
    /// The date is stored as an 8-byte IEEE floating-point number.
    /// </summary>
    [ BiffRecordPos( 0, 8, TFieldType.Float ) ]
    private double m_dDate;
    /// <summary>
    /// Count of SXFormula records for this cache.
    /// </summary>
    [ BiffRecordPos( 8, 4 ) ]
    private uint m_uiFormulaCount;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  CacheDataExRecord()
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
    public  CacheDataExRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  CacheDataExRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// The date that the PivotTable cache was created or was last refreshed.
    /// The date is stored as an 8-byte IEEE floating-point number.
    /// </summary>
    public double RefreshDate
    {
      get
      {
        return m_dDate;
      }
      set
      {
        m_dDate = value;
      }
    }
    /// <summary>
    /// Count of SXFormula records for this cache.
    /// </summary>
    public uint FormulaCount
    {
      get
      {
        return m_uiFormulaCount;
      }
      set
      {
        m_uiFormulaCount = value;
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
      m_dDate = provider.ReadDouble( iOffset + 0 );
      m_uiFormulaCount = provider.ReadUInt32( iOffset + 8 );
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
      provider.WriteDouble( iOffset + 0, m_dDate );
      provider.WriteUInt32( iOffset + 8, m_uiFormulaCount );
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
