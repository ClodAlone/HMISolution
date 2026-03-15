//-------------------------------------------------------------------------------------------------
// <copyright file="RecordsInGroup.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using Syncfusion.Grouping;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// A Read-only collection of sorted <see cref="Record"/> elements that meet filter criteria and are children of a <see cref="Group"/>.
    /// See <see cref="RecordFilterDescriptorCollection"/> or <see cref="TableDescriptor.RecordFilters"/> for filter criteria.
    /// An instance of this collection is returned by the <see cref="Group.FilteredRecords"/> property
    /// of a <see cref="Group"/> object. <para/>
    /// The collection
    /// provides support for determining a record's position in the grouped group using the <see cref="FlattenedRecordsInGroupCollectionBase.IndexOf"/>
    /// method.
    /// </summary>
    public class FlattenedFilteredRecordsInGroupCollection : FlattenedRecordsInGroupCollectionBase
    {
        /// <summary>
        /// Initializes the FlattenedFilteredRecordsInGroupCollection
        /// </summary>
        /// <param name="group">The Group.</param>
        public FlattenedFilteredRecordsInGroupCollection(Group group)
            : base(group)
        {
            counterKind = CounterKind.FilteredRecordsCount;
        }
    }

    /// <summary>
    /// A Read-only collection of sorted <see cref="Record"/> elements that are children of a <see cref="Group"/>.
    /// An instance of this collection is returned by the <see cref="Group.Records"/> property
    /// of a <see cref="Group"/> object. This collection contains all records, it is not filtered. <para/>
    /// The collection
    /// provides support for determining a record's position in the grouped group using the <see cref="FlattenedRecordsInGroupCollectionBase.IndexOf"/>
    /// method.
    /// </summary>
    public class FlattenedRecordsInGroupCollection : FlattenedRecordsInGroupCollectionBase
    {
        /// <summary>
        /// Initializes the FlattenedRecordsInGroupCollection
        /// </summary>
        /// <param name="group">The Group.</param>
        public FlattenedRecordsInGroupCollection(Group group)
            : base(group)
        {
            counterKind = CounterKind.RecordsCount;
        }
    }

    /// <summary>
    /// A Read-only collection base class for <see cref="FlattenedFilteredRecordsInGroupCollection"/> 
    /// and <see cref="FlattenedRecordsInGroupCollection"/>.
    /// </summary>
    public class FlattenedRecordsInGroupCollectionBase : IList, IDisposable
    {
        internal Group _group;
        internal Record lastElement;
        internal int lastIndex;
        internal int version;
        internal int lastCount;
        internal int counterKind = CounterKind.RecordsCount;

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object and releases internal objects.
        /// </summary>
        public void Dispose()
        {
            _group = null;
            lastElement = null;
        }

        #endregion

        internal FlattenedRecordsInGroupCollectionBase(Group group)
        {
            _group = group;
        }

        /// <summary>
        /// Gets (and caches) the element at the zero-based index.
        /// Setting is not supported and will throw an exception since the collection is Read-only.
        /// </summary>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// group if changes have been made to the group or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public Record this[int index]
        {
            get
            {
                int count = Count;
                if (index < 0 || index >= count)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (lastIndex != -1 && lastCount == count && version == _group.Engine.Version)
                {
                    if (index == lastIndex)
                    {
                        return lastElement;
                    }
                    else if (index == lastIndex + 1)
                    {
                        lastIndex++;
                        lastElement = (Record)ElementHelper.GetNextElementStepIn(lastElement, counterKind, typeof(Record));
                        Debug.Assert(lastElement == ElementHelper.__FindElement(_group, CounterFactory.CreateCounter(index, counterKind), typeof(Record), false));
                        return lastElement;
                    }
                }

                Element ele = ElementHelper.__FindElement(_group, CounterFactory.CreateCounter(index, counterKind), typeof(Record), false);
                lastElement = (Record)ele;
                lastCount = count;
                lastIndex = index;
                version = _group.Engine.Version;
                return lastElement;
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        ICounterFactory CounterFactory
        {
            get
            {
                return this._group.Engine.CounterFactory;
            }
        }

        /// <summary>
        /// Determines if the element belongs to this collection.
        /// </summary>
        /// <param name="value">The Object to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic).</param>
        /// <returns>True if item is found in the collection; otherwise, False.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// group if changes have been made to the group or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public bool Contains(Record value)
        {
            if (value == null)
            {
                return false;
            }

            value.EnsureInitialized(this, true);
            if (value.GetCounter().GetValue(counterKind) == 0)
            {
                return false;
            }

            return CheckParentGroup(value);
        }

        internal bool CheckParentGroup(Record value)
        {
            Group g = value.ParentGroup;
            while (g != null)
            {
                if (g == _group)
                {
                    return true;
                }

                g = g.ParentGroup;
            }

            return false;
        }
        
        /// <summary>
        /// Returns the zero-based index of the occurrence of the element in the collection.
        /// </summary>
        /// <param name="value">The element to locate in the collection. The value can be a NULL reference (Nothing in Visual Basic). </param>
        /// <returns>The zero-based index of the occurrence of the element within the entire collection, if found; otherwise, -1.</returns>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// group if changes have been made to the group or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public int IndexOf(Record value)
        {
            if (!Contains(value))
            {
                return -1;
            }

            double pos = ElementHelper.GetCumulatedPosition(value, counterKind);
            double offset = ElementHelper.GetCumulatedPosition(_group, counterKind);
            return (int)(pos - offset);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from ArrayList. The Array must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// group if changes have been made to the Group or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public void CopyTo(Record[] array, int index)
        {
            int n = 0;
            foreach (Record element in this)
            {
                array[index + n] = element;
                n++;
            }
        }

        ////        public FilteredRecordsInGroupCollection SyncRoot
        ////        {
        ////            get
        ////            {
        ////                return null;
        ////            }
        ////        }

        /// <summary>
        /// Returns an enumerator for the entire collection.
        /// </summary>
        /// <returns>An Enumerator for the entire collection.</returns>
        /// <remarks>Enumerators only allow reading the data in the collection. 
        /// Enumerators cannot be used to modify the underlying collection.</remarks>
        public FlattenedRecordsInGroupCollectionEnumerator GetEnumerator()
        {
            _group.EnsureInitialized(this);
            return new FlattenedRecordsInGroupCollectionEnumerator(this);
        }

        #region IList Members
        
        /// <summary>
        /// Returns True because this collection is always Read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                throw new InvalidOperationException("Collection is read only");
            }
        }

        /// <summary>
        /// Not supported because collection is Read-only.
        /// </summary>
        /// <param name="index">The list index</param>
        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        bool IList.Contains(object value)
        {
            return Contains((Record)value);
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((Record)value);
        }

        int IList.Add(object value)
        {
            throw new InvalidOperationException("Collection is Read-only.");
        }

        /// <summary>
        /// Returns False since this collection has no fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection. The property also
        /// ensures that the collection is in sync with the underlying
        /// table if changes have been made to the Table or the TableDescriptor.
        /// </summary>
        /// <remarks>
        /// The method calls <see cref="Table.EnsureInitialized"/>.
        /// </remarks>
        public int Count
        {
            get
            {
                if (_group == null || _group.IsDisposed)
                {
                    return 0;
                }

                _group.EnsureInitialized(this);
                return (int)_group.GetCounter().GetValue(counterKind);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((Record[])array, index);
        }

        object ICollection.SyncRoot
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Enumerator class for <see cref="Record"/> elements of a <see cref="FlattenedRecordsInGroupCollectionBase"/>.
    /// </summary>
    public class FlattenedRecordsInGroupCollectionEnumerator : IEnumerator
    {
        Record _cursor, _next;
        FlattenedRecordsInGroupCollectionBase _coll;

        /// <summary>
        /// Initalizes the enumerator and attaches it to the collection.
        /// </summary>
        /// <param name="collection">The parent collection to enumerate.</param>
        /// <remarks>
        /// The method 
        /// ensures that the collection is in sync with the underlying
        /// group if changes have been made to the group or the TableDescriptor.
        /// <para/>
        /// The method calls <see cref="Element.EnsureInitialized"/>.
        /// </remarks>
        public FlattenedRecordsInGroupCollectionEnumerator(FlattenedRecordsInGroupCollectionBase collection)
        {
            _coll = collection;
            _cursor = null;
            if (_coll.Count > 0)
            {
                _next = _coll[0];
            }
        }

        #region IEnumerator Members

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public virtual void Reset()
        {
            _cursor = null;
            _next = _coll[0];
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        public Record Current
        {
            get
            {
                return _cursor;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// True if the enumerator was successfully advanced to the next element; False if the enumerator has passed the end of the collection.
        /// </returns>
        public bool MoveNext()
        {
            if (_next == null)
            {
                return false;
            }

            _cursor = _next;

            _next = (Record)ElementHelper.GetNextElementStepIn(_next, _coll.counterKind, typeof(Record));

            if (_next != null && !_coll.CheckParentGroup(_next))
            {
                _next = null;
            }

            return _cursor != null;
        }
        #endregion
    }
}
