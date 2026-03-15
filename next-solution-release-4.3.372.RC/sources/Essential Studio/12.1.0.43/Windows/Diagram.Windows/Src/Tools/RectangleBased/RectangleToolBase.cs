#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Base class for tools that draw tracking rectangles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a base implementation for tools that require the
    /// user to draw a rectangle. When a mouse down event is received, the
    /// point at which the event occurred becomes the first point in the
    /// rectangle. As the mouse moves, the rectangle is tracked.
    /// </para>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public abstract class RectangleToolBase
        : UITool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleToolBase"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The name.</param>
        public RectangleToolBase(DiagramController controller, string name)
            : base(controller, name)
        { 
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the rendering helper node created from original.
        /// </summary>
        /// <value>The rendering helper.</value>
        protected Node RenderingHelper
        {
            get
            {
                Node nodeRenderingHelper = null;
                RectangleF rectTemp = CreateNodeBoundingRect();

                if (rectTemp.Size.Width != 0 && rectTemp.Height != 0)
                {
                    nodeRenderingHelper = CreateNode(rectTemp);
                    AlterStyle(nodeRenderingHelper);
                }

                return nodeRenderingHelper;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The rectangle tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            return base.ProcessMouseDown(evtArgs);
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The rectangle tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            this.CanRender = false;
            this.CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            if (this.InAction)
            {
                // create current work rect
                System.Drawing.Rectangle rectTemp = Geometry.ConvertRectangle(GetFrameRectangle(true));

                if (this.WorkRect != rectTemp)
                {
                    // update work rects
                    this.WorkRectPrev = this.WorkRect;
                    this.WorkRect = rectTemp;
                    this.CanRender = true;
                }
                if (!(this is BitmapTool))
                {
                    PathNode node = this.RenderingHelper as PathNode;
                    if (node != null)
                        foreach (Label label in node.Labels)
                        {
                            if (label.Position == Position.Custom)
                                this.WorkRect = System.Drawing.Rectangle.Inflate(this.WorkRect, (int)label.OffsetX, (int)label.OffsetY);
                        }
                }
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                //if (this.Name != "BitmapTool")
                //{
                //    UpdateCursor(CanAddNode(this.RenderingHelper));
                //}
                if (!(this.Controller.ActiveTool is BitmapTool))
                {
                    UpdateCursor(CanAddNode(this.RenderingHelper));
                }
            }

            return this;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The rectangle tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            this.CanRender = false;
			Tool toolToReturn = base.ProcessMouseUp(evtArgs);
            if (this.InAction)
            {
                this.InAction = false;

                if (this.CurrentPoint != this.StartPoint)
                {
                    // create node bounding rectangle
                    RectangleF rectBounds = CreateNodeBoundingRect();

                    if (rectBounds.Width != 0 && rectBounds.Height != 0)
                    {
                        // create node
                        Node nodeToInsert = CreateNode(rectBounds);

                        if (nodeToInsert != null && CanInsert(nodeToInsert))
                        {
                            this.Controller.Model.HistoryManager.StartAtomicAction("Insert Node");

                            // insert node
                            this.Controller.Model.AppendChild(nodeToInsert);

                            this.Controller.Model.HistoryManager.EndAtomicAction();

                            ActionComplete(nodeToInsert);
                        }
                    }
                    else
                    {
                        this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                    }
                }
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            }

            return toolToReturn;
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            Node node = this.RenderingHelper;

            if (this.InAction && node != null)
            {
                // draw rendering helper
                node.Draw(gfx);
            }
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Creates the node from given rectangle base.
        /// </summary>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <returns>The rectangle node.</returns>
        protected abstract Node CreateNode(RectangleF rectBounding);

        /// <summary>
        /// Method used to perform additional actions after node is inserted into document
        /// ( start TextEditor after Text node is inserted into document for example )
        /// </summary>
        /// <param name="nodeInserted">node inserted into document</param>
        protected virtual void ActionComplete(Node nodeInserted)
        { 
        }

        /// <summary>
        /// Get node boundiong rectangle in model coordinates.
        /// </summary>
        /// <returns>Bounding rectangle for new node.</returns>
        private RectangleF CreateNodeBoundingRect()
        {
            // create rectangle to create node
            RectangleF rcFrame = GetFrameRectangle(this.Controller.View.Grid.SnapToGrid);           
            rcFrame = this.Controller.ConvertToModelCoordinates(rcFrame);			
            if (!this.InAction)
            {
                System.Drawing.Drawing2D.Matrix mtxScale = this.Controller.Model.DocumentScale.GetScaleTransformation(MeasureUnits.Pixel);
                mtxScale.Invert();
                rcFrame = Geometry.AppendMatrix(rcFrame, mtxScale);
            }

            //if (this.Controller != null && this.Controller.View != null
            //    && this.Controller.View.Grid.SnapToGrid)
            //{
            //    rcFrame = Geometry.ConvertRectangle(rcFrame);
            //}

            return rcFrame;
        }
        #endregion
    }
}
