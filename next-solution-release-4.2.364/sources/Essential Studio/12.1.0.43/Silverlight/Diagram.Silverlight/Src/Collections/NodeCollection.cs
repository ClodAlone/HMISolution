// <copyright file="NodeCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Diagram
{
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
    using System.Windows.Media;

    /// <summary>
    /// Collection, used to store <see cref="Node"/> objects.
    /// </summary>
    public class NodeCollection : IList, IEnumerable
    
    {        
        /// <summary>
        /// Typed members storage.
        /// </summary>
        private IList mmembers;

        /// <summary>
        /// Represents the <see cref="DiagramPage"/>. 
        /// </summary>
        private Panel mdiagramPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class .
        /// </summary>
        /// <param name="mdiagramPage">Panel instance</param>
        public NodeCollection(Panel mdiagramPage)
        {
            this.mdiagramPage = mdiagramPage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCollection"/> class .
        /// </summary>
        public NodeCollection()
        {
        }

        /// <summary>
        /// Gets the count of the members in the collection.
        /// </summary>
        public int Count
        {
            get { return this.Members.Count; }
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

        /// <summary>
        /// Gets the members of the list.
        /// </summary>
        protected IList Members
        {
            get
            {
                if (this.mmembers == null)
                {
                    this.mmembers = new List<object>();
                }

                return this.mmembers;
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

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        public int Add(object value)
        {
            return this.AddValue(value);
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="icol">The <see cref="ICollection"/> object</param>
        public void AddRange(ICollection icol)
        {
        }       

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only. </exception>
        public void Clear()
        {          

            for (int i = 0; i < this.Members.Count;i++ )
            {
                if (this.Members[i] != null)
                {
                    DiagramView view = GetDiagramView(this.Members[i] as DependencyObject);
                    if (view != null)
                    {
                        view.selectionListCleared = true;
                    }
                }
                (this.Members[i] as ICommon).IsSelected = false;
                
                i--;               
            }
           
        }

        internal static DiagramView GetDiagramView(DependencyObject element)
        {
            while (element != null && !(element is DiagramView))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as DiagramView;
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
        }

        /// <summary>
        /// Removes the specified value from the list.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        public void Remove(object value)
        {            
            if (value is INodeGroup)
            {
                List<INodeGroup> groupNodes = this.GetGroupList(value as INodeGroup);
                foreach (ICommon groupNode in groupNodes)
                {
                    groupNode.IsSelected = false;
                    this.Members.Remove(groupNode);
                }
            }
            else
            {
                (value as ICommon).IsSelected = false;
                this.Members.Remove(value);
            }

            this.Members.Remove(value);
        }

        /// <summary>
        /// Removes an item at the specified index value.
        /// </summary>
        /// <param name="index">The location.</param>
        public void RemoveAt(int index)
        {
            this.Members.RemoveAt(index);
        }

        /// <summary>
        /// Selects an item from the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        public void Select(object value)
        {
            this.Clear();
            this.Add(value);
        }

        /// <summary>
        /// Selects all the members of the list.
        /// </summary>
        public void SelectAll()
        {
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }  

        /// <summary>
        /// Gets the group list.
        /// </summary>
        /// <param name="node">The node object.</param>
        /// <returns>Gets the <see cref="T:System.Collections.IEnumerable"/> collection.</returns>
        internal List<INodeGroup> GetGroupList(INodeGroup node)
        {
            IEnumerable<INodeGroup> list = this.mdiagramPage.Children.OfType<INodeGroup>();
            INodeGroup rootItem = this.GetRootNodeFromGroup(list, node);
            return this.GetGroupList(list, rootItem);
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        private IEnumerator GetEnumerator()
        {
            return this.Members.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.IList"></see>.
        /// </summary>
        /// <returns>The position into which the new element was inserted.</returns>
        /// <param name="value">The <see cref="T:System.Object"></see> to add to the <see cref="T:System.Collections.IList"></see>. </param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList"></see> is read-only.-or- The <see cref="T:System.Collections.IList"></see> has a fixed size. </exception>
        private int AddValue(object value)
        {
            int indexToReturn = -1;
            if (value is INodeGroup)
            {
                List<INodeGroup> groupNodes = this.GetGroupList(value as INodeGroup);

                foreach (ICommon groupNode in groupNodes)
                {
                    groupNode.IsSelected = true;
                    if (!this.Members.Contains(groupNode))
                    {
                        indexToReturn = this.Members.Add(groupNode);
                    }
                    else
                    {
                        indexToReturn = this.Members.IndexOf(groupNode);
                    }
                }
            }
            else
            {
                (value as ICommon).IsSelected = true;
                if (!this.Members.Contains((value as ICommon)))
                {
                    indexToReturn = this.Members.Add(value);
                }
                else
                {
                    indexToReturn = this.Members.IndexOf(value);
                }
            }

            return indexToReturn;
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
                collection.AddRange(this.GetGroupList(list, child));
            }

            return collection;
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
                        return this.GetRootNodeFromGroup(list, item);
                    }
                }

                return null;
            }
        }             

        #region class variables

        #endregion

        #region Initialization

        #endregion

        #region IList Members

        #endregion

        #region Public Members

        #endregion

        #region ICollection Members

        #endregion

        #region IEnumerable Members

        #endregion
    }
}
