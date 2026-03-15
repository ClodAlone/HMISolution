#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.XlsIO.Interfaces;


namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  ///    Extends List with MoveRange, InsertRange and RemoveRange methods. The Item property
  ///    will grow the array on demand or return NULL if an index is out of range.
  /// </summary>
  public class SFArrayList<T>
    : List<T>
    , ICloneable
    where T : class
  {
    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded. Initializes a new instance of the SFArrayList
    /// class that is empty and has the default initial capacity.
    /// </summary>
    public SFArrayList()
      : base()
    {
    }

    /// <summary>
    ///   <para>Initializes a new instance of the SFArrayList class that contains
    /// elements copied from the specified collection and has the same initial
    /// capacity as the number of elements copied.</para>
    /// </summary>
    /// <param name="c">The <see cref="System.Collections.ICollection" /> whose elements are copied to the new list.</param>
    public SFArrayList( ICollection<T> c )
      : base( c )
    {
    }
    #endregion

    #region ICloneable members
    /// <summary>
    ///   <para>Overridden. Creates a deep copy of the SFArrayList.</para>
    /// </summary>
    /// <returns>
    ///   <para>A deep copy of the SFArrayList.</para>
    /// </returns>
    public object Clone()
    {
      SFArrayList<T> al = new SFArrayList<T>();

      foreach( T o in this )
      {
        T objToAdd = ( o is ICloneable )
          ? ( T )( ( ICloneable ) o ).Clone()
          : o;

        al.Add( objToAdd );
      }

      return al;
    }
    /// <summary>
    ///   <para>Overridden. Creates a deep copy of the SFArrayList.</para>
    /// </summary>
    /// <param name="parent">Parent object for the new items.</param>
    /// <returns>
    ///   <para>A deep copy of the SFArrayList.</para>
    /// </returns>
    public object Clone( object parent )
    {
      SFArrayList<T> al = new SFArrayList<T>();

      foreach( T o in this )
      {
        T obj = o;
        ICloneParent cloneParent = obj as ICloneParent;

        if( cloneParent != null )
        {
          obj = ( T )cloneParent.Clone( parent );
        }

        al.Add( obj );
      }

      return al;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Gets / sets the element at the specified index.
    /// In C#, this property is the indexer for the SFArrayList class.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get / set.</param>
    /// <value>
    /// The element at the specified index.
    /// When querying the value and the index is out of range, an empty (<see langword="null" />) object will be returned.
    /// When setting the value and the index is out of range the array will be enlarged. See SFArrayList.EnsureCount
    /// </value>
    new public T this[ int index ]
    {
      get
      {
        //return base[ index ];
        return( index >= Count || index < 0 )
          ? null
          : base[ index ];
      }
      set
      {
        this.EnsureCount( index + 1 );
        base[ index ] = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Enlarges the array if needed.
    /// </summary>
    /// <param name="value">The size to be checked. If the array has less elements, empty (<see langword="null"/>) objects will be appended
    /// at the end of the array.</param>
    public void EnsureCount( int value )
    {
      int iCount = Count;

      if( iCount < value )
        AddRange( new T[ value - iCount ] );
    }

    #endregion

  }
}
