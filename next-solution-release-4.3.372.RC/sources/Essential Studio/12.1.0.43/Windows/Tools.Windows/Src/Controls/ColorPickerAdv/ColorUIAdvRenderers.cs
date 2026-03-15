#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    public class ColorUIAdvRenderer
    {
        private const int horizontalOffcet = 5;
        private const int verticalOffcet = 2;

        private static readonly Color c_ItemBorderColor = Color.FromArgb(197, 197, 197);
        private static readonly Color c_HighlightedBorderColor = Color.FromArgb(243, 148, 54);
        private static readonly Color c_SelectedBorderColor = Color.FromArgb(235, 75, 13);
        private static readonly Color c_SelectedHighlightedBorderColor = Color.FromArgb(255, 226, 148);
        private static readonly Color c_DefaultBackColor = SystemColors.Control;
        private static readonly Color c_GroupHeaderBackColor = SystemColors.ControlDark;

        private const ContentAlignment anyRight = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        private const ContentAlignment anyBottom = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        private const ContentAlignment anyCenter = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        private const ContentAlignment anyMiddle = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;

        protected Point m_textPoint = Point.Empty;
        private Rectangle m_bounds = Rectangle.Empty;
        private ColorPickerUIAdv m_colorPicker = null;

        protected virtual Color BackColor
        {
            get { return c_DefaultBackColor; }
        }

        protected Rectangle Bounds
        {
            get { return m_bounds; }
        }

        public ColorPickerUIAdv ColorPicker
        {
            get { return m_colorPicker; }
        }

        protected virtual Color ItemBorderColor
        {
            get { return c_ItemBorderColor; }
        }

        protected virtual Color HighlightedBorderColor
        {
            get { return c_HighlightedBorderColor; }
        }

        protected virtual Color SelectedBorderColor
        {
            get { return c_SelectedBorderColor; }
        }

        protected virtual Color SelectedHighlightedBorderColor
        {
            get { return c_SelectedHighlightedBorderColor; }
        }

        protected virtual Color GroupHeaderBackColor
        {
            get { return c_GroupHeaderBackColor; }
        }

        public ColorUIAdvRenderer(ColorPickerUIAdv control)
        {
            m_colorPicker = control;
            m_bounds = control.ClientRectangle;
        }

        public virtual void OnPaint(PaintEventArgs e)
        {

            // Draw Background
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(brush, this.ColorPicker.ClientRectangle);
            }

            this.DrawInterior(e.Graphics);
        }

        private Point m_focus = new Point(-1, -1);
        private Color m_foreColor = Color.Black;
        protected virtual Color ForeColor
        {
            get { return m_foreColor; }
        }

        protected virtual void DrawInterior(Graphics g)
        {
            Rectangle rect = this.Bounds;

            if (this.ColorPicker.Groups != null && this.ColorPicker.Groups.Count != 0)
            {
                for (int i = 0; i < this.ColorPicker.Groups.Count; i++)
                {
                    ColorUIAdvGroup group = this.ColorPicker.Groups[i];

                    if (group.Visible)
                    {
                        this.DrawGroupHeader(g, group);

                        this.DrawGroupInterior(g, group);
                    }
                }
            }
        }

        protected virtual void DrawGroupHeader(Graphics g, ColorUIAdvGroup group)
        {
            // fill header
            Rectangle header = Rectangle.Empty;

            if (group.Header.Dock == DockStyle.Top)
            {
                header = new Rectangle(group.Bounds.X, group.Bounds.Y, group.Bounds.Width, group.HeaderHeight);
            }
            using (Brush br = new SolidBrush(this.GroupHeaderBackColor))
            {
                g.FillRectangle(br, header);
            }

            this.DrawText(g, group, header);
        }

        public virtual void DrawText(Graphics g, ColorUIAdvGroup group, Rectangle labelBounds)
        {
            this.ComputeTextPosition(group, labelBounds);

            DrawParams param = new DrawParams();
            param.Enabled = this.ColorPicker.Enabled;
            param.ControlBackColor = this.ColorPicker.BackColor;
            param.Bounds = CalcTextRectangle(g, group, this.ColorPicker.TextAlign, labelBounds);
            param.Align = this.ColorPicker.TextAlign;

            this.DrawTextInternal(g, group.Name, this.ForeColor, this.ColorPicker.Font, m_textPoint, param);
        }

        private void DrawTextInternal(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param)
        {
            using (StringFormat fmt = new StringFormat())
            {
                fmt.HotkeyPrefix = HotkeyPrefix.Show;
                fmt.Alignment = TranslateAlignment(param.Align);
                fmt.LineAlignment = TranslateLineAlignment(param.Align);

                RectangleF textRect = param.Bounds;

                if (param.Enabled)
                {
                    using (Brush brush = new SolidBrush(textColor))
                    {
                        g.DrawString(str, textFont, brush, textRect, fmt);
                    }
                }
                else
                {
                    ControlPaint.DrawStringDisabled(g, str, textFont, param.ControlBackColor, param.Bounds, fmt);       
                }
            }
        }

       private StringAlignment TranslateAlignment(ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                return StringAlignment.Far;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                return StringAlignment.Center;
            }

            return StringAlignment.Near;
        }

        private StringAlignment TranslateLineAlignment(ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                return StringAlignment.Far;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                return StringAlignment.Center;
            }

            return StringAlignment.Near;
        }

        private Rectangle VAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                withinThis.Y += withinThis.Height - alignThis.Height;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                withinThis.Y += (withinThis.Height - alignThis.Height) / 2;
            }

            withinThis.Height = alignThis.Height;

            return withinThis;
        }

        private Rectangle HAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                withinThis.X += withinThis.Width - alignThis.Width;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                withinThis.X += (withinThis.Width - alignThis.Width) / 2;
            }

            withinThis.Width = alignThis.Width;

            return withinThis;
        }

        public virtual void ComputeTextPosition(ColorUIAdvGroup group, Rectangle labelBounds)
        {
            Graphics g = this.ColorPicker.CreateGraphics();

            StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            SizeF textSize = g.MeasureString(group.Name, this.ColorPicker.Font, this.ColorPicker.Width, sf);
            g.Dispose();

            Rectangle buttonRect = labelBounds;

            int yOffcet = labelBounds.Y;

            int horizontalGap = (int)Math.Max(0, (buttonRect.Width - 2 * horizontalOffcet - textSize.Width) / 2);
            int verticalGap = (int)Math.Max(0, (buttonRect.Height - 2 * verticalOffcet - textSize.Height) / 2);

            switch (this.ColorPicker.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;
                case ContentAlignment.BottomLeft:
                    m_textPoint = new Point(horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;
                case ContentAlignment.BottomRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleLeft:
                    m_textPoint = new Point(horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.TopCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, verticalOffcet);
                    break;

                case ContentAlignment.TopLeft:
                    m_textPoint = new Point(horizontalOffcet, verticalOffcet);
                    break;

                case ContentAlignment.TopRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, verticalOffcet);
                    break;
            }

            m_textPoint.Y += yOffcet;
        }

        protected virtual Rectangle CalcTextRectangle(Graphics g, ColorUIAdvGroup group, ContentAlignment align, Rectangle labelBounds)
        {
            Rectangle clientRect = labelBounds;
            clientRect.Inflate(-horizontalOffcet, -verticalOffcet);
            Rectangle textRect = clientRect;

            if (g != null)
            {
                Size textSize;
                using (StringFormat format = new StringFormat())
                {
                    SizeF efSize = g.MeasureString(group.Name, this.ColorPicker.Font, new SizeF((float)textRect.Width, (float)textRect.Height), format);
                    textSize = Size.Ceiling(efSize);
                }

                textRect = this.HAlignWithin(textSize, textRect, align);
                textRect = this.VAlignWithin(textSize, textRect, align);
            }

            int num1 = Math.Min(textRect.Bottom, clientRect.Bottom);
            textRect.Y = Math.Max(textRect.Y, clientRect.Y);
            textRect.Height = num1 - textRect.Y;

            return textRect;
        }

        protected virtual void DrawGroupInterior(Graphics g, ColorUIAdvGroup group)
        {
            if (group.Items != null && group.Items.Count > 0)
            {
                Rectangle drawRect = group.Bounds;

                int yOffcet = drawRect.Y;

                if (group.Header.Dock == DockStyle.Top)
                    yOffcet += group.HeaderHeight;

                int defYOffcet = yOffcet;
                if (group.IsSubItemsVisible)
                {
                    defYOffcet += group.ParentControl.BorderOffset + group.ParentControl.ColorItemSize.Height;

                    if (group.ParentControl.VerticalItemsSpacing < ColorPickerUIAdv.DEF_BASECOLORSOFFSET)
                        defYOffcet += ColorPickerUIAdv.DEF_BASECOLORSOFFSET;
                    else
                        defYOffcet += group.ParentControl.VerticalItemsSpacing;
                }
                else
                {
                    defYOffcet += group.ParentControl.BorderOffset * 2 + group.ParentControl.ColorItemSize.Height;
                }

                Rectangle inheritArea = new Rectangle(new Point(0, defYOffcet), new Size(group.Size.Width, group.Size.Height - defYOffcet));

                for (int i = 0; i < group.Items.Count; i++)
                {
                    GroupColorItem item = group.Items[i] as GroupColorItem;
                    Rectangle rect = item.Bounds;

                    Color fillColor = item.Color;
                    using (Brush brush = new SolidBrush(fillColor))
                    {
                        g.FillRectangle(brush, rect);
                    }
                    rect.Width--;
                    rect.Height--;

                    Color borderColor = Color.Empty;

                    if (item.State == ColorItemState.Normal)
                        borderColor = this.ItemBorderColor;
                    else if (item.State == ColorItemState.Highlighted)
                        borderColor = this.HighlightedBorderColor;
                    else
                        borderColor = this.SelectedBorderColor;

                    g.DrawRectangle(new Pen(borderColor), rect);

                    if (item.State == ColorItemState.Highlighted ||
                        item.State == ColorItemState.Selected)
                    {
                        rect.Inflate(-1, -1);

                        g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), rect);
                    }

                    if (group.IsSubItemsVisible)
                    {
                        this.DrawInheritItems(g, group, item, inheritArea);
                    }
                }
            }
        }

        protected virtual void DrawInheritItems(Graphics g, ColorUIAdvGroup group, GroupColorItem colorItem, Rectangle inheritArea)
        {
            ColorItemCollection col = colorItem.SubItems;

            Size size = group.ParentControl.ColorItemSize;

            if (col != null && col.Count > 0)
            {
                for (int i = 0; i < group.SubItemsDepth; i++)
                {
                    if (i < col.Count)
                    {
                        ColorItem item = col[i];
                        Rectangle rect = item.Bounds;
                        using (Brush brush = new SolidBrush(item.Color))
                        {
                            g.FillRectangle(brush, rect);
                        }

                        if (this.ColorPicker.VerticalItemsSpacing != 0)
                        {
                            rect.Width--;
                            rect.Height--;

                            Color borderColor = Color.Empty;

                            if (item.State == ColorItemState.Normal)
                                borderColor = this.ItemBorderColor;
                            else if (item.State == ColorItemState.Highlighted)
                                borderColor = this.HighlightedBorderColor;
                            else
                                borderColor = this.SelectedBorderColor;

                            g.DrawRectangle(new Pen(borderColor), rect);

                            if (item.State == ColorItemState.Highlighted ||
                                item.State == ColorItemState.Selected)
                            {
                                rect.Inflate(-1, -1);

                                g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), rect);
                            }
                        }
                    }
                }

                if (this.ColorPicker.VerticalItemsSpacing == 0)
                {
                    int width = this.ColorPicker.ColorItemSize.Width;
                    int height = this.ColorPicker.ColorItemSize.Height * group.SubItemsDepth;

                    int x = this.ColorPicker.BorderOffset + (this.ColorPicker.HorizontalItemsSpacing + size.Width) * colorItem.Index;
                    int yOffcet = inheritArea.Location.Y;
                    g.DrawRectangle(new Pen(this.ItemBorderColor), new Rectangle(x, yOffcet, width - 1, height - 1));
                    if (this.ColorPicker.HighLightedItem != null || this.ColorPicker.SelectedItem != null)
                    {
                        if (this.ColorPicker.HighLightedItem != null)
                        {
                            Rectangle bounds = this.ColorPicker.HighLightedItem.Bounds;

                            bounds.Width--;
                            bounds.Height--;
                            g.DrawRectangle(new Pen(this.HighlightedBorderColor), bounds);

                            bounds.Inflate(-1, -1);
                            g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), bounds);
                        }

                        if (this.ColorPicker.HighLightedItem != this.ColorPicker.SelectedItem &&
                            this.ColorPicker.SelectedItem != null)
                        {
                            Rectangle bounds = this.ColorPicker.SelectedItem.Bounds;

                            bounds.Width--;
                            bounds.Height--;
                            g.DrawRectangle(new Pen(this.SelectedBorderColor), bounds);

                            bounds.Inflate(-1, -1);
                            g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), bounds);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// ColorUIAdv Office2007 Renderer class
    /// </summary>
    public class ColorUIAdvOffice2007Renderer : ColorUIAdvRenderer
    {
        public ColorUIAdvOffice2007Renderer(ColorPickerUIAdv control) :
            base(control)
        {
        }

        protected Office2007Colors ColorTable
        {
            get
            {
                return Office2007Colors.GetColorTable(this.ColorPicker.Office2007Theme);
            }
        }

        protected override Color ForeColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvTextColor;
            }
        }

        protected override Color ItemBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvItemBorderColor;
            }
        }

        protected override Color BackColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvBackColor;
            }
        }

        protected override Color HighlightedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvHighlightedBorderColor;
            }
        }

        protected override Color SelectedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvSelectedBorderColor;
            }
        }

        protected override Color SelectedHighlightedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvSelectedHighlightedBorderColor;
            }
        }

        protected override Color GroupHeaderBackColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvGroupHeaderBackColor;
            }
        }
    }
    public class ColorUIAdvOffice2010Renderer : ColorUIAdvRenderer
    {
        public ColorUIAdvOffice2010Renderer(ColorPickerUIAdv control) :
            base(control)
        {
        }

        protected Office2010Colors ColorTable
        {
            get
            {
                return Office2010Colors.GetColorTable(this.ColorPicker.Office2010Theme);
            }
        }

        protected override Color ForeColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvTextColor;
            }
        }

        protected override Color ItemBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvItemBorderColor;
            }
        }

        protected override Color BackColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvBackColor;
            }
        }

        protected override Color HighlightedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvHighlightedBorderColor;
            }
        }

        protected override Color SelectedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvSelectedBorderColor;
            }
        }

        protected override Color SelectedHighlightedBorderColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvSelectedHighlightedBorderColor;
            }
        }

        protected override Color GroupHeaderBackColor
        {
            get
            {
                return this.ColorTable.ColorUIAdvGroupHeaderBackColor;
            }
        }
    }
    public class ColorUIAdvMetroRenderer
    {
        private const int horizontalOffcet = 5;
        private const int verticalOffcet = 2;

        private static readonly Color c_ItemBorderColor = Color.White;
        private static readonly Color c_HighlightedBorderColor = Color.FromArgb(243, 148, 54);
        private static readonly Color c_SelectedBorderColor = Color.FromArgb(235, 75, 13);
        private static readonly Color c_SelectedHighlightedBorderColor = Color.Black;
        private static readonly Color c_DefaultBackColor = Color.White;
        private static readonly Color c_GroupHeaderBackColor = ColorTranslator.FromHtml("#D1D3D4");

        private const ContentAlignment anyRight = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        private const ContentAlignment anyBottom = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        private const ContentAlignment anyCenter = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        private const ContentAlignment anyMiddle = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;

        protected Point m_textPoint = Point.Empty;
        private Rectangle m_bounds = Rectangle.Empty;
        private ColorPickerUIAdv m_colorPicker = null;

        protected virtual Color BackColor
        {
            get { return c_DefaultBackColor; }
        }

        protected Rectangle Bounds
        {
            get { return m_bounds; }
        }

        public ColorPickerUIAdv ColorPicker
        {
            get { return m_colorPicker; }
        }

        protected virtual Color ItemBorderColor
        {
            get { return c_ItemBorderColor; }
        }

        protected virtual Color HighlightedBorderColor
        {
            get { return c_HighlightedBorderColor; }
        }

        protected virtual Color SelectedBorderColor
        {
            get { return c_SelectedBorderColor; }
        }

        protected virtual Color SelectedHighlightedBorderColor
        {
            get { return c_SelectedHighlightedBorderColor; }
        }

        protected virtual Color GroupHeaderBackColor
        {
            get { return c_GroupHeaderBackColor; }
        }

        public ColorUIAdvMetroRenderer(ColorPickerUIAdv control)
        {
            m_colorPicker = control;
            m_bounds = control.ClientRectangle;
           
        }

        public virtual void OnPaint(PaintEventArgs e)
        {

            // Draw Background
            using (Brush brush = new SolidBrush(this.BackColor))
                e.Graphics.FillRectangle(brush, this.ColorPicker.ClientRectangle);

            this.DrawInterior(e.Graphics);
        }

        private Point m_focus = new Point(-1, -1);
        private Color m_foreColor = Color.Black;
        protected virtual Color ForeColor
        {
            get { return m_foreColor; }
        }

        protected virtual void DrawInterior(Graphics g)
        {
            Rectangle rect = this.Bounds;

            if (this.ColorPicker.Groups != null && this.ColorPicker.Groups.Count != 0)
            {
                for (int i = 0; i < this.ColorPicker.Groups.Count; i++)
                {
                    ColorUIAdvGroup group = this.ColorPicker.Groups[i];

                    if (group.Visible)
                    {
                        this.DrawGroupHeader(g, group);

                        this.DrawGroupInterior(g, group);
                    }
                }
            }
        }

        protected virtual void DrawGroupHeader(Graphics g, ColorUIAdvGroup group)
        {
           //  fill header
            Rectangle header = Rectangle.Empty;

            if (group.Header.Dock == DockStyle.Top)
            {
                header = new Rectangle(group.Bounds.X, group.Bounds.Y, group.Bounds.Width, group.HeaderHeight);
            }
            using(Brush brush=new SolidBrush(Color.LightGray))
            {
                g.FillRectangle(brush, header);
            }

            this.DrawText(g, group, header);
        }

        public virtual void DrawText(Graphics g, ColorUIAdvGroup group, Rectangle labelBounds)
        {
            this.ComputeTextPosition(group, labelBounds);

            DrawParams param = new DrawParams();
            param.Enabled = this.ColorPicker.Enabled;
            param.ControlBackColor = this.ColorPicker.BackColor;
            param.Bounds = CalcTextRectangle(g, group, this.ColorPicker.TextAlign, labelBounds);
            param.Align = this.ColorPicker.TextAlign;

            this.DrawTextInternal(g, group.Name, this.ForeColor, this.ColorPicker.Font, m_textPoint, param);
        }

        private void DrawTextInternal(Graphics g, string str, Color textColor, Font textFont, PointF pt, DrawParams param)
        {
            using (StringFormat fmt = new StringFormat())
            {
                fmt.HotkeyPrefix = HotkeyPrefix.Show;
                fmt.Alignment = TranslateAlignment(param.Align);
                fmt.LineAlignment = TranslateLineAlignment(param.Align);

                RectangleF textRect = param.Bounds;

                if (param.Enabled)
                {
                    using (Brush brush = new SolidBrush(textColor))
                    {
                        g.DrawString(str, textFont, brush, textRect, fmt);
                    }
                }
                else
                {
                    ControlPaint.DrawStringDisabled(g, str, textFont, param.ControlBackColor, param.Bounds, fmt);
                }
            }
        }

        private StringAlignment TranslateAlignment(ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                return StringAlignment.Far;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                return StringAlignment.Center;
            }

            return StringAlignment.Near;
        }

        private StringAlignment TranslateLineAlignment(ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                return StringAlignment.Far;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                return StringAlignment.Center;
            }

            return StringAlignment.Near;
        }

        private Rectangle VAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyBottom) != ((ContentAlignment)0))
            {
                withinThis.Y += withinThis.Height - alignThis.Height;
            }
            else if ((align & anyMiddle) != ((ContentAlignment)0))
            {
                withinThis.Y += (withinThis.Height - alignThis.Height) / 2;
            }

            withinThis.Height = alignThis.Height;

            return withinThis;
        }

        private Rectangle HAlignWithin(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & anyRight) != ((ContentAlignment)0))
            {
                withinThis.X += withinThis.Width - alignThis.Width;
            }
            else if ((align & anyCenter) != ((ContentAlignment)0))
            {
                withinThis.X += (withinThis.Width - alignThis.Width) / 2;
            }

            withinThis.Width = alignThis.Width;

            return withinThis;
        }

        public virtual void ComputeTextPosition(ColorUIAdvGroup group, Rectangle labelBounds)
        {
            Graphics g = this.ColorPicker.CreateGraphics();

            StringFormat sf = (StringFormat)StringFormat.GenericTypographic.Clone();
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            SizeF textSize = g.MeasureString(group.Name, this.ColorPicker.Font, this.ColorPicker.Width, sf);
            g.Dispose();

            Rectangle buttonRect = labelBounds;

            int yOffcet = labelBounds.Y;

            int horizontalGap = (int)Math.Max(0, (buttonRect.Width - 2 * horizontalOffcet - textSize.Width) / 2);
            int verticalGap = (int)Math.Max(0, (buttonRect.Height - 2 * verticalOffcet - textSize.Height) / 2);

            switch (this.ColorPicker.TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;
                case ContentAlignment.BottomLeft:
                    m_textPoint = new Point(horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;
                case ContentAlignment.BottomRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, 2 * verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleLeft:
                    m_textPoint = new Point(horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.MiddleRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, verticalGap + verticalOffcet);
                    break;

                case ContentAlignment.TopCenter:
                    m_textPoint = new Point(horizontalGap + horizontalOffcet, verticalOffcet);
                    break;

                case ContentAlignment.TopLeft:
                    m_textPoint = new Point(horizontalOffcet, verticalOffcet);
                    break;

                case ContentAlignment.TopRight:
                    m_textPoint = new Point(horizontalGap * 2 + horizontalOffcet, verticalOffcet);
                    break;
            }

            m_textPoint.Y += yOffcet;
        }

        protected virtual Rectangle CalcTextRectangle(Graphics g, ColorUIAdvGroup group, ContentAlignment align, Rectangle labelBounds)
        {
            Rectangle clientRect = labelBounds;
            clientRect.Inflate(-horizontalOffcet, -verticalOffcet);
            Rectangle textRect = clientRect;

            if (g != null)
            {
                Size textSize;
                using (StringFormat format = new StringFormat())
                {
                    SizeF efSize = g.MeasureString(group.Name, this.ColorPicker.Font, new SizeF((float)textRect.Width, (float)textRect.Height), format);
                    textSize = Size.Ceiling(efSize);
                }

                textRect = this.HAlignWithin(textSize, textRect, align);
                textRect = this.VAlignWithin(textSize, textRect, align);
            }

            int num1 = Math.Min(textRect.Bottom, clientRect.Bottom);
            textRect.Y = Math.Max(textRect.Y, clientRect.Y);
            textRect.Height = num1 - textRect.Y;

            return textRect;
        }

        protected virtual void DrawGroupInterior(Graphics g, ColorUIAdvGroup group)
        {
            if (group.Items != null && group.Items.Count > 0)
            {
                Rectangle drawRect = group.Bounds;

                int yOffcet = drawRect.Y;

                if (group.Header.Dock == DockStyle.Top)
                    yOffcet += group.HeaderHeight;

                int defYOffcet = yOffcet;
                if (group.IsSubItemsVisible)
                {
                    defYOffcet += group.ParentControl.BorderOffset + group.ParentControl.ColorItemSize.Height;

                    if (group.ParentControl.VerticalItemsSpacing < ColorPickerUIAdv.DEF_BASECOLORSOFFSET)
                        defYOffcet += ColorPickerUIAdv.DEF_BASECOLORSOFFSET;
                    else
                        defYOffcet += group.ParentControl.VerticalItemsSpacing;
                }
                else
                {
                    defYOffcet += group.ParentControl.BorderOffset * 2 + group.ParentControl.ColorItemSize.Height;
                }

                Rectangle inheritArea = new Rectangle(new Point(0, defYOffcet), new Size(group.Size.Width, group.Size.Height - defYOffcet));

                for (int i = 0; i < group.Items.Count; i++)
                {
                    GroupColorItem item = group.Items[i] as GroupColorItem;
                    Rectangle rect = item.Bounds;

                    Color fillColor = item.Color;
                    using (Brush brush = new SolidBrush(fillColor))
                        g.FillRectangle(brush, rect);

                    rect.Width--;
                    rect.Height--;

                    Color borderColor = Color.Empty;

                    if (item.State == ColorItemState.Normal)
                        borderColor = this.ItemBorderColor;
                    else if (item.State == ColorItemState.Highlighted)
                        borderColor = this.HighlightedBorderColor;
                    else
                        borderColor = this.SelectedBorderColor;

                    g.DrawRectangle(new Pen(borderColor), rect);

                    if (item.State == ColorItemState.Highlighted ||
                        item.State == ColorItemState.Selected)
                    {
                       // rect.Inflate(-1, -1);

                        g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), rect);
                    }

                    if (group.IsSubItemsVisible)
                    {
                        this.DrawInheritItems(g, group, item, inheritArea);
                    }
                }
            }
        }

        protected virtual void DrawInheritItems(Graphics g, ColorUIAdvGroup group, GroupColorItem colorItem, Rectangle inheritArea)
        {
            ColorItemCollection col = colorItem.SubItems;

            Size size = group.ParentControl.ColorItemSize;

            if (col != null && col.Count > 0)
            {
                for (int i = 0; i < group.SubItemsDepth; i++)
                {
                    if (i < col.Count)
                    {
                        ColorItem item = col[i];
                        Rectangle rect = item.Bounds;

                        using (Brush brush = new SolidBrush(item.Color))
                            g.FillRectangle(brush, rect);

                        if (this.ColorPicker.VerticalItemsSpacing != 0)
                        {
                            rect.Width--;
                            rect.Height--;

                            Color borderColor = Color.Empty;

                            if (item.State == ColorItemState.Normal)
                                borderColor = this.ItemBorderColor;
                            else if (item.State == ColorItemState.Highlighted)
                                borderColor = this.HighlightedBorderColor;
                            else
                                borderColor = this.SelectedBorderColor;

                            g.DrawRectangle(new Pen(borderColor), rect);

                            if (item.State == ColorItemState.Highlighted ||
                                item.State == ColorItemState.Selected)
                            {
                             //  rect.Inflate(1, 1);

                                g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), rect);
                            }
                        }
                    }
                }

                if (this.ColorPicker.VerticalItemsSpacing == 0)
                {
                    int width = this.ColorPicker.ColorItemSize.Width;
                    int height = this.ColorPicker.ColorItemSize.Height * group.SubItemsDepth;

                    int x = this.ColorPicker.BorderOffset + (this.ColorPicker.HorizontalItemsSpacing + size.Width) * colorItem.Index;
                    int yOffcet = inheritArea.Location.Y;
                    g.DrawRectangle(new Pen(this.ItemBorderColor), new Rectangle(x, yOffcet, width - 1, height - 1));
                    if (this.ColorPicker.HighLightedItem != null || this.ColorPicker.SelectedItem != null)
                    {
                        if (this.ColorPicker.HighLightedItem != null)
                        {
                            Rectangle bounds = this.ColorPicker.HighLightedItem.Bounds;

                            bounds.Width--;
                            bounds.Height--;
                            g.DrawRectangle(new Pen(this.HighlightedBorderColor), bounds);

                          //  bounds.Inflate(-1, -1);
                            g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), bounds);
                        }

                        if (this.ColorPicker.HighLightedItem != this.ColorPicker.SelectedItem &&
                            this.ColorPicker.SelectedItem != null)
                        {
                            Rectangle bounds = this.ColorPicker.SelectedItem.Bounds;

                            bounds.Width--;
                            bounds.Height--;
                            g.DrawRectangle(new Pen(this.SelectedBorderColor), bounds);

                          //  bounds.Inflate(-1, -1);
                            g.DrawRectangle(new Pen(this.SelectedHighlightedBorderColor), bounds);
                        }
                    }
                }
            }
        }
    }
}