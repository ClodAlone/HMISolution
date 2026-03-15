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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class is obsolete. Use the <see cref="ChartLegendItem"/> class.
    /// </summary>
    [Obsolete("Class is obsolete. Use the ChartLegendItem class.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class LegendItem : ChartLegendItem
    {
        #region Constructors
        /// <summary>
        ///     Overloaded constructor.
        /// </summary>
        public LegendItem()
        {
        }
        /// <summary>
        ///     Overloaded constructor.
        /// </summary>
        /// <param name="text">Text of item.</param>
        public LegendItem(string text)
            : base(text)
        {
        }
        #endregion
    }

    /// <summary>
    /// The ChartLegendItem class holds information about each legend item like text, color and interior. Use this to add
    /// custom legend items through the ChartLegend.CustomItems list. Or parse through the auto generated ChartLegend.Items list.
    /// </summary>
    public class ChartLegendItem
      : IDisposable
    {
        #region Constants
        private readonly static Font c_defaultFont = new Font("Verdana", 10);
        #endregion

        #region Members
        /// <summary>
        /// The collection of subitems.
        /// </summary>
        protected ChartLegendItemsCollection m_children = null;

        /// <summary>
        /// The text of title.
        /// </summary>
        protected string m_text = "";
        /// <summary>
        /// The appearance style of item.
        /// </summary>
        protected ChartLegendItemStyle m_style = new ChartLegendItemStyle();

        /// <summary>
        /// Indicates whether item is visible.
        /// </summary>
        protected bool m_visible = true;
        /// <summary>
        /// Indicates whether item is checked.
        /// </summary>
        protected bool m_isChecked = false;

        /// <summary>
        /// The bounds of item.
        /// </summary>
        protected RectangleF m_bounds = Rectangle.Empty;
        /// <summary>
        /// The bounds of icon.
        /// </summary>
        protected RectangleF m_iconRect = Rectangle.Empty;
        /// <summary>
        /// The bounds of title.
        /// </summary>
        protected RectangleF m_textRect = Rectangle.Empty;

        /// <summary>
        /// Indicates is the shadow drawing pass.
        /// </summary>
        protected bool m_isDrawingShadow = false;
        private IChartLegend m_legend = null;
        private ChartLegendItem m_owner = null;
        private Image m_iconImage = null;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public LegendDrawItemTextEventHandler m_textHandler = null;
        #endregion

        #region Events
        /// <summary>
        /// Raised when the any properties is changed.
        /// </summary>
        public event EventHandler Changed;
        /// <summary>
        /// Raised when s <see cref="IsChecked"/> property is changed.
        /// </summary>
        public event EventHandler CheckedChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the the child collection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartLegendItemsCollection Children
        {
            get
            {
                return m_children;
            }
        }
        /// <summary>
        /// Returns the <see cref="ChartLegendItemStyle"/> for this item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartLegendItemStyle ItemStyle
        {
            get
            {
                return m_style;
            }
        }
        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        public Color BorderColor
        {
            get
            {
                return m_style.BorderColor;
            }
            set
            {
                if (m_style.BorderColor != value)
                {
                    m_style.BorderColor = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the font of the text.
        /// </summary>
        public Font Font
        {
            get
            {
                if (m_style.Font == null)
                {
                    if (m_owner != null)
                    {
                        return m_owner.Font;
                    }

                    if (m_legend != null)
                    {
                        return m_legend.Font;
                    }

                    return c_defaultFont;
                }

                return m_style.Font;
            }
            set
            {
                if (m_style.Font != value)
                {
                    m_style.Font = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the image index value of the item in the item's image list.
        /// </summary>
        public int ImageIndex
        {
            get
            {
                return m_style.ImageIndex;
            }
            set
            {
                if (m_style.ImageIndex != value)
                {
                    m_style.ImageIndex = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the ImageList associated with this item.
        /// </summary>
        [DefaultValue(null)]
        public ChartImageCollection ImageList
        {
            get
            {
                return m_style.ImageList;
            }
            set
            {
                if (m_style.ImageList != value)
                {
                    m_style.ImageList = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the interior brush for the rectangular area that represents a legend.
        /// </summary>
        public BrushInfo Interior
        {
            get
            {
                return m_style.Interior;
            }
            set
            {
                if (m_style.Interior != value)
                {
                    m_style.Interior = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the size of the rectangle holding the representation icon of the item.
        /// </summary>
        public Size RepresentationSize
        {
            get
            {
                return m_style.RepresentationSize;
            }
            set
            {
                if (m_style.RepresentationSize != value)
                {
                    m_style.RepresentationSize = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Indicates whether the symbol is to be displayed.
        /// </summary>
        public bool ShowSymbol
        {
            get
            {
                return m_style.ShowSymbol;
            }
            set
            {
                if (m_style.ShowSymbol != value)
                {
                    m_style.ShowSymbol = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the spacing of the item within the legend.
        /// </summary>
        [DefaultValue(0)]
        public int Spacing
        {
            get
            {
                return m_style.Spacing;
            }
            set
            {
                if (m_style.Spacing != value)
                {
                    m_style.Spacing = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the symbol that is to be associated with this item.
        /// </summary>
        public ChartSymbolInfo Symbol
        {
            get
            {
                return m_style.Symbol;
            }
            set
            {
                if (m_style.Symbol != value)
                {
                    m_style.Symbol = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the border item that is to be associated with this item's border.
        /// </summary>
        public ChartLineInfo Border
        {
            get
            {
                return m_style.Border;
            }
            set
            {
                if (m_style.Border != value)
                {
                    m_style.Border = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                if (m_text != value)
                {
                    m_text = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the text of the item.
        /// </summary>
        public Color TextColor
        {
            get
            {
                if (m_style.TextColor.IsEmpty)
                {
                    if (m_owner != null)
                    {
                        return m_owner.TextColor;
                    }

                    if (m_legend != null)
                    {
                        return m_legend.ForeColor;
                    }

                    return Color.Black;
                }
                
                return m_style.TextColor;
            }
            set
            {
                if (m_style.TextColor != value)
                {
                    m_style.IsStyleChanged = true;
                    m_style.TextColor = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the type of representation for the legend item.
        /// </summary>
        public ChartLegendItemType Type
        {
            get
            {
                return m_style.Type;
            }
            set
            {
                if (m_style.Type != value)
                {
                    m_style.Type = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Indicates whether the icon is to be displayed.
        /// </summary>
        public bool ShowIcon
        {
            get
            {
                return m_style.ShowIcon;
            }
            set
            {
                if (m_style.ShowIcon != value)
                {
                    m_style.ShowIcon = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the left/right alignment of the icon with respect to the legend text.
        /// </summary>
        public LeftRightAlignment IconAlignment
        {
            get
            {
                return m_style.IconAlignment;
            }
            set
            {
                if (m_style.IconAlignment != value)
                {
                    m_style.IconAlignment = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the vertical alignment of the legend text.
        /// </summary>
        public VerticalAlignment TextAligment
        {
            get
            {
                return m_style.TextAlignment;
            }
            set
            {
                if (m_style.TextAlignment != value)
                {
                    m_style.TextAlignment = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Indicates if the checkbox associated with this legend item is to be displayed. Also see <see cref="IsChecked"/>.
        /// </summary>
        public bool VisibleCheckBox
        {
            get
            {
                return m_style.VisibleCheckBox;
            }
            set
            {
                if (m_style.VisibleCheckBox != value)
                {
                    m_style.VisibleCheckBox = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Indicates if the shadow is to be shown.
        /// </summary>
        public bool ShowShadow
        {
            get
            {
                return m_style.ShowShadow;
            }
            set
            {
                if (m_style.ShowShadow != value)
                {
                    m_style.ShowShadow = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the size of shadow offset.
        /// </summary>
        public Size ShadowOffset
        {
            get
            {
                return m_style.ShadowOffset;
            }
            set
            {
                if (m_style.ShadowOffset != value)
                {
                    m_style.ShadowOffset = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the color of the Shadow.
        /// </summary>
        public Color ShadowColor
        {
            get
            {
                return m_style.ShadowColor;
            }
            set
            {
                if (m_style.ShadowColor != value)
                {
                    m_style.ShadowColor = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets value indicates whether <see cref="ChartLegendItem"/> is visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_visible;
            }
            set
            {
                if (m_visible != value)
                {
                    m_visible = value;
                    RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the state of <see cref="ChartLegendItem"/> checkbox.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return m_isChecked;
            }
            set
            {
                if (m_isChecked != value)
                {
                    m_isChecked = value;
                    this.OnCheckedChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the lines of text in multiline configurations.
        /// </summary>
        public string[] TextLines
        {
            get
            {
                return this.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            }
            set
            {
                this.Text = String.Join(Environment.NewLine, value);
            }
        }
        /// <summary>
        /// Gets the rendering bounds of the <see cref="ChartLegendItem"/>.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                return m_bounds;
            }
        }
        /// <summary>
        /// Gets or sets the icon image.
        /// </summary>
        /// <value>The image.</value>
        public Image Image
        {
            get
            {
                if (m_iconImage == null)
                {
                    if (m_style.ImageList != null
                        && m_style.ImageIndex > -1
                        && m_style.ImageIndex < m_style.ImageList.Count)
                    {
                        return m_style.ImageList[m_style.ImageIndex];
                    }
                }

                return m_iconImage;
            }
            set
            {
                if (m_iconImage != value)
                {
                    m_iconImage = value;
                    this.RaiseChanged(this, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        ///     Overloaded constructor.
        /// </summary>
        public ChartLegendItem()
        {
            m_children = new ChartLegendItemsCollection();
            m_children.Changed += new ChartListChangeHandler(OnChildrenChanged);
        }
        /// <summary>
        ///     Overloaded constructor.
        /// </summary>
        /// <param name="text">Text of item.</param>
        public ChartLegendItem(string text)
        {
            m_text = text;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the legend.
        /// </summary>
        /// <param name="legend">The legend.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLegend(IChartLegend legend)
        {
            m_legend = legend;
        }
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        private void SetOwner(ChartLegendItem owner)
        {
            m_owner = owner;
        }
        /// <summary>
        /// Indicates if <see cref="ChartLegendItem"/> contains the specified coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsHit(int x, int y)
        {
            return m_iconRect.Contains(x, y);
        }
        /// <summary>
        /// Measures the size of <see cref="ChartLegendItem"/>.
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        public SizeF Measure(Graphics g)
        {
            float spacing = m_style.Spacing;
            SizeF result = g.MeasureString(m_text, this.Font);

            if (m_style.ShowIcon)
            {
                Size iconSize = m_style.RepresentationSize;
                result.Width += spacing + iconSize.Width;
                result.Height = Math.Max(result.Height, iconSize.Height);
            }

            return Size.Ceiling(result);
        }
        /// <summary>
        /// Sets the bounds of <see cref="ChartLegendItem"/>.
        /// </summary>
        /// <param name="rect"></param>
        public void Arrange(RectangleF rect)
        {
            float spacing = m_style.Spacing;
            Size iconSize = m_style.RepresentationSize;

            if (m_style.IconAlignment == LeftRightAlignment.Right)
            {
                m_iconRect = new RectangleF(rect.Right - iconSize.Width,
                  rect.Top + (rect.Height - iconSize.Height) / 2, iconSize.Width, iconSize.Height);
                m_textRect = m_style.ShowIcon ? new RectangleF(rect.Left, rect.Top,
                  rect.Width - spacing - iconSize.Width, rect.Height) : new RectangleF(rect.Left, rect.Top,
                  rect.Width + spacing, rect.Height);
            }
            else
            {
                m_iconRect = new RectangleF(rect.Left, rect.Top + (rect.Height - iconSize.Height) / 2,
                  iconSize.Width, iconSize.Height);
                m_textRect = m_style.ShowIcon ? new RectangleF(rect.Left + spacing + iconSize.Width, rect.Top,
                  rect.Width - spacing - iconSize.Width, rect.Height) : new RectangleF(rect.Left + spacing + iconSize.Width, rect.Top,
                  rect.Width + spacing, rect.Height);
            }

            m_bounds = rect;
        }
        /// <summary>
        /// Draws the <see cref="ChartLegendItem"/>.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (m_style.ShowShadow)
            {
                GraphicsContainer gc = DrawingHelper.BeginTransform(g);
                m_isDrawingShadow = true;
                g.TranslateTransform(m_style.ShadowOffset.Width, m_style.ShadowOffset.Height);
                this.DrawInternal(g);
                m_isDrawingShadow = false;
                DrawingHelper.EndTransform(g, gc);
            }

            this.DrawInternal(g);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_legend = null;
            m_owner = null;

            if (m_children != null)
            {
                m_children.Changed -= new ChartListChangeHandler(OnChildrenChanged);
                m_children.Clear();
                m_children = null;
            }
        }
        #endregion

        private void RaiseDrawItemText(ChartLegendDrawItemTextEventArgs e)
        {
            if (m_textHandler != null)
            {
                m_textHandler(this, e);
            }
        }

        #region Implementation
        /// <summary>
        /// Internal method for the drawing of item.
        /// </summary>
        /// <param name="g"></param>
        private void DrawInternal(Graphics g)
        {
            if (m_text != string.Empty)
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.FormatFlags |= StringFormatFlags.NoClip;

                #region Sets alignment
                switch (m_style.TextAlignment)
                {
                    case VerticalAlignment.Top:
                        stringFormat.LineAlignment = StringAlignment.Near;
                        break;

                    case VerticalAlignment.Center:
                        stringFormat.LineAlignment = StringAlignment.Center;
                        break;

                    case VerticalAlignment.Bottom:
                        stringFormat.LineAlignment = StringAlignment.Far;
                        break;
                }
                #endregion

                ChartLegendDrawItemTextEventArgs drawArgs = new ChartLegendDrawItemTextEventArgs(g, this.Text, m_textRect);
                this.RaiseDrawItemText(drawArgs);

                if (!drawArgs.Handled)
                {
                    using (Brush sb = this.GetTextBrush())
                    {
                        g.DrawString(m_text, this.Font, sb, m_textRect, stringFormat);
                    }
                }
            }

            if (m_style.ShowIcon)
            {
                this.DrawIcon(g, m_iconRect, m_style.Type);

                #region Draw Symbol
                if (!m_isDrawingShadow && m_style.ShowSymbol)
                {
                    PointF pt = ChartMath.GetCenter(m_iconRect);

                    using (SolidBrush sb = new SolidBrush(m_style.Symbol.Color))
                    {
                        using (Pen pn = this.GetPen())
                        {
                            RenderingHelper.DrawPointSymbol(g, m_style.Symbol.Shape, m_style.Symbol.Marker,
                              m_style.Symbol.Size, m_style.Symbol.Offset, m_style.Symbol.ImageIndex,
                              sb, pn, m_style.ImageList, pt, false);
                        }
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Returns <see cref="BrushInfo"/> for the icon.
        /// </summary>
        /// <returns></returns>
        protected virtual BrushInfo GetBrushInfo()
        {
            return m_isDrawingShadow ? new BrushInfo(m_style.ShadowColor) : m_style.Interior;
        }
        /// <summary>
        /// Returns border <see cref="Pen"/> for the icon.
        /// </summary>
        /// <returns></returns>
        protected virtual Pen GetPen()
        {         
            return m_isDrawingShadow ? new Pen(m_style.ShadowColor) : m_style.Border.GdipPen.Clone() as Pen;
        }
        /// <summary>
        /// Returns line <see cref="Pen"/> for the icon.
        /// </summary>
        /// <returns></returns>
        protected virtual Pen GetLinePen()
        {
            return m_isDrawingShadow ? new Pen(m_style.ShadowColor,m_style.Border.Width) : new Pen(m_style.Interior.BackColor, m_style.Border.Width);
        }
        /// <summary>
        /// Returns <see cref="Brush"/> for the text.
        /// </summary>
        /// <returns></returns>
        protected virtual Brush GetTextBrush()
        {
            return m_isDrawingShadow ? new SolidBrush(m_style.ShadowColor) : new SolidBrush(this.TextColor);
        }
        /// <summary>
        /// This method is called when <see cref="ChartLegendItem.IsChecked"/> was changed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCheckedChanged(EventArgs args)
        {
            this.RaiseCheckedChanged(this, args);
        }
        /// <summary>
        /// Called when [children changed].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="args">The args.</param>
        private void OnChildrenChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (ChartLegendItem item in args.NewItems)
                {
                    item.ItemStyle.SetToLowerLevel(m_style);
                    item.ItemStyle.VisibleCheckBox = false;
                    item.SetLegend(m_legend);
                    item.SetOwner(m_owner);
                }
            }

            if (args.OldItems != null)
            {
                foreach (ChartLegendItem item in args.OldItems)
                {
                    item.ItemStyle.BaseStyle = item.ItemStyle.BaseStyle.BaseStyle;
                    item.SetLegend(null);
                    item.SetOwner(null);
                }
            }

            this.RaiseChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ChartLegendItem.Changed"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RaiseChanged(object sender, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(sender, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ChartLegendItem.CheckedChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void RaiseCheckedChanged(object sender, EventArgs args)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(sender, args);
            }
        }

        /// <summary>
        /// Draws legend icon by the specified <see cref="ChartLegendItemType"/>.
        /// </summary>
        /// <param name="g">Instance of <see cref="Graphics"/>.</param>
        /// <param name="bounds">Rectangle of icon.</param>
        /// <param name="shape">Shape of icon.</param>
        protected virtual void DrawIcon(Graphics g, RectangleF bounds, ChartLegendItemType shape)
        {
            bool isLine = false;
            bool isFilledShape = false;
            bool isImage = false;

            GraphicsPath gp = new GraphicsPath();

            switch (this.Type)
            {
                case ChartLegendItemType.None:
                    #region None
                    {
                        isLine = false;
                        isFilledShape = false;
                        isImage = false;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Line:
                    #region Line
                    {
                        gp.AddLines(new PointF[]{ 
              new PointF( bounds.X, bounds.Bottom ),
              new PointF( bounds.X + bounds.Width / 3, bounds.Top ),
              new PointF( bounds.X + 2 * bounds.Width / 3, bounds.Bottom ),
              new PointF( bounds.Right, bounds.Top )});

                        isLine = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Rectangle:
                    #region Rectangle
                    {
                        gp.AddRectangle(bounds);
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Spline:
                    #region Spline
                    {
                        gp.AddCurve(new PointF[]{
                                new PointF( bounds.X, bounds.Bottom ),
                                new PointF( bounds.X + bounds.Width / 3, bounds.Top ),
                                new PointF( bounds.X + 2 *  bounds.Width / 3, bounds.Top + bounds.Height / 2 ),
                                new PointF( bounds.Right, bounds.Top ),
                                new PointF( bounds.Right, bounds.Bottom )
                              });

                        isLine = false;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Area:
                    #region Area shape
                    {
                        gp.AddPolygon(new PointF[]{ 
              new PointF( bounds.Left, bounds.Bottom ),
              new PointF( bounds.Left + bounds.Width/3, bounds.Top ),
              new PointF( bounds.Left - bounds.Width/3, bounds.Top + bounds.Height / 2 ),
              new PointF( bounds.Right, bounds.Top ),
              new PointF( bounds.Right, bounds.Bottom ) });
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.PieSlice:
                    #region Pie
                    {
                        gp.AddPie(bounds.X, bounds.Y, 2 * bounds.Width, 2 * bounds.Height, -180, 90);
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Image:
                    #region Image
                    {
                        isImage = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Circle:
                    #region Circle
                    {
                        gp.AddEllipse(bounds);
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Diamond:
                    #region Diamond
                    {
                        gp.AddPolygon(new PointF[]{
              new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
              new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
              new PointF( bounds.X + bounds.Width / 2, bounds.Bottom ),
              new PointF( bounds.X, bounds.Y + bounds.Height / 2 ) });
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Hexagon:
                    #region Hexagon
                    {
                        gp.AddPolygon(new PointF[]{
              new PointF( bounds.X + bounds.Width / 4, bounds.Y ),
              new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Y ),
              new PointF( bounds.Right, bounds.Y + bounds.Height / 2 ),
              new PointF( bounds.X + bounds.Width * ( 3f / 4f ), bounds.Bottom ),
              new PointF( bounds.X + bounds.Width / 4, bounds.Bottom ),
              new PointF( bounds.X, bounds.Y + bounds.Height / 2 ) });
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Pentagon:
                    #region Pentagon
                    {
                        gp.AddPolygon(new PointF[]{
              new PointF( bounds.X + bounds.Width / 5f, bounds.Y ),
              new PointF( bounds.Right - bounds.Width / 5f, bounds.Y ),
              new PointF( bounds.Right, bounds.Y + bounds.Height * ( 3f / 5f ) ),
              new PointF( bounds.X + bounds.Width / 2f, bounds.Bottom ),
              new PointF( bounds.X, bounds.Y + bounds.Height * ( 3f / 5f ) ) });
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Triangle:
                    #region Triangle
                    {
                        gp.AddPolygon(new PointF[]{
              new PointF( bounds.X + bounds.Width / 2, bounds.Y ),
              new PointF( bounds.X, bounds.Bottom ),
              new PointF( bounds.Right, bounds.Bottom )});
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.InvertedTriangle:
                    #region InvertedTriangle
                    {
                        gp.AddPolygon(new PointF[]{
              new PointF( bounds.X, bounds.Y ),
              new PointF( bounds.Right, bounds.Y ),
              new PointF( bounds.X + bounds.Width / 2, bounds.Bottom ) });
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.Cross:
                    #region Cross
                    {
                        gp.AddLine(bounds.Left, bounds.Top + bounds.Height / 2,
                          bounds.Right, bounds.Top + bounds.Height / 2);
                        gp.CloseFigure();
                        gp.AddLine(bounds.Left + bounds.Width / 2, bounds.Top,
                          bounds.Left + bounds.Width / 2, bounds.Bottom);
                        gp.CloseFigure();
                        isLine = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.SplineArea:
                    #region SplineArea
                    {
                        gp.AddCurve(new PointF[]{
                                new PointF( bounds.X, bounds.Bottom ),
                                new PointF( bounds.X + bounds.Width / 3, bounds.Top ),
                                new PointF( bounds.X + 2 * bounds.Width / 3, bounds.Top + bounds.Height / 2 ),
                                new PointF( bounds.Right, bounds.Top ),
                                new PointF( bounds.Right, bounds.Bottom )
                              });
                        gp.CloseFigure();
                        isFilledShape = true;
                    }
                    #endregion
                    break;

                case ChartLegendItemType.StraightLine:
                    #region StraightLine
                    {
                        gp.AddLine(bounds.Left + bounds.Width / 8, bounds.Y + bounds.Height / 2,
                          bounds.Right - bounds.Width / 8, bounds.Y + bounds.Height / 2);
                        isLine = true;
                    }
                    #endregion
                    break;
            }

            #region Draw shape
            if (isImage && this.Image != null)
            {
                g.DrawImage(this.Image, bounds);
            }

            if (isFilledShape)
            {
                BrushPaint.FillPath(g, gp, this.GetBrushInfo());

                using (Pen pen = this.GetPen())
                {
                    g.DrawPath(pen, gp);
                }
            }

            if (isLine)
            {
                using (Pen pen = this.GetLinePen())
                {
                    g.DrawPath(pen, gp);
                }
            }
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// This type is used internally to create legend items associated with a series. Such auto generated legend items are usually 
    /// of this type in the <see cref="Syncfusion.Windows.Forms.Chart.ChartLegendItem"/> list.
    /// </summary>
    public sealed class ChartSeriesLegendItem : ChartLegendItem
    {
        #region Constants
        private const int c_emptyIndex = -1;
        #endregion

        #region Members
        private int m_pointIndex = c_emptyIndex;
        private ChartSeries m_series = null;
        private bool m_drawSeriesIcon = true;
        #endregion

        #region Properties
        /// <summary>
        /// The <see cref="ChartSeries"/> corresponding to this item.
        /// </summary>
        public ChartSeries Series
        {
            get
            {
                return m_series;
            }
        }
        /// <summary>
        /// Indicates the method for the drawing of legend icon. If true, an icon representing the series type will be rendered.
        /// </summary>
        public bool DrawSeriesIcon
        {
            get { return m_drawSeriesIcon; }
            set { m_drawSeriesIcon = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initialize new instance of <see cref="ChartSeriesLegendItem"/> class.
        /// </summary>
        /// <param name="series"></param>
        public ChartSeriesLegendItem(ChartSeries series)
            : this(series, c_emptyIndex)
        {
        }
        /// <summary>
        /// Initialize new instance of <see cref="ChartSeriesLegendItem"/> class.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="index"></param>
        public ChartSeriesLegendItem(ChartSeries series, int index)
        {
            m_series = series;
            m_pointIndex = index;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets settings of item by series.
        /// </summary>
        public void Refresh(bool useSeriesStyle)
        {
            m_isChecked = m_series.Visible;

            
           // if(useSeriesStyle)
            if ((useSeriesStyle) && (!m_style.IsStyleChanged))
            {
                if (m_pointIndex != c_emptyIndex)
                {
                    ChartStyleInfo style = m_series.GetOfflineStyle(m_pointIndex);

                    m_style.TextColor = style.TextColor;
                    m_style.Symbol = style.Symbol;
                    m_style.Border = style.Border;
                    m_style.Interior = style.Interior;
                    m_style.ImageList = style.Images;
                    m_style.ImageIndex = style.ImageIndex;
                    m_text = style.Text;
                }
                else
                {
                    ChartStyleInfo style = m_series.GetOfflineStyle();

                    m_style.TextColor = style.TextColor;
                    m_style.Symbol = style.Symbol;
                    m_style.Border = style.Border;
                    m_style.Interior = style.Interior;
                    m_style.ImageList = style.Images;
                    m_style.ImageIndex = style.ImageIndex;
                    m_text = m_series.Text;
                    
                }

                this.RaiseChanged(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Overrides <see cref="ChartLegendItem.DrawIcon"/> method.
        /// </summary>
        /// <param name="g">Instance of <see cref="Graphics"/>.</param>
        /// <param name="bounds">Rectangle of icon.</param>
        /// <param name="shape">Shape of icon.</param>
        protected override void DrawIcon(Graphics g, RectangleF bounds, ChartLegendItemType shape)
        {
            if (m_drawSeriesIcon)
            {
                if (m_pointIndex == c_emptyIndex)
                {
                    m_series.Renderer.DrawIcon(g, Rectangle.Round(bounds), m_isDrawingShadow, this.ShadowColor);
                }
                else
                {
                    m_series.Renderer.DrawIcon(m_pointIndex, g, Rectangle.Round(bounds), m_isDrawingShadow, this.ShadowColor);
                }
            }
            else
            {
                base.DrawIcon(g, bounds, shape);
            }
        }
        /// <summary>
        /// Overrides <see cref="ChartLegendItem.OnCheckedChanged"/> method.
        /// </summary>
        /// <param name="args">Argument.</param>
        protected override void OnCheckedChanged(EventArgs args)
        {
            m_series.Visible = m_isChecked;

            base.OnCheckedChanged(args);
        }
        /// <summary>
        /// Overrides <see cref="ChartLegendItem.GetBrushInfo"/> method.
        /// </summary>
        /// <returns></returns>
        protected override BrushInfo GetBrushInfo()
        {
            BrushInfo brushInfo = m_style.Interior;

            if (m_isDrawingShadow)
            {
                brushInfo = new BrushInfo(m_style.ShadowColor);
            }
            else
            {
                ChartColumnConfigItem config = m_series.ConfigItems.ColumnItem;

                if (m_series.ChartModel.ColorModel.AllowGradient)
                {
                    if (config.ShadingMode == ChartColumnShadingMode.PhongCylinder)
                    {
                        if (brushInfo.Style == BrushStyle.Solid)
                        {
                            float[] pos;
                            Color[] col;

                            ChartSeriesRenderer.PhongShadingColors(brushInfo.BackColor, brushInfo.BackColor,
                              config.LightColor, config.LightAngle, config.PhongAlpha, out col, out pos);

                            brushInfo = new BrushInfo(GradientStyle.Horizontal, col);
                        }
                    }
                }
            }

            return brushInfo;
        }
        #endregion
    }
}