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
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    #region EventArgs Descendants
    /// <summary>
    /// Event arguments for ToolStripEventHandler
    /// </summary>
    public class ToolStripEventArgs
        : EventArgs
    {
        #region Public Fields
        /// <summary>
        /// Underlying ToolStrip.
        /// </summary>
        public ToolStrip ToolStrip;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the ToolStripEventArgs class.
        /// </summary>
        /// <param name="toolStrip">Underlying ToolStrip.</param>
        public ToolStripEventArgs(ToolStrip toolStrip)
        {
            this.ToolStrip = toolStrip;
        }
        #endregion
    }
    #endregion

    #region Delegates
    /// <summary>
    /// Delegate for events related to ToolStrips.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="args"> EventArgs that contains the event data.</param>
    public delegate void ToolStripEventHandler(object sender, ToolStripEventArgs args);
    #endregion
}
#endif
