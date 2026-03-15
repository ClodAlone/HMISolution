#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SyncfusionFramework1_0 || SyncfusionFramework1_1

using System;
using System.Collections;
using System.Text;

using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation.Collections
{
    /// <summary>
    ///    Extends ArrayList with MoveRange, InsertRange and RemoveRange methods. The Item property
    ///    will grow the array on demand or return NULL if an index is out of range.
    /// </summary>
    public class SFArrayList : ArrayList
    {
        #region Constructors

        /// <summary>
        /// Overloaded. Initializes a new instance of the SFArrayList
        /// class that is empty and has the default initial capacity.
        /// </summary>
        public SFArrayList()
            : base()
        {
        }

        /// <summary>
        ///   <para>Initializes a new instance of the SFArrayList class that contains elements copied from the specified
        /// collection and has the same initial capacity as the number of elements copied.</para>
        /// </summary>
        /// <param name="c">The <see cref="System.Collections.ICollection" /> whose elements are copied to the new list.</param>
        public SFArrayList( ICollection c )
            : base(c)
        {
        }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// Gets / sets the element at the specified index.
        /// In C#, this property is the indexer for the SFArrayList class.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get / set.</param>
        /// <value>
        /// The element at the specified index.
        /// When querying the value and the index is out of range, an empty (<see langword="null" />) object will be returned.
        /// When setting the value and the index is out of range the array will be enlarged. See <see cref="SFArrayList.EnsureCount"/>
        /// </value>
        public override object this[int index]
        {
            get
              {
            return ( index >= Count || index < 0 )
              ? null
              : base[ index ];
              }
              set
              {
            this.EnsureCount( index + 1 );
            base[ index ] = value;
              }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        ///   <para>Overridden. Creates a deep copy of the SFArrayList.</para>
        /// </summary>
        /// <returns>
        ///   <para>A deep copy of the SFArrayList.</para>
        /// </returns>
        public override object Clone()
        {
            SFArrayList al = new SFArrayList();

              foreach( object o in this )
              {
            object objToAdd = ( o is ICloneable )
              ? ( ( ICloneable )o ).Clone()
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
            SFArrayList al = new SFArrayList();

              foreach( object o in this )
              {
            object obj = o;
            ICloneParent cloneParent = obj as ICloneParent;

            if( cloneParent != null )
            {
              obj = cloneParent.Clone( parent );
            }

            al.Add( obj );
              }

              return al;
        }

        /// <summary>
        /// Enlarges the array if needed.
        /// </summary>
        /// <param name="value">The size to be checked. If the array has less elements, empty (<see langword="null"/>) objects will be appended
        /// at the end of the array.</param>
        public void EnsureCount( int value )
        {
            int iCount = Count;

              if( iCount < value )
            AddRange( new object[ value - iCount ] );
        }

        #endregion Methods
    }
}

#endif