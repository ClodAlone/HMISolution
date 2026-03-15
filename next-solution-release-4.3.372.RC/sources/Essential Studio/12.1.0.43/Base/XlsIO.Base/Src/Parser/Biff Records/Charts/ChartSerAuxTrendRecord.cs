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
  /// This record defines a series trend line.
  /// </summary>
  [ Biff( TBIFFRecord.ChartSerAuxTrend ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class ChartSerAuxTrendRecord : BiffRecordRaw
  {
    #region Class constants
    /// <summary>
    /// Represents default byte array for NaN value.
    /// </summary>
    public static readonly byte[] DEF_NAN_BYTE_ARRAY = { 0xff, 0xff, 0xff, 0xff, 0, 1, 0xff, 0xff };
    /// <summary>
    /// Represents default NaN value;
    /// </summary>
    public static readonly double DEF_NAN_VALUE = BitConverter.ToDouble( DEF_NAN_BYTE_ARRAY, 0 );
    /// <summary>
    /// Regression type.
    /// </summary>
    public enum TRegression
    {
      /// <summary>
      /// Represents the Polynomial regression type.
      /// </summary>
      Polynomial    = 0,
      /// <summary>
      /// Represents the Exponential regression type.
      /// </summary>
      Exponential   = 1,
      /// <summary>
      /// Represents the Logarithmic regression type.
      /// </summary>
      Logarithmic   = 2,
      /// <summary>
      /// Represents the Power regression type.
      /// </summary>
      Power         = 3,
      /// <summary>
      /// Represents the MovingAverage regression type.
      /// </summary>
      MovingAverage = 4,
    }
    /// <summary>
    /// Correct size of the record.
    /// </summary>
    public const int DEF_RECORD_SIZE = 28;
    #endregion

    #region Class members
    /// <summary>
    /// Regression type.
    /// </summary>
    [ BiffRecordPos( 0, 1 ) ]
    private byte m_RegType;
    /// <summary>
    /// Polynomial order or moving average period.
    /// </summary>
    [ BiffRecordPos( 1, 1 ) ]
    private byte m_Order = 1;
    /// <summary>
    /// IEEE number; specifies forced intercept.
    /// (#NA if no intercept is specified)
    /// </summary>
    [ BiffRecordPos( 2, 8, TFieldType.Float ) ]
    private double m_numIntercept = DEF_NAN_VALUE;
    /// <summary>
    /// True if the equation is displayed.
    /// </summary>
    [ BiffRecordPos( 10, 1 ) ]
    private byte m_bEquation;
    /// <summary>
    /// True if the R-squared value is displayed.
    /// </summary>
    [ BiffRecordPos( 11, 1 ) ]
    private byte m_bRSquared;
    /// <summary>
    /// IEEE number; specifies number of periods to forecast forward.
    /// </summary>
    [ BiffRecordPos( 12, 8, TFieldType.Float ) ]
    private double m_NumForecast;
    /// <summary>
    /// IEEE number; specifies number of periods to forecast backward.
    /// </summary>
    [ BiffRecordPos( 20, 8, TFieldType.Float ) ]
    private double m_NumBackcast;
    #endregion

    #region Class properties
    /// <summary>
    /// Regression type.
    /// </summary>
    public TRegression RegressionType
    {
      get
      {
        return (TRegression) m_RegType;
      }
      set
      {
        m_RegType = (byte) value;
      }
    }
    /// <summary>
    /// Polynomial order or moving average period.
    /// </summary>
    public byte Order
    {
      get
      {
        return m_Order;
      }
      set
      {
        m_Order = value;
      }
    }
    /// <summary>
    /// IEEE number; specifies forced intercept.
    /// (#NA if no intercept is specified)
    /// </summary>
    public double NumIntercept
    {
      get
      {
        return m_numIntercept;
      }
      set
      {
        m_numIntercept = value;
      }
    }
    /// <summary>
    /// True if the equation is displayed.
    /// </summary>
    public bool IsEquation
    {
      get
      {
        return (m_bEquation == 1);
      }
      set
      {
        m_bEquation = value ? (byte)1 : (byte)0;
      }
    }
    /// <summary>
    /// True if the R-squared value is displayed.
    /// </summary>
    public bool IsRSquared
    {
      get
      {
        return ( m_bRSquared == 1 );
      }
      set
      {
        m_bRSquared = (byte)( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// IEEE number; specifies number of periods to forecast forward.
    /// </summary>
    public double NumForecast
    {
      get
      {
        return m_NumForecast;
      }
      set
      {
        m_NumForecast = value;
      }
    }
    /// <summary>
    /// IEEE number; specifies number of periods to forecast backward.
    /// </summary>
    public double NumBackcast
    {
      get
      {
        return m_NumBackcast;
      }
      set
      {
        m_NumBackcast = value;
      }
    }
    /// <summary>
    /// Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    /// <summary>
    /// Maximum possible size of the record.
    /// </summary>
    public override int MaximumRecordSize
    {
      get
      {
        return DEF_RECORD_SIZE;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Updates trend line type on serialize.
    /// </summary>
    /// <param name="type">Represents trend line type.</param>
    public void UpdateType( ExcelTrendLineType type )
    {
      if( type != ExcelTrendLineType.Linear )
      {
        m_RegType = ( byte )type;
      }
      else
      {
        m_RegType = ( byte )ExcelTrendLineType.Polynomial;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor, initializes all fields with default values.
    /// </summary>
    public  ChartSerAuxTrendRecord()
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
    public  ChartSerAuxTrendRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }
    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  ChartSerAuxTrendRecord( int iReserve )
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
      m_RegType = provider.ReadByte( iOffset + 0 );
      m_Order = provider.ReadByte( iOffset + 1 );
      m_numIntercept = provider.ReadDouble( iOffset + 2 );
      m_bEquation = provider.ReadByte( iOffset + 10 );
      m_bRSquared = provider.ReadByte( iOffset + 11 );
      m_NumForecast = provider.ReadDouble( iOffset + 12 );
      m_NumBackcast = provider.ReadDouble( iOffset + 20 );
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
      provider.WriteByte( iOffset + 0, m_RegType );
      provider.WriteByte( iOffset + 1, m_Order );
      provider.WriteDouble( iOffset + 2, m_numIntercept );
      provider.WriteByte( iOffset + 10, m_bEquation );
      provider.WriteByte( iOffset + 11, m_bRSquared );
      provider.WriteDouble( iOffset + 12, m_NumForecast );
      provider.WriteDouble( iOffset + 20, m_NumBackcast );
      m_iLength = 28;
    }
    #endregion
  }
}
