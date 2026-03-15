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
    /// Interactive tool for drawing polylines.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class PolyLineTool
        : PolyLineBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="PolyLineTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PolyLineTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("PolyLineTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction && this.Points.Length >= 2)
            {
                using (Pen pen = new Pen(Color.FromArgb(CommonUsedValues.HALF_OPAQUE, Color.Black)))
                {
                    // draw rendering helper
                    gfx.DrawLines(pen, this.Points);
                }
            }
        }

        /// <summary>
        /// Creates Polyline derived node.
        /// </summary>
        /// <param name="pts">points to create polyline derived node from.</param>
        /// <returns>The node</returns>
        protected override Node CreateNode(PointF[] pts)
        {
            LineBase toReturn = new PolylineNode(pts);

            SetDecorator(toReturn);

            return toReturn;
        }

        /// <summary>
        /// Creates Node's GraphicsPath from given points.
        /// </summary>
        /// <param name="pts">points to create path from</param>
        /// <returns>path created from given points</returns>
        protected override GraphicsPath CreatePath(PointF[] pts)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddLines(pts);

            return path;
        }
        #endregion
    }
}
