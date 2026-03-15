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

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Table of all data validation in the worksheet.
  /// </summary>
  public class DataValidationTable : CollectionBaseEx<DataValidationCollection>
  {
    #region Class members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_worksheet;
    /// <summary>
    /// Dictionary, key - DValRecord, value - DataValidationCollection.
    /// </summary>
    private Dictionary<DValRecord, DataValidationCollection> m_hashDVals = new Dictionary<DValRecord, DataValidationCollection>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates DataValidationTable.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public DataValidationTable( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    /// <summary>
    /// Creates DataValidationTable.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    /// <param name="arrRecords">List with data validation records.</param>
    /// <param name="iOffset">Offset to the first data validation record.</param>
    public DataValidationTable( IApplication application, object parent,
      List<BiffRecordRaw> arrRecords, ref int iOffset )
      : this( application, parent )
    {
      Parse( arrRecords, ref iOffset );
    }

    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_worksheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_worksheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet." );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Extracts data validation table from array of records.
    /// </summary>
    /// <param name="arrRecords">List with data validation records.</param>
    /// <param name="iOffset">Offset to the first data validation record.</param>
    public void Parse( List<BiffRecordRaw> arrRecords, ref int iOffset )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );
      
      int iCount = arrRecords.Count;

      if( iOffset < 0 || iOffset > iCount )
        throw new ArgumentOutOfRangeException( "iOffset",
          "Value cannot be less than 0 or greater than arrRecords.Count." );

      do
      {
        // Use 'as' to increase performance.
        DValRecord dval = arrRecords[ iOffset ] as DValRecord;

        if( dval == null ) break;

        DataValidationCollection dvCollection = new DataValidationCollection( Application, this,
          arrRecords, ref iOffset );

        base.Add( dvCollection );
        //m_hashDVals.Add( dval, dvCollection );
        m_hashDVals[ dval ] = dvCollection;
      }
      while( iOffset < iCount );
    }
    /// <summary>
    /// Adds new data validation collection.
    /// </summary>
    /// <param name="dval">Collection to add.</param>
    /// <returns>Newly added data validation collection</returns>
    public DataValidationCollection Add( DataValidationCollection dval )
    {
      DValRecord dvalRecord = dval.Record;

      if( m_hashDVals.ContainsKey( dvalRecord ) )
      {
        return m_hashDVals[ dvalRecord ];
      }
      else
      {
        m_hashDVals.Add( dvalRecord, dval );
        base.Add( dval );

        return dval;
      }
    }
    /// <summary>
    /// Adds new data validation to the collection.
    /// </summary>
    /// <param name="dval">DValRecord to add.</param>
    /// <returns>Newly added data validation collection.</returns>
    [ CLSCompliant( false ) ]
    public DataValidationCollection Add( DValRecord dval )
    {
      if( m_hashDVals.ContainsKey( dval ) )
      {
        return m_hashDVals[ dval ];
      }
      else

      {
          DataValidationCollection dvalCollection ;
          for (int i = 0, len = base.Count; i < len; i++)
          {
              if (base[i].Worksheet.Index == this.Worksheet.Index)
              {
                  dvalCollection = base[i];
                  m_hashDVals.Add(dval, dvalCollection);
                  return dvalCollection;
              }
          }
          dvalCollection = new DataValidationCollection(
                Application, this, dval);
          m_hashDVals.Add(dval, dvalCollection);
          base.Add(dvalCollection);
          return dvalCollection;
      }
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns clone of current object.</returns>
    public override object Clone( object parent )
    {
      DataValidationTable result = ( DataValidationTable )base.Clone( parent );
      //result.QuietMode = QuietMode;
      List<DataValidationCollection> list = result.InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        DataValidationCollection validation = list[ i ];
        result.m_hashDVals.Add( validation.Record, validation );
      }

      return result;
    }
    /// <summary>
    /// Searches for corresponding data validation.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Found data validation or null.</returns>
    public DataValidationImpl FindDataValidation( long cellIndex )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        DataValidationCollection dvCollection = this[ i ];
        DataValidationImpl result = dvCollection.FindByCellIndex( cellIndex );

        if( result != null ) return result;
      }

      return null;
    }
    /// <summary>
    /// Searches for corresponding data validation.
    /// </summary>
    /// <param name="row">One-based row index.</param>
    /// <param name="column">One-based column index.</param>
    /// <returns>Found data validation or null.</returns>
    public DataValidationImpl FindDataValidation( int row, int column )
    {
      long lCellIndex = RangeImpl.GetCellIndex( column, row );
      return FindDataValidation( lCellIndex );
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        DataValidationCollection dvCollection = ( DataValidationCollection )InnerList[ i ];
        dvCollection.UpdateNamedRangeIndexes( arrNewIndex );
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="dicNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      for( int i = 0, len = Count; i < len; i++ )
      {
        DataValidationCollection dvCollection = ( DataValidationCollection )InnerList[ i ];
        dvCollection.UpdateNamedRangeIndexes( dicNewIndex );
      }
    }
    /// <summary>
    /// Removes specified rectangles from the collection.
    /// </summary>
    /// <param name="rectangles">Rectangles to remove.</param>
    public void Remove( Rectangle[] rectangles )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        this[ i ].Remove( rectangles );
      }
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<DataValidationCollection> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        DataValidationCollection validation = list[ i ];
        validation.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<DataValidationCollection> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        DataValidationCollection validation = list[ i ];
        validation.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    internal void CopyMoveTo( DataValidationTable destDataValidation,
      int iSourceRow, int iSourceColumn,
      int iDestRow, int iDestColumn,
      int iRowCount, int iColumnCount,
      bool isMove )
    {
      if( destDataValidation == null )
        throw new ArgumentNullException( "destDataValidation" );

      if( destDataValidation == this )
      {
        // We need to cache all data validations, or cache them in the case of
        // intersection and re-assign destDataValidation.
        DataValidationTable tempTable = new DataValidationTable( Application, Parent );
        CopyMoveTo( tempTable, iSourceRow, iSourceColumn, iDestRow, iDestColumn, iRowCount, iColumnCount, isMove );
        tempTable.CopyMoveTo( this, iDestRow, iDestColumn, iDestRow, iDestColumn, iRowCount, iColumnCount, isMove );
      }
      else
      {
        Rectangle sourceRect = new Rectangle( iSourceColumn - 1, iSourceRow - 1, iColumnCount - 1, iRowCount - 1 );
        Rectangle[] sourceRectangles = new Rectangle[] { sourceRect };

        Rectangle destRect = new Rectangle( iDestColumn - 1, iDestRow - 1, iColumnCount - 1, iRowCount - 1 );
        Rectangle[] destRectangles = new Rectangle[] { destRect };

        destDataValidation.Remove( destRectangles );

        foreach( DataValidationCollection dvCollection in m_hashDVals.Values )
        {
          destDataValidation.Add( dvCollection,
            iSourceRow, iSourceColumn,
            iDestRow, iDestColumn,
            iRowCount, iColumnCount );

          if( isMove )
            dvCollection.Remove( sourceRectangles );
        }
      }
    }
    private void Add( DataValidationCollection dvCollection,
      int iSourceRow, int iSourceColumn,
      int iDestRow, int iDestColumn,
      int iRowCount, int iColumnCount )
    {
      // We are creating clone in order to keep data unchanged if we add this record to our storage.
      DValRecord dval = ( DValRecord )dvCollection.Record.Clone();
      DataValidationCollection currentCollection;
      bool bAdd = false; // Indicates whether we should add current collection after all operations.

      if( !m_hashDVals.TryGetValue( dval, out currentCollection ) )
      {
        currentCollection = new DataValidationCollection( Application, this, dval );
        bAdd = true;
      }

      currentCollection.AddFrom( dvCollection, iSourceRow, iSourceColumn,
        iDestRow, iDestColumn,
        iRowCount, iColumnCount );

      if( bAdd && currentCollection.Count > 0 )
        Add( currentCollection );
    }
    protected override void OnClearComplete()
    {
        m_hashDVals.Clear();        
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets parent worksheet. Read-only.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_worksheet;
      }
    }
    /// <summary>
    /// Gets parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_worksheet.ParentWorkbook;
      }
    }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public DataValidationCollection this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count )
          throw new ArgumentOutOfRangeException( "index",
            "Value cannot be less than 0 or greater than Count." );

        return ( DataValidationCollection )List[ index ];
      }
    }
    /// <summary>
    /// Gets number of required shapes objects.
    /// </summary>
    public int ShapesCount
    {
      get
      {
        int iResult = 0;

        for( int i = 0, iCount = Count; i < iCount; i++ )
        {
          iResult += this[ i ].ShapesCount;
        }

        return iResult;
      }
    }
    #endregion
  }
}
