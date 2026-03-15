#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Controls.Grid
{
    #region Record selection interfaces
    /// <exclude/>
    /// <summary>
    /// Adds record selection support.
    /// </summary>
    /// <typeparam name="T">The record type.</typeparam>
    public interface ISupportsRecordSelection<T> where T : ISelectable, IComparable
    {
        /// <summary>
        /// Gets the selected record collection.
        /// </summary>
        GridSelectedObjectsBase<T> SelectedNodes
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether record selection is enabled.
        /// </summary>
        bool EnableNodeSelection
        {
            get;
            set;
        }

        /// <summary>
        /// The underlying GridControlBase that holds these records.
        /// </summary>
        GridControlBase InternalGrid
        {
            get;
        }

        /// <summary>
        /// Return s the record at a given grid row index.
        /// </summary>
        /// <param name="gridRowIndex">The grid row index.</param>
        /// <returns>The record object.</returns>
        T GetNodeAtRowIndex(int gridRowIndex);

        int GetRowIndexFromItem(object item);
    }

    /// <exclude/>
    /// <summary>
    /// Adds an IsSelected property.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Gets or internally sets whether this record is selected.
        /// </summary>
        bool IsSelected
        {
            get;
            set;
        }
    }
    #endregion

    #region SelectedTreeNodes support
 
    /// <summary>
    /// A collection object that holds selected nodes in a GridTreeControl.
    /// </summary>
    public class GridSelectedTreeNodes : GridSelectedObjectsBase<GridTreeNode>
    {
        public GridSelectedTreeNodes(GridTreeControlImpl owner)
            : base(owner)
        {

        }

        /// <summary>
        /// A cancelable event that is raised just prior to the object being selected or unselected in the grid.
        /// </summary>
        public event GridTreeNodeCancelEventHandler SelectedNodeChanging;

        /// <summary>
        /// Raises the SelectedNodeChanging event.
        /// </summary>
        /// <param name="node">The object being selected or unselected.</param>
        /// <param name="action">The action being taken.</param>
        /// <returns>True if the action should be done to the object, false otherwise.</returns>
        protected override bool OnSelectedObjectChanging(GridTreeNode node, NotifyCollectionChangedAction action) 
        {
            if (SelectedNodeChanging != null)
            {
                GridTreeNodeActions action1 = action == NotifyCollectionChangedAction.Add ? GridTreeNodeActions.Selecting
                                                                                            : GridTreeNodeActions.Unselecting; 
                GridTreeNodeCancelEventArgs e = new GridTreeNodeCancelEventArgs(node, action1);
                SelectedNodeChanging(this, e);
                return !e.Cancel;
            }
            return true;
        }

        /// <summary>
        /// A cancelable event raised prior to the collection being cleared.
        /// </summary>
#if !SILVERLIGHT
        public event CancelEventHandler SelectedNodesClearing;
#else
        public event EventHandler<CancelEventArgs> SelectedNodesClearing;
#endif

        /// <summary>
        /// Raised the SelectedNodesClearing event.
        /// </summary>
        /// <returns>True if the collection should be cleared, false otherwise.</returns>
        protected bool OnSelectedNodesClearing()
        {
            if (SelectedNodesClearing != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                SelectedNodesClearing(this, e);
                return !e.Cancel;
            }
            return true;
        }
    }

    #region event support classes for GridTreeNodes

    /// <summary>
    /// An event handler for cancelable GridSelectedTreeNode events.
    /// </summary>
    /// <param name="sender">The object that raised this event.</param>
    /// <param name="e">The cancelable event arguments.</param>
    public delegate void GridTreeNodeCancelEventHandler(object sender, GridTreeNodeCancelEventArgs e);

    /// <summary>
    /// An event handler for GridSelectedTreeNode events.
    /// </summary>
    /// <param name="sender">The object that raised this event.</param>
    /// <param name="e">The cancelable event arguments.</param>
    public delegate void GridTreeNodeEventHandler(object sender, GridTreeNodeEventArgs e);



    /// <summary>
    /// Base class for GridSelectedNode events.
    /// </summary>
    public class GridTreeNodeEventArgs
    {
        GridTreeNode node;
        GridTreeNodeActions action;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridSelectedTreeNodeEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public GridTreeNodeEventArgs(GridTreeNode node, GridTreeNodeActions action)
        {
            this.node = node;
            this.action = action;
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public GridTreeNode Node
        {
            get { return node; }
            set { node = value; }
        }

        /// <summary>
        /// Gets the action that triggerred this event.
        /// </summary>
        public GridTreeNodeActions Action
        {
            get { return action; }
        }
    }

    /// <summary>
    /// Event aguments for cancelable events involving a GridSelectedObject.
    /// </summary>
    public class GridTreeNodeCancelEventArgs : GridTreeNodeEventArgs
    {
        private bool cancel = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridSelectedObjectEventArgs"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        public GridTreeNodeCancelEventArgs(GridTreeNode node, GridTreeNodeActions action)
            : base(node, action)
        {

        }

        /// <summary>
        /// Gets or sets whether the action should be done.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }

    }

    /// <summary>
    /// Enumerates actions that can raise an event that uses the <see cref="GridSelectedObjectActions"/> arguments.
    /// </summary>
    public enum GridTreeNodeActions
    {
        /// <summary>
        /// An object is about to be expanded.
        /// </summary>
        Expanding,
        /// <summary>
        /// An object has been expanded.
        /// </summary>
        Expanded,
        /// <summary>
        /// An object is about to be collapsed.
        /// </summary>
        Collapsing,
        /// <summary>
        /// An object has been collapsed.
        /// </summary>
        Collapsed,
        /// <summary>
        /// An object is about to be added to the collection.
        /// </summary>
        Selecting,
        /// <summary>
        /// An object is about to be removed from the SelectedNodes collection.
        /// </summary>
        Unselecting,
    }


    #endregion
   
    
    
    #endregion

    #region BaseClass for selectible object collections (like GridTreeNode collections)

    /// <summary>
    /// A collection object that holds ISelectable, IComparable objects for a ISupportsRecordSelection object like GridTreeControl.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the collection.</typeparam>
    /// <remarks>The collection only supports INotifyCollectionChanged.Add, INotifyCollectionChanged.Remove
    /// and INotifyCollectionChanged.Reset actions. Internally, the enumerated objects are maintained
    /// as a sorted list to facilitate lookups to determine whether an object is contained in the list. 
    /// The Contains method does a binary search to quickly determine if an object is in this list.</remarks>
    public class GridSelectedObjectsBase<T> : IEnumerable<T>, ICloneable, INotifyCollectionChanged where T : ISelectable, IComparable
    {
        internal HashSet<T> list;
        internal ISupportsRecordSelection<T> owner;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="owner">The ISupportsRecordSelection<T> object that owns this collection.</param>
        public GridSelectedObjectsBase(ISupportsRecordSelection<T> owner)
        {
            list = new HashSet<T>();
            this.owner = owner;
        }

        /// <summary>
        /// Addes a range of objects to the collection.
        /// </summary>
        /// <param name="range">The objects to be added.</param>
        /// <remarks>
        /// This method properly sets the ISelectable.IsSelected value on the object.
        /// </remarks>
        internal void AddRange(IEnumerable<T> range)
        {
            AddRange(range, default(T));
        }

        internal void AddRange(IEnumerable<T> range, T activeRange)
        {
           bool selectedNodeChanging = OnSelectedObjectChanging(activeRange, NotifyCollectionChangedAction.Add);
            if (!selectedNodeChanging)
            {
                foreach (T n in range)
                {
                    n.IsSelected = false;
                }
            }
            else
            {
                foreach (T n in range)
                {
                     
                    if (!list.Contains(n))
                    {
                        list.Add(n);
                        n.IsSelected = true;
                    }
                }
#if SILVERLIGHT
                OnCollectionChanged(NotifyCollectionChangedAction.Add, range, -1); 
#else
                OnCollectionChanged(NotifyCollectionChangedAction.Add, new HashSet<T>(range));
#endif
            }
        }
     
        /// <summary>
        /// Removes a range of objects from the collection.
        /// </summary>
        /// <param name="range">The objects to be removed.</param>
        /// <remarks>
        /// This method properly sets the ISelectable.IsSelected value on the object.
        /// </remarks>
        internal void RemoveRange(IEnumerable<T> range)
        {
             foreach (T n in range)
            {
                if (list.Contains(n))
                {
                    list.Remove(n);
                    n.IsSelected = false;
                }
            }

#if SILVERLIGHT
             this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, range, -1);
#else
            this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, new HashSet<T>(range));
#endif

            RefreshGrid();
        }

        private bool lockGridRefresh = false;

        /// <summary>
        /// Gets or sets whether GridControl.InvalidateCells() is automatically called when the collection is modified.
        /// </summary>
        /// <remarks>
        /// If you set this value to true and then do actions that change the contents of teh collection, then parent
        /// GridControlBase display is not automatically refreshed. It would be your responsibilty to call GridControl.InvalidateCells()
        /// to ensure the display properly reflects the content of the selected objects collection.
        /// </remarks>
        public bool LockGridRefresh
        {
            get { return lockGridRefresh; }
            set { lockGridRefresh = value; }
        }

        /// <summary>
        /// Adds an object to the collection.
        /// </summary>
        /// <param name="n">The object to be added.</param>
        /// <remarks>
        /// This method properly sets the ISelectable.IsSelected value on the object.
        /// </remarks>
        public void Add(T n)
        {
            if (n != null)
            {

                if (!list.Contains(n))
                {
                    if (OnSelectedObjectChanging(n, NotifyCollectionChangedAction.Add))
                    {
                        list.Add(n);
                        //list.Insert(-loc - 1, n);
                        n.IsSelected = true;
#if SILVERLIGHT
                    OnCollectionChanged(NotifyCollectionChangedAction.Add,n, list.ToList().IndexOf(n));
#else
                        OnCollectionChanged(NotifyCollectionChangedAction.Add, n);
#endif
                    }
                }
            }
        }

        private void RefreshGrid()
        {
            if (!lockGridRefresh)
            {
                owner.InternalGrid.InvalidateCells();
            }
        }


        /// <summary>
        /// Removes an object from the collections.
        /// </summary>
        /// <param name="n">The object to be removed.</param>
        /// <remarks>
        /// This method properly sets the ISelectable.IsSelected value on the object.
        /// </remarks>
        public void Remove(T n)
        {
            if (list.Contains(n))
            {
                if (OnSelectedObjectChanging(n, NotifyCollectionChangedAction.Remove))
                {
                    n.IsSelected = false;
                    list.Remove(n);
                    RefreshGrid();
#if SILVERLIGHT
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, n, list.ToList().IndexOf(n));
#else
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, n);
#endif
                }
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        /// <remarks>
        /// This method properly sets the ISelectable.IsSelected value on the object.
        /// </remarks>
        public void Clear()
        {
            if (list.Count <= 0)
                return;
            var Nodes = new HashSet<T>(list);
            if (OnSelectedObjectsClearing())
            {
                foreach (T n in list)
                    n.IsSelected = false;

                list.Clear();
                RefreshNodes(Nodes);
                OnCollectionChanged(NotifyCollectionChangedAction.Reset);
            }
        }

        /// <summary>
        /// Code to refresh the particular GridTreeNodes
        /// </summary>
        /// <param name="Nodes">The nodes.</param>
        internal void RefreshNodes(HashSet<T> Nodes)
        {
            foreach (GridTreeNode node in (IEnumerable)Nodes)
                owner.InternalGrid.InvalidateCell(GridRangeInfo.Row(owner.GetRowIndexFromItem(node.Item)));
        }
        /// <summary>
        /// Returns whether or not the passed in object is in this collection.
        /// </summary>
        /// <param name="n">The object to check.</param>
        /// <returns>True if the object is in this collection, false otherwise.</returns>
        /// <remarks> This method does not check the value of the ISelectable.IsSelected
        /// property on the passed in object. It only considers whether or not the object 
        /// is in the collection.</remarks>
        public bool Contains(T n)
        {
            return list.Contains(n); //list.BinarySearch(n) >= 0;
        }

        /// <summary>
        /// Sets the ISelectable.IsSelected value on the object to isSelected, and also
        /// calls the Add or Remove method to properly include or exclude the object in this collection
        /// </summary>
        /// <param name="n">The object.</param>
        /// <param name="isSelected">Whether the object is being selected or unselected.</param>
        public void SetSelected(T n, bool isSelected)
        {
            if (n == null)
                return;

            if (isSelected && !n.IsSelected)
            {
                 Add(n);
            }
            else if (!isSelected && n.IsSelected)
            {
                 Remove(n);
            }
            else if (n.IsSelected && !list.Contains(n))
            {
                 Add(n);
            }
            else if (!n.IsSelected && list.Contains(n) )
            {
                 Remove(n);
            }
        }

        /// <summary>
        /// Gets the number of objects in this collection.
        /// </summary>
        public int Count
        {
            get
            {
                if (list != null)
                    return list.Count;

                return 0;
            }
        }

        /// <summary>
        /// Returns the object at the given collection position.
        /// </summary>
        /// <param name="i">The position.</param>
        /// <returns>The object.</returns>
        public T this[int i]
        {
            get
            {
                if (i < 0 || i >= list.Count)
                    throw new ArgumentOutOfRangeException("index", "Index out of range.");
                return list.ToList()[i];
            }
        }

        #region IEnumerable<T> Members
        /// <exclude/>
        /// <summary>
        /// Enumerator for this collection.
        /// </summary>
        /// <returns>The return type of the objects of enumeration.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// The Enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Returns a clone of this collection.
        /// </summary>
        /// <returns>A new collection holding the same objects.</returns>
        public object Clone()
        {
            GridSelectedObjectsBase<T> col = new GridSelectedObjectsBase<T>(this.owner);
            col.AddRange(this.list);
            return col;
        }

        #endregion

        #region events
        
        /// <summary>
        /// This method is called just prior to an object being added to or removed from the collection.
        /// </summary>
        /// <param name="item">The object being added or removed.</param>
        /// <param name="action">The action being taken, must be Add, Remove, or Reset.</param>
        /// <returns>True if the action should be done to the object, false otherwise.</returns>
        /// <remarks>Override this clase to raise events which can cancel the associated action.</remarks>
        protected virtual bool OnSelectedObjectChanging(T item, NotifyCollectionChangedAction action) 
        {
            if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove
                && action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentOutOfRangeException("action", "action must be Add, Remove or Reset.");
            }
            return true;
        }

        /// <summary>
        /// This method is called just prior to the collection being cleared.
        /// </summary>
        /// <returns>True if the collection should be cleared, false otherwise.</returns>
        protected virtual bool OnSelectedObjectsClearing()
        {
            return true;
        }

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Event raised after an object has been added or removed from the collection, or after the collection
        /// has been reset.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

#if !SILVERLIGHT
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            if (CollectionChanged != null)
            {

                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, item);
                CollectionChanged(this, e);
            }
        }
         protected void OnCollectionChanged(NotifyCollectionChangedAction action, HashSet<T> item)
        {
            if (CollectionChanged != null)
            {

                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, item.ToList());
                CollectionChanged(this, e);
            }
        }
      
#else
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, item, index);
                CollectionChanged(this, e);
            }
        }
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, IEnumerable<T> item, int index)
        {
            if (CollectionChanged != null)
            {

                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, item,index);
                CollectionChanged(this, e);
            }
        }
#endif

        protected void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action);
                CollectionChanged(this, e);
            }
        }


        #endregion

        #endregion
    }

    #endregion

    #region Sort Interface
    /// <exclude />
    public interface ISupportsSortStates
    {
        /// <exclude />
        List<SortState> SortStates
        {
            get;
        }
        /// <exclude />
        string PropertyNameFromColumnIndex(int columnIndex);
        /// <exclude />
        int FindPropertyNameInStates(string name);
    }
    #endregion

}
