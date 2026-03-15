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
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for inserting nodes into a diagram.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.InsertNodeTool"/>
    /// </remarks>
    public class InsertNodeTool
        : RectangleToolBase
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertNodeTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public InsertNodeTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("InsertNodeTool"))
        {
            this.ToolCursor = this.ActionCursor = Cursors.Cross;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The tool.</returns>
        protected override Node CreateNode(RectangleF rectBounding)
        {
            Node nodeToReturn = (Node)this.Controller.NodeToInsert.Clone();
            nodeToReturn.Size = rectBounding.Size;
            nodeToReturn.PinPoint = new PointF(rectBounding.X + rectBounding.Width / 2, rectBounding.Y + rectBounding.Height / 2);
            return nodeToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The rectangle tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            if (this.InAction)
            {
                this.InAction = false;
                this.Controller.Model.HistoryManager.StartAtomicAction("Insert Node");
                Node nodeToInsert = (Node)this.Controller.NodeToInsert.Clone();
                IUnitIndependent node = nodeToInsert;
                SizeF szPinOffset = node.GetPinPointOffset(MeasureUnits.Pixel);
                float dMagnification = this.Controller.View.Magnification / 100f;
                PointF ptPosition = PointF.Empty;
                if ((this.WorkRect.Width == 0 || this.WorkRect.Height == 0) && evtArgs.Button == MouseButtons.Left)
                {
                    ptPosition.X = evtArgs.X - szPinOffset.Width * dMagnification;
                    ptPosition.Y = evtArgs.Y - szPinOffset.Height * dMagnification;
                    // snap to grid
                    ptPosition = this.Controller.View.Grid.GetNearestGridPoint(ptPosition, this.Controller.Viewer.ShowRulers ? this.RulerHeight : 0);
                    ptPosition.X = ptPosition.X + szPinOffset.Width * dMagnification;
                    ptPosition.Y = ptPosition.Y + szPinOffset.Height * dMagnification;
                    ptPosition = this.Controller.ConvertToModelCoordinates(ptPosition);
                }
                else if (this.WorkRect.Width != 0 || this.WorkRect.Height != 0)
                {
                    ptPosition = this.RenderingHelper.PinPoint;
                    nodeToInsert.Size = this.RenderingHelper.Size;
                }
                Matrix mtxScale = this.Controller.Model.DocumentScale.GetScaleTransformation(this.Controller.Model.MeasurementUnits);
                mtxScale.Invert();
                ptPosition = Geometry.AppendMatrix(ptPosition, mtxScale);
                nodeToInsert.PinPoint = ptPosition;
                // insert node
                this.Controller.Model.AppendChild(nodeToInsert);
                this.Controller.Model.HistoryManager.EndAtomicAction();
            }
            return base.ProcessMouseUp(evtArgs);
        }
        #endregion
    }
}