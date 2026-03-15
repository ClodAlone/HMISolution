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
using System.Collections;

using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Collections.Generic;

#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Represents a range of empty cells. All cells are located in the same row.
  /// </summary>
  [ Biff( TBIFFRecord.MulBlank ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MulBlankRecord
    : CellPositionBase
    , IMultiCellRecord
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    public const int DEF_FIXED_SIZE = 6;
    /// <summary>
    /// Minimum record size.
    /// </summary>
    private const int DEF_MINIMUM_SIZE = DEF_FIXED_SIZE;
    /// <summary>
    /// Size of the subitem.
    /// </summary>
    public const int DEF_SUB_ITEM_SIZE = 2;
    #endregion

    #region Class members
    /// <summary>
    /// List of 16-bit indexes to XF records.
    /// </summary>
    private List<ushort> m_arrExtFormatIndexes;
    /// <summary>
    /// Index to last column.
    /// </summary>
    private int m_iLastCol = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Index to first column.
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return m_iColumn;
      }
      set
      {
        m_iColumn = value;
      }
    }

    /// <summary>
    /// List of 16-bit indexes to XF records.
    /// </summary>
    public List<ushort> ExtendedFormatIndexes
    {
      get
      {
        return m_arrExtFormatIndexes;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        m_arrExtFormatIndexes = value;
      }
    }

    /// <summary>
    /// Index to last column.
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_iLastCol;
      }
      set
      {
        m_iLastCol = value;
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
        return DEF_MINIMUM_SIZE;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  MulBlankRecord()
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
      iOffset -= 2; // we have to restore xf read by CellPositionBase class.

      if( m_iLength % 2 != 0 )
        throw new WrongBiffRecordDataException( "( Length - 6 ) % 2 != 0" );

      int iCount = ( m_iLength - 6 );

      if( version != ExcelVersion.Excel97to2003 )
      {
        iCount -= 6; // 2 additional bytes for 3 fields (row and first/last column).
      }

      iCount /= 2;
      m_arrExtFormatIndexes = new List<ushort>( iCount );

      for( int i = 0; i < iCount; iOffset += 2, i++ )
      {
        m_arrExtFormatIndexes.Add( provider.ReadUInt16( iOffset ) );
      }

      if( version == ExcelVersion.Excel97to2003 )
      {
        m_iLastCol = provider.ReadUInt16( iOffset );
      }
      else
      {
        m_iLastCol = provider.ReadInt32( iOffset );
      }

      InternalDataIntegrityCheck();
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
      m_iLength = GetStoreSize( version );

      iOffset -= 2; // we should subtracts size of the extended format.

      int iCount = m_arrExtFormatIndexes.Count;

      for( int i = 0; i < iCount; iOffset += 2, i++ )
      {
        provider.WriteUInt16( iOffset, m_arrExtFormatIndexes[ i ] );
      }

      provider.WriteUInt16( iOffset, ( ushort )m_iLastCol );
    }

    /// <summary>
    /// This method checks the record's internal data array for integrity.
    /// </summary>
    /// <exception cref="WrongBiffRecordDataException">If there is any internal error.</exception>
    private void InternalDataIntegrityCheck()
    {
      if( m_iLastCol - m_iColumn + 1 != m_arrExtFormatIndexes.Count )
      {
        throw new WrongBiffRecordDataException
          ( "m_usLastCol - m_usFirstCol + 1 != m_arrExtFormatIndexes.Length" );
      }
    }

    /// <summary>
    /// Creates BlankRecord corresponding to the specified column.
    /// </summary>
    /// <param name="iColumnIndex">Column index.</param>
    /// <returns>Corresponding BlankRecord.</returns>
    public BlankRecord GetBlankRecord( int iColumnIndex )
    {
      if( iColumnIndex < m_iColumn || iColumnIndex > m_iLastCol )
        throw new ArgumentOutOfRangeException( "iColumnIndex", "Value cannot be less m_usFirstCol and greater than m_usLastCol" );

      int index = iColumnIndex - m_iColumn;

      ushort usXFIndex = m_arrExtFormatIndexes[ index ];
      BlankRecord result = ( BlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Blank );

      result.Row = m_iRow;
      result.Column = iColumnIndex;
      result.ExtendedFormatIndex = usXFIndex;

      return result;
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = m_arrExtFormatIndexes.Count * DEF_SUB_ITEM_SIZE + DEF_FIXED_SIZE;//base.GetStoreSize( version );

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 6; // 2 additional bytes for last column, first column and row.

      return iResult;
    }
    /// <summary>
    /// Increases last column of the record.
    /// </summary>
    /// <param name="provider">Object that provides access to the data.</param>
    /// <param name="recordStart">Offset to the record start (record code).</param>
    /// <param name="iLength">Record data length.</param>
    /// <param name="columnDelta">Value that must be added to the column width.</param>
    /// <param name="version">Excel version used to fill data.</param>
    public static void IncreaseLastColumn( DataProvider provider, int recordStart,
      int iLength, ExcelVersion version, int columnDelta )
    {
      int iLastColumnOffset = recordStart + iLength + BiffRecordRaw.DEF_HEADER_SIZE;
      int iColumn;

      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          iLastColumnOffset -= ExcelConstants.ShortSize;
          iColumn = provider.ReadInt16( iLastColumnOffset ) + columnDelta;
          provider.WriteInt16( iLastColumnOffset, ( short )iColumn );
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          iLastColumnOffset -= ExcelConstants.IntSize;
          iColumn = provider.ReadInt32( iLastColumnOffset ) + columnDelta;
          provider.WriteInt32( iLastColumnOffset, ( short )iColumn );
          break;

        default:
          throw new ArgumentOutOfRangeException( "version" );
      }
    }
    #endregion

    #region IMultiCellRecord members
    /// <summary>
    /// Returns size of the subrecord if it was placed as separate record (including BiffRecord header). Read-only.
    /// </summary>
    public int GetSeparateSubRecordSize( ExcelVersion version )
    {
      int iResult = BlankRecord.DEF_RECORD_SIZE_WITH_HEADER;

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 4;

      return iResult;
    }
    /// <summary>
    /// Returns size of the subrecord. Read-only.
    /// </summary>
    public int SubRecordSize
    {
      get
      {
        return DEF_SUB_ITEM_SIZE;
      }
    }
    /// <summary>
    /// Returns type of the subrecord. Read-only.
    /// </summary>
    public TBIFFRecord SubRecordType
    {
      get
      {
        return TBIFFRecord.Blank;
      }
    }
    /// <summary>
    /// Inserts cell inside this record.
    /// </summary>
    /// <param name="cell">Cell to insert.</param>
    public void Insert( ICellPositionFormat cell )
    {
      int iColumnIndex = cell.Column;
      int iRowIndex = cell.Row;
      ushort usXFIndex = cell.ExtendedFormatIndex;

      if( Row != iRowIndex || m_iColumn >iColumnIndex || m_iLastCol < iColumnIndex )
        throw new ArgumentOutOfRangeException( "cell.Column" );

      m_arrExtFormatIndexes[ iColumnIndex - m_iColumn ] = usXFIndex;
    }
    /// <summary>
    /// Removes information about specified column from the record and splits record into two.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based index of the column to remove.</param>
    /// <returns>Splitted records.</returns>
    public ICellPositionFormat[] Split( int iColumnIndex )
    {
      if( iColumnIndex < m_iColumn || iColumnIndex > m_iLastCol )
        return new ICellPositionFormat[]{ this };

      int iLeftColumns = iColumnIndex - m_iColumn;
      int iRightColumns = m_iLastCol - iColumnIndex;
      ICellPositionFormat cellLeft = null;
      ICellPositionFormat cellRight = null;

      cellLeft = CreateRecord( m_iColumn, iColumnIndex - 1 );
      cellRight = CreateRecord( iColumnIndex + 1, m_iLastCol );

      ICellPositionFormat[] arrResult = new ICellPositionFormat[ 2 ] { cellLeft, cellRight };

      return arrResult;
    }
    /// <summary>
    /// Creates record based in the information from this record.
    /// </summary>
    /// <param name="iFirstCol">The first column index in the resulting record.</param>
    /// <param name="iLastCol">The last column index in the resulting record.</param>
    /// <returns>Created record.</returns>
    private ICellPositionFormat CreateRecord( int iFirstCol, int iLastCol )
    {
      if( iFirstCol > iLastCol ) return null;

      if( iFirstCol == iLastCol ) return CreateBlankRecord( iFirstCol );

      MulBlankRecord result = ( MulBlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulBlank );
      result.m_iColumn = iFirstCol;
      result.m_iLastCol = iLastCol;
      result.m_iRow = m_iRow;

      int iCount = iLastCol - iFirstCol + 1;
      List<ushort> arrNewIndexes = new List<ushort>( iCount );
      result.m_arrExtFormatIndexes = arrNewIndexes;

      for( int i = 0, iOffset = iFirstCol - m_iColumn; i < iCount; i++, iOffset++ )
      {
        arrNewIndexes[ i ] = m_arrExtFormatIndexes[ iOffset ];
      }

      return result;
    }
    /// <summary>
    /// Creates blank record with specified column index.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index of the created record.</param>
    /// <returns>Created record.</returns>
    private ICellPositionFormat CreateBlankRecord( int iColumnIndex )
    {
      BlankRecord blank = ( BlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Blank );
      blank.ExtendedFormatIndex = m_arrExtFormatIndexes[ iColumnIndex - m_iColumn ];
      blank.Row = Row;
      blank.Column = iColumnIndex;

      return blank;
    }
    /// <summary>
    /// Splits record into subrecords.
    /// </summary>
    /// <param name="bIgnoreStyles">Indicates whether styles must be ignored.</param>
    /// <returns>Array with all subrecords.</returns>
    public BiffRecordRaw[] Split( bool bIgnoreStyles )
    {
      BiffRecordRaw[] arrResult = new BiffRecordRaw[ m_iLastCol - m_iColumn + 1 ];

      for( int iColumnIndex = m_iColumn, i = 0; iColumnIndex <= m_iLastCol; iColumnIndex++, i++ )
      {
        ICellPositionFormat record = CreateBlankRecord( iColumnIndex );
        arrResult[ i ] = ( BiffRecordRaw )record;
      }

      return arrResult;
    }
    #endregion
  }
}
