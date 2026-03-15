#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Collections;


namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// VS2008-like tab splitter UI.
    /// </summary>
    /// 
    [ToolboxBitmap(typeof(TabSplitterContainer), "ToolboxIcons.TabSplitterContainer.bmp")]
    [Designer(typeof(Design.TabSplitterContainerDesigner))]
    public class TabSplitterContainer : Control
    {
        #region Data

        /// <summary>
        /// Indicates, whether panels are in collapsed state.
        /// </summary>
        private bool m_bCollapsed = false;

        /// <summary>
        /// Indicates, whether control's panels are swapped.
        /// </summary>
        private bool m_bSwapped = false;

        /// <summary>
        /// Splitter control
        /// </summary>
        private TabSplitter m_splitter;

        /// <summary>
        /// Collection of primary pages.
        /// </summary>
        private TabSplitterPagesCollection m_primaryPages;

        /// <summary>
        /// Collection of secondary pages.
        /// </summary>
        private TabSplitterPagesCollection m_secondaryPages;

        /// <summary>
        /// Collection with selected page in Collapsed mode
        /// </summary>
        private TabSplitterPagesCollection m_selectedPages = null;
        #endregion Data

        #region Construction/finalization

        public TabSplitterContainer()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TabSplitterContainer));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
            Initialize();
        }

        ~TabSplitterContainer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Additional initialization of the control.
        /// </summary>
        private void Initialize()
        {
            m_primaryPages = new TabSplitterPagesCollection(this);
            m_secondaryPages = new TabSplitterPagesCollection(this);

            m_splitter = new TabSplitter(this);
            Controls.Add(m_splitter);

            AdwiseEvents();
        }

        private void AdwiseEvents()
        {
            m_splitter.LocationChanged += new EventHandler(OnSplitterLocationChanged);
        }

        private void UnadwiseEvents()
        {
            m_splitter.LocationChanged -= new EventHandler(OnSplitterLocationChanged);
        }

        #endregion

        #region Properties

        #region Overrides

        /// <summary>
        /// Gets the collection of controls contained within the control.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ControlCollection Controls
        {
            get { return base.Controls; }
        }

        #endregion Overrides

        #region Appearance

        /// <summary>
        /// Indicates, whether panels are in collapsed state.
        /// </summary>
        /// <remarks>Value is false, if control is in expanded state.</remarks>
        [DefaultValue(false)]
        [Category("Appearance"), Description("Indicates whether the secondary pane is collapsed")]
        public bool Collapsed
        {
            get
            {
                return m_bCollapsed;
            }
            set
            {
                if (m_bCollapsed != value)
                {
                    m_bCollapsed = value;
                    OnCollapsedChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the splitter orientation.
        /// </summary>
        [DefaultValue(Orientation.Horizontal)]
        [Category("Appearance"), Description("Defines the horizontal or vertical orientation of the splitter.")]
        public Orientation Orientation
        {
            get
            {
                return m_splitter.Orientation;
            }
            set
            {
                m_splitter.Orientation = value;
            }
        }

        /// <summary>
        /// Indicates, whether control's panels are swapped.
        /// </summary>
        /// <remarks>
        /// If value is true, primary items are layouted as secondary and vice versa.
        /// Layout of tabs in splitter is also swapped.
        /// </remarks>
        [DefaultValue(false)]
        [Category("Appearance"), Description("Indicates whether the primary and secondary panes are swapped")]
        public bool Swapped
        {
            get
            {
                return m_bSwapped;
            }
            set
            {
                if (m_bSwapped != value)
                {
                    m_bSwapped = value;

                    OnSwappedChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets the location of the splitter, in pixels, from the left or top edge of the TabSplitContainer.
        /// </summary>
        [Category("Appearance"), Description("Gets or sets the location of the splitter, in pixels, from the left or top edge of the TabSplitContainer.")]
        public int SplitterPosition
        {
            get { return m_splitter.Position; }
            set { m_splitter.Position = value; }
        }

        /// <summary>
        /// Gets or Sets the backcolor for TabSplitter.
        /// </summary>
        [
        Category("Appearance"),
        Description("Gets or Sets the backcolor for TabSplitter."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        DefaultValue(typeof(SystemColors), "ControlDark")
        ]
        public Color SplitterBackColor
        {
            get
            {
                return this.m_splitter.BackColor;
            }
            set
            {
                if (this.m_splitter.BackColor != value)
                {
                    this.m_splitter.BackColor = value;
                }
            }
        }

        #endregion Appearance

        #region Items

        /// <summary>
        /// Returns collection of primary nested items.
        /// </summary>
        [Category("Data"), Description("Gets an object representing the collection of the pages contained in the primary pane.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]

        public TabSplitterPagesCollection PrimaryPages
        {
            get
            {
                return m_primaryPages;
            }
        }

        /// <summary>
        /// Returns collection of secondary items.
        /// </summary>
        [Category("Data"), Description("Gets an object representing the collection of the pages contained in the secondary pane.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabSplitterPagesCollection SecondaryPages
        {
            get
            {
                return m_secondaryPages;
            }
        }

        #endregion Items

        #region Internal
        /// <summary>
        /// 
        /// </summary>
        internal Control Splitter
        {
            get { return m_splitter; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal TabSplitterPagesCollection PrimaryPagesInternal
        {
            get { return this.Swapped ? this.SecondaryPages : this.PrimaryPages; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal TabSplitterPagesCollection SecondaryPagesInternal
        {
            get { return !this.Swapped ? this.SecondaryPages : this.PrimaryPages; }
        }

        #endregion

        #endregion Properties

        #region Events

        /// <summary>
        /// Occurs when <see cref="TabSplitterContainer.Orientation"/> property is changed.
        /// </summary>
        [Description("Occurs when the Orientation property value has been changed")]
        public event EventHandler OrientationChanged
        {
            add { m_splitter.OrientationChanged += value; }
            remove { m_splitter.OrientationChanged -= value; }
        }

        /// <summary>
        /// Occurs when <see cref="TabSplitterContainer.Collapsed"/> property is changed.
        /// </summary>
        [Description("Occurs when the Collapsed property value has been changed")]
        public event EventHandler CollapsedChanged;

        /// <summary>
        /// Occurs when <see cref="TabSplitterContainer.Swapped"/> property is changed.
        /// </summary>
        [Description("Occurs when the Swapped property value has been changed")]
        public event EventHandler SwappedChanged;

        /// <summary>
        /// Occurs when the Splitter's position has been changed.
        /// </summary>
        [Description("Occurs when the Splitter's position has been changed.")]
        public event EventHandler SplitterPositionChanged;


        #region Internal events
        /// <summary>
        /// 
        /// </summary>
        internal event CancelEventHandler SplitterPositionChanging
        {
            add { m_splitter.PositionChanging += value; }
            remove { m_splitter.PositionChanging -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal event CancelEventHandler OrientationChanging
        {
            add { m_splitter.OrientationChanging += value; }
            remove { m_splitter.OrientationChanging -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal event CancelEventHandler CollapsedChanging
        {
            add { m_splitter.CollapsedChanging += value; }
            remove { m_splitter.CollapsedChanging -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal event CancelEventHandler SwappedChanging
        {
            add { m_splitter.SwappedChanging += value; }
            remove { m_splitter.SwappedChanging -= value; }
        }
        #endregion

        #endregion Events

        #region Overrides
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnadwiseEvents();

                // Order of clean-up does matter.
                m_splitter = null;

                m_primaryPages = null;
                m_secondaryPages = null;
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            Rectangle rcPrimary = this.ClientRectangle;
            Rectangle rcSecondary = this.ClientRectangle;

            if (this.Orientation == Orientation.Horizontal)
            {
                rcPrimary.Height = m_splitter.Top;

                if (!this.Collapsed)
                {
                    rcSecondary.Y = m_splitter.Bottom;
                    rcSecondary.Height -= m_splitter.Bottom;
                }
                else rcSecondary.Height = m_splitter.Top;
            }
            else
            {
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    rcPrimary.X = m_splitter.Right;
                    rcPrimary.Width -= m_splitter.Right;

                    if (this.Collapsed)
                    {
                        rcSecondary.X = rcPrimary.X;
                        rcSecondary.Width = rcPrimary.Width;
                    }
                    else rcSecondary.Width = m_splitter.Left;
                }
                else
                {
                    rcPrimary.Width = m_splitter.Left;

                    if (!this.Collapsed)
                    {
                        rcSecondary.X = m_splitter.Right;
                        rcSecondary.Width -= m_splitter.Right;
                    }
                    else rcSecondary.Width = m_splitter.Left;
                }
            }

            foreach (Control c in this.PrimaryPagesInternal)
            {
                c.Bounds = rcPrimary;
            }

            foreach (Control c in this.SecondaryPagesInternal)
            {
                c.Bounds = rcSecondary;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            SuspendLayout();

            m_splitter.AdjustBounds();

            base.OnResize(e);

            ResumeLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            TabSplitterPage page = e.Control as TabSplitterPage;
            if (page != null)
            {
                page.TextChanged += new EventHandler(OnPageTextChanged);
                page.ImageChanged += new EventHandler(OnPageImageChanged);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            TabSplitterPage page = e.Control as TabSplitterPage;
            if (page != null)
            {
                page.TextChanged -= new EventHandler(OnPageTextChanged);
                page.ImageChanged -= new EventHandler(OnPageImageChanged);

                page.Owner = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (this.Orientation == Orientation.Vertical)
            {
                SuspendLayout();

                m_splitter.AdjustBounds();

                ResumeLayout();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSplitterPositionChanged(EventArgs e)
        {
            if (SplitterPositionChanged != null)
            {
                SplitterPositionChanged(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnSwappedChanged()
        {
            PerformLayout();

            m_splitter.PerformLayout();
            m_splitter.Invalidate();

            RaiseSimpleEvent(this.SwappedChanged);
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        protected virtual void OnCollapsedChanged()
        {
            this.SuspendLayout();

            if (this.Collapsed)
            {
                if (this.PrimaryPagesInternal.SelectedIndex >= 0 || this.SecondaryPagesInternal.SelectedIndex < 0)
                {
                    m_selectedPages = this.PrimaryPagesInternal;
                    this.SecondaryPagesInternal.SelectedIndex = -1;
                }
                else m_selectedPages = this.SecondaryPagesInternal;
            }
            else
            {
                this.PrimaryPagesInternal.SelectActiveItem();
                this.SecondaryPagesInternal.SelectActiveItem();
            }

            m_splitter.AdjustBounds();

            m_splitter.PerformLayout();
            m_splitter.Invalidate();

            this.ResumeLayout();

            RaiseSimpleEvent(this.CollapsedChanged);
        }

        #endregion

        #region Event handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSplitterLocationChanged(object sender, EventArgs e)
        {
            OnSplitterPositionChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageTextChanged(object sender, EventArgs e)
        {
            m_splitter.PerformLayout(); ;
            m_splitter.Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageImageChanged(object sender, EventArgs e)
        {
            m_splitter.Invalidate();
        }

        #endregion Event handlers

        #region Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pages"></param>
        internal void OnPagesChanged()
        {
            m_splitter.PerformLayout();
            m_splitter.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabSplitterPanelsCollection"></param>
        /// <param name="oldSelection"></param>
        /// <param name="newSelection"></param>
        internal void OnSelectedIndexChanged(TabSplitterPagesCollection pages)
        {
            if (this.Collapsed)
            {
                if (m_selectedPages != pages && pages.SelectedIndex >= 0)
                {
                    if (m_selectedPages != null)
                    {
                        m_selectedPages.SelectedIndex = -1;
                    }
                    m_selectedPages = pages;
                }
            }
            m_splitter.Invalidate();
        }

        /// <summary>
        /// Raises simple events (<see cref="EventHandler"/>).
        /// </summary>
        private void RaiseSimpleEvent(EventHandler handler)
        {
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Utility methods

        #region *** TabSplitter
        class TabSplitter : Control
        {
            #region Constants
            const int MIN_HEIGHT = 20;
            const int BAR_THICKNESS = 3;
            const int GRIP_WIDTH = 50;
            const int GRIP_HEIGHT = 5;

            const int LEFT_PADDING = 8;

            const int IMAGE_PADDING = 2;
            const int TAB_IMAGE_SIZE = 16;//12;
            const int TAB_BEVEL_SIZE = 18;
            const int TAB_OVERLAP_SIZE = 10;
            const int TAB_MARGIN = 4;
            const int TAB_PADDING = 2;

            /// <summary>
            /// Splitter buttons IDs
            /// </summary>
            const int BT_VERTICAL = 0;
            const int BT_HORIZONTAL = 1;
            const int BT_COLLAPSED = 2;
            const int BT_SWAP = 3;
            const int BT_NONE = 4;

            const int BS_PATTERN = 3;
            const int PATINVERT = 0x005A0049;
            static readonly IntPtr WVR_REDRAW = new IntPtr(0x0300);
            #endregion

            #region Constructors/Destructor
            /// <summary>
            /// 
            /// </summary>
            static TabSplitter()
            {
                Bitmap buttons = new Bitmap(typeof(TabSplitterContainer).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.TabSplitterContainer.TabSplitterButtons.bmp"));

                m_buttons = new ImageList();
                m_buttons.ImageSize = new Size(11, 11);
                m_buttons.Images.AddStrip(buttons);
                m_buttons.TransparentColor = Color.Magenta;

                m_blend = new Blend();
                m_blend.Positions = new float[] { 0.0f, 0.2f, 1.0f };
                m_blend.Factors = new float[] { 0.0f, 1.0f, 1.0f };
            }
            /// <summary>
            /// 
            /// </summary>
            public TabSplitter(TabSplitterContainer owner)
            {
                m_owner = owner;

                m_tooltip = new ToolTip();
                m_tooltip.Active = false;

                this.Dock = DockStyle.None;

                this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            }
            #endregion

            #region Properties
            /// <summary>
            /// 
            /// </summary>
            public int Position
            {
                get
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        return this.Top;
                    }
                    return this.Left;
                }
                set
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        SetBounds(0, value, 0, 0, BoundsSpecified.Y);
                    }
                    else
                    {
                        SetBounds(value, 0, 0, 0, BoundsSpecified.X);
                    }

                    AdjustPosition();
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public Orientation Orientation
            {
                get
                {
                    return m_orientation;
                }
                set
                {
                    if (m_orientation != value)
                    {
                        m_orientation = value;

                        double pos = m_relativePos;
                        m_relativePos = m_prevRelativePos;
                        m_prevRelativePos = pos;

                        if (this.Collapsed)
                        {
                            this.Collapsed = false;
                        }
                        else
                        {
                            AdjustBounds();
                        }

                        OnOrientationChanged();
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private int SplitterHeight
            {
                get
                {
                    if (m_splitterHeight < 0)
                    {
                        using (Bitmap bmp = new Bitmap(1, 1))
                        {
                            using (Graphics g = Graphics.FromImage(bmp))
                            {
                                int height = (int)g.MeasureString("X", this.Font).Height + 1 + TAB_MARGIN;

                                m_splitterHeight = height < MIN_HEIGHT ? MIN_HEIGHT : height;
                            }
                        }
                    }
                    return m_splitterHeight;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private TabSplitterPagesCollection PrimaryItemsInternal
            {
                get { return m_owner.PrimaryPagesInternal; }
            }
            /// <summary>
            /// 
            /// </summary>
            private TabSplitterPagesCollection SecondaryItemsInternal
            {
                get { return m_owner.SecondaryPagesInternal; }
            }
            /// <summary>
            /// 
            /// </summary>
            private bool Collapsed
            {
                get { return m_owner.Collapsed; }
                set { m_owner.Collapsed = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            private bool Swapped
            {
                get { return m_owner.Swapped; }
                set { m_owner.Swapped = value; }
            }
            /// <summary>
            /// 
            /// </summary>
            private Size ButtonSize
            {
                get
                {
                    Size size = m_buttons.ImageSize;

                    size.Width += 2 * IMAGE_PADDING;
                    size.Height += 2 * IMAGE_PADDING;

                    return size;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private LayoutInfo LayoutData
            {
                get
                {
                    if (m_layoutData == null)
                    {
                        m_layoutData = new LayoutInfo(this);
                        m_layoutData.PerformLayout();
                    }
                    return m_layoutData;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private int SelectedButton
            {
                get
                {
                    return m_selectedButton;
                }
                set
                {
                    if (m_selectedButton != value)
                    {
                        InvalidateButton(m_selectedButton);

                        m_selectedButton = value;

                        InvalidateButton(m_selectedButton);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private int PressedButton
            {
                get
                {
                    return m_pressedButton;
                }
                set
                {
                    if (m_pressedButton != value)
                    {
                        InvalidateButton(m_pressedButton);

                        m_pressedButton = value;

                        InvalidateButton(m_pressedButton);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private int CheckedButton
            {
                get
                {
                    if (!this.Collapsed)
                    {
                        return this.Orientation == Orientation.Horizontal ? BT_HORIZONTAL : BT_VERTICAL;
                    }
                    return BT_NONE;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            private int SplitBarPosition
            {
                get
                {
                    if (this.Orientation == Orientation.Vertical)
                    {
                        return this.RightToLeft == RightToLeft.Yes ? m_rcSplitBar.Right - this.Width : m_rcSplitBar.X;
                    }
                    return m_rcSplitBar.Y;
                }
            }
            #endregion

            #region Overrides
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="specified"></param>
            protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
            {
                Size size = m_owner.ClientSize;

                if (m_orientation == Orientation.Horizontal)
                {
                    x = 0;

                    if (y > size.Height - SplitterHeight)
                    {
                        y = size.Height - SplitterHeight;
                    }
                    if (y < 0)
                    {
                        y = 0;
                    }
                    width = size.Width;
                    height = SplitterHeight;
                }
                else
                {
                    y = 0;

                    if (x > size.Width - SplitterHeight)
                    {
                        x = size.Width - SplitterHeight;
                    }
                    if (x < 0)
                    {
                        x = 0;
                    }
                    width = SplitterHeight;
                    height = size.Height;
                }
                base.SetBoundsCore(x, y, width, height, specified);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="levent"></param>
            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);

                m_layoutData = null;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnRightToLeftChanged(EventArgs e)
            {
                base.OnRightToLeftChanged(e);

                PerformLayout();
            }

            protected override void OnFontChanged(EventArgs e)
            {
                base.OnFontChanged(e);

                m_splitterHeight = -1;

                SuspendLayout();
                AdjustBounds();
                ResumeLayout();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle rcClip = e.ClipRectangle;

                bool bCollapsed = this.Collapsed;

                TabSplitterPagesCollection primaryPanels = this.PrimaryItemsInternal;
                int primarySelected = primaryPanels.SelectedIndex;

                TabSplitterPagesCollection secondaryPanels = this.SecondaryItemsInternal;
                int secondarySelected = secondaryPanels.SelectedIndex;

                Region saveRegion = g.Clip;
                g.SetClip(Rectangle.Inflate(this.ClientRectangle, -1, -1), CombineMode.Intersect);

                for (int i = 0; i < primaryPanels.Count; i++)
                {
                    if (i != primarySelected)
                    {
                        Rectangle rc = this.LayoutData.GetPrimaryTabRect(i);
                        if (!rc.IsEmpty && rc.IntersectsWith(rcClip))
                        {
                            DrawTabBackground(g, rc, true, false);
                            DrawTab(g, rc, true, primaryPanels[i]);
                        }
                    }
                }

                for (int i = 0; i < secondaryPanels.Count; i++)
                {
                    int j = bCollapsed ? i : secondaryPanels.Count - i - 1;

                    if (j != secondarySelected)
                    {
                        Rectangle rc = this.LayoutData.GetSecondaryTabRect(j);
                        if (!rc.IsEmpty && rc.IntersectsWith(rcClip))
                        {
                            DrawTabBackground(g, rc, bCollapsed, false);
                            DrawTab(g, rc, bCollapsed, secondaryPanels[j]);
                        }
                    }
                }

                g.SetClip(saveRegion, CombineMode.Replace);

                Rectangle rcPrimarySelected = this.LayoutData.GetPrimaryTabRect(primarySelected);
                if (!rcPrimarySelected.IsEmpty && rcPrimarySelected.IntersectsWith(rcClip))
                {
                    DrawTabBackground(g, rcPrimarySelected, true, true);
                    DrawTab(g, rcPrimarySelected, true, primaryPanels[primarySelected]);
                }

                Rectangle rcSecondarySelected = this.LayoutData.GetSecondaryTabRect(secondarySelected);
                if (!rcSecondarySelected.IsEmpty && rcSecondarySelected.IntersectsWith(rcClip))
                {
                    DrawTabBackground(g, rcSecondarySelected, bCollapsed, true);
                    DrawTab(g, rcSecondarySelected, bCollapsed, secondaryPanels[secondarySelected]);
                }

                DrawGrip(g, ref rcClip);

                for (int button = BT_VERTICAL; button < BT_NONE; button++)
                {
                    DrawButton(g, ref rcClip, button);
                }

                base.OnPaint(e);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                Graphics g = e.Graphics;

                Rectangle rc = this.ClientRectangle;

                if (this.Orientation == Orientation.Horizontal)
                {
                    Rectangle rcBrush = new Rectangle(0, 0, 1, rc.Height);

                    using (LinearGradientBrush brush = new LinearGradientBrush(rcBrush, this.BackColor, SystemColors.Control, LinearGradientMode.Horizontal))
                    {
                        brush.Blend = m_blend;
                        g.FillRectangle(brush, rc.X + 1, rc.Y + 1, rc.Width - 2, rc.Height - 2);
                    }
                }
                else
                {
                    Rectangle vertBrush = new Rectangle(0, 0, 1, rc.Height);
                    using (LinearGradientBrush brush = new LinearGradientBrush(vertBrush, this.BackColor, SystemColors.Control, LinearGradientMode.Horizontal))
                    {
                        g.FillRectangle(brush, rc.X + 1, rc.Y + 1, rc.Width - 2, rc.Height - 2);
                    }
                }

                using (Pen pen = new Pen(SystemColors.ControlDark))
                {
                    g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
                {
                    m_pLastMouseDown = new Point(e.X, e.Y);

                    int button = this.LayoutData.GetButtonID(e.X, e.Y);
                    if (button != BT_NONE)
                    {
                        this.PressedButton = button;
                    }
                    else
                    {
                        m_pressedPanel = this.LayoutData.GetPage(e.X, e.Y);

                        if (m_pressedPanel == null)
                        {
                            m_bMoving = true;
                            m_rcSplitBar = Rectangle.Empty;
                        }
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseMove(MouseEventArgs e)
            {
                if (m_bMoving)
                {
                    if (!m_rcSplitBar.IsEmpty || GetDistance(e.X, m_pLastMouseDown.X) > BAR_THICKNESS || GetDistance(e.Y, m_pLastMouseDown.Y) > BAR_THICKNESS)
                    {
                        DrawSplitBar(GetSplitBarBounds(e.X, e.Y));
                    }
                }
                else
                {
                    if (m_pressedPanel == null)
                    {
                        this.SelectedButton = this.LayoutData.GetButtonID(e.X, e.Y);
                    }

                    if (e.Button == MouseButtons.None)
                    {
                        UpdateToolTip(e.X, e.Y);
                    }
                }

                base.OnMouseMove(e);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseLeave(EventArgs e)
            {
                this.SelectedButton = BT_NONE;

                base.OnMouseLeave(e);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="e"></param>
            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (m_bMoving)
                {
                    m_bMoving = false;

                    if (!m_rcSplitBar.IsEmpty)
                    {
                        int position = this.SplitBarPosition;

                        DrawSplitBar(Rectangle.Empty);

                        SetPositionInternal(position);
                    }
                }
                else
                {
                    if (m_pressedPanel != null)
                    {
                        if (m_pressedPanel == this.LayoutData.GetPage(e.X, e.Y))
                        {
                            OnClickPage(m_pressedPanel);
                        }
                        m_pressedPanel = null;
                    }
                    else
                    {
                        int pressedButton = this.PressedButton;
                        if (pressedButton != BT_NONE)
                        {
                            int selectedButton = this.SelectedButton;

                            this.PressedButton = BT_NONE;

                            if (pressedButton == selectedButton)
                            {
                                OnClickButton(pressedButton);
                            }
                            else
                            {
                                InvalidateButton(selectedButton);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case NativeMethods.WM_SETCURSOR:
                        if (OnWmSetCursor(ref m)) return;
                        break;
                    case NativeMethods.WM_CAPTURECHANGED:
                        OnWmCaptureChanged(ref m);
                        break;
                    case NativeMethods.WM_NCCALCSIZE:
                        OnWmNcCalcSize(ref m);
                        return;
                    case NativeMethods.WM_LBUTTONDBLCLK:
                        OnWmLButtondDlclk(ref m);
                        break;
                }
                base.WndProc(ref m);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    m_owner = null;
                }

                base.Dispose(disposing);
            }
            #endregion

            #region Message handlers
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            private bool OnWmSetCursor(ref Message m)
            {
                bool bResult = false;

                Point p = PointToClient(Cursor.Position);
                if (this.LayoutData.GetButtonID(p.X, p.Y) == BT_NONE && this.LayoutData.GetPage(p.X, p.Y) == null)
                {
                    Cursor c = (m_orientation == Orientation.Horizontal) ? Cursors.HSplit : Cursors.VSplit;

                    SetCursor(c.Handle);

                    m.Result = (IntPtr)1;
                    bResult = true;
                }
                return bResult;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            private void OnWmCaptureChanged(ref Message m)
            {
                m_bMoving = false;

                if (!m_rcSplitBar.IsEmpty)
                {
                    DrawSplitBar(Rectangle.Empty);
                }

                this.PressedButton = BT_NONE;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            private void OnWmNcCalcSize(ref Message m)
            {
                base.WndProc(ref m);
                m.Result = WVR_REDRAW;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="m"></param>
            private void OnWmLButtondDlclk(ref Message m)
            {
                int x = NativeMethods.LOWORD((int)m.LParam);
                int y = NativeMethods.HIWORD((int)m.LParam);

                TabSplitterPage page = this.LayoutData.GetPage(x, y);
                if (page != null)
                {
                    m_owner.Collapsed = true;

                    TabSplitterPagesCollection pages = page.Owner;
                    if (pages != null)
                    {
                        pages.Select(page);
                    }
                }
                else
                {
                    if (this.LayoutData.GetButtonID(x, y) == BT_NONE)
                    {
                        m_owner.Collapsed = !m_owner.Collapsed;
                    }
                }
            }

            #endregion

            #region Implementation

            /// <summary>
            /// 
            /// </summary>
            internal void AdjustBounds()
            {
                Size size = m_owner.ClientSize;

                if (m_orientation == Orientation.Horizontal)
                {
                    int top = this.Collapsed ? size.Height : (int)(m_relativePos * size.Height + 0.5);

                    SetBounds(0, top, size.Width, SplitterHeight, BoundsSpecified.All);
                }
                else
                {
                    int left = this.Collapsed ? size.Width : (int)(m_relativePos * size.Width + 0.5);

                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        left = size.Width - (left + SplitterHeight);
                    }

                    SetBounds(left, 0, SplitterHeight, size.Height, BoundsSpecified.All);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            internal void AdjustPosition()
            {
                Size size = m_owner.ClientSize;

                if (m_orientation == Orientation.Horizontal)
                {
                    if (size.Height > 0)
                    {
                        if (this.Bottom < size.Height)
                        {
                            m_relativePos = (double)this.Top / size.Height;
                            this.Collapsed = false;
                        }
                        else this.Collapsed = true;
                    }
                }
                else
                {
                    if (size.Width > 0)
                    {
                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            if (this.Left > 0)
                            {
                                m_relativePos = (double)(size.Width - this.Right) / size.Width;
                                this.Collapsed = false;
                            }
                            else this.Collapsed = true;
                        }
                        else
                        {
                            if (this.Right < size.Width)
                            {
                                m_relativePos = (double)this.Left / size.Width;
                                this.Collapsed = false;
                            }
                            else this.Collapsed = true;
                        }
                    }
                }
            }


            private void SetPositionInternal(int position)
            {
                if (OnPositionChanging(position))
                {
                    this.Position = position;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="orientation"></param>
            private void SetOrientationInternal(Orientation orientation)
            {
                if (OnOrientationChanging(orientation))
                {
                    this.Orientation = orientation;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bCollapsed"></param>
            private void SetCollapsedInternal(bool bCollapsed)
            {
                if (OnCollapsedChanging(bCollapsed))
                {
                    this.Collapsed = bCollapsed;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bSwapped"></param>
            private void SetSwappedInternal(bool bSwapped)
            {
                if (OnSwappedChanging(bSwapped))
                {
                    this.Swapped = bSwapped;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            private void OnOrientationChanged()
            {
                if (OrientationChanged != null)
                {
                    OrientationChanged(m_owner, EventArgs.Empty);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="button"></param>
            private void OnClickButton(int button)
            {
                switch (button)
                {
                    case BT_VERTICAL:
                        SetOrientationInternal(Orientation.Vertical);
                        break;
                    case BT_HORIZONTAL:
                        SetOrientationInternal(Orientation.Horizontal);
                        break;
                    case BT_COLLAPSED:
                        SetCollapsedInternal(!this.Collapsed);
                        break;
                    case BT_SWAP:
                        SetSwappedInternal(!this.Swapped);
                        break;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="page"></param>
            private void OnClickPage(TabSplitterPage page)
            {
                TabSplitterPagesCollection pages = page.Owner;
                if (pages != null)
                {
                    pages.Select(page);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            private bool OnPositionChanging(int position)
            {
                if (this.PositionChanging != null)
                {
                    PositionEventArgs ea = new PositionEventArgs(position);

                    this.PositionChanging(m_owner, ea);

                    return !ea.Cancel;
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="orientation"></param>
            /// <returns></returns>
            private bool OnOrientationChanging(Orientation orientation)
            {
                if (this.OrientationChanging != null)
                {
                    OrientationEventArgs ea = new OrientationEventArgs(orientation);

                    this.OrientationChanging(m_owner, ea);

                    return !ea.Cancel;
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bCollapsed"></param>
            /// <returns></returns>
            private bool OnCollapsedChanging(bool bCollapsed)
            {
                if (this.CollapsedChanging != null)
                {
                    CollapsedEventArgs ea = new CollapsedEventArgs(bCollapsed);

                    this.CollapsedChanging(m_owner, ea);

                    return !ea.Cancel;
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="bSwapped"></param>
            /// <returns></returns>
            private bool OnSwappedChanging(bool bSwapped)
            {
                if (this.SwappedChanging != null)
                {
                    SwappedEventArgs ea = new SwappedEventArgs(bSwapped);

                    this.SwappedChanging(m_owner, ea);

                    return !ea.Cancel;
                }
                return true;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rc"></param>
            /// <param name="bPrimary"></param>
            /// <param name="control"></param>
            private void DrawTab(Graphics g, Rectangle rc, bool bPrimary, Control control)
            {
                TabSplitterPage page = control as TabSplitterPage;
                if (page != null)
                {
                    Rectangle rcImage = new Rectangle(0, 0, TAB_IMAGE_SIZE, TAB_IMAGE_SIZE);

                    if (this.Orientation == Orientation.Horizontal)
                    {
                        int x = rc.X;
                        int width = rc.Width - TAB_BEVEL_SIZE;

                        if (!bPrimary)
                        {
                            x += TAB_BEVEL_SIZE;

                            rc.Y += 1;
                            rc.Height -= 1;
                        }

                        rcImage.X = x + TAB_PADDING;
                        rcImage.Y = rc.Y + (rc.Height - TAB_IMAGE_SIZE) / 2;

                        RectangleF rcText = new RectangleF(rcImage.Right, rc.Y, width - rcImage.Width - 2 * TAB_PADDING, rc.Height);

                        if (this.RightToLeft == RightToLeft.Yes)
                        {
                            rcText.X = rc.Left + rc.Right - rcText.Right;
                            rcImage.X = rc.Left + rc.Right - rcImage.Right;
                        }

                        using (StringFormat sf = new StringFormat(StringFormatFlags.NoWrap))
                        {
                            sf.LineAlignment = StringAlignment.Center;
                            g.DrawString(page.Text, this.Font, SystemBrushes.ControlText, rcText, sf);
                        }
                    }
                    else
                    {
                        if (!bPrimary)
                        {
                            rc.X += 1;
                            rc.Width -= 1;

                            rc.Y += TAB_BEVEL_SIZE;
                            rc.Height -= TAB_BEVEL_SIZE;
                        }

                        rcImage.X = rc.X + (rc.Width - TAB_IMAGE_SIZE) / 2;
                        rcImage.Y = rc.Y + TAB_PADDING;
                    }

                    Image image = page.Image;
                    Size szImage = image.Size;

                    if (szImage.Width <= TAB_IMAGE_SIZE && szImage.Height <= TAB_IMAGE_SIZE)
                    {
                        g.DrawImage(image, new Point(rcImage.X + (TAB_IMAGE_SIZE - szImage.Width) / 2, rcImage.Y + (TAB_IMAGE_SIZE - szImage.Height) / 2));
                    }
                    else
                    {
                        g.DrawImage(image, rcImage);
                    }
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rc"></param>
            /// <param name="bPrimary"></param>
            /// <param name="bSelected"></param>
            private void DrawTabBackground(Graphics g, Rectangle rc, bool bPrimary, bool bSelected)
            {
                Point[] points = GetTabPolygon(ref rc, bPrimary, this.RightToLeft == RightToLeft.Yes);

                using (Brush brush = GetTabBrush(ref rc, bPrimary, bSelected))
                {
                    g.FillPolygon(brush, points);
                }

                SmoothingMode saveMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.DrawLines(SystemPens.ControlDark, points);

                g.SmoothingMode = saveMode;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rc"></param>
            /// <param name="bPrimary"></param>
            /// <returns></returns>
            private Point[] GetTabPolygon(ref Rectangle rc, bool bPrimary, bool bRtl)
            {
                Point[] points = this.Orientation == Orientation.Horizontal ?
                new Point[]
				{
					new Point(rc.X, rc.Y),
					new Point(rc.X, rc.Bottom - 2),
					new Point(rc.X + 2 , rc.Bottom),
					new Point(rc.Right - TAB_BEVEL_SIZE, rc.Bottom),
					new Point(rc.Right - TAB_BEVEL_SIZE + 4, rc.Bottom - 2),
					new Point(rc.Right, rc.Y)
				} :
                new Point[]
				{
					new Point(rc.X, rc.Y),
					new Point(rc.Right - 2, rc.Y),
					new Point(rc.Right , rc.Y + 2),
					new Point(rc.Right, rc.Bottom - TAB_BEVEL_SIZE),
					new Point(rc.Right - 2, rc.Bottom - TAB_BEVEL_SIZE + 4),
					new Point(rc.X, rc.Bottom)
				};

                if (!bPrimary || bRtl)
                {
                    Matrix m = new Matrix();

                    if (!bPrimary)
                    {
                        m.Rotate(180f);
                        m.Translate(-rc.Left - rc.Right, -rc.Top - rc.Bottom);
                    }

                    if (bRtl)
                    {
                        m.Scale(-1f, 1f);
                        m.Translate(-rc.Left - rc.Right, 0f);
                    }

                    m.TransformPoints(points);
                }

                return points;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="rc"></param>
            /// <param name="bPrimary"></param>
            /// <returns></returns>
            private Brush GetTabBrush(ref Rectangle rc, bool bPrimary, bool bSelected)
            {
                if (!bSelected)
                {
                    LinearGradientBrush brush;

                    if (this.Orientation == Orientation.Horizontal)
                    {
                        brush = new LinearGradientBrush(new Rectangle(rc.X, rc.Y, 1, rc.Height), s_normalTabGradientBegin, s_normalTabGradientEnd, bPrimary ? 90f : -90f);
                    }
                    else
                    {
                        brush = new LinearGradientBrush(new Rectangle(rc.X, rc.Y, rc.Width, 1), s_normalTabGradientBegin, s_normalTabGradientEnd, bPrimary ? 0f : 180f);
                    }

                    brush.WrapMode = WrapMode.TileFlipXY;
                    return brush;
                }
                return new SolidBrush(s_selectedTabBackground);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rcClip"></param>
            private void DrawGrip(Graphics g, ref Rectangle rcClip)
            {
                Rectangle rc = this.LayoutData.GripBounds;
                if (!rc.IsEmpty && rc.IntersectsWith(rcClip))
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        int x1 = rc.Left;
                        int x2 = rc.Right;

                        g.DrawLine(SystemPens.ControlLightLight, x1, rc.Top, x2, rc.Top);
                        g.DrawLine(SystemPens.ControlDark, x1, rc.Top + 1, x2, rc.Top + 1);

                        g.DrawLine(SystemPens.ControlLightLight, x1, rc.Bottom - 2, x2, rc.Bottom - 2);
                        g.DrawLine(SystemPens.ControlDark, x1, rc.Bottom - 1, x2, rc.Bottom - 1);
                    }
                    else
                    {
                        int y1 = rc.Top;
                        int y2 = rc.Bottom;

                        g.DrawLine(SystemPens.ControlLightLight, rc.Left, y1, rc.Left, y2);
                        g.DrawLine(SystemPens.ControlDark, rc.Left + 1, y1, rc.Left + 1, y2);

                        g.DrawLine(SystemPens.ControlLightLight, rc.Right - 2, y1, rc.Right - 2, y2);
                        g.DrawLine(SystemPens.ControlDark, rc.Right - 1, y1, rc.Right - 1, y2);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rcClip"></param>
            /// <param name="button"></param>
            private void DrawButton(Graphics g, ref Rectangle rcClip, int button)
            {
                Rectangle rc = this.LayoutData.GetButtonRect(button);
                if (!rc.IsEmpty && rc.IntersectsWith(rcClip))
                {
                    Color backColor = GetButtonBackground(button);

                    if (backColor != Color.Empty)
                    {
                        using (Brush brush = new SolidBrush(backColor))
                        {
                            g.FillRectangle(brush, rc);
                        }
                        g.DrawRectangle(SystemPens.Highlight, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                    }

                    Image image = m_buttons.Images[button];

                    if (button >= BT_COLLAPSED && this.Orientation == Orientation.Vertical)
                    {
                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }

                    bool bRotate = this.RightToLeft == RightToLeft.Yes && this.Orientation == Orientation.Vertical;

                    if (button == BT_COLLAPSED && (this.Collapsed ^ bRotate))
                    {
                        image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }

                    g.DrawImage(image, rc.X + IMAGE_PADDING, rc.Y + IMAGE_PADDING);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="button"></param>
            private void InvalidateButton(int button)
            {
                if (button != BT_NONE)
                {
                    Invalidate(this.LayoutData.GetButtonRect(button));
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="button"></param>
            /// <returns></returns>
            private Color GetButtonBackground(int button)
            {
                int checkedButton = this.CheckedButton;

                if (button == m_selectedButton)
                {
                    if (m_pressedButton == BT_NONE)
                        return (button == checkedButton) ? s_pressedButtonBackground : s_selectedButtonBackground;

                    if (button == m_pressedButton)
                        return s_pressedButtonBackground;

                    if (button == checkedButton)
                        return s_checkedButtonBackground;

                    return Color.Empty;
                }

                if (button == m_pressedButton)
                    return (button == checkedButton) ? s_pressedButtonBackground : s_selectedButtonBackground;

                if (button == checkedButton)
                    return s_checkedButtonBackground;

                return Color.Empty;
            }

            /// <summary>
            /// 
            /// </summary>
            private void DrawSplitBar(Rectangle rc)
            {
                if (m_rcSplitBar != rc)
                {
                    IntPtr hdc = NativeMethods.GetWindowDC(m_owner.Handle);

                    if (hdc != IntPtr.Zero)
                    {
                        IntPtr brush = GetDragBrush();
                        if (brush != IntPtr.Zero)
                        {
                            IntPtr oldBrush = NativeMethods.SelectObject(hdc, brush);

                            if (!m_rcSplitBar.IsEmpty)
                            {
                                NativeMethods.PatBlt(hdc, m_rcSplitBar.X, m_rcSplitBar.Y, m_rcSplitBar.Width, m_rcSplitBar.Height, PATINVERT);
                            }

                            if (!rc.IsEmpty)
                            {
                                NativeMethods.PatBlt(hdc, rc.X, rc.Y, rc.Width, rc.Height, PATINVERT);
                            }

                            NativeMethods.SelectObject(hdc, oldBrush);
                            NativeMethods.DeleteObject(brush);
                        }
                        NativeMethods.ReleaseDC(m_owner.Handle, hdc);
                    }
                    m_rcSplitBar = rc;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="pt"></param>
            /// <returns></returns>
            private Rectangle GetSplitBarBounds(int x, int y)
            {
                Point p = new Point(x, y);
                Rectangle rc = m_owner.ClientRectangle;

                p = m_owner.PointToClient(PointToScreen(p));
                if (m_orientation == Orientation.Horizontal)
                {
                    rc.Y = p.Y - BAR_THICKNESS / 2;
                    if (rc.Y > rc.Height - BAR_THICKNESS)
                    {
                        rc.Y = rc.Height - BAR_THICKNESS;
                    }
                    if (rc.Y < 0)
                    {
                        rc.Y = 0;
                    }
                    rc.Height = BAR_THICKNESS;
                }
                else
                {
                    rc.X = p.X - BAR_THICKNESS / 2;
                    if (rc.X > rc.Width - BAR_THICKNESS)
                    {
                        rc.X = rc.Width - BAR_THICKNESS;
                    }
                    if (rc.X < 0)
                    {
                        rc.X = 0;
                    }
                    rc.Width = BAR_THICKNESS;
                }
                return rc;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x1"></param>
            /// <param name="x2"></param>
            /// <returns></returns>
            private static int GetDistance(int x1, int x2)
            {
                return x1 > x2 ? x1 - x2 : x2 - x1;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            private static IntPtr GetDragBrush()
            {
                IntPtr brush = IntPtr.Zero;

                IntPtr bmp = NativeMethods.CreateBitmap(8, 8, 1, 1, s_pattern);
                if (bmp != IntPtr.Zero)
                {
                    NativeMethods.LOGBRUSH lb = new NativeMethods.LOGBRUSH();
                    lb.lbColor = 0;//Black
                    lb.lbStyle = BS_PATTERN;
                    lb.lbHatch = bmp;

                    brush = NativeMethods.CreateBrushIndirect(ref lb);

                    NativeMethods.DeleteObject(bmp);
                }
                return brush;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            private void UpdateToolTip(int x, int y)
            {
                string sToolTip = null;

                TabSplitterPage page = this.LayoutData.GetPage(x, y);
                if (page != null)
                {
                    sToolTip = page.ToolTip;
                }
                else
                {
                    switch (this.LayoutData.GetButtonID(x, y))
                    {
                        case BT_VERTICAL:
                            sToolTip = SR.GetString("TabSplitterVerticalSplit");
                            break;
                        case BT_HORIZONTAL:
                            sToolTip = SR.GetString("TabSplitterHorizontalSplit");
                            break;
                        case BT_COLLAPSED:
                            sToolTip = SR.GetString(this.Collapsed ? "TabSplitterExpandPane" : "TabSplitterCollapsePane");
                            break;
                        case BT_SWAP:
                            sToolTip = SR.GetString("TabSplitterSwapPanes");
                            break;
                    }
                }

                if (m_tooltip.GetToolTip(this) != sToolTip)
                {
                    m_tooltip.Active = false;
                    m_tooltip.SetToolTip(this, sToolTip);
                    m_tooltip.Active = true;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hCursor"></param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
            static extern IntPtr SetCursor(IntPtr hCursor);
            #endregion

            #region Events
            /// <summary>
            /// 
            /// </summary>
            public event EventHandler OrientationChanged;

            /// <summary>
            /// 
            /// </summary>
            internal event CancelEventHandler PositionChanging;
            /// <summary>
            /// 
            /// </summary>
            internal event CancelEventHandler OrientationChanging;
            /// <summary>
            /// 
            /// </summary>
            internal event CancelEventHandler CollapsedChanging;
            /// <summary>
            /// 
            /// </summary>
            internal event CancelEventHandler SwappedChanging;
            #endregion

            #region Fields
            /// <summary>
            /// 
            /// </summary>
            TabSplitterContainer m_owner;

            /// <summary>
            /// 
            /// </summary>
            private int m_splitterHeight = -1;

            /// <summary>
            /// 
            /// </summary>
            private double m_relativePos = 0.5;
            /// <summary>
            /// 
            /// </summary>
            private double m_prevRelativePos = 0.5;
            /// <summary>
            /// Splitter orientation.
            /// </summary>
            private Orientation m_orientation = Orientation.Horizontal;

            /// <summary>
            /// 
            /// </summary>
            private bool m_bMoving = false;
            /// <summary>
            /// Mouse coordinates of the last MouseDown event
            /// </summary>
            private Point m_pLastMouseDown = Point.Empty;
            /// <summary>
            /// 
            /// </summary>
            private Rectangle m_rcSplitBar = Rectangle.Empty;
            /// <summary>
            /// 
            /// </summary>
            private LayoutInfo m_layoutData = null;
            /// <summary>
            /// 
            /// </summary>
            private int m_selectedButton = BT_NONE;
            /// <summary>
            /// 
            /// </summary>
            private int m_pressedButton = BT_NONE;
            /// <summary>
            /// 
            /// </summary>
            private TabSplitterPage m_pressedPanel = null;
            /// <summary>
            /// 
            /// </summary>
            private ToolTip m_tooltip;

            /// <summary>
            /// 
            /// </summary>
            private static short[] s_pattern = { 0x55, 0xaa, 0x55, 0xaa, 0x55, 0xaa, 0x55, 0xaa };

            /// <summary>
            /// 
            /// </summary>
            private static ImageList m_buttons;
            /// <summary>
            /// 
            /// </summary>
            private static Blend m_blend;
            /// <summary>
            /// Button colors
            /// </summary>
            private static Color s_selectedButtonBackground = Color.FromArgb(0xC1, 0xD2, 0xEE);
            private static Color s_pressedButtonBackground = Color.FromArgb(0x98, 0xB5, 0xE2);
            private static Color s_checkedButtonBackground = Color.FromArgb(0xE1, 0xE6, 0xE8);
            /// <summary>
            /// Tab colors
            /// </summary>
            private static Color s_normalTabGradientBegin = Color.FromArgb(0xFA, 0xFA, 0xF5);
            private static Color s_normalTabGradientEnd = Color.FromArgb(0xED, 0xEA, 0xDA);
            private static Color s_selectedTabBackground = Color.FromArgb(0xFF, 0xFF, 0xFF);
            #endregion

            #region *** LayoutInfo
            /// <summary>
            /// 
            /// </summary>
            class LayoutInfo
            {
                #region Constructor
                /// <summary>
                /// 
                /// </summary>
                /// <param name="owner"></param>
                public LayoutInfo(TabSplitter splitter)
                {
                    m_splitter = splitter;
                }
                #endregion

                #region Properties
                /// <summary>
                /// 
                /// </summary>
                public Rectangle GripBounds
                {
                    get { return m_gripBounds; }
                }
                #endregion

                #region Methods
                /// <summary>
                /// 
                /// </summary>
                /// <param name="splitter"></param>
                public void PerformLayout()
                {
                    Clear();

                    Rectangle rc = m_splitter.ClientRectangle;
                    if (rc.Width > 0 && rc.Height > 0)
                    {
                        if (m_splitter.Orientation == Orientation.Horizontal)
                        {
                            PerformLayoutHorizontal(ref rc);
                        }
                        else
                        {
                            PerformLayoutVertical(ref rc);
                        }
                    }
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="p"></param>
                /// <returns></returns>
                public int GetButtonID(int x, int y)
                {
                    for (int i = 0; i < BT_NONE; i++)
                    {
                        if (m_rcButtons[i].Contains(x, y))
                        {
                            return i;
                        }
                    }
                    return BT_NONE;
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public TabSplitterPage GetPage(int x, int y)
                {
                    bool bCollapsed = m_splitter.Collapsed;

                    if (bCollapsed)
                    {
                        for (int i = m_secondaryBounds.Count - 1; i >= 0; i--)
                        {
                            if (((Rectangle)m_secondaryBounds[i]).Contains(x, y))
                                return m_splitter.SecondaryItemsInternal[i] as TabSplitterPage;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_secondaryBounds.Count; i++)
                        {
                            if (((Rectangle)m_secondaryBounds[i]).Contains(x, y))
                                return m_splitter.SecondaryItemsInternal[i] as TabSplitterPage;
                        }
                    }
                    for (int i = m_primaryBounds.Count - 1; i >= 0; i--)
                    {
                        if (((Rectangle)m_primaryBounds[i]).Contains(x, y))
                            return m_splitter.PrimaryItemsInternal[i] as TabSplitterPage;
                    }
                    return null;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="buttonID"></param>
                /// <returns></returns>
                public Rectangle GetButtonRect(int buttonID)
                {
                    return m_rcButtons[buttonID];
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="tabID"></param>
                /// <returns></returns>
                public Rectangle GetPrimaryTabRect(int tabID)
                {
                    if (tabID >= 0 && tabID < m_primaryBounds.Count)
                    {
                        return (Rectangle)m_primaryBounds[tabID];
                    }
                    return Rectangle.Empty;
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="tabID"></param>
                /// <returns></returns>
                public Rectangle GetSecondaryTabRect(int tabID)
                {
                    if (tabID >= 0 && tabID < m_secondaryBounds.Count)
                    {
                        return (Rectangle)m_secondaryBounds[tabID];
                    }
                    return Rectangle.Empty;
                }
                #endregion

                #region Implementation

                /// <summary>
                /// 
                /// </summary>
                private void Clear()
                {
                    m_primaryBounds.Clear();
                    m_secondaryBounds.Clear();

                    for (int i = 0; i < BT_NONE; i++)
                    {
                        m_rcButtons[i] = Rectangle.Empty;
                    }
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="rc"></param>
                private void PerformLayoutHorizontal(ref Rectangle rc)
                {
                    using (Bitmap image = new Bitmap(1, 1))
                    {
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            int x = rc.X + LEFT_PADDING;
                            int y = rc.Y;
                            int height = rc.Height - TAB_MARGIN;

                            Size szButton = m_splitter.ButtonSize;

                            foreach (TabSplitterPage page in m_splitter.PrimaryItemsInternal)
                            {
                                int textWidth = (int)(g.MeasureString(page.Text, m_splitter.Font).Width) + 1;

                                Rectangle rcTab = GetLayoutRect(x, y, textWidth + TAB_IMAGE_SIZE + 2 * TAB_PADDING + TAB_BEVEL_SIZE, height);
                                if (rc.IntersectsWith(rcTab))
                                {
                                    m_primaryBounds.Add(rcTab);
                                    x += rcTab.Width - TAB_OVERLAP_SIZE;
                                }
                                else break;
                            }

                            if (!m_splitter.Collapsed)
                            {
                                x += TAB_OVERLAP_SIZE;

                                Rectangle rcSwap = GetLayoutRect(x, rc.Y + (rc.Height - szButton.Height) / 2, szButton.Width, szButton.Height);
                                if (rc.IntersectsWith(rcSwap))
                                {
                                    m_rcButtons[BT_SWAP] = rcSwap;
                                    x += rcSwap.Width;
                                }

                                y += TAB_MARGIN - 1;
                                height += 1;
                            }

                            foreach (TabSplitterPage page in m_splitter.SecondaryItemsInternal)
                            {
                                int textWidth = (int)(g.MeasureString(page.Text, m_splitter.Font).Width) + 1;

                                Rectangle rcTab = GetLayoutRect(x, y, textWidth + TAB_IMAGE_SIZE + 2 * TAB_PADDING + TAB_BEVEL_SIZE, height);
                                if (rc.IntersectsWith(rcTab))
                                {
                                    m_secondaryBounds.Add(rcTab);
                                    x += rcTab.Width - TAB_OVERLAP_SIZE;
                                }
                                else break;
                            }

                            x += TAB_OVERLAP_SIZE;

                            int gripLeft = x + 1;
                            int gripRight = gripLeft + GRIP_WIDTH;

                            int buttonsLeft = rc.Right - 3 * szButton.Width - 1;
                            int buttonsTop = rc.Y + (rc.Height - szButton.Height) / 2;

                            if (buttonsLeft <= gripRight)
                            {
                                buttonsLeft = gripRight + 1;
                            }
                            else
                            {
                                gripLeft += (buttonsLeft - gripRight) / 2;
                            }

                            m_gripBounds = GetLayoutRect(gripLeft, rc.Top + (rc.Height - GRIP_HEIGHT) / 2, GRIP_WIDTH, GRIP_HEIGHT);

                            for (int i = BT_VERTICAL; i <= BT_COLLAPSED; i++)
                            {
                                m_rcButtons[i] = GetLayoutRect(buttonsLeft, buttonsTop, szButton.Width, szButton.Height);
                                buttonsLeft += szButton.Width;
                            }
                        }
                    }
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="rc"></param>
                private void PerformLayoutVertical(ref Rectangle rc)
                {
                    int x = rc.X;
                    int y = rc.Y + LEFT_PADDING;
                    int width = rc.Width - TAB_MARGIN;

                    Size szButton = m_splitter.ButtonSize;

                    bool bRtl = m_splitter.RightToLeft == RightToLeft.Yes;

                    if (bRtl)
                    {
                        width += 1;
                    }

                    for (int page = 0; page < m_splitter.PrimaryItemsInternal.Count; ++page)
                    {
                        Rectangle rcTab = GetLayoutRect(x, y, width, TAB_IMAGE_SIZE + 2 * TAB_PADDING + TAB_BEVEL_SIZE);
                        if (rc.IntersectsWith(rcTab))
                        {
                            m_primaryBounds.Add(rcTab);
                            y += rcTab.Height - TAB_OVERLAP_SIZE;
                        }
                        else break;
                    }

                    if (!m_splitter.Collapsed)
                    {
                        y += TAB_OVERLAP_SIZE;

                        Rectangle rcSwap = GetLayoutRect(rc.X + (rc.Width - szButton.Width) / 2, y, szButton.Width, szButton.Height);

                        if (rc.IntersectsWith(rcSwap))
                        {
                            m_rcButtons[BT_SWAP] = rcSwap;
                            y += rcSwap.Height;
                        }

                        x += TAB_MARGIN;

                        if (bRtl)
                        {
                            width -= 1;
                        }
                        else
                        {
                            x -= 1;
                            width += 1;
                        }
                    }

                    for (int page = 0; page < m_splitter.SecondaryItemsInternal.Count; ++page)
                    {
                        Rectangle rcTab = GetLayoutRect(x, y, width, TAB_IMAGE_SIZE + 2 * TAB_PADDING + TAB_BEVEL_SIZE);
                        if (rc.IntersectsWith(rcTab))
                        {
                            m_secondaryBounds.Add(rcTab);
                            y += rcTab.Height - TAB_OVERLAP_SIZE;
                        }
                        else break;
                    }

                    y += TAB_OVERLAP_SIZE;

                    int gripTop = y + 1;
                    int gripBottom = gripTop + GRIP_WIDTH;

                    int buttonsLeft = rc.X + (rc.Width - szButton.Width) / 2;
                    int buttonsTop = rc.Bottom - 3 * szButton.Height - 1;

                    if (buttonsTop <= gripBottom)
                    {
                        buttonsTop = gripBottom + 1;
                    }
                    else
                    {
                        gripTop += (buttonsTop - gripBottom) / 2;
                    }

                    m_gripBounds = GetLayoutRect(rc.Left + (rc.Width - GRIP_HEIGHT) / 2, gripTop, GRIP_HEIGHT, GRIP_WIDTH);

                    for (int i = BT_VERTICAL; i <= BT_COLLAPSED; i++)
                    {
                        m_rcButtons[i] = GetLayoutRect(buttonsLeft, buttonsTop, szButton.Width, szButton.Height);
                        buttonsTop += szButton.Height;
                    }
                }
                /// <summary>
                /// 
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <param name="width"></param>
                /// <param name="height"></param>
                /// <returns></returns>
                private Rectangle GetLayoutRect(int x, int y, int width, int height)
                {
                    Rectangle rc = new Rectangle(x, y, width, height);

                    if (m_splitter.RightToLeft == RightToLeft.Yes)
                    {
                        rc.X = m_splitter.ClientSize.Width - x - width;
                    }

                    return rc;
                }
                #endregion

                #region Fields
                /// <summary>
                /// 
                /// </summary>
                TabSplitter m_splitter;
                /// <summary>
                /// Tabs rectangles
                /// </summary>
                private ArrayList m_primaryBounds = new ArrayList();
                private ArrayList m_secondaryBounds = new ArrayList();
                private Rectangle[] m_rcButtons = new Rectangle[BT_NONE];
                private Rectangle m_gripBounds;
                #endregion
            }
            #endregion
        }
        #endregion

        #region *** PositionEventArgs
        internal class PositionEventArgs : CancelEventArgs
        {
            public PositionEventArgs(int pos)
                : base(false)
            {
                Position = pos;
            }
            public int Position;
        }
        #endregion

        #region *** OrientationEventArgs
        internal class OrientationEventArgs : CancelEventArgs
        {
            public OrientationEventArgs(Orientation orientation)
                : base(false)
            {
                Orientation = orientation;
            }
            public Orientation Orientation;
        }
        #endregion

        #region *** CollapsedEventArgs
        internal class CollapsedEventArgs : CancelEventArgs
        {
            public CollapsedEventArgs(bool collapsed)
                : base(false)
            {
                Collapsed = collapsed;
            }
            public bool Collapsed;
        }
        #endregion

        #region *** SwappedEventArgs
        internal class SwappedEventArgs : CancelEventArgs
        {
            public SwappedEventArgs(bool swapped)
                : base(false)
            {
                Swapped = swapped;
            }
            public bool Swapped;
        }
        #endregion
    }
}
