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
  /// Represents a cell that contains a floating-point value.
  /// </summary>
  [ Biff( TBIFFRecord.Number ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class NumberRecord :
    CellPositionBase,
    IDoubleValue,
    IValueHolder
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    private const int DEF_RECORD_SIZE = 14;
    #endregion

    #region Class members
    /// <summary>
    /// IEEE floating-point value.
    /// </summary>
    [ BiffRecordPos( 6, 8, TFieldType.Float ) ]
    private double m_dbValue = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// IEEE floating-point value.
    /// </summary>
    public double Value
    {
      get
      {
        return m_dbValue;
      }
      set
      {
        m_dbValue = value;
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
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Read-only. Returns maximum possible size of record's
    /// internal data array.
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
    public override int MaximumMemorySize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }

    /// <summary>
    /// Returns double value. Read-only.
    /// </summary>
    public double DoubleValue
    {
      get
      {
        return m_dbValue;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  NumberRecord()
      : base()
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
    /// <param name="version">Excel version used to fill data.</param>
    protected override void ParseCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      m_dbValue = provider.ReadDouble( iOffset );
    }
    /// <summary>
    /// In this method, class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="iOffset">Offset to the record's data.</param>
    /// <param name="version">Excel version used to fill data.</param>
    protected override void InfillCellData( DataProvider provider, int iOffset, ExcelVersion version )
    {
      provider.WriteDouble( iOffset, m_dbValue );
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = DEF_RECORD_SIZE;

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    /// <summary>
    /// Reads record's value from the data provider.
    /// </summary>
    /// <param name="provider">Provider to read data from.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version that was used to infill.</param>
    /// <returns>Record's value.</returns>
    public static double ReadValue( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += DEF_HEADER_SIZE + ExcelConstants.IntSize + ExcelConstants.ShortSize; // row, column + xf

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize;
      }

      return provider.ReadDouble( recordStart );
    }
    #endregion

    #region IValueHolder Members
    /// <summary>
    /// Value of the record.
    /// </summary>
    object IValueHolder.Value
    {
      get
      {
        return m_dbValue;
      }
      set
      {
        m_dbValue = ( double )value;
      }
    }

    #endregion
  }
}
