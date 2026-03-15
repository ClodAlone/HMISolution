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
    /// Interactive tool for drawing curves.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.CurveNode"/>
    /// </remarks>
    public class CurveTool
        : PolyLineBase
    {
        #region Class initlalize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CurveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public CurveTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("CurveTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates Polyline derived node.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The curve tool.</returns>
        protected override Node CreateNode(PointF[] pts)
        {
            return new CurveNode(pts);
        }

        /// <summary>
        /// Creates Node's GraphicsPath from given points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected override GraphicsPath CreatePath(PointF[] pts)
        {
            return PathFactory.CreateCurve(pts);
        }
        #endregion
    }
}
