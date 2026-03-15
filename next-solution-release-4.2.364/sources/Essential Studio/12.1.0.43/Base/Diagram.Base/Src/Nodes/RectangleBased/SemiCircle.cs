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
    /// Implements a semi-circle shape.
    /// </summary>
    [Serializable]
    public class SemiCircle:FilledPath
    {
        #region Class members
        /// <summary>
        /// Semi-cirlce type
        /// </summary>
        private SemiCircleType m_type = SemiCircleType.Closed;
        
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of bounding rectangle.</param>
        /// <param name="y">Y-coordinate of bounding rectangle.</param>
        /// <param name="width">Width of bounding rectangle.</param>
        /// <param name="height">Height of bounding rectangle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        public SemiCircle(float x, float y, float width, float height, MeasureUnits measureUnits)
            : this(new RectangleF(x, y, width, height), measureUnits)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="rcBounds">Bounds of semi-circle to create.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public SemiCircle(RectangleF rcBounds, MeasureUnits measureUnits)
        {
            InitializeSemiCircle(rcBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="pts">Control points used to create the semi-circle.</param>
        /// <param name="measureUnits">Specifies points measure units.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public SemiCircle(PointF[] pts, MeasureUnits measureUnits)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)), measureUnits)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of bounding rectangle.</param>
        /// <param name="y">Y-coordinate of bounding rectangle.</param>
        /// <param name="width">Width of bounding rectangle.</param>
        /// <param name="height">Height of bounding rectangle.</param>
        public SemiCircle(float x, float y, float width, float height)
            : this(new RectangleF(x, y, width, height), MeasureUnits.Pixel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="rcBounds">Bounds of ellipse to create.</param>
        public SemiCircle(RectangleF rcBounds)
        {
            InitializeSemiCircle(rcBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="pts">Control points used to create the semi-circle.</param>
        /// <remarks>
        /// The array passed in must contain two points that specify a rectangle.
        /// The first point in the array is the upper-left corner of the rectangle
        /// and the second point is the lower-right corner of the rectangle.
        /// </remarks>
        public SemiCircle(PointF[] pts)
            : this(new RectangleF(pts[0], new SizeF(pts[1].X - pts[0].X, pts[1].Y - pts[0].Y)), MeasureUnits.Pixel)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public SemiCircle(SemiCircle src)
            : base(src)
        {
            m_type = src.Type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SemiCircle"/> class.
        /// </summary>
        /// <param name="info">Deserialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected SemiCircle(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                if (entry.Name == "type")
                    m_type = (SemiCircleType)info.GetValue("type", typeof(SemiCircleType));
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value to specifies the semi-circle type.
        /// </summary>        
        [
        Browsable(true),
        Category("Appearance"),
        Description("Semi-Circle type."),
        DefaultValue(SemiCircleType.Closed)
        ]
        public SemiCircleType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if (value != m_type)
                {
                    if (OnPropertyChanging(this.FullContainerName, DPN.SemiCircleType, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.SemiCircleType);

                        // assign new value
                        m_type = value;
                        // assign new GraphicsPath
                        m_gpPath = CreateGraphicsPath(new PointF[] { new PointF(0, 0), new PointF(this.BoundingRect.Width, this.BoundingRect.Height) });
                        
                        // raise property changed event
                        OnPropertyChanged(this.FullContainerName, DPN.SemiCircleType);
                    }
                }
            }
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
            info.AddValue("type", m_type);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new SemiCircle(this);
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
            RectangleF rectBounds = this.GraphicsPath.GetBounds();

            GraphicsPath path = new GraphicsPath();
            path.AddLine(rectBounds.X, rectBounds.Y, rectBounds.X + rectBounds.Width, rectBounds.Y);
            path.AddArc(new RectangleF(rectBounds.X, rectBounds.Y, rectBounds.Width, rectBounds.Height / 2), 0, 180);
            path.CloseFigure();

            Color color = this.FillStyle.Color;
            int opacity = 147;
            int red = (color.R * opacity / 255) + 255 * (255 - opacity) / 255;
            int green = (color.G * opacity / 255) + 255 * (255 - opacity) / 255;
            int blue = (color.B * opacity / 255) + (255) * (255 - opacity) / 255;
            if (red == 255 && green == 255)
            {
                red -= 20;
                green -= 20;
            }
            Color newColor = Color.FromArgb(opacity, red, green, blue);
            SolidBrush brush = new SolidBrush(newColor);
            gfx.FillPath(brush, path);
            path.Reset();

            float height = rectBounds.Height * .20f;
            float width = height / 2;

            RectangleF temp = new RectangleF(rectBounds.Width * .15f, rectBounds.Height * .15f, width, height);
            path.AddEllipse(temp);
            Matrix matrix = new Matrix();
            matrix.RotateAt(40, new PointF(rectBounds.Width * .15f, rectBounds.Height * .20f), MatrixOrder.Prepend);
            path.Transform(matrix);
            LinearGradientBrush ellipseBrush = new LinearGradientBrush(temp, Color.Transparent, Color.FromArgb(200, Color.White), LinearGradientMode.Vertical);
            gfx.FillPath(ellipseBrush, path);
            RectangleF bounds = path.GetBounds();
            path.Reset();
            RectangleF ellipseRect = new RectangleF(rectBounds.X, rectBounds.Height * .65f, rectBounds.Width, rectBounds.Height - rectBounds.Height * .65f);
            path.AddEllipse(ellipseRect);

            PathGradientBrush pathBrush = new PathGradientBrush(path);
            pathBrush.CenterColor = Color.FromArgb(180, Color.White);
            pathBrush.SurroundColors = new Color[] { Color.Transparent };
            
            // pathBrush.CenterPoint = new PointF(path.GetBounds().Width/2, path.GetBounds().Height*.4f);
            gfx.FillPath(pathBrush, path);

            gfx.Restore(st);
        }
        protected override void Render(Graphics gfx)
        {
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            base.Render(gfx);
        }
        /// <summary>
        /// Creates node's path with given array of points.
        /// </summary>
        /// <param name="pts">Points to create path from.</param>
        /// <returns>Created GraphicsPath, otherwise null.</returns>
        protected override GraphicsPath CreateLogicalGraphicsPath(PointF[] pts)
        {
            RectangleF rect = Geometry.CreateRect(pts);
            rect.Height = rect.Height * 2;
            // Create start and sweep angles on ellipse.
            int startAngle = 0;
            int sweepAngle = -180;

            // Draw arc to screen.
            GraphicsPath path = new GraphicsPath();            
            path.AddArc(rect, startAngle, sweepAngle);
            if (this.Type != SemiCircleType.Open)
                path.CloseFigure();
            return path;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Initializes the semi-circle.
        /// </summary>
        /// <param name="rectBounds">The rect bounds.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        private void InitializeSemiCircle(RectangleF rectBounds, MeasureUnits measureUnits)
        {
            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);
            
            // assign new GraphicsPath
            m_gpPath = CreateGraphicsPath(new PointF[] { new PointF( 0, 0 ), new PointF( rectBounds.Width, rectBounds.Height ) });

            // Set path point.
            this.PathPoints = (PointF[])m_gpPath.PathPoints.Clone();
            
            // calc pin offset
            SizeF szPinOffsetUnitIndependent = new SizeF(rectBounds.Width / 2, rectBounds.Height / 2);
            
            // assign default PinLocation
            PointF ptPinPointUnitIndependent = new PointF(
                rectBounds.Location.X + szPinOffsetUnitIndependent.Width,
                rectBounds.Location.Y + szPinOffsetUnitIndependent.Height);
            
            // assign node size value
            SizeF szSizeUnitIndependent = rectBounds.Size;
            
            // Init BoundsInfo
            CreateBoundsInfo(ptPinPointUnitIndependent, szPinOffsetUnitIndependent, szSizeUnitIndependent);

            this.BoundsInfo.Unit = measureUnits;
            UpdateBoundingRectangle();
            m_bCanChangePath = false;
            m_bIsVertexEditable = false;
        }
        #endregion
    }
}

