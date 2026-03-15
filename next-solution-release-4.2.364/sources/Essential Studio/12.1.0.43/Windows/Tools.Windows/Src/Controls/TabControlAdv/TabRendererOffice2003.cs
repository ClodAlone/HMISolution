#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
//  icensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// This renderer allows user to draw TabPages with Office2003 style.
    /// </summary>
    public class TabRendererOffice2003 : OneNoteStyleRenderer
    {
        #region Class constants
        private const int DEF_LABELEDIT_OFFSET_Y_TOP = 2;
        private const int DEF_LABELEDIT_OFFSET_Y_BOTTOM = 6;

        // Theese colors used to draw shadow for selected item.
        private static readonly Color DEF_SHADOW_COLOR_DARK = Color.FromArgb(70, Color.Black);
        private static readonly Color DEF_SHADOW_COLOR_LIGHT = Color.FromArgb(20, Color.Black);
        
        /// <summary>
        /// Width of line, drawn under items, when any item is selected.
        /// </summary>
        protected internal const int DEF_SELECTION_LINE_WIDTH = 5;
        
        /// <summary>
        /// The default horizontal overlap.
        /// </summary>
        protected internal const int c_defaultOverlapSizeX = 26;
       
        /// <summary>
        /// Unique renderer name.
        /// </summary>
        private const string DEF_RENDERER_NAME = "Office2003Style";
        
        /// <summary>
        /// Item border width.
        /// </summary>
        public const int DEF_BORDER_WIDTH = 1;

        /// <summary>
        /// Used for correct item bounds calculation.
        /// </summary>
       private const int DEF_ITEM_BOUNDS_OFFSET = 15;

        /// <summary>
        /// Used to draw borders in Office2003 style.
        /// </summary>
        private const int DEF_RIGHT_CORNER_OFFSET = 1;

        /// <summary>
        /// Width of shadow rectangle.
        /// </summary>
        private const int DEF_SHADOW_WIDTH = 3;

        /// <summary>
        /// Used in shadow shape creation.
        /// </summary>
        private const int DEF_SHADOW_CORNER_OFFSET = 2;
        #endregion

        #region Class members
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyOffice2003 m_tabPropertyExtender;

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
        public static new StyleRendererPropertyOffice2003 TabPanelPropertyExtender
        {
            get
            {
                return m_tabPropertyExtender;
            }
        }

        #endregion

        #region Class Initialize/Finalize methods
        static TabRendererOffice2003()
        {
            m_tabPropertyExtender = new StyleRendererPropertyOffice2003();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererOffice2003), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyOffice2003();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererOffice2003), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererOffice2003 class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererOffice2003(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }
        #endregion

        #region Class overrides
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

        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            // To draw the focus rectangle
            int nPadX = this.panelRenderer.TabPanelData.Padding.X;
            int nPadY = this.panelRenderer.TabPanelData.Padding.Y;

            RectangleF rectTextAndImage = drawItemInfo.BoundsInterior;
            RectangleF closeButtonBounds = GetCloseButtonBounds(drawItemInfo);

            SizeF overlapSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);
            switch (this.TabAlignment)
            {
                case TabAlignment.Top:
                case TabAlignment.Bottom:
                    if (!this.NeedMirroredBackground())
                    {
                        rectTextAndImage.X += overlapSize.Width / 2;
                    }
                    rectTextAndImage.Width -= overlapSize.Width / 2 - 3;
                    break;

                case TabAlignment.Left:
                    if (!this.NeedRotateTextWhenVertical)
                    {
                        if (!this.NeedMirroredBackground())
                        {
                            rectTextAndImage.Y += overlapSize.Width / 2;
                        }
                        rectTextAndImage.Height -= overlapSize.Width / 2;
                    }
                    else
                    {
                        if (!this.NeedMirroredBackground())
                        {
                             rectTextAndImage.X += overlapSize.Width / 2;
                        }
                        rectTextAndImage.Width -= overlapSize.Width / 2;
                    }
                    break;
                case TabAlignment.Right:
                    if (!this.NeedRotateTextWhenVertical)
                    {
                        if (!this.NeedMirroredBackground())
                        {
                            rectTextAndImage.Y += overlapSize.Width / 2;
                        }
                        rectTextAndImage.Height -= overlapSize.Width / 2;
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

            float width = m_rectF.Width;
            float height = m_rectF.Height;
            if (this.NeedMirroredBackground())
            {
                m_rectF.X -= overlapSize.Width / 2;
            }

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

            TabControlAdv tabControl = this.TabControl as TabControlAdv;
            bool isMirrored = NeedMirroredBackground();

            if (this.NeedRotateTextWhenVertical)
            {
                m_rectF.X += nPadX / 2;
                m_rectF.Y += nPadY / 2;
                width = m_rectF.Width - nPadX;
                height = m_rectF.Height - nPadY * 2;
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

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            // Draw borders, if bounds are not empty.
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                Graphics g = drawItemInfo.Graphics;

                RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

                // Make Graphics horizontal
                ApplyTransform(g);

                SmoothingMode oldSmoothingMode = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.HighQuality;

                bool bIsSelected = IsSelectedState(drawItemInfo.State);

                // Draw borders
                using (Pen borderPen = new Pen(Office2003Colors.SelBorderColor, DEF_BORDER_WIDTH))
                {
                    bool bIsMirrored = panelRenderer.IsMirrored;

                    bounds = CorrectItemBounds(bounds);

                    switch (this.TabAlignment)
                    {
                        case TabAlignment.Top:
                            if (bIsSelected)
                            {
                                bounds.Height -= DEF_BORDER_WIDTH;
                            }
                            break;

                        case TabAlignment.Bottom:
                            if (bIsSelected)
                            {
                                bounds.Height -= DEF_BORDER_WIDTH * 2;
                            }
                            break;

                        case TabAlignment.Left:
                            if (bIsSelected)
                            {
                                bounds.Height -= DEF_BORDER_WIDTH;
                            }

                            if (bIsMirrored)
                            {
                                bounds.Height -= DEF_BORDER_WIDTH;
                            }
                            break;

                        case TabAlignment.Right:
                            if (bIsSelected)
                            {
                                bounds.Height -= DEF_BORDER_WIDTH;
                            }
                            else if (!bIsMirrored)
                            {
                                bounds.Height += DEF_BORDER_WIDTH;
                            }
                            break;
                    }

                    // Draw Top border for selected item
                    if (bIsSelected)
                    {
                        if (bIsMirrored)
                        {
                            g.DrawLine(borderPen, bounds.Left + DEF_RIGHT_CORNER_OFFSET, bounds.Top, bounds.Right, bounds.Top);
                        }
                        else
                        {
                            g.DrawLine(borderPen, bounds.Left, bounds.Top, bounds.Right - DEF_RIGHT_CORNER_OFFSET, bounds.Top);
                        }
                    }

                    bool bShouldDrawBorder = true;
                    bShouldDrawBorder = bIsSelected || !IsFirstTab(true);

                    float leftBorderY = bIsSelected ? bounds.Top : bounds.Top + DEF_RIGHT_CORNER_OFFSET;

                    int bordersY = 0;

                    // Right border
                    if (bShouldDrawBorder)
                    {
                        bordersY = bIsMirrored ? (int)leftBorderY : (int)bounds.Top + DEF_RIGHT_CORNER_OFFSET;
                        g.DrawLine(borderPen, bounds.Right, bordersY, bounds.Right, bounds.Bottom);
                    }

                    // Left border
                    bShouldDrawBorder = bIsSelected || !IsFirstTab(false);
                    if (bShouldDrawBorder)
                    {
                        bordersY = bIsMirrored ? (int)bounds.Top + DEF_RIGHT_CORNER_OFFSET : (int)leftBorderY;
                        g.DrawLine(borderPen, bounds.Left, bounds.Bottom, bounds.Left, bordersY);
                    }
                }

                g.SmoothingMode = oldSmoothingMode;

                // Draw shadow for selected item.
                if (bIsSelected)
                {
                    RectangleF shadowBounds = GetShadowBounds(bounds);
                    DrawShadow(g, shadowBounds);
                }

                g.ResetTransform();
            }
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            // Draw background, if bounds are not empty.
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                // Draw it only for selected item.
                if (IsSelectedState(drawItemInfo.State))
                {
                    Graphics g = drawItemInfo.Graphics;

                    RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

                    // Get the border path and fill it
                    using (GraphicsPath path = this.GetBorderPathFromBounds(bounds))
                    {
                        // Make Graphics horizontal
                        base.ApplyTransform(g);

                        // Get appropriate colors for background
                        Color backColorTop = Office2003Colors.SelColor;
                        backColorTop = ChevronPainter.BlendColor(MenuColors.SelColor, backColorTop);
                        Color backColorBottom = Office2003Colors.MenuItemHotColorDark;

                        // Draw background
                        using (Brush brush = new LinearGradientBrush(bounds, backColorTop, backColorBottom, LinearGradientMode.Vertical))
                        {
                            g.FillPath(brush, path);
                        }

                        g.ResetTransform();
                    }
                }
            }
        }

        private bool m_bIgnoreBoundsCorrection = false;

        public override RectangleF GetTextPosition(Graphics g, string text, Font font, RectangleF rectLayout, StringFormat stringformat)
        {
            if (this.NeedRotateTextWhenVertical)
            {
                rectLayout = this.Bounds;
            }

            RectangleF rect = base.GetTextPosition(g, text, font, rectLayout, stringformat);
            return rect;
        }

        public override bool HitTest(PointF mousePosition)
        {
            m_bIgnoreBoundsCorrection = true;
            bool succeed = base.HitTest(mousePosition);
            m_bIgnoreBoundsCorrection = false;
            return succeed;
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            RectangleF correctedBounds = base.CorrectBounds(bounds);

            // Exclude selection line area because of it is not drawn by this renderer,
            correctedBounds.Height -= DEF_SELECTION_LINE_WIDTH;

            return correctedBounds;
        }

        protected override GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            if (m_bIgnoreBoundsCorrection)
            {
                return base.GetBorderPathFromBounds(bounds);
            }
            GraphicsPath path = new GraphicsPath();

            bool bIsMirrored = panelRenderer.IsMirrored;

            bounds = CorrectItemBounds(bounds);
            if (this.TabAlignment == TabAlignment.Bottom || this.TabAlignment == TabAlignment.Right)
            {
                bounds.Height -= DEF_BORDER_WIDTH;
            }

            // top border
            if (bIsMirrored)
            {
                path.AddLine(bounds.Left + DEF_RIGHT_CORNER_OFFSET, bounds.Top, bounds.Right, bounds.Top);
            }
            else
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
            }

            // right border
            int offsetY = bIsMirrored ? 0 : DEF_RIGHT_CORNER_OFFSET;
            path.AddLine(bounds.Right, bounds.Top + offsetY, bounds.Right, bounds.Bottom);

            // bottom border
            path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);

            // left border
            offsetY = bIsMirrored ? DEF_RIGHT_CORNER_OFFSET : 0;
            path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top + offsetY);

            path.CloseFigure();

            return path;
        }

        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF size = base.GetPreferredSize(g);
            size.Height += DEF_SELECTION_LINE_WIDTH / 2;
            return size;
        }

        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(c_defaultOverlapSizeX, 6);
        }

        /// <summary>
        /// Gets the interior bounds
        /// </summary>
        /// <param name="currentBounds">Current Bounds</param>
        /// <param name="selectedTab">Selected Tab</param>
        /// <returns>Returns interrior bounds</returns>
        protected override RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
        {
            RectangleF rectTextAndImage = currentBounds;
            if (!this.NeedRotateTextWhenVertical)
            {
                rectTextAndImage = RectangleF.Inflate(rectTextAndImage, -this.panelRenderer.TabPanelData.Padding.X, -this.panelRenderer.TabPanelData.Padding.Y);
            }
            else
            {
                rectTextAndImage.X += this.panelRenderer.TabPanelData.Padding.Y;
            }

            rectTextAndImage = CorrectInteriorBounds(rectTextAndImage);

            rectTextAndImage.X--;

            return rectTextAndImage;
        }

        protected override RectangleF CorrectInteriorBounds(RectangleF bounds, RectangleF interriorBounds)
        {
            bool bIsVertical = this.TabAlignment == TabAlignment.Left || this.TabAlignment == TabAlignment.Right;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (bIsVertical)
            {
                if (!this.TabControl.Renderer.TabPanelData.RotateTextWhenVertical)
                {
                    if (this.ShowCloseButton)
                    {
                        int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                        if (bIsMirrored)
                        {
                            bounds.Offset(0, closeButtonOffset - this.CorrectCloseButtonPosition.X);
                        }

                        bounds.Height -= closeButtonOffset - this.CorrectCloseButtonPosition.X;
                    }
                }
            }
            else
            {
                if (this.ShowCloseButton)
                {
                    int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                    if (bIsMirrored)
                    {
                        bounds.Offset(closeButtonOffset - this.CorrectCloseButtonPosition.X, 0);
                    }

                    bounds.Width -= closeButtonOffset - this.CorrectCloseButtonPosition.X;
                }
            }

            interriorBounds = RectangleF.Intersect(bounds, interriorBounds);

            return interriorBounds;
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
                switch (this.TabAlignment)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        if (!this.NeedMirroredBackground())
                        {
                            rectTextAndImage.X += overlapSize.Width / 2;
                        }
                        rectTextAndImage.Width -= overlapSize.Width / 2;
                        break;

                    case TabAlignment.Left:
                    case TabAlignment.Right:
                        if (!this.NeedRotateTextWhenVertical)
                        {
                            if (!this.NeedMirroredBackground())
                            {
                                rectTextAndImage.Y += overlapSize.Width / 2;
                            }
                            rectTextAndImage.Height -= overlapSize.Width / 2;
                        }
                        break;
                }

                rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, rectTextAndImage, true);

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

        #endregion

        #region Class utility methods
        /// <summary>
        /// Indicates whether the item is the first item from the specified side.
        /// </summary>
        /// <param name="bIsRightToLeft">true if Right to Left</param>
        /// <returns>Returns true if first tab</returns>
        private bool IsFirstTab(bool bIsRightToLeft)
        {
            bool bIsFirstTab = true;

            if (this.TabControl != null && this.TabControl.Renderer != null &&
                this.TabControl.Renderer.Renderers != null)
            {
                ArrayList arrRenderers = this.TabControl.Renderer.Renderers;
                if (arrRenderers.Count > 0)
                {
                    TabRendererBase tabRenderer = null;
                    RectangleF currentBounds = this.GetCurrentBounds();

                    for (int i = 0, len = arrRenderers.Count; i < len; i++)
                    {
                        tabRenderer = arrRenderers[i] as TabRendererBase;

                        if (tabRenderer != null && tabRenderer != this)
                        {
                            RectangleF bounds = tabRenderer.GetCurrentBounds();

                            if (bIsRightToLeft)
                            {
                                if (bounds.Right > currentBounds.Right)
                                {
                                    bIsFirstTab = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (bounds.Left < currentBounds.Left)
                                {
                                    bIsFirstTab = false;
                                    break;
                               }
                            }
                        }
                    }
                }
            }

            return bIsFirstTab;
        }

        private RectangleF CorrectItemBounds(RectangleF bounds)
        {
            if (!this.NeedRotateTextWhenVertical)
            {
                if (!panelRenderer.IsMirrored)
                {
                    bounds.Offset(DEF_ITEM_BOUNDS_OFFSET, 0);
                }

                bounds.Width -= DEF_ITEM_BOUNDS_OFFSET;
            }
            return bounds;
        }

        /// <summary>
        /// Returns the border's path for item, based on item's bounds and whether item is selected or not.
        /// </summary>
        /// <returns> Returns the border's path</returns>
        /// <param name="bounds">Rectangle bounds</param>
        /// <param name="bIsSelected">true if selected</param>
        protected GraphicsPath GetBorderPathFromBounds(RectangleF bounds, bool bIsSelected)
        {
            GraphicsPath path = new GraphicsPath();

            bounds = CorrectItemBounds(bounds);

            if (bIsSelected)
            {
                // Top border
                path.AddLine(bounds.Left, bounds.Top, bounds.Right - DEF_RIGHT_CORNER_OFFSET, bounds.Top);
            }

            // Right border
            path.AddLine(bounds.Right, bounds.Top + DEF_RIGHT_CORNER_OFFSET, bounds.Right, bounds.Bottom);

            float leftBorderY = bIsSelected ? bounds.Top : bounds.Top + DEF_RIGHT_CORNER_OFFSET;

            // Left border
            path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, leftBorderY);

            return path;
        }

        /// <summary> 
        /// Returns the path to draw the shadow in.
        /// </summary>
        /// <param name="bounds"> Shadow rectangle. </param>
        /// <returns> Path to draw shadow in. </returns>
        private GraphicsPath GetShadowPath(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            bool bIsMirrored = panelRenderer.IsMirrored;

            // RightToLeft
            if (bIsMirrored)
            {
                path.AddLine(bounds.Left, bounds.Top + DEF_SHADOW_CORNER_OFFSET, bounds.Right, bounds.Top);
                path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top + DEF_SHADOW_CORNER_OFFSET);
            }
            else
            {
                path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top + DEF_SHADOW_CORNER_OFFSET);
                path.AddLine(bounds.Right, bounds.Top + DEF_SHADOW_CORNER_OFFSET, bounds.Right, bounds.Bottom);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
            }

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Returns the rectangle to draw shadow in from item's bounds.
        /// </summary>
        /// <param name="bounds"> Item bounds to draw shadow for. </param>
        /// <returns>Returns Shadow bounds</returns>
        private RectangleF GetShadowBounds(RectangleF bounds)
        {
            bool bisMirrored = this.panelRenderer.IsMirrored;

            bounds = CorrectItemBounds(bounds);

            if (bisMirrored)
            {
                bounds.Offset(-DEF_SHADOW_WIDTH, DEF_RIGHT_CORNER_OFFSET);
            }
            else
            {
                bounds.Offset(bounds.Width, DEF_RIGHT_CORNER_OFFSET);
            }

            bounds.Height -= DEF_RIGHT_CORNER_OFFSET;
            bounds.Width = DEF_SHADOW_WIDTH;

            return bounds;
        }

        /// <summary>
        /// Draws shadow in specified Rectangle.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="shadowRectangle">Shadow Ractangle</param>
        protected virtual void DrawShadow(Graphics g, RectangleF shadowRectangle)
        {
            if (g == null)
                throw new ArgumentNullException("g");

            using (Brush brush = new LinearGradientBrush(shadowRectangle, DEF_SHADOW_COLOR_DARK, DEF_SHADOW_COLOR_LIGHT, LinearGradientMode.Horizontal))
            {
                GraphicsPath path = this.GetShadowPath(shadowRectangle);

                g.FillPath(brush, path);
            }
        }

        #endregion
    }
}
