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
    /// Inherits from Tool class. Used for moving node handles: Control point, EndPoint etc.
    /// </summary>
    public class HandleMoveTool
        : Tool
    {
        #region Class members
        private PathNode m_nodeHandleOwner;
        private IHandle m_handleMoving;
        private IHandle m_handleRenderingHelper;
        private PathNode m_nodeRenderingHelper;
        private PointF m_ptLastMousePosition;
        private ConnectionPoint m_portPosCon;
        private System.Drawing.Rectangle m_rectPort = System.Drawing.Rectangle.Empty;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleMoveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="toolPrevious">The tool previous.</param>
        /// <param name="nodeHandleOwner">The node handle owner.</param>
        /// <param name="handleMoving">The handle moving.</param>
        public HandleMoveTool(DiagramController controller, Tool toolPrevious, PathNode nodeHandleOwner, IHandle handleMoving)
            : base(controller, Resources.Strings.Toolnames.Get("HandleMoveTool"))
        {
            if (nodeHandleOwner == null)
                throw new ArgumentNullException("nodeHandleOwner");

            if (handleMoving == null)
                throw new ArgumentNullException("handleMoving");

            this.ToolCursor = this.ActionCursor = Cursors.Cross;
            m_nodeHandleOwner = nodeHandleOwner;
            m_handleMoving = handleMoving;
            m_toolPreceding = toolPrevious;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the rendering helper node.
        /// </summary>
        /// <value>The rendering helper.</value>
        /// <remarks>
        /// Node clone of current node moving for tracking.
        /// </remarks>
        public PathNode RenderHelper
        {
            get
            {
                if (m_nodeRenderingHelper == null)
                {
                    CreateRenderHelper();
                }

                return m_nodeRenderingHelper;
            }
        }
        private IHandle RenderingHelperHandle
        {
            get
            {
                if (m_handleRenderingHelper == null)
                {
                    if (m_handleMoving is HeadEndPoint)
                    {
                        m_handleRenderingHelper = ((IEndPointContainer)this.RenderHelper).HeadEndPoint;
                    }
                    else if (m_handleMoving is TailEndPoint)
                    {
                        m_handleRenderingHelper = ((IEndPointContainer)this.RenderHelper).TailEndPoint;
                    }
                    else if (m_handleMoving is ControlPoint)
                    {
                        m_handleRenderingHelper = this.RenderHelper.GetControlPointByID(m_handleMoving.ID);
                    }
                    if (m_handleRenderingHelper != null)
                        m_handleRenderingHelper.IsMoving = true;
                }

                return m_handleRenderingHelper;
            }
        }

        /// <summary>
        /// Gets or sets the last mouse position.
        /// </summary>
        /// <value>The last mouse position.</value>
        protected PointF LastMousePosition
        {
            get { return m_ptLastMousePosition; }
            set { m_ptLastMousePosition = this.Controller.ConvertFromModelToClientCoordinates(value); }
        }

        /// <summary>
        /// Gets or sets the moving handle.
        /// </summary>
        /// <value>The moving handle.</value>
        protected IHandle MovingHandle
        {
            get { return m_handleMoving; }
            set { m_handleMoving = value; }
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
                GraphicsState save = gfx.Save();
                

                if (Controller.ResizingStyle == RenderingHelperStyle.GhostCopy)
                {
                    // append parent's transforms
                    Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(m_nodeHandleOwner);
                    gfx.MultiplyTransform(matrixTemp, MatrixOrder.Append);

                    // render helper
                    node.Draw(gfx);
                }
                else
                {
                    DrawRenderHelper(node, gfx);
                }
                // restore graphics state
                gfx.Restore(save);

                IEndPointContainer con = node is IEndPointContainer ? node as IEndPointContainer : null;
                if (m_portPosCon != null && con != null && (m_portPosCon.CheckType(con.HeadEndPoint) || m_portPosCon.CheckType(con.TailEndPoint)))
                {
                    // Draw port frames
                    if (m_portPosCon is CentralPort )
                    {
                        HighlightCenterPortContainer(gfx, m_portPosCon);
                    }
                    // endpoint possible connection                   
                    DrawFrameAroundPort(gfx, m_portPosCon);                    
                }
            }
        }

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The move tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);

            PointF ptCur = this.Controller.View.Grid.GetNearestGridPoint(this.CurrentPoint,RulerHeight);
            ptCur = this.Controller.ConvertToModelCoordinates(ptCur);

            bool bControlPoint = m_handleMoving is ControlPoint;
            Matrix mxtTransform = HandlesHitTesting.GetParentsTransformations(m_nodeHandleOwner, bControlPoint);
            PointF ptHandleLocation = Geometry.AppendMatrix(m_handleMoving.Location, mxtTransform);

            // check if handle can move
            if (!m_handleMoving.AllowMoveX)
                ptCur.X = ptHandleLocation.X;

            if (!m_handleMoving.AllowMoveY)
                ptCur.Y = ptHandleLocation.Y;

            this.LastMousePosition = ptCur;
            this.StartPoint = Geometry.ConvertPoint(ptCur);
            this.InAction = true;

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The move tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            if (DisconectEndPoints() || m_handleMoving is ControlPoint)
            {
                Point ptCur = GetCurrentPoint(evtArgs);
                this.CurrentPoint = ptCur;
                this.RenderHelper.BoundsInfo.IsResizing = true;
                m_handleMoving.IsMoving = true;
                // update node state
                UpdateHelperNode();
            }

            return this;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The move tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            this.CanRender = false;

            if (this.InAction)
            {
                this.InAction = false;
                Point ptCur = GetCurrentPoint(evtArgs);
                this.CurrentPoint = ptCur;
                PointF ptEnd = this.Controller.ConvertToModelCoordinates(this.Controller.View.Grid.GetNearestGridPoint(this.CurrentPoint, RulerHeight));
                SnapToPort(ref ptEnd);

                if (GetStartPoint(false) != ptEnd && CanMoveHandle())
                {
                    this.Controller.Model.HistoryManager.StartAtomicAction("Move End Point");
                   
                    this.Controller.Model.BeginUpdate();

                    UpdateHandleContainer(m_handleMoving, m_nodeHandleOwner, ptEnd);

                    if (m_handleMoving.InUpdate)
                        m_handleMoving.InUpdate = false;

                    if (m_handleMoving.IsMoving)
                        // reconnect connection
                        ReconnectEndPoint();

                    m_handleMoving.IsMoving = false;
                    m_handleRenderingHelper.IsMoving = false;

                    // update node connections
                    this.Controller.Model.LinkManager.SynchronizeNodeConnections(m_nodeHandleOwner);
                    UpdatePortRefreshRect();

                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                    
                    // redraw port outline
                    this.Controller.UpdateInfo.UpdateRefreshRect(m_rectPort);

                    this.Controller.Model.EndUpdate();
                    this.Controller.Model.HistoryManager.EndAtomicAction();
                }
            }

            this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            
            // redraw port outline
            this.Controller.UpdateInfo.UpdateRefreshRect(m_rectPort);

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

        #region Class helper methods
        /// <summary>
        /// Update helper node state.
        /// </summary>
        private void UpdateHelperNode()
        {
            this.CanRender = false;

            if (this.InAction)
            {
                UpdateRenderHelper(this.RenderingHelperHandle, this.RenderHelper);

                // update refresh rects
                UpdateWorkRect();

                bool canConnectWithPort = false;
                CheckConnectionPossibility(this.CurrentPoint, out canConnectWithPort);
                // update cursor
                UpdateCursor(CanMoveHandle() && canConnectWithPort);

                this.CanRender = true;
            }
        }
        private void CreateRenderHelper()
        {
            m_nodeRenderingHelper = (PathNode)m_nodeHandleOwner.Clone();
            m_nodeRenderingHelper.Parent = m_nodeHandleOwner.Parent;
            m_nodeRenderingHelper.UpdateReferences(m_nodeHandleOwner);
            m_nodeRenderingHelper.UpdateServiceReferences(m_nodeHandleOwner);

            AlterStyle(m_nodeRenderingHelper);

            ConnectorBase originalConnector = m_nodeHandleOwner as ConnectorBase;
            ConnectorBase connector = m_nodeRenderingHelper as ConnectorBase;

            if (originalConnector != null && connector != null)
            {
                // Set whether endpoints are connected
                ConnectorState state = ConnectorState.Default;

                if (originalConnector.TailEndPoint.Port != null)
                    state |= ConnectorState.TailEndPointConnected;

                if (originalConnector.HeadEndPoint.Port != null)
                    state |= ConnectorState.HeadEndPointConnected;

                connector.ConnectorState = state;

                // enable rotation for connector
                connector.EditStyle.AllowRotate = true;
            }
            else
            {
                Line lineOrig = m_nodeHandleOwner as Line;
                Line lineClone = m_nodeRenderingHelper as Line;

                if (lineOrig != null && lineClone != null)
                {
                    // Set whether endpoints are connected
                    ConnectorState state = ConnectorState.Default;

                    if (lineOrig.TailEndPoint.Port != null)
                        state |= ConnectorState.TailEndPointConnected;

                    if (lineOrig.HeadEndPoint.Port != null)
                        state |= ConnectorState.HeadEndPointConnected;

                    lineClone.ConnectorState = state;
                    
                    // enable rotation for line
                    lineClone.EditStyle.AllowRotate = true;
                }
            }
        }
        private void UpdatePortRefreshRect()
        {
            ConnectionPoint port = m_portPosCon;

            if (port != null)
            {
                float fZoomFactor = this.Controller.View.Magnification / 100f;
                RectangleF rectTemp = GetPortBounds(port, fZoomFactor);
                rectTemp.Inflate(2f / fZoomFactor, 2f / fZoomFactor);

                System.Drawing.Rectangle rect = Geometry.ConvertRectangle(rectTemp);
                rect = this.Controller.ConvertFromModelToClientCoordinates(rect);
                m_rectPort = rect;

                this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, m_rectPort);
            }
            else
            {
                if (!m_rectPort.IsEmpty)
                {
                    this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, m_rectPort);
                    m_rectPort = System.Drawing.Rectangle.Empty;
                }
            }
        }
        private void UpdateRenderHelper(IHandle handleMoving, PathNode nodeVtxCnt)
        {
            PointF ptCur = this.CurrentPoint;
            PointF ptSnap = this.Controller.ConvertToModelCoordinates(ptCur);
            bool snapToPort = SnapToPort(ref ptSnap);
            if (!snapToPort)
            {           
                ptCur = this.Controller.View.Grid.GetNearestGridPoint(ptCur,RulerHeight);               
                ptCur = this.Controller.ConvertToModelCoordinates(ptCur);
                SnapToPort(ref ptCur);
            }               
            if (this.LastMousePosition != ptCur)
            {
                if (snapToPort)
                {
                    UpdateHandleContainer(handleMoving, nodeVtxCnt, ptSnap);                   
                    ptSnap = this.Controller.View.Grid.GetNearestGridPoint(ptSnap,RulerHeight);                    
                    this.LastMousePosition = ptSnap;                                      
                }
                else
                {
                    this.LastMousePosition = ptCur;                   
                    UpdateHandleContainer(handleMoving, nodeVtxCnt, ptCur);
                }
                this.CanRender = true;
                
            }
        }
        private bool SnapToPort(ref PointF ptSnapping)
        {
            // snap to port only if moving handle is EndPoint
            bool snapToPort = false;
            if (m_handleMoving is EndPoint)
            {
                ConnectionPoint port = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, ptSnapping);

                // check for connection to center port under mouse node
                if (port == null)
                {
                    port = CheckConnectionCenterPortInternal(ptSnapping); 
                }

                if (port != null && port.CanConnect(m_handleMoving as EndPoint))
                {
                    m_portPosCon = port;

                    Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(port.Container, true);
                    PointF[] pts = new PointF[] { port.GetPosition() };
                    mtxTemp.TransformPoints(pts);
                    ptSnapping = pts[0];
                    snapToPort = true;
                }
            }
            return snapToPort;
        }

        private void UpdateHandleContainer(IHandle handleMoving, PathNode nodeHandleOwner, PointF ptCur)
        {
            // Create transform matrix
            Matrix mtxTransforms = new Matrix();
            nodeHandleOwner.AppendFlipTransforms(mtxTransforms);

            // get parent transformations
            Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(m_nodeHandleOwner, false);
            mtxTransforms.Multiply(mtxParent, MatrixOrder.Append);
            mtxTransforms.Invert();

            PointF[] pts = new PointF[] { ptCur };
            mtxTransforms.TransformPoints(pts);
            ptCur = pts[0];

            // if moving handle is in local coordinates
            // convert its location to global
            PointF ptHandleCurLoc = handleMoving.Location;
            Matrix mtxTemp = new Matrix();

            // consider if moving handle is ControlPoint
            if (handleMoving is ControlPoint)
            {
                mtxTemp = nodeHandleOwner.GetTransformations();
            }
            else if (handleMoving is EndPoint)
            {
                nodeHandleOwner.AppendFlipTransforms(mtxTemp);
            }

            pts[0] = ptHandleCurLoc;
            mtxTemp.TransformPoints(pts);

            ptHandleCurLoc = pts[0];

            // calc move offset
            SizeF szMoveOffset = new SizeF(ptCur.X - ptHandleCurLoc.X, ptCur.Y - ptHandleCurLoc.Y);

            ConnectorBase connector = nodeHandleOwner as ConnectorBase;

            bool bMCP = (connector != null) &&
                        ((connector.ConnectorState & ConnectorState.MergeControlPoints)
                        == ConnectorState.MergeControlPoints);

            if (bMCP)
                connector.ConnectorState = connector.ConnectorState & (~ConnectorState.MergeControlPoints);

            // move handle
            this.Controller.Model.BeginUpdate();            
            handleMoving.Move(szMoveOffset, MeasureUnits.Pixel);
            // check for posiblility to connect
            CheckConnectionPossibility();
            this.Controller.Model.EndUpdate();

            if (bMCP)
                connector.ConnectorState |= ConnectorState.MergeControlPoints;
        }
        private void UpdateWorkRect()
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.RenderHelper, MeasureUnits.Pixel);
            
            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(m_nodeHandleOwner);

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

            this.WorkRectPrev = this.WorkRect;
            this.WorkRect = Geometry.ConvertRectangle(rectTemp);
            
            // include possible connection point frame
            UpdatePortRefreshRect();
        }

        /// <summary>
        /// Determines whether this instance can move the specified offset.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can move the specified offset; otherwise, <c>false</c>.
        /// </returns>
        private bool CanMoveHandle()
        {
            double dRotationAngle = CommonUsedValues.ALLOWED_ROTATE_ANGLE;
            float fOriginalAngle = m_nodeHandleOwner.RotationAngle;
            float fHelperAngle = this.RenderHelper.RotationAngle;

            bool bCanMove = m_nodeHandleOwner.EditStyle.AllowRotate;

            if (!bCanMove)
                bCanMove = (fOriginalAngle - dRotationAngle < fHelperAngle && fOriginalAngle + dRotationAngle > fHelperAngle);

            if (bCanMove)
            {
                // get parent transformations
                Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodeHandleOwner);
                bCanMove = CheckBoundaryConstraints(this.RenderHelper, matrixParent);
            }

            return bCanMove;
        }

        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        private void CheckConnectionPossibility()
        {
            if (!(this.RenderingHelperHandle is EndPoint)) return;

            EndPoint endPointTemp = (EndPoint)this.RenderingHelperHandle;

            // check handle location
            m_portPosCon = GetPortUnderEndPoint(endPointTemp);

            if (m_portPosCon == null)
            {
                m_portPosCon = CheckConnectionCenterPortInternal(endPointTemp.Location);
            }
            endPointTemp.Port = m_portPosCon;
        }

        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        /// <param name="ptTest">The point.</param>
        /// <param name="canConnectWithPort">true, if the port can accept connection</param>
        /// <returns>The connection point</returns>
        protected ConnectionPoint CheckConnectionPossibility(PointF ptTest, out bool canConnectWithPort)
        {
            IEndPointContainer con = this.RenderHelper is IEndPointContainer ? this.RenderHelper as IEndPointContainer : null;
            
            ConnectionPoint portToReturn = base.CheckConnectionPossibility(ptTest);           
            canConnectWithPort = true;
            if (portToReturn != null && con != null && !portToReturn.CheckType(con.HeadEndPoint))
            {
                portToReturn = null;
                canConnectWithPort = false;
            }

            return portToReturn;
        }

        /// <summary>
        /// Gets the port under end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The connection point.</returns>
        private ConnectionPoint GetPortUnderEndPoint(EndPoint endPoint)
        {
            ConnectionPoint port = null;

            if (endPoint != null)
            {
                port = HandlesHitTesting.GetConnectionPointAtPoint(this.Controller.Model.Nodes, endPoint.Location);

                if (port != null && !port.CanConnect(endPoint))
                    port = null;
            }

            return port;
        }

        /// <summary>
        /// Checks the for possibility connect to center port.
        /// </summary>
        /// <param name="ptPoint">Point to check.</param>
        /// <returns>The connection point.</returns>
        private ConnectionPoint CheckConnectionCenterPortInternal(PointF ptPoint)
        {
            ConnectionPoint portToReturn = null;
            EndPoint endPoint = m_handleMoving as EndPoint;

            Node headUnderNode = null;
            
            // check for head endpoint
            NodeCollection nodes = this.Controller.GetAllNodesAtPoint(this.Controller.Model, ptPoint, false);

            foreach (Node node in nodes)
            {
                if (node.EnableCentralPort && node != endPoint.Container)
                {
                    headUnderNode = node;
                    break;
                }
            }

            if (headUnderNode != null && headUnderNode.EnableCentralPort)
            {
                portToReturn = headUnderNode.CentralPort;

                if (portToReturn.Connections.Count >= portToReturn.ConnectionsLimit)
                    portToReturn = null;
            }

            return portToReturn;
        }

        /// <summary>
        /// Disconects the end points.
        /// </summary>
        private bool DisconectEndPoints()
        {
            EndPoint endPoint = this.MovingHandle as EndPoint;

            if (endPoint != null && endPoint.Port != null)
            {
                endPoint.Port.Disconnect(endPoint);
            }
            return endPoint != null && endPoint.Port == null ? true : false;
        }

        /// <summary>
        /// Reconnects the end points.
        /// </summary>
        private void ReconnectEndPoint()
        {
            EndPoint endPoint = this.MovingHandle as EndPoint;

            if (endPoint != null && m_portPosCon != null)
            {
                m_portPosCon.TryConnect(endPoint);
            }
        }

        /// <summary>
        /// Gets the current mouse point.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The point.</returns>
        private Point GetCurrentPoint(MouseEventArgs evtArgs)
        {
            Point ptCur = new Point(evtArgs.X, evtArgs.Y);

            if (!m_handleMoving.AllowMoveX)
                ptCur.X = (int)this.LastMousePosition.X;

            if (!m_handleMoving.AllowMoveY)
                ptCur.Y = (int)this.LastMousePosition.Y;
            return ptCur;
        }
        #endregion
    }
}
