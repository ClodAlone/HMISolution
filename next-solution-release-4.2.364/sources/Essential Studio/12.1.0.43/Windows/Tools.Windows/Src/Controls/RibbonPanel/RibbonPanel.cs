#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control for managing layout of ToolStrips.
    /// </summary>
    [Designer(typeof(RibbonPanelDesigner))]
    [ToolboxItem(false)]
    public class RibbonPanel
        : ContainerControl, IOffice12Settings,IVisualStyle 
    {
        #region Constants
      
        /// <summary> Width of button that scrolling tab items. </summary>
        private const int SCROLLBARBUTTON_WIDTH = 12;
       
        /// <summary> Border width of Ribbon panel. </summary>
        private const int BORDER_WIDTH = 2;

        private const SetWindowPosFlags m_swpFlags =
            SetWindowPosFlags.SWP_NOZORDER |
            SetWindowPosFlags.SWP_NOSIZE |
            SetWindowPosFlags.SWP_NOMOVE |
            SetWindowPosFlags.SWP_NOACTIVATE |
            SetWindowPosFlags.SWP_FRAMECHANGED;

        /// <summary> Interval for timer. </summary>
        private const int TIMER_INT = 200;
        #endregion

        #region Fields
      
        /// <summary>
        /// Corresponding TabItem. Can be null if RibbonPanel is independent control.
        /// </summary>
        private ToolStripTabItem m_tabItem;
    
        private List<ToolStripEx> m_toolStripsExpanded;
       
        /// <summary> Position of rightmost tab Item. </summary>
        private int m_iScrollPosition = 0;
    
        private bool m_bIsRightScroll;
     
        private bool m_bIsLeftScroll;
       
        /// <summary> Indicates if right scroll button is selected. </summary>
        private bool m_bRightScrollSelected = false;
       
        /// <summary> Indicates if left scroll button is selected. </summary>
        private bool m_bLeftScrollSelected = false;
       
        /// <summary>
        /// Timer for handling mouse keeping pushed.
        /// </summary>
        private Timer m_timer;
      
        /// <summary> Interval for timer. </summary>
        private int m_timerInt = TIMER_INT;
        
        /// <summary> Currently pushed button. </summary>
        private ScrollButtonsArea m_pushedButton;
       
        private static Dictionary<ToolStripEx.ColorScheme, RibbonPanelRenderer> m_renderers;

        private static Dictionary<ToolStripEx.ColorScheme, Office2010RibbonPanelRenderer> office2010Renderers;

        private static Dictionary<ToolStripEx.ColorScheme, Office2013RibbonPanelRenderer> office2013Renderers;
        #region Fields For IOffice12Settings Implementation
        /// <summary>
        /// Launcher style.
        /// </summary>
        private LauncherStyle m_launcherStyle = LauncherStyle.Default;

        /// <summary>
        /// Indicates whether caption should be shown.
        /// </summary>
        private BoolEx m_bShowCaption = BoolEx.Default;

        /// <summary>
        /// Indicates whether launcher should be shown.
        /// </summary>
        private BoolEx m_bShowLauncher = BoolEx.Default;

        /// <summary>
        /// Caption style.
        /// </summary>
        private CaptionStyle m_captionStyle = CaptionStyle.Default;

        /// <summary>
        /// Caption text style.
        /// </summary>
        private CaptionTextStyle m_captionTextStyle = CaptionTextStyle.Default;

        /// <summary>
        /// Caption alignment.
        /// </summary>
        private CaptionAlignment m_captionAlignment = CaptionAlignment.Default;

        /// <summary>
        /// Caption font.
        /// </summary>
        private Font m_captionFont = null;

        /// <summary>
        /// Caption minimal height.
        /// </summary>
        private int m_nCaptionMinHeight = -1;

        /// <summary>
        /// Color scheme.
        /// </summary>
        private ToolStripEx.ColorScheme m_colorScheme = ToolStripEx.ColorScheme.Default;

        /// <summary>
        /// Border style.
        /// </summary>
        private ToolStripBorderStyle m_borderStyle = ToolStripBorderStyle.Default;
        #endregion

        #endregion

        #region Enums
        /// <summary>
        /// Different areas of the control.
        /// </summary>
        protected enum ScrollButtonsArea
        {
            /// <summary> Out of scroll buttons. </summary>
            None,

            /// <summary> Right scroll button. </summary>
            RightScrollButton,

            /// <summary> Left scroll button. </summary>
            LeftScrollButton
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets corresponding tab item.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripTabItem TabItem
        {
            get
            {
                return m_tabItem;
            }
            set
            {
                if (m_tabItem != value)
                {
                    m_tabItem = value;
                    InitTabItem();
                }
            }
        }

        /// <summary> Gets or sets position of rightmost tab Item. </summary>
        internal int ScrollPositionInternal
        {
            get
            {
                return m_iScrollPosition;
            }
            set
            {
                if (m_iScrollPosition != value)
                {
                    m_iScrollPosition = value;

                    UpdateFrame();
                    PerformLayout();
                }
            }
        }

        /// <summary> Gets or sets position of rightmost tab Item and transfer the value to ScrollPosition property. </summary>
        [Browsable(false)]
        public int ScrollPosition
        {
            get
            {
                return m_iScrollPosition;
            }
            set
            {
                ScrollPositionInternal = GetValidScrollPosition(value);
            }
        }

        /// <summary> Gets a value indicating whether if right scroll button must be located on Ribbon panel. </summary>
        internal bool IsRightScroll
        {
            get
            {
                return m_bIsRightScroll;
            }
        }

        /// <summary> Gets a value indicating whether if left scroll button must be located on Ribbon panel. </summary>
        internal bool IsLeftScroll
        {
            get
            {
                return m_bIsLeftScroll;
            }
        }

        /// <summary> Gets a value indicating whether if right scroll button is selected. </summary>
        internal bool RightScrollSelected
        {
            get
            {
                return m_bRightScrollSelected;
            }
        }

        /// <summary> Gets a value indicating whether if left scroll button is selected. </summary>
        internal bool LeftScrollSelected
        {
            get
            {
                return m_bLeftScrollSelected;
            }
        }

        /// <summary>
        ///  Gets or sets the space between controls.
        /// </summary>
        public new Padding Margin
        {
            get { return base.Margin; }
            set { base.Margin = value; }
        }
    
        protected virtual RibbonPanelRenderer Renderer
        {
            get
            {
                ToolStripEx.ColorScheme colorScheme = this.OfficeColorScheme;

                if (this.RibbonStyle == RibbonStyle.Office2007)
                {
                    if (!m_renderers.ContainsKey(colorScheme))
                    {
                        switch (colorScheme)
                        {
                            case ToolStripEx.ColorScheme.Managed:
                                m_renderers[colorScheme] = new RibbonPanelRenderer(Office12ColorTable.ManagedColors);
                                break;
                            case ToolStripEx.ColorScheme.Silver:
                                m_renderers[colorScheme] = new RibbonPanelRenderer(new Office12ColorTable());
                                break;
                            case ToolStripEx.ColorScheme.Blue:
                                m_renderers[colorScheme] = new RibbonPanelRenderer(new OfficeBlue());
                                break;
                            case ToolStripEx.ColorScheme.Black:
                                m_renderers[colorScheme] = new RibbonPanelRenderer(new OfficeBlack());
                                break;
                        }
                    }

                    return m_renderers[colorScheme];
                }
                else if (this.RibbonStyle == RibbonStyle.Office2013)
                {
                    if (!office2013Renderers.ContainsKey(colorScheme))
                    {
                        switch (colorScheme)
                        {
                            case ToolStripEx.ColorScheme.Managed:
                                office2013Renderers[colorScheme] = new Office2013RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Managed));
                                break;
                            case ToolStripEx.ColorScheme.Silver:
                                office2013Renderers[colorScheme] = new Office2013RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Silver));
                                break;
                            case ToolStripEx.ColorScheme.Blue:
                                office2013Renderers[colorScheme] = new Office2013RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Blue));
                                break;
                            case ToolStripEx.ColorScheme.Black:
                                office2013Renderers[colorScheme] = new Office2013RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Black));
                                break;
                        }
                    }

                    return office2013Renderers[colorScheme];
                }
                
                else
                {
                    if (!office2010Renderers.ContainsKey(colorScheme))
                    {
                        switch (colorScheme)
                        {
                            case ToolStripEx.ColorScheme.Managed:
                                office2010Renderers[colorScheme] = new Office2010RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Managed));
                                break;
                            case ToolStripEx.ColorScheme.Silver:
                                office2010Renderers[colorScheme] = new Office2010RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Silver));
                                break;
                            case ToolStripEx.ColorScheme.Blue:
                                office2010Renderers[colorScheme] = new Office2010RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Blue));
                                break;
                            case ToolStripEx.ColorScheme.Black:
                                office2010Renderers[colorScheme] = new Office2010RibbonPanelRenderer(new Office2010ColorTable(Office2010ColorScheme.Black));
                                break;
                        }
                    }

                    return office2010Renderers[colorScheme];
                }
            }
        }

        /// <summary> Gets or sets area in which user pushed mouse button. </summary>
        protected ScrollButtonsArea PushedButton
        {
            get
            {
                return m_pushedButton;
            }
            set
            {
                if (m_pushedButton != value)
                {
                    m_pushedButton = value;
                }
            }
        }

        /// <summary> Gets bounds of right scroll button. </summary>
        protected Rectangle RightScrollBounds
        {
            get
            {
                Rectangle rc = Rectangle.Empty;

                if (m_bIsRightScroll)
                {
                    rc = new Rectangle(this.DisplayRectangle.Width, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, this.Height);
                }

                return rc;
            }
        }

        /// <summary> Gets bounds of left scroll button. </summary>
        protected Rectangle LeftScrollBounds
        {
            get
            {
                Rectangle rc = Rectangle.Empty;

                if (m_bIsLeftScroll)
                {
                    int iWidth = SCROLLBARBUTTON_WIDTH + BORDER_WIDTH;

                    rc = new Rectangle(-iWidth, 0, iWidth, this.Height);
                }

                return rc;
            }
        }
    
        protected override Padding DefaultPadding
        {
            get
            {
                if (this.RibbonStyle == RibbonStyle.Office2013)
                {
                    if (this.RightToLeft == System.Windows.Forms.RightToLeft.No)
                        return new Padding(0, 1, 35, 0);
                    else
                        return new Padding(35, 1, 0, 0);
                }
                return new Padding(0, 1, 0, 0);
            }
        }

        /// <summary>
        /// Gets or sets the height and width of the control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the control relative
        ///     to the upper-left corner of its container.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Point Location
        {
            get { return base.Location; }
            set { base.Location = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its parent controls
        /// are displayed.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get { return base.Visible; }
            set { base.Visible = value; }
        }
     
        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (m_ribbonPanelLayout == null)
                {
                    m_ribbonPanelLayout = new RibbonPanelLayout(base.LayoutEngine);
                }
                return m_ribbonPanelLayout;
            }
        }
        #endregion

        #region Initialization
 
        static RibbonPanel()
        {
            m_renderers = new Dictionary<ToolStripEx.ColorScheme, RibbonPanelRenderer>();

            office2010Renderers = new Dictionary<ToolStripEx.ColorScheme, Office2010RibbonPanelRenderer>();
            office2013Renderers = new Dictionary<ToolStripEx.ColorScheme, Office2013RibbonPanelRenderer>();
        }

        /// <summary>
        /// Initializes a new instance of the RibbonPanel class.
        /// </summary>
        public RibbonPanel()
        {
            m_toolStripsExpanded = new List<ToolStripEx>();

            m_timer = new Timer();
            m_timer.Tick += new EventHandler(OnTimerTick);

            this.Margin = new Padding(4, 0, 4, 0);
            this.DoubleBuffered = true;
        }

        /// <summary>
        ///  Initializes a new instance of the RibbonPanel class.
        /// </summary>
        /// <param name="tabItem">Corresponding tab item.</param>
        public RibbonPanel(ToolStripTabItem tabItem)
            : this()
        {
            if (tabItem != null)
            {
                m_tabItem = tabItem;
                InitTabItem();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds new tool strip to the control.
        /// </summary>
        /// <param name="toolStrip">ToolStrip to be added.</param>
        public void AddToolStrip(ToolStripEx toolStrip)
        {
            toolStrip.AutoSize = false;
            toolStrip.Dock = DockStyle.None;
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;

            this.Controls.Add(toolStrip);
        }

        /// <summary>
        /// Redraws toolstrips.
        /// </summary>
        public void UpdateCaptions()
        {
            foreach (Control control in this.Controls)
            {
                ToolStripEx ts = control as ToolStripEx;
                if (ts != null)
                {
                    ts.UpdateCaption();
                }
            }
        }

        /// <summary>
        /// Redraws toolstrips using new renderers.
        /// </summary>
        public void UpdateRenderers()
        {
            foreach (Control control in this.Controls)
            {
                ToolStripEx ts = control as ToolStripEx;
                if (ts != null)
                {
                    ts.UpdateRenderer();
                }
            }
        }

        /// <summary>
        /// Redraws toolstrips using new renderers.
        /// </summary>
        public void UpdateRenderers(bool office12Mode)
        {
            foreach (Control control in this.Controls)
            {
                ToolStripEx ts = control as ToolStripEx;
                if (ts != null)
                {
                    if (office12Mode)
                        ts.Office12Mode = true;

                    ts.UpdateRenderer();
                }
            }
        }
        #endregion

        #region Overrides
     
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size size = Size.Empty;

            Size _size = new Size(1, 1);

            foreach (Control c in this.Controls)
            {
                Size sz = c.AutoSize ? c.GetPreferredSize(_size) : c.Size;

                size.Width += sz.Width;

                if (size.Height < sz.Height)
                {
                    size.Height = sz.Height;
                }
            }

            size.Width += this.Padding.Horizontal;
            size.Height += this.Padding.Vertical;

            if (size.Height < 25)
            {
                size.Height = 25;
            }

            size.Width += 2 * BORDER_WIDTH;
            size.Height += 2 * BORDER_WIDTH;

            return size;
        }

        /// <summary>
        /// Locates toolstrips according to RightToLeft value.
        /// </summary>
        /// <param name="pToolStrip"> ToolStripEx to be located. </param>
        /// <param name="pRightToLeft"> RightToLeft value. </param>
        /// <param name="pX"> Left value for ToolStripEx. </param>
        private void SetToolStripLocationRTL(ToolStripEx pToolStrip, RightToLeft pRightToLeft, int pX)
        {
            pToolStrip.Left = GetToolStripLocationRTL(pToolStrip, pRightToLeft, pX);
        }
     
        private int GetToolStripLocationRTL(ToolStripEx pToolStrip, RightToLeft pRightToLeft, int pX)
        {
            if (pRightToLeft == RightToLeft.Yes)
            {
                return this.DisplayRectangle.Right - pX - pToolStrip.Width;
            }
            return this.DisplayRectangle.X + pX;
        }

        /// <summary>
        /// Updates toolstrips appearance.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (!this.Disposing)
            {
                UpdateCaptions();
                UpdateRenderers();
            }
        }
     
        protected override void OnPaintBackground(PaintEventArgs pe)
        {
            RibbonPanelRenderer renderer = this.Renderer;

            if (renderer != null)
            {
                renderer.DrawBackground(this, pe);
            }
            else base.OnPaintBackground(pe);
        }
       
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Region = GetRegion();
        }
      
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            OnToolStripAdded(e.Control as ToolStripEx);
        }
     
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            OnToolStripRemoved(e.Control as ToolStripEx);
        }
      
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Region = GetRegion();

            ScrollPositionInternal = GetValidScrollPosition(ScrollPositionInternal);
        }
 
        protected virtual void OnToolStripAdded(ToolStripEx ts)
        {
            if (ts != null)
            {
                ts.TabItem = this.TabItem;

                UpdateToolStrip(ts);

                ts.ItemClicked += new ToolStripItemClickedEventHandler(OnToolStripItemClicked);

                if (ToolStripAdded != null)
                {
                    ToolStripAdded(this, new ToolStripEventArgs(ts));
                }
            }
        }
     
        protected virtual void OnToolStripRemoved(ToolStripEx ts)
        {
            if (ts != null)
            {
                ts.TabItem = null;

                UpdateToolStrip(ts);

                ts.ItemClicked -= new ToolStripItemClickedEventHandler(OnToolStripItemClicked);

                if (ToolStripRemoved != null)
                {
                    ToolStripRemoved(this, new ToolStripEventArgs(ts));
                }
            }
        }
     
        protected override void WndProc(ref Message m)
        {
            switch ((Msg)m.Msg)
            {
                case Msg.WM_NCCALCSIZE:
                    if (OnNcCalcSize(ref m))
                        return;
                    break;

                case Msg.WM_NCPAINT:
                    if (OnNcPaint(ref m))
                        return;
                    break;

                case Msg.WM_NCHITTEST:
                    if (OnNcHitTest(ref m))
                        return;
                    break;
            }

            base.WndProc(ref m);
        }

        /// <summary> Reaction on selection scroll buttons also. </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
            {
                if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    if (new Rectangle(10, this.Bounds.Height - 20, 20, 20).Contains(e.Location))
                    {
                        if (this.m_tabItem.GetCurrentParent().Parent != null)
                        {
                            if ((this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).UseDefaultHighlightColor)
                                office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = ColorTranslator.FromHtml("#cde6f7");
                            else
                                office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = Color.FromArgb(100, (this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MenuColor);
                        }
                        this.Refresh();
                    }
                    else
                    {
                        office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = Color.Transparent;
                        this.Refresh();
                    }
                }
                else
                {
                    if (new Rectangle(this.Bounds.X + this.Bounds.Width - 20, this.Bounds.Height - 20, 20, 20).Contains(e.Location))
                    {
                        if (this.m_tabItem.GetCurrentParent().Parent != null)
                        {
                            if ((this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).UseDefaultHighlightColor)
                                office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = ColorTranslator.FromHtml("#cde6f7");
                            else
                                office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = Color.FromArgb(100, (this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MenuColor);
                        }
                        this.Refresh();
                    }
                    else
                    {
                        office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = Color.Transparent;
                        this.Refresh();
                    }
                }
            }
            bool bRightScrollSelected = false;

            // If mouse over right scroll button than highlight it.
            if (m_bIsRightScroll)
            {
                bRightScrollSelected = RightScrollBounds.Contains(e.Location);

                if (bRightScrollSelected != m_bRightScrollSelected)
                {
                    m_bRightScrollSelected = bRightScrollSelected;
                    RefreshScroll();
                }
            }

            // If mouse over left scroll button than highlight it.
            if (m_bIsLeftScroll && !bRightScrollSelected)
            {
                bool bLeftScrollSelected = LeftScrollBounds.Contains(e.Location);

                if (bLeftScrollSelected != m_bLeftScrollSelected)
                {
                    m_bLeftScrollSelected = bLeftScrollSelected;
                    RefreshScroll();
                }
            }
        }

        /// <summary> Reaction on deselection scroll buttons also. </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.RibbonStyle == Tools.RibbonStyle.Office2013)
            {
                office2013Renderers[this.OfficeColorScheme].UpButtonBackColor = Color.Transparent;
                this.Refresh();
            }
            if (m_bRightScrollSelected)
            {
                m_bRightScrollSelected = false;
                RefreshScroll();
            }
            else if (m_bLeftScrollSelected)
            {
                m_bLeftScrollSelected = false;
                RefreshScroll();
            }
        }
     
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                bool bMouseDown = false;
                if ((this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    if (new Rectangle(5, this.Bounds.Height - 20, 20, 20).Contains(e.Location) && this.RibbonStyle == Tools.RibbonStyle.Office2013)
                    {
                        if ((this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv) != null)
                        {
                            if (!(this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MinimizePanel)
                            {
                                (this.Parent as RibbonControlAdv).MinimizePanel = true;
                            }
                            else
                            {
                                (this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MinimizePanel = false;
                            }
                        }
                    }
                }
                else
                {
                    if (new Rectangle(this.Bounds.Right - 20, this.Bounds.Height - 20, 20, 20).Contains(e.Location) && this.RibbonStyle == Tools.RibbonStyle.Office2013)
                    {
                        if ((this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv) != null)
                        {
                            if (!(this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MinimizePanel)
                            {
                                (this.Parent as RibbonControlAdv).MinimizePanel = true;
                            }
                            else
                            {
                                (this.m_tabItem.GetCurrentParent().Parent as RibbonControlAdv).MinimizePanel = false;
                            }
                        }
                    }
                }
                if (m_bIsRightScroll && RightScrollBounds.Contains(e.Location))
                {
                    bMouseDown = true;
                    this.Capture = true;

                    PushedButton = ScrollButtonsArea.RightScrollButton;
                    ScrollToRight();
                    StartTimer(ScrollButtonsArea.RightScrollButton);
                }
                if (!bMouseDown && (m_bIsLeftScroll && LeftScrollBounds.Contains(e.Location)))
                {
                    this.Capture = true;

                    PushedButton = ScrollButtonsArea.LeftScrollButton;
                    ScrollToLeft();
                    StartTimer(ScrollButtonsArea.LeftScrollButton);
                }
            }
        }

        /// <summary>
        /// Resets timers and releases mouse capture.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.Capture = false;
        }

        /// <summary> Handles release of mouse capture. </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            if (!this.Capture)
            {
                m_timer.Stop();
                this.PushedButton = ScrollButtonsArea.None;
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (!this.Visible)
            {
                CloseOpenedDropDown();
            }
        }

        /// <summary>
        /// Method search for opened DropDown and closes it.
        /// </summary>
        private void CloseOpenedDropDown()
        {
            // If DropDown is opened, we must close it.
            for (int i = 0, count = this.Controls.Count; i < count; i++)
            {
                ToolStripEx ts = this.Controls[i] as ToolStripEx;

                if (ts != null)
                {
                    if (CloseOpenedDropDown(ts.Items))
                    {
                        break;
                    }
                }
            }
        }

        private bool CloseOpenedDropDown(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (CloseOpenedDropDown(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates if item has DropDown and this DropDown is opened.
        /// </summary>
        /// <param name="item"> Item to check. </param>
        /// <returns>Returns true if closed the dropdown</returns>
        private bool CloseOpenedDropDown(ToolStripItem item)
        {
            ToolStripDropDownItem tsDropDownItem = item as ToolStripDropDownItem;
            if (tsDropDownItem != null)
            {
                if (tsDropDownItem.DropDown.Visible)
                {
                    tsDropDownItem.HideDropDown();
                    return true;
                }
            }
            else
            {
                ToolStripPanelItem tsPanelItem = item as ToolStripPanelItem;
                if (tsPanelItem != null)
                {
                    return CloseOpenedDropDown(tsPanelItem.Items);
                }
            }

            return false;
        }
        #endregion

        #region Implementation
 
        private void UpdateFrame()
        {
            if (this != null && this.IsHandleCreated)
            {
                WindowsAPI.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, m_swpFlags);
            }
        }
  
        private bool OnNcCalcSize(ref Message m)
        {
            bool bResult = false;

            RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));

            if (this.RibbonStyle == RibbonStyle.Office2007)
            {
                // Decrease Display rectangle according to border width.
                rc.left += BORDER_WIDTH;
                rc.right -= BORDER_WIDTH;
                rc.top += BORDER_WIDTH;
                rc.bottom -= BORDER_WIDTH;
            }

            int iWidth = rc.Width;
            int iCollapsedWidth = this.GetCollapsedWidth();

            // If needed cut rectangle to locate Scrollbar buttons.
            if (iCollapsedWidth - iWidth > 0)
            {
                if (RightToLeft == RightToLeft.Yes)
                {
                    m_bIsLeftScroll = iCollapsedWidth - m_iScrollPosition > Width - 2 * BORDER_WIDTH - SCROLLBARBUTTON_WIDTH;
                    m_bIsRightScroll = m_iScrollPosition > 0;
                }
                else
                {
                    m_bIsLeftScroll = m_iScrollPosition > 0;
                    m_bIsRightScroll = iCollapsedWidth - m_iScrollPosition > Width - 2 * BORDER_WIDTH - SCROLLBARBUTTON_WIDTH;
                }

                if (m_bIsLeftScroll)
                {
                    rc.left += SCROLLBARBUTTON_WIDTH;
                }

                if (m_bIsRightScroll)
                {
                    rc.right -= SCROLLBARBUTTON_WIDTH;
                }
            }
            else
            {
                m_bIsRightScroll = false;
                m_bIsLeftScroll = false;
            }

            Marshal.StructureToPtr(rc, m.LParam, false);

            return bResult;
        }

        protected virtual bool OnNcPaint(ref Message m)
        {
            bool bResult = false;

            RibbonPanelRenderer renderer = this.Renderer;

            if (renderer != null)
            {
                renderer.DrawFrame(this);
            }

            return bResult;
        }

        protected virtual bool OnNcHitTest(ref Message m)
        {
            bool bResult = false;

            int xPos = WindowsAPI.GET_X_LPARAM((int)m.LParam);
            int yPos = WindowsAPI.GET_Y_LPARAM((int)m.LParam);

            Point p = PointToClient(new Point(xPos, yPos));

            if (RightScrollBounds.Contains(p) || LeftScrollBounds.Contains(p))
            {
                m.Result = new IntPtr((int)HitTest.HTCLIENT);
                bResult = true;
            }

            return bResult;
        }

        /// <summary>
        /// Returns collapsed width of all ToolStrips
        /// </summary>
        /// <returns>Returns collapsed width</returns>
        private int GetCollapsedWidth()
        {
            int result = 0;

            foreach (Control c in this.Controls)
            {
                ToolStripEx ts = c as ToolStripEx;
                if (ts != null)
                {
                    result += ts.CollapsedWidth;
                }
            }

            return result;
        }

        private void UpdateExpandedToolStrips()
        {
            m_toolStripsExpanded.Clear();

            int spaceWidth = this.DisplayRectangle.Width - GetCollapsedWidth();

            for (int i = 0, count = this.Controls.Count; i < count; i++)
            {
                ToolStripEx ts = this.Controls[i] as ToolStripEx;
                if (ts != null && ts.Visible)
                {
                    Size szExpanded = ts.ExpandedSize;

                    int increment = szExpanded.Width - ts.CollapsedWidth;

                    if (increment < spaceWidth)
                    {
                        m_toolStripsExpanded.Add(ts);
                        spaceWidth -= increment;
                    }
                    else break;
                }
            }
        }

        /// <summary>
        /// Manages visibility of panel according to the state of corresponding tab item.
        /// </summary>
        private void UpdateVisibility()
        {
            this.Visible = m_tabItem.Checked && m_tabItem.ShowPanel;
        }
  
        private void UpdateToolStrip(ToolStripEx ts)
        {
            ts.UpdateCaption();
            ts.UpdateRenderer();
        }
    
        private void InitTabItem()
        {
            if (m_tabItem != null)
            {
                m_tabItem.CheckStateChanged += new EventHandler(OnTabCheckStateChanged);

                UpdateVisibility();
            }
        }
  
        private Region GetRegion()
        {
            Region region = null;

            if (this.IsHandleCreated)
            {
                RECT rc = new RECT();

                if (WindowsAPI.GetWindowRect(this.Handle, ref rc))
                {
                    if (this.RibbonStyle == RibbonStyle.Office2007)
                        region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, rc.Size), 2);
                    else
                        region = new Region(new Rectangle(Point.Empty, new Size(rc.Width + 1, rc.Height + 1)));
                }
            }

            return region;
        }

        /// <summary>
        /// Initializes and starts timer.
        /// </summary>
        /// <param name="mousePushedArea">Area where mouse was pushed and caused timer to start.</param>
        private void StartTimer(ScrollButtonsArea mousePushedArea)
        {
            m_timer.Interval = m_timerInt * 4;
            m_timer.Tag = mousePushedArea;
            m_timer.Start();
        }

        internal void RefreshScroll()
        {
            RedrawWindowFlags flags = RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE;
            WindowsAPI.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, flags);
        }

        /// <summary> Move controls to right according to scroll position and their location
        /// and set needed properties for Layout or NCCalcSize methods. </summary>
        private void ScrollToRight()
        {
            int iWidth = Width - 2 * BORDER_WIDTH - SCROLLBARBUTTON_WIDTH;

            // Determine which control must be positioned first and move group to preview needed controls.
            foreach (Control c in this.Controls)
            {
                ToolStripEx ts = c as ToolStripEx;

                if (ts != null && RightToLeft == RightToLeft.Yes ? ts.Left <= iWidth : ts.Right >= iWidth)
                {
                    int iScrollPosition = RightToLeft == RightToLeft.Yes ?
                        ScrollPositionInternal - ts.Left + (m_bIsLeftScroll ? 0 : SCROLLBARBUTTON_WIDTH) :
                        ScrollPositionInternal + ts.Left;

                    ScrollPositionInternal = GetValidScrollPosition(iScrollPosition);

                    break;
                }
            }
        }

        /// <summary> Move controls to left according to scroll position and their location. 
        /// and set needed properties for Layout or NCCalcSize methods </summary>
        private void ScrollToLeft()
        {
            int iWidth = Width - 2 * BORDER_WIDTH - 2 * SCROLLBARBUTTON_WIDTH;

            // Determine which control must be positioned last and move group to preview needed controls.
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                ToolStripEx ts = this.Controls[i] as ToolStripEx;

                if (ts != null)
                {
                    if (RightToLeft == RightToLeft.Yes ? ts.Right >= 0 : ts.Left <= 0)
                    {
                        int iScrollPosition = RightToLeft == RightToLeft.Yes ?
                            ScrollPositionInternal + iWidth - ts.Right + (m_bIsRightScroll ? 0 : SCROLLBARBUTTON_WIDTH) :
                            ScrollPositionInternal - iWidth + ts.Right;

                        ScrollPositionInternal = GetValidScrollPosition(iScrollPosition);

                        break;
                    }
                }
            }
        }

        /// <summary> Method checks position value and leave as it was before. </summary>
        /// <param name="position">Scroll position</param>
        /// <returns>Returns Scroll position</returns>
        private int GetValidScrollPosition(int position)
        {
            int iValue = position;

            if (iValue != 0)
            {
                int iCollapsedWidth = GetCollapsedWidth();
                int iWidth = Width - 2 * BORDER_WIDTH;

                if (iValue < 0 || iCollapsedWidth < iWidth)
                {
                    iValue = 0;
                }
                else if (iCollapsedWidth > iWidth)
                {
                    int iMaxPos = iCollapsedWidth - iWidth + SCROLLBARBUTTON_WIDTH;

                    if (iValue > iMaxPos)
                    {
                        iValue = iMaxPos;
                    }
                }
            }

            return iValue;
        }

        internal ToolStripTabGroup GetTabGroup()
        {
            ToolStripTabGroup tsTabGroup = null;

            ToolStripTabItem tsTabItem = this.TabItem;
            if (tsTabItem != null)
            {
                RibbonControlAdvHeader header = tsTabItem.GetCurrentParent() as RibbonControlAdvHeader;
                if (header != null)
                {
                    if (header.TabGroupsHash.ContainsKey(tsTabItem))
                    {
                        tsTabGroup = header.TabGroupsHash[tsTabItem];
                    }
                }
            }

            return tsTabGroup;
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when new toolstrip is added to the collection.
        /// </summary>
        [Description("Occurs when new toolstrip is added to the collection")]
        public event ToolStripEventHandler ToolStripAdded;

        /// <summary>
        /// Occurs when toolstrip is removed from the collection.
        /// </summary>
        [Description("Occurs when toolstrip is removed from the collection.")]
        public event ToolStripEventHandler ToolStripRemoved;

        /// <summary>
        /// Occurs when toolstrip item is clicked.
        /// </summary>
        [Description("Occurs when toolstrip item is clicked.")]
        public event ToolStripItemClickedEventHandler ToolStripItemClicked;
        #endregion

        #region Event Handlers

        public void OnTabCheckStateChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        public void OnToolStripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (ToolStripItemClicked != null)
            {
                ToolStripItemClicked(sender, e);
            }
        }

        /// <summary>
        /// Handles mouse keeping pushed..
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            ScrollButtonsArea area = (ScrollButtonsArea)((Timer)sender).Tag;

            Point p = PointToClient(Control.MousePosition);

            switch (area)
            {
                case ScrollButtonsArea.RightScrollButton:
                    if (RightScrollBounds.IsEmpty)
                    {
                        this.Capture = false;
                    }
                    else if (RightScrollBounds.Contains(p))
                    {
                        ScrollToRight();
                    }
                    break;

                case ScrollButtonsArea.LeftScrollButton:
                    if (LeftScrollBounds.IsEmpty)
                    {
                        this.Capture = false;
                    }
                    else if (LeftScrollBounds.Contains(p))
                    {
                        ScrollToLeft();
                    }
                    break;
            }

            m_timer.Interval = m_timerInt;
        }
        #endregion

        #region IOffice12Properties Members
        /// <summary>
        /// Gets or sets launcher style.
        /// </summary>
        [Description("Specifies the style of the launcher button.")]
        public LauncherStyle LauncherStyle
        {
            get
            {
                if (m_launcherStyle == LauncherStyle.Default)
                {
                    IOffice12Settings office12props = this.Parent as IOffice12Settings;
                    return office12props != null ? office12props.LauncherStyle : LauncherStyle.Office2007;
                }
                return m_launcherStyle;
            }
            set
            {
                if (m_launcherStyle != value)
                {
                    m_launcherStyle = value;
                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether caption should be shown.
        /// </summary>
        /// <value>true, if Caption should be shown, false otherwise</value>
        [Description("Specifies whether the caption should be shown.")]
        public virtual bool ShowCaption
        {
            get
            {
                if (m_bShowCaption == BoolEx.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.ShowCaption : true;
                }

                return m_bShowCaption == BoolEx.True;
            }
            set
            {
                BoolEx bValue = ToolStripEx.BoolToBoolEx(value);

                if (m_bShowCaption != bValue)
                {
                    m_bShowCaption = bValue;

                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether launcher should be shown.
        /// </summary>
        /// <value>true, if Launcher should be shown, false otherwise</value>
        [Description("Specfies whether the launcher buttton should be shown.")]
        public virtual bool ShowLauncher
        {
            get
            {
                if (m_bShowLauncher == BoolEx.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.ShowLauncher : true;
                }

                return m_bShowLauncher == BoolEx.True;
            }
            set
            {
                BoolEx bValue = ToolStripEx.BoolToBoolEx(value);

                if (m_bShowLauncher != bValue)
                {
                    m_bShowLauncher = bValue;

                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets caption style.
        /// </summary>
        [Description("Specifies the style (Top, Bottom) of caption")]
        public virtual CaptionStyle CaptionStyle
        {
            get
            {
                if (m_captionStyle == CaptionStyle.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.CaptionStyle : this.DefaultCaptionStyle;
                }
                return m_captionStyle;
            }
            set
            {
                if (m_captionStyle != value)
                {
                    m_captionStyle = value;

                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets caption text style.
        /// </summary>
        [Description("Specifies the style of caption text.")]
        public virtual CaptionTextStyle CaptionTextStyle
        {
            get
            {
                if (m_captionTextStyle == CaptionTextStyle.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.CaptionTextStyle : CaptionTextStyle.Plain;
                }
                return m_captionTextStyle;
            }
            set
            {
                if (m_captionTextStyle != value)
                {
                    m_captionTextStyle = value;
                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets caption alignment.
        /// </summary>
        [Description("Specifies the alignment of caption in the control")]
        public virtual CaptionAlignment CaptionAlignment
        {
            get
            {
                if (m_captionAlignment == CaptionAlignment.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.CaptionAlignment : CaptionAlignment.Near;
                }
                return m_captionAlignment;
            }
            set
            {
                if (m_captionAlignment != value)
                {
                    m_captionAlignment = value;
                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets caption font.
        /// </summary>
        [Description("Specifies the caption font.")]
        public virtual Font CaptionFont
        {
            get
            {
                if (m_captionFont == null)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.CaptionFont : this.Font;
                }
                return m_captionFont;
            }
            set
            {
                if (m_captionFont != value)
                {
                    m_captionFont = value;

                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets caption minimal height.
        /// </summary>
        [Description("Specifies the minimum height of caption.")]
        public virtual int CaptionMinHeight
        {
            get
            {
                if (m_nCaptionMinHeight == -1)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.CaptionMinHeight : 0;
                }
                return m_nCaptionMinHeight;
            }
            set
            {
                if (m_nCaptionMinHeight != value)
                {
                    m_nCaptionMinHeight = value;

                    UpdateCaptions();
                }
            }
        }

        /// <summary>
        /// Gets or sets border style.
        /// </summary>
        [Description("Specifies the borderstyle for ToolStripEx.")]
        public virtual ToolStripBorderStyle BorderStyle
        {
            get
            {
                if (m_borderStyle == ToolStripBorderStyle.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.BorderStyle : this.DefaultBorderStyle;
                }
                return m_borderStyle;
            }
            set
            {
                if (m_borderStyle != value)
                {
                    m_borderStyle = value;

                    UpdateCaptions();
                }
            }
        }

        [Browsable(false)]
        public Office2013ColorScheme Office2013ColorScheme
        {
            get
            {
                IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                return (office12settings != null) ? office12settings.Office2013ColorScheme : Tools.Office2013ColorScheme.White;
            }
        }

        [Browsable(false)]
        public RibbonStyle RibbonStyle
        {
            get
            {
                IOffice12Settings office12settings = this.Parent as IOffice12Settings;

                return (office12settings != null) ? office12settings.RibbonStyle : RibbonStyle.Office2007;
            }
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                    OfficeColorScheme = ToolStripEx.ColorScheme.Blue;
                else if (value == "Office2007Silver")
                    OfficeColorScheme = ToolStripEx.ColorScheme.Silver;
                else if (value == "Office2007Black")
                    OfficeColorScheme = ToolStripEx.ColorScheme.Black;
                else if (value == "Office2007Managed")
                    OfficeColorScheme = ToolStripEx.ColorScheme.Managed;
            }
        }
        /// <summary>
        /// Gets or sets color scheme.
        /// </summary>
        [Description("Specifies the color scheme (Silver. Blue).")]
        public virtual ToolStripEx.ColorScheme OfficeColorScheme
        {
            get
            {
                if (m_colorScheme == ToolStripEx.ColorScheme.Default)
                {
                    IOffice12Settings office12settings = this.Parent as IOffice12Settings;
                    return office12settings != null ? office12settings.OfficeColorScheme : this.DefaultOfficeColorScheme;
                }
                return m_colorScheme;
            }
            set
            {
                m_colorScheme = value;
                UpdateRenderers();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets default color scheme
        /// </summary>
        [Description("Specifies the default color scheme (Silver. Blue).")]
        protected virtual ToolStripEx.ColorScheme DefaultOfficeColorScheme
        {
            get { return ToolStripEx.ColorScheme.Managed; }
        }

        /// <summary>
        /// Gets default border style of toolstrips
        /// </summary>
        [Description("Specifies the default border style.")]
        protected virtual ToolStripBorderStyle DefaultBorderStyle
        {
            get { return ToolStripBorderStyle.None; }
        }

        /// <summary>
        /// Gets default caption style.
        /// </summary>
        [Description("Specifies the default style (Top, Bottom) of caption")]
        protected virtual CaptionStyle DefaultCaptionStyle
        {
            get { return CaptionStyle.Top; }
        }

        /// <summary>
        /// Gets default launcher style.
        /// </summary>
        [Description("Specifies the default style of the launcher button.")]
        protected virtual LauncherStyle DefaultLauncherStyle
        {
            get { return LauncherStyle.Office2007; }
        }
        #endregion

        #region ShouldSerialize & Reset Methods

        public bool ShouldSerializeLauncherStyle()
        {
            return m_launcherStyle != LauncherStyle.Default;
        }

        public void ResetLauncherStyle()
        {
            LauncherStyle = LauncherStyle.Default;
        }

        public bool ShouldSerializeShowCaption()
        {
            return m_bShowCaption != BoolEx.Default;
        }

        public void ResetShowCaption()
        {
            m_bShowCaption = BoolEx.Default;
        }

        public bool ShouldSerializeShowLauncher()
        {
            return m_bShowLauncher != BoolEx.Default;
        }

        public void ResetShowLauncher()
        {
            m_bShowLauncher = BoolEx.Default;
            UpdateCaptions();
        }

        public bool ShouldSerializeCaptionStyle()
        {
            return m_captionStyle != CaptionStyle.Default;
        }

        public void ResetCaptionStyle()
        {
            this.CaptionStyle = CaptionStyle.Default;
        }

        public bool ShouldSerializeCaptionTextStyle()
        {
            return m_captionTextStyle != CaptionTextStyle.Default;
        }

        public void ResetCaptionTextStyle()
        {
            this.CaptionTextStyle = CaptionTextStyle.Default;
        }

        public bool ShouldSerializeCaptionAlignment()
        {
            return m_captionAlignment != CaptionAlignment.Default;
        }

        public void ResetCaptionAlignment()
        {
            this.CaptionAlignment = CaptionAlignment.Default;
        }

        public bool ShouldSerializeCaptionFont()
        {
            return m_captionFont != null;
        }

        public void ResetCaptionFont()
        {
            this.CaptionFont = null;
        }

        public bool ShouldSerializeCaptionMinHeight()
        {
            return m_nCaptionMinHeight != -1;
        }

        public void ResetCaptionMinHeight()
        {
            this.CaptionMinHeight = -1;
        }

        public bool ShouldSerializeBorderStyle()
        {
            return m_borderStyle != ToolStripBorderStyle.Default;
        }

        public void ResetBorderStyle()
        {
            this.BorderStyle = ToolStripBorderStyle.Default;
        }

        public bool ShouldSerializeOfficeColorScheme()
        {
            return m_colorScheme != ToolStripEx.ColorScheme.Default;
        }

        public void ResetOfficeColorScheme()
        {
            this.OfficeColorScheme = ToolStripEx.ColorScheme.Default;
        }

        public bool ShouldSerializeMargin()
        {
            return this.Margin != new Padding(4, 0, 4, 0);
        }
    
       public void ResetMargin()
        {
            this.Margin = new Padding(4, 0, 4, 0);
        }
        #endregion

        #region Fields
       private RibbonPanelLayout m_ribbonPanelLayout = null;
        #endregion

        #region Nested classes
       public class RibbonPanelLayout : LayoutEngine
        {
            #region Constructors
       
            public RibbonPanelLayout(LayoutEngine baseLayout)
            {
                m_baseLayout = baseLayout;
            }
            #endregion

            #region Constants
            /// <summary> Default distance between two ToolStrips inside a RibbonPanel. </summary>
            private const int SPACE_BETWEEN_TOOLSTRIP = 2;
            #endregion

            #region Overrides

            public override void InitLayout(object child, BoundsSpecified specified)
            {
                if (m_baseLayout != null)
                {
                    m_baseLayout.InitLayout(child, specified);
                }
            }
            
            public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
            {
                RibbonPanel panel = container as RibbonPanel;

                if (panel != null)
                {
                    Rectangle rc = panel.DisplayRectangle;

                    int height = rc.Height;
                    int iPosition = -panel.ScrollPositionInternal;

                    RightToLeft rtl = panel.RightToLeft;

                    panel.UpdateExpandedToolStrips();
                    if (panel.Controls.Count > 0)
                    {
                        for (int i = 0, count = panel.Controls.Count; i < count; i++)
                        {
                            ToolStripEx ts = panel.Controls[i] as ToolStripEx;

                            if (ts != null && ts.Visible)
                            {
                                if (panel.m_toolStripsExpanded.Contains(ts))
                                {
                                    ts.State = ToolStripEx.ToolStripExState.Expanded;

                                    if (ts.AutoSize)
                                    {
                                        ts.Width = ts.GetPreferredSize(Size.Empty).Width;
                                    }
                                }
                                else
                                {
                                    if (ts.State != ToolStripEx.ToolStripExState.Collapsed)
                                    {
                                        ts.State = ToolStripEx.ToolStripExState.Collapsed;
                                    }
                                    else ts.Width = ts.CollapsedWidth;
                                }

                                ts.Top = rc.Top;
                                ts.Height = height;

                                panel.SetToolStripLocationRTL(ts, rtl, iPosition);

                                iPosition += ts.Width + SPACE_BETWEEN_TOOLSTRIP;
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            #endregion

            #region Fields
            private LayoutEngine m_baseLayout;
            #endregion
        }
        #endregion
    }
}
#endif