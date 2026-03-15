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
using System.ComponentModel.Design;
using System.Text;
using Syncfusion.Windows.Forms.Tools.Design;

namespace Syncfusion.Windows.Forms.Tools.Controls.RibbonTabControl.Interfaces
{
    /// <summary>
    /// Interface for designer of header control.
    /// </summary>
    public interface IHeaderDesigner
    {
        #region Methods
        /// <summary>
        /// Retrieves designer of part of header control that manages tabs.
        /// </summary>
        /// <returns>Designer of part of header control that manages tabs.</returns>
        RibbonHeaderControlDesigner GetTabHeaderDesigner();
        #endregion
    }
}
#endif
