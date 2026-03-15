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

using Syncfusion.XlsIO.Parser;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  ///
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  [ CLSCompliant( false ) ]
  public class OffsetArrayList
    : IEnumerable
    , IList
    , IList<IBiffStorage>
    , ICollection
  {
    #region Class members
    /// <summary>
    /// Storage of all records.
    /// </summary>
    private List<IBiffStorage>     m_list = new List<IBiffStorage>();
    #endregion

    #region IList Properties
    /// <summary>
    /// Read-only. True if list has fixed size.
    /// </summary>
    public bool   IsFixedSize
    {
      get
      {
        return ( ( IList )m_list ).IsFixedSize;
      }
    }

    /// <summary>
    /// Read-only. True if list is Read-only.
    /// </summary>
    public bool   IsReadOnly
    {
      get
      {
        return ( m_list as IList ).IsReadOnly;
      }
    }

    /// <summary>
    /// Gets / sets record in the list at the specified index.
    /// </summary>
    public IBiffStorage this[ int index ]
    {
      get
      {
        // Use as to increase performance.
        return m_list[ index ];
      }
      set
      {
        m_list[ index ] = value;
      }
    }

    /// <summary>
    /// Gets / sets record in the list at the specified index.
    /// </summary>
    object IList.this[ int index ]
    {
      get
      {
        return m_list[ index ];
      }
      set
      {
        IBiffStorage raw = value as IBiffStorage;

        if( raw != null )
          m_list[ index ] = raw;
      }
    }

    #endregion

    #region IList methods
    /// <summary>
    /// Removes the item at the specified index from the list.
    /// </summary>
    /// <param name="index">Index of the item to removed.</param>
    public void   RemoveAt( int index )
    {
      m_list.RemoveAt( index );
    }

    /// <summary>
    /// Inserts an item to the list at the specified position.
    /// </summary>
    /// <param name="index">Index at which value should be inserted.</param>
    /// <param name="value">The record to insert into list.</param>
    public void   Insert( int index, IBiffStorage value )
    {
      m_list.Insert( index, value );
    }

    /// <summary>
    /// Removes the first occurrence of a specific record from the list.
    /// </summary>
    /// <param name="value">Value to remove.</param>
    public bool   Remove( IBiffStorage value )
    {
      int index = m_list.IndexOf( value );

      if( index >= 0 )
        m_list.RemoveAt( index );

      return index >= 0;
    }

    /// <summary>
    /// Determines whether the list contains a specific value.
    /// </summary>
    /// <param name="value">The record to locate in the list.</param>
    /// <returns>True if the value is found in the list; otherwise False.</returns>
    public bool   Contains( IBiffStorage value )
    {
      return m_list.Contains( value );
    }

    /// <summary>
    /// Removes all items from the list.
    /// </summary>
    public void   Clear()
    {
      m_list.Clear();
      //m_iOffset = 0;
      //m_offsets.Clear();
      //m_dict.Clear();
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="value">Record to locate in the list.</param>
    /// <returns>The index of the value if found in the list; otherwise -1. </returns>
    public int    IndexOf( IBiffStorage value )
    {
      return m_list.IndexOf( value );
    }

    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="value">The item to add to the list.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    public int    Add( IBiffStorage value )
    {
//      int index = m_list.Count;
      // TODO: optimize or uncomment this code.
      //value.StreamPos = m_iOffset;
      //m_iOffset += value.GetStoreSize( version ) + BiffRecordRaw.DEF_HEADER_SIZE;
      m_list.Add( value );
      return m_list.Count - 1;
//      Insert( index, value );
//      return index;
    }
    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="value">The item to add to the list.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    void ICollection<IBiffStorage>.Add( IBiffStorage value )
    {
      m_list.Add( value );
    }
    /// <summary>
    /// Adds a range of items to the list.
    /// </summary>
    /// <param name="value">
    /// Collection of the records that should be added to the list.
    /// </param>
    public void   AddList( IList value )
    {
      for( int i = 0, len = value.Count; i < len;  i++ )
      {
        IBiffStorage raw = value [ i ] as IBiffStorage;
        Add( raw );
      }
    }
    /// <summary>
    /// Adds a range of items to the list.
    /// </summary>
    /// <param name="value">
    /// Collection of the records that should be added to the list.
    /// </param>
    public void   AddRange( ICollection value )
    {
      foreach( IBiffStorage raw in value )
      {
        Add( raw );
      }
    }
    /// <summary>
    /// Adds a range of items to the list.
    /// </summary>
    /// <param name="value">
    /// Collection of the records that should be added to the list.
    /// </param>
    public void AddRange( ICollection<IBiffStorage> value )
    {
      m_list.AddRange( value );
    }
    #endregion

    #region IList methods
    /// <summary>
    /// Inserts an item to the list at the specified position.
    /// </summary>
    /// <param name="index">Index at which value should be inserted.</param>
    /// <param name="value">The record to insert into list.</param>
    public void   Insert( int index, object value )
    {
      BiffRecordRaw raw = value as BiffRecordRaw;

      if( raw != null )
      {
        Insert( index, raw );
      }
    }

    /// <summary>
    /// Removes the first occurrence of a specific record from the list.
    /// </summary>
    /// <param name="value">Value to remove.</param>
    public void   Remove( object value )
    {
      BiffRecordRaw raw = value as BiffRecordRaw;
      Remove( raw );
    }

    /// <summary>
    /// Determines whether the list contains a specific value.
    /// </summary>
    /// <param name="value">The record to locate in the list.</param>
    /// <returns>True if the value is found in the list; otherwise False.</returns>
    public bool   Contains( object value )
    {
      BiffRecordRaw raw = value as BiffRecordRaw;
      return ( raw != null )
        ? Contains( raw )
        : false;
    }

    /// <summary>
    /// Determines the index of a specific item in the list.
    /// </summary>
    /// <param name="value">Record to locate in the list.</param>
    /// <returns>The index of the value if found in the list; otherwise -1. </returns>
    public int    IndexOf( object value )
    {
      BiffRecordRaw raw = value as BiffRecordRaw;
      return ( raw != null )
        ? IndexOf( raw )
        : -1;
    }
    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="value">The item to add to the list.</param>
    /// <returns>The position into which the new element was inserted.</returns>
    public int    Add( object value )
    {
      BiffRecordRaw raw = value as BiffRecordRaw;
      return ( raw != null )
        ? Add( raw )
        : -1;
    }
    #endregion

    #region ICollection Members
    /// <summary>
    /// Read-only. Gets a value indicating whether access to the
    /// ICollection is synchronized (thread-safe).
    /// </summary>
    public bool   IsSynchronized
    {
      get
      {
        return ( ( ICollection )m_list ).IsSynchronized;
      }
    }

    /// <summary>
    /// Read-only. Gets the number of elements contained in the ICollection.
    /// </summary>
    public int    Count
    {
      get
      {
        return m_list.Count;
      }
    }

    /// <summary>
    /// Copies the elements of the ICollection to an array,
    /// starting at a particular array index.
    /// </summary>
    /// <param name="array">
    /// The one dimensional array that is the destination of the
    /// elements copied from ICollection. The array must have
    /// zero-based indexing.
    /// </param>
    /// <param name="index">
    /// The zero-based index in an array at which copying begins.
    /// </param>
    public void   CopyTo( Array array, int index )
    {
      ( ( IList )m_list ).CopyTo( array, index );
    }

    /// <summary>
    /// Read-only. Gets an object that can be used to synchronize
    /// access to the ICollection.
    /// </summary>
    public object SyncRoot
    {
      get
      {
        return ( ( ICollection )m_list ).SyncRoot;
      }
    }
    #endregion

    #region IEnumerable Members
    /// <summary>
    /// Returns an enumerator that can iterate through a collection.
    /// </summary>
    /// <returns>
    /// An IEnumerator that can be used to iterate through the collection.
    /// </returns>
    public IEnumerator GetEnumerator()
    {
      return m_list.GetEnumerator();
    }
    /// <summary>
    /// Returns an enumerator that can iterate through a collection.
    /// </summary>
    /// <returns>
    /// An IEnumerator that can be used to iterate through the collection.
    /// </returns>
    IEnumerator<IBiffStorage> IEnumerable<IBiffStorage>.GetEnumerator()
    {
      return m_list.GetEnumerator();
    }
    #endregion

    #region Offsets update methods
    /// <summary>
    /// Updates offsets to the Biff records.
    /// </summary>
    public void UpdateBiffRecordsOffsets()
    {
      CalculateRecordsStreamPos();
    }
    /// <summary>
    /// Method that updates StreamPos field in records stored in this collection.
    /// </summary>
    protected void CalculateRecordsStreamPos()
    {
#if true//DEBUG
      int iPos = 0;

      for( int i = 0, len = m_list.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        IBiffStorage raw = m_list[ i ] as IBiffStorage;
#if false

        long lStreamPos = raw.StreamPos;

        if( lStreamPos != -1 && lStreamPos != iPos )
        {
            //throw new ArgumentOutOfRangeException( "offset " + lStreamPos + " " + iPos + " " + i );
            System.Diagnostics.Debug.Assert( false, "Broken on not equeal offsets." );
        }
#endif

        raw.StreamPos = iPos;
        iPos += BiffRecordRaw.DEF_HEADER_SIZE + raw.GetStoreSize( ExcelVersion.Excel97to2003 );//Length;
      }
#endif
    }
    #endregion

    #region ICollection<IBiffStorage> Members

    public void CopyTo( IBiffStorage[] array, int arrayIndex )
    {
      m_list.CopyTo( array, arrayIndex );
    }

    #endregion
  }
}
