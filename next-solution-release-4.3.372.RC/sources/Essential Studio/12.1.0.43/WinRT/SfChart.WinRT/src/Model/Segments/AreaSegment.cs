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
using System.Collections.ObjectModel;
#if WINDOWS_PHONE
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using WindowsLinesegment = System.Windows.Media.LineSegment;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI;
using Windows.UI.Xaml.Data;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
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
    /// Represents chart area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="AreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class AreaSegment : ChartSegment
    {
        #region fields

        private IList<double> XValues, YValues;

        private ChartSeriesBase containerSeries;

	private Canvas segmentCanvas;	

        Path segPath;

        bool segmentUpdated;

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

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public AreaSegment()
        {

        }

        /// <summary>
        /// Called when instance created for AreaSegments
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        public AreaSegment(List<double> xValues, List<double> yValues, AdornmentSeries series, object item)
        {
            base.Series = series;
            containerSeries = series;
            base.Item = item;
            SetData(xValues, yValues);
        }

        /// <summary>
        /// Called when instance created for AreaSegments 
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public AreaSegment(List<double> xValues, IList<double> yValues)
        {
            SetData(xValues, yValues);
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="XValues"></param>
        /// <param name="YValues"></param>
        public override void SetData(IList<double> XValues, IList<double> YValues)
        {
            this.XValues = XValues;
            this.YValues = YValues;
            XRange = new DoubleRange(XValues.Min(), XValues.Max());
            YRange = new DoubleRange(YValues.Min(),YValues.Max());
        }

        #endregion

        #region methods

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
            segmentCanvas = new Canvas();
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            segmentCanvas.Children.Add(segPath);
            return segmentCanvas;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segmentCanvas;
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
            if (transformer != null)
            {
                if (segmentUpdated)
                    Series.SeriesRootPanel.Clip = null;
                PathFigure figure = new PathFigure();
                PathGeometry segmentGeometry = new PathGeometry();
                WindowsLinesegment lineSegment;
                double start = containerSeries.ActualYAxis.ActualRange.Start;
                double origin = containerSeries.ActualXAxis.Origin;
                origin = origin == 0d ? start < 0d ? 0d : start : origin;
                figure.StartPoint = transformer.TransformToVisible(XValues[0], origin);
                for (int index = 0; index < XValues.Count; index++)
                {
                    lineSegment = new WindowsLinesegment();
                    lineSegment.Point = transformer.TransformToVisible(XValues[index], YValues[index]);
                    figure.Segments.Add(lineSegment);
                }
                lineSegment = new WindowsLinesegment();
                lineSegment.Point = transformer.TransformToVisible(XValues[XValues.Count - 1], origin);
                figure.Segments.Add(lineSegment);
                figure.IsClosed = true;
                segmentGeometry.Figures.Add(figure);
                this.segPath.Data = segmentGeometry;
                segmentUpdated = true;
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

        #endregion
    }
}
