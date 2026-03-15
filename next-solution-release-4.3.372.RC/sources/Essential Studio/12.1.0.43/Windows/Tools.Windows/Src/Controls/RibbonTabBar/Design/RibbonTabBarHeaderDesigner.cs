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
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabBar.Design
{
    /// <summary>
    /// Designer for RibbonTabBarHeader.
    /// </summary>
   public class RibbonTabBarHeaderDesigner
        : ParentControlDesigner, IHeaderDesigner
    {
        #region Fields
        /// <summary>
        /// Design time RibbonHeaderControl instance.
        /// </summary>
        private RibbonTabBarHeader m_control;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of RibbonHeaderControlDesigner.
        /// </summary>
        /// <param name="component">Component param</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            m_control = (RibbonTabBarHeader)Component;

            EnableDesignMode(m_control.TabHeaderControl, "TabHeaderControl");
            EnableDesignMode(m_control.LeftPanel, "LeftPanel");
            EnableDesignMode(m_control.TopPanel, "TopPanel");
        }
        #endregion

        #region IHeaderDesigner Implementation
        /// <summary>
        /// Retrieves designer of part of header control that manages tabs.
        /// </summary>
        /// <returns>Designer of part of header control that manages tabs.</returns>
        public RibbonHeaderControlDesigner GetTabHeaderDesigner()
        {
            IDesignerHost tabHaderDesignerHost = ((Control)m_control.TabHeaderControl).Site.GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (tabHaderDesignerHost != null)
            {
                return tabHaderDesignerHost.GetDesigner((Control)m_control.TabHeaderControl) as RibbonHeaderControlDesigner;
            }

            return null;
        }
        #endregion
    }
}
#endif
