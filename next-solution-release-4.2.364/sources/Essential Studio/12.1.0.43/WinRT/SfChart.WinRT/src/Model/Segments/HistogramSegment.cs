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
using Syncfusion.UI.Xaml.Charts;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart Histogram segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HistogramSeries"/>
    [ClassReference(IsReviewed = false)]
    public class HistogramSegment : ColumnSegment
    {
        #region fields

        #endregion
        #region constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public HistogramSegment(double x1,double y1,double x2,double y2, HistogramSeries series):base(x1,y1,x2,y2)
        {
            base.Series = series;
        }

        #endregion

    }

    /// <summary>
    /// Class implementation for HistogramDistributionSegment
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class HistogramDistributionSegment : ChartSegment
    {

        #region fields

        private HistogramSeries containerSeries;

        private Polyline polyLine;

        private PointCollection Points;

        private PointCollection distributionPoints;

        #endregion

        #region constructor  
   
        /// <summary>
        /// Called when instance created for HistogramDistributionSegment
        /// </summary>
        /// <param name="distributionPoints"></param>
        /// <param name="series"></param>
        public HistogramDistributionSegment(PointCollection distributionPoints, HistogramSeries series)
        {
            base.Series = series;
            this.containerSeries = series;
            this.distributionPoints = distributionPoints;
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
            polyLine = new Polyline();
            SetVisualBindings(polyLine);
            return polyLine;
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
            return polyLine;
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
            this.Points = new PointCollection();

            foreach (var item in distributionPoints)
            {
                Point point = transformer.TransformToVisible(item.X,item.Y);
                Points.Add(point);
            }
            this.polyLine.Points = Points;
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
