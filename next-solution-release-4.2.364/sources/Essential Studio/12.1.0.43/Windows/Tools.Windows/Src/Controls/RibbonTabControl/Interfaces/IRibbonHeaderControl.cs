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
using System.Text;
using System.Windows.Forms;

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces
{
    /// <summary>
    /// Interface for RibbonHeaderControl.
    /// </summary>
    public interface IRibbonHeaderControl
    {
        #region Properties
        /// <summary>
        /// Gets collection of tab groups.
        /// </summary>
        TabGroupsCollection Groups { get; }

        /// <summary>
        /// Gets or sets a value indicating whether with of control should be filled with RibbonTabItems.
        /// </summary>
        bool FillWidthWithItems { get; set; }

        /// <summary>
        /// Gets or sets the first color of caption gradiend.
        /// </summary>
        Color GroupsCaptionColor1 { get; set; }

        /// <summary>
        /// Gets or sets the second color of caption gradiend.
        /// </summary>
        Color GroupsCaptionColor2 { get; set; }

        /// <summary>
        /// Gets or sets color of caption font.
        /// </summary>
        Color GroupsCaptionFontColor { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab group.</returns>
        RibbonTabGroup AddGroup();

        /// <summary>
        /// Adds ribbon tab group to the control.
        /// </summary>
        /// <param name="group">RibbonTabGroup instance to be added.</param>
        /// <returns>Added RibbonTabGroup instance.</returns>
        RibbonTabGroup AddGroup(RibbonTabGroup group);

        /// <summary>
        /// Adds new ribbon tab item to the new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab item.</returns>
        RibbonTabItem AddTabItem();

        /// <summary>
        /// Adds toolstrip item to the newly created tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        ToolStripItem AddItem(ToolStripItem item);

        /// <summary>
        /// Adds new ribbon tab item to the specified tab group.
        /// </summary>
        /// <param name="group">RibbonTabGroup that has to host new item.</param>
        /// <returns>Newly created RibbonTabItem.</returns>
        RibbonTabItem AddTabItem(RibbonTabGroup group);

        /// <summary>
        /// Adds toolstrip item to the specified tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <param name="group">RibbonTabGroup that has to host the item.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        ToolStripItem AddItem(ToolStripItem item, RibbonTabGroup group);

        /// <summary>
        /// Returns control that has to be shown behind transparent childs.
        /// </summary>
        /// <returns>Required control.</returns>
        Control GetParentForChildsTransparentRendering();

        /// <summary>
        /// Returns header for tab control.
        /// </summary>
        /// <returns>Required tab header.</returns>
        Control GetTabsHeader();
        #endregion

        #region Events
        /// <summary>
        /// Raised when newly created tab item is added to the control.
        /// </summary>
        event RibbonTabItemEventHandler RibbonTabItemAdded;

        /// <summary>
        /// Raised when tab is about to be changed.
        /// </summary>
        event RibbonTabItemChangingEventHandler TabChanging;

        /// <summary>
        /// Raised when tab is changed.
        /// </summary>
        event RibbonTabItemChangedEventHandler TabChanged;
        #endregion
    }
}
#endif
