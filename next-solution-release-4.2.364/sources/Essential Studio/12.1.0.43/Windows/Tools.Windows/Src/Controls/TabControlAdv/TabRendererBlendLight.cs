#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.BlendDarkRendererProperty"/>
    /// tab style.
    /// </summary>
    public class BlendLightRendererProperty : TabUIDefaultProperties
    {
        internal static Color BackgroundColor
        {
            get
            {
                Color backGround = Color.FromArgb(112, 112, 112);


                return backGround;
            }
        }

        public override bool DrawLeftToRight
        {
            get { return false; }
        }

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(141, 141, 141);
        }

        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(180, 180, 180);
        }

        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.FromArgb(112, 112, 112);
        }
        public override Color DefaultTabForeColor(ITabPanelData panelData, ITabControl tabControl)
        {
            return Color.Black;
        }







    }

    public class TabRendererBlendLight : TabRendererBase
    {
        private const int c_iCloseButtonPaddingThemes = 8;
        private const int c_iCloseButtonPadding = 5;
        private const int c_iCorrectCloseButtonX = -3;

        static BlendLightRendererProperty tabPanelPropertyExtender;
        /// <summary>
        /// Returns the unique name of this tab renderer.
        /// </summary>
        public static string TabStyleName
        {
            get
            {
                return "BlendLight";
            }
        }
        /// <summary>
        /// Returns the <see cref="Syncfusion.Windows.Forms.Tools.ITabDefaultProperties"/>
        /// instance that provides default properties for this renderer.
        /// </summary>
        public static BlendLightRendererProperty TabPanelPropertyExtender
        {
            get { return tabPanelPropertyExtender; }
        }
        static TabRendererBlendLight()
        {
            tabPanelPropertyExtender = new BlendLightRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererBlendLight), TabPanelPropertyExtender);
        }

        public static void RegisterTabType()
        {
            tabPanelPropertyExtender = new BlendLightRendererProperty();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererBlendLight), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Creates a new instance of the TabRendererBlendLight class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererBlendLight(ITabControl parent, ITabPanelRenderer panelRenderer)
            : base(parent, panelRenderer)
        {
        }
        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBackground"/>.
        /// </summary>
        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {


            // Take a look at TabControlAdv.Init for notes on why we need this check.
            bool bIsSelected = ((drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected);
            bool bIsHotLight = ((drawItemInfo.State & DrawItemState.HotLight) == DrawItemState.HotLight);

            if (bIsSelected)
            {
                DrawSelectedBackground(drawItemInfo);
            }
            else
            {
                DrawUnSelectedBackground(drawItemInfo);

            }

            if (bIsHotLight)
            {
                DrawHotLightBackground(drawItemInfo);
            }



        }
        private void DrawUnSelectedBackground(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;
            if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Top)
            {

                PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top+6),
                    new PointF(drawItemInfo.Bounds.Left+1,drawItemInfo.Bounds.Top+5),
                    new PointF(drawItemInfo.Bounds.Left+2,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Right-3,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Top+5),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top+6),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1)
                };

                g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);
                drawItemInfo.ForeColor = Color.Black;

            }
            else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Left)
            {
                PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-3),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Top+1),
                    new PointF(drawItemInfo.Bounds.Left+6 ,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+6,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-3)
                };
                g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);
                drawItemInfo.ForeColor = Color.Black;


            }
            else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Bottom)
            {
                PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-6),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right ,drawItemInfo.Bounds.Bottom-6),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Bottom-5),
                    new PointF(drawItemInfo.Bounds.Right-3,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Bottom-3),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Bottom-3),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Left+2,drawItemInfo.Bounds.Bottom-5),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-6)
                };
                g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);
                drawItemInfo.ForeColor = Color.Black;
            }
            else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Right)
            {
                PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-7,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-5,drawItemInfo.Bounds.Top+2),
                    new PointF(drawItemInfo.Bounds.Right-3 ,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Right-3,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-5,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Right-7,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top)
                };
                g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);
                drawItemInfo.ForeColor = Color.Black;
            }
        }
        private void DrawSelectedBackground(DrawTabEventArgs drawItemInfo)
        {
            if (this.panelRenderer.TabPanelBackColor != drawItemInfo.BackColor
                  || !this.panelRenderer.IsBackgroundSolid())
            {
                Graphics g = drawItemInfo.Graphics;
                if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Top)
                {

                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top+6),
                    new PointF(drawItemInfo.Bounds.Left+1,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Left+3 ,drawItemInfo.Bounds.Top+2),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Top+2),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top+6),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1)

                };

                    g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);


                }
                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Left)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-3),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Top+1),
                    new PointF(drawItemInfo.Bounds.Left+6 ,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+6,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-3)
                };
                    g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);



                }
                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Bottom)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-6),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right ,drawItemInfo.Bounds.Bottom-6),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left+2,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-6)
                };
                    g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);

                }
                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Right)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-7,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-5,drawItemInfo.Bounds.Top+2),
                    new PointF(drawItemInfo.Bounds.Right-3 ,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Right-3,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-5,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Right-7,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top)
                };
                    g.FillPolygon(new SolidBrush(drawItemInfo.BackColor), pts);

                }
            }


        }
        private void DrawHotLightBackground(DrawTabEventArgs drawItemInfo)
        {
            if (!((drawItemInfo.State & DrawItemState.Selected) == DrawItemState.Selected))
            {
                Graphics g = drawItemInfo.Graphics;
                if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Top)
                {

                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Left+1,drawItemInfo.Bounds.Top+1),
                    new PointF(drawItemInfo.Bounds.Left+3 ,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Top+1),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-1)

                };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(166, 166, 166)), pts);
                    drawItemInfo.ForeColor = Color.Black;

                }

                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Left)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-3),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top+3),
                    new PointF(drawItemInfo.Bounds.Left+1,drawItemInfo.Bounds.Top+1),
                    new PointF(drawItemInfo.Bounds.Left+3 ,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-1,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+3,drawItemInfo.Bounds.Bottom-1),
                    new PointF(drawItemInfo.Bounds.Left+1,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-3)
                };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(166, 166, 166)), pts);
                    drawItemInfo.ForeColor = Color.Black;


                }
                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Bottom)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right ,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left+4,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left+2,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom-4)
                };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(166, 166, 166)), pts);
                    drawItemInfo.ForeColor = Color.Black;

                }
                else if (this.panelRenderer.TabPanelData.Alignment == TabAlignment.Right)
                {
                    PointF[] pts = new PointF[]
                {
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Top),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Top+2),
                    new PointF(drawItemInfo.Bounds.Right ,drawItemInfo.Bounds.Top+4),
                    new PointF(drawItemInfo.Bounds.Right,drawItemInfo.Bounds.Bottom-4),
                    new PointF(drawItemInfo.Bounds.Right-2,drawItemInfo.Bounds.Bottom-2),
                    new PointF(drawItemInfo.Bounds.Right-4,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Bottom),
                    new PointF(drawItemInfo.Bounds.Left,drawItemInfo.Bounds.Top)
                };
                    g.FillPolygon(new SolidBrush(Color.FromArgb(166, 166, 166)), pts);
                    drawItemInfo.ForeColor = Color.Black;

                }
            }





        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawBorders"/>.
        /// </summary>
        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
        }

        /// <summary>
        /// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.TabRendererBase.DrawInterior"/>.
        /// </summary>
        protected override void DrawInterior(DrawTabEventArgs drawItemInfo)
        {
            Graphics g = drawItemInfo.Graphics;

            // Convert to horizontal co-ords
            RectangleF rectTextAndImage = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.BoundsInterior, true);
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
}
