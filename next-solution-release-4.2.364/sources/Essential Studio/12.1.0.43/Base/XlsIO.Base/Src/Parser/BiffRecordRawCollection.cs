#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    /// <summary>
    /// Collection of the Biff records.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude]
    [CLSCompliant( false )]
    public class BiffRecordRawCollection : System.Collections.CollectionBase
    {
        #region Indexers

        /// <summary>
        /// Gets / sets Biff record at the specified position.
        /// </summary>
        public BiffRecordRaw this[int index]
        {
            get
              {
            return ( ( BiffRecordRaw )List[ index ] );
              }
              set
              {
            List[ index ] = value;
              }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Adds an object to the end of the collection.
        /// </summary>
        /// <param name="value">Value to add.</param>
        /// <returns>Index of the added value.</returns>
        public int Add( BiffRecordRaw value )
        {
            return List.Add( value );
        }

        /// <summary>
        /// Determines whether the CollectionBase contains a specific element.
        /// </summary>
        /// <param name="value">Value to search.</param>
        /// <returns>True if value was found in the collection; otherwise False.</returns>
        public bool Contains( BiffRecordRaw value )
        {
            return InnerList.Contains( value );
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional
        /// Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">Array that will receive data from the collection.</param>
        /// <param name="index">
        /// The zero-based index in an array at which copying begins.
        /// </param>
        public void CopyTo( BiffRecordRaw[] array, int index )
        {
            List.CopyTo( array, index );
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the collection instance.
        /// </summary>
        /// <returns>
        /// An enumerator that can iterate through the collection instance.
        /// </returns>
        public new BiffRecordRawEnumerator GetEnumerator()
        {
            return new BiffRecordRawEnumerator( this );
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based
        /// index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="value">Value to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence within
        /// the entire collection.
        /// </returns>
        public int IndexOf( BiffRecordRaw value )
        {
            return InnerList.IndexOf( value );
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">Position at which item will be inserted.</param>
        /// <param name="value">Item to insert.</param>
        public void Insert( int index, BiffRecordRaw value )
        {
            List.Insert( index, value );
        }

        /// <summary>
        /// Removes value from the collection.
        /// </summary>
        /// <param name="value">Item to remove from the collection.</param>
        public void Remove( BiffRecordRaw value )
        {
            List.Remove( value );
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Biff records collection enumerator.
        /// </summary>
        public class BiffRecordRawEnumerator : System.Collections.IEnumerator
        {
            #region Fields

            /// <summary>
            /// Collection of the Biff records for which enumerator was created.
            /// </summary>
            private BiffRecordRawCollection m_collection;

            /// <summary>
            /// Current Biff record.
            /// </summary>
            private BiffRecordRaw m_current;

            /// <summary>
            /// Index of the current record.
            /// </summary>
            private int m_iPosition = -1;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Creates enumerator for the specified collection of Biff records.
            /// </summary>
            /// <param name="collection">
            /// Collection of Biff records for which enumerator is created.
            /// </param>
            public BiffRecordRawEnumerator( BiffRecordRawCollection collection )
            {
                m_collection = collection;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Read-only. Returns current biff record.
            /// </summary>
            public BiffRecordRaw Current
            {
                get
                {
                  if( m_current == null )
                throw new ArgumentException( "Call Reset() and then MoveNext() method first." );

                  return m_current;
                }
            }

            /// <summary>
            /// Read-only. Returns current Biff record.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                  return this.Current;
                }
            }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if it currently points at the valid record;
            /// otherwise False.
            /// </returns>
            public bool MoveNext()
            {
                if( m_iPosition < m_collection.Count - 1 )
                {
                  m_iPosition++;
                  m_current = m_collection[ m_iPosition ];
                  return true;
                }
                else
                {
                  m_current = null;
                  return false;
                }
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is
            /// before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                m_iPosition = -1;
                m_current = null;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}