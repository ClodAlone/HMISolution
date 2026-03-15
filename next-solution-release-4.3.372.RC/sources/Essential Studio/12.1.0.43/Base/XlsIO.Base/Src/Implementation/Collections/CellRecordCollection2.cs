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
using System.Collections;
using System.Globalization;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Implementation.Security;
using System.Collections.Generic;

using Syncfusion.XlsIO.Interfaces;
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
#if  SILVERLIGHT
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Summary description for CellRecordCollection.
  /// </summary>
  public class CellRecordCollection
    : CommonObject
    , IDictionary
    //, IEnumerable
  {
    #region Class members
    /// <summary>
    /// Table with cell records.
    /// </summary>
    private RecordTable m_dicRecords;
    /// <summary>
    /// Table with created ranges.
    /// </summary>
    private SFTable m_colRanges;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private IInternalWorksheet m_worksheet;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Indicates whether we should use ranges cache or not.
    /// </summary>
    private bool m_bUseCache;
    /// <summary>
    /// Record extractor to get Biff records from.
    /// </summary>
    private RecordExtractor m_recordExtractor;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public CellRecordCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      m_colRanges = new SFTable( m_book.MaxRowCount, m_book.MaxColumnCount );
      m_dicRecords = new RecordTable( m_book.MaxRowCount, m_worksheet );
      m_recordExtractor = new RecordExtractor();
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( IInternalWorksheet ) ) as IInternalWorksheet;

      if( m_worksheet == null )
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent worksheet" );

      m_book = m_worksheet.ParentWorkbook;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns first used row. Read-only.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_worksheet.FirstRow;
      }
    }
    /// <summary>
    /// Returns last used row. Read-only.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_worksheet.LastRow;
      }
    }
    /// <summary>
    /// Returns first used column. Read-only.
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return m_worksheet.FirstColumn;
      }
    }
    /// <summary>
    /// Returns last used column. Read-only.
    /// </summary>
    public int LastColumn
    {
      get
      {
        return m_worksheet.LastColumn;
      }
    }
    /// <summary>
    /// Represents parent worksheet.
    /// </summary>
    internal IInternalWorksheet Sheet
    {
      get
      {
          if (m_worksheet is ExternWorksheetImpl)
              return (ExternWorksheetImpl)m_worksheet;
        return ( WorksheetImpl )m_worksheet;
      }
    }
    /// <summary>
    /// Table with cell records. Read-only.
    /// </summary>
    public RecordTable Table
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_dicRecords;
      }
      [ DebuggerStepThrough ]
      set
      {
        m_dicRecords = value;
      }
    }
    /// <summary>
    /// Indicates whether we should use ranges cache or not. Default value if false.
    /// </summary>
    public bool UseCache
    {
      get
      {
        return m_bUseCache;
      }
      set
      {
        if( value != m_bUseCache )
        {
          if( value )
          {
            CreateRangesCollection();
          }
          else
          {
            m_colRanges = null;
          }

          m_bUseCache = value;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public ExcelVersion Version
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        //throw new NotImplementedException();
        m_dicRecords.RowCount = m_book.MaxRowCount;

        if( FirstRow >= 0 )
        {
          int iMaxRowIndex = -1;

          for( int i = FirstRow - 1, last = LastRow; i < last; i++ )
          {
            ApplicationImpl application = m_book.AppImplementation;
            RowStorage row = m_dicRecords.GetOrCreateRow( i, application.StandardHeightInRowUnits, false, value );

            if( row != null )
            {
              row.SetVersion( value, AppImplementation.RowStorageAllocationBlockSize );
              iMaxRowIndex = i;
            }
          }

          if( iMaxRowIndex >= 0 )
          {
            m_worksheet.LastRow = iMaxRowIndex + 1;
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public RecordExtractor RecordExtractor
    {
      get
      {
        return m_recordExtractor;
      }
    }
    #endregion

    #region IDictionary properties
    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        throw new NotImplementedException();
        //return m_dicRecords.CellCount;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the IDictionary has a fixed size. Read-only.
    /// </summary>
    public bool IsFixedSize
    {
      get
      {
        return false;
        //return m_dicRecords.IsFixedSize;
      }
    }

    /// <summary>
    ///  Gets a value indicating whether the IDictionary is read-only. Read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get
      {
        return false;
        //return m_dicRecords.IsReadOnly;
      }
    }

    /// <summary>
    /// Gets an ICollection containing the keys of the IDictionary. Read-only.
    /// </summary>
    public ICollection Keys
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Gets an ICollection containing the values in the IDictionary. Read-only.
    /// </summary>
    public ICollection Values
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    public object this[ object key ]
    {
      get
      {
        if( key is long ) return this[ ( long )key ];

        throw new NotSupportedException( "Non Int64 keys are not support" );
      }
      set
      {
        if( key is long )
        {
          this[ ( long )key ] = value as ICellPositionFormat;
        }
        else
        {
          throw new NotSupportedException( "Non Int64 keys are not support" );
        }
      }
    }
    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat this[ long key ]
    {
      get
      {
        int iRow = RangeImpl.GetRowFromCellIndex( key ) - 1;
        int iColumn = RangeImpl.GetColumnFromCellIndex( key ) - 1;
        return m_dicRecords[ iRow, iColumn ] as ICellPositionFormat;
      }
      set
      {
        int iRowIndex = RangeImpl.GetRowFromCellIndex( key );
        int iColumnIndex = RangeImpl.GetColumnFromCellIndex( key );
        this[ iRowIndex, iColumnIndex ] = value;
      }
    }
    /// <summary>
    /// Gets or sets the element with the specified one-based row and column indexes.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat this[ int iRow, int iColumn ]
    {
      get
      {
        return m_dicRecords[ iRow - 1, iColumn - 1 ] as ICellPositionFormat;
      }
      set
      {
        if( value == null )
        {
          Remove( iRow, iColumn );
        }
        else
        {
          m_dicRecords[ iRow - 1, iColumn - 1 ] = value;
          WorksheetHelper.AccessColumn( m_worksheet, iColumn );
          WorksheetHelper.AccessRow( m_worksheet, iRow );
        }
      }
    }
    #endregion

    #region IDictionary methods
    /// <summary>
    /// Removes all elements from the IDictionary.
    /// </summary>
    public void Clear()
    {
      if (m_dicRecords != null)
          m_dicRecords.Clear();
    }
    /// <summary>
    /// Adds an element with the provided key and value to the IDictionary.
    /// </summary>
    /// <param name="key">The Object to use as the key of the element to add.</param>
    /// <param name="value">The Object to use as the value of the element to add. </param>
    public void Add( object key, object value )
    {
      if( key is long )
      {
        Add( ( long )key, value as ICellPositionFormat );
      }
    }
    /// <summary>
    /// Returns an IDictionaryEnumerator for the IDictionary.
    /// </summary>
    /// <returns>An IDictionaryEnumerator for the IDictionary.</returns>
    public IDictionaryEnumerator GetEnumerator()
    {
      return new RecordTableEnumerator( /*m_dicRecords*/ this );
    }
    /// <summary>
    /// Removes the element with the specified key from the IDictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    public void Remove( object key )
    {
      if( key is long )
      {
        Remove( ( long )key );
      }
    }
    /// <summary>
    /// Determines whether the IDictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the IDictionary.</param>
    /// <returns>True if the IDictionary contains an element with the key; otherwise, False.</returns>
    public bool Contains( object key )
    {
      return Contains( ( long )key );
    }
    #endregion

    #region IEnumerable methods
    /// <summary>
    /// Returns an IDictionaryEnumerator for the IDictionary.
    /// </summary>
    /// <returns>An IDictionaryEnumerator for the IDictionary.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new RecordTableEnumerator( /*m_dicRecords*/ this );
    }
    #endregion

    #region IDictionary typed methods
    /// <summary>
    /// Adds an element with the provided key and value to the IDictionary.
    /// </summary>
    /// <param name="key">The Object to use as the key of the element to add.</param>
    /// <param name="value">The Object to use as the value of the element to add. </param>
    [ CLSCompliant( false ) ]
    public void Add( long key, ICellPositionFormat value )
    {
      Add( value );
    }
    ///// <summary>
    ///// Adds an element with the provided key and value to the IDictionary.
    ///// </summary>
    ///// <param name="rowIndex">Zero based row index of the item to set.</param>
    ///// <param name="columnIndex">Zero based column index of the item to set.</param>
    ///// <param name="value">The Object to use as the value of the element to add. </param>
    //[CLSCompliant( false )]
    //public void Add( int rowIndex, int columnIndex, ICellPositionFormat value )
    //{
    //  if( m_dicRecords.Contains( rowIndex, columnIndex ) )
    //    throw new ArgumentOutOfRangeException( "Collection already contains such member." );

    //  SetCellRecord( rowIndex + 1, columnIndex + 1, value );
    //}
    /// <summary>
    /// Adds an specified cell to the IDictionary.
    /// </summary>
    /// <param name="value">The Object to use as the value of the element to add. </param>
    [CLSCompliant( false )]
    public void Add( ICellPositionFormat value )
    {
      int iRowIndex = value.Row;
      int iColumnIndex = value.Column;

      if( m_dicRecords.Contains( iRowIndex, iColumnIndex ) )
        throw new ArgumentOutOfRangeException( "Collection already contains such member." );

      //m_dicRecords[ iRowIndex, iColumnIndex ] = value;
      SetCellRecord( iRowIndex + 1, iColumnIndex + 1, value );
    }

    /// <summary>
    /// Removes the element with the specified key from the IDictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    public void Remove( long key )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( key ) - 1;
      int iColumn = RangeImpl.GetColumnFromCellIndex( key ) - 1;

//      if( m_arrRows[ iRow ] > 0 )
//      {
//        if( m_dicRecords.Contains( iRow, iColumn ) )
//        {
          m_dicRecords[ iRow, iColumn ] = null;
//          m_arrRows[ iRow ]--;
//        }
//      }
    }

    /// <summary>
    /// Removes the element with the specified key from the IDictionary.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to remove.</param>
    /// <param name="iColumn">One-based column index of the cell to remove.</param>
    public void Remove( int iRow, int iColumn )
    {
      m_dicRecords[ iRow - 1, iColumn - 1 ] = null;
    }

    /// <summary>
    /// Determines whether collection contains row.
    /// </summary>
    /// <param name="iRowIndex">Zero-based row index.</param>
    /// <returns>True if the collection contains at least one element with specified row index; otherwise, False.</returns>
    public bool ContainsRow( int iRowIndex )
    {
      return m_dicRecords.ContainsRow( iRowIndex );
    }

    /// <summary>
    /// Determines whether the IDictionary contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the IDictionary.</param>
    /// <returns>True if the IDictionary contains an element with the key; otherwise, False.</returns>
    public bool Contains( long key )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( key ) - 1;
      int iColumn = RangeImpl.GetColumnFromCellIndex( key ) - 1;
      return m_dicRecords.Contains( iRow, iColumn );
    }

    /// <summary>
    /// Determines whether the IDictionary contains an element with the specified key.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to locate.</param>
    /// <param name="iColumn">One-based column index of the cell to locate.</param>
    /// <returns>True if the IDictionary contains an element with the key; otherwise, False.</returns>
    public bool Contains( int iRow, int iColumn )
    {
      return m_dicRecords.Contains( iRow - 1, iColumn - 1 );
    }

    #endregion

    #region ICollection Members
    /// <summary>
    /// Gets a value indicating whether access to the ICollection is synchronized. Read-only.
    /// </summary>
    public bool IsSynchronized
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Gets an object that can be used to synchronize access to the ICollection. Read-only.
    /// </summary>
    public object SyncRoot
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    /// <summary>
    /// Copies the elements of the ICollection to an Array, starting at a particular Array index.
    /// </summary>
    /// <param name="array">
    /// The one-dimensional Array that is the destination of the elements copied
    /// from ICollection. The Array must have zero-based indexing.
    /// </param>
    /// <param name="index">The zero-based index in array at which copying begins.</param>
    public void CopyTo( Array array, int index )
    {
      throw new NotSupportedException();
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Saves all rows into specified OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all rows.</param>
    /// <param name="arrDBCells">List into which all DBCell record must be placed.</param>
    /// <returns>Number of DBCells.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When records is null.
    /// </exception>
    [ CLSCompliant( false ) ]
    public int Serialize( OffsetArrayList records, List<DBCellRecord> arrDBCells )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ExcelVersion version = ExcelVersion.Excel97to2003;

      int iDBCount = 0;
      if( FirstRow < 0 ) return iDBCount;

      int iFirstRow = FirstRow;
      int iLastRow = LastRow;
      int iFirstColumn = FirstColumn;
      int iLastColumn = LastColumn;
      //IDictionary rowInfo = m_worksheet.RowInformation;
      List<RowStorage> ranges = new List<RowStorage>( 32 );

      for( int i = iFirstRow; i <= iLastRow; i++ )
      {
        int iRowRecSize = 0;
        int iFirstRowOffset = 0;

        int iStartRow = i;

        i = PrepareNextRowsBlock( records, ranges, i, ref iRowRecSize,
          ref iFirstRowOffset, iLastRow, iFirstColumn, iLastColumn, ExcelVersion.Excel97to2003 );

        // save cells data into records array
        DBCellRecord dbcell = ( DBCellRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DBCell );

        int iRangesCount = ranges.Count;
        dbcell.CellOffsets = new ushort[ iRangesCount ];

        arrDBCells.Add( dbcell );

        if( iRangesCount > 0 )
        {
          dbcell.CellOffsets[ 0 ] = ( ushort )iRowRecSize;

          for( int j = 0, lenJ = iRangesCount - 1; j < lenJ; j++ )
          {
            //ICellPositionFormat[] arrCells = ( ICellPositionFormat[] )ranges[ j ];
            RowStorage arrCells = ranges[ j ];
            int iStoreSize = ( arrCells != null )
              ? arrCells.GetStoreSize( version )
              : 0;
            
            if( iStoreSize != 0 ) iStoreSize += BiffRecordRaw.DEF_HEADER_SIZE;

            dbcell.CellOffsets[ j + 1 ] = ( ushort )iStoreSize;
            //iPos = 0;

            if( arrCells != null && iStoreSize > 0 ) records.Add( arrCells );

            iFirstRowOffset += iStoreSize;
          }

          dbcell.RowOffset = iFirstRowOffset;

          RowStorage arrLastRow = ( RowStorage )ranges[ iRangesCount - 1 ];

          if( arrLastRow != null && arrLastRow.GetStoreSize( version ) > 0 )
          {
            records.Add( arrLastRow );
            dbcell.RowOffset += arrLastRow.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;
          }
        }

        records.Add( dbcell );
        iDBCount++;
        ranges.Clear();
      }

      return iDBCount;
    }

    /// <summary>
    /// Prepares next block of rows.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all rows.</param>
    /// <param name="ranges">List to add serialization data into.</param>
    /// <param name="i">Start row index.</param>
    /// <param name="iRowRecSize">Size of the records.</param>
    /// <param name="iFirstRowOffset">Offset to the first row.</param>
    /// <param name="iLastRow">One-based last row index to serialize.</param>
    /// <param name="iFirstSheetCol">One-based first column index in the worksheet.</param>
    /// <param name="iLastSheetCol">One-based last column index in the worksheet.</param>
    /// <param name="version">Destination excel version.</param>
    /// <returns>Row index after block saving.</returns>
    private int PrepareNextRowsBlock( OffsetArrayList records, List<RowStorage> ranges,
      int i, ref int iRowRecSize, ref int iFirstRowOffset, int iLastRow,
      int iFirstSheetCol, int iLastSheetCol, ExcelVersion version )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( ranges == null )
        throw new ArgumentNullException( "ranges" );

      int iFirstCol, iLastCol;
      RowRecord row;

      int iMaxFor = 0;
      int iStartRow = i;
      //IDictionary rowInfo = m_worksheet.RowInformation;
      int iDefRowHeight = m_worksheet.DefaultRowHeight;

      for( ; i <= iLastRow; i++ )
      {
        RowStorage arrData = GetRowData( i, iFirstSheetCol, iLastSheetCol,
          out iFirstCol, out iLastCol, version );
        //row = rowInfo[ i ] as RowRecord;
        if( arrData != null )
        {
          row = arrData.CreateRowRecord( m_book );
          row.Worksheet = (m_worksheet as WorksheetImpl);
          row.RowNumber = ( ushort )( i - 1 );
        }
        else
        {
          row = null;
        }
        //row = ( arrData != null ) ? arrData.CreateRowRecord() : null;


        if( arrData != null && arrData.UsedSize > 0 )
        {
          ranges.Add( arrData );

          if( row == null )
          {
            row = ( RowRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Row );
            row.RowNumber = ( ushort )( i - 1 );
            row.Height = ( ushort )iDefRowHeight;
            row.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
          }

//          row.FirstColumn = ( ushort )( iFirstCol - 1 );
//          row.LastColumn = ( ushort )iLastCol;
          row.FirstColumn = ( ushort )arrData.FirstColumn;
          row.LastColumn = ( ushort )arrData.LastColumn;

          int iMaxSize = row.MaximumRecordSize + BiffRecordRaw.DEF_HEADER_SIZE;

          if( i != iStartRow ) iRowRecSize += iMaxSize;

          iFirstRowOffset += iMaxSize;
          records.Add( row );
        }
        else if( row != null )
        {
          records.Add( row );
          ranges.Add( arrData );

          int iSize = row.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;
          iFirstRowOffset += iSize;

          if( i != iStartRow ) iRowRecSize += iSize;
        }
        else if( i == iStartRow )
        {
          iStartRow++;
        }

        iMaxFor++;

        if( iMaxFor == 32 ) break;
      }

      return i;
    }
    /// <summary>
    /// Returns array of all cells in the specified row.
    /// </summary>
    /// <param name="index">One-based index of row.</param>
    /// <param name="iFirstColumn">One-based first column in the parent worksheet.</param>
    /// <param name="iLastColumn">One-based last column in the parent worksheet.</param>
    /// <param name="min">Variable that receives first used column of the row.</param>
    /// <param name="max">Variable that receives last used column of the row.</param>
    /// <param name="version">Destination excel version.</param>
    /// <returns>Array of all cells in the specified row.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When values is null.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected RowStorage GetRowData( int index, int iFirstColumn,
      int iLastColumn, out int min, out int max, ExcelVersion version )
    {
      RowStorage arrRow = m_dicRecords.Rows[ index - 1 ];

      min = int.MinValue;
      max  = int.MaxValue;

      if( arrRow != null )
      {
        if( version != arrRow.Version )
        {
          arrRow = ( RowStorage )arrRow.Clone( IntPtr.Zero );
          arrRow.SetVersion( version, AppImplementation.RowStorageAllocationBlockSize );
        }

        min = arrRow.FirstColumn + 1;
        max = arrRow.LastColumn + 1;
      }

      return arrRow;
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Extracts ranges from the reader.
    /// </summary>
    /// <param name="reader">Reader to extract ranges from.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    /// <param name="hashNewXFIndexes">Dictionary with new XF indexes when bIgnoreStyles is set to true.</param>
    /// <param name="decryptor">Decryptor used to decrypt data if necessary.</param>
    [ CLSCompliant( false ) ]
    public void ExtractRanges( BiffReader reader, bool bIgnoreStyles, Dictionary<int, int> hashNewXFIndexes,
      IDecryptor decryptor )
    {
      // TODO: add usage of hashNewXFIndexes
      m_dicRecords.ExtractRanges( reader, bIgnoreStyles, m_book.InnerSST, ( WorksheetImpl )m_worksheet, decryptor );
    }
    /// <summary>
    /// Extracts ranges from the reader.
    /// </summary>
    /// <param name="index">IndexRecord that contains all required information.</param>
    /// <param name="reader">Reader to extract ranges from.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    /// <param name="hashNewXFIndexes">Dictionary with new XF indexes when bIgnoreStyles is set to true.</param>
    /// <returns>True if parsing succeeded.</returns>
    [ CLSCompliant( false ) ]
    public bool ExtractRangesFast( IndexRecord index, BiffReader reader,
      bool bIgnoreStyles, Dictionary<int, int> hashNewXFIndexes )
    {
      // TODO: add usage of hashNewXFIndexes
      return m_dicRecords.ExtractRangesFast( index, reader, bIgnoreStyles, m_book.InnerSST, ( WorksheetImpl )m_worksheet );
    }
    /// <summary>
    /// Adds record to the collection.
    /// </summary>
    /// <param name="recordToAdd">Record to add.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    [ CLSCompliant( false ) ]
    public void AddRecord( BiffRecordRaw recordToAdd, bool bIgnoreStyles )
    {
      if( recordToAdd == null )
        throw new ArgumentNullException( "recordToAdd" );

      switch( recordToAdd.TypeCode )
      {
        case TBIFFRecord.MulBlank:
          AddRecord( ( MulBlankRecord )recordToAdd, bIgnoreStyles );
          break;

        case TBIFFRecord.MulRK:
          AddRecord( ( MulRKRecord )recordToAdd, bIgnoreStyles );
          break;

        default:
          AddRecord( ( ICellPositionFormat )recordToAdd, bIgnoreStyles );
          break;
      }
    }
    /// <summary>
    /// Adds record to the collection.
    /// </summary>
    /// <param name="cell">Record to add.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    [ CLSCompliant( false ) ]
    public void AddRecord( ICellPositionFormat cell, bool bIgnoreStyles )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );

      if( bIgnoreStyles )
      {
        cell.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
      }

      SetCellRecord( cell.Row + 1, cell.Column + 1, cell );

//      m_worksheet.AccessRow( cell.Row + 1 );
//      m_worksheet.AccessColumn( cell.Column + 1 );
    }
    /// <summary>
    /// Adds MulRKRecord to the records collection.
    /// </summary>
    /// <param name="mulRK">Record to add.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    private void AddRecord( MulRKRecord mulRK, bool bIgnoreStyles )
    {
      if( mulRK == null )
        throw new ArgumentNullException( "mulRK" );

      List<MulRKRecord.RkRec> arrSubRecords = mulRK.Records;
      int iRow = mulRK.Row;

      for( int j = mulRK.FirstColumn, iCount = 0, iLast = mulRK.LastColumn;
        j <= iLast; j++, iCount++ )
      {
        RKRecord rk = ( RKRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.RK );

        rk.SetRKRecord( arrSubRecords[ iCount ] );
        rk.Row = iRow;
        rk.Column = j;

        AddRecord( ( ICellPositionFormat )rk, bIgnoreStyles );
      }
    }
    /// <summary>
    /// Adds MulBlankRecord to the records collection.
    /// </summary>
    /// <param name="mulBlank">Record to add.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    private void AddRecord( MulBlankRecord mulBlank, bool bIgnoreStyles )
    {
      if( mulBlank == null )
        throw new ArgumentNullException( "mulBlank" );

      if( !bIgnoreStyles )
      {
        for( int j = mulBlank.FirstColumn, iLast = mulBlank.LastColumn; j <= iLast; j++ )
        {
          AddRecord( ( ICellPositionFormat )mulBlank.GetBlankRecord( j ), bIgnoreStyles );
        }
      }
    }
    /// <summary>
    /// Adds formula record and its string value.
    /// </summary>
    /// <param name="formula">Formula record to add.</param>
    /// <param name="stringRecord">String value to add.</param>
    /// <param name="bIgnoreStyles">Indicates whether styles should be ignored.</param>
    private void AddRecord( FormulaRecord formula, StringRecord stringRecord, bool bIgnoreStyles )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      //int iRow = formula.Row + 1;
      //int iColumn = formula.Column + 1;
      //long lCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );

      AddRecord( ( ICellPositionFormat )formula, bIgnoreStyles );
    }
    /// <summary>
    /// Indicates whether specified formula is complex
    ///  and requires range object to be created for it.
    /// </summary>
    /// <param name="formula">FormulaRecord to check.</param>
    /// <returns>
    /// True if specified formula is complex and requires
    ///  range object to be created for it.
    /// </returns>
    internal bool IsRequireRange( FormulaRecord formula )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      Ptg[] arrPtg = formula.ParsedExpression;

      for( int i = 0, len = arrPtg.Length; i < len; i++ )
      {
        FormulaToken tokenCode = arrPtg[ i ].TokenCode;

        if( FormulaUtil.IndexOf( FormulaUtil.NameCodes, tokenCode ) != -1
          || FormulaUtil.IndexOf( FormulaUtil.NameXCodes, tokenCode ) != -1 )
        {
          return true;
        }
      }

      return false;
    }

    #endregion

    #region Class methods
    internal void UpdateRows(int rowCount)
    {
        m_dicRecords.UpdateRows(rowCount);
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public CellRecordCollection Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      CellRecordCollection result = ( CellRecordCollection )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_dicRecords = ( RecordTable )m_dicRecords.Clone( result.m_worksheet );
      result.m_colRanges = new SFTable( m_book.MaxRowCount, m_book.MaxColumnCount );

      return result;
    }
    /// <summary>
    /// Sets cell range.
    /// </summary>
    /// <param name="key">Cell key.</param>
    /// <param name="range">Range to set.</param>
    [ CLSCompliant( false ) ]
    public void SetRange( long key, RangeImpl range )
    {
      if( m_bUseCache )
      {
        int iRow = RangeImpl.GetRowFromCellIndex( key );
        int iColumn = RangeImpl.GetColumnFromCellIndex( key );
        SetRange( iRow, iColumn, range );
      }
    }
    /// <summary>
    /// Sets cell range.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="range">Range to set.</param>
    [ CLSCompliant( false ) ]
    public void SetRange( int iRow, int iColumn, RangeImpl range )
    {
      if( m_bUseCache )
      {
        m_colRanges[ iRow - 1, iColumn - 1 ] = range;
      }
    }

    /// <summary>
    /// Returns cell range.
    /// </summary>
    /// <param name="key">Cell key.</param>
    /// <returns>Corresponding cell range.</returns>
    public RangeImpl GetRange( long key )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( key );
      int iColumn = RangeImpl.GetColumnFromCellIndex( key );
      return GetRange( iRow, iColumn );
    }
    /// <summary>
    /// Returns cell range.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Corresponding cell range.</returns>
    public RangeImpl GetRange( int iRow, int iColumn )
    {
      RangeImpl result = null;

      if( m_bUseCache )
      {
        result = m_colRanges[ iRow - 1, iColumn - 1 ] as RangeImpl;
      }

      return result;
    }
    /// <summary>
    /// Sets cell record.
    /// </summary>
    /// <param name="key">Cell key.</param>
    /// <param name="cell">Cell to set.</param>
    [ CLSCompliant( false ) ]
    public void SetCellRecord( long key, ICellPositionFormat cell )
    {
      this[ key ] = cell;
    }
    /// <summary>
    /// Sets cell record.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="cell">Cell to set.</param>
    [ CLSCompliant( false ) ]
    public void SetCellRecord( int iRow, int iColumn, ICellPositionFormat cell )
    {
      this[ iRow, iColumn ] = cell;
    }
    /// <summary>
    /// Returns cell record.
    /// </summary>
    /// <param name="key">Cell key.</param>
    /// <returns>Corresponding cell record.</returns>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat GetCellRecord( long key )
    {
      return this[ key ];
    }
    /// <summary>
    /// Returns cell record.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <returns>Corresponding cell record.</returns>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat GetCellRecord( int iRow, int iColumn )
    {
      return this[ iRow, iColumn ];
    }
    /// <summary>
    /// Clears range in the dictionary that corresponds to the specified range.
    /// </summary>
    /// <param name="rect">Rectangle to clear.</param>
    public void ClearRange( Rectangle rect )
    {
      int iFirstRow = rect.Top;// + 1;
      int iFirstColumn = rect.Left;// + 1;

      int iLastRow = rect.Bottom;// + 1;
      int iLastColumn = rect.Right;// + 1;

      // NOTE: this code segment can be optimized.
      int iBlockSize = m_book.Application.RowStorageAllocationBlockSize;
      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        RowStorage row = m_dicRecords.Rows[ iRow ];

        if( row != null )
          row.Remove( iFirstColumn, iLastColumn, iBlockSize );
//        int iMaxCount = m_arrRows[ iRow - 1 ];
//
//        if( iMaxCount == 0 ) continue;
//
//        for( int iColumn = iFirstColumn; iColumn <= iLastColumn; iColumn++ )
//        {
//          int iCellIndex = RangeImpl.GetCellIndex( iColumn, iRow );
//          Remove( iCellIndex );
//        }
      }
    }
    /// <summary>
    /// Copies cells from another worksheet.
    /// </summary>
    /// <param name="sourceCells">Source cells collection to copy cells from.</param>
    /// <param name="hashStyleNames">
    /// Dictionary with changes in style indexes,
    /// key - old style index,
    /// value - new style index.
    /// </param>
    /// <param name="hashWorksheetNames">Dictionary with changes in worksheet names.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="dicNewNameIndexes">Dictionary with new name indexes.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    public void CopyCells( CellRecordCollection sourceCells, Dictionary<string, string> hashStyleNames,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> hashExtFormatIndexes,
      Dictionary<int, int> dicNewNameIndexes, Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dictExternSheet )
    {
      //CellRecordCollection dicSourceCells = sourceCells.m_dicRecords;
      //      IDictionary dicSourceStrings = basedOn.m_dicStringCells;
      WorkbookImpl sourceBook = sourceCells.m_book;
      SSTDictionary sourceSST = sourceBook.InnerSST;
      SSTDictionary destSST = m_book.InnerSST;

      Clear();
      m_dicRecords.CopyCells( sourceCells.m_dicRecords, sourceSST, destSST,
        hashExtFormatIndexes, hashWorksheetNames, dicNewNameIndexes, dicFontIndexes, dictExternSheet );
    }

    /// <summary>
    /// Returns string value associated with specified cell index.
    /// </summary>
    /// <param name="cellIndex">Index to cell.</param>
    /// <param name="bAutofitRows">Indicates whether we are interested in correct height or width.</param>
    /// <returns>String value associated with specified cell index.</returns>
    public RichTextString GetRTFString( long cellIndex, bool bAutofitRows )
    {
      ICellPositionFormat cell = GetCellRecord( cellIndex );

      if( cell == null ) return null;

      switch( cell.TypeCode )
      {
        case TBIFFRecord.LabelSST:
          return GetLabelSSTRTFString( cellIndex, bAutofitRows );

        case TBIFFRecord.Formula:
        case TBIFFRecord.RK:
        case TBIFFRecord.Number:
        {
          TextWithFormat text = new TextWithFormat();

          string strText = GetFormulaStringValue( cellIndex );

          if( strText != null )
          {
            text.Text = strText;
          }
          else
          {
            double dValue = GetNumber( cellIndex );

            if( cell.TypeCode == TBIFFRecord.Formula && Double.IsNaN( dValue ) )
            {
              text.Text = GetFormulaErrorBoolText( cell as FormulaRecord );
            }
            else
            {
              FormatImpl format = GetFormat( cellIndex );
              text.Text = format.ApplyFormat( dValue, true );
            }
          }

          return new RangeRichTextString( Application, /*this*/m_worksheet, cellIndex, text );
        }

        case TBIFFRecord.BoolErr:
        {
          TextWithFormat text = new TextWithFormat();
          text.Text = RangeImpl.ParseBoolError( ( BoolErrRecord )cell );

          return new RangeRichTextString( Application, m_worksheet, cellIndex, text );
        }

        default:
          return null;
      }
    }
    /// <summary>
    /// Returns string value associated with specified cell index.
    /// </summary>
    /// <param name="cellIndex">Index to cell.</param>
    /// <param name="bAutofitRows">Indicates whether we are interested in correct height or width.</param>
    /// <param name="richText">String object to fill.</param>
    /// <returns>String value associated with specified cell index.</returns>
    public void FillRTFString( long cellIndex, bool bAutofitRows, RichTextString richText )
    {
      richText.ClearFormatting();
      richText.Text = string.Empty;

      ICellPositionFormat cell = GetCellRecord( cellIndex );

      if( cell == null )
        return;

      switch( cell.TypeCode )
      {
        case TBIFFRecord.LabelSST:
          LabelSSTRecord label = ( LabelSSTRecord )cell;
          FillLabelSSTRTFString( label, bAutofitRows, richText );
          break;

        case TBIFFRecord.Formula:
        case TBIFFRecord.RK:
        case TBIFFRecord.Number:
          {
            //TextWithFormat text = new TextWithFormat();

            string strText = GetFormulaStringValue( cellIndex );
            int iXFIndex = cell.ExtendedFormatIndex;
            ExtendedFormatImpl xFormat = m_book.InnerExtFormats[ iXFIndex ];
            richText.DefaultFontIndex = xFormat.FontIndex;

            if( strText == null )
            {
              double dValue = GetNumber( cellIndex );

              if( cell.TypeCode == TBIFFRecord.Formula && Double.IsNaN( dValue ) )
              {
                strText = GetFormulaErrorBoolText( cell as FormulaRecord );
              }
              else
              {
                if( !Double.IsNaN( dValue ) )
                {
                  FormatImpl format = xFormat.NumberFormatObject;//GetFormat( cellIndex );
                  strText = format.ApplyFormat( dValue, true );
                }
                else
                {
                  strText = string.Empty;
                }
              }
            }

            richText.ClearFormatting();
            richText.Text = strText;
          }
          break;

        case TBIFFRecord.BoolErr:
          richText.ClearFormatting();
          richText.Text = RangeImpl.ParseBoolError( ( BoolErrRecord )cell );
          break;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="bAutofitRows">Indicates whether we are interested in correct height or width.</param>
    /// <returns></returns>
    public RichTextString GetLabelSSTRTFString( long cellIndex, bool bAutofitRows )
    {
      RangeRichTextString result = ( ( WorksheetImpl )Sheet ).CreateLabelSSTRTFString( cellIndex );

      FormatImpl format = GetFormat( cellIndex );
      string strText = format.ApplyFormat( result.Text, true );

      if( strText == result.Text || bAutofitRows ) return result;

      // In this situation we have to create new rich text string in order to autofit correctly.
      IFont font = GetFont( cellIndex );
      RichTextString rtf = new RichTextString( Application, /*this*/m_book, false, true );
      rtf.Text = strText;
      rtf.SetFont( 0, strText.Length - 1, font );

      return rtf;
    }
    /// <summary>
    /// Fills rich text string object with string data.
    /// </summary>
    /// <param name="labelSST">Record to get data from.</param>
    /// <param name="bAutofitRows">Indicates whether we are interested in correct height or width.</param>
    /// <param name="richText">String to fill.</param>
    [ CLSCompliant( false ) ]
    public void FillLabelSSTRTFString( LabelSSTRecord labelSST, bool bAutofitRows,
      RichTextString richText )
    {
      FillRichText( richText, labelSST.SSTIndex );

      int iXFIndex = labelSST.ExtendedFormatIndex;
      ExtendedFormatImpl extFormat = m_book.InnerExtFormats[ iXFIndex ];
      int iFormatIndex = extFormat.NumberFormatIndex;
      FormatImpl format = m_book.InnerFormats[ iFormatIndex ];
      string strText = format.ApplyFormat( richText.Text, true );

      if( !( strText == richText.Text || bAutofitRows ) )
      {
        // In this situation we have to create new rich text string in order to autofit correctly.
        IFont font = extFormat.Font;//GetFont( cellIndex );
        richText.Text = strText;
        richText.SetFont( 0, strText.Length - 1, font );
      }
      else
      {
        richText.DefaultFontIndex = extFormat.FontIndex;
      }
    }
    /// <summary>
    /// Returns text value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Text value if appropriate record was found; otherwise - empty string.</returns>
    public string GetText( long cellIndex )
    {
      ICellPositionFormat record = GetCellRecord( cellIndex );

      if( record != null )
      {
        if( record.TypeCode == TBIFFRecord.Label )
          return ( ( LabelRecord )record ).Label;

        if( record.TypeCode == TBIFFRecord.LabelSST )
        {
          LabelSSTRecord labelSSTRecord = ( LabelSSTRecord )record;
          int iSSTIndex = labelSSTRecord.SSTIndex;
          TextWithFormat textWithFormat = ( TextWithFormat )m_book.InnerSST[ iSSTIndex ];
          return textWithFormat.Text;
        }
      }

      return null;
    }
    /// <summary>
    /// Gets error value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns string that represents error value.</returns>
    public string GetError( long cellIndex )
    {
      BoolErrRecord error = GetCellRecord( cellIndex ) as BoolErrRecord;

      if( error != null && error.IsErrorCode )
      {
        string result = ErrorPtg.DEF_ERROR_NAME;
        int iErrorCode = error.BoolOrError;

        if( FormulaUtil.ErrorCodeToName.ContainsKey( iErrorCode ) )
        {
          result = ( string )FormulaUtil.ErrorCodeToName[ iErrorCode ];
        }

        return result;
      }

      return null;
    }
    /// <summary>
    /// Gets bool value by cellindex.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="value">Returns value by cell index.</param>
    /// <returns>If true than value is correct; otherwise incorrect.</returns>
    public bool GetBool( long cellIndex, out bool value )
    {
      BoolErrRecord error = GetCellRecord( cellIndex ) as BoolErrRecord;
      value = false;

      if( error != null && !error.IsErrorCode )
      {
        value = ( error.BoolOrError > 0 );

        return true;
      }

      return false;
    }
    /// <summary>
    /// Indicates if collection by index contain number.
    /// </summary>
    /// <param name="cellIndex">Index of record.</param>
    /// <returns>Returns true if contain number; otherwise - false.</returns>
    public bool ContainNumber( long cellIndex )
    {
      IDoubleValue record = GetCellRecord( cellIndex ) as IDoubleValue;

      return ( record != null );
    }
    /// <summary>
    /// Indicates if collection by index contain bool or error.
    /// </summary>
    /// <param name="cellIndex">Index of record.</param>
    /// <returns>Returns true if contain bool or error; otherwise - false.</returns>
    public bool ContainBoolOrError( long cellIndex )
    {
      BoolErrRecord error = GetCellRecord( cellIndex ) as BoolErrRecord;

      return ( error != null );
    }
    /// <summary>
    /// Indicates if collection by index contain number as formula value.
    /// </summary>
    /// <param name="cellIndex">Index of record.</param>
    /// <returns>Returns true if contain number; otherwise - false.</returns>
    public bool ContainFormulaNumber( long cellIndex )
    {
      FormulaRecord formula = GetCellRecord( cellIndex ) as FormulaRecord;

      return formula != null && !formula.IsBool && !formula.IsError;
    }
    /// <summary>
    /// Indicates if collection by index contain bool or error as formula value.
    /// </summary>
    /// <param name="cellIndex">Index of record.</param>
    /// <returns>Returns true if contain number; otherwise - false.</returns>
    public bool ContainFormulaBoolOrError( long cellIndex )
    {
      FormulaRecord formula = GetCellRecord( cellIndex ) as FormulaRecord;

      return formula != null && ( formula.IsBool || formula.IsError );
    }
    /// <summary>
    /// Returns cell value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Cell value if appropriate record was found; otherwise returns double.MinValue.</returns>
    public double GetNumber( long cellIndex )
    {
      IDoubleValue record = GetCellRecord( cellIndex ) as IDoubleValue;

      return ( record != null ) ? record.DoubleValue : Double.MinValue;
    }
    /// <summary>
    /// Returns cell value by cell index without formula value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Cell value if appropriate record was found; otherwise returns double.MinValue.</returns>
    public double GetNumberWithoutFormula( long cellIndex )
    {
      IDoubleValue record = GetCellRecord( cellIndex ) as IDoubleValue;

      return ( record != null && record.TypeCode != TBIFFRecord.Formula )
        ? record.DoubleValue : Double.MinValue;
    }
    /// <summary>
    /// Returns formula value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Formula value if appropriate record was found; otherwise double.MinValue.</returns>
    public double GetFormulaNumberValue( long cellIndex )
    {
      FormulaRecord formula = GetCellRecord( cellIndex ) as FormulaRecord;

      return ( formula == null ) ? double.MinValue : formula.Value;
    }
    /// <summary>
    /// Sets string formula value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="strValue">String value to set.</param>
    public void SetStringValue( long cellIndex, string strValue )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex ) - 1;
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex ) - 1;

      RowStorage row = m_dicRecords.Rows[ iRow ];

      if( row == null )
        throw new NotSupportedException( "This property is only for formula ranges." );

      row.SetFormulaStringValue( iColumn, strValue, Application.RowStorageAllocationBlockSize );
    }
    /// <summary>
    /// Returns string formula value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>String formula value.</returns>
    public string GetFormulaStringValue( long cellIndex )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex ) - 1;
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex ) - 1;
      RowStorage storage = m_dicRecords.Rows[ iRow ];
      return storage.GetFormulaStringValue( iColumn );
    }
    /// <summary>
    /// Gets date time by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns date time or null.</returns>
    public DateTime GetDateTime( long cellIndex )
    {
      double number = GetNumberWithoutFormula( cellIndex );

      if( number == double.MinValue )
        return DateTime.MinValue;

      FormatImpl format = GetFormat( cellIndex );

      switch( format.GetFormatType( number ) )
      {
        case ExcelFormatType.DateTime:
          return 
#if ( WINRT )
              DateTimeExtension.FromOADate(number);
#else
              DateTime.FromOADate( number );
#endif


        default:
          return DateTime.MinValue;
      }
    }
    /// <summary>
    /// Copies cell into another worksheet.
    /// </summary>
    /// <param name="cell">Cell to copy.</param>
    /// <param name="strFormulaValue">Formula string value of the cell.</param>
    /// <param name="dicXFIndexes">
    /// Dictionary with updated extended format indexes,
    /// or Null if indexes were not updated.
    /// </param>
    /// <param name="lNewIndex">New cell index</param>
    /// <param name="book">Source workbook.</param>
    /// <param name="dicFontIndexes">
    /// Dictionary with updated font indexes or Null if indexes were not updated.
    /// </param>
    /// <param name="options">Copy options.</param>
    [ CLSCompliant( false ) ]
    public bool CopyCell( ICellPositionFormat cell, string strFormulaValue,
      IDictionary dicXFIndexes, long lNewIndex, WorkbookImpl book,
      Dictionary<int, int> dicFontIndexes, ExcelCopyRangeOptions options )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );

      bool bCreateRange = false;

      int iRow = RangeImpl.GetRowFromCellIndex( lNewIndex ) - 1;
      int iColumn = RangeImpl.GetColumnFromCellIndex( lNewIndex ) - 1;

      ICloneable toClone = ( ICloneable )cell;
      ICellPositionFormat cellNew = ( ICellPositionFormat )toClone.Clone();

      if( cellNew.TypeCode == TBIFFRecord.LabelSST )
      {
        LabelSSTRecord label = ( LabelSSTRecord )cellNew;
        int iLabelIndex = label.SSTIndex;

        SSTDictionary localSST = m_book.InnerSST;
        SSTDictionary sourceSST = book.InnerSST;

        iLabelIndex = localSST.AddCopy( iLabelIndex, sourceSST, dicFontIndexes );
        label.SSTIndex = iLabelIndex;
      }
      else if( cellNew.TypeCode == TBIFFRecord.Formula )
      {
        bCreateRange = true;
        FormulaRecord formulaDest = ( FormulaRecord )cellNew;
        
        if( Sheet.IsArrayFormula( lNewIndex ) )
        {
          cellNew = GetCellRecord( lNewIndex );
        }
        else
        {
          bool bUpdateFormula = ( options & ExcelCopyRangeOptions.UpdateFormulas ) != 0;
          int iRowOffset = bUpdateFormula ? iRow - cell.Row : 0;
          int iColOffset = bUpdateFormula ? iColumn - cell.Column : 0;

          formulaDest.ParsedExpression = ( ( WorksheetImpl )Sheet ).UpdateFormula( formulaDest.ParsedExpression,
            iRowOffset, iColOffset );
        }
      }

      if ((options & ExcelCopyRangeOptions.All) != 0 || (options & ExcelCopyRangeOptions.CopyStyles) != 0)
      {
          int iXFIndex = cellNew.ExtendedFormatIndex;
          iXFIndex = GetXFIndex(iXFIndex, dicXFIndexes, options);
          cellNew.ExtendedFormatIndex = (ushort)iXFIndex;
      }
      else
      {
          int newRowIndex = RangeImpl.GetRowFromCellIndex(lNewIndex);
          int newColumnIndex = RangeImpl.GetColumnFromCellIndex(lNewIndex);
          cellNew.ExtendedFormatIndex = (Sheet[newRowIndex, newColumnIndex] as ICellPositionFormat).ExtendedFormatIndex;
      }

      cellNew.Column = iColumn;
      cellNew.Row = iRow;

      SetCellRecord( lNewIndex, cellNew );

      if( strFormulaValue != null )
      {
          string strCalculatedValue = null;
          IWorksheet sheet = m_dicRecords.Application.ActiveSheet;

          if (sheet != null && sheet.CalcEngine != null)
          {
              bool isPreserveFormula = sheet.CalcEngine.PreserveFormula;
              sheet.CalcEngine.PreserveFormula = true;
              strCalculatedValue = sheet[iRow + 1, iColumn + 1].CalculatedValue;
              sheet.CalcEngine.PreserveFormula = isPreserveFormula;
          }

          
        StringRecord stringRecord = ( StringRecord )BiffRecordFactory.GetRecord( TBIFFRecord.String );
        if (strCalculatedValue != null)
        {
            if (strCalculatedValue.Contains("\""))
            strCalculatedValue = strCalculatedValue.Substring(1, strCalculatedValue.Length - 2);
            stringRecord.Value = strCalculatedValue;
        }
        else
            stringRecord.Value = strFormulaValue;
        m_dicRecords.SetFormulaValue( iRow + 1, iColumn + 1, FormulaRecord.DEF_STRING_VALUE, stringRecord );
      }

      WorksheetHelper.AccessColumn( m_worksheet, cellNew.Column + 1 );
      WorksheetHelper.AccessRow( m_worksheet, cellNew.Row + 1 );

      return bCreateRange;
    }
    /// <summary>
    /// Caches intersection of the two ranges.
    /// </summary>
    /// <param name="destination">The first range to intersect.</param>
    /// <param name="source">The second range to intersect.</param>
    /// <param name="rectIntersection">Output intersection rectangle.</param>
    /// <returns>RecordTable with intersection.</returns>
    public RecordTable CacheIntersection( IRange destination, IRange source, out Rectangle rectIntersection )
    {
      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination.Worksheet != source.Worksheet )
      {
        rectIntersection = Rectangle.FromLTRB( -1, -1, -1, -1 );
        return null;
      }

      int iSourceColumn = source.Column;
      int iSourceRow = source.Row;

      int iRowCount = source.LastRow - iSourceRow + 1;
      int iColumnCount = source.LastColumn - iSourceColumn + 1; // check if +1 is needed

      Rectangle destRect = new Rectangle( destination.Column, destination.Row, 
        iColumnCount, iRowCount );

      Rectangle sourceRect = new Rectangle( iSourceColumn, iSourceRow,
        iColumnCount, iRowCount );

      if( !UtilityMethods.Intersects( destRect, sourceRect ) )
      {
        rectIntersection = Rectangle.FromLTRB( -1, -1, -1, -1 );
        return null;
      }

      rectIntersection = Rectangle.Intersect( destRect, sourceRect );

      if( rectIntersection.Width == 0 || rectIntersection.Height == 0 )
      {
        rectIntersection = Rectangle.FromLTRB( -1, -1, -1, -1 );
        return null;
      }

      RecordTable result = new RecordTable( m_book.MaxRowCount, m_worksheet );

      // Cache the intersection part.
      for( int row = rectIntersection.Top; row < rectIntersection.Bottom; row++ )
      {
        RowStorage arrRow = m_dicRecords.Rows[ row - 1 ];

        if( arrRow != null )
        {
          RowStorage newRow = arrRow.Clone( rectIntersection.Left - 1, rectIntersection.Right - 1,
            Application.RowStorageAllocationBlockSize );
          result.SetRow( row - 1, newRow );
        }
      }

      return result;
    }
    /// <summary>
    /// Returns minimum used row.
    /// </summary>
    /// <param name="iStartColumn">Start column.</param>
    /// <param name="iEndColumn">End column.</param>
    /// <returns>Minimum used row.</returns>
    public int GetMinimumRowIndex( int iStartColumn, int iEndColumn )
    {
      int iStartRow = m_worksheet.FirstRow;
      int iMinimumRow = m_worksheet.LastRow;

      for( int iRow = iStartRow; iRow < iMinimumRow; iRow++ )
      {
        for( int i = iStartColumn; i <= iEndColumn; i++ )
        {
          //long cellIndex = RangeImpl.GetCellIndex( i, iRow );

          if( m_dicRecords.Contains( iRow - 1, i - 1 ) )
          {
            iMinimumRow = iRow;
            break;
          }
        }
      }

      return iMinimumRow;
    }
    /// <summary>
    /// Returns maximum used row.
    /// </summary>
    /// <param name="iStartColumn">Start column.</param>
    /// <param name="iEndColumn">End column.</param>
    /// <returns>maximum used row.</returns>
    public int GetMaximumRowIndex( int iStartColumn, int iEndColumn )
    {
      int iStartRow = m_worksheet.LastRow;
      int iMaximumRow = m_worksheet.FirstRow;

      for( int iRow = iStartRow; iRow >= iMaximumRow; iRow-- )
      {
        //if( m_arrRows[ iRow - 1 ] == 0 ) continue;

        for( int i = iStartColumn; i <= iEndColumn; i++ )
        {

          long lCellIndex = RangeImpl.GetCellIndex( i, iRow );

          if( Contains( lCellIndex ) )
          {
            iMaximumRow = iRow;
            break;
          }
        }
      }

      return iMaximumRow;
    }
    /// <summary>
    /// Returns minimum used column.
    /// </summary>
    /// <param name="iStartRow">One-based index of the start row.</param>
    /// <param name="iEndRow">One-based index of the end row.</param>
    /// <returns>One-based index of the minimum used column.</returns>
    public int GetMinimumColumnIndex( int iStartRow, int iEndRow )
    {
      return m_dicRecords.GetMinimumColumnIndex( iStartRow - 1, iEndRow - 1 ) + 1;
    }
    /// <summary>
    /// Returns maximum used column.
    /// </summary>
    /// <param name="iStartRow">Start row.</param>
    /// <param name="iEndRow">End row.</param>
    /// <returns>Maximum used column.</returns>
    public int GetMaximumColumnIndex( int iStartRow, int iEndRow )
    {
      return m_dicRecords.GetMaximumColumnIndex( iStartRow - 1, iEndRow - 1 ) + 1;
    }
    /// <summary>
    /// Gets formula value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns formula value or null.</returns>
    public string GetFormula( long cellIndex )
    {
      return GetFormula( cellIndex, false );
    }
    /// <summary>
    /// Gets formula value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="isR1C1">If true - returns in R1C1 notations.</param>
    /// <returns>Returns formula value or null.</returns>
    public string GetFormula( long cellIndex, bool isR1C1 )
    {
      return GetFormula( cellIndex, isR1C1, null );
    }
    /// <summary>
    /// Gets formula value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="isR1C1">If true - returns in R1C1 notations.</param>
    /// <param name="numberInfo">Represents number info. Can be null.</param>
    /// <returns>Returns formula value or null.</returns>
    public string GetFormula( long cellIndex, bool isR1C1, NumberFormatInfo numberInfo )
    {
      FormulaRecord formula = GetCellRecord( cellIndex ) as FormulaRecord;

      if( formula != null )
      {
        try
        {
          FormulaUtil formulaParser = m_book.FormulaUtil;

          return "=" + formulaParser.ParsePtgArray( formula.ParsedExpression,
            formula.Row, formula.Column, isR1C1, numberInfo, false );
        }
        catch( Exception ex )
        {
          System.Diagnostics.Debug.WriteLine( ex.Message, "Exception" );
          System.Diagnostics.Debug.WriteLine( ex.StackTrace, "Stack trace" );
          return null;
        }
      }

      return null;
    }
    /// <summary>
    /// Gets string value by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell Index.</param>
    /// <returns>Returns string value or empty string.</returns>
    public string GetValue(long cellIndex, int row, int column, IRange range, string seperator)
    {
      if( Contains( cellIndex ) )
      {
        string result = "";
        string value;
        bool bChack = false;

        value = GetFormula( cellIndex );

        if( value != null )
        {
            result = range[row, column].DisplayText;
            return result;
        }

        value = GetText( cellIndex );

        if( value != null )
        {
          result = value;
          bChack = true;
        }

        if( bChack )
        {
            if (result.StartsWith("\""))
            {
                result = result.Replace("\"", "\"\"");
                result = '"' + result + '"';
            }
			else if(result.Contains("\""))
            {
                result = result.Replace( "\"", "\"\"" );
                return '"' + result + '"';
            }
            if (result.Contains(seperator))
            {
                result = "\"" + result + "\"";
            }
            return result;
        }

        value = GetError( cellIndex );
        if( value != null ) result = value;

        bool bVal;
        if( GetBool( cellIndex, out bVal ) ) result = bVal.ToString();

        double dValue = GetNumberWithoutFormula( cellIndex );
        if( dValue != Double.MinValue ) result = dValue.ToString();

        DateTime time = GetDateTime( cellIndex );
        if (time != DateTime.MinValue)
        {
            result = range[row, column].DisplayText;
        }

        return result;
      }

      return String.Empty;
    }
    /// <summary>
    /// Gets extended format index by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns extended format index or int.MINVALUE.</returns>
    public int GetExtendedFormatIndex( long cellIndex )
    {
      int iRow = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iColumn = RangeImpl.GetColumnFromCellIndex( cellIndex );

      return GetExtendedFormatIndex( iRow, iColumn );
    }
    /// <summary>
    /// Gets extended format index by cell index.
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index</param>
    /// <returns>Returns extended format index or int.MinValue if not found.</returns>
    public int GetExtendedFormatIndex( int row, int column )
    {
      row--;
      column--;

      RowStorage currentRow = Table.Rows[ row ];
      int iResult = int.MinValue;

      if( currentRow != null )
      {
        iResult = currentRow.GetXFIndexByColumn( column );
      }

      return iResult;
    }
    /// <summary>
    /// Gets extended format index of the row.
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <returns>Returns extended format index or int.MinValue if not found.</returns>
    public int GetExtendedFormatIndexByRow(int row)
    {
        row--;

        RowStorage currentRow = Table.Rows[row];
        int iResult = int.MinValue;

        if (currentRow != null)
        {
            iResult=currentRow.ExtendedFormatIndex;
        }

        return iResult;
    }
    /// <summary>
    /// Gets extended format index of the column.
    /// </summary>
    /// <param name="column">One-based column index.</param>
    /// <returns>Returns extended format index or int.MinValue if not found.</returns>
    public int GetExtendedFormatIndexByColumn(int column)
    {
        int iResult = int.MinValue;

        ColumnInfoRecord columnInfo = (ColumnInfoRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ColumnInfo);
        columnInfo.FirstColumn = (ushort)(column - 1);

        if (columnInfo != null)
            iResult = columnInfo.ExtendedFormatIndex;        

        return iResult;
    }
    /// <summary>
    /// Gets font by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns font or null.</returns>
    public IFont GetCellFont( long cellIndex )
    {
      int iXFIndex = GetExtendedFormatIndex( cellIndex );

      if( iXFIndex < 0 )
        return null;

      ExtendedFormatImpl format = m_book.InnerExtFormats[ iXFIndex ];
      
      return format.Font;
    }
    /// <summary>
    /// Copies style from one cell into another.
    /// </summary>
    /// <param name="iSourceRow">One-based row index of the source cell.</param>
    /// <param name="iSourceColumn">One-based column index of the source cell.</param>
    /// <param name="iDestRow">One-based row index of the destination cell.</param>
    /// <param name="iDestColumn">One-based column index of the destination cell.</param>
    public void CopyStyle( int iSourceRow, int iSourceColumn, int iDestRow, int iDestColumn )
    {
      ICellPositionFormat cell = GetCellRecord( iSourceRow, iSourceColumn );

      if( cell == null ) return;

      ushort usXFIndex = cell.ExtendedFormatIndex;
      SetCellStyle( iDestRow, iDestColumn, usXFIndex );

    }
    /// <summary>
    /// Creates new cell without adding it to the collection..
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to create.</param>
    /// <param name="iColumn">One-based column index of the cell to create.</param>
    /// <param name="recordType">Record type.</param>
    /// <returns>Created cell.</returns>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat CreateCellNoAdd( int iRow, int iColumn, TBIFFRecord recordType )
    {
      ICellPositionFormat cell = ( ICellPositionFormat )
        BiffRecordFactory.GetRecord( recordType );

      cell.Row = iRow - 1;
      cell.Column = iColumn - 1;

      return cell;
    }
    /// <summary>
    /// Creates new cell.
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to create.</param>
    /// <param name="iColumn">One-based column index of the cell to create.</param>
    /// <param name="recordType">Record type.</param>
    /// <returns>Created cell.</returns>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat CreateCell( int iRow, int iColumn, TBIFFRecord recordType )
    {
      ICellPositionFormat cell = CreateCellNoAdd( iRow, iColumn, recordType );
      SetCellRecord( iRow, iColumn, cell );

      return cell;
    }
    /// <summary>
    /// Gets cell style by cell index.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Returns cell index.</returns>
    public IStyle GetCellStyle( long cellIndex )
    {
      int iXFIndex = GetCellRecord( cellIndex ).ExtendedFormatIndex;

      return m_book.InnerStyles.GetByXFIndex( iXFIndex );
    }
    /// <summary>
    /// Returns object containing cell formatting.
    /// </summary>
    /// <param name="cellIndex">Cell index to get formatting for.</param>
    /// <returns>An ojbect containing cell formatting.</returns>
    public IExtendedFormat GetCellFormatting( long cellIndex )
    {
      int iXFIndex = GetCellRecord( cellIndex ).ExtendedFormatIndex;
      return m_book.InnerExtFormats[ iXFIndex ];
    }
    /// <summary>
    /// Sets number value.
    /// </summary>
    /// <param name="iCol">Represents column index. One-based.</param>
    /// <param name="iRow">Represents row index. One-based.</param>
    /// <param name="dValue">Represents number value.</param>
    public void SetNumberValue( int iRow, int iCol, double dValue )
    {
      SetNumberValue( iCol, iRow, dValue, m_book.DefaultXFIndex );
    }
    /// <summary>
    /// Sets number value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="dValue">Represents number value.</param>
    public void SetNumberValue( long cellIndex, double dValue )
    {
      int iRow  = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iCol = RangeImpl.GetColumnFromCellIndex( cellIndex );

      SetNumberValue( iCol, iRow, dValue );
    }
    /// <summary>
    /// Sets number value.
    /// </summary>
    /// <param name="iCol">Represents column index. One-based.</param>
    /// <param name="iRow">Represents row index. One-based.</param>
    /// <param name="dValue">Represents number value.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    public void SetNumberValue( int iRow, int iCol, double dValue, int iXFIndex )
    {
      NumberRecord record = ( NumberRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.Number );
      record.Value = dValue;
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      //Add( record );
      this[ iRow, iCol ] = record;
    }
    /// <summary>
    /// Sets boolean value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="bValue">Represents boolean value.</param>
    public void SetBooleanValue( int iRow, int iCol, bool bValue )
    {
      SetBooleanValue( iCol, iRow, bValue, m_book.DefaultXFIndex );
    }
    /// <summary>
    /// Sets boolean value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="bValue">Represents boolean value.</param>
    public void SetBooleanValue( long cellIndex, bool bValue )
    {
      int iRow  = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iCol  = RangeImpl.GetColumnFromCellIndex( cellIndex );

      SetBooleanValue( iCol, iRow, bValue );
    }
    /// <summary>
    /// Sets boolean value.
    /// </summary>
    /// <param name="iCol">Represents column index. One-based.</param>
    /// <param name="iRow">Represents row index. One-based.</param>
    /// <param name="bValue">Represents boolean value.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    public void SetBooleanValue( int iRow, int iCol, bool bValue, int iXFIndex )
    {
      BoolErrRecord record = ( BoolErrRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.BoolErr );
      record.IsErrorCode = false;
      record.BoolOrError = bValue ? ( byte )1 : ( byte )0;
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      SetCellRecord( RangeImpl.GetCellIndex( iCol, iRow ), record );
    }
    /// <summary>
    /// Sets error value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents error value.</param>
    public void SetErrorValue( int iRow, int iCol, string strValue )
    {
      SetErrorValue( iCol, iRow, strValue, m_book.DefaultXFIndex );
    }
    /// <summary>
    /// Sets error value.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <param name="strValue">Represents error value.</param>
    public void SetErrorValue( long cellIndex, string strValue )
    {
      int iRow  = RangeImpl.GetRowFromCellIndex( cellIndex );
      int iCol = RangeImpl.GetColumnFromCellIndex( cellIndex );

      SetErrorValue( iCol, iRow, strValue );
    }
    /// <summary>
    /// Sets error value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents error value.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    public void SetErrorValue( int iRow, int iCol, string strValue, int iXFIndex )
    {
      int iCode;
      if( FormulaUtil.ErrorNameToCode.TryGetValue( strValue, out iCode ) )
      {
        SetErrorValue( iRow, iCol, ( byte )iCode, iXFIndex );
      }
      else
      {
        throw new ArgumentOutOfRangeException( "strValue" );
      }
    }
    /// <summary>
    /// Sets error value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="errorCode">Represents error code.</param>
    /// <param name="iXFIndex">Represents xf index.</param>
    public void SetErrorValue( int iRow, int iCol, byte errorCode, int iXFIndex )
    {
      BoolErrRecord record = ( BoolErrRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.BoolErr );
      record.IsErrorCode = true;
      record.BoolOrError = ( byte )errorCode;
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      SetCellRecord( RangeImpl.GetCellIndex( iCol, iRow ), record );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents formula to set.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    public void SetFormula( int iRow, int iCol, string strValue, int iXFIndex )
    {
      SetFormula( iRow, iCol, strValue, iXFIndex, false );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents formula to set.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    /// <param name="isR1C1">If true - value in R1C1 notation.</param>
    /// <param name="formatInfo">Represent number format info, can be null.</param>
    public void SetFormula( int iRow, int iCol, string strValue, int iXFIndex
      , bool isR1C1, NumberFormatInfo formatInfo )
    {
      SetFormula( iRow, iCol, strValue, iXFIndex, isR1C1, true, formatInfo );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents formula to set.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    /// <param name="isR1C1">If true - value in R1C1 notation.</param>
    public void SetFormula( int iRow, int iCol, string strValue, int iXFIndex, bool isR1C1 )
    {
      SetFormula( iRow, iCol, strValue, iXFIndex, isR1C1, true, null );
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="strValue">Represents formula to set.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    /// <param name="isR1C1">If true - value in R1C1 notation.</param>
    /// <param name="bParse">If true - parse formula.</param>
    /// <param name="formatInfo">Represent number format info, can be null.</param>
    public void SetFormula( int iRow, int iCol, string strValue, int iXFIndex, bool isR1C1, bool bParse, NumberFormatInfo formatInfo )
    {
      FormulaRecord record = ( FormulaRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.Formula );
      strValue = strValue.Substring( 1 );
      FormulaUtil frmUtil = m_worksheet.ParentWorkbook.FormulaUtil;

      if( bParse )
      {
        frmUtil.NumberFormat = NumberFormatInfo.InvariantInfo;

        record.ParsedExpression = frmUtil.ParseString( strValue, Sheet, null, iRow - 1, iCol - 1, isR1C1 );

        frmUtil.NumberFormat = null;
      }

      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      SetCellRecord( RangeImpl.GetCellIndex( iCol, iRow ), record );
    }
    /// <summary>
    /// Sets blank value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    public void SetBlank( int iRow, int iCol, int iXFIndex )
    {
      BlankRecord record = ( BlankRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.Blank );
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      SetCellRecord( iRow, iCol, record );
      //Add( iCol - 1, iRow - 1, record );
    }
    /// <summary>
    /// Sets RTF value.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    /// <param name="rtf">Represents rtf to set.</param>
    public void SetRTF( int iRow, int iCol, int iXFIndex, TextWithFormat rtf )
    {
      if( rtf == null )
        throw new ArgumentNullException( "rtf" );

      LabelSSTRecord record = ( LabelSSTRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.LabelSST );
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;

      SortedList<int, int> lstRuns = rtf.InnerFormattingRuns;
      if( lstRuns != null && lstRuns.Count <= 1 )
        rtf.FormattingRuns.Clear();

      record.SSTIndex = m_book.InnerSST.AddIncrease( rtf );

      SetCellRecord( iRow, iCol, record );
      //Add( iCol - 1, iRow - 1, record );
    }
    /// <summary>
    /// Sets string value from existing SST collection.
    /// </summary>
    /// <param name="iCol">Represents column index. One based.</param>
    /// <param name="iRow">Represents row index. One based.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    /// <param name="iSSTIndex">Represents SST index</param>
    public void SetSingleStringValue( int iRow, int iCol, int iXFIndex, int iSSTIndex )
    {
      LabelSSTRecord record = ( LabelSSTRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.LabelSST );
      record.Row = iRow - 1;
      record.Column = iCol - 1;
      record.ExtendedFormatIndex = ( ushort )iXFIndex;
      record.SSTIndex = iSSTIndex;

      m_book.InnerSST.AddIncrease( iSSTIndex );

      Add( /*RangeImpl.GetCellIndex( iCol, iRow ),*/ record );
    }
    /// <summary>
    /// Sets cell to the non-SST string record (LabelRecord).
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    /// <param name="strValue">String value to set.</param>
    internal void SetNonSSTString( int row, int column, int iXFIndex, string strValue )
    {
      LabelRecord label = ( LabelRecord )m_recordExtractor.GetRecord( ( int )TBIFFRecord.Label );
      label.Row = row - 1;
      label.Column = column - 1;
      label.ExtendedFormatIndex = ( ushort )iXFIndex;
      label.Label = strValue;

      SetCellRecord( row, column, label );
    }
    /// <summary>
    /// Free's range object.
    /// </summary>
    /// <param name="iRow">One-based row index of the range object to remove from internal cache.</param>
    /// <param name="iColumn">One-based column index of the range object to remove from internal cache.</param>
    public void FreeRange( int iRow, int iColumn )
    {
      SetRange( iRow, iColumn, null );

      // Now we could try to remove cell record if it is not necessary.
      ICellPositionFormat cell = GetCellRecord( iRow, iColumn );

      if( cell != null && cell.TypeCode == TBIFFRecord.Blank )
      {
        int iXFIndex = cell.ExtendedFormatIndex;
        int iDefaultIndex = m_book.DefaultXFIndex;
        IOutline row = WorksheetHelper.GetRowOutline( Sheet, iRow );//RowInformation[ iRow ] as RowRecord;
        WorksheetImpl sheet = Sheet as WorksheetImpl;

        ColumnInfoRecord column = ( sheet != null ) ?
          sheet.ColumnInformation[ iColumn ] as ColumnInfoRecord :
          null;

        if( column != null )
        {
          iDefaultIndex = column.ExtendedFormatIndex;
        }

        if( row != null )
        {
          iDefaultIndex = row.ExtendedFormatIndex;
        }

        if( iXFIndex == iDefaultIndex )
        {
          Remove( iRow, iColumn );
        }
      }
    }
    /// <summary>
    /// Removes all data saving cells formatting.
    /// </summary>
    public void ClearData()
    {
        if (m_dicRecords != null)
        {
            for (int iRow = 0, iRowCount = m_dicRecords.RowCount; iRow < iRowCount; iRow++)
            {
                RowStorage arrRow = m_dicRecords.Rows[iRow];

                if (arrRow == null) continue;

                arrRow.ClearData();
                arrRow.ExtendedFormatIndex = (ushort)m_book.DefaultXFIndex;

                //        for( int iCol = 0, iColCount = m_dicRecords.ColCount; iCol < iColCount; iCol++ )
                //        {
                //          ICellPositionFormat cell = this[ iRow, iCol ];
                //
                //          if( cell != null && cell.TypeCode != TBIFFRecord.Blank )
                //          {
                //            BlankRecord blank = ( BlankRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Blank );
                //            blank.Row = cell.Row;
                //            blank.Column = cell.Column;
                //            blank.ExtendedFormatIndex = cell.ExtendedFormatIndex;
                //            m_dicRecords[ iRow, iCol ] = blank;
                //          }
                //        }
            }
        }
    }
    /// <summary>
    /// Sets array formula.
    /// </summary>
    /// <param name="record">Record to set.</param>
    [ CLSCompliant( false ) ]
    public void SetArrayFormula( ArrayRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      RowStorage rowStorage = m_dicRecords.Rows[ record.FirstRow ];
      rowStorage.SetArrayRecord( record.FirstColumn, record, Application.RowStorageAllocationBlockSize );
    }
    /// <summary>
    /// Returns array record corresponding to the cell
    /// </summary>
    /// <param name="iRow">One-based row index of the cell to get ArrayRecord for.</param>
    /// <param name="iColumn">One-based column index of the cell to get ArrayRecord for.</param>
    /// <returns>Corresponding array record.</returns>
    [ CLSCompliant( false ) ]
    public ArrayRecord GetArrayRecord( int iRow, int iColumn )
    {
      iRow--;
      iColumn--;

      RowStorage rowStorage = m_dicRecords.Rows[ iRow ];

      if( rowStorage == null ) return null;

      FormulaRecord formula = ( rowStorage.HasFormulaRecord( iColumn ) ) ?
        rowStorage.GetRecord( iColumn, Application.RowStorageAllocationBlockSize ) as FormulaRecord
        : null;

      if( formula == null ) return null;

      Ptg[] arrPtgs = formula.ParsedExpression;

      if( arrPtgs.Length != 1 ) return null;

      Ptg token = arrPtgs[ 0 ];

      if( token.TokenCode != FormulaToken.tExp ) return null;

      ControlPtg control = token as ControlPtg;

      if( control.RowIndex != iRow )
        rowStorage = m_dicRecords.Rows[ control.RowIndex ];

#if DEBUG
      if( rowStorage == null )
        throw new ArgumentNullException( "rowStorage" );
#endif

      return rowStorage.GetArrayRecord( control.ColumnIndex );
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect, int iDestIndex,
      Rectangle destRect )
    {
      m_dicRecords.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    /// <summary>
    /// Removes last column from the worksheet.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index.</param>
    public void RemoveLastColumn( int iColumnIndex )
    {
      m_dicRecords.RemoveLastColumn( iColumnIndex - 1 );
    }
    /// <summary>
    /// Removes last column from the worksheet.
    /// </summary>
    /// <param name="iRowIndex">One-based column index.</param>
    public void RemoveRow( int iRowIndex )
    {
      m_dicRecords.RemoveRow( iRowIndex - 1 );
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNameIndexes( WorkbookImpl book, int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      m_dicRecords.UpdateNameIndexes( book, arrNewIndex );
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    /// <param name="dicNewIndex">New indexes.</param>
    public void UpdateNameIndexes( WorkbookImpl book, IDictionary<int, int> dicNewIndex )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      m_dicRecords.UpdateNameIndexes( book, dicNewIndex );
    }
    /// <summary>
    /// Replaces all shared formula with ordinary formula.
    /// </summary>
    [ CLSCompliant( false ) ]
    public void ReplaceSharedFormula()
    {
      m_dicRecords.ReplaceSharedFormula( m_book );
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      if( arrNewIndexes == null )
        throw new ArgumentNullException( "arrNewIndexes" );

      m_dicRecords.UpdateStringIndexes( arrNewIndexes );
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>List with cell indexes that contains specified value.</returns>
    public List<long> Find( IRange range, string findValue
      , ExcelFindType flags, bool bIsFindFirst )
    {
        return m_dicRecords.Find(range, findValue, flags, bIsFindFirst, m_book);
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="findOptions">Way to find</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>
    /// List with cell indexes that contains specified value.
    /// </returns>
    public List<long> Find(IRange range, string findValue, ExcelFindType flags, ExcelFindOptions findOptions, bool bIsFindFirst)
    {
        
        return m_dicRecords.Find(range, findValue, flags,findOptions, bIsFindFirst, m_book);
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>List with cell indexes that contains specified value.</returns>
    public List<long> Find( IRange range, double findValue
      , ExcelFindType flags, bool bIsFindFirst )
    {
      return m_dicRecords.Find( range, findValue, flags, bIsFindFirst, m_book );
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="bIsError">Indicates whether we should look for error code or boolean value.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <returns>List with cell indexes that contains specified value.</returns>
    public List<long> Find( IRange range, byte findValue
      , bool bIsError, bool bIsFindFirst )
    {
      return m_dicRecords.Find( range, findValue, bIsError, bIsFindFirst, m_book );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictIndexes"></param>
    public List<long> Find( Dictionary<int, object> dictIndexes )
    {
      return m_dicRecords.Find( dictIndexes );
    }
    /// <summary>
    /// Caches and removes specified rectangle from the table.
    /// </summary>
    /// <param name="sourceRange">Source range.</param>
    /// <param name="iDeltaRow">Row delta to add to the resulting table.</param>
    /// <param name="iDeltaColumn">Column delta to add to the resulting table.</param>
    /// <param name="iMaxRow">Output maximum zero-based row index.</param>
    /// <param name="iMaxColumn">Output maximum zero-based column index.</param>
    /// <returns>Cached table.</returns>
    public RecordTable CacheAndRemove( RangeImpl sourceRange, int iDeltaRow, int iDeltaColumn,
      ref int iMaxRow, ref int iMaxColumn )
    {
      Rectangle sourceRect = sourceRange.GetRectangles()[ 0 ];
      RecordTable result = m_dicRecords.CacheAndRemove( sourceRect, iDeltaRow, iDeltaColumn,
        ref iMaxRow, ref iMaxColumn );

      // Make row/column one-based.
      iMaxRow++;
      iMaxColumn++;

      return result;

    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    public void UpdateExtendedFormatIndex( Dictionary<int, int> dictFormats )
    {
      m_dicRecords.UpdateExtendedFormatIndex( dictFormats );
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="arrFormats">Array with updated extended formats.</param>
    public void UpdateExtendedFormatIndex( int[] arrFormats )
    {
      m_dicRecords.UpdateExtendedFormatIndex( arrFormats );
    }
    /// <summary>
    /// This method updates indexes to the extended formats after version change.
    /// </summary>
    /// <param name="maxCount">New restriction for maximum possible XF index.</param>
    public void UpdateExtendedFormatIndex( int maxCount )
    {
      m_dicRecords.UpdateExtendedFormatIndex( maxCount );
    }
    /// <summary>
    /// Sets cell style.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iColumn">One-based column index.</param>
    /// <param name="iXFIndex">Index of the extended format to set.</param>
    public void SetCellStyle( int iRow, int iColumn, int iXFIndex )
    {
      RowStorage storage = Table.GetOrCreateRow( iRow - 1,
        AppImplementation.StandardHeightInRowUnits, true, m_worksheet.Version );

      storage.SetCellStyle( iRow - 1, iColumn - 1, iXFIndex, Application.RowStorageAllocationBlockSize );

      if( iXFIndex != m_book.DefaultXFIndex )
      {
        WorksheetHelper.AccessColumn( m_worksheet, iColumn );
        WorksheetHelper.AccessRow( m_worksheet, iRow );
      }
    }
    /// <summary>
    /// Sets row style.
    /// </summary>
    /// <param name="iRow">One-based row index.</param>
    /// <param name="iXFIndex">Index of the extended format to set.</param>
    public void SetCellStyle(int iRow, int index)
    {
        RowStorage storage = Table.GetOrCreateRow(iRow - 1,
         AppImplementation.StandardHeightInRowUnits, true, m_worksheet.Version);

        storage.ExtendedFormatIndex = (ushort)index;
        storage.IsFormatted = true;
    }
    /// <summary>
    /// Looks through all records and calls AddIncrease for each LabelSST record.
    /// </summary>
    public void ReAddAllStrings()
    {
      for( int i = FirstRow, last = LastRow; i <= last; i++ )
      {
        ApplicationImpl application = m_book.AppImplementation;
        RowStorage row = m_dicRecords.GetOrCreateRow( i - 1, application.StandardHeightInRowUnits,
          false, ExcelVersion.Excel97to2003 );

        if( row != null )
        {
          row.ReAddAllStrings( m_book.InnerSST );
        }
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      m_dicRecords.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      m_dicRecords.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iRowIndex"></param>
    /// <param name="iRowCount"></param>
    private void InsertIntoDefaultRows( int iRowIndex, int iRowCount )
    {
      Table.InsertIntoDefaultRows( iRowIndex, iRowCount );
    }
    /// <summary>
    /// Parses bool or error value from formula value.
    /// </summary>
    /// <param name="formula">Represents formula record.</param>
    /// <returns>Returns value string.</returns>
    private string GetFormulaErrorBoolText( FormulaRecord formula )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      if( !Double.IsNaN( formula.Value ) )
        throw new ArgumentException( "Formula record doesnot support error or bool." );

      if( formula.IsBool )
      {
        bool bVal = formula.BooleanValue;

        return bVal.ToString().ToUpper();
      }

      int iIndex = formula.ErrorValue;
      string strErrorName;

      return ( FormulaUtil.ErrorCodeToName.TryGetValue( iIndex, out strErrorName ) ) ?
        strErrorName :
        ErrorPtg.DEF_ERROR_NAME;
    }
    /// <summary>
    /// Creates internal cache for range objects.
    /// </summary>
    private void CreateRangesCollection()
    {
      m_colRanges = new SFTable( m_book.MaxRowCount, m_book.MaxColumnCount );
    }
    /// <summary>
    /// Updates sheet references in the formula.
    /// </summary>
    /// <param name="formula">Formula to update.</param>
    /// <param name="dicSheetNames">Dictionary with new worksheet names.</param>
    /// <param name="book">Source workbook.</param>
    private void UpdateSheetReferences( FormulaRecord formula, IDictionary dicSheetNames, WorkbookImpl book )
    {
      if( formula == null )
        throw new ArgumentNullException( "formula" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      Ptg[] arrPtg = formula.ParsedExpression;

      for( int i = 0, len = arrPtg.Length; i < len; i++ )
      {
        Ptg token = arrPtg[ i ];

        if( token is ISheetReference )
        {
          ISheetReference reference = ( ISheetReference )token;
          ushort usOldRefIndex = reference.RefIndex;
          string strSheetName = book.GetSheetNameByReference( usOldRefIndex );

          if( dicSheetNames != null && dicSheetNames.Contains( strSheetName ) )
          {
            strSheetName = ( string )dicSheetNames[ strSheetName ];
          }

          int iNewReference = m_book.AddSheetReference( strSheetName );
          reference.RefIndex = ( ushort )iNewReference;
        }
      }
    }
    /// <summary>
    /// Copies string records.
    /// </summary>
    /// <param name="dicSourceStrings">Source collection with strings to copy.</param>
    private void CopyStrings( IDictionary dicSourceStrings )
    {
      //      if( dicSourceStrings == null )
      //        throw new ArgumentNullException( "dicSourceStrings" );
      //
      //      foreach( DictionaryEntry entry in dicSourceStrings )
      //      {
      //        object clone = ( ( ICloneable )entry.Value ).Clone();
      //        m_dicStringCells[ entry.Key ] = clone;
      //      }
    }
    /// <summary>
    /// Returns format applied to the cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Format applied to the cell.</returns>
    private FormatImpl GetFormat( long cellIndex )
    {
      ICellPositionFormat cell = GetCellRecord( cellIndex );

      if( cell != null )
      {
        int iXFIndex = cell.ExtendedFormatIndex;
        ExtendedFormatImpl extFormat = m_book.InnerExtFormats[ iXFIndex ];
        int iIndex = extFormat.NumberFormatIndex;
        return m_book.InnerFormats[ iIndex ];
      }

      return null;
    }
    /// <summary>
    /// Returns font applied to the cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Font applied to the cell.</returns>
    private IFont GetFont( long cellIndex )
    {
      ICellPositionFormat cell = GetCellRecord( cellIndex );

      if( cell != null )
      {
        int iXFIndex = cell.ExtendedFormatIndex;
        ExtendedFormatImpl extFormat = m_book.InnerExtFormats[ iXFIndex ];
        int iIndex = extFormat.FontIndex;
        return m_book.InnerFonts[ iIndex ];
      }

      return null;
    }
    /// <summary>
    /// Gets new index of an extended format.
    /// </summary>
    /// <param name="iOldIndex">Old index of the extended format.</param>
    /// <param name="dicXFIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="options">Copy range options.</param>
    /// <returns>New index of the extended format.</returns>
    private int GetXFIndex( int iOldIndex, IDictionary dicXFIndexes, ExcelCopyRangeOptions options )
    {
      if( ( options & ExcelCopyRangeOptions.CopyStyles ) == 0 )
      {
        iOldIndex = m_book.DefaultXFIndex;
      }
      else if( dicXFIndexes != null && dicXFIndexes.Contains( iOldIndex ) )
      {
        iOldIndex = ( int )dicXFIndexes[ iOldIndex ];
      }

      return iOldIndex;
    }
    /// <summary>
    /// Updates LabelSST indexes after SST record parsing.
    /// </summary>
    /// <param name="dictUpdatedIndexes">Dictionary with indexes to update, key - old index, value - new index.</param>
    internal void UpdateLabelSSTIndexes( Dictionary<int, int> dictUpdatedIndexes, IncreaseIndex method )
    {
      m_dicRecords.UpdateLabelSSTIndexes( dictUpdatedIndexes, method );
    }
    /// <summary>
    /// Fills string object with data.
    /// </summary>
    /// <param name="richText">String object to fill.</param>
    /// <param name="sstIndex">Index in the shared strings table.</param>
    private void FillRichText( RichTextString richText, int sstIndex )
    {
      object textObject = m_book.InnerSST[ sstIndex ];
      TextWithFormat textWithFormat = textObject as TextWithFormat;

      if( textWithFormat != null )
      {
        richText.SetTextObject( textWithFormat.TypedClone() );
      }
      else
      {
        TextWithFormat currentText = richText.TextObject;

        if( currentText != null )
        {
          currentText.ClearFormatting();
          currentText.Text = textObject as string;
        }
        else
        {
          richText.SetTextObject( ( TextWithFormat )textObject );
        }
      }
    }
    /// <summary>
    /// Returns cell type.
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    internal Syncfusion.XlsIO.Implementation.WorksheetImpl.TRangeValueType GetCellType( int row, int column )
    {
      RowStorage storage = m_dicRecords.Rows[ row - 1 ];
      return ( storage != null ) ?
        storage.GetCellType( column - 1, false ) :
        WorksheetImpl.TRangeValueType.Blank;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// This method is called when object is about to be disposed.
    /// </summary>
    protected override void OnDispose()
    {
      if( !m_bIsDisposed )
      {
        if( m_dicRecords != null )
        {
          m_dicRecords.Dispose();
          m_dicRecords = null;
        }

        if( m_colRanges != null )
        {
          m_colRanges.Clear();
          m_colRanges = null;
        }

        m_book = null;
        m_worksheet = null;
      }

      base.OnDispose();
    }

    #endregion

    #region Class static methods
    /// <summary>
    /// Searches for the record of specified type.
    /// </summary>
    /// <param name="recordType">Record type to look for.</param>
    /// <param name="iRow">One-based index to the row to look at.</param>
    /// <param name="iCol">One-based index of the column to start looking at.</param>
    /// <param name="iLastCol">One-based index of the column to end looking at.</param>
    /// <returns>Column index that contains record of the specified type or value beyond iLastCol if not found.</returns>
    public int FindRecord( TBIFFRecord recordType, int iRow, int iCol, int iLastCol )
    {
      RowStorage row = m_dicRecords.Rows[ iRow - 1 ];
      return ( row != null ) ?
        row.FindRecord( recordType, iCol - 1, iLastCol - 1 ) + 1 :
        iLastCol + 1;
    }
    #endregion
  }
}