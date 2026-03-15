#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using WindowsLinesegment = System.Windows.Media.LineSegment;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsLinesegment = Windows.UI.Xaml.Media.LineSegment;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Represents chart stacking area segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="StackingAreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StackingAreaSegment:ChartSegment
    {
        #region fields

        bool segmentUpdated;

        private List<double> XValues;
        
        private List<double> YValues;
        
        private StackingAreaSeries containerSeries;

        private Path segPath;

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
        /// <summary>
        /// Called when instance created for StackingAreaSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        /// <param name="series"></param>
        public StackingAreaSegment(List<double> xValues,List<double> yValues,StackingAreaSeries series)
        {
            Series = series;
            containerSeries = series;
            SetData(xValues,yValues);
        }

        /// <summary>
        /// Called when instance created for StackingAreaSegment
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        public StackingAreaSegment(List<double> xValues, List<double> yValues)
        {
            SetData(xValues, yValues);
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
            segPath = new Path();
            segPath.Tag = this;
            SetVisualBindings(segPath);
            return segPath;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return null;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="xValues"></param>
        /// <param name="yValues"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(IList<double> xValues, IList<double> yValues)
        {
            XValues = xValues as List<double>;
            YValues = yValues as List<double>;
            double xMax = xValues.Max();
            double yMax = yValues.Max();
            double xMin = xValues.Min();
            double yMin = yValues.Min();
            XRange = new DoubleRange(xMin, xMax);
            YRange = new DoubleRange(yMin, yMax);
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
                figure.StartPoint = transformer.TransformToVisible(XValues[0], YValues[0]);
                for (int index = 0; index < XValues.Count; index++)
                {
                    lineSegment = new WindowsLinesegment();
                    lineSegment.Point = transformer.TransformToVisible(XValues[index], YValues[index]);
                    figure.Segments.Add(lineSegment);
                }
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
    }
}
