#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents Office 2007 Style ColorPicker.
    /// </summary>
    [Designer(typeof(ColorPickerUIDesigner), typeof(System.ComponentModel.Design.IDesigner))]
    [Serializable]
    [
    DesignerSerializer(typeof(ColorUIAdvDesignSerializer), typeof(CodeDomSerializer)),
    //Designer(typeof(ColorPickerUIAdvDesigner)),
    ToolboxBitmap(typeof(ColorPickerUIAdv), "ToolboxIcons.ColorPickerUIAdv.bmp"),
    Description("Represents Office 2007 Style ColorPicker.")
    ]
    public class ColorPickerUIAdv :
        Control,IVisualStyle 
    {
        #region Fields
        internal const int DEF_ITEMSCOUNT = 10;
        internal const int DEF_SUBITEMSCOUNT = DEF_ITEMSCOUNT / 2;
        internal const int DEF_BASECOLORSOFFSET = 6;
        private const int DEF_CONTROLWIDTH = 172;
        private ColorUIAdvRenderer m_renderer = null;
        private ColorUIAdvMetroRenderer  mrenderer = null;
        private Office2007Theme m_officeTheme = Office2007Theme.Blue;
        private Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        private Syncfusion.Windows.Forms.ButtonAdv m_btnMoreColors;
        private Syncfusion.Windows.Forms.ButtonAdv m_btnState;
        private bool m_bUseOffice2007Style = false;
        private BorderStyle m_borderStyle = BorderStyle.None;
        private ColorUIAdvGroup m_themeGroup = null;
        private ColorUIAdvGroup m_standartGroup = null;
        private ColorUIAdvGroup m_recentGroup = null;
        private ColorUIAdvGroup m_activeGroup = null;
        private int m_xSpacing = 4;
        private int m_borderOffset = 3;
        private int m_ySpacing = 0;
        private Size m_itemSize = new Size(13, 13);
        private Point m_focus = new Point(0, 0);
        private ColorItem m_ciHighLightedItem = null;
        private ColorItem m_ciSelectedItem = null;
        private ColorUIAdvGroupsCollection m_groups = null;
        private ColorUIAdvGroupsCollection m_customGroups = null;
        private ContentAlignment m_textAlign = ContentAlignment.MiddleLeft;
        private int m_buttonsHeight = 23;
        private bool m_bInSetBoundsCore = false;
        private Color m_automaticColor = Color.Black;
        private int m_selectedTabIndex = 0;
        internal Hashtable ChangedItems;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CISIZE = default(Size);


        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerUIAdv"/> class.
        /// </summary>
        public ColorPickerUIAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ColorPickerUIAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            InitializeComponent();

            Init();
        }

        #endregion

        #region Properties
        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CISIZE;
            }
            set
            {
                CISIZE = value;
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
            this.ColorItemSize = new Size((int)(CISIZE.Width * scaleFactor), (int)(CISIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();

        }
        /// <summary>
        /// Gets the StateButton
        /// </summary>
        [Browsable(false)]
        public ButtonAdv StateButton
        {
            get { return m_btnState; }
        }

        /// <summary>
        /// Gets the MoreColorButton 
        /// </summary>
        [Browsable(false)]
        public ButtonAdv MoreColorsButton
        {
            get { return m_btnMoreColors; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorUIAdvGroup ActiveGroup
        {
            get 
            {
                return m_activeGroup; 
            }

            set
            {
                m_activeGroup = value;
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
                    Office2007Theme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007Theme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007Theme = Office2007Theme.Black;
                else if (value == "Office2010Blue")
                    Office2010Theme = Office2010Theme.Blue;
                else if (value == "Office2010Silver")
                    Office2010Theme = Office2010Theme.Silver;
                else if (value == "Office2010Black")
                    Office2010Theme = Office2010Theme.Black;
                else if (value == "Managed")
                {
                    if (this.Style == visualstyle.Office2010)
                        Office2010Theme = Office2010Theme.Managed;
                    else
                        Office2007Theme = Office2007Theme.Managed;
                }
            }
        }
        /// <summary>
        /// Gets or sets Office2007 theme for control drawing.
        /// Default is blue color theme.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Office2007Theme.Blue)]
        [Description("Gets or sets Office2007 theme for control drawing. Default is blue color theme.")]
        public Office2007Theme Office2007Theme
        {
            get 
            {
                return this.m_officeTheme; 
            }

            set
            {
                if (this.m_officeTheme != value)
                {
                    this.m_officeTheme = value;

                    foreach (ButtonAdv button in this.Controls)
                        button.Office2007ColorScheme = value;

                    this.SetRenderer();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets Office2010 theme for control drawing.
        /// Default is blue color theme.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(Office2010Theme.Blue)]
        [Description("Gets or sets Office2010 theme for control drawing. Default is blue color theme.")]
        public Office2010Theme Office2010Theme
        {
            get
            {
                return this.m_office2010Theme;
            }

            set
            {
                if (this.m_office2010Theme != value)
                {
                    this.m_office2010Theme = value;

                    foreach (ButtonAdv button in this.Controls)
                        button.Office2010ColorScheme = value;

                    this.SetRenderer();
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets horizontal spacing between ColorItems.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(4)]
        [Description("Gets or sets horizontal spacing between ColorItems.")]
        public int HorizontalItemsSpacing
        {
            get 
            {
                return m_xSpacing; 
            }

            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be nonNegative");

                if (m_xSpacing != value)
                {
                    m_xSpacing = value;

                    this.UpdateControl();
                }
            }
        }

        /// <summary>
        /// Gets or sets border offset for ColorItems.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(3)]
        [Description("Gets or sets border offset for ColorItems.")]
        public int BorderOffset
        {
            get 
            {
                return m_borderOffset; 
            }

            set
            {
                if (m_borderOffset != value)
                {
                    m_borderOffset = value;

                    foreach (ColorUIAdvGroup group in this.Groups)
                        group.UpdateGroupSize();
                }
            }
        }

        /// <summary>
        /// Gets or sets vertical spacing between ColorItems.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(0)]
        [Description("Gets or sets vertical spacing between ColorItems.")]
        public int VerticalItemsSpacing
        {
            get 
            {
                return m_ySpacing; 
            }

            set
            {
                if (value < 0)
                    throw new ArgumentException("Value must be nonNegative");

                if (m_ySpacing != value)
                {
                    m_ySpacing = value;

                    this.UpdateControl();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use Office2007 style for control drawing.
        /// Default value is true.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Indicates whether to use Office2007 style for control drawing. Default value is true.")]
        public bool UseOffice2007Style
        {
            get 
            {
                return m_bUseOffice2007Style; 
            }
            set
            {
              
               
                    if (m_bUseOffice2007Style != value)
                    {
                        m_bUseOffice2007Style = value;

                        foreach (Control control in this.Controls)
                        {
                            if (control is ButtonAdv)
                            {
                                ButtonAdv button = control as ButtonAdv;

                                button.UseVisualStyle = value;
                            }
                        }

                        this.SetRenderer();
                        this.Invalidate();
                    }
                
            }
        }
        public enum visualstyle
        {
            Default,
            Office2007,
            Office2010,
            Metro
        }
		/// <summary>
		/// Gets the metrocolor.
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#16A5DC");
		/// <summary>
		/// Gets or sets the metrocolor.
		/// </summary>
        public Color MetroColor
        {
            get { return metroColor; }
            set { metroColor = value; }
        }

        private visualstyle colorPickerStyle = visualstyle.Default;
        public visualstyle Style
        {
            get { return colorPickerStyle; }
            set
            {
                colorPickerStyle = value;
              
                
                    if (colorPickerStyle == visualstyle.Metro)
                    {
                        if (!UseOffice2007Style)
                        {
                            mrenderer = new ColorUIAdvMetroRenderer(this);
                            m_borderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                            foreach (Control control in this.Controls)
                            {
                                if (control is ButtonAdv)
                                {
                                    (control as ButtonAdv).Height = m_buttonsHeight;
                                    (control as ButtonAdv).UseVisualStyle = true;
                                    (control as ButtonAdv).BackColor = this.metroColor;
                                    (control as ButtonAdv).Appearance = ButtonAppearance.Metro;
                                    (control as ButtonAdv).BackColor = this.metroColor;
                                }
                            }
                            this.SetRenderer();
                            this.Invalidate();
                        }
                        else if(colorPickerStyle == visualstyle.Metro)
                        {
                            colorPickerStyle = visualstyle.Office2007;
                            m_renderer = new ColorUIAdvRenderer(this);
                        }
                    }
                    else if (colorPickerStyle == visualstyle.Office2007)
                    {
                        m_renderer = new ColorUIAdvOffice2007Renderer(this);
                        foreach (Control control in this.Controls)
                        {
                            if (control is ButtonAdv)
                            {
                                ButtonAdv button = control as ButtonAdv;
                                button.UseVisualStyle = true;
                                button.Appearance = ButtonAppearance.Office2007;
                                this.Invalidate();
                            }
                        }
                        colorPickerStyle = visualstyle.Office2007;
                        this.SetRenderer();
                        this.Invalidate();
                    }
                    else if (colorPickerStyle == visualstyle.Office2010)
                    {
                        m_renderer = new ColorUIAdvOffice2007Renderer(this);
                        foreach (Control control in this.Controls)
                        {
                            if (control is ButtonAdv)
                            {
                                ButtonAdv button = control as ButtonAdv;
                                button.UseVisualStyle = true;
                                button.Appearance = ButtonAppearance.Office2010;
                                this.Invalidate();
                            }
                        }
                        colorPickerStyle = visualstyle.Office2010;
                        this.SetRenderer();
                        this.Invalidate();
                    }
                    else
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control is ButtonAdv)
                            {
                                (control as ButtonAdv).UseVisualStyle = false;
                                (control as ButtonAdv).BackColor = DefaultBackColor;
                            }
                        }
                    this.SetRenderer();
                    this.Invalidate();
                }


            }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets size for ColorItems.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets size for ColorItems.")]
        public Size ColorItemSize
        {
            get 
            {
                return m_itemSize; 
            }

            set
            {
                if (m_itemSize != value)
                {
                    m_itemSize = value;

                    this.UpdateControl();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border style of the control.
        /// </summary>
        /// <value>A <see cref="System.Windows.Forms.BorderStyle"/> value. The default is BorderStyle.None.</value>
        [
        Category("Appearance"),
        DefaultValue(BorderStyle.None),
        Description("Specifies the border style of the control.")
        ]
        public BorderStyle BorderStyle
        {
            get
            {
                return m_borderStyle;
            }
            set
            {
                if (m_borderStyle != value)
                {
                    m_borderStyle = value;

                    UpdateStyles();

                    this.UpdateControl();
                }
            }
        }

        /// <summary>
        /// Gets or sets groups collection of the control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal ColorUIAdvGroupsCollection Groups
        {
            get 
            {
                return m_groups; 
            }
            
            set
            {
                if (m_groups != value)
                    m_groups = value;
            }
        }

        /// <summary>
        /// Gets or Sets Custom Groups collection of the control.
        /// </summary>
        [Category("Groups")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets or Sets Custom Groups collection of the control.")]
        public ColorUIAdvGroupsCollection CustomGroups
        {
            get { return m_customGroups; }
        }

        public bool ShouldSerializeCustomGroups()
        {
            if (m_customGroups.Count != 0)
            {
                foreach (ColorUIAdvGroup group in this.CustomGroups)
                    group.ShouldSerialize = true;
            }

            return true;
        }

        /// <summary>
        /// Gets the ThemeGroup
        /// </summary>
        [Category("Groups")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Represent group of theme colors.")]
        public ColorUIAdvGroup ThemeGroup
        {
            get { return m_themeGroup; }
        }

        [Category("Groups")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Represent group of standard colors.")]
        public ColorUIAdvGroup StandardGroup
        {
            get { return m_standartGroup; }
        }

        [Category("Groups")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Represent group of recent colors.")]
        public ColorUIAdvGroup RecentGroup
        {
            get { return m_recentGroup; }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// Default value is MiddleLeft alignment.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the text alignment. Default value is MiddleLeft alignment.")]
        public ContentAlignment TextAlign
        {
            get 
            {
                return m_textAlign; 
            }

            set
            {
                if (m_textAlign != value)
                {
                    m_textAlign = value;

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the buttons.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(23)]
        [Description("Gets or sets the height of the buttons.")]
        public int ButtonsHeight
        {
            get
            {
                return m_buttonsHeight; 
            }

            set
            {
                if (m_buttonsHeight != value)
                {
                    m_buttonsHeight = value;

                    foreach (Control control in this.Controls)
                    {
                        if (control is ButtonAdv)
                            (control as ButtonAdv).Height = m_buttonsHeight;
                    }

                    this.UpdateControl();
                }
            }
        }

        [Browsable(false)]
        public ColorItem HighLightedItem
        {
            get { return m_ciHighLightedItem; }
        }

        [Browsable(false)]
        public ColorItem SelectedItem
        {
            get
            {
                return m_ciSelectedItem;
            }
        }

        /// <summary>
        /// Gets or sets the the selected color.
        /// </summary>
        /// <remarks>If color isn't found in any group it's added to recent colors group.</remarks>
        [Description("Gets or sets the selected color.")]
        public Color SelectedColor
        {
            get
            {
                Color selectedColor = Color.Empty;

                if (this.SelectedItem != null)
                {
                    selectedColor = this.SelectedItem.Color;
                }

                return selectedColor;
            }
            set
            {
                if (this.SelectedColor != value)
                {
                    if (value != Color.Empty)
                    {
                        SelectColor(value);
                    }
                    else
                    {
                        this.SelectedItem.State = ColorItemState.Normal;
                    }
                }
            }
        }

        private void SelectColor(Color colorToSelect)
        {
            ColorItem colorItem = null;

            foreach (ColorUIAdvGroup group in this.Groups)
            {
                foreach (ColorItem item in group.Items)
                {
                    if (item.Color == colorToSelect ||
                        (colorToSelect.IsNamedColor && item.Color.Name == colorToSelect.Name) || 
                        (colorToSelect.IsKnownColor && item.Color.ToKnownColor() == colorToSelect.ToKnownColor()) ||
                        item.Color.ToArgb() == colorToSelect.ToArgb())
                    {
                        colorItem = item;
                        break;
                    }
                    else
                    {
                        GroupColorItem groupColorItem = item as GroupColorItem;

                        if (groupColorItem != null)
                        {
                            foreach (ColorItem subItem in groupColorItem.SubItems)
                            {
                                if (subItem.Color == colorToSelect ||
                                    (colorToSelect.IsNamedColor && subItem.Color.Name == colorToSelect.Name) ||
                                    (colorToSelect.IsKnownColor && subItem.Color.ToKnownColor() == colorToSelect.ToKnownColor()) ||
                                    subItem.Color.ToArgb() == colorToSelect.ToArgb())
                                {
                                    colorItem = subItem;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (colorItem == null)
            {
                colorItem = new GroupColorItem(this.RecentGroup, colorToSelect);
                this.RecentGroup.Items.Add(colorItem);
            }

            if (m_ciSelectedItem != null)
            {
                m_ciSelectedItem.State = ColorItemState.Normal;
            }

            colorItem.State = ColorItemState.Selected;
            m_ciSelectedItem = colorItem;

            this.Invalidate();
        }

        /// <summary>
        ///  Gets or sets color, which become selected after automatic button click.
        /// Default value is black color.
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(typeof(Color), "Black")]
        [Description("Gets or sets color, which become selected after automatic button click. Default value is black color.")]
        public Color AutomaticColor
        {
            get 
            {
                return m_automaticColor; 
            }

            set 
            { 
                m_automaticColor = value; 
            }
        }

        /// <summary>
        /// Gets or sets index of TabPageAdv, that was selected in previous selection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedTabIndex
        {
            get 
            {
                return m_selectedTabIndex; 
            }
            set
            {
                if (m_selectedTabIndex != value)
                    m_selectedTabIndex = value;
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Raised when ColorItem is picked or new <see cref="ColorItem"/> was added from <see cref="ColorDlgAdv"/>.
        /// </summary>
        [Description("Raised when ColorItem is picked or new ColorItem was added from ColorDlgAdv.")]
        public event ColorPickedEventHandler Picked;

        /// <summary>
        /// Raised when mouse hovers over <see cref="ColorItem"/>.
        /// </summary>
        [Description("Raised when mouse hovers over ColorItem.")]
        public event ColorPickedEventHandler ItemSelection;
        #endregion

        #region Methods

        private void CalculateSize()
        {
            if (this.Groups == null || this.Groups.Count == 0)
            {
                if (this.Dock == DockStyle.None)
                {
                    this.Size = new Size(DEF_CONTROLWIDTH, this.ButtonsHeight * 2);
                }
                else
                {
                    CalcItemSpacing(this.Width, this.Height);
                }
            }
            else
            {
                int width = 0;
                int height = 0;

                if (this.m_btnState.Visible)
                    height += this.ButtonsHeight;

                if (this.m_btnMoreColors.Visible)
                    height += this.ButtonsHeight;

                foreach (ColorUIAdvGroup group in this.Groups)
                {
                    group.ShouldChangeParentSize = false;

                    group.UpdateGroupSize();

                    group.ShouldChangeParentSize = true;

                    if (group.Visible)
                        height += group.Size.Height;
                }

                width = this.GetMaxGroupWidth();

                if (this.Dock == DockStyle.None)
                {
                    this.Size = new Size(width, height);
                }
                else
                {
                    CalcItemSpacing(this.Width, this.Height);
                }
            }
        }

        private int GetMaxGroupWidth()
        {
            int width = 0;

            if (this.Groups != null && this.Groups.Count > 0)
            {
                foreach (ColorUIAdvGroup group in this.Groups)
                {
                    if (group.Visible && group.Size.Width > width)
                        width = group.Size.Width;
                }
            }

            return width;
        }

        /// <summary>
        /// Updates layout and invalidates control.
        /// </summary>
        public void UpdateControl()
        {
            this.CalculateSize();
            this.CalculateGroupsBounds();
            this.Invalidate();
        }

        private void Init()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();

            if (m_groups == null)
                m_groups = new ColorUIAdvGroupsCollection(this);

            if (m_customGroups == null)
                m_customGroups = new ColorUIAdvGroupsCollection(this);

            if (ChangedItems == null)
                ChangedItems = new Hashtable();

            this.InitDefaultGroups();

            if (m_renderer == null)
                this.SetRenderer();

            this.CalculateSize();
        }

        /// <summary>
        /// Fills default groups with colors.
        /// </summary>
        public void InitDefaultGroups()
        {
            m_themeGroup = new ColorUIAdvGroup(this, ColorUIAdvGroups.ThemeColors, false, true);
            m_standartGroup = new ColorUIAdvGroup(this, ColorUIAdvGroups.StandardColors, false, true);
            m_recentGroup = new ColorUIAdvGroup(this, ColorUIAdvGroups.RecentColors, false, true);

            m_groups.Add(m_themeGroup);
            m_groups.Add(m_standartGroup);
            m_groups.Add(m_recentGroup);
        }

        private void CalculateGroupsBounds()
        {
            int height = 0;

            if (this.Groups != null && this.Groups.Count != 0)
            {
                foreach (ColorUIAdvGroup group in this.Groups)
                    if (group.Visible && group.Size.Width < this.ClientSize.Width)
                    {
                        group.ShouldChangeParentSize = false;

                        group.Size = new Size(this.ClientSize.Width, group.Size.Height);

                        group.ShouldChangeParentSize = true;
                    }

                if ((this.StateButton.Dock == DockStyle.Top && this.StateButton.Visible) ||
                    (this.MoreColorsButton.Dock == DockStyle.Top && this.MoreColorsButton.Visible))
                    height += this.ButtonsHeight;
                ArrayList groups = this.Groups.SortedItems;

                for (int i = 0; i < groups.Count; i++)
                {
                    ColorUIAdvGroup group = groups[i] as ColorUIAdvGroup;

                    if (group.Visible)
                    {
                        Size size = group.Size;

                        group.Bounds = new Rectangle(0, height, size.Width, size.Height);

                        group.CalculateItemsBounds();

                        height += size.Height;
                    }
                }
            }
        }

        internal int FindNextGroup(int index, bool down)
        {
            if (down)
            {
                while (index <= this.Groups.Count)
                {
                    for (int i = 0; i < this.Groups.Count; i++)
                        if (this.Groups[i].Visible && this.Groups[i].Index == index)
                            return i;

                    index++;
                }

                return -1;
            }
            else
            {
                while (index >= 0)
                {
                    for (int i = this.Groups.Count - 1; i >= 0; i--)
                        if (this.Groups[i].Visible && this.Groups[i].Index == index)
                            return i;

                    index--;
                }

                return -1;
            }
        }

        private void SetRenderer()
        {
            if (this.UseOffice2007Style && this.Style == visualstyle.Office2007)
            {
              
                m_renderer = new ColorUIAdvOffice2007Renderer(this);
                foreach (Control control in this.Controls)
                {
                    if (control is ButtonAdv)
                    {
                        (control as ButtonAdv).UseVisualStyle = true;
                        (control as ButtonAdv).Appearance = ButtonAppearance.Office2007;
                    }
                }
            }
            else if (this.Style == visualstyle.Office2010)
            {
                m_renderer = new ColorUIAdvOffice2010Renderer(this);
                foreach (Control control in this.Controls)
                {
                    if (control is ButtonAdv)
                    {
                        (control as ButtonAdv).UseVisualStyle = true;
                        (control as ButtonAdv).Appearance = ButtonAppearance.Office2010;
                    }
                }
            }
            else if (this.Style == visualstyle.Metro)
            {
                mrenderer = new ColorUIAdvMetroRenderer(this);
                this.BackColor = Color.White;
                foreach (Control control in this.Controls)
                {
                    if (control is ButtonAdv)
                    {
                        (control as ButtonAdv).Height = m_buttonsHeight;
                        (control as ButtonAdv).UseVisualStyle = true;
                        (control as ButtonAdv).Appearance = ButtonAppearance.Metro;
                        (control as ButtonAdv).BackColor = this.metroColor; 
                    }
                   
                }

                this.Invalidate();
            }
            else if (this.Style == visualstyle.Default)
            {
                m_renderer = new ColorUIAdvRenderer(this);
                foreach (Control control in this.Controls)
                {
                    if (control is ButtonAdv)
                    {
                        (control as ButtonAdv).Height = m_buttonsHeight;
                        (control as ButtonAdv).UseVisualStyle = false;

                    }

                }
            }
        }

        internal ColorItem GetItemFromFocus(Point focus)
        {
            ColorItem foundItem = null;
            ColorUIAdvGroup group = this.ActiveGroup;

            if (group != null)
            {
                int x = 0;
                int y = group.Bounds.Y;

                x += this.BorderOffset + (this.ColorItemSize.Width + this.HorizontalItemsSpacing) * focus.X;
                y += this.BorderOffset;

                if (group.Header.Dock == DockStyle.Top)
                    y += group.HeaderHeight;

                Point location = new Point(x, y);
                Size size = this.ColorItemSize;

                Rectangle rect = new Rectangle(location, size);

                int index = -1;
                foreach (GroupColorItem item in group.Items)
                {
                    if (item.Bounds == rect)
                    {
                        index = group.FindNextItem(item.Index - 1, true);
                        break;
                    }
                }

                if (index >= 0)
                {
                    GroupColorItem colorItem = group.Items[index] as GroupColorItem;

                    if (focus.Y == 0)
                    {
                        foundItem = colorItem;
                    }
                    else
                    {
                        for (int i = 0; i < colorItem.SubItems.Count; i++)
                        {
                            if (focus.Y == i + 1)
                            {
                                foundItem = colorItem.SubItems[i];

                                break;
                            }
                        }
                    }
                }
            }

            return foundItem;
        }

        internal Point GetFocusFromItem(ColorItem item)
        {
            Point newFocus = new Point(-1, -1);

            foreach (ColorUIAdvGroup group in this.Groups)
            {
                if (group.Visible && group.Bounds.Contains(item.Bounds))
                {
                    for (int i = 0; i < group.Items.Count; i++)
                    {
                        GroupColorItem baseItem = group.Items[i] as GroupColorItem;

                        if (baseItem.Bounds == item.Bounds)
                        {
                            newFocus = new Point(group.FindNextItem(baseItem.Index - 1, true), 0);
                            break;
                        }
                        else if (group.IsSubItemsVisible)
                        {
                            foreach (ColorItem subItem in baseItem.SubItems)
                                if (subItem.Bounds == item.Bounds)
                                {
                                    newFocus = new Point(group.FindNextItem(baseItem.Index - 1, true), subItem.Index + 1);
                                    break;
                                }
                        }
                    }
                }
            }

            return newFocus;
        }

        internal Point GetItemUnderPoint(Point p)
        {
            ColorUIAdvGroup activeGroup = this.ActiveGroup;
            Rectangle bounds = this.ActiveGroup.Bounds;

            int across = -1;
            int down = -1;

            int index = -1;

            for (int i = 0; i < activeGroup.Items.Count; i++)
            {
                int foundIndex = activeGroup.FindNextItem(index++, true);

                GroupColorItem colorItem = null;
                if (foundIndex != -1)
                    colorItem = activeGroup.Items[foundIndex] as GroupColorItem;
                else
                    break;

                Rectangle rect = colorItem.Bounds;

                if (rect.Contains(p))
                {
                    across = colorItem.Index;
                    down = 0;

                    break;
                }
                else if (activeGroup.IsSubItemsVisible)
                {
                    for (int j = 0; j < activeGroup.SubItemsDepth; j++)
                    {
                        if (j < colorItem.SubItems.Count)
                        {
                            ColorItem item = colorItem.SubItems[j];

                            if (item.Bounds.Contains(p))
                            {
                                across = colorItem.Index;
                                down = j + 1;

                                break;
                            }
                        }
                    }

                    if (down != -1)
                        break;
                }
            }

            return new Point(across, down);
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }

        protected virtual void OnPicked(ColorPickedEventArgs e)
        {
            if (this.Picked != null)
                this.Picked(this, e);
        }

        protected virtual void OnItemSelection(ColorPickedEventArgs e)
        {
            if (this.ItemSelection != null)
                this.ItemSelection(this, e);
        }

        private void InvalidatePrevItem(ref ColorItem item)
        {
            Point newFocus = new Point(-1, -1);

            if (this.ActiveGroup != null)
            {
                item = this.GetItemFromFocus(m_focus);
                if (item.State == ColorItemState.Highlighted && this.SelectedItem != item)
                {
                    item.State = ColorItemState.Normal;
                    this.Invalidate(item.Bounds);
                }
            }
        }

        private void InvalidateNextItem(ref ColorItem item)
        {
            item = this.GetItemFromFocus(m_focus);
            if (item != null)
            {
                item.State = ColorItemState.Highlighted;
                m_ciHighLightedItem = item;
                this.Invalidate(m_ciHighLightedItem.Bounds);

                if (this.SelectedItem != null &&
                    this.SelectedItem != m_ciHighLightedItem && this.SelectedItem.State == ColorItemState.Highlighted)
                {
                    this.SelectedItem.State = ColorItemState.Selected;
                    this.Invalidate(this.SelectedItem.Bounds);
                }
            }
        }

        private Size GetMinSize()
        {
            int height = 0;
            int width = 0;

            if (this.MoreColorsButton.Visible || m_bInSetBoundsCore)
                height += this.ButtonsHeight;

            if (this.StateButton.Visible || m_bInSetBoundsCore)
                height += this.ButtonsHeight;

            int maxItemsCount = 0;

            foreach (ColorUIAdvGroup group in this.Groups)
            {
                if (group.Visible)
                {
                    if (maxItemsCount < group.Items.Count)
                        maxItemsCount = group.Items.Count;

                    height += group.HeaderHeight + this.ColorItemSize.Height;

                    if (group.IsSubItemsVisible)
                        height += this.BorderOffset * 2 + DEF_BASECOLORSOFFSET + group.SubItemsDepth * this.ColorItemSize.Height;
                    else
                        height += this.BorderOffset * 2;
                }
            }

            width = this.BorderOffset * 2 + maxItemsCount * this.ColorItemSize.Width;

            return new Size(width, height);
        }

        private void CalcItemSpacing(int width, int height)
        {
            double hSpace, vSpace;
            int maxItemsCount = 0;

            foreach (ColorUIAdvGroup group in this.Groups)
                if (group.Visible && maxItemsCount < group.Items.Count)
                    maxItemsCount = group.Items.Count;

            if (maxItemsCount <= 1)
                return;

            int vSpacesCount = 0;
            int groupsVisibleSubItems = 0;
            foreach (ColorUIAdvGroup group in this.Groups)
            {
                if (group.Visible && group.IsSubItemsVisible)
                {
                    vSpacesCount += group.SubItemsDepth - 1;
                    groupsVisibleSubItems++;
                }
            }

            Size defSize = this.GetMinSize();

            if (vSpacesCount != 0)
                vSpace = (height - defSize.Height) / vSpacesCount;
            else
                vSpace = 0;

            if (vSpace > DEF_BASECOLORSOFFSET && vSpace != this.VerticalItemsSpacing)
            {
                // Need to recalculate verticl spacing to include
                // adjusted 1 vertical space for each group if it's subitems are visible.
                vSpacesCount += groupsVisibleSubItems;

                if (vSpacesCount != 0)
                    vSpace = (height + groupsVisibleSubItems * DEF_BASECOLORSOFFSET - defSize.Height) / vSpacesCount;
                else
                    vSpace = 0;
            }

            if (height >= defSize.Height)
                this.VerticalItemsSpacing = (int)vSpace;

            hSpace = width - this.BorderOffset * 2 - maxItemsCount * this.ColorItemSize.Width;
            int hValue = (int)(hSpace / (maxItemsCount - 1));

            if (width >= defSize.Width)
                this.HorizontalItemsSpacing = hValue;
        }

        private bool ShouldRemoveRecentItems()
        {
            foreach (ColorUIAdvGroup group in this.Groups)
            {
                if (group.GroupType == ColorUIAdvGroups.RecentColors)
                {
                    int width = this.BorderOffset + group.Items.Count * this.ColorItemSize.Width +
                        (group.Items.Count - 1) * this.HorizontalItemsSpacing;

                    if (width > (this.ClientSize.Width - this.ColorItemSize.Width - this.HorizontalItemsSpacing))
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Processes the dialog key.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>Returns bool value</returns>
        protected override bool ProcessDialogKey(Keys keys)
        {
            if (keys == Keys.Enter || (keys >= Keys.Left && keys <= Keys.Down))
            {
                return ProcessDialogKeyInternal(keys);
            }

            return base.ProcessDialogKey(keys);
        }

        /// <summary>
        /// Processes the dialog key internal.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>Returns bool property</returns>
        protected virtual bool ProcessDialogKeyInternal(Keys keys)
        {
            ColorItem item = null;

            if (m_focus.X == -1 && m_focus.Y == -1)
                m_focus = new Point(0, 0);

            switch (keys)
            {
                case Keys.Enter:
                    if (this.StateButton.Focused || this.MoreColorsButton.Focused)
                    {
                        if (this.StateButton.Focused)
                            this.StateButton.PerformClick();
                        else
                            this.MoreColorsButton.PerformClick();

                        return true;
                    }

                    item = this.GetItemFromFocus(m_focus);

                    if (item != null)
                    {
                        if (this.SelectedItem != null)
                        {
                            this.SelectedItem.State = ColorItemState.Normal;
                            this.Invalidate(this.SelectedItem.Bounds);
                        }

                        item.State = ColorItemState.Selected;
                        m_ciSelectedItem = item;
                        m_ciHighLightedItem = null;
                        this.Invalidate(this.SelectedItem.Bounds);
                    }

                    ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);

                    this.OnPicked(args);
                    return true;

                case Keys.Left:
                    this.InvalidatePrevItem(ref item);

                    if (this.ActiveGroup != null)
                    {
                        if (m_focus.Y == 0)
                        {
                            if (m_focus.X > 0)
                                m_focus.X = m_focus.X - 1;
                            else if (m_focus.X == 0)
                                m_focus.X = this.ActiveGroup.Items.Count - 1;
                        }
                        else
                        {
                            GroupColorItem baseItem = null;

                            do
                            {
                                int foundIndex = -1;

                                if (m_focus.X == 0)
                                    m_focus.X = this.ActiveGroup.Items.Count;

                                foundIndex = this.ActiveGroup.FindNextItem(m_focus.X--, false);

                                baseItem = this.ActiveGroup.Items[foundIndex] as GroupColorItem;
                            }
                            while (baseItem.SubItems.Count <= m_focus.Y - 1);
                        }
                    }
                    else
                        return true;

                    this.InvalidateNextItem(ref item);
                    return true;

                case Keys.Right:
                    this.InvalidatePrevItem(ref item);

                    if (this.ActiveGroup != null)
                    {
                        if (m_focus.Y == 0)
                        {
                            if (m_focus.X < this.ActiveGroup.Items.Count - 1)
                                m_focus.X = m_focus.X + 1;
                            else if (m_focus.X == this.ActiveGroup.Items.Count - 1)
                                m_focus.X = 0;
                        }
                        else
                        {
                            GroupColorItem baseItem = null;

                            do
                            {
                                int foundIndex = -1;

                                if (m_focus.X == this.ActiveGroup.Items.Count - 1)
                                    m_focus.X = -1;

                                foundIndex = this.ActiveGroup.FindNextItem(m_focus.X++, true);

                                baseItem = this.ActiveGroup.Items[foundIndex] as GroupColorItem;
                            }
                            while (baseItem.SubItems.Count <= m_focus.Y - 1);
                        }
                    }
                    else
                        return true;

                    this.InvalidateNextItem(ref item);
                    return true;

                case Keys.Up:
                    this.InvalidatePrevItem(ref item);

                    if (this.MoreColorsButton.Focused)
                    {
                        this.MoreColorsButton.State = ButtonAdvState.Default;

                        if (this.SelectedItem != null)
                        {
                            this.Focus();
                            this.SelectedItem.State = ColorItemState.Highlighted;
                        }
                        else
                        {
                            while (this.ActiveGroup == null || this.ActiveGroup.Items.Count <= m_focus.X)
                            {
                                int groupIndex = -1;

                                if (this.ActiveGroup == null)
                                    groupIndex = this.FindNextGroup(this.Groups.Count - 1, false);
                                else
                                    groupIndex = this.FindNextGroup(this.ActiveGroup.Index - 1, false);

                                if (groupIndex != -1)
                                {
                                    this.ActiveGroup = this.Groups[groupIndex];

                                    if (m_focus.X >= this.ActiveGroup.Items.Count)
                                        continue;

                                    GroupColorItem baseItem = this.ActiveGroup.Items[m_focus.X] as GroupColorItem;

                                    if (this.ActiveGroup.IsSubItemsVisible && baseItem.SubItems.Count > 0)
                                        m_focus.Y = baseItem.SubItems.Count;

                                    this.Focus();
                                }
                                else
                                {
                                    this.ActiveGroup = null;

                                    this.StateButton.State = ButtonAdvState.MouseOver;
                                    this.StateButton.Focus();

                                    return true;
                                }
                            }
                        }
                    }
                    else if (this.StateButton.Focused)
                    {
                        this.StateButton.State = ButtonAdvState.Default;

                        this.MoreColorsButton.State = ButtonAdvState.MouseOver;
                        this.MoreColorsButton.Focus();

                        return true;
                    }
                    else
                    {
                        GroupColorItem baseItem = this.GetItemFromFocus(new Point(m_focus.X, 0)) as GroupColorItem;

                        if (baseItem != null)
                        {
                            if (this.ActiveGroup.IsSubItemsVisible && m_focus.Y > 0)
                                m_focus.Y = m_focus.Y - 1;
                            else
                            {
                                do
                                {
                                    int groupIndex = this.FindNextGroup(this.ActiveGroup.Index - 1, false);

                                    if (groupIndex != -1)
                                    {
                                        this.ActiveGroup = this.Groups[groupIndex];

                                        if (this.ActiveGroup.IsSubItemsVisible)
                                        {
                                            GroupColorItem colorItem = this.ActiveGroup.Items[m_focus.X] as GroupColorItem;
                                            if (colorItem.SubItems.Count > 0)
                                                m_focus.Y = colorItem.SubItems.Count;
                                        }
                                    }
                                    else
                                    {
                                        this.ActiveGroup = null;

                                        this.StateButton.State = ButtonAdvState.MouseOver;
                                        this.StateButton.Focus();

                                        return true;
                                    }
                                }
                                while (this.ActiveGroup.Items.Count <= m_focus.X);
                            }
                        }
                    }

                    this.InvalidateNextItem(ref item);
                    return true;

                case Keys.Down:
                    this.InvalidatePrevItem(ref item);

                    if (this.MoreColorsButton.Focused)
                    {
                        this.MoreColorsButton.State = ButtonAdvState.Default;

                        if (this.StateButton.Visible)
                        {
                            this.ActiveGroup = null;

                            this.StateButton.State = ButtonAdvState.MouseOver;
                            this.StateButton.Focus();
                            return true;
                        }
                        else
                        {
                            this.Focus();
                            this.ActiveGroup = this.Groups[this.FindNextGroup(0, true)];
                        }
                    }
                    else if (this.StateButton.Focused)
                    {
                        this.StateButton.State = ButtonAdvState.Default;

                        this.Focus();

                        if (this.SelectedItem == null)
                            this.ActiveGroup = this.Groups[this.FindNextGroup(0, true)];
                    }
                    else
                    {
                        GroupColorItem baseItem = this.GetItemFromFocus(new Point(m_focus.X, 0)) as GroupColorItem;

                        if (baseItem != null)
                        {
                            if (this.ActiveGroup.IsSubItemsVisible && m_focus.Y < baseItem.SubItems.Count)
                                m_focus.Y = m_focus.Y + 1;
                            else
                            {
                                m_focus.Y = 0;

                                do
                                {
                                    int groupIndex = this.FindNextGroup(this.ActiveGroup.Index + 1, true);

                                    if (groupIndex != -1)
                                        this.ActiveGroup = this.Groups[groupIndex];
                                    else
                                    {
                                        this.ActiveGroup = null;

                                        this.MoreColorsButton.State = ButtonAdvState.MouseOver;
                                        this.MoreColorsButton.Focus();

                                        return true;
                                    }
                                }
                                while (this.ActiveGroup.Items.Count <= m_focus.X);
                            }
                        }
                    }

                    this.InvalidateNextItem(ref item);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnGotFocus(e);

            this.UpdateControl();

            if (this.ActiveGroup == null && this.Groups.Count > 0)
            {
                if (this.SelectedItem == null)
                {
                    this.ActiveGroup = this.Groups[0];

                    if (this.ActiveGroup.Items.Count == 0)
                        return;

                    if (m_focus.X == -1 && m_focus.Y == -1)
                        m_focus = new Point(0, 0);

                    ColorItem item = this.GetItemFromFocus(m_focus);

                    if (item != null)
                    {
                        item.State = ColorItemState.Highlighted;
                        m_ciHighLightedItem = item;
                        this.Invalidate(m_ciHighLightedItem.Bounds);
                    }
                }
                else
                {
                    foreach (ColorUIAdvGroup group in this.Groups)
                        if (group.Bounds.Contains(this.SelectedItem.Bounds))
                        {
                            this.ActiveGroup = group;
                            m_focus = this.GetFocusFromItem(this.SelectedItem);
                        }
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point p = new Point(e.X, e.Y);

            if (this.Groups != null && this.Groups.Count > 0)
            {
                foreach (ColorUIAdvGroup group in this.Groups)
                {
                    if (group.Bounds.Contains(p) && this.ActiveGroup != group && group.Visible)
                    {
                        this.ActiveGroup = group;
                        break;
                    }
                }

                if (this.ActiveGroup != null)
                {
                    m_focus = this.GetItemUnderPoint(p);

                    if (m_focus.X != -1 && m_focus.Y != -1)
                    {
                        ColorItem item = this.GetItemFromFocus(m_focus);

                        if (item != null && item != m_ciHighLightedItem)
                        {
                            item.State = ColorItemState.Highlighted;
                            this.Invalidate(item.Bounds);

                            if (m_ciHighLightedItem != null)
                            {
                                if (m_ciHighLightedItem != this.SelectedItem)
                                    m_ciHighLightedItem.State = ColorItemState.Normal;

                                this.Invalidate(m_ciHighLightedItem.Bounds);
                            }

                            m_ciHighLightedItem = item;

                            ColorPickedEventArgs args = new ColorPickedEventArgs(m_ciHighLightedItem.Color);
                            this.OnItemSelection(args);
                        }
                    }
                    else
                    {
                        if (m_ciHighLightedItem != null &&
                            m_ciHighLightedItem != this.SelectedItem)
                        {
                            m_ciHighLightedItem.State = ColorItemState.Normal;
                            this.Invalidate(m_ciHighLightedItem.Bounds);
                        }
                        else if (m_ciHighLightedItem == this.SelectedItem)
                        {
                            if (this.SelectedItem != null)
                            {
                                this.SelectedItem.State = ColorItemState.Selected;
                                this.Invalidate(this.SelectedItem.Bounds);
                            }
                        }

                        m_ciHighLightedItem = null;
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.Focused)
                this.Focus();

            Point p = new Point(e.X, e.Y);

            Rectangle rect = this.ClientRectangle;
            rect.Y += this.ButtonsHeight;
            rect.Height -= this.ButtonsHeight * 2;
            if (this.Groups != null && this.Groups.Count > 0)
            {
                if (rect.Contains(p))
                {
                    m_focus = this.GetItemUnderPoint(p);

                    if (m_focus.X != -1 && m_focus.Y != -1)
                    {
                        ColorItem item = this.GetItemFromFocus(m_focus);
                        item.State = ColorItemState.Selected;
                        this.Invalidate(item.Bounds);

                        if (this.SelectedItem != null && item != this.SelectedItem)
                        {
                            this.SelectedItem.State = ColorItemState.Normal;
                            this.Invalidate(this.SelectedItem.Bounds);
                        }

                        m_ciSelectedItem = item;
                        ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);
                        this.OnPicked(args);
                    }
                }
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (colorPickerStyle == visualstyle.Metro)
            {
                foreach (Control control in this.Controls)
                {
                    if (control is ButtonAdv)
                    {
                        (control as ButtonAdv).Height = m_buttonsHeight;
                        (control as ButtonAdv).UseVisualStyle = true;
                        (control as ButtonAdv).Appearance = ButtonAppearance.Metro;
                        (control as ButtonAdv).BackColor = this.metroColor;
                    }
                }
               
                mrenderer.OnPaint(e);
                if (m_borderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
                {
                    Pen pen = new Pen(Color.FromArgb(209, 211, 212), 1);
                    e.Graphics.DrawRectangle(pen, this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
                    pen.Dispose();
                }
            }
            else
                m_renderer.OnPaint(e);
                
           
        }

        /// <summary>
        /// Gets the required creation parameters when the control handle is created.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Windows.Forms.CreateParams"/> that contains the required creation parameters when the handle to the control is created.</returns>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                switch (m_borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        {
                            cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE; // WS_EX_DLGFRAME
                            break;
                        }
                    case BorderStyle.FixedSingle:
                        {
                            cp.Style |= NativeMethods.WS_BORDER;
                            break;
                        }
                }

                return cp;
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        /// <summary>
        /// Gets or sets the size that is the lower limit that <see cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)"/> can specify.
        /// </summary>
        /// <value></value>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"/> representing the width and height of a rectangle.</returns>
        public override Size MinimumSize
        {
            get
            {
                return this.GetMinSize();
            }
        }
#endif
        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new <see cref="P:System.Windows.Forms.Control.Left"/> property value of the control.</param>
        /// <param name="y">The new <see cref="P:System.Windows.Forms.Control.Top"/> property value of the control.</param>
        /// <param name="width">The new <see cref="P:System.Windows.Forms.Control.Width"/> property value of the control.</param>
        /// <param name="height">The new <see cref="P:System.Windows.Forms.Control.Height"/> property value of the control.</param>
        /// <param name="specified">A bitwise combination of the <see cref="T:System.Windows.Forms.BoundsSpecified"/> values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            m_bInSetBoundsCore = true;

            Size minSize = this.GetMinSize();

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (width <= minSize.Width &&
				(specified == BoundsSpecified.X ||
                 specified == (BoundsSpecified.X | BoundsSpecified.Width) ||
                 specified == (BoundsSpecified.X | BoundsSpecified.Width | BoundsSpecified.Height) ||
                 specified == (BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width | BoundsSpecified.Height)))
                x = this.Location.X;

            if (height <= minSize.Height &&
                (specified == BoundsSpecified.Y ||
                 specified == (BoundsSpecified.Y | BoundsSpecified.Height) ||
                 specified == (BoundsSpecified.Y | BoundsSpecified.Width | BoundsSpecified.Height) ||
                 specified == (BoundsSpecified.X | BoundsSpecified.Y | BoundsSpecified.Width | BoundsSpecified.Height)))
                y = this.Location.Y;
#endif

            if (width < minSize.Width && height < minSize.Height)
            {
                base.SetBoundsCore(x, y, minSize.Width, minSize.Height, specified);
            }
            else if (width < minSize.Width)
            {
                base.SetBoundsCore(x, y, minSize.Width, height, specified);
            }
            else if (height < minSize.Height)
            {
                base.SetBoundsCore(x, y, width, minSize.Height, specified);
            }
            else
            {
                this.CalcItemSpacing(width, height);

                base.SetBoundsCore(x, y, width, height, specified);
            }

            m_bInSetBoundsCore = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.CalcItemSpacing(this.Width, this.Height);
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Drawing.Color"/> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"/> property.</returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
            }
        }

        #endregion

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_btnState = new Syncfusion.Windows.Forms.ButtonAdv();
            this.m_btnMoreColors = new Syncfusion.Windows.Forms.ButtonAdv();
            this.SuspendLayout();
            // 
            // m_btnState
            // 
            this.m_btnState.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.m_btnState.ComboEditBackColor = System.Drawing.Color.Empty;
            this.m_btnState.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_btnState.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnState.IsMouseDown = false;
            this.m_btnState.KeepFocusRectangle = false;
            this.m_btnState.Location = new System.Drawing.Point(0, 0);
            this.m_btnState.Name = "m_btnState";
            this.m_btnState.Size = new System.Drawing.Size(150, 23);
            this.m_btnState.TabIndex = 0;
            this.m_btnState.Text = SR.GetString(SR.ColorPickerStateButton);
            this.m_btnState.UseVisualStyle = true;
            this.m_btnState.Click += new System.EventHandler(M_btnState_Click);
            // 
            // m_btnMoreColors
            // 
            this.m_btnMoreColors.Appearance = Syncfusion.Windows.Forms.ButtonAppearance.Office2007;
            this.m_btnMoreColors.ComboEditBackColor = System.Drawing.Color.Empty;
            this.m_btnMoreColors.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.m_btnMoreColors.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_btnMoreColors.IsMouseDown = false;
            this.m_btnMoreColors.KeepFocusRectangle = false;
            this.m_btnMoreColors.Location = new System.Drawing.Point(0, 127);
            this.m_btnMoreColors.Name = "m_btnMoreColors";
            this.m_btnMoreColors.Size = new System.Drawing.Size(150, 23);
            this.m_btnMoreColors.TabIndex = 0;
            this.m_btnMoreColors.Text = SR.GetString(SR.ColorPickerMoreColorsButton);
            this.m_btnMoreColors.UseVisualStyle = true;
            this.m_btnMoreColors.Click += new System.EventHandler(M_btnMoreColors_Click);
            // 
            // ColorPickerUIAdv
            // 
            this.Controls.Add(this.m_btnState);
            this.Controls.Add(this.m_btnMoreColors);
            this.Name = "ColorPickerUIAdv";
            this.ResumeLayout(false);
            CISIZE = this.ColorItemSize;
        }
        #endregion

        #region Event handlers

        private void M_btnState_Click(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                ISelectionService selectionService = this.GetService(typeof(ISelectionService)) as ISelectionService;
                ArrayList list = new ArrayList(); 

                list.Add(this);
                selectionService.SetSelectedComponents(list);
            }
            else
            {
                if (this.SelectedItem != null && this.SelectedItem.Color == this.AutomaticColor)
                {
                    return;
                }
                bool picked = false;

                foreach (ColorUIAdvGroup group in this.Groups)
                {
                    for (int i = 0; i < group.Items.Count; i++)
                    {
                        GroupColorItem baseItem = group.Items[i] as GroupColorItem;

                        if (baseItem.Color == this.AutomaticColor)
                        {
                            picked = true;

                            if (this.SelectedItem != null)
                            {
                                this.SelectedItem.State = ColorItemState.Normal;
                                this.Invalidate(this.SelectedItem.Bounds);
                            }

                            baseItem.State = ColorItemState.Selected;
                            m_ciSelectedItem = baseItem;

                            this.Invalidate(this.SelectedItem.Bounds);

                            ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);

                            this.OnPicked(args);

                            break;
                        }

                        if (group.IsSubItemsVisible)
                        {
                            for (int j = 0; j < baseItem.SubItems.Count; j++)
                            {
                                ColorItem item = baseItem.SubItems[j];

                                if (item.Color == this.AutomaticColor && this.SelectedItem != item)
                                {
                                    picked = true;

                                    if (this.SelectedItem != null)
                                    {
                                        this.SelectedItem.State = ColorItemState.Normal;
                                        this.Invalidate(this.SelectedItem.Bounds);
                                    }

                                    item.State = ColorItemState.Selected;
                                    m_ciSelectedItem = item;

                                    this.Invalidate(this.SelectedItem.Bounds);

                                    ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);

                                    this.OnPicked(args);

                                    break;
                                }
                            }
                        }
                    }

                    if (picked)
                        break;
                }

                if (!picked)
                {
                    foreach (ColorUIAdvGroup group in this.Groups)
                    {
                        if (group.GroupType == ColorUIAdvGroups.RecentColors)
                        {
                            if (!group.Visible)
                                group.Visible = true;

                            GroupColorItem item = new GroupColorItem(group, this.AutomaticColor);

                            int itemsCount = 0;
                            foreach (ColorUIAdvGroup colorGroup in this.Groups)
                                if (colorGroup.Items.Count > itemsCount)
                                    itemsCount = colorGroup.Items.Count;

                            if (this.ShouldRemoveRecentItems() && group.Items.Count >= itemsCount)
                                group.Items.Remove(group.Items[0]);

                            for (int i = 0; i < group.Items.Count; i++)
                                group.Items[i].Index++;

                            if (this.SelectedItem != null)
                            {
                                this.SelectedItem.State = ColorItemState.Normal;
                                this.Invalidate(this.SelectedItem.Bounds);
                            }

                            item.State = ColorItemState.Selected;
                            m_ciSelectedItem = item;
                            m_ciHighLightedItem = null;

                            group.Items.Add(item);
                            group.CalculateItemsBounds();

                            this.Invalidate(group.Bounds);

                            ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);

                            this.OnPicked(args);

                            break;
                        }
                    }
                }
            }
        }

        private void M_btnMoreColors_Click(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                ISelectionService selectionService = this.GetService(typeof(ISelectionService)) as ISelectionService;
                ArrayList list = new ArrayList();

                list.Add(this);
                selectionService.SetSelectedComponents(list);
            }
            else
            {
                if (this.Parent is PopupControlContainer)
                    (this.Parent as PopupControlContainer).HidePopup(PopupCloseType.Deactivated);
                ColorDlgAdv dialog = new ColorDlgAdv(this);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (ColorUIAdvGroup group in this.Groups)
                    {
                        if (group.GroupType == ColorUIAdvGroups.RecentColors)
                        {
                            if (!group.Visible)
                                group.Visible = true;

                            GroupColorItem item = new GroupColorItem(group, dialog.Color);

                            int itemsCount = 0;
                            foreach (ColorUIAdvGroup colorGroup in this.Groups)
                                if (colorGroup.Items.Count > itemsCount)
                                    itemsCount = colorGroup.Items.Count;

                            if (this.ShouldRemoveRecentItems() && group.Items.Count >= itemsCount && itemsCount != 0)
                                group.Items.Remove(group.Items[0]);

                            for (int i = 0; i < group.Items.Count; i++)
                                group.Items[i].Index++;

                            if (this.SelectedItem != null)
                            {
                                this.SelectedItem.State = ColorItemState.Normal;
                                this.Invalidate(this.SelectedItem.Bounds);
                            }

                            item.State = ColorItemState.Selected;
                            m_ciSelectedItem = item;
                            m_ciHighLightedItem = null;

                            group.Items.Add(item);
                            group.CalculateItemsBounds();

                            this.Invalidate(group.Bounds);

                            ColorPickedEventArgs args = new ColorPickedEventArgs(this.SelectedItem.Color);

                            this.OnPicked(args);

                            break;
                        }
                    }
                }
                dialog.Dispose();
            }
        }

        #endregion

        #region Delegates and event arguments

        /// <summary>
        /// Delegate for the <see cref="ColorPickerUIAdv.Picked"/> event.
        /// </summary>
        /// <param name="sender"> Sender Object</param>
        /// <param name="args">ColorPickedEventArgs that contains the event data.</param>
        /// <remarks>The ColorPickedEventHandler uses the <see cref="ColorPickedEventArgs"/>
        /// class as the event data.</remarks>
        public delegate void ColorPickedEventHandler(object sender, ColorPickedEventArgs args);

        /// <summary>
        /// This class is used for keep color that was picked in <see cref="ColorPickerUIAdv"/>.
        /// </summary>
        public class ColorPickedEventArgs : EventArgs
        {
            private Color m_color;

            public Color Color
            {
                get { return m_color; }
            }

            public ColorPickedEventArgs(Color color)
            {
                m_color = color;
            }
        }

        #endregion
    }

    public class ColorUIAdvDesignSerializer : CodeDomSerializer
    {
        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.
                GetSerializer(typeof(ColorPickerUIAdv).BaseType, typeof(CodeDomSerializer));

            return baseSerializer.Deserialize(manager, codeObject);
        }

        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            CodeDomSerializer baseSerializer = (CodeDomSerializer)manager.
                GetSerializer(typeof(ColorPickerUIAdv).BaseType, typeof(CodeDomSerializer));

            ColorPickerUIAdv colorPicker = value as ColorPickerUIAdv;

            CodeStatementCollection themeStatements = new CodeStatementCollection();
            CodeStatementCollection standardStatements = new CodeStatementCollection();
            CodeStatementCollection recentStatements = new CodeStatementCollection();

            ArrayList themeItems = new ArrayList();
            ArrayList addedThemeItems = new ArrayList();

            ArrayList standardItems = new ArrayList();
            ArrayList addedStandardItems = new ArrayList();

            ArrayList addedRecentItems = new ArrayList();

            ItemComparer comparator = new ItemComparer();

            if (colorPicker.ChangedItems.Count > 0)
            {
                foreach (object o in colorPicker.ChangedItems.Keys)
                {
                    ColorItem item = o as ColorItem;
                    GroupColorItem baseItem = item as GroupColorItem;

                    ColorUIAdvGroup group = colorPicker.ChangedItems[item] as ColorUIAdvGroup;

                    if (group.GroupType == ColorUIAdvGroups.ThemeColors)
                        themeItems.Add(item);
                    else if (group.GroupType == ColorUIAdvGroups.StandardColors)
                        standardItems.Add(item);
                }

                themeItems.Sort(comparator);
                standardItems.Sort(comparator);
            }

            if (colorPicker.RecentGroup.Items.Count != 0)
            {
                for (int i = 0; i < colorPicker.RecentGroup.Items.Count; i++)
                {
                    ColorItem item = colorPicker.RecentGroup.Items[i];

                    addedRecentItems.Add(item);
                }

                addedRecentItems.Sort(comparator);
            }

            if (colorPicker.StandardGroup.Items.Count > ColorPickerUIAdv.DEF_ITEMSCOUNT || this.NonGroupColorItemsAdded(colorPicker.StandardGroup, ref addedStandardItems))
            {
                if (colorPicker.StandardGroup.Items.Count > ColorPickerUIAdv.DEF_ITEMSCOUNT)
                {
                    for (int i = ColorPickerUIAdv.DEF_ITEMSCOUNT; i < colorPicker.StandardGroup.Items.Count; i++)
                    {
                        ColorItem item = colorPicker.StandardGroup.Items[i];

                        addedStandardItems.Add(item);
                    }
                }

                addedStandardItems.Sort(comparator);
            }

            if (colorPicker.ThemeGroup.Items.Count > ColorPickerUIAdv.DEF_ITEMSCOUNT || this.NonGroupColorItemsAdded(colorPicker.ThemeGroup, ref addedThemeItems))
            {
                if (colorPicker.ThemeGroup.Items.Count > ColorPickerUIAdv.DEF_ITEMSCOUNT)
                {
                    for (int i = ColorPickerUIAdv.DEF_ITEMSCOUNT; i < colorPicker.ThemeGroup.Items.Count; i++)
                    {
                        ColorItem item = colorPicker.ThemeGroup.Items[i];

                        addedThemeItems.Add(item);
                    }
                }

                addedThemeItems.Sort(comparator);
            }

            object codeObject = baseSerializer.Serialize(manager, value);

            if (codeObject is CodeStatementCollection)
            {
                CodeStatementCollection statements = codeObject as CodeStatementCollection;

                if (statements != null && statements.Count > 0)
                {
                    string controlName = string.Empty;
                    CodeAssignStatement assStatement = statements[0] as CodeAssignStatement;

                    if (assStatement != null)
                        controlName = (assStatement.Left as CodeFieldReferenceExpression).FieldName;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                    CodeStatementCollection defGroupsStatements = this.GetDefaultGroupsStatements(colorPicker, controlName);

                    for (int i = 0; i < defGroupsStatements.Count; i++)
                    {
                        statements.Insert(1 + i, defGroupsStatements[i]);
                    }
#endif

                    if ((colorPicker.ChangedItems != null && colorPicker.ChangedItems.Count > 0) ||
                            (addedThemeItems.Count > 0 || addedStandardItems.Count > 0 || addedRecentItems.Count > 0))
                    {
                        CodeFieldReferenceExpression controlFieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), controlName);

                        if (addedRecentItems.Count > 0)
                        {
                            for (int t = 0; t < addedRecentItems.Count; t++)
                            {
                                ColorItem item = addedRecentItems[t] as ColorItem;
                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 0, t);

                                CodeStatementCollection col = this.GetColorItemStatements(index, controlName, "RecentGroup", item);
                                recentStatements.AddRange(col);
                            }

                            for (int t = 0; t < addedRecentItems.Count; t++)
                            {
                                ColorItem item = addedRecentItems[t] as ColorItem;

                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 0, t);
                                CodeFieldReferenceExpression itemName = new CodeFieldReferenceExpression(null, "groupColorItem" + index);
                                CodePropertyReferenceExpression propExpr1 = new CodePropertyReferenceExpression(controlFieldRef, "RecentGroup");
                                CodePropertyReferenceExpression propExpr2 = new CodePropertyReferenceExpression(propExpr1, "Items");
                                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(propExpr2, "Add", new CodeExpression[] { itemName });
                                recentStatements.Add(methodInvoke);
                            }
                        }

                        if (addedStandardItems.Count > 0)
                        {
                            for (int t = 0; t < addedStandardItems.Count; t++)
                            {
                                ColorItem item = addedStandardItems[t] as ColorItem;
                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 1, t);

                                CodeStatementCollection col = this.GetColorItemStatements(index, controlName, "StandardGroup", item);
                                standardStatements.AddRange(col);
                            }

                            for (int t = 0; t < addedStandardItems.Count; t++)
                            {
                                ColorItem item = addedStandardItems[t] as ColorItem;

                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 1, t);
                                CodeFieldReferenceExpression itemName = new CodeFieldReferenceExpression(null, "groupColorItem" + index);

                                CodePropertyReferenceExpression propExpr1 = new CodePropertyReferenceExpression(controlFieldRef, "StandardGroup");
                                CodePropertyReferenceExpression propExpr2 = new CodePropertyReferenceExpression(propExpr1, "Items");

                                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(propExpr2, "Add", new CodeExpression[] { itemName });
                                standardStatements.Add(methodInvoke);
                            }
                        }

                        if (addedThemeItems.Count > 0)
                        {
                            for (int t = 0; t < addedThemeItems.Count; t++)
                            {
                                ColorItem item = addedThemeItems[t] as ColorItem;
                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 2, t);

                                CodeStatementCollection col = this.GetColorItemStatements(index, controlName, "ThemeGroup", item);
                                themeStatements.AddRange(col);
                            }

                            for (int t = 0; t < addedThemeItems.Count; t++)
                            {
                                ColorItem item = addedThemeItems[t] as ColorItem;

                                int index = this.GetBaseColorItemIndex(colorPicker, addedRecentItems, addedStandardItems, 2, t);
                                CodeFieldReferenceExpression itemName = new CodeFieldReferenceExpression(null, "groupColorItem" + index);

                                CodePropertyReferenceExpression propExpr1 = new CodePropertyReferenceExpression(controlFieldRef, "ThemeGroup");
                                CodePropertyReferenceExpression propExpr2 = new CodePropertyReferenceExpression(propExpr1, "Items");

                                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(propExpr2, "Add", new CodeExpression[] { itemName });
                                themeStatements.Add(methodInvoke);
                            }
                        }

                        for (int t = 0; t < standardItems.Count; t++)
                        {
                            ColorItem item = standardItems[t] as ColorItem;

                            CodeAssignStatement statement = this.GetColorAssignmentStatement(controlName, "StandardGroup", item);
                            standardStatements.Add(statement);
                        }

                        for (int t = 0; t < themeItems.Count; t++)
                        {
                            ColorItem item = themeItems[t] as ColorItem;
                            CodeAssignStatement statement = this.GetColorAssignmentStatement(controlName, "ThemeGroup", item);

                            themeStatements.Add(statement);
                        }

                        if (recentStatements.Count > 0)
                        {
                            for (int i = 0; i < statements.Count; i++)
                            {
                                CodeAssignStatement assignStatement = statements[i] as CodeAssignStatement;
                                if (assignStatement != null)
                                {
                                    CodePropertyReferenceExpression propExpr1 = assignStatement.Left as CodePropertyReferenceExpression;
                                    if (propExpr1 != null)
                                    {
                                        CodePropertyReferenceExpression propExpr2 = propExpr1.TargetObject as CodePropertyReferenceExpression;
                                        if (propExpr2 != null && propExpr2.PropertyName == "RecentGroup")
                                        {
                                            for (int j = 0; j < recentStatements.Count; j++)
                                            {
                                                statements.Insert(statements.IndexOf(assignStatement), recentStatements[j]);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (standardStatements.Count > 0)
                        {
                            for (int i = 0; i < statements.Count; i++)
                            {
                                CodeAssignStatement assignStatement = statements[i] as CodeAssignStatement;
                                if (assignStatement != null)
                                {
                                    CodePropertyReferenceExpression propExpr1 = assignStatement.Left as CodePropertyReferenceExpression;
                                    if (propExpr1 != null)
                                    {
                                        CodePropertyReferenceExpression propExpr2 = propExpr1.TargetObject as CodePropertyReferenceExpression;
                                        if (propExpr2 != null && propExpr2.PropertyName == "StandardGroup")
                                        {
                                            for (int j = 0; j < standardStatements.Count; j++)
                                            {
                                                statements.Insert(statements.IndexOf(assignStatement), standardStatements[j]);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (themeStatements.Count > 0)
                        {
                            for (int i = 0; i < statements.Count; i++)
                            {
                                CodeAssignStatement assignStatement = statements[i] as CodeAssignStatement;
                                if (assignStatement != null)
                                {
                                    CodePropertyReferenceExpression propExpr1 = assignStatement.Left as CodePropertyReferenceExpression;
                                    if (propExpr1 != null)
                                    {
                                        CodePropertyReferenceExpression propExpr2 = propExpr1.TargetObject as CodePropertyReferenceExpression;
                                        if (propExpr2 != null && propExpr2.PropertyName == "ThemeGroup")
                                        {
                                            for (int j = 0; j < themeStatements.Count; j++)
                                            {
                                                statements.Insert(statements.IndexOf(assignStatement), themeStatements[j]);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					for(int i = 0; i < statements.Count; i++ )
					{
						CodeVariableDeclarationStatement varDeclStatetment = statements[ i ] as CodeVariableDeclarationStatement;
						CodeAssignStatement assignStatement = statements[ i ] as CodeAssignStatement;

						if( varDeclStatetment != null )
						{
							CodeObjectCreateExpression objCreateExpr = varDeclStatetment.InitExpression as CodeObjectCreateExpression;
							
							if(objCreateExpr != null && objCreateExpr.CreateType.BaseType == "Syncfusion.Windows.Forms.Tools.GroupColorItem")
							{
								CodeExpressionCollection codeExprCol = objCreateExpr.Parameters;
								
								CodeFieldReferenceExpression fieldRef = codeExprCol[ 0 ] as CodeFieldReferenceExpression;
								
								if(fieldRef != null && fieldRef.TargetObject is CodeThisReferenceExpression)
									fieldRef.TargetObject = null;
							}
						}
						else if(assignStatement != null)
						{
							CodePropertyReferenceExpression propRef = assignStatement.Left as CodePropertyReferenceExpression;
							
							if( propRef != null && propRef.PropertyName == "Group" )
							{
								CodeFieldReferenceExpression fieldRef = assignStatement.Right as CodeFieldReferenceExpression;
								
								if(fieldRef != null && fieldRef.TargetObject is CodeThisReferenceExpression)
									fieldRef.TargetObject = null;
							}
						}
					}
#endif
                }
            }

            return codeObject;
        }

        private CodeAssignStatement GetColorAssignmentStatement(string control, string groupName, ColorItem item)
        {
            GroupColorItem baseItem = item as GroupColorItem;
            CodeAssignStatement assignmentStatement = new CodeAssignStatement();

            CodePropertyReferenceExpression propExpr1 = null;

            CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), control);
            CodePropertyReferenceExpression propExpr2 = new CodePropertyReferenceExpression(fieldRef, groupName);
            CodePropertyReferenceExpression propExpr3 = new CodePropertyReferenceExpression(propExpr2, "Items");

            if (baseItem != null)
            {
                CodeArrayIndexerExpression arrIndexer1 = new CodeArrayIndexerExpression(propExpr3, new CodeExpression[] { new CodePrimitiveExpression(item.Index) });

                propExpr1 = new CodePropertyReferenceExpression(arrIndexer1, "Color");
            }
            else
            {
                CodeArrayIndexerExpression arrIndexer1 = new CodeArrayIndexerExpression(propExpr3, new CodeExpression[] { new CodePrimitiveExpression(item.BaseItem.Index) });
                CodeCastExpression castExpr = new CodeCastExpression(typeof(Syncfusion.Windows.Forms.Tools.GroupColorItem), arrIndexer1);
                CodePropertyReferenceExpression propExpr4 = new CodePropertyReferenceExpression(castExpr, "SubItems");
                CodeArrayIndexerExpression arrIndexer2 = new CodeArrayIndexerExpression(propExpr4, new CodeExpression[] { new CodePrimitiveExpression(item.Index) });

                propExpr1 = new CodePropertyReferenceExpression(arrIndexer2, "Color");
            }

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Drawing.Color)), "FromArgb", new CodeExpression[] { new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.R)), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.G)), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.B)) });
            assignmentStatement.Left = propExpr1;
            assignmentStatement.Right = methodInvoke;
            return assignmentStatement;
        }

        /// <summary>
        /// Get the BaseColor Item index 
        /// </summary>
        /// <param name="colorPicker">Color Picker</param>
        /// <param name="recentItems">Recent items</param>
        /// <param name="standardItems">Standard items</param>
        /// <param name="groupIndex"> 0 - RecentGroup, 1 - StandardGroup, 2 - ThemeGroup.</param>
        /// <param name="i">Index of new added ColorItem.</param>
        /// <returns>Integer value for BasecolorItem index</returns>
        private int GetBaseColorItemIndex(ColorPickerUIAdv colorPicker, ArrayList recentItems, ArrayList standardItems, int groupIndex, int i)
        {
            int index = 0;

            if (colorPicker.CustomGroups.Count > 0)
            {
                foreach (ColorUIAdvGroup group in colorPicker.CustomGroups)
                    index += group.Items.Count;

                index++;
            }

            switch (groupIndex)
            {
                case 0:
                    index += i; 
                    break;
                case 1:
                    index += recentItems.Count + i; 
                    break;
                case 2:
                    index += recentItems.Count + standardItems.Count + i; 
                    break;
            }

            return index;
        }

        private CodeStatementCollection GetColorItemStatements(int index, string control, string groupName, ColorItem item)
        {
            GroupColorItem baseItem = item as GroupColorItem;
            CodeStatementCollection statements = new CodeStatementCollection();

            if (baseItem != null)
            {
                CodeFieldReferenceExpression itemName = new CodeFieldReferenceExpression(null, "groupColorItem" + index);

                CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), control);
                CodePropertyReferenceExpression propExpr1 = new CodePropertyReferenceExpression(fieldRef, groupName);
                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Drawing.Color)), "FromArgb", new CodeExpression[] { new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.R)), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.G)), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(item.Color.B)) });                                 
                CodeVariableDeclarationStatement varDecl = new CodeVariableDeclarationStatement(typeof(Syncfusion.Windows.Forms.Tools.GroupColorItem), itemName.FieldName, new CodeObjectCreateExpression(typeof(Syncfusion.Windows.Forms.Tools.GroupColorItem), new CodeExpression[] { new CodePrimitiveExpression(null), methodInvoke }));
                CodeAssignStatement colorAss = new CodeAssignStatement(new CodePropertyReferenceExpression(itemName, "Color"), methodInvoke);
                CodeAssignStatement groupAss = new CodeAssignStatement(new CodePropertyReferenceExpression(itemName, "Group"), propExpr1);
                CodeAssignStatement indexAss = new CodeAssignStatement(new CodePropertyReferenceExpression(itemName, "Index"), new CodePrimitiveExpression(baseItem.Index));

                statements.Add(varDecl);
                statements.Add(colorAss);
                statements.Add(groupAss);
                statements.Add(indexAss);

                for (int i = 0; i < baseItem.SubItems.Count; i++)
                {
                    ColorItem colorItem = baseItem.SubItems[i];
                    CodeMethodInvokeExpression colorMethodInvoke = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Drawing.Color)), "FromArgb", new CodeExpression[] { new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( colorItem.Color.R ) ), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( colorItem.Color.G ) ), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( colorItem.Color.B ) ) });
                    CodeMethodInvokeExpression childItemMethodInvoke = new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(null, itemName.FieldName), "SubItems"), "Add", new CodeExpression[] { new CodeObjectCreateExpression( typeof(Syncfusion.Windows.Forms.Tools.ColorItem), new CodeExpression[] { new CodeFieldReferenceExpression( null, itemName.FieldName ), colorMethodInvoke }) });
                    statements.Add(childItemMethodInvoke);
                }
            }
            else
            {
                CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), control);
                CodePropertyReferenceExpression propExpr2 = new CodePropertyReferenceExpression(fieldRef, groupName);
                CodePropertyReferenceExpression propExpr3 = new CodePropertyReferenceExpression(propExpr2, "Items");

                CodeArrayIndexerExpression arrIndexer1 = new CodeArrayIndexerExpression(propExpr3, new CodeExpression[] { new CodePrimitiveExpression(item.BaseItem.Index) });
                CodeCastExpression castExpr = new CodeCastExpression(typeof(Syncfusion.Windows.Forms.Tools.GroupColorItem), arrIndexer1);
                CodePropertyReferenceExpression propExpr4 = new CodePropertyReferenceExpression(castExpr, "SubItems");
                CodeMethodInvokeExpression colorMethodInvoke = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(System.Drawing.Color)), "FromArgb", new CodeExpression[] { new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( item.Color.R ) ), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( item.Color.G ) ),	new CodeCastExpression(typeof(byte), new CodePrimitiveExpression( item.Color.B ) ) });
                CodeObjectCreateExpression objCreate = new CodeObjectCreateExpression(typeof(Syncfusion.Windows.Forms.Tools.ColorItem), new CodeExpression[] { castExpr, colorMethodInvoke });
                CodeMethodInvokeExpression itemMethodInvoke = new CodeMethodInvokeExpression(propExpr4, "Add", new CodeExpression[] { objCreate });

                statements.Add(itemMethodInvoke);
            }

            return statements;
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private CodeStatementCollection GetDefaultGroupsStatements(ColorPickerUIAdv colorPicker, string controlName)
        {
            CodeStatementCollection statements = new CodeStatementCollection();

            CodeStatementCollection rCol = null;
            CodeStatementCollection sCol = null;
            CodeStatementCollection tCol = null;

            foreach (ColorUIAdvGroup group in colorPicker.Groups)
            {
                if (group.GroupType == ColorUIAdvGroups.RecentColors)
                    rCol = this.GetGroupStatements(controlName, group);
                else if (group.GroupType == ColorUIAdvGroups.StandardColors)
                    sCol = this.GetGroupStatements(controlName, group);
                else if (group.GroupType == ColorUIAdvGroups.ThemeColors)
                    tCol = this.GetGroupStatements(controlName, group);
            }

            for (int i = 0; i < rCol.Count; i++)
                statements.Add(rCol[i]);

            for (int i = 0; i < sCol.Count; i++)
                statements.Add(sCol[i]);

            for (int i = 0; i < tCol.Count; i++)
                statements.Add(tCol[i]);

            return statements;
        }

        private CodeStatementCollection GetGroupStatements(string controlName, ColorUIAdvGroup group)
        {
            CodeStatementCollection statements = new CodeStatementCollection();

            string groupName = string.Empty;
            if (group.GroupType == ColorUIAdvGroups.RecentColors)
                groupName = "RecentGroup";
            else if (group.GroupType == ColorUIAdvGroups.StandardColors)
                groupName = "StandardGroup";
            else
                groupName = "ThemeGroup";

            statements.Add(new CodeCommentStatement(string.Empty));
            statements.Add(new CodeCommentStatement(controlName + "." + groupName));
            statements.Add(new CodeCommentStatement(string.Empty ));

            CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), controlName);
            CodePropertyReferenceExpression propExpr1 = new CodePropertyReferenceExpression(fieldRef, groupName);

            CodeAssignStatement assStatement;
            CodePropertyReferenceExpression propRef;

            if (group.HeaderHeight != ColorUIAdvGroup.DEF_HEADERHEIGHT)
            {
                propRef = new CodePropertyReferenceExpression(propExpr1, "HeaderHeight");
                assStatement = new CodeAssignStatement(propRef, new CodePrimitiveExpression(group.HeaderHeight));

                statements.Add(assStatement);
            }

            if (group.IsSubItemsVisible != ColorUIAdvGroup.DEF_ISSUBITEMSVISIBLE)
            {
                propRef = new CodePropertyReferenceExpression(propExpr1, "IsSubItemsVisible");
                assStatement = new CodeAssignStatement(propRef, new CodePrimitiveExpression(group.IsSubItemsVisible));

                statements.Add(assStatement);
            }

            if (group.Name != ColorUIAdvGroup.DEF_GROUPNAME)
            {
                propRef = new CodePropertyReferenceExpression(propExpr1, "Name");
                assStatement = new CodeAssignStatement(propRef, new CodePrimitiveExpression(group.Name));

                statements.Add(assStatement);
            }

            if (group.SubItemsDepth != ColorUIAdvGroup.DEF_SUBITEMSDEPTH)
            {
                propRef = new CodePropertyReferenceExpression(propExpr1, "SubItemsDepth");
                assStatement = new CodeAssignStatement(propRef, new CodePrimitiveExpression(group.SubItemsDepth));

                statements.Add(assStatement);
            }

            if (group.Visible != ColorUIAdvGroup.DEF_VISIBLE)
            {
                propRef = new CodePropertyReferenceExpression(propExpr1, "Visible");
                assStatement = new CodeAssignStatement(propRef, new CodePrimitiveExpression(group.Visible));

                statements.Add(assStatement);
            }

            return statements;
        }
#endif

        private bool NonGroupColorItemsAdded(ColorUIAdvGroup group, ref ArrayList listOfItems)
        {
            bool itemsAdded = false;

            for (int i = 0; i < group.Items.Count; i++)
            {
                GroupColorItem groupItem = group.Items[i] as GroupColorItem;

                int startIndex = 0;

                if (group.GroupType == ColorUIAdvGroups.ThemeColors)
                    startIndex = ColorPickerUIAdv.DEF_SUBITEMSCOUNT;

                for (int j = startIndex; j < groupItem.SubItems.Count; j++)
                {
                    listOfItems.Add(groupItem.SubItems[j]);
                    itemsAdded = true;
                }
            }

            return itemsAdded;
        }

        private class ItemComparer : IComparer
        {
            public ItemComparer()
            {
            }

            public int Compare(object item1, object item2)
            {
                CompareInfo ci;

                if (item1 == null)
                {
                    if (item2 == null)
                        return 0;
                    return -1;
                }
                if (item2 == null)
                    return 1;

                ColorItem colorItem1 = item1 as ColorItem;
                ColorItem colorItem2 = item2 as ColorItem;

                GroupColorItem baseColorItem1 = colorItem1 as GroupColorItem;
                GroupColorItem baseColorItem2 = colorItem2 as GroupColorItem;

                string item1Index;
                string item2Index;

                if (baseColorItem1 != null)
                    item1Index = baseColorItem1.Index.ToString();
                else
                    item1Index = colorItem1.BaseItem.Index.ToString();

                if (baseColorItem2 != null)
                    item2Index = baseColorItem2.Index.ToString();
                else
                    item2Index = colorItem2.BaseItem.Index.ToString();

                if (baseColorItem1 != null && baseColorItem2 == null &&
                    item1Index == item2Index)
                    return -1;

                if (baseColorItem1 == null && baseColorItem2 != null &&
                    item1Index == item2Index)
                    return 1;

                ci = Application.CurrentCulture.CompareInfo;
                return ci.Compare(item1Index, item2Index, CompareOptions.StringSort);
            }
        }
    }

    [Serializable]
    public enum ColorUIAdvGroups
    {
        /// <summary>
        /// Represents Theme Colors
        /// </summary>
        ThemeColors,

        /// <summary>
        /// Represents tandard Colors
        /// </summary>
        StandardColors,

        /// <summary>
        /// Represents Recent Colors
        /// </summary>
        RecentColors,

        /// <summary>
        /// represents Custom Colors
        /// </summary>
        CustomColors
    }

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
    [ToolboxItem(false)]
	[DesignTimeVisible(false)]
	public class ColorUIGroup : Component
    { }
#else
    public class ColorUIGroup
    { 
    }
#endif

    [Serializable]
    [TypeConverter(typeof(ColorUIAdvGroupTypeConverter))]
    public class ColorUIAdvGroup :
        ColorUIGroup
    {
        internal const string DEF_GROUPNAME = "CustomGroup";
        private const int DEF_GROUPWIDTH = 172;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        internal const int DEF_HEADERHEIGHT = 20;
        internal const bool DEF_ISSUBITEMSVISIBLE = false;
        internal const int DEF_SUBITEMSDEPTH = 5;
        internal const bool DEF_VISIBLE = true;
#endif

        // Theme colors
        private Color thColor0 = Color.White;
        private Color thColor1 = Color.Black;
        private Color thColor2 = Color.FromArgb(237, 233, 229);
        private Color thColor3 = Color.FromArgb(30, 75, 120);
        private Color thColor4 = Color.FromArgb(78, 131, 185);
        private Color thColor5 = Color.FromArgb(190, 83, 75);
        private Color thColor6 = Color.FromArgb(154, 187, 88);
        private Color thColor7 = Color.FromArgb(127, 102, 160);
        private Color thColor8 = Color.FromArgb(77, 171, 200);
        private Color thColor9 = Color.FromArgb(245, 154, 64);

        private Color thSub0Color0 = Color.FromArgb(239, 239, 239);
        private Color thSub0Color1 = Color.FromArgb(214, 214, 214);
        private Color thSub0Color2 = Color.FromArgb(189, 189, 189);
        private Color thSub0Color3 = Color.FromArgb(165, 165, 165);
        private Color thSub0Color4 = Color.FromArgb(133, 133, 133);

        private Color thSub1Color0 = Color.FromArgb(123, 123, 123);
        private Color thSub1Color1 = Color.FromArgb(91, 91, 91);
        private Color thSub1Color2 = Color.FromArgb(58, 58, 58);
        private Color thSub1Color3 = Color.FromArgb(42, 42, 42);
        private Color thSub1Color4 = Color.FromArgb(9, 9, 9);

        private Color thSub2Color0 = Color.FromArgb(222, 215, 198);
        private Color thSub2Color1 = Color.FromArgb(198, 190, 148);
        private Color thSub2Color2 = Color.FromArgb(148, 142, 82);
        private Color thSub2Color3 = Color.FromArgb(74, 69, 41);
        private Color thSub2Color4 = Color.FromArgb(24, 28, 16);

        private Color thSub3Color0 = Color.FromArgb(206, 223, 239);
        private Color thSub3Color1 = Color.FromArgb(140, 182, 222);
        private Color thSub3Color2 = Color.FromArgb(90, 142, 206);
        private Color thSub3Color3 = Color.FromArgb(24, 60, 90);
        private Color thSub3Color4 = Color.FromArgb(16, 36, 66);

        private Color thSub4Color0 = Color.FromArgb(214, 223, 239);
        private Color thSub4Color1 = Color.FromArgb(189, 207, 222);
        private Color thSub4Color2 = Color.FromArgb(148, 182, 214);
        private Color thSub4Color3 = Color.FromArgb(57, 93, 140);
        private Color thSub4Color4 = Color.FromArgb(41, 60, 90);

        private Color thSub5Color0 = Color.FromArgb(239, 223, 222);
        private Color thSub5Color1 = Color.FromArgb(231, 182, 181);
        private Color thSub5Color2 = Color.FromArgb(214, 150, 148);
        private Color thSub5Color3 = Color.FromArgb(148, 52, 49);
        private Color thSub5Color4 = Color.FromArgb(99, 36, 33);

        private Color thSub6Color0 = Color.FromArgb(239, 239, 222);
        private Color thSub6Color1 = Color.FromArgb(214, 231, 181);
        private Color thSub6Color2 = Color.FromArgb(198, 215, 156);
        private Color thSub6Color3 = Color.FromArgb(115, 150, 57);
        private Color thSub6Color4 = Color.FromArgb(82, 101, 41);

        private Color thSub7Color0 = Color.FromArgb(231, 223, 239);
        private Color thSub7Color1 = Color.FromArgb(198, 199, 214);
        private Color thSub7Color2 = Color.FromArgb(181, 166, 198);
        private Color thSub7Color3 = Color.FromArgb(90, 77, 123);
        private Color thSub7Color4 = Color.FromArgb(57, 52, 74);

        private Color thSub8Color0 = Color.FromArgb(214, 239, 247);
        private Color thSub8Color1 = Color.FromArgb(181, 223, 231);
        private Color thSub8Color2 = Color.FromArgb(148, 207, 222);
        private Color thSub8Color3 = Color.FromArgb(49, 134, 156);
        private Color thSub8Color4 = Color.FromArgb(24, 93, 107);

        private Color thSub9Color0 = Color.FromArgb(247, 231, 214);
        private Color thSub9Color1 = Color.FromArgb(255, 215, 181);
        private Color thSub9Color2 = Color.FromArgb(247, 190, 140);
        private Color thSub9Color3 = Color.FromArgb(231, 109, 8);
        private Color thSub9Color4 = Color.FromArgb(156, 77, 8);

        // Standart colors
        private Color stColor0 = Color.FromArgb(192, 3, 0);
        private Color stColor1 = Color.FromArgb(253, 2, 0);
        private Color stColor2 = Color.FromArgb(254, 192, 0);
        private Color stColor3 = Color.FromArgb(253, 255, 0);
        private Color stColor4 = Color.FromArgb(148, 205, 82);
        private Color stColor5 = Color.FromArgb(1, 178, 75);
        private Color stColor6 = Color.FromArgb(0, 177, 239);
        private Color stColor7 = Color.FromArgb(2, 112, 191);
        private Color stColor8 = Color.FromArgb(0, 36, 89);
        private Color stColor9 = Color.FromArgb(113, 51, 154);

        internal Color[] M_themeColors = new Color[10];
        internal Color[,] M_themeSubColors = new Color[10, 5];
        internal Color[] M_standardColors = new Color[10];

        private ColorItemCollection m_items = null;
        private ColorPickerUIAdv m_control = null;
        private LabelItem m_label = null;
        private Size m_baseColorsArea = Size.Empty;
        private Size m_inheritColorsArea = Size.Empty;
        private bool m_bIsInheritColorAreaVisible = false;
        private ColorUIAdvGroups m_groupType = ColorUIAdvGroups.CustomColors;
        private int m_headerHeight = 20;
        private Rectangle m_bounds = Rectangle.Empty;
        private string m_name = SR.GetString(SR.ColorPickerCustomColorsGroup);
        private bool m_bVisible = true;
        private int m_subItemsDepth = 5;
        private int m_index = -1;
        private Size m_size = Size.Empty;
        private bool m_bIsDefaultGroup = false;

        /// <summary>
        /// Gets collection of base ColorItems for this group.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Gets collection of base ColorItems for this group.")]
        public ColorItemCollection Items
        {
            get { return m_items; }
        }

        protected bool ShouldSerializeItems()
        {
            return !this.IsDefaultGroup;
        }

        /// <summary>
        /// Gets or sets type of the Group.
        /// </summary>
        internal ColorUIAdvGroups GroupType
        {
            get
            {
                return m_groupType; 
            }

            set
            {
                if (m_groupType != value)
                {
                    m_groupType = value;

                    switch (m_groupType)
                    {
                        case ColorUIAdvGroups.ThemeColors:
                            this.Name = SR.GetString(SR.ColorPickerThemeColorsGroup);
                            this.IsSubItemsVisible = true;
                            break;

                        case ColorUIAdvGroups.StandardColors:
                            this.Name = SR.GetString(SR.ColorPickerStandardColorsGroup);
                            this.IsSubItemsVisible = false;
                            break;

                        case ColorUIAdvGroups.RecentColors:
                            this.Name = SR.GetString(SR.ColorPickerRecentColorsGroup);
                            this.IsSubItemsVisible = false;
                            this.Visible = false;
                            break;

                        default:
                            this.Name = SR.GetString(SR.ColorPickerCustomColorsGroup);
                            break;
                    }

                    if (this.Items != null)
                    {
                        this.Items.Clear();
                        this.InitColors();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Group's header height.
        /// </summary>
        [DefaultValue(20)]
        [Description("Getsor sets Group's header height.")]
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

                    this.UpdateGroupSize();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Bounds
        {
            get 
            {
                return m_bounds; 
            }
            set
            {
                if (m_bounds != value)
                {
                    m_bounds = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets group's name.
        /// </summary>
        [DefaultValue(DEF_GROUPNAME)]
        [Description("Gets or sets group's name.")]
        public string Name
        {
            get 
            {
                return m_name; 
            }
            set
            {
                if (m_name != value)
                {
                    m_name = value;

                    if (this.ParentControl != null)
                    {
                        this.ParentControl.Invalidate(this.Bounds);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether group is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Indicates whether group is visible.")]
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
                    m_bVisible = value;

                    if (this.ParentControl != null)
                        this.ParentControl.UpdateControl();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size Size
        {
            get 
            {
                return m_size; 
            }
            set
            {
                if (m_size != value)
                {
                    m_size = value;

                    if (this.ParentControl != null && this.ShouldChangeParentSize)
                        this.ParentControl.UpdateControl();
                }
            }
        }

        private bool m_bShouldChangeParentSize = true;
        internal bool ShouldChangeParentSize
        {
            get { return m_bShouldChangeParentSize; }
            set { m_bShouldChangeParentSize = value; }
        }

        /// <summary>
        /// Gets or sets count of visible SubColorItems.
        /// </summary>
        [DefaultValue(5)]
        [Description("Gets or sets count of visible SubColorItems.")]
        public int SubItemsDepth
        {
            get 
            {
                return m_subItemsDepth; 
            }
            set
            {
                if (m_subItemsDepth != value)
                {
                    m_subItemsDepth = value;

                    this.UpdateGroupSize();
                }
            }
        }

        [DefaultValue(-1)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Index
        {
            get 
            {
                return m_index; 
            }
            set
            {
                if (m_index != value)
                {
                    m_index = value;

                    if (this.ParentControl != null)
                        this.ParentControl.UpdateControl();
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorPickerUIAdv ParentControl
        {
            get { return m_control; }
            set { m_control = value; }
        }

        [Browsable(false)]
        public LabelItem Header
        {
            get { return m_label; }
        }

        [Browsable(false)]
        internal Size BaseColorsArea
        {
            get { return m_baseColorsArea; }
        }

        [Browsable(false)]
        internal Size InheritColorsArea
        {
            get { return m_inheritColorsArea; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether SubColorItems should be visible of hidden.
        /// </summary>
        [DefaultValue(false)]
        [Description("Specifies whether SubColorItems should be visible of hidden.")]
        public bool IsSubItemsVisible
        {
            get 
            {
                return m_bIsInheritColorAreaVisible; 
            }
            set
            {
                if (m_bIsInheritColorAreaVisible != value)
                {
                    m_bIsInheritColorAreaVisible = value;

                    UpdateGroupSize();
                }
            }
        }

        private bool m_bShouldSerialize = false;
        [Browsable(false)]
        [DefaultValue(false)]
        internal bool ShouldSerialize
        {
            get 
            {
                return m_bShouldSerialize; 
            }
            set
            {
                if (m_bShouldSerialize != value)
                    m_bShouldSerialize = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsDefaultGroup
        {
            get { return m_bIsDefaultGroup; }
        }

        internal void CalculateItemsBounds()
        {
            if (this.Items != null && this.Items.Count != 0 && this.ParentControl != null)
            {
                Rectangle drawRect = this.Bounds;

                int xOffcet = this.ParentControl.BorderOffset;
                int yOffcet = drawRect.Y;

                if (this.Header.Dock == DockStyle.Top)
                    yOffcet += this.HeaderHeight;

                Point location = Point.Empty;
                Size size = this.ParentControl.ColorItemSize;

                int defYOffcet = yOffcet;
                if (this.IsSubItemsVisible)
                {
                    defYOffcet += this.ParentControl.BorderOffset + this.ParentControl.ColorItemSize.Height;

                    if (this.ParentControl.VerticalItemsSpacing < ColorPickerUIAdv.DEF_BASECOLORSOFFSET)
                        defYOffcet += ColorPickerUIAdv.DEF_BASECOLORSOFFSET;
                    else
                        defYOffcet += this.ParentControl.VerticalItemsSpacing;
                }
                else
                {
                    defYOffcet += this.ParentControl.BorderOffset * 2 + this.ParentControl.ColorItemSize.Height;
                }

                Rectangle inheritArea = new Rectangle(new Point(0, defYOffcet), new Size(this.Size.Width, this.Size.Height - defYOffcet));

                int index = -1;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    int foundIndex = this.FindNextItem(index++, true);

                    GroupColorItem item = null;
                    if (foundIndex != -1)
                    {
                        item = this.Items[foundIndex] as GroupColorItem;
                    }
                    else
                    {
                        break;
                    }

                    if (this.Visible)
                    {
                        location = new Point(xOffcet + (size.Width + this.ParentControl.HorizontalItemsSpacing) * index, yOffcet + this.ParentControl.BorderOffset);
                        Rectangle rect = new Rectangle(location, size);

                        item.Bounds = rect;
                    }
                    else
                        item.Bounds = Rectangle.Empty;

                    if (this.IsSubItemsVisible)
                    {
                        this.CalculateInheritItemsBounds(item, inheritArea);
                    }
                }
            }
        }

        private void CalculateInheritItemsBounds(GroupColorItem colorItem, Rectangle inheritArea)
        {
            Point location = Point.Empty;
            Size size = this.ParentControl.ColorItemSize;

            int yOffcet = inheritArea.Location.Y;

            ColorItemCollection col = colorItem.SubItems;

            if (col != null && col.Count > 0)
            {
                int x = this.ParentControl.BorderOffset + (this.ParentControl.HorizontalItemsSpacing + size.Width) * colorItem.Index;

                for (int i = 0; i < this.SubItemsDepth; i++)
                {
                    int y = yOffcet + (this.ParentControl.VerticalItemsSpacing + size.Height) * i;

                    Rectangle rect = new Rectangle(x, y, size.Width, size.Height);

                    if (i < col.Count)
                    {
                        ColorItem item = col[i];

                        if (this.Visible)
                            item.Bounds = rect;
                        else
                            item.Bounds = Rectangle.Empty;
                    }
                }

                if (this.SubItemsDepth < col.Count)
                {
                    for (int i = this.SubItemsDepth; i < col.Count; i++)
                        col[i].Bounds = Rectangle.Empty;
                }
            }
        }

        internal void UpdateGroupSize()
        {
            int groupWidth = 0;
            int groupHeight = this.HeaderHeight;

            if (this.ParentControl != null)
            {
                Size colorItemSize = m_control.ColorItemSize;
                int nItems = 0;

                if (this.Items != null)
                {
                    nItems = this.Items.Count;
                }

                int baseAreaWidth = this.ParentControl.BorderOffset * 2 + colorItemSize.Width * nItems + this.ParentControl.HorizontalItemsSpacing * (nItems - 1);
                int baseAreaHeight = 0;

                if (this.IsSubItemsVisible)
                {
                    baseAreaHeight = this.ParentControl.BorderOffset + colorItemSize.Height;

                    if (this.ParentControl.VerticalItemsSpacing < ColorPickerUIAdv.DEF_BASECOLORSOFFSET)
                        baseAreaHeight += ColorPickerUIAdv.DEF_BASECOLORSOFFSET;
                    else
                        baseAreaHeight += this.ParentControl.VerticalItemsSpacing;
                }
                else
                {
                    baseAreaHeight = this.ParentControl.BorderOffset * 2 + colorItemSize.Height;
                }

                m_baseColorsArea = new Size(baseAreaWidth, baseAreaHeight);

                int inheritAreaHeight = this.ParentControl.BorderOffset + colorItemSize.Height * this.SubItemsDepth + this.ParentControl.VerticalItemsSpacing * (this.SubItemsDepth - 1);

                m_inheritColorsArea = new Size(m_baseColorsArea.Width, inheritAreaHeight);

                groupHeight += m_baseColorsArea.Height;
                groupWidth = m_baseColorsArea.Width;

                if (this.IsSubItemsVisible)
                    groupHeight += m_inheritColorsArea.Height;

                this.Size = new Size(groupWidth, groupHeight);

                if (m_label != null && (m_label.Dock == DockStyle.Top || m_label.Dock == DockStyle.Bottom))
                {
                    m_label.Size = new Size(this.Size.Width, this.HeaderHeight);
                }
            }
        }

        internal int FindNextItem(int index, bool movingRight)
        {
            if (movingRight)
            {
                index++;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (this.Items[i].Index == index)
                        return i;
                }
            }
            else
            {
                index--;

                for (int i = this.Items.Count - 1; i >= 0; i--)
                {
                    if (this.Items[i].Index == index)
                        return i;
                }
            }

            return -1;
        }

        public ColorUIAdvGroup(ColorPickerUIAdv control, ColorUIAdvGroups group, bool isDesignMode)
        {
            m_control = control;
            this.GroupType = group;

            Init(isDesignMode);

            InitColors();
        }

        public ColorUIAdvGroup(ColorPickerUIAdv control, ColorUIAdvGroups group, bool isDesignMode, bool isDefault)
            : this(control, group, isDesignMode)
        {
            m_bIsDefaultGroup = isDefault;
        }

        private void Init(bool isDesignMode)
        {
            m_label = new LabelItem(this);

            if (m_items == null)
                m_items = new ColorItemCollection(this, true);

            if (!isDesignMode)
            {
                if (this.GroupType == ColorUIAdvGroups.ThemeColors)
                {
                    this.IsSubItemsVisible = true;
                }
                else if (this.GroupType == ColorUIAdvGroups.RecentColors)
                {
                    this.Visible = false;
                }
            }

            this.UpdateGroupSize();
        }

        private void InitColors()
        {
            if (m_groupType == ColorUIAdvGroups.ThemeColors)
            {
                M_themeColors[0] = thColor0;
                M_themeColors[1] = thColor1;
                M_themeColors[2] = thColor2;
                M_themeColors[3] = thColor3;
                M_themeColors[4] = thColor4;
                M_themeColors[5] = thColor5;
                M_themeColors[6] = thColor6;
                M_themeColors[7] = thColor7;
                M_themeColors[8] = thColor8;
                M_themeColors[9] = thColor9;

                M_themeSubColors[0, 0] = thSub0Color0;
                M_themeSubColors[0, 1] = thSub0Color1;
                M_themeSubColors[0, 2] = thSub0Color2;
                M_themeSubColors[0, 3] = thSub0Color3;
                M_themeSubColors[0, 4] = thSub0Color4;

                M_themeSubColors[1, 0] = thSub1Color0;
                M_themeSubColors[1, 1] = thSub1Color1;
                M_themeSubColors[1, 2] = thSub1Color2;
                M_themeSubColors[1, 3] = thSub1Color3;
                M_themeSubColors[1, 4] = thSub1Color4;

                M_themeSubColors[2, 0] = thSub2Color0;
                M_themeSubColors[2, 1] = thSub2Color1;
                M_themeSubColors[2, 2] = thSub2Color2;
                M_themeSubColors[2, 3] = thSub2Color3;
                M_themeSubColors[2, 4] = thSub2Color4;

                M_themeSubColors[3, 0] = thSub3Color0;
                M_themeSubColors[3, 1] = thSub3Color1;
                M_themeSubColors[3, 2] = thSub3Color2;
                M_themeSubColors[3, 3] = thSub3Color3;
                M_themeSubColors[3, 4] = thSub3Color4;

                M_themeSubColors[4, 0] = thSub4Color0;
                M_themeSubColors[4, 1] = thSub4Color1;
                M_themeSubColors[4, 2] = thSub4Color2;
                M_themeSubColors[4, 3] = thSub4Color3;
                M_themeSubColors[4, 4] = thSub4Color4;

                M_themeSubColors[5, 0] = thSub5Color0;
                M_themeSubColors[5, 1] = thSub5Color1;
                M_themeSubColors[5, 2] = thSub5Color2;
                M_themeSubColors[5, 3] = thSub5Color3;
                M_themeSubColors[5, 4] = thSub5Color4;

                M_themeSubColors[6, 0] = thSub6Color0;
                M_themeSubColors[6, 1] = thSub6Color1;
                M_themeSubColors[6, 2] = thSub6Color2;
                M_themeSubColors[6, 3] = thSub6Color3;
                M_themeSubColors[6, 4] = thSub6Color4;

                M_themeSubColors[7, 0] = thSub7Color0;
                M_themeSubColors[7, 1] = thSub7Color1;
                M_themeSubColors[7, 2] = thSub7Color2;
                M_themeSubColors[7, 3] = thSub7Color3;
                M_themeSubColors[7, 4] = thSub7Color4;

                M_themeSubColors[8, 0] = thSub8Color0;
                M_themeSubColors[8, 1] = thSub8Color1;
                M_themeSubColors[8, 2] = thSub8Color2;
                M_themeSubColors[8, 3] = thSub8Color3;
                M_themeSubColors[8, 4] = thSub8Color4;

                M_themeSubColors[9, 0] = thSub9Color0;
                M_themeSubColors[9, 1] = thSub9Color1;
                M_themeSubColors[9, 2] = thSub9Color2;
                M_themeSubColors[9, 3] = thSub9Color3;
                M_themeSubColors[9, 4] = thSub9Color4;

                GroupColorItem item1 = new GroupColorItem(this, thColor0);
                item1.SubItems.Add(new ColorItem(item1, thSub0Color0));
                item1.SubItems.Add(new ColorItem(item1, thSub0Color1));
                item1.SubItems.Add(new ColorItem(item1, thSub0Color2));
                item1.SubItems.Add(new ColorItem(item1, thSub0Color3));
                item1.SubItems.Add(new ColorItem(item1, thSub0Color4));

                GroupColorItem item2 = new GroupColorItem(this, thColor1);
                item2.SubItems.Add(new ColorItem(item2, thSub1Color0));
                item2.SubItems.Add(new ColorItem(item2, thSub1Color1));
                item2.SubItems.Add(new ColorItem(item2, thSub1Color2));
                item2.SubItems.Add(new ColorItem(item2, thSub1Color3));
                item2.SubItems.Add(new ColorItem(item2, thSub1Color4));

                GroupColorItem item3 = new GroupColorItem(this, thColor2);
                item3.SubItems.Add(new ColorItem(item3, thSub2Color0));
                item3.SubItems.Add(new ColorItem(item3, thSub2Color1));
                item3.SubItems.Add(new ColorItem(item3, thSub2Color2));
                item3.SubItems.Add(new ColorItem(item3, thSub2Color3));
                item3.SubItems.Add(new ColorItem(item3, thSub2Color4));

                GroupColorItem item4 = new GroupColorItem(this, thColor3);
                item4.SubItems.Add(new ColorItem(item4, thSub3Color0));
                item4.SubItems.Add(new ColorItem(item4, thSub3Color1));
                item4.SubItems.Add(new ColorItem(item4, thSub3Color2));
                item4.SubItems.Add(new ColorItem(item4, thSub3Color3));
                item4.SubItems.Add(new ColorItem(item4, thSub3Color4));

                GroupColorItem item5 = new GroupColorItem(this, thColor4);
                item5.SubItems.Add(new ColorItem(item5, thSub4Color0));
                item5.SubItems.Add(new ColorItem(item5, thSub4Color1));
                item5.SubItems.Add(new ColorItem(item5, thSub4Color2));
                item5.SubItems.Add(new ColorItem(item5, thSub4Color3));
                item5.SubItems.Add(new ColorItem(item5, thSub4Color4));

                GroupColorItem item6 = new GroupColorItem(this, thColor5);
                item6.SubItems.Add(new ColorItem(item6, thSub5Color0));
                item6.SubItems.Add(new ColorItem(item6, thSub5Color1));
                item6.SubItems.Add(new ColorItem(item6, thSub5Color2));
                item6.SubItems.Add(new ColorItem(item6, thSub5Color3));
                item6.SubItems.Add(new ColorItem(item6, thSub5Color4));

                GroupColorItem item7 = new GroupColorItem(this, thColor6);
                item7.SubItems.Add(new ColorItem(item7, thSub6Color0));
                item7.SubItems.Add(new ColorItem(item7, thSub6Color1));
                item7.SubItems.Add(new ColorItem(item7, thSub6Color2));
                item7.SubItems.Add(new ColorItem(item7, thSub6Color3));
                item7.SubItems.Add(new ColorItem(item7, thSub6Color4));

                GroupColorItem item8 = new GroupColorItem(this, thColor7);
                item8.SubItems.Add(new ColorItem(item8, thSub7Color0));
                item8.SubItems.Add(new ColorItem(item8, thSub7Color1));
                item8.SubItems.Add(new ColorItem(item8, thSub7Color2));
                item8.SubItems.Add(new ColorItem(item8, thSub7Color3));
                item8.SubItems.Add(new ColorItem(item8, thSub7Color4));

                GroupColorItem item9 = new GroupColorItem(this, thColor8);
                item9.SubItems.Add(new ColorItem(item9, thSub8Color0));
                item9.SubItems.Add(new ColorItem(item9, thSub8Color1));
                item9.SubItems.Add(new ColorItem(item9, thSub8Color2));
                item9.SubItems.Add(new ColorItem(item9, thSub8Color3));
                item9.SubItems.Add(new ColorItem(item9, thSub8Color4));

                GroupColorItem item10 = new GroupColorItem(this, thColor9);
                item10.SubItems.Add(new ColorItem(item10, thSub9Color0));
                item10.SubItems.Add(new ColorItem(item10, thSub9Color1));
                item10.SubItems.Add(new ColorItem(item10, thSub9Color2));
                item10.SubItems.Add(new ColorItem(item10, thSub9Color3));
                item10.SubItems.Add(new ColorItem(item10, thSub9Color4));

                this.Items.Add(item1);
                this.Items.Add(item2);
                this.Items.Add(item3);
                this.Items.Add(item4);
                this.Items.Add(item5);
                this.Items.Add(item6);
                this.Items.Add(item7);
                this.Items.Add(item8);
                this.Items.Add(item9);
                this.Items.Add(item10);
            }
            else if (m_groupType == ColorUIAdvGroups.StandardColors)
            {
                M_standardColors[0] = stColor0;
                M_standardColors[1] = stColor1;
                M_standardColors[2] = stColor2;
                M_standardColors[3] = stColor3;
                M_standardColors[4] = stColor4;
                M_standardColors[5] = stColor5;
                M_standardColors[6] = stColor6;
                M_standardColors[7] = stColor7;
                M_standardColors[8] = stColor8;
                M_standardColors[9] = stColor9;

                this.Items.Add(new GroupColorItem(this, stColor0));
                this.Items.Add(new GroupColorItem(this, stColor1));
                this.Items.Add(new GroupColorItem(this, stColor2));
                this.Items.Add(new GroupColorItem(this, stColor3));
                this.Items.Add(new GroupColorItem(this, stColor4));
                this.Items.Add(new GroupColorItem(this, stColor5));
                this.Items.Add(new GroupColorItem(this, stColor6));
                this.Items.Add(new GroupColorItem(this, stColor7));
                this.Items.Add(new GroupColorItem(this, stColor8));
                this.Items.Add(new GroupColorItem(this, stColor9));
            }
        }
    }

    [Serializable]
    [Editor(typeof(ColorUIAdvGroupsCollectionEditor), typeof(UITypeEditor))]
    public class ColorUIAdvGroupsCollection : CollectionBase
    {
        #region Members

        private ColorPickerUIAdv m_colorPicker;

        #endregion

        #region Events

        public event EventHandler CollectionChanged;

        #endregion

        #region Constructors

        public ColorUIAdvGroupsCollection(ColorPickerUIAdv control)
        {
            if (control == null)
            {
                throw new NullReferenceException("ColorPickerUIAdv can't be NULL");
            }

            m_colorPicker = control;
        }

        #endregion

        #region Properties

        public ColorUIAdvGroup this[int index]
        {
            get
            {
                return (ColorUIAdvGroup)this.List[index];
            }
            set
            {
                if (index < 0 || index >= this.List.Count)
                {
                    throw new IndexOutOfRangeException("index");
                }
                if (value == null)
                {
                    throw new NullReferenceException("value can't be NULL");
                }

                if (this.List[index] != value)
                {
                    this.List[index] = value;
                }
            }
        }

        internal ArrayList SortedItems
        {
            get
            {
                ArrayList list = new ArrayList(this);

                list.Sort(new Comparer());

                return list;
            }
        }

        internal int MaxIndex
        {
            get
            {
                int result = -1;

                foreach (ColorUIAdvGroup group in this.InnerList)
                {
                    if (result < group.Index)
                    {
                        result = group.Index;
                    }
                }

                return result;
            }
        }
        #endregion

        #region Methods

        public void Add(ColorUIAdvGroup group)
        {
            if (group == null)
            {
                throw new NullReferenceException("Group can't be NULL");
            }

            this.List.Add(group);

            if (this == m_colorPicker.CustomGroups && !m_colorPicker.Groups.Contains(group))
                m_colorPicker.Groups.Add(group);
        }

        public bool Contains(ColorUIAdvGroup group)
        {
            if (group == null)
            {
                throw new NullReferenceException("Group can't be NULL");
            }

            return this.List.Contains(group);
        }

        public void Remove(ColorUIAdvGroup group)
        {
            if (group == null)
            {
                throw new NullReferenceException("Group can't be NULL");
            }

            if (!this.Contains(group))
            {
                throw new NullReferenceException("Group doesn't exist in collection");
            }

            this.List.Remove(group);

            if (this == m_colorPicker.CustomGroups && m_colorPicker.Groups.Contains(group))
                m_colorPicker.Groups.Remove(group);
            m_colorPicker.UpdateControl();
        }

        public int IndexOf(ColorUIAdvGroup group)
        {
            if (group == null)
            {
                throw new NullReferenceException("Group can't be NULL");
            }

            return this.List.IndexOf(group);
        }

        public void Insert(int index, ColorUIAdvGroup group)
        {
            if (group == null)
            {
                throw new NullReferenceException("Group can't be NULL");
            }

            if (index < 0 || index >= this.List.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            this.List.Insert(index, group);

            if (this == m_colorPicker.CustomGroups && !m_colorPicker.Groups.Contains(group))
                m_colorPicker.Groups.Insert(index, group);
        }

        protected void OnCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Overrides

        protected override void OnInsert(int index, object value)
        {
            ColorUIAdvGroup group = value as ColorUIAdvGroup;

            if (!this.Contains(group))
            {
                base.OnInsert(index, value);

                if (group.ParentControl == null)
                {
                    group.ParentControl = m_colorPicker;
                }

                if (group.Index < 0)
                {
                    group.Index = m_colorPicker.Groups.MaxIndex + 1;
                }

                if (this == m_colorPicker.CustomGroups && !m_colorPicker.Groups.Contains(group))
                {
                    m_colorPicker.Groups.Add(group);
                }

                this.OnCollectionChanged();
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            if (this != m_colorPicker.CustomGroups)
                m_colorPicker.UpdateControl();
        }

        protected override void OnClear()
        {
            base.OnClear();

            if (this == m_colorPicker.CustomGroups)
            {
                foreach (ColorUIAdvGroup group in this)
                {
                    if (m_colorPicker.Groups.Contains(group))
                        m_colorPicker.Groups.Remove(group);
                }
            }
        }

        #endregion

        #region Nested classes

       public class Comparer : IComparer
        {
            #region IComparer Members

            public int Compare(object x, object y)
            {
                ColorUIAdvGroup g1 = x as ColorUIAdvGroup;
                ColorUIAdvGroup g2 = y as ColorUIAdvGroup;

                if (g1.Index < g2.Index)
                    return -1;

                if (g1.Index > g2.Index)
                    return 1;

                return 0;
            }

            #endregion
        }

        #endregion
    }

    public class ColorUIAdvGroupTypeConverter :
        ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                ColorUIAdvGroup group = value as ColorUIAdvGroup;

                if (group != null && group.ParentControl != null)
                {
                    if (group.IsDefaultGroup)
                    {
                        System.Reflection.ConstructorInfo ci = typeof(ColorUIAdvGroup).GetConstructor(
                        new Type[] { typeof(ColorPickerUIAdv), typeof(ColorUIAdvGroups), typeof(bool), typeof(bool) });

                        if (group.ShouldSerialize)
                        {
                            return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { group.ParentControl, group.GroupType, true, true }, false);
                        }
                        else
                        {
                            return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { group.ParentControl, group.GroupType, true, true });
                        }                
                    }
                    else
                    {
                        System.Reflection.ConstructorInfo ci = typeof(ColorUIAdvGroup).GetConstructor(new Type[] { typeof(ColorPickerUIAdv), typeof(ColorUIAdvGroups), typeof(bool) });
                        if (group.ShouldSerialize)
                        {
                            return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { group.ParentControl, group.GroupType, true }, false);
                        }
                        else
                        {
                            return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { group.ParentControl, group.GroupType, true });
                        }   
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                return true;

            return base.CanConvertTo(context, destinationType);
        }
    }

    public class LabelItem
    {
        private readonly Size DEF_LABELSIZE = new Size(172, 20);

        private Size m_size = Size.Empty;
        public Size Size
        {
            get 
            {
                return m_size; 
            }
            set
            {
                if (m_size != value)
                    m_size = value;
            }
        }

        private DockStyle m_dockStyle = DockStyle.Top;
        [DefaultValue(DockStyle.Top)]
        public DockStyle Dock
        {
            get 
            {
                return m_dockStyle; 
            } 
        }

        public LabelItem(ColorUIAdvGroup group)
        {
            this.Size = new Size(172, group.HeaderHeight);
        }
    }
    /// <summary>
    /// CheckBoxAdv Designer
    /// </summary>
    public class ColorPickerUIDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        /// <summary>
        /// Designer ActionList collection
        /// </summary>
        private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        /// <summary>
        ///  Initializes a new instance of the CheckBoxAdvDesigner class
        /// </summary>
        public ColorPickerUIDesigner()
            : base()
        {
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

        /// <summary>
        /// Gets a value indication the designer action
        /// </summary>
        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == this.actionLists)
                {
                    this.actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    this.actionLists.Add(new ColorPickerUIActionList(this.Component));
                }

                return this.actionLists;
            }
        }

#endif
    }
    internal class ColorUIAdvGroupsCollectionEditor : CollectionEditor
    {
        private ColorPickerUIAdv m_colorPicker = null;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                m_colorPicker = context.Instance as ColorPickerUIAdv;
            }

            return base.EditValue(context, provider, value);
        }

        public ColorUIAdvGroupsCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override object CreateInstance(Type itemType)
        {
            return new ColorUIAdvGroup(m_colorPicker, ColorUIAdvGroups.CustomColors, true);
        }

        protected override void DestroyInstance(object instance)
        {
            base.DestroyInstance(instance);

            m_colorPicker.UpdateControl();
        }
    }
}