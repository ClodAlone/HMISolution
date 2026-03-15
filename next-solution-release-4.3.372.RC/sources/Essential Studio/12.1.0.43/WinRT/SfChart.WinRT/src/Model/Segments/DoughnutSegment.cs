#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.UI.Xaml.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using WindowsLineSegment = System.Windows.Media.LineSegment;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLineSegment = Windows.UI.Xaml.Media.LineSegment;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for DoughnutSegment
    /// </summary>
    public class DoughnutSegment:ChartSegment
    {
        #region Fields

        Path segmentPath;

        private double xData,yData,angleOfSlice,currentRadius;

        private DoughnutSeries parentSeries;

        internal Point startPoint;

        private int doughnutIndex, doughnutSeriesCount;

        private bool isInitializing = true;

        private PathGeometry pathGeometry;

        #endregion

        #region Properties
        public double ActualStartAngle
        {
            get { return (double)GetValue(ActualStartAngleProperty); }
            set { SetValue(ActualStartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualStartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStartAngleProperty =
            DependencyProperty.Register("ActualStartAngle", typeof(double), typeof(DoughnutSegment), new PropertyMetadata(0d, OnAngleChanged));

        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DoughnutSegment).OnAngleChanged(e);
        }

        private void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!isInitializing )
                this.Update(this.Series.CreateTransformer(new Size(), false));
        }

        public double ActualEndAngle
        {
            get { return (double)GetValue(ActualEndAngleProperty); }
            set { SetValue(ActualEndAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualStartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualEndAngleProperty =
            DependencyProperty.Register("ActualEndAngle", typeof(double), typeof(DoughnutSegment), new PropertyMetadata(0d, OnAngleChanged));


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
            DependencyProperty.Register("IsExploded", typeof(bool), typeof(DoughnutSegment), new PropertyMetadata(false, new PropertyChangedCallback(OnIsExplodedChaned)));

        
        private double startAngle;
        /// <summary>
        /// Gets the start angle of the <see cref="DoughnutSegment"/>.
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

        private double endAngle;
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

        #region CallBacks

        private static void OnIsExplodedChaned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DoughnutSegment).OnPropertyChanged("IsExploded");
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Called when instance created for DoughnutSegment
        /// </summary>
        /// <param name="startAngle"></param>
        /// <param name="endAngle"></param>
        /// <param name="series"></param>
        public DoughnutSegment(double startAngle, double endAngle, DoughnutSeries series)
        {
            base.Series = series;
            this.StartAngle = startAngle;
            this.EndAngle = endAngle;
            this.parentSeries = series;
            doughnutSeriesCount = series.GetDoughnutSeriesCount();
            doughnutIndex = GetDoughnutSeriesIndex(series);
            currentRadius = CalculateSegmentRadius(series);
            isInitializing = false;
        }

        #endregion

        #region Methods

        internal int GetDoughnutSeriesIndex(ChartSeriesBase currentSeries)
        {
            int index = 0;
            var doughnutSeries = (from series in parentSeries.Area.VisibleSeries where series.GetType() == typeof(DoughnutSeries) select series).ToList();
            return (index = doughnutSeries.IndexOf(currentSeries)) >= 0 ? index : -1;
        }

        private double CalculateSegmentRadius(DoughnutSeries series)
        {
            if (doughnutIndex == 0)
                parentSeries.Area.CircularSegmentRadius[doughnutIndex] = Math.Pow(2, doughnutSeriesCount);
            else
                parentSeries.Area.CircularSegmentRadius[doughnutIndex] = parentSeries.Area.CircularSegmentRadius[doughnutIndex - 1] / 2;

            return parentSeries.Area.CircularSegmentRadius[doughnutIndex];
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
           element.SetBinding(Shape.FillProperty, binding);
           binding = new Binding();
           binding.Source = this;
           binding.Path = new PropertyPath("Stroke");
           element.SetBinding(Shape.StrokeProperty, binding);
           binding = new Binding();
           binding.Source = this;
           binding.Path = new PropertyPath("StrokeThickness");
           element.SetBinding(Shape.StrokeThicknessProperty, binding);
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
            {
                Point center = new Point(transformer.Viewport.Width * 0.5d, transformer.Viewport.Height * 0.5d);

                double radius = parentSeries.Radius = (DoughnutSeries.DOUGHNUTSIZE * Math.Min(transformer.Viewport.Width, transformer.Viewport.Height)) / currentRadius;
                double dradius = parentSeries.DoughnutCoefficient * radius * (doughnutIndex + 1);

                if (Math.Round((ActualEndAngle - ActualStartAngle), 2) == 6.28)
                {
                    GeometryGroup geometryGroup = new GeometryGroup();
                    geometryGroup.FillRule = FillRule.EvenOdd;
                    

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
                    if (this.IsExploded)
                    {
                        center = new Point(center.X + (parentSeries.ExplodeRadius * Math.Cos(AngleOfSlice)), center.Y + (parentSeries.ExplodeRadius * Math.Sin(AngleOfSlice)));
                    }

                    startPoint = new Point(center.X + radius * Math.Cos(ActualStartAngle), center.Y + radius * Math.Sin(ActualStartAngle));
                    Point endPoint = new Point(center.X + radius * Math.Cos(ActualEndAngle), center.Y + radius * Math.Sin(ActualEndAngle));
                    Point startDPoint = new Point(center.X + dradius * Math.Cos(ActualStartAngle), center.Y + dradius * Math.Sin(ActualStartAngle));
                    Point endDPoint = new Point(center.X + dradius * Math.Cos(ActualEndAngle), center.Y + dradius * Math.Sin(ActualEndAngle));
                    
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = startPoint;
                    ArcSegment seg = new ArcSegment();
                    seg.Point = endPoint;
                    seg.Size = new Size(radius, radius);
                    seg.RotationAngle = ActualEndAngle - ActualStartAngle;
                    seg.IsLargeArc = ActualEndAngle - ActualStartAngle > Math.PI;
                    seg.SweepDirection = SweepDirection.Clockwise;
                    figure.Segments.Add(seg);

                    WindowsLineSegment line = new WindowsLineSegment();
                    line.Point = endDPoint;
                    figure.Segments.Add(line);

                    seg = new ArcSegment();
                    seg.Point = startDPoint;
                    seg.Size = new Size(dradius, dradius);
                    seg.RotationAngle = ActualEndAngle - ActualStartAngle;
                    seg.IsLargeArc = ActualEndAngle - ActualStartAngle > Math.PI;
                    seg.SweepDirection = SweepDirection.Counterclockwise;
                    figure.Segments.Add(seg);
                    figure.IsClosed = true;

                    this.pathGeometry = new PathGeometry();
                    pathGeometry.Figures = new PathFigureCollection() { figure };
                    this.segmentPath.Data = pathGeometry;
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
