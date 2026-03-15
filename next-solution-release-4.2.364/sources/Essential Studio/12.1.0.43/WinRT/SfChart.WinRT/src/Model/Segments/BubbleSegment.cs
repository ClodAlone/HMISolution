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
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Controls;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart bubble segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="BubbleSeries"/>
    [ClassReference(IsReviewed = false)]
    public class BubbleSegment:ChartSegment
    {

        #region members

        private double segmentRadius;

        private Ellipse ellipseSegment;

        private BubbleSeries containerSeries;

        private double xPos=0, yPos = 0;

        /// <summary>
        /// Gets the x data of this segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double XData
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the y data of this segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {

            get;
            internal set;
        }

        /// <summary>
        /// Gets the size of this segment in terms of chart data.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Size
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the segment radius in units of pixels.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double SegmentRadius
        {
            get
            {
                return segmentRadius;
            }
            set
            {
                segmentRadius = value;
            }
        }

        #endregion

        #region Constructor

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="size"></param>
        /// <param name="series"></param>
        public BubbleSegment(double xPos,double yPos,double size,BubbleSeries series)
        {
            base.Series = series;
            SetData(xPos,yPos);
            this.Size = size;
            this.segmentRadius = size;
            containerSeries = series;
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(params double[] Values)
        {
            XData = Values[0];
            YData = Values[1];
            this.xPos = Values[0];
            this.yPos = Values[1];
            XRange = new DoubleRange(xPos,xPos);
            YRange = new DoubleRange(yPos, yPos);
        }

        #endregion

        #region Methods        
        /// <summary>
        /// Method implementation for Set Binding to visual elements 
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
            ellipseSegment = new Ellipse();
            SetVisualBindings(ellipseSegment);
            ellipseSegment.Tag = this;
            return ellipseSegment;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return ellipseSegment;
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
                ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double pos = xIsLogarithmic ? Math.Log(xPos, xBase) : xPos;
                if (pos >= xStart && pos <= xEnd && (!double.IsNaN(yPos) || Series.ShowEmptyPoints))
                {
                    Point point1 = transformer.TransformToVisible(xPos, yPos);
                    ellipseSegment.Height = ellipseSegment.Width = 2 * this.segmentRadius;
                    ellipseSegment.SetValue(Canvas.LeftProperty, point1.X - this.segmentRadius);
                    ellipseSegment.SetValue(Canvas.TopProperty, point1.Y - this.segmentRadius);
                }
                else
                    ellipseSegment.ClearUIValues();
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
