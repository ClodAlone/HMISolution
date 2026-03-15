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
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// This node is used to draw spline-like appearance.
    /// </summary>
    [Serializable]
    public class SplineNode
        : Line
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SplineNode"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public SplineNode(SplineNode src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplineNode"/> class.
        /// </summary>
        /// <param name="pts">The points.</param>
        public SplineNode(PointF[] pts)
            : this(pts[0], pts[1], pts[2])
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplineNode"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptControl">The control point.</param>
        public SplineNode(PointF ptStart, PointF ptEnd, PointF ptControl)
            : base()
        {
            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointHead = new HeadEndPoint(this, ptEnd);

            InitializeSpline(ptStart, ptEnd, ptControl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SplineNode"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected SplineNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Determines whether this node can edit vertex point.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit vertex point; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditVertexPoint()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node can edit segment.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit segment; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditSegment()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node can edit control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit control points; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditControlPoint()
        {
            return true;
        }

        /// <summary>
        /// Override base class's(Line) method
        /// </summary>
        /// <param name="rcBounds">Updated bounds info.</param>
        protected override void UpdateBoundsInfo(RectangleF rcBounds)
        {
            UpdateNonLineBoundsInfo(rcBounds);
        }

        /// <summary>
        /// Determines whether this spline can draw control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this spline can draw control points; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanDrawControlPoints()
        {
            return true;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new SplineNode(this);
        }

        ///// <summary>
        ///// Returns an array containing all vertices belonging to this shape.
        ///// </summary>
        ///// <returns>Array of points in local coordinates.</returns>
        //public override PointF[] GetPoints()
        //{
        //    return this.GetPathPoints();
        //}

        /// <summary>
        /// Determines whether this node allow move it handle.
        /// </summary>
        /// <param name="handle">The handle to move.</param>
        /// <param name="ptNewLocation">The new endpoint location to check.</param>
        /// <returns>
        /// <c>true</c> if this node allow move it handle; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanMoveHandle(IHandle handle, PointF ptNewLocation)
        {
            return this.ContainsHandle(handle);
        }

        /// <summary>
        /// Retrieves array of points needed to construct node's GraphicsPath.
        /// </summary>
        /// <returns>The path points.</returns>
        protected override PointF[] GetPathPoints()
        {
            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

            PointF ptUpperLeft = new PointF(ptPinPoint.X - szPinOffset.Width, ptPinPoint.Y - szPinOffset.Height);

            PointF ptStart = this.TailEndPoint.Location;
            PointF ptEnd = this.HeadEndPoint.Location;

            // Create point array with two end points.
            PointF[] pts = new PointF[3];

            pts[0].X = ptStart.X - ptUpperLeft.X;
            pts[0].Y = ptStart.Y - ptUpperLeft.Y;

            pts[2].X = ptEnd.X - ptUpperLeft.X;
            pts[2].Y = ptEnd.Y - ptUpperLeft.Y;

            Matrix mtxTemp = new Matrix();
            mtxTemp.RotateAt(Geometry.ConvertToFullCircle(this.RotationAngle), szPinOffset.ToPointF(), MatrixOrder.Append);
            AppendLocalFlipTransforms(mtxTemp);

            mtxTemp.Invert();

            // Transform added end points.
            mtxTemp.TransformPoints(pts);

            // Add control point.
            pts[1] = ((ControlPoint)this.ControlPoints[0]).Location;

            return pts;
        }

        /// <summary>
        /// Updates the graphics path form points.
        /// </summary>
        /// <param name="pts">The points.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            GraphicsPath gpTemp = CreateGraphicsPath(pts);
            RectangleF rcBounds = gpTemp.GetBounds();
            Geometry.TranslateToOrigin(pts, rcBounds.Location);

            // Update control points
            UpdateControlPoints(pts, 1);

            // assign new GraphicsPath
            m_gpPath = CreateLogicalGraphicsPath(pts);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            return PathFactory.CreateCurve(pts);
        }

        /// <summary>
        /// Gets the new path points bounds.
        /// </summary>
        /// <param name="pathPoints">The path points.</param>
        /// <returns>
        /// The <see cref="System.Drawing.RectangleF"/>.
        /// </returns>
        protected override RectangleF GetNewPathBounds(PointF[] pathPoints)
        {
            return CreateGraphicsPath(pathPoints).GetBounds();
        }
        #endregion

        #region Class initialize shape
        /// <summary>
        /// Initializes the arc.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptControl">The control point.</param>
        private void InitializeSpline(PointF ptStart, PointF ptEnd, PointF ptControl)
        {
            // get bounding rect
            PointF[] pts = new PointF[] { ptStart, ptEnd, ptControl };
            RectangleF rectBounding = Geometry.CreateRect(pts);

            // Init bounds info from bounds rectangle.
            InitBoundsInfo(rectBounding);

            // Convert to local coordinates. 
            // !! Flip and rotate don't used becouse spline created without flip and rotate transformations.
            Geometry.TranslateToGridOrigin(pts);

            this.ControlPoints.Add(new ControlPoint(this, pts[2], 1));

            // assign new GraphicsPath
            GraphicsPath gpPath = PathFactory.CreateCurve(pts[0], pts[1], pts[2]);
            this.PathPoints = (PointF[])gpPath.PathPoints.Clone();
            m_gpPath = gpPath;

            UpdateBoundingRectangle();
            m_bIsVertexEditable = false;
            m_minPoints = 3;
            m_maxPoints = 3;
        }
        #endregion
    }
}