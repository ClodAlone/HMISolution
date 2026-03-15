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

namespace Pipeline
{
    public class Pipeline : Shape, IDisposable
    {
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", 
            typeof(PointCollection), typeof(Pipeline), 
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ShadesProperty = DependencyProperty.Register("Shades",
            typeof(int), typeof(Pipeline),
            new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public PointCollection Points
        {
            set { SetValue(PointsProperty, value); }
            get { return (PointCollection)GetValue(PointsProperty); }
        }

        public int Shades
        {
            set { SetValue(ShadesProperty, value); }
            get { return (int)GetValue(ShadesProperty); }
        }

        private Geometry _polylineGeometry;

        public Pipeline()
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(StrokeProperty, typeof(Pipeline));
            dpd.AddValueChangedSafe(this, OnColorChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FillProperty, typeof(Pipeline));
            dpd.AddValueChangedSafe(this, OnColorChanged);
        }

        private void OnColorChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        internal void CacheDefiningGeometry()
        {
            PointCollection points = this.Points;
            PathFigure figure = new PathFigure();
            if (points == null)
            {
                this._polylineGeometry = Geometry.Empty;
            }
            else
            {
                if (points.Count > 0)
                {
                    figure.StartPoint = points[0];
                    if (points.Count > 1)
                    {
                        Point[] pointArray = new Point[points.Count - 1];
                        for (int i = 1; i < points.Count; i++)
                        {
                            pointArray[i - 1] = points[i];
                        }
                        figure.Segments.Add(new PolyLineSegment(pointArray, true));
                    }
                }
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                geometry.FillRule = System.Windows.Media.FillRule.EvenOdd;
                if (geometry.Bounds == Rect.Empty)
                {
                    this._polylineGeometry = Geometry.Empty;
                }
                else
                {
                    this._polylineGeometry = geometry;
                }
            }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                CacheDefiningGeometry();
                return this._polylineGeometry;
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (Points == null || Points.Count < 2 || Stroke == null)
                return;

            for (int shade = Shades; shade >= 0; --shade)
                DrawLines(dc, shade);
        }

        void DrawLines(DrawingContext dc, int shade)
        {
            Color color1 = Colors.White;
            Color color2 = Colors.Black;

            if (Stroke is SolidColorBrush)
            {
                color2 = (Stroke as SolidColorBrush).Color;
            }
            if (Fill is SolidColorBrush)
            {
                color1 = (Fill as SolidColorBrush).Color;
            }

            int shades = Math.Max(Shades, 1);
            var percentage = (float)shade / (float)shades;
            Brush brush = new SolidColorBrush(InterpolateColors(color1, color2, percentage));
            var penStart = new Pen()
            {
                Brush = brush,
                Thickness = StrokeThickness * percentage,
                StartLineCap = StrokeStartLineCap,
                EndLineCap = PenLineCap.Round
            };
            var pen = new Pen()
            {
                Brush = brush,
                Thickness = StrokeThickness * percentage,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            var penEnd = new Pen()
            {
                Brush = brush,
                Thickness = StrokeThickness * percentage,
                StartLineCap = PenLineCap.Round,
                EndLineCap = StrokeEndLineCap
            };

            for (int i = 0; i < Points.Count - 1; ++i)
            {
                if (i == 0)
                    dc.DrawLine(penStart, Points[i], Points[i + 1]);
                else if (i == Points.Count - 2)
                    dc.DrawLine(penEnd, Points[i], Points[i + 1]);
                else
                    dc.DrawLine(pen, Points[i], Points[i + 1]);
            }
        }

        public static Color InterpolateColors(Color color1, Color color2, float percentage)
        {
            double a1 = color1.A / 255.0, r1 = color1.R / 255.0, g1 = color1.G / 255.0, b1 = color1.B / 255.0;
            double a2 = color2.A / 255.0, r2 = color2.R / 255.0, g2 = color2.G / 255.0, b2 = color2.B / 255.0;

            byte a3 = Convert.ToByte((a1 + (a2 - a1) * percentage) * 255);
            byte r3 = Convert.ToByte((r1 + (r2 - r1) * percentage) * 255);
            byte g3 = Convert.ToByte((g1 + (g2 - g1) * percentage) * 255);
            byte b3 = Convert.ToByte((b1 + (b2 - b1) * percentage) * 255);
            return Color.FromArgb(a3, r3, g3, b3);
        }

        public void Dispose()
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(StrokeProperty, typeof(Pipeline));
            dpd.RemoveValueChangedSafe(this, OnColorChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FillProperty, typeof(Pipeline));
            dpd.RemoveValueChangedSafe(this, OnColorChanged);
        }
    }
}
