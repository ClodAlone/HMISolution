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
    /// Interactive tool for drawing rectangles.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RectangleToolBase"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Rectangle"/>
    /// </remarks>
    public class RectangleTool
        : RectangleToolBase
    {
        #region Class initialize/finalize members
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public RectangleTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("RectangleTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The rectangle node.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            return new Rectangle(rectBounding);
        }
        #endregion
    }
}