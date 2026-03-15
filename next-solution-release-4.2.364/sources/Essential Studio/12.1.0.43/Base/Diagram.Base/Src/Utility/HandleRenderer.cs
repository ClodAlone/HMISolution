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
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Renderer used to render the handle.
    /// </summary>
    public class HandleRenderer
    {
        private Color m_handleColor = Color.GreenYellow;
        private Color m_handleDisabledColor = Color.Gray;
        private Color m_handleOutlineColor = Color.Black;
        private SolidBrush m_sbrushHandle;
        private Pen m_spenHandleOutline;

        #region Class properties
        /// <summary>
        /// Gets or sets the color of the handle.
        /// </summary>
        /// <value>The color of the handle.</value>
        public Color HandleColor
        {
            get
            {
                return m_handleColor;
            }
            set
            {
                if (value != m_handleColor)
                {
                    m_handleColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the handle outline.
        /// </summary>
        /// <value>The color of the handle outline.</value>
        public Color HandleOutlineColor
        {
            get
            {
                return m_handleOutlineColor;
            }
            set
            {
                if (value != m_handleOutlineColor)
                {
                    m_handleOutlineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the handle disabled.
        /// </summary>
        /// <value>The color of the handle disabled.</value>
        public Color HandleDisabledColor
        {
            get
            {
                return m_handleDisabledColor;
            }
            set
            {
                if (value != m_handleDisabledColor)
                {
                    m_handleDisabledColor = value;
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="HandleRenderer"/> class.
        /// </summary>
        public HandleRenderer()
        {
            m_sbrushHandle = new SolidBrush(m_handleColor);
            m_spenHandleOutline = new Pen(m_handleOutlineColor, 0f);

        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Outlines the bounding rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        public void OutlineBoundingRectangle(Graphics gfx, Node node)
        {
            OutlineBoundingRectangle(gfx, node, false);
        }

        /// <summary>
        /// Outlines the bounding rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> to draw outline bounds without rotation.</param>
        public void OutlineBoundingRectangle(Graphics gfx, Node node, bool bWithoutRotation)
        {
            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            Matrix matrixTemp = node.GetTransformations();
            RectangleF rcBoundingRectangle = RectangleF.Empty;

            if (bWithoutRotation)
            {
                // get PinPoint and PinOffset
                PointF[] pts = node.GraphicsPath.PathPoints;
                matrixTemp.TransformPoints(pts);
                RectangleF rcBounds = Geometry.CreateRect(pts);

                // Translate pin on it offsets
                matrixTemp = new Matrix(1, 0, 0, 1, rcBounds.X, rcBounds.Y);
                rcBoundingRectangle.Size = rcBounds.Size;
            }
            else
            {
                rcBoundingRectangle.Size = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
            }

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // 4 - Outline shape's bounding rectangle
            OutlineBoundingRectangle(gfx, rcBoundingRectangle, matrixTemp);
        }

        /// <summary>
        /// Outlines the bounding rectangle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="rcBoundingRectangle">The bounding rectangle.</param>
        /// <param name="mtxTransfrom">Matrix transformations.</param>
        public void OutlineBoundingRectangle(Graphics gfx, RectangleF rcBoundingRectangle, Matrix mtxTransfrom)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            gfx.MultiplyTransform(mtxTransfrom);

            // static pen is not used here as OutOfMemoryExceptions might occur some times
            using (Pen pen = new Pen(HandleColor, 1f / gfx.PageScale))
            {
                pen.DashStyle = DashStyle.Solid;
                gfx.DrawRectangle(pen, 0, 0, rcBoundingRectangle.Width, rcBoundingRectangle.Height);
            }

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Outlines the segments line.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        public void OutlineSegmentsLine(Graphics gfx, PathNode node)
        {
            // 1 - Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            Matrix matrixTemp = node.GetTransformations();

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // Get vertex points.
            PointF[] pathPoints = node.GetPoints();
            matrixTemp.TransformPoints(pathPoints);

            // 3 - update pen's properties to ouline shape's bounding rectangle
            m_spenHandleOutline.Color = HandleColor;
            m_spenHandleOutline.DashStyle = DashStyle.Dash;
            m_spenHandleOutline.Width = 1f / gfx.PageScale;

            // 4 - Outline shape's segments
            gfx.DrawPolygon(m_spenHandleOutline, pathPoints);

            // 5 - Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Draws the selection handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawSelectionHandles(Graphics gfx, Node node)
        {
            DrawSelectionHandles(gfx, node, false);
        }

        /// <summary>
        /// Draws the selection handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> handles will draw on outline.</param>
        public void DrawSelectionHandles(Graphics gfx, Node node, bool bWithoutRotation)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // apply node transformations to graphics
            Matrix matrixTemp = CreateParentMatrix(node);
            gfx.MultiplyTransform(matrixTemp);

            // get node local transformations
            Matrix matrixLocalTransform = bWithoutRotation ? new Matrix() : node.GetLocalTransformations();
            node.AppendLocalFlipTransforms(matrixLocalTransform);

            // reset matrix
            matrixTemp.Reset();

            // append node local transformations               
            matrixTemp.Multiply(matrixLocalTransform);

            DrawResizeHandles(gfx, node, matrixTemp, bWithoutRotation);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Renders selection/resize handles.
        /// Override this method to change appearance and positioning of selection/resize handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="rcBoundary">Rectangle which has all node elements (ports, labels, etc).</param>
        /// <param name="mtxTransform">Transformation matrix</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> to skip rotation transformation.</param>
        public void DrawSelectionHandles(Graphics gfx, Node node, RectangleF rcBoundary, Matrix mtxTransform, bool bWithoutRotation)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            float fParentsRotation = bWithoutRotation ? 0 : HandlesHitTesting.GetParentsRotation(node);

            DrawResizeHandles(gfx, node, rcBoundary, mtxTransform, fParentsRotation);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Draws the rotation handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawRotationHandles(Graphics gfx, Node node)
        {
            DrawPinPoint(gfx, node);
            DrawRotationHandle(gfx, node);
        }

        /// <summary>
        /// Draws the rotation handle.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawRotationHandle(Graphics gfx, Node node)
        {
            if (!node.EditStyle.HideRotationHandle)
            {
                DrawRotationPrimitive(gfx, node, HandlePrimitive.RotationHandle);
            }
        }

        /// <summary>
        /// Draws the pin point.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawPinPoint(Graphics gfx, Node node)
        {
            if (!node.EditStyle.HidePinPoint)
            {
                DrawRotationPrimitive(gfx, node, HandlePrimitive.PinPoint);
            }
        }

        /// <summary>
        /// Draws the end points.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="endPointContainer">The end point container.</param>
        public void DrawEndPoints(Graphics gfx, Node endPointContainer)
        {
            if (endPointContainer == null && !(endPointContainer is IEndPointContainer))
                throw new ArgumentNullException("endPointRenderring");

            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // get node's dimensions in unit independent values
            EndPoint headEndPoint = ((IEndPointContainer)endPointContainer).HeadEndPoint;
            EndPoint tailEndPoint = ((IEndPointContainer)endPointContainer).TailEndPoint;

            // Create handle's bounding rectangle.
            DrawEndPoint(gfx, headEndPoint, headEndPoint.Location);
            DrawEndPoint(gfx, tailEndPoint, tailEndPoint.Location);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Draw endPoint handle in specific position.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="endPoint">EndPoint handle.</param>
        /// <param name="ptPosition">EndPoint position.</param>
        public void DrawEndPoint(Graphics gfx, EndPoint endPoint, PointF ptPosition)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize = 0;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = CommonUsedValues.END_POINT_HANDLE_TOUCH_SIZE / fPageScale;
            else
                fHandleSize = CommonUsedValues.END_POINT_HANDLE_SIZE / fPageScale;

            RectangleF rectHandle = Geometry.CreateRect(ptPosition, new SizeF(fHandleSize, fHandleSize));

            // Create brush to fill handle interiors
            using (SolidBrush brushHandleFill = new SolidBrush(HandleColor))
            {
                // if endpoint is connected with port -- fill its interiors with red
                if (endPoint.Port != null)
                {
                    brushHandleFill.Color = Color.Red;
                }

                if (!endPoint.AllowMoveX && !endPoint.AllowMoveY)
                    brushHandleFill.Color = HandleDisabledColor;

                // Draw head end point 
                DrawEndPointIteriors(gfx, rectHandle, brushHandleFill);

                if (endPoint is HeadEndPoint)
                {
                    DrawHeadEndPoint(gfx, rectHandle);
                }
                else
                {
                    DrawTailEndPoint(gfx, rectHandle);
                }
            }
        }

        /// <summary>
        /// Draws the vertex handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawVertexHandles(Graphics gfx, PathNode node)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            PointF[] pathPoints = node.GetPoints();

            Matrix mtxTransformations = node.GetTransformations();
            node.AppendFlipTransforms(mtxTransformations);

            mtxTransformations.TransformPoints(pathPoints);
            PointF ptVertex;

            // update pen's properties to draw vertexes
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;
            m_spenHandleOutline.Width = 1f / gfx.PageScale;

            for (int i = 0, len = pathPoints.Length; i < len; i++)
            {
                ptVertex = pathPoints[i];

                DrawVertexHandle(gfx, m_sbrushHandle, m_spenHandleOutline, ptVertex);
            }

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Gets the handle position.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The node.</param>
        /// <returns>The point</returns>
        public PointF GetHandlePosition(BoxPosition handle, Node node)
        {
            PointF ptHandleLocation = PointF.Empty;

            // get given node unit independent size value
            SizeF szNodeSize = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);

            RectangleF nodeBounds = new RectangleF(PointF.Empty, szNodeSize);
            return GetHandlePosition(handle, nodeBounds);
        }

        /// <summary>
        /// Gets the handle position.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="nodeBounds">The node bounds.</param>
        /// <returns>The handle position.</returns>
        public PointF GetHandlePosition(BoxPosition handle, RectangleF nodeBounds)
        {
            PointF ptHandleLocation = PointF.Empty;

            switch (handle)
            {
                case BoxPosition.TopLeft:
                    ptHandleLocation = nodeBounds.Location;
                    break;
                case BoxPosition.TopCenter:
                    ptHandleLocation = new PointF(nodeBounds.X + (nodeBounds.Width / 2), nodeBounds.Y);
                    break;
                case BoxPosition.TopRight:
                    ptHandleLocation = new PointF(nodeBounds.X + nodeBounds.Width, nodeBounds.Y);
                    break;
                case BoxPosition.MiddleLeft:
                    ptHandleLocation = new PointF(nodeBounds.X, nodeBounds.Y + (nodeBounds.Height / 2));
                    break;
                case BoxPosition.MiddleRight:
                    ptHandleLocation = new PointF(nodeBounds.X + nodeBounds.Width, nodeBounds.Y + (nodeBounds.Height / 2));
                    break;
                case BoxPosition.BottomLeft:
                    ptHandleLocation = new PointF(nodeBounds.X, nodeBounds.Y + nodeBounds.Height);
                    break;
                case BoxPosition.BottomCenter:
                    ptHandleLocation = new PointF(nodeBounds.X + (nodeBounds.Width / 2), nodeBounds.Y + nodeBounds.Height);
                    break;
                case BoxPosition.BottomRight:
                    ptHandleLocation = new PointF(nodeBounds.X + nodeBounds.Width, nodeBounds.Y + nodeBounds.Height);
                    break;
            }

            return new PointF(ptHandleLocation.X, ptHandleLocation.Y);
        }

        /// <summary>
        /// Gets the bounding rect.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The bounding rect.</returns>
        public System.Drawing.Rectangle GetBoundingRect(NodeCollection nodes)
        {
            System.Drawing.Rectangle rectTrackerToReturn = new System.Drawing.Rectangle(
                Int32.MaxValue, Int32.MaxValue, Int32.MinValue, Int32.MinValue);

            foreach (Node nodeCur in nodes)
            {
                UpdateBounds(nodeCur, ref rectTrackerToReturn);
            }

            return rectTrackerToReturn;
        }

        /// <summary>
        /// Creates the parent matrix.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The matrix.</returns>
        public Matrix CreateParentMatrix(Node node)
        {
            // get node's dimensions in unit independent values
            PointF ptUnitIndependentPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);
            SizeF sztransf = new SizeF(
                ptUnitIndependentPinPoint.X - szUnitIndependentPinOffset.Width,
                ptUnitIndependentPinPoint.Y - szUnitIndependentPinOffset.Height);

            // Create matrix.
            Matrix matrixTemp = new Matrix();

            // Translate to pinPoint.
            matrixTemp.Translate(sztransf.Width, sztransf.Height);

            return matrixTemp;
        }

        /// <summary>
        /// Updates the bounds.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="rectUpdating">The rect updating.</param>
        /// <summary>
        /// Create matrix with append parent's
        /// transformations on given graphics - WITHOUT ROTATIONS        
        /// </summary>
        private void UpdateBounds(Node node, ref System.Drawing.Rectangle rectUpdating)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            GraphicsPath path = (GraphicsPath)node.GraphicsPath.Clone();

            RectangleF rectBounding = path.GetBounds(node.GetTransformations());
            float fShapeX = rectBounding.X;
            float fShapeY = rectBounding.Y;

            int fShapeRight = (int)(rectBounding.Right + node.LineStyle.LineWidth);
            int fShapeBottom = (int)(rectBounding.Bottom + node.LineStyle.LineWidth);

            if (fShapeX < rectUpdating.X)
            {
                rectUpdating.Width = rectUpdating.Width + (int)(rectUpdating.X - fShapeX);
                rectUpdating.X = (int)fShapeX;
            }

            if (fShapeY < rectUpdating.Top)
            {
                rectUpdating.Height = rectUpdating.Height + (int)(rectUpdating.Y - fShapeY);
                rectUpdating.Y = (int)fShapeY;
            }

            if (fShapeRight > rectUpdating.Right)
                rectUpdating.Width = fShapeRight - rectUpdating.X;

            if (fShapeBottom > rectUpdating.Bottom)
                rectUpdating.Height = fShapeBottom - rectUpdating.Y;
        }

        /// <summary>
        /// General rendering method.
        /// Override this method to fully change handles rendering.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="handleEditMode">The handle edit mode.</param>
        /// <param name="node">The node.</param>
        public virtual void Render(Graphics grfx, HandleEditMode handleEditMode, Node node)
        {
            switch (handleEditMode)
            {
                case HandleEditMode.Resize:
                    if (node.ShowResizeHandles())
                    {
                        this.OutlineBoundingRectangle(grfx, node);
                        this.DrawRotationHandles(grfx, node);
                        this.DrawSelectionHandles(grfx, node);
                    }
                    else
                    {
                        this.OutlineBoundingRectangle(grfx, node);
                        if (node is IEndPointContainer)
                            this.DrawEndPoints(grfx, node);
                    }
                    break;
                case HandleEditMode.Vertex:
                    PathNode pathNode = node as PathNode;
                    if (node is IEndPointContainer)
                    {
                        this.DrawEndPoints(grfx, node);
                        this.OutlineBoundingRectangle(grfx, node);

                        if (pathNode.CanDrawControlPoints())
                        {
                            pathNode.DrawControlPoints(grfx);
                        }
                    }
                    else if (pathNode != null && (pathNode.CanChangePath || pathNode.IsVertexEditable))
                    {
                        this.OutlineBoundingRectangle(grfx, pathNode);

                        // Draw control points if can.
                        if (pathNode.CanDrawControlPoints())
                        {
                            pathNode.DrawControlPoints(grfx);
                        }
                        else
                        {
                            this.DrawVertexHandles(grfx, pathNode);
                        }
                    }
                    break;
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Draws the vertex handle.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="brushVertextFill">Brush used to fill vertex interior.</param>
        /// <param name="penVertexOutline">Pen used to draw vertex outline.</param>
        /// <param name="ptHandle">The rect handle.</param>
        public static void DrawVertexHandle(Graphics gfx, Brush brushVertextFill, Pen penVertexOutline, PointF ptHandle)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleHalfSize ;

            if (HandlesHitTesting.TouchMode)
                fHandleHalfSize = (CommonUsedValues.END_POINT_HANDLE_TOUCH_SIZE / fPageScale) / 2;
            else
                fHandleHalfSize = (CommonUsedValues.END_POINT_HANDLE_SIZE / fPageScale) / 2;

            PointF[] pts = new PointF[4]
            {
                new PointF( ptHandle.X, ptHandle.Y - fHandleHalfSize ),
                new PointF( ptHandle.X + fHandleHalfSize, ptHandle.Y),
                new PointF( ptHandle.X, ptHandle.Y + fHandleHalfSize ),
                new PointF( ptHandle.X - fHandleHalfSize, ptHandle.Y )
            };

            // Fill handle interiors
            gfx.FillPolygon(brushVertextFill, pts);

            // Outline handle
            gfx.DrawPolygon(penVertexOutline, pts);
        }

        /// <summary>
        /// Draws the resize handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="matrixTransform">The matrix transform.</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> handles will rotate around pin line other shape points.</param>
        private void DrawResizeHandles(Graphics gfx, Node node, Matrix matrixTransform, bool bWithoutRotation)
        {
            RectangleF rcNodeBounds = RectangleF.Empty;
            float fParentsRotation = 0;

            if (bWithoutRotation)
            {
                PointF[] ptsPoints = node.GraphicsPath.PathPoints;
                Matrix mtxLocal = node.GetLocalTransformations();
                node.AppendLocalFlipTransforms(mtxLocal);
                mtxLocal.TransformPoints(ptsPoints);
                rcNodeBounds = Geometry.CreateRect(ptsPoints);
            }
            else
            {
                SizeF szNodeSize = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
                rcNodeBounds = new RectangleF(PointF.Empty, szNodeSize);
                fParentsRotation = HandlesHitTesting.GetParentsRotation(node);
            }

            DrawResizeHandles(gfx, node, rcNodeBounds, matrixTransform, fParentsRotation);
        }
        private void DrawResizeHandles(Graphics gfx, Node node, RectangleF rcBoundary, Matrix matrixTransform, float fParentsRotation)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize ;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / fPageScale) ;
            else
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_SIZE / fPageScale) ;

            // Draw handles
            Array handles = Enum.GetValues(typeof(BoxPosition));
            RectangleF rectHandle;

            // PointF array for transforming.
            PointF[] pts = new PointF[1];
            Matrix matrixParentRotate;

            foreach (BoxPosition handle in handles)
            {
                if (handle != BoxPosition.Center)
                {
                    // Get handle position
                    pts[0] = GetHandlePosition(handle, rcBoundary);

                    // Transform points
                    matrixTransform.TransformPoints(pts);

                    // Create handle's bounding rectangle.
                    rectHandle = Geometry.CreateRect(pts[0], new SizeF(fHandleSize, fHandleSize));

                    GraphicsState stateSave = null;

                    if (fParentsRotation != 0)
                    {
                        stateSave = gfx.Save();
                        matrixParentRotate = new Matrix();
                        matrixParentRotate.RotateAt(-fParentsRotation, pts[0]);
                        gfx.MultiplyTransform(matrixParentRotate, MatrixOrder.Prepend);
                    }

                    DrawResizeHandle(gfx, handle, Enabled(handle, node), rectHandle);

                    if (fParentsRotation != 0)
                        gfx.Restore(stateSave);
                }
            }
        }

        /// <summary>
        /// Draws the rotation primitive.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="rotationPrimitive">The rotation primitive.</param>
        private void DrawRotationPrimitive(Graphics gfx, Node node, HandlePrimitive rotationPrimitive)
        {
            float fPageScale = gfx.PageScale;

            // 1 - Save Graphics state
            GraphicsState save = gfx.Save();

            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;

            // get node's dimensions in unit independent values
            PointF ptUnitIndependentPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            float dx = ptUnitIndependentPinPoint.X - szUnitIndependentPinOffset.Width;
            float dy = ptUnitIndependentPinPoint.Y - szUnitIndependentPinOffset.Height;
            Matrix matrixTemp = new Matrix(1, 0, 0, 1, dx, dy);

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // apply matrix to graphics
            gfx.MultiplyTransform(matrixTemp);

            // 3 - Create brush to fill PinPoint interiors
            m_spenHandleOutline.Width = 1f / fPageScale;
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;

            if ((rotationPrimitive == HandlePrimitive.RotationHandle && !node.EditStyle.AllowRotate) ||
                (rotationPrimitive == HandlePrimitive.PinPoint && !node.EditStyle.AllowMoveX && !node.EditStyle.AllowMoveY))
            {
                m_sbrushHandle.Color = HandleDisabledColor;
            }
            else
            {
                if (m_sbrushHandle.Color != HandleColor)
                    m_sbrushHandle.Color = HandleColor;
            }

            // 5 - Draw needed primitive 
            if (rotationPrimitive == HandlePrimitive.PinPoint)
                DrawPinPoint(gfx, node, m_sbrushHandle, m_spenHandleOutline);
            else if (rotationPrimitive == HandlePrimitive.RotationHandle)
                DrawRotationHandle(gfx, node, m_sbrushHandle, m_spenHandleOutline);

            // 6 - restore Graphics State
            gfx.Restore(save);
        }

        private void DrawTailEndPoint(Graphics gfx, RectangleF rectBounds)
        {
            m_spenHandleOutline.Width = 1f / gfx.PageScale;
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;

            float fOffset = rectBounds.Width / 4;
            
            // draw x
            gfx.DrawLine(
                m_spenHandleOutline,
                rectBounds.Left + fOffset, 
                rectBounds.Bottom - fOffset,
                rectBounds.Right - fOffset, 
                rectBounds.Top + fOffset);

            gfx.DrawLine(
                m_spenHandleOutline,
                rectBounds.Left + fOffset, 
                rectBounds.Top + fOffset,
                rectBounds.Right - fOffset, 
                rectBounds.Bottom - fOffset);
        }
        private void DrawHeadEndPoint(Graphics gfx, RectangleF rectBounds)
        {
            m_spenHandleOutline.Width = 1f / gfx.PageScale;
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;

            float fHalfWidth = rectBounds.Width / 2;
            float fQuaterWidth = rectBounds.Width / 4;

            // draw cross
            gfx.DrawLine(
                m_spenHandleOutline, 
                rectBounds.X + fQuaterWidth, 
                rectBounds.Y + fHalfWidth, 
                rectBounds.Right - fQuaterWidth, 
                rectBounds.Y + fHalfWidth);

            gfx.DrawLine(
                m_spenHandleOutline, 
                rectBounds.X + fHalfWidth, 
                rectBounds.Y + fQuaterWidth,
                rectBounds.X + fHalfWidth, 
                rectBounds.Bottom - fQuaterWidth);
        }
        private void DrawEndPointIteriors(Graphics gfx, RectangleF rectHandle, Brush brushFill)
        {
            // update pen to draw handle outline
            m_spenHandleOutline.Width = 1f / gfx.PageScale;
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;

            // Fill handle interiors
            gfx.FillRectangle(brushFill, rectHandle);

            // Outline handle
            gfx.DrawRectangle(m_spenHandleOutline, rectHandle.X, rectHandle.Y, rectHandle.Width, rectHandle.Height);
        }

        /// <summary>
        /// Draws the resize handle.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="bEnable">if set to <c>true</c> to draw activate handle, otherwise - <b>false</b>.</param>
        /// <param name="rectHandle">The rect handle.</param>
        private void DrawResizeHandle(Graphics gfx, BoxPosition handle, bool bEnable, RectangleF rectHandle)
        {
            m_spenHandleOutline.Width = 1f / gfx.PageScale;
            m_spenHandleOutline.DashStyle = DashStyle.Solid;
            m_spenHandleOutline.Color = HandleOutlineColor;

            // Check handle state
            if (!bEnable)
                m_sbrushHandle.Color = HandleDisabledColor;
            else
                if (m_sbrushHandle.Color != HandleColor)
                    m_sbrushHandle.Color = HandleColor;

            // Fill handle interiors
            gfx.FillRectangle(m_sbrushHandle, rectHandle);

            // Outline handle
            gfx.DrawRectangle(m_spenHandleOutline, rectHandle.X, rectHandle.Y, rectHandle.Width, rectHandle.Height);
        }

        /// <summary>
        /// Draws the pin point.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="brushPinPoint">The brush pin point.</param>
        /// <param name="penHandleOutline">The pen handle outline.</param>
        private static void DrawPinPoint(Graphics gfx, Node node, Brush brushPinPoint, Pen penHandleOutline)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleOffset, fHandleSize;

            if (HandlesHitTesting.TouchMode)
            {
                fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / 2f) / fPageScale;
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE) / fPageScale;
            }
            else
            {
                fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_SIZE / 2f) / fPageScale;
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_SIZE) / fPageScale;
            }
            // get node's dimensions in unit independent values
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            // 1 - Calc handle's rect
            RectangleF rect = new RectangleF(
                szUnitIndependentPinOffset.Width - fHandleOffset, 
                szUnitIndependentPinOffset.Height - fHandleOffset,
                fHandleSize, 
                fHandleSize);

            // 2 - Draw PinPoint Interiors
            using (GraphicsPath path = PathFactory.CreateArc(rect, 0, 360))
                gfx.FillPath(brushPinPoint, path);

            // 3 - Draw Inner cross
            gfx.DrawLine(
                penHandleOutline, 
                szUnitIndependentPinOffset.Width, 
                szUnitIndependentPinOffset.Height - 1,
                szUnitIndependentPinOffset.Width, 
                szUnitIndependentPinOffset.Height + 1);

            // 4 - Draw Inner cross
            gfx.DrawLine(
                penHandleOutline, 
                szUnitIndependentPinOffset.Width - 1, 
                szUnitIndependentPinOffset.Height,
                szUnitIndependentPinOffset.Width + 1, 
                szUnitIndependentPinOffset.Height);

            // 5 - Draw PinPoint outline
            gfx.DrawEllipse(
                penHandleOutline,
                szUnitIndependentPinOffset.Width - fHandleOffset, 
                szUnitIndependentPinOffset.Height - fHandleOffset,
                fHandleSize, 
                fHandleSize);
        }

        /// <summary>
        /// Draws the rotation handle.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="brushPinPoint">The brush pin point.</param>
        /// <param name="penHandleOutline">The pen handle outline.</param>
        private static void DrawRotationHandle(Graphics gfx, Node node, Brush brushPinPoint, Pen penHandleOutline)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = CommonUsedValues.ROTATION_HANDLE_TOUCH_SIZE / fPageScale;
            else
                fHandleSize = CommonUsedValues.ROTATION_HANDLE_SIZE / fPageScale;

            // get node's dimensions in unit independent values
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            float fRHO;
            if (HandlesHitTesting.TouchMode)
                fRHO = CommonUsedValues.ROTATION_HANDLE_TOUCH_OFFSET / fPageScale;
            else
                fRHO = CommonUsedValues.ROTATION_HANDLE_OFFSET / fPageScale;
            float fYOffset = (szUnitIndependentPinOffset.Height < 0) ? szUnitIndependentPinOffset.Height - fRHO : -fRHO;

            PointF ptRotationHandleLocation = new PointF(szUnitIndependentPinOffset.Width, fYOffset);

            // PointF array for transforming.
            PointF[] pts = new PointF[] { ptRotationHandleLocation };

            Matrix matrixTemp = new Matrix();

            // Get parents rotation angle
            matrixTemp.RotateAt(node.RotationAngle, new PointF(szUnitIndependentPinOffset.Width, szUnitIndependentPinOffset.Height), MatrixOrder.Append);

            // Transform points
            matrixTemp.TransformPoints(pts);

            // Create handle's bounding rectangle.
            RectangleF rect = Geometry.CreateRect(pts[0], new SizeF(fHandleSize, fHandleSize));

            using (GraphicsPath path = PathFactory.CreateArc(rect, 0, 360))
            {
                // 1 - Draw RotationHandle Interiors
                gfx.FillPath(brushPinPoint, path);

                // 2 - Draw PinPoint outline
                gfx.DrawPath(penHandleOutline, path);
            }
        }

        /// <summary>
        /// Enables the specified handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The shape.</param>
        /// <returns>true, if handle is enabled.</returns>
        private static bool Enabled(BoxPosition handle, Node node)
        {
            bool bValueToReturn = true;
            switch (handle)
            {
                case BoxPosition.TopLeft:
                case BoxPosition.TopRight:
                case BoxPosition.BottomLeft:
                case BoxPosition.BottomRight:
                    bValueToReturn = EditStyle.CanChangeWidth(node) && EditStyle.CanChangeHeight(node);
                    break;
                case BoxPosition.MiddleLeft:
                case BoxPosition.MiddleRight:
                    bValueToReturn = EditStyle.CanChangeWidth(node);
                    break;
                case BoxPosition.TopCenter:
                case BoxPosition.BottomCenter:
                    bValueToReturn = EditStyle.CanChangeHeight(node);
                    break;
            }

            return bValueToReturn;
        }
        #endregion
    }

    /// <summary>
    /// Helper class for rendering.
    /// </summary>
    public class RenderingHelper
    {
        #region Class extern methods
        /// <summary>
        /// API function CreateIconIndirect
        /// </summary>
        /// <param name="iconinfo">The icon info.</param>
        /// <returns>The IntPtr.</returns>
        [System.Runtime.InteropServices.DllImport("USER32.DLL")]
        private static extern IntPtr CreateIconIndirect(ref ICONINFO iconinfo);
        #endregion

        #region Class public methods
        /// <summary>
        /// Render node collection to image.
        /// </summary>
        /// <param name="nodes">The nodes to draw.</param>
        /// <returns>Metafile image.</returns>
        public static Image RenderToImage(NodeCollection nodes)
        {
            return RenderToImage(nodes, new RenderingStyle());
        }

        /// <summary>
        /// Render given node collection to image.
        /// </summary>
        /// <param name="nodes">The nodes to draw.</param>
        /// <param name="styleRender">The render style.</param>
        /// <returns>Metafile image.</returns>
        public static Image RenderToImage(NodeCollection nodes, RenderingStyle styleRender)
        {
            // Get nodes bounding rectangle.
            RectangleF rectTemp = GetBoundingRectangle(nodes, MeasureUnits.Pixel, true);
            System.Drawing.Rectangle rectBounding = Geometry.ConvertRectangle(rectTemp);
            System.Drawing.Imaging.Metafile metafile;

            // render nodes to metafile
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();

                // create metafile to return
                IntPtr hdc = gfxBmp.GetHdc();
                metafile = new System.Drawing.Imaging.Metafile(stream, hdc);

                // create graphics to draw on
                using (Graphics gfxmeta = Graphics.FromImage(metafile))
                {
                    gfxmeta.SmoothingMode = styleRender.SmoothingMode;
                    gfxmeta.TextRenderingHint = styleRender.TextRenderingHint;
                    gfxmeta.InterpolationMode = styleRender.InterpolationMode;

                    gfxmeta.TranslateTransform(-(rectBounding.Left), -(rectBounding.Top));
                    GraphicsState state;

                    foreach (Node node in nodes)
                    {
                        // save graphics state
                        state = gfxmeta.Save();

                        // append parent's transforms
                        Matrix matrixTemp = HandlesHitTesting.GetParentsTransformations(node);
                        gfxmeta.MultiplyTransform(matrixTemp, MatrixOrder.Append);

                        node.Draw(gfxmeta);

                        // restore graphics state
                        gfxmeta.Restore(state);
                    }
                }

                gfxBmp.ReleaseHdc(hdc);
            }

            return metafile;
        }

        /// <summary>
        /// Renders composite node using given rendering style.
        /// </summary>
        /// <param name="nodeComposite">The composite node.</param>
        /// <param name="styleRender">The render style.</param>
        /// <returns>Metafile image.</returns>
        public static Image RenderToImage(ICompositeNode nodeComposite, RenderingStyle styleRender)
        {
            // Get nodes bounding rectangle.
            NodeCollection nodes = GetNodes(nodeComposite);
            RectangleF rcBounding =
                ((IUnitIndependent)nodeComposite).GetBoundingRectangle(MeasureUnits.Pixel, false);
            RectangleF rectTemp = RectangleF.Empty;
            Node nodeTemp = nodeComposite as Node;

            if (nodeTemp != null)
            {
                rectTemp = nodeTemp.RefreshRect;
                rectTemp.Size = ((IUnitIndependent)nodeComposite).GetSize(MeasureUnits.Pixel);
                float fLineWidth = Math.Abs(rcBounding.X - rectTemp.X) / 2;
                rectTemp.Width += fLineWidth;
                rectTemp.Height += fLineWidth;
            }
            else
            {
                rectTemp = GetBoundingRectangle(nodes, MeasureUnits.Pixel);
            }

            rectTemp.X -= rcBounding.X;
            rectTemp.Y -= rcBounding.Y;

            // Render nodes to metafile.
            System.Drawing.Imaging.Metafile metaToReturn;

            // Render nodes to metaifle.
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics gfxBmp = Graphics.FromImage(bmp))
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();

                // create metafile to return
                IntPtr hdc = gfxBmp.GetHdc();
                metaToReturn = new System.Drawing.Imaging.Metafile(stream, hdc);

                // create graphics to draw on
                using (Graphics gph = Graphics.FromImage(metaToReturn))
                {
                    gph.SmoothingMode = styleRender.SmoothingMode;
                    gph.TextRenderingHint = styleRender.TextRenderingHint;
                    gph.InterpolationMode = styleRender.InterpolationMode;

                    if (rectTemp.X < 0)
                        gph.TranslateTransform(-rectTemp.X, 0, MatrixOrder.Append);

                    if (rectTemp.Y < 0)
                        gph.TranslateTransform(0, -rectTemp.Y, MatrixOrder.Append);

                    foreach (Node node in nodes)
                    {
                        node.Draw(gph);
                    }
                }
                gfxBmp.ReleaseHdc(hdc);
            }

            return metaToReturn;
        }

        /// <summary>
        /// Renders composite node
        /// </summary>
        /// <param name="nodeComposite">The node composite.</param>
        /// <returns>The image.</returns>
        public static Image RenderToImage(ICompositeNode nodeComposite)
        {
            return RenderToImage(nodeComposite, new RenderingStyle());
        }

        /// <summary>
        /// Creates a cursor from a bitmap.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <returns>The cursor.</returns>
        public static System.Windows.Forms.Cursor CreateCursor(System.Drawing.Bitmap bmp)
        {
            ICONINFO iconInfo = new ICONINFO();
            iconInfo.hbmMask = bmp.GetHbitmap();
            iconInfo.hbmColor = bmp.GetHbitmap();

            IntPtr handle = CreateIconIndirect(ref iconInfo);

            return new System.Windows.Forms.Cursor(handle);
        }

        /// <summary>
        /// Creates a cursor from a bitmap and combines it with another cursor.
        /// </summary>
        /// <param name="bmp">The bitmap.</param>
        /// <param name="prototype">The prototype cursor.</param>
        /// <returns>The cursor.</returns>
        public static System.Windows.Forms.Cursor CreateCursor(Bitmap bmp, System.Windows.Forms.Cursor prototype)
        {
            System.Windows.Forms.Cursor cursor = null;

            using (Bitmap bmpCursor = bmp.Clone() as Bitmap)
            {
                using (Graphics gfx = Graphics.FromImage(bmpCursor))
                {
                    prototype.Draw(gfx, new System.Drawing.Rectangle(Point.Empty, prototype.Size));
                }

                try
                {
                    ICONINFO iconInfo = new ICONINFO();
                    iconInfo.xHotspot = (uint)(bmpCursor.Width / 4);
                    iconInfo.yHotspot = (uint)(bmpCursor.Height / 4);
                    iconInfo.hbmMask = bmpCursor.GetHbitmap();
                    iconInfo.hbmColor = bmpCursor.GetHbitmap();

                    IntPtr handle = CreateIconIndirect(ref iconInfo);
                    cursor = new System.Windows.Forms.Cursor(handle);
                }
                catch 
                { 
                }
            }

            return cursor;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Calculate bounding rectangle of the given model.
        /// </summary>
        /// <param name="model">Model to calculate bouncing rectangle</param>
        /// <param name="measureUnits"><see cref="Syncfusion.Windows.Forms.Diagram.MeasureUnits"/> to convert bounding rectangle to.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF GetBoundingRectangle(Model model, MeasureUnits measureUnits)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            RectangleF rcRefresh = MeasureUnitsConverter.ToPixels(model.Bounds, model.MeasurementUnits);

            // cosider line width
            float fLineWidth = MeasureUnitsConverter.ConvertX(
                model.LineStyle.LineWidth,
                model.LineStyle.MeasureUnit,
                measureUnits);
            float fHalfLineWidth = fLineWidth / 2f;

            rcRefresh.X -= fHalfLineWidth;
            rcRefresh.Y -= fHalfLineWidth;
            rcRefresh.Width += fLineWidth;
            rcRefresh.Height += fLineWidth;

            // consider shadow if it is visible
            ShadowStyle styleShadow = model.ShadowStyle;

            float fOffsetX = MeasureUnitsConverter.ConvertX(styleShadow.OffsetX, styleShadow.MeasureUnit, model.MeasurementUnits);
            float fOffsetY = MeasureUnitsConverter.ConvertY(styleShadow.OffsetY, styleShadow.MeasureUnit, model.MeasurementUnits);

            RectangleF rcTemp = rcRefresh;
            rcTemp.Offset(fOffsetX, fOffsetY);

            rcRefresh = RectangleF.Union(rcRefresh, rcTemp);

            return MeasureUnitsConverter.Convert(rcRefresh, model.MeasurementUnits, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Calculate bounding rectangle of given nodes collection.
        /// </summary>
        /// <param name="nodes">Nodes collection to calculate bounding rectangle.</param>
        /// <param name="measureUnits"><see cref="Syncfusion.Windows.Forms.Diagram.MeasureUnits"/> to convert bounding rectangle to.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF GetBoundingRectangle(ICollection nodes, MeasureUnits measureUnits)
        {
            return GetBoundingRectangle(nodes, measureUnits, true);
        }

        /// <summary>
        /// Calculate bounding rectangle of given nodes collection.
        /// </summary>
        /// <param name="nodes">Nodes collection to calculate bounding rectangle.</param>
        /// <param name="measureUnits"><see cref="Syncfusion.Windows.Forms.Diagram.MeasureUnits"/> to convert bounding rectangle to.</param>
        /// <param name="considerShadow">Indicates that need to consider shadow.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF GetBoundingRectangle(ICollection nodes, MeasureUnits measureUnits, bool considerShadow)
        {
            RectangleF rectTrackerToReturn = new RectangleF(Int32.MaxValue, Int32.MaxValue, Int32.MinValue, Int32.MinValue);
            Node nodeTemp;

            foreach (object nodeCur in nodes)
            {
                nodeTemp = nodeCur as Node;

                if (nodeTemp != null)
                    UpdateBounds(nodeTemp, ref rectTrackerToReturn, measureUnits, considerShadow);
            }

            return rectTrackerToReturn;
        }

        /// <summary>
        /// Calculate bounding rectangle of the given node.
        /// </summary>
        /// <param name="node">Node to calculate bounding rectangle.</param>
        /// <param name="measureUnits"><see cref="Syncfusion.Windows.Forms.Diagram.MeasureUnits"/> to convert bounding rectangle to.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF GetBoundingRectangle(Node node, MeasureUnits measureUnits)
        {
            return GetBoundingRectangle(node, measureUnits, true);
        }

        /// <summary>
        /// Calculate bounding rectangle of the given node.
        /// </summary>
        /// <param name="node">Node to calculate bounding rectangle.</param>
        /// <param name="measureUnits"><see cref="Syncfusion.Windows.Forms.Diagram.MeasureUnits"/> to convert bounding rectangle to.</param>
        /// <param name="considerShadow">Indicates that need to consider shadow.</param>
        /// <returns>Bounding rectangle.</returns>
        public static RectangleF GetBoundingRectangle(Node node, MeasureUnits measureUnits, bool considerShadow)
        {
            RectangleF rcRefresh = node.RefreshRect;

            if (considerShadow)
            {
                // consider shadow if it is visible
                ShadowStyle styleShadow = node.ShadowStyle;

                float fOffsetX = MeasureUnitsConverter.ConvertX(styleShadow.OffsetX, styleShadow.MeasureUnit, node.MeasurementUnit);
                float fOffsetY = MeasureUnitsConverter.ConvertY(styleShadow.OffsetY, styleShadow.MeasureUnit, node.MeasurementUnit);

                RectangleF rcTemp = rcRefresh;
                rcTemp.Offset(fOffsetX, fOffsetY);

                rcRefresh = RectangleF.Union(rcRefresh, rcTemp);
            }

            // consider parent's transformations
            Matrix mtx = HandlesHitTesting.GetParentsTransformations(node);

            PointF[] pts = new PointF[] 
            { 
                rcRefresh.Location,
                new PointF( rcRefresh.X + rcRefresh.Width, rcRefresh.Y ),
                new PointF( rcRefresh.Right, rcRefresh.Bottom ),
                new PointF( rcRefresh.X, rcRefresh.Y + rcRefresh.Height ) 
            };

            mtx.TransformPoints(pts);

            rcRefresh = Geometry.CreateRect(pts);

            return rcRefresh;
        }

        /// <summary>
        /// Considers the handles.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="rectBounding">The bounding rectangle.</param>
        /// <param name="fMagnification">The magnification value.</param>
        /// <param name="measureUnits">The measure units.</param>
        public static void ConsiderHandles(Node node, ref RectangleF rectBounding, float fMagnification, MeasureUnits measureUnits)
        {
            // update magnify value
            fMagnification /= 100f;

            SizeF szNodeSize = ((IUnitIndependent)node).GetSize(measureUnits);
            RectangleF rcBounding = new RectangleF(PointF.Empty, szNodeSize);

            // RESIZE HANDLE
            HandleStyle styleHandle = View.HandleStyles.ResizeHandlesStyle;
            float fHandleSize = (int)styleHandle.HandleSize / fMagnification;
            fHandleSize = MeasureUnitsConverter.ConvertX(fHandleSize, styleHandle.MeasureUnit, measureUnits);
            float fHalfHandleSize = (float)Math.Ceiling(fHandleSize / 2f);

            rcBounding.Inflate(fHalfHandleSize, fHalfHandleSize);

            // PIN POINT
            styleHandle = View.HandleStyles.PinPointStyle;
            fHandleSize = (int)styleHandle.HandleSize / fMagnification;
            fHandleSize = MeasureUnitsConverter.ConvertX(fHandleSize, styleHandle.MeasureUnit, measureUnits);
            SizeF szPinOffset = ((IUnitIndependent)node).GetPinPointOffset(measureUnits);

            RectangleF rcTemp = Geometry.CreateRect(
                new PointF(szPinOffset.Width, szPinOffset.Height),
                new SizeF(fHandleSize, fHandleSize));

            rcBounding = RectangleF.Union(rcTemp, rcBounding);

            // ROTATION HANDLE
            styleHandle = View.HandleStyles.RotationHandleStyle;
            fHandleSize = (int)styleHandle.HandleSize / fMagnification;

            // ROTATION HANDLE OFFSET
            float fRHO;
            if (HandlesHitTesting.TouchMode)
                fRHO = CommonUsedValues.ROTATION_HANDLE_TOUCH_OFFSET / fMagnification;
            else
                fRHO = CommonUsedValues.ROTATION_HANDLE_OFFSET / fMagnification;

            float fYOffset = (szPinOffset.Height < 0)
                                ? szPinOffset.Height - fRHO : -fRHO;

            PointF ptRotationHandleLocation = new PointF(szPinOffset.Width, fYOffset);

            // Create handle's bounding rectangle.
            rcTemp = Geometry.CreateRect(ptRotationHandleLocation, new SizeF(fHandleSize, fHandleSize));
            rcBounding = RectangleF.Union(rcTemp, rcBounding);

            // OVERALL
            Matrix mtxTemp = node.GetTransformations();
            node.AppendFlipTransforms(mtxTemp);

            Matrix mtxParent = HandlesHitTesting.GetParentsTransformations(node);
            mtxTemp.Multiply(mtxParent, MatrixOrder.Append);

            PointF[] pts = new PointF[]
            {
                rcBounding.Location,
                new PointF( rcBounding.Right, rcBounding.Y ),
                new PointF( rcBounding.Right, rcBounding.Bottom ),
                new PointF( rcBounding.X, rcBounding.Bottom )
            };

            mtxTemp.TransformPoints(pts);
            rcTemp = Geometry.CreateRect(pts);

            rectBounding = RectangleF.Union(rectBounding, rcTemp);
        }
        #endregion

        #region Class helper methods
        private static NodeCollection GetNodes(ICompositeNode nodeComposite)
        {
            if (nodeComposite == null)
                throw new ArgumentNullException("nodeComposite");

            NodeCollection nodesToReturn = new NodeCollection();
            int nCounter = 0;
            int nLength = nodeComposite.ChildCount;
            Node nodeTemp;

            while (nCounter < nLength)
            {
                nodeTemp = nodeComposite.GetChild(nCounter);
                nodesToReturn.Add(nodeTemp);

                nCounter++;
            }

            return nodesToReturn;
        }
        private static void UpdateBounds(Node node, ref RectangleF rectUpdating, MeasureUnits measureUnits, bool considerShadow)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            RectangleF rectBounding = GetBoundingRectangle(node, measureUnits, considerShadow);

            if (rectBounding.X < rectUpdating.X)
            {
                rectUpdating.Width = rectUpdating.Width + (rectUpdating.X - rectBounding.X);
                rectUpdating.X = rectBounding.X;
            }

            if (rectBounding.Y < rectUpdating.Top)
            {
                rectUpdating.Height = rectUpdating.Height + (rectUpdating.Y - rectBounding.Y);
                rectUpdating.Y = rectBounding.Y;
            }

            if (rectBounding.Right > rectUpdating.Right)
            {
                rectUpdating.Width = rectBounding.Right - rectUpdating.X;
            }

            if (rectBounding.Bottom > rectUpdating.Bottom)
            {
                rectUpdating.Height = rectBounding.Bottom - rectUpdating.Y;
            }
        }
        #endregion

        #region Class helper struct
        /// <summary>
        /// API-Structure ICONINFO
        /// </summary>
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct ICONINFO
        {
            public bool fIcon;
            public uint xHotspot;
            public uint yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        #endregion
    }

    /// <summary>
    /// Base class for custom handle renderers
    /// </summary>
    public abstract class UserHandleRenderer
    {
        /// <summary>
        /// Handle color.
        /// </summary>
        protected Color m_handleColor = Color.GreenYellow;

        /// <summary>
        /// Handle disabled color.
        /// </summary>
        protected Color m_handleDisabledColor = Color.Gray;

        /// <summary>
        /// Handle outline color.
        /// </summary>
        protected Color m_handleOutlineColor = Color.Black;

        /// <summary>
        /// Renderer of custom handle renderer.
        /// </summary>
        private HandleRenderer handleRenderer = new HandleRenderer();

        #region Class properties
        /// <summary>
        /// Gets or sets the color of the handle.
        /// </summary>
        /// <value>The color of the handle.</value>
        public Color HandleColor
        {
            get
            {
                return m_handleColor;
            }
            set
            {
                if (value != m_handleColor)
                {
                    m_handleColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the handle outline.
        /// </summary>
        /// <value>The color of the handle outline.</value>
        public Color HandleOutlineColor
        {
            get
            {
                return m_handleOutlineColor;
            }
            set
            {
                if (value != m_handleOutlineColor)
                {
                    m_handleOutlineColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the handle disabled.
        /// </summary>
        /// <value>The color of the handle disabled.</value>
        public Color HandleDisabledColor
        {
            get
            {
                return m_handleDisabledColor;
            }
            set
            {
                if (value != m_handleDisabledColor)
                {
                    m_handleDisabledColor = value;
                }
            }
        }
        #endregion
        /// <summary>
        /// General rendering method.
        /// Override this method to fully change handles rendering.
        /// </summary>
        /// <param name="grfx">Graphics to draw on.</param>
        /// <param name="handleEditMode">The handle edit mode.</param>
        /// <param name="node">The node.</param>
        public virtual void Render(Graphics grfx, HandleEditMode handleEditMode, Node node)
        {
            switch (handleEditMode)
            {
                case HandleEditMode.Resize:
                    if (node is IEndPointContainer)
                    {
                        this.DrawEndPoints(grfx, node);
                        this.OutlineBoundingRectangle(grfx, node);
                    }
                    else
                    {
                        this.OutlineBoundingRectangle(grfx, node);
                        if (!node.EditStyle.HidePinPoint)
                        {
                            this.DrawPinPoint(grfx, node);
                        }
                        if (!node.EditStyle.HideRotationHandle)
                        {
                            this.DrawRotationHandle(grfx, node);
                        }
                        this.DrawSelectionHandles(grfx, node);
                    }
                    break;
                case HandleEditMode.Vertex:
                    PathNode pathNode = node as PathNode;
                    if (node is IEndPointContainer)
                    {
                        this.DrawEndPoints(grfx, node);
                        this.OutlineBoundingRectangle(grfx, node);

                        if (pathNode.CanDrawControlPoints())
                        {
                            pathNode.DrawControlPoints(grfx);
                        }
                    }
                    else if (pathNode != null && (pathNode.CanChangePath || pathNode.IsVertexEditable))
                    {
                        this.OutlineBoundingRectangle(grfx, pathNode);

                        // Draw control points if can.
                        if (pathNode.CanDrawControlPoints())
                        {
                            pathNode.DrawControlPoints(grfx);
                        }
                        else
                        {
                            this.DrawVertexHandles(grfx, pathNode);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// This method renders outlining rectangle for given node.
        /// Override this method to change appearance and positioning of outline.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        public virtual void OutlineBoundingRectangle(Graphics gfx, Node node)
        {
            OutlineBoundingRectangle(gfx, node, false);
        }

        /// <summary>
        /// This method renders outlining rectangle for given node.
        /// Override this method to change appearance and positioning of outline.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> to skip rotation transformation.</param>
        public virtual void OutlineBoundingRectangle(Graphics gfx, Node node, bool bWithoutRotation)
        {
            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            Matrix matrixTemp = node.GetTransformations();
            RectangleF rcBoundingRectangle = RectangleF.Empty;

            if (bWithoutRotation)
            {
                // get PinPoint and PinOffset
                PointF[] pts = node.GraphicsPath.PathPoints;
                matrixTemp.TransformPoints(pts);
                RectangleF rcBounds = Geometry.CreateRect(pts);

                // Translate pin on it offsets
                matrixTemp = new Matrix(1, 0, 0, 1, rcBounds.X, rcBounds.Y);
                rcBoundingRectangle.Size = rcBounds.Size;
            }
            else
            {
                rcBoundingRectangle.Size = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
            }

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // 4 - Outline shape's bounding rectangle
            OutlineBoundingRectangle(gfx, rcBoundingRectangle, matrixTemp);
        }

        /// <summary>
        /// This method renders outlining rectangle for given node.
        /// Override this method to change appearance and positioning of outline.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="rcBoundingRectangle">Rectangle which has all node elements (ports, labels, etc).</param>
        /// <param name="mtxTransfrom">Transformation matrix.</param>
        public virtual void OutlineBoundingRectangle(Graphics gfx, RectangleF rcBoundingRectangle, Matrix mtxTransfrom)
        {
            handleRenderer.OutlineBoundingRectangle(gfx, rcBoundingRectangle, mtxTransfrom);
        }

        /// <summary>
        /// Renders outlining rectangle.
        /// Override this method to change appearance of outline
        /// </summary>
        /// <param name="gfx">Target Graphics</param>
        /// <param name="size">size of outlining rectangle</param>
        public virtual void OnDrawOutlineRectangle(Graphics gfx, SizeF size)
        {
            using (Pen pen = new Pen(HandleColor, 1f / gfx.PageScale))
            {
                pen.DashStyle = DashStyle.Solid;
                gfx.DrawRectangle(pen, 0, 0, size.Width, size.Height);
            }
        }

        /// <summary>
        /// Outlines the segments line.
        /// Override this method to change appearance and positioning of outline.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public virtual void OutlineSegmentsLine(Graphics gfx, PathNode node)
        {
            // 1 - Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            Matrix matrixTemp = node.GetTransformations();

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // Get verrex points.
            PointF[] pathPoints = node.GetPoints();
            matrixTemp.TransformPoints(pathPoints);

            // 3 - drawing outline
            OnDrawSegmentOutline(gfx, pathPoints);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Renders segment outlining.
        /// Override this method to change appearance of segment outline
        /// </summary>
        /// <param name="gfx">Target Graphics</param>
        /// <param name="pathPoints">The segment path points.</param>
        public virtual void OnDrawSegmentOutline(Graphics gfx, PointF[] pathPoints)
        {
            using (Pen pen = new Pen(HandleOutlineColor))
            {
                pen.Color = HandleColor;
                pen.DashStyle = DashStyle.Dash;
                pen.Width = 1f / gfx.PageScale;

                // Outline shape's segments
                gfx.DrawPolygon(pen, pathPoints);
            }
        }

        /// <summary>
        /// Renders selection/resize handles.
        /// Override this method to change appearance and positioning of selection/resize handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public virtual void DrawSelectionHandles(Graphics gfx, Node node)
        {
            DrawSelectionHandles(gfx, node, false);
        }

        /// <summary>
        /// Renders selection/resize handles.
        /// Override this method to change appearance and positioning of selection/resize handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> to skip rotation transformation.</param>
        public virtual void DrawSelectionHandles(Graphics gfx, Node node, bool bWithoutRotation)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // apply node transformations to graphics
            Matrix matrixTemp = handleRenderer.CreateParentMatrix(node);
            gfx.MultiplyTransform(matrixTemp);

            // get node local transformations
            Matrix matrixLocalTransform = bWithoutRotation ? new Matrix() : node.GetLocalTransformations();
            node.AppendLocalFlipTransforms(matrixLocalTransform);

            // reset matrix
            matrixTemp.Reset();

            // append node local transformations               
            matrixTemp.Multiply(matrixLocalTransform);

            RectangleF rcNodeBounds = RectangleF.Empty;
            float fParentsRotation = 0;

            if (bWithoutRotation)
            {
                PointF[] ptsPoints = node.GraphicsPath.PathPoints;
                Matrix mtxLocal = node.GetLocalTransformations();
                node.AppendLocalFlipTransforms(mtxLocal);
                mtxLocal.TransformPoints(ptsPoints);
                rcNodeBounds = Geometry.CreateRect(ptsPoints);
            }
            else
            {
                SizeF szNodeSize = ((IUnitIndependent)node).GetSize(MeasureUnits.Pixel);
                rcNodeBounds = new RectangleF(PointF.Empty, szNodeSize);
                fParentsRotation = HandlesHitTesting.GetParentsRotation(node);
            }

            OnDrawResizeHandles(gfx, node, rcNodeBounds, matrixTemp, fParentsRotation);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Renders selection/resize handles.
        /// Override this method to change appearance and positioning of selection/resize handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        /// <param name="rcBoundary">Rectangle which has all node elements (ports, labels, etc).</param>
        /// <param name="mtxTransform">Transformation matrix</param>
        /// <param name="bWithoutRotation">if set to <c>true</c> to skip rotation transformation.</param>
        public virtual void DrawSelectionHandles(Graphics gfx, Node node, RectangleF rcBoundary, Matrix mtxTransform, bool bWithoutRotation)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            float fParentsRotation = bWithoutRotation ? 0 : HandlesHitTesting.GetParentsRotation(node);

            OnDrawResizeHandles(gfx, node, rcBoundary, mtxTransform, fParentsRotation);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Renders resize handle.
        /// Override this method to change appearance of resize handle.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        /// <param name="rcBoundary">The node bounding rectangle.</param>
        /// <param name="matrixTransform">Transformation matrix.</param>
        /// <param name="fParentsRotation">if set to <c>true</c> to skip rotation transformation.</param>
        public virtual void OnDrawResizeHandles(Graphics gfx, Node node, RectangleF rcBoundary, Matrix matrixTransform, float fParentsRotation)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / fPageScale;
            else
                fHandleSize = CommonUsedValues.RESIZE_HANDLE_SIZE / fPageScale;

            // Draw handles
            Array handles = Enum.GetValues(typeof(BoxPosition));
            RectangleF rectHandle;

            // PointF array for transforming.
            PointF[] pts = new PointF[1];
            Matrix matrixParentRotate;

            foreach (BoxPosition handle in handles)
            {
                if (handle != BoxPosition.Center)
                {
                    // Get handle position
                    pts[0] = handleRenderer.GetHandlePosition(handle, rcBoundary);

                    // Transform points
                    matrixTransform.TransformPoints(pts);

                    // Create handle's bounding rectangle.
                    rectHandle = Geometry.CreateRect(pts[0], new SizeF(fHandleSize, fHandleSize));

                    GraphicsState stateSave = null;

                    if (fParentsRotation != 0)
                    {
                        stateSave = gfx.Save();
                        matrixParentRotate = new Matrix();
                        matrixParentRotate.RotateAt(-fParentsRotation, pts[0]);
                        gfx.MultiplyTransform(matrixParentRotate, MatrixOrder.Prepend);
                    }

                    OnDrawResizeHandleShape(gfx, handle, node, rectHandle);

                    if (fParentsRotation != 0)
                        gfx.Restore(stateSave);
                }
            }
        }

        /// <summary>
        /// Renders resize handle.
        /// Override this method to change appearance of resize handle.
        /// </summary>
        /// <param name="gfx">Target Graphics</param>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The node.</param>
        /// <param name="rectHandle">The handle bounds.</param>
        public virtual void OnDrawResizeHandleShape(Graphics gfx, BoxPosition handle, Node node, RectangleF rectHandle)
        {
            using (Pen pen = new Pen(m_handleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;
                pen.Color = HandleOutlineColor;

                // Create brush to fill PinPoint interiors
                using (Brush brush =
                    (!Enabled(handle, node) ? new SolidBrush(HandleDisabledColor) :
                    new SolidBrush(HandleColor)))
                {
                    // Fill handle interiors
                    gfx.FillRectangle(brush, rectHandle);

                    // Outline handle
                    gfx.DrawRectangle(pen, rectHandle.X, rectHandle.Y, rectHandle.Width, rectHandle.Height);
                }
            }
        }

        /// <summary>
        /// Draws the rotation handle.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public virtual void DrawRotationHandle(Graphics gfx, Node node)
        {
            // 1 - Save Graphics state
            GraphicsState save = gfx.Save();

            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;

            // get node's dimensions in unit independent values
            PointF ptUnitIndependentPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            // 2 - append parent's transformations on given graphics - WITHOUT ROTATIONS
            float dx = ptUnitIndependentPinPoint.X - szUnitIndependentPinOffset.Width;
            float dy = ptUnitIndependentPinPoint.Y - szUnitIndependentPinOffset.Height;
            Matrix matrixTemp = new Matrix(1, 0, 0, 1, dx, dy);

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // apply matrix to graphics
            gfx.MultiplyTransform(matrixTemp);

            OnDrawRotationHandleInterior(gfx, node);

            // 5 - restore Graphics State
            gfx.Restore(save);
        }

        /// <summary>
        /// Draws the pin point interior itself.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public virtual void OnDrawRotationHandleInterior(Graphics gfx, Node node)
        {
            using (Pen pen = new Pen(m_handleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;
                pen.Color = HandleOutlineColor;

                // Create brush to fill PinPoint interiors
                using (Brush brush =
                    (!node.EditStyle.AllowRotate) ? new SolidBrush(HandleDisabledColor) :
                    new SolidBrush(HandleColor))
                {
                    // Draw needed primitive 
                    // Handles are immutable to scale
                    float fPageScale = gfx.PageScale;
                    float fHandleSize;
                    if (HandlesHitTesting.TouchMode)
                        fHandleSize = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / fPageScale;
                    else
                        fHandleSize = CommonUsedValues.ROTATION_HANDLE_SIZE / fPageScale;

                    // get node's dimensions in unit independent values
                    SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

                    float fRHO;
                    if (HandlesHitTesting.TouchMode)
                        fRHO = CommonUsedValues.ROTATION_HANDLE_TOUCH_OFFSET / fPageScale;
                    else
                        fRHO = CommonUsedValues.ROTATION_HANDLE_OFFSET / fPageScale;
                    float fYOffset = (szUnitIndependentPinOffset.Height < 0) ? szUnitIndependentPinOffset.Height - fRHO : -fRHO;

                    PointF ptRotationHandleLocation = new PointF(szUnitIndependentPinOffset.Width, fYOffset);

                    // PointF array for transforming.
                    PointF[] pts = new PointF[] { ptRotationHandleLocation };

                    Matrix matrixTemp = new Matrix();

                    // Get parents rotation angle
                    matrixTemp.RotateAt(node.RotationAngle, new PointF(szUnitIndependentPinOffset.Width, szUnitIndependentPinOffset.Height), MatrixOrder.Append);

                    // Transform points
                    matrixTemp.TransformPoints(pts);

                    // Create handle's bounding rectangle.
                    using (GraphicsPath path = CreateRotationHandleShape(pts[0], new SizeF(fHandleSize, fHandleSize)))
                    {
                        // 1 - Draw RotationHandle Interiors
                        gfx.FillPath(brush, path);

                        // 2 - Draw RotationHandle outline
                        gfx.DrawPath(pen, path);
                    }
                }
            }
        }

        /// <summary>
        /// Method creates and returns GraphicsPath that represents rotation handle
        /// Override this method to change rotationhandle appearance
        /// </summary>
        /// <param name="location">Location of rotation handle</param>
        /// <param name="handleSize">size of rotation handle</param>
        /// <returns>The graphics path.</returns>
        public virtual GraphicsPath CreateRotationHandleShape(PointF location, SizeF handleSize)
        {
            RectangleF rect = Geometry.CreateRect(location, handleSize);
            return PathFactory.CreateArc(rect, 0, 360);
        }

        /// <summary>
        /// Draws the pin point.
        /// Override this method to change appearance and positioning of pinpoint.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public void DrawPinPoint(Graphics gfx, Node node)
        {
            float fPageScale = gfx.PageScale;
            float fHandleOffset, fHandleSize;
            if (HandlesHitTesting.TouchMode)
            {
                fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / 2f) / fPageScale;
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE) / fPageScale;
            }
            else
            {
                fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_SIZE / 2f) / fPageScale;
                fHandleSize = (CommonUsedValues.RESIZE_HANDLE_SIZE) / fPageScale;
            }

            // Save Graphics state
            GraphicsState save = gfx.Save();

            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;

            // get node's dimensions in unit independent values
            PointF ptUnitIndependentPinPoint = ((IUnitIndependent)node).GetPinPoint(MeasureUnits.Pixel);
            SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

            // append parent's transformations on given graphics - WITHOUT ROTATIONS
            float dx = ptUnitIndependentPinPoint.X - szUnitIndependentPinOffset.Width;
            float dy = ptUnitIndependentPinPoint.Y - szUnitIndependentPinOffset.Height;
            Matrix matrixTemp = new Matrix(1, 0, 0, 1, dx, dy);

            // append flip transforms
            node.AppendFlipTransforms(matrixTemp);

            // apply matrix to graphics
            gfx.MultiplyTransform(matrixTemp);

            OnDrawPinPointShape(gfx, node);

            // restore Graphics State
            gfx.Restore(save);
        }

        /// <summary>
        /// Renders PinPoint shape.
        /// Override this method to change appearance of pinpoint.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="node">The node.</param>
        public virtual void OnDrawPinPointShape(Graphics gfx, Node node)
        {
            using (Pen pen = new Pen(HandleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;

                using (Brush brush =
                    (!node.EditStyle.AllowMoveX && !node.EditStyle.AllowMoveY) ?
                    new SolidBrush(HandleDisabledColor) :
                    new SolidBrush(HandleColor))
                {
                    float fPageScale = gfx.PageScale;
                    float fHandleOffset, fHandleSize;
                    if (HandlesHitTesting.TouchMode)
                    {
                        fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / 2f) / fPageScale;
                        fHandleSize = (CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE) / fPageScale;
                    }
                    else
                    {
                        fHandleOffset = (CommonUsedValues.RESIZE_HANDLE_SIZE / 2f) / fPageScale;
                        fHandleSize = (CommonUsedValues.RESIZE_HANDLE_SIZE) / fPageScale;
                    }
                    // get node's dimensions in unit independent values
                    SizeF szUnitIndependentPinOffset = ((IUnitIndependent)node).GetPinPointOffset(MeasureUnits.Pixel);

                    // 1 - Calc handle's rect
                    RectangleF rect = new RectangleF(
                        szUnitIndependentPinOffset.Width - fHandleOffset, 
                        szUnitIndependentPinOffset.Height - fHandleOffset,
                        fHandleSize, 
                        fHandleSize);

                    // 2 - Draw PinPoint Interiors
                    using (GraphicsPath path = PathFactory.CreateArc(rect, 0, 360))
                    {
                        gfx.FillPath(brush, path);

                        // 3 - Draw Inner cross
                        gfx.DrawLine(
                            pen, 
                            szUnitIndependentPinOffset.Width, 
                            szUnitIndependentPinOffset.Height - 1,
                            szUnitIndependentPinOffset.Width, 
                            szUnitIndependentPinOffset.Height + 1);

                        // 4 - Draw Inner cross
                        gfx.DrawLine(
                            pen, 
                            szUnitIndependentPinOffset.Width - 1, 
                            szUnitIndependentPinOffset.Height,
                            szUnitIndependentPinOffset.Width + 1, 
                            szUnitIndependentPinOffset.Height);

                        // 5 - Draw PinPoint outline
                        gfx.DrawEllipse(
                            pen,
                            szUnitIndependentPinOffset.Width - fHandleOffset, 
                            szUnitIndependentPinOffset.Height - fHandleOffset,
                            fHandleSize, 
                            fHandleSize);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the end points.
        /// Override this method to change appearance and positioning of endpoint.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="endPointContainer">The end point container.</param>
        public virtual void DrawEndPoints(Graphics gfx, Node endPointContainer)
        {
            if (endPointContainer == null && !(endPointContainer is IEndPointContainer))
                throw new ArgumentNullException("endPointRenderring");

            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            // get node's dimensions in unit independent values
            EndPoint headEndPoint = ((IEndPointContainer)endPointContainer).HeadEndPoint;
            EndPoint tailEndPoint = ((IEndPointContainer)endPointContainer).TailEndPoint;

            // Create handle's bounding rectangle.
            DrawEndPoint(gfx, headEndPoint, headEndPoint.Location);
            DrawEndPoint(gfx, tailEndPoint, tailEndPoint.Location);

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Draw endPoint handle in specific position.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="endPoint">EndPoint handle.</param>
        /// <param name="ptPosition">EndPoint position.</param>
        public virtual void DrawEndPoint(Graphics gfx, EndPoint endPoint, PointF ptPosition)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleSize;
            if (HandlesHitTesting.TouchMode)
                fHandleSize = CommonUsedValues.RESIZE_HANDLE_TOUCH_SIZE / fPageScale;
            else
                fHandleSize = CommonUsedValues.RESIZE_HANDLE_SIZE / fPageScale;

            RectangleF rectHandle = Geometry.CreateRect(ptPosition, new SizeF(fHandleSize, fHandleSize));

            // Draw head end point 
            OnDrawEndPointOutline(gfx, rectHandle, endPoint);

            if (endPoint is HeadEndPoint)
            {
                OnDrawHeadInterior(gfx, rectHandle);
            }
            else
            {
                OnDrawTailInterior(gfx, rectHandle);
            }
        }

        /// <summary>
        /// Draws tail endpoint interior.
        /// Override this method to change interior.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        public virtual void OnDrawTailInterior(Graphics gfx, RectangleF rectBounds)
        {
            using (Pen pen = new Pen(this.HandleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;

                float fOffset = rectBounds.Width / 4;

                // draw x
                gfx.DrawLine(pen, rectBounds.Left + fOffset, rectBounds.Bottom - fOffset, rectBounds.Right - fOffset, rectBounds.Top + fOffset);

                gfx.DrawLine(pen, rectBounds.Left + fOffset, rectBounds.Top + fOffset, rectBounds.Right - fOffset, rectBounds.Bottom - fOffset);
            }
        }

        /// <summary>
        /// Draws head endpoint interior.
        /// Override this method to change interior.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rectBounds">The rect bounds.</param>
        public virtual void OnDrawHeadInterior(Graphics gfx, RectangleF rectBounds)
        {
            using (Pen pen = new Pen(this.HandleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;

                float fHalfWidth = rectBounds.Width / 2;
                float fQuaterWidth = rectBounds.Width / 4;

                // draw cross
                gfx.DrawLine(
                    pen,
                    rectBounds.X + fQuaterWidth, 
                    rectBounds.Y + fHalfWidth,
                    rectBounds.Right - fQuaterWidth, 
                    rectBounds.Y + fHalfWidth);

                gfx.DrawLine(
                    pen,
                    rectBounds.X + fHalfWidth, 
                    rectBounds.Y + fQuaterWidth,
                    rectBounds.X + fHalfWidth, 
                    rectBounds.Bottom - fQuaterWidth);
            }
        }

        /// <summary>
        /// Draws endpoint outline and background.
        /// Override this method to change endpoint outline and background.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="rectHandle">The rect handle.</param>
        /// <param name="endpoint">The endpoint.</param>
        public virtual void OnDrawEndPointOutline(Graphics gfx, RectangleF rectHandle, EndPoint endpoint)
        {
            using (Pen pen = new Pen(this.HandleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;

                // Create brush to fill PinPoint interiors
                using (SolidBrush brush = new SolidBrush(this.HandleColor))
                {
                    // if endpoint is connected with port -- fill its interiors with red
                    if (endpoint.Port != null)
                    {
                        brush.Color = Color.Red;
                    }

                    if (!endpoint.AllowMoveX && !endpoint.AllowMoveY)
                        brush.Color = this.HandleDisabledColor;
                    
                    // Fill handle interiors
                    gfx.FillRectangle(brush, rectHandle);

                    // Outline handle
                    gfx.DrawRectangle(pen, rectHandle.X, rectHandle.Y, rectHandle.Width, rectHandle.Height);
                }
            }
        }

        /// <summary>
        /// Draws the vertex handles.
        /// </summary>
        /// <param name="gfx">The graphics.</param>
        /// <param name="node">The node.</param>
        public virtual void DrawVertexHandles(Graphics gfx, PathNode node)
        {
            // Save Graphics state
            GraphicsState save = gfx.Save();
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;

            PointF[] pathPoints = node.GetPoints();

            Matrix mtxTransformations = node.GetTransformations();
            node.AppendFlipTransforms(mtxTransformations);

            mtxTransformations.TransformPoints(pathPoints);
            PointF ptVertex;

            using (Pen pen = new Pen(this.HandleOutlineColor))
            {
                pen.Width = 1f / gfx.PageScale;
                pen.DashStyle = DashStyle.Solid;

                // Create brush to fill PinPoint interiors
                using (SolidBrush brush = new SolidBrush(this.HandleColor))
                {
                    for (int i = 0, len = pathPoints.Length; i < len; i++)
                    {
                        ptVertex = pathPoints[i];

                        OnDrawVertexHandleShape(gfx, brush, pen, ptVertex);
                    }
                }
            }

            // Restore graphics state
            gfx.Restore(save);
        }

        /// <summary>
        /// Called when needs to draw vertex handle shape.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        /// <param name="brushVertextFill">The brush vertext fill.</param>
        /// <param name="penVertexOutline">The pen vertex outline.</param>
        /// <param name="ptHandle">The handle position in.</param>
        public virtual void OnDrawVertexHandleShape(Graphics gfx, Brush brushVertextFill, Pen penVertexOutline, PointF ptHandle)
        {
            // Handles are immutable to scale
            float fPageScale = gfx.PageScale;
            float fHandleHalfSize;

            if (HandlesHitTesting.TouchMode)
                fHandleHalfSize = (CommonUsedValues.VERTEX_HANDLE_TOUCH_SIZE / fPageScale) / 2;
            else
                fHandleHalfSize = (CommonUsedValues.VERTEX_HANDLE_SIZE / fPageScale) / 2;
            

            PointF[] pts = new PointF[4]
            {
                new PointF( ptHandle.X, ptHandle.Y - fHandleHalfSize ),
                new PointF( ptHandle.X + fHandleHalfSize, ptHandle.Y),
                new PointF( ptHandle.X, ptHandle.Y + fHandleHalfSize ),
                new PointF( ptHandle.X - fHandleHalfSize, ptHandle.Y )
            };

            // Fill handle interiors
            gfx.FillPolygon(brushVertextFill, pts);

            // Outline handle
            gfx.DrawPolygon(penVertexOutline, pts);
        }

        /// <summary>
        /// Gets the handle position.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The node.</param>
        /// <returns>The point.</returns>
        public virtual PointF GetHandlePosition(BoxPosition handle, Node node)
        {
            return handleRenderer.GetHandlePosition(handle, node);
        }

        /// <summary>
        /// Gets the bounding rect.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The rect.</returns>
        public virtual System.Drawing.Rectangle GetBoundingRect(NodeCollection nodes)
        {
            return handleRenderer.GetBoundingRect(nodes);
        }

        /// <summary>
        /// Checks if the specified handle is enabled.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="node">The shape.</param>
        /// <returns>true, if handle is enabled.</returns>
        public static bool Enabled(BoxPosition handle, Node node)
        {
            bool bValueToReturn = true;
            switch (handle)
            {
                case BoxPosition.TopLeft:
                case BoxPosition.TopRight:
                case BoxPosition.BottomLeft:
                case BoxPosition.BottomRight:
                    bValueToReturn = EditStyle.CanChangeWidth(node) && EditStyle.CanChangeHeight(node);
                    break;
                case BoxPosition.MiddleLeft:
                case BoxPosition.MiddleRight:
                    bValueToReturn = EditStyle.CanChangeWidth(node);
                    break;
                case BoxPosition.TopCenter:
                case BoxPosition.BottomCenter:
                    bValueToReturn = EditStyle.CanChangeHeight(node);
                    break;
            }

            return bValueToReturn;
        }
    }
}
