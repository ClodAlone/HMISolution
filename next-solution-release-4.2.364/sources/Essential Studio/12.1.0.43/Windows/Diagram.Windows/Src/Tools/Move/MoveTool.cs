#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Interactive tool for moving nodes on a diagram.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class MoveTool
        : Tool
    {
        #region Class constants
        /// <summary>
        /// Determine mouse scroll area.
        /// </summary>
        private const float c_fSCROLL_MARGIN = 20f;

        /// <summary>
        /// Determine speed of scrolling.
        /// </summary>
        private const float c_fSCROLL_SPEED = 10f;

        /// <summary>
        /// Indicate way of selection in update selection on mouse up.
        /// </summary>
        private const bool c_bSELECTION_CIRCLE = true;

        /// <summary>
        /// Defaut offset to clone moved nodes with Ctrl key.
        /// </summary>
        private const float c_fDRAG_COPY_OFFSET = 10f;
        #endregion

        #region Class static members
        #endregion

        #region Class members
        /// <summary>
        /// Storage for control node hosting control's snapshots.
        /// </summary>
        private Hashtable m_hashCtrl;
        private Node m_nodeAdded;
        private NodeCollection m_renderingHelpers;
        private Image m_bmpHelper;
        private NodeCollection m_nodesToMove = null;
        private ConnectionPoint m_portHeadEndPointPossibleConnection;
        private ConnectionPoint m_portTailEndPointPossibleConnection;

        /// <summary>
        /// Head endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectHeadPC;

        /// <summary>
        /// Tail endpoint possible connection port bounding frame.
        /// </summary>
        private System.Drawing.Rectangle m_rectTailPC;

        /// <summary>
        /// Offset of current mouse position from the original 
        /// mouse position in client coordinates.
        /// </summary>
        protected SizeF m_currentOffset;

        /// <summary>
        /// Offset from upper left position to mouse position.
        /// </summary>
        private SizeF m_szMouseOffset;
        private bool m_bMoved;
        private PointF m_ptPrevious;
        private SizeF m_szViewSize;
        private float m_fDragCopyOffset = c_fDRAG_COPY_OFFSET;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets node collection to move on mouse up.
        /// </summary>
        protected NodeCollection NodesToMove
        {
            get
            {
                if (m_nodesToMove == null)
                    m_nodesToMove = new NodeCollection();

                return m_nodesToMove;
            }
        }

        /// <summary>
        /// Gets the snapshots hashtable where key is control node and value - image cache.
        /// </summary>
        /// <value>The snapshots.</value>
        protected Hashtable Snapshots
        {
            get
            {
                if (m_hashCtrl == null)
                {
                    m_hashCtrl = new Hashtable();
                }

                return m_hashCtrl;
            }
        }

        /// <summary>
        /// Gets a value indicating whether move tool
        /// has moved nodes it is operating with.
        /// </summary>
        public bool Moved
        {
            get { return m_bMoved; }
        }

        /// <summary>
        /// Gets the cache size of the work area.
        /// !! Getting from viewer control only one's.
        /// </summary>
        /// <value>The size of the work.</value>
        private SizeF WorkSize
        {
            get
            {
                if (m_szViewSize == SizeF.Empty)
                {
                    Control control = this.Controller.Viewer as Control;

                    if (control != null)
                    {
                        m_szViewSize = control.Size;

                        m_szViewSize.Width -= SystemInformation.VerticalScrollBarWidth;
                        m_szViewSize.Height -= SystemInformation.HorizontalScrollBarHeight;
                    }
                }

                return m_szViewSize;
            }
        }

        /// <summary>
        /// Gets or sets offset to clone moved node with using Ctrl key.
        /// </summary>
        /// <value>The drag copy offset.</value>
        public float DragCopyOffset
        {
            get { return m_fDragCopyOffset; }
            set { m_fDragCopyOffset = value; }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="toolPrevious">The tool previous.</param>
        /// <param name="nodeAdded">The node added.</param>
        public MoveTool(DiagramController controller, Tool toolPrevious, Node nodeAdded)
            : base(controller, Resources.Strings.Toolnames.Get("MoveTool"))
        {
            m_toolPreceding = toolPrevious;
            this.ActionCursor = Cursors.SizeAll;

            m_szMouseOffset = SizeF.Empty;
            m_nodeAdded = nodeAdded;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        public override void DeactivateTool()
        {
            base.DeactivateTool();

            this.Snapshots.Clear();

            if (m_bmpHelper != null)
            {
                m_bmpHelper.Dispose();
                m_bmpHelper = null;
            }
        }

        /// <summary>
        /// Aborts tool actions.
        /// </summary>
        /// <returns>The tool to abort.</returns>
        public override Tool Abort()
        {
            m_renderingHelpers = null;
            m_nodesToMove = null;

            if (m_bmpHelper != null)
            {
                m_bmpHelper.Dispose();
                m_bmpHelper = null;
            }

            this.Snapshots.Clear();

            return base.Abort();
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction && m_bMoved && m_renderingHelpers != null && m_renderingHelpers.Count > 0)
            {
                GraphicsState state = gfx.Save();

                // if( m_bmpHelper != null )
                // {
                //   gfx.TranslateTransform( m_currentOffset.Width, m_currentOffset.Height );
                //   gfx.DrawImageUnscaled( m_bmpHelper, this.WorkRect );
                // }
                // else
                {
                    // Draw moving nodes
                    for (int i = 0, nLength = m_renderingHelpers.Count; i < nLength; i++)
                    {
                        GraphicsState state1 = gfx.Save();
                        Node originalNode = this.NodesToMove[i];

                        if (Controller.DraggingStyle == RenderingHelperStyle.GhostCopy)
                        {
                            if (originalNode is ControlNode)
                            {
                                if (this.Snapshots.ContainsKey(originalNode))
                                {
                                    float fZoom = gfx.PageScale;
                                    SizeF szCurOffset = GetCurrentOffset();
                                    gfx.TranslateTransform(szCurOffset.Width / fZoom, szCurOffset.Height / fZoom, MatrixOrder.Append);

                                    // append parent's transforms
                                    Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(originalNode);
                                    gfx.MultiplyTransform(mtxTemp);

                                    // append node transformations
                                    Matrix mtx = originalNode.GetTransformations();
                                    originalNode.AppendFlipTransforms(mtx);
                                    gfx.MultiplyTransform(mtx);
                                    // render control node snapshot
                                    Image img = this.Snapshots[originalNode] as Image;

                                    gfx.DrawImageUnscaled(img, 0, 0);
                                }
                            }
                            else
                            {
                                // append parent's transforms
                                Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(originalNode);

                                // append parent scale
                                HandlesHitTesting.AppendScaleTransforms(originalNode, mtxTemp, true);

                                gfx.MultiplyTransform(mtxTemp);
                                m_renderingHelpers[i].Draw(gfx);
                            }
                        }
                        else
                        {
                            if (!(m_renderingHelpers[i] is Group))
                                DrawRenderHelper(m_renderingHelpers[i], gfx);
                            else
                            {
                                Group group = m_renderingHelpers[i] as Group;
                                DrawRenderHelper(group, gfx);
                            }
                        }

                        gfx.Restore(state1);
                    }
                }

                gfx.Restore(state);

                // highlight port if end point over port
                HighlightPorts(gfx);
                //Draws the Guides
                if (this.Controller.Guides.Enable && !(m_renderingHelpers[0] is ConnectorBase))
                {
                    this.Controller.DrawGuides(m_renderingHelpers[0], gfx, m_renderingHelpers[0].BoundingRectangle);
                }
            }
        }

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
			Tool toolToReturn = base.ProcessMouseDown(evtArgs);
            IEndPointContainer endPointContainer = null;

            if (this.Controller.SelectionList.Count == 1)
            {
                endPointContainer = this.Controller.SelectionList[0] as IEndPointContainer;
            }

            // Stop connector movement
            if (endPointContainer != null)
            {
                ConnectionPoint HeadPort = endPointContainer.HeadEndPoint.Port;
                ConnectionPoint TailPort = endPointContainer.TailEndPoint.Port;
                if (HeadPort != null && TailPort != null)
                {
                    return null;
                }
            }
            

            // get nodes involved in move action
            NodeCollection nodes = GetSelectionList();
            m_nodesToMove = GetAllowedNodes(nodes);

            // Mouse point in view coordinates
            Point ptCur = new Point(evtArgs.X, evtArgs.Y);

            // update previous mouse position.
            m_ptPrevious = ptCur;

            if (NodesToMove.Count > 0)
            {
                UpdateMouseOffset(ptCur);
                TryEnhanceRendering();

                // Calc mouse offset from drawing ImageTracker
                RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.NodesToMove, MeasureUnits.Pixel, false);
                rectTemp = this.Controller.ConvertFromModelToClientCoordinates(rectTemp);

                this.WorkRect = Geometry.ConvertRectangle(rectTemp);
                this.InAction = true;
            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);
            this.CanRender = false;

            if (this.InAction && NodesToMove != null && NodesToMove.Count > 0)
            {
                PointF ptCur = new PointF(evtArgs.X, evtArgs.Y);

                // get current mouse location with snaped and origin offset m_szMouseOffset
                if (!(!m_bMoved && ptCur == m_ptPrevious))
                    ptCur = SmartSnap(ptCur, m_szMouseOffset);

                // calc current mouse offset
                SizeF szMouseOffset = new SizeF(ptCur.X - m_ptPrevious.X, ptCur.Y - m_ptPrevious.Y);
                SizeF szOffset = szMouseOffset;

                foreach (Node node in m_renderingHelpers)
                {
                    if (!EditStyle.CanMoveX(node))
                    {
                        szOffset.Width = 0;
                    }

                    if (!EditStyle.CanMoveY(node))
                    {
                        szOffset.Height = 0;
                    }

                    if (!szOffset.IsEmpty)
                    {
                        // update offset
                        m_currentOffset.Width += szOffset.Width;
                        m_currentOffset.Height += szOffset.Height;

                        // check connection possibility
                        CheckConnectionPossibility(szMouseOffset);

                        //Gets the BoundrayConstraintEnabled value
                        bool boundaryConstraints = this.Controller.Model.BoundaryConstraintsEnabled;
                        float fZoomFactor = this.Controller.View.Magnification / 100f;

                        foreach (Node nodeMoving in this.m_renderingHelpers)
                        {
                            nodeMoving.BoundsInfo.IsResizing = true;
                            this.Controller.Model.BoundaryConstraintsEnabled = false;

                            //gets the current pinpoint
                            PointF[] ptPinPoint = new PointF[] { ((IUnitIndependent)nodeMoving).GetPinPoint(nodeMoving.MeasurementUnit)};

                            Matrix mtxTransformations = HandlesHitTesting.GetParentsTransformations(nodeMoving, false);

                            // apply transformations
                            mtxTransformations.TransformPoints(ptPinPoint);
                            
                            ptPinPoint[0].X += szMouseOffset.Width / fZoomFactor;
                            ptPinPoint[0].Y += szMouseOffset.Height / fZoomFactor;

                            mtxTransformations.Invert();
                            mtxTransformations.TransformPoints(ptPinPoint);

                            //sets the pinpoint
                            ((IUnitIndependent)nodeMoving).SetPinPoint(ptPinPoint[0], nodeMoving.MeasurementUnit);
                        }
                        this.Controller.Model.BoundaryConstraintsEnabled = boundaryConstraints;

                        // update work rect
                        UpdateWorkRect();

                        this.CanRender = true;
                    }

                    if (!szMouseOffset.IsEmpty)
                        m_bMoved = true;

                    // update cursor
                    UpdateCursor(CanMove());

                    // 7 - update previous point
                    m_ptPrevious = ptCur;
                }

            }

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            MoveNodes(evtArgs);
            return m_toolPreceding;
        }

        /// <summary>
        /// Processes the double click.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessDoubleClick(MouseEventArgs evtArgs)
        {
            MoveNodes(evtArgs);
            return base.ProcessDoubleClick(evtArgs);
        }

        /// <summary>
        /// Raise the origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            this.CanRender = false;

            if (this.InAction)
            {
                // check connection possibility
                CheckConnectionPossibility(SizeF.Empty);

                // Do not update work rect because node
                // don't moved in client coordinates.
                this.CanRender = true;
                m_bMoved = true;

                // update cursor
                UpdateCursor(CanMove());
            }

            base.OnOriginChanged(evtArgs);
        }
        #endregion

        #region Class helper methods
        #region Rendering
        /// <summary>
        /// Highlight the ports if end point are over.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        private void HighlightPorts(Graphics gfx)
        {
            // hightlight node with enable centerPort under endpoint 
            if (m_portHeadEndPointPossibleConnection is CentralPort)
            {
                HighlightCenterPortContainer(gfx, m_portHeadEndPointPossibleConnection);
            }

            if (m_portTailEndPointPossibleConnection is CentralPort)
            {
                HighlightCenterPortContainer(gfx, m_portTailEndPointPossibleConnection);
            }

            // Head endpoint possible connection
            if (m_portHeadEndPointPossibleConnection != null && !this.NodesToMove.Contains(m_portHeadEndPointPossibleConnection.Container))
            {
                DrawFrameAroundPort(gfx, m_portHeadEndPointPossibleConnection);
            }

            // Tail endpoint possible connection
            if (m_portTailEndPointPossibleConnection != null && !this.NodesToMove.Contains(m_portTailEndPointPossibleConnection.Container))
            {
                DrawFrameAroundPort(gfx, m_portTailEndPointPossibleConnection);
            }
        }

        /// <summary>
        /// If there are more than 7 nodes to render during MoveTool rendering
        /// render moveing nodes to bitmap in order to enhance MoveTool rendering
        /// </summary>
        private void TryEnhanceRendering()
        {
            // if nodes bounding rectangle exceeds max Bitmap size - skip caching
            RectangleF rcBounds = new HandleRenderer().GetBoundingRect(m_renderingHelpers);

            if (rcBounds.Width > 10000 || rcBounds.Height > 10000) return;

            if (m_bmpHelper == null)
            {
                int nCounter = 0;
                ICompositeNode nodeComposite;
                
                // count moving nodes
                foreach (Node node in m_renderingHelpers)
                {
                    nodeComposite = node as ICompositeNode;

                    if (nodeComposite != null)
                    {
                        nCounter += nodeComposite.ChildCount;
                    }

                    nCounter++;

                    if (m_renderingHelpers.Count == 1 && m_renderingHelpers.First is PseudoGroup)
                    {
                        // m_bmpHelper = RenderingHelper.RenderToBitmap( ( PseudoGroup ) m_RenderingHelpers.First );
                        break;
                    }
                    else if (nCounter > 6)
                    {
                        m_bmpHelper = RenderingHelper.RenderToImage(m_renderingHelpers);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Update the port refresh rect.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="rectPortFrame">The rect port frame.</param>
        private new void UpdatePortRefreshRect(ConnectionPoint port, ref System.Drawing.Rectangle rectPortFrame)
        {
            if (port != null)
            {
                float fZoomFactor = this.Controller.View.Magnification / 100f;
                RectangleF rectTemp = GetPortBounds(port, fZoomFactor);
                rectTemp.Inflate(2f / fZoomFactor, 2f / fZoomFactor);

                rectPortFrame = Geometry.ConvertRectangle(rectTemp);
                rectPortFrame = this.Controller.ConvertFromModelToClientCoordinates(rectPortFrame);
                this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, rectPortFrame);
            }
            else
            {
                if (!rectPortFrame.IsEmpty)
                {
                    this.WorkRect = Geometry.UpdateRectWith(this.WorkRect, rectPortFrame);
                    rectPortFrame = System.Drawing.Rectangle.Empty;
                }
            }
        }

        /// <summary>
        /// Update the work rect.
        /// </summary>
        private void UpdateWorkRect()
        {
            SizeF szOffset = GetCurrentOffset();
            this.WorkRectPrev = this.WorkRect;

            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(NodesToMove, MeasureUnits.Pixel, false);
            rectTemp = this.Controller.ConvertFromModelToClientCoordinates(rectTemp);

            Point ptNewLocation = Point.Empty;
            ptNewLocation.X = (int)(rectTemp.X + szOffset.Width);
            ptNewLocation.Y = (int)(rectTemp.Y + szOffset.Height);

            this.WorkRect = new System.Drawing.Rectangle(ptNewLocation, this.WorkRect.Size);

            // include possible connection point frame
            UpdatePortRefreshRect(m_portHeadEndPointPossibleConnection, ref m_rectHeadPC);
            UpdatePortRefreshRect(m_portTailEndPointPossibleConnection, ref m_rectTailPC);
        }
        #endregion

        #region Move Nodes
        /// <summary>
        /// Checks whether move command can be performed.
        /// </summary>
        /// <returns>true, if can move.</returns>
        protected virtual bool CanMove()
        {
            bool bCanMove = true;
            Model model = (this.Controller != null) ? this.Controller.Model : null;

            if (model != null && model.BoundaryConstraintsEnabled && NodesToMove.Count > 0)
            {
                Matrix mtxParent;
                Matrix mtxParentInvert;
                SizeF szOffsetCur = this.Controller.ConvertToModelCoordinates(GetCurrentOffset());

                foreach (Node parent in NodesToMove)
                {
                    mtxParent = HandlesHitTesting.GetParentsTransformations(parent);
                    mtxParentInvert = mtxParent.Clone();
                    mtxParentInvert.Invert();

                    SizeF szMoveOffset = CalcMoveOffset(mtxParentInvert, parent, szOffsetCur);
                    mtxParent.Translate(szMoveOffset.Width, szMoveOffset.Height, MatrixOrder.Prepend);

                    // translate node by calced offset
                    bCanMove = CheckBoundaryConstraints(parent, mtxParent);

                    if (!bCanMove)
                        break;
                }

                if (bCanMove)
                    bCanMove = AllowMove();
            }

            return bCanMove;
        }
        private bool AllowMove()
        {
            bool bAllowMove = false;
            bool bOne = NodesToMove.Count == 1;
            EditStyle protection = new EditStyle();

            foreach (Node parent in NodesToMove)
            {
                HandlesHitTesting.GetSumEditStyle(parent, ref protection);
                bAllowMove = (!bOne) ? (protection.AllowMoveX && protection.AllowMoveY) : protection.AllowMoveX || protection.AllowMoveY;

                if (!bAllowMove)
                    break;
            }

            return bAllowMove;
        }

        /// <summary>
        /// Move the nodes to current offset.
        /// </summary>
        protected virtual void MoveNodes()
        {
            if (this.NodesToMove.Count <= 0)
                return;

            // clone move nodes
            if (this.Controller.AllowCopyAtCtrlDrag && (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == (Keys.Control | Keys.Shift)))
            {
                CloneMoveNodes(this.NodesToMove);
            }
            else
            {
                Model model = this.Controller.Model;
                model.HistoryManager.StartAtomicAction("Move Nodes");
                model.BeginUpdate();

                // optimization 
                Disconnect();

                Matrix mtxTemp;
                SizeF szOffset;
                SizeF szOffsetCur = this.Controller.ConvertToModelCoordinates(GetCurrentOffset());

                // remove from selection for a while
                // note: used for the optimize update PseudoGroup.
                // this.Controller.SelectionList.Clear();
                foreach (Node nodeMoving in this.NodesToMove)
                {
                    nodeMoving.BoundsInfo.IsResizing = false;
                    // get parents transforms
                    mtxTemp = HandlesHitTesting.GetParentsTransformations(nodeMoving);
                    mtxTemp.Invert();

                    szOffset = CalcMoveOffset(mtxTemp, nodeMoving, szOffsetCur);

                    PointF ptPinPoint = ((IUnitIndependent)nodeMoving).GetPinPoint(nodeMoving.MeasurementUnit);
                    ptPinPoint.X += szOffset.Width;
                    ptPinPoint.Y += szOffset.Height;
                    // update pin point
                    ((IUnitIndependent)nodeMoving).SetPinPoint(ptPinPoint, nodeMoving.MeasurementUnit);

                }

                // reselect moved nodes
                // this.Controller.SelectionList.AddRange( this.NodesToMove );
                Connect();
                model.EndUpdate();
                model.HistoryManager.EndAtomicAction();
            }
        }

        /// <summary>
        /// Move the selected nodes
        /// </summary>         
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void MoveNodes(MouseEventArgs evtArgs)
        {
            this.CanRender = false;

            if (this.InAction)
            {
                this.InAction = false;

                if (m_bMoved && CanMove())
                {
                    if (m_currentOffset.Width != 0 || m_currentOffset.Height != 0)
                    {
                        MoveNodes();
                    }

                    // force refresh last work rect
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);

                    // redraw port frames
                    this.Controller.UpdateInfo.UpdateRefreshRect(m_rectHeadPC);
                    this.Controller.UpdateInfo.UpdateRefreshRect(m_rectTailPC);
                }
                else if (!m_bMoved && m_nodeAdded == null && !this.Controller.InPlaceEditor.Visible)
                {
                    UpdateSelection(c_bSELECTION_CIRCLE);
                }
                else if (m_currentOffset.Width != 0 || m_currentOffset.Height != 0)
                {
                    // if we can't move nodes --> force refresh last work rect
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);

                    // redraw port frames
                    this.Controller.UpdateInfo.UpdateRefreshRect(m_rectHeadPC);
                    this.Controller.UpdateInfo.UpdateRefreshRect(m_rectTailPC);
                }

                if (m_bmpHelper != null)
                {
                    m_bmpHelper.Dispose();
                    m_bmpHelper = null;
                }

                m_bMoved = false;
            }
        }

        /// <summary>
        /// Clones the move nodes.
        /// </summary>
        /// <param name="nodesMoving">Nodes collection to clone.</param>
        private void CloneMoveNodes(NodeCollection nodesMoving)
        {
            SizeF szCurrentOffset = GetCurrentOffset();

            // check if we can clone node
            if (Math.Abs(szCurrentOffset.Width) < this.DragCopyOffset && Math.Abs(szCurrentOffset.Height) < this.DragCopyOffset)
            {
                // if we can't clone nodes --> force refresh last work rect
                this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
            }
            else
            {
                // Calc origin offset 
                PointF ptCurOrigin = this.Controller.View.Origin;
                
                // snap current point and convert it to document coordinates
                PointF ptCurrent = SmartSnap(this.CurrentPoint, m_szMouseOffset);
                ptCurrent = this.Controller.ConvertToModelCoordinates(ptCurrent);
                PointF ptStart = this.Controller.ConvertToModelCoordinates(this.StartPoint);
                
                // convert move offset to model coordinates
                SizeF szOffset = GetOriginOffset();
                szOffset.Width = ptCurrent.X - ptStart.X - szOffset.Width;
                szOffset.Height = ptCurrent.Y - ptStart.Y - szOffset.Height;

                // cloned node new pin location
                PointF ptNewPin;
                
                // offset from pin point to cloned node bounding rect
                SizeF szPinOffset;
                
                // node being cloned
                Node nodeOriginal;
                
                // corresponding cloned node
                Node nodeClone;

                if (nodesMoving.Count == 1 && nodesMoving.First is PseudoGroup)
                {
                    // PseudoGroup children being moved
                    nodesMoving = GetGroupChildren((PseudoGroup)nodesMoving.First);
                }

                NodeCollection nodesCloned = nodesMoving.Clone() as NodeCollection;
                
                // move cloned nodes
                for (int i = 0, nLength = nodesCloned.Count; i < nLength; i++)
                {
                    // get helper and origin node from move list
                    nodeOriginal = nodesMoving[i];
                    nodeClone = nodesCloned[i];

                    szPinOffset = ((IUnitIndependent)nodeOriginal).GetPinPointOffset(MeasureUnits.Pixel);
                    ptNewPin = nodeOriginal.ConvertToModelCoordinates(new PointF(szPinOffset.Width, szPinOffset.Height));
                    ptNewPin.X += szOffset.Width;
                    ptNewPin.Y += szOffset.Height;
                    
                    // apply parent's transforms
                    nodeClone.RotationAngle += HandlesHitTesting.GetParentsRotation(nodeOriginal);
                    nodeClone.FlipX = HandlesHitTesting.GetParentsFlipX(nodeOriginal);
                    nodeClone.FlipY = HandlesHitTesting.GetParentsFlipY(nodeOriginal);

                    ((IUnitIndependent)nodeClone).SetPinPoint(ptNewPin, MeasureUnits.Pixel);
                }
                this.Controller.Model.BeginUpdate();
                
                // Append cloned nodes to document
                this.Controller.Model.HistoryManager.StartAtomicAction("Insert Nodes");

                // restore connections
                foreach (Node movedNode in nodesMoving)
                    RestoreConnections(movedNode, nodesCloned);

                int n;
                int nAdded = this.Controller.Model.AppendChildren(nodesCloned, out n);

                // update restored connections
                foreach (Node movedNode in nodesMoving)
                    this.Controller.Model.LinkManager.SynchronizeNodeConnections(movedNode);

                this.Controller.Model.HistoryManager.EndAtomicAction();
                this.Controller.Model.EndUpdate();
            }
        }
        #endregion

        /// <summary>
        /// Updates the move offset.
        /// </summary>
        /// <param name="nodeMoving">The moving node.</param>
        /// <param name="szMovingOffset">The offset.</param>
        /// <returns>The size of the offset.</returns>
        protected SizeF UpdateMoveOffset(Node nodeMoving, SizeF szMovingOffset)
        {
            if (!EditStyle.CanMoveX(nodeMoving))
            {
                szMovingOffset.Width = 0;
            }

            if (!EditStyle.CanMoveY(nodeMoving))
            {
                szMovingOffset.Height = 0;
            }

            return szMovingOffset;
        }

        /// <summary>
        /// Get move offset with origin offsets in client coordinates.
        /// </summary>
        /// <returns>Current move offset with origin offsets.</returns>
        protected virtual SizeF GetCurrentOffset()
        {
            SizeF szOffset = this.Controller.ConvertFromModelToClientCoordinates(GetOriginOffset());

            // append origin offset to move offset
            szOffset.Width = m_currentOffset.Width - szOffset.Width;
            szOffset.Height = m_currentOffset.Height - szOffset.Height;

            return szOffset;
        }

        /// <summary>
        /// Updates the mouse offset.
        /// </summary>
        /// <param name="ptCur">The current mouse position.</param>
        private void UpdateMouseOffset(PointF ptCur)
        {
            if (this.Controller.SelectionList.Count > 0)
            {
                MeasureUnits units = MeasureUnits.Pixel;
                double dMagnification = this.Controller.Viewer.Magnification / 100;

                IUnitIndependent node = this.Controller.SelectionList[0];
                RectangleF rcTemp = node.GetBoundingRectangle(units, false);
                RectangleF rcBoundingTemp;

                for (int i = 1, nLenght = this.Controller.SelectionList.Count; i < nLenght; i++)
                {
                    node = this.Controller.SelectionList[i];
                    rcBoundingTemp = node.GetBoundingRectangle(units, false);
                    rcTemp = RectangleF.Union(rcBoundingTemp, rcTemp);
                }

                rcTemp.X -= this.Controller.View.Origin.X;
                rcTemp.Y -= this.Controller.View.Origin.Y;

                // Calc mouse upper offset.
                m_szMouseOffset.Width = (float)(ptCur.X - rcTemp.X * dMagnification);
                m_szMouseOffset.Height = (float)(ptCur.Y - rcTemp.Y * dMagnification);
            }
        }

        /// <summary>
        /// Get the group children collection.
        /// </summary>
        /// <param name="nodeComposite">The node composite.</param>
        /// <returns>Gtoup children.</returns>
        private NodeCollection GetGroupChildren(ICompositeNode nodeComposite)
        {
            int nGroupMembersCount = nodeComposite.ChildCount;
            int nCounter = 0;
            NodeCollection nodesToReturn = new NodeCollection();
            Node nodeTemp;

            while (nCounter < nGroupMembersCount)
            {
                nodeTemp = nodeComposite.GetChild(nCounter);

                nodesToReturn.Add(nodeTemp);

                nCounter++;
            }

            return nodesToReturn;
        }

        /// <summary>
        /// Updates the selection list on mouse down.
        /// </summary>
        /// <param name="bOverway">if set to <c>true</c> node selected 
        /// from start to end and back, <b>false</b> node seleted only from start to end.</param>
        private void UpdateSelection(bool bOverway)
        {
            PointF ptMouseLocation = this.Controller.ConvertFromModelToClientCoordinates(this.Controller.MouseLocation);
            NodeCollection lstNodesUnderMouse = this.Controller.GetNodesAtPoint(Geometry.ConvertPoint(ptMouseLocation));

            if (lstNodesUnderMouse.Count == 0)
                return;

            NodeCollection lstSelectionList = this.Controller.SelectionList;
            NodeCollection lstRemoveList = new NodeCollection();
            NodeCollection lstSelectedUnderMouse = new NodeCollection();
            Node nodeToSelect = null;
            bool bControlKey = (Control.ModifierKeys == Keys.Control);

            // get all possibility nodes to deselect from selection list
            foreach (Node nodeCur in lstSelectionList)
            {
                PointF ptPoint = GetLocalPoint(nodeCur.Parent as Node, ptMouseLocation);

                if (nodeCur.ContainsPoint(ptPoint) && nodeCur.EditStyle.AllowSelect)
                {
                    lstSelectedUnderMouse.Add(nodeCur);
                }
            }

            // select next/previous node in under node lists
            if (bControlKey)
            {
                nodeToSelect = lstNodesUnderMouse.First;
            }
            else
            {
                nodeToSelect = GetSelectedNode(nodeToSelect, bOverway);
                if (!lstSelectionList.Contains(nodeToSelect))
                {
                    lstRemoveList.AddRange(lstSelectionList);
                }
            }

            if (nodeToSelect != null)
            {
                if ((lstRemoveList.Contains(nodeToSelect) || !lstSelectionList.Contains(nodeToSelect))
                    && nodeToSelect.EditStyle.AllowSelect)
                {
                    if (lstRemoveList.Contains(nodeToSelect))
                    {
                        lstRemoveList.Remove(nodeToSelect);
                        if (lstRemoveList.Count == 1)
                            lstSelectionList.Remove(lstRemoveList.First);
                        else
                            lstSelectionList.Remove(lstRemoveList);
                    }
                    else
                    {
                        if (lstRemoveList.Count == 1)
                            lstSelectionList.Remove(lstRemoveList.First);
                        else
                            lstSelectionList.Remove(lstRemoveList);
                        lstSelectionList.Add(nodeToSelect);
                    }
                }
                else if (bControlKey)
                {
                    lstSelectionList.Remove(nodeToSelect);
                }
            }
        }

        /// <summary>
        /// Get node to select from founded alternative selections.
        /// </summary>
        /// <param name="selectNode">The select node.</param>
        /// <param name="bOverway">if set to <c>true</c> overway.</param>
        /// <returns>The selected node</returns>
        private Node GetSelectedNode(Node selectNode, bool bOverway)
        {
            DiagramController controller = this.Controller;
            Node nodeHit = selectNode;
            NodeCollection lstSelected = controller.SelectionList;
            NodeCollection nodesUnderMouse = controller.GetAllNodesAtPoint(controller.Model, controller.MouseLocation, true);

            bool bSuccess = false;

            // find if any nodes to select locate in selection list
            foreach (Node selected in nodesUnderMouse)
            {
                if (lstSelected.Contains(selected))
                {
                    selectNode = selected;
                    bSuccess = true;
                }
            }

            // if selection not empty select node by step by z order
            if (lstSelected.Count > 0 && bSuccess)
            {
                // if node to select null, set node to select  as first node from selection node collection
                if (selectNode == null)
                {
                    selectNode = lstSelected.First;
                }
                else
                {
                    //// find node to select  throw hit node
                    //int index = nodesUnderMouse.IndexOf(selectNode);

                    //// if not all node view, move next
                    //if (index >= 0)
                    //{
                    //    if (bOverway)
                    //    {
                    //        index = (index >= nodesUnderMouse.Count - 1) ? 0 : index + 1;
                    //    }
                    //    else
                    //    {
                    //        int nLastIndex = nodesUnderMouse.Count - 1;
                    //        bool bChangeWay = false;

                    //        if ((m_bSelectNext && index == 0) || (!m_bSelectNext && index == nLastIndex))
                    //           bChangeWay = true;

                    //        if (m_bSelectNext)
                    //            index = (index == 0) ? index + 1 : index - 1;
                    //        else
                    //            index = (index == nLastIndex) ? index - 1 : index + 1;

                    //        if (bChangeWay)
                    //            m_bSelectNext = !m_bSelectNext;
                    //    }

                    //    selectNode = nodesUnderMouse[index];
                    //}
                    
                    foreach (Node node in nodesUnderMouse)
                    {
                        if (node is Group)
                        {
                            int index = 0;
                            if (nodesUnderMouse.IndexOf(node) < nodesUnderMouse.Count - 1)
                                index = nodesUnderMouse.IndexOf(node) + 1;
                            while (index < nodesUnderMouse.Count - 1 && selectNode == nodesUnderMouse[index] && (nodesUnderMouse[index] is Group))
                            {
                                index = nodesUnderMouse.IndexOf(nodesUnderMouse[index]) + 1;
                            }
                            selectNode = nodesUnderMouse[index];
                            break;
                        }
                        else
                        {
                            selectNode = nodesUnderMouse.First;
                            break;
                        }
                    }                   
                }
            }
            else
            {
                selectNode = nodeHit;
            }

            if (Control.ModifierKeys == Keys.Control)
            {
                NodeCollection nodes = this.Controller.NodesHit;
                int nIndex = 0;

                while (CheckParentContains(lstSelected, selectNode) && nIndex >= 0)
                {
                    nIndex = nodes.IndexOf(selectNode) - 1;
                }
            }

            return selectNode;
        }

        /// <summary>
        /// Check if node parents contains in list.
        /// </summary>
        /// <param name="lstSelected">The list to check contains.</param>
        /// <param name="selected">The node to check.</param>
        /// <returns>true, if node is contained in the parent.</returns>
        private bool CheckParentContains(NodeCollection lstSelected, Node selected)
        {
            bool bSuccess = false;
            Node parent = selected.Parent as Node;

            // locking for parent
            while (parent != null && !bSuccess)
            {
                bSuccess = lstSelected.Contains(parent);
                parent = parent.Parent as Node;
            }

            // locking for child
            if (!bSuccess)
            {
                bSuccess = CheckChildContains(lstSelected, selected as ICompositeNode);
            }

            return bSuccess;
        }

        /// <summary>
        /// Check if composite node contains any child in collection.
        /// </summary>
        /// <param name="lstSelected">The collection to check.</param>
        /// <param name="composite">The composite node.</param>
        /// <returns><b>True</b> if any child in collection conatin composite node.</returns>
        private bool CheckChildContains(NodeCollection lstSelected, ICompositeNode composite)
        {
            bool bSuccess = false;
            Node child;

            if (composite != null)
            {
                // locking for child
                for (int i = 0, nLength = composite.ChildCount; i < nLength; i++)
                {
                    child = composite.GetChild(i);
                    bSuccess = lstSelected.Contains(child);

                    if (!bSuccess)
                    {
                        bSuccess = CheckChildContains(lstSelected, child as ICompositeNode);
                    }

                    if (bSuccess)
                    {
                        break;
                    }
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Moves the nodes.
        /// </summary>
        /// <param name="mtxTemp">The matrix.</param>
        /// <param name="nodeMoving">The moving node.</param>
        /// <param name="szOffsetCur">The offset size.</param>
        /// <returns>The size of the offset</returns>
        private SizeF CalcMoveOffset(Matrix mtxTemp, Node nodeMoving, SizeF szOffsetCur)
        {
            // get moving node location
            PointF ptPin = ((IUnitIndependent)nodeMoving).GetPinPoint(MeasureUnits.Pixel);

            PointF[] pts = new PointF[] { ptPin };
            mtxTemp.TransformPoints(pts);

            PointF[] pts1 = new PointF[] { new PointF(ptPin.X + szOffsetCur.Width, ptPin.Y + szOffsetCur.Height) };
            mtxTemp.TransformPoints(pts1);

            float fOffsetX = (nodeMoving.EditStyle.AllowMoveX) ? pts1[0].X - pts[0].X : 0f;
            float fOffsetY = (nodeMoving.EditStyle.AllowMoveY) ? pts1[0].Y - pts[0].Y : 0f;

            return MeasureUnitsConverter.Convert(new SizeF(fOffsetX, fOffsetY), MeasureUnits.Pixel, nodeMoving.MeasurementUnit);
        }

        /// <summary>
        /// Returns a collection of nodes that are allowed to move.
        /// </summary>
        /// <param name="nodesIn">Nodes to test.</param>
        /// <returns>Collection of nodes that can move.</returns>
        protected NodeCollection GetAllowedNodes(NodeCollection nodesIn)
        {
            return GetAllowedNodes(nodesIn, null);
        }
        private NodeCollection GetAllowedNodes(NodeCollection nodesIn, ICompositeNode parent)
        {
            NodeCollection nodesOut = new NodeCollection();
            m_renderingHelpers = new NodeCollection();

            Node nodeRenderingHelper;

            foreach (Node curNode in nodesIn)
            {  
                // clone existing node
                nodeRenderingHelper = (Node)curNode.Clone();
                nodeRenderingHelper.Parent = curNode.Parent;

                // if current node is control node
                // make control node snapshot which will
                // be used while toll state rendering
                ControlNode nodeCtrl = curNode as ControlNode;

                if (nodeCtrl != null && Controller.DraggingStyle == RenderingHelperStyle.GhostCopy)
                {
                    Image img = MakeControlNodeSnapshot(nodeCtrl);

                    if (!this.Snapshots.ContainsKey(nodeCtrl))
                        this.Snapshots.Add(nodeCtrl, img);
                }

                // alter node's rendirring style - make it half opaque
                AlterStyle(nodeRenderingHelper);
                nodeRenderingHelper.UpdateServiceReferences(curNode);

                // add to rendering helpers
                m_renderingHelpers.Add(nodeRenderingHelper);

                // add to moving nodes
                nodesOut.Add(curNode);
            }

            return nodesOut;
        }
        #endregion

        #region Connection helpers
        /// <summary>
        /// Checks for connection possibility.
        /// </summary>
        private void CheckConnectionPossibility(SizeF szCurrentOffset)
        {
            // if we are moving only one node ( endpoint container ) check for connection possibility
            if (m_renderingHelpers != null && m_renderingHelpers.Count == 1)
            {
                if (szCurrentOffset == SizeF.Empty)
                    szCurrentOffset = GetCurrentOffset();
                if (!CheckConnectionCenterPort(szCurrentOffset))
                {
                    IEndPointContainer endPointContainer = m_renderingHelpers[0] as IEndPointContainer;

                    if (endPointContainer != null)
                    {
                        // Create HeadEndPoint clone in order to move it and
                        // to test connection possibility against it
                        EndPoint endPointTemp = (EndPoint)endPointContainer.HeadEndPoint.Clone();
                        
                        // move end point
                        PointF ptLocationNew = endPointTemp.Location;
                        ptLocationNew.X += szCurrentOffset.Width;
                        ptLocationNew.Y += szCurrentOffset.Height;
                        
                        // set new location
                        endPointTemp.Location = ptLocationNew;
                        
                        // Head EndPoint
                        m_portHeadEndPointPossibleConnection = GetPortUnderEndPoint(endPointTemp);

                        // Create HeadEndPoint clone in order to move it and
                        // to test connection possibility against it
                        endPointTemp = (EndPoint)endPointContainer.TailEndPoint.Clone();
                        
                        // move end point
                        ptLocationNew = endPointTemp.Location;
                        ptLocationNew.X += szCurrentOffset.Width;
                        ptLocationNew.Y += szCurrentOffset.Height;
                        
                        // set new location
                        endPointTemp.Location = ptLocationNew;

                        // Tail end point
                        m_portTailEndPointPossibleConnection = GetPortUnderEndPoint(endPointTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the port under end point.
        /// </summary>
        /// <param name="endPoint">The end point.</param>
        /// <returns>The port at the end point.</returns>
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
        /// <returns>true, if connection can be made to center port.</returns>
        private bool CheckConnectionCenterPort(SizeF szCurrentOffset)
        {
            bool bSuccess = false;
            IEndPointContainer endPointContainer;
            EndPoint endPoint;
            Node headUnderNode = null;
            Node tailUnderNode = null;

            foreach (Node node in m_renderingHelpers)
            {
                endPointContainer = node as IEndPointContainer;

                // check for enbale center port
                if (endPointContainer != null)
                {
                    // check for head endpoint
                    endPoint = endPointContainer.HeadEndPoint;
                    
                    // move end point
                    PointF ptLocationNew = endPoint.Location;
                    ptLocationNew.X += szCurrentOffset.Width;
                    ptLocationNew.Y += szCurrentOffset.Height;

                    NodeCollection nodes = this.Controller.GetAllNodesAtPoint(this.Controller.Model, ptLocationNew, false);

                    foreach (Node nodeCur in nodes)
                    {
                        if (nodeCur.EnableCentralPort && nodeCur != endPoint.Container)
                        {
                            headUnderNode = nodeCur;
                            break;
                        }
                    }

                    bool bExist = !this.NodesToMove.Contains(headUnderNode);

                    if (headUnderNode != null && headUnderNode.EnableCentralPort && bExist)
                    {
                        m_portHeadEndPointPossibleConnection = headUnderNode.CentralPort;
                        bSuccess = true;
                    }
                    else
                        m_portHeadEndPointPossibleConnection = null;

                    // check for tail endpoint
                    endPoint = endPointContainer.TailEndPoint;
                    
                    // move end point
                    ptLocationNew = endPoint.Location;
                    ptLocationNew.X += szCurrentOffset.Width;
                    ptLocationNew.Y += szCurrentOffset.Height;

                    nodes = this.Controller.GetAllNodesAtPoint(this.Controller.Model, ptLocationNew, false);

                    foreach (Node nodeCur in nodes)
                    {
                        if (nodeCur.EnableCentralPort && nodeCur != endPoint.Container)
                        {
                            tailUnderNode = nodeCur;
                            break;
                        }
                    }
                    if (tailUnderNode != null && tailUnderNode != headUnderNode
                        && tailUnderNode.EnableCentralPort && bExist)
                    {
                        m_portTailEndPointPossibleConnection = tailUnderNode.CentralPort;
                        bSuccess = true;
                    }
                    else
                        m_portTailEndPointPossibleConnection = null;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// If Moving node is only one and it is IEndPointContainer-> connect its endpoints.
        /// </summary>
        private void Connect()
        {
            if (this.NodesToMove.Count == 1 && (this.NodesToMove[0] is IEndPointContainer))
            {
                IEndPointContainer endPointContainer = (IEndPointContainer)this.NodesToMove[0];

                // connect head endpoint
                if (m_portHeadEndPointPossibleConnection != null)
                {
                    m_portHeadEndPointPossibleConnection.TryConnect(endPointContainer.HeadEndPoint);
                }

                // connect tail endpoint
                if (m_portTailEndPointPossibleConnection != null)
                {
                    m_portTailEndPointPossibleConnection.TryConnect(endPointContainer.TailEndPoint);
                }
            }
        }

        /// <summary>
        /// If Moving node is only one and it is IEndPointContainer-> disconnect its endpoints.
        /// </summary>
        private void Disconnect()
        {
            if (this.NodesToMove.Count == 1)
            {
                IEndPointContainer endPointContainer = this.NodesToMove[0] as IEndPointContainer;

                if (endPointContainer != null)
                {
                    EndPoint endPoint = endPointContainer.HeadEndPoint;

                    // disconnect head
                    if (endPoint.Port != null)
                        endPoint.Port.Disconnect(endPoint);

                    // disconnect tail
                    endPoint = endPointContainer.TailEndPoint;

                    if (endPoint.Port != null)
                        endPoint.Port.Disconnect(endPoint);
                }
            }
        }

        /// <summary>
        /// Restores the connections as original node.
        /// </summary>
        /// <param name="originalNode">The original node.</param>
        /// <param name="clonedNodes">The cloned node.</param>
        private void RestoreConnections(Node originalNode, NodeCollection clonedNodes)
        {
            int nPort;

            Node clonedNode = FindNodeByFullName(originalNode, clonedNodes);
            ICompositeNode originalComposite = originalNode as ICompositeNode;

            if (originalComposite != null)
            {
                for (int i = 0, length = originalComposite.ChildCount; i < length; i++)
                {
                    RestoreConnections(originalComposite.GetChild(i), clonedNodes);
                }
            }

            foreach (ConnectionPoint port in originalNode.Ports)
            {
                foreach (EndPoint connection in port.Connections)
                {
                    Node connector = connection.Container as Node;
                    Node clonedConnector = FindNodeByFullName(connector, clonedNodes);

                    IEndPointContainer endPointContainer = connector as IEndPointContainer;
                    IEndPointContainer endPointContainerCloned = clonedConnector as IEndPointContainer;

                    if (clonedConnector != null && endPointContainer != null)
                    {
                        if (connection is HeadEndPoint)
                        {
                            nPort = originalNode.Ports.IndexOf(endPointContainer.HeadEndPoint.Port);

                            if (nPort >= 0 && nPort < clonedNode.Ports.Count)
                                clonedNode.Ports[nPort].Connect(endPointContainerCloned.HeadEndPoint);
                        }
                        else if (connection is TailEndPoint)
                        {
                            nPort = originalNode.Ports.IndexOf(endPointContainer.TailEndPoint.Port);

                            if (nPort >= 0 && nPort < clonedNode.Ports.Count)
                                clonedNode.Ports[nPort].Connect(endPointContainerCloned.TailEndPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find node by unique full name in cloned collection.
        /// </summary>
        /// <param name="node">The original node.</param>
        /// <param name="nodes">The cloned nodes collection.</param>
        /// <returns>The node having the given full name.</returns>
        private Node FindNodeByFullName(Node node, NodeCollection nodes)
        {
            Node nodeToReturn = null;

            if (node != null)
            {
                ICompositeNode composite;
                NodeCollection children = new NodeCollection();
                string originalFullname = node.FullName;
                Model model = node.Root;

                // skip model name
                if (model != null && originalFullname.StartsWith(model.Name + "."))
                {
                    originalFullname = originalFullname.Remove(0, model.Name.Length + 1);
                }

                foreach (Node clone in nodes)
                {
                    // compare names
                    if (originalFullname == clone.FullName)
                    {
                        nodeToReturn = clone;
                        break;
                    }

                    composite = clone as ICompositeNode;

                    // check node children
                    if (composite != null)
                    {
                        children.Clear();

                        for (int i = 0, length = composite.ChildCount; i < length; i++)
                        {
                            children.Add(composite.GetChild(i));
                        }

                        nodeToReturn = FindNodeByFullName(node, children);

                        if (nodeToReturn != null)
                            break;
                    }
                }
            }

            return nodeToReturn;
        }
        #endregion
    }
}
