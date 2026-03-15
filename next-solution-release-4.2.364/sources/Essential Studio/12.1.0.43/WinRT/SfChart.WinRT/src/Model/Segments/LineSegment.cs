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
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart line segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="LineSeries"/>
    [ClassReference(IsReviewed = false)]
    public class LineSegment : ChartSegment
    {
        #region fields

        bool segmentUpdated;

        Line line;

        DataTemplate CustomTemplate;

        private double x1;

        private double x2;

        private double y1;

        private double y2;

        /// <summary>
        /// Gets or Sets the x start point of the line
        /// </summary>
        public double X1
        {

            get
            {

                return x1;
            }
            set
            {
                x1 = value;
                OnPropertyChanged("X1");
            }
        }

        /// <summary>
        /// Gets or Sets the x end point of the line
        /// </summary>
        public double X2
        {

            get
            {

                return x2;
            }
            set
            {
                x2 = value;
                OnPropertyChanged("X2");
            }
        }

        /// <summary>
        /// Gets or Sets the y start point of the line
        /// </summary>
        public double Y1
        {

            get
            {

                return y1;
            }
            set
            {
                y1 = value;
                OnPropertyChanged("Y1");
            }
        }

        /// <summary>
        /// Gets or Sets the y end point of the line
        /// </summary>
        public double Y2
        {

            get
            {

                return y2;
            }
            set
            {
                y2 = value;
                OnPropertyChanged("Y2");
            }
        }

        private ChartSeriesBase lineSeries;

        ContentControl control;

        #endregion

        #region properties
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
        /// Called when instance created for LineSegment with following arguments
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="lineSeries"></param>
        public LineSegment(double x1, double y1, double x2, double y2, AdornmentSeries lineSeries, object item)
        {
            base.Series = lineSeries;
            this.lineSeries = lineSeries;;
            base.Item = item;
            SetData(x1, y1, x2, y2);
            if(lineSeries is LineSeries)
                CustomTemplate = (lineSeries as LineSeries).CustomTemplate;
        }

        /// <summary>
        /// Called when instance created for LineSegment 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="X2"></param>
        /// <param name="Y2"></param>
        public LineSegment(double x1, double y1, double X2, double Y2, object item)
        {
            this.X1 = x1;
            this.X2 = X2Value;
            this.Y1 = y1;
            this.Y2 = Y2Value;
            this.Item = item;
        }

        #endregion

        #region methods

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(params double[] Values)
        {
            X1Value = Values[0];
            Y1Value = Values[1];
            X2Value = Values[2];
            Y2Value = Values[3];

            XRange = new DoubleRange(X1Value, X2Value);
            YRange = new DoubleRange(Y1Value, Y2Value);

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
            if (CustomTemplate == null)
            {
                line = new Line();
                line.DataContext = this;
                SetVisualBindings(line);
                line.Tag = this;
                return line;

            }
            else
            {
                control = new ContentControl();
                control.Content = this;
                control.ContentTemplate = CustomTemplate;
                return control;
            }
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return line;
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
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;

            if (cartesianTransformer != null)
            {
                if (segmentUpdated)
                    Series.SeriesRootPanel.Clip = null;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double interval = cartesianTransformer.XAxis.VisibleInterval;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                if (line != null)
                {
                    double left = xIsLogarithmic ? Math.Log(X1Value, xBase) : X1Value;
                    double right = xIsLogarithmic ? Math.Log(X2Value, xBase) : X2Value;
                    if (((left >= xStart && left <= xEnd)
                    || (right >= xStart && right <= xEnd) || (xStart >= left && xStart <= right) )&& ((!double.IsNaN(Y1Value) && !double.IsNaN(Y2Value)) || Series.ShowEmptyPoints))
                    {
                        Point point1 = transformer.TransformToVisible(X1Value, Y1Value);
                        Point point2 = transformer.TransformToVisible(X2Value, Y2Value);
                        line.X1 = point1.X;
                        line.Y1 = point1.Y;

                        line.X2 = point2.X;
                        line.Y2 = point2.Y;
                    }
                    else
                    {
                        line.ClearUIValues();
                    }
                }
                else
                {
                    Point point = transformer.TransformToVisible(X1Value, Y1Value);
                    this.X1 = point.X;
                    this.Y1 = point.Y;
                    point = transformer.TransformToVisible(X2Value, Y2Value);
                    this.X2 = point.X;
                    this.Y2 = point.Y;

                }
                segmentUpdated = true;
            }
            else
            {
                ChartTransform.ChartPolarTransformer polarTransformer = transformer as ChartTransform.ChartPolarTransformer;
                if (line != null)
                {
                    Point point1 = polarTransformer.TransformToVisible(X1Value, Y1Value);
                    Point point2 = polarTransformer.TransformToVisible(X2Value, Y2Value);
                    line.X1 = point1.X;
                    line.Y1 = point1.Y;

                    line.X2 = point2.X;
                    line.Y2 = point2.Y;
                }
                else
                {
                    Point point = transformer.TransformToVisible(X1Value, Y1Value);
                    this.X1 = point.X;
                    this.Y1 = point.Y;
                    point = transformer.TransformToVisible(X2Value, Y2Value);
                    this.X2 = point.X;
                    this.Y2 = point.Y;

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
