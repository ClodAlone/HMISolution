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
using WindowsLinesegment = System.Windows.Media.LineSegment;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Class implementation for StepAreaSegment
    /// </summary>
    public class StepAreaSegment : ChartSegment
    {

        #region fields

        Path segPath;

        List<Point> stepAreaPoints = new List<Point>();

        #endregion

        #region ctor

       

        /// <summary>
        /// Called when instance created for StepAreaSegment
        /// </summary>
        /// <param name="pointsCollection"></param>
        /// <param name="series"></param>
        public StepAreaSegment(List<Point> pointsCollection, StepAreaSeries series)
        {
            base.Series = series;
           // this.stepAreaPoints = pointsCollection;
            this.XRange = DoubleRange.Empty;
            this.YRange = DoubleRange.Empty;

            this.SetData(pointsCollection);         
        }
        #endregion

        #region properties
        private double _xData;
        public double XData
        {
            get { return _xData; }
            set
            {
                _xData = value;
                OnPropertyChanged("XData");
            }
        }

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

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="StepAreaPoints"></param>
        public override void SetData(List<Point> StepAreaPoints)
        {
            this.stepAreaPoints = StepAreaPoints;
            foreach (Point pt in StepAreaPoints)
            {
                XRange += pt.X;
                YRange += pt.Y;
            }   
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
        public override UIElement CreateVisual(Size size)
        {
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            return segPath;
        
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        public override UIElement GetRenderedVisual()
        {
            return segPath;
        }

        /// <summary>
        /// Updates the segments based on its data point value. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="transformer">Reresents the view port of chart control.(refer <see cref="IChartTransformer"/>)</param>
        public override void Update(IChartTransformer transformer)
        {
            PathFigure figure = new PathFigure();
            if (this.stepAreaPoints.Count > 0)
            {
                figure.StartPoint = transformer.TransformToVisible(this.stepAreaPoints[0].X, this.stepAreaPoints[0].Y);
                PathGeometry segmentGeometry = new PathGeometry();
                WindowsLinesegment linesegment;
                for (int i = 1; i < this.stepAreaPoints.Count; i++)
                {
                    
                    if (this.stepAreaPoints[i] != null)
                    { 
                        linesegment = new WindowsLinesegment();
                        linesegment.Point = transformer.TransformToVisible(this.stepAreaPoints[i].X, this.stepAreaPoints[i].Y);
                        figure.Segments.Add(linesegment);
                    }
                }

                figure.IsClosed = true;
                segmentGeometry.Figures.Add(figure);
                this.segPath.Data = segmentGeometry;
            }
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        [ClassReference(IsReviewed = false)]
        protected override void SetVisualBindings(Shape element)
        {
            base.SetVisualBindings(element);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
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