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
using System.Text;


namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record contains the cell range and text for a ToolTip.
  /// It occurs in conjunction with the HLINK record for hyperlinks in the Hyperlink Table.
  /// </summary>
  [ Biff( TBIFFRecord.QuickTip ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class QuickTipRecord : BiffRecordRawWithArray
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    private const int DEF_FIXED_PART_SIZE = 10;
    #endregion

    #region Class members

    /// <summary>
    /// 0x0800 (repeated record identifier).
    /// </summary>
    [ BiffRecordPos( 0, 2 ) ]
    private ushort m_usRecordId = ( ushort ) TBIFFRecord.QuickTip;

    /// <summary>
    /// Cell range address of all cells containing the ToolTip.
    /// </summary>
    private TAddr m_addrCellRange;

    /// <summary>
    /// ToolTip string.
    /// </summary>
    private string m_strToolTip = string .Empty;
    #endregion

    #region Class properties
    /// <summary>
    /// Cell range address of all cells containing the ToolTip.
    /// </summary>
    public TAddr CellRange
    {
      get
      {
        return m_addrCellRange;
      }
      set
      {
        m_addrCellRange = value;
      }
    }
    /// <summary>
    /// ToolTip string.
    /// </summary>
    public string ToolTip
    {
      get
      {
        return m_strToolTip;
      }
      set
      {
        m_strToolTip = ( value[ value.Length - 1 ] != '\0' )
          ? value + "\0"
          : value;
      }
    }

    /// <summary>
    /// Read-only. Returns minimum possible size of the record.
    /// </summary>
    public override int MinimumRecordSize
    {
      get
      {
        return DEF_FIXED_PART_SIZE;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  QuickTipRecord()
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
    public  QuickTipRecord( Stream stream, out int itemSize )
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
    public  QuickTipRecord( int iReserve )
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
    /// If the last symbol of string (it is also last symbol of the data array) is not zero.
    /// </exception>
    public override void ParseStructure()
    {
      m_usRecordId = GetUInt16( 0 );

      if( m_usRecordId != 0x0800 )
      {
        throw new WrongBiffRecordDataException( "QuickTip first word must be 0x0800." );
      }

      if( m_iLength % 2 != 0 )
      {
        throw new WrongBiffRecordDataException( );
      }

      this.m_addrCellRange = GetAddr( 2 );
      m_strToolTip = Encoding.Unicode.GetString( m_data, 10, m_iLength - 10 );
      int iZeroIndex = m_strToolTip.IndexOf( '\0' );

      if (iZeroIndex != -1)
      {
          if (iZeroIndex != m_strToolTip.Length - 1)
          {
              throw new WrongBiffRecordDataException("Zero-terminated string does not fit data array.");
          }

          m_strToolTip = m_strToolTip.Remove(iZeroIndex, 1);
      }
    }

    /// <summary>
    /// In this method, a class must pack all of its properties into
    /// an internal data array, m_data. This method is called by
    /// FillStream, when the record must be serialized into a stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
      //m_usRecordId = ( ushort ) TBIFFRecord.QuickTip;
      m_data = new byte[ GetStoreSize( ExcelVersion.Excel97to2003 ) ];

      SetUInt16( 0, m_usRecordId );
      m_iLength = ExcelConstants.ShortSize;

      SetAddr( m_iLength, m_addrCellRange );
      m_iLength += 8;

      byte[] arrString = Encoding.Unicode.GetBytes( m_strToolTip );
      int iLength = arrString.Length;

      SetBytes( m_iLength, arrString );
      m_iLength += iLength;
    }

    /// <summary>
    /// Size of the required storage space. Read-only.
    /// </summary>
    public override int GetStoreSize( ExcelVersion version )
    {
      return DEF_FIXED_PART_SIZE + Encoding.Unicode.GetByteCount( m_strToolTip );
    }
    #endregion
  }
}
