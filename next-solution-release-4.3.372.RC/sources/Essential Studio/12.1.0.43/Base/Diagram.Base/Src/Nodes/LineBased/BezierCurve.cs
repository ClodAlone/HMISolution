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
    /// Line node shape with two additional controlPoints to change line flexure.
    /// Can be used as connector.
    /// </summary>
    [Serializable]
    public class BezierCurve
        : Line
    {
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="BezierCurve"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        public BezierCurve(BezierCurve src)
            : base(src)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BezierCurve"/> class.
        /// </summary>
        /// <param name="pts">The array of points.</param>
        public BezierCurve(PointF[] pts)
        {
            int nCurve = (pts.Length - 1) / 3;
            if (pts.Length < 2 || pts.Length != nCurve * 3 + 1)
                throw new ArgumentException("Parameter is not valid. The number of points in the array should be a multiple of 3 plus 1, such as 4, 7, or 10.");
            // set endpoints
            m_endPointTail = new TailEndPoint(this, pts[0]);
            m_endPointHead = new HeadEndPoint(this, pts[pts.Length-1]);
            if(pts.Length == 2)
                InitializeBezier(pts[0], pts[1]);
            else
                InitializeBezier(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BezierCurve"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        public BezierCurve(PointF ptStart, PointF ptEnd)
            : base()
        {
            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointHead = new HeadEndPoint(this, ptEnd);

            InitializeBezier(ptStart, ptEnd);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BezierCurve"/> class.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptCtrStart">The first control point.</param>
        /// <param name="ptEnd">The end point.</param>
        /// <param name="ptCtrlEnd">The second control point.</param>
        public BezierCurve(PointF ptStart, PointF ptCtrStart, PointF ptEnd, PointF ptCtrlEnd)
            : base()
        {
            // set endpoints
            m_endPointTail = new TailEndPoint(this, ptStart);
            m_endPointHead = new HeadEndPoint(this, ptEnd);
            PointF[] pts = new PointF[] { ptStart, ptCtrStart, ptCtrlEnd, ptEnd };
            InitializeBezier(pts);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BezierCurve"/> class.
        /// </summary>
        /// <param name="info">The serialization Info.</param>
        /// <param name="context">The streaming context.</param>
        protected BezierCurve(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { 
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Gets the path points.
        /// </summary>
        /// <returns>The path points.</returns>
        protected override PointF[] GetPathPoints()
        {
            PointF ptPinPoint = this.BoundsInfo.GetPinPoint(MeasureUnits.Pixel);
            SizeF szPinOffset = this.BoundsInfo.GetPinOffset(MeasureUnits.Pixel);

            PointF ptUpperLeft = new PointF(
                ptPinPoint.X - szPinOffset.Width,
                ptPinPoint.Y - szPinOffset.Height);

            PointF ptStart = this.TailEndPoint.Location;
            PointF ptEnd = this.HeadEndPoint.Location;

            // Create point array with two end points.
            int nLength = this.ControlPoints.Count;
            PointF[] pts = this.GraphicsPath.PathPoints;

            pts[0].X = ptStart.X - ptUpperLeft.X;
            pts[0].Y = ptStart.Y - ptUpperLeft.Y;

            pts[pts.Length - 1].X = ptEnd.X - ptUpperLeft.X;
            pts[pts.Length - 1].Y = ptEnd.Y - ptUpperLeft.Y;

            Matrix mtxTemp = new Matrix();
            mtxTemp.RotateAt(
                Geometry.ConvertToFullCircle(this.RotationAngle),
                szPinOffset.ToPointF(), 
                MatrixOrder.Append);

            AppendLocalFlipTransforms(mtxTemp);

            mtxTemp.Invert();

            // Transform added end points.
            mtxTemp.TransformPoints(pts);

            //// Add control point.
            for (int n = 0; n < nLength; n++)
            {
                pts[n + 1] = ((ControlPoint)this.ControlPoints[n]).Location;
            }

            return pts;
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
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public override object Clone()
        {
            return new BezierCurve(this);
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
        /// Draws the control points.
        /// </summary>
        /// <param name="gfx">Graphics to draw on.</param>
        public override void DrawControlPoints(Graphics gfx)
        {
            PointF[] pts;
            Matrix mtxTransform = GetTransformations();
            AppendFlipTransforms(mtxTransform);

            // 1 - draw connecting lines
            using (Pen penHelperLine = new Pen(Color.Red, 0f))
            {
                penHelperLine.DashStyle = DashStyle.Dash;

                for (int i = 0; i < this.ControlPoints.Count; i++)
                {
                    if (i == 0)
                    {
                        pts = new PointF[] { ((ControlPoint)this.ControlPoints[i]).Location };
                        mtxTransform.TransformPoints(pts);
                        // end endpoint -> end control point
                        gfx.DrawLine(penHelperLine, this.TailEndPoint.Location, pts[0]);
                    }
                    else if (i == this.ControlPoints.Count -1)
                    {
                        pts = new PointF[] { ((ControlPoint)this.ControlPoints[i - 1]).Location, ((ControlPoint)this.ControlPoints[i]).Location };
                        mtxTransform.TransformPoints(pts);
                        // start endpoint -> start control point
                        gfx.DrawLine(penHelperLine, pts[0], pts[1]);
                        gfx.DrawLine(penHelperLine, pts[1], this.HeadEndPoint.Location );
                    }
                    else
                    {
                        pts = new PointF[] { ((ControlPoint)this.ControlPoints[i - 1]).Location, ((ControlPoint)this.ControlPoints[i]).Location };
                        mtxTransform.TransformPoints(pts);
                        // end endpoint -> end control point
                        gfx.DrawLine(penHelperLine, pts[0], pts[1]);
                    }
                }
            }

            // 2 - Draw control points
            base.DrawControlPoints(gfx);
        }

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
        /// Updates the graphics path form points.
        /// </summary>
        /// <param name="pts">The points.</param>
        protected override void UpdateGraphicsPath(PointF[] pts)
        {
            Geometry.TranslateToGridOrigin(pts);

            // Update control points
            UpdateControlPoints(pts, 1);

            // Update Graphics path.
            m_gpPath = CreateLogicalGraphicsPath(pts);
        }

        /// <summary>
        /// Create the logical graphics path.
        /// </summary>
        /// <param name="pts">The path points.</param>
        /// <returns>The graphics path.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            return PathFactory.CreateBezier(pts);
        }

        /// <summary>
        /// Updates the Pin position, pin offset and size from new bounds rectangle.
        /// </summary>
        /// <param name="rcBounds">New bounds rectangle.</param>
        protected override void UpdateBoundsInfo(RectangleF rcBounds)
        {
            UpdateNonLineBoundsInfo(rcBounds);
        }
        #endregion

        #region Class initialize shape
        /// <summary>
        /// Initializes the bezier.
        /// </summary>
        /// <param name="ptStart">The start point.</param>
        /// <param name="ptEnd">The end point.</param>
        private void InitializeBezier(PointF ptStart, PointF ptEnd)
        {
            // 1 - Calc control points
            PointF ptCtrFirst = new PointF(ptStart.X + (ptEnd.X - ptStart.X) / 2, ptStart.Y);
            PointF ptCtrSecond = new PointF(ptEnd.X - (ptEnd.X - ptStart.X) / 2, ptEnd.Y);

            // 2 - Init BoundsInfo.
            PointF[] pts = new PointF[] { ptStart, ptCtrFirst, ptCtrSecond, ptEnd };
            InitializeBezier(pts);
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Initializes the bezier.
        /// </summary>
        /// <param name="pts">The points collection.</param>
        private void InitializeBezier(PointF[] pts)
        {
            // 1 - Get bounding rect
            RectangleF rectPathBounding = Geometry.CreateRect(pts);

            // 2 - Initialize BoundsInfo.
            InitBoundsInfo(rectPathBounding);

            // 3 - Update points
            Geometry.TranslateToGridOrigin(pts);

            // 4 - Assign new GraphicsPath
            m_gpPath = CreateLogicalGraphicsPath(pts);
            this.PathPoints = (PointF[])this.GraphicsPath.PathPoints.Clone();

            // 5 - Assign control points
            for(int i = 1, j = 0; i < pts.Length; i = i + 3, j = j + 2)
            SetContolPoints(pts[i], pts[i+1], j);

            m_bIsVertexEditable = false;
            m_minPoints = 4;
            m_maxPoints = pts.Length;

            UpdateBoundingRectangle();
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Sets the control points.
        /// </summary>
        /// <param name="ptFirstPoint">The first point.</param>
        /// <param name="ptSecondPoint">The second point.</param>
        /// <param name="startIndex">Index starting at which ControlPoints will be inserted.</param>
        private void SetContolPoints(PointF ptFirstPoint, PointF ptSecondPoint, int startIndex)
        {
            // Add first point
            this.ControlPoints.Add(new ControlPoint(this, ptFirstPoint, startIndex));

            // Add second point
            this.ControlPoints.Add(new ControlPoint(this, ptSecondPoint, startIndex + 1));
        }
        #endregion
    }
}
