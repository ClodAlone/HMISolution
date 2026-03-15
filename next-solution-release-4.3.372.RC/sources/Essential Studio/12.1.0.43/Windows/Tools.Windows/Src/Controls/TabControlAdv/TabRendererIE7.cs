#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererOffice2003"/>
    /// tab style.
    /// </summary>
    public class StyleRendererPropertyIE7 : TabPanelProperty2D
    {
        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 9;
        #endregion

        #region Class overrides

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(200, 224, 250);
        }

        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            g.FillRectangle(new SolidBrush(bgColor), bounds);
        }

        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        /// <param name="tabSize">tab size</param>
        /// <returns>Returns Overlap size</returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF overlapSize = base.GetOverlapSize(tabSize);
            overlapSize.Height = DEF_OVERLAP_HEIGHT;
            return overlapSize;
        }

        #endregion
    }

    /// <summary>
    /// This renderer allows user to draw TabPages with Internet Explorer 7 style.
    /// </summary>
    public class TabRendererIE7 : TabRenderer2D
    {
        #region Constants
        private const int DEF_CORNER_RADIUS = 3;
        private const int DEF_OVERLAP_HEIGHT = 2;
        private const string DEF_RENDERER_NAME = "InternetExplorer7Style";

        private static readonly Color DEF_BORDER_COLOR = Color.FromArgb(145, 150, 162);

        private static readonly Color DEF_BACKCOLOR_TOP_START = Color.FromArgb(242, 245, 250);
        private static readonly Color DEF_BACKCOLOR_TOP_END = Color.FromArgb(229, 234, 245);
        private static readonly Color DEF_BACKCOLOR_MIDDLE_START = Color.FromArgb(207, 215, 235);
        private static readonly Color DEF_BACKCOLOR_MIDDLE_END = Color.FromArgb(216, 222, 240);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_START = Color.FromArgb(222, 227, 244);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_END = Color.FromArgb(238, 240, 253);

        private static readonly Color DEF_BACKCOLOR_TOP_START_SELECTED = Color.FromArgb(252, 253, 253);
        private static readonly Color DEF_BACKCOLOR_TOP_END_SELECTED = Color.FromArgb(231, 245, 251);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_START_SELECTED = Color.FromArgb(207, 231, 250);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_END_SELECTED = Color.FromArgb(185, 209, 250);

        private static readonly Color DEF_BACKCOLOR_TOP_START_HIGHLIGHTED = Color.FromArgb(237, 243, 252);
        private static readonly Color DEF_BACKCOLOR_TOP_END_HIGHLIGHTED = Color.FromArgb(198, 221, 247);
        private static readonly Color DEF_BACKCOLOR_MIDDLE_START_HIGHLIGHTED = Color.FromArgb(153, 198, 238);
        private static readonly Color DEF_BACKCOLOR_MIDDLE_END_HIGHLIGHTED = Color.FromArgb(164, 204, 238);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_START_HIGHLIGHTED = Color.FromArgb(176, 211, 240);
        private static readonly Color DEF_BACKCOLOR_BOTTOM_END_HIGHLIGHTED = Color.FromArgb(224, 236, 251);

        #endregion

        #region members
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyIE7 m_tabPropertyExtender;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the unique name of this tab renderer.
        /// </summary>
        public static new string TabStyleName
        {
            get
            {
                return DEF_RENDERER_NAME;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static new StyleRendererPropertyIE7 TabPanelPropertyExtender
        {
            get
            {
                return m_tabPropertyExtender;
            }
        }

        public override Color TabBorderColor
        {
            get
            {
                return DEF_BORDER_COLOR;
            }
        }
        #endregion

        #region Initialization
        static TabRendererIE7()
        {
            m_tabPropertyExtender = new StyleRendererPropertyIE7();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererIE7), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyIE7();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererIE7), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererIE7 class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererIE7(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }
        #endregion

        #region Overrides
        public override RectangleF Bounds
        {
            get
            {
                return base.Bounds;
            }
            set
            {
                base.Bounds = value;
            }
        }
        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF size = base.GetPreferredSize(g);
            size.Height += DEF_OVERLAP_HEIGHT;
            return size;
        }
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            tabSize.Height = DEF_OVERLAP_HEIGHT;
            tabSize.Width = 0;
            return tabSize;
        }

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            // Do nothing here
        }

        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
           Graphics g = drawItemInfo.Graphics;

            // Convert to horizontal co-ords
            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
            rectTextAndImage.Offset(0, -1);
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            // Transform g to horizontal co-ords
            this.ApplyTransform(g);

            // Determine text brush
            if (drawItemInfo.TextBrush == null)
            {
                if (((int)drawItemInfo.State & (int)DrawItemState.Selected) <= 0)
                    drawItemInfo.TextBrush = new SolidBrush(Color.FromArgb(192, drawItemInfo.ForeColor));
                else
                    drawItemInfo.TextBrush = new SolidBrush(drawItemInfo.ForeColor);
            }

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            if (ShowCloseButton)
            {
                DrawCloseButton(g, closeButtonBounds);
            }

            if (((int)drawItemInfo.State & (int)DrawItemState.Focus) > 0)
                DrawFocusRect(g, RectangleF.Inflate(this.Bounds, -1, -1), drawItemInfo.ForeColor, drawItemInfo.BackColor);

            g.ResetTransform();
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);

            SaveGraphicsState(g, ref curBounds);

            SmoothingMode oldSM = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if ((drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                curBounds.Height -= 1;
            }

            drawItemInfo.Graphics.DrawPath(new Pen(DEF_BORDER_COLOR, 1), this.GetBorderPathFromBounds(curBounds));

            RectangleF whiteBounds = curBounds;
            whiteBounds.Inflate(-1, -1);

            using (GraphicsPath whitePath = this.GetBorderPathFromBounds(whiteBounds))
            {
                drawItemInfo.Graphics.DrawPath(new Pen(Color.White, 1), whitePath);
            }

            g.SmoothingMode = oldSM;

            RestoreGraphicsState(g);

            g.ResetTransform();
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);

            SaveGraphicsState(g, ref curBounds);

            RectangleF bounds = curBounds;

            bounds.Inflate(-1, -1);

            // Checking whether one of dimensions is zero-sized
            if ((bounds.Width > 0.0) && (bounds.Height > 0.0))
            {
                using (GraphicsPath path = this.GetBorderPathFromBounds(bounds))
                {
                    using (Region region = new Region(path))
                    {
                        using (Brush brush = GetBackgroundBrush(bounds, drawItemInfo.State))
                        {
                            drawItemInfo.Graphics.FillRegion(brush, region);
                        }
                    }
                }

                RestoreGraphicsState(g);

                g.ResetTransform();
            }
        }
        protected virtual GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = new PointF[] { new PointF(bounds.Left, bounds.Bottom), new PointF(bounds.Left, bounds.Top + DEF_CORNER_RADIUS ), new PointF(bounds.Left + DEF_CORNER_RADIUS, bounds.Top) };
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(bounds.Right - DEF_CORNER_RADIUS, bounds.Top), new PointF(bounds.Right, bounds.Top + DEF_CORNER_RADIUS ), new PointF(bounds.Right, bounds.Bottom) };
            path.AddLines(aptLines);
            path.CloseFigure();

            return path;
        }

        protected virtual Brush GetBackgroundBrush(RectangleF bounds, DrawItemState state)
        {
            LinearGradientBrush brush = null;
            bool bIsSelected = (state & DrawItemState.Selected) == DrawItemState.Selected;
            bool bIsHighlighted = (state & DrawItemState.HotLight) == DrawItemState.HotLight;

            // Checking if our dimensions are able to be drawn
            if ((bounds.Width != 0) && (bounds.Height != 0))
            {
                if (bIsSelected)
                {
                    brush = new LinearGradientBrush(bounds, DEF_BACKCOLOR_TOP_START_SELECTED, DEF_BACKCOLOR_BOTTOM_END_SELECTED, LinearGradientMode.Vertical);

                    ColorBlend blend = new ColorBlend();
                    blend.Colors = new Color[] { DEF_BACKCOLOR_TOP_START_SELECTED, DEF_BACKCOLOR_TOP_END_SELECTED, DEF_BACKCOLOR_BOTTOM_START_SELECTED, DEF_BACKCOLOR_BOTTOM_END_SELECTED };

                    blend.Positions = new float[] { 0f, 0.33f, 0.33f, 1f };
                    brush.InterpolationColors = blend;
                }
                else if (bIsHighlighted)
                {
                    brush = new LinearGradientBrush(bounds, DEF_BACKCOLOR_TOP_START_HIGHLIGHTED, DEF_BACKCOLOR_BOTTOM_END_HIGHLIGHTED, LinearGradientMode.Vertical);

                    ColorBlend blend = new ColorBlend();
                    blend.Colors = new Color[] { DEF_BACKCOLOR_TOP_START_HIGHLIGHTED, DEF_BACKCOLOR_TOP_END_HIGHLIGHTED, DEF_BACKCOLOR_MIDDLE_START_HIGHLIGHTED, DEF_BACKCOLOR_MIDDLE_END_HIGHLIGHTED, DEF_BACKCOLOR_BOTTOM_START_HIGHLIGHTED, DEF_BACKCOLOR_BOTTOM_END_HIGHLIGHTED };

                    blend.Positions = new float[] { 0f, 0.33f, 0.33f, 0.65f, 0.66f, 1f };
                    brush.InterpolationColors = blend;
                }
                else
                {
                    brush = new LinearGradientBrush(bounds, DEF_BACKCOLOR_TOP_START, DEF_BACKCOLOR_BOTTOM_END, LinearGradientMode.Vertical);

                    ColorBlend blend = new ColorBlend();
                    blend.Colors = new Color[] { DEF_BACKCOLOR_TOP_START, DEF_BACKCOLOR_TOP_END, DEF_BACKCOLOR_MIDDLE_START, DEF_BACKCOLOR_MIDDLE_END, DEF_BACKCOLOR_BOTTOM_START, DEF_BACKCOLOR_BOTTOM_END };

                    blend.Positions = new float[] { 0f, 0.33f, 0.33f, 0.65f, 0.66f, 1f };
                    brush.InterpolationColors = blend;
                }
            }
            return brush;
        }

        #endregion
    }
}
