#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// TabRenderer Office2010.
    /// </summary>
    public class TabRendererOffice2010 : TabRenderer2D
    {
        #region Class Constants
        private const int DEF_CORNER_RADIUS = 2;
        private const string DEF_RENDERER_NAME = "Office2010Style";
        #endregion

        #region Class Members

        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyOffice2010 m_tabPropertyExtender;

        /// <summary>
        /// Contains information about Office2010 theme colors.
        /// </summary>
        private Office2010Colors m_themeColors;

        /// <summary>
        /// Contains information about Office2010 color theme.
        /// </summary>
        private Office2010Theme m_theme;

        /// <summary>
        /// The TabControl.
        /// </summary>
        private ITabControl m_parent;

        /// <summary>
        /// Blend for drawing top ellipse when office2010ColorScheme is black and tabPage is highlighted.
        /// </summary>
        private Blend m_topEllipseBlend;

        /// <summary>
        /// Blend for drawing bottom ellipse when office2010ColorScheme is black and tabPage is highlighted.
        /// </summary>
        private Blend m_bottomEllipseBlend;

        #endregion

        #region Class Properties

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
        public static new StyleRendererPropertyOffice2010 TabPanelPropertyExtender
        {
            get
            {
                return m_tabPropertyExtender;
            }
        }

        /// <summary>
        /// Gets information about Office2010 theme colors.
        /// </summary>
        public Office2010Colors ThemeColors
        {
            get { return m_themeColors; }
        }

        /// <summary>
        /// Gets or sets information about Office2010 color theme.
        /// </summary>
        public Office2010Theme Theme
        {
            get
            {
                return m_theme;
            }
            set
            {
                if (m_theme != value)
                {
                    m_theme = value;

                    OnOffice2010ColorSchemeChanged();
                }
            }
        }

        public override Color TabBorderColor
        {
            get
            {
                if (this.ThemeColors != null)
                {
                    return ThemeColors.TabDefaultBorderColor;
                }
                else
                {
                    return base.TabBorderColor;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize Methods

        static TabRendererOffice2010()
        {
            m_tabPropertyExtender = new StyleRendererPropertyOffice2010();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererOffice2010), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyOffice2010();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererOffice2010), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererOffice2010 class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererOffice2010(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
            m_parent = parent;
            this.Theme = parent.Office2010ColorTheme;

            OnOffice2010ColorSchemeChanged();

            m_topEllipseBlend = new Blend();
            m_topEllipseBlend.Positions = new float[] { 0, 0.3f, 0.3f, 1 };
            m_topEllipseBlend.Factors = new float[] { 0, 0.2f, 0.25f, 1 };

            m_bottomEllipseBlend = new Blend();
            m_bottomEllipseBlend.Positions = new float[] { 0, 0.3f, 0.3f, 0.6f, 0.6f, 1 };
            m_bottomEllipseBlend.Factors = new float[] { 0, 0.3f, 0.3f, 0.8f, 0.8f, 1 };

            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010Colors_ManagedColorsApplied);
        }

        #endregion

        #region Class Overrides

        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF size = base.GetPreferredSize(g);

            return size;
        }

        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            tabSize.Width = 0;
            tabSize.Height = 0;
            return tabSize;
        }

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            // do nothing here
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            if (curBounds.Width > 0 && curBounds.Height > 0)
            {
                // Make g horizontal
                ApplyTransform(g);

                SaveGraphicsState(g, ref curBounds);
                SmoothingMode oldSM = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                bool bIsSelected = (drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected;
                bool bIsHotLight = (drawItemInfo.State & DrawItemState.HotLight) == DrawItemState.HotLight;

                if (bIsHotLight)
                {
                    if (bIsSelected)
                    {
                        // draw 'ouuter' border
                        using (GraphicsPath path = GetBorderPathFromBounds(curBounds))
                        {
                            using (Pen borderPen = new Pen(ThemeColors.TabSelectedHotLightBorderColor, 1))
                            {
                                g.DrawPath(borderPen, path);
                            }
                        }

                        // draw 'inner' border
                        RectangleF innerBounds = curBounds;
                        innerBounds.Inflate(-1, -1);

                        using (GraphicsPath innerPath = this.GetBorderPathFromBounds(innerBounds))
                        {
                            using (Pen innerPen = new Pen(ThemeColors.TabSelectedHotLightInnerBorderColor, 1))
                            {
                                drawItemInfo.Graphics.DrawPath(innerPen, innerPath);
                            }
                        }
                    }
                    else
                    {
                        using (GraphicsPath path = GetBorderPathFromBounds(curBounds))
                        {
                            using (Pen borderPen = new Pen(ThemeColors.TabDefaultBorderColor))
                            {
                                g.DrawPath(borderPen, path);
                            }
                        }

                        RectangleF innerBounds = curBounds;
                        innerBounds.Inflate(-1, -1);
                        innerBounds.Height++;

                        if (m_parent.Office2010ColorTheme != Office2010Theme.Black)
                        {
                            using (GraphicsPath innerPath = this.GetBorderPathFromBounds(innerBounds))
                            {
                                using (Pen innerPen = new Pen(ThemeColors.TabHighlightInnerBorderColor, 1))
                                {
                                    drawItemInfo.Graphics.DrawPath(innerPen, innerPath);
                                }
                            }
                        }

                        using (Pen borderLinePen = new Pen(ThemeColors.TabHotLightBottomBorderLineColor, 1))
                        {
                            g.DrawLine(borderLinePen, curBounds.Left + 1, curBounds.Bottom, curBounds.Right - 1, curBounds.Bottom);
                        }
                    }
                }
                else if (bIsSelected)
                {
                    // draw 'ouuter' border
                    using (GraphicsPath path = GetBorderPathFromBounds(curBounds))
                    {
                        using (Pen borderPen = new Pen(ThemeColors.TabDefaultBorderColor, 1))
                        {
                            g.DrawPath(borderPen, path);
                        }
                    }

                    // draw 'inner' border
                    RectangleF innerBounds = curBounds;
                    innerBounds.Inflate(-1, -1);

                    using (GraphicsPath innerPath = this.GetBorderPathFromBounds(innerBounds))
                    {
                        using (Pen innerPen = new Pen(ThemeColors.TabSelectedInnerBorderColor, 1))
                        {
                            drawItemInfo.Graphics.DrawPath(innerPen, innerPath);
                        }
                    }
                }

                g.SmoothingMode = oldSM;

                RestoreGraphicsState(g);

                g.ResetTransform();
            }
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            bool bIsSelected = (drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool bIsHotLight = (drawItemInfo.State & DrawItemState.HotLight) == DrawItemState.HotLight;

            if (bIsSelected)
            {
                DrawSelectedBackground(drawItemInfo);
            }
            else
                if (bIsHotLight)
                {
                    DrawHotLightBackground(drawItemInfo);
                }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DrawItemCallback = null;
                Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010Colors_ManagedColorsApplied);
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Class Utility Methods

        private void DrawDefaultBackground(DrawTabEventArgs drawItemInfo)
        {
        }

        private void DrawSelectedBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            if (curBounds.Width > 0 && curBounds.Height > 0)
            {
                // Make g horizontal
                ApplyTransform(g);

                SaveGraphicsState(g, ref curBounds);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                Color cGradientBegin = ThemeColors.TabSelectedGradientTopColor;
                Color cGradientEnd = ThemeColors.TabSelectedGradientBottomColor;

                if (drawItemInfo.BackColor != Color.Empty)
                    cGradientBegin = cGradientEnd = drawItemInfo.BackColor;

                using (GraphicsPath path = GetBorderPathFromBounds(curBounds))
                {
                    using (LinearGradientBrush gradientBrush = new LinearGradientBrush(curBounds, cGradientBegin, cGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillPath(gradientBrush, path);
                    }
                }

                RestoreGraphicsState(g);

                g.ResetTransform();
            }
        }

        private void DrawHotLightBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            if (curBounds.Width > 0 && curBounds.Height > 0)
            {
                // Make g horizontal
                ApplyTransform(g);

                SaveGraphicsState(g, ref curBounds);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                // draw background
                RectangleF bounds = curBounds;
                bounds.Inflate(-1, -1);

                if (m_parent.Office2010ColorTheme != Office2010Theme.Black)
                {
                    using (GraphicsPath path = GetBorderPathFromBounds(curBounds))
                    {
                        using (LinearGradientBrush gradientBrush = new LinearGradientBrush(curBounds, ThemeColors.TabHotLightGradientTopBeginColor, ThemeColors.TabHotLightGradientBottomEndColor, LinearGradientMode.Vertical))
                        {
                            ColorBlend blend = new ColorBlend();
                            blend.Colors = new Color[] { ThemeColors.TabHotLightGradientTopBeginColor, ThemeColors.TabHotLightGradientTopEndColor, ThemeColors.TabHotLightGradientBottomBeginColor, ThemeColors.TabHotLightGradientBottomEndColor };
                            blend.Positions = new float[] { 0, 0.5f, 0.5f, 1 };
                            gradientBrush.InterpolationColors = blend;

                            g.FillPath(gradientBrush, path);
                        }
                    }

                    // draw top-ellipse
                    bounds = curBounds;

                    bounds.Inflate(-5, 0);
                    float height = bounds.Height;
                    bounds.Offset(0, -height);
                    bounds.Height = height * 2;

                    if (bounds.Width > 0 && bounds.Height > 0)
                    {
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            path.AddEllipse(bounds);

                            using (PathGradientBrush topBrush = new PathGradientBrush(path))
                            {
                                topBrush.CenterColor = ThemeColors.TabHotLightGradientCircleColor;
                                Color endColor = Color.FromArgb(50, ThemeColors.TabHotLightGradientCircleColor);
                                topBrush.SurroundColors = new Color[] { endColor };

                                g.FillPath(topBrush, path);
                            }
                        }
                    }
                }
                else
                {
                    bounds = new RectangleF(curBounds.X, curBounds.Y, curBounds.Width + 1, curBounds.Height / 2);

                    using (SolidBrush gradientBrush = new SolidBrush(ThemeColors.TabHotLightGradientCircleColor))
                    {
                        g.FillRectangle(gradientBrush, curBounds);
                    }

                    using (GraphicsPath path = GetTopEllipseRoundedPath(bounds))
                    {
                        using (LinearGradientBrush gradientBrush = new LinearGradientBrush(bounds, ThemeColors.TabHotLightGradientTopBeginColor, ThemeColors.TabHotLightGradientTopEndColor, LinearGradientMode.Vertical))
                        {
                            gradientBrush.Blend = m_topEllipseBlend;

                            g.FillPath(gradientBrush, path);
                        }
                    }

                    // draw bottom-ellipse
                    bounds = new RectangleF(curBounds.X, curBounds.Y + curBounds.Height * 0.6f, curBounds.Width, curBounds.Height);
                    bounds.Inflate(curBounds.Width / 5, 0);

                    if (bounds.Width > 0 && bounds.Height > 0)
                    {
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            path.AddEllipse(bounds);

                            using (PathGradientBrush brush = new PathGradientBrush(path))
                            {
                                brush.CenterColor = ThemeColors.TabHotLightGradientBottomEndColor;
                                brush.SurroundColors = new Color[] { Color.FromArgb(50, ThemeColors.TabHotLightGradientBottomBeginColor) };

                                brush.Blend = m_bottomEllipseBlend;

                                g.FillPath(brush, path);
                            }
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

            PointF[] aptLines = new PointF[] { new PointF(bounds.Left, bounds.Bottom), new PointF(bounds.Left, bounds.Top + DEF_CORNER_RADIUS), new PointF(bounds.Left + DEF_CORNER_RADIUS, bounds.Top) };

            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(bounds.Right - DEF_CORNER_RADIUS, bounds.Top), new PointF(bounds.Right, bounds.Top + DEF_CORNER_RADIUS), new PointF(bounds.Right, bounds.Bottom) };

            path.AddLines(aptLines);

            return path;
        }

        protected override Color GetForeColor()
        {
            return ThemeColors.TabForeColor;
        }

        protected override Color GetActiveForeColor()
        {
            return ThemeColors.ActiveTabForeColor;
        }

        /// <summary>
        /// Gets specified rounded path for the rectangle for drawing top ellipse 
        /// when office2010ColorScheme is black and tabPage is highlighted.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics Path</returns>
        protected GraphicsPath GetTopEllipseRoundedPath(RectangleF bounds)
        {
            float iLeft = bounds.X;
            float iTop = bounds.Y;
            float iRight = bounds.Right - 1;
            float iBottom = bounds.Bottom - 1;

            GraphicsPath path = new GraphicsPath();

            path.AddLine(iLeft, iBottom - 3, iLeft, iTop + 2);
            path.AddLine(iLeft, iTop + 2, iLeft + 2, iTop);
            path.AddLine(iLeft + 2, iTop, iRight - 2, iTop);
            path.AddLine(iRight - 2, iTop, iRight, iTop + 2);
            path.AddLine(iRight, iTop + 2, iRight, iBottom - 3);
            path.AddLine(iRight, iBottom - 3, iRight - 3, iBottom);
            path.AddLine(iRight - 3, iBottom, iLeft + 3, iBottom);
            path.AddLine(iLeft + 3, iBottom, iLeft, iBottom - 3);

            return path;
        }

        private void Office2010Colors_ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
        {
            if (m_theme == Office2010Theme.Managed)
            {
                OnOffice2010ColorSchemeChanged();
            }
        }

        private void OnOffice2010ColorSchemeChanged()
        {
            m_themeColors = Office2010Colors.GetColorTable(m_theme);
        }

        #endregion
    }

    public class StyleRendererPropertyOffice2010 : TabPanelProperty2D
    {
        #region Class Constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        public const int OverlapHeight = 0;
        #endregion

        #region Class Overrides

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            TabRendererOffice2010 trOffice2010 = null;
            if (tabControl.Renderer.Renderers.Count > 0)
            {
                trOffice2010 = tabControl.Renderer.Renderers[0] as TabRendererOffice2010;
            }
            if (trOffice2010 != null)
            {
                return trOffice2010.ThemeColors.TabSelectedGradientBottomColor;
            }
            else
            {
                return Office2010Colors.Default.TabSelectedGradientBottomColor;
            }
        }

        public override Color DefaultTabForeColor(ITabPanelData panelData, ITabControl tabControl)
        {
            TabRendererOffice2010 trOffice2010 = null;
            if (tabControl.Renderer.Renderers.Count > 0)
            {
                trOffice2010 = tabControl.Renderer.Renderers[0] as TabRendererOffice2010;
            }
            if (trOffice2010 != null)
            {
                return trOffice2010.ThemeColors.TabForeColor;
            }
            else
            {
                return Office2010Colors.Default.TabForeColor;
            }
        }

        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            TabRendererOffice2010 trOffice2010 = null;
            if (tabControl.Renderer.Renderers.Count > 0)
            {
                trOffice2010 = tabControl.Renderer.Renderers[0] as TabRendererOffice2010;
            }
            Pen borderPen;
            Brush br = new SolidBrush(bgColor);
            Rectangle newBounds = new Rectangle(bounds.X, bounds.Y + OverlapHeight, bounds.Width, bounds.Height);
            if (trOffice2010 != null)
            {
                g.FillRectangle(br, newBounds);
                borderPen = new Pen(trOffice2010.ThemeColors.TabDefaultBorderColor);
            }
            else
            {
                g.FillRectangle(br, newBounds);
                borderPen = new Pen(Office2010Colors.Default.TabDefaultBorderColor);
            }

            g.DrawLine(borderPen, newBounds.Left, newBounds.Bottom, newBounds.Right, newBounds.Bottom);
            br.Dispose();
            borderPen.Dispose();
        }

        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        /// <param name="tabSize">Tab Size</param>
        /// <returns>Overlap Size</returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF overlapSize = base.GetOverlapSize(tabSize);
            overlapSize.Height = OverlapHeight;
            return overlapSize;
        }

        #endregion
    }
}
