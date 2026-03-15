#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Syncfusion.XlsIO.Implementation.Security;
using TRangeValueType = Syncfusion.XlsIO.Implementation.WorksheetImpl.TRangeValueType;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif (WP)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Implements a two-dimensional table that holds an SFArrayList of rows. Each row
  /// is an SFArrayList of objects.
  /// </summary>
  /// <remarks>
  /// <p>This is a memory efficient way to represent a table where values can remain empty. Only rows
  /// that actually contain data will allocate an SFArrayList and the array only holds
  /// as many objects as the specific row contains columns.</p>
  /// <p>When you access data that are out of range, an empty () object will be returned.
  /// If you set data that are out of range, an exception will be thrown. If you set data for
  /// a row that is empty, the row will be allocated before the value is stored.</p>
  /// <p>SFTable provides methods that let you insert, remove or rearrange columns or m_arrRows
  /// in the table.</p>
  /// </remarks>
  public class RecordTable
    : ICloneable
    , IDisposable
  {
    #region Class members
    /// <summary>
    /// Number of m_arrRows in the collection.
    /// </summary>
    private int m_iRowCount;
    /// <summary>
    /// Collection of rows.
    /// </summary>
    private ArrayListEx m_arrRows = new ArrayListEx();
    /// <summary>
    /// First created row.
    /// </summary>
    private int m_iFirstRow = -1;
    /// <summary>
    /// Last created row.
    /// </summary>
    private int m_iLastRow = -1;
    /// <summary>
    /// Indicates whether object was disposed.
    /// </summary>
    private bool m_bIsDisposed;
    /// <summary>
    /// This array contains all shared formulas found during parsing.
    /// </summary>
    private Dictionary<long, SharedFormulaRecord> m_arrShared;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private IInternalWorksheet m_sheet;
    #endregion

    #region Class Initialize/Finalize methods
    /// <overload>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class.
    /// </overload>
    /// <summary>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class that is empty.
    /// </summary>
    /// <param name="iRowCount">Number of rows in the collection.</param>
    /// <param name="sheet">Parent worksheet object.</param>
    public RecordTable( int iRowCount, IInternalWorksheet sheet )
    {
      m_sheet = sheet;
      m_book = sheet.ParentWorkbook;
      m_iRowCount = iRowCount;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="SFTable" />
    /// class and optional copies of data from an existing table.
    /// </summary>
    /// <param name="data">Represents record table data.</param>
    /// <param name="clone">Indicates is clone.</param>
    /// <param name="sheet">Parent worksheet object.</param>
    protected RecordTable( RecordTable data, bool clone, IInternalWorksheet sheet )
    {
      m_iRowCount = data.m_iRowCount;
      m_sheet = sheet;
      m_book = sheet.ParentWorkbook;

      if( clone )
      {
        m_iFirstRow = data.m_iFirstRow;
        m_iLastRow = data.m_iLastRow;
        ArrayListEx dataRows = data.m_arrRows;

        for( int i = 0; i < m_iRowCount; i++ )
        {
          RowStorage row = data.Rows[ i ];

          if( row != null )
          {
            m_arrRows[ i ] = ( RowStorage )row.Clone( m_book.HeapHandle );
          }
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
      if( !m_bIsDisposed )
      {
        if( m_iFirstRow >= 0 )
        {
          for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
          {
            RowStorage arrRow = m_arrRows[ i ];

            if( arrRow != null ) arrRow.Dispose();
          }
        }

        m_iFirstRow = -1;
        m_iLastRow = -1;
        m_iRowCount = -1;
        m_arrRows = null;
        m_bIsDisposed = true;
        m_book = null;

        if( m_arrShared != null )
        {
          m_arrShared.Clear();
          m_arrShared = null;
        }

        GC.SuppressFinalize( this );
      }
    }
    /// <summary>
    /// Object finalizer.
    /// </summary>
    ~RecordTable()
    {
      Dispose();
    }
    #endregion

    #region ICloneable methods
    /// <summary>
    ///   <para>Creates a deep copy of the <see cref="RecordTable" />.</para>
    /// </summary>
    /// <returns>
    ///   <para>A deep copy of the <see cref="RecordTable" />.</para>
    /// </returns>
    public virtual object Clone()
    {
      return new RecordTable( this, true, m_sheet );
    }
    /// <summary>
    ///   <para>Creates a deep copy of the <see cref="RecordTable" />.</para>
    /// </summary>
    /// <param name="parentWorksheet">Parent worksheet object.</param>
    /// <returns>
    ///   <para>A deep copy of the <see cref="RecordTable" />.</para>
    /// </returns>
    public virtual object Clone( IInternalWorksheet parentWorksheet )
    {
      return new RecordTable( this, true, parentWorksheet );
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Return parent Application object. Read-only.
    /// </summary>
    public IApplication Application
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_book.Application;
      }
    }
    /// <summary>
    /// Return parent Application object. Read-only.
    /// </summary>
    public ApplicationImpl AppImplementation
    {
      get
      {
        return m_book.AppImplementation;
      }
    }
    /// <summary>
    /// Returns the SFArrayList from all rows.
    /// </summary>
    public ArrayListEx Rows
    {
      get
      {
        return m_arrRows;
      }
    }
    /// <summary>
    /// Gets the number of rows contained in the <see cref="SFTable" />. Read-only.
    /// </summary>
    public int RowCount
    {
      get
      {
        return m_iRowCount;
      }
      set
      {
        m_iRowCount = value;
        m_arrRows.ReduceSizeIfNecessary( value );
        m_iLastRow = Math.Min( m_iLastRow, value );
        m_iFirstRow = Math.Min( m_iFirstRow, value );
      }
    }
    /// <summary>
    /// Returns zero-based index of the first created row.
    /// </summary>
    public int FirstRow
    {
      get
      {
        return m_iFirstRow;
      }
    }
    /// <summary>
    /// Returns zero-based index of the last created row.
    /// </summary>
    public int LastRow
    {
      get
      {
        return m_iLastRow;
      }
    }
    /// <summary>
    ///   <para>Gets / sets an element at the specified coordinates in the <see cref="SFTable" />.</para>
    /// </summary>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="colIndex">The zero-based column index.</param>
    /// <remarks>
    /// If you query for an element and the coordinates are out of range, an empty (<see langword="null" />) object will be returned.<para/>
    /// If you set an element and the the coordinates are out of range, an exception is thrown.
    /// </remarks>
    public object this[ int rowIndex, int colIndex ]
    {
      get
      {
        if( rowIndex >= m_iRowCount || rowIndex < 0
          /*|| colIndex >= m_iColumnCount || colIndex < 0*/ )
          return null;

        RowStorage arrRow = Rows[ rowIndex ];

        return ( arrRow == null )
          ? null
          : arrRow.GetRecord( colIndex, Application.RowStorageAllocationBlockSize );
      }
      set
      {
        if( rowIndex >= m_iRowCount || rowIndex < 0 )
          throw new ArgumentOutOfRangeException( "rowIndex" );

        //        if( colIndex >= m_iColumnCount || colIndex < 0 )
        //          throw new ArgumentOutOfRangeException( "colIndex" );

        RowStorage arrRow = GetOrCreateRow( rowIndex, m_sheet.DefaultRowHeight,
          value != null, m_book.Version );

        //        object savedValue = arrRow.GetRecord( colIndex );
        //
        //        if( savedValue != null )
        //        {
        //          if( value == null ) m_iCellCount--;
        //        }
        //        else if( value != null )
        //        {
        //          m_iCellCount++;
        //        }


        if (arrRow != null)
        {
            arrRow.SetWorkbook(m_book, rowIndex);
            arrRow.SetRecord(colIndex, (ICellPositionFormat)value,
              Application.RowStorageAllocationBlockSize);
        }
      }
    }
    /// <summary>
    /// Returns List with shared formulas. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public Dictionary<long, SharedFormulaRecord> SharedFormulas
    {
      get
      {
        if( m_arrShared == null )
          m_arrShared = new Dictionary<long, SharedFormulaRecord>();

        return m_arrShared;
      }
    }
    /// <summary>
    /// Number of shared formulas. Read-only.
    /// </summary>
    private int SharedCount
    {
      get
      {
        return ( m_arrShared != null ) ? m_arrShared.Count : 0;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    ///   <para>Removes all elements from the <see cref="SFTable" />.</para>
    /// </summary>
    public void Clear()
    {
      if( m_iFirstRow != -1 )
      {
        for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
        {
          RowStorage row = m_arrRows[ i ];
          m_arrRows[ i ] = null;

          if( row != null )
            row.Dispose();
        }
      }
    }
    internal void UpdateRows(int rowCount)
    {
        m_arrRows.UpdateSize(rowCount);
    }
    /// <summary>
    /// Creates a collection of cells for a row.
    /// </summary>
    /// <param name="iRowIndex">Zero-based row index.</param>
    /// <param name="height">Row height.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>An SFArrayList or derived object for the cell collection.</returns>
    public virtual RowStorage CreateCellCollection( int iRowIndex, int height, ExcelVersion version )
    {
      RowStorage result = new RowStorage( iRowIndex, height, m_book.DefaultXFIndex );
      result.IsFormatted = false;
      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          result.SetCellPositionSize( 4, AppImplementation.RowStorageAllocationBlockSize, version );
          break;

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          result.SetCellPositionSize( 8, AppImplementation.RowStorageAllocationBlockSize, version );
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      return result;
    }
    /// <summary>
    ///   <para>Indicates whether an element is at the specified coordinates in the <see cref="SFTable" />.</para>
    /// </summary>
    /// <param name="rowIndex">The zero-based row index.</param>
    /// <param name="colIndex">The zero-based column index.</param>
    /// <returns>
    ///   <para>
    ///     <see langword="true" /> if an element exists at the specified coordinates in the <see cref="SFTable" />;
    ///  <see langword="false" /> otherwise.</para>
    /// </returns>
    public bool Contains( int rowIndex, int colIndex )
    {
      if( rowIndex < 0 || rowIndex >= m_iRowCount 
        /*|| colIndex < 0 || colIndex >= m_iColumnCount*/ )
        return false;

      if( m_arrRows == null ) return false;

      RowStorage arrRow = m_arrRows[ rowIndex ];

      return ( arrRow == null )
        ? false
        : arrRow.Contains( colIndex );
    }
    /// <summary>
    /// Extracts Array record from the collection.
    /// </summary>
    /// <param name="cell">Cell that contains array-entered formula.</param>
    /// <returns>ArrayRecord if there is one; null if cell doesn't contain array-entered formula.</returns>
    [ CLSCompliant( false ) ]
    public ArrayRecord GetArrayRecord( ICellPositionFormat cell )
    {
      if( cell == null )
        throw new ArgumentNullException( "cell" );

      if( cell.TypeCode != TBIFFRecord.Formula ) return null;

      FormulaRecord formula = ( FormulaRecord )cell;
      Ptg[] arrPtgs = formula.ParsedExpression;

      if( arrPtgs == null || arrPtgs.Length != 1 ) return null;

      ControlPtg control = arrPtgs[ 0 ] as ControlPtg;

      if( control == null ) return null;

      int iRow = control.RowIndex;
      int iColumn = control.ColumnIndex;

      RowStorage arrRow = ( RowStorage )Rows[ iRow ];

      if( arrRow == null ) return null;

      return arrRow.GetArrayRecord( iColumn );
    }
    /// <summary>
    /// Updates first and last row indexes.
    /// </summary>
    /// <param name="iRowIndex">Zero-based index of the created row.</param>
    public void AccessRow( int iRowIndex )
    {
      if( m_iFirstRow < 0 )
      {
        m_iFirstRow = iRowIndex;
        m_iLastRow = iRowIndex;
      }
      else
      {
        m_iFirstRow = Math.Min( m_iFirstRow, iRowIndex );
        m_iLastRow = Math.Max( m_iLastRow, iRowIndex );
      }
    }
    /// <summary>
    /// Sets row.
    /// </summary>
    /// <param name="iRowIndex">Zero-based row index to set.</param>
    /// <param name="row">Row object to set.</param>
    public void SetRow( int iRowIndex, RowStorage row )
    {
      Rows[ iRowIndex ] = row;
      AccessRow( iRowIndex );
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
      if( m_iFirstRow < 0 || m_arrRows == null ) return;
      int iBlockSize = Application.RowStorageAllocationBlockSize;

      for( int iRow = m_iFirstRow; iRow <= m_iLastRow; iRow ++ )
      {
        RowStorage arrRow = m_arrRows[ iRow ];

        if( arrRow != null )
        {
          arrRow.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect, iBlockSize, m_book );
        }
      }

      //UpdateArrayFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    /// <summary>
    /// Removes last column from the worksheet.
    /// </summary>
    /// <param name="iColumnIndex">Zero-based column index.</param>
    public void RemoveLastColumn( int iColumnIndex )
    {
      if( m_arrRows == null ) return;

      int iBlockSize = m_book.Application.RowStorageAllocationBlockSize;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow != null )
        {
          arrRow.Remove( iColumnIndex, iColumnIndex + 1, iBlockSize );
//          range.Value = "";
//          range.CellStyle = m_book.Styles[ "Normal" ];
//          range.ColumnWidth = StandardWidth;
        }
      }
    }
    /// <summary>
    /// Removes row from the worksheet.
    /// </summary>
    /// <param name="iRowIndex">Zero-based row index to remove.</param>
    public void RemoveRow( int iRowIndex )
    {
      if( m_arrRows == null ) return;

      RowStorage row = m_arrRows[ iRowIndex ];

      if( row != null )
        row.Dispose();

      SetRow( iRowIndex, null );

      bool bFound = false;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        if( m_arrRows[ i ] != null )
        {
          bFound = true;
          m_iFirstRow = i;
          break;
        }
      }

      for( int i = m_iLastRow; i >= m_iFirstRow; i-- )
      {
        if( m_arrRows[ i ] != null )
        {
          bFound = true;
          m_iLastRow = i;
          break;
        }
      }

      if( !bFound )
      {
        m_iFirstRow = -1;
        m_iLastRow = -1;
      }
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

      if( m_iFirstRow < 0 ) return;
      int iBlockSize = Application.RowStorageAllocationBlockSize;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = m_arrRows[ i ];

        if( row != null )
        {
          row.UpdateNameIndexes( book, arrNewIndex, iBlockSize );
        }
      }
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

      if( m_iFirstRow < 0 ) return;
      int iBlockSize = Application.RowStorageAllocationBlockSize;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = m_arrRows[ i ];

        if( row != null )
        {
          row.UpdateNameIndexes( book, dicNewIndex, iBlockSize );
        }
      }
    }
    /// <summary>
    /// Replaces all shared formula with ordinary formula.
    /// </summary>
    /// <param name="book"></param>
    public void ReplaceSharedFormula( WorkbookImpl book )
    {
      if( SharedCount > 0 )
      {
        foreach( KeyValuePair<long, SharedFormulaRecord> pair in SharedFormulas )
        {
          long lCellIndex = pair.Key;
          SharedFormulaRecord shared = pair.Value;
          int iRow = RangeImpl.GetRowFromCellIndex( lCellIndex );
          int iColumn = RangeImpl.GetColumnFromCellIndex( lCellIndex );
          ReplaceSharedFormula( book, iRow, iColumn, shared );
        }
      }

      m_arrShared = null;
    }
    /// <summary>
    /// Replaces all shared formula with ordinary formula.
    /// </summary>
    [ CLSCompliant( false ) ]
    public void ReplaceSharedFormula( WorkbookImpl book, int row, int column, SharedFormulaRecord shared )
    {
      for( int iRow = shared.FirstRow, iLastRow = shared.LastRow; iRow <= iLastRow; iRow++ )
      {
        RowStorage rowStorage = m_arrRows[ iRow ];

        if( rowStorage != null )
          rowStorage.ReplaceSharedFormula( book, row, column, shared );
      }
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      if( arrNewIndexes == null )
        throw new ArgumentNullException( "arrNewIndexes" );

      if( m_iFirstRow < 0 ) return;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = m_arrRows[ i ];

        if( row != null )
          row.UpdateStringIndexes( arrNewIndexes );
      }
    }
    /// <summary>
    /// Copies cells from another worksheet.
    /// </summary>
    /// <param name="sourceCells">Source cells collection to copy cells from.</param>
    /// <param name="sourceSST">Source SST dictionary.</param>
    /// <param name="destSST">Destination SST dictionary.</param>
    /// <param name="hashExtFormatIndexes">Dictionary with new extended format indexes.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicNameIndexes">Dictionary with new name indexes.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    public void CopyCells( RecordTable sourceCells, SSTDictionary sourceSST,
      SSTDictionary destSST, Dictionary<int, int> hashExtFormatIndexes,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicNameIndexes,
      Dictionary<int, int> dicFontIndexes, Dictionary<int, int> dictExternSheet )
    {
      m_iFirstRow = sourceCells.m_iFirstRow;
      m_iLastRow = sourceCells.m_iLastRow;

      m_arrRows = new ArrayListEx();

      if( sourceCells.m_iFirstRow < 0 ) return;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = sourceCells.m_arrRows[ i ];

        if( row != null )
        {
          RowStorage clone = row.Clone( sourceSST, destSST, hashExtFormatIndexes,
            hashWorksheetNames, dicNameIndexes, dicFontIndexes, dictExternSheet );

          SetRow( i, clone );
        }
      }
    }

    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    public List<long> Find( IRange range, string findValue
      , ExcelFindType flags, bool bIsFindFirst, WorkbookImpl book )
    {
        return Find(range, findValue, flags, ExcelFindOptions.None, bIsFindFirst, book);
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value to find.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="findOptions">Way to find the value.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <param name="book">The book.</param>
    /// <returns>
    /// List with cell indexes that contains specified value.
    /// </returns>
    public List<long> Find(IRange range, string findValue
, ExcelFindType flags,ExcelFindOptions findOptions, bool bIsFindFirst, WorkbookImpl book)
    {
        if (range == null)
            throw new ArgumentNullException("range");

        if (findValue == null || findValue.Length == 0)
            return null;

        bool bIsTextIsError = FormulaUtil.ErrorNameToCode.ContainsKey(findValue);
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);
        bool bIsFormula = (flags & ExcelFindType.Formula) == ExcelFindType.Formula;
        bool bIsFormulaStringValue = (flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue;

        if (!(bIsText || bIsFormula || bIsError || bIsFormulaStringValue))
            throw new ArgumentException("Parameter flags is not valid.", "flags");

        int iErrorCode = 0;
        bIsError = bIsError && bIsTextIsError;
        List<long> arrIndexes = new List<long>();

        if (bIsError)
            iErrorCode = FormulaUtil.ErrorNameToCode[findValue];

        int iFirstColumn = range.Column - 1;
        int iLastColumn = range.LastColumn - 1;

        if (m_arrRows != null)
        {
            for (int i = range.Row - 1, iLastRow = range.LastRow - 1; i <= iLastRow; i++)
            {
                RowStorage arrRow = m_arrRows[i];
                
                if (arrRow == null)
                    continue;

                arrIndexes.AddRange(arrRow.Find(iFirstColumn, iLastColumn, findValue,
                  flags,findOptions, iErrorCode, bIsFindFirst, book));
            }
        }
        book.IsStartsOrEndsWith = null;
        return arrIndexes;
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="flags">If true - finds first value; otherwise - all values.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    public List<long> Find( IRange range, double findValue
      , ExcelFindType flags, bool bIsFindFirst, WorkbookImpl book )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      bool bFormula = ( ( flags & ExcelFindType.FormulaValue ) != 0 );
      bool bNumber = ( ( flags & ExcelFindType.Number ) != 0 );

      if( !( bFormula || bNumber ) )
        throw new ArgumentException( "Parameter flags is not valid.", "flags" );

      List<long> arrIndexes = new List<long>();

      int iFirstColumn = range.Column - 1;
      int iLastColumn = range.LastColumn - 1;

      if( m_arrRows != null )
      {
        for( int i = range.Row - 1, iLastRow = range.LastRow - 1; i <= iLastRow; i++ )
        {
          RowStorage arrRow = m_arrRows[ i ];

          if( arrRow == null )
            continue;

          arrIndexes.AddRange( arrRow.Find( iFirstColumn, iLastColumn, findValue,
            flags, bIsFindFirst, book ) );

          if( bIsFindFirst && arrIndexes.Count > 0 ) break;
        }
      }

      return arrIndexes;
    }
    /// <summary>
    /// Returns found values or null.
    /// </summary>
    /// <param name="range">Storage range.</param>
    /// <param name="findValue">Value for finding.</param>
    /// <param name="bErrorCode">Indicates whether we should look for error code or boolean value.</param>
    /// <param name="bIsFindFirst">If findfirst - true then returns range; otherwise - array with all found values.</param>
    /// <param name="book">Parent workbook.</param>
    /// <returns></returns>
    public List<long> Find( IRange range, byte findValue
      , bool bErrorCode, bool bIsFindFirst, WorkbookImpl book )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      List<long> arrIndexes = new List<long>();

      int iFirstColumn = range.Column - 1;
      int iLastColumn = range.LastColumn - 1;

      if( m_arrRows != null )
      {
        for( int i = range.Row - 1, iLastRow = range.LastRow - 1; i <= iLastRow; i++ )
        {
          RowStorage arrRow = m_arrRows[ i ];

          if( arrRow == null ) continue;

          arrIndexes.AddRange( arrRow.Find( iFirstColumn, iLastColumn, findValue,
            bErrorCode, bIsFindFirst, book ) );
        }
      }

      return arrIndexes;
    }
    /// <summary>
    /// Returns minimum used column.
    /// </summary>
    /// <param name="iStartRow">Start row.</param>
    /// <param name="iEndRow">End row.</param>
    /// <returns>Minimum used column.</returns>
    public int GetMinimumColumnIndex( int iStartRow, int iEndRow )
    {
      bool bFound = false;
      int iMinimumColumn = int.MaxValue;

      if( m_arrRows == null ) return -1;

      for( int iRow = iStartRow; iRow <= iEndRow; iRow++ )
      {
        RowStorage arrRow = m_arrRows[ iRow ];

        if( arrRow != null && arrRow.UsedSize > 0 )
        {
          iMinimumColumn = Math.Min( iMinimumColumn, arrRow.FirstColumn );
          bFound = true;
        }
      }

      return bFound ? iMinimumColumn : -1;
    }
    /// <summary>
    /// Returns maximum used column.
    /// </summary>
    /// <param name="iStartRow">Start row.</param>
    /// <param name="iEndRow">End row.</param>
    /// <returns>Maximum used column.</returns>
    public int GetMaximumColumnIndex( int iStartRow, int iEndRow )
    {
      bool bFound = false;
      int iMaximumColumn = int.MinValue;

      if( m_arrRows == null ) return -1;

      for( int iRow = iStartRow; iRow <= iEndRow; iRow++ )
      {
        RowStorage arrRow = m_arrRows[ iRow ];

        if( arrRow != null && arrRow.UsedSize > 0 )
        {
          iMaximumColumn = Math.Max( iMaximumColumn, arrRow.LastColumn );
          bFound = true;
        }
      }

      return bFound ? iMaximumColumn : -1;
    }
    /// <summary>
    /// Determines whether collection contains row.
    /// </summary>
    /// <param name="iRowIndex">Zero-based row index.</param>
    /// <returns>True if the collection contains at least one element with specified row index; otherwise, False.</returns>
    public bool ContainsRow( int iRowIndex )
    {
      return ( m_arrRows != null && iRowIndex >= m_iFirstRow
        && iRowIndex <= m_iLastRow
        && m_arrRows[ iRowIndex ] != null );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictIndexes"></param>
    public List<long> Find( Dictionary<int, object> dictIndexes )
    {
      List<long> arrRanges = new List<long>();

      if( dictIndexes == null || dictIndexes.Count == 0 || m_arrRows == null )
        return arrRanges;

      //foreach( DictionaryEntry entry in m_dicRecordsCells )
      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow != null )
        {
          arrRow.Find( dictIndexes, arrRanges );
        }
      }

      return arrRanges;
    }
    /// <summary>
    /// Caches and removes specified rectangle from the table.
    /// </summary>
    /// <param name="rectSource">Source rectangle.</param>
    /// <param name="iDeltaRow">Row delta to add to the resulting table.</param>
    /// <param name="iDeltaColumn">Column delta to add to the resulting table.</param>
    /// <param name="iMaxRow">Output maximum zero-based row index.</param>
    /// <param name="iMaxColumn">Output maximum zero-based column index.</param>
    /// <returns>Cached table.</returns>
    public RecordTable CacheAndRemove( Rectangle rectSource, int iDeltaRow, int iDeltaColumn,
      ref int iMaxRow, ref int iMaxColumn )
    {
      // TODO: here we can optimize a little bit by evaluating real required size.
      RecordTable result = new RecordTable( m_iRowCount, m_sheet );

      int iStartRow = Math.Max( rectSource.Y, m_iFirstRow );
      int iEndRow = Math.Min( rectSource.Bottom, m_iLastRow );
      int iStartColumn = rectSource.X;
      int iLastColumn = rectSource.Right;
      int iBlockSize = Application.RowStorageAllocationBlockSize;

      for( int iRow = iStartRow; iRow <= iEndRow; iRow++ )
      {
        RowStorage arrRow = m_arrRows[ iRow ];

        if( arrRow != null )
        {
          RowStorage newRow = arrRow.Clone( iStartColumn, iLastColumn,
            Application.RowStorageAllocationBlockSize );

          if( newRow != null )
          {
            newRow.RowColumnOffset( iDeltaRow, iDeltaColumn, iBlockSize );
            arrRow.Remove( iStartColumn, iLastColumn, iBlockSize );
            iMaxRow = Math.Max( iMaxRow, iRow + iDeltaRow );
            iMaxColumn = Math.Max( iMaxColumn, newRow.LastColumn );
          }

          result.SetRow( iRow + iDeltaRow, newRow );
        }
      }

      result.m_iFirstRow = rectSource.Y + iDeltaRow;//m_iFirstRow + iDeltaRow;
      result.m_iLastRow = rectSource.Bottom + iDeltaRow;//m_iLastRow + iDeltaRow;
      return result;
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    public void UpdateExtendedFormatIndex( Dictionary<int, int> dictFormats )
    {
      if( m_arrRows == null || m_iFirstRow < 0 ) return;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow == null ) continue;

        arrRow.UpdateExtendedFormatIndex( dictFormats, Application.RowStorageAllocationBlockSize );
      }
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="arrFormats">Array with updated extended formats.</param>
    public void UpdateExtendedFormatIndex( int[] arrFormats )
    {
      if( m_arrRows == null || m_iFirstRow < 0 )
        return;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow == null )
          continue;

        arrRow.UpdateExtendedFormatIndex( arrFormats, Application.RowStorageAllocationBlockSize );
      }
    }
    /// <summary>
    /// This method updates indexes to the extended formats after version change.
    /// </summary>
    /// <param name="maxCount">New restriction for maximum possible XF index.</param>
    public void UpdateExtendedFormatIndex( int maxCount )
    {
      if( m_arrRows == null || m_iFirstRow < 0 ) return;
      int iDefaultXF = m_book.DefaultXFIndex;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow == null ) continue;

        arrRow.UpdateExtendedFormatIndex( maxCount, iDefaultXF );
      }
    }
    /// <summary>
    /// Updates LabelSST indexes after SST record parsing.
    /// </summary>
    /// <param name="dictUpdatedIndexes">Dictionary with indexes to update, key - old index, value - new index.</param>
    internal void UpdateLabelSSTIndexes( Dictionary<int, int> dictUpdatedIndexes, IncreaseIndex method )
    {
      if( m_arrRows == null ) return;

      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage arrRow = m_arrRows[ i ];

        if( arrRow == null ) continue;

        arrRow.UpdateLabelSSTIndexes( dictUpdatedIndexes, method );
      }
    }
    /// <summary>
    /// Extracts ranges from the reader.
    /// </summary>
    /// <param name="reader">Reader to extract ranges from.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    /// <param name="sst">SSTDictionary of the parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public void ExtractRanges( BiffReader reader, bool bIgnoreStyles,
      SSTDictionary sst, WorksheetImpl sheet, IDecryptor decryptor )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      BinaryReader binaryReader = reader.BaseReader;
      Stream stream = binaryReader.BaseStream;
      byte[] arrData = reader.Buffer;
      RowStorage currentRow = null;
      int iRowIndex = -1;
      int iCurrentRow = 0;
      int iCurrentColumn = 0;
      ByteArrayDataProvider provider = new ByteArrayDataProvider( arrData );

      while( true )
      {
        long lStartPosition = stream.Position;
        stream.Read( arrData, 0, BiffRecordRaw.DEF_HEADER_SIZE );

        short sRecordType = BitConverter.ToInt16( arrData, 0 );//binaryReader.ReadInt16();
        TBIFFRecord recordType = ( TBIFFRecord )sRecordType;
        short sRecordLen = BitConverter.ToInt16( arrData, 2 );//binaryReader.ReadInt16();

        if( recordType == 0 )
          throw new ArgumentOutOfRangeException( "recordType" );

        switch( recordType )
        {
          case TBIFFRecord.MulRK:
          case TBIFFRecord.MulBlank:
            stream.Read( arrData, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen );

            if( decryptor != null )
            {
              decryptor.Decrypt( provider, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen,
                lStartPosition + BiffRecordRaw.DEF_HEADER_SIZE );
              //throw new NotImplementedException();
            }

            iCurrentRow = BitConverter.ToUInt16( arrData, BiffRecordRaw.DEF_HEADER_SIZE );
            iCurrentColumn = BitConverter.ToUInt16( arrData, BiffRecordRaw.DEF_HEADER_SIZE + 2 );
            sRecordLen += BiffRecordRaw.DEF_HEADER_SIZE;
            int iLastColumn = BitConverter.ToUInt16( arrData, sRecordLen - 2 );
            WorksheetHelper.AccessColumn( m_sheet, iLastColumn + 1 );

            if( iRowIndex != iCurrentRow )
            {
              // We can set height to 0, since if file is correct than RowRecord
              // must be present for each row that has any data.
              currentRow = GetOrCreateRow( iCurrentRow, 0, true, ExcelVersion.Excel97to2003 );
              iRowIndex = iCurrentRow;
            }

            currentRow.HasMultiRkBlank = true;

            if( bIgnoreStyles )
            {
              // TODO: implement ignore styles mode.
              throw new NotImplementedException();
            }

            if( currentRow.LastColumn < iCurrentColumn )
            {
              currentRow.AppendRecordData( /*sRecordType,*/ sRecordLen, arrData, Application.RowStorageAllocationBlockSize );
            }
            else
            {
              currentRow.InsertRecordData( iCurrentColumn, sRecordLen, arrData, Application.RowStorageAllocationBlockSize );
            }

            currentRow.UpdateColumnIndexes( iCurrentColumn, iLastColumn );
            break;

          case TBIFFRecord.LabelSST:
          case TBIFFRecord.Blank:
          case TBIFFRecord.RK:
          case TBIFFRecord.BoolErr:
          case TBIFFRecord.Formula:
          case TBIFFRecord.Label:
          case TBIFFRecord.RString:
          case TBIFFRecord.Number:
            // Here we should read row number.
            //stream.Position = lStartPosition;
            stream.Read( arrData, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen );
            sRecordLen += BiffRecordRaw.DEF_HEADER_SIZE;

            if( decryptor != null )
            {
              decryptor.Decrypt( provider, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen,
                lStartPosition + BiffRecordRaw.DEF_HEADER_SIZE );
              //throw new NotImplementedException();
            }

            //if( recordType == TBIFFRecord.Formula )
            //  FormulaRecord.UpdateOptions( arrData );

            iCurrentRow = BitConverter.ToUInt16( arrData, BiffRecordRaw.DEF_HEADER_SIZE );
            iCurrentColumn = BitConverter.ToUInt16( arrData, BiffRecordRaw.DEF_HEADER_SIZE + 2 );
            int extFormatIndex = BitConverter.ToUInt16(arrData, BiffRecordRaw.DEF_HEADER_SIZE + 4);

            WorksheetHelper.AccessColumn( m_sheet, iCurrentColumn + 1 );

            if( iRowIndex != iCurrentRow )
            {
              // We can set height to 0, since if file is correct than RowRecord
              // must be present for each row that has any data.
              int iRowHeight = m_book.AppImplementation.StandardHeightInRowUnits;
              currentRow = GetOrCreateRow( iCurrentRow, iRowHeight, true, ExcelVersion.Excel97to2003 );
              iRowIndex = iCurrentRow;
            }

            if( recordType == TBIFFRecord.LabelSST )
            {
              int iLabelIndex = BitConverter.ToInt32( arrData, LabelSSTRecord.DEF_INDEX_OFFSET + BiffRecordRaw.DEF_HEADER_SIZE );
              sst.AddIncrease( iLabelIndex );
            }

            if( currentRow.LastColumn < iCurrentColumn )
            {
              currentRow.AppendRecordData( /*sRecordType,*/ sRecordLen, arrData, Application.RowStorageAllocationBlockSize );
            }
            else
            {
              currentRow.InsertRecordData( iCurrentColumn, sRecordLen, arrData, Application.RowStorageAllocationBlockSize );
            }
            if (m_book.GetExtFormat(extFormatIndex).WrapText)
                currentRow.IsWrapText = true;
            currentRow.UpdateColumnIndexes( iCurrentColumn, iCurrentColumn );
            break;

          case TBIFFRecord.Array:
          case TBIFFRecord.String:
            stream.Read( arrData, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen );
            sRecordLen += BiffRecordRaw.DEF_HEADER_SIZE;

            if( decryptor != null )
            {
              decryptor.Decrypt( provider, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen,
                lStartPosition + BiffRecordRaw.DEF_HEADER_SIZE );
              //throw new NotImplementedException();
            }

            if (currentRow != null)
                currentRow.AppendRecordData( /*sRecordType,*/ sRecordLen, arrData, Application.RowStorageAllocationBlockSize );
            break;

          case TBIFFRecord.SharedFormula2:
            stream.Position = lStartPosition;
            SharedFormulaRecord shared = ( SharedFormulaRecord )reader.GetRecord( decryptor );
            AddSharedFormula( iCurrentRow, iCurrentColumn, shared );
            break;

          case TBIFFRecord.Row:
            // TODO: find way to change this.
            stream.Read( arrData, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen );

            if( decryptor != null )
            {
              decryptor.Decrypt( provider, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen,
                lStartPosition + BiffRecordRaw.DEF_HEADER_SIZE );
              //throw new NotImplementedException();
            }

            RowRecord row = ( RowRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Row );
            row.ParseStructure( provider, 4, 0, ExcelVersion.Excel97to2003 );
            sheet.ParseRowRecord( row, bIgnoreStyles );
            break;

          case TBIFFRecord.DBCell:
            stream.Position += sRecordLen;
            break;

          case TBIFFRecord.Table:
            stream.Read(arrData, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen);
            sRecordLen += BiffRecordRaw.DEF_HEADER_SIZE;
            TableRecord tableRecord = (TableRecord)BiffRecordFactory.GetRecord(TBIFFRecord.Table);
            tableRecord.ParseStructure(provider, BiffRecordRaw.DEF_HEADER_SIZE, sRecordLen, ExcelVersion.Excel97to2003);
            if (currentRow!= null)
                currentRow.AppendRecordData(sRecordLen, arrData, Application.RowStorageAllocationBlockSize);
            break;

          default:
            stream.Position = lStartPosition;
            return;
        }
      }
    }
    /// <summary>
    /// Adds shared formula to record table.
    /// </summary>
    /// <param name="row">Row index of the cell containing shared formula description.</param>
    /// <param name="column">Column index of the cell containing shared formula description.</param>
    /// <param name="shared">Shared formula record to add.</param>
    public void AddSharedFormula( int row, int column, SharedFormulaRecord shared )
    {
      long index = RangeImpl.GetCellIndex( column, row );
      SharedFormulas.Add( index, shared );
    }
    /// <summary>
    /// Extracts ranges from the reader.
    /// </summary>
    /// <param name="index">Index record</param>
    /// <param name="reader">Reader to extract ranges from.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    /// <param name="sst">SSTDictionary of the parent workbook.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <returns>True if parsing was succeeded (this doesn't guarantees records correctness).</returns>
    [ CLSCompliant( false ) ]
    public bool ExtractRangesFast( IndexRecord index, BiffReader reader, bool bIgnoreStyles,
      SSTDictionary sst, WorksheetImpl sheet )
    {
      if( index == null ) return false;

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Stream stream = reader.BaseStream;
      long lStartPosition = stream.Position;
      BinaryReader binaryReader = reader.BaseReader;
      DataProvider provider = reader.DataProvider;
      byte[] arrBuffer = reader.Buffer;
      DBCellRecord dbCell = ( DBCellRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DBCell );
      bool bResult = true;

      // Check whether Index record contains correct offsets to DBCell records.
      int[] arrDbCells = index.DbCells;

      for( int i = 0, len = arrDbCells.Length; ( i < len ) && bResult; i++ )
      {
        int iDBCellOffset = arrDbCells[ i ];
        stream.Position = iDBCellOffset;
        stream.Read( arrBuffer, 0, BiffRecordRaw.DEF_HEADER_SIZE );
        int iCode = BitConverter.ToInt16( arrBuffer, 0 );
        int iLength = BitConverter.ToInt16( arrBuffer, 2 );

        if( iCode != ( int )TBIFFRecord.DBCell )
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "XlsIO was unable to parse records using IndexRecord." );
          bResult = false;
          break;
        }

        stream.Read( arrBuffer, BiffRecordRaw.DEF_HEADER_SIZE, iLength );
        dbCell.Length = iLength;
        dbCell.ParseStructure( provider, BiffRecordRaw.DEF_HEADER_SIZE, iLength, ExcelVersion.Excel97to2003 );
        dbCell.StreamPos = iDBCellOffset;
        bResult = ParseDBCellRecord( dbCell, reader, bIgnoreStyles, sst, sheet );
      }

      if( !bResult )
      {
        stream.Position = lStartPosition;
        Clear();
      }

      return bResult;
    }
    /// <summary>
    /// Returns row from the collection or creates one if necessary.
    /// </summary>
    /// <param name="rowIndex">Zero-based row index.</param>
    /// <param name="height">Row height.</param>
    /// <param name="bCreate">Indicates whether to create row if it doesn't exist.</param>
    /// <param name="version">Excel version.</param>
    /// <returns>Desired row object.</returns>
    public RowStorage GetOrCreateRow( int rowIndex, int height, bool bCreate, ExcelVersion version )
    {
      RowStorage arrRow = Rows[ rowIndex ] as RowStorage;

      if( arrRow == null && bCreate )
      {
        m_arrRows[ rowIndex ] = arrRow = CreateCellCollection( rowIndex, height, version );
        AccessRow( rowIndex );
        WorksheetHelper.AccessRow( m_sheet, rowIndex + 1 );
        //arrRow.CreateDataProvider( m_book.HeapHandle );
      }
      
      if( arrRow != null && bCreate && arrRow.Provider == null )
        arrRow.CreateDataProvider( m_book.HeapHandle );

      return arrRow;
    }
    /// <summary>
    /// Ensures that array will be able to handle desired number of rows.
    /// </summary>
    /// <param name="iSize"></param>
    public void EnsureSize( int iSize )
    {
      m_arrRows.UpdateSize( iSize );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="iRowIndex"></param>
    /// <param name="iRowCount"></param>
    public void InsertIntoDefaultRows( int iRowIndex, int iRowCount )
    {
      if( iRowIndex <= m_iLastRow )
      {
        EnsureSize( m_iLastRow + iRowCount + 1 );
        m_arrRows.Insert( iRowIndex, iRowCount, m_iLastRow - iRowIndex + 1 );
        m_iLastRow += iRowCount;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dbCell"></param>
    /// <param name="reader"></param>
    /// <param name="bIgnoreStyles"></param>
    /// <param name="sst"></param>
    /// <param name="sheet"></param>
    /// <returns>True if parsing succeeded (this doesn't guarantee data correctness).</returns>
    private bool ParseDBCellRecord( DBCellRecord dbCell, BiffReader reader, bool bIgnoreStyles,
      SSTDictionary sst, WorksheetImpl sheet )
    {
      if( dbCell == null )
        throw new ArgumentNullException( "dbCell" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      RowRecord row = ( RowRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Row );
      ushort[] arrCellOffsets = dbCell.CellOffsets;
      int iRowsCount = arrCellOffsets.Length;

      if( iRowsCount == 0 ) return true;

      long lCurrentRowPos = dbCell.StreamPos - dbCell.RowOffset;
      byte[] arrBuffer = reader.Buffer;
      DataProvider provider = reader.DataProvider;
      int iBufferSize = arrBuffer.Length;
      Stream stream = reader.BaseStream;
      long lCurrentDataPos = lCurrentRowPos + arrCellOffsets[ 0 ]
        + RowRecord.DEF_RECORD_SIZE + BiffRecordRaw.DEF_HEADER_SIZE;
      int iDataSize;
      int iLength;
      bool bResult = true;

      for( int i = 1, len = iRowsCount; i < len && bResult; i++ )
      {
        stream.Position = lCurrentRowPos;
        //row = ( RowRecord )row.Clone();
        iLength = FillRowRecord( row, stream, arrBuffer, provider );

        if( iLength < 0 ) return false;

        lCurrentRowPos += iLength + BiffRecordRaw.DEF_HEADER_SIZE;
        sheet.ParseRowRecord( row, bIgnoreStyles );

        stream.Position = lCurrentDataPos;
        iDataSize = arrCellOffsets[ i ];
        lCurrentDataPos += iDataSize;
        bResult &= ReadStorageData( row, stream, iDataSize, arrBuffer, sst );
        //row.AppendRecordData(

      }

      if( bResult )
      {
        long lCurrentPosition = ( iRowsCount > 1 )
          ? stream.Position
          : lCurrentRowPos + RowRecord.DEF_RECORD_SIZE + BiffRecordRaw.DEF_HEADER_SIZE;

        iDataSize = ( int )( dbCell.StreamPos - lCurrentPosition );

        stream.Position = lCurrentRowPos;
        //row = ( RowRecord )row.Clone();
        iLength = FillRowRecord( row, stream, arrBuffer, provider );
        bResult &= ( iLength > 0 );

        if( bResult )
        {
          sheet.ParseRowRecord( row, bIgnoreStyles );
          stream.Position = lCurrentDataPos;
          bResult &= ReadStorageData( row, stream, iDataSize, arrBuffer, sst );
        }
      }

      return bResult;
    }
    /// <summary>
    /// Fills RowRecord with data from the stream.
    /// </summary>
    /// <param name="row">RowRecord to fill.</param>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="arrBuffer">Temporary buffer.</param>
    /// <param name="provider">Data provider that gives additional access to the temporary buffer.</param>
    /// <returns>Record length.</returns>
    private int FillRowRecord( RowRecord row, Stream stream, byte[] arrBuffer, DataProvider provider )
    {
      const int iBytesToRead = BiffRecordRaw.DEF_HEADER_SIZE + RowRecord.DEF_RECORD_SIZE;

      stream.Read( arrBuffer, 0, iBytesToRead );
      int iCode = BitConverter.ToInt16( arrBuffer, 0 );
      int iLength = BitConverter.ToInt16( arrBuffer, 2 );

      if( iCode != ( int )TBIFFRecord.Row )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Couldn't parse RowRecord at offset " + stream.Position );
        return -1;
      }

      if( RowRecord.DEF_RECORD_SIZE < iLength )
      {
        stream.Read( arrBuffer, iBytesToRead, iLength - RowRecord.DEF_RECORD_SIZE );
      }

      row.ParseStructure( provider, BiffRecordRaw.DEF_HEADER_SIZE, iLength, ExcelVersion.Excel97to2003 );
      return iLength;
    }
    /// <summary>
    /// Reads RowStorage data from the stream.
    /// </summary>
    /// <param name="row">Corresponding RowRecord.</param>
    /// <param name="stream">Stream to read data from.</param>
    /// <param name="iDataSize">Size of the data to read.</param>
    /// <param name="arrBuffer">Temporary buffer.</param>
    /// <param name="sst">Strings shared table which can be updated during parsing.</param>
    private bool ReadStorageData( RowRecord row, Stream stream, int iDataSize, byte[] arrBuffer,
      SSTDictionary sst )
    {
      bool bResult = true;

      if( iDataSize > 0 )
      {
        RowStorage storage = GetOrCreateRow( row.RowNumber, 0, true, ExcelVersion.Excel97to2003 );
        storage.UpdateRowInfo( row, Application.UseFastRecordParsing );
        int iBufferSize = arrBuffer.Length;

        while( iDataSize > 0 )
        {
          int iBytesToRead = Math.Min( iDataSize, iBufferSize );
          stream.Read( arrBuffer, 0, iBytesToRead );

          storage.AppendRecordData( iBytesToRead, arrBuffer, Application.RowStorageAllocationBlockSize );
          iDataSize -= iBytesToRead;
        }

        // Here we have call some method that will update row data (SharedFormula + LabelSST).
        // Also we have to update flags for MulRK/MulBlank and RK/Blank records
        bResult = storage.PrepareRowData( sst, ref m_arrShared );
      }

      return bResult;
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormulaFlags()
    {
      for( int iRow = m_iFirstRow; iRow <= m_iLastRow; iRow++ )
      {
        RowStorage arrRow = m_arrRows[ iRow ];

        if( arrRow != null )
        {
          arrRow.UpdateFormulaFlags();
        }
      }

      //UpdateArrayFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    #endregion

    #region GetValue optimize methods
    /// <summary>
    /// Gets bool value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - 0.</returns>
    public int GetBoolValue( int iRow, int iCol )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? 0
        : arrRow.GetBoolValue( iCol - 1 );
    }
    /// <summary>
    /// Gets formula bool value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - 0.</returns>
    public int GetFormulaBoolValue( int iRow, int iCol )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? 0
        : arrRow.GetFormulaBoolValue( iCol - 1 );
    }
    /// <summary>
    /// Gets error value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - null.</returns>
    public string GetErrorValue( int iRow, int iCol )
    {
      RowStorage arrRow = Rows[ iRow - 1 ] as RowStorage;

      return ( arrRow == null )
        ? null
        : arrRow.GetErrorValue( iCol - 1 );
    }
    /// <summary>
    /// Gets the error value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="iRow">The i row.</param>
    /// <returns></returns>
    internal string GetErrorValue(byte value,int iRow)
    {
        RowStorage arrRow = Rows[iRow - 1] as RowStorage;
        return (arrRow==null)
            ? null
            : arrRow.GetErrorString(value & 0xff);
    }
    /// <summary>
    /// Gets formula error value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - null.</returns>
    public string GetFormulaErrorValue( int iRow, int iCol )
    {
      RowStorage arrRow = Rows[ iRow - 1 ] as RowStorage;

      return ( arrRow == null )
        ? null
        : arrRow.GetFormulaErrorValue( iCol - 1 );
    }
    /// <summary>
    /// Gets number value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - double.NaN.</returns>
    public double GetNumberValue( int iRow, int iCol )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];
      if (arrRow == null)
          return double.NaN;
      else
      {
          arrRow.SetWorkbook(m_book, iRow);
          return arrRow.GetNumberValue(iCol - 1,m_sheet.Index);
      }    
    }
    /// <summary>
    /// Gets formula number value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns value; otherwise - double.NaN.</returns>
    public double GetFormulaNumberValue( int iRow, int iCol )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? double.NaN
        : arrRow.GetFormulaNumberValue( iCol - 1 );
    }
    /// <summary>
    /// Gets string value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <param name="sst">Represents sst dictionary.</param>
    /// <returns>If found - returns value; otherwise - null.</returns>
    public string GetStringValue( int iRow, int iCol, SSTDictionary sst )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? null
        : arrRow.GetStringValue( iCol - 1, sst );
    }
    /// <summary>
    /// Gets string value by row or column indexes. Without check input parameters.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <param name="sst">Represents sst dictionary.</param>
    /// <returns>If found - returns value; otherwise - null.</returns>
    public string GetFormulaStringValue( int iRow, int iCol, SSTDictionary sst )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? null
        : arrRow.GetFormulaStringValue( iCol - 1 );
    }
    /// <summary>
    /// Gets array of formula ptg.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>If found - returns ptg array; otherwise - null.</returns>
    public Ptg[]  GetFormulaValue( int iRow, int iCol )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      return ( arrRow == null )
        ? null
        : arrRow.GetFormulaValue( iCol - 1 );
    }
    /// <summary>
    /// Indicates if there is formula record.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iCol">One based column index.</param>
    /// <returns>Indicates whether formula record is contained.</returns>
    public bool HasFormulaRecord( int iRow, int iCol )
    {
      RowStorage arrRow = Rows[ iRow - 1 ];

      return ( arrRow == null )
        ? false
        : arrRow.HasFormulaRecord( iCol - 1 );
    }
    /// <summary>
    /// Indicates is contain formula array.
    /// </summary>
    /// <param name="iRow">Zero based row index.</param>
    /// <param name="iCol">Zero based column index.</param>
    /// <returns>If found return true; otherwise - false.</returns>
    public bool HasFormulaArrayRecord( int iRow, int iCol )
    {
      RowStorage arrRow = Rows[ iRow ];

      return ( arrRow == null )
        ? false
        : arrRow.HasFormulaArrayRecord( iCol );
    }
    /// <summary>
    /// Gets cell type from current column.
    /// </summary>
    /// <param name="row">Indicates row.</param>
    /// <param name="column">Indicates column.</param>
    /// <param name="bNeedFormulaSubType">Indicates is need to indentify formula sub type.</param>
    /// <returns>Returns cell type.</returns>
    public TRangeValueType GetCellType( int row, int column, bool bNeedFormulaSubType )
    {
      RowStorage arrRow = Rows[ row - 1 ] as RowStorage;

      return ( arrRow == null )
        ? TRangeValueType.Blank
        : arrRow.GetCellType( column - 1, bNeedFormulaSubType );
    }
    /// <summary>
    /// Sets formula value. Use for setting FormulaError, FormulaBoolean, FormulaNumber, FormulaString values.
    /// </summary>
    /// <param name="iRow">One based row index.</param>
    /// <param name="iColumn">One based column index.</param>
    /// <param name="value">Represents value for set.</param>
    /// <param name="strRecord">Represents string record as formula string value. Can be null.</param>
    [ CLSCompliant( false ) ]
    public void SetFormulaValue( int iRow, int iColumn, double value, StringRecord strRecord )
    {
      RowStorage arrRow = m_arrRows[ iRow - 1 ];

      if( arrRow == null )
        throw new ApplicationException( "Cannot sets formula value." );

      arrRow.SetFormulaValue( iColumn - 1, value, strRecord, Application.RowStorageAllocationBlockSize );
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = m_arrRows[ i ];

        if( row != null )
          row.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      for( int i = m_iFirstRow; i <= m_iLastRow; i++ )
      {
        RowStorage row = m_arrRows[ i ];

        if( row != null )
          row.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    #endregion
  }

  /// <summary>
  /// Represents enumerator for SFTable
  /// </summary>
  public class RecordTableEnumerator : IDictionaryEnumerator
  {
    #region Class members
    /// <summary>
    /// Table to enumerate.
    /// </summary>
    private CellRecordCollection m_table;
    /// <summary>
    /// One-based current row index.
    /// </summary>
    private int m_iRow = -1;
//    /// <summary>
//    /// One-based current column index.
//    /// </summary>
//    private int m_iColumn;
//    /// <summary>
//    /// Passed cells in the current row.
//    /// </summary>
//    private int m_iCellInRow = 0;
    /// <summary>
    /// SFTable to enumerate.
    /// </summary>
    private RecordTable m_sfTable;
    /// <summary>
    /// Offset inside current row.
    /// </summary>
    private int m_iOffset;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation instances of this class without arguments.
    /// </summary>
    private RecordTableEnumerator()
    {
    }
    /// <summary>
    /// Initializes new instance of the enumerator.
    /// </summary>
    /// <param name="table">Table to enumerate.</param>
    public RecordTableEnumerator( CellRecordCollection table )
    {
      if( table == null )
        throw new ArgumentNullException( "table" );

      m_table = table;
      m_sfTable = table.Table;
    }
    #endregion

    #region IEnumerator Members

    /// <summary>
    /// Sets the enumerator to its initial position, which is before
    /// the first element in the collection.
    /// </summary>
    public void Reset()
    {
      m_iRow = m_sfTable.FirstRow - 1;
      m_iOffset = 0;
      MoveNextRow();
      //m_iColumn = 0;
      //m_iCellInRow = 0;
    }

    /// <summary>
    /// Gets the current element in the collection.
    /// </summary>
    public object Current
    {
      get
      {
        if( m_iRow < 0 )
        {
          return null;
        }
        else
        {
          RowStorage arrRow = ( RowStorage )m_sfTable.Rows[ m_iRow ];

          if( arrRow == null ) return new DictionaryEntry( null, null );

          ICellPositionFormat record = ( ICellPositionFormat )arrRow.GetRecordAtOffset( m_iOffset );
          long lKey = RangeImpl.GetCellIndex( record.Column + 1, record.Row + 1 );
          return new DictionaryEntry( lKey, record );
        }
      }
    }

    /// <summary>
    /// Advances the enumerator to the next element of the collection.
    /// </summary>
    /// <returns></returns>
    public bool MoveNext()
    {
      int iLastRow = m_sfTable.LastRow;

      if( m_iRow < 0 )
      {
        m_iRow = m_sfTable.FirstRow;
        //m_iColumn = m_table.FirstColumn;
        m_iOffset = 0;

        if( m_iRow < 0 || m_iRow > iLastRow ) return false;

        RowStorage arrRow = m_sfTable.Rows[ m_iRow ];

        while( ( arrRow == null || arrRow.UsedSize <= 0 ) && m_iRow <= iLastRow )
        {
          m_iRow++;
          arrRow = m_sfTable.Rows[ m_iRow ];
        }

        return true;
      }
      else
      {
        RowStorage arrRow = ( RowStorage )m_sfTable.Rows[ m_iRow ];
		
		if(arrRow != null)
		{
			m_iOffset = arrRow.MoveNextCell( m_iOffset );

			if( m_iOffset >= arrRow.UsedSize )
			{
				return MoveNextRow();
			}

			return true;
		}
		else
			return false;
      }
    }

    /// <summary>
    /// Moves pointer to the next non-empty row if it is possible.
    /// </summary>
    /// <returns>True if operation succeeded.</returns>
    private bool MoveNextRow()
    {
      int iLastRow = m_sfTable.LastRow;
      ArrayListEx arrRows = m_sfTable.Rows;
      m_iRow++;

      while( m_iRow <= iLastRow )
      {
        RowStorage arrRow = arrRows[ m_iRow ];

        if( arrRow != null && arrRow.UsedSize > 0 )
        {
          m_iOffset = 0;
          return true;
        }

        m_iRow++;
      }

      return false;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets the key of the current dictionary entry. Read-only.
    /// </summary>
    public object Key
    {
      get
      {
        return Entry.Key;
      }
    }
    /// <summary>
    /// Gets the value of the current dictionary entry. Read-only.
    /// </summary>
    public object Value
    {
      get
      {
        return Entry.Value;
      }
    }
    /// <summary>
    /// Gets both the key and the value of the current dictionary entry. Read-only.
    /// </summary>
    public DictionaryEntry Entry
    {
      get
      {
        if( m_iRow < 0 )
          throw new InvalidOperationException();

        RowStorage arrRow = m_sfTable.Rows[ m_iRow ];
        ICellPositionFormat cell = ( ICellPositionFormat )arrRow.GetRecordAtOffset( m_iOffset );

        return new DictionaryEntry( RangeImpl.GetCellIndex( cell.Column + 1, cell.Row + 1 ),
          cell );
      }
    }
    #endregion
  }
}