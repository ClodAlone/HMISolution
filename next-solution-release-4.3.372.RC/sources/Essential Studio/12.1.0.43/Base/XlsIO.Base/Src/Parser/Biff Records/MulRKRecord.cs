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
using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// Represents a cell range containing RK value cells.
  /// All cells are located in the same row.
  /// </summary>
  [ Biff( TBIFFRecord.MulRK ) ]
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class MulRKRecord
    : CellPositionBase
    , IMultiCellRecord
  {
    #region Class constants
    /// <summary>
    /// Size of the fixed part.
    /// </summary>
    public const int DEF_FIXED_SIZE = 6;
    /// <summary>
    /// Size of the subitem.
    /// </summary>
    public const int DEF_SUB_ITEM_SIZE = 6;
    #endregion

    #region Class members
    /// <summary>
    /// List of RkRec structures.
    /// </summary>
    private List<RkRec> m_arrRKs = null;
    /// <summary>
    /// Index to last column.
    /// </summary>
    private int  m_iLastCol = 0;
    #endregion

    #region Class properties
    /// <summary>
    /// Index to the first column.
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
    /// List of RkRec structures.
    /// </summary>
    public List<RkRec> Records
    {
      get
      {
        return m_arrRKs;
      }
      set
      {
        m_arrRKs = value;
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
        return 6;
      }
    }
    #endregion

    #region Internal classes
    /// <summary>
    /// Contains information about single RK entry in MulRKRecord.
    /// </summary>
    [ CLSCompliant( false ) ]
    public class RkRec
    {
      #region Class memebers
      /// <summary>
      /// Index of ExtendedFormat of the RK entry.
      /// </summary>
      private ushort m_usExtFormatIndex;
      /// <summary>
      /// RK number of the RK entry.
      /// </summary>
      private int   m_iRk;
      #endregion

      #region Class constructors
      // TODO: implement class here
      /// <summary>
      /// Default constructor. To prevent user creating class
      /// instance without parameters.
      /// </summary>
      private RkRec()
      {
      }
      /// <summary>
      /// Constructs RkRec and sets its ExtendedFormat and Rk number values.
      /// </summary>
      /// <param name="xf">Value of ExtendedFormat index.</param>
      /// <param name="rk">Value of RkNumber.</param>
      public RkRec( ushort xf, int rk )
      {
        m_usExtFormatIndex = xf;
        m_iRk = rk;
      }
      #endregion

      #region Class properties
      /// <summary>
      /// Index of ExtendedFormatRecord for this Rk number.
      /// </summary>
      public ushort ExtFormatIndex
      {
        get
        {
          return m_usExtFormatIndex;
        }
        set
        {
          m_usExtFormatIndex = value;
        }
      }

      /// <summary>
      /// Read-only. 32-bit value contained by the record.
      /// </summary>
      public int    Rk
      {
        get
        {
          return m_iRk;
        }
        set
        {
          m_iRk = value;
        }
      }

      /// <summary>
      /// Read-only. Returns double value stored as Rk.
      /// </summary>
      public double RkNumber
      {
        get
        {
          bool bIEEEFloat = ( m_iRk & 0x2 ) == 0x2;
          bool bValueNotChanged = ( m_iRk & 0x1 ) == 0x1;

          long lValue = ( m_iRk >> 2 );

          if( bIEEEFloat )
          {
            double dbValue = (double)lValue;
            return ( bValueNotChanged ) ? dbValue / 100.0 : dbValue;
          }
          else
          {
            double dbValue = (double)BitConverterGeneral.Int64BitsToDouble( lValue << 34 );
            return ( bValueNotChanged ) ? dbValue / 100.0 : dbValue;
          }
        }
      }
    }
    #endregion

    #endregion

    #region Class Initialize/Finalize methods

    /// <summary>
    /// Default constructor
    /// </summary>
    public  MulRKRecord()
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
      iOffset -= 2; // we should restore extended format.

      //int iCount = ( Length - 6 ) / 6;
      int iCount = Length - 6;

      if( version != ExcelVersion.Excel97to2003 )
      {
        iCount -= 6; // 2 additional bytes for 3 field.
      }

      iCount /= 6;
      m_arrRKs = new List<RkRec>( iCount );

      if( Length % 6 != 0 )
      {
        throw new WrongBiffRecordDataException();
      }

      int offset = iOffset;

      for( int i = 0; i < iCount; i++, offset += 6 )
      {
        RkRec recordToAdd = new RkRec(
          provider.ReadUInt16( offset ),
          provider.ReadInt32( offset + 2 ) );

        m_arrRKs.Add( recordToAdd );
      }

      if( version == ExcelVersion.Excel97to2003 )
      {
        m_iLastCol = provider.ReadUInt16( offset );
      }
      else
      {
        m_iLastCol = provider.ReadInt32( offset );
      }
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

      iOffset -= 2;// XF index is not needed for this record.

      for( int i = 0, len = m_arrRKs.Count; i < len; i++, iOffset += DEF_SUB_ITEM_SIZE )
      {
        RkRec rkRec = m_arrRKs[ i ];
        provider.WriteUInt16( iOffset, rkRec.ExtFormatIndex );
        provider.WriteInt32( iOffset + 2, rkRec.Rk );
      }

      if( version == ExcelVersion.Excel97to2003 )
      {
        provider.WriteUInt16( iOffset, ( ushort )m_iLastCol );
      }
      else
      {
        provider.WriteInt32( iOffset, m_iLastCol );
      }
    }
    /// <summary>
    /// Returns size of the required storage space.
    /// </summary>
    /// <param name="version">Excel version.</param>
    /// <returns>Size of the required storage space.</returns>
    public override int GetStoreSize( ExcelVersion version )
    {
      int iResult = m_arrRKs.Count * DEF_SUB_ITEM_SIZE + DEF_FIXED_SIZE;

      if( version != ExcelVersion.Excel97to2003 )
        iResult += 6; // 2 additional bytes for last column, first column and row.

      return iResult;
    }
    #endregion

    #region IMultiCellRecord members
    /// <summary>
    /// Returns size of the subrecord if it was placed as separate record (including BiffRecord header). Read-only.
    /// </summary>
    public int GetSeparateSubRecordSize( ExcelVersion version )
    {
      int iResult = RKRecord.DEF_RECORD_SIZE_WITH_HEADER;

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
        return TBIFFRecord.RK;
      }
    }
    /// <summary>
    /// Inserts cell inside this record.
    /// </summary>
    /// <param name="cell">Cell to insert.</param>
    public void Insert( ICellPositionFormat cell )
    {
      if( cell.TypeCode == this.TypeCode )
      {
        MergeRecords( ( MulRKRecord )cell );
      }
      else
      {
        InsertSubRecord( cell );
      }
    }
    /// <summary>
    /// Merges this and specified records.
    /// </summary>
    /// <param name="mulRK">Record to merge with.</param>
    private void MergeRecords( MulRKRecord mulRK )
    {
      if( mulRK == null )
        throw new ArgumentNullException( "mulRK" );

      if( mulRK.Row != m_iRow )
        throw new ArgumentOutOfRangeException( "Row", "Rows should be equal for both MulRK records." );

      if( mulRK.FirstColumn == LastColumn + 1 )
      {
        m_iLastCol = mulRK.LastColumn;
        m_arrRKs.AddRange( mulRK.m_arrRKs );
      }
      else if( mulRK.LastColumn + 1 == FirstColumn )
      {
        m_iColumn = mulRK.m_iColumn;
        m_arrRKs.InsertRange( 0, mulRK.m_arrRKs );
      }
      else
      {
        throw new ArgumentException( "Two MulRK records doesn't correspond each other." );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    public void InsertSubRecord( ICellPositionFormat cell )
    {
      if( cell.TypeCode != TBIFFRecord.RK )
        throw new ArgumentOutOfRangeException( "cell.TypeCode" );

      int iColumnIndex = cell.Column;
      int iRowIndex = cell.Row;
      ushort usXFIndex = cell.ExtendedFormatIndex;

      bool bNull = m_arrRKs == null;

      if( bNull || m_arrRKs.Count == 0 )
      {
        if( bNull )m_arrRKs = new List<RkRec>();

        RkRec rkRec = CreateSubRecord( ( RKRecord )cell );
        m_arrRKs.Add( rkRec );
        m_iRow = cell.Row;
        m_iColumn = m_iLastCol = cell.Column;
      }
      else
      {

        if( Row != iRowIndex )
          throw new ArgumentOutOfRangeException( "Row" );

        if( m_iColumn <= iColumnIndex && m_iLastCol >= iColumnIndex )
        {
          RKRecord rk = ( RKRecord )cell;
          int iIndex = iColumnIndex - m_iColumn;
          RkRec rkRec = m_arrRKs[ iIndex ];
          rkRec.ExtFormatIndex = usXFIndex;
          rkRec.Rk = rk.RKNumberInt;
        }
        else if( iColumnIndex == m_iColumn - 1 )
        {
          RkRec rkRec = CreateSubRecord( ( RKRecord )cell );
          m_arrRKs.Insert( 0, rkRec );
          m_iColumn--;
        }
        else if( iColumnIndex == m_iLastCol + 1 )
        {
          RkRec rkRec = CreateSubRecord( ( RKRecord )cell );
          m_arrRKs.Add( rkRec );
          m_iLastCol++;
        }
        else
        {
          throw new ArgumentOutOfRangeException( "cell.Column" );
        }
      }
    }
    /// <summary>
    /// Creates subrecord corresponding to the specified record.
    /// </summary>
    /// <param name="rk"></param>
    /// <returns></returns>
    private RkRec CreateSubRecord( RKRecord rk )
    {
      if( rk == null )
        throw new ArgumentNullException( "rk" );

      return new RkRec( rk.ExtendedFormatIndex, rk.RKNumberInt );
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

      if( iFirstCol == iLastCol ) return CreateRkRecord( iFirstCol );

      MulRKRecord result = ( MulRKRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MulRK );
      result.m_iColumn = iFirstCol;
      result.m_iLastCol = iLastCol;
      result.m_iRow = m_iRow;

      int iCount = iLastCol - iFirstCol + 1;
      List<RkRec> arrNew = new List<RkRec>( iCount );
      result.m_arrRKs = arrNew;

      for( int i = 0, iOffset = iFirstCol - m_iColumn; i < iCount; i++, iOffset++ )
      {
        arrNew[ i ] = m_arrRKs[ iOffset ];
      }

      return result;
    }
    /// <summary>
    /// Creates blank record with specified column index.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index of the created record.</param>
    /// <returns>Created record.</returns>
    private ICellPositionFormat CreateRkRecord( int iColumnIndex )
    {
      RKRecord rk = ( RKRecord )BiffRecordFactory.GetRecord( TBIFFRecord.RK );
      RkRec rec = m_arrRKs[ iColumnIndex - m_iColumn ];
      rk.ExtendedFormatIndex = rec.ExtFormatIndex;
      rk.RKNumberInt = rec.Rk;
      rk.Row = Row;
      rk.Column = iColumnIndex;

      return rk;
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
        ICellPositionFormat record = CreateRkRecord( iColumnIndex );
        arrResult[ i ] = ( BiffRecordRaw )record;
      }

      return arrResult;
    }
    #endregion
  }
}
