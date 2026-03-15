#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{

    /// <summary>
    /// Represents the DragMove event data.
    /// </summary>
    public class DragMoveEventArgs : EventArgs
    {
         #region Public properies
        /// <summary>
        /// Gets or sets the header before edit.
        /// </summary>
        /// <value>The header before edit.</value>
        public object Item
        {
            get;
            set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the BeforeEditEventArgs class.
        /// </summary>
        /// <param name="item">The header object.</param>
        public DragMoveEventArgs(object item)
        {
            Item = item;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ExpandCollapseEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Cancel
        {
            set;
            get;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeCancellableEditEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public NodeCancellableEditEventArgs(object node)
        {
            Node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Node
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeEditEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public NodeEditEventArgs(object node)
        {
            Node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Node
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoadonDemandEventArgs : RoutedEventArgs
    {
        private object treeViewItem;

        /// <summary>
        /// 
        /// </summary>
        public object TreeViewItem
        {
            get { return treeViewItem; }
            set { treeViewItem = value; }
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeEditorCancellableEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public NodeEditorCancellableEventArgs(object node)
        {
            Node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Node
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ContinueEditing
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NodeEditorEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public NodeEditorEventArgs(object node)
        {
            Node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Node
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the TreeViewAdvDrag EventArgs.
    /// </summary>
    public class TreeViewAdvDragEventArgs : EventArgs
    {
        /// <summary>
        /// When dragging on control this event executed.
        /// </summary>
        /// <param name="source">Passing the source.</param>
        /// <param name="sourceParentTreeViewItem">Passing Parent Tree View item.</param>
        /// <param name="sourceParentTreeView">Passing parent Tree View.</param>
        /// <param name="target">Setting the target.</param>
        /// <param name="targetParentTreeViewItem">Setting the Parent Tree view item.</param>
        /// <param name="targetParentTreeView">Setting the parent tree view.</param>
        /// /// <param name="dropmode"></param>
        public TreeViewAdvDragEventArgs(List<TreeViewItemAdv> source, 
            List<TreeViewItemAdv> sourceParentTreeViewItem, 
            List<TreeViewAdv> sourceParentTreeView, 
            TreeViewItemAdv target, 
            TreeViewItemAdv targetParentTreeViewItem, 
            TreeViewAdv targetParentTreeView,DropMode dropmode)
        {
            //this.Data = data;
            //this.DestinationData = destinationObject;
            this.Source = source;
            this.SourceParentTreeViewItem = sourceParentTreeViewItem;
            this.SourceParentTreeView = sourceParentTreeView;
            this.Target = target;
            this.TargetParentTreeViewItem = targetParentTreeViewItem;
            this.TargetParentTreeView = targetParentTreeView;
            this.DropMode = dropmode;
        }

        ///// <summary>
        ///// Gets or sets the destination data.
        ///// </summary>
        ///// <value>The destination data.</value>
        //public object DestinationData
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TreeViewAdvDragEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get;
            set;
        }

        ///// <summary>
        ///// Gets or sets the data.
        ///// </summary>
        ///// <value>The data.</value>
        //public object Data
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// 
        /// </summary>
        public DropMode DropMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public List<TreeViewItemAdv> Source
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source parent tree view.
        /// </summary>
        /// <value>The source parent tree view.</value>
        public List<TreeViewAdv> SourceParentTreeView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source parent tree view item.
        /// </summary>
        /// <value>The source parent tree view item.</value>
        public List<TreeViewItemAdv> SourceParentTreeViewItem
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public TreeViewItemAdv Target
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target parent tree view.
        /// </summary>
        /// <value>The target parent tree view.</value>
        public TreeViewAdv TargetParentTreeView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target parent tree view item.
        /// </summary>
        /// <value>The target parent tree view item.</value>
        public TreeViewItemAdv TargetParentTreeViewItem
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the TreeViewAdvDrag EventArgs.
    /// </summary>
    public class TreeViewAdvDragStartEventArgs : EventArgs
    {
        /// <summary>
        /// When dragging on control this event executed.
        /// </summary>
        /// <param name="source">Passing the source.</param>
        /// <param name="sourceParentTreeViewItem">Passing Parent Tree View item.</param>
        /// <param name="sourceParentTreeView">Passing parent Tree View.</param>
        public TreeViewAdvDragStartEventArgs(List<TreeViewItemAdv> source,
            List<TreeViewItemAdv> sourceParentTreeViewItem,
            List<TreeViewAdv> sourceParentTreeView)
        {
            this.Source = source;
            this.SourceParentTreeViewItem = sourceParentTreeViewItem;
            this.SourceParentTreeView = sourceParentTreeView;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TreeViewAdvDragEventArgs"/> is handled.
        /// </summary>
        /// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse event args.
        /// </summary>
        /// <value>The mouse event args.</value>
        public MouseEventArgs MouseEventArgs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public List<TreeViewItemAdv> Source
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source parent tree view.
        /// </summary>
        /// <value>The source parent tree view.</value>
        public List<TreeViewAdv> SourceParentTreeView
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source parent tree view item.
        /// </summary>
        /// <value>The source parent tree view item.</value>
        public List<TreeViewItemAdv> SourceParentTreeViewItem
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContextMenuEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public object Node { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Handled { get; set; }
    }

}
