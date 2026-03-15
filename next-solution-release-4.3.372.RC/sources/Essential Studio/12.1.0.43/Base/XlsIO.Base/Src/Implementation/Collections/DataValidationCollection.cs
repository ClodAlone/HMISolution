#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser.Biff_Records;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Incapsulates one DValRecord and multiple DVRecords from xls file.
  /// </summary>
  public class DataValidationCollection
    : CollectionBaseEx<DataValidationImpl>
  {
    #region Class members
    /// <summary>
    /// Single DValRecord from data validation block.
    /// </summary>
    private DValRecord m_dvalRecord;
    /// <summary>
    /// Parent data validation table.
    /// </summary>
    private DataValidationTable m_parentTable;
    /// <summary>
    /// Key - DVRecord, value - corresponding DataValidationImpl.
    /// </summary>
    private Dictionary<DVRecord, DataValidationImpl> m_hashRecords = new Dictionary<DVRecord, DataValidationImpl>();
    /// <summary>
    /// Represents array list that storage dv data.
    /// </summary>
    private List<BiffRecordRaw> m_arrStorage;
    /// <summary>
    /// Indicates is delay parsing.
    /// </summary>
    private bool m_bIsDelay;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public DataValidationCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="dval"></param>
    [ CLSCompliant( false ) ]
    public DataValidationCollection( IApplication application, object parent, DValRecord dval )
      : this( application, parent )
    {
      m_dvalRecord = ( DValRecord )dval.Clone();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="arrRecords"></param>
    /// <param name="iOffset"></param>
    public DataValidationCollection( IApplication application, object parent,
      List<BiffRecordRaw> arrRecords, ref int iOffset )
      : this( application, parent )
    {
      Parse( arrRecords, ref iOffset, false );
    }
    /// <summary>
    /// Searches and sets all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_parentTable = FindParent( typeof( DataValidationTable ) ) as DataValidationTable;

      if( m_parentTable == null )
        throw new ArgumentNullException( "Can't find parent table." );
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Adds data validation to the collection.
    /// Checks if same data validation exists in the collection,
    /// updates and returns it (if found).
    /// </summary>
    /// <param name="dv">Data validation to add.</param>
    /// <returns>Data validation that was added or that was updated.</returns>
    public DataValidationImpl Add( DataValidationImpl dv )
    {
      if( m_bIsDelay )
      {
        int iIndex = 0;
        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
      }

      DVRecord record = dv.DVRecord;

      if( m_hashRecords.ContainsKey( record ) )
      {
        DataValidationImpl originalDV = m_hashRecords[ record ];

        if( originalDV != dv )
        {
          originalDV.AddRange( dv );
        }

        return originalDV;
      }
      else
      {
        m_hashRecords.Add( record, dv );
        base.Add( dv );

        return dv;
      }
    }
    /// <summary>
    /// Saves collection into list of BiffRecords.
    /// </summary>
    /// <param name="records">OffsetArrayList with BiffRecords.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If records parameter is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_bIsDelay )
      {
        if( m_arrStorage.Count != m_dvalRecord.DVNumber )
          throw new ApplicationException( "Cannot find data validation entries." );

        records.Add( m_dvalRecord );
        records.AddList( m_arrStorage );
      }
      else
      {
        if( Count == 0 ) return;

        m_dvalRecord.DVNumber = ( uint )Count;
        records.Add( m_dvalRecord );

        for( int i = 0, len = Count; i < len; i++ )
        {
          DataValidationImpl dataValidation = ( DataValidationImpl )List[ i ];
          dataValidation.Serialize( records );
        }
      }
    }
    /// <summary>
    /// Creates collection from array.
    /// </summary>
    /// <param name="arrRecords">Array of Biff records.</param>
    /// <param name="iOffset">Start offset in the array (will be updated after parsing).</param>
    /// <param name="bIsParse">Indicates is parse on create or delay for first usage.</param>
    private void Parse( List<BiffRecordRaw> arrRecords, ref int iOffset, bool bIsParse )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      if( iOffset < 0 || iOffset > arrRecords.Count )
        throw new ArgumentOutOfRangeException( "iOffset",
          "Value cannot be less than 0 or greater than arrRecords.Count." );

      m_dvalRecord = arrRecords[ iOffset ] as DValRecord;
      iOffset++;

      if( m_dvalRecord == null )
        throw new ArgumentNullException( "Can't find DVal record at the specified position." );

      int iCount = ( int )m_dvalRecord.DVNumber;

      if( bIsParse )
      {
        UpdateRecords( arrRecords, ref iOffset, iCount );
      }
      else
      {
        m_arrStorage = new List<BiffRecordRaw>( iCount );
        m_arrStorage.AddRange( arrRecords.GetRange( iOffset, iCount ) );
        m_bIsDelay = true;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dv"></param>
    public void Remove( DataValidationImpl dv )
    {
      if( m_bIsDelay )
      {
        int iIndex = 0;
        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
      }

      int index = List.IndexOf( dv );

      if( index >= 0 )
      {
        base.Remove( dv );
        m_hashRecords.Remove( dv.DVRecord );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangles"></param>
    public void Remove( Rectangle[] rectangles )
    {
      if( Count > 0 )
      {
        for( int i = Count - 1; i >= 0; i-- )
        {
          this[ i ].RemoveRange( rectangles );
        }
      }
    }
    /// <summary>
    /// Clones current instance.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <returns>Returns new object.</returns>
    public override object Clone( object parent )
    {
      DataValidationCollection result = ( DataValidationCollection )base.Clone( parent );
      result.m_dvalRecord = ( DValRecord )CloneUtils.CloneCloneable( m_dvalRecord );

      if( m_bIsDelay )
      {
        result.m_bIsDelay = m_bIsDelay;
        result.m_arrStorage = CloneUtils.CloneCloneable( m_arrStorage );
      }
      else
      {
        List<DataValidationImpl> list = result.InnerList;

        for( int i = 0, len = list.Count; i < len; i++ )
        {
          DataValidationImpl validation = list[ i ];
          result.m_hashRecords.Add( validation.DVRecord, validation );
        }
      }

      return result;
    }
    /// <summary>
    /// Returns data validation for specified cell.
    /// </summary>
    /// <param name="cellIndex">Cell index.</param>
    /// <returns>Data validation for specified cell.</returns>
    public DataValidationImpl FindByCellIndex( long cellIndex )
    {
      List<DataValidationImpl> list = InnerList;

      if( m_bIsDelay )
      {
        int iIndex = 0;
        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
      }

      for( int i = 0, len = Count; i < len; i++ )
      {
        DataValidationImpl result = list[ i ];

        if( result.ContainsCell( cellIndex ) ) return result;
      }

      return null;
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      if( m_bIsDelay )
      {
//        int iIndex = 0;
//        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
        FormulaUtil parser = Workbook.FormulaUtil;

        for( int i = 0, len = m_arrStorage.Count; i < len; i++ )
        {
          DVRecord dv = ( DVRecord )m_arrStorage[ i ];
          parser.UpdateNameIndex( dv.FirstFormulaTokens, arrNewIndex );
          parser.UpdateNameIndex( dv.SecondFormulaTokens, arrNewIndex );
        }
      }

      m_hashRecords.Clear();

      for( int i = 0, len = Count; i < len; i++ )
      {
        DataValidationImpl dv = ( DataValidationImpl )InnerList[ i ];
        dv.UpdateNamedRangeIndexes( arrNewIndex );

        m_hashRecords.Add( dv.DVRecord, dv );
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

      if( m_bIsDelay )
      {
        FormulaUtil parser = Workbook.FormulaUtil;

        for( int i = 0, len = m_arrStorage.Count; i < len; i++ )
        {
          DVRecord dv = ( DVRecord )m_arrStorage[ i ];
          parser.UpdateNameIndex( dv.FirstFormulaTokens, dicNewIndex );
          parser.UpdateNameIndex( dv.SecondFormulaTokens, dicNewIndex );
        }
      }
      else
      {
        m_hashRecords.Clear();

        for( int i = 0, len = Count; i < len; i++ )
        {
          DataValidationImpl dv = ( DataValidationImpl )InnerList[ i ];
          dv.UpdateNamedRangeIndexes( dicNewIndex );

          m_hashRecords.Add( dv.DVRecord, dv );
        }
      }
    }
    /// <summary>
    /// Adds new DVRecord to the collection.
    /// </summary>
    /// <param name="dv">DVRecord to add.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If dv is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public DataValidationImpl AddDVRecord( DVRecord dv )
    {
      if( dv == null )
        throw new ArgumentNullException( "dv" );

      if( m_bIsDelay )
      {
        int iIndex = 0;
        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
      }

      return AddLocalRecord( dv );
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<DataValidationImpl> list = InnerList;
      for( int i = 0, len = list.Count; i < len; i++ )
      {
        DataValidationImpl dataValidation = list[ i ];
        dataValidation.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<DataValidationImpl> list = InnerList;
      for( int i = 0, len = list.Count; i < len; i++ )
      {
        DataValidationImpl dataValidation = list[ i ];
        dataValidation.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Updates collection.
    /// </summary>
    /// <param name="arrRecords">Array of Biff records.</param>
    /// <param name="iOffset">Start offset in the array (will be updated after parsing).</param>
    /// <param name="iCount">Represents count of records in collection.</param>
    private void UpdateRecords( List<BiffRecordRaw> arrRecords, ref int iOffset, int iCount )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      if( arrRecords.Count > iCount + iOffset )
        throw new ArgumentOutOfRangeException( "iCount" );

      for( uint i = 0; i < iCount; i++, iOffset++ )
      {
        DVRecord dv = arrRecords[ iOffset ] as DVRecord;

        if( dv == null )
          throw new ArgumentNullException( "Not enough DVRecords" );

        AddLocalRecord( dv );
      }

      m_arrStorage = null;
      m_bIsDelay = false;
    }
    /// <summary>
    /// Adds local data validation record.
    /// </summary>
    /// <param name="dv">Represents record to add.</param>
    /// <returns>Returns new instance of data validation.</returns>
    private DataValidationImpl AddLocalRecord( DVRecord dv )
    {
      if( dv == null )
        throw new ArgumentNullException( "dv" );

      if( !m_hashRecords.ContainsKey( dv ) )
      {
        DataValidationImpl dataValidation = AppImplementation.CreateDataValidationImpl( this, dv );

        m_hashRecords.Add( dv, dataValidation );
        base.Add( dataValidation );
        return dataValidation;
      }
      else
      {
        DataValidationImpl data = m_hashRecords[ dv ];

        return data;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns data validation records.
    /// </summary>
    public List<BiffRecordRaw> DataValidations
    {
      get
      {
        return m_arrStorage;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxHPosition
    {
      get
      {
        return m_dvalRecord.PromtBoxHPos;
      }
      set
      {
        m_dvalRecord.PromtBoxHPos = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int PromptBoxVPosition
    {
      get
      {
        return m_dvalRecord.PromtBoxVPos;
      }
      set
      {
        m_dvalRecord.PromtBoxVPos = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxVisible
    {
      get
      {
        return m_dvalRecord.IsPromtBoxVisible;
      }
      set
      {
        m_dvalRecord.IsPromtBoxVisible = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool IsPromptBoxPositionFixed
    {
      get
      {
        return m_dvalRecord.IsPromtBoxPosFixed;
      }
      set
      {
        m_dvalRecord.IsPromtBoxPosFixed = value;
      }
    }
    /// <summary>
    /// Parent workbook.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_parentTable.Workbook;
      }
    }
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    public WorksheetImpl Worksheet
    {
      get
      {
        return m_parentTable.Worksheet;
      }
    }
    /// <summary>
    /// Gets parent table. Read-only.
    /// </summary>
    public DataValidationTable ParentTable
    {
      get
      {
        return m_parentTable;
      }
    }
    /// <summary>
    /// Gets DValRecord representing this collection. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public DValRecord Record
    {
      get
      {
        return m_dvalRecord;
      }
    }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public DataValidationImpl this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count )
          throw new ArgumentOutOfRangeException( "index",
            "Value cannot be less than 0 or greater than Count." );

        return ( DataValidationImpl )List[ index ];
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

        if( !m_bIsDelay )
        {
          for( int i = 0, iCount = Count; i < iCount; i++ )
          {
            iResult += this[ i ].ShapesCount;
          }
        }
        else
        {
          for( int i = 0, iCount = m_arrStorage.Count; i < iCount; i++ )
          {
            DVRecord dv = m_arrStorage[ i ] as DVRecord;

            if( dv != null )
            {
              iResult += dv.AddrListSize;
            }
          }
        }

        return iResult;
      }
    }

    #endregion

    internal void AddFrom( DataValidationCollection dvCollection,
      int iSourceRow, int iSourceColumn,
      int iDestRow, int iDestColumn,
      int iRowCount, int iColumnCount )
    {
      int iRowDelta = iDestRow - iSourceRow;
      int iColumnDelta = iDestColumn - iSourceColumn;
      DelayedParse();
      dvCollection.DelayedParse();

      foreach( KeyValuePair<DVRecord, DataValidationImpl> entry in dvCollection.m_hashRecords )
      //for( int i = 0, len = dvCollection.m_arrStorage.Count; i < len; i++ )
      {
        DataValidationImpl dv = entry.Value;
        bool cloneDataValidation = false;
        for (int i = 0; i < dv.DVRanges.Length; i++)
        {
            IRange range = this.Worksheet.Range[dv.DVRanges[i]];
            if (range.Row <= iSourceRow && range.Column <= iSourceColumn
                && range .LastColumn >= iSourceColumn && range .LastRow >= iSourceRow)
                cloneDataValidation = true;
        }
        if (cloneDataValidation)
        {
            DataValidationImpl dvToAdd = dv.Clone(this, iSourceRow, iSourceColumn,
              iRowDelta, iColumnDelta, iRowCount, iColumnCount);
            this.Add(dvToAdd);
        }
      }
    }

    private void DelayedParse()
    {
      if( m_bIsDelay )
      {
        int iIndex = 0;
        UpdateRecords( m_arrStorage, ref iIndex, m_arrStorage.Count );
      }
    }
  }
}
