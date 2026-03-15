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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
    public class TabPrimitiveTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            TabPrimitive primitive = value as TabPrimitive;

            if (destinationType == typeof(string) && primitive != null)
            {
                return primitive.TabPrimitiveType.ToString() + "Primitive";
            }

            if ((destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
                && primitive != null)
            {
                System.Reflection.ConstructorInfo ci = typeof(TabPrimitive).GetConstructor(
                    new Type[] { typeof(TabPrimitiveType), typeof(Image), typeof(Color), typeof(bool), typeof(int), typeof(string), typeof(string) });

                return new System.ComponentModel.Design.Serialization.InstanceDescriptor(ci, new object[] { primitive.TabPrimitiveType, primitive.Image, primitive.TransparentImageColor, primitive.Visible, primitive.Indent, primitive.Name, primitive.ToolTip });
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

    [Serializable]
    public class TabPrimitivesCollection :
        CollectionBase,
        IDisposable
    {
        #region Members
        private TabPrimitivesHost m_tabPrimitivesHost = null;
        private static int m_iNameIndex = 0;
        #endregion

        #region Events
        public event EventHandler CollectionChanged;
        #endregion

        protected void OnCollectionChanged()
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, EventArgs.Empty);
            }
        }

        #region Constructors
        internal TabPrimitivesCollection(TabPrimitivesHost tabPrimitivesHost)
        {
            if (tabPrimitivesHost == null)
            {
                throw new NullReferenceException("tabPrimitivesHost can't be NULL");
            }

            m_tabPrimitivesHost = tabPrimitivesHost;
        }
        #endregion

        #region Indexer

        public TabPrimitive this[int index]
        {
            get
            {
                return (TabPrimitive)this.List[index];
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
        #endregion

        #region Methods
        public void Add(TabPrimitive tabPrimitive)
        {
            if (tabPrimitive == null)
            {
                throw new NullReferenceException("TabPrimitive can't be NULL");
            }

            this.List.Add(tabPrimitive);
        }

        public bool Contains(TabPrimitive tabPrimitive)
        {
            if (tabPrimitive == null)
            {
                throw new NullReferenceException("TabPrimitive can't be NULL");
            }

            return this.List.Contains(tabPrimitive);
        }

        public void Remove(TabPrimitive tabPrimitive)
        {
            if (tabPrimitive == null)
            {
                throw new NullReferenceException("tabPrimitive can't be NULL");
            }

            if (!this.Contains(tabPrimitive))
            {
                throw new NullReferenceException("tabPrimitive isn't included in collection");
            }

            this.List.Remove(tabPrimitive);
        }

        public int IndexOf(TabPrimitive tabPrimitive)
        {
            if (tabPrimitive == null)
            {
                throw new NullReferenceException("tabPrimitive can't be NULL");
            }

            return this.List.IndexOf(tabPrimitive);
        }

        public void Insert(int index, TabPrimitive tabPrimitive)
        {
            if (tabPrimitive == null)
            {
                throw new NullReferenceException("tabPrimitive can't be NULL");
            }

            if (index < 0 || index >= this.List.Count)
            {
                throw new IndexOutOfRangeException("index");
            }

            this.List.Insert(index, tabPrimitive);
        }
        #endregion

        #region Overrides
        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);

            TabPrimitive primitive = (TabPrimitive)value;
            if (primitive.Name == String.Empty && ((ITabControl)m_tabPrimitivesHost.TabControl).IsDesignMode())
            {
                primitive.Name = "TabPrimitive" + m_iNameIndex;
                m_iNameIndex++;
            }
            primitive.SetTabPrimitiveHost(m_tabPrimitivesHost);

            this.OnCollectionChanged();
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);

            ((TabPrimitive)value).SetTabPrimitiveHost(null);

            this.OnCollectionChanged();
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            base.OnSetComplete(index, oldValue, newValue);

            ((TabPrimitive)oldValue).SetTabPrimitiveHost(null);
            ((TabPrimitive)newValue).SetTabPrimitiveHost(m_tabPrimitivesHost);

            this.OnCollectionChanged();
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Clear();
            m_tabPrimitivesHost = null;
        }

        #endregion
    }

    public class TabPrimitiveClickEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Clicked primitive.
        /// </summary>
        private TabPrimitive m_tabPrimitive = null;

        public TabPrimitiveClickEventArgs(TabPrimitive tabPrimitive)
            : base()
        {
            m_tabPrimitive = tabPrimitive;
        }

        /// <summary>
        /// Gets the primitive that gets clicked.
        /// </summary>
        public TabPrimitive TabPrimitive
        {
            get
            {
                return m_tabPrimitive;
            }
        }
    }

    public delegate void TabPrimitiveClick(object sender, TabPrimitiveClickEventArgs e);

    [Serializable]
    [TypeConverter(typeof(TabPrimitiveTypeConverter))]
    public class TabPrimitive
    {
        #region Constants

        // LoadImageFromResourse
        private const string c_sImagePath = "Syncfusion.Windows.Forms.Tools.Controls.TabControlAdv.Images.";
        private const string c_sFirstTabImage = "FirstTab.bmp";
        private const string c_sLastTabImage = "LastTab.bmp";
        private const string c_sNextPageImage = "NextPage.bmp";
        private const string c_sNextTabImage = "NextTab.bmp";
        private const string c_sPreviousPageImage = "PreviousPage.bmp";
        private const string c_sPreviousTabImage = "PreviousTab.bmp";
        private const string c_sCloseButtonImage = "CloseButton.bmp";
        private const string c_sDropDownPartial = "DropDownPartial.bmp";
        private const string c_sDropDownFull = "DropDownFull.bmp";

        // Drawing
        private static readonly Color c_cBackgroundHot = Color.FromArgb(255, 238, 194);
        private static readonly Color c_cBackgroundPressed = Color.FromArgb(242, 210, 101);
        private static readonly Color c_cBorder = Color.FromArgb(72, 72, 109);
        #endregion

        #region Members
        /// <summary>
        /// Primitive host.
        /// </summary>
        private TabPrimitivesHost m_tabPrimitivesHost = null;

        /// <summary>
        /// This primitive visibility.
        /// </summary>
        private bool m_bVisible = true;

        /// <summary>
        /// This primitive location.
        /// </summary>
        private Point m_ptLocation = Point.Empty;

        /// <summary>
        /// This primitive size.
        /// </summary>
        private Size m_szSize = new Size(15, 15);

        /// <summary>
        /// Button state.
        /// </summary>
        private ButtonState m_state = ButtonState.Normal;

        /// <summary>
        /// Primitive type.
        /// </summary>
        private TabPrimitiveType m_type;

        /// <summary>
        /// Image for draw foreground.
        /// </summary>
        private Image m_image = null;

        /// <summary>
        /// Transparent image color.
        /// </summary>
        private Color m_cTransparentImageColor = Color.Empty;

        /// <summary>
        /// Default image.
        /// </summary>
        private Bitmap m_defaultImage = null;

        /// <summary>
        /// If can click this button.
        /// </summary>
        private bool m_bEnabled = true;

        /// <summary>
        /// Primitive indent.
        /// </summary>
        private int m_iIndent = 1;

        /// <summary>
        /// Is full mode image. Using only in DropDown primitive type.
        /// </summary>
        private bool m_bIsFullMode = false;

        /// <summary>
        /// Tab primitive name.
        /// </summary>
        private string m_sName = String.Empty;

        /// <summary>
        /// ToolTip text for the tabPrimitive.
        /// </summary>
        private string m_toolTipText = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets tab primitive name.
        /// </summary>
        [Description("Gets or sets tab primitive name.")]
        public string Name
        {
            get
            {
                return m_sName;
            }
            set
            {
                m_sName = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether fullmode or not. This property using only in DropDown primitive type.
        /// </summary>
        [Browsable(false)]
        public bool IsFullMode
        {
            get
            {
                return m_bIsFullMode;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether primitive enabled or not.
        /// If TabPrimitivType isn't Custom or DropDown then Enabled is sets automatically.
        /// </summary>
        [DefaultValue(true)]
        [Description("Gets or sets primitive enabled. If TabPrimitivType isn't Custom or DropDown then Enabled is sets automatically.")]
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
                    m_bEnabled = value;

                    if (m_tabPrimitivesHost != null)
                    {
                        m_tabPrimitivesHost.TabControl.Invalidate(this.Bounds);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets primitive indent.
        /// </summary>
        [DefaultValue(1)]
        [Description("Gets or sets primitive indent.")]
        public int Indent
        {
            get
            {
                return m_iIndent;
            }
            set
            {
                if (m_iIndent != value && value >= 0)
                {
                    m_iIndent = value;
                    if (m_tabPrimitivesHost != null)
                    {
                        m_tabPrimitivesHost.RefreshTabControl();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets transparent image color.
        /// </summary>
        [Description("Gets or sets transparent image color.")]
        public Color TransparentImageColor
        {
            get
            {
                return m_cTransparentImageColor;
            }
            set
            {
                if (m_cTransparentImageColor != value)
                {
                    m_cTransparentImageColor = value;
                    if (m_tabPrimitivesHost != null)
                    {
                        m_tabPrimitivesHost.TabControl.Invalidate(this.Bounds);
                    }
                }
            }
        }

        protected virtual bool ShouldSerialazeTransparentImageColor()
        {
            return m_cTransparentImageColor != Color.Empty;
        }

        public virtual void ResetTransparentImageColor()
        {
            m_cTransparentImageColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets image for draw foreground.
        /// If this value is null then draw default image.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets image for draw foreground. If this value is null then draw default image.")]
        public Image Image
        {
            get
            {
                return m_image;
            }
            set
            {
                if (m_image != value)
                {
                    m_image = value;

                    if (m_tabPrimitivesHost != null)
                    {
                        m_tabPrimitivesHost.TabControl.Invalidate(this.Bounds);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets primitive type.
        /// </summary>
        [DefaultValue(TabPrimitiveType.Custom)]
        [Description("Gets or sets primitive type.")]
        public TabPrimitiveType TabPrimitiveType
        {
            get
            {
                return m_type;
            }
            set
            {
                if (m_type != value)
                {
                    m_type = value;

                    m_bEnabled = true;
                    m_state = ButtonState.Normal;

                    this.LoadImageFromResourse();

                    if (this.m_tabPrimitivesHost != null)
                    {
                        this.m_tabPrimitivesHost.RefreshTabControl();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the toolTip text for this tabPrimitive.
        /// </summary>
        /// <value>The toolTip text for this tabPrimitive.</value>
        [DefaultValue("")]
        [Localizable(true)]
        [Description("Gets or sets the toolTip text for this tabPrimitive.")]
        public string ToolTip
        {
            get
            {
                return m_toolTipText;
            }
            set
            {
                if (m_toolTipText != value)
                {
                    m_toolTipText = value;
                }
            }
        }

        /// <summary>
        /// Gets primitive state.
        /// Flat == Hot.
        /// </summary>
        [Browsable(false)]
        public ButtonState State
        {
            get
            {
                return m_state;
            }
        }

        /// <summary>
        /// Gets primitive bounds.
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.m_ptLocation, this.m_szSize);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether primitive visible or not.
        /// </summary>
        [DefaultValue(true)]
        [Description("Gets or sets this primitive visibility.")]
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

                    if (m_tabPrimitivesHost != null)
                    {
                        this.m_tabPrimitivesHost.RefreshTabControl();
                    }
                }
            }
        }

        /// <summary>
        /// Gets this primitive size.
        /// </summary>
        [Browsable(false)]
        public Size Size
        {
            get
            {
                return m_szSize;
            }
        }

        /// <summary>
        /// Gets this primitive location.
        /// </summary>
        [Browsable(false)]
        public Point Location
        {
            get
            {
                return m_ptLocation;
            }
        }

        #endregion

        #region Constructors
        public TabPrimitive(TabPrimitiveType type, Image image, Color transparentImageColor, bool visible, int indent, string name, string toolTipText)
            : this(type, image, transparentImageColor, visible, indent, name)
        {
            m_toolTipText = toolTipText;
        }

        public TabPrimitive(TabPrimitiveType type, Image image, Color transparentImageColor, bool visible, int indent, string name)
            : this(type)
        {
            m_image = image;
            m_cTransparentImageColor = transparentImageColor;
            m_bVisible = visible;
            m_iIndent = indent;
            m_sName = name;
        }

        public TabPrimitive(TabPrimitiveType type)
        {
            m_type = type;

            this.LoadImageFromResourse();
        }

        public TabPrimitive()
            : this(TabPrimitiveType.Custom)
        {
        }
        #endregion

        #region Methods
        internal void SetTabPrimitiveHost(TabPrimitivesHost tabPrimitivesHost)
        {
            m_tabPrimitivesHost = tabPrimitivesHost;
        }
        internal void SetIsFullMode(bool isFullMode)
        {
            if (this.TabPrimitiveType == TabPrimitiveType.DropDown && m_bIsFullMode != isFullMode)
            {
                m_bIsFullMode = isFullMode;
                this.LoadImageFromResourse();
            }
        }

        /// <summary>
        /// Load image from resourse.
        /// </summary>
        private void LoadImageFromResourse()
        {
            if (m_type != TabPrimitiveType.Custom)
            {
                string path = c_sImagePath;

                switch (m_type)
                {
                    case TabPrimitiveType.FirstTab:
                        path += c_sFirstTabImage;
                        break;
                    case TabPrimitiveType.LastTab:
                        path += c_sLastTabImage;
                        break;
                    case TabPrimitiveType.NextPage:
                        path += c_sNextPageImage;
                        break;
                    case TabPrimitiveType.NextTab:
                        path += c_sNextTabImage;
                        break;
                    case TabPrimitiveType.PreviousPage:
                        path += c_sPreviousPageImage;
                        break;
                    case TabPrimitiveType.PreviousTab:
                        path += c_sPreviousTabImage;
                        break;
                    case TabPrimitiveType.Close:
                        path += c_sCloseButtonImage;
                        break;
                    case TabPrimitiveType.DropDown:
                        if (m_bIsFullMode)
                        {
                            path += c_sDropDownFull;
                        }
                        else
                        {
                            path += c_sDropDownPartial;
                        }
                        break;
                }

                System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
                Bitmap bm = new Bitmap(asm.GetManifestResourceStream(path));

                bm.MakeTransparent(Color.White);
                m_defaultImage = bm;
            }
            else
            {
                m_defaultImage = null;
            }
        }

        /// <summary>
        /// Calculate image rectangle for paint.
        /// </summary>
        /// <param name="bitmapSize">Image size.</param>
        /// <returns>Returns Image Rectangle</returns>
        protected virtual Rectangle CalculateImageRect(Size bitmapSize)
        {
            Rectangle bounds = this.Bounds;
            Size bmSize = bitmapSize;

            int x, y, width, height;

            if (bmSize.Width > bounds.Width)
            {
                width = bounds.Width;
                x = bounds.X;
            }
            else
            {
                width = bmSize.Width;
                x = (int)bounds.X + ((bounds.Width - width) / 2);
            }

            if (bmSize.Height > bounds.Height)
            {
                height = bounds.Height;
                y = bounds.Y;
            }
            else
            {
                height = bmSize.Height;
                y = (int)bounds.Y + ((bounds.Height - height) / 2);
            }

            return new Rectangle(x, y, width, height);
        }

        /// <summary>
        /// Draw primitive background.
        /// </summary>
        /// <param name="g">Graphics object</param>
        protected virtual void DrawBackground(Graphics g)
        {
            if (this.State != ButtonState.Normal)
            {
                Rectangle bounds = this.Bounds;

                Color color = (this.State == ButtonState.Flat) ?
                    c_cBackgroundHot : c_cBackgroundPressed;
                using(Brush brush=new SolidBrush(color))
                    g.FillRectangle(brush, bounds);
            }
        }

        /// <summary>
        /// Draw primitive border.
        /// </summary>
        /// <param name="g">Graphics object</param>
        protected virtual void DrawBorder(Graphics g)
        {
            if (this.State != ButtonState.Normal)
            {
                Rectangle bounds = this.Bounds;

                // increment width and height for fixed drawing DrawRectangle function
                bounds.Width--;
                bounds.Height--;

                g.DrawRectangle(new Pen(c_cBorder), bounds);
            }
        }

        /// <summary>
        /// Draw primitive foreground.
        /// </summary>
        /// <param name="g">Graphics object</param>
        protected virtual void DrawForeground(Graphics g)
        {
            Bitmap bm = null;

            if (this.Image != null)
            {
                bm = new Bitmap(this.Image);
                bm.MakeTransparent(this.TransparentImageColor);
            }
            else if (m_defaultImage != null)
            {
                bm = (Bitmap)m_defaultImage.Clone();
            }

            if (bm != null)
            {
                if (m_tabPrimitivesHost != null)
                {
                    if (m_tabPrimitivesHost.TabControl.IsMirrored
                        && this.TabPrimitiveType != TabPrimitiveType.Custom
                        && this.TabPrimitiveType != TabPrimitiveType.Close
                        && this.TabPrimitiveType != TabPrimitiveType.DropDown)
                    {
                        bm.RotateFlip(RotateFlipType.Rotate180FlipY);
                    }

                    if (m_tabPrimitivesHost.NeedRotate)
                    {
                        bm.RotateFlip(RotateFlipType.Rotate90FlipX);
                    }
                }

                Rectangle imageRect = CalculateImageRect(bm.Size);
                if (this.Enabled)
                {
                    g.DrawImage(bm, imageRect);
                }
                else
                {
                    ControlPaint.DrawImageDisabled(g, new Bitmap(bm, imageRect.Size), imageRect.X, imageRect.Y, Color.Transparent);
                }
            }
        }

        /// <summary>
        /// Draw primitive.
        /// </summary>
        /// <param name="g">Graphics object</param>
        protected internal virtual void Draw(Graphics g)
        {
            if (this.Visible)
            {
                if (this.Enabled)
                {
                    this.DrawBackground(g);
                    this.DrawBorder(g);
                }
                this.DrawForeground(g);
            }
        }

        internal void SetLocation(Point location)
        {
            m_ptLocation = location;
        }
        internal void SetLocation(int x, int y)
        {
            this.SetLocation(new Point(x, y));
        }

        /// <summary>
        /// Sets primitive state.
        /// </summary>
        /// <param name="state">Button State</param>
        internal void SetState(ButtonState state)
        {
            m_state = state;
        }
        #endregion
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TabPrimitivesHost :
        Disposable
    {
        #region Constants
        /// <summary>
        /// Offset of the tooltip from the mouse position.
        /// </summary>
        private readonly Point DEF_TOOLTIP_OFFSET = new Point(13, 15);

        /// <summary>
        /// Interval of showing the tooltip when mouse moves between the tabPrimitives.
        /// </summary>
        private const int DEF_TOOLTIP_TIMER_INTERVAL = 100;

        /// <summary>
        /// Initial interval of showing the toolTip.
        /// </summary>
        private const int DEF_TOOLTIP_INITIAL_TIMER_INTERVAL = 500;
        #endregion

        #region Members
        /// <summary>
        /// Tab control.
        /// </summary>
        private TabControlAdv m_tabControl = null;

        /// <summary>
        /// Collection of primitives.
        /// </summary>
        private TabPrimitivesCollection m_tabPrimitives = null;

        /// <summary>
        /// Control visibility.
        /// </summary>
        private bool m_bVisible = false;

        /// <summary>
        /// This control location.
        /// </summary>
        private Point m_ptLocation = Point.Empty;

        /// <summary>
        /// This control size.
        /// </summary>
        private Size m_szSize = Size.Empty;

        /// <summary>
        /// If layout needed.
        /// </summary>
        private bool m_bNeedLayout = true;

        /// <summary>
        /// This alignment.
        /// </summary>
        private TabPrimitiveHostAlignment m_alignment = TabPrimitiveHostAlignment.Near;

        /// <summary>
        /// Pushed primitive.
        /// </summary>
        private TabPrimitive m_pushedPrimitive = null;

        /// <summary>
        /// PopupMenu of drop down primitive.
        /// </summary>
        private PrimitiveDropDownPopupMenu m_dropDownPopupMenu = null;

        /// <summary>
        /// Used when user holds the mouse over the tabPrimitives.
        /// </summary>
        private Timer m_toolTipTimer = null;

        /// <summary>
        /// The toolTip control.
        /// </summary>
        private ToolTipAdv m_toolTip = null;

        /// <summary>
        /// The mouse position.
        /// </summary>
        private Point m_mousePosition = Point.Empty;

        /// <summary>
        /// The tabPrimitive where mouse is over.
        /// </summary>
        private int m_currentTabPrimitive = -1;

        /// <summary>
        /// Indicates whether toolTip shows for the first time.
        /// </summary>
        private bool m_bShowToolTipFirstTime = true;
        #endregion

        #region Properties

        /// <summary>
        /// Gets Collection of primitives.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Collection of primitives.")]
        public TabPrimitivesCollection TabPrimitives
        {
            get
            {
                return m_tabPrimitives;
            }
        }

        /// <summary>
        /// Gets parent tab control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabControlAdv TabControl
        {
            get
            {
                return m_tabControl;
            }
        }

        /// <summary>
        /// Gets bounds.
        /// </summary>
        [Browsable(false)]
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(this.m_ptLocation, this.m_szSize);
            }
        }

        /// <summary>
        /// Gets or sets host alignment.
        /// </summary>
        [Description("Gets or sets control alignment.")]
        public TabPrimitiveHostAlignment Alignment
        {
            get
            {
                return m_alignment;
            }
            set
            {
                if (m_alignment != value)
                {
                    m_alignment = value;
                    this.RefreshTabControl();
                }
            }
        }

        protected virtual bool ShouldSerializeAlignment()
        {
            return this.Alignment != TabPrimitiveHostAlignment.Near;
        }

        public virtual void ResetAlignment()
        {
            this.Alignment = TabPrimitiveHostAlignment.Near;
        }

        /// <summary>
        /// Gets or sets a value indicating whether control is visible or not.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets control visibility.")]
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
                    this.RefreshTabControl();
                }
            }
        }

        /// <summary>
        /// Gets this control size.
        /// </summary>
        [Browsable(false)]
        public Size Size
        {
            get
            {
                this.Layout();
                return m_szSize;
            }
        }

        /// <summary>
        /// Gets this control location.
        /// </summary>
        [Browsable(false)]
        public Point Location
        {
            get
            {
                this.Layout();
                return m_ptLocation;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether layout needed or not.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool NeedLayout
        {
            get
            {
                return m_bNeedLayout;
            }
            set
            {
                if (m_bNeedLayout != value)
                {
                    m_bNeedLayout = value;
                }
            }
        }

        /// <summary>
        /// Gets the toolTip text.
        /// </summary>
        protected string ToolTipText
        {
            get
            {
                string tooltipText = string.Empty;
                if (m_currentTabPrimitive != -1)
                {
                    tooltipText = TabPrimitives[m_currentTabPrimitive].ToolTip;
                }
                return tooltipText;
            }
        }

        /// <summary>
        /// Gets a value indicating whether tooltips should be shown.
        /// </summary>
        protected bool ShouldShowToolTips
        {
            get
            {
                bool bShouldShow = false;
                if (m_tabControl != null)
                {
                    bShouldShow = m_tabControl.ShowToolTips && !m_tabControl.IsDesignMode;
                }
                return bShouldShow;
            }
        }

        #endregion

        #region Constructors
        internal TabPrimitivesHost(TabControlAdv tabControl)
        {
            m_tabControl = tabControl;

            m_tabPrimitives = new TabPrimitivesCollection(this);
            m_tabPrimitives.CollectionChanged += new EventHandler(M_TabPrimitives_CollectionChanged);
        }
        #endregion

        #region Methods

        /// <summary>
        /// Refresh parent tab control.
        /// </summary>
        internal void RefreshTabControl()
        {
            this.m_tabControl.SetNeedLayout(true);
            this.m_tabControl.Invalidate();
        }

        /// <summary>
        /// Gets a value indicating whether rotate control on 90 degree needed.
        /// </summary>
        internal bool NeedRotate
        {
            get
            {
                return this.m_tabControl.Alignment == TabAlignment.Left || this.m_tabControl.Alignment == TabAlignment.Right;
            }
        }

        /// <summary>
        /// Calculate control size.
        /// </summary>
        protected virtual void CalculateSize()
        {
            int width = 0;
            int height = 0;

            foreach (TabPrimitive tabPrimitive in m_tabPrimitives)
            {
                if (tabPrimitive.Visible)
                {
                    width += tabPrimitive.Size.Width + tabPrimitive.Indent;
                    if (tabPrimitive.Size.Height > height)
                    {
                        height = tabPrimitive.Size.Height;
                    }
                }
            }

            m_szSize = NeedRotate ? new Size(height, width) : new Size(width, height);
        }

        /// <summary>
        /// Calculate child primitive location.
        /// </summary>
        protected virtual void CalculatePrimitivesLocation()
        {
            if (m_tabPrimitives.Count <= 0) return;

            int x = m_ptLocation.X;
            int y = m_ptLocation.Y;

            if (!this.TabControl.IsMirrored)
            {
                foreach (TabPrimitive tabPrimitive in m_tabPrimitives)
                {
                    if (tabPrimitive.Visible)
                    {
                        if (NeedRotate)
                        {
                            tabPrimitive.SetLocation(new Point(x, y));
                            y += tabPrimitive.Size.Height + tabPrimitive.Indent;
                        }
                        else
                        {
                            tabPrimitive.SetLocation(new Point(x, y));
                            x += tabPrimitive.Size.Width + tabPrimitive.Indent;
                        }
                    }
                }
            }
            else
            {
                for (int i = m_tabPrimitives.Count - 1; i >= 0; i--)
                {
                    TabPrimitive tabPrimitive = m_tabPrimitives[i];

                    if (tabPrimitive.Visible)
                    {
                        if (NeedRotate)
                        {
                            tabPrimitive.SetLocation(new Point(x, y));
                            y += tabPrimitive.Size.Height + tabPrimitive.Indent;
                        }
                        else
                        {
                            tabPrimitive.SetLocation(new Point(x, y));
                            x += tabPrimitive.Size.Width + tabPrimitive.Indent;
                        }
                    }
                }
            }
        }

        public virtual void Layout()
        {
            if (m_bNeedLayout)
            {
                this.CalculateSize();
                this.CalculatePrimitivesLocation();

                m_bNeedLayout = false;
            }
        }

        /// <summary>
        /// Refresh child primitive CanClick properties.
        /// </summary>
        public virtual void RefreshPrimitiveEnabled()
        {
            if (m_tabControl.TabCount > 0)
            {
                foreach (TabPrimitive primitive in m_tabPrimitives)
                {
                    TabPrimitiveType primitiveType = primitive.TabPrimitiveType;

                    if (primitiveType == TabPrimitiveType.FirstTab
                        || primitiveType == TabPrimitiveType.PreviousPage
                        || primitiveType == TabPrimitiveType.PreviousTab)
                    {
                        bool enabled = false;
                        for (int i = m_tabControl.SelectedIndex - 1; i >= 0; i--)
                        {
                            if (m_tabControl.TabPages[i].TabVisible)
                            {
                                enabled = true;
                            }
                        }

                        primitive.Enabled = enabled;
                    }
                    else if (primitiveType == TabPrimitiveType.LastTab
                        || primitiveType == TabPrimitiveType.NextPage
                        || primitiveType == TabPrimitiveType.NextTab)
                    {
                        bool enabled = false;
                        for (int i = m_tabControl.SelectedIndex + 1; i < m_tabControl.TabPages.Count; i++)
                        {
                            if (m_tabControl.TabPages[i].TabVisible)
                            {
                                enabled = true;
                            }
                        }

                        primitive.Enabled = enabled;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the timer.
        /// </summary>
        private void InitTimer()
        {
            if (m_toolTipTimer == null)
            {
                m_toolTipTimer = new Timer();
                m_toolTipTimer.Tick += new EventHandler(OnToolTipTimerTick);
            }
        }

        /// <summary>
        /// Initializes the tooltip.
        /// </summary>
        private void InitToolTip()
        {
            if (m_toolTip == null && m_tabControl != null)
            {
                m_toolTip = new ToolTipAdv(m_tabControl);
                m_toolTip.BackColor = SystemColors.Info;
                m_toolTip.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        /// <summary>
        /// Handles mouse hovering over the tabPrimitives.
        /// </summary>
        /// <param name="sender">Sender Object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        private void OnToolTipTimerTick(object sender, EventArgs e)
        {
            InitToolTip();

            ShowToolTip(this.ToolTipText);

            m_toolTipTimer.Stop();
        }

        /// <summary>
        /// Validates position to show tooltip in.
        /// </summary>
        /// <param name="pos">Position to check.</param>
        /// <returns>True, if position is in tab's bounds to show tooltip for, otherwise- false.</returns>
        private bool IsValidToolTipPosition(Point pos)
        {
            bool bShoudShow = false;
            Point position = m_tabControl.PointToClient(pos);

            foreach (TabPrimitive primitive in m_tabPrimitives)
            {
                if (primitive.Visible && primitive.Bounds.Contains(position))
                {
                    bShoudShow = true;
                }
            }

            return bShoudShow;
        }

        /// <summary>
        /// Shows or hides the toolTip window.
        /// </summary>
        /// <param name="text">Text to show in toolTip. If text is null or empty
        /// string, toolTip is hidden.</param>
        protected void ShowToolTip(string text)
        {
            if (m_toolTip != null)
            {
                if (text == null || text == string.Empty || !this.ShouldShowToolTips
                    || (m_dropDownPopupMenu != null && m_dropDownPopupMenu.IsShowing()))
                {
                    if (m_toolTip.Visible)
                    {
                        m_toolTip.HidePopup();
                    }
                }
                else
                {
                    Point showPos = Control.MousePosition;

                    if (IsValidToolTipPosition(showPos))
                    {
                        showPos.Offset(DEF_TOOLTIP_OFFSET.X, DEF_TOOLTIP_OFFSET.Y);
                        m_toolTip.Text = text;
                        m_toolTip.ShowPopup(showPos);
                    }
                }
            }
        }

        /// <summary>
        /// Starts showing tooltips.
        /// </summary>
        /// <param name="interval">Tootip interval</param>
        protected void StartShowingToolTip(int interval)
        {
            if (interval < 0)
            {
                throw new ArgumentException("interval");
            }

            if (m_toolTipTimer == null)
            {
                InitTimer();
            }

            if (m_toolTipTimer != null)
            {
                m_toolTipTimer.Interval = interval;
                m_toolTipTimer.Start();
            }
        }

        /// <summary>
        /// Stops showing tooltips
        /// </summary>
        protected void StopShowingToolTip()
        {
            if (m_toolTipTimer != null)
            {
                ShowToolTip(null);
                m_bShowToolTipFirstTime = true;
                m_toolTipTimer.Interval = DEF_TOOLTIP_INITIAL_TIMER_INTERVAL;
                m_toolTipTimer.Stop();
            }
        }

        internal void SetLocation(Point location)
        {
            m_ptLocation = location;
            m_bNeedLayout = true;
        }
        internal void SetLocation(int x, int y)
        {
            this.SetLocation(new Point(x, y));
        }
        internal void SetFullModeForAllDropDownPrimitives(bool isFullMode)
        {
            foreach (TabPrimitive primitive in m_tabPrimitives)
            {
                primitive.SetIsFullMode(isFullMode);
            }
        }
        #endregion

        #region Handled
        internal void HandledOnPaint(Graphics g)
        {
            if (this.Visible)
            {
                this.Layout();

                foreach (TabPrimitive primitive in m_tabPrimitives)
                {
                    primitive.Draw(g);
                }
            }
        }

        internal void HandledMouseMove(MouseEventArgs e)
        {
            int newHitTabPrimitive = -1;

            if (this.Visible && e.Button == MouseButtons.None)
            {
                m_mousePosition = new Point(e.X, e.Y);

                foreach (TabPrimitive primitive in m_tabPrimitives)
                {
                    if (primitive.Visible)
                    {
                        bool inRect = primitive.Bounds.Contains(m_mousePosition);

                        if (inRect)
                        {
                            newHitTabPrimitive = m_tabPrimitives.IndexOf(primitive);
                        }

                        if (primitive.State == ButtonState.Normal && inRect)
                        {
                            primitive.SetState(ButtonState.Flat);
                            m_tabControl.Invalidate(primitive.Bounds);
                        }
                        if (primitive.State == ButtonState.Flat && !inRect)
                        {
                            primitive.SetState(ButtonState.Normal);
                            m_tabControl.Invalidate(primitive.Bounds);
                        }
                    }
                }
            }

            if (newHitTabPrimitive != -1)
            {
                if (newHitTabPrimitive != m_currentTabPrimitive && this.ShouldShowToolTips && !m_bShowToolTipFirstTime)
                {
                    ShowToolTip(null);
                    StartShowingToolTip(DEF_TOOLTIP_TIMER_INTERVAL);
                }
            }
            else
            {
                ShowToolTip(null);
            }

            m_currentTabPrimitive = newHitTabPrimitive;
        }

        internal void HandledMouseDown(MouseEventArgs e)
        {
            if (this.Visible && m_pushedPrimitive == null)
            {
                Point pt = new Point(e.X, e.Y);

                foreach (TabPrimitive primitive in m_tabPrimitives)
                {
                    if (primitive.Visible && primitive.Enabled)
                    {
                        if (primitive.Bounds.Contains(pt))
                        {
                            primitive.SetState(ButtonState.Pushed);
                            m_pushedPrimitive = primitive;
                            m_tabControl.Invalidate(primitive.Bounds);

                            break;
                        }
                    }
                }
            }
        }

        internal void HandledMouseUp(MouseEventArgs e)
        {
            if (this.Visible)
            {
                if (m_pushedPrimitive != null && m_pushedPrimitive.Enabled)
                {
                    Point pt = new Point(e.X, e.Y);

                    bool isClick = m_pushedPrimitive.Bounds.Contains(pt);

                    if (!(isClick && m_pushedPrimitive.TabPrimitiveType == TabPrimitiveType.DropDown))
                    {
                        m_pushedPrimitive.SetState(isClick ? ButtonState.Flat : ButtonState.Normal);
                        m_tabControl.Invalidate(m_pushedPrimitive.Bounds);
                    }

                    if (isClick)
                    {
                        this.OnButtonClick(m_pushedPrimitive);
                    }

                    m_pushedPrimitive = null;
                }
            }
        }

        internal void HandledMouseLeave()
        {
            if (this.Visible)
            {
                foreach (TabPrimitive primitive in m_tabPrimitives)
                {
                    if (primitive.State == ButtonState.Flat)
                    {
                        primitive.SetState(ButtonState.Normal);
                        m_tabControl.Invalidate(primitive.Bounds);
                    }
                }
                StopShowingToolTip();
            }
        }

        internal void HandledMouseHover(EventArgs e)
        {
            if (m_tabControl != null && m_tabControl.ShowToolTips && m_bShowToolTipFirstTime)
            {
                m_bShowToolTipFirstTime = false;
                StartShowingToolTip(DEF_TOOLTIP_INITIAL_TIMER_INTERVAL);
            }
        }

        #endregion

        protected virtual void OnButtonClick(TabPrimitive primitive)
        {
            TabPrimitiveClickEventArgs e = new TabPrimitiveClickEventArgs(primitive);
            this.m_tabControl.OnTabPrimitiveClick(e);

            if (e.Cancel)
            {
                return;
            }

            bool mirrored = m_tabControl.RightToLeft == RightToLeft.Yes;

            switch (primitive.TabPrimitiveType)
            {
                case TabPrimitiveType.NextTab:
                    if (m_tabControl.SelectedIndex < m_tabControl.TabCount - 1)
                    {
                        for (int i = m_tabControl.SelectedIndex + 1; i < m_tabControl.TabCount; i++)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                            {
                                m_tabControl.SelectedIndex = i;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                break;
                            }
                        }
                    }
                    break;

                case TabPrimitiveType.PreviousTab:
                    if (m_tabControl.SelectedIndex > 0)
                    {
                        for (int i = m_tabControl.SelectedIndex - 1; i >= 0; i--)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                            {
                                m_tabControl.SelectedIndex = i;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                break;
                            }
                        }
                    }
                    break;

                case TabPrimitiveType.FirstTab:
                    if (m_tabControl.SelectedIndex > 0)
                    {
                        for (int i = 0; i < m_tabControl.TabCount; i++)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                            {
                                m_tabControl.SelectedIndex = i;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                break;
                            }
                        }
                    }
                    break;

                case TabPrimitiveType.LastTab:
                    if (m_tabControl.SelectedIndex < m_tabControl.TabCount - 1)
                    {
                        for (int i = m_tabControl.TabPages.Count - 1; i >= 0; i--)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                            {
                                m_tabControl.SelectedIndex = i;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                break;
                            }
                        }
                    }
                    break;

                case TabPrimitiveType.NextPage:
                    {
                        bool canScroll = mirrored ? m_tabControl.Renderer.CanScrollLeft : m_tabControl.Renderer.CanScrollRight;

                        if (canScroll && m_tabControl.SizeMode != TabSizeMode.ShrinkToFit)
                        {
                            if (!mirrored)
                            {
                                m_tabControl.Renderer.Scroll(ScrollIncrement.Page, ScrollDirection.Right);
                                for (int i = m_tabControl.SelectedIndex; i < m_tabControl.Renderer.Renderers.Count; i++)
                                {
                                    TabRendererBase renderer = m_tabControl.Renderer.Renderers[i] as TabRendererBase;
                                    if (renderer.Bounds.Left > 0)
                                    {
                                        if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                                        {
                                            m_tabControl.SelectedIndex = i;
                                            m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m_tabControl.Renderer.Scroll(ScrollIncrement.Page, ScrollDirection.Left);
                                for (int i = m_tabControl.Renderer.Renderers.Count - 1; i >= 0; i--)
                                {
                                    TabRendererBase renderer = m_tabControl.Renderer.Renderers[i] as TabRendererBase;
                                    if (renderer.Bounds.Left > 0)
                                    {
                                        if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                                        {
                                            m_tabControl.SelectedIndex = i;
                                            m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (m_tabControl.SelectedIndex < m_tabControl.TabCount - 1)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[m_tabControl.TabCount - 1].TabVisible)
                            {
                                m_tabControl.SelectedIndex = m_tabControl.TabCount - 1;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                            }
                        }
                        break;
                    }

                case TabPrimitiveType.PreviousPage:
                    {
                        bool canScroll = mirrored ? m_tabControl.Renderer.CanScrollRight : m_tabControl.Renderer.CanScrollLeft;

                        if (canScroll && m_tabControl.SizeMode != TabSizeMode.ShrinkToFit)
                        {
                            if (!mirrored)
                            {
                                m_tabControl.Renderer.Scroll(ScrollIncrement.Page, ScrollDirection.Left);

                                for (int i = 0; i < m_tabControl.Renderer.Renderers.Count; i++)
                                {
                                    TabRendererBase renderer = m_tabControl.Renderer.Renderers[i] as TabRendererBase;
                                    if (renderer.Bounds.Left > 0)
                                    {
                                        if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                                        {
                                            m_tabControl.SelectedIndex = i;
                                            m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m_tabControl.Renderer.Scroll(ScrollIncrement.Page, ScrollDirection.Right);

                                for (int i = m_tabControl.Renderer.Renderers.Count - 1; i >= 0; i--)
                                {
                                    TabRendererBase renderer = m_tabControl.Renderer.Renderers[i] as TabRendererBase;
                                    if (renderer.Bounds.Left > 0)
                                    {
                                        if (m_tabControl.IsDesignMode || m_tabControl.TabPages[i].TabVisible)
                                        {
                                            m_tabControl.SelectedIndex = i;
                                            m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (m_tabControl.SelectedIndex > 0)
                        {
                            if (m_tabControl.IsDesignMode || m_tabControl.TabPages[0].TabVisible)
                            {
                                m_tabControl.SelectedIndex = 0;
                                m_tabControl.SelectedTab.SetSelectedAtDesignTime();
                            }
                        }
                        break;
                    }
                case TabPrimitiveType.Close:
                    if (m_tabControl.SelectedTab != null)
                    {
                        Form form = m_tabControl.SelectedTab.Tag as Form;

                        m_tabControl.SelectedTab.Close();

                        if (form != null)
                        {
                            form.Close();
                        }
                    }
                    break;
                case TabPrimitiveType.DropDown:
                    if (m_dropDownPopupMenu == null)
                    {
                        InitDropDownPopupMenu();
                    }

                    if (m_dropDownPopupMenu.IsShowing())
                    {
                        m_dropDownPopupMenu.Hide();
                    }
                    else
                    {
                        FillItemsDropDownPopupMenu();

                        Point pt = primitive.Location;

                        if (this.TabControl.Alignment == TabAlignment.Left || this.TabControl.Alignment == TabAlignment.Right)
                        {
                            pt.X = this.Location.X + this.Size.Width;

                            if (this.TabControl.IsMirrored)
                            {
                                pt.Y += primitive.Size.Height;
                            }
                        }
                        else
                        {
                            pt.Y += this.Size.Height;

                            if (this.TabControl.IsMirrored)
                            {
                                pt.X += primitive.Size.Width;
                            }
                        }

                        m_dropDownPopupMenu.Show(this.TabControl, pt);
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize DropDown PopupMenu.
        /// </summary>
        private void InitDropDownPopupMenu()
        {
            m_dropDownPopupMenu = new PrimitiveDropDownPopupMenu();
            m_dropDownPopupMenu.TabControlAdv = this.TabControl;
            m_dropDownPopupMenu.ParentBarItem = new ParentBarItem();
            m_dropDownPopupMenu.ParentBarItem.PopupClosed += new EventHandler(PopupClosed);
        }

        /// <summary>
        /// Fill items in DropDown PopupMenu.
        /// </summary>
        private void FillItemsDropDownPopupMenu()
        {
            ParentBarItem pbi = m_dropDownPopupMenu.ParentBarItem;

            foreach (BarItem bi in pbi.Items)
            {
                bi.Click -= new EventHandler(BarItem_Click);
            }

            pbi.Items.Clear();

            foreach (TabPageAdv tp in this.TabControl.TabPages)
            {
                //Check whether the TabVisibe is true
                if (tp.TabVisible)
                {
                    BarItem bi = new BarItem();

                    bi.Text = tp.Text;
                    bi.ImageList = this.TabControl.ImageList;
                    bi.ImageIndex = tp.ImageIndex;
                    bi.Enabled = tp.Enabled;
                    bi.Tag = tp;

                    bi.Click += new EventHandler(BarItem_Click);

                    pbi.Items.Add(bi);
                }
            }

            m_dropDownPopupMenu.ActiveBounds = m_pushedPrimitive.Bounds;
        }

        private void M_TabPrimitives_CollectionChanged(object sender, EventArgs e)
        {
            this.RefreshTabControl();
        }

        private void PopupClosed(object sender, EventArgs e)
        {
            if (m_pushedPrimitive != null && m_pushedPrimitive.State == ButtonState.Pushed)
            {
                Point pt = this.TabControl.PointToClient(Control.MousePosition);
                m_pushedPrimitive.SetState(m_pushedPrimitive.Bounds.Contains(pt) ? ButtonState.Flat : ButtonState.Normal);
                m_tabControl.Invalidate(m_pushedPrimitive.Bounds);
            }
        }

        private void BarItem_Click(object sender, EventArgs e)
        {
            BarItem bi = sender as BarItem;

            if (bi != null)
            {
                TabPageAdv tp = bi.Tag as TabPageAdv;

                if (tp != null)
                {
                    this.TabControl.SelectedTab = tp;
                }
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Component and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        /// <remarks>See the documentation for the <see cref="System.ComponentModel.Component"/> class and its Dispose member.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_toolTip != null)
                {
                    m_toolTip.Dispose();
                    m_toolTip = null;
                }

                if (m_tabPrimitives != null)
                {
                    m_tabPrimitives.Dispose();
                    m_tabPrimitives = null;
                }
            }

            base.Dispose(disposing);
        }
    }

    [ToolboxItem(false)]
   public class PrimitiveDropDownPopupMenu : PopupMenu
    {
        private TabControlAdv m_tabControlAdv = null;

        private Rectangle m_activeBounds = Rectangle.Empty;

        public Rectangle ActiveBounds
        {
            get
            {
                return m_activeBounds;
            }
            set
            {
                m_activeBounds = value;
            }
        }

        public TabControlAdv TabControlAdv
        {
            get
            {
                return m_tabControlAdv;
            }
            set
            {
                m_tabControlAdv = value;
            }
        }

        public override bool IsRelatedControl(Control control, bool askParent)
        {
            if (m_tabControlAdv != null && control == m_tabControlAdv)
            {
                Point pt = m_tabControlAdv.PointToClient(Control.MousePosition);
                if (m_activeBounds.Contains(pt))
                {
                    return true;
                }
            }

            return base.IsRelatedControl(control, askParent);
        }
    }

    public enum TabPrimitiveHostAlignment
    {
        /// <summary>
        /// Represents far
        /// </summary>
        Far,

        /// <summary>
        /// Represents Near
        /// </summary>
        Near
    }

    public enum TabPrimitiveType
    {
        /// <summary>
        /// Represents First tab
        /// </summary>
        FirstTab,

        /// <summary>
        ///  Represents Last Tab
        /// </summary>
        LastTab,

        /// <summary>
        /// Represent Next page
        /// </summary>
        NextPage,

        /// <summary>
        /// Represents Previous page
        /// </summary>
        PreviousPage,

        /// <summary>
        /// Represents Next tab
        /// </summary>
        NextTab,

        /// <summary>
        /// Represents Previous tab
        /// </summary>
        PreviousTab,

        /// <summary>
        /// Represents close
        /// </summary>
        Close,

        /// <summary>
        /// Represents dropdown
        /// </summary>
        DropDown,

        /// <summary>
        /// Represents custom
        /// </summary>
        Custom
    }
}
