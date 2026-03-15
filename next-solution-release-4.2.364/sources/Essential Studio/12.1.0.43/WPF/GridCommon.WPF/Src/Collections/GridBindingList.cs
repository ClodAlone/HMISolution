#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Collections.Generic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Delegate for ListChanging event
    /// </summary>
    public delegate void GridListChangingEventHandler(object sender, GridListChangingEventArgs args);

    /// <summary>
    /// Event arguments for ListChanging
    /// </summary>
    public class GridListChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Gets or sets the type of the list changed.
        /// </summary>
        /// <value>The type of the list changed.</value>
        public ListChangedType ListChangedType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new index.
        /// </summary>
        /// <value>The new index.</value>
        public int NewIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the old index.
        /// </summary>
        /// <value>The old index.</value>
        public int OldIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public object Item
        {
            get;
            internal set;
        }

    }

    /// <summary>
    /// Interface for IGridBindingList. 
    /// </summary>
    /// <remarks>
    /// It has some additional properties to define Changing events.
    /// </remarks>
    public interface IGridBindingList : IBindingList
    {
        /// <summary>
        /// Occurs when [list changing].
        /// </summary>
        event GridListChangingEventHandler ListChanging;
        /// <summary>
        /// Gets or sets a value indicating whether [raise list changing events].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [raise list changing events]; otherwise, <c>false</c>.
        /// </value>
        bool RaiseListChangingEvents
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Implementation of the IGridBindingList interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridBindingList<T> : BindingList<T>, IGridBindingList
    {

        /// <summary>
        /// Removes all elements from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            bool result = this.OnRaiseListChangingEventArgs(new GridListChangingEventArgs()
            {
                ListChangedType = ListChangedType.Reset,
                NewIndex = -1
            });
            if (!result)
                base.ClearItems();
        }

        /// <summary>
        /// Inserts the specified item in the list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index where the item is to be inserted.</param>
        /// <param name="item">The item to insert in the list.</param>
        protected override void InsertItem(int index, T item)
        {
            bool result = this.OnRaiseListChangingEventArgs(new GridListChangingEventArgs()
            {
                ListChangedType = ListChangedType.ItemAdded,
                NewIndex = index,
                Item = item
            });
            if (!result)
                base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// You are removing a newly added item and <see cref="P:System.ComponentModel.IBindingList.AllowRemove"/> is set to false.
        /// </exception>
        protected override void RemoveItem(int index)
        {
            bool result = this.OnRaiseListChangingEventArgs(new GridListChangingEventArgs()
            {
                NewIndex = index,
                OldIndex = -1,
                ListChangedType = ListChangedType.ItemDeleted,
                Item = this[index]
            });
            if (!result)
                base.RemoveItem(index);
        }

        /// <summary>
        /// Replaces the item at the specified index with the specified item.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new value for the item at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// 	<paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.
        /// </exception>
        protected override void SetItem(int index, T item)
        {
            bool result = this.OnRaiseListChangingEventArgs(new GridListChangingEventArgs()
            {
                NewIndex = -1,
                OldIndex = index,
                ListChangedType = ListChangedType.ItemChanged,
                Item = this[index]
            });
            if (!result)
                base.SetItem(index, item);
        }

        /// <summary>
        /// Raises the <see cref="E:RaiseListChangingEventArgs"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Collections.Generics.ListChangingEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private bool OnRaiseListChangingEventArgs(GridListChangingEventArgs args)
        {
            if (this.RaiseListChangingEvents)
            {
                if (ListChanging != null)
                    ListChanging(this, args);
            }
            return args.Cancel;
        }

        #region IGridBindingList Members

        /// <summary>
        /// Occurs when [list changing].
        /// </summary>
        public event GridListChangingEventHandler ListChanging;

        private bool _raiseListChangingEvents = true;
        /// <summary>
        /// Gets or sets a value indicating whether [raise list changing events].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [raise list changing events]; otherwise, <c>false</c>.
        /// </value>
        public bool RaiseListChangingEvents
        {
            get
            {
                return _raiseListChangingEvents;
            }
            set
            {
                _raiseListChangingEvents = value;
            }
        }

        #endregion
    }
}