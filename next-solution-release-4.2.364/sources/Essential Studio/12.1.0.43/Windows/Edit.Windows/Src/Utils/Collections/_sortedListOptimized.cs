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
#endregion

namespace Syncfusion.Windows.Forms.Edit.Utils
{
  /// <summary>
  /// Optimized version of SortedList collection. Instead of keeping two
  /// array one for keys and one for values, we change values array on
  /// Hashtable collection, and only keep keys collection sorted.
  /// Performance of this collection greater that SortedList has.
  /// </summary>
  [ Serializable() ]
  public class SortedListEx
    : IDictionary
    , ICloneable
  {
    #region Class constants
    /// <summary>
    /// Default capacity of internal buffers
    /// </summary>
    private const int _defaultCapacity = 16;
    #endregion

    #region Class members
    /// <summary>
    /// array which store keys in sorted order
    /// </summary>
    private object[ ]      keys;
    /// <summary>
    /// collection store value
    /// </summary>
    private Hashtable     values;
    /// <summary>
    /// size of collection
    /// </summary>
    private int           _size;
    /// <summary>
    /// version of collection data
    /// </summary>
    private int           version;
    /// <summary>
    /// default comparer for keys
    /// </summary>
    private IComparer     comparer;
    /// <summary>
    /// List of keys
    /// </summary>
    private KeyList       keyList;
    /// <summary>
    /// list of values
    /// </summary>
    private ValueList     valueList;
    #endregion

    #region Class Properties
    /// <summary>
    /// Capacity of internal buffers
    /// </summary>
    public virtual int Capacity
    {
      get
      {
        return keys.Length;
      }
      set
      {
        if( value != keys.Length )
        {
          if( value < _size )
            throw new ArgumentOutOfRangeException( "value" );

          if( value > 0 )
          {
            object[ ] newKeys = new object[ value ];

            if( _size > 0 )
            {
              Array.Copy( keys, 0, newKeys, 0, _size );
            }

            keys = newKeys;
          }
          else
          {
            keys = new object[ _defaultCapacity ];
          }
        }
      }
    }

    /// <summary>
    /// Size of the collection. Read-only
    /// </summary>
    public virtual int Count
    {
      get
      {
        return _size;
      }
    }

    /// <summary>
    /// List of keys. Read-only.
    /// </summary>
    public virtual ICollection Keys
    {
      get
      {
        return GetKeyList();
      }
    }

    /// <summary>
    /// list of values. Read-only.
    /// </summary>
    public virtual ICollection Values
    {
      get
      {
        return GetValueList();
      }
    }

    /// <summary>
    /// Returns TRUE if list is readonly, FALSE otherwise. Read-only
    /// </summary>
    public virtual bool IsReadOnly
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns TRUE if collection has fixed size, FALSE otherwise.
    /// </summary>
    public virtual bool IsFixedSize
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Returns TRUE if the collection is synchronized, FALSE otherwise
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
    public virtual object this[ object key ]
    {
      get
      {
        if( values.ContainsKey( key ) ) return values[ key ];
        return null;
      }
      set
      {
        if( key == null )
          throw new ArgumentNullException( "key" );

        if( values.ContainsKey( key ) )
        {
          values[ key ] = value;
        }
        else
        {
          int index = Array.BinarySearch( keys, 0, _size, key, comparer );
          Insert( ~index, key, value );
        }

        version++;
      }
    }

    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor
    /// </summary>
    public SortedListEx()
    {
      keys = new Object[ _defaultCapacity ];
      values = new Hashtable( _defaultCapacity );
      comparer = Comparer.Default;
    }

    /// <summary>
    /// Creates an empty list with the specified initial capacity
    /// </summary>
    /// <param name="initialCapacity">initial capacity</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When initialCapacity is less than zero
    /// </exception>
    public SortedListEx( int initialCapacity )
    {
      if( initialCapacity < 0 )
        throw new ArgumentOutOfRangeException( "initialCapacity" );
      keys = new Object[ initialCapacity ];
      values = new Hashtable( initialCapacity );
      comparer = Comparer.Default;
    }

    /// <summary>
    /// Creates an empty SortedList with the default initial capacity
    /// and specified comparer.
    /// </summary>
    /// <param name="comparer">
    /// The IComparer to use to determine whether two keys are equal.
    /// </param>
    public SortedListEx( IComparer comparer ) : this()
    {
      if( comparer != null ) this.comparer = comparer;
    }

    /// <summary>
    /// Creates an empty SortedList with the specified initial capacity
    /// and specified comparer.
    /// </summary>
    /// <param name="comparer">Initial capacity</param>
    /// <param name="capacity">
    /// The IComparer to use to determine whether two keys are equal
    /// </param>
    public SortedListEx( IComparer comparer, int capacity ) : this( comparer )
    {
      Capacity = capacity;
    }

    /// <summary>
    /// Copies the elements from the specified dictionary to a new list
    /// with the same initial capacity as the number of elements copied
    /// </summary>
    /// <param name="d">The IDictionary to copy</param>
    public SortedListEx( IDictionary d )  : this( d, null )
    {
    }

    /// <summary>
    /// Copies the elements from the specified dictionary to a new list with the same
    /// initial capacity as the number of elements copied and with the specified comparer
    /// </summary>
    /// <param name="d">The IDictionary to copy</param>
    /// <param name="comparer">
    /// The IComparer to use to determine whether two keys are equal.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When argument d is null
    /// </exception>
    public SortedListEx( IDictionary d, IComparer comparer )
      : this( comparer, ( d != null ? d.Count : 0 ) )
    {
      if( d == null )
        throw new ArgumentNullException( "d" );

      // create copy of dictionary values
      d.Keys.CopyTo( keys, 0 );
      values = new Hashtable( d );

      Array.Sort( keys, comparer );
      _size = d.Count;
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Returns a synchronized ( thread-safe ) wrapper for the SortedList
    /// </summary>
    /// <param name="list">The SortedList to synchronize</param>
    /// <returns>A synchronized ( thread-safe ) wrapper for the SortedList</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When list is null
    /// </exception>
    public static SortedListEx Synchronized( SortedListEx list )
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      return new SyncSortedListEx( list );
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds an element with the provided key and value to the list
    /// </summary>
    /// <param name="key">The Object to use as the key of the element to add</param>
    /// <param name="value">The Object to use as the value of the element to add</param>
    /// <exception cref="System.ArgumentNullException">
    /// When key is null
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When list already contains specified key
    /// </exception>
    public virtual void     Add( object key, object value )
    {
      if( key == null )
        throw new ArgumentNullException( "key" );

      if( values.ContainsKey( key ) )
        throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_104 );

      Insert( _size, key, value );
    }
    /// <summary>
    /// Removes all elements from the collection
    /// </summary>
    public virtual void     Clear()
    {
      version++;
      _size = 0;

      keys = new Object[ _defaultCapacity ];
      values = new Hashtable( _defaultCapacity );
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance</returns>
    public virtual object   Clone()
    {
      SortedListEx sl = new SortedListEx( _size );
      Array.Copy( keys, 0, sl.keys, 0, _size );
      sl.values = new Hashtable( values );
      sl._size = _size;
      sl.version = version;
      sl.comparer = comparer;

      // Don't copy keyList nor valueList.
      return sl;
    }
    /// <summary>
    /// Determines whether the list contains an element with the specified key
    /// </summary>
    /// <param name="key">key of the element to search</param>
    /// <returns>TRUE if list contains specified key</returns>
    public virtual bool     Contains( object key )
    {
      return values.ContainsKey( key );
    }
    /// <summary>
    /// Determines whether the list contains an element with the specified key
    /// </summary>
    /// <param name="key">key of the element to search</param>
    /// <returns>TRUE if list contains specified key</returns>
    public virtual bool     ContainsKey( object key )
    {
      // Yes, this is a SPEC'ed duplicate of Contains().
      return values.ContainsKey( key );
    }
    /// <summary>
    /// Determines whether the list contains the specified value
    /// </summary>
    /// <param name="value">value of the element to search</param>
    /// <returns>TRUE if list contains specified value</returns>
    public virtual bool     ContainsValue( object value )
    {
      return values.ContainsValue( value );
    }
    /// <summary>
    /// Copies all the elements of the SortedListEx to the specified one-dimensional Array
    /// starting at the specified destination Array index
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the destination of the
    /// elements copied from the current list</param>
    /// <param name="arrayIndex">The index in array at which copying begins</param>
    /// <exception cref="System.ArgumentNullException">
    /// If specified array is null
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If rank of the array is not 1 or there is not enough elements
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If specified arrayIndex is less than zero
    /// </exception>
    public virtual void     CopyTo( Array array, int arrayIndex )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );
      if( array.Rank != 1 )
        throw new ArgumentException();
      if( arrayIndex < 0 )
        throw new ArgumentOutOfRangeException( "arrayIndex" );
      if( array.Length - arrayIndex < Count )
        throw new ArgumentException();

      for( int i = 0; i<Count; i++ )
      {
        DictionaryEntry entry = new DictionaryEntry( keys[ i ], values[ keys[ i ] ] );
        array.SetValue( entry, i + arrayIndex );
      }
    }
    /// <summary>
    /// Gets the value at the specified index of the SortedListEx
    /// </summary>
    /// <param name="index">The zero-based index of the value to get</param>
    /// <returns>The value at the specified index of the SortedListEx</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list
    /// </exception>
    public virtual object   GetByIndex( int index )
    {
      if( index < 0 || index >= _size )
        throw new ArgumentOutOfRangeException( "index" );

      return values[ keys[ index ] ];
    }
    /// <summary>
    /// Gets the key at the specified index of the SortedListEx
    /// </summary>
    /// <param name="index">The zero-based index of the key to get</param>
    /// <returns>The key at the specified index of the SortedListEx</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list
    /// </exception>
    public virtual object   GetKey( int index )
    {
      if( index < 0 || index >= _size )
        throw new ArgumentOutOfRangeException( "index" );

      return keys[ index ];
    }
    /// <summary>
    /// Gets the keys in the SortedListEx
    /// </summary>
    /// <returns>An IList containing the keys in the SortedListEx</returns>
    public virtual IList    GetKeyList()
    {
      if( keyList == null ) keyList = new KeyList( this );
      return keyList;
    }
    /// <summary>
    /// Gets the values in the SortedListEx
    /// </summary>
    /// <returns>An IList containing the values in the SortedListEx</returns>
    public virtual IList    GetValueList()
    {
      if( valueList == null )
        valueList = new ValueList( this );
      else
        valueList.UpdateValues();

      return valueList;
    }
    /// <summary>
    /// Returns the zero-based index of the specified key
    /// </summary>
    /// <param name="key">The key to locate</param>
    /// <returns>The zero-based index of key, if key is found; otherwise, -1</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If specified key is null
    /// </exception>
    public virtual int      IndexOfKey( object key )
    {
      if( key == null )
        throw new ArgumentNullException( "key" );

      int ret = Array.BinarySearch( keys, 0, _size, key, comparer );

      return ret >= 0 ? ret : -1;
    }
    /// <summary>
    /// Returns the zero-based index of the first occurrence of the specified value
    /// </summary>
    /// <param name="value">The value to locate ( can be NULL )</param>
    /// <returns>
    /// The zero-based index of the first occurrence of value, if value is found;
    /// otherwise, -1.
    /// </returns>
    public virtual int      IndexOfValue( object value )
    {
      object key = null;

      IDictionaryEnumerator enm = values.GetEnumerator();
      enm.Reset();

      while( enm.MoveNext() )
      {
        if( enm.Value.Equals( value ) )
        {
          key = enm.Key;
          break;
        }
      }

      if( key == null ) return -1;

      return Array.IndexOf( keys, key, 0, _size );
    }
    /// <summary>
    /// Removes the element at the specified index
    /// </summary>
    /// <param name="index">The zero-based index of the element to remove</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list
    /// </exception>
    public virtual void     RemoveAt( int index )
    {
      if( index < 0 || index >= _size )
        throw new ArgumentOutOfRangeException( "index" );

      _size--;
      object key = keys[ index ] ;
      if( index < _size )
      {
        Array.Copy( keys, index + 1, keys, index, _size - index );
      }
      keys[ _size ] = null;
      values.Remove( key );
      version++;
    }
    /// <summary>
    ///Removes the element with the specified key from SortedListEx
    /// </summary>
    /// <param name="key">The key of the element to remove</param>
    public virtual void     Remove( object key )
    {
      int i = IndexOfKey( key );
      if( i >= 0 ) RemoveAt( i );
    }
    /// <summary>
    /// Replaces the value at the specific index
    /// </summary>
    /// <param name="index">The zero-based index at which to save value</param>
    /// <param name="value">The Object to save into. Can be NULL</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When index is less than zero or greater than size of the list
    /// </exception>
    public virtual void     SetByIndex( int index, object value )
    {
      if( index < 0 || index >= _size )
        throw new ArgumentOutOfRangeException( "index" );

      values[ keys[ index ] ] = value;

      version++;
    }
    /// <summary>
    /// Sets the capacity to the actual number of elements
    /// </summary>
    public virtual void     TrimToSize()
    {
      Capacity = _size;
    }
    /// <summary>
    /// Returns an IDictionaryEnumerator that can iterate through the SortedListEx
    /// </summary>
    /// <returns>An IDictionaryEnumerator for the SortedListEx</returns>
    public virtual IDictionaryEnumerator GetEnumerator()
    {
      return new SortedListExEnumerator( this, 0, _size, SortedListExEnumerator.DictEntry );
    }
    /// <summary>
    /// Returns an IEnumerator that can iterate through the SortedListEx
    /// </summary>
    /// <returns>An IEnumerator for the SortedListEx</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return new SortedListExEnumerator( this, 0, _size, SortedListExEnumerator.DictEntry );
    }
    #endregion

    #region Private helper methods
    /// <summary>
    /// Inserts element with specified key and value at specified index
    /// </summary>
    /// <param name="index">The zero-based index to insert element at</param>
    /// <param name="key">The key of the element to insert</param>
    /// <param name="value">The value of the element to insert</param>
    private void Insert( int index, object key, object value )
    {
      if( _size == keys.Length )
        EnsureCapacity( _size + 1 );

      if( index < _size )
      {
        Array.Copy( keys, index, keys, index + 1, _size - index );
      }

      keys[ index ] = key;
      values[ key ] = value;

      _size++;
      version++;
    }

    /// <summary>
    /// Ensures that the capacity of this instance is at least the specified value
    /// </summary>
    /// <param name="min">The minimum capacity to ensure</param>
    private void EnsureCapacity( int min )
    {
      int newCapacity = keys.Length == 0? 16: keys.Length * 2;
      if( newCapacity < min ) newCapacity = min;
      Capacity = newCapacity;
    }
    #endregion

    #region Internal classes declarations
    [ Serializable() ]
    private class SyncSortedListEx : SortedListEx
    {
      #region Class members
      /// <summary>
      /// Wrapped SortedListEx
      /// </summary>
      private SortedListEx  _list;
      /// <summary>
      /// Sync object
      /// </summary>
      private object        _root;
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Creates SyncSortedListEx for specified SortedListEx
      /// </summary>
      /// <param name="list">SortedListEx that will be wrapped</param>
      internal SyncSortedListEx( SortedListEx list )
      {
        _list = list;
        _root = list.SyncRoot;
      }
      #endregion

      #region Class Properties
      /// <summary>
      /// Capacity of internal buffers
      /// </summary>
      public override int Capacity
      {
        get{ lock( _root ) {  return _list.Capacity; } }
      }

      /// <summary>
      /// Size of the collection. Read-only
      /// </summary>
      public override int Count
      {
        get { lock( _root ) { return _list.Count; } }
      }

      /// <summary>
      /// Returns the object that can be used to synchronize access to the collection.
      /// Read-only.
      /// </summary>
      public override object SyncRoot
      {
        get { return _root; }
      }

      /// <summary>
      /// Returns TRUE if list is readonly, FALSE otherwise. Read-only
      /// </summary>
      public override bool IsReadOnly
      {
        get { return _list.IsReadOnly; }
      }

      /// <summary>
      /// Returns TRUE if collection has fixed size, FALSE otherwise.
      /// </summary>
      public override bool IsFixedSize
      {
        get { return _list.IsFixedSize; }
      }

      /// <summary>
      /// Returns TRUE if the collection is synchronized, FALSE otherwise
      /// </summary>
      public override bool IsSynchronized
      {
        get { return true; }
      }

      /// <summary>
      /// Gets or sets the value associated with the specified key.
      /// </summary>
      public override object this[ object key ]
      {
        get
        {
          lock( _root )
          {
            return _list[ key ];
          }
        }
        set
        {
          lock( _root )
          {
            _list[ key ] = value;
          }
        }
      }

      #endregion

      #region Public Methods
      /// <summary>
      /// Adds an element with the provided key and value to the list
      /// </summary>
      /// <param name="key">The Object to use as the key of the element to add</param>
      /// <param name="value">The Object to use as the value of the element to add</param>
      public override void    Add( object key, object value )
      {
        lock( _root )
        {
          _list.Add( key, value );
        }
      }

      /// <summary>
      /// Removes all elements from collection
      /// </summary>
      public override void    Clear()
      {
        lock( _root )
        {
          _list.Clear();
        }
      }

      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>A new object that is a copy of the current instance.</returns>
      public override object  Clone()
      {
        lock( _root )
        {
          return _list.Clone();
        }
      }

      /// <summary>
      /// Determines whether the list contains an element with the specified key
      /// </summary>
      /// <param name="key">key of the element to search</param>
      /// <returns>TRUE if list contains specified key</returns>
      public override bool    Contains( object key )
      {
        lock( _root )
        {
          return _list.Contains( key );
        }
      }

      /// <summary>
      /// Determines whether the list contains an element with the specified key
      /// </summary>
      /// <param name="key">key of the element to search</param>
      /// <returns>TRUE if list contains specified key</returns>
      public override bool ContainsKey( object key )
      {
        lock( _root )
        {
          return _list.ContainsKey( key );
        }
      }

      /// <summary>
      /// Determines whether the list contains the specified value
      /// </summary>
      /// <param name="value">value of the element to search</param>
      /// <returns>TRUE if list contains specified value</returns>
      public override bool ContainsValue( object value )
      {
        lock( _root )
        {
          return _list.ContainsValue( value );
        }
      }

      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list</param>
      /// <param name="index">The index in array at which copying begins</param>
      public override void    CopyTo( Array array, int index )
      {
        lock( _root )
        {
          _list.CopyTo( array, index );
        }
      }

      /// <summary>
      /// Gets the value at the specified index of the list
      /// </summary>
      /// <param name="index">The zero-based index of the value to get</param>
      /// <returns>The value at the specified index of the SortedListEx</returns>
      public override object  GetByIndex( int index )
      {
        lock( _root )
        {
          return _list.GetByIndex( index );
        }
      }

      /// <summary>
      /// Returns an IDictionaryEnumerator that can iterate through the list
      /// </summary>
      /// <returns>An IDictionaryEnumerator for the list</returns>
      public override IDictionaryEnumerator GetEnumerator()
      {
        lock( _root )
        {
          return _list.GetEnumerator();
        }
      }

      /// <summary>
      /// Gets the key at the specified index of the list
      /// </summary>
      /// <param name="index">The zero-based index of the key to get</param>
      /// <returns>The key at the specified index of the list</returns>
      public override object  GetKey( int index )
      {
        lock( _root )
        {
          return _list.GetKey( index );
        }
      }

      /// <summary>
      /// Gets the keys in the list
      /// </summary>
      /// <returns>An IList containing the keys in the list</returns>
      public override IList   GetKeyList()
      {
        lock( _root )
        {
          return _list.GetKeyList();
        }
      }

      /// <summary>
      /// Gets the values in the list
      /// </summary>
      /// <returns>An IList containing the values in the list</returns>
      public override IList   GetValueList()
      {
        lock( _root )
        {
          return _list.GetValueList();
        }
      }

      /// <summary>
      /// Returns the zero-based index of the specified key
      /// </summary>
      /// <param name="key">The key to locate</param>
      /// <returns>The zero-based index of key, if key is found; otherwise, -1</returns>
      public override int     IndexOfKey( object key )
      {
        lock( _root )
        {
          return _list.IndexOfKey( key );
        }
      }

      /// <summary>
      /// Returns the zero-based index of the first occurrence of the specified value
      /// </summary>
      /// <param name="value">The value to locate ( can be NULL )</param>
      /// <returns>
      /// The zero-based index of the first occurrence of value, if value is found;
      /// otherwise, -1.
      /// </returns>
      public override int     IndexOfValue( object value )
      {
        lock( _root )
        {
          return _list.IndexOfValue( value );
        }
      }

      /// <summary>
      /// Removes the element at the specified index
      /// </summary>
      /// <param name="index">The zero-based index of the element to remove</param>
      public override void    RemoveAt( int index )
      {
        lock( _root )
        {
          _list.RemoveAt( index );
        }
      }

      /// <summary>
      ///Removes the element with the specified key from list
      /// </summary>
      /// <param name="key">The key of the element to remove</param>
      public override void    Remove( object key )
      {
        lock( _root )
        {
          _list.Remove( key );
        }
      }

      /// <summary>
      /// Replaces the value at a specific index
      /// </summary>
      /// <param name="index">The zero-based index at which to save value</param>
      /// <param name="value">The Object to save into. Can be NULL</param>
      public override void    SetByIndex( int index, object value )
      {
        lock( _root )
        {
          _list.SetByIndex( index, value );
        }
      }

      /// <summary>
      /// Sets the capacity to the actual number of elements
      /// </summary>
      public override void    TrimToSize()
      {
        lock( _root )
        {
          _list.TrimToSize();
        }
      }
      #endregion
    }

    [ Serializable() ]
    private class SortedListExEnumerator : IDictionaryEnumerator, ICloneable
    {
      #region Class constants
      /// <summary>
      /// If it is assumed to getObjectRetType then Current will return key
      /// of the current element
      /// </summary>
      internal const int Keys = 1;
      /// <summary>
      /// If it is assumed to getObjectRetType then Current will return value
      /// of the current element
      /// </summary>
      internal const int Values = 2;
      /// <summary>
      /// If it is assumed to getObjectRetType then Current will return both -
      /// key and value ( as DictionaryEntry )
      /// </summary>
      internal const int DictEntry = 3;
      #endregion

      #region Class members
      /// <summary>
      /// List for which is this enumerator
      /// </summary>
      private SortedListEx  SortedListEx;
      /// <summary>
      /// Key of the current element
      /// </summary>
      private object        key;
      /// <summary>
      /// Values of the current element
      /// </summary>
      private object        value;
      /// <summary>
      /// Index of current element
      /// </summary>
      private int           index;
      /// <summary>
      /// Starting index for the enumerator
      /// </summary>
      private int           startIndex;
      /// <summary>
      /// Ending index for this enumerator
      /// </summary>
      private int           endIndex;
      /// <summary>
      /// version of collection data
      /// </summary>
      private int           version;
      /// <summary>
      /// TRUE if current element is correct, FALSE otherwise
      /// ( before beginning or after end )
      /// </summary>
      private bool          current;
      /// <summary>
      /// Specifies what should return method Current ( Key, Value or both )
      /// </summary>
      private int           getObjectRetType;
      #endregion

      #region Class initialize methods
      /// <summary>
      /// Creates enumerator for specified list, starting from specified index
      /// and with specified count
      /// </summary>
      /// <param name="SortedListEx">list for which to create enumerator</param>
      /// <param name="index">starting index</param>
      /// <param name="count">number of elements to enumerate</param>
      /// <param name="getObjRetType">type of enumerating values ( keys, value, DicEntry )</param>
      internal SortedListExEnumerator( SortedListEx SortedListEx, int index, int count,
        int getObjRetType )
      {
        this.SortedListEx = SortedListEx;
        this.index = index;
        startIndex = index;
        endIndex = index + count;
        version = SortedListEx.version;
        getObjectRetType = getObjRetType;
        current = false;
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Creates a new object that is a copy of the current instance.
      /// </summary>
      /// <returns>copy of the current instance</returns>
      public object Clone()
      {
        return MemberwiseClone();
      }

      /// <summary>
      /// Returns key of the current element. Read-only.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false
      /// </exception>
      public virtual object Key
      {
        get
        {
          if( version != SortedListEx.version )
            throw new InvalidOperationException();

          if( current == false )
            throw new InvalidOperationException();

          return key;
        }
      }

      /// <summary>
      /// Advances the enumerator to the next element of the collection
      /// </summary>
      /// <returns>
      /// TRUE if the enumerator was successfully advanced to the next element;
      /// FALSE if the enumerator has passed the end of the collection
      /// </returns>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// </exception>
      public virtual bool MoveNext()
      {
        if( version != SortedListEx.version )
          throw new InvalidOperationException();

        if( index < endIndex )
        {
          key = SortedListEx.keys[ index ];
          value = SortedListEx.values[ key ];
          index++;
          current = true;
          return true;
        }
        key = null;
        value = null;
        current = false;
        return false;
      }

      /// <summary>
      /// Return DictionaryEntry for the current element
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false
      /// </exception>
      public virtual DictionaryEntry Entry
      {
        get
        {
          if( version != SortedListEx.version )
            throw new InvalidOperationException();

          if( current == false )
            throw new InvalidOperationException();

          return new DictionaryEntry( key, value );
        }
      }

      /// <summary>
      /// The current element in the collection. Read-only.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// If current is false
      /// </exception>
      public virtual object Current
      {
        get
        {
          if( current == false )
            throw new InvalidOperationException();

          if( getObjectRetType==Keys )
            return key;
          else if( getObjectRetType==Values )
            return value;
          else
            return new DictionaryEntry( key, value );
        }
      }

      /// <summary>
      /// Returns value for the current element
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// or if current is false
      /// </exception>
      public virtual object Value
      {
        get
        {
          if( version != SortedListEx.version )
            throw new InvalidOperationException();
          if( current == false )
            throw new InvalidOperationException();
          return value;
        }
      }

      /// <summary>
      /// Sets the enumerator to its initial position, which is before
      /// the first element in the collection.
      /// </summary>
      /// <exception cref="System.InvalidOperationException">
      /// When the current version is not equal to the SortedListEx version
      /// </exception>
      public virtual void Reset()
      {
        if( version != SortedListEx.version )
          throw new InvalidOperationException();
        index = startIndex;
        current = false;
        key = null;
        value = null;
      }
      #endregion
    }

    [ Serializable() ]
    private class KeyList : IList
    {
      #region Class members
      /// <summary>
      /// List for which this collection was created
      /// </summary>
      private SortedListEx SortedListEx;
      #endregion

      #region Class initialize methods
      /// <summary>
      /// Creates KeyList for specified SortedListEx
      /// </summary>
      /// <param name="SortedListEx">SortedListEx for which KeyList must be created</param>
      internal KeyList( SortedListEx SortedListEx )
      {
        this.SortedListEx = SortedListEx;
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Size of the collection. Read-only
      /// </summary>
      public virtual int Count
      {
        get { return SortedListEx._size; }
      }

      /// <summary>
      /// Returns TRUE if list is readonly, FALSE otherwise. Read-only
      /// </summary>
      public virtual bool IsReadOnly
      {
        get { return true; }
      }

      /// <summary>
      /// Returns TRUE if collection has fixed size, FALSE otherwise.
      /// </summary>
      public virtual bool IsFixedSize
      {
        get { return true; }
      }

      /// <summary>
      /// Returns TRUE if the collection is synchronized, FALSE otherwise
      /// </summary>
      public virtual bool IsSynchronized
      {
        get { return SortedListEx.IsSynchronized; }
      }

      /// <summary>
      /// Returns the object that can be used to synchronize access to the collection.
      /// Read-only.
      /// </summary>
      public virtual object SyncRoot
      {
        get { return SortedListEx.SyncRoot; }
      }

      /// <summary>
      /// Adds an element with the provided key to the list
      /// </summary>
      /// <param name="key">The Object to use as the key of the element to add</param>
      public virtual int Add( object key )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Removes all elements from the collection
      /// </summary>
      public virtual void Clear()
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Determines whether the list contains an element with the specified key
      /// </summary>
      /// <param name="key">key of the element to search</param>
      /// <returns>TRUE if list contains specified key</returns>
      public virtual bool Contains( object key )
      {
        return SortedListEx.Contains( key );
      }

      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list</param>
      /// <param name="arrayIndex">The index in array at which copying begins</param>
      /// <exception cref="System.ArgumentException">
      /// If array is null or rank of the array is not 1
      /// </exception>
      public virtual void CopyTo( Array array, int arrayIndex )
      {
        if( array != null && array.Rank != 1 )
          throw new ArgumentException();

        // defer error checking to Array.Copy
        Array.Copy( SortedListEx.keys, 0, array, arrayIndex, SortedListEx.Count );
      }

      /// <summary>
      /// Insert the value at the specific index
      /// </summary>
      /// <param name="index">The zero-based index at which to save value</param>
      /// <param name="value">The Object to save into. Can be NULL</param>
      public virtual void Insert( int index, object value )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Gets or sets the key at the specified index
      /// </summary>
      public virtual object this[ int index ]
      {
        get
        {
          return SortedListEx.GetKey( index );
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      /// <summary>
      /// Returns an IEnumerator that can iterate through the list
      /// </summary>
      /// <returns>An IEnumerator for the list</returns>
      public virtual IEnumerator GetEnumerator()
      {
        return new SortedListExEnumerator( SortedListEx, 0, SortedListEx.Count, SortedListExEnumerator.Keys );
      }

      /// <summary>
      /// Returns the zero-based index of the specified key
      /// </summary>
      /// <param name="key">The key to locate</param>
      /// <returns>The zero-based index of the key, if the key is found; otherwise, -1</returns>
      public virtual int IndexOf( object key )
      {
        if( key==null )
          throw new ArgumentNullException( "key" );

        int i = Array.BinarySearch( SortedListEx.keys, 0,
          SortedListEx.Count, key, SortedListEx.comparer );
        if( i >= 0 ) return i;
        return -1;
      }

      /// <summary>
      ///Removes the element with the specified key from list
      /// </summary>
      /// <param name="key">The key of the element to remove</param>
      public virtual void Remove( object key )
      {
        throw new NotSupportedException();
      }

      /// <summary>
      /// Removes the element at the specified index from the list
      /// </summary>
      /// <param name="index">The zero-based index of the element to remove</param>
      public virtual void RemoveAt( int index )
      {
        throw new NotSupportedException();
      }
      #endregion
    }

    [ Serializable() ]
    private class ValueList : IList
    {
      #region Class members
      /// <summary>
      /// List for which this collection was created
      /// </summary>
      private SortedListEx SortedListEx;
      /// <summary>
      /// Array of values.
      /// </summary>
      private Array vals;
      #endregion

      #region Class Initialize/Finalize methods
      /// <summary>
      /// Creates ValueList for specified SortedListEx
      /// </summary>
      /// <param name="SortedListEx">SortedListEx for which ValueList must be created</param>
      internal ValueList( SortedListEx SortedListEx )
      {
        this.SortedListEx = SortedListEx;
        UpdateValues();
      }
      #endregion

      #region Class public methods
      /// <summary>
      /// Re-read values from the list
      /// </summary>
      public virtual void UpdateValues()
      {
        vals = new ArrayList( SortedListEx.values.Values ).ToArray();
        Array keys = new ArrayList( SortedListEx.values.Keys ).ToArray();
        Array.Sort( keys, vals, SortedListEx.comparer );
      }
      /// <summary>
      /// Size of the collection. Read-only
      /// </summary>
      public virtual int Count
      {
        get { return SortedListEx._size; }
      }
      /// <summary>
      /// Returns TRUE if list is readonly, FALSE otherwise. Read-only
      /// </summary>
      public virtual bool IsReadOnly
      {
        get { return true; }
      }
      /// <summary>
      /// Returns TRUE if collection has fixed size, FALSE otherwise.
      /// </summary>
      public virtual bool IsFixedSize
      {
        get { return true; }
      }
      /// <summary>
      /// Returns TRUE if the collection is synchronized, FALSE otherwise
      /// </summary>
      public virtual bool IsSynchronized
      {
        get { return SortedListEx.IsSynchronized; }
      }
      /// <summary>
      /// Returns the object that can be used to synchronize access to the collection.
      /// Read-only.
      /// </summary>
      public virtual object SyncRoot
      {
        get { return SortedListEx.SyncRoot; }
      }
      /// <summary>
      /// Adds an element with the provided key to the list
      /// </summary>
      /// <param name="key">The Object to use as the key of the element to add</param>
      public virtual int Add( object key )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Removes all elements from the collection
      /// </summary>
      public virtual void Clear()
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Determines whether the list contains an element with the specified value
      /// </summary>
      /// <param name="value">value to search</param>
      /// <returns>TRUE if list contains specified value</returns>
      public virtual bool Contains( object value )
      {
        return SortedListEx.ContainsValue( value );
      }
      /// <summary>
      /// Copies all the elements of the list to the specified one-dimensional Array
      /// starting at the specified destination Array index
      /// </summary>
      /// <param name="array">The one-dimensional Array that is the destination of the
      /// elements copied from the current list</param>
      /// <param name="arrayIndex">The index in array at which copying begins</param>
      public virtual void CopyTo( Array array, int arrayIndex )
      {
        if( array != null && array.Rank != 1 )
          throw new ArgumentException();

        // defer error checking to Array.Copy
        Array.Copy( vals, 0, array, arrayIndex, SortedListEx.Count );
      }
      /// <summary>
      /// Insert the value at the specific index
      /// </summary>
      /// <param name="index">The zero-based index at which to save value</param>
      /// <param name="value">The Object to save into. Can be NULL</param>
      public virtual void Insert( int index, object value )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Gets or sets the value at the specified index
      /// </summary>
      public virtual object this[ int index ]
      {
        get
        {
          return SortedListEx.GetByIndex( index );
        }
        set
        {
          SortedListEx.SetByIndex( index,value );
        }
      }
      /// <summary>
      /// Returns an IEnumerator that can iterate through the list
      /// </summary>
      /// <returns>An IEnumerator for the list</returns>
      public virtual IEnumerator GetEnumerator()
      {
        return new SortedListExEnumerator( SortedListEx, 0, SortedListEx.Count, SortedListExEnumerator.Values );
      }
      /// <summary>
      /// Returns the zero-based index of the specified value
      /// </summary>
      /// <param name="value">The value to locate</param>
      /// <returns>The zero-based index of the value, if the value is found; otherwise, -1</returns>
      public virtual int IndexOf( object value )
      {
        return Array.IndexOf( vals, value, 0, SortedListEx.Count );
      }
      /// <summary>
      ///Removes the specified value from list
      /// </summary>
      /// <param name="value">The value to remove</param>
      public virtual void Remove( object value )
      {
        throw new NotSupportedException();
      }
      /// <summary>
      /// Removes the element at the specified index from the list
      /// </summary>
      /// <param name="index">The zero-based index of the element to remove</param>
      public virtual void RemoveAt( int index )
      {
        throw new NotSupportedException();
      }
      #endregion
    }
    #endregion
  }
}