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
    public class StyleRendererPropertyVS2008 : OneNoteStyleRendererProperty
    {
        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 7;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets a value indicating whether host OS is Vista.
        /// </summary>
        internal bool IsVistaOS
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the default items border color.
        /// </summary>
        public virtual Color DefaultBorderColor
        {
            get
            {
                Color borderColor = Color.Empty;
                if (this.IsVistaOS)
                {
                    borderColor = Color.FromArgb(127, 157, 185);
                }
                else
                {
                    borderColor = WindowsXPThemeColors.TabControlAdvActiveBorderColor;
                }

                return borderColor;
            }
        }
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
        }

        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            Color backgroundColor = Color.Empty;
            if (this.IsVistaOS)
            {
                backgroundColor = Color.FromArgb(233, 236, 250);
            }
            else
            {
                backgroundColor = WindowsXPThemeColors.TabControlAdvTabPanelBackGroundColor;
            }

            return backgroundColor;
        }

        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            Color inactiveTabColor = Color.Empty;
            if (this.IsVistaOS)
            {
                inactiveTabColor = Color.FromArgb(154, 182, 211);
            }
            else
            {
                inactiveTabColor = WindowsXPThemeColors.TabControlAdvInactiveBottomTabColor;
            }

            return inactiveTabColor;
        }

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            Color activeTabColor = Color.Empty;
            if (this.IsVistaOS)
            {
                activeTabColor = Color.FromArgb(210, 230, 250);
            }
            else
            {
                activeTabColor = WindowsXPThemeColors.TabControlAdvActiveBottomTabColor;
            }

            return activeTabColor;
        }

        public override Font DefaultActiveTabFont(ITabPanelData panelData, ITabControl tabControl)
        {
            return Syncfusion.Drawing.FontUtil.CreateFont(this.DefaultInactiveTabFont(panelData, tabControl), FontStyle.Bold);
        }

        // The selected tab overlaps the inactive tabs by this much.
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF overlapSize = base.GetOverlapSize(tabSize);
            overlapSize.Height = DEF_OVERLAP_HEIGHT;
            return overlapSize;
        }
        #endregion
    }

    public class TabRendererVS2008 : OneNoteStyleRenderer
    {
        #region Constants

        private const string DEF_RENDERER_NAME = "VS2008Style";
     
        /// <summary>
        /// Selected item line width.
        /// </summary>
        protected internal const int DEF_SELECTION_LINE_WIDTH = 2;
        private const int DEF_CLOSE_BUTTON_PADDING = 3;

        private const int DEF_DEFAULT_BORDER_WIDTH = 5;
       
        /// <summary>
        /// Selected item border color.
        /// </summary>
        protected internal static readonly Color DEF_SELECTED_BORDER_COLOR = Color.FromArgb(127, 157, 185);
        private const int c_cornerCut = 3;
        #endregion

        #region Members
       
        /// <summary>
        /// Color to draw tabs borders with.
        /// </summary>
        private Color m_borderColor = m_tabPropertyExtender.DefaultBorderColor;
       
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyVS2008 m_tabPropertyExtender;
       
        private RectangleF m_panelBounds = RectangleF.Empty;
       
        /// <summary>
        /// Indicates whether border path is used for drawing background.
        /// </summary>
        private bool m_borderPathForBackground = false;
        #endregion

        #region Initialization
        static TabRendererVS2008()
        {
            m_tabPropertyExtender = new StyleRendererPropertyVS2008();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererVS2008), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyVS2008();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererVS2008), TabPanelPropertyExtender);
        }
   
        /// <summary>
        /// Initializes a new instance of the TabRendererVS2008 class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererVS2008(ITabControl parent, ITabPanelRenderer panelRenderer) : base(parent, panelRenderer)
        {
        }
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
        public static new StyleRendererPropertyVS2008 TabPanelPropertyExtender
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
                Color borderColor = Color.Empty;
                if (this.IsVistaOS)
                {
                    borderColor = Color.FromArgb(127, 157, 185);
                }
                else
                {
                    borderColor = WindowsXPThemeColors.TabControlAdvActiveBorderColor;
                }

                return borderColor;
            }
        }

        protected override Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
        }

        public override int CloseButtonPadding
        {
            get
            {
                return DEF_CLOSE_BUTTON_PADDING;
            }
        }

        private bool IsVerticalTabAlignment
        {
            get
            {
                return this.TabAlignment == TabAlignment.Top || this.TabAlignment == TabAlignment.Bottom;
            }
        }

        /// <summary>
        /// Gets a value indicating whether host OS is Vista.
        /// </summary>
        internal bool IsVistaOS
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }
        #endregion

        #region Class overrides

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
                overlappedRect.Y -= this.panelRenderer.TabPanelData.Padding.X;
                overlappedRect.Height += this.panelRenderer.TabPanelData.Padding.X;
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

            // overlappedRect.Height += overlappedSize.Height;
            return overlappedRect;
        }

        /// <summary>
        /// Gets the interior bounds of the tab.
        /// </summary>
        /// <param name="currentBounds">The bounds of the current tab</param>
        /// <param name="selectedTab">The Tab which is selected</param>
        /// <returns>Returns Rectangle</returns>
        protected override RectangleF GetInteriorBounds(RectangleF currentBounds, bool selectedTab)
        {
            RectangleF rectTextAndImage = currentBounds;

            int nPadX = this.panelRenderer.TabPanelData.Padding.X;
            int nPadY = this.panelRenderer.TabPanelData.Padding.Y;

            bool bIsMirrored = this.NeedMirroredBackground();

            if (this.NeedRotateTextWhenVertical)
            {
                if (this.TabAlignment == TabAlignment.Left)
                {
                    if (bIsMirrored)
                    {
                        rectTextAndImage.Y -= nPadX + 1;
                    }
                    else
                    {
                        rectTextAndImage.Y += nPadX;
                    }
                }
                else if (this.TabAlignment == TabAlignment.Right)
                {
                    if (bIsMirrored)
                    {
                        rectTextAndImage.Y += nPadX + 1;
                    }
                    else
                    {
                        rectTextAndImage.Y -= nPadX;
                    }
                }
            }
            else
                if (bIsMirrored)
                {
                    rectTextAndImage.X -= nPadX + this.parent.BorderWidth;
                    rectTextAndImage.Width += nPadX - 1;
                }
                else
                {
                    rectTextAndImage.X += nPadX;
                    rectTextAndImage.Width += nPadX;
                }

            rectTextAndImage = CorrectInteriorBounds(rectTextAndImage);

            rectTextAndImage = RectangleF.Inflate(rectTextAndImage, 0, -nPadY);

            return rectTextAndImage;
        }

        protected virtual GraphicsPath GetTabBorderPath(RectangleF panelBounds, RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            int tabBorderWidth = this.TabControl.BorderWidth > 2 ? this.TabControl.BorderWidth : 3;
            float height = bounds.Height;
            bool bIsMirrored = this.NeedMirroredBackground();
            float bottom = bounds.Bottom;

            PointF[] aptLines = null;

            aptLines = bIsMirrored ?
                new PointF[] { new PointF( panelBounds.Right - 1, bottom ), new PointF( panelBounds.Right - 1, bottom - c_cornerCut + 1 ), new PointF( panelBounds.Right - c_cornerCut, bottom - ( tabBorderWidth - c_cornerCut + 1 ) ) } :
                new PointF[]
                { 
                    new PointF( panelBounds.Left, bottom ),
                    new PointF( panelBounds.Left, bottom - c_cornerCut + 1 ),
                    new PointF( panelBounds.Left + c_cornerCut - 1, bottom - ( tabBorderWidth - c_cornerCut + 1 ) ) 
                };

            path.AddLines(aptLines);

            PointF[] aptCurve = bIsMirrored ?
                new PointF[] { new PointF( bounds.Right - c_cornerCut, bottom - ( tabBorderWidth - c_cornerCut + 1 ) ), new PointF( bounds.Right - tabBorderWidth - c_cornerCut + 4, bounds.Bottom - ( tabBorderWidth - c_cornerCut + 1 ) ), new PointF( bounds.Right - height + 1, bounds.Top + 2 ), new PointF( bounds.Right - height - 5, bounds.Top ) } 
                :
                new PointF[]
                {
                    new PointF( bounds.Left + c_cornerCut - 1, bounds.Bottom - ( tabBorderWidth - c_cornerCut + 1 ) ),
                    new PointF( bounds.Left + tabBorderWidth - c_cornerCut + 1, bounds.Bottom - ( tabBorderWidth - c_cornerCut + 1 ) ),
                    new PointF( bounds.Left + height - 2, bounds.Top + 2 ),
                    new PointF( bounds.Left + height + 4, bounds.Top )
                };

            path.AddLines(aptCurve);

            aptLines = bIsMirrored ?
                new PointF[] { new PointF( bounds.Left + 2, bounds.Top ), new PointF( bounds.Left, bounds.Top + 2 ), new PointF( bounds.Left, bounds.Bottom - tabBorderWidth + 2 ) } 
                    :
                new PointF[]
                {
                    new PointF( bounds.Right - 2, bounds.Top ),
                    new PointF( bounds.Right, bounds.Top + 2 ),
                    new PointF( bounds.Right, bounds.Bottom - tabBorderWidth + 2 )
                };

            path.AddLines(aptLines);

            if ((this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Width > panelBounds.Width)
                || ( !this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Height > panelBounds.Width))
            {
                if (bIsMirrored)
                {
                    aptLines = new PointF[]
                    {
                        new PointF( panelBounds.Left - 1, panelBounds.Bottom - tabBorderWidth )
                    };
                }
                else
                {
                    aptLines = new PointF[]
                    {
                        new PointF( panelBounds.Right, panelBounds.Bottom - tabBorderWidth )
                    };
                }
            }
            else
            {
                aptLines = bIsMirrored ?
                    new PointF[] { new PointF( panelBounds.Left + c_cornerCut - 1, bottom - ( tabBorderWidth - c_cornerCut + 1 ) ), new PointF( panelBounds.Left, bottom - c_cornerCut + 1 ), new PointF( panelBounds.Left, bottom ) } : 
                    new PointF[]
                    { 
                        new PointF( panelBounds.Right - c_cornerCut, bottom - ( tabBorderWidth - c_cornerCut + 1 ) ),
                        new PointF( panelBounds.Right - 1, bottom - c_cornerCut + 1 ),
                        new PointF( panelBounds.Right - 1, bottom )
                    };
            }

            path.AddLines(aptLines);

            return path;
        }

        /// <summary>
        /// Gets the path for the tab.
        /// </summary>
        /// <param name="bounds">Rectangle bounds</param>
        /// <returns>Returns Graphics Path</returns>
        protected override GraphicsPath GetBorderPathFromBounds(RectangleF bounds)
        {
            GraphicsPath path = new GraphicsPath();

            int tabBorderWidth = this.TabControl.BorderWidth > 2 ? this.TabControl.BorderWidth : 3;
            float height = bounds.Height;

            bool bIsMirrored = this.NeedMirroredBackground();

            int adjustValue = 0;
            if (!m_borderPathForBackground)
            {
                adjustValue = 1;
            }

            PointF[] aptLines = null;

            PointF[] aptCurve = bIsMirrored ?
                new PointF[] { new PointF( bounds.Right - 3, bounds.Bottom - adjustValue ), new PointF( bounds.Right - 3, bounds.Bottom - 2 ), new PointF( bounds.Right - tabBorderWidth - 1, bounds.Bottom - tabBorderWidth ), new PointF( bounds.Right - height + 1, bounds.Top + 2 ), new PointF( bounds.Right - height - 5, bounds.Top + 1 ) } 
                    :
                new PointF[]
                {
                    new PointF( bounds.Left + 2, bounds.Bottom - adjustValue ),
                    new PointF( bounds.Left + 2, bounds.Bottom - 2 ),
                    new PointF( bounds.Left + tabBorderWidth, bounds.Bottom - tabBorderWidth ),
                    new PointF( bounds.Left + height - 3, bounds.Top + 3 ),
                    new PointF( bounds.Left + height + 2, bounds.Top + 1 )
                };

            path.AddLines(aptCurve);

            aptLines = bIsMirrored ?
                new PointF[] { new PointF( bounds.Left + 2, bounds.Top + 1 ), new PointF( bounds.Left, bounds.Top + 3 ), new PointF( bounds.Left, bounds.Bottom - adjustValue ) }
                :
                new PointF[]
                {
                    new PointF( bounds.Right - 2, bounds.Top + 1 ),
                    new PointF( bounds.Right, bounds.Top + 3 ),
                    new PointF( bounds.Right, bounds.Bottom - adjustValue )
                };

            path.AddLines(aptLines);

            return path;
        }

        /// <summary>
        /// Gets the path for the line beneath the tabs.
        /// </summary>
        /// <returns>Returns Graphics path</returns>
        protected virtual GraphicsPath GetLowerTabBorderPath()
        {
            GraphicsPath path = new GraphicsPath();
            int tabBorderWidth = this.TabControl.BorderWidth > 2 ? this.TabControl.BorderWidth : 3;
            bool bIsMirrored = this.NeedMirroredBackground();
            int rightCorner = c_cornerCut;
            int leftCorner = c_cornerCut;
            PointF[] aptLines = null;

            if ((bIsMirrored && (this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Width > m_panelBounds.Width)) || (!this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Height > m_panelBounds.Width))
            {
                leftCorner = 0;
            }

            aptLines = new PointF[] { new PointF( m_panelBounds.Left, m_panelBounds.Bottom ), new PointF( m_panelBounds.Left, m_panelBounds.Bottom - ( tabBorderWidth - leftCorner + 1 ) ), new PointF( m_panelBounds.Left + leftCorner, m_panelBounds.Bottom - tabBorderWidth ) };

            path.AddLines(aptLines);

            if ((!bIsMirrored && (this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Width > m_panelBounds.Width)) || (!this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Height > m_panelBounds.Width))
            {
                rightCorner = 0;
            }

            aptLines = new PointF[] { new PointF( m_panelBounds.Right - rightCorner, m_panelBounds.Bottom - tabBorderWidth ), new PointF( m_panelBounds.Right, m_panelBounds.Bottom - ( tabBorderWidth - rightCorner + 1 ) ), new PointF( m_panelBounds.Right, m_panelBounds.Bottom ) };

            path.AddLines(aptLines);

            path.CloseFigure();

            return path;
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            bounds.Height -= DEF_SELECTION_LINE_WIDTH;
            return bounds;
        }

        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF size = base.GetPreferredSize(g);
            size.Height += DEF_SELECTION_LINE_WIDTH;

            Font inactiveTabFont = null;
            Font activeTabFont = null;

            if (this.TabData != null)
            {
                inactiveTabFont = this.TabData.Font;
            }
            if (inactiveTabFont == null)
            {
                inactiveTabFont = this.GetTabFont(true);
            }
            if (this.panelRenderer != null && this.panelRenderer.TabPanelData != null)
            {
                activeTabFont = this.panelRenderer.TabPanelData.ActiveTabFont;
            }
            if (activeTabFont == null)
            {
                activeTabFont = this.GetTabFont(false);
            }

            if (!this.NeedRotateTextWhenVertical)
            {
                size.Height += this.TabControl.BorderWidth - DEF_DEFAULT_BORDER_WIDTH;

                if (inactiveTabFont.Size >= 12f || activeTabFont.Size >= 12f)
                {
                    size.Width += size.Height / 4;
                }

                int defaultBorderWidth = 5;
                size.Width += parent.BorderWidth - defaultBorderWidth;
            }
            else
            {
                if (inactiveTabFont.Size >= 12f || activeTabFont.Size >= 12f)
                {
                    size.Height += size.Width / 4;
                }
            }

            return size;
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            if (this.IsVistaOS)
            {
                m_borderColor = Color.FromArgb(127, 157, 185);
            }
            else
            {
                m_borderColor = this.IsSelectedState(drawItemInfo.State) || this.IsHotLightState(drawItemInfo.State) ?
                    WindowsXPThemeColors.TabControlAdvActiveBorderColor : WindowsXPThemeColors.TabControlAdvInactiveBorderColor;
            }

            Color lightBorderColor = WindowsXPThemeColors.TabControlAdvLightBorderColor;
            if (this.IsVistaOS)
            {
                lightBorderColor = Color.White;
            }

            RectangleF bounds = drawItemInfo.Bounds;
            Graphics g = drawItemInfo.Graphics;
            int borderWidth = this.TabControl.BorderWidth > 2 ? this.TabControl.BorderWidth : 3;
            bool bIsMirrored = this.NeedMirroredBackground();

            m_panelBounds = TabUtils.ApplyTransform(g, TabAlignment, panelRenderer.Bounds, true);

            if (this.TabAlignment == TabAlignment.Right)
            {
                if (IsSelectedState(drawItemInfo.State))
                {
                    bounds.Height -= TabRendererOffice2003.DEF_BORDER_WIDTH;
                }
            }
            RectangleF curBounds = TabUtils.ApplyTransform(g, this.TabAlignment, bounds, true);

            ApplyTransform(g);
            SaveGraphicsState(g, ref curBounds);

            if (this.IsSelectedState(drawItemInfo.State) && !this.NeedRotateText())
            {
                int adjustVal = 0;
                if (bIsMirrored)
                {
                    adjustVal = 1;
                }

                using (GraphicsPath gp = this.GetTabBorderPath(
                    new RectangleF(m_panelBounds.X, m_panelBounds.Y + 1, m_panelBounds.Width, m_panelBounds.Height),
                    new RectangleF(curBounds.X + adjustVal, curBounds.Y + 1, curBounds.Width - 1, curBounds.Height)))
                using (Pen pen = new Pen(lightBorderColor))
                {
                    g.DrawPath(pen, gp);
                }

                using (GraphicsPath gp = this.GetTabBorderPath(m_panelBounds, curBounds))
                using (Pen pen = new Pen(this.BorderColor))
                {
                    g.DrawPath(pen, gp);
                }
            }
            else
            {
                using (GraphicsPath gp = this.GetBorderPathFromBounds(curBounds))
                using (Pen pen = new Pen(this.BorderColor))
                {
                    g.DrawPath(pen, gp);
                }
            }

            using (Pen pen = new Pen(this.BorderColor))
            {
                float left = m_panelBounds.Left;
                float right = m_panelBounds.Right;
                float bottom = m_panelBounds.Bottom;
                float top = m_panelBounds.Top;

                switch (this.TabAlignment)
                {
                    case TabAlignment.Top:
                    case TabAlignment.Bottom:
                        left = this.TabControl.ClientRectangle.Left;
                        right = this.TabControl.ClientRectangle.Right;
                        break;
                    case TabAlignment.Right:
                    case TabAlignment.Left:
                        left = this.TabControl.ClientRectangle.Top;
                        right = this.TabControl.ClientRectangle.Bottom;
                        break;
                }

                if (this.TabControl.BorderVisible)
                {
                    left += borderWidth;
                    right -= borderWidth;
                }

                if (!this.NeedRotateText())
                {
                    g.DrawLine(pen, left - 1, bottom - 1, right, bottom - 1);
                }
            }

            using (Pen pen = new Pen(lightBorderColor))
            {
                if (!bIsMirrored ||
                    !((this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Width > m_panelBounds.Width)
                    || (!this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Height > m_panelBounds.Width)))
                {
                    g.DrawLine(pen, m_panelBounds.Left + 1, m_panelBounds.Bottom, m_panelBounds.Left + 1, m_panelBounds.Bottom - borderWidth + 2);
                }

                if (bIsMirrored || !((this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Width > m_panelBounds.Width) || (!this.IsVerticalTabAlignment && this.TabControl.ClientRectangle.Height > m_panelBounds.Width)))
                {
                    g.DrawLine(pen, m_panelBounds.Right - 2, m_panelBounds.Bottom, m_panelBounds.Right - 2, m_panelBounds.Bottom - borderWidth + 2);
                }
            }

            RestoreGraphicsState(g);
            g.ResetTransform();
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                Color backColorBottom = Color.Empty;
                Color backColorTop = Color.Empty;

                Graphics g = drawItemInfo.Graphics;
                RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);
                int borderWidth = this.TabControl.BorderWidth > 2 ? this.TabControl.BorderWidth : 3;

                this.ApplyTransform(g);

                base.SaveGraphicsState(g, ref bounds);

                if (this.IsHotLightState(drawItemInfo.State) && !this.IsSelectedState(drawItemInfo.State))
                {
                    if (this.IsVistaOS)
                    {
                        backColorTop = Color.FromArgb(252, 253, 254);
                        backColorBottom = Color.FromArgb(131, 209, 242);
                    }
                    else
                    {
                        backColorTop = WindowsXPThemeColors.TabControlAdvHighLightedTopTabColor;
                        backColorBottom = WindowsXPThemeColors.TabControlAdvHighLightedBottomTabColor;
                    }
                }
                else if (this.IsSelectedState(drawItemInfo.State))
                {
                    if (this.IsVistaOS)
                    {
                        backColorTop = Color.FromArgb(252, 253, 254);
                        backColorBottom = m_tabPropertyExtender.DefaultActiveTabColor(this.panelRenderer.TabPanelData, this.TabControl);
                    }
                    else
                    {
                        backColorTop = WindowsXPThemeColors.TabControlAdvActiveTopTabColor;
                        backColorBottom = WindowsXPThemeColors.TabControlAdvActiveBottomTabColor;
                    }
                }
                else
                {
                    if (this.IsVistaOS)
                    {
                        backColorTop = Color.FromArgb(252, 253, 254);
                        backColorBottom = m_tabPropertyExtender.DefaultInactiveTabColor(this.panelRenderer.TabPanelData, this.TabControl);
                    }
                    else
                    {
                        backColorTop = WindowsXPThemeColors.TabControlAdvInactiveTopTabColor;
                        backColorBottom = WindowsXPThemeColors.TabControlAdvInactiveBottomTabColor;
                    }
                }

                m_borderPathForBackground = true;

                using (GraphicsPath path = this.GetBorderPathFromBounds(bounds))
                using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(bounds, backColorTop, backColorBottom, LinearGradientMode.Vertical))
                {
                    if (this.IsSelectedState(drawItemInfo.State))
                    {
                        Blend blend = new Blend();
                        blend.Positions = new float[] { 0f, 0.6f, 0.6f, 1f };
                        blend.Factors = new float[] { 0f, 0.85f, 0.85f, 1f };

                        backgroundBrush.Blend = blend;
                    }

                    g.FillPath(backgroundBrush, path);
                }

                using (GraphicsPath path = this.GetLowerTabBorderPath())
                using (Brush backgroundBrush = new SolidBrush(backColorBottom))
                {
                    if (!this.NeedRotateText())
                    {
                        g.FillPath(backgroundBrush, path);
                    }
                }

                m_borderPathForBackground = false;

                base.RestoreGraphicsState(g);

                g.ResetTransform();
            }
        }

        protected override void DrawFocusRect(Graphics g, RectangleF focusRect, Color fore, Color back)
        {
            // do nothing here
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

            if (!this.NeedRotateTextWhenVertical)
            {
                int defaultBorderWidth = 5;
                rectTextAndImage.X += (parent.BorderWidth - defaultBorderWidth) / 2;
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

        /// <summary>
        /// Returns the forecolor with which to draw the tab text.
        /// </summary>
        /// <returns>Fore color.</returns>
        protected override Color GetForeColor()
        {
            return Color.Black;
        }

        private bool NeedRotateText()
        {
            return (this.TabAlignment == TabAlignment.Right || this.TabAlignment == TabAlignment.Left)
                    && this.panelRenderer.TabPanelData.RotateTextWhenVertical;
        }

        #endregion
    }
}
