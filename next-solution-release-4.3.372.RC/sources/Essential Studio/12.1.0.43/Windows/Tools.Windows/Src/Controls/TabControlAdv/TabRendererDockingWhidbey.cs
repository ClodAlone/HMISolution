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
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererDockingWhidbey.cs"/>
    /// tab style.
    /// </summary>
    public class StyleRendererPropertyDockingWhidbey : TabPanelProperty2D
    {
        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 2;

        internal static Color BackgroundColor
        {
            get
            {
                Color backGround = Color.FromKnownColor(KnownColor.ControlLight);

                if (XPThemes.IsSilverThemeOn)
                    backGround = VS2005Colors.PanelColor;

                return backGround;
            }
        }

        #endregion

        #region Class overrides

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.White;
        }

        /// <summary>
        /// Indicates whether to draw ellipsis if text width is larger than tab width.
        /// </summary>
        public override bool DrawEllipsis
        {
            get
            {
                return true;
            }
        }

        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            g.FillRectangle(new SolidBrush(bgColor), bounds);
        }

        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        /// <param name="tabSize">Tab size</param>
        /// <returns>Returns the overlap size</returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF overlapSize = base.GetOverlapSize(tabSize);
            overlapSize.Height = DEF_OVERLAP_HEIGHT;
            return overlapSize;
        }
        #endregion
    }

    /// <summary>
    /// This renderer allows user to draw TabPages VS2005DockingStyle style.
    /// </summary>
    public class TabRendererDockingWhidbey : TabRenderer2D
    {
        #region Constants
        private const int DEF_CORNER_RADIUS = 2;
        private const int DEF_OVERLAP_HEIGHT = 2;
        private const string DEF_RENDERER_NAME = "VS2012DockingStyle";

        private static readonly Color DEF_BORDER_COLOR = Color.FromKnownColor(KnownColor.ControlDark);
        #endregion

        #region members
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyDockingWhidbey m_tabPropertyExtender;

        private RectangleF m_panelBounds = RectangleF.Empty;
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
        public static new StyleRendererPropertyDockingWhidbey TabPanelPropertyExtender
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
        static TabRendererDockingWhidbey()
        {
            m_tabPropertyExtender = new StyleRendererPropertyDockingWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingWhidbey), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyDockingWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingWhidbey), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererDockingWhidbey class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererDockingWhidbey(ITabControl parent, ITabPanelRenderer panelRenderer)
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

            // Transformed to horizontal alignment
            RectangleF currentBounds = TabUtils.ApplyTransform(g, TabAlignment, drawItemInfo.Bounds, true);
            m_panelBounds = TabUtils.ApplyTransform(g, TabAlignment, panelRenderer.Bounds, true);

            // This could instead be imp. as a no-transform version with ControlPaint
            this.ApplyTransform(g);

            float fLineX = 0.0F;
            bool bIsMirrored = panelRenderer.IsMirrored;

            using (Pen pen = new Pen(DEF_BORDER_COLOR))
            {
                if (((int)drawItemInfo.State & (int)DrawItemState.Selected) > 0)
                {
                    if (m_panelBounds.Y + m_panelBounds.Height - currentBounds.Y - currentBounds.Height > 0)
                    {
                        using (GraphicsPath path = this.GetTabBorderPath(currentBounds))
                        {
                            g.DrawPath(pen, path);
                        }
                        using (GraphicsPath path = this.GetLowerTabBorderPath())
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                    {
                        using (GraphicsPath path = this.GetBorderPathFromBounds(currentBounds))
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                }
                else
                {
                    fLineX = bIsMirrored ? currentBounds.Left : currentBounds.Right - 1;
                    g.DrawLine(pen, fLineX, currentBounds.Top + DEF_CORNER_RADIUS + 2, fLineX, currentBounds.Bottom - 5);
                }
            }

            g.ResetTransform();
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
            m_panelBounds = TabUtils.ApplyTransform(g, this.TabAlignment, panelRenderer.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);

            RectangleF bounds = curBounds;

            using (Brush brush = GetBackgroundBrush(drawItemInfo.State))
            {
                using (GraphicsPath path = this.GetTabBorderPath(bounds))
                {
                    drawItemInfo.Graphics.FillPath(brush, path);
                }
                using (GraphicsPath path = this.GetLowerTabBorderPath())
                {
                    drawItemInfo.Graphics.FillPath(brush, path);
                }
            }

            g.ResetTransform();
        }

        /// <summary>
        /// Gets the path for for the tab and the line beneath the tabs.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics Path</returns>
        protected virtual GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom), new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom - 3) };
                                  
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF( bounds.Left, bounds.Bottom - 3 ), new PointF( bounds.Left, bounds.Top + DEF_CORNER_RADIUS ), new PointF( bounds.Left + DEF_CORNER_RADIUS, bounds.Top ) }; 
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF( bounds.Right - DEF_CORNER_RADIUS, bounds.Top ), new PointF( bounds.Right, bounds.Top + DEF_CORNER_RADIUS ), new PointF( bounds.Right, bounds.Bottom - 3 ) }; 
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF( m_panelBounds.Right, m_panelBounds.Bottom - 3 ), new PointF( m_panelBounds.Right, m_panelBounds.Bottom ) }; 
            path.AddLines(aptLines);

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Gets the path for the tab.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics path</returns>
        protected virtual GraphicsPath GetTabBorderPath(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF( bounds.Left, bounds.Bottom ), new PointF( bounds.Left, bounds.Top + DEF_CORNER_RADIUS ), new PointF( bounds.Left + DEF_CORNER_RADIUS, bounds.Top ) };
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(bounds.Right - DEF_CORNER_RADIUS, bounds.Top), new PointF(bounds.Right, bounds.Top + DEF_CORNER_RADIUS), new PointF(bounds.Right, bounds.Bottom) };
                                                                         
            path.AddLines(aptLines);

            return path;
        }

        /// <summary>
        /// Gets the path for the line beneath the tabs.
        /// </summary>
        /// <returns>Returns Graphics Path</returns>
        protected virtual GraphicsPath GetLowerTabBorderPath()
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom), new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom - 3) }; 
                                      
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(m_panelBounds.Right, m_panelBounds.Bottom - 3), new PointF(m_panelBounds.Right, m_panelBounds.Bottom) };
                                     
            path.AddLines(aptLines);

            path.CloseFigure();
            return path;
        }

        protected virtual Brush GetBackgroundBrush(DrawItemState state)
        {
            SolidBrush brush = null;
            bool bIsSelected = (state & DrawItemState.Selected) == DrawItemState.Selected;
            if (bIsSelected)
            {
                brush = new SolidBrush(Color.White);
            }
            else
            {
                if (XPThemes.IsSilverThemeOn)
                    brush = new SolidBrush(VS2005Colors.PanelColor);
                else
                    brush = new SolidBrush(SystemColors.ControlLight);
            }

            return brush;
        }

        #endregion
    }

    // <summary>
    /// This renderer allows user to draw TabPages VS2012DockingStyle style.
    /// </summary>
    public class TabRendererDockingVS2012 : TabRenderer2D
    {
        #region Constants
        private const int DEF_CORNER_RADIUS = 2;
        private const int DEF_OVERLAP_HEIGHT = 2;
        private const string DEF_RENDERER_NAME = "VS2005DockingStyle";

        private static readonly Color DEF_BORDER_COLOR = Color.FromKnownColor(KnownColor.ControlDark);
        #endregion

        #region members
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyDockingWhidbey m_tabPropertyExtender;

        private RectangleF m_panelBounds = RectangleF.Empty;
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
        public static new StyleRendererPropertyDockingWhidbey TabPanelPropertyExtender
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
        static TabRendererDockingVS2012()
        {
            m_tabPropertyExtender = new StyleRendererPropertyDockingWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingVS2012), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyDockingWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingVS2012), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererDockingWhidbey class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererDockingVS2012(ITabControl parent, ITabPanelRenderer panelRenderer)
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

            // Transformed to horizontal alignment
            RectangleF currentBounds = TabUtils.ApplyTransform(g, TabAlignment, drawItemInfo.Bounds, true);
            m_panelBounds = TabUtils.ApplyTransform(g, TabAlignment, panelRenderer.Bounds, true);

            // This could instead be imp. as a no-transform version with ControlPaint
            this.ApplyTransform(g);

            float fLineX = 0.0F;
            bool bIsMirrored = panelRenderer.IsMirrored;

            using (Pen pen = new Pen(DEF_BORDER_COLOR))
            {
                if (((int)drawItemInfo.State & (int)DrawItemState.Selected) > 0)
                {
                    if (m_panelBounds.Y + m_panelBounds.Height - currentBounds.Y - currentBounds.Height > 0)
                    {
                        //using (GraphicsPath path = this.GetTabBorderPath(currentBounds))
                        //{
                        //    g.DrawPath(pen, path);
                        //}
                        //using (GraphicsPath path = this.GetLowerTabBorderPath())
                        //{
                        //    g.DrawPath(pen, path);
                        //}
                    }
                    else
                    {
                        Pen pen1 = new Pen(Color.FromArgb(209, 209, 209));
                        Pen pen2 = new Pen(Color.Red);
                        fLineX = bIsMirrored ? currentBounds.Left - 1 : currentBounds.Right - 1;
                        g.DrawLine(pen1, fLineX, currentBounds.Top, fLineX, currentBounds.Bottom);
                    }
                }
                else
                {
                    Pen pen1 = new Pen(Color.FromArgb(209, 209, 209));
                    fLineX = bIsMirrored ? currentBounds.Left - 1 : currentBounds.Right - 1;
                    g.DrawLine(pen1, fLineX - 1, currentBounds.Top, fLineX - 1, currentBounds.Bottom);
                }
            }

            g.ResetTransform();
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
            m_panelBounds = TabUtils.ApplyTransform(g, this.TabAlignment, panelRenderer.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);

            RectangleF bounds = curBounds;

            using (Brush brush = GetBackgroundBrush(drawItemInfo.State))
            {
                using (GraphicsPath path = this.GetTabBorderPath(bounds))
                {
                    drawItemInfo.Graphics.FillPath(brush, path);
                }
                using (GraphicsPath path = this.GetLowerTabBorderPath())
                {
                    drawItemInfo.Graphics.FillPath(brush, path);
                }
            }

            g.ResetTransform();
        }

        /// <summary>
        /// Gets the path for for the tab and the line beneath the tabs.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics Path</returns>
        protected virtual GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF(bounds.Right, bounds.Top), new PointF(bounds.Right, bounds.Top), new PointF(bounds.Right, bounds.Bottom) };
            path.AddLines(aptLines);

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Gets the path for the tab.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics path</returns>
        protected virtual GraphicsPath GetTabBorderPath(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF(bounds.Left, bounds.Bottom), new PointF(bounds.Left, bounds.Top + DEF_CORNER_RADIUS), new PointF(bounds.Left + DEF_CORNER_RADIUS, bounds.Top) };
            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(bounds.Right - DEF_CORNER_RADIUS, bounds.Top), new PointF(bounds.Right, bounds.Top + DEF_CORNER_RADIUS), new PointF(bounds.Right, bounds.Bottom) };

            path.AddLines(aptLines);

            return path;
        }

        /// <summary>
        /// Gets the path for the line beneath the tabs.
        /// </summary>
        /// <returns>Returns Graphics Path</returns>
        protected virtual GraphicsPath GetLowerTabBorderPath()
        {
            GraphicsPath path = new GraphicsPath();

            PointF[] aptLines = null;

            aptLines = new PointF[] { new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom), new PointF(m_panelBounds.Left - 3, m_panelBounds.Bottom - 3) };

            path.AddLines(aptLines);

            aptLines = new PointF[] { new PointF(m_panelBounds.Right, m_panelBounds.Bottom - 3), new PointF(m_panelBounds.Right, m_panelBounds.Bottom) };

            path.AddLines(aptLines);

            path.CloseFigure();
            return path;
        }

        protected virtual Brush GetBackgroundBrush(DrawItemState state)
        {
            SolidBrush brush = null;
            bool bIsSelected = (state & DrawItemState.Selected) == DrawItemState.Selected;
            if (bIsSelected)
            {
                brush = new SolidBrush(Color.FromArgb(246, 246, 246));
            }
            else
            {
                if (XPThemes.IsSilverThemeOn)
                    brush = new SolidBrush(VS2005Colors.PanelColor);
                else
                    brush = new SolidBrush(Color.FromArgb(239, 239, 242));
            }

            return brush;
        }

        #endregion
    }
}