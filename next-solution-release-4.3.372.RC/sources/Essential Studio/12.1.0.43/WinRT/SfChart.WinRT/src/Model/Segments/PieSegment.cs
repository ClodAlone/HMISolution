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
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsLineSegment = System.Windows.Media.LineSegment;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart pie segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="PieSeries"/>
    [ClassReference(IsReviewed = false)]
    public class PieSegment : ChartSegment
    {
        #region fields

        private double startAngle;

        private PieSeries parentSeries;

        private Path segmentPath;

        internal Point startPoint;

		private bool isInitializing = true;

        private PathGeometry segmentGeometry;

        private double currentRadius;

        private int pieSeriesCount;

        private int pieIndex;

        private double angleOfSlice;

        private double xData;

        private double yData;

        private double endAngle;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets a value that indicates whether this segment is exploded.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsExploded
        {
            get { return (bool)GetValue(IsExplodedProperty); }
            set { SetValue(IsExplodedProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsExploded.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(PieSegment), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExplodedChanged)));

        private static void OnIsExplodedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PieSegment).OnPropertyChanged("IsExploded");
        }

        
        /// <summary>
        /// Gets the start angle of the <see cref="PieSegment"/>.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double StartAngle
        {
            get
            {
                return startAngle;
            }
            internal set
            {
                startAngle = value;
                if (Series != null && !Series.CanAnimate)
                    ActualStartAngle = value;
                OnPropertyChanged("StartAngle");
            }
        }

        public double ActualStartAngle
        {
            get { return (double)GetValue(ActualStartAngleProperty); }
            set { SetValue(ActualStartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualStartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStartAngleProperty =
            DependencyProperty.Register("ActualStartAngle", typeof(double), typeof(PieSegment), new PropertyMetadata(0d, OnAngleChanged));

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PieSegment).OnAngleChanged(e);
        }

        private void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!isInitializing)
                this.Update(this.Series.CreateTransformer(new Size(), false));
        }

        public double ActualEndAngle
        {
            get { return (double)GetValue(ActualEndAngleProperty); }
            set { SetValue(ActualEndAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualStartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualEndAngleProperty =
            DependencyProperty.Register("ActualEndAngle", typeof(double), typeof(PieSegment), new PropertyMetadata(0d, OnAngleChanged));

        /// <summary>
        /// Gets the end angle of the <see cref="PieSegment"/>.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double EndAngle
        {
            get
            {
                return endAngle;
            }
            internal set
            {
                endAngle = value;
                if (Series != null && !Series.CanAnimate)
                    ActualEndAngle = value;
                OnPropertyChanged("EndAngle");
            }
        }

        /// <summary>
        /// Gets the actual angle the <see cref="PieSegment"/> 
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double AngleOfSlice
        {
            get
            {
                return angleOfSlice;
            }
            internal set
            {
                angleOfSlice = value;
                OnPropertyChanged("AngleOfSlice");
            }
        }

        /// <summary>
        /// Gets the X-Value of the <see cref="PieSegment"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get
            {
                return xData;
            }
            internal set
            {
                xData = value;
                OnPropertyChanged("XData");
            }
        }

        /// <summary>
        /// Gets the Y-Value of the <see cref="PieSegment"/>
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {
            get
            {
                return yData;
            }
            internal set
            {
                yData = value;
                OnPropertyChanged("YData");
            }
        }


        #endregion

        #region constructor

       

        /// <summary>
        /// Called when instance created for PieSegment
        /// </summary>
        /// <param name="arcStartAngle"></param>
        /// <param name="arcEndAngle"></param>
        /// <param name="series"></param>
        public PieSegment(double arcStartAngle, double arcEndAngle, PieSeries series, object item)
        {
            base.Series = series;
            this.StartAngle = arcStartAngle;
            this.EndAngle = arcEndAngle;
            //this.ActualStartAngle = Series.EnableAnimation ? 0 : this.startAngle;
            //this.ActualEndAngle = Series.EnableAnimation ? 0 : this.endAngle;
            this.parentSeries = series;
            pieSeriesCount = series.GetPieSeriesCount();
            pieIndex = GetPieSeriesIndex(series);
            base.Item = item;
            currentRadius = CalculateSegmentRadius(series);
            isInitializing = false;
        }

        /// <summary>
        /// Called when instance created for PieSegment with four arguments
        /// </summary>
        /// <param name="arcStartAngle"></param>
        /// <param name="arcEndAngle"></param>
        /// <param name="isEmptyInterior"></param>
        /// <param name="series"></param>
        public PieSegment(double arcStartAngle, double arcEndAngle, bool isEmptyInterior, PieSeries series, object item)
            : this(arcStartAngle, arcEndAngle, series, item)
        {
            this.IsEmptySegmentInterior = isEmptyInterior;
        }

        #endregion

        #region methods


        internal int GetPieSeriesIndex(ChartSeriesBase currentSeries)
        {
            int index = 0;
            var pieSeries = (from series in parentSeries.Area.VisibleSeries where series is PieSeries select series).ToList();
            return (index = pieSeries.IndexOf(currentSeries)) >= 0 ? index : -1;
        }

        private double CalculateSegmentRadius(PieSeries series)
        {
            if (pieIndex == 0)
                parentSeries.Area.CircularSegmentRadius[pieIndex] = Math.Pow(2, pieSeriesCount);
            else
                parentSeries.Area.CircularSegmentRadius[pieIndex] = parentSeries.Area.CircularSegmentRadius[pieIndex - 1] / 2;

            return parentSeries.Area.CircularSegmentRadius[pieIndex];
        }

        /// <summary>
        /// Used for creating UIElement for rendering this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size">Size of the panel</param>
        /// <returns>
        /// retuns UIElement
        /// </returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement CreateVisual(Size size)
        {
            segmentPath = new Path();
            SetVisualBindings(segmentPath);
            segmentPath.Tag = this;
            return segmentPath;
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Interior");
            element.SetBinding(Shape.FillProperty,binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty,binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty,binding);

        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segmentPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        [ClassReference(IsReviewed = false)]
        public override void Update(IChartTransformer transformer)
        {
            segmentPath.Width = transformer.Viewport.Width;
            segmentPath.Height = transformer.Viewport.Height;
            segmentPath.VerticalAlignment = VerticalAlignment.Center;
            segmentPath.HorizontalAlignment = HorizontalAlignment.Center;
            if (pieIndex == 0)
            {
                Point center = new Point(transformer.Viewport.Width * 0.5d, transformer.Viewport.Height * 0.5d);

                double radius = (parentSeries.PieCoefficient * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height)) / currentRadius;
                parentSeries.Radius = radius;
                if (Math.Round((ActualEndAngle - ActualStartAngle), 2) == 6.28)
                {
                    EllipseGeometry ellipseGeometry = new EllipseGeometry()
                    {
                        Center = center,
                        RadiusX = radius,
                        RadiusY = radius
                    };
                    this.segmentPath.Data = ellipseGeometry;
                }
                else if ((ActualEndAngle - ActualStartAngle) != 0)
                {
                    if (this.IsExploded)
                    {
                        center = new Point(center.X + (parentSeries.ExplodeRadius * Math.Cos(AngleOfSlice)), center.Y + (parentSeries.ExplodeRadius * Math.Sin(AngleOfSlice)));
                    }
                    startPoint = new Point(center.X + radius * Math.Cos(ActualStartAngle), center.Y + radius * Math.Sin(ActualStartAngle));
                    Point endPoint = new Point(center.X + radius * Math.Cos(ActualEndAngle), center.Y + radius * Math.Sin(ActualEndAngle));
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = center;
                    WindowsLineSegment line = new WindowsLineSegment();
                    line.Point = startPoint;
                    figure.Segments.Add(line);

                    ArcSegment seg = new ArcSegment();
                    seg.Point = endPoint;
                    seg.Size = new Size(radius, radius);
                    seg.RotationAngle = ActualEndAngle + ActualStartAngle;
                    seg.IsLargeArc = ActualEndAngle - ActualStartAngle > Math.PI;
                    seg.SweepDirection = SweepDirection.Clockwise;
                    figure.Segments.Add(seg);

                    figure.IsClosed = true;
                    this.segmentGeometry = new PathGeometry();
                    segmentGeometry.Figures = new PathFigureCollection() { figure };
                    this.segmentPath.Data = segmentGeometry;
                }
            }
            else if (pieIndex >= 1)
            {
                double radius = 0.8 * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height) / currentRadius;//((Series.Area.Series.Count * 2) / (Series.Area.Series.IndexOf(Series) + 1));
                parentSeries.Radius = radius;
                double dradius = parentSeries.PieCoefficient * radius;

                Point center = ChartLayoutUtils.GetCenter(transformer.Viewport);

                if (this.IsExploded)
                {
                    center = new Point(center.X + (parentSeries.ExplodeRadius * Math.Cos(AngleOfSlice)), center.Y + (parentSeries.ExplodeRadius * Math.Sin(AngleOfSlice)));
                }
                startPoint = new Point(center.X + radius * Math.Cos(ActualStartAngle), center.Y + radius * Math.Sin(ActualStartAngle));
                Point endPoint = new Point(center.X + radius * Math.Cos(ActualEndAngle), center.Y + radius * Math.Sin(ActualEndAngle));

                if (Math.Round((ActualEndAngle - ActualStartAngle), 2) == 6.28)
                {
                    GeometryGroup geometryGroup = new GeometryGroup();
                    geometryGroup.Children.Add(new EllipseGeometry()
                    {
                        Center = center,
                        RadiusX = radius,
                        RadiusY = radius
                    });
                    geometryGroup.Children.Add(new EllipseGeometry()
                    {
                        Center = center,
                        RadiusX = dradius,
                        RadiusY = dradius
                    });
                    this.segmentPath.Data = geometryGroup;
                }
                else if ((ActualEndAngle - ActualStartAngle) != 0)
                {
                    Point startDPoint = new Point(center.X + dradius * Math.Cos(ActualStartAngle), center.Y + dradius * Math.Sin(ActualStartAngle));
                    Point endDPoint = new Point(center.X + dradius * Math.Cos(ActualEndAngle), center.Y + dradius * Math.Sin(ActualEndAngle));

                    PathFigure figure = new PathFigure();
                    figure.StartPoint = startPoint;

                    ArcSegment arcseg = new ArcSegment();
                    arcseg.Point = endPoint;
                    arcseg.Size = new Size(radius, radius);
                    arcseg.RotationAngle = ActualEndAngle - ActualStartAngle;
                    arcseg.IsLargeArc = ActualEndAngle - ActualStartAngle > Math.PI;
                    arcseg.SweepDirection = SweepDirection.Clockwise;
                    figure.Segments.Add(arcseg);

                    WindowsLineSegment line = new WindowsLineSegment();
                    line.Point = endDPoint;
                    figure.Segments.Add(line);

                    arcseg = new ArcSegment();
                    arcseg.Point = startDPoint;
                    arcseg.Size = new Size(dradius, dradius);
                    arcseg.RotationAngle = ActualEndAngle - ActualStartAngle;
                    arcseg.IsLargeArc = ActualEndAngle - ActualStartAngle > Math.PI;
                    arcseg.SweepDirection = SweepDirection.Counterclockwise;
                    figure.Segments.Add(arcseg);

                    figure.IsClosed = true;
                    this.segmentGeometry = new PathGeometry();
                    segmentGeometry.Figures = new PathFigureCollection() { figure };
                    this.segmentPath.Data = segmentGeometry;
                }
            }
        }

        /// <summary>
        /// Called whenever the segment's size changed. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="size"></param>
        [ClassReference(IsReviewed = false)]
        public override void OnSizeChanged(Size size)
        {

        }

        #endregion
    }
}
