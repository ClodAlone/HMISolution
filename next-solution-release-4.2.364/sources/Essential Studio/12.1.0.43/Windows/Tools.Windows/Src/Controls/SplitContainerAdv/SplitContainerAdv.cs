#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Design;
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using Syncfusion.Windows.Forms.Design.Serialization;
#endif
using Syncfusion.Windows.Forms.Tools.Events;
using Syncfusion.Windows.Forms.Tools.Renderers;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools.Enums;
using System.Reflection;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// This control is container that consists of two panels, splitter between them 
    /// and allows user simply put other controls on these panels
    /// and drag splitter vertically or horizontally to resize these panels.
    /// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    [Docking(DockingBehavior.AutoDock)]
#endif
#if SyncfusionFramework1_0
	[ToolboxItem(false)]
#endif
    [
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	DesignerSerializer( typeof( SplitContainerAdvSerializer ), typeof( CodeDomSerializer ) ),
#endif
Designer(typeof(SplitContainerAdvDesigner)),
ToolboxBitmap(typeof(SplitContainerAdv), "ToolboxIcons.SplitContainerAdv.bmp"),
Description("Represents a control with collapsible panels and splitters.")
]
    public class SplitContainerAdv :
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		ContainerControl,
#else
 Control,
#endif
 IMouseHookHLProcClient,
        IMessageFilter,
        IVisualStyle,
        IThemedControl,
        ISupportInitialize
    {
        #region Class constants
        /// <summary>
        /// Default splitter encrement in pixels, when moving.
        /// </summary>
        private const int DEF_INCREMENT = 1;
        /// <summary>
        /// Default splitter position.
        /// </summary>
        private const int DEF_SPLITTER_DISTANCE = 48;
        /// <summary>
        /// Default splitter border width.
        /// </summary>
        private const int DEF_SPLITTER_BORDER_WIDTH = 1;
        /// <summary>
        /// Minimum splitter width allowed.
        /// </summary>
        private const int DEF_MIN_SPLITTER_WIDTH = 1;
        /// <summary>
        /// Minimum split panel's size allowed.
        /// </summary>
        private const int DEF_MIN_PANEL_SIZE = 25;
        /// <summary>
        /// Default splitter width.
        /// </summary>
        private const int DEF_SPLITTER_WIDTH = 7;
        /// <summary>
        /// Default container size.
        /// </summary>
        private static readonly Size DEF_SIZE = new Size(100, 100);
        /// <summary>
        /// Default name of first panel
        /// </summary>
        private const string DEF_PANEL1_NAME = "panel1";
        /// <summary>
        /// Default name of second panel
        /// </summary>
        private const string DEF_PANEL2_NAME = "panel2";
        private int fixedPanelHeight = 0;

        #endregion

        #region Class members
        /// <summary>
        /// A splitter offset.
        /// </summary>
        private Point m_splitterOffset = Point.Empty;
        /// <summary>
        /// Determines the number of pixels the splitter moves
        /// each increment.
        /// </summary>
        private int m_splitterIncrement = DEF_INCREMENT;
        /// <summary>
        /// Previously selected components at Design-Time before
        /// splitter moving.
        /// </summary>
        private ICollection m_prevSelectedComponents = null;
        /// <summary>
        /// Indicates, is container still initializing or not.
        /// </summary>
        private bool m_bIsInitializing = false;
        /// <summary>
        /// Hosted Form.
        /// </summary>
        private Form m_host = null;
        /// <summary>
        /// Splitter distance from left( top ) border.
        /// </summary>
        private int m_splitterDistance = (DEF_SIZE.Width - DEF_SPLITTER_WIDTH) / 2;
        /// <summary>
        /// 
        /// </summary>
        private int m_fixedDistance;
        /// <summary>
        /// Minimum panel1 size.
        /// </summary>
        private int m_panel1MinSize = DEF_MIN_PANEL_SIZE;
        /// <summary>
        /// Minimum panel2 size.
        /// </summary>
        private int m_panel2MinSize = DEF_MIN_PANEL_SIZE;
        /// <summary>
        /// Indicates, can user move splitter or not.
        /// </summary>
        private bool m_bIsSplitterFixed = false;
        /// <summary>
        /// Used for selecting panels bounds recalculation method
        /// during container resize, in case FixedPanel is not default.
        /// </summary>
        private bool m_bIsSizeChanging = false;
        /// <summary>
        /// Indicates, what panel size is fixed during container resize operations.
        /// </summary>
        private Syncfusion.Windows.Forms.Tools.Enums.FixedPanel m_fixedPanel = Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None;
        /// <summary>
        /// Split panels Border Style.
        /// </summary>
        private BorderStyle m_borderStyle = BorderStyle.None;
        /// <summary>
        /// Left( or top ) split panel.
        /// </summary>
        private SplitPanelAdv m_panel1 = null;
        /// <summary>
        /// Right ( or bottom ) split panel.
        /// </summary>
        private SplitPanelAdv m_panel2 = null;
        /// <summary>
        /// A panel which will be collapsed when some events on it occurs.
        /// </summary>
        private CollapsedPanel m_panelToBeCollapsed = CollapsedPanel.None;
        /// <summary>
        /// Used to check, if user is adding any controls to container manually,
        /// or container's split panels initialization in progress.
        /// </summary>
        private bool m_bInitializing = false;
        /// <summary>
        /// This panel is drawn highlited at design-time.
        /// </summary>
        private SplitPanelAdv m_selectedPanel = null;
        /// <summary>
        /// used for splitter drawing
        /// </summary>
        private Rectangle m_prevMovingRect = Rectangle.Empty;
        /// <summary>
        /// Previous cursor before splitter moving operation is stored here.
        /// </summary>
        private Cursor m_prevCursor = null;
        /// <summary>
        /// Previous control's size before it is changed.
        /// </summary>
        private Size m_prevSize = Size.Empty;
        /// <summary>
        /// Indicates, is spliter moving operation in progress, or not.
        /// </summary>
        private bool m_bIsSplitterMoving = false;
        /// <summary>
        /// Splitter rectangle, it is drawn in.
        /// </summary>
        public RectangleF m_splitterRect = new RectangleF();
        /// <summary>
        /// Splitter orientation( vertical or horisontal ).
        /// </summary>
        private Orientation m_orientation = Orientation.Horizontal;
        /// <summary>
        /// Renderer, which draws control.
        /// </summary>
        private IRenderer m_renderer = null;
        /// <summary>
        /// Renderer instance-specific information
        /// </summary>
        private IRendererInfo m_rendererInfo;
        /// <summary>
        /// Brush to draw background using gradient styles.
        /// </summary>
        private BrushInfo m_bgBrush = BrushInfo.Empty;
        /// <summary>
        /// Brush to draw background using gradient styles, while under mouse pointer.
        /// </summary>
        private BrushInfo m_bgHotBrush = BrushInfo.Empty;
        /// <summary>
        /// Indicates whether themes are enabled.
        /// </summary>
        private bool m_bThemesEnabled;
        /// <summary>
        /// Themes control drawing.
        /// </summary>
        private ThemedControlDrawing m_tcd;
        /// <summary>
        /// Indicates whether we should ignore theme background.
        /// </summary>
        private bool m_bIgnoreThemeBackground = false;
        /// <summary>
        /// To retain the panel background color at design time.
        /// </summary>        
        private bool ignorePanelBackgroundColor = false;
        /// <summary>
        /// Current control style.
        /// </summary>
        private Style m_eStyle = Style.Default;
        /// <summary>
        /// Current control draw state. We can draw regarding of state.
        /// </summary>
        private DrawState m_eDrawState = DrawState.Normal;
        /// <summary>
        /// Current control drag state. We can draw respectively to this state.
        /// </summary>
        private DragState m_eDragState = DragState.Normal;
        /// <summary>
        /// Brush which fills a thumbnail arrow (if one).
        /// </summary>
        private BrushInfo m_brushExpandFill = BrushInfo.Empty;
        /// <summary>
        /// Brush which fills a thumbnail arrow (if one), while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotExpandFill = BrushInfo.Empty;
        /// <summary>
        /// Brush which draws a thumbnail arrow.
        /// </summary>
        private Color m_colorExpandLine = Color.Empty;
        /// <summary>
        /// Brush which draws a thumbnail arrow, while under mouse cursor.
        /// </summary>
        private Color m_colorHotExpandLine = Color.Empty;
        /// <summary>
        /// If thumbnail exists and grip present, draws it.
        /// </summary>
        private BrushInfo m_brushGripDark = BrushInfo.Empty;
        /// <summary>
        /// If thumbnail exists and grip present, draws it, while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotGripDark = BrushInfo.Empty;
        /// <summary>
        /// If thumbnail exists and grip present, draws a shadow near it.
        /// </summary>
        private BrushInfo m_brushGripLight = BrushInfo.Empty;
        /// <summary>
        /// If thumbnail exists and grip present, draws a shadow near it, while under mouse cursor.
        /// </summary>
        private BrushInfo m_brushHotGripLight = BrushInfo.Empty;
        /// <summary>
        /// An event which leads to collapsing of previously specified panel.
        /// </summary>
        private TogglePanelOn m_togglePanelOn;
        /// <summary>
        /// Indicates whether the panel is toggled.
        /// </summary>
        private bool m_bPanelToggled = false;
        /// <summary>
        /// If calculating of panel bounds in CollapsePanel function.
        /// </summary>
        private bool m_bCalculateAfterCollapsing = false;
        /// <summary>
        /// Indicates that calculating of panel bounds in OnSplitterDistanceChanged function.
        /// </summary>
        private bool m_bCalculateAfterSplitterDistanceChange = false;
        /// <summary>
        /// Ratio of the splitContainer width(height) to panel1 width(height).
        /// </summary>
        private double m_ratioSplitterDistance = 1;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private int CTRLSIZE = default(int);

        #endregion

        #region Class events
        /// <summary>
        /// Occurs, when splitter is moved to new position.
        /// </summary>
        [Category("Actions")]
        [Description("Occurs, when splitter is moved to new position.")]
        public event SplitterMoveEventHandler SplitterMoved;
        /// <summary>
        /// Occurs, while splitter is moving.
        /// </summary>
        [Category("Actions")]
        [Description("Occurs, while splitter is moving.")]
        public event SplitterMoveEventHandler SplitterMoving;
        /// <summary>
        /// Occurs, when splitter orientation is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs, when splitter orientation is changed.")]
        public event EventHandler OrientationChanged;
        /// <summary>
        /// Occurs, when splitter theme is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs, when splitter theme is changed.")]
        public event EventHandler ThemeChanged;
        #endregion

        #region Class properties

        /// <summary>
        /// Gets Hosted Form.
        /// </summary>
        protected Form Host
        {
            get
            {
                return m_host;
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;
                switch (value)
                {
                    case "Office2007Blue":
                        Style = Style.Office2007Blue;
                        break;
                    case "Office2007Silver":
                        Style = Style.Office2007Silver;
                        break;
                    case "Office2007Black":
                        Style = Style.Office2007Black;
                        break;
                    case "Managed":
                    case "VS2005":
                        Style = Style.VS2005;
                        break;
                    case "Default":
                        Style = Style.Default;
                        break;
                    case "Mozilla":
                        Style = Style.Mozilla;
                        break;
                    case "None":
                        Style = Style.None;
                        break;
                    case "Office2003":
                        Style = Style.Office2003;
                        break;
                   }
            }
        }
        /// <summary>
        /// Gets or Sets Panel1 minimum size.
        /// </summary>
        [
        Browsable(true),
        Category("Layout"),
        Description("Gets or Sets Panel1 minimum size."),
        DefaultValue(DEF_MIN_PANEL_SIZE),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public int Panel1MinSize
        {
            get
            {
                return m_panel1MinSize;
            }
            set
            {
                if (value != m_panel1MinSize)
                {
                    // check if new minimum panel size is valid. If not,
                    // throw exception.
                    if (!m_bIsInitializing)
                    {
                        int maxSize = (this.Orientation == Orientation.Horizontal) ? this.Width : this.Height;
                        maxSize -= (m_panel2MinSize + this.SplitterWidth);

                        if (value < 0 || value > maxSize)
                        {
                            throw new ArgumentOutOfRangeException("Panel1MinSize", value, " Panel1 minimum size is greater then container size.");
                        }
                    }

                    m_panel1MinSize = value;

                    // correct panels bounds according to newly set panel1 minimum size.
                    if (!m_bIsInitializing)
                    {
                        CorrectPanelsBounds();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets Panel2 minimum size.
        /// </summary>
        [
        Browsable(true),
        Category("Layout"),
        Description("Gets or Sets Panel2 minimum size."),
        DefaultValue(DEF_MIN_PANEL_SIZE),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public int Panel2MinSize
        {
            get
            {
                return m_panel2MinSize;
            }
            set
            {
                if (value != m_panel2MinSize)
                {
                    // check if new minimum panel size is valid. If not,
                    // throw exception.
                    if (!m_bIsInitializing)
                    {
                        int maxSize = (Orientation == Orientation.Horizontal) ? this.Width : this.Height;
                        maxSize -= (m_panel1MinSize + SplitterWidth);

                        if (value < 0 || value > maxSize)
                        {
                            throw new ArgumentOutOfRangeException("Panel2MinSize", value, " Panel2 minimum size is greater then container size.");
                        }
                    }

                    m_panel2MinSize = value;

                    // correct panels bounds according to newly set panel2 minimum size.
                    if (!m_bIsInitializing)
                    {
                        CorrectPanelsBounds();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets, whether user is allowed to move splitter or not.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or Sets, whether user is allowed to move splitter or not.")]
        [DefaultValue(false)]
        public bool IsSplitterFixed
        {
            get
            {
                return m_bIsSplitterFixed;
            }
            set
            {
                if (value != m_bIsSplitterFixed)
                {
                    m_bIsSplitterFixed = value;
                    OnIsSplitterFixedChanged();
                }
            }
        }

        /// <summary>
        /// Determines the number of pixels the splitter moves in
        /// each increment.
        /// </summary>
        [
        Description("Determines the number of pixels the splitter moves in each increment."),
        DefaultValue(DEF_INCREMENT),
        Category("Layout")
        ]
        public int SplitterIncrement
        {
            get
            {
                return m_splitterIncrement;
            }
            set
            {
                if (value != m_splitterIncrement)
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException("value");

                    m_splitterIncrement = value;
                }
            }
        }

        /// <summary>
        /// Gets or Sets splitter distance from left( top ) border.
        /// </summary>
        [
        Description("Gets or Sets splitter distance from left( top ) border."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        RefreshProperties(RefreshProperties.All),
        DefaultValue(DEF_SPLITTER_DISTANCE),
        Category("Layout")
        ]
        public int SplitterDistance
        {
            get
            {
                return m_splitterDistance;
            }
            set
            {
                if (value != m_splitterDistance)
                {
                    m_splitterDistance = value;

                    OnSplitterDistanceChanged();
                }
            }
        }

        /// <summary>
        /// Gets or Sets, which panel has fixed size during container resizing.
        /// </summary>
        [
        Category("Layout"),
        Description("Gets or Sets, which panel has fixed size during container resizing."),
        DefaultValue(typeof(Syncfusion.Windows.Forms.Tools.Enums.FixedPanel), "None")
        ]
        public Syncfusion.Windows.Forms.Tools.Enums.FixedPanel FixedPanel
        {
            get
            {
                return m_fixedPanel;
            }
            set
            {
                if (value != m_fixedPanel)
                {
                    m_fixedPanel = value;
                    if (m_fixedPanel != Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None)
                    {
                        fixedPanelHeight = (m_fixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1) ? m_panel1.Height : m_panel2.Height;
                    }
                }
            }
        }
        private Syncfusion.Windows.Forms.Tools.Enums.FixedPanel FixedPanelInternal
        {
            get
            {
                if (m_panel1.Collapsed || m_panel2.Collapsed)
                {
                    return Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None;
                }
                return m_fixedPanel;
            }
        }

        /// <summary>
        /// Gets the collection of controls contained within this control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Control.ControlCollection Controls
        {
            get
            {
                return base.Controls;
            }
        }

        /// <summary>
        /// Gets minimum container Width ( or Height, in Vertical Orientation case  )
        /// allowed.
        /// </summary>
        protected
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
 new
#endif
 int MinimumSize
        {
            get
            {
                int minSize = 1;

                switch (this.FixedPanelInternal)
                {
                    case Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1:
                        minSize = (this.Orientation == Orientation.Horizontal) ? m_panel1.Width :
                            m_panel1.Height;

                        minSize += m_panel2MinSize;
                        break;

                    case Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel2:
                        minSize = (this.Orientation == Orientation.Horizontal) ? m_panel2.Width :
                            m_panel2.Height;

                        minSize += m_panel1MinSize;
                        break;

                    case Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None:
                        minSize = m_panel1MinSize + m_panel2MinSize;
                        break;
                }

                return minSize + this.SplitterWidth;
            }
        }

        /// <summary>
        /// Gets or Sets split panel, drawn highlighted at design-time.
        /// </summary>
        protected internal SplitPanelAdv SelectedPanel
        {
            get
            {
                return m_selectedPanel;
            }
            set
            {
                if (value != m_selectedPanel)
                {
                    m_selectedPanel = value;

                    // redraw panels after new one is selected due to
                    // selection frame redraw.
                    m_panel1.Invalidate();
                    m_panel2.Invalidate();
                }
            }
        }

        /// <summary>
        /// Indicates, is splitter moving operation is in progress or not. Readonly.
        /// </summary>
        protected internal bool IsSplitterMoving
        {
            get
            {
                return m_bIsSplitterMoving;
            }
        }

        /// <summary>
        /// Gets or Sets splitter width.
        /// </summary>
        [
        Description(" Gets or Sets splitter width."),
        DefaultValue(DEF_SPLITTER_WIDTH),
        Category("Layout")
        ]
        public int SplitterWidth
        {
            get
            {
                float width = (this.Orientation == Orientation.Horizontal) ?
                    m_splitterRect.Width : m_splitterRect.Height;

                return (int)width;
            }
            set
            {
                int prevSplitterWidth = (this.Orientation == Orientation.Horizontal) ?
                    (int)m_splitterRect.Width : (int)m_splitterRect.Height;

                int width = (this.Orientation == Orientation.Horizontal) ?
                    this.Width : this.Height;

                if (value != prevSplitterWidth)
                {
                    // check, if value is valid
                    if (!m_bInitializing && (value < DEF_MIN_SPLITTER_WIDTH ||
                        value + m_panel1MinSize + m_panel2MinSize > width))
                    {
                        throw new ArgumentOutOfRangeException("SplitterWidth", value, "Is lesser then minimum size allowed.");
                    }

                    if (Orientation == Orientation.Horizontal)
                    {
                        m_splitterRect.Width = value;
                    }
                    else
                    {
                        m_splitterRect.Height = value;
                    }

                    if (!m_bInitializing)
                    {
                        OnSplitterWidthChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the horizontal or vertical
        /// orientation of the SplitContainerAdv panels.
        /// </summary>
        [
        DefaultValue(typeof(Orientation), "Horizontal"),
        RefreshProperties(RefreshProperties.All),
        Description("Gets or sets a value indicating the horizontal or vertical orientation of the SplitContainerAdv panels."),
        Category("Behavior")
        ]
        public Orientation Orientation
        {
            get
            {
                return m_orientation;
            }
            set
            {
                if (value != m_orientation)
                {
                    m_orientation = value;

                    OnOrientationChanged();
                }
            }
        }

        /// <summary>
        /// Gets left( top ) split panel.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The left or top panel in SplitContainerAdv."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public SplitPanelAdv Panel1
        {
            get
            {
                return m_panel1;
            }
        }

        /// <summary>
        /// Gets right( bottom ) split panel.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("The right or bottom panel in SplitContainerAdv."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
        ]
        public SplitPanelAdv Panel2
        {
            get
            {
                return m_panel2;
            }
        }
        /// <summary>
        ///  Gets or Sets, whether Panel1 is collapsed or not.
        /// </summary>
        [
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false),
        Browsable(true),
        Category("Layout"),
        Description("This determines if Panel1 is collapsed.")
        ]
        public bool Panel1Collapsed
        {
            get
            {
                return m_panel1.Collapsed;
            }
            set
            {
                if (value != m_panel1.Collapsed)
                {
                    CollapsePanel(m_panel1, value);
                }
            }
        }

        /// <summary>
        /// Gets or Sets, whether Panel2 is collapsed.
        /// </summary>
        [
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(false),
        Browsable(true),
        Category("Layout"),
        Description("This determines if Panel2 is collapsed.")
        ]
        public bool Panel2Collapsed
        {
            get
            {
                return m_panel2.Collapsed;
            }
            set
            {
                if (value != m_panel2.Collapsed)
                {
                    CollapsePanel(m_panel2, value);
                }
            }
        }
        /// <summary>
        /// Gets or Sets currently collapsed panel.
        /// </summary>
        [
        Browsable(false),
        Description("Gets or Sets currently collapsed panel."),
        Category("Layout"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(CollapsedPanel.None)
        ]
        public CollapsedPanel CollapsedPanel
        {
            get
            {
                CollapsedPanel collapsedPanel = CollapsedPanel.None;
                if (null != m_panel1 && m_panel1.Collapsed)
                {
                    collapsedPanel = CollapsedPanel.Panel1;
                }
                else if (null != m_panel2 && m_panel2.Collapsed)
                {
                    collapsedPanel = CollapsedPanel.Panel2;
                }
                return collapsedPanel;

            }
            set
            {
                switch (value)
                {
                    case CollapsedPanel.Panel1:
                        {
                            CollapsePanel(m_panel1, true);
                            m_eDragState = DragState.Collapsed;
                            break;
                        }
                    case CollapsedPanel.Panel2:
                        {
                            CollapsePanel(m_panel2, true);
                            m_eDragState = DragState.Collapsed;
                            break;
                        }
                    case CollapsedPanel.None:
                        {
                            CollapsePanel(m_panel1, false);
                            CollapsePanel(m_panel2, false);
                            m_eDragState = DragState.Normal;
                            break;
                        }
                }
                UpdateRendererInfo();
            }
        }

        /// <summary>
        /// Gets or Sets the panel to be collapsed when some predefined event occurs on it.
        /// </summary>
        [
        Browsable(true),
        Description("Gets or Sets the panel to be collapsed when some predefined event occurs on it."),
        Category("Behavior"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(CollapsedPanel.None)
        ]
        public CollapsedPanel PanelToBeCollapsed
        {
            get
            {
                return m_panelToBeCollapsed;
            }
            set
            {
                if (value != m_panelToBeCollapsed)
                {
                    m_panelToBeCollapsed = value;
                    UpdateRendererInfo();
                    InvalidateSplitter();
                }
            }
        }

        /// <summary>
        /// An event which leads to collapsing of previously specified panel.
        /// </summary>
        [
        Browsable(true),
        Description("An event which leads to collapsing of previously specified panel."),
        Category("Behavior"),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(TogglePanelOn.Click)
        ]
        public TogglePanelOn TogglePanelOn
        {
            get
            {
                return m_togglePanelOn;
            }
            set
            {
                if (value != m_togglePanelOn)
                {
                    m_togglePanelOn = value;
                }
            }
        }
        /// <summary>
        /// Determines split panels border style.
        /// </summary>
        [Category("Appearance")]
        [Description("Determines split panels border style.")]
        [DefaultValue(typeof(BorderStyle), "None")]
        public BorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }
            set
            {
                if (value != m_borderStyle)
                {
                    m_borderStyle = value;

                    OnBorderStyleChanged();
                }
            }
        }

        /// <summary>
        /// The background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        /// <remarks>
        /// The SplitContainerAdv control provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance - Styles"),
        Description("Lets you set the background color, gradient, etc.")
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
                    m_bgBrush = value;
                    Invalidate();
                }
            }
        }
        /// <summary>
        /// The hot (under mouse cursor) background color, gradient and other styles can be set through 
        /// this property.
        /// </summary>
        /// <remarks>
        /// The SplitContainerAdv control provides this property to enable specialized
        /// custom gradient backgrounds.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        Category("Appearance - Styles"),
        Description("Lets you set the background color, gradient, etc., while under mouse cursor"),
        ]
        public BrushInfo HotBackgroundColor
        {
            get
            {
                return m_bgHotBrush;
            }

            set
            {
                if (m_bgHotBrush != value)
                {
                    m_bgHotBrush = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Indicates whether the control is themed.
        /// </summary>
        [Description("Indicates if the control is themed.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ThemesEnabled
        {
            get
            {
                return m_bThemesEnabled;
            }
            set
            {
                if (m_bThemesEnabled != value)
                {
                    m_bThemesEnabled = value;
                    UpdateStyles();
                    InvalidateWindow();
                    OnThemeChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Indicates whether the control will ignore the theme's background color and draw the backcolor instead.
        /// </summary>
        [Description("Indicates if the control will ignore the theme's background color and draw the backcolor instead.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IgnoreThemeBackground
        {
            get
            {
                return m_bIgnoreThemeBackground;
            }
            set
            {
                if (m_bIgnoreThemeBackground != value)
                {
                    m_bIgnoreThemeBackground = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the <see cref="Style"/> <see cref="BackgroundColor"/> and Panel.BackgroundColor will be used instead. 
        /// </summary>
        [Description("Gets or sets a value indicating whether to ignore the Style BackgroundColor and Panel.BackgroundColor will be used instead.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool IgnorePanelBackgroundColor
        {
            get
            {
                return ignorePanelBackgroundColor;
            }
            set
            {
                if (ignorePanelBackgroundColor != value)
                {
                    ignorePanelBackgroundColor = value;
                    Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets the Themed control.
        /// </summary>
        [Browsable(false)]
        public ThemedControlDrawing ThemedControl
        {
            get
            {
                return m_tcd;
            }
        }

        /// <summary>
        /// Indicates the current style of the control.
        /// </summary>
        [
        Description("Indicates the current style of the control."),
        Category("Appearance"),
        DefaultValue(Style.None),
        RefreshProperties(RefreshProperties.All)
        ]
        public Style Style
        {
            get
            {
                return m_eStyle;
            }
            set
            {
                if (m_eStyle != value)
                {
                    m_eStyle = value;
                    OnStyleChanged();

                    Update();
                }
            }
        }
        /// <summary>
        /// Sets and gets current control state.
        /// </summary>
        protected internal DrawState DrawState
        {
            get
            {
                return m_eDrawState;
            }
            set
            {
                if (m_eDrawState != value)
                {
                    m_eDrawState = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Sets and gets current control movement state.
        /// </summary>
        protected internal DragState DragState
        {
            get
            {
                return m_eDragState;
            }
            set
            {
                if (m_eDragState != value)
                {
                    m_eDragState = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets brush details for filling thumbnail arrows.
        /// </summary>
        [Description("Specifies the brush details for filling thumbnail arrows.")]
        [Category("Appearance - Styles")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BrushInfo ExpandFill
        {
            get
            {
                return m_brushExpandFill;
            }
            set
            {
                if (m_brushExpandFill != value)
                {
                    m_brushExpandFill = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set brush for filling thumbnail arrows, while under mouse cursor.
        /// </summary>
        [Description("Makes able to set brush for filling thumbnail arrows, while under mouse cursor.")]
        [Category("Appearance - Styles")]
        public BrushInfo HotExpandFill
        {
            get
            {
                return m_brushHotExpandFill;
            }
            set
            {
                if (m_brushHotExpandFill != value)
                {
                    m_brushHotExpandFill = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set pen color for drawing thumbnail arrows.
        /// </summary>
        [Description("Makes able to set pen color for drawing thumbnail arrows.")]
        [Category("Appearance - Styles")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ExpandLine
        {
            get
            {
                return m_colorExpandLine;
            }
            set
            {
                if (m_colorExpandLine != value)
                {
                    m_colorExpandLine = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set pen color for drawing thumbnail arrows, while under mouse cursor.
        /// </summary>
        [Description("Makes able to set pen color for drawing thumbnail arrows, while under mouse cursor.")]
        [Category("Appearance - Styles")]
        public Color HotExpandLine
        {
            get
            {
                return m_colorHotExpandLine;
            }
            set
            {
                if (m_colorHotExpandLine != value)
                {
                    m_colorHotExpandLine = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set brush for drawing a shadow around grip in thumbnail, if any.
        /// </summary>
        [Description("Makes able to set brush for drawing a grip in thumbnail, if any.")]
        [Category("Appearance - Styles")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public BrushInfo GripDark
        {
            get
            {
                return m_brushGripDark;
            }
            set
            {
                if (m_brushGripDark != value)
                {
                    m_brushGripDark = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set brush for drawing a shadow around grip in thumbnail, if any, while under mouse cursor.
        /// </summary>
        [Description("Makes able to set brush for drawing a shadow around grip in thumbnail, if any, while under mouse cursor.")]
        [Category("Appearance - Styles")]
        public BrushInfo HotGripDark
        {
            get
            {
                return m_brushHotGripDark;
            }
            set
            {
                if (m_brushHotGripDark != value)
                {
                    m_brushHotGripDark = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set brush for drawing a grip in thumbnail, if any.
        /// </summary>
        [Description("Makes able to set brush for drawing a shadow around a grip in thumbnail, if any.")]
        [Category("Appearance - Styles")]
        public BrushInfo GripLight
        {
            get
            {
                return m_brushGripLight;
            }
            set
            {
                if (m_brushGripLight != value)
                {
                    m_brushGripLight = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Makes able to set brush for drawing a grip in thumbnail, if any, while under mouse cursor.
        /// </summary>
        [Description("Makes able to set brush for drawing a grip in thumbnail, if any, while under mouse cursor.")]
        [Category("Appearance - Styles")]
        public BrushInfo HotGripLight
        {
            get
            {
                return m_brushHotGripLight;
            }
            set
            {
                if (m_brushHotGripLight != value)
                {
                    m_brushHotGripLight = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gives readonly access to container's RenderInfo.
        /// </summary>
        [Browsable(false)]
        public IRendererInfo RenderInfo
        {
            get
            {
                return m_rendererInfo;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the container enables the user to
        /// scroll to any controls placed outside of its visible boundaries.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gets a value indicating whether the 
          container enables the user to scroll to any controls placed outside of its visible boundaries.")]
        [Obsolete("Don't use this property. Use instead SpitContainerAdv.Panel.AutoScroll property.")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public new bool AutoScroll
#else
        public bool AutoScroll
#endif
        {
            get
            {
                return false;
            }

            set
            {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				base.AutoScroll = false;
#endif
            }
        }

        /// <summary>
        /// Gets the size of the auto-scroll margin.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gets the size of the auto-scroll margin.")]
        [Obsolete("Don't use this property. Use instead SpitContainerAdv.Panel.AutoScrollMargin property.")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public new Size AutoScrollMargin
#else
        public Size AutoScrollMargin
#endif
        {
            get
            {
                return Size.Empty;
            }
            set
            {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                base.AutoScrollMargin = Size.Empty;
#endif
            }
        }

        /// <summary>
        /// Gets the minimum size of the auto-scroll.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gets the minimum size of the auto-scroll.")]
        [Obsolete("Don't use this property. Use instead SpitContainerAdv.Panel.AutoScrollMinSize property.")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public new Size AutoScrollMinSize
#else
        public Size AutoScrollMinSize
#endif
        {
            get
            {
                return Size.Empty;
            }
            set
            {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				base.AutoScrollMinSize = Size.Empty;
#endif
            }
        }

        /// <summary>
        /// Gets the location of the auto-scroll position.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("Don't use this property. Use instead SpitContainerAdv.Panel.AutoScrollPosition property.")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        public new Point AutoScrollPosition
#else
        public Point AutoScrollPosition
#endif
        {
            get
            {
                return Point.Empty;
            }
            set
            {
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				base.AutoScrollPosition = Point.Empty;
#endif
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Gets where this control is scrolled to 
        /// in System.Windows.Forms.ScrollableControl.ScrollControlIntoView( System.Windows.Forms.Control ).
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"Gets where this control is scrolled to 
          in System.Windows.Forms.ScrollableControl.ScrollControlIntoView( System.Windows.Forms.Control ).")]
        [Obsolete("Don't use this property. Use instead SpitContainerAdv.Panel.AutoScrollOffset property.")]
        public new Point AutoScrollOffset
        {
            get
            {
                return Point.Empty;
            }
            set
            {
                base.AutoScrollOffset = Point.Empty;
            }
        }
#endif

        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SplitContainerAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(SplitContainerAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            InitializeComponent();

            // allow using transparent background
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // set repaint on resize
            base.SetStyle(ControlStyles.ResizeRedraw, true);

            // optimize painting( reduce flicker )
            base.SetStyle(ControlStyles.AllPaintingInWmPaint |
                WhidbeyCompatibleControlStyles.DoubleBuffer, true);
            //Fix for issue #12583
            base.SetStyle(ControlStyles.ContainerControl, true);

            m_bInitializing = true;

            InitializePanels();
            InitializeSplitter();

            base.Dock = DockStyle.None;
            this.Size = new Size(150, 100);

            base.ParentChanged += new EventHandler(OnParentChanged);

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
            {
                m_tcd = new ThemedControlDrawing(ThemedControls.EDIT, this);
            }
            m_bInitializing = false;
        }
        #region For Touch

        bool isScaling = false;
        [Browsable(false)]
        public int BeforeTouchSize
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
        ///Gets or Sets the touchmode
        /// </summary>
		[DefaultValue(false)]
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
        ///Applies scaling control
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.SplitterWidth = (int)(CTRLSIZE * scaleFactor);
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel)
                {
                    foreach (Control ctrl1 in (ctrl as Panel).Controls)
                    {
                        PropertyInfo fi = ctrl1.GetType().GetProperty("EnableTouchMode");
                        if (fi != null)
                            fi.SetValue(ctrl1, this.EnableTouchMode, null);
                    }
                }
            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion
        /// <summary>
        /// Initialize splitter width and height.
        /// </summary>
        private void InitializeSplitter()
        {
            m_prevSize = DEF_SIZE;
            Size panelSize = new Size((DEF_SIZE.Width - DEF_SPLITTER_WIDTH) / 2, DEF_SIZE.Height);

            // calculate splitter rectangle
            m_splitterRect.Location = new Point(panelSize.Width, 0);
            SplitterWidth = DEF_SPLITTER_WIDTH;
            CTRLSIZE = SplitterWidth;
            m_splitterRect.Height = panelSize.Height;
        }

        /// <summary>
        /// Initialzies container and split panels
        /// </summary>
        private void InitializePanels()
        {
            // initialize panel1
            m_panel1 = new SplitPanelAdv(this);
            m_panel1.Name = "SplitPanel1";
            m_panel1.Bounds = Rectangle.Round(new RectangleF(0, 0, m_splitterRect.Left, m_splitterRect.Height));

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			m_panel1.UniqueName = DEF_PANEL1_NAME; 
#endif

            // initialize panel2
            m_panel2 = new SplitPanelAdv(this);
            m_panel2.Name = "SplitPanel2";
            m_panel2.Bounds = Rectangle.Round(new RectangleF(m_splitterRect.Right, 0, Width - m_splitterRect.Width, m_splitterRect.Height));

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			m_panel2.UniqueName = DEF_PANEL2_NAME; 
#endif

            this.Controls.Add(m_panel1);
            this.Controls.Add(m_panel2);
        }

        /// <summary>
        /// free child controls
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            Syncfusion.Runtime.InteropServices.NativeMethods.ReleaseCapture();

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                base.ParentChanged -= new EventHandler(OnParentChanged);

                UnSubscribeHost();

                if (m_tcd != null)
                {
                    m_tcd.Dispose();
                    m_tcd = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Designer required variables
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // set current style the default one.
            this.Style = Style.None;
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Invoked when splitter is moved.
        /// </summary>
        /// <param name="args">Arguments related to this event.</param>
        protected virtual void OnSplitterMoved(SplitterMoveEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (SplitterMoved != null)
            {
                SplitterMoved(this, args);
                m_eDragState = DragState.Normal;
            }
        }

        /// <summary>
        /// Invoked when splitter is being moved.
        /// </summary>
        /// <param name="args">Arguments related to this event.</param>
        protected virtual void OnSplitterMoving(SplitterMoveEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            if (SplitterMoving != null)
            {
                m_eDragState = DragState.Dragged;
                SplitterMoving(this, args);
            }
        }

        /// <summary>
        /// Invoked when splitter is changing its fixed state.
        /// </summary>
        protected virtual void OnIsSplitterFixedChanged()
        {
            // cancel splitter moving, if in move mode, 
            // and splitter is fixed
            if (m_bIsSplitterFixed && m_bIsSplitterMoving)
            {
                FinishSplitterMoving(false);
            }
        }

        /// <summary>
        /// Invoked when splitter is changing its distance.
        /// </summary>
        protected virtual void OnSplitterDistanceChanged()
        {
            int width = (Orientation == Orientation.Horizontal) ? this.Width : this.Height;

            m_splitterDistance = (m_splitterDistance < m_panel1MinSize) ?
                m_panel1MinSize : m_splitterDistance;

            int maxSplitDistance = width - m_panel2MinSize - SplitterWidth;

            m_splitterDistance = (m_splitterDistance > maxSplitDistance) ?
                maxSplitDistance : m_splitterDistance;

            if (Orientation == Orientation.Horizontal)
            {
                m_splitterRect.X = m_splitterDistance;
            }
            else
            {
                m_splitterRect.Y = m_splitterDistance;
            }

            m_bCalculateAfterSplitterDistanceChange = true;

            RecalculatePanelsBounds();

            m_bCalculateAfterSplitterDistanceChange = false;

            if (Orientation == Orientation.Horizontal)
            {
                if (this.Panel1.Width != 0)
                {
                    m_ratioSplitterDistance = (double)base.Width / (double)Panel1.Width;
                }
            }
            else
            {
                if (this.Panel1.Height != 0)
                {
                    m_ratioSplitterDistance = (double)base.Height / (double)Panel1.Height;
                }
            }
        }

        /// <summary>
        /// Invoked when splitter changed its border.
        /// </summary>
        protected virtual void OnBorderStyleChanged()
        {
            m_panel1.SetBorderStyle(m_borderStyle);
            m_panel2.SetBorderStyle(m_borderStyle);
        }

        /// <summary>
        /// Invoked when splitter changed its width.
        /// </summary>
        protected virtual void OnSplitterWidthChanged()
        {
            RecalculatePanelsBounds();
            if (!EnableTouchMode && CTRLSIZE != this.SplitterWidth)
            {
                CTRLSIZE = this.SplitterWidth;
            }
            InvalidateSplitter();
            //TODO: add event rising here
        }

        /// <summary>
        /// Invoked when splitter changed its orientation.
        /// </summary>
        protected virtual void OnOrientationChanged()
        {
            if (Size.Width == 0 || Size.Height == 0)
            {
                Size = m_prevSize;
            }
            float coef = 1;
            float splitterPos = 0;

            switch (Orientation)
            {
                // calclulate split panels size and new split position
                // for horisontal orientation
                case Orientation.Horizontal:
                    coef = m_splitterRect.Top / this.Height;

                    m_splitterRect.Width = m_splitterRect.Height;
                    m_splitterRect.Height = this.Height;

                    splitterPos = coef * this.Width;
                    m_splitterRect.Location = new PointF(splitterPos, 0);

                    Panel1.Height = this.Height;
                    Panel1.Width = (int)m_splitterRect.Left;

                    Panel2.Height = this.Height;
                    Panel2.Width = this.Width - (int)m_splitterRect.Right;
                    Panel2.Location = new Point((int)m_splitterRect.Right, 0);

                    m_splitterDistance = (int)m_splitterRect.X;
                    break;

                // calclulate split panels size and new split position
                // for vertical orientation
                case Orientation.Vertical:
                    coef = m_splitterRect.Left / Width;

                    m_splitterRect.Height = m_splitterRect.Width;
                    m_splitterRect.Width = Width;

                    splitterPos = coef * base.Height;
                    m_splitterRect.Location = new PointF(0, splitterPos);

                    Panel1.Height = (int)m_splitterRect.Top;
                    Panel1.Width = Width;

                    Panel2.Height = Height - (int)m_splitterRect.Bottom;
                    Panel2.Width = Width;
                    Panel2.Location = new Point(0, (int)m_splitterRect.Bottom);

                    m_splitterDistance = (int)m_splitterRect.Y;
                    break;
            }

            // fire orientation changed event
            RaiseOrientationChanged();
            CorrectPanelsBounds();
            UpdateRendererInfo();
            Invalidate();
        }

        /// <summary>
        /// Invoked when mouse grabs the splitter.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Clicks == 1)
            {
                if (!m_panel1.Collapsed && !m_panel2.Collapsed)
                {
                    StartSplitterMoving(new Point(e.X, e.Y));
                }
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Invoked when mouse is moving over splitter. 
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point mousePos = new Point(e.X, e.Y);

            SetCursor(mousePos);

            MoveSplitterTo(mousePos);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when mouse leaves the splitter.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            ResetPreviousCursor();

            m_eDrawState = DrawState.Normal;
            UpdateRendererInfo();
            Invalidate();
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Invoked when mouse enters splitter region.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            SetSplitterCursor();

            m_eDrawState = DrawState.Hovered;
            Invalidate();
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Invoked when mouse releases splitter. 
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!m_panel1.Collapsed && !m_panel2.Collapsed)
            {
                FinishSplitterMoving(true);
            }
            else
            {
                m_bIsSplitterMoving = false;
            }

            Invalidate();

            base.OnMouseUp(e);
        }

        /// <override/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Rectangle curBounds = this.Bounds;

            if (Orientation == Orientation.Horizontal)
            {
                if (width < MinimumSize)
                {
                    width = MinimumSize;
                }
            }
            else
            {
                if (height < MinimumSize)
                {
                    height = MinimumSize;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);

            m_bIsSizeChanging = true;

            if (m_prevSize != Size)
            {
                // calculate new panels bounds and splitter position 
                // for new size
                RecalculatePanelsBounds();

                if (FixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        m_splitterDistance = (int)Math.Floor((double)base.Width / m_ratioSplitterDistance);
                    }
                    else
                    {
                        m_splitterDistance = (int)Math.Floor((double)base.Height / m_ratioSplitterDistance);
                    }
                }

                m_prevSize = Size;

                // correct panels bounds and splitter position,
                // if they are out of minimum panels size range
                CorrectPanelsBounds();

                m_bIsSizeChanging = false;
            }
            m_bIsSizeChanging = false;
        }


        /// <summary>
        /// A standard OnPaint message handler.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw focused spliter rectangle, if not design mode
            if (m_renderer == null || Height <= 0 || Width <= 0)
                return;

            base.OnPaint(e);

            // Draw splitter if control hasn't background image else draw background image will be displayed
            if (this.BackgroundImage == null)
            {
                Rectangle splitterFocusedRect = Rectangle.Round(m_splitterRect);
                // This code add border to splitter.
                //splitterFocusedRect.Inflate( -DEF_SPLITTER_BORDER_WIDTH,	-DEF_SPLITTER_BORDER_WIDTH );

                IncreaseBounds(ref splitterFocusedRect);
                m_renderer.UpdateRendererInfo(this, m_rendererInfo);
                m_renderer.Draw(e, m_rendererInfo, splitterFocusedRect);
            }
            if (!this.Panel1Collapsed && !this.Panel2Collapsed)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (this.Size.Width != this.Panel1.Size.Width
                        || this.Size.Width != this.Panel2.Size.Width)
                        this.RecalculatePanelsBounds();
                }
                else
                {
                    if (this.Size.Height != this.Panel1.Size.Height
                        || this.Size.Height != this.Panel2.Size.Height)
                        this.RecalculatePanelsBounds();
                }
            }

            this.RaisePaintEvent(this, e);
        }
        /// <summary>
        /// Increases bounds.
        /// </summary>
        /// <param name="bounds"></param>
        protected void IncreaseBounds(ref Rectangle bounds)
        {
            bounds.X -= 1;
            bounds.Y -= 1;
            bounds.Width += 1;
            bounds.Height += 1;
        }
        /// <summary>
        /// Invalidates window.
        /// </summary>
        private void InvalidateWindow()
        {
            Invalidate();
        }

        /// <summary>
        /// Invoked when splitter theme is changed.
        /// </summary>
        protected virtual void OnThemeChanged(EventArgs e)
        {
            if (ThemeChanged != null)
            {
                ThemeChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked when a control is removed from the splitter container.
        /// </summary>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (!e.Control.Disposing)
            {
                throw new ArgumentException("User is not allowed to remove any controls  from this control manually");
            }
        }

        /// <summary>
        /// Invoked when a control is added to the splitter container.
        /// </summary>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            Control controlToAdd = e.Control;

            // prevent user from manual controls adding.
            if (!this.Contains(controlToAdd) && controlToAdd != m_panel1 &&
                controlToAdd != m_panel2)
            {
                throw new ArgumentException("User is not allowed to add any controls to this control manually");
            }

            base.OnControlAdded(e);
        }

        /// <summary>
        /// Invokes when mouse clicks the splitter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (this.PanelToBeCollapsed == Enums.CollapsedPanel.Panel1)
            {
                m_bPanelToggled = this.Panel1Collapsed;
            }
            else if (this.PanelToBeCollapsed == Enums.CollapsedPanel.Panel2)
            {
                m_bPanelToggled = this.Panel2Collapsed;
            }
            if (m_bPanelToggled && m_togglePanelOn == TogglePanelOn.Click)
            {
                ExpandPanel();
                SetFixedPanelHeight();
            }
            else if (m_togglePanelOn == TogglePanelOn.Click)
            {
                if(this.FixedPanel != Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None)
                    fixedPanelHeight = (this.FixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1) ? m_panel1.Height : m_panel2.Height;
                TogglePanel();
            }

            base.OnClick(e);
        }

        private void SetFixedPanelHeight()
        {
            if (this.FixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel2)
                m_panel2.Height = fixedPanelHeight;
            else if (this.FixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1)
                m_panel1.Height = fixedPanelHeight;
            RecalculateFixedPanelsBounds();
        }

        /// <summary>
        /// Invoked when mouse double clicks the splitter
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (m_bPanelToggled && m_togglePanelOn == TogglePanelOn.DoubleClick)
            {
                ExpandPanel();
                SetFixedPanelHeight();
            }
            else if (m_togglePanelOn == TogglePanelOn.DoubleClick)
            {
                if (this.FixedPanel != Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.None)
                    fixedPanelHeight = (this.FixedPanel == Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1) ? m_panel1.Height : m_panel2.Height;
                TogglePanel();
            }

            base.OnDoubleClick(e);
        }
        /// <summary>
        /// Invalidate splitter when SystemColor changed.
        /// </summary>
        protected override void OnSystemColorsChanged(EventArgs e)
        {
            base.OnSystemColorsChanged(e);
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            if(Orientation == Orientation.Horizontal)
                RecalculatePanelsBounds();
            
            base.OnRightToLeftChanged(e);
        }
        #endregion

        #region Splitter Moving Logic
        /// <summary>
        /// Start splitter moving operation.
        /// </summary>
        private void StartSplitterMoving(Point mousePos)
        {
            if (!m_bIsSplitterFixed)
            {
                if (m_bIsSplitterMoving)
                {
                    FinishSplitterMoving(true);
                }

                if (m_splitterRect.Contains(mousePos))
                {
                    // turn on splitter moving
                    m_bIsSplitterMoving = true;
                    System.Diagnostics.Debug.WriteLine("TRUE", "StartSplitterMoving");

                    // calculate intitial splitter rectangle during it's moving operation
                    m_prevMovingRect = Rectangle.Round(m_splitterRect);

                    m_splitterOffset.X = mousePos.X - m_prevMovingRect.X;
                    m_splitterOffset.Y = mousePos.Y - m_prevMovingRect.Y;

                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            m_prevMovingRect.Location = PointToScreen(new Point(m_prevMovingRect.Location.X, 0));
                            break;

                        case Orientation.Vertical:
                            m_prevMovingRect.Location = PointToScreen(new Point(0, m_prevMovingRect.Location.Y));
                            break;
                    }

                    // draw split rectangle at initial position
                    DrawSplitter(m_prevMovingRect);
                }
            }
        }

        /// <summary>
        /// Corrects an increment.
        /// </summary>
        /// <param name="destPoint"> Destination point to move splitter to. </param>
        /// <param name="srcPoint"> Source point splitter is moving from. </param>
        /// <param name="increment"> Number of pixels for one step. </param>
        /// <returns> Corrected location. </returns>
        private Point CorrectIncrement(Point destPoint, Point srcPoint, int increment)
        {
            if (increment <= 0)
                throw new ArgumentOutOfRangeException("incement");

            bool bIsHorisontal = (this.Orientation == Orientation.Horizontal);

            int distance = 0;
            int reminder = 0;

            if (Orientation == Orientation.Horizontal)
            {
                distance = Math.Abs(destPoint.X - srcPoint.X);

                int sign = (destPoint.X > srcPoint.X) ? 1 : -1;
#if !SyncfusionFramework1_0
                int quotient = Math.DivRem(distance, increment, out reminder);
                destPoint.X = srcPoint.X + quotient * sign * increment;
#endif
            }
            else
            {
                distance = Math.Abs(destPoint.Y - srcPoint.Y);
                int sign = (destPoint.Y > srcPoint.Y) ? 1 : -1;
#if !SyncfusionFramework1_0
                int quotient = Math.DivRem(distance, increment, out reminder);
                destPoint.Y = srcPoint.Y + quotient * sign * increment;
#endif
            }

            return destPoint;
        }

        /// <summary>
        /// Checks, if splitter can be moved to spevified location,
        /// according to minimum size, splitter size and orientation.
        /// </summary>
        /// <param name="destination"> Destination point to move splitter to. 
        /// Point is in container client coordinates.</param>
        /// <returns> True, if can move spliter to destination, otherwise - false. </returns>
        private bool CanMoveTo(Point destination)
        {
            bool bCanMoveTo = false;

            int maxSize = (this.Orientation == Orientation.Horizontal) ?
                this.Width : this.Height;

            maxSize -= (m_panel2MinSize + SplitterWidth);

            int destPoint = (this.Orientation == Orientation.Horizontal) ?
                destination.X : destination.Y;

            bCanMoveTo = (destPoint > m_panel1MinSize &&
                destPoint < maxSize);

            return bCanMoveTo;

        }

        /// <summary>
        /// Moves splitter to specified point.
        /// </summary>
        protected void MoveSplitterTo(Point mousePos)
        {
            if (m_bIsSplitterMoving && !m_bIsSplitterFixed)
            {
                Rectangle rect = Rectangle.Round(m_splitterRect);

                // calculate new splitter location
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        int rectX = mousePos.X - m_splitterOffset.X;
                        rect.Location = new Point(rectX, 0);
                        break;

                    case Orientation.Vertical:
                        int rectY = mousePos.Y - m_splitterOffset.Y;
                        rect.Location = new Point(0, rectY);
                        break;
                }

                Point prevLocation = rect.Location;

                // Correct new splitter location according to pixels increment 
                // count per step.
                Point startPoint = Point.Round(m_splitterRect.Location);
                Point endPoint = rect.Location;
                rect.Location = CorrectIncrement(endPoint, startPoint, this.SplitterIncrement);

                // check if destination point to move splitter to satisfies
                // panel's min size.
                bool bCanMoveTo = CanMoveTo(rect.Location);

                // correct spliter rectangle location, if not valid one's
                if (!bCanMoveTo)
                {
                    rect.Location = CorrectSplitRectLocation(rect.Location);
                }

                // to draw spliter rectangle, must be in screen coordinates
                rect.Location = PointToScreen(rect.Location);

                if (m_prevMovingRect != rect)
                {
                    // clear previous drawn split rectangle
                    DrawSplitter(m_prevMovingRect);

                    // store newly moved split rectangle
                    m_prevMovingRect = rect;

                    // draw split rectangle at new position
                    DrawSplitter(rect);
                }

                SplitterMoveEventArgs args = new SplitterMoveEventArgs(prevLocation, rect.Location);
                OnSplitterMoving(args);
            }
        }

        /// <summary>
        /// Correct Splitter Location according to minimul panels size
        /// and mouse position.
        /// </summary>
        /// <param name="pt"> Splitter location to correct. </param>
        /// <returns> Corrected Splitter location. </returns>
        private Point CorrectSplitRectLocation(Point pt)
        {
            Point location = pt;

            int maxSize = m_panel2MinSize + SplitterWidth;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (location.X < m_panel1MinSize)
                    {
                        while (location.X < Panel1MinSize)
                        {
                            location.X += SplitterIncrement;
                        }
                    }
                    else if (location.X > this.Width - maxSize)
                    {
                        maxSize = this.Width - maxSize;
                        while (location.X > maxSize)
                        {
                            location.X -= SplitterIncrement;
                        }
                    }
                    break;

                case Orientation.Vertical:
                    if (location.Y < m_panel1MinSize)
                    {
                        while (location.Y < Panel1MinSize)
                        {
                            location.Y += SplitterIncrement;
                        }
                    }
                    else if (location.Y > this.Height - maxSize)
                    {
                        maxSize = this.Height - maxSize;
                        while (location.Y > maxSize)
                        {
                            location.Y -= SplitterIncrement;
                        }
                    }
                    break;
            }

            return location;
        }

        /// <summary>
        /// Finish splitter moving operation.
        /// </summary>
        /// <param name="success"> If success is true, move splitter to new position,
        /// otherwise - restore previous. </param>
        protected internal void FinishSplitterMoving(bool success)
        {
            if (m_bIsSplitterMoving && !m_bIsSplitterFixed)
            {
                PointF prevLocation = m_splitterRect.Location;

                // store new splitter bounds, if successed,
                // or restore previous bounds, if canceled
                m_splitterRect.Location = success ?
                    PointToClient(m_prevMovingRect.Location) :
                    m_splitterRect.Location;

                // turn off splitter moving 
                m_bIsSplitterMoving = false;
                System.Diagnostics.Debug.WriteLine("FALSE", "FinishSplitterMoving");

                // clear splitter rectangle
                DrawSplitter(m_prevMovingRect);

                // set newly pannel's bounds
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.SplitterDistance = (this.RightToLeft == RightToLeft.Yes) ? this.Width - (int)m_splitterRect.Right : (int)m_splitterRect.X;
                }
                else
                {
                    this.SplitterDistance = (int)m_splitterRect.Y;
                }

                // reduce flickering
                if (DesignMode)
                {
                    m_panel1.Refresh();
                    m_panel2.Refresh();
                }

                InvalidateSplitter();

                SplitterMoveEventArgs args = new SplitterMoveEventArgs(prevLocation, m_splitterRect.Location);
                OnSplitterMoved(args);

                m_splitterOffset = Point.Empty;
            }
        }

        #endregion

        #region Cursors change processing
        /// <summary>
        /// Sets splitter cursor.
        /// </summary>
        private void SetSplitterCursor()
        {
            // remember previous cursor
            if (m_prevCursor == null)
            {
                m_prevCursor = Cursor.Current;
            }

            // set splitter cursor
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    if (Cursor.Current != Cursors.VSplit)
                    {
                        Cursor.Current = Cursors.VSplit;
                    }
                    break;
                case Orientation.Vertical:
                    if (Cursor.Current != Cursors.HSplit)
                    {
                        Cursor.Current = Cursors.HSplit;
                    }
                    break;
            }
        }

        /// <summary>
        /// Restores previous cursor before split moving operations.
        /// </summary>
        private void ResetPreviousCursor()
        {
            if (Cursor.Current != m_prevCursor)
            {
                Cursor.Current = m_prevCursor;
            }
        }

        /// <summary>
        /// Set appropriate cursor.
        /// </summary>
        /// <param name="mousePos"></param>
        private void SetCursor(Point mousePos)
        {
            // set split cursor
            if (m_splitterRect.Contains(mousePos) && !this.IsSplitterFixed)
            {
                SetSplitterCursor();
            }
            // restore previous cursor, if not splitter is moving
            else if (!m_bIsSplitterMoving)
            {
                ResetPreviousCursor();
            }
        }

        #endregion

        #region Class utility methods
        /// <summary>
        /// Invalidates splitter region.
        /// </summary>
        private void InvalidateSplitter()
        {
            base.Invalidate(Rectangle.Round(m_splitterRect));
        }

        /// <summary>
        /// Subscribes hosted form for events.
        /// </summary>
        private void SubscribeHost()
        {
            if (m_host != null)
            {
                m_host.Deactivate += new EventHandler(HostDeactivate);
            }
        }

        /// <summary>
        /// Unsubscribes hosted form from events.
        /// </summary>
        private void UnSubscribeHost()
        {
            if (m_host != null)
            {
                m_host.Deactivate -= new EventHandler(HostDeactivate);
            }
        }

        /// <summary>
        /// Sets collapsed state for specified panel.
        /// </summary>
        private void CollapsePanel(SplitPanelAdv panel, bool bCollapsed)
        {
            if (this.FixedPanel != Enums.FixedPanel.None && this.CollapsedPanel == CollapsedPanel.None)
            {
                m_fixedDistance = m_splitterDistance;
            }

            if (panel != null)
            {
                SplitPanelAdv otherPanel = (panel == this.Panel2) ? this.Panel1 :
                  this.Panel2;

                // if both panels are collapsed, 
                // show other panel.
                if (otherPanel.Collapsed && bCollapsed)
                {
                    otherPanel.Collapsed = !bCollapsed;
                }

                panel.Collapsed = bCollapsed;

            }
            else
            {
                m_panel1.Collapsed = false;
                m_panel2.Collapsed = false;
            }

            if (this.FixedPanel != Enums.FixedPanel.None && this.CollapsedPanel == CollapsedPanel.None)
            {
                m_splitterDistance = m_fixedDistance;
            }

            m_bCalculateAfterCollapsing = true;

            RecalculatePanelsBounds();

            m_bCalculateAfterCollapsing = false;

            UpdateRendererInfo();
        }

        /// <summary>
        /// Draws splitter rectangle.
        /// </summary>
        /// <param name="splitterRect"> Rectangle to draw splitter in. </param>
        private void DrawSplitter(Rectangle splitterRect)
        {
            HalftonePainter.DrawRectangle(splitterRect);
        }

        /// <summary>
        /// Recalculates panels bounds, if  one of them is collapsed.
        /// </summary>
        /// <returns> true, if recalculated succesfully, othervise - false. </returns>
        private bool RecalculateCollapsedPanelsBounds()
        {
            bool bRecalculated = false;

            if (Orientation == Orientation.Horizontal)
            {
                m_splitterRect.Height = this.Height;

                // resize panel2 to client container area
                if (m_panel1.Collapsed)
                {
                    m_splitterRect.Location = Point.Empty;
                    m_panel2.Location = new Point((int)m_splitterRect.Width, 0);
                    m_panel2.Width = this.Width - (int)m_splitterRect.Width;
                    m_panel2.Height = this.Height;
                    m_panel1.Width = 0;

                    bRecalculated = true;
                }

                    // resize panel1 to client container area
                else if (m_panel2.Collapsed)
                {
                    m_splitterRect.Location = new Point((int)(this.Width - m_splitterRect.Width), 0);
                    m_panel1.Location = Point.Empty;
                    m_panel1.Width = this.Width - (int)m_splitterRect.Width;
                    m_panel1.Height = this.Height;
                    m_panel2.Width = 0;

                    bRecalculated = true;
                }
                else
                {
                    m_splitterRect.Location = new Point(m_splitterDistance, 0);
                }
            }
            else
            {
                m_splitterRect.Width = this.Width;

                // resize panel2 to client container area
                if (m_panel1.Collapsed)
                {
                    m_splitterRect.Location = Point.Empty;
                    m_panel2.Location = new Point(0, (int)m_splitterRect.Height);
                    m_panel2.Width = this.Width;
                    m_panel2.Height = this.Height - (int)m_splitterRect.Height;
                    m_panel1.Height = 0;

                    bRecalculated = true;
                }

                // resize panel1 to client container area
                else if (m_panel2.Collapsed)
                {
                    m_splitterRect.Location = new Point(0, (int)(this.Height - m_splitterRect.Height));
                    m_panel1.Location = Point.Empty;
                    m_panel1.Width = this.Width;
                    m_panel1.Height = this.Height - (int)m_splitterRect.Height;
                    m_panel2.Height = 0;

                    bRecalculated = true;
                }
                else
                {
                    m_splitterRect.Location = new Point(0, m_splitterDistance);
                }
            }

            InvalidateSplitter();

            return bRecalculated;
        }
        /// <summary>
        /// Recalculates panels bounds according to container size
        /// and fixed panel.
        /// </summary>
        /// <returns> true, if recalculated succesfully, othervise - false. </returns>
        private bool RecalculateFixedPanelsBounds()
        {
            bool bRecalculated = false;

            switch (this.FixedPanelInternal)
            {
                // do not change panel1 size, only resize panel2 
                case Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel1:
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (this.RightToLeft == RightToLeft.No)
                            {
                                m_panel1.Left = 0;
                                m_panel1.Width = m_splitterDistance;

                                m_splitterRect.X = m_panel1.Right;

                                m_panel2.Left = (int)m_splitterRect.Right;
                                m_panel2.Width = this.Width - m_panel2.Left;

                            }
                            else
                            {
                                m_panel1.Width = m_splitterDistance;
                                m_panel1.Left = this.Width - m_splitterDistance;

                                m_splitterRect.X = m_panel1.Left - this.SplitterWidth;

                                m_panel2.Left = 0;
                                m_panel2.Width = (int)m_splitterRect.Left;
                            }

                            m_panel1.Height = m_panel2.Height = Height;

                            if (this.Panel1.Width != 0)
                            {
                                m_ratioSplitterDistance = (double)base.Width / (double)Panel1.Width;
                            }
                        }
                        else
                        {
                            m_panel2.Height = this.Height - (int)m_splitterRect.Bottom;
                            m_panel2.Top = (int)m_splitterRect.Bottom;
                            m_panel1.Width = m_panel2.Width = this.Width;
                            m_panel1.Height = (int)m_splitterRect.Top;
                            if (this.Panel1.Height != 0)
                            {
                                m_ratioSplitterDistance = (double)base.Height / (double)Panel1.Height;
                            }
                        }
                        bRecalculated = true;
                    }
                    break;

                // do not change panel2 size, only resize panel1
                case Syncfusion.Windows.Forms.Tools.Enums.FixedPanel.Panel2:
                    {
                        if (Orientation == Orientation.Horizontal)
                        {
                            if (!m_bCalculateAfterCollapsing && !m_bCalculateAfterSplitterDistanceChange)
                            {
                                m_splitterRect.X = this.Width - m_panel2.Width - SplitterWidth;
                                m_splitterDistance = (int)m_splitterRect.X;
                            }
                            m_panel1.Width = (int)m_splitterRect.X;
                            m_panel2.Left = (int)m_splitterRect.Right;
                            m_panel1.Height = m_panel2.Height = this.Height;
                            m_panel2.Width = this.Width - (int)m_splitterRect.Right;
                            if (this.Panel1.Width != 0)
                            {
                                m_ratioSplitterDistance = (double)base.Width / (double)Panel1.Width;
                            }
                        }
                        else
                        {
                            if (!m_bCalculateAfterCollapsing && !m_bCalculateAfterSplitterDistanceChange)
                            {
                                m_splitterRect.Y = this.Height - m_panel2.Height - SplitterWidth;
                                m_splitterDistance = (int)m_splitterRect.Y;
                            }
                            else
                            {
                                m_splitterRect.Y = this.Height - fixedPanelHeight - SplitterWidth;
                                m_splitterDistance = (int)m_splitterRect.Y;
                            }
                            
                            m_panel1.Height = (int)m_splitterRect.Y;
                            m_panel2.Top = (int)m_splitterRect.Bottom;
                            m_panel1.Width = m_panel2.Width = this.Width;
                            m_panel2.Height = this.Height - (int)m_splitterRect.Bottom;
                            if (this.Panel1.Height != 0)
                            {
                                m_ratioSplitterDistance = (double)base.Height / (double)Panel1.Height;
                            }
                        }

                        bRecalculated = true;
                    }
                    break;
            }

            return bRecalculated;
        }

        /// <summary>
        /// Recalculates split panels bounds proportionally.
        /// </summary>
        private void RecalculatePanelsBoundsCommon()
        {
            switch (m_orientation)
            {
                case Orientation.Horizontal:
                    {
                        float coef = (float)(Width) / m_prevSize.Width;
                        float splitterPos = coef * m_splitterDistance;

                        Panel1.Top = 0;
                        Panel2.Top = 0;

                        if (this.RightToLeft == RightToLeft.No)
                        {
                            m_splitterRect.Location = new PointF(splitterPos, 0);

                            Panel1.Left = 0;
                            Panel1.Width = (int)m_splitterRect.Left;

                            Panel2.Left = (int)m_splitterRect.Right;
                            Panel2.Width = Width - (int)m_splitterRect.Right;
                        }
                        else
                        {
                            m_splitterRect.Location = new PointF(this.Width - m_splitterRect.Width - splitterPos, 0);

                            Panel1.Left = (int)m_splitterRect.Right;
                            Panel1.Width = Width - (int)m_splitterRect.Right;

                            Panel2.Left = 0;
                            Panel2.Width = (int)m_splitterRect.Left;
                        }

                        m_splitterRect.Height = Panel1.Height = Panel2.Height = Height;
                    }
                    break;

                case Orientation.Vertical:
                    {
                        float coef = (float)(Height) / m_prevSize.Height;
                        float splitterPos = coef * m_splitterRect.Top;

                        Panel1.Left = 0;
                        Panel2.Left = 0;

                        m_splitterRect.Location = new PointF(0, splitterPos);

                        if (this.Parent is DockHost && this.RightToLeft == RightToLeft.Yes)
                        {
                            m_splitterRect.Location = new PointF(0, this.Height - m_splitterRect.Height - splitterPos);
                        }

                        Panel1.Top = 0;
                        Panel1.Height = (int)m_splitterRect.Top;

                        Panel2.Top = (int)m_splitterRect.Bottom;
                        Panel2.Height = Height - (int)m_splitterRect.Bottom;

                        m_splitterRect.Width = Panel1.Width = Panel2.Width = Width;

                    }
                    break;
            }
        }
        
        /// <summary>
        /// Sets new split panels bounds according to splitter location,
        /// fixed and collapsed pannels.
        /// </summary>
        private void RecalculatePanelsBounds()
        {
            bool bRecalculated = RecalculateCollapsedPanelsBounds();

            if (!bRecalculated && m_bIsSizeChanging)
            {
                bRecalculated = RecalculateFixedPanelsBounds();

                if (bRecalculated)
                {
                    m_prevSize = Size;
                }
            }

            if (!bRecalculated)
            {
                RecalculatePanelsBoundsCommon();
            }
        }

        /// <summary>
        /// Corrects split panels bounds and splitter position, if 
        /// any of panels has size less then minimum panel size allowed.
        /// </summary>
        private void CorrectPanelsBounds()
        {
            int panel2Width = m_panel2MinSize + SplitterWidth;
            int distance = (this.Orientation == Orientation.Horizontal) ? this.Width : this.Height;

            // correct splitter distance
            //splitter rectangle will be corrected in RecalculateCollapsedPanelsBounds
            if (m_splitterDistance < m_panel1MinSize)
            {
                m_splitterDistance = m_panel1MinSize;
            }
            else if (m_splitterDistance > distance - m_panel2MinSize)
            {
                m_splitterDistance = distance - panel2Width;
            }

            // recalculate bounds after correction according to 
            // new splitter rectangle
            RecalculatePanelsBounds();
        }

        /// <summary>
        /// Updates current info on rendering information, basing on current <see href="SplitContainerAdv"/> settings.
        /// </summary>
        /// <remarks>
        /// This method must be called if any render relevant changes of
        /// the <see href="SplitContainerAdv" /> instance were made.
        /// </remarks>
        private void UpdateRendererInfo()
        {
            m_rendererInfo = m_renderer.UpdateRendererInfo(this, m_rendererInfo);
        }
        /// <summary>
        /// If parameter panel equal Collapsed panel than collapses this panel
        /// </summary>
        /// <param name="panel"> event sender </param>
        protected internal virtual void OnPanelClick(SplitPanelAdv panel)
        {
            /*
            if( m_togglePanelOn == TogglePanelOn.Click && CollapsedPanelToSplitPanelAdv(m_panelToBeCollapsed)== panel)
            {
              if( !m_bPanelToggled )
              {
                TogglePanel( );
              }
              else
              {
                ExpandPanel( );
              }
            }
            */
        }

        /// <summary>
        /// If parameter panel equal Collapsed panel and mode is DoubleClickMode than collapses this panel
        /// </summary>
        /// <param name="panel"> event sender </param>
        protected internal virtual void OnPanelDoubleClick(SplitPanelAdv panel)
        {
            /*
            if( m_togglePanelOn == TogglePanelOn.DoubleClick && CollapsedPanelToSplitPanelAdv( m_panelToBeCollapsed ) == panel )
            {
              if( !m_bPanelToggled )
              {
                TogglePanel( );
              }
              else
              {
                ExpandPanel( );
              }
            }
             */
        }

        /// <summary>
        /// Collapses a panel specified in m_panelToBeCollapsed, when specified event occurs.
        /// </summary>
        private void TogglePanel()
        {
            CollapsePanel(CollapsedPanelToSplitPanelAdv(m_panelToBeCollapsed), true);
            m_bPanelToggled = true;
        }

        /// <summary>
        /// Expands a panel specified in m_panelToBeCollapsed, when specified event occurs.
        /// </summary>
        private void ExpandPanel()
        {
            CollapsePanel(CollapsedPanelToSplitPanelAdv(m_panelToBeCollapsed), false);
            m_bPanelToggled = false;
        }

        /// <summary>
        /// Returns an instance of SplitPanelAdv, according to provided CollapsedPanel.
        /// </summary>
        /// <param name="panel">A CollapsedPanel enum member.</param>
        /// <returns>An appropriate SplitPanelAdv instance, already owned by this SplitContainerAdv, or null if panel == CollapsedPanel.None.</returns>
        private SplitPanelAdv CollapsedPanelToSplitPanelAdv(CollapsedPanel panel)
        {
            SplitPanelAdv splitPanelAdv = null;
            switch (panel)
            {
                case CollapsedPanel.Panel1:
                    splitPanelAdv = Panel1;
                    break;
                case CollapsedPanel.Panel2:
                    splitPanelAdv = Panel2;
                    break;
                case CollapsedPanel.None:
                    break;
            }
            return splitPanelAdv;
        }

        /// <summary>
        /// Retrieves a panel opposite to the collapsed one.
        /// </summary>
        /// <returns>A SplitPanelAdv instance.</returns>
        private SplitPanelAdv PanelOppositeToTheCollapsedOne()
        {
            SplitPanelAdv splitPanelAdv = null;
            switch (m_panelToBeCollapsed)
            {
                case CollapsedPanel.Panel1:
                    splitPanelAdv = Panel2;
                    break;
                case CollapsedPanel.Panel2:
                    splitPanelAdv = Panel1;
                    break;
                case CollapsedPanel.None:
                    break;
            }
            return splitPanelAdv;
        }

        #endregion

        #region Class codedom serialization

        #region BackGroundColor
        /// <summary>
        /// Resets backgroung color to the default value.
        /// </summary>
        private void ResetBackgroundColor()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.BackgroundColor);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_bgBrush = bi;
            Invalidate();
        }

        /// <summary>
        /// Should a control serialize its background color.
        /// </summary>
        /// <returns>true, if yes.</returns>
        protected bool ShouldSerializeBackgroundColor()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.BackgroundColor, m_bgBrush);
        }
        #endregion

        #region HotBacckgroundColor
        /// <summary>
        /// Resets hot backgroung color to the default value.
        /// </summary>
        private void ResetHotBackgroundColor()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.HotBackgroundColor);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_bgHotBrush = bi;
            Invalidate();
        }

        /// <summary>
        /// Should a control serialize its hot background color.
        /// </summary>
        /// <returns>true, if yes.</returns>
        protected bool ShouldSerializeHotBackgroundColor()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.HotBackgroundColor, m_bgHotBrush);
        }
        #endregion

        #region ExpandFill
        /// <summary>
        /// 
        /// </summary>
        private void ResetExpandFill()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.ExpandFill);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushExpandFill = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeExpandFill()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.ExpandFill, m_brushExpandFill);
        }
        #endregion

        #region ExpandLine
        /// <summary>
        /// 
        /// </summary>
        private void ResetExpandLine()
        {
            Color bi = Color.Transparent;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.ExpandLine);

                if (defValue is Color)
                {
                    bi = (Color)defValue;
                }
            }

            m_colorExpandLine = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeExpandLine()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.ExpandLine, m_colorExpandLine);
        }
        #endregion

        #region GripDark
        /// <summary>
        /// 
        /// </summary>
        private void ResetGripDark()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.GripDark);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushGripDark = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeGripDark()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.GripDark, m_brushGripDark);
        }
        #endregion

        #region GripLight
        /// <summary>
        /// 
        /// </summary>
        private void ResetGripLight()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.GripLight);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushGripLight = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeGripLight()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.GripLight, m_brushGripLight);
        }
        #endregion

        #region HotExpandFill
        /// <summary>
        /// 
        /// </summary>
        private void ResetHotExpandFill()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.HotExpandFill);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushHotExpandFill = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHotExpandFill()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.HotExpandFill, m_brushHotExpandFill);
        }
        #endregion

        #region HotExpandLine
        /// <summary>
        /// 
        /// </summary>
        private void ResetHotExpandLine()
        {
            Color bi = Color.Transparent;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.HotExpandLine);

                if (defValue is Color)
                {
                    bi = (Color)defValue;
                }
            }

            m_colorHotExpandLine = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHotExpandLine()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.HotExpandLine, m_colorExpandLine);
        }
        #endregion

        #region HotGripDark
        /// <summary>
        /// 
        /// </summary>
        private void ResetHotGripDark()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.HotGripDark);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushHotGripDark = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHotGripDark()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.HotGripDark, m_brushHotGripDark);
        }
        #endregion

        #region HotGripLight
        /// <summary>
        /// 
        /// </summary>
        private void ResetHotGripLight()
        {
            BrushInfo bi = BrushInfo.Empty;

            if (null != m_renderer)
            {
                object defValue = m_renderer.GetDefaultValue(RendererProperty.HotGripLight);

                if (defValue is BrushInfo)
                {
                    bi = (BrushInfo)defValue;
                }
            }

            m_brushHotGripLight = bi;
            Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ShouldSerializeHotGripLight()
        {
            if (m_renderer == null)
            {
                return true;
            }

            return !m_renderer.CompareWithDefaultValue(RendererProperty.HotGripLight, m_brushHotGripLight);
        }
        #endregion

        #endregion

        #region Class event's handlers
        /// <summary>
        /// Invoked when splitter style is being changed.
        /// </summary>
        private void OnStyleChanged()
        {
            m_renderer = Renderer.GetRenderer(m_eStyle);
            m_rendererInfo = m_renderer.GetAppropriateThemeSettings(this, m_bIsInitializing);
            if (this.Panel1 != null && this.Panel2 != null)
            {
                this.ChangePanelsBackground();
            }
        }

        private void ChangePanelsBackground()
        {
            Style style = this.Style;
            Color[] colors = new Color[1] { Color.Empty };
            BrushInfo bi = BrushInfo.Empty;
            if (!this.IgnorePanelBackgroundColor)
            {
                switch (style)
                {
                    case Style.Default:
                    case Style.Mozilla:
                    case Style.VS2005:
                        bi = new BrushInfo(Color.FromArgb(128, 128, 128));
                        break;

                    case Style.Office2003:
                    case Style.OfficeXP:
                        colors = new Color[2] { Color.FromArgb(198, 214, 233), Color.FromArgb(180, 194, 235) };
                        bi = new BrushInfo(GradientStyle.Vertical, colors);
                        break;

                    case Style.Office2007Black:
                        colors = new Color[2] { Color.FromArgb(146, 146, 146), Color.FromArgb(83, 83, 83) };
                        bi = new BrushInfo(GradientStyle.Vertical, colors);
                        break;
                    case Style.Office2007Blue:
                        colors = new Color[2] { Color.FromArgb(231, 242, 255), Color.FromArgb(179, 209, 252) };
                        bi = new BrushInfo(GradientStyle.Vertical, colors);
                        break;
                    case Style.Office2007Silver:
                        colors = new Color[2] { Color.FromArgb(223, 227, 231), Color.FromArgb(190, 194, 203) };
                        bi = new BrushInfo(GradientStyle.Vertical, colors);
                        break;
                }
                this.Panel1.BackgroundColor = bi;
                this.Panel2.BackgroundColor = bi;
            }
        }
        /// <summary>
        /// Invoked when object's parent is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParentChanged(object sender, EventArgs e)
        {
            // unsubscribe previous host from events;
            UnSubscribeHost();

            m_host = base.FindForm();

            // subsribe new host for events; 
            SubscribeHost();
        }

        /// <summary>
        /// Invoked when host is deactivated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostDeactivate(object sender, EventArgs e)
        {
            // cancel splitter moving operation when parent form lost focus
            FinishSplitterMoving(false);
        }
        #endregion

        #region Class events risers
        /// <summary>
        /// Raises OrientationChanged event.
        /// </summary>
        private void RaiseOrientationChanged()
        {
            if (OrientationChanged != null)
            {
                OrientationChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region IMessageFilter interface implementation
        /// <summary>
        /// This method is almost a stub, needed only for mouse hook subscribing.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref Message m)
        {
            // do nothing, needed just for mouse hooks subscribing.
            return false;
        }

        /// <summary>
        /// Unselectes components at design time.
        /// </summary>
        private void UnselectComponents()
        {
            if (DesignMode)
            {
                ISelectionService selService = (ISelectionService)GetService(typeof(ISelectionService));

                bool bIsSelected = (selService != null && selService.PrimarySelection == this);

                // if this container is selected, unselect components
                if (bIsSelected)
                {
                    m_prevSelectedComponents = selService.GetSelectedComponents();

                    selService.SetSelectedComponents(new Control[] { });
                }
            }
        }

        /// <summary>
        /// Restores previous selection at design-time.
        /// </summary>
        private void RestoreSelectedComponents()
        {
            ISelectionService selService = (ISelectionService)GetService(typeof(ISelectionService));

            if (selService != null)
            {
                bool bIsSelected = bIsSelected = (selService.PrimarySelection == this);

                if (!bIsSelected)
                {
                    selService.SetSelectedComponents(m_prevSelectedComponents);
                }
            }
        }

        ///<returns> True, is container selected, otherwise - false. </returns>
        private bool IsContainerSelected()
        {
            ISelectionService selService = (ISelectionService)GetService(typeof(ISelectionService));

            bool bIsSelected = (selService != null && selService.PrimarySelection == this);

            return bIsSelected;
        }

        /// <summary>
        /// For handling mouse messages in design mode.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="point"></param>
        /// <param name="hwnd"></param>
        /// <param name="wHitTestCode"></param>
        /// <param name="dwExtraInfo"></param>
        /// <returns></returns>
        /// <remarks>In design mode, mouse messages can be handled only here.</remarks>
        public bool MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
        {
            bool bIsMouseHandled = false;

            if (DesignMode)
            {
                Point mousePos = this.PointToClient(point);

                switch (msg)
                {
                    case NativeMethods.WM_LBUTTONDOWN:
                        if (m_splitterRect.Contains(mousePos))
                        {
                            // if container is not selected, just return
                            bool bIsSelected = IsContainerSelected();
                            if (bIsSelected)
                            {
                                NativeMethods.SetCapture(hwnd);
                                StartSplitterMoving(mousePos);
                                bIsMouseHandled = true;
                            }
                        }
                        break;

                    case NativeMethods.WM_MOUSEMOVE:
                        if (m_bIsSplitterMoving)
                        {
                            MoveSplitterTo(mousePos);
                            bIsMouseHandled = true;
                        }
                        break;

                    case NativeMethods.WM_LBUTTONUP:
                        if (m_bIsSplitterMoving)
                        {
                            //RestoreSelectedComponents();
                            FinishSplitterMoving(true);
                            NativeMethods.ReleaseCapture();
                            bIsMouseHandled = true;
                        }
                        break;
                }
            }

            return bIsMouseHandled;
        }

        #endregion

        #region IsupportInitialize interface implementation

        /// <summary>
        /// Begins control initializing.
        /// </summary>
        public virtual void BeginInit()
        {
            m_bIsInitializing = true;
            Panel1.SuspendLayout();
            Panel2.SuspendLayout();
        }

        /// <summary>
        /// Ends control initializing.
        /// </summary>
        public virtual void EndInit()
        {
            m_bIsInitializing = false;
            Panel1.ResumeLayout(false);
            Panel2.ResumeLayout(false);

            CorrectPanelsBounds();
        }
        #endregion
    }

}
