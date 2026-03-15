// <copyright file="DragTreeViewItemAdvEventArgs.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Windows;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Contains arguments relevant to drag-and-drop of the TreeViewAdv.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public class DragTreeViewItemAdvEventArgs : <see cref="RoutedEventArgs"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// This managed class is not typically used in XAML.
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// This class is responsible for packaging the event data for drag-and-drop.
    /// </remarks>
    /// <seealso cref="TreeViewAdv"/>
    /// <seealso cref="RoutedEventArgs"/>
    public class DragTreeViewItemAdvEventArgs : RoutedEventArgs
    {
        #region Member

        /// <summary>
        /// Data object that contains the data associated with the dragging items.
        /// </summary>
        private DataObject m_data = null;

        /// <summary>
        /// Local variable for treeview item collection
        /// </summary>
        private TreeViewItemAdvCollection m_collection = null;

        /// <summary>
        /// Local variable for cancel flag
        /// </summary>
        private bool m_cancel = false;

        /// <summary>
        /// Local variable for allow drop flag
        /// </summary>
        private bool m_AllowDragDrop = true;

        /// <summary>
        /// Local variable for target object
        /// </summary>
        private object _target;

        /// <summary>
        /// Local variable for target over object
        /// </summary>
        private object _targetover;

        /// <summary>
        /// Local variable for drop index
        /// </summary>
        private int m_dropIndex = -1;

        /// <summary>
        /// The target drop-and-drop operation.
        /// </summary>
        private TreeViewItemAdvDragDropEffects m_enEffects = TreeViewItemAdvDragDropEffects.None;

        #endregion Member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="DragTreeViewItemAdvEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public DragTreeViewItemAdvEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data of data object.</value>
        public DataObject Data
        {
            get
            {
                return m_data;
            }

            set
            {
                if (value != m_data)
                {
                    m_data = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the dragging items.
        /// </summary>
        /// <value>The dragging items.</value>
        public TreeViewItemAdvCollection DraggingItems
        {
            get
            {
                return m_collection;
            }
            internal set
            {
                if (value != m_collection)
                {
                    m_collection = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the target drop item.
        /// </summary>
        /// <value>The target drop item.</value>
        public object TargetDropItem
        {
            get
            {
                return _target;
            }
            internal set
            {
                if (value != _target)
                {
                    _target = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the target over item.
        /// </summary>
        /// <value>The target over item.</value>
        public object TargetOverItem
        {
            get
            {
                return _targetover;
            }
            internal set
            {
                if (value != _targetover)
                {
                    _targetover = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this drop operation needs to be cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }
            set
            {
                m_cancel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this drop operation needs to be cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool AllowDragDrop
        {
            get
            {
                return m_AllowDragDrop;
            }
            set
            {
                m_AllowDragDrop = value;
            }
        }

        /// <summary>
        /// Gets or sets the effects.
        /// </summary>
        /// <value>The effects.</value>
        public TreeViewItemAdvDragDropEffects Effects
        {
            get
            {
                return m_enEffects;
            }

            set
            {
                if (value != m_enEffects)
                {
                    m_enEffects = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the drop.
        /// </summary>
        /// <value>The index of the drop.</value>
        public int DropIndex
        {
            get
            {
                return m_dropIndex;
            }
            set
            {
                m_dropIndex = value;
            }
        }

        #endregion Properties
    }
}