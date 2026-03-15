#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Round Rectangle Tool.
    /// </summary>
    public class RoundRectTool
        : RectangleToolBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRectTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public RoundRectTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("RoundRectTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The round rectangle node.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            PointF[] pts = new PointF[2]
            {
                new PointF( rectBounding.Left, rectBounding.Top ),
                new PointF( rectBounding.Right, rectBounding.Bottom ) 
            };

            return new RoundRect(pts);
        }
        #endregion
    }
}
