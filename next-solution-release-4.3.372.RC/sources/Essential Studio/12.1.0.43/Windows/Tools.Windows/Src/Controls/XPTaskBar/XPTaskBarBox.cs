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

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Collections;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Provides data for the XPTaskBarBox's <see cref="XPTaskBarBox.ItemClick"/> event.
    /// </summary>
    public class XPTaskBarItemClickArgs : EventArgs
    {
        private XPTaskBarItem item;
        /// <summary>
        /// Returns the item that was clicked.
        /// </summary>
        public XPTaskBarItem XPTaskBarItem
        {
            get { return item; }
        }
        /// <summary>
        /// Creates a new instance of the XPTaskBarBox class.
        /// </summary>
        /// <param name="item">The item that was clicked.</param>
        public XPTaskBarItemClickArgs(XPTaskBarItem item)
        {
            this.item = item;
        }
    }

    /// <summary>
    /// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.ItemClick"/>
    /// event in the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name=" e">An <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarItemClickArgs"/> that contains the event data.</param>
    public delegate void XPTaskBarItemClickHandler(object sender, XPTaskBarItemClickArgs e);

    /// <summary>
    /// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.ProvideItemsBackgroundBrush"/>
    /// and <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.ProvideHeaderBackgroundBrush"/> event.
    /// </summary>
    /// <remarks>
    /// If the bounds can be represented by a Rectangle then the
    /// Bounds property will have a valid value. If it cannot be
    /// represented by a Rectangle then the Path property will have a
    /// valid value.
    /// </remarks>
    public class ProvideBrushEventArgs : EventArgs
    {
        private Brush brush;
        private Rectangle itemBounds;
        private GraphicsPath path;
        /// <summary>
        /// Creates an instance of the ProvideBrushEventArgs class.
        /// </summary>
        /// <param name="bounds">The bounds for which a brush is requested.</param>
        /// <param name="path">The GraphicsPath for which a brush is requested.</param>
        public ProvideBrushEventArgs(Rectangle bounds, GraphicsPath path)
        {
            this.itemBounds = bounds;
            this.path = path;
        }
        /// <summary>
        /// Returns the bounds for which a brush is requested.
        /// </summary>
        /// <value>The Rectangle specifying the bounds.</value>
        public Rectangle Bounds
        {
            get { return this.itemBounds; }
        }

        /// <summary>
        /// Returns the GraphicsPath for which a brush is requested.
        /// </summary>
        /// <value>A GraphicsPath object.</value>
        public GraphicsPath Path
        {
            get { return this.path; }
        }
        /// <summary>
        /// Gets / sets the Brush that will be used to draw the specified
        /// Bounds or Path.
        /// </summary>
        /// <value>A Brush object.</value>
        /// <remarks>The event handler should set this property for it
        /// to be used while drawing the specified bounds.</remarks>
        public Brush Brush
        {
            get { return this.brush; }
            set { this.brush = value; }
        }
    }

    /// <summary>
    /// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.ProvideItemsBackgroundBrush"/>
    /// and <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.ProvideHeaderBackgroundBrush"/> in the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.ProvideBrushEventArgs"/> that contains the event data.</param>
    public delegate void ProvideBrushEventHandler(object sender, ProvideBrushEventArgs args);

    [Syncfusion.Documentation.DocumentationExclude()]
    public class ThemedXPTaskBarBoxDrawing : ThemedControlDrawing
    {
        XPTaskBarBox taskBarBox = null;
        public ThemedXPTaskBarBoxDrawing(XPTaskBarBox control, string classList)
            : base(classList)
        {
            this.taskBarBox = control;
            this.taskBarBox.HandleDestroyed += new EventHandler(taskBarBox_HandleDestroyed);
        }

        void taskBarBox_HandleDestroyed(object sender, EventArgs e)
        {
            if (this.taskBarBox != null)
            {
                this.taskBarBox.HandleDestroyed -= new EventHandler(taskBarBox_HandleDestroyed);
                this.taskBarBox = null;
            }
        }

        public void DrawHeader(Graphics g, Rectangle rect)
        {
            this.DrawThemeBackground(g, ThemeParts.EBP_NORMALGROUPHEAD,
                0, rect);
        }
        public void DrawItemsBG(Graphics g, Rectangle rect)
        {
            this.DrawThemeBackground(g, ThemeParts.EBP_NORMALGROUPBACKGROUND, 0, rect);
        }
        public void DrawButton(Graphics g, Rectangle rect, bool expand)
        {
            int state = ThemeStates.EBNGE_NORMAL;
            if (this.taskBarBox.HeaderHit && this.taskBarBox.ShouldToggle())
                state = ThemeStates.EBNGE_HOT;
            if (Control.MouseButtons == MouseButtons.Left && this.taskBarBox.HeaderHit && this.taskBarBox.ShouldToggle())
                state = ThemeStates.EBNGE_PRESSED;

            this.DrawThemeBackground(g, expand ? ThemeParts.EBP_NORMALGROUPEXPAND : ThemeParts.EBP_NORMALGROUPCOLLAPSE,
                state, rect);
        }
        public Size GetHeaderButtonSize()
        {
            Graphics g = this.taskBarBox.CreateGraphics();
            Size sz = this.GetPartSize(g, ThemeParts.EBP_NORMALGROUPEXPAND, ThemeStates.EBHC_NORMAL, (int)THEMESIZE.TS_DRAW);
            g.Dispose();
            return sz;
        }
        public Rectangle GetBarTextExtent(String text, Rectangle boundingRect, bool forheader)
        {
            Graphics g = this.taskBarBox.CreateGraphics();
            Rectangle extent = this.GetTextExtent(g, forheader ? ThemeParts.EBP_NORMALGROUPHEAD : ThemeParts.EBP_NORMALGROUPBACKGROUND,
                0, text, boundingRect,
                DrawTextFormats.DT_CALCRECT | DrawTextFormats.DT_LEFT | DrawTextFormats.DT_WORDBREAK);
            g.Dispose();
            return extent;
        }
        public void DrawBarText(Graphics g, String text, Rectangle boundingRect, bool forheader, bool hot, bool bIsMirrored, bool bClipText)
        {
            int nFormats = DrawTextFormats.DT_WORDBREAK;
            if (bIsMirrored)
            {
                nFormats |= (DrawTextFormats.DT_RIGHT | DrawTextFormats.DT_RTLREADING);
            }
            else
            {
                nFormats |= DrawTextFormats.DT_LEFT;
            }

            if (!bClipText)
                nFormats |= (DrawTextFormats.DT_END_ELLIPSIS | DrawTextFormats.DT_MODIFYSTRING);

            this.DrawThemeText(g, forheader ? ThemeParts.EBP_NORMALGROUPHEAD : ThemeParts.EBP_NORMALGROUPBACKGROUND,
                0, text, boundingRect, nFormats, 0);
        }

        public void DrawBarText(Graphics g, String text, Rectangle boundingRect, bool forheader, bool hot, bool bIsMirrored, StringAlignment alignmentFormat, bool bClipText)
        {
            int nFormats = DrawTextFormats.DT_WORDBREAK;

            if (bIsMirrored)
            {
                nFormats |= DrawTextFormats.DT_RTLREADING;
            }

            switch (alignmentFormat)
            {
                case StringAlignment.Near:
                    if (bIsMirrored)
                    {
                        nFormats |= DrawTextFormats.DT_RIGHT;
                    }
                    else
                    {
                        nFormats |= DrawTextFormats.DT_LEFT;
                    }
                    break;

                case StringAlignment.Center:
                    nFormats |= DrawTextFormats.DT_CENTER;
                    break;

                case StringAlignment.Far:
                    if (bIsMirrored)
                    {
                        nFormats |= DrawTextFormats.DT_LEFT;
                    }
                    else
                    {
                        nFormats |= DrawTextFormats.DT_RIGHT;
                    }
                    break;
            }

            if (!bClipText)
                nFormats |= (DrawTextFormats.DT_END_ELLIPSIS | DrawTextFormats.DT_MODIFYSTRING);

            this.DrawThemeText(g, forheader ? ThemeParts.EBP_NORMALGROUPHEAD : ThemeParts.EBP_NORMALGROUPBACKGROUND,
                0, text, boundingRect, nFormats, 0);
        }

        public Color GetTextThemeColor(bool header, bool hot)
        {
            int part = header ? ThemeParts.EBP_NORMALGROUPHEAD : ThemeParts.EBP_NORMALGROUPBACKGROUND;
            int state = 0;
            ulong color = 0;
            NativeMethods.GetThemeColor(this.HTheme, part, state, 3803 /*TMT_TEXTCOLOR*/, ref color);
            int rgb = NativeMethods.COLORREFToRGB((int)color);
            return Color.FromArgb(NativeMethods.GetRValue(rgb),
                NativeMethods.GetGValue(rgb),
                NativeMethods.GetBValue(rgb));
        }
    }
    /// <summary>
    /// Represents a task bar box within the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBar"/>.
    /// </summary>
    /// <seealso cref="Syncfusion.Windows.Forms.Tools.XPTaskBar"/>
    /// <remarks>
    /// <para>The <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/> contains 
    /// a header and a content area. </para>
    /// <para>The content area further contains 2 portions.
    /// The first portion(Task-List portion) shows a list of clickable tasks represented by the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarItem"/>s.
    /// The second portion(Panel portion) shows a <see cref="System.Windows.Forms.Panel"/> in the content area, if there is a child Panel
    /// added to the child Controls list. Note that the <see cref="XPTaskBarBox"/> can
    /// contain only one Panel. If you try to add more than one Panel to the <see cref="System.Windows.Forms.Control.Controls"/> list,
    /// an exception will be thrown.</para>
    /// <para>This control is XP Themes aware and if themes support is turned ON
    /// (<see cref="Syncfusion.Windows.Forms.Tools.XPTaskBar.ThemesEnabled"/> property set to true), it will draw
    /// the header and the content portion using themes in XP. Optionally, you can owner draw
    /// the header portion and the content portion with a 
    /// custom brush to create a Windows XP task menu look-and-feel in other platforms.</para>
    /// </remarks>
    /// <example>
    /// Take a look at the XPTaskBar class reference for an example on how to use this class.
    /// </example>
    [
    Designer(
        typeof(XPTaskBarBoxDesigner),
        typeof(IDesigner)),
    ToolboxItem(false),
    DefaultEvent("ItemClick")
    ]
    public class XPTaskBarBox : Control, IProvideLayoutInformation
    {
        #region tooltip processing implementation

        private bool m_bIsItemClicked = false;

        /// <summary>
        /// Indicates whether tooltip is shown.
        /// </summary>
        private bool m_bShowToolTip = false;

        /// <summary>
        /// Representes the Tooltip control.
        /// </summary>
        private ToolTip m_toolTip = null;

        /// <summary>
        /// Tooltip initialization.
        /// </summary>
        private void InitializeToolTip()
        {
            m_toolTip = new ToolTip();
            m_toolTip.ShowAlways = true;
            m_toolTip.ReshowDelay = 0;
            m_toolTip.InitialDelay = 0;
            m_toolTip.Active = false;
        }

        /// <summary>
        /// Retrieves the XPTaskBarItem item at point.
        /// </summary>
        /// <param name="pt">Point ( in screen coordinates ) to get item from.</param>
        protected XPTaskBarItem GetItemAtPoint(Point pt)
        {
            if (items.Count == 0) return null;

            Point mousePos = PointToClient(Control.MousePosition);
            XPTaskBarItem item = null;

            for (int i = 0, len = items.Count; i < len; i++)
            {
                item = items[i];
                if (!item.Bounds.Contains(mousePos)) continue;

                return item;
            }

            return null;
        }
        /// <summary>
        /// Indicates whether tooltip is shown.
        /// </summary>
        [DefaultValue(false), Description("Indicates whether tooltip is shown.")]
        [Browsable(true)]
        public bool ShowToolTip
        {
            get
            {
                return m_bShowToolTip;
            }
            set
            {
                if (value != m_bShowToolTip)
                {
                    m_bShowToolTip = value;

                    if (!m_bShowToolTip) m_toolTip.Active = false;
                }
            }
        }

        #endregion

        #region FIELDS
        private bool m_bToggleByButton = false;
        private bool m_bWrapHeaderText = false;
        private bool m_bPropagateBackColorToItems = false;
        private Color hotTrackColor = Color.Empty;

        /// <summary>
        /// Indicates whether the XpTaskBarbaritem's text should get wrapped or not
        /// </summary>
        private bool m_Wraptext = false;
        /// <summary>
        /// Indicates whether the XpTaskBarBox expands or collapses only when
        /// the Collapse Button is clicked or always when the header is clicked.
        /// </summary>
        [DefaultValue(false),
       Category("Appearance"),
       Description("Gets or Sets, should XpTaskBarBox expand or collapse only when the Collapse Button is clicked, or allways when header is clicked.")]
        public bool ToggleByButton
        {
            get
            {
                return m_bToggleByButton;
            }
            set
            {
                if (value != m_bToggleByButton)
                {
                    m_bToggleByButton = value;
                }
            }
        }
        /// <summary>
        /// Indicates whether the item's text in XPTaskBarBox should get wrapped 
        /// </summary>
        [Category("Behavior"), Description("Indicates whether the XPTaskBarItem's text should get wrapped or not")]
        public bool WrapText
        {
            get
            {
                return m_Wraptext;
            }
            set
            {
                if (value != m_Wraptext)
                {
                    m_Wraptext = value;
                }
            }
        }
        /// <summary>
        /// Serializes the Wraptext value
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeWrapText()
        {
            return this.WrapText != false;
        }
        /// <summary>
        /// resets the wraptext value
        /// </summary>
        public void ResetWrapText()
        {
            this.WrapText = false;
        }
        private Color headerBackColor;
        private Color itemBackColor;
        private XPTaskBarItemsCollection items;
        private ImageList imageList = null;
        private ImageListAdv imageListAdv = new ImageListAdv ();
        private ImageListAdv headerImageListAdv = new ImageListAdv();
        private ImageList headerImageList = null;
        private int headerImageIndex = -1;
        private bool showCollapseButton = true;
        private XPTaskBarItem hitItemOnMouseDown = null;
        private Font headerFont = null;
        private Color headerForeColor = SystemColors.ControlText;
        private bool m_bDrawFocusRect = true;
        private XPTaskBarItem focusedItem = null;
        private XPTaskBarItem selectedItem = null;

        /// <summary>
        /// Indicates whether themed drawing should be simulated for color schemes other than the default blue.
        /// </summary>
        /// <remarks>This is necessary because the themes API uses the blue color scheme to draw
        /// the background colors for all theme colors. Turn this off if you want to use the blue color scheme for
        /// all the other color schemes.</remarks>
        public static bool SimulateThemedPaintingForNonDefaultThemes = true;
        /// <summary>
        /// Specifies the animation delay when the task box is opened/closed. Default is 50ms.
        /// </summary>
        public static int AnimationDelayInMilliSeconds = 50;
        /// <summary>
        /// Specifies the total animation positions. Default is 10.
        /// </summary>
        public static int AnimationPositions = 10;

        private int animationDelay = 50;
        private int animationPositionsCount = 10;

        private int padx = 5;
        private int pady = 5;
        private readonly int EXP_BUTTON_SIZE = 16;
        private int headerHeight = 0;
        private int extendedHeaderImageHeight = 0;
        private int preferredHeight = 0;
        private ExpanderButton button = new ExpanderButton();
        internal bool allowBoundsChange = false;
        private bool needLayout = false;
        private AnimationHelper animationHelper;
        private WeakReference m_weakThemedDrawing;
        private int preferredChildPanelHeight = 30;
        private bool focusTextBounds = false;

        /// <summary>
        /// Specifies the text header alignment.
        /// </summary>
        private StringAlignment m_HeaderTextAlign = StringAlignment.Near;
        /// <summary>
        /// Specifies the header direction.
        /// </summary>
        private HeaderDirectionFormat m_HeaderDirection = HeaderDirectionFormat.LeftToRight;

        /// <summary>
        /// Indicate whether occurs same action about XPTaskBarItem.
        /// </summary>
        private ItemAction m_enItemAction = ItemAction.None;

        /// <summary>
        /// Item which animating.
        /// </summary>
        private XPTaskBarItem m_animatedItem = null;

        /// <summary>
        /// Indicate whether the items added or removed it's drawing with animation.
        /// </summary>
        private bool m_bUseAdditionalAnimation = false;

        /// <summary>
        /// Name for visible property.
        /// </summary>
        private const string DEF_VISIBLE_PROPERTY_NAME = "Visible";

        #endregion FIELDS

        #region Enums

        /// <summary>
        /// Specifies the header direction format.
        /// </summary>
        public enum HeaderDirectionFormat
        {
            LeftToRight, // Normal logic.
            RightToLeft  // Mirrored logic.
        }
        #endregion

        /// <summary>
        /// Creates a new instance of the XPTaskBarBox.
        /// </summary>
        public XPTaskBarBox()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(XPTaskBarBox));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            this.animationDelay = XPTaskBarBox.AnimationDelayInMilliSeconds;
            this.animationPositionsCount = XPTaskBarBox.AnimationPositions;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, true);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.button.ExpandedState = false;

            items = new XPTaskBarItemsCollection();
            items.CollectionChanged += new CollectionChangeEventHandler(this.TaskMenuItems_CollectionChanged);
            items.ItemPropertyChanged += new SyncfusionPropertyChangedEventHandler(this.TaskMenuItems_PropertyChanged);

            this.animationHelper = new AnimationHelper();
            this.animationHelper.AnimationPositionChanged += new EventHandler(this.AnimationListener);
            this.animationHelper.AnimationDone += new EventHandler(this.AnimationDone);

            InitializeToolTip();
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Make sure to relelase these event handlers
                if (imageList != null)
                {
                    if (imageList != null)
                        imageList.RecreateHandle -= new EventHandler(ImageList_Recreated);

                    imageList = null;
                }
                if (this.animationHelper != null)
                {
                    this.animationHelper.AnimationPositionChanged -= new EventHandler(AnimationListener);
                    this.animationHelper.AnimationDone -= new EventHandler(AnimationDone);
                    animationHelper = null;
                }
                if (items != null)
                {
                    items.CollectionChanged -= new CollectionChangeEventHandler(TaskMenuItems_CollectionChanged);
                    items.ItemPropertyChanged -= new SyncfusionPropertyChangedEventHandler(TaskMenuItems_PropertyChanged);
                    items.Clear();
                    items = null;
                }

                button = null;

                Panel childPanel = this.ChildPanel;
                if (childPanel != null)
                {
                    this.DetachChildPanel(childPanel);
                    childPanel.Dispose();
                }

                if (null != this.CollapsedStateChanged)
                {
                    Delegate[] delegates = this.CollapsedStateChanged.GetInvocationList();
                    foreach (Delegate del in delegates)
                    {
                        this.CollapsedStateChanged -= (EventHandler)del;
                    }
                }

                this.Parent = null;
                if (this.ThemedDrawing != null)
                {
                    this.ThemedDrawing.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void AnimationListener(object sender, EventArgs e)
        {
            OnLayoutNeed(EventArgs.Empty);
        }

        private void OnAnimation(object sender, EventArgs e)
        {
            CancelEventArgs args = new CancelEventArgs();
            this.OnCollapsedStateChanging(args);

            if (!args.Cancel)
            {
                this.OnBeforeAnimation(EventArgs.Empty);
            }
        }
        private void AnimationDone(object sender, EventArgs e)
        {
            this.OnAfterAnimation(EventArgs.Empty);
            this.OnCollapsedStateChanged(EventArgs.Empty);
        }

        Size IProvideLayoutInformation.PreferredSize
        {
            get { return this.Size; }
        }

        Size IProvideLayoutInformation.MinimumSize
        {
            get { return this.Size; }
        }

        /// <summary>
        /// Indicates the state of the task bar box.
        /// </summary>
        /// <value>True if collapsed; false otherwise. Default is false.</value>
        [SRCategory(SR.CategoryAppearance), DefaultValue(false),
        Description("Specifies the state of the task bar box.")]
        public bool Collapsed
        {
            get { return this.button.ExpandedState; }
            set
            {
                if (value == true)
                    this.CollapseContent(false);
                else
                    this.ExpandContent(false);
            }
        }

        /// <summary>
        /// Gets / sets the header direction.
        /// </summary>
        [
            Category("Appearance"),
            Description("Gets or sets header direction."),
            DefaultValue(HeaderDirectionFormat.LeftToRight)
        ]
        public HeaderDirectionFormat HeaderDirection
        {
            get
            {
                return m_HeaderDirection;
            }
            set
            {
                if (m_HeaderDirection != value)
                {
                    m_HeaderDirection = value;
                    UpdateExpanderButtonPosition();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Returns the button in the header used to collapse, expand the task bar box.
        /// </summary>
        protected ExpanderButton ExpanderButton
        {
            get { return this.button; }
        }


        /// <summary>
        /// Gets / sets the horizontal padding provided in pixels between contents of the header and the header's left and right borders.
        /// </summary>
        [DefaultValue(5),
        SRCategory(SR.CategoryAppearance),
        Description("The horizontal padding provided in pixels between contents of the header and the header's left and right borders.")]
        public int PADX
        {
            get { return this.padx; }
            set
            {
                if (this.padx != value)
                {
                    this.padx = value;
                    OnLayoutNeed(new EventArgs());
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets / sets the vertical padding provided in pixels between contents of the header and the header's top and bottom borders.
        /// </summary>
        [SRCategory(SR.CategoryAppearance),
        Description("The vertical padding provided in pixels between contents of the header and the header's top and bottom borders.")]
        public int PADY
        {
            get { return this.pady; }
            set
            {
                if (this.pady != value)
                {
                    this.pady = value;
                    OnLayoutNeed(new EventArgs());
                    Invalidate();
                }
            }
        }

        private bool ShouldSerializePADY()
        {
            int defaultPadY = 5;

            if (this.IsOffice2007Style)
            {
                defaultPadY = 2;
            }

            return PADY != defaultPadY;
        }

        private bool IsOffice2007Style
        {
            get
            {
                return this.Parent != null && !this.Parent.ThemesEnabled && this.Parent.Style == XPTaskBarStyle.Office2007;
            }
        }
        private bool IsOffice2010Style
        {
            get
            {
                return this.Parent != null && !this.Parent.ThemesEnabled && this.Parent.Style == XPTaskBarStyle.Office2010;
            }
        }
        private bool IsMetroStyle
        {
            get
            {
                return this.Parent != null && !this.Parent.ThemesEnabled && this.Parent.Style == XPTaskBarStyle.Metro;
            }
        }
        internal void ResetPADY()
        {
            int defaultPadY = 5;

            if (this.IsOffice2007Style || this.IsOffice2010Style)
            {
                defaultPadY = 2;
            }
            if (this.IsMetroStyle )
            {
                defaultPadY = 2;
            }
            this.pady = defaultPadY;

            this.Invalidate();
        }

        /// <summary>
        /// Indicates whether it's dimensions have changed, requiring a fresh layout.
        /// </summary>
        /// <remarks>
        /// Internal property. You do not have to call this property directly.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public bool NeedLayout
        {
            get { return needLayout; }
        }

        /// <summary>
        /// Gets or sets the hot tracking color.
        /// </summary>
        /// <value>The color of the hot track.</value>
        [DefaultValue(typeof(Color), "Empty"), Description("Indicates the color to be set for hottracking")]
        public Color HotTrackColor
        {
            get { return hotTrackColor; }
            set
            {
                if (hotTrackColor != value)
                    hotTrackColor = value;
            }
        }

        /// <summary>
        /// Lays out it's items.
        /// </summary>
        /// <param name="g">The Graphics object based on which to determine the sizes and positions.</param>
        /// <remarks>
        /// <para>Note that the XPTaskBarBox control follows a different layout pattern from a usual Windows Forms Control.
        /// When requested a Layout by the default Windows Forms Layout event, this control will only mark its
        /// child positions as dirty and recalculate its child positions when a subsequent Paint event occurs,
        /// with a call to this Layout method. This technique is followed to reduce flicker.</para>
        /// </remarks>
        public new virtual void Layout(Graphics g)
        {
            this.UpdatePreferredHeight(g);
        }

        /// </override>
        protected override void OnHandleCreated(EventArgs e)
        {
            // Recalc on handle created, because box bounds are not set until handle is created.
            this.OnLayout(new LayoutEventArgs(this, "Bounds"));

            if (this.Parent != null)
            {
                this.Parent.PerformLayout();
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// Overrides default layout behavior.
        /// </summary>
        /// <param name="sender"></param>
        /// <remarks>
        /// This control will delay laying out itself and its children until the next paint message.
        /// To force a layout call the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.TaskMenuItems_PropertyChanged"/> method.
        /// </remarks>
        //		protected override void OnLayout(LayoutEventArgs levent)
        //		{
        //            System.Diagnostics.Trace.WriteLine(this.Name.ToString() + " OnLayout");
        //
        //			OnLayoutNeed( new EventArgs() );
        ////			SetNeedLayout(true);
        //			base.OnLayout(levent);
        //		}


        private void TaskMenuItems_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
        {
            if (e.PropertyName == DEF_VISIBLE_PROPERTY_NAME && UseAdditionalAnimation)
            {
                if ((bool)e.NewValue)
                {
                    m_animatedItem = sender as XPTaskBarItem;

                    if (m_animatedItem != null)
                    {
                        this.m_enItemAction = ItemAction.Show;
                        this.animationHelper.StartAnimation(this.AnimationPositionsCount, true, this.AnimationDelay);
                    }
                }
                else
                {
                    m_animatedItem = sender as XPTaskBarItem;

                    if (m_animatedItem != null)
                    {
                        this.m_enItemAction = ItemAction.Hide;
                        this.animationHelper.StartAnimation(this.AnimationPositionsCount, false, this.AnimationDelay);
                    }
                }
            }

            if (e.PropertyChangeEffect == PropertyChangeEffect.NeedRepaint)
                this.Invalidate();
        }

        private void TaskMenuItems_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add)
            {
                ((XPTaskBarItem)e.Element).Parent = this;

                if (UseAdditionalAnimation && this.IsHandleCreated)
                {
                    m_animatedItem = e.Element as XPTaskBarItem;

                    if (m_animatedItem != null)
                    {
                        this.m_enItemAction = ItemAction.Show;
                        this.animationHelper.StartAnimation(this.AnimationPositionsCount, true, this.AnimationDelay);
                    }
                }
            }
            else if (e.Action == CollectionChangeAction.Remove)
            {
                ((XPTaskBarItem)e.Element).Parent = null;
                if (this.FocusedItem == e.Element)
                    this.FocusedItem = null;
                if (UseAdditionalAnimation)
                {
                    m_animatedItem = e.Element as XPTaskBarItem;

                    if (m_animatedItem != null)
                    {
                        this.m_enItemAction = ItemAction.Hide;
                        this.animationHelper.StartAnimation(this.AnimationPositionsCount, false, this.AnimationDelay);
                    }
                }
            }

            //			System.Diagnostics.Trace.WriteLine(this.Name.ToString() + " TaskMenuItems_CollectionChanged");
            OnLayoutNeed(new EventArgs());
            //			this.SetNeedLayout(true);
        }
        #region CHILD_PANEL_LOGIC
        /// <summary>
        /// Called when a child <see cref="Panel"/> gets added to this control.
        /// </summary>
        /// <param name="childPanel">A <see cref="Panel"/> instance.</param>
        protected virtual void AttachChildPanel(Panel childPanel)
        {
            if (childPanel == null)
                return;

            childPanel.Dock = DockStyle.None;
            childPanel.ParentChanged += new EventHandler(this.ChildPanel_ParentChanged);
            childPanel.SizeChanged += new EventHandler(this.ChildPanel_SizeChanged);
            childPanel.Disposed += new EventHandler(this.ChildPanel_Disposing);
            childPanel.LocationChanged += new EventHandler(this.ChildPanel_LocationChanged);
            childPanel.DockChanged += new EventHandler(this.ChildPanel_DockChanged);
            childPanel.ControlAdded += new ControlEventHandler(childPanel_ControlAdded);
            childPanel.MouseHover += new EventHandler(childPanel_MouseHover);
            childPanel.MouseMove += new MouseEventHandler(childPanel_MouseMove);
            childPanel.MouseEnter += new EventHandler(childPanel_MouseEnter);
            OnLayoutNeed(new EventArgs());
            Invalidate();
        }

        private void childPanel_MouseEnter(object sender, EventArgs e)
        {
            if (this.FindForm() == null && !(this.Focused || this.ContainsFocus)) //Hosted in Panel in MFC
                this.Focus();
        }

        private void childPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.FindForm() == null && !(this.Focused || this.ContainsFocus)) //Hosted in Panel in MFC
                this.Focus();
        }

        private void childPanel_MouseHover(object sender, EventArgs e)
        {
            if (this.FindForm() == null && !(this.Focused || this.ContainsFocus)) //Hosted in Panel in MFC
                this.Focus();
        }

        private void childPanel_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.Click += new EventHandler(Control_Click);
            e.Control.MouseDown += new MouseEventHandler(Control_MouseDown);
            e.Control.MouseHover += new EventHandler(Control_MouseHover);
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.FindForm() == null && !(this.Focused || this.ContainsFocus)) //Hosted in Panel in MFC
                this.Focus();
        }

        private void Control_MouseHover(object sender, EventArgs e)
        {
            if (this.FindForm() == null && !(this.Focused || this.ContainsFocus)) //Hosted in Panel in MFC
                this.Focus();
        }

        private void Control_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl is Button && ctrl.Parent is Panel && this.FindForm() == null) //Hosted in Panel in MFC
                this.Focus();
        }
        /// <summary>
        /// Called when the child panel gets removed from this control.
        /// </summary>
        /// <param name="childPanel">The <see cref="Panel"/> instance.</param>
        protected virtual void DetachChildPanel(Panel childPanel)
        {
            if (childPanel == null)
                return;

            childPanel.ParentChanged -= new EventHandler(this.ChildPanel_ParentChanged);
            childPanel.SizeChanged -= new EventHandler(this.ChildPanel_SizeChanged);
            childPanel.Disposed -= new EventHandler(this.ChildPanel_Disposing);
            childPanel.LocationChanged -= new EventHandler(this.ChildPanel_LocationChanged);
            childPanel.DockChanged -= new EventHandler(this.ChildPanel_DockChanged);
        }
        private void ChildPanel_ParentChanged(object sender, EventArgs e)
        {
            this.DetachChildPanel(sender as Panel);
        }
        private void ChildPanel_SizeChanged(object sender, EventArgs e)
        {
            //			System.Diagnostics.Trace.WriteLine(this.Name.ToString() + " ChildPanel_SizeChanged");
            OnLayoutNeed(new EventArgs());
            //			this.SetNeedLayout(true);
        }
        private void ChildPanel_LocationChanged(object sender, EventArgs e)
        {
            //			System.Diagnostics.Trace.WriteLine(this.Name.ToString() + " ChildPanel_LocationChanged");
            OnLayoutNeed(new EventArgs());
            //			this.SetNeedLayout(true);
        }
        private void ChildPanel_Disposing(object sender, EventArgs e)
        {
            this.DetachChildPanel(sender as Panel);
        }
        private void ChildPanel_DockChanged(object sender, EventArgs e)
        {
            if (this.ChildPanel.Dock == DockStyle.None)
                return;

            if (this.DesignMode)
                MessageBox.Show("The DockStyle cannot be set for a Panel in the XPTaskBarBox. It will now be reset to DockStyle.None.", "XPTaskBarBox warning:");

            this.ChildPanel.Dock = DockStyle.None;
        }
        /// <summary>
        /// Called to update the child panel's bounds, based on the current settings and layout.
        /// </summary>
        /// <param name="top">The top location for the Panel.</param>
        protected virtual void UpdateChildPanelBounds(int top)
        {
            // 2 pixel padding to the left, right, top and bottom
            Panel childPanel = this.ChildPanel;
            if (childPanel != null)
            {
                childPanel.SuspendLayout();
                this.SuspendLayout();
                childPanel.Location = new Point(2, top + 2);
                childPanel.Width = this.Width - 4;

                if (this.Parent != null && this.Parent.AutoSize)
                {
                    childPanel.AutoSize = this.Parent.AutoSize;
                    int height = childPanel.GetPreferredSize(Size.Empty).Height;

                    if (this.Parent.AutoSizeMode == AutoSizeMode.GrowOnly)
                    {
                        if (height < childPanel.Height)
                            height = childPanel.Height;
                    }

                    if (this.preferredChildPanelHeight != height && height != 0)
                    {
                        this.preferredChildPanelHeight = height;
                        childPanel.Height = height;
                    }
                }
                else
                {
                    childPanel.Height = this.preferredChildPanelHeight;
                }

                this.ResumeLayout(false);
                childPanel.ResumeLayout(true);
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnControlRemoved"/>.
        /// </summary>
        /// <param name="e">The ControlEventArgs that contains the event data.</param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (e.Control is Panel)
                this.DetachChildPanel(e.Control as Panel);

            base.OnControlRemoved(e);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnControlAdded"/>.
        /// </summary>
        /// <param name="e">The ControlEventArgs that contains the event data.</param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (!(e.Control is Panel))
            {
                if (this.DesignMode)
                    MessageBox.Show("Only Panels can be parented by the XPTaskBarBox.");

                e.Control.Parent = null;
            }
            else if (this.Controls.Count > 1)
            {
                if (this.DesignMode)
                    MessageBox.Show("More than one Panel Control cannot be parented by the XPTaskBarBox.");

                e.Control.Parent = null;
            }
            else
            {
                this.AttachChildPanel(e.Control as Panel);
                base.OnControlAdded(e);
            }
        }
        #endregion CHILD_PANEL_LOGIC
        #region PROPERTIES
        /// <summary>
        /// Gets or sets a value indicating whether header text should be clipped.
        /// </summary>
        /// <value><c>true</c> if header text should be clipped; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether text is clipped.")]
        public bool ClipHeaderText
        {
            get
            {
                return m_bWrapHeaderText;
            }
            set
            {
                m_bWrapHeaderText = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        /// <returns>true if the control has focus; otherwise, false.</returns>
        [Browsable(false)]
        [DefaultValue(false)]
        public new bool Focused
        {
            get
            {
                return this.ContainsFocus;
            }
            set
            {
                if (value != this.ContainsFocus)
                {
                    if (value)
                    {
                        this.Focus();
                    }

                    Invalidate();
                }
            }
        }

        /// <override/>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        /// <summary>
        /// Returns the list of <see cref="XPTaskBarItem"/>s.
        /// </summary>
        /// <value>
        /// A <see cref="XPTaskBarItemsCollection"/> object.
        /// </value>
        /// <remarks>
        /// You can add, remove, insert <see cref="XPTaskBarItem"/>s into this collection.
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Specifies the XPTaskBarItems collection."),
        SRCategory(SR.CategoryData),
        MergableProperty(false),
        Localizable(true)
        ]
        public XPTaskBarItemsCollection Items
        {
            get { return items; }
        }

        /// <summary>
        /// Indicates whether to show or hide the expand-collapse button in the header.
        /// </summary>
        [
        Category("Appearance"),
        DefaultValue(true),
        Description("Specifies whether to show or hide the expand-collapse button in the header.")
        ]
        public bool ShowCollapseButton
        {
            get { return this.showCollapseButton; }
            set
            {
                if (this.showCollapseButton != value)
                {
                    this.showCollapseButton = value;
                    Invalidate();
                    //					this.SetNeedLayout(true);
                }
            }
        }

        /// <summary>
        /// Gets / sets the header text alignment.
        /// </summary>
        [
            Category("Appearance"),
            Description("Gets or sets header text alignment."),
            DefaultValue(StringAlignment.Near),
        ]
        public StringAlignment HeaderTextAlign
        {
            get
            {
                return m_HeaderTextAlign;
            }
            set
            {
                if (m_HeaderTextAlign != value)
                {
                    m_HeaderTextAlign = value;

                    this.Invalidate(GetHeaderRect());
                }
            }
        }

        /// <summary>
        /// Returns the current child <see cref="Panel"/>, if any.
        /// </summary>
        protected virtual Panel ChildPanel
        {
            get
            {
                if (this.Controls.Count > 0)
                    return this.Controls[0] as Panel;
                return null;
            }
        }
        /// <summary>
        /// Gets / sets the preferred height required to draw the child panel.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will be the height set on the child panel.
        /// The Panel's width will be automatically set based on the available width for the <see cref="XPTaskBarBox"/>.
        /// </para>
        /// </remarks>
        [
        Category("Appearance"),
        DefaultValue(30),
        Description("Specifies the preferred height required to draw the child Panel.")
        ]
        public int PreferredChildPanelHeight
        {
            get { return this.preferredChildPanelHeight; }
            set
            {
                if (this.preferredChildPanelHeight != value)
                {
                    this.preferredChildPanelHeight = value;
                    //					OnLayoutNeed( new EventArgs() );
                    Invalidate();
                    //					this.SetNeedLayout(true);
                }
            }
        }

        /// <summary>
        /// Raises the ProvideHeaderBackgroundBrush event.
        /// </summary>
        /// <param name="args">A ProvideBrushEventArgs instance 
        /// containing data regarding this event.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. <para>The OnProvideHeaderBackGroundBrush method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnProvideHeaderBackGroundBrush 
        /// in a derived class, be sure to call the base class's 
        /// OnProvideHeaderBackGroundBrush method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        /// <example>
        /// Take a look at the <see cref="XPTaskBar"/> class reference for an example of this event handler
        /// that uses a gradient brush to draw the header's background.
        /// </example>
        protected virtual void OnProvideHeaderBackGroundBrush(ProvideBrushEventArgs args)
        {
            if (this.ProvideHeaderBackgroundBrush != null)
            {
                this.ProvideHeaderBackgroundBrush(this, args);
            }
        }


        /// <summary>
        /// Occurs when the header portion of the <see cref="XPTaskBarBox"/> gets drawn.
        /// </summary>
        /// <remarks>
        /// This event allows you to provide a Brush with which the
        /// background of the header will get drawn. This event will not be fired when
        /// XP Themes is set to be used to render the header.
        /// </remarks>
        /// <example>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\CS\Form1.cs" name="XPTaskBar brush provider" lang="C#"><code lang="C#">
        ///		private void xpTaskBarBox_ProvideHeaderBackgroundBrush(object sender, Syncfusion.Windows.Forms.Tools.ProvideBrushEventArgs args)
        ///		{
        ///			// Blend settings
        ///			float[] relativeIntensities = {0.0f, 0.0f, 1.0f};
        ///			float[] relativePositions   = {0.0F, 0.5f, 1.0F};
        ///			Blend blend = new Blend();
        ///			blend.Factors = relativeIntensities;
        ///			blend.Positions = relativePositions;
        ///
        ///			XPTaskBarBox box = sender as XPTaskBarBox;
        ///
        ///			// Header back brush
        ///			LinearGradientBrush lgBrush = new LinearGradientBrush(args.Bounds,
        ///				Color.White, box.HeaderBackColor, 0, true);
        ///			lgBrush.Blend = blend;
        ///			args.Brush = lgBrush;
        ///		}
        ///
        ///		private void xpTaskBarBox_ProvideItemsBackgroundBrush(object sender, Syncfusion.Windows.Forms.Tools.ProvideBrushEventArgs args)
        ///		{
        ///			// Blend settings
        ///			float[] relativeIntensities = {0.0f, 0.0f, 1.0f};
        ///			float[] relativePositions   = {0.0F, 0.5f, 1.0F};
        ///			Blend blend = new Blend();
        ///			blend.Factors = relativeIntensities;
        ///			blend.Positions = relativePositions;
        ///
        ///			// Items back brush
        ///			LinearGradientBrush lgBrush = new LinearGradientBrush(args.Bounds,
        ///				Color.WhiteSmoke, Color.Silver, 91, false);
        ///			lgBrush.Blend = blend;
        ///			args.Brush = lgBrush;
        ///		}</code></coderef>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\VB\Form1.vb" name="XPTaskBar brush provider" lang="VB"><code lang="VB">
        ///        Private Sub xpTaskBarBox_ProvideHeaderBackgroundBrush(ByVal sender As Object, ByVal args As ProvideBrushEventArgs)
        ///
        ///            ' Blend settings
        ///            Dim relativeIntensities() As Single
        ///            relativeIntensities = New Single() {0.0!, 0.0!, 1.0!}
        ///            Dim relativePositions() As Single
        ///            relativePositions = New Single() {0.0!, 0.5!, 1.0!}
        ///            Dim blend As Blend
        ///            blend = New Blend()
        ///            blend.Factors = relativeIntensities
        ///            blend.Positions = relativePositions
        ///            Dim box As XPTaskBarBox
        ///            box = CType(sender, XPTaskBarBox)
        ///            ' Header back brush
        ///            Dim lgBrush As LinearGradientBrush
        ///            lgBrush = New LinearGradientBrush(args.Bounds, Color.White, box.HeaderBackColor, 0, True)
        ///            lgBrush.Blend = blend
        ///            args.Brush = lgBrush
        ///
        ///        End Sub
        ///        Private Sub xpTaskBarBox_ProvideItemsBackgroundBrush(ByVal sender As Object, ByVal args As ProvideBrushEventArgs)
        ///
        ///            ' Blend settings
        ///            Dim relativeIntensities() As Single
        ///            relativeIntensities = New Single() {0.0!, 0.0!, 1.0!}
        ///            Dim relativePositions() As Single
        ///            relativePositions = New Single() {0.0!, 0.5!, 1.0!}
        ///            Dim blend As Blend
        ///            blend = New Blend()
        ///            blend.Factors = relativeIntensities
        ///            blend.Positions = relativePositions
        ///            ' Items back brush
        ///            Dim lgBrush As LinearGradientBrush
        ///            lgBrush = New LinearGradientBrush(args.Bounds, Color.WhiteSmoke, Color.Silver, 91, False)
        ///            lgBrush.Blend = blend
        ///            args.Brush = lgBrush
        ///
        ///        End Sub</code></coderef>
        /// </example>
        [SRCategory(SR.CategoryAppearance), Description("Occurs when the header portion of XPTaskBarBox gets drawn.")]
        public event ProvideBrushEventHandler ProvideHeaderBackgroundBrush;

        /// <summary>
        /// Raises the ProvideItemsBackgroundBrush event.
        /// </summary>
        /// <param name="args">A ProvideItemsBackgroundBrush instance
        /// that contains data related to this event.</param>
        /// <remarks>Raising an event invokes the event handler
        /// through a delegate. For more information, see Raising
        /// an Event. <para>The OnProvideItemsBackGroundBrush method also
        /// allows derived classes to handle the event without
        /// attaching a delegate. This is the preferred technique
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnProvideItemsBackGroundBrush
        /// in a derived class, be sure to call the base class's
        /// OnProvideItemsBackGroundBrush method so that registered
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnProvideItemsBackGroundBrush(ProvideBrushEventArgs args)
        {
            if (this.ProvideItemsBackgroundBrush != null)
            {
                this.ProvideItemsBackgroundBrush(this, args);
            }
        }

        /// <summary>
        /// Occurs after the box has been collapsed or expanded.
        /// </summary>
        [SRCategory(SR.CategoryAction), Description("Occurs after the box has been collapsed or expanded.")]
        public event EventHandler CollapsedStateChanged;

        /// <summary>
        /// Occurs when the box has being collapsed or expanded.
        /// </summary>
        [SRCategory(SR.CategoryAction), Description("Occurs when the box has being collapsed or expanded.")]
        public event CancelEventHandler CollapsedStateChanging;

        /// <summary>
        /// Raises the CollapsedStateChanged event.
        /// </summary>
        /// <param name="args">A EventArgs instance
        /// that contains data related to this event.</param>
        /// <remarks>Raising an event invokes the event handler
        /// through a delegate. For more information, see Raising
        /// an Event. <para>The OnCollapsedStateChanged method also
        /// allows derived classes to handle the event without
        /// attaching a delegate. This is the preferred technique
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnCollapsedStateChanged
        /// in a derived class, be sure to call the base class's
        /// OnCollapsedStateChanged method so that registered
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnCollapsedStateChanged(EventArgs args)
        {
            if (this.Collapsed)
            {
                this.FocusedItem = null;
                if (this.ChildPanel != null)
                    this.ChildPanel.Visible = false;
            }
            else
            {
                Panel pnl = this.Parent as Panel;
                if (pnl.AutoScroll)
                    pnl.ScrollControlIntoView(this);
            }
            if (this.CollapsedStateChanged != null)
            {
                this.CollapsedStateChanged(this, args);
            }
        }
        /// <summary>
        /// Raises the CollapsedStateChanging event
        /// </summary>
        /// <param name="args">
        /// A EventArgs instance
        /// that contains data related to this event.</param>
        protected virtual void OnCollapsedStateChanging(CancelEventArgs args)
        {
            if (this.CollapsedStateChanging != null)
            {
                this.CollapsedStateChanging(this, args);
            }
        }

        /// <summary>
        /// Occurs when the content portion of the XPTaskBarBox gets drawn.
        /// </summary>
        /// <remarks>
        /// This event allows you to provide a Brush with which the
        /// background of the content portion will be drawn. This event will not be fired 
        /// when XP Themes is set to be used for drawing the <see cref="XPTaskBarBox"/>.
        /// </remarks>
        /// <example>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\CS\Form1.cs" name="XPTaskBar brush provider" lang="C#"><code lang="C#">
        ///		private void xpTaskBarBox_ProvideHeaderBackgroundBrush(object sender, Syncfusion.Windows.Forms.Tools.ProvideBrushEventArgs args)
        ///		{
        ///			// Blend settings
        ///			float[] relativeIntensities = {0.0f, 0.0f, 1.0f};
        ///			float[] relativePositions   = {0.0F, 0.5f, 1.0F};
        ///			Blend blend = new Blend();
        ///			blend.Factors = relativeIntensities;
        ///			blend.Positions = relativePositions;
        ///
        ///			XPTaskBarBox box = sender as XPTaskBarBox;
        ///
        ///			// Header back brush
        ///			LinearGradientBrush lgBrush = new LinearGradientBrush(args.Bounds,
        ///				Color.White, box.HeaderBackColor, 0, true);
        ///			lgBrush.Blend = blend;
        ///			args.Brush = lgBrush;
        ///		}
        ///
        ///		private void xpTaskBarBox_ProvideItemsBackgroundBrush(object sender, Syncfusion.Windows.Forms.Tools.ProvideBrushEventArgs args)
        ///		{
        ///			// Blend settings
        ///			float[] relativeIntensities = {0.0f, 0.0f, 1.0f};
        ///			float[] relativePositions   = {0.0F, 0.5f, 1.0F};
        ///			Blend blend = new Blend();
        ///			blend.Factors = relativeIntensities;
        ///			blend.Positions = relativePositions;
        ///
        ///			// Items back brush
        ///			LinearGradientBrush lgBrush = new LinearGradientBrush(args.Bounds,
        ///				Color.WhiteSmoke, Color.Silver, 91, false);
        ///			lgBrush.Blend = blend;
        ///			args.Brush = lgBrush;
        ///		}</code></coderef>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\VB\Form1.vb" name="XPTaskBar brush provider" lang="VB"><code lang="VB">
        ///        Private Sub xpTaskBarBox_ProvideHeaderBackgroundBrush(ByVal sender As Object, ByVal args As ProvideBrushEventArgs)
        ///
        ///            ' Blend settings
        ///            Dim relativeIntensities() As Single
        ///            relativeIntensities = New Single() {0.0!, 0.0!, 1.0!}
        ///            Dim relativePositions() As Single
        ///            relativePositions = New Single() {0.0!, 0.5!, 1.0!}
        ///            Dim blend As Blend
        ///            blend = New Blend()
        ///            blend.Factors = relativeIntensities
        ///            blend.Positions = relativePositions
        ///            Dim box As XPTaskBarBox
        ///            box = CType(sender, XPTaskBarBox)
        ///            ' Header back brush
        ///            Dim lgBrush As LinearGradientBrush
        ///            lgBrush = New LinearGradientBrush(args.Bounds, Color.White, box.HeaderBackColor, 0, True)
        ///            lgBrush.Blend = blend
        ///            args.Brush = lgBrush
        ///
        ///        End Sub
        ///        Private Sub xpTaskBarBox_ProvideItemsBackgroundBrush(ByVal sender As Object, ByVal args As ProvideBrushEventArgs)
        ///
        ///            ' Blend settings
        ///            Dim relativeIntensities() As Single
        ///            relativeIntensities = New Single() {0.0!, 0.0!, 1.0!}
        ///            Dim relativePositions() As Single
        ///            relativePositions = New Single() {0.0!, 0.5!, 1.0!}
        ///            Dim blend As Blend
        ///            blend = New Blend()
        ///            blend.Factors = relativeIntensities
        ///            blend.Positions = relativePositions
        ///            ' Items back brush
        ///            Dim lgBrush As LinearGradientBrush
        ///            lgBrush = New LinearGradientBrush(args.Bounds, Color.WhiteSmoke, Color.Silver, 91, False)
        ///            lgBrush.Blend = blend
        ///            args.Brush = lgBrush
        ///
        ///        End Sub</code></coderef>
        /// </example>
        [SRCategory(SR.CategoryAppearance), Description("Occurs when the content portion of the XPTaskBarBox gets drawn")]
        public event ProvideBrushEventHandler ProvideItemsBackgroundBrush;

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.BackgroundImage"/>.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        Browsable(false)
        ]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; return; }
        }

        /// <summary>
        /// Gets / sets the background color with which the header will be drawn.
        /// </summary>
        /// <value>A Color value representing the header backcolor.</value>
        /// <remarks>
        /// <para>This setting will be ignored when XP Themes is set to be used for drawing
        /// the header.</para>
        /// </remarks>
        [SRCategory(SR.CategoryAppearance),
        Description("Gets or Set the background color with which the header will be drawn.")]
        public Color HeaderBackColor
        {
            get
            {
                Color color = this.BackColor;
                if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Office2007)
                {
                    color = this.Parent.Office2007ColorTable.XPTaskBarBoxBackColor;
                }
                else if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Office2010)
                {
                    color = this.Parent.Office2010ColorTable.XPTaskBarBoxBackColor;
                }
                else if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Metro)
                {
                    headerBackColor = ColorTranslator.FromHtml("#16A5DC");
                    color = headerBackColor;
                }
                else if (this.headerBackColor != Color.Empty)
                {
                    color = headerBackColor;
                }

                return color;
            }
            set
            {
                headerBackColor = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Resets the header backcolor to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual void ResetHeaderBackColor()
        {
            if (!this.ThemesEnabled && this.Parent != null )
            {
                if (this.Parent.Style == XPTaskBarStyle.Office2007)
                    this.HeaderBackColor = this.Parent.Office2007ColorTable.XPTaskBarBoxBackColor;
                else if (this.Parent.Style == XPTaskBarStyle.Office2010)
                    this.HeaderBackColor = this.Parent.Office2010ColorTable.XPTaskBarBoxBackColor;
                else if (this.Parent.Style == XPTaskBarStyle.Metro)
                    this.HeaderBackColor = this.Parent.MetroColor ;
            }
            else
            {
                this.HeaderBackColor = Color.Empty;
            }
        }
        /// <summary>
        /// Indicates whether the Header BackColor has a value different from its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeHeaderBackColor()
        {
            bool bShould = false;

            if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Office2007)
            {
                bShould = this.HeaderBackColor != this.Parent.Office2007ColorTable.XPTaskBarBoxBackColor;
            }
            else
            {
                bShould = this.HeaderBackColor != Color.Empty;
            }

            return bShould;
        }

        /// <summary>
        /// Gets or sets the fore color of the header.
        /// </summary>
        /// <remarks>
        /// <para>This setting will be ignored when XP Themes is set to be used for drawing
        /// the header.</para>
        /// </remarks>
        [
            SRCategory(SR.CategoryAppearance),
            Description("Gets or Sets the forecolor of the header text.")
        ]
        public Color HeaderForeColor
        {
            get
            {
                Color color = headerForeColor;
                if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Metro)
                {
                    headerForeColor = Color.White;
                    color = headerForeColor;
                }
                return color;
            }
            set
            {
                if (headerForeColor != value)
                {
                    headerForeColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Resets the header forecolor to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void ResetHeaderForeColor()
        {
            this.HeaderForeColor = SystemColors.ControlText;
        }

        /// <summary>
        /// Indicates whether the Header ForeColor has a value different from its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeHeaderForeColor()
        {
            return (this.headerForeColor != SystemColors.ControlText);
        }

        /// <summary>
        /// Gets / sets the text font with which the header text will be drawn.
        /// </summary>
        /// <value>A Font value. Default is the Font property with Bold setting.</value>
        /// <remarks>This will be ignored when <see cref="ThemesEnabled"/> is set to true.</remarks>
        [SRCategory(SR.CategoryAppearance),
        Description("Gets or Set the text font with which the header text will be drawn.")]
        public Font HeaderFont
        {
            get
            {
                if (this.headerFont != null)
                    return this.headerFont;
                else
                    return FontUtil.CreateFont(this.Font, FontStyle.Bold);
            }
            set
            {
                if (this.headerFont != value)
                {
                    this.headerFont = value;
                    Invalidate();
                    //					this.SetNeedLayout(true);
                }
            }
        }
        public bool ShouldSerializeHeaderFont()
        {
            return this.headerFont != null;
        }
        public void ResetHeaderFont()
        {
            this.HeaderFont = null;
        }
        /// <summary>
        /// Indicates whether the control should take focus when the user click the header or an item.
        /// </summary>
        /// <value>True to take focus; false otherwise. Default is true.</value>
        [DefaultValue(true),
        Description("Specifies if control should take focus on header or item click."),
        Category("Behavior"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        [Obsolete]
        public virtual bool FocusOnClick
        {
            get { return this.DrawFocusRect; }
            set
            {
                this.DrawFocusRect = value;
            }
        }

        /// <summary>
        /// Indicates whether the control should take focus when the user click the header or an item.
        /// </summary>
        /// <value>True to take focus; false otherwise. Default is true.</value>
        [DefaultValue(true),
        Description("Specifies if control should take focus on header or item click."),
        Category("Behavior")
        ]
        public virtual bool DrawFocusRect
        {
            get { return this.m_bDrawFocusRect; }
            set
            {
                this.m_bDrawFocusRect = value;
            }
        }

        /// <summary>
        /// Gets / sets the color with which the Items portion of the XPTaskBarBox will be drawn.
        /// </summary>
        /// <value>The Color value with which the Items portion will be drawn.</value>
        /// <para>This setting will be ignored when XP Themes is set to be used for drawing
        /// the content portion.</para>
        [SRCategory(SR.CategoryAppearance),
        Description("Gets or Sets the Color with which the Items portion of the XPTaskBarBox will be drawn.")]
        public Color ItemBackColor
        {
            get
            {
                Color color = this.BackColor;
                if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Office2007)
                {
                    color = Color.White;
                }
                if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Metro)
                {
                    color = ControlPaint.Light(this.Parent .MetroColor );
                }
                else if (this.itemBackColor != Color.Empty)
                {
                    color = itemBackColor;
                }

                return color;
            }
            set
            {
                itemBackColor = value;
                this.Invalidate();
            }
        }
        /// <summary>
        /// Resets the item backcolor to its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected internal virtual void ResetItemBackColor()
        {
            if (!this.ThemesEnabled && this.Parent != null && (this.Parent.Style == XPTaskBarStyle.Office2007||this.Parent.Style == XPTaskBarStyle.Office2010))
            {
                this.ItemBackColor = Color.White;
            }
            if (!this.ThemesEnabled && this.Parent != null && this.Parent.Style == XPTaskBarStyle.Metro )
            {
                this.ItemBackColor = Color.White ;
            }
            else
            {
                if (m_bPropagateBackColorToItems)
                {
                    this.ItemBackColor = this.BackColor;
                }
                else
                {
                    this.ItemBackColor = Color.Empty;
                }
            }
        }
        /// <summary>
        /// Indicates whether the Item BackColor has a value different from its default value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual bool ShouldSerializeItemBackColor()
        {
            bool bShould = false;

            if (!this.ThemesEnabled && this.Parent != null && (this.Parent.Style == XPTaskBarStyle.Office2007||this.Parent.Style == XPTaskBarStyle.Office2010))
            {
                bShould = (this.ItemBackColor != Color.White);
            }
            else
            {
                if (m_bPropagateBackColorToItems)
                {
                    bShould = (this.ItemBackColor != this.BackColor);
                }
                else
                {
                    bShould = (this.ItemBackColor != Color.Empty);
                }
            }

            return bShould;
        }
        /// <summary>
        /// Gets / sets the ImageListAdv which contains the images with which the <see cref="Items"/> and the header will be drawn.
        /// </summary>
        /// <value>An ImageListAdv instance containing the images.</value>
        [
        DefaultValue(null),
        SRCategory(SR.CategoryAppearance),
        Description("Specifies the ImageListAdv containing the images in this XPTaskBarBox")
        ]
        public ImageListAdv ImageListAdv
        {
            get
            {
                return imageListAdv;
            }
            set
            {

                if (value != null)
                    this.ImageList = value.ToImageList();
                imageListAdv = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets the HeaderImageListAdv which contains the images with which the <see cref="Items"/> and the header will be drawn.
        /// </summary>
        /// <value>An HeaderImageListAdv instance containing the images.</value>
        [
        DefaultValue(null),
        SRCategory(SR.CategoryAppearance),
        Description("Specifies the HeaderImageListAdv containing the images in this XPTaskBarBox")
        ]
        public ImageListAdv HeaderImageListAdv
        {
            get
            {
                return headerImageListAdv;
            }
            set
            {

                if (value != null)
                    this.HeaderImageList = value.ToImageList();
                headerImageListAdv = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets / sets the ImageList which contains the images with which the <see cref="Items"/> and the header will be drawn.
        /// </summary>
        /// <value>An ImageList instance containing the images.</value>
        [
        DefaultValue(null),
        SRCategory(SR.CategoryAppearance),
        Description("Specifies the ImageList containing the images in this XPTaskBarBox")
        ]
        public ImageList ImageList
        {
            get { return imageList; }
            set
            {
                if (imageList != value)
                {
                    ImageList oldImageList = imageList;
                    imageList = value;
                    Invalidate();
                    //					this.SetNeedLayout(true);

                    if (oldImageList != null)
                        oldImageList.RecreateHandle -= new EventHandler(this.ImageList_Recreated);

                    if (imageList != null)
                        imageList.RecreateHandle += new EventHandler(this.ImageList_Recreated);

                    imageList = value;
                }
            }
        }

        /// <summary>
        /// Gets / sets the ImageList that will be used to draw the header image.
        /// </summary>
        /// <value>An <see cref="System.Windows.Forms.ImageList"/> instance. Default is the
        /// value from the parent <see cref="XPTaskBar.HeaderImageList"/> property.</value>
        /// <remarks>
        /// This ImageList will automatically be picked up from the parent <see cref="XPTaskBar"/> instance.
        /// You can override this behavior by setting a different ImageList in this property.
        /// </remarks>
        [
        SRCategory(SR.CategoryAppearance),
        Description("Specifies the ImageList that will be used to draw the header image.")
        ]
        public ImageList HeaderImageList
        {
            get
            {
                if (this.headerImageList != null)
                    return this.headerImageList;
                else if (this.Parent != null)
                    return this.Parent.HeaderImageList;
                else
                    return null;
            }
            set
            {
                if (this.headerImageList != value)
                {
                    if (this.headerImageList != null)
                        this.headerImageList.RecreateHandle -= new EventHandler(ImageList_Recreated);

                    this.headerImageList = value;

                    if (this.headerImageList != null)
                        this.headerImageList.RecreateHandle += new EventHandler(ImageList_Recreated);

                    Invalidate();
                    //					this.SetNeedLayout(true);
                }
            }
        }

        [Documentation.DocumentationExclude()]
        public bool ShouldSerializeHeaderImageList()
        {
            if (this.headerImageList != null)
                return true;
            else
                return false;
        }

        [Documentation.DocumentationExclude()]
        public void ResetHeaderImageList()
        {
            this.HeaderImageList = null;
        }

        /// <summary>
        /// Gets / sets the index into the <see cref="HeaderImageList"/>.
        /// </summary>
        /// <remarks>
        /// The zero-based index to the image in the XPTaskBarBox.HeaderImageList 
        /// The default is -1, which signifies no image.
        /// </remarks>
        [
            SRCategory(SR.CategoryAppearance),
            TypeConverter(typeof(ImageIndexConverter)),
            Editor(typeof(ImageIndexEditor), typeof(UITypeEditor)),
            Description("Specifies the index into the HeaderImageList")
        ]
        public int HeaderImageIndex
        {
            get { return this.headerImageIndex; }
            set
            {
                if (this.headerImageIndex != value)
                {
                    this.headerImageIndex = value;
                    Invalidate();
                    //					this.SetNeedLayout(true);
                }
            }
        }

        private void ImageList_Recreated(object sender, EventArgs e)
        {
            Invalidate();
            //			this.SetNeedLayout(true);
        }

        /// <summary>
        /// Gets / sets the preferred height for this control with the current settings.
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        protected virtual int PreferredHeight
        {
            get { return preferredHeight; }
            set
            {
                if (preferredHeight != value)
                {
                    preferredHeight = value;
                    OnLayoutNeed(new EventArgs());
                    //					if(this.Parent != null)
                    //						this.Parent.PerformLayout();
                }
            }
        }
        /// <summary>
        /// Returns the preferred height for this box.
        /// </summary>
        /// <returns>The height in pixels.</returns>
        /// <remarks>Internally used by the XPTaskBar while laying out these controls.</remarks>
        public int GetPreferredHeight()
        {
            return this.preferredHeight;
        }

        /// <summary>
        /// Returns the size of the expand/collapse button.
        /// </summary>
        /// <returns>The Size of the button.</returns>
        protected Size GetHeaderButtonSize()
        {
            if (!this.ShowCollapseButton)
                return new Size(0, 0);

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                if ((!XPThemes.IsOliveGreenThemeOn && !XPThemes.IsSilverThemeOn)
                    || !XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes)
                    return this.ThemedDrawing.GetHeaderButtonSize();
            }

            return new Size(this.EXP_BUTTON_SIZE, this.EXP_BUTTON_SIZE);
        }

        /// <summary>
        /// Gets / sets the <see cref="XPTaskBar"/> parent.
        /// </summary>
        [
        SRCategory(SR.CategoryBehavior),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description(@"The parent of this control."),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public new XPTaskBar Parent
        {
            get { return base.Parent as XPTaskBar; }
            set { base.Parent = value; }
        }

        /// <summary>
        /// Reflects the corresponding property in the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBar"/>
        /// parent.
        /// </summary>
        [Category("Appearance"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Reflects the corresponding property in the XPTaskBar parent.")]
        public bool ThemesEnabled
        {
            get
            {
                bool bThemesEnabled = false;

                if (this.Parent != null)
                {
                    bThemesEnabled = this.Parent.ThemesEnabled;
                }

                return bThemesEnabled && XPThemes.IsAppThemed;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether focus should be set only to the text bounds.
        /// </summary>
        /// <value><c>true</c> if focus to be set to text bounds; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Description("Indicates whether focus should be set only to the text bounds.")]
        public bool FocusTextBounds
        {
            get
            {
                return focusTextBounds;
            }
            set
            {
                if (focusTextBounds != value)
                    focusTextBounds = value;
            }
        }

        /// <summary>
        /// Gets / sets the AnimationDelay for the XPTaskBarBox.
        /// </summary>
        /// <remarks>
        /// Set this property to a different value if the AnimationDelay for this
        /// XPTaskBarBox is to be different. By default this is set to <see cref="XPTaskBarBox.AnimationDelayInMilliSeconds"/>.
        /// </remarks>
        [Description("Specifies the animation delay during expand/collpase."),
        Category("Behavior")]
        public int AnimationDelay
        {
            get
            {
                return this.animationDelay;
            }

            set
            {
                this.animationDelay = value;
            }
        }


        [Documentation.DocumentationExclude()]
        public bool ShouldSerializeAnimationDelay()
        {
            if (this.AnimationDelay != XPTaskBarBox.AnimationDelayInMilliSeconds)
                return true;
            else
                return false;
        }

        [Documentation.DocumentationExclude()]
        public void ResetAnimationDelay()
        {
            this.AnimationDelay = XPTaskBarBox.AnimationDelayInMilliSeconds;
        }

        /// <summary>
        /// Gets / sets the AnimationPositions count for the XPTaskBarBox.
        /// </summary>
        /// <remarks>
        /// Set this property to a different value if the AnimationPositions count for this
        /// XPTaskBarBox is to be different. By default this is set to <see cref="XPTaskBarBox.AnimationPositions"/>.
        /// </remarks>
        [Description("Specifies the number of animation positions during expand/collapse."),
        Category("Behavior")]
        public int AnimationPositionsCount
        {
            get
            {
                return this.animationPositionsCount;
            }

            set
            {
                if (value <= 0)
                    throw new ArgumentException("Value should be greater than zero");
                else
                    this.animationPositionsCount = value;

            }
        }


        [Documentation.DocumentationExclude()]
        public bool ShouldSerializeAnimationPositionsCount()
        {
            if (this.AnimationPositionsCount != XPTaskBarBox.AnimationPositions)
                return true;
            else
                return false;
        }

        [Documentation.DocumentationExclude()]
        public void ResetAnimationPositionsCount()
        {
            this.AnimationPositionsCount = XPTaskBarBox.AnimationPositions;
        }


        /// <summary>
        /// Gets or sets value indicating whether the items added or removed it's drawing with animation.
        /// </summary>
        [
        Category("Behavior"),
        Description("Indicate whether the items added or removed it's drawing with animation."),
        DefaultValue(typeof(bool), "false")
        ]
        public bool UseAdditionalAnimation
        {
            get
            {
                return m_bUseAdditionalAnimation;
            }
            set
            {
                if (value != m_bUseAdditionalAnimation)
                {
                    m_bUseAdditionalAnimation = value;
                }
            }
        }

        /// <summary>
        /// Specifies whether background color is propagated to items when it changes.
        /// </summary>
        [Description("Specifies whether background color is propagated to items when it changes.")]
        [Category("Apperance")]
        [DefaultValue(false)]
        public bool PropagateBackColorToItems
        {
            get
            {
                return m_bPropagateBackColorToItems;
            }
            set
            {
                m_bPropagateBackColorToItems = value;
            }
        }

        #endregion PROPERTIES

        /// <summary>
        /// Overloaded. Expands the content area of this task bar box, if collapsed, with animation.
        /// </summary>
        /// <seealso cref="CollapseContent(bool)"/>
        public void ExpandContent()
        {
            this.ExpandContent(true);
        }
        /// <summary>
        /// Expands the content area of this task bar box, if collapsed.
        /// </summary>
        /// <param name="useAnimation">Indicates whether to use animation while expanding.</param>
        /// <seealso cref="CollapseContent()"/>
        public void ExpandContent(bool useAnimation)
        {
            if (this.button.ExpandedState)
            {
                this.ToggleContentVisibility(useAnimation);
            }
        }

        /// <summary>
        /// Overloaded. Collapses the content area of this task bar box, if expanded.
        /// </summary>
        /// <param name="useAnimation">Indicates whether to use animation while collapsing.</param>
        /// <seealso cref="ExpandContent()"/>
        public void CollapseContent(bool useAnimation)
        {
            if (!this.button.ExpandedState)
            {
                this.ToggleContentVisibility(useAnimation);
            }
        }

        /// <summary>
        /// Collapses the content area of this task bar box, if expanded, with animation.
        /// </summary>
        /// <seealso cref="ExpandContent(bool)"/>
        public void CollapseContent()
        {
            this.CollapseContent(true);
        }

        #region events
        /// <summary>
        /// The event that gets thrown when the user clicks on an item.
        /// </summary>
        /// <remarks>
        /// The XPTaskBarItemClickArgs contains a reference to the XPTaskBarItem that was clicked.
        /// </remarks>
        /// <example>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\CS\Form1.cs" name="XPTaskBar Click Handler" lang="C#"><code lang="C#">
        ///		private void xpTaskBarBox_ItemClick(object sender, Syncfusion.Windows.Forms.Tools.XPTaskBarItemClickArgs e)
        ///		{
        ///			this.aboutDrawingLabel.BorderStyle = BorderStyle.None;
        ///			this.aboutLable.BorderStyle = BorderStyle.None;
        ///			this.aboutItemsLabel.BorderStyle = BorderStyle.None;
        ///
        ///			switch(e.XPTaskBarItem.Tag as string)
        ///			{
        ///				case "about":
        ///					this.aboutLable.BorderStyle = BorderStyle.FixedSingle;
        ///					break;
        ///				case "about items":
        ///					this.aboutItemsLabel.BorderStyle = BorderStyle.FixedSingle;
        ///					break;
        ///				case "about drawing":
        ///					this.aboutDrawingLabel.BorderStyle = BorderStyle.FixedSingle;
        ///					break;
        ///				case "aboutSyncfusion":
        ///					this.ShowAboutDialog();
        ///					break;
        ///			}
        ///		}</code></coderef>
        /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\VB\Form1.vb" name="XPTaskBar Click Handler" lang="VB"><code lang="VB">
        ///        Private Sub xpTaskBarBox_ItemClick(ByVal sender As Object, ByVal e As XPTaskBarItemClickArgs)
        ///
        ///            Me.aboutDrawingLabel.BorderStyle = BorderStyle.None
        ///            Me.aboutLable.BorderStyle = BorderStyle.None
        ///            Me.aboutItemsLabel.BorderStyle = BorderStyle.None
        ///            Select Case CType(e.XPTaskBarItem.Tag, String)
        ///                Case "about"
        ///                    Me.aboutLable.BorderStyle = BorderStyle.FixedSingle
        ///                    'End Section
        ///                Case "about items"
        ///                    Me.aboutItemsLabel.BorderStyle = BorderStyle.FixedSingle
        ///                    'End Section
        ///                Case "about drawing"
        ///                    Me.aboutDrawingLabel.BorderStyle = BorderStyle.FixedSingle
        ///                    'End Section
        ///                Case "aboutSyncfusion"
        ///                    Me.ShowAboutDialog()
        ///                    'End Section
        ///            End Select
        ///
        ///        End Sub</code></coderef>
        /// </example>
        [SRCategory(SR.CategoryAction), Description("Occurs when the user clicks on an item")]
        public event XPTaskBarItemClickHandler ItemClick;

        /// <summary>
        /// This event gets called before the box expands or collapses.
        /// </summary>
        [Description("Occurs before the box expands or collapses.")]
        public event EventHandler BeforeAnimation;

        /// <summary>
        /// This event gets called after the box expands or collapses.
        /// </summary>
        [Description("Occurs after the box expands or collapses.")]
        public event EventHandler AfterAnimation;

        /// <summary>
        /// This event is raised when 
        /// </summary>
        public event EventHandler LayoutNeed;
        #endregion

        /// <summary>
        /// Raises the LayoutNeed event.
        /// </summary>
        /// <param name="e">A EventArgs that contains the event data.</param>
        protected virtual void OnLayoutNeed(EventArgs e)
        {
            if (this.LayoutNeed != null)
                this.LayoutNeed(this, e);
        }

        /// <summary>
        /// Raises the BeforeAnimation event.
        /// </summary>
        /// <param name="e">A EventArgs that contains the event data.</param>
        /// <returns>True if there were listeners; false otherwise.</returns>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnBeforeAnimation method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnBeforeAnimation 
        /// in a derived class, be sure to call the base class's 
        /// OnBeforeAnimation method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnBeforeAnimation(EventArgs e)
        {
            if (this.BeforeAnimation != null)
                this.BeforeAnimation(this, e);
        }

        /// <summary>
        /// Raises the AfterAnimation event.
        /// </summary>
        /// <param name="e">A EventArgs that contains the event data.</param>
        /// <returns>True if there were listeners; false otherwise.</returns>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. For more information, see Raising 
        /// an Event. <para>The OnAfterAnimation method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnAfterAnimation 
        /// in a derived class, be sure to call the base class's 
        /// OnAfterAnimation method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnAfterAnimation(EventArgs e)
        {
            if (this.AfterAnimation != null)
                this.AfterAnimation(this, e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnFontChanged"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            OnLayoutNeed(new EventArgs());
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        protected internal virtual void UpdatePreferredHeight(Graphics g)
        {
            if (!IsHandleCreated)
                return;

            bool ownedG = false;
            if (g == null)
            {
                g = this.CreateGraphics();
                ownedG = true;
            }

            this.headerHeight = DetermineHeaderHeight(g);

            UpdateExpanderButtonPosition();

            // Determines the height of the taskbarbox based on the Taskbar box alignment.
            int itemsRectHeight = 0;

            if (this.Parent.BoxItemsAlignment == XPTaskBar.ItemsAlignment.Vertical)
                itemsRectHeight = DetermineItemsRectHeight(g);
            else
                itemsRectHeight = DetermineItemsRectHeightOnHorizontalAlignment(g);

            //Animation Change
            if (this.animationHelper.AnimationOn)
            {
                if (this.m_enItemAction == ItemAction.None)
                {
                    this.PreferredHeight = this.headerHeight + itemsRectHeight - itemsRectHeight * (this.animationHelper.MaxAnimationPosition - this.animationHelper.AnimationPosition) / this.animationHelper.MaxAnimationPosition;
                }
                else
                {
                    if (!this.Collapsed && m_animatedItem != null)
                    {
                        int heightItem = m_animatedItem.Bounds.Height;

                        float heightAnimation = (float)(this.animationHelper.MaxAnimationPosition -
                            this.animationHelper.AnimationPosition);
                        heightAnimation = heightAnimation / (float)this.animationHelper.MaxAnimationPosition;
                        heightAnimation = heightItem * heightAnimation;

                        int increment = (int)(heightAnimation);

                        switch (m_enItemAction)
                        {
                            case ItemAction.Show:
                                {
                                    this.PreferredHeight = this.headerHeight + itemsRectHeight - increment;
                                    break;
                                }
                            case ItemAction.Hide:
                                {
                                    this.PreferredHeight = this.headerHeight + itemsRectHeight - increment + heightItem;
                                    break;
                                }
                        }
                    }
                    else
                    {
                        this.PreferredHeight = this.headerHeight;
                    }
                }
            }
            else
            {
                int prefHeight = this.headerHeight;// + itemsRectHeight - itemsRectHeight*(this.animationHelper.MaxAnimationPosition-this.animationHelper.AnimationPosition)/this.animationHelper.MaxAnimationPosition;
                if (!this.button.ExpandedState)
                    prefHeight += itemsRectHeight;
                this.PreferredHeight = prefHeight;

                this.m_enItemAction = ItemAction.None;
                this.m_animatedItem = null;
            }

            if (ownedG)
                g.Dispose();
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Updates the expander button position based on the current box dimensions.
        /// </para>
        /// </remarks>
        protected virtual void UpdateExpanderButtonPosition()
        {
            Rectangle headerRect = this.GetHeaderRect();

            Size btnSize = this.GetHeaderButtonSize();

            bool needMirrored = this.GetHeaderNeedMirrored();
            int nLeft = needMirrored ? this.PADX : (this.ClientRectangle.Width - this.PADX - btnSize.Width);

            this.ExpanderButton.Bounds = new Rectangle(nLeft,
                headerRect.Top + (headerRect.Height - btnSize.Height) / 2, btnSize.Width, btnSize.Height);
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <returns>The height for the items area.</returns>
        /// <remarks>
        /// <para>
        /// Called by <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.UpdatePreferredHeight"/>
        /// to determine the height for the items area.
        /// </para>
        /// </remarks>
        protected virtual int DetermineItemsRectHeight(Graphics g)
        {
            int height = this.PADY;
            foreach (XPTaskBarItem item in items)
            {
                if (item.Visible)
                {
                    if (focusTextBounds)
                    {
                        Size textSize = Size.Ceiling(g.MeasureString(item.Text, item.ItemFont));
                        int prefWidth = textSize.Width;

                        int max = (this.ClientRectangle.Width - (2 * (PADX + PADY))) - PADX;

                        if (prefWidth > max)
                        {
                            prefWidth = max;
                        }
                        int prefHeight = GetPrefferedHeightForText(g, item.Text, this.Font,
                            max, false);

                        if (prefWidth == max)
                            this.UpdateWidthForText(g, item.Text, this.Font, ref prefWidth, false);

                        if (item.Image != null)
                        {
                            prefWidth += item.Image.Width;
                        }

                        if (this.ImageList != null && item.ImageIndex != -1 && prefHeight < this.ImageList.ImageSize.Height)
                        {
                            prefHeight = this.ImageList.ImageSize.Height;
                        }
                        else if (item.Image != null && prefHeight < item.Image.Height)
                        {
                            prefHeight = item.Image.Height;
                        }

                        int x = 0;
                        int y = height + 1 + this.headerHeight;
                        if (!GetIsMirrored())
                        {
                            x = PADX;
                        }
                        else
                        {
                            x = this.ClientRectangle.Right - (prefWidth + PADX);
                        }
                        item.Bounds = new Rectangle(x, y,
                            prefWidth, prefHeight);
                        height += item.Bounds.Height + PADY;                        
                    }
                    else
                    {
                        int itemWidth = this.ClientRectangle.Width - (this.PADX * 2);
                        int textWidth = itemWidth;
                        if (this.ImageList != null && this.ImageList.Images.Count > 0)
                        {
                            textWidth -= this.ImageList.ImageSize.Width;
                        }
                        else if (item.Image != null)
                        {
                            textWidth -= item.Image.Width;
                        }

                        int prefHeight = GetPrefferedHeightForText(g, item.Text, this.Font,
                            textWidth, false);

                        if (this.ImageList != null && item.ImageIndex != -1 && prefHeight < this.ImageList.ImageSize.Height)
                        {
                            prefHeight = this.ImageList.ImageSize.Height;
                        }
                        else if (item.Image != null && prefHeight < item.Image.Height)
                        {
                            prefHeight = item.Image.Height;
                        }

                        item.Bounds = new Rectangle(PADX, height + 1 + this.headerHeight,
                            itemWidth, prefHeight);
                        height += item.Bounds.Height + PADY;
                    }
                }
            }
            if (this.ChildPanel != null)
            {
                // If there were no items, then remove the PADY inserted above.
                if (this.items.Count == 0)
                    height -= this.PADY;

                this.UpdateChildPanelBounds(this.headerHeight + height);
                height += this.preferredChildPanelHeight + 4;
                this.ChildPanel.ResumeLayout(false);
            }
            return height;
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <returns>The height for the items area.</returns>
        /// /// <remarks>
        /// <para>
        /// Called by <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.UpdatePreferredHeight"/>
        /// to determine the height for the items area.
        /// </para>
        /// </remarks>
        protected virtual int DetermineItemsRectHeightOnHorizontalAlignment(Graphics g)
        {
            int tempHeight = 0;
            int height = this.PADY;
            int width = this.PADX;

            tempHeight = this.headerHeight;
            int row = 1;
            foreach (XPTaskBarItem item in items)
            {
                if (item.Visible)
                {
                    if (focusTextBounds)
                    {
                        Size textSize = Size.Ceiling(g.MeasureString(item.Text, item.ItemFont));
                        int prefWidth = textSize.Width;

                        int max = (this.ClientRectangle.Width - (2 * (PADX + PADY))) - PADX;

                        if (prefWidth > max)
                        {
                            prefWidth = max;
                        }

                        int prefHeight = GetPrefferedHeightForText(g, item.Text, this.Font,
                            prefWidth, false);

                        if (prefWidth == max)
                            this.UpdateWidthForText(g, item.Text, this.Font, ref prefWidth, false);

                        if (item.Image != null)
                        {
                            prefWidth += item.Image.Width;
                        }
                        if (this.ImageList != null && item.ImageIndex != -1 && prefHeight < this.ImageList.ImageSize.Height)
                        {
                            prefHeight = this.ImageList.ImageSize.Height;
                        }
                        else if (item.Image != null && prefHeight < item.Image.Height)
                        {
                            prefHeight = item.Image.Height;
                        }
                        if ((width + item.Bounds.Width + PADX) > (this.ClientRectangle.Width - (this.PADX * 2)))
                        {
                            width = PADX;
                            tempHeight += item.Bounds.Height + PADY;
                            row++;
                        }
                        item.Bounds = new Rectangle(width, tempHeight + PADY,
                            prefWidth, prefHeight);
                        width += item.Bounds.Width + PADX;
                        height = (item.Bounds.Height * row) + PADY * row;
                    }
                    else
                    {
                        int itemWidth = 150;
                        int textWidth = itemWidth;
                        if (this.ImageList != null && this.ImageList.Images.Count > 0)
                        {
                            textWidth -= this.ImageList.ImageSize.Width;
                        }
                        else if (item.Image != null)
                        {
                            textWidth -= item.Image.Width;
                        }

                        int prefHeight = GetPrefferedHeightForText(g, item.Text, this.Font,
                            textWidth, false);

                        if (this.ImageList != null && item.ImageIndex != -1 && prefHeight < this.ImageList.ImageSize.Height)
                        {
                            prefHeight = this.ImageList.ImageSize.Height;
                        }
                        else if (item.Image != null && prefHeight < item.Image.Height)
                        {
                            prefHeight = item.Image.Height;
                        }
                        if ((width + item.Bounds.Width + PADX) > (this.ClientRectangle.Width - (this.PADX * 2)))
                        {
                            width = PADX;
                            tempHeight += item.Bounds.Height + PADY;
                            row++;
                        }

                        item.Bounds = new Rectangle(width, tempHeight + PADY,
                            itemWidth, prefHeight);
                        width += item.Bounds.Width + PADX;
                        height = (item.Bounds.Height * row) + PADY * row;
                    }
                }
            }
            if (this.ChildPanel != null)
            {
                // If there were no items, then remove the PADY inserted above.
                if (this.items.Count == 0)
                    height -= this.PADY;

                this.UpdateChildPanelBounds(this.headerHeight + height);
                height += this.preferredChildPanelHeight + 4;
            }
            return height;
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <returns>The height for the header portion.</returns>
        /// <remarks>
        /// <para>
        /// Called by <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox.UpdatePreferredHeight"/>
        /// to determine the height for the header area.
        /// </para>
        /// </remarks>
        protected virtual int DetermineHeaderHeight(Graphics g)
        {
            Size headerImageSize = GetHeaderImageSize();
            Size btnSize = this.GetHeaderButtonSize();

            int availableTextWidth = this.ClientRectangle.Width - this.PADX * 3 - btnSize.Width;

            if (headerImageSize.Width > 0)
                availableTextWidth -= (headerImageSize.Width + this.PADX);

            int prefHeight = GetPrefferedHeightForText(g, this.Text, this.HeaderFont, availableTextWidth, true);

            if (prefHeight < btnSize.Height)
                prefHeight = btnSize.Height;

            if (prefHeight < headerImageSize.Height)
            {
                this.extendedHeaderImageHeight = headerImageSize.Height - prefHeight;
                prefHeight = headerImageSize.Height;
            }
            else
                this.extendedHeaderImageHeight = 0;

            prefHeight += this.PADY * 2;
            return prefHeight;
        }

        /// <summary>
        /// Advanced method to aid customization.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context.</param>
        /// <param name="text">The text for which to determine the height.</param>
        /// <param name="width">The available width.</param>
        /// <returns>The preferred height.</returns>
        /// <remarks>
        /// <para>
        /// Called to determine the required height for the specified text given the available width.
        /// </para>
        /// </remarks>
        protected virtual int GetPrefferedHeightForText(Graphics g, string text, Font font, int width, bool forheader)
        {
            if (width <= 0)
            {
                width = 5;
            }

            StringFormat sf = new StringFormat();

            if (forheader && !this.ClipHeaderText)
            {
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;
            }

            Size szText = Size.Ceiling(g.MeasureString(text, font, width, sf));

            sf.Dispose();

            return szText.Height;
        }

        private void UpdateWidthForText(Graphics g, string text, Font font, ref int width, bool forheader)
        {
            if (width <= 0)
            {
                width = 5;
            }

            StringFormat sf = new StringFormat();

            if (forheader && !this.ClipHeaderText)
            {
                sf.FormatFlags |= StringFormatFlags.NoWrap;
                sf.Trimming = StringTrimming.EllipsisCharacter;
            }

            Size szText = Size.Ceiling(g.MeasureString(text, font, width, sf));

            sf.Dispose();

            width = szText.Width;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetSelectedItemNull()
        {
            this.selectedItem = null;
        }

        #region PAINTING
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            UpdatePreferredHeight(e.Graphics);

            Rectangle headerRect = this.GetHeaderRect();

            if (e.ClipRectangle.IntersectsWith(headerRect))
                DrawHeader(e);
            if (e.ClipRectangle.IntersectsWith(new Rectangle(0, this.headerHeight + 1, this.ClientRectangle.Width,
                this.ClientRectangle.Height - this.headerHeight)))
                DrawItems(e);

            if (this.Focused)
            {
                if (this.FocusedItem != null)
                {
                    Rectangle rect = this.FocusedItem.Bounds;
                    if (!this.ThemesEnabled && (this.Parent.Style == XPTaskBarStyle.Office2007 ||this.Parent.Style == XPTaskBarStyle.Office2010))
                    {
                        SizeF size = e.Graphics.MeasureString(this.FocusedItem.Text, this.FocusedItem.ItemFont);
                        Image image = this.FocusedItem.Image;
                        int nImgWidth = 0;

                        if (image != null)
                        {
                            nImgWidth = image.Width;
                        }

                        rect.Width = (int)size.Width;

                        if (this.RightToLeft == RightToLeft.No)
                        {
                            rect.X = this.FocusedItem.Bounds.X + this.FocusedItem.LeftSpacing + nImgWidth;
                        }
                        else
                        {
                            rect.X = this.FocusedItem.Bounds.Right - (int)size.Width + this.PADX + this.FocusedItem.LeftSpacing;
                        }

                        if (this.DrawFocusRect)
                        {
                            rect.Height++;
                        }
                    }
                    if (this.Parent.Style != XPTaskBarStyle.Metro)
                    {
                       if (this.DrawFocusRect)
                        ControlPaint.DrawFocusRectangle(e.Graphics, rect);
                    }
                }
                else
                {
                    if (this.Parent.Style != XPTaskBarStyle.Metro)
                    {
                        if (this.DrawFocusRect)
                            ControlPaint.DrawFocusRectangle(e.Graphics, this.GetHeaderRect());
                    }
                }
            }

            Rectangle itemsBounds = this.GetItemsRect();
            Point mousePos = this.PointToClient(MousePosition);

            // Draw separator between items when Drag&Drop is performed.
            if (m_bIsDragging && itemsBounds.Contains(mousePos))
            {
                DrawSeparator(e.Graphics);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// The Rectangle representing the header.
        /// </summary>
        /// <returns>A Rectangle instance.</returns>
        public virtual Rectangle GetHeaderRect()
        {
            int headerTop = GetExtraHeightForHeaderImage();

            return new Rectangle(0, headerTop, this.ClientRectangle.Width, this.headerHeight - headerTop);
        }

        internal Rectangle GetAnimateRect()
        {
            Rectangle animateRect = GetItemsRect();

            // Convert to screen co-ords
            return this.RectangleToScreen(animateRect);
        }

        /// <summary>
        /// The Rectangle representing the Items area.
        /// </summary>
        /// <returns>A Rectangle instance.</returns>
        public virtual Rectangle GetItemsRect()
        {
            return new Rectangle(0, this.headerHeight, this.ClientRectangle.Width, this.ClientRectangle.Height - this.headerHeight);
        }
        /// <summary>
        /// Draws the items portion of the task bar box.
        /// </summary>
        /// <param name="ea">The PaintEventArgs containing information about this Paint event.</param>
        protected virtual void DrawItems(PaintEventArgs ea)
        {
            bool bIsMirrored = GetIsMirrored();
            Graphics gfxTarget = ea.Graphics;
            Rectangle itemBounds = GetItemsRect();

            if (null != this.ThemedDrawing)
            {
                this.ThemedDrawing.DrawMirrored = bIsMirrored;
            }

            DrawItemsBackground(gfxTarget, itemBounds);

            foreach (XPTaskBarItem item in items)
            {
                if (item.Visible)
                {
                    DrawItem(ea, bIsMirrored, gfxTarget, item);
                }
            }
        }

        private void DrawItemsBackground(Graphics gfxTarget, Rectangle itemBounds)
        {
            // Group items background
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                if (XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes &&
                    (XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn))
                {
                    Color colorBG = XPThemes.IsOliveGreenThemeOn ? Color.FromArgb(246, 246, 236) : Color.FromArgb(240, 241, 245);
                    using (SolidBrush br = new SolidBrush(colorBG))
                        gfxTarget.FillRectangle(br, itemBounds);
                }
                else
                {
                    this.ThemedDrawing.DrawItemsBG(gfxTarget, itemBounds);
                }
            }
            else if (this.Parent.Style == XPTaskBarStyle.Office2007 || this.Parent.Style == XPTaskBarStyle.Office2010)
            {
                using (SolidBrush br = new SolidBrush(this.ItemBackColor))
                {
                    gfxTarget.FillRectangle(br, itemBounds);
                }
            }
            else
            {
                // Background
                ProvideBrushEventArgs args = new ProvideBrushEventArgs(itemBounds, null);
                this.OnProvideItemsBackGroundBrush(args);

                using (Brush itemBrush = (null != args.Brush) ? args.Brush : new SolidBrush(itemBackColor))
                {
                    gfxTarget.FillRectangle(itemBrush, itemBounds);
                }
            }
        }

        private void DrawItem(PaintEventArgs ea, bool bIsMirrored, Graphics gfxTarget, XPTaskBarItem item)
        {
            Rectangle bounds = item.Bounds;
            bounds.X = bounds.X + item.LeftSpacing;
            Image image = item.Image;
            Font itemFont = item.ItemFont;
            if (this.ThemesEnabled || this.Parent.Style != XPTaskBarStyle.Office2007 && this.Parent.Style != XPTaskBarStyle.Office2010)
            {
                if (item.Visible && image != null)
                {
                    DrawItemImage(ea, bIsMirrored, gfxTarget, item, ref bounds, image);
                }
                if (item.Text != String.Empty && item.Visible)
                {
                    DrawItemText(bIsMirrored, gfxTarget, item, bounds, itemFont);
                }
            }
            else
            {
                if (image != null)
                {
                    int nImgWidth = image.Width;

                    if (!bIsMirrored)
                    {
                        bounds.Offset(nImgWidth, 0);
                    }
                    bounds.Width -= nImgWidth;
                }

                if (item.Text != String.Empty)
                {
                    DrawItemText(bIsMirrored, gfxTarget, item, bounds, itemFont);
                }
                if (item.Visible && image != null)
                {
                    bounds = item.Bounds;
                    bounds.X = bounds.X + item.LeftSpacing;
                    DrawItemImage(ea, bIsMirrored, gfxTarget, item, ref bounds, image);
                }
            }
        }

        private void DrawItemImage(PaintEventArgs ea, bool bIsMirrored, Graphics gfxTarget, XPTaskBarItem item, ref Rectangle bounds, Image image)
        {
            int nImgWidth = image.Width;
            int nImgHeight = image.Height;
            int nImgLeft = bIsMirrored ? bounds.Right - nImgWidth : bounds.Left;

            if (item.Enabled)
            {
                ea.Graphics.DrawImage(image, nImgLeft, bounds.Top, nImgWidth, nImgHeight);
            }
            else
            {
                ControlPaint.DrawImageDisabled(gfxTarget, image, nImgLeft, bounds.Top, this.itemBackColor);
            }

            if (!bIsMirrored)
            {
                bounds.Offset(nImgWidth, 0);
            }

            bounds.Width -= nImgWidth;
        }

        private void DrawItemText(bool bIsMirrored, Graphics gfxTarget, XPTaskBarItem item, Rectangle bounds, Font itemFont)
        {
            Font font = this.Font;
            if (this.HitItem == item && item.Enabled)
                font = FontUtil.CreateFont(font, FontStyle.Underline);

            Color color = this.ForeColor;
            if (item.ForeColor != Color.Empty)
                color = item.ForeColor;

            // Draw text manually after querying for the text color (Need to do this since 
            // there is no way an underline can be drawn for the highlighted text.

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                if (XPThemes.IsOliveGreenThemeOn && XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes)
                    if (HitItem == item)
                        color = Color.FromArgb(114, 146, 29);
                    else
                        color = Color.FromArgb(86, 102, 45);
                else if (XPThemes.IsSilverThemeOn && XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes)
                    if (HitItem == item)
                        color = Color.FromArgb(126, 124, 124);
                    else
                        color = Color.FromArgb(63, 63, 61);
                else
                    color = this.ThemedDrawing.GetTextThemeColor(false, HitItem == item);
            }
            else
            {
                if (HitItem == item)
                {
                    if (hotTrackColor != Color.Empty)
                        color = hotTrackColor;
                    else
                        color = ControlPaint.Light(item.ForeColor);
                        
                    Color bgColor = this.itemBackColor;
                    if (bgColor == Color.Transparent)
                        bgColor = this.BackColor;

                    Control parent = this.Parent;
                    while (bgColor == Color.Transparent
                                && parent != null)
                    {
                        bgColor = parent.BackColor;
                        parent = parent.Parent;
                    }

                    if (bgColor != Color.Transparent && hotTrackColor == Color.Empty)
                        DrawingUtils.AdjustForeColorBrightnessForBackColor(ref color, bgColor, 0.5f);
                }
            }
            StringFormat sfFormat;
            if (this.WrapText)
            {
                sfFormat = new StringFormat(StringFormatFlags.LineLimit);
            }
            else
            {
                sfFormat = new StringFormat(StringFormatFlags.NoWrap);
                sfFormat.Trimming = StringTrimming.EllipsisCharacter;
            }
            if (bIsMirrored)
            {
                sfFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            if (item.Enabled)
            {
                if (this.Focused)
                {
                    if (item == this.focusedItem && !this.ThemesEnabled  )
                    {
                        if (this.Parent.Style == XPTaskBarStyle.Office2007)
                        {
                            Brush brBack = new SolidBrush(this.Parent.Office2007ColorTable.XPTaskBarBoxActiveHighlightedItemColor);
                            using (Pen brBorder = new Pen(Color.FromArgb(227, 197, 147)))
                            {
                                Rectangle rect = this.focusedItem.Bounds;
                                if (this.Parent != null)
                                {
                                    rect.X = 1;
                                    rect.Width = this.Parent.ClientRectangle.Width - 3;
                                }
                                gfxTarget.FillRectangle(brBack, rect);
                                gfxTarget.DrawRectangle(brBorder, rect);
                            }
                            brBack.Dispose();
                        }
                        else if (this.Parent.Style == XPTaskBarStyle.Office2010)
                        {
                            Brush brBack = new SolidBrush(this.Parent.Office2010ColorTable.XPTaskBarBoxActiveHighlightedItemColor);
                            using (Pen brBorder = new Pen(Color.FromArgb(227, 197, 147)))
                            {
                                Rectangle rect = this.focusedItem.Bounds;
                                if (this.Parent != null)
                                {
                                    rect.X = 1;
                                    rect.Width = this.Parent.ClientRectangle.Width - 3;
                                }
                                gfxTarget.FillRectangle(brBack, rect);
                                gfxTarget.DrawRectangle(brBorder, rect);
                            }
                            brBack.Dispose();
                        }
                        else if (this.Parent.Style == XPTaskBarStyle.Metro)
                        {
                            using (Brush brBack = new SolidBrush(ControlPaint.Light(this.Parent.MetroColor)))
                            {
                                Rectangle rect = this.focusedItem.Bounds;
                                if (this.Parent != null)
                                {
                                    rect.X = 1;
                                    rect.Width = this.Parent.ClientRectangle.Width - 3;
                                }
                                gfxTarget.FillRectangle(brBack, rect);
                            }
                        }
                    }                    
                }
                else
                {
                    if (item == this.selectedItem && !this.ThemesEnabled && (this.Parent.Style == XPTaskBarStyle.Office2007 || this.Parent.Style == XPTaskBarStyle.Office2010))
                    {
                        if (this.Parent.Style == XPTaskBarStyle.Office2007)
                        {
                            using (Brush br = new SolidBrush(this.Parent.Office2007ColorTable.XPTaskBarBoxInactiveHighlightedItemColor))
                            {
                                Rectangle rect = this.selectedItem.Bounds;
                                if (this.Parent != null)
                                {
                                    rect.X = 1;
                                    rect.Width = this.Parent.ClientRectangle.Width - 2;
                                }
                                gfxTarget.FillRectangle(br, rect);
                            }
                        }
                        else if (this.Parent.Style == XPTaskBarStyle.Office2010)
                        {
                            using (Brush br = new SolidBrush(this.Parent.Office2010ColorTable.XPTaskBarBoxInactiveHighlightedItemColor))
                            {
                                Rectangle rect = this.selectedItem.Bounds;
                                if (this.Parent != null)
                                {
                                    rect.X = 1;
                                    rect.Width = this.Parent.ClientRectangle.Width - 2;
                                }
                                gfxTarget.FillRectangle(br, rect);
                            }
                        }
                    }
                }

                using (Brush brush = new SolidBrush(color))
                {
                    gfxTarget.DrawString(item.Text, itemFont, brush, bounds, sfFormat);
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(SystemColors.GrayText))
                {
                    gfxTarget.DrawString(item.Text, font, brush, bounds, sfFormat);
                }
            }
            sfFormat.Dispose();
        }

        /// <summary>
        /// Draws the header portion of the task bar box.
        /// </summary>
        /// <param name="ea">The PaintEventArgs containing information about this Paint event.</param>
        protected virtual void DrawHeader(PaintEventArgs ea)
        {
            bool bIsMirrored = GetIsMirrored();
            bool needMirrored = GetHeaderNeedMirrored();

            Graphics gfxTarget = ea.Graphics;
            GraphicsPath headerBorderPath = GetHeaderBorderPath();

            if (null != this.ThemedDrawing)
            {
                this.ThemedDrawing.DrawMirrored = bIsMirrored;
            }

            RectangleF bounds = headerBorderPath.GetBounds();

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
            {
                if (XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes &&
                    (XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn))
                {
                    using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfxTarget, headerBorderPath, bIsMirrored))
                    {
                        Graphics gfxVirt = mdDrawer.VirtualGfx;
                        GraphicsPath pathVirt = mdDrawer.VirtualPath;

                        Matrix offsetMatrix = new Matrix();
                        offsetMatrix.Translate(0, GetExtraHeightForHeaderImage());
                        pathVirt.Transform(offsetMatrix);

                        Rectangle rectVirt = mdDrawer.VirtualBounds;

                        Color color1, color2;
                        if (XPThemes.IsOliveGreenThemeOn)
                        {
                            color1 = Color.FromArgb(255, 252, 236);
                            color2 = Color.FromArgb(224, 231, 184);
                        }
                        else
                        {
                            color1 = Color.White;
                            color2 = Color.FromArgb(214, 215, 224);
                        }

                        using (LinearGradientBrush headerBrush = new LinearGradientBrush(rectVirt, color1, color2, LinearGradientMode.Horizontal))
                        {
                            gfxVirt.FillPath(headerBrush, pathVirt);
                        }
                        if (this.BackgroundImage != null)
                            gfxTarget.DrawImage(this.BackgroundImage, pathVirt.GetBounds());
                    }
                }
                else
                {
                    this.ThemedDrawing.DrawHeader(gfxTarget, Rectangle.Ceiling(bounds));
                }
            }
            else if (this.Parent.Style == XPTaskBarStyle.Office2007)
            {
                using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfxTarget, headerBorderPath, bIsMirrored))
                {
                    Graphics gfxVirt = mdDrawer.VirtualGfx;
                    GraphicsPath pathVirt = mdDrawer.VirtualPath;

                    RectangleF rect = pathVirt.GetBounds();
                    rect.Y = bounds.Y;

                    using (SolidBrush br = new SolidBrush(Color.White))
                    {
                        gfxVirt.FillRectangle(br, rect);
                    }

                    Color backColor = this.HeaderBackColor;

                    rect.Y += 1;
                    rect.X += 1;
                    if (!this.HeaderHit)
                    {
                        Color startColor = backColor;
                        Color endColor = Color.FromArgb(175, backColor);
                        if (this.Parent.Office2007ColorScheme == Office2007Theme.Silver || this.Parent.Office2007ColorScheme == Office2007Theme.Black)
                        {
                            endColor = Color.FromArgb(120, backColor);
                        }
                        using (LinearGradientBrush linearBr = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Horizontal))
                        {
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.5f, 0.5f, 1f };
                            blend.Factors = new float[] { 0f, 1f, 1f, 0f };
                            linearBr.Blend = blend;

                            gfxVirt.FillRectangle(linearBr, rect);
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush linearBr = new LinearGradientBrush(rect, Color.FromArgb(175, backColor),
                            Color.FromArgb(70, backColor), LinearGradientMode.Horizontal))
                        {
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.3f, 0.7f, 1f };
                            blend.Factors = new float[] { 0f, 1f, 1f, 0f };
                            linearBr.Blend = blend;

                            gfxVirt.FillRectangle(linearBr, rect);
                        }
                    }

                    using (Pen pen = new Pen(this.Parent.Office2007ColorTable.XPTaskBarBoxHeaderLowerLineColor))
                    {
                        gfxVirt.DrawLine(pen, rect.Left - 1, rect.Bottom - 2, rect.Right, rect.Bottom - 2);
                    }
                    if (this.BackgroundImage != null)
                        gfxTarget.DrawImage(this.BackgroundImage, rect);
                }
            }
            else if (this.Parent.Style == XPTaskBarStyle.Office2010)
            {
                using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfxTarget, headerBorderPath, bIsMirrored))
                {
                    Graphics gfxVirt = mdDrawer.VirtualGfx;
                    GraphicsPath pathVirt = mdDrawer.VirtualPath;

                    RectangleF rect = pathVirt.GetBounds();
                    rect.Y = bounds.Y;

                    using (SolidBrush br = new SolidBrush(Color.White))
                    {
                        gfxVirt.FillRectangle(br, rect);
                    }

                    Color backColor = this.HeaderBackColor;

                    rect.Y += 1;
                    rect.X += 1;
                    if (!this.HeaderHit)
                    {
                        Color startColor = backColor;
                        Color endColor = Color.FromArgb(175, backColor);
                        if (this.Parent.Office2010ColorScheme == Office2010Theme.Silver || this.Parent.Office2010ColorScheme == Office2010Theme.Black)
                        {
                            endColor = Color.FromArgb(220, backColor);
                        }
                        using (LinearGradientBrush linearBr = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical))
                        {
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.5f, 0.5f, 1f };
                            blend.Factors = new float[] { 0f, 1f, 1f, 0f };
                            linearBr.Blend = blend;

                            gfxVirt.FillRectangle(linearBr, rect);
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush linearBr = new LinearGradientBrush(rect, Color.FromArgb(175, backColor),
                            Color.FromArgb(220, backColor), LinearGradientMode.Vertical))
                        {
                            Blend blend = new Blend();
                            blend.Positions = new float[] { 0f, 0.3f, 0.7f, 1f };
                            blend.Factors = new float[] { 0f, 1f, 1f, 0f };
                            linearBr.Blend = blend;

                            gfxVirt.FillRectangle(linearBr, rect);
                        }
                    }

                    using (Pen pen = new Pen(this.Parent.Office2010ColorTable.XPTaskBarBoxHeaderLowerLineColor))
                    {
                        gfxVirt.DrawLine(pen, rect.Left - 1, rect.Bottom - 2, rect.Right, rect.Bottom - 2);
                    }
                    if (this.BackgroundImage != null)
                        gfxTarget.DrawImage(this.BackgroundImage, rect);
                }
            }
            else if (this.Parent.Style == XPTaskBarStyle.Metro)
            {
                using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfxTarget, headerBorderPath, bIsMirrored))
                {
                    Graphics gfxVirt = mdDrawer.VirtualGfx;
                    GraphicsPath pathVirt = mdDrawer.VirtualPath;

                    RectangleF rect = pathVirt.GetBounds();
                    rect.Y = bounds.Y;

                    using (SolidBrush br = new SolidBrush(this.HeaderBackColor))
                    {
                        gfxVirt.FillRectangle(br, rect);
                    }

                    Color backColor = this.HeaderBackColor;

                    rect.Y += 1;
                    rect.X += 1;

                    if (this.BackgroundImage != null)
                        gfxTarget.DrawImage(this.BackgroundImage, rect);
                }
            }
            else
            {
                ProvideBrushEventArgs args = new ProvideBrushEventArgs(this.GetHeaderRect(), null);
                this.OnProvideHeaderBackGroundBrush(args);

                using (CMirroredDrawer mdDrawer = new CMirroredDrawer(gfxTarget, headerBorderPath, bIsMirrored))
                {
                    Graphics gfxVirt = mdDrawer.VirtualGfx;
                    GraphicsPath pathVirt = mdDrawer.VirtualPath;

                    // offset header path so that image is drawn slightly upper
                    // then header
                    Matrix offsetMatrix = new Matrix();
                    offsetMatrix.Translate(0, GetExtraHeightForHeaderImage());
                    pathVirt.Transform(offsetMatrix);

                    // Header background
                    using (Brush brush = (args.Brush != null) ? args.Brush : new SolidBrush(HeaderBackColor))
                    {
                        gfxVirt.FillPath(brush, pathVirt);
                    }
                     if (this.BackgroundImage != null)
                    gfxTarget.DrawImage(this.BackgroundImage, pathVirt.GetBounds());
                }
            }

            // Header foreground
            if (this.Text != String.Empty)
            {
                Rectangle layoutRect = this.GetHeaderTextBounds();

                bool bTextDrawn = false;
                Color colorText;
                if (this.HeaderForeColor != Color.Empty)
                    colorText = this.HeaderForeColor;
                else
                    colorText = this.ForeColor;

                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled)
                {
                    if (XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes &&
                        (XPThemes.IsOliveGreenThemeOn || XPThemes.IsSilverThemeOn || XPThemes.IsDefaultBlueThemeOn))
                    {
                        colorText = XPThemes.IsOliveGreenThemeOn ? Color.FromArgb(86, 102, 45) : Color.FromArgb(63, 63, 61);
                        if (XPThemes.IsDefaultBlueThemeOn)
                            colorText = Color.FromArgb(33, 93, 198);
                    }
                    else
                    {
                        this.ThemedDrawing.DrawBarText(gfxTarget, this.Text, layoutRect, true, false, bIsMirrored, this.HeaderTextAlign, this.ClipHeaderText);
                        bTextDrawn = true;
                    }
                }
                else if (this.Parent.Style == XPTaskBarStyle.Office2007)
                {
                    colorText = this.Parent.Office2007ColorTable.XPTaskBarBoxForeColor;
                }
                else if (this.Parent.Style == XPTaskBarStyle.Office2010)
                {
                    colorText = this.Parent.Office2010ColorTable.XPTaskBarBoxForeColor;
                }

                if (!bTextDrawn)
                {
                    StringFormat sfFormat = new StringFormat();
                    if (bIsMirrored)
                    {
                        sfFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    }

                    if (!this.ClipHeaderText)
                    {
                        sfFormat.FormatFlags |= StringFormatFlags.NoWrap;
                        sfFormat.Trimming = StringTrimming.EllipsisCharacter;
                    }

                    sfFormat.Alignment = this.HeaderTextAlign;

                    using (Brush brush = new SolidBrush(colorText))
                    {
                        gfxTarget.DrawString(Text, HeaderFont, brush, layoutRect, sfFormat);
                    }
                    sfFormat.Dispose();
                }
            }

            // The Expander Button
            if (this.ShowCollapseButton)
            {
                if (this.Parent.Style == XPTaskBarStyle.Default || (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
                {
                    this.button.DrawButton(gfxTarget, this.ThemedDrawing, this.ThemesEnabled);
                }
                else if (this.Parent.Style == XPTaskBarStyle.Office2010)
                {
                    Color arrowColor = this.Parent.Office2010ColorTable.XPTaskBarBoxArrowColor;
                    Color backColor = this.Parent.Office2010ColorTable.XPTaskBarBoxBackColor;
                    this.button.DrawOffice2010Button(gfxTarget, arrowColor, backColor);
                }
                else if (this.Parent.Style == XPTaskBarStyle.Metro)
                {
                    Color arrowColor = Color.White;
                    Color backColor = this.Parent.MetroColor;
                    this.button.DrawMetroButton(gfxTarget, arrowColor, backColor);
                }
                else
                {
                    Color arrowColor = this.Parent.Office2007ColorTable.XPTaskBarBoxArrowColor;
                    Color backColor = this.Parent.Office2007ColorTable.XPTaskBarBoxBackColor;
                    this.button.DrawOffice2007Button(gfxTarget, arrowColor, backColor);
                }
            }

            // The header image
            ImageList ilImgList = this.HeaderImageList;
            if (ilImgList != null && this.HeaderImageIndex != -1
                && ilImgList.Images.Count > this.HeaderImageIndex)
            {
                int nImgLeft = needMirrored ?
                    (this.ClientRectangle.Width - this.PADX - ilImgList.ImageSize.Width) :
                    (this.PADX);
                ilImgList.Draw(gfxTarget, nImgLeft, this.PADY, this.HeaderImageIndex);
            }
        }

        /// <summary>
        /// Indicates whether mirrored icon and expander button in header is required.
        /// </summary>
        /// <returns></returns>
        private bool GetHeaderNeedMirrored()
        {
            bool headerRTL = this.HeaderDirection == HeaderDirectionFormat.RightToLeft;
            return (this.GetIsMirrored() != headerRTL);
        }

        /// <summary>
        /// Returns the header text bounds.
        /// </summary>
        /// <returns>A Rectangle in the XPTaskBarBox client area.</returns>
        protected virtual Rectangle GetHeaderTextBounds()
        {
            int nClientWidth = this.ClientRectangle.Width;
            int nAvailableWidth = nClientWidth - this.PADX * 3 - this.GetHeaderButtonSize().Width;

            int nTop = this.PADY;
            int nLeft = 0;

            bool needMirrored = this.GetHeaderNeedMirrored();

            if (needMirrored)
            {
                nLeft = nClientWidth - nAvailableWidth - this.PADX;
            }
            else
            {
                nLeft = this.PADX;
            }

            Size headerImageSize = this.GetHeaderImageSize();
            if (headerImageSize.Width > 0)
            {
                int nImgOffset = headerImageSize.Width + this.PADX;
                if (!needMirrored)
                {
                    nLeft += nImgOffset;
                }
                nAvailableWidth -= nImgOffset;
            }

            int headerTop = this.GetExtraHeightForHeaderImage();
            nTop += headerTop;

            return new Rectangle(nLeft, nTop, nAvailableWidth, this.headerHeight - this.PADY * 2 - headerTop);
        }
        #endregion PAINTING

        #region Drag & Drop
        // Theese constants used to draw separator that indicates, where to 
        // insert item in Drag&Drop operations.
        private const int DEF_TRIANGLE_WIDTH = 2;
        private const int DEF_SEPARATOR_WIDTH = 2;
        private const int DEF_SEPARATOR_INFLATE = 3;
        private const int DEF_OFFSET = 1;

        private const int DEF_INVALID_INSERTION_INDEX = int.MaxValue;
        /// <summary>
        /// Used to determine, start drag and drop or not.
        /// </summary>
        private Point m_mouseDownPos = Point.Empty;
        /// <summary>
        /// Item to drag.
        /// </summary>
        private XPTaskBarItem m_dragItem = null;
        /// <summary>
        /// Indicates whether dragging is performed.
        /// </summary>
        private bool m_bIsDragging = false;
        /// <summary>
        /// Used to draw separator line between items, when dragging.
        /// </summary>
        private Rectangle m_separatorBounds = Rectangle.Empty;
        private Point m_mousePos = Point.Empty;
        private int m_insertionIndex = DEF_INVALID_INSERTION_INDEX;

        /// <summary>
        /// Returns the index to insert the item, from the specified point.
        /// </summary>
        /// <param name="pt"> Point to determine insertion index for. </param>
        /// <returns> Item index or the inset item after; -1 indicates item should be inserted first.
        /// </returns>
        private int GetInsertionIndex(Point pt)
        {
            int indexToInsertAt = DEF_INVALID_INSERTION_INDEX;

            // Insert as first item
            if (this.Items.Count == 0)
            {
                indexToInsertAt = -1;
            }

            else
            {
                int minDistance = int.MaxValue;

                // Insert as fisrt item, if there is no items
                if (pt.Y <= this.Items[0].Bounds.Bottom)
                {
                    indexToInsertAt = -1;
                }

                // Determine item to insert after
                else
                {
                    for (int i = 0, len = this.Items.Count; i < len; i++)
                    {
                        int bottom = this.Items[i].Bounds.Bottom;
                        int distance = Math.Abs(bottom - pt.Y);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            indexToInsertAt = i;
                        }
                    }
                }
            }

            return indexToInsertAt;
        }

        private void InvalidateSeparator()
        {
            Rectangle bounds = this.GetSeparatorLineBounds();
            bounds.Inflate(DEF_TRIANGLE_WIDTH, DEF_TRIANGLE_WIDTH);
            Invalidate(bounds);
        }

        /// <summary>
        /// Returns separator line bounds.
        /// </summary>
        private Rectangle GetSeparatorLineBounds()
        {
            Rectangle bounds = GetItemsRect();

            // Calculate bounds to draw separator in
            bounds.Inflate(-DEF_SEPARATOR_INFLATE, 0);
            bounds.Height = DEF_SEPARATOR_WIDTH;

            // Separator is drawn after some item
            if (m_insertionIndex >= 0 && m_insertionIndex < this.Items.Count)
            {
                XPTaskBarItem item = this.Items[m_insertionIndex];
                bounds.Y = item.Bounds.Bottom;
                if (bounds.Y >= this.ClientRectangle.Height)
                {
                    bounds.Y = this.ClientRectangle.Height - DEF_SEPARATOR_WIDTH;
                }
            }

                // Separator is drawn before first item
            else if (m_insertionIndex < 0)
            {
                if (this.Items.Count > 0)
                {
                    XPTaskBarItem item = this.Items[0];
                    bounds.Y = item.Bounds.Top;
                    bounds.Y--;
                }
                else
                {
                    bounds.Y++;
                }
            }

            return bounds;
        }

        /// <summary>
        /// Draws separator between items, when dragging is performed.
        /// </summary>
        private void DrawSeparator(Graphics g)
        {
            if (m_bIsDragging)
            {
                RectangleF prevClipRect = g.ClipBounds;

                // Set drawing area to items drawing bounds
                Rectangle bounds = GetItemsRect();
                g.SetClip(bounds);

                bounds = this.GetSeparatorLineBounds();

                // Draw separator line
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(brush, bounds);
                }

                // Draw left and right separator "corners" 
                int j = DEF_TRIANGLE_WIDTH;
                for (int i = 0, len = DEF_TRIANGLE_WIDTH; i < len; i++)
                {
                    // Left triangle
                    g.DrawLine(Pens.Black, bounds.Left + i, bounds.Top - j,
                        bounds.Left + i, bounds.Bottom + j - DEF_OFFSET);

                    // Right triangle
                    g.DrawLine(Pens.Black, bounds.Right - i - DEF_OFFSET, bounds.Top - j,
                        bounds.Right - i - DEF_OFFSET, bounds.Bottom + j - DEF_OFFSET);

                    j--;
                }

                // Restore previous clip bounds
                g.SetClip(prevClipRect);
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        protected void AttemptStartDragging()
        {
            DragDropEffects dragEffects = DragDropEffects.None;
            Point mousePos = this.PointToClient(MousePosition);

            if (m_mouseDownPos != Point.Empty)
            {
                int width = Math.Abs(m_mouseDownPos.X - mousePos.X);
                int height = Math.Abs(m_mouseDownPos.Y - mousePos.Y);

                if (this.AllowDrop && m_dragItem != null && !m_bIsDragging &&
                    (width >= SystemInformation.DragSize.Width ||
                    height >= SystemInformation.DragSize.Height))
                {
                    m_mouseDownPos = Point.Empty;
                    m_bIsDragging = true;
                    this.FocusedItem = m_dragItem;
                    dragEffects = this.DoDragDrop(m_dragItem, DragDropEffects.Copy | DragDropEffects.Move);
                    m_dragItem = null;
                }
            }

            if (dragEffects == DragDropEffects.Move)
            {
                this.FocusedItem = null;
                Invalidate();
            }
        }
        /// <summary>
        /// Initiates the DragEnter process.
        /// </summary>
        protected void ProcessDragEnter(DragEventArgs e)
        {
            m_bIsDragging = e.Data.GetDataPresent(typeof(XPTaskBarItem));

            if (m_bIsDragging)
            {
                Point mousePos = this.PointToClient(MousePosition);
                m_insertionIndex = this.GetInsertionIndex(mousePos);
                InvalidateSeparator();
            }
        }
        /// <summary>
        /// Initiates the DragLeave process.
        /// </summary>
        protected void ProcessDragLeave()
        {
            m_bIsDragging = false;
            InvalidateSeparator();
            m_insertionIndex = DEF_INVALID_INSERTION_INDEX;
        }
        /// <summary>
        /// Initiates the DragOver process.
        /// </summary>
        protected void ProcessDragOver(DragEventArgs e)
        {
            IDataObject data = e.Data;

            // XpTaskBarItem is dragged
            if (data.GetDataPresent(typeof(XPTaskBarItem)))
            {
                Point mousePos = this.PointToClient(MousePosition);

                // Clear previous drawn separator
                InvalidateSeparator();

                // Calculate item index to draw separator line after
                m_insertionIndex = this.GetInsertionIndex(mousePos);

                // Draw separator at new position	
                InvalidateSeparator();

                Rectangle itemsBounds = this.GetItemsRect();

                // Determine drag effect
                if (itemsBounds.Contains(mousePos))
                {
                    e.Effect = ((Control.ModifierKeys & Keys.Control) != Keys.None) ?
                        DragDropEffects.Copy : DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }
        /// <summary>
        /// Initiates the DragDrop process.
        /// </summary>
        protected void ProcessDragDrop(DragEventArgs e)
        {
            m_bIsDragging = false;

            InvalidateSeparator();

            IDataObject data = e.Data as IDataObject;
            if (data != null)
            {
                XPTaskBarItem draggedItem =
                    data.GetData(typeof(XPTaskBarItem)) as XPTaskBarItem;

                if (draggedItem != null)
                {
                    XPTaskBarBox taskBarBox = draggedItem.Parent as XPTaskBarBox;

                    Image image = null;
                    bool keepImageIndex = false;
                    if (taskBarBox != this)
                    {
                        //If switched taskBarBoxes have the same ImageList we can keep ImageIndex
                        //of dragged item.
                        keepImageIndex = (taskBarBox != null) && (taskBarBox.ImageList != null)
                            && (taskBarBox.ImageList == this.ImageList);

                        if (!keepImageIndex)
                        {
                            image = draggedItem.Image;
                            if (image != null)
                            {
                                image = new Bitmap(image);
                            }
                        }
                    }

                    if (taskBarBox != null && (e.Effect == DragDropEffects.Move ||
                        e.Effect == DragDropEffects.Copy))
                    {
                        int indexInsertAt = m_insertionIndex;

                        if (e.Effect == DragDropEffects.Move)
                        {
                            int draggedItemIndex = this.Items.IndexOf(draggedItem);

                            if (draggedItemIndex >= 0 && draggedItemIndex <= indexInsertAt)
                            {
                                indexInsertAt--;
                            }

                            taskBarBox.Items.Remove(draggedItem);
                        }

                        else if (e.Effect == DragDropEffects.Copy)
                        {
                            draggedItem = draggedItem.Clone() as XPTaskBarItem;
                        }

                        if (taskBarBox != this)
                        {
                            //If ImageLists of taskBarBoxes are different we copy 
                            //Image and set ImageIndex to -1.
                            if (!keepImageIndex)
                            {
                                draggedItem.ImageIndex = -1;
                                draggedItem.SetImage(image);
                            }
                        }

                        indexInsertAt = (indexInsertAt < 0) ? 0 : ++indexInsertAt;
                        this.Items.Insert(indexInsertAt, draggedItem);
                    }

                    taskBarBox.FinishDragging();
                }
            }

            m_insertionIndex = DEF_INVALID_INSERTION_INDEX;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal void FinishDragging()
        {
            m_bIsDragging = false;
            Invalidate();
        }


        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            ProcessDragLeave();
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            ProcessDragEnter(drgevent);
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            ProcessDragOver(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            ProcessDragDrop(drgevent);
        }
        #endregion
        /// <summary>
        /// Indicates whether the collapse button has to be toggled.
        /// </summary>
        protected internal bool ShouldToggle()
        {
            Point mousePos = this.PointToClient(Control.MousePosition);
            bool bIsMouseOverButton = (this.ShowCollapseButton &&
                button != null && button.Bounds.Contains(mousePos));

            bool bShouldToggle = (this.HeaderHit && (!this.ToggleByButton ||
                bIsMouseOverButton));

            return bShouldToggle;
        }

        #region HIGHLIGHING
        private bool headerHit = false;
        /// <summary>
        /// Indicates whether the mouse is currently over the header portion.
        /// </summary>
        /// <value>True to indicate the mouse is over the header; false otherwise.</value>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool HeaderHit
        {
            get { return headerHit; }
            set
            {
                if (headerHit != value)
                {
                    headerHit = value;
                    if (this.headerHit)
                        this.Cursor = Cursors.Hand;
                    else
                        this.Cursor = Cursors.Default;
                    this.Invalidate(new Rectangle(this.ClientRectangle.Left, this.ClientRectangle.Top, this.ClientRectangle.Width, this.headerHeight));
                }
            }
        }
        private XPTaskBarItem hitItem = null;
        /// <summary>
        /// Gets / sets the current <see cref="XPTaskBarItem"/> on which the mouse is on.
        /// </summary>
        /// <value>An XPTaskBarItem instance.</value>
        protected virtual XPTaskBarItem HitItem
        {
            get { return hitItem; }
            set
            {
                if (hitItem != value)
                {
                    if (hitItem != null)
                        this.Invalidate(hitItem.Bounds);
                    hitItem = value;
                    if (hitItem != null && hitItem.Enabled)
                    {
                        this.Invalidate(hitItem.Bounds);
                        this.Cursor = Cursors.Hand;
                    }
                    else
                        this.Cursor = Cursors.Default;
                }
            }
        }

        private bool m_bHitTaskBoxArea = false;

        [Browsable(false)]
        public bool HitTaskBoxArea
        {
            get { return m_bHitTaskBoxArea; }
            set
            {
                if (m_bHitTaskBoxArea != value)
                    m_bHitTaskBoxArea = value;
            }
        }

        private XPTaskBarItem FocusedItem
        {
            get { return this.focusedItem; }
            set
            {
                if (this.focusedItem != value)
                {
                    if (this.focusedItem != null)
                        this.Invalidate();
                    else if (this.Focused)
                        // The header probably needs to be redrawn
                        this.Invalidate(this.GetHeaderRect());

                    this.focusedItem = value;
                    this.selectedItem = value;

                    if (this.focusedItem != null)
                        this.Invalidate();
                    else if (this.Focused)
                        // The header probably needs to be redrawn
                        this.Invalidate(this.GetHeaderRect());
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="XPTaskBarItem"/> at the specified point on the client.
        /// </summary>
        /// <param name="mousePoint">The <see cref="System.Drawing.Point"/> representing the client co-ordinates.</param>
        /// <returns>The XPTaskBarItem at the specified point. Null if none is found.</returns>
        public XPTaskBarItem HitTest(Point mousePoint)
        {
            foreach (XPTaskBarItem item in items)
            {
                if (item.Bounds.Contains(mousePoint) && item.Visible)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Deactivate tooltip on LostFocus event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            m_toolTip.Active = false;
            base.OnLostFocus(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            if (this.FindForm() == null && !this.Parent.ContainsFocus)
                this.Focus();

            base.OnMouseHover(e);
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Cache the hit item and invoke ItemClick only if this is the same item on mouse up.
                this.hitItemOnMouseDown = HitItem;

                if (this.HeaderHit)
                    this.Invalidate(this.button.Bounds);


                m_mouseDownPos = new Point(e.X, e.Y);
                m_dragItem = this.GetItemAtPoint(m_mouseDownPos);
            }

            base.OnMouseDown(e);
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.FindForm() == null && !this.Parent.ContainsFocus)
                this.Focus();

            Rectangle headerRect = this.GetHeaderRect();

            AttemptStartDragging();

            if (this.ShowCollapseButton && headerRect.Contains(e.X, e.Y))
            {
                this.HeaderHit = true;
            }
            else
                this.HeaderHit = false;

            //bool bShouldChangeCursor = this.ShouldToggle();

            //this.Cursor = bShouldChangeCursor ? Cursors.Hand : Cursors.Default;
            if (this.HeaderHit && this.ToggleByButton && !this.ExpanderButton.Bounds.Contains(e.X, e.Y))
                this.Cursor = Cursors.Default;
            else if (this.HeaderHit && this.ToggleByButton && this.ExpanderButton.Bounds.Contains(e.X, e.Y))
                this.Cursor = Cursors.Hand;

            XPTaskBarItem newHitItem = this.HitTest(new Point(e.X, e.Y));

            if ((newHitItem != null) && (newHitItem != this.HitItem || m_bIsItemClicked) && this.ShowToolTip)
            {
                m_toolTip.Active = false;
                string strToolTipText = newHitItem.ToolTip;
                if (strToolTipText != null && strToolTipText.Length > 0)
                {
                    m_toolTip.SetToolTip(this, strToolTipText);
                    m_toolTip.Active = true;
                }
            }

            m_bIsItemClicked = false;

            if (newHitItem == null)
            {
                m_toolTip.Active = false;
            }

            if (!this.HeaderHit && HitTest(new Point(e.X, e.Y)) == null)
            {
                this.HitTaskBoxArea = true;
            }
            else
            {
                this.HitTaskBoxArea = false;
            }

            this.HitItem = newHitItem;

            base.OnMouseMove(e);
        }
        /// </override>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (this.Parent != null)
            {
                foreach (XPTaskBarBox box in this.Parent.Controls)
                {
                    box.SetSelectedItemNull();
                    box.Invalidate();
                }
            }
        }
        /// </override>
        protected override void OnLeave(EventArgs e)
        {
            this.Focused = false;
            base.OnLeave(e);
            this.focusedItem = null;
            this.Invalidate();
        }
        /// </override>
        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                return true;
            }
            else if (keyData == Keys.Tab)
            {
                // If currently on the header
                if (this.FocusedItem == null && this.Items.Count > 0
                    && !this.Collapsed)
                {
                    return true;
                }
            }
            else if (keyData == (Keys.Tab | Keys.Shift))
            {
                // If not currently on the header
                if (this.FocusedItem != null)
                {
                    return true;
                }
            }
            else if ((keyData & Keys.KeyCode) == Keys.Up)
            {
                if (!this.Collapsed)
                    return this.CanMoveFocusedItem(true);
            }
            else if ((keyData & Keys.KeyCode) == Keys.Down)
            {
                if (!this.Collapsed)
                    return this.CanMoveFocusedItem(false);
            }
            else if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                return true;
            }
            return base.IsInputKey(keyData);
        }
        /// </override>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keys keyData = e.KeyData;
            if (keyData == Keys.Space)
            {
                this.ToggleContentVisibility(true);
                e.Handled = true;
            }
            else if (keyData == Keys.Tab)
            {
                // If currently on the header
                if (this.FocusedItem == null && this.Items.Count > 0
                    && !this.Collapsed)
                {
                    MoveFocusedItem(false);
                    e.Handled = true;
                }
            }
            else if (keyData == (Keys.Tab | Keys.Shift))
            {
                // If currently on the header
                if (this.FocusedItem != null)
                {
                    this.FocusedItem = null;
                    e.Handled = true;
                }
            }
            else if ((keyData & Keys.KeyCode) == Keys.Up)
            {
                if (!this.Collapsed)
                    e.Handled = this.MoveFocusedItem(true);
            }
            else if ((keyData & Keys.KeyCode) == Keys.Down)
            {
                if (!this.Collapsed)
                    e.Handled = this.MoveFocusedItem(false);
            }
            else if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                if (this.FocusedItem != null)
                    this.PerformItemClick(this.FocusedItem);
                else
                    this.ToggleContentVisibility(true);
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
        private bool CanMoveFocusedItem(bool up)
        {
            if (up)
            {
                // Already on top
                if (this.FocusedItem == null)
                    return false;
                else
                    return true;
            }
            else
            {
                int i = this.Items.IndexOf(this.FocusedItem);
                // Already on the last item.
                if (i == this.Items.Count - 1)
                    return false;
                else
                    return true;
            }
        }
        private bool MoveFocusedItem(bool up)
        {
            if (up)
            {
                // Already on top
                if (this.FocusedItem == null)
                    return false;
                else
                {
                    int i = this.Items.IndexOf(this.FocusedItem);
                    i--;

                    while (i >= 0)
                    {
                        XPTaskBarItem item = this.Items[i];

                        if (item.Visible && item.Enabled)
                        {
                            break;
                        }

                        i--;
                    }

                    if (i >= 0)
                        this.FocusedItem = this.Items[i];
                    else
                        this.FocusedItem = null;
                    return true;
                }
            }
            else
            {
                int i = this.Items.IndexOf(this.FocusedItem);
                int countDec = this.Items.Count - 1;
                i++;
                while (i <= countDec)
                {
                    XPTaskBarItem item = this.Items[i];

                    if (item.Visible && item.Enabled)
                    {
                        break;
                    }

                    i++;
                }

                // Already on the last item.
                if (i >= this.Items.Count)
                    return false;
                else
                {
                    this.FocusedItem = this.Items[i];
                    return true;
                }
            }
        }

        /// <summary>
        /// Toggles the collapsed/expanded state of the box.
        /// </summary>
        /// <param name="useAnimation">Indicates whether to use animation.</param>
        protected virtual void ToggleContentVisibility(bool useAnimation)
        {
            CancelEventArgs e = new CancelEventArgs();
            this.OnCollapsedStateChanging(e);

            if (!e.Cancel)
            {
                Point p = this.PointToClient(Control.MousePosition);
                if (this.ToggleByButton && !this.ExpanderButton.Bounds.Contains(p))
                {
                    return;
                }
                this.button.ExpandedState = !this.button.ExpandedState;
                // Make the child visible
                if (!this.Collapsed)
                {
                    if (this.ChildPanel != null)
                        this.ChildPanel.Visible = true;
                }

                if (this.Visible && useAnimation)
                {
                    this.Invalidate(this.button.Bounds);
                    bool expand = false;
                    if (this.button.ExpandedState == true)
                        expand = false;
                    else
                        expand = true;

                    this.OnBeforeAnimation(EventArgs.Empty);
                    this.animationHelper.StartAnimation(this.AnimationPositionsCount, expand, this.AnimationDelay);
                }
                else
                {
                    this.OnCollapsedStateChanged(EventArgs.Empty);
                }

                this.Update();
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_mouseDownPos = Point.Empty;
                m_bIsDragging = false;

                // OnMouseUp could be called after this control gets disposed, if it is a right
                // mouse up that follows a context menu popup (in whose handler the control could have been
                // destroyed).
                if (this.IsHandleCreated)
                {
                    if (this.HitItem != null && this.HitItem.Enabled)
                    {
                        this.FocusedItem = this.HitItem;
                    }

                    if (HeaderHit)
                    {
                        this.ToggleContentVisibility(true);
                    }
                    else if (HitItem != null && this.hitItemOnMouseDown == HitItem && HitItem.Enabled)
                    {
                        this.PerformItemClick(HitItem);
                        m_bIsItemClicked = true;
                    }

                    this.hitItemOnMouseDown = null;
                }

                if (HeaderHit || m_bIsItemClicked || this.HitTaskBoxArea || (HitItem != null && HitItem.Enabled))
                {
                    this.Focused = true;

                    Panel pnlParent = this.Parent as Panel;

                    if (pnlParent != null)
                    {
                        foreach (Control ctrlChild in pnlParent.Controls)
                        {
                            XPTaskBarBox xpBox = ctrlChild as XPTaskBarBox;
                            if (xpBox != null && !xpBox.Equals(this))
                            {
                                xpBox.Focused = false;
                            }
                        }
                    }
                }
                else
                {
                    this.Focused = false;
                }

                this.Invalidate();
            }

            base.OnMouseUp(e);
        }
        /// <summary>
        /// Raises the ItemClick event.
        /// </summary>
        /// <param name="args">An XPTaskBarItemClickArgs instance 
        /// containing data regarding this event.</param>
        /// <remarks>Raising an event invokes the event handler 
        /// through a delegate. <para>The OnItemClick method also 
        /// allows derived classes to handle the event without 
        /// attaching a delegate. This is the preferred technique 
        /// for handling the event in a derived class.</para>
        /// <para>Notes to Inheritors:  When overriding OnItemClick 
        /// in a derived class, be sure to call the base class's 
        /// OnItemClick method so that registered 
        /// delegates receive the event.</para>
        /// </remarks>
        protected virtual void OnItemClick(XPTaskBarItemClickArgs args)
        {
            if (this.ItemClick != null)
            {
                ItemClick(this, args);
            }
        }
        /// <summary>
        /// Raises the <see cref="ItemClick"/> event for the specified <see cref="XPTaskBarItem"/>.
        /// </summary>
        /// <param name="item">A <see cref="XPTaskBarItem"/> instance.</param>
        public void PerformItemClick(XPTaskBarItem item)
        {
            this.OnItemClick(new XPTaskBarItemClickArgs(item));
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            HitItem = null;
            this.HeaderHit = false;
            base.OnMouseLeave(e);
        }

        #endregion HIGHLIGHING

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnBackColorChanged"/>.
        /// </summary>
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (m_bPropagateBackColorToItems)
            {
                this.itemBackColor = this.BackColor;
            }
        }
        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.OnTextChanged"/>.
        /// </summary>
        /// <param name="e">EventArgs that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            OnLayoutNeed(new EventArgs());
        }
        /// <summary>
        /// Returns the <see cref="System.Drawing.Drawing2D.GraphicsPath"/> object representing the
        /// header border.
        /// </summary>
        /// <returns>The GraphicsPath object.</returns>
        protected GraphicsPath GetHeaderBorderPath()
        {
            GraphicsPath path = new GraphicsPath();

            int headerTop = GetExtraHeightForHeaderImage();

            Point[] lines = {new Point(0, headerTop + 2),
								new Point(2,headerTop + 0),
								new Point(this.ClientRectangle.Width-2, headerTop + 0),
								new Point(this.ClientRectangle.Width, headerTop + 2),
								new Point(this.ClientRectangle.Width, this.headerHeight),
								new Point(0, this.headerHeight),
								new Point(0, headerTop + 2)};
            path.AddLines(lines);
            return path;
        }

        [Documentation.DocumentationExclude()]
        protected virtual int GetExtraHeightForHeaderImage()
        {
            return this.extendedHeaderImageHeight;
        }

        [Documentation.DocumentationExclude()]
        protected Size GetHeaderImageSize()
        {
            if (this.HeaderImageList != null && this.HeaderImageIndex != -1
                && this.HeaderImageList.Images.Count > this.HeaderImageIndex)
            {
                return this.HeaderImageList.ImageSize;
            }
            return Size.Empty;
        }

        [Documentation.DocumentationExclude()]
        protected bool GetIsMirrored()
        {
            return RightToLeft.Yes == RightToLeft;
        }

        #region Delegates
        public delegate bool FocusEventHandler();
        #endregion

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SETFOCUS && this.Parent.AutoScroll)
            {
                this.Parent.IsChildSetFocus = true;
            }

            base.WndProc(ref m);
        }

        private ThemedXPTaskBarBoxDrawing ThemedDrawing
        {
            get
            {
                ThemedXPTaskBarBoxDrawing themedDrawing = null;

                if (XPThemes.IsThemedOS)
                {
                    if (m_weakThemedDrawing == null)
                    {
                        m_weakThemedDrawing = new WeakReference(CreateThemedDrawing());
                    }

                    if (m_weakThemedDrawing.IsAlive)
                    {
                        themedDrawing = (ThemedXPTaskBarBoxDrawing)m_weakThemedDrawing.Target;
                    }
                    else
                    {
                        themedDrawing = CreateThemedDrawing();
                        m_weakThemedDrawing.Target = themedDrawing;
                    }

                }

                return themedDrawing;
            }
        }

        private ThemedXPTaskBarBoxDrawing CreateThemedDrawing()
        {
            return new ThemedXPTaskBarBoxDrawing(this, ThemedControls.EXPLORERBAR);
        }
    }


    /// <summary>
    /// Specifies the action which occurs with item.
    /// </summary>
    public enum ItemAction
    {
        /// <summary>
        /// Item is showing.
        /// </summary>
        Show,
        /// <summary>
        /// Item is hiding.
        /// </summary>
        Hide,
        None
    }

    /// <summary>
    /// A collection of <see cref="XPTaskBarItem"/>s.
    /// </summary>
    public class XPTaskBarItemsCollection : ArrayListExt
    {
        /// <summary>
        /// Gets / sets a reference to the XPTaskBarItem at the specified index location in the
        /// XPTaskBarItemsCollection object.
        /// In C#, this property is the indexer for the XPTaskBarItemsCollection class.
        /// </summary>
        /// <param name="index">The location of the XPTaskBarItem in the 
        /// XPTaskBarItemsCollection collection.</param>
        /// <value>The reference to the XPTaskBarItem.</value>
        public new XPTaskBarItem this[int index]
        {
            get
            {
                return (XPTaskBarItem)base[index];
            }
            set
            {
                base[index] = value;
            }
        }
        protected override void AddHandlers(object newItem)
        {
            if (!(newItem is XPTaskBarItem))
                throw new ApplicationException("Only objects of type XPTaskBarItem can be added to the XPTaskBarItemsCollection list.");
            base.AddHandlers(newItem);
        }
        // Need this to force the designer from using this method while serializing (otherwise some designer-bugs show up).
        public virtual void AddRange(XPTaskBarItem[] items)
        {
            base.AddRange(items);
        }

        public override void Sort()
        {
            base.Sort(new XPTaskBarItemsComparer());
        }
    }

    /// <summary>
    /// Class implementing IComparer that assists in sorting of XpTaskBarBox Items.
    /// </summary>
    public class XPTaskBarItemsComparer : IComparer
    {
        //Constructor
        public XPTaskBarItemsComparer()
        {

        }

        #region IComparer Members
        /// <summary>
        /// Implementation of compare method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            XPTaskBarItem objX, objY;

            objX = x as XPTaskBarItem;
            objY = y as XPTaskBarItem;

            return objX.Text.CompareTo(objY.Text);
        }

        #endregion
    }

    /// <summary>
    /// Represents an item in the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>.
    /// </summary>
    /// <example>
    /// Take a look at the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBar"/> class reference for an example of the usage of this class.
    /// </example>
    [TypeConverter(
         typeof(XPTaskBarItemConverter)
         ),
    Serializable()
    ]
    public class XPTaskBarItem :
        IChangeNotifyingItem,
        ISerializable,
        ICloneable
    {
        private const string DEF_NAME = "XPTaskBarItem";

        private static int m_uniqueID = -1;

        public static string GenerateUniqueName()
        {
            return DEF_NAME + (++m_uniqueID).ToString();
        }

        #region FIELDS
        /// <summary>
        /// Used to draw item's image, after item has been dragged to other TaskBarBox.
        /// </summary>
        private Image m_image = null;
        /// <summary>
        /// Indicates whether the item is displayed. 
        /// </summary>
        private bool m_bVisible = true;
        /// <summary>
        /// Indicates whether the item can respond to user interaction.
        /// </summary>
        private bool m_bEnabled = true;
        /// <summary>
        /// Name of the item.
        /// </summary>
        private string m_sName = String.Empty;

        private Font m_itemFont = null;//SystemFonts.DefaultFont;
        private int m_leftSpacing = 0;

        private string text = String.Empty;
        private int imageIndex = -1;
        private Color foreColor = Color.Empty;
        private object tag = null;
        [NonSerialized()]
        private object parent;
        #endregion FIELDS

        /// <summary>
        /// Occurs when one of the XPTaskBarItem's property has changed.
        /// </summary>
        /// <remarks>
        /// This event may not be thrown for some of the properties
        /// in BarItem. Take a look at the property's documentation
        /// to confirm whether this event will be thrown for a property.
        /// <para>This event will also be raised when it's associated
        /// MainBarManager's value changes too.</para></remarks>
        public event SyncfusionPropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="args">An SyncfusionPropertyChangedEventArgs that contains the event data.</param>
        /// <remarks>
        /// The OnPropertyChanged method also allows derived classes to handle the event 
        /// without attaching a delegate. This is the preferred technique for 
        /// handling the event in a derived class. 
        /// <para>Notes to Inheritors:  When overriding OnPropertyChanged in a derived 
        /// class, be sure to call the base class's OnPropertyChanged method so that 
        /// registered delegates receive the event.</para>
        /// </remarks>
        protected void OnPropertyChanged(SyncfusionPropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Overloaded. Creates a new instance of the XPTaskBarItem class.
        /// </summary>
        public XPTaskBarItem()
        {
            this.Name = this.Text = GenerateUniqueName();
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Text", this.Text);
            info.AddValue("ImageIndex", this.ImageIndex);

            info.AddValue("Visible", this.Visible);
            info.AddValue("Enabled", this.Enabled);
            info.AddValue("Name", this.Name);

            info.AddValue("ForeColor", this.ForeColor);
            info.AddValue("Tag", this.Tag);

            info.AddValue("ItemFont", this.ItemFont);
            info.AddValue("LeftSpacing", this.LeftSpacing);
        }

        [
        Browsable(false),
        ]
        protected XPTaskBarItem(SerializationInfo info, StreamingContext context)
        {
            this.text = info.GetString("Text");
            this.imageIndex = info.GetInt32("ImageIndex");

            this.m_bVisible = info.GetBoolean("Visible");
            this.m_bEnabled = info.GetBoolean("Enabled");
            this.m_sName = info.GetString("Name");

            this.foreColor = (Color)info.GetValue("ForeColor", typeof(Color));
            this.tag = info.GetValue("Tag", typeof(object));

            this.ItemFont = (Font)info.GetValue("ItemFont", typeof(Font));
            this.LeftSpacing = info.GetInt32("LeftSpacing");
        }

        /// <summary>
        /// Creates a new instance of the XPTaskBarItem class and 
        /// sets its text, forecolor, imageIndex and indicates
        /// whether it is a hyper link.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="foreColor">The forecolor for the item.</param>
        /// <param name="imageIndex">The index into the XPTaskBarBox's ImageList.</param>
        /// <param name="toolTip">A tag object that you can set for convenience.</param>
        public XPTaskBarItem(string text, Color foreColor, /*bool hyperLink,*/ int imageIndex, string toolTip)
        {
            this.text = text;
            this.foreColor = foreColor;
            this.imageIndex = imageIndex;
            this.tag = null;
            this.ToolTip = toolTip;
        }

        /// <summary>
        /// Creates a new instance of the XPTaskBarItem class and 
        /// sets its text, forecolor, imageIndex, tag and indicates
        /// whether it is a hyper link.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="foreColor">The forecolor for the item.</param>
        /// <param name="imageIndex">The index into the XPTaskBarBox's ImageList.</param>
        /// <param name="tag">A tag object that you can set for convenience.</param>
        public XPTaskBarItem(string text, Color foreColor, /*bool hyperLink,*/ int imageIndex, object tag, string toolTip) :
            this(text, foreColor, imageIndex, toolTip)
        {
            this.tag = tag;
        }

        /// <summary>
        /// Creates a new instance of the XPTaskBarItem class and 
        /// sets its text, forecolor, imageIndex, tag and indicates
        /// whether it is a hyper link.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="foreColor">The forecolor for the item.</param>
        /// <param name="imageIndex">The index into the XPTaskBarBox's ImageList.</param>
        /// <param name="tag">A tag object that you can set for convenience.</param>
        /// <param name="toolTip">Contains text for tooltip. Empty string or null value means no tooltip.</param>
        /// <param name="visible">Indicates whether the item is displayed.</param>
        /// <param name="enabled">Indicates whether the item can respond to user interaction.</param>
        /// <param name="name">Name of the item.</param>		
        public XPTaskBarItem(string text, Color foreColor, int imageIndex, object tag, string toolTip, bool visible, bool enabled, string name) :
            this(text, foreColor, imageIndex, tag, toolTip)
        {
            this.Visible = visible;
            this.Enabled = enabled;
            this.Name = name;
        }

        public XPTaskBarItem(string text, Color foreColor, int imageIndex, object tag, string toolTip, bool visible, bool enabled, string name, Font font) :
            this(text, foreColor, imageIndex, tag, toolTip, visible, enabled, name)
        {
            this.ItemFont = font;
        }

        public XPTaskBarItem(string text, Color foreColor, int imageIndex, object tag, string toolTip, bool visible, bool enabled, string name, Font font, int leftSpacing)
            :
            this(text, foreColor, imageIndex, tag, toolTip, visible, enabled, name, font)
        {
            this.LeftSpacing = leftSpacing;
        }

        private string m_strToolTip = string.Empty;

        protected internal void SetImage(Image image)
        {
            m_image = image;
        }

        /// <summary>
        /// Returns item's image.
        /// </summary>
        protected internal Image Image
        {
            get
            {
                Image img = null;

                if (m_image != null)
                {
                    img = m_image;
                }
                else
                {
                    XPTaskBarBox taskBarBox = parent as XPTaskBarBox;
                    if (taskBarBox != null)
                    {
                        ImageList imgList = taskBarBox.ImageList;
                        if (imgList != null && imageIndex >= 0 && imageIndex <= imgList.Images.Count)
                        {
                            img = imgList.Images[imageIndex];
                        }
                    }
                }

                return img;
            }
        }
        /// <summary>
        /// Gets / sets the text for tooltip. Empty string or null value means no tooltip.
        /// </summary>
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
#endif
        public string ToolTip
        {
            get
            {
                return m_strToolTip;
            }
            set
            {
                if (value != m_strToolTip)
                {
                    m_strToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item font.
        /// </summary>
        public Font ItemFont
        {
            get
            {
                if (m_itemFont != null)
                    return m_itemFont;
                else
                {
                    XPTaskBarBox box = this.Parent as XPTaskBarBox;
                    if (box.Font != null)
                    return FontUtil.CreateFont(box.Font, FontStyle.Regular);
                    else
                        return FontUtil.CreateFont(SystemFonts.DefaultFont, FontStyle.Regular);
                }
            }
            set
            {
                if (value != m_itemFont)
                {
                    m_itemFont = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the spacing (indent) to the left of text.
        /// </summary>
        /// <value>The left spacing.</value>
        public int LeftSpacing
        {
            get
            {
                return m_leftSpacing;
            }
            set
            {
                if (value != m_leftSpacing)
                {
                    m_leftSpacing = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the item is displayed.
        /// </summary>
        [
            DefaultValue(true),
            Description("Gets or sets a value indicating whether the item is displayed.")
        ]
        public bool Visible
        {
            get
            {
                return m_bVisible;
            }
            set
            {
                if (m_bVisible != value)
                {
                    bool oldValue = m_bVisible;
                    m_bVisible = value;
                    OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(
                        PropertyChangeEffect.NeedRepaint, "Visible", oldValue, m_bVisible));
                }
            }
        }

        /// <summary>
        /// Indicates whether the item can respond to user interaction.
        /// </summary>
        [
            DefaultValue(true),
            Description("Gets or sets a value indicating whether the item can respond to user interaction.")
        ]
        public bool Enabled
        {
            get
            {
                return m_bEnabled;
            }
            set
            {
                if (m_bEnabled != value)
                {
                    bool oldValue = m_bEnabled;
                    m_bEnabled = value;
                    OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(
                        PropertyChangeEffect.NeedRepaint, "Enabled", oldValue, m_bEnabled));
                }
            }
        }

        /// <summary>
        /// Gets / sets the name of the item.
        /// </summary>
        [
            DefaultValue(""),
            Description("Gets or sets the name of the item.")
        ]
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                if (value != m_sName)
                {
                    string oldValue = m_sName;
                    m_sName = value;
                    OnPropertyChanged(new SyncfusionPropertyChangedEventArgs(
                        PropertyChangeEffect.None, "Name", oldValue, m_sName));
                }
            }
        }

        /// <summary>
        /// Gets / sets the item's text.
        /// </summary>
        /// <value>A string value.</value>
        /// <remarks><para>Changing this property's value will throw the PropertyChanged event.</para></remarks>
        [
        DefaultValue(""),
        Description("Specifies the item's text."),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))
        ]
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    string oldValue = text;
                    text = value;
                    OnPropertyChanged(
                        new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Text", oldValue, text));
                }
            }
        }

        /// <summary>
        /// Retrieves the image list associated with the item.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ImageList ImageList
        {
            get
            {
                ImageList imgList = null;
                XPTaskBarBox taskBarBox = parent as XPTaskBarBox;
                if (taskBarBox != null)
                {
                    imgList = taskBarBox.ImageList;
                }

                return imgList;
            }
        }
        /// <summary>
        /// Gets / sets an index into the ImageList in the corresponding XPTaskBarBox.
        /// </summary>
        /// <value>A zero-based index that represents the position 
        /// in the ImageList control (assigned to the ImageList 
        /// property of the XPTaskBarBox) where the image is located. The default is -1.</value>
        /// <remarks><para>Changing this property's value will throw the PropertyChanged event.</para></remarks>
        [
            TypeConverter(typeof(ImageIndexConverter)),
            Editor(typeof(ImageIndexEditor), typeof(UITypeEditor)),
            DefaultValue(-1),
            Description("Specifies an index into the ImageList in the corresponding XPTaskBarBox.")
        ]
        public int ImageIndex
        {
            get { return imageIndex; }
            set
            {
                if (imageIndex != value)
                {
                    int oldValue = imageIndex;
                    imageIndex = value;

                    if (imageIndex >= 0)
                    {
                        this.SetImage(null);
                    }
                    OnPropertyChanged(
                        new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "ImageIndex", oldValue, imageIndex));
                }
            }
        }

        /// <summary>
        /// Gets / sets the XPTaskBarBox that this item is part of.
        /// </summary>
        [Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Parent
        {
            get { return this.parent; }
            set { this.parent = value; }
        }

        /// <summary>
        /// Gets / sets the foreground color (typically the color 
        /// of the text) property of the XPTaskBarItem.
        /// </summary>
        /// <value>
        /// A color that represents the foreground color. The default is 
        /// Color.Empty.
        /// </value>
        /// <remarks><para>Changing this property's value will throw the PropertyChanged event.</para></remarks>
        [
        Description("Gets or sets the foreground color (typically the color of the text) property of the XPTaskBarItem."),
        ]
        public Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    Color oldValue = foreColor;
                    foreColor = value;
                    OnPropertyChanged(
                        new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Forecolor", oldValue, foreColor));
                }
            }
        }
        protected bool ShouldSerializeForeColor()
        {
            if (this.foreColor == Color.Empty)
                return false;
            else
                return true;
        }
        protected void ResetForeColor()
        {
            this.foreColor = Color.Empty;
        }

        /// <summary>
        /// Gets / sets the object that contains data about the item.
        /// </summary>
        /// <value>
        /// An Object that contains data about the control. 
        /// The default is a null reference (Nothing in Visual Basic).
        /// </value>
        /// <remarks>
        /// Any type derived from the Object class can be assigned 
        /// to this property. If the Tag property is set through 
        /// the Windows Forms designer, only text may be assigned.
        /// </remarks>
        [
            DefaultValue(null),
            TypeConverter(typeof(StringConverter)),
            Description("Gets or sets the object that contains data about the item.")
        ]
        public object Tag
        {
            get { return tag; }
            set
            {
                tag = value;
            }
        }
        private Rectangle bounds;
        public Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds != value)
                {
                    bounds = value;
                    OnPropertyChanged(
                        new SyncfusionPropertyChangedEventArgs(PropertyChangeEffect.NeedLayout, "Bounds", null, null));
                }
            }
        }

        #region ICloneable Members
        /// <summary>
        /// Creates a copy of ArrayList Collection.
        /// </summary>
        public object Clone()
        {
            XPTaskBarItem item = new XPTaskBarItem();
            item.ForeColor = this.ForeColor;
            item.Enabled = this.Enabled;
            item.ImageIndex = this.imageIndex;
            item.Text = this.Text;
            item.ToolTip = this.ToolTip;
            item.Visible = this.Visible;
            item.Tag = this.Tag;
            item.Bounds = this.Bounds;
            item.Name = this.Name;
            return item;
        }

        #endregion
    }
    /// <summary>
    /// Specifies the button that is drawn on a XPTaskBarBox header help expand/collapse it.
    /// </summary>
    public class ExpanderButton
    {
        private Rectangle bounds = Rectangle.Empty;
        /// <summary>
        /// Gets / sets the bounds of the button in the XPTaskBarBox's client Rectangle.
        /// </summary>
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        // True indicates the button arrow is drawn downward indicating "can expand".
        private bool expandLAF;
        /// <summary>
        /// Indicates whether the button is expanded or collapsed.
        /// </summary>
        /// <value>True for expanded state; false for collapsed.</value>
        public bool ExpandedState
        {
            get { return expandLAF; }
            set { expandLAF = value; }
        }

        internal void DrawButton(Graphics g, ThemedXPTaskBarBoxDrawing themedDrawing, bool themesEnabled)
        {
            if (!g.ClipBounds.IntersectsWith(bounds))
                return;

            bool done = false;
            Color arrowColorPen = Color.Black;
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && themesEnabled)
            {
                if (XPThemes.IsOliveGreenThemeOn && XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes)
                {
                    arrowColorPen = Color.FromArgb(86, 102, 45);
                }
                else if (XPThemes.IsSilverThemeOn && XPTaskBarBox.SimulateThemedPaintingForNonDefaultThemes)
                {
                    arrowColorPen = Color.FromArgb(86, 102, 45);
                }
                else
                {
                    themedDrawing.DrawButton(g, Bounds, ExpandedState);
                    done = true;
                }
            }

            if (!done)
            {
                SmoothingMode oldS = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Brush brush = new SolidBrush(Color.WhiteSmoke))
                {
                    g.FillEllipse(brush, bounds);
                }

                Point[] lines = null;
                if (!expandLAF)//Up
                {
                    Point[] temp = { new Point( bounds.Left + 5, bounds.Bottom - 8 ),
								     new Point( bounds.Left + bounds.Width / 2, bounds.Top + 4 ),
								     new Point( bounds.Right - 5, bounds.Bottom - 8 ) };
                    lines = temp;
                }
                else
                {
                    Point[] temp = { new Point( bounds.Left + 5, bounds.Top + 4 ),
					    			 new Point( bounds.Left + bounds.Width / 2, bounds.Bottom - 8 ),
						    		 new Point( bounds.Right - 5, bounds.Top + 4 ) };
                    lines = temp;
                }

                using (Pen arrowPen = new Pen(arrowColorPen, 1))
                {
                    g.DrawLines(arrowPen, lines);
                    lines[0].Offset(0, 3);
                    lines[1].Offset(0, 3);
                    lines[2].Offset(0, 3);
                    g.DrawLines(arrowPen, lines);
                }

                g.SmoothingMode = oldS;
            }
        }

        /// <summary>
        /// Draws office2007 like collapse button.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="arrowColor"></param>
        /// <param name="backColor"></param>
        internal void DrawOffice2007Button(Graphics g, Color arrowColor, Color backColor)
        {
            Point[] linesUpper = null;
            Point[] linesLower = null;
            if (!expandLAF)//Up
            {
                Point[] temp1 = { new Point( bounds.Left + 5, bounds.Top + 7 ),
								  new Point( bounds.Left + bounds.Width / 2, bounds.Top + 4 ),
								  new Point( bounds.Right - 5, bounds.Top + 7 ) };
                linesUpper = temp1;

                Point[] temp2 = { new Point( bounds.Left + 6, bounds.Top + 7 ),
					              new Point( bounds.Left + bounds.Width / 2, bounds.Top + 5 ),
					              new Point( bounds.Right - 6, bounds.Top + 7 ) };
                linesLower = temp2;
            }
            else
            {
                Point[] temp1 = { new Point( bounds.Left + 5, bounds.Top + 4 ),
					    	      new Point( bounds.Left + bounds.Width / 2, bounds.Top + 7 ),
						    	  new Point( bounds.Right - 5, bounds.Top + 4 ) };
                linesUpper = temp1;

                Point[] temp2 = { new Point( bounds.Left + 6, bounds.Top + 4 ),
					    	      new Point( bounds.Left + bounds.Width / 2, bounds.Top + 6 ),
						    	  new Point( bounds.Right - 6, bounds.Top + 4 ) };
                linesLower = temp2;
            }

            using (Pen arrowPen = new Pen(ControlPaint.Light(backColor), 2))
            {
                g.DrawLines(arrowPen, linesUpper);
                g.DrawLines(arrowPen, linesLower);
            }
            using (Pen arrowPen = new Pen(arrowColor))
            {
                g.DrawLines(arrowPen, linesUpper);
                g.DrawLines(arrowPen, linesLower);
            }

            linesUpper[0].Offset(0, 4);
            linesUpper[1].Offset(0, 4);
            linesUpper[2].Offset(0, 4);
            linesLower[0].Offset(0, 4);
            linesLower[1].Offset(0, 4);
            linesLower[2].Offset(0, 4);

            using (Pen arrowPen = new Pen(ControlPaint.Light(backColor), 2))
            {
                g.DrawLines(arrowPen, linesUpper);
                g.DrawLines(arrowPen, linesLower);
            }
            using (Pen arrowPen = new Pen(arrowColor))
            {
                g.DrawLines(arrowPen, linesUpper);
                g.DrawLines(arrowPen, linesLower);
            }
        }

        /// <summary>
        /// Draws office2010 like collapse button.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="arrowColor"></param>
        /// <param name="backColor"></param>
        internal void DrawOffice2010Button(Graphics g, Color arrowColor, Color backColor)
        {
            Point[] linesUpper = null;
            Point[] linesLower = null;
            if (!expandLAF)//Up
            {
                Point[] temp1 = { new Point( bounds.Left + 5, bounds.Top + 7 ),
								  new Point( bounds.Left + bounds.Width / 2, bounds.Top + 4 ),
								  new Point( bounds.Right - 5, bounds.Top + 7 ) };
                linesUpper = temp1;

                Point[] temp2 = { new Point( bounds.Left + 6, bounds.Top + 7 ),
					              new Point( bounds.Left + bounds.Width / 2, bounds.Top + 5 ),
					              new Point( bounds.Right - 6, bounds.Top + 7 ) };
                linesLower = temp2;
            }
            else
            {
                Point[] temp1 = { new Point( bounds.Left + 5, bounds.Top + 4 ),
					    	      new Point( bounds.Left + bounds.Width / 2, bounds.Top + 7 ),
						    	  new Point( bounds.Right - 5, bounds.Top + 4 ) };
                linesUpper = temp1;

                Point[] temp2 = { new Point( bounds.Left + 6, bounds.Top + 4 ),
					    	      new Point( bounds.Left + bounds.Width / 2, bounds.Top + 6 ),
						    	  new Point( bounds.Right - 6, bounds.Top + 4 ) };
                linesLower = temp2;
            }

            using (Pen arrowPen = new Pen(ControlPaint.Light(backColor), 2))
            {
                g.DrawLines(arrowPen, linesUpper);
                //g.DrawLines(arrowPen, linesLower);
            }
            using (Pen arrowPen = new Pen(arrowColor,2))
            {
                g.DrawLines(arrowPen, linesUpper);
                //g.DrawLines(arrowPen, linesLower);
            }

            linesUpper[0].Offset(0, 4);
            linesUpper[1].Offset(0, 4);
            linesUpper[2].Offset(0, 4);
            linesLower[0].Offset(0, 4);
            linesLower[1].Offset(0, 4);
            linesLower[2].Offset(0, 4);

            //using (Pen arrowPen = new Pen(ControlPaint.Light(backColor), 2))
            //{
            //    g.DrawLines(arrowPen, linesUpper);
            //    //g.DrawLines(arrowPen, linesLower);
            //}
            //using (Pen arrowPen = new Pen(arrowColor))
            //{
            //    g.DrawLines(arrowPen, linesUpper);
            //    //g.DrawLines(arrowPen, linesLower);
            //}
        }
        /// <summary>
        /// Draws Metro collapse button.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="arrowColor"></param>
        /// <param name="backColor"></param>
        internal void DrawMetroButton(Graphics g, Color arrowColor, Color backColor)
        {
            Point[] linesUpper = null;
            SmoothingMode sMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawEllipse(Pens.White,bounds);
            if (!expandLAF)//Up
            {
                Point[] temp1 = { new Point( bounds.Left + 4, bounds.Top + 9 ),
								  new Point( bounds.Left + bounds.Width / 2, bounds.Top + 5 ),
								  new Point( bounds.Right - 4, bounds.Top + 9 ) };
                linesUpper = temp1;
            }
            else
            {
                Point[] temp1 = { new Point( bounds.Left + 4, bounds.Top + 6 ),
					    	      new Point( bounds.Left + bounds.Width / 2, bounds.Top + 10 ),
						    	  new Point( bounds.Right - 4, bounds.Top + 6 ) };
                linesUpper = temp1;
            }
            using (Pen arrowPen = new Pen(arrowColor,2))
            {
                g.DrawLines(arrowPen, linesUpper);
            }
        }
    }
    class XPTaskBarItemConverter : ByteStreamTypeConverter
    {
        public override void OnBeforeDeserialize()
        {
#if SINGLE_DLL_BUILD
            AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Windows", typeof(XPTaskBarItem).Assembly);
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Controls", typeof(XPTaskBarItem).Assembly);
#endif

            // For backward compatibility
            AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(XPTaskBarItem).FullName, typeof(XPTaskBarItem).Assembly);
        }

        public override void OnAfterDeserialize()
        {
            // Reset
            AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(XPTaskBarItem).FullName, null);
        }

        public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
                return true;
            else
                return base.CanConvertTo(context, destinationType);
        } // end of method CanConvertTo

        public override /*TypeConverter*/ object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor)
                && (value is XPTaskBarItem))
            {
                XPTaskBarItem typename = (XPTaskBarItem)value;
                Type[] args;
                args = new Type[10];
                args[0] = typeof(string);
                args[1] = typeof(Color);
                args[2] = typeof(int);
                args[3] = typeof(object);
                args[4] = typeof(string);
                args[5] = typeof(bool);
                args[6] = typeof(bool);
                args[7] = typeof(string);
                args[8] = typeof(Font);
                args[9] = typeof(int);

                ConstructorInfo constructorInfo;
                constructorInfo = typeof(XPTaskBarItem).GetConstructor(args);
                if (constructorInfo != null)
                {
                    object[] argValues;
                    argValues = new Object[10];
                    argValues[0] = typename.Text;
                    argValues[1] = typename.ForeColor;
                    argValues[2] = typename.ImageIndex;
                    argValues[3] = typename.Tag;
                    argValues[4] = typename.ToolTip;
                    argValues[5] = typename.Visible;
                    argValues[6] = typename.Enabled;
                    argValues[7] = typename.Name;
                    argValues[8] = typename.ItemFont;
                    argValues[9] = typename.LeftSpacing;
                    return new InstanceDescriptor(constructorInfo, argValues);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        } // end of method ConvertTo
    }
}
