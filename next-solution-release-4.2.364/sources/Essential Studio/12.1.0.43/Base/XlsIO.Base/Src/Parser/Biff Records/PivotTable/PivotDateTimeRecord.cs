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
  /// Contains information about date/time value in the pivot table.
  /// </summary>
  [ Biff( TBIFFRecord.PivotDateTime ) ]
  [ CLSCompliant( false ) ]
  public class PivotDateTimeRecord
    : BiffRecordRaw
    , IValueHolder
  {
    #region Class constants
    /// <summary>
    /// Size of the record.
    /// </summary>
    private const int DefaultRecordSize = 8;
    #endregion

    #region Class members
    /// <summary>
    /// Year.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usYear;
    /// <summary>
    /// Months.
    /// </summary>
    [ BiffRecordPos( 2, 2 ) ]
    private ushort m_usMonth;
    /// <summary>
    /// Days.
    /// </summary>
    [ BiffRecordPos( 4, 1 ) ]
    private byte m_btDay;
    /// <summary>
    /// Hours.
    /// </summary>
    [ BiffRecordPos( 5, 1 ) ]
    private byte m_btHour;
    /// <summary>
    /// Minutes.
    /// </summary>
    [ BiffRecordPos( 6, 1 ) ]
    private byte m_btMinute;
    /// <summary>
    /// Seconds.
    /// </summary>
    [ BiffRecordPos( 7, 1 ) ]
    private byte m_btSecond;
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor.
    /// </summary>
    public  PivotDateTimeRecord()
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
    public  PivotDateTimeRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for the data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">If amount of bytes requested is less than zero.</exception>
    public  PivotDateTimeRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Year.
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
    /// Months.
    /// </summary>
    public ushort Month
    {
      get
      {
        return m_usMonth;
      }
      set
      {
        if( value < 1 || value > 12 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 12" );

        m_usMonth = value;
      }
    }
    /// <summary>
    /// Days.
    /// </summary>
    public byte Day
    {
      get
      {
        return m_btDay;
      }
      set
      {
        if( value < 1 || value > 31 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less 1 and greater than 31" );

        m_btDay = value;
      }
    }
    /// <summary>
    /// Hours.
    /// </summary>
    public byte Hour
    {
      get
      {
        return m_btHour;
      }
      set
      {
        if( value < 0 || value > 23 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0 and greater than 23" );

        m_btHour = value;
      }
    }
    /// <summary>
    /// Minutes.
    /// </summary>
    public byte Minute
    {
      get
      {
        return m_btMinute;
      }
      set
      {
        if( value < 0 || value > 59 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0 and greater than 59" );

        m_btMinute = value;
      }
    }
    /// <summary>
    /// Seconds.
    /// </summary>
    public byte Second
    {
      get
      {
        return m_btSecond;
      }
      set
      {
        if( value < 0 || value > 59 )
          throw new ArgumentOutOfRangeException( "value", "Value cannot be less than 0 and greater than 59" );

        m_btSecond = value;
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
        return DefaultRecordSize;
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
        return DefaultRecordSize;
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
      m_usYear = provider.ReadUInt16( iOffset + 0 );
      m_usMonth = provider.ReadUInt16( iOffset + 2 );
      m_btDay = provider.ReadByte( iOffset + 4 );
      m_btHour = provider.ReadByte( iOffset + 5 );
      m_btMinute = provider.ReadByte( iOffset + 6 );
      m_btSecond = provider.ReadByte( iOffset + 7 );
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
      provider.WriteUInt16( iOffset + 0, m_usYear );
      provider.WriteUInt16( iOffset + 2, m_usMonth );
      provider.WriteByte( iOffset + 4, m_btDay );
      provider.WriteByte( iOffset + 5, m_btHour );
      provider.WriteByte( iOffset + 6, m_btMinute );
      provider.WriteByte( iOffset + 7, m_btSecond );
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

    #region IValueHolder Members
    /// <summary>
    /// Value of the record.
    /// </summary>
    object IValueHolder.Value
    {
      get
      {
        return new DateTime( Year, Month, Day, Hour, Minute, Second, 0 );
      }
      set
      {
        DateTime dateTime = ( DateTime )value;
        Year = ( ushort )dateTime.Year;
        Month = ( byte )dateTime.Month;
        Day = ( byte )dateTime.Day;
        Hour = ( byte )dateTime.Hour;
        Minute = ( byte )dateTime.Minute;
        Second = ( byte )dateTime.Second;
      }
    }

    #endregion
  }
}
