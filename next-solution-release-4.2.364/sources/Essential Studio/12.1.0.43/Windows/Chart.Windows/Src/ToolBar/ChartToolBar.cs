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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Defines the chart's toolbar.
    /// </summary>
    [ToolboxItem(false)]
    internal class ChartToolBar : ChartDockControl
    {
        #region Constants
        private const int c_minItemSize = 16;
        private const int c_gripSize = 4;
        #endregion

        #region Members
        private ChartToolBarInfo m_info;

        private bool m_showGrip = true;
        private bool m_bAutoSize = true;
        private int m_spacing = 0;
        private int m_padding = 2;
        private int m_header = 0;
        private Size m_autoSize;
        private Rectangle m_gripRect = Rectangle.Empty;

        private bool m_bShowBorder = true;
        private LineInfo m_border = new LineInfo();

        private Color m_buttonFrColor = Color.Transparent;
        private Color m_buttonBkColor = Color.Transparent;
        private FlatStyle m_buttonFlatStyle = FlatStyle.Flat;
        private Size m_buttonSize = new Size(22, 22);

        [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
        private ChartToolBarButtonCollection m_buttons;
        private ChartToolBarItemCollection m_items = new ChartToolBarItemCollection();
        private ToolTip m_toolTip = new ToolTip();

        private ChartToolBarItemBase m_focusedItem = null;

        private int m_iconPadding = 2;
        #endregion

        #region Events
        public event EventHandler ItemClick;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the button size of the ToolBar buttons.
        /// </summary>
        public Size ButtonSize
        {
            get
            {
                return m_buttonSize;
            }

            set
            {
                if (m_buttonSize != value)
                {
                    m_buttonSize = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets/set the orientation of the ToolBar.
        /// </summary>
        public override ChartOrientation Orientation
        {
            get
            {
                return base.Orientation;
            }

            set
            {
                if (base.Orientation != value)
                {
                    base.Orientation = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Spacing.
        /// </summary>
        public int Spacing
        {
            get
            {
                return m_spacing;
            }

            set
            {
                if (m_spacing != value)
                {
                    m_spacing = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets padding within the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Windows.Forms.Padding"></see> representing the control's internal spacing characteristics.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public new int Padding
        {
            get { return m_padding; }

            set { m_padding = value; }
        }

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <value>The info.</value>
        /// <internalonly/>
        [DocumentationExclude()]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartToolBarInfo Info
        {
            get
            {
                return m_info;
            }
        }

        /// <summary>
        /// Gets the toolbar buttons collection.
        /// </summary>
        [Obsolete("This property isn't used anymore")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChartToolBarButtonCollection Buttons
        {
            get
            {
                return m_buttons;
            }
        }

        /// <summary>
        /// Gets the items collection.
        /// </summary>
        public ChartToolBarItemCollection Items
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Indicates if this element can be resized automatically.
        /// </summary>
        public override bool AutoSize
        {
            get
            {
                return m_bAutoSize;
            }

            set
            {
                if (m_bAutoSize != value)
                {
                    m_bAutoSize = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grip is shown.
        /// </summary>
        /// <value><c>true</c> if grip is shown; otherwise, <c>false</c>.</value>
        public bool ShowGrip
        {
            get { return m_showGrip; }

            set { m_showGrip = value; }
        }

        /// <summary>
        /// Gets or sets the height of the header.
        /// </summary>
        public int Header
        {
            get
            {
                return m_header;
            }

            set
            {
                if (m_header != value)
                {
                    m_header = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or Sets a value indicating whether the border of the ToolBar is to be shown or not.
        /// </summary>
        public bool ShowBorder
        {
            get
            {
                return m_bShowBorder;
            }

            set
            {
                if (m_bShowBorder != value)
                {
                    m_bShowBorder = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the information that is to be used for drawing border.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public LineInfo Border
        {
            get
            {
                return m_border;
            }
        }

        /// <summary>
        /// Gets or sets the flatstyle appearance for the ToolBar button control.
        /// </summary>
        [Obsolete("This property isn't used anymore")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FlatStyle ButtonFlatStyle
        {
            get
            {
                return m_buttonFlatStyle;
            }

            set
            {
                if (m_buttonFlatStyle != value)
                {
                    m_buttonFlatStyle = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the BackInterior of the ToolBar Button.
        /// </summary>
        public Color ButtonBackColor
        {
            get
            {
                return m_buttonBkColor;
            }

            set
            {
                if (m_buttonBkColor != value)
                {
                    m_buttonBkColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the ForeColor of the ToolBar Button.
        /// </summary>
        public Color ButtonForeColor
        {
            get
            {
                return m_buttonFrColor;
            }

            set
            {
                if (m_buttonFrColor != value)
                {
                    m_buttonFrColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon padding.
        /// </summary>
        /// <value>The icon padding.</value>
        public int IconPadding
        {
            get { return m_iconPadding; }

            set { m_iconPadding = value; }
        }

        /// <summary>
        /// Gets or sets the docking position of the ToolBar.
        /// </summary>
        public override ChartDock Position
        {
            get
            {
                return base.Position;
            }

            set
            {
                if (base.Position != value)
                {
                    base.Position = value;
                    OnButtonSizeChange();
                }
            }
        }

        /// <summary>
        /// Gets or sets the focused item.
        /// </summary>
        /// <value>The focused item.</value>
        internal ChartToolBarItemBase FocusedItem
        {
            get
            {
                return m_focusedItem;
            }

            set
            {
                if (m_focusedItem != value)
                {
                    m_focusedItem = value;
                    m_toolTip.SetToolTip(this, m_focusedItem == null ? "" : m_focusedItem.ToolTip);
                    this.Invalidate();
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBar"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public ChartToolBar(ChartControl parent)
        {
            this.Parent = parent;
            #pragma warning disable 612
            m_buttons = new ChartToolBarButtonCollection();
            #pragma warning restore 612

            m_items.Changed += new ChartListChangeHandler(OnItemsChanged);

            m_info = new ChartToolBarInfo(this);

            m_border.SettingsChanged += new EventHandler(OnBorderSettingsChanged);

            this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, false);

            OnButtonSizeChange();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"></see> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                #pragma warning disable 612
                m_buttons = null;
                #pragma warning restore 612

                m_info = null;
                m_border.SettingsChanged -= new EventHandler(OnBorderSettingsChanged);
                m_items.Changed -= new ChartListChangeHandler(OnItemsChanged);
                m_items = null;
                m_border = null;
                m_focusedItem = null;
                m_toolTip.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Measure size of control.
        /// </summary>
        /// <param name="size"></param>
        /// <returns>Returns the size of control.</returns>
        public override SizeF Measure(SizeF size)
        {
            bool isHor = this.Orientation == ChartOrientation.Horizontal;
            int length = m_items.Count;

            int xOffset = m_padding;
            int yOffset = m_padding + m_header;
            int width = 0;
            int height = 0;
            int itemWidth = 0;
            int itemHeight = 0;

            if (m_bShowBorder)
            {
                xOffset += (int)m_border.Width;
                yOffset += (int)m_border.Width;
            }

            #region Mesure
            m_buttonSize = new Size(Math.Max(m_buttonSize.Width, c_minItemSize),
                Math.Max(m_buttonSize.Height, c_minItemSize));

            for (int i = 0; i < length; i++)
            {
                Size itemSize = m_items[i].DesiredSize;

                itemWidth = Math.Max(itemWidth, itemSize.Width);
                itemHeight = Math.Max(itemHeight, itemSize.Height);

                if (isHor)
                {
                    height = Math.Max(itemSize.Height, height);
                    width += itemSize.Width;
                }
                else
                {
                    width = Math.Max(itemSize.Width, width);
                    height += itemSize.Height;
                }
            }

            if (m_showGrip)
            {
                if (isHor)
                {
                    m_gripRect = new Rectangle(xOffset, yOffset, c_gripSize, height);
                    xOffset += c_gripSize + m_padding;
                    width += c_gripSize + m_padding;
                }
                else
                {
                    m_gripRect = new Rectangle(xOffset, yOffset, width, c_gripSize);
                    yOffset += c_gripSize + m_padding;
                    height += c_gripSize + m_padding;
                }
            }

            if (isHor)
            {
                width += (length - 1) * m_spacing + 2 * m_padding;
                height += 2 * m_padding + m_header;
            }
            else
            {
                width += 2 * m_padding;
                height += (length - 1) * m_spacing + m_header + 2 * m_padding;
            }

            if (m_bShowBorder)
            {
                width += (int)(2 * m_border.Width);
                height += (int)(2 * m_border.Width);
            }

            m_autoSize = new Size(width, height);
            #endregion

            #region Arrange
            int position = 0;

            for (int i = 0; i < length; i++)
            {
                ChartToolBarItemBase item = m_items[i];
                Size sz = item.DesiredSize;

                if (isHor)
                {
                    item.Arrange(new Rectangle(xOffset + position, yOffset, sz.Width, itemHeight));
                    position += sz.Width + m_spacing;
                }
                else
                {
                    item.Arrange(new Rectangle(xOffset, yOffset + position, itemWidth, sz.Height));
                    position += sz.Height + m_spacing;
                }
            }
            #endregion

            if (this.Position != ChartDock.Floating && !this.DockingFree)
            {
                if (isHor)
                {
                    m_autoSize.Width = (int)size.Width;
                }
                else
                {
                    m_autoSize.Height = (int)size.Height;
                }
            }

            if (m_bAutoSize)
            {
                this.Size = m_autoSize;
                this.Invalidate();
            }

            return this.Size;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/>.</param>
        internal void Draw(Graphics g)
        {
            Bitmap bmp = new Bitmap(this.Width, this.Height);

            using (Graphics gt = Graphics.FromImage(bmp))
            {
                using (SolidBrush sb = new SolidBrush(this.BackColor))
                {
                    gt.FillRectangle(sb, this.ClientRectangle);
                }

                this.OnPaint(new PaintEventArgs(gt, this.ClientRectangle));
            }

            g.DrawImage(bmp, this.Bounds);
        }

        /// <summary>
        /// Method is called when the Paint event is raised.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            #region Draw items
            foreach (ChartToolBarItemBase item in m_items)
            {
                item.Draw(e.Graphics);
            }
            #endregion

            #region Draw grip
            if (m_showGrip)
            {
                HatchStyle hatchStyle = Orientation == ChartOrientation.Horizontal ?
                    HatchStyle.NarrowHorizontal : HatchStyle.NarrowVertical;

                using (HatchBrush br = new HatchBrush(hatchStyle, Color.Gray, Color.Transparent))
                {
                    e.Graphics.FillRectangle(br, m_gripRect);
                }
            }
            #endregion

            #region Draw header
            if ((Text != "") && (m_header > e.Graphics.MeasureString(Text, Font, Width).Height))
            {
                StringFormat strFormat = new StringFormat(StringFormatFlags.NoClip);
                strFormat.Alignment = StringAlignment.Center;

                Rectangle rect = new Rectangle(0, 0, this.Width, m_header);

                if (m_bShowBorder)
                {
                    rect.Inflate(-(int)m_border.Width, -(int)m_border.Width);
                }

                using (Brush textBrsh = new SolidBrush(this.ForeColor))
                {
                    e.Graphics.DrawString(Text, Font, textBrsh, rect, strFormat);
                }
            }
            #endregion

            #region Draw border
            if (m_bShowBorder)
            {
                Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                using (Pen pen = m_border.Pen.Clone() as Pen)
                {
                    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;

                    e.Graphics.DrawRectangle(pen, rect);
                    e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, m_header);
                }
            }
            #endregion
        }

        /// <summary>
        /// Method is called when the SizeChanged event is raised.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            if (m_bAutoSize && Size != m_autoSize)
            {
                this.Size = m_autoSize;
            }

            this.Location = this.CheckLocation(this.Location);
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Method is called when the DoubleClick event is raised.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            if (this.FocusedItem == null)
            {
                if (m_info.ShowDialog)
                {
                    using (ToolBarPropertyForm form = new ToolBarPropertyForm())
                    {
                        form.SetInfo(m_info);
                        form.ShowDialog();
                    }
                }

                base.OnDoubleClick(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Click"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            ChartToolBarItemBase focusedItem = this.FocusedItem;

            if ((focusedItem != null) && (focusedItem.Click()))
            {
                this.RaiseItemClick(focusedItem);
            }
            else
            {
                base.OnClick(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            ChartToolBarItemBase focused = null;

            if (this.Cursor == Cursors.Default)
            {
                foreach (ChartToolBarItemBase item in m_items)
                {
                    if (item.Bounds.Contains(e.X, e.Y))
                    {
                        focused = item;
                        break;
                    }
                }

                this.FocusedItem = focused;
            }

            if (this.FocusedItem == null)
            {
                base.OnMouseMove(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            this.FocusedItem = null;
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.FocusedItem == null)
            {
                base.OnMouseDown(e);
            }
            else
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"></see> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"></see> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.FocusedItem == null)
            {
                base.OnMouseUp(e);
            }
            else
            {
                this.Invalidate();
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Raises the item click.
        /// </summary>
        /// <param name="item">The item.</param>
        private void RaiseItemClick(ChartToolBarItemBase item)
        {
            if (ItemClick != null)
            {
                ItemClick(item, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Method is called when the button size is changed.
        /// </summary>
        private void OnButtonSizeChange()
        {
            if ((Behavior & ChartDockingFlags.Dockable) == ChartDockingFlags.None
                || (this.Position == ChartDock.Floating))
            {
                if (this.Parent != null)
                {
                    this.Measure(this.Parent.Size);
                }
                else
                {
                    this.Measure(SystemInformation.PrimaryMonitorSize);
                }
            }

            this.OnSizeChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Handles the SettingsChanged event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnBorderSettingsChanged(object sender, EventArgs e)
        {
            this.OnButtonSizeChange();
        }

        /// <summary>
        /// Called when items is changed.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="args">The args.</param>
        private void OnItemsChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (ChartToolBarItemBase item in args.NewItems)
                {
                    item.SetOwner(this);
                }
            }

            if (args.OldItems != null)
            {
                foreach (ChartToolBarItemBase item in args.OldItems)
                {
                    item.SetOwner(null);
                }
            }

            this.OnButtonSizeChange();
        }
        #endregion
    }
}