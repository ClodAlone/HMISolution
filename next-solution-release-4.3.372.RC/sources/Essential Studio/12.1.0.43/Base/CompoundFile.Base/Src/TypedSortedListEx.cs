#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#if SILVERLIGHT || WP

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Syncfusion.CompoundFile;

#if DOCIO
using ICloneable = Syncfusion.CompoundFile.DocIO.ICloneable;

namespace Syncfusion.CompoundFile.DocIO.Net
{
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class TypedSortedListEx<TKey, TValue> :
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IDictionary<TKey, TValue>,
    IDictionary
    where TKey : IComparable
  {
    #region Class constants
    /// <summary>
    /// Default capacity of internal buffers.
    /// </summary>
    private const int DefaultCapacity = 16;
    #endregion

    #region Class members
    /// <summary>
    /// Array which store keys in sorted order.
    /// </summary>
    private TKey[]      m_arrKeys;
    /// <summary>
    /// Collection stores values.
    /// </summary>
    private Dictionary<TKey, TValue> m_dicValues;
    /// <summary>
    /// Size of collection.
    /// </summary>
    private int         m_iSize;
    /// <summary>
    /// Version of collection data.
    /// </summary>
    private int         m_iVersion;
    /// <summary>
    /// Default comparer for keys.
    /// </summary>
    private IComparer<TKey>   m_comparer;
    /// <summary>
    /// List of keys.
    /// </summary>
    private KeyList m_listKeys;
    /// <summary>
    /// List of values.
    /// </summary>
    private ValueList m_lstValues;
    #endregion

    #region Class Properties
    /// <summary>
    /// Capacity of internal buffers.
    /// </summary>
    public virtual int Capacity
    {
      get
      {
        return m_arrKeys.Length;
      }
      set
      {
        if( value != m_arrKeys.Length )
        {
          if( value < m_iSize )
            throw new ArgumentOutOfRangeException( "value" );

          if( value > 0 )
          {
            TKey[] newKeys = new TKey[ value ];

            if( m_iSize > 0 )
              Array.Copy( m_arrKeys, 0, newKeys, 0, m_iSize );

            m_arrKeys = newKeys;
          }
          else
          {
            m_arrKeys = new TKey[ DefaultCapacity ];
          }
        }
      }
    }

    /// <summary>
    /// Size of the collection. Read-only.
    /// </summary>
    public virtual int Count
    {
      get
      {
        return m_iSize;
      }
    }

    /// <summary>
    /// List of keys. Read-only.
    /// </summary>
    public virtual IList<TKey> Keys
    {
      get
      {
        return GetKeyList();
      }
    }

    /// <summary>
    /// List of values. Read-only.
    /// </summary>
    public virtual IList<TValue> Values
    {
      get
      {
        return GetValueList();
      }
    }

    /// <summary>
    /// Returns True if list is readonly, False otherwise. Read-only.
    /// </summary>
    public virtual bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns True if collection has fixed size, False otherwise.
    /// </summary>
    public virtual bool IsFixedSize
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns True if the collection is synchronized, False otherwise.
    /// </summary>
    public virtual bool IsSynchronized
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns the object that can be used to synchronize access to the collection.
    /// Read-only.
    /// </summary>
    public virtual object SyncRoot
    {
      get
      {
        return this;
      }
    }
    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    public virtual TValue this[ TKey key ]
    {
      get
      {
        TValue result;
        m_dicValues.TryGetValue( key, out result );
        return result;
      }
      set
      {
        if( key == null )
          throw new ArgumentNullException( "key" );

        if( m_dicValues.ContainsKey( key ) )
        {
          m_dicValues[ key ] = value;
        }
        else
        {
          Add( key, value );
        }

        m_iVersion++;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public TypedSortedListEx()
    {
      m_arrKeys = new TKey[ DefaultCapacity ];
      m_dicValues = new Dictionary<TKey, TValue>( DefaultCapacity );
      m_comparer = Comparer<TKey>.Default;
    }

    /// <summary>
    /// Creates an empty list with the specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">Initial capacity.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When initialCapacity is less than zero.
    /// </exception>
    public TypedSortedListEx( int initialCapacity )
    {
      if( initialCapacity < 0 )
        throw new ArgumentOutOfRangeException( "initialCapacity" );

      m_arrKeys = new TKey[ initialCapacity ];
      m_dicValues = new Dictionary<TKey, TValue>( initialCapacity );
      m_comparer = Comparer<TKey>.Default;
    }

    /// <summary>
    /// Creates an empty SortedList with the default initial capacity
    /// and specified comparer.
    /// </summary>
    /// <param name="comparer">
    /// The IComparer is used to determine whether two keys are equal.
    /// </param>
    public TypedSortedListEx( IComparer<TKey> comparer )
      : this()
    {
      if( comparer != null )
        m_comparer = comparer;
    }

    /// <summary>
    /// Creates an empty SortedList with the specified initial capacity
    /// and specified comparer.
    /// </summary>
    /// <param name="comparer">Initial capacity.</param>
    /// <param name="capacity">
    /// The IComparer is used to determine whether two keys are equal.
    /// </param>
    public TypedSortedListEx( IComparer<TKey> comparer, int capacity )
      : this( comparer )
    {
      Capacity = capacity;
    }

    /// <summary>
    /// Copies the elements from the specified dictionary to a new list
    /// with the same initial capacity as the number of elements copied.
    /// </summary>
    /// <param name="d">The IDictionary to copy.</param>
    public TypedSortedListEx( IDictionary<TKey, TValue> d )
      : this( d, null )
    {
    }

    /// <summary>
    /// Copies the elements from the specified dictionary to a new list with the same
    /// initial capacity as the number of elements copied and with the specified comparer.
    /// </summary>
    /// <param name="d">The IDictionary to copy.</param>
    /// <param name="comparer">
    /// The IComparer to use to determine whether two keys are equal.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When argument d is null.
    /// </exception>
    public TypedSortedListEx( IDictionary<TKey, TValue> d, IComparer<TKey> comparer )
      : this( comparer, ( d != null ? d.Count : 0 ) )
    {
      if( d == null )
        throw new ArgumentNullException( "d" );

      // Create copy of dictionary values.
      d.Keys.CopyTo( m_arrKeys, 0 );
      m_dicValues = new Dictionary<TKey, TValue>( d );

      Array.Sort( m_arrKeys, comparer );
      m_iSize = d.Count;
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Returns a synchronized (thread-safe) wrapper for the SortedList.
    /// </summary>
    /// <param name="list">The SortedList to synchronize.</param>
    /// <returns>A synchronized (thread-safe) wrapper for the SortedList.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When list is null.
    /// </exception>
    public static TypedSortedListEx<TKey, TValue> Synchronized( TypedSortedListEx<TKey, TValue> list )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      throw new NotImplementedException();
      //return new SyncSortedListEx( list );
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds an element with the provided key and value to the list.
    /// </summary>
    /// <param name="key">The Object to use as the key of the element to add.</param>
    /// <param name="value">The Object to use as the value of the element to add.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When key is null.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When list already contains specified key.
    /// </exception>
    public virtual void Add( TKey key, TValue value )
    {
      if( key == null )
        throw new ArgumentNullException( "key" );

      if( m_dicValues.ContainsKey( key ) )
        throw new ArgumentException( "Duplicated" );

      int index = Array.BinarySearch( m_arrKeys, 0, m_iSize, key, m_comparer );
      Insert( ~index, key, value );
    }

    /// <summary>
    /// Removes all elements from the collection.
    /// </summary>
    public virtual void Clear()
    {
      m_iVersion++;
      m_iSize = 0;

      m_arrKeys = new TKey[ DefaultCapacity ];
      m_dicValues = new Dictionary<TKey, TValue>( DefaultCapacity );
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance.</returns>
    public virtual object Clone()
    {
      TypedSortedListEx<TKey, TValue> sl = new TypedSortedListEx<TKey, TValue>( m_iSize );
      Array.Copy( m_arrKeys, 0, sl.m_arrKeys, 0, m_iSize );
      sl.m_dicValues = new Dictionary<TKey, TValue>( m_dicValues );
      sl.m_iSize = m_iSize;
      sl.m_iVersion = m_iVersion;
      sl.m_comparer = m_comparer;

      // Don't copy keyList or the valueList.
      return sl;
    }
    /// <summary>
    /// Clone current instance.
    /// </summary>
    /// <returns>Returns clone of current object.</returns>
    public TypedSortedListEx<TKey, TValue> CloneAll()
    {
      int iLen = Count;
      TypedSortedListEx<TKey, TValue> result = ( TypedSortedListEx<TKey, TValue> )MemberwiseClone();
      result.m_arrKeys = new TKey[ iLen ];
      result.m_dicValues = new Dictionary<TKey, TValue>( iLen );
      result.m_listKeys = null;
      result.m_lstValues = null;
      result.m_iSize = 0;

      for( int i = 0;i < iLen;i++ )
      {
        TKey key = GetKey( i );
        TValue o = m_dicValues[ key ];

        ICloneable toClone = o as ICloneable;

        if( toClone != null )
        {
          o = ( TValue )toClone.Clone();
        }

        result.Add( key, o );
      }

      return result;
    }
    /// <summary>
    /// Determines whether the list contains an element with the specified key.
    /// </summary>
    /// <param name="key">Key of the element to search.</param>
    /// <returns>True if list contains specified key.</returns>
    public virtual bool Contains( TKey key )
    {
      return m_dicValues.ContainsKey( key );
    }

    /// <summary>
    /// Determines whether the list contains an element with the specified key.
    /// </summary>
    /// <param name="key">Key of the element to search.</param>
    /// <returns>True if list contains specified key.</returns>
    public virtual bool ContainsKey( TKey key )
    {
      return m_dicValues.ContainsKey( key );
    }

    /// <summary>
    /// Determines whether the list contains the specified value.
    /// </summary>
    /// <param name="value">Value of the element to search.</param>
    /// <returns>True if list contains specified value.</returns>
    public virtual bool ContainsValue( TValue value )
    {
      return m_dicValues.ContainsValue( value );
    }

    /// <summary>
    /// Copies all the elements of the SortedListEx to the specified one-dimensional Array
    /// starting at the specified destination Array index.
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the
    /// elements copied from the current list.</param>
    /// <param name="arrayIndex">The index in array at which copying begins.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If specified array is null.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If rank of the array is not 1 or there are not enough elements.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If specified arrayIndex is less than zero.
    /// </exception>
    public virtual void CopyTo( Array array, int arrayIndex )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );

      if( array.Rank != 1 )
        throw new ArgumentException( "array" );

      if( arrayIndex < 0 )
        throw new ArgumentOutOfRangeException( "arrayIndex" );

      if( array.Length - arrayIndex < Count )
        throw new ArgumentException();

      for( int i = 0;i < Count;i++ )
      {
        KeyValuePair<TKey, TValue> entry = new KeyValuePair<TKey, TValue>( m_arrKeys[ i ],
          m_dicValues[ m_arrKeys[ i ] ] );

        array.SetValue( entry, i + arrayIndex );
      }
    }

    /// <summary>
    /// Gets the value at the specified index of the SortedListEx.
    /// </summary>
    /// <param name="index">The zero-based index of the value to get.</param>
    /// <returns>The value at the specified index of the SortedListEx.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list.
    /// </exception>
    public virtual TValue GetByIndex( int index )
    {
      if( index < 0 || index >= m_iSize )
        throw new ArgumentOutOfRangeException( "index" );

      return m_dicValues[ m_arrKeys[ index ] ];
    }

    /// <summary>
    /// Gets the key at the specified index of the SortedListEx.
    /// </summary>
    /// <param name="index">The zero-based index of the key to get.</param>
    /// <returns>The key at the specified index of the SortedListEx.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list.
    /// </exception>
    public virtual TKey GetKey( int index )
    {
      if( index < 0 || index >= m_iSize )
        throw new ArgumentOutOfRangeException( "index" );

      return m_arrKeys[ index ];
    }

    /// <summary>
    /// Gets the keys in the SortedListEx.
    /// </summary>
    /// <returns>An IList containing the keys in the SortedListEx.</returns>
    public virtual IList<TKey> GetKeyList()
    {
      if( m_listKeys == null )
        m_listKeys = new KeyList( this );

      return m_listKeys;
    }

    /// <summary>
    /// Gets the values in the SortedListEx.
    /// </summary>
    /// <returns>An IList containing the values in the SortedListEx.</returns>
    public virtual IList<TValue> GetValueList()
    {
      if( m_lstValues == null )
      {
        m_lstValues = new ValueList( this );
      }
      else
      {
        m_lstValues.UpdateValues();
      }

      return m_lstValues;
    }

    /// <summary>
    /// Returns the zero-based index of the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>The zero-based index of key, if key is found; otherwise, -1.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If specified key is null.
    /// </exception>
    public virtual int IndexOfKey( TKey key )
    {
      if( key == null )
        throw new ArgumentNullException( "key" );

      int ret = Array.BinarySearch( m_arrKeys, 0, m_iSize, key, m_comparer );

      return ret >= 0 ? ret : -1;
    }

    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified value.
    /// </summary>
    /// <param name="value">The value to locate (can be NULL).</param>
    /// <returns>
    /// The zero-based index of the first occurrence of value, if value is found;
    /// otherwise, -1.
    /// </returns>
    public virtual int IndexOfValue( TValue value )
    {
      object key = null;

      IDictionaryEnumerator enm = m_dicValues.GetEnumerator();
      enm.Reset();

      while( enm.MoveNext() )
      {
        if( enm.Value.Equals( value ) )
        {
          key = enm.Key;
          break;
        }
      }

      if( key == null )
        return -1;

      return Array.IndexOf( m_arrKeys, key, 0, m_iSize );
    }

    /// <summary>
    /// Removes the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list.
    /// </exception>
    public virtual void RemoveAt( int index )
    {
      if( index < 0 || index >= m_iSize )
        throw new ArgumentOutOfRangeException( "index" );

      m_iSize--;
      TKey key = m_arrKeys[ index ];
      if( index < m_iSize )
      {
        Array.Copy( m_arrKeys, index + 1, m_arrKeys, index, m_iSize - index );
      }
      m_arrKeys[ m_iSize ] = default( TKey );
      m_dicValues.Remove( key );
      m_iVersion++;
    }

    /// <summary>
    ///Removes the element with the specified key from SortedListEx.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    public virtual bool Remove( TKey key )
    {
      int i = IndexOfKey( key );
      bool result;

      if( i >= 0 )
      {
        RemoveAt( i );
        result = true;
      }
      else
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Replaces the value at the specific index.
    /// </summary>
    /// <param name="index">The zero-based index at which to save value.</param>
    /// <param name="value">The Object to save into. Can be NULL.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list.
    /// </exception>
    public virtual void SetByIndex( int index, TValue value )
    {
      if( index < 0 || index >= m_iSize )
        throw new ArgumentOutOfRangeException( "index" );

      m_dicValues[ m_arrKeys[ index ] ] = value;

      m_iVersion++;
    }

    /// <summary>
    /// Sets the capacity to the actual number of elements.
    /// </summary>
    public virtual void TrimToSize()
    {
      Capacity = m_iSize;
    }

    /// <summary>
    /// Returns an IEnumerator that can iterate through the SortedListEx.
    /// </summary>
    /// <returns>An IEnumerator for the SortedListEx.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      //return new SortedListExEnumerator(this, 0, _size, SortedListExEnumerator.DictEntry);
      throw new NotImplementedException();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return new TypedSortedListExEnumerator( this, 0, m_iSize );
    }

    #endregion

    #region Private helper methods
    /// <summary>
    /// Inserts element with specified key and value at specified index.
    /// </summary>
    /// <param name="index">The zero-based index to insert element at.</param>
    /// <param name="key">The key of the element to insert.</param>
    /// <param name="value">The value of the element to insert.</param>
    private void Insert( int index, TKey key, TValue value )
    {
      if( m_iSize == m_arrKeys.Length )
        EnsureCapacity( m_iSize + 1 );

      if( index < m_iSize )
      {
        Array.Copy( m_arrKeys, index, m_arrKeys, index + 1, m_iSize - index );
      }

      m_arrKeys[ index ] = key;
      m_dicValues[ key ] = value;

      m_iSize++;
      m_iVersion++;
    }

    /// <summary>
    /// Ensures that the capacity of this instance is at least the specified value.
    /// </summary>
    /// <param name="min">The minimum capacity to ensure.</param>
    private void EnsureCapacity( int min )
    {
      int newCapacity = m_arrKeys.Length == 0 ? 16 : m_arrKeys.Length * 2;

      if( newCapacity < min ) newCapacity = min;

      Capacity = newCapacity;
    }
    #endregion

    #region Internal classes declarations
    /// <summary>
    /// 
    /// </summary>
#if !SILVERLIGHT && !WP
    [Serializable()]
#endif
    private class TypedSortedListExEnumerator
      : IEnumerator< KeyValuePair<TKey, TValue> >
      , ICloneable
    {
      #region Class members
      /// <summary>
      /// List for which is this enumerator.
      /// </summary>
      private TypedSortedListEx<TKey, TValue> m_list;
      /// <summary>
      /// Key of the current element.
      /// </summary>
      private TKey m_key;
      /// <summary>
      /// Values of the current element.
      /// </summary>
      private TValue m_value;
      /// <summary>
      /// Index of current element.
      /// </summary>
      private int m_iIndex;
      /// <summary>
      /// Starting index for the enumerator.
      /// </summary>
      private int m_iStartIndex;
      /// <summary>
      /// Ending index for this enumerator.
      /// </summary>
      private int m_iEndIndex;
      /// <summary>
      /// Version of collection data.
      /// </summary>
      private int m_iVersion;
      /// <summary>
      /// True if current element is correct, False otherwise
      /// (before beginning or after end).
      /// </summary>
      private bool m_bCurrent;
      #endregion

      #region Class initialize methods
      /// <summary>
      /// Creates enumerator for specified list, starting from specified index
      /// and with specified count.
      /// </summary>
      /// <param name="list">List for which enumerator is being created.</param>
      /// <param name="index">Starting index.</param>
      /// <param name="count">Number of elements to enumerate.</param>
      internal TypedSortedListExEnumerator( TypedSortedListEx<TKey, TValue> list, int index, int count )
      {
        m_list = list;
        m_iIndex = index;
        m_iStartIndex = index;
        m_iEndIndex = index + count;
        m_iVersion = m_list.m_iVersion;
        m_bCurrent = false;
      }

      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or
      /// resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>Copy of the current instance.</returns>
      public object Clone()
      {
        return MemberwiseClone();
      }

      /// <summary>
      /// Returns key of the current element. Read-only.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false.
      /// </exception>
      public virtual TKey Key
      {
        get
        {
          if( m_iVersion != m_list.m_iVersion )
            throw new InvalidOperationException();

          if( m_bCurrent == false )
            throw new InvalidOperationException();

          return m_key;
        }
      }

      /// <summary>
      /// Advances the enumerator to the next element of the collection.
      /// </summary>
      /// <returns>
      /// True if the enumerator was successfully advanced to the next element;
      /// False if the enumerator has passed the end of the collection.
      /// </returns>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version.
      /// </exception>
      public virtual bool MoveNext()
      {
        if( m_iVersion != m_list.m_iVersion )
          throw new InvalidOperationException();

        if( m_iIndex < m_iEndIndex )
        {
          m_key = m_list.m_arrKeys[ m_iIndex ];
          m_value = m_list.m_dicValues[ m_key ];
          m_iIndex++;
          m_bCurrent = true;
          return true;
        }

        m_key = default( TKey );
        m_value = default( TValue );
        m_bCurrent = false;
        return false;
      }

      /// <summary>
      /// Return DictionaryEntry for the current element.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false.
      /// </exception>
      public virtual KeyValuePair<TKey, TValue> Entry
      {
        get
        {
          if( m_iVersion != m_list.m_iVersion )
            throw new InvalidOperationException();

          if( m_bCurrent == false )
            throw new InvalidOperationException();

          return new KeyValuePair<TKey, TValue>( m_key, m_value );
        }
      }

      /// <summary>
      /// The current element in the collection. Read-only.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// If current is false.
      /// </exception>
      public virtual KeyValuePair<TKey, TValue> Current
      {
        get
        {
          if( m_bCurrent == false )
            throw new InvalidOperationException();

          return new KeyValuePair<TKey, TValue>( m_key, m_value );
        }
      }

      /// <summary>
      /// The current element in the collection. Read-only.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// If current is false.
      /// </exception>
      object IEnumerator.Current
      {
        get
        {
          if( m_bCurrent == false )
            throw new InvalidOperationException();

          return new KeyValuePair<TKey, TValue>( m_key, m_value );
        }
      }

      /// <summary>
      /// Returns value for the current element.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false.
      /// </exception>
      public virtual object Value
      {
        get
        {
          if( m_iVersion != m_list.m_iVersion )
            throw new InvalidOperationException();

          if( m_bCurrent == false )
            throw new InvalidOperationException();

          return m_value;
        }
      }

      /// <summary>
      /// Sets the enumerator to its initial position, which is before
      /// the first element in the collection.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version.
      /// </exception>
      public virtual void Reset()
      {
        if( m_iVersion != m_list.m_iVersion )
          throw new InvalidOperationException();

        m_iIndex = m_iStartIndex;
        m_bCurrent = false;
        m_key = default( TKey );
        m_value = default( TValue );
      }
      #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    private class KeysEnumerator: IEnumerator<TKey>
    {
      #region Members
      /// <summary>
      /// 
      /// </summary>
      private TypedSortedListEx<TKey, TValue> m_list;
      /// <summary>
      /// Index of current element.
      /// </summary>
      private int m_iIndex = -1;
      /// <summary>
      /// Version of collection data.
      /// </summary>
      private int m_iVersion;
      #endregion

      #region Constructors
      /// <summary>
      /// 
      /// </summary>
      /// <param name="list"></param>
      public KeysEnumerator( TypedSortedListEx<TKey, TValue> list )
      {
        if( list == null )
          throw new ArgumentNullException( "list" );

        m_list = list;
        m_iVersion = m_list.m_iVersion;
      }
      #endregion

      #region IEnumerator<TKey> Members
      /// <summary>
      /// 
      /// </summary>
      public TKey Current
      {
        get
        {
          if( m_iVersion != m_list.m_iVersion )
            throw new InvalidOperationException( "Parent collection was changed" );

          if( m_iIndex < 0 || m_iIndex >= m_list.m_iSize )
            throw new InvalidOperationException();

          return m_list.m_arrKeys[ m_iIndex ];
        }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        //throw new Exception( "The method or operation is not implemented." );
      }

      #endregion

      #region IEnumerator Members

      object IEnumerator.Current
      {
        get
        {
          if( m_iIndex < 0 || m_iIndex >= m_list.m_iSize )
            throw new InvalidOperationException();

          if( m_iVersion != m_list.m_iVersion )
            throw new InvalidOperationException( "Parent collection was changed" );

          return m_list.m_arrKeys[ m_iIndex ];
        }
      }

      public bool MoveNext()
      {
        if( m_iVersion != m_list.m_iVersion )
          throw new InvalidOperationException( "Parent collection was changed" );

        if( m_iIndex < 0 )
        {
          m_iIndex = 0;
        }
        else
        {
          m_iIndex++;
        }

        if( m_iIndex >= m_list.m_iSize )
        {
          m_iIndex = -1;
        }

        return m_iIndex >= 0;
      }

      public void Reset()
      {
        if( m_iVersion != m_list.m_iVersion )
          throw new InvalidOperationException( "Parent collection was changed" );

        m_iIndex = -1;
      }

      #endregion
    }

    /// <summary>
    /// 
    /// </summary>
#if !SILVERLIGHT && !WP
    [Serializable()]
#endif
    private class KeyList
      : IList<TKey>
    {
      #region Class members
      /// <summary>
      /// List for which this collection was created.
      /// </summary>
      private TypedSortedListEx<TKey, TValue> m_list;
      #endregion

      #region Class initialize methods
      /// <summary>
      /// Creates KeyList for specified SortedListEx.
      /// </summary>
      /// <param name="list">TypedSortedListEx for which KeyList must be created.</param>
      internal KeyList( TypedSortedListEx<TKey, TValue> list )
      {
        m_list = list;
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Size of the collection. Read-only.
      /// </summary>
      public virtual int Count
      {
        get
        {
          return m_list.m_iSize;
        }
      }

      /// <summary>
      /// Returns True if list is readonly, False otherwise. Read-only.
      /// </summary>
      public virtual bool IsReadOnly
      {
        get
        {
          return true;
        }
      }

      /// <summary>
      /// Returns True if collection has fixed size, False otherwise.
      /// </summary>
      public virtual bool IsFixedSize
      {
        get
        {
          return true;
        }
      }

      /// <summary>
      /// Returns True if the collection is synchronized, False otherwise.
      /// </summary>
      public virtual bool IsSynchronized
      {
        get
        {
          return m_list.IsSynchronized;
        }
      }

      /// <summary>
      /// Returns the object that can be used to synchronize access to the collection.
      /// Read-only.
      /// </summary>
      public virtual object SyncRoot
      {
        get
        {
          return m_list.SyncRoot;
        }
      }

      /// <summary>
      /// Adds an element with the provided key to the list.
      /// </summary>
      /// <param name="key">The Object to use as the key of the element to add.</param>
      public void Add( TKey key )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Removes all elements from the collection.
      /// </summary>
      public virtual void Clear()
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Determines whether the list contains an element with the specified key.
      /// </summary>
      /// <param name="key">Key of the element to search.</param>
      /// <returns>True if list contains specified key.</returns>
      public virtual bool Contains( TKey key )
      {
        return m_list.ContainsKey( key );
      }

      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index.
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list.</param>
      /// <param name="arrayIndex">The index in array at which copying begins.</param>
      /// <exception cref="System.ArgumentException">
      /// If array is null or rank of the array is not 1.
      /// </exception>
      public virtual void CopyTo( TKey[] array, int arrayIndex )
      {
        if( array == null )
          throw new ArgumentException( "array" );

        // Defer error checking to Array.Copy.
        Array.Copy( m_list.m_arrKeys, 0, array, arrayIndex, m_list.Count );
      }

      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index.
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list.</param>
      /// <param name="arrayIndex">The index in array at which copying begins.</param>
      /// <exception cref="System.ArgumentException">
      /// If array is null or rank of the array is not 1.
      /// </exception>
      public virtual void CopyTo( Array array, int arrayIndex )
      {
        if( array == null )
          throw new ArgumentNullException( "array" );

        if( array.Rank != 1 )
          throw new ArgumentException( "array" );

        // Defer error checking to Array.Copy.
        Array.Copy( m_list.m_arrKeys, 0, array, arrayIndex, m_list.Count );
      }

      /// <summary>
      /// Insert the value at the specific index.
      /// </summary>
      /// <param name="index">The zero-based index at which to save value.</param>
      /// <param name="value">The Object to save into. Can be NULL.</param>
      public virtual void Insert( int index, TKey value )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Gets or sets the key at the specified index.
      /// </summary>
      public virtual TKey this[ int index ]
      {
        get
        {
          return m_list.GetKey( index );
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      /// <summary>
      /// Returns an IEnumerator that can iterate through the list.
      /// </summary>
      /// <returns>An IEnumerator for the list.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        return new KeysEnumerator( m_list );
      }

      /// <summary>
      /// Returns an IEnumerator that can iterate through the list.
      /// </summary>
      /// <returns>An IEnumerator for the list.</returns>
      public IEnumerator<TKey> GetEnumerator()
      {
        return new KeysEnumerator( m_list );
      }

      /// <summary>
      /// Returns the zero-based index of the specified key.
      /// </summary>
      /// <param name="key">The key to locate.</param>
      /// <returns>The zero-based index of the key, if the key is found; otherwise, -1.</returns>
      public virtual int IndexOf( TKey key )
      {
        if( key == null )
          throw new ArgumentNullException( "key" );

        int i = Array.BinarySearch( m_list.m_arrKeys, 0, m_list.Count, key, m_list.m_comparer );
        return ( i >= 0 ) ? i : -1;
      }

      /// <summary>
      ///Removes the element with the specified key from list.
      /// </summary>
      /// <param name="key">The key of the element to remove.</param>
      public bool Remove( TKey key )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Removes the element at the specified index from the list.
      /// </summary>
      /// <param name="index">The zero-based index of the element to remove.</param>
      public virtual void RemoveAt( int index )
      {
        throw new NotSupportedException();
      }
      #endregion
    }

    /// <summary>
    /// 
    /// </summary>
#if !SILVERLIGHT && !WP
    [Serializable()]
#endif
    private class ValueList : IList<TValue>
    {
      #region Class members
      /// <summary>
      /// List for which this collection was created.
      /// </summary>
      private TypedSortedListEx<TKey, TValue> m_list;
      /// <summary>
      /// Array that contain values.
      /// </summary>
      private TValue[] vals;
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Creates ValueList for specified SortedListEx.
      /// </summary>
      /// <param name="list">List for which ValueList must be created.</param>
      internal ValueList( TypedSortedListEx<TKey, TValue> list )
      {
        m_list = list;
        UpdateValues();
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Re-read values from the list.
      /// </summary>
      public virtual void UpdateValues()
      {
        int iCount = m_list.Count;

        vals = new TValue[ iCount ];
        m_list.m_dicValues.Values.CopyTo( vals, 0 );

        TKey[] keys = new TKey[ iCount ];
        m_list.m_dicValues.Keys.CopyTo( keys, 0 );
#if ( WINRT || WP )
        //SortedDictionary<TKey, TValue> sortDic = new SortedDictionary<TKey, TValue>();
        System.Collections.Generic.SortedDictionary<TKey,TValue> sortDic= new System.Collections.Generic.SortedDictionary<TKey,TValue>(m_list.m_comparer);
        
        foreach (KeyValuePair<TKey,TValue> dicValue in m_list.m_dicValues)
        {
            sortDic.Add(dicValue.Key, dicValue.Value);
        }
        vals = new TValue[sortDic.Count];
        sortDic.Values.CopyTo(vals, 0);

        keys = new TKey[sortDic.Count];
        sortDic.Keys.CopyTo(keys, 0);
#else
        Array.Sort( keys, vals, m_list.m_comparer );
#endif
      }
      /// <summary>
      /// Size of the collection. Read-only.
      /// </summary>
      public virtual int Count
      {
        get
        {
          return m_list.m_iSize;
        }
      }
      /// <summary>
      /// Returns True if list is readonly, False otherwise. Read-only.
      /// </summary>
      public virtual bool IsReadOnly
      {
        get
        {
          return true;
        }
      }
      /// <summary>
      /// Returns True if collection has fixed size, False otherwise.
      /// </summary>
      public virtual bool IsFixedSize
      {
        get
        {
          return true;
        }
      }
      /// <summary>
      /// Returns True if the collection is synchronized, False otherwise.
      /// </summary>
      public virtual bool IsSynchronized
      {
        get
        {
          return m_list.IsSynchronized;
        }
      }
      /// <summary>
      /// Returns the object that can be used to synchronize access to the collection.
      /// Read-only.
      /// </summary>
      public virtual object SyncRoot
      {
        get
        {
          return m_list.SyncRoot;
        }
      }
      /// <summary>
      /// Adds an element with the provided key to the list.
      /// </summary>
      /// <param name="value">Value to add.</param>
      public virtual void Add( TValue value )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Removes all elements from the collection.
      /// </summary>
      public virtual void Clear()
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Determines whether the list contains an element with the specified value.
      /// </summary>
      /// <param name="value">Value to search.</param>
      /// <returns>True if list contains specified value.</returns>
      public virtual bool Contains( TValue value )
      {
        return m_list.ContainsValue( value );
      }
      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index.
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list.</param>
      /// <param name="arrayIndex">The index in array at which copying begins.</param>
      public virtual void CopyTo( TValue[] array, int arrayIndex )
      {
        if( array == null )
          throw new ArgumentNullException( "array" );

        if( array.Rank != 1 )
          throw new ArgumentException( "arrray" );

        // Defer error checking to Array.Copy.
        Array.Copy( vals, 0, array, arrayIndex, m_list.Count );
      }
      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index.
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list.</param>
      /// <param name="arrayIndex">The index in array at which copying begins.</param>
      public virtual void CopyTo( Array array, int arrayIndex )
      {
        if( array != null && array.Rank != 1 )
          throw new ArgumentException();

        // Defer error checking to Array.Copy.
        Array.Copy( vals, 0, array, arrayIndex, m_list.Count );
      }
      /// <summary>
      /// Insert the value at the specific index.
      /// </summary>
      /// <param name="index">The zero-based index at which to save value.</param>
      /// <param name="value">The Object to save into. Can be NULL.</param>
      public virtual void Insert( int index, TValue value )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Gets or sets the value at the specified index.
      /// </summary>
      public virtual TValue this[ int index ]
      {
        get
        {
          return m_list.GetByIndex( index );
        }
        set
        {
          m_list.SetByIndex( index, value );
        }
      }
      /// <summary>
      /// Returns an IEnumerator that can iterate through the list.
      /// </summary>
      /// <returns>An IEnumerator for the list.</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }
      /// <summary>
      /// Returns an IEnumerator that can iterate through the list.
      /// </summary>
      /// <returns>An IEnumerator for the list.</returns>
      public virtual IEnumerator<TValue> GetEnumerator()
      {
        return ( ( IEnumerable<TValue> )vals ).GetEnumerator();
        //throw new NotImplementedException();
      }
      /// <summary>
      /// Returns the zero-based index of the specified value.
      /// </summary>
      /// <param name="value">The value to locate.</param>
      /// <returns>The zero-based index of the value, if the value is found, otherwise -1.</returns>
      public virtual int IndexOf( TValue value )
      {
        return Array.IndexOf( vals, value, 0, m_list.Count );
      }
      /// <summary>
      ///Removes the specified value from list.
      /// </summary>
      /// <param name="value">The value to remove.</param>
      public virtual bool Remove( TValue value )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Removes the element at the specified index from the list.
      /// </summary>
      /// <param name="index">The zero-based index of the element to remove.</param>
      public virtual void RemoveAt( int index )
      {
        throw new NotSupportedException();
      }
      #endregion
    }
    #endregion

    #region IDictionary Members

    public void Add(object key, object value)
    {
      TKey keyTyped = ( TKey )key;
      TValue valueTyped = ( TValue )value;
      Add( keyTyped, valueTyped );
    }

    public bool Contains(object key)
    {
      bool result;

      if( key is TKey )
      {
        TKey typedKey = ( TKey )key;
        result = ContainsKey( typedKey );
      }
      else
      {
        result = false;
      }

      return result;
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return ( ( IDictionary )m_dicValues ).GetEnumerator();
    }

    ICollection IDictionary.Keys
    {
      get
      {
        return m_dicValues.Keys;
      }
    }

    public void Remove(object key)
    {
      if( key is TKey )
      {
        Remove( ( TKey )key );
      }
    }

    ICollection IDictionary.Values
    {
      get
      {
        return m_dicValues.Values;
      }
    }

    public object this[object key]
    {
      get
      {
        return ( key is TKey ) ?
          this[ ( TKey )key ] :
          default( TValue );
      }
      set
      {
        this[ ( TKey )key ] = ( TValue )value;
      }
    }

    #endregion

    #region IDictionary<TKey,TValue> Members


    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
      get
      {
        return Keys;
      }
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      return m_dicValues.TryGetValue( key, out value );
    }

    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
      get
      {
        return m_dicValues.Values;
      }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, TValue> item)
    {
      Add( item.Key, item.Value );
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      TValue value;
      bool result;

      if( TryGetValue( item.Key, out value ) )
      {
        result = value.Equals( item.Value );
      }
      else
      {
        result = false;
      }

      return result;
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      ( ( ICollection<KeyValuePair<TKey, TValue>> )m_dicValues ).CopyTo( array, arrayIndex );
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      return Remove( item.Key );
    }

    #endregion
  }
}
#endif
#endif
