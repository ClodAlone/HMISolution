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
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.ComponentModel;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
    [Syncfusion.Documentation.DocumentationExclude()]
    public class ThemedTabDrawing : ThemedControlDrawing
    {
        TabControlAdv tabControl = null;

        public ThemedTabDrawing(TabControlAdv tabControl, string classList)
            : base(classList)
        {
            this.tabControl = tabControl;
        }

        public void DrawTabPane(Graphics g, Rectangle rect)
        {
            this.DrawThemeBackground(g, ThemeParts.TABP_PANE,
                0, rect);
        }

        public void DrawTabBody(Graphics g, Rectangle rect, Rectangle clipRect)
        {
            this.DrawThemeBackground(g, ThemeParts.TABP_BODY,
                0, rect, clipRect);
        }

        public void DrawTabBody(Graphics g, Rectangle rect)
        {
            this.DrawThemeBackground(g, ThemeParts.TABP_BODY,
                0, rect);
        }

        public void DrawTabBackground(Graphics g, ITabRenderer renderer, bool selectedTab, Rectangle rect)
        {
            int state = ThemeStates.TIS_NORMAL;
            if (selectedTab)
                state = ThemeStates.TIS_SELECTED;
            else if (renderer.HotTrack)
                state = ThemeStates.TIS_HOT;

            this.DrawThemeBackground(g, ThemeParts.TABP_TABITEM,
                state, rect);
        }

        public void DetachTabControl()
        {
            XPThemes.UnregisterControlDrawing(this);
            this.tabControl = null;
        }

        protected override void Dispose(bool disposing)
        {
            DetachTabControl();

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRenderer3D"/>
    /// tab style.
    /// </summary>
    public class TabPanelProperty3D : TabUIDefaultProperties
    {
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DrawLeftToRight"/>.
        /// </summary>
        public override bool DrawLeftToRight
        {
            get { return false; }
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.GetOverlapSize"/>.
        /// </summary>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(TabRenderer3D.OVERLAPX, TabRenderer3D.OVERLAPY);
        }
    }

    /// <summary>
    /// The tab renderer that implements the 3D tabs look-and-feel.
    /// </summary>
    public class TabRenderer3D : TabRendererBase
    {
        private const int DEF_LABELEDIT_OFFSET_Y_BOTTOM = 3;
        private const int c_iCloseButtonPaddingThemes = 10;
        private const int c_iCloseButtonPadding = 5;
        private const int c_iCorrectCloseButtonX = -3;

        /// <summary>
        /// The default horizontal overlap.
        /// </summary>
        public static readonly int OVERLAPX = 6;
        /// <summary>
        /// The default vertical overlap.
        /// </summary>
        public static readonly int OVERLAPY = 3;

        static TabPanelProperty3D tabPanelPropertyExtender;
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static TabPanelProperty3D TabPanelPropertyExtender
        {
            get { return tabPanelPropertyExtender; }
        }

        /// <summary>
        /// Returns the unique name of this tab renderer.
        /// </summary>
        public static string TabStyleName
        {
            get
            {
                return "3D";
            }
        }

        static TabRenderer3D()
        {
            tabPanelPropertyExtender = new TabPanelProperty3D();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRenderer3D), TabPanelPropertyExtender);
        }

        public static void RegisterTabType()
        {
            tabPanelPropertyExtender = new TabPanelProperty3D();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRenderer3D), TabPanelPropertyExtender);
        }
        /// <summary>
        /// Creates a new instance of the TabRenderer3D class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRenderer3D(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }

        public override int LabelEditOffsetY
        {
            get
            {
                int offsetY = 0;

                if (this.TabAlignment == TabAlignment.Bottom)
                {
                    offsetY = DEF_LABELEDIT_OFFSET_Y_BOTTOM;
                }

                return offsetY;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.GetOverlapSize"/>.
        /// </summary>
        /// <returns></returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return tabPanelPropertyExtender.GetOverlapSize(tabSize);
        }
        /// <summary>
        /// Adjusts the bounds and graphics based on alignment.
        /// </summary>
        protected RectangleF AdjustBoundsAndGraphicsForAlignment(Graphics g, RectangleF oldBounds, TabAlignment align)
        {
            // Increase tab bounds based on alignment
            RectangleF newBounds = oldBounds;
            switch (align)
            {
                case TabAlignment.Top: newBounds.Height += 3; break;
                case TabAlignment.Right: newBounds.Offset(-3, 0); newBounds.Width += 3; break;
                case TabAlignment.Left: newBounds.Width += 3; break;
                case TabAlignment.Bottom: newBounds.Offset(0, -3); newBounds.Height += 3; break;
            }
            g.SetClip(new Region(oldBounds), CombineMode.Intersect);
            return newBounds;
        }
        /// <summary>
        /// Returns the polygonal border of the tab from the bounds.
        /// </summary>
        /// <param name="bounds">The tab's bounds.</param>
        /// <returns>The polygonal border.</returns>
        public PointF[] GetPolygonFromBounds(RectangleF bounds)
        {
            bool bIsMirrored = panelRenderer.IsMirrored;

            // Array of X coordinates for 3D tab borders drawing
            float[] afX = bIsMirrored ?
                // RTL X-coordinates
                new float[4] { bounds.Right - 1, bounds.Right - 3, bounds.Left + 2, bounds.Left } :
                // Default X-coordinates (for LTR)
                new float[4] { bounds.Left, bounds.Left + 2, bounds.Right - 3, bounds.Right - 1 };

            PointF[] polygon =
			{
				new PointF( afX[0], bounds.Bottom - 3 ),
				new PointF( afX[0], bounds.Top + 2),
				new PointF( afX[1], bounds.Top ),
				new PointF( afX[2], bounds.Top ),
				new PointF( afX[3], bounds.Top + 2 ),
				new PointF( afX[3], bounds.Bottom - 3 ),
				new PointF( afX[2], bounds.Bottom - 1 ),
				new PointF( afX[1], bounds.Bottom - 1 )
			};

            return polygon;
        }

        /// <summary>
        /// Returns the polygonal shade border of the tab from the bounds.
        /// </summary>
        /// <param name="bounds">The tab's bounds.</param>
        /// <returns>The polygonal border.</returns>
        public PointF[] GetShadePolygonFromBounds(RectangleF bounds)
        {
            bool bIsMirrored = panelRenderer.IsMirrored;

            // Array of X coordinates for 3D tab shade drawing
            float[] afX = bIsMirrored ?
                // RTL X-coordinates
                new float[2] { bounds.Left + 1, bounds.Right - 2 } :
                // Default X-coordinates (for LTR)
                new float[2] { bounds.Right - 2, bounds.Left + 1 };

            PointF[] polygon =
			{
				new PointF( afX[0], bounds.Top + 1 ),
				new PointF( afX[0], bounds.Bottom - 2 ),
				new PointF( afX[1], bounds.Bottom - 2 )
			};

            return polygon;
        }

        private void GetRotateTypes(ref RotateFlipType rotateForward, ref RotateFlipType rotateBack)
        {
            rotateForward = RotateFlipType.RotateNoneFlipNone;

            switch (this.TabAlignment)
            {
                case TabAlignment.Right:
                    rotateForward = RotateFlipType.Rotate270FlipNone;
                    rotateBack = RotateFlipType.Rotate90FlipNone;
                    break;

                case TabAlignment.Bottom:
                    rotateBack = rotateForward = RotateFlipType.Rotate180FlipNone;
                    break;

                case TabAlignment.Left:
                    rotateForward = RotateFlipType.Rotate90FlipNone;
                    rotateBack = RotateFlipType.Rotate270FlipNone;
                    break;
            }
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBackground"/>.
        /// </summary>
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = drawItemInfo.Bounds;

            Region oldClipRegion = g.Clip;
            curBounds = AdjustBoundsAndGraphicsForAlignment(g, curBounds, this.TabAlignment);

            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
            {
                bool tabSelected = this.panelRenderer.TabPanelData.SelectedIndex == -1 ?
                    false :
                    this.panelRenderer.TabPanelData.TabsData[this.panelRenderer.TabPanelData.SelectedIndex] == this.TabData;


                Rectangle itemBounds = Rectangle.Ceiling(curBounds);

                if (this.TabAlignment != TabAlignment.Top)
                {
                    Point itemLocation = itemBounds.Location;

                    using (Bitmap backgoundBmp = new Bitmap(itemBounds.Width, itemBounds.Height))
                    {
                        RotateFlipType rotateForward = RotateFlipType.RotateNoneFlipNone;
                        RotateFlipType rotateBack = RotateFlipType.RotateNoneFlipNone;

                        this.GetRotateTypes(ref rotateForward, ref rotateBack);

                        backgoundBmp.RotateFlip(rotateForward);

                        using (Graphics rotatedGraphics = Graphics.FromImage(backgoundBmp))
                        {
                            itemBounds = Rectangle.Empty;
                            itemBounds.Size = backgoundBmp.Size;

                            this.TabControl.ThemedDrawing.DrawTabBackground(rotatedGraphics, this,
                                tabSelected, itemBounds);
                        }

                        backgoundBmp.RotateFlip(rotateBack);

                        g.DrawImage(backgoundBmp, itemLocation);
                    }
                }
                else
                {
                    this.TabControl.ThemedDrawing.DrawTabBackground(g, this,
                        tabSelected, itemBounds);
                }
            }
            else
            {
                // We cannot avoid drawing this to avoid color-distortion in
                // low color mode, because we need to draw this to clear the other tab's
                // border below this.
                PointF[] polygon = GetPolygonFromBounds(curBounds);
                g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), polygon);
            }
            g.SetClip(oldClipRegion, CombineMode.Replace);
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBorders"/>.
        /// </summary>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                return;

            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = drawItemInfo.Bounds;
            Region oldClipRegion = g.Clip;
            curBounds = AdjustBoundsAndGraphicsForAlignment(g, curBounds, this.TabAlignment);

            PointF[] polygon = GetPolygonFromBounds(curBounds);

            bool bIsMirrored = panelRenderer.IsMirrored;

            // Draw left line, Top-Left Hatch, top line
            Color colorLight = SystemColors.ControlLightLight;
            // Points are connected in order to draw polyline with light pen
            PointF[] aptLightBorder = new PointF[]
			{
				polygon[0], polygon[1], polygon[2], polygon[3]
			};
            g.DrawLines(new Pen(colorLight), aptLightBorder);

            // Draw right line, Right-Bottom hatch, bottom line
            Color colorDark = SystemColors.ControlDarkDark;
            // Points are connected in order to draw polyline with dark pen
            PointF[] aptDarkBorder = new PointF[]
			{
				polygon[4], polygon[5], polygon[6], polygon[7]
			};
            g.DrawLines(new Pen(colorDark), aptDarkBorder);

            // Draw right shade, bottom shade
            Color colorShade = Color.DarkGray;
            PointF[] polygonShade = GetShadePolygonFromBounds(curBounds);
            // Points are connected in order to draw shade
            g.DrawLines(new Pen(colorShade), polygonShade);

            g.SetClip(oldClipRegion, CombineMode.Replace);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawInterior"/>.
        /// </summary>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            ApplyTransform(g);

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            TabPanelData panelData = panelRenderer.TabPanelData as TabPanelData;

            if (ShowCloseButton)
            {
                this.DrawCloseButton(g, closeButtonBounds);
            }

            if (((int)drawItemInfo.State & (int)DrawItemState.Focus) > 0)
                DrawFocusRect(g, this.Bounds, drawItemInfo.ForeColor, drawItemInfo.BackColor);

            g.ResetTransform();
        }

        public override int CloseButtonPadding
        {
            get
            {
                int ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = c_iCloseButtonPaddingThemes;
                }
                else
                {
                    ret = c_iCloseButtonPadding;
                }

                return ret;
            }
        }

        protected override Point CorrectCloseButtonPosition
        {
            get
            {
                Point ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = new Point((panelRenderer.IsMirrored) ? -c_iCorrectCloseButtonX : c_iCorrectCloseButtonX, 0);
                }
                else
                {
                    ret = new Point(0, 0);
                }

                return ret;
            }
        }
    }

    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererWorkbookMode"/>
    /// tab style.
    /// </summary>
    public class TabPanelPropertyWorkbookMode : TabUIDefaultProperties
    {
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DrawLeftToRight"/>
        /// </summary>
        public override bool DrawLeftToRight
        {
            get { return false; }
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.GetOverlapSize"/>.
        /// </summary>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(TabRendererWorkbookMode.OVERLAPX, TabRenderer3D.OVERLAPY);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DefaultActiveTabColor"/>.
        /// </summary>
        /// <param name="panelData"></param>
        /// <param name="tabControl"></param>
        /// <returns></returns>
        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.White;
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DefaultActiveTabFont"/>.
        /// </summary>
        /// <param name="panelData"></param>
        /// <param name="tabControl"></param>
        /// <returns></returns>
        public override Font DefaultActiveTabFont(ITabPanelData panelData, ITabControl tabControl)
        {
            return Syncfusion.Drawing.FontUtil.CreateFont(this.DefaultInactiveTabFont(panelData, tabControl), FontStyle.Bold);
        }
    }

    /// <summary>
    /// The tab renderer that implements workbook mode tabs look-and-feel.
    /// </summary>
    public class TabRendererWorkbookMode : TabRendererBase
    {
        private const int c_iCorrectCloseButtonX = -3;
        private const int DEF_LABELEDIT_OFFSET_Y_BOTTOM = 3;

        /// <summary>
        /// The default horizontal overlap.
        /// </summary>
        public static readonly int OVERLAPX = 10;
        /// <summary>
        /// The default vertical overlap.
        /// </summary>
        public static readonly int OVERLAPY = 0;


        /// <summary>
        /// Returns the unique name of this tab renderer.
        /// </summary>
        public static string TabStyleName
        {
            get
            {
                return "Workbook";
            }
        }

        static TabPanelPropertyWorkbookMode tabPanelPropertyExtender;
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static TabPanelPropertyWorkbookMode TabPanelPropertyExtender
        {
            get { return tabPanelPropertyExtender; }
        }
        static TabRendererWorkbookMode()
        {
            tabPanelPropertyExtender = new TabPanelPropertyWorkbookMode();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererWorkbookMode), TabPanelPropertyExtender);
        }

        public static void RegisterTabType()
        {
            tabPanelPropertyExtender = new TabPanelPropertyWorkbookMode();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererWorkbookMode), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Creates a new instance of the TabRendererWorkbookMode class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererWorkbookMode(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.GetOverlapSize"/>.
        /// </summary>
        /// <returns></returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return tabPanelPropertyExtender.GetOverlapSize(tabSize);
        }

        public override int LabelEditOffsetY
        {
            get
            {
                int offsetY = 0;

                if (this.TabAlignment == TabAlignment.Bottom)
                {
                    offsetY = DEF_LABELEDIT_OFFSET_Y_BOTTOM;
                }

                return offsetY;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.HitTest"/>.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public override bool HitTest(PointF mousePosition)
        {
            // Get the corner points and create a region out of the polygon
            RectangleF rect = this.GetCurrentBounds();

            // Hittest the region to verify if the point is in the rect
            return rect.Contains(mousePosition);
        }

        /// <summary>
        /// Returns the polygonal border of the tab from the bounds.
        /// </summary>
        /// <param name="bounds">The tab's bounds.</param>
        /// <returns>The polygonal border.</returns>
        public PointF[] GetPolygonFromBounds(RectangleF bounds)
        {
            bool bIsMirrored = panelRenderer.IsMirrored;

            // Array of X coordinates for 3D tab borders drawing
            float[] afX = bIsMirrored ?
                // RTL X-coordinates
                new float[4] { bounds.Right - 1, bounds.Right - OVERLAPX / 2 - 3, bounds.Left + OVERLAPX / 2 + 2, bounds.Left } :
                // Default X-coordinates (for LTR)
                new float[4] { bounds.Left, bounds.Left + OVERLAPX / 2 + 2, bounds.Right - OVERLAPX / 2 - 3, bounds.Right - 1 };

            PointF[] m_polygon;

            if (base.TabControl.ThemesEnabled)
            {
                if (this.TabAlignment == TabAlignment.Top ||
                    this.TabAlignment == TabAlignment.Left ||
                    this.TabAlignment == TabAlignment.Bottom)
                {
                    PointF[] polygon =
					{			
						new PointF( afX[0], bounds.Bottom ),
						new PointF( afX[1], bounds.Top ),
						new	PointF( afX[2], bounds.Top ),
						new PointF( afX[3], bounds.Bottom )
					};
                    m_polygon = polygon;
                }
                else
                {
                    PointF[] polygon =
					{			
						new PointF( afX[0], bounds.Bottom - 2 ),
						new PointF( afX[1], bounds.Top ),
						new	PointF( afX[2], bounds.Top ),
						new PointF( afX[3], bounds.Bottom - 2 )
					};
                    m_polygon = polygon;
                }
            }
            else
            {
                PointF[] polygon =
				{			
					new PointF( afX[0], bounds.Bottom - 1 ),
					new PointF( afX[1], bounds.Top ),
					new	PointF( afX[2], bounds.Top ),
					new PointF( afX[3], bounds.Bottom - 1 )
				};
                m_polygon = polygon;
            }

            return m_polygon;
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.GetCurrentBounds"/>.
        /// </summary>
        /// <returns></returns>
        public override RectangleF GetCurrentBounds()
        {
            // Will always draw over the adjacent tab
            SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
            RectangleF overlappedRect = this.Bounds;
            overlappedRect.Inflate(overlappedSize.Width / 2, 0);
            overlappedRect.Offset(0, -overlappedSize.Height);
            overlappedRect.Height += overlappedSize.Height;

            return overlappedRect;
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBackground"/>.
        /// </summary>
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(g);

            // Determine bg color
            if (((int)drawItemInfo.State & (int)DrawItemState.Selected) > 0)
            {
                if (drawItemInfo.BackColor == Color.Empty)
                    drawItemInfo.BackColor = SystemColors.Window;
            }

            if (drawItemInfo.BackColor == Color.Empty)
                drawItemInfo.BackColor = this.panelRenderer.TabPanelData.BackColor;


            PointF[] polygon = GetPolygonFromBounds(curBounds);

            // We cannot avoid drawing this to avoid color-distortion in
            // low color mode, because we need to draw this to clear the other tab's
            // border below this.
            g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), polygon);

            g.ResetTransform();
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBorders"/>.
        /// </summary>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

            bool bSelectedTab = (panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] == this.TabData);

            Region oldClipRegion = g.Clip;
            if (!bSelectedTab && TabAlignment.Top == this.TabAlignment)
            {
                // BUGFIX:
                // Clip rectangle's height is reduced for one pixel to avoid redundant pixel
                // drawing for inactive tabs in Workbook renderer
                RectangleF rectClip = curBounds;
                --rectClip.Height;

                g.SetClip(rectClip, CombineMode.Intersect);
            }

            // Make g horizontal
            ApplyTransform(g);

            SmoothingMode oldSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            PointF[] polygon = GetPolygonFromBounds(curBounds);

            g.DrawLines(new Pen(SystemColors.ControlDarkDark), polygon);

            // If not selected give it some shadows
            if (!bSelectedTab)
            {
                bool bIsMirrored = panelRenderer.IsMirrored;
                // When RTL is ON and alignment is Right/Left colors of opposite sides are swapped due to
                // non-normalized bounds returned from TabUtils.ApplyTransform()
                bool bSwapVertical = (bIsMirrored &&
                    (TabAlignment.Left == this.TabAlignment || TabAlignment.Right == this.TabAlignment));

                // Swap shade/highlight colors for Left/Right tab alignment when RTL is ON
                Color color1 = bSwapVertical ? SystemColors.ControlDark : SystemColors.ControlLightLight;
                Color color2 = bSwapVertical ? SystemColors.ControlLightLight : SystemColors.ControlDark;

                // Array of points to draw highligh line (shifted in opposite direction for RTL)
                PointF[] aptLiteLine = bIsMirrored ?
                    // Shifted to the left for LTR
                    new PointF[] { polygon[0] + new Size(-1, 0), polygon[1] + new Size(-1, 1) } :
                    // Shifted to the right for RTL
                    new PointF[] { polygon[0] + new Size(1, 0), polygon[1] + new Size(1, 1) };
                g.DrawLine(new Pen(color1), aptLiteLine[0], aptLiteLine[1]);

                // Array of points to draw shade line
                PointF[] aptDarkLine = bIsMirrored ?
                    // Shifted to the right for RTL
                    new PointF[] { polygon[2] + new Size(1, 1), polygon[3] + new Size(1, 0) } :
                    // Shifted to the left for LTR
                    new PointF[] { polygon[2] + new Size(-1, 1), polygon[3] + new Size(-1, 0) };
                g.DrawLine(new Pen(color2), aptDarkLine[0], aptDarkLine[1]);
            }

            g.SmoothingMode = oldSmoothingMode;

            g.ResetTransform();

            if (!bSelectedTab)
            {
                g.Clip = oldClipRegion;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawInterior"/>.
        /// </summary>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            // Make g horizontal
            ApplyTransform(g);

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            if (ShowCloseButton)
            {
                DrawCloseButton(g, closeButtonBounds);
            }

            if (((int)drawItemInfo.State & (int)DrawItemState.Focus) > 0)
                DrawFocusRect(g, RectangleF.Inflate(this.Bounds, -1, -1), drawItemInfo.ForeColor, drawItemInfo.BackColor);

            g.ResetTransform();
        }

        protected override Point CorrectCloseButtonPosition
        {
            get
            {
                bool selectedTab = false;
                int correctX;
                if (this.panelRenderer.TabPanelData.SelectedIndex >= 0)
                {
                    selectedTab = (this.panelRenderer.TabPanelData.TabsData[this.panelRenderer.TabPanelData.SelectedIndex] == this.TabData);
                }

                if (selectedTab)
                {
                    SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
                    correctX = Convert.ToInt32(overlapSize.Width / 2);

                    if (panelRenderer.IsMirrored)
                    {
                        correctX = -correctX;
                    }
                }
                else
                {
                    correctX = 0;
                }

                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    correctX += (panelRenderer.IsMirrored) ? -c_iCorrectCloseButtonX : c_iCorrectCloseButtonX;
                }

                return new Point(correctX, 0);
            }
        }

        public override int CloseButtonPadding
        {
            get
            {
                return base.CloseButtonPadding;
            }
        }

        public override Color TabBorderColor
        {
            get
            {
                return SystemColors.ControlDarkDark;
            }
        }
    }

    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRenderer2D"/>
    /// tab style.
    /// </summary>
    public class TabPanelProperty2D : TabUIDefaultProperties
    {
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DefaultTabPanelBackgroundColor"/>.
        /// </summary>
        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return DefaultInactiveTabColor(panelData, tabControl);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.GetOverlapSize"/>.
        /// </summary>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(TabRenderer2D.OVERLAPX, 0);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DefaultInactiveTabColor"/>.
        /// </summary>
        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            Color activeTabColor = panelData.ActiveTabColor;
            if (activeTabColor == Color.Empty)
                activeTabColor = this.DefaultActiveTabColor(panelData, tabControl);
            return ControlPaint.Light(activeTabColor);
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties.DefaultActiveTabFont"/>.
        /// </summary>
        public override Font DefaultActiveTabFont(ITabPanelData panelData, ITabControl tabControl)
        {
            return Syncfusion.Drawing.FontUtil.CreateFont(this.DefaultInactiveTabFont(panelData, tabControl), FontStyle.Bold);
        }
    }

    /// <summary>
    /// The tab renderer that implements the 2D tabs look-and-feel.
    /// </summary>
    public class TabRenderer2D : TabRendererBase
    {
        private const int DEF_LABELEDIT_OFFSET_Y = 2;
        private const int c_iCloseButtonPaddingThemes = 8;
        private const int c_iCloseButtonPadding = 5;
        private const int c_iCorrectCloseButtonX = -3;

        static TabPanelProperty2D tabPanelPropertyExtender;

        /// <summary>
        /// Used internally to represent the borders.
        /// </summary>
        protected enum BorderSide { left, top, right, bottom }
        /// <summary>
        /// Specifies the default border colors for the 4 borders.
        /// </summary>
        protected static Color[] buttonBorderColors = { SystemColors.ControlLightLight, SystemColors.ControlLightLight, SystemColors.ControlDarkDark, SystemColors.ControlDarkDark };
        /// <summary>
        /// Returns the border color given the border and the alignment.
        /// </summary>
        /// <param name="border">The <see cref="Syncfusion.Windows.Forms.Tools.TabRenderer2D.BorderSide"/>.</param>
        /// <param name="curAlign">The <see cref="System.Windows.Forms.TabAlignment"/>.</param>
        /// <returns>The Color for the border.</returns>
        protected static Color BorderColors(BorderSide border, TabAlignment curAlign)
        {
            int borderPos = (int)border;
            if (curAlign == TabAlignment.Right)
                borderPos++;
            else if (curAlign == TabAlignment.Left)
            {
                borderPos += 3;
                if (border == BorderSide.left || border == BorderSide.right)
                    borderPos += 2;
            }
            else if (curAlign == TabAlignment.Bottom)
            {
                if (border == BorderSide.top || border == BorderSide.bottom)
                    borderPos += 2;
            }

            while (borderPos > 3)
                borderPos = borderPos - 4;

            return buttonBorderColors[borderPos];
        }

        /// <summary>
        /// Returns the unique name of this tab renderer.
        /// </summary>
        public static string TabStyleName
        {
            get
            {
                return "2D";
            }
        }

        /// <summary>
        /// The default horizontal overlap.
        /// </summary>
        public static readonly int OVERLAPX = 2;

        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static TabPanelProperty2D TabPanelPropertyExtender
        {
            get { return tabPanelPropertyExtender; }
        }
        static TabRenderer2D()
        {
            tabPanelPropertyExtender = new TabPanelProperty2D();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRenderer2D), TabPanelPropertyExtender);
        }

        public static void RegisterTabType()
        {
            tabPanelPropertyExtender = new TabPanelProperty2D();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRenderer2D), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Creates a new instance of the TabRenderer2D class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRenderer2D(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.GetOverlapSize"/>.
        /// </summary>
        /// <returns></returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return tabPanelPropertyExtender.GetOverlapSize(tabSize);
        }

        public override int LabelEditOffsetY
        {
            get
            {
                return DEF_LABELEDIT_OFFSET_Y;
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBackground"/>.
        /// </summary>
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            // Take a look at TabControlAdv.Init for notes on why we need this check.
            if (this.panelRenderer.TabPanelBackColor != drawItemInfo.BackColor
                || !this.panelRenderer.IsBackgroundSolid())
            {
                Graphics g = drawItemInfo.Graphics;

                g.FillRectangle(new SolidBrush(drawItemInfo.BackColor), drawItemInfo.Bounds);
            }
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBorders"/>.
        /// </summary>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            // Transformed to horizontal alignment
            RectangleF currentBounds = TabUtils.ApplyTransform(g, this.TabAlignment,
                drawItemInfo.Bounds, true);

            // This could instead be imp. as a no-transform version with ControlPaint
            this.ApplyTransform(g);

            float fLineX = 0.0F;
            bool bIsMirrored = panelRenderer.IsMirrored;
            // When RTL is ON and alignment is Right/Left colors of opposite sides are swapped due to
            // non-normalized bounds returned from TabUtils.ApplyTransform()
            // Color swapping ensures correct highlighting/shading effect for corresponding sides
            bool bSwapVertical = (bIsMirrored &&
                (this.TabAlignment == TabAlignment.Right || this.TabAlignment == TabAlignment.Left));

            if (((int)drawItemInfo.State & (int)DrawItemState.Selected) > 0)
            {
                // Draw left line
                fLineX = bIsMirrored ? currentBounds.Right - 1 : currentBounds.Left;
                Color colorLeft = BorderColors(bSwapVertical ? BorderSide.right : BorderSide.left, this.TabAlignment);
                g.DrawLine(new Pen(colorLeft), new PointF(fLineX, currentBounds.Bottom - 1),
                    new PointF(fLineX, currentBounds.Top));

                // top line
                Color colorTop = BorderColors(bSwapVertical ? BorderSide.bottom : BorderSide.top, this.TabAlignment);
                g.DrawLine(new Pen(colorTop), new PointF(currentBounds.Left, currentBounds.Top),
                    new PointF(currentBounds.Right - 1, currentBounds.Top));

                // right line
                fLineX = bIsMirrored ? currentBounds.Left : currentBounds.Right - 1;
                Color colorRight = BorderColors(bSwapVertical ? BorderSide.left : BorderSide.right, this.TabAlignment);
                g.DrawLine(new Pen(colorRight), new PointF(fLineX, currentBounds.Top),
                    new PointF(fLineX, currentBounds.Bottom - 1));
            }
            else
            {
                fLineX = (bIsMirrored && !bSwapVertical) ? currentBounds.Left : currentBounds.Right - 1;
                g.DrawLine(new Pen(Color.FromArgb(128, drawItemInfo.ForeColor)), fLineX, currentBounds.Top + 2,
                    fLineX, currentBounds.Bottom - 3);
            }

            g.ResetTransform();
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawInterior"/>.
        /// </summary>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;
            Brush br=new SolidBrush(drawItemInfo.ForeColor);
            // Convert to horizontal co-ords
            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            // Transform g to horizontal co-ords
            this.ApplyTransform(g);

            // Determine text brush
            if (drawItemInfo.TextBrush == null)
            {
                if (((int)drawItemInfo.State & (int)DrawItemState.Selected) <= 0)
                {
                    br = new SolidBrush(Color.FromArgb(192, drawItemInfo.ForeColor));
                    drawItemInfo.TextBrush = br;
                }
                else
                {
                    br = new SolidBrush(drawItemInfo.ForeColor);
                    drawItemInfo.TextBrush = br;
                }
            }

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            if (ShowCloseButton)
            {
                DrawCloseButton(g, closeButtonBounds, drawItemInfo);
            }

            if (((int)drawItemInfo.State & (int)DrawItemState.Focus) > 0)
                DrawFocusRect(g, RectangleF.Inflate(this.Bounds, -1, -1), drawItemInfo.ForeColor, drawItemInfo.BackColor);

            g.ResetTransform();
            br.Dispose();
        }

        public override int CloseButtonPadding
        {
            get
            {
                int ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = c_iCloseButtonPaddingThemes;
                }
                else
                {
                    ret = c_iCloseButtonPadding;
                }

                return ret;
            }
        }

        protected override Point CorrectCloseButtonPosition
        {
            get
            {
                Point ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = new Point((panelRenderer.IsMirrored) ? -c_iCorrectCloseButtonX : c_iCorrectCloseButtonX, 0);
                }
                else
                {
                    ret = new Point(0, 0);
                }

                return ret;
            }
        }
    }

    /// <summary>
    /// This tab renderer implements the Office 2003 OneNoteStyle look-and-feel.
    /// </summary>
    public class OneNoteStyleRenderer : TabRenderer3D
    {
        #region TABINIT
        /// <summary>
        /// The default horizontal overlap.
        /// </summary>
        internal static int DefaultOverlapSizeX = 30;
        // Use the above DemoTabPanelProperty as my default properties provider.
        static OneNoteStyleRendererProperty tabPropertyExtender;
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static new OneNoteStyleRendererProperty TabPanelPropertyExtender
        {
            get { return tabPropertyExtender; }
        }

        /// <summary>
        /// Returns the unique name of this tab renderer.
        /// </summary>
        public static new string TabStyleName
        {
            get
            {
                return "OneNoteStyle";
            }
        }

        static OneNoteStyleRenderer()
        {
            tabPropertyExtender = new OneNoteStyleRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(OneNoteStyleRenderer), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            tabPropertyExtender = new OneNoteStyleRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(OneNoteStyleRenderer), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Creates a new instance of the TabRenderer3D class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public OneNoteStyleRenderer(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {

        }

        /// </override>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return tabPropertyExtender.GetOverlapSize(tabSize);
        }

        #endregion TABINIT

        private const int DEF_LABELEDIT_OFFSET_Y_TOP = 2;
        private const int DEF_LABELEDIT_OFFSET_Y_BOTTOM = 3;

        public override int LabelEditOffsetY
        {
            get
            {
                int offsetY = 0;

                switch (this.TabAlignment)
                {
                    case TabAlignment.Top:
                        offsetY = DEF_LABELEDIT_OFFSET_Y_TOP;
                        break;
                    case TabAlignment.Bottom:
                        offsetY = DEF_LABELEDIT_OFFSET_Y_BOTTOM;
                        break;
                }

                return offsetY;
            }
        }

        protected new bool NeedRotateTextWhenVertical
        {
            get
            {
                return ((this.TabAlignment == TabAlignment.Left ||
                    this.TabAlignment == TabAlignment.Right) && (this.panelRenderer != null &&
                    this.panelRenderer.TabPanelData != null &&
                    this.panelRenderer.TabPanelData.RotateTextWhenVertical));
            }
        }

        public override RectangleF GetBoundsForScrolling()
        {
            // Will always draw over the adjacent tab
            SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);

            RectangleF overlappedRect = this.Bounds;
            // Do not use the height which will just be some empty space on top.

            bool bNeedRotate = this.NeedRotateTextWhenVertical;

            if (bNeedRotate)
            {
                overlappedRect.Y -= this.panelRenderer.TabPanelData.Padding.X;
                overlappedRect.Height += this.panelRenderer.TabPanelData.Padding.X;
                overlappedRect.X -= overlappedSize.Width / 2;
            }
            else
            {
                overlappedRect.Width += overlappedSize.Width / 2;
            }
            //overlappedRect.Height += overlappedSize.Height;

            return overlappedRect;
        }

        public override RectangleF GetCurrentBounds()
        {
            // Will always draw over the adjacent tab
            SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);

            RectangleF overlappedRect = this.Bounds;
            // Do not use the height which will just be some empty space on top.

            bool bNeedRotate = this.NeedRotateTextWhenVertical;

            bool bIsMirrored = NeedMirroredBackground();

            if (bNeedRotate)
            {
                overlappedRect.X += overlappedSize.Height / 2;
                overlappedRect.Y -= overlappedSize.Width / 2;
                overlappedRect.Height += overlappedSize.Width / 2;
            }

            if (bIsMirrored)
            {
                if (!bNeedRotate)
                {
                    overlappedRect.Width += overlappedSize.Width / 2;
                }
            }
            else
            {
                if (!bNeedRotate)
                {
                    bool correctX = true;

                    if (panelRenderer.IsMirrored)
                    {
                        TabControlAdv tabControl = this.TabControl as TabControlAdv;

                        if (tabControl != null && !tabControl.RotateTabsWhenRTL)
                        {
                            correctX = false;
                        }
                    }

                    if (correctX)
                    {
                        overlappedRect.X -= overlappedSize.Width / 2;
                    }

                    overlappedRect.Width += overlappedSize.Width / 2;
                }
            }
            //overlappedRect.Height += overlappedSize.Height;

            return overlappedRect;
        }

        protected override SizeF CorrectPreferredSize(SizeF preferredSize)
        {
            ITabPanelData panelData = null;

            if (this.panelRenderer != null)
            {
                panelData = this.panelRenderer.TabPanelData;

                if (panelData != null)
                {
                    switch (this.TabAlignment)
                    {
                        case TabAlignment.Top:
                        case TabAlignment.Bottom:
                            preferredSize.Width += (panelData.Padding.X * 2);
                            preferredSize.Height += (panelData.Padding.Y * 2);
                            break;

                        case TabAlignment.Right:
                        case TabAlignment.Left:
                            if (this.NeedRotateTextWhenVertical)
                            {
                                preferredSize.Height += (panelData.Padding.X * 2);
                                preferredSize.Width += (panelData.Padding.Y * 2);
                            }
                            else
                            {
                                preferredSize.Width += (panelData.Padding.X * 2);
                                preferredSize.Height += (panelData.Padding.Y * 2);
                            }
                            break;
                    }
                }
            }

            return preferredSize;
        }

        /// <summary>
        /// Gets the interior bounds of the tab.
        /// </summary>
        /// <param name="currentBounds">The bounds of the current tab</param>
        /// <param name="selectedTab">The Tab which is selected</param>
        /// <returns></returns>
        protected override RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
        {
            RectangleF rectTextAndImage = currentBounds;

            int nPadX = this.panelRenderer.TabPanelData.Padding.X;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (this.NeedRotateTextWhenVertical && this.TabControl != null)
            {
                if (bIsMirrored)
                {
                    rectTextAndImage.Y -= nPadX;
                }
                rectTextAndImage.X += 1;
            }

            else if (bIsMirrored)
            {
                rectTextAndImage.X -= nPadX;
                rectTextAndImage.Width += nPadX - 1;
            }
            else
            {
                rectTextAndImage.X += nPadX;
                rectTextAndImage.Width += nPadX;
            }

            rectTextAndImage = CorrectInteriorBounds(rectTextAndImage);

            rectTextAndImage = RectangleF.Inflate(rectTextAndImage, 0,
                -this.panelRenderer.TabPanelData.Padding.Y);

            return rectTextAndImage;
        }

        protected override RectangleF GetCloseButtonBounds(DrawTabEventArgs drawItemInfo)
        {
            TabPanelData panelData = panelRenderer.TabPanelData as TabPanelData;

            RectangleF closeButtonBounds = Rectangle.Empty;

            if (this.ShowCloseButton)
            {
                Graphics g = drawItemInfo.Graphics;
                SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);

                RectangleF rectTextAndImage = drawItemInfo.BoundsInterior;
                if (!this.ShouldDrawRotatedWhenVertical)
                {
                    rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
                    rectTextAndImage.Inflate(-(float)Math.Ceiling(overlapSize.Width / 2), 0);
                }

                closeButtonBounds.Size = new SizeF(CloseButtonSize, CloseButtonSize);

                Rectangle itemBounds = Rectangle.Round(TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true));
                if (this.ShouldDrawRotatedWhenVertical)
                {
                    itemBounds = drawItemInfo.Bounds;
                }

                Rectangle buttonRect = Rectangle.Empty;

                if (this.ShouldDrawRotatedWhenVertical)
                {
                    if (this.TabAlignment == TabAlignment.Right)
                    {
                        if (panelRenderer.IsMirrored)
                        {
                            closeButtonBounds.Location = new PointF(rectTextAndImage.Right - 1, rectTextAndImage.Top);
                        }
                        else
                        {
                            closeButtonBounds.Location = new PointF(rectTextAndImage.Left, rectTextAndImage.Top);
                        }
                    }
                    else if (this.TabAlignment == TabAlignment.Left)
                    {
                        if (panelRenderer.IsMirrored)
                        {
                            closeButtonBounds.Location = new PointF(rectTextAndImage.Left, rectTextAndImage.Top);
                        }
                        else
                        {
                            closeButtonBounds.Location = new PointF(rectTextAndImage.Right - 1, rectTextAndImage.Top);
                        }
                    }

                    buttonRect = Rectangle.Ceiling(rectTextAndImage);
                }
                else
                {
                    if (this.panelRenderer.IsMirrored)
                    {
                        closeButtonBounds.Location = rectTextAndImage.Location;
                        closeButtonBounds.Offset(-(CloseButtonSize + CloseButtonPadding), 0);
                    }
                    else
                    {
                        closeButtonBounds.Location = new PointF(rectTextAndImage.Right + CloseButtonPadding, rectTextAndImage.Top);
                    }

                    buttonRect = Rectangle.Ceiling(rectTextAndImage);
                }

                closeButtonBounds.Height = Math.Min(closeButtonBounds.Height, buttonRect.Height);
                closeButtonBounds.Y = (buttonRect.Height - closeButtonBounds.Height) / 2 + buttonRect.Y;

                closeButtonBounds.Intersect(itemBounds);
            }

            return closeButtonBounds;
        }

        public override bool HitTest(PointF mousePosition)
        {
            RectangleF bounds = this.GetCurrentBounds();

            if (this.NeedRotateTextWhenVertical)
            {
                if (this.TabControl != null)
                {
                    Control control = this.TabControl.GetControl();
                    if (control != null)
                    {
                        using (Graphics g = control.CreateGraphics())
                        {
                            RectangleF rectMousePos = new RectangleF(mousePosition,
                                SizeF.Empty);

                            rectMousePos = TabUtils.ApplyTransform(g, this.TabAlignment,
                                rectMousePos, false);

                            mousePosition = rectMousePos.Location;
                            GraphicsState savedState = g.Save();
                            bounds = TabUtils.ApplyTransform(g, this.TabAlignment, bounds, false);
                            g.Restore(savedState);
                            g.ResetTransform();
                        }
                    }
                }
            }

            GraphicsPath path = this.GetBorderPathFromBounds(bounds);
            int selectedIndex = this.panelRenderer.TabPanelData.SelectedIndex;
            ITabData processedTabData = this.panelRenderer.TabPanelData.TabsData[selectedIndex] as ITabData;
            if (processedTabData != this.TabData)
            {
                path.CloseFigure();
            }
            Region region = new Region(path);

            // Hittest the region to verify if the point is in the rect
            if (region.IsVisible(mousePosition))
                return true;
            else
                return false;
        }

        #region DRAWING

        private const int c_iCorrectCloseButtonX = -3;
        private const int c_iCloseButtonPadding = 5;

        protected virtual GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            //int slopeSpan = 24;
            float height = bounds.Height;

            int slopeSpan = (int)(height * 1.1f);
            int topSlopeSpanY = (int)(height / 4f);
            int topSlopeSpanX = (int)(height / 10f);

            slopeSpan -= (4 + topSlopeSpanY);

            //bounds.Offset(-OverlapX/2, 0);

            bool bIsMirrored = this.NeedMirroredBackground();

            PointF[] aptCurve = bIsMirrored ?
                new PointF[]
				{
					new PointF(bounds.Right, bounds.Bottom-1),
					new PointF(bounds.Right - (4), bounds.Bottom - 3),
					new PointF(bounds.Right - (4 + slopeSpan), bounds.Top + topSlopeSpanX),
					new PointF(bounds.Right - (4 + slopeSpan + topSlopeSpanY), bounds.Top)
				} :
                new PointF[]
				{
					new PointF(bounds.Left, bounds.Bottom-1),
					new PointF(bounds.Left + (4), bounds.Bottom - 3),
					new PointF(bounds.Left + (4 + slopeSpan), bounds.Top + topSlopeSpanX),
					new PointF(bounds.Left + (4 + slopeSpan + topSlopeSpanY), bounds.Top)
				};

            path.AddCurve(aptCurve, 0.25f);

            PointF[] aptLines = bIsMirrored ?
                new PointF[]
				{
					new PointF(bounds.Left + 4, bounds.Top),
					new PointF(bounds.Left + 1, bounds.Top + 4),
					new PointF(bounds.Left + 1, bounds.Bottom)
				} :
                new PointF[]
				{
					new PointF(bounds.Right - 4, bounds.Top),
					new PointF(bounds.Right - 1, bounds.Top + 4),
					new PointF(bounds.Right - 1, bounds.Bottom)
				};

            path.AddLines(aptLines);

            return path;
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            if (drawItemInfo.Bounds.Width == 0 || drawItemInfo.Bounds.Height == 0)
                return;

            Graphics gph = drawItemInfo.Graphics;

            RectangleF curBounds = TabUtils.ApplyTransform(gph, this.TabAlignment, drawItemInfo.Bounds, true);

            // Make g horizontal
            ApplyTransform(gph);

            SaveGraphicsState(gph, ref curBounds);

            Color bgColor = drawItemInfo.BackColor;
            Color backColorTop = ControlPaint.LightLight(bgColor);

            // Get the border path and fill it.
            GraphicsPath path = null;
            if ((drawItemInfo.State & DrawItemState.Selected) > 0)
            {
                path = this.GetBorderPathFromBounds(curBounds);
            }
            else
            {
                path = this.GetBorderPathFromBounds(curBounds);
                path.CloseFigure();
            }

            if ((drawItemInfo.State & DrawItemState.Selected) > 0)
            {
                if (panelRenderer.TabPanelData.ActiveTabColor == Color.Empty)
                {
                    backColorTop = Office2003Colors.SelColor;

                    backColorTop = ChevronPainter.BlendColor(MenuColors.SelColor, backColorTop);
                    bgColor = Office2003Colors.MenuItemHotColorDark;
                }
            }

            //			float r, g, b = 0;
            //			float curB = bgColor.GetBrightness();
            //			Syncfusion.Drawing.DrawingUtils.ConvertHSBToRGB(
            //				bgColor.GetHue(), bgColor.GetSaturation(), 0.99f, out r, out g, out b);
            //			Color lightColor = Color.FromArgb(bgColor.A, (int)Math.Floor(r * 255f),
            //				(int)Math.Floor(g * 255f),
            //				(int)Math.Floor(b * 255f));


            Brush brs = new LinearGradientBrush(curBounds, backColorTop, bgColor, LinearGradientMode.Vertical);
            gph.FillPath(brs, path);

            RestoreGraphicsState(gph);

            gph.ResetTransform();
        }

        protected virtual bool ShouldDrawHighLightUpper
        {
            get
            {
                return true;
            }
        }

        protected virtual Color BorderColor
        {
            get
            {
                return Office2003Colors.SelBorderColor;
            }
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

            bool bIsMirrored = this.NeedMirroredBackground();

            RectangleF whiteBounds = curBounds;
            whiteBounds.Inflate(-1, -1);

            if (bIsMirrored)
            {
                if ((drawItemInfo.State & DrawItemState.Selected) <= 0)
                {
                    whiteBounds.X -= 1;
                    whiteBounds.Width += 1;
                }
            }
            else
            {
                if ((drawItemInfo.State & DrawItemState.Selected) <= 0)
                    whiteBounds.Width += 1;
                whiteBounds.X += 1;
                whiteBounds.Width -= 1;
            }
            GraphicsPath gp = this.GetBorderPathFromBounds(whiteBounds);

            if (ShouldDrawHighLightUpper)
            {
                drawItemInfo.Graphics.DrawPath(new Pen(Color.White, 1), gp);
            }

            drawItemInfo.Graphics.DrawPath(new Pen(this.BorderColor, 1),
                this.GetBorderPathFromBounds(curBounds));

            g.SmoothingMode = oldSM;

            RestoreGraphicsState(g);

            g.ResetTransform();
        }

        protected virtual RectangleF CorrectInteriorBounds(RectangleF bounds, RectangleF interriorBounds)
        {
            bool bIsVertical = (this.TabAlignment == TabAlignment.Left ||
                this.TabAlignment == TabAlignment.Right);

            float offset = bIsVertical ? bounds.Width : bounds.Height;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (bIsVertical)
            {
                if (!this.TabControl.Renderer.TabPanelData.RotateTextWhenVertical)
                {
                    if (!bIsMirrored)
                    {
                        bounds.Offset(0, offset);
                    }
                    else
                    {
                        bounds.Height += this.panelRenderer.TabPanelData.Padding.X;
                    }

                    if (this.ShowCloseButton)
                    {
                        int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                        if (bIsMirrored)
                        {
                            bounds.Offset(0, closeButtonOffset - this.CorrectCloseButtonPosition.X);
                        }

                        bounds.Height -= closeButtonOffset - this.CorrectCloseButtonPosition.X;
                    }

                    bounds.Height -= offset;
                }
                else
                {
                    // TODO: add logic for RotateTextWhenVertical = true 
                    // and ShrinkToFit correction.
                }
            }
            else
            {
                if (!bIsMirrored)
                {
                    bounds.Offset(offset, 0);
                }
                else
                {
                    bounds.Width += this.panelRenderer.TabPanelData.Padding.X;
                }

                if (this.ShowCloseButton)
                {
                    int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                    if (bIsMirrored)
                    {
                        bounds.Offset(closeButtonOffset - this.CorrectCloseButtonPosition.X, 0);
                    }

                    bounds.Width -= closeButtonOffset - this.CorrectCloseButtonPosition.X;
                }

                bounds.Width -= offset;
            }

            interriorBounds = RectangleF.Intersect(bounds, interriorBounds);

            return interriorBounds;
        }

        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF rectTextAndImage = drawItemInfo.BoundsInterior;
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
            switch (this.TabAlignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    rectTextAndImage = RectangleF.Inflate(rectTextAndImage, -(float)Math.Ceiling(overlapSize.Width / 2), 0);
                    break;

                case TabAlignment.Left:
                case TabAlignment.Right:
                    if (!this.NeedRotateTextWhenVertical)
                    {
                        rectTextAndImage = RectangleF.Inflate(rectTextAndImage, 0, -(float)Math.Ceiling(overlapSize.Width / 2));
                    }
                    break;
            }

            // Correct interrior bounds
            if (this.TabControl.Renderer.TabPanelData.SizeMode == TabSizeMode.ShrinkToFit)
            {
                rectTextAndImage = CorrectInteriorBounds(drawItemInfo.Bounds, rectTextAndImage);
            }

            rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, rectTextAndImage, true);

            ApplyTransform(g);

            RectangleF m_rectF = this.GetInteriorBounds(this.Bounds, true);

            float width = m_rectF.Width - 15F;
            float height = m_rectF.Height;

            if (rectTextAndImage.Width > width
                && this.panelRenderer.TabPanelData.SizeMode == TabSizeMode.Fixed)
            {
                rectTextAndImage.Inflate((width - rectTextAndImage.Width) / 2, 0);
            }

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            if (ShowCloseButton)
            {
                DrawCloseButton(g, closeButtonBounds);
            }

            // To draw the focus rectangle
            int nPadX = this.panelRenderer.TabPanelData.Padding.X;
            int nPadY = this.panelRenderer.TabPanelData.Padding.Y;

            TabControlAdv tabControl = this.TabControl as TabControlAdv;
            bool isMirrored = NeedMirroredBackground();

            if (this.NeedRotateTextWhenVertical)
            {
                m_rectF.X += nPadX + 1;
                width = m_rectF.Width - nPadX;
                height = m_rectF.Height - nPadY * 2;

                if (tabControl.Alignment == TabAlignment.Right)
                {
                    m_rectF.Y -= nPadY * 2;
                }
            }
            else
                if (tabControl.RightToLeft == RightToLeft.Yes)
                {
                    m_rectF.X += nPadX * 2 + 1;
                }

            RectangleF rectF = new RectangleF(m_rectF.X, m_rectF.Y, width, height);
            if (((int)drawItemInfo.State & (int)DrawItemState.Focus) > 0)
                DrawFocusRect(g, rectF, drawItemInfo.ForeColor, drawItemInfo.BackColor);

            g.ResetTransform();
        }

        public override int CloseButtonPadding
        {
            get
            {
                return c_iCloseButtonPadding;
            }
        }

        protected override Point CorrectCloseButtonPosition
        {
            get
            {
                return new Point(c_iCorrectCloseButtonX, 0);
            }
        }

        public override Color TabBorderColor
        {
            get
            {
                return Office2003Colors.SelBorderColor;
            }
        }
        #endregion DRAWING
    }

    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.OneNoteStyleRenderer"/>
    /// tab style.
    /// </summary>
    public class OneNoteStyleRendererProperty : TabUIDefaultProperties
    {
        // The selected tab overlaps the inactive tabs by this much.
        /// </override>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            if (tabSize != SizeF.Empty)
            {
                float ox = ((float)tabSize.Height * 4f / 3f) - 3.33f;
                if (ox < 6f) ox = 6f;
                return new SizeF(ox, 6);
            }
            return new SizeF(OneNoteStyleRenderer.DefaultOverlapSizeX, 6);
        }

        public override bool DrawLeftToRight
        {
            get { return false; }
        }

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.MenuItemHotColorDark;
        }

        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.MenuMarginColorDark;
        }

        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.DockBarColorDark;
        }

        public override Color DefaultFixedSingleBorderColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.SelBorderColor;
        }

        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            if (bounds.Height > 0 && bounds.Width > 0)
            {
                // Make sure to specify in the overload below that the bg is not solid.
                using (Brush brush = GetFillBrush(tabControl, bounds, bgColor))
                {
                    g.FillRectangle(brush, bounds);
                }
            }
        }

        public override bool IsBackgroundSolid()
        {
            return false;
        }

        private Brush GetFillBrush(ITabControl tabControl, RectangleF rectFill, Color backColor)
        {
            Brush brush = null;
            switch (((TabControlAdv)tabControl).Alignment)
            {
                case TabAlignment.Top:
                    brush = new LinearGradientBrush(rectFill, backColor,
                        Color.White, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Bottom:
                    brush = new LinearGradientBrush(rectFill, Color.White,
                        backColor, LinearGradientMode.Vertical);
                    break;

                case TabAlignment.Left:
                    brush = new LinearGradientBrush(rectFill, backColor,
                        Color.White, LinearGradientMode.Horizontal);
                    break;

                case TabAlignment.Right:
                    brush = new LinearGradientBrush(rectFill, Color.White,
                        backColor, LinearGradientMode.Horizontal);
                    break;
            }

            return brush;
        }
    }
    #region ONE_NOTE_STYLE_FLAT
    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.OneNoteStyleFlatTabsRenderer"/>
    /// tab style.
    /// </summary>
    public class OneNoteStyleFlatTabPanelProperty : TabUIDefaultProperties
    {
        /// </override>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(0, 6);
        }
        /// </override>
        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.MenuItemPressedColorDark;
        }
        /// </override>
        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.MenuMarginColorDark;
        }
        /// </override>
        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.DockBarColorDark;
        }
        /// </override>
        public override Color DefaultFixedSingleBorderColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Office2003Colors.SelBorderColor;
        }
        /// </override>
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            if (bounds.Height > 0 && bounds.Width > 0)
            {
                // Make sure to specify in the overload below that the bg is not solid.
                Brush b = new LinearGradientBrush(bounds, bgColor, Color.White, LinearGradientMode.Vertical);
                g.FillRectangle(b, bounds);
                b.Dispose();
            }
        }
        /// </override>
        public override bool IsBackgroundSolid()
        {
            return false;
        }
    }

    /// <summary>
    /// This tab renderer implements the Office 2003 OneNoteStyle vertical flat tabs look-and-feel.
    /// </summary>
    public class OneNoteStyleFlatTabsRenderer : TabRenderer3D
    {
        #region TABINIT
        // Use the above OneNoteStyleFlatTabsRendererTabPanelProperty as my default properties provider.
        static OneNoteStyleFlatTabPanelProperty tabPropertyExtender;
        /// <summary>
        /// Internal property.
        /// </summary>
        public static new OneNoteStyleFlatTabPanelProperty TabPanelPropertyExtender
        {
            get { return tabPropertyExtender; }
        }
        /// <summary>
        /// Internal property.
        /// </summary>
        public static new string TabStyleName
        {
            get { return "OneNoteFlatStyle"; }
        }

        static OneNoteStyleFlatTabsRenderer()
        {
            tabPropertyExtender = new OneNoteStyleFlatTabPanelProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(OneNoteStyleFlatTabsRenderer), TabPanelPropertyExtender);
        }
        public OneNoteStyleFlatTabsRenderer(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }

        /// </override>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return tabPropertyExtender.GetOverlapSize(tabSize);
        }

        #endregion TABINIT

        /// </override>
        public override bool HitTest(PointF mousePosition)
        {
            GraphicsPath path = null;
            bool selectedTab = (this.panelRenderer.TabPanelData.TabsData[this.panelRenderer.TabPanelData.SelectedIndex] == this.TabData);
            path = this.GetBorderPathFromBounds(this.GetCurrentBounds(), selectedTab);
            path.CloseFigure();

            Region region = new Region(path);

            // Hittest the region to verify if the point is in the rect
            if (region.IsVisible(mousePosition))
                return true;
            else
                return false;
        }

        #region DRAWING
        /// </override>
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            RectangleF curBounds = new RectangleF(drawItemInfo.Bounds.Left,
                drawItemInfo.Bounds.Top, drawItemInfo.Bounds.Width, drawItemInfo.Bounds.Height);

            curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, curBounds, true);

            // Make g horizontal
            ApplyTransform(g);

            Color bgColor = drawItemInfo.BackColor;

            // Get the border path and fill it.
            GraphicsPath path = null;
            path = this.GetBorderPathFromBounds(curBounds, ((drawItemInfo.State & DrawItemState.Selected) > 0));
            path.CloseFigure();

            if ((drawItemInfo.State & DrawItemState.HotLight) > 0)
            {
                bgColor = Office2003Colors.SelColor;
            }

            Brush brush = new LinearGradientBrush(curBounds, ControlPaint.LightLight(bgColor),
                bgColor, LinearGradientMode.Vertical);

            g.FillPath(brush, path);

            g.ResetTransform();
        }

        /// </override>
        private GraphicsPath GetBorderPathFromBounds(RectangleF bounds, bool selectedTab)
        {
            GraphicsPath path = new GraphicsPath();

            bool bIsMirrored = panelRenderer.IsMirrored;

            if (selectedTab)
            {
                float[] aptPtX = bIsMirrored ?
                    new float[]
					{
						bounds.Right,		/*0*/
						bounds.Right - 1,	/*1*/
						bounds.Right - 3,	/*2*/
						bounds.Right - 5,	/*3*/
						bounds.Left + 6,	/*4*/
						bounds.Left + 7,	/*5*/
						bounds.Left + 2,	/*6*/
						bounds.Left + 1,	/*7*/
						bounds.Left			/*8*/
					} :
                    new float[]
					{
						bounds.Left,		/*0*/
						bounds.Left + 1,	/*1*/
						bounds.Left + 3,	/*2*/
						bounds.Left + 5,	/*3*/
						bounds.Right - 6,	/*4*/
						bounds.Right - 7,	/*5*/
						bounds.Right - 2,	/*6*/
						bounds.Right - 1,	/*7*/
						bounds.Right		/*8*/
					};

                path.AddLine(
                    new PointF(aptPtX[0], bounds.Bottom - 1),
                    new PointF(aptPtX[0], bounds.Top + 5)
                    );

                path.AddBezier(
                    new PointF(aptPtX[0], bounds.Top + 5),
                    new PointF(aptPtX[1], bounds.Top + 3),
                    new PointF(aptPtX[2], bounds.Top + 1),
                    new PointF(aptPtX[3], bounds.Top)
                    );

                path.AddLine(
                    new PointF(aptPtX[3], bounds.Top),
                    new PointF(aptPtX[4], bounds.Top)
                    );

                path.AddBezier(
                    new PointF(aptPtX[5], bounds.Top),
                    new PointF(aptPtX[6], bounds.Top + 1),
                    new PointF(aptPtX[7], bounds.Top + 4),
                    new PointF(aptPtX[7], bounds.Top + 5)
                    );

                path.AddLine(
                    new PointF(aptPtX[8], bounds.Top + 5),
                    new PointF(aptPtX[8], bounds.Bottom)
                    );
            }
            else
            {
                float[] aptPtX = bIsMirrored ?
                    new float[]
					{
						bounds.Right,		/*0*/
						bounds.Right - 2,	/*1*/
						bounds.Left + 2,	/*2*/
						bounds.Left			/*3*/
					} :
                    new float[]
					{
						bounds.Left,		/*0*/
						bounds.Left + 2,	/*1*/
						bounds.Right - 2,	/*2*/
						bounds.Right		/*3*/
					};

                path.AddLine(
                    new PointF(aptPtX[0], bounds.Bottom - 1),
                    new PointF(aptPtX[0], bounds.Top + 2)
                    );

                path.AddLine(
                    new PointF(aptPtX[0], bounds.Top + 2),
                    new PointF(aptPtX[1], bounds.Top)
                    );

                path.AddLine(
                    new PointF(aptPtX[1], bounds.Top),
                    new PointF(aptPtX[2], bounds.Top)
                    );

                path.AddLine(
                    new PointF(aptPtX[2], bounds.Top),
                    new PointF(aptPtX[3], bounds.Top + 2)
                    );

                path.AddLine(
                    new PointF(aptPtX[3], bounds.Top + 2),
                    new PointF(aptPtX[3], bounds.Bottom)
                    );
            }
            return path;
        }

        /// </override>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            RectangleF r = new RectangleF(drawItemInfo.Bounds.Left,
                drawItemInfo.Bounds.Top, drawItemInfo.Bounds.Width, drawItemInfo.Bounds.Height);


            Graphics g = drawItemInfo.Graphics;

            r = TabUtils.ApplyTransform(g, this.TabAlignment, r, true);

            // Make g horizontal
            ApplyTransform(g);

            SmoothingMode oldSM = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            drawItemInfo.Graphics.DrawPath(new Pen(Office2003Colors.SelBorderColor, 1),
                this.GetBorderPathFromBounds(r, ((drawItemInfo.State & DrawItemState.Selected) > 0)));

            g.SmoothingMode = oldSM;

            g.ResetTransform();
        }
        /// </override>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            Rectangle bounds = drawItemInfo.BoundsInterior;
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment,
                RectangleF.FromLTRB(bounds.Left, bounds.Top, bounds.Right, bounds.Bottom), true);

            ApplyTransform(g);

            DrawTextAndImage(g, rectTextAndImage, drawItemInfo);

            if (ShowCloseButton)
            {
                this.DrawCloseButton(g, closeButtonBounds);
            }

            g.ResetTransform();
        }

        private const int c_iCloseButtonPaddingThemes = 8;
        private const int c_iCloseButtonPadding = 5;
        public override int CloseButtonPadding
        {
            get
            {
                int ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = c_iCloseButtonPaddingThemes;
                }
                else
                {
                    ret = c_iCloseButtonPadding;
                }

                return ret;
            }
        }

        private const int c_iCorrectCloseButtonX = -3;
        protected override Point CorrectCloseButtonPosition
        {
            get
            {
                Point ret;
                if (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.TabControl.ThemesEnabled)
                {
                    ret = new Point((panelRenderer.IsMirrored) ? -c_iCorrectCloseButtonX : c_iCorrectCloseButtonX, 0);
                }
                else
                {
                    ret = new Point(0, 0);
                }

                return ret;
            }
        }

        #endregion DRAWING
    }
    #endregion ONE_NOTE_STYLE_FLAT
}
