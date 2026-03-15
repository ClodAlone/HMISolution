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

#region File using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Interfaces;

using MergeRegion = Syncfusion.XlsIO.Parser.Biff_Records.MergeCellsRecord.MergedRegion;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class contains information about all merged
  /// cells in the parent workbook.
  /// </summary>
  public class MergeCellsImpl
    : CommonObject
    , ICloneParent
    //, IMergeCells
  {
    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Contains all not parsed merge records.
    /// </summary>
    private List<MergeCellsRecord> m_arrRecordsToParse = new List<MergeCellsRecord>();
    /// <summary>
    /// Indicates whether object was parsed.
    /// </summary>
    private bool m_bParsed = true;
    /// <summary>
    /// Cell range address list of all merged cells.
    /// </summary>
    private List<Rectangle> m_arrCells = new List<Rectangle>();
    #endregion

    #region Class Initialize methods
    /// <summary>
    /// Sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public MergeCellsImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
    }
    #endregion

    #region Class serialize method
    /// <summary>
    /// Saves merges into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList to save all records into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( m_bParsed )
      {
        int iCount = MergeCount;

        if( iCount < 0 )
          return;

        MergeRegion[] arrRegions = new MergeRegion[ iCount ];

        for( int i = 0; i < iCount; i++ )
        {
          Rectangle rect = ( Rectangle )m_arrCells[ i ];
          MergeRegion merge = RectangleToMergeRegion( rect );
          arrRegions[ i ] = merge;
        }

        int iCountLeft = iCount;
        int iStartIndex = 0;
        int iCurCount;

        while( iStartIndex != iCount )
        {
          iCurCount = iCount - iStartIndex;

          if( iCurCount > MergeCellsRecord.DEF_MAXIMUM_REGIONS )
          {
            iCurCount = MergeCellsRecord.DEF_MAXIMUM_REGIONS;
          }

          MergeCellsRecord mergeCells = ( MergeCellsRecord )BiffRecordFactory.GetRecord( TBIFFRecord.MergeCells );
          mergeCells.SetRegions( iStartIndex, iCurCount, arrRegions );
          records.Add( mergeCells );

          iStartIndex += iCurCount;
        }
      }
      else
      {
        records.AddRange( m_arrRecordsToParse );
      }
    }
    #endregion

    #region Class Helper properties
    /// <summary>
    /// Number of merges in the collection.
    /// </summary>
    internal int MergeCount
    {
      get
      {
        // TODO: parsing can be delayed here.
        Parse();
        return m_arrCells.Count;
      }
    }
    /// <summary>
    /// Collection of all merged regions.
    /// </summary>
    internal List<Rectangle> MergedRegions
    {
      get
      {
        Parse();
        return m_arrCells;
      }
    }
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void FindParents()
    {
      object parent = FindParent( typeof( WorksheetImpl ) );

      if( parent == null )
        throw new ArgumentNullException( "Can't find parent workbook" );

      m_sheet = ( WorksheetImpl ) parent;
    }
    /// <summary>
    /// Returns single merge region from the collection.
    /// </summary>
    internal MergeRegion this[ Rectangle rect ]
    {
      get
      {
        return FindMergedRegion( rect );
      }
    }
    /// <summary>
    /// Returns single merge region from the collection.
    /// </summary>
    internal Rectangle this[ int index ]
    {
      get
      {
        Parse();
        return m_arrCells[ index ];
      }
    }
    /// <summary>
    /// Gets array of custom extended formats that represents custom merged range.
    /// </summary>
    /// <returns>Returns array of Merged extended formats.</returns>
    public IList<ExtendedFormatImpl> GetMergedExtendedFormats()
    {
      Parse();

      IList<ExtendedFormatImpl> list = new List<ExtendedFormatImpl>();
      CellRecordCollection cells = m_sheet.CellRecords;

      for( int i = 0, iCount = MergeCount; i < iCount; i++ )
      {
        Rectangle rect = ( Rectangle )m_arrCells[ i ];
        MergeRegion region = RectangleToMergeRegion( rect );
        ExtendedFormatImpl format = GetFormat( region );
        list.Add( format );
      }

      return list;
    }
    /// <summary>
    /// Gets extended format describing region.
    /// </summary>
    /// <param name="region">Region to get format for.</param>
    /// <returns>Extended format describing region.</returns>
    [ CLSCompliant( false ) ]
    public ExtendedFormatImpl GetFormat( MergeRegion region )
    {
      WorkbookImpl book = ( WorkbookImpl )m_sheet.Workbook;
      CellRecordCollection cells = m_sheet.CellRecords;

      //long lFirstCellIndex = RangeImpl.GetCellIndex( region.ColumnFrom + 1, region.RowFrom + 1 );
      int iFirstXFIndex = cells.GetExtendedFormatIndex( region.RowFrom + 1, region.ColumnFrom + 1 );//cells.GetCellRecord( lFirstCellIndex ).ExtendedFormatIndex;
      int iEndXFIndex = cells.GetExtendedFormatIndex( region.RowTo + 1, region.ColumnTo + 1 );

      if( iFirstXFIndex < 0 )
        iFirstXFIndex = book.DefaultXFIndex;

      if( iEndXFIndex < 0 )
        iEndXFIndex = book.DefaultXFIndex;

      ExtendedFormatsCollection XFColl = book.InnerExtFormats;
      ExtendedFormatImpl format = XFColl.GatherTwoFormats( iFirstXFIndex, iEndXFIndex );
      return format;
    }
    #endregion

    #region IMergeCells Members
    /// <summary>
    /// Adds new merge to the existing merges.
    /// </summary>
    /// <param name="range">Range that should be merged.</param>
    /// <param name="operation">
    /// Operation type - Information on what needs to be done if some of 
    /// the cells are already merged.
    /// </param>
    public void AddMerge( RangeImpl range, ExcelMergeOperation operation )
    {
      int iFirtsRow = range.FirstRow - 1;
      int iFirtsCol = range.FirstColumn - 1;
      int iLastRow  = range.LastRow - 1;
      int iLastCol  = range.LastColumn - 1;

      Parse();
      AddMerge( iFirtsRow, iLastRow, iFirtsCol, iLastCol, operation );
    }
    /// <summary>
    /// Adds a new region if it intersects with other regions.
    /// If operation is Leave, the function leaves the old region. Otherwise
    /// it deletes old region.
    /// </summary>
    /// <param name="region">Region that will be merged.</param>
    /// <param name="operation">
    /// Operation type - tells what should be done if some of 
    /// the cells are already merged.
    /// </param>
    private void AddMerge( MergeRegion region, ExcelMergeOperation operation )
    {
      int iFirstRow = region.RowFrom;
      int iFirstCol = region.ColumnFrom;
      int iLastRow  = region.RowTo;
      int iLastCol  = region.ColumnTo;

      AddMerge( iFirstRow, iLastRow, iFirstCol, iLastCol, operation );
    }
    /// <summary>
    /// Adds new region if it intersects with other regions.
    /// </summary>
    /// <param name="RowFrom">First row to merge. Zero-based.</param>
    /// <param name="RowTo">Last row to merge. Zero-based.</param>
    /// <param name="ColFrom">First column to merge. Zero-based.</param>
    /// <param name="ColTo">Last column to merge. Zero-based.</param>
    /// <param name="operation">
    /// Operation type - tells what should be done if some of 
    /// the cells are already merged.
    /// </param>
    public void AddMerge( int RowFrom, int RowTo, int ColFrom, int ColTo, ExcelMergeOperation operation )
    {
      Rectangle newRectangle = new Rectangle( ColFrom, RowFrom, ColTo - ColFrom, RowTo - RowFrom );

      if( operation == ExcelMergeOperation.Delete )
      {
        DeleteMerge( newRectangle );
      }

      m_arrCells.Add( newRectangle );
    }
    /// <summary>
    /// Removes merge that contains a specific cell.
    /// </summary>
    /// <param name="range">
    /// Range of the cells to be removed.
    /// </param>
    public void DeleteMerge( Rectangle range )
    {
      Parse();

      List<Rectangle> lstToDelete = new List<Rectangle>();

      for( int i = 0, iCount = MergeCount; i < iCount; i++ )
      {
        Rectangle currentRange = ( Rectangle )m_arrCells[ i ];

        if( UtilityMethods.Intersects( currentRange, range ) )
        {
          lstToDelete.Add( currentRange );
        }
      }

      for( int i = 0, iCount = lstToDelete.Count; i < iCount; i++ )
      {
        m_arrCells.Remove( lstToDelete[ i ] );
      }
    }
    /// <summary>
    /// Clear all merges.
    /// </summary>
    public void Clear()
    {
      m_arrCells.Clear();
      m_arrRecordsToParse = null;
      m_bParsed = true;
    }
    #endregion

    #region Implementation methods
    /// <summary>
    /// Adds merge regions from the MergeCellsRecord.
    /// </summary>
    /// <param name="mergeRecord">Record with regions.</param>
    [ CLSCompliant( false ) ]
    public void     AddMerge( MergeCellsRecord mergeRecord )
    {
      m_arrRecordsToParse.Add( mergeRecord );
      m_bParsed = false;
    }
    /// <summary>
    /// Adds merge regions from the MergeCellsRecord.
    /// </summary>
    public void     Parse()
    {
      if( m_bParsed ) return;

      for( int j = 0, iRecordCount = m_arrRecordsToParse.Count; j < iRecordCount; j++ )
      {
        MergeCellsRecord mergeRecord = ( MergeCellsRecord )m_arrRecordsToParse[ j ];
        MergeRegion[] arrRegions = mergeRecord.Regions;

        for( int i = 0, len = arrRegions.Length; i < len; i++ )
        {
          AddMerge( arrRegions[ i ], ExcelMergeOperation.Leave );
        }
      }

      m_bParsed = true;
      m_arrRecordsToParse = null;
    }
    /// <summary>
    /// Removes row from the collection.
    /// </summary>
    /// <param name="iRowIndex">Row index to remove.</param>
    public void     RemoveRow( int iRowIndex )
    {
      if( iRowIndex < 1 || iRowIndex > m_sheet.Workbook.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex" );

      InsertRemoveRow( iRowIndex, true, 1 );
    }
    /// <summary>
    /// Removes row(s) from the collection.
    /// </summary>
    /// <param name="rowIndex">Row index to remove.</param>
    /// <param name="count">Number of rows to remove.</param>
    public void RemoveRow( int rowIndex, int count )
    {
      if( rowIndex < 1 || rowIndex > m_sheet.Workbook.MaxRowCount )
        throw new ArgumentOutOfRangeException( "rowIndex" );

      InsertRemoveRow( rowIndex, true, count );
    }
    /// <summary>
    /// Inserts row to the collection.
    /// </summary>
    /// <param name="iRowIndex">Row index to insert.</param>
    /// <param name="iRowCount">Number of row to insert.</param>
    public void     InsertRow( int iRowIndex, int iRowCount )
    {
      if( iRowIndex < 1 || iRowIndex > m_sheet.Workbook.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex" );

      InsertRemoveRow( iRowIndex, false, iRowCount );
    }
    /// <summary>
    /// Removes column from the collection
    /// </summary>
    /// <param name="iColumnIndex">Column index to remove.</param>
    public void     RemoveColumn( int iColumnIndex )
    {
      InsertRemoveColumn( iColumnIndex, true, 1 );
    }
    /// <summary>
    /// Removes columns from the collection.
    /// </summary>
    /// <param name="index">Column index to remove.</param>
    /// <param name="count">Number of columns to remove</param>
    public void RemoveColumn( int index, int count )
    {
      InsertRemoveColumn( index, true, count );
    }
    /// <summary>
    /// Inserts column into collection.
    /// </summary>
    /// <param name="iColumnIndex"></param>
    public void     InsertColumn( int iColumnIndex )
    {
      InsertRemoveColumn( iColumnIndex, false, 1 );
    }
    /// <summary>
    /// Inserts column into collection
    /// </summary>
    /// <param name="iColumnIndex">Column index to insert.</param>
    /// <param name="iColumnCount">Number of columns to insert.</param>
    public void     InsertColumn( int iColumnIndex, int iColumnCount )
    {
      InsertRemoveColumn( iColumnIndex, false, iColumnCount );
    }
    /// <summary>
    /// Removes or inserts one row from merges collection.
    /// </summary>
    /// <param name="iRowIndex">One-based row index to remove or insert.</param>
    /// <param name="isRemove">Indicates whether to remove or insert rows.</param>
    /// <param name="iRowCount">Count of row to remove.</param>
    protected void  InsertRemoveRow( int iRowIndex, bool isRemove, int iRowCount )
    {
      // iStartIndex is made to be zero-based index.
      iRowIndex--;

      if( iRowIndex < 0 || iRowIndex > m_sheet.Workbook.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowIndex" );

      // The easiest way is to clear Merges 
      // and then add all updated merges to the collection.
      Parse();

      List<Rectangle> lstOldRegions = m_arrCells;
      m_arrCells = new List<Rectangle>();

      for( int i = 0, iCount = lstOldRegions.Count; i < iCount; i++ )
      {
        Rectangle rect = ( Rectangle )lstOldRegions[ i ];
        MergeRegion region = RectangleToMergeRegion( rect );
        region = InsertRemoveRow( region, iRowIndex, isRemove, iRowCount, m_sheet.Workbook );

        if (region != null)
        {
            AddMerge(region, ExcelMergeOperation.Delete);
            if (!isRemove && (region.RowFrom +1 == iRowIndex))
            {
                for (int mergeRegionCount = 1; mergeRegionCount <= iRowCount; mergeRegionCount++)
                {
                    region.RowFrom = region.RowFrom + 1;
                    region.RowTo = region.RowTo + 1;
                    AddMerge(region, ExcelMergeOperation.Delete);
                }
            }
        }
      }
    }
    /// <summary>
    /// Removes or inserts one column from merges collection.
    /// </summary>
    /// <param name="iColumnIndex">One-based column index to remove.</param>
    /// <param name="isRemove">Indicates whether to remove or insert rows.</param>
    /// <param name="iCount">Number of columns to insert or remove.</param>
    protected void  InsertRemoveColumn( int iColumnIndex, bool isRemove, int iCount )
    {
      // iColumnIndex is made to be zero-based.
      iColumnIndex--;

      if( iColumnIndex < 0 || iColumnIndex >= m_sheet.Workbook.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumnIndex" );

      if( iCount < 0 || iCount >= m_sheet.Workbook.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iCount" );

      // The easiest way is to clear Merges 
      // and then add all updated merges to the collection.
      Parse();

      List<Rectangle> lstOldRegions = m_arrCells;
      m_arrCells = new List<Rectangle>();

      for( int i = 0, iRegionCount = lstOldRegions.Count; i < iRegionCount; i++ )
      {
        Rectangle rect = ( Rectangle )lstOldRegions[ i ];
        MergeRegion region = RectangleToMergeRegion( rect );
        region = InsertRemoveColumn( region, iColumnIndex, isRemove, iCount, m_sheet.Workbook );

        if( region != null )
          AddMerge( region, ExcelMergeOperation.Delete );
      }
    }
    ///// <summary>
    ///// Returns hashtable with all merges from specified range
    ///// (key means index of the top-left corner of merge region, value - bottom-right cell).
    ///// </summary>
    ///// <param name="range">Range to get merges from.</param>
    ///// <returns>
    ///// Dictionary with all merges from specified range.
    ///// Null if at least one merge isn't contained in the range completely.</returns>
    //public Dictionary<long, long> GetMerges( IRange range )
    //{
    //  Parse();
    //  Dictionary<long, long> hashStartCells = new Dictionary<long, long>();

    //  for( int i = range.Row, lastRow = range.LastRow; i <= lastRow; i++ )
    //  {
    //    long lIndex = RangeImpl.GetCellIndex( range.Column, i );

    //    for( int j = range.Column, lastCol = range.LastColumn; j <= lastCol; j++, lIndex++ )
    //    {
    //      long lTopLeftCell;

    //      if( m_hashCells.TryGetValue( lIndex, out lTopLeftCell ) )
    //      {
    //        if( !hashStartCells.ContainsKey( lTopLeftCell ) )
    //        {
    //          MergeRegion region = ( MergeRegion )m_hashRangions[ lTopLeftCell ];
            
    //          if( region.RowFrom + 1 < range.Row || region.RowTo + 1 > lastRow
    //            || region.ColumnFrom + 1 < range.Column || region.ColumnTo + 1 > lastCol )
    //            return null;

    //          long lBottomRightCell = RangeImpl.GetCellIndex( region.ColumnTo + 1, region.RowTo + 1 );
    //          hashStartCells.Add( lTopLeftCell, lBottomRightCell );
    //        }
    //      }
    //    }
    //  }

    //  return hashStartCells;
    //}
    /// <summary>
    /// Moves merges from source range into destination.
    /// </summary>
    /// <param name="destination">Destination range.</param>
    /// <param name="source">Source range.</param>
    /// <param name="bIsMove">Indicates whether we are moving merges (true) or copying (false).</param>
    public void CopyMoveMerges( IRange destination, IRange source, bool bIsMove )
    {
      if( destination == null )
        throw new ArgumentNullException( "destination" );

      if( source == null )
        throw new ArgumentNullException( "source" );

      Parse();
      int iRowDelta = destination.Row - source.Row;
      int iColumnDelta = destination.Column - source.Column;

      //CheckRanges( destination, source );
      //Dictionary<long, MergeRegion> hashRegions = new Dictionary<long, MergeRegion>();
      List<MergeRegion> lstRegions = new List<MergeRegion>();
      CacheMerges( source, lstRegions );

      if( bIsMove )
        RemoveCache( lstRegions );

      // Add cache at new location.
      WorksheetImpl sheet = ( WorksheetImpl )source.Worksheet;
      sheet.MergeCells.AddCache( lstRegions, iRowDelta, iColumnDelta );
    }
    /// <summary>
    /// Finds all merged ranges for specific range.
    /// </summary>
    /// <param name="range">Range to find merges in.</param>
    /// <param name="bIsMove">Indicates whether delete merges from the collection is needed or not.</param>
    /// <returns>Merges list.</returns>
    [ CLSCompliant( false ) ]
    public List<MergeRegion> FindMergesToCopyMove( IRange range, bool bIsMove )
    {
      List<MergeRegion> lstRegions = new List<MergeRegion>();
      CacheMerges( range, lstRegions );

      if( bIsMove )
        RemoveCache( lstRegions );

      return lstRegions;
    }
    /// <summary>
    /// Caches range's merges.
    /// </summary>
    /// <param name="range">Range for which we have to cache merged regions.</param>
    /// <param name="lstRegions">List to save merges in.</param>
    [ CLSCompliant( false ) ]
    public void CacheMerges( IRange range, List<MergeRegion> lstRegions )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      if( lstRegions == null )
        throw new ArgumentNullException( "lstRegions" );

      Parse();
      int iFirstRow = range.Row - 1;
      int iFirstColumn = range.Column - 1;

      int iLastRow = range.LastRow - 1;
      int iLastColumn = range.LastColumn - 1;

      for( int iRow = iFirstRow; iRow <= iLastRow; iRow++ )
      {
        for( int iColumn = iFirstColumn; iColumn <= iLastColumn; iColumn++ )
        {
          Rectangle rect = new Rectangle( iColumn, iRow, 0, 0 );
          MergeRegion region = FindMergedRegion( rect );

          if( region != null && !lstRegions.Contains( region ) )
          {
            lstRegions.Add( region );
          }
        }
      }
    }
    /// <summary>
    /// Checks whether specified region is fully inside range.
    /// </summary>
    /// <param name="region">Region to check.</param>
    /// <param name="range">Range to check.</param>
    private static void CheckRegion( MergeRegion region, IRange range )
    {
      if( region == null )
        throw new ArgumentNullException( "region" );

      if( range == null )
        throw new ArgumentNullException( "range" );

      int iFirstRow = range.Row;
      int iFirstColumn = range.Column;
      int iLastRow = range.LastRow;
      int iLastColumn = range.LastColumn;

      if( region.ColumnFrom < iFirstColumn
        || region.ColumnTo > iLastColumn
        || region.RowFrom < iFirstRow
        || region.RowTo > iLastRow )
      {
        throw new ArgumentOutOfRangeException( "region" );
      }
    }
    /// <summary>
    /// Removes all cached items from this collection.
    /// </summary>
    /// <param name="lstRegions">Cached items to remove.</param>
    private void RemoveCache( List<MergeRegion> lstRegions )
    {
      if( lstRegions == null )
        throw new ArgumentNullException( "lstRegions" );

      Parse();

      foreach( MergeRegion region in lstRegions )
      {
        Rectangle rect = Rectangle.FromLTRB( region.ColumnFrom, region.RowFrom, region.ColumnTo, region.RowTo );
        m_arrCells.Remove( rect );
      }
    }
    /// <summary>
    /// Adds cache into collection.
    /// </summary>
    /// <param name="lstRegions">List with merge regions to add.</param>
    /// <param name="iRowDelta">Row delta.</param>
    /// <param name="iColDelta">Column delta.</param>
    [ CLSCompliant( false ) ]
    public void AddCache( List<MergeRegion> lstRegions, int iRowDelta, int iColDelta )
    {
      if( lstRegions == null )
        throw new ArgumentNullException( "lstRegions" );

      Parse();

      foreach( MergeRegion region in lstRegions )
      {
        region.MoveRegion( iRowDelta, iColDelta );
        AddMerge( region, ExcelMergeOperation.Delete );
      }
    }
    /// <summary>
    /// Adds dictionary with that describes merge region to the collection.
    /// </summary>
    /// <param name="dictMerges">
    /// Regions to add, key - top-left cell index, value - bottom-right cell index.
    /// </param>
    /// <param name="iRowDelta">Row delta.</param>
    /// <param name="iColumnDelta">Column delta.</param>
    public void AddMerges( IDictionary dictMerges, int iRowDelta, int iColumnDelta )
    {
      if( dictMerges == null )
        throw new ArgumentNullException( "dictMerges" );

      Parse();

      foreach( DictionaryEntry entry in dictMerges )
      {
        long iTopLeftCell = ( long )entry.Key;
        long iBottomRightCell = ( long )entry.Value;

        int iRowFrom = RangeImpl.GetRowFromCellIndex( iTopLeftCell );
        int iColFrom = RangeImpl.GetColumnFromCellIndex( iTopLeftCell );

        int iRowTo = RangeImpl.GetRowFromCellIndex( iBottomRightCell );
        int iColTo = RangeImpl.GetColumnFromCellIndex( iBottomRightCell );

        AddMerge( iRowFrom + iRowDelta - 1, iRowTo + iRowDelta - 1,
          iColFrom + iColumnDelta - 1, iColTo + iColumnDelta - 1, ExcelMergeOperation.Delete );
      }
    }
    /// <summary>
    /// Finds left top cell of merged region.
    /// </summary>
    /// <param name="rect">Merged region rectangle.</param>
    /// <returns>Left top cell range.</returns>
    public Rectangle GetLeftTopCell( Rectangle rect )
    {
      MergeRegion region = FindMergedRegion( rect );

      return ( region != null ) ? new Rectangle( region.ColumnFrom, region.RowFrom, 0, 0 ) : Rectangle.FromLTRB( -1, -1, -1, -1 );//Rectangle.Empty;
    }
    ///// <summary>
    ///// Indicate if collection contain cell index.
    ///// </summary>
    ///// <param name="lCellIndex">Cell index to check</param>
    ///// <returns>Returns true if contain; otherwise - false.</returns>
    //public bool ContainCellIndex( long lCellIndex )
    //{
    //  Parse();
    //  return m_hashCells.ContainsKey( lCellIndex );
    //}
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>A copy of the current object.</returns>
    public object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      MergeCellsImpl result = ( MergeCellsImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();

      result.m_arrCells = CloneList( m_arrCells );
      result.m_arrRecordsToParse = CloneUtils.CloneCloneable( m_arrRecordsToParse );

      return result;
    }
    /// <summary>
    /// Creates copy of the specified list.
    /// </summary>
    /// <param name="list">List to clone.</param>
    /// <returns>A copy of the specified list.</returns>
    private List<Rectangle> CloneList( List<Rectangle> list )
    {
      int iCount = list.Count;
      List<Rectangle> result = new List<Rectangle>( iCount );

      for( int i = 0; i < iCount; i++ )
        result.Add( list[ i ] );

      return result;
    }
    /// <summary>
    /// Sets new dimensions - removes unnecessary items (that are out of bounds) or truncates them.
    /// </summary>
    /// <param name="newRowCount">New maximum possible row count.</param>
    /// <param name="newColumnCount">New maximum possible column count.</param>
    public void SetNewDimensions( int newRowCount, int newColumnCount )
    {
      newRowCount--;
      newColumnCount--;
      Parse();

      List<Rectangle> lstOldRegions = m_arrCells;
      m_arrCells = new List<Rectangle>();

      for( int i = 0, iRegionCount = lstOldRegions.Count; i < iRegionCount; i++ )
      {
        Rectangle rect = ( Rectangle )lstOldRegions[ i ];
        MergeRegion region = RectangleToMergeRegion( rect );
        region.RowTo = Math.Min( region.RowTo, newRowCount );
        region.ColumnTo = Math.Min( region.ColumnTo, newColumnCount );

        if( region.CellsCount > 1 )
          AddMerge( region, ExcelMergeOperation.Delete );
      }
    }
    /// <summary>
    /// Finds merged region for specific cell.
    /// </summary>
    /// <param name="rectangle">Cell range address.</param>
    /// <returns>Merged region that includes specified cell.</returns>
    [ CLSCompliant( false ) ]
    public MergeRegion FindMergedRegion( Rectangle rectangle )
    {
      MergeRegion region = null;

      for( int i = 0, iCount = MergeCount; i < iCount; i++ )
      {
        Rectangle currentRectangle = ( Rectangle )m_arrCells[ i ];

        if( UtilityMethods.Intersects( currentRectangle, rectangle ) )
        {
          region = RectangleToMergeRegion( currentRectangle );
          break;
        }
      }

      return region;
    }
    /// <summary>
    /// Converts rectangle to merged region.
    /// </summary>
    /// <param name="rect">Rectangle to convert.</param>
    /// <returns>Merged region.</returns>
    [ CLSCompliant( false ) ]
    public MergeRegion RectangleToMergeRegion( Rectangle rect )
    {
      int iFirstCol = rect.X;
      int iFirstRow = rect.Y;
      int iLastCol = rect.Right;
      int iLastRow = rect.Bottom;

      return new MergeRegion( iFirstRow, iLastRow, iFirstCol, iLastCol );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Inserts or removes row into the merge region before first row.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove row operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [CLSCompliant( false )]
    public static MergeRegion InsertRemoveRowLower( MergeRegion region, bool isRemove,
      int iRowIndex, int iRowCount, IWorkbook book )
    {
      if( iRowCount <= 0 || iRowCount > book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowCount" );

      int iDelta = isRemove ? -iRowCount : iRowCount;

      int iRowFrom = region.RowFrom + iDelta;

      if( iRowFrom >= book.MaxRowCount )
        return null;

      if( iRowFrom < iRowIndex ) iRowFrom = iRowIndex;
      int iRowTo = region.RowTo + iDelta;

      if( iRowTo < iRowIndex ) return null;

      iRowFrom = NormalizeRow( iRowFrom, book );
      iRowTo = NormalizeRow( iRowTo, book );
      
      return new MergeRegion( iRowFrom, iRowTo, region.ColumnFrom, region.ColumnTo );
    }
    /// <summary>
    /// Inserts or removes row into region the merge at the first row.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove row operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveRowStart( MergeRegion region, bool isRemove,
      int iRowCount, IWorkbook book )
    {
      if( iRowCount <= 0 || iRowCount > book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowCount" );

      int iDelta = isRemove ? -iRowCount : iRowCount;
      int iRowTo = region.RowTo + iDelta;
      int iRowFrom = isRemove ? region.RowFrom : region.RowFrom + iDelta;

      if( iRowTo < region.RowFrom ) return null;

      if( iRowFrom >= book.MaxRowCount ) return null;

      iRowFrom = NormalizeRow( iRowFrom, book );
      iRowTo = NormalizeRow( iRowTo, book );

      return new MergeRegion( iRowFrom, iRowTo, region.ColumnFrom, region.ColumnTo );
    }
    /// <summary>
    /// Inserts or removes row into the merge region in the middle or at the end of the range.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove row operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveRowMiddleEnd( MergeRegion region, bool isRemove,
      int iRowIndex, int iRowCount, IWorkbook book )
    {
      if( iRowCount <= 0 || iRowCount > book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRowCount" );

      int iDelta = isRemove ? -iRowCount : iRowCount;
      int iRowTo = region.RowTo + iDelta;

      if( iRowTo < iRowIndex ) iRowTo = iRowIndex - 1;

      iRowTo = NormalizeRow( iRowTo, book );
      
      return new MergeRegion( region.RowFrom, iRowTo, region.ColumnFrom, region.ColumnTo );
    }
    /// <summary>
    /// Inserts or removes row after end of the region.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove row operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveRowAbove( MergeRegion region, bool isRemove, int iRowCount )
    {
      return new MergeRegion( region.RowFrom, region.RowTo,
        region.ColumnFrom, region.ColumnTo );
    }
    /// <summary>
    /// Inserts or removes row.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove row operation.</param>
    /// <param name="iRowIndex">Row index.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iRowCount">Number of rows to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns></returns>
    [ CLSCompliant( false ) ]
    static public MergeRegion InsertRemoveRow( MergeRegion region, int iRowIndex, bool isRemove,
      int iRowCount, IWorkbook book )
    {
      if( region.RowFrom == 0 && region.RowTo == book.MaxRowCount - 1 )
      {
        return new MergeRegion( region.RowFrom, region.RowTo,
          region.ColumnFrom, region.ColumnTo );

      }
      else if( region.RowFrom > iRowIndex ) // whole merge is lower than the row
      {
        return InsertRemoveRowLower( region, isRemove, iRowIndex, iRowCount, book );
      }
      else if( region.RowFrom == iRowIndex ) // at the start
      {
        return InsertRemoveRowStart( region, isRemove, iRowCount, book );
      }
      else if( region.RowFrom < iRowIndex && region.RowTo > iRowIndex ) // middle
      {
        return InsertRemoveRowMiddleEnd( region, isRemove, iRowIndex, iRowCount, book );
      }
      else if( region.RowTo == iRowIndex ) // at the end
      {
        return InsertRemoveRowMiddleEnd( region, isRemove, iRowIndex, iRowCount, book );
      }
      else if ( region.RowTo < iRowIndex ) // whole merge is above the row
      {
        return InsertRemoveRowAbove( region, isRemove, iRowCount );
      }

      return null;
    }
    /// <summary>
    /// Inserts or removes column into the merge region before first column.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove column operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveColumnLower( MergeRegion region, bool isRemove,
      int iColumnIndex, int iCount, IWorkbook book )
    {
      if( iCount <= 0 || iCount > book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iCount" );

      int iDelta = isRemove ? -iCount : iCount;

      int iColumnFrom = region.ColumnFrom + iDelta;

      if( iColumnFrom >= book.MaxColumnCount )
        return null;

      if( iColumnFrom < iColumnIndex ) iColumnFrom = iColumnIndex;
      int iColumnTo = region.ColumnTo + iDelta;

      if( iColumnTo < iColumnIndex ) return null;

      iColumnFrom = NormalizeColumn( iColumnFrom, book );
      iColumnTo = NormalizeColumn( iColumnTo, book );
      return new MergeRegion( region.RowFrom, region.RowTo, iColumnFrom, iColumnTo );
    }
    /// <summary>
    /// Inserts or removes column into the merge region at the first column.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove column operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveColumnStart( MergeRegion region,
      bool isRemove, int iCount, IWorkbook book )
    {
      if( iCount <= 0 || iCount > book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iCount" );

      int iDelta = isRemove ? -iCount : iCount;
      int iColumnTo = region.ColumnTo + iDelta;
      int iColumnFrom = isRemove ? region.ColumnFrom : region.ColumnFrom + iDelta;

      if( iColumnTo < region.ColumnFrom ) return null;

      if( iColumnFrom >= book.MaxColumnCount ) return null;

      iColumnFrom = NormalizeRow( iColumnFrom, book );
      iColumnTo = NormalizeRow( iColumnTo, book );

      return new MergeRegion( region.RowFrom, region.RowTo, iColumnFrom, iColumnTo );
    }
    /// <summary>
    /// Inserts or removes column into the merge region in the middle or at the end of the range.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove column operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveColumnMiddleEnd( MergeRegion region, bool isRemove,
      int iColumnIndex, int iCount, IWorkbook book )
    {
      if( iCount <= 0 || iCount > book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iCount" );

      int iDelta = isRemove ? -iCount : iCount;
      int iColumnTo = region.ColumnTo + iDelta;

      if( iColumnTo < iColumnIndex ) iColumnTo = iColumnIndex - 1;

      iColumnTo = NormalizeColumn( iColumnTo, book );
      
      return new MergeRegion( region.RowFrom, region.RowTo, region.ColumnFrom, iColumnTo );
    }
    /// <summary>
    /// Inserts or removes column after end of the region.
    /// </summary>
    /// <param name="region">Region to modify after insert/remove column operation.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion  InsertRemoveColumnAbove( MergeRegion region, bool isRemove, int iCount )
    {
      return new MergeRegion( region.RowFrom, region.RowTo,
        region.ColumnFrom, region.ColumnTo );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="region">Region to modify after insert/remove column operation.</param>
    /// <param name="iColumnIndex">Column index.</param>
    /// <param name="isRemove">Indicates whether it is remove operation.</param>
    /// <param name="iCount">Number of columns to insert.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Modified region.</returns>
    [ CLSCompliant( false ) ]
    public static MergeRegion InsertRemoveColumn( MergeRegion region,
      int iColumnIndex, bool isRemove, int iCount, IWorkbook book )
    {
      if( region.ColumnFrom == 0 && region.ColumnTo == book.MaxColumnCount - 1 )
      {
        return new MergeRegion( region );
      }
      if( region.ColumnFrom > iColumnIndex ) // whole merge is lower then the row
      {
        return InsertRemoveColumnLower( region, isRemove,  iColumnIndex, iCount , book);
      }
      else if( region.ColumnFrom == iColumnIndex ) // at the start
      {
        return InsertRemoveColumnStart( region, isRemove, iCount, book );
      }
      else if( region.ColumnFrom < iColumnIndex && region.ColumnTo > iColumnIndex ) // middle
      {
        return InsertRemoveColumnMiddleEnd( region, isRemove, iColumnIndex, iCount, book );
      }
      else if( region.ColumnTo == iColumnIndex ) // at the end
      {
        return InsertRemoveColumnMiddleEnd( region, isRemove, iColumnIndex, iCount, book );
      }
      else if ( region.ColumnTo < iColumnIndex ) // whole merge is above the row
      {
        return InsertRemoveColumnAbove( region, isRemove, iCount );
      }

      return null;
    }
    /// <summary>
    /// Ensures that row index is in correct range.
    /// </summary>
    /// <param name="iRowIndex">Row index to check.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Row index from the correct range.</returns>
    [ CLSCompliant( false ) ]
    public static int NormalizeRow( int iRowIndex, IWorkbook book )
    {
      if( iRowIndex < 0 ) return 0;

      return Math.Min( iRowIndex, book.MaxRowCount - 1 );
    }
    /// <summary>
    /// Ensures that column index is in correct range.
    /// </summary>
    /// <param name="iColumnIndex">Column index to check.</param>
    /// <param name="book">Workbook where operation is performed.</param>
    /// <returns>Column index from the correct range.</returns>
    [ CLSCompliant( false ) ]
    public static int NormalizeColumn( int iColumnIndex, IWorkbook book )
    {
      if( iColumnIndex < 0 ) return 0;

      return Math.Min( iColumnIndex, book.MaxColumnCount - 1 );
    }
    #endregion
  }
}
