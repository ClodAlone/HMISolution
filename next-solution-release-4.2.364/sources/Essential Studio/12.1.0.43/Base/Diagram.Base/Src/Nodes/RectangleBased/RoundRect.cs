#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Implementation of rounded rectangle shape.
    /// </summary>
    [Serializable]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class RoundRect
        : FilledPath
    {
        #region Class constants
        /// <summary>
        /// Default value of curve radius.
        /// </summary>
        protected const float c_fCURVE_RADIUS = 15.0f;
        #endregion

        #region Class members
        /// <summary>
        /// Curve radius.
        /// </summary>
        protected float m_fCurveRadius = c_fCURVE_RADIUS;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets percentage of the width and height of the rectangle that is included in the curves.
        /// </summary>
        /// <value>The curve radius.</value>
        /// <remarks></remarks>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Radius of curves.")
        ]
        public float CurveRadius
        {
            get 
            { 
                return (float)Math.Ceiling(m_fCurveRadius); 
            }
            set
            {
                if (CheckValueRadius(value) && OnPropertyChanging(this.FullContainerName, DPN.CurveRadius, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.CurveRadius);

                    // assign new value
                    RectangleF rcBounds = this.BoundingRectangle;
                    m_fCurveRadius = value;
                    InitializeRoundRectangle(rcBounds, value, this.BoundsInfo.Unit);

                    // update service references after recreating bounds info
                    this.BoundsInfo.UpdateServiceReferences(this);

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.CurveRadius);
                }
            }
        }
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of rectangle.</param>
        /// <param name="y">Y-coordinate of rectangle.</param>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public RoundRect(float x, float y, float width, float height, float fCurveRadius, MeasureUnits measureUnits)
            : this(new RectangleF(x, y, width, height), fCurveRadius, measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of rectangle.</param>
        /// <param name="y">Y-coordinate of rectangle.</param>
        /// <param name="width">Width of rectangle.</param>
        /// <param name="height">Height of rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public RoundRect(float x, float y, float width, float height, MeasureUnits measureUnits)
            : this(new RectangleF(x, y, width, height), measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="rcBounds">Rectangle containing position and size.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public RoundRect(RectangleF rcBounds, MeasureUnits measureUnits)
            : base()
        {
            InitializeRoundRectangle(rcBounds, c_fCURVE_RADIUS, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="rcBounds">Rectangle containing position and size.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public RoundRect(RectangleF rcBounds, float fCurveRadius, MeasureUnits measureUnits)
            : base()
        {
            InitializeRoundRectangle(rcBounds, fCurveRadius, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="rcBounds">The rc bounds.</param>
        public RoundRect(RectangleF rcBounds)
            : base()
        {
            InitializeRoundRectangle(rcBounds, c_fCURVE_RADIUS, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="rcBounds">Rectangle containing position and size.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        public RoundRect(RectangleF rcBounds, float fCurveRadius)
            : base()
        {
            InitializeRoundRectangle(rcBounds, fCurveRadius, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="pts">Points specifying rectangle.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public RoundRect(PointF[] pts)
            : this(pts, MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="pts">Points specifying rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public RoundRect(PointF[] pts, MeasureUnits measureUnits)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)), measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="pts">Points specifying rectangle.</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public RoundRect(PointF[] pts, float fCurveRadius, MeasureUnits measureUnits)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)), fCurveRadius, measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public RoundRect(RoundRect src)
            : base(src)
        {
            m_fCurveRadius = src.CurveRadius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundRect"/> class.
        /// </summary>
        /// <param name="info">Serialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected RoundRect(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_fCurveRadius = c_fCURVE_RADIUS;
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "curveRadius":
                        m_fCurveRadius = info.GetSingle("curveRadius");
                        break;
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Initializes the round rectangle.
        /// </summary>
        /// <param name="rectBounds">The round rectangle bounds .</param>
        /// <param name="fCurveRadius">The curve radius.</param>
        /// <param name="measureUnits">Bounding rectangle's measure units.</param>
        private void InitializeRoundRectangle(RectangleF rectBounds, float fCurveRadius, MeasureUnits measureUnits)
        {
            float fCurve = fCurveRadius;

            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            //// assign new GraphicsPath
            m_gpPath = PathFactory.CreateRoundRectangle(new PointF(0, 0), rectBounds.Size, fCurve);
            //// Set path point.
            this.PathPoints = (PointF[])m_gpPath.PathPoints.Clone();
            //// calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            //// assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                            rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                            rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            //// assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            //// Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = measureUnits;
            m_bCanChangePath = false;
            m_bIsVertexEditable = false;
            UpdateBoundingRectangle();
        }

        /// <summary>
        /// Checks the value radius.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>true, of value radius check.</returns>
        private bool CheckValueRadius(float value)
        {
            bool bResult = true;

            SizeF szRectangle = this.BoundsInfo.GetSize(MeasureUnits.Pixel);

            if (Math.Min(szRectangle.Width, szRectangle.Height) / 2 < value)
            {
                bResult = false;
            }

            return bResult;
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new RoundRect(this);
        }

        /// <summary>
        /// Populates a SerializationInfo with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">SerializationInfo object to populate.</param>
        /// <param name="context">Destination streaming context.</param>
        protected override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // Add curve radius to serialization info.
            info.AddValue("curveRadius", m_fCurveRadius);
        }

        /// <summary>
        /// Draw the shading over node.
        /// </summary>
        /// <param name="gfx">The Graphics to draw on.</param>
        protected override void DrawShading(Graphics gfx)
        {
            GraphicsState st = gfx.Save();
            Region clp = new Region(this.GraphicsPath);
            gfx.Clip = clp;
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            RectangleF r = this.GraphicsPath.GetBounds();
            Color color = this.FillStyle.Color;
            int opacity = 147;
            int red = (color.R * opacity / 255) + 255 * (255 - opacity) / 255;
            int green = (color.G * opacity / 255) + 255 * (255 - opacity) / 255;
            int blue = (color.B * opacity / 255) + 255 * (255 - opacity) / 255;
            if (red == 255 && green == 255)
            {
                red -= 15;
                green -= 15;
            }
            Color newColor = Color.FromArgb(opacity, red, green, blue);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(r.X, r.Y, r.X + r.Width, r.Y);
            path.AddLine(r.X + r.Width, r.Y, r.X + r.Width, r.Y + 5 + r.Height / 4);
            path.AddArc(new RectangleF(r.X, r.Y, r.Width, r.Height / 3), 0, 180);
            path.CloseFigure();

            SolidBrush brush = new SolidBrush(newColor);
            gfx.DrawPath(new Pen(newColor), path);
            gfx.FillPath(brush, path);

            RectangleF bounds = path.GetBounds();
            path.Reset();

            RectangleF ellipseRect = new RectangleF(r.X, r.Height * .6f, r.Width, r.Height - r.Height * .6f);
            path.AddEllipse(ellipseRect);

            PathGradientBrush pathBrush = new PathGradientBrush(path);
            pathBrush.CenterColor = Color.FromArgb(180, Color.White);
            pathBrush.SurroundColors = new Color[] { Color.Transparent };

            // pathBrush.CenterPoint = new PointF(path.GetBounds().Width/2, path.GetBounds().Height*.4f);
            gfx.FillPath(pathBrush, path);
            gfx.Restore(st);
        }

        /// <summary>
        /// Updates the graphics path and region.
        /// </summary>
        /// <param name="szOldSize">Old size value.</param>
        /// <param name="szNewSize">New size value.</param>
        protected override void UpdateGraphicsPath(SizeF szOldSize, SizeF szNewSize)
        {
            // call base to update graphics path
            base.UpdateGraphicsPath(szOldSize, szNewSize);

            // get graphics path bounds
            RectangleF rcBounds = this.GraphicsPath.GetBounds();

            // assign new GraphicsPath
            PointF ptSize = new PointF(rcBounds.Width, rcBounds.Height);
            Matrix mtxScale = GetScaleTransformation();
            mtxScale.Invert();
            ptSize = Geometry.AppendMatrix(ptSize, mtxScale);

            m_gpPath = CreateLogicalGraphicsPath(new PointF[] { PointF.Empty, ptSize });

            // Set path point.
            SetPoints((PointF[])this.GraphicsPath.PathPoints.Clone(), true);
        }

        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            RectangleF rcBounds = Geometry.CreateRect(pts);
            return PathFactory.CreateRoundRectangle(rcBounds.Location, rcBounds.Size, this.CurveRadius);
        }
        #endregion
    }
}
