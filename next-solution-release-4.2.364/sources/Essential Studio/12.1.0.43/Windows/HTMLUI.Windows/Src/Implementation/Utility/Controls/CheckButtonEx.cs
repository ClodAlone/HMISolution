#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility
{
    /// <summary>
    /// Class to supports CheckButtonEx
    /// </summary>
    [ToolboxItem(false)]
    internal class CheckButtonEx : CheckBox
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the CheckButtonEx class
        /// </summary>
        public CheckButtonEx()
            : base()
        {
            this.CheckAlign = ContentAlignment.MiddleCenter;
            this.Text = string.Empty;
        }
        #endregion

        #region Custom drawing
        /// <summary>
        /// Overridden. Paints check box.
        /// </summary>
        /// <param name="pevent">Event arguments.</param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (this.Focused)
            {
                ControlPaint.DrawFocusRectangle(pevent.Graphics, this.ClientRectangle);
            }
        }
        #endregion
    }
}