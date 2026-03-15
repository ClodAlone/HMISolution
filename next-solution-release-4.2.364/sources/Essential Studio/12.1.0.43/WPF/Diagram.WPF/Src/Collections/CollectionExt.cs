// <copyright file="CollectionExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Represents a collection of <see cref="Node"/> and <see cref="LineConnector"/> objects.
    /// </summary>
    public class CollectionExt : IList, INotifyCollectionChanged, ICollectionViewFactory
    {
        #region Class Fields

        /// <summary>
        /// Indicates whether the value is cleared.
        /// </summary>
        private static bool m_cleared = false;

        /// <summary>
        /// Object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
        /// </summary>
        private object m_syncRoot = null;

        /// <summary>
        /// Refers to the Source collection object.
        /// </summary>
        private IEnumerable m_source;

        /// <summary>
        /// Refers to the internal data (nodes and connectors)objects.
        /// </summary>
        private ObservableCollection<object> m_dataInternal = new ObservableCollection<object>();
        private Dictionary<object, object> m_dataInternalDict = new Dictionary<object, object>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CollectionExt"/> is cleared.
        /// </summary>
        /// <value><c>true</c> if cleared; otherwise, <c>false</c>.</value>
        internal static bool Cleared
        {
            get { return m_cleared; }
            set { m_cleared = value; }
        }

        /// <summary>
        /// Gets or sets custom source collection.
        /// </summary>
        public IEnumerable SourceCollection
        {
            get
            {
                return m_source;
            }

            set
            {
                if (value != null && m_dataInternal.Count > 0)
                {
                    throw new InvalidOperationException("Source collection can not be used while collection contains elements itself.");
                }

                if (m_source != null)
                {
                    UnhookCollection(m_source);
                }

                if (value != null && m_source == null)
                {
                    UnhookCollection(m_dataInternal);
                }

                m_source = value;

                if (m_source != null)
                {
                    HookCollection(m_source);
                }
                else
                {
                    HookCollection(m_dataInternal);
                }

                if (CollectionChanged != null)
                {
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        /// <summary>
        /// Gets the source collection or internal data list  depending on the presence of the source collection.
        /// </summary>
        protected IEnumerable Data
        {
            get
            {
                return m_source ?? m_dataInternal;
            }
        }

        /// <summary>
        /// Gets the count of the items in the collection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Count
        {
            get
            {
                if (m_source != null)
                {
                    ICollection collection = m_source as ICollection;
                    if (collection == null)
                    {
                        throw new InvalidOperationException("Can not get the count of the items in a source collection.");
                    }

                    return collection.Count;
                }

                return m_dataInternal.Count;
            }
        }
        #endregion

        #region Events

        /// <summary>Occurs when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExt"/> class .
        /// </summary>
        public CollectionExt()
        {
            HookCollection(m_dataInternal);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Check if the element already exist before adding into collection.
        /// </summary>
        internal int AddIfDoesNotExist(object value)
        {
            if (!this.Contains(value))
                return this.Add(value);
            else
                return -1;
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public int Add(object value)
        {
            CheckEmptySource();
            if (!m_dataInternalDict.ContainsKey(value))
            {
                m_dataInternalDict.Add(value, null);
            }
            return ValidateIListSupport().Add(value);
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {
            CheckEmptySource();
            CollectionExt.Cleared = true;
            IList obj = ValidateIListSupport();
            obj.Clear();
            m_dataInternalDict.Clear();
        }

        /// <summary>Inserts an item to the <see cref="T:System.Collections.IList"></see> at the specified index.</summary>
        /// <param name="index">The zero-based index at which value should be inserted. </param>
        /// <param name="value">The <see cref="T:System.Object"></see> to insert into the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">value is null reference in the <see cref="T:System.Collections.IList"></see>.</exception>
        public void Insert(int index, object value)
        {
            CheckEmptySource();
            ValidateIListSupport().Insert(index, value);
            m_dataInternalDict.Add(value, null);
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> has a fixed size.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> has a fixed size; otherwise, false.</returns>
        public bool IsFixedSize
        {
            get
            {
                return ValidateIListSupport().IsFixedSize;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.IList"></see> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.IList"></see> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                IList data = Data as IList;
                return (data != null) ? data.IsReadOnly : false;
            }
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to remove from the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void Remove(object value)
        {
            if (value != null)
            {
                m_dataInternalDict.Remove(value);
            }
            if ((value is Node) && !(value is Group))
            {
                if ((value as Node).AllowDelete)
                {
                    CheckEmptySource();                    
                    ValidateIListSupport().Remove(value);
                    //(value as Node).IsSelected = false;

                    if ((value as Node).IsGrouped)
                    {
                        foreach (Group s in (value as Node).Groups)
                        {
                            (value as Node).IsGrouped = false;
                            if (!s.AllowSingleChild && s.NodeChildren.Count < 2)
                            {
                                ValidateIListSupport().Remove(s);
                                if ((value as Node).Groups.Count < 2 && s.NodeChildren.Count!= 0)
                                {
                                    if (s.NodeChildren[0] is Node)
                                    {
                                        (s.NodeChildren[0] as Node).IsGrouped = false;
                                    }//(s.NodeChildren[0] as Node).Groups.Remove(s);
                                    else if((s.NodeChildren[0] is LineConnector))
                                    {
                                        (s.NodeChildren[0] as LineConnector).IsGrouped = false;
                                    }
                                }
                            }

                        }
                    }
                }

            }
            else if (value is Group)
            {

                if ((value as Group).AllowDelete)
                {
                    for (int j = 0; j <= (value as Group).NodeChildren.Count - 1; j++)
                    {
                        if ((value as Group).NodeChildren[j] is Node && DiagramCommandManager.IsUnGroupCommand)
                        {
                            ((value as Group).NodeChildren[j] as Node).IsGrouped = false;
                            foreach (Group s in ((value as Group).NodeChildren[j] as Node).Groups)
                            {
                                if (s.NodeChildren.Contains(((value as Group).NodeChildren[j] as Node)) && ((value as Group).NodeChildren[j] as Node).Groups.Count > 0 && s.NodeChildren.Count > 1)
                                {
                                    ((value as Group).NodeChildren[j] as Node).IsGrouped = true;
                                }
                            }
                            if ((((value as Group).NodeChildren[j] as Node).AllowDelete) && !((value as Group).NodeChildren[j] is Group))
                            {
                                CheckEmptySource();
                                (value as Group).RemoveChild(((value as Group).NodeChildren[j] as Node));
                            }
                        }
                        else if ((value as Group).NodeChildren[j] is Node)
                        {
                            if (((value as Group).NodeChildren[j] as Node).IsGrouped == false)
                            {
                                ((value as Group).NodeChildren[j] as Node).IsGrouped = false;
                                foreach (Group s in ((value as Group).NodeChildren[j] as Node).Groups)
                                {
                                    if (s.NodeChildren.Contains(((value as Group).NodeChildren[j] as Node)) && ((value as Group).NodeChildren[j] as Node).Groups.Count > 0 && s.NodeChildren.Count > 1)
                                    {
                                        ((value as Group).NodeChildren[j] as Node).IsGrouped = true;
                                    }
                                }
                                if ((((value as Group).NodeChildren[j] as Node).AllowDelete) && !((value as Group).NodeChildren[j] is Group))
                                {
                                    CheckEmptySource();
                                    (value as Group).RemoveChild(((value as Group).NodeChildren[j] as Node));
                                }
                            }
                            (value as Group).IsSelected = false;
                        }
                        else
                        {
                            ((value as Group).NodeChildren[j] as LineConnector).IsGrouped = false;
                        }
                    }
                }
                CheckEmptySource();

                if (value is LineConnector)
                {
                    if ((value as LineConnector).AllowDelete)
                    {
                        ValidateIListSupport().Remove(value);
                    }
                }
                else
                {
                    ValidateIListSupport().Remove(value);
                }

                //ValidateIListSupport().Remove(value as Group);
                //(value as Group).IsSelected = false;
            }


            else
            {
                CheckEmptySource();

                if (value is LineConnector)
                {
                    if ((value as LineConnector).AllowDelete)
                    {
                        ValidateIListSupport().Remove(value);
                    }
                }
                else
                {
                    ValidateIListSupport().Remove(value);
                }

                //ValidateIListSupport().Remove(value);
            }

        }

        /// <summary>Removes the <see cref="T:System.Collections.IList"></see> item at the specified index.</summary>
        /// <param name="index">The zero-based index of the item to remove. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public void RemoveAt(int index)
        {
            object obj = this.GetItemAt(index);
            if (obj != null)
            {
                m_dataInternalDict.Remove(obj);
            }
            if (obj is Node && !(obj is Group))
            {
                if ((obj as Node).AllowDelete && !((obj as Node).IsGrouped))
                {
                    CheckEmptySource();
                    ValidateIListSupport().RemoveAt(index);
                }
            }
            else if (obj is Group)
            {
                if ((obj as Group).AllowDelete)
                {
                    for (int j = 0; j <= (obj as Group).NodeChildren.Count - 1; j++)
                    {
                        if (((obj as Group).NodeChildren[j] as Node).AllowDelete)
                        {
                            CheckEmptySource();
                            (obj as Group).RemoveChild(((obj as Group).NodeChildren[j] as Node));
                        }
                        ((obj as Group).NodeChildren[j] as Node).IsGrouped = false;
                    }
                    CheckEmptySource();
                    ValidateIListSupport().RemoveAt(this.IndexOf((obj as Group)));
                }
            }
            else
            {
                CheckEmptySource();

                if (obj is LineConnector)
                {
                    if ((obj as LineConnector).AllowDelete)
                    {
                        ValidateIListSupport().RemoveAt(index);
                    }
                }
                else
                {
                    ValidateIListSupport().RemoveAt(index);
                }

                //ValidateIListSupport().RemoveAt(index);
            }
        }

        /// <summary>Gets or sets the element at the specified index.</summary>
        /// <returns>The element at the specified index.</returns>
        /// <param name="index">The zero-based index of the element to get or set. </param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is not a valid index in the <see cref="T:System.Collections.IList"></see>. </exception>
        public object this[int index]
        {
            get
            {
                return GetItemAt(index);
            }

            set
            {
                CheckEmptySource();

                ValidateIListSupport()[index] = value;
            }
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException">array is null. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        public void CopyTo(Array array, int index)
        {
            foreach (UIElement element in this)
            {
                array.SetValue(element, index++);
            }
        }

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).</summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
        public object SyncRoot
        {
            get
            {
                return m_syncRoot;
            }
        }

        /// <summary>Creates a new view on the collection that implements this interface. Typically, user code does not call this method.</summary>
        /// <returns>The newly created view.</returns>
        public ICollectionView CreateView()
        {
            return null;
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.IList"></see> contains a specific value.</summary>
        /// <returns>true if the <see cref="T:System.Object"></see> is found in the <see cref="T:System.Collections.IList"></see>; otherwise, false.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public bool Contains(object value)
        {
            if (value != null)
            {
                return m_dataInternalDict.ContainsKey(value);
            }
            else
            {
                return false;
            }
            //return ValidateIListSupport().Contains(value);
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public int IndexOf(object value)
        {
            return ValidateIListSupport().IndexOf(value);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            if (m_source != null)
            {
                return m_source.GetEnumerator();
            }

            return m_dataInternal.GetEnumerator();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Subscribes to the CollectionChanged event.
        /// </summary>
        /// <param name="collection">Represents the IEnumerable collection.</param>
        private void HookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;

            if (changes != null)
            {
                changes.CollectionChanged += OnChangesCollectionChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from the CollectionChanged event.
        /// </summary>
        /// <param name="collection">Represents the IEnumerable collection.</param>
        private void UnhookCollection(IEnumerable collection)
        {
            INotifyCollectionChanged changes = collection as INotifyCollectionChanged;
            if (changes != null)
            {
                changes.CollectionChanged -= OnChangesCollectionChanged;
            }
        }

        /// <summary>
        /// Reraises notifications on collection change.
        /// </summary>
        /// <param name="sender">object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        /// <remarks>Notifies of the collection change.</remarks>
        private void OnChangesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Checks whether current source supports the IList interface.
        /// </summary>
        /// <returns>The collection of objects in <see cref="IList"/></returns>
        private IList ValidateIListSupport()
        {
            IList result = Data as IList;

            if (result == null)
            {
                throw new InvalidOperationException("Operation invalid, source collection should support IList.");
            }

            return result;
        }

        /// <summary>
        /// Get an item by index from the internal or external source through the IList or IEnumerable interfaces.
        /// </summary>
        /// <param name="index">The integer value representing the index</param>
        /// <returns>The item at the specified index.</returns>
        private object GetItemAt(int index)
        {
            IEnumerable data = Data;
            IList list = data as IList;

            if (list != null)
            {
                return list[index];
            }

            ICollection collection = data as ICollection;

            if (collection != null && collection.Count <= index)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            IEnumerator enumerator = data.GetEnumerator();

            if (enumerator == null)
            {
                throw new InvalidOperationException("Can not get an enumerator from the source collection.");
            }

            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }

            return enumerator.Current;
        }

        /// <summary>
        /// Checks whether the source is not set and raises an exception if the collection source is not null.
        /// </summary>
        private void CheckEmptySource()
        {
            if (m_dataInternal.Count > 0)
            {
                if (SourceCollection != null)
                {
                    throw new InvalidOperationException("SourceCollection is used.");
                }
            }
        }

        #endregion
    }
}
