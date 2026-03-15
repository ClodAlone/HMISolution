#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// Collection of toolstrips.
    /// </summary>
    public class ToolStripsCollection
        : ArrayList
    {
        #region Overrides
        public override int Add(object value)
        {
            int result = -1;

            ToolStrip toolStrip = value as ToolStrip;
            if (toolStrip != null)
            {
                result = base.Add(toolStrip);
                OnToolStripAdded(toolStrip);
            }
            else throw new ArgumentException("ToolStrip is expected");

            return result;
        }

        protected virtual void OnToolStripAdded(ToolStrip toolStrip)
        {
            toolStrip.Disposed += new EventHandler(ToolStrip_Disposed);

            if (ToolStripAdded != null)
            {
                ToolStripAdded(this, new ToolStripEventArgs(toolStrip));
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Raised when new toolstrip is added.
        /// </summary>
        public event ToolStripEventHandler ToolStripAdded;

        /// <summary>
        /// Raised when disposed toolstrip is removed.
        /// </summary>
        public event ToolStripEventHandler ToolStripRemoved;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Removes disposed control from collection.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        public void ToolStrip_Disposed(object sender, EventArgs e)
        {
            ToolStrip toolStrip = (ToolStrip)sender;

            Remove(toolStrip);
            toolStrip.Disposed -= new EventHandler(ToolStrip_Disposed);

            if (ToolStripRemoved != null)
            {
                ToolStripRemoved(this, new ToolStripEventArgs(toolStrip));
            }
        }
        #endregion
    }
}
#endif