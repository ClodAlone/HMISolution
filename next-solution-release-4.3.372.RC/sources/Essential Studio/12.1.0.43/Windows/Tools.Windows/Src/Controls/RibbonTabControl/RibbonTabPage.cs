#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Control representing one tab page.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(typeof(RibbonTabPageDesigner))]
    public partial class RibbonTabPage
        : Panel
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the RibbonTabPage class.
        /// </summary>
        public RibbonTabPage()
        {
        }
        #endregion
    }
}
#endif
