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
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    public  class TabRendererVS2010: TabRenderer2D
    {
        #region constants
        private const string DEF_RENDERER_NAME = "VS2010Style";
        private const int DEF_CORNER_RADIUS = 2;
        private const int DEF_OVERLAP_HEIGHT = 3;
        protected internal const int DEF_SELECTION_LINE_WIDTH = 5;
        #endregion

        #region fields

        private static StyleRendererPropertyVS2010 tabPanelProperty;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the <see cref="TabRendererVS2010"/> class.
        /// </summary>
        static TabRendererVS2010()
        {
            tabPanelProperty = new StyleRendererPropertyVS2010();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererVS2010), tabPanelProperty);
        }

        public TabRendererVS2010(ITabControl tabControl, ITabPanelRenderer tabPanelRenderer):base(tabControl,tabPanelRenderer)
        {

        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the name of the tab style.
        /// </summary>
        /// <value>The name of the tab style.</value>
        public static new string TabStyleName
        {
            get
            {
                return DEF_RENDERER_NAME;
            }
        }

        /// <summary>
        /// Gets the tab panel property extender.
        /// </summary>
        /// <value>The tab panel property extender.</value>
        public static new StyleRendererPropertyVS2010 TabPanelPropertyExtender
        {
            get
            {
                return tabPanelProperty;
            }
        }

        /// <summary>
        /// Bounds of the close button.
        /// </summary>
        private Rectangle lastCloseButtonBounds = Rectangle.Empty;

        /// <summary>
        /// Gets the bounds of the close button.
        /// </summary>
        public new Rectangle CloseButtonBounds
        {
            get
            {
                return lastCloseButtonBounds;
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Registers the type of the tab.
        /// </summary>
        public static new void RegisterTabType()
        {
            tabPanelProperty = new StyleRendererPropertyVS2010();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererVS2010), tabPanelProperty);
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                Graphics g = drawItemInfo.Graphics;
                RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
                base.ApplyTransform(g);
               
                if (this.IsSelectedState(drawItemInfo.State))
                {
                    Color startColor = Color.FromArgb(255, 252, 242);
                    Color endColor = Color.FromArgb(255, 232, 166);
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Vertical))
                    {
                        Blend blend = new Blend();
                        blend.Factors = new float[] { 0.0f, 0.2f, 1f, 1f };
                        blend.Positions = new float[] { 0.0f, 0.4f, 0.5f, 1f };
                        brush.Blend = blend;
                        g.FillRectangle(brush, bounds);
                    }
                    FillCorners(bounds, g);
                }
                else if (this.IsHotLightState(drawItemInfo.State))
                {
                    Color startColor = Color.FromArgb(111, 119, 118);
                    Color endColor = Color.FromArgb(79, 95, 116);
                    using (LinearGradientBrush brush = new LinearGradientBrush(bounds, startColor, endColor, LinearGradientMode.Vertical))
                    {
                        RectangleF rec = RectangleF.Empty;
                        if (this.TabAlignment == TabAlignment.Top || this.TabAlignment == TabAlignment.Left)
                            rec = new RectangleF(bounds.Location, new SizeF(bounds.Width, bounds.Height - DEF_SELECTION_LINE_WIDTH + 1));
                        else
                            rec = new RectangleF(bounds.Location, new SizeF(bounds.Width, bounds.Height - DEF_SELECTION_LINE_WIDTH + 4));
                        Blend blend = new Blend();
                        blend.Factors = new float[] { 0.0f, 0.6f , 1f };
                        blend.Positions = new float[] { 0.0f,0.5f, 1f };
                        brush.Blend = blend;
                        g.FillRectangle(brush, rec);
                        using (Pen pen = new Pen(Color.FromArgb(155, 167, 183)))
                        {
                            Rectangle rect = new Rectangle((int)rec.X,(int) rec.Y,(int) rec.Width - 1 ,(int) rec.Height);
                            g.DrawRectangle(pen, rect);
                        }
                    }
                    Color corner = Color.FromArgb(41, 57, 85);
                    using (SolidBrush bh = new SolidBrush(corner))
                    {
                        if (TabAlignment == TabAlignment.Top || TabAlignment == TabAlignment.Left)
                        {
                            RectangleF cornerRec = new RectangleF(bounds.X, bounds.Y, 1, 1);
                            g.FillRectangle(bh, cornerRec);
                            cornerRec = new RectangleF(bounds.Right - 1, bounds.Y, 1, 1);
                            g.FillRectangle(bh, cornerRec);
                        }
                        else
                        {
                            RectangleF cornerRec = new RectangleF(bounds.X, bounds.Y-1, 1, 1);
                            g.FillRectangle(bh, cornerRec);
                            cornerRec = new RectangleF(bounds.Right - 1, bounds.Y-1, 1, 1);
                            g.FillRectangle(bh, cornerRec);
                        }
                    }
                }
                else
                {
                    using(SolidBrush solidBrush=new SolidBrush(Color.FromArgb(41,57,85)))
                    {
                        RectangleF rec = new RectangleF(bounds.Location, new SizeF(bounds.Width, bounds.Height - DEF_SELECTION_LINE_WIDTH));
                        g.FillRectangle(solidBrush, rec);
                    }
                }
                g.ResetTransform();
            }
        }

        private void FillCorners(RectangleF bounds, Graphics g)
        {
            Color cornerEnd = Color.FromArgb(205, 207, 206);
            Color corner = Color.FromArgb(41, 57, 85);
            RectangleF cornerRec = new RectangleF(bounds.X, bounds.Y + 1, 1, 1);
            using (SolidBrush bh = new SolidBrush(cornerEnd))
            {
                g.FillRectangle(bh, cornerRec);
                cornerRec = new RectangleF(bounds.X + 1, bounds.Y, 1, 1);
                g.FillRectangle(bh, cornerRec);
                cornerRec = new RectangleF(bounds.Right - 1, bounds.Y + 1, 1, 1);
                g.FillRectangle(bh, cornerRec);
                cornerRec = new RectangleF(bounds.Right - 2, bounds.Y, 1, 1);
                g.FillRectangle(bh, cornerRec);
                bh.Color = corner;
                cornerRec = new RectangleF(bounds.X, bounds.Y, 1, 1);
                g.FillRectangle(bh, cornerRec);
                cornerRec = new RectangleF(bounds.Right - 1, bounds.Y, 1, 1);
                g.FillRectangle(bh, cornerRec);
            }
        }

        protected override void DrawCloseButton(Graphics g, RectangleF closeButtonBounds)
        {
           // base.DrawCloseButton(g, closeButtonBounds);
            Color borderColor = Color.FromArgb(229, 195, 101);
            Color bgColor = Color.FromArgb(255, 252, 244);
            if (g == null)
                throw new ArgumentNullException("g");

            if ((int)closeButtonBounds.Width >= this.CloseButtonSize &&
                (int)closeButtonBounds.Height >= this.CloseButtonSize)
            {
                GraphicsState savedState = null;

                if (ShouldDrawRotatedWhenVertical)
                {
                    savedState = g.Save();
                    g.ResetTransform();
                }

                closeButtonBounds.X += CorrectCloseButtonPosition.X;
                closeButtonBounds.Y += CorrectCloseButtonPosition.Y;

                closeButtonBounds.Height--;
                Rectangle rect = Rectangle.Ceiling(closeButtonBounds);

                if (this.HitCloseButton)
                {
                    Rectangle rec = Rectangle.Round(closeButtonBounds);
                    rec.Height = rec.Height + 1;
                    rec.Inflate(3 , 3);
                    using (SolidBrush sbr = new SolidBrush(bgColor))
                    {
                        g.FillRectangle(sbr, rec);
                    }
                    
                    ControlPaint.DrawBorder(g, rec, borderColor, ButtonBorderStyle.Solid);
                }

                Pen pen = new Pen(Color.FromArgb(200, Color.Black));

                g.DrawLine(pen, rect.X + 1, rect.Y,
                    rect.X + rect.Width, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X, rect.Y,
                    rect.X + rect.Width - 1, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X + rect.Width - 1, rect.Y,
                    rect.X, rect.Y + rect.Height);

                g.DrawLine(pen, rect.X + rect.Width, rect.Y,
                    rect.X + 1, rect.Y + rect.Height);



                if (savedState != null)
                {
                    g.Restore(savedState);
                }

                lastCloseButtonBounds = Rectangle.Round(closeButtonBounds);
            }
        }

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            //base.DrawFocusRect(g, focusRect, fore, back);
        }

        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            base.DrawInterior(drawItemInfo);
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
           // base.DrawBorders(drawItemInfo);
        }

        protected override Color GetForeColor()
        {
            bool selectedTab = panelRenderer.TabPanelData.SelectedIndex == -1 ?
                false : panelRenderer.TabPanelData.TabsData[panelRenderer.TabPanelData.SelectedIndex] ==this.TabData;
            if (!selectedTab)
            {
                return Color.White;
            }
            return base.GetForeColor();
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            if (this.TabAlignment == TabAlignment.Top || this.TabAlignment == TabAlignment.Left)
            {
                return new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height - 1);
            }

            return new RectangleF(bounds.X, bounds.Y + 1, bounds.Width, bounds.Height - 4);
        }

        protected override RectangleF CorrectInteriorBounds(RectangleF rectTextAndImage)
        {
            TabPanelData panelData = this.panelRenderer.TabPanelData as TabPanelData;

            if (ShowCloseButton)
            {
                int closeButtonOffset = this.CloseButtonSize + this.CloseButtonPadding;
                bool bRotateTextWhenVertical = ShouldDrawRotatedWhenVertical;

                if (bRotateTextWhenVertical)
                {
                    rectTextAndImage.Height -= closeButtonOffset;
                    if(TabAlignment == TabAlignment.Right)
                        rectTextAndImage.Height += 4;
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
        public override bool CloseButtonHitTest(Point pt)
        {
            //return base.CloseButtonHitTest(pt);
            return lastCloseButtonBounds.Contains(pt);
        }

        #endregion
    }
    public class StyleRendererPropertyVS2010 : TabPanelProperty2D
    {
        #region constants
       
        #endregion

        #region fields

        #endregion

        #region Initialization


        #endregion
        
        #region properties

        #endregion

        #region methods
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            //base.OnPaintPanelBackground(tabControl, g, bgColor, bounds);
        }

        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            return new SizeF(2, 5);
        }
        #endregion
    }
}
