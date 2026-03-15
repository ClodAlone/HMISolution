#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for free-hand drawing.
    /// </summary>
    public class PencilTool : PencilBase
    {
        #region Initialize/Finalize Methods
        public PencilTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("PencilTool"))
        {
            this.ToolCursor = this.ActionCursor = Resources.Cursors.Pencil;
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// Draw the Specified range of Points into Line.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(System.Drawing.Graphics gfx)
        {
            Pen pen = new Pen(Color.Black);
            gfx.DrawLines(pen, this.Points);
        }
        #endregion
    }
}
