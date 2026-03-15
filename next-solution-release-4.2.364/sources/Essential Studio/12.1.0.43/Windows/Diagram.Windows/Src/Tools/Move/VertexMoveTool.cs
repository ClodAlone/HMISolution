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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Vertext Move Tool.
    /// </summary>
    public class VertexMoveTool
        : Tool
    {
        #region Class members
        /// <summary>
        /// Helper node used to render current tool state.
        /// </summary>
        private PathNode m_renderingHelper;

        /// <summary>
        /// Original node whose vertex is being moved.
        /// </summary>
        private PathNode m_nodeVertexContainer;

        /// <summary>
        /// Moving vertex index.
        /// </summary>
        private int m_nMovingVertexIndex;

        /// <summary>
        /// Previous mouse position.
        /// </summary>
        private PointF m_ptLastMousePosition;

        #endregion

        #region Class initialize/finalize members
        /// <summary>
        /// Initializes a new instance of the <see cref="VertexMoveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="toolPreceding">The tool preceding.</param>
        /// <param name="nodeVertexContainer">The node vertex container.</param>
        /// <param name="movingVertexIndex">Index of the moving vertex.</param>
        public VertexMoveTool(DiagramController controller, Tool toolPreceding, PathNode nodeVertexContainer, int movingVertexIndex)
            : base(controller, Resources.Strings.Toolnames.Get("VertexMoveTool"))
        {
            if (nodeVertexContainer == null)
                throw new ArgumentNullException("node");

            this.ToolCursor = this.ActionCursor = Resources.Cursors.EditVertex;

            m_nMovingVertexIndex = movingVertexIndex;
            m_nodeVertexContainer = nodeVertexContainer;
            m_toolPreceding = toolPreceding;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the render helper created from original node.
        /// </summary>
        /// <value>The render helper.</value>
        protected PathNode RenderHelper
        {
            get
            {
                if (m_renderingHelper == null)
                {
                    m_renderingHelper = (PathNode)m_nodeVertexContainer.Clone();

                    AlterStyle(m_renderingHelper);
                }

                return m_renderingHelper;
            }
        }

        /// <summary>
        /// Gets or sets the last mouse position.
        /// </summary>
        /// <value>The last mouse position.</value>
        protected PointF LastMousePosition
        {
            get { return m_ptLastMousePosition; }
            set { m_ptLastMousePosition = value; }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Events the sink origin changed.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        private void EventSink_OriginChanged(ViewOriginEventArgs evtArgs)
        {
            // update node state
            UpdateHelperNode();
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            Node node = this.RenderHelper;

            if (this.InAction && node != null)
            {
                // append parent's transforms
                Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(m_nodeVertexContainer);
                gfx.MultiplyTransform(matrixTemp, MatrixOrder.Append);

                // render helper
                node.Draw(gfx);
            }
        }

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The vertext move tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);

            // sign to origin change event
            this.Controller.Viewer.EventSink.OriginChanged += new ViewOriginEventHandler(EventSink_OriginChanged);
            this.InAction = true;

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The vertext move tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // call base to update current position
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);

            // update editable node
            UpdateHelperNode();

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The vertext move tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            this.CanRender = false;

            if (this.InAction)
            {
                this.InAction = false;

                if (CanMoveVertex())
                {
                    PointF ptCur = GetCurrentPoint(true);
                    ptCur = this.Controller.ConvertToModelCoordinates(ptCur);
                    this.Controller.Model.HistoryManager.StartAtomicAction("Move Vertex");
                    UpdateVertexContainer(m_nodeVertexContainer, ptCur);
                    this.Controller.Model.HistoryManager.EndAtomicAction();
                }
                else
                {
                    // if we can't move nodes --> force refresh last work rect
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                }
            }

            return m_toolPreceding;
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Determines whether this active node vertex can move vertex.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this active node vertex  can move vertex; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMoveVertex()
        {
            bool bCanMove = false;

            if (m_nodeVertexContainer != null)
            {
                // get parent transformations
                Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodeVertexContainer);
                bCanMove = CheckBoundaryConstraints(this.RenderHelper, matrixParent);
            }

            return bCanMove;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Update node state to mouse offset.
        /// </summary>
        private void UpdateHelperNode()
        {
            this.CanRender = false;

            if (this.InAction)
            {
                UpdateRenderHelper(this.RenderHelper);

                // update refresh rects
                UpdateWorkRect();

                // update cursor
                UpdateCursor(CanMoveVertex());
            }
        }
        private void UpdateWorkRect()
        {
            // set WorkRectPrev
            this.WorkRectPrev = this.WorkRect;

            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.RenderHelper, MeasureUnits.Pixel);
            
            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(m_nodeVertexContainer);

            PointF[] pts = new PointF[]
                         {
                             rectTemp.Location,
                             new PointF( rectTemp.Right, rectTemp.Top ),
                             new PointF( rectTemp.Left, rectTemp.Bottom ),
                             new PointF( rectTemp.Right, rectTemp.Bottom )
                         };

            mtx.TransformPoints(pts);

            rectTemp = Geometry.CreateRect(pts);
            rectTemp = this.Controller.ConvertFromModelToClientCoordinates(rectTemp);
            this.WorkRect = Geometry.ConvertRectangle(rectTemp);
        }

        private void UpdateRenderHelper(PathNode nodeVtxCnt)
        {
            PointF ptCur = this.Controller.View.Grid.GetNearestGridPoint(this.CurrentPoint);
            ptCur = this.Controller.ConvertToModelCoordinates(ptCur);

            if (this.LastMousePosition != ptCur)
            {
                this.LastMousePosition = ptCur;
                this.CanRender = true;

                UpdateVertexContainer(nodeVtxCnt, ptCur);
            }
        }
        private void UpdateVertexContainer(PathNode nodeVtxCnt, PointF ptCur)
        {
            // Create transform matrix
            Matrix mtxTransforms = nodeVtxCnt.GetTransformations();
            nodeVtxCnt.AppendFlipTransforms(mtxTransforms);
            
            // get parent transformations
            Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(m_nodeVertexContainer, false);
            mtxTransforms.Multiply(mtxParent, MatrixOrder.Append);

            mtxTransforms.Invert();

            PointF[] pts = new PointF[] { ptCur };
            mtxTransforms.TransformPoints(pts);

            nodeVtxCnt.SetPoint(m_nMovingVertexIndex, pts[0]);
        }
        #endregion
    }
}
