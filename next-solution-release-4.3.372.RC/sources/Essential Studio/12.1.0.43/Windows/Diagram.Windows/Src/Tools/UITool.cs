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
    /// UI Tool
    /// </summary>
    public abstract class UITool
        : Tool
    {
        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="UITool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The name.</param>
        public UITool(DiagramController controller, string name)
            : base(controller, name)
        {
        }
        #endregion

        #region Class members
        private Node m_nodeHit;
        private SingleActionTools m_toolToActivate;
        private int m_nVertexHit;
        private int m_nSegmentHit;
        private BoxPosition m_boxPositionHit = BoxPosition.Center;
        private object m_objHelper;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the tool to activate.
        /// </summary>
        /// <value>The tool to activate.</value>
        protected SingleActionTools ToolToActivate
        {
            get
            {
                return m_toolToActivate;
            }
            set
            {
                if (m_toolToActivate != value)
                    m_toolToActivate = value;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseDown(MouseEventArgs evtArgs)
        {
            Tool toolToReturn = base.ProcessMouseDown(evtArgs);

            if (evtArgs.Button == MouseButtons.Left)
            {
                // activate single action tool if possible
                if (!this.InAction)
                {
                    switch (this.ToolToActivate)
                    {
                        case SingleActionTools.MoveTool:
                            toolToReturn = PerformMove(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.ResizeTool:
                            toolToReturn = PerformResize(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.RotateTool:
                            toolToReturn = PerformRotate(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.PinPointMoveTool:
                            toolToReturn = PerformPinMove(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.ControlPointMoveTool:
                            toolToReturn = PerformHandleMove(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.VertexMoveTool:
                            toolToReturn = PerformVertexMove(evtArgs);
                            CleanUp();
                            break;
                        case SingleActionTools.VertexDeleteAction:
                            DeleteVertex();
                            Check(CurrentPoint);
                            break;
                        case SingleActionTools.VertexInsertAction:
                            InsertVertex(this.CurrentPoint);
                            Check(this.CurrentPoint);
                            break;
                        case SingleActionTools.LineSegmentTool:
                            toolToReturn = PerformLineSegmentMove(evtArgs);
                            CleanUp();
                            break;
                        default:
                            this.InAction = true;
                            //Fix SD209, no need to update the diagram(viewer)cursor.
                            //We can get the current cursor of viewer by ActiveTool.ActionCursor
                            
                            //this.Controller.Cursor = this.ActionCursor;
                            break;
                    }
                }
            }
            else if (!this.InAction &&!this.SingleActionTool)
                CleanUp();

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            CurrentPoint = new Point(evtArgs.X, evtArgs.Y);

            if (this.Controller.MouseTrackingEnabled && !this.InAction)
            {
                PointF ptModelCoord = CurrentPoint;

                // Convert to model coordinate.
                ptModelCoord = this.Controller.ConvertToModelCoordinates(ptModelCoord);

                Check(ptModelCoord);
            }

            return base.ProcessMouseMove(evtArgs);
        }
        #endregion

        #region Checking tool activation
        /// <summary>
        /// Checks for possibility tool activate.
        /// </summary>
        /// <param name="ptCurrent">The pt current.</param>
        public void Check(Point ptCurrent)
        {
            Check((PointF)Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Checks for possibility tool activate.
        /// </summary>
        /// <param name="ptCurrent">The pt current.</param>
        public void Check(PointF ptCurrent)
        {
            // only one tool can be activated at a time.
            // check whether mouse is over resize handle
            bool bSuccess = CanActivateResizeTool(ptCurrent);

            // check whether mouse is over pin point
            if (!bSuccess)
                bSuccess = CanActivatePinMoveTool(ptCurrent);

            // check whether mouse is over rotation handle
            if (!bSuccess)
                bSuccess = CanActivateRotateTool(ptCurrent);

            // check whether mouse is over connector end point.
            if (!bSuccess)
                bSuccess = CanActivateEndPointMoveTools(ptCurrent);

            // check wheher mouse is over pathNode control point.
            if (!bSuccess)
                bSuccess = CanActivateControlPointMoveTools(ptCurrent);

            // check whether mouse is over vertex handle
            if (!bSuccess)
                bSuccess = CanActivateVertexRelatingTools(ptCurrent);

            // check whether mouse is over segment line
            if (!bSuccess)
                bSuccess = CanActivateSegmentRelatingTools(ptCurrent);

            // check whether mouse is over node
            if (!bSuccess || (Control.ModifierKeys == Keys.Control && this.CurrentCursor != Resources.Cursors.DeleteVertex))
                CanActivateMoveTool(ptCurrent);
        }

        /// <summary>
        /// Determines whether this control point move tool can be activate on specified point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this tool can be activate on specified current point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateControlPointMoveTools(PointF ptCurrent)
        {
            bool bSuccess = false;
            IHandle handleHit = null;

            PathNode nodeHit = HandlesHitTesting.GetControlPointAtPoint(this.Controller.SelectionList, ptCurrent, ref handleHit);
            m_nodeHit = nodeHit;

            if (handleHit != null && IsVertexEditMode(nodeHit) && nodeHit.CanEditControlPoint()
                && !(Control.ModifierKeys == Keys.Control) && (handleHit.AllowMoveX || handleHit.AllowMoveY))
            {
                this.ToolToActivate = SingleActionTools.ControlPointMoveTool;
                this.ToolCursor = Cursors.Cross;
                m_objHelper = handleHit;
                bSuccess = true;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.ToolCursor = Cursors.Default;
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this end point tool can be activate on specified point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this tool can be activate on specified current point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateEndPointMoveTools(PointF ptCurrent)
        {
            bool bSuccess = false;
            IHandle handleHit = null;

            m_nodeHit = HandlesHitTesting.GetEndPointAtPoint(this.Controller.SelectionList, ptCurrent, ref handleHit);

            if (handleHit != null && (handleHit.AllowMoveX || handleHit.AllowMoveY))
            {
                this.ToolToActivate = SingleActionTools.ControlPointMoveTool;
                this.ToolCursor = Cursors.Cross;
                m_objHelper = handleHit;
                bSuccess = true;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.ToolCursor = Cursors.Default;
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this segments relative tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this segment relative tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use CanActivateSegmentRelatingTools() method with one PointF parameter from more accuracy.")]
        protected virtual bool CanActivateSegmentRelatingTools(Point ptCurrent)
        {
            return CanActivateSegmentRelatingTools(Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Determines whether this segments relative tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this segment relative tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateSegmentRelatingTools(PointF ptCurrent)
        {
            bool bSuccess = false;
            PathNode nodeHit = HandlesHitTesting.GetSegmentLineAtPoint(this.Controller.SelectionList, ptCurrent, ref m_nSegmentHit);

            if (IsVertexEditMode(nodeHit) && nodeHit.CanEditSegment())
            {
                // check for InsertDelete Command activation possibility.
                bSuccess = CanActivateVertexInsertCommand(nodeHit, ptCurrent);

                if (!bSuccess)
                    bSuccess = CanActivateLineSegmentTool(nodeHit, ptCurrent);

                m_nodeHit = nodeHit;
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this vertex add/remove tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this vertex add/remove tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use CanActivateVertexRelatingTools() method with one PointF parameter from more accuracy.")]
        protected virtual bool CanActivateVertexRelatingTools(Point ptCurrent)
        {
            return CanActivateVertexRelatingTools(Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Determines whether this vertex add/remove tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this vertex add/remove tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateVertexRelatingTools(PointF ptCurrent)
        {
            bool bSuccess = false;

            // update hitTesting factors
            HandlesHitTesting.ScaleFactor = this.Controller.Viewer.Magnification / 100.0f;

            PathNode nodeHit = HandlesHitTesting.GetVertexHandleAtPoint(this.Controller.SelectionList, ptCurrent, ref m_nVertexHit);
            IHandle handleHit = null;
            HandlesHitTesting.GetEndPointAtPoint(this.Controller.SelectionList, ptCurrent, ref handleHit);

            if (IsVertexEditMode(nodeHit) && nodeHit.CanEditVertexPoint() && handleHit == null)
            {
                // 1 - check for VertexDelete Command activation possibility.
                bSuccess = CanActivateVertexDeleteCommand(nodeHit);

                // 2 - check whether mouse is over Line or PolyLine handles
                if (!bSuccess)
                    bSuccess = CanActivateVertexMoveTool(nodeHit);

                m_nodeHit = nodeHit;
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether this rotate tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this rotate tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use CanActivateRotateTool() method with one PointF parameter from more accuracy.")]
        protected virtual bool CanActivateRotateTool(Point ptCurrent)
        {
            return CanActivateRotateTool(Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Determines whether this rotate tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this rotate tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateRotateTool(PointF ptCurrent)
        {
            HandlesHitTesting.ScaleFactor = this.Controller.Viewer.Magnification / 100.0f;

            // check whether pin move tool can be activated
            m_nodeHit = HandlesHitTesting.GetRotationHandleAtPoint(GetSelectionList(), ptCurrent);

            if (m_nodeHit != null)
            {
                this.ToolToActivate = SingleActionTools.RotateTool;
                this.CurrentCursor = Resources.Cursors.Rotate;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return (m_nodeHit != null);
        }

        /// <summary>
        /// Determines whether this pin move tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this pin move tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use CanActivatePinMoveTool() method with one PointF parameter from more accuracy.")]
        protected virtual bool CanActivatePinMoveTool(Point ptCurrent)
        {
            return CanActivatePinMoveTool(Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Determines whether this pin move tool can be activate on given point.
        /// </summary>
        /// <param name="ptCurrent">The current point.</param>
        /// <returns>
        /// <c>true</c> if this pin move tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivatePinMoveTool(PointF ptCurrent)
        {
            HandlesHitTesting.ScaleFactor = this.Controller.Viewer.Magnification / 100.0f;

            // check whether pin move tool can be activated
            m_nodeHit = HandlesHitTesting.GetPinPointAtPoint(GetSelectionList(), ptCurrent);

            bool bVertexEditMove = (m_nodeHit != null) ? m_nodeHit.EditStyle.DefaultHandleEditMode == HandleEditMode.Vertex : false;

            if (m_nodeHit != null && !bVertexEditMove)
            {
                this.ToolToActivate = SingleActionTools.PinPointMoveTool;
                this.CurrentCursor = Cursors.Cross;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return (m_nodeHit != null);
        }

        /// <summary>
        /// Determines whether this line segment tool can be activate on given point.
        /// </summary>
        /// <param name="nodeHit">The node under mouse.</param>
        /// <param name="ptLocation">The current point.</param>
        /// <returns>
        /// <c>true</c> if this segment tool can be activate on specified point; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateLineSegmentTool(PathNode nodeHit, PointF ptLocation)
        {
            bool bCanActivate = false;

            if (nodeHit != null && this.Controller.SelectionList.Contains(nodeHit) && nodeHit.CanEditSegment())
            {
                int nLineSegmentIndex = nodeHit.GetLineSegmentAtPoint(ptLocation);

                if (nLineSegmentIndex >= 0)
                {
                    bCanActivate = true;

                    this.ToolToActivate = SingleActionTools.LineSegmentTool;
                    this.CurrentCursor = Cursors.NoMove2D;
                }
                else
                {
                    this.ToolToActivate = SingleActionTools.None;
                    this.CurrentCursor = this.ToolCursor;
                }
            }

            return bCanActivate;
        }

        /// <summary>
        /// Determines whether this vertex insert command can be activated with specified node hit.
        /// </summary>
        /// <param name="nodeHit">The node hit.</param>
        /// <param name="ptTesting">The point testing.</param>
        /// <returns>
        /// <c>true</c> if this vertex insert command can activate with specified node hit; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateVertexInsertCommand(PathNode nodeHit, PointF ptTesting)
        {
            bool bHandlehit = false;

            if ((nodeHit != null) && (Control.ModifierKeys == (Keys.Control | Keys.Shift)))
            {
                if (nodeHit.GetLineSegmentAtPoint(ptTesting) >= 0 && nodeHit.MaxPoints > nodeHit.PointCount && nodeHit.CanEditVertexPoint())
                {
                    bHandlehit = true;
                    this.ToolToActivate = SingleActionTools.VertexInsertAction;
                    this.CurrentCursor = Resources.Cursors.InsertVertex;
                }
                else
                {
                    this.ToolToActivate = SingleActionTools.None;
                    this.CurrentCursor = this.ToolCursor;
                }
            }

            return bHandlehit;
        }

        /// <summary>
        /// Determines whether this vertex delete command can be activated with specified node hit.
        /// </summary>
        /// <param name="nodeHit">The node hit.</param>
        /// <returns>
        /// <c>true</c> if this vertex delete command can activate with specified node hit; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateVertexDeleteCommand(PathNode nodeHit)
        {
            bool bHandleHit = false;

            if ((nodeHit != null) && (Control.ModifierKeys == Keys.Control))
            {
                if (nodeHit.PointCount > nodeHit.MinPoints && IsVertexEditMode(nodeHit)
                    && nodeHit.CanEditVertexPoint() && !(nodeHit is OrthogonalConnector))
                {
                    bHandleHit = true;
                    this.ToolToActivate = SingleActionTools.VertexDeleteAction;
                    this.CurrentCursor = Resources.Cursors.DeleteVertex;
                }
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return bHandleHit;
        }

        /// <summary>
        /// Determines whether this vertex move tool can be activate on on specified nodeHit.
        /// </summary>
        /// <param name="nodeHit">The node hit.</param>
        /// <returns>
        /// <c>true</c> if this vertex move tool tool can be activate on specified nodeHit; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateVertexMoveTool(PathNode nodeHit)
        {
            bool bHandleHit = false;

            if (nodeHit != null && IsVertexEditMode(nodeHit) && nodeHit.CanEditVertexPoint()
                && !(nodeHit is OrthogonalConnector))
            {
                bHandleHit = true;
                this.ToolToActivate = SingleActionTools.VertexMoveTool;
                this.CurrentCursor = Resources.Cursors.EditVertex;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return bHandleHit;
        }

        /// <summary>
        /// Determines whether this move tool can be activate on on specified nodeHit.
        /// </summary>
        /// <param name="ptMouseLocation">The current mouse location.</param>
        /// <returns>
        /// <c>true</c> if this move tool tool can be activate on specified nodeHit; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateMoveTool(PointF ptMouseLocation)
        {
            bool bHandleHit = false;

            // get all nodes at point
            NodeCollection nodes = this.Controller.GetAllNodesAtPoint(this.Controller.Model, ptMouseLocation, true);
            m_nodeHit = null;

            // get first visible node
            foreach (Node node in nodes)
            {
                if (node.Visible)
                {
                    m_nodeHit = node;
                    break;
                }
            }

            if (m_nodeHit != null && EditStyle.CanSelect(m_nodeHit))
            {
                bHandleHit = true;
                this.ToolToActivate = SingleActionTools.MoveTool;
                this.CurrentCursor = Cursors.SizeAll;
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return bHandleHit;
        }

        /// <summary>
        /// Determines whether this resize tool can be activate on on specified nodeHit.
        /// </summary>
        /// <param name="ptCurrent">The current mouse location.</param>
        /// <returns>
        /// <c>true</c> if this resize tool tool can be activate on specified nodeHit; otherwise, <c>false</c>.
        /// </returns>
        [Obsolete("Use CanActivateResizeTool() method with one PointF parameter from more accuracy.")]
        protected virtual bool CanActivateResizeTool(Point ptCurrent)
        {
            return CanActivateResizeTool(Geometry.ConvertPoint(ptCurrent));
        }

        /// <summary>
        /// Determines whether this resize tool can be activate on on specified nodeHit.
        /// </summary>
        /// <param name="ptCurrent">The current mouse location.</param>
        /// <returns>
        /// <c>true</c> if this resize tool tool can be activate on specified nodeHit; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanActivateResizeTool(PointF ptCurrent)
        {
            HandlesHitTesting.ScaleFactor = this.Controller.Viewer.Magnification / 100.0f;

            // check whether resize tool can be activated
            m_nodeHit = HandlesHitTesting.GetResizeHandleAtPoint(GetSelectionList(), ptCurrent, ref m_boxPositionHit);

            if (m_nodeHit != null)
            {
                this.ToolToActivate = SingleActionTools.ResizeTool;
                this.CurrentCursor = GetCursor(m_boxPositionHit, m_nodeHit);
            }
            else
            {
                this.ToolToActivate = SingleActionTools.None;
                this.CurrentCursor = this.ToolCursor;
            }

            return (m_nodeHit != null);
        }

        /// <summary>
        /// Determines whether this node in vertex edit mode.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// <c>true</c> if return true node in vertex edit mode; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsVertexEditMode(Node node)
        {
            bool bSuccess = false;

            if (node != null)
                bSuccess = node.EditStyle.DefaultHandleEditMode == HandleEditMode.Vertex;

            return bSuccess;
        }
        #endregion

        #region Tools perform
        /// <summary>
        /// Performs the pin move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        protected virtual Tool PerformHandleMove(MouseEventArgs evtArgs)
        {
            HandleMoveTool toolHandleMove = new HandleMoveTool(this.Controller, this, m_nodeHit as PathNode, (IHandle)m_objHelper);
            toolHandleMove.ProcessMouseDown(evtArgs);

            return toolHandleMove;
        }

        /// <summary>
        /// Performs the line segment move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        protected virtual Tool PerformLineSegmentMove(MouseEventArgs evtArgs)
        {
            LineSegmentTool toolSegmentMove = new LineSegmentTool(this.Controller, this, m_nodeHit as PathNode);
            toolSegmentMove.ProcessMouseDown(evtArgs);

            return toolSegmentMove;
        }

        /// <summary>
        /// Activate ResizeTool and process its MouseDown.
        /// </summary>
        /// <param name="evtArgs">Event args</param>
        /// <returns>The tool to resize.</returns>
        protected virtual Tool PerformResize(MouseEventArgs evtArgs)
        {
            ResizeTool toolResize = new ResizeTool(this.Controller, m_nodeHit, m_boxPositionHit, this);
            toolResize.ProcessMouseDown(evtArgs);

            return toolResize;
        }

        /// <summary>
        /// Activate MoveTool and process its MouseDown.
        /// </summary>
        /// <param name="evtArgs">Event args</param>
        /// <returns>true, if move.</returns>
        protected virtual Tool PerformMove(MouseEventArgs evtArgs)
        {
            Node nodeToAdd = null;
            Node nodeToSelect = null;
            NodeCollection lstSelectionList = this.Controller.SelectionList;
            NodeCollection lstRemoveList = new NodeCollection();
            NodeCollection lstSelectedUnderMouse = new NodeCollection();
            NodeCollection lstNodeUnderMouse = this.Controller.GetAllNodesAtPoint(this.Controller.Model, this.Controller.MouseLocation, true);

            // get all possibility nodes to deselect from selection list
            foreach (Node nodeCur in lstSelectionList)
            {
                PointF ptPoint = GetLocalPoint(nodeCur.Parent as Node, this.Controller.MouseLocation);

                if (nodeCur.ContainsPoint(ptPoint))
                {
                    lstSelectedUnderMouse.Add(nodeCur);
                }
            }

            if (lstSelectedUnderMouse.Count == 0 && Control.ModifierKeys != Keys.Control)
            {
                lstRemoveList.AddRange(lstSelectionList);
            }

            if ((lstNodeUnderMouse.Count > 0 && lstSelectionList.Count == lstRemoveList.Count)
                || (Control.ModifierKeys == Keys.Control && lstNodeUnderMouse.Count > 0))
            {
                Node lastNode = lstNodeUnderMouse.First;
                if (!lstSelectionList.Contains(lastNode))
                {
                    nodeToAdd = lastNode;

                    // if node already selected - remove it from remove list
                    if (lstRemoveList.Contains(nodeToAdd))
                    {
                        lstRemoveList.Remove(nodeToAdd);
                    }
                    else
                    {
                        nodeToSelect = nodeToAdd;
                    }
                }
            }

            if (lstRemoveList.Count > 0)
            {
                if (this.Controller.Model.EnableSelectionListSubstitute && lstRemoveList.Count > 1)
                    lstSelectionList.Remove(lstRemoveList);
                else
                    foreach (Node node in lstRemoveList)
                        lstSelectionList.Remove(node);
            }                
            int index = 0;
            if (nodeToSelect != null)
                index = lstSelectionList.Add(nodeToSelect);

            if (index == -1)
                return null;
            else
            {
                // Make active MoveTool and perform move
                MoveTool toolMove = new MoveTool(this.Controller, this, nodeToAdd);
                toolMove.ProcessMouseDown(evtArgs);
                return toolMove;
            }
        }

        /// <summary>
        /// Performs the rotate.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The rotate tool.</returns>
        protected virtual Tool PerformRotate(MouseEventArgs evtArgs)
        {
            RotateTool toolRotate;
            try
            {
                toolRotate = new RotateTool(this.Controller, m_nodeHit, this);
                toolRotate.ProcessMouseDown(evtArgs);
            }
            catch (ArgumentException)
            {
                toolRotate = null;
            }

            return toolRotate;
        }

        /// <summary>
        /// Performs the pin move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        protected virtual Tool PerformPinMove(MouseEventArgs evtArgs)
        {
            PinPointMoveTool toolPinMove = new PinPointMoveTool(this.Controller, this, m_nodeHit);
            toolPinMove.ProcessMouseDown(evtArgs);

            return toolPinMove;
        }

        /// <summary>
        /// Performs the vertex move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        protected virtual Tool PerformVertexMove(MouseEventArgs evtArgs)
        {
            VertexMoveTool toolVertexMove = new VertexMoveTool(this.Controller, this, (PathNode)m_nodeHit, m_nVertexHit);
            toolVertexMove.ProcessMouseDown(evtArgs);

            return toolVertexMove;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the cursor.
        /// </summary>
        /// <param name="handlePos">The handle position.</param>
        /// <param name="node">The node.</param>
        /// <returns>The cursor.</returns>
        public static Cursor GetCursor(BoxPosition handlePos, Node node)
        {
            if (node == null)
                throw new ArgumentNullException(" node can't be null ");

            Cursor cursorToReturn = Cursors.Default;
            handlePos = ResizeTool.GetResizeCursors(handlePos, node);

            switch (handlePos)
            {
                case BoxPosition.TopCenter:
                case BoxPosition.BottomCenter:
                    cursorToReturn = Cursors.SizeNS;
                    break;
                case BoxPosition.MiddleLeft:
                case BoxPosition.MiddleRight:
                    cursorToReturn = Cursors.SizeWE;
                    break;
                case BoxPosition.TopLeft:
                case BoxPosition.BottomRight:
                    cursorToReturn = Cursors.SizeNWSE;
                    break;
                case BoxPosition.TopRight:
                case BoxPosition.BottomLeft:
                    cursorToReturn = Cursors.SizeNESW;
                    break;
            }

            return cursorToReturn;
        }

        /// <summary>
        /// Deactivate tools and reset all tool changes.
        /// </summary>
        private void CleanUp()
        {
            this.ToolToActivate = SingleActionTools.None;

            m_nodeHit = null;
            this.InAction = false;
        }

        /// <summary>
        /// Inserts the vertex.
        /// </summary>
        /// <param name="ptCurrent">The pt current.</param>
        private void InsertVertex(Point ptCurrent)
        {
            PathNode pathNode = m_nodeHit as PathNode;
            int segIdx;

            if (pathNode != null)
            {
                // Reset append origin.
                ptCurrent = this.Controller.ConvertToModelCoordinates(ptCurrent);

                // Get current segment node under mouse.
                segIdx = pathNode.GetLineSegmentAtPoint(ptCurrent);

                // Insert vertex only if segment exists
                if (segIdx != -1)
                {
                    // get point projection to line segment
                    PointF[] ptsSegment = pathNode.GetLineSegmentPoints(segIdx);

                    // Transform point to local.
                    PointF[] ptPoint = new PointF[] { Geometry.GetProjection(ptCurrent, ptsSegment[0], ptsSegment[1]) };

                    // Create matrix transform.
                    Matrix mtxTransforms = pathNode.GetTransformations();
                    pathNode.AppendFlipTransforms(mtxTransforms);
                    mtxTransforms.Invert();

                    // Get new vertex location in local coordinates.
                    mtxTransforms.TransformPoints(ptPoint);

                    // insert vertex.
                    this.Controller.Model.HistoryManager.StartAtomicAction("Insert Vertex");
                    pathNode.InsertPoint(segIdx + 1, ptPoint[0]);
                    this.Controller.Model.HistoryManager.EndAtomicAction();
                }
            }
        }

        /// <summary>
        /// Deletes the vertex.
        /// </summary>
        private void DeleteVertex()
        {
            PathNode ptsNode = m_nodeHit as PathNode;

            if ((ptsNode != null) && (ptsNode.PointCount > ptsNode.MinPoints))
            {
                this.Controller.Model.HistoryManager.StartAtomicAction("Remove Vertex");
                ((PathNode)m_nodeHit).RemovePoint(m_nVertexHit);
                this.Controller.Model.HistoryManager.EndAtomicAction();
            }
        }
        #endregion
    }
}
