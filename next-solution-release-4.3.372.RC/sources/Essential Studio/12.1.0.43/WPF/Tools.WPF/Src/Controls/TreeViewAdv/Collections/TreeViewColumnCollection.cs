// <copyright file="TreeViewColumnCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for TreeView Column Collection.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TreeViewColumnCollection : ObservableCollection<TreeViewColumn>
    {
        #region Members

        /// <summary>
        /// Presents actual indices
        /// </summary>
        private List<int> m_actualIndices = new List<int>();

        /// <summary>
        /// Presents columns
        /// </summary>
        private List<TreeViewColumn> m_columns = new List<TreeViewColumn>();

        /// <summary>
        /// Presents internal event arg
        /// </summary>
        private TreeViewColumnCollectionChangedEventArgs m_internalEventArg;

        /// <summary>
        /// Presents view mode
        /// </summary>
        private bool m_inViewMode;

        /// <summary>
        /// Presents isImmutable
        /// </summary>
        private bool m_isImmutable;

        /// <summary>
        /// Presents owner
        /// </summary>
        private DependencyObject m_owner;

        internal List<double> indexs = new List<double>();

        internal double totalWidth;

        #endregion Members

        #region Events

        /// <summary>
        /// Occurs when [m_internal collection changed].
        /// </summary>
        private event NotifyCollectionChangedEventHandler M_internalCollectionChanged;

        /// <summary>
        /// Occurs when [internal collection changed].
        /// </summary>
        internal event NotifyCollectionChangedEventHandler InternalCollectionChanged
        {
            add
            {
                M_internalCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(M_internalCollectionChanged, value);
            }

            remove
            {
                M_internalCollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(M_internalCollectionChanged, value);
            }
        }

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the column collection.
        /// </summary>
        /// <value>The column collection.</value>
        internal List<TreeViewColumn> ColumnCollection
        {
            get
            {
                return m_columns;
            }
        }

        /// <summary>
        /// Gets the index list.
        /// </summary>
        /// <value>The index list.</value>
        internal List<int> IndexList
        {
            get
            {
                return m_actualIndices;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [in view mode].
        /// </summary>
        /// <value><c>true</c> if [in view mode]; otherwise, <c>false</c>.</value>
        internal bool InViewMode
        {
            get
            {
                return m_inViewMode;
            }

            set
            {
                m_inViewMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        internal DependencyObject Owner
        {
            get
            {
                return m_owner;
            }

            set
            {
                if (value != m_owner)
                {
                    m_owner = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is immutable.
        /// </summary>
        /// <value>
        /// true if this instance is immutable; otherwise, false.
        /// </value>
        private bool IsImmutable
        {
            get
            {
                return m_isImmutable;
            }

            set
            {
                m_isImmutable = value;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Blocks the write.
        /// </summary>
        internal void BlockWrite()
        {
            IsImmutable = true;
        }

        /// <summary>
        /// Unblocks the write.
        /// </summary>
        internal void UnblockWrite()
        {
            IsImmutable = false;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            VerifyAccess();
            m_internalEventArg = ClearPreprocess();
            base.ClearItems();
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="column">The column.</param>
        protected override void InsertItem(int index, TreeViewColumn column)
        {
            VerifyAccess();
            m_internalEventArg = InsertPreprocess(index, column);
            if (column.Width.IsStar)
                indexs.Add(column.Width.Value);
            else if (column.Width.IsAuto)
            {
                totalWidth += column.MinWidth;
            }
            else
            {
                totalWidth += column.Width.Value;
            }
            base.InsertItem(index, column);
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex != newIndex)
            {
                VerifyAccess();
                m_internalEventArg = MovePreprocess(oldIndex, newIndex);
                base.MoveItem(oldIndex, newIndex);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnInternalCollectionChanged();
            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            VerifyAccess();
            m_internalEventArg = RemoveAtPreprocess(index);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Sets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="column">The column.</param>
        protected override void SetItem(int index, TreeViewColumn column)
        {
            VerifyAccess();
            m_internalEventArg = SetPreprocess(index, column);

            if (m_internalEventArg != null)
            {
                base.SetItem(index, column);
            }
        }

        /// <summary>
        /// Clears the preprocess.
        /// </summary>
        /// <returns>TreeViewColumn CollectionChangedEventArgs</returns>
        private TreeViewColumnCollectionChangedEventArgs ClearPreprocess()
        {
            TreeViewColumn[] clearedColumns = new TreeViewColumn[base.Count];

            if (base.Count > 0)
            {
                base.CopyTo(clearedColumns, 0);
            }

            foreach (TreeViewColumn column in m_columns)
            {
                column.ResetPrivateData();
                column.PropertyChanged -= new PropertyChangedEventHandler(ColumnPropertyChanged);
            }

            m_columns.Clear();
            m_actualIndices.Clear();
            return new TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, clearedColumns);
        }

        /// <summary>
        /// Columns the property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void ColumnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TreeViewColumn column = sender as TreeViewColumn;

            if ((M_internalCollectionChanged != null) && (column != null))
            {
                M_internalCollectionChanged(this, new TreeViewColumnCollectionChangedEventArgs(column, e.PropertyName));
            }
        }

        /// <summary>
        /// Inserts the preprocess.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="column">The column.</param>
        /// <returns>TreeViewColumn CollectionChangedEventArgs</returns>
        private TreeViewColumnCollectionChangedEventArgs InsertPreprocess(int index, TreeViewColumn column)
        {
            int actualIndex = m_columns.Count;

            if ((index < 0) || (index > actualIndex))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            ValidateColumnForInsert(column);
            m_columns.Add(column);
            column.ActualIndex = actualIndex;
            m_actualIndices.Insert(index, actualIndex);
            column.PropertyChanged += new PropertyChangedEventHandler(ColumnPropertyChanged);
            return new TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, column, index, actualIndex);
        }

        /// <summary>
        /// Moves the preprocess.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        /// <returns>TreeViewColumn CollectionChangedEventArgs</returns>
        private TreeViewColumnCollectionChangedEventArgs MovePreprocess(int oldIndex, int newIndex)
        {
            VerifyIndexInRange(oldIndex, "oldIndex");
            VerifyIndexInRange(newIndex, "newIndex");
            int actualIndex = m_actualIndices[oldIndex];

            if (oldIndex < newIndex)
            {
                for (int i = oldIndex; i < newIndex; i++)
                {
                    m_actualIndices.Insert(i, m_actualIndices[i + 1]);
                }
            }
            else
            {
                for (int j = oldIndex; j > newIndex; j--)
                {
                    m_actualIndices.Insert(j, m_actualIndices[j - 1]);
                }
            }

            m_actualIndices.Insert(newIndex, actualIndex);
            return new TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, m_columns[actualIndex], newIndex, oldIndex, actualIndex);
        }

        /// <summary>
        /// Called when [internal collection changed].
        /// </summary>
        private void OnInternalCollectionChanged()
        {
            if ((M_internalCollectionChanged != null) && (m_internalEventArg != null))
            {
                M_internalCollectionChanged(this, m_internalEventArg);
                m_internalEventArg = null;
            }
        }

        /// <summary>
        /// Removes at preprocess.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>TreeViewColumn CollectionChangedEventArgs</returns>
        private TreeViewColumnCollectionChangedEventArgs RemoveAtPreprocess(int index)
        {
            VerifyIndexInRange(index, "index");
            int actualIndex = m_actualIndices[index];
            if (actualIndex >= m_columns.Count)
                actualIndex = m_columns.Count - 1;
            TreeViewColumn oldValue = m_columns[actualIndex];
            oldValue.ResetPrivateData();
            oldValue.PropertyChanged -= new PropertyChangedEventHandler(ColumnPropertyChanged);
            m_columns.RemoveAt(actualIndex);
            UpdateIndexList(actualIndex, index);
            UpdateActualIndexInColumn(actualIndex);
            return new TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldValue, index, actualIndex);
        }

        /// <summary>
        /// Sets the preprocess.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>TreeViewColumn CollectionChangedEventArgs</returns>
        private TreeViewColumnCollectionChangedEventArgs SetPreprocess(int index, TreeViewColumn newColumn)
        {
            VerifyIndexInRange(index, "index");
            TreeViewColumn oldItem = base[index];

            if (oldItem != newColumn)
            {
                int actualIndex = m_actualIndices[index];
                RemoveAtPreprocess(index);
                InsertPreprocess(index, newColumn);
                return new TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newColumn, oldItem, index, actualIndex);
            }

            return null;
        }

        /// <summary>
        /// Updates the actual index in column.
        /// </summary>
        /// <param name="iStart">The i start.</param>
        private void UpdateActualIndexInColumn(int iStart)
        {
            for (int i = iStart; i < m_columns.Count; i++)
            {
                m_columns[i].ActualIndex = i;
            }
        }

        /// <summary>
        /// Updates the index list.
        /// </summary>
        /// <param name="actualIndex">The actual index.</param>
        /// <param name="index">The index.</param>
        private void UpdateIndexList(int actualIndex, int index)
        {
            for (int i = 0; i < index; i++)
            {
                int num2 = m_actualIndices[i];

                if (num2 > actualIndex && m_actualIndices.Count <= m_columns.Count)
                {
                    m_actualIndices.Insert(i, num2 - 1);
                }
            }

            for (int j = index + 1; j < m_actualIndices.Count; j++)
            {
                int num4 = m_actualIndices[j];

                if (num4 < actualIndex && m_actualIndices.Count <= m_columns.Count)
                {
                    m_actualIndices.Insert(j - 1, num4);
                }
                else if (num4 > actualIndex && m_actualIndices.Count <= m_columns.Count)
                {
                    m_actualIndices.Insert(j - 1, num4 - 1);
                }
            }

            m_actualIndices.RemoveAt(m_actualIndices.Count - 1);
        }

        /// <summary>
        /// Validates the column for insert.
        /// </summary>
        /// <param name="column">The column.</param>
        private void ValidateColumnForInsert(TreeViewColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            if (column.ActualIndex > 0)
            {
                throw new InvalidOperationException("ListView_NotAllowShareColumnToTwoColumnCollection");
            }
        }

        /// <summary>
        /// Verifies the access.
        /// </summary>
        private void VerifyAccess()
        {
            if (IsImmutable)
            {
                throw new InvalidOperationException("ListView_TreeViewColumnCollectionIsReadOnly");
            }

            base.CheckReentrancy();
        }

        /// <summary>
        /// Verifies the index in range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="indexName">Name of the index.</param>
        private void VerifyIndexInRange(int index, string indexName)
        {
            if ((index < 0) || (index >= m_actualIndices.Count))
            {
                throw new ArgumentOutOfRangeException(indexName);
            }
        }

        #endregion Implementation
    }
}