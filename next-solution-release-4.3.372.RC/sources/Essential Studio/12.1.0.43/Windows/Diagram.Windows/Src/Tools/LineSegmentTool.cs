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
    /// Interactive tool for moving the segments of a line.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class LineSegmentTool : Tool
    {
        #region Class members
        /// <summary>
        /// Offset from UpperLeftPoint to mouse.
        /// </summary>
        private SizeF m_szMouseOffset;

        /// <summary>
        /// Path node which line segment is being moved.
        /// </summary>
        private PathNode m_nodePath;

        /// <summary>
        /// Cloned path node used to render tool state.
        /// </summary>
        private PathNode m_renderingHelper;

        /// <summary>
        /// index of segment in current moveble line member.
        /// </summary>
        private int m_nLineSegIdx;

        /// <summary>
        /// offset moveble segment in line member.
        /// </summary>
        private PointF m_ptPrevious;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegmentTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="toolPreceding">The tool preceding.</param>
        /// <param name="nodePath">The node path.</param>
        public LineSegmentTool(DiagramController controller, Tool toolPreceding, PathNode nodePath)
            : base(controller, Resources.Strings.Toolnames.Get("LineSegmentTool"))
        {
            if (nodePath == null)
                throw new ArgumentNullException("nodePath");

            this.ToolCursor = this.ActionCursor = Cursors.NoMove2D;

            m_toolPreceding = toolPreceding;
            m_nodePath = nodePath;
        }
        #endregion

        #region Class properties
        private PathNode RenderHelper
        {
            get
            {
                if (m_renderingHelper == null)
                {
                    m_renderingHelper = (PathNode)m_nodePath.Clone();
                    AlterStyle(m_renderingHelper);

                    // if we are moving connector disable control points merging
                    ConnectorBase originalConnector = m_nodePath as ConnectorBase;
                    ConnectorBase connector = m_renderingHelper as ConnectorBase;

                    if (originalConnector != null && connector != null)
                    {
                        // Set whether endpoints are connected
                        ConnectorState state = ConnectorState.Default;
                        connector.ConnectorState = state;
                    }
                    else
                    {
                        // disable line node control points merging
                        Line lineOrig = m_nodePath as Line;
                        Line lineClone = m_renderingHelper as Line;

                        if (lineOrig != null && lineClone != null)
                        {
                            // Set whether endpoints are connected
                            ConnectorState state = ConnectorState.Default;
                            lineClone.ConnectorState = state;
                        }
                    }
                }

                return m_renderingHelper;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction)
            {
                GraphicsState save = gfx.Save();
                
                // append parent's transforms
                Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(m_nodePath);
                gfx.MultiplyTransform(matrixTemp, MatrixOrder.Append);

                this.RenderHelper.Draw(gfx);

                gfx.Restore(save);
            }
        }

        /// <summary>
        /// Start segment move action
        /// </summary>
        /// <param name="evtArgs">mouse event arg</param>
        /// <returns>base tools</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);
            PointF ptTemp = new PointF(evtArgs.X,evtArgs.Y);
            this.StartPoint = Geometry.ConvertPoint(ptTemp);
            
            // update previous point
            ptTemp = m_ptPrevious = this.Controller.ConvertToModelCoordinates(ptTemp);

            //ptTemp = this.Controller.ConvertToModelCoordinates(ptTemp);

            // Set mouse offset size.
            m_nLineSegIdx = m_nodePath.GetLineSegmentAtPoint(ptTemp);

            if (m_nLineSegIdx != -1)
            {
                int segCount = m_nodePath.LineSegmentCount;

                PointF ptFirstPoint = m_nodePath.GetPoint(m_nLineSegIdx % segCount);
                PointF ptSecondPoint = m_nodePath.GetPoint((m_nLineSegIdx + 1) % segCount);
                RectangleF rcRect = Geometry.CreateRect(ptFirstPoint, ptSecondPoint);

                m_szMouseOffset = GetPointOffset(ptTemp, m_nodePath);

                m_szMouseOffset.Width -= rcRect.X;
                m_szMouseOffset.Height -= rcRect.Y;

                m_szMouseOffset = this.Controller.ConvertFromModelToClientCoordinates(m_szMouseOffset);

                this.InAction = true;
            }

            return toolToReturn;
        }

        /// <summary>
        /// On Move segment
        /// </summary>
        /// <param name="evtArgs">mouse event arg</param>
        /// <returns>base tools</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // call base to update current mouse position
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);
            this.RenderHelper.BoundsInfo.IsResizing = true;
            // update helper node state
            UpdateHelperNode();

            return toolToReturn;
        }

        /// <summary>
        /// End segment move action
        /// </summary>
        /// <param name="evtArgs">mouse event arg</param>
        /// <returns>previous tool</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            this.CanRender = false;
            this.RenderHelper.BoundsInfo.IsResizing = false;
            if (this.InAction)
            {
                this.InAction = false;
                m_nodePath.BoundsInfo.IsSegmentChanging = true;
                this.Controller.Model.HistoryManager.StartAtomicAction("Move Line Segment");

                this.Controller.Model.BeginUpdate();
                this.Controller.Model.BridgeManager.BeginUpdateIntersection();

                // calc move offset
                PointF ptStart = this.Controller.ConvertToModelCoordinates(GetStartPoint(false));
                PointF ptEnd = this.Controller.ConvertToModelCoordinates(SmartSnap(this.CurrentPoint, m_szMouseOffset));
                SizeF szOffset = GetMoveOffset(ptEnd, ptStart);
                OrthogonalConnector orthoConnector = m_nodePath as OrthogonalConnector;
                if (orthoConnector != null)
                {
                    if(orthoConnector.MoveConnectedSegments)
                    DisconnectEndPoint(m_nodePath, m_nLineSegIdx);
                }
                else
                {
                    PolyLineConnector polyConnector = m_nodePath as PolyLineConnector;
                    if(polyConnector !=null && polyConnector.MoveConnectedSegments)                        
                        DisconnectEndPoint(m_nodePath, m_nLineSegIdx);
                }
                // move segment
                m_nodePath.MoveLineSegment(m_nLineSegIdx, szOffset);

                if (orthoConnector != null)
                {
                    if (orthoConnector.LineSegments.Count == 1 && orthoConnector.ToNode != null && orthoConnector.FromNode != null)
                    {
                        OrthogonalLineSegment lineSegment = orthoConnector.LineSegments[0] as OrthogonalLineSegment;
                        PointF[] pts = null;
                        switch (lineSegment.Orientation)
                        {
                            case Orientation.Horizontal:
                                pts = new PointF[]{
                                    lineSegment.Point1, new PointF(lineSegment.Point1.X, lineSegment.Point1.Y + szOffset.Height), new PointF(lineSegment.Point2.X, lineSegment.Point1.Y + szOffset.Height), lineSegment.Point2
                                };
                                break;
                            case Orientation.Vertical:
                                pts = new PointF[]{
                                    lineSegment.Point1, new PointF(lineSegment.Point1.X + szOffset.Width, lineSegment.Point1.Y), new PointF(lineSegment.Point1.X + szOffset.Width, lineSegment.Point2.Y), lineSegment.Point2
                                };
                                break;
                        }
                        orthoConnector.SetPoints(pts);
                    }
                    if (orthoConnector.LineSegments.Count > 2)
                    {
                        orthoConnector.MergeSegements();
                        PointF[] pts = orthoConnector.GetPoints(true);
                        orthoConnector.SetPoints(pts);
                    }
                }
                // update node connections
                this.Controller.Model.LinkManager.SynchronizeNodeConnections(m_nodePath);
                this.Controller.Model.BridgeManager.EndUpdateIntersection();

                this.Controller.Model.EndUpdate();
                this.Controller.Model.HistoryManager.EndAtomicAction();
                m_nodePath.BoundsInfo.IsSegmentChanging = false;
            }

            // manually update refresh rect
            this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);

            return m_toolPreceding;
        }

        /// <summary>
        /// Raise the origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // update node state
            UpdateHelperNode();

            base.OnOriginChanged(evtArgs);
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Determines whether this segment  can move.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this segment can move; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMove()
        {
            bool bCanMove = false;

            if (m_nodePath != null)
            {
                // get parent transformations
                Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodePath);
                bCanMove = CheckBoundaryConstraints(this.RenderHelper, matrixParent);
            }

            return bCanMove;
        }

        /// <summary>
        /// Get the move offset.
        /// </summary>
        /// <param name="ptEnd">The end point position.</param>
        /// <param name="ptStart">The start point position.</param>
        /// <returns>Size of the offset.</returns>
        public SizeF GetMoveOffset(PointF ptEnd, PointF ptStart)
        {
            PointF[] pts = new PointF[] { ptStart, ptEnd };

            if (m_nodePath != null)
            {
                Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(m_nodePath, true);
                mtxTemp.Invert();
                mtxTemp.TransformPoints(pts);
            }

            // calc offset
            return new SizeF((!EditStyle.CanMoveX(m_nodePath) ? 0 : pts[1].X - pts[0].X), (!EditStyle.CanMoveY(m_nodePath) ? 0 : pts[1].Y - pts[0].Y));
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Disconnect the Line segment end point
        /// </summary>
        private void DisconnectEndPoint(PathNode node, int nLineSegIdx)
        {
            IEndPointContainer container = node as IEndPointContainer;
            IEndPointContainer renderHelper = this.RenderHelper as IEndPointContainer;
            if (container != null)
            {
                if (container.HeadEndPoint.Port != null && nLineSegIdx == node.LineSegmentCount - 2)
                {                   
                    if (container.HeadEndPoint.Location.X != renderHelper.HeadEndPoint.Location.X || container.HeadEndPoint.Location.Y != renderHelper.HeadEndPoint.Location.Y)
                        container.HeadEndPoint.Port.Disconnect(container.HeadEndPoint);
                }
                if (container.TailEndPoint.Port != null && nLineSegIdx == 0)
                {                   
                    if (container.TailEndPoint.Location.X != renderHelper.TailEndPoint.Location.X || container.TailEndPoint.Location.Y != renderHelper.TailEndPoint.Location.Y)
                        container.TailEndPoint.Port.Disconnect(container.TailEndPoint);                    
                }
            }
        }
        /// <summary>
        /// Update helper node state.
        /// </summary>
        private void UpdateHelperNode()
        {
            this.CanRender = false;

            if (this.InAction)
            {
                // Snap end point
                PointF ptEnd = this.Controller.ConvertToModelCoordinates(SmartSnap(this.CurrentPoint, m_szMouseOffset));
                
                // calc offset
                SizeF szOffset = GetMoveOffset(ptEnd, m_ptPrevious);
                
                // update previous point
                m_ptPrevious = ptEnd;

                if (!szOffset.IsEmpty)
                {
                    this.CanRender = true;
                    
                    // move segment
                    this.RenderHelper.MoveLineSegment(m_nLineSegIdx, szOffset);
                    
                    // update refresh rects
                    UpdateWorkRect();

                    // update cursor
                    UpdateCursor(CanMove());
                }
            }
        }
        private void UpdateWorkRect()
        {
            // update PrevWorkRect
            this.WorkRectPrev = this.WorkRect;

            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.RenderHelper, MeasureUnits.Pixel);
            
            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(m_nodePath);

            PointF[] pts = new PointF[] 
                                        { 
                                          rectTemp.Location,
                                          new PointF( rectTemp.X + rectTemp.Width, rectTemp.Y ),
                                          new PointF( rectTemp.Right, rectTemp.Bottom ),
                                          new PointF( rectTemp.X, rectTemp.Y + rectTemp.Height ) 
                                        };

            mtx.TransformPoints(pts);

            rectTemp = Geometry.CreateRect(pts);
            rectTemp = this.Controller.ConvertFromModelToClientCoordinates(rectTemp);
            this.WorkRect = Geometry.ConvertRectangle(rectTemp);
        }
        #endregion
    }
}
