#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
#if WINDOWS_PHONE
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsLineSegment = System.Windows.Media.LineSegment;
using System.Windows;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart polygon to create any shapes in 3D.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class Polygon3D
    {
        #region Members

        internal DependencyObject Tag { get; set; }

        private Panel panel;

        internal Brush Stroke { get; set; }

        internal Brush Fill { get; set; }

        private readonly double strokeThickness;

        /// <summary>
        /// The constant of plane.
        /// </summary>
        protected double d;

        /// <summary>
        /// The normal of plane.
        /// </summary>
        protected Vector3D normal;

        internal int Index { get; set; }

        /// <summary>
        /// Points of polygon.
        /// </summary>
        internal protected Vector3D[] VectorPoints;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return normal;
            }
        }

        /// <summary>
        /// Gets the A component.
        /// </summary>
        /// <value>The A component.</value>
        public double A
        {
            get
            {
                return normal.X;
            }
        }

        /// <summary>
        /// Gets the B component.
        /// </summary>
        /// <value>The B component.</value>
        public double B
        {
            get
            {
                return normal.Y;
            }
        }

        /// <summary>
        /// Gets the C component.
        /// </summary>
        /// <value>The C component.</value>
        public double C
        {
            get
            {
                return normal.Z;
            }
        }

        /// <summary>
        /// Gets the D component.
        /// </summary>
        /// <value>The D component.</value>
        public double D
        {
            get
            {
                return d;
            }
        }

        /// <summary>
        /// Gets the points of polygon.
        /// </summary>
        /// <value>The points.</value>
        public virtual Vector3D[] Points
        {
            get
            {
                return VectorPoints;
            }
        }

        #endregion

        #region Constructor

        public Polygon3D()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        public Polygon3D(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            CalcNormal(v1, v2, v3);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public Polygon3D(Vector3D[] points)
        {
            CalcNormal(points[0], points[1], points[2]);
            VectorPoints = points;
            CalcNormal();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="index"></param>
        public Polygon3D(Vector3D[] points, int index)
            : this(points)
        {
            Index = index;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="normal">The normal.</param>
        /// <param name="d">The d.</param>
        public Polygon3D(Vector3D normal, double d)
        {
            this.normal = normal;
            this.d = d;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <param name="stroke"></param>
        /// <param name="strokeThickness"></param>
        /// <param name="fill"></param>
        /// <param name="opacity"></param>
        public Polygon3D(Vector3D[] points, DependencyObject tag, int index, Brush stroke, double strokeThickness, Brush fill)
            : this(points)
        {
            Element = new Path();
            Index = index;
            this.Tag = tag;
            this.Stroke = stroke;
            this.strokeThickness = strokeThickness;
            this.Fill = fill;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon3D"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="polygon">The plane.</param>
        public Polygon3D(Vector3D[] points, Polygon3D polygon)
            : this(points)
        {
            polygon.Element = null;
            Element = new Path();
            Index = polygon.Index;
            Stroke = polygon.Stroke;
            Tag = polygon.Tag;
            Graphics3D = polygon.Graphics3D;
            Fill = polygon.Fill;
            strokeThickness = polygon.strokeThickness;
            IsSplitted = true;
        }

        #endregion

        #region methods

        /// <summary>
        /// Creates the UI element.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal static UIElement3D CreateUIElement(Vector3D position, UIElement element, double xLen, double yLen)
        {
            Canvas.SetZIndex(element, 0);
            var vectorColl = new Vector3D[3];
            var x = position.X;
            var y = position.Y;
#if SILVERLIGHT_UNCOMMON || WINDOWS_PHONE8 || WINDOWS_PHONE7
            var desiredWidth = (element as FrameworkElement).ActualWidth;
            var desiredHeight = (element as FrameworkElement).ActualHeight;
#else
            var desiredWidth = element.DesiredSize.Width;
            var desiredHeight = element.DesiredSize.Height;
#endif
            vectorColl[0] = new Vector3D(x, y, position.Z);
            vectorColl[1] = new Vector3D(x + desiredWidth, y + desiredHeight + yLen, position.Z);
            vectorColl[2] = new Vector3D(x + desiredWidth + xLen, y + desiredHeight + yLen, position.Z);
            return new UIElement3D(element, vectorColl);
        }

        internal static PolyLine3D CreatePolyline(List<Vector3D> points, Path element)
        {
            if (points.Count == 2)
            { 
                var prePoint = points[1];
                points.Add(new Vector3D(prePoint.X, prePoint.Y, prePoint.Z));
            }
            return new PolyLine3D(element, points);
        }

        internal static Polygon3D[] CreateBox(Vector3D v1, Vector3D v2, DependencyObject tag, int index,
            Graphics3D graphics3D, Brush stroke, Brush fill, double strokeThickness, bool inverse)
        {
            var res = new Polygon3D[6];

            var p1 = new[]
            {
                new Vector3D(v1.X, v1.Y, v1.Z),
                new Vector3D(v2.X, v1.Y, v1.Z),
                new Vector3D(v2.X, v2.Y, v1.Z),
                new Vector3D(v1.X, v2.Y, v1.Z)
            };

            var p2 = new[]
            {
                new Vector3D(v1.X, v1.Y, v2.Z),
                new Vector3D(v2.X, v1.Y, v2.Z),
                new Vector3D(v2.X, v2.Y, v2.Z),
                new Vector3D(v1.X, v2.Y, v2.Z)
            };

            var p3 = new[]
            {
                new Vector3D(v1.X, v1.Y, v2.Z),
                new Vector3D(v2.X, v1.Y, v2.Z),
                new Vector3D(v2.X, v1.Y, v1.Z),
                new Vector3D(v1.X, v1.Y, v1.Z)
            };

            var p4 = new[]
            {
                new Vector3D(v1.X, v2.Y, v2.Z),
                new Vector3D(v2.X, v2.Y, v2.Z),
                new Vector3D(v2.X, v2.Y, v1.Z),
                new Vector3D(v1.X, v2.Y, v1.Z)
            };

            var p5 = new[]
            {
                new Vector3D(v1.X, v1.Y, v1.Z),
                new Vector3D(v1.X, v1.Y, v2.Z),
                new Vector3D(v1.X, v2.Y, v2.Z),
                new Vector3D(v1.X, v2.Y, v1.Z)
            };

            var p6 = new[]
            {
                new Vector3D(v2.X, v1.Y, v1.Z),
                new Vector3D(v2.X, v1.Y, v2.Z),
                new Vector3D(v2.X, v2.Y, v2.Z),
                new Vector3D(v2.X, v2.Y, v1.Z)
            };

            res[0] = new Polygon3D(p1, tag, index, stroke, strokeThickness, fill);
            res[1] = new Polygon3D(p2, tag, index, stroke, strokeThickness, fill);
            res[2] = new Polygon3D(p3, tag, index, stroke, strokeThickness, fill);
            res[3] = new Polygon3D(p4, tag, index, stroke, strokeThickness, fill);
            res[4] = new Polygon3D(p5, tag, index, stroke, strokeThickness, fill);
            res[5] = new Polygon3D(p6, tag, index, stroke, strokeThickness, fill);

            if (inverse)
            {
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[5]);
            }
            else
            {
                graphics3D.AddVisual(res[5]);
                graphics3D.AddVisual(res[4]);
                graphics3D.AddVisual(res[0]);
                graphics3D.AddVisual(res[1]);
                graphics3D.AddVisual(res[2]);
                graphics3D.AddVisual(res[3]);
            }
            return res;
        }

        /// <summary>
        /// Updates the box.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="visibility">The visibility.</param>
        internal static void UpdateBox(Polygon3D[] plan, Vector3D v1, Vector3D v2, Brush stroke, Visibility visibility)
        {
            if (plan.Length < 6) return;

            plan[0].Update(new[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) }, stroke, visibility);

            plan[1].Update(new[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ) }, stroke, visibility);

            plan[2].Update(new[] { new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v1.Z ) }, stroke, visibility);

            plan[3].Update(new[] { new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) }, stroke, visibility);

            plan[4].Update(new[] { new Vector3D( v1.X, v1.Y, v1.Z ),
                                        new Vector3D( v1.X, v1.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v2.Z ),
                                        new Vector3D( v1.X, v2.Y, v1.Z ) }, stroke, visibility);

            plan[5].Update(new[] { new Vector3D( v2.X, v1.Y, v1.Z ),
                                        new Vector3D( v2.X, v1.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v2.Z ),
                                        new Vector3D( v2.X, v2.Y, v1.Z ) }, stroke, visibility);
        }

        /// <summary>
        /// Creates the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        internal static Line3D CreateLine(Line line, double x1, double y1, double x2, double y2, double depth)
        {
            var strokeThickness = line.StrokeThickness;
            var vectorColl = new Vector3D[3];
            vectorColl[0] = new Vector3D(x1, y1, depth);
            vectorColl[1] = new Vector3D(x1 + strokeThickness, y2 + strokeThickness, depth);
            vectorColl[2] = new Vector3D(x2, y2, depth);
            return new Line3D(line, vectorColl);
        }

        /// <summary>
        /// Tests this instance to the existing.
        /// </summary>
        /// <returns>Indicates whether Normal of Plane is valid or Not.</returns>
        public bool Test()
        {
            return !normal.IsValid;
        }

        /// <summary>
        /// The epsilon
        /// </summary>
        public const double Epsilon = 0.00001;

        /// <summary>
        /// Gets the point on the plane.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(double x, double y)
        {
            var z = -(A * x + B * y + D) / C;

            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// Gets the point of intersect ray with plane.
        /// </summary>
        /// <param name="position">The pos.</param>
        /// <param name="ray">The ray.</param>
        /// <returns>Returns Vector3D instance.</returns>
        public Vector3D GetPoint(Vector3D position, Vector3D ray)
        {
            var dir = normal * (-d) - position;

            var sv = dir & normal;
            var sect = sv / (normal & ray);

            return position + ray * sect;
        }

        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>Returns Vector3D instance.</returns>
        internal virtual Vector3D GetNormal(Matrix3D transform)
        {
            Vector3D norm;

            if (VectorPoints != null)
            {
                norm = ChartMath.GetNormal(transform * VectorPoints[0],
                    transform * VectorPoints[1], transform * VectorPoints[2]);


                for (var i = 3; (i < VectorPoints.Length) && !norm.IsValid; i++)
                {
                    var v1 = transform * VectorPoints[i];
                    var v2 = transform * VectorPoints[0];
                    var v3 = transform * VectorPoints[i / 2];

                    norm = ChartMath.GetNormal(v1, v2, v3);
                }
            }
            else
            {
                norm = transform & normal;
                norm.Normalize();
            }

            return norm;
        }

        internal bool IsSplitted;

        /// <summary>
        /// Transforms by the specified <see cref="Matrix3D"/>.
        /// </summary>
        public virtual void Transform(Matrix3D matrix)
        {
            if (Points != null)
            {
                for (var i = 0; i < Points.Length; i++)
                {
                    Points[i] = matrix * Points[i];
                }

                CalcNormal();
            }
            else
            {
                var v = matrix * (normal * -d);
                normal = matrix & normal;
                normal.Normalize();
                d = -(normal & v);
            }
        }

        internal Graphics3D Graphics3D { get; set; }

        /// <summary>
        /// Gets or sets the element.
        /// </summary>
        /// <value>
        /// The element.
        /// </value>
        public UIElement Element { get; set; }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D"/>.
        /// </summary>
        /// <param name="panel"></param>
        /// <returns>Return ChartRegion.</returns>
        internal virtual void Draw(Panel panel)
        {
            this.panel = panel;
            if (VectorPoints == null || VectorPoints.Length <= 0) return;
            var transform = Graphics3D.Transform;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            segmentPath.Tag = Tag;
            if (Tag != null && Tag is ChartSegment3D)
            {
                ((ChartSegment3D)Tag).Polygons.Add(this);
            }
            if (!panel.Children.Contains(segmentPath))
                panel.Children.Add(segmentPath);

            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();
            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }
            segmentGeometry.Figures.Add(figure);
            segmentPath.Data = segmentGeometry;
            var lightCoef = (int)(16 * (2 * Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            if (lightCoef < 0 && Fill != null)
            {
                var actualBrush = ((SolidColorBrush)Fill).Color;
                segmentPath.Fill = ApplyLight(actualBrush);
              
            }
            else
            {
                segmentPath.Fill = Fill;
            }
            segmentPath.StrokeThickness = strokeThickness;
            segmentPath.Stroke = Stroke;
        }

        internal void ReDraw()
        {
            if (VectorPoints == null || VectorPoints.Length <= 0) return;
            var transform = Graphics3D.Transform;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();
            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }
            segmentGeometry.Figures.Add(figure);
            segmentPath.Data = segmentGeometry;
            var lightCoef = (int)(16 * (2 * Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            if (lightCoef < 0 && Fill != null)
            {
                var actualBrush = ((SolidColorBrush)Fill).Color;
                segmentPath.Fill = ApplyLight(actualBrush);

            }
            else
            {
                segmentPath.Fill = Fill;
            }
            segmentPath.StrokeThickness = strokeThickness;
            segmentPath.Stroke = Stroke;
        }

        internal void Update(Vector3D[] updatedPoints, Brush interior, Visibility visibility)
        {
            VectorPoints = updatedPoints;
            var segmentPath = Element as Path;
            if (segmentPath == null) return;
            segmentPath.Visibility = visibility;
            if (Graphics3D == null) return;
            var transform = Graphics3D.Transform;
            var figure = new PathFigure();
            var segmentGeometry = new PathGeometry();
            if (transform != null)
            {
                figure.StartPoint = transform.ToScreen(VectorPoints[0]);
                foreach (var lineSegment in VectorPoints.Select(item => new WindowsLineSegment { Point = transform.ToScreen(item) }))
                {
                    figure.Segments.Add(lineSegment);
                }
            }
            segmentGeometry.Figures.Add(figure);
            var lightCoef = (int)(16 * (2 * Math.Abs(normal & new Vector3D(0, 0, 1)) - 1));
            if (lightCoef < 0 && interior != null)
            {
                var actualBrush = ((SolidColorBrush)interior).Color;
                segmentPath.Fill = ApplyLight(actualBrush);
            }
            else
            {
                segmentPath.Fill = interior;
            }
            segmentPath.Data = segmentGeometry;
        }

        private static SolidColorBrush ApplyLight(Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, (byte)(color.R * 0.9), (byte)(color.G * 0.9), (byte)(color.B * 0.9)));
        }

        internal void CalcNormal()
        {
            CalcNormal(Points[0], Points[1], Points[2]);

            for (var i = 3; (i < Points.Length) && (Test()); i++)
            {
                CalcNormal(Points[i], Points[0], Points[i / 2]);
            }
        }

        /// <summary>
        /// Calculates the normal.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <param name="v3">The v3.</param>
        protected void CalcNormal(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            var n = (v1 - v2) * (v3 - v2);
            var l = n.GetLength();

            if (l < Epsilon)
            {
                l = 1;
            }

            normal = new Vector3D(n.X / l, n.Y / l, n.Z / l);
            d = -(A * v1.X + B * v1.Y + C * v1.Z);
        }
        #endregion
    }

    /// <summary>
    /// Used to plot any UIElement in 3D view.
    /// </summary>
    public class UIElement3D : Polygon3D
    {

        readonly FrameworkElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIElement3D"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="points">The points.</param>
        public UIElement3D(UIElement element, Vector3D[] points)
            :base(points)
        {
            this.element = element as FrameworkElement;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel"></param>
        internal override void Draw(Panel panel)
        {
            if (element.Parent == null)
                panel.Children.Add(element);
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var transform = Graphics3D.Transform;
            if (transform == null) return;
            var actual3DPosition = transform.ToScreen(VectorPoints[0]);
            var x = actual3DPosition.X;
            var y = actual3DPosition.Y;
#if !WPF
            if (element is TextBlock)
            {
                x = x - element.ActualWidth / 2;
                y = y - element.ActualHeight / 2;
            }
            else
            {
                x = x - element.DesiredSize.Width / 2;
                y = y - element.DesiredSize.Height / 2;
            }
#else
            x = x - element.DesiredSize.Width / 2;
            y = y - element.DesiredSize.Height / 2;
#endif
            Canvas.SetLeft(element, x);
            Canvas.SetTop(element, y);
        }
    }

    /// <summary>
    /// Used to draw line in 3D view.
    /// </summary>
    public class PolyLine3D : Polygon3D
    {
        readonly Path element;

        /// <summary>
        /// Initializes a new instance of the <see cref="Line3D"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="points">The points.</param>
        public PolyLine3D(Path element, List<Vector3D> vectors)
            : base(vectors.ToArray())
        {
            this.element = element;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel"></param>
        internal override void Draw(Panel panel)
        {
            if (((FrameworkElement)element).Parent == null)
                panel.Children.Add(element);
            var transform = Graphics3D.Transform;
            if (transform == null) return;

            var pathFigure = new PathFigure();
            var pathGeometry = new PathGeometry();

            pathGeometry.Figures.Add(pathFigure);
            element.Data = pathGeometry;
            pathFigure.StartPoint = transform.ToScreen(VectorPoints[0]);
            var segment = new PolyLineSegment();
            segment.Points = new PointCollection();
            foreach (var vectorPoint in VectorPoints)
            {
                segment.Points.Add(transform.ToScreen(vectorPoint));
            }
            pathFigure.Segments.Add(segment);
        }
    }

    /// <summary>
    /// Used to draw line in 3D view.
    /// </summary>
    public class Line3D : Polygon3D
    {
        readonly UIElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="Line3D"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="points">The points.</param>
        public Line3D(UIElement element, Vector3D[] points)
            : base(points)
        {
            this.element = element;
        }

        /// <summary>
        /// Draws to the specified <see cref="Graphics3D" />.
        /// </summary>
        /// <param name="panel"></param>
        internal override void Draw(Panel panel)
        {
            if (((FrameworkElement)element).Parent == null)
                panel.Children.Add(element);
            var transform = Graphics3D.Transform;
            if (transform == null) return;
            var actual3DPosition1 = transform.ToScreen(VectorPoints[0]);
            var actual3DPosition2 = transform.ToScreen(VectorPoints[2]);
            var line = element as Line;
            if (line == null) return;
            line.X1 = actual3DPosition1.X;
            line.X2 = actual3DPosition2.X;
            line.Y1 = actual3DPosition1.Y;
            line.Y2 = actual3DPosition2.Y;
        }
    }
}
