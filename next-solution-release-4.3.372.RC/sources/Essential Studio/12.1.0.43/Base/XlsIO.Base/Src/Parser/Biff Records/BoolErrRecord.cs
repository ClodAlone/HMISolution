#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.IO;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record represents a Boolean value or error value cell.
  /// </summary>
  [ Biff( TBIFFRecord.BoolErr ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class BoolErrRecord :
    CellPositionBase,
    IValueHolder
  {
    #region Class constants
    /// <summary>
    /// Size of the record.
    /// </summary>
    private const int DEF_RECORD_SIZE = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Boolean or error value (type depends on the m_bIsErrorCode).
    /// </summary>
    [ BiffRecordPos( 6, 1 ) ]
    private byte m_BoolOrError = 0;
    /// <summary>
    /// False = Boolean value; True = Error code.
    /// </summary>
    [ BiffRecordPos( 7, 1 ) ]
    private byte m_IsErrorCode = 0;
    #endregion

    #region Class Properties
    /// <summary>
    /// Boolean or error value (type depends on the IsError property).
    /// </summary>
    public byte BoolOrError
    {
      get
      {
        return m_BoolOrError;
      }
      set
      {
        m_BoolOrError = value;
      }
    }

    /// <summary>
    /// False = Boolean value; True = Error code.
    /// </summary>
    public bool IsErrorCode
    {
      get
      {
        return ( m_IsErrorCode == 1 );
      }
      set
      {
        m_IsErrorCode = value ? ( byte ) 1 : ( byte ) 0;
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
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default Constructor that fills all data with default values.
    /// </summary>
    public  BoolErrRecord()
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
      m_BoolOrError = provider.ReadByte( iOffset );
      m_IsErrorCode = provider.ReadByte( iOffset + 1 );
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
      provider.WriteByte( iOffset, m_BoolOrError );
      provider.WriteByte( iOffset + 1, m_IsErrorCode );
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
    /// <returns>Record's value (BoolOrError and IsErrorCode).</returns>
    public static int ReadValue( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += DEF_HEADER_SIZE + ExcelConstants.IntSize + ExcelConstants.ShortSize; // row, column + xf

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize;
      }

      return provider.ReadInt16( recordStart );
    }
    #endregion

    #region IValueHolder Members
    /// <summary>
    /// Value of the record.
    /// </summary>
    public object Value
    {
      get
      {
        return IsErrorCode ?
        ( object )BoolOrError :
        ( object )( BoolOrError != 0 );
      }
      set
      {
        if( value is bool )
        {
          IsErrorCode = false;
          BoolOrError = ( byte )value;
        }
        else
        {
          IsErrorCode = true;
          BoolOrError = ( byte )value;
        }
      }
    }

    #endregion
  }
}
