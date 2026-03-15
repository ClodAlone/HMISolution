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
    /// Interactive tool for drawing ellipses.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.RectangleToolBase"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Ellipse"/>
    /// </remarks>
    public class EllipseTool
        : RectangleToolBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="EllipseTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public EllipseTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("EllipseTool"))
        {
            this.ToolCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Complete the action.
        /// </summary>
        /// <param name="rectBounding">The rectangle bounding.</param>
        /// <returns>The node.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            return new Ellipse(rectBounding);
        }
        #endregion
    }
}