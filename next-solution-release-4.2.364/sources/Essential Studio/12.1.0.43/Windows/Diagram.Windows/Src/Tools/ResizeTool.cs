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
    /// Interactive tool for resizing nodes.
    /// </summary>
    /// <remarks>
    /// <seealso cref="Syncfusion.Windows.Forms.Diagram.Tool"/>
    /// </remarks>
    public class ResizeTool : Tool
    {
        #region Class members
        /// <summary>
        /// Storage for control node hosting control's snapshots.
        /// </summary>
        private Hashtable m_hashCtrl;

        /// <summary>
        /// Size factor as width/height.
        /// </summary>
        private float m_fSizeFactor;
        private SizeF m_szMouseOffset;
        private SizeF m_sizeOffset;
        private SizeF m_szOriginalResize;
        private PointF m_ptPrevious;
        private PointF m_ptLocation;
        /// <summary>
        /// Resizing node clone used while rendering.
        /// </summary>
        private Node m_nodeRenderingHelper;
        private Node m_nodeResizing;
        private BoxPosition m_handleHit;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeTool"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="nodeResizing">The node resizing.</param>
        /// <param name="handleHit">The handle hit.</param>
        /// <param name="toolPrevious">The tool previous.</param>
        public ResizeTool(DiagramController controller, Node nodeResizing, BoxPosition handleHit, Tool toolPrevious)
            : base(controller, Resources.Strings.Toolnames.Get("ResizeTool"))
        {
            if (nodeResizing == null)
                throw new ArgumentNullException("nodeResizing");

            if (handleHit == BoxPosition.Center)
                throw new ArgumentOutOfRangeException("handleHit");

            m_toolPreceding = toolPrevious;
            m_nodeResizing = nodeResizing;
            m_handleHit = handleHit;
            
            // Calc size factor.
            m_fSizeFactor = m_nodeResizing.Size.Width / m_nodeResizing.Size.Height;
            m_szOriginalResize = new SizeF(0, 0);
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
        /// Gets the rendering helper node.
        /// </summary>
        /// <value>The rendering helper.</value>
        /// <remarks>
        /// Node clone of current node resizing for tracking.
        /// </remarks>
        public Node RenderHelper
        {
            get
            {
                if (m_nodeRenderingHelper == null)
                {
                    m_nodeRenderingHelper = (Node)m_nodeResizing.Clone();
                    m_nodeRenderingHelper.Parent = m_nodeResizing.Parent;
                    // append scale transformation
                    using (Matrix mtxScale = new Matrix())
                    {
                        HandlesHitTesting.AppendScaleTransforms(m_nodeResizing, mtxScale, true);
                        RectangleF rcNode = new RectangleF(m_nodeRenderingHelper.PinPoint, m_nodeRenderingHelper.Size);

                        rcNode = Geometry.AppendMatrix(rcNode, mtxScale);

                        // set new pin point and size values
                        m_nodeRenderingHelper.Size = rcNode.Size;
                        m_nodeRenderingHelper.PinPoint = rcNode.Location;
                    }

                    AlterStyle(m_nodeRenderingHelper);
                    m_nodeRenderingHelper.UpdateServiceReferences(m_nodeResizing);
                }
                return m_nodeRenderingHelper;
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Activate resize tool.
        /// </summary>
        public override void ActivateTool()
        {
            this.ToolCursor = this.ActionCursor = UITool.GetCursor(m_handleHit, m_nodeResizing);
            base.ActivateTool();
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
        /// Deactivate tool.
        /// </summary>
        /// <returns>The tool.</returns>
        public override Tool Abort()
        {
            m_nodeResizing = null;
            m_nodeRenderingHelper = null;
            this.Snapshots.Clear();

            return base.Abort();
        }

        /// <summary>
        /// Draw resizing helper node.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <remarks>
        /// Draw clone of resizing node with applying alter style.
        /// </remarks>
        public override void Draw(Graphics gfx)
        {
            if (this.InAction && this.RenderHelper != null)
            {
                // save graphics state
                GraphicsState save = gfx.Save();

                if (Controller.ResizingStyle == RenderingHelperStyle.GhostCopy)
                {
                    // append parent's transforms
                    Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(m_nodeResizing);
                    gfx.MultiplyTransform(matrixTemp);

                    if (this.RenderHelper is ControlNode)
                    {
                        Matrix mtx = this.RenderHelper.GetTransformations();

                        gfx.MultiplyTransform(mtx);

                        Size szSize = Geometry.ConvertSize(
                            ((IUnitIndependent)this.RenderHelper).GetSize(MeasureUnits.Pixel));
                        ((ControlNode)this.RenderHelper).HostingControl.Location = new Point(-1000, -1000);
                        ((ControlNode)this.RenderHelper).HostingControl.Size = new Size((int)(szSize.Width * gfx.PageScale), (int)(szSize.Height * gfx.PageScale));
                        Image img = MakeControlNodeSnapshot((ControlNode)this.RenderHelper);
                        gfx.DrawImage(img, 0, 0, szSize.Width, szSize.Height);
                    }
                    else
                        // Draw resizing nodes
                        this.RenderHelper.Draw(gfx);
                }
                else
                {
                    if (!(RenderHelper is Group))
                        DrawRenderHelper(RenderHelper, gfx);
                    else
                    {
                        Group group = RenderHelper as Group;
                        DrawRenderHelper(group, gfx);
                    }
                }

                // restore graphics state
                gfx.Restore(save);

                //Draws the guides
                if (this.Controller.Guides.Enable)
                {
                    this.Controller.DrawGuides(RenderHelper, gfx, new RectangleF(RenderHelper.BoundingRectangle.Location, RenderHelper.Size));
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

            //ControlNode nodeCtrl = m_nodeResizing as ControlNode;
            
            //// special treatment for control node
            //if (nodeCtrl != null)
            //{
            //    Image img = MakeControlNodeSnapshot(nodeCtrl);

            //    if (!this.Snapshots.ContainsKey(nodeCtrl))
            //        this.Snapshots.Add(nodeCtrl, img);
            //}

            if (EditStyle.CanChangeHeight(m_nodeResizing) || EditStyle.CanChangeWidth(m_nodeResizing))
                this.InAction = true;
            else
                this.InAction = false;

            // set previous point.
            this.CurrentPoint = Geometry.ConvertPoint(TransformMousePoint(new Point(evtArgs.X, evtArgs.Y)));
            m_ptPrevious = this.Controller.ConvertToModelCoordinates(this.CurrentPoint);

            // Set mouse offset size.
            m_szMouseOffset = GetMouseOffset(m_ptPrevious, m_nodeResizing);

            PointF ptHandleLocation = new HandleRenderer().GetHandlePosition(m_handleHit, m_nodeResizing);

            m_szMouseOffset = this.Controller.ConvertFromModelToClientCoordinates(m_szMouseOffset);

            UpdateWorkRect();

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
            this.RenderHelper.BoundsInfo.IsResizing = true;
            if (Controller.ResizingStyle == RenderingHelperStyle.GhostCopy)
                UpdateHelperNode();
            else
            {
                if (this.InAction)
                {
                    PointF ptCur = TransformMousePoint(this.CurrentPoint);
                    PointF ptSnapped = this.Controller.ConvertToModelCoordinates(SmartSnap(ptCur, m_szMouseOffset));
                    this.CanRender = true;

                    // Calculate offset
                    SizeF szOffset = CalculateSizeOffset(ptCur);

                    // Update previous position
                    m_ptPrevious = new PointF(ptSnapped.X, ptSnapped.Y);

                    // append scale transformation
                    using (Matrix mtxScale = new Matrix())
                    {
                        HandlesHitTesting.AppendScaleTransforms(m_nodeResizing, mtxScale, true);
                        mtxScale.Invert();
                        RectangleF rcOffset = new RectangleF(PointF.Empty, szOffset);

                        // set new offset value
                        szOffset = Geometry.AppendMatrix(rcOffset, mtxScale).Size;
                    }

                    // Clip offset
                    szOffset = ClipResize(szOffset);

                    if (this.RenderHelper is PseudoGroup)
                    {
                        if (!this.AllowResize())
                        {
                            szOffset = SizeF.Empty;
                        }
                    }

                    bool bCanResize = true;
                    if (!szOffset.IsEmpty)
                    {
                        AssignAspectRatio(ref szOffset);

                        bool bBoundaryConstraintsEnable = Controller.Model.BoundaryConstraintsEnabled;
                        bCanResize = CanResize(szOffset);
                        m_sizeOffset = szOffset;
                    }

                    // update working rects
                    this.WorkRectPrev = this.WorkRect;
                    UpdateWorkRect();

                    // update cursor
                    UpdateCursor((this.RenderHelper is PseudoGroup) ? this.AllowResize() : bCanResize);
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
            Point ptEnd = new Point(evtArgs.X, evtArgs.Y);

            if (this.InAction && GetStartPoint(false) != ptEnd)
            {
                this.CanRender = true;

                if (CanResize())
                {
                    Model model = this.Controller.Model;
                    bool bBoundaryConstraintsEnable = model.BoundaryConstraintsEnabled;

                    this.InAction = false;

                    // Start atomic action as three properties will be changed.
                    model.HistoryManager.StartAtomicAction("Size and Position");
                    model.BeginUpdate();

                    if (this.Controller.ResizingStyle == RenderingHelperStyle.GhostCopy)
                    {
                        CompleteAction();
                    }
                    else
                    {
                        PointF ptCur = TransformMousePoint(this.CurrentPoint);

                        // Calculate offset
                        SizeF szOffset = CalculateMouseOffset(ptCur);

                        // append scale transformation
                        using (Matrix mtxScale = new Matrix())
                        {
                            HandlesHitTesting.AppendScaleTransforms(m_nodeResizing, mtxScale, true);
                            mtxScale.Invert();
                            RectangleF rcOffset = new RectangleF(PointF.Empty, szOffset);

                            // set new offset value
                            szOffset = Geometry.AppendMatrix(rcOffset, mtxScale).Size;
                        }

                        // Clip offset
                        szOffset = ClipResize(szOffset);


                        if (this.m_nodeResizing is PseudoGroup)
                        {
                            if (!this.AllowResize())
                            {
                                szOffset = SizeF.Empty;
                            }
                        }
                    


                        bool bCanResize = true;
                        if (!szOffset.IsEmpty)
                        {
                            AssignAspectRatio(ref szOffset);

                            bBoundaryConstraintsEnable = Controller.Model.BoundaryConstraintsEnabled;
                            bCanResize = CanResize(szOffset);
                            if (bCanResize)
                            {
                                // disable boundary constrains
                                QuiteBoundarySet(false);
                                // Update node pin and size.
                                UpdateSizeAndPinPosition(m_nodeResizing, m_handleHit, szOffset);
                                // restore boundary constrains
                                QuiteBoundarySet(bBoundaryConstraintsEnable);
                            }
                        }
                    }
                    model.EndUpdate();
                    model.HistoryManager.EndAtomicAction();
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
        /// Draws render helper
        /// </summary>
        /// <param name="renderHelper">The render helper.</param>
        /// <param name="gfx">Graphics to draw on.</param>
        protected override void DrawRenderHelper(Node renderHelper, Graphics gfx)
        {
            // append parent's transforms
            Matrix mtxTemp = HandlesHitTesting.GetParentsTransformations(renderHelper);
            HandlesHitTesting.AppendScaleTransforms(renderHelper, mtxTemp, true);
            gfx.MultiplyTransform(mtxTemp);

            System.Drawing.Rectangle renderHelperBounds = System.Drawing.Rectangle.Empty;
            SizeF renderHelperSize = MeasureUnitsConverter.Convert(new SizeF(renderHelper.Size.Width, renderHelper.Size.Height), renderHelper.MeasurementUnit, MeasureUnits.Pixel);
            renderHelperBounds = new System.Drawing.Rectangle(Point.Empty, new Size((int)renderHelperSize.Width, (int)renderHelperSize.Height));
            renderHelperBounds.Offset(Geometry.ConvertPoint(m_ptLocation));
            renderHelperBounds.Width += (int)m_sizeOffset.Width;
            renderHelperBounds.Height += (int)m_sizeOffset.Height;

            // append node transformations
            Matrix mtx = renderHelper.GetTransformations();
            renderHelper.AppendFlipTransforms(mtx);
            gfx.MultiplyTransform(mtx);

            DrawRenderHelper(renderHelper, gfx, renderHelperBounds);
        }

        /// <summary>
        /// Raise the origin changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.ViewOriginEventArgs"/> instance containing the event data.</param>
        protected override void OnOriginChanged(ViewOriginEventArgs evtArgs)
        {
            // turn on tool if left mouse button pressed
            if (Control.MouseButtons == MouseButtons.Left)
            {
                this.InAction = true;
            }

            // update node state
            UpdateHelperNode();

            base.OnOriginChanged(evtArgs);
        }
        #endregion

        #region Class static methods
        /// <summary>
        /// Appends node flip to handle.
        /// </summary>
        /// <param name="handle">Box position</param>
        /// <param name="node">The node</param>
        /// <remarks>
        /// Flip BoxPosition handle horizontally and/or vertically using node property flip.
        /// </remarks>
        public static void AppendHandleFlip(ref BoxPosition handle, Node node)
        {
            int nHandleIndex = (int)handle;

            // Append Flip by vertical.
            if (HandlesHitTesting.GetParentsFlipX(node))
                nHandleIndex = (CommonUsedValues.ALLOWED_BOX_POSTIONS_NUMBER_FLIPPED_X - nHandleIndex)
                    % CommonUsedValues.ALLOWED_BOX_POSTIONS_NUMBER;

            // Append Flip by horizontal.
            if (HandlesHitTesting.GetParentsFlipY(node))
                nHandleIndex = (CommonUsedValues.ALLOWED_BOX_POSTIONS_NUMBER_FLIPPED_Y - nHandleIndex)
                    % CommonUsedValues.ALLOWED_BOX_POSTIONS_NUMBER;

            if (nHandleIndex != (int)handle)
                handle = (BoxPosition)Enum.ToObject(typeof(BoxPosition), nHandleIndex);
        }

        /// <summary>
        /// Get Cursor for resize with node rotate.
        /// </summary>
        /// <param name="handlePos">Get current position</param>
        /// <param name="node">The node.</param>
        /// <returns>Box position</returns>
        public static BoxPosition GetResizeCursors(BoxPosition handlePos, Node node)
        {
            // Appden flips to this handle.
            AppendHandleFlip(ref handlePos, node);

            // Maximum offset: 4 becouse from boxPosition use 8 position 
            // and one cursor type has both positions. 8 / 2 = 4.
            int max = 4;
            int index = GetIndexHandle(handlePos, node);

            // If index lower zero move to last index.
            if (index < 0)
            {
                index = Math.Abs(max + index) % max;
            }
            else if (index > max)
            {
                // If index biggest max move to first index.
                index = index % max;
            }

            return (BoxPosition)Enum.ToObject(typeof(BoxPosition), index);
        }

        /// <summary>
        /// Gets the index handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The node.</param>
        /// <returns>The index handle.</returns>
        private static int GetIndexHandle(BoxPosition handle, Node node)
        {
            int indexHandle = (int)handle;

            float fParentAngle = HandlesHitTesting.GetParentsRotation(node);

            if (node.FlipX)
                fParentAngle = -fParentAngle;
            if (node.FlipY)
                fParentAngle = -fParentAngle;

            float fAngle = node.RotationAngle + fParentAngle;

            // 1 - Get cursor offset.
            float angleChange = (fAngle < 0) ?
                -CommonUsedValues.fDEF_ANGLE_CURSOR_CHANGE : CommonUsedValues.fDEF_ANGLE_CURSOR_CHANGE;

            // Offset added only if rotation angle larger that DEF_ANGLE_URSOR_CHANGE angle.
            int offset = (int)((fAngle + angleChange) / (CommonUsedValues.DEF_RIGHT_ANGLE / 2));

            // if flip enable change offset.
            FlipValue(node, ref offset);

            // 2 - Get offseted index.
            indexHandle += offset;

            return indexHandle;
        }

        /// <summary>
        /// Flips the value by node flip flags.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="valueToUpdate">The value.</param>
        private static void FlipValue(Node node, ref int valueToUpdate)
        {
            // Flip to negative value
            if (HandlesHitTesting.GetParentsFlipX(node))
                valueToUpdate = -valueToUpdate;

            if (HandlesHitTesting.GetParentsFlipY(node))
                valueToUpdate = -valueToUpdate;
        }

        /// <summary>
        /// Flips the value by node flip flags.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="valueToUpdate">The value.</param>
        private static void FlipValue(Node node, ref float valueToUpdate)
        {
            // Flip to negative value
            if (HandlesHitTesting.GetParentsFlipX(node))
                valueToUpdate = -valueToUpdate;

            if (HandlesHitTesting.GetParentsFlipY(node))
                valueToUpdate = -valueToUpdate;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Updates size and pin point position.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="posHandle">The position handle.</param>
        /// <param name="offset">The offset.</param>
        protected void UpdateSizeAndPinPosition(Node node, BoxPosition posHandle, SizeF offset)
        {
            // Append flip to handle.
            AppendHandleFlip(ref posHandle, node);

            // get node's dimensions in unit independent values
            SizeF szUnitIndependentSize = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);
            PointF ptUnitIndependentPin = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);

            // calc width, height factors for changing pin offset proportionally
            double fWidthFactor = szUnitIndependentPinOffset.Width / szUnitIndependentSize.Width;
            double fHeightFactor = szUnitIndependentPinOffset.Height / szUnitIndependentSize.Height;

            this.Controller.Model.BeginUpdate();
            UpdateSize(node, offset, szUnitIndependentSize);
            if(szUnitIndependentSize != node.Size)
                UpdatePinPosition(node, fWidthFactor, fHeightFactor, offset, ptUnitIndependentPin, posHandle);
            this.Controller.Model.EndUpdate();
        }

        /// <summary>
        /// Determines whether this instance can resize the specified node hit.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can resize the specified node hit; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanResize()
        {
            bool bCanResize = true;

            Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodeResizing);
            bCanResize = CheckBoundaryConstraints(this.RenderHelper, matrixParent);

            return bCanResize;
        }
        /// <summary>
        /// Determines whether this instance can resize the specified node hit.
        /// </summary>
        /// <param name="szOffset">The size offset.</param>
        /// <returns>
        /// <c>true</c> if this instance can resize the specified node hit; otherwise, <c>false</c>.
        /// </returns>
        protected bool CanResize(SizeF szOffset)
        {
            Matrix matrixParent = HandlesHitTesting.GetParentsTransformations(m_nodeResizing);
            bool bCanResize = true;
            Model document = this.Controller.Model;

            if (document != null && document.BoundaryConstraintsEnabled)
            {
                PointF ptPinPoint = ((IUnitIndependent)this.RenderHelper).GetPinPoint(MeasureUnits.Pixel);
                SizeF szSize = ((IUnitIndependent)this.RenderHelper).GetSize(MeasureUnits.Pixel);
                RectangleF rectBounding = new RectangleF(PointF.Empty, szSize);
                Matrix mtxNode = this.RenderHelper.GetTransformations();
                this.RenderHelper.AppendFlipTransforms(mtxNode);

                PointF[] pts = new PointF[]
                    {
                       rectBounding.Location,
                       new PointF( rectBounding.Right, rectBounding.Top ),
                       new PointF( rectBounding.Left, rectBounding.Bottom ),
                       new PointF( rectBounding.Right, rectBounding.Bottom )
                    };
                PointF[] ptsPinPoint = new PointF[] { ptPinPoint };
                mtxNode.TransformPoints(pts);

                // append parent transformations
                if (matrixParent != null)
                {
                    matrixParent.TransformPoints(ptsPinPoint);
                    matrixParent.TransformPoints(pts);
                }
                rectBounding = Geometry.CreateRect(pts);
                rectBounding = RectangleF.Union(rectBounding, new RectangleF(ptsPinPoint[0], SizeF.Empty));
                switch (m_handleHit)
                {
                    case BoxPosition.TopLeft:
                        rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width, rectBounding.Location.Y - szOffset.Height);
                        break;
                    case BoxPosition.TopCenter:
                        if (!this.RenderHelper.EditStyle.AspectRatio)
                            rectBounding.Location = new PointF(rectBounding.Location.X, rectBounding.Location.Y - szOffset.Height);
                        else
                            rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width / 2, rectBounding.Location.Y - szOffset.Height);
                        break;
                    case BoxPosition.TopRight:
                        rectBounding.Location = new PointF(rectBounding.Location.X, rectBounding.Location.Y - szOffset.Height);
                        break;
                    case BoxPosition.MiddleRight:
                        if (this.RenderHelper.EditStyle.AspectRatio)
                            rectBounding.Location = new PointF(rectBounding.Location.X, rectBounding.Location.Y - szOffset.Height / 2);
                        break;
                    case BoxPosition.BottomCenter:
                        if (this.RenderHelper.EditStyle.AspectRatio)
                            rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width / 2, rectBounding.Location.Y);
                        break;
                    case BoxPosition.BottomLeft:
                        rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width, rectBounding.Location.Y);
                        break;
                    case BoxPosition.MiddleLeft:
                        if (this.RenderHelper.EditStyle.AspectRatio)
                            rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width, rectBounding.Location.Y - szOffset.Height / 2);
                        else
                            rectBounding.Location = new PointF(rectBounding.Location.X - szOffset.Width, rectBounding.Location.Y);
                        break;
                }
                rectBounding.Size = new SizeF(rectBounding.Size.Width + szOffset.Width, rectBounding.Size.Height + szOffset.Height);

                // check for bounds contains
                RectangleF rcMBounds = RectangleF.Empty;
                rcMBounds.Size = MeasureUnitsConverter.Convert(document.LogicalSize, document.MeasurementUnits, MeasureUnits.Pixel);
                bCanResize = rcMBounds.Contains(rectBounding);
            }
            return bCanResize;
        }

        private bool AllowResize()
        {
            EditStyle protection = new EditStyle();
            HandlesHitTesting.GetSumEditStyle(m_nodeResizing, ref protection);

            return protection.AllowChangeHeight && protection.AllowChangeWidth && protection.AllowMoveX && protection.AllowMoveY;
        }
        #endregion        

        #region Class helper methods
		 /// <summary>
        /// Gets the mouse point offset.
        /// </summary>
        /// <param name="ptPoint">The pt point.</param>
        /// <param name="node">The node.</param>
        /// <returns>The point offset.</returns>
        private SizeF GetMouseOffset(PointF ptPoint, Node node)
        {
            PointF ptPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);
            PointF ptCorner = PointF.Empty;
            if (m_handleHit == BoxPosition.MiddleRight || m_handleHit == BoxPosition.BottomCenter || m_handleHit == BoxPosition.BottomRight)
                ptCorner = new PointF(ptPinPoint.X + szPinOffset.Width, ptPinPoint.Y + szPinOffset.Height);
            else if (m_handleHit == BoxPosition.TopRight)
                ptCorner = new PointF(ptPinPoint.X + szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);
            else if (m_handleHit == BoxPosition.BottomLeft)
                ptCorner = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y + szPinOffset.Height);
            else
                ptCorner = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);
            return new SizeF(ptPoint.X - ptCorner.X, ptPoint.Y - ptCorner.Y);
        }
        /// <summary>
        /// Update helper node state.
        /// </summary>
        private void UpdateHelperNode()
        {
            if (this.InAction)
            {
                this.CanRender = false;

                PointF ptCur = TransformMousePoint(this.CurrentPoint);
                PointF ptSnapped = this.Controller.ConvertToModelCoordinates(SmartSnap(ptCur, m_szMouseOffset));

                // Check if curPoint change.
                if (ptSnapped != m_ptPrevious)
                {
                    this.CanRender = true;
                    
                    // Calculate offset
                    SizeF szOffset = CalculateSizeOffset(ptCur);

                    // Update previous position
                    m_ptPrevious = new PointF(ptSnapped.X, ptSnapped.Y);

                    // append scale transformation
                    using (Matrix mtxScale = new Matrix())
                    {
                        HandlesHitTesting.AppendScaleTransforms(m_nodeResizing, mtxScale, true);
                        mtxScale.Invert();
                        RectangleF rcOffset = new RectangleF(PointF.Empty, szOffset);

                        // set new offset value
                        szOffset = Geometry.AppendMatrix(rcOffset, mtxScale).Size;
                    }

                    // Clip offset
                    szOffset = ClipResize(szOffset);

                    if (this.RenderHelper is PseudoGroup)
                    {
                        if (!this.AllowResize())
                        {
                            szOffset = SizeF.Empty;
                        }
                    }

                    bool bCanResize = true;
                    if (!szOffset.IsEmpty)
                    {
                        AssignAspectRatio(ref szOffset);
                        bCanResize = CanResize(szOffset);
                        if (bCanResize)
                        {
                            // safe history pause
                            this.Controller.Model.HistoryManager.Pause();
                            // Update node pin and size.
                            UpdateSizeAndPinPosition(this.RenderHelper, m_handleHit, szOffset);
                            // safe history resume
                            this.Controller.Model.HistoryManager.Resume();
                        }
                    }

                    // update working rects
                    this.WorkRectPrev = this.WorkRect;
                    UpdateWorkRect();

                    // update cursor
                    UpdateCursor((this.RenderHelper is PseudoGroup) ? this.AllowResize() : bCanResize);
                }
            }
        }
        
        /// <summary>
        /// Update workRect bounds.
        /// </summary>
        private void UpdateWorkRect()
        {
            RectangleF rectTemp = RenderingHelper.GetBoundingRectangle(this.RenderHelper, MeasureUnits.Pixel, false);
            if (Controller.ResizingStyle != RenderingHelperStyle.GhostCopy)
            {
                PointF ptCur = TransformMousePoint(this.CurrentPoint);
                PointF ptStart = Geometry.ConvertPoint(TransformMousePoint(this.StartPoint));
                ptStart = this.Controller.ConvertToModelCoordinates(ptStart);

                // Calculate offset
                PointF ptStartPoint = ptStart;
                PointF ptEndPoint = SmartSnap(ptCur, m_szMouseOffset);
                ptEndPoint = this.Controller.ConvertToModelCoordinates(ptEndPoint);

                // Calc offset using transformed end point value
                SizeF szChange = SizeF.Empty;
                szChange.Width = ptEndPoint.X - ptStartPoint.X;
                szChange.Height = ptEndPoint.Y - ptStartPoint.Y;

                m_ptLocation = szChange.ToPointF();
                bool bAspectRatio = GetNodeAspectRatio(this.RenderHelper);
                PointF ptOffset = PointF.Empty;
                if (!m_ptLocation.IsEmpty)
                {
                    switch (m_handleHit)
                    {
                        case BoxPosition.BottomCenter:
                            if (bAspectRatio)
                                ptOffset.X = -(int)m_sizeOffset.Width / 2;
                            break;
                        case BoxPosition.BottomRight:
                            break;
                        case BoxPosition.MiddleRight:
                            if (bAspectRatio)
                                ptOffset.Y = -(int)m_sizeOffset.Height / 2;
                            break;
                        case BoxPosition.TopCenter:
                            if (bAspectRatio)
                                ptOffset.X = -(int)m_sizeOffset.Width / 2;
                            ptOffset.Y = (int)m_ptLocation.Y;
                            break;
                        case BoxPosition.TopRight:
                            if (bAspectRatio)
                            {
                                ptOffset.Y = -(int)m_sizeOffset.Height;
                            }
                            else
                                ptOffset.Y = (int)m_ptLocation.Y;
                            break;
                        case BoxPosition.BottomLeft:
                            if (bAspectRatio)
                            {
                                ptOffset.X = -(int)m_sizeOffset.Width;
                            }
                            else
                                ptOffset.X = (int)m_ptLocation.X;
                            break;
                        case BoxPosition.MiddleLeft:
                            if (bAspectRatio)
                                ptOffset.Y = -(int)m_sizeOffset.Height / 2;
                            ptOffset.X = (int)m_ptLocation.X;
                            break;
                        case BoxPosition.TopLeft:
                            if (bAspectRatio)
                            {
                                ptOffset.Y = -(int)m_sizeOffset.Height;
                                ptOffset.X = -(int)m_sizeOffset.Width;
                            }
                            else
                            {
                                ptOffset.Y = (int)m_ptLocation.Y;
                                ptOffset.X = (int)m_ptLocation.X;
                            }
                            break;
                    }
                }
                rectTemp.Offset(ptOffset);
                m_ptLocation = ptOffset;

                if (!m_sizeOffset.IsEmpty)
                {
                    rectTemp.Width += m_sizeOffset.Width;
                    rectTemp.Height += m_sizeOffset.Height;
                }
            }
            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(m_nodeResizing);

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

        /// <summary>
        /// Assigns the aspect ratio.
        /// </summary>
        /// <param name="szOffset">The size offset.</param>
        private void AssignAspectRatio(ref SizeF szOffset)
        {
            bool bAspectRatio = GetNodeAspectRatio(this.RenderHelper);

            if (!bAspectRatio)
                return;

            bool bFromCorner = m_handleHit == BoxPosition.BottomLeft || m_handleHit == BoxPosition.BottomRight ||
                m_handleHit == BoxPosition.TopLeft || m_handleHit == BoxPosition.TopRight;

            if (bFromCorner)
            {
                if (Math.Abs(szOffset.Width) > Math.Abs(szOffset.Height))
                {
                    if (szOffset.Width < 0)
                        szOffset.Width = szOffset.Height * m_fSizeFactor;
                    else
                        szOffset.Height = szOffset.Width / m_fSizeFactor;
                }
                else
                {
                    if (szOffset.Height < 0)
                        szOffset.Height = szOffset.Width / m_fSizeFactor;
                    else
                        szOffset.Width = szOffset.Height * m_fSizeFactor;
                }
            }
            else
            {
                if (Math.Abs(m_szOriginalResize.Width) > Math.Abs(m_szOriginalResize.Height))
                    szOffset.Height = szOffset.Width / m_fSizeFactor;
                else
                    szOffset.Width = szOffset.Height * m_fSizeFactor;
            }
        }

        /// <summary>
        /// Clips the resize.
        /// </summary>
        /// <param name="szOffset">The offset.</param>
        /// <returns>The size.</returns>
        private SizeF ClipResize(SizeF szOffset)
        {
            SizeF szToReturn = szOffset;
            int nMinSize = CommonUsedValues.DEF_MIN_RESIZE_SIZE;

            // Get current size of rendering node.
            SizeF szSize = ((IUnitIndependent)this.RenderHelper).GetSize(MeasureUnits.Pixel);
            SizeF originalSize = MeasureUnitsConverter.ToPixels(m_nodeResizing.Size, m_nodeResizing.MeasurementUnit);
            if (Controller.ResizingStyle == RenderingHelperStyle.GhostCopy || this.InAction)
            {
                originalSize.Width += m_szOriginalResize.Width;
                originalSize.Height += m_szOriginalResize.Height;
            }
            else
            {
                originalSize.Width += m_szOriginalResize.Width/2;
                originalSize.Height += m_szOriginalResize.Height/2;
            }

            // Original minimum side.
            bool bOnTop = originalSize.Height < nMinSize;
            bool bOnLeft = originalSize.Width < nMinSize;

            // Resize with enable aspect ratio.
            szToReturn.Width = bOnLeft ? -szSize.Width + nMinSize : originalSize.Width - szSize.Width;
            szToReturn.Height = bOnTop ? -szSize.Height + nMinSize : originalSize.Height - szSize.Height;

            // Check for zero size.
            if (szSize.Width + szToReturn.Width == 0 || szSize.Height + szToReturn.Height == 0)
                throw new ArgumentException("Width and/or height of node can't be zero.");

            return szToReturn;
        }

        /// <summary>
        /// Transforms the mouse point to node coordinate.
        /// </summary>
        /// <param name="ptPoint">The point.</param>
        /// <returns>The point to node coordinate</returns>
        private PointF TransformMousePoint(PointF ptPoint)
        {
            ptPoint = this.Controller.ConvertToModelCoordinates(ptPoint);

            PointF[] ptCur = new PointF[] { ptPoint };
            PointF ptPinPoint = ((IUnitIndependent)m_nodeResizing).GetPinPoint(MeasureUnits.Pixel);

            // Create rotate inverting matrix.
            float fAngle = Geometry.ConvertToFullCircle(m_nodeResizing.RotationAngle);

            Matrix mtxTemp = new Matrix();
            mtxTemp.RotateAt(fAngle, ptPinPoint, MatrixOrder.Append);
            m_nodeResizing.AppendFlipTransforms(mtxTemp);

            fAngle = HandlesHitTesting.GetParentsRotation(m_nodeResizing);
            mtxTemp.RotateAt(fAngle, ptPinPoint, MatrixOrder.Append);
            HandlesHitTesting.AppendParentsFlipTransformations(mtxTemp, m_nodeResizing);

            // Invert matrix.
            mtxTemp.Invert();

            // Transform point.
            mtxTemp.TransformPoints(ptCur);

            return this.Controller.ConvertFromModelToClientCoordinates(ptCur[0]);
        }
        private void UpdatePinPosition(Node node, double dWidthFactor, double dHeightFactor, SizeF offset, PointF ptPinPosition, BoxPosition posHandle)
        {
            // Check for aspect ratio flag
            bool bAspectRatio = GetNodeAspectRatio(m_nodeRenderingHelper);

            // 1 - Calc new Pin location.
            PointF ptPinLocation = ptPinPosition;
            double dOffsetX = offset.Width;
            double dOffsetY = offset.Height;

            if (node.FlipX)
                dWidthFactor = 1 - dWidthFactor;

            if (node.FlipY)
                dHeightFactor = 1 - dHeightFactor;

            if (bAspectRatio)
            {
                switch (m_handleHit)
                {
                    case BoxPosition.TopCenter:
                    case BoxPosition.BottomCenter:
                        dOffsetX -= dOffsetX / (2 * dWidthFactor);
                        break;
                    case BoxPosition.MiddleRight:
                    case BoxPosition.MiddleLeft:
                        dOffsetY -= dOffsetY / (2 * dHeightFactor);
                        break;
                }
            }

            dOffsetX *= dWidthFactor;
            dOffsetY *= dHeightFactor;

            // Update pin OffsetX and OffsetY value with current handle.
            UpdateHelperPinPosition(ref dOffsetX, ref dOffsetY, dWidthFactor, dHeightFactor, posHandle);

            ptPinLocation.X += (float)dOffsetX;
            ptPinLocation.Y += (float)dOffsetY;

            float angle = GetRotationAngle(node);

            // Rotate point use geometry method to get a true-running
            Geometry.RotatePointAt(angle, ptPinPosition, ref ptPinLocation);

            // 3 - Update current Pin location
            ((IUnitIndependent)node).SetPinPoint(ptPinLocation, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Updates node's size.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="szUnitIndependentSize">Size in unit independent.</param>
        private void UpdateSize(Node node, SizeF offset, SizeF szUnitIndependentSize)
        {
            szUnitIndependentSize = new SizeF(szUnitIndependentSize.Width + offset.Width, szUnitIndependentSize.Height + offset.Height);
            
            // update node's size
            ((IUnitIndependent)node).SetSize(szUnitIndependentSize, MeasureUnits.Pixel);
        }
        private void UpdateHelperPinPosition(ref double dOffsetX, ref double dOffsetY, double dWidthFactor, double dHeightFactor, BoxPosition posHandle)
        {
            // update offset
            switch (posHandle)
            {
                case BoxPosition.TopRight:
                case BoxPosition.TopCenter:
                    dOffsetY = dOffsetY - dOffsetY / dHeightFactor;
                    break;
                case BoxPosition.TopLeft:
                    dOffsetX = dOffsetX - dOffsetX / dWidthFactor;
                    dOffsetY = dOffsetY - dOffsetY / dHeightFactor;
                    break;
                case BoxPosition.MiddleLeft:
                case BoxPosition.BottomLeft:
                    dOffsetX = dOffsetX - dOffsetX / dWidthFactor;
                    break;
            }
        }

        /// <summary>
        /// Calculate size change and new rectangle.
        /// </summary>
        /// <param name="ptCurrentPoint">The current point.</param>
        /// <returns>The size of the offset.</returns>
        private SizeF CalculateSizeOffset(PointF ptCurrentPoint)
        {
            // Create size and location for tracking.
            SizeF szChange = SizeF.Empty;
            PointF ptLocation = PointF.Empty;

            PointF ptStartPoint = m_ptPrevious;
            PointF ptEndPoint = SmartSnap(ptCurrentPoint, m_szMouseOffset);
            ptEndPoint = this.Controller.ConvertToModelCoordinates(ptEndPoint);

            // Calc offset using transformed end point value
            szChange.Width = ptEndPoint.X - ptStartPoint.X;
            szChange.Height = ptEndPoint.Y - ptStartPoint.Y;
            // Get tracking size and location change.
            ConsiderHandleHit(ref ptLocation, ref szChange);
            // Lock width changing if node can't change width
            if (!m_nodeResizing.EditStyle.AllowChangeWidth)
                ptLocation.X = szChange.Width = 0;

            // Lock height changing if node can't change height
            if (!m_nodeResizing.EditStyle.AllowChangeHeight)
                ptLocation.Y = szChange.Height = 0;

            return szChange;
        }

        /// <summary>
        /// Calculate the mouse offset
        /// </summary>
        /// <param name="ptCurrentPoint">The current point.</param>
        /// <returns>The size of the mouse offset.</returns>
        private SizeF CalculateMouseOffset(PointF ptCurrentPoint)
        {
            // Create size and location for tracking.
            SizeF szChange = SizeF.Empty;
            PointF ptLocation = PointF.Empty;
            m_ptPrevious = Geometry.ConvertPoint(TransformMousePoint(this.StartPoint));
            m_ptPrevious = this.Controller.ConvertToModelCoordinates(m_ptPrevious);
            PointF ptStartPoint = m_ptPrevious;
            PointF ptEndPoint = SmartSnap(ptCurrentPoint, m_szMouseOffset);
            ptEndPoint = this.Controller.ConvertToModelCoordinates(ptEndPoint);

            // Calc offset using transformed end point value
            szChange.Width = ptEndPoint.X - ptStartPoint.X;
            szChange.Height = ptEndPoint.Y - ptStartPoint.Y;

            // Get tracking size and location change.
            ConsiderHandleHit(ref ptLocation, ref szChange);

            // Lock width changing if node can't change width
            if (!m_nodeResizing.EditStyle.AllowChangeWidth)
                ptLocation.X = szChange.Width = 0;

            // Lock height changing if node can't change height
            if (!m_nodeResizing.EditStyle.AllowChangeHeight)
                ptLocation.Y = szChange.Height = 0;

            return szChange;
        }

        /// <summary>
        /// Gets the rotation angle.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The rotation angle.</returns>
        private float GetRotationAngle(Node node)
        {
            float angle = node.RotationAngle;

            angle = Geometry.ConvertToFullCircle(angle);

            // Flip angle value.
            FlipValue(node, ref angle);

            return angle;
        }

        /// <summary>
        /// Gets the node aspect ratio.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>true, if get node aspect ratio</returns>
        private bool GetNodeAspectRatio(Node node)
        {
            bool bResult = false;

            if (node.EditStyle.AspectRatio)
            {
                if (!node.EditStyle.AllowChangeWidth || !node.EditStyle.AllowChangeHeight)
                    throw new ArgumentException("Can't resize ratio, AllowChangeWidth or AllowChangeHeight is disable");

                bResult = true;
            }
            else
            {
                ICompositeNode compositeNode = this.RenderHelper as ICompositeNode;

                if (compositeNode != null)
                    bResult = !HandlesHitTesting.CanResizeChildren(compositeNode);
            }

            return bResult;
        }
        #endregion

        #region Resize handles
        /// <summary>
        /// Considers the handle hit.
        /// </summary>
        /// <param name="ptLocation">The point location.</param>
        /// <param name="szChange">The size change.</param>
        private void ConsiderHandleHit(ref PointF ptLocation, ref SizeF szChange)
        {
            // Change location and size by current handle
            switch (m_handleHit)
            {
                case BoxPosition.TopCenter:
                    ResizeOnTopCenterHandle(ref ptLocation, ref szChange);
                    break;
                case BoxPosition.TopRight:
                    ResizeOnTopRightHandle(ref ptLocation, ref szChange);
                    break;
                case BoxPosition.MiddleRight:
                    ResizeOnMiddleRightHandle(ref szChange);
                    break;
                case BoxPosition.BottomCenter:
                    ResizeOnBottomCenterHandle(ref szChange);
                    break;
                case BoxPosition.BottomLeft:
                    ResizeOnBottomLeftHandle(ref ptLocation, ref szChange);
                    break;
                case BoxPosition.MiddleLeft:
                    ResizeOnMiddleLeftHandle(ref ptLocation, ref szChange);
                    break;
                case BoxPosition.TopLeft:
                    ResizeOnTopLeftHandle(ref ptLocation, ref szChange);
                    break;
                case BoxPosition.BottomRight:
                    ResizeOnBottomRightHandle(ref szChange);
                    break;
            }
        }

        /// <summary>
        /// Resizes the on top center handle.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szChange">The size.</param>
        private void ResizeOnTopCenterHandle(ref PointF ptLocation, ref SizeF szChange)
        {
            szChange.Width = 0;
            ptLocation.Y += szChange.Height;
            szChange.Height = -szChange.Height;

            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on top right handle.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szChange">The size.</param>
        private void ResizeOnTopRightHandle(ref PointF ptLocation, ref SizeF szChange)
        {
            // Check for aspect ratio flag
            ptLocation.Y += szChange.Height;
            szChange.Height = -szChange.Height;

            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on middle right handle.
        /// </summary>
        /// <param name="szChange">The size.</param>
        private void ResizeOnMiddleRightHandle(ref SizeF szChange)
        {
            // Check for aspect ratio flag
            szChange.Height = 0;

            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on bottom center handle.
        /// </summary>
        /// <param name="szChange">The size.</param>
        private void ResizeOnBottomCenterHandle(ref SizeF szChange)
        {
            // Check for aspect ratio flag
            szChange.Width = 0;

            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on bottom left handle.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szChange">The size.</param>
        private void ResizeOnBottomLeftHandle(ref PointF ptLocation, ref SizeF szChange)
        {
            ptLocation.X += szChange.Width;
            szChange.Width = -szChange.Width;

            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on middle left handle.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szChange">The size.</param>
        private void ResizeOnMiddleLeftHandle(ref PointF ptLocation, ref SizeF szChange)
        {
            // Check for aspect ratio flag
            szChange.Height = 0;
            ptLocation.X += szChange.Width;
            szChange.Width = -szChange.Width;

            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on top left handle.
        /// </summary>
        /// <param name="ptLocation">The location.</param>
        /// <param name="szChange">The size.</param>
        private void ResizeOnTopLeftHandle(ref PointF ptLocation, ref SizeF szChange)
        {
            // Check for aspect ratio flag
            ptLocation.X += szChange.Width;
            ptLocation.Y += szChange.Height;
            szChange.Width = -szChange.Width;
            szChange.Height = -szChange.Height;

            UpdateOriginalResize(szChange.Width, szChange.Height);
        }

        /// <summary>
        /// Resizes the on bottom right handle.
        /// </summary>
        /// <param name="szChange">The size.</param>
        private void ResizeOnBottomRightHandle(ref SizeF szChange)
        {
            // update original size offset.
            UpdateOriginalResize(szChange.Width, szChange.Height);
        }
        private void UpdateOriginalResize(float width, float height)
        {
            // update original size offset.
            m_szOriginalResize.Width += width;
            m_szOriginalResize.Height += height;
        }

        /// <summary>
        /// Completes the tool action.
        /// </summary>
        private void CompleteAction()
        {
            bool bBoundaryConstraintsEnable = this.Controller.Model.BoundaryConstraintsEnabled;

            SizeF szOldPinPointOffset = m_nodeResizing.PinPointOffset;
            PointF ptOldPinPoint = m_nodeResizing.PinPoint;

            SizeF szPinPointOffset = ((IUnitIndependent)this.RenderHelper).GetPinPointOffset(this.RenderHelper.MeasurementUnit);
            PointF ptPinPoint = ((IUnitIndependent)this.RenderHelper).GetPinPoint(this.RenderHelper.MeasurementUnit);
            SizeF szSize = ((IUnitIndependent)this.RenderHelper).GetSize(this.RenderHelper.MeasurementUnit);

            // Quit disable boundary constrains
            QuiteBoundarySet(false);
            // update pin 
            ((IUnitIndependent)m_nodeResizing).SetPinPoint(ptPinPoint, this.RenderHelper.MeasurementUnit);
            // update size
            ((IUnitIndependent)m_nodeResizing).SetSize(szSize, this.RenderHelper.MeasurementUnit);
            if (m_nodeResizing.Size != szSize)
            {
                szPinPointOffset = m_nodeResizing.PinPointOffset;
            }
            // update pin offset
            ((IUnitIndependent)m_nodeResizing).SetPinPointOffset(szPinPointOffset, this.RenderHelper.MeasurementUnit);
            // Quit restore boundary constrains
            QuiteBoundarySet(bBoundaryConstraintsEnable);
        }

        #endregion
    }
}
