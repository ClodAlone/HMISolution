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
using System.Windows.Media;
using System.Windows.Shapes;
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart HiLo segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="HiLoSeries"/>
    [ClassReference(IsReviewed = false)]
    public class HiLoSegment : ChartSegment
    {
        #region fields

        private double lowValue;

        private double highValue;

        private double xVal;

        private HiLoSeries containerSeries;

        private Line segLine;

        #endregion

        #region properties

        public double High { get; set; }

        public double Low { get; set; }

        public object XValue { get; set; }
        #endregion

        #region constructor

        /// <summary>
        /// Called when instance created for HiLoSegment
        /// </summary>
        /// <param name="xVal"></param>
        /// <param name="hghValue"></param>
        /// <param name="lwValue"></param>
        /// <param name="series"></param>
        public HiLoSegment(double xVal, double hghValue, double lwValue, HiLoSeries series, object item)
        {
            base.Series = series;
            containerSeries = series;
            base.Item = item;
            SetData(xVal, hghValue, lwValue);
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
            this.highValue = Values[1];
            this.lowValue = Values[2];
            this.xVal = Values[0];
            XRange = new DoubleRange(Values[0], Values[0]);
            if (!double.IsNaN(Values[1]) || !double.IsNaN(Values[2]))
                YRange = DoubleRange.Union(Values[1], Values[2]);
            else
                YRange = DoubleRange.Empty;
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
            segLine = new Line();
            segLine.Tag = this;
            SetVisualBindings(segLine);
            return segLine;
        }

        /// <summary>
        /// Gets the UIElement used for rendering this segment.
        /// </summary>
        /// <returns>reurns UIElement</returns>
        [ClassReference(IsReviewed = false)]
        public override UIElement GetRenderedVisual()
        {
            return segLine;
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
                double xStart = cartesianTransformer.XAxis.VisibleRange.Start;
                double xEnd = cartesianTransformer.XAxis.VisibleRange.End;
                double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
                bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
                double edgeValue = xIsLogarithmic ? Math.Log(xVal, xBase) : xVal;
                if (edgeValue >= xStart && edgeValue <= xEnd && ((!double.IsNaN(highValue) && !double.IsNaN(lowValue)) || Series.ShowEmptyPoints))
                {
                Point hipoint = transformer.TransformToVisible(xVal, highValue);
                Point lopoint = transformer.TransformToVisible(xVal, lowValue);
                segLine.X1 = hipoint.X;
                segLine.Y1 = hipoint.Y;
                segLine.X2 = lopoint.X;
                segLine.Y2 = lopoint.Y;
                }
                else
                {
                    segLine.ClearUIValues();
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
