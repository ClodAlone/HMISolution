#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for drawing closed curves.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.ClosedCurveNode"/>
    /// </remarks>
    public class ClosedCurveTool
        : PolyLineBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedCurveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public ClosedCurveTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("ClosedCurveTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates Node's GraphicsPath from given points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected override GraphicsPath CreatePath(PointF[] pts)
        {
            GraphicsPath pathToReturn = null;

            if (pts.Length == 2 && (pts[0] != pts[1]))
            {
                pathToReturn = PathFactory.CreateLine(pts[0], pts[1]);
            }
            else if (pts.Length > 2)
            {
                pathToReturn = PathFactory.CreateClosedCurve(pts);
            }

            return pathToReturn;
        }

        /// <summary>
        /// Creates Polyline derived node.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The closed curve node.</returns>
        protected override Node CreateNode(PointF[] pts)
        {
            return new ClosedCurveNode(pts);
        }
        #endregion
    }
}
