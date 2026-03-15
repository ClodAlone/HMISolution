#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Specifies how the nodes are positioned when dropped onto the diagram.
    /// </summary>
    public enum DropPosition
    {
        /// <summary>
        /// Nodes are positioned related to the location of the drop.
        /// </summary>
        RelativeOffset,

        /// <summary>
        /// Positions each node at the location of the drop.
        /// </summary>
        AbsolutePosition
    }

    /// <summary>
    /// Encapsulates data needed to drag-and-drop a collection of nodes.
    /// </summary>
    public class DragDropData
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropData"/> class.
        /// </summary>
        public DragDropData()
        {
            this.nodes = new NodeCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropData"/> class.
        /// </summary>
        /// <param name="node">Node to drag-and-drop.</param>
        /// <param name="dragHelper">Drag helper</param>
        public DragDropData(Node node, DiagramDragHelper dragHelper)
        {
            this.nodes = new NodeCollection();
            this.nodes.Add(node);
            this.dropPosition = DropPosition.AbsolutePosition;
            m_dragHelper = dragHelper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropData"/> class.
        /// </summary>
        /// <param name="node">Node to drag-and-drop.</param>
        /// <param name="dropPos">Determines how the node is dropped onto the diagram.</param>
        /// <param name="dragHelper">Drag helper </param>
        public DragDropData(Node node, DropPosition dropPos, DiagramDragHelper dragHelper)
        {
            this.nodes = new NodeCollection();
            this.nodes.Add(node);
            this.dropPosition = dropPos;
            m_dragHelper = dragHelper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropData"/> class.
        /// </summary>
        /// <param name="nodes">Nodes to drag-and-drop.</param>
        /// <param name="dragHelper">Drag helper </param>
        public DragDropData(NodeCollection nodes, DiagramDragHelper dragHelper)
        {
            this.nodes = new NodeCollection();
            this.nodes.AddRange(nodes);
            if (nodes.Count > 1)
            {
                this.dropPosition = DropPosition.RelativeOffset;
            }
            else
            {
                this.dropPosition = DropPosition.AbsolutePosition;
            }
            m_dragHelper = dragHelper;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropData"/> class.
        /// </summary>
        /// <param name="nodes">Nodes to drag-and-drop.</param>
        /// <param name="dropPos">Determines how the node is dropped onto the diagram.</param>
        /// <param name="dragHelper">Drag helper</param>
        public DragDropData(NodeCollection nodes, DropPosition dropPos, DiagramDragHelper dragHelper)
        {
            this.nodes = new NodeCollection();
            this.nodes.AddRange(nodes);
            this.dropPosition = dropPos;
            m_dragHelper = dragHelper;
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the drag helper.
        /// </summary>
        public DiagramDragHelper DragHelper
        {
            get { return m_dragHelper; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether dragged node cue is enabled or not
        /// </summary>
        public bool DragNodeCueEnabled
        {
            get
            {
                return m_bDragNodeCueEnabled;
            }
            set
            {
                if (value != m_bDragNodeCueEnabled)
                    m_bDragNodeCueEnabled = value;
            }
        }

        /// <summary>
        /// Gets the collection of nodes to drag-and-drop.
        /// </summary>
        public NodeCollection Nodes
        {
            get
            {
                return this.nodes;
            }
        }

        /// <summary>
        /// Gets or sets how the nodes are positioned on the diagram.
        /// </summary>
        public DropPosition DropPosition
        {
            get
            {
                return this.dropPosition;
            }
            set
            {
                this.dropPosition = value;
            }
        }

        #endregion

        #region Member Variables

        private DropPosition dropPosition = DropPosition.AbsolutePosition;
        private NodeCollection nodes = null;
        private bool m_bDragNodeCueEnabled = true;
        private DiagramDragHelper m_dragHelper;
        #endregion
    }
}
