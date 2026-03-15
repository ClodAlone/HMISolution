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
    /// Node that is rendered as a curve.
    /// </summary>
    [Serializable]
    public class CurveNode
        : LineBase
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CurveNode"/> class.
        /// </summary>
        /// <param name="src">The curve node source.</param>
        public CurveNode(CurveNode src)
            : base(src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveNode"/> class.
        /// </summary>
        /// <param name="pts">The points collection.</param>
        public CurveNode(PointF[] pts)
            : base()
        {
            InitializeCurve(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurveNode"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected CurveNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Indicate that selection and resize handles will draw on diagram canvas.
        /// </summary>
        /// <returns><c>True</c> if node can be resized, otherwise - <c>false</c>.</returns>
        public override bool ShowResizeHandles()
        {
            return this.EditStyle.DefaultHandleEditMode == HandleEditMode.Resize;
        }

        /// <summary>
        /// Determines whether this node can edit segments.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit segments; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditSegment()
        {
            return false;
        }

        /// <summary>
        /// Determines whether this node can edit vertex points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this node can edit vertex point; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanEditVertexPoint()
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
        /// Determines whether this instance can draw control points.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can draw control points; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanDrawControlPoints()
        {
            return true;
        }

        /// <summary>
        /// Returns an array containing all vertices belonging to this shape.
        /// </summary>
        /// <returns>Array of points in local coordinates.</returns>
        public override PointF[] GetPoints()
        {
            return this.GetPathPoints();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new CurveNode(this);
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
            return PathFactory.CreateCurve(pathPoints).GetBounds();
        }

        /// <summary>
        /// Retrieves array of points needed to construct node's GraphicsPath.
        /// </summary>
        /// <returns>The path points</returns>
        protected override PointF[] GetPathPoints()
        {
            // create points array
            int nControlCount = this.ControlPoints.Count;
            PointF[] pts = new PointF[nControlCount];

            // 1 - Create array control points for calculate.
            for (int n = 0; n < nControlCount; n++)
            {
                pts[n] = ((ControlPoint)this.ControlPoints[n]).Location;
            }

            return pts;
        }

        /// <summary>
        /// Updates the graphics path.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            GraphicsPath gpPathTemp = CreateGraphicsPath(pts);
            RectangleF rcBounds = gpPathTemp.GetBounds();
            Geometry.TranslateToOrigin(pts, rcBounds.Location);

            // update control points list
            UpdateControlPoints(pts, 0);

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
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected override void SetPointsInternal(PointF[] ptsPath)
        {
            // pause history
            SafeHistoryPause();

            // update control points 
            UpdateControlPoints(ptsPath, 0);

            // resume history recording
            SafeHistoryResume();

            base.SetPointsInternal(ptsPath);
        }
        #endregion

        #region Class initialize shape
        /// <summary>
        /// Initializes the curve.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        private void InitializeCurve(PointF[] pts)
        {
            // get bounding rect
            GraphicsPath gpPathTemp = CreateGraphicsPath(pts);
            RectangleF rectBounding = gpPathTemp.GetBounds();
            ControlPoint ctrlPoint;

            // iterate through each segment end insert control point for each segment
            for (int n = 0, nLength = pts.Length; n < nLength; n++)
            {
                // convert control point location to local coordinates
                pts[n].X -= rectBounding.X;
                pts[n].Y -= rectBounding.Y;

                // create control point
                ctrlPoint = new ControlPoint(this, pts[n], n);
                this.ControlPoints.Add(ctrlPoint);
            }

            // assign new GraphicsPath
            // create curve path
            m_gpPath = CreateLogicalGraphicsPath(pts);
            GraphicsPath gpPath = this.GraphicsPath;

            // cache input points as vertexes
            this.PathPoints = (PointF[])gpPath.PathPoints.Clone();

            RectangleF rectPathBounding = gpPath.GetBounds();
            rectPathBounding.Offset(rectBounding.Location);

            // Create bounds info
            InitBoundsInfo(rectPathBounding);

            UpdateBoundingRectangle();
            m_bIsVertexEditable = false;
        }
        #endregion
    }
}
