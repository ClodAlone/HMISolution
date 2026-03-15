// <copyright file="TreeViewColumnCollectionChangedEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents TreeViewColumnCollectionChangedEventArgs class
    /// </summary>
    internal class TreeViewColumnCollectionChangedEventArgs : NotifyCollectionChangedEventArgs
    {
        #region Members

        /// <summary>
        /// Presents Actual index
        /// </summary>
        private int m_actualIndex = -1;

        /// <summary> cleared columns
        /// Presents
        /// </summary>
        private ReadOnlyCollection<TreeViewColumn> m_clearedColumns = null;

        /// <summary>
        /// Presents column
        /// </summary>
        private TreeViewColumn m_column = null;

        /// <summary>
        /// Presents property name
        /// </summary>
        private string m_propertyName = String.Empty;

        #endregion Members

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="clearedColumns">The cleared columns.</param>
        internal TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction action, TreeViewColumn[] clearedColumns)
            : base(action)
        {
            m_actualIndex = -1;
            m_clearedColumns = Array.AsReadOnly<TreeViewColumn>(clearedColumns);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="propertyName">Name of the property.</param>
        internal TreeViewColumnCollectionChangedEventArgs(TreeViewColumn column, string propertyName)
            : base(NotifyCollectionChangedAction.Reset)
        {
            m_actualIndex = -1;
            m_column = column;
            m_propertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="changedItem">The changed item.</param>
        /// <param name="index">The index.</param>
        /// <param name="actualIndex">The actual index.</param>
        internal TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction action, TreeViewColumn changedItem, int index, int actualIndex)
            : base(action, changedItem, index)
        {
            m_actualIndex = -1;
            m_actualIndex = actualIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="changedItem">The changed item.</param>
        /// <param name="index">The index.</param>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="actualIndex">The actual index.</param>
        internal TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction action, TreeViewColumn changedItem, int index, int oldIndex, int actualIndex)
            : base(action, changedItem, index, oldIndex)
        {
            m_actualIndex = -1;
            m_actualIndex = actualIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewColumnCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        /// <param name="index">The index.</param>
        /// <param name="actualIndex">The actual index.</param>
        internal TreeViewColumnCollectionChangedEventArgs(NotifyCollectionChangedAction action, TreeViewColumn newItem, TreeViewColumn oldItem, int index, int actualIndex)
            : base(action, newItem, oldItem, index)
        {
            m_actualIndex = -1;
            m_actualIndex = actualIndex;
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets the actual index.
        /// </summary>
        /// <value>The actual index.</value>
        internal int ActualIndex
        {
            get
            {
                return m_actualIndex;
            }
        }

        /// <summary>
        /// Gets the cleared columns.
        /// </summary>
        /// <value>The cleared columns.</value>
        internal ReadOnlyCollection<TreeViewColumn> ClearedColumns
        {
            get
            {
                return m_clearedColumns;
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        internal TreeViewColumn Column
        {
            get
            {
                return m_column;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        internal string PropertyName
        {
            get
            {
                return m_propertyName;
            }
        }

        #endregion Properties
    }
}