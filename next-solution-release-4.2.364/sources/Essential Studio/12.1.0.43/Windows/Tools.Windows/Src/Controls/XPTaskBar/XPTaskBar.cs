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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    public enum XPTaskBarStyle
    {
        /// <summary>
        /// Classic appearance.
        /// </summary>
        Default,

        /// <summary>
        /// Office 2007-like appearance.
        /// </summary>
        Office2007,
        /// <summary>
        /// Office 2010-like appearance.
        /// </summary>
        Office2010,
        /// <summary>
        /// Metro theme appearance.
        /// </summary>
        Metro,
    }

    /// <summary>
    /// Represents a Windows XP like task menu panel.
    /// </summary>
    /// <seealso cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>
    /// <remarks>
    /// <para>An XPTaskBar is a panel that can display a set of command items 
    /// (<see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarItem"/>) or frequently
    /// used features (like "Search", "Advanced Search", etc) in panels, that
    /// can be classified, contained and displayed within one or more command boxes 
    /// (<see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>).
    /// The command boxes themselves can be expanded and collapsed by the user, to show or hide its set of
    /// command items and optionally a child panel.</para>
    /// <para>This task bar panel can be used to provide quick and easy shortcuts to commonly
    /// used commands and features.</para>
    /// <para>To enable themes support in XP turn on the <see cref="ThemesEnabled"/> property.</para>
    /// <para>This is the look-and-feel used in the Windows XP Control Panel Window.</para>
    /// </remarks>
    /// <example>
    /// The following example creates a <see cref="XPTaskBar"/> with 2 <see cref="XPTaskBarBox"/>s and few <see cref="XPTaskBarItem"/>s.
    /// <para>Use the Syncfusion.Windows.Forms.Tools namespace for this code.</para>
    /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\CS\Form1.cs" name="Initializing XPTaskBar" lang="C#"><code lang="C#">
    /// XPTaskBarBox taskBarBox1 = new XPTaskBarBox();
    /// taskBarBox1.HeaderBackColor = Color.Blue;
    /// taskBarBox1.ImageList = this.imageList1;
    /// taskBarBox1.Text = "Header Text";
    /// taskBarBox1.ItemBackColor = Color.WhiteSmoke;
    /// taskBarBox1.Items.Add(new XPTaskBarItem("Item 1", Color.Black, 0, "Tag1"));
    /// taskBarBox1.Items.Add(new XPTaskBarItem("Item 2", Color.Black, 0, "Tag2"));
    /// taskBarBox1.ItemClick += new XPTaskBarItemClickHandler(xpTaskBarBox_ItemClick);
    /// XPTaskBarBox taskBarBox2 = new XPTaskBarBox();
    /// taskBarBox2.HeaderBackColor = Color.Blue;
    /// taskBarBox2.ImageList = this.imageList1;
    /// taskBarBox2.Text = "Another Header Text";
    /// taskBarBox2.ItemBackColor = Color.WhiteSmoke;
    /// taskBarBox2.Items.Add(new XPTaskBarItem("Item 3", Color.Black, 0, "Tag3"));
    /// taskBarBox2.ItemClick += new XPTaskBarItemClickHandler(this.xpTaskBarBox_ItemClick);
    /// this.xpTaskBar1.Controls.Add(taskBarBox1);
    /// this.xpTaskBar1.Controls.Add(taskBarBox2);</code></coderef>
    /// <coderef file="\Tools\Samples\GroupBar Package\XPTaskBar\VB\Form1.vb" name="Initializing XPTaskBar" lang="VB"><code lang="VB">
    ///            Dim taskBarBox1 As XPTaskBarBox
    ///            taskBarBox1 = New XPTaskBarBox()
    ///            taskBarBox1.HeaderBackColor = Color.Blue
    ///            taskBarBox1.ImageList = Me.imageList1
    ///            taskBarBox1.Text = "Header Text"
    ///            taskBarBox1.ItemBackColor = Color.WhiteSmoke
    ///            taskBarBox1.Items.Add(New XPTaskBarItem("Item 1", Color.Black, 0, "Tag1"))
    ///            taskBarBox1.Items.Add(New XPTaskBarItem("Item 2", Color.Black, 0, "Tag2"))
    ///            AddHandler taskBarBox1.ItemClick, New XPTaskBarItemClickHandler(AddressOf taskMenuBox_ItemClick)
    ///            Dim taskBarBox2 As XPTaskBarBox
    ///            taskBarBox2 = New XPTaskBarBox()
    ///            taskBarBox2.HeaderBackColor = Color.Blue
    ///            taskBarBox2.ImageList = Me.imageList1
    ///            taskBarBox2.Text = "Another Header Text"
    ///            taskBarBox2.ItemBackColor = Color.WhiteSmoke
    ///            taskBarBox2.Items.Add(New XPTaskBarItem("Item 3", Color.Black, 0, "Tag3"))
    ///            AddHandler taskBarBox2.ItemClick, New XPTaskBarItemClickHandler(AddressOf taskMenuBox_ItemClick)
    ///            Me.xpTaskBar1.Controls.Add(taskBarBox1)
    ///            Me.xpTaskBar1.Controls.Add(taskBarBox2)</code></coderef>
    /// </example>
    [
    Designer(
        typeof(Syncfusion.Windows.Forms.Tools.Design.XPTaskBarDesigner),
        typeof(System.ComponentModel.Design.IDesigner)),
    DefaultChildType(typeof(XPTaskBarBox)),
    ToolboxBitmap(typeof(XPTaskBar), "ToolboxIcons.XPTaskBar.bmp"),
    Description("Represents a Windows XP like task menu panel.")
    ]
    public class XPTaskBar :
        Panel,
        ISupportInitialize,
        INonClientPaintingSupport,
        IVisualStyle 
    {
        /// <summary>
        /// Offset for width of XPTaskBarBox by <see cref="BorderStyle.FixedSingle"/> border style.
        /// </summary>
        private const int DEF_BORDERSTYLE_FIXEDSINGLE_OFFSET = 2;

        /// <summary>
        /// Offset for width of XPTaskBarBox by <see cref="BorderStyle.Fixed3D"/> border style.
        /// </summary>
        private const int DEF_BORDERSTYLE_FIXED3D_OFFSET = 4;

        private FlowLayout flowLayout;
        private bool needLayout = false;
        private bool themesEnabled = false;
        private ImageList headerImageList = null;
        private bool layingOut = false;
        private bool verticalLayout = true;
        private int colWidthOnHorizontalAlignment = 100;
        private Hashtable htTaskBarStates = null;
        private bool autoPersistStates = true;
        private static bool s_isDevEnv = Application.ExecutablePath.ToLower().IndexOf("devenv.exe") >= 0;
        private bool autoSize = false;

        /// <summary>
        /// Indicates XPTaskBar has been already initialized.
        /// </summary>
        /// <remark>internal usage flag</remark>
        private bool m_bInitializing = false;

        /// <summary>
        /// Indicates whether layout is in progress.
        /// </summary>
        /// <remark>internal usage flag</remark>
        private bool m_bLayoutInProgress = false;

        /// <summary>
        /// Specifies an advanced appearance this control.
        /// </summary>
        private XPTaskBarStyle m_style = XPTaskBarStyle.Default;

        /// <summary>
        /// Specifies office 2007 color scheme.
        /// </summary>
        private Office2007Theme m_colorScheme = Office2007Theme.Blue;

        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2007Colors m_office2007ColorTable = null;
        /// <summary>
        /// Specifies office 2010 color scheme.
        /// </summary>
        private Office2010Theme m_color2010Scheme = Office2010Theme.Blue;

        /// <summary>
        /// Color table for Office2007 visual style.
        /// </summary>
        private Office2010Colors m_office2010ColorTable = null;
        private ControlDrawing cd = new ControlDrawing();
        private int m_verticalPadding = 0;

        /// <summary>
        /// Indicates, whether WM_SETFOCUSED was received by child.
        /// </summary>
        private bool m_bChildSetFocus = false;

        /// <summary>
        /// Minimum size for XPTaskBar.
        /// </summary>
        private Size m_minimumSize = Size.Empty;

        /// <summary>
        /// specifies the Alignment of Taskbar box items.
        /// </summary>
        private ItemsAlignment m_itemsAlignment;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Occurs when the <see cref="MinimumSize"/> property changes.
        /// </summary>
        [Description("Occurs when the MinimumSize property changes.")]
        public event EventHandler MinimumSizeChanged;

        private void RaiseMinimumSizeChanged()
        {
            if (MinimumSizeChanged != null)
            {
                MinimumSizeChanged(this, EventArgs.Empty);
            }
        }

        #region enum
        /// <summary>
        /// Specifies the Taskbar box's items alignment
        /// </summary>
        public enum ItemsAlignment
        {
            /// <summary>
            /// Represents Vertical
            /// </summary>
            Vertical,

            /// <summary>
            /// Represents Horizontal
            /// </summary>
            Horizontal
        }

        #endregion
        #region For Touch

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

        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion

        /// <summary>
        /// Gets or sets the Taskbar box's items alignment.
        /// </summary>
        [Browsable(true),
        Description("Gets or sets the Taskbar box's items alignment.")]
        [DefaultValue(ItemsAlignment.Vertical)]
        public ItemsAlignment BoxItemsAlignment
        {
            get
            {
                return this.m_itemsAlignment;
            }
            set
            {
                if (this.m_itemsAlignment != value)
                {
                    this.m_itemsAlignment = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Raises the MinimumSizeChanged event.
        /// </summary>
        protected virtual void OnMinimumSizeChanged()
        {
            if (this.MinimumSize.Width > this.Size.Width || this.MinimumSize.Height > this.Size.Height)
            {
                Size minSize = Size.Empty;

                minSize.Width = (this.MinimumSize.Width > this.Size.Width) ?
                    this.MinimumSize.Width : this.Size.Width;

                minSize.Height = (this.MinimumSize.Height > this.Size.Height) ?
                    this.MinimumSize.Height : this.Size.Height;

                this.Size = minSize;
            }

            RaiseMinimumSizeChanged();
        }
        
        /// <summary>
        /// Initializes a new instance of the XPTaskBar class.
        /// </summary>
        public XPTaskBar()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(XPTaskBar));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            this.AutoSize = false;

            flowLayout = new FlowLayout(this, FlowLayoutMode.Vertical, FlowAlignment.Near, 0, 0);
            flowLayout.AutoLayout = false;
            flowLayout.VGapChanged += new ValueChangedEventHandler(FlowLayout_VGapChanged);

            this.htTaskBarStates = new Hashtable();

            base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            CTRLSIZE = this.Size;
        }

       public void FlowLayout_VGapChanged(object sender, ValueChangedEventArgs e)
        {
           if(!this.DesignMode)
               m_verticalPadding = (int)e.newValue;
        }

        /// <summary>
        /// Gets or sets an advanced appearance for the xpTaskBar.
        /// </summary>
        [Description("Gets or sets an advanced appearance for the xpTaskBar.")]
        [Category("Appearance")]
        [DefaultValue(XPTaskBarStyle.Default)]
        public XPTaskBarStyle Style
        {
            get
            {
                return m_style;
            }
            set
            {
                if (m_style != value)
                {
                    m_style = value;
                    OnStyleChanged();
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
                    Style = XPTaskBarStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    Style = XPTaskBarStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    Style = XPTaskBarStyle.Office2007;
                    Office2007ColorScheme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    Style = XPTaskBarStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    Style = XPTaskBarStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    Style = XPTaskBarStyle.Office2010;
                    Office2010ColorScheme = Office2010Theme.Black;
                }
                else if (value == "Metro")
                {
                    Style = XPTaskBarStyle.Metro;
                }
                else if (value == "Managed")
                {
                    Office2007ColorScheme = Office2007Theme.Managed;
                }
                else if (value == "Default")
                    Style = XPTaskBarStyle.Default;

            }
        }
        /// <summary>
        /// Gets or sets office 2007 color scheme.
        /// </summary>
        [Description("Gets or sets office 2007 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2007Theme.Blue)]
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
                    OnStyleChanged();
                }
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

        /// <summary>
        /// Gets or sets office 2010 color scheme.
        /// </summary>
        [Description("Gets or sets office 2010 color scheme.")]
        [Category("Appearance")]
        [DefaultValue(Office2010Theme.Blue)]
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
                    OnStyleChanged();
                }
            }
        }

        /// <summary>
        /// Gets color table for Office2010 visual style.
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
        private BorderStyle VSBorderStyle
        {
            get
            {
                return  this.BorderStyle;
            }
        }

        /// <summary>
        /// Gets or sets the vertical spacing between the layout task bar.
        /// </summary>
        [Description("Gets or sets the vertical spacing between the layout task bar.")]
        [Category("Layout")]
        public int VerticalPadding
        {
            get
            {
                return m_verticalPadding;
            }
            set
            {
                if (value >= 0)
                {
                    m_verticalPadding = value;

                    if (!this.themesEnabled && (this.Style == XPTaskBarStyle.Office2007||this.Style == XPTaskBarStyle.Office2010))
                    {
                        flowLayout.VGap = m_verticalPadding + 1;
                    }
                    else
                    {
                        flowLayout.VGap = m_verticalPadding;
                    }
                }
            }
        }

        private bool ShouldSerializeVerticalPadding()
        {
            return m_verticalPadding != 0;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        private bool ShouldSerializeAutoSize()
        {
            return autoSize != false;
        }

        private void ResetVerticalPadding()
        {
            m_verticalPadding = 0;
            if (!this.themesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
            {
                flowLayout.VGap = m_verticalPadding + 1;
            }
            else
            {
                flowLayout.VGap = m_verticalPadding;
            }
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
                base.BackColor = value;
            }
        }

        private bool ShouldSerializeBackColor()
        {
            bool b = this.BackColor != SystemColors.Control;
            if (!this.ThemesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
            {
                b = this.BackColor != Color.White;
            }
            return b;
        }

        /// <summary>
        /// Resets the back color to it's default value. (overridden method)
        /// </summary>
        public override void ResetBackColor()
        {
            if (!this.ThemesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
            {
                this.BackColor = Color.White;
            }
            else
            {
                this.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal spacing between the layout task bar.
        /// </summary>
        [DefaultValue(0)]
        [Description("Gets or sets the horizontal spacing between the layout task bar.")]
        [Category("Layout")]
        public int HorizontalPadding
        {
            get
            {
                return flowLayout.HGap;
            }
            set
            {
                if (flowLayout.HGap != value && value >= 0)
                {
                    flowLayout.HGap = value;
                }
            }
        }      

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="XPTaskBarBox"/>s should be aligned vertically
        /// or horizontally in this control.
        /// </summary>
        /// <value>True to align vertically, one below the other; false to align horizontally.
        /// Default is true.</value>
        /// <remarks>
        /// When you set this property to false, you should typically also set the 
        /// <see cref="ColWidthOnHorizontalAlignment"/> property.
        /// </remarks>
        [DefaultValue(true), Category("Appearance"),
        Description("Specifies whether the child boxes should be aligned vertically or horizontally.")]
        public bool VerticalLayout
        {
            get
            { 
                return this.verticalLayout;
            }
            set
            {
                if (this.verticalLayout != value)
                {
                    this.verticalLayout = value;
                    LayoutInternal();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically persists the collapsed state of the child boxes.
        /// </summary>
        /// <value>True to persist the expanded state every time the control is disposed; false otherwise. 
        /// Default is true.</value>
        /// <remarks>
        /// <p>When this property is true, the expanded states of the child task bar boxes are
        /// cached as the users expands/collapses them and when this control is disposed, the cached
        /// state is persisted in the Isolated Storage.</p>
        /// <p>
        /// When the application loads again and when child task bar boxes are added to this control,
        /// the saved state is reapplied on the task bar boxes.
        /// </p>
        /// <p>
        /// State is saved in the Isolated Storage of the system, scoped by the current user identity.
        /// </p>
        /// <p>You can also optionally, explicitly control the persistent store and/or
        /// the time of persistence using explicit calls to <see cref="LoadBoxExpandedStates()"/>
        /// and <see cref="SaveBoxExpandedStates()"/> methods.</p>
        /// </remarks>
        [DefaultValue(true), Description("Automatically persists the collapsed state of the child boxes."),
        Category("Behavior")]
        public bool AutoPersistStates
        {
            get 
            {
                return this.autoPersistStates; 
            }
            set
            {
                this.autoPersistStates = value;
            }
        }

        /// <summary>
        /// Gets or sets the width for each column when in horizontal alignment mode.
        /// </summary>
        /// <value>
        /// Default is 100.
        /// </value>
        /// <remarks>
        /// This property will be used when the <see cref="XPTaskBarBox"/>s are aligned
        /// horizontally, by setting the <see cref="VerticalLayout"/> property to false.
        /// </remarks>
        [DefaultValue(100),
        Description("Specifies the width for each column when in horizontal alignment mode."),
        SRCategory(SR.CategoryAppearance),
        ]
        public int ColWidthOnHorizontalAlignment
        {
            get
            { 
                return this.colWidthOnHorizontalAlignment; 
            }
            set
            {
                if (this.colWidthOnHorizontalAlignment != value)
                {
                    this.colWidthOnHorizontalAlignment = value;
                    LayoutInternal();
                }
            }
        }

        /// <summary>
        /// Gets or sets minimum size for XPTaskBar.
        /// </summary>
        [
        DefaultValue(typeof(Size), "0;0"),
        Description("The minimum size the XPTaskBar can be resized to."),
        Category("Layout")
        ]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public Size MinimumSize
#else
        public new Size MinimumSize
#endif
        {
            get
            {
                return m_minimumSize;
            }
            set
            {
                if (value != m_minimumSize)
                {
                    m_minimumSize.Width = (value.Width < 0) ?
                        0 : value.Width;

                    m_minimumSize.Height = (value.Height < 0) ?
                        0 : value.Height;

                    OnMinimumSizeChanged();
                }
            }
        }

        /// <summary>
        /// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
        /// </summary>
        /// <param name="disposing">bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.AutoPersistStates && !this.DesignMode && !s_isDevEnv)
                {
                    this.SaveBoxExpandedStates();
                }

                foreach (XPTaskBarBox box in this.Controls)
                {
                    box.CollapsedStateChanged -= new EventHandler(Box_CollapsedChanged);
                    box.LayoutNeed -= new EventHandler(Box_LayoutNeed);
                }

                this.htTaskBarStates.Clear();
            }

            // Make sure to relelase these event handlers
            if (this.headerImageList != null)
            {
                this.headerImageList.RecreateHandle -= new EventHandler(ImageList_Recreated);
                this.headerImageList = null;
            }
            if (flowLayout != null)
            {
                this.flowLayout.VGapChanged -= new ValueChangedEventHandler(FlowLayout_VGapChanged);
                this.flowLayout.Dispose();
                this.flowLayout = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is automatically resized to display its entire contents.
        /// </summary>
        [Description("Gets or sets a value indicating whether the control is automatically resized to display its entire contents."), Browsable(true),DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get
            {
                return this.autoSize;
            }
            set
            {
                if(this.autoSize!=value)
                {
                    base.AutoSize = value;
                    this.autoSize = value;

                    LayoutInternal();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return (Size)(new Size(200, 100));
            }
        }

        /// <summary>
        /// Gets or sets the text associated with the control.
        /// </summary>
        [Localizable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        /// <summary>
        /// Gets or sets the height and width of the control.
        /// </summary>
        [Localizable(true)]
        public new Size Size
        {
            get
            {
                return base.Size; 
            }
            set 
            {
                base.Size = value; 
            }
        }
        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the control relative to the upper-left corner of its container.
        /// </summary>
        [Localizable(true)]
        public new Point Location
        {
            get
            {
                return base.Location; 
            }
            set
            {
                base.Location = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether need layout
        /// Internal method, not to be used directly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Indicates whether this control's content needs to be laid out.
        /// </para>
        /// </remarks>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public bool NeedLayout
        {
            get
            {
                bool result = needLayout;
                if (!result)
                {
                    foreach (XPTaskBarBox taskMenuBox in this.Controls)
                    {
                        result |= taskMenuBox.NeedLayout;
                        if (result)
                            break;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the ImageList that will be used to draw the header images in the individual <see cref="XPTaskBarBox"/>.
        /// </summary>
        /// <value>An <see cref="System.Windows.Forms.ImageList"/> instance. Default is null.</value>
        /// <remarks>
        /// This ImageList will automatically be used by the child <b>XPTaskBarBox</b> instances.
        /// You can override this behavior by setting a different ImageList in the <see cref="XPTaskBarBox.ImageList"/> property.
        /// </remarks>
        [
        DefaultValue(null),
        SRCategory(SR.CategoryAppearance),
        Description("Specifies the ImageList that will be used to draw the header images in the child boxes")
        ]
        public ImageList HeaderImageList
        {
            get
            { 
                return this.headerImageList; 
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

                    LayoutInternal();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether XP Themes (visual styles) should be used for this control when
        /// available.
        /// </summary>
        /// <remarks>
        /// <para>This property will also affect the <see cref="Syncfusion.Windows.Forms.Tools.XPTaskBarBox"/>
        /// children's themes usage.</para>
        /// </remarks>
        [
        DefaultValue(false),
        Category(@"Appearance"),
        Description("Specifies whether XP Themes (visual styles) should be used for this control when available.")
        ]
        public bool ThemesEnabled
        {
            get
            {
                return this.themesEnabled;
            }
            set
            {
                if (this.themesEnabled != value)
                {
                    this.themesEnabled = value;
                    if (this.themesEnabled)
                    {
                        this.BorderStyle = BorderStyle.None;
                        if (m_verticalPadding == 0)
                        {
                            flowLayout.VGap = 0;
                        }
                    }
                    else if ((this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
                    {
                        this.BorderStyle = BorderStyle.FixedSingle;
                        if (flowLayout.VGap == 0)
                        {
                            flowLayout.VGap = 1;
                        }
                    }

                    if (!m_bInitializing)
                    {
                        this.ResetBackColor();
                        foreach (XPTaskBarBox box in this.Controls)
                        {
                            box.ResetPADY();
                            box.ResetItemBackColor();
                            box.ResetHeaderBackColor();
                        }
                    }

                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Lays out its children.
        /// </summary>
        /// <param name="g">The Graphics object based on which to determine the sizes and positions.</param>
        /// <remarks>
        /// <para>Note that the XPTaskBar control follows a different layout pattern from the usual Windows Forms Control.
        /// When requested a layout by the default Windows Forms Layout event, this control will only mark its
        /// child positions as dirty and recalculate its child positions when a subsequent Paint event occurs,
        /// with a call to this Layout method. This technique is followed to reduce flicker.</para>
        /// </remarks>
        protected new virtual void Layout(Graphics g)
        {
            this.LayoutInternal();
        }

        private void LayoutInternal()
        {
            if (!m_bInitializing)
            {
                if (layingOut)
                    return;

                layingOut = true;

                int nChildrenHeight = 0;
                Rectangle rectLayout = Rectangle.Empty;

                this.SuspendLayout();

                nChildrenHeight = GetTaskChildrenHeight();
                LayoutTaskBarBoxes(nChildrenHeight);

                rectLayout = this.ClientRectangle;

                rectLayout.Y = this.DisplayRectangle.Y;
                rectLayout.Height = this.DisplayRectangle.Height;
                
                if (this.VerticalLayout)
                {
                    rectLayout.Height = Int32.MaxValue;
                }
                else
                {
                    rectLayout.Height = this.Height;
                    rectLayout.Width = Int32.MaxValue;
                }

                this.FlowLayout.CustomLayoutBounds = rectLayout;

                this.flowLayout.LayoutContainer();

                this.ResumeLayout(true);
                Invalidate(true);
                layingOut = false;
            }
        }

        /// <summary>
        /// Calculates taskBarBoxesHeight.
        /// </summary>
        /// <returns>Children heght</returns>
        /// <remark>used only with vertical layout</remark>
        private int GetTaskChildrenHeight()
        {
            // Calculate height of all taskBoxes.
            int nHeight = 0;
            foreach (XPTaskBarBox taskMenuBox in this.Controls)
            {
                taskMenuBox.Layout(null);

                nHeight += taskMenuBox.GetPreferredHeight() + this.VerticalPadding;
            }

            // include paddings
            nHeight += this.DockPadding.Top + this.DockPadding.Bottom;

            return nHeight;
        }

        /// <summary>
        /// This method gets called every time before the XPTaskBarBoxes get laid out
        /// by the <see cref="FlowLayout"/> component.
        /// </summary>
        /// <remarks>
        /// <para>You shouldn't have to override this method typically. Do so when you need a 
        /// different layout logic than the default one. When overriding this method, make sure to call the base class. 
        /// The base class will set up the margins and bounds for the layout.</para>
        /// <para>The FlowLayout component used internally can be accessed using the FlowLayout property.</para>
        /// </remarks>
        protected virtual void OnUpdateFlowLayoutBeforeLayout()
        {
            this.HorizontalPadding = this.DockPadding.Left;
            this.VerticalPadding = this.DockPadding.Top;
            Rectangle custBounds = this.Bounds;
            custBounds = this.ClientRectangle;

            if (this.VerticalLayout)
            {
                custBounds.Y = this.DisplayRectangle.Y;

                // To simiulate infinite height
                custBounds.Height = Int32.MaxValue;

                // this.flowLayout.CustomLayoutBounds = custBounds;
            }
            else
            {
                custBounds.X = this.DisplayRectangle.X;

                // this.flowLayout.CustomLayoutBounds = custBounds;
            }

            this.flowLayout.HorzNearMargin = this.DockPadding.Left;
            this.flowLayout.HorzFarMargin = this.DockPadding.Right;
            this.flowLayout.TopMargin = this.DockPadding.Top;
            this.flowLayout.BottomMargin = this.DockPadding.Bottom;
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Tools.FlowLayout"/> component used
        /// to manage the layout of the <see cref="XPTaskBarBox"/>s inside this control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public FlowLayout FlowLayout
        {
            get
            {
                return this.flowLayout;
            }
        }

        /// <summary>
        /// Internal method used to layout the <see cref="XPTaskBarBox"/>s.
        /// </summary>
        /// <param name="nTaskBarBoxesHeght">TaskBar Box height</param>
        protected virtual void LayoutTaskBarBoxes(int nTaskBarBoxesHeght)
        {
            // Update tasbBoxes size.
            foreach (XPTaskBarBox taskMenuBox in this.Controls)
            {
                int nClientWidth = this.Width - this.DockPadding.Left
                    - this.DockPadding.Right - GetBorderStyleOffset();

                int width = ((nTaskBarBoxesHeght > this.Height) && this.AutoScroll) ? nClientWidth - SystemInformation.HorizontalScrollBarHeight : nClientWidth;

                if (!this.VerticalLayout)
                    width = this.ColWidthOnHorizontalAlignment;

                Size newSize = new Size(width, taskMenuBox.GetPreferredHeight());
                newSize = this.OptimizeSize(newSize);
                if (newSize != taskMenuBox.Size)
                    taskMenuBox.Size = newSize;
            }
        }

        /// <summary>
        /// Gets offset for width of XPTaskBarBox by different <see cref="BorderStyle"/>.
        /// </summary>
        /// <returns>Returns BOrder Style offset</returns>
        private int GetBorderStyleOffset()
        {
            int offset = 0;

            switch (BorderStyle)
            {
                case BorderStyle.Fixed3D:
                    {
                        offset = DEF_BORDERSTYLE_FIXED3D_OFFSET;
                        break;
                    }
                case BorderStyle.FixedSingle:
                    {
                        offset = DEF_BORDERSTYLE_FIXEDSINGLE_OFFSET;
                        break;
                    }
            }

            return offset;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (!m_bLayoutInProgress)
            {
                this.SuspendLayout();
                m_bLayoutInProgress = true;
                LayoutInternal();
                m_bLayoutInProgress = false;
                this.ResumeLayout(true);
            }
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.MinimumSize.Width > 0 && width < this.MinimumSize.Width)
            {
                width = this.MinimumSize.Width;
            }

            if (this.MinimumSize.Height > 0 && height < this.MinimumSize.Height)
            {
                height = this.MinimumSize.Height;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (!m_bLayoutInProgress)
            {
                m_bLayoutInProgress = true;
                if (this.flowLayout != null)
                {
                    this.flowLayout.HorzNearMargin = this.DockPadding.Left;
                    this.flowLayout.HorzFarMargin = this.DockPadding.Right;
                    LayoutInternal();
                }
                base.OnLayout(levent);
                m_bLayoutInProgress = false;
            }
        }
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color metrocolor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		///Gets or Sets the Metrocolor
		/// </summary>
        public Color MetroColor
        {
            get
            {
                return metrocolor;
            }
            set
            {
                if (metrocolor!=value )
                {
                    metrocolor = value;
                    OnStyleChanged();
                }
               
            }
        }
        /// <summary>
        ///Gets or sets the border color. 
        /// </summary>
        private Color borderClr = Color.Black;
        public Color BorderColor
        {
            get { return borderClr; }
            set { borderClr = value; }
        }
        protected override /*ContainerControl*/ CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= ~0x200 /*WS_EX_CLIENTEDGE*/;
                cp.Style &= ~0x800000 /*WS_BORDER*/;
                if (!(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
                {
                    if (this.VSBorderStyle == BorderStyle.FixedSingle)
                    {
                        cp.Style |= 0x800000;
                    }
                    if (this.VSBorderStyle == BorderStyle.Fixed3D)
                    {
                        cp.ExStyle |= 0x200;
                    }
                }
                cp.Style = cp.Style | (int)ControlStyles.AllPaintingInWmPaint | (int)ControlStyles.UserPaint | (int)WhidbeyCompatibleControlStyles.DoubleBuffer;
                return cp;
            }
        }

        private void InvalidateWindow()
        {
            int redrawFlags = NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE;
            NativeMethodsHelper.RedrawWindow(this.Handle, redrawFlags);
        }

        private IntPtr cachedRgn = IntPtr.Zero;
        IntPtr INonClientPaintingSupport.NonClientPaint(PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = displayRect;
            int w = 2;
            if (this.VSBorderStyle == BorderStyle.FixedSingle)
            {
                w = 1;
            }

            // The borders as 4 rectangles
            Rectangle[] clipRects = new Rectangle[] { new Rectangle( bounds.Location, new Size( w, bounds.Height ) ), new Rectangle( bounds.Location, new Size( bounds.Width, w ) ), new Rectangle( bounds.Width - w, bounds.Y, w, bounds.Height ), new Rectangle( bounds.X, bounds.Height - w, bounds.Width, w ) };

            // Fill the border-rectangles with the bg brush, since some of the 
            // 3d border types are only 1 pixel wide.
            for (int i = 0; i < 4; i++)
            {
                using (SolidBrush br = new SolidBrush(this.BackColor))
                    g.FillRectangle(br, clipRects[i]);
            }

            Color borderColor = BorderColor;
            if (this.Style == XPTaskBarStyle.Office2007)
            {
                borderColor = this.Office2007ColorTable.XPTaskBarBorderColor;
            }
            else if (this.Style == XPTaskBarStyle.Office2010)
            {
                borderColor = this.Office2010ColorTable.XPTaskBarBorderColor;
            }
            if (this.Style != XPTaskBarStyle.Metro)
            {
                cd.DrawBorder(g, bounds, this.VSBorderStyle, Border3DStyle.Sunken, ButtonBorderStyle.Solid, borderColor);
            }

            // return a region excluding where you just drew.
            return NativeMethods.CreateRectRgn(windowRectInScreen.Left + w, windowRectInScreen.Top + w, windowRectInScreen.Right - w, windowRectInScreen.Bottom - w);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.ThemesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
            {
                using (SolidBrush br = new SolidBrush(Color.White))
                {
                    e.Graphics.FillRectangle(br, this.ClientRectangle);
                }

                if (this.Style == XPTaskBarStyle.Office2007)
                {
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        XPTaskBarBox box = this.Controls[i] as XPTaskBarBox;
                        if (box != null)
                        {
                            Rectangle rect = box.Bounds;
                            using (Pen pen = new Pen(this.Office2007ColorTable.XPTaskBarBoxHeaderUpperLineColor))
                            {
                                e.Graphics.DrawLine(pen, rect.Left, rect.Top - 1, rect.Right, rect.Top - 1);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.Controls.Count; i++)
                    {
                        XPTaskBarBox box = this.Controls[i] as XPTaskBarBox;
                        if (box != null)
                        {
                            Rectangle rect = box.Bounds;
                            using (Pen pen = new Pen(this.Office2010ColorTable.XPTaskBarBoxHeaderUpperLineColor))
                            {
                                e.Graphics.DrawLine(pen, rect.Left, rect.Top - 1, rect.Right, rect.Top - 1);
                            }
                        }
                    }
                }
            }
            
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_NCPAINT && this.VSBorderStyle != BorderStyle.None && !(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled))
            {
                this.cachedRgn = DrawingUtils.NCPaintHelper(this, this, ref m);
            }

            base.WndProc(ref m);
        }

        private void OnStyleChanged()
        {
            if (!this.ThemesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010 || this.Style == XPTaskBarStyle.Metro))
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                if (this.flowLayout.VGap == 0)
                {
                    this.flowLayout.VGap = 1;
                }
            }
            else
            {
                if (m_verticalPadding == 0)
                {
                    this.flowLayout.VGap = 0;
                }
            }

            if(this.Style == XPTaskBarStyle.Office2007)
                m_office2007ColorTable = Office2007Colors.GetColorTable(m_colorScheme);
            else if( this.Style == XPTaskBarStyle.Office2010)
                m_office2010ColorTable = Office2010Colors.GetColorTable(m_color2010Scheme);

            if (!m_bInitializing && !this.ThemesEnabled)
            {
                this.ResetBackColor();
                foreach (XPTaskBarBox box in this.Controls)
                {
                    box.ResetPADY();
                    box.ResetItemBackColor();
                    box.ResetHeaderBackColor();
                }
            }

            this.InvalidateWindow();
            this.Invalidate(true);
        }

        private Size OptimizeSize(Size size)
        {
            if (size.Width < 0)
                size.Width = 0;
            if (size.Height < 0)
                size.Height = 0;
            return size;
        }
        private void ImageList_Recreated(object sender, EventArgs e)
        {
            LayoutInternal();
        }

        internal bool GetIsMirrored()
        {
            return RightToLeft.Yes == RightToLeft;
        }

        #region PERSIST_STATE
        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (this.AutoPersistStates && this.htTaskBarStates != null)
            {
                if (e.Control is XPTaskBarBox)
                {
                    XPTaskBarBox box = e.Control as XPTaskBarBox;
                    if (this.htTaskBarStates.Contains(box))
                    {
                        box.Collapsed = (bool)this.htTaskBarStates[box];
                    }
                    else
                    {
                        this.htTaskBarStates[box.Text] = box.Collapsed;
                    }
                    box.CollapsedStateChanged += new EventHandler(this.Box_CollapsedChanged);
                }
            }

            if (e.Control is XPTaskBarBox)
            {
                XPTaskBarBox box = e.Control as XPTaskBarBox;
                box.LayoutNeed += new EventHandler(Box_LayoutNeed);
            }

            base.OnControlAdded(e);
        }
        
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (e.Control is XPTaskBarBox)
            {
                XPTaskBarBox box = e.Control as XPTaskBarBox;
                box.CollapsedStateChanged -= new EventHandler(Box_CollapsedChanged);
                box.LayoutNeed -= new EventHandler(Box_LayoutNeed);
            }

            base.OnControlRemoved(e);
        }
        private void Box_CollapsedChanged(object sender, EventArgs e)
        {
            XPTaskBarBox box = sender as XPTaskBarBox;
            this.htTaskBarStates[box.Text] = box.Collapsed;
        }

        /// <summary>
        /// Overloaded. Saves the expanded state of the child task bar boxes in the Isolated Storage.
        /// </summary>
        /// <remarks>
        /// <p>Call this method whenever you want to save the current expanded state of 
        /// the task bar boxes in the Isolated Storage.</p>
        /// <p>Note that you do not have to call this method to persist state. You could 
        /// use the <see cref="AutoPersistStates"/> property instead.</p>
        /// </remarks>
        public void SaveBoxExpandedStates()
        {
            this.SaveBoxExpandedStates(AppStateSerializer.GetSingleton());
        }

        /// <summary>
        /// Saves the expanded state of the child task bar boxes 
        /// into the specified serializer.
        /// </summary>
        /// <param name="serializer">A <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
        /// <remarks>
        /// <p>Call this method whenever you want to save the current expanded state of 
        /// the task bar boxes in a custom location (instead of the default Isolated Storage).</p>
        /// <p>Note that you do not have to call this method to persist state. You could 
        /// use the <see cref="AutoPersistStates"/> property instead.</p>
        /// </remarks>
        public virtual void SaveBoxExpandedStates(AppStateSerializer serializer)
        {
            if (serializer != null)
            {
                try
                {
                    serializer.SerializeObject(GetPersistenceKey(), this.htTaskBarStates);
                }
                catch (Exception e)
                {
                    Debug.Assert(false, "SaveBoxExpandedStates Failed.", e.Message);
                }
            }
        }
        private string GetPersistenceKey()
        {
            return "XPTaskBarBox Collapsed States: (" +
                   this.Name + " )";
        }

        /// <summary>
        /// Overloaded. Loads the expanded state of the child task bar boxes from the Isolated Storage.
        /// </summary>
        /// <remarks>
        /// <p>Call this method whenever you want to load the saved expanded states of 
        /// the task bar boxes from the Isolated Storage.</p>
        /// <p>Note that you do not have to call this method to persist state. You could 
        /// use the <see cref="AutoPersistStates"/> property instead.</p>
        /// </remarks>
        /// <returns>Returns Load expanded states</returns>
        public bool LoadBoxExpandedStates()
        {
            return this.LoadBoxExpandedStates(AppStateSerializer.GetSingleton());
        }

        /// <summary>
        /// Loads the expanded state of the child task bar boxes from the specified AppStateSerializer.
        /// </summary>
        /// <param name="serializer">A <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
        /// <returns>True if loaded successfully; false otherwise.</returns>
        /// <remarks>
        /// <p>Call this method whenever you want to load the saved expanded states of 
        /// the task bar boxes from a specific location.</p>
        /// <p>Note that you do not have to call this method to persist state. You could 
        /// use the <see cref="AutoPersistStates"/> property instead.</p>
        /// </remarks>
        public virtual bool LoadBoxExpandedStates(AppStateSerializer serializer)
        {
            if (serializer != null)
            {
                try
                {
                    this.htTaskBarStates =
                        serializer.DeserializeObject(GetPersistenceKey()) as Hashtable;

                    this.ApplyDeserializedStates(this.htTaskBarStates);
                    if (this.htTaskBarStates == null)
                        this.htTaskBarStates = new Hashtable();
                }
                catch (Exception e)
                {
                    Debug.Assert(false, "LoadBoxExpandedStates Failed.", e.Message);
                    return false;
                }
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Applies the deserialized expanded state information on the current task bar boxes.
        /// </summary>
        /// <param name="htStates">A <see cref="[REDACTED]"/> instance.</param>
        /// <remarks>
        /// This method will be called to apply the persisted expanded-state of the
        /// task bar boxes. The hash table should contain text-boolean pairs indicating
        /// the text of the task bar box and it's collapsed state (true for collapsed, false for expanded).
        /// </remarks>
        protected virtual void ApplyDeserializedStates(Hashtable htStates)
        {
            if (htStates == null)
                return;

            foreach (XPTaskBarBox box in this.Controls)
            {
                if (box != null && htStates.Contains(box.Text))
                {
                    box.Collapsed = (bool)htStates[box.Text];
                }
            }
        }
        void ISupportInitialize.BeginInit()
        {
            m_bInitializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            if(this.Dock==DockStyle.None)
            base.AutoSize = false;
            m_bInitializing = false;

            if (!this.DesignMode
               && !s_isDevEnv)
            {
                this.LoadBoxExpandedStates();
            }

            if (!this.themesEnabled && (this.Style == XPTaskBarStyle.Office2007 ||this.Style == XPTaskBarStyle.Office2010) && this.flowLayout != null && this.flowLayout.VGap == 0)
            {
                this.flowLayout.VGap = 1;
            }

            if (!this.ThemesEnabled && (this.Style == XPTaskBarStyle.Office2007 || this.Style == XPTaskBarStyle.Office2010))
            {
                this.ResetBackColor();
                foreach (XPTaskBarBox box in this.Controls)
                {
                    box.ResetPADY();
                    box.ResetItemBackColor();
                    box.ResetHeaderBackColor();
                }
            }
        }
        #endregion

        private void Box_LayoutNeed(object sender, EventArgs e)
        {
            if (!m_bLayoutInProgress)
            {
                this.m_bLayoutInProgress = true;
                this.SuspendLayout();

                LayoutInternal();

                this.m_bLayoutInProgress = false;
                this.ResumeLayout(true);
            }
        }
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
        protected override Point ScrollToControl(Control activeControl)
        {
            Point pt;

            if (this.AutoScroll)
            {
                if (!this.IsChildSetFocus)
                {
                    pt = base.ScrollToControl(activeControl);
                }
                else
                {
                    this.IsChildSetFocus = false;
                    pt = this.AutoScrollPosition;
                }
            }
            else
            {
                pt = base.ScrollToControl(activeControl);
            }

            return pt;
        }
#endif
        internal bool IsChildSetFocus
        {
            get { return m_bChildSetFocus; }
            set { m_bChildSetFocus = value; }
        }
    }
}
