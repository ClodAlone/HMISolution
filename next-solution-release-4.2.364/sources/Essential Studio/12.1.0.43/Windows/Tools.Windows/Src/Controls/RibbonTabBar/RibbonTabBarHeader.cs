#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar.Design;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar
{
    /// <summary>
    /// RibbonTabBar Header
    /// </summary>
    [Designer(typeof(RibbonTabBarHeaderDesigner))]
    [ToolboxItem(false)]
    public partial class RibbonTabBarHeader
        : UserControl, IRibbonHeaderControl
    {
        #region Internal Properties
        /// <summary>
        /// Gets ribbon header control.
        /// </summary>
        protected internal RibbonHeaderControl TabHeaderControl
        {
            get
            {
                return ribbonHeader;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets left panel.
        /// </summary>
        public ToolStrip LeftPanel
        {
            get
            {
                return pnlLeft;
            }
        }

        /// <summary>
        /// Gets top panel.
        /// </summary>
        public ToolStrip TopPanel
        {
            get
            {
                return pnlTop;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the RibbonTabBarHeader class.
        /// </summary>
        public RibbonTabBarHeader()
        {
            InitializeComponent();

            pnlLeft.Renderer = new RibbonTabBarRenderer();
            pnlTop.Renderer = new RibbonTabBarRenderer();
        }
        #endregion

        #region IRibbonHeaderControl Implementation
        /// <summary>
        /// Gets collection of tab groups.
        /// </summary>
        public TabGroupsCollection Groups
        {
            get
            {
                return ribbonHeader.Groups;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether with of control should be filled with RibbonTabItems.
        /// </summary>
        public bool FillWidthWithItems
        {
            get
            {
                return ribbonHeader.FillWidthWithItems;
            }
            set
            {
                ribbonHeader.FillWidthWithItems = value;
            }
        }

        /// <summary>
        /// Adds new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab group.</returns>
        public RibbonTabGroup AddGroup()
        {
            return ribbonHeader.AddGroup();
        }

        /// <summary>
        /// Adds ribbon tab group to the control.
        /// </summary>
        /// <param name="group">RibbonTabGroup instance to be added.</param>
        /// <returns>Added RibbonTabGroup instance.</returns>
        public RibbonTabGroup AddGroup(RibbonTabGroup group)
        {
            return ribbonHeader.AddGroup(group);
        }

        /// <summary>
        /// Adds new ribbon tab item to the new ribbon tab group.
        /// </summary>
        /// <returns>Newly created tab item.</returns>
        public RibbonTabItem AddTabItem()
        {
            return ribbonHeader.AddTabItem();
        }

        /// <summary>
        /// Adds toolstrip item to the newly created tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        public ToolStripItem AddItem(ToolStripItem item)
        {
            return ribbonHeader.AddItem(item);
        }

        /// <summary>
        /// Adds new ribbon tab item to the specified tab group.
        /// </summary>
        /// <param name="group">RibbonTabGroup that has to host new item.</param>
        /// <returns>Newly created RibbonTabItem.</returns>
        public RibbonTabItem AddTabItem(RibbonTabGroup group)
        {
            return ribbonHeader.AddTabItem(group);
        }

        /// <summary>
        /// Adds toolstrip item to the specified tab group.
        /// </summary>
        /// <param name="item">ToolStripItem instance to be added.</param>
        /// <param name="group">RibbonTabGroup that has to host the item.</param>
        /// <returns>Added ToolStripItem instance.</returns>
        public ToolStripItem AddItem(ToolStripItem item, RibbonTabGroup group)
        {
            return ribbonHeader.AddItem(item, group);
        }

        /// <summary>
        /// Raised when newly created tab item is added to the control.
        /// </summary>
        public event RibbonTabItemEventHandler RibbonTabItemAdded
        {
            add
            {
                ribbonHeader.RibbonTabItemAdded += value;
            }
            remove
            {
                ribbonHeader.RibbonTabItemAdded -= value;
            }
        }

        /// <summary>
        /// Raised when tab is about to be changed.
        /// </summary>
        public event RibbonTabItemChangingEventHandler TabChanging
        {
            add
            {
                ribbonHeader.TabChanging += value;
            }
            remove
            {
                ribbonHeader.TabChanging -= value;
            }
        }

        /// <summary>
        /// Raised when tab is changed.
        /// </summary>
        public event RibbonTabItemChangedEventHandler TabChanged
        {
            add
            {
                ribbonHeader.TabChanged += value;
            }
            remove
            {
                ribbonHeader.TabChanged -= value;
            }
        }

        /// <summary>
        /// Returns control that has to be shown behind transparent childs.
        /// </summary>
        /// <returns>Required control.</returns>
        public Control GetParentForChildsTransparentRendering()
        {
            return this;
        }

        /// <summary>
        /// Returns header for tab control.
        /// </summary>
        /// <returns>Required tab header.</returns>
        public Control GetTabsHeader()
        {
            return ribbonHeader;
        }

        /// <summary>
        /// Gets or sets the first color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor1
        {
            get
            {
                return ribbonHeader.GroupsCaptionColor1;
            }
            set
            {
                ribbonHeader.GroupsCaptionColor1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the second color of caption gradiend.
        /// </summary>
        public Color GroupsCaptionColor2
        {
            get
            {
                return ribbonHeader.GroupsCaptionColor2;
            }
            set
            {
                ribbonHeader.GroupsCaptionColor2 = value;
            }
        }

        /// <summary>
        /// Gets or sets color of caption font.
        /// </summary>
        public Color GroupsCaptionFontColor
        {
            get
            {
                return ribbonHeader.GroupsCaptionFontColor;
            }
            set
            {
                ribbonHeader.GroupsCaptionFontColor = value;
            }
        }
        #endregion
    }
}