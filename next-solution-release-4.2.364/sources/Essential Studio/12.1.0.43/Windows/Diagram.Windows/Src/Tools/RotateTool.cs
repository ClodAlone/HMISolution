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
    /// Interactive tool for rotating nodes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class RotateTool
        : Tool
    {
        #region Class members
        /// <summary>
        /// Storage for control node hosting control's snapshots.
        /// </summary>
        private Hashtable m_hashCtrl;
        private Node m_nodeRotating;
        private Node m_nodeRenderingHelper;

        /// <summary>
        /// Node pin position in model coordinates.
        /// </summary>
        private Point m_ptNodePinLocation;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RotateTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public RotateTool(DiagramController controller)
            : base(controller, Resources.Strings.Toolnames.Get("RotateTool"))
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="nodeRotating">The rotating node.</param>
        /// <param name="toolPreceding">The preceding tool.</param>
        public RotateTool(DiagramController controller, Node nodeRotating, Tool toolPreceding)
            : base(controller, Resources.Strings.Toolnames.Get("RotateTool"))
        {
            if (nodeRotating == null)
                throw new ArgumentNullException("nodeRotating");

            if (!EditStyle.CanRotate(nodeRotating))
                throw new ArgumentException("not allowed to rotate");

            this.ToolCursor = this.ActionCursor = Resources.Cursors.Rotate;
            m_toolPreceding = toolPreceding;

            // assign rotating node
            m_nodeRotating = nodeRotating;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the snapshots hash where key is node and value - caches image.
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
        /// Gets the render helper created from original node.
        /// </summary>
        /// <value>The render helper.</value>
        protected Node RenderHelper
        {
            get
            {
                if (m_nodeRenderingHelper == null)
                {
                    m_nodeRenderingHelper = (Node)m_nodeRotating.Clone();
                    AlterStyle(m_nodeRenderingHelper);
                }

                return m_nodeRenderingHelper;
            }
        }

        /// <summary>
        /// Gets the rotated current angle.
        /// </summary>
        /// <value>The current angle.</value>
        public float CurrentAngle
        {
            get
            {
                PointF ptPin = this.Controller.ConvertFromModelToClientCoordinates(m_ptNodePinLocation);
                double radians = Geometry.LineAngle(GetStartPoint(false), ptPin, GetCurrentPoint(false));

                if (double.IsNaN(radians))
                    radians = 0;

                float fAngle = (float)((radians * 180.0) / Math.PI);

                if (m_nodeRotating != null)
                {
                    if (HandlesHitTesting.GetParentsFlipX(m_nodeRotating))
                        fAngle = -fAngle;

                    if (HandlesHitTesting.GetParentsFlipY(m_nodeRotating))
                        fAngle = -fAngle;
                }

                return fAngle;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Aborts tool actions.
        /// </summary>
        /// <returns>The tool to abort.</returns>
        public override Tool Abort()
        {
            this.Snapshots.Clear();

            return base.Abort();
        }

        /// <summary>
        /// Deactivates the tool.
        /// </summary>
        public override void DeactivateTool()
        {
            base.DeactivateTool();

            this.Snapshots.Clear();
        }

        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction && this.RenderHelper != null)
            {
                GraphicsState state = gfx.Save();

                if (Controller.RotatingStyle == RenderingHelperStyle.GhostCopy)
                {
                    // append parent's transforms
                    Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(m_nodeRotating);
                    HandlesHitTesting.AppendScaleTransforms(m_nodeRotating, mtxTemp, true);
                    gfx.MultiplyTransform(mtxTemp);

                    if (m_nodeRotating is ControlNode)
                    {
                        // append node transformations
                        mtxTemp = HandlesHitTesting.GetParentsTransformations(m_nodeRotating, true);
                        gfx.MultiplyTransform(mtxTemp);

                        // get node pin location in model coordinates
                        SizeF szPinOffset = ((IUnitIndependent)m_nodeRotating).GetPinPointOffset(MeasureUnits.Pixel);
                        PointF ptLoc = m_nodeRotating.ConvertToModelCoordinates(new PointF(szPinOffset.Width, szPinOffset.Height));

                        // append tool transformations
                        mtxTemp.Reset();
                        mtxTemp.RotateAt(this.CurrentAngle, ptLoc);
                        HandlesHitTesting.AppendScaleTransforms(m_nodeRotating, mtxTemp, true);
                        gfx.MultiplyTransform(mtxTemp, MatrixOrder.Append);

                        Image img = this.Snapshots[m_nodeRotating] as Image;
                        gfx.DrawImageUnscaled(img, 0, 0);
                    }
                    else
                        // draw rendering  helper
                        this.RenderHelper.Draw(gfx);
                }
                else
                {
                    if (!(RenderHelper is Group))
                        DrawRenderHelper(RenderHelper, gfx);
                    else
                    {
                        Group group = RenderHelper as Group;
                        foreach (Node node in group.Nodes)
                        {
                            GraphicsState gfxState = gfx.Save();
                            DrawRenderHelper(node, gfx);
                            gfx.Restore(gfxState);
                        }
                    }
                }

                gfx.Restore(state);
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

            PointF[] ptsPin = new PointF[1];

            // set m_ptPinLocation
            ptsPin[0] = ((IUnitIndependent)m_nodeRotating).GetPinPoint(MeasureUnits.Pixel);

            // append parent transformations
            Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(m_nodeRotating);
            mtxParent.TransformPoints(ptsPin);

            m_ptNodePinLocation = Geometry.ConvertPoint(ptsPin[0]);

            ControlNode nodeCtrl = m_nodeRotating as ControlNode;
            
            // special treatment for control node
            if (nodeCtrl != null && Controller.RotatingStyle == RenderingHelperStyle.GhostCopy)
            {
                Image img = MakeControlNodeSnapshot(nodeCtrl);

                if (!this.Snapshots.ContainsKey(nodeCtrl))
                    this.Snapshots.Add(nodeCtrl, img);
            }

            this.InAction = true;

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse move.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseMove(MouseEventArgs evtArgs)
        {
            // call base to update current mouse position
            Tool toolToReturn = base.ProcessMouseMove(evtArgs);

            // update node state
            UpdateHelperNode();

            return toolToReturn;
        }

        /// <summary>
        /// Processes the mouse up.
        /// </summary>
        /// <param name="evtArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>The tool.</returns>
        public override Tool ProcessMouseUp(MouseEventArgs evtArgs)
        {
            if (this.InAction)
            {
                this.InAction = false;
                this.CanRender = true;

                if (CanRotate())
                {
                    Model model = this.Controller.Model;
                    model.HistoryManager.StartAtomicAction("Rotate");
                    model.BeginUpdate();

                    // update rotating node's RotationAngle
                    m_nodeRotating.RotationAngle += this.CurrentAngle;

                    model.BridgeManager.EndUpdateIntersection();

                    model.EndUpdate();
                    model.HistoryManager.EndAtomicAction();
                    
                    // rendering helper is no mode needed
                    m_nodeRenderingHelper = null;
                }
                else
                {
                    // if we can't move nodes --> force refresh last work rect
                    this.Controller.UpdateInfo.UpdateRefreshRect(this.WorkRect);
                }
            }

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

        #region Class pulic methods
        /// <summary>
        /// Determines whether this active can rotate.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this active node can rotate; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRotate()
        {
            bool bCanRotate = true;

            if (m_nodeRotating != null)
            {
                Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodeRotating);
                this.RenderHelper.RotationAngle = m_nodeRotating.RotationAngle + this.CurrentAngle;
                bCanRotate = CheckBoundaryConstraints(this.RenderHelper, matrixParent);
            }

            return bCanRotate;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Update helper node state.
        /// </summary>
        private void UpdateHelperNode()
        {
            if (this.InAction)
            {
                this.CanRender = true;

                // rotate rendering helper
                this.RenderHelper.RotationAngle = m_nodeRotating.RotationAngle + this.CurrentAngle;

                // update work rects
                this.WorkRectPrev = this.WorkRect;
                UpdateWorkRect();

                // update cursor
                UpdateCursor(CanRotate());
            }
        }

        private void UpdateWorkRect()
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.RenderHelper, MeasureUnits.Pixel, false);
            
            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(m_nodeRotating);

            PointF[] pts = new PointF[] 
                                   { 
                                       rectTemp.Location,
                                       new PointF( rectTemp.X + rectTemp.Width, rectTemp.Y ),
                                       new PointF( rectTemp.Right, rectTemp.Bottom ),
                                       new PointF( rectTemp.X, rectTemp.Y + rectTemp.Height ) 
                                   };

            HandlesHitTesting.AppendScaleTransforms(m_nodeRotating, mtx, true);
            mtx.TransformPoints(pts);

            rectTemp = Geometry.CreateRect(pts);
            rectTemp = this.Controller.ConvertFromModelToClientCoordinates(rectTemp);
            this.WorkRect = Geometry.ConvertRectangle(rectTemp);
        }
        #endregion
    }
}
