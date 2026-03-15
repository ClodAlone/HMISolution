// <copyright file="NodeCollection.cs" company="Syncfusion">
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
using System.Windows.Controls;

namespace Syncfusion.Windows.Diagram
{
    /// <summary>
    /// Collection, used to store <see cref="Node"/> objects.
    /// </summary>
    public class NodeCollection : IList, IEnumerable
    {

        internal event NotifyCollectionChangedEventHandler CollectionChanged;

        //private NotifyCollectionChangedEventHandler handler;

        #region class variables

        /// <summary>
        /// Typed members detect collection change is in progress.
        /// </summary>
        internal bool m_collClear;

        /// <summary>
        /// Typed members storage.
        /// </summary>
        private ArrayList m_members;

        /// <summary>
        /// Represents the <see cref="DiagramPage"/>. 
        /// </summary>
        private Panel mdiagramPage;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class .
        /// </summary>
        public NodeCollection()
        {
            //handler = CollectionChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class .
        /// </summary>
        /// <param name="mdiagramPage">Panel instance</param>
        public NodeCollection(Panel mdiagramPage)
        {
            this.mdiagramPage = mdiagramPage;
        }
        #endregion

        #region IList Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public int Add(object value)
        {
            return AddValue(value);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        private int AddValue(object value)
        {
            int nIndexToReturn = -1;
            if (value is INodeGroup)
            {
                List<INodeGroup> groupNodes = GetGroupList(value as INodeGroup);

                foreach (ICommon groupNode in groupNodes)
                {
                    groupNode.IsSelected = true;
                    if (!this.Members.Contains(groupNode))
                    {
                        nIndexToReturn = this.Members.Add(groupNode);
                    }
                    else
                    {
                        nIndexToReturn = this.Members.IndexOf(groupNode);
                    }
                }
            }
            else
            {
                (value as ICommon).IsSelected = true;
                if (!this.Members.Contains(value))
                {
                    nIndexToReturn = this.Members.Add(value);
                }
                else
                {
                    nIndexToReturn = this.Members.IndexOf(value);
                }
            }
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
            return nIndexToReturn;
        }

        /// <summary>
        /// Selects an item from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        public void Select(object value)
        {
            this.Clear();
            this.Add(value);
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {
           
            List<object> listcollection = new List<object>();
            List<object> listgroup = new List<object>();
            foreach (object obj in this.Members)
            {
                if (obj is Group)
                {
                    listgroup.Add(obj);
                    continue;
                }
                else
                {
                    listcollection.Add(obj);
                }


            }
            foreach (object n in listcollection)
            {
                (n as ICommon).IsSelected = false;
            }
            foreach (object g in listgroup)
            {
                if (!(g as Group).AlwaysSelected)
                {
                    (g as Group).IsSelected = false;
                }

            }
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        
        /// <summary>
        /// Gets the members of the list.
        /// </summary>
        protected ArrayList Members
        {
            get
            {
                if (m_members == null)
                {
                    m_members = new ArrayList();
                }

                return m_members;
            }
        }

        /// <summary>
        /// Specifies whether the List contains the specified value.
        /// </summary>
        /// <param name="value">The value to be searched in the list.</param>
        /// <returns>True, if it contains value, false otherwise.</returns>
        public bool Contains(object value)
        {
            return this.Members.Contains(value);
        }

        /// <summary>
        /// Selects all the members of the list.
        /// </summary>
        public void SelectAll()
        {
            this.Members.Clear();
            this.Members.AddRange(mdiagramPage.Children.OfType<ICommon>() as ICollection);
            foreach (object obj in this.Members)
            {
                (obj as ICommon).IsSelected = true;
            }
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>Determines the index of a specific item in the <see cref="T:System.Collections.IList"></see>.</summary>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to locate in the <see cref="T:System.Collections.IList"></see>. </param>
        public int IndexOf(object value)
        {
            return this.Members.IndexOf(value);
        }

        /// <summary>
        /// Inserts an item at the specified index value.
        /// </summary>
        /// <param name="index">The location.</param>
        /// <param name="value">Item to be inserted.</param>
        public void Insert(int index, object value)
        {
            this.Members.Insert(index, value);
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="icol">The <see cref="ICollection"/> object</param>
        public void AddRange(ICollection icol)
        {
            this.Members.AddRange(icol);
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the List values are of fixed size.
        /// </summary>
        /// <returns>True, if it is of fixed size, false otherwise.</returns>
        public bool IsFixedSize
        {
            get
            {
                return this.Members.IsFixedSize;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the List values can be only read or written to.
        /// </summary>
        /// <returns>True, if it is read only, false otherwise.</returns>
        public bool IsReadOnly
        {
            get
            {
                return this.Members.IsReadOnly;
            }
        }

        /// <summary>
        /// Removes the specified value from the list.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        public void Remove(object value)
        {
            m_collClear = true;
            if (value is INodeGroup)
            {
                List<INodeGroup> groupNodes = GetGroupList(value as INodeGroup);
                foreach (ICommon groupNode in groupNodes)
                {
                    this.Members.Remove(groupNode);  
                    groupNode.IsSelected = false;
                     
                }
            }
            else
            {
                 this.Members.Remove(value);
                (value as ICommon).IsSelected = false;
                
            }

            this.Members.Remove(value);
            m_collClear = false;
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Removes an item at the specified index value.
        /// </summary>
        /// <param name="index">The location.</param>
        public void RemoveAt(int index)
        {
            this.Members.RemoveAt(index);
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                CollectionChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the node object at the specified location.
        /// </summary>
        /// <param name="index">The location</param>
        /// <returns>The member at the specified location.</returns>
        public object this[int index]
        {
            get
            {
                return this.Members[index];
            }

            set
            {
                this.Members[index] = value;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the group list.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>Gets the <see cref="T:System.Collections.IEnumerable"/> collection.</returns>
        internal List<INodeGroup> GetGroupList(INodeGroup node)
        {
            IEnumerable<INodeGroup> list = mdiagramPage.Children.OfType<INodeGroup>();
            INodeGroup rootItem = GetRootNodeFromGroup(list, node);
            return GetGroupList(list, rootItem);
        }

        /// <summary>
        /// Gets the root node from group.
        /// </summary>
        /// <param name="list">The collection of nodes in the list.</param>
        /// <param name="node">The <see cref="Node"/> object.</param>
        /// <returns>The root <see cref="Node"/> object. </returns>
        private INodeGroup GetRootNodeFromGroup(IEnumerable<INodeGroup> list, INodeGroup node)
        {
            if (node == null || node.ParentID == Guid.Empty)
            {
                return node;
            }
            else
            {
                foreach (INodeGroup item in list)
                {
                    if (item.ID == node.ParentID)
                    {
                        return GetRootNodeFromGroup(list, item);
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the group list.
        /// </summary>
        /// <param name="list">The collection of nodes.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>The node collection.</returns>
        private List<INodeGroup> GetGroupList(IEnumerable<INodeGroup> list, INodeGroup parent)
        {
            List<INodeGroup> collection = new List<INodeGroup>();
            collection.Add(parent);

            var children = list.Where(node => node.ParentID == parent.ID);

            foreach (INodeGroup child in children)
            {
                collection.AddRange(GetGroupList(list, child));
            }

            return collection;
        }

        #endregion

        #region ICollection Members

        /// <summary>Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing. </param>
        /// <param name="index">The zero-based index in array at which copying begins. </param>
        /// <exception cref="T:System.ArgumentNullException">array is null. </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
        public void CopyTo(Array array, int index)
        {
            this.Members.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the count of the members in the collection.
        /// </summary>
        public int Count
        {
            get { return this.Members.Count; }
        }

        /// <summary>Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).</summary>
        /// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
        public bool IsSynchronized
        {
            get { return this.Members.IsSynchronized; }
        }

        /// <summary>Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</summary>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
        public object SyncRoot
        {
            get
            {
                return this.Members.SyncRoot;
            }
        }
        #endregion

        #region IEnumerable Members

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        private IEnumerator GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }

        #endregion
    }
}
