#region Copyright Syncfusion Inc. 2001 - 2014
//
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Core.Licensing;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools.Design;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// The MultiColumnTreeView control is an advanced tree control that surpasses the 
    /// functionality and look of the standard TreeView control.
    /// </summary>
    [
    ToolboxItem(true),
    Designer(typeof(MultiColumnTreeViewDesigner)),
    ToolboxBitmap(typeof(TreeViewAdv), "ToolboxIcons.MultiColumnTreeView.bmp"),
    DefaultProperty("Nodes"),
    Description("Advanced tree control with MultiColumn support.")
    ]
    public class MultiColumnTreeView :
      ScrollControl,
      ISupportInitialize,
      IThemedControl,
      INonClientPaintingSupport,
      IProvideCustomContextMenuPositionalInformation,
      IDragDispatcher,
      ISuppportHistory
    {
        #region Class constants

        /// <summary></summary>
        internal const string NodeLevelStyleBaseName = "NodeLevel";

        /// <summary></summary>
        internal const string BaseStyleBaseName = "BaseStyle";

        /// <summary></summary>
        internal const string ColumnStyleBaseName = "ColumnStyle";

        /// <summary></summary>
        internal const string SubItemStyleBaseName = "SubItemStyle";

        /// <summary></summary>
        internal const string DefaultBaseStyleName = "Standard";


        /// <summary></summary>
        internal const string DefaultColumnStyleName = "Standard - Column";

        /// <summary></summary>
        internal const string DefaultSubItemStyleName = "Standard - SubItem";

        /// <summary>Default image index.</summary>
        internal const int DefaultImageIndex = -1;

        /// <summary></summary>
        internal const int DefaultHeaderHeigh = TreeColumnAdv.DefaultHeaderHeigh;

        /// <summary></summary>
        private const int DEF_MIN_COLUMN_WIDTH = 4;
        /// <summary>
        ///Minimum column width
        /// </summary>
        private static int MinColumnWidth = 60;
        /// <summary>
        /// A border width when Border style is set to BorderStyle.Fixed3D.
        /// </summary>
        private const int FIXED3D_BORDER_WIDTH = 2;
        ///<summary></summary>
      private int drawFlag = 0;
        /// <summary></summary>
        /// /// <summary>
        /// 
        /// </summary>
        private enum PrintDirection
        {
            /// <summary>
            /// 
            /// </summary>
            Horizontal,

            /// <summary>
            /// 
            /// </summary>
            Vertical
        }
        internal enum InternalBooleanFields : int
        {
            /// <summary>True - we show header, otherwise False.</summary>
            ShowHeader,

            /// <summary>True - select node when collapsed, otherwise False.</summary>
            SelectOnCollapse,

            /// <summary>True - enabled Undo/Redo manager, otherwise False.</summary>
            HistoryEnabled,

            /// <summary>True - force control to ignore recalculations requests, otherwise False.</summary>
            SuspendExpandRecalculate,

            /// <summary>Auto custom controls adding in MultiColumnTreeView.Controls
            /// false - only in designer editor.</summary>
            AutoControlsAdding,

            /// <summary></summary>
            ClickedOnSelection,

            /// <summary>True - indicate that tree is free for draging operation, otherwise False.</summary>
            CanDrag,

            /// <summary>True - in control enabled hot tracking functionality, otherwise False.</summary>
            HotTracking,

            /// <summary>True - show level lines in tree, otherwise False.</summary>
            ShowLines,

            /// <summary>True - show root line in tree heirarchy, otherwise False.</summary>
            ShowRootLines,

            /// <summary>True - control in printing process, otherwise False.</summary>
            Printing,

            /// <summary>Indicates whether multiple nodes can be selected with mouse down and drag.</summary>
            AllowMouseBasedSelection,

            /// <summary>True - load data on user demand, otherwise False.</summary>
            LoadOnDemand,

            /// <summary> Indicates whether the <see cref="BeforeNodePaint"/> and <see cref="AfterNodePaint"/> events will be fired before drawing a node.</summary>
            OwnerDrawNodes,

            /// <summary></summary>
            OwnerDrawNodesBackground,

            /// <summary></summary>
            LabelEdit,

            /// <summary></summary>
            AddSeparatorAtEnd,

            /// <summary></summary>
            MouseBasedSelectionOn,

            /// <summary></summary>
            AllowKeyboardSearch,

            /// <summary>True - paint full row selection, otherwise paint only label selection.</summary>
            FullRowSelect,

            /// <summary>True - on control focus lost do not show selection, otherwise False.</summary>
            HideSelection,

            /// <summary></summary>
            PreparingDragCueBitmap,

            /// <summary>True - indicate that we in Dragging process, otherwise False.</summary>
            Dragging,

            /// <summary>Indicates whether mouse has left out of control.</summary>
            MouseLeaved,

            /// <summary>Stub variable which indicates whether artificial drag-and-drop works
            /// when AllowDrop is set to false.</summary>
            AllowDropStubWorks,

            /// <summary>Indicates whether control must draw dotted rectangle around 
            /// selected node when it has no focus.</summary>
            KeepDottedSelection,

            /// <summary>Indicates whether cue image should be drawn at a distance below the mouse 
            /// cursor while dragging.</summary>
            KeepDragCapturePoint,

            /// <summary></summary>
            ThemedBorder,

            /// <summary></summary>
            IgnoreThemeBackground,

            /// <summary></summary>
            IgnoreNextMouseMove,

            /// <summary></summary>
            EnsureVisibleSelectedNode,

            /// <summary></summary>
            TransparentControls,

            /// <summary></summary>
            DragOnText,

            /// <summary></summary>
            ShowDragNodeCue,

            /// <summary></summary>
            BroughtIntoView,

            /// <summary>indicates the direction of selection</summary>
            SelectUpwardDirection,

            /// <summary></summary>
            SortWithChildNodes,

            /// <summary></summary>
            InVScroll,

            /// <summary></summary>
            InHScroll,

            /// <summary>True - custom control refresh required, otherwise False. 
            /// Need update custom controls visibility and bounds.</summary>
            NeedUpdateCustomControls,

            /// <summary>True - indicate that control catch key down and processing it, otherwise False.</summary>
            IsKeyDown,

            /// <summary>True - indicate that control catch Mouse down and processing it, otherwise False.</summary>
            IsMouseDown,

            /// <summary>True - indicate Left mouse button catch and processing, otherwise False.</summary>
            IsLeftMouseDown,

            /// <summary>True - indicate Mouse up catch and procesing, otherwise False.</summary>
            IsMouseUp,

            /// <summary></summary>
            EnsureVisible,

            /// <summary></summary>
            IsMouseDownWithCtrl,

            /// <summary></summary>
            CancelEdit,

            /// <summary></summary>
            DragCueOn,

            /// <summary></summary>
            ShouldSelectNodeOnEnter,

            /// <summary></summary>
            NeedUpdateEditTop,

            /// <summary></summary>
            IgnoreLeave,

            /// <summary>To avoid nested calling of EndEdit method.</summary>
            IsEditEnding,

            /// <summary></summary>
            CanGetDragImage,

            /// <summary></summary>
            DesignMode,

            /// <summary></summary>
            ColumnsMovedMode,

            /// <summary>True - force nodes to recalculate own height for multiline cases, 
            /// otherwise False.</summary>
            AutoAdjustMultiLineHeight,
            /// <summary></summary>
            IsDoubleClick,

            /// <summary>MUST BE LAST DECALRED IN ENUM!!! Maximum counter of bit flags.</summary>
            [Browsable(false)]
            Max,
        }

        private const int PRF_NONCLIENT = 0x00000002;
        private bool scrollable = true;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default item height
        /// </summary>
        private static int ITMHEIGHT = default(int);
        #endregion

        #region Class members
        /// <summary>Storage of all boolean internal varaibles. Optimize control memory usage. By default each
        /// boolean field in class eat 4 bytes of memory - bit array reserve one bit for each boolean value and 
        /// align collection internal size to integer.</summary>

        private Point m_ptPrintPosition;

        private Image m_imgControl = null;
 
        private int m_iNodeHeight = 0;
  
        private PrintDirection m_pdCurrentDirection;

        private int m_iScrollBarHeight = 0;
   
        private int m_iScrollBarWidth = 0;

        private int m_iPageNumber = 0;

        private DateTime m_dtDateTime;

        private PrintDocument m_pdDocument;

        private BitArray m_boolean = new BitArray((int)InternalBooleanFields.Max);

        private TreeNodeAdv m_lastNodeOver = null;

        private TreeNodeAdv m_tnHelpTextNode = null;

        private TreeNodeAdvSubItem m_previousHelpTextSubItem = null;

        private TreeNodeAdv m_lastSelectedNode = null;

        private TreeNodeAdv m_lastButtonDownNode = null;

        private TreeNodeAdv m_root;

        private TreeNodeAdv m_activeNode = null;

        private TreeNodeAdv m_selectionBaseNode = null;

        private TreeNodeAdv m_activeNodeForLabelEdit = null;

        private TreeNodeAdv m_rMouseDownNode = null;

        private TreeNodeAdv m_lMouseDownNode = null;

        /// <summary>Indicates the current now which is selected by key pressing</summary>
        private TreeNodeAdv m_lastSelectedByKeyBoard = null;

        private TreeNodeAdv[] m_draggedTnas;

        /// <summary>Node which must be highlighted like parent for dragging.</summary>
        private TreeNodeAdv m_highlightedNode;

        /// <summary>Special graphics for text measuring.</summary>
        private Graphics m_measure;

        private int m_headerHeight = DefaultHeaderHeigh;
        /// <summary>
        ///Header height
        /// </summary>
        private static int HDRHeight = default(int);
        /// <summary>Storage of columns.</summary>
        private TreeColumnAdvCollection m_columns;

        private HistoryManager m_historyManager = null;

        private StyleNamePairsList m_stylePairs = null;

        /// <summary>
        /// Custom control collection. Key - custom control. Value - node.
        /// </summary>
        private Hashtable m_htCustomControlCollection = new Hashtable();

        private IntPtr m_rgnCached = IntPtr.Zero;

        /// <summary>
        /// ImageList with images that are displayed 
        /// instead of expand/collapse button.
        /// </summary>
        private ImageList m_nodeStateImageList = null;

        /// <summary>
        /// Index of default image for collapse button.
        /// </summary>
        private int m_defaultCollapseImageIndex = DefaultImageIndex;

        /// <summary>
        /// Index of default image for expand button.
        /// </summary>
        private int m_defaultExpandImageIndex = DefaultImageIndex;

        private bool useDefaultDrawing = false;

        private Border3DSide m_borderSides = Border3DSide.All;

        private Border3DStyle m_border3DStyle = Border3DStyle.Sunken;

        private BorderStyle m_borderStyle = BorderStyle.Fixed3D;

        private ButtonBorderStyle m_borderSingle = ButtonBorderStyle.Solid;

        internal bool DisableReplacing = false;

        internal bool DisableFinding = false;

        private Color m_clrBorder = Color.Black;

        private ToolTipAdv m_helpText;

        internal ITreeNodeAdvPaintFilter m_paintFilter = null;

        /// <summary>
        /// Nodes needed to be highlighted for selecting child.
        /// </summary>
        private Hashtable m_hashHighlightedNodes = new Hashtable();

        /// <summary>
        /// Point to remember last click-point.
        /// NOTE : It will be set at OnMouseDown and null
        ///  ( set to Point.Empty ) at OnMouseUp.
        ///  ( need to fix issue # 180 )
        /// </summary>
        private Point m_lastButtonDownPoint = Point.Empty;

        private IContainer components;

        private int m_itemHeight = -1;

        private int m_parentIndent = 19;

        private ArrayList m_latestMouseBasedSelection = null;

        private Timer m_keyInputTimer;

        private TextBox m_labelEditor;

        private string m_strKeySearch = string.Empty;

        private Pen m_penLine;

        private DashStyle m_lineStyle = DashStyle.Dot;

        private Color m_clrLine = Color.Gray;

        /// <summary>
        /// Collection contains checked nodes in tree
        /// </summary>
        private CheckedNodesColection m_checkedNodes;

        private SelectedNodesCollection m_selectedNodes;

        private Point m_ptMouseDown = Point.Empty;

        private Timer m_timerLabelEditStart;

        private string m_strPathSeparator = "\\";

        private PrintDocument m_printDocument;

        private ToolTipAdv m_toolTip;

        private int m_gutterSpace = 3;

        private int m_iNodeCount = 0;

        private TreeSelectionMode m_selectionMode = TreeSelectionMode.Single;

        private ImageList m_leftImageList;

        private ImageList m_rightImageList;

        private ImageList m_stateImageList;
 
        private BrushInfo m_selectedNodeBackground = new BrushInfo(SystemColors.Highlight);

        private BrushInfo m_inactiveSelectedNodeBackground = new BrushInfo(SystemColors.Control);

        private Color m_clrSelectedNodeFore = SystemColors.HighlightText;

        private Color m_clrInactiveSelectedNodeFore = SystemColors.ControlText;

        internal int m_currentSelectedNodeIndex = -1;

        private BrushInfo m_bgBrush;

        private DragHelper m_dragHelper = null;

        private Hashtable m_baseStyles = new Hashtable();

        private TreeNodeAdvStyleInfo m_boundStyle = null;

        /// <summary>Indicates the currently selected keys.</summary>
        private Keys m_pressedKey = Keys.None;

        private int m_hScrollPos = 1;
  
        private int m_vScrollPos = 1;

        private int m_textSelectionLength = 0;

        private int m_selectionStart = 0;

        private string m_strLastText = string.Empty;

        private BrushInfo m_HeaderBackground = new BrushInfo(SystemColors.Control);

        private int m_iNcScreenOffsetX = 0;

        private int m_iNcScreenOffsetY = 0;
   
        private int m_iColumnsPressedPosition = 0;

        private int m_iColumnsPressedIndex = 0;

        private int m_iColumnsOffset = 0;

        private TreeColumnAdv m_pressedColumn;

        private Region m_drawingRgn = new Region();

        private bool m_bScrollersRefreshed = false;

        private bool nodeInRefresh = false;
        #endregion

        #region Class properties
        /// <summary>Gets or sets a value indicating whether recalculate height of nodes for proper multi-line text displaying</summary>
        [
        DefaultValue(false),
        Description("Recalculate height of nodes for proper multi-line text displaying, otherwise False."),
        Category("Appearance")
        ]
        public bool AutoAdjustMultiLineHeight
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AutoAdjustMultiLineHeight];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.AutoAdjustMultiLineHeight] != value)
                {
                    m_boolean[(int)InternalBooleanFields.AutoAdjustMultiLineHeight] = value;

                    if (null != Root)
                    {
                        Root.RecalculateAllDimensions();
                        Invalidate();
                    }
                }
            }
        }

      
        [
        Browsable(true),
        DefaultValue(DefaultHeaderHeigh),
        Category("Appearance"),
        Description("Define header height if control works in Tree list view mode.")
        ]
        public int HeaderHeight
        {
            get
            {
                return m_headerHeight;
            }
            set
            {
                if (m_headerHeight != value)
                {
                    m_headerHeight = value;
                    UpdateColumnsHeight();
                    RecreateHandle();
                    InvalidateNc();
                }
            }
        }

        /// <summary>
        /// Gets the number of tree nodes that can be fully visible in the tree view 
        /// control.
        /// </summary>
        /// <value>
        /// The number of <see cref="TreeNodeAdv"/> items that can be fully visible in 
        /// the <see cref="MultiColumnTreeView"/> control.
        /// </value>
        /// <remarks>
        /// The <b>VisibleCount</b> value can be greater than the number of tree nodes 
        /// in the tree view. The value is calculated by dividing the height of the 
        /// client window by the height of a tree node item. The result is the total 
        /// number of <see cref="TreeNodeAdv"/> objects that the <see cref="MultiColumnTreeView"/> is 
        /// capable of displaying within its current dimensions.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Gets the number of tree nodes that can be fully visible in the tree view control.")
        ]
        public int VisibleCount
        {
            get
            {
                return this.TreeColumnRectangle.Height / this.ItemHeight;
            }
        }

        /// <override/>
        /// <summary>Overriden. </summary>
        [
        DefaultValue(false)
        ]
        public override bool AllowIncreaseSmallChange
        {
            get
            {
                return base.AllowIncreaseSmallChange;
            }
            set
            {
                base.AllowIncreaseSmallChange = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether default graphics to be used for drawing instead of double buffering
        /// for faster drawing performance.
        /// </summary>
        [
        Browsable(false),
        DefaultValue(false)
        ]
        public bool UseDefaultDrawing
        {
            get
            {
                return this.useDefaultDrawing;
            }
            set
            {
                if (this.useDefaultDrawing != value)
                {
                    this.useDefaultDrawing = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the Horizontal scrollbar.
        /// </summary>
        [
        Description("The position of the Horizontal scrollbar"),
        Category("Scrolling"),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public int HScrollPos
        {
            get
            {
                if (this.HScrollBar.Enabled)
                {
                    return m_hScrollPos;
                }

                return 0;
            }
            set
            {
                if (m_hScrollPos != value)
                {
                    NeedUpdateCustomControls = true;
                    int width = m_hScrollPos - value;
                    m_hScrollPos = value;

                    ScrollWindow(width, 0, this.ClientRectangle, this.ClientRectangle, true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the position of the Vertical scrollbar.
        /// </summary>
        [
        Description("The position of the Vertical scrollbar."),
        Category("Scrolling"),
        Browsable(false),
        DefaultValue(1)
        ]
        public int VScrollPos
        {
            get
            {
                return m_vScrollPos;
            }
            set
            {
                if (m_vScrollPos != value)
                {
                    m_vScrollPos = value;
                    NeedUpdateCustomControls = true;
                    NeedUpdateBounds = true;

                    int nValue = value;

                    if (nValue < VScrollBar.Minimum)
                    {
                        nValue = VScrollBar.Minimum;
                    }
                    else if (nValue > VScrollBar.Maximum)
                    {
                        nValue = VScrollBar.Maximum;
                    }

                    int iScrollValue = nValue + VScrollBar.LargeChange - VScrollBar.SmallChange;

                    if (iScrollValue == VScrollBar.Maximum)
                    {
                        TreeNodeAdv nodeAdv = RowIndexToNode(m_vScrollPos + VScrollBar.LargeChange);

                        if (nodeAdv != LastVisibleNode )
                        {
                            ScrollWindow(nValue);
                        }
                    }
                    else
                    {
                        ScrollWindow(nValue);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enables or disables vertical scrollbar.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool VScroll
        {
            get
            {
                return base.VScroll;
            }
            set
            {
                base.VScroll = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enables or disables scrollbars.
        /// </summary>
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Appearance"),
        Description("lLets you set the background color, gradient, etc. for column headers zonee"),
        DefaultValue(true)]
        public virtual  bool Scrollable
        {
            get
            {
                return scrollable;
            }
            set
            {
                scrollable = value;
                if (!scrollable)
                {
                    VScroll = false;
                    HScroll = false;
                }
                else
                {
                    RefreshHScrollbar();
                    RefreshVScrollbar();
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether Enables or disables horizontal scrollbar.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool HScroll
        {
            get
            {
                return base.HScroll;
            }
            set
            {
                base.HScroll = value;
            }
        }

        /// <summary>
        /// Gets or sets the HistoryManager to use.
        /// </summary>
        [
        Description(@"HistoryManager to use for Undo\Redo operations."),
        Browsable(false),
        DefaultValue(null)
        ]
        public HistoryManager HistoryManager
        {
            get
            {
                return m_historyManager;
            }
            set
            {
                if (value != m_historyManager)
                {
                    m_historyManager = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether register items in history list.
        /// </summary>
        [
        DefaultValue(false),
        Category("Appearance"),
        Description(@"Indicates, record actions for Undo\Redo using, or not.")
        ]
        public bool HistoryEnabled
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.HistoryEnabled];
            }
            set
            {
                if (value != m_boolean[(int)InternalBooleanFields.HistoryEnabled])
                {
                    m_boolean[(int)InternalBooleanFields.HistoryEnabled] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control will ignore the theme's background color and draw 
        /// the <see cref="BackgroundColor"/> instead when <see cref="ThemesEnabled"/> is true.
        /// </summary>
        /// <value>True to ignore theme background; false otherwise. Default is false.</value>
        [
        Description("This will ignore the theme's background color and draw the BackgroundColor when ThemesEnabled is true."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public bool IgnoreThemeBackground
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IgnoreThemeBackground];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.IgnoreThemeBackground] != value)
                {
                    m_boolean[(int)InternalBooleanFields.IgnoreThemeBackground] = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control and it's parts should be drawn themed.
        /// </summary>
        /// <value>True to enable themes; false otherwise. Default is false.</value>
        [
        Description("Indicates if the control is drawn themed."),
        Category("Appearance")
        ]
        public bool ThemesEnabled
        {
            get
            {
                if (this.StandardStyle != null)
                {
                    return this.StandardStyle.ThemesEnabled;
                }

                return false;
            }
            set
            {
                if (this.StandardStyle != null)
                {
                    this.StandardStyle.ThemesEnabled = value;

                    RecreateHandle();
                    this.Root.RecalculateAllDimensions();
                    Invalidate();
                    OnThemeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the border sides of the control that will be drawn.
        /// </summary>
        /// <value>One of the <see cref="System.Windows.Forms.Border3DSide"/> values. Default is Border3DSide.All.</value>
        [
        Description("Indicates the border sides of the control."),
        Category("Appearance"),
        DefaultValue(Border3DSide.All)
        ]
        public Border3DSide BorderSides
        {
            get
            {
                return m_borderSides;
            }
            set
            {
                if (m_borderSides != value)
                {
                    m_borderSides = value;
                    OnBorderSidesChanged(EventArgs.Empty);
                    InvalidateWindow();
                }
            }
        }

        /// <override/>
        /// <summary></summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color, gradient and other styles. This will override the BackColor setting.
        /// </summary>
        /// <remarks>
        /// The <see cref="MultiColumnTreeView"/> provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Appearance"),
        Description("Lets you set the background color, gradient, etc. overriding the BackColor setting.")
        ]
        public BrushInfo BackgroundColor
        {
            get
            {
                return m_bgBrush;
            }

            set
            {
                if (m_bgBrush != value)
                {
                    m_bgBrush = (value == null) ? BrushInfo.Empty : value;

                    this.DisableScrollWindow = (this.IsVerticalGradient);

                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color, gradient and other styles for column's headers zone.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        RefreshProperties(RefreshProperties.Repaint),
        Category("Appearance"),
        Description("Lets you set the background color, gradient, etc. for column headers zone")
        ]
        public BrushInfo ColumnsHeaderBackground
        {
            get
            {
                return m_HeaderBackground;
            }

            set
            {
                if (m_HeaderBackground != value)
                {
                    m_HeaderBackground = (value == null) ? BrushInfo.Empty : value;

                    InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the 2D border.
        /// </summary>
        [
        Description("Indicates the color of the 2D border."),
        Category("Appearance")
        ]
        public Color BorderColor
        {
            get
            {
                return m_clrBorder;
            }
            set
            {
                if (m_clrBorder != value)
                {
                    m_clrBorder = value;
                    OnBorderColorChanged(EventArgs.Empty);
                    InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the 2D border style.
        /// </summary>
        /// <value>One of the <see cref="ButtonBorderStyle"/> values. Default is ButtonBorderStyle.Solid.</value>
        [
        Description("Indicates the 2D border style."),
        Category("Appearance"),
        DefaultValue(ButtonBorderStyle.Solid)
        ]
        public ButtonBorderStyle BorderSingle
        {
            get
            {
                return m_borderSingle;
            }
            set
            {
                if (m_borderSingle != value)
                {
                    m_borderSingle = value;
                    OnBorderSingleChanged(EventArgs.Empty);
                    InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        /// <value>One of the <see cref="BorderStyle"/> values. Default is BorderStyle.Fixed3D.</value>
        [
        DefaultValue(BorderStyle.Fixed3D),
        Description("Indicates the border style of the control.")
        ]
        public new BorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }
            set
            {
                base.BorderStyle = BorderStyle.None;
                if (m_borderStyle != value)
                {
                    BorderStyle baseValue = value;
                    // The textbox doesn't give you a 1 pixel border for FixedSingle,
                    // so specifying 3D border and then drawing a single border manually.
                    if (value == BorderStyle.FixedSingle)
                    {
                        baseValue = BorderStyle.Fixed3D;
                    }
                    base.BorderStyle = baseValue;

                    m_borderStyle = value;
                    this.UpdateStyles();
                    InvalidateWindow();
                    OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the style of the 3D border.
        /// </summary>
        /// <value>One of the <see cref="Border3DStyle"/> values. Default is Border3DStyle.Sunken.</value>
        [
        Description("Indicates the style of the 3D border."),
        Category("Appearance"),
        DefaultValue(Border3DStyle.Sunken)
        ]
        public Border3DStyle Border3DStyle
        {
            get
            {
                return m_border3DStyle;
            }
            set
            {
                if (m_border3DStyle != value)
                {
                    m_border3DStyle = value;
                    OnBorder3DStyleChanged(EventArgs.Empty);
                    InvalidateWindow();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether control must draw dotted rectangle around 
        /// selected node when it has no focus.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Gets or sets value which indicates if control must draw dotted rectangle around selected node then it has no focus."),
        DefaultValue(true)
        ]
        public bool KeepDottedSelection
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.KeepDottedSelection];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.KeepDottedSelection] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cue image should be drawn 
        /// at a distance below the mouse cursor while dragging. 
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Gets or sets value which indicates if cue image should be drawn at a distance below the mouse cursor while draging or not. "),
        DefaultValue(false)
        ]
        public bool KeepDragCapturePoint
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.KeepDragCapturePoint];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.KeepDragCapturePoint] = value;
                DragHelper.DragWindow.DragParent = (value) ? this : null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sort treeview including all the child nodes.
        /// </summary>
        /// <value><c>true</c> if sort all with child nodes; otherwise, <c>false</c>.
        /// </value>
        /// <example>This example describes how to sort all the nodes in the MultiColumnTreeView
        /// <code language="C#">
        /// If SortWithChildNodes property is set to true,the user can sort all the nodes including all the child nodes in the treeViewAdv.
        /// The SortOrder of the Root should be specified for the sorting all nodes.
        /// // Sorts only the root nodes.
        /// private void button1_Click(object sender, System.EventArgs e)
        /// {
        /// this.treeViewAdv1.Nodes.Sort();
        /// }
        /// // Sort all the root nodes and the child nodes in the TreeviewAdv
        /// private void button2_Click_1(object sender, System.EventArgs e)
        /// {
        /// this.treeViewAdv1.Root.SortOrder=SortOrder.Ascending;
        /// this.treeViewAdv1.SortWithChildNodes=true;
        /// this.treeViewAdv1.Root.Sort();
        /// }
        /// </code><code language="VB">
        /// 'Sorts only the root nodes.
        /// Private Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        /// Me.treeViewAdv1.Nodes.Sort()
        /// End Sub
        /// 'Sort all the root nodes and the child nodes in the TreeviewAdv
        /// Private Sub button2_Click_1(ByVal sender As Object, ByVal e As System.EventArgs)
        /// Me.treeViewAdv1.Root.SortOrder=SortOrder.Ascending
        /// Me.treeViewAdv1.SortWithChildNodes=True
        /// Me.treeViewAdv1.Root.Sort()
        /// End Sub
        /// </code></example>
        [
        Browsable(true),
        Description("This will sort all the nodes including child nodes."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public bool SortWithChildNodes
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.SortWithChildNodes];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.SortWithChildNodes] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether an alpha blended image of the selected nodes
        /// should be drawn beside the cursor during drag and drop.
        /// </summary>
        /// <value><para>True to show an alpha blended image; false otherwise. Default is true.</para><para>You could customize the style in which nodes are drawn in the above image by 
        /// adding a "DragNodeCueStyle" style to the <see cref="BaseStyles"/> collection.</para></value>
        [
        DefaultValue(true),
        Description("Specifies whether a semi-transaprent image of the selected nodes should be drawn beside the cursor during drag and drop."),
        Category("Appearance")
        ]
        public virtual bool ShowDragNodeCue
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ShowDragNodeCue];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.ShowDragNodeCue] = value;
            }
        }

        [
        Browsable(false),
        DocumentationExclude(),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public TreeNodeAdv ActiveNode
        {
            get
            {
                return m_activeNode;
            }
            set
            {
                if (m_activeNode != value)
                {
                    m_activeNode = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the first fully-visible tree node in the tree view control.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> that represents the first fully-visible 
        /// tree node in the tree view control.</value>
        /// <remarks>
        /// Initially, the <b>TopVisibleNode</b> returns the first root tree node, which is 
        /// located at the top of the <see cref="MultiColumnTreeView"/>. However, if the user has scrolled 
        /// the contents, another tree node might be at the top.
        /// <seealso cref="LastVisibleNode"/></remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public TreeNodeAdv TopVisibleNode
        {
            get
            {
                return RowIndexToNode(this.VScrollPos);
            }
        }

        /// <summary>
        /// Gets the last visible node.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> instance.</value>
        /// <remarks><seealso cref="TopVisibleNode"/></remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public virtual TreeNodeAdv LastVisibleNode
        {
            get
            {
                TreeNodeAdv node = this.TopVisibleNode;

                if (node != null)
                {
                    int y = NodeToPoint(node).Y + node.Height;
                    int height = this.ClientHeight;
                    TreeNodeAdv nodeNext = null;

                    while (node.NextVisibleNode != null)
                    {
                        nodeNext = node.NextVisibleNode;
                        y += nodeNext.Height;

                        if (y > height)
                            break;

                        node = nodeNext;
                    }
                }

                return node;
            }
        }

        /// <summary>
        /// Gets the helptext control of the MultiColumnTreeView.
        /// </summary>
        /// <remarks>This is the control used to display the <see cref="TreeNodeAdv.HelpText"/> of the nodes.</remarks>
        [
        Browsable(false),
        Description("The helptext control of the MultiColumnTreeView."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public virtual ToolTipAdv HelpTextControl
        {
            get
            {
                return m_helpText;
            }
        }

        /// <summary>
        /// Gets the tooltip control of the MultiColumnTreeView.
        /// </summary>
        /// <remarks>
        /// This is the control used to display the tooltip for the nodes when the text of the nodes are partially visible.
        /// </remarks>
        [
        Browsable(false),
        Description("The tooltip control of the MultiColumnTreeView."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public virtual ToolTipAdv ToolTipControl
        {
            get
            {
                return m_toolTip;
            }
        }

        /// <summary>
        /// Gets or sets the text color of the selected node.
        /// </summary>
        /// <value>Default is a system color.</value>
        [
        Description("Indicates the text color of the selected node."),
        Category("Appearance")
        ]
        public virtual Color SelectedNodeForeColor
        {
            get
            {
                return m_clrSelectedNodeFore;
            }
            set
            {
                m_clrSelectedNodeFore = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text color of the selected node when not focussed.
        /// </summary>
        /// <value>Default is a system color.</value>
        [
        Description("Indicates the text color of the selected node when not focused."),
        Category("Appearance")
        ]
        public virtual Color InactiveSelectedNodeForeColor
        {
            get
            {
                return m_clrInactiveSelectedNodeFore;
            }
            set
            {
                m_clrInactiveSelectedNodeFore = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background of the selected node.
        /// </summary>
        /// <value>Default is based on a system color.</value>
        [
        Description("Indicates the background of the selected node."),
        Category("Appearance")
        ]
        public virtual BrushInfo SelectedNodeBackground
        {
            get
            {
                return m_selectedNodeBackground;
            }
            set
            {
                if (value != m_selectedNodeBackground)
                {
                    m_selectedNodeBackground = (value == null) ? BrushInfo.Empty : value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the background of the selected node when the control is not focussed.
        /// </summary>
        /// <value>Default is based on a system color.</value>
        [
        Description("Indicates the background of the selected node when the control is not focused."),
        Category("Appearance")
        ]
        public virtual BrushInfo InactiveSelectedNodeBackground
        {
            get
            {
                return m_inactiveSelectedNodeBackground;
            }
            set
            {
                if (m_inactiveSelectedNodeBackground != value)
                {
                    m_inactiveSelectedNodeBackground = (value == null) ? BrushInfo.Empty : value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the drag-drop operation will occur only if the node is dragged on the text area.
        /// </summary>
        /// <value>Default is true.</value>
        [
        Description("Indicates if the drag-drop operation will occur only if the node is dragged on the text area."),
        DefaultValue(true),
        Category("Behavior")
        ]
        public virtual bool DragOnText
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.DragOnText];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.DragOnText] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node has no children.
        /// </summary>
        /// <value>An index into the <see cref="StateImageList"/>. Default is zero.</value>
        [
        Description("Specifies the StateImageList index value of the image that is displayed when a tree node has no children."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor)),
        Localizable(true)
        ]
        public virtual int NoChildrenImgIndex
        {
            get
            {
                if (this.StandardStyle != null)
                {
                    return this.StandardStyle.NoChildrenImgIndex;
                }

                return -1;
            }
            set
            {
                if (this.NoChildrenImgIndex != value)
                {
                    this.StandardStyle.NoChildrenImgIndex = value;
                    this.Root.RecalculateAllDimensions();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node is collapsed.
        /// </summary>
        /// <value>An index into the <see cref="StateImageList"/>. Default is 1.</value>
        [
        Description("Specifies the StateImageList index value of the image that is displayed when a tree node is collapsed."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor)),
        Localizable(true)
        ]
        public virtual int ClosedImgIndex
        {
            get
            {
                return this.StandardStyle.ClosedImgIndex;
            }
            set
            {
                if (this.ClosedImgIndex != value)
                {
                    this.StandardStyle.ClosedImgIndex = value;
                    this.Root.RecalculateAllDimensions();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StateImageList"/> index value of the image that is displayed when a tree node is expanded.
        /// </summary>
        /// <value>An index into the <see cref="StateImageList"/>. Default is 2.</value>
        [
        Description("Specifies the StateImageList index value of the image that is displayed when a tree node is expanded."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        Editor(typeof(StateImageListUITypeEditor), typeof(UITypeEditor)),
        Localizable(true)
        ]
        public virtual int OpenImgIndex
        {
            get
            {
                return this.StandardStyle.OpenImgIndex;
            }
            set
            {
                if (this.OpenImgIndex != value)
                {
                    this.StandardStyle.OpenImgIndex = value;
                    this.Root.RecalculateAllDimensions();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the imagelist that holds images to be drawn based on the state of the node.
        /// </summary>
        /// <remarks>The <see cref="OpenImgIndex"/>, <see cref="ClosedImgIndex"/> and
        /// <see cref="NoChildrenImgIndex"/> properties refer to an image inside this list.</remarks>
        [
        Description("Indicates the imagelist that holds images to be drawn based the state of the node."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(null)
        ]
        public virtual ImageList StateImageList
        {
            get
            {
                return m_stateImageList;
            }
            set
            {
                m_stateImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets the imagelist that holds images to be drawn on the right of the node.
        /// </summary>
        /// <remarks>The <see cref="TreeNodeAdv.RightImageIndices"/> will then indicate
        /// which images are to be drawn in the node.</remarks>
        [
        Description("Indicates the imagelist that holds images to be drawn on the right of the node."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(null)
        ]
        public virtual ImageList RightImageList
        {
            get
            {
                return m_rightImageList;
            }
            set
            {
                m_rightImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets the imagelist that holds images to be drawn on the left of the node.
        /// </summary>
        /// <remarks>The <see cref="TreeNodeAdv.LeftImageIndices"/> will then indicate
        /// which images are to be drawn in the node.</remarks>
        [
        Description("Indicates the imagelist that holds images to be drawn on the left of the node."),
        Category("Appearance - Images"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(null)
        ]
        public virtual ImageList LeftImageList
        {
            get
            {
                return m_leftImageList;
            }
            set
            {
                m_leftImageList = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the controls (eg PlusMinus) will have a transparent background.
        /// Setting this property slows down drawing of the MultiColumnTreeView control.
        /// </summary>
        /// <value>Default is false.</value>
        [
        Description("Indicates if the controls (eg PlusMinus) will have a transparent background."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public virtual bool TransparentControls
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.TransparentControls];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.TransparentControls] != value)
                {
                    m_boolean[(int)InternalBooleanFields.TransparentControls] = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the selected node will be brought to view by scrolling, if necessary.
        /// </summary>
        /// <value>Default is true.</value>
        [
        Description("Indicates if the selected node will be brought to view by scrolling, if necessary."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public virtual bool EnsureVisibleSelectedNode
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.EnsureVisibleSelectedNode];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.EnsureVisibleSelectedNode] = value;
            }
        }

        /// <summary>
        /// Gets or sets the selection mode for the tree.
        /// </summary>
        /// <value>Default is <b>TreeSelectionMode.Single</b>.</value>
        /// <remarks>
        /// Note that setting this property does not affect the current selection state.
        /// For example, if the current selection includes multiple nodes and this property gets set
        /// to <b>TreeSelectionMode.Single</b>, then the <see cref="SelectionNodes"/>
        /// will not be cleared to show a single selection.
        /// </remarks>
        [
        Description("Indicates the selection mode for the tree."),
        Category("Behavior"),
        DefaultValue(TreeSelectionMode.Single)
        ]
        public virtual TreeSelectionMode SelectionMode
        {
            get
            {
                return m_selectionMode;
            }
            set
            {
                if (m_selectionMode != value)
                {
                    m_selectionMode = value;
                    OnSelectionModeChanged();
                }
            }
        }

        /// <summary>
        /// Gets a collection of base styles used in the tree.
        /// </summary>
        /// <value>A Hashtable of style names versus styles. The style names are of type string and
        /// the styles are of type <see cref="TreeNodeAdvStyleInfo"/>.</value>
        /// <remarks>
        /// This collection holds the standard style that specifies the global node settings
        /// for all the nodes (is named "Standard"), the node level styles for nodes at specific levels (should use the
        /// convention "NodeLevelX") and other custom base styles. Also when you specify a style named
        /// "DragNodeCueStyle" that style will be applied on the nodes before preparing the 
        /// drag-cue bitmap during drag-and-drop, a feature that can be turned on using the <see cref="ShowDragNodeCue"/> property.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Editor(typeof(TreeNodeAdvBaseStylesEditor), typeof(UITypeEditor)),
        Category("Styles"),
        Description("Collection of base styles is used by nodes, columns and subitems.")
        ]
        public virtual Hashtable BaseStyles
        {
            get
            {
                return m_baseStyles;
            }
        }

        [
        DocumentationExclude(),
        Category("Styles"),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public StyleNamePairsList BaseStylePairs
        {
            get
            {
                // Need to recreate stylePairs list instead of clearing its items.
                // Exception during code dom serialization process.
                // Fix for defect 2639
                m_stylePairs = new StyleNamePairsList(this);

                foreach (string styleName in m_baseStyles.Keys)
                {
                    StyleNamePair pair = new StyleNamePair(styleName, m_baseStyles[styleName] as IStyleInfo);
                    m_stylePairs.Add(pair);
                }

                return m_stylePairs;
            }
        }

        /// <summary>
        /// Gets the standard style that all the nodes inherit from, by default.
        /// </summary>
        [
        Category("Styles"),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Can also be edited via the BaseStyles property editor. It is the style that all the nodes inherit from, by default.")
        ]
        public TreeNodeAdvStyleInfo StandardStyle
        {
            get
            {
                return m_baseStyles[MultiColumnTreeView.DefaultBaseStyleName] as TreeNodeAdvStyleInfo;
            }
        }

        /// <summary>
        /// Gets the standard style that all columns inherit from, by default.
        /// </summary>
        [
        Category("Styles"),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Can also be edited via the BaseStyles property editor. It is the style that all the columns inherit from, by default.")
        ]
        public TreeColumnAdvStyleInfo StandardColumnStyle
        {
            get
            {
                return m_baseStyles[MultiColumnTreeView.DefaultColumnStyleName] as TreeColumnAdvStyleInfo;
            }
        }

        /// <summary>
        /// Gets the standard style that all sub-items inherit from, by default.
        /// </summary>
        [
        Category("Styles"),
        Browsable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Can also be edited via the BaseStyles property editor. It is the style that all the subitems inherit from, by default.")
        ]
        public TreeNodeAdvSubItemStyleInfo StandardSubItemStyle
        {
            get
            {
                return m_baseStyles[MultiColumnTreeView.DefaultSubItemStyleName] as TreeNodeAdvSubItemStyleInfo;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="TreeNodeAdv.GetPath"/> method adds a separator at the end of the path string returned.
        /// </summary>
        /// <value>Default is false.</value>
        [
        Description("Indicates if the TreeNodeAdv.GetPath method adds a separator at the end of the path string returned."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool AddSeparatorAtEnd
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AddSeparatorAtEnd];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.AddSeparatorAtEnd] = value;
            }
        }

        /// <summary>
        /// Gets or sets the space left on the left side of the control.
        /// </summary>
        /// <value>Default is 3.</value>
        [
        Description("Indicates the space left on the left side of the control."),
        Category("Appearance"),
        DefaultValue(3)
        ]
        public virtual int GutterSpace
        {
            get
            {
                return m_gutterSpace;
            }
            set
            {
                if (m_gutterSpace != value)
                {
                    m_gutterSpace = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the label text of the tree nodes can be edited.
        /// </summary>
        /// <value>True if the label text of the tree nodes can be edited; false otherwise. The default is false.</value>
        /// <remarks>
        /// The <see cref="BeginEdit()"/> method will let you begin editing a node
        /// programmatically irrespective of this setting.
        /// </remarks>
        [
        Description("Gets or sets a value indicating whether the label text of the tree nodes can be edited."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool LabelEdit
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.LabelEdit];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.LabelEdit] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="BeforeNodePaint"/> and <see cref="AfterNodePaint"/> events will be fired before drawing a node.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if the BeforeNodePaint event will be fired before drawing a node."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool OwnerDrawNodes
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.OwnerDrawNodes];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.OwnerDrawNodes] = value;
            }
        }

        /// <summary>
        ///Gets or sets a value indicating whether the <see cref="NodeBackgroundPaint"/> event will be fired before drawing a node's background.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if the NodeBackgroundPaint event will be fired before drawing a node's background."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool OwnerDrawNodesBackground
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.OwnerDrawNodesBackground];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.OwnerDrawNodesBackground] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree should follow the load-on-demand paradigm.
        /// </summary>
        /// <value>Default value is false.</value>
        /// <remarks><para>When set to true, all the nodes will have the plus-minus set to visible to begin with. 
        /// You should then handle the <see cref="BeforeExpand"/> event of the nodes and add subnodes to the respective nodes.
        /// The tree will then keep or hide the plus-minus based on whether or not children were added.</para><para>This provides you a way to delay loading nodes in trees until the user initiates a node expand.</para></remarks>
        [
        Description("Specifies if the tree should follow the load-on-demand paradigm."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool LoadOnDemand
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.LoadOnDemand];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.LoadOnDemand] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MultiColumnTreeView is printing.
        /// </summary>
        [
        Description("Indicates if the MultiColumnTreeView is printing."),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool Printing
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.Printing];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.Printing] = value;
            }
        }

        /// <summary>
        /// Gets the PrintDocument of the MultiColumnTreeView.
        /// <remarks>when you use print document it will give the snap shot only 
        /// so use printpreview</remarks>
        /// </summary>
        public PrintDocument PrintDocument
        {
            get
            {
                if (m_printDocument == null)
                {
                    m_printDocument = new PrintDocument();
                    m_printDocument.PrintPage += new PrintPageEventHandler(OnPrint);
                }

                return m_printDocument;
            }
        }
        public Image ToImage()
        {
            PrepareTreeImage(this);
            return m_imgControl;
        }
        public void PrintPreview()
        {
            PrintPreviewTree(this, String.Empty);
        }

        /// <summary>
        /// Shows a PrintPreview dialog displaying the Tree control passed in.
        /// </summary>
        /// <param name="pTreeView">MultiColumnTreeView for print preview.</param>
        /// <param name="pTitle">Title for document.</param>
        private void PrintPreviewTree(MultiColumnTreeView pTreeView, string pTitle)
        {
            String m_sTitle = pTitle;
            PrepareTreeImage(pTreeView);

            PrintPreviewDialog dialog = new PrintPreviewDialog();
            dialog.Document = m_pdDocument;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Gets an image that shows the entire tree, not just what is visible on the Control
        /// </summary>
        /// <param name="tree"></param>
        private void PrepareTreeImage(MultiColumnTreeView pTreeView)
        {
            if (pTreeView != null && pTreeView.Nodes.Count > 0)
            {
                MultiColumnTreeView treeView = new MultiColumnTreeView();
                TreeColumnAdv[] treecolumn = new TreeColumnAdv[pTreeView.Columns.Count];
                for (int i = 0; i < pTreeView.Columns.Count; i++)
                {
                    treecolumn[i] = new TreeColumnAdv();
                }
                treeView.Root = pTreeView.Root;
                for (int i = 0; i < pTreeView.Columns.Count; i++)
                {

                    treeView.Columns.Add(pTreeView.Columns[i]);
                    treeView.Columns[i].Text = pTreeView.Columns[i].Text;
                }
                treeView.BackgroundImage = pTreeView.BackgroundImage;
                treeView.HideSelection = pTreeView.HideSelection;
                treeView.Indent = pTreeView.Indent;
                treeView.ShowColumnsHeader = false;
                treeView.InteractiveCheckBoxes = pTreeView.InteractiveCheckBoxes;
                treeView.ItemHeight = pTreeView.ItemHeight;
                treeView.LineColor = pTreeView.LineColor;
                treeView.LineStyle = pTreeView.LineStyle;
                treeView.RightToLeft = pTreeView.RightToLeft;
                treeView.ShowLines = pTreeView.ShowLines;
                treeView.ShowRootLines = pTreeView.ShowRootLines;
                treeView.ShowCheckBoxes = pTreeView.ShowCheckBoxes;
                treeView.ShowOptionButtons = pTreeView.ShowOptionButtons;
                treeView.ShowPlusMinus = pTreeView.ShowPlusMinus;
                treeView.ThemesEnabled = pTreeView.ThemesEnabled;
                treeView.NodeCount = pTreeView.NodeCount;
                treeView.Font = pTreeView.Font;
                treeView.ForeColor = pTreeView.ForeColor;
                treeView.Margin = pTreeView.Margin;
                treeView.ThemedBorder = pTreeView.ThemedBorder;
                foreach (string name in pTreeView.BaseStyles.Keys)
                {
                    treeView.BaseStyles[name] = pTreeView.BaseStyles[name];
                }
                treeView.NodeStateImageList = pTreeView.NodeStateImageList;
                treeView.DefaultCollapseImageIndex = pTreeView.DefaultCollapseImageIndex;
                treeView.DefaultExpandImageIndex = pTreeView.DefaultExpandImageIndex;
                treeView.LeftImageList = pTreeView.LeftImageList;
                treeView.RightImageList = pTreeView.RightImageList;
                treeView.StateImageList = pTreeView.StateImageList;
                treeView.BackgroundColor = pTreeView.BackgroundColor;
                m_iScrollBarWidth = pTreeView.Width - pTreeView.ClientSize.Width;
                m_iScrollBarHeight = pTreeView.Height - pTreeView.ClientSize.Height;
                int iHeight = 0;
                if (treeView.Nodes[0] != null)
                    iHeight = treeView.Nodes[0].Height;
                m_iNodeHeight = treeView.ItemHeight;
                int iWidth = 0;
                TreeNodeAdv node = new TreeNodeAdv();
                if (pTreeView.Columns.Count != 0)
                {
                    node = treeView.Nodes[0].NextSelectableNode;
                    for (int i = 0; i < pTreeView.Columns.Count; i++)
                    {
                        treecolumn[i] = treeView.Columns[i];
                        treecolumn[i].Text = treeView.Columns[i].Text;
                    }
                    while (node != null)
                    {
                        m_iNodeHeight = node.Height;
                        iHeight += m_iNodeHeight;

                        if (treeView.RightToLeft == RightToLeft.No)
                        {
                            if (node.PrintTextBounds.Right > iWidth)
                            {
                                iWidth = node.PrintTextBounds.Right;
                            }
                        }
                        node = node.NextSelectableNode;
                    }
                    treeView.Columns[0].Width = iWidth;
                    iWidth = iWidth * pTreeView.Columns.Count;
                }
                else
                {
                    iWidth = treeView.Nodes[0].PrintTextBounds.Right;
                    node = treeView.Nodes[0].NextSelectableNode;
                    while (node != null)
                    {
                        iHeight += m_iNodeHeight;

                        if (treeView.RightToLeft == RightToLeft.No)
                        {
                            if (node.PrintTextBounds.Right > iWidth)
                            {
                                iWidth = node.PrintTextBounds.Right;
                            }
                        }
                        else
                        {
                            if (node.PrintTextBounds.Right < 0)
                            {
                                iWidth = (-node.PrintTextBounds.Right) + treeView.ClientRectangle.Width;
                            }
                        }
                        node = node.NextSelectableNode;
                    }
                }
                treeView.SelectedNode = null;
                treeView.Height = iHeight + m_iScrollBarHeight;
                treeView.Width = iWidth + m_iScrollBarWidth;
                treeView.BorderStyle = BorderStyle.None;
                treeView.Dock = DockStyle.None;
                treeView.Office2007ScrollBars = false;
                treeView.HScrollBar.Enabled = false;
                treeView.VScrollBar.Enabled = false;
                treeView.VScroll = false;
                treeView.HScroll = false;
                m_imgControl = GetImage(treeView.Handle, treeView.Width, treeView.Height);
            }

            //give the window time to update
            //Application.DoEvents();           
        }

        /// <summary>
        /// Returns an image of the specified width and height, of a control represented by handle.
        /// </summary>
        /// <param name="pHandle"></param>
        /// <param name="pWidth"></param>
        /// <param name="pHeight"></param>
        /// <returns>Returns Image</returns>
        private Image GetImage(IntPtr pHandle, int pWidth, int pHeight)
        {
            IntPtr screenDC = NativeMethods.GetDC(IntPtr.Zero);
            IntPtr bmp = NativeMethods.CreateCompatibleBitmap(screenDC, pWidth, pHeight);
            Image img = Bitmap.FromHbitmap(bmp);
            try
            {                
                using (Graphics g = Graphics.FromImage(img))
                {
                    IntPtr hdc = g.GetHdc();
                    try
                    {
                        NativeMethods.SendMessage(pHandle, 0x0318 /*WM_PRINTCLIENT*/, hdc, (0x00000010 | 0x00000004 | 0x00000002));
                    }
                    finally
                    {
                        g.ReleaseHdc(hdc);
                    }
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);
            }
            return img;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the nodes will have an option button.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if the nodes will have an option button."),
        Category("Appearance"),
        DefaultValue(false)
       ]
        public virtual bool ShowOptionButtons
        {
            get
            {
                return this.StandardStyle.ShowOptionButton;
            }
            set
            {
                this.StandardStyle.ShowOptionButton = value;
                this.Root.RecalculateAllDimensions();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets a value indicating whether selected node is in editing mode.
        /// </summary>
        [
        Description("Indicates if the selected node is in editing mode."),
        Category("Appearance"),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public bool IsEditing
        {
            get
            {
                return m_labelEditor.Visible;
            }
        }

        /// <summary>
        /// Gets or sets the separator string that splits the path of a node. 
        /// Call <see cref="TreeNodeAdv.GetPath"/> to get the path of the specified node.
        /// </summary>
        /// <value>Default value is "\".</value>
        [
        Description("Indicates the separator string that splits the path of a node."),
        Category("Appearance"),
        DefaultValue("\\"),
        Localizable(true)
        ]
        public virtual string PathSeparator
        {
            get
            {
                return m_strPathSeparator;
            }
            set
            {
                m_strPathSeparator = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the plus minus controls are visible.
        /// </summary>
        /// <value>Default value is true.</value>
        [
        Description("Indicates if the plus minus controls are visible."),
        Category("Appearance"),
        DefaultValue(true)
        ]
        public virtual bool ShowPlusMinus
        {
            get
            {
                return this.StandardStyle.ShowPlusMinus;
            }
            set
            {
                this.StandardStyle.ShowPlusMinus = value;
                this.Root.UpdateAllPlusMinusVisibility();
                this.Root.RecalculateAllDimensions();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tree lines are visible.
        /// </summary>
        /// <value>Default value is true.</value>
        [
        Description("Indicates if the tree lines are visible."),
        Category("Appearance"),
        DefaultValue(true)
        ]
        public virtual bool ShowLines
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ShowLines];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.ShowLines] != value)
                {
                    m_boolean[(int)InternalBooleanFields.ShowLines] = value;

                    if (this.Root != null)
                    {
                        this.Root.RecalculateAllDimensions();
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether lines are drawn between the tree nodes that are at the root of the tree view.
        /// </summary>
        /// <value>Default value is true.</value>
        [
        Description("Indicates whether lines are displayed between root nodes."),
        Category("Appearance"),
        DefaultValue(true)
        ]
        public virtual bool ShowRootLines
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ShowRootLines];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.ShowRootLines] != value)
                {
                    m_boolean[(int)InternalBooleanFields.ShowRootLines] = value;

                    UpdateRootPlusMinusVisibility();

                    if (this.Root != null)
                    {
                        this.Root.RecalculateAllDimensions();
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether header drawn or not.</summary>
        /// <value>Default value is True.</value>
        [
        Description("Indicates whether header drawn or not."),
        Category("Appearance"),
        DefaultValue(true)
        ]
        public virtual bool ShowColumnsHeader
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ShowHeader];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.ShowHeader] != value)
                {
                    m_boolean[(int)InternalBooleanFields.ShowHeader] = value;

                    if (this.IsHandleCreated)
                    {
                        RecreateHandle();
                        InvalidateNc();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the indent of the child nodes from the parent node.
        /// </summary>
        /// <value>Default value is 19.</value>
        [
        Description("Indicates the indent of the child nodes from the parent node."),
        Category("Appearance"),
        DefaultValue(19)
        ]
        public virtual int Indent
        {
            get
            {
                return m_parentIndent;
            }
            set
            {
                if (m_parentIndent != value)
                {
                    m_parentIndent = value;

                    if (null != Root)
                    {
                        this.Root.RecalculateAllDimensions();
                    }

                    Invalidate();
                }
            }

        }

        /// <summary>
        /// Gets or sets a value indicating whether the nodes will have a hot tracked appearance when the mouse cursor is hovering over them.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if the nodes will have a hot tracked appearance when the mouse cursor is hovering over them."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public virtual bool HotTracking
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.HotTracking];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.HotTracking] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple nodes can be selected with mouse down and drag.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates multiple nodes can be selected with mouse down and drag."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool AllowMouseBasedSelection
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AllowMouseBasedSelection];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.AllowMouseBasedSelection] = value;
            }
        }

        /// <summary>
        /// Gets the collection of Nodes which are in Expanded state. Result is 
        /// calculated on reach call.
        /// </summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvCollection ExpandedNodes
        {
            get
            {
                TreeNodeAdvCollection collection = new TreeNodeAdvCollection(null);
                iterate(this.Nodes[0], true, collection);
                return collection;
            }
        }

        /// <summary>
        /// Gets the collection of Nodes which are in Collapsed state. Result is 
        /// calculated on reach call.
        /// </summary>
        [
        Browsable(false)
        ]
        public TreeNodeAdvCollection CollapsedNodes
        {
            get
            {
                TreeNodeAdvCollection collection = new TreeNodeAdvCollection(null);
                iterate(this.Nodes[0], false, collection);
                return collection;
            }
        }

        /// <summary>
        /// Gets the selected nodes of the MultiColumnTreeView.
        /// </summary>
        /// <remarks>Use this property only when <see cref="SelectionMode"/> property
        /// lets you select multiple nodes. Otherwise, use <see cref="SelectedNode"/> to get the single selected node.</remarks>
        [
        Description("Indicates the selected nodes of the MultiColumnTreeView."),
        Category("Behavior"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public SelectedNodesCollection SelectedNodes
        {
            get
            {
                return m_selectedNodes;
            }
        }

        /// <summary>
        /// Gets the checked nodes of the MultiColumnTreeView.
        /// </summary>
        [
        Description("Indicates the checked nodes of the MultiColumnTreeView."),
        Category("Behavior"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public CheckedNodesColection CheckedNodes
        {
            get
            {
                return m_checkedNodes;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the MultiColumnTreeView will hide it's selected nodes when not focussed.
        /// </summary>
        /// <value>True to hide selection; false otherwise. Default value is true.</value>
        [
        Description("Indicates if the MultiColumnTreeView will hide it's selected nodes when not focused."),
        Category("Appearance"),
        DefaultValue(true)
        ]
        public virtual bool HideSelection
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.HideSelection];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.HideSelection] != value)
                {
                    m_boolean[(int)InternalBooleanFields.HideSelection] = value;
                    if (!Focused)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the complete row will be highlighted when a node is selected.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if the complete row will be highlighted when a node is selected."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public virtual bool FullRowSelect
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.FullRowSelect];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.FullRowSelect] != value)
                {
                    m_boolean[(int)InternalBooleanFields.FullRowSelect] = value;
                    if (m_selectedNodes.Count > 0)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the tree lines.
        /// </summary>
        /// <value>Default is Color.Gray.</value>
        [
        Description("Indicates the color of the tree lines."),
        Category("Appearance")
        ]
        public virtual Color LineColor
        {
            get
            {
                return m_clrLine;
            }
            set
            {
                if (m_clrLine != value)
                {
                    m_clrLine = value;
                    m_penLine = new Pen(value);
                    m_penLine.DashStyle = m_lineStyle;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the line style of the tree lines.
        /// </summary>
        /// <value>Default value is DashStyle.Dot.</value>
        [
        Description("Indicates the line style of the tree lines."),
        Category("Appearance"),
        DefaultValue(DashStyle.Dot)
        ]
        public virtual DashStyle LineStyle
        {
            get
            {
                return m_lineStyle;
            }
            set
            {
                if (value == DashStyle.Custom)
                {
                    if (this.DesignMode)
                    {
                        MessageBox.Show("You can't set the LineStyle to Custom");
                    }
                    return;
                }
                if (m_lineStyle != value)
                {
                    m_lineStyle = value;
                    m_penLine.DashStyle = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether keyboard based searching should be allowed.
        /// </summary>
        /// <value>Default value is true.</value>
        /// <remarks>
        /// When set to true, the users can key in char keys to browse to the next node that begins with
        /// that character(s). Multiple characters entered in succession will be assumed to be part of the
        /// same word, so search will be performed on that substring. Search will be restricted to 
        /// <see cref="TreeNodeAdv.IsVisible"/> and <see cref="TreeNodeAdv.Expanded"/> nodes.
        /// </remarks>
        [
        Description("Gets/sets a value to indicate if keyboard based searching should be allowed."),
        DefaultValue(true),
        Category("Behavior")
        ]
        public virtual bool AllowKeyboardSearch
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AllowKeyboardSearch];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.AllowKeyboardSearch] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state of the parent node's checkbox is based on the checkstate of it's child nodes' checkboxes.
        /// </summary>
        /// <value>Default value is false.</value>
        /// <remarks>
        /// If all child nodes are checked the parent node is also checked. The same with unchecked.
        /// If some child nodes are checked and some are unchecked then the parent node will have an indeterminate state.
        /// If the CheckState of the parent node is set by code or by clicking on it the state of all subnodes will be set to that state.
        /// </remarks>
        [
        Description("Indicates if the state of the a node's checkbox indicates the checkstate of the child nodes checkboxes."),
        Category("Behavior"),
        DefaultValue(false)
        ]
        public virtual bool InteractiveCheckBoxes
        {
            get
            {
                return this.StandardStyle.InteractiveCheckBox;
            }
            set
            {
                this.StandardStyle.InteractiveCheckBox = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the collapsed node should be selected if any of the child node is selected or not.
        /// </summary>
        /// <value><c>true</c> if the collapsed node should be selected if that node has a selected child node; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// If this property is set to false, it won't trigger the <see cref="BeforeSelect"/> and <see cref="AfterSelect"/> event after collapsing the node. 
        /// </remarks>
        [
        Description("Indicates whether the node should be selected if it's child node is selected or not."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool SelectOnCollapse
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.SelectOnCollapse];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.SelectOnCollapse] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checkboxes will be shown for the nodes.
        /// </summary>
        /// <value>Default value is false.</value>
        [
        Description("Indicates if check boxes will be shown for the nodes."),
        Category("Appearance")
        ]
        public virtual bool ShowCheckBoxes
        {
            get
            {
                return this.StandardStyle.ShowCheckBox;
            }
            set
            {
                this.StandardStyle.ShowCheckBox = value;

                if (this.Root != null)
                {
                    this.Root.RecalculateAllDimensions();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected node of the MultiColumnTreeView.
        /// </summary>
        /// <remarks><para>The tree fires the <see cref="BeforeSelect"/> event to let you cancel the change 
        /// and <see cref="AfterSelect"/> event to notify you of a new selected node.</para><para>Use to <see cref="SelectedNodes"/> property when multi-node selection is turned on.</para></remarks>
        [
        Description("Indicates the selected node of the MultiColumnTreeView."),
        Category("Behavior"),
        DefaultValue(null),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always)
        ]
        public virtual TreeNodeAdv SelectedNode
        {
            get
            {
                if (this.SelectedNodes.Count > 0)
                {
                    return this.SelectedNodes[0];
                }

                return null;
            }
            set
            {
                if (value == this.Root)
                {
                    value = null;
                }

                if ((m_selectedNodes.Count == 0 || m_selectedNodes[0] != value)
          && SetSelectedNode(value, m_selectedNodes, TreeViewAdvAction.Unknown))
                {
                    this.ActiveNode = value;
                    m_selectionBaseNode = value;

                    if (this.ActiveNode != null)
                    {
                        this.ActiveNode.BringIntoView();
                    }
                }

                this.IsBroughtIntoView = false;
            }
        }

        /// <summary>
        /// Gets the base node, based on which multiple selection will be performed.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> instance or null if there is no such node.</value>
        /// <remarks>
        /// This node will be consulted while extending the selection in a multi-select
        /// scenario using user interaction or when calling the <see cref="ExtendSelectionTo(TreeNodeAdv, bool)"/> method.
        /// </remarks>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("The base node, based on which multiple selection will be performed."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual TreeNodeAdv SelectionBaseNode
        {
            get
            {
                return m_selectionBaseNode;
            }
        }

        /// <summary>
        /// Gets or sets the default height of the nodes.
        /// </summary>
        /// <value>Default value is dependent on the control's font height.</value>
        [
        Description(@"Indicates the default height of the nodes."),
        Category("Appearance")
        ]
        public virtual int ItemHeight
        {
            get
            {
                if (m_itemHeight != -1)
                {
                    return m_itemHeight;
                }
                else
                {
                    return base.FontHeight + 3;
                }
            }
            set
            {
                if (m_itemHeight != value)
                {
                    //root.SetHeightIfChanged(itemHeight,value);
                    m_itemHeight = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the root node of the MultiColumnTreeView.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        DocumentationExclude()
        ]
        public virtual TreeNodeAdv Root
        {
            get
            {
                return m_root;
            }
            set
            {
                m_root = value;
                m_root.TreeView = this;

                RefreshCustomControlCollection();

                m_root.Visible = true;
                UpdateRootPlusMinusVisibility();
                m_root.RecalculateAllDimensions();
                m_root.Expand();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets the top-level nodes collection of the MultiColumnTreeView.
        /// </summary>
        [
        Description("Indicates the top-level nodes of the MultiColumnTreeView."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public virtual TreeNodeAdvCollection Nodes
        {
            get
            {
                return m_root.Nodes;
            }
        }

        /// <summary>
        /// Gets / sets the columns of the MultiColumnTreeView.
        /// </summary>
        [
        Description("Collection of columns is used by MultiColumnTreeView."),
        Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public virtual TreeColumnAdvCollection Columns
        {
            get
            {
                return m_columns;
            }
        }

        /// <summary>
        /// Gets or sets the node on which the user did a right-mouse down.
        /// </summary>
        /// <value>A <see cref="TreeNodeAdv"/> instance.</value>
        /// <remarks><para>This property will return a non-null value only when the user
        /// has his mouse down or when the context menu is being shown for the tree.</para><para>Use this property in your context-menu's popup event to determine on which
        /// node the user had right-clicked. However, do not use this property in a context menu
        /// item's Click property as this would be set to null by then. If the user right-clicked in the empty region then
        /// this property will return null.</para><para>
        /// When the user instead used the keyboard to invoke the context menu (via Shift+F10)
        /// then this property will return the currently selected node and the
        /// menu will also appear beside the selected node.
        /// </para></remarks>
        [
        Description("Indicates the node on which the user did a right-mouse down."),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual TreeNodeAdv RMouseDownNode
        {
            get
            {
                return m_rMouseDownNode;
            }
            set
            {
                if (m_rMouseDownNode != value)
                {
                    m_rMouseDownNode = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Indicates whether the control should scroll while the user is dragging a horizontal scrollbar thumb.
        /// </summary>
        [
        Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should scroll while the user is dragging a horizontal scrollbar thumb."),
        DefaultValue(true)
        ]
        public override bool HorizontalThumbTrack
        {
            get
            {
                return base.HorizontalThumbTrack;
            }
            set
            {
                base.HorizontalThumbTrack = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Recalculation of the Nodes maximum height should be done while expanding/collapsing.
        /// </summary>
        /// <value><c>true</c> if suspend recalculate the nodes hieght while expand/collapse; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property can be reduced the delay while expanding/ collapsing the large number of nodes, if we set it as true.
        /// </remarks>
        [
        Browsable(true),
        Category("Behavior"),
        Description("Gets or sets a value indicating whether the Recalculation of the Nodes maximum height should be done while expanding/collapsing."),
        DefaultValue(false)
        ]
        public bool SuspendExpandRecalculate
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.SuspendExpandRecalculate];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.SuspendExpandRecalculate] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the size box should be drawn when both scrollbars are visible
        /// and the control is not a docked window in an MDIChild window. Note: Another better solution is drawing the NonClientArea
        /// ourselves. See SizeGripStyle which implements this newer solution.
        /// </summary>
        /// <remarks>
        /// Showing the size box works around a problem with .NET controls because by
        /// default the the area at the bottom right is not drawn and that can cause
        /// drawing glitches. Note: Another better solution is drawing NonClientArea
        /// ourselves. See SizeGripStyle which implements this newer solution.
        /// </remarks>
        [
        DefaultValue(false),
        Browsable(false),
        Category("Scrolling"),
        Description("Specifies if size box should be drawn when both scrollbars are visible and the control is not docked in an MDIChild window.")
        ]
        public new bool SmartSizeBox
        {
            get
            {
                return base.SmartSizeBox;
            }
            set
            {
                base.SmartSizeBox = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether If tree in design mode than all unvisible node should be displayed.
        /// </summary>
        protected internal bool DesignModeInternal
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.DesignMode];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.DesignMode] = value;
            }
        }

        /// <summary>
        /// Indicates whether the control should scroll while the user is dragging a vertical scrollbar thumb.
        /// </summary>
        [
        Browsable(true),
        Category("Scrolling"),
        Description("Specifies if the control should scroll while the user is dragging a vertical scrollbar thumb."),
        DefaultValue(true)
        ]
        public override bool VerticalThumbTrack
        {
            get
            {
                return base.VerticalThumbTrack;
            }
            set
            {
                base.VerticalThumbTrack = value;
            }
        }

        /// <summary>
        /// Gets or sets ImageList with images that are displayed 
        /// instead of expand/collapse button.
        /// </summary>
        /// The below description helps the user  to set Custom images for expand/collapse (+/-) signs in the MultiColumnTreeView
        /// The standard  +/- signs for the expand/collapse buttons in the MultiColumnTreeView can be replaced with 
        /// the custom  images by setting ImageList to the newly added NodeStateImageList property of the MultiColumnTreeView.
        /// Single  click on the image expands or collapses the current node.
        /// By  setting some particular index of default image for expand/collapse button in the 
        /// TreeviewAdv's DefaultCollapseImageIndex and DefaultExpandImageIndex property  ,all the
        /// ParentNode's  can be displayed with default Images for expanding and collapsing actions.
        /// Each  Parent Node's +/- signs can be set with different images ,by setting the TreeNodeAdv's
        /// CollpaseImageIndex  and ExpandImageIndex.
        [
        DefaultValue(null),
        Description("ImageList with images that are displayed instead of expand/collapse button."),
        Category("Appearance - Images")
        ]
        public ImageList NodeStateImageList
        {
            get
            {
                return m_nodeStateImageList;
            }
            set
            {
                if (value != m_nodeStateImageList)
                {
                    m_nodeStateImageList = value;
                    OnNodeStateImageListChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets index of default image for collapse button.
        /// </summary>
        [
        DefaultValue(DefaultImageIndex),
        Description("Index of default image for collapse button."),
        Editor(typeof(NodeStateImageListUITypeEditor), typeof(UITypeEditor)),
        Category("Appearance - Images")
        ]
        public int DefaultCollapseImageIndex
        {
            get
            {
                return m_defaultCollapseImageIndex;
            }
            set
            {
                if (value != m_defaultCollapseImageIndex)
                {
                    m_defaultCollapseImageIndex = value;
                    OnDefaultCollapseImageIndexChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets index of default image for expand button.
        /// </summary>
        [
        DefaultValue(DefaultImageIndex),
        Description("Index of default image for expand button."),
        Editor(typeof(NodeStateImageListUITypeEditor), typeof(UITypeEditor)),
        Category("Appearance - Images")
        ]
        public int DefaultExpandImageIndex
        {
            get
            {
                return m_defaultExpandImageIndex;
            }
            set
            {
                if (value != m_defaultExpandImageIndex)
                {
                    m_defaultExpandImageIndex = value;
                    OnDefaultExpandImageIndexChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default node should be selected on the MultiColumnTreeView control gains focus.
        /// </summary>
        /// <value><c>true</c> if node should be selected on MultiColumnTreeView gains focus; otherwise, <c>false</c>.
        /// </value>
        [
        Description("Gets or sets a value indicating whether the default node should be selected on the MultiColumnTreeView control gains focus."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool ShouldSelectNodeOnEnter
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ShouldSelectNodeOnEnter];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.ShouldSelectNodeOnEnter] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enables or disables horizontal scrollbar. 
        /// This property will be set/reset by the tree as and when required.
        /// </summary>
        protected bool HorizontalScrollBar
        {
            get
            {
                return base.HScrollBar.Enabled;
            }
            set
            {
                base.HScrollBar.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Enables or disables vertical scrollbar. 
        /// This property will be set/reset by the tree as and when required.
        /// </summary>
        protected bool VerticallScrollBar
        {
            get
            {
                return this.VScrollBar.Enabled;
            }
            set
            {
                this.VScrollBar.Enabled = value;
            }
        }

        [DocumentationExclude()]
        protected int ClientHeight
        {
            get
            {
                int height = TreeColumnRectangle.Height;
                // The HorizontalScrollBarHeight should be excluded already when calling ClientRectangle.Height.
                //if(HorizontalScroll)
                // height -= SystemInformation.HorizontalScrollBarHeight;
                return height;
            }
        }

        private bool needUpdateBounds = false;
        /// <summary>
        /// Gets or sets a value indicating whether bounds of the nodes collecion update need.
        /// </summary>
        internal bool NeedUpdateBounds
        {
            get { return needUpdateBounds; }
            set { needUpdateBounds = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether custom controls visibilITY and bounds update need.
        /// </summary>
        internal virtual bool NeedUpdateCustomControls
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.NeedUpdateCustomControls];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.NeedUpdateCustomControls] = value;
            }
        }

        internal int NodeCount
        {
            get
            {
                return m_iNodeCount;
            }
            set
            {
                m_iNodeCount = value;
            }
        }

        internal bool NodeInRefresh
        {
            get
            {
                return nodeInRefresh;
            }
            set
            {
                nodeInRefresh = value;
            }
        }
        internal bool IsBroughtIntoView
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.BroughtIntoView];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.BroughtIntoView] != value)
                {
                    m_boolean[(int)InternalBooleanFields.BroughtIntoView] = value;
                }
            }
        }
    
        internal TreeNodeAdvStyleInfo BoundStyle
        {
            get
            {
                return m_boundStyle;
            }
        }


        internal bool NeedRootLinesSpace
        {
            get
            {
                if (!this.ShowLines && !this.ShowPlusMinus)
                {
                    return false;
                }
                else
                {
                    return this.ShowRootLines;
                }
            }
        }

        internal Rectangle SelectedNodesBounds
        {
            get
            {
                Rectangle bounds = Rectangle.Empty;
                foreach (TreeNodeAdv node in m_selectedNodes)
                {
                    Rectangle nodeBounds = node.Bounds;
                    bounds = Rectangle.Union(bounds, nodeBounds);
                }
                return bounds;
            }
        }

        /// <summary>
        /// Gets custom control collection.
        /// </summary>
        internal Hashtable CustomControlCollection
        {
            get
            {
                return m_htCustomControlCollection;
            }
        }

        internal bool ThemedBorder
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ThemedBorder];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.ThemedBorder] = value;
            }
        }

        private bool IsVerticalGradient
        {
            get
            {
                if (m_bgBrush != null
          && m_bgBrush.Style == BrushStyle.Gradient
          && m_bgBrush.GradientStyle == GradientStyle.Vertical)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsHorizontalGradient
        {
            get
            {
                if (m_bgBrush != null
          && m_bgBrush.Style == BrushStyle.Gradient
          && m_bgBrush.GradientStyle == GradientStyle.Horizontal)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

 
        private Timer LabelEditStartTimer
        {
            get
            {
                return m_timerLabelEditStart;
            }
            set
            {
                if (m_timerLabelEditStart != null)
                {
                    m_timerLabelEditStart.Tick -= new EventHandler(this.LabelEditStartTimer_Tick);
                    m_timerLabelEditStart.Enabled = false;
                }

                if (this.ActiveNode == null || !this.LabelEdit)
                {
                    value = null;
                }

                m_timerLabelEditStart = value;

                if (m_timerLabelEditStart != null)
                {
                    m_timerLabelEditStart.Tick += new EventHandler(this.LabelEditStartTimer_Tick);
                    m_timerLabelEditStart.Interval = SystemInformation.DoubleClickTime;
                    m_timerLabelEditStart.Enabled = true;
                    m_activeNodeForLabelEdit = this.ActiveNode;
                }
            }
        }

        private bool DragCueOn
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.DragCueOn];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.DragCueOn] != value)
                {
                    m_boolean[(int)InternalBooleanFields.DragCueOn] = value;

                    if (value)
                    {
                        Bitmap bmp = this.GetDraggedNodesBitmap();

                        if (bmp != null)
                        {
                            Point pt = GetDragWindowLocation(false);
                            pt.Y += bmp.Height / 2;
                            //pt.Y += SystemInformation.CursorSize.Height;
                            DragHelper.StartDrag(bmp, pt, DragDropEffects.All);
                        }
                    }
                }
            }
        }

        private bool MultiSelect
        {
            get
            {
                return this.SelectionMode != TreeSelectionMode.Single;
            }
        }

        private bool SingleSelect
        {
            get
            {
                return !this.MultiSelect;
            }
        }


        private bool MouseBasedSelectionOn
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.MouseBasedSelectionOn];
            }
            set
            {
                if (m_boolean[(int)InternalBooleanFields.MouseBasedSelectionOn] != value)
                {
                    m_boolean[(int)InternalBooleanFields.MouseBasedSelectionOn] = value;
                    m_latestMouseBasedSelection = null;
                }
            }
        }

    
        private TreeNodeAdv LMouseDownNode
        {
            get
            {
                return m_lMouseDownNode;
            }
            set
            {
                if (m_lMouseDownNode != value)
                {
                    m_lMouseDownNode = value;
                    Invalidate();
                }
            }
        }

        /// <value>True if control has at least one column, otherwise False.</value>
        protected internal bool HasColumns
        {
            get
            {
                return (m_columns != null && m_columns.Count > 0);
            }
        }

        /// <summary>Gets or sets special measure graphics that allowing measuring without 
        /// control creation.</summary>
        protected internal Graphics MeasureGraphics
        {
            get
            {
                if (m_measure == null)
                {
                    m_measure = Graphics.FromImage(new Bitmap(1, 1));
                } //this.CreateGraphics();

                return m_measure;
            }
            set
            {
                if (m_measure != null)
                {
                    m_measure.Dispose();
                }

                m_measure = value;
            }
        }

        protected internal bool AutoControlsAdding
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AutoControlsAdding];
            }
            set
            {
                if (value != m_boolean[(int)InternalBooleanFields.AutoControlsAdding])
                {
                    m_boolean[(int)InternalBooleanFields.AutoControlsAdding] = value;
                }
            }
        }

        protected bool ClickedOnSelection
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ClickedOnSelection];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.ClickedOnSelection] = value;
            }
        }

        protected bool CanDrag
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.CanDrag];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.CanDrag] = value;
            }
        }

        protected bool PreparingDragCueBitmap
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.PreparingDragCueBitmap];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.PreparingDragCueBitmap] = value;
            }
        }

        protected bool Dragging
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.Dragging];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.Dragging] = value;
            }
        }
        protected bool MouseLeaved
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.MouseLeaved];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.MouseLeaved] = value;
            }
        }

        protected bool AllowDropStubWorks
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.AllowDropStubWorks];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.AllowDropStubWorks] = value;
            }
        }

        protected bool IgnoreNextMouseMove
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IgnoreNextMouseMove];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IgnoreNextMouseMove] = value;
            }
        }

        protected bool SelectUpwardDirection
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.SelectUpwardDirection];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.SelectUpwardDirection] = value;
            }
        }

        protected bool InVScroll
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.InVScroll];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.InVScroll] = value;
            }
        }

        protected bool InHScroll
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.InHScroll];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.InHScroll] = value;
            }
        }
        protected bool IsKeyDown
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsKeyDown];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsKeyDown] = value;
            }
        }

        protected bool IsMouseDown
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsMouseDown];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsMouseDown] = value;
            }
        }

        protected bool IsLeftMouseDown
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsLeftMouseDown];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsLeftMouseDown] = value;
            }
        }

        protected bool IsMouseUp
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsMouseUp];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsMouseUp] = value;
            }
        }

        protected bool EnsureVisibleFlag
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.EnsureVisible];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.EnsureVisible] = value;
            }
        }

        protected bool IsMouseDownWithCtrl
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsMouseDownWithCtrl];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsMouseDownWithCtrl] = value;
            }
        }
        protected bool CancelEdit
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.CancelEdit];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.CancelEdit] = value;
            }
        }
 
        protected bool NeedUpdateEditTop
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.NeedUpdateEditTop];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.NeedUpdateEditTop] = value;
            }
        }

        protected bool IgnoreLeave
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IgnoreLeave];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IgnoreLeave] = value;
            }
        }

        protected bool IsEditEnding
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.IsEditEnding];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.IsEditEnding] = value;
            }
        }
 
        protected bool CanGetDragImage
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.CanGetDragImage];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.CanGetDragImage] = value;
            }
        }
  
        protected bool ColumnsMovedMode
        {
            get
            {
                return m_boolean[(int)InternalBooleanFields.ColumnsMovedMode];
            }
            set
            {
                m_boolean[(int)InternalBooleanFields.ColumnsMovedMode] = value;
            }
        }
    
        protected DragHelper DragHelper
        {
            get
            {
                if (m_dragHelper == null)
                {
                    m_dragHelper = new DragHelper();
                }

                return m_dragHelper;
            }
            set
            {
                m_dragHelper = value;
            }
        }

        /// <summary> Gets vertical lines above plusminus rectangle</summary>
        internal Region ExcludedDrawingRegion
        {
            get
            {
                return m_drawingRgn;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// This event will be triggered when mouse hover occurs in tree nodes and it returns the particular node details which is currently being pointed from its arguement.
        /// </summary>
        public event TreeViewAdvNodeEventHandler NodeHotTrackChanged;

        protected void RaiseNodeHotTracked()
        {
            if(this.NodeHotTrackChanged != null)
                NodeHotTrackChanged(this, new TreeViewAdvNodeEventArgs(m_lastNodeOver));
        }
        /// <summary>
        /// Occures before a node gets into the edit mode.
        /// </summary>
        [
        Description("Occurs before a node gets into edit mode."),
        Category("Editor Action")
        ]
        public event TreeViewAdvBeforeEditEventHandler BeforeEdit;

        /// <summary>
        /// Occurs when the text entered by the user changes in the Node editor control.
        /// </summary>
        [
        Description("Occurs for each TextChanged event in the node editor control."),
        Category("Editor Action")
        ]
        public event TreeNodeAdvCancelableEditEventHandler NodeEditorValidateString;

        /// <summary>
        /// Occurs before the newly entered text in the Node editor gets stored.
        /// </summary>
        [
        Description("Lets you validate the new node label entered by the user."),
        Category("Editor Action")
        ]
        public event TreeNodeAdvCancelableEditEventHandler NodeEditorValidating;

        /// <summary>
        /// Occurs after the newly entered text in the Node editor gets stored.
        /// </summary>
        [
        Description("Notifies that a new label has been provided for a node by the user."),
        Category("Editor Action")
        ]
        public event TreeNodeAdvEditEventHandler NodeEditorValidated;

        /// <summary>
        /// Occurs after the Editing mode gets cancelled by Escape key.
        /// </summary>
        [
        Description("Occurs when the user cancels the editing mode."),
        Category("Editor Action")
        ]
        public event TreeNodeAdvEditEventHandler EditCancelled;

        /// <summary>
        /// Occurs before a node is selected.
        /// </summary>
        /// <remarks>
        /// The collection in the <see cref="TreeViewAdvSelectionEventArgs.SelectedNodes"/> property is 
        /// both read-only and fixed size.
        /// </remarks>
        [
        Description("Occurs before a node is selected."),
        Category("Action")
        ]
        public event TreeNodeAdvBeforeSelectEventHandler BeforeSelect;

        /// <summary>
        /// Occurs before a node's checkbox is checked.
        /// </summary>
        [
        Description("Occurs before a node's checkbox is checked."),
        Category("Action")
        ]
        public event TreeViewAdvBeforeCheckEventHandler BeforeCheck;

        /// <summary>
        /// Occurs after a node is selected.
        /// </summary>
        /// <remarks>
        /// You can determine the selected node using the <see cref="MultiColumnTreeView.SelectedNode"/> property.
        /// </remarks>
        [
        Description("Occurs after a node is selected."),
        Category("Action")
        ]
        public event EventHandler AfterSelect;

        /// <summary>
        /// Occurs after a node is checked.
        /// </summary>
        /// <remarks><para>This event will be fired when the node's <see cref="TreeNodeAdv.CheckState"/> property has changed or when a new 
        /// node has been <see cref="TreeNodeAdv.Optioned"/>.</para><para>You could alternatively listen to the individual node's 
        /// <see cref="Syncfusion.Windows.Forms.Tools.TreeNodeAdv.CheckStateChanged"/> event.</para></remarks>
        [
        Description("Occurs after a node is checked."),
        Category("Action")
        ]
        public event TreeNodeAdvEventHandler AfterCheck;

        /// <summary>
        /// Occurs after one or more node's CheckState has changed due to <see cref="TreeNodeAdv.InteractiveCheckBox"/> setting.
        /// </summary>
        /// <remarks><para>When <see cref="TreeNodeAdv.InteractiveCheckBox"/> is turned on in a parent node, changing the parent's <see cref="TreeNodeAdv.CheckState"/> or one
        /// of it's children's will cause the CheckStates of the parent and the child nodes to be updated appropriately.
        /// This event will be fired at the end of all these updates.</para></remarks>
        [
        Description("Occurs after one or more node's CheckState has changed due to TreeNodeAdv.InteractiveCheckBox setting."),
        Category("Action")
        ]
        public event TreeNodeAdvEventHandler AfterInteractiveChecks;

        /// <summary>
        /// Occurs before a node is expanded.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to do some processing of the specified node before it's expanded.
        /// Use this event when you set the <see cref="MultiColumnTreeView.LoadOnDemand"/> property to true to add child nodes to the specified node before it is expanded.
        /// </remarks>
        [
        Description("Occurs before a node is expanded."),
        Category("Action")
        ]
        public event TreeViewAdvCancelableNodeEventHandler BeforeExpand;

        /// <summary>
        /// Occurs before a node has collapsed.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to do some processing of the specified node before it's collapsed.
        /// </remarks>
        [
        Description("Occurs before a node has collapsed."),
        Category("Action")
        ]
        public event TreeViewAdvCancelableNodeEventHandler BeforeCollapse;

        /// <summary>
        /// Occurs after a node is expanded.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to do some processing of the specified node after it's expanded.
        /// </remarks>
        [
        Description("Occurs after a node is expanded."),
        Category("Action")
        ]
        public event TreeViewAdvNodeEventHandler AfterExpand;

        /// <summary>
        /// Occurs after a node has collapsed.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to do some processing of the specified node after it's collapsed.
        /// </remarks>
        [
        Description("Occurs after a node has collapsed."),
        Category("Action")
        ]
        public event TreeViewAdvNodeEventHandler AfterCollapse;

        /// <summary>
        /// Fired before a node is being painted when the <see cref="MultiColumnTreeView.OwnerDrawNodes"/> property is set to true.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to draw the node yourself. If you set the <see cref="TreeNodeAdvPaintEventArgs.Handled"/>
        /// property to true the MultiColumnTreeView assumes that you have drawn all the contents of the node and no additional drawing will be done by the MultiColumnTreeView.
        /// If you leave it to false the MultiColumnTreeView will automatically draw the usual contents of the node. Do not draw the background of the node here. 
        /// Otherwise it will draw over the vertical line. Use the NodeBackgroundPaint for painting the background.
        /// </remarks>
        [
        Description("Fired before a node is being painted when the MultiColumnTreeView.OwnerDrawNodes property is set to true."),
        Category("Appearance")
        ]
        public event TreeNodeAdvPaintEventHandler BeforeNodePaint;

        /// <summary>
        /// Fired after a node is being painted when the <see cref="MultiColumnTreeView.OwnerDrawNodes"/> property is set to true.
        /// </summary>
        /// <remarks>
        /// This event is ideal for custom drawing portions of the node in addition to the default drawing.
        /// The HandledXXX properties of the event args can be ignored for this event.
        /// </remarks>
        [
        Description("Fired after a node is being painted when the MultiColumnTreeView.OwnerDrawNodes property is set to true."),
        Category("Appearance")
        ]
        public event TreeNodeAdvPaintEventHandler AfterNodePaint;

        /// <summary>
        /// Fired to draw the background of a node if the <see cref="OwnerDrawNodesBackground"/> property is set.
        /// </summary>
        /// <remarks>
        /// Handle this event when you want to draw the background of the node yourself.
        /// </remarks>
        [
        Description("Fired to draw the background of a node if the OwnerDrawNodesBackground property is set."),
        Category("Appearance")
        ]
        public event TreeNodeAdvPaintBackgroundEventHandler NodeBackgroundPaint;

        /// <summary>
        /// Occurs when the user begins a drag of one or more items in the tree view control.
        /// </summary>
        /// <remarks><p>The <b>Item</b> property in the argument is an array of MultiColumnTreeView nodes that
        /// are currently selected.</p><p>
        /// You can choose to initiate an ole drag-and-drop operation in this event handler.
        /// </p></remarks>
        /// <example>
        /// To initiate an ole drag-drop in this event handler:
        /// <code lang="C#">
        /// // MultiColumnTreeView.ItemDrag event listener
        /// private void treeViewAdv1_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
        /// {
        ///  // Begin a drag and drop operation of the selected nodes (or some other data).
        ///  TreeNodeAdv[] nodes = e.Item as TreeNodeAdv[];
        ///  DragDropEffects result = this.DoDragDrop(nodes, DragDropEffects.Copy | DragDropEffects.Move);
        ///  // more app logic based on result...
        /// }
        /// </code><code lang="VB">
        /// ' MultiColumnTreeView.ItemDrag event listener
        /// Private Sub treeViewAdv1_ItemDrag(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemDragEventArgs) Handles treeViewAdv2.ItemDrag
        ///  ' Begin a drag and drop operation of the selected nodes (or some other data).
        ///  Dim nodes As TreeNodeAdv() = CType(e.Item, TreeNodeAdv())
        ///  Dim result As DragDropEffects = Me.DoDragDrop(nodes, DragDropEffects.Copy Or DragDropEffects.Move)
        ///  ' more app logic based on result...
        /// End Sub 'treeViewAdv1_ItemDrag '
        /// </code><para>Also take a look at our ..\Tools\Samples\Tree Package\TreeViewAdvDragDrop
        /// sample for more information on how to turn on drag-drop cues.</para></example>
        [
        Description("Occurs when the user begins dragging an item."),
        Category("Drag Drop")
        ]
        public event ItemDragEventHandler ItemDrag;

        /// <summary>
        /// Occurs when <see cref="NodeStateImageList"/> is changed.
        /// </summary>
        [
        Description("Occurs when NodeStateImageList is changed."),
        Category("Property Changed")
        ]
        public event EventHandler NodeStateImageListChanged;

        /// <summary>
        /// Occurs when <see cref="DefaultCollapseImageIndex"/> is changed.
        /// </summary>
        [
        Description("Occurs when DefaultCollapseImageIndex is changed."),
        Category("Property Changed")
        ]
        public event EventHandler DefaultCollapseImageIndexChanged;

        /// <summary>
        /// Occurs when <see cref="DefaultExpandImageIndex"/> is changed.
        /// </summary>
        [
        Description("Occurs when DefaultExpandImageIndex is changed."),
        Category("Property Changed")
        ]
        public event EventHandler DefaultExpandImageIndexChanged;

        /// <summary>
        /// Occurs when the tree's BorderStyle is changed
        /// </summary>
        [
        Description("Occurs when the tree's BorderStyle is changed"),
        Category("Property Changed")
        ]
        public event EventHandler BorderStyleChanged;

        /// <summary>
        /// Occurs when the tree's Border3DStyle is changed
        /// </summary>
        [
        Description("Occurs when the tree's Border3DStyle is changed"),
        Category("Property Changed")
        ]
        public event EventHandler Border3DStyleChanged;

        /// <summary>
        /// Occurs when the tree's Border2DStyle is changed
        /// </summary>
        [
        Description("Occurs when the tree's Border2DStyle is changed"),
        Category("Property Changed")
        ]
        public event EventHandler BorderSingleChanged;

        /// <summary>
        /// Occurs when the tree's BorderColor is changed
        /// </summary>
        [
        Description("Occurs when the tree's BorderColor is changed"),
        Category("Property Changed")
        ]
        public event EventHandler BorderColorChanged;

        /// <summary>
        /// Occurs when the tree's GradientBackground is changed
        /// </summary>
        [
        Description("Occurs when the tree's BackgroundColorChanged is changed"),
        Category("Property Changed")
        ]
        public event EventHandler BackgroundColorChanged;

        /// <summary>
        /// Occurs when the tree's BorderSides is changed
        /// </summary>
        [
        Description("Occurs when the tree's BorderSides is changed"),
        Category("Property Changed")
        ]
        public event EventHandler BorderSidesChanged;

        /// <summary>
        /// Occurs when the ThemesEnabled property changes
        /// </summary>
        [
        Description("Occurs when the tree's BorderSides is changed"),
        Category("Occurs when the ThemesEnabled property changes")
        ]
        public event EventHandler ThemeChanged;

        /// <summary>
        /// Occurs when the tree's selected column changed.
        /// </summary>
        [
        Description("Occurs when the tree's selected column is changed."),
        Category("Action")
        ]
        public event TreeColumnChangedEventHandler ColumnHighlighted;

        /// <summary>
        /// Occurs when the tree's column is clicked.
        /// </summary>
        [
        Description("Occurs when the tree's column is clicked."),
        Category("Action")
        ]
        public event TreeColumnChangedEventHandler ColumnClick;

        /// <summary>
        /// Occurs when the tree's column is double clicked.
        /// </summary>
        [
        Description("Occurs when the tree's column is double clicked."),
        Category("Action")
        ]
        public event TreeColumnChangedEventHandler ColumnDoubleClick;

        /// <summary>
        /// Occurs when the tree's column is resized.
        /// </summary>
        [
        Description("Occurs when the tree's column is resized."),
        Category("Action")
        ]
        public event TreeColumnResizedEventHandler ColumnResized;

        /// <summary>
        /// Occurs when resizing the tree's column.
        /// </summary>
        [
        Description("Occurs when resizing the tree's column."),
        Category("Action")
        ]
        public event TreeColumnResizeEventHandler ColumnResizing;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Creates a new MultiColumnTreeView control.
        /// </summary>
        public MultiColumnTreeView()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new LicensedComponent(typeof(MultiColumnTreeView));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            this.AutoAdjustMultiLineHeight = true;
            this.SelectOnCollapse = true;
            this.EnsureVisibleFlag = true;
            this.AllowKeyboardSearch = true;
            this.HideSelection = true;
            this.ThemedBorder = true;
            this.EnsureVisibleSelectedNode = true;
            this.DragOnText = true;
            this.KeepDottedSelection = true;
            this.ShowDragNodeCue = true;
            this.ShowColumnsHeader = true;
            this.ShowLines = true;
            this.ShowRootLines = true;
            this.ShouldSelectNodeOnEnter = true;
            this.CanGetDragImage = true;
            this.AutoControlsAdding = true;

            m_bgBrush = BrushInfo.Empty;
            m_stylePairs = new StyleNamePairsList(this);

            InitializeStyles();
            InitializeComponent();

            this.BackColor = SystemColors.Window;
            this.BackColorChanged += new EventHandler(this.GradientPanel_BackColorChanged);

            // Initialize the Root node.
            this.Root = new TreeNodeAdv("root");

            // Initialize Columns 
            m_columns = new TreeColumnAdvCollection(this);
            m_columns.CollectionChanged += new CollectionChangeEventHandler(columns_CollectionChanged);

            m_penLine = new Pen(Color.Gray);
            m_penLine.DashStyle = DashStyle.Dot;

            m_checkedNodes = new CheckedNodesColection();
            m_selectedNodes = new SelectedNodesCollection();
            m_selectedNodes.CollectionChanged += new CollectionChangeEventHandler(this.selectedNodes_Changed);

            VScrollBar.SupportsThumbTrack = true;
            HScrollBar.SupportsThumbTrack = true;

            this.HScrollBar.SmallChange = 10;
            this.AllowIncreaseSmallChange = false;

            this.InsideScrollMargins = new Size(0, 10);

            m_ptPrintPosition = Point.Empty;

            m_pdDocument = new PrintDocument();
            m_pdDocument.BeginPrint += new PrintEventHandler(OnPrintDocumentBeginPrint);
            m_pdDocument.PrintPage += new PrintPageEventHandler(OnPrintDocumentPrintPage);
            CTRLSIZE = this.Size;
            HDRHeight = this.HeaderHeight;
            ITMHEIGHT = this.ItemHeight;
        }

        /// <summary>in destructor don't forget to clean resources.</summary>
        ~MultiColumnTreeView()
        {
            Dispose(this.IsDisposed);
        }

        /// <summary>Initialize control styles. Crete list of standard styles.</summary>
        protected virtual void InitializeStyles()
        {
            // Create the default base style:
            m_boundStyle = new TreeNodeAdvStyleInfo(new TreeBoundStyleInfoStore(this));

            // create default style for nodes, columns and subitems
            m_baseStyles[MultiColumnTreeView.DefaultBaseStyleName] = new TreeNodeAdvStyleInfo(new TreeViewAdvStyleInfoIdentity(this));
            m_baseStyles[MultiColumnTreeView.DefaultColumnStyleName] = new TreeColumnAdvStyleInfo(new TreeColumnAdvStyleInfoIdentity(this));
            m_baseStyles[MultiColumnTreeView.DefaultSubItemStyleName] = new TreeNodeAdvSubItemStyleInfo(new TreeNodeAdvSubItemStyleInfoIdentity(this));
        }

        /// <summary></summary>
        void ISupportInitialize.BeginInit()
        {
            // Need this for initial vertical scroller setup
            RefreshVScrollbar(null, 0);
        }

        /// <summary></summary>
        void ISupportInitialize.EndInit()
        {
            m_root.RecalculateAllDimensions();
            m_root.Expand();

            VerticallScrollBar = true;
            HorizontalScrollBar = true;

            // Another option is to call BeginUpdate and EndUpdate in BeginInit and EndInit respectively.
            RefreshVScrollbar();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing"/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeHelper.Dispose(ref m_dragHelper);
                DisposeHelper.Dispose(ref m_measure);
                DisposeHelper.Dispose(ref m_penLine);

                // dispose timer
                this.LabelEditStartTimer = null;

                // For defect 2792: Memory leak due to ToolTips.
                DisposeHelper.Dispose(ref m_helpText);
                DisposeHelper.Dispose(ref m_toolTip);

                if (m_printDocument != null)
                {
                    m_printDocument.PrintPage -= new PrintPageEventHandler(OnPrint);
                    m_printDocument = null;
                }

                if (m_selectedNodes != null)
                {
                    m_selectedNodes.CollectionChanged -= new CollectionChangeEventHandler(selectedNodes_Changed);
                    m_selectedNodes = null;
                }

                if (m_checkedNodes != null)
                {
                    m_checkedNodes.Clear();
                    m_checkedNodes = null;
                }

                if (m_labelEditor != null)
                {
                    m_labelEditor.TextChanged -= new EventHandler(this.labelEditor_TextChanged);
                    m_labelEditor.MouseUp -= new MouseEventHandler(this.labelEditor_MouseUp);
                    m_labelEditor.KeyDown -= new KeyEventHandler(this.labelEditor_KeyDown);
                    m_labelEditor.Leave -= new EventHandler(this.labelEditor_Leave);
                    m_labelEditor.LostFocus -= new EventHandler(this.labelEditor_LostFocus);

                    // and now dispose control
                    m_labelEditor.Dispose();
                    m_labelEditor = null;
                }

                // NOTE: do not dispose image lists we do not create this resources
                // that is why it's not our problem of there disposing. Just null references
                // is more then sufficient. Release references on user specified resources.
                m_stateImageList = null;
                m_rightImageList = null;
                m_leftImageList = null;
                m_nodeStateImageList = null;
                m_drawingRgn.Dispose();

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            m_keyInputTimer = new Timer(this.components);
            m_labelEditor = new TextBox();
            m_toolTip = new ToolTipAdv(this);
            m_helpText = new ToolTipAdv(this);
            this.SuspendLayout();
            // 
            // keyInputTimer
            // 
            m_keyInputTimer.Interval = 500;
            m_keyInputTimer.Tick += new EventHandler(this.keyInputTimer_Tick);
            // 
            // labelEditor
            // 
            m_labelEditor.AcceptsReturn = true;
            m_labelEditor.AcceptsTab = true;
            m_labelEditor.BorderStyle = BorderStyle.FixedSingle;
            m_labelEditor.Location = new Point(530, 0);
            m_labelEditor.Name = "m_labelEditor";
            m_labelEditor.TabIndex = 0;
            m_labelEditor.Text = "";
            m_labelEditor.Visible = false;
            m_labelEditor.TextChanged += new EventHandler(this.labelEditor_TextChanged);
            m_labelEditor.KeyUp += new KeyEventHandler(this.labelEditor_KeyUp);
            m_labelEditor.KeyDown += new KeyEventHandler(this.labelEditor_KeyDown);
            m_labelEditor.MouseUp += new MouseEventHandler(labelEditor_MouseUp);
            m_labelEditor.Leave += new EventHandler(this.labelEditor_Leave);
            m_labelEditor.LostFocus += new EventHandler(labelEditor_LostFocus);
            // 
            // toolTip
            // 
            m_toolTip.BackColor = SystemColors.Info;
            m_toolTip.BorderStyle = BorderStyle.FixedSingle;
            m_toolTip.Name = "m_toolTip";
            m_toolTip.TabIndex = 1;
            m_toolTip.Text = "toolTip";
            m_toolTip.Visible = false;
            // 
            // helpText
            // 
            m_helpText.BorderStyle = BorderStyle.FixedSingle;
            m_helpText.Name = "m_helpText";
            m_helpText.TabIndex = 0;
            m_helpText.Text = "help text";
            m_helpText.Visible = false;
            // 
            // MultiColumnTreeView
            // 
            this.Controls.AddRange(new Control[]
        {
          m_labelEditor
        });
            this.Size = new Size(121, 97);
            this.ResumeLayout(false);
        }
        #endregion

        #region Class codedom serialization

        protected bool ShouldSerializeColumns()
        {
            return this.HasColumns;
        }

        /// <summary></summary>
        protected void ResetBorderColor()
        {
            this.BorderColor = Color.Black;
        }

        protected bool ShouldSerializeBorderColor()
        {
            if (m_clrBorder == Color.Black)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary></summary>
        protected void ResetBackgroundColor()
        {
            this.BackgroundColor = BrushInfo.Empty;
        }

        protected bool ShouldSerializeBackgroundColor()
        {
            return (m_bgBrush != BrushInfo.Empty);
        }

        /// <summary></summary>
        protected void ResetColumnsHeaderBackground()
        {
            m_HeaderBackground = BrushInfo.Empty;
        }

        protected bool ShouldSerializeColumnsHeaderBackground()
        {
            return (m_HeaderBackground != BrushInfo.Empty);
        }

        /// <summary>
        /// Determines if the BackColor property was modified.
        /// </summary>
        /// <returns>Returns bool property</returns>
        protected bool ShouldSerializeBackColor()
        {
            return (this.BackColor != SystemColors.Window);
        }

        /// <override/>
        /// <summary>
        /// Resets the <see cref="BackColor"/> property.
        /// </summary>
        public override void ResetBackColor()
        {
            this.BackColor = SystemColors.Window;
        }

        /// <summary></summary>
        protected void ResetThemesEnabled()
        {
            this.StandardStyle.ResetThemesEnabled();
        }

        protected bool ShouldSerializeThemesEnabled()
        {
            return this.StandardStyle.ShouldSerializeThemesEnabled();
        }

        /// <summary>
        /// Resets the <see cref="OpenImgIndex"/> property.
        /// </summary>
        public void ResetOpenImgIndex()
        {
            this.StandardStyle.ResetOpenImgIndex();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeOpenImgIndex()
        {
            return this.StandardStyle.ShouldSerializeOpenImgIndex();
        }

        /// <summary>
        /// Resets the <see cref="ShowOptionButtons"/> property.
        /// </summary>
        public void ResetShowOptionButtons()
        {
            this.StandardStyle.ResetShowOptionButton();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeShowOptionButtons()
        {
            return this.StandardStyle.ShouldSerializeShowOptionButton();
        }

        /// <summary>
        /// Resets the <see cref="ShowPlusMinus"/> property.
        /// </summary>
        public void ResetShowPlusMinus()
        {
            this.StandardStyle.ResetShowPlusMinus();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeShowPlusMinus()
        {
            return this.StandardStyle.ShouldSerializeShowPlusMinus();
        }

        /// <summary>
        /// Resets the <see cref="ShowCheckBoxes"/> property.
        /// </summary>
        public void ResetShowCheckBoxes()
        {
            this.StandardStyle.ResetShowCheckBox();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeShowCheckBoxes()
        {
            return this.StandardStyle.ShouldSerializeShowCheckBox();
        }

        /// <summary>
        /// Resets the <see cref="InteractiveCheckBoxes"/> property.
        /// </summary>
        public void ResetInteractiveCheckBoxes()
        {
            this.StandardStyle.ResetInteractiveCheckBox();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeInteractiveCheckBoxes()
        {
            return this.StandardStyle.ShouldSerializeInteractiveCheckBox();
        }

        /// <summary>
        /// Resets the <see cref="ClosedImgIndex"/> property.
        /// </summary>
        public void ResetClosedImgIndex()
        {
            this.StandardStyle.ResetClosedImgIndex();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeClosedImgIndex()
        {
            return this.StandardStyle.ShouldSerializeClosedImgIndex();
        }

        /// <summary>
        /// Resets the <see cref="NoChildrenImgIndex"/> property.
        /// </summary>
        public void ResetNoChildrenImgIndex()
        {
            this.StandardStyle.ResetNoChildrenImgIndex();
            this.Root.RecalculateAllDimensions();
            Invalidate();
        }

        protected bool ShouldSerializeNoChildrenImgIndex()
        {
            return this.StandardStyle.ShouldSerializeNoChildrenImgIndex();
        }

        [DocumentationExclude()]
        protected bool ShouldSerializeInactiveSelectedNodeBackground()
        {
            return !m_inactiveSelectedNodeBackground.Equals(new BrushInfo(SystemColors.Control));
        }

        /// <summary></summary>
        [DocumentationExclude()]
        protected void ResetInactiveSelectedNodeBackground()
        {
            this.InactiveSelectedNodeBackground = new BrushInfo(SystemColors.Control);
        }

        protected bool ShouldSerializeLineColor()
        {
            return m_clrLine != Color.Gray;
        }

        /// <summary></summary>
        protected void ResetLineColor()
        {
            LineColor = Color.Gray;
        }

        /// <summary>
        /// Resets the <see cref="ItemHeight"/> property.
        /// </summary>
        public void ResetItemHeight()
        {
            m_itemHeight = -1;
            this.Root.RecalculateAllDimensions();
        }

        protected bool ShouldSerializeItemHeight()
        {
            return m_itemHeight == -1 ? false : true;
        }

        [DocumentationExclude()]
        protected bool ShouldSerializeSelectedNodeBackground()
        {
            return !m_selectedNodeBackground.Equals(new BrushInfo(SystemColors.Highlight));
        }

        /// <summary></summary>
        [DocumentationExclude()]
        protected void ResetSelectedNodeBackground()
        {
            this.SelectedNodeBackground = new BrushInfo(SystemColors.Highlight);
        }

        [DocumentationExclude()]
        protected bool ShouldSerializeInactiveSelectedNodeForeColor()
        {
            return m_clrInactiveSelectedNodeFore != SystemColors.ControlText;
        }

        /// <summary></summary>
        [DocumentationExclude()]
        protected void ResetInactiveSelectedNodeForeColor()
        {
            InactiveSelectedNodeForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Recreate CustomControlCollection.
        /// </summary>
        private void RefreshCustomControlCollection()
        {
            foreach (DictionaryEntry element in this.CustomControlCollection)
            {
                Control control = (Control)element.Key;
                TreeNodeAdv node = (TreeNodeAdv)element.Value;
                if (this.Controls.Contains(control))
                {
                    this.Controls.Remove(control);
                    node.UnSubscribeControlEvents(control);
                }
            }

            this.CustomControlCollection.Clear();

            if (this.Root != null)
            {
                this.Root.CustomControlCollectionChanging(this.Root, CollectionChangeAction.Add);
            }
        }

        [DocumentationExclude()]
        protected bool ShouldSerializeSelectedNodeForeColor()
        {
            return m_clrSelectedNodeFore != SystemColors.HighlightText;
        }

        [DocumentationExclude()]
        protected void ResetSelectedNodeForeColor()
        {
            SelectedNodeForeColor = SystemColors.HighlightText;
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        /// Implement this interface to support keyboard based (Shift+F10) context menu 
        /// invocation. The context menu will then appear near the selected node.
        /// </summary>
        /// <remarks>The PopupMenu in the XPMenus framework will then call this method to
        /// determine the location for popup.</remarks>
        /// <returns></returns>
        Point IProvideCustomContextMenuPositionalInformation.GetMenuPositionForKeyboardInvoke()
        {
            if (this.SelectedNode != null)
            {
                Rectangle selBounds = this.SelectedNode.TextBounds;

                return new Point(selBounds.X, selBounds.Y + selBounds.Height / 2);
            }

            return Point.Empty;
        }

        /// <summary>
        /// Calls the <see cref="OnItemDrag"/> to raise the <see cref="ItemDrag"/> event.
        /// </summary>
        /// <param name="e">An ItemDragEventArgs that contains the event data.</param>
        public void RaiseItemDrag(ItemDragEventArgs e)
        {
            if (!(e.Item is TreeNodeAdv[]))
            {
                throw new Exception("The RaiseItemDrag method is called with invalid argument type.");
            }

            m_draggedTnas = e.Item as TreeNodeAdv[];
            OnItemDrag(e);

            if (!this.AllowDropStubWorks)
            {
                m_draggedTnas = null;
            }
        }

        /// <summary>
        /// Overloaded. Begins the editing of the specified node.
        /// </summary>
        /// <param name="node">The node to edit.</param>
        public void BeginEdit(TreeNodeAdv node)
        {
            if (node != null && node.TreeView == this)
            {
                //this.SelectedNode = node;
                if (SetSelectedNode(node, m_selectedNodes, TreeViewAdvAction.Unknown))
                {
                    this.ActiveNode = node;
                    m_selectionBaseNode = node;
                }

                if (this.ActiveNode == node)
                {
                    BeginEdit();
                }
            }
        }

        /// <summary>
        /// Begins the editing of the selected node.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (this.ActiveNode == null)
            {
                return;
            }

            // In case the node was just added then a paint message will set it's bounds.
            this.Update();

            SetSelectedNode(this.ActiveNode, m_selectedNodes, TreeViewAdvAction.Unknown);

            if (this.SelectedNode != this.ActiveNode)
            {
                return;
            }

            // Initiates the LabelEditor for every node's before edit event.
            m_labelEditor.Text = string.Empty;

            TreeNodeAdvBeforeEditEventArgs args = new TreeNodeAdvBeforeEditEventArgs(m_activeNode, m_labelEditor);
            OnBeforeEdit(args);

            // Checks whether the text box value has been changed or not. If it's changed, 
            // the changed values should be displayed in Label edit text box.
            m_labelEditor.Text = (args.TextBox.Text != null && args.TextBox.Text.Length > 0) ?
        args.TextBox.Text : args.Node.Text;

            if (args.Cancel)
            {
                return;
            }

            Rectangle editorBounds = m_activeNode.TextBounds;
            editorBounds.Width = Math.Max(50, editorBounds.Width);

            m_labelEditor.Bounds = editorBounds;

            PrepareEditor();

            this.NeedUpdateEditTop = true;
            ShowEditor();
        }

        /// <summary>
        /// Saves or Cancels the editing of the selected node.
        /// </summary>
        /// <param name="cancel">True to cancel editing; false to save changes.</param>
        public virtual void EndEdit(bool cancel)
        {
            if (!this.IsEditing || this.IsEditEnding)
            {
                return;
            }

            bool continueEditing = false;

            if (!cancel)
            {
                TreeNodeAdvCancelableEditEventArgs args = new TreeNodeAdvCancelableEditEventArgs(m_activeNode, m_labelEditor.Text);
                OnNodeEditorValidating(args);
                cancel = args.Cancel;

                if (cancel)
                {
                    continueEditing = args.ContinueEditing;
                }
            }

            if (cancel)
            {
                m_activeNode.Text = (string)m_labelEditor.Tag;
            }
            else
            {
                string newText = m_labelEditor.Text;

                if (m_historyManager != null && this.HistoryEnabled && newText != m_activeNode.Text)
                {
                    TreeViewCommand cmd = new TreeViewCommand(m_activeNode, newText);
                    m_historyManager.Do(cmd);
                }

                m_activeNode.Text = newText;
                m_activeNode.RecalculateAllDimensions();

            }

            if (!continueEditing)
            {
                HideEditor();
                Focus();
            }
            else
            {
                PrepareEditor();
                ShowEditor();
            }

            if (!cancel)
            {
                OnNodeEditorValidated(new TreeNodeAdvEditEventArgs(m_activeNode, m_activeNode.Text));
            }
            else if (this.CancelEdit)
            {
                OnEditCancelled(new TreeNodeAdvEditEventArgs(m_activeNode, m_labelEditor.Text));
            }
        }

        /// <summary>
        /// Forces the end of the editing of the selected node.
        /// </summary>
        public void EndEdit()
        {
            EndEdit(false);
        }

        /// <summary>
        /// Returns a <see cref="System.Drawing.Bitmap"/> that contains the image of the dragged nodes
        /// with it's state image.
        /// </summary>
        /// <returns>A <see cref="System.Drawing.Bitmap"/> instance when there is atleast
        /// one selected node; Null otherwise.</returns>
        public Bitmap GetDraggedNodesBitmap()
        {
            if (m_draggedTnas == null || m_draggedTnas.Length == 0)
            {
                return null;
            }

            Rectangle clipRect = this.TreeColumnRectangle;

            // If HorizontalScroll is on, adjust the X and width to reflect the whole tree rect.
            if (HScrollBar.Enabled)
            {
                clipRect.X = -this.HScrollBar.Value;
                clipRect.Width = this.TreeColumnRectangle.Width + this.HScrollBar.Maximum;
            }

            Bitmap bmp = new Bitmap(clipRect.Width, clipRect.Height);
            if (!this.UseDefaultDrawing)
            {
                Graphics g = Graphics.FromImage(bmp);

                if (clipRect.X < 0)
                {
                    // Translate b'cos the bmp's coords start from 0 whereas
                    // the clip-rect's X co-ord is negative b'cos of scrolling.
                    g.TranslateTransform((float)-clipRect.X, 0, MatrixOrder.Append);
                }

                PaintEventArgs pea = new PaintEventArgs(g, clipRect);

                // Prevent drawing the selection rect while preparing the bitmap
                this.PreparingDragCueBitmap = true;

                // Also apply the DragNodeCueStyle while preparing the bitmap.
                string oldBaseStyle = String.Empty;
                if (this.BaseStyles.Contains("DragNodeCueStyle"))
                {
                    oldBaseStyle = this.StandardStyle.BaseStyle;
                    this.StandardStyle.BaseStyle = "DragNodeCueStyle";

                }
                // Don't call this since OnPaintBackground includes logic for drawing child backgrounds.
                // this.InvokePaintBackground(this, pea);
                g.FillRectangle(SystemBrushes.Window, clipRect);

                OnPaint(pea);

                if (this.BaseStyles.Contains("DragNodeCueStyle"))
                {
                    this.StandardStyle.BaseStyle = oldBaseStyle;
                }

                this.PreparingDragCueBitmap = false;

                g.Dispose();
            }

            Bitmap selBmp = this.ExtractSelectedBoundsFromBmp(bmp, m_draggedTnas);

            bmp.Dispose();

            return selBmp;
        }

        /// <summary>
        /// Begins the printing process of the MultiColumnTreeView.
        /// </summary>
        public void Print()
        {
            this.PrintDocument.Print();
        }

        /// <summary>
        /// Returns the tree node at the specified point in client co-ordinates.
        /// </summary>
        /// <param name="pt">The point in client co-ordinates.</param>
        /// <returns>A <see cref="TreeNodeAdv"/>.</returns>
        public TreeNodeAdv PointToNode(Point pt)
        {
            int y = 0;
            int rowIndex = this.VScrollPos;

            TreeNodeAdv node = RowIndexToNode(rowIndex);

            while (node != null)
            {
                if (node.Visible)
                    y += node.Height;
                if (y > pt.Y && (node.Visible || this.DesignModeInternal))
                {
                    return node;
                }

                node = node.NextVisibleNode;
            }

            return null;
        }

        /// <summary>
        /// Returns the location of the tree node in client co-ordinates.
        /// </summary>
        /// <param name="node">The <see cref="TreeNodeAdv"/> whose location you need.</param>
        /// <returns>A <see cref="System.Drawing.Point"/>.</returns>
        /// <remarks>Operation has linear computation complexity algorithm that is why use 
        /// it carefully on large tree hierarchies.</remarks>
        public Point NodeToPoint(TreeNodeAdv node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node == m_root)
            {
                if (!this.VerticallScrollBar || !m_root.HasNodes)
                {
                    return new Point(0, -m_root.Height / 2);
                }
                else if (m_root.HasNodes)
                {
                    return new Point(0, NodeToPoint(m_root.FirstNode).Y - m_root.Height / 2);
                }
            }

            int y = 0;
            int rowIndex = node.TreeRowIndex;
            int direction = (rowIndex < this.VScrollPos) ? -1 : 1;
            int n = this.VScrollPos;
            node = RowIndexToNode(n);

            while (n != rowIndex)
            {
                if (node == null)
                {
                    break;
                }

                y += (direction * node.Height);

                node = (direction > 0) ? node.NextVisibleNode : node.PrevVisibleNode;
                if (node != null)
                {
                    n = node.TreeRowIndex;
                }
            }

            return new Point(0, y);
        }

        /// <summary>
        /// Returns the total height of the rows from the specified start to end.
        /// </summary>
        /// <param name="start">The top row.</param>
        /// <param name="end">The bottom row.</param>
        /// <returns>The total height.</returns>
        public int GetHeightOfRows(int start, int end)
        {
            int y = 0;

            TreeNodeAdv node = RowIndexToNode(start);
            if (!node.Visible && !this.DesignModeInternal)
            {
                return 0;
            }

            for (int n = start; n <= end; n++)
            {
                if (node == null)
                {
                    break;
                }
                y += node.Height;

                node = node.NextVisibleNode;
            }

            return y;
        }

        /// <summary>
        /// Returns the rectangular area in which the tree node will be drawn.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/>.</param>
        /// <returns>A <see cref="System.Drawing.Rectangle"/>.</returns>
        public Rectangle NodeToRectangle(TreeNodeAdv node)
        {
            Point pt = NodeToPoint(node);
            return new Rectangle(pt, new Size(TreeColumnRectangle.Width, node.Height));
        }

        /// <summary>
        /// Returns the tree node at the specified row index.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>A <see cref="TreeNodeAdv"/>.</returns>
        public TreeNodeAdv RowIndexToNode(int rowIndex)
        {
            return Root.GetNodeAtAbsoluteRowIndex(rowIndex);
        }

        /// <summary>
        /// Returns the row index of a tree node.
        /// </summary>
        /// <param name="node">A <see cref="TreeNodeAdv"/>.</param>
        /// <returns>The row index.</returns>
        public int NodeToRowIndex(TreeNodeAdv node)
        {
            return node.TreeRowIndex;
        }

        /// <summary>
        /// Cancels any current mouse based selection and edit mode.
        /// </summary>
        public new void CancelMode()
        {
            CancelMouseBasedSelection();

            this.CancelEdit = true;
            EndEdit(true);
            this.CancelEdit = false;
        }

        /// <summary>
        /// Returns a node from the specified path. Make sure that the path does not end with a separator when calling this.
        /// </summary>
        /// <param name="path">The path of the node.</param>
        /// <returns>The node that has the specified path.</returns>
        public TreeNodeAdv GetNodeFromPath(string path)
        {
            return GetNode(m_root, path);
        }

        /// <summary>
        /// Returns the path of the specified node.
        /// </summary>
        /// <param name="node">Node whose path is to be returned.</param>
        /// <returns>The path of the node.</returns>
        public string GetPathFromNode(TreeNodeAdv node)
        {
            if (node == null)
            {
                return string.Empty;
            }

            return node.GetPath(m_strPathSeparator);
        }

        /// <summary>
        /// Overloaded. Extends the selection to the specified node.
        /// </summary>
        /// <param name="selNode">A TreeNodeAdv.</param>
        /// <remarks>This method will not do anything if the <see cref="SelectionMode"/>
        /// property is set to <b>TreeSelectionMode.Single</b>.</remarks>
        public void ExtendSelectionTo(TreeNodeAdv selNode)
        {
            ExtendSelectionTo(selNode, true);
        }

        /// <summary>
        /// Extends the selection to the specified node.
        /// </summary>
        /// <param name="pSelectedNode">A TreeNodeAdv.</param>
        /// <param name="removeCurrentMultipleSelection">Indicates whether or not any current selection should be removed.</param>
        /// <remarks>This method will not do anything if the <see cref="SelectionMode"/>
        /// property is set to <b>TreeSelectionMode.Single</b>.</remarks>
        public void ExtendSelectionTo(TreeNodeAdv pSelectedNode, bool removeCurrentMultipleSelection)
        {
            if (this.SelectionMode == TreeSelectionMode.Single)
            {
                return;
            }

            if (m_selectionBaseNode != null && pSelectedNode != null)
            {
                bool bMultiSelectAll = this.SelectionMode == TreeSelectionMode.MultiSelectAll;

                if (bMultiSelectAll || (pSelectedNode.Parent == m_selectionBaseNode.Parent))
                {
                    ArrayList lstNewNodes = NewlySelectedNodes(pSelectedNode, bMultiSelectAll);
                    ArrayList lstNodesToRemove = null;

                    if (removeCurrentMultipleSelection)
                    {
                        lstNodesToRemove = this.MouseBasedSelectionOn ? m_latestMouseBasedSelection : this.SelectedNodes;
                    }
                    // Cache the latest set of selected nodes if this is mouse-based selection.
                    if (this.MouseBasedSelectionOn)
                    {
                        m_latestMouseBasedSelection = new ArrayList(lstNewNodes);
                    }

                    if (SetSelectedNode(lstNewNodes, lstNodesToRemove, TreeViewAdvAction.ByMouse))
                    {
                        this.ActiveNode = pSelectedNode;
                    }
                }
            }
        }

        /// <summary>
        /// Method fills an array with newly selected nodes.
        /// </summary>
        /// <param name="pSelectedNode"> Currently selected node. </param>
        /// <param name="pMultiSelectAll"> Indicates whether all nodes must be selected or only nodes on same level. </param>
        /// <returns> List of selected nodes. </returns>
        private ArrayList NewlySelectedNodes(TreeNodeAdv pSelectedNode, bool pMultiSelectAll)
        {
            ArrayList lstNewNodes = new ArrayList();

            //If we deselect a node by Up/Down arrow key the previously selected node should be removed from selected nodes list.
            if (m_selectedNodes.Contains(pSelectedNode) && (m_pressedKey == Keys.Up || m_pressedKey == Keys.Down))
            {
                m_selectedNodes.Remove(this.SelectUpwardDirection ? pSelectedNode.NextVisibleNode : pSelectedNode.PrevVisibleNode);

                EnsureVisible(pSelectedNode);
                lstNewNodes = m_selectedNodes;
                m_pressedKey = Keys.None;
            }
            else
            {
                // If nodes has been selected by mouse.
                if (m_pressedKey == Keys.None)
                {
                    //this.SelectUpwardDirection = NodeToRowIndex(m_selectionBaseNode) >= NodeToRowIndex(pSelectedNode);
                    this.SelectUpwardDirection = (m_selectionBaseNode.Bounds.Y >= pSelectedNode.Bounds.Y);
                }

                TreeNodeAdv node = m_selectionBaseNode;
                bool bSelectUpwardDirection = this.SelectUpwardDirection;

                while (node != null && node != pSelectedNode)
                {
                    if (node.Enabled && (pMultiSelectAll || node.Parent == pSelectedNode.Parent))
                    {
                        lstNewNodes.Add(node);
                    }

                    node = (bSelectUpwardDirection ? node.PrevVisibleNode : node.NextVisibleNode);
                }

                lstNewNodes.Add(pSelectedNode);
            }

            return lstNewNodes;
        }

        /// <summary>
        /// Begins a drag-and-drop operation.
        /// Added by lucas in order to resolve problem 169.
        /// </summary>
        /// <param name="data">The data to drag.</param>
        /// <param name="allowedEffects">One of the DragDropEffects values.</param>
        /// <returns>A value from the DragDropEffects enumeration that
        ///  represents the final effect that was performed
        ///  during the drag-and-drop operation.</returns>
        public new DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            this.CanDrag = false;
            DragDropEffects result = DragDropEffects.None;
            result = base.DoDragDrop(data, allowedEffects);

            if (!AllowDrop)
            {
                // This snippet of code was added by Lucas i order to fix
                // issue # 169 
                this.AllowDropStubWorks = true;
                result = DragDropEffects.None;
            }

            return result;
        }

        [DocumentationExclude()]
        public void DispatchDragOver(DragEventArgs drgevent)
        {
            OnDragOver(drgevent);
        }


        [DocumentationExclude()]
        public void DispatchOnDragDrop(DragEventArgs drgevent)
        {
            OnDragDrop(drgevent);
        }

        [DocumentationExclude()]
        public void DispatchOnDragEnter(DragEventArgs drgevent)
        {
            OnDragEnter(drgevent);
        }

        [DocumentationExclude()]
        public void DispatchOnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }

        [DocumentationExclude()]
        public void DispatchOnQueryContinueDrag(QueryContinueDragEventArgs args)
        {
            OnQueryContinueDrag(args);
        }

        [DocumentationExclude()]
        public void DispatchOnGiveFeedback(GiveFeedbackEventArgs args)
        {
            OnGiveFeedback(args);
        }

        [DocumentationExclude()]
        public DragDropEffects DispatchDoDragDrop(object data, DragDropEffects allowedEffects)
        {
            return DoDragDrop(data, allowedEffects);
        }

        /// <summary>
        /// Returns the width required to draw the text specified using the font specified.
        /// </summary>
        /// <param name="graphics">A <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="text">The text that is to be drawn.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> using which to draw.</param>
        /// <returns>Width required.</returns>
        public Size MeasureDisplayStringSize(Graphics graphics, string text, Font font)
        {
            return ControlDrawing.MeasureDisplayStringSize(graphics, text, font, GetIsMirrored());
        }

        /// <summary>
        /// Returns the width required to draw the text specified using the font specified.
        /// </summary>
        /// <param name="graphics">A <see cref="System.Drawing.Graphics"/> object.</param>
        /// <param name="text">The text that is to be drawn.</param>
        /// <param name="font">The <see cref="System.Drawing.Font"/> using which to draw.</param>
        /// <param name="width"></param>
        /// <returns>Size required for text.</returns>
        public Size MeasureDisplayStringSize(Graphics graphics, string text, Font font, int width)
        {
            return ControlDrawing.MeasureDisplayStringSize(graphics, text, font, GetIsMirrored(), width);
        }

        /// <summary>
        /// Collapses all the tree nodes.
        /// </summary>
        /// <remarks><p>The CollapseAll method collapses all the <see cref="TreeNodeAdv"/> 
        /// objects, including all the child tree nodes, that are in the 
        /// <see cref="MultiColumnTreeView"/> control.</p><p>The state of a <b>TreeNodeAdv</b> persists. For example, suppose that 
        /// a parent tree node is expanded. If the child tree nodes were not 
        /// previously collapsed, they will appear in their previously-expanded 
        /// state. Calling the <b>CollapseAll</b> method ensures that all the tree nodes 
        /// appear in the collapsed state.</p></remarks>
        public void CollapseAll()
        {
            if (this.Root.HasNodes)
            {
                this.Root.CollapseAll();
            }
        }

        /// <summary>
        /// Expands all the tree nodes.
        /// </summary>
        /// <remarks><p>The <b>ExpandAll</b> method expands all the <see cref="TreeNodeAdv"/> 
        /// objects, including all the child tree nodes, that are in the <see cref="MultiColumnTreeView"/> 
        /// control.</p></remarks>
        public void ExpandAll()
        {
            if (this.Root.HasNodes)
            {
                this.Root.ExpandAll();
            }
        }

        /// <summary>
        /// Cancels the edit mode.
        /// </summary>
        /// <remarks><p> The <b>CancelEditMode</b> method cancels the edit mode when the node is in the EditingMode.</p></remarks>
        public void CancelEditMode()
        {
            this.CancelEdit = true;
            EndEdit(true);
            this.CancelEdit = false;
        }

        /// <summary>
        /// Retrieves the number of tree nodes, optionally including those in all 
        /// subtrees, assigned to the tree view control.
        /// </summary>
        /// <param name="includeSubTrees"><b>true</b> to count the <see cref="TreeNodeAdv"/> 
        /// items that the subtrees contain; false otherwise. </param>
        /// <returns>The number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.</returns>
        /// <remarks>
        /// If includeSubTrees is <b>true</b>, the result is the number of all the tree nodes in the entire tree structure.
        /// </remarks>
        /// <example>This example describes how to count all the nodes(including child nodes) of the treeViewAdv
        /// The user could get the total number of nodes by calling GetNodeCount method with the 
        /// bool argument which indicates whether count should include sub trees or not. If we
        /// pass it as true, it will count the nodes with the subtrees also.
        /// <code language="C#">
        /// private void button1_Click(object sender, System.EventArgs e) 
        /// { 
        ///  //Call the tree control's "GetNodeCount" method with true to 
        ///  //get the total number of nodes in the tree 
        /// int TotalNodesInTree = this.treeViewAdv1.GetNodeCount( true ); 
        /// MessageBox.Show( "Total nodes in tree = " + TotalNodesInTree.ToString()); 
        /// } 
        /// //Add nodes  
        /// private void button2_Click(object sender, System.EventArgs e) 
        /// { 
        /// this.treeViewAdv1.SelectedNode.Nodes.Add(new TreeNodeAdv()); 
        /// } 
        /// //Remove nodes 
        /// private void button3_Click(object sender, System.EventArgs e) 
        /// { 
        /// this.treeViewAdv1.SelectedNode.Parent.Nodes.Remove(this.treeViewAdv1.SelectedNode); 
        /// } 
        /// </code><code language="VB"> 
        /// Private Sub button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        /// ' Call the tree control's  "GetNodeCount" method with true to 
        /// ' get the total number of nodes in the tree 
        /// Dim TotalNodesInTree As Integer = Me.treeViewAdv1.GetNodeCount(True) 
        /// MessageBox.Show("Total nodes in tree = " &amp; TotalNodesInTree.ToString()) 
        /// End Sub 
        /// 'Add nodes 
        /// Private Sub button2_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        /// Me.treeViewAdv1.SelectedNode.Nodes.Add(New TreeNodeAdv()) 
        /// End Sub 
        /// 'Remove nodes 
        /// Private Sub button3_Click(ByVal sender As Object, ByVal e As System.EventArgs) 
        /// Me.treeViewAdv1.SelectedNode.Parent.Nodes.Remove(Me.treeViewAdv1.SelectedNode) 
        /// End Sub 
        /// </code></example>
        public int GetNodeCount(bool includeSubTrees)
        {
            return this.Root.GetNodeCount(includeSubTrees);
        }

        /// <summary>
        /// Overloaded. Scrolls the control so that the specified node becomes visible.
        /// </summary>
        /// <param name="node">The node that requires visibility.</param>
        public void EnsureVisible(TreeNodeAdv node)
        {
            RefreshVScrollbar();
            EnsureVisibleV(node, false);
            EnsureVisibleH(node);
        }

        /// <summary>
        /// Scrolls the control so that the specified node becomes visible and 
        /// optionally forces it to be the top-most visible node.
        /// </summary>
        /// <param name="node">The node that is to be scrolled</param>
        public void EnsureVisibleH(TreeNodeAdv node)
        {
            // if node is not full displayed than change scroll position
            if (null != Root && null != node)
            {
                int leftPoint = node.NodeX - node.Bounds.X - TreeAllColumnsRectangle.Width + node.Width;

                if (leftPoint > this.HScrollBar.Value)
                {
                    this.HScrollBar.Value = leftPoint;
                }
                else if (node.NodeX < 0)
                {
                    this.HScrollBar.Value = node.NodeX - node.Bounds.X;
                }
            }
        }

        /// <summary>
        /// Scrolls the control so that the specified node becomes visible and 
        /// optionally forces it to be the top-most visible node.
        /// </summary>
        /// <param name="node">The node that is to be scrolled.</param>
        /// <param name="showOnTop">True to force it to be the the top-most visible node; false to just scroll it into view.</param>
        public void EnsureVisibleV(TreeNodeAdv node, bool showOnTop)
        {
            if (!this.VerticallScrollBar || (!node.Visible && !this.DesignModeInternal))
            {
                return;
            }

            int rowIndex = NodeToRowIndex(node);
            TreeNodeAdv last = this.LastVisibleNode;

            int lastRowIndex;
            if (last != null)
            {
                lastRowIndex = NodeToRowIndex(last);
            }
            else
            {
                lastRowIndex = this.VScrollBar.Maximum;
            }

            if (showOnTop)
            {
                this.VScrollPos = rowIndex;
            }
            else
            {
                if (rowIndex >= this.VScrollPos && rowIndex <= lastRowIndex)
                {
                    return;
                }

                if (rowIndex < this.VScrollPos)
                {
                    this.VScrollPos = rowIndex;
                }

                if (rowIndex >= lastRowIndex)
                {
                    this.VScrollPos = GetEVVScrollPos(node);
                }
            }
        }

        /// <summary>
        /// Overloaded. Returns the node at the specified location.
        /// </summary>
        /// <param name="x">The X co-ordinate.</param>
        /// <param name="y">The Y co-ordinate.</param>
        /// <returns>The node at the point.</returns>
        public TreeNodeAdv GetNodeAtPoint(int x, int y)
        {
            return GetNodeAtPoint(new Point(x, y));
        }

        /// <summary>
        /// Returns the node at the specified location.
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <returns>The node at the point.</returns>
        public TreeNodeAdv GetNodeAtPoint(Point pt)
        {
            return GetNodeAtPoint(pt, false);
        }

        /// <summary>
        /// Returns the node at the specified location.
        /// </summary>
        /// <param name="pt">Location.</param>
        /// <param name="textBounds">Indicates whether the testing will be done using the bounds of the text, not the whole bounds of the node.</param>
        /// <returns>The node at the point.</returns>
        public TreeNodeAdv GetNodeAtPoint(Point pt, bool textBounds)
        {
            return GetNodeAtPoint(pt, textBounds, false);
        }

        /// <summary>
        /// Returns the node at the specified location.
        /// </summary>
        /// <param name="pt">Location.</param>
        /// <param name="pHitTextBounds">Indicates whether the testing will be done using the bounds of the text, not the whole bounds of the node.</param>
        /// <param name="pHitTextOrImageBounds">Indicates whether the testing will be done using the bounds of the images and text, 
        /// not the whole bounds of the node.</param>
        /// <returns>The node at the point.</returns>
        /// <remarks>If both the pHitTextBounds and pHitTextOrImageBounds params are false then the testing will be done on the 
        /// whole node.</remarks>
        public TreeNodeAdv GetNodeAtPoint(Point pt, bool pHitTextBounds, bool pHitTextOrImageBounds)
        {
            // Force any pending paint messages so that the bounds of nodes can be updated.
            //this.Update();

            TreeNodeAdv node = null;

            // if node doesn't visible than return null;
            if (!(this.HasColumns && !this.Columns[0].Visible))
            {
                node = PointToNode(pt);

                if (node != null)
                {
                    // Initialize helper variables.
                    int iRowIndex = NodeToRowIndex(node);
                    Rectangle rcText = node.TextBounds;
                    Rectangle rcTextAndImage = node.TextAndImageBounds;

                    // Intersect Text and TextAndImage bounds if tree has columns.
                    if (this.HasColumns)
                    {
                        Rectangle rcCol = new Rectangle(0 - HScrollPos, 0 - VScrollPos, Columns[0].Width, this.Height);
                        rcText.Intersect(rcCol);
                        rcTextAndImage.Intersect(rcCol);
                    }

                    // return null if Text or TextAndImage don't contain hit point.
                    if (iRowIndex >= this.VScrollPos && iRowIndex <= NodeToRowIndex(this.LastVisibleNode) &&
                        (pHitTextBounds || pHitTextOrImageBounds))
                    {
                        if (!(pHitTextBounds && rcText.Contains(pt) ||
                            pHitTextOrImageBounds && rcTextAndImage.Contains(pt)))
                        {
                            node = null;
                        }
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Gets the treeNodeAdvSubItem at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TreeNodeAdvSubItem GetSubItemAtPoint(int x, int y)
        {
            return GetSubItemAtPoint(new Point(x, y));
        }

        /// <summary>
        /// Gets the treeNodeAdvSubItem at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public TreeNodeAdvSubItem GetSubItemAtPoint(Point pt)
        {
            TreeNodeAdv node = null;
            TreeNodeAdvSubItem subItem = null;

            if (this.HasColumns)
            {
                node = PointToNode(pt);

                if (node != null)
                {
                    for (int i = 0; i < node.SubItems.Count; i++)
                    {
                        if (node.SubItems[i].Bounds.Contains(pt))
                        {
                            subItem = node.SubItems[i];
                        }
                    }
                }
            }

            return subItem;
        }

        /// <summary>
        /// Returns the node at the specified point. 
        /// </summary>
        /// <param name="pt">Specified point.</param>
        /// <returns>Node at specified point if exist; null otherwise.</returns>
        public TreeNodeAdv GetNodeAtPointEx(Point pt)
        {
            // Force any pending paint messages so that the bounds of nodes can be updated.
            this.Update();

            // if node doesn't visible than return null;
            if (this.HasColumns && !this.Columns[0].Visible)
            {
                return null;
            }

            TreeNodeAdv nd = PointToNode(pt);
            if (nd != null)
            {
                int rowIndex = NodeToRowIndex(nd);
                if (rowIndex >= this.VScrollPos && rowIndex <= NodeToRowIndex(LastVisibleNode))
                {
                    if (nd.TextBounds.Contains(pt))
                    {
                        return nd;
                    }
                    if (nd.TextAndImageBounds.Contains(pt))
                    {
                        return nd;
                    }
                }
                else
                {
                    return nd;
                }
            }

            return null;
        }
        #endregion

        #region Class overrides
        /// <summary>Method called by column when it style changed.</summary>
        /// <param name="e"></param>
        protected internal virtual void OnColumnChanged(TreeColumnStyleChangedEventArgs e)
        {
            // only if changed column width make re-layouting
            if (null != e && null != e.StyleChange && e.StyleChange.Sip.PropertyName == "Width")
            {
                Rectangle rect = e.Column.Bounds;
                rect.Y = -m_headerHeight;

                if (GetIsMirrored())
                {
                    rect.X = 0;
                }

                rect.Width = this.Width;
                rect.Height = this.Height;
                InvalidateNc(rect);

                RefreshHScrollbar();
                DoLayout();
            }
            else
            {
                InvalidateNc();
            }
        }

        /// <summary>
        /// Method called when selected column changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColumnSelected(TreeViewColumnSelectedChangedEventArgs e)
        {
            if (null != ColumnHighlighted)
            {
                ColumnHighlighted(this, e);
            }
        }

        /// <summary>
        /// Method called when column catch click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColumnClick(TreeColumnAdv column)
        {
            if (null != ColumnClick)
            {
                ColumnClick(this, new TreeViewColumnSelectedChangedEventArgs(column));
            }
        }

        /// <summary>
        /// Method called when column catch resized event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColumnResized(TreeColumnAdv column, int prevWidth, int currentWidth)
        {
            if (null != ColumnResized)
            {
                ColumnResized(this, new TreeViewColumnResizedEventArgs(column, prevWidth, currentWidth));
            }
        }

        /// <summary>
        /// Method called when column catch resizing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColumnResizing(TreeColumnAdv column, int prevPos, int currentPos)
        {
            if (null != ColumnResizing)
            {
                ColumnResizing(this, new TreeViewColumnResizeEventArgs(column, prevPos, currentPos));
            }
        }

        /// <summary>
        /// Method called when column catch double click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnColumnDoubleClick(TreeColumnAdv column)
        {
            if (null != ColumnDoubleClick)
            {
                ColumnDoubleClick(this, new TreeViewColumnSelectedChangedEventArgs(column));
            }
        }

        [DocumentationExclude()]
        protected virtual void OnSelectionModeChanged()
        {
            // reset selected nodes to satisfy new SelectionMode
            // in any case
            if (m_selectedNodes.Count > 1)
            {
                m_selectedNodes.RemoveRange(1, m_selectedNodes.Count - 1);
                ActiveNode = SelectedNode = m_selectedNodes[0];
            }
        }

        /// <summary>
        /// Raises the NodeEditorValidateString event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnNodeEditorValidateString method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnNodeEditorValidateString in a derived
        /// class, be sure to call the base class's OnNodeEditorValidateString method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnNodeEditorValidateString(TreeNodeAdvCancelableEditEventArgs e)
        {
            if (NodeEditorValidateString != null)
            {
                NodeEditorValidateString(this, e);
            }
        }

        /// <summary>
        /// Raises the NodeEditorValidating event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnNodeEditorValidating method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnNodeEditorValidating in a derived
        /// class, be sure to call the base class's OnNodeEditorValidating method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnNodeEditorValidating(TreeNodeAdvCancelableEditEventArgs e)
        {
            if (NodeEditorValidating != null)
            {
                this.IsEditEnding = true;
                NodeEditorValidating(this, e);
                this.IsEditEnding = false;
            }
        }

        /// <summary>
        /// Raises the NodeEditorValidated event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnNodeEditorValidated method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnNodeEditorValidated in a derived
        /// class, be sure to call the base class's OnNodeEditorValidated method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnNodeEditorValidated(TreeNodeAdvEditEventArgs e)
        {
            if (NodeEditorValidated != null)
            {
                NodeEditorValidated(this, e);
            }
        }

        /// <summary>
        /// Raises the edit cancel event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnEditCancelled method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnEditCancelled in a derived
        /// class, be sure to call the base class's OnEditCancelled method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnEditCancelled(TreeNodeAdvEditEventArgs e)
        {
            if (EditCancelled != null)
            {
                EditCancelled(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeEdit event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnBeforeEdit method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeEdit in a derived
        /// class, be sure to call the base class's OnBeforeEdit method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBeforeEdit(TreeNodeAdvBeforeEditEventArgs e)
        {
            if (BeforeEdit != null)
            {
                BeforeEdit(this, e);
            }
        }

        /// <summary>
        /// Raises the NodeStateImageListChanged event.
        /// </summary>
        protected virtual void OnNodeStateImageListChanged()
        {
            InvalidateWindow();

            RaiseNodeStateImageList();
        }

        /// <summary>
        /// Raises the DefaultExpandImageIndexChanged event.
        /// </summary>
        protected virtual void OnDefaultExpandImageIndexChanged()
        {
            Invalidate();

            RaiseDefaultExpandImageIndexChanged();
        }

        /// <summary>
        /// Raises the DefaultCollapseImageIndexChanged event.
        /// </summary>
        protected virtual void OnDefaultCollapseImageIndexChanged()
        {
            Invalidate();

            RaiseDefaultCollapseImageIndexChanged();
        }

        /// <summary>
        /// Raises the ItemDrag event.
        /// </summary>
        /// <param name="e">An ItemDragEventArgs that contains the event data.</param>
        /// <remarks><para>The OnItemDrag method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnItemDrag in a derived
        /// class, be sure to call the base class's OnItemDrag method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnItemDrag(ItemDragEventArgs e)
        {
            if (ItemDrag != null)
            {
                if (!(e.Item is TreeNodeAdv[]))
                {
                    throw new Exception("The OnItemDrag method is called with invalid argument type.");
                }

                ItemDrag(this, e);
            }
        }

        /// <summary>
        /// Raises the NodeBackgroundPaint event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnNodeBackgroundPaint method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnNodeBackgroundPaint in a derived
        /// class, be sure to call the base class's OnNodeBackgroundPaint method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnNodeBackgroundPaint(TreeNodeAdvPaintBackgroundEventArgs e)
        {
            if (this.NodeBackgroundPaint != null)
            {
                this.NodeBackgroundPaint(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeNodePaint event.
        /// </summary>
        /// <param name="e">An <see cref="TreeNodeAdvPaintEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnBeforeNodePaint method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeNodePaint in a derived
        /// class, be sure to call the base class's OnBeforeNodePaint method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBeforeNodePaint(TreeNodeAdvPaintEventArgs e)
        {
            if (BeforeNodePaint != null)
            {
                BeforeNodePaint(this, e);
            }
        }

        /// <summary>
        /// This event will be triggered if double click occurs on TreeNodeAdv 
        /// </summary>
        public event TreeNodeAdvMouseClickArgs NodeMouseDoubleClick;
        protected void RaiseNodeDoubleClick(TreeNodeAdv hitnode, MouseButtons button, int clicks, int x, int y, int delta)
        {
            if (this.NodeMouseDoubleClick != null)
            {
                this.NodeMouseDoubleClick(this, new MultiColumnTreeViewAdvMouseClickEventArgs(hitnode, button, clicks, x, y, delta));
            }
        }

        /// <summary>
        /// This event will be triggered if single click occurs on TreeNodeAdv 
        /// </summary>
        public event TreeNodeAdvMouseClickArgs NodeMouseClick;
        protected void RaiseNodeSingleClick(TreeNodeAdv hitnode, MouseButtons button, int clicks, int x, int y, int delta)
        {
            if (this.NodeMouseClick != null)
            {
                this.NodeMouseClick(this, new MultiColumnTreeViewAdvMouseClickEventArgs(hitnode, button, clicks, x, y, delta));
            }
        }

        /// <summary>
        /// This Event will be triggered when treenode match is found based on search string
        /// </summary>
        public event TreeViewOnAfterFindArgs OnNodeAfterFound;

        internal void RaiseNodeAfterFindEvent(TreeNodeAdv node, string searchText)
        {
            if (this.OnNodeAfterFound != null)
            {
                OnNodeAfterFound(this, new TreeNodeAdvAfterFindArgs(node, searchText));
            }
        }

        /// <summary>
        /// This Event will be triggered once treenode match yet to be found based on search string
        /// </summary>
        public event TreeViewOnBeforeFindArgs OnNodeBeforeFind;

        internal void RaiseNodeBeforeFindEvent(TreeNodeAdv node, string searchText)
        {
            TreeNodeAdvBeforeFindArgs args = new TreeNodeAdvBeforeFindArgs(node, searchText);
            if (this.OnNodeBeforeFind != null)
            {
                OnNodeBeforeFind(this, args);
                DisableFinding = args.Cancel;
            }
        }

        /// <summary>
        /// This Event will be triggered on matched treenode text is being replaced based on search string
        /// </summary>
        public event TreeViewOnReplacingArgs OnNodeReplacing;

        internal void RaiseNodeReplacingEvent(TreeNodeAdv node, string searchText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            TreeNodeAdvOnReplacingArgs args = new TreeNodeAdvOnReplacingArgs(node, searchText, replaceText, searchOption, searchRange);
            if (this.OnNodeReplacing != null)
            {
                OnNodeReplacing(this, args);
                this.DisableReplacing = args.Cancel;
            }
        }

        /// <summary>
        /// This Event will be triggered on matched treenode text after gets replaced based on search string
        /// </summary>
        public event TreeViewOnReplacedArgs OnNodeReplaced;

        internal void RaiseNodeReplacedEvent(TreeNodeAdv node, string searchText, string replaceText)
        {
            if (this.OnNodeReplaced != null)
            {
                OnNodeReplaced(this, new TreeNodeAdvOnReplacedArgs(searchText, replaceText, node));
            }
        }


        /// <summary>
        /// Raises the AfterNodePaint event.
        /// </summary>
        /// <param name="e">An <see cref="TreeNodeAdvPaintEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnAfterNodePaint method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterNodePaint in a derived
        /// class, be sure to call the base class's OnAfterNodePaint method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnAfterNodePaint(TreeNodeAdvPaintEventArgs e)
        {
            if (AfterNodePaint != null)
            {
                AfterNodePaint(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeExpand event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnBeforeExpand method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeExpand in a derived
        /// class, be sure to call the base class's OnBeforeExpand method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBeforeExpand(TreeViewAdvCancelableNodeEventArgs e)
        {
            if (BeforeExpand != null)
            {
                BeforeExpand(this, e);
            }
            if (this.CustomControlCollection.Count > 0)
                this.NeedUpdateCustomControls = true;
        }

        /// <summary>
        /// Raises the BeforeCollapse event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnBeforeCollapse method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeCollapse in a derived
        /// class, be sure to call the base class's OnBeforeCollapse method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBeforeCollapse(TreeViewAdvCancelableNodeEventArgs e)
        {
            if (BeforeCollapse != null)
            {
                BeforeCollapse(this, e);
            }
        }

        /// <summary>
        /// Raises the AfterExpand event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnAfterExpand method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterExpand in a derived
        /// class, be sure to call the base class's OnAfterExpand method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnAfterExpand(TreeViewAdvNodeEventArgs e)
        {
            this.ValidateScrollPosition();

            if (AfterExpand != null)
            {
                AfterExpand(this, e);
            }
        }

        /// <summary>
        /// Raises the AfterCollapse event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnAfterCollapse method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterCollapse in a derived
        /// class, be sure to call the base class's OnAfterCollapse method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnAfterCollapse(TreeViewAdvNodeEventArgs e)
        {
            this.ValidateScrollPosition();

            if (AfterCollapse != null)
            {
                AfterCollapse(this, e);
            }
            if (this.CustomControlCollection.Count > 0)
            {
                this.NeedUpdateCustomControls = true;
                Invalidate();
            }
        }

 
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (m_toolTip.IsShowing() && (this.VerticallScrollBar || this.HorizontalScrollBar))
            {
                m_toolTip.HidePopup();
            }

            if (this.IsEditing)
            {
                this.CancelEditMode();
            }

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// Raises the AfterSelect event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnAfterSelect method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterSelect in a derived
        /// class, be sure to call the base class's OnAfterSelect method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnAfterSelect(EventArgs e)
        {
            if (AfterSelect != null)
            {
                AfterSelect(this, e);
            }
        }

        /// <summary>
        /// Raises the AfterCheck event.
        /// </summary>
        /// <param name="e">A <see cref="TreeNodeAdvEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnAfterCheck method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterCheck in a derived
        /// class, be sure to call the base class's OnAfterCheck method so that
        /// registered delegates receive the event.</para></remarks>
        protected internal virtual void OnAfterCheck(TreeNodeAdvEventArgs e)
        {
            if (this.IsMouseDown)
            {
                e.Action = TreeViewAdvAction.ByMouse;
            }

            if (this.IsKeyDown)
            {
                e.Action = TreeViewAdvAction.ByKeyboard;
            }

            // add node to or remove from checked nodes collection
            if (e.Node != null)
            {
                m_checkedNodes.ResolveNode(e.Node);
            }

            if (AfterCheck != null)
            {
                AfterCheck(this, e);
            }
        }

        /// <summary>
        /// Raises the AfterInteractiveChecks event.
        /// </summary>
        /// <param name="e">A <see cref="TreeNodeAdvEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnAfterInteractiveChecks method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnAfterInteractiveChecks in a derived
        /// class, be sure to call the base class's OnAfterInteractiveChecks method so that
        /// registered delegates receive the event.</para></remarks>
        protected internal virtual void OnAfterInteractiveChecks(TreeNodeAdvEventArgs e)
        {
            if (AfterInteractiveChecks != null)
            {
                AfterInteractiveChecks(this, e);
            }
        }

        /// <summary>
        /// Raises the BeforeSelect event.
        /// </summary>
        /// <param name="e">An <see cref="TreeViewAdvCancelableSelectionEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnBeforeSelect method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeSelect in a derived
        /// class, be sure to call the base class's OnBeforeSelect method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBeforeSelect(TreeViewAdvCancelableSelectionEventArgs e)
        {
            if (BeforeSelect != null)
            {
                this.Invoke(BeforeSelect, new object[] { this, e });
            }
        }

        /// <summary>
        /// Raises the BeforeCheck event.
        /// </summary>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.TreeViewAdvBeforeCheckEventArgs"/> that contains the event data.</param>
        /// <remarks><para>The OnBeforeCheck method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBeforeCheck in a derived
        /// class, be sure to call the base class's OnBeforeCheck method so that
        /// registered delegates receive the event.</para></remarks>
        protected internal virtual void OnBeforeCheck(TreeNodeAdvBeforeCheckEventArgs e)
        {
            if (BeforeCheck != null)
            {
                BeforeCheck(this, e);
            }
        }

        /// <summary>
        /// Selects a default node (the first visible one) if the tree did not
        /// have anything focused.
        /// </summary>
        protected virtual void SetDefaultSelectionOnEnter()
        {
            if (this.Root.HasNodes)
            {
                // If the mouse is down on a node, then select that node instead.
                // This will be the case when the tree gets focus via mouse down the first time.
                if (Control.MouseButtons != MouseButtons.None)
                {
                    TreeNodeAdv nodeBelowMouse = GetNodeAtPoint(PointToClient(Control.MousePosition));

                    if (nodeBelowMouse != null && nodeBelowMouse.Enabled
            && SetSelectedNode(nodeBelowMouse, m_selectedNodes, TreeViewAdvAction.Unknown))
                    {
                        this.ActiveNode = nodeBelowMouse;
                        m_selectionBaseNode = nodeBelowMouse;
                    }
                }

                if (this.SelectedNode == null)
                {
                    // This logic should change once Enabled property is supported.
                    TreeNodeAdv newSelNode = this.Root.NextSelectableNode;

                    if (SetSelectedNode(newSelNode, m_selectedNodes, TreeViewAdvAction.Unknown))
                    {
                        this.ActiveNode = newSelNode;
                        m_selectionBaseNode = newSelNode;
                    }
                }
            }
        }

        protected override void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            if (this.VScrollPos != VScrollBar.Value)
            {
                this.InVScroll = true;
                this.VScrollPos = VScrollBar.Value;
                this.InVScroll = false;
            }
            base.OnVScrollBarValueChanged(sender, e);
        }

        protected override void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            if (this.HScrollPos != HScrollBar.Value)
            {
                this.InHScroll = true;
                this.HScrollPos = HScrollBar.Value;
                this.InHScroll = false;
            }

            InvalidateNc();
            base.OnHScrollBarValueChanged(sender, e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (m_root != null)
            {
                rootMaxXChanged(m_root.MaxX);
            }

            ValidateScrollPosition();
            RefreshHScrollbar();

            if (null != Root)
            {
                RefreshVScrollbar();
            }

            if (this.BackgroundColor.Style == BrushStyle.Gradient || GetIsMirrored() || IsColumnsGradient())
            {
                Invalidate(this.ClientRectangle);
            }

            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            // System.Diagnostics.Trace.WriteLine("OnInvalidated" + e.InvalidRect);
            base.OnInvalidated(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) > 0)
            {
                this.RMouseDownNode = (this.HasColumns && !this.Columns[0].Visible) ? null :
          PointToNode(new Point(e.X, e.Y));
            }

            base.OnMouseDown(e);

            if (IsEditing)
            {
                return;
            }

            this.IsMouseDown = true;
            this.Dragging = false;
            this.ClickedOnSelection = false;
            this.CanDrag = false; // Enabled dragging on mouse move.

            Point pt = new Point(e.X, e.Y);
            m_ptMouseDown = pt;
            m_lastButtonDownPoint = pt;
            m_lastButtonDownNode = GetNodeAtPoint(pt);

            bool newlyFocused = !this.ContainsFocus;
            bool leftButtonClick = (e.Button == MouseButtons.Left);
            bool rightButtonClick = (e.Button == MouseButtons.Right);
            this.IsLeftMouseDown = leftButtonClick;

            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 1 && this.GetNodeAtPoint(e.X, e.Y) != null)
            {
                this.RaiseNodeSingleClick(this.GetNodeAtPoint(e.X, e.Y), e.Button, e.Clicks, e.X, e.Y, e.Delta);
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Clicks == 2 && this.GetNodeAtPoint(e.X, e.Y) != null)
            {
                this.RaiseNodeDoubleClick(this.GetNodeAtPoint(e.X, e.Y), e.Button, e.Clicks, e.X, e.Y, e.Delta);
            }
            // Tells the node to process the click.
            TreeNodeAdv nodeAtPt = GetNodeAtPoint(pt);
            if (nodeAtPt == null)
            {
                nodeAtPt = this.Root;
            }

            if (leftButtonClick && nodeAtPt.ProcessMouseDown(pt))
            {
                this.IsMouseDown = false;
                return;
            }

            bool prevFocus = Focused;
            // If some other control fails validating and takes away the focus, then just return
            if (prevFocus && !Focused)
            {
                this.IsMouseDown = false;
                return;
            }

            bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);
            bool continueProcessing = true;

            if (leftButtonClick && m_activeNode != null)
            {
                bool bToolTipClicked = CheckToolTipClicked(pt);

                if (e.Clicks == 2)
                {
                    continueProcessing = false;
                }
                else if (e.Clicks == 1)
                {
                    if (bToolTipClicked)
                    {
                        OnClick(e);

#if ! ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                        OnMouseClick(e);
#endif
                    }
                }
            }

            if (e.Clicks == 2)
            {
                OnMouseUp(e);
                this.IsMouseDown = false;
                return;
            }

            if (continueProcessing)
            {
                // Check if we should turn on mouse based selection:
                if (leftButtonClick && this.AllowMouseBasedSelection)
                {
                    TreeNodeAdv mouseDownNode = GetNodeAtPoint(pt, false, !this.FullRowSelect);

                    if (mouseDownNode == null)
                    {
                        mouseDownNode = this.Root;
                    }

                    if (mouseDownNode != m_activeNode && !this.SelectedNodes.Contains(mouseDownNode))
                    {
                        this.MouseBasedSelectionOn = true;
                    }
                }

                // If NO Ctrl || Shift
                if (!bCtrl && !bShift)
                {
                    if (leftButtonClick)
                    {
                        // Clicked on active node
                        if (m_activeNode != null && m_activeNode.Bounds.Contains(pt))
                        {
                            if (m_activeNode.TextBounds.Contains(pt)
                || this.FullRowSelect)
                            {
                                // Can start the edit mode.
                                if (!newlyFocused)
                                {
                                    this.ClickedOnSelection = true;
                                }
                                // Can start a drag
                                this.CanDrag = true;
                            }
                            else if (!this.DragOnText)
                            {
                                this.CanDrag = true;
                            }
                        }
                    }
                    else if (rightButtonClick)
                    {
                        if (this.RMouseDownNode != null && this.RMouseDownNode.Bounds.Contains(pt))
                        {
                            if (!this.DragOnText ||
                RMouseDownNode.TextBounds.Contains(pt) ||
                this.FullRowSelect)
                            {
                                this.CanDrag = true;
                            }
                        }
                    }
                }

                if (!leftButtonClick)
                {
                    m_lastSelectedNode = GetNodeAtPoint(pt);
                    this.EnsureVisibleFlag = false;

                    if (m_lastSelectedNode != null)
                    {
                        ApplyMouseBasedSelectionOn(m_lastSelectedNode);
                    }

                    this.EnsureVisibleFlag = true;
                    this.RMouseDownNode = m_lastSelectedNode;
                    m_lastSelectedNode = null;

                    this.IsMouseDown = false;
                }

                if (continueProcessing)
                {
                    // Clicked on selection nodes WITHOUT Ctrl || Shift
                    TreeNodeAdv nodeAtPoint = GetNodeAtPoint(pt);
                    if (nodeAtPoint == null)
                    {
                        nodeAtPoint = this.Root;
                    }

                    if (m_selectedNodes.Contains(nodeAtPoint) && !bCtrl && !bShift)
                    {
                        if (!newlyFocused)
                            //Need set to true to change selection, doesn't depend wheather clicking on text area or not.
                            this.ClickedOnSelection = true;

                        if (!this.DragOnText || (this.DragOnText && nodeAtPoint.TextBounds.Contains(pt)) || this.FullRowSelect)
                        {
                            this.CanDrag = true;
                        }
                    }
                    else
                    {
                        TreeNodeAdv retNode = GetNodeAtPoint(pt, false, !this.FullRowSelect);

                        // Mouse down on a new node.
                        if (retNode != null && retNode.Enabled)
                        {
                            if (!this.DragOnText || (this.DragOnText && retNode.TextBounds.Contains(pt)) || this.FullRowSelect)
                            {
                                this.CanDrag = true;
                            }

                            if (this.SingleSelect && bCtrl)
                            {
                                this.ClickedOnSelection = true;
                            }

                            // If NO Ctrl & Shift, Add to selection only the clicked node
                            if ((!bCtrl && !bShift) || this.SingleSelect)
                            {
                                m_lMouseDownNode = retNode;
                            }
                            // Control pressed or Shift and Control both pressed
                            else if (!bShift || (bCtrl && bShift))
                            {
                                // For all cases
                                m_lMouseDownNode = retNode;
                            }
                            // Shift pressed
                            else
                            {
                                bool removeCurrentMultipleSelection = !bCtrl;

                                // Scroll position should not move while selecting node by mouse
                                m_pressedKey = Keys.None;
                                this.EnsureVisibleFlag = false;
                                ExtendSelectionTo(retNode, removeCurrentMultipleSelection);
                                this.EnsureVisibleFlag = true;
                            }
                        }

                        this.StartAutoScrollingInternal();
                    }

                    if (leftButtonClick)
                    {
                        m_lastSelectedNode = GetNodeAtPoint(pt);
                        this.LMouseDownNode = m_lastSelectedNode;
                        this.EnsureVisibleFlag = false;

                        if (m_lastSelectedNode != null)
                        {
                            ApplyMouseBasedSelectionOn(m_lastSelectedNode);
                        }

                        this.EnsureVisibleFlag = true;
                        m_lastSelectedNode = null;
                        this.IsMouseDown = false;
                    }
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.IsMouseUp = true;

            bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

            // This code snippet was added by Lucas in order to fix problem 169.
            if (this.AllowDropStubWorks)
            {
                m_draggedTnas = null;
                this.AllowDropStubWorks = false;
                DragHelper.EndDrag();
            }

            m_highlightedNode = null;
            m_lastButtonDownNode = null;
            m_pressedColumn = null;

            this.MouseBasedSelectionOn = false;

            NodesSelectionOnMouseUp(bCtrl, bShift, e);

            m_ptMouseDown = Point.Empty;

            base.OnMouseUp(e);
            this.IsMouseUp = false;
        }
        /// <summary>
        /// Method handles nodes selection and scroll events on OnMouseUp event.
        /// </summary>
        /// <param name="bCtrl"> Indicates whether Ctrl key is pressed. </param>
        /// <param name="bShift"> Indicates whether Shift key is pressed. </param>
        /// <param name="e"> Mouse event args. </param>
        private void NodesSelectionOnMouseUp(bool bCtrl, bool bShift, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point pt = new Point(e.X, e.Y);
                TreeNodeAdv node = GetNodeAtPoint(pt);

                // If it was clicked on selection update selection on mouse up in case of dragging begin.
                if (this.ClickedOnSelection && m_ptMouseDown == pt)
                {
                    if (node != null)
                    {
                        ArrayList newNodes = new ArrayList(new TreeNodeAdv[] { node });

                        if (SetSelectedNode(newNodes, m_selectedNodes, TreeViewAdvAction.ByMouse, true, true))
                        {
                            this.ActiveNode = node;
                            m_selectionBaseNode = node;

                            if (!bCtrl && node.TextBounds.Contains(pt))
                            {
                                this.LabelEditStartTimer = new Timer();
                            }
                        }
                    }
                }

                if (!bShift || (bCtrl && bShift))
                {
                    if (this.MultiSelect)
                    {
                        //Add/Remove to the SelectedNodes.
                        if (m_selectedNodes.Contains(node) && this.IsMouseDownWithCtrl)
                        {
                            if (SetSelectedNode(null, node, TreeViewAdvAction.ByMouse))
                            {
                                this.ActiveNode = node;
                            }

                            this.IsMouseDownWithCtrl = false;
                        }
                    }
                }

                this.LMouseDownNode = this.RMouseDownNode = null;

                StopAutoScrollingInternal();
            }
            else
            {
                this.Dragging = false;
                Invalidate();

                if (e.Button == MouseButtons.Right)
                {
                    this.RMouseDownNode = null;
                }
            }

        }

        protected override void OnDoubleClick(EventArgs e)
        {
            Point pt = PointToClient(Cursor.Position);
            TreeNodeAdv node = GetNodeAtPoint(pt);

            if (node != null)
            {
                CheckBoxPart checkBox = node.CheckBox as CheckBoxPart;

                if (node.PlusMinus.Visible
                    && !(null != checkBox && checkBox.Visible && node.EnabledButtons && checkBox.Bounds.Contains(pt)))
                {
                    Rectangle pmBounds = node.PlusMinus.Bounds;
                    // Provide some leeway around the bounds.
                    pmBounds.Inflate(4, 3);

                    if (!pmBounds.Contains(pt))
                    {
                        // fix for 2409
                        if ((this.IsLeftMouseDown && node.Bounds.Contains(pt)) && (node.HasNodes || this.LoadOnDemand))
                        {
                            node.Expanded = !node.Expanded;
                        }
                    }
                }
            }

            ClearSelectionClickMonitors();
            base.OnDoubleClick(e);
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.Root.RecalculateAllDimensions();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.MouseLeaved = false;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Point pt = this.PointToClient(new Point(MousePosition.X, MousePosition.Y));
            TreeNodeAdv node = this.GetNodeAtPoint(pt);
            if (node != m_lastNodeOver && node != null && this.HotTracking)
            {
                if (m_lastNodeOver != null)
                {
                    Invalidate(m_lastNodeOver.Bounds);
                }
                m_lastNodeOver = node;
                if (NodeHotTrackChanged != null)
                {
                    RaiseNodeHotTracked();
                }
            }

            // This part of code is used for resizing columns.
            ProcessColumnResize(e);

            if (this.AllowDropStubWorks)
            {
                GiveFeedbackEventArgs ea = new GiveFeedbackEventArgs(DragDropEffects.None, true);

                OnGiveFeedback(ea);

                // Fix for defect #1247: MultiColumnTreeView has apparently not notified that the 
                // "drag and drop" operation has ended.
                m_draggedTnas = null;
                this.AllowDropStubWorks = false;
                DragHelper.EndDrag();

                return;
            }

            // Call this method to continue listening to MouseHover, otherwise
            // MouseHover doesn't occur until you move out of the bounds and come back in.
            ResetMouseEventArgs();

            if (e.Button == MouseButtons.None && this.ContextMenu != null)
            {
                // If the user showed a context menu on mouse_down then we don't get a mouseUp where we could
                // have turned this off.
                this.RMouseDownNode = null;
                this.LMouseDownNode = null;
            }

            if (this.IgnoreNextMouseMove)
            {
                // When the help-text tooltip is shown, that immediately generates a MouseMove on the tree.
                this.IgnoreNextMouseMove = false;
                return;
            }

            if (m_helpText != null && m_helpText.IsShowing())
            {
                if (m_tnHelpTextNode != null)
                {
                    if (!m_tnHelpTextNode.TextBounds.Contains(e.X, e.Y))
                    {
                        m_helpText.HidePopup();
                        m_tnHelpTextNode = null;
                    }
                }
                else if (this.GetSubItemAtPoint(e.X, e.Y) != m_previousHelpTextSubItem)
                {
                    m_helpText.HidePopup();
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                ProcessLeftMouseButton(e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ProcessRightMouseButton(e);
            }

            // Invalidate mouse over nodes if hottracking is enabled.
            ProcessHotTracking(e);
        }

        private void ProcessColumnResize(MouseEventArgs e)
        {
            if (this.ColumnsMovedMode)
            {
                // initializing helper variables
                Point pt = PointToScreen(new Point(e.X, e.Y));
                int sign = (GetIsMirrored()) ? -1 : 1;
                int tempOffset = sign * (pt.X - m_iColumnsPressedPosition);

                // check on validate
                TreeColumnAdv[] columns = this.Columns.VisibleColumns;

                if (columns.Length > m_iColumnsPressedIndex)
                {
                    TreeColumnAdv column = columns[m_iColumnsPressedIndex];

                    if (column.Width + tempOffset < DEF_MIN_COLUMN_WIDTH)
                    {
                        tempOffset = DEF_MIN_COLUMN_WIDTH - column.Width;
                    }

                    Cursor.Current = Cursors.VSplit;

                    int prevPos = sign * m_iColumnsOffset;
                    int currentPos = sign * tempOffset;

                    // clears last drawed line in old position.
                    DrawReversibleLine(prevPos);
                    // draws new line in valid position.
                    DrawReversibleLine(currentPos);

                    // rizes event
                    OnColumnResizing(column, prevPos, currentPos);

                    // change offset
                    m_iColumnsOffset = tempOffset;
                }
            }
        }
        private void ProcessHotTracking(MouseEventArgs e)
        {
            if (this.HotTracking)
            {
                TreeNodeAdv lastVisibleNode = this.LastVisibleNode;

                int y = 0;

                if (lastVisibleNode != null)
                {
                    y = lastVisibleNode.Bounds.Bottom;
                }

                Point pt = new Point(e.X, e.Y);

                TreeNodeAdv node = null;
                if (pt.Y <= y)
                // Call this sparingly. Calling this only if the mouse is above the last visible node.
                {
                    node = GetNodeAtPoint(pt);
                }

                if (node != m_lastNodeOver)
                {
                    if (m_lastNodeOver != null)
                    {
                        // Clear previous hot-tracking node:
                        Invalidate(m_lastNodeOver.Bounds);
                    }
                    m_lastNodeOver = node;
                }
                if (node != null)
                {
                    // Redraw current hot-tracking node.
                    Invalidate(node.Bounds);
                }
            }
        }

        private void ProcessRightMouseButton(MouseEventArgs e)
        {
            if (!this.ColumnsMovedMode && !this.Dragging && this.CanDrag && this.RMouseDownNode != null)
            {
                if (IsDraggedPastMouseDownPoint(new Point(e.X, e.Y)))
                {
                    TreeNodeAdv[] tnas = null;

                    if (m_selectedNodes.Contains(this.RMouseDownNode))
                    {
                        tnas = m_selectedNodes.ToArray(typeof(TreeNodeAdv)) as TreeNodeAdv[];
                    }
                    else
                    {
                        tnas = new TreeNodeAdv[] { this.RMouseDownNode };
                    }

                    // Let the user begin a drag and drop.
                    RaiseItemDrag(new ItemDragEventArgs(e.Button, tnas));

                    // Otherwise the tree thinks that we are still dragging even after an Esc.
                    this.CanDrag = false;
                }
            }
        }

        private void ProcessLeftMouseButton(MouseEventArgs e)
        {
            TreeNodeAdv[] tnas = null;

            if (!this.ColumnsMovedMode && IsDraggedPastMouseDownPoint(new Point(e.X, e.Y)))
            {
                if (m_activeNode != m_root && !this.Dragging && this.CanDrag)
                {
                    if (this.LMouseDownNode != null && (this.SingleSelect))
                    {
                        tnas = new TreeNodeAdv[] { this.LMouseDownNode };
                    }
                    else if (this.MultiSelect && m_selectedNodes.Count > 0)
                    {
                        ArrayList tnasArray = new ArrayList(m_selectedNodes.ToArray(typeof(TreeNodeAdv)));

                        if (this.LMouseDownNode != null)
                        {
                            bool needAdd = true;

                            if (this.SelectionMode == TreeSelectionMode.MultiSelectSameLevel)
                            {
                                TreeNodeAdv treeNode = m_selectedNodes[0];
                                needAdd = (treeNode.Parent == this.LMouseDownNode.Parent);
                            }

                            if (tnasArray.Contains(this.LMouseDownNode))
                            {
                                needAdd = false;
                            }

                            if (needAdd)
                            {
                                tnasArray.Add(this.LMouseDownNode);
                            }
                        }

                        tnas = tnasArray.ToArray(typeof(TreeNodeAdv)) as TreeNodeAdv[];
                    }
                    else if (this.SingleSelect && this.SelectedNode != null)
                    {
                        tnas = new TreeNodeAdv[] { this.SelectedNode };
                    }

                    TreeNodeAdv prevLMouseDownNode = this.LMouseDownNode;
                    this.LMouseDownNode = null;

                    if (tnas != null)
                    {
                        // Let the user begin a drag and drop.
                        RaiseItemDrag(new ItemDragEventArgs(e.Button, tnas));

                        // This happens when sometimes OnQueryFeedback doesn't get called.
                        if (this.DragCueOn)
                        {
                            this.DragCueOn = false;
                            DragHelper.EndDrag();
                        }

                        // Otherwise the tree thinks that we are still dragging even after an Esc.
                        this.CanDrag = false;

                        // If no dragging took place:
                        if (this.MouseBasedSelectionOn)
                        // ProcessMouseBasedSelection will reset this anyway,
                        // but a paint occurs before the reset happens and the user
                        // see a flicker. We do this to avoid that flicker.
                        {
                            this.LMouseDownNode = prevLMouseDownNode;
                        }
                    }
                }

                if (this.MouseBasedSelectionOn)
                {
                    this.ProcessMouseBasedSelection(e);
                }
                else
                {
                    this.LMouseDownNode = null;
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.MouseLeaved = true;

            // Clear hot-tracking node, if any.   
            if (m_lastNodeOver != null)
            {
                Invalidate(m_lastNodeOver.Bounds);
                m_lastNodeOver = null;
            }
            base.OnMouseLeave(e);

            if (Control.MouseButtons == MouseButtons.None) // Make sure this is not called from treeViewAdv.DoDragDrop
            {
                StopAutoScrollingInternal();
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            if (this.HideSelection)
            {
                Invalidate();
                InvalidateNc();
            }

            base.OnLeave(e);
        }

        /// <summary>
        /// This method indicates that the multipleNodes can be selected with mouseDown and drag .
        /// </summary>
        /// <param name="e"></param>
        protected virtual void ProcessMouseBasedSelection(MouseEventArgs e)
        {
            TreeNodeAdv mouseOverNode = GetNodeAtPoint(new Point(e.X, e.Y), false);

            if (mouseOverNode == null || mouseOverNode == this.Root || mouseOverNode.Enabled == false)
            {
                return;
            }

            if (this.SingleSelect)
            {
                this.LMouseDownNode = mouseOverNode;
            }
            else
            {
                if (LMouseDownNode != null)
                {
                    ApplyMouseBasedSelectionOn(this.LMouseDownNode);
                    this.LMouseDownNode = null;
                    // But continue selection:
                    //this.MouseBasedSelectionOn = false;
                }
                else
                {
                    ExtendSelectionTo(mouseOverNode, true);
                }
            }
        }

        [DocumentationExclude()]
        protected virtual void UpdateTips(bool mouseHover)
        {
            TreeNodeAdv node = null;
            TreeNodeAdvSubItem subItem = null;
            bool hidePopups = false;

            // Don't show tooltips if in edit mode.
            // hide tooltip when we have't focus and mouse is not hover.
            if (!(this.ContainsFocus || this.Focused || mouseHover) || this.IsEditing)
            {
                hidePopups = true;
            }

            // If this is not called form mouse-hover and if the parent form
            // doesn't have focus:
            Form form = this.FindForm();

            // Form.ContainsFocus returns true even if a dialog is currently open!
            // So the tips stay open even if a MessageBox is shown.
            if (!mouseHover && form != null && !form.ContainsFocus)
            {
                hidePopups = true;
            }

            if (!hidePopups)
            {
                Point ptClient = PointToClient(Control.MousePosition);

                // Show tooltips only if the mouse hovers over text.
                node = GetNodeAtPoint(ptClient, true, false);

                subItem = this.GetSubItemAtPoint(ptClient);

                if (node == null || node == m_root || ptClient.X < node.TextBounds.Left || ptClient.X > node.TextBounds.Right)
                {
                    hidePopups = true;
                }
            }

            if (subItem != null)
            {
                if (subItem.HelpText != null && subItem.HelpText.Length > 0)
                {
                    if (!m_toolTip.IsShowing())
                    {
                        m_helpText.PopupParent = null;
                    }

                    if (!m_helpText.IsShowing())
                    {
                        Point pt = new Point(Control.MousePosition.X, Control.MousePosition.Y + 30);
                        this.IgnoreNextMouseMove = true;
                        m_helpText.Text = subItem.HelpText;
                        m_tnHelpTextNode = null;
                        m_previousHelpTextSubItem = subItem;

                        m_helpText.ShowPopup(pt);
                    }
                }
                return;
            }

            if (hidePopups)
            {
                m_helpText.HidePopup();
                m_toolTip.HidePopup();
                return;
            }

            Rectangle rc = new Rectangle(node.TextBounds.Location, node.TextBounds.Size);
            rc.Intersect(TreeColumnRectangle);

            if (node.TextBounds != rc)
            {
                if (!m_toolTip.IsShowing())
                {
                    m_toolTip.Font = node.Font;
                    m_toolTip.Text = node.Text;

                    m_toolTip.ShowPopup(PointToScreen(node.TextBounds.Location));
                }

                m_toolTip.CurrentPopupChild = m_helpText;
                m_helpText.PopupParent = m_toolTip;
            }

            if (node.HelpText != null && node.HelpText.Length > 0)
            {
                // Reset the PopupParent relationship if the toolTip is not showing.
                if (!m_toolTip.IsShowing())
                {
                    m_helpText.PopupParent = null;
                }

                if (!m_helpText.IsShowing())
                {
                    Point pt = new Point(Control.MousePosition.X, Control.MousePosition.Y + 30);
                    this.IgnoreNextMouseMove = true;
                    m_helpText.Text = node.HelpText;
                    m_tnHelpTextNode = node;

                    m_helpText.ShowPopup(pt);
                }
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            try
            {
                this.UpdateTips(true);
            }
            finally
            {
                // Commented out on 7/26/04. Moved this to OnMouseMove, otherwise, MouseHover gets call multiple times for each hover.

                // Call this method to continue listening to MouseHover, otherwise
                // MouseHover occurs only once.
                //ResetMouseEventArgs();
            }
        }

        protected override void OnSetCursor(ref Message m)
        {
            Cursor.Current = this.Cursor;
        }


        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            this.Dragging = true;
            base.OnDragEnter(drgevent);

            m_helpText.HidePopup();
            m_toolTip.HidePopup();

            Invalidate();

            this.StartAutoScrollingInternal();
        }

        protected override void OnDragLeave(EventArgs e)
        {
            m_highlightedNode = null;
            base.OnDragLeave(e);
            this.Dragging = false;
            Invalidate();
            StopAutoScrollingInternal();
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            // Define on what node cursor is located.
            Point p = new Point(e.X, e.Y);
            p = PointToClient(p);
            m_highlightedNode = GetNodeAtPoint(p);

            if (m_highlightedNode == m_lastButtonDownNode)
            {
                m_highlightedNode = null;
            }
            this.MouseBasedSelectionOn = false;
            base.OnDragOver(e);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            this.Dragging = false;
            Invalidate();

            // Let the client use his own semantics for insertion.
            base.OnDragDrop(drgevent);
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs args)
        {
            base.OnQueryContinueDrag(args);

            if (args.Action == DragAction.Drop)
            {
                m_highlightedNode = null;
                DragHelper.EndDrag();
                this.DragCueOn = false;
                this.CanGetDragImage = false;
            }
            else if (args.Action == DragAction.Cancel)
            {
                DragHelper.CancelDrag();
                this.DragCueOn = false;
                this.CanGetDragImage = false;
            }
            else
            {
                this.CanGetDragImage = true;
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs args)
        {
            base.OnGiveFeedback(args);

            if (this.ShowDragNodeCue)
            {
                if (this.CanGetDragImage)
                {
                    this.DragCueOn = true;

                    if (DragHelper.IsDragging
                        // This condition is added by Lucas in order
                        // to resolve problem # 169.
            || this.AllowDropStubWorks)
                    {
                        Point pt = GetDragWindowLocation(true);
                        DragHelper.DoDrag(pt, args.Effect);
                    }
                }
            }
        }

        // Do this instead of listening to KeyDown in TextBox which prevents the gong sound when hitting esc. in text box.
        // In a native app. scenario this won't be hit, so we instead listen to the TextBox's KeyDown event and process the keys there.
 
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.IsEditing)
            {
                if (keyData == Keys.Return)
                {
                    if (this.IsEditing)
                    {
                        EndEdit(false);
                        return true;
                    }
                }
                else if (keyData == Keys.Escape)
                {
                    // This gets called only when the label editor has focus.
                    this.CancelEdit = true;
                    EndEdit(true);
                    this.CancelEdit = false;

                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            bool bBaseIsInputKey = true;

            if ((this.LabelEdit && keyData == Keys.F2) || keyData == Keys.Return || keyData == Keys.Escape)
            {
                bBaseIsInputKey = false;
            }
            else
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.PageDown:
                    case Keys.PageUp:
                    case Keys.Home:
                    case Keys.End:
                        bBaseIsInputKey = !((keyData & Keys.Control) == Keys.Control || this.ActiveNode != null);
                        break;

                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Left:
                        bBaseIsInputKey = !(this.ActiveNode != null);
                        break;

                    case Keys.Space:
                        bBaseIsInputKey = false;
                        break;

                    case Keys.Back:
                        bBaseIsInputKey = !this.AllowKeyboardSearch;
                        break;
                }
            }

            if (bBaseIsInputKey)
            {
                return base.IsInputKey(keyData);
            }
            else
            {
                return true;
            }
        }
 
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // It ensures the visibility of the node and moves the scroll bar position according to the selection.
            EnsureVisibleSelectedNode = true;
            Keys keyData = e.KeyData;
            Keys keyPressed = keyData & Keys.KeyCode;

            if (keyData == Keys.F2 && this.LabelEdit)
            {
                m_pressedKey = Keys.F2;

                bool bIsAnyMouseButtonDown = (Control.MouseButtons == MouseButtons.Left ||
          Control.MouseButtons == MouseButtons.Right);

                if (bIsAnyMouseButtonDown && m_lastButtonDownPoint != Point.Empty)
                {
                    if (GetNodeAtPointEx(m_lastButtonDownPoint) != null)
                    {
                        return;
                    }
                }

                BeginEdit();
                e.Handled = true;
            }
            else if (keyData == Keys.Return)
            {
                if (this.ActiveNode != null && this.ActiveNode.HasNodes)
                {
                    this.ActiveNode.Expanded = !this.ActiveNode.Expanded;

                    e.Handled = true;
                }
            }

            if (!e.Handled && keyData == Keys.Escape)
            {
                m_pressedKey = Keys.Escape;
                CancelMouseBasedSelection();
                e.Handled = true;

                // Fix for the issue with Focus , when escape key is pressed  in treeViewAdv displayed in a dialog
                if (this.Parent is Form && ((Form)this.Parent).CancelButton != null)
                {
                    ((Form)this.Parent).CancelButton.PerformClick();
                }
            }

            if (!e.Handled)
            {
                if (keyPressed == Keys.PageDown)
                {
                    ProcessPageDownKey(e);
                }
                else if (keyPressed == Keys.PageUp)
                {
                    ProcessPageUpKey(e);
                }
                else if (keyPressed == Keys.Home)
                {
                    ProcessHomeKey(e);
                }
                else if (keyPressed == Keys.End)
                {
                    ProcessEndKey(e);
                }
                else if (keyPressed == Keys.Up && this.ActiveNode != null)
                {
                    ProcessUpKey(e);
                }
                else if (keyPressed == Keys.Down && this.ActiveNode != null)
                {
                    ProcessDownKey(e);
                }
                else if ((keyData == (GetIsMirrored() ? Keys.Right : Keys.Left) || keyData == Keys.Back) && this.ActiveNode != null)
                {
                    ProcessLeftOrBackSpaceKey(e);
                }
                else if (keyData == (GetIsMirrored() ? Keys.Left : Keys.Right) && this.ActiveNode != null)
                {
                    ProcessRightArrowKey(e);
                }
                else if (keyPressed == Keys.Space)
                {
                    ProcessSpaceKey(e);
                }

                if (!e.Handled && this.AllowKeyboardSearch &&
          (keyData.ToString().Length == 1 || keyData == Keys.Space || keyData == Keys.Back ||
          ((Control.ModifierKeys & Keys.Shift) > 0 && (keyData & ~(Keys.ShiftKey | Keys.Shift)) > 0) ||
          (keyData.ToString().StartsWith("D") && keyData.ToString().Length == 2)))
                {
                    ProccessSearchKey(e);
                }
            }

            base.OnKeyDown(e);
        }

        private void ProcessSpaceKey(KeyEventArgs e)
        {
            Keys keyData = e.KeyData;
            Keys keyPressed = keyData & Keys.KeyCode;
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);

            if (this.MultiSelect && bCtrl)
            {
                if (m_activeNode != null)
                {
                    if (m_activeNode.IsSelected)
                    {
                        SetSelectedNode(null, m_activeNode, TreeViewAdvAction.ByKeyboard);
                    }
                    else
                    {
                        SetSelectedNode(m_activeNode, new ArrayList(), TreeViewAdvAction.ByKeyboard);
                    }

                    m_selectionBaseNode = m_activeNode;
                }
            }
            else // Space but not Ctrl
            {
                if (m_strKeySearch == "" || !this.AllowKeyboardSearch)
                {
                    CheckState newState = CheckState.Checked;
                    bool newStateKnown = false;

                    // Toggle the activeNode
                    this.IsKeyDown = true;

                    if (m_activeNode != null && m_activeNode.EnabledButtons)
                    {
                        m_activeNode.Checked = !m_activeNode.Checked;
                        newState = m_activeNode.CheckState;
                        newStateKnown = true;
                    }

                    // And also toggle the selected nodes, if any.
                    foreach (TreeNodeAdv node in this.SelectedNodes)
                    {
                        // Set the state based on the current activeNode's state.
                        if (node != m_activeNode && node.EnabledButtons)
                        {
                            if (newStateKnown)
                            {
                                node.CheckState = newState;
                            }
                            else
                            {
                                node.Checked = !node.Checked;
                                newState = node.CheckState;
                                newStateKnown = true;
                            }
                        }
                    }

                    this.IsKeyDown = false;
                }
            }
        }

        private void ProccessSearchKey(KeyEventArgs e)
        {
            Keys keyData = e.KeyData;

            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            m_keyInputTimer.Enabled = false;
            m_keyInputTimer.Enabled = true;

            if (keyData == Keys.Space && m_strKeySearch.Length > 0)
            {
                m_strKeySearch += " ";
            }

            if (keyData == Keys.Back)
            {
                if (m_strKeySearch.Length > 0)
                {
                    m_strKeySearch = m_strKeySearch.Substring(0, m_strKeySearch.Length - 1);
                }
            }

            if (keyData.ToString().StartsWith("D") && keyData.ToString().Length == 2)
            {
                m_strKeySearch += keyData.ToString().Substring(keyData.ToString().Length - 1, 1);
            }
            else if (keyData != Keys.Space && keyData != Keys.Back)
            {
                if (bShift)
                    m_strKeySearch += (keyData & ~e.Modifiers);
                else
                    m_strKeySearch += keyData.ToString();
            }

            FindNode(m_strKeySearch, bShift);
        }

        private void ProcessRightArrowKey(KeyEventArgs e)
        {
            if (m_activeNode != null /* && selectedNode.HasChildren*/ )
            {
                if (!m_activeNode.Expanded)
                {
                    m_activeNode.Expanded = true;
                }
                else
                {
                    TreeNodeAdv next = m_activeNode.NextSelectableNode;
                    if (next != null && next.Parent == m_activeNode)
                    {
                        TreeNodeAdv newNode = next;

                        if (SetSelectedNode(next, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                        {
                            this.ActiveNode = newNode;
                            m_selectionBaseNode = newNode;
                        }
                    }
                }
            }

            e.Handled = true;
        }

        private void ProcessLeftOrBackSpaceKey(KeyEventArgs e)
        {
            if (m_activeNode.Expanded && m_activeNode.HasNodes && e.KeyCode != Keys.Back)
            {
                m_activeNode.Expanded = false;
            }
            else
            {
                if (m_activeNode.Parent != null && m_activeNode.Parent != m_root &&
          m_activeNode.Parent.Enabled)
                {
                    TreeNodeAdv newNode = m_activeNode.Parent;

                    if (SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                    {
                        this.ActiveNode = newNode;
                        m_selectionBaseNode = newNode;
                    }
                }
            }

            e.Handled = true;
        }

        private void ProcessPageDownKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (this.SelectionMode == TreeSelectionMode.Single && this.SelectedNode == null);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            m_pressedKey = Keys.PageDown;
            this.SelectUpwardDirection = false;

            if (bCtrl)
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.LargeIncrement);

                TreeNodeAdv newNode = this.LastVisibleNode;

                // Move the active node.
                if (bIsValidSingleSelection || IsMultipleSelection(newNode))
                {
                    this.ActiveNode = newNode;
                    m_selectionBaseNode = newNode;
                }

                e.Handled = true;
            }
            else if (bShift)
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.Last);

                if (this.ActiveNode != null)
                {
                    BeginUpdate();

                    for (int i = NodeToRowIndex(this.ActiveNode); i <= this.VScrollBar.Maximum; i++)
                    {
                        TreeNodeAdv newNode = RowIndexToNode(i);

                        if (IsMultipleSelection(newNode))
                        {
                            ExtendSelectionTo(newNode);
                        }
                        else
                        {
                            SetNextNodeSelected(newNode);
                        }
                    }

                    TreeNodeAdv lastNode = RowIndexToNode(this.VScrollBar.Maximum);
                    lastNode.BringIntoView();
                    EndUpdate(false);
                }

                e.Handled = true;
            }
            else if (this.ActiveNode != null)
            {
                // If the node that is LargeChange distance away from the
                // current ActiveNode is selectable, do so.
                int curPos = NodeToRowIndex(this.ActiveNode);
                int newSelRow = curPos + this.VScrollBar.LargeChange - 1;
                if (newSelRow > this.VScrollBar.Maximum)
                {
                    newSelRow = this.VScrollBar.Maximum;
                }

                TreeNodeAdv newNode = RowIndexToNode(newSelRow);

                if (newNode.Enabled && SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                {
                    this.ActiveNode = newNode;
                    m_selectionBaseNode = newNode;
                }

                e.Handled = true;
            }
        }

        private void ProcessPageUpKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            this.SelectUpwardDirection = true;
            m_pressedKey = Keys.PageUp;

            if (bCtrl)
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.LargeDecrement);

                if (bIsValidSingleSelection)
                {
                    // Move the active node.
                    this.ActiveNode = this.TopVisibleNode;
                    m_selectionBaseNode = this.TopVisibleNode;
                }

                e.Handled = true;
            }
            else if (bShift)
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.First);

                if (this.ActiveNode != null)
                {
                    BeginUpdate();

                    for (int i = NodeToRowIndex(this.ActiveNode); i >= this.VScrollBar.Minimum; i--)
                    {
                        TreeNodeAdv newNode = RowIndexToNode(i);

                        if (IsMultipleSelection(newNode))
                        {
                            ExtendSelectionTo(newNode);
                        }
                        else
                        {
                            SetNextNodeSelected(newNode);
                        }
                    }

                    EndUpdate(false);

                    TreeNodeAdv firstNode = RowIndexToNode(this.VScrollBar.Minimum);
                    firstNode.BringIntoView();

                    RefreshVScrollbar(firstNode, firstNode.Bounds.Y);
                }

                e.Handled = true;
            }
            else if (this.ActiveNode != null)
            {
                // If the node that is LargeChange distance away from the
                // current ActiveNode is selectable, do so.
                int curPos = NodeToRowIndex(this.ActiveNode);
                int newSelRow = curPos - this.VScrollBar.LargeChange + 1;
                if (newSelRow < this.VScrollBar.Minimum)
                {
                    newSelRow = this.VScrollBar.Minimum;
                }

                TreeNodeAdv newNode = RowIndexToNode(newSelRow);

                if (newNode.Enabled && SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                {
                    this.ActiveNode = newNode;
                    m_selectionBaseNode = newNode;
                }

                e.Handled = true;
            }
        }


        private void ProcessHomeKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            this.SelectUpwardDirection = true;
            m_pressedKey = Keys.Home;
            if (bCtrl && !(bCtrl && bShift))
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.First);

                if (bIsValidSingleSelection)
                {
                    this.ActiveNode = RowIndexToNode(this.VScrollBar.Minimum);
                    m_selectionBaseNode = this.ActiveNode;
                }

                e.Handled = true;
            }
            else if (bShift || (bCtrl && bShift))
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.First);

                // Fix for the issue #1703: Shift-Home and Shift-End do not properly extend 
                // multi-selection with MultiColumnTreeView control.
                if (this.ActiveNode != null)
                {
                    BeginUpdate();

                    for (int i = NodeToRowIndex(this.ActiveNode); i >= this.VScrollBar.Minimum; i--)
                    {
                        TreeNodeAdv newNode = RowIndexToNode(i);

                        if (IsMultipleSelection(newNode))
                        {
                            ExtendSelectionTo(newNode);
                        }
                        else
                        {
                            SetNextNodeSelected(newNode);
                        }
                    }

                    EndUpdate(false);

                    // After selecting all the node upto first node of the tree, scroll 
                    // bar should go into the top of the tree
                    TreeNodeAdv firstNode = RowIndexToNode(this.VScrollBar.Minimum);
                    firstNode.BringIntoView();

                    RefreshVScrollbar(firstNode, firstNode.Bounds.Y);

                }
                e.Handled = true;
            }
            else if (this.ActiveNode != null)
            {
                TreeNodeAdv newNode = RowIndexToNode(this.VScrollBar.Minimum);

                if (newNode.Enabled && SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                {
                    this.ActiveNode = newNode;
                    m_selectionBaseNode = newNode;

                    // Fix for defect #1698: Root lines do not get painted consistently 
                    // in treeviewAdv using End key and Home key 
                    RefreshVScrollbar(newNode, newNode.Bounds.Y);
                }

                e.Handled = true;
            }
        }

        private void ProcessDownKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            m_pressedKey = Keys.Down;
            this.SelectUpwardDirection = false;

            if (m_activeNode.NextSelectableNode != null)
            {
                TreeNodeAdv newNode = this.ActiveNode.NextSelectableNode;

                if (bShift)
                {
                    //Fix for defect 1695: ScrollBar flashes ,when multiselection of nodes are done by holding down the shift key.
                    BeginUpdate();
                    if (IsMultipleSelection(newNode))
                    {
                        m_lastSelectedByKeyBoard = newNode;
                        ExtendSelectionTo(newNode);
                    }
                    else
                    {
                        SetNextNodeSelected(newNode);
                    }
                    EndUpdate(false);
                }
                else if (bCtrl)
                {
                    if (bIsValidSingleSelection || IsMultipleSelection(newNode))
                    {
                        //Fix for defect 1702: The treeViewAdv is not getting scrolled to keep the focus rectangle in sight .
                        this.ActiveNode = newNode;

                        if (ActiveNode == LastVisibleNode)
                        {
                            VScrollBar.SendScrollMessage(ScrollEventType.SmallIncrement);
                        }
                    }
                    else
                    {
                        VScrollBar.SendScrollMessage(ScrollEventType.SmallIncrement);
                    }
                }
                else
                {
                    SetNextNodeSelected(newNode);
                }
                //     Invalidate();
            }
            else
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.SmallIncrement);
            }

            e.Handled = true;
        }

        private void ProcessUpKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            m_pressedKey = Keys.Up;
            if (this.ActiveNode.PrevSelectableNode != null && this.ActiveNode.PrevSelectableNode != m_root)
            {
                TreeNodeAdv newNode = this.ActiveNode.PrevSelectableNode;
                this.SelectUpwardDirection = true;
                if (bShift)
                {
                    //Fix for defect 1695: ScrollBar flashes ,when multiselection of nodes are done by holding down the shift key.
                    BeginUpdate();

                    if (IsMultipleSelection(newNode))
                    {
                        m_lastSelectedByKeyBoard = newNode;
                        ExtendSelectionTo(newNode);
                    }
                    else
                    {
                        SetNextNodeSelected(newNode);
                    }

                    EndUpdate(false);
                }
                else if (bCtrl)
                {
                    if (bIsValidSingleSelection || IsMultipleSelection(newNode))
                    {
                        // Fix for defect 1700: Attempting to make a multi-selection 
                        // of tree nodes using the keyboard is pretty broken.
                        if (ActiveNode == TopVisibleNode)
                        {
                            VScrollBar.SendScrollMessage(ScrollEventType.SmallDecrement);
                        }

                        this.ActiveNode = newNode;
                    }
                    else
                    {
                        this.VScrollBar.SendScrollMessage(ScrollEventType.SmallDecrement);
                    }
                }
                else
                {
                    SetNextNodeSelected(newNode);
                }
            }
            else
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.SmallDecrement);
            }

            e.Handled = true;
        }


        private void ProcessEndKey(KeyEventArgs e)
        {
            bool bIsValidSingleSelection = (SelectedNode == null && SelectionMode == TreeSelectionMode.Single);
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) > 0);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) > 0);

            this.SelectUpwardDirection = false;
            m_pressedKey = Keys.End;

            if (bCtrl && !(bCtrl && bShift))
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.Last);

                if (bIsValidSingleSelection)
                {
                    this.ActiveNode = RowIndexToNode(this.VScrollBar.Maximum);
                    m_selectionBaseNode = this.ActiveNode;
                }

                e.Handled = true;
            }
            else if (bShift || (bCtrl && bShift))
            {
                this.VScrollBar.SendScrollMessage(ScrollEventType.Last);

                // Fix for the issue #1703: Shift-Home and Shift-End do not properly extend 
                // multi-selection with MultiColumnTreeView control.
                if (this.ActiveNode != null)
                {
                    this.VScrollBar.SendScrollMessage(ScrollEventType.Last);
                    TreeNodeAdv newNode = this.ActiveNode;
                    BeginUpdate();
                    for (int i = NodeToRowIndex(this.ActiveNode); i <= this.VScrollBar.Maximum; i++)
                    {
                        newNode = RowIndexToNode(i);

                        if (IsMultipleSelection(newNode))
                        {
                            ExtendSelectionTo(newNode);
                        }
                        else
                        {
                            SetNextNodeSelected(newNode);
                        }
                    }
                    TreeNodeAdv lastNode = RowIndexToNode(this.VScrollBar.Maximum);
                    lastNode.BringIntoView();
                    EndUpdate(false);
                }
                e.Handled = true;
            }
            else if (this.ActiveNode != null)
            {
                TreeNodeAdv newNode = RowIndexToNode(this.VScrollBar.Maximum);

                if (newNode.Enabled && SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                {
                    this.ActiveNode = newNode;
                    m_selectionBaseNode = newNode;
                }
                e.Handled = true;
            }
        }

        protected override bool IsInputChar(char charCode)
        {
            return true;
        }

        protected override void OnUpdatingChanged(EventArgs e)
        {
            if (this.Updating)
            {
                //this.measuringGraphics = this.CreateGraphics();
            }
            else
            {
                RefreshVScrollbar();
            }
        }

        /// <summary>
        /// Raises the ThemeChanged event.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <remarks><para>The OnThemeChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnThemeChanged in a derived
        /// class, be sure to call the base class's OnThemeChanged method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnThemeChanged(EventArgs e)
        {
            if (this.ThemeChanged != null)
            {
                this.ThemeChanged(this, e);
            }
        }

        protected override void OnScrollbarsVisibleChanged(EventArgs e)
        {
            if (!this.HorizontalScrollBar)
            {
                this.HScrollPos = 0;
            }

            base.OnScrollbarsVisibleChanged(e);
        }

        /// <summary>
        /// Raises the BorderStyleChanged event.
        /// </summary>
        /// <remarks><para>The OnBorderStyleChanged method also allows derived classes to handle the event
        /// without attaching a delegate. This is the preferred technique for
        /// handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBorderStyleChanged in a derived
        /// class, be sure to call the base class's OnBorderStyleChanged method so that
        /// registered delegates receive the event.</para></remarks>
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            if (BorderStyleChanged != null)
            {
                BorderStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the Border3DStyleChanged event.
        /// </summary>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorder3DStyleChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBorder3DStyleChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorder3DStyleChanged method so that registered 
        /// delegates receive the event.</para></remarks>
        protected virtual void OnBorder3DStyleChanged(EventArgs e)
        {
            if (Border3DStyleChanged != null)
            {
                Border3DStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the BorderSingleChanged event.
        /// </summary>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorderSingleChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBorderSingleChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorderSingleChanged method so that registered 
        /// delegates receive the event.</para></remarks>
        protected virtual void OnBorderSingleChanged(EventArgs e)
        {
            if (BorderSingleChanged != null)
            {
                BorderSingleChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the BorderColorChanged event.
        /// </summary>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorderColorChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnBorderColorChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorderColorChanged method so that registered 
        /// delegates receive the event.</para></remarks>
        protected virtual void OnBorderColorChanged(EventArgs e)
        {
            if (BorderColorChanged != null)
            {
                BorderColorChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the GradientBackgroundChanged event.
        /// </summary>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnGradientBackgroundChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OnGradientBackgroundChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnGradientBackgroundChanged method so that registered 
        /// delegates receive the event.</para></remarks>
        protected virtual void OnBackgroundColorChanged(EventArgs e)
        {
            if (BackgroundColorChanged != null)
            {
                BackgroundColorChanged(this, e);
            }
        }
        /// <summary>
        /// Raises the BorderSidesChanged event.
        /// </summary>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBorderSidesChanged method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para><para>Notes to Inheritors: When overriding OmBorderSidesChanged 
        /// in a derived class, be sure to call the base class's 
        /// OnBorderSidesChanged method so that registered 
        /// delegates receive the event.</para></remarks>
        protected virtual void OnBorderSidesChanged(EventArgs e)
        {
            if (BorderSidesChanged != null)
            {
                BorderSidesChanged(this, e);
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new TreeViewAdvAcessibleObject(this);
        }


        protected override void OnEnter(EventArgs arg)
        {
            // Choose a default selected node if none is selected.
            if (this.SelectedNode == null && !this.IsBroughtIntoView)
            {
                if (this.ShouldSelectNodeOnEnter || Focused)
                {
                    this.SetDefaultSelectionOnEnter();
                }
            }

            base.OnEnter(arg);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            InvalidateNodeCollection(this.SelectedNodes);
            InvalidateNodeCollection(m_hashHighlightedNodes.Values);

            base.OnGotFocus(e);
        }

 
        protected override void OnLostFocus(EventArgs e)
        {
            this.UpdateTips(false);

            for (int i = 0; i < this.SelectedNodes.Count; i++)
            {
                TreeNodeAdv tn = this.SelectedNodes[i];
                if (tn.Visible || this.DesignModeInternal)
                {
                    // Fix for defect #1694: MultiColumnTreeView control has incorrect painting/invalidation 
                    // when the control loses focus.
                    if (this.FullRowSelect)
                    {
                        Rectangle rect = tn.Bounds;

                        rect.Inflate(tn.Bounds.X, 0);
                        rect.Width -= tn.Bounds.X;
                        Invalidate(rect);

                    }
                    else
                    {
                        Invalidate(tn.Bounds);
                    }
                }
            }

            using (Graphics g = this.CreateGraphics())
            {
                // This change made by lucas in order to keep dotted rectangle
                // around selected nodes when control has no focus.
                if (KeepDottedSelection)
                {
                    for (int i = 0, len = SelectedNodes.Count; i < len; ++i)
                    {
                        TreeNodeAdv node = SelectedNodes[i];

                        if (node.Visible || this.DesignModeInternal)
                        {
                            node.DrawFocusRect(g);
                        }
                    }
                }
            }

            base.OnLostFocus(e);
        }

        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw any child controls.
        /// </summary>
        public override void Refresh()
        {
            if (refreshFlag)
                this.Root.RecalculateAllDimensions();
            else
                refreshFlag = true;
            base.Refresh();
        }
 
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            // Update scrollers information.
            if (this.HScrollBar != null)
            {
                this.HScrollBar.BeginUpdate();
                this.HScrollBar.FetchScrollBar();
                this.HScrollBar.EndUpdate();
            }
            if (this.VScrollBar != null)
            {
                this.VScrollBar.BeginUpdate();
                this.VScrollBar.FetchScrollBar();
                this.VScrollBar.EndUpdate();
            }
        }

        #endregion

        #region Class WndProc

        protected override CreateParams CreateParams
        {
            get
            {
                BorderStyle border = m_borderStyle;

                CreateParams cparams = base.CreateParams;
                cparams.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;
                cparams.ExStyle &= ~NativeMethods.WS_EX_CLIENTEDGE;
                cparams.Style &= ~NativeMethods.WS_BORDER;

                if (BorderStyle.Fixed3D == border)
                {
                    cparams.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;

                    if (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive &&
            XPThemes.IsAppThemed && this.ThemedBorder)
                    {
                        border = BorderStyle.FixedSingle;
                    }
                }

                if (BorderStyle.FixedSingle == border)
                {
                    cparams.Style |= NativeMethods.WS_BORDER;
                }

                return cparams;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            ControlStyles ctlstyles = ControlStyles.Selectable | ControlStyles.UserMouse |
        ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor |
        ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint;

            this.SetStyle(ctlstyles, true);
            this.UpdateStyles();
        }

        protected override void WndProc(ref Message m)
        {
            bool bCallBase = true;
            CheckOnColumnClicked(ref m);

            if (m_rgnCached != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(m_rgnCached);
                m_rgnCached = IntPtr.Zero;
            }

            if (m.Msg == NativeMethods.WM_THEMECHANGED)
            {
                InvalidateWindow();
            }
            else if (m.Msg == NativeMethods.WM_NCPAINT)
            {
                INonClientPaintingSupport supp = this;
                m_rgnCached = DrawingUtils.NCPaintHelper(this, supp, ref m);
            }
            else if (m.Msg == NativeMethods.WM_PRINT)
            {
                if (((int)m.LParam & PRF_NONCLIENT) != 0)
                {
                    using (Graphics g = Graphics.FromHdc(m.WParam))
                    {
                        NativeMethods.RECT rc = new NativeMethods.RECT();
                        if (NativeMethods.GetWindowRect((int)this.Handle, ref rc) != 0)
                        {
                            Rectangle rcWnd = new Rectangle(rc.left, rc.top, rc.Width, rc.Height);
                            Rectangle rcClip = new Rectangle(0, 0, rc.Width, rc.Height);

                            NCPaintEventArgs ncEventArgs = new NCPaintEventArgs(g, rcClip, rcClip, rcWnd, IntPtr.Zero);
                            OnNCPaint(ncEventArgs);
                        }
                    }
                }
            }
            else if (m.Msg == NativeMethods.WM_NCMOUSEMOVE)
            {
                WmNcMouseMove(ref m);
            }
            else if (m.Msg == NativeMethods.WM_CONTEXTMENU)
            {
                // LParam == -1 means that this is due to a keyboard message
                if ((int)m.LParam == -1)
                {
                    if (WmContextMenuByKey(ref m))
                    {
                        return;
                    }
                }
                else if (IsDraggedPastMouseDownPoint(PointToClient(Control.MousePosition)))
                {
                    return;
                } // without calling the base class.
            }
            else if (m.Msg == NativeMethods.WM_CAPTURECHANGED)
            {
                WmCaptureChanged(ref m);
            }
            else if (m.Msg == NativeMethods.WM_NCLBUTTONDOWN)
            {
                bCallBase = WmNcLButtonDown(ref m);
            }
            else if (m.Msg == NativeMethods.WM_LBUTTONUP)
            {
                if (this.ColumnsMovedMode)
                {
                    this.Capture = false;
                }
            }
            else if (m.Msg == NativeMethods.WM_NCHITTEST)
            {
                // if hit test processed by us then do not allow base.WndProc call
                bCallBase = !WmNcHitTest(ref m);
            }
            else if (m.Msg == NativeMethods.WM_GETDLGCODE)
            {
                m.Result = new IntPtr((int)(NativeMethods.DialogCodes.DLGC_WANTARROWS) | m.Result.ToInt32());
                bCallBase = false;
            }
            else if (m.Msg == NativeMethods.WM_LBUTTONDBLCLK && NativeMethods.MK_LBUTTON == (int)m.WParam)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                bool bToolTipClicked = CheckToolTipClicked(pt);

                if (bToolTipClicked)
                {
                    m.Result = IntPtr.Zero;
                    OnDoubleClick(EventArgs.Empty);

                    // Fix for the defect MouseDoubleClick event does not trigger when we double click on ToolTip
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    OnMouseDoubleClick(new MouseEventArgs(MouseButtons.Left, 2, MousePosition.X, MousePosition.Y, 0));
#endif
                    bCallBase = false;
                }
            }

            if (bCallBase)
            {
                base.WndProc(ref m);
            }
        }

        private void CheckOnColumnClicked(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCMBUTTONDOWN
        || m.Msg == NativeMethods.WM_NCLBUTTONDOWN
        || m.Msg == NativeMethods.WM_NCRBUTTONDOWN)
            {
                WmNcButtonDown(ref m);
            }
            else if (m.Msg == NativeMethods.WM_NCMBUTTONDBLCLK
        || m.Msg == NativeMethods.WM_NCLBUTTONDBLCLK
        || m.Msg == NativeMethods.WM_NCRBUTTONDBLCLK)
            {
                WmNcButtonDblClk(ref m);
            }
            else if (m.Msg == NativeMethods.WM_NCMBUTTONUP
        || m.Msg == NativeMethods.WM_NCLBUTTONUP
        || m.Msg == NativeMethods.WM_NCRBUTTONUP)
            {
                WmNcButtonUp(ref m);
            }
        }

        private void WmNcButtonDblClk(ref Message m)
        {
            if (this.ShowColumnsHeader && this.HasColumns)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                TreeColumnAdv column = GetColumnByScreenPoint(pt);

                if (null != column)
                {
                    OnColumnDoubleClick(column);
                    m_boolean[(int)InternalBooleanFields.IsDoubleClick] = true;
                }
            }
        }

        private bool WmNcHitTest(ref Message m)
        {
            bool result = false;

            if (this.ShowColumnsHeader)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                pt.Offset(-m_iNcScreenOffsetX, -m_iNcScreenOffsetY);
                bool cursorInHeaderZone = false;

                if (this.HasColumns)
                {
                    foreach (TreeColumnAdv column in this.Columns.VisibleColumns)
                    {
                        bool selected = column.Bounds.Contains(pt);
                        bool lastValue = column.Highlighted;
                        column.Highlighted = selected;

                        if (selected && !lastValue)
                        {
                            OnColumnSelected(new TreeViewColumnSelectedChangedEventArgs(column));
                        }

                        if (column.Highlighted)
                        {
                            cursorInHeaderZone = true;
                        }
                    }
                }

                if (cursorInHeaderZone || (pt.Y <= m_headerHeight))
                {
                    m.Result = (IntPtr)NativeMethods.HTBORDER;
                    result = true;
                }
            }

            return result;
        }

        private bool WmContextMenuByKey(ref Message m)
        {
            // (This workaround only helps .Net ContextMenus)

            // If showing context menu by keyboard then
            // override the default behavior (which is to show the menu in the middle of the control)
            // and show the context menu beside the selected node
            if (this.ContextMenu != null && this.SelectedNode != null)
            {
                Point pt = ((IProvideCustomContextMenuPositionalInformation)this).GetMenuPositionForKeyboardInvoke();
                this.RMouseDownNode = this.SelectedNode;
                this.ContextMenu.Show(this, pt);
                this.RMouseDownNode = null;
                return true;
            }

            return false;
        }

        private void WmCaptureChanged(ref Message m)
        {
            if (this.ColumnsMovedMode)
            {
                bool mirrored = GetIsMirrored();
                TreeColumnAdv column = (this.Columns.VisibleColumns.Length > m_iColumnsPressedIndex) ?
          this.Columns.VisibleColumns[m_iColumnsPressedIndex] : null;

                // disable mode flag.
                this.ColumnsMovedMode = false;

                // initializing helper variables.
                Point center = PointToScreen(Point.Empty);
                int sign = (mirrored) ? -1 : 1;

                // clears last drawed line.
                DrawReversibleLine(sign * m_iColumnsOffset);

                if (null != column)
                {
                    // changes moved column width.
                    column.Width += m_iColumnsOffset;
                    OnColumnResized(column, column.Width - m_iColumnsOffset, column.Width);
                }

                if (mirrored)
                {
                    // updates horizontal scroller.
                    RefreshHScrollbar();

                    // changed scroller position relatively column offset.
                    this.HScrollBar.Value += Math.Min(m_iColumnsOffset,
                          this.HScrollBar.Maximum - this.HScrollBar.Minimum - this.HScrollBar.LargeChange + 1);
                }

                // reset column offset.
                m_iColumnsOffset = 0;
            }
        }


        private void WmNcMouseMove(ref Message m)
        {
            // Checks if cursor on resized zone than change cursor type for tree.
            if (ShowColumnsHeader && HasColumns)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                pt.Offset(-m_iNcScreenOffsetX, -m_iNcScreenOffsetY);
                int rightBorder = 0;
                int leftBorder = 0;

                TreeColumnAdv[] columns = this.Columns.VisibleColumns;

                if (columns.Length > 0)
                {
                    TreeColumnAdv colFirst = columns[0];

                    if (colFirst.Bounds.Y <= pt.Y && colFirst.Bounds.Bottom >= pt.Y)
                    {
                        for (int i = 0, len = columns.Length; i < len; i++)
                        {
                            TreeColumnAdv columnVis = columns[i];
                            TreeColumnAdv columnVisNext = (i + 1 != len) ? columns[i + 1] : null;

                            if (GetIsMirrored())
                            {
                                rightBorder = columnVis.Bounds.X + (Math.Min(2, columnVis.Bounds.Width));
                                leftBorder = ((i + 1 != len)) ? columnVisNext.Bounds.X + columnVisNext.Width - 2 : columnVis.Bounds.X - 2;
                            }
                            else
                            {
                                rightBorder = ((i + 1 != len)) ? columnVisNext.Bounds.X : columnVis.Bounds.X + columnVis.Bounds.Width;
                                leftBorder = columnVis.Bounds.X + columnVis.Bounds.Width - (Math.Min(2, columnVis.Bounds.Width));
                            }

                            if (rightBorder >= pt.X && leftBorder <= pt.X)
                            {
                                Cursor.Current = Cursors.VSplit;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private bool WmNcLButtonDown(ref Message m)
        {
            Point p = PointToClient(new Point(MousePosition.X, MousePosition.Y));

            if (this.IsEditing && GetNodeAtPoint(p) != null)
            {
                EndEdit(false);

                // If the edit wasn't committed don't let the control process this message.
                if (this.IsEditing)
                {
                    return false;
                }
            }

            if (this.ShowColumnsHeader && this.HasColumns)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));

                // translate point to client
                pt.Offset(-m_iNcScreenOffsetX, -m_iNcScreenOffsetY);

                // initializing helper variables
                int rightBorder;
                int leftBorder;
                TreeColumnAdv[] columns = this.Columns.VisibleColumns;

                if (columns.Length > 0)
                {
                    TreeColumnAdv colFirst = columns[0];

                    // Checks vertical position
                    if (colFirst.Bounds.Y <= pt.Y && colFirst.Bounds.Bottom >= pt.Y)
                    {
                        for (int i = 0, len = columns.Length; i < len; i++)
                        {
                            // calculates resized zone for column.
                            TreeColumnAdv columnVis = columns[i];
                            TreeColumnAdv columnVisNext = (i + 1 != len) ? columns[i + 1] : null;

                            if (GetIsMirrored())
                            {
                                rightBorder = columnVis.Bounds.X + (Math.Min(2, columnVis.Bounds.Width));
                                leftBorder = ((i + 1 != len)) ? columnVisNext.Bounds.X + columnVisNext.Width - 2 : columnVis.Bounds.X - 2;
                            }
                            else
                            {
                                rightBorder = (i + 1 != len) ? columnVisNext.Bounds.X + 2 : columnVis.Bounds.X + columnVis.Bounds.Width;
                                leftBorder = columnVis.Bounds.X + columnVis.Bounds.Width - (Math.Min(2, columnVis.Bounds.Width));
                            }

                            // if point in resized zone than begin drag
                            if (rightBorder >= pt.X && leftBorder <= pt.X)
                            {
                                this.Capture = true;
                                m_iColumnsPressedIndex = i;
                                m_iColumnsPressedPosition = pt.X + m_iNcScreenOffsetX;
                                Point center = PointToScreen(Point.Empty);

                                DrawReversibleLine(0);

                                this.ColumnsMovedMode = true;
                                Cursor.Current = Cursors.VSplit;
                                break;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void WmNcButtonDown(ref Message m)
        {
            if (this.ShowColumnsHeader && this.HasColumns)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                m_pressedColumn = GetColumnByScreenPoint(pt);
            }
        }

        private void WmNcButtonUp(ref Message m)
        {
            if (this.ShowColumnsHeader && this.HasColumns)
            {
                Point pt = new Point(NativeMethods.LOWORD(m.LParam), NativeMethods.HIWORD(m.LParam));
                TreeColumnAdv column = GetColumnByScreenPoint(pt);

                if (column == m_pressedColumn && !m_boolean[(int)InternalBooleanFields.IsDoubleClick])
                {
                    OnColumnClick(column);
                }

                m_boolean[(int)InternalBooleanFields.IsDoubleClick] = false;
            }
        }

        /// <summary>
        /// Draws reversible line.
        /// </summary>
        /// <param name="offset"> Horizontal offset relatively client rectangle point (0,0)</param>
        private void DrawReversibleLine(int offset)
        {
            Point center = PointToScreen(Point.Empty);

            Rectangle rect = new Rectangle(m_iColumnsPressedPosition + offset
                                          , center.Y
                                          , 1
                                          , ClientRectangle.Height);

            ControlPaint.DrawReversibleFrame(rect, Color.Gray, FrameStyle.Thick);
        }

        /// <summary>
        /// Gets column by mouse position
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private TreeColumnAdv GetColumnByScreenPoint(Point pt)
        {
            /* Fix for the Issue SD 5364 */
            this.Refresh();
            // translate point to client
            pt.Offset(-m_iNcScreenOffsetX, -m_iNcScreenOffsetY);

            // finding column
            foreach (TreeColumnAdv column in Columns)
            {
                if (column.Bounds.Contains(pt))
                {
                    return column;
                }
            }

            return null;
        }

        protected override void OnNcCalcSize(ref Rectangle client)
        {
            if (this.ShowColumnsHeader)
            {
                client.Y += m_headerHeight;
                client.Height -= m_headerHeight;
            }

            base.OnNcCalcSize(ref client);
        }
        /// <summary>
        ///  Method draws non-client area of an MultiColumnTreeView.
        /// </summary>
        /// <param name="ncEventArgs"> EventArgs with Non-client info for drawing. </param>
        protected override void OnNCPaint(NCPaintEventArgs ncEventArgs)
        {
            base.OnNCPaint(ncEventArgs);

            // double buffering for drawing required for flickering problem solving.
            Rectangle rcInScreen = ncEventArgs.WindowInScreenRectangle;
            if (rcInScreen.Width <= 0 || rcInScreen.Height <= 0)
            {
                return;
            }

            Bitmap bmp=null;
            Graphics g=null;
            if (this.useDefaultDrawing)
            {
                g = ncEventArgs.Graphics;
            }
            else
            {
                bmp = new Bitmap(rcInScreen.Width, rcInScreen.Height, ncEventArgs.Graphics);
                g = Graphics.FromImage(bmp);
            }

            Rectangle bounds = ncEventArgs.DisplayRectangle;
            m_iNcScreenOffsetX = rcInScreen.X;
            m_iNcScreenOffsetY = rcInScreen.Y;

            // This is not good for the following reasons:
            // 1) When dragging a hidden tree into the visible desktop range, clipping
            // is not proper and as a result the BG is drawn over the whole control and stays there!
            // 2) Even in other scenarios, we can see the BG color being drawn first
            // followed by the tree. Causing a flicker effect.
            //
            // So, instead fill the bg only in the border area.
            // 
            // Without this some 3D styles (SunkenOuter) will leave a 1 pixel transparent area.
            //g.FillRectangle(new SolidBrush(this.BackColor),bounds);

            bool bThemed = (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive &&
                XPThemes.IsAppThemed && this.ThemedBorder);

            int w = GetBordersOffset();
            int iOffset = (bThemed && BorderStyle == BorderStyle.Fixed3D ? FIXED3D_BORDER_WIDTH : 0);

            int headerOffset = DrawHeader(g, bounds, w + iOffset);
            // The borders as 4 rectangles
            Rectangle[] clipRects = new Rectangle[]
			{
				new Rectangle( bounds.Location, new Size( w, bounds.Height ) ),
				new Rectangle( bounds.Location, new Size( bounds.Width, w ) ),
				new Rectangle( bounds.Width - w, bounds.Y, w, bounds.Height ),
				new Rectangle( bounds.X, bounds.Height - w, bounds.Width, w )
			};

            if (bThemed)
            {
                for (int i = 0; i < 4; i++)
                {
                    ThemedDrawing.Edit.DrawThemeBackground(g, 1, 1, bounds, clipRects[i]);
                }
            }
            else
            {
                using (Brush backBrush = new SolidBrush(this.BackColor))
                {
                    // Fill the border-rectangles with the bg brush, since some of the 
                    // 3d border types are only 1 pixel wide.
                    for (int i = 0; i < 4; i++)
                    {
                        g.FillRectangle(backBrush, clipRects[i]);
                    }
                }

                if (this.BorderSides != Border3DSide.All)
                {
                    if (this.BorderSides != Border3DSide.Middle)
                    {
                        ControlDrawing cd = new ControlDrawing();

                        cd.DrawBorder(g, bounds, this.BorderStyle, this.Border3DStyle,
                            this.BorderSingle, this.BorderColor, this.BorderSides);
                    }
                }
                else
                {
                    ControlDrawing cd = new ControlDrawing();

                    cd.DrawBorder(g, bounds, this.BorderStyle, this.Border3DStyle,
                        this.BorderSingle, this.BorderColor);
                }
            }

            if (!this.UseDefaultDrawing)
            {
                // draw double buffered header
                ncEventArgs.Graphics.DrawImage(bmp, 0, 0);

                g.Dispose();
                bmp.Dispose();
            }

            if (ncEventArgs.ClipRegion != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(ncEventArgs.ClipRegion);
            }

            // return a region excluding where you just drew. 
            ncEventArgs.ClipRegion = NativeMethods.CreateRectRgn(rcInScreen.Left + w,
                rcInScreen.Top + w + (!(bThemed && BorderStyle == BorderStyle.Fixed3D) ? headerOffset : 0),
                rcInScreen.Right - w,
                rcInScreen.Bottom - w);
        }
        #endregion

        #region Class Paint/Print Logic

        protected void InvalidateWindow()
        {
            if (this.IsHandleCreated)
            {
                const int redrawFlags = NativeMethods.RDW_FRAME |
                NativeMethods.RDW_INVALIDATE;

                NativeMethodsHelper.RedrawWindow(this.Handle, redrawFlags);
            }
        }

        /// <summary>
        /// Invalidates nodes in collection.
        /// </summary>
        /// <param name="col">Collection to invalidate.</param>
        protected void InvalidateNodeCollection(ICollection col)
        {
            foreach (TreeNodeAdv node in col)
            {
                if (node.Visible || this.DesignModeInternal)
                {
                    Rectangle invRect = new Rectangle(
                      this.TreeAllColumnsRectangle.X,
                      node.Bounds.Y,
                      this.TreeAllColumnsRectangle.Width,
                      node.Bounds.Height);

                    Invalidate(invRect);
                }
            }
        }
        /// <summary></summary>
        /// <param name="e">The PaintEventArgs using this to draw the non client area.</param>
        /// <param name="displayRect">The control's window bounds into which to draw. Left and Top are usually zero.</param>
        /// <param name="rcInScreen">The control's bounds in screen co-ordinates.</param>
        /// <returns>HRgn (as IntPtr) that excludes the region you just drew in the displayRect.</returns>
        IntPtr INonClientPaintingSupport.NonClientPaint(PaintEventArgs e, Rectangle displayRect, Rectangle rcInScreen)
        {
            NCPaintEventArgs ncEventArgs = new NCPaintEventArgs(e.Graphics, e.ClipRectangle, displayRect, rcInScreen, IntPtr.Zero);
            OnNCPaint(ncEventArgs);

            return ncEventArgs.ClipRegion;
        }

        /// <summary>Method draw header of the tree view </summary>
        /// <param name="g">Graphics on which we should draw columns.</param>
        /// <param name="bounds">Control bounds.</param>
        /// <param name="bordersOffset">area reserved for borders</param>
        /// <returns>Height of header.</returns>
        protected virtual int DrawHeader(Graphics g, Rectangle bounds, int bordersOffset)
        {
            int topOffset = 0;

            bool bMirror = this.GetIsMirrored();
            int direction = (bMirror) ? -1 : 1;
            int xOffset = ((bMirror) ? bounds.Right : bounds.X) + direction * bordersOffset;

            // paint caption here
            if (this.ShowColumnsHeader)
            {
                topOffset = m_headerHeight;

                // fill header area
                BrushPaint.FillRectangle(g, new Rectangle(bounds.X + bordersOffset, bounds.Y + bordersOffset,
                  bounds.Width - bordersOffset * 2, topOffset), m_HeaderBackground);
            }

            // if we have columns draw them
            if (this.HasColumns)
            {
                if (bMirror)
                {
                    if (HorizontalScrollBar)
                    {
                        RefreshHScrollbar();
                    }
                    xOffset += this.HScrollBar.Maximum - this.HScrollBar.Minimum
                     - this.HScrollBar.LargeChange - this.HScrollPos + 1;
                    xOffset -= (VerticallScrollBar && bMirror) ? SystemInformation.VerticalScrollBarWidth : 0;
                }
                else
                {
                    xOffset -= this.HScrollPos;
                }

                // draw columns headers
                foreach (TreeColumnAdv column in this.Columns.VisibleColumns)
                {
                    // skip column drawing if it not visible
                    if (bMirror)
                    {
                        xOffset -= column.Width;
                    }


                    if (this.ShowColumnsHeader)
                    {
                        column.Draw(g, xOffset += (VerticallScrollBar && bMirror) ? SystemInformation.VerticalScrollBarWidth: 0, bounds.Y + bordersOffset);
                    }
                    // update column bounds
                    column.UpdateColumnLocation(xOffset -= (VerticallScrollBar && bMirror) ? SystemInformation.VerticalScrollBarWidth: 0, bounds.Y + bordersOffset);
                    if (!bMirror)
                    {
                        xOffset += column.Width;
                    }
                }
            }

            return topOffset;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawBackground(e);
            Draw(e);

            /* Fix for the Issue SD 5364 */
            Point newOffset = this.PointToScreen(this.ClientRectangle.Location);
            m_iNcScreenOffsetX = newOffset.X;
            m_iNcScreenOffsetY = newOffset.Y - this.HeaderHeight - FIXED3D_BORDER_WIDTH - GetBordersOffset();

            if (this.NeedUpdateEditTop)
            {
                this.NeedUpdateEditTop = false;

                if (m_labelEditor.Visible && m_activeNode != null
                    && m_labelEditor.Top != m_activeNode.Bounds.Top)
                {
                    m_labelEditor.Top = m_activeNode.Bounds.Top;
                }
            }
        }

        [DocumentationExclude()]
        protected virtual void DrawBackground(PaintEventArgs e)
        {
            Rectangle rcClient = this.ClientRectangle;

            if (rcClient.Height <= 0 || rcClient.Width <= 0)
            {
                return;
            }

            // draw general control backgrounds
            bool bXpTheme = (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed);

            if (!(ThemesEnabled && bXpTheme) || IgnoreThemeBackground)
            {
                if (this.BackgroundColor != BrushInfo.Empty)
                {
                    if (this.IsHorizontalGradient)
                    {
                        int left = this.HorizontalScrollBar ? -this.HScrollPos : 0;
                        int top = 0;
                        int width = 0;

                        // Fix for the defect #2532 - Increase the width amount while Horizontal Brushing.
                        width = this.HScrollBar.Maximum + this.Width;

                        int height = rcClient.Height;
                        rcClient = new Rectangle(left, top, width, height);
                    }

                    BrushPaint.FillRectangle(e.Graphics, rcClient, this.BackgroundColor);
                }
            }
            else
            {
                ThemedDrawing.Edit.DrawThemeBackground(e.Graphics, 1, this.Enabled ? 1 : 4,
                  new Rectangle(-2, -2, rcClient.Width + 4, Height + 4));
            }

            DrawBackgroundColumns(e);
        }


        [DocumentationExclude()]
        protected virtual void DrawBackgroundColumns(PaintEventArgs e)
        {
            if (this.HasColumns)
            {
                // draw columns area backgrounds
                Rectangle rcClient = this.ClientRectangle;
                bool bMirror = this.GetIsMirrored();
                int xOffset = ((bMirror) ? rcClient.Right : rcClient.X);
                xOffset += ((bMirror) ? 1 : -1) * this.HScrollPos;
                foreach (TreeColumnAdv column in this.Columns.VisibleColumns)
                {
                    if (bMirror)
                    {
                        xOffset = column.Bounds.X;
                        //xOffset -= column.Width;
                    }

                    if (column.AreaBackground != BrushInfo.Empty)
                    {
                        Rectangle rcArea = new Rectangle(xOffset, rcClient.Y, column.Width, rcClient.Height);
                        using (Region clip = e.Graphics.Clip)
                        {
                            if (clip.IsVisible(rcArea))
                            {
                                BrushPaint.FillRectangle(e.Graphics, rcArea, column.AreaBackground);
                            }
                        }
                    }

                    if (!bMirror)
                    {
                        xOffset += column.Width;
                    }
                }
            }
        }

        [DocumentationExclude()]
        protected virtual void Draw(PaintEventArgs e)
        {
            if (null != this.Root)
            {
                if (!this.Root.HasNodes)
                {
                    this.VerticallScrollBar = false;
                    if (!m_bScrollersRefreshed)
                    {
                        RefreshVScrollbar();
                        RefreshHScrollbar();
                        m_bScrollersRefreshed = true;
                    }
                    return;
                }
                else
                {
                    m_bScrollersRefreshed = false;
                }

                TreeNodeAdv currentNode = PointToNode(this.ClientRectangle.Location);
                if (currentNode == null)
                {
                    return;
                }

                Point point = PointToClient(Control.MousePosition);
                bool bLMousePressed = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
                int y = NodeToPoint(currentNode).Y;
                int firstVisibleNodeRowIndex = NodeToRowIndex(currentNode);
                int height = this.ClientRectangle.Bottom;
                RefreshVScrollbar(currentNode, NodeToPoint(currentNode).Y);
                RefreshHScrollbar();

                if (currentNode != null)
                {
                    TreeNodeAdv lastVisibleNode = this.LastVisibleNode;

                    // Draw nodes visible to user
                    TreeNodeAdv node = currentNode;
                    ArrayList drawedNodes = new ArrayList();

                    while (y < height && node != null)
                    {
                        if (this.NeedUpdateCustomControls)
                        {
                            drawedNodes.Add(node);
                        }

                        // draws if tree hasn't columns or first column is visible.
                        if (!this.HasColumns || this.Columns[0].Visible)
                        {
                            // creates clip region for node
                            Region old = e.Graphics.Clip;
                            Rectangle rectangle = TreeColumnRectangle;

                            if (GetIsMirrored() && FullRowSelect && VerticallScrollBar)
                            {
                                rectangle.X = this.Columns[0].Bounds.X;
                            }
                            
                            Region clipRgn = new Region(rectangle);
                            e.Graphics.Clip = clipRgn;

                            if (node == SelectionBaseNode)
                                NeedUpdateBounds = false;

                            // first background then item
                            DrawNode(e.Graphics, e.ClipRectangle, node, y, Point.Empty, false, true);
                            DrawNode(e.Graphics, e.ClipRectangle, node, y, point, bLMousePressed, false);

                            e.Graphics.Clip = old;
                            // free memory
                            clipRgn.Dispose();
                        }
                        DrawNodeSubItems(e.Graphics, node, y);

                        y += node.Height;

                        node = node.NextVisibleNode;
                    }

                    if (this.ShowLines)
                    {
                        if (lastVisibleNode != null && !this.HasColumns || this.Columns[0].Visible)
                        {
                            // creates clip region for node
                            Region old = e.Graphics.Clip;
                            Region clipRgn = new Region(TreeColumnRectangle);
                            clipRgn.Intersect(ExcludedDrawingRegion);
                            e.Graphics.Clip = clipRgn;

                            DrawVerticalLines(e.Graphics, currentNode, this.Root, 0, lastVisibleNode);

                            ExcludedDrawingRegion.MakeInfinite();
                            e.Graphics.Clip = old;
                            // free memory
                            clipRgn.Dispose();
                        }
                    }

                    UpdateCustomControls(e, drawedNodes);

                    if (NeedUpdateBounds && SelectionBaseNode!=null)
                    {
                        if (firstVisibleNodeRowIndex > NodeToRowIndex(SelectionBaseNode))
                            SelectionBaseNode.SetBounds(new Rectangle(SelectionBaseNode.NodeX, 0, SelectionBaseNode.Width, SelectionBaseNode.Height));
                        else
                            SelectionBaseNode.SetBounds(new Rectangle(SelectionBaseNode.NodeX, y, SelectionBaseNode.Width, SelectionBaseNode.Height));
                    }

                }
            }
        }

   
        protected virtual void DrawNode(Graphics g, Rectangle clip, TreeNodeAdv node,
          int y, Point mousePos, bool mouseDown, bool background)
        {
            const int spc = 3;

            int nNodeWidth = this.ClientRectangle.Width - m_gutterSpace + this.HScrollBar.Value;
            int nNodeX = 0;
            bool bIsMirrored = GetIsMirrored();

            if (bIsMirrored)
            {
                nNodeX = MirrorHorizontalPosition(m_gutterSpace, nNodeWidth);
            }
            else
            {
                nNodeX = m_gutterSpace - this.HScrollBar.Value;
            }

            node.SetBounds(new Rectangle(nNodeX, y, nNodeWidth, node.Height));

            bool selected = false;
            bool active = false;
            bool hotTrack = false;

            if (!this.PreparingDragCueBitmap)
            {
                selected = (m_selectedNodes.Contains(node)) && (!this.HideSelection || this.Focused);
                active = (node == m_activeNode);

                hotTrack = (this.HotTracking && !this.MouseLeaved &&
          node.TextAndImageBounds.Contains(PointToClient(Control.MousePosition)) &&
          !this.Dragging);
            }

            int nNodeLeft = node.Bounds.X;
            int nNodeWidth1 = spc + node.Level * this.Indent + node.Width;

            if (bIsMirrored)
            {
                nNodeLeft = MirrorHorizontalPosition(nNodeLeft, nNodeWidth1);
            }

            int nNodeRight = nNodeLeft + nNodeWidth1;

            TreeNodeAdv mouseDownNode = GetMouseDownNode();

            /*  // this is unnecessary code
             // Highlighting node
             if( node.OptionedChild != null && !node.OptionedChild.IsVisible )
             {
           selected = m_hashHighlightedNodes.ContainsValue( node );
             } */

            if (background)
            {
                DrawNodeBackground(g, node, y, mouseDownNode, clip, active, selected, hotTrack);
            }
            else
            {
                DrawNodeInternal(g, node, mouseDownNode, clip, active, selected, hotTrack,
                  mouseDown, mousePos, nNodeLeft, nNodeRight);
            }
        }


        protected virtual void DrawNodeSubItems(Graphics g, TreeNodeAdv node, int offsetY)
        {
            bool bMirror = GetIsMirrored();

            if (this.HasColumns)
            {
                int w = GetBordersOffset();
                int scrollOffset = (VerticallScrollBar && bMirror) ? SystemInformation.VerticalScrollBarWidth : 0;
                TreeNodeAdv mouseDownNode = GetMouseDownNode();
                Rectangle rcTreeColumn = this.Columns[0].Bounds;
                int offsetX = ((bMirror) ? rcTreeColumn.X : rcTreeColumn.Right) - w - scrollOffset;
                Rectangle selectionBounds = GetSelectionBounds(offsetY, bMirror, node);

                if (!this.Columns[0].Visible)
                {
                    offsetX = (bMirror) ? this.ClientRectangle.Width : 0;
                }

                // NOTE: first column is always used for tree hierachy drawing
                for (int i = 1, len = this.Columns.Count; i < len; i++)
                {
                    TreeColumnAdv column = this.Columns[i];

                    if (column.Visible)
                    {
                        if (bMirror)
                        {
                            offsetX = column.Bounds.X;
                            //offsetX -= column.Width;
                        }

                        if (i < node.SubItems.Count && node.SubItems[i].Visible)
                        {
                            TreeNodeAdvSubItem subitem = node.SubItems[i];

                            if (g.Clip.IsVisible(offsetX, offsetY, column.Width, subitem.Height))
                            {
                                subitem.Draw(g, offsetX, offsetY, mouseDownNode, selectionBounds);
                            }
                        }
                        // If node is selected and FullRowSelected mode then draw background for current column's cell
                        else if (FullRowSelect)
                        {
                            Rectangle bounds = new Rectangle(offsetX, offsetY, column.Width, node.Height);
                            DrawBackgroundForEmptyColumnCell(g, selectionBounds, bounds, node, mouseDownNode);
                        }

                        if (!bMirror)
                        {
                            offsetX += column.Width;
                        }
                    }
                }

                if (FullRowSelect && node == this.ActiveNode)
                {
                    // if fullRowSelect mode and current node is activated than draw focus rectangle
                    ControlPaint.DrawFocusRectangle(g, selectionBounds, node.TextColor, node.Background.BackColor);
                }
            }
        }

        /// <summary>
        /// Fills empty cell for column
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        protected virtual void DrawBackgroundForEmptyColumnCell(Graphics g, Rectangle selectionBounds, Rectangle bounds, TreeNodeAdv node, TreeNodeAdv mouseDownNode)
        {
            BrushInfo bi = BrushInfo.Empty;
            bool selected = (this.SelectedNodes.Contains(node) && (!HideSelection || Focused));

            if (selected && (mouseDownNode == null ||
        this.SelectedNodes.Contains(mouseDownNode)))
            {
                if (Focused)
                {
                    bi = SelectedNodeBackground.Clone();
                }
                else if (!HideSelection)
                {
                    bi = InactiveSelectedNodeBackground.Clone();
                }
            }
            else // The owner draw handler should know that this is drawn unselected.
            {
                selected = false;
            }

            if (node == mouseDownNode)
            {
                selected = true;
                bi = SelectedNodeBackground.Clone();
            }

            if (selected) // draws only if parent node is selected
            {
                Region old = g.Clip; //saving old clip region

                using (Region clipRgn = new Region(bounds))
                {
                    clipRgn.Intersect(old);
                    g.Clip = clipRgn;
                   
                    BrushPaint.FillRectangle(g, selectionBounds, bi);
                }

                g.Clip = old; // revert region
            }
        }


        protected virtual void DrawNodeInternal(Graphics g, TreeNodeAdv node, TreeNodeAdv mouseDownNode,
          Rectangle clip, bool active, bool selected, bool hotTrack, bool mouseDown,
          Point mousePos, int nNodeLeft, int nNodeRight)
        {
            int scrollOffset = (GetIsMirrored()) ? this.HScrollPos : 0;

            if (active || this.OwnerDrawNodes ||
        !((nNodeLeft > clip.Left + scrollOffset && nNodeLeft > clip.Right + scrollOffset) ||
        (nNodeRight < clip.Left + scrollOffset && nNodeRight < clip.Right + scrollOffset)))
            {
                if (selected)
                {
                    if (mouseDownNode != null)
                    {
                        // If the mouseDownNode is a selected node, then draw the selected nodes.
                        if (!m_selectedNodes.Contains(mouseDownNode))
                        {
                            selected = false;
                        }
                    }
                }

                // draw it selected
                if (node == mouseDownNode)
                {
                    selected = true;
                }

                // Call the BeforeNodePaint event
                bool bFlag = (node == m_highlightedNode) || selected;

                if (!selected)
                {
                    if (m_highlightedNode != null)
                    {
                        if (m_highlightedNode.Nodes.Count == 0)
                        {
                            bFlag = false;
                        }
                    }
                    else
                    {
                        bFlag = false;
                    }
                }

                Point point = new Point(node.TextBounds.Left, node.TextBounds.Y);
                Color c = node.GetForeColor(selected, hotTrack);
                int level = node.Level - 1;

                TreeNodeAdvPaintEventArgs e = new TreeNodeAdvPaintEventArgs(
                  node, g, node.Bounds, point, level, m_parentIndent,
                  bFlag, active, this.FullRowSelect, hotTrack, c);

                if (m_paintFilter == null || !m_paintFilter.OnBeforeNodePaint(e))
                {
                    if (this.OwnerDrawNodes)
                    {
                        OnBeforeNodePaint(e);
                    }
                }

                if (!e.Handled)
                {
                    node.Draw(ThemedDrawing.TreeView, ThemedDrawing.Button, m_penLine, mousePos, mouseDown, e);
                }

                if (this.OwnerDrawNodes)
                {
                    OnAfterNodePaint(e);
                }

                if (m_paintFilter != null)
                {
                    m_paintFilter.OnAfterNodePaint(e);
                }
            }
        }


        protected virtual void DrawNodeBackground(Graphics g, TreeNodeAdv node, int offsetY, TreeNodeAdv mouseDownNode,
          Rectangle clip, bool active, bool selected, bool hotTrack)
        {
            // Also, assuming this is the case in ITreeNodeAdvPaintFilter.OnNodeBackgroundPaint
            BrushInfo bi = BrushInfo.Empty;

            // If the mouseDownNode is a selected node, then draw the selected nodes.
            if (selected && (mouseDownNode == null ||
        m_selectedNodes.Contains(mouseDownNode)))
            {
                if (Focused)
                {
                    bi = this.SelectedNodeBackground.Clone();
                }
                else if (!this.HideSelection)
                {
                    bi = this.InactiveSelectedNodeBackground.Clone();
                }
            }
            else // The owner draw handler should know that this is drawn unselected.
            {
                selected = false;
            }

            if (node == mouseDownNode)
            {
                selected = true;
                bi = this.SelectedNodeBackground.Clone();
            }

            TreeNodeAdvPaintBackgroundEventArgs e = new TreeNodeAdvPaintBackgroundEventArgs(
              node, g, selected, active, this.FullRowSelect, hotTrack, bi);

            if (m_paintFilter == null || !m_paintFilter.OnNodeBackgroundPaint(e))
            {
                if (this.OwnerDrawNodesBackground)
                {
                    OnNodeBackgroundPaint(e);
                }
            }

            // if user leave rendering to us
            if (!e.Handled)
            {
                Rectangle fullRow = Rectangle.Empty;

                if (FullRowSelect)
                {
                    fullRow = GetSelectionBounds(offsetY, GetIsMirrored(), node);
                }
                else
                {
                    fullRow = new Rectangle(0, node.Bounds.Y, node.Bounds.Width + node.Bounds.X, node.Bounds.Height);
                }

                BrushPaint.FillRectangle(g, fullRow, node.Background);

                if (!e.BrushInfo.IsEmpty && !this.IsEditing)
                {
                    Rectangle bounds = this.FullRowSelect ? fullRow : node.TextBounds;

                    if (bounds.IntersectsWith(clip))
                    {
                        if (!FullRowSelect || !selected)
                        {
                            bounds.Intersect(this.TreeColumnRectangle);
                        }
                                                
                        BrushPaint.FillRectangle(g, bounds, e.BrushInfo);
                    }
                }
            }
        }

        /// <summary>Draws the vertical lines of the tree.</summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="node">Node to draw the vertical lines to.</param>
        /// <param name="rowIndex">The RowIndex of the node.</param>
        /// <param name="lastNode">The Last Visible Node Row Index for comparing if in the node iteration nodes have passed it.</param>
        /// <param name="topCRNode"/>
        protected virtual void DrawVerticalLines(Graphics g, TreeNodeAdv topCRNode, TreeNodeAdv node,
          int rowIndex, TreeNodeAdv lastvisiblenode)
        {
            int lastNode = lastvisiblenode.TreeRowIndex;
            int endLineY2 = lastvisiblenode.Bounds.Y;
            int visibleCount = node.VisibleNodeCount;

            // if  node and children are not visible upwards  
            // || node (and children) are not visible downwards
            // || has no children 
            // || not expanded
            if ((this.VerticallScrollBar && rowIndex + visibleCount <= VScrollBar.Value)
                || (this.VerticallScrollBar && rowIndex > lastNode)
                || !node.HasNodes
                || !node.Visible && !this.DesignModeInternal
                || !node.Expanded)
            {
                return;
            }

            if (node.HasNodes)
            {
                TreeNodeAdv lastCNode = node.LastVisibleNode;

                // Lose the visible children of the last node
                if (lastCNode != null)
                {
                    visibleCount -= (lastCNode.VisibleNodeCount - ((lastCNode.Visible || this.DesignModeInternal) ? 1 : 0));
                }
            }

            TreeNodeAdv topVNode = topCRNode;

            // First child starts somewhere below the top visible node.
            if (topVNode.Level <= node.Level)
            {
                topVNode = node.FirstNode;
            }
            else
            {
                // Find the immediate child (one of the parent).
                while (topVNode != null && topVNode.Parent != node)
                {
                    topVNode = topVNode.Parent;
                }
                // The top node was not a child or grand-child
                if (topVNode == null)
                {
                    topVNode = node.FirstNode;
                }
            }

            // if !(node and immediate children are not visible - upwards)
            if (!(this.VerticallScrollBar && rowIndex + visibleCount <= this.VScrollBar.Value))
            {
                int level = node.Level;

                if (level != 0 || this.ShowRootLines)
                {
                    bool bIsMirrored = GetIsMirrored();

                    // the X of the line
                    int lineX = level * this.Indent + node.PlusMinus.Width / 2 + m_gutterSpace;

                    if (this.HScrollBar.Enabled && !bIsMirrored)
                    {
                        lineX -= this.HScrollBar.Value;
                    }

                    if (level != 0 && !this.NeedRootLinesSpace)
                    {
                        lineX -= this.Indent;
                        lineX += node.PlusMinus.Width / 2;
                    }

                    // Swap horizontally if mirrored
                    if (bIsMirrored)
                    {
                        lineX = MirrorHorizontalPosition(lineX, 1);
                    }

                    // The  last child of the node. It will determine the endpoint of the vertical line.
                    TreeNodeAdv lastChild = node.LastVisibleNode;
                    if (lastChild != null)
                    {
                        // Instead of going through each child node, start from the top visible node
                        // and look for a direct child of this node.
                        for (int i = topVNode.Index, nodeCount = node.Nodes.Count; i < nodeCount; i++)
                        {
                            TreeNodeAdv cNode = node.Nodes[i];

                            int rowI = cNode.TreeRowIndex;

                            // if node  not visible downwards break because the nodes after this 
                            // won't be visible either
                            if (this.VerticallScrollBar && rowI > lastNode && (cNode.Visible || this.DesignModeInternal))
                            {
                                lastChild = cNode;
                                break;
                            }

                            if (this.TransparentControls && cNode.ShouldDrawPlusMinus())
                            {
                                Point pt = NodeToPoint(cNode);

                                // Set  the clip to the rectangle of the plusminus.
                                Rectangle rcPlusMinus = new Rectangle(lineX - 2,
                                pt.Y + (cNode.Height - cNode.PlusMinus.Height) / 2,
                                4, cNode.PlusMinus.Height);

                                g.SetClip(rcPlusMinus, CombineMode.Exclude);
                            }
                        }

                        // The  start Y of the line.
                        int lineY1 = 0;

                        // Call this sparingly
                        if ((level == 0 && this.VScrollPos == 1) ||
                            (node.TreeRowIndex >= this.VScrollPos))
                        {
                            lineY1 = (level == 0 && this.VScrollPos == 1) ?
                            NodeToPoint(node.FirstNode).Y + node.FirstNode.Height / 2 :
                            NodeToPoint(node).Y + node.Height;
                        }

                        // The  end Y of the line.
                        int lineY2 = NodeToPoint(lastChild).Y + lastChild.Height / 2;
                        TreeNodeAdv tempNode = lastChild.NextNode;
                        int tempY2 = NodeToPoint(lastChild.PrevNode).Y + lastChild.Height / 2;
                        if (lastChild.TreeRowIndex != lastNode && VScrollPos == VisibleCount)
                        {
                            if (tempNode == null)
                            {
                               
                                if (lineY2 > tempY2)
                                    lineY2 = tempY2;
                            }
                        }
                         double tempCount = ((double)(this.TreeColumnRectangle.Height) / ((double)this.ItemHeight));
                        if (((tempCount - (double)VisibleCount) == 0) && !lastChild.HasNodes)
                        {                            
                            if (drawFlag == 0)
                           {
                        if (lineY2 == tempY2 + lastChild.Height)
                                   lineY2 = tempY2;
                               drawFlag++;
                            }
                        }
                        // Draw the line
                        g.DrawLine(m_penLine, new Point(lineX, lineY1), new Point(lineX, lineY2));
                        if (this.TransparentControls)
                        {
                            // Reset  the clip of the  graphics.
                            g.ResetClip();
                        }
                    }
                }
            }

            int count = node.Nodes.Count;

            for (int i = node.Nodes.IndexOf(topVNode); i < count; i++)
            {
                int rowI = NodeToRowIndex(node.Nodes[i]);

                // Testing  if during the for statement nodes have passed 
                // visible  range (bottom) so the method won't get called useless.
                if (VerticallScrollBar && rowI > lastNode)
                {
                    return;
                }

                // Draw the lines for the child nodes.
                DrawVerticalLines(g, topCRNode, node.Nodes[i], rowI, lastvisiblenode);
            }
        }

        protected virtual void OnPrint(object sender, PrintPageEventArgs e)
        {
            this.Printing = true;
            Invalidate(true);
            e.Graphics.TranslateTransform(e.MarginBounds.X, e.MarginBounds.Y, MatrixOrder.Append);
            OnPaint(new PaintEventArgs(e.Graphics, this.ClientRectangle));

            this.Printing = false;
        }


        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            bool bgPainted = false;

            if (this.ThemesEnabled && XPThemes.IsThemedOS &&
        XPThemes.IsThemeActive && !this.IgnoreThemeBackground)
            {
                int ox = pevent.ClipRectangle.Left;
                int oy = pevent.ClipRectangle.Top;
                int dx = pevent.ClipRectangle.Width;
                int dy = pevent.ClipRectangle.Height;

                if ((ox != 0) || (oy != 0) ||
          (dx != this.ClientRectangle.Width) ||
          (dy != this.ClientRectangle.Height))
                {
                    bgPainted = PaintChildrenBackground(pevent.Graphics, this, pevent.ClipRectangle);
                }

                bgPainted = false;
            }

            if (!bgPainted)
            {
                base.OnPaintBackground(pevent);
            }
        }

        [DocumentationExclude()]
        protected virtual void ThemedPaintBackground(Graphics graphics, Rectangle rect, Rectangle clip)
        {
            ThemedDrawing.Edit.DrawThemeBackground(graphics, 1, Enabled ? 1 : 4, rect, clip);
        }

        protected virtual bool PaintChildrenBackground(Graphics graphics, Control control, Rectangle clipRect)
        {
            foreach (Control child in control.Controls)
            {
                Rectangle childBounds = new Rectangle(child.Location, child.Size);
                childBounds = child.Parent.RectangleToScreen(childBounds);
                childBounds = RectangleToClient(childBounds);

                if (childBounds.Contains(clipRect))
                {
                    if (PaintChildrenBackground(graphics, child, clipRect))
                    {
                        return true;
                    }

                    Rectangle client = this.TreeColumnRectangle;
                    client = RectangleToScreen(client);
                    client = child.RectangleToClient(client);

                    clipRect = RectangleToScreen(clipRect);
                    clipRect = child.RectangleToClient(clipRect);
                    ThemedPaintBackground(graphics, client, clipRect);
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Class layout logic

        private void UpdateCustomControls(PaintEventArgs e, ArrayList drawedNodes)
        {
            if (this.NeedUpdateCustomControls)
            {
                NeedUpdateCustomControls = false;

                foreach (DictionaryEntry htElement in this.CustomControlCollection)
                {
                    Control control = (Control)htElement.Key;
                    TreeNodeAdv node = (TreeNodeAdv)htElement.Value;

                    if (drawedNodes.Contains(htElement.Value))
                    {
                        control.Visible = true;
                        node.UpdateCustomConrtol();
                    }
                    else
                    {
                        if (node.Bounds.Y >= e.ClipRectangle.Y)
                        {
                            control.Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>Method calculate how much space reserved for borders by control.</summary>
        /// <remarks>returned value must be be in range [0;2].</remarks>
        /// <returns>pixels reserved for borders.</returns>
        protected int GetBordersOffset()
        {
            int w = (m_borderStyle == BorderStyle.Fixed3D) ? 2 : 1;

            if (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive &&
        XPThemes.IsAppThemed && this.ThemedBorder)
            {
                w = 1;
            }

            if (m_borderStyle == BorderStyle.None)
            {
                w = 0;
            }

            return w;
        }

        /// <summary>Gets Rectangle reserved by control for tree painting. 
        /// RTL mirror effect is applied on result rectangle automatically. 
        /// Scrolling effects also applied on result rectangle automatically.</summary>
        protected internal Rectangle TreeColumnRectangle
        {
            get
            {
                return GetColumnsRect(false);
            }
        }
        /// <summary> Gets Rectangle reserved by control for tree painting. </summary>
        protected internal Rectangle TreeAllColumnsRectangle
        {
            get
            {
                return GetColumnsRect(true);
            }
        }

        /// <summary>
        /// Gets bounds for columns
        /// </summary>
        /// <param name="isAll"> if true than calculates for all columns, else calculates for first column</param>
        /// <returns></returns>
        private Rectangle GetColumnsRect(bool isAll)
        {
            Rectangle rc = this.ClientRectangle;

            // we have column in which we have to show tree nodes
            if (this.HasColumns)
            {
                int width = (isAll) ? this.Columns.GetTotalColumnsWidth(true) : this.Columns[0].Width;

                if (GetIsMirrored())
                {
                    int offsetX = this.HScrollBar.Maximum - this.HScrollBar.Minimum -
                        this.HScrollBar.LargeChange + 1 - this.HScrollPos;

                    rc = new Rectangle(rc.Right - width + offsetX, rc.Y, width, rc.Height);
                }
                else
                {
                    rc = new Rectangle(rc.X - this.HScrollPos, rc.Y, width, rc.Height);
                }
            }

            return rc;
        }

   
        private void DoLayout()
        {
        }
        #endregion

        #region Label Editor

        private void PrepareEditor()
        {
            //Set this before the BeforeEdit event.
            m_strLastText = m_activeNode.Text;
            m_labelEditor.Visible = true;
            this.IsEditEnding = true;
            m_labelEditor.Focus();
            this.IsEditEnding = false;
            m_labelEditor.SelectAll();
        }
        private void ShowEditor()
        {
            m_labelEditor.Tag = m_activeNode.Text;
            m_labelEditor.Font = m_activeNode.Font;

            m_textSelectionLength = m_labelEditor.SelectionLength;
            m_selectionStart = m_labelEditor.SelectionStart;
        }

        private void HideEditor()
        {
            this.IgnoreLeave = true;
            m_labelEditor.Hide();
            this.IgnoreLeave = false;
        }

        private void LabelEditStartTimer_Tick(object sender, EventArgs e)
        {
            if (this.ActiveNode == m_activeNodeForLabelEdit)
            {
                this.LabelEditStartTimer = null;
                this.BeginEdit(this.ActiveNode);
            }
        }

        private void labelEditor_KeyUp(object sender, KeyEventArgs e)
        {
            m_textSelectionLength = m_labelEditor.SelectionLength;
            m_selectionStart = m_labelEditor.SelectionStart;
        }

        // This will be called in a native app. scenario (with COM interop)
        private void labelEditor_KeyDown(object sender, KeyEventArgs e)
        {
            m_textSelectionLength = m_labelEditor.SelectionLength;
            m_selectionStart = m_labelEditor.SelectionStart;
            Keys keyData = e.KeyData;

            if (keyData == Keys.Return)
            {
                if (this.IsEditing)
                {
                    EndEdit(false);
                    return;
                }
            }
            else if (keyData == Keys.Escape)
            {
                if (this.IsEditing)
                {
                    this.CancelEdit = true;
                    EndEdit(true);
                    this.CancelEdit = false;
                    return;
                }
            }
        }

        private void labelEditor_TextChanged(object sender, EventArgs e)
        {
            if (m_labelEditor.Text != m_strLastText)
            {
                TreeNodeAdvCancelableEditEventArgs args = new TreeNodeAdvCancelableEditEventArgs(m_activeNode, m_labelEditor.Text);
                OnNodeEditorValidateString(args);

                if (args.Cancel)
                {
                    m_labelEditor.Text = m_strLastText;

                    m_labelEditor.SelectionStart = (m_labelEditor.SelectionStart == -1) ?
            m_labelEditor.Text.Length : m_selectionStart;

                    m_labelEditor.SelectionLength = m_textSelectionLength;
                }
                else
                {
                    m_strLastText = m_labelEditor.Text;
                }

                // adding +15 to the measured width because the edit when typing did not display the first character.
                Graphics g = CreateGraphics();
                int editorWidth = Math.Min(
                  g.MeasureString(m_labelEditor.Text, m_labelEditor.Font).ToSize().Width + 15,
                  this.TreeColumnRectangle.Width - m_activeNode.TextBounds.X);
                g.Dispose();

                if (editorWidth < 50)
                {
                    editorWidth = 50;
                }
                m_labelEditor.Width = editorWidth;
                m_strLastText = m_labelEditor.Text;
            }
        }
 
        private void labelEditor_MouseUp(object sender, MouseEventArgs e)
        {
            m_textSelectionLength = m_labelEditor.SelectionLength;
            m_selectionStart = m_labelEditor.SelectionStart;
        }

        private void labelEditor_LostFocus(object sender, EventArgs e)
        {
            if (!this.IgnoreLeave && this.ClientHeight > 0 && !this.IsEditEnding)
            {
                EndEdit(false);
            }
        }

        private void labelEditor_Leave(object sender, EventArgs e)
        {
            if (!this.IgnoreLeave)
            {
                EndEdit();
            }
        }
        #endregion

        #region Class utility methods

        private void ScrollWindow(int pValue)
        {
            int iDelta = m_vScrollPos > pValue ? 1 : -1;

            m_vScrollPos = pValue;
            ScrollWindow(0, iDelta, this.ClientRectangle, this.ClientRectangle, true);
        }

        private void RaiseNodeStateImageList()
        {
            if (NodeStateImageListChanged != null)
            {
                NodeStateImageListChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseDefaultExpandImageIndexChanged()
        {
            if (DefaultExpandImageIndexChanged != null)
            {
                DefaultExpandImageIndexChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseDefaultCollapseImageIndexChanged()
        {
            if (DefaultCollapseImageIndexChanged != null)
            {
                DefaultCollapseImageIndexChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets height for all columns
        /// </summary>
        private void UpdateColumnsHeight()
        {
            if (this.HasColumns)
            {
                foreach (TreeColumnAdv column in this.Columns)
                {
                    column.Height = m_headerHeight;
                }
            }
        }

        private Bitmap ExtractSelectedBoundsFromBmp(Bitmap bmp, TreeNodeAdv[] tnas)
        {
            Rectangle boundingRect = this.GetSelectedNodesRectangle(tnas);
            TreeNodeAdv parent = (tnas != null && tnas.Length > 0) ? (TreeNodeAdv)tnas[0].Parent : null;

            // Adjust the bounds to take scrolling into account
            if (this.HScrollBar.Enabled)
            {
                boundingRect.X += this.HScrollBar.Value;
            }

            Rectangle[] rectsDest = new Rectangle[tnas.Length];

            int minIndex = GetMinIndex(tnas);
            int maxIndex = GetMaxIndex(tnas);

            for (int i = minIndex, j = 0; i <= maxIndex; i++)
            {
                if (null == parent || !this.SelectedNodes.Contains(parent.Nodes[i]))
                {
                    continue;
                }

                Point location = new Point(0, this.ItemHeight * (i - minIndex));
                Size size = new Size(boundingRect.Width, this.ItemHeight);

                // Adjust the bounds to take scrolling into account
                if (this.HScrollBar.Enabled)
                {
                    location.X += this.HScrollBar.Value;
                }

                rectsDest[j++] = new Rectangle(location, size);
            }

            Bitmap selBmp = new Bitmap(boundingRect.Width, boundingRect.Height);

            Graphics g = Graphics.FromImage(selBmp);

            if (this.BackgroundColor != BrushInfo.Empty)
            {
                BrushPaint.FillRectangle(g, new Rectangle(0, 0, selBmp.Width, selBmp.Height), this.BackgroundColor);
            }
            else

            // To get the BackColor.
            // Don't call OnPaintBackground since it includes logic to draw child control's bg.
            {
                g.FillRectangle(SystemBrushes.Window, 0, 0, boundingRect.Width, boundingRect.Height);
            }

            for (int i = 0; i < this.SelectedNodes.Count; i++)
            {
                TreeNodeAdv treeNode = this.SelectedNodes[i] as TreeNodeAdv;

                g.DrawString(treeNode.Text, treeNode.Font, Brushes.Black, rectsDest[i]);
            }

            g.Dispose();

            return selBmp;
        }

        /// <summary>
        /// Returns the smallest rectangle enclosing the selected region of all the nodes specified.
        /// </summary>
        /// <param name="tnas">The nodes whose selected region is to be included in the resultant rect.</param>
        /// <returns>The bounding Rectangle.</returns>
        /// <remarks>The node's text and left images will be included in the rectangles.</remarks>
        protected Rectangle GetSelectedNodesRectangle(TreeNodeAdv[] tnas)
        {
            Rectangle boundingRect = Rectangle.Empty;

            if (tnas != null && tnas.Length > 0)
            {
                TreeNodeAdv parent = tnas[0].Parent;

                int minIndex = GetMinIndex(tnas);
                int maxIndex = GetMaxIndex(tnas);

                int maxWidth = tnas[0].Width;
                for (int i = 1; i < tnas.Length; i++)
                {
                    if (tnas[i].Width > maxWidth)
                    {
                        maxWidth = tnas[i].Width;
                    }
                }

                Point location = (null != parent) ? parent.Nodes[minIndex].Bounds.Location : Point.Empty;
                Size size = new Size(maxWidth, (maxIndex - minIndex + 1) * this.ItemHeight);

                boundingRect = new Rectangle(location, size);
            }

            return boundingRect;
        }

        private int GetMinIndex(TreeNodeAdv[] tnas)
        {
            int min = tnas[0].Index;

            for (int i = 1; i < tnas.Length; i++)
            {
                if (tnas[i].Index < min)
                {
                    min = tnas[i].Index;
                }
            }

            return min;
        }

        private int GetMaxIndex(TreeNodeAdv[] tnas)
        {
            int max = tnas[0].Index;
            for (int i = 1; i < tnas.Length; i++)
            {
                if (tnas[i].Index > max)
                {
                    max = tnas[i].Index;
                }
            }

            return max;
        }

        
        internal void rootMaxXChanged(int maxX)
        {
            if (!HScroll)
            {
                return;
            }

            int width = this.Width;
            if (this.VerticallScrollBar)
            {
                width -= SystemInformation.VerticalScrollBarWidth;
                width -= 3; // for borders
            }

            width = Math.Max(0, width);

            if (!this.InHScroll && maxX > width && !this.HasColumns)
            {
                this.HScrollBar.Minimum = 0;
                this.HScrollBar.Maximum = maxX; //-Width;
                this.HScrollBar.LargeChange = width; //(int)(((float)maxX-Width) * ((float)Width/(float)maxX));
            }

            if (maxX > width && this.Root.HasNodes)
            {
                this.HScrollBar.Enabled = true;
                this.HorizontalScrollBar = true;
            }
            else
            {
            }
        }

        [DocumentationExclude()]
        internal bool GetIsMirrored()
        {
            return (RightToLeft.Yes == RightToLeft);
        }

        [DocumentationExclude()]
        protected int MirrorHorizontalPosition(int nX, int nWidth)
        {
            int nBordWidth = (m_borderStyle == BorderStyle.Fixed3D) ? 2 : 1;
            int nOffset1 = 2 * nBordWidth + nWidth + nX;

            if (HorizontalScrollBar)
            {
                nOffset1 -= this.HScrollBar.Maximum - this.HScrollBar.Minimum -
                    this.HScrollBar.LargeChange - this.HScrollBar.Value + 1;
            }
            if (VerticallScrollBar && !this.Office2007ScrollBars)
            {
                nOffset1 += SystemInformation.VerticalScrollBarWidth;
            }

            int nRes = this.Width - nOffset1;

            return nRes;
        }

        /// <summary>Method recalculate Horizontal scroll Maximum value.</summary>
        private void RefreshHScrollbar()
        {
            if (!scrollable)
                return;
            int totalColumns = 0;

            if (this.Columns != null && this.Columns.VisibleColumns.Length > 0)
            {
                foreach (TreeColumnAdv column in this.Columns.VisibleColumns)
                {
                    totalColumns += column.Width;
                }
            }
            else if (null != Root)
            {
                totalColumns = Root.MaxX;
            }

            int width = this.DisplayRectangle.Width;
            bool bEnabled = (width < totalColumns);

            // apply limits
            width = Math.Max(0, width);
            int widthNew = Math.Max(width, totalColumns);

            this.HScrollBar.Minimum = 0;
            this.HScrollBar.Maximum = widthNew;
            this.HScrollBar.LargeChange = DisplayRectangle.Width + 1;
            if (this.Office2007ScrollBars && this.VerticallScrollBar)
            {
                this.HScrollBar.LargeChange -= SystemInformation.VerticalScrollBarWidth;
            }

            if (this.HScrollBar.Value >= this.HScrollBar.Maximum - this.HScrollBar.LargeChange)
            {
                this.HScrollBar.Value = this.HScrollBar.Maximum - this.HScrollBar.LargeChange + 1;
            }

            bEnabled = bEnabled && (this.HScrollBar.Maximum - this.HScrollBar.Minimum + 1 - this.HScrollBar.LargeChange > 0);

            // Set Horizontal scrollbar Value before setting the Horizontal scrollbar and HScroll visibility
            // to prevent showing Horizontal scrollbar when it is not needed.
            if (!bEnabled)
            {
                this.HScrollBar.Value = 0;
            }

            this.HorizontalScrollBar = bEnabled;
            this.HScroll = bEnabled;
        }


        private void RefreshVScrollbar()
        {
            RefreshVScrollbar(PointToNode(Point.Empty), 0);
        }

        private void RefreshVScrollbar(TreeNodeAdv currentNode, int y)
        {
            if (!scrollable)
                return;
            // init scrollbar values
            if (!this.InVScroll)
            {
                VScrollBar.Minimum = 1;
                VScrollBar.Maximum = Math.Max(VScrollBar.Minimum, Root.VisibleNodeCount - 1);
                VScrollPos = VScrollPos; // Hack for setting correct VScrollPos (including min/max)
                VScrollBar.Value = VScrollPos;

                int clientHeight = this.ClientHeight;

                // get first visible node in client
                TreeNodeAdv node = Root;
                int visibleNodesCount = 0;
                int iNodesHeight = 0;

                // iterate due to the height of next visible nodes.
                while (node != null && iNodesHeight + node.Height < clientHeight)
                {
                    TreeNodeAdv node1 = node.NextVisibleNode;
                    while (node1 != null && node1.HasNodes && iNodesHeight + node1.Height < clientHeight && node.Multiline)
                    {
                        iNodesHeight += node1.Height;
                        node1 = node1.NextVisibleNode;
                    }
                    iNodesHeight += node.Height;

                    node = node.NextVisibleNode;
                    visibleNodesCount++;
                }               
                VScrollBar.LargeChange = visibleNodesCount;
                if (node != null && node.Multiline)
                    VScrollBar.LargeChange = visibleNodesCount - 1;

                int height = TreeColumnRectangle.Height;

                // should scrollbar be shown - check for last visible node
                if (currentNode != null && VScrollPos == 1)
                {
                    while (currentNode != null && y + currentNode.Height < height)
                    {
                        y += currentNode.Height;
                        currentNode = currentNode.NextVisibleNode;
                    }
                }

                this.VerticallScrollBar = (y > height || currentNode != null || VScrollPos > 1);
                this.VScroll = (currentNode != null || VScrollPos > 1);
            }
        }

        private void MoveScrollBarOnClick(TreeNodeAdv currentNode)
        {
            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }

            int yOffset = 0;
            Point currentLocation = NodeToPoint(currentNode);
            TreeNodeAdv childNode;
            Point childLocation;
            bool subvisible = false;
            foreach (TreeNodeAdv node in currentNode.Nodes)
            {
                if (node.Visible)
                    subvisible = true;
            }
            // Count nuber of subnodes will be visible.
            if (subvisible)
            {
                for (int i = 0, len = currentNode.Nodes.Count; i < len; i++)
                {
                    // if clicked node is shifted to start - stop lifting it.
                    if (currentLocation.Y < 0)
                    {
                        break;
                    }

                childNode = currentNode.Nodes[i];
                childLocation = NodeToPoint(childNode);

                // If subnode doesn't fit to be shown in the client area
                // we must shift scroll by this node's height.
                if ((childLocation.Y + childNode.Height) > this.TreeColumnRectangle.Height)
                {
                    yOffset++;
                    currentLocation.Y -= childNode.Height;
                }

                // if clicked node is shifted to start - stop lifting it.
                if (currentLocation.Y == 0)
                {
                    break;
                    }
                }
            }

            // there are some subnodes expanded and are likely to be shown.
            if (yOffset > 0)
            {
                VScrollPos += yOffset;
            }
        }

        internal void ValidateScrollPosition()
        {
            if (!TreeColumnRectangle.IsEmpty)
            {
                if (this.Root != null && this.VScrollPos != 1)
                {
                    // Determine if top row needs to be adjusted:
                    TreeNodeAdv firstVisibleNode = this.TopVisibleNode;
                    TreeNodeAdv lastVisibleNode = this.LastVisibleNode;

                    // If atleast one node
                    if (firstVisibleNode != null)
                    {
                        // Check if this is the last node in the tree:
                        int belowLastNodeLocationY = lastVisibleNode.Bounds.Bottom;
                        belowLastNodeLocationY += 3;
                        TreeNodeAdv belowLastNode = PointToNode(new Point(1, belowLastNodeLocationY));
                        if (belowLastNode == null)
                        {
                            // The lastVisibleNode is the last in the tree
                            // Check if the empty space can be accomodated by some hidden tree nodes on top
                            int htAvailable = this.ClientHeight - lastVisibleNode.Bounds.Bottom;
                            int firstVisibleRowIndex = NodeToRowIndex(firstVisibleNode);
                            int preferredFirstVisibleRowIndex = firstVisibleRowIndex;
                            while (htAvailable > 0 && preferredFirstVisibleRowIndex > 1)
                            {
                                preferredFirstVisibleRowIndex--;
                                TreeNodeAdv node = RowIndexToNode(preferredFirstVisibleRowIndex);
                                if (node.Visible)
                                    htAvailable -= node.Bounds.Height;
                            }
                            if (htAvailable <= 0)
                            {
                                preferredFirstVisibleRowIndex++;
                            }
                            if (firstVisibleRowIndex > preferredFirstVisibleRowIndex)
                            {
                                // Adjust scroll bar to make hidden rows visible
                                this.VScrollPos -= (firstVisibleRowIndex - preferredFirstVisibleRowIndex);
                            }
                        }
                    }
                    refreshFlag = false;
                    Refresh();
                }
            }
        }
        private bool refreshFlag = true;
        /// <summary>
        /// Gets node by path.
        /// </summary>
        /// <param name="node"> TreeNodeAdv object, root for search </param>
        /// <param name="path"> Node path. </param>
        /// <returns> TreeNodeAdv object by path. </returns>
        private TreeNodeAdv GetNode(TreeNodeAdv node, string path)
        {
            string name;
            int index = path.IndexOf(this.PathSeparator);

            if (index == -1)
            {
                name = path;
            }
            else
            {
                name = path.Substring(0, index);
            }

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                if (node.Nodes[i].Text == name)
                {
                    if ((path == name) || (AddSeparatorAtEnd && path == name + this.PathSeparator))
                    {
                        return node.Nodes[i];
                    }

                    string newPath = path.Substring(index + 1, path.Length - index - 1);
                    TreeNodeAdv ret = GetNode(node.Nodes[i], newPath);

                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }

        private bool CheckToolTipClicked(Point mousePos)
        {
            if (m_toolTip == null || !m_toolTip.IsShowing())
            {
                return false;
            }

            Point ptScreen = this.PointToScreen(mousePos);
            Rectangle rcWindow = m_toolTip.Parent.RectangleToScreen(m_toolTip.Bounds);

            return rcWindow.Contains(ptScreen);
        }

        private void ApplyMouseBasedSelectionOn(TreeNodeAdv node)
        {
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);
            bool bShift = ((Control.ModifierKeys & Keys.Shift) == Keys.Shift);

            if (!bCtrl && !bShift)
            {
                if (!(this.MultiSelect && m_selectedNodes.Contains(node)))
                {
                    // Remove every thing else.
                    if (SetSelectedNode(node, m_selectedNodes, TreeViewAdvAction.ByMouse))
                    {
                        this.ActiveNode = node;
                        m_selectionBaseNode = node;
                    }
                }
            }
            // Control pressed or Shift with Control key pressed
            else if (!bShift || (bCtrl && bShift))
            {
                if (this.MultiSelect)
                {
                    // Add/Remove to the SelectedNodes.
                    if (m_selectedNodes.Contains(node) && !bCtrl)
                    {
                        if (SetSelectedNode(null, node, TreeViewAdvAction.ByMouse))
                        {
                            this.ActiveNode = node;
                        }
                    }
                    else /*if(!bCtrl)*/
                    {
                        bool allSelect = this.SelectionMode == TreeSelectionMode.MultiSelectAll;

                        TreeNodeAdv firstSelNode = null;
                        if (this.SelectedNodes.Count > 0)
                        {
                            firstSelNode = this.SelectedNodes[0];
                        }
                        if (m_selectedNodes.Count > 0 && m_selectedNodes.Contains(node) && bCtrl)
                        {
                            this.IsMouseDownWithCtrl = true;
                            return;
                        }
                        else
                        {
                            this.IsMouseDownWithCtrl = false;
                        }

                        if (firstSelNode == null || allSelect ||
              firstSelNode.Parent == node.Parent)
                        {
                            if (SetSelectedNode(node, new ArrayList(), TreeViewAdvAction.ByMouse))
                            {
                                this.ActiveNode = node;
                            }
                        }
                    }

                    if (this.ActiveNode == node)
                    {
                        m_selectionBaseNode = node;
                    }
                }
            }
        }

        private void ClearSelectionClickMonitors()
        {
            this.LabelEditStartTimer = null;
            this.ClickedOnSelection = false;
            m_ptMouseDown = Point.Empty;
        }

        protected internal void SetSelectionBaseNode(TreeNodeAdv node)
        {
            m_selectionBaseNode = node;
        }

        private bool IsDraggedPastMouseDownPoint(Point pt)
        {
            if (m_ptMouseDown == Point.Empty)
            {
                return true;
            }

            if (Math.Abs(m_ptMouseDown.X - pt.X) > 2 || Math.Abs(m_ptMouseDown.Y - pt.Y) > 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StopAutoScrollingInternal()
        {
            this.AutoScrollBounds = Rectangle.Empty;

            #region /* comments */
            // Code snippet was commented by Lucas in order to fix issue # 146
            // ( part 2 - problem dealed with autoscrolling )
            //this.AutoScrolling = ScrollBars.None;
            #endregion
        }

        private void StartAutoScrollingInternal()
        {
            this.AccelerateScrolling = AccelerateScrollingBehavior.Default;

            #region /* comments */
            // Code snippet was commented by Lucas in order to fix issue # 146
            // ( part 2 - problem dealed with autoscrolling )
            // this.AutoScrolling = ScrollBars.None;
            #endregion
        }


        private Point GetDragWindowLocation(bool bFinal)
        {
            Point pt = Control.MousePosition;

            if (!KeepDragCapturePoint)
            {
                pt.Y += 20;

                if (bFinal)
                {
                    pt.Y += DragHelper.DragWindow.DragBitmap.Height / 2;
                }
            }
            // make x coordinate for proper drag window location.
            else
            {
                Rectangle boundingRect = this.GetSelectedNodesRectangle(m_draggedTnas);

                // Adjust the bounds to take scrolling into account
                if (this.HScrollBar.Enabled)
                {
                    boundingRect.X += this.HScrollBar.Value;
                }

                Point origin = Point.Empty;
                Point cltPoint = PointToClient(pt);
                origin.X = cltPoint.X - boundingRect.Left;
                origin.Y = cltPoint.Y - boundingRect.Top;

                DragHelper.DragWindow.SetOrigin(origin);
            }

            return pt;
        }
        private void CancelMouseBasedSelection()
        {
            this.MouseBasedSelectionOn = false;
            this.LMouseDownNode = null;
        }

        private void SetNextNodeSelected(TreeNodeAdv newNode)
        {
            if (newNode == null)
            {
                throw new ArgumentNullException("newNode");
            }

            if (!SetSelectedNode(newNode, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
            {
                return;
            }

            this.ActiveNode = newNode;
            m_selectionBaseNode = newNode;
            // Redraw a tree line for currently selected node.
            RefreshVScrollbar(newNode, newNode.Bounds.Y);
        }

        private bool IsMultipleSelection(TreeNodeAdv node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (SelectionMode == TreeSelectionMode.Single)
            {
                return false;
            }

            return true;
        }

        private void FindNode(string s, bool bShiftPressed)
        {
            int count = 0;

            TreeNodeAdv FirstNode;
            if (bShiftPressed && Nodes.Count > 0)
                FirstNode = this.Nodes[0];
            else
                FirstNode = m_activeNode;

            if (FirstNode == null && Nodes.Count > 0)
            {
                FirstNode = Nodes[0];
            }
            if (FirstNode == null)
            {
                return;
            }

            TreeNodeAdv node;
            if (FirstNode.Text.Length >= s.Length && s.ToUpper() == FirstNode.Text.Substring(0, s.Length).ToUpper())
            {
                node = FirstNode;
            }
            else
            {
                node = FirstNode.NextSelectableNode;
                if (node == null)
                {
                    node = FirstNode;
                }
            }
            bool head = false;
            while (node != FirstNode || !head)
            {
                int i = 0;
                for (i = 0; i < s.Length && i < node.Text.Length; i++)
                {
                    if (s.ToUpper()[i] != node.Text.ToUpper()[i])
                    {
                        break;
                    }
                }
                if (i > count)
                {
                    count = i;
                    if (SetSelectedNode(node, m_selectedNodes, TreeViewAdvAction.ByKeyboard))
                    {
                        this.ActiveNode = node;
                        m_selectionBaseNode = node;
                    }
                }
                node = node.NextSelectableNode;
                if (node == null)
                {
                    node = Nodes[0];
                    head = true;
                }
            }
            /*
                  for(int i=0;i<Nodes.Count;i++)
                  {
                    FindVisibleNode(Nodes[i],s,ref match);
                  }
            */
            Invalidate();
        }

        internal void MakeDirty()
        {
            IComponentChangeService changeService = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

            if (changeService != null)
            {
                changeService.OnComponentChanging(this, null);
                changeService.OnComponentChanged(this, null, null, null);
            }
            Invalidate();
        }

        internal void ExpandedChanged(TreeNodeAdv node, bool expanded)
        {
            try
            {
                if (this.Updating)
                {
                    return;
                }

                if (this.IsVerticalGradient)
                {
                    Invalidate();
                    return;
                }

                Point pt = NodeToPoint(node);

                if (node.HasNodes)
                {
                    if (pt.Y < Height)
                    {
                        Invalidate(new Rectangle(pt, new Size(this.ClientRectangle.Width, Height - pt.Y)));
                    }
                }
                else
                {
                    Invalidate(new Rectangle(pt, new Size(this.ClientRectangle.Width, node.Height)));
                }

                // Init the scrollbar before updating since the
                // DrawVerticalLines method is more efficient if the VScrollbar setting is known.
                RefreshVScrollbar();

                if (expanded)
                {
                    MoveScrollBarOnClick(node);
                }

                Update();
            }
            finally
            {
                // Call the AfterXX events after updating, so that the node's Y will be up to date.
                // Node's Y will be used from within the ValidateScrollPosition
                TreeViewAdvNodeEventArgs e = new TreeViewAdvNodeEventArgs(node);

                if (expanded)
                {
                    OnAfterExpand(e);
                }
                else
                {
                    OnAfterCollapse(e);
                }
            }
        }

        internal void NodesChanging()
        {
            CancelMode();
        }

        internal void NodesChanged()
        {
            if (!this.Updating)
            {
                // If last node is null, then just reset the scroll to top.
                if (this.LastVisibleNode == null)
                {
                    this.VScrollPos = 1;
                    this.VScrollBar.Value = this.VScrollPos;
                }

                Invalidate();
            }
        }

        private void keyInputTimer_Tick(object sender, EventArgs e)
        {
            m_keyInputTimer.Enabled = false;
            m_strKeySearch = "";
            Invalidate();
        }

        private int GetEVVScrollPos(TreeNodeAdv node)
        {
            int y0 = NodeToPoint(node).Y + node.Height;
            int y = y0;
            int rowIndex = NodeToRowIndex(node);
            int targetRowIndex = rowIndex;

            int height = ClientHeight;

            while (y0 - y < height && node != null)
            {
                y -= node.Height;
                rowIndex--;
                TreeNodeAdv nNode = RowIndexToNode(rowIndex);
                if (y0 - y < height)
                {
                    node = nNode;
                }
                else
                {
                    break;
                }
            }
            if (node != null)
            {
                int topRowIndex = NodeToRowIndex(node);
                // If only partly-visible then scroll once more so that it's fully visible
                if (y0 - y > height && topRowIndex != targetRowIndex) // as opposed to yo-y = height
                {
                    //topRowIndex++;
                }
                return topRowIndex;
            }
            else
            {
                return 1;
            }

        }

    
        internal bool ExpandedChanging(TreeNodeAdv node, bool expanding)
        {
            TreeViewAdvCancelableNodeEventArgs e = new TreeViewAdvCancelableNodeEventArgs(node, false);

            if (expanding)
            {
                this.VScrollBar.BeginUpdate();
                this.HScrollBar.BeginUpdate();
                OnBeforeExpand(e);
                this.VScrollBar.EndUpdate();
                this.HScrollBar.EndUpdate();
            }
            else
            {
                OnBeforeCollapse(e);

                if (!SelectOnCollapse)
                {
                    if (!e.Cancel && IsCollapsedContainsSelectionNodes(e.Node))
                    {
                        TreeNodeAdvCollection removedNodes = new TreeNodeAdvCollection(null);
                        bool shouldSelectCollapse = false;
                        bool shouldSetActive = false;

                        if (IsCollapsedContainsActiveNode(e.Node))
                        {
                            shouldSetActive = true;
                        }

                        if (IsCollapsedContainsSelectionNodes(e.Node))
                        {
                            shouldSelectCollapse = true;
                        }

                        foreach (TreeNodeAdv tna in SelectedNodes)
                        {
                            if (IsNodeUnderCollapsedNode(e.Node, tna))
                            {
                                removedNodes.Add(tna);
                            }
                        }

                        foreach (TreeNodeAdv tna in removedNodes)
                        {
                            SelectedNodes.Remove(tna);
                        }

                        removedNodes.Clear();
                        foreach (TreeNodeAdv highlighted in m_hashHighlightedNodes.Values)
                        {
                            if (IsNodeUnderCollapsedNode(e.Node, highlighted))
                            {
                                removedNodes.Add(highlighted);
                            }
                        }

                        removedNodes.Add(e.Node);

                        foreach (TreeNodeAdv tna in removedNodes)
                        {
                            m_hashHighlightedNodes.Remove(tna);
                        }

                        if (!this.MultiSelect)
                        {
                            SelectedNode = null;
                        }

                        if (shouldSelectCollapse && m_selectedNodes.Contains(e.Node))
                        {
                            SetSelectedNode(e.Node, new ArrayList(), TreeViewAdvAction.Collapse);
                        }
                        if (shouldSetActive && shouldSelectCollapse)
                        {
                            m_activeNode = e.Node;
                        }
                    }
                }
                else
                {
                    // Added conditions for collapsed parent node is to be focused when its + icon is clicked.
                    if (!e.Cancel && this.MultiSelect)
                    {
                        if (IsCollapsedContainsSelectionNodes(e.Node) ||
              IsCollapsedContainsActiveNode(e.Node) ||
              IsNodeUnderCollapsedNode(e.Node, SelectedNode) ||
              e.Node == SelectedNode)
                        {
                            this.RemoveNodeFromList(e.Node);
                        }

                    }
                    // if SelectionMode is Single and the collapsed node contains the selected node as child node, The collapsed node should change into SelectedNode .
                    else if (!e.Cancel && this.SingleSelect)
                    {
                        if (IsNodeUnderCollapsedNode(e.Node, SelectedNode))
                        {
                            SelectedNode = e.Node;
                        }
                    }

                    // Fix #1471: AfterSelect event does not fire if selection is changed due to collapse of node
                    if ((SelectedNode != null) && !e.Cancel &&
            (!(SelectedNode == e.Node) && SelectedNode.Parent == e.Node))
                    {
                        SelectedNode = e.Node;
                    }
                }
            }

            return !e.Cancel;
        }

        /// <summary>
        /// Returns true/false if the specified node is under the collapsed node.
        /// </summary>
        /// <param name="collapsed">The collapsed.</param>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if specified node under collapsed node; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNodeUnderCollapsedNode(TreeNodeAdv collapsed, TreeNodeAdv node)
        {
            // wrong arguments
            if (node == null || collapsed == null || node == collapsed)
            {
                return false;
            }

            // if top of hierarchy reached
            if (node.Parent == null || node.Parent == this.Root)
            {
                return false;
            }
            else if (node.Parent == collapsed)
            {
                return true;
            }

            return IsNodeUnderCollapsedNode(collapsed, node.Parent);
        }

        /// <summary>
        /// Returns true/false if the active node is under the collapsed node.
        /// </summary>
        /// <param name="collapsed">The collapsed.</param>
        /// <returns><c>true</c> if active node under collapsed node; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCollapsedContainsActiveNode(TreeNodeAdv collapsed)
        {
            return IsNodeUnderCollapsedNode(collapsed, this.ActiveNode);
        }

        /// <summary>
        /// If the some columns has gradient style than return true.
        /// </summary>
        /// <returns></returns>
        private bool IsColumnsGradient()
        {
            if (this.HasColumns)
            {
                foreach (TreeColumnAdv column in Columns)
                {
                    if (column.AreaBackground.Style == BrushStyle.Gradient && column.AreaBackground.GradientStyle != GradientStyle.Horizontal)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the nodes list from selected nodes list, if its under the collapsed node.
        /// </summary>
        /// <param name="collapsedNode">Collapsed node.</param>
        private void RemoveNodeFromList(TreeNodeAdv collapsedNode)
        {
            TreeNodeAdvCollection toRemove = new TreeNodeAdvCollection();

            // If selected node is under collapsed node, it should be removed from selected list.
            foreach (TreeNodeAdv tna in SelectedNodes)
            {
                if (IsNodeUnderCollapsedNode(collapsedNode, tna))
                {
                    toRemove.Add(tna);
                }
            }

            m_activeNode = collapsedNode;

            if (!m_selectedNodes.Contains(collapsedNode))
            {
                SetSelectedNode(collapsedNode, toRemove, TreeViewAdvAction.Collapse);
            }
            else if (toRemove.Count > 0)
            {
                collapsedNode = null;
                SetSelectedNode(collapsedNode, toRemove, TreeViewAdvAction.Collapse);
            }

        }

        /// <summary>
        /// Returns true if the node which is to be collapsed contains any selcted nodes.
        /// </summary>
        /// <param name="collapsednode">The Collapsed node.</param>
        /// <returns><c>true</c> if collapsed node contains selection nodes; otherwise, it returns <c>false</c>.
        /// </returns>
        private bool IsCollapsedContainsSelectionNodes(TreeNodeAdv collapsednode)
        {
            foreach (TreeNodeAdv tna in SelectedNodes)
            {
                if (IsNodeUnderCollapsedNode(collapsednode, tna))
                {
                    return true;
                }
            }

            if (!SelectOnCollapse && IsNodeUnderCollapsedNode(collapsednode, m_activeNode))
            {
                return true;
            }

            return false;
        }

        /// <summary>Exclude node from selection and if it is an ActiveNode - reset 
        /// ActiveNode value.</summary>
        /// <param name="node">Node for removing from tree</param>
        internal void RemovedNode(TreeNodeAdv node)
        {
            ArrayList removedNodes = new ArrayList(new TreeNodeAdv[] { node });
            SetSelectedNode(null, removedNodes, TreeViewAdvAction.Unknown, false, true);

            if (this.ActiveNode == node)
            {
                this.ActiveNode = null;
            }
        }

        internal bool IsBaseStyleRemoveable(string styleName)
        {
            if (styleName == DefaultBaseStyleName ||
        styleName == DefaultColumnStyleName ||
        styleName == DefaultSubItemStyleName)
            {
                return false;
            }

            return true;
        }

        internal string GetHintTextForStyle(string styleName)
        {
            string hint = String.Empty;
            if (styleName == DefaultBaseStyleName)
            {
                hint = "Standard Style applied for all nodes. Not deletable.";
            }
            else
            {
                int nodeLevel = this.IsNodeLevelStyle(styleName);
                if (nodeLevel != -1)
                {
                    hint = "Style will be used for nodes in Level " + nodeLevel.ToString();
                }
            }

            return hint;
        }

        private int IsNodeLevelStyle(string styleName)
        {
            string nodeLevelString = MultiColumnTreeView.NodeLevelStyleBaseName;
            if (styleName.IndexOf(nodeLevelString) == 0)
            {
                styleName = styleName.Substring(nodeLevelString.Length);
                int level = -1;
                try
                {
                    level = Int32.Parse(styleName);
                }
                catch
                {
                }
                if (level < -1)
                {
                    level = -1;
                }
                return level;
            }
            else
            {
                return -1;
            }
        }

        private void UpdateRootPlusMinusVisibility()
        {
            if (this.Root != null)
            {
                // Pushing the visibility setting of the PM part in the node
                // to improve painting performance.
                foreach (TreeNodeAdv node in this.Root.Nodes)
                {
                    node.UpdatePlusMinusVisibility();
                }
            }
        }

        private void iterate(TreeNodeAdv tna, bool bExpanded, TreeNodeAdvCollection collection)
        {
            // Checks whether the node contains child node or not. If yes, foreach 
            // statement will iterate through all sibiling nodes of this node.
            if (tna.HasNodes)
            {
                if (tna.Expanded == bExpanded)
                {
                    if (!collection.Contains(tna))
                    {
                        collection.Add(tna);
                    }
                }

                 iterate(tna.Nodes[0], bExpanded, collection);
            }

            if (tna.NextNode != null)
            {
                iterate(tna.NextNode, bExpanded, collection);
            }
        }

        internal bool SetSelectedNode(TreeNodeAdv nodeToAdd, ArrayList removedNodes, TreeViewAdvAction action)
        {
            ArrayList addedNodes = new ArrayList();

            if (nodeToAdd != null)
            {
                addedNodes.Add(nodeToAdd);
            }

            return SetSelectedNode(addedNodes, removedNodes, action);
        }

        internal bool SetSelectedNode(TreeNodeAdv nodeToAdd, TreeNodeAdv nodeToRemove, TreeViewAdvAction action)
        {
            ArrayList addedNodes = new ArrayList(), removedNodes = new ArrayList();

            if (nodeToAdd != null)
            {
                addedNodes.Add(nodeToAdd);
            }

            if (nodeToRemove != null)
            {
                removedNodes.Add(nodeToRemove);
            }

            return SetSelectedNode(addedNodes, removedNodes, action);
        }

             internal bool SetSelectedNode(ArrayList nodesToAdd, ArrayList nodesToRemove, TreeViewAdvAction action)
        {
            return SetSelectedNode(nodesToAdd, nodesToRemove, action, true, false);
        }

        /// <summary>
        /// This method is used internally by the tree control to add and remove selected nodes. This
        /// method will fire the appropriate selection events to let the user cancel the selection, etc.
        /// </summary>
        /// <param name="nodesToAdd">The nodes to add.</param>
        /// <param name="nodesToRemove">The nodes to remove.</param>
        /// <param name="action">Specifies what kind of action triggered this call.</param>
        /// <param name="fireEvent">Indicates whether selection events should be fired before and after this selection change.</param>
        /// <param name="forceRemove">Indicates whether the specified nodes to be removed from selection will be removed even if the user
        /// cancelled the selection change the BeforeSelect event handler.</param>
        /// <returns>True if the selection changed; false otherwise.</returns>
        protected internal bool SetSelectedNode(ArrayList nodesToAdd, ArrayList nodesToRemove,
          TreeViewAdvAction action, bool fireEvent, bool forceRemove)
        {
            if (nodesToRemove == null && nodesToAdd == null)
            {
                return true;
            }

            if (nodesToRemove == null)
            {
                nodesToRemove = new ArrayList();
            }
            if (nodesToAdd == null)
            {
                // See if there is anything to remove, or else quit.
                bool removeable = false;

                foreach (TreeNodeAdv node in nodesToRemove)
                {
                    if (m_selectedNodes.Contains(node))
                    {
                        removeable = true;
                        break;
                    }
                }
                if (!removeable)
                {
                    return true;
                }

                nodesToAdd = new ArrayList();
            }
            // Procede only if the nodes to add and remove are different.
            if (nodesToRemove.Count == nodesToAdd.Count)
            {
                bool different = false;
                for (int i = 0; i < nodesToRemove.Count; i++)
                {
                    if (nodesToRemove[i] != nodesToAdd[i])
                    {
                        different = true;
                        break;
                    }
                }
                if (!different)
                {
                    return true;
                }
            }

            // We will be operating on this collection, so cannot enumerate on it.
            if (nodesToRemove == m_selectedNodes)
            {
                nodesToRemove = m_selectedNodes.Clone() as ArrayList;
            }

            // NOthing to change, return.
            if (nodesToAdd.Count == 0 && nodesToRemove.Count == 0)
            {
                return true;
            }

            SelectedNodesCollection newNodes = this.SelectedNodes.Clone() as SelectedNodesCollection;

            foreach (TreeNodeAdv node in nodesToRemove)
            {
                newNodes.Remove(node);
            }

            bool bIsSingleValid = (newNodes.Count == 0 && SelectionMode == TreeSelectionMode.Single);

            foreach (TreeNodeAdv node in nodesToAdd)
            {
                bool bIsMultipleValid = ((newNodes.Count > 0 && newNodes[0].Parent == node.Parent
          && SelectionMode == TreeSelectionMode.MultiSelectSameLevel) || newNodes.Count == 0
          || SelectionMode == TreeSelectionMode.MultiSelectAll);

                if ((!bIsSingleValid && !bIsMultipleValid)
          || (newNodes.Contains(node) && SelectionMode == TreeSelectionMode.MultiSelectSameLevel))
                {
                    continue;
                }

                newNodes.Add(node);
            }

            OrderNodesByRowIndex(newNodes);

            TreeViewAdvCancelableSelectionEventArgs args = new TreeViewAdvCancelableSelectionEventArgs(newNodes, action, false);

            if (fireEvent)
            {
                args.SelectedNodes.SetFixedSize(true);
                OnBeforeSelect(args);
                args.SelectedNodes.SetFixedSize(false);
            }

            if (!args.Cancel)
            {
                m_selectedNodes.Clear();

                TreeNodeAdv lastSelected = null;
                // Select nodes in upward and downward direction according to the selection by up/down keys
                if (this.SelectUpwardDirection)
                {
                    for (int i = newNodes.Count - 1; i >= 0; i--)
                    {
                        TreeNodeAdv node = newNodes[i];
                        lastSelected = node;
                        m_selectedNodes.Add(node);
                        if (this.EnsureVisibleSelectedNode && this.EnsureVisibleFlag)
                        {
                            if ((!this.IsMouseUp) || (this.IsMouseUp && node == m_lastSelectedNode))
                            {
                                EnsureVisible(node);
                            }
                        }
                    }
                }
                else
                {
                    foreach (TreeNodeAdv node in newNodes)
                    {
                        lastSelected = node;
                        m_selectedNodes.Add(node);

                        if (this.EnsureVisibleSelectedNode && this.EnsureVisibleFlag)
                        {
                            if ((!this.IsMouseUp) || (this.IsMouseUp && node == m_lastSelectedNode))
                            {
                                if (m_lastSelectedByKeyBoard != null)
                                {
                                    if (node == m_lastSelectedByKeyBoard)
                                    {
                                        EnsureVisible(node);
                                    }
                                }
                                else
                                {
                                    EnsureVisible(node);
                                }
                            }
                        }
                    }

                    m_lastSelectedByKeyBoard = null;
                }

                if (newNodes.Count > 0)
                {
                    this.RefreshHighlitedNodes();
                    Invalidate();
                }

                if (lastSelected != null && lastSelected != this.Root)
                {
                    m_currentSelectedNodeIndex = lastSelected.Parent.Nodes.IndexOf(lastSelected);
                }
                else
                {
                    m_currentSelectedNodeIndex = -1;
                }

                if (fireEvent)
                {
                    OnAfterSelect(EventArgs.Empty);
                }
            }
            else if (forceRemove)
            {
                foreach (TreeNodeAdv node in nodesToRemove)
                {
                    m_selectedNodes.Remove(node);
                }

                if (nodesToRemove.Count > 0)
                {
                    this.RefreshHighlitedNodes();
                    Invalidate();
                }
            }

            return !args.Cancel;
        }

        /// <summary>
        /// Highlighted all parent nodes.
        /// </summary>
        /// <param name="node"></param>
        private void AddHighlightParent(TreeNodeAdv node)
        {
            if (node != null && node != this.Root &&
        !m_hashHighlightedNodes.ContainsKey(node))
            {
                m_hashHighlightedNodes.Add(node, node);
                AddHighlightParent(node.Parent);
            }
        }

        /// <summary>
        /// Refresh Highlighting.
        /// </summary>
        private void RefreshHighlitedNodes()
        {
            if (null == m_hashHighlightedNodes)
            {
                m_hashHighlightedNodes = new Hashtable();
            }
            else
            {
                m_hashHighlightedNodes.Clear();
            }

            foreach (TreeNodeAdv node in this.SelectedNodes)
            {
                AddHighlightParent(node.Parent);
            }
        }
        private void OrderNodesByRowIndex(ArrayList nodes)
        {
            ArrayList nodesX = new ArrayList();

            foreach (TreeNodeAdv node in nodes)
            {
                if (nodesX.Count == 0)
                {
                    // Insert on top
                    nodesX.Add(node);
                    continue;
                }

                int nodeRowIndex = NodeToRowIndex(node);
                int i = -1;
                bool inserted = false;
                foreach (TreeNodeAdv nodeX in nodesX)
                {
                    i++;
                    if (NodeToRowIndex(nodeX) > nodeRowIndex)
                    {
                        // Insert before the one that has a higher rowindex.
                        nodesX.Insert(i, node);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                // Insert at the bottom.
                {
                    nodesX.Add(node);
                }
            }
            nodes.Clear();
            nodes.AddRange(nodesX);
        }

        /// <summary>
        /// Gets bounds for area which must be select
        /// </summary>
        /// <param name="offsetY"> distance from top of clientArea to Y position of node</param>
        /// <param name="bMirror"> true if RightToLeft mode</param>
        /// <param name="node"> current node object </param>
        /// <returns></returns>
        private Rectangle GetSelectionBounds(int offsetY, bool bMirror, TreeNodeAdv node)
        {
            // initializing helper variables
            int boundsWidth = 0;
            int offsetX = 0;
            Rectangle selectionBounds = new Rectangle();
            int borderOffset = GetBordersOffset();
            int scrollOffset = (VerticallScrollBar && bMirror) ? SystemInformation.VerticalScrollBarWidth : 0;

            if (HasColumns && Columns.Count > 0) // calculating width of selection for tree with columns 
            {
                offsetX = ((!bMirror) ? Columns[0].Bounds.X : Columns[0].Bounds.Right) - borderOffset - scrollOffset;

                for (int i = 0, len = Columns.Count; i < len; i++)
                {
                    if (Columns[i].Visible)
                    {
                        boundsWidth += (bMirror) ? -Columns[i].Width : Columns[i].Width;
                    }
                }
            }
            else // calculating width of selection for tree without columns
            {
                offsetX = ((!bMirror) ? node.Bounds.X : node.Bounds.Right);
                boundsWidth = (bMirror) ? -node.Bounds.Width : node.Bounds.Width;
            }

            if (bMirror) // revert rectangle if mirror mode
            {
                offsetX += boundsWidth;
                boundsWidth = -boundsWidth;
            }

            selectionBounds.Width = boundsWidth;
            selectionBounds.X = offsetX;
            selectionBounds.Y = offsetY;
            selectionBounds.Height = node.Height;

            return selectionBounds;
        }

        /// <summary>
        /// Gets mouse down node
        /// </summary>
        /// <returns>Returns Mouse down node</returns>
        private TreeNodeAdv GetMouseDownNode()
        {
            TreeNodeAdv mouseDownNode = null;
            bool bCtrl = ((Control.ModifierKeys & Keys.Control) == Keys.Control);

            // Only RMouseDownNode or LMouseDownNode will be non-null.
            if (this.RMouseDownNode != null)
            {
                mouseDownNode = this.RMouseDownNode;
            }
            else if (this.LMouseDownNode != null && (this.SingleSelect || !bCtrl))
            {
                mouseDownNode = this.LMouseDownNode;
            }

            return mouseDownNode;
        }
        #endregion

        #region Class event handlers
          private void GradientPanel_BackColorChanged(object sender, EventArgs e)
        {
            InvalidateWindow();
        }

        private void selectedNodes_Changed(object sender, CollectionChangeEventArgs e)
        {
            if (e.Element != null)
            {
                if (e.Action == CollectionChangeAction.Add)
                {
                    TreeNodeAdv node = e.Element as TreeNodeAdv;

                    if (node != null && node.TreeView != this)
                    {
                        throw new ArgumentException("A TreeNodeAdv that was not a child of the MultiColumnTreeView was added to the MultiColumnTreeView's SelectedNodes collection. This is not allowed.");
                    }

                    this.LMouseDownNode = this.RMouseDownNode = null;
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    this.ClickedOnSelection = false;
                }
            }

            Invalidate();
        }


        private void columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            UpdateColumnsHeight();
            InvalidateNc();
            RefreshHScrollbar();
        }

        private void OnPrintDocumentBeginPrint(object sender, PrintEventArgs e)
        {
            m_ptPrintPosition = new Point(0, 0);
            m_pdCurrentDirection = PrintDirection.Horizontal;
            m_iPageNumber = 0;
            m_dtDateTime = DateTime.Now;
        }
    
        private void OnPrintDocumentPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rcSource = new Rectangle(m_ptPrintPosition, e.MarginBounds.Size);
            Rectangle rcDestination = e.MarginBounds;
            m_iPageNumber++;

            if ((rcSource.Height % m_iNodeHeight) > 0)
            {
                rcSource.Height -= (rcSource.Height % m_iNodeHeight);
            }

            g.DrawImage(m_imgControl, rcDestination, rcSource, GraphicsUnit.Pixel);
            // Check to see if we need more pages.
            if ((m_imgControl.Height - m_iScrollBarHeight) > rcSource.Bottom ||
                (m_imgControl.Width - m_iScrollBarWidth) > rcSource.Right)
            {
                e.HasMorePages = true;
            }

            if (m_pdCurrentDirection == PrintDirection.Horizontal)
            {
                if (rcSource.Right < (m_imgControl.Width - m_iScrollBarWidth))
                {
                    m_ptPrintPosition.X += (rcSource.Width + 1);
                }
                else
                {
                    m_ptPrintPosition.X = 0;
                    m_ptPrintPosition.Y += (rcSource.Height + 1);
                    m_pdCurrentDirection = PrintDirection.Vertical;
                }
            }
            else if (m_pdCurrentDirection == PrintDirection.Vertical && rcSource.Right < (m_imgControl.Width - m_iScrollBarWidth))
            {
                m_pdCurrentDirection = PrintDirection.Horizontal;
                m_ptPrintPosition.X += (rcSource.Width + 1);
            }
            else
            {
                m_ptPrintPosition.Y += (rcSource.Height + 1);
            }

            using (Brush brush = new SolidBrush(Color.White))
            {
                // Print footer.
                string sFooter = m_iPageNumber.ToString(System.Globalization.NumberFormatInfo.CurrentInfo);
                Font fontFooter = new Font(FontFamily.GenericSansSerif, 10f);
                SizeF szfFooter = g.MeasureString(sFooter, fontFooter);

                PointF ptBottomCenter = new PointF(e.PageBounds.Width / 2, e.MarginBounds.Bottom + ((e.PageBounds.Bottom - e.MarginBounds.Bottom) / 2));
                PointF ptFooterLocation = new PointF(ptBottomCenter.X - (szfFooter.Width / 2), ptBottomCenter.Y - (szfFooter.Height / 2));

                g.DrawString(sFooter, fontFooter, brush, ptFooterLocation);
            }
        }




        #endregion

#if ! ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
            }
        }
        bool isScaling = false;

        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            this.ItemHeight = (int)(ITMHEIGHT * scaleFactor);
            this.HeaderHeight = (int)(HDRHeight * scaleFactor);
            foreach (TreeColumnAdv clmn in this.Columns)
            {
                clmn.Width = (int)(MinColumnWidth * scaleFactor);
                clmn.Font = this.Font;
            }
            foreach (TreeNodeAdv item in this.Nodes)
            {
                foreach (TreeNodeAdvSubItem itm in item.SubItems)
                {
                    itm.Font = item.Font;
                }
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        [
        EditorBrowsable(EditorBrowsableState.Never),
        Browsable(false)
        ]
        public new event EventHandler PaddingChanged
        {
            add
            {
                base.PaddingChanged += value;
            }
            remove
            {
                base.PaddingChanged -= value;
            }
        }
#endif
    }

    #region SearchFunctionality

    public class TreeViewFindReplaceDialog
    {
        internal bool isMatchFound = false;
        private int NodePointIndex = -1;
        private string searchText = string.Empty;
        internal TreeNodeAdvCollection treeNodeAdvCollection = new TreeNodeAdvCollection();
        private MultiColumnTreeView treeView = null;
        private TreeViewSearchOption m_searchOptions = TreeViewSearchOption.MatchWholeText;
        private TreeViewSearchRange m_treeSearchRange = TreeViewSearchRange.TreeView;
        private TreeViewSearchNavigation m_nodeSearchType = TreeViewSearchNavigation.SearchAll;

        /// <summary>
        /// Initializes new instances of TreeViewAdvFindReplaceDialog class
        /// </summary>
        /// <param name="tree">TreeViewAdv instance</param>
        public TreeViewFindReplaceDialog(MultiColumnTreeView tree)
        {
            treeView = tree;
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdvCollection that matches search string
        /// </summary>
        internal TreeNodeAdvCollection SearchTreeNodeAdvCollection
        {
            get
            {
                return treeNodeAdvCollection;
            }
            set
            {
                treeNodeAdvCollection = value;
            }
        }

        /// <summary>
        /// TreeViewAdv instance
        /// </summary>
        internal MultiColumnTreeView TreeView
        {
            get
            {
                return treeView;
            }
            set
            {
                if (value != treeView)
                    treeView = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeViewAdv Search Option
        /// </summary>
        public TreeViewSearchOption TreeViewSearchOption
        {
            get
            {
                return m_searchOptions;
            }
            set
            {
                if (m_searchOptions != value)
                    m_searchOptions = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeViewAdv search range 
        /// </summary>
        public TreeViewSearchRange TreeViewSearchRange
        {
            get
            {
                return m_treeSearchRange;
            }
            set
            {
                if (m_treeSearchRange != value)
                    m_treeSearchRange = value;
            }
        }

        /// <summary>
        /// Gets/Sets value of TreeNodeAdv search navigation type
        /// </summary>
        public TreeViewSearchNavigation TreeViewSearchNavigation
        {
            get
            {
                return m_nodeSearchType;
            }
            set
            {
                if (m_nodeSearchType != value)
                    m_nodeSearchType = value;
            }
        }

        /// <summary>
        /// Highlights matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>true if match found</returns>
        public bool Find(string nodeText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeMatched = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            isNodeMatched = this.Find(nodeText);
            return isNodeMatched;
        }

        /// <summary>
        /// Highlights matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <returns>true if match found</returns>
        public bool Find(string nodeText)
        {
            this.SearchTreeNodeAdvCollection.Clear();
            if (this.Search(nodeText))
            {
                this.searchText = nodeText;
                if (this.TreeView != null && this.NodePointIndex == -1)
                {
                    NodePointIndex = 0;
                }
                else
                {
                    if (this.TreeViewSearchNavigation != TreeViewSearchNavigation.SearchUp)
                    {
                        NodePointIndex = this.SearchTreeNodeAdvCollection.IndexOf(this.TreeView.SelectedNode);
                        if (NodePointIndex == this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            if (this.TreeViewSearchNavigation == TreeViewSearchNavigation.SearchAll)
                                NodePointIndex = 0;
                        }
                        else if (NodePointIndex <= this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            NodePointIndex += 1;
                        }

                    }
                    else
                    {
                        NodePointIndex = this.SearchTreeNodeAdvCollection.IndexOf(this.TreeView.SelectedNode);
                        if (NodePointIndex > 0 && NodePointIndex <= this.SearchTreeNodeAdvCollection.Count - 1)
                        {
                            NodePointIndex -= 1;
                        }
                    }
                }
                if (this.NodePointIndex >= 0 && this.NodePointIndex < this.SearchTreeNodeAdvCollection.Count
                          && this.TreeView.SelectedNode != this.SearchTreeNodeAdvCollection[NodePointIndex])
                {
                    this.TreeView.RaiseNodeBeforeFindEvent(this.SearchTreeNodeAdvCollection[NodePointIndex], this.searchText);
                    if (!this.TreeView.DisableFinding)
                    {
                        this.TreeView.SelectedNode = this.SearchTreeNodeAdvCollection[NodePointIndex];
                        this.TreeView.RaiseNodeAfterFindEvent(this.SearchTreeNodeAdvCollection[NodePointIndex], this.searchText);
                    }
                    return true;
                }
            }
            else
            {
                this.TreeView.SelectedNode = null;
                return false;
            }
            return false;
        }

        /// <summary>
        /// Highlights all matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if match found</returns>
        public bool FindAll(string nodeText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeMatched = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            isNodeMatched = this.FindAll(nodeText);
            return isNodeMatched;
        }

        /// <summary>
        /// Highlights all matched TreeNodeAdv based on search string
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <returns>returns true if match found</returns>
        public bool FindAll(string nodeText)
        {
            this.SearchTreeNodeAdvCollection.Clear();
            this.TreeView.SelectedNode = null;
            if (this.Search(nodeText))
            {
                this.searchText = nodeText;
                foreach (TreeNodeAdv node in this.SearchTreeNodeAdvCollection)
                {
                    this.TreeView.RaiseNodeBeforeFindEvent(node, nodeText);
                }
                if (!this.TreeView.DisableFinding)
                {
                    this.TreeView.SelectedNodes.AddRange(this.SearchTreeNodeAdvCollection);
                    foreach (TreeNodeAdv nodes in this.TreeView.SelectedNodes)
                    {
                        this.TreeView.RaiseNodeAfterFindEvent(nodes, nodeText);
                    }
                }
                return true;
            }
            else
            {
                this.TreeView.SelectedNode = null;
                return false;
            }
        }

        /// <summary>
        /// returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if match found</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplaced = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            if (this.Find(nodeText))
            {
                isNodeTextReplaced = Replace(replaceText);
                return isNodeTextReplaced;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText, TreeViewSearchOption searchOption)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, searchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeText, string replaceText)
        {
            bool isNodeTextReplace = this.Replace(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="ReplaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool Replace(string nodeReplaceText)
        {
            if (this.SearchTreeNodeAdvCollection != null && this.SearchTreeNodeAdvCollection.Count > 0 && this.TreeView.SelectedNode != null)
            {
                this.TreeView.RaiseNodeReplacingEvent(this.TreeView.SelectedNode, this.TreeView.SelectedNode.Text, nodeReplaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
                if (!this.TreeView.DisableReplacing)
                {
                    this.TreeView.SelectedNode.Text = nodeReplaceText;
                    this.TreeView.RaiseNodeReplacedEvent(this.TreeView.SelectedNode, this.TreeView.SelectedNode.Text, nodeReplaceText);
                }
                this.SearchTreeNodeAdvCollection.Remove(this.TreeView.SelectedNode);
                this.TreeView.SelectedNode = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchoption">TreeViewSearchOption</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchOption searchoption)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, searchoption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if all matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText)
        {
            bool isNodeTextReplace = this.ReplaceAll(nodeText, replaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
            return isNodeTextReplace;
        }

        /// <summary>
        /// Returns true if all matched TreeNodeAdv text replaced
        /// </summary>
        /// <param name="nodeText">Search Text</param>
        /// <param name="replaceText">Text to be replaced</param>
        /// <param name="searchOption">TreeViewSearchOption</param>
        /// <param name="searchRange">TreeViewSearchRange</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeText, string replaceText, TreeViewSearchOption searchOption, TreeViewSearchRange searchRange)
        {
            bool isNodeTextReplaced = false;
            this.TreeViewSearchOption = searchOption;
            this.TreeViewSearchRange = searchRange;
            if (this.FindAll(nodeText))
            {
                isNodeTextReplaced = ReplaceAll(replaceText);
                return isNodeTextReplaced;
            }
            return false;
        }

        /// <summary>
        /// Returns true if matched all TreeNodeAdv text replaced
        /// </summary>
        /// <param name="ReplaceText">Text to be replaced</param>
        /// <returns>returns true if matched TreeNodeAdv text replaced</returns>
        public bool ReplaceAll(string nodeReplaceText)
        {
            if (this.SearchTreeNodeAdvCollection != null && this.SearchTreeNodeAdvCollection.Count > 0 && this.TreeView.SelectedNode != null)
            {
                foreach (TreeNodeAdv node in SearchTreeNodeAdvCollection)
                {
                    this.TreeView.RaiseNodeReplacingEvent(node, node.Text, nodeReplaceText, this.TreeViewSearchOption, this.TreeViewSearchRange);
                    node.Text = nodeReplaceText;
                    this.TreeView.RaiseNodeReplacedEvent(node, node.Text, nodeReplaceText);
                }
                this.TreeView.SelectedNode = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Defines if Search Text matches any TreeNodeAdv text
        /// </summary>
        /// <param name="Text">Search String</param>
        /// <returns>returns true if match found</returns>
        internal bool Search(string Text)
        {
            this.FindNameInTreeView(this.TreeView, Text);
            if (this.SearchTreeNodeAdvCollection.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Find and return TreeNodeAdv based on TreeSearchOption
        /// </summary>
        /// <param name="node">TreeNodeAdv instances</param>
        /// <param name="name">Search Text</param>
        /// <returns>SearchTreeNodeAdvCollection</returns>
        internal TreeNodeAdv FindNameInTreeView(TreeNodeAdv node, String name)
        {
            TreeNodeAdv matchedNode = null;
            if (node == null)
                return null;

            switch (this.TreeViewSearchOption)
            {
                case TreeViewSearchOption.MatchCase:
                    if (node.Text == name)
                    {
                        isMatchFound = true;
                    }
                    break;
                case TreeViewSearchOption.MatchWholeText:
                    if (node.Text.ToLower() == (name.ToLower()) || name != string.Empty && node.Text.ToLower().StartsWith(name.ToLower()))
                    {
                        isMatchFound = true;
                    }
                    break;
            }

            if (node != null && isMatchFound)
            {
                if (this.TreeViewSearchRange == TreeViewSearchRange.ChildNode)
                {
                    if (node.Level > 1)
                        this.SearchTreeNodeAdvCollection.Add(node);
                }
                else
                {
                    this.SearchTreeNodeAdvCollection.Add(node);
                }
                isMatchFound = false;
            }

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                matchedNode = FindNameInTreeView(node.Nodes[i], name);
                if (matchedNode != null)
                    return matchedNode;
            }

            return null;
        }

        /// <summary>
        /// Find and return matched TreeNodeAdv Collection
        /// </summary>
        /// <param name="treeView">TreeViewAdv Instances</param>
        /// <param name="name">Search String</param>
        /// <returns>SearchTreeNodeAdvCollection</returns>
        internal TreeNodeAdv FindNameInTreeView(MultiColumnTreeView treeView, String name)
        {
            bool isNodePresent = false;
            if (treeView == null)
                return null;

            if (this.TreeViewSearchRange == TreeViewSearchRange.RootNode)
            {
                for (int i = 0; i < this.TreeView.Nodes.Count; i++)
                {
                    if (this.TreeViewSearchOption == TreeViewSearchOption.MatchCase)
                    {
                        if (this.TreeView.Nodes[i].Text == name)
                        {
                            isNodePresent = true;
                            this.SearchTreeNodeAdvCollection.Add(this.TreeView.Nodes[i]);
                        }
                    }
                    else if (this.TreeView.Nodes[i].Text.ToLower() == name.ToLower() || this.TreeView.Nodes[i].Text.ToLower().StartsWith(name))
                    {
                        isNodePresent = true;
                        this.SearchTreeNodeAdvCollection.Add(this.TreeView.Nodes[i]);
                    }
                }
            }

            if (this.TreeViewSearchRange == TreeViewSearchRange.TreeView
                || this.TreeViewSearchRange == TreeViewSearchRange.ChildNode)
            {
                for (int i = 0; i < treeView.Nodes.Count; i++)
                {
                    TreeNodeAdv matchedNode = FindNameInTreeView(treeView.Nodes[i], name);
                    if (matchedNode != null)
                    {
                        isNodePresent = true;
                        this.SearchTreeNodeAdvCollection.Add(matchedNode);
                    }
                }
            }

            if (isNodePresent)
            {
                isNodePresent = false;
                return this.SearchTreeNodeAdvCollection[0];
            }
            else
            {
                return null;
            }
        }
    }

    #endregion

}
