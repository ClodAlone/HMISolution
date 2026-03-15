#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Represents a cell that contains an RK value
  /// (encoded integer or floating-point value).
  /// </summary>
  [ Biff( TBIFFRecord.RK ) ]
  [ CLSCompliant( false ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  public class RKRecord :
    CellPositionBase,
    IDoubleValue,
    IValueHolder
  {
    #region Class constants
    /// <summary>
    /// Correct record size.
    /// </summary>
    internal const int DEF_RECORD_SIZE = 10;
    /// <summary>
    /// Record size with header.
    /// </summary>
    internal const int DEF_RECORD_SIZE_WITH_HEADER = DEF_RECORD_SIZE + DEF_HEADER_SIZE;
    /// <summary>
    /// Offset to the number from the start of the record's data.
    /// </summary>
    internal const int DEF_NUMBER_OFFSET = 6;
    /// <summary>
    /// Offset to the number from the start of the record's data.
    /// </summary>
    internal const int DEF_HEADER_NUMBER_OFFSET = DEF_NUMBER_OFFSET + DEF_HEADER_SIZE;
    /// <summary>
    /// Represents rk mask.
    /// </summary>
    public const uint DEF_RK_MASK = 0xfffffffc;
    /// <summary>
    /// Maximum number that is possible to store as rk record.
    /// </summary>
    private const int MaxRkNumber = 0x20000000;
    /// <summary>
    /// Minimum number that is possible to store as rk record.
    /// </summary>
    private const int MinRkNumber = -MaxRkNumber;
    #endregion

    #region Class members
    /// <summary>
    /// RK value.
    /// </summary>
    [ BiffRecordPos( 6, 4, true ) ]
    private int m_iNumber = 0;
    /// <summary>
    /// True if value is multiplied by 100.
    /// </summary>
    [ BiffRecordPos( 6, 0, TFieldType.Bit ) ]
    private bool m_bValueNotChanged;

    /// <summary>
    /// True if signed integer; False if floating-point value.
    /// </summary>
    [ BiffRecordPos( 6, 1, TFieldType.Bit ) ]
    private bool m_bIEEEFloat;
    #endregion

    #region Class properties
    /// <summary>
    /// Read-only. RK value.
    /// </summary>
    public int    RKNumberInt
    {
      get
      {
        return m_iNumber;
      }
      set
      {
        m_iNumber = value;
        m_bValueNotChanged = ( value & 0x1 ) != 0;
        m_bIEEEFloat = ( value & 0x2 ) != 0;
      }
    }
    /// <summary>
    /// RK value converted to double.
    /// </summary>
    public double RKNumber
    {
      get
      {
        long lValue = ( m_iNumber >> 2 );

        if( IsNotFloat )
        {
          double dbValue = (double)lValue;
          return ( IsValueChanged ) ? dbValue / 100.0 : dbValue;
        }
        else
        {
          double dbValue = (double)BitConverterGeneral.Int64BitsToDouble( lValue << 34 );
          return ( IsValueChanged ) ? dbValue / 100.0 : dbValue;
        }
      }
      set
      {
        SetRKNumber( value );
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
    /// True if this is not a floating point value.
    /// </summary>
    public bool IsNotFloat
    {
      get
      {
        return m_bIEEEFloat;
      }
      set
      {
        m_bIEEEFloat = value;
      }
    }
    /// <summary>
    /// True if value was multiplied by 100.
    /// </summary>
    public bool IsValueChanged
    {
      get
      {
        return m_bValueNotChanged;
      }
      set
      {
        m_bValueNotChanged = value;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public  RKRecord()
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
      m_iNumber = provider.ReadInt32( iOffset );
      m_bIEEEFloat = provider.ReadBit( iOffset, 1 );
      m_bValueNotChanged = provider.ReadBit( iOffset, 0 );
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
      provider.WriteInt32( iOffset, m_iNumber );
      provider.WriteBit( iOffset, m_bIEEEFloat, 1 );
      provider.WriteBit( iOffset, m_bValueNotChanged, 0 );
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
    #endregion

    #region RkNumbers
    /// <summary>
    /// Converts string to the Rk number.
    /// </summary>
    /// <param name="value">String to convert.</param>
    public void SetRKNumber( string value )
    {
      double dbValue;

      if( Double.TryParse( value, NumberStyles.Any, null, out dbValue ) )
      {
        SetRKNumber( dbValue );
      }
    }
    /// <summary>
    /// Sets RkNumber to the specified value.
    /// </summary>
    /// <param name="value">Double value to set.</param>
    public void SetRKNumber( double value )
    {
      m_iNumber = ConvertToRKNumber( value );
      m_bValueNotChanged = ( ( m_iNumber & 0x01 ) != 0 );
      m_bIEEEFloat = ( ( m_iNumber & 0x02 ) != 0 );
    }
    /// <summary>
    /// Sets integer value to the specified value.
    /// </summary>
    /// <param name="rkNumber">Value to set.</param>
    public void SetConvertedNumber( int rkNumber )
    {
      m_iNumber = rkNumber;
      m_bValueNotChanged = ( ( m_iNumber & 0x01 ) != 0 );
      m_bIEEEFloat = ( ( m_iNumber & 0x02 ) != 0 );
    }
    /// <summary>
    /// Sets RkRecord values from MulRKRecord.RkRec
    /// </summary>
    /// <param name="rc">MulRKRecord.RkRec with needed values.</param>
    public void SetRKRecord( MulRKRecord.RkRec rc )
    {
      m_usExtendedFormat = rc.ExtFormatIndex;
      m_iNumber = rc.Rk;

      m_bIEEEFloat = ( m_iNumber & 0x2 ) == 0x2;
      m_bValueNotChanged = ( m_iNumber & 0x1 ) == 0x1;
    }
    /// <summary>
    /// Converts RKRecord into MulRKRecord.RkRec.
    /// </summary>
    /// <returns>Converted record.</returns>
    public MulRKRecord.RkRec GetAsRkRec()
    {
      // update m_iNumber fields
      if( m_bValueNotChanged ) m_iNumber |= 0x1;
      if( m_bIEEEFloat ) m_iNumber |= 0x2;

      return new MulRKRecord.RkRec( m_usExtendedFormat, m_iNumber );
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Converts string to RK number.
    /// </summary>
    /// <param name="value">String to parse.</param>
    /// <returns>Parsed RK number. If returns int.MaxValue - cannot parse RK number.</returns>
    public static int ConvertToRKNumber( string value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, value, "ConvertToRKNumber" );

      double dbValue;

      if( Double.TryParse( value, NumberStyles.Any, null, out dbValue ) )
      {
        return ConvertToRKNumber( dbValue );
      }

      return int.MaxValue;
    }

    /// <summary>
    /// Converts double to RK number.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted RK number. If returns int.MaxValue - cannot parse RK number.</returns>
    public static int ConvertToRKNumber( double value )
    {
      if( value > MaxRkNumber || value < MinRkNumber )
        return int.MaxValue;

      long lValue = BitConverterGeneral.DoubleToInt64Bits( value );
      int iRes = 0;
      bool isCalc = true;

      if( ( lValue & 0x3ffffffff ) == 0 )
      {
        iRes = ConvertDouble( lValue, false );
        isCalc = false;
      }

      if( isCalc )
      {
        int iInt = ( int )Math.Round( value, 0 );

        if( ( value - iInt ) == 0 && iInt > 0 && iInt <= 0x3fffffff )
        {
          iRes = iInt << 2;
          iRes |= 0x2;
          isCalc = false;
        }
      }

      if( isCalc )
      {
        value *= 100;
        lValue = BitConverterGeneral.DoubleToInt64Bits( value );

        if( ( lValue & 0x3ffffffff ) == 0 )
        {
          iRes = ConvertDouble( lValue, true );
          isCalc = false;
        }
      }

      if( isCalc )
      {
        int iInt = ( int )Math.Round( value, 0 );

        if( ( value - iInt ) == 0 && iInt > 0 && iInt <= 0x3fffffff )
        {
          iRes = iInt << 2;
          iRes |= 0x3;
        }
      }

      return ( isCalc )
        ? int.MaxValue
        : iRes;
    }

    /// <summary>
    /// Converts Rk number to double.
    /// </summary>
    /// <param name="rkNumber">Rk number to convert.</param>
    /// <returns>Converted double value.</returns>
    public static double ConvertToDouble( int rkNumber )
    {
      bool bIsValueChanged = ( ( rkNumber & 0x01 ) != 0 );
      bool bIsNotFloat = ( ( rkNumber & 0x02 ) != 0 );

      long lValue = ( rkNumber >> 2 );

      if( bIsNotFloat )
      {
        double dbValue = (double)lValue;
        return ( bIsValueChanged ) ? dbValue / 100.0 : dbValue;
      }
      else
      {
        double dbValue = (double)BitConverterGeneral.Int64BitsToDouble( lValue << 34 );
        return ( bIsValueChanged ) ? dbValue / 100.0 : dbValue;
      }
    }
    /// <summary>
    /// Converts double value to integer. Value as IEEE double or IEEE / 100 double.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <param name="bValueNotChanged">Indicates is convert to IEEE / 100.</param>
    /// <returns>Int value corresponding to the double value.</returns>
    private static int ConvertDouble( long value, bool bValueNotChanged )
    {
      int iResult = ( int )( value >> 32 );

      if( bValueNotChanged )
        iResult |= 0x1;

      return iResult;
    }
    /// <summary>
    /// Encodes rk number.
    /// </summary>
    /// <param name="value">Represents value to encode.</param>
    /// <returns>Returns encoded value.</returns>
    public static double EncodeRK( int value )
    {
      double num;

      if( ( value & 0x02 ) > 0 )
      {
        // int value.
        num = ( double )( value >> 2 );
      }
      else
      {
        num =
#if AllowUnsafeCode
          ( ApplicationImpl.UseUnsafeCodeStatic )
          ? UnsafeGetDouble( value )
          :
#endif
          SafeGetDouble( value );
      }

      if( ( value & 0x01 ) > 0 )
      {
        // divide by 100.
        num /= 100;
      }

      return num;
    }
#if AllowUnsafeCode
    /// <summary>
    /// Gets double value using unsafe code.
    /// </summary>
    /// <param name="value">Represents value.</param>
    /// <returns>Returns double value.</returns>
    private static double UnsafeGetDouble( int value )
    {
      double num = 0;

      unsafe
      {
        int* pNumber = ( int* )&num;
        *( pNumber + 1 ) = ( int )( value & DEF_RK_MASK );
        *pNumber = 0;
      }

      return num;
    }
#endif
    /// <summary>
    /// Gets double value using safe code.
    /// </summary>
    /// <param name="value">Represents value.</param>
    /// <returns>Returns double value.</returns>
    private static double SafeGetDouble( int value )
    {
      byte[] arrDouble = new byte[ 8 ];
      byte[] arrValue = BitConverter.GetBytes( value & DEF_RK_MASK );

      Buffer.BlockCopy( arrValue, 0, arrDouble, 4, 4 );

      return BitConverter.ToDouble( arrDouble, 0 );
    }
    /// <summary>
    /// Reads record's value from the data provider.
    /// </summary>
    /// <param name="provider">Provider to read data from.</param>
    /// <param name="recordStart">Offset to the record's start.</param>
    /// <param name="version">Excel version that was used to infill.</param>
    /// <returns>Record's value.</returns>
    public static int ReadValue( DataProvider provider, int recordStart, ExcelVersion version )
    {
      recordStart += DEF_HEADER_SIZE + ExcelConstants.IntSize + ExcelConstants.ShortSize; // row, column + xf

      if( version != ExcelVersion.Excel97to2003 )
      {
        recordStart += ExcelConstants.IntSize;
      }

      return provider.ReadInt32( recordStart );
    }
    #endregion

    #region IDoubleValue Members
    /// <summary>
    /// Returns double value. Read-only.
    /// </summary>
    public double DoubleValue
    {
      get
      {
        return RKNumber;
      }
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
        return RKNumber;
      }
      set
      {
        RKNumber = ( double )value;
      }
    }

    #endregion
  }
}
