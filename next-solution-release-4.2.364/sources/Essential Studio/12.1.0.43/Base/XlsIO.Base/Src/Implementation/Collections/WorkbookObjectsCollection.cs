#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <exclude/>
  /// <summary>
  /// Summary description for WorkbookObjectsCollection.
  /// </summary>
  public class WorkbookObjectsCollection
    : CollectionBaseEx<object>
    , ITabSheets
  {
    #region Class members
    /// <summary>
    /// Dictionary name of the object - to - value.
    /// </summary>
    private Dictionary<string, int> m_hashNameToValue = new Dictionary<string, int>( System.StringComparer.CurrentCultureIgnoreCase );
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates a collection and sets its Application and Parent properties.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    public WorkbookObjectsCollection( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Add new object to the collection.
    /// </summary>
    /// <param name="namedObject">Named object to adding.</param>
    [ CLSCompliant( false ) ]
    public void Add( ISerializableNamedObject namedObject )
    {
      if( namedObject == null )
        throw new ArgumentNullException( "workbookObject" );

      if( namedObject.Name == null )
        throw new ArgumentNullException( "Name can't be NULL." );
      
      if(m_hashNameToValue.ContainsKey(namedObject.Name))
          throw new ArgumentException("Sheet Name is already existed in workbook");

      int index = List.Count;
      namedObject.RealIndex = index;
      m_hashNameToValue.Add( namedObject.Name, index );
      InnerList.Add( namedObject );

      namedObject.NameChanged += new ValueChangedEventHandler( object_NameChanged );
    }
    /// <summary>
    /// Moves sheet from one position into another.
    /// </summary>
    /// <param name="iOldIndex">Old sheet index.</param>
    /// <param name="iNewIndex">New sheet index.</param>
    public void Move( int iOldIndex, int iNewIndex )
    {
      if( iOldIndex == iNewIndex ) return;

      ISerializableNamedObject toMove = this[ iOldIndex ];
      InnerList.RemoveAt( iOldIndex );

      InnerList.Insert( iNewIndex, toMove );
      int iStart = Math.Min( iNewIndex, iOldIndex );
      int iFinish = Math.Max( iNewIndex, iOldIndex );

      // update indexes
      for( int i = iStart; i <= iFinish; i++ )
      {
        this[ i ].RealIndex = i;
      }

      m_book.MoveSheetIndex( iOldIndex, iNewIndex );
      m_book.UpdateActiveSheetAfterMove( iOldIndex, iNewIndex );

      if( TabSheetMoved != null )
      {
        TabSheetMovedEventArgs args = new TabSheetMovedEventArgs( iOldIndex, iNewIndex );
        TabSheetMoved( this, args );
      }
//      WorksheetImpl sheet = this[ iNewIndex ] as WorksheetImpl;
//
//      // TODO: this should be done in some unified model for charts, worksheets and possibly any other object
//      // that microsoft will store as tabsheet in the future.
//      if( sheet != null )
//        m_book.InnerWorksheets.UpdateSheetIndex( sheet, iOldIndex );
    }
    /// <summary>
    /// Moves specified tab sheet before another tab sheet.
    /// </summary>
    /// <param name="sheetToMove">The tab sheet to move.</param>
    /// <param name="sheetForPlacement">The tab sheet to locate new position.</param>
    public void MoveBefore( ITabSheet sheetToMove, ITabSheet sheetForPlacement ) 
    {
      ISerializableNamedObject objToMove = ( ISerializableNamedObject )sheetToMove;
      ISerializableNamedObject objToLocate = ( ISerializableNamedObject )sheetForPlacement;

      int iMoveIndex = objToMove.RealIndex;
      int iLocateIndex = objToLocate.RealIndex;

      int iDestIndex = ( iMoveIndex > iLocateIndex )
        ? iLocateIndex
        : iLocateIndex - 1;

      Move( iMoveIndex, iDestIndex );
    }
    /// <summary>
    /// Moves specified tab sheet after another tab sheet.
    /// </summary>
    /// <param name="sheetToMove">The tab sheet to move.</param>
    /// <param name="sheetForPlacement">The tab sheet to locate new position.</param>
    public void MoveAfter( ITabSheet sheetToMove, ITabSheet sheetForPlacement )
    {
      ISerializableNamedObject objToMove = ( ISerializableNamedObject )sheetToMove;
      ISerializableNamedObject objToLocate = ( ISerializableNamedObject )sheetForPlacement;

      int iMoveIndex = objToMove.RealIndex;
      int iLocateIndex = objToLocate.RealIndex;

      int iDestIndex = ( iMoveIndex > iLocateIndex )
        ? iLocateIndex + 1
        : iLocateIndex;

      Move( iMoveIndex, iDestIndex );
    }
    /// <summary>
    /// Disposes internal data.
    /// </summary>
    public void DisposeInternalData()
    {
      for( int i = 0, iCount = Count; i < iCount; i++ )
      {
        WorksheetBaseImpl sheet = InnerList[ i ] as WorksheetBaseImpl;
        sheet.Dispose();
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// 
    /// </summary>
    [ CLSCompliant( false ) ]
    public ISerializableNamedObject this[ int index ]
    {
      get
      {
        if( index < 0 || index >= List.Count )
          throw new ArgumentOutOfRangeException( "index" );

        return List[ index ] as ISerializableNamedObject;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public INamedObject this[ string name ]
    {
      get
      {
        int index;

        return ( m_hashNameToValue.TryGetValue( name, out index ) ) ?
          this[ index ] :
          null;
      }
    }
    /// <summary>
    /// Parent workbook.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Creates copy of the collection.
    /// </summary>
    /// <param name="parent">Parent object for the new collection.</param>
    /// <returns>A copy of the collection.</returns>
    public override object Clone( object parent )
    {
      WorkbookObjectsCollection result = new WorkbookObjectsCollection( Application, parent );

      List<object> list = InnerList;
      IList<object> destList = result.List;
      result.m_book.Objects = result;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        WorksheetBaseImpl item = list[ i ] as WorksheetBaseImpl;
        object itemToAdd = item.Clone( result, false );
        destList.Add( itemToAdd );
      }

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        WorksheetBaseImpl sourceItem = list[ i ] as WorksheetBaseImpl;
        WorksheetBaseImpl destItem = destList[ i ] as WorksheetBaseImpl;
        sourceItem.CloneShapes( destItem );
      }

      return result;
    }
    #endregion

    #region ITabSheets members
    /// <summary>
    /// Returns single entry from the collection.
    /// </summary>
    ITabSheet ITabSheets.this[ int index ]
    {
      get
      {
        if( index < 0 || index > Count - 1 )
          throw new ArgumentOutOfRangeException( "index", "Value cannot be less than 0 and greater than Count - 1." );

        return ( ITabSheet )InnerList[ index ];
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Searches for all necessary parents.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion
    
    #region Implementation of sync Name property
    /// <summary>
    /// Performs additional processes after inserting a new element into the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected override void OnInsertComplete( int index, object value )
    {
      ISerializableNamedObject obj = ( ISerializableNamedObject )value;
      obj.NameChanged += new ValueChangedEventHandler( object_NameChanged );
      m_hashNameToValue[ obj.Name ] = index;

      for( int i = index, count = List.Count; i < count; i++ )
      {
        this[ index ].RealIndex = i;
      }

      m_book.IncreaseSheetIndex( index );

      base.OnInsertComplete( index, value );
    }

    /// <summary>
    /// Performs additional processes after setting a value in the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at the index.</param>
    protected override void OnSetComplete( int index, object oldValue, object newValue )
    {
      WorksheetImpl sheetOld = ( WorksheetImpl )oldValue;
      WorksheetImpl sheetNew = ( WorksheetImpl )newValue;

      sheetOld.NameChanged -= new ValueChangedEventHandler( object_NameChanged );
      m_hashNameToValue.Remove( sheetOld.Name );

      m_hashNameToValue[ sheetNew.Name ] = index;

      base.OnSetComplete( index, oldValue, newValue );
    }

    /// <summary>
    /// Performs additional processes after removing an element from the collection.
    /// </summary>
    /// <param name="index">The zero-based index at which value can be found.</param>
    /// <param name="value">The value of the element to remove from the index.</param>
    protected override void OnRemoveComplete( int index, object value )
    {
      ISerializableNamedObject obj = ( ISerializableNamedObject )value;
      obj.NameChanged -= new ValueChangedEventHandler( object_NameChanged );
      m_hashNameToValue.Remove( obj.Name );

      int iCount = List.Count;

      for( int i = index; i < iCount; i++ )
      {
        ISerializableNamedObject curObject = this[ index ];
        curObject.RealIndex = i;
        m_hashNameToValue[ curObject.Name ] = i;
      }

      m_book.DecreaseSheetIndex( index );
      int activeSheetIndex = m_book.ActiveSheetIndex;

      if( index < m_book.ActiveSheetIndex || ( index == m_book.ActiveSheetIndex && index == iCount ) )
      {
        activeSheetIndex -= 1;
        FindVisibleWorksheet(activeSheetIndex);
      }

      (this[activeSheetIndex] as ITabSheet).Activate();
      base.OnRemoveComplete( index, value );
    }

    private void FindVisibleWorksheet( int proposedIndex )
    {
      if( ( this[ proposedIndex ] as ITabSheet ).Visibility == WorksheetVisibility.Visible )
      {
        m_book.ActiveSheetIndex = proposedIndex;
      }
      else
      {
        int newValue = -1;

        for( int i = proposedIndex - 1; i >= 0; i-- )
        {
          if( ( this[ i ] as ITabSheet ).Visibility == WorksheetVisibility.Visible )
          {
            newValue = i;
            break;
          }
        }

        if( newValue == -1 )
        {
          for( int i = proposedIndex + 1, last = Count; i < last; i-- )
          {
            if( ( this[ i ] as ITabSheet ).Visibility == WorksheetVisibility.Visible )
            {
              newValue = i;
              break;
            }
          }
        }
        if (newValue == -1)
            throw new Exception("A workbook must contain at least one visible worksheet. To hide, delete, or move the selected sheet(s), you must first insert a new sheet or unhide a sheet that is already hidden.");

        m_book.ActiveSheetIndex = newValue;
      }
    }

    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      base.OnClearComplete();
      m_hashNameToValue.Clear();
    }

    /// <summary>
    /// This method is called when sheet name was changed.
    /// </summary>
    /// <param name="sender">Sender of the event.</param>
    /// <param name="e">Event arguments.</param>
    /// <exception cref="System.ArgumentException">
    /// When workbook already contains worksheet with specified name.
    /// </exception>
    private void object_NameChanged( object sender, ValueChangedEventArgs e )
    {
      string strNewName = ( string )e.newValue;

      if( m_hashNameToValue.ContainsKey( strNewName ) )
        throw new ArgumentException( "Name of worksheet must be unique in a workbook." );

      string strOldName = ( string )e.oldValue;
      int iOldIndex = m_hashNameToValue[ strOldName ];
      m_hashNameToValue.Remove( strOldName );
      m_hashNameToValue[ strNewName ] = iOldIndex;
    }
    #endregion

    #region Class events
    /// <summary>
    /// 
    /// </summary>
    public event TabSheetMovedEventHandler TabSheetMoved;
    #endregion
  }
  /// <exclude/>
  /// <summary>
  /// 
  /// </summary>
  public sealed class TabSheetMovedEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Old index in the tab sheets collection.
    /// </summary>
    private int m_iOldIndex;
    /// <summary>
    /// New index in the tab sheets collection.
    /// </summary>
    private int m_iNewIndex;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    private TabSheetMovedEventArgs()
    {
    }
    /// <summary>
    /// Initializes new instance of the event arguments.
    /// </summary>
    /// <param name="oldIndex">Old index in the tab sheets collection.</param>
    /// <param name="newIndex">New index in the tab sheets collection.</param>
    public TabSheetMovedEventArgs( int oldIndex, int newIndex )
    {
      m_iOldIndex = oldIndex;
      m_iNewIndex = newIndex;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns old index in the tab sheets collection. Read-only.
    /// </summary>
    public int OldIndex
    {
      get
      {
        return m_iOldIndex;
      }
    }
    /// <summary>
    /// Returns new index in the tab sheets collection. Read-only.
    /// </summary>
    public int NewIndex
    {
      get
      {
        return m_iNewIndex;
      }
    }
    #endregion
  }
  /// <exclude/>
  /// <summary>
  /// 
  /// </summary>
  public delegate void TabSheetMovedEventHandler( object sender, TabSheetMovedEventArgs args );
}
