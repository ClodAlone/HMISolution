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
    /// Node that gets rendered as closed curve.
    /// </summary>
    [Serializable]
    public class ClosedCurveNode
        : FilledPath
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedCurveNode"/> class.
        /// </summary>
        /// <param name="pts">The points.</param>
        public ClosedCurveNode(PointF[] pts)
            : base()
        {
            InitializeCurve(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedCurveNode"/> class.
        /// </summary>
        /// <param name="src">The closed curved node.</param>
        public ClosedCurveNode(ClosedCurveNode src)
            : base(src)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedCurveNode"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected ClosedCurveNode(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Determines whether this instance can edit control point.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance can edit control point ; otherwise, <c>false</c>.
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new ClosedCurveNode(this);
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
        /// Sets the points internal.
        /// </summary>
        /// <param name="ptsPath">The new path points.</param>
        protected override void SetPointsInternal(PointF[] ptsPath)
        {
            // pause history recording
            SafeHistoryPause();

            // update control points
            UpdateControlPoints(ptsPath, 0);

            // resume history recording
            SafeHistoryResume();

            base.SetPointsInternal(ptsPath);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Gets the control points.
        /// </summary>
        /// <returns>The control points.</returns>
        protected override PointF[] GetPathPoints()
        {
            // create points array
            PointF[] pts = new PointF[this.ControlPoints.Count];

            // 1 - Create array control points for calculate.
            for (int n = 0, nLength = this.ControlPoints.Count; nLength > n; n++)
            {
                pts[n] = ((ControlPoint)this.ControlPoints[n]).Location;
            }

            return pts;
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
            return PathFactory.CreateClosedCurve(pathPoints).GetBounds();
        }

        /// <summary>
        /// Updates the graphics path.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            GraphicsPath gpPathTemp = PathFactory.CreateClosedCurve(pts);
            RectangleF rcBounds = gpPathTemp.GetBounds();
            Geometry.TranslateToOrigin(pts, rcBounds.Location);

            // Update control points
            for (int n = 0, nLength = this.ControlPoints.Count; nLength > n; n++)
            {
                ((ControlPoint)this.ControlPoints[n]).Location = pts[n];
            }

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
            return PathFactory.CreateClosedCurve(pts);
        }
        #endregion

        #region Class initialize shape
        /// <summary>
        /// Initializes the curve.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        private void InitializeCurve(PointF[] pts)
        {
            if (pts.Length < 2)
            {
                throw new ArgumentOutOfRangeException("must be at least two points.");
            }

            // get bounding rect
            GraphicsPath gpPathTemp = PathFactory.CreateClosedCurve(pts);
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
            m_gpPath = CreateLogicalGraphicsPath(pts);
            GraphicsPath gpPath = this.GraphicsPath;
            
            // cache input points as vertexes
            this.PathPoints = (PointF[])gpPath.PathPoints.Clone();

            RectangleF rectPathBounding = gpPath.GetBounds();
            rectPathBounding.Offset(rectBounding.Location);
            
            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectPathBounding.Width / 2, rectPathBounding.Height / 2);
            
            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectPathBounding.X + szPinOffsetUnitIndependent.Width,
                rectPathBounding.Y + szPinOffsetUnitIndependent.Height);
            
            // assign node size value
            SizeF szSizeUnitIndependent = rectPathBounding.Size;
            
            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            UpdateBoundingRectangle();
            m_bCanChangePath = true;
            m_bIsVertexEditable = false;
        }
        #endregion
    }
}
