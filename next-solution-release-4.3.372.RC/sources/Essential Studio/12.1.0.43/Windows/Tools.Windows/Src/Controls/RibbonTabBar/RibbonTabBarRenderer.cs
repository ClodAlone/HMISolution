#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar
{
    /// <summary>
    /// Renderer for RibbonTabBar.
    /// </summary>
   public class RibbonTabBarRenderer
        : Office12ToolStripRenderer
    {
        #region Static Fields
        /// <summary>
        /// Blend for painting gradient background.
        /// </summary>
        private static Blend m_backgroundBlend = new Blend();
        #endregion

        #region Initialization
    
        static RibbonTabBarRenderer()
        {
            m_backgroundBlend.Positions = new float[] { 0.0F, 0.5F, 1.0F };
            m_backgroundBlend.Factors = new float[] { 0.0F, 1.0F, 0.0F };
        }
        #endregion

        #region Overrides
      
        protected override bool HasCaption(System.Windows.Forms.ToolStrip toolStrip)
        {
            return false;
        }
        protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            if (!SystemInfo.IsVisualStyleEnabled || !PaintToolStripBackground(e))
            {
                base.OnRenderToolStripBackground(e);
            }
        }
        #endregion

        #region Private Methods
     
        private bool PaintToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            Color clBegin = this.ColorTable.MenuStripGradientBegin;
            Color clEnd = this.ColorTable.MenuStripGradientEnd;
            Size szParent = e.ToolStrip.Parent.ClientSize;

            if (szParent.Width > 0 && szParent.Height > 0)
            {
                Rectangle rcParent = new Rectangle(Point.Empty, szParent);

                using (LinearGradientBrush brush = new LinearGradientBrush(rcParent, clBegin, clEnd, LinearGradientMode.Horizontal))
                {
                    brush.TranslateTransform(szParent.Width - e.ToolStrip.Location.X, szParent.Height - e.ToolStrip.Location.Y, MatrixOrder.Append);
                    brush.Blend = m_backgroundBlend;
                    e.Graphics.FillRectangle(brush, e.ToolStrip.ClientRectangle);

                    // InvalidateInnerControls();
                }
            }

            return true;
        }
        #endregion
    }
}
#endif
