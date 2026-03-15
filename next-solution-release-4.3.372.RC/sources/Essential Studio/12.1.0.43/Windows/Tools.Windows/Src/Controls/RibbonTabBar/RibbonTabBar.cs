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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar.Design;
using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar
{
    /// <summary>
    /// RibbonTab Bar
    /// </summary>
    [Designer(typeof(RibbonTabBarDesigner))]
    [ToolboxItem(false)]
    public partial class RibbonTabBar
        : Syncfusion.Windows.Forms.Tools.RibbonTabControl
    {
        #region Properties
        /// <summary>
        /// Gets left panel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStrip LeftPanel
        {
            get
            {
                return ((RibbonTabBarHeader)this.Header).LeftPanel;
            }
        }

        /// <summary>
        /// Gets top panel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ToolStrip TopPanel
        {
            get
            {
                return ((RibbonTabBarHeader)this.Header).TopPanel;
            }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the RibbonTabBar class.
        /// </summary>
        public RibbonTabBar()
        {
            InitializeComponent();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Retrieves header for the control.
        /// </summary>
        /// <returns>IRibbonHeaderControl instance.</returns>
        protected override IRibbonHeaderControl GetHeader()
        {
            return new RibbonTabBarHeader();
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when tab is about to be changed.
        /// </summary>
        public new event RibbonTabItemChangingEventHandler TabChanging
        {
            add
            {
                this.Header.TabChanging += value;
            }
            remove
            {
                this.Header.TabChanging -= value;
            }
        }

        /// <summary>
        /// Raised when tab is changed.
        /// </summary>
        public new event RibbonTabItemChangedEventHandler TabChanged
        {
            add
            {
                this.Header.TabChanged += value;
            }
            remove
            {
                this.Header.TabChanged -= value;
            }
        }
        #endregion
    }
}
#endif
