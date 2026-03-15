#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;
#endif


namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart StepLine segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StepLineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StepLineSegment : ChartSegment
    {

        #region fields
        /// <summary>
        /// Poly property declaration
        /// </summary>
        protected Polyline Poly;

        private StepLineSeries containerSeries;

        private List<Point> listPoints;

        private Point pointStart;

        private Point pointEnd;

        private Point stepMidPoint;

        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the X1 dependency property.
        /// </summary>
        private static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the X2 dependency property.
        /// </summary>
        private static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y1 dependency property.
        /// </summary>
        private static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y2 dependency property.
        /// </summary>
        private static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StepX dependency property.
        /// </summary>
        private static readonly DependencyProperty StepXProperty =
            DependencyProperty.Register("StepX", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the StepY dependency property.
        /// </summary>
        private static readonly DependencyProperty StepYProperty =
            DependencyProperty.Register("StepY", typeof(double), typeof(StepLineSegment), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the Points dependency property.
        /// </summary>
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(PointCollection), typeof(StepLineSegment), new PropertyMetadata(null));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the x1. This is a dependency property.
        /// </summary>
        /// <value>The x1 value.</value>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        /// <summary>
        /// Gets or sets the x2. This is a dependency property.
        /// </summary>
        /// <value>The x2 value.</value>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// Gets or sets the y1. This is a dependency property.
        /// </summary>
        /// <value>The y1 value.</value>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        /// <summary>
        /// Gets or sets the y2. This is a dependency property.
        /// </summary>
        /// <value>The y2 value.</value>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// Gets or sets the step X. This is a dependency property.
        /// </summary>
        /// <value>The step X.</value>
        public double StepX
        {
            get { return (double)GetValue(StepXProperty); }
            set { SetValue(StepXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the step Y. This is a dependency property.
        /// </summary>
        /// <value>The step Y.</value>
        public double StepY
        {
            get { return (double)GetValue(StepYProperty); }
            set { SetValue(StepYProperty, value); }
        }

        /// <summary>
        /// Gets or sets the segment's points. This is a dependency property.
        /// </summary>
        /// <value>The points.</value>
        public PointCollection Points
        {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public double X1Value { get; set; }
        public double Y1Value { get; set; }
        public double X2Value { get; set; }
        public double Y2Value { get; set; }

        private double _yData;
        public double YData
        {
            get { return _yData; }
            set
            {
                _yData = value;
                OnPropertyChanged("YData");
            }
        }
        #endregion

        #region ctor
     

        /// <summary>
        /// Called when instance created for SteplineSegment
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="stepPoint"></param>
        /// <param name="point2"></param>
        /// <param name="series"></param>
        public StepLineSegment(Point point1, Point stepPoint, Point point2, StepLineSeries series)
        {

            base.Series = series;
            this.containerSeries = series;

            listPoints = new List<Point>();
            listPoints.Add(point1);
            listPoints.Add(stepPoint);
            listPoints.Add(point2);
            SetData(listPoints);
           
        }

        

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="linePoints"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(List<Point> linePoints)
        {
            pointStart = linePoints[0];
            stepMidPoint = linePoints[1];
            pointEnd = linePoints[2];
            X1Value = pointStart.X;
            X2Value = pointEnd.X;
            Y1Value = pointStart.Y;
            Y2Value = pointEnd.Y;
            XRange = new DoubleRange(pointStart.X, pointEnd.X);
            YRange = new DoubleRange(pointStart.Y, stepMidPoint.Y);
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
            
                Poly = new Polyline();                
                SetVisualBindings(Poly);
                Poly.Fill = new SolidColorBrush(Colors.Transparent);
                Poly.Tag = this;
                return Poly;
        
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return Poly;
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

            Point point1 = transformer.TransformToVisible(pointStart.X, pointStart.Y);
            Point point2 = transformer.TransformToVisible(pointEnd.X, pointEnd.Y);
            Point stepPoint = transformer.TransformToVisible(stepMidPoint.X, stepMidPoint.Y);
            Points = new PointCollection();
                        
            if (X1 != point1.X || X2 != point2.X || Y1 != point1.Y || Y2 != point2.Y || StepX != stepPoint.X || StepY != stepPoint.Y)
            {
                this.X1 = point1.X;
                this.X2 = point2.X;
                this.Y1 = point1.Y;
                this.Y2 = point2.Y;
                this.StepX = stepPoint.X;
                this.StepY = stepPoint.Y;
                Points.Add(point1);
                Points.Add(point2);
                Points.Add(stepPoint);
                Poly.Points = Points;
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
