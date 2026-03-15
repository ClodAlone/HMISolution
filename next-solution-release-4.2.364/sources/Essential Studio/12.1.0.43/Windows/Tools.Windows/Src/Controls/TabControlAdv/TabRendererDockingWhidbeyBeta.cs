#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.//  Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// TabRenderer WhidbeyBeta.
    /// </summary>
    public class TabRendererDockingWhidbeyBeta : TabRendererWhidbey
    {
        #region Class constants
        /// <summary>
        /// Unique renderer name.
        /// </summary>
        private const string DEF_RENDERER_NAME = "VS2005DockingStyleBeta";
        private const int DEF_CURVE_OFFSET = 4;
        private const float DEF_CURVE_COEF = 0.25f;
        private const int DEF_CURVE_OFFSET_BOTTOM = 3;
        private const float DEF_CURVE_COEF_1 = 1.1f;
        private const float DEF_CURVE_COEF_2 = 4f;
        private const float DEF_CURVE_COEF_3 = 10f;
        private const float DEF_CURVE_SHRINK_TO_FIT_OFFSET = 1.5f;
        private const int c_iCorrectCloseButtonXThemes = 3;
        private const int c_iCloseButtonPadding = 0;
        #endregion

        #region Class members
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyWhidbey m_tabPropertyExtender;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets name of this tab renderer.
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
        public static new StyleRendererPropertyWhidbey TabPanelPropertyExtender
        {
            get
            {
                return m_tabPropertyExtender;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        static TabRendererDockingWhidbeyBeta()
        {
            m_tabPropertyExtender = new StyleRendererPropertyWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingWhidbeyBeta), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererDockingWhidbeyBeta), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererDockingWhidbeyBeta class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererDockingWhidbeyBeta(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }

        #endregion

        #region Class overrides
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF size = base.GetOverlapSize(tabSize);
            size.Width *= 2;
            return size;
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
                overlappedRect.X += overlappedSize.Height / 4;
                overlappedRect.Y -= overlappedSize.Width / 2;
                overlappedRect.Width += overlappedSize.Width / 4;
                overlappedRect.Height += overlappedSize.Width / 2;
            }
            else
            {
                // overlappedRect.X -= overlappedSize.Width / 4;
                overlappedRect.Width += overlappedSize.Width / 2;
            }

            // overlappedRect.Height += overlappedSize.Height;
            return overlappedRect;
        }

        public override RectangleF GetCurrentBounds()
        {
            // Will always draw over the adjacent tab
            SizeF overlappedSize = this.GetOverlapSize(this.panelRenderer.TabPanelData.TabSize);

            RectangleF overlappedRect = this.Bounds; 
            
            // Do not use the height which will just be some empty space on top.
            bool bNeedRotate = this.NeedRotateTextWhenVertical;

            if (panelRenderer.IsMirrored)
            {
                if (bNeedRotate)
                {
                    overlappedRect.X += overlappedSize.Height/4;
                    overlappedRect.Y -= (overlappedSize.Width/4)+2;
                    overlappedRect.Height += overlappedSize.Width/4;
                }
                else
                {
                    overlappedRect.X -= overlappedSize.Width/4;
                    overlappedRect.Width += overlappedSize.Width / 2;
                }
            }
            else
            {
                if (bNeedRotate)
                {
                    overlappedRect.Y -= overlappedSize.Width / 4;
                    overlappedRect.Height += overlappedSize.Width / 4;
                }
                else
                {
                    overlappedRect.X -= overlappedSize.Width / 4;
                    overlappedRect.Width += overlappedSize.Width / 2;
                }
            }

            return overlappedRect;
        }

        protected override GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            bool bIsMirrored = panelRenderer.IsMirrored;

            // left border curve
            PointF[] border = this.GetVerticalBorder(bounds, true);
            path.AddCurve(border, DEF_CURVE_COEF);

            // right border curve
            border = this.GetVerticalBorder(bounds, false);
            path.AddCurve(border, DEF_CURVE_COEF);

            return path;
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
                return new Point (panelRenderer.IsMirrored ? -c_iCorrectCloseButtonXThemes : c_iCorrectCloseButtonXThemes, 0);
            }
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
                    rectTextAndImage.Inflate(-(float)Math.Ceiling(overlapSize.Width / 3), 0);
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

        /// <summary>
        /// Gets the interior bounds of the tab.
        /// </summary>
        /// <param name="currentBounds">The bounds of the current tab</param>
        /// <param name="selectedTab">The Tab which is selected</param>
        /// <returns>Rectangle bounds</returns>
        protected override RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
        {
            RectangleF rectTextAndImage = currentBounds;

            int nPadX = this.panelRenderer.TabPanelData.Padding.X;
            int nPadY = this.panelRenderer.TabPanelData.Padding.Y;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (!this.NeedRotateTextWhenVertical)
            {
                rectTextAndImage.Width += nPadX;
            }

            rectTextAndImage = CorrectInteriorBounds(rectTextAndImage);

            rectTextAndImage = RectangleF.Inflate(rectTextAndImage, 0, -this.panelRenderer.TabPanelData.Padding.Y);

            return rectTextAndImage;
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
                    rectTextAndImage = RectangleF.Inflate(rectTextAndImage, -(float)Math.Ceiling(overlapSize.Width / 3), 0);
                    break;

                case TabAlignment.Left:
                case TabAlignment.Right:
                    if (!this.NeedRotateTextWhenVertical)
                    {
                        rectTextAndImage = RectangleF.Inflate(rectTextAndImage, 0, -(float)Math.Ceiling(overlapSize.Width / 3));
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

            // To draw the focus rectangle
            int nPadX = this.panelRenderer.TabPanelData.Padding.X;
            int nPadY = this.panelRenderer.TabPanelData.Padding.Y;

            TabControlAdv tabControl = this.TabControl as TabControlAdv;
            bool isMirrored = NeedMirroredBackground();

            if (NeedRotateTextWhenVertical)
            {
                m_rectF.X += nPadX / 2;
                m_rectF.Y -= nPadY * 2;
                width = m_rectF.Width - nPadX;
                height = m_rectF.Height - nPadY * 4;
            }
            else
                if (tabControl.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                {
                    m_rectF.X += nPadX * 4;
                }
                else
                {
                    m_rectF.X += nPadX;
                }

            RectangleF rectF = new RectangleF(m_rectF.X, m_rectF.Y, width, height);

            if (((int)drawItemInfo.State & (int)System.Windows.Forms.DrawItemState.Focus) > 0)
                DrawFocusRect(g, rectF, drawItemInfo.ForeColor, drawItemInfo.BackColor);

            if (ShowCloseButton)
            {
                DrawCloseButton(g, closeButtonBounds);
            }

            g.ResetTransform();
        }

        protected override RectangleF CorrectInteriorBounds(RectangleF bounds, RectangleF interriorBounds)
        {
            bool bIsVertical = this.TabAlignment == TabAlignment.Left || this.TabAlignment == TabAlignment.Right;

            float offset = bIsVertical ? bounds.Width * DEF_CURVE_SHRINK_TO_FIT_OFFSET :
                bounds.Height * DEF_CURVE_SHRINK_TO_FIT_OFFSET;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (bIsVertical)
            {
                if (!this.TabControl.Renderer.TabPanelData.RotateTextWhenVertical)
                {
                    bounds.Offset(0, offset / 2);

                    if (this.ShowCloseButton)
                    {
                        int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                        if (bIsMirrored)
                        {
                            bounds.Offset(0, closeButtonOffset + this.CorrectCloseButtonPosition.X);
                        }

                        bounds.Height -= closeButtonOffset + this.CorrectCloseButtonPosition.X;
                    }

                    bounds.Height -= offset;
                }
            }
            else
            {
                bounds.Offset(offset / 2, 0);

                if (this.ShowCloseButton)
                {
                    int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;

                    if (bIsMirrored)
                    {
                        bounds.Offset(closeButtonOffset + this.CorrectCloseButtonPosition.X, 0);
                    }

                    bounds.Width -= closeButtonOffset + this.CorrectCloseButtonPosition.X;
                }

                bounds.Width -= offset;
            }

            interriorBounds = RectangleF.Intersect(bounds, interriorBounds);

            return interriorBounds;
        }

        protected override RectangleF CorrectInteriorBounds(RectangleF rectTextAndImage)
        {
            TabPanelData panelData = this.panelRenderer.TabPanelData as TabPanelData;

            if (ShowCloseButton)
            {
                int closeButtonOffset = this.CloseButtonSize * 2 + this.CloseButtonPadding + this.CorrectCloseButtonPosition.X;
                bool bRotateTextWhenVertical = ShouldDrawRotatedWhenVertical;

                if (bRotateTextWhenVertical)
                {
                    rectTextAndImage.Height -= closeButtonOffset;
                }
                else
                {
                    rectTextAndImage.Width -= closeButtonOffset;
                }
                if (this.panelRenderer.IsMirrored)
                {
                    if (bRotateTextWhenVertical)
                    {
                        rectTextAndImage.Offset(0, closeButtonOffset);
                    }
                    else
                    {
                        rectTextAndImage.Offset(closeButtonOffset, 0);
                    }
                }
            }

            return rectTextAndImage;
        }

        protected override SizeF GetItemPreferredSize(Graphics g)
        {
            SizeF prefferedSize = base.GetItemPreferredSize(g);
            if (!this.ShouldDrawRotatedWhenVertical)
            {
                prefferedSize.Width += this.CloseButtonSize + this.CorrectCloseButtonPosition.X;
            }
            else
            {
                prefferedSize.Height += this.CloseButtonSize + this.CorrectCloseButtonPosition.X;
            }
            return prefferedSize;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Gets points of vertical border curve.
        /// </summary>
        /// <param name="bounds">Bounds to calculate borders for.</param>
        /// <param name="left">If true, calculate left border, othrerwise -
        /// right border.</param>
        /// <returns>Returns points of vertical border</returns>
        private PointF[] GetVerticalBorder(RectangleF bounds, bool left)
        {
            PointF[] border = null;

            if (bounds != RectangleF.Empty)
            {
                float height = bounds.Height;

                int slopeSpan = (int)(height * DEF_CURVE_COEF_1);
                int topSlopeSpanY = (int)(height / DEF_CURVE_COEF_2);
                int topSlopeSpanX = (int)(height / DEF_CURVE_COEF_3);

                slopeSpan -= DEF_CURVE_OFFSET + topSlopeSpanY;

                // calclulate border
                border = left ? new PointF[] { new PointF( bounds.Right, bounds.Bottom - 1 ), new PointF( bounds.Right - DEF_CURVE_OFFSET, bounds.Bottom - DEF_CURVE_OFFSET_BOTTOM ), new PointF(bounds.Right - ( DEF_CURVE_OFFSET + slopeSpan ), bounds.Top + topSlopeSpanX ), new PointF(bounds.Right - ( DEF_CURVE_OFFSET + slopeSpan + topSlopeSpanY ), bounds.Top ) } : new PointF[] { new PointF( bounds.Left + ( DEF_CURVE_OFFSET + slopeSpan + topSlopeSpanY ), bounds.Top ), new PointF( bounds.Left + ( DEF_CURVE_OFFSET + slopeSpan ), bounds.Top + topSlopeSpanX ), new PointF( bounds.Left + DEF_CURVE_OFFSET, bounds.Bottom - DEF_CURVE_OFFSET_BOTTOM ), new PointF( bounds.Left, bounds.Bottom - 1 ) };
            }
            return border;
        }
        #endregion
    }
}
