using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFUtilities.Extensions;

namespace PolyBezier
{
    public class PolyBezier : Shape, IDisposable
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", 
            typeof(PointCollection), typeof(PolyBezier), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, 
                OnPoinsChanged));

        public PointCollection Points
        {
            set { SetValue(PointsProperty, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        private static void OnPoinsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as PolyBezier;
            if (control == null)
                return;
            control.SetPathData();
        }

        public static readonly DependencyProperty IsClosedCurveProperty =
            DependencyProperty.Register("IsClosedCurve", typeof(bool), typeof(PolyBezier),
                                        new PropertyMetadata(default(bool), OnIsClosedCurveChanged));

        private static void OnIsClosedCurveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as PolyBezier;
            if (control == null)
                return;
            control.SetPathData();
        }

        public bool IsClosedCurve
        {
            get { return (bool)GetValue(IsClosedCurveProperty); }
            set { SetValue(IsClosedCurveProperty, value); }
        }

        private Geometry _polylineGeometry;

        public PolyBezier()
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(StrokeProperty, typeof(PolyBezier));
            dpd.AddValueChangedSafe(this, OnStrokeChanged);
        }

        private void OnStrokeChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        void SetPathData()
        {
            this._polylineGeometry = Geometry.Empty;

            if (Points == null) return;
            var points = new List<Point>();

            foreach (var point in Points)
            {
                //var pointProperties = point.GetType().GetProperties();
                //if (pointProperties.All(p => p.Name != "X") ||
                //pointProperties.All(p => p.Name != "Y"))
                //    continue;
                //var x = (float)point.GetType().GetProperty("X").GetValue(point, new object[] { });
                //var y = (float)point.GetType().GetProperty("Y").GetValue(point, new object[] { });
                //points.Add(new Point(x, y));
                points.Add(point);
            }

            if (points.Count <= 1)
                return;

            var myPathFigure = new PathFigure { StartPoint = points.FirstOrDefault() };


            var myPathSegmentCollection = new PathSegmentCollection();

            var beizerSegments = InterpolationUtils.InterpolatePointWithBeizerCurves(points, IsClosedCurve);

            if (beizerSegments == null || beizerSegments.Count < 1)
            {
                //Add a line segment <this is generic for more than one line>
                foreach (var point in points.GetRange(1, points.Count - 1))
                {

                    var myLineSegment = new LineSegment { Point = point };
                    myPathSegmentCollection.Add(myLineSegment);
                }
            }
            else
            {
                foreach (var beizerCurveSegment in beizerSegments)
                {
                    var segment = new BezierSegment
                    {
                        Point1 = beizerCurveSegment.FirstControlPoint,
                        Point2 = beizerCurveSegment.SecondControlPoint,
                        Point3 = beizerCurveSegment.EndPoint
                    };
                    myPathSegmentCollection.Add(segment);
                }
            }


            myPathFigure.Segments = myPathSegmentCollection;

            var myPathFigureCollection = new PathFigureCollection { myPathFigure };

            var myPathGeometry = new PathGeometry { Figures = myPathFigureCollection };

            _polylineGeometry = myPathGeometry;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (_polylineGeometry == null)
                    SetPathData();
                return this._polylineGeometry;
            }
        }

        public void Dispose()
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(StrokeProperty, typeof(PolyBezier));
            dpd.RemoveValueChangedSafe(this, OnStrokeChanged);
        }
    }
}
