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
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererOffice2003"/>
    /// tab style.
    /// </summary>
    public class StyleRendererPropertyOffice2003 : OneNoteStyleRendererProperty
    {
        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 3;
        #endregion

        #region Class overrides
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            // Do nothing here. Office2003 Background is drawn by TabControl.
        }

        /// <summary>
        /// Returns the size by which the selected tab overlaps the inactive tabs.
        /// </summary>
        /// <param name="tabSize">Tab Size</param>
        /// <returns>Returns Overlap size</returns>
        public override SizeF GetOverlapSize(SizeF tabSize)
        {
            SizeF overlapSize = base.GetOverlapSize(tabSize);
            overlapSize.Height = DEF_OVERLAP_HEIGHT;
            return overlapSize;
        }
        #endregion
    }
}