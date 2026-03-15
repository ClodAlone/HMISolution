#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents the border element used in drawing the circular gauge control.
    /// </summary>
    public class GaugeBorder : GaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="Offset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(GaugeBorder), new PropertyMetadata(0d, new PropertyChangedCallback(OnOffsetChanged)));

        #endregion

        #region Private members
    
        /// <summary>
        /// The path for gauge border
        /// </summary>
        private Path mborderPath;

        private Path pointerpath;

        internal bool pathonce = false;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the GaugeBorder class.
        /// </summary>
        public GaugeBorder()
        {
            DefaultStyleKey = typeof(GaugeBorder);
            //this.Background = new SolidColorBrush(Colors.Transparent);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see cref="Offset"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback OffsetChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the offset
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        public double Offset
        {
            get
            {
                return (double)GetValue(OffsetProperty);
            }

            set
            {
                SetValue(OffsetProperty, value);
            }
        }
        #endregion

        #region Overrides

        /// <summary>
        /// On Apply Template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mborderPath = this.GetTemplateChild("Path") as Path;
            pointerpath = this.GetTemplateChild("pointercapPath") as Path;
            UpdateVisualStyle();
            this.RefreshGaugeBorder();   
        }

        #endregion

        #region Implementation
        
        /// <summary>
        /// Refreshs the Gauge Border
        /// </summary>
        internal void RefreshGaugeBorder()
        {            
            if (this.mborderPath != null)
            {
                if (this.GaugeElementParent is CircularGauge)
                {
                    CircularGauge gauge = this.GaugeElementParent as CircularGauge;
                    if (gauge.FrameType == GaugeFrameType.Circular)
                    {
                        double length = gauge.Radius - this.Offset;

                        EllipseGeometry ellipseGeometry = new EllipseGeometry();
                        ellipseGeometry.Center = new Point(length / 2, length / 2);
                        ellipseGeometry.RadiusX = length;
                        ellipseGeometry.RadiusY = length;
                        this.mborderPath.Data = ellipseGeometry;
                    }
                    else if (gauge.FrameType == GaugeFrameType.SemiCircular)
                    {
                        double length = gauge.Radius - (2 * this.Offset);

                        PathGeometry path = new PathGeometry();
                        Point centerPoint = new Point((length + this.Offset) / 2, length + (this.Offset / 2));
                        double innerlength = gauge.Radius - (2 * this.Offset);
                        double innerRadius = gauge.Scales[0].PointerCap.PointerCapRadius;
                        double outerRadius = length+innerRadius;
                        Point innerCenterPoint = new Point((innerlength / 2), innerlength + (this.Offset / 2));
                        Point startPoint1 = this.ConvertToStageCoordinates(outerRadius, 180, centerPoint);
                        Point endPoint1 = this.ConvertToStageCoordinates(outerRadius, 0, centerPoint);
                        ArcSegment arc;
                        if (pathonce)
                        {
                            ////PathGeometry pointercappath = new PathGeometry();
                            ////PathFigure figure2 = new PathFigure();
                            ////figure2.StartPoint = new Point(startPoint2.X, startPoint2.Y );

                            ////arc = new ArcSegment();
                            ////arc.RotationAngle = 0;
                            ////arc.IsLargeArc = false;
                            ////arc.SweepDirection = SweepDirection.Counterclockwise;
                            ////arc.Point = new Point(endPoint2.X, endPoint2.Y);
                            ////arc.Size = new Size(innerRadius, innerRadius);
                            ////figure2.Segments.Add(arc);
                            ////pathonce = true;

                            ////pointercappath.Figures.Add(figure2);
                            ////this.pointerpath.Data = pointercappath;
                            ////this.pointerpath.Fill = gauge.OuterFrameBrush;
                            ////pointerpath.Visibility = Visibility.Visible;
                            ////pointerpath.SetValue(Canvas.ZIndexProperty, 10);
                        }

                        PathFigure figure1 = new PathFigure();
                        figure1.StartPoint = new Point(startPoint1.X, (startPoint1.Y + innerRadius) - (this.Offset / 2));

                        arc = new ArcSegment();

                        arc.RotationAngle = 0;
                        arc.IsLargeArc = false;
                        arc.SweepDirection = SweepDirection.Clockwise;
                        arc.Point = new Point(endPoint1.X, (endPoint1.Y + innerRadius) - (this.Offset/2));
                        arc.Size = new Size(outerRadius - this.Offset, (length + (2 * innerRadius)) - (2 * this.Offset));
                        figure1.Segments.Add(arc);

                        LineSegment line = new LineSegment();
                        line.Point = new Point(startPoint1.X, (startPoint1.Y + innerRadius) - (this.Offset / 2));
                        figure1.Segments.Add(line);

                        path.Figures.Add(figure1);
                        this.mborderPath.Data = path;                        
                    }
                    else
                    {                        
                        double length = gauge.Radius - (2 * this.Offset);
                        PathGeometry path = new PathGeometry();
                        Point centerPoint = new Point((length + this.Offset) / 2, length + (this.Offset / 2));
                        double outerRadius = length;
                        double innerRadius = gauge.Scales[0].PointerCap.PointerCapRadius;
                        Point startPoint1 = this.ConvertToStageCoordinates(outerRadius, 180, centerPoint);
                        Point endPoint1 = this.ConvertToStageCoordinates(outerRadius, 270, centerPoint);
                        Point lastPoint = this.ConvertToStageCoordinates(outerRadius, 0, centerPoint);
                        Point startPoint2 = this.ConvertToStageCoordinates(innerRadius, 180, centerPoint);
                        ArcSegment arc;
                        lastPoint.Y += innerRadius / 2;
                        endPoint1.Y += innerRadius / 2;
                        lastPoint.X -= innerRadius / 2;
                        PathFigure figure1 = new PathFigure();
                        figure1.StartPoint = new Point(startPoint2.X + this.Offset-10, endPoint1.Y +10);

                        LineSegment line = new LineSegment();
                        line.Point = new Point((startPoint2.X + this.Offset) - 10, (endPoint1.Y + (this.Offset / 2)) + 10);
                        figure1.Segments.Add(line);

                        QuadraticBezierSegment qarc = new QuadraticBezierSegment();
                        qarc.Point1 = new Point((startPoint2.X + this.Offset) - 10, endPoint1.Y + (this.Offset / 2));
                        qarc.Point2 = new Point(startPoint2.X + this.Offset, endPoint1.Y + (this.Offset / 2));
                        figure1.Segments.Add(qarc);

                        arc = new ArcSegment();
                        arc.RotationAngle = 0;
                        arc.IsLargeArc = false;
                        arc.SweepDirection = SweepDirection.Clockwise;
                        arc.Point = new Point((lastPoint.X + (this.Offset/2)) - innerRadius, lastPoint.Y - 10);
                        arc.Size = new Size(outerRadius, length);
                        figure1.Segments.Add(arc);

                        qarc = new QuadraticBezierSegment();
                        qarc.Point1 = new Point(lastPoint.X + (this.Offset / 2) - innerRadius, lastPoint.Y);
                        qarc.Point2 = new Point((lastPoint.X + (this.Offset / 2)) - (2* innerRadius), lastPoint.Y);
                        figure1.Segments.Add(qarc);

                        line = new LineSegment();
                        line.Point = new Point((startPoint2.X + this.Offset) + 10, lastPoint.Y);
                        figure1.Segments.Add(line);

                        qarc = new QuadraticBezierSegment();
                        qarc.Point1 = new Point(startPoint2.X + this.Offset - 10, lastPoint.Y);
                        qarc.Point2 = new Point(startPoint2.X + this.Offset - 10, lastPoint.Y - 10); 
                        figure1.Segments.Add(qarc);

                        path.Figures.Add(figure1);
                        this.mborderPath.Data = path;   
                    }
                }
                else if (this.GaugeElementParent is LinearGauge)
                {
                    LinearGauge gauge = this.GaugeElementParent as LinearGauge;

                    double width = gauge.GetDefaultSize().Width;
                    double height = gauge.GetDefaultSize().Height;
                    if (!double.IsNaN(gauge.Height) && !double.IsNaN(gauge.Width))
                    {
                        width = gauge.Width;
                        height = gauge.Height;
                    }
                    else if (gauge.ActualHeight != 0 && gauge.ActualWidth != 0)
                    {
                        width = gauge.ActualWidth;
                        height = gauge.ActualHeight;
                    }

                    RectangleGeometry rectGeometry = new RectangleGeometry();
                    if (width >= this.Offset)
                    {
                        width -= this.Offset;
                    }

                    if (height >= this.Offset)
                    {
                        height -= this.Offset;
                    }

                    rectGeometry.Rect = new Rect(this.Offset/2, this.Offset/2, width, height);
                    rectGeometry.RadiusX = gauge.RadiusX;
                    rectGeometry.RadiusY = gauge.RadiusY;

                    this.mborderPath.Data = rectGeometry;
                    this.mborderPath.StrokeThickness = gauge.BorderThickness.Left;
                }
                else if (this.GaugeElementParent is DigitalGauge)
                {
                    DigitalGauge gauge = this.GaugeElementParent as DigitalGauge;

                    double width = gauge.GetDefaultSize().Width;
                    double height = gauge.GetDefaultSize().Height;
                    if (!double.IsNaN(gauge.Height) && !double.IsNaN(gauge.Width))
                    {
                        width = gauge.Width;
                        height = gauge.Height;
                    }
                    else if (gauge.ActualHeight != 0 && gauge.ActualWidth != 0)
                    {
                        width = gauge.ActualWidth;
                        height = gauge.ActualHeight;
                    }

                    RectangleGeometry rectGeometry = new RectangleGeometry();
                    if (width >= this.Offset)
                    {
                        width -= this.Offset;
                    }

                    if (height >= this.Offset)
                    {
                        height -= this.Offset;
                    }

                    rectGeometry.Rect = new Rect(this.Offset / 2, this.Offset / 2, width, height);                  
                    this.mborderPath.Data = rectGeometry;
                    this.mborderPath.StrokeThickness = gauge.BorderThickness.Left;
                }                
               
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Converts polar coordinates to stage coordinates.
        /// </summary>
        /// <param name="radius">Radius of the border</param>
        /// <param name="angle">Angle of the border</param>
        /// <param name="pointToShift">Point to shift</param>
        /// <returns>Returns point</returns>
        internal Point ConvertToStageCoordinates(double radius, double angle, Point pointToShift)
        {
            Point point = new Point(radius * Math.Cos(angle * Math.PI / 180), radius * Math.Sin(angle * Math.PI / 180));
            point.X += pointToShift.X;
            point.Y += pointToShift.Y;
            return point;
        }

        /// <summary>
        /// Measures the Size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <returns>Returns Size</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            Size size = availableSize;
            if (this.GaugeElementParent is CircularGauge)
            {
                CircularGauge gauge = this.GaugeElementParent as CircularGauge;
                if (gauge.Radius > this.Offset)
                {
                    size = new Size(gauge.Radius - this.Offset, gauge.Radius - this.Offset);
                }
                else
                {
                    size = new Size();
                }
            }
            else if (this.GaugeElementParent is LinearGauge)
            {
                LinearGauge gauge = this.GaugeElementParent as LinearGauge;
                double width = gauge.GetDefaultSize().Width;
                double height = gauge.GetDefaultSize().Height;
                if (!double.IsNaN(gauge.Height) && !double.IsNaN(gauge.Width))
                {
                    width = gauge.Width;
                    height = gauge.Height;
                }
                else if (gauge.ActualHeight != 0 && gauge.ActualWidth != 0)
                {
                    width = gauge.ActualWidth;
                    height = gauge.ActualHeight;
                }

                if (width >= this.Offset)
                {
                    width -= this.Offset;
                }

                if (height >= this.Offset)
                {
                    height -= this.Offset;
                }

                    size = new Size(width, height);
            }
            else if (this.GaugeElementParent is DigitalGauge)
            {
                DigitalGauge gauge = this.GaugeElementParent as DigitalGauge;
                double width = gauge.GetDefaultSize().Width;
                double height = gauge.GetDefaultSize().Height;
                if (!double.IsNaN(gauge.Height) && !double.IsNaN(gauge.Width))
                {
                    width = gauge.Width;
                    height = gauge.Height;
                }
                else if (gauge.ActualHeight != 0 && gauge.ActualWidth != 0)
                {
                    width = gauge.ActualWidth;
                    height = gauge.ActualHeight;
                }

                if (width >= this.Offset)
                {
                    width -= this.Offset;
                }

                if (height >= this.Offset)
                {
                    height -= this.Offset;
                }

                size = new Size(width, height);
            }

                return size;           
        }

        /// <summary>
        /// Updates property value cache and raises OffsetChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnOffsetChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.OffsetChanged != null)
            {
                this.OffsetChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnOffsetChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeBorder instance = (GaugeBorder)d;
            instance.OnOffsetChanged(e);
        }

        #endregion
    }
}
