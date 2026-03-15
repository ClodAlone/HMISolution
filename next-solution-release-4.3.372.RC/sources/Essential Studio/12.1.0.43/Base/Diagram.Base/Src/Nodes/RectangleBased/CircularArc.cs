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
    /// Implements a circular arc shape.
    /// </summary>
    [Serializable]
    public class CircularArc : FilledPath
    {
        #region Class members
        /// <summary>
        /// Circular arc type
        /// </summary>
        private ArcType m_type = ArcType.Open;
        private float m_fStartAngle = 0F;
        private float m_fSweepAngle = 270F;
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="rcBounds">Bounds of circular arc to create.</param>
        public CircularArc(RectangleF rcBounds)
        {
            InitializeArc(rcBounds, MeasureUnits.Pixel);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="rcBounds">Bounds of circular arc to create.</param>
        /// <param name="startAngle">Specifies the start angle of the arc</param>
        /// <param name="sweepAngle">Specifies the sweep angle of the arc</param>
        public CircularArc(RectangleF rcBounds, float startAngle, float sweepAngle)
            : this(rcBounds, startAngle, sweepAngle, MeasureUnits.Pixel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="rcBounds">Bounds of circular arc to create.</param>
        /// <param name="startAngle">Specifies the start angle of the arc</param>
        /// <param name="sweepAngle">Specifies the sweep angle of the arc</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        public CircularArc(RectangleF rcBounds, float startAngle, float sweepAngle, MeasureUnits measureUnits)
        {
            m_fStartAngle = startAngle;
            m_fSweepAngle = sweepAngle;
            InitializeArc(rcBounds, measureUnits);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of bounding rectangle.</param>
        /// <param name="y">Y-coordinate of bounding rectangle.</param>
        /// <param name="width">Width of bounding rectangle.</param>
        /// <param name="height">Height of bounding rectangle.</param>
        /// <param name="startAngle">Specifies the start angle of the arc</param>
        /// <param name="sweepAngle">Specifies the sweep angle of the arc</param>
        public CircularArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
            : this(new RectangleF(x, y, width, height), startAngle, sweepAngle, MeasureUnits.Pixel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="x">X-coordinate of bounding rectangle.</param>
        /// <param name="y">Y-coordinate of bounding rectangle.</param>
        /// <param name="width">Width of bounding rectangle.</param>
        /// <param name="height">Height of bounding rectangle.</param>
        /// <param name="startAngle">Specifies the start angle of the arc</param>
        /// <param name="sweepAngle">Specifies the sweep angle of the arc</param>
        /// <param name="measureUnits">Specifies points measure units.</param>        
        public CircularArc(float x, float y, float width, float height, float startAngle, float sweepAngle, MeasureUnits measureUnits)
            : this(new RectangleF(x, y, width, height), startAngle, sweepAngle, measureUnits)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularArc"/> class.
        /// </summary>
        /// <param name="src">Object to copy.</param>
        public CircularArc(CircularArc src)
            : base(src)
        {
            m_type = src.Type;
            m_fStartAngle = src.m_fStartAngle;
            m_fSweepAngle = src.m_fSweepAngle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Arc"/> class.
        /// </summary>
        /// <param name="info">Deserialization state information.</param>
        /// <param name="context">Streaming context information.</param>
        protected CircularArc(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            foreach (SerializationEntry entry in info)
            {
                switch (entry.Name)
                {
                    case "type":
                        m_type = (ArcType)info.GetValue("type", typeof(ArcType));
                        break;
                    case "startAngle":
                        m_fStartAngle = info.GetSingle("startAngle");
                        break;
                    case "sweepAngle":
                        m_fSweepAngle = info.GetSingle("sweepAngle");
                        break;
                }
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets a value to specifies the circular arc type.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Arc type."),
        DefaultValue(ArcType.Open)
        ]
        public ArcType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if (value != m_type)
                {
                    if (OnPropertyChanging(this.FullContainerName, DPN.ArcType, value))
                    {
                        // make history record
                        RecordPropertyChanged(DPN.ArcType);

                        // assign new value
                        m_type = value;
                        // assign new GraphicsPath
                        m_gpPath = CreateGraphicsPath(new PointF[] { new PointF(0, 0), new PointF(this.BoundingRect.Width, this.BoundingRect.Height) });

                        // raise property changed event
                        OnPropertyChanged(this.FullContainerName, DPN.ArcType);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to circular arc start angle.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Start angle of arc."),
        DefaultValue(0F)
        ]
        public float StartAngle
        {
            get
            {
                return m_fStartAngle;
            }
            set
            {
                if (value != m_fStartAngle && OnPropertyChanging(this.FullContainerName, DPN.StartAngle, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.StartAngle);

                    // assign new value
                    m_fStartAngle = value;
                    // assign new GraphicsPath
                    m_gpPath = CreateGraphicsPath(new PointF[] { new PointF(0, 0), new PointF(this.Size.Width, this.Size.Height) });

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.StartAngle);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to circular arc sweep angle.
        /// </summary>
        [
        Browsable(true),
        Category("Appearance"),
        Description("Sweep angle of arc."),
        DefaultValue(27F)
        ]
        public float SweepAngle
        {
            get
            {
                return m_fSweepAngle;
            }
            set
            {
                if (value != m_fSweepAngle && OnPropertyChanging(this.FullContainerName, DPN.SweepAngle, value))
                {
                    // make history record
                    RecordPropertyChanged(DPN.SweepAngle);

                    // assign new value
                    m_fSweepAngle = value;
                    // assign new GraphicsPath
                    m_gpPath = CreateGraphicsPath(new PointF[] { new PointF(0, 0), new PointF(this.Size.Width, this.Size.Height) });

                    // raise property changed event
                    OnPropertyChanged(this.FullContainerName, DPN.SweepAngle);
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
            info.AddValue("startAngle", m_fStartAngle);
            info.AddValue("sweepAngle", m_fSweepAngle);
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>Copy of the object this method is invoked against.</returns>
        public override object Clone()
        {
            return new CircularArc(this);
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

        /// <summary>
        /// Render a circular arc
        /// </summary>
        /// <param name="gfx">The Graphics to draw on</param>
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
            // Draw arc to screen.
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect, m_fStartAngle, m_fSweepAngle);
            if (this.Type != ArcType.Open)
                path.CloseFigure();
            return path;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Initializes a circular arc.
        /// </summary>
        /// <param name="rectBounds">The rect bounds.</param>
        /// <param name="measureUnits">Specifies rcBounds measure units.</param>
        private void InitializeArc(RectangleF rectBounds, MeasureUnits measureUnits)
        {
            rectBounds = MeasureUnitsConverter.ToPixels(rectBounds, measureUnits);

            // assign new GraphicsPath
            m_gpPath = CreateGraphicsPath(new PointF[] { new PointF(0, 0), new PointF(rectBounds.Width, rectBounds.Height) });

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
