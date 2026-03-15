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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Win32API;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools
{
    #region Constants
    public enum UtilMsg
    {
        WM_HOVERITEM = (int)Win32API.Msg.WM_USER + 0x1000,
        WM_LEAVEITEM,
    }
    public enum ToolStripBorderStyle
    {
        [Browsable(false)]
        Default,
        None = 0,
        StaticEdge,
        Etched,
    }
    public enum CaptionStyle
    {
        [Browsable(false)]
        Default,
        Top,
        Bottom
    }
    public enum CaptionTextStyle
    {
        [Browsable(false)]
        Default,
        Plain,
        Shadow,
        Etched,
    }
    public enum CaptionAlignment
    {
        [Browsable(false)]
        Default,
        Near,
        Center,
        Far
    }
    public enum LauncherStyle
    {
        [Browsable(false)]
        Default,
        Office12,
        Office2007,
        Metro
    }
    #endregion

    #region IToolStripExSupport
    /// <summary>
    /// 
    /// </summary>
    public interface IToolStripExSupport
    {
        /// <summary>
        /// 
        /// </summary>
        bool ShowCaption { get; }
        /// <summary>
        /// 
        /// </summary>
        bool ShowLauncher { get; }
        /// <summary>
        /// 
        /// </summary>
        bool GroupedButtons { get; }
        /// <summary>
        /// 
        /// </summary>
        bool LauncherSelected { get; }
        /// <summary>
        /// 
        /// </summary>
        int CaptionHeight { get; }
    }
    #endregion

    #region IToolStripExSupport2
    /// <summary>
    /// 
    /// </summary>
    public interface IToolStripExSupport2 : IToolStripExSupport
    {
        ToolStripBorderStyle BorderStyle { get; }
        LauncherStyle LauncherStyle { get; }
        CaptionStyle CaptionStyle { get; }
        CaptionTextStyle CaptionTextStyle { get; }
        CaptionAlignment CaptionAlignment { get; }
        Font CaptionFont { get; }
        int CaptionMinHeight { get; }
    }
    #endregion

    #region IToolStripTabItem
    /// <summary>
    /// 
    /// </summary>
    public interface IToolStripTabItem
    {
    }
    #endregion

    #region PanelItemRenderEventArgs
    public class PanelItemRenderEventArgs : ToolStripRenderEventArgs
    {
        #region Constructors
        public PanelItemRenderEventArgs(Graphics g, ToolStrip tsParentStrip, ToolStrip tsPanelStrip)
            : base(g, tsParentStrip)
        {
            m_tsPanelStrip = tsPanelStrip;
        }
        #endregion

        #region Properties
        public ToolStrip PanelStrip
        {
            get
            {
                return m_tsPanelStrip;
            }
        }
        #endregion

        #region Fields
        ToolStrip m_tsPanelStrip;
        #endregion
    }
    #endregion

    #region Office12ToolStripRenderer
    /// <summary>
    /// 
    /// </summary>
    public partial class Office12ToolStripRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
    {
        #region Constants
        const int STATICEDGE_WIDTH = 1;
        const int ETCHED_WIDTH = 2;
        #endregion

        #region Constructors/destructors
        /// <summary>
        /// 
        /// </summary>
        static Office12ToolStripRenderer()
        {
            m_blMenuBar = new Blend();
            m_blMenuBar.Positions = new float[] { 0.0F, 0.5F, 1.0F };
            m_blMenuBar.Factors = new float[] { 0.0F, 1.0F, 0.0F };

            m_blStatusBar = new Blend();
            m_blStatusBar.Positions = new float[] { 0.0F, 0.5F, 0.55F, 1.0F };
            m_blStatusBar.Factors = new float[] { 0.0F, 0.2F, 0.6F, 1.0F };

            m_blToolBar = new Blend();
            m_blToolBar.Positions = new float[] { 0.0F, 0.3F, 1.0F };
            m_blToolBar.Factors = new float[] { 1.0F, 0.0F, 1.0F };

            m_blMenuItemUp = new Blend();
            m_blMenuItemUp.Positions = new float[] { 0.0F, 0.4F, 0.5F, 1.0F };
            m_blMenuItemUp.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blMenuItemDown = new Blend();
            m_blMenuItemDown.Positions = new float[] { 0.0F, 0.3F, 0.32F, 1.0F };
            m_blMenuItemDown.Factors = new float[] { 1.0F, 0.5F, 0.4F, 0.0F };

            m_blGrouped = new Blend();
            m_blGrouped.Positions = new float[] { 0.0F, 0.4F, 0.4F, 1.0F };
            m_blGrouped.Factors = new float[] { 0.8F, 1.0F, 0.0F, 0.4F };

            m_blScrollButton = new Blend();
            m_blScrollButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blScrollButton.Factors = new float[] { 0.2F, 0.15F, 1.0F, 0.5F };

            m_blButtonSelected = new Blend();
            m_blButtonSelected.Positions = new float[] { 0.0F, 0.4F, 0.4F, 1.0F };
            m_blButtonSelected.Factors = new float[] { 0.2F, 0.0F, 0.8F, 0.2F };

            m_blButtonCollapsed = new Blend();
            m_blButtonCollapsed.Positions = new float[] { 0.0F, 0.15F, 0.15F, 1.0F };
            m_blButtonCollapsed.Factors = new float[] { 1.0F, 0.8F, 0.0F, 1.0F };

            m_blButtonShadow = new Blend();
            m_blButtonShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
            m_blButtonShadow.Factors = new float[] { 0.0f, 0.3f, 0.6f, 0.8f, 0.95f, 1.0f };

            m_blImageBackground = new Blend();
            m_blImageBackground.Positions = new float[] { 0.0F, 0.8F, 0.8F, 1.0F };
            m_blImageBackground.Factors = new float[] { 0.0F, 0.2F, 1.0F, 1.0F };

            m_blButtonFlash = new Blend();
            m_blButtonFlash.Positions = new float[] { 0f, 0.4f, 1f };
            m_blButtonFlash.Factors = new float[] { 0f, 0.6f, 1f };

            m_blCaption = new Blend();
            m_blCaption.Positions = new float[] { 0.0F, 0.2F, 1.0F };
            m_blCaption.Factors = new float[] { 0.0F, 0.5F, 1.0F };

            m_blScrollerBackground = new Blend();
            m_blScrollerBackground.Positions = new float[] { 0.0F, 0.35F, 1.0F };
            m_blScrollerBackground.Factors = new float[] { 0.3F, 0.6F, 0.4F };

            m_blStandardScrollButton = new Blend();
            m_blStandardScrollButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blStandardScrollButton.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blScroller = new Blend();
            m_blScroller.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blScroller.Factors = new float[] { 0.3F, 0.0F, 1.0F, 0.4F };

            m_blGroupedBackGroundCollapsed = new Blend();
            m_blGroupedBackGroundCollapsed.Positions = new float[] { 0.0f, 0.5f, 0.5f, 0.92f, 0.92f, 1.0f };
            m_blGroupedBackGroundCollapsed.Factors = new float[] { 0.0f, 0.0f, 0.1f, 0.5f, 0.92f, 1.0f };

            m_blGroupedBackGround = new Blend();
            m_blGroupedBackGround.Positions = new float[] { 0.0f, 0.1f, 0.2f, 1.0f };
            m_blGroupedBackGround.Factors = new float[] { 0.6f, 0.8f, 0.9f, 0.95f };

            m_gCaption = Graphics.FromImage(new Bitmap(1, 1));
            m_gCaption.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            m_htControls = new Hashtable();
            m_controlsLock = new Object();
        }
        /// <summary>
        /// 
        /// </summary>
        public Office12ToolStripRenderer()
            : this(new Office12ColorTable(), ERENDERTYPE.Normal)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="erType"></param>
        public Office12ToolStripRenderer(ERENDERTYPE erType)
            : this(new Office12ColorTable(), erType)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorTable"></param>
        public Office12ToolStripRenderer(Office12ColorTable colorTable)
            : this(colorTable, ERENDERTYPE.Normal)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="erType"></param>
        /// <param name="colorTable"></param>
        public Office12ToolStripRenderer(Office12ColorTable colorTable, ERENDERTYPE erType)
            : base(colorTable)
        {
            RoundedEdges = false;

            m_eRenderType = erType;

            m_htPens = new Hashtable((int)EPEN.MAX);
            m_htBrushes = new Hashtable((int)EBRUSH.MAX);
            m_htBitmaps = new Hashtable((int)EBITMAP.MAX);

            m_ComboBoxRenderer = new ComboBoxRenderer(this);
        }
        #endregion

        #region Caption Functions

        protected virtual bool HasCaption(ToolStrip toolStrip)
        {
            return  ToolStripRendererUtils.HasCaption(toolStrip, this.RenderType);
        }

        protected virtual void DrawCaption(ToolStrip toolStrip)
        {
            if (HasCaption(toolStrip))
            {
                Rectangle rcUpdate = ToolStripRendererUtils.GetCaptionBounds(toolStrip, this.RenderType);
                if (rcUpdate.Width > 0 && rcUpdate.Height > 0)
                {
                    IntPtr hWnd = toolStrip.Handle;

                    RECT rcWindow = new RECT();
                    if (WindowsAPI.GetWindowRect(hWnd, ref rcWindow))
                    {
                        rcUpdate.X -= rcWindow.left;
                        rcUpdate.Y -= rcWindow.top;

                        IntPtr hdc = WindowsAPI.GetWindowDC(hWnd);

                        if (hdc != IntPtr.Zero)
                        {
                            if (toolStrip.Dock == DockStyle.Left || toolStrip.Dock == DockStyle.Right)
                            {
                                // Vertical orientation
                                Matrix m = new Matrix();
                                m.Rotate(-90f, MatrixOrder.Append);
                                m.Translate(rcUpdate.X, rcUpdate.Bottom, MatrixOrder.Append);

                                XFORM xform = new XFORM();
                                xform.eM11 = m.Elements[0];
                                xform.eM12 = m.Elements[1];
                                xform.eM21 = m.Elements[2];
                                xform.eM22 = m.Elements[3];
                                xform.eDx = m.Elements[4];
                                xform.eDy = m.Elements[5];

                                SetGraphicsMode(hdc, 2/*GM_ADVANCED*/);
                                SetWorldTransform(hdc, ref xform);

                                rcUpdate = new Rectangle(0, 0, rcUpdate.Height, rcUpdate.Width);
                            }

                            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcUpdate))
                            {
                                Graphics g = bg.Graphics;
                                bool isFormSelected = ((toolStrip.TopLevelControl is Form) && toolStrip.TopLevelControl.ContainsFocus);
                                bool bSelected = (isFormSelected && GetIsSelected(toolStrip));
                                Color clTabGroup = Color.Empty;

                                bool bIsToolStripGrouped = IsGroupedToolStrip(toolStrip, ref clTabGroup);

                                Color clrBegin = bSelected ?
                                    bIsToolStripGrouped ? this.OfficeColorTable.CaptionGroupedHighlightGradientBegin : this.OfficeColorTable.CaptionHighlightGradientBegin :
                                    bIsToolStripGrouped ? this.OfficeColorTable.CaptionGroupedGradientBegin : this.OfficeColorTable.CaptionGradientBegin;

                                Color clrEnd = bSelected ?
                                    bIsToolStripGrouped ? this.OfficeColorTable.CaptionGroupedHighlightGradientEnd : this.OfficeColorTable.CaptionHighlightGradientEnd :
                                    bIsToolStripGrouped ? this.OfficeColorTable.CaptionGroupedGradientEnd : this.OfficeColorTable.CaptionGradientEnd;
                                if (SystemInformation.HighContrast)
                                {
                                    clrBegin = Color.Black;
                                    clrEnd = Color.Black;
                                }
                                using (LinearGradientBrush brush = GetVerticalBrush(ref rcUpdate, clrBegin, clrEnd))
                                {
                                    brush.Blend = m_blCaption;
                                    brush.WrapMode = WrapMode.TileFlipXY;
                                    g.FillRectangle(brush, rcUpdate);
                                }

                                Font f = ToolStripRendererUtils.GetCaptionFont(toolStrip);

                                Rectangle rcText = new Rectangle(0, 0, rcUpdate.Width, rcUpdate.Height);

                                if (ToolStripRendererUtils.HasLauncher(toolStrip))
                                {
                                    rcText.Width -= LAUNCHER_WIDTH;
                                }

                                TextFormatFlags tf = ToolStripRendererUtils.GetCaptionFormat(toolStrip);
                                Color clrText = this.CaptionText;

                                switch (ToolStripRendererUtils.GetCaptionTextStyle(toolStrip))
                                {
                                    case CaptionTextStyle.Shadow:
                                        {
                                            rcText.Offset(1, 1);
                                            Color clr = Office12ColorTable.GetAlphaBlendedColor(Color.Black,
                                                bIsToolStripGrouped ? this.OfficeColorTable.CaptionGroupedGradientBegin : this.OfficeColorTable.CaptionGradientBegin, 48);

                                            TextRenderer.DrawText(g, toolStrip.Text, f, rcText, clr, tf);
                                            rcText.Offset(-2, -2);
                                        }
                                        break;
                                    case CaptionTextStyle.Etched:
                                        {
                                            if (clrText.GetBrightness() < 0.9F)
                                            {
                                                rcText.Offset(1, 1);
                                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, Color.White, tf);
                                                rcText.Offset(-1, -1);
                                            }
                                            else
                                            {
                                                rcText.Offset(-1, -1);
                                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, Color.Black, tf);
                                                rcText.Offset(1, 1);
                                            }
                                        }
                                        break;
                                }

                                Color color = (toolStrip.ForeColor == Color.MidnightBlue) ? clrText : toolStrip.ForeColor;
                                if (SystemInformation.HighContrast)
                                    color = SystemColors.MenuText;
                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, color, tf);
                                PaintLauncher(g, rcUpdate, toolStrip as IToolStripExSupport2);

                                bg.Render();
                            }
                            WindowsAPI.ReleaseDC(hWnd, hdc);
                        }
                    }
                }
            }
        }

        protected virtual void DrawBorders(ToolStrip ts)
        {
            switch (ToolStripRendererUtils.GetBorderStyle(ts))
            {
                case ToolStripBorderStyle.StaticEdge:
                    DrawStaticEdgeBorders(ts);
                    break;
                case ToolStripBorderStyle.Etched:
                    DrawEtchedBorders(ts);
                    break;
            }
        }

        protected virtual void DrawStaticEdgeBorders(ToolStrip ts)
        {
            IntPtr wnd = ts.Handle;

            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                if (hdc != IntPtr.Zero)
                {
                    Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        Color clBorder = IsGroupedToolStrip(ts) ? this.OfficeColorTable.RibbonPanelGroupedBorderEnd : this.BorderColor;

                        using (Pen pen = new Pen(clBorder))
                        {
                            Point[] pt = new Point[]
							{
								new Point(rc.X,rc.Bottom-2),
								new Point(rc.X,rc.Y),
								new Point(rc.Right-2,rc.Y)
							};

                            g.DrawLines(pen, pt);
                        }

                        using (Pen pen = new Pen(Color.White))
                        {
                            Point[] pt = new Point[]
							{
								new Point(rc.Right-1,rc.Y),
								new Point(rc.Right-1,rc.Bottom-1),
								new Point(rc.X,rc.Bottom-1),
							};

                            g.DrawLines(pen, pt);
                        }
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        protected virtual void DrawEtchedBorders(ToolStrip ts)
        {
            IntPtr wnd = ts.Handle;

            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                if (hdc != IntPtr.Zero)
                {
                    Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                    WindowsAPI.ExcludeClipRect(hdc, rc.X + ETCHED_WIDTH, rc.Y + ETCHED_WIDTH, rc.Right - ETCHED_WIDTH, rc.Bottom - ETCHED_WIDTH);

                    using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rc))
                    {
                        Graphics g = bg.Graphics;
                        GraphicsState gState = g.Save();

                        using (Region region = RendererUtils.GetRoundedRegion(rc, 1))
                        {
                            Color clBorder = this.BorderColor;
                            Color clTabGroup = Color.Empty;
                            bool bIsGrouped = IsGroupedToolStrip(ts, ref clTabGroup);

                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            Color color = System.Drawing.ColorTranslator.FromHtml("#E7EEF6");
                            using(Brush br=new SolidBrush(color))
                                g.FillRegion(br, region);

                            if (bIsGrouped)
                            {
                                ToolStripEx tsEx = ts as ToolStripEx;
                                bool bSelected = GetIsSelected(ts);

                                clBorder = this.OfficeColorTable.RibbonPanelGroupedBorderEnd;

                                if (tsEx != null && tsEx.State == ToolStripEx.ToolStripExState.Collapsed)
                                {
                                    // Collapsed ToolStripEx is selected and DropDown is not visible.
                                    if (bSelected && !tsEx.DropDownButton.DropDown.Visible)
                                    {
                                        using (Pen penGrouped = new Pen(GetGroupedBackgroundCollapsedBrush(Color.White, clTabGroup, rc.Height)))
                                        {
                                            g.DrawPolygon(penGrouped, RendererUtils.GetRoundedPolygon(Rectangle.Inflate(rc, -1, -1), 1));
                                        }
                                    }
                                }
                            }

                            using (Pen pen = new Pen(clBorder))
                            {
                                if (!bIsGrouped)
                                {
                                    rc.Width -= 1;
                                    rc.Height -= 1;
                                }
                                g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rc, 1));
                            }
                        }

                        g.Restore(gState);
                        bg.Render();
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        protected virtual Image GetLauncherImage(IToolStripExSupport2 iRibbon)
        {
            bool bSelected = iRibbon.LauncherSelected;
            bool isFormFocused = false;
            bool isFormNotNull = false;
            if (iRibbon is ToolStrip)
            {
                ToolStrip ts = iRibbon as ToolStrip;
                isFormFocused = (ts.TopLevelControl is Form) && (ts.TopLevelControl.ContainsFocus);
                isFormNotNull = !(ts.TopLevelControl is Form);
            }
            switch (iRibbon.LauncherStyle)
            {
                case LauncherStyle.Office2007:
                    return GetLauncherImage((bSelected && isFormNotNull || (bSelected && isFormFocused) ? EBITMAP.ebLauncher2007Selected : EBITMAP.ebLauncher2007));
            }

            return GetLauncherImage((bSelected && isFormNotNull) || (bSelected && isFormFocused) ? EBITMAP.ebLauncherSelected : EBITMAP.ebLauncher);
        }

        protected virtual Image GetLauncherImage(EBITMAP eBitmap)
        {
            Image image = m_htBitmaps[eBitmap] as Image;
            if (image == null)
            {
                Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_WIDTH, LAUNCHER_HEIGHT);
                image = new Bitmap(rcImage.Width, rcImage.Height);

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.Transparent);

                    switch (eBitmap)
                    {
                        case EBITMAP.ebLauncher:
                        case EBITMAP.ebLauncherSelected:
                            {
                                Rectangle rc = Rectangle.Inflate(rcImage, -1, -1);
                                rc.Width += 1;
                                rc.Height += 1;

                                Point[] ptLauncher = GetLauncherPolygon(rc);

                                using (GraphicsPath path = new GraphicsPath())
                                {
                                    path.AddPolygon(ptLauncher);
                                    using (Region rgLauncher = new Region(path))
                                    {
                                        g.Clip = rgLauncher;

                                        using (Brush brush = new SolidBrush(this.LauncherBackground))
                                        {
                                            g.FillRegion(brush, rgLauncher);
                                        }

                                        rc.X += rc.Width / 2 - 1;
                                        rc.Y += 1;
                                        rc.Width /= 2;
                                        rc.Height /= 2;

                                        using (Region region = new Region(new Rectangle(rc.X + rc.Width / 2 - 1, rc.Y, 2, rc.Height)))
                                        {
                                            region.Union(new Rectangle(rc.X, rc.Y + rc.Height / 2 - 1, rc.Width, 2));

                                            Color color = (eBitmap == EBITMAP.ebLauncher) ? this.LauncherText : this.LauncherTextSelected;

                                            using (SolidBrush brush = new SolidBrush(color))
                                            {
                                                g.FillRegion(brush, region);
                                            }
                                        }
                                        g.DrawPolygon(LauncherBorder, ptLauncher);
                                    }
                                }
                            }
                            break;
                        case EBITMAP.ebLauncher2007:
                        case EBITMAP.ebLauncher2007Selected:
                            {
                                if (eBitmap == EBITMAP.ebLauncher2007Selected)
                                {
                                    g.DrawRectangle(this.LauncherBorder, rcImage);

                                    Rectangle rcBackground = Rectangle.Inflate(rcImage, -1, -1);

                                    using (LinearGradientBrush brush = GetVerticalBrush(ref rcBackground, this.LauncherBackground, Color.White))
                                    {
                                        g.FillRectangle(brush, rcBackground);
                                    }
                                }

                                Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                                rc.Offset(1, 1);

                                using (Region region = new Region(rc))
                                {
                                    region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                                    region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                                    region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                                    region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                                    region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                                    region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                                    g.FillRegion(Brushes.White, region);

                                    region.Translate(-1, -1);

                                    Color color = eBitmap == EBITMAP.ebLauncher2007 ? this.LauncherText : this.LauncherTextSelected;

                                    if (this.ColorTable is Office12ColorTable || this.ColorTable is OfficeBlack)
                                        color = System.Drawing.ColorTranslator.FromHtml("#656870");

                                    if (this.ColorTable is OfficeBlue)
                                        color = System.Drawing.ColorTranslator.FromHtml("#668EAF");

                                    using (SolidBrush brush = new SolidBrush(color))
                                    {
                                        g.FillRegion(brush, region);
                                    }
                                }
                            }
                            break;
                    }
                }

                m_htBitmaps[eBitmap] = image;
            }
            return image;
        }

        protected virtual void PaintLauncher(Graphics g, Rectangle rcCaption, IToolStripExSupport2 iRibbon)
        {
            if (iRibbon != null && iRibbon.ShowLauncher)
            {
                Rectangle rc = ToolStripRendererUtils.GetLauncherBounds(rcCaption, false);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Image image = GetLauncherImage(iRibbon);

                    if (image != null)
                    {
                        g.DrawImage(image, rc.Location);
                    }
                }
            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            if (toolStrip != null)
            {
                Attach(toolStrip);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                base.OnRenderToolStripBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBorder(e))
            {
                base.OnRenderToolStripBorder(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
            {
                base.OnRenderItemText(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemImage(e))
            {
                base.OnRenderItemImage(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemCheck(e))
            {
                base.OnRenderItemCheck(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
            {
                base.OnRenderButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
            {
                base.OnRenderDropDownButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
            {
                base.OnRenderOverflowButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintArrow(e))
            {
                base.OnRenderArrow(e);
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHandleCreated(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                AssignHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHandleDestroyed(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                ReleaseHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRendererChanged(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && ts.Renderer != this)
            {
                Detach(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDockChanged(object sender, EventArgs e)
        {
            ToolStripRendererUtils.UpdateFrame(sender as ToolStrip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLayoutCompleted(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && HasCaption(ts))
            {
                if (ts.AutoSize)
                {
                    bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;

                    int nMaxBound = bVertical ? ts.Padding.Left : ts.Padding.Top;

                    ToolStripItemCollection items = ts.Items;

                    for (int i = 0, count = items.Count; i < count; i++)
                    {
                        ToolStripItem item = items[i];
                        if (item.Placement == ToolStripItemPlacement.Main)
                        {
                            int nItemBound = bVertical ? item.Bounds.Right : item.Bounds.Bottom;
                            if (nItemBound > nMaxBound)
                            {
                                nMaxBound = nItemBound;
                            }
                        }
                    }

                    if (bVertical)
                    {
                        nMaxBound += ts.Padding.Right + ToolStripRendererUtils.GetCaptionHeight(ts, this.RenderType);

                        if (nMaxBound != ts.MinimumSize.Width)
                        {
                            ts.MinimumSize = new Size(nMaxBound, ts.MinimumSize.Height);
                        }
                    }
                    else
                    {
                        nMaxBound += ts.Padding.Bottom + ToolStripRendererUtils.GetCaptionHeight(ts, this.RenderType);

                        if (nMaxBound != ts.MinimumSize.Height)
                        {
                            ts.MinimumSize = new Size(ts.MinimumSize.Width, nMaxBound);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripComboBox)
            {
                OnAddedComboBox(e.Item as ToolStripComboBox);
            }
            else if (e.Item is ToolStripDropDownItem)
            {
                OnAddedDropDownItem(e.Item as ToolStripDropDownItem);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemRemoved(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripComboBox)
            {
                OnRemovedComboBox(e.Item as ToolStripComboBox);
            }
            else if (e.Item is ToolStripDropDownItem)
            {
                OnRemovedDropDownItem(e.Item as ToolStripDropDownItem);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnComboBoxHandleCreated(object sender, EventArgs e)
        {
            SubclassControl(sender as Control, m_ComboBoxRenderer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnComboBoxHandleDestroyed(object sender, EventArgs e)
        {
            ReleaseControl(sender as Control);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRoundedToolstripRegionChanged(object sender, EventArgs e)
        {
            ToolStrip toolstrip = sender as ToolStrip;

            if (toolstrip != null && toolstrip.IsHandleCreated)
            {
                toolstrip.Region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, toolstrip.Size), TOOLSTRIP_RADIUS);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Attach(ToolStrip ts)
        {
            Subscribe(ts);

            m_refCounter++;

            if (m_refCounter == 1)
            {
                SystemInfo.SettingsChanged += new SettingsChangedEventHandler(SettingsChangedHandler);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Detach(ToolStrip ts)
        {
            Unsubscribe(ts);

            if (m_refCounter > 0)
            {
                m_refCounter--;

                if (m_refCounter == 0)
                {
                    SystemInfo.SettingsChanged -= new SettingsChangedEventHandler(SettingsChangedHandler);
                    Clear();
                }
            }
        }

        #region Attach/Detach items
        void AttachItems(ToolStrip ts)
        {
            ts.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);
            ts.ItemRemoved += new ToolStripItemEventHandler(OnItemRemoved);

            foreach (ToolStripItem item in ts.Items)
            {
                OnItemAdded(ts, new ToolStripItemEventArgs(item));
            }
        }
        void DetachItems(ToolStrip ts)
        {
            ts.ItemAdded -= new ToolStripItemEventHandler(OnItemAdded);
            ts.ItemRemoved -= new ToolStripItemEventHandler(OnItemRemoved);

            if (!(ts.Renderer is Office12ToolStripRenderer))
            {
                foreach (ToolStripItem item in ts.Items)
                {
                    OnItemRemoved(ts, new ToolStripItemEventArgs(item));
                }
            }
        }
        #endregion

        void Subscribe(ToolStrip ts)
        {
            ts.RendererChanged += new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated += new EventHandler(OnHandleCreated);
                ts.HandleDestroyed += new EventHandler(OnHandleDestroyed);
                ts.DockChanged += new EventHandler(OnDockChanged);

                if (ts.IsHandleCreated)
                {
                    OnHandleCreated(ts, EventArgs.Empty);
                }

                // Workaround to fix layouting problem in standard ToolStrip controls
                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted += new EventHandler(OnLayoutCompleted);
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated += new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged += new EventHandler(OnRoundedToolstripRegionChanged);
            }

            AttachItems(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Unsubscribe(ToolStrip ts)
        {
            DetachItems(ts);

            ts.RendererChanged -= new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated -= new EventHandler(OnHandleCreated);
                ts.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
                ts.DockChanged -= new EventHandler(OnDockChanged);

                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
                }

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ReleaseHandle(ts);
                    ClearDropDownRegions(ts);
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated -= new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged -= new EventHandler(OnRoundedToolstripRegionChanged);

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ts.Region = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void AssignHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                if (!m_htControls.ContainsKey(ts))
                {
                    CToolStripWindow wnd = new CToolStripWindow(ts);

                    wnd.AssignHandle(ts.Handle);
                    m_htControls.Add(ts, wnd);
                }
            }

            ts.Region = ToolStripRendererUtils.GetRegion(ts);

            ToolStripRendererUtils.UpdateFrame(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void ReleaseHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                object obj = m_htControls[ts];
                if (obj != null)
                {
                    CToolStripWindow wnd = obj as CToolStripWindow;

                    if (wnd != null)
                    {
                        wnd.ReleaseHandle();
                    }

                    m_htControls.Remove(ts);
                }
            }

            if (!(ts.Renderer is Office12ToolStripRenderer))
            {
                ts.Region = null;
            }

            ToolStripRendererUtils.UpdateFrame(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        void SubclassControl(Control control, INativeMessageFilter messageFilter)
        {
            lock (m_controlsLock)
            {
                NativeMessageHandler handler = m_htControls[control] as NativeMessageHandler;

                if (handler == null)
                {
                    handler = new NativeMessageHandler();
                    m_htControls.Add(control, handler);

                    handler.Assign(control.Handle);
                }

                handler.MessageFilter = messageFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        void ReleaseControl(Control control)
        {
            lock (m_controlsLock)
            {
                object obj = m_htControls[control];
                if (obj != null)
                {
                    NativeMessageHandler handler = obj as NativeMessageHandler;

                    if (handler != null)
                    {
                        handler.MessageFilter = null;
                    }

                    m_htControls.Remove(control);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripComboBox"></param>
        void OnAddedComboBox(ToolStripComboBox toolStripComboBox)
        {
            Control comboBox = toolStripComboBox.Control;

            if (comboBox != null)
            {
                comboBox.HandleCreated += new EventHandler(OnComboBoxHandleCreated);
                comboBox.HandleDestroyed += new EventHandler(OnComboBoxHandleDestroyed);

                if (comboBox.IsHandleCreated)
                {
                    SubclassControl(comboBox, m_ComboBoxRenderer);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripComboBox"></param>
        void OnRemovedComboBox(ToolStripComboBox toolStripComboBox)
        {
            Control comboBox = toolStripComboBox.Control;

            if (comboBox != null)
            {
                comboBox.HandleCreated -= new EventHandler(OnComboBoxHandleCreated);
                comboBox.HandleDestroyed -= new EventHandler(OnComboBoxHandleDestroyed);

                if (comboBox.IsHandleCreated)
                {
                    ReleaseControl(comboBox);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripDropDownItem"></param>
        void OnAddedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
        {
            AttachItems(toolStripDropDownItem.DropDown);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripDropDownItem"></param>
        void OnRemovedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
        {
            DetachItems(toolStripDropDownItem.DropDown);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        private void ClearDropDownRegions(ToolStrip toolStrip)
        {
            ToolStripItemCollection items = toolStrip.Items;

            for (int i = 0, count = items.Count; i < count; i++)
            {
                ToolStripDropDownItem item = items[i] as ToolStripDropDownItem;
                if (item != null && item.DropDown != null && item.Placement != ToolStripItemPlacement.None)
                {
                    ClearDropDownRegions(item.DropDown);
                    item.DropDown.Region = null;
                }
            }

            ToolStripDropDownItem overflowButton = toolStrip.OverflowButton;
            if (overflowButton != null && overflowButton.DropDown != null)
            {
                overflowButton.DropDown.Region = null;
            }
        }

        private bool PaintToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is BottomToolstrip)
            {
                bResult = PaintBottomToolstripBackground(e);
            }
            else if (e.ToolStrip is ContextMenuStripEx)
            {
                bResult = PaintContextMenuBackground(e);
            }
            else if (e.ToolStrip is MenuStrip)
            {
                bResult = PaintMenuBackground(e);
            }
            else if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                bResult = PaintToolStripGalleryDropDownBackground(e);
            }
            else
                if (e.ToolStrip is StatusStrip)
                {
                    bResult = PaintStatusStripBackground(e);
                }
                else
                    if (e.ToolStrip is ToolStripDropDown)
                    {
                        bResult = PaintDropdownBackground(e);
                    }
                    else
                    {
                        bResult = PaintGroupedToolbarBackground(e) || PaintToolbarBackground(e);
                    }

            return bResult;
        }

        private bool PaintToolStripBorder(ToolStripRenderEventArgs e)
        {
            bool bResult = true;

            if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                if (e.ToolStrip.IsDropDown)
                {
                    e.Graphics.DrawPolygon(MenuBorder, RendererUtils.GetRoundedPolygon(rc, TOOLSTRIP_RADIUS));
                }
                else if (e.ToolStrip is MenuStrip)
                {
                    e.Graphics.DrawLine(MenuStripBorder, new Point(rc.X, rc.Bottom - 1), new Point(rc.Right - 1, rc.Bottom - 1));
                }
                else
                {
                    switch (GetRenderType(e.ToolStrip))
                    {
                        case ERENDERTYPE.Grouped:
                            PaintGroupBorders(e);
                            break;
                    }
                }
            }
            return bResult;
        }

        private bool PaintDropdownBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripDropDown ts = e.ToolStrip as ToolStripDropDown;
            if (ts != null)
            {
                if (ts.Width > 0 && ts.Height > 0)
                {
                    Rectangle rc = new Rectangle(Point.Empty, ts.Size);
                    if (!RendererUtils.IsValidRegion(ts, e.Graphics))
                    {
                        ts.Region = RendererUtils.GetRoundedRegion(rc, TOOLSTRIP_RADIUS);
                    }

                    e.Graphics.FillRectangle(ToolStripDropDownBackground, rc);
                }
                bResult = true;
            }

            return bResult;
        }

        private bool PaintMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStrip ms = e.ToolStrip;
            if (ms != null)
            {
                Size szMenu = ms.Size;
                if (szMenu.Width > 0 && szMenu.Height > 0)
                {
                    Rectangle rcMenu = new Rectangle(Point.Empty, szMenu);

                    Color clBegin = ColorTable.MenuStripGradientBegin;
                    Color clEnd = ColorTable.MenuStripGradientEnd;

                    Control parent = ms.Parent;
                    if (parent != null)
                    {
                        Size szParent = parent.ClientSize;
                        if (szParent.Width > 0 && szParent.Height > 0)
                        {
                            Rectangle rcParent = new Rectangle(Point.Empty, szParent);
                            using (LinearGradientBrush brush = GetHorizontalBrush(ref rcParent, clBegin, clEnd))
                            {
                                brush.TranslateTransform(szParent.Width - ms.Location.X, szParent.Height - ms.Location.Y, MatrixOrder.Append);

                                brush.Blend = m_blMenuBar;
                                e.Graphics.FillRectangle(brush, rcMenu);
                            }
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush brush = GetHorizontalBrush(ref rcMenu, clBegin, clEnd))
                        {
                            brush.Blend = m_blMenuBar;
                            e.Graphics.FillRectangle(brush, rcMenu);
                        }
                    }
                    bResult = true;
                }
            }

            return bResult;
        }

        private bool PaintStatusStripBackground(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);

                Color clBegin = ColorTable.StatusStripGradientBegin;
                Color clEnd = ColorTable.StatusStripGradientEnd;

                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blStatusBar;
                    e.Graphics.FillRectangle(brush, rc);
                }
            }

            return true;
        }

        private bool PaintToolbarBackground(ToolStripRenderEventArgs e)
        {
            ToolStrip ts = e.ToolStrip;
            if (ts != null)
            {
                if (ts.Width > 0 && ts.Height > 0)
                {
                    Rectangle rc = new Rectangle(Point.Empty, ts.ClientSize);

                    if (RenderType == ERENDERTYPE.TabBar)
                    {
                        Color clBegin = ColorTable.MenuStripGradientBegin;
                        Color clEnd = ColorTable.MenuStripGradientEnd;

                        using (LinearGradientBrush brush = GetHorizontalBrush(ref rc, clBegin, clEnd))
                        {
                            PanelItemRenderEventArgs pie = e as PanelItemRenderEventArgs;
                            if (pie != null)
                            {
                                Point pt = pie.PanelStrip.Location;
                                brush.TranslateTransform(-pt.X, -pt.Y, MatrixOrder.Append);
                            }

                            brush.WrapMode = WrapMode.TileFlipXY;
                            brush.Blend = m_blMenuBar;
                            e.Graphics.FillRectangle(brush, rc);
                        }
                    }
                    else
                    {
                        bool bSelected = GetIsSelected(e.ToolStrip);

                        Color clBegin = bSelected ? this.ToolStripHighlightGradientBegin : ColorTable.ToolStripGradientBegin;
                        Color clEnd = bSelected ? this.ToolStripHighlightGradientEnd : ColorTable.ToolStripGradientEnd;

                        bool bRounded = RoundedEdges && e.ToolStrip.Dock == DockStyle.None;
                        using (Region region = RendererUtils.GetRoundedRegion(rc, bRounded ? TOOLSTRIP_RADIUS : 0))
                        {
                            POINT ptBrush = new POINT();

                            Control bgControl = ts.Parent is RibbonPanel ? ts.Parent : ts;
                            WindowsAPI.MapWindowPoints(ts.Handle, bgControl.Handle, ref ptBrush, 1);

                            Rectangle rcBrush = new Rectangle(-ptBrush.x, -ptBrush.y, bgControl.ClientSize.Width, bgControl.ClientSize.Height);

                            using (LinearGradientBrush brush = GetVerticalBrush(ref rcBrush, clBegin, clEnd))
                            {
                                PanelItemRenderEventArgs pie = e as PanelItemRenderEventArgs;
                                if (pie != null)
                                {
                                    POINT pt = new POINT(0, 0);
                                    WindowsAPI.MapWindowPoints(pie.PanelStrip.Handle, e.ToolStrip.Handle, ref pt, 1);
                                    brush.TranslateTransform(-pt.x, -pt.y, MatrixOrder.Append);
                                }

                                brush.Blend = m_blToolBar;
                                brush.WrapMode = WrapMode.TileFlipXY;
                                e.Graphics.FillRegion(brush, region);
                            }
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        private bool PaintGroupedToolbarBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStrip ts = e.ToolStrip;

            if (ts != null && ts.Width > 0 && ts.Height > 0)
            {
                Color clTabGroup = Color.White;

                bool bIsGrouped = IsGroupedToolStrip(e.ToolStrip, ref clTabGroup);

                if (bIsGrouped)
                {
                    bool bRounded = RoundedEdges && e.ToolStrip.Dock == DockStyle.None;
                    Rectangle rc = new Rectangle(Point.Empty, ts.ClientSize);

                    using (Region region = RendererUtils.GetRoundedRegion(rc, bRounded ? TOOLSTRIP_RADIUS : 0))
                    {
                        POINT ptBrush = new POINT();

                        Control bgControl = ts.Parent is RibbonPanel ? ts.Parent : ts;
                        WindowsAPI.MapWindowPoints(ts.Handle, bgControl.Handle, ref ptBrush, 1);

                        Rectangle rcBrush = new Rectangle(-ptBrush.x, -ptBrush.y, bgControl.ClientSize.Width, bgControl.ClientSize.Height);

                        Color bgColor = GetIsSelected(ts) ? this.ToolStripHighlightGradientBegin : clTabGroup;
                        using (LinearGradientBrush brush = GetGroupedBackgroundBrush(bgColor, Color.White, ref rcBrush))
                        {
                            PanelItemRenderEventArgs pie = e as PanelItemRenderEventArgs;
                            if (pie != null)
                            {
                                POINT pt = new POINT(0, 0);
                                WindowsAPI.MapWindowPoints(pie.PanelStrip.Handle, e.ToolStrip.Handle, ref pt, 1);
                                brush.TranslateTransform(-pt.x, -pt.y, MatrixOrder.Append);
                            }
                            e.Graphics.FillRegion(brush, region);
                        }
                    }

                    bResult = true;
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintContextMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            RECT rect = new RECT();

            ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

            if (menuStrip != null)
            {
                if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                        Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                        Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

                        using (Brush brush = new SolidBrush(this.OfficeColorTable.ToolStripDropDownBackground))
                        {
                            e.Graphics.FillRectangle(brush, rectangleBody);
                        }

                        using (Brush brush = new SolidBrush(this.OfficeColorTable.ContextMenuTitle))
                        {
                            e.Graphics.FillRectangle(brush, rectangleTitle);
                        }

                        using (Pen pen = new Pen(Color.FromArgb(60, Color.Black)))
                        {
                            e.Graphics.DrawLine(
                                pen, rectangleTitle.Left + 3, rectangleTitle.Bottom, rectangleTitle.Right - 3, rectangleTitle.Bottom);
                        }

                        Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

                        TextFormatFlags flags = TextFormatFlags.EndEllipsis;

                        if (menuStrip.RightToLeft == RightToLeft.Yes)
                        {
                            flags |= TextFormatFlags.Right;
                        }

                        TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, this.OfficeColorTable.RibbonTabText, flags);

                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintBottomToolstripBackground(ToolStripRenderEventArgs e)
        {
            BottomToolstrip toolstrip = e.ToolStrip as BottomToolstrip;

            if (toolstrip != null)
            {
                using (Brush brush = new SolidBrush(this.OfficeColorTable.BottomToolstrip))
                {
                    Rectangle rect = new Rectangle(new Point(0, 0), toolstrip.Size);
                    e.Graphics.FillRegion(brush, RendererUtils.GetRoundedRegion(rect, TOOLSTRIP_RADIUS));
                    rect.Inflate(-1, -1);
                    using (Pen p = new Pen(Color.FromArgb(150, Color.White)))
                    {
                        e.Graphics.DrawPolygon(p, RendererUtils.GetRoundedPolygon(rect, TOOLSTRIP_RADIUS));
                    }
                    rect.X--;
                    rect.Y--;
                    using (Pen p = new Pen(Color.FromArgb(40, Color.Black)))
                    {
                        e.Graphics.DrawPolygon(p, RendererUtils.GetRoundedPolygon(rect, TOOLSTRIP_RADIUS));
                    }
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item.IsOnDropDown)
            {
                bResult = PaintDropDownMenuItemBackground(e);
            }
            else
            {
                bResult = PaintToolStripMenuItemBackground(e);
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

            if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
            {
                rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
            }

            if (e.ToolStrip is ContextMenuStripEx)
            {
                ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                rc.Y += statusStrip.TitleHeight;
                rc.Height -= statusStrip.TitleHeight;
            }

            using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
            {
                g.FillRectangle(brush, rc);
            }

            int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
            int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);
            g.DrawLine(Pens.White, beginX, rc.Top, beginX, rc.Bottom - 1);
            g.DrawLine(this.MenuItemBorder, endX, rc.Top, endX, rc.Bottom - 1);

            return true;
        }

        private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripItem tsItem = e.Item;
            if (tsItem.Enabled)
            {
                if (tsItem.Selected || tsItem.Pressed)
                {
                    Rectangle rc = GetItemRect(tsItem);

                    using (Region rgnItem = RendererUtils.GetRoundedRegion(rc, MENUITEM_RADIUS))
                    {
                        Graphics g = e.Graphics;

                        Color clBegin = tsItem.Pressed ? ColorTable.MenuItemPressedGradientBegin : ColorTable.MenuItemSelectedGradientBegin;
                        Color clEnd = tsItem.Pressed ? ColorTable.MenuItemPressedGradientEnd : ColorTable.MenuItemSelectedGradientEnd;

                        using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                        {
                            brush.Blend = m_blMenuItemUp;
                            g.FillRegion(brush, rgnItem);
                        }

                        PaintFlash(g, rc, false);

                        g.DrawPolygon(MenuItemBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));
                    }

                    bResult = true;
                }
            }

            return bResult;
        }

        private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

            if (tsItem != null && tsItem.Selected || tsItem.Pressed)
            {
                Rectangle rc = GetItemRect(tsItem);

                if (tsItem.Enabled)
                {
                    using (Region rgBackground = RendererUtils.GetRoundedRegion(rc, MENUITEM_RADIUS))
                    {
                        Graphics g = e.Graphics;

                        using (LinearGradientBrush brush = GetVerticalBrush(ref rc, ColorTable.MenuItemPressedGradientBegin, ColorTable.MenuItemPressedGradientEnd))
                        {
                            brush.Blend = ToolStripRendererUtils.GetIsPressed(tsItem) ? m_blMenuItemDown : m_blMenuItemUp;
                            e.Graphics.FillRegion(brush, rgBackground);
                        }

                    }

                    e.Graphics.DrawPolygon(MenuItemBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));
                }
                else
                {
                    using (Pen pen = new Pen(ColorTable.MenuItemPressedGradientEnd))
                    {
                        e.Graphics.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                    }
                }


                bResult = true;
            }
            return bResult;
        }

        private bool PaintArrow(ToolStripArrowRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                Point loc = new Point();
                loc.Y = item.Height - item.Padding.Bottom - GetDownArrowSize().Height;
                loc.X = (item.Width - item.Padding.Horizontal - this.ArrowDownImage.Width) / 2 + item.Padding.Left;

                e.Graphics.DrawImage(this.ArrowDownImage, loc);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                Rectangle rc = ToolStripRendererUtils.GetTextRect(e);

                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, e.Item.ForeColor, e.TextFormat);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;
                if (tsItem != null && tsItem.Checked && !ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
                {
                    PaintItemCheckBackground(e);
                }
            }

            Image image = e.Image;
            Rectangle rc = ToolStripRendererUtils.GetImageRect(e);

            if (image != null && rc.Width > 0 && rc.Height > 0)
            {
                bool bDisposeImage = false;

                if (!e.Item.Enabled)
                {
                    image = CreateDisabledImage(image);
                    bDisposeImage = true;
                }

                Graphics g = e.Graphics;

                if (e.Item is CollapsedDropDownButton)
                {
                    Rectangle borderRect = rc;
                    borderRect.Inflate(ToolStripEx.DEF_IMAGE_BORDER_OFFSET, ToolStripEx.DEF_IMAGE_BORDER_OFFSET);
                    bool bIsGroupedToolStrip = IsGroupedToolStrip(e.ToolStrip);

                    Color cl1 = (ToolStripRendererUtils.GetIsChecked(e.Item) && !bIsGroupedToolStrip)
                        ? (this.OfficeColorTable.CollapsedImagePressedGradientBegin)
                        : (this.OfficeColorTable.CollapsedImageGradientBegin);

                    Color cl2 = (ToolStripRendererUtils.GetIsChecked(e.Item) && !bIsGroupedToolStrip)
                        ? (this.OfficeColorTable.CollapsedImagePressedGradientEnd)
                        : (this.OfficeColorTable.CollapsedImageGradientEnd);

                    using (LinearGradientBrush brush = GetVerticalBrush(ref borderRect, cl1, cl2))
                    {
                        brush.Blend = m_blImageBackground;
                        brush.WrapMode = WrapMode.TileFlipXY;
                        g.FillRegion(brush, RendererUtils.GetRoundedRegion(borderRect, 2));
                    }

                    g.DrawPolygon(this.ImageBorderPen, RendererUtils.GetRoundedPolygon(borderRect, 2));
                }

                if (e.Item is OfficeButton)
                {
                    if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        rc.X -= 1;
                    }
                    else
                    {
                        rc.X += 1;
                    }
                }

                if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                {
                    Size szImage = image.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    g.DrawImage(image, new Point(iImageX, iImageY));
                }
                else
                {
                    InterpolationMode interpolation = g.InterpolationMode;

                    if (image.Size != rc.Size)
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    g.DrawImage(image, rc);

                    g.InterpolationMode = interpolation;
                }

                if (bDisposeImage)
                {
                    image.Dispose();
                }
            }

            return true;
        }

        private bool PaintItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            ToolStripItem tsItem = e.Item;

            if (tsItem.Image == null || ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
            {
                PaintItemCheckBackground(e);

                Rectangle rc = GetCheckRect(e);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Size szImage = this.CheckButton.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
                }
            }

            return true;
        }

        private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = GetCheckRect(e);
            if (rc.Width > 0 && rc.Height > 0)
            {
                Graphics g = e.Graphics;

                GraphicsState gState = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.FillRectangle(this.CheckBackground, Rectangle.Inflate(rc, -1, -1));
                g.DrawPolygon(this.CheckBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));

                g.Restore(gState);
            }
        }

        private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintTabBarButtonBackground(e.Item, e.Graphics, GetButtonRect(e.Item)))
            {
                if (!PaintGroupedButtonBackground(e))
                {
                    if (!PaintCollapsedButtonBackground(e))
                    {
                        PaintToolBarButtonBackground(e);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintToolBarButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintButtonBackgroundPressed(e))
            {
                if (!PaintButtonBackgroundChecked(e))
                {
                    PaintButtonBackgroundSelected(e);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (GetIsGrouped(e.Item))
            {
                if (!PaintGroupedBackgroundPressed(e))
                {
                    if (!PaintGroupedBackgroundChecked(e))
                    {
                        if (!PaintGroupedBackgroundSelected(e))
                        {
                            PaintGroupedBackgroundNormal(e);
                        }
                    }
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool PaintTabBarButtonBackground(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (GetIsTabItem(item))
            {
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (!PaintTabBackgroundPressed(item, g, rc))
                    {
                        if (!PaintTabBackgroundChecked(item, g, rc))
                        {
                            PaintTabBackgroundSelected(item, g, rc);
                        }
                    }
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintCollapsedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                if (!PaintCollapsedBackgroundPressed(e))
                {
                    if (!PaintCollapsedBackgroundSelected(e))
                    {
                        PaintCollapsedBackgroundNormal(e);
                    }
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;

                    Color cl1 = ColorTable.ButtonPressedGradientBegin;
                    Color cl2 = ColorTable.ButtonPressedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);
                    PaintButtonPressedShadow(g, rcBackground);

                    g.DrawImage(this.PressedFlashImage, rc.X, rc.Y + rc.Height / 2, rc.Width, rc.Height);

                    PaintButtonPressedBorder(g, ref rc, ColorTable.ButtonPressedGradientEnd);
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                    Color cl1 = ColorTable.ButtonCheckedGradientBegin;
                    Color cl2 = ColorTable.ButtonCheckedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);
                    PaintButtonPressedShadow(g, rcBackground);

                    PaintFlashChecked(g, rcBackground);

                    PaintButtonPressedBorder(g, ref rc, ColorTable.ButtonCheckedGradientEnd);
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;
            bool isPanelNotInForm = false;
            if (!(e.ToolStrip.TopLevelControl is Form))
            {
                  isPanelNotInForm = true;
            }
            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (isPanelNotInForm || (rc.Width > 0 && rc.Height > 0 && e.ToolStrip.TopLevelControl.ContainsFocus))
                {
                    Graphics g = e.Graphics;

                    Color cl1 = this.OfficeColorTable.ButtonSelectedGradientBegin;
                    Color cl2 = this.OfficeColorTable.ButtonSelectedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);

                    g.DrawImage(this.SelectedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height);

                    PaintButtonBorder(g, rc, EBUTTONSTATE.Selected);
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;

                    Color cl1 = ColorTable.ButtonPressedGradientBegin;
                    Color cl2 = ColorTable.ButtonPressedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);
                    PaintButtonPressedShadow(g, rcBackground);

                    g.DrawImage(this.PressedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height);
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;

                    Color cl1 = ColorTable.ButtonCheckedGradientBegin;
                    Color cl2 = ColorTable.ButtonCheckedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);
                    PaintButtonPressedShadow(g, rcBackground);

                    PaintFlashChecked(g, rcBackground);
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;

                    Color cl1 = ColorTable.ButtonSelectedGradientBegin;
                    Color cl2 = ColorTable.ButtonSelectedGradientEnd;

                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintGradientSelected(g, rcBackground, cl1, cl2);
                    g.DrawImage(this.SelectedFlashImage, rcBackground.X, rcBackground.Y + rcBackground.Height / 2, rcBackground.Width, rcBackground.Height);
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintGroupedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = GetButtonRect(e.Item);
            if (rc.Width > 0 && rc.Height > 0)
            {
                PaintGradientGrouped(e.Graphics, GetButtonBackgroundRect(e.Item, rc));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool PaintTabBackgroundPressed(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (item.Pressed)
            {
                Rectangle rcTab = rc;
                rcTab.Height -= 1;

                using (Region region = GetTabbedRegion(rcTab))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Color cl1 = ColorTable.ButtonPressedGradientBegin;
                    Color cl2 = ColorTable.ButtonPressedGradientEnd;

                    PaintGradientSelected(g, rcTab, cl1, cl2);
                    PaintTabBorder(g, rcTab, EBUTTONSTATE.Pressed);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintTabBackgroundChecked(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(item))
            {
                using (Region region = GetTabbedRegion(rc))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    Color cl1 = ColorTable.ButtonPressedGradientBegin;
                    Color cl2 = ColorTable.ButtonPressedGradientEnd;

                    PaintGradientSelected(g, rc, cl1, cl2);

                    PaintFlash(g, rc, false);
                    PaintTabBorder(g, rc, EBUTTONSTATE.Checked);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintTabBackgroundSelected(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rcTab = rc;
                rcTab.Height -= 1;

                using (Region region = GetTabbedRegion(rcTab))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;


                    Color cl1 = ColorTable.ButtonPressedGradientBegin;
                    Color cl2 = ColorTable.ButtonPressedGradientEnd;

                    PaintGradientSelected(g, rcTab, cl1, cl2);
                    PaintTabBorder(g, rcTab, EBUTTONSTATE.Selected);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (IsGroupedToolStrip(e.ToolStrip))
                    {
                        e.Graphics.FillRectangle(Brushes.White, rc);
                    }
                    else
                    {
                        Color cl1 = this.OfficeColorTable.CollapsedToolstripSelectedGradientBegin;
                        Color cl2 = this.OfficeColorTable.CollapsedToolstripSelectedGradientEnd;

                        PaintGradientCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), cl1, cl2);
                        PaintButtonBorder(e.Graphics, rc, EBUTTONSTATE.Collapsed);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (IsGroupedToolStrip(e.ToolStrip))
                    {
                        using (Brush brushGroupedPressed = new SolidBrush(OfficeColorTable.PanelBackground))
                        {
                            e.Graphics.FillRectangle(brushGroupedPressed, rc);
                        }
                    }
                    else
                    {
                        Color cl1 = this.OfficeColorTable.CollapsedToolstripPressedGradientBegin;
                        Color cl2 = this.OfficeColorTable.CollapsedToolstripPressedGradientEnd;

                        PaintGradientCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), cl1, cl2);
                        PaintButtonBorder(e.Graphics, rc, EBUTTONSTATE.Collapsed);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = GetButtonRect(e.Item);
            if (rc.Width > 0 && rc.Height > 0)
            {
                if (IsGroupedToolStrip(e.ToolStrip))
                {
                    e.Graphics.FillRectangle(Brushes.White, rc);
                }
                else
                {
                    Color cl1 = this.OfficeColorTable.CollapsedToolstripGradientBegin;
                    Color cl2 = this.OfficeColorTable.CollapsedToolstripGradientEnd;

                    PaintGradientCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), cl1, cl2);
                    PaintButtonBorder(e.Graphics, rc, EBUTTONSTATE.Collapsed);
                }
            }

            return true;
        }

        private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            return PaintButtonBackground(e);
        }

        private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;
            ToolStripSplitButton tsButton = e.Item as ToolStripSplitButton;

            if (tsButton != null)
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);
                    if (rcBackground.Width > 0 && rcBackground.Height > 0)
                    {
                        Graphics g = e.Graphics;
                        ToolStrip ts = e.ToolStrip;

                        EBUTTONSTATE ebState = EBUTTONSTATE.Normal;
                        bool bIsDisabled = ToolStripRendererUtils.GetIsDisabled(e.Item);

                        bool bGrouped = GetIsGrouped(e.Item);
                        bool bDropDownPressed = tsButton.DropDown.Visible && (tsButton.DropDown.OwnerItem == tsButton) && !bIsDisabled;
                        bool bSelected = (tsButton.Selected || bDropDownPressed) && !bIsDisabled;

                        Rectangle rcDropDown = Rectangle.Intersect(tsButton.DropDownButtonBounds, rcBackground);
                        Rectangle rcButton = rcBackground;

                        rcButton.Width -= rcDropDown.Width;
                        if (e.Item.RightToLeft == RightToLeft.Yes)
                        {
                            rcButton.X += rcDropDown.Width;
                        }

                        if (rcButton.Width > 0 && rcButton.Height > 0)
                        {
                            if (tsButton.ButtonPressed && !bIsDisabled)
                            {
                                PaintGradientSelected(g, rcButton, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd);
                                PaintButtonPressedShadow(g, rcButton);

                                g.DrawImage(this.PressedFlashImage, rcButton.X, rcButton.Y + rcButton.Height / 2, rcButton.Width, rcButton.Height);

                                g.DrawLine(this.ButtonPressedBorder, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1);
                                ebState = EBUTTONSTATE.Pressed;
                            }
                            else if (bSelected)
                            {
                                PaintGradientSelected(g, rcButton, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd);

                                g.DrawImage(this.SelectedFlashImage, rcButton.X, rcButton.Y + rcButton.Height / 2, rcButton.Width, rcButton.Height);

                                g.DrawLine(this.ButtonSelectedBorder, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1);
                                ebState = EBUTTONSTATE.Selected;
                            }
                            else if (bGrouped)
                            {
                                PaintGradientGrouped(g, rcButton);
                            }
                        }

                        if (rcDropDown.Width > 0 && rcDropDown.Height > 0)
                        {
                            GraphicsState gState = g.Save();
                            g.SetClip(rcDropDown);

                            if (bDropDownPressed)
                            {
                                PaintGradientSelected(g, rcDropDown, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd);
                                PaintButtonPressedShadow(g, rcDropDown);

                                PaintFlashChecked(g, rcDropDown);
                            }
                            else if (tsButton.DropDownButtonSelected && !bIsDisabled)
                            {
                                PaintGradientSelected(g, rcDropDown, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd);
                                g.DrawImage(this.SelectedFlashImage, rcDropDown.X, rcDropDown.Y + rcDropDown.Height / 2, rcDropDown.Width, rcDropDown.Height);

                                g.DrawLine(this.ButtonHighlightBorder, rcDropDown.X, rcDropDown.Y, rcDropDown.X, rcDropDown.Bottom - 1);
                            }
                            else if (bGrouped)
                            {
                                PaintGradientGrouped(g, rcDropDown);
                            }

                            Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                            Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                            bool bVertical = ts != null && (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right);
                            ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                            base.DrawArrow(new ToolStripArrowRenderEventArgs(g, e.Item, rcArrow, color, dir));

                            g.Restore(gState);
                        }

                        if (!bGrouped)
                        {
                            PaintButtonBorder(e.Graphics, rc, ebState);
                        }
                    }
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void PaintSplitButtonExBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripSplitButtonEx tsButton = e.Item as ToolStripSplitButtonEx;
            bool isFormFocused = ((e.ToolStrip.TopLevelControl is Form) && (e.ToolStrip.TopLevelControl.ContainsFocus));
            bool isTopLevelControlNotForm = (!(e.ToolStrip.TopLevelControl is Form));
            if (tsButton != null)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    if (rcBackground.Width > 0 && rcBackground.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        bool bIsDisabled = ToolStripRendererUtils.GetIsDisabled(e.Item);
                        bool bDropDownPressed = tsButton.DropDown.Visible && (tsButton.DropDown.OwnerItem == tsButton) && !bIsDisabled;
                        bool bSelected = tsButton.Selected && !bIsDisabled;

                        Rectangle rcButton = Rectangle.Intersect(tsButton.ImageBounds, rcBackground);
                        Rectangle rcDropDown = Rectangle.Intersect(tsButton.ButtonBounds, rcBackground);

                        Color clImageBegin, clImageEnd, clButtonBegin, clButtonEnd;
                        EBUTTONSTATE eImageState, eButtonState;
                        Rectangle rcImageBorder, rcButtonBorder;
                        Bitmap bmpImage, bmpButton;
                        Pen penImage;
                        Pen penButton;
                        bool bPaint = true;

                        // We need to initialize variables for further use.
                        // So initialize them as ImageSelectedButtonSelectedInActive state.
                        rcImageBorder = rcButtonBorder = Rectangle.Empty;
                        clImageBegin = ColorTable.ButtonSelectedGradientBegin;
                        clImageEnd = ColorTable.ButtonSelectedGradientEnd;
                        clButtonBegin = OfficeColorTable.SelectedButtonInActiveBegin;
                        clButtonEnd = OfficeColorTable.SelectedButtonInActiveEnd;

                        eButtonState = eImageState = EBUTTONSTATE.Selected;

                        bmpButton = bmpImage = this.SelectedFlashImage;
                        penImage = this.ButtonSelectedBorder;
                        penButton = this.ButtonHighlightBorder;

                        switch (tsButton.ButtonState)
                        {
                            case ToolStripSplitButtonEx.SplitButtonState.ImageSelected:
                                if (!isFormFocused)
                                {
                                    bPaint = false;
                                }
                                if (isTopLevelControlNotForm)
                                {
                                    bPaint = true;
                                }
                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ButtonSelected:

                                clImageBegin = OfficeColorTable.SelectedButtonInActiveBegin;
                                clImageEnd = OfficeColorTable.SelectedButtonInActiveEnd;
                                clButtonBegin = ColorTable.ButtonSelectedGradientBegin;
                                clButtonEnd = ColorTable.ButtonSelectedGradientEnd;

                                eButtonState = eImageState = EBUTTONSTATE.Selected;

                                bmpButton = bmpImage = this.SelectedFlashImage;
                                penImage = this.ButtonSelectedBorder;
                                penButton = this.ButtonHighlightBorder;

                                if (!isFormFocused)
                                {
                                    bPaint = false;
                                }
                                if (isTopLevelControlNotForm)
                                {
                                    bPaint = true;
                                }
                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ImagePressed:

                                clImageBegin = ColorTable.ButtonPressedGradientBegin;
                                clImageEnd = ColorTable.ButtonPressedGradientEnd;
                                clButtonBegin = OfficeColorTable.SelectedButtonInActiveBegin;
                                clButtonEnd = OfficeColorTable.SelectedButtonInActiveEnd;

                                eImageState = EBUTTONSTATE.Pressed;
                                eButtonState = EBUTTONSTATE.Selected;

                                bmpImage = this.PressedFlashImage;
                                bmpButton = this.SelectedFlashImage;

                                penImage = this.ButtonPressedBorder;
                                penButton = this.ButtonHighlightBorder;

                                rcImageBorder = tsButton.ImageBounds;
                                rcButtonBorder = tsButton.ButtonBounds;

                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ButtonPressed:

                                clImageBegin = OfficeColorTable.SelectedButtonInActiveBegin;
                                clImageEnd = OfficeColorTable.SelectedButtonInActiveEnd;
                                clButtonBegin = ColorTable.ButtonPressedGradientBegin;
                                clButtonEnd = ColorTable.ButtonPressedGradientEnd;

                                eImageState = EBUTTONSTATE.Selected;
                                eButtonState = EBUTTONSTATE.Pressed;

                                bmpImage = this.SelectedFlashImage;
                                bmpButton = this.PressedFlashImage;

                                penImage = this.ButtonHighlightBorder;
                                penButton = this.ButtonPressedBorder;

                                rcImageBorder = tsButton.ImageBounds;
                                rcButtonBorder = tsButton.ButtonBounds;

                                break;

                            default:

                                // SplitButtonState is None, so don't paint the item.
                                bPaint = false;

                                break;
                        }


                        if (bPaint)
                        {
                            bool bImage = (tsButton.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image;
                            bool bDrawButton = (rcButton.Width > 0 && rcButton.Height > 0);
                            bool bDrawDropDown = (rcDropDown.Width > 0 && rcDropDown.Height > 0);

                            // Draw Gradient background, shadow, flash images and border
                            // for Image button and Split button on SplitButtonEx.
                            if (bDrawButton)
                            {
                                PaintGradientSelected(g, rcButton, clImageBegin, clImageEnd);

                                if (eImageState == EBUTTONSTATE.Pressed)
                                {
                                    PaintButtonPressedShadow(g, rcButton);
                                }

                                g.DrawImage(bmpImage, rcButton.X, rcButton.Y + rcButton.Height / 2, rcButton.Width, rcButton.Height);
                            }

                            if (bDrawDropDown)
                            {
                                PaintGradientSelected(g, rcDropDown, clButtonBegin, clButtonEnd);

                                if (eButtonState == EBUTTONSTATE.Pressed)
                                {
                                    PaintButtonPressedShadow(g, rcDropDown);
                                }

                                g.DrawImage(bmpButton, rcDropDown.X, rcDropDown.Y + rcDropDown.Height / 2, rcDropDown.Width, rcDropDown.Height);
                            }

                            if (bImage && bDrawButton && bDrawDropDown)
                            {
                                if (tsButton.RightToLeft == RightToLeft.Yes)
                                {
                                    g.DrawLine(penImage, rcButton.Left, rcButton.Y, rcButton.Left, rcButton.Bottom - 1);
                                    g.DrawLine(penButton, rcDropDown.Right - 1, rcDropDown.Y, rcDropDown.Right - 1, rcDropDown.Bottom - 1);
                                }
                                else
                                {
                                    g.DrawLine(penImage, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1);
                                    g.DrawLine(penButton, rcDropDown.X, rcDropDown.Y, rcDropDown.X, rcDropDown.Bottom - 1);
                                }
                            }

                            if (eImageState == EBUTTONSTATE.Pressed || eButtonState == EBUTTONSTATE.Pressed)
                            {
                                if (bImage)
                                {
                                    PaintButtonBorder(g, rcImageBorder, eImageState);
                                    PaintButtonBorder(g, rcButtonBorder, eButtonState);
                                }
                                else
                                {
                                    PaintButtonBorder(g, rc, EBUTTONSTATE.Pressed);
                                }
                            }
                            else
                            {
                                PaintButtonBorder(g, rc, EBUTTONSTATE.Selected);
                            }
                        }

                        Image image = tsButton.Image;

                        // Draw image.
                        if ((tsButton.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                        {
                            if (image != null)
                            {
                                bool bDisposeImage = false;

                                if (bIsDisabled)
                                {
                                    image = CreateDisabledImage(image);
                                    bDisposeImage = true;
                                }

                                g.DrawImage(image, tsButton.InternalLayout.ImageRectangle);

                                if (bDisposeImage)
                                {
                                    image.Dispose();
                                }
                            }
                        }

                        Color clForeColor = bIsDisabled ? SystemColors.GrayText : tsButton.ForeColor;

                        // Draw text.
                        if ((tsButton.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                        {
                            TextRenderer.DrawText(g, tsButton.Text, tsButton.Font, tsButton.InternalLayout.TextRectangle, clForeColor,ToolStripRendererUtils.GetTextFormatFlags(tsButton.TextAlign));
                        }

                        // Draw arrow.
                        Rectangle arrowRectangle = tsButton.InternalLayout.DropDownButtonRectangle;

                        using (Brush brush = new SolidBrush(clForeColor))
                        {
                            Point pt = new Point(arrowRectangle.Left + (arrowRectangle.Width / 2), arrowRectangle.Top + (arrowRectangle.Height / 2));
                            Point[] points = new Point[] { new Point(pt.X - 2, pt.Y - 1), new Point(pt.X + 3, pt.Y - 1), new Point(pt.X, pt.Y + 2) };

                            g.FillPolygon(brush, points);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Indicates if a ToolStrip locates on grouped RibbonPanel.
        /// </summary>
        /// <param name="pToolStrip"> ToolStrip instance. </param>
        /// <returns> True - ToolStrip locates on grouped RibbonPanel,
        /// otherwise - False.</returns>
        private bool IsGroupedToolStrip(ToolStrip ts, ref Color pColor)
        {
            bool bResult = false;

            if (ts != null)
            {
                RibbonPanel panel = GetParentPanel(ts);

                if (panel != null)
                {
                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

                    if (tsTabGroup != null)
                    {
                        pColor = tsTabGroup.Color;
                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// Indicates if a ToolStrip locates on grouped RibbonPanel.
        /// </summary>
        /// <param name="pToolStrip"> ToolStrip instance. </param>
        /// <returns> True - ToolStrip locates on grouped RibbonPanel,
        /// otherwise - False.</returns>
        private bool IsGroupedToolStrip(ToolStrip ts)
        {
            bool bResult = false;

            if (ts != null)
            {
                RibbonPanel panel = GetParentPanel(ts);

                if (panel != null)
                {
                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();
                    bResult = (tsTabGroup != null);
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private RibbonPanel GetParentPanel(ToolStrip ts)
        {
            Control parent = ts.Parent;

            ToolStripDropDown dropDown = parent as ToolStripDropDown;
            if (dropDown != null)
            {
                CollapsedDropDownButton item = dropDown.OwnerItem as CollapsedDropDownButton;
                if (item != null)
                {
                    ToolStrip owner = item.GetCurrentParent();
                    if (owner != null)
                    {
                        parent = owner.Parent;
                    }
                }
            }
            return parent as RibbonPanel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

            if (item != null)
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        Image imgArrow = ArrowOverflow;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);
                    }
                }
            }
            else
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
                        Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);
                    }
                }
            }

            return true;
        }

        protected void PaintGradientSelected(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color clMedium = Office12ColorTable.GetAlphaBlendedColor(clBegin, clEnd, 128);

                int topHeight = (int)(rc.Height * 0.38);
                int bottomHeight = rc.Height - topHeight;

                if (topHeight > 0)
                {
                    Rectangle rcTop = new Rectangle(rc.X, rc.Y, rc.Width, topHeight + 1);

                    using (LinearGradientBrush brush = GetVerticalBrush(ref rcTop, clBegin, clMedium))
                    {
                        brush.WrapMode = WrapMode.TileFlipY;
                        g.FillRectangle(brush, rcTop);
                    }
                }

                Rectangle rcBottom = new Rectangle(rc.X, rc.Y + topHeight, rc.Width, bottomHeight);

                using (LinearGradientBrush brush = GetVerticalBrush(ref rcBottom, clEnd, clMedium))
                {
                    brush.WrapMode = WrapMode.TileFlipY;
                    g.FillRectangle(brush, rcBottom);
                }
            }
        }

        private void PaintGradientCollapsed(Graphics g, Rectangle rc, Color clBegin, Color clEnd)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonCollapsed;

                    brush.WrapMode = WrapMode.TileFlipXY;
                    g.FillRectangle(brush, rc);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void PaintGradientGrouped(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = this.GroupGradientBegin;
                Color cl2 = this.GroupGradientEnd;

                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, cl2, cl1))
                {
                    brush.Blend = m_blGrouped;

                    brush.WrapMode = WrapMode.TileFlipXY;
                    g.FillRectangle(brush, rc);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintGroupBorders(ToolStripRenderEventArgs e)
        {
            ToolStrip ts = e.ToolStrip;
            if (!ts.IsDropDown && ERENDERTYPE.Grouped == GetRenderType(ts))
            {
                Graphics g = e.Graphics;

                ToolStripItemCollection tsItems = ts.Items;
                ToolStripItemPlacement tsPlacement = ToolStripItemPlacement.Main;

                if (tsItems != null)
                {
                    int nItems = tsItems.Count;

                    Rectangle[] rects = new Rectangle[2];

                    for (int i = 0; i < nItems; i++)
                    {
                        ToolStripItem item = tsItems[i];
                        if (item.Placement == tsPlacement)
                        {
                            int idx = item.Alignment == ToolStripItemAlignment.Left ? 0 : 1;
                            Rectangle rc = rects[idx];

                            if (item is ToolStripButton || item is ToolStripDropDownButton || item is ToolStripSplitButton)
                            {
                                int x1, x2, y1, y2;

                                Rectangle rcItem = GetButtonBounds(item);

                                if (!rc.IsEmpty)
                                {
                                    if (rc.Y == rcItem.Y)
                                    {
                                        x1 = rcItem.X > rc.X ? rc.Right - 1 : rc.Left - 1;
                                        x2 = x1 + 1;
                                        y1 = rc.Y + 1;
                                        y2 = rc.Bottom - 2;

                                        g.DrawLine(this.GroupBorder, x1, y1, x1, y2);
                                        g.DrawLine(this.ButtonHighlightGrouped, x2, y1, x2, y2);

                                        rects[idx] = Rectangle.Union(rc, rcItem);
                                    }
                                    else
                                    {
                                        PaintGroupBorder(g, rc);
                                        rects[idx] = rcItem;
                                    }
                                }
                                else
                                {
                                    rects[idx] = rcItem;
                                }
                            }
                            else
                            {
                                PaintGroupBorder(g, rc);
                                rects[idx] = Rectangle.Empty;
                            }
                        }
                    }
                    PaintGroupBorder(g, rects[0]);
                    PaintGroupBorder(g, rects[1]);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void PaintGroupBorder(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                g.DrawPolygon(this.GroupBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));

                int x1 = rc.Left + 1;
                int y1 = rc.Top + 1;
                int x2 = rc.Right - 2;
                int y2 = rc.Bottom - 2;

                g.DrawLine(this.ButtonHighlightGrouped, x1, y1, x1, y2);
                g.DrawLine(this.ButtonHighlightGrouped, x2, y1, x2, y2);
            }
        }

        protected void PaintButtonBorder(Graphics g, Rectangle rc, EBUTTONSTATE state)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                if (state != EBUTTONSTATE.Normal)
                {
                    GraphicsState gState = g.Save();
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    switch (state)
                    {
                        case EBUTTONSTATE.Selected:
                            {
                                g.DrawPolygon(this.ButtonSelectedBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));
                                g.DrawRectangle(this.ButtonHighlightBorder, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                            }
                            break;
                        case EBUTTONSTATE.Pressed:
                        case EBUTTONSTATE.Checked:
                            {
                                g.DrawPolygon(this.ButtonPressedBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));
                            }
                            break;
                        case EBUTTONSTATE.Collapsed:
                            {
                                g.DrawPolygon(this.CollapsedBorderPen, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));
                                g.DrawRectangle(this.ButtonHighlightBorder, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                            }
                            break;

                    }
                    g.Restore(gState);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <param name="clHighlightBorder"></param>
        private void PaintButtonPressedBorder(Graphics g, ref Rectangle rc, Color clHighlightBorder)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color clBorderBegin = ColorTable.ButtonPressedBorder;
                Color clBorderEnd = Office12ColorTable.GetAlphaBlendedColor(clBorderBegin, Color.White, 128);

                using (LinearGradientBrush brush = GetVerticalBrush(rc.Top, rc.Height, clBorderBegin, clBorderEnd))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point(rc.Left, rc.Bottom-2),
								new Point(rc.Left, rc.Top+1),
								new Point(rc.Left+1, rc.Top),
								new Point(rc.Right-2, rc.Top),
								new Point(rc.Right-1, rc.Top+1),
								new Point(rc.Right-1, rc.Bottom-2),
							};
                        g.DrawLines(pen, points);
                    }
                }

                Color clHighlightBegin = Color.FromArgb(32, clHighlightBorder);
                Color clHighlightEnd = clHighlightBorder;

                using (LinearGradientBrush brush = GetVerticalBrush(rc.Top + 1, rc.Height - 1, clHighlightBegin, clHighlightEnd))
                {
                    using (Pen pen = new Pen(brush))
                    {
                        Point[] points = new Point[]
							{
								new Point(rc.Right-2, rc.Y+2),
								new Point(rc.Right-2, rc.Bottom-1),
								new Point(rc.Left + 1, rc.Bottom-1),
								new Point(rc.Left + 1, rc.Y+2),
							};
                        g.DrawLines(pen, points);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        protected void PaintButtonPressedShadow(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color clBegin = Color.FromArgb(160, ColorTable.ButtonPressedBorder);
                Color clEnd = Color.Transparent;

                rc.Height = Math.Max(rc.Height / 10, 2);

                using (LinearGradientBrush brush = GetVerticalBrush(ref rc, clBegin, clEnd))
                {
                    brush.Blend = m_blButtonShadow;
                    g.FillRectangle(brush, rc);
                }
            }
        }

        private void PaintTabBorder(Graphics g, Rectangle rc, EBUTTONSTATE eState)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Pen pen = eState == EBUTTONSTATE.Selected ? this.ButtonSelectedGradientEnd : this.ButtonPressedGradientEnd;
                g.DrawLines(pen, GetTabbedPath(rc));
            }
        }

        private void PaintFlash(Graphics g, Rectangle rc, bool bCenter)
        {
            GraphicsState gState = g.Save();
            g.SetClip(rc, CombineMode.Intersect);

            Rectangle rcFlash = rc;

            if (!bCenter)
            {
                rcFlash.Offset(0, -rcFlash.Height / 2);
                g.DrawImage(SelectedFlashImage, rcFlash);
                rcFlash.Offset(0, rcFlash.Height);
            }
            g.DrawImage(SelectedFlashImage, rcFlash);

            g.Restore(gState);
        }

        private void PaintFlashChecked(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                GraphicsState gState = g.Save();
                g.SetClip(rc);

                Rectangle rcFlash = new Rectangle(rc.X, rc.Y + rc.Height / 2, rc.Width, rc.Height);
                rcFlash.Inflate(rcFlash.Width / 2, rcFlash.Height / 4);

                g.DrawImage(this.CheckedFlashImage, rcFlash);

                g.Restore(gState);
            }
        }


        private Region GetTabbedRegion(Rectangle rc)
        {
            Region rgnResult = new Region(rc);

            int y = rc.Y;
            int x = rc.X;
            int h = rc.Height;

            rgnResult.Exclude(new Rectangle(x, y, 1, h - 1));
            rgnResult.Exclude(new Rectangle(x + 1, y, 1, h - 2));
            rgnResult.Exclude(new Rectangle(x + 2, y, 1, 2));
            rgnResult.Exclude(new Rectangle(x + 3, y, 1, 1));

            x = rc.Right - 4;

            rgnResult.Exclude(new Rectangle(x, y, 1, 1));
            rgnResult.Exclude(new Rectangle(x + 1, y, 1, 2));
            rgnResult.Exclude(new Rectangle(x + 2, y, 1, h - 2));
            rgnResult.Exclude(new Rectangle(x + 3, y, 1, h - 1));

            return rgnResult;
        }

        private Point[] GetTabbedPath(Rectangle rc)
        {
            Point[] points = new Point[]
				{
					new Point(rc.X, rc.Bottom-1),
					new Point(rc.X+2, rc.Bottom-3),
					new Point(rc.X+2, rc.Y+2),
					new Point(rc.X+4, rc.Y),
					new Point(rc.Right-5, rc.Y),
					new Point(rc.Right-3, rc.Y+2),
					new Point(rc.Right-3, rc.Bottom-3),
					new Point(rc.Right-1, rc.Bottom-1),
				};

            return points;
        }

        private Point[] GetLauncherPolygon(Rectangle rc)
        {
            Point[] points = new Point[]
                {
                    new Point(rc.X, rc.Y+1),
                    new Point(rc.X, rc.Y),
                    new Point(rc.Right-1, rc.Y),
                    new Point(rc.Right-1, rc.Bottom-1),
                    new Point(rc.Right-2, rc.Bottom-1),
                };

            return points;
        }

        private Rectangle GetItemRect(ToolStripItem item)
        {
            Rectangle rc = new Rectangle(Point.Empty, item.Size);

            if (item.Owner is ToolStripDropDownMenu)
            {
                rc.X += MENUITEM_PADDING_LEFT;
                rc.Width -= MENUITEM_HORIZONTAL;
            }

            return rc;
        }

        protected Rectangle GetButtonRect(ToolStripItem item)
        {
            return GetButtonRect(item, new Rectangle(Point.Empty, item.Size), this.RenderType);
        }
        protected Rectangle DPI_125_GetButtonRect(ToolStripItem item)
        {
            return GetButtonRect(item, new Rectangle(Point.Empty, new Size(item.Size.Width + 2 , item.Size.Height + 2)), this.RenderType);
        }
        private Rectangle GetButtonBounds(ToolStripItem item)
        {
            return GetButtonRect(item, item.Bounds, this.RenderType);
        }

        private Rectangle GetButtonBounds(ToolStripItem item, ERENDERTYPE erType)
        {
            return GetButtonRect(item, item.Bounds, erType);
        }

        private Rectangle GetButtonRect(ToolStripItem item, Rectangle rc)
        {
            return GetButtonRect(item, rc, this.RenderType);
        }

        public static Rectangle GetButtonRect(ToolStripItem item, Rectangle rc, ERENDERTYPE erType)
        {
            Rectangle rcResult = rc;

            if (rcResult.Width > 0 && rcResult.Height > 0)
            {
                if (item is ToolStripOverflowButton)
                {
                    Control parent = item.GetCurrentParent();
                    if (parent != null)
                    {
                        rcResult.Intersect(parent.ClientRectangle);
                    }
                    rcResult.Inflate(-1, -1);
                }
                else
                {
                    if (item.IsOnDropDown)
                    {
                        rcResult.Inflate(-1, 0);
                    }
                    else if (item.Owner is StatusStrip)
                    {
                        //rcResult.Height -= 2;
                    }
                }
            }

            return rcResult;
        }

        protected Rectangle GetButtonBackgroundRect(ToolStripItem item, Rectangle rc)
        {
            Rectangle rcResult = rc;

            if (!(item is ToolStripOverflowButton))
            {
                rcResult.Inflate(-1, -1);
            }

            return rcResult;
        }

        internal virtual Rectangle GetCheckRect(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(e.ImageRectangle.Left - IMAGE_PADDING, IMAGE_MARGIN,
                e.ImageRectangle.Width + 2 * IMAGE_PADDING, e.Item.Height - 2 * IMAGE_MARGIN);

            return rc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="cl1"></param>
        /// <param name="cl2"></param>
        /// <returns></returns>
        protected LinearGradientBrush GetVerticalBrush(ref Rectangle rc, Color cl1, Color cl2)
        {
            return GetVerticalBrush(rc.Top, rc.Height, cl1, cl2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <param name="height"></param>
        /// <param name="cl1"></param>
        /// <param name="cl2"></param>
        /// <returns></returns>
        protected LinearGradientBrush GetVerticalBrush(int top, int height, Color cl1, Color cl2)
        {
            Rectangle rcBrush = new Rectangle(0, top, 1, height);

            //No chance of width being 0 as it is hard coded to 1 - we will only check for height
            if (rcBrush.Height == 0)
            {
                Exception he = new Exception("GetVerticalBrush rcBrush height is 0. Adjusting height to 15.");
                Syncfusion.Diagnostics.TraceUtil.TraceExceptionCatched(he);
                ExceptionManager.RaiseExceptionCatched(this, he);
                rcBrush.Height = 15;
            }

            return new LinearGradientBrush(rcBrush, cl1, cl2, 90);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="cl1"></param>
        /// <param name="cl2"></param>
        /// <returns></returns>
        protected LinearGradientBrush GetHorizontalBrush(ref Rectangle rc, Color cl1, Color cl2)
        {
            Rectangle rcBrush = new Rectangle(rc.Left, rc.Top, rc.Width, 1);

            return new LinearGradientBrush(rcBrush, cl1, cl2, 0F);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static bool GetIsLButtonDown()
        {
            VirtualKeys vKey = WindowsAPI.GetSystemMetrics(SystemMetricsCodes.SM_SWAPBUTTON) != 0 ? VirtualKeys.VK_RBUTTON : VirtualKeys.VK_LBUTTON;
            return (WindowsAPI.GetAsyncKeyState((VirtualKeys)vKey) & 0x8000) != 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsItem"></param>
        /// <returns></returns>
        private bool GetIsGrouped(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (tsItem != null && !(tsItem is ToolStripOverflowButton) && !(tsItem is CollapsedDropDownButton) && !tsItem.IsOnDropDown)
            {
                bResult = ERENDERTYPE.Grouped == GetRenderType(tsItem.Owner);
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsItem"></param>
        /// <returns></returns>
        private bool GetIsTabItem(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (this.RenderType == ERENDERTYPE.TabBar && !tsItem.IsOnDropDown)
            {
                bResult = tsItem is IToolStripTabItem;

                if (!bResult)
                {
                    Type t = tsItem.GetType();
                    bResult = t == typeof(ToolStripButton) || t == typeof(ToolStripDropDownButton);
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        internal static bool GetIsSelected(ToolStrip ts)
        {
            CToolStripWindow wnd = m_htControls[ts] as CToolStripWindow;

            if (wnd != null)
            {
                return wnd.Selected;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private ERENDERTYPE GetRenderType(ToolStrip ts)
        {
            if (ts != null)
            {
                IToolStripExSupport iRibbon = ts as IToolStripExSupport;
                if (iRibbon != null && iRibbon.GroupedButtons)
                {
                    return ERENDERTYPE.Grouped;
                }
            }

            return this.RenderType;
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        private void Clear()
        {
            Clear(m_htPens);
            Clear(m_htBrushes);
            Clear(m_htBitmaps);
        }
        /// <summary>
        /// Clean up specified resources being used.
        /// </summary>
        /// <param name="hTable">Hastable of resources to dispose</param>
        private void Clear(Hashtable hTable)
        {
            foreach (Object obj in hTable)
            {
                IDisposable iDispose = obj as IDisposable;

                if (iDispose != null)
                {
                    iDispose.Dispose();
                }
            }
            hTable.Clear();
        }
        //Event handlers
        /// <summary>
        /// Handle changes of display settings and user preferences.
        /// </summary>
        protected void SettingsChangedHandler()
        {
            //Clear cache of used GDI objects
            Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ePen"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Pen GetKnownPen(EPEN ePen, Color color, int width)
        {
            Pen pen = m_htPens[ePen] as Pen;
            if (pen == null)
            {
                pen = new Pen(color, width);
                m_htPens[ePen] = pen;
            }
            return pen;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eBrush"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Brush GetKnownBrush(EBRUSH eBrush, Color color)
        {
            Brush brush = m_htBrushes[eBrush] as Brush;
            if (brush == null)
            {
                brush = new SolidBrush(color);
                m_htBrushes[eBrush] = brush;
            }
            return brush;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorBase"></param>
        /// <returns></returns>
        protected Bitmap GetFlashImage(Color colorBase, Size size)
        {
            Rectangle rcFlash = new Rectangle(0, 0, size.Width, size.Height);
            Bitmap bitmap = new Bitmap(rcFlash.Width, rcFlash.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(rcFlash);

                using (PathGradientBrush brFlash = new PathGradientBrush(path))
                {
                    brFlash.Blend = m_blButtonFlash;

                    brFlash.CenterColor = colorBase;
                    brFlash.SurroundColors = new Color[] { Color.Transparent };

                    g.FillRectangle(brFlash, rcFlash);
                }
                path.Dispose();
            }
            return bitmap;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Size GetDownArrowSize()
        {
            return this.ArrowDownImage.Size;
        }
        /// Gets background brush for collapsed ToolStripEx.
        /// </summary>
        /// <param name="clBegin"> Starting color for a gradient. </param>
        /// <param name="clEnd"> Final color for a gradient. </param>
        /// <param name="nHeight"> Brush's height for a gradient. </param>
        /// <returns> LinearGradientBrush for furhter painting. </returns>
        private LinearGradientBrush GetGroupedBackgroundCollapsedBrush(Color clBegin, Color clEnd, int nHeight)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);
            brush.Blend = m_blGroupedBackGroundCollapsed;

            return brush;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private LinearGradientBrush GetGroupedBackgroundBrush(Color clBegin, Color clEnd, ref Rectangle rc)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(rc.X, rc.Y, 1, rc.Height), clBegin, clEnd, LinearGradientMode.Vertical);

            brush.Blend = m_blGroupedBackGround;
            brush.WrapMode = WrapMode.TileFlipXY;

            return brush;
        }
        #endregion

        #region Properties
        public ERENDERTYPE RenderType
        {
            get
            {
                return m_eRenderType;
            }
            set
            {
                m_eRenderType = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Office12ColorTable OfficeColorTable
        {
            get
            {
                return this.ColorTable as Office12ColorTable;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionText
        {
            get
            {
                return this.OfficeColorTable.CaptionText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionGradientBegin
        {
            get
            {
                return this.OfficeColorTable.CaptionGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionGradientEnd
        {
            get
            {
                return this.OfficeColorTable.CaptionGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionHighlightGradientBegin
        {
            get
            {
                return this.OfficeColorTable.CaptionHighlightGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionHighlightGradientEnd
        {
            get
            {
                return this.OfficeColorTable.CaptionHighlightGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color ToolStripHighlightGradientBegin
        {
            get
            {
                return this.OfficeColorTable.ToolStripHighlightGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color ToolStripHighlightGradientEnd
        {
            get
            {
                return this.OfficeColorTable.ToolStripHighlightGradientEnd;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherBackground
        {
            get
            {
                return this.OfficeColorTable.LauncherBackground;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherText
        {
            get
            {
                return this.OfficeColorTable.LauncherText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherTextSelected
        {
            get
            {
                return this.OfficeColorTable.LauncherTextSelected;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color GroupGradientBegin
        {
            get
            {
                return this.OfficeColorTable.GroupGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color GroupGradientEnd
        {
            get
            {
                return this.OfficeColorTable.GroupGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color BorderColor
        {
            get
            {
                Office12ColorTable colorTable = this.ColorTable as Office12ColorTable;

                if (colorTable != null)
                {
                    return colorTable.ToolStripBorder;
                }

                return SystemColors.ControlDark;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ToolStripBorder
        {
            get { return GetKnownPen(EPEN.epToolStripBorder, ColorTable.ToolStripBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuBorder
        {
            get { return GetKnownPen(EPEN.epMenuBorder, ColorTable.MenuBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuItemBorder
        {
            get { return GetKnownPen(EPEN.epMenuItemBorder, ColorTable.MenuItemBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuStripBorder
        {
            get
            {
                Office12ColorTable colorTable = ColorTable as Office12ColorTable;
                Color color = colorTable != null ? colorTable.MenuStripGradientMiddle : ColorTable.MenuBorder;

                return GetKnownPen(EPEN.epMenuStripBorder, color, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuItemPressedGradientEnd
        {
            get { return GetKnownPen(EPEN.epMenuItemPressedGradientEnd, ColorTable.MenuItemPressedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedGradientBegin
        {
            get { return GetKnownPen(EPEN.epButtonPressedGradientBegin, ColorTable.ButtonPressedGradientBegin, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedGradientEnd
        {
            get { return GetKnownPen(EPEN.epButtonPressedGradientEnd, ColorTable.ButtonPressedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonSelectedGradientEnd
        {
            get { return GetKnownPen(EPEN.epButtonSelectedGradientEnd, ColorTable.ButtonSelectedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonSelectedBorder
        {
            get
            {
                Color cl = Color.FromArgb(128, ColorTable.ButtonSelectedBorder);
                return GetKnownPen(EPEN.epButtonSelectedBorder, cl, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedBorder
        {
            get { return GetKnownPen(EPEN.epButtonPressedBorder, ColorTable.ButtonPressedBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen CollapsedBorderPen
        {
            get
            {
                return GetKnownPen(EPEN.epCollapsedBorder, Color.FromArgb(16, 0, 0, 0), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ImageBorderPen
        {
            get
            {
                return GetKnownPen(EPEN.epImageBorder, Color.FromArgb(32, 0, 0, 0), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonHighlightBorder
        {
            get
            {
                Color color = Office12ColorTable.GetAlphaBlendedColor(Color.Transparent, Color.White, 128);
                return GetKnownPen(EPEN.epButtonHighlightBorder, color, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonHighlightGrouped
        {
            get
            {
                return GetKnownPen(EPEN.epHighlightGrouped, Office12ColorTable.GetAlphaBlendedColor(Color.White, this.OfficeColorTable.GroupBorder, 160), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen GroupBorder
        {
            get
            {
                return GetKnownPen(EPEN.epGroupBorder, this.OfficeColorTable.GroupBorder, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen LauncherBorder
        {
            get
            {
                return GetKnownPen(EPEN.epLauncherBorder, this.OfficeColorTable.LauncherBorder, 2);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen CheckBorder
        {
            get
            {
                return GetKnownPen(EPEN.epCheckBorder, this.OfficeColorTable.CheckBorder, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush Shadow
        {
            get { return GetKnownBrush(EBRUSH.ebShadow, Color.FromArgb(128, Color.Black)); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush ToolStripDropDownBackground
        {
            get { return GetKnownBrush(EBRUSH.ebToolStripDropDownBackground, ColorTable.ToolStripDropDownBackground); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush CheckBackground
        {
            get
            {
                return GetKnownBrush(EBRUSH.ebCheckBackground, ColorTable.CheckBackground);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap ArrowDownImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowDown] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT, ARROW_HEIGHT + 1);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        if (g.DpiX > 96)
                        {
                            bitmap = new Bitmap(15, 15);
                            using ( Graphics graphics = Graphics.FromImage(bitmap))
                                {
                                graphics.DrawLine(Pens.Black , new Point(3,3) , new Point(10,3));
                                graphics.DrawLine(Pens.White , new Point(3, 4), new Point(10, 4));
                                for (int i = 0; i <= 5; i++)
                                {
                                    graphics.DrawLine(Pens.White, new Point(2 + i, 6 + i), new Point(11 - i, 6 + i));
                                }
                                for (int i = 0; i <= 4; i++)
                                {
                                    graphics.DrawLine(Pens.Black, new Point(3 + i, 6 + i), new Point(10 - i, 6 + i));
                                }
                            }
                            return bitmap;
                        }
                        else
                        {
                            g.Clear(Color.Transparent);
                        Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH, 1);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(0, 3);
                            while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(-1, 0);
                                rc.Y += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(0, 1);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                                }
                            }
                        }
                    }

                    m_htBitmaps[EBITMAP.ebArrowDown] = bitmap;
                }
                return bitmap;
            }
        }
        protected Bitmap ArrowOverflow
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowOverflow] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        // First arrow.
                        Rectangle rc = new Rectangle(0, 2, 1, 3);

                        using (Region rgDark = new Region(rc))
                        {
                            rgDark.Union(rc);

                            rc.Inflate(0, -1);
                            rc.X += 1;
                            rgDark.Union(rc);

                            g.FillRegion(SystemBrushes.ControlText, rgDark);
                        }

                        // Second arrow.
                        rc = new Rectangle(4, 2, 1, 3);
                        using (Region rgDark = new Region(rc))
                        {
                            rgDark.Union(rc);

                            rc.Inflate(0, -1);
                            rc.X += 1;
                            rgDark.Union(rc);

                            g.FillRegion(SystemBrushes.ControlText, rgDark);
                        }

                        // Highlight for first arrow.
                        rc = new Rectangle(0, 1, 1, 1);
                        using (Region rgLight = new Region(rc))
                        {
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);

                            g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                        }

                        // Highlight for second arrow.
                        rc = new Rectangle(4, 1, 1, 1);
                        using (Region rgLight = new Region(rc))
                        {
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);

                            g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                        }
                    }

                    m_htBitmaps[EBITMAP.ebArrowOverflow] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap ArrowRightImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowRight] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(3, 0);
                            while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(1, 0);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                    m_htBitmaps[EBITMAP.ebArrowRight] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap SelectedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebSelectedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonSelectedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebSelectedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap PressedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebPressedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonPressedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebPressedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap CheckedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebCheckedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonCheckedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebCheckedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap CheckButton
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebCheckButton] as Bitmap;
                if (bitmap == null)
                {
                    Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
                    bitmap = new Bitmap(rcFlash.Width, rcFlash.Height);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Point[] points = new Point[]
						{
							new Point(1,8), 
							new Point(3,12),
							new Point(8,1)
						};

                        using(GraphicsPath path = new GraphicsPath())
                        path.AddLines(points);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(Pens.MidnightBlue, points);
                    }
                    m_htBitmaps[EBITMAP.ebCheckButton] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static StringFormat CaptionFormat
        {
            get
            {
                if (m_sfCaption == null)
                {
                    m_sfCaption = new StringFormat();
                    m_sfCaption.HotkeyPrefix = HotkeyPrefix.Hide;
                    m_sfCaption.Alignment = StringAlignment.Near;
                    m_sfCaption.Trimming = StringTrimming.Character;
                    m_sfCaption.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                }

                return m_sfCaption;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static Blend ToolBarBlend
        {
            get { return m_blToolBar; }
        }
        #endregion

        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum ERENDERTYPE
        {
            Normal = 0,
            Grouped,
            TabBar,
            MAX,
        }
        /// <summary>
        /// 
        /// </summary>
        public enum EBUTTONSTATE
        {
            Normal = 0,
            Pressed,
            Selected,
            Checked,
            Collapsed,
            MAX
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EPEN
        {
            epMenuBorder = 0,
            epMenuItemBorder,
            epMenuStripBorder,
            epButtonBorder,
            epButtonBorderHighlight,
            epButtonBorderDropdown,
            epMenuItemPressedGradientEnd,
            epButtonPressedGradientBegin,
            epButtonPressedGradientMiddle,
            epButtonPressedGradientEnd,
            epButtonSelectedGradientEnd,
            epButtonSelectedBorder,
            epButtonPressedBorder,
            epButtonHighlightBorder,
            epHighlightGrouped,
            epToolStripBorder,
            epGroupBorder,
            epCheckBorder,
            epLauncherBorder,
            epCollapsedBorder,
            epImageBorder,
            MAX,
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EBRUSH
        {
            ebShadow = 0,
            ebToolStripDropDownBackground,
            ebCheckBackground,
            MAX,
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EBITMAP
        {
            ebArrowDown = 0,
            ebArrowRight,
            ebArrowOverflow,
            ebSelectedFlash,
            ebPressedFlash,
            ebCheckedFlash,
            ebLauncher,
            ebLauncherSelected,
            ebLauncher2007,
            ebLauncher2007Selected,
            ebCheckButton,
            ebSystemButtonSelectedFlash,
            ebSystemButtonPressedFlash,
            ebRightArrow,
            ebLeftArrow,
            ebDownArrow,
            ebUpArrow,
            ebRadioButtonChecked,
            ebRadioButtonUnchecked,
            ebRadioButtonCheckedSelected,
            ebRadioButtonUncheckedSelected,
            ebRadioButtonCheckedPressed,
            ebRadioButtonUncheckedPressed,
            MAX,
        };
        #endregion

        #region Fields
        private const int MENUITEM_PADDING_LEFT = 2;
        private const int MENUITEM_PADDING_RIGHT = 1;
        private const int MENUITEM_HORIZONTAL = (MENUITEM_PADDING_LEFT + MENUITEM_PADDING_RIGHT);

        private const int IMAGE_PADDING = 2;
        private const int IMAGE_MARGIN = 1;

        internal const int MENUITEM_RADIUS = 1;
        internal const int TOOLSTRIP_RADIUS = 2;

        protected static Blend m_blMenuBar;
        protected static Blend m_blStatusBar;
        protected static Blend m_blToolBar;
        protected static Blend m_blMenuItemUp;
        protected static Blend m_blMenuItemDown;
        protected static Blend m_blGrouped;
        protected static Blend m_blScrollButton;
        protected static Blend m_blButtonSelected;
        protected static Blend m_blButtonCollapsed;
        protected static Blend m_blButtonShadow;
        protected static Blend m_blImageBackground;
        protected static Blend m_blButtonFlash;
        protected static Blend m_blCaption;
        protected static Blend m_blScrollerBackground;
        protected static Blend m_blStandardScrollButton;
        protected static Blend m_blScroller;
        /// <summary>
        /// 
        /// </summary>
        private static Blend m_blGroupedBackGroundCollapsed;
        /// <summary>
        /// 
        /// </summary>
        private static Blend m_blGroupedBackGround;

        private static Graphics m_gCaption;
        private static StringFormat m_sfCaption = null;
        private ERENDERTYPE m_eRenderType;

        private int m_refCounter = 0;

        private Hashtable m_htPens;
        private Hashtable m_htBrushes;
        protected Hashtable m_htBitmaps;

        private ComboBoxRenderer m_ComboBoxRenderer;

        static Hashtable m_htControls;
        static Object m_controlsLock;

        static int LAUNCHER_WIDTH = 14;//12;
        static int LAUNCHER_HEIGHT = 14;//12;
        static int ARROW_WIDTH = 5;
        static int ARROW_HEIGHT = 6;

        #endregion

        #region Imports
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDC"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        static protected extern bool SetWorldTransform(IntPtr hDC, ref XFORM xform);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDC"></param>
        /// <param name="iMode"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        static protected extern int SetGraphicsMode(IntPtr hDC, int iMode);
        #endregion
    }
    #endregion

    #region RendererUtils
    public class RendererUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Region GetRoundedRegion(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right;
            int iBottom = rc.Bottom;

            Region rgnResult = new Region(rc);

            for (int i = 0; i < iRadius; i++)
            {
                rgnResult.Exclude(new Rectangle(iLeft + i, iTop, 1, iRadius - i));
                rgnResult.Exclude(new Rectangle(iLeft + i, iBottom, 1, i - iRadius));
                rgnResult.Exclude(new Rectangle(iRight - i, iTop, -1, iRadius - i));
                rgnResult.Exclude(new Rectangle(iRight - i, iBottom, -1, i - iRadius));
            }

            return rgnResult;
        }

        public static bool IsValidRegion(ToolStrip toolStrip, Graphics g)
        {
            bool bResult = false;

            if (toolStrip.Region != null)
            {
                SizeF szfRegion = toolStrip.Region.GetBounds(g).Size;
                bResult = (szfRegion == toolStrip.Size);
            }

            return bResult;
        }

        public static Rectangle[] GetAntiAliasingRectangles(Rectangle rect)
        {
            int iLeft = rect.X;
            int iTop = rect.Y;
            int iWidth = rect.Width;
            int iHeight = rect.Height;

            return new Rectangle[]{
             new Rectangle(iLeft+1,iTop,iWidth-2,iHeight),
             new Rectangle(iLeft+2,iTop,iWidth-4,iHeight),
             new Rectangle(iLeft,iTop+1,iWidth,iHeight-2),
             new Rectangle(iLeft,iTop+2,iWidth,iHeight-4),
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="radious"></param>
        /// <returns></returns>
        public GraphicsPath GetRoundedPath(RectangleF bounds, int radious)
        {
            GraphicsPath path = null;

            if (radious > 0)
            {
                float d = 2 * radious;
                float w = bounds.Width;
                float h = bounds.Height;

                RectangleF corner = new RectangleF(0, 0, d, d);

                path = new GraphicsPath();

                // Top left
                path.AddArc(corner, 180, 90);
                // Top
                path.AddLine(radious, 0, w - radious, 0);
                // Top right
                corner.Offset(w - corner.Width, 0);
                path.AddArc(corner, 270, 90);
                // Right
                path.AddLine(w, radious, w, h - radious);
                // Bottom right						
                corner.Offset(0, h - corner.Height);
                path.AddArc(corner, 0, 90);
                // Bottom
                path.AddLine(w - radious + 1, h, radious - 1, h);
                // Bottom left
                corner.Offset(corner.Width - w, 0);
                path.AddArc(corner, 90, 90);
                // Left
                path.AddLine(0, h - radious, 0, radious);

                Matrix m = new Matrix();

                m.Translate(bounds.X, bounds.Y);
                path.Transform(m);
            }

            return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Point[] GetRoundedPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point(iLeft, iTop+iRadius),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight-iRadius, iTop),
					new Point(iRight, iTop+iRadius),
					new Point(iRight, iBottom-iRadius),
					new Point(iRight-iRadius, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
				};

            return points;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Point[] GetSquareRoundedToRightPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point( iLeft, iTop ),
					new Point( iRight - iRadius, iTop ),
					new Point( iRight, iTop + iRadius ),
					new Point( iRight, iBottom - iRadius ),
					new Point( iRight - iRadius, iBottom ),
					new Point( iLeft, iBottom ),
				};

            return points;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Point[] GetSquareRoundedToLeftPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point( iLeft, iTop + iRadius ),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight, iTop),
					new Point(iRight, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
				};

            return points;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Point[] GetSquareRoundedToUpPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point(iLeft, iTop+iRadius),
					new Point(iLeft+iRadius, iTop),
					new Point(iRight-iRadius, iTop),
					new Point(iRight, iTop+iRadius),
					new Point(iRight, iBottom),
					new Point(iLeft, iBottom),
				};

            return points;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="iRadius"></param>
        /// <returns></returns>
        public static Point[] GetSquareRoundedToDownPolygon(Rectangle rc, int iRadius)
        {
            int iLeft = rc.X;
            int iTop = rc.Y;
            int iRight = rc.Right - 1;
            int iBottom = rc.Bottom - 1;

            Point[] points = new Point[]
				{
					new Point(iLeft, iTop),
					new Point(iRight, iTop),
					new Point(iRight, iBottom-iRadius),
					new Point(iRight-iRadius, iBottom),
					new Point(iLeft+iRadius, iBottom),
					new Point(iLeft, iBottom-iRadius),
				};
            return points;
        }
    }
    #endregion

    #region Office2010ToolStripRenderer

    public partial class Office2010ToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region Fields

        private Office2010ColorTable colorTable = null;
        Office2010ToolStripRendererUtils rendererHelper = null;
        Bitmap launcherImageNormal = null;
        Bitmap launcerImageSelected = null;
        Bitmap arrowDownImage = null;
        const int LAUNCHER_SIZE = 14;
        private ComboBoxRenderer comboBoxRenderer = null;
        private ERENDERTYPE m_eRenderType;
        Color[] selectionColors = null;
        ColorBlend selectionBlend = null;
        static int ARROW_WIDTH = 5;
        static int ARROW_HEIGHT = 6;

        #endregion


        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum ERENDERTYPE
        {
            Normal = 0,
            Grouped,
            TabBar,
            MAX,
        }
        #endregion

        #region Properties

        internal Color[] SelectionColors
        {
            get
            {
                if (selectionColors == null)
                {
                    selectionColors = new Color[] 
                    {
                        Color.FromArgb(255,226,119),
                        Color.FromArgb(255,227,124),
                        Color.FromArgb(255,229,133),
                        Color.FromArgb(255,238,164),
                        Color.FromArgb(255,244,192),
                        Color.FromArgb(255,250,214),
                        Color.FromArgb(255,252,224)
                    };
                }

                return selectionColors;
            }
        }


      
        public ERENDERTYPE RenderType
        {
            get
            {
                return m_eRenderType;
            }
            set
            {
                m_eRenderType = value;
            }
        }

        internal ColorBlend SelectionBlend
        {
            get
            {
                if (selectionBlend == null)
                {
                    selectionBlend = new ColorBlend(7);
                    selectionBlend.Positions = new float[]
                    {
                       0F,
                       0.83F,
                       0.86F,
                       0.9F,
                       0.93F,
                       0.96F,
                       1F
                    };
                }

                return selectionBlend;
            }

        }

        public Office2010ToolStripRendererUtils RendererHelper
        {
            get { return rendererHelper; }
            set { rendererHelper = value; }
        }
        
        public Bitmap LauncherImageNormal
        {
            get 
            {
                if (launcherImageNormal == null)
                {
                    Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_SIZE, LAUNCHER_SIZE);
                    launcherImageNormal = new Bitmap(rcImage.Width, rcImage.Height);

                    using (Graphics g = Graphics.FromImage(launcherImageNormal))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                        rc.Offset(1, 1);

                        using (Region region = new Region(rc))
                        {
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                            region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                            region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                            region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                            region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                            region.Translate(-1, -1);

                            using (SolidBrush brush = new SolidBrush(colorTable.FormBorderInActive))
                            {
                                g.FillRegion(brush, region);
                            }
                        }
                    }
                }
                
                return launcherImageNormal;
            }
        }

        public Bitmap LauncerImageSelected
        {
            get 
            {
                if (launcerImageSelected == null)
                {
                    Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_SIZE, LAUNCHER_SIZE);
                    
                    launcerImageSelected = new Bitmap(rcImage.Width, rcImage.Height);

                    using (Graphics g = Graphics.FromImage(launcerImageSelected))
                    {
                        g.Clear(Color.Transparent);

                        using (Region region = new Region(Rectangle.Inflate(rcImage,-1,-1)))
                        {
                            using (Brush brush = new LinearGradientBrush(rcImage,colorTable.ToolstripButtonSelectedBackground,colorTable.ToolstripButtonSelectedBottomCenterColor,LinearGradientMode.Vertical))
                            {
                                g.FillRegion(brush, region);
                            }
                        }

                        Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                        rc.Offset(1, 1);

                        using (Region region = new Region(rc))
                        {
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                            region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                            region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                            region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                            region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                            region.Translate(-1, -1);

                            using (SolidBrush brush = new SolidBrush(colorTable.FormBorderInActive))
                            {
                                g.FillRegion(brush, region);
                            }
                        }
                    }
                }

                return launcerImageSelected; 
            }
        }

        private ComboBoxRenderer ComboRenderer
        {
            get { return comboBoxRenderer; }
        }

        protected Bitmap ArrowDownImage
        {
            get
            {
                if (arrowDownImage == null)
                {
                    arrowDownImage = new Bitmap(ARROW_HEIGHT, ARROW_HEIGHT + 1);
                    using (Graphics g = Graphics.FromImage(arrowDownImage))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH, 1);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(0, 3);
                            while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(-1, 0);
                                rc.Y += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(0, 1);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                }
                return arrowDownImage;
            }
        }

        protected Bitmap ArrowRightImage
        {
            get
            {
                if (arrowDownImage == null)
                {
                    arrowDownImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
                    using (Graphics g = Graphics.FromImage(arrowDownImage))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(3, 0);
                            while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(1, 0);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                }

                return arrowDownImage;
            }
        }

        #endregion

        #region Ctor
        public Office2010ToolStripRenderer()
            : this(new Office2010ColorTable(), ERENDERTYPE.Normal)
        {
        }

        public Office2010ToolStripRenderer(Office2010ColorTable colorTable, ERENDERTYPE erType)
            : base(colorTable)
        {
            this.colorTable = colorTable;
            rendererHelper = new Office2010ToolStripRendererUtils(this);
            comboBoxRenderer = new ComboBoxRenderer(this);
        }

        #endregion

        #region Overrides

        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            this.RendererHelper.Initialize(toolStrip);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintArrow(e))
            {
                base.OnRenderArrow(e);
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
            {
                base.OnRenderButtonBackground(e);
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
            {
                base.OnRenderDropDownButtonBackground(e);
            }
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            base.OnRenderGrip(e);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemImage(e))
            {
                base.OnRenderItemImage(e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
            {
                base.OnRenderItemText(e);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
            {
                base.OnRenderOverflowButtonBackground(e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            base.OnRenderStatusStripSizingGrip(e);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                base.OnRenderToolStripBackground(e);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBorder(e))
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            base.OnRenderToolStripPanelBackground(e);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            base.OnRenderToolStripContentPanelBackground(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderLabelBackground(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            base.OnRenderItemCheck(e);
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderToolStripStatusLabelBackground(e);
        }

        #endregion

        #region Implementations

        private bool PaintItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;
                if (tsItem != null && tsItem.Checked && !ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
                {
                    PaintItemCheckBackground(e);
                }
            }

            Image image = e.Image;
            Rectangle rc = ToolStripRendererUtils.GetImageRect(e);

            if (image != null && rc.Width > 0 && rc.Height > 0)
            {
                bool bDisposeImage = false;

                if (!e.Item.Enabled)
                {
                    image = CreateDisabledImage(image);
                    bDisposeImage = true;
                }

                Graphics g = e.Graphics;

                if (e.Item is CollapsedDropDownButton)
                {
                    Rectangle borderRect = rc;
                    borderRect.Inflate(ToolStripEx.DEF_IMAGE_BORDER_OFFSET, ToolStripEx.DEF_IMAGE_BORDER_OFFSET);

                    using (Pen pen = new Pen(colorTable.FormBorderInActive))
                    {
                        g.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(borderRect, 2));
                    }
                }

                if (e.Item is OfficeButton)
                {
                    if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        rc.X -= 1;
                    }
                    else
                    {
                        rc.X += 1;
                    }
                }

                if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                {
                    Size szImage = image.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    g.DrawImage(image, new Point(iImageX, iImageY));
                }
                else
                {
                    InterpolationMode interpolation = g.InterpolationMode;

                    if (image.Size != rc.Size)
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    g.DrawImage(image, rc);

                    g.InterpolationMode = interpolation;
                }

                if (bDisposeImage)
                {
                    image.Dispose();
                }
            }

            return true;
        }

        private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
        {
            
        }

        private Image GetToolstripItemImageSelected(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            rect.Width -= 1; rect.Height -= 1;
            Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            using (Graphics g = Graphics.FromImage(bmp))
            {
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
                using (Region region = new Region(rect))
                {
                    g.Clip = region;
                }

                using (GraphicsPath ellipse = new GraphicsPath())
                {
                    float offset = rect.Width / 5;
                    ellipse.AddEllipse(new RectangleF(-offset / 2, 0, rect.Width + offset, 30));
                    ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Height - 15)));

                    using (PathGradientBrush p = new PathGradientBrush(ellipse))
                    {
                        p.CenterColor = colorTable.ToolstripButtonSelectedBottomCenterColor;
                        p.SurroundColors = new Color[] { Color.FromArgb(12, colorTable.ToolstripButtonSelectedBottomSurroundColors) };
                        p.FocusScales = new PointF(0.6F, 0.1F);
                        g.FillPath(p, ellipse);
                    }
                }
            }
            return bmp;
        }

        public Size GetDownArrowSize()
        {
            return this.ArrowDownImage.Size;
        }

        private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                Rectangle rc = ToolStripRendererUtils.GetTextRect(e);

                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, e.Item.ForeColor, e.TextFormat);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

            if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
            {
                rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
            }

            if (e.ToolStrip is ContextMenuStripEx)
            {
                ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                rc.Y += statusStrip.TitleHeight;
                rc.Height -= statusStrip.TitleHeight;
            }

            using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
            {
                g.FillRectangle(brush, rc);
            }

            int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
            int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);

            g.DrawLine(Pens.White, beginX, rc.Top, beginX, rc.Bottom - 1);
            using (Pen pen = new Pen(colorTable.ContextMenuImageMargin))
            {
                g.DrawLine(pen, endX, rc.Top, endX, rc.Bottom - 1);
            }

            return true;
        }

        public void DrawCaption(ToolStrip ts)
        {
            if (ToolStripRendererUtils.HasCaption(ts, Office12ToolStripRenderer.ERENDERTYPE.Normal))
            {
                IntPtr wnd = ts.Handle;

                RECT wndRect = new RECT();
                if (WindowsAPI.GetWindowRect(wnd, ref wndRect))
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    if (hdc != IntPtr.Zero)
                    {
                        Rectangle rcUpdate = ToolStripRendererUtils.GetCaptionBounds(ts, Office12ToolStripRenderer.ERENDERTYPE.Normal);

                        rcUpdate.X -= wndRect.left;
                        rcUpdate.Y -= wndRect.top;

                        using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcUpdate))
                        {
                            Graphics g = bg.Graphics;
                            Color lancherBackColor = colorTable.RibbonPanelBackgroundGradientEnd;

                            if (ts.Parent != null && ts.Parent.Parent != null && ts.Parent.Parent is RibbonControlAdv)
                            {
                                if ((ts.Parent.Parent as RibbonControlAdv).CaptionStyle == CaptionStyle.Top)
                                {
                                    lancherBackColor = Color.FromArgb(237, 244, 252);
                                }
                            }
                            if (SystemInformation.HighContrast)
                            {
                                lancherBackColor = Color.Black;
                            }
                            using (Brush brush = new SolidBrush(lancherBackColor))
                            {
                                g.FillRectangle(brush, rcUpdate);
                            }

                            Font f = ToolStripRendererUtils.GetCaptionFont(ts);

                            Rectangle rcText = new Rectangle(0, 0, rcUpdate.Width, rcUpdate.Height);

                            if (ToolStripRendererUtils.HasLauncher(ts))
                            {
                                rcText.Width -= LAUNCHER_SIZE;
                            }

                            TextFormatFlags tf = ToolStripRendererUtils.GetCaptionFormat(ts);
                            Color clrText = colorTable.CaptionText;
                            rcText.Y -= 1;
                            Color color = (ts.ForeColor == SystemColors.ControlText) ? clrText : ts.ForeColor;
                            if (SystemInformation.HighContrast)
                                clrText = SystemColors.MenuText;
                            TextRenderer.DrawText(g, ts.Text, f, rcText, clrText, tf);

                            PaintLauncher(g, rcUpdate, ts as IToolStripExSupport2);

                            bg.Render();
                        }
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        public void DrawBorders(ToolStrip ts)
        {
            if (ts is IToolStripExSupport2)
            {
                if ((ts as IToolStripExSupport2).BorderStyle == ToolStripBorderStyle.None)
                    return;
            }

            IntPtr wnd = ts.Handle;

            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                if (hdc != IntPtr.Zero)
                {
                    Rectangle rcBorder = new Rectangle(rect.Width - 3, 0, 3, rect.Height);

                    using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcBorder))
                    {
                        Graphics g = bg.Graphics;
                        GraphicsState gState = g.Save();

                        using (Brush brush = new SolidBrush(colorTable.RibbonPanelBackgroundGradientBegin))
                        {
                            g.FillRectangle(brush, rcBorder);
                        }
                        using (Pen pen = new Pen(new LinearGradientBrush(rcBorder, colorTable.RibbonPanelBackgroundGradientBegin, colorTable.FormBorderActive, LinearGradientMode.Vertical)))
                        {
                            g.DrawLine(pen, rcBorder.Right - 2, rcBorder.Top, rcBorder.Right - 2, rcBorder.Bottom);
                        }

                        g.Restore(gState);
                        bg.Render();
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            return PaintButtonBackground(e);
        }

        private bool PaintToolStripBorder(ToolStripRenderEventArgs e)
        {
            return true;
        }

        private bool PaintToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is BottomToolstrip)
            {
                bResult = PaintBottomToolstripBackground(e);
            }
            else if (e.ToolStrip is ContextMenuStripEx)
            {
                bResult = PaintContextMenuBackground(e);
            }
            else if (e.ToolStrip is MenuStrip)
            {
                bResult = PaintMenuBackground(e);
            }
            else if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                bResult = PaintToolStripGalleryDropDownBackground(e);
            }
            else
                if (e.ToolStrip is StatusStrip)
                {
                    bResult = PaintStatusStripBackground(e);
                }
                else
                    if (e.ToolStrip is ToolStripDropDown)
                    {
                        bResult = PaintDropdownBackground(e);
                    }
                    else
                    {
                        bResult = PaintGroupedToolbarBackground(e) || PaintToolbarBackground(e);
                    }

            return bResult;
        }

        private bool PaintToolbarBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            ToolStrip ts = e.ToolStrip;

            if (ts != null)
            {
                bool selected = this.RendererHelper.GetIsSelected(ts);
                Rectangle rc = new Rectangle(Point.Empty, ts.Size);
                Graphics g = e.Graphics;
                if (rc.Height > 0 && rc.Width > 0)
                {
                    using (LinearGradientBrush brush = new LinearGradientBrush(rc, colorTable.RibbonPanelBackgroundGradientBegin, colorTable.RibbonPanelBackgroundGradientEnd, LinearGradientMode.Vertical))
                    {
                        PanelItemRenderEventArgs args = e as PanelItemRenderEventArgs;
                        if (args != null)
                        {
                            Point pt = args.PanelStrip.Location;
                            brush.TranslateTransform(-pt.X, -pt.Y, MatrixOrder.Append);
                        }

                    g.FillRectangle(brush, rc);
                    }
                }
                result = true;
            }

            return result;
        }

        private bool PaintGroupedToolbarBackground(ToolStripRenderEventArgs e)
        {
            return false;
        }

        private bool PaintDropdownBackground(ToolStripRenderEventArgs e)
        {
            return false;
        }

        private bool PaintStatusStripBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private void DrawCornerPoint(Graphics g, Point point)
        {
            g.FillRectangle(Brushes.White, new Rectangle(point.X - 1, point.Y - 1, 2, 2));
            g.FillRectangle(Brushes.DarkGray, new Rectangle(point.X - 2, point.Y - 2, 2, 2));
        }

        private bool PaintToolStripGalleryDropDownBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                ToolStripGalleryDropDown dropDown = (ToolStripGalleryDropDown)e.ToolStrip;
                if (dropDown.ShowGrip)
                {
                    Rectangle rect = new Rectangle(0, dropDown.Height - ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT,
                        dropDown.Width, ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT);
                    using (LinearGradientBrush b = new LinearGradientBrush (rect, Color.White, this.colorTable.GripGradientEnd, LinearGradientMode.Vertical))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillRectangle(b, rect);
                    }

                    Point p = new Point(rect.Right - 4, rect.Bottom - 3);
                    DrawCornerPoint(e.Graphics, p);
                    p.Y -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    p.Y += 4;
                    p.X -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    result = true;
                }
            }


            return result;
        }

        private bool PaintMenuBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintContextMenuBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

             RECT rect = new RECT();

            ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

            if (menuStrip != null)
            {
                if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                        Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                        Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

                        using (Brush brush = new SolidBrush(this.colorTable.ToolStripDropDownBackground))
                        {
                            e.Graphics.FillRectangle(brush, rectangleBody);
                        }

                        using (Brush brush = new SolidBrush(this.colorTable.ContextMenuTitleBackground))
                        {
                            e.Graphics.FillRectangle(brush, rectangleTitle);
                        }

                        using (Pen pen = new Pen(Color.FromArgb(60, Color.Black)))
                        {
                            e.Graphics.DrawLine(
                                pen, rectangleTitle.Left + 3, rectangleTitle.Bottom, rectangleTitle.Right - 3, rectangleTitle.Bottom);
                        }

                        Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

                        TextFormatFlags flags = TextFormatFlags.EndEllipsis;

                        if (menuStrip.RightToLeft == RightToLeft.Yes)
                        {
                            flags |= TextFormatFlags.Right;
                        }

                        TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, this.colorTable.CaptionText, flags);

                        result = true;
                    }
                }
            }
            return result;
        }

        private bool PaintBottomToolstripBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintLauncher(Graphics g, Rectangle rcCaption, IToolStripExSupport2 iRibbon)
        {
            if (iRibbon != null && iRibbon.ShowLauncher)
            {
                Rectangle rc = ToolStripRendererUtils.GetLauncherBounds(rcCaption, false);
               
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Image image = iRibbon.LauncherSelected ? this.LauncerImageSelected : this.LauncherImageNormal;
                    if (iRibbon is ToolStrip)
                    {
                        ToolStrip ts = iRibbon as ToolStrip;

                        if (ts.TopLevelControl is Form && !ts.TopLevelControl.ContainsFocus)
                        {
                            image = this.LauncherImageNormal;
                        }
                    }
                    if (image != null)
                    {
                        g.DrawImage(image, rc.Location);
                    }
                    
                    return true;
                }
            }

            return false;
        }

        private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintTabBarButtonBackground(e.Item, e.Graphics, Office12ToolStripRenderer.GetButtonRect(e.Item,new Rectangle(Point.Empty,e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal)))
            {
                if (!PaintGroupedButtonBackground(e))
                {
                    if (!PaintCollapsedButtonBackground(e))
                    {
                        PaintToolBarButtonBackground(e);
                    }
                }
            }
            return true;
        }

        private bool PaintTabBarButtonBackground(ToolStripItem toolStripItem, Graphics graphics, Rectangle rectangle)
        {
            bool result = false;

            return result;
        }
         
        private bool PaintCollapsedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                if (!PaintCollapsedBackgroundPressed(e))
                {
                    if (!PaintCollapsedBackgroundSelected(e))
                    {
                        PaintCollapsedBackgroundNormal(e);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private void PaintCollapsedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {

        }

        private bool PaintCollapsedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
            
            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rect = new Rectangle(Point.Empty, item.Size);

                using (GraphicsPath ellipse = new GraphicsPath())
                {
                    float offset = rect.Width / 4;
                    ellipse.AddEllipse(new RectangleF(offset / 2, 0, rect.Width - offset, rect.Height));
                    ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Height / 2)));
                    using (PathGradientBrush p = new PathGradientBrush(ellipse))
                    {
                        p.CenterColor = colorTable.RibbonPanelBackgroundGradientBegin;
                        p.SurroundColors = new Color[] { Color.FromArgb(12, colorTable.RibbonPanelBackgroundGradientEnd) };
                        p.FocusScales = new PointF(0.6F, 0.1F);
                        e.Graphics.FillPath(p, ellipse);
                    }
                    result = true;
                }
            }
            return result;
        }

        private bool PaintCollapsedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
          
            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsPressed(item))
            {
                int offset =(e.Item.Height / 4);
                Rectangle rect = new Rectangle(0, offset, e.Item.Width, e.Item.Height - offset);

                using (Brush brush = new LinearGradientBrush(rect,Color.Transparent,colorTable.FormBorderInActive, LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                
                result = true;
            }

            return result;
        }

        private bool PaintArrow(ToolStripArrowRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                Point loc = new Point();
                loc.Y = item.Height - item.Padding.Bottom - GetDownArrowSize().Height;
                loc.X = (item.Width - item.Padding.Horizontal - this.ArrowDownImage.Width) / 2 + item.Padding.Left;

                e.Graphics.DrawImage(this.ArrowDownImage, loc);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintGroupedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintToolBarButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = true;
           
            if (!PaintButtonBackgroundPressed(e))
            {
                if (!PaintButtonBackgroundChecked(e))
                {
                   result = PaintButtonBackgroundSelected(e);
                }
            }

            return result;
        }

        private bool PaintButtonBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
            bool isPanelNotInForm = false;
            if (!(e.ToolStrip.TopLevelControl is Form))
            {
                isPanelNotInForm = true;
            }
            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

                if (isPanelNotInForm || (rc.Width > 0 && rc.Height > 0 && e.ToolStrip.TopLevelControl.ContainsFocus))
                {
                    Graphics g = e.Graphics;
                    
                    Image img = this.GetToolstripItemImageSelected(rc);
                    
                    g.DrawImage(img, Point.Empty);
                }
            }

            return result;
        }

        private bool PaintButtonBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);

                Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(polygon);

                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.SurroundColors = new Color[] { colorTable.ToolstripButtonPressedBorder };
                        brush.CenterColor = colorTable.ToolstripButtonCheckedBackground;
                        brush.FocusScales = new PointF(0.99f, 0.99f);

                        e.Graphics.FillPolygon(brush, polygon);
                    }

                    PaintButtonBorder(e, polygon, colorTable.ToolstripButtonPressedBorder);

                    result = true;
                }
            }

            return result;
        }

        private bool PaintButtonBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);

                Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(polygon);

                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        brush.SurroundColors = new Color[] { colorTable.ToolstripButtonPressedBorder };
                        brush.CenterColor = colorTable.ToolstripButtonPressedBackground;
                        brush.FocusScales = new PointF(1,1);

                        e.Graphics.FillPolygon(brush, polygon);
                    }

                    PaintButtonBorder(e, polygon, colorTable.ToolstripButtonPressedBorder);

                    result = true;
                }
            }

            return result;
        }

        private void PaintButtonBorder(ToolStripItemRenderEventArgs e, Point[] polygon, Color border)
        {
            using (Pen pen = new Pen(border))
            {
                e.Graphics.DrawPolygon(pen, polygon);
            }
        }

        private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            ToolStripSplitButton tsBtn = e.Item as ToolStripSplitButton;

            if (tsBtn != null && tsBtn.Enabled)
            {
                Rectangle rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.ButtonBounds.Height - 2));
                Rectangle rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.DropDownButtonBounds.Width, tsBtn.Bounds.Height - 2));

                bool paintBorderPressed = tsBtn.ButtonPressed || tsBtn.DropDownButtonPressed;
                bool paintBorderSelected = !paintBorderPressed && (tsBtn.DropDownButtonSelected || tsBtn.ButtonSelected);

                GraphicsState state = e.Graphics.Save();

                Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);

                e.Graphics.SetClip(RendererUtils.GetRoundedRegion(rcBounds, 2), CombineMode.Intersect);

                #region Paint Button Background

                rcButton.X += 1; rcButton.Width -= 1;
                if (tsBtn.ButtonPressed)
                {
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcButton);
                }
                else if (tsBtn.ButtonSelected || tsBtn.DropDownButtonPressed && (e.ToolStrip.TopLevelControl.ContainsFocus))
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcButton);
                }

                #endregion

                #region Paint DropDownButtonBackground

                if (tsBtn.DropDownButtonPressed)
                {
                    rcDropDown.X -= 1;
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown);
                }
                else if (tsBtn.DropDownButtonSelected && (e.ToolStrip.TopLevelControl==null || e.ToolStrip.TopLevelControl.ContainsFocus))
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown);
                }

                #endregion

                #region PaintBorder

                if (paintBorderPressed)
                {
                    rcBounds.X += 1; rcBounds.Width -= 1;
                    using (Pen pen = new Pen(colorTable.ToolstripButtonPressedBorder))
                    {
                        e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                        e.Graphics.DrawLine(pen,rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                    }
                }
                else if (paintBorderSelected)
                {
                    rcBounds.X += 1; rcBounds.Width -= 1;

                    using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBottomCenterColor))
                    {
                        e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(Rectangle.Inflate(rcBounds, -1, -1), 2));
                        e.Graphics.DrawLine(pen,new Point(rcDropDown.Left-1 , rcDropDown.Top) , new Point(rcDropDown.Left - 1 , rcDropDown.Bottom));
                        e.Graphics.DrawLine(pen, new Point(rcDropDown.Left + 1, rcDropDown.Top), new Point(rcDropDown.Left + 1, rcDropDown.Bottom));
                    }

                    using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBorder))
                    {
                        e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                        e.Graphics.DrawLine(pen, rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                    }
                }

                #endregion

                #region Paint Arrrow

                Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                bool bVertical = tsBtn != null && (tsBtn.Dock == DockStyle.Left || tsBtn.Dock == DockStyle.Right);
                ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                base.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, rcArrow, color, dir));

                #endregion

                e.Graphics.Restore(state);

                result = true;
            }

            return result;
        }

        private bool PaintSplitButtonBackgroundPressed(Graphics g, Rectangle rect)
        {
            using (Brush brush = new SolidBrush(colorTable.ToolstripButtonPressedBackground))
            {
                g.FillRectangle(brush, rect);
            }

            return true;
        }

        private bool PaintSplitButtonBackgroundSelected(Graphics g, Rectangle rect)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.Empty,Color.Empty, LinearGradientMode.Vertical))
            {
                this.SelectionBlend.Colors = this.SelectionColors;
                brush.InterpolationColors = this.SelectionBlend;

                g.FillRectangle(brush, rect);
            }

            return true;
        }

        private bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

            if (item != null)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        Image imgArrow = this.GetToolstripItemImageSelected(Rectangle.Inflate(rc,1,1));

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);

                        result = true;
                    }
                }
            }
            else
            {
                ToolStripOverflowButton tsBtn = e.Item as ToolStripOverflowButton;
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                rc.Inflate(-3, -1);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        if (tsBtn != null)
                        {
                            if (tsBtn.Selected)
                            {
                                using (LinearGradientBrush brush = new LinearGradientBrush(rc, Color.Empty ,Color.Empty, LinearGradientMode.Vertical))
                                {
                                    brush.InterpolationColors = this.SelectionBlend;
                                    this.SelectionBlend.Colors = this.SelectionColors;

                                    e.Graphics.FillRectangle(brush, rc);
                                }
                            }
                            else if (tsBtn.Pressed)
                            {
                                using (Brush brush = new SolidBrush(colorTable.ToolstripButtonPressedBackground))
                                {
                                    e.Graphics.FillRectangle(brush, rc);
                                }
                            }
                            else
                            {
                                using (Brush brush = new LinearGradientBrush(rc, colorTable.TabScrollButtonGradientBegin, colorTable.TabScrollButtonGradientEnd, LinearGradientMode.Vertical))
                                {
                                    e.Graphics.FillRectangle(brush, rc);
                                }
                            }
                        }

                        using (Pen pen = new Pen(colorTable.TabScrollButtonBorder))
                        {
                            e.Graphics.DrawRectangle(pen, rc);
                        }

                        bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
                        Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);

                        result = true;
                    }
                }
            }

            return result;
        }

        internal bool PaintSplitButtonExBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
            bool isFormFocused = (e.ToolStrip.TopLevelControl is Form && e.ToolStrip.TopLevelControl.ContainsFocus);
            ToolStripSplitButtonEx tsBtn = e.Item as ToolStripSplitButtonEx;

            if (tsBtn != null && tsBtn.Enabled)
            {
                Rectangle rcButton = Rectangle.Empty;
                Rectangle rcDropDown = Rectangle.Empty;
                if(tsBtn.RightToLeft == RightToLeft.Yes)
                {
                    rcDropDown = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.Bounds.Height - 2));
                    rcButton = new Rectangle(new Point(rcDropDown.Right, rcDropDown.Top), new Size(tsBtn.ImageBounds.Width, tsBtn.ImageBounds.Height - 2));
                }
                else
                {
                    rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ImageBounds.Width, tsBtn.ImageBounds.Height - 2));
                    rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.ButtonBounds.Width, tsBtn.Bounds.Height - 2));
                }

                bool paintBorderPressed = tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImagePressed;
                bool paintBorderSelected = !paintBorderPressed && (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImageSelected || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected);

                GraphicsState state = e.Graphics.Save();

                Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);

                e.Graphics.SetClip(RendererUtils.GetRoundedRegion(rcBounds, 2), CombineMode.Intersect);

                #region Paint Button Background

                rcButton.X += 1; rcButton.Width -= 1;
                if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImagePressed || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed)
                {
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcButton);
                }
                else if ((tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && isFormFocused) || (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && !(e.ToolStrip.TopLevelControl is Form)))
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcButton);
                }

                #endregion

                #region Paint DropDownButtonBackground

                if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed)
                {
                    rcDropDown.X -= 1;
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown);
                }
                else if ((tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && isFormFocused) || (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && !(e.ToolStrip.TopLevelControl is Form)))
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown);
                }

                #endregion

                #region PaintBorder

                if (paintBorderPressed)
                {
                    rcBounds.X += 1; rcBounds.Width -= 1;
                    using (Pen pen = new Pen(colorTable.ToolstripButtonPressedBorder))
                    {
                        e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                        e.Graphics.DrawLine(pen, rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                    }
                }
                else if (paintBorderSelected)
                {
                    rcBounds.X += 1; rcBounds.Width -= 1;

                    using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBottomCenterColor))
                    {
                        if ((tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && isFormFocused) || (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected && !(e.ToolStrip.TopLevelControl is Form)))
                        {
                            rcDropDown.Width -= 3;
                            e.Graphics.DrawPolygon(pen, RendererUtils.GetSquareRoundedToRightPolygon(Rectangle.Inflate(rcDropDown, -1, -1), 2));
                            e.Graphics.DrawLine(pen, new Point(rcDropDown.Left + 1, rcDropDown.Top), new Point(rcDropDown.Left + 1, rcDropDown.Bottom));
                        }
                        else if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImageSelected)
                        {
                            rcButton.Width += 1;
                            e.Graphics.DrawPolygon(pen, RendererUtils.GetSquareRoundedToLeftPolygon(Rectangle.Inflate(rcButton, -1, -1), 2));
                            e.Graphics.DrawLine(pen, new Point(rcDropDown.Left - 1, rcDropDown.Top), new Point(rcDropDown.Left - 1, rcDropDown.Bottom));
                        }
                    }

                    using (Pen pen = new Pen(colorTable.ToolstripButtonSelectedBorder))
                    {
                        e.Graphics.DrawPolygon(pen, RendererUtils.GetRoundedPolygon(rcBounds, 2));
                        e.Graphics.DrawLine(pen, rcDropDown.Location, new Point(rcDropDown.Left, rcDropDown.Bottom));
                    }
                }

                #endregion

                #region Paint Text, Image and Arrow

                Image image = tsBtn.Image;

                // Draw image.
                if ((tsBtn.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    if (image != null)
                    {
                        bool bDisposeImage = false;

                        if (!tsBtn.Enabled)
                        {
                            image = CreateDisabledImage(image);
                            bDisposeImage = true;
                        }

                        e.Graphics.DrawImage(image, tsBtn.InternalLayout.ImageRectangle);

                        if (bDisposeImage)
                        {
                            image.Dispose();
                        }
                    }
                }

                Color clForeColor = !tsBtn.Enabled ? SystemColors.GrayText : tsBtn.ForeColor;

                // Draw text.
                if ((tsBtn.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                {
                    TextRenderer.DrawText(e.Graphics, tsBtn.Text, tsBtn.Font, tsBtn.InternalLayout.TextRectangle, clForeColor, ToolStripRendererUtils.GetTextFormatFlags(tsBtn.TextAlign));
                }

                // Draw arrow.
                Rectangle arrowRectangle = tsBtn.InternalLayout.DropDownButtonRectangle;

                using (Brush brush = new SolidBrush(clForeColor))
                {
                    Point pt = new Point(arrowRectangle.Left + (arrowRectangle.Width / 2), arrowRectangle.Top + (arrowRectangle.Height / 2));
                    Point[] points = new Point[] { new Point(pt.X - 2, pt.Y - 1), new Point(pt.X + 3, pt.Y - 1), new Point(pt.X, pt.Y + 2) };

                    e.Graphics.FillPolygon(brush, points);
                }

                #endregion

                e.Graphics.Restore(state);

                result = true;
            }

            return result;
        }

        private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (e.Item.IsOnDropDown)
            {
                result = PaintDropDownMenuItemBackground(e);
            }
            else
            {
                result = PaintToolStripMenuItemBackground(e);
            }
            return result;
        }

        private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

            if ((tsItem != null && tsItem.Enabled) && tsItem.Selected || tsItem.Pressed)
            {
                Image image = GetToolstripItemImageSelected(new Rectangle(Point.Empty,tsItem.Size));

                e.Graphics.DrawImage(image, Point.Empty);

                result = true;
            }

            return result;
        }


        #endregion

        #region ** Office2010ToolStripRenderer Utilities

        public class Office2010ToolStripRendererUtils
        {
            #region Fields

            Office2010ToolStripRenderer renderer = null;
            Hashtable htControls = null;
            object m_controlsLock = null;

            #endregion

            #region Properties

            public Hashtable HtControls
            {
                get { return htControls; }
            }

            public Office2010ToolStripRenderer Renderer
            {
                get { return renderer; }
            }

            #endregion

            #region Ctor
            public Office2010ToolStripRendererUtils(Office2010ToolStripRenderer renderer)
            {
                this.renderer = renderer;
                htControls = new Hashtable();
                m_controlsLock = new object();
            }
            #endregion
            
            #region Attach - Detach ToolStrip

            private void Attach(ToolStrip ts)
            {
                Subscribe(ts);
            }
            
            private void Detach(ToolStrip toolstrip)
            {
                Unsubscribe(toolstrip);
            }

            private void Subscribe(ToolStrip ts)
            {
                ts.RendererChanged += new EventHandler(OnRendererChanged);

                if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
                {
                    ts.HandleCreated += new EventHandler(OnHandleCreated);
                    ts.HandleDestroyed += new EventHandler(OnHandleDestroyed);
                    ts.DockChanged += new EventHandler(OnDockChanged);

                    if (ts.IsHandleCreated)
                    {
                        OnHandleCreated(ts, EventArgs.Empty);
                    }

                    if (ts.GetType() == typeof(ToolStrip))
                    {
                        ts.LayoutCompleted += new EventHandler(OnLayoutCompleted);
                    }
                }
                else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
                {
                    ts.HandleCreated += new EventHandler(OnRoundedToolStripRegionChanged);
                    ts.SizeChanged += new EventHandler(OnRoundedToolStripRegionChanged);
                }

                AttachItems(ts);
            }

            private void Unsubscribe(ToolStrip ts)
            {
                DetachItems(ts);

                ts.RendererChanged -= new EventHandler(OnRendererChanged);

                if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
                {
                    ts.HandleCreated -= new EventHandler(OnHandleCreated);
                    ts.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
                    ts.DockChanged -= new EventHandler(OnDockChanged);

                    if (ts.GetType() == typeof(ToolStrip))
                    {
                        ts.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
                    }

                    if (!(ts.Renderer is Office12ToolStripRenderer))
                    {
                        ReleaseHandle(ts);
                        ClearDropDownRegions(ts);
                    }
                }
                else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
                {
                    ts.HandleCreated -= new EventHandler(OnRoundedToolStripRegionChanged);
                    ts.SizeChanged -= new EventHandler(OnRoundedToolStripRegionChanged);

                    if (!(ts.Renderer is Office12ToolStripRenderer))
                    {
                        ts.Region = null;
                    }
                }
            }
                       
            #endregion

            #region Attach - Detach ToolStripItems

            private void AttachItems(ToolStrip ts)
            {
                ts.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);
                ts.ItemRemoved += new ToolStripItemEventHandler(OnItemRemoved);

                foreach (ToolStripItem item in ts.Items)
                {
                    OnItemAdded(ts, new ToolStripItemEventArgs(item));
                }
            }

            private void DetachItems(ToolStrip ts)
            {
                ts.ItemAdded -= new ToolStripItemEventHandler(OnItemAdded);
                ts.ItemRemoved -= new ToolStripItemEventHandler(OnItemRemoved);

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    foreach (ToolStripItem item in ts.Items)
                    {
                        OnItemRemoved(ts, new ToolStripItemEventArgs(item));
                    }
                }
            }

            #endregion

            #region Event Handlers

            void OnItemRemoved(object sender, ToolStripItemEventArgs e)
            {
                if (e.Item is ToolStripComboBox)
                {
                    OnRemovedComboBox(e.Item as ToolStripComboBox);
                }
            }

            private void OnRemovedComboBox(ToolStripComboBox toolStripComboBox)
            {
                Control comboBox = toolStripComboBox.Control;

                if (comboBox != null)
                {
                    comboBox.HandleCreated -= new EventHandler(OnComboBoxHandleCreated);
                    comboBox.HandleDestroyed -= new EventHandler(OnComboBoxHandleDestroyed);

                    if (comboBox.IsHandleCreated)
                    {
                        ReleaseControl(comboBox);
                    }
                }
            }

            void OnItemAdded(object sender, ToolStripItemEventArgs e)
            {
                if (e.Item is ToolStripComboBox)
                {
                    OnAddedComboBox(e.Item as ToolStripComboBox);
                }
                else if (e.Item is ToolStripDropDownItem)
                {
                    OnAddedDropDownItem(e.Item as ToolStripDropDownItem);
                }
            }

            private void OnAddedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
            {
                
            }

            private void OnAddedComboBox(ToolStripComboBox toolStripComboBox)
            {
                Control comboBox = toolStripComboBox.Control;

                if (comboBox != null)
                {
                    comboBox.HandleCreated += new EventHandler(OnComboBoxHandleCreated);
                    comboBox.HandleDestroyed += new EventHandler(OnComboBoxHandleDestroyed);

                    if (comboBox.IsHandleCreated)
                    {
                        SubclassControl(comboBox, this.Renderer.ComboRenderer);
                    }
                }
            }

            void OnComboBoxHandleCreated(object sender, EventArgs e)
            {
                SubclassControl(sender as Control, this.Renderer.ComboRenderer);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OnComboBoxHandleDestroyed(object sender, EventArgs e)
            {
                ReleaseControl(sender as Control);
            }

            void OnRoundedToolStripRegionChanged(object sender, EventArgs e)
            {
                
            }

            void OnLayoutCompleted(object sender, EventArgs e)
            {
                
            }

            void OnDockChanged(object sender, EventArgs e)
            {
                
            }

            void OnHandleDestroyed(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;
                if (ts != null)
                {
                    ReleaseHandle(ts);
                }
            }

            void OnHandleCreated(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;
                if (ts != null)
                {
                    AssignHandle(ts);
                }
            }
            
            void OnRendererChanged(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;

                if (ts != null && ts.Renderer != this.Renderer)
                {
                    Detach(ts);
                }
            }
            #endregion

            #region Implementations

            public void Initialize(ToolStrip toolstrip)
            {
                if (toolstrip != null)
                {
                    Attach(toolstrip);
                }
            }

            private void ClearDropDownRegions(ToolStrip ts)
            {

            }

            private void AssignHandle(ToolStrip ts)
            {
                lock (m_controlsLock)
                {
                    if (!htControls.ContainsKey(ts))
                    {
                        Office12ToolStripRenderer.CToolStripWindow wnd = new Office12ToolStripRenderer.CToolStripWindow(ts);

                        wnd.AssignHandle(ts.Handle);
                        htControls.Add(ts, wnd);
                    }
                }
                using (Region region = new Region(new Rectangle(Point.Empty, ts.Size)))
                {
                    ts.Region = region;
                }

                ToolStripRendererUtils.UpdateFrame(ts);
            }

            private void ReleaseHandle(ToolStrip ts)
            {
                lock (m_controlsLock)
                {
                    object obj = htControls[ts];
                    if (obj != null)
                    {
                        Office12ToolStripRenderer.CToolStripWindow wnd = obj as Office12ToolStripRenderer.CToolStripWindow;

                        if (wnd != null)
                        {
                            wnd.ReleaseHandle();
                        }

                        htControls.Remove(ts);
                    }
                }

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ts.Region = null;
                }

                ToolStripRendererUtils.UpdateFrame(ts);
            }

            internal bool GetIsSelected(ToolStrip ts)
            {
                Office12ToolStripRenderer.CToolStripWindow cwd = this.HtControls[ts] as Office12ToolStripRenderer.CToolStripWindow;

                if (cwd != null)
                {
                    return cwd.Selected;
                }
                else
                    return false;
            }

            void SubclassControl(Control control, INativeMessageFilter messageFilter)
            {
                lock (m_controlsLock)
                {
                    NativeMessageHandler handler = htControls[control] as NativeMessageHandler;

                    if (handler == null)
                    {
                        handler = new NativeMessageHandler();
                        htControls.Add(control, handler);

                        handler.Assign(control.Handle);
                    }

                    handler.MessageFilter = messageFilter;
                }
            }

            void ReleaseControl(Control control)
            {
                lock (m_controlsLock)
                {
                    object obj = htControls[control];
                    if (obj != null)
                    {
                        NativeMessageHandler handler = obj as NativeMessageHandler;

                        if (handler != null)
                        {
                            handler.MessageFilter = null;
                        }

                        htControls.Remove(control);
                    }
                }
            }

            #endregion
        }

        #endregion
    }

    #endregion

    #region ** ToolStripRendererUtils
    public class ToolStripRendererUtils
    {
        #region Fields
        const SetWindowPosFlags m_swpFlags =
            SetWindowPosFlags.SWP_NOZORDER |
            SetWindowPosFlags.SWP_NOSIZE |
            SetWindowPosFlags.SWP_NOMOVE |
            SetWindowPosFlags.SWP_NOACTIVATE |
            SetWindowPosFlags.SWP_FRAMECHANGED;

        const int LAUNCHER_SIZE = 14;
        const int STATICEDGE_WIDTH = 1;
        const int ETCHED_WIDTH = 2;
        #endregion

        #region Static Helper Methods
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetTextRect(ToolStripItemTextRenderEventArgs e)
        {
            Rectangle result = e.TextRectangle;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                result.Size = TextRenderer.MeasureText(item.Text, item.Font);

                result.Y = item.Padding.Top;

                if (item.DisplayStyle == ToolStripItemDisplayStyle.ImageAndText)
                {
                    result.Y += item.Owner.ImageScalingSize.Height
                        + ToolStripEx.DEF_PIXELS_BETWEEN_ELEMENTS_COLLAPSED + 2 * ToolStripEx.DEF_IMAGE_BORDER_OFFSET;
                }

                result.X = (item.Width - item.Padding.Horizontal - result.Width) / 2 + item.Padding.Left;
            }

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetImageRect(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle result = e.ImageRectangle;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                result.Y = item.Padding.Top + ToolStripEx.DEF_IMAGE_BORDER_OFFSET;
                result.X =
                    (item.Width - item.Padding.Horizontal - result.Width) / 2 + item.Padding.Left;
            }

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool IsCheckMarginVisible(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (tsItem is ToolStripMenuItem)
            {
                ToolStripDropDownMenu tsddMenu = tsItem.Owner as ToolStripDropDownMenu;
                if (tsddMenu != null)
                {
                    bResult = tsddMenu.ShowCheckMargin;
                }
            }

            return bResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Region GetRegion(ToolStrip ts)
        {
            Region region = null;

            IToolStripExSupport2 iToolStrip = ts as IToolStripExSupport2;

            if (iToolStrip != null)
            {
                switch (iToolStrip.BorderStyle)
                {
                    case ToolStripBorderStyle.Etched:
                        {
                            if (ts.IsHandleCreated)
                            {
                                RECT rc = new RECT();
                                if (WindowsAPI.GetWindowRect(ts.Handle, ref rc))
                                {
                                    region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, rc.Size), 1);
                                }
                            }
                        }
                        break;
                }
            }

            return region;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static void UpdateFrame(ToolStrip ts)
        {
            if (ts != null && ts.IsHandleCreated)
            {
                WindowsAPI.SetWindowPos(ts.Handle, IntPtr.Zero, 0, 0, 0, 0, m_swpFlags);
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static CaptionStyle GetCaptionStyle(ToolStrip ts)
        {
            CaptionStyle result = CaptionStyle.Top;

            if (ts is IToolStripExSupport2)
            {
                result = (ts as IToolStripExSupport2).CaptionStyle;
            }

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int GetBorderWidth(ToolStrip toolStrip)
        {
            switch (GetBorderStyle(toolStrip))
            {
                case ToolStripBorderStyle.StaticEdge:
                    return STATICEDGE_WIDTH;
                case ToolStripBorderStyle.Etched:
                    return ETCHED_WIDTH;
            }
            return 0;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static ToolStripBorderStyle GetBorderStyle(ToolStrip ts)
        {
            IToolStripExSupport2 iToolStrip = ts as IToolStripExSupport2;
            if (iToolStrip != null)
            {
                return iToolStrip.BorderStyle;
            }
            return ToolStripBorderStyle.None;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Font GetCaptionFont(ToolStrip ts)
        {
            IToolStripExSupport2 iToolStripEx = ts as IToolStripExSupport2;

            if (iToolStripEx != null)
            {
                return iToolStripEx.CaptionFont;
            }
            return ts.Font;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool HasCaption(ToolStrip toolStrip, MetroToolStripRenderer.ERENDERTYPE renderType)
        {
            bool bResult = false;
            if (toolStrip != null)
            {
                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null)
                {
                    bResult = iRibbon.ShowCaption;
                }
                else
                {
                    bResult = toolStrip.GetType() == typeof(ToolStrip) && renderType != MetroToolStripRenderer.ERENDERTYPE.TabBar;
                }
            }

            return bResult;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool HasCaption(ToolStrip toolStrip, Office12ToolStripRenderer.ERENDERTYPE renderType)
        {
            bool bResult = false;
            if (toolStrip != null)
            {
                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null)
                {
                    bResult = iRibbon.ShowCaption;
                }
                else
                {
                    bResult = toolStrip.GetType() == typeof(ToolStrip) && renderType != Office12ToolStripRenderer.ERENDERTYPE.TabBar;
                }
            }

            return bResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool HasCaption(ToolStrip toolStrip, Office2010ToolStripRenderer.ERENDERTYPE renderType)
        {
            bool bResult = false;
            if (toolStrip != null)
            {
                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null)
                {
                    bResult = iRibbon.ShowCaption;
                }
                else
                {
                    bResult = toolStrip.GetType() == typeof(ToolStrip) && renderType != Office2010ToolStripRenderer.ERENDERTYPE.TabBar;
                }
            }

            return bResult;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int GetCaptionHeight(ToolStrip toolStrip, MetroToolStripRenderer.ERENDERTYPE renderType)
        {
            int nResult = 0;

            if (ToolStripRendererUtils.HasCaption(toolStrip, renderType))
            {
                nResult = TextRenderer.MeasureText("X", ToolStripRendererUtils.GetCaptionFont(toolStrip)).Height;

                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null && iRibbon.ShowLauncher && nResult < LAUNCHER_SIZE)
                {
                    nResult = LAUNCHER_SIZE;
                }

                IToolStripExSupport2 iToolStripEx2 = toolStrip as IToolStripExSupport2;

                if (iToolStripEx2 != null && iToolStripEx2.CaptionMinHeight > nResult)
                {
                    nResult = iToolStripEx2.CaptionMinHeight;
                }
            }

            return nResult;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int GetCaptionHeight(ToolStrip toolStrip, Office12ToolStripRenderer.ERENDERTYPE renderType)
        {
            int nResult = 0;

            if (ToolStripRendererUtils.HasCaption(toolStrip, renderType))
            {
                nResult = TextRenderer.MeasureText("X", ToolStripRendererUtils.GetCaptionFont(toolStrip)).Height;

                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null && iRibbon.ShowLauncher && nResult < LAUNCHER_SIZE)
                {
                    nResult = LAUNCHER_SIZE;
                }

                IToolStripExSupport2 iToolStripEx2 = toolStrip as IToolStripExSupport2;

                if (iToolStripEx2 != null && iToolStripEx2.CaptionMinHeight > nResult)
                {
                    nResult = iToolStripEx2.CaptionMinHeight;
                }
            }

            return nResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static int GetCaptionHeight(ToolStrip toolStrip, Office2010ToolStripRenderer.ERENDERTYPE renderType)
        {
            int nResult = 0;

            if (ToolStripRendererUtils.HasCaption(toolStrip, renderType))
            {
                nResult = TextRenderer.MeasureText("X", ToolStripRendererUtils.GetCaptionFont(toolStrip)).Height;

                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null && iRibbon.ShowLauncher && nResult < LAUNCHER_SIZE)
                {
                    nResult = LAUNCHER_SIZE;
                }

                IToolStripExSupport2 iToolStripEx2 = toolStrip as IToolStripExSupport2;

                if (iToolStripEx2 != null && iToolStripEx2.CaptionMinHeight > nResult)
                {
                    nResult = iToolStripEx2.CaptionMinHeight;
                }
            }

            return nResult;
        }


        [Syncfusion.Documentation.DocumentationExclude()]
        public static void ApplyBorders(ToolStrip ts, ref RECT rc)
        {
            int nWidth = GetBorderWidth(ts);

            if (nWidth > 0)
            {
                WindowsAPI.InflateRect(ref rc, -nWidth, -nWidth);
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void ApplyCaption(ToolStrip ts, ref RECT rc, MetroToolStripRenderer.ERENDERTYPE renderType)
        {
            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);

            if (nHeight > 0)
            {
                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.right -= nHeight;
                            break;
                        default:
                            rc.left += nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.bottom -= nHeight;
                            break;
                        default:
                            rc.top += nHeight;
                            break;
                    }
                }
            }
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static void ApplyCaption(ToolStrip ts, ref RECT rc, Office12ToolStripRenderer.ERENDERTYPE renderType)
        {
            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);

            if (nHeight > 0)
            {
                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.right -= nHeight;
                            break;
                        default:
                            rc.left += nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.bottom -= nHeight;
                            break;
                        default:
                            rc.top += nHeight;
                            break;
                    }
                }
            }
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetLauncherBounds(Rectangle rcCaption, bool bVertical)
        {
            Rectangle rcResult = new Rectangle();

            if (!bVertical)
            {
                rcResult.X = rcCaption.Right - (LAUNCHER_SIZE + 1);
                rcResult.Y = rcCaption.Y;
                rcResult.Width = LAUNCHER_SIZE + 1;
                rcResult.Height = LAUNCHER_SIZE + 1;
            }
            else
            {
                rcResult.X = rcCaption.Left;
                rcResult.Y = rcCaption.Y;
                rcResult.Width = LAUNCHER_SIZE + 1;
                rcResult.Height = LAUNCHER_SIZE + 1;
            }

            return rcResult;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetCaptionBounds(ToolStrip ts, MetroToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rc = Rectangle.Empty;

            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);
            if (nHeight > 0)
            {
                rc = ts.RectangleToScreen(ts.ClientRectangle);

                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.X = rc.Right;
                            rc.Width = nHeight;
                            break;
                        default:
                            rc.X -= nHeight;
                            rc.Width = nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.Y = rc.Bottom;
                            rc.Height = nHeight;
                            break;
                        default:
                            rc.Y -= nHeight;
                            rc.Height = nHeight;
                            break;
                    }
                }
            }

            return rc;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetCaptionBounds(ToolStrip ts, Office12ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rc = Rectangle.Empty;

            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);
            if (nHeight > 0)
            {
                rc = ts.RectangleToScreen(ts.ClientRectangle);

                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.X = rc.Right;
                            rc.Width = nHeight;
                            break;
                        default:
                            rc.X -= nHeight;
                            rc.Width = nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.Y = rc.Bottom;
                            rc.Height = nHeight;
                            break;
                        default:
                            rc.Y -= nHeight;
                            rc.Height = nHeight;
                            break;
                    }
                }
            }

            return rc;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetCaptionBounds(ToolStrip ts, Office2010ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rc = Rectangle.Empty;

            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);
            if (nHeight > 0)
            {
                rc = ts.RectangleToScreen(ts.ClientRectangle);

                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.X = rc.Right;
                            rc.Width = nHeight;
                            break;
                        default:
                            rc.X -= nHeight;
                            rc.Width = nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.Y = rc.Bottom;
                            rc.Height = nHeight;
                            break;
                        default:
                            rc.Y -= nHeight;
                            rc.Height = nHeight;
                            break;
                    }
                }
            }

            return rc;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetLauncherBounds(ToolStrip toolStrip, Office12ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rcResult = Rectangle.Empty;

            if (toolStrip != null && toolStrip.IsHandleCreated)
            {
                if (ToolStripRendererUtils.HasLauncher(toolStrip))
                {
                    Rectangle rcCaption = ToolStripRendererUtils.GetCaptionBounds(toolStrip, renderType);
                    if (!rcCaption.IsEmpty)
                    {
                        bool bVertical = toolStrip.Dock == DockStyle.Left || toolStrip.Dock == DockStyle.Right;
                        rcResult = ToolStripRendererUtils.GetLauncherBounds(rcCaption, bVertical);
                    }
                }
            }

            return rcResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetLauncherBounds(ToolStrip toolStrip, Office2010ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rcResult = Rectangle.Empty;

            if (toolStrip != null && toolStrip.IsHandleCreated)
            {
                if (ToolStripRendererUtils.HasLauncher(toolStrip))
                {
                    Rectangle rcCaption = ToolStripRendererUtils.GetCaptionBounds(toolStrip, renderType);
                    if (!rcCaption.IsEmpty)
                    {
                        bool bVertical = toolStrip.Dock == DockStyle.Left || toolStrip.Dock == DockStyle.Right;
                        rcResult = ToolStripRendererUtils.GetLauncherBounds(rcCaption, bVertical);
                    }
                }
            }

            return rcResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool HasCaption(ToolStrip toolStrip, Office2013ToolStripRenderer.ERENDERTYPE renderType)
        {
            bool bResult = false;
            if (toolStrip != null)
            {
                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null)
                {
                    bResult = iRibbon.ShowCaption;
                }
                else
                {
                    bResult = toolStrip.GetType() == typeof(ToolStrip) && renderType != Office2013ToolStripRenderer.ERENDERTYPE.TabBar;
                }
            }

            return bResult;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static int GetCaptionHeight(ToolStrip toolStrip, Office2013ToolStripRenderer.ERENDERTYPE renderType)
        {
            int nResult = 0;

            if (ToolStripRendererUtils.HasCaption(toolStrip, renderType))
            {
                nResult = TextRenderer.MeasureText("X", ToolStripRendererUtils.GetCaptionFont(toolStrip)).Height;

                IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

                if (iRibbon != null && iRibbon.ShowLauncher && nResult < LAUNCHER_SIZE)
                {
                    nResult = LAUNCHER_SIZE;
                }

                IToolStripExSupport2 iToolStripEx2 = toolStrip as IToolStripExSupport2;

                if (iToolStripEx2 != null && iToolStripEx2.CaptionMinHeight > nResult)
                {
                    nResult = iToolStripEx2.CaptionMinHeight;
                }
            }

            return nResult;
        }

        public static Rectangle GetCaptionBounds(ToolStrip ts, Office2013ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rc = Rectangle.Empty;

            int nHeight = ToolStripRendererUtils.GetCaptionHeight(ts, renderType);
            if (nHeight > 0)
            {
                rc = ts.RectangleToScreen(ts.ClientRectangle);

                if (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right)
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.X = rc.Right;
                            rc.Width = nHeight;
                            break;
                        default:
                            rc.X -= nHeight;
                            rc.Width = nHeight;
                            break;
                    }
                }
                else
                {
                    switch (ToolStripRendererUtils.GetCaptionStyle(ts))
                    {
                        case CaptionStyle.Bottom:
                            rc.Y = rc.Bottom;
                            rc.Height = nHeight;
                            break;
                        default:
                            rc.Y -= nHeight;
                            rc.Height = nHeight;
                            break;
                    }
                }
            }

            return rc;
        }
        [Syncfusion.Documentation.DocumentationExclude()]
        public static Rectangle GetLauncherBounds(ToolStrip toolStrip, Office2013ToolStripRenderer.ERENDERTYPE renderType)
        {
            Rectangle rcResult = Rectangle.Empty;

            if (toolStrip != null && toolStrip.IsHandleCreated)
            {
                if (ToolStripRendererUtils.HasLauncher(toolStrip))
                {
                    Rectangle rcCaption = ToolStripRendererUtils.GetCaptionBounds(toolStrip, renderType);
                    if (!rcCaption.IsEmpty)
                    {
                        bool bVertical = toolStrip.Dock == DockStyle.Left || toolStrip.Dock == DockStyle.Right;
                        rcResult = ToolStripRendererUtils.GetLauncherBounds(rcCaption, bVertical);
                    }
                }
            }

            return rcResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool HasLauncher(ToolStrip toolStrip)
        {
            bool bResult = false;

            IToolStripExSupport iRibbon = toolStrip as IToolStripExSupport;

            if (iRibbon != null)
            {
                bResult = iRibbon.ShowCaption && iRibbon.ShowLauncher;
            }

            return bResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool ContainsCursor(ToolStripItem tsItem)
        {
            Rectangle itemRect = tsItem.GetCurrentParent().RectangleToScreen(tsItem.Bounds);
            bool result = itemRect.Contains(Cursor.Position);

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool GetIsPressed(ToolStripItem tsItem)
        {
            bool bResult = false;
            if (tsItem is ToolStripSplitButton)
            {
                ToolStripSplitButton tsSplitButton = tsItem as ToolStripSplitButton;
                bResult = tsSplitButton != null && tsSplitButton.ButtonPressed;
            }
            else //if (!(tsItem is ToolStripDropDownItem))
            {
                bResult = tsItem.Pressed && ContainsCursor(tsItem);
            }
            return bResult;
        }
        
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool GetIsChecked(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (tsItem is ToolStripButton)
            {
                ToolStripButton tsButtton = tsItem as ToolStripButton;
                if (tsButtton != null)
                {
                    bResult = tsButtton.Checked;
                }
            }
            else if (tsItem is ToolStripMenuItem)
            {
                ToolStripMenuItem tsMenuItem = tsItem as ToolStripMenuItem;
                if (tsMenuItem != null)
                {
                    bResult = tsMenuItem.Checked;
                }
            }
            else if (tsItem is ToolStripDropDownItem)
            {
                ToolStripDropDownItem tsddItem = tsItem as ToolStripDropDownItem;
                if (tsddItem != null)
                {
                    bResult = tsddItem == tsddItem.DropDown.OwnerItem && tsddItem.DropDown.Visible;
                }
            }
            return bResult;
        }
        
        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool GetIsSelected(ToolStripItem tsItem)
        {
            bool bResult = tsItem.Selected;
            if (!bResult && tsItem is ToolStripSplitButton)
            {
                ToolStripSplitButton tsButton = tsItem as ToolStripSplitButton;
                if (tsButton != null && tsButton.DropDownButtonPressed)
                {
                    bResult = true;
                }
            }
            return bResult;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static bool GetIsDisabled(ToolStripItem item)
        {
            return (!item.Enabled || !item.GetCurrentParent().Enabled);
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static CaptionTextStyle GetCaptionTextStyle(ToolStrip ts)
        {
            CaptionTextStyle result = CaptionTextStyle.Shadow;

            if (ts is IToolStripExSupport2)
            {
                result = (ts as IToolStripExSupport2).CaptionTextStyle;
            }

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static LauncherStyle GetLauncherStyle(ToolStrip ts)
        {
            LauncherStyle result = LauncherStyle.Office12;

            if (ts is IToolStripExSupport2)
            {
                result = (ts as IToolStripExSupport2).LauncherStyle;
            }

            return result;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static TextFormatFlags GetCaptionFormat(ToolStrip ts)
        {
            TextFormatFlags tf = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            IToolStripExSupport2 iToolStripEx = ts as IToolStripExSupport2;
            if (iToolStripEx != null)
            {
                if (ts is ToolStripEx)
                {
                    tf = tf | TextFormatFlags.NoPrefix;
                }
                switch (iToolStripEx.CaptionAlignment)
                {
                    case CaptionAlignment.Center:
                        return tf | TextFormatFlags.HorizontalCenter;
                    case CaptionAlignment.Far:
                        return tf | TextFormatFlags.Right;
                }
            }
            return tf | TextFormatFlags.Left;
        }

        [Syncfusion.Documentation.DocumentationExclude()]
        public static TextFormatFlags GetTextFormatFlags(ContentAlignment alignment)
        {
            TextFormatFlags flags;

            switch (alignment)
            {
                case ContentAlignment.BottomCenter:
                    flags = TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.BottomLeft:
                    flags = TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;

                case ContentAlignment.BottomRight:
                    flags = TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;

                case ContentAlignment.MiddleCenter:
                    flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.MiddleLeft:
                    flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;

                case ContentAlignment.MiddleRight:
                    flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;

                case ContentAlignment.TopCenter:
                    flags = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;

                case ContentAlignment.TopLeft:
                    flags = TextFormatFlags.Top | TextFormatFlags.Left;
                    break;

                case ContentAlignment.TopRight:
                    flags = TextFormatFlags.Top | TextFormatFlags.Right;
                    break;

                default:
                    flags = TextFormatFlags.Default;
                    break;
            }

            return flags;
        }

        #endregion
    }
    #endregion
    #region MetroToolStripRenderer
    /// <summary>
    /// MetroToolStripRenderer
    /// </summary>
    public partial class MetroToolStripRenderer : System.Windows.Forms.ToolStripProfessionalRenderer
    {
        #region Constants
        const int STATICEDGE_WIDTH = 1;
        const int ETCHED_WIDTH = 2;
        Color menuColor = ColorTranslator.FromHtml("#87CEFF");
        internal Color StripMetroColor
        {
            get
            {
                return menuColor;
            }
            set
            {
                if (menuColor != value)
                {
                    menuColor = value;
                }
            }
        }

        #endregion

        #region Constructors/destructors
        /// <summary>
        /// 
        /// </summary>
        static MetroToolStripRenderer()
        {
            m_blMenuBar = new Blend();
            m_blMenuBar.Positions = new float[] { 0.0F, 0.5F, 1.0F };
            m_blMenuBar.Factors = new float[] { 0.0F, 1.0F, 0.0F };

            m_blStatusBar = new Blend();
            m_blStatusBar.Positions = new float[] { 0.0F, 0.5F, 0.55F, 1.0F };
            m_blStatusBar.Factors = new float[] { 0.0F, 0.2F, 0.6F, 1.0F };

            m_blToolBar = new Blend();
            m_blToolBar.Positions = new float[] { 0.0F, 0.3F, 1.0F };
            m_blToolBar.Factors = new float[] { 1.0F, 0.0F, 1.0F };

            m_blMenuItemUp = new Blend();
            m_blMenuItemUp.Positions = new float[] { 0.0F, 0.4F, 0.5F, 1.0F };
            m_blMenuItemUp.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blMenuItemDown = new Blend();
            m_blMenuItemDown.Positions = new float[] { 0.0F, 0.3F, 0.32F, 1.0F };
            m_blMenuItemDown.Factors = new float[] { 1.0F, 0.5F, 0.4F, 0.0F };

            m_blGrouped = new Blend();
            m_blGrouped.Positions = new float[] { 0.0F, 0.4F, 0.4F, 1.0F };
            m_blGrouped.Factors = new float[] { 0.8F, 1.0F, 0.0F, 0.4F };

            m_blScrollButton = new Blend();
            m_blScrollButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blScrollButton.Factors = new float[] { 0.2F, 0.15F, 1.0F, 0.5F };

            m_blButtonSelected = new Blend();
            m_blButtonSelected.Positions = new float[] { 0.0F, 0.4F, 0.4F, 1.0F };
            m_blButtonSelected.Factors = new float[] { 0.2F, 0.0F, 0.8F, 0.2F };

            m_blButtonCollapsed = new Blend();
            m_blButtonCollapsed.Positions = new float[] { 0.0F, 0.15F, 0.15F, 1.0F };
            m_blButtonCollapsed.Factors = new float[] { 1.0F, 0.8F, 0.0F, 1.0F };

            m_blButtonShadow = new Blend();
            m_blButtonShadow.Positions = new float[] { 0.0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };
            m_blButtonShadow.Factors = new float[] { 0.0f, 0.3f, 0.6f, 0.8f, 0.95f, 1.0f };

            m_blImageBackground = new Blend();
            m_blImageBackground.Positions = new float[] { 0.0F, 0.8F, 0.8F, 1.0F };
            m_blImageBackground.Factors = new float[] { 0.0F, 0.2F, 1.0F, 1.0F };

            m_blButtonFlash = new Blend();
            m_blButtonFlash.Positions = new float[] { 0f, 0.4f, 1f };
            m_blButtonFlash.Factors = new float[] { 0f, 0.6f, 1f };

            m_blCaption = new Blend();
            m_blCaption.Positions = new float[] { 0.0F, 0.2F, 1.0F };
            m_blCaption.Factors = new float[] { 0.0F, 0.5F, 1.0F };

            m_blScrollerBackground = new Blend();
            m_blScrollerBackground.Positions = new float[] { 0.0F, 0.35F, 1.0F };
            m_blScrollerBackground.Factors = new float[] { 0.3F, 0.6F, 0.4F };

            m_blStandardScrollButton = new Blend();
            m_blStandardScrollButton.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blStandardScrollButton.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blScroller = new Blend();
            m_blScroller.Positions = new float[] { 0.0F, 0.5F, 0.5F, 1.0F };
            m_blScroller.Factors = new float[] { 0.3F, 0.0F, 1.0F, 0.4F };

            m_blGroupedBackGroundCollapsed = new Blend();
            m_blGroupedBackGroundCollapsed.Positions = new float[] { 0.0f, 0.5f, 0.5f, 0.92f, 0.92f, 1.0f };
            m_blGroupedBackGroundCollapsed.Factors = new float[] { 0.0f, 0.0f, 0.1f, 0.5f, 0.92f, 1.0f };

            m_blGroupedBackGround = new Blend();
            m_blGroupedBackGround.Positions = new float[] { 0.0f, 0.1f, 0.2f, 1.0f };
            m_blGroupedBackGround.Factors = new float[] { 0.6f, 0.8f, 0.9f, 0.95f };

            m_gCaption = Graphics.FromImage(new Bitmap(1, 1));
            m_gCaption.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            m_htControls = new Hashtable();
            m_controlsLock = new Object();
        }
        /// <summary>
        /// 
        /// </summary>
        public MetroToolStripRenderer()
            : this(new Color(), ERENDERTYPE.Normal)
        {

        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="erType"></param>
        /// <param name="colorTable"></param>
        public MetroToolStripRenderer(Color colorTable, ERENDERTYPE erType)
            : base()
        {
            RoundedEdges = false;

            m_eRenderType = erType;

            m_htPens = new Hashtable((int)EPEN.MAX);
            m_htBrushes = new Hashtable((int)EBRUSH.MAX);
            m_htBitmaps = new Hashtable((int)EBITMAP.MAX);

            //m_ComboBoxRenderer = new ComboBoxRenderer(this);
         
        }
        #endregion

        #region Caption Functions

        protected virtual bool HasCaption(ToolStrip toolStrip)
        {
            return ToolStripRendererUtils.HasCaption(toolStrip, this.RenderType);
        }

        protected virtual void DrawCaption(ToolStrip toolStrip)
        {
            if (HasCaption(toolStrip))
            {
                Rectangle rcUpdate = ToolStripRendererUtils.GetCaptionBounds(toolStrip, this.RenderType);
                if (rcUpdate.Width > 0 && rcUpdate.Height > 0)
                {
                    IntPtr hWnd = toolStrip.Handle;

                    RECT rcWindow = new RECT();
                    if (WindowsAPI.GetWindowRect(hWnd, ref rcWindow))
                    {
                        rcUpdate.X -= rcWindow.left;
                        rcUpdate.Y -= rcWindow.top;

                        IntPtr hdc = WindowsAPI.GetWindowDC(hWnd);

                        if (hdc != IntPtr.Zero)
                        {
                            if (toolStrip.Dock == DockStyle.Left || toolStrip.Dock == DockStyle.Right)
                            {
                                // Vertical orientation
                                Matrix m = new Matrix();
                                m.Rotate(-90f, MatrixOrder.Append);
                                m.Translate(rcUpdate.X, rcUpdate.Bottom, MatrixOrder.Append);

                                XFORM xform = new XFORM();
                                xform.eM11 = m.Elements[0];
                                xform.eM12 = m.Elements[1];
                                xform.eM21 = m.Elements[2];
                                xform.eM22 = m.Elements[3];
                                xform.eDx = m.Elements[4];
                                xform.eDy = m.Elements[5];

                                SetGraphicsMode(hdc, 2/*GM_ADVANCED*/);
                                SetWorldTransform(hdc, ref xform);

                                rcUpdate = new Rectangle(0, 0, rcUpdate.Height, rcUpdate.Width);
                            }

                            using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcUpdate))
                            {
                                Graphics g = bg.Graphics;

                                Color clTabGroup = Color.Empty;

                                bool bIsToolStripGrouped = IsGroupedToolStrip(toolStrip, ref clTabGroup);

                                using (SolidBrush brush = new SolidBrush(StripMetroColor))
                                {                                   
                                    g.FillRectangle(brush, rcUpdate);

                                }
                                Font f = ToolStripRendererUtils.GetCaptionFont(toolStrip);

                                Rectangle rcText = new Rectangle(0, 0, rcUpdate.Width, rcUpdate.Height);

                                if (ToolStripRendererUtils.HasLauncher(toolStrip))
                                {
                                    rcText.Width -= LAUNCHER_WIDTH;
                                }

                                TextFormatFlags tf = ToolStripRendererUtils.GetCaptionFormat(toolStrip);
                                Color clrText = this.CaptionText;

                                switch (ToolStripRendererUtils.GetCaptionTextStyle(toolStrip))
                                {
                                    case CaptionTextStyle.Shadow:
                                        {
                                            rcText.Offset(1, 1);

                                            TextRenderer.DrawText(g, toolStrip.Text, f, rcText, toolStrip.ForeColor, tf);
                                            rcText.Offset(-2, -2);
                                        }
                                        break;
                                    case CaptionTextStyle.Etched:
                                        {
                                            if (clrText.GetBrightness() < 0.9F)
                                            {
                                                rcText.Offset(1, 1);
                                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, Color.White, tf);
                                                rcText.Offset(-1, -1);
                                            }
                                            else
                                            {
                                                rcText.Offset(-1, -1);
                                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, Color.Black, tf);
                                                rcText.Offset(1, 1);
                                            }
                                        }
                                        break;
                                }

                                Color color = (toolStrip.ForeColor == Color.MidnightBlue) ? clrText : toolStrip.ForeColor;
                                TextRenderer.DrawText(g, toolStrip.Text, f, rcText, color, tf);
                                PaintLauncher(g, rcUpdate, toolStrip as IToolStripExSupport2);

                                bg.Render();
                            }
                            WindowsAPI.ReleaseDC(hWnd, hdc);
                        }
                    }
                }
            }
        }

        protected virtual void DrawBorders(ToolStrip ts)
        {
            switch (ToolStripRendererUtils.GetBorderStyle(ts))
            {
                case ToolStripBorderStyle.StaticEdge:
                    DrawStaticEdgeBorders(ts);
                    break;
                case ToolStripBorderStyle.Etched:
                    DrawEtchedBorders(ts);
                    break;
            }
        }

        protected virtual void DrawStaticEdgeBorders(ToolStrip ts)
        {
            IntPtr wnd = ts.Handle;

            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                if (hdc != IntPtr.Zero)
                {
                    Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        Color clBorder =  this.BorderColor;

                        using (Pen pen = new Pen(clBorder))//clBorder
                        {
                            Point[] pt = new Point[]
                            {
                                new Point(rc.X,rc.Bottom-2),
                                new Point(rc.X,rc.Y),
                                new Point(rc.Right-2,rc.Y)
                            };

                            g.DrawLines(pen, pt);
                        }

                        using (Pen pen = new Pen(Color.White))
                        {
                            Point[] pt = new Point[]
                            {
                                new Point(rc.Right-1,rc.Y),
                                new Point(rc.Right-1,rc.Bottom-1),
                                new Point(rc.X,rc.Bottom-1),
                            };

                            g.DrawLines(pen, pt);
                        }
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        protected virtual void DrawEtchedBorders(ToolStrip ts)
        {
            IntPtr wnd = ts.Handle;

            RECT rect = new RECT();
            if (WindowsAPI.GetWindowRect(wnd, ref rect))
            {
                IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                if (hdc != IntPtr.Zero)
                {
                    Rectangle rc = new Rectangle(0, 0, rect.Width, rect.Height);

                    WindowsAPI.ExcludeClipRect(hdc, rc.X + ETCHED_WIDTH, rc.Y + ETCHED_WIDTH, rc.Right - ETCHED_WIDTH, rc.Bottom - ETCHED_WIDTH);

                    using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rc))
                    {
                        Graphics g = bg.Graphics;
                        GraphicsState gState = g.Save();

                        using (Region region = RendererUtils.GetRoundedRegion(rc, 1))
                        {
                            Color clBorder = this.BorderColor;
                            Color clTabGroup = Color.Empty;
                            bool bIsGrouped = IsGroupedToolStrip(ts, ref clTabGroup);

                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            Color color = System.Drawing.ColorTranslator.FromHtml("#E7EEF6");
                            using (Brush brush = new SolidBrush(color))
                            {
                                g.FillRegion(brush, region);
                            }

                            if (bIsGrouped)
                            {
                                ToolStripEx tsEx = ts as ToolStripEx;
                                bool bSelected = GetIsSelected(ts);

                                clBorder = StripMetroColor;

                                if (tsEx != null && tsEx.State == ToolStripEx.ToolStripExState.Collapsed)
                                {
                                    // Collapsed ToolStripEx is selected and DropDown is not visible.
                                    if (bSelected && !tsEx.DropDownButton.DropDown.Visible)
                                    {
                                        using (Pen penGrouped = new Pen(GetGroupedBackgroundCollapsedBrush(Color.White, clTabGroup, rc.Height)))
                                        {
                                            g.DrawPolygon(penGrouped, RendererUtils.GetRoundedPolygon(Rectangle.Inflate(rc, -1, -1), 1));
                                        }
                                    }
                                }
                            }

                        }

                        g.Restore(gState);
                        bg.Render();
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        protected virtual Image GetLauncherImage(IToolStripExSupport2 iRibbon)
        {
            bool bSelected = iRibbon.LauncherSelected;

            switch (iRibbon.LauncherStyle)
            {
                case LauncherStyle.Metro:
                    return GetLauncherImage(bSelected ? EBITMAP.ebLauncher2007Selected : EBITMAP.ebLauncher2007);
            }

            return GetLauncherImage(bSelected ? EBITMAP.ebLauncherSelected : EBITMAP.ebLauncher);
        }

        protected virtual Image GetLauncherImage(EBITMAP eBitmap)
        {
            Image image = m_htBitmaps[eBitmap] as Image;
            if (image == null)
            {
                Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_WIDTH, LAUNCHER_HEIGHT);
                image = new Bitmap(rcImage.Width, rcImage.Height);

                using (Graphics g = Graphics.FromImage(image))
                {
                    g.Clear(Color.Transparent);

                    switch (eBitmap)
                    {
                        case EBITMAP.ebLauncher:
                        case EBITMAP.ebLauncherSelected:
                            {
                                Rectangle rc = Rectangle.Inflate(rcImage, -1, -1);
                                rc.Width += 1;
                                rc.Height += 1;

                                Point[] ptLauncher = GetLauncherPolygon(rc);

                                using (GraphicsPath path = new GraphicsPath())
                                {
                                    path.AddPolygon(ptLauncher);
                                    using (Region rgLauncher = new Region(path))
                                    {
                                        g.Clip = rgLauncher;

                                        using (Brush brush = new SolidBrush(this.LauncherBackground))
                                        {
                                            g.FillRegion(brush, rgLauncher);
                                        }

                                        rc.X += rc.Width / 2 - 1;
                                        rc.Y += 1;
                                        rc.Width /= 2;
                                        rc.Height /= 2;

                                        using (Region region = new Region(new Rectangle(rc.X + rc.Width / 2 - 1, rc.Y, 2, rc.Height)))
                                        {
                                            region.Union(new Rectangle(rc.X, rc.Y + rc.Height / 2 - 1, rc.Width, 2));

                                            Color color = (eBitmap == EBITMAP.ebLauncher) ? this.LauncherText : this.LauncherTextSelected;

                                            using (SolidBrush brush = new SolidBrush(color))
                                            {
                                                g.FillRegion(brush, region);
                                            }
                                        }
                                        g.DrawPolygon(LauncherBorder, ptLauncher);
                                    }
                                }
                            }
                            break;
                        case EBITMAP.ebLauncher2007:
                        case EBITMAP.ebLauncher2007Selected:
                            {
                                if (eBitmap == EBITMAP.ebLauncher2007Selected)
                                {
                                    g.DrawRectangle(this.LauncherBorder, rcImage);

                                    Rectangle rcBackground = Rectangle.Inflate(rcImage, -1, -1);
                                    
                                    using (SolidBrush brush = new SolidBrush(StripMetroColor))
                                    {
                                        g.FillRectangle(brush, rcBackground);
                                    }
                                }

                                Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                                rc.Offset(1, 1);

                                using (Region region = new Region(rc))
                                {
                                    region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                                    region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                                    region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                                    region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                                    region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                                    region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                                    g.FillRegion(Brushes.White, region);

                                    region.Translate(-1, -1);

                                    Color color = eBitmap == EBITMAP.ebLauncher2007 ? this.LauncherText : this.LauncherTextSelected;

                                    using (SolidBrush brush = new SolidBrush(color))
                                    {
                                        g.FillRegion(brush, region);
                                    }
                                }
                            }
                            break;
                    }
                }

                m_htBitmaps[eBitmap] = image;
            }
            return image;
        }

        protected virtual void PaintLauncher(Graphics g, Rectangle rcCaption, IToolStripExSupport2 iRibbon)
        {
            if (iRibbon != null && iRibbon.ShowLauncher)
            {
                Rectangle rc = ToolStripRendererUtils.GetLauncherBounds(rcCaption, false);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Image image = GetLauncherImage(iRibbon);

                    if (image != null)
                    {
                        g.DrawImage(image, rc.Location);
                    }
                }
            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            if (toolStrip != null)
            {
                Attach(toolStrip);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                base.OnRenderToolStripBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBorder(e))
            {
                base.OnRenderToolStripBorder(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
            {
                base.OnRenderItemText(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemImage(e))
            {
                base.OnRenderItemImage(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemCheck(e))
            {
                base.OnRenderItemCheck(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
            {
                base.OnRenderButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
            {
                base.OnRenderDropDownButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
            {
                base.OnRenderOverflowButtonBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintArrow(e))
            {
                base.OnRenderArrow(e);
            }
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHandleCreated(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                AssignHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHandleDestroyed(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                ReleaseHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRendererChanged(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && ts.Renderer != this)
            {
                Detach(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDockChanged(object sender, EventArgs e)
        {
            ToolStripRendererUtils.UpdateFrame(sender as ToolStrip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLayoutCompleted(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && HasCaption(ts))
            {
                if (ts.AutoSize)
                {
                    bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;

                    int nMaxBound = bVertical ? ts.Padding.Left : ts.Padding.Top;

                    ToolStripItemCollection items = ts.Items;

                    for (int i = 0, count = items.Count; i < count; i++)
                    {
                        ToolStripItem item = items[i];
                        if (item.Placement == ToolStripItemPlacement.Main)
                        {
                            int nItemBound = bVertical ? item.Bounds.Right : item.Bounds.Bottom;
                            if (nItemBound > nMaxBound)
                            {
                                nMaxBound = nItemBound;
                            }
                        }
                    }

                    if (bVertical)
                    {
                        nMaxBound += ts.Padding.Right + ToolStripRendererUtils.GetCaptionHeight(ts, this.RenderType);

                        if (nMaxBound != ts.MinimumSize.Width)
                        {
                            ts.MinimumSize = new Size(nMaxBound, ts.MinimumSize.Height);
                        }
                    }
                    else
                    {
                        nMaxBound += ts.Padding.Bottom + ToolStripRendererUtils.GetCaptionHeight(ts, this.RenderType);

                        if (nMaxBound != ts.MinimumSize.Height)
                        {
                            ts.MinimumSize = new Size(ts.MinimumSize.Width, nMaxBound);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemAdded(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripComboBox)
            {
                OnAddedComboBox(e.Item as ToolStripComboBox);
            }
            else if (e.Item is ToolStripDropDownItem)
            {
                OnAddedDropDownItem(e.Item as ToolStripDropDownItem);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemRemoved(object sender, ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripComboBox)
            {
                OnRemovedComboBox(e.Item as ToolStripComboBox);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnComboBoxHandleCreated(object sender, EventArgs e)
        {
            //SubclassControl(sender as Control, m_ComboBoxRenderer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnComboBoxHandleDestroyed(object sender, EventArgs e)
        {
            ReleaseControl(sender as Control);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRoundedToolstripRegionChanged(object sender, EventArgs e)
        {
            ToolStrip toolstrip = sender as ToolStrip;

            if (toolstrip != null && toolstrip.IsHandleCreated)
            {
                toolstrip.Region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, toolstrip.Size), TOOLSTRIP_RADIUS);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Attach(ToolStrip ts)
        {
            Subscribe(ts);

            m_refCounter++;

            if (m_refCounter == 1)
            {
                SystemInfo.SettingsChanged += new SettingsChangedEventHandler(SettingsChangedHandler);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Detach(ToolStrip ts)
        {
            Unsubscribe(ts);

            if (m_refCounter > 0)
            {
                m_refCounter--;

                if (m_refCounter == 0)
                {
                    SystemInfo.SettingsChanged -= new SettingsChangedEventHandler(SettingsChangedHandler);
                    Clear();
                }
            }
        }

        #region Attach/Detach items
        void AttachItems(ToolStrip ts)
        {
            ts.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);
            ts.ItemRemoved += new ToolStripItemEventHandler(OnItemRemoved);

            foreach (ToolStripItem item in ts.Items)
            {
                OnItemAdded(ts, new ToolStripItemEventArgs(item));
            }
        }
        void DetachItems(ToolStrip ts)
        {
            ts.ItemAdded -= new ToolStripItemEventHandler(OnItemAdded);
            ts.ItemRemoved -= new ToolStripItemEventHandler(OnItemRemoved);

            if (!(ts.Renderer is MetroToolStripRenderer))//Office12ToolStripRenderer
            {
                foreach (ToolStripItem item in ts.Items)
                {
                    OnItemRemoved(ts, new ToolStripItemEventArgs(item));
                }
            }
        }
        #endregion

        void Subscribe(ToolStrip ts)
        {
            ts.RendererChanged += new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated += new EventHandler(OnHandleCreated);
                ts.HandleDestroyed += new EventHandler(OnHandleDestroyed);
                ts.DockChanged += new EventHandler(OnDockChanged);

                if (ts.IsHandleCreated)
                {
                    OnHandleCreated(ts, EventArgs.Empty);
                }

                // Workaround to fix layouting problem in standard ToolStrip controls
                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted += new EventHandler(OnLayoutCompleted);
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated += new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged += new EventHandler(OnRoundedToolstripRegionChanged);
            }

            AttachItems(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Unsubscribe(ToolStrip ts)
        {
            DetachItems(ts);

            ts.RendererChanged -= new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated -= new EventHandler(OnHandleCreated);
                ts.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
                ts.DockChanged -= new EventHandler(OnDockChanged);

                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
                }

                if (!(ts.Renderer is MetroToolStripRenderer))
                {
                    ReleaseHandle(ts);
                    ClearDropDownRegions(ts);
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated -= new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged -= new EventHandler(OnRoundedToolstripRegionChanged);

                if (!(ts.Renderer is MetroToolStripRenderer))
                {
                    ts.Region = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void AssignHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                if (!m_htControls.ContainsKey(ts))
                {
                    CToolStripWindow wnd = new CToolStripWindow(ts);

                    wnd.AssignHandle(ts.Handle);
                    m_htControls.Add(ts, wnd);
                }
            }

            ts.Region = ToolStripRendererUtils.GetRegion(ts);

            ToolStripRendererUtils.UpdateFrame(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void ReleaseHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                object obj = m_htControls[ts];
                if (obj != null)
                {
                    CToolStripWindow wnd = obj as CToolStripWindow;

                    if (wnd != null)
                    {
                        wnd.ReleaseHandle();
                    }

                    m_htControls.Remove(ts);
                }
            }

            if (!(ts.Renderer is MetroToolStripRenderer))
            {
                ts.Region = null;
            }

            ToolStripRendererUtils.UpdateFrame(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        void SubclassControl(Control control, INativeMessageFilter messageFilter)
        {
            lock (m_controlsLock)
            {
                NativeMessageHandler handler = m_htControls[control] as NativeMessageHandler;

                if (handler == null)
                {
                    handler = new NativeMessageHandler();
                    m_htControls.Add(control, handler);

                    handler.Assign(control.Handle);
                }

                handler.MessageFilter = messageFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        void ReleaseControl(Control control)
        {
            lock (m_controlsLock)
            {
                object obj = m_htControls[control];
                if (obj != null)
                {
                    NativeMessageHandler handler = obj as NativeMessageHandler;

                    if (handler != null)
                    {
                        handler.MessageFilter = null;
                    }

                    m_htControls.Remove(control);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripComboBox"></param>
        void OnAddedComboBox(ToolStripComboBox toolStripComboBox)
        {
            Control comboBox = toolStripComboBox.Control;

            if (comboBox != null)
            {
                comboBox.HandleCreated += new EventHandler(OnComboBoxHandleCreated);
                comboBox.HandleDestroyed += new EventHandler(OnComboBoxHandleDestroyed);

                if (comboBox.IsHandleCreated)
                {
                    //SubclassControl(comboBox, m_ComboBoxRenderer);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripComboBox"></param>
        void OnRemovedComboBox(ToolStripComboBox toolStripComboBox)
        {
            Control comboBox = toolStripComboBox.Control;

            if (comboBox != null)
            {
                comboBox.HandleCreated -= new EventHandler(OnComboBoxHandleCreated);
                comboBox.HandleDestroyed -= new EventHandler(OnComboBoxHandleDestroyed);

                if (comboBox.IsHandleCreated)
                {
                    ReleaseControl(comboBox);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripDropDownItem"></param>
        void OnAddedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
        {
            AttachItems(toolStripDropDownItem.DropDown);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStripDropDownItem"></param>
        void OnRemovedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
        {
            DetachItems(toolStripDropDownItem.DropDown);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        private void ClearDropDownRegions(ToolStrip toolStrip)
        {
            ToolStripItemCollection items = toolStrip.Items;

            for (int i = 0, count = items.Count; i < count; i++)
            {
                ToolStripDropDownItem item = items[i] as ToolStripDropDownItem;
                if (item != null && item.DropDown != null && item.Placement != ToolStripItemPlacement.None)
                {
                    ClearDropDownRegions(item.DropDown);
                    item.DropDown.Region = null;
                }
            }

            ToolStripDropDownItem overflowButton = toolStrip.OverflowButton;
            if (overflowButton != null && overflowButton.DropDown != null)
            {
                overflowButton.DropDown.Region = null;
            }
        }

        private bool PaintToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is BottomToolstrip)
            {
                bResult = PaintBottomToolstripBackground(e);
            }
            else if (e.ToolStrip is ContextMenuStripEx)
            {
                bResult = PaintContextMenuBackground(e);
            }
            else if (e.ToolStrip is MenuStrip)
            {
                bResult = PaintMenuBackground(e);
            }
            else if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                bResult = PaintToolStripGalleryDropDownBackground(e);
            }
            else
                if (e.ToolStrip is StatusStrip)
                {
                    bResult = PaintStatusStripBackground(e);
                }
                else
                    if (e.ToolStrip is ToolStripDropDown)
                    {
                        bResult = PaintDropdownBackground(e);
                    }
                    else
                    {
                        bResult = PaintGroupedToolbarBackground(e) || PaintToolbarBackground(e);
                    }

            return bResult;
        }

        private bool PaintToolStripBorder(ToolStripRenderEventArgs e)
        {
            bool bResult = true;

            if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                if (e.ToolStrip.IsDropDown)
                {
                    e.Graphics.DrawPolygon(MenuBorder, RendererUtils.GetRoundedPolygon(rc, TOOLSTRIP_RADIUS));
                }
                else if (e.ToolStrip is MenuStrip)
                {
                    e.Graphics.DrawLine(MenuStripBorder, new Point(rc.X, rc.Bottom - 1), new Point(rc.Right - 1, rc.Bottom - 1));
                }
                else
                {
                    switch (GetRenderType(e.ToolStrip))
                    {
                        case ERENDERTYPE.Grouped:
                            PaintGroupBorders(e);
                            break;
                    }
                }
            }
            return bResult;
        }

        private bool PaintDropdownBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripDropDown ts = e.ToolStrip as ToolStripDropDown;
            if (ts != null)
            {
                if (ts.Width > 0 && ts.Height > 0)
                {
                    Rectangle rc = new Rectangle(Point.Empty, ts.Size);
                    if (!RendererUtils.IsValidRegion(ts, e.Graphics))
                    {
                        ts.Region = RendererUtils.GetRoundedRegion(rc, TOOLSTRIP_RADIUS);
                    }

                    e.Graphics.FillRectangle(ToolStripDropDownBackground, rc);
                }
                bResult = true;
            }

            return bResult;
        }

        private bool PaintMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStrip ms = e.ToolStrip;
            if (ms != null)
            {
                Size szMenu = ms.Size;
                if (szMenu.Width > 0 && szMenu.Height > 0)
                {
                    Rectangle rcMenu = new Rectangle(Point.Empty, szMenu);                 

                    Control parent = ms.Parent;
                    if (parent != null)
                    {
                        Size szParent = parent.ClientSize;
                        if (szParent.Width > 0 && szParent.Height > 0)
                        {
                            Rectangle rcParent = new Rectangle(Point.Empty, szParent);
                            using (SolidBrush brush = new SolidBrush(StripMetroColor))                          
                            {                              
                                e.Graphics.FillRectangle(brush, rcMenu);
                            }
                        }
                    }
                    else
                    {
                        using (SolidBrush brush =new SolidBrush(StripMetroColor))                       
                        {                         
                            e.Graphics.FillRectangle(brush, rcMenu);
                        }
                    }
                    bResult = true;
                }
            }

            return bResult;
        }

        private bool PaintStatusStripBackground(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
               
                using (SolidBrush brush = new SolidBrush(StripMetroColor))               
                {                  
                    e.Graphics.FillRectangle(brush, rc);
                }
            }

            return true;
        }

        private bool PaintToolbarBackground(ToolStripRenderEventArgs e)
        {
            ToolStrip ts = e.ToolStrip;
            if (ts != null)
            {
                if (ts.Width > 0 && ts.Height > 0)
                {
                    Rectangle rc = new Rectangle(Point.Empty, ts.ClientSize);

                    if (RenderType == ERENDERTYPE.TabBar)
                    {                      
                        using (SolidBrush brush = new SolidBrush(StripMetroColor))                       
                        {                           
                            e.Graphics.FillRectangle(brush, rc);
                        }
                    }                    
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        private bool PaintGroupedToolbarBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStrip ts = e.ToolStrip;

            if (ts != null && ts.Width > 0 && ts.Height > 0)
            {
                Color clTabGroup = Color.White;

                bool bIsGrouped = IsGroupedToolStrip(e.ToolStrip, ref clTabGroup);

                if (bIsGrouped)
                {
                    bool bRounded = RoundedEdges && e.ToolStrip.Dock == DockStyle.None;
                    Rectangle rc = new Rectangle(Point.Empty, ts.ClientSize);

                    using (Region region = RendererUtils.GetRoundedRegion(rc, bRounded ? TOOLSTRIP_RADIUS : 0))
                    {                     
                        using (SolidBrush brush = new SolidBrush(StripMetroColor))                      
                        {
                            e.Graphics.FillRegion(brush, region);
                        }
                    }

                    bResult = true;
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintContextMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            RECT rect = new RECT();

            ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

            if (menuStrip != null)
            {
                if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                        Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                        Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

                        using (Brush brush = new SolidBrush(StripMetroColor))
                        {
                            e.Graphics.FillRectangle(brush, rectangleBody);
                        }

                        using (Brush brush = new SolidBrush(StripMetroColor))
                        {
                            e.Graphics.FillRectangle(brush, rectangleTitle);
                        }

                        using (Pen pen = new Pen(Color.FromArgb(60, Color.Black)))
                        {
                            //e.Graphics.DrawLine(
                            //    pen, rectangleTitle.Left + 3, rectangleTitle.Bottom, rectangleTitle.Right - 3, rectangleTitle.Bottom);
                        }

                        Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

                        TextFormatFlags flags = TextFormatFlags.EndEllipsis;

                        if (menuStrip.RightToLeft == RightToLeft.Yes)
                        {
                            flags |= TextFormatFlags.Right;
                        }

                        TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, Color.Black, flags);

                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintBottomToolstripBackground(ToolStripRenderEventArgs e)
        {
            BottomToolstrip toolstrip = e.ToolStrip as BottomToolstrip;

            if (toolstrip != null)
            {
                using (Brush brush = new SolidBrush(StripMetroColor))
                {
                    Rectangle rect = new Rectangle(new Point(0, 0), toolstrip.Size);                   
                    rect.Inflate(-1, -1);
                    using (Pen p = new Pen(Color.FromArgb(150, Color.White)))
                    {
                        e.Graphics.DrawPolygon(p, RendererUtils.GetRoundedPolygon(rect, TOOLSTRIP_RADIUS));
                    }
                    rect.X--;
                    rect.Y--;
                    using (Pen p = new Pen(Color.FromArgb(40, Color.Black)))
                    {
                        e.Graphics.DrawPolygon(p, RendererUtils.GetRoundedPolygon(rect, TOOLSTRIP_RADIUS));
                    }
                }

                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item.IsOnDropDown)
            {
                bResult = PaintDropDownMenuItemBackground(e);
            }
            else
            {
                bResult = PaintToolStripMenuItemBackground(e);
            }
            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

            if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
            {
                rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
            }

            if (e.ToolStrip is ContextMenuStripEx)
            {
                ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                rc.Y += statusStrip.TitleHeight;
                rc.Height -= statusStrip.TitleHeight;
            }

            using (Brush brush = new SolidBrush(StripMetroColor))//ContextMenu backcolor
            {
                g.FillRectangle(brush, rc);
            }

            int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
            int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);
            g.DrawLine(Pens.White, beginX, rc.Top, beginX, rc.Bottom - 1);
            g.DrawLine(Pens.DarkGray, endX, rc.Top, endX, rc.Bottom - 1);

            return true;
        }

        private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripItem tsItem = e.Item;
            if (tsItem.Enabled)
            {
                if (tsItem.Selected || tsItem.Pressed)
                {
                    Rectangle rc = GetItemRect(tsItem);

                    using (Region rgnItem = RendererUtils.GetRoundedRegion(rc, MENUITEM_RADIUS))
                    {
                        Graphics g = e.Graphics;

                        using (SolidBrush brush = new SolidBrush(StripMetroColor))                      
                        {
                            g.FillRegion(brush, rgnItem);
                        }

                    }

                    bResult = true;
                }
            }

            return bResult;
        }

        private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

            if (tsItem != null && tsItem.Selected || tsItem.Pressed)
            {
                Rectangle rc = GetItemRect(tsItem);

                if (tsItem.Enabled)
                {
                    using (Region rgBackground = RendererUtils.GetRoundedRegion(rc, MENUITEM_RADIUS))
                    {
                        Graphics g = e.Graphics;
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, StripMetroColor)))                       
                        {
                            e.Graphics.FillRectangle(brush, rc);
                        }
                    }                   
                }
                else
                {
                    using (Pen pen = new Pen(StripMetroColor))
                    {
                        e.Graphics.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                    }
                }

                bResult = true;
            }
            return bResult;
        }

        private bool PaintArrow(ToolStripArrowRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                Point loc = new Point();
                loc.Y = item.Height - item.Padding.Bottom - GetDownArrowSize().Height;
                loc.X = (item.Width - item.Padding.Horizontal - this.ArrowDownImage.Width) / 2 + item.Padding.Left;

                e.Graphics.DrawImage(this.ArrowDownImage, loc);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                Rectangle rc = ToolStripRendererUtils.GetTextRect(e);

                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, e.Item.ForeColor, e.TextFormat);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;
                if (tsItem != null && tsItem.Checked && !ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
                {
                    PaintItemCheckBackground(e);
                }
            }

            Image image = e.Image;
            Rectangle rc = ToolStripRendererUtils.GetImageRect(e);

            if (image != null && rc.Width > 0 && rc.Height > 0)
            {
                bool bDisposeImage = false;

                if (!e.Item.Enabled)
                {
                    image = CreateDisabledImage(image);
                    bDisposeImage = true;
                }

                Graphics g = e.Graphics;

                if (e.Item is CollapsedDropDownButton)
                {
                    Rectangle borderRect = rc;
                    borderRect.Inflate(ToolStripEx.DEF_IMAGE_BORDER_OFFSET, ToolStripEx.DEF_IMAGE_BORDER_OFFSET);
                    bool bIsGroupedToolStrip = IsGroupedToolStrip(e.ToolStrip);
                   
                    using (SolidBrush brush = new SolidBrush(StripMetroColor))                  
                    {
                        g.FillRectangle(brush, rc);                        
                    }                    
                }

                if (e.Item is OfficeButton)
                {
                    if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        rc.X -= 1;
                    }
                    else
                    {
                        rc.X += 1;
                    }
                }

                if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                {
                    Size szImage = image.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    g.DrawImage(image, new Point(iImageX, iImageY));
                }
                else
                {
                    InterpolationMode interpolation = g.InterpolationMode;

                    if (image.Size != rc.Size)
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    g.DrawImage(image, rc);

                    g.InterpolationMode = interpolation;
                }

                if (bDisposeImage)
                {
                    image.Dispose();
                }
            }

            return true;
        }

        private bool PaintItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            ToolStripItem tsItem = e.Item;

            if (tsItem.Image == null || ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
            {
                PaintItemCheckBackground(e);

                Rectangle rc = GetCheckRect(e);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Size szImage = this.CheckButton.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
                }
            }

            return true;
        }

        private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = GetCheckRect(e);
            if (rc.Width > 0 && rc.Height > 0)
            {
                Graphics g = e.Graphics;

                GraphicsState gState = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;

                g.FillRectangle(this.CheckBackground, Rectangle.Inflate(rc, -1, -1));
                g.DrawPolygon(this.CheckBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));

                g.Restore(gState);
            }
        }

        private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintTabBarButtonBackground(e.Item, e.Graphics, GetButtonRect(e.Item)))
            {
                if (!PaintGroupedButtonBackground(e))
                {
                    if (!PaintCollapsedButtonBackground(e))
                    {
                        PaintToolBarButtonBackground(e);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintToolBarButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintButtonBackgroundPressed(e))
            {
                if (!PaintButtonBackgroundChecked(e))
                {
                    PaintButtonBackgroundSelected(e);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (GetIsGrouped(e.Item))
            {
                if (!PaintGroupedBackgroundPressed(e))
                {
                    if (!PaintGroupedBackgroundChecked(e))
                    {
                        if (!PaintGroupedBackgroundSelected(e))
                        {
                            PaintGroupedBackgroundNormal(e);
                        }
                    }
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool PaintTabBarButtonBackground(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (GetIsTabItem(item))
            {
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (!PaintTabBackgroundPressed(item, g, rc))
                    {
                        if (!PaintTabBackgroundChecked(item, g, rc))
                        {
                            PaintTabBackgroundSelected(item, g, rc);
                        }
                    }
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintCollapsedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                if (!PaintCollapsedBackgroundPressed(e))
                {
                    if (!PaintCollapsedBackgroundSelected(e))
                    {
                        PaintCollapsedBackgroundNormal(e);
                    }
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                  
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);
                    
                    using (SolidBrush brush = new SolidBrush(StripMetroColor))                   
                    {
                        g.FillRectangle(brush, rcBackground);
                    }                  
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;                    
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintBackgroundSelected(g, rcBackground, StripMetroColor); 
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintButtonBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintBackgroundSelected(g, rcBackground, StripMetroColor);
                }

                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintBackgroundSelected(g, rcBackground, StripMetroColor);                   
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintBackgroundSelected(g, rcBackground, StripMetroColor);             
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintGroupedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Graphics g = e.Graphics;
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    PaintBackgroundSelected(g, rcBackground, StripMetroColor);                    
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintGroupedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = GetButtonRect(e.Item);
            if (rc.Width > 0 && rc.Height > 0)
            {
                PaintBackgroundGrouped(e.Graphics, GetButtonBackgroundRect(e.Item, rc));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool PaintTabBackgroundPressed(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (item.Pressed)
            {
                Rectangle rcTab = rc;
                rcTab.Height -= 1;

                using (Region region = GetTabbedRegion(rcTab))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    PaintBackgroundSelected(g, rcTab, StripMetroColor);
                    PaintTabBorder(g, rcTab, EBUTTONSTATE.Pressed);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintTabBackgroundChecked(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(item))
            {
                using (Region region = GetTabbedRegion(rc))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    PaintBackgroundSelected(g, rc, StripMetroColor);
                    PaintTabBorder(g, rc, EBUTTONSTATE.Checked);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintTabBackgroundSelected(ToolStripItem item, Graphics g, Rectangle rc)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rcTab = rc;
                rcTab.Height -= 1;

                using (Region region = GetTabbedRegion(rcTab))
                {
                    GraphicsState gState = g.Save();
                    g.SetClip(region, CombineMode.Intersect);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    PaintBackgroundSelected(g, rcTab, StripMetroColor);
                    PaintTabBorder(g, rcTab, EBUTTONSTATE.Selected);

                    g.Restore(gState);
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (IsGroupedToolStrip(e.ToolStrip))
                    {
                        e.Graphics.FillRectangle(Brushes.White, rc);
                    }
                    else
                    {
                        PaintBackgroundCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), StripMetroColor);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item))
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    if (IsGroupedToolStrip(e.ToolStrip))
                    {
                        using (Brush brushGroupedPressed = new SolidBrush(e.ToolStrip.BackColor))
                        {
                            e.Graphics.FillRectangle(brushGroupedPressed, rc);
                        }
                    }
                    else
                    {
                        PaintBackgroundCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), StripMetroColor);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private bool PaintCollapsedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {
            Rectangle rc = GetButtonRect(e.Item);
            if (rc.Width > 0 && rc.Height > 0)
            {
                if (IsGroupedToolStrip(e.ToolStrip))
                {
                    e.Graphics.FillRectangle(Brushes.White, rc);
                }
                else
                {
                    PaintBackgroundCollapsed(e.Graphics, GetButtonBackgroundRect(e.Item, rc), StripMetroColor);
                }
            }

            return true;
        }

        private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            return PaintButtonBackground(e);
        }

        private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;
            ToolStripSplitButton tsButton = e.Item as ToolStripSplitButton;

            if (tsButton != null)
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);
                    if (rcBackground.Width > 0 && rcBackground.Height > 0)
                    {
                        Graphics g = e.Graphics;
                        ToolStrip ts = e.ToolStrip;

                        bool bIsDisabled = ToolStripRendererUtils.GetIsDisabled(e.Item);

                        bool bGrouped = GetIsGrouped(e.Item);
                        bool bDropDownPressed = tsButton.DropDown.Visible && (tsButton.DropDown.OwnerItem == tsButton) && !bIsDisabled;
                        bool bSelected = (tsButton.Selected || bDropDownPressed) && !bIsDisabled;

                        Rectangle rcDropDown = Rectangle.Intersect(tsButton.DropDownButtonBounds, rcBackground);
                        Rectangle rcButton = rcBackground;

                        rcButton.Width -= rcDropDown.Width;
                        if (e.Item.RightToLeft == RightToLeft.Yes)
                        {
                            rcButton.X += rcDropDown.Width;
                        }

                        if (rcButton.Width > 0 && rcButton.Height > 0)
                        {
                            if (bSelected)
                            {
                                PaintBackgroundSelected(g, rcButton,StripMetroColor);
                                g.DrawLine(Pens.DarkGray, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1);
                            }
                            else if (bGrouped)
                            {
                                PaintBackgroundGrouped(g, rcButton);
                            }
                        }

                        if (rcDropDown.Width > 0 && rcDropDown.Height > 0)
                        {
                            GraphicsState gState = g.Save();
                            g.SetClip(rcDropDown);

                            if (bDropDownPressed)
                            {
                                PaintBackgroundSelected(g, rcDropDown, StripMetroColor);                               
                            }
                            else if (tsButton.DropDownButtonSelected && !bIsDisabled)
                            {
                                PaintBackgroundSelected(g, rcDropDown,StripMetroColor);                               
                            }
                            else if (bGrouped)
                            {
                                PaintBackgroundGrouped(g, rcDropDown);
                            }

                            Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                            Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                            bool bVertical = ts != null && (ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right);
                            ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                            base.DrawArrow(new ToolStripArrowRenderEventArgs(g, e.Item, rcArrow, color, dir));

                            g.Restore(gState);
                        }
                    }
                }
                bResult = true;
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal void PaintSplitButtonExBackground(ToolStripItemRenderEventArgs e)
        {
            ToolStripSplitButtonEx tsButton = e.Item as ToolStripSplitButtonEx;

            if (tsButton != null)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    Rectangle rcBackground = GetButtonBackgroundRect(e.Item, rc);

                    if (rcBackground.Width > 0 && rcBackground.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        bool bIsDisabled = ToolStripRendererUtils.GetIsDisabled(e.Item);
                        bool bDropDownPressed = tsButton.DropDown.Visible && (tsButton.DropDown.OwnerItem == tsButton) && !bIsDisabled;
                        bool bSelected = tsButton.Selected && !bIsDisabled;

                        Rectangle rcButton = Rectangle.Intersect(tsButton.ImageBounds, rcBackground);
                        Rectangle rcDropDown = Rectangle.Intersect(tsButton.ButtonBounds, rcBackground);

                        EBUTTONSTATE eImageState, eButtonState;
                        Rectangle rcImageBorder, rcButtonBorder;
                        Bitmap bmpImage, bmpButton;
                        Pen penImage;
                        Pen penButton;
                        bool bPaint = true;

                        // We need to initialize variables for further use.
                        // So initialize them as ImageSelectedButtonSelectedInActive state.
                        rcImageBorder = rcButtonBorder = Rectangle.Empty;
                        eImageState = eButtonState = EBUTTONSTATE.Selected;

                        bmpButton = bmpImage = this.SelectedFlashImage;
                        penImage = this.ButtonSelectedBorder;
                        penButton = this.ButtonHighlightBorder;

                        switch (tsButton.ButtonState)
                        {
                            case ToolStripSplitButtonEx.SplitButtonState.ImageSelected:
                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ButtonSelected:

                                eButtonState = eImageState = EBUTTONSTATE.Selected;

                                bmpButton = bmpImage = this.SelectedFlashImage;
                                penImage = this.ButtonSelectedBorder;
                                penButton = this.ButtonHighlightBorder;

                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ImagePressed:

                                eImageState = EBUTTONSTATE.Pressed;
                                eButtonState = EBUTTONSTATE.Selected;

                                bmpImage = this.PressedFlashImage;
                                bmpButton = this.SelectedFlashImage;

                                penImage = this.ButtonPressedBorder;
                                penButton = this.ButtonHighlightBorder;

                                rcImageBorder = tsButton.ImageBounds;
                                rcButtonBorder = tsButton.ButtonBounds;

                                break;

                            case ToolStripSplitButtonEx.SplitButtonState.ButtonPressed:


                                eImageState = EBUTTONSTATE.Selected;
                                eButtonState = EBUTTONSTATE.Pressed;

                                bmpImage = this.SelectedFlashImage;
                                bmpButton = this.PressedFlashImage;

                                penImage = this.ButtonHighlightBorder;
                                penButton = this.ButtonPressedBorder;

                                rcImageBorder = tsButton.ImageBounds;
                                rcButtonBorder = tsButton.ButtonBounds;

                                break;

                            default:

                                // SplitButtonState is None, so don't paint the item.
                                bPaint = false;

                                break;
                        }


                        if (bPaint)
                        {
                            bool bImage = (tsButton.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image;
                            bool bDrawButton = (rcButton.Width > 0 && rcButton.Height > 0);
                            bool bDrawDropDown = (rcDropDown.Width > 0 && rcDropDown.Height > 0);

                            // Draw Gradient background, shadow, flash images and border
                            // for Image button and Split button on SplitButtonEx.
                            if (bDrawButton)
                            {
                                PaintBackgroundSelected(g, rcButton, StripMetroColor);                             
                            }

                            if (bDrawDropDown)
                            {
                                PaintBackgroundSelected(g, rcDropDown, StripMetroColor);                                          
                            }

                            if (bImage && bDrawButton && bDrawDropDown)
                            {
                                if (tsButton.RightToLeft == RightToLeft.Yes)
                                {
                                    g.DrawLine(penImage, rcButton.Left, rcButton.Y, rcButton.Left, rcButton.Bottom - 1);
                                    g.DrawLine(penButton, rcDropDown.Right - 1, rcDropDown.Y, rcDropDown.Right - 1, rcDropDown.Bottom - 1);
                                }
                                else
                                {
                                    g.DrawLine(penImage, rcButton.Right - 1, rcButton.Y, rcButton.Right - 1, rcButton.Bottom - 1);
                                    g.DrawLine(penButton, rcDropDown.X, rcDropDown.Y, rcDropDown.X, rcDropDown.Bottom - 1);
                                }
                            }
                        }

                        Image image = tsButton.Image;

                        // Draw image.
                        if ((tsButton.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                        {
                            if (image != null)
                            {
                                bool bDisposeImage = false;

                                if (bIsDisabled)
                                {
                                    image = CreateDisabledImage(image);
                                    bDisposeImage = true;
                                }

                                g.DrawImage(image, tsButton.InternalLayout.ImageRectangle);

                                if (bDisposeImage)
                                {
                                    image.Dispose();
                                }
                            }
                        }

                        Color clForeColor = bIsDisabled ? SystemColors.GrayText : tsButton.ForeColor;

                        // Draw text.
                        if ((tsButton.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                        {
                            TextRenderer.DrawText(g, tsButton.Text, tsButton.Font, tsButton.InternalLayout.TextRectangle, clForeColor, ToolStripRendererUtils.GetTextFormatFlags(tsButton.TextAlign));
                        }

                        // Draw arrow.
                        Rectangle arrowRectangle = tsButton.InternalLayout.DropDownButtonRectangle;

                        using (Brush brush = new SolidBrush(clForeColor))
                        {
                            Point pt = new Point(arrowRectangle.Left + (arrowRectangle.Width / 2), arrowRectangle.Top + (arrowRectangle.Height / 2));
                            Point[] points = new Point[] { new Point(pt.X - 2, pt.Y - 1), new Point(pt.X + 3, pt.Y - 1), new Point(pt.X, pt.Y + 2) };

                            g.FillPolygon(brush, points);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Indicates if a ToolStrip locates on grouped RibbonPanel.
        /// </summary>
        /// <param name="pToolStrip"> ToolStrip instance. </param>
        /// <returns> True - ToolStrip locates on grouped RibbonPanel,
        /// otherwise - False.</returns>
        private bool IsGroupedToolStrip(ToolStrip ts, ref Color pColor)
        {
            bool bResult = false;

            if (ts != null)
            {
                RibbonPanel panel = GetParentPanel(ts);

                if (panel != null)
                {
                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();

                    if (tsTabGroup != null)
                    {
                        pColor = tsTabGroup.Color;
                        bResult = true;
                    }
                }
            }

            return bResult;
        }
        /// <summary>
        /// Indicates if a ToolStrip locates on grouped RibbonPanel.
        /// </summary>
        /// <param name="pToolStrip"> ToolStrip instance. </param>
        /// <returns> True - ToolStrip locates on grouped RibbonPanel,
        /// otherwise - False.</returns>
        private bool IsGroupedToolStrip(ToolStrip ts)
        {
            bool bResult = false;

            if (ts != null)
            {
                RibbonPanel panel = GetParentPanel(ts);

                if (panel != null)
                {
                    ToolStripTabGroup tsTabGroup = panel.GetTabGroup();
                    bResult = (tsTabGroup != null);
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private RibbonPanel GetParentPanel(ToolStrip ts)
        {
            Control parent = ts.Parent;

            ToolStripDropDown dropDown = parent as ToolStripDropDown;
            if (dropDown != null)
            {
                CollapsedDropDownButton item = dropDown.OwnerItem as CollapsedDropDownButton;
                if (item != null)
                {
                    ToolStrip owner = item.GetCurrentParent();
                    if (owner != null)
                    {
                        parent = owner.Parent;
                    }
                }
            }
            return parent as RibbonPanel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

            if (item != null)
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        Image imgArrow = ArrowOverflow;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);
                    }
                }
            }
            else
            {
                Rectangle rc = GetButtonRect(e.Item);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
                        Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);
                    }
                }
            }

            return true;
        }

        protected void PaintBackgroundSelected(Graphics g, Rectangle rc, Color clMetro)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (SolidBrush brush = new SolidBrush(clMetro))              
                {                 
                    g.FillRectangle(brush, rc);
                }
            }
        }

        private void PaintBackgroundCollapsed(Graphics g, Rectangle rc, Color clMetro)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                using (SolidBrush brush = new SolidBrush(clMetro))               
                {                 
                    g.FillRectangle(brush, rc);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void PaintBackgroundGrouped(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Color cl1 = this.GroupGradientBegin;
                using (SolidBrush brush = new SolidBrush(cl1))               
                {
                    g.FillRectangle(brush, rc);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PaintGroupBorders(ToolStripRenderEventArgs e)
        {
            ToolStrip ts = e.ToolStrip;
            if (!ts.IsDropDown && ERENDERTYPE.Grouped == GetRenderType(ts))
            {
                Graphics g = e.Graphics;

                ToolStripItemCollection tsItems = ts.Items;
                ToolStripItemPlacement tsPlacement = ToolStripItemPlacement.Main;

                if (tsItems != null)
                {
                    int nItems = tsItems.Count;

                    Rectangle[] rects = new Rectangle[2];

                    for (int i = 0; i < nItems; i++)
                    {
                        ToolStripItem item = tsItems[i];
                        if (item.Placement == tsPlacement)
                        {
                            int idx = item.Alignment == ToolStripItemAlignment.Left ? 0 : 1;
                            Rectangle rc = rects[idx];

                            if (item is ToolStripButton || item is ToolStripDropDownButton || item is ToolStripSplitButton)
                            {
                                int x1, x2, y1, y2;

                                Rectangle rcItem = GetButtonBounds(item);

                                if (!rc.IsEmpty)
                                {
                                    if (rc.Y == rcItem.Y)
                                    {
                                        x1 = rcItem.X > rc.X ? rc.Right - 1 : rc.Left - 1;
                                        x2 = x1 + 1;
                                        y1 = rc.Y + 1;
                                        y2 = rc.Bottom - 2;

                                        g.DrawLine(this.GroupBorder, x1, y1, x1, y2);
                                        g.DrawLine(this.ButtonHighlightGrouped, x2, y1, x2, y2);

                                        rects[idx] = Rectangle.Union(rc, rcItem);
                                    }
                                    else
                                    {
                                        PaintGroupBorder(g, rc);
                                        rects[idx] = rcItem;
                                    }
                                }
                                else
                                {
                                    rects[idx] = rcItem;
                                }
                            }
                            else
                            {
                                PaintGroupBorder(g, rc);
                                rects[idx] = Rectangle.Empty;
                            }
                        }
                    }
                    PaintGroupBorder(g, rects[0]);
                    PaintGroupBorder(g, rects[1]);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rc"></param>
        private void PaintGroupBorder(Graphics g, Rectangle rc)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                g.DrawPolygon(this.GroupBorder, RendererUtils.GetRoundedPolygon(rc, MENUITEM_RADIUS));

                int x1 = rc.Left + 1;
                int y1 = rc.Top + 1;
                int x2 = rc.Right - 2;
                int y2 = rc.Bottom - 2;

                g.DrawLine(this.ButtonHighlightGrouped, x1, y1, x1, y2);
                g.DrawLine(this.ButtonHighlightGrouped, x2, y1, x2, y2);
            }
        }         
        
        private void PaintTabBorder(Graphics g, Rectangle rc, EBUTTONSTATE eState)
        {
            if (rc.Width > 0 && rc.Height > 0)
            {
                Pen pen = eState == EBUTTONSTATE.Selected ? this.ButtonSelectedGradientEnd : this.ButtonPressedGradientEnd;
                g.DrawLines(pen, GetTabbedPath(rc));
            }
        }
        
        private Region GetTabbedRegion(Rectangle rc)
        {
            Region rgnResult = new Region(rc);

            int y = rc.Y;
            int x = rc.X;
            int h = rc.Height;

            rgnResult.Exclude(new Rectangle(x, y, 1, h - 1));
            rgnResult.Exclude(new Rectangle(x + 1, y, 1, h - 2));
            rgnResult.Exclude(new Rectangle(x + 2, y, 1, 2));
            rgnResult.Exclude(new Rectangle(x + 3, y, 1, 1));

            x = rc.Right - 4;

            rgnResult.Exclude(new Rectangle(x, y, 1, 1));
            rgnResult.Exclude(new Rectangle(x + 1, y, 1, 2));
            rgnResult.Exclude(new Rectangle(x + 2, y, 1, h - 2));
            rgnResult.Exclude(new Rectangle(x + 3, y, 1, h - 1));

            return rgnResult;
        }

        private Point[] GetTabbedPath(Rectangle rc)
        {
            Point[] points = new Point[]
				{
					new Point(rc.X, rc.Bottom-1),
					new Point(rc.X+2, rc.Bottom-3),
					new Point(rc.X+2, rc.Y+2),
					new Point(rc.X+4, rc.Y),
					new Point(rc.Right-5, rc.Y),
					new Point(rc.Right-3, rc.Y+2),
					new Point(rc.Right-3, rc.Bottom-3),
					new Point(rc.Right-1, rc.Bottom-1),
				};

            return points;
        }

        private Point[] GetLauncherPolygon(Rectangle rc)
        {
            Point[] points = new Point[]
                {
                    new Point(rc.X, rc.Y+1),
                    new Point(rc.X, rc.Y),
                    new Point(rc.Right-1, rc.Y),
                    new Point(rc.Right-1, rc.Bottom-1),
                    new Point(rc.Right-2, rc.Bottom-1),
                };

            return points;
        }

        private Rectangle GetItemRect(ToolStripItem item)
        {
            Rectangle rc = new Rectangle(Point.Empty, item.Size);

            if (item.Owner is ToolStripDropDownMenu)
            {
                rc.X += MENUITEM_PADDING_LEFT;
                rc.Width -= MENUITEM_HORIZONTAL;
            }

            return rc;
        }

        protected Rectangle GetButtonRect(ToolStripItem item)
        {
            return GetButtonRect(item, new Rectangle(Point.Empty, item.Size), this.RenderType);
        }

        private Rectangle GetButtonBounds(ToolStripItem item)
        {
            return GetButtonRect(item, item.Bounds, this.RenderType);
        }

        private Rectangle GetButtonBounds(ToolStripItem item, ERENDERTYPE erType)
        {
            return GetButtonRect(item, item.Bounds, erType);
        }

        private Rectangle GetButtonRect(ToolStripItem item, Rectangle rc)
        {
            return GetButtonRect(item, rc, this.RenderType);
        }

        public static Rectangle GetButtonRect(ToolStripItem item, Rectangle rc, ERENDERTYPE erType)
        {
            Rectangle rcResult = rc;

            if (rcResult.Width > 0 && rcResult.Height > 0)
            {
                if (item is ToolStripOverflowButton)
                {
                    Control parent = item.GetCurrentParent();
                    if (parent != null)
                    {
                        rcResult.Intersect(parent.ClientRectangle);
                    }
                    rcResult.Inflate(-1, -1);
                }
                else
                {
                    if (item.IsOnDropDown)
                    {
                        rcResult.Inflate(-1, 0);
                    }
                    else if (item.Owner is StatusStrip)
                    {
                        //rcResult.Height -= 2;
                    }
                }
            }

            return rcResult;
        }

        protected Rectangle GetButtonBackgroundRect(ToolStripItem item, Rectangle rc)
        {
            Rectangle rcResult = rc;

            if (!(item is ToolStripOverflowButton))
            {
                rcResult.Inflate(-1, -1);
            }

            return rcResult;
        }

        internal virtual Rectangle GetCheckRect(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = new Rectangle(e.ImageRectangle.Left - IMAGE_PADDING, IMAGE_MARGIN,
                e.ImageRectangle.Width + 2 * IMAGE_PADDING, e.Item.Height - 2 * IMAGE_MARGIN);

            return rc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static bool GetIsLButtonDown()
        {
            VirtualKeys vKey = WindowsAPI.GetSystemMetrics(SystemMetricsCodes.SM_SWAPBUTTON) != 0 ? VirtualKeys.VK_RBUTTON : VirtualKeys.VK_LBUTTON;
            return (WindowsAPI.GetAsyncKeyState((VirtualKeys)vKey) & 0x8000) != 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsItem"></param>
        /// <returns></returns>
        private bool GetIsGrouped(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (tsItem != null && !(tsItem is ToolStripOverflowButton) && !(tsItem is CollapsedDropDownButton) && !tsItem.IsOnDropDown)
            {
                bResult = ERENDERTYPE.Grouped == GetRenderType(tsItem.Owner);
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tsItem"></param>
        /// <returns></returns>
        private bool GetIsTabItem(ToolStripItem tsItem)
        {
            bool bResult = false;

            if (this.RenderType == ERENDERTYPE.TabBar && !tsItem.IsOnDropDown)
            {
                bResult = tsItem is IToolStripTabItem;

                if (!bResult)
                {
                    Type t = tsItem.GetType();
                    bResult = t == typeof(ToolStripButton) || t == typeof(ToolStripDropDownButton);
                }
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        internal static bool GetIsSelected(ToolStrip ts)
        {
            CToolStripWindow wnd = m_htControls[ts] as CToolStripWindow;

            if (wnd != null)
            {
                return wnd.Selected;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        private ERENDERTYPE GetRenderType(ToolStrip ts)
        {
            if (ts != null)
            {
                IToolStripExSupport iRibbon = ts as IToolStripExSupport;
                if (iRibbon != null && iRibbon.GroupedButtons)
                {
                    return ERENDERTYPE.Grouped;
                }
            }

            return this.RenderType;
        }
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        private void Clear()
        {
            Clear(m_htPens);
            Clear(m_htBrushes);
            Clear(m_htBitmaps);
        }
        /// <summary>
        /// Clean up specified resources being used.
        /// </summary>
        /// <param name="hTable">Hastable of resources to dispose</param>
        private void Clear(Hashtable hTable)
        {
            foreach (Object obj in hTable)
            {
                IDisposable iDispose = obj as IDisposable;

                if (iDispose != null)
                {
                    iDispose.Dispose();
                }
            }
            hTable.Clear();
        }
        //Event handlers
        /// <summary>
        /// Handle changes of display settings and user preferences.
        /// </summary>
        protected void SettingsChangedHandler()
        {
            //Clear cache of used GDI objects
            Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ePen"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Pen GetKnownPen(EPEN ePen, Color color, int width)
        {
            Pen pen = m_htPens[ePen] as Pen;
            if (pen == null)
            {
                pen = new Pen(color, width);
                m_htPens[ePen] = pen;
            }
            return pen;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eBrush"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Brush GetKnownBrush(EBRUSH eBrush, Color color)
        {
            Brush brush = m_htBrushes[eBrush] as Brush;
            if (brush == null)
            {
                brush = new SolidBrush(color);
                m_htBrushes[eBrush] = brush;
            }
            return brush;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorBase"></param>
        /// <returns></returns>
        protected Bitmap GetFlashImage(Color colorBase, Size size)
        {
            Rectangle rcFlash = new Rectangle(0, 0, size.Width, size.Height);
            Bitmap bitmap = new Bitmap(rcFlash.Width, rcFlash.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(rcFlash);

                    using (PathGradientBrush brFlash = new PathGradientBrush(path))
                    {
                        brFlash.Blend = m_blButtonFlash;

                        brFlash.CenterColor = colorBase;
                        brFlash.SurroundColors = new Color[] { Color.Transparent };

                        g.FillRectangle(brFlash, rcFlash);
                    }
                }
            }
            return bitmap;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Size GetDownArrowSize()
        {
            return this.ArrowDownImage.Size;
        }
        /// Gets background brush for collapsed ToolStripEx.
        /// </summary>
        /// <param name="clBegin"> Starting color for a gradient. </param>
        /// <param name="clEnd"> Final color for a gradient. </param>
        /// <param name="nHeight"> Brush's height for a gradient. </param>
        /// <returns> LinearGradientBrush for furhter painting. </returns>
        private LinearGradientBrush GetGroupedBackgroundCollapsedBrush(Color clBegin, Color clEnd, int nHeight)
        {
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, 1, nHeight), clBegin, clEnd, LinearGradientMode.Vertical);
            brush.Blend = m_blGroupedBackGroundCollapsed;

            return brush;
        }
        #endregion

        #region Properties
        public ERENDERTYPE RenderType
        {
            get
            {
                return m_eRenderType;
            }
            set
            {
                m_eRenderType = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionText
        {
            get
            {
                return this.CaptionText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionGradientBegin
        {
            get
            {
                return this.CaptionGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionGradientEnd
        {
            get
            {
                return this.CaptionGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionHighlightGradientBegin
        {
            get
            {
                return this.CaptionHighlightGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color CaptionHighlightGradientEnd
        {
            get
            {
                return this.CaptionHighlightGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color ToolStripHighlightGradientBegin
        {
            get
            {
                return this.ToolStripHighlightGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color ToolStripHighlightGradientEnd
        {
            get
            {
                return this.ToolStripHighlightGradientEnd;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherBackground
        {
            get
            {
                return this.LauncherBackground;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherText
        {
            get
            {
                return this.LauncherText;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color LauncherTextSelected
        {
            get
            {
                return this.LauncherTextSelected;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color GroupGradientBegin
        {
            get
            {
                return this.GroupGradientBegin;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color GroupGradientEnd
        {
            get
            {
                return this.GroupGradientEnd;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Color BorderColor
        {
            get
            {
                Office12ColorTable colorTable = this.ColorTable as Office12ColorTable;

                if (colorTable != null)
                {
                    return colorTable.ToolStripBorder;
                }

                return SystemColors.ControlDark;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ToolStripBorder
        {
            get { return GetKnownPen(EPEN.epToolStripBorder, ColorTable.ToolStripBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuBorder
        {
            get { return GetKnownPen(EPEN.epMenuBorder, ColorTable.MenuBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuItemBorder
        {
            get { return GetKnownPen(EPEN.epMenuItemBorder, ColorTable.MenuItemBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuStripBorder
        {
            get
            {
                Office12ColorTable colorTable = ColorTable as Office12ColorTable;
                Color color = colorTable != null ? colorTable.MenuStripGradientMiddle : ColorTable.MenuBorder;

                return GetKnownPen(EPEN.epMenuStripBorder, color, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuItemPressedGradientEnd
        {
            get { return GetKnownPen(EPEN.epMenuItemPressedGradientEnd, ColorTable.MenuItemPressedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedGradientBegin
        {
            get { return GetKnownPen(EPEN.epButtonPressedGradientBegin, ColorTable.ButtonPressedGradientBegin, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedGradientEnd
        {
            get { return GetKnownPen(EPEN.epButtonPressedGradientEnd, ColorTable.ButtonPressedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonSelectedGradientEnd
        {
            get { return GetKnownPen(EPEN.epButtonSelectedGradientEnd, ColorTable.ButtonSelectedGradientEnd, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonSelectedBorder
        {
            get
            {
                Color cl = Color.FromArgb(128, ColorTable.ButtonSelectedBorder);
                return GetKnownPen(EPEN.epButtonSelectedBorder, cl, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonPressedBorder
        {
            get { return GetKnownPen(EPEN.epButtonPressedBorder, ColorTable.ButtonPressedBorder, 1); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen CollapsedBorderPen
        {
            get
            {
                return GetKnownPen(EPEN.epCollapsedBorder, Color.FromArgb(16, 0, 0, 0), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ImageBorderPen
        {
            get
            {
                return GetKnownPen(EPEN.epImageBorder, Color.FromArgb(32, 0, 0, 0), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonHighlightBorder
        {
            get
            {
                Color color = Office12ColorTable.GetAlphaBlendedColor(Color.Transparent, Color.White, 128);
                return GetKnownPen(EPEN.epButtonHighlightBorder, color, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen ButtonHighlightGrouped
        {
            get
            {
                return GetKnownPen(EPEN.epHighlightGrouped, Office12ColorTable.GetAlphaBlendedColor(Color.White, StripMetroColor, 160), 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen GroupBorder
        {
            get
            {
                return GetKnownPen(EPEN.epGroupBorder, this.StripMetroColor, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen LauncherBorder
        {
            get
            {
                return GetKnownPen(EPEN.epLauncherBorder, this.StripMetroColor, 2);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Pen CheckBorder
        {
            get
            {
                return GetKnownPen(EPEN.epCheckBorder, this.StripMetroColor, 1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush Shadow
        {
            get { return GetKnownBrush(EBRUSH.ebShadow, Color.FromArgb(128, Color.Black)); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush ToolStripDropDownBackground
        {
            get { return GetKnownBrush(EBRUSH.ebToolStripDropDownBackground, ColorTable.ToolStripDropDownBackground); }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Brush CheckBackground
        {
            get
            {
                return GetKnownBrush(EBRUSH.ebCheckBackground, ColorTable.CheckBackground);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap ArrowDownImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowDown] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT, ARROW_HEIGHT + 1);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH, 1);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(0, 3);
                            while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(-1, 0);
                                rc.Y += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(0, 1);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }

                    m_htBitmaps[EBITMAP.ebArrowDown] = bitmap;
                }
                return bitmap;
            }
        }
        protected Bitmap ArrowOverflow
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowOverflow] as Bitmap;

                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT + 1, ARROW_HEIGHT + 1);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        // First arrow.
                        Rectangle rc = new Rectangle(0, 2, 1, 3);

                        using (Region rgDark = new Region(rc))
                        {
                            rgDark.Union(rc);

                            rc.Inflate(0, -1);
                            rc.X += 1;
                            rgDark.Union(rc);

                            g.FillRegion(SystemBrushes.ControlText, rgDark);
                        }

                        // Second arrow.
                        rc = new Rectangle(4, 2, 1, 3);
                        using (Region rgDark = new Region(rc))
                        {
                            rgDark.Union(rc);

                            rc.Inflate(0, -1);
                            rc.X += 1;
                            rgDark.Union(rc);

                            g.FillRegion(SystemBrushes.ControlText, rgDark);
                        }

                        // Highlight for first arrow.
                        rc = new Rectangle(0, 1, 1, 1);
                        using (Region rgLight = new Region(rc))
                        {
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);

                            g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                        }

                        // Highlight for second arrow.
                        rc = new Rectangle(4, 1, 1, 1);
                        using (Region rgLight = new Region(rc))
                        {
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X += 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);
                            rc.X -= 1;
                            rc.Y += 1;
                            rgLight.Union(rc);

                            g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                        }
                    }

                    m_htBitmaps[EBITMAP.ebArrowOverflow] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap ArrowRightImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebArrowRight] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(3, 0);
                            while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(1, 0);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                    m_htBitmaps[EBITMAP.ebArrowRight] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap SelectedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebSelectedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonSelectedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebSelectedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap PressedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebPressedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonPressedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebPressedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap CheckedFlashImage
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebCheckedFlash] as Bitmap;
                if (bitmap == null)
                {
                    bitmap = GetFlashImage(ColorTable.ButtonCheckedHighlight, new Size(20, 20));
                    m_htBitmaps[EBITMAP.ebCheckedFlash] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Bitmap CheckButton
        {
            get
            {
                Bitmap bitmap = m_htBitmaps[EBITMAP.ebCheckButton] as Bitmap;
                if (bitmap == null)
                {
                    Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
                    bitmap = new Bitmap(rcFlash.Width, rcFlash.Height);

                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.Clear(Color.Transparent);

                        Point[] points = new Point[]
						{
							new Point(1,8), 
							new Point(3,12),
							new Point(8,1)
						};

                        using(GraphicsPath path = new GraphicsPath())
                        path.AddLines(points);

                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(Pens.MidnightBlue, points);
                    }
                    m_htBitmaps[EBITMAP.ebCheckButton] = bitmap;
                }
                return bitmap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static StringFormat CaptionFormat
        {
            get
            {
                if (m_sfCaption == null)
                {
                    m_sfCaption = new StringFormat();
                    m_sfCaption.HotkeyPrefix = HotkeyPrefix.Hide;
                    m_sfCaption.Alignment = StringAlignment.Near;
                    m_sfCaption.Trimming = StringTrimming.Character;
                    m_sfCaption.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                }

                return m_sfCaption;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static Blend ToolBarBlend
        {
            get { return m_blToolBar; }
        }
        #endregion

        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum ERENDERTYPE
        {
            Normal = 0,
            Grouped,
            TabBar,
            MAX,
        }
        /// <summary>
        /// 
        /// </summary>
        public enum EBUTTONSTATE
        {
            Normal = 0,
            Pressed,
            Selected,
            Checked,
            Collapsed,
            MAX
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EPEN
        {
            epMenuBorder = 0,
            epMenuItemBorder,
            epMenuStripBorder,
            epButtonBorder,
            epButtonBorderHighlight,
            epButtonBorderDropdown,
            epMenuItemPressedGradientEnd,
            epButtonPressedGradientBegin,
            epButtonPressedGradientMiddle,
            epButtonPressedGradientEnd,
            epButtonSelectedGradientEnd,
            epButtonSelectedBorder,
            epButtonPressedBorder,
            epButtonHighlightBorder,
            epHighlightGrouped,
            epToolStripBorder,
            epGroupBorder,
            epCheckBorder,
            epLauncherBorder,
            epCollapsedBorder,
            epImageBorder,
            MAX,
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EBRUSH
        {
            ebShadow = 0,
            ebToolStripDropDownBackground,
            ebCheckBackground,
            MAX,
        };
        /// <summary>
        /// 
        /// </summary>
        protected enum EBITMAP
        {
            ebArrowDown = 0,
            ebArrowRight,
            ebArrowOverflow,
            ebSelectedFlash,
            ebPressedFlash,
            ebCheckedFlash,
            ebLauncher,
            ebLauncherSelected,
            ebLauncher2007,
            ebLauncher2007Selected,
            ebCheckButton,
            ebSystemButtonSelectedFlash,
            ebSystemButtonPressedFlash,
            ebRightArrow,
            ebLeftArrow,
            ebDownArrow,
            ebUpArrow,
            ebRadioButtonChecked,
            ebRadioButtonUnchecked,
            ebRadioButtonCheckedSelected,
            ebRadioButtonUncheckedSelected,
            ebRadioButtonCheckedPressed,
            ebRadioButtonUncheckedPressed,
            MAX,
        };
        #endregion

        #region Fields
        private const int MENUITEM_PADDING_LEFT = 2;
        private const int MENUITEM_PADDING_RIGHT = 1;
        private const int MENUITEM_HORIZONTAL = (MENUITEM_PADDING_LEFT + MENUITEM_PADDING_RIGHT);

        private const int IMAGE_PADDING = 2;
        private const int IMAGE_MARGIN = 1;

        internal const int MENUITEM_RADIUS = 1;
        internal const int TOOLSTRIP_RADIUS = 2;

        protected static Blend m_blMenuBar;
        protected static Blend m_blStatusBar;
        protected static Blend m_blToolBar;
        protected static Blend m_blMenuItemUp;
        protected static Blend m_blMenuItemDown;
        protected static Blend m_blGrouped;
        protected static Blend m_blScrollButton;
        protected static Blend m_blButtonSelected;
        protected static Blend m_blButtonCollapsed;
        protected static Blend m_blButtonShadow;
        protected static Blend m_blImageBackground;
        protected static Blend m_blButtonFlash;
        protected static Blend m_blCaption;
        protected static Blend m_blScrollerBackground;
        protected static Blend m_blStandardScrollButton;
        protected static Blend m_blScroller;
        /// <summary>
        /// 
        /// </summary>
        private static Blend m_blGroupedBackGroundCollapsed;
        /// <summary>
        /// 
        /// </summary>
        private static Blend m_blGroupedBackGround;

        private static Graphics m_gCaption;
        private static StringFormat m_sfCaption = null;
        private ERENDERTYPE m_eRenderType;

        private int m_refCounter = 0;

        private Hashtable m_htPens;
        private Hashtable m_htBrushes;
        protected Hashtable m_htBitmaps;


        static Hashtable m_htControls;
        static Object m_controlsLock;

        static int LAUNCHER_WIDTH = 14;//12;
        static int LAUNCHER_HEIGHT = 14;//12;
        static int ARROW_WIDTH = 5;
        static int ARROW_HEIGHT = 6;

        #endregion

        #region Imports
        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        protected struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;
        };
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDC"></param>
        /// <param name="xform"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        static protected extern bool SetWorldTransform(IntPtr hDC, ref XFORM xform);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDC"></param>
        /// <param name="iMode"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        static protected extern int SetGraphicsMode(IntPtr hDC, int iMode);
        #endregion
    }
    #endregion

    #region Office2013ToolStripRenderer

    public partial class Office2013ToolStripRenderer : ToolStripProfessionalRenderer
    {
        #region Fields

        private Office2010ColorTable colorTable = null;
        Office2013ToolStripRendererUtils rendererHelper = null;
        Bitmap launcherImageNormal = null;
        Bitmap launcerImageSelected = null;
        Bitmap arrowDownImage = null;
        const int LAUNCHER_SIZE = 14;
        private ComboBoxRenderer comboBoxRenderer = null;
        private ERENDERTYPE m_eRenderType;
        Color menuColor = ColorTranslator.FromHtml("#87CEFF");
        private bool useDefaultHighlightColor = true;
        ColorBlend selectionBlend = null;
        static int ARROW_WIDTH = 5;
        static int ARROW_HEIGHT = 6;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected Pen MenuItemBorder
        {
            get { return GetKnownPen(EPEN.epMenuItemBorder, ColorTable.MenuItemBorder, 1); }
        }
        private Hashtable m_htPens;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ePen"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected Pen GetKnownPen(EPEN ePen, Color color, int width)
        {
            Pen pen = m_htPens[ePen] as Pen;
            if (pen == null)
            {
                pen = new Pen(color, width);
                m_htPens[ePen] = pen;
            }
            return pen;
        }
        protected enum EPEN
        {
            epMenuBorder = 0,
            epMenuItemBorder,
            epMenuStripBorder,
            epButtonBorder,
            epButtonBorderHighlight,
            epButtonBorderDropdown,
            epMenuItemPressedGradientEnd,
            epButtonPressedGradientBegin,
            epButtonPressedGradientMiddle,
            epButtonPressedGradientEnd,
            epButtonSelectedGradientEnd,
            epButtonSelectedBorder,
            epButtonPressedBorder,
            epButtonHighlightBorder,
            epHighlightGrouped,
            epToolStripBorder,
            epGroupBorder,
            epCheckBorder,
            epLauncherBorder,
            epCollapsedBorder,
            epImageBorder,
            MAX,
        };
        #region Enums
        /// <summary>
        /// 
        /// </summary>
        public enum ERENDERTYPE
        {
            Normal = 0,
            Grouped,
            TabBar,
            MAX,
        }
        #endregion

        #region Properties
        private Office2013ColorScheme toolStipOffice2013ColorScheme = Office2013ColorScheme.White;
        internal Office2013ColorScheme ToolStipOffice2013ColorScheme
        {
            get
            {
                return toolStipOffice2013ColorScheme;
            }
            set
            {
                toolStipOffice2013ColorScheme = value;
            }
        }
        internal Color MenuColor
        {
            get
            {
                return menuColor;
            }
            set
            {
                if (menuColor != value)
                {                    
                      menuColor =value ;
                      ComboRenderer.MenuColor = value;
                }
            }
        }
        /// <summary>
        /// Gets or Sets whether default highlight color should be used 
        /// </summary>
        internal bool UseDefaultHighlightColor
        {
            get
            {
                return useDefaultHighlightColor;
            }
            set
            {
                if (useDefaultHighlightColor != value)
                    useDefaultHighlightColor = value;
            }
        }



        public ERENDERTYPE RenderType
        {
            get
            {
                return m_eRenderType;
            }
            set
            {
                m_eRenderType = value;
            }
        }

        internal ColorBlend SelectionBlend
        {
            get
            {
                if (selectionBlend == null)
                {
                    selectionBlend = new ColorBlend(7);
                    selectionBlend.Positions = new float[]
                    {
                       0F,
                       0.83F,
                       0.86F,
                       0.9F,
                       0.93F,
                       0.96F,
                       1F
                    };
                }

                return selectionBlend;
            }

        }

        public Office2013ToolStripRendererUtils RendererHelper
        {
            get { return rendererHelper; }
            set { rendererHelper = value; }
        }

        public Bitmap LauncherImageNormal
        {
            get
            {
                if (launcherImageNormal == null)
                {
                    Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_SIZE, LAUNCHER_SIZE);
                    launcherImageNormal = new Bitmap(rcImage.Width, rcImage.Height);

                    using (Graphics g = Graphics.FromImage(launcherImageNormal))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                        rc.Offset(1, 1);

                        using (Region region = new Region(rc))
                        {
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                            region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                            region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                            region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                            region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                            region.Translate(-1, -1);

                            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#999999")))
                            {
                                g.FillRegion(brush, region);
                            }
                        }
                    }
                }

                return launcherImageNormal;
            }
        }

        public Bitmap LauncerImageSelected
        {
            get
            {
                if (launcerImageSelected == null)
                {
                    Rectangle rcImage = new Rectangle(0, 0, LAUNCHER_SIZE, LAUNCHER_SIZE);

                    launcerImageSelected = new Bitmap(rcImage.Width, rcImage.Height);

                    using (Graphics g = Graphics.FromImage(launcerImageSelected))
                    {
                        g.Clear(Color.Transparent);
                        Color color;
                        if(UseDefaultHighlightColor)
                            color = ColorTranslator.FromHtml("#cde6f7");
                        else
                            color = ControlPaint.LightLight(ControlPaint.LightLight(menuColor));
                        using (Region region = new Region(Rectangle.Inflate(rcImage, -1, -1)))
                        {
                            using (SolidBrush brush = new SolidBrush(color))
                            {
                                g.FillRegion(brush, region);
                            }

                        }

                        Rectangle rc = Rectangle.Inflate(rcImage, -3, -3);
                        rc.Offset(1, 1);

                        using (Region region = new Region(rc))
                        {
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, rc.Width - 1, 3));
                            region.Exclude(new Rectangle(rc.X + 1, rc.Y + 1, 3, rc.Height - 1));
                            region.Exclude(new Rectangle(rc.X + 5, rc.Y + 4, 2, 1));
                            region.Exclude(new Rectangle(rc.X + 4, rc.Y + 5, 1, 2));
                            region.Exclude(new Rectangle(rc.X, rc.Y + 6, 1, 6));
                            region.Exclude(new Rectangle(rc.X + 6, rc.Y, 6, 1));

                            region.Translate(-1, -1);

                            using (SolidBrush brush = new SolidBrush(menuColor))
                            {
                                g.FillRegion(brush, region);
                            }
                        }
                    }
                }

                return launcerImageSelected;
            }
        }

        private ComboBoxRenderer ComboRenderer
        {
            get { return comboBoxRenderer; }
        }

        protected Bitmap ArrowDownImage
        {
            get
            {
                if (arrowDownImage == null)
                {
                    arrowDownImage = new Bitmap(ARROW_HEIGHT, ARROW_HEIGHT + 1);
                    using (Graphics g = Graphics.FromImage(arrowDownImage))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, ARROW_WIDTH, 1);
                        using (Region rgDark = new Region(rc))
                        {
                            while (rc.X < rc.Right && rc.Y < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(-1, 0);
                                rc.Y += 1;
                            }
                            using (SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#777777")))
                            {
                                g.FillRegion(brush, rgDark);
                            }
                        }
                    }
                }
                return arrowDownImage;
            }
        }

        protected Bitmap ArrowRightImage
        {
            get
            {
                if (arrowDownImage == null)
                {
                    arrowDownImage = new Bitmap(ARROW_HEIGHT + 1, ARROW_WIDTH);
                    using (Graphics g = Graphics.FromImage(arrowDownImage))
                    {
                        g.Clear(Color.Transparent);

                        Rectangle rc = new Rectangle(0, 0, 1, ARROW_WIDTH);
                        using (Region rgDark = new Region(rc))
                        {
                            rc.Offset(3, 0);
                            while (rc.Y < rc.Bottom && rc.X < ARROW_HEIGHT)
                            {
                                rgDark.Union(rc);

                                rc.Inflate(0, -1);
                                rc.X += 1;
                            }

                            g.FillRegion(SystemBrushes.ControlText, rgDark);

                            using (Region rgLight = rgDark.Clone())
                            {
                                rgLight.Translate(1, 0);
                                rgLight.Exclude(rgDark);
                                g.FillRegion(SystemBrushes.ButtonHighlight, rgLight);
                            }
                        }
                    }
                }

                return arrowDownImage;
            }
        }

        #endregion

        #region Ctor
        public Office2013ToolStripRenderer()
            : this(new Office2010ColorTable(), ERENDERTYPE.Normal)
        {
        }

        public Office2013ToolStripRenderer(Office2010ColorTable colorTable, ERENDERTYPE erType)
            : base(colorTable)
        {
            this.colorTable = colorTable;
            this.m_htPens = new Hashtable();
            rendererHelper = new Office2013ToolStripRendererUtils(this);
            comboBoxRenderer = new ComboBoxRenderer(this);
        }

        #endregion

        #region Overrides

        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            this.RendererHelper.Initialize(toolStrip);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintArrow(e))
            {
                base.OnRenderArrow(e);
            }
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintButtonBackground(e))
            {
                base.OnRenderButtonBackground(e);
            }
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintDropDownButtonBackground(e))
            {
                base.OnRenderDropDownButtonBackground(e);
            }
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            base.OnRenderGrip(e);
        }

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderItemBackground(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemImage(e))
            {
                base.OnRenderItemImage(e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintItemText(e))
            {
                base.OnRenderItemText(e);
            }
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintOverflowButtonBackground(e))
            {
                base.OnRenderOverflowButtonBackground(e);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            base.OnRenderStatusStripSizingGrip(e);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintSplitButtonBackground(e))
            {
                base.OnRenderSplitButtonBackground(e);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                if (!SystemInformation.HighContrast)
                {
                    Brush br = new SolidBrush(Color.White);
                    Color clr;
                    ToolStrip ts = e.ToolStrip;
                    e.Graphics.FillRectangle(br, e.AffectedBounds);
                    RibbonControlAdv ribbon = ts.Parent as RibbonControlAdv;
                    if (ribbon != null)
                    {
                        if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                        {
                            clr = ColorTranslator.FromHtml("#F8F8F8");
                            br = new SolidBrush(clr);
                            e.Graphics.FillRectangle(br, e.AffectedBounds);
                        }
                        else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                        {
                            clr = ColorTranslator.FromHtml("#F0F0F0");
                            br = new SolidBrush(clr);
                            e.Graphics.FillRectangle(br, e.AffectedBounds);
                        }
                    }
                    br.Dispose();
                }
                else
                    base.OnRenderToolStripBackground(e);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBorder(e))
            {
                base.OnRenderToolStripBorder(e);
            }
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            base.OnRenderToolStripPanelBackground(e);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            base.OnRenderToolStripContentPanelBackground(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderLabelBackground(e);
        }
        protected Bitmap CheckButton
        {
            get
            {
                if (checkButton == null)
                {
                    Rectangle rcFlash = new Rectangle(0, 0, 10, 14);
                    checkButton = new Bitmap(rcFlash.Width, rcFlash.Height);
                    using (Graphics g = Graphics.FromImage(checkButton))
                    {
                        g.Clear(Color.Transparent);
                        Point[] points = new Point[]
						{
							new Point(1,8), 
							new Point(3,12),
							new Point(8,1)
						};
                        using (GraphicsPath path = new GraphicsPath())
                            path.AddLines(points);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawLines(Pens.MidnightBlue, points);
                    }
                }
                return checkButton;
            }
        }
        Bitmap checkButton;
        private Rectangle GetCheckRect(ToolStripItemImageRenderEventArgs e)
        {
            int IMAGE_MARGIN = 1, IMAGE_PADDING = 1;
            Rectangle rc = new Rectangle(e.ImageRectangle.Left - IMAGE_PADDING, IMAGE_MARGIN,
                e.ImageRectangle.Width + 2 * IMAGE_PADDING, e.Item.Height - 2 * IMAGE_MARGIN);
            return rc;
        }
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = GetCheckRect(e);
            if (rc.Width > 0 && rc.Height > 0)
            {
                Size szImage = this.CheckButton.Size;

                int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
            }
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderToolStripStatusLabelBackground(e);
        }

        #endregion

        #region Implementations

        private bool PaintItemImage(ToolStripItemImageRenderEventArgs e)
        {
            if (e.Item is ToolStripMenuItem)
            {
                ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;
                if (tsItem != null && tsItem.Checked && !ToolStripRendererUtils.IsCheckMarginVisible(tsItem))
                {
                    PaintItemCheckBackground(e);
                }
            }

            Image image = e.Image;
            Rectangle rc = ToolStripRendererUtils.GetImageRect(e);

            if (image != null && rc.Width > 0 && rc.Height > 0)
            {
                bool bDisposeImage = false;

                if (!e.Item.Enabled)
                {
                    image = CreateDisabledImage(image);
                    bDisposeImage = true;
                }

                Graphics g = e.Graphics;

                if (e.Item is CollapsedDropDownButton)
                {
                    Rectangle borderRect = rc;
                    borderRect.Inflate(ToolStripEx.DEF_IMAGE_BORDER_OFFSET, ToolStripEx.DEF_IMAGE_BORDER_OFFSET);

                    using (Pen pen = new Pen(ColorTranslator.FromHtml("#c6c6c6")))
                    {
                        g.DrawRectangle(pen, borderRect);
                    }
                }

                if (e.Item is OfficeButton)
                {
                    if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
                    {
                        rc.X -= 1;
                    }
                    else
                    {
                        rc.X += 1;
                    }
                }

                if (e.Item.ImageScaling == ToolStripItemImageScaling.None)
                {
                    Size szImage = image.Size;

                    int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                    int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                    g.DrawImage(image, new Point(iImageX, iImageY));
                }
                else
                {
                    InterpolationMode interpolation = g.InterpolationMode;

                    if (image.Size != rc.Size)
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    }

                    g.DrawImage(image, rc);

                    g.InterpolationMode = interpolation;
                }

                if (bDisposeImage)
                {
                    image.Dispose();
                }
            }

            return true;
        }

        private void PaintItemCheckBackground(ToolStripItemImageRenderEventArgs e)
        {

        }

        private Image GetToolstripItemImageSelected(Rectangle rect)
        {
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            rect.Width -= 1; rect.Height -= 1;
            Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                Color color;
                if (UseDefaultHighlightColor)
                    color = ColorTranslator.FromHtml("#cde6f7");
                else
                    color = ControlPaint.LightLight(menuColor);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            return bmp;
        }

        public Size GetDownArrowSize()
        {
            return this.ArrowDownImage.Size;
        }

        private bool PaintItemText(ToolStripItemTextRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                Rectangle rc = ToolStripRendererUtils.GetTextRect(e);

                TextRenderer.DrawText(e.Graphics, e.Text, e.TextFont, rc, e.Item.ForeColor, e.TextFormat);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

            if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
            {
                rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
            }

            if (e.ToolStrip is ContextMenuStripEx)
            {
                ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                rc.Y += statusStrip.TitleHeight;
                rc.Height -= statusStrip.TitleHeight;
            }

            using (Brush brush = new SolidBrush(ColorTable.ImageMarginGradientBegin))
            {
                g.FillRectangle(brush, rc);
            }

            int beginX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Right) : (rc.Left);
            int endX = (e.ToolStrip.RightToLeft == RightToLeft.Yes) ? (rc.Left) : (rc.Right);


            return true;
        }

        public void DrawCaption(ToolStrip ts)
        {
            RibbonPanel panel = ts.Parent as RibbonPanel;
            if (ToolStripRendererUtils.HasCaption(ts, Office12ToolStripRenderer.ERENDERTYPE.Normal))
            {
                IntPtr wnd = ts.Handle;

                RECT wndRect = new RECT();
                if (WindowsAPI.GetWindowRect(wnd, ref wndRect))
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    if (hdc != IntPtr.Zero)
                    {
                        Rectangle rcUpdate = ToolStripRendererUtils.GetCaptionBounds(ts, Office12ToolStripRenderer.ERENDERTYPE.Normal);

                        rcUpdate.X -= wndRect.left;
                        rcUpdate.Y -= wndRect.top;

                        using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcUpdate))
                        {
                            Graphics g = bg.Graphics;

                            Color lancherBackColor = Color.White;
                            if (panel != null)
                            {
                                if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.DarkGray)
                                {
                                    lancherBackColor = ColorTranslator.FromHtml("#F0F0F0");
                                }
                                else if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.LightGray)
                                {
                                    lancherBackColor = ColorTranslator.FromHtml("#F8F8F8");
                                }
                            }
                            if (SystemInformation.HighContrast)
                            {
                                lancherBackColor = Color.Black;
                            }
                            using (Brush brush = new SolidBrush(lancherBackColor))
                            {
                                
                                g.FillRectangle(brush, rcUpdate);
                            }

                            Font f = ToolStripRendererUtils.GetCaptionFont(ts);

                            Rectangle rcText = new Rectangle(0, 0, rcUpdate.Width, rcUpdate.Height);

                            if (ToolStripRendererUtils.HasLauncher(ts))
                            {
                                rcText.Width -= LAUNCHER_SIZE;
                            }

                            TextFormatFlags tf = ToolStripRendererUtils.GetCaptionFormat(ts);
                            Color clrText = ColorTranslator.FromHtml("#666666");
                            rcText.Y -= 1;
                           // Color color = (ts.ForeColor == SystemColors.ControlText) ? clrText :Color .Gray ;
                            if (SystemInformation.HighContrast)
                                clrText = SystemColors.MenuText;
                            TextRenderer.DrawText(g, ts.Text, f, rcText, clrText, tf);

                            PaintLauncher(g, rcUpdate, ts as IToolStripExSupport2);

                            bg.Render();
                        }
                    }

                    WindowsAPI.ReleaseDC(wnd, hdc);
                }
            }
        }

        public void DrawBorders(ToolStrip ts)
        {
            if (ts is IToolStripExSupport2)
            {
                if ((ts as IToolStripExSupport2).BorderStyle == ToolStripBorderStyle.None)
                    return;
            }

            IntPtr wnd = ts.Handle;
            RibbonPanel panel = ts.Parent as RibbonPanel;
            if (panel != null)
            {
                RibbonControlAdv ribbon = panel.Parent as RibbonControlAdv;
                RECT rect = new RECT();
                if (WindowsAPI.GetWindowRect(wnd, ref rect))
                {
                    IntPtr hdc = WindowsAPI.GetWindowDC(wnd);
                    if (hdc != IntPtr.Zero)
                    {
                        Rectangle rcBorder = new Rectangle(rect.Width - 3, 0, 3, rect.Height);
                        Brush br = new SolidBrush(Color.White);
                        Color clr = Color.Empty;
                        using (BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(hdc, rcBorder))
                        {
                            Graphics g = bg.Graphics;
                            GraphicsState gState = g.Save();
                            if (panel.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                            {
                                clr = ColorTranslator.FromHtml("#F0F0F0");
                                using (br = new SolidBrush(clr))
                                {
                                    g.FillRectangle(br, rcBorder);
                                }
                            }
                            else if (panel.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                            {
                                clr = ColorTranslator.FromHtml("#F8F8F8");
                                using (br = new SolidBrush(clr))
                                {
                                    g.FillRectangle(br, rcBorder);
                                }
                            }
                            else if (panel.Office2013ColorScheme == Office2013ColorScheme.White)
                            {
                                g.FillRectangle(br, rcBorder);
                            }
                            if (rcBorder.Height > 0 && rcBorder.Width > 0)
                            {
                                using (Pen pen = new Pen(new LinearGradientBrush(rcBorder, Color.LightGray, Color.LightGray, LinearGradientMode.Vertical)))
                                {
                                    g.DrawLine(pen, rcBorder.Right - 2, rcBorder.Top + 3, rcBorder.Right - 2, rcBorder.Bottom - 4);
                                }
                            }

                            g.Restore(gState);
                            bg.Render();
                        }
                        br.Dispose();
                        WindowsAPI.ReleaseDC(wnd, hdc);
                    }
                }
            }
        }

        private bool PaintDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            return PaintButtonBackground(e);
        }

        private bool PaintToolStripBorder(ToolStripRenderEventArgs e)
        {
            return true;
        }

        private bool PaintToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            if (e.ToolStrip is BottomToolstrip)
            {
                bResult = PaintBottomToolstripBackground(e);
            }
            else if (e.ToolStrip is ContextMenuStripEx)
            {
                bResult = PaintContextMenuBackground(e);
            }
            else if (e.ToolStrip is MenuStrip)
            {
                bResult = PaintMenuBackground(e);
            }
            else if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                bResult = PaintToolStripGalleryDropDownBackground(e);
            }
            else
                if (e.ToolStrip is StatusStrip)
                {
                    bResult = PaintStatusStripBackground(e);
                }
                else
                    if (e.ToolStrip is ToolStripDropDown)
                    {
                        bResult = PaintDropdownBackground(e);
                    }
                    else
                    {
                        bResult = PaintGroupedToolbarBackground(e) || PaintToolbarBackground(e);
                    }

            return bResult;
        }

        private bool PaintToolbarBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            ToolStrip ts = e.ToolStrip;

            if (ts != null)
            {
                bool selected = this.RendererHelper.GetIsSelected(ts);
                Rectangle rc = new Rectangle(Point.Empty, ts.Size);
                Graphics g = e.Graphics;

                if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.White)
                {
                    using (LinearGradientBrush brushs = new LinearGradientBrush(rc, Color.White, Color.White, LinearGradientMode.Vertical))
                    {
                        PanelItemRenderEventArgs argss = e as PanelItemRenderEventArgs;
                        if (argss != null)
                        {
                            Point pt = argss.PanelStrip.Location;
                            brushs.TranslateTransform(-pt.X, -pt.Y, MatrixOrder.Append);
                        }

                        g.FillRectangle(brushs, rc);
                    }
                }
                if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.DarkGray)
                {
                    using (LinearGradientBrush brushs = new LinearGradientBrush(rc, ColorTranslator.FromHtml("#F0F0F0"), ColorTranslator.FromHtml("#F0F0F0"), LinearGradientMode.Vertical))
                    {
                        PanelItemRenderEventArgs argss = e as PanelItemRenderEventArgs;
                        if (argss != null)
                        {
                            Point pt = argss.PanelStrip.Location;
                            brushs.TranslateTransform(-pt.X, -pt.Y, MatrixOrder.Append);
                        }

                        g.FillRectangle(brushs, rc);
                    }
                }
                if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.LightGray)
                {
                    using (LinearGradientBrush brushs = new LinearGradientBrush(rc, ColorTranslator.FromHtml("#F8F8F8"), ColorTranslator.FromHtml("#F8F8F8"), LinearGradientMode.Vertical))
                    {
                        PanelItemRenderEventArgs argss = e as PanelItemRenderEventArgs;
                        if (argss != null)
                        {
                            Point pt = argss.PanelStrip.Location;
                            brushs.TranslateTransform(-pt.X, -pt.Y, MatrixOrder.Append);
                        }

                        g.FillRectangle(brushs, rc);
                    }
                }

                if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.White)
                {
                    Color clor = Color.White;
                    using (SolidBrush brush = new SolidBrush(clor))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }
                if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.DarkGray)
                {
                    Color clor = ColorTranslator.FromHtml("#F0F0F0");
                    using (SolidBrush brush = new SolidBrush(clor))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }
                else if (ToolStipOffice2013ColorScheme == Office2013ColorScheme.LightGray)
                {
                    Color clor = ColorTranslator.FromHtml("#F8F8F8");
                    using (SolidBrush brush = new SolidBrush(clor))
                    {
                        g.FillRectangle(brush, rc);
                    }
                }

                if (selected)
                {

                }

                result = true;
            }

            return result;
        }

        private bool PaintGroupedToolbarBackground(ToolStripRenderEventArgs e)
        {
            return false;
        }

        private bool PaintDropdownBackground(ToolStripRenderEventArgs e)
        {
            return false;
        }

        private bool PaintStatusStripBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private void DrawCornerPoint(Graphics g, Point point)
        {
            g.FillRectangle(Brushes.White, new Rectangle(point.X - 1, point.Y - 1, 2, 2));
            g.FillRectangle(Brushes.DarkGray, new Rectangle(point.X - 2, point.Y - 2, 2, 2));
        }

        private bool PaintToolStripGalleryDropDownBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            if (e.ToolStrip is ToolStripGalleryDropDown)
            {
                ToolStripGalleryDropDown dropDown = (ToolStripGalleryDropDown)e.ToolStrip;
                if (dropDown.ShowGrip)
                {
                    Rectangle rect = new Rectangle(0, dropDown.Height - ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT,
                        dropDown.Width, ToolStripGalleryDropDown.GALLERY_DRAGGER_HEIGHT);
                    using (LinearGradientBrush b = new LinearGradientBrush(rect, Color.White, Color.White, LinearGradientMode.Vertical))
                    {
                        b.WrapMode = WrapMode.TileFlipXY;
                        e.Graphics.FillRectangle(b, rect);
                    }

                    Point p = new Point(rect.Right - 4, rect.Bottom - 3);
                    DrawCornerPoint(e.Graphics, p);
                    p.Y -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    p.Y += 4;
                    p.X -= 4;
                    DrawCornerPoint(e.Graphics, p);
                    result = true;
                }
            }


            return result;
        }

        private bool PaintMenuBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintContextMenuBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            RECT rect = new RECT();

            ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

            if (menuStrip != null)
            {
                if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                        Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                        Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            e.Graphics.FillRectangle(brush, rectangleBody);
                        }

                        using (Brush brush = new SolidBrush(Color.FromArgb(30, Color.LightGray)))
                        {
                            e.Graphics.FillRectangle(brush, rectangleTitle);
                        }

                        Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);

                        TextFormatFlags flags = TextFormatFlags.EndEllipsis;

                        if (menuStrip.RightToLeft == RightToLeft.Yes)
                        {
                            flags |= TextFormatFlags.Right;
                        }
                        
                        //TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, Color.Gray, flags);

                        result = true;
                    }
                }
            }
            return result;
        }

        private bool PaintBottomToolstripBackground(ToolStripRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintLauncher(Graphics g, Rectangle rcCaption, IToolStripExSupport2 iRibbon)
        {
            if (iRibbon != null && iRibbon.ShowLauncher)
            {
                Rectangle rc = ToolStripRendererUtils.GetLauncherBounds(rcCaption, false);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    Image image = iRibbon.LauncherSelected ? this.LauncerImageSelected : this.LauncherImageNormal;

                    if (image != null)
                    {
                        if (iRibbon is ToolStrip)
                        {
                            ToolStrip ts = iRibbon as ToolStrip;
                            if (ts.TopLevelControl is Form && !ts.TopLevelControl.ContainsFocus)
                            {
                                image = this.LauncherImageNormal;
                            }
                        }
                        g.DrawImage(image, rc.Location);
                    }

                    return true;
                }
            }

            return false;
        }

        private bool PaintButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (!PaintTabBarButtonBackground(e.Item, e.Graphics, Office12ToolStripRenderer.GetButtonRect(e.Item, new Rectangle(Point.Empty, e.Item.Size), Office12ToolStripRenderer.ERENDERTYPE.Normal)))
            {
                if (!PaintGroupedButtonBackground(e))
                {
                    if (!PaintCollapsedButtonBackground(e))
                    {
                        PaintToolBarButtonBackground(e);
                    }
                }
            }
            return true;
        }

        private bool PaintTabBarButtonBackground(ToolStripItem toolStripItem, Graphics graphics, Rectangle rectangle)
        {
            bool result = false;

            return result;
        }

        private bool PaintCollapsedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item is CollapsedDropDownButton)
            {
                if (!PaintCollapsedBackgroundPressed(e))
                {
                    if (!PaintCollapsedBackgroundSelected(e))
                    {
                        PaintCollapsedBackgroundNormal(e);
                    }
                }

                bResult = true;
            }

            return bResult;
        }

        private void PaintCollapsedBackgroundNormal(ToolStripItemRenderEventArgs e)
        {

        }

        private bool PaintCollapsedBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsSelected(item))
            {
                Rectangle rect = new Rectangle(Point.Empty, item.Size);

                using (GraphicsPath ellipse = new GraphicsPath())
                {
                    float offset = rect.Width / 4;
                    ellipse.AddEllipse(new RectangleF(offset / 2, 0, rect.Width - offset, rect.Height));
                    ellipse.Transform(new Matrix(1, 0, 0, 1, 0, (rect.Height / 2)));
                    using (PathGradientBrush p = new PathGradientBrush(ellipse))
                    {
                        p.CenterColor = colorTable.RibbonPanelBackgroundGradientBegin;
                        p.SurroundColors = new Color[] { Color.FromArgb(12, colorTable.RibbonPanelBackgroundGradientEnd) };
                        p.FocusScales = new PointF(0.6F, 0.1F);
                        e.Graphics.FillPath(p, ellipse);
                    }
                    result = true;
                }
            }
            return result;
        }

        private bool PaintCollapsedBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;
            if (item != null && ToolStripRendererUtils.GetIsPressed(item))
            {
                Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.DarkGray)                {
                    using (Brush brush = new SolidBrush(ColorTranslator.FromHtml("#E0E0E0")))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }
                else
                {
                    using (Brush brush = new SolidBrush(ColorTranslator.FromHtml("#F0F0F0")))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                }

                result = true;
            }

            return result;
        }

        private bool PaintArrow(ToolStripArrowRenderEventArgs e)
        {
            bool bResult = false;

            CollapsedDropDownButton item = e.Item as CollapsedDropDownButton;

            if (item != null)
            {
                Point loc = new Point();
                loc.Y = item.Height - item.Padding.Bottom - GetDownArrowSize().Height;
                loc.X = (item.Width - item.Padding.Horizontal - this.ArrowDownImage.Width) / 2 + item.Padding.Left;

                e.Graphics.DrawImage(this.ArrowDownImage, loc);

                bResult = true;
            }

            return bResult;
        }

        private bool PaintGroupedButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintToolBarButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = true;

            if (!PaintButtonBackgroundPressed(e))
            {
                if (!PaintButtonBackgroundChecked(e))
                {
                    result = PaintButtonBackgroundSelected(e);
                }
            }

            return result;
        }

        private bool PaintButtonBackgroundSelected(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
            bool isPanelNotInForm = false;
            if (!(e.ToolStrip.TopLevelControl is Form))
            {
                isPanelNotInForm = true;
            }
            if (ToolStripRendererUtils.GetIsSelected(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);

                if (isPanelNotInForm || (rc.Width > 0 && rc.Height > 0 && e.ToolStrip.TopLevelControl.ContainsFocus))
                {
                    Graphics g = e.Graphics;

                    Image img = this.GetToolstripItemImageSelected(rc);

                    g.DrawImage(img, Point.Empty);
                }
            }
            isPanelNotInForm = false;
            return result;
        }

        private bool PaintButtonBackgroundChecked(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (ToolStripRendererUtils.GetIsChecked(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);

                //Point[] polygon = RendererUtils.GetRoundedPolygon(rect, 2);

                //using (GraphicsPath path = new GraphicsPath())
                //{
                //path.AddPolygon(polygon);
                Color color;
                if (UseDefaultHighlightColor)
                    color = ColorTranslator.FromHtml("#b1d6f0");
                else
                    color = ControlPaint.LightLight(menuColor);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    //brush.SurroundColors = new Color[] { Color .Blue  };
                    //brush.CenterColor = Color.Blue;
                    // brush.FocusScales = new PointF(0.99f, 0.99f);

                    e.Graphics.FillRectangle(brush, rect);
                }



                result = true;
                //}
            }

            return result;
        }

        private bool PaintButtonBackgroundPressed(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (ToolStripRendererUtils.GetIsPressed(e.Item) && !ToolStripRendererUtils.GetIsDisabled(e.Item))
            {
                Rectangle rect = new Rectangle(0, 0, e.Item.Width - 1, e.Item.Height - 1);
                Color color;
                if (UseDefaultHighlightColor)
                    color = ColorTranslator.FromHtml("#b3deff");
                else
                    color = ControlPaint.Light(menuColor);
                using (SolidBrush brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                result = true;

            }

            return result;
        }

        private void PaintButtonBorder(ToolStripItemRenderEventArgs e, Point[] polygon, Color border)
        {
            using (Pen pen = new Pen(border))
            {
                e.Graphics.DrawPolygon(pen, polygon);
            }
        }

        private bool PaintSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            ToolStripSplitButton tsBtn = e.Item as ToolStripSplitButton;

            if (tsBtn != null && tsBtn.Enabled)
            {
                Rectangle rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.ButtonBounds.Height - 2));
                Rectangle rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.DropDownButtonBounds.Width, tsBtn.Bounds.Height - 2));

                bool paintBorderPressed = tsBtn.ButtonPressed || tsBtn.DropDownButtonPressed;
                bool paintBorderSelected = !paintBorderPressed && (tsBtn.DropDownButtonSelected || tsBtn.ButtonSelected);

                GraphicsState state = e.Graphics.Save();

                Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);

                //e.Graphics.SetClip(RendererUtils.GetRoundedRegion(rcBounds, 2), CombineMode.Intersect);

                #region Paint Button Background

                rcButton.X += 1; rcButton.Width -= 1;
                if (tsBtn.ButtonPressed)
                {
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcButton);
                }
                else if (tsBtn.ButtonSelected || tsBtn.DropDownButtonPressed)
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcButton);
                }

                #endregion

                #region Paint DropDownButtonBackground

                if (tsBtn.DropDownButtonPressed)
                {
                    rcDropDown.X -= 1;
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown);
                }
                else if (tsBtn.DropDownButtonSelected)
                {
                    PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown);
                }

                #endregion

               

                #region Paint Arrrow

                Color color = e.Item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;
                Rectangle rcArrow = Rectangle.Inflate(rcDropDown, -1, -1);

                bool bVertical = tsBtn != null && (tsBtn.Dock == DockStyle.Left || tsBtn.Dock == DockStyle.Right);
                ArrowDirection dir = bVertical ? ArrowDirection.Right : ArrowDirection.Down;

                base.DrawArrow(new ToolStripArrowRenderEventArgs(e.Graphics, e.Item, rcArrow, color, dir));

                #endregion

                e.Graphics.Restore(state);

                result = true;
            }

            return result;
        }

        private bool PaintSplitButtonBackgroundPressed(Graphics g, Rectangle rect)
        {
            Color color;
            if (UseDefaultHighlightColor)
                color = ColorTranslator.FromHtml("#b3deff");
            else
                color = ControlPaint.LightLight(menuColor);
            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, rect);
            }

            return true;
        }

        private bool PaintSplitButtonBackgroundSelected(Graphics g, Rectangle rect)
        {
            Color color;
            if (UseDefaultHighlightColor)
                color = ColorTranslator.FromHtml("#cde6f7");
            else
                color = ControlPaint.LightLight(menuColor);
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, color, color, LinearGradientMode.Vertical))
            {
                //this.SelectionBlend.Colors = this.SelectionColors;
              //  brush.InterpolationColors = this.SelectionBlend;

                g.FillRectangle(brush, rect);
            }

            return true;
        }

        private bool PaintOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            RibbonControlAdvHeader.QuickItemsOverflowButton item = e.Item as RibbonControlAdvHeader.QuickItemsOverflowButton;

            if (item != null)
            {
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        PaintButtonBackground(e);

                        Image imgArrow = this.GetToolstripItemImageSelected(Rectangle.Inflate(rc, 1, 1));

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);

                        result = true;
                    }
                }
            }
            else
            {
                ToolStripOverflowButton tsBtn = e.Item as ToolStripOverflowButton;
                Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                rc.Inflate(-3, -1);

                if (rc.Width > 0 && rc.Height > 0)
                {
                    ToolStrip ts = e.ToolStrip;
                    if (ts != null)
                    {
                        RibbonPanel panel = ts.Parent as RibbonPanel;
                        if (panel != null)
                        {
                            RibbonControlAdv ribbon = panel.Parent as RibbonControlAdv;
                            if (tsBtn != null)
                            {
                                if (tsBtn.Selected)
                                {
                                    using (Brush brush = new SolidBrush(Color.FromArgb(120, menuColor)))
                                    {
                                        e.Graphics.FillRectangle(brush, rc);
                                    }
                                }
                                else if (tsBtn.Pressed)
                                {
                                    using (Brush brush = new SolidBrush(Color.FromArgb(200, menuColor)))
                                    {
                                        e.Graphics.FillRectangle(brush, rc);
                                    }
                                }
                                else
                                {
                                    Brush br = new SolidBrush(Color.White);
                                    Color clr = Color.Empty;
                                    if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.DarkGray)
                                    {
                                        clr = ColorTranslator.FromHtml("#F0F0F0");
                                        using (br = new SolidBrush(clr))
                                        {
                                            e.Graphics.FillRectangle(br, rc);
                                        }
                                    }
                                    else if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.LightGray)
                                    {
                                        clr = ColorTranslator.FromHtml("#F8F8F8");
                                        using (br = new SolidBrush(clr))
                                        {
                                            e.Graphics.FillRectangle(br, rc);
                                        }
                                    }
                                    else if (this.ToolStipOffice2013ColorScheme == Office2013ColorScheme.White)
                                    {
                                        using (br = new SolidBrush(Color.FromArgb(150, Color.White)))
                                        {
                                            e.Graphics.FillRectangle(br, rc);
                                        }
                                    }
                                    br.Dispose();
                                }
                            }
                        }


                        bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;
                        Image imgArrow = bVertical ? ArrowRightImage : ArrowDownImage;

                        int left = rc.X + (rc.Width - imgArrow.Size.Width) / 2;
                        int top = rc.Y + (rc.Height - imgArrow.Size.Height) / 2;

                        e.Graphics.DrawImage(imgArrow, left, top);

                        result = true;
                    }
                }
            }

            return result;
        }

        internal bool PaintSplitButtonExBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;
            bool isPanelNotInForm = false;
            if (!(e.ToolStrip.TopLevelControl is Form))
            {
                isPanelNotInForm = true;
            }
            ToolStripSplitButtonEx tsBtn = e.Item as ToolStripSplitButtonEx;

            if (tsBtn != null && tsBtn.Enabled)
            {
                Rectangle rcButton = Rectangle.Empty;
                Rectangle rcDropDown = Rectangle.Empty;
                if (tsBtn.RightToLeft == RightToLeft.Yes)
                {
                    rcDropDown = new Rectangle(Point.Empty, new Size(tsBtn.ButtonBounds.Width, tsBtn.Bounds.Height - 2));
                    rcButton = new Rectangle(new Point(rcDropDown.Right, rcDropDown.Top), new Size(tsBtn.ImageBounds.Width, tsBtn.ImageBounds.Height - 2));
                }
                else
                {
                    rcButton = new Rectangle(Point.Empty, new Size(tsBtn.ImageBounds.Width, tsBtn.ImageBounds.Height - 2));
                    rcDropDown = new Rectangle(new Point(rcButton.Right, rcButton.Top), new Size(tsBtn.ButtonBounds.Width, tsBtn.Bounds.Height - 2));
                }

                bool paintBorderPressed = tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImagePressed;
                bool paintBorderSelected = !paintBorderPressed && (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImageSelected || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected);

                GraphicsState state = e.Graphics.Save();

                Rectangle rcBounds = new Rectangle(0, 0, tsBtn.Width - 2, tsBtn.Height - 2);

               

                #region Paint Button Background

                rcButton.X += 1; rcButton.Width -= 1;
                if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImagePressed || tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed)
                {
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcButton);
                }
                else if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ImageSelected)
                {
                    if(isPanelNotInForm || e.ToolStrip.TopLevelControl.ContainsFocus)
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcButton);
                }

                #endregion

                #region Paint DropDownButtonBackground

                if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonPressed)
                {
                    rcDropDown.X -= 1;
                    PaintSplitButtonBackgroundPressed(e.Graphics, rcDropDown);
                }
                else if (tsBtn.ButtonState == ToolStripSplitButtonEx.SplitButtonState.ButtonSelected)
                {
                    if (isPanelNotInForm || e.ToolStrip.TopLevelControl.ContainsFocus)
                        PaintSplitButtonBackgroundSelected(e.Graphics, rcDropDown);
                }

                #endregion



                #region Paint Text, Image and Arrow

                Image image = tsBtn.Image;

                // Draw image.
                if ((tsBtn.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    if (image != null)
                    {
                        bool bDisposeImage = false;

                        if (!tsBtn.Enabled)
                        {
                            image = CreateDisabledImage(image);
                            bDisposeImage = true;
                        }

                        e.Graphics.DrawImage(image, tsBtn.InternalLayout.ImageRectangle);

                        if (bDisposeImage)
                        {
                            image.Dispose();
                        }
                    }
                }

                Color clForeColor = !tsBtn.Enabled ? SystemColors.GrayText : tsBtn.ForeColor;

                // Draw text.
                if ((tsBtn.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                {
                    TextRenderer.DrawText(e.Graphics, tsBtn.Text, tsBtn.Font, tsBtn.InternalLayout.TextRectangle, clForeColor, ToolStripRendererUtils.GetTextFormatFlags(tsBtn.TextAlign));
                }

                // Draw arrow.
                Rectangle arrowRectangle = tsBtn.InternalLayout.DropDownButtonRectangle;

                using (Brush brush = new SolidBrush(clForeColor))
                {
                    Point pt = new Point(arrowRectangle.Left + (arrowRectangle.Width / 2), arrowRectangle.Top + (arrowRectangle.Height / 2));
                    Point[] points = new Point[] { new Point(pt.X - 2, pt.Y - 1), new Point(pt.X + 3, pt.Y - 1), new Point(pt.X, pt.Y + 2) };

                    e.Graphics.FillPolygon(brush, points);
                }

                #endregion

                e.Graphics.Restore(state);

                result = true;
            }
            isPanelNotInForm = false;
            return result;
        }

        private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            if (e.Item.IsOnDropDown)
            {
                result = PaintDropDownMenuItemBackground(e);
            }
            else
            {
                result = PaintToolStripMenuItemBackground(e);
            }
            return result;
        }

        private bool PaintToolStripMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            return result;
        }

        private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool result = false;

            ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

            if ((tsItem != null && tsItem.Enabled) && tsItem.Selected || tsItem.Pressed)
            {
                Image image = GetToolstripItemImageSelected(new Rectangle(Point.Empty, tsItem.Size));

                e.Graphics.DrawImage(image, Point.Empty);

                result = true;
            }

            return result;
        }


        #endregion

        #region ** Office2013ToolStripRenderer Utilities

        public class Office2013ToolStripRendererUtils
        {
            #region Fields

            Office2013ToolStripRenderer renderer = null;
            Hashtable htControls = null;
            object m_controlsLock = null;

            #endregion

            #region Properties

            public Hashtable HtControls
            {
                get { return htControls; }
            }

            public Office2013ToolStripRenderer Renderer
            {
                get { return renderer; }
            }

            #endregion

            #region Ctor
            public Office2013ToolStripRendererUtils(Office2013ToolStripRenderer renderer)
            {
                this.renderer = renderer;
                htControls = new Hashtable();
                m_controlsLock = new object();
            }
            #endregion

            #region Attach - Detach ToolStrip

            private void Attach(ToolStrip ts)
            {
                Subscribe(ts);
            }

            private void Detach(ToolStrip toolstrip)
            {
                Unsubscribe(toolstrip);
            }

            private void Subscribe(ToolStrip ts)
            {
                ts.RendererChanged += new EventHandler(OnRendererChanged);

                if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
                {
                    ts.HandleCreated += new EventHandler(OnHandleCreated);
                    ts.HandleDestroyed += new EventHandler(OnHandleDestroyed);
                    ts.DockChanged += new EventHandler(OnDockChanged);

                    if (ts.IsHandleCreated)
                    {
                        OnHandleCreated(ts, EventArgs.Empty);
                    }

                    if (ts.GetType() == typeof(ToolStrip))
                    {
                        ts.LayoutCompleted += new EventHandler(OnLayoutCompleted);
                    }
                }
                else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
                {
                    ts.HandleCreated += new EventHandler(OnRoundedToolStripRegionChanged);
                    ts.SizeChanged += new EventHandler(OnRoundedToolStripRegionChanged);
                }

                AttachItems(ts);
            }

            private void Unsubscribe(ToolStrip ts)
            {
                DetachItems(ts);

                ts.RendererChanged -= new EventHandler(OnRendererChanged);

                if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
                {
                    ts.HandleCreated -= new EventHandler(OnHandleCreated);
                    ts.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
                    ts.DockChanged -= new EventHandler(OnDockChanged);

                    if (ts.GetType() == typeof(ToolStrip))
                    {
                        ts.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
                    }

                    if (!(ts.Renderer is Office12ToolStripRenderer))
                    {
                        ReleaseHandle(ts);
                        ClearDropDownRegions(ts);
                    }
                }
                else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
                {
                    ts.HandleCreated -= new EventHandler(OnRoundedToolStripRegionChanged);
                    ts.SizeChanged -= new EventHandler(OnRoundedToolStripRegionChanged);

                    if (!(ts.Renderer is Office12ToolStripRenderer))
                    {
                        ts.Region = null;
                    }
                }
            }

            #endregion

            #region Attach - Detach ToolStripItems

            private void AttachItems(ToolStrip ts)
            {
                ts.ItemAdded += new ToolStripItemEventHandler(OnItemAdded);
                ts.ItemRemoved += new ToolStripItemEventHandler(OnItemRemoved);

                foreach (ToolStripItem item in ts.Items)
                {
                    OnItemAdded(ts, new ToolStripItemEventArgs(item));
                    if(ts.Parent is RibbonControlAdv)
                    (ts.Parent as RibbonControlAdv).HeaderInternal.QuickItemAdded += new ToolStripItemEventHandler(HeaderInternal_QuickItemAdded);
                }
            }

            void HeaderInternal_QuickItemAdded(object sender, ToolStripItemEventArgs e)
            {
                OnItemAdded(sender,e);
            }

            private void DetachItems(ToolStrip ts)
            {
                ts.ItemAdded -= new ToolStripItemEventHandler(OnItemAdded);
                ts.ItemRemoved -= new ToolStripItemEventHandler(OnItemRemoved);

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    foreach (ToolStripItem item in ts.Items)
                    {
                        OnItemRemoved(ts, new ToolStripItemEventArgs(item));
                    }
                }
            }

            #endregion

            #region Event Handlers

            void OnItemRemoved(object sender, ToolStripItemEventArgs e)
            {
                if (e.Item is ToolStripComboBox)
                {
                    OnRemovedComboBox(e.Item as ToolStripComboBox);
                }
            }

            private void OnRemovedComboBox(ToolStripComboBox toolStripComboBox)
            {
                Control comboBox = toolStripComboBox.Control;

                if (comboBox != null)
                {
                    comboBox.HandleCreated -= new EventHandler(OnComboBoxHandleCreated);
                    comboBox.HandleDestroyed -= new EventHandler(OnComboBoxHandleDestroyed);

                    if (comboBox.IsHandleCreated)
                    {
                        ReleaseControl(comboBox);
                    }
                }
            }

            void OnItemAdded(object sender, ToolStripItemEventArgs e)
            {
                if (e.Item is ToolStripComboBox)
                {
                    OnAddedComboBox(e.Item as ToolStripComboBox);
                }
                else if (e.Item is ToolStripDropDownItem)
                {
                    OnAddedDropDownItem(e.Item as ToolStripDropDownItem);
                }
            }

            private void OnAddedDropDownItem(ToolStripDropDownItem toolStripDropDownItem)
            {

            }

            private void OnAddedComboBox(ToolStripComboBox toolStripComboBox)
            {
                Control comboBox = toolStripComboBox.Control;

                if (comboBox != null)
                {
                    comboBox.HandleCreated += new EventHandler(OnComboBoxHandleCreated);
                    comboBox.HandleDestroyed += new EventHandler(OnComboBoxHandleDestroyed);

                    if (comboBox.IsHandleCreated)
                    {
                        SubclassControl(comboBox, this.Renderer.ComboRenderer);
                    }
                }
            }

            void OnComboBoxHandleCreated(object sender, EventArgs e)
            {
                SubclassControl(sender as Control, this.Renderer.ComboRenderer);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void OnComboBoxHandleDestroyed(object sender, EventArgs e)
            {
                ReleaseControl(sender as Control);
            }

            void OnRoundedToolStripRegionChanged(object sender, EventArgs e)
            {

            }

            void OnLayoutCompleted(object sender, EventArgs e)
            {

            }

            void OnDockChanged(object sender, EventArgs e)
            {

            }

            void OnHandleDestroyed(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;
                if (ts != null)
                {
                    ReleaseHandle(ts);
                }
            }

            void OnHandleCreated(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;
                if (ts != null)
                {
                    AssignHandle(ts);
                }
            }

            void OnRendererChanged(object sender, EventArgs e)
            {
                ToolStrip ts = sender as ToolStrip;

                if (ts != null && ts.Renderer != this.Renderer)
                {
                    Detach(ts);
                }
            }
            #endregion

            #region Implementations

            public void Initialize(ToolStrip toolstrip)
            {
                if (toolstrip != null)
                {
                    Attach(toolstrip);
                }
            }

            private void ClearDropDownRegions(ToolStrip ts)
            {

            }

            private void AssignHandle(ToolStrip ts)
            {
                lock (m_controlsLock)
                {
                    if (!htControls.ContainsKey(ts))
                    {
                        Office12ToolStripRenderer.CToolStripWindow wnd = new Office12ToolStripRenderer.CToolStripWindow(ts);

                        wnd.AssignHandle(ts.Handle);
                        htControls.Add(ts, wnd);
                    }
                }
                using(Region region =new Region(new Rectangle(Point.Empty, ts.Size)))
                ts.Region = region;

                ToolStripRendererUtils.UpdateFrame(ts);
            }

            private void ReleaseHandle(ToolStrip ts)
            {
                lock (m_controlsLock)
                {
                    object obj = htControls[ts];
                    if (obj != null)
                    {
                        Office12ToolStripRenderer.CToolStripWindow wnd = obj as Office12ToolStripRenderer.CToolStripWindow;

                        if (wnd != null)
                        {
                            wnd.ReleaseHandle();
                        }

                        htControls.Remove(ts);
                    }
                }

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ts.Region = null;
                }

                ToolStripRendererUtils.UpdateFrame(ts);
            }

            internal bool GetIsSelected(ToolStrip ts)
            {
                Office12ToolStripRenderer.CToolStripWindow cwd = this.HtControls[ts] as Office12ToolStripRenderer.CToolStripWindow;

                if (cwd != null)
                {
                    return cwd.Selected;
                }
                else
                    return false;
            }

            void SubclassControl(Control control, INativeMessageFilter messageFilter)
            {
                lock (m_controlsLock)
                {
                    NativeMessageHandler handler = htControls[control] as NativeMessageHandler;

                    if (handler == null)
                    {
                        handler = new NativeMessageHandler();
                        htControls.Add(control, handler);

                        handler.Assign(control.Handle);
                    }

                    handler.MessageFilter = messageFilter;
                }
            }

            void ReleaseControl(Control control)
            {
                lock (m_controlsLock)
                {
                    object obj = htControls[control];
                    if (obj != null)
                    {
                        NativeMessageHandler handler = obj as NativeMessageHandler;

                        if (handler != null)
                        {
                            handler.MessageFilter = null;
                        }

                        htControls.Remove(control);
                    }
                }
            }

            #endregion
        }

        #endregion
    }

    #endregion
    #region MetroToolStripRenderer
    /// <summary>
    /// 
    /// </summary>
    partial class newMetroToolStripRenderer : Office12ToolStripRenderer
    {
        #region Constants
        const int STATICEDGE_WIDTH = 1;
        const int ETCHED_WIDTH = 2;
        #endregion

        #region Constructors/destructors

        static newMetroToolStripRenderer()
        {
            m_blMenuItemUp = new Blend();
            m_blMenuItemUp.Positions = new float[] { 0.0F, 0.4F, 0.5F, 1.0F };
            m_blMenuItemUp.Factors = new float[] { 0.0F, 0.2F, 1.0F, 0.4F };

            m_blMenuItemDown = new Blend();
            m_blMenuItemDown.Positions = new float[] { 0.0F, 0.3F, 0.32F, 1.0F };
            m_blMenuItemDown.Factors = new float[] { 1.0F, 0.5F, 0.4F, 0.0F };

           
        }

        public Color MetroColor;
        public newMetroToolStripRenderer(Color _metroColor)
            : this(new Office12ColorTable(), ERENDERTYPE.Normal)
        {
            MetroColor = _metroColor;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="erType"></param>
        public newMetroToolStripRenderer(ERENDERTYPE erType)
            : this(new Office12ColorTable(), erType)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorTable"></param>
        public VisualStyle Style;
        public newMetroToolStripRenderer(Office12ColorTable colorTable)
            : this(colorTable, ERENDERTYPE.Normal)
        {

        }
        public newMetroToolStripRenderer(VisualStyle style)
        {
            Style = style;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="erType"></param>
        /// <param name="colorTable"></param>
        public newMetroToolStripRenderer(Office12ColorTable colorTable, ERENDERTYPE erType)
            : base(colorTable)
        {
            RoundedEdges = false;

            m_eRenderType = erType;
 
        }
        #endregion

        #region Overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toolStrip"></param>
        protected override void Initialize(ToolStrip toolStrip)
        {
            base.Initialize(toolStrip);

            if (toolStrip != null)
            {
                Attach(toolStrip);
            }
        }
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            //base.OnRenderItemCheck(e);
            paintItemCheckBackGround(e);
        }
        private bool paintItemCheckBackGround(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle rc = GetCheckRect(e);
            if (rc.Width > 0 && rc.Height > 0)
            {
                Size szImage = this.CheckButton.Size;

                int iImageX = rc.X + (rc.Width - szImage.Width) / 2;
                int iImageY = rc.Y + (rc.Height - szImage.Height) / 2;

                e.Graphics.DrawImage(this.CheckButton, new Point(iImageX, iImageY));
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        protected override void InitializeItem(ToolStripItem item)
        {
            base.InitializeItem(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                base.OnRenderToolStripBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBorder(e))
            {
                base.OnRenderToolStripBorder(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintMenuItemBackground(e))
            {
                base.OnRenderMenuItemBackground(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintImageMargin(e))
            {
                base.OnRenderImageMargin(e);
            }
        }

        #endregion

        #region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHandleCreated(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                AssignHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHandleDestroyed(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;
            if (ts != null)
            {
                ReleaseHandle(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRendererChanged(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && ts.Renderer != this)
            {
                Detach(ts);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDockChanged(object sender, EventArgs e)
        {
            ToolStripRendererUtils.UpdateFrame(sender as ToolStrip);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLayoutCompleted(object sender, EventArgs e)
        {
            ToolStrip ts = sender as ToolStrip;

            if (ts != null && HasCaption(ts))
            {
                if (ts.AutoSize)
                {
                    bool bVertical = ts.Dock == DockStyle.Left || ts.Dock == DockStyle.Right;

                    int nMaxBound = bVertical ? ts.Padding.Left : ts.Padding.Top;

                    ToolStripItemCollection items = ts.Items;

                    for (int i = 0, count = items.Count; i < count; i++)
                    {
                        ToolStripItem item = items[i];
                        if (item.Placement == ToolStripItemPlacement.Main)
                        {
                            int nItemBound = bVertical ? item.Bounds.Right : item.Bounds.Bottom;
                            if (nItemBound > nMaxBound)
                            {
                                nMaxBound = nItemBound;
                            }
                        }
                    }

                }
            }
        }
   

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRoundedToolstripRegionChanged(object sender, EventArgs e)
        {
            ToolStrip toolstrip = sender as ToolStrip;

            if (toolstrip != null && toolstrip.IsHandleCreated)
            {
                toolstrip.Region = RendererUtils.GetRoundedRegion(new Rectangle(Point.Empty, toolstrip.Size), TOOLSTRIP_RADIUS);
            }
        }
        #endregion

        #region Implementation
 
        void Attach(ToolStrip ts)
        {
            Subscribe(ts);

            m_refCounter++;

            if (m_refCounter == 1)
            {
                SystemInfo.SettingsChanged += new SettingsChangedEventHandler(SettingsChangedHandler);
            }
        }

        void Detach(ToolStrip ts)
        {
            Unsubscribe(ts);

            if (m_refCounter > 0)
            {
                m_refCounter--;

                if (m_refCounter == 0)
                {
                    SystemInfo.SettingsChanged -= new SettingsChangedEventHandler(SettingsChangedHandler);

                }
            }
        }

        #region Attach/Detach items
   
        #endregion

        void Subscribe(ToolStrip ts)
        {
            ts.RendererChanged += new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated += new EventHandler(OnHandleCreated);
                ts.HandleDestroyed += new EventHandler(OnHandleDestroyed);
                ts.DockChanged += new EventHandler(OnDockChanged);

                if (ts.IsHandleCreated)
                {
                    OnHandleCreated(ts, EventArgs.Empty);
                }

                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted += new EventHandler(OnLayoutCompleted);
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated += new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged += new EventHandler(OnRoundedToolstripRegionChanged);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void Unsubscribe(ToolStrip ts)
        {


            ts.RendererChanged -= new EventHandler(OnRendererChanged);

            if (ts is IToolStripExSupport || ts.GetType() == typeof(ToolStrip))
            {
                ts.HandleCreated -= new EventHandler(OnHandleCreated);
                ts.HandleDestroyed -= new EventHandler(OnHandleDestroyed);
                ts.DockChanged -= new EventHandler(OnDockChanged);

                if (ts.GetType() == typeof(ToolStrip))
                {
                    ts.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
                }

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ReleaseHandle(ts);
                  
                }
            }
            else if (ts is ContextMenuStripEx || ts is BottomToolstrip || ts is ToolStripGalleryDropDown)
            {
                ts.HandleCreated -= new EventHandler(OnRoundedToolstripRegionChanged);
                ts.SizeChanged -= new EventHandler(OnRoundedToolstripRegionChanged);

                if (!(ts.Renderer is Office12ToolStripRenderer))
                {
                    ts.Region = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void AssignHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                if (!m_htControls.ContainsKey(ts))
                {
                    CToolStripWindow wnd = new CToolStripWindow(ts);

                    wnd.AssignHandle(ts.Handle);
                    m_htControls.Add(ts, wnd);
                }
            }

            ts.Region = ToolStripRendererUtils.GetRegion(ts);

            ToolStripRendererUtils.UpdateFrame(ts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ts"></param>
        void ReleaseHandle(ToolStrip ts)
        {
            lock (m_controlsLock)
            {
                object obj = m_htControls[ts];
                if (obj != null)
                {
                    CToolStripWindow wnd = obj as CToolStripWindow;

                    if (wnd != null)
                    {
                        wnd.ReleaseHandle();
                    }

                    m_htControls.Remove(ts);
                }
            }

            if (!(ts.Renderer is Office12ToolStripRenderer))
            {
                ts.Region = null;
            }

            ToolStripRendererUtils.UpdateFrame(ts);
        }
   

        private bool PaintToolStripBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

          if (e.ToolStrip is ContextMenuStripEx)
            {
                bResult = PaintContextMenuBackground(e);
            }

            else
   
                if (e.ToolStrip is ToolStripDropDown)
                    {
                        bResult = PaintDropdownBackground(e);
                    }


            return bResult;
        }

        private bool PaintToolStripBorder(ToolStripRenderEventArgs e)
        {
            bool bResult = true;

            if (e.ToolStrip.Width > 0 && e.ToolStrip.Height > 0)
            {
                Rectangle rc = new Rectangle(new Point(0,0),e.ToolStrip.Size);
                Pen p = new Pen(Color.FromArgb(230, 231, 232),1);
                if (e.ToolStrip.IsDropDown)
                {
                    
                   
                    e.Graphics.DrawRectangle(p, rc.X + 1, rc.Y + 1, rc.Width - 3, rc.Height - 3);
                }
                if (e.ToolStrip is MenuStrip)
                {
                    Pen pen = new Pen(Color.White);
                    e.Graphics.DrawLine(MenuStripBorder, new Point(rc.X, rc.Bottom - 1), new Point(rc.Right - 1, rc.Bottom - 1));
                    pen.Dispose();
                   
                }

            }
            return bResult;
        }

        private bool PaintDropdownBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;
        
            ToolStripDropDown ts = e.ToolStrip as ToolStripDropDown;
            if (ts != null)
            {
                if (ts.Width > 0 && ts.Height > 0)
                {
                    Rectangle rc = new Rectangle(Point.Empty, ts.Size);
                    SolidBrush s = new SolidBrush(Color.White);
                    e.Graphics.FillRectangle(s, rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 4);
                    s.Dispose();
                }
                bResult = true;
            }

            return bResult;
        }

        private bool PaintMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            ToolStrip ms = e.ToolStrip;
            if (ms != null)
            {
                Size szMenu = ms.Size;
                if (szMenu.Width > 0 && szMenu.Height > 0)
                {
                    Rectangle rcMenu = new Rectangle(Point.Empty, szMenu);

                    Color clBegin = ColorTable.MenuStripGradientBegin;
                    Color clEnd = ColorTable.MenuStripGradientEnd;
                    Control parent = ms.Parent;
                    if (parent != null)
                    {
                        Size szParent = parent.ClientSize;
                        if (szParent.Width > 0 && szParent.Height > 0)
                        {
                            Rectangle rcParent = new Rectangle(Point.Empty, szParent);
                            using (LinearGradientBrush brush = GetHorizontalBrush(ref rcParent, clBegin, clEnd))
                            {
                                brush.TranslateTransform(szParent.Width - ms.Location.X, szParent.Height - ms.Location.Y, MatrixOrder.Append);

                                brush.Blend = m_blMenuBar;
                                e.Graphics.FillRectangle(brush, rcMenu);
                            }
                        }
                    }
                    else
                    {
                        using (LinearGradientBrush brush = GetHorizontalBrush(ref rcMenu, clBegin, clEnd))
                        {
                            brush.Blend = m_blMenuBar;
                            e.Graphics.FillRectangle(brush, rcMenu);
                        }
                    }
                    bResult = true;
                }
            }

            return bResult;
        }

        private bool PaintContextMenuBackground(ToolStripRenderEventArgs e)
        {
            bool bResult = false;

            RECT rect = new RECT();

            ContextMenuStripEx menuStrip = e.ToolStrip as ContextMenuStripEx;

            if (menuStrip != null)
            {
                if (WindowsAPI.GetWindowRect(menuStrip.Handle, ref rect))
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        Graphics g = e.Graphics;

                        Rectangle rc = new Rectangle(Point.Empty, rect.Size);

                        Rectangle rectangleTitle = new Rectangle(0, 0, rc.Width, menuStrip.TitleHeight);
                        Rectangle rectangleBody = new Rectangle(0, menuStrip.TitleHeight, rc.Width, rc.Height - menuStrip.TitleHeight);

                        using (Brush brush = new SolidBrush(this.OfficeColorTable.ToolStripDropDownBackground))
                        {
                            SolidBrush z = new SolidBrush(Color.White);
                            e.Graphics.FillRectangle(z, rc.X + 2, rc.Y + 2, rc.Width - 4, rc.Height - 4);
                            z.Dispose();

                        }

                        Rectangle rcText = Rectangle.Inflate(rectangleTitle, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN, -ContextMenuStripEx.CONTEXTMENUSTRIP_MARGIN);
                        TextFormatFlags flags = TextFormatFlags.EndEllipsis;
                        if (menuStrip.RightToLeft == RightToLeft.Yes)
                        {
                            flags |= TextFormatFlags.Right;
                        }

                        TextRenderer.DrawText(g, menuStrip.Text, menuStrip.HeaderFont, rcText, this.OfficeColorTable.RibbonTabText, flags);

                        bResult = true;
                    }
                }
            }

            return bResult;
        }
       
        private bool PaintMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            if (e.Item.IsOnDropDown)
            {
                bResult = PaintDropDownMenuItemBackground(e);
            }

            return bResult;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool PaintImageMargin(ToolStripRenderEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rc = Rectangle.Inflate(e.AffectedBounds, -1, -2);

            if (e.ToolStrip.RightToLeft == RightToLeft.Yes)
            {
                rc.X = e.ToolStrip.ClientRectangle.Width - rc.Width;
            }

            if (e.ToolStrip is ContextMenuStripEx)
            {
                ContextMenuStripEx statusStrip = e.ToolStrip as ContextMenuStripEx;

                rc.Y += statusStrip.TitleHeight;
                rc.Height -= statusStrip.TitleHeight;
            }


            using (Brush brush = new SolidBrush(Color.White))
            {
                Pen pen= new Pen(ColorTranslator.FromHtml("#E6E7E8"));
                g.FillRectangle(brush, rc.X, rc.Y - 1, rc.Width, rc.Height + 2);
                g.DrawLine(pen, new Point(rc.Width, rc.Y + 2), new Point(rc.Width, rc.Height - 2));
                pen.Dispose();
            }

            return true;
        }

        private bool PaintDropDownMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            bool bResult = false;

            SolidBrush Brush2 = new SolidBrush( MetroColor);
            Pen pen = new Pen(Color.FromArgb(28,181,229));
            ToolStripMenuItem tsItem = e.Item as ToolStripMenuItem;

            if (tsItem != null && tsItem.Selected || tsItem.Pressed)
            {
                Rectangle rc = GetItemRect(tsItem);

                if (tsItem.Enabled)
                {
                    using (Region rgBackground = RendererUtils.GetRoundedRegion(rc, MENUITEM_RADIUS))
                    {
                        Graphics g = e.Graphics;

                        using (LinearGradientBrush brush = GetVerticalBrush(ref rc, ColorTable.MenuItemPressedGradientBegin, ColorTable.MenuItemPressedGradientEnd))
                        {
                            
                            brush.Blend = ToolStripRendererUtils.GetIsPressed(tsItem) ? m_blMenuItemDown : m_blMenuItemUp;
                            e.Graphics.FillRectangle(Brush2, rc.X-1, rc.Y-1, rc.Width+2 , rc.Height);
                            e.Graphics.DrawRectangle(pen, rc.X , rc.Y + 1, rc.Width - 1, rc.Height-2);
                        }

                    }
                   
                }
                pen.Dispose();
                Brush2.Dispose();
                bResult = true;
            }
            return bResult;
        }

        private Rectangle GetItemRect(ToolStripItem item)
        {
            Rectangle rc = new Rectangle(Point.Empty, item.Size);

            if (item.Owner is ToolStripDropDownMenu)
            {
                rc.X += MENUITEM_PADDING_LEFT;
                rc.Width -= MENUITEM_HORIZONTAL;
            }

            return rc;
        }

        private Rectangle GetButtonBounds(ToolStripItem item)
        {
            return GetButtonRect(item, item.Bounds, this.RenderType);
        }

        private Rectangle GetButtonBounds(ToolStripItem item, ERENDERTYPE erType)
        {
            return GetButtonRect(item, item.Bounds, erType);
        }

        private Rectangle GetButtonRect(ToolStripItem item, Rectangle rc)
        {
            return GetButtonRect(item, rc, this.RenderType);
        }

        private ERENDERTYPE GetRenderType(ToolStrip ts)
        {
            if (ts != null)
            {
                IToolStripExSupport iRibbon = ts as IToolStripExSupport;
                if (iRibbon != null && iRibbon.GroupedButtons)
                {
                    return ERENDERTYPE.Grouped;
                }
            }

            return this.RenderType;
        }
   
        private void Clear(Hashtable hTable)
        {
            foreach (Object obj in hTable)
            {
                IDisposable iDispose = obj as IDisposable;

                if (iDispose != null)
                {
                    iDispose.Dispose();
                }
            }
            hTable.Clear();
        }

        #endregion

        #region Fields
        private const int MENUITEM_PADDING_LEFT = 2;
        private const int MENUITEM_PADDING_RIGHT = 1;
        private const int MENUITEM_HORIZONTAL = (MENUITEM_PADDING_LEFT + MENUITEM_PADDING_RIGHT);
        private const int IMAGE_PADDING = 2;
        private const int IMAGE_MARGIN = 1;
        private ERENDERTYPE m_eRenderType;
        private int m_refCounter = 0;
        static Hashtable m_htControls=null;
        static Object m_controlsLock=null;

        #endregion


    }
    #endregion
   
    #region ** ToolStripRendererUtils
    
    #endregion
}
#endif