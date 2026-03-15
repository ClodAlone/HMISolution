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
    /// Represents the default tab properties for the <see cref="Syncfusion.Windows.Forms.Tools.TabRendererOffice2003"/>
    /// tab style.
    /// </summary>
    public class StyleRendererPropertyWhidbey : OneNoteStyleRendererProperty
    {
        #region Class constants
        /// <summary>
        /// Space between top border and panel.
        /// </summary>
        private const int DEF_OVERLAP_HEIGHT = 6;
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the default items border color.
        /// </summary>
        public virtual Color DefaultBorderColor
        {
            get
            {
                return SystemColors.ControlDark;
            }
        }
        public override void OnPaintPanelBackground(ITabControl tabControl, Graphics g, Color bgColor, Rectangle bounds)
        {
            // Do nothing here. VS2005 style has no background.
        }

        public override Color DefaultTabPanelBackgroundColor(ITabPanelData panelData, ITabControl tabControl)
        {
            if (panelData.BackColor == Color.Empty)
                return SystemColors.Control;
            else
                return panelData.BackColor;
        }

        public override Color DefaultInactiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            if(panelData.InactiveTabColor == Color.Empty)
                return SystemColors.Control;
            else
                return panelData.InactiveTabColor;
        }

        public override Color DefaultActiveTabColor(ITabPanelData panelData, ITabControl tabControl)
        {
            if (panelData.ActiveTabColor == Color.Empty)
                return SystemColors.Control;
            else
                return panelData.ActiveTabColor;
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
}