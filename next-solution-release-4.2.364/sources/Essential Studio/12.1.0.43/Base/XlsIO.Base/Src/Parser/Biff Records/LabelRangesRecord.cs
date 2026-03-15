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

using System;
using System.IO;
using Syncfusion.XlsIO.Implementation.Exceptions;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record contains the addresses of all row and column label ranges
  /// in the current sheet.
  /// </summary>
  [ Biff( TBIFFRecord.LabelRanges ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class LabelRangesRecord : BiffRecordRawWithArray
  {
    #region Class members

    /// <summary>
    /// Number of ranges in row ranges array.
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRowRangesCount = 0;

    /// <summary>
    /// Cell range address list with all row label ranges.
    /// </summary>
    private TAddr[] m_arrRowRanges = null;

    /// <summary>
    /// Number of ranges in column ranges array.
    /// </summary>
    private ushort m_usColRangesCount = 0;

    /// <summary>
    /// Cell range address list with all column label ranges.
    /// </summary>
    private TAddr[] m_arrColRanges = null;
    #endregion

    #region Class properties

    /// <summary>
    /// Read-only. Number of ranges in row ranges array.
    /// </summary>
    public ushort RowRangesCount
    {
      get
      {
        return m_usRowRangesCount;
      }
    }

    /// <summary>
    /// Cell range address list with all row label ranges.
    /// </summary>
    public TAddr[] RowRanges
    {
      get
      {
        return m_arrRowRanges;
      }
      set
      {
        m_arrRowRanges = value;
        m_usRowRangesCount = ( value != null ) ? ( ushort ) value.Length : ( ushort ) 0;
      }
    }

    /// <summary>
    /// Read-only. Number of ranges in column ranges array.
    /// </summary>
    public ushort ColRangesCount
    {
      get
      {
        return m_usColRangesCount;
      }
    }

    /// <summary>
    /// Cell range address list with all column label ranges.
    /// </summary>
    public TAddr[] ColRanges
    {
      get
      {
        return m_arrColRanges;
      }
      set
      {
        m_arrColRanges = value;
        m_usColRangesCount = ( value != null ) ? ( ushort ) value.Length : ( ushort ) 0;
      }
    }

    /// <summary>
    /// Read-only. Minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return 4;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  LabelRangesRecord()
      : base()
    {
    }

    /// <summary>
    /// Read / initialize constructor.
    /// </summary>
    /// <param name="stream">Stream from which record data should be read.</param>
    /// <param name="itemSize">Size of read item.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is not specified.
    /// </exception>
    /// <exception cref="System.ApplicationException">
    /// If stream does not support read or seek operations.
    /// </exception>
    public  LabelRangesRecord( Stream stream, out int itemSize )
      : base( stream, out itemSize )
    {
    }

    /// <summary>
    /// Reserved for record's internal data array.
    /// </summary>
    /// <param name="iReserve">Amount of bytes for data array.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If amount of bytes requested is less than zero.
    /// </exception>
    public  LabelRangesRecord( int iReserve )
      : base( iReserve )
    {
    }

    #endregion

    #region Record Serialization

    /// <summary>
    /// Parse structure of record. Converts data buffer to special
    /// values according to record specification.
    /// </summary>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.WrongBiffRecordDataException">
    /// If last byte of the parsed data won't be the last byte of read data.
    /// </exception>
    public override void ParseStructure()
    {
      AutoExtractFields();

      m_arrRowRanges = new TAddr[ m_usRowRangesCount ];

      int offset = 2;
      for( int i = 0; i < m_usRowRangesCount; i++, offset += 8 )
      {
        m_arrRowRanges[ i ] = GetAddr( offset );
      }

      m_usColRangesCount = GetUInt16( offset );
      m_arrColRanges = new TAddr[ m_usColRangesCount ];
      offset += 2;

      for( int i = 0; i < m_usColRangesCount; i++, offset += 8 )
      {
        m_arrColRanges[ i ] = GetAddr( offset );
      }

      if( offset != m_iLength )
      {
        throw new WrongBiffRecordDataException();
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      AutoExtractFields();

      int offset = 2;
      for( int i = 0; i < m_usRowRangesCount; i++, offset += 8 )
      {
        SetAddr( offset, m_arrRowRanges[ i ] );
      }

      SetUInt16( offset, m_usColRangesCount );
      offset += 2;

      for( int i = 0; i < m_usColRangesCount; i++, offset += 8 )
      {
        SetAddr( offset, m_arrColRanges[ i ] );
      }
    }

    #endregion
  }
}
