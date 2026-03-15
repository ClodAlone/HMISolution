#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Syncfusion.XlsIO.Implementation.Collections
{
  public class CollectionBase<T> : IList<T>
  {
    #region Members
    /// <summary>
    /// List with collection items.
    /// </summary>
    private List<T> m_arrItems;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the number of elements that the System.Collections.CollectionBase can contain.
    /// </summary>
    public int Capacity
    {
      get
      {
        return m_arrItems.Capacity;
      }
      set
      {
        m_arrItems.Capacity = value;
      }
    }
    /// <summary>
    /// Gets the number of elements contained in the System.Collections.CollectionBase instance.
    /// </summary>
    public int Count
    {
      get
      {
        return m_arrItems.Count;
      }
    }
    /// <summary>
    /// Gets the list of elements in the instance.
    /// </summary>
    protected internal List<T> InnerList
    {
      get
      {
        return m_arrItems;
      }
    }
    /// <summary>
    /// Gets the list of elements in the instance.
    /// </summary>
    protected IList<T> List
    {
      get
      {
        return m_arrItems;
      }
    }
    public T this[ int i ]
    {
      get
      {
        return m_arrItems[ i ];
      }
      set
      {
        T oldValue = m_arrItems[ i ];
        OnSet( i, oldValue, value );
        m_arrItems[ i ] = value;
        OnSetComplete( i, oldValue, value );
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the class with the default initial capacity.
    /// </summary>
    public CollectionBase()
    {
      m_arrItems = new List<T>();
    }
    //
    /// <summary>
    /// Initializes a new instance of the class with the specified capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    public CollectionBase( int capacity )
    {
      m_arrItems = new List<T>( capacity );
    }
    /// <summary>
    /// Removes all objects from the System.Collections.CollectionBase instance.
    /// </summary>
    public void Clear()
    {
      OnClear();
      m_arrItems.Clear();
      OnClearComplete();
    }
    /// <summary>
    /// Inserts an element into the list at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which item should be inserted.</param>
    /// <param name="item">The object to insert. The value can be null for reference types.</param>
    public void Insert( int index, T item )
    {
      OnInsert( index, item );
      m_arrItems.Insert( index, item );
      OnInsertComplete( index, item );
    }
    /// <summary>
    /// Returns an enumerator that iterates through this instance.
    /// </summary>
    /// <returns>An enumerator for this instance.</returns>
    public IEnumerator<T> GetEnumerator()
    {
      return m_arrItems.GetEnumerator();
    }
    /// <summary>
    /// Performs additional custom processes when clearing the contents of this instance.
    /// </summary>
    protected virtual void OnClear()
    {
    }
    /// <summary>
    /// Performs additional custom processes after clearing the contents of this instance.
    /// </summary>
    protected virtual void OnClearComplete()
    {
    }
    /// <summary>
    ///  Performs additional custom processes before inserting a new element into this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected virtual void OnInsert( int index, T value )
    {
    }
    /// <summary>
    ///  Performs additional custom processes after inserting a new element into this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert value.</param>
    /// <param name="value">The new value of the element at index.</param>
    protected virtual void OnInsertComplete( int index, T value )
    {
    }
    /// <summary>
    ///  Performs additional custom processes when removing an element from this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which value can be found.</param>
    /// <param name="value">The value of the element to remove from index.</param>
    protected virtual void OnRemove( int index, T value )
    {
    }
    /// <summary>
    ///  Performs additional custom processes after removing an element from this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which value can be found.</param>
    /// <param name="value">The value of the element to remove from index.</param>
    protected virtual void OnRemoveComplete( int index, T value )
    {
    }
    /// <summary>
    /// Performs additional custom processes before setting a value in this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at index.</param>
    protected virtual void OnSet( int index, T oldValue, T newValue )
    {
    }
    /// <summary>
    /// Performs additional custom processes after setting a value in this instance.
    /// </summary>
    /// <param name="index">The zero-based index at which oldValue can be found.</param>
    /// <param name="oldValue">The value to replace with newValue.</param>
    /// <param name="newValue">The new value of the element at index.</param>
    protected virtual void OnSetComplete( int index, T oldValue, T newValue )
    {
    }
    /// <summary>
    /// Removes the element at the specified index of this instance.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt( int index )
    {
      T value = this[ index ];
      OnRemove( index, value );
      m_arrItems.RemoveAt( index );
      OnRemoveComplete( index, value );
    }
    #endregion

    #region IList<T> Members

    public int IndexOf( T item )
    {
      return m_arrItems.IndexOf( item );
    }

    #endregion

    #region ICollection<T> Members

    public virtual void Add( T item )
    {
      int index = Count;
      OnInsert( index, item );
      m_arrItems.Add( item );
      OnInsertComplete( index, item );
    }

    public bool Contains( T item )
    {
      return m_arrItems.Contains( item );
    }

    public void CopyTo( T[] array, int arrayIndex )
    {
      m_arrItems.CopyTo( array, arrayIndex );
    }

    public bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    public bool Remove( T item )
    {
      int index = IndexOf( item );
      bool result = false;

      if( index >= 0 )
      {
        RemoveAt( index );
        result = true;
      }

      return result;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ( m_arrItems as IEnumerable ).GetEnumerator();
    }

    #endregion
  }
}
