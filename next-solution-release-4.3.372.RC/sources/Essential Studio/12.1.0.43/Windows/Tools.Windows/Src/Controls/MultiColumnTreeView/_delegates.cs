#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.NodeMouseClick"/> event.
    /// </summary>
    public delegate void TreeNodeAdvMouseClickArgs(object sender, MultiColumnTreeViewAdvMouseClickEventArgs e);
    /// <summary>
    /// Provides data for the <see cref="MultiColumnTreeViewAdv.NodeMouseClick"/> event.
    /// </summary>
    public class MultiColumnTreeViewAdvMouseClickEventArgs : EventArgs
    {
        private TreeNodeAdv node;
        private MouseButtons m_Mousebutton;
        private int m_Click;
        private int x;
        private int y;
        private int m_Delta;

        public MultiColumnTreeViewAdvMouseClickEventArgs(TreeNodeAdv treenode, MouseButtons button, int clicks, int X, int Y, int delta)
        {
            this.node = treenode;
            this.Mousebutton = button;
            this.m_Click = clicks;
            this.x = X;
            this.y = Y;
            this.m_Delta = delta;
        }

        /// <summary>
        /// Returns TreeNodeAdv instance
        /// </summary>

        public TreeNodeAdv Node
        {
            get { return node; }
        }
        
        /// <summary>
        /// Gets/Sets which mouse button was pressed
        /// </summary>
        public MouseButtons Mousebutton
        {
            get { return m_Mousebutton; }
            set { m_Mousebutton = value; }
        }

        /// <summary>
        /// Gets the number of times the mouse button was pressed and released
        /// </summary>
        public int Clicks
        {
            get { return m_Click; }
        }

        /// <summary>
        /// Gets the x-coordinate of the mouse during the generating mouse event.
        /// </summary>
        public int X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the y-coordinate of the mouse during the generating mouse event
        /// </summary>
        public int Y
        {
            get { return y; }
        }

        /// <summary>
        /// Gets a signed count of the number of detents the mouse wheel has rotated, multiplied by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
        /// </summary>
        public int Delta
        {
            get { return m_Delta; }
        }
    }

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.OnNodeAfterFound"/> event.
    /// </summary>
    public delegate void TreeViewOnAfterFindArgs(object sender, TreeNodeAdvAfterFindArgs e);
    public class TreeNodeAdvAfterFindArgs : EventArgs
    {
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvAfterFindArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv instance</param>
        /// <param name="n_searchText">TreeNodeAdv Text which needs to be searched</param>
        public TreeNodeAdvAfterFindArgs(TreeNodeAdv m_node, string n_searchText)
        {
            node = m_node;
            m_searchText = n_searchText;
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdv instance that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets value of search string
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.OnNodeBeforeFind"/> event.
    /// </summary>
    public delegate void TreeViewOnBeforeFindArgs(object sender, TreeNodeAdvBeforeFindArgs e);
    public class TreeNodeAdvBeforeFindArgs : SyncfusionCancelEventArgs
    {
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeViewOnBeforeFindArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv instance</param>
        /// <param name="n_searchText">TreeNodeAdv text which needs to be searched</param>
        public TreeNodeAdvBeforeFindArgs(TreeNodeAdv m_node, string n_searchText)
        {
            node = m_node;
            m_searchText = n_searchText;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.OnNodeReplacing"/> event.
    /// </summary>
    public delegate void TreeViewOnReplacingArgs(object sender, TreeNodeAdvOnReplacingArgs e);
    public class TreeNodeAdvOnReplacingArgs : SyncfusionCancelEventArgs
    {
        private string m_replaceText = string.Empty;
        private string m_searchText = string.Empty;
        private TreeViewSearchOption m_treesearchOption;
        private TreeViewSearchRange m_treesearchrange;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvOnReplacingArgs class
        /// </summary>
        /// <param name="m_node">TreeNodeAdv Instance</param>
        /// <param name="n_searchText">Search String</param>
        /// <param name="n_replace">Replace String</param>
        /// <param name="findOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        public TreeNodeAdvOnReplacingArgs(TreeNodeAdv m_node, string n_searchText, string n_replace, TreeViewSearchOption findOption, TreeViewSearchRange searchRange)
        {
            node = m_node;
            m_searchText = n_searchText;
            m_replaceText = n_replace;
            m_treesearchOption = findOption;
            m_treesearchrange = searchRange;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches search string
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                if (!base.Cancel)
                    return node;
                else
                    return null;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeViewAdv search option value
        /// </summary>
        public TreeViewSearchOption TreeViewSearchOption
        {
            get
            {
                if (!base.Cancel)
                    return m_treesearchOption;
                else
                    return TreeViewSearchOption.MatchCase;
            }
            set
            {
                if (m_treesearchOption != value)
                {
                    m_treesearchOption = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeViewAdv search range value
        /// </summary>
        public TreeViewSearchRange TreeViewSearchRange
        {
            get
            {
                if (!base.Cancel)
                    return m_treesearchrange;
                else
                    return TreeViewSearchRange.TreeView;
            }
            set
            {
                if (m_treesearchrange != value)
                {
                    m_treesearchrange = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                if (!base.Cancel)
                    return m_searchText;
                else
                    return string.Empty;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv replace text value
        /// </summary>
        public string ReplaceText
        {
            get
            {
                if (!base.Cancel)
                    return m_replaceText;
                else
                    return string.Empty;
            }
            set
            {
                if (m_replaceText != value)
                {
                    m_replaceText = value;
                }
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.OnNodeReplaced"/> event.
    /// </summary>
    public delegate void TreeViewOnReplacedArgs(object sender, TreeNodeAdvOnReplacedArgs e);
    public class TreeNodeAdvOnReplacedArgs : SyncfusionEventArgs
    {
        private string m_replaceText = string.Empty;
        private string m_searchText = string.Empty;
        private TreeNodeAdv node;

        /// <summary>
        /// Initializes new instances of TreeNodeAdvOnReplacedArgs class
        /// </summary>
        /// <param name="n_searchText">Search String</param>
        /// <param name="n_replace">Replace String</param>
        /// <param name="m_node">TreeNodeAdv Instances</param>
        public TreeNodeAdvOnReplacedArgs(string n_searchText, string n_replace, TreeNodeAdv m_node)
        {
            m_searchText = n_searchText;
            m_replaceText = n_replace;
            node = m_node;
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv instance value that matches Search String
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
            set
            {
                if (node != value && value != null)
                {
                    node = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv search string value
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (m_searchText != value)
                {
                    m_searchText = value;
                }
            }
        }

        /// <summary>
        /// Gets/Sets TreeNodeAdv replace text value
        /// </summary>
        public string ReplaceText
        {
            get
            {
                return m_replaceText;
            }
            set
            {
                if (m_replaceText != value)
                {
                    m_replaceText = value;
                }
            }
        }
    }


    /// <summary>
    /// Provides data for the MultiColumnTreeView selection events.
    /// </summary>
    public class TreeNodeAdvEventArgs : EventArgs
    {
        #region Class members

        private TreeViewAdvAction action;

        private TreeNodeAdv node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the <see cref="TreeViewAdvAction"/> associated with the event.
        /// </summary>
        public TreeViewAdvAction Action
        {
            get
            {
                return this.action;
            }

            set
            {
                this.action = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="TreeNodeAdv"/> associated with the event.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return this.node;
            }

            set
            {
                this.node = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvEventArgs class.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
        public TreeNodeAdvEventArgs(TreeNodeAdv node)
        {
            this.node = node;
        }

        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvEventArgs class.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
        /// <param name="action">A <see cref="TreeViewAdvAction"/> type.</param>
        public TreeNodeAdvEventArgs(TreeNodeAdv node, TreeViewAdvAction action)
        {
            this.node = node;
            this.action = action;
        }
        #endregion
    }

    /// <summary>
    /// Provides data for the cancelable validation events in the MultiColumnTreeView.
    /// </summary>
    public class TreeNodeAdvCancelableEditEventArgs : TreeNodeAdvEditEventArgs
    {
        #region Class members

        private bool cancel;

        private bool continueEditing;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether the event should be cancelled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.cancel;
            }
            set
            {
                this.cancel = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether editing should end now.
        /// </summary>
        /// <value>This property is consulted only when <see cref="Cancel"/> is set to true.
        /// If you Cancel the operation and if this property is set
        /// to false, editing mode will end; otherwise editing mode will be preserved.
        /// Default is true.</value>
        /// <remarks><p>This  property will be ignored by the 
        /// <see cref="MultiColumnTreeView.NodeEditorValidateString"/> event.</p></remarks>
        public bool ContinueEditing
        {
            get
            {
                return this.continueEditing;
            }
            set
            {
                this.continueEditing = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvCancelableEditEventArgs class.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
        /// <param name="label">The new text for the node.</param>
        public TreeNodeAdvCancelableEditEventArgs(TreeNodeAdv node, string label)
            : base(node, label)
        {
            this.cancel = false;
            this.continueEditing = true;
        }
        #endregion
    }

    /// <summary>
    /// Provides data for the editing events in the <see cref="MultiColumnTreeView"/>.
    /// </summary>
    public class TreeNodeAdvEditEventArgs : EventArgs
    {
        #region Class members

        private string label;

        private TreeNodeAdv node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the label for the node.
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> that is currently being edited.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvEditEventArgs class.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/> instance.</param>
        /// <param name="label">The label for the node.</param>
        public TreeNodeAdvEditEventArgs(TreeNodeAdv node, string label)
        {
            this.node = node;
            this.label = label;
        }
        #endregion
    }

    /// <summary>
    /// Provides data for the <see cref="MultiColumnTreeView.BeforeEdit"/> event.
    /// </summary>
    public class TreeNodeAdvBeforeEditEventArgs : CancelEventArgs
    {
        #region Class members
 
        private TextBox textBox;

        private TreeNodeAdv node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/>.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
        }

        /// <summary>
        /// Gets the <see cref="TextBox"/> that is used to edit the node.
        /// </summary>
        public TextBox TextBox
        {
            get
            {
                return textBox;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvBeforeEditEventArgs class.
        /// </summary>
        /// <param name="node">Specifies the <see cref="TreeNodeAdv"/>.</param>
        /// <param name="textBox">A <see cref="TextBox"/> instance.</param>
        public TreeNodeAdvBeforeEditEventArgs(TreeNodeAdv node, TextBox textBox)
        {
            this.node = node;
            this.textBox = textBox;
        }
        #endregion
    }

    public class TreeColumnStyleChangedEventArgs : EventArgs
    {
        #region Class members
  
        private StyleChangedEventArgs m_args;
  
        private TreeColumnAdv m_column;
        #endregion

        #region Class properties
        /// <summary>Gets TreeColumnAdv</summary>
        public TreeColumnAdv Column
        {
            get
            {
                return m_column;
            }
        }

        /// <summary>Gets  StyleChanged EventArgs</summary>
        public StyleChangedEventArgs StyleChange
        {
            get
            {
                return m_args;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeColumnStyleChangedEventArgs class.</summary>
        /// <param name="column">Tree Column </param>
        /// <param name="change"> EventArgs that contains the event data.</param>
        public TreeColumnStyleChangedEventArgs(TreeColumnAdv column, StyleChangedEventArgs change)
        {
            m_column = column;
            m_args = change;
        }
        #endregion
    }

    public class TreeViewColumnSelectedChangedEventArgs : EventArgs
    {
        #region Class members

        private TreeColumnAdv m_column;
        #endregion

        #region Class properties
        /// <summary> Gets TreeColumnAdv</summary>
        public TreeColumnAdv Column
        {
            get
            {
                return m_column;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeViewColumnSelectedChangedEventArgs class.</summary>
        /// <param name="column">Tree Column</param>
        public TreeViewColumnSelectedChangedEventArgs(TreeColumnAdv column)
        {
            m_column = column;
        }
        #endregion
    }

    public class TreeViewColumnResizeEventArgs : EventArgs
    {
        #region Class members
 
        private TreeColumnAdv m_column;
  
        private int m_lastPosition;

        private int m_currentPos;
        #endregion

        #region Class properties
        /// <summary>Gets TreeColumnadv</summary>
        public TreeColumnAdv Column
        {
            get
            {
                return m_column;
            }
        }

        /// <summary> Gets previous horizontal offset relatively start resizing position. </summary>
        public int PreviousPosition
        {
            get
            {
                return m_lastPosition;
            }
        }

        /// <summary> Gets current horizontal offset relatively start resizing position.</summary>
        public int CurrentPosition
        {
            get
            {
                return m_currentPos;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary> Initializes a new instance of the TreeViewColumnResizeEventArgs class.</summary>
        /// <param name="column">Tree Column</param>
        /// <param name="previous">Previous Index</param>
        /// <param name="current">Current Index</param>
        public TreeViewColumnResizeEventArgs(TreeColumnAdv column, int previous, int current)
        {
            m_column = column;
            m_lastPosition = previous;
            m_currentPos = current;
        }
        #endregion
    }

    public class TreeViewColumnResizedEventArgs : EventArgs
    {
        #region Class members
  
        private TreeColumnAdv m_column;

        private int m_lastWidth;

        private int m_currentWidth;
        #endregion

        #region Class properties
        /// <summary>Gets TreeColumnAdv</summary>
        public TreeColumnAdv Column
        {
            get
            {
                return m_column;
            }
        }

        /// <summary> Gets previous column width. </summary>
        public int PreviousWidth
        {
            get
            {
                return m_lastWidth;
            }
        }

        /// <summary> Gets current column width.</summary>
        public int CurrentWidth
        {
            get
            {
                return m_currentWidth;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeViewColumnResizedEventArgs class.</summary>
        /// <param name="column"> Tree Column </param>
        /// <param name="previous">Previous Index</param>
        /// <param name="current">Current Index</param>
        public TreeViewColumnResizedEventArgs(TreeColumnAdv column, int previous, int current)
        {
            m_column = column;
            m_lastWidth = previous;
            m_currentWidth = current;
        }
        #endregion
    }
    public class TreeViewAdvSelectionEventArgs : EventArgs
    {
        #region Class members
        private SelectedNodesCollection m_selectedNodes;
 
        private TreeViewAdvAction m_action;
        #endregion

        #region Class properties
        /// <summary>Gets selected nodes collection</summary>
        public SelectedNodesCollection SelectedNodes
        {
            get
            {
                return this.m_selectedNodes;
            }
        }

        /// <summary>Gets TreeViewAdv Action  </summary>
        public TreeViewAdvAction Action
        {
            get
            {
                return this.m_action;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeViewAdvSelectionEventArgs class.</summary>
        /// <param name="selectedNodes">Selected Nodes Collection</param>
        /// <param name="action">TreeviewAdv Action</param>
        public TreeViewAdvSelectionEventArgs(SelectedNodesCollection selectedNodes, TreeViewAdvAction action)
        {
            this.m_selectedNodes = selectedNodes;
            this.m_action = action;
        }
        #endregion
    }

    /// <summary>
    /// Provides data for the <see cref="TreeViewAdv.BeforeSelect"/> event.
    /// </summary>
    public class TreeViewAdvCancelableSelectionEventArgs : TreeViewAdvSelectionEventArgs
    {
        #region Class members
        private bool m_cancel;
        #endregion

        #region Class properties
        /// <summary> Gets or sets a value indicating whether</summary>
        public bool Cancel
        {
            get
            {
                return this.m_cancel;
            }
            set
            {
                this.m_cancel = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeViewAdvCancelableSelectionEventArgs class.</summary>
        /// <param name="selectedNodes">Selected Nodes Collection</param>
        /// <param name="action"> TreeViewAdv action</param>
        /// <param name="cancel">True if cancel the event</param>
        public TreeViewAdvCancelableSelectionEventArgs(SelectedNodesCollection selectedNodes, TreeViewAdvAction action, bool cancel)
            : base(selectedNodes, action)
        {
            this.m_cancel = cancel;
        }
        #endregion
    }

    /// <summary>
    /// Provides data for the <see cref="TreeViewAdv.AfterCollapse"/> event.
    /// </summary>
    public class TreeViewAdvNodeEventArgs : EventArgs
    {
        #region Class members
        private TreeNodeAdv node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> which is associated with the action.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return this.node;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeViewAdvNodeEventArgs class.</summary>
        /// <param name="node">Tree node </param>
        public TreeViewAdvNodeEventArgs(TreeNodeAdv node)
        {
            this.node = node;
        }
        #endregion
    }

    /// <summary>
    /// Custom EventArgs class which is used in <see cref="TreeViewAdv.BeforeExpand"/> event.
    /// </summary>
    public class TreeViewAdvCancelableNodeEventArgs : TreeViewAdvNodeEventArgs
    {
        #region Class members

        private bool m_bCancel = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether the event is to be cancelled.
        /// </summary>
        public bool Cancel
        {
            get
            {
                return this.m_bCancel;
            }
            set
            {
                this.m_bCancel = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeViewAdvCancelableNodeEventArgs class
        /// </summary>
        /// <param name="node">The node which is associated with the event</param>
        /// <param name="cancel">True if cancel the event</param>
        public TreeViewAdvCancelableNodeEventArgs(TreeNodeAdv node, bool cancel)
            : base(node)
        {
            this.m_bCancel = cancel;
        }
        #endregion
    }

    /// <summary>
    /// Custom EventArgs class that is passed to BeforeCheck event of <see cref="TreeViewAdv.BeforeCheck"/> event.
    /// </summary>
    public class TreeNodeAdvBeforeCheckEventArgs : TreeViewAdvCancelableNodeEventArgs
    {
        #region Class members

        private CheckState m_state;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the checkstate of the node
        /// </summary>
        public CheckState NewCheckState
        {
            get
            {
                return this.m_state;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvBeforeCheckEventArgs class 
        /// </summary>
        /// <param name="node">The node which is involved in the action</param>
        /// <param name="cancel">Parameter to indicate whether the action should be cancelled</param>
        /// <param name="newState">The new state of check box associated with node</param>
        public TreeNodeAdvBeforeCheckEventArgs(TreeNodeAdv node, bool cancel, CheckState newState)
            : base(node, cancel)
        {
            this.m_state = newState;
        }
        #endregion
    }

    /// <summary>Base class fro TreeNode paint event message classes.</summary>
    public class TreeNodeAdvBasePaintEventArgs : EventArgs
    {
        #region Class constants
        /// <summary>Declaration of boolean values that can be used handled
        /// by class inheritors.</summary>
        internal enum InternalFields : int
        {
            /// <summary>
            /// Represents Selected
            /// </summary>
            Selected,

            /// <summary>Represents Active</summary>
            Active,

            /// <summary>Represents Full Row Select</summary>
            FullRowSelect,

            /// <summary>Represents HotTracker</summary>
            HotTracker,

            /// <summary>Represents handled</summary>
            Handled,

            /// <summary>Represents Handled Plus Minus</summary>
            HandledPlusMinus,

            /// <summary>Represents Handled CheckBox</summary>
            HandledCheckBox,

            /// <summary>Represents Handled Option Button</summary>
            HandledOptionButton,

            /// <summary>Represents Handled LeftImagrList</summary>
            HandledLeftImageList,

            /// <summary>Represents Handled State ImageList</summary>
            HandledStateImageList,

            /// <summary>Represents Hadled Right Image List</summary>
            HandledRightImageList,

            /// <summary>Represents Handled Text</summary>
            HandledText,
        }
        #endregion

        #region Class members

        private Graphics graphics;

        private Rectangle bounds;

        private TreeNodeAdv node;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the <see cref="TreeNodeAdv"/> which is associated with the action.
        /// </summary>
        public TreeNodeAdv Node
        {
            get
            {
                return node;
            }
        }

        /// <summary>
        /// Gets the bounds of <see cref="TreeNodeAdv"/></summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Drawing.Graphics"/> object associated with the event
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>Initializes a new instance of the TreeNodeAdvBasePaintEventArgs class.</summary>
        /// <param name="g">Graphics object</param>
        /// <param name="node">Tree node</param>
        public TreeNodeAdvBasePaintEventArgs(Graphics g, TreeNodeAdv node)
            : this(g, node, node.Bounds)
        {
        }

        /// <summary>Initializes a new instance of the TreeNodeAdvBasePaintEventArgs class.</summary>
        /// <param name="g">Graphics object</param>
        /// <param name="node"> Tree node</param>
        /// <param name="bounds"> Rectangle bounds</param>
        public TreeNodeAdvBasePaintEventArgs(Graphics g, TreeNodeAdv node, Rectangle bounds)
        {
            this.node = node;
            this.graphics = g;
            this.bounds = bounds;
        }
        #endregion
    }

    /// <summary>
    /// Event args that are passed in the DrawNode event of the TreeViewAdv control.
    /// Contains information about the appearance of the node and the location and sizes of different parts of the node.
    /// </summary>
    public class TreeNodeAdvPaintEventArgs : TreeNodeAdvBasePaintEventArgs
    {
        #region Class members

        private int level;

        private int indent;

        private Point textLocation;

        private Color foreColor = Color.Empty;

        private int rightMargin = 0;

        /// <summary>Optimization of boolean values storage. (12 boolean varables)</summary>
        private BitArray _bits = new BitArray(12, false);
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the level of node
        /// </summary>
        /// <remarks>
        /// An instance of <see cref="System.Int32"/></remarks>
        public int Level
        {
            get
            {
                return level;
            }
        }

        /// <summary>
        /// Gets the indent of node
        /// </summary>
        /// <remarks>
        /// An instance of <see cref="System.Int32"/></remarks>
        public int Indent
        {
            get
            {
                return indent;
            }
        }

        /// <summary>
        /// Gets or sets foreground color
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                this.foreColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the RightMargin
        /// </summary>
        public int RightMargin
        {
            get
            {
                return rightMargin;
            }
            set
            {
                rightMargin = value;
            }
        }

        /// <summary>
        /// Gets the location of text as <see cref="System.Drawing.Point"/></summary>
        public Point TextLocation
        {
            get
            {
                return textLocation;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled
        /// </summary>
        public bool Handled
        {
            get
            {
                return _bits.Get((int)InternalFields.Handled);
            }
            set
            {
                _bits.Set((int)InternalFields.Handled, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting PlusMinus button was handled
        /// </summary>
        public bool HandledPlusMinus
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledPlusMinus);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledPlusMinus, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting CheckBox was handled
        /// </summary>
        public bool HandledCheckBox
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledCheckBox);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledCheckBox, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting Option button was handled
        /// </summary>
        public bool HandledOptionButton
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledOptionButton);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledOptionButton, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting images in left side was handled
        /// </summary>
        public bool HandledLeftImageList
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledLeftImageList);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledLeftImageList, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting state image was handled
        /// </summary>
        public bool HandledStateImageList
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledStateImageList);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledStateImageList, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting text was handled
        /// </summary>
        public bool HandledText
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledText);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledText, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether painting images in right side was handled
        /// </summary>
        public bool HandledRightImageList
        {
            get
            {
                return _bits.Get((int)InternalFields.HandledRightImageList);
            }
            set
            {
                _bits.Set((int)InternalFields.HandledRightImageList, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return _bits.Get((int)InternalFields.Selected);
            }
            set
            {
                _bits.Set((int)InternalFields.Selected, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is Active
        /// </summary>
        public bool Active
        {
            get
            {
                return _bits.Get((int)InternalFields.Active);
            }
            set
            {
                _bits.Set((int)InternalFields.Active, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the FullRowSelect is enabled
        /// </summary>
        public bool FullRowSelect
        {
            get
            {
                return _bits.Get((int)InternalFields.FullRowSelect);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HotTracking is enabled
        /// </summary>
        public bool HotTracked
        {
            get
            {
                return _bits.Get((int)InternalFields.HotTracker);
            }
            set
            {
                _bits.Set((int)InternalFields.HotTracker, value);
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the TreeNodeAdvPaintEventArgs class 
        /// </summary>
        /// <param name="node">The node associated with event</param>
        /// <param name="g">The instance of Graphics class</param>
        /// <param name="bounds">Bounds of the Node</param>
        /// <param name="textLocation">Location of Text</param>
        /// <param name="level">The Level of Node</param>
        /// <param name="indent">The Indent of Node</param>
        /// <param name="selected">Indicates whether the Node is selected</param>
        /// <param name="active">Indicates whether the Node is active</param>
        /// <param name="fullRowSelect">Indicates whether FullRowSelect is enabled</param>
        /// <param name="hotTracked">Indicates whether HotTracking is enabled</param>
        /// <param name="foreColor">The foreground color of node</param>
        public TreeNodeAdvPaintEventArgs(TreeNodeAdv node, Graphics g, Rectangle bounds, Point textLocation, int level, int indent, bool selected, bool active, bool fullRowSelect, bool hotTracked, Color foreColor)
            : base(g, node, bounds)       
        {
            this.textLocation = textLocation;
            this.level = level;
            this.indent = indent;
            this.foreColor = foreColor;

            _bits.Set((int)InternalFields.Selected, selected);
            _bits.Set((int)InternalFields.Active, active);
            _bits.Set((int)InternalFields.FullRowSelect, fullRowSelect);
            _bits.Set((int)InternalFields.HotTracker, hotTracked);
        }
        #endregion
    }

    /// <summary>
    /// Event args that are passed in the NodeBackGround event of the TreeViewAdv control. 
    /// Contains information about the appearance of the node background and the location and sizes of different parts of the node.
    /// </summary>
    public class TreeNodeAdvPaintBackgroundEventArgs : TreeNodeAdvBasePaintEventArgs
    {
        #region Class members
        private BrushInfo brushInfo;

        /// <summary>Optimization of boolean values storage. (5 boolean varaibles)</summary>
        private BitArray _bits = new BitArray(5, false);
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value indicating whether the node is selected
        /// </summary>
        public bool Selected
        {
            get
            {
                return _bits.Get((int)InternalFields.Selected);
            }
            set
            {
                _bits.Set((int)InternalFields.Selected, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is Active
        /// </summary>
        public bool Active
        {
            get
            {
                return _bits.Get((int)InternalFields.Active);
            }
            set
            {
                _bits.Set((int)InternalFields.Active, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the FullRowSelect is enabled
        /// </summary>
        public bool FullRowSelect
        {
            get
            {
                return _bits.Get((int)InternalFields.FullRowSelect);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HotTracking is enabled
        /// </summary>
        public bool HotTracked
        {
            get
            {
                return _bits.Get((int)InternalFields.HotTracker);
            }
            set
            {
                _bits.Set((int)InternalFields.HotTracker, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled
        /// </summary>
        public bool Handled
        {
            get
            {
                return _bits.Get((int)InternalFields.Handled);
            }
            set
            {
                _bits.Set((int)InternalFields.Handled, value);
            }
        }

        /// <summary>
        /// Gets or sets the BrushInfo with which the background will be painted by default, if you don't 
        /// mark this event as handled.
        /// </summary>
        /// <remarks>You can optionally change the properties of this BrushInfo object
        /// or provide a new BrushInfo without
        /// marking this event as handled <see cref="Handled"/>.</remarks>
        public BrushInfo BrushInfo
        {
            get
            {
                return this.brushInfo;
            }
            set
            {
                this.brushInfo = value;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>Initializes a new instance of the TreeNodeAdvPaintBackgroundEventArgs class.</summary>
        /// <param name="node">The node associated with event</param>
        /// <param name="g">The instance of Graphics class</param>
        /// <param name="selected">Indicates whether the Node is selected</param>
        /// <param name="active">Indicates whether the Node is active</param>
        /// <param name="fullRowSelect">Indicates whether FullRowSelect is enabled</param>
        /// <param name="hotTracked">Indicates whether HotTracking is enabled</param>
        /// <param name="brushInfo">The BrushInfo with which the background will be painted </param>
        public TreeNodeAdvPaintBackgroundEventArgs(TreeNodeAdv node, Graphics g, bool selected, bool active, bool fullRowSelect, bool hotTracked, BrushInfo brushInfo) : base(g, node)
        {
            this.brushInfo = brushInfo;

            _bits.Set((int)InternalFields.Selected, selected);
            _bits.Set((int)InternalFields.Active, active);
            _bits.Set((int)InternalFields.FullRowSelect, fullRowSelect);
            _bits.Set((int)InternalFields.HotTracker, hotTracked);
        }
        #endregion
    }

    /// <summary>TreeColumn Style ChangedEventHandler</summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeColumnStyleChangedEventHandler(object sender, TreeColumnStyleChangedEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.ColumnSelected"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeColumnChangedEventHandler(object sender, TreeViewColumnSelectedChangedEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.ColumnResizing"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeColumnResizeEventHandler(object sender, TreeViewColumnResizeEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.ColumnResized"/> event.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeColumnResizedEventHandler(object sender, TreeViewColumnResizedEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.NodeEditorValidated"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvEditEventHandler(object sender, TreeNodeAdvEditEventArgs e);

    /// <summary><p>Handles the <see cref="MultiColumnTreeView.NodeEditorValidateString"/>  
    /// and <see cref="MultiColumnTreeView.NodeEditorValidating"/> event.</p></summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvCancelableEditEventHandler(object sender, TreeNodeAdvCancelableEditEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.BeforeNodePaint"/> event of the MultiColumnTreeView control.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvPaintEventHandler(object sender, TreeNodeAdvPaintEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.NodeBackgroundPaint"/> event of the MultiColumnTreeView control.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvPaintBackgroundEventHandler(object sender, TreeNodeAdvPaintBackgroundEventArgs e);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.AfterCheck"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvEventHandler(object sender, TreeNodeAdvEventArgs e);

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.BeforeSelect"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="args">EventArgs that contains the event data.</param>
    public delegate void TreeNodeAdvBeforeSelectEventHandler(object sender, TreeViewAdvCancelableSelectionEventArgs args);

    /// <summary>
    /// Handles the <see cref="MultiColumnTreeView.BeforeEdit"/> event.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeViewAdvBeforeEditEventHandler(object sender, TreeNodeAdvBeforeEditEventArgs e);

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.BeforeExpand"/> event of the TreeViewAdv control.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeViewAdvNodeEventHandler(object sender, TreeViewAdvNodeEventArgs e);

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.BeforeExpand"/> event of the TreeViewAdv control.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>
    public delegate void TreeViewAdvCancelableNodeEventHandler(object sender, TreeViewAdvCancelableNodeEventArgs e);

    /// <summary>
    /// Handles the <see cref="TreeViewAdv.BeforeCheck"/> event of the TreeViewAdv control.
    /// </summary>
    /// <param name="sender">Sender Object</param>
    /// <param name="e">EventArgs that contains the event data.</param>/>
    public delegate void TreeViewAdvBeforeCheckEventHandler(object sender, TreeNodeAdvBeforeCheckEventArgs e);
}