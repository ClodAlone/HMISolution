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
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
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
    /// Represents chart bar segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="BarSeries"/>
    [ClassReference(IsReviewed = false)]
    public class BarSegment : ChartSegment
    {

        #region fields
        /// <summary>
        /// Variables declarations
        /// </summary>
        protected double Left = 0d, Top = 0d, Bottom = 0d, Right = 0d;
        /// <summary>
        /// barSegment variable declaration
        /// </summary>
        protected Rectangle barSegment;
        /// <summary>
        /// Variable declaration for SegmentCanvas
        /// </summary>
        protected Canvas SegmentCanvas;
        /// <summary>
        /// Variable declaration for segmentWidth
        /// </summary>
        internal Size segmentSize;

        #endregion

        #region Properties

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
        /// Gets the y data of this segment
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double YData
        {
            get;
            internal set;
        }

        #endregion

        #region ctor

        /// <summary>
        /// Constructor 
        /// </summary>
        public BarSegment()
        {
            segmentSize = new Size();
        }

        /// <summary>
        /// Defines a Column Rect and Range
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public BarSegment(double x1, double y1, double x2, double y2, BarSeries series)
        {
            base.Series = series;
            SetData(x1, y1, x2, y2);
        }

        /// <summary>
        /// Called when instance created for BarSegment with following Parameters
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public BarSegment(double x1, double y1, double x2, double y2)
        {
            SetData(x1, y1, x2, y2);
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
            Left = Values[0];
            Top = Values[1];
            Right = Values[2];
            Bottom = Values[3];
            XRange = new DoubleRange(Left, Right);
            YRange = new DoubleRange(Top, Bottom);            
        }

        #endregion

        #region methods

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
            barSegment = new Rectangle();
            SetVisualBindings(barSegment);
            barSegment.Tag = this;
            return barSegment;

        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return barSegment;
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
                double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
                double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);
                double left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
                double right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;
                if (left >= xStart && left <= xEnd || right >= xStart && right <= xEnd && (!double.IsNaN(YData) || Series.ShowEmptyPoints))
                {
                    Point tlpoint = transformer.TransformToVisible(Left, Top);
                    Point rbpoint = transformer.TransformToVisible(Right, Bottom);
                    Rect rect = new Rect(tlpoint, rbpoint);
                    barSegment.SetValue(Canvas.LeftProperty, rect.X);
                    barSegment.SetValue(Canvas.TopProperty, rect.Y);
                    barSegment.Width = segmentSize.Width = rect.Width;
                    barSegment.Height = segmentSize.Height= rect.Height;
                    //barSegment.RenderTransform = Series.animated
                    //                                 ? null
                    //                                 : new ScaleTransform() {ScaleX = 0};
                }
                else
                    barSegment.ClearUIValues();
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
