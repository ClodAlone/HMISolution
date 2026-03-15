#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Styles;
//using Syncfusion.Windows.Forms.Tools.Controls.MultiColumnTreeView.Designers;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// The TreeNodeAdv represents a node in a <see cref="MultiColumnTreeView"/>. It contains information about the specific node like text, background style and other settings.
    /// </summary>
    /// <remarks><p>The <see cref="Nodes"/> collection holds all the child <b>TreeNodeAdv</b> objects assigned to the current 
    /// <b>TreeNodeAdv</b>. You can add, remove or clone a <b>TreeNodeAdv</b>; when doing so, all child tree 
    /// nodes are added, removed or cloned. Each <b>TreeNodeAdv</b> can contain a collection of other 
    /// <b>TreeNodeAdv</b> objects. This can make it difficult to determine where you are in the 
    /// <see cref="MultiColumnTreeView"/> when iterating through the collection. To determine your location in a tree 
    /// structure, use the <see cref="FullPath"/> property. The <b>FullPath</b> string can be parsed using the 
    /// <see cref="MultiColumnTreeView.PathSeparator"/> string value to determine where a <b>TreeNodeAdv</b> label begins and ends.
    /// </p><p>The <b>TreeNodeAdv</b> label is set by setting the <see cref="Text"/> 
    /// property explicitly. The alternative is to create the tree node using one of 
    /// the <b>TreeNodeAdv</b> constructors that has a string parameter that represents 
    /// the <see cref="Text"/> property.</p><p>You can specify images for the node using the <see cref="TreeNodeAdvStyleInfo.LeftImageIndices"/>,
    /// <see cref="TreeNodeAdvStyleInfo.OpenImgIndex"/>, <see cref="TreeNodeAdvStyleInfo.ClosedImgIndex"/>,
    /// <see cref="TreeNodeAdvStyleInfo.NoChildrenImgIndex"/> and <see cref="TreeNodeAdvStyleInfo.RightImageIndices"/> properties.
    /// </p><p>The order in which the tree node's contents are drawn is as follows:
    /// <list type="number"><item><description>Checkbox</description></item><item><description>Option Buttons</description></item><item><description>Left images</description></item><item><description>State image</description></item><item><description>Node Label</description></item><item><description>Right images</description></item></list>
    /// The "State image" will be one of <b>OpenImgIndex</b>, <b>ClosedImgIndex</b> and <b>NoChildrenImgIndex</b>.
    /// </p><p>
    /// Selecting specific tree nodes and iterating through the <see cref="Nodes"/> collection can be 
    /// achieved by using the following property values: <see cref="FirstNode"/>, 
    /// <see cref="LastNode"/>, <see cref="NextNode"/>, <see cref="PrevNode"/>, <see cref="NextVisibleNode"/>, 
    /// <see cref="PrevVisibleNode"/>. Assign the <see cref="TreeNodeAdv"/> object returned 
    /// by one of the aforementioned properties to the <see cref="MultiColumnTreeView.SelectedNode"/> property to select that 
    /// tree node in the <b>TreeViewAdv</b> control.
    /// </p><p>
    /// Tree nodes can be expanded to display the next level of child tree nodes. 
    /// The user can expand the tree node by pressing the plus (+) button next to the 
    /// TreeNodeAdv, if one is displayed or you can expand the TreeNodeAdv by calling the 
    /// <see cref="Expand"/> method. To expand all child tree node levels in the <see cref="Nodes"/> 
    /// collection, call the <see cref="ExpandAll"/> method. You can collapse the child 
    /// TreeNodeAdv level by calling the <see cref="CollapseAll"/> method or the user can 
    /// press the minus (-) button next to the TreeNodeAdv, if one is displayed. You can 
    /// also alternate the TreeNode between the expanded and collapsed states using the <see cref="Expanded"/> property.
    /// </p></remarks>
    /// <example><p>
    ///  The following example displays customer information in a <see cref="MultiColumnTreeView"/> 
    ///  control. The root tree nodes display customer names, and the child tree 
    ///  nodes display the order numbers assigned to each customer. In this 
    ///  example, 1,000 customers are displayed with 15 orders each. The 
    ///  repainting of the <b>TreeViewAdv</b> is suppressed by using the <see cref="ScrollControl.BeginUpdate()"/> 
    ///  and <see cref="ScrollControl.EndUpdate()"/> methods, and a wait Cursor is displayed while the 
    ///  <b>TreeViewAdv</b> creates and paints the <see cref="TreeNodeAdv"/> objects. This example 
    ///  assumes you have a Customer object that can hold a collection of Order 
    ///  objects. It also assumes that you have created an instance of a 
    ///  <b>TreeViewAdv</b> control on a Form.
    /// </p><code lang="C#">
    /// // Create a new ArrayList to hold the Customer objects.
    /// private ArrayList customerArray = new ArrayList(); 
    /// private void FillMyTreeView()
    /// {
    ///   // Add customers to the ArrayList of Customer objects.
    ///   for(int x=0; x!=1000; x++)
    ///   {
    ///     customerArray.Add(new Customer("Customer" + x.ToString()));
    ///   } 
    ///   // Add orders to each Customer object in the ArrayList.
    ///   foreach(Customer customer1 in customerArray)
    ///   {
    ///     for(int y=0; y!=15; y++)
    ///     {
    ///       customer1.CustomerOrders.Add(new Order("Order" + y.ToString()));    
    ///     }
    ///   }
    ///   
    ///   // Display a wait cursor while the TreeNodeAdvs are being created.
    ///   Cursor.Current = new Cursor("C:\\Cursors\\MyWait.cur");
    ///   // Clear the TreeViewAdv each time the method is called.
    ///   treeViewAdv1.Nodes.Clear();
    ///   // Add a root TreeNodeAdv for each Customer object in the ArrayList.
    ///   foreach(Customer customer2 in customerArray)
    ///   {
    ///     treeViewAdv1.Nodes.Add(new TreeNodeAdv(customer2.CustomerName));
    ///     // Add a child treenode for each Order object in the current Customer object.
    ///     foreach(Order order1 in customer2.CustomerOrders)
    ///     {
    ///       treeViewAdv1.Nodes[customerArray.IndexOf(customer2)].Nodes.Add(
    ///         new TreeNodeAdv(customer2.CustomerName + "." + order1.OrderID));
    ///     }
    ///   }
    ///   // Reset the cursor to the default for all controls.
    ///   Cursor.Current = Cursors.Default;
    /// }
    /// </code><code lang="VB">
    /// ' Create a new ArrayList to hold the Customer objects.
    /// Private customerArray As New ArrayList()
    /// Private Sub FillMyTreeView()
    ///   ' Add customers to the ArrayList of Customer objects.
    ///   Dim x As Integer
    ///   For x = 0 To 999
    ///     customerArray.Add(New Customer("Customer" + x.ToString()))
    ///   Next x
    ///   
    ///   ' Add orders to each Customer object in the ArrayList.
    ///   Dim customer1 As Customer
    ///   For Each customer1 In customerArray
    ///     Dim y As Integer
    ///     For y = 0 To 14
    ///       customer1.CustomerOrders.Add(New Order("Order" + y.ToString()))
    ///     Next y
    ///   Next customer1
    ///   
    ///   ' Display a wait cursor while the TreeNodeAdvs are being created.
    ///   Cursor.Current = New Cursor("C:\Cursors\MyWait.cur")
    ///   
    ///   ' Clear the TreeViewAdv each time the method is called.
    ///   treeViewAdv1.Nodes.Clear()
    ///   
    ///   ' Add a root TreeNodeAdv for each Customer object in the ArrayList.
    ///   Dim customer2 As Customer
    ///   For Each customer2 In customerArray
    ///     treeViewAdv1.Nodes.Add(New TreeNodeAdv(customer2.CustomerName))
    ///     
    ///     ' Add a child TreeNodeAdv for each Order object in the current Customer object.
    ///     Dim order1 As Order
    ///     For Each order1 In customer2.CustomerOrders
    ///       treeViewAdv1.Nodes(customerArray.IndexOf(customer2)).Nodes.Add( _
    ///         New TreeNodeAdv(customer2.CustomerName + "." + order1.OrderID))
    ///     Next order1
    ///   Next customer2
    ///   
    ///   ' Reset the cursor to the default for all controls.
    ///   Cursor.Current = System.Windows.Forms.Cursors.Default
    ///   
    ///   ' Begin repainting the TreeView.
    ///   treeViewAdv1.EndUpdate()
    ///   End Sub 'FillMyTreeView
    /// </code></example>
    [
    TypeConverter(typeof(TreeNodeAdvConverter)),
    Serializable()
    ]
    public class TreeNodeAdv :
      MarshalByRefObject,
      ICloneable,
      IComparable,
      ISupportInitialize,
      ISerializable
    {
        #region Class constants
        /// <summary>
        /// Inflate offset for drawing selection rectangle.
        /// </summary>
        internal const int c_nDrawTextFlags = DrawTextFormats.DT_EXPANDTABS |
          DrawTextFormats.DT_NOPREFIX;
        /// <summary>
        /// Default image index.
        /// </summary>
        private const int DefaultImageIndex = MultiColumnTreeView.DefaultImageIndex;
        /// <summary></summary>
        private const int spc = 3;
        /// <summary></summary>
        private const int c_nDisplayNodeTextWidthPadding = 4;
        /// <summary></summary>
        private const int DefaultParentIndent = 19;
        #endregion

        #region Class static members
        /// <summary></summary>
        internal static TreeNodeAdv checkStateChangingSourceNode;
        /// <summary></summary>
        private static Hashtable htRecalculatingNodes = new Hashtable();
        #endregion

        #region Class members
        /// <summary>Collection of subitems.</summary>
        private TreeNodeAdvSubItemCollection m_subitems;
        /// <summary></summary>
        private Size m_printTextSize = Size.Empty;
        /// <summary></summary>
        private object m_tag = null;
        /// <summary>
        /// CustomControl relative location.
        /// </summary>
        private int m_customControlRelativeLocation = 0;
        /// <summary></summary>
        private TreeNodePrimitivesCollection m_primitives;
        /// <summary></summary>
        private int m_visibleNodeCount = -1;
        /// <summary></summary>
        private bool m_bIsUndoRedoPerforming = false;
        /// <summary>
        /// Node custom control.
        /// </summary>
        private Control m_customControl = null;
        /// <summary></summary>
        private TreeNodeAdvStyleInfo m_nodeData = null;
        /// <summary></summary>
        private ChildTreeNodeAdvStyleInfo m_childStyle = null;
        /// <summary></summary>
        private bool m_expanded = false;
        /// <summary></summary>
        private TreeNodeAdvCollection m_nodes;
        /// <summary></summary>
        private MultiColumnTreeView m_treeView = null;
        /// <summary></summary>
        private TreeNodeAdv m_parent = null;
        /// <summary></summary>
        private TreeNodeAdvPart m_plusMinus;
        /// <summary></summary>
        private OptionButtonPart m_optionButton;
        /// <summary>X-delta reserved for Left Images drawing.</summary>
        private int m_leftImageListXRel = 0;
        /// <summary>X-delta reserved for State Images drawing. State images is painting 
        /// after Left Images.</summary>
        private int m_stateImageListXRel = 0;
        /// <summary></summary>
        private int m_rightImageListXRel = 0;
        /// <summary>X-delta reserved for horizontal anchor drawing.</summary>
        private int m_lineRightRel = 0;
        /// <summary></summary>
        private Rectangle m_bounds = Rectangle.Empty;
        /// <summary>
        /// Show plus on expand. Use only on LoadOnDemand mode.
        /// </summary>
        private bool m_bShowPlusOnExpand = false;
        /// <summary>
        /// Horizontal offset of text. 
        /// </summary>
        private int m_textLocationXRel = 0;
        /// <summary>
        /// Width of node text. 
        /// </summary>
        private int m_iTextWidth = 0;
        /// <summary>
        /// Position of node with indents in pixels. X-delta reserved for level lines drawing.
        /// </summary>
        private int m_nodeXRel = 0;
        /// <summary></summary>
        private CheckBoxPart m_checkBox;
        /// <summary> Custom visible property.</summary>
        internal bool m_innerVisible = true;
        /// <summary></summary>
        private bool m_optioned = false;
        /// <summary></summary>
        private CultureInfo m_culture = CultureInfo.CurrentCulture;
        /// <summary></summary>
        private bool m_expandedOnce = false;
        /// <summary></summary>
        private int m_width = 0;
        /// <summary></summary>
        private int m_maxX = 0;
        /// <summary></summary>
        internal TreeNodeAdvAccessibleObject m_acsoNode;
        // Caching Partial-Checked-State related
        /// <summary></summary>
        private Hashtable m_partialCheckedState = null;
        #endregion

        #region Class properties

        /// <summary>Get subitem by it order index.</summary>
        public TreeNodeAdvSubItem this[int index]
        {
            get
            {
                return this.SubItems[index];
            }
            set
            {
                this.SubItems[index] = value;
            }
        }

        /// <summary>Get collection of node subitems.</summary>
        [
        Description("Gets collection of node subitems."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Data")
        ]
        public TreeNodeAdvSubItemCollection SubItems
        {
            get
            {
                if (m_subitems == null)
                {
                    m_subitems = new TreeNodeAdvSubItemCollection(this);
                    m_subitems.CollectionChanged += new CollectionChangeEventHandler(SubItems_CollectionChanged);
                }

                return m_subitems;
            }
        }

        /// <summary>Indicate has node subitems or not.</summary>
        protected internal bool HasSubItems
        {
            get
            {
                return (m_subitems != null && m_subitems.Count > 0);
            }
        }

        /// <summary>Get collection of primitives assigned to node.</summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Data")
        ]
        public TreeNodePrimitivesCollection Primitives
        {
            get
            {
                if (m_primitives == null)
                {
                    m_primitives = new TreeNodePrimitivesCollection(this);
                }

                return m_primitives;
            }
        }

        /// <summary></summary>
        protected internal bool HasPrimitives
        {
            get
            {
                return (m_primitives != null && m_primitives.Count > 0);
            }
        }

        /// <summary>
        /// Returns the collection of <see cref="TreeNodeAdv"/> objects assigned to the 
        /// current tree node.
        /// </summary>
        /// <value>
        /// A <see cref="TreeNodeAdvCollection"/> that represents the tree nodes assigned 
        /// to the current tree node.
        /// </value>
        /// <remarks>
        /// The <see cref="Nodes"/> property can hold a collection of other <see cref="TreeNodeAdv"/> 
        /// objects. Each of the tree node in the collection has a <see cref="Nodes"/> property 
        /// that can contain its own <see cref="TreeNodeAdvCollection"/>. This nesting of 
        /// tree nodes can make it difficult to navigate a tree structure. The <see cref="FullPath"/> 
        /// property makes it easier to determine your location in a tree.
        /// </remarks>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public TreeNodeAdvCollection Nodes
        {
            get
            {
                if (m_nodes == null)
                {
                    m_nodes = new TreeNodeAdvCollection(this);
                }

                return m_nodes;
            }
            set
            {
                if (value != m_nodes)
                {
                    if (m_nodes != null)
                    {
                        m_nodes.ResetParent();
                    }

                    m_nodes = value;

                    if (m_nodes != null)
                    {
                        m_nodes.SetParent(this);
                    }
                }
            }
        }
        /// <summary></summary>
        [
        Browsable(false)
        ]
        public bool HasNodes
        {
            get
            {
                return (m_nodes != null && m_nodes.Count > 0);
            }
        }

        /// <summary>
        /// Gets or sets node custom control.
        /// </summary>
        [
        Editor(typeof(CustomControlEditor), typeof(UITypeEditor)),
        DefaultValue(null),
        Description("Gets or sets node custom control."),
        Category("Data")
        ]
        public Control CustomControl
        {
            get
            {
                return m_customControl;
            }
            set
            {
                if (m_customControl != value)
                {
                    m_customControl = value;

                    if (this.TreeView != null)
                    {
                        CustomControlCollectionChanging(this, CollectionChangeAction.Refresh);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the horizontal padding used between the different parts of the tree node.
        /// </summary>
        public static int PartsPadX
        {
            get
            {
                return spc;
            }
        }
        /// <summary>
        /// Gets / sets the font of the node.
        /// </summary>
        [Description("The font of the node."), Category("Appearance"), Localizable(true)]

        public Font Font
        {
            get
            {
                return this.CurrentStyleInfo.Font;
            }
            set
            {
                this.CurrentStyleInfo.Font = value;
                this.RecalculateDimensions();
            }
        }

        /// <summary>
        /// Gets / sets the color of the text.
        /// </summary>
        [Description("The Color of the text."), Category("Appearance")]

        public Color TextColor
        {
            get
            {
                return this.CurrentStyleInfo.TextColor;
            }
            set
            {
                this.CurrentStyleInfo.TextColor = value;
            }
        }

        /// <summary>
        /// Gets / sets the background of the node.
        /// </summary>
        [
        Description("The background of the node."),
        Category("Appearance")
        ]
        public BrushInfo Background
        {
            get
            {
                return this.CurrentStyleInfo.Background;
            }
            set
            {
                this.CurrentStyleInfo.Background = value;
            }
        }

        /// <summary>
        /// Gets / sets the text of the node.
        /// </summary>
        [
        Description("The text of the node."), Category("Appearance"), Localizable(true),
        Editor(typeof(Syncfusion.Windows.Forms.Tools.Controls.MultiColumnTreeView.Designers.MultilineStringEditor), typeof(UITypeEditor))
        ]
        public string Text
        {
            get
            {
                return this.CurrentStyleInfo.Text;
            }
            set
            {
                this.CurrentStyleInfo.Text = value;
            }
        }

        /// <summary>
        /// Gets / sets the help text of the node.
        /// </summary>
        [
        Description("The help text of the node."),
        Category("Appearance"),
        Localizable(true)
        ]
        public string HelpText
        {
            get
            {
                return this.CurrentStyleInfo.HelpText;
            }
            set
            {
                this.CurrentStyleInfo.HelpText = value;
            }
        }

        /// <summary>
        /// Gets / sets is node text should be drawn as multiline text or single line.
        /// </summary>
        [
        Description("Gets / sets is node text should be drawn as multiline text or single line."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(false)
        ]
        public bool Multiline
        {
            get
            {
                return this.NodeStyle.Multiline;
            }
            set
            {
                this.NodeStyle.Multiline = value;
            }
        }

        /// <summary>
        /// Gets / sets the height of the node.
        /// </summary>
        [
        Description("The height of the node."),
        Category("Appearance")
        ]
        public int Height
        {
            get
            {
                return this.NodeStyle.Height;
            }
            set
            {
                this.NodeStyle.Height = value;
            }
        }

        /// <summary>
        /// Indicates the color of the Check mark.
        /// </summary>
        [
        DefaultValue(typeof(Color), "ControlText"),
        Category("Appearance"),
        Description("Indicates the color of the Check mark.")
        ]
        public Color CheckColor
        {
            get
            {
                return this.NodeStyle.CheckColor;
            }
            set
            {
                if (value != this.NodeStyle.CheckColor)
                {
                    this.NodeStyle.CheckColor = value;
                }
            }
        }

        /// <summary>
        /// Indicates the color of the check mark when it is in intermediate state.
        /// </summary>
        [
        DefaultValue(typeof(Color), "ControlDark"),
        Category("Appearance"),
        Description("Indicates the color of the check mark when it is in intermediate state.")
        ]
        public Color IntermediateCheckColor
        {
            get
            {
                return this.NodeStyle.IntermediateCheckColor;
            }
            set
            {
                if (value != this.NodeStyle.IntermediateCheckColor)
                {
                    this.NodeStyle.IntermediateCheckColor = value;
                }
            }
        }
        /// <summary>
        /// Indicates the appearance of checkbox background.
        /// </summary>
        [
        Browsable(false),
        DefaultValue(typeof(Brush), "Window"),
        Description("Indicates the appearance of checkbox background.")
        ]
        public Brush CheckBoxBackGround
        {
            get
            {
                return this.NodeStyle.CheckBoxBackground;
            }
            set
            {
                if (value != this.NodeStyle.CheckBoxBackground)
                {
                    this.NodeStyle.CheckBoxBackground = value;
                }
            }
        }

        /// <summary>
        /// Indicates the appearance of checkbox background when the checkbox is in intermediate state.
        /// </summary>
        [
        Browsable(false),
        DefaultValue(typeof(Brush), "Control"),
        Description(" Indicates the appearance of checkbox background when the checkbox is in intermediate state.")
        ]
        public Brush IntermediateCheckBoxBackGround
        {
            get
            {
                return this.NodeStyle.IntermediateCheckBoxBackground;
            }
            set
            {
                if (value != this.NodeStyle.IntermediateCheckBoxBackground)
                {
                    this.NodeStyle.IntermediateCheckBoxBackground = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the checkbox of the node is visible.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates if the checkbox of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowCheckBox
        {
            get
            {
                return this.NodeStyle.ShowCheckBox;
            }
            set
            {
                this.NodeStyle.ShowCheckBox = value;
                RecalculateDimensions();
            }
        }

        /// <summary>
        /// Gets or sets show plus on expand. Use only on LoadOnDemand mode.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates if the plus on expand of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowPlusOnExpand
        {
            get
            {
                return m_bShowPlusOnExpand;
            }
            set
            {
                m_bShowPlusOnExpand = value;
            }
        }

        /// <summary>
        /// Indicates whether the option button of the node is visible.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates if the optionbutton of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowOptionButton
        {
            get
            {
                return this.NodeStyle.ShowOptionButton;
            }
            set
            {
                this.NodeStyle.ShowOptionButton = value;
                RecalculateDimensions();
            }
        }

        /// <summary>
        /// Indicates whether the plus/minus of the node is visible.
        /// </summary>
        [
        Description("Indicates if the plus/minus of the node is visible."),
        Category("Appearance")
        ]
        public bool ShowPlusMinus
        {
            get
            {
                return this.NodeStyle.ShowPlusMinus;
            }
            set
            {
                this.NodeStyle.ShowPlusMinus = value;
            }
        }

        /// <summary>
        /// Indicates the color of the Option button.
        /// </summary>
        [
        DefaultValue(typeof(Color), "White"),
        Category("Appearance"),
        Description("Indicates the color of the Option button.")
        ]
        public Color OptionButtonColor
        {
            get
            {
                return this.NodeStyle.OptionButtonColor;
            }
            set
            {
                if (value != this.NodeStyle.OptionButtonColor)
                {
                    this.NodeStyle.OptionButtonColor = value;
                }
            }
        }

        /// <summary>
        /// Indicates the color of the Option button in selected state.
        /// </summary>
        [
        DefaultValue(typeof(Color), "Black"),
        Category("Appearance"),
        Description("Indicates the color of the Option button in selected state.")
        ]
        public Color SelectedOptionButtonColor
        {
            get
            {
                return this.NodeStyle.SelectedOptionButtonColor;
            }
            set
            {
                if (value != this.NodeStyle.SelectedOptionButtonColor)
                {
                    this.NodeStyle.SelectedOptionButtonColor = value;
                }
            }
        }

        /// <summary>
        /// Gets / sets the sort order of the node.
        /// </summary>
        [
        Description("Indicates the sort order of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public SortOrder SortOrder
        {
            get
            {
                return this.NodeStyle.SortOrder;
            }
            set
            {
                this.NodeStyle.SortOrder = value;
            }
        }

        /// <summary>
        /// Gets / sets the sort type of the node.
        /// </summary>
        [
        Description("Indicates the sort type of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public TreeNodeAdvSortType SortType
        {
            get
            {
                return this.NodeStyle.SortType;
            }
            set
            {
                this.NodeStyle.SortType = value;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="IComparer"/> object that compares two nodes.
        /// </summary>
        [
        Description("Indicates the IComparer object that compares two nodes."),
        Category("Sorting")
        ]
        public IComparer Comparer
        {
            get
            {
                return this.NodeStyle.Comparer;
            }
            set
            {
                this.NodeStyle.Comparer = value;
            }
        }

        /// <summary>
        /// Gets / sets the compare options used in the sorting of the node.
        /// </summary>
        [
        Description("Indicates the compare options used in the sorting of the node."),
        Category("Sorting"),
        Localizable(true)
        ]
        public CompareOptions CompareOptions
        {
            get
            {
                return this.NodeStyle.CompareOptions;
            }
            set
            {
                this.NodeStyle.CompareOptions = value;
            }
        }

        /// <summary>
        /// Gets / sets the CheckState of the node.
        /// </summary>
        /// <remarks><p>Note that setting this property will fire the <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.BeforeCheck"/>
        /// event. If you do not want this event to be fired, you can access the tree's 
        /// internal data structure as follows:</p><code lang="C#">
        /// treeNodeAdv.NodeStyle.CheckState = CheckState.Checked;
        /// </code><code lang="VB">
        /// treeNodeAdv.NodeStyle.CheckState = CheckState.Checked
        /// </code></remarks>
        [
        Description("Indicates the checkState of the node."),
        Category("Appearance")
        ]
        public CheckState CheckState
        {
            get
            {
                return this.NodeStyle.CheckState;
            }
            set
            {
                if (this.CheckState != value)
                {
                    if (TreeNodeAdv.checkStateChangingSourceNode == null)
                    {
                        TreeNodeAdv.checkStateChangingSourceNode = this;
                    }

                    try
                    {
                        // Let the user cancel this setting:
                        MultiColumnTreeView tree = this.TreeView;
                        if (tree != null)
                        {
                            TreeNodeAdvBeforeCheckEventArgs args =
                              new TreeNodeAdvBeforeCheckEventArgs(this, false, value);
                            tree.OnBeforeCheck(args);
                            if (args.Cancel)
                            {
                                return;
                            }
                        }

                        // Caching Partial-Checked-State related
                        // Cache the state if changing from partial checked state.
                        if (this.InteractiveCheckBox && this.CheckState == CheckState.Indeterminate)
                        {
                            this.CachePartialCheckedState();
                        }

                        this.NodeStyle.CheckState = value;

                        if (tree != null)
                        {
                            tree.OnAfterCheck(new TreeNodeAdvEventArgs(this, TreeViewAdvAction.Unknown));

                            if (TreeNodeAdv.checkStateChangingSourceNode == this)
                            {
                                tree.OnAfterInteractiveChecks(new TreeNodeAdvEventArgs(this));
                            }
                        }
                    }
                    finally
                    {
                        if (TreeNodeAdv.checkStateChangingSourceNode == this)
                        {
                            TreeNodeAdv.checkStateChangingSourceNode = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets / sets the base style for the node from which to inherit.
        /// </summary>
        /// <remarks>The specified base style should be available in the <see cref="MultiColumnTreeView.BaseStyles"/>
        /// collection.</remarks>
        [
        Description("The base style for the node"),
        Category("Appearance - Inherited"),
        Editor(typeof(BaseStyleSelectorUITypeEditor), typeof(UITypeEditor))
        ]
        public string BaseStyle
        {
            get
            {
                return this.CurrentStyleInfo.BaseStyle;
            }
            set
            {
                this.CurrentStyleInfo.BaseStyle = value;
            }
        }

        /// <summary>
        /// Gets / sets the object that contains data about the tree node.
        /// </summary>
        /// <value>
        /// An <see cref="System.Object"/> that contains data about the tree node. The default is a null reference (Nothing in Visual Basic).
        /// </value>
        /// <remarks><p>Any Object derived type may be assigned to this property. If this property is 
        /// being set through the Windows Forms designer, only text may be assigned.</p><p>When the tree node is cloned, if this object is cloneable (implements ICloneable
        /// interface) then it will be.</p></remarks>
        [
        Description("The tag of the node"),
        Category("Data"),
        TypeConverter(typeof(StringConverter))
        ]
        public object Tag
        {
            get
            {
                return this.CurrentStyleInfo.Tag;
            }
            set
            {
                this.CurrentStyleInfo.Tag = value;
            }
        }

        /// <summary>
        /// Gets / sets the image index indicating the image in the StateImageList where the node has no children.
        /// </summary>
        [
        Description("The imageindex indicating the image in the StateImageList where the node has no children."),
        Category("Appearance - Images"),
        Localizable(true),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor))
        ]
        public int NoChildrenImgIndex
        {
            get
            {
                return this.NodeStyle.NoChildrenImgIndex;
            }
            set
            {
                this.NodeStyle.NoChildrenImgIndex = value;
            }
        }

        /// <summary>
        /// Gets / sets the image index in the StateImageList where the node is not expanded.
        /// </summary>
        [
        Description("Indicates the imageindex in the StateImageList where the node is not expanded."),
        Category("Appearance - Images"),
        Localizable(true),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor))
        ]
        public int ClosedImgIndex
        {
            get
            {
                return this.NodeStyle.ClosedImgIndex;
            }
            set
            {
                this.NodeStyle.ClosedImgIndex = value;
            }
        }

        /// <summary>
        /// Gets / sets the image index in the StateImageList where the node is expanded.
        /// </summary>
        [
        Description("Indicates the imageindex in the StateImageList where the node is expanded."),
        Category("Appearance - Images"),
        Localizable(true),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor))
        ]
        public int OpenImgIndex
        {
            get
            {
                return this.NodeStyle.OpenImgIndex;
            }
            set
            {
                this.NodeStyle.OpenImgIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets image index of image for expand button.
        /// </summary>
        [
        Description("Image index of image for expand button."),
        Category("Appearance - Images"),
        Editor(typeof(NodeStateImageListUITypeEditor), typeof(UITypeEditor))
        ]
        public int ExpandImageIndex
        {
            get
            {
                return this.NodeStyle.ExpandImageIndex;
            }
            set
            {
                this.NodeStyle.ExpandImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets image index of image for collapse button.
        /// </summary>
        [
        Description("Image index of image for collapse button"),
        Category("Appearance - Images"),
        Editor(typeof(NodeStateImageListUITypeEditor), typeof(UITypeEditor))
        ]
        public int CollapseImageIndex
        {
            get
            {
                return this.NodeStyle.CollapseImageIndex;
            }
            set
            {
                this.NodeStyle.CollapseImageIndex = value;
            }
        }

        /// <summary>
        /// Gets / sets the image indices of the images to be drawn on the left of the node's text.
        /// </summary>
        [
        Description("The imageindex to be drawn on the left of the node's text."),
        Category("Appearance - Images")
        ]
        public int[] LeftImageIndices
        {
            get
            {
                return this.CurrentStyleInfo.LeftImageIndices;
            }
            set
            {
                this.CurrentStyleInfo.LeftImageIndices = value;
            }
        }

        /// <summary>
        /// Gets / sets the image indices of the images to be drawn on the right of the node's text.
        /// </summary>
        [
        Description("The imageindex to be drawn on the right of the node's text."),
        Category("Appearance - Images"),
        Localizable(true)
        ]
        public int[] RightImageIndices
        {
            get
            {
                return this.CurrentStyleInfo.RightImageIndices;
            }
            set
            {
                this.CurrentStyleInfo.RightImageIndices = value;
            }
        }

        /// <summary>
        /// Gets / sets left image for node
        /// </summary>
        [
        Description("Gets / sets the image that will be drawn on the left of the node`s text."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image LeftImage
        {
            get
            {
                return this.CurrentStyleInfo.LeftImage;
            }
            set
            {
                this.CurrentStyleInfo.LeftImage = value;
            }
        }

        /// <summary>
        /// Gets / sets right image for node
        /// </summary>
        [
        Description("Gets / sets the image that will be drawn on the right of the node`s text."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image RightImage
        {
            get
            {
                return this.CurrentStyleInfo.RightImage;
            }
            set
            {
                this.CurrentStyleInfo.RightImage = value;
            }
        }

        /// <summary>
        /// Gets / sets open state image for node.
        /// </summary>
        [
        Description("Gets / sets the image that will be shown where the node is expanded."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image OpenImage
        {
            get
            {
                return this.NodeStyle.OpenImage;
            }
            set
            {
                this.NodeStyle.OpenImage = value;
            }
        }

        /// <summary>
        /// Gets / sets close state image for node.
        /// </summary>
        [
        Description("Gets / sets the image that will be shown where the node is collapsed."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image ClosedImage
        {
            get
            {
                return this.NodeStyle.ClosedImage;
            }
            set
            {
                this.NodeStyle.ClosedImage = value;
            }
        }

        /// <summary>
        /// Gets / sets image for node that has no children.
        /// </summary>
        [
        Description("Gets / sets the image that will be shown where the node has no children."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image NoChildrenImage
        {
            get
            {
                return this.NodeStyle.NoChildrenImage;
            }
            set
            {
                this.NodeStyle.NoChildrenImage = value;
            }
        }

        /// <summary>
        /// Gets / sets image for state button of expanded node.
        /// </summary>
        [
        Description("Gets / sets the image for state button where the node is expanded."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image ExpandedImage
        {
            get
            {
                return this.NodeStyle.ExpandedImage;
            }
            set
            {
                this.NodeStyle.ExpandedImage = value;
            }
        }

        /// <summary>
        /// Gets / sets image for state button of collapsed node.
        /// </summary>
        [
        Description("Gets / sets the image for state button where the node is collapsed."),
        Category("Appearance - Images"),
        DefaultValue(null)
        ]
        public Image CollapsedImage
        {
            get
            {
                return this.NodeStyle.CollapsedImage;
            }
            set
            {
                this.NodeStyle.CollapsedImage = value;
            }
        }

        /// <summary>
        /// Gets / sets the space between images for LeftImageList.
        /// </summary>
        [
        Description("Gets / sets the padding of left image for the node."),
        Category("Appearance - Images"),
        DefaultValue(0)
        ]
        public int LeftImagePadding
        {
            get
            {
                return this.CurrentStyleInfo.LeftImagePadding;
            }
            set
            {
                this.CurrentStyleInfo.LeftImagePadding = value;
            }
        }

        /// <summary>
        /// Gets / sets the space between images for RightImageList.
        /// </summary>
        [
        Description("Gets / sets the padding of right image for the node."),
        Category("Appearance - Images"),
        DefaultValue(0)
        ]
        public int RightImagePadding
        {
            get
            {
                return this.CurrentStyleInfo.RightImagePadding;
            }
            set
            {
                this.CurrentStyleInfo.RightImagePadding = value;
            }
        }

        /// <summary>
        /// Gets / sets the space before StateImage.
        /// </summary>
        [
        Description("Gets / sets the left side padding of state image for the node."),
        Category("Appearance - Images"),
        DefaultValue(0)
        ]
        public int LeftStateImagePadding
        {
            get
            {
                return this.NodeStyle.LeftStateImagePadding;
            }
            set
            {
                this.NodeStyle.LeftStateImagePadding = value;
            }
        }

        /// <summary>
        /// Gets / sets the space after StateImage.
        /// </summary>
        [
        Description("Gets / sets the right side padding of state image for the node."),
        Category("Appearance - Images"),
        DefaultValue(0)
        ]
        public int RightStateImagePadding
        {
            get
            {
                return this.NodeStyle.RightStateImagePadding;
            }
            set
            {
                this.NodeStyle.RightStateImagePadding = value;
            }
        }

        /// <summary>
        /// Indicates whether the node's controls will be themed.
        /// </summary>
        [
        Description("Indicates if the node's controls will be themed."),
        Category("Appearance")
        ]
        public bool ThemesEnabled
        {
            get
            {
                return this.NodeStyle.ThemesEnabled;
            }
            set
            {
                this.NodeStyle.ThemesEnabled = value;
            }
        }

        /// <summary>
        /// Indicates whether the node will have an interactive checkbox.
        /// </summary>
        [
        Description("Indicates if the node will have an interactive checkbox."),
        Category("Behavior")
        ]
        public bool InteractiveCheckBox
        {
            get
            {
                return this.NodeStyle.InteractiveCheckBox;
            }
            set
            {
                this.NodeStyle.InteractiveCheckBox = value;
            }
        }

        /// <summary>
        /// Indicates whether the node has been expanded at least once.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Indicates if the node has been expanded at least once."),
        DefaultValue(false)
        ]
        public bool ExpandedOnce
        {
            get
            {
                return m_expandedOnce;
            }
            set
            {
                if (m_expandedOnce != value)
                {
                    m_expandedOnce = value;
                    MultiColumnTreeView tree = this.TreeView;
                    if (tree != null && tree.LoadOnDemand)
                    {
                        UpdatePlusMinusVisibility();
                    }
                }
            }
        }

        /// <summary>
        /// Gets / sets the culture of the node.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Localizable(true)
        ]
        public CultureInfo Culture
        {
            get
            {
                return this.NodeStyle.Culture;
                ;
            }
            set
            {
                this.NodeStyle.Culture = value;
            }
        }

        /// <summary>
        /// Indicates whether the node is in editing state.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public bool IsEditing
        {
            get
            {
                return (TreeView.SelectedNode == this) && (TreeView.IsEditing);
            }
        }

        /// <summary>
        /// Indicates whether the node is selected.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public bool IsSelected
        {
            get
            {
                if (this.TreeView != null)
                {
                    return TreeView.SelectedNodes.Contains(this);
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Indicates whether the node is the currently active node.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public bool IsActiveNode
        {
            get
            {
                if (this.TreeView != null)
                {
                    return TreeView.ActiveNode == this;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's drawing bounds.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int NodeX
        {
            get
            {
                int nX = 0;

                if (GetIsMirrored())
                {
                    nX = this.Bounds.Right - m_nodeXRel;
                }
                else
                {
                    nX = this.Bounds.X + m_nodeXRel;
                }

                return nX;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's left images.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int LeftImagesX
        {
            get
            {
                return this.Bounds.X + m_nodeXRel + m_leftImageListXRel;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's state image.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int StateImageX
        {
            get
            {
                return this.Bounds.X + m_nodeXRel + m_stateImageListXRel;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's right images.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int RightImagesX
        {
            get
            {
                return this.Bounds.X + m_nodeXRel + m_rightImageListXRel;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's checkbox.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int CheckBoxX
        {
            get
            {
                return m_checkBox.Bounds.X;
            }
        }

        /// <summary>
        /// Returns the horizontal distance between the tree border and the beginning
        /// of the node's option button.
        /// </summary>
        /// <remarks>This property returns a valid value only when queried from
        /// an owner draw paint event like <see cref="MultiColumnTreeView.AfterNodePaint"/>.</remarks>
        [
        Browsable(false)
        ]
        public int OptionButtonX
        {
            get
            {
                return m_optionButton.Bounds.X;
            }
        }

        /// <summary>
        /// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the checkbox of a tree node.
        /// </summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvPart CheckBox
        {
            get
            {
                return m_checkBox;
            }
        }

        /// <summary>
        /// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the option button part of a tree node.
        /// </summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvPart OptionButton
        {
            get
            {
                return m_optionButton;
            }
        }

        /// <summary>
        /// Returns a <see cref="TreeNodeAdvPart"/> corresponding to the plus-minus part of a tree node.
        /// </summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvPart PlusMinus
        {
            get
            {
                return m_plusMinus;
            }
        }

        /// <summary>
        /// Indicates whether the node's checkbox is checked.
        /// </summary>
        [
        Description("Indicates if the node's checkbox is checked."),
        Category("Behavior"),
        DefaultValue(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Checked
        {
            get
            {
                return (this.CheckState == CheckState.Checked);
            }
            set
            {
                this.CheckState = (value ? CheckState.Checked : CheckState.Unchecked);
            }
        }

        /// <summary>
        /// Indicates whether the node is enabled.
        /// </summary>
        [
        Description("Specifies if the node is enabled."),
        Category("Appearance"),
        Localizable(true)
        ]
        public bool Enabled
        {
            get
            {
                return NodeStyle.Enabled;
            }
            set
            {
                NodeStyle.Enabled = value;
            }
        }

        /// <summary>Show/hide tree node from view.</summary>
        [
        Description("Specifies is node will be visible to user or not."),
        Category("Appearance"),
        Localizable(true),
        DefaultValue(true),
        Editor(typeof(VisibleEditor), typeof(UITypeEditor))
        ]
        public bool Visible
        {
            get
            {
                return m_innerVisible;
            }
            set
            {
                bool oldVisibility = Visible;

                // only if we have changes run our internal logic
                if (value != oldVisibility)
                {
                    if (this.TreeView != null)
                    {
                        this.TreeView.NeedUpdateCustomControls = true;
                    }

                    // increase or decriese visible nodes counter
                    if ((oldVisibility && !value))
                    {
                        AdjustVisibleNodeCount(-1);
                    }
                    else if (!oldVisibility && value)
                    {
                        AdjustVisibleNodeCount(1);
                    }

                    // change visibility
                    m_innerVisible = value;
                    this.TreeView.Update();                   
                    }     
                    if (!value)
                    {
                       this.TreeView.ValidateScrollPosition();
                    }
                }       
           }

        /// <summary>
        /// Indicates whether the buttons in the node are enabled.
        /// </summary>
        /// <value>True to enable the buttons; False otherwise.</value>
        /// <remarks>The checkbox and option buttons can be disabled keeping the rest of the node enabled
        /// using this property.</remarks>
        [
        Description("Specifies if the buttons in the node are enabled."),
        Category("Appearance"),
        Localizable(true)
        ]
        public bool EnabledButtons
        {
            get
            {
                return NodeStyle.EnabledButtons;
            }
            set
            {
                NodeStyle.EnabledButtons = value;
            }
        }

        /// <summary>
        /// Indicates whether the first child should be marked as <see cref="Optioned"/> and this node's <see cref="OptionedChild"/> if none of the other children is Optioned in a parent node.
        /// </summary>
        /// <value>True to ensure a default optioned child; False otherwise.</value>
        [
        Description("Specifies if atleast one child of the parent node should be Optioned at all times."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool EnsureDefaultOptionedChild
        {
            get
            {
                return NodeStyle.EnsureDefaultOptionedChild;
            }
            set
            {
                NodeStyle.EnsureDefaultOptionedChild = value;
            }
        }

        /// <summary>
        /// Indicates whether the node's option button is checked.
        /// </summary>
        [
        Description("Indicates if the node's option button is checked."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public bool Optioned
        {
            get
            {
                return m_optioned;
            }
            set
            {
                if (m_optioned != value)
                {
                    m_optioned = value;

                    OnCheckStateChanged(EventArgs.Empty);

                    MultiColumnTreeView tree = this.TreeView;

                    if (tree != null)
                    {
                        // Calling Invalidate fixes defect 345  - Clicking Radio/Option buttons fail to cause a Refresh
                        if (tree.IsHandleCreated && tree.Visible)
                        {
                            tree.Invalidate(this.RowBounds);
                        }

                        tree.OnAfterCheck(new TreeNodeAdvEventArgs(this));
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the node has child nodes.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Please replace all calls on HasNodes property calls.", true)]

        public bool HasChildren
        {
            get
            {
                return this.HasNodes;
            }
        }

        /// <summary>Return absolute order index of tree node.</summary>
        /// <remarks>0 (zero) - mean first node in tree.</remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public int TreeRowIndex
        {
            get
            {
                return (this.ParentNode != null) ? this.ParentNode.GetTreeRowIndexOfChild(this) : 0;
            }
        }

        /// <summary>
        /// Returns the first child tree node in the tree node collection.
        /// </summary>
        /// <value>The first child TreeNodeAdv in the <see cref="Nodes"/> collection.</value>
        /// <remarks>
        /// The <b>FirstNode</b> is the first child TreeNodeAdv in the 
        /// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="Nodes"/> property of 
        /// the current tree node. If the <see cref="TreeNode"/> has no child tree node, the 
        /// <b>FirstNode</b> property returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv FirstNode
        {
            get
            {
                if (this.HasNodes)
                {
                    return this.Nodes[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the last child tree node in the tree node collection.
        /// </summary>
        /// <value>The last child TreeNodeAdv in the <see cref="Nodes"/> collection.</value>
        /// <remarks>
        /// The <b>LastNode</b> is the last child TreeNodeAdv in the 
        /// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="Nodes"/> property of 
        /// the current tree node. If the <see cref="TreeNodeAdv"/> has no child tree node, the 
        /// <b>LastNode</b> property returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv LastNode
        {
            get
            {
                if (this.HasNodes)
                {
                    return this.Nodes[this.Nodes.Count - 1];
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the last visible child tree node in the tree node collection.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv LastVisibleNode
        {
            get
            {
                if (this.HasNodes)
                {
                    for (int i = this.Nodes.Count - 1; i >= 0; i--)
                    {
                        if (this.Nodes[i].Visible)
                        {
                            return this.Nodes[i];
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the previous sibling tree node.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the previous sibling tree node.</value>
        /// <remarks>
        /// The <b>PrevNode</b> is the previous sibling <see cref="TreeNodeAdv"/> in the 
        /// <see cref="TreeNodeAdvCollection"/> stored in the <see cref="MultiColumnTreeView.Nodes"/> 
        /// property of the tree node's parent <b>TreeNodeAdv</b>. If there is no previous 
        /// tree node, the <b>PrevNode</b> property returns a null reference (Nothing in 
        /// Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv PrevNode
        {
            get
            {
                if (this.ParentNode == null)
                {
                    return null;
                }

                if (this.Index > 0)
                {
                    return this.ParentNode.Nodes[this.Index - 1];
                }
                
                if (this.Index == 0)
                {
                        return this.ParentNode;
                }

                return null;
            }
        }

        /// <summary></summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv PrevSiblingNode
        {
            get
            {
                TreeNodeAdv node1 = this.ParentNode;
                if (node1 == null)
                {
                    return null;
                }

                if (this.Index >0)
                {
                    TreeNodeAdv node2 = this.ParentNode.Nodes[this.Index - 1];

                    while (node2.HasNodes)
                    {
                        node2 = node2.LastNode;
                    }


                    return node2;
                }

                if (this.Index == 0)
                {
                        return null;
                }

                return node1;
            }
        }

        /// Returns the previous visible tree node.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the previous 
        /// visible tree node.</value>
        /// <remarks>
        /// The <b>PrevVisibleNode</b> can be a child, sibling or a tree node from 
        /// another branch. If there is no previous tree node, the <b>PrevVisibleNode</b> 
        /// property returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv PrevVisibleNode
        {
            get
            {
                TreeNodeAdv node = this;

                while (node != null)
                {
                    TreeNodeAdv nodeParent = node.ParentNode;

                    if (nodeParent == null)
                    {
                        break;
                    }

                    for (int i = node.Index - 1; i >= 0; i--)
                    {
                        TreeNodeAdv nodePrevious = nodeParent.Nodes[i];

                        // If previous node has nodes, so try to find previous node there.
                        if (nodePrevious.Visible && nodePrevious.HasNodes && nodePrevious.Expanded)
                        {
                            for (int j = nodePrevious.Nodes.Count - 1; j >= 0; j--)
                            {
                                TreeNodeAdv nodeChild = nodePrevious.Nodes[j];

                                if (nodeChild.Visible)
                                {
                                    return nodeChild;
                                }
                            }
                        }
                        // If previous node contains no nodes, return its if it's visible.
                        else if (nodePrevious.Visible)
                        {
                            return nodePrevious;
                        }
                    }

                    // If no nodes before, return Parent node.
                    if (nodeParent.Visible)
                    {
                        return nodeParent;
                    }

                    node = nodeParent;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the previous selectable tree node.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the previous 
        /// selectable tree node.</value>
        /// <remarks>
        /// The <b>PrevSelectableNode</b> can be a child, sibling or a tree node from 
        /// another branch. If there is no previous tree node, the <b>PrevSelectableNode</b> 
        /// property returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv PrevSelectableNode
        {
            get
            {
                TreeNodeAdv prev = this.PrevVisibleNode;

                while (prev != null && prev.Enabled == false)
                {
                    prev = prev.PrevVisibleNode;
                }

                return prev;
            }
        }

        /// <summary>
        /// Returns the next sibling tree node.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the next sibling tree node.</value>
        /// <remarks>
        /// The <b>NextNode</b> is the next sibling <b>TreeNodeAdv</b> in the 
        /// <see cref="TreeNodeCollection"/> stored in the <see cref="TreeNodeAdv.Nodes"/> 
        /// property of the tree node's parent <b>TreeNodeAdv</b>. If there is no next 
        /// tree node, the <b>NextNode</b> property returns a null reference (Nothing in 
        /// Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv NextNode
        {
            get
            {
                if (this.ParentNode == null)
                {
                    return null;
                }

                if (this.Index < ParentNode.Nodes.Count - 1)
                {
                    return this.ParentNode.Nodes[this.Index + 1];
                }

                if (this.Index > 0)
                {
                    return this.ParentNode.NextNode;
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv NextSiblingNode
        {
            get
            {
                if (this.HasNodes)
                {
                    return this.Nodes[0];
                }
                else if (this.ParentNode != null && this.Index == this.ParentNode.Nodes.Count - 1)
                {
                    TreeNodeAdv node = this.ParentNode;
                    while (node != null && node.NextNode == null)
                    {
                        node = node.ParentNode;
                    }
                    if (node != null)
                    {
                        foreach (TreeNodeAdv subNode in node.Nodes)
                        {
                            if (this.Index <= ParentNode.Nodes.Count - 1)
                            {
                                node = null;
                            }
                        }
                    }

                    // is we reach end of tree then return NULL
                    return (node == null) ? null : node.NextNode;
                    
                }

               return this.NextNode;
            }
        }

        /// <summary>
        /// Returns the next visible tree node.
        /// </summary>
        /// <value>
        /// A TreeNodeAdv that represents the next visible tree node.
        /// </value>
        /// <remarks>
        /// The <b>NextVisibleNode</b> can be a child, sibling or a tree node from 
        /// another branch. If there is no next tree node, the <b>NextVisibleNode</b> property 
        /// returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv NextVisibleNode
        {
            get
            {
                // try to get child
                if (this.HasNodes && this.Expanded)
                {
                    for (int i = 0, len = this.Nodes.Count; i < len; i++)
                    {
                        TreeNodeAdv nodeChild = this.Nodes[i];
                        if (nodeChild.Visible)
                        {
                            return nodeChild;
                        }
                    }
                }

                // if no visible child found then try to find visible parent node
                TreeNodeAdv node = this;
                while (node != null)
                {
                    TreeNodeAdv parent = node.ParentNode;
                    if (parent == null)
                    {
                        break;
                    }

                    for (int i = node.Index + 1, len = parent.Nodes.Count; i < len; i++)
                    {
                        TreeNodeAdv parNode = parent.Nodes[i];
                        if (parNode.Visible)
                        {
                            return parNode;
                        }
                    }

                    node = parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the next selectable tree node.
        /// </summary>
        /// <value>
        /// A TreeNodeAdv that represents the next selectable tree node.
        /// </value>
        /// <remarks>
        /// The <b>NextSelectableNode</b> can be a child, sibling or a tree node from 
        /// another branch. If there is no next tree node, the <b>NextSelectableNode</b> property 
        /// returns a null reference (Nothing in Visual Basic).
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv NextSelectableNode
        {
            get
            {
                TreeNodeAdv next = this.NextVisibleNode;

                while (next != null && next.Enabled == false)
                {
                    next = next.NextVisibleNode;
                }

                return next;
            }
        }

        /// <summary>
        /// Returns the child node who's option button is checked.
        /// </summary>
        /// <value>
        /// A TreeNodeAdv that represents the next visible tree node.
        /// </value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv OptionedChild
        {
            get
            {
                if (this.HasNodes)
                {
                    for (int i = 0, len = this.Nodes.Count; i < len; i++)
                    {
                        if (this.Nodes[i].Optioned)
                        {
                            return this.Nodes[i];
                        }
                    }

                    if (this.EnsureDefaultOptionedChild)
                    {
                        TreeNodeAdv node = this.Nodes[0];
                        node.m_optioned = true;

                        if (this.TreeView != null && node.OptionButton.Visible)
                        {
                            this.TreeView.Invalidate(node.OptionButton.Bounds);
                        }

                        return node;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the bounds of the tree node.
        /// </summary>
        /// <value>
        /// The <see cref="System.Drawing.Rectangle"/> that represents the bounds of the tree node.
        /// </value>
        /// <remarks>
        /// The coordinates are relative to the upper left corner of the <see cref="MultiColumnTreeView"/> control.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Rectangle Bounds
        {
            get
            {
                return m_bounds;
            }
        }

        /// <summary></summary>
        [
        DocumentationExclude(),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public Rectangle DragCueBounds
        {
            get
            {
                if (this.TreeView != null && this.TreeView.StateImageList != null)
                {
                    Rectangle nodeBounds = this.Bounds;

                    int left = nodeBounds.X + m_nodeXRel + m_stateImageListXRel;
                    int right = this.TextBounds.Right;
                    return new Rectangle(left, nodeBounds.Y, right - left, nodeBounds.Height);
                }
                else
                {
                    return this.TextBounds;
                }
            }
        }

        /// <summary>
        /// Returns the bounds of the text area of the node.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Rectangle TextBounds
        {
            get
            {
                int nX = 0;
                int nInc = m_nodeXRel + m_textLocationXRel;

                if (GetIsMirrored())
                {
                    nX = this.Bounds.Right - nInc - m_iTextWidth;
                }
                else
                {
                    nX = this.Bounds.X + nInc;
                }

                return new Rectangle(nX, this.Bounds.Y, m_iTextWidth, this.Bounds.Height);
            }
        }

        /// <summary>
        /// Returns the bounds of the left images, state images, text area and the right images of the node.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public Rectangle TextAndImageBounds
        {
            get
            {
                int nInc = m_nodeXRel;

                if (m_leftImageListXRel > 0)
                {
                    nInc += m_leftImageListXRel;
                }
                else if (m_stateImageListXRel > 0)
                {
                    nInc += m_stateImageListXRel;
                }
                else
                {
                    nInc += m_textLocationXRel;
                }

                int nWidth = m_nodeXRel + m_width - nInc;

                int nX = 0;

                if (GetIsMirrored())
                {
                    nX = this.Bounds.Right - nInc - nWidth;
                }
                else
                {
                    nX = this.Bounds.X + nInc;
                }

                return new Rectangle(nX, this.Bounds.Y, nWidth, this.Bounds.Height);
            }
        }

        /// <summary>
        /// Indicates whether the tree node is visible.
        /// </summary>
        /// <remarks> Return True only if node has property Visible set to True, and 
        /// parent node Visible to user (expanded and visible).</remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool IsVisible
        {
            get
            {
                bool result = this.Visible;

                if (this.ParentNode != null)
                {
                    result = (result & this.ParentNode.Expanded & this.ParentNode.IsVisible);
                }

                return result;
            }
        }

        /// <summary>
        /// Returns the parent tree node of the current tree node, if there is any.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the parent of the current 
        /// tree node.</value>
        /// <remarks>
        /// If this is the top most node in the tree, the Parent property returns the
        /// TreeViewAdv's <see cref="MultiColumnTreeView.Root"/> node.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets the parent tree node of the current tree node, if there is any.")
        ]
        public TreeNodeAdv Parent
        {
            get
            {
                return this.ParentNode;
            }
        }

        /// <summary>
        /// Returns the position of the tree node in the <see cref="Parent"/>'s tree node collection.
        /// </summary>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets the position of the tree node in the parent's tree nodes collection.")
        ]
        public int Index
        {
            get
            {
                if (this.ParentNode != null)
                {
                    return this.ParentNode.Nodes.IndexOf(this);
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the level of the node.
        /// </summary>
        /// <remarks>
        /// Specifies how deep a node is in the tree. The top-most visible nodes belong
        /// to level 1. The <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.Root"/> node is level 0.
        /// </remarks>
        [
        Browsable(false)
        ]
        public int Level
        {
            get
            {
                if (ParentNode == null)
                {
                    return 0;
                }

                return ParentNode.Level + 1;
            }
        }

        /// <summary>
        /// Gets / sets the parent tree view that the tree node is assigned to.
        /// </summary>
        /// <value>A <see cref="MultiColumnTreeView"/> that represents the parent tree view that 
        /// the tree node is assigned to.</value>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public MultiColumnTreeView TreeView
        {
            get
            {
                if (m_treeView != null)
                {
                    return m_treeView;
                }
                else if (this.ParentNode != null)
                {
                    return ParentNode.TreeView;
                }

                return null;
            }
            set
            {
                m_treeView = value;
            }
        }

        /// <summary>
        /// Returns the path from the root tree node to the current tree node.
        /// </summary>
        /// <value>The path from the root tree node to the current tree node.</value>
        /// <remarks><p>You can also use the more flexible <see cref="GetPath"/> method to
        /// get the path with a specific path separator.</p><p>The path consists of the labels of all of the tree nodes that must be 
        /// navigated to get to this tree node, starting at the root tree node. The node 
        /// labels are separated by the delimiter character specified in the 
        /// <see cref="MultiColumnTreeView.PathSeparator"/> property of the TreeViewAdv control that 
        /// contains this node. For example, if the delimiter character of the tree view 
        /// control named "Location" is set to the backslash character, (\), the <b>FullPath</b> 
        /// property value is "Country\Region\State".</p> If node not a part of tree then will be
        /// used default OS Path separator <see cref="Path.PathSeparator"/>.</remarks>
        [
        Browsable(false)
        ]
        public string FullPath
        {
            get
            {
                return this.GetPath((this.TreeView != null) ? this.TreeView.PathSeparator : "" + Path.PathSeparator);
            }
        }

        /// <summary>
        /// Gets / sets the object that contains data about the tree node.
        /// </summary>
        /// <value>
        /// An <see cref="System.Object"/> that contains data about the tree node. The default is a null reference (Nothing in Visual Basic).
        /// </value>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public object TagObject
        {
            get
            {
                return m_tag;
            }
            set
            {
                m_tag = value;
            }
        }

        /// <summary>
        /// Indicates the expanded state of a tree node.
        /// </summary>
        /// <value>True if the tree node is in the expanded state; false otherwise.
        /// </value>
        [
        Description("Indicates if the node is expanded."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public bool Expanded
        {
            get
            {
                return m_expanded;
            }
            set
            {
                if (m_expanded != value)
                {
                    MultiColumnTreeView tree = this.TreeView;
                    bool bUpdating = false;

                    if (tree != null)
                    {
                        bUpdating = this.TreeView.SuspendExpandRecalculate;

                        if (this.ParentNode != null)
                        {
                            if (!tree.ExpandedChanging(this, value))
                            {
                                return;
                            }
                        }

                        if (value && this.ParentNode != null)
                        {
                            this.ExpandedOnce = true;
                        }
                    }

                    m_expanded = value;

                    // Reset m_visibleNodeCount value for all parent nodes.
                    // This is used in calculation of visible nodes.
                    m_visibleNodeCount = -1;
                    TreeNodeAdv nodeParent = m_parent;

                    while (nodeParent != null)
                    {
                        nodeParent.m_visibleNodeCount = -1;
                        nodeParent = nodeParent.m_parent;
                    }

                    RecalculateDimensions();

                    if (tree != null && this.HasNodes && !(this.ShowPlusOnExpand && tree.LoadOnDemand))
                    {
                        tree.ExpandedChanged(this, value);
                    }

                    if (!bUpdating)
                    {
                        RecalculateMaxX();
                    }
                }
            }
        }

        /// <summary>
        /// Gets image for collapse button.
        /// </summary>
        private Image CollapseImage
        {
            get
            {
                return GetNodeStateImage();
            }
        }

        /// <summary>
        /// Gets image for expand button.
        /// </summary>
        private Image ExpandImage
        {
            get
            {
                return GetNodeStateImage();
            }
        }

        /// <summary></summary>
        protected internal int VisibleNodeCount
        {
            get
            {
                if (m_visibleNodeCount == -1)
                {
                    // Calculate visible node count only if this has been added to the tree.
                    if (this.TreeView == null)
                    {
                        return 0;
                    }

                    m_visibleNodeCount = 0;

                    if (this.HasNodes && this.Expanded)
                    {
                        for (int n = 0, len = this.Nodes.Count; n < len; n++)
                        {
                            TreeNodeAdv node = this.Nodes[n];

                            if (node.Visible)
                            {
                                m_visibleNodeCount += this.Nodes[n].VisibleNodeCount;
                            }
                        }
                    }

                    if (this.Visible)
                    {
                        m_visibleNodeCount++;
                    }
                }

                return m_visibleNodeCount;
            }
        }

        /// <summary>True - if node used as TreeView root node.</summary>
        private bool IsRoot
        {
            get
            {
                return (this.TreeView != null && this.TreeView.Root == this);
            }
        }

        /// <summary></summary>
        protected internal TreeNodeAdvAccessibleObject AccesibleObject
        {
            get
            {
                if (m_acsoNode == null)
                {
                    m_acsoNode = new TreeNodeAdvAccessibleObject(this);
                }

                return m_acsoNode;
            }
        }

        /// <summary></summary>
        private int ParentIndent
        {
            get
            {
                if (this.TreeView != null)
                {
                    return this.TreeView.Indent;
                }

                return DefaultParentIndent;
            }
        }

        /// <summary>
        /// Returns the TreeView History manager this node belongs to.
        /// </summary>
        protected HistoryManager HistoryManager
        {
            get
            {
                return (this.TreeView != null && this.TreeView.HistoryEnabled) ?
                  this.TreeView.HistoryManager : null;
            }
        }

        /// <summary>
        /// Indicates whether the node is in UndoRedo state.
        /// </summary>
        protected internal bool IsUndoRedoPerforming
        {
            get
            {
                return m_bIsUndoRedoPerforming;
            }
            set
            {
                if (value != m_bIsUndoRedoPerforming)
                {
                    m_bIsUndoRedoPerforming = value;
                }
            }
        }

        /// <summary>Get Font that used for HotTracking.</summary>
        /// <remarks>Don't forget to Dispose font after use. Property on each call return 
        /// new instance of Font.</remarks>
        private Font HotFont
        {
            get
            {
                Font dFont = NodeStyle.Font;
                return FontUtil.CreateFont(dFont, dFont.Style | FontStyle.Underline);
            }
        }

        /// <summary></summary>
        private Hashtable CachedPartialCheckedState
        {
            get
            {
                return m_partialCheckedState;
            }
        }

        /// <summary></summary>
        protected internal int Width
        {
            get
            {
                return m_width;
            }
            set
            {
                if (value != m_width)
                {
                    m_width = value;
                    this.MaxX = Math.Max(this.Right, m_maxX);
                }
            }
        }

        /// <summary></summary>
        protected internal int Right
        {
            get
            {
                return (this.Bounds.X + m_nodeXRel + m_width);
            }
        }

        /// <summary>
        /// Gets / sets the maximum width of all the children and subchildren of this given node.
        /// </summary>
        /// <remarks>Value does not include subitems.</remarks>
        protected internal int MaxX
        {
            get
            {
                return m_maxX;
            }
            set
            {
                if (m_maxX != value)
                {
                    m_maxX = value;

                    if (this.ParentNode != null)
                    {
                        // Notify the parent if maxX has changed and if the parent's maxX is 
                        // smaller then this value it will be changed to this value.
                        this.ParentNode.childMaxXChanged(m_maxX);
                    }
                    else
                    {
                        // Notify the TreeView that the Root's maxX has changed and set the HScrollBar's values.
                        if (this.TreeView != null)
                        {
                            this.TreeView.rootMaxXChanged(m_maxX);
                        }
                    }
                }
            }
        }

        /// <summary></summary>
        [DocumentationExclude()]
        public Rectangle PrintTextBounds
        {
            get
            {
                return new Rectangle(this.TextBounds.Location, m_printTextSize);
            }
        }

        /// <summary>New better name for API. this.Parent is obsolite property.</summary>
        private TreeNodeAdv ParentNode
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        /// <summary>Return node style info. If Node has sub items then instead of 
        /// tree node style will be returned first sub item style.</summary>
        protected internal ITreeNodeAdvSubItemStyle CurrentStyleInfo
        {
            get
            {
                return this.NodeStyle;
            }
        }

        /// <summary>Return calculated row bounds. Bounds contains visible and not 
        /// visible regions for user (scrolling in mind).</summary>
        protected internal Rectangle RowBounds
        {
            get
            {
                Rectangle rcBounds = this.Bounds;
                int widthCols = (this.TreeView != null) ? this.TreeView.Columns.GetTotalColumnsWidth(true) : 0;
                int widthMaxX = (this.TreeView != null) ? this.TreeView.Root.MaxX : 0;
                int widthClient = (this.TreeView != null) ? this.TreeView.Width : 0;
                int width = Math.Max(Math.Max(widthCols, widthMaxX), widthClient);

                return new Rectangle(rcBounds.Left, rcBounds.Top, width, rcBounds.Height);
            }
        }

        /// <summary>
        /// Returns the information about the node's appearance and state.
        /// </summary>
        /// <remarks>This property exposes the node's style information store.</remarks>
        [
        Description("Contains information about the node's appearance and state."),
        Category("Appearance"),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdvStyleInfo NodeStyle
        {
            get
            {
                return m_nodeData;
            }
        }

        /// <summary>
        /// Returns the information about the immediate child-nodes' appearance and state.
        /// </summary>
        [
        Description("Contains information about the immediate child-nodes' appearance and state."),
        Category("Appearance - Children"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public TreeNodeAdvStyleInfo ChildStyle
        {
            get
            {
                return m_childStyle;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when the check state of the node changes.
        /// </summary>
        /// <remarks><para>This event will be fired when the CheckedState property of the node has changed or when a new node has been Optioned.</para><para>You could alternatively listen to the <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.AfterCheck"/>
        /// event of the tree which will be called when the CheckState is changing for any node in the tree.
        /// If you want to cancel the check state change, then listen to 
        /// <see cref="Syncfusion.Windows.Forms.Tools.TreeViewAdv.BeforeCheck"/> of the tree.</para></remarks>
        public event EventHandler CheckStateChanged;
        #endregion

        #region Class Initialization
        /// <overloaded>
        /// Initializes a new instance of the <see cref="TreeNodeAdv"/> class.
        /// </overloaded>
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdv"/> class.
        /// </summary>
        public TreeNodeAdv()
        {
            Initialize("TreeNodeAdv", null, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdv"/> class with the specified label text.
        /// </summary>
        /// <param name="text"/>
        public TreeNodeAdv(string text)
        {
            Initialize(text, null, null);
        }

        /// <summary></summary>
        /// <param name="subitems"></param>
        public TreeNodeAdv(string[] subitems)
        {
            if (subitems == null)
            {
                throw new ArgumentNullException("subitems");
            }

            TreeNodeAdvSubItem[] items = new TreeNodeAdvSubItem[subitems.Length];

            for (int i = 0, len = items.Length; i < len; i++)
            {
                items[i] = new TreeNodeAdvSubItem(this, subitems[i]);
            }

            Initialize(string.Empty, null, items);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeAdv"/> class 
        /// with the specified label text and child tree nodes.
        /// </summary>
        /// <param name="text"/>
        /// <param name="nodes"/>
        public TreeNodeAdv(string text, TreeNodeAdv[] nodes)
        {
            Initialize(text, nodes, null);
        }

        /// <summary></summary>
        /// <param name="items"></param>
        public TreeNodeAdv(TreeNodeAdvSubItem[] items)
        {
            Initialize("TreeNodeAdv", null, items);
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="items"></param>
        public TreeNodeAdv(string text, TreeNodeAdvSubItem[] items)
        {
            Initialize(text, null, items);
        }

        /// <summary></summary>
        void ISupportInitialize.BeginInit()
        {
        }

        /// <summary></summary>
        void ISupportInitialize.EndInit()
        {
            if (HasNodes)
            {
                this.Nodes.Sort(NodeStyle.SortOrder);
            }
        }

        /// <summary></summary>
        /// <param name="text"></param>
        /// <param name="nodeArray"></param>
        private void Initialize(string text, TreeNodeAdv[] nodeArray, TreeNodeAdvSubItem[] items)
        {
            m_nodeData = new TreeNodeAdvStyleInfo(new TreeNodeAdvStyleInfoIdentity(this));
            m_nodeData.Changed += new StyleChangedEventHandler(StyleChanged);
            m_nodeData.Text = text;

            m_childStyle = new ChildTreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this));

            m_plusMinus = new TreeNodeAdvPart(this, new Size(9, 9));
            m_checkBox = new CheckBoxPart(this, new Size(13, 13));
            m_optionButton = new OptionButtonPart(this, new Size(14, 14));

            if (nodeArray != null && nodeArray.Length > 0)
            {
                this.Nodes.AddRange(nodeArray);
            }

            if (items != null && items.Length > 0)
            {
                this.SubItems.AddRange(items);
            }

            if (!this.HasSubItems)
            {
                // add first subitem that represents tree node data (!and don't forget to copy node style!)
                TreeNodeAdvSubItem first = new TreeNodeAdvSubItem(this);
                first.SubItemStyle.InheritStyle(this.NodeStyle, StyleModifyType.Copy);
                this.SubItems.Add(first);
            }

            RecalculateDimensions();
        }
        #endregion

        #region Class codedom serialization
   
        public void ResetPrimitives()
        {
            if (m_primitives != null)
            {
                m_primitives.Clear();
                m_primitives = null;
            }
        }

        protected bool ShouldSerializePrimitives()
        {
            return (m_primitives != null && m_primitives.Count > 0);
        }

        public void ResetExpandImageIndex()
        {
            this.NodeStyle.ResetExpandImageIndex();
        }

        protected bool ShouldSerializeExpandImageIndex()
        {
            return this.NodeStyle.ShouldSerializeExpandImageIndex();
        }

        public void ResetCollapseImageIndex()
        {
            this.NodeStyle.ResetCollapseImageIndex();
        }

        protected bool ShouldSerializeCollapseImageIndex()
        {
            return this.NodeStyle.ShouldSerializeCollapseImageIndex();
        }


        public void ResetLeftImage()
        {
            this.CurrentStyleInfo.ResetLeftImage();
        }


        protected bool ShouldSerializeLeftImage()
        {
            return this.CurrentStyleInfo.ShouldSerializeLeftImage();
        }

        public void ResetRightImage()
        {
            this.CurrentStyleInfo.ResetRightImage();
        }

        protected bool ShouldSerializeRightImage()
        {
            return this.CurrentStyleInfo.ShouldSerializeRightImage();
        }

        public void ResetClosedImage()
        {
            this.NodeStyle.ResetClosedImage();
        }

        protected bool ShouldSerializeClosedImage()
        {
            return this.NodeStyle.ShouldSerializeClosedImage();
        }

        public void ResetOpenImage()
        {
            this.NodeStyle.ResetOpenImage();
        }

        protected bool ShouldSerializeOpenImage()
        {
            return this.NodeStyle.ShouldSerializeOpenImage();
        }

        public void ResetFont()
        {
            this.CurrentStyleInfo.ResetFont();
        }

        protected bool ShouldSerializeFont()
        {
            return this.CurrentStyleInfo.ShouldSerializeFont();
        }

        public void ResetEnabled()
        {
            this.NodeStyle.ResetEnabled();
        }

        protected bool ShouldSerializeEnabled()
        {
            return this.NodeStyle.ShouldSerializeEnabled();
        }

        public void ResetTextColor()
        {
            this.CurrentStyleInfo.ResetTextColor();
        }

        protected bool ShouldSerializeTextColor()
        {
            return this.CurrentStyleInfo.ShouldSerializeTextColor();
        }

        public void ResetBackground()
        {
            this.CurrentStyleInfo.ResetBackground();
        }

        protected bool ShouldSerializeBackground()
        {
            return this.CurrentStyleInfo.ShouldSerializeBackground();
        }

        public void ResetText()
        {
            this.CurrentStyleInfo.ResetText();
        }

        protected bool ShouldSerializeText()
        {
            return this.CurrentStyleInfo.ShouldSerializeText();
        }

        public void ResetComparer()
        {
            this.NodeStyle.ResetComparer();
        }

        protected bool ShouldSerializeComparer()
        {
            return this.NodeStyle.ShouldSerializeComparer();
        }

        public void ResetEnabledButtons()
        {
            this.NodeStyle.ResetEnabledButtons();
        }


        protected bool ShouldSerializeEnabledButtons()
        {
            return this.NodeStyle.ShouldSerializeEnabledButtons();
        }

        /// <summary>
        /// Resets the <see cref="EnsureDefaultOptionedChild"/> property to its default value.
        /// </summary>
        public void ResetEnsureDefaultOptinedChild()
        {
            this.NodeStyle.ResetEnsureDefaultOptinedChild();
        }

        /// <summary>
        /// Determines if the <see cref="TreeNodeAdvStyleInfo.EnsureDefaultOptionedChild"/> property was modified.
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeEnsureDefaultOptinedChild()
        {
            return this.NodeStyle.ShouldSerializeEnsureDefaultOptinedChild();
        }

        public void ResetCulture()
        {
            this.NodeStyle.ResetCulture();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeCulture()
        {
            return this.NodeStyle.ShouldSerializeCulture();
        }

        /// <summary></summary>
        public void ResetInteractiveCheckBox()
        {
            this.NodeStyle.ResetInteractiveCheckBox();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeInteractiveCheckBox()
        {
            return this.NodeStyle.ShouldSerializeInteractiveCheckBox();
        }

        /// <summary></summary>
        public void ResetThemesEnabled()
        {
            this.NodeStyle.ResetThemesEnabled();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeThemesEnabled()
        {
            return this.NodeStyle.ShouldSerializeThemesEnabled();
        }

        /// <summary></summary>
        public void ResetHelpText()
        {
            this.CurrentStyleInfo.ResetHelpText();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeHelpText()
        {
            return this.CurrentStyleInfo.ShouldSerializeHelpText();
        }

        /// <summary></summary>
        public void ResetHeight()
        {
            this.NodeStyle.ResetHeight();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeHeight()
        {
            return this.NodeStyle.ShouldSerializeHeight();
        }

        /// <summary></summary>
        public void ResetShowCheckBox()
        {
            this.NodeStyle.ResetShowCheckBox();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeShowCheckBox()
        {
            return this.NodeStyle.ShouldSerializeShowCheckBox();
        }

        /// <summary></summary>
        public void ResetOpenImgIndex()
        {
            this.NodeStyle.ResetOpenImgIndex();
        }

        /// <summary></summary>
        /// <returns></returns>
        protected bool ShouldSerializeOpenImgIndex()
        {
            return this.NodeStyle.ShouldSerializeOpenImgIndex();
        }

        public void ResetClosedImgIndex()
        {
            this.NodeStyle.ResetClosedImgIndex();
        }

        protected bool ShouldSerializeClosedImgIndex()
        {
            return this.NodeStyle.ShouldSerializeClosedImgIndex();
        }

        public void ResetShowOptionButton()
        {
            this.NodeStyle.ResetShowOptionButton();
        }

        protected bool ShouldSerializeShowOptionButton()
        {
            return this.NodeStyle.ShouldSerializeShowOptionButton();
        }
        public void ResetShowPlusMinus()
        {
            this.NodeStyle.ResetShowPlusMinus();
        }

        protected bool ShouldSerializeShowPlusMinus()
        {
            return this.NodeStyle.ShouldSerializeShowPlusMinus();
        }
        public void ResetSortOrder()
        {
            this.NodeStyle.ResetSortOrder();
        }

        protected bool ShouldSerializeSortOrder()
        {
            return this.NodeStyle.ShouldSerializeSortOrder();
        }

        public void ResetSortType()
        {
            this.NodeStyle.ResetSortType();
        }
        protected bool ShouldSerializeSortType()
        {
            return this.NodeStyle.ShouldSerializeSortType();
        }

        public void ResetCompareOptions()
        {
            this.NodeStyle.ResetCompareOptions();
        }

        protected bool ShouldSerializeCompareOptions()
        {
            return this.NodeStyle.ShouldSerializeCompareOptions();
        }

        public void ResetCheckState()
        {
            this.NodeStyle.ResetCheckState();
        }

        protected bool ShouldSerializeCheckState()
        {
            return this.NodeStyle.ShouldSerializeCheckState();
        }

        public void ResetBaseStyle()
        {
            this.CurrentStyleInfo.ResetBaseStyle();
        }

        protected bool ShouldSerializeBaseStyle()
        {
            return this.CurrentStyleInfo.ShouldSerializeBaseStyle();
        }

        public void ResetTag()
        {
            this.CurrentStyleInfo.ResetTag();
        }

        protected bool ShouldSerializeTag()
        {
            return this.CurrentStyleInfo.ShouldSerializeTag();
        }

        public void ResetLeftImageIndices()
        {
            this.CurrentStyleInfo.ResetLeftImageIndices();
        }

        protected bool ShouldSerializeLeftImageIndices()
        {
            return this.CurrentStyleInfo.ShouldSerializeLeftImageIndices();
        }
        public void ResetRightImageIndices()
        {
            this.CurrentStyleInfo.ResetRightImageIndices();
        }

        protected bool ShouldSerializeRightImageIndices()
        {
            return this.CurrentStyleInfo.ShouldSerializeRightImageIndices();
        }
        public void ResetNoChildrenImgIndex()
        {
            this.NodeStyle.ResetNoChildrenImgIndex();
        }
        protected bool ShouldSerializeNoChildrenImgIndex()
        {
            return this.NodeStyle.ShouldSerializeNoChildrenImgIndex();
        }

        /// <summary>
        /// Reset property Multiline value to default value
        /// </summary>
        public virtual void ResetMultiline()
        {
            this.NodeStyle.ResetMultiline();
        }

        /// <summary>
        /// Indicate should or not we serialize Multiline property value.
        /// </summary>
        /// <returns>True - serialization required, otherwise False.</returns>
        protected virtual bool ShouldSerializeMultiline()
        {
            return this.NodeStyle.ShouldSerializeMultiline();
        }
        #endregion

        #region Class serialization

        private TreeNodeAdv(SerializationInfo info, StreamingContext context)
            : this()
        {
            m_nodeData = new TreeNodeAdvStyleInfo(info.GetValue("NodeStyle", typeof(TreeNodeAdvStyleInfoStore)) as TreeNodeAdvStyleInfoStore);
            m_nodeData.Changed += new StyleChangedEventHandler(StyleChanged);

            m_childStyle = new ChildTreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this), info.GetValue("ChildStyle", typeof(TreeNodeAdvStyleInfoStore)) as TreeNodeAdvStyleInfoStore);

            // TODO: check is all required fields well deserialized? for example Visible?!
        }

        /// <summary>
        /// Populates the provided SerializationInfo with the data needed to serialize the object .
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NodeStyle", NodeStyle.Store);
            info.AddValue("ChildStyle", ChildStyle.Store);

            // TODO: check is all required fields well serialized? for example Visible, Expanded?!
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Returns the child node who's option button is checked.
        /// </summary>
        /// <returns></returns>
        protected internal TreeNodeAdv GetOptionedChild()
        {
            if (this.HasNodes)
            {
                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    TreeNodeAdv node = this.Nodes[i];

                    if (node.Optioned)
                    {
                        return node;
                    }
                }
            }

            return null;
        }

        /// <summary>Raises the CheckStateChanged event.</summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnCheckStateChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for handling the event in a 
        /// derived class.</para><para>Notes to Inheritors:  When overriding OnCheckStateChanged in a 
        /// derived class, be sure to call the base class's OnCheckStateChanged method so that
        /// registered delegates receive the event.</para></remarks>
        protected internal virtual void OnCheckStateChanged(EventArgs e)
        {
            if (CheckStateChanged != null)
            {
                CheckStateChanged(this, e);
            }
        }

        /// <summary>Called by primitives collection when collection detect changes.</summary>
        /// <param name="e"/>
        protected internal virtual void OnPrimitivesCollectionChanged(CollectionChangeEventArgs e)
        {
            MakeDirty();
        }
        protected internal virtual void OnSubItemChanged(StyleChangedEventArgs e)
        {
            MakeDirty();

            RecalculateDimensions();
        }

        protected internal virtual void OnBeforeChildRemove(CollectionChangeEventArgs e)
        {
            MultiColumnTreeView tree = this.TreeView;
            TreeNodeAdv node = e.Element as TreeNodeAdv;

            if (node != null && tree != null && tree.HistoryEnabled &&
              !node.IsUndoRedoPerforming)
            {
                HistoryManager historyManager = tree.HistoryManager;

                if (historyManager != null)
                {
                    TreeViewCommand cmd = new TreeViewCommand(node, Action.Remove);
                    historyManager.Do(cmd);
                }
            }
        }
        private void RemoveCheckedNode(TreeNodeAdv node, CheckedNodesColection nodes)
        {
            foreach (TreeNodeAdv subNode in node.Nodes)
            {
                RemoveCheckedNode(subNode, nodes);
            }
            nodes.Remove(node);
        }

        protected internal virtual void OnChildsCollectionChanged(CollectionChangeEventArgs e)
        {
            if (e.Action != CollectionChangeAction.Refresh)
            {
                CustomControlCollectionChanging(e.Element as TreeNodeAdv, e.Action);
            }

            if (this.TreeView != null)
            {
                TreeView.NodesChanging();
            }

            if (e.Action == CollectionChangeAction.Remove)
            {
                TreeNodeAdv node = (TreeNodeAdv)e.Element;
                node.m_treeView = null; // reset reference on tree - performance issue!
                MultiColumnTreeView tree = this.TreeView;

                // If the selected node is the removed node or the selected node is a child of the removed node
                if (tree != null && tree.SelectedNode != null &&
                  (tree.SelectedNode == node || tree.SelectedNode.IsParent(node)))
                {
                    AdjustSelectedNode(tree.m_currentSelectedNodeIndex);
                }

                // remove nodes from the checked list
                if (TreeView != null)
                {
                    this.RemoveCheckedNode(node, TreeView.CheckedNodes);
                }

                RemovedNode(node);
            }
            else if (e.Action == CollectionChangeAction.Add)
            {
                TreeNodeAdv node = (TreeNodeAdv)e.Element;
                node.m_treeView = this.TreeView; // set reference on tree view - performance issue!
                node.ParentNode = this;
                UpdateSelectedNodeIndexCache();
                AddedNode(node);
                MakeDirty();

                if (this.TreeView != null)
                {
                    // if node is checked, add it to checked nodes List in parent TreeView
                    TreeView.CheckedNodes.ResolveNode(node);

                    HistoryManager historyManager = this.HistoryManager;

                    if (historyManager != null && !node.IsUndoRedoPerforming)
                    {
                        TreeViewCommand cmd = new TreeViewCommand(node, Action.Add);
                        historyManager.Do(cmd);
                    }
                }
            }
            // This will be called while sorting, clearing and moving items in the list.
            else if (e.Action == CollectionChangeAction.Refresh)
            {
                this.TreeView.NodeInRefresh = true;
                UpdateSelectedNodeIndexCache();

                MultiColumnTreeView tva = this.TreeView;

                if (tva != null)
                {
                    foreach (TreeNodeAdv node in this.Nodes)
                    {
                        AddedNode(node);

                        // if node is checked, add it to checked nodes List in parent TreeView
                        tva.CheckedNodes.ResolveNode(node);

                        HistoryManager historyManager = this.HistoryManager;

                        if (historyManager != null && !node.IsUndoRedoPerforming)
                        {
                            TreeViewCommand cmd = new TreeViewCommand(node, Action.Add);
                            historyManager.Do(cmd);
                        }
                    }
                }

                MakeDirty();
                this.TreeView.NodeInRefresh = false;
            }

            UpdatePlusMinusVisibility();

            // Reset only my visible node count setting (not the children's)
            ResetVisibleNodeCount(false);

            MakeDirty();

            if (TreeView != null)
            {
                TreeView.NodesChanged();
            }
        }

        /// <summary>
        /// Custom control collection changing.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        protected internal virtual void CustomControlCollectionChanging(TreeNodeAdv node, CollectionChangeAction action)
        {
            MultiColumnTreeView tree = this.TreeView;

            if (tree != null && node != null)
            {
                switch (action)
                {
                    case CollectionChangeAction.Add:
                        if (node.CustomControl != null)
                        {
                            if (!tree.CustomControlCollection.ContainsKey(node.CustomControl))
                            {
                                tree.CustomControlCollection.Add(node.CustomControl, node);

                                if (tree.AutoControlsAdding)
                                {
                                    tree.Controls.Add(node.CustomControl);
                                    node.SubscribeControlEvents(node.CustomControl);
                                }
                            }
                            else if (tree.CustomControlCollection[node.CustomControl] != node)
                            {
                                node.CustomControl = null;
                            }
                        }

                        foreach (TreeNodeAdv childNode in node.Nodes)
                        {
                            CustomControlCollectionChanging(childNode, action);
                        }

                        tree.NeedUpdateCustomControls = true;

                        break;

                    case CollectionChangeAction.Remove:
                        if (node.CustomControl != null)
                        {
                            if (tree.CustomControlCollection.ContainsKey(node.CustomControl))
                            {
                                tree.CustomControlCollection.Remove(node.CustomControl);
                            }

                            if (tree.Controls.Contains(node.CustomControl))
                            {
                                tree.Controls.Remove(node.CustomControl);
                                node.UnSubscribeControlEvents(node.CustomControl);
                            }
                        }

                        foreach (TreeNodeAdv childNode in node.Nodes)
                        {
                            CustomControlCollectionChanging(childNode, action);
                        }

                        tree.NeedUpdateCustomControls = true;

                        break;

                    case CollectionChangeAction.Refresh:
                        if (tree.CustomControlCollection.ContainsValue(node))
                        {
                            foreach (DictionaryEntry htElement in tree.CustomControlCollection)
                            {
                                if (htElement.Value == node)
                                {
                                    tree.CustomControlCollection.Remove(htElement.Key);

                                    Control control = htElement.Key as Control;

                                    if (tree.Controls.Contains(control))
                                    {
                                        tree.Controls.Remove(control);
                                        this.UnSubscribeControlEvents(control);
                                    }

                                    break;
                                }
                            }
                        }

                        if (node.CustomControl != null)
                        {
                            if (!tree.CustomControlCollection.ContainsKey(node.CustomControl))
                            {
                                tree.CustomControlCollection.Add(node.CustomControl, node);

                                if (tree.AutoControlsAdding)
                                {
                                    tree.Controls.Add(node.CustomControl);
                                    this.SubscribeControlEvents(node.CustomControl);
                                }
                            }
                            else if (tree.CustomControlCollection[node.CustomControl] != node)
                            {
                                node.CustomControl = null;
                            }
                        }

                        RecalculateDimensions();
                        tree.NeedUpdateCustomControls = true;
                        tree.Invalidate();
                        break;
                }
            }
        }
        #endregion

        #region Class event handlers

        private void CustomControl_SizeChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;

            if (control.Visible && control.Size.Height != this.Height)
            {
                control.Size = new Size(control.Width, this.Height);
            }
        }

        /// <summary> Occurs when sub item collection changed.</summary>
        /// <param name="sender"/>
        /// <param name="e"/>
        private void SubItems_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            // invalidate area of subitem if it settings changed and it's visible to user
            if (this.Visible)
            {
                TreeNodeAdvSubItem item = e.Element as TreeNodeAdvSubItem;

                if (item != null)
                {
                    InvalidateTreeView(item.Bounds);
                }
            }
        }
        private void CustomControl_LocationChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            Point controlLocation = new Point(this.Bounds.X + m_customControlRelativeLocation + m_nodeXRel, this.Bounds.Y);

            if (control.Visible && control.Location != controlLocation)
            {
                control.Location = controlLocation;
            }
        }

        internal void StyleChanged(object sender, StyleChangedEventArgs e)
        {
            bool bUpdate = false;

            StyleInfoProperty sip = e.Sip;

            if (null != sip)
            {
                switch (sip.PropertyName)
                {
                    case "Multiline":
                        bUpdate = true;
                        goto default;

                    case "Comparer":
                        this.Nodes.Comparer = this.NodeStyle.Comparer;
                        break;

                    case "CheckState":
                        CheckState_Changed();
                        break;

                    case "ShowPlusMinus":
                        ShowPlusMinusChanged();
                        bUpdate = true;
                        goto default;

                    case "StateNoChildrenImage":
                        bUpdate = true;
                        goto default;

                    case "ShowOptionButton":
                    case "ShowCheckBox":
                    case "LeftImageIndices":
                    case "StateImage":
                    case "RightImageIndices":
                    case "Text":
                    case "Font":
                        bUpdate = true;
                        break;

                    default:
                        if (bUpdate)
                            RecalculateDimensions();
                        break;
                }
            }

            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                tree.NeedUpdateCustomControls = true;
            }

            InvalidateTreeView();
        }
        #endregion

        #region Class Public Methods
        /// <summary>Recalculates the dimensions of all the UI elements in this node 
        /// and it's children.</summary>
        /// <remarks>Method is call recalculation recursively. Please keep in mind that 
        /// this can greatly reduce performance.</remarks>
        public void RecalculateAllDimensions()
        {
            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                tree.NeedUpdateCustomControls = true;
            }

            // Marking a flag that says I am in the process of recalculating my MaxX
            htRecalculatingNodes[this] = 1;

            // Lose my previous maxX
            m_maxX = 0;

            for (int i = 0, len = this.Nodes.Count; i < len; i++)
            {
                this.Nodes[i].RecalculateAllDimensions();
            }

            RecalculateDimensions();

            // Clearing the flag.
            htRecalculatingNodes.Remove(this);
        }

        /// <summary>
        /// Returns the path of the node.
        /// </summary>
        /// <param name="separator">The separator string.</param>
        /// <returns>The path of the node.</returns>
        /// <remarks><p>You can also use the <see cref="FullPath"/> property to get the full path
        /// with the path separator specified in the <see cref="MultiColumnTreeView.PathSeparator"/>
        /// property.</p></remarks>
        public string GetPath(string separator)
        {
            StringBuilder path = new StringBuilder();
            GetFullPath(path, separator);

            if (this.TreeView != null && this.TreeView.AddSeparatorAtEnd)
            {
                path.Append(separator);
            }

            return path.ToString();
        }

        /// <summary>Optimization method that allow to reduce cost of FullPath operation.</summary>
        /// <param name="path">FullPath container.</param>
        /// <param name="separator">The separator string.</param>
        private void GetFullPath(StringBuilder path, string separator)
        {
            if (this.ParentNode != null)
            {
                this.ParentNode.GetFullPath(path, separator);

                if (this.ParentNode.ParentNode != null)
                {
                    path.Append(separator);
                }

                path.Append(this.Text);
            }
        }

        /// <summary>
        /// Expands parent nodes to make this node visible and also scrolls
        /// the tree such that this node is brought into view.
        /// </summary>
        public void BringIntoView()
        {
            if (!this.Visible && this.ParentNode != null)
            {
                this.ParentNode.ExpandParentSelf();
            }

            if (!this.Visible)
            {
                return;
            }

            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                // Update the tree so that any unhidden node's bounds will be set.
                tree.Update();
                tree.EnsureVisible(this);
                tree.IsBroughtIntoView = true;
            }
        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        public void Expand()
        {
            this.Expanded = true;
        }

        /// <summary>
        /// Expands this node and all the subnodes.
        /// </summary>
        public void ExpandAll()
        {
            MultiColumnTreeView tree = this.TreeView;
            if (tree != null)
            {
                tree.BeginUpdate();
            }

            this.Expanded = true;

            // NOTE: user can catch here event and add several nodes into collection.
            // We will not catch this if will use FOR instead of FOREACH. Enumerators
            // has protection from collection changes and will throw exception if user
            // will try to change collection in process of foreach executing.

            if (this.HasNodes)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    this.Nodes[i].ExpandAll();
                }
            }

            if (tree != null)
            {
                tree.EndUpdate();
            }
        }

        /// <summary>
        /// Collapses this node and all it's children.
        /// </summary>
        public void CollapseAll()
        {
            MultiColumnTreeView tree = this.TreeView;
            if (tree != null)
            {
                tree.BeginUpdate();
            }

            // NOTE: user can catch here event and add several nodes into collection.
            // We will not catch this if will use FOR instead of FOREACH. Enumerators
            // has protection from collection changes and will throw exception if user
            // will try to change collection in process of foreach executing.

            if (this.HasNodes)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    this.Nodes[i].CollapseAll();
                }
            }

            if (tree != null && this != tree.Root)
            {
                this.Expanded = false;
            }

            if (tree != null)
            {
                tree.EndUpdate();
            }
        }

        /// <summary>
        /// Removes itself from the parent node, if there is any.
        /// </summary>
        public void Remove()
        {
            if (this.ParentNode != null)
            {
                this.ParentNode.Nodes.Remove(this);
            }
        }

        /// <summary>
        /// Returns the number of child tree nodes.
        /// </summary>
        /// <param name="includeSubTrees"><b>True</b> if the resulting count includes all tree 
        /// nodes indirectly rooted at this tree node; <b>false</b> otherwise. </param>
        /// <returns>The number of child tree nodes assigned to the <see cref="Nodes"/> collection.</returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            if (!this.HasNodes)
            {
                return 0;
            }

            int count = this.Nodes.Count;

            if (includeSubTrees)
            {
                foreach (TreeNodeAdv node in this.Nodes)
                {
                    count += node.GetNodeCount(true);
                }
            }

            return count;
        }

        /// <overloaded>
        /// Moves this node to a different collection.
        /// </overloaded>
        /// <summary>
        /// Moves the node to the end of the specified collection.
        /// </summary>
        /// <param name="newNodesCollection">
        /// A <see cref="TreeNodeAdvCollection"/> to which this node will move.
        /// </param>
        /// <remarks><para>A node can be positioned to any other TreeNodeAdvCollection in the same tree or in a different TreeViewAdv control.</para><para>Note: All of the descendants of the node will move along with it.</para><para>Note: A node cannot be moved to one of it's own Descendants.</para></remarks>
        public void Move(TreeNodeAdvCollection newNodesCollection)
        {
            this.Move(newNodesCollection, -1);
        }

        /// <summary>
        /// Moves the node to a new collection at the specified index.
        /// </summary>
        /// <param name="newNodesCollection">
        /// A <see cref="TreeNodeAdvCollection"/> to which this node will move.</param>
        /// <param name="index">
        /// The new index of the node in the new collection.
        /// </param>
        /// <remarks>
        /// Moving a node by index will ensure that the node ends up at the index 
        /// specified. Note that the node will first be removed from its existing
        /// collection and then added to the specified collection at the specified index.
        /// If the source collection and destination collection are the same, make sure
        /// to take into account the above semantics while specifying the index, or
        /// use the Move override that lets you specify a relative position.
        /// </remarks>
        public void Move(TreeNodeAdvCollection newNodesCollection, int index)
        {
            HistoryManager historyManager = this.HistoryManager;

            if (historyManager != null)
            {
                historyManager.BeginBlock();
            }

            if (newNodesCollection == null)
            {
                return;
            }

            if (this.ParentNode != null)
            {
                // First remove from the parent's collection
                this.ParentNode.Nodes.Remove(this);
            }

            // Add to the newNodesCollection:
            if (index == -1)
            {
                newNodesCollection.Add(this);
            }
            else
            {
                newNodesCollection.Insert(index, this);
            }

            if (historyManager != null)
            {
                historyManager.CloseBlock();
            }
        }

        /// <summary>
        /// Moves the node to a specified position in relation to the specified 
        /// "relative node".
        /// </summary>
        /// <param name="relativeNode">The "relative node" that determines this node's new position.</param>
        /// <param name="nodePosition">Specifies where this node will be moved in relation to the "relative node". </param>
        /// <remarks><para>A node can be positioned relative to any other node in the same tree or even a different TreeViewAdv Control.</para></remarks>
        public void Move(TreeNodeAdv relativeNode, NodePositions nodePosition)
        {
            HistoryManager historyManager = this.HistoryManager;

            if (historyManager != null)
            {
                historyManager.BeginBlock();
            }

            if (relativeNode == null || (
              (nodePosition == NodePositions.Next || nodePosition == NodePositions.Previous) &&
              relativeNode.ParentNode == null)
              )
            {
                return;
            }

            if (this.ParentNode != null)
            {
                // First remove from the parent's collection
                this.ParentNode.Nodes.Remove(this);
            }

            switch (nodePosition)
            {
                case NodePositions.First:
                    this.Move(relativeNode.Nodes, 0);
                    break;
                case NodePositions.Last:
                    this.Move(relativeNode.Nodes, -1);
                    break;
                case NodePositions.Next:
                    this.Move(relativeNode.ParentNode.Nodes,
                      relativeNode.ParentNode.Nodes.IndexOf(relativeNode) + 1);
                    break;
                case NodePositions.Previous:
                    this.Move(relativeNode.ParentNode.Nodes,
                      relativeNode.ParentNode.Nodes.IndexOf(relativeNode));
                    break;
            }

            if (historyManager != null)
            {
                historyManager.CloseBlock();
            }
        }

        public void Move(TreeNodeAdvCollection col, NodePositions nodePosition)
        {
            HistoryManager historyManager = this.HistoryManager;

            if (historyManager != null)
            {
                historyManager.BeginBlock();
            }

            if (col == null)
            {
                throw new ArgumentNullException("col");
            }

            if (this.ParentNode != null)
            {
                // First remove from the parent's collection
                this.ParentNode.Nodes.Remove(this);
            }

            switch (nodePosition)
            {
                case NodePositions.Previous:
                case NodePositions.First:
                    Move(col, 0);
                    break;

                case NodePositions.Next:
                case NodePositions.Last:
                    Move(col, -1);
                    break;
            }

            if (historyManager != null)
            {
                historyManager.CloseBlock();
            }
        }

        /// <summary>
        /// Indicates whether the current node is a direct or indirect child of the specified node.
        /// </summary>
        /// <param name="targetNode">The node that is to be tested for ancestry.</param>
        /// <returns>True if the targetNode is a parent of this node; False otherwise.</returns>
        public bool IsParent(TreeNodeAdv targetNode)
        {
            if (targetNode == null)
            {
                return false;
            }

            TreeNodeAdv node = this;
            while (node != null && node.ParentNode != targetNode)
            {
                node = node.ParentNode;
            }

            if (node != null && node.ParentNode == targetNode)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether node is contained in it's nodes collection or in it's subnodes nodes collection.
        /// </summary>
        /// <param name="node">Node to look for.</param>
        /// <returns>True if node is contained.</returns>
        public bool HasNode(TreeNodeAdv node)
        {
            if (this.HasNodes)
            {
                if (this.Nodes.Contains(node))
                {
                    return true;
                }

                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    if (this.Nodes[i].HasNode(node))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>Method search by binary search algorithm node that has 
        /// specified unique row Index.</summary>
        /// <returns>Null - if nothing found, otherwise reference on node.</returns>
        /// <param name="rowIndex">Unique row index. Valid values are higher 0 (zero).</param>
        public TreeNodeAdv GetNodeAtAbsoluteRowIndex(int rowIndex)
        {
            TreeNodeAdv node = this;

            do
            {
                if (node.TreeRowIndex == rowIndex)
                {
                    return node;
                }

                int index = node.Nodes.BinarySearch(rowIndex, TreeRowIndexComparer.Default);
                if (-1 == index)
                {
                    return null;
                }
                if (0 <= index)
                {
                    return node.Nodes[index];
                }

                node = node.Nodes[(~index) - 1];
            }
            while (true);
        }

        /// <summary>Method search node by relative position from current node.</summary>
        /// <returns>Found node, otherwise Null.</returns>
        /// <param name="rowIndex">Relative diff number. Negative values mean Previous node logic,
        /// positive mean Next node logic.</param>
        public TreeNodeAdv GetNodeAtRelativeRowIndex(int rowIndex)
        {
            TreeNodeAdv node = this;

            if (rowIndex != 0)
            {
                int direction = (rowIndex < 0) ? 1 : -1;

                while (rowIndex != 0 && node != null)
                {
                    node = (direction == 1) ? node.PrevVisibleNode : node.NextVisibleNode;
                    rowIndex += direction;
                }
            }

            return node;
        }

        protected internal int GetTreeRowIndexOfChild(TreeNodeAdv child)
        {
            int rowIndex = this.TreeRowIndex + 1;

            if (this.HasNodes)
            {
                for (int n = 0, count = this.Nodes.Count; n < count; n++)
                {
                    TreeNodeAdv node = this.Nodes[n];

                    if (node == child)
                    {
                        break;
                    }

                    rowIndex += node.VisibleNodeCount;
                }
            }

            return rowIndex;
        }

        /// <summary>
        /// If "adv" node is multiparent for current node than returns true, else returns false.
        /// Method uses recursion.
        /// </summary>
        /// <param name="adv"></param>
        /// <returns></returns>
        public bool IsNodeRelative(TreeNodeAdv adv)
        {
            if (adv == null)
            {
                return false;
            }

            if (this == adv)
            {
                return true;
            }

            if (this.ParentNode != null)
            {
                return this.ParentNode.IsNodeRelative(adv);
            }

            return false;
        }
        #endregion

        #region Sorting
        int IComparable.CompareTo(object obj)
        {
            if (obj is TreeNodeAdv)
            {
                TreeNodeAdv node = obj as TreeNodeAdv;

                if (this.ParentNode == null)
                {
                    return Text.CompareTo(node.Text);
                }

                switch (this.ParentNode.SortType)
                {
                    case TreeNodeAdvSortType.Text:
                        return NodeTextCompare(node.Text);

                    case TreeNodeAdvSortType.Tag:
                        return NodeTagCompare(node.Tag);

                    case TreeNodeAdvSortType.CheckBox:
                        {
                            if (this.Checked)
                            {
                                if (node.Checked)
                                {
                                    return 0;
                                }

                                return 1;
                            }

                            if (node.Checked)
                            {
                                return -1;
                            }

                            return (this.CheckState == CheckState.Indeterminate) ?
                              ((node.CheckState == CheckState.Indeterminate) ? 0 : 1) :
                              ((node.CheckState == CheckState.Unchecked) ? 0 : -1);
                        }
                }
            }
            // obj will be string when BinarySearch is called with a Text.
            else if (obj is string)
            {
                string text = (string)obj;

                if (ParentNode == null)
                {
                    return NodeTextCompare(text);
                }

                switch (ParentNode.SortType)
                {
                    case TreeNodeAdvSortType.Tag:
                        return NodeTagCompare(text);

                    default:
                    case TreeNodeAdvSortType.Text:
                        return NodeTextCompare(text);
                }
            }
            // obj could of some other type. This will be the case when sorting 
            // is by tag and the tags are of unique type.
            else
            {
                if (ParentNode != null && ParentNode.SortType == TreeNodeAdvSortType.Tag)
                {
                    return NodeTagCompare(obj);
                }
            }

            return 0;
        }

        private int NodeTextCompare(string compareText)
        {
            return m_culture.CompareInfo.Compare(this.Text, compareText,
              (ParentNode != null) ? ParentNode.NodeStyle.CompareOptions : CompareOptions.None);
        }

        private int NodeTagCompare(object compareTag)
        {
            if (this.Tag != null)
            {
                if (compareTag != null)
                {
                    if (this.Tag is IComparable)
                    {
                        if (compareTag is IComparable)
                        {
                            return ((IComparable)this.Tag).CompareTo(compareTag);
                        }

                        return 1;
                    }

                    return -1;
                }

                return 1;
            }

            if (compareTag != null)
            {
                return -1;
            }

            return 0;
        }

        /// <overloaded>
        /// Sorts the tree nodes.
        /// </overloaded>
        /// <summary>
        /// Sorts the tree nodes with the current 
        /// <see cref="TreeNodeAdvStyleInfo.SortOrder"/> and <see cref="TreeNodeAdvStyleInfo.SortType"/>.
        /// </summary>
        public void Sort()
        {
            if (this.HasNodes)
            {
                this.Nodes.Sort(NodeStyle.SortOrder);

                if (this.TreeView.SortWithChildNodes)
                {
                    foreach (TreeNodeAdv tna in this.Nodes)
                    {
                        tna.SortOrder = this.SortOrder;
                        tna.Sort();
                    }
                }
            }
        }

        /// <summary>
        /// Sorts the tree nodes with the specified sort type and the current
        /// <see cref="P:NodeStyle.SortOrder"/>.
        /// </summary>
        /// <param name="sortType">One of the <see cref="TreeNodeAdvSortType"/> value.</param>
        /// <remarks>This will also set the value in the <see cref="P:NodeStyle.SortOrder"/> to the
        /// specified sort type.
        /// </remarks>
        public void Sort(TreeNodeAdvSortType sortType)
        {
            if (this.HasNodes)
            {
                m_nodeData.SortType = sortType;
                Sort();

                if (this.TreeView.SortWithChildNodes)
                {
                    foreach (TreeNodeAdv tna in this.Nodes)
                    {
                        tna.Sort(sortType);
                    }
                }
            }
        }
        #endregion

        #region Clone
        /// <summary>Simple memberwise clone.</summary>
        /// <returns>Reference on cloned object.</returns>
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Creates a clone of this node.
        /// </summary>
        /// <returns>The clone of the node.</returns>
        public TreeNodeAdv Clone()
        {
            TreeNodeAdv newNode = new TreeNodeAdv(this.Text);

            newNode.LeftStateImagePadding = this.LeftStateImagePadding;
            newNode.RightStateImagePadding = this.RightStateImagePadding;
            newNode.LeftImagePadding = this.LeftImagePadding;
            newNode.RightImagePadding = this.RightImagePadding;

            newNode.ExpandImageIndex = this.ExpandImageIndex;
            newNode.CollapseImageIndex = this.CollapseImageIndex;

            newNode.NodeStyle.ModifyStyle(NodeStyle, StyleModifyType.Copy);
            newNode.ChildStyle.ModifyStyle(ChildStyle, StyleModifyType.Copy);
            newNode.SetBounds(this.Bounds);

            if (this.HasSubItems)
            {
                newNode.SetSubItems(this.SubItems.Clone());
            }

            if (this.HasPrimitives)
            {
                newNode.SetPrimitives(this.Primitives.Clone());
            }

            newNode.SetCustomControlMember(this.CustomControl);

            newNode.Expanded = this.Expanded;
            newNode.ShowPlusOnExpand = this.ShowPlusOnExpand;

            if (this.HasNodes)
            {
                newNode.Nodes.Clear();

                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    newNode.Nodes.Add(this.Nodes[i].Clone());
                }
            }

            if (this.Tag is ICloneable)
            {
                newNode.Tag = ((ICloneable)this.Tag).Clone();
            }

            return newNode;
        }
        #endregion

        #region Mouse Hit Test

        internal Rectangle MouseInControl(Point pt)
        {
            if (m_plusMinus.Visible && m_plusMinus.Bounds.Contains(pt))
            {
                return m_plusMinus.Bounds;
            }

            if (m_checkBox.Visible && m_checkBox.Bounds.Contains(pt))
            {
                return m_checkBox.Bounds;
            }

            if (m_optionButton.Visible && m_optionButton.Bounds.Contains(pt))
            {
                return m_optionButton.Bounds;
            }

            return Rectangle.Empty;
        }

        internal bool ProcessMouseDown(Point pt)
        {
            if (!this.Enabled)
            {
                return false;
            }

            if (m_plusMinus.Visible)
            {
                Rectangle pmBounds = m_plusMinus.Bounds;

                // Provide some leeway around the bounds.
                pmBounds.Inflate(4, 3);
                if (pmBounds.Contains(pt))
                {
                    if (!this.ShowPlusOnExpand || !this.TreeView.LoadOnDemand)
                    {
                        this.Expanded = !this.Expanded;
                    }
                    else
                    {
                        if (!this.Expanded)
                        {
                            this.Expanded = true;
                        }
                        else
                        {
                            this.TreeView.ExpandedChanging(this, true);
                        }
                    }

                    return true;
                }
            }

            if (m_checkBox.Visible && this.EnabledButtons && m_checkBox.Bounds.Contains(pt))
            {
                bool differentSelectionBaseNode = (this.TreeView != null && this.TreeView.SelectionBaseNode != this);
                ToggleCheckState((Control.ModifierKeys & Keys.Shift) > 0 && differentSelectionBaseNode);
                return true;
            }

            if (m_optionButton.Visible && this.EnabledButtons && m_optionButton.Bounds.Contains(pt))
            {
                if (!m_optioned && ParentNode != null)
                {
                    TreeNodeAdv optionedChild = ParentNode.GetOptionedChild();

                    if (optionedChild != null)
                    {
                        optionedChild.Optioned = false;
                    }

                    Optioned = true;
                }

                return true;
            }

            return false;
        }

        private CheckState GetToggledState(CheckState current)
        {
            CheckState newState = CheckState.Checked;

            // SIngle node toggle.
            if (current == CheckState.Checked)
            {
                newState = CheckState.Unchecked;
            }
            else if (current == CheckState.Unchecked && this.InteractiveCheckBox
              && m_partialCheckedState != null)
            {
                newState = CheckState.Indeterminate;
            }
            else
            {
                newState = CheckState.Checked;
            }

            return newState;
        }

        [DocumentationExclude()]
        protected void ToggleCheckState(bool multiNodeToggle)
        {
            MultiColumnTreeView tree = this.TreeView;

            if (multiNodeToggle == false || tree.SelectionBaseNode == null)
            {
                this.CheckState = this.GetToggledState(this.CheckState);
            }
            else if (this.TreeView != null)
            {
                TreeNodeAdv selectionBaseNode = tree.SelectionBaseNode;

                CheckState newCheckState = selectionBaseNode.CheckState;

                bool up = (selectionBaseNode.Bounds.Y > this.Bounds.Y);

                ArrayList newNodes = new ArrayList();
                TreeNodeAdv tna = selectionBaseNode;

                // Parse through all the nodes and set the new check state.
                // This will trigger multiple AfterInteractiveChecks events in the tree.
                while (tna != null && tna != this)
                {
                    if (tna.Enabled)
                    {
                        newNodes.Add(tna);

                        // Set the CheckState only if ShowCheckBox is on.
                        if (tna.ShowCheckBox)
                        {
                            tna.CheckState = newCheckState;
                        }
                    }

                    tna = (up ? tna.PrevVisibleNode : tna.NextVisibleNode);
                }

                // Finally toggle my check state as well.
                this.CheckState = newCheckState;
            }
        }
        #endregion

        #region Nodes Changed
        internal void AddedNode(TreeNodeAdv node)
        {
            if (node.ParentNode != this && node.ParentNode != null)
            {
                // Remove this from the current parent.
                node.ParentNode.Nodes.Remove(node);
            }

            // Do the parenting after setting the Visibility, so that
            // the AdjustVisibleNodeCount method doesn't get triggered on myself.
            node.ParentNode = this;

            if (!this.IsRoot)
            {
                node.CheckStateChanged += new EventHandler(childCheckStateChanged);
                UpdateInteractiveCheckState();
            }

            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                // Recursively call this method so that bounds get updated.
                foreach (TreeNodeAdv childNode in node.Nodes)
                {
                    node.AddedNode(childNode);
                }

                if (tree.ShouldPrepareUpdate(false))
                {
                    node.RecalculateDimensions();
                    this.MaxX = Math.Max(node.Right, this.MaxX);

                    // When the first child is added certain widths might change.
                    if (this.Nodes.Count == 1)
                    {
                        RecalculateDimensions();
                    }
                }
            }

            if (node.IsVisible && this.TreeView !=null && !this.TreeView.NodeInRefresh)
            {
                AdjustVisibleNodeCount(node.VisibleNodeCount);
            }

            UpdatePlusMinusVisibility();
            node.UpdateAllPlusMinusVisibility();
        }

        internal void RemovedNodes(ArrayList nodes)
        {
            foreach (TreeNodeAdv node in nodes)
            {
                RemovedNode(node);
            }

            MakeDirty();
        }
        internal void RemovedNode(TreeNodeAdv node)
        {
            if (this.Visible & this.Expanded)
            {
                AdjustVisibleNodeCount(node.VisibleNodeCount);
            }

            // notify tree that node removed. (Selection updates)
            if (this.TreeView != null)
            {
                this.TreeView.RemovedNode(node);
            }

            // Necessary to make the parent null and hide it b'cos
            // the VisibileNodeCount needs to be udpated appropriately
            // so that the logic will work when adding the node back to the tree.
            node.ParentNode = null;

            // This will trigger recalculating the maxX.
            this.childMaxXChanged(0);

            // Caching Partial-Checked-State related
            if (this.InteractiveCheckBox)
            {
                if (m_partialCheckedState != null)
                {
                    m_partialCheckedState.Remove(node);

                    if (m_partialCheckedState.Count == 0)
                    {
                        m_partialCheckedState = null;
                    }
                }

                UpdateInteractiveCheckState();
            }
        }

        private void AdjustSelectedNode(int prevIndex)
        {
            TreeNodeAdv newSelectedNode = null;

            // No children so make myself the selected node.
            if (!this.HasNodes && !this.IsRoot)
            {
                if (this.Enabled)
                {
                    newSelectedNode = this;
                }
            }

            if (this.HasNodes)
            {
                if (newSelectedNode == null)
                {
                    // There were children so use the prev index to determine the new selection.
                    if (prevIndex >= this.Nodes.Count)
                    {
                        prevIndex = this.Nodes.Count - 1;
                    }

                    if (prevIndex >= 0)
                    {
                        TreeNodeAdv newNode = this.Nodes[prevIndex];

                        while ((newNode.ParentNode != this || !newNode.Enabled) && ++prevIndex < this.Nodes.Count)
                        {
                            newNode = this.Nodes[prevIndex];
                        }

                        if (newNode.ParentNode == this && newNode.Enabled)
                        {
                            newSelectedNode = newNode;
                        }
                    }
                }
            }

            if (newSelectedNode == null)
            {
                // Still can't find a selectable node, so parse down and up.
                TreeNodeAdv selectableNode = this.NextSelectableNode;

                if (selectableNode == null)
                {
                    selectableNode = this.PrevSelectableNode;
                }

                newSelectedNode = selectableNode;
            }

            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                if (tree.SetSelectedNode(newSelectedNode, tree.SelectedNodes, TreeViewAdvAction.Unknown))
                {
                    tree.ActiveNode = newSelectedNode;
                    tree.SetSelectionBaseNode(newSelectedNode);
                }
                else
                {
                    tree.SetSelectedNode(null, tree.SelectedNodes, TreeViewAdvAction.Unknown, false, true);
                    tree.ActiveNode = null;
                }
            }
        }

        private void UpdateSelectedNodeIndexCache()
        {
            MultiColumnTreeView tree = this.TreeView;

            if (tree != null && tree.SelectedNode != null &&
              tree.SelectedNode.ParentNode == this)
            {
                if (this.Nodes.IndexOf(tree.SelectedNode) == -1)
                {
                    tree.SetSelectedNode(null, tree.SelectedNodes, TreeViewAdvAction.Unknown, false, true);
                    tree.ActiveNode = null;
                }
                else
                {
                    tree.m_currentSelectedNodeIndex = this.Nodes.IndexOf(tree.SelectedNode);
                }
            }
        }
        #endregion

        #region Drawing

        protected internal bool ShouldDrawPlusMinus()
        {
            return (m_plusMinus.Visible &&
              ((this.ParentNode != null && ParentNode.Expanded && this.HasNodes) ||
              this.TreeView.LoadOnDemand && !m_expandedOnce));
        }

        protected internal Color GetForeColor(bool selected, bool hotTracked)
        {
            MultiColumnTreeView tree = this.TreeView;

            if (tree != null)
            {
                if (selected)
                {
                    if (tree.Focused)
                    {
                        return tree.SelectedNodeForeColor;
                    }
                    else if (!tree.HideSelection)
                    {
                        return tree.InactiveSelectedNodeForeColor;
                    }
                }

                if (!tree.Enabled || !this.Enabled)
                {
                    return SystemColors.GrayText;
                }
                else
                {
                    return hotTracked ? SystemColors.HotTrack : this.TextColor;
                }
            }

            return NodeStyle.TextColor;
        }

        protected internal void Draw(ThemedControlDrawing treeTD, ThemedControlDrawing buttonTD,
          Pen linePen, Point mousePos, bool mouseDown, TreeNodeAdvPaintEventArgs e)
        {
            MultiColumnTreeView tree = this.TreeView;

            bool bIsMirrored = tree.GetIsMirrored();

            DrawHorizontalLine(e, bIsMirrored, linePen);
            DrawLeftImageList(e, bIsMirrored);
            DrawStateImageList(e, bIsMirrored);

            DrawText(e);

            // if FullRowSelect mode than TreeView draw FocusRect
            if (e.Active && !tree.FullRowSelect)
            {
                DrawFocusRect(e.Graphics);
            }

            DrawRightImageList(e, bIsMirrored);
            DrawControls(e, treeTD, buttonTD, mousePos, mouseDown);
        }

        private void DrawHorizontalLine(TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored, Pen linePen)
        {
            MultiColumnTreeView tvaTree = this.TreeView;

            // Draw the Horizontal line, if necessary
            if (this.Level != 1 || tvaTree.ShowRootLines)
            {
                int left = 0;
                int right = 0;

                if (bIsMirrored)
                {
                    int nOffset = this.Bounds.Right - m_nodeXRel;
                    right = nOffset - (m_plusMinus.Width + 1) / 2;
                    left = nOffset - m_lineRightRel;
                }
                else
                {
                    int nOffset = this.Bounds.X + m_nodeXRel;
                    left = nOffset + (m_plusMinus.Width + 1) / 2;
                    right = nOffset + m_lineRightRel;
                }

                if (ParentNode != null && tvaTree.ShowLines)
                {
                    if (this.HasNodes && m_nodeData.ShowPlusMinus)
                    {
                        if (bIsMirrored)
                        {
                            right -= m_plusMinus.Width / 2;
                        }
                        else
                        {
                            left += m_plusMinus.Width / 2;
                        }
                    }

                    int nLineY = Bounds.Y + NodeStyle.Height / 2;
                    eaEventArgs.Graphics.DrawLine(linePen, left, nLineY, right, nLineY);
                }
            }
        }
        private void DrawLeftImageList(TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored)
        {
            MultiColumnTreeView tvaTree = this.TreeView;
            ImageList leftImageList = tvaTree.LeftImageList;
            Image leftSide = this.LeftImage;

            // image has higher priority then image list and it indexes
            if (leftSide != null)
            {
                int x = 0;
                int nY = Bounds.Y + (NodeStyle.Height - Math.Min(leftSide.Height, NodeStyle.Height)) / 2;
                int nInc = m_nodeXRel + m_leftImageListXRel;

                if (bIsMirrored)
                {
                    x = this.Bounds.Right - nInc;
                    x -= LeftImagePadding;
                }
                else
                {
                    x = this.Bounds.X + nInc;
                    x += LeftImagePadding;
                }

                Rectangle source = new Rectangle(Point.Empty, leftSide.Size);
                Rectangle destination = new Rectangle(
                  x + ((bIsMirrored) ? -leftSide.Width : 0),
                  nY, leftSide.Width, Math.Min(leftSide.Height, NodeStyle.Height));
                eaEventArgs.Graphics.DrawImage(leftSide, destination, source, GraphicsUnit.Pixel);
            }
            else if (leftImageList != null && !eaEventArgs.HandledLeftImageList)
            {
                int x = 0;
                if (m_leftImageListXRel != int.MinValue)
                {
                    int nInc = m_nodeXRel + m_leftImageListXRel;

                    if (bIsMirrored)
                    {
                        x = this.Bounds.Right - nInc;
                    }
                    else
                    {
                        x = this.Bounds.X + nInc;
                    }

                    int nImgWidth = leftImageList.ImageSize.Width;
                    int nY = Bounds.Y + (NodeStyle.Height - leftImageList.ImageSize.Height) / 2;

                    int[] indexes = this.LeftImageIndices;
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        int index = indexes[i];
                        if (index >= 0 && index < leftImageList.Images.Count)
                        {
                            if (bIsMirrored)
                            {
                                x -= nImgWidth;
                                x -= LeftImagePadding;
                            }

                            eaEventArgs.Graphics.DrawImage(leftImageList.Images[index], x, nY);

                            if (!bIsMirrored)
                            {
                                x += nImgWidth;
                                x += LeftImagePadding;
                            }
                        }
                    }
                }
            }
        }

        private void DrawStateImageList(TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored)
        {
            MultiColumnTreeView tvaTree = this.TreeView;
            ImageList stateImgList = tvaTree.StateImageList;
            Image stateImg = (!this.HasNodes && !(tvaTree.LoadOnDemand && !m_expandedOnce)) ? this.NoChildrenImage
              : ((Expanded) ? this.OpenImage : this.ClosedImage);

            if (stateImg != null)
            {
                int x = 0;
                int nInc = m_nodeXRel + m_stateImageListXRel;
                int nY = Bounds.Y + (NodeStyle.Height - Math.Min(stateImg.Height, NodeStyle.Height)) / 2;

                if (bIsMirrored)
                {
                    x = this.Bounds.Right - nInc;
                    x -= LeftStateImagePadding;
                }
                else
                {
                    x = this.Bounds.X + nInc;
                    x += LeftStateImagePadding;
                }

                Rectangle source = new Rectangle(Point.Empty, stateImg.Size);
                Rectangle destination = new Rectangle(
                  x + ((bIsMirrored) ? -stateImg.Width : 0),
                  nY, stateImg.Width, Math.Min(stateImg.Height, NodeStyle.Height));
                eaEventArgs.Graphics.DrawImage(stateImg, destination, source, GraphicsUnit.Pixel);
            }
            else if (stateImgList != null && !eaEventArgs.HandledStateImageList && m_stateImageListXRel != int.MinValue)
            {
                int imgIndex = 0;

                if (!this.HasNodes && !(tvaTree.LoadOnDemand && !m_expandedOnce))
                {
                    imgIndex = this.NoChildrenImgIndex;
                }
                else
                {
                    if (Expanded)
                    {
                        imgIndex = this.OpenImgIndex;
                    }
                    else
                    {
                        imgIndex = this.ClosedImgIndex;
                    }
                }

                if (imgIndex >= 0 && imgIndex < stateImgList.Images.Count)
                {
                    int x = 0;
                    int nInc = m_stateImageListXRel + m_nodeXRel;

                    if (bIsMirrored)
                    {
                        x = this.Bounds.Right - nInc - stateImgList.ImageSize.Width;
                        x -= LeftStateImagePadding;
                    }
                    else
                    {
                        x = this.Bounds.X + nInc;
                        x += LeftStateImagePadding;
                    }

                    eaEventArgs.Graphics.DrawImage(
                      stateImgList.Images[imgIndex], x,
                      Bounds.Y + (NodeStyle.Height - stateImgList.ImageSize.Height) / 2);
                }
            }
        }

        /// <summary>
        /// Calculates width of string which must be drawn with specified font.
        /// </summary>
        /// <param name="g">Context device for drawing.</param>
        /// <param name="f">Specified font.</param>
        /// <param name="width">Limit measuring by width.</param>
        /// <returns>Width of specified string in pixel.</returns>
        private Size GetNodeTextSize(Graphics g, Font f, int width)
        {
            Size size = Size.Empty;

            if (this.Text != null && this.Text.Length > 0)
            {
                size = ControlDrawing.MeasureDisplayStringSize(g, this.Text, f, GetIsMirrored(), width);
            }

            size.Width += c_nDisplayNodeTextWidthPadding;

            return size;
        }

        /// <summary>
        /// Calculates width of string which must be drawn with specified font.
        /// </summary>
        /// <param name="g">Context device for drawing.</param>
        /// <param name="f">Specified font.</param>
        /// <returns>Width of specified string in pixel.</returns>
        private Size GetNodeTextSize(Graphics g, Font f)
        {
            Size size = Size.Empty;

            if (this.Text != null && this.Text.Length > 0)
            {
                size = ControlDrawing.MeasureDisplayStringSize(g, this.Text, f, GetIsMirrored());
            }

            size.Width += c_nDisplayNodeTextWidthPadding;

            return size;
        }


        private void DrawText(TreeNodeAdvPaintEventArgs eaEventArgs)
        {
            MultiColumnTreeView tvaTree = this.TreeView;
            Graphics g = eaEventArgs.Graphics;

            if (m_textLocationXRel != int.MinValue)
            {
                if (tvaTree.Printing)
                {
                    Point textLocation = this.PrintTextBounds.Location;
                    int offsetX = PrintTextBounds.Width - TextBounds.Width;
                    if (offsetX > 0 && GetIsMirrored())
                    {
                        textLocation.Offset(-offsetX, 0);
                    }

                    using (Brush brush = new SolidBrush(eaEventArgs.ForeColor))
                    {
                        g.DrawString(this.Text, this.Font, brush, textLocation);
                    }
                }
                else
                {
                    if (!eaEventArgs.HandledText && (!tvaTree.IsEditing || tvaTree.ActiveNode != this))
                    {
                        Point textLocation = new Point(this.PrintTextBounds.X, this.PrintTextBounds.Y);
                        
						if(!this.Multiline)
							textLocation.Y = (textLocation.Y + this.NodeStyle.Height / 2 - this.Font.Height / 2);
                        using (Brush brush = new SolidBrush(eaEventArgs.ForeColor))
                        {
                            g.DrawString(this.Text, this.Font, brush, textLocation);
                        }
                    }
                }
            }
        }

        /// <summary>Draw text by native GDI API.</summary>
        /// <param name="g">graphics which handle we have to use.</param>
        /// <param name="f">Font which we have to use for text drawing.</param>
        /// <param name="color">Text color.</param>
        private void DrawTextInternal(Graphics g, Font f, Color color)
        {
            IntPtr clipRgn = g.Clip.GetHrgn(g);
            IntPtr hdc = g.GetHdc();
            IntPtr hFont = f.ToHfont();
            try
            {
                IntPtr prevFont = NativeMethods.SelectObject(hdc, hFont);

                NativeMethods.SelectClipRgn(hdc, clipRgn);
                NativeMethods.SetTextColor(hdc, color.ToArgb() & 0xFFFFFF);
                NativeMethods.SetBkMode(hdc, 1); // TRANSPARENT

                Rectangle rcText = this.TextBounds;
                NativeMethods.RECT rect = new NativeMethods.RECT(rcText);
                rect.left += c_nDisplayNodeTextWidthPadding / 2;
                int nFlags = c_nDrawTextFlags;

                if (!this.Multiline)
                {
                    nFlags |= DrawTextFormats.DT_SINGLELINE | DrawTextFormats.DT_VCENTER;
                }

                if (GetIsMirrored())
                {
                    nFlags |= DrawTextFormats.DT_RTLREADING;
                }

                nFlags |= DrawTextFormats.DT_END_ELLIPSIS;

                NativeMethods.DrawText(hdc, this.Text, this.Text.Length, ref rect, nFlags);

                prevFont = NativeMethods.SelectObject(hdc, prevFont);
                NativeMethods.SelectClipRgn(hdc, IntPtr.Zero);
            }
            finally
            {
                NativeMethods.DeleteObject(clipRgn);
                NativeMethods.DeleteObject(hFont);
                g.ReleaseHdc(hdc);
            }
        }

        /// <summary>
        /// Draws dotted border around selected node.
        /// This will be used to fast drawing when TreeCtrl loses focus.
        /// </summary>
        /// <param name="g">Device context needed for drawing.</param>
        protected internal void DrawFocusRect(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            MultiColumnTreeView tvaTree = this.TreeView;

            if (tvaTree == null)
            {
                throw new ArgumentNullException("tvaTree");
            }

            Rectangle rect;

            if (!tvaTree.Focused && !tvaTree.KeepDottedSelection)
            {
                return;
            }

            if (tvaTree.FullRowSelect)
            {
                rect = Bounds;

                rect.Inflate(Bounds.X, 0);
                rect.Width -= Bounds.X;
            }
            else
            {
                rect = tvaTree.Printing ? this.PrintTextBounds : this.TextBounds;
            }

            if (!this.IsEditing)
            {
                if (g.ClipBounds.IntersectsWith(rect))
                {
                    ControlPaint.DrawFocusRectangle(g, rect,
                      this.TextColor, this.Background.BackColor);
                }
            }
        }

        private void DrawRightImageList(TreeNodeAdvPaintEventArgs eaEventArgs, bool bIsMirrored)
        {
            MultiColumnTreeView tvaTree = this.TreeView;
            ImageList rightImgList = tvaTree.RightImageList;
            Image rightSide = this.RightImage;

            if (rightSide != null)
            {
                int x = 0;
                int nInc = m_rightImageListXRel + m_nodeXRel;
                int nY = Bounds.Y + (this.Height - Math.Min(this.Height, rightSide.Size.Height)) / 2;

                if (bIsMirrored)
                {
                    x = this.Bounds.Right - nInc;
                    x -= RightImagePadding;
                }
                else
                {
                    x = this.Bounds.X + nInc;
                    x += RightImagePadding;
                }

                Rectangle source = new Rectangle(Point.Empty, rightSide.Size);
                Rectangle destination = new Rectangle(
                  x + ((bIsMirrored) ? -rightSide.Width : 0),
                  nY, rightSide.Width, Math.Min(this.Height, rightSide.Size.Height));

                eaEventArgs.Graphics.DrawImage(rightSide, destination, source, GraphicsUnit.Pixel);
            }
            else if (rightImgList != null && !eaEventArgs.HandledRightImageList)
            {
                if (m_rightImageListXRel != int.MinValue)
                {
                    int x = 0;
                    int nInc = m_rightImageListXRel + m_nodeXRel;

                    if (bIsMirrored)
                    {
                        x = this.Bounds.Right - nInc;
                    }
                    else
                    {
                        x = this.Bounds.X + nInc + RightImagePadding;
                    }

                    int nImgWidth = rightImgList.ImageSize.Width;
                    int nY = Bounds.Y + (this.Height - rightImgList.ImageSize.Height) / 2;

                    int[] indexes = this.RightImageIndices;

                    if (tvaTree.Printing)
                    {
                        int offsetX = this.PrintTextBounds.Width - this.TextBounds.Width;
                        if (offsetX > 0)
                        {
                            x = bIsMirrored ? x - offsetX : x + offsetX;
                        }
                    }

                    for (int i = 0; i < indexes.Length; i++)
                    {
                        int index = indexes[i];
                        if (index >= 0 && index < rightImgList.Images.Count)
                        {
                            if (bIsMirrored)
                            {
                                x -= nImgWidth;
                                x -= RightImagePadding;
                            }

                            eaEventArgs.Graphics.DrawImage(rightImgList.Images[index], x, nY);

                            if (!bIsMirrored)
                            {
                                x += nImgWidth;
                                x += RightImagePadding;
                            }
                        }
                    }
                }
            }
        }

        private void DrawControls(TreeNodeAdvPaintEventArgs e, ThemedControlDrawing treeTD, ThemedControlDrawing buttonTD, Point pt, bool mouseDown)
        {
            // Draw the themed controls
            if (this.NodeStyle.ThemesEnabled && XPThemes.IsThemedOS &&
              XPThemes.IsAppThemed && XPThemes.IsThemeActive)
            {
                // Draw the plus minus.
                if (!e.HandledPlusMinus && this.ShouldDrawPlusMinus())
                {
                    int stateID = (!Expanded || (this.ShowPlusOnExpand && this.TreeView.LoadOnDemand)) ? 1 : 2;
                    treeTD.DrawThemeBackground(e.Graphics, 2, stateID, m_plusMinus.Bounds);
                }

                DrawThemedCheckBoxControl(e, buttonTD, pt, mouseDown);
                DrawThemedOptionButtonControl(e, buttonTD, pt, mouseDown);
            }
            else
            {
                DrawPlusMinus(e);
                DrawCheckBox(e);
                DrawOptionButton(e);
            }
        }

        private void DrawOptionButton(TreeNodeAdvPaintEventArgs e)
        {
            // Draw the optionbutton.
            if (!e.HandledOptionButton && m_optionButton.Visible && ParentNode != null && ParentNode.Expanded &&
              m_optionButton.M_xIndentFromNodeLeft != int.MinValue)
            {
                ButtonState bstate = ButtonState.Checked;
                if (!m_optioned)
                {
                    bstate = ButtonState.Normal;
                }
                if (!this.Enabled || !this.EnabledButtons)
                {
                    bstate |= ButtonState.Inactive;
                }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                ControlPaint.DrawRadioButton( e.Graphics, m_optionButton.Bounds, bstate );
#else
                ControlPaintHelper.DrawRadioButton(e.Graphics, m_optionButton.Bounds, bstate, this);
#endif
            }
        }

        private void DrawCheckBox(TreeNodeAdvPaintEventArgs e)
        {
            // Draw the checkbox.
            if (!e.HandledCheckBox && m_checkBox.Visible && ParentNode != null && ParentNode.Expanded &&
              m_checkBox.M_xIndentFromNodeLeft != int.MinValue)
            {
                ButtonState bstate = ButtonState.Normal;

                switch (this.CheckState)
                {
                    case CheckState.Checked:
                        bstate = ButtonState.Checked;
                        break;

                    case CheckState.Indeterminate:
                        bstate = ButtonState.All;
                        break;
                }

                if (!this.Enabled || !this.EnabledButtons)
                {
                    bstate |= ButtonState.Inactive;
                }

                ControlPaintHelper.DrawCheckBox(e.Graphics, m_checkBox.Bounds, bstate, this);
            }
        }

        private void DrawPlusMinus(TreeNodeAdvPaintEventArgs e)
        {
            bool transparent = this.TreeView.TransparentControls;
            Brush backBrush = new SolidBrush(TreeView.BackColor);

            // Draw the plusminus.
            if (!e.HandledPlusMinus && this.ShouldDrawPlusMinus())
            {
                Graphics g = e.Graphics;
                int width = m_plusMinus.Width;
                int height = m_plusMinus.Height;
                Rectangle rc = new Rectangle(m_plusMinus.Location.X, m_plusMinus.Location.Y, width - 1, height - 1);

                if (!transparent)
                {
                    g.FillRectangle(backBrush, m_plusMinus.Bounds);
                }

                if (ExpandImage != null && this.Expanded)
                {
                    g.DrawImage(this.ExpandImage, rc);
                }
                else
                {
                    if (Expanded)
                    {
                        g.DrawRectangle(Pens.Gray, rc);
                        this.TreeView.ExcludedDrawingRegion.Exclude(rc);
                    }
                }
                if (this.Expanded ? ExpandImage == null : CollapseImage == null)
                {
                    g.DrawLine(Pens.Black, m_plusMinus.Location.X + 2, m_plusMinus.Location.Y + height / 2,
                      m_plusMinus.Location.X + width - 3, m_plusMinus.Location.Y + height / 2);
                }

                if (!Expanded || (this.ShowPlusOnExpand && this.TreeView.LoadOnDemand))
                {
                    if (CollapseImage != null)
                    {
                        g.DrawImage(this.CollapseImage, rc);
                    }
                    else
                    {
                        g.DrawRectangle(Pens.Gray, rc);
                        g.DrawLine(Pens.Black, m_plusMinus.Location.X + width / 2, m_plusMinus.Location.Y + 2,
                          m_plusMinus.Location.X + width / 2, m_plusMinus.Location.Y + height - 3);
                        this.TreeView.ExcludedDrawingRegion.Exclude(rc);
                    }
                }
            }

            backBrush.Dispose();
        }

        private void DrawThemedOptionButtonControl(TreeNodeAdvPaintEventArgs e, ThemedControlDrawing buttonTD, Point pt, bool mouseDown)
        {
            // Draw the OptionButton
            if (!e.HandledOptionButton && m_optionButton.Visible && ParentNode != null && ParentNode.Expanded)
            {
                int partNr = 1;

                if (m_optioned)
                {
                    partNr = 5;
                }

                if (TreeView.Enabled && this.Enabled && this.EnabledButtons)
                {
                    if (m_optionButton.Bounds.Contains(pt))
                    {
                        partNr += ((mouseDown) ? 2 : 1);
                    }
                }
                else
                {
                    partNr += 3;
                }

                buttonTD.DrawThemeBackground(e.Graphics, 2, partNr, m_optionButton.Bounds);
            }
        }


        private void DrawThemedCheckBoxControl(TreeNodeAdvPaintEventArgs e, ThemedControlDrawing buttonTD, Point pt, bool mouseDown)
        {
            // Draw the checkbox
            if (!e.HandledCheckBox && m_checkBox.Visible && ParentNode != null && ParentNode.Expanded)
            {
                // Calculating the part needed
                int partNr = 1;
                switch (this.CheckState)
                {
                    case CheckState.Unchecked:
                        partNr = 1;
                        break;
                    case CheckState.Checked:
                        partNr = 5;
                        break;
                    case CheckState.Indeterminate:
                        partNr = 9;
                        break;
                }

                if (TreeView.Enabled && this.Enabled && this.EnabledButtons)
                {
                    if (m_checkBox.Bounds.Contains(pt))
                    {
                        partNr += ((mouseDown) ? 2 : 1);
                    }
                }
                else
                {
                    partNr += 3;
                }

                buttonTD.DrawThemeBackground(e.Graphics, 3, partNr, m_checkBox.Bounds);
            }
        }
        #endregion

        #region Layouting
        /// <summary></summary>
        /// <remarks>Calculating only the width, not the locations (as we don't know the Y)</remarks>
        private void RecalculateDimensions()
        {
            if (this.TreeView == null)
            {
                return;
            }

            MultiColumnTreeView tree = TreeView;

            int parentIndent = tree.Indent;
            m_nodeXRel = (Level - 1) * parentIndent;

            int width = 0;
            bool leftMostPartFound = false;
            int pmWidthWithSpace = 0;

            if (this.Level != 1 || tree.NeedRootLinesSpace)
            {
                if (this.NodeStyle.ShowPlusMinus)
                {
                    PlusMinus.M_xIndentFromNodeLeft = 0;
                }
                // Whether or not we show plus-minus:
                width += m_plusMinus.Width + spc;

                pmWidthWithSpace = width;

                if (width < parentIndent)
                {
                    width = parentIndent + m_plusMinus.Width / 2;
                }
            }

            if (this.Level != 1 && !tree.NeedRootLinesSpace)
            {
                m_nodeXRel -= parentIndent;
                m_nodeXRel += m_plusMinus.Width / 2;
            }

            m_lineRightRel = width - 1;

            if (m_primitives != null && m_primitives.Count > 0)
            {
                LayoutPrimitives(width);
            }
            else
            {
                #region CheckBox layouting logic
                if (this.NodeStyle.ShowCheckBox)
                {
                    if (!leftMostPartFound)
                    {
                        leftMostPartFound = true;
                        // If possible center the vertical line to the checkbox 
                        if (width - CheckBox.Width / 2 > pmWidthWithSpace)
                        {
                            width -= CheckBox.Width / 2;
                        }
                        m_lineRightRel = width - 1;
                    }
                    CheckBox.M_xIndentFromNodeLeft = width;
                    width += CheckBox.Width + spc;
                }
                #endregion

                #region ShowOptionButton layouting logic
                if (this.ShowOptionButton)
                {
                    if (!leftMostPartFound)
                    {
                        leftMostPartFound = true;
                        // If possible center the vertical line to the option button 
                        if (width - OptionButton.Width / 2 > pmWidthWithSpace)
                        {
                            width -= OptionButton.Width / 2;
                        }
                        m_lineRightRel = width - 1;
                    }

                    OptionButton.M_xIndentFromNodeLeft = width;

                    width += OptionButton.Width + spc;
                }
                #endregion

                #region LeftImage layouting logic
                ImageList leftImageList = tree.LeftImageList;

                if (this.LeftImage != null)
                {
                    // NOTE: required for proper horizontal line end point caclulation
                    if (!leftMostPartFound)
                    {
                        leftMostPartFound = true;

                        // If possible center the vertical line to the Left Image 
                        if (width - this.LeftImage.Size.Width / 2 > pmWidthWithSpace)
                        {
                            width -= this.LeftImage.Size.Width / 2;
                        }

                        m_lineRightRel = width - 1;
                    }

                    m_leftImageListXRel = width;
                    width += this.LeftImage.Size.Width;
                    width += LeftImagePadding;
                    width += spc;
                }
                else if (leftImageList != null)
                {
                    int[] indexes = this.LeftImageIndices;
                    bool atleast1 = false;
                    int leftWidth = leftImageList.ImageSize.Width;
                    int maxImagesCount = leftImageList.Images.Count;

                    for (int i = 0; i < indexes.Length; i++)
                    {
                        int index = indexes[i];

                        if (index >= 0 && index < maxImagesCount)
                        {
                            if (!leftMostPartFound)
                            {
                                leftMostPartFound = true;

                                // If possible center the vertical line to the Left Image 
                                if (width - leftWidth / 2 > pmWidthWithSpace)
                                {
                                    width -= leftWidth / 2;
                                }

                                m_lineRightRel = width - 1;
                            }

                            if (!atleast1)
                            {
                                m_leftImageListXRel = width;
                                atleast1 = true;
                            }

                            width += leftWidth;
                            width += LeftImagePadding;
                        }
                    }

                    if (atleast1)
                    {
                        width += spc;
                    }
                }
                #endregion

                #region StateImage layouting logic
                ImageList stateImgList = tree.StateImageList;
                Image stateImg = (!this.HasNodes && !(TreeView.LoadOnDemand && !m_expandedOnce)) ? this.NoChildrenImage :
                  ((Expanded) ? this.OpenImage : this.ClosedImage);

                if (stateImg != null)
                {
                    if (!leftMostPartFound)
                    {
                        leftMostPartFound = true;
                        // If possible center the vertical line to the state Image. 
                        if (width - stateImg.Width / 2 > pmWidthWithSpace)
                        {
                            width -= stateImg.Width / 2;
                        }
                        m_lineRightRel = width - 1;
                    }
                    m_stateImageListXRel = width;
                    width += stateImg.Width + spc;
                    width += LeftStateImagePadding + RightStateImagePadding;
                }
                else if (stateImgList != null)
                {
                    int imgIndex = 0;
                    if (!this.HasNodes && !(TreeView.LoadOnDemand && !m_expandedOnce))
                    {
                        imgIndex = this.NodeStyle.NoChildrenImgIndex;
                    }
                    else
                    {
                        if (Expanded)
                        {
                            imgIndex = this.NodeStyle.OpenImgIndex;
                        }
                        else
                        {
                            imgIndex = this.NodeStyle.ClosedImgIndex;
                        }
                    }

                    if (imgIndex >= 0 && imgIndex < stateImgList.Images.Count)
                    {
                        if (!leftMostPartFound)
                        {
                            leftMostPartFound = true;
                            // If possible center the vertical line to the state Image. 
                            if (width - stateImgList.ImageSize.Width / 2 > pmWidthWithSpace)
                            {
                                width -= stateImgList.ImageSize.Width / 2;
                            }
                            m_lineRightRel = width - 1;
                        }
                        m_stateImageListXRel = width;
                        width += stateImgList.ImageSize.Width + spc;
                        width += LeftStateImagePadding + RightStateImagePadding;
                    }
                }
                #endregion

                if (!leftMostPartFound)
                {
                    leftMostPartFound = true;

                    // If possible make the text start a few pixels to the left of the vertical line 
                    if (width - 9 > pmWidthWithSpace)
                    {
                        width -= 9;
                    }

                    m_lineRightRel = width - 1;
                }

                m_textLocationXRel = width;

                Graphics g = this.TreeView.MeasureGraphics; // cached Graphics for measuring

                if (this.Multiline)
                {
                    int limit = this.TreeView.TreeColumnRectangle.Width - m_nodeXRel -
                        m_textLocationXRel - 2;

                    Size textBounds = GetNodeTextSize(g, m_nodeData.Font, limit);
                    m_iTextWidth = textBounds.Width;

                    if (this.TreeView.AutoAdjustMultiLineHeight)
                    {
                        Height = textBounds.Height;
                    }

                    // TODO: calculate m_printTextSize correctly
                }
                else
                {
                    m_iTextWidth = GetNodeTextSize(g, m_nodeData.Font).Width;
                    m_printTextSize = Size.Round(g.MeasureString(this.Text, m_nodeData.Font));
                }

                if (m_iTextWidth > 0)
                {
                    width += m_iTextWidth + spc;
                }

                #region RightImage layouting logic
                m_rightImageListXRel = width;

                if (this.RightImage != null)
                {
                    width += this.RightImage.Size.Width + spc;
                    width += RightImagePadding;
                }
                else if (tree.RightImageList != null)
                {
                    width += tree.RightImageList.ImageSize.Width * m_nodeData.RightImageIndices.Length + spc;
                    width += m_nodeData.RightImageIndices.Length * RightImagePadding;
                }
                #endregion

                if (this.CustomControl != null)
                {
                    m_customControlRelativeLocation = width;
                    width += this.CustomControl.Width;
                }

                this.Width = width;
                int myMax = this.Bounds.X + m_nodeXRel + width;

                if (!this.HasNodes)
                {
                    this.MaxX = myMax;
                }
                else
                {
                    // If, in the middle of recalculating, ignore this.
                    if (htRecalculatingNodes[this] == null)
                    {
                        RecalculateMaxX();
                    }
                    else if (myMax > this.MaxX)
                    {
                        this.MaxX = myMax;
                    }
                }
            }
        }

        /// <summary>
        /// Be very discrete about calling this, as it could cause performance problems.
        /// </summary>
        private void RecalculateMaxX()
        {
            int max = spc + (Level - 1) * ParentIndent + this.Width;

            if (this.Expanded && this.HasNodes)
            {
                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    max = Math.Max(max, this.Nodes[i].MaxX);
                }
            }

            this.MaxX = max;
        }

        private bool LayoutCustomControl(ref int relativeLocation)
        {
            int indent = 0;

            m_customControlRelativeLocation = relativeLocation;

            return LayoutPrimitive(ref relativeLocation,
              ref indent, 0,
              this.CustomControl.Width, true);
        }

        private void LayoutPrimitives(int width)
        {
            bool leftMostPartFound = false;
            int pmWidthWithSpace = 0;

            // sort primitives by their indexes
            Hashtable htItems = new Hashtable();
            ArrayList arrItems = new ArrayList();

            for (int i = 0, len = m_primitives.Count; i < len; i++)
            {
                TreeNodePrimitive pt = m_primitives[i];
                ArrayList arrItemsWithSameIndex = (ArrayList)htItems[pt.Index];

                if (arrItemsWithSameIndex == null)
                {
                    arrItemsWithSameIndex = new ArrayList();
                    arrItems.Add(pt.Index);
                }

                arrItemsWithSameIndex.Add(pt);

                htItems[pt.Index] = arrItemsWithSameIndex;
            }

            arrItems.Sort();

            CheckBox.M_xIndentFromNodeLeft = -1;
            CheckBox.M_xIndentFromNodeLeft = int.MinValue;
            OptionButton.M_xIndentFromNodeLeft = int.MinValue;
            m_leftImageListXRel = int.MinValue;
            m_rightImageListXRel = int.MinValue;
            m_stateImageListXRel = int.MinValue;
            m_textLocationXRel = int.MinValue;

            for (int k = 0, len = arrItems.Count; k < len; k++)
            {
                ArrayList itemsList = htItems[arrItems[k]] as ArrayList;

                if (itemsList == null || itemsList.Count == 0)
                {
                    continue;
                }

                foreach (TreeNodePrimitive pt in itemsList)
                {
                    switch (pt.PrimitiveType)
                    {
                        case PredefinedPrimitiveTypes.CheckBox:
                            if (NodeStyle.ShowCheckBox)
                            {
                                leftMostPartFound = LayoutCheckBox(ref width,
                                  pmWidthWithSpace, leftMostPartFound);
                            }
                            break;

                        case PredefinedPrimitiveTypes.OptionsButton:
                            if (NodeStyle.ShowOptionButton)
                            {
                                leftMostPartFound = LayoutOptionsButton(ref width,
                                  pmWidthWithSpace, leftMostPartFound);
                            }
                            break;

                        case PredefinedPrimitiveTypes.LeftImages:
                            m_leftImageListXRel = width;

                            leftMostPartFound = LayoutImages(TreeView.LeftImageList,
                              NodeStyle.LeftImageIndices, ref width, pmWidthWithSpace,
                              LeftImagePadding, leftMostPartFound);
                            break;

                        case PredefinedPrimitiveTypes.RightImages:
                            m_rightImageListXRel = width;

                            leftMostPartFound = LayoutImages(TreeView.RightImageList,
                              NodeStyle.RightImageIndices, ref width, pmWidthWithSpace,
                              RightImagePadding, leftMostPartFound);
                            break;

                        case PredefinedPrimitiveTypes.StateImage:
                            int imgIndex = 0;

                            if (!this.HasNodes && !(TreeView.LoadOnDemand && !m_expandedOnce))
                            {
                                imgIndex = this.NodeStyle.NoChildrenImgIndex;
                            }
                            else
                            {
                                imgIndex = (Expanded) ? NodeStyle.OpenImgIndex : NodeStyle.ClosedImgIndex;
                            }

                            leftMostPartFound = LayoutStateImage(TreeView.StateImageList, imgIndex, ref width,
                              pmWidthWithSpace, LeftStateImagePadding + RightStateImagePadding,
                              leftMostPartFound);
                            break;

                        case PredefinedPrimitiveTypes.Text:
                            m_textLocationXRel = width;
                            leftMostPartFound = LayoutText(ref width, leftMostPartFound);
                            break;

                        case PredefinedPrimitiveTypes.CustomControl:
                            if (this.CustomControl != null)
                            {
                                leftMostPartFound = LayoutCustomControl(ref width);
                            }
                            break;
                    }
                }
            }

        }

        private bool LayoutPrimitive(ref int relativeLocation, ref int indent, int pmWidthWithSpace,
          int primitiveWidth, bool leftMostPartFound)
        {
            if (!leftMostPartFound)
            {
                leftMostPartFound = true;

                // If possible center the vertical line to the checkbox 
                if (relativeLocation - primitiveWidth / 2 > pmWidthWithSpace)
                {
                    relativeLocation -= primitiveWidth / 2;
                }

                m_lineRightRel = relativeLocation - 1;
            }

            indent = relativeLocation;

            relativeLocation += primitiveWidth + spc;

            return leftMostPartFound;
        }

        private bool LayoutCheckBox(ref int relativeLocation, int pmWidthWithSpace, bool leftMostPartFound)
        {
            return LayoutPrimitive(ref relativeLocation,
              ref CheckBox.M_xIndentFromNodeLeft, pmWidthWithSpace,
              CheckBox.Width, leftMostPartFound);
        }

        private bool LayoutOptionsButton(ref int relativeLocation, int pmWidthWithSpace, bool leftMostPartFound)
        {
            return LayoutPrimitive(ref relativeLocation,
              ref OptionButton.M_xIndentFromNodeLeft, pmWidthWithSpace,
              OptionButton.Width, leftMostPartFound);
        }

        private bool LayoutImages(ImageList imageList, int[] arrIndexes,
          ref int relativeLocation, int pmWidthWithSpace, int imagePadding, bool leftMostPartFound)
        {
            if (imageList != null && imageList.Images.Count > 0 &&
              arrIndexes != null && arrIndexes.Length > 0)
            {
                bool atleast1 = false;

                for (int i = 0; i < arrIndexes.Length; i++)
                {
                    int index = arrIndexes[i];

                    if (index >= 0 && index < imageList.Images.Count)
                    {
                        if (!leftMostPartFound)
                        {
                            leftMostPartFound = true;

                            // If possible center the vertical line to the Left Image 
                            if (relativeLocation - imageList.ImageSize.Width / 2 > pmWidthWithSpace)
                            {
                                relativeLocation -= imageList.ImageSize.Width / 2;
                            }

                            m_lineRightRel = relativeLocation - 1;
                        }

                        if (!atleast1)
                        {
                            atleast1 = true;
                        }

                        relativeLocation += imageList.ImageSize.Width;
                        relativeLocation += imagePadding;
                    }
                }

                if (atleast1)
                {
                    relativeLocation += spc;
                }
            }

            return leftMostPartFound;
        }
        private bool LayoutStateImage(ImageList imageList, int imageIndex,
          ref int relativeLocation, int pmWidthWithSpace, int imagePadding, bool leftMostPartFound)
        {
            if (imageList != null && imageList.Images.Count > 0)
            {
                if (imageIndex >= 0 && imageIndex < imageList.Images.Count)
                {
                    if (!leftMostPartFound)
                    {
                        leftMostPartFound = true;

                        // If possible center the vertical line to the state Image. 
                        if (relativeLocation - imageList.ImageSize.Width / 2 > pmWidthWithSpace)
                        {
                            relativeLocation -= imageList.ImageSize.Width / 2;
                        }

                        m_lineRightRel = relativeLocation - 1;
                    }

                    m_stateImageListXRel = relativeLocation;

                    relativeLocation += imageList.ImageSize.Width + spc;
                    relativeLocation += imagePadding;
                }
            }

            return leftMostPartFound;
        }

        private bool LayoutText(ref int relativeLocation, bool leftMostPartFound)
        {
            using (Graphics g = TreeView.CreateGraphics())
            {
                m_iTextWidth = GetNodeTextSize(g, m_nodeData.Font).Width;

                m_printTextSize = Size.Round(g.MeasureString(this.Text, this.Font));

                if (!leftMostPartFound)
                {
                    leftMostPartFound = true;
                    m_lineRightRel = relativeLocation - 1;
                }

                if (m_iTextWidth > 0)
                {
                    relativeLocation += m_iTextWidth + spc;
                }

                this.Width = relativeLocation;
                int myMax = this.Bounds.X + m_nodeXRel + m_width;
                if (!this.HasNodes)
                {
                    this.MaxX = myMax;
                }
                else
                {
                    // If, in the middle of recalculating, ignore this.
                    if (htRecalculatingNodes[this] == null)
                    {
                        RecalculateMaxX();
                    }
                    else if (myMax > this.MaxX)
                    {
                        this.MaxX = myMax;
                    }
                }
            }

            return leftMostPartFound;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Called when the child maxX is changed.
        /// </summary>
        /// <param name="childMax">The child's maxX</param>
        internal void childMaxXChanged(int childMax)
        {
            if (this.Expanded)
            {
                if (childMax > m_maxX)
                {
                    this.MaxX = childMax;
                }
                else
                {
                    // If, in the middle of recalculating, ignore this.
                    if (htRecalculatingNodes[this] == null)
                    {
                        RecalculateMaxX();
                    }
                }
            }
        }

        /// <summary>
        /// Gets image for collapse or expand button.
        /// </summary>
        /// <returns></returns>
        private Image GetNodeStateImage()
        {
            Image stateImage = null;
            Image img = (this.Expanded) ? this.ExpandedImage : this.CollapsedImage;
            if (img != null)
            {
                stateImage = img;
            }
            else if (this.TreeView != null && this.TreeView.NodeStateImageList != null)
            {
                ImageList.ImageCollection images = this.TreeView.NodeStateImageList.Images;

                if (images != null && images.Count > 0)
                {
                    int imageIndex = this.Expanded ? this.ExpandImageIndex : this.CollapseImageIndex;
                    int defaultImageIndex = this.Expanded ? this.TreeView.DefaultExpandImageIndex
                      : this.TreeView.DefaultCollapseImageIndex;

                    if (imageIndex > DefaultImageIndex && imageIndex < images.Count)
                    {
                        stateImage = images[imageIndex];
                    }
                    else if (defaultImageIndex > DefaultImageIndex && defaultImageIndex < images.Count)
                    {
                        stateImage = images[defaultImageIndex];
                    }
                }
            }

            return stateImage;
        }

        private void CheckState_Changed()
        {
            // Fire event.
            OnCheckStateChanged(EventArgs.Empty);

            // Changes with respect to Caching Partial-Checked-State related:
            if (this.InteractiveCheckBox)
            {
                if ((this.CheckState == CheckState.Checked || this.CheckState == CheckState.Unchecked))
                {
                    CheckState state = this.CheckState;

                    if (this.HasNodes)
                    {
                        for (int i = 0; i < this.Nodes.Count; i++)
                        {
                            // Set checkstate to the child nodes if interactive checkbox is set.
                            if (this.Nodes[i].ShowCheckBox)
                            {
                                this.Nodes[i].CheckState = state;
                            }
                        }
                    }
                }
                else if (this.CheckState == CheckState.Indeterminate)
                {
                    // If there is a cached partial checked state, apply it.
                    if (this.CachedPartialCheckedState != null)
                    {
                        if (this.HasNodes)
                        {
                            foreach (TreeNodeAdv child in this.Nodes)
                            {
                                if (child.ShowCheckBox && this.CachedPartialCheckedState.Contains(child))
                                {
                                    child.CheckState = (CheckState)this.CachedPartialCheckedState[child];
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ShowPlusMinusChanged()
        {
            UpdatePlusMinusVisibility();

            this.InvalidateTreeView();
        }

        internal void UpdateAllPlusMinusVisibility()
        {
            foreach (TreeNodeAdv node in this.Nodes)
            {
                node.UpdateAllPlusMinusVisibility();
            }

            UpdatePlusMinusVisibility();
        }

        internal void UpdatePlusMinusVisibility()
        {
            bool oldValue = m_plusMinus.Visible;

            if (NodeStyle.ShowPlusMinus &&
              (this.HasNodes ||
              (this.TreeView != null && this.TreeView.LoadOnDemand && !this.ExpandedOnce))
              )
            {
                if (this.Level == 1 && this.TreeView != null)
                {
                    m_plusMinus.Visible = this.TreeView.NeedRootLinesSpace;
                }
                else
                {
                    m_plusMinus.Visible = true;
                }
            }
            else
            {
                m_plusMinus.Visible = false;
            }

            if (oldValue != m_plusMinus.Visible && this.TreeView != null)
            {
                this.TreeView.Invalidate(this.Bounds);
            }
        }

        /// <summary>Method return True is node in RTL mode, otherwise False.</summary>
        /// <returns>True - RTL mode enabled, otherwise False.</returns>
        [DocumentationExclude()]
        internal bool GetIsMirrored()
        {
            if (this.TreeView != null)
            {
                return this.TreeView.GetIsMirrored();
            }

            return false;
        }

        /// <summary>Caching Partial-Checked-State related : Start</summary>
        private void ClearCachedPartialCheckedState()
        {
            if (m_partialCheckedState != null)
            {
                m_partialCheckedState.Clear();
                m_partialCheckedState = null;
            }
        }


        private void CachePartialCheckedState()
        {
            // Cache the states of the children, anew.
            if (m_partialCheckedState != null)
            {
                m_partialCheckedState.Clear();
            }

            m_partialCheckedState = new Hashtable();

            if (this.HasNodes)
            {
                foreach (TreeNodeAdv node in this.Nodes)
                {
                    if (node.ShowCheckBox)
                    {
                        m_partialCheckedState[node] = node.CheckState;
                    }
                }
            }
        }

        private void childCheckStateChanged(object sender, EventArgs e)
        {
            UpdateInteractiveCheckState();
        }

        private void UpdateInteractiveCheckState()
        {
            if (!NodeStyle.InteractiveCheckBox
              || TreeNodeAdv.checkStateChangingSourceNode == this
              || this.IsParent(TreeNodeAdv.checkStateChangingSourceNode)
              )
            {
                return;
            }

            int firstChildWithCheckBox = -1;

            if (HasNodes)
            {
                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    if (this.Nodes[i].ShowCheckBox)
                    {
                        firstChildWithCheckBox = i;
                        break;
                    }
                }
            }

            // if nothing found then exit
            if (firstChildWithCheckBox == -1)
            {
                return;
            }

            // NOTE: if we are here then this.Nodes collection has items and we found something
            CheckState first = this.Nodes[firstChildWithCheckBox].CheckState;
            bool same = true;

            for (int i = firstChildWithCheckBox + 1, len = this.Nodes.Count; i < len; i++)
            {
                TreeNodeAdv nodesInternal = this.Nodes[i];

                if (nodesInternal.ShowCheckBox && first != nodesInternal.CheckState)
                {
                    same = false;
                }
            }

            // Make sure to clear the cached checked state before setting this.
            this.ClearCachedPartialCheckedState();

            // Changes wrt Caching Partial-Checked-State related : Start
            if (same)
            {
                this.CheckState = first;

                // We just went from indeterminate to determinate state, so no need to cache;
                this.ClearCachedPartialCheckedState();
            }
            else
            {
                this.CheckState = CheckState.Indeterminate;
            }
            // Changes wrt Caching Partial-Checked-State related : End
        }

        /// <summary>Invalidate TreeView if we have reference on it. Invalidate 
        /// full window.</summary>
        private void InvalidateTreeView()
        {
            if (this.TreeView != null)
            {
                TreeView.Invalidate();
            }
        }

        /// <summary>Invalidate TreeView if we have reference on it. Invalidate 
        /// part of windows specified by rect.</summary>
        /// <param name="rect">Invalidate rectangle.</param>
        private void InvalidateTreeView(Rectangle rect)
        {
            if (this.TreeView != null)
            {
                TreeView.Invalidate(rect);
            }
        }

        internal void SetCustomControlMember(Control control)
        {
            m_customControl = control;
        }

        private void ExpandParentSelf()
        {
            // Fixed defect that the BringIntoView method does not expand the tree 
            // when its already expanded but it collapsed by its parent(Forum: 42052).
            if (this.ParentNode != null && (!this.ParentNode.Expanded || !this.ParentNode.IsVisible))
            {
                this.ParentNode.ExpandParentSelf();
            }

            this.Expand();
        }

        internal void MakeDirty()
        {
            if (TreeView != null)
            {
                this.TreeView.MakeDirty();
            }
        }

        internal void SetHeightIfChanged(int oldHeight, int newHeight)
        {
            if (this.Height == oldHeight)
            {
                this.Height = newHeight;
            }

            if (HasNodes)
            {
                for (int i = 0, len = this.Nodes.Count; i < len; i++)
                {
                    this.Nodes[i].SetHeightIfChanged(oldHeight, newHeight);
                }
            }
        }
        internal void UpdateCustomConrtol()
        {
            if (this.CustomControl != null && this.CustomControl.Visible)
            {
                this.CustomControl.Location = new Point(this.Bounds.X + m_customControlRelativeLocation + m_nodeXRel, this.Bounds.Y);
                this.CustomControl.Height = this.Height;

                this.CustomControl.Update();
            }
        }

        /// <summary>
        /// Return custom control bounds.
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetCustomControlBounds()
        {
            return new Rectangle(
              this.Bounds.X + m_customControlRelativeLocation + m_nodeXRel, this.Bounds.Y,
              this.CustomControl.Width, this.Bounds.Height);
        }

        internal void SubscribeControlEvents(Control control)
        {
            control.SizeChanged += new EventHandler(CustomControl_SizeChanged);
            control.LocationChanged += new EventHandler(CustomControl_LocationChanged);
        }

        internal void UnSubscribeControlEvents(Control control)
        {
            control.SizeChanged -= new EventHandler(CustomControl_SizeChanged);
            control.LocationChanged -= new EventHandler(CustomControl_LocationChanged);
        }

        /// <summary>Force recalculation of VisibleNodeCount property value. Reset value cache.</summary>
        /// <param name="recurseOnChildren">True - force reset for child nodes too, otherwise False.</param>
        internal void ResetVisibleNodeCount(bool recurseOnChildren)
        {
            bool recurseOnParent = (m_visibleNodeCount > 0);

            m_visibleNodeCount = -1;

            if (recurseOnParent && this.ParentNode != null)
            {
                this.ParentNode.ResetVisibleNodeCount(false);
            }

            if (recurseOnChildren && HasNodes)
            {
                for (int n = 0, len = this.Nodes.Count; n < len; n++)
                {
                    TreeNodeAdv node = this.Nodes[n];
                    node.ResetVisibleNodeCount(true);
                }
            }
        }

        /// <summary>Call this when node gets expanded or collapsed, nodes are 
        /// changed or height is changed.</summary>
        /// <param name="delta">Allowed any values not equal to -1 value, other values 
        /// will force recursive update of VisibleNodeCount in upper direction 
        /// till root node.</param>
        internal void AdjustVisibleNodeCount(int delta)
        {
            // if VisibleNodeCount not initialized yet then don't touch it.
            if (m_visibleNodeCount != -1)
            {
                m_visibleNodeCount += delta;
            }

            // send notification to parent node that visible node count changed
            if (this.ParentNode != null)
            {
                this.ParentNode.AdjustVisibleNodeCount(delta);
            }
        }

        /// <summary>Update node bounds.</summary>
        /// <param name="bounds">New node bounds.</param>
        protected internal void SetBounds(Rectangle bounds)
        {
            m_bounds = bounds;
        }

        /// <summary>Method allow to replace Primitives collection if needed. Low level 
        /// jobs with internal node data. Only for inheritors.</summary>
        /// <param name="primitives"/>
        protected internal void SetPrimitives(TreeNodePrimitivesCollection primitives)
        {
            if (primitives != null)
            {
                m_primitives = primitives;
                m_primitives.SetParent(this);

                RecalculateAllDimensions();
            }
        }

        /// <summary>Method allow to replace subitems collection if needed. Low level 
        /// jobs with internal node data. Only for inheritors.</summary>
        /// <param name="collection">Collection that will replace current node Subitems collection.</param>
        protected internal void SetSubItems(TreeNodeAdvSubItemCollection collection)
        {
            if (collection != null)
            {
                m_subitems = collection;
                m_subitems.SetParent(this);

                RecalculateAllDimensions();
            }
        }
        #endregion

        #region Class internal declarations
        /// <summary>Comparer that allow to sort TreeNodeAdv classes by internal 
        /// TreeRowIndex property value. Also can be used for binary searches.</summary>
        public sealed class TreeRowIndexComparer : IComparer
        {
            /// <summary>Get reference on statically created comparer.</summary>
            public static readonly IComparer Default = new TreeRowIndexComparer();

            /// <summary>Hide constructor from user. Only one instance on class allowed in AppDomain.</summary>
            private TreeRowIndexComparer()
            {
            }

            /// <summary>Compare TreeNodes by TreeRowIndex property value.</summary>
            public int Compare(object x, object y)
            {
                int node1 = (x as TreeNodeAdv).TreeRowIndex;
                int node2 = (int)y;

                if (node1 < node2)
                {
                    return -1;
                }
                if (node1 > node2)
                {
                    return 1;
                }
                return 0;
            }
        }

        /// <summary>Class is used by node visible property.</summary>
        internal class VisibleEditor : UITypeEditor
        {
          
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                return base.EditValue(context, provider, value);
            }

        
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.DropDown;
            }
        }
        #endregion
    }
}