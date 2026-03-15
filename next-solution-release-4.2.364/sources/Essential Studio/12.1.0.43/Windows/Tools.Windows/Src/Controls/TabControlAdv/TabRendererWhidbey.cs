#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
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
    /// This renderer allows user to draw TabPages with Office2003 style.
    /// </summary>
    public class TabRendererWhidbey : OneNoteStyleRenderer
    {
        #region Class constants
       
        /// <summary>
        /// Unique renderer name.
        /// </summary>
        private const string DEF_RENDERER_NAME = "VS2005Style";
        
        /// <summary>
        /// Selected item line width.
        /// </summary>
        protected internal const int DEF_SELECTION_LINE_WIDTH = 2;
        private const int DEF_CLOSE_BUTTON_PADDING = 3;

        /// <summary>
        /// Selected item border color.
        /// </summary>
        protected internal static readonly Color DEF_SELECTED_BORDER_COLOR = Color.FromArgb(127, 157, 185);
        #endregion

        #region Class members
      
        /// <summary>
        /// Color to draw tabs borders with.
        /// </summary>
        private Color m_borderColor = m_tabPropertyExtender.DefaultBorderColor;
        
        /// <summary>
        /// Use TabPanelPropertyExtender property as my default properties provider.
        /// </summary>
        private static StyleRendererPropertyWhidbey m_tabPropertyExtender;
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
        public static new StyleRendererPropertyWhidbey TabPanelPropertyExtender
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
                return DEF_SELECTED_BORDER_COLOR;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        static TabRendererWhidbey()
        {
            m_tabPropertyExtender = new StyleRendererPropertyWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererWhidbey), TabPanelPropertyExtender);
        }

        public static new void RegisterTabType()
        {
            m_tabPropertyExtender = new StyleRendererPropertyWhidbey();
            TabRendererFactory.RegisterTabType(TabStyleName, typeof(TabRendererWhidbey), TabPanelPropertyExtender);
        }

        /// <summary>
        /// Initializes a new instance of the TabRendererWhidbey class.
        /// </summary>
        /// <param name="parent">The tab control parent.</param>
        /// <param name="panelRenderer">The <see cref="Syncfusion.Windows.Forms.Tools.ITabPanelRenderer"/> parent.</param>
        public TabRendererWhidbey(ITabControl parent, ITabPanelRenderer panelRenderer) : base(parent, panelRenderer)
        {
        }
        #endregion

        #region Class overrides
        public override int CloseButtonPadding
        {
            get
            {
                return DEF_CLOSE_BUTTON_PADDING;
            }
        }

        protected override RectangleF CorrectBounds(RectangleF bounds)
        {
            return bounds;
        }

        protected override Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
        }

        protected override bool ShouldDrawHighLightUpper
        {
            get
            {
                return false;
            }
        }

        public override SizeF GetPreferredSize(Graphics g)
        {
            SizeF size = base.GetPreferredSize(g);
            size.Height += DEF_SELECTION_LINE_WIDTH;
            return size;
        }

        protected override void DrawBorders(DrawTabEventArgs drawItemInfo)
        {
            m_borderColor = this.IsSelectedState(drawItemInfo.State) ? DEF_SELECTED_BORDER_COLOR : m_tabPropertyExtender.DefaultBorderColor;

            if (this.TabAlignment == TabAlignment.Right)
            {
                if (IsSelectedState(drawItemInfo.State))
                {
                    RectangleF bounds = drawItemInfo.Bounds;
                    bounds.Height -= TabRendererOffice2003.DEF_BORDER_WIDTH;
                }
            }
            base.DrawBorders(drawItemInfo);
        }

        protected override void DrawBackground(DrawTabEventArgs drawItemInfo)
        {
            if (drawItemInfo.Bounds.Width > 0 && drawItemInfo.Bounds.Height > 0)
            {
                Graphics g = drawItemInfo.Graphics;

                RectangleF bounds = TabUtils.ApplyTransform(g, this.TabAlignment, drawItemInfo.Bounds, true);

                // Make g horizontal
                this.ApplyTransform(g);

                base.SaveGraphicsState(g, ref bounds);

                // Get the border path and fill it.
                using (GraphicsPath path = this.GetBorderPathFromBounds(bounds))
                {
                    Color backColorBottom = Color.Empty;
                    Color backColorTop = Color.Empty;

                    // Get colors for seleced item
                    if (this.IsSelectedState(drawItemInfo.State))
                    {
                        backColorBottom = m_tabPropertyExtender.DefaultActiveTabColor(this.panelRenderer.TabPanelData, this.TabControl);
                        backColorTop = backColorBottom;
                    }
                    else
                    {
                        backColorBottom = m_tabPropertyExtender.DefaultInactiveTabColor(this.panelRenderer.TabPanelData, this.TabControl);
                        backColorTop = ControlPaint.LightLight(backColorBottom);
                    }

                    using (Brush backgroundBrush = new LinearGradientBrush(bounds, backColorTop, backColorBottom, LinearGradientMode.Vertical))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.FillPath(backgroundBrush, path);
                    }
                }

                base.RestoreGraphicsState(g);

                g.ResetTransform();
            }
        }

        #endregion
    }
}
