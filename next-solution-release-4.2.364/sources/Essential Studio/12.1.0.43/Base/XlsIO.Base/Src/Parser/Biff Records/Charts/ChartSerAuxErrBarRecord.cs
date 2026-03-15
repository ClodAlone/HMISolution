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
  /// This record defines the series error bars.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSerAuxErrBar ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartSerAuxErrBarRecord : BiffRecordRaw
  {
    #region Class cosntants
    /// <summary>
    /// Error-bar type.
    /// </summary>
    public enum TErrorBarValue : int
    {
      /// <summary>
      /// Represents the XDirectionPlus error-bar type.
      /// </summary>
      XDirectionPlus  = 1,
      /// <summary>
      /// Represents the XDirectionMinus error-bar type.
      /// </summary>
      XDirectionMinus = 2,
      /// <summary>
      /// Represents the YDirectionPlus error-bar type.
      /// </summary>
      YDirectionPlus  = 3,
      /// <summary>
      /// Represents the YDirectionMinus error-bar type.
      /// </summary>
      YDirectionMinus = 4
    }

    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DefaultRecordSize = 14;
    #endregion

    #region Class members
    /// <summary>
    /// Error-bar type.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_ErrorBarValue;
    /// <summary>
    /// Error-bar value source.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_ErrorBarType = ( byte )ExcelErrorBarType.Fixed;
    /// <summary>
    /// True if the error bars are T-shaped (have a line on the top and bottom).
    /// </summary>
    [ BiffRecordPos( 2, 1 ) ]
    private byte m_TeeTop = 1;
    /// <summary>
    /// Reserved; must be 1.
    /// </summary>
    [ BiffRecordPos( 3, 1 ) ]
    private byte m_Reserved = 1;
    /// <summary>
    /// IEEE number; specifies the fixed value, percentage,
    /// or number of standard deviations for the error bars.
    /// </summary>
    [ BiffRecordPos( 4, 8, TFieldType.Float ) ]
    private double m_NumValue = 10;
    /// <summary>
    /// Number of values or cell references used for custom error bars.
    /// </summary>
    [ BiffRecordPos( 12, 2 ) ]
    private ushort m_usValuesNumber;
    #endregion

    #region Class properties
    /// <summary>
    /// Error-bar type.
    /// </summary>
    public TErrorBarValue ErrorBarValue
    {
      get
      {
        return ( TErrorBarValue )m_ErrorBarValue;
      }
      set
      {
        m_ErrorBarValue = ( byte )value;
      }
    }
    /// <summary>
    /// Error-bar value source.
    /// </summary>
    public ExcelErrorBarType ErrorBarType
    {
      get
      {
        return ( ExcelErrorBarType )m_ErrorBarType;
      }
      set
      {
        m_ErrorBarType = ( byte )value;
      }
    }
    /// <summary>
    /// True if the error bars are T-shaped (have a line on the top and bottom).
    /// </summary>
    public bool TeeTop
    {
      get
      {
        return (m_TeeTop == 1);
      }
      set
      {
        m_TeeTop = (byte) (value ? 1 : 0);
      }
    }
    /// <summary>
    /// Reserved; must be 1. Read-only.
    /// </summary>
    public byte Reserved
    {
      get
      {
        return m_Reserved;
      }
    }
    /// <summary>
    /// IEEE number; specifies the fixed value, percentage,
    /// or number of standard deviations for the error bars.
    /// </summary>
    public double NumValue
    {
      get
      {
        return m_NumValue;
      }
      set
      {
        m_NumValue = value;
      }
    }
    /// <summary>
    /// Number of values or cell references used for custom error bars.
    /// </summary>
    public ushort ValuesNumber
    {
      get
      {
        return m_usValuesNumber;
      }
      set
      {
        m_usValuesNumber = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartSerAuxErrBarRecord()
      : base()
    {
    }
    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">If stream is not specified.</exception>
    /// <exception cref="System.ApplicationException">If stream does not support read or seek operations.</exception>
    public  ChartSerAuxErrBarRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSerAuxErrBarRecord( int iReserve )
      : base( iReserve )
    {
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
      m_ErrorBarValue = provider.ReadByte( iOffset + 0 );
      m_ErrorBarType = provider.ReadByte( iOffset + 1 );
      m_TeeTop = provider.ReadByte( iOffset + 2 );
      m_Reserved = provider.ReadByte( iOffset + 3 );
      m_NumValue = provider.ReadDouble( iOffset + 4 );
      m_usValuesNumber = provider.ReadUInt16( iOffset + 12 );
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
      provider.WriteByte( iOffset + 0, m_ErrorBarValue );
      provider.WriteByte( iOffset + 1, m_ErrorBarType );
      provider.WriteByte( iOffset + 2, m_TeeTop );
      provider.WriteByte( iOffset + 3, m_Reserved );
      provider.WriteDouble( iOffset + 4, m_NumValue );
      provider.WriteUInt16( iOffset + 12, m_usValuesNumber );
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