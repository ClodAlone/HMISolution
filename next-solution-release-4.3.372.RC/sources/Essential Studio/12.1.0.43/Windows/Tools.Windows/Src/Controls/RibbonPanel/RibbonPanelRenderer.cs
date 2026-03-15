#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
    public class RibbonPanelRenderer
    {
        #region Constants
 
        private const int BORDER_WIDTH = 2;
    
         private const int SCROLLBARBUTTON_WIDTH = 12;
    
        private const int ARROW_HEIGHT = 5;
   
       private const int ARROW_WIDTH = 3;
        #endregion

        #region Enums
   
        protected enum EBITMAP
        {
            /// <summary>
            /// Represent Right arrow
            /// </summary>
            ebRightArrow = 0,

            /// <summary>
            /// Reprents left arrow
            /// </summary>
            ebLeftArrow
        }
        #endregion

        #region Properties
     
        protected Bitmap RightArrow
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebRightArrow] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        Rectangle rcRightArrow = new Rectangle(0, 0, 1, ARROW_HEIGHT);

                        using (Region rg = new Region(rcRightArrow))
                        {
                            rg.Union(rcRightArrow);

                            rcRightArrow.Inflate(0, -1);
                            rcRightArrow.X += 1;
                            rg.Union(rcRightArrow);

                            rcRightArrow.Inflate(0, -1);
                            rcRightArrow.X += 1;
                            rg.Union(rcRightArrow);

                            using (Brush brush = new SolidBrush((m_colorTable !=null)? m_colorTable.RibbonText : ribbonText))
                            {
                                g.FillRegion(brush, rg);
                            }
                        }
                    }

                    m_htBitmaps[EBITMAP.ebRightArrow] = bitmap;
                }

                return bitmap;
            }
        }
        
        protected Bitmap LeftArrow
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebLeftArrow] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_WIDTH, ARROW_HEIGHT);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        Rectangle rcLeftArrow = new Rectangle(0, 2, 1, 1);

                        using (Region rg = new Region(rcLeftArrow))
                        {
                            rg.Union(rcLeftArrow);

                            rcLeftArrow.Inflate(0, 1);
                            rcLeftArrow.X += 1;
                            rg.Union(rcLeftArrow);

                            rcLeftArrow.Inflate(0, 1);
                            rcLeftArrow.X += 1;
                            rg.Union(rcLeftArrow);

                            using (Brush brush = new SolidBrush((m_colorTable != null) ? m_colorTable.RibbonText : ribbonText))
                            {
                                g.FillRegion(brush, rg);
                            }
                        }
                    }

                    m_htBitmaps[EBITMAP.ebLeftArrow] = bitmap;
                }

                return bitmap;
            }
        }
        #endregion

        #region Constructors
        static RibbonPanelRenderer()
        {
            m_blBackGround = new Blend();
            m_blBackGround.Positions = new float[] { 0.0F, 0.3F, 1.0F };
            m_blBackGround.Factors = new float[] { 1.0F, 0.0F, 1.0F };

            m_blGroupedBackGround = new Blend();
            m_blGroupedBackGround.Positions = new float[] { 0.0f, 0.1f, 0.2f, 1.0f };
            m_blGroupedBackGround.Factors = new float[] { 0.6f, 0.8f, 0.9f, 0.95f };
        }
 
        public RibbonPanelRenderer(Office12ColorTable colorTable)
        {
            m_colorTable = colorTable;
        }

        /// <summary>
        /// Intended for compatibility with the <see cref="Office2010RibbonPanelRenderer"/>. Not to be used in the applications.
        /// </summary>
        /// <param name="colorTable">Instance of Office2010ColorTable</param>
        internal RibbonPanelRenderer(ProfessionalColorTable colorTable) : this(colorTable as Office12ColorTable)
        { }

        #endregion

        #region Methods
   
        public int GetBorderWidth(RibbonPanel panel)
        {
            return BORDER_WIDTH;
        }
  
        public virtual void DrawBackground(RibbonPanel panel, PaintEventArgs pe)
        {
            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(panel.Handle, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Graphics g = pe.Graphics;
                    Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

				 if (!SystemInformation.HighContrast)
                    {
                        using (LinearGradientBrush brush = (tsTabGroup != null) ?
                            GetGroupedBackgroundBrush(tsTabGroup.Color, Color.White, rc.Height) :
                            GetBackgroundBrush(rc.Height, false))
                        {
                            brush.TranslateTransform(0, -2, MatrixOrder.Append);
                            pe.Graphics.FillRectangle(brush, rc);
                        }
                    }

                    ToolStripTabItem tab = panel.TabItem;
                    if (tab != null)
                    {
                        ToolStrip ts = tab.GetCurrentParent();
                        if (ts != null)
                        {
                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                            g.ExcludeClip(new Rectangle(rcTab.X - rect.left, 0, rcTab.Width, 2));
                        }
                    }
                }
            }
        }

        public virtual void DrawFrame(RibbonPanel panel)
        {
            IntPtr wnd = panel.Handle;

            RECT rect = new RECT();

            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    try
                    {
                        Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                        if (hdc != IntPtr.Zero)
                        {
                            Rectangle rectangle = new Rectangle(BORDER_WIDTH, BORDER_WIDTH, rect.Width - 2 * BORDER_WIDTH, rect.Height - 2 * BORDER_WIDTH);

                            if (panel.IsLeftScroll)
                            {
                                rectangle.X += SCROLLBARBUTTON_WIDTH;
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            if (panel.IsRightScroll)
                            {
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            WindowsAPI.ExcludeClipRect(hdc, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

                            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rc))
                            {
                                Graphics g = bg.Graphics;

                                ToolStripTabItem tab = panel.TabItem;
                                ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

                                if (!IsPanelAboveTheTabItem(panel))
                                {
                                    if (tab != null)
                                    {
                                        ToolStrip ts = tab.GetCurrentParent();

                                        if (ts != null)
                                        {
                                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                                            Rectangle rcConnection = new Rectangle(rcTab.X - rect.left, 0, rcTab.Width, 2);

                                            using (LinearGradientBrush brush = (tsTabGroup != null) ?
                                                GetGroupedBackgroundBrush(
                                                Office12ColorTable.GetAlphaBlendedColor(tsTabGroup.Color, Color.White, 112),
                                                Office12ColorTable.GetAlphaBlendedColor(tsTabGroup.Color, Color.White, 96),
                                                rc.Height) :
                                                GetBackgroundBrush(rc.Height, false))
                                            {
                                                g.FillRectangle(brush, rcConnection);
                                            }

                                            g.ExcludeClip(rcConnection);
                                        }
                                    }
                                }

                                DrawBorders(g, panel, tsTabGroup, rc);

                                if (panel.IsRightScroll)
                                {
                                    DrawRightScroll(g, panel, rc);
                                }

                                if (panel.IsLeftScroll)
                                {
                                    DrawLeftScroll(g, panel, rc);
                                }
                                bg.Render();
                            }
                        }
                    }
                    finally
                    {
                        WindowsAPI.ReleaseDC(wnd, hdc);
                    }
                }
            }
        }

        public virtual void DrawBorders(Graphics g, RibbonPanel panel, ToolStripTabGroup tsTabGroup, Rectangle rect)
        {
            bool bIsTabGroup = tsTabGroup != null;

            using (Pen penHighLight = GetBorderPenHighLight())
            {
                g.DrawPolygon(penHighLight, RendererUtils.GetRoundedPolygon(Rectangle.Inflate(rect, -1, -1), 1));
            }

            using (Pen pen = bIsTabGroup ? GetGroupedBorderPen() : GetBorderPen())
            {
                g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rect, 2));
            }

            if (bIsTabGroup)
            {
                Color clMedium = Office12ColorTable.GetAlphaBlendedColor(tsTabGroup.Color, Color.White, 96);

                using (Pen penLine = new Pen(clMedium))
                {
                    g.DrawLine(penLine, rect.X + 3 + (panel.IsLeftScroll ? SCROLLBARBUTTON_WIDTH : 0), rect.Y + 1, rect.Right - 4 - (panel.IsRightScroll ? SCROLLBARBUTTON_WIDTH : 0), rect.Y + 1);
                }
            }
        }

        public virtual void DrawRightScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            Rectangle rc = new Rectangle(rect.Width - SCROLLBARBUTTON_WIDTH - BORDER_WIDTH, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

            using (Brush brush = GetBackgroundBrush(rc.Height, panel.RightScrollSelected))
            {
                g.FillRectangle(brush, rc);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawPolygon(Pens.White, RendererUtils.GetSquareRoundedToRightPolygon(Rectangle.Inflate(rc, -1, -1), 1));

            using (Pen pen = GetBorderPen())
            {
                g.DrawPolygon(pen, RendererUtils.GetSquareRoundedToRightPolygon(rc, 2));
            }

            // Drawing arrow.
            Image imgRightArrow = RightArrow;

            int left = rect.Width - rc.Width + (rc.Width - ARROW_WIDTH) / 2;
            int top = (rc.Height - ARROW_HEIGHT) / 2;

            g.DrawImage(imgRightArrow, left, top);
        }

        public virtual void DrawLeftScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            Rectangle rc = new Rectangle(0, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

            using (Brush brush = GetBackgroundBrush(rc.Height, panel.LeftScrollSelected))
            {
                g.FillRectangle(brush, rc);
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;

            g.DrawPolygon(Pens.White, RendererUtils.GetSquareRoundedToLeftPolygon(Rectangle.Inflate(rc, -1, -1), 1));

            using (Pen pen = GetBorderPen())
            {
                g.DrawPolygon(pen, RendererUtils.GetSquareRoundedToLeftPolygon(rc, 2));
            }

            // Drawing arrow.
            Image imgLeftArrow = LeftArrow;

            int left = (rc.Width - ARROW_WIDTH) / 2;
            int top = (rc.Height - ARROW_HEIGHT) / 2;

            g.DrawImage(imgLeftArrow, left, top);
        }
        #endregion

        #region Implementation

        private LinearGradientBrush GetBackgroundBrush(int nHeight, bool bSelected)
        {
            Color clBegin = bSelected ? m_colorTable.ButtonSelectedGradientBegin : m_colorTable.ToolStripGradientBegin;
            Color clEnd = bSelected ? m_colorTable.ButtonSelectedGradientEnd : m_colorTable.ToolStripGradientEnd;

            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);
            brush.Blend = m_blBackGround;

            return brush;
        }
    
        private LinearGradientBrush GetGroupedBackgroundBrush(Color clBegin, Color clEnd, int nHeight)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);
            brush.Blend = m_blGroupedBackGround;

            return brush;
        }
    
        private Pen GetGroupedBorderPen()
        {
            return new Pen(m_colorTable.RibbonPanelGroupedBorderEnd);
        }

        private Pen GetBorderPenHighLight()
        {
            return new Pen(m_colorTable.RibbonPanelBorderBegin);
        }
  
        private Pen GetBorderPen()
        {
            return new Pen(m_colorTable.RibbonPanelBorderEnd);
        }

        /// <summary>
        /// Indicates whether RibbonPanel is located above the corresponding TabItem.
        /// </summary>
        /// <param name="panel"> RibbonPanel control. </param>
        /// <returns>Returns bool value</returns>
        protected static bool IsPanelAboveTheTabItem(RibbonPanel panel)
        {
            bool bResult = false;

            RibbonControlAdv.RibbonControlPopup ctrlPopup = panel.Parent as RibbonControlAdv.RibbonControlPopup;

            if (ctrlPopup != null)
            {
                RibbonControlAdv ribbon = ctrlPopup.Owner;
                int iTabItemY = ribbon.PointToScreen(panel.TabItem.Bounds.Location).Y;

                NativeMethods.RECT rcRibbonPopup = new NativeMethods.RECT();
                NativeMethods.GetWindowRect((int)ctrlPopup.Handle, ref rcRibbonPopup);

                bResult = iTabItemY > rcRibbonPopup.top;
            }

            return bResult;
        }
        #endregion

        #region Fields
  
        private Office12ColorTable m_colorTable;

        private Color ribbonText = Color.Black;
     
       private static Blend m_blBackGround;
  
        private static Blend m_blGroupedBackGround;
    
        private Hashtable m_htBitmaps = new Hashtable();
        #endregion
    }

    public class Office2010RibbonPanelRenderer : RibbonPanelRenderer
    {
        #region Fields
        
        private const int BORDER_WIDTH = 2;

        private const int SCROLLBARBUTTON_WIDTH = 12;

        private const int ARROW_HEIGHT = 5;

        private const int ARROW_WIDTH = 3;

        Office2010ColorTable colorTable = null;

        #endregion

        #region Ctor
        public Office2010RibbonPanelRenderer()
            : this(new Office2010ColorTable( Office2010ColorScheme.Blue))
        {
        }

        public Office2010RibbonPanelRenderer(Office2010ColorTable colorTable)
            : base(colorTable)
        {
           this.colorTable = colorTable;
        }
        #endregion

        #region overrides
        
        public override void DrawBackground(RibbonPanel panel, PaintEventArgs pe)
        {
            RECT rect = new RECT();

            if (WindowsAPI.GetWindowRect(panel.Handle, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Graphics g = pe.Graphics;

                    Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();
                    if (!SystemInformation.HighContrast)
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(rc, colorTable.RibbonPanelBackgroundGradientBegin, colorTable.RibbonPanelBackgroundGradientEnd, LinearGradientMode.Vertical))
                        {
                            g.FillRectangle(brush, rc);
                        }
                    }

                    ToolStripTabItem tab = panel.TabItem;
                    if (tab != null)
                    {
                        ToolStrip ts = tab.GetCurrentParent();
                        if (ts != null)
                        {
                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                            g.ExcludeClip(new Rectangle(rcTab.X - rect.left, 0, rcTab.Width, 2));
                        }
                    }

                    DrawBorders(g, panel, tsTabGroup, rc);

                }
            }
        }

        public override void DrawBorders(Graphics g, RibbonPanel panel, ToolStripTabGroup tsTabGroup, Rectangle rect)
        {
            Color color = (tsTabGroup!=null) ? Color.FromArgb(100, tsTabGroup.Color) : colorTable.FormBorderActive;

            if (panel.OfficeColorScheme == ToolStripEx.ColorScheme.Silver)
                color = ColorTranslator.FromHtml("#E5E7E9");
            if (panel.OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                color = ColorTranslator.FromHtml("#717171");
            if (panel.OfficeColorScheme == ToolStripEx.ColorScheme.Blue)
                color = ColorTranslator.FromHtml("#BBCEE6");
            using (Pen pen= new Pen(color))
            {
                g.DrawRectangle(pen, rect);
            }
            if (panel.Parent is RibbonControlAdv)
            {
                Pen groupPen = new Pen((panel.Parent as RibbonControlAdv).ActiveTabGroupColor);
                g.DrawLine(groupPen, new Point(rect.X, rect.Y), new Point(rect.Width, rect.Y));
            }
        }

        public override void DrawFrame(RibbonPanel panel)
        {
            IntPtr wnd = panel.Handle;

            RECT rect = new RECT();

            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    try
                    {
                        Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                        if (hdc != IntPtr.Zero)
                        {
                            Rectangle rectangle = new Rectangle(1, 1, rect.Width - 2 * 1, rect.Height - 2 * 1);

                            if (panel.IsLeftScroll)
                            {
                                rectangle.X += SCROLLBARBUTTON_WIDTH;
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            if (panel.IsRightScroll)
                            {
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            WindowsAPI.ExcludeClipRect(hdc, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

                            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rc))
                            {
                                Graphics g = bg.Graphics;

                                ToolStripTabItem tab = panel.TabItem;
                                ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

                                if (!IsPanelAboveTheTabItem(panel))
                                {
                                    if (tab != null)
                                    {
                                        ToolStrip ts = tab.GetCurrentParent();

                                        if (ts != null)
                                        {
                                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                                            Rectangle rcConnection = new Rectangle(rcTab.X - rect.left, 0, rcTab.Width, 2);

                                            using (Brush brush = new LinearGradientBrush(rc,colorTable.RibbonPanelBackgroundGradientBegin,colorTable.RibbonPanelBackgroundGradientEnd,LinearGradientMode.Vertical))
                                            {
                                                g.FillRectangle(brush, rcConnection);
                                            }

                                            g.ExcludeClip(rcConnection);
                                        }
                                    }
                                }

                                DrawBorders(g, panel, tsTabGroup, rect);

                                if (panel.IsRightScroll)
                                {
                                    DrawRightScroll(g, panel, rc);
                                }

                                if (panel.IsLeftScroll)
                                {
                                    DrawLeftScroll(g, panel, rc);
                                }


                            }
                        }
                    }
                    finally
                    {
                        WindowsAPI.ReleaseDC(wnd, hdc);
                    }
                }
            }
        }

        private Image GetToolstripItemImageSelected(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            rect.Width -= 1; rect.Height -= 1;
            Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            using (SolidBrush brush = new SolidBrush(colorTable.ToolstripButtonSelectedBackground))
            {
                g.FillPolygon(brush, polygon);
            }

            using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBorder))
            {
                g.DrawPolygon(pen, polygon);
            }

            rect.Inflate(-1, -1);
            polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBottomCenterColor))
            {
                g.DrawPolygon(pen, polygon);
            }

            g.Clip = new Region(rect);

            using (GraphicsPath ellipse = new GraphicsPath())
            {
                float offset = rect.Width / 5;
                ellipse.AddEllipse(new RectangleF(-offset / 2, 0, rect.Width + offset, 30));
                ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Width - 15)));

                using (PathGradientBrush p = new PathGradientBrush(ellipse))
                {
                    p.CenterColor = colorTable.ToolstripButtonSelectedBottomCenterColor;
                    p.SurroundColors = new Color[] { Color.FromArgb(12, colorTable.ToolstripButtonSelectedBottomSurroundColors) };
                    p.FocusScales = new PointF(0.6F, 0.1F);
                    g.FillPath(p, ellipse);
                }
            }
            return bmp;
        }

        public override void DrawLeftScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Rectangle rc = new Rectangle(0, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

                if (panel.LeftScrollSelected)
                {
                    Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty, rc.Size), 2, 2);
                    g.DrawImage(GetToolstripItemImageSelected(rcTemp), rc.Location);
                }
                else
                {
                    using (Brush brush = new LinearGradientBrush(rc, colorTable.TabScrollButtonGradientBegin, colorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(colorTable.TabScrollButtonBorder))
                {
                    g.DrawRectangle(pen, rc);
                }

                Image imgArrow = base.LeftArrow;

                int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
                int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

                g.DrawImage(imgArrow, left, top);
            }
        }

        public override void DrawRightScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Rectangle rc = new Rectangle(rect.Width - SCROLLBARBUTTON_WIDTH - BORDER_WIDTH, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

                if (panel.RightScrollSelected)
                {
                    Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty, rc.Size), 2, 2);
                    g.DrawImage(GetToolstripItemImageSelected(rcTemp), rc.Location);
                }
                else
                {
                    using (Brush brush = new LinearGradientBrush(rc, colorTable.TabScrollButtonGradientBegin, colorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(colorTable.TabScrollButtonBorder))
                {
                    g.DrawRectangle(pen, rc);
                }

                Image imgArrow = base.RightArrow;

                int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
                int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

                g.DrawImage(imgArrow, left, top);
            }
        }
        
        #endregion
    }
    public class Office2013RibbonPanelRenderer : RibbonPanelRenderer
    {
        #region Fields

        private const int BORDER_WIDTH = 2;

        private const int SCROLLBARBUTTON_WIDTH = 12;

        private const int ARROW_HEIGHT = 5;

        private const int ARROW_WIDTH = 3;

        Office2010ColorTable colorTable = null;

        private Image UpImage = null;
        private Image PinImage = null;
        #endregion
        private Color upButtonBackColor = Color.Transparent;
        internal Color UpButtonBackColor
        {
            get
            {
                return upButtonBackColor;
            }
            set
            {
                upButtonBackColor = value;
            }
        }
        #region Ctor
        public Office2013RibbonPanelRenderer()
            : this(new Office2010ColorTable(Office2010ColorScheme.Blue))
        {
        }

        public Office2013RibbonPanelRenderer(Office2010ColorTable colorTable)
            : base(colorTable)
        {
            this.colorTable = colorTable;
            PinImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.2013pin.png"));
            UpImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.2013Up.png"));
        }
        #endregion

        #region overrides

        public override void DrawBackground(RibbonPanel panel, PaintEventArgs pe)
        {
            RECT rect = new RECT();
            RibbonControlAdv ribbon = panel.Parent as RibbonControlAdv;
            if (WindowsAPI.GetWindowRect(panel.Handle, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    Graphics g = pe.Graphics;
                    Brush br = new SolidBrush(Color.White);
                    Color clr = Color.Empty;
                    Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();
                    if (!SystemInformation.HighContrast)
                    {

                        if (panel.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                        {
                            clr = ColorTranslator.FromHtml("#F0F0F0");
                            using (br = new SolidBrush(clr))
                            {
                                g.FillRectangle(br, rc);
                            }
                        }
                        else if (panel.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                        {
                            clr = ColorTranslator.FromHtml("#F8F8F8");
                            using (br = new SolidBrush(clr))
                            {
                                g.FillRectangle(br, rc);
                            }
                        }
                        else if (panel.Office2013ColorScheme == Office2013ColorScheme.White)
                        {
                            g.FillRectangle(br, rc);
                        }
                    }

                    ToolStripTabItem tab = panel.TabItem;
                    if (tab != null)
                    {
                        ToolStrip ts = tab.GetCurrentParent();
                        if (ts != null)
                        {
                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                            g.ExcludeClip(new Rectangle(rcTab.X - rect.left+1, 0, rcTab.Width-2,2));
                        }
                    }
                    {
                        int leftvalue = 26;
                        if (panel.Parent is RibbonControlAdv)
                        {
                            if ((panel.Parent as RibbonControlAdv).RightToLeft == RightToLeft.Yes)
                            {
                                pe.Graphics.FillRectangle(new SolidBrush(this.UpButtonBackColor), new Rectangle(0, rc.Y + rc.Height - 18, 30, 18));
                                pe.Graphics.DrawImage(UpImage, new Rectangle(3, rc.Y + rc.Height - 22, 25, 25));
                            }
                            else
                            {
                                pe.Graphics.FillRectangle(new SolidBrush(this.UpButtonBackColor), new Rectangle(rc.X + rc.Width - leftvalue-5, rc.Y + rc.Height - 18, 50, 18));
                                pe.Graphics.DrawImage(UpImage, new Rectangle(rc.X + rc.Width - leftvalue, rc.Y + rc.Height - 22, 25, 25));
                            }
                        }
                        else
                        {
                            if ((panel.Parent as RibbonControlAdv.RibbonControlPopup).RightToLeft == RightToLeft.Yes)
                            {
                                pe.Graphics.FillRectangle(new SolidBrush(this.UpButtonBackColor), new Rectangle(0, rc.Y + rc.Height - 18, 30, 18));
                                pe.Graphics.DrawImage(PinImage, new Rectangle(3,  rc.Y + rc.Height - 22, 25, 25));
                            }
                            else
                            {
                                pe.Graphics.FillRectangle(new SolidBrush(this.UpButtonBackColor), new Rectangle(rc.X + rc.Width - leftvalue - 5, rc.Y + rc.Height - 18, 50, 18));
                                pe.Graphics.DrawImage(PinImage, new Rectangle(rc.X + rc.Width - leftvalue-2, rc.Y + rc.Height - 22, 25, 25));
                            }
                        }
                    }
                    DrawBorders(g, panel, tsTabGroup, rc);
                    br.Dispose();
                }
            }
        }

        public override void DrawBorders(Graphics g, RibbonPanel panel, ToolStripTabGroup tsTabGroup, Rectangle rect)
        {
            Color color = (tsTabGroup != null) ? Color.FromArgb(100, tsTabGroup.Color) : colorTable.FormBorderActive;

            if (panel.OfficeColorScheme == ToolStripEx.ColorScheme.Silver)
                color = ColorTranslator.FromHtml("#B6BABF");
            using (Pen pen = new Pen(color))
            {
                g.DrawRectangle(pen, rect);
            }
            if (panel.Parent is RibbonControlAdv)
            {
                Pen groupPen = new Pen((panel.Parent as RibbonControlAdv).ActiveTabGroupColor);
                g.DrawLine(groupPen, new Point(rect.X, rect.Y), new Point(rect.Width, rect.Y));
            }
        }

        public override void DrawFrame(RibbonPanel panel)
        {
            IntPtr wnd = panel.Handle;
            RibbonControlAdv ribbon = panel.Parent as RibbonControlAdv;
            RECT rect = new RECT();

            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                if (rect.Width > 0 && rect.Height > 0)
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    try
                    {
                        Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                        if (hdc != IntPtr.Zero)
                        {
                            Rectangle rectangle = new Rectangle(1, 1, rect.Width - 2 * 1, rect.Height - 2 * 1);

                            if (panel.IsLeftScroll)
                            {
                                rectangle.X += SCROLLBARBUTTON_WIDTH;
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            if (panel.IsRightScroll)
                            {
                                rectangle.Width -= SCROLLBARBUTTON_WIDTH;
                            }

                            WindowsAPI.ExcludeClipRect(hdc, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);

                            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rc))
                            {
                                Graphics g = bg.Graphics;

                                ToolStripTabItem tab = panel.TabItem;
                                ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

                                if (!IsPanelAboveTheTabItem(panel))
                                {
                                    if (tab != null)
                                    {
                                        ToolStrip ts = tab.GetCurrentParent();

                                        if (ts != null)
                                        {
                                            Rectangle rcTab = ts.RectangleToScreen(tab.Bounds);
                                            Rectangle rcConnection = new Rectangle(rcTab.X - rect.left, 0, rcTab.Width, 2);
                                            Brush br = new SolidBrush(Color.White);
                                            Color clr = Color.Empty;
                                            if (panel.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                                            {
                                                clr= ColorTranslator.FromHtml("#F0F0F0");
                                                using (br= new SolidBrush(clr))
                                                {
                                                    g.FillRectangle(br, rcConnection);
                                                }
                                            }
                                            else if (panel.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                                            {
                                                clr= ColorTranslator.FromHtml("#F8F8F8");
                                                using (br = new SolidBrush(clr))
                                                {
                                                    g.FillRectangle(br, rcConnection);
                                                }
                                            }
                                            else if (panel.Office2013ColorScheme == Office2013ColorScheme.White)
                                            {
                                                g.FillRectangle(br, rcConnection);
                                            }
                                            g.ExcludeClip(rcConnection);
                                            br.Dispose();
                                        }
                                    }
                                }

                                DrawBorders(g, panel, tsTabGroup, rect);

                                if (panel.IsRightScroll)
                                {
                                    DrawRightScroll(g, panel, rc);
                                }

                                if (panel.IsLeftScroll)
                                {
                                    DrawLeftScroll(g, panel, rc);
                                }


                            }
                        }
                    }
                    finally
                    {
                        WindowsAPI.ReleaseDC(wnd, hdc);
                    }
                }
            }
        }

        private Image GetToolstripItemImageSelected(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            rect.Width -= 1; rect.Height -= 1;
            Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            using (SolidBrush brush = new SolidBrush(colorTable.ToolstripButtonSelectedBackground))
            {
                g.FillPolygon(brush, polygon);
            }

            using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBorder))
            {
                g.DrawPolygon(pen, polygon);
            }

            rect.Inflate(-1, -1);
            polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBottomCenterColor))
            {
                g.DrawPolygon(pen, polygon);
            }

            g.Clip = new Region(rect);

            using (GraphicsPath ellipse = new GraphicsPath())
            {
                float offset = rect.Width / 5;
                ellipse.AddEllipse(new RectangleF(-offset / 2, 0, rect.Width + offset, 30));
                ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Width - 15)));

                using (PathGradientBrush p = new PathGradientBrush(ellipse))
                {
                    p.CenterColor = colorTable.ToolstripButtonSelectedBottomCenterColor;
                    p.SurroundColors = new Color[] { Color.FromArgb(12, colorTable.ToolstripButtonSelectedBottomSurroundColors) };
                    p.FocusScales = new PointF(0.6F, 0.1F);
                    g.FillPath(p, ellipse);
                }
            }
            return bmp;
        }

        public override void DrawLeftScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Rectangle rc = new Rectangle(0, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

                if (panel.LeftScrollSelected)
                {
                    Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty, rc.Size), 2, 2);
                    g.DrawImage(GetToolstripItemImageSelected(rcTemp), rc.Location);
                }
                else
                {
                    using (Brush brush = new LinearGradientBrush(rc, colorTable.TabScrollButtonGradientBegin, colorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(colorTable.TabScrollButtonBorder))
                {
                    g.DrawRectangle(pen, rc);
                }

                Image imgArrow = base.LeftArrow;

                int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
                int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

                g.DrawImage(imgArrow, left, top);
            }
        }

        public override void DrawRightScroll(Graphics g, RibbonPanel panel, Rectangle rect)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Rectangle rc = new Rectangle(rect.Width - SCROLLBARBUTTON_WIDTH - BORDER_WIDTH, 0, SCROLLBARBUTTON_WIDTH + BORDER_WIDTH, rect.Height);

                if (panel.RightScrollSelected)
                {
                    Rectangle rcTemp = Rectangle.Inflate(new Rectangle(Point.Empty, rc.Size), 2, 2);
                    g.DrawImage(GetToolstripItemImageSelected(rcTemp), rc.Location);
                }
                else
                {
                    using (Brush brush = new LinearGradientBrush(rc, colorTable.TabScrollButtonGradientBegin, colorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }

                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(colorTable.TabScrollButtonBorder))
                {
                    g.DrawRectangle(pen, rc);
                }

                Image imgArrow = base.RightArrow;

                int left = rc.Left + (rc.Width - ARROW_WIDTH) / 2;
                int top = rc.Top + (rc.Height - ARROW_HEIGHT) / 2;

                g.DrawImage(imgArrow, left, top);
            }
        }

        #endregion
    }
}
#endif
