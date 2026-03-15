#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Core.Licensing;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// The XPTaskPane is a WordXP like control that you can see on the right when you start Microsoft Word XP.
    /// It is a wizard based class.
    /// You can easily Add/Remove pages using the designer verbs in the designer or by calling the AddPage/RemovePage methods of the base class.
    /// </summary>
    [Designer(typeof(XPTaskPaneDesigner))]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(XPTaskPane), "ToolboxIcons.XPTaskPane.bmp")]
    [Description("Provides options to navigate through pages.")]
    public class XPTaskPane : Wizard, INonClientPaintingSupport,IVisualStyle 
    {
        private XPTaskPaneHeader header;
        private XPTaskPaneHeaderLabel titleLabel;
        private IContainer components;
        private XPToolBar leftToolBar;
        private ImageList imageList;
        private BarItem backButton;
        private BarItem nextButton;
        private XPToolBar rightToolBar;
        private BarItem closeButton;
        private ParentBarItem dropdownButton;
        private bool m_bVerticalScroll = false;
        private int m_nScrollSpeed = 10;
        private VerticalScrollBar m_verticalScrollUp;
        private VerticalScrollBar m_verticalScrollDown;
        private VisualStyle m_vStyle = VisualStyle.Default;
        private ControlDrawing cd;
        private Office2007Theme m_colorScheme = Office2007Theme.Blue;
        private Office2010Theme m_color2010Scheme = Office2010Theme.Blue;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);
        /// <summary>
        /// 
        /// </summary>
        private static Size HeaderSize = default(Size);
        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2007Colors m_office2007ColorTable = null;
        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2010Colors m_office2010ColorTable = null;

        /// <summary>
        /// Gets or sets the scrolling speed of Vertical Scroll.
        /// </summary>
        [Browsable(true),
        DefaultValue(10),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Description("Indicates the scrolling speed of Vertical Scroll.")]
        public int ScrollSpeed
        {
            get
            {
                return m_nScrollSpeed;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("ScrollSpeed", "ScrollSpeed must be more than 1.");

                if (m_nScrollSpeed != value)
                    m_nScrollSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enables scroll buttons that occupy vertical space instead of horizontal space instead of default scrollbar.
        /// </summary>
        [Browsable(true),
        DefaultValue(false),
        Description("Indicates whether to enable scroll buttons that occupy vertical space instead of horizontal space instead of default scrollbar.")]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public bool VerticalScroll
#else
        public new bool VerticalScroll
#endif
        {
            get
            {
                return m_bVerticalScroll;
            }
            set
            {
                m_bVerticalScroll = value;

                if (!value)
                {
                    this.UpScroll.Visible = false;
                    this.DownScroll.Visible = false;
                }
                else
                {
                    this.CardLayout.LayoutMode = CardLayoutMode.Default;
                    UpdateVerticalScrolls();
                }
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VerticalScrollBar UpScroll
        {
            get
            {
                return m_verticalScrollUp;
            }
        }

       public bool ShouldSerializeUpScroll()
        {
            if (this.UpScroll.Location == new Point(0, 0) || this.UpScroll.Size == new Size(192, 10) ||
                this.UpScroll.Visible == false)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the vertical down scroll bar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public VerticalScrollBar DownScroll
        {
            get
            {
                return m_verticalScrollDown;
            }
        }

       public bool ShouldSerializeDownScroll()
        {
            if (this.DownScroll.Location == new Point(0, 0) || this.DownScroll.Size == new Size(192, 10) ||
                this.DownScroll.Visible == false)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets or sets the PageContainer of the XPTaskPane.
        /// </summary>
        [Description("Gets or sets the PageContainer of the WizardControl.")]
        [Category("Panels")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public WizardContainer TaskPanePageContainer
        {
            get 
            { 
                return this.PageContainer; 
            }
            set
            {
                if (this.TaskPanePageContainer != value)
                    SetPageContainer(value);
            }
        }
        public override void SetPageContainer(WizardContainer container)
        {
            base.SetPageContainer(container);
            if (this.TaskPanePageContainer != container)
            {
                if (this.CardLayout.ContainerControl == null)
                {
                    this.CardLayout.ContainerControl = container;
                }
            }
        }

        /// <summary>
        /// Gets or sets the array that holds pages.
        /// </summary>
        [Description("The array that holds the pages.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Appearance")]
        public virtual XPTaskPage[] TaskPages
        {
            get
            {
                if (!(base.WizardPages is XPTaskPage[]))
                {
                    // Older versions would have serialized this as WizardPage[].
                    XPTaskPage[] taskPages = new XPTaskPage[base.WizardPages.Length];
                    base.WizardPages.CopyTo(taskPages, 0);
                    return taskPages;
                }
                else
                    return this.WizardPages as XPTaskPage[];
            }
            set
            {
                this.WizardPages = value;
            }
        }

        /// <summary>
        /// Gets or sets the wizard pages.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override WizardPage[] WizardPages
        {
            get 
            { 
                return base.WizardPages; 
            }
            set 
            { 
                base.WizardPages = value; 
            }
        }

        /// <summary>
        /// Gets the Header Panel.
        /// </summary>
        [Description("The Header Panel.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XPTaskPaneHeader Header
        {
            get { return header; }
        }

       public bool ShouldSerializeHeader()
        {
            if (this.Header.BorderColor == Color.Black && this.Header.BorderStyle == BorderStyle.None &&
                this.Header.Dock == DockStyle.Top && this.Header.Size == new Size(192, 22))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets or sets the backcolor. (overridden property)
        /// </summary>
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                if (this.DesignMode && this.VisualStyle != VisualStyle.Default)
                {
                    if (this.VisualStyle == VisualStyle.Office2007 &&
                        value != Office2007ColorTable.GroupBarItemColorLight)
                        throw new ArgumentException("Cannot change BackColor, while VisualStyle isn't set to Default.");
                }

                base.BackColor = value;
            }
        }

        /// <summary>
        /// Resets the BackColor property to its default value.
        /// </summary>
        public override void ResetBackColor()
        {
            if (this.VisualStyle != VisualStyle.Office2007)
                base.ResetBackColor();
        }

        /// <summary>
        /// Gets the Header Title-Label.
        /// </summary>
        [Description("Reference to the Header Title-Label.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public XPTaskPaneHeaderLabel HeaderLabel
        {
            get { return this.titleLabel; }
        }

       public bool ShouldSerializeHeaderLabel()
        {
            if (this.HeaderLabel.Dock == DockStyle.Fill && this.HeaderLabel.TextAlign == ContentAlignment.MiddleCenter)
                return false;
            else
                return true;
        }
		/// <summary>
		///Metrocolor of xptaskpane
		/// </summary>
       internal Color xpmetroColor = Color.White;
       public Color MetroColor
       {
           get
           {
               return xpmetroColor;
           }
           set
           {
               if (xpmetroColor != value)
               {
                   xpmetroColor = value;
                   this.OnStyleChanged();
               }
           }
       }
        /// <summary>
        /// Gets the Header's left XPToolBar control.
        /// </summary>
        [Description("Reference to the Header's left XPToolBar control.")]
        [Category("Appearance")]

        // Not persisting state as the design-time does not have access to all the child items of
        // the toolbar. The XPToolBar.Items array will not serialized properly.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public XPToolBar HeaderLeftToolbar
        {
            get { return this.leftToolBar; }
        }

        /// <summary>
        /// Gets the Header's right XPToolBar control.
        /// </summary>
        [Description("Reference to the Header's right XPToolBar control.")]
        [Category("Appearance")]

        // Not persisting state as the design-time does not have access to all the child items of
        // the toolbar. The XPToolBar.Items array will not serialized properly.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public XPToolBar HeaderRightToolbar
        {
            get { return this.rightToolBar; }
        }

        /// <summary>
        /// Gets the Header's dropdown menu (<see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/>).
        /// </summary>
        [Description("Reference to the Header's dropdown menu (ParentBarItem).")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ParentBarItem HeaderMenuItem
        {
            get { return this.dropdownButton; }
        }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private BorderStyle m_borderStyle = BorderStyle.None;

		[Category("Appearance")]
		[DefaultValue(BorderStyle.None)]
		public BorderStyle BorderStyle
		{
			get { return m_borderStyle; }
			set 
			{
				if (m_borderStyle != value)
				{
					m_borderStyle = value;
					this.UpdateStyles();
				}
			}
		}
#endif

        public XPTaskPane()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new LicensedComponent(typeof(XPTaskPane));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            this.CardLayout.LayoutMode = CardLayoutMode.Fill;
            this.nextButton.Click += new EventHandler(this.NextButton_Click);
            this.backButton.Click += new EventHandler(this.BackButton_Click);
            this.closeButton.Click += new EventHandler(this.CloseButton_Click);
            HeaderSize = this.Header.Size;
            cd = new ControlDrawing();
            CTRLSIZE = this.Size;
            RefreshAppearance();

            // TODO: Add any initialization after the InitForm call
        }

        /// <summary>
        /// Gets or sets the style to be used for drawing the <see cref="XPTaskPane"/> control.
        /// </summary>
        [
        Description("Gets or sets a value indicating the style used for drawing the control."),
        Category("Appearance"),
        DefaultValue(VisualStyle.Default),
        TypeConverter(typeof(DefaultVisualStyleEnumFilter))
        ]
        public VisualStyle VisualStyle
        {
            get
            { 
                return m_vStyle; 
            }
            set
            {
                if (m_vStyle != value)
                {
                    m_vStyle = value;
                    this.OnStyleChanged();
                }
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
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                if (value == "Office2010Blue")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    if (this.VisualStyle == VisualStyle.Office2010)
                        Office2010ColorScheme = Office2010Theme.Managed;
                    else
                        Office2007ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Default")
                    VisualStyle = VisualStyle.Default;
                else if (value == "Office2003")
                    VisualStyle = VisualStyle.Office2003;
                else if (value == "OfficeXP")
                    VisualStyle = VisualStyle.OfficeXP;
                else if (value == "Office2007Outlook")
                    VisualStyle = VisualStyle.Office2007Outlook;
                else if (value == "Metro")
                    VisualStyle = VisualStyle.Metro;

            }
        }
        /// <summary>
        /// Gets or sets  office 2007 color scheme for the control.
        /// </summary>
        [
        Category("Appearance"),
        Description("Specifies office 2007 color scheme for the control."),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007ColorScheme
        {
            get
            {
                return m_colorScheme;
            }
            set
            {
                if (m_colorScheme != value)
                {
                    m_colorScheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Gets or sets  office 2010 color scheme for the control.
        /// </summary>
        [
        Category("Appearance"),
        Description("Specifies office 2010 color scheme for the control."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_color2010Scheme;
            }
            set
            {
                if (m_color2010Scheme != value)
                {
                    m_color2010Scheme = value;
                    this.OnStyleChanged();
                }
            }
        }
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        internal Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (m_office2010ColorTable == null) ?
                    Office2010Colors.Default : m_office2010ColorTable;

                return colorTable;
            }
        }
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        internal Office2007Colors Office2007ColorTable
        {
            get
            {
                Office2007Colors colorTable = (m_office2007ColorTable == null) ?
                    Office2007Colors.Default : m_office2007ColorTable;

                return colorTable;
            }
        }
        bool isScaling = false;

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
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
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
            this.ResumeLayout();
            this.Invalidate();
        }

        private void OnStyleChanged()
        {
            if (this.VisualStyle == VisualStyle.Office2007)
            {
                this.leftToolBar.Style = VisualStyle.Office2007;
                this.rightToolBar.Style = VisualStyle.Office2007;
                this.HeaderMenuItem.Style = VisualStyle.Office2007;
                m_office2007ColorTable = Office2007Colors.GetColorTable(m_colorScheme);
                this.leftToolBar.Office2007Theme = m_colorScheme;
                this.rightToolBar.Office2007Theme = m_colorScheme;
                this.HeaderMenuItem.Office2007Theme = m_colorScheme;

                foreach (XPTaskPage page in this.TaskPages)
                {
                    page.ThemesEnabled = false;
                    page.BorderColor = Office2007ColorTable.GroupBarBorderColor;
                }

                this.BackColor = Office2007ColorTable.GroupBarItemColorLight;

                this.Header.BackColor = Color.Transparent;
                this.HeaderLabel.BackColor = Color.Transparent;
                this.HeaderLabel.ForeColor = Office2007ColorTable.GroupBarItemTextColor;
            }
            else if (this.VisualStyle == VisualStyle.Office2010)
            {
                this.leftToolBar.Style = VisualStyle.Office2010;
                this.rightToolBar.Style = VisualStyle.Office2010;
                this.HeaderMenuItem.Style = VisualStyle.Office2010;
                m_office2010ColorTable = Office2010Colors.GetColorTable(m_color2010Scheme);
                this.leftToolBar.Office2010Theme = m_color2010Scheme;
                this.rightToolBar.Office2010Theme = m_color2010Scheme;
                //this.HeaderMenuItem.Office2010Theme = m_color2010Scheme;

                foreach (XPTaskPage page in this.TaskPages)
                {
                    page.ThemesEnabled = false;
                    page.BorderColor = Office2010ColorTable.GroupBarBorderColor;
                }

                this.BackColor = Office2010ColorTable.GroupBarItemColorLight;                
                this.Header.BackColor = Color.Transparent;
                this.HeaderLabel.BackColor = Color.Transparent;
                this.HeaderLabel.ForeColor = Office2010ColorTable.GroupBarHeaderTextColor;
            }
            else if (this.VisualStyle == VisualStyle.Metro )
            {
                leftToolBar.Style = VisualStyle.Metro ;
                rightToolBar.Style = VisualStyle.Metro;
                this.HeaderMenuItem.Style = VisualStyle.Metro;

                foreach (XPTaskPage page in this.TaskPages)
                {
                    page.ThemesEnabled = false;
                    page.BorderColor = MetroColor;
                    page.BackColor = MetroColor;
                }
                leftToolBar.BackColor = MetroColor;
                rightToolBar.BackColor = MetroColor;
                this.BackColor = MetroColor;
                this.Header.BackColor = Color.Transparent;
                this.HeaderLabel.BackColor = Color.Transparent;
            }
            else
            {
                leftToolBar.Style = VisualStyle.OfficeXP;
                rightToolBar.Style = VisualStyle.OfficeXP;
                this.HeaderMenuItem.Style = VisualStyle.OfficeXP;

                this.Header.BackColor = this.BackColor;

                foreach (XPTaskPage page in this.TaskPages)
                    page.BackColor = this.BackColor;

                this.HeaderLabel.BackColor = this.BackColor;
                this.HeaderLabel.ForeColor = Color.Black;

                foreach (XPTaskPage page in this.TaskPages)
                    page.ThemesEnabled = true;
            }

            if (this.IsHandleCreated)
            {
                // flags: SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED
                NativeMethods.SetWindowPos(this.Handle, IntPtr.Zero, 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0004 | 0x0020);
            }

            this.Invalidate();
        }

        IntPtr INonClientPaintingSupport.NonClientPaint(PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen)
        {
            int w = 0;
            Graphics g = e.Graphics;
            Rectangle bounds = displayRect;

            if (this.BorderStyle == BorderStyle.None)
            {
                if (this.VisualStyle == VisualStyle.Office2007)
                {
                    w = 2;

                    bounds.Inflate(-1, -1);
                    using (Pen pen = new Pen(Office2007ColorTable.XPTaskPaneBorderColor, 2))
                    {
                        g.DrawRectangle(pen, bounds);
                    }

                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Rectangle borderPath = bounds;
                    borderPath.Width--;
                    borderPath.Height--;

                    using (GraphicsPath fillPath = GetOffice2007PathFromBounds(borderPath))
                    {
                        using (Pen pen = new Pen(Office2007ColorTable.XPTaskPaneInternalBorderColor))
                        {
                            g.DrawPath(pen, fillPath);
                        }
                    }
                }
            }
            else
            {
                w = this.BorderStyle == BorderStyle.Fixed3D ? 2 : 1;
                cd.DrawBorder(g, bounds, this.BorderStyle, Border3DStyle.Sunken, ButtonBorderStyle.Solid, Color.Black);
            }

            return NativeMethods.CreateRectRgn(windowRectInScreen.Left + w, windowRectInScreen.Top + w, windowRectInScreen.Right - w, windowRectInScreen.Bottom - w);
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (leftToolBar != null)
                {
                    foreach (BarItem item in leftToolBar.Items)
                        item.Dispose();

                    Bar ltBar = leftToolBar.Bar;
                    leftToolBar.Dispose();
                    leftToolBar = null;
                    ltBar.Dispose();
                    ltBar = null;
                }

                if (rightToolBar != null)
                {
                    foreach (BarItem item in rightToolBar.Items)
                    {
                        if (item is ParentBarItem)
                            (item as ParentBarItem).BeforePopup -= new System.ComponentModel.CancelEventHandler(this.DropDown_BeforePopup);

                        item.Dispose();
                    }

                    Bar rtBar = rightToolBar.Bar;
                    rightToolBar.Dispose();
                    rightToolBar = null;
                    rtBar.Dispose();
                    rtBar = null;
                }
                if (this.m_verticalScrollUp != null)
                {
                    this.m_verticalScrollUp.Dispose();
                    this.m_verticalScrollUp = null;
                }
                if (this.m_verticalScrollDown != null)
                {
                    this.m_verticalScrollDown.Dispose();
                    this.m_verticalScrollDown = null;
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XPTaskPane));
            this.header = new Syncfusion.Windows.Forms.Tools.XPTaskPaneHeader();
            this.titleLabel = new Syncfusion.Windows.Forms.Tools.XPTaskPaneHeaderLabel();
            this.rightToolBar = new Syncfusion.Windows.Forms.Tools.XPMenus.XPToolBar();
            this.dropdownButton = new Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.closeButton = new Syncfusion.Windows.Forms.Tools.XPMenus.BarItem();
            this.leftToolBar = new Syncfusion.Windows.Forms.Tools.XPMenus.XPToolBar();
            this.backButton = new Syncfusion.Windows.Forms.Tools.XPMenus.BarItem();
            this.nextButton = new Syncfusion.Windows.Forms.Tools.XPMenus.BarItem();
            this.m_verticalScrollDown = new VerticalScrollBar(false);
            this.m_verticalScrollUp = new VerticalScrollBar(true);
            this.header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // header
            // 
            this.header.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.titleLabel,
																				 this.rightToolBar,
																				 this.leftToolBar});
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(192, 22);
            this.header.TabIndex = 2;
            this.header.BorderStyle = BorderStyle.None;
            this.header.Paint += new System.Windows.Forms.PaintEventHandler(this.Header_Paint);
            this.header.VisibleChanged += new EventHandler(Header_VisibleChanged);
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(45, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(102, 22);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Title";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rightToolBar
            // 
            this.rightToolBar.Bar = new Syncfusion.Windows.Forms.Tools.XPMenus.Bar(null, string.Empty, (((Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.AllowQuickCustomizing | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.IsMainMenu)
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.Visible)
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.DrawDragBorder
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.RotateWhenVertical), new Syncfusion.Windows.Forms.Tools.XPMenus.BarItemsDesignTime(new Syncfusion.Windows.Forms.Tools.XPMenus.BarItem[] {
																																																			this.dropdownButton,
																																																			this.closeButton }), new int[0]);
            this.rightToolBar.Location = new System.Drawing.Point(147, 0);
            this.rightToolBar.Name = "rightToolBar";
            this.rightToolBar.Size = new System.Drawing.Size(45, 22);
            this.rightToolBar.TabIndex = 1;
            this.rightToolBar.TabStop = false;
            this.rightToolBar.Text = "xpToolBar1";
            // 
            // parentBarItem1
            // 
            this.dropdownButton.CategoryIndex = -1;
            this.dropdownButton.ID = string.Empty;
            this.dropdownButton.ImageIndex = 2;
            this.dropdownButton.ImageList = this.imageList;
            this.dropdownButton.BeforePopup += new System.ComponentModel.CancelEventHandler(this.DropDown_BeforePopup);
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(12, 12);
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // closeButton
            // 
            this.closeButton.CategoryIndex = -1;
            this.closeButton.ID = string.Empty;
            this.closeButton.ImageIndex = 3;
            this.closeButton.ImageList = this.imageList;
            // 
            // leftToolBar
            // 
            this.leftToolBar.Bar = new Syncfusion.Windows.Forms.Tools.XPMenus.Bar(null, string.Empty, ((Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.AllowQuickCustomizing
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.Visible)
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.DrawDragBorder
                | Syncfusion.Windows.Forms.Tools.XPMenus.BarStyle.RotateWhenVertical), new Syncfusion.Windows.Forms.Tools.XPMenus.BarItemsDesignTime(new Syncfusion.Windows.Forms.Tools.XPMenus.BarItem[] {
																																																		  this.backButton,
																																																			this.nextButton}), new int[0]);
            this.rightToolBar.Location = new System.Drawing.Point(0, 0);
            this.leftToolBar.Name = "leftToolBar";
            this.leftToolBar.Size = new System.Drawing.Size(45, 22);
            this.leftToolBar.TabIndex = 2;
            this.leftToolBar.TabStop = false;
            this.leftToolBar.Text = "xpToolBar1";
            // 
            // backButton
            // 
            this.backButton.CategoryIndex = -1;
            this.backButton.ID = string.Empty;
            this.backButton.ImageIndex = 0;
            this.backButton.ImageList = this.imageList;
            // 
            // nextButton
            // 
            this.nextButton.CategoryIndex = -1;
            this.nextButton.ID = string.Empty;
            this.nextButton.ImageIndex = 1;
            this.nextButton.ImageList = this.imageList;
            //
            // m_verticalScrollUp
            //
            this.m_verticalScrollUp.Location = new Point(0, 0);
            this.m_verticalScrollUp.Name = "UpScrollBar";
            this.m_verticalScrollUp.Size = new Size(192, 10);
            this.m_verticalScrollUp.TabIndex = 4;
            this.m_verticalScrollUp.Visible = false;
            //
            // m_verticalScrollDown
            //
            this.m_verticalScrollDown.Dock = DockStyle.Bottom;
            this.m_verticalScrollDown.Name = "DownScrollBar";
            this.m_verticalScrollDown.Size = new Size(192, 10);
            this.m_verticalScrollDown.TabIndex = 5;
            this.m_verticalScrollDown.Visible = false;
            // 
            // XPTaskPane
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.header,
																		  this.m_verticalScrollDown,
																		  this.m_verticalScrollUp});
            this.Name = "XPTaskPane";
            this.Size = new System.Drawing.Size(192, 296);
            this.AfterPageSelect += new Wizard.WizardPageSelectEventHandler(this.XPTaskPane_AfterPageSelect);
            this.header.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Called by the Designer when the control is first created.
        /// </summary>
        protected override void OnEndInit()
        {
            base.OnEndInit();

            this.CardLayout.ContainerControl = PageContainer;

            if (!this.DesignMode && PageContainer != null)
            {
                RefreshMenuItems();
            }

            this.CardLayout.First();
            this.CardLayout.Next();
            this.CardLayout.Previous();

            this.Header.SendToBack();

            UpdateRTLRelatedProperties();
        }
        private void RefreshMenuItems()
        {
            dropdownButton.Items.Clear();
            for (int i = 0; i < PageContainer.Controls.Count; i++)
            {
                if (PageContainer.Controls[i] is XPTaskPage)
                {
                    XPTaskPage page = PageContainer.Controls[i] as XPTaskPage;
                    BarItem item = new BarItem(page.Title);
                    item.Tag = page.LayoutName;
                    item.Click += new EventHandler(ItemClick);
                    dropdownButton.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Gets rounded borders for drawing control in Office2007 style.
        /// </summary>
        /// <param name="bounds">Bounds of the control ot get path from.</param>
        /// <returns>Returns Graphics path</returns>
        public GraphicsPath GetOffice2007PathFromBounds(Rectangle bounds)
        {
            GraphicsPath fillPath = new GraphicsPath();
            Point[] pts;
            pts = new Point[] 
            {
                new Point(bounds.Left + 1, bounds.Top),
                               new Point(bounds.Right - 1, bounds.Top),
                               new Point(bounds.Right, bounds.Top + 1),
                               new Point(bounds.Right, bounds.Bottom),
                               new Point(bounds.Left, bounds.Bottom),
                               new Point(bounds.Left, bounds.Top + 1)
            };
            fillPath.AddLines(pts);

            return fillPath;
        }

        private void DropDown_BeforePopup(object sender, CancelEventArgs e)
        {
            if (this.SelectedPage != null)
            {
                foreach (BarItem item in this.dropdownButton.Items)
                {
                    if ((string)item.Tag == this.SelectedPage.LayoutName)
                        item.Checked = true;
                    else
                        item.Checked = false;
                }
            }
        }

        protected override void PageSettingsChanged()
        {
            RefreshMenuItems();
            RefreshAppearance();
        }

        private void ItemClick(object sender, EventArgs e)
        {
            BarItem item = (BarItem)sender;
            for (int i = 0; i < PageContainer.Controls.Count; i++)
            {
                if (PageContainer.Controls[i] is XPTaskPage)
                {
                    if (((XPTaskPage)PageContainer.Controls[i]).LayoutName == (string)item.Tag)
                    {
                        SelectedPage = (XPTaskPage)PageContainer.Controls[i];
                        return;
                    }
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.PreviousPage();
            UpdateVerticalScrolls();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            this.NextPage();
            UpdateVerticalScrolls();
        }

        protected override void RefreshAppearance()
        {
            if (SelectedPage == null) return;
            titleLabel.Text = SelectedPage.Title;

            UpdateRTLRelatedProperties();
            RefreshButtons();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(SystemColors.ControlDark))
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, header.Width - 1, header.Height - 1));
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
            UpdateVerticalScrolls();
            UpdateDockRelatedProperties();

            Invalidate(true);
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);

            UpdateDockRelatedProperties();
        }

        /// <summary>
        /// Performs VerticalScrolls layout logic
        /// </summary>
        private void UpdateVerticalScrolls()
        {
            if (this.TaskPanePageContainer != null && this.SelectedPage != null && this.VerticalScroll)
            {
                Rectangle rectTaskPane = this.Bounds;

                // Consider header height
                int nHeaderHeight = 0;

                if (this.Header != null && this.Header.Visible)
                {
                    nHeaderHeight = this.Header.Height;
                    rectTaskPane.Height -= nHeaderHeight;
                }

                if (rectTaskPane.Bottom >= this.SelectedPage.Bottom && this.Top >= this.SelectedPage.Top)
                {
                    Rectangle rectBounds = this.SelectedPage.Bounds;
                    this.SelectedPage.SetBounds(rectBounds.X, rectTaskPane.Y + rectTaskPane.Height - rectBounds.Height, rectBounds.Width, rectBounds.Height);
                }

                if (this.m_verticalScrollUp.Top > (this.SelectedPage.Top + nHeaderHeight))
                    m_verticalScrollUp.Visible = true;
                else if (m_verticalScrollUp.Visible && this.m_verticalScrollUp.Top <= (this.SelectedPage.Top + nHeaderHeight))
                    m_verticalScrollUp.Visible = false;

                if (rectTaskPane.Bottom < this.SelectedPage.Bottom)
                    m_verticalScrollDown.Visible = true;
                else if (m_verticalScrollDown.Visible && rectTaskPane.Bottom >= this.SelectedPage.Bottom)
                    m_verticalScrollDown.Visible = false;

                if (this.SelectedPage != null)
                {
                    m_verticalScrollDown.Width = m_verticalScrollUp.Width = this.SelectedPage.Bounds.Width;
                    m_verticalScrollDown.Refresh();
                    m_verticalScrollUp.Refresh();
                }

                if (m_verticalScrollDown.Top <= (this.Top + nHeaderHeight))
                    m_verticalScrollDown.Visible = false;
            }
        }

        private void Header_VisibleChanged(object sender, EventArgs e)
        {
            Panel header = sender as Panel;
            XPTaskPane parent = header.Parent as XPTaskPane;

            if (parent == null)
                throw new NullReferenceException("Header.Parent can not be null.");

            if (header.Visible)
                parent.UpScroll.Top = header.Bottom;
            else
                parent.UpScroll.Top = header.Top;

            UpdateVerticalScrolls();
        }

        private void XPTaskPane_AfterPageSelect(object sender, WizardPageSelectEventArgs e)
        {
            RefreshAppearance();
        }

        private void RefreshButtons()
        {
            if (SelectedPage == null) return;
            ArrayList list = new ArrayList(WizardPages);
            if (list.IndexOf(SelectedPage) == list.Count - 1)
            {
                nextButton.Enabled = false;
            }
            else
            {
                nextButton.Enabled = true;
            }
            if (list.IndexOf(SelectedPage) == 0)
            {
                backButton.Enabled = false;
            }
            else
            {
                backButton.Enabled = true;
            }
        }

        protected void UpdateRTLRelatedProperties()
        {
            UpdateRTLRelatedProperties(GetIsMirrored());
        }

        protected void UpdateRTLRelatedProperties(bool bIsMirrored)
        {
            if (null != titleLabel)
                titleLabel.Location = new Point(bIsMirrored ? this.ClientRectangle.Left + rightToolBar.Width : this.ClientRectangle.Left + leftToolBar.Width, titleLabel.Location.Y);

            if (null != leftToolBar)
            {
                leftToolBar.Location = new Point(bIsMirrored ? this.ClientRectangle.Right - leftToolBar.Width : 0, leftToolBar.Location.Y);
                UpdateToolBarItemsImageDrawing(leftToolBar, bIsMirrored);
            }

            if (null != rightToolBar)
            {
                rightToolBar.Location = new Point(bIsMirrored ? 0 : this.ClientRectangle.Right - rightToolBar.Width, rightToolBar.Location.Y);
                UpdateToolBarItemsImageDrawing(rightToolBar, bIsMirrored);
            }
        }

        protected void UpdateToolBarItemsImageDrawing(XPToolBar xptbToolBar, bool bIsMirrored)
        {
            int nItems = xptbToolBar.Items.Count;
            for (int nItem = 0; nItem < nItems; ++nItem)
            {
                BarItem biItem = xptbToolBar.Items[nItem];
                biItem.DrawImageMirrored = bIsMirrored;
            }
        }

        protected void UpdateDockRelatedProperties()
        {
            UpdateDockRelatedProperties(GetIsMirrored());
        }

        protected void UpdateDockRelatedProperties(bool bIsMirrored)
        {
            if (null != titleLabel)
            {
                titleLabel.Location = new Point(bIsMirrored ? this.ClientRectangle.Left + rightToolBar.Width : this.ClientRectangle.Left + leftToolBar.Width, titleLabel.Location.Y);
                titleLabel.Size = new Size(this.ClientRectangle.Width - leftToolBar.Width - rightToolBar.Width, this.Header.Height);
            }

            if (null != leftToolBar)
                leftToolBar.Location = new Point(bIsMirrored ? this.ClientRectangle.Right - leftToolBar.Width : 0, leftToolBar.Location.Y);

            if (null != rightToolBar)
                rightToolBar.Location = new Point(bIsMirrored ? 0 : this.ClientRectangle.Right - rightToolBar.Width, rightToolBar.Location.Y);
        }

        private IntPtr cachedRgn = IntPtr.Zero;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCCALCSIZE)
            {
                if (this.BorderStyle == BorderStyle.None &&
                    this.VisualStyle == VisualStyle.Office2007)
                {
                    NativeMethods.RECT rect = (NativeMethods.RECT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));

                    rect.left += 2;
                    rect.top += 2;
                    rect.right -= 2;
                    rect.bottom -= 2;

                    Marshal.StructureToPtr(rect, m.LParam, false);
                }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				if (this.BorderStyle == BorderStyle.FixedSingle)
				{
					NativeMethods.RECT rect = (NativeMethods.RECT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));

					rect.left += 1;
					rect.top += 1;
					rect.right -= 1;
					rect.bottom -= 1;

					Marshal.StructureToPtr(rect, m.LParam, false);
				}
				else if(this.BorderStyle == BorderStyle.Fixed3D)
				{
					NativeMethods.RECT rect = (NativeMethods.RECT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));

					rect.left += 2;
					rect.top += 2;
					rect.right -= 2;
					rect.bottom -= 2;

					Marshal.StructureToPtr(rect, m.LParam, false);
				}
#endif
            }

            if (m.Msg == NativeMethods.WM_NCPAINT)
            {
                if (this.cachedRgn != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(this.cachedRgn);
                    this.cachedRgn = IntPtr.Zero;
                }

                this.cachedRgn = DrawingUtils.NCPaintHelper(this, this, ref m);
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            UpdateRTLRelatedProperties();
        }

        /// <summary>
        /// Called by the designer when the user clicks on the next or back buttons.
        /// </summary>
        /// <param name="pt">Point for hit point</param>
        /// <returns>Returns true if designer click</returns>
        public bool DesignerClick(Point pt)
        {
            BarItem item = leftToolBar.HitTest(pt.X, pt.Y);
            if (nextButton == item)
            {
                this.NextPage();
                return true;
            }
            if (backButton == item)
            {
                this.PreviousPage();
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// The Panel based XPTaskPage.
    /// </summary>
    [Designer(typeof(XPTaskPageDesigner))]
    public class XPTaskPage : WizardPage
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            XPTaskPane pane = (this.Parent as WizardContainer).Parent as XPTaskPane;

            if (pane != null && pane.VisualStyle == VisualStyle.Office2007)
            {
                Rectangle bounds = this.ClientRectangle;
                using (SolidBrush fillBrush = new SolidBrush(pane.Office2007ColorTable.XPTaskPageBackColor))
                {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    using (GraphicsPath fillPath = pane.GetOffice2007PathFromBounds(bounds))
                    {
                        e.Graphics.FillPath(fillBrush, fillPath);
                    }
#else
				    e.Graphics.FillRectangle( fillBrush, bounds );
#endif
                }
            }
            else if (pane != null && pane.VisualStyle == VisualStyle.Office2010)
            {
                Rectangle bounds = this.ClientRectangle;
                using (SolidBrush fillBrush = new SolidBrush(pane.Office2010ColorTable.XPTaskPageBackColor))
                {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    using (GraphicsPath fillPath = pane.GetOffice2007PathFromBounds(bounds))
                    {
                        e.Graphics.FillPath(fillBrush, fillPath);
                    }
#else
				    e.Graphics.FillRectangle( fillBrush, bounds );
#endif
                }
            }
            else
                base.OnPaint(e);
        }
    }

    public class XPTaskPageDesigner : ScrollableControlDesigner
    {
        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.Locked;
            }
        }
    }

    [ToolboxItem(false)]
    public class XPTaskPaneHeader : Syncfusion.Windows.Forms.Tools.GradientPanel
    {
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                XPTaskPane pane = this.Parent as XPTaskPane;

                if (pane.VisualStyle == VisualStyle.Office2007 &&
                    value != Color.Transparent)
                    throw new ArgumentException("Cannot change BackColor, while VisualStyle isn't set to Default.");
                if (pane.VisualStyle == VisualStyle.Metro)
                    value = pane.MetroColor;
                base.BackColor = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            XPTaskPane pane = this.Parent as XPTaskPane;
            if (pane != null && pane.VisualStyle == VisualStyle.Office2007)
            {
                Color color1 = pane.Office2007ColorTable.GroupBarItemColorLight;
                Color color2 = pane.Office2007ColorTable.GroupBarItemColorDark;

                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

                using (LinearGradientBrush lgb = new LinearGradientBrush(this.ClientRectangle, color1, color2, LinearGradientMode.Vertical))
                {
                    lgb.Blend = blend;
                    e.Graphics.FillRectangle(lgb, this.ClientRectangle);
                }
            }
            else if (pane != null && pane.VisualStyle == VisualStyle.Office2010)
            {
                Color color1 = pane.Office2010ColorTable.GroupBarItemColorLight;
                Color color2 = pane.Office2010ColorTable.GroupBarItemColorDark;

                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

                using (LinearGradientBrush lgb = new LinearGradientBrush(this.ClientRectangle, color1, color2, LinearGradientMode.Vertical))
                {
                    lgb.Blend = blend;
                    e.Graphics.FillRectangle(lgb, this.ClientRectangle);
                }
            }
        }
    }

    [ToolboxItem(false)]
    public class XPTaskPaneHeaderLabel : Label
    {
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                XPTaskPane pane = (this.Parent as XPTaskPaneHeader).Parent as XPTaskPane;

                if (pane.VisualStyle == VisualStyle.Office2007 &&
                    value != Color.Transparent)
                    throw new ArgumentException("Cannot change BackColor, while VisualStyle isn't set to Default.");
                if (pane.VisualStyle == VisualStyle.Metro)
                    value = pane.MetroColor;
                base.BackColor = value;
            }
        }
    }

    /// <summary>
    /// The XPTaskPane Designer.
    /// </summary>
    public class XPTaskPaneDesigner : WizardDesigner
    {
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            XPTaskPaneHeader header = (this.Control as XPTaskPane).Header;

            foreach (Control control in header.Controls)
            {
                XPToolBar toolBar = control as XPToolBar;

                if (null != toolBar)
                {
                    toolBar.InDesignMode = true;
                }
            }
        }

        protected override void OnAddPage(object sender, EventArgs e)
        {
            IDesignerHost ih = GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (ih == null) return;

            XPTaskPage page = ih.CreateComponent(typeof(XPTaskPage)) as XPTaskPage;
            if (((XPTaskPane)Control).CardLayout.ContainerControl == null)
            {
                ((XPTaskPane)Control).CardLayout.ContainerControl = ((XPTaskPane)Control).PageContainer;
            }
           ((XPTaskPane)Control).AddPage(page);
        }

        protected override void OnMouseDragBegin(int x, int y)
        {
            if (((XPTaskPane)Control).DesignerClick(new Point(x, y))) return;
            base.OnMouseDragBegin(x, y);
        }

        protected override void InitializeNewComponent()
        {
            base.InitializeNewComponent();

            Wizard wizard = this.Wizard;
            WizardContainer pageContainer = wizard.PageContainer;
            if (((XPTaskPane)Control).CardLayout.ContainerControl == null)
            {
                ((XPTaskPane)Control).CardLayout.ContainerControl = pageContainer;
            }
            PropertyDescriptor pd = TypeDescriptor.GetProperties(pageContainer).Find("Locked", false);
            if (pd != null)
            {
                pd.SetValue(pageContainer, true);
            }

            pageContainer.Dock = DockStyle.Fill;
            pageContainer.BringToFront();
        }
    }

    /// <summary>
    /// VerticalScroll Bar
    /// </summary>
    [ToolboxItem(false)]
    public class VerticalScrollBar : Panel
    {
        #region initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the VerticalScrollBar class
        /// </summary>
        /// <param name="bUpScroll">Defines scroll direction</param>
        public VerticalScrollBar(bool bUpScroll)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint, true);

            UpdateDrawingObjects();
            using(SolidBrush brush =new SolidBrush(ButtonOffice2003Colors.MouseOverColor))
            this.MouseOverBrush = brush;

            m_bUpScroll = bUpScroll;

            this.VisibleChanged += new EventHandler(VerticalScrollBar_VisibleChanged);
            XPThemes.ThemeChanged += new EventHandler(XPThemes_ThemeChanged);
        }

        #endregion

        #region fields
        /// <summary>
        /// Timer used for scrolling.
        /// </summary>
        private Timer m_timerScroll;

        /// <summary>
        /// Defines scroll direction.
        /// </summary>
        private bool m_bUpScroll = false;

        /// <summary>
        /// Internal use flag - indicates whether mouse is currently over VerticalScrollBar.
        /// </summary>
        private bool m_mouseOver = false;

        /// <summary>
        /// Brush used to draw VerticalScrllBar when Mouse is over it.
        /// </summary>
        private Brush m_brushMouseOver = null;

        /// <summary>
        /// Brush used to draw VerticalScrollBar.
        /// </summary>
        private Brush m_brushDefault = null;

        /// <summary>
        /// Pen used to draw VerticalScrollbar border.
        /// </summary>
        private Pen m_penBorder = null;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets brush used to draw VerticalScrollbar when mouse is over it.
        /// </summary>
        private Brush MouseOverBrush
        {
            get
            {
                if (m_brushMouseOver == null)
                    m_brushMouseOver = Brushes.Orange;

                return m_brushMouseOver;
            }
            set
            {
                if (m_brushMouseOver != value)
                    m_brushMouseOver = value;
            }
        }

        /// <summary>
        /// Gets or sets brush used to draw VerticalScrollbar.
        /// </summary>
        private Brush DefaultBrush
        {
            get
            {
                if (m_brushDefault == null)
                    m_brushDefault = Brushes.Gray;

                return m_brushDefault;
            }
            set
            {
                if (m_brushDefault != value)
                    m_brushDefault = value;
            }
        }

        /// <summary>
        /// Gets or sets pen used to draw VerticalScrollbar border.
        /// </summary>
        private Pen BorderPen
        {
            get
            {
                if (m_penBorder == null)
                    m_penBorder = Pens.Black;

                return m_penBorder;
            }
            set
            {
                if (m_penBorder != value)
                    m_penBorder = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether mouse is over VerticalScrollbar.
        /// </summary>
        private bool MouseOver
        {
            get
            {
                return m_mouseOver;
            }
            set
            {
                m_mouseOver = value;
            }
        }

        /// <summary>
        /// Gets Scrolling speed.
        /// </summary>
        private int ScrollSpeed
        {
            get
            {
                XPTaskPane taskPane = this.Parent as XPTaskPane;
                return taskPane.ScrollSpeed;
            }
        }

        /// <summary>
        /// Gets or sets timer used for scrolling purporses.
        /// </summary>
        private Timer ScrollTimer
        {
            get
            {
                if (m_timerScroll == null)
                    m_timerScroll = new Timer();

                return m_timerScroll;
            }
            set
            {
                if (m_timerScroll != value)
                    m_timerScroll = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether scrolling direction is up or not.
        /// </summary>
        private bool UpScroll
        {
            get
            {
                return m_bUpScroll;
            }
        }
        #endregion

        #region overrides
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.VisibleChanged -= new EventHandler(VerticalScrollBar_VisibleChanged);
                XPThemes.ThemeChanged -= new EventHandler(XPThemes_ThemeChanged);

                this.Parent = null;

                if (m_timerScroll != null)
                {
                    m_timerScroll.Dispose();
                    m_timerScroll = null;
                }

                if (m_brushMouseOver != null)
                {
                    m_brushMouseOver.Dispose();
                    m_brushMouseOver = null;
                }

                if (m_brushDefault != null)
                {
                    m_brushDefault.Dispose();
                    m_brushDefault = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw internals
            if (this.MouseOver)
                e.Graphics.FillRectangle(this.MouseOverBrush, 1, 1, this.Width - 2, this.Height - 2);
            else
                e.Graphics.FillRectangle(this.DefaultBrush, 1, 1, this.Width - 2, this.Height - 2);

            // Draw border
            e.Graphics.DrawRectangle(this.BorderPen, 0, 0, this.Width - 1, this.Height - 1);

            // Draw direction arrow
            DrawUpDownButton(e.Graphics);

            base.OnPaint(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.MouseOver = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.MouseOver = false;
            StopTimer();

            base.OnMouseLeave(e);

            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            StartTimer();

            base.OnMouseHover(e);
        }

        #endregion

        #region event handlers
        private void VerticalScrollBar_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Created)
            {
                if (!this.Visible)
                {
                    StopTimer();
                    ((XPTaskPane)this.Parent).CardLayout.RestoreChildPosition -= new CardLayout.RestoreChildPositionEventHandler(RestoreChildPosition_eventHandler);
                }
                else
                {
                    this.BringToFront();
                    ((XPTaskPane)this.Parent).CardLayout.RestoreChildPosition += new CardLayout.RestoreChildPositionEventHandler(RestoreChildPosition_eventHandler);
                }
            }
        }

        private void RestoreChildPosition_eventHandler(object sender, CancelEventArgs evtArgs)
        {
            evtArgs.Cancel = true;
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            XPTaskPane taskPane = this.Parent as XPTaskPane;
            int nAmount = 1;

            if (!this.UpScroll)
                nAmount = -nAmount;

            if (taskPane.SelectedPage != null)
            {
                Rectangle rectBounds = taskPane.SelectedPage.Bounds;
                Rectangle rectTaskPane = taskPane.Bounds;

                if (taskPane.Header != null && taskPane.Header.Visible)
                    rectTaskPane.Height -= taskPane.Header.Height;

                if (this.UpScroll && ((rectBounds.Y + nAmount) > 0))
                    this.Visible = false;
                else if (!this.UpScroll && ((rectBounds.Bottom + nAmount) < rectTaskPane.Bottom))
                    this.Visible = false;
                else
                {
                    if (this.UpScroll && !taskPane.DownScroll.Visible && (rectBounds.Bottom > rectTaskPane.Bottom))
                        taskPane.DownScroll.Visible = true;

                    if (!this.UpScroll && !taskPane.UpScroll.Visible && (rectBounds.Top < rectTaskPane.Top))
                        taskPane.UpScroll.Visible = true;

                    // Move Scrolling Control
                    taskPane.SelectedPage.SetBounds(rectBounds.X, rectBounds.Y + nAmount, rectBounds.Width, rectBounds.Height);
                }
            }
        }
        private void XPThemes_ThemeChanged(object sender, EventArgs e)
        {
            UpdateDrawingObjects();
            Invalidate();
        }

        #endregion

        #region helper methods
        /// <summary>
        /// Updates brushes and pens used for drawing routine.
        /// </summary>
        private void UpdateDrawingObjects()
        {
            if (XPThemes.IsDefaultBlueThemeOn)
            {
                this.DefaultBrush = new SolidBrush(WindowsXPColors.DefaultBlueBottomColor);
                this.BorderPen = new Pen(WindowsXPColors.DefaultBlueBorderColor);
            }
            else if (XPThemes.IsOliveGreenThemeOn)
            {
                this.DefaultBrush = new SolidBrush(WindowsXPColors.OliveGreenBottomColor);
                this.BorderPen = new Pen(WindowsXPColors.OliveGreenBorderColor);
            }
            else if (XPThemes.IsSilverThemeOn)
            {
                this.DefaultBrush = new SolidBrush(WindowsXPColors.SilverBottomColor);
                this.BorderPen = new Pen(WindowsXPColors.SilverBorderColor);
            }
            else if (!XPThemes.IsThemeActive)
            {
                this.DefaultBrush = new SolidBrush(this.BackColor);
                this.BorderPen = Pens.Black;
            }
        }

        /// <summary>
        /// Scrollbar direction arrow drawing routine.
        /// </summary>
        /// <param name="g">Graphics to draw on.</param>
        public void DrawUpDownButton(Graphics g)
        {
            float x, y;

            float cx = 7; // Width of the arrow
            float cy = 4; // Height of the arrow

            float horizontalMargin = (this.Width - cx) / 2;
            float verticalMargin = (this.Height - cy) / 2;

            x = horizontalMargin;
            y = verticalMargin;

            if (this.UpScroll)
            {    
               g.FillRectangle(Brushes.Black, x + 3, y, 1, 1);
                g.FillRectangle(Brushes.Black, x + 2, y + 1, 3, 1);
                g.FillRectangle(Brushes.Black, x + 1, y + 2, 5, 1);
                g.FillRectangle(Brushes.Black, x, y + 3, 7, 1);
            }
            else
            {
                g.FillRectangle(Brushes.Black, x, y, 7, 1);
                g.FillRectangle(Brushes.Black, x + 1, y + 1, 5, 1);
                g.FillRectangle(Brushes.Black, x + 2, y + 2, 3, 1);
                g.FillRectangle(Brushes.Black, x + 3, y + 3, 1, 1);
            }
        }

        /// <summary>
        /// Starts scroll timer.
        /// </summary>
        private void StartTimer()
        {
            this.ScrollTimer.Interval = this.ScrollSpeed;
            this.ScrollTimer.Tick += new EventHandler(ScrollTimer_Tick);
            this.ScrollTimer.Start();
        }

        /// <summary>
        /// Stops scroll timer.
        /// </summary>
        private void StopTimer()
        {
            if (this.ScrollTimer != null && this.ScrollTimer.Enabled)
            {
                this.ScrollTimer.Stop();
                this.ScrollTimer.Tick -= new EventHandler(ScrollTimer_Tick);
            }
        }

        #endregion
    }
}