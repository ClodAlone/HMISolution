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
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
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
    /// Represents chart candle segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="CandleSeries"/>
    [ClassReference(IsReviewed = false)]
    public class CandleSegment:ChartSegment
    {
        
        #region fields

        private Point cdpBottomLeft;

        private Point cdpRightTop;

        private Point hiPoint;

        private Point loPoint;

        private CandleSeries containerSeries;

        private bool isbull;

        private Line hiLoLine;

        private Canvas segmentCanvas;

        private Rectangle columnSegment;

        private Brush bullFillColor,bearFillColor;
        #endregion
        
        #region properties

        /// <summary>
        /// Gets the actual color used to paint the interior of the segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush ActualInterior
        {
            get
            {
                return isbull
                    ? BullFillColor:BearFillColor;
            }
        }

        /// <summary>
        /// Gets the high value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BearFillColor
        {
            get
            {
                return bearFillColor == null
                    ? this.Interior : bearFillColor;
            }
            set
            {
                if (bearFillColor != value)
                {
                    bearFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        /// <summary>
        /// Gets the low value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BullFillColor
        {
            get
            {
                return bullFillColor == null
                    ? this.Interior : bullFillColor;
            }
            set
            {
                if (bullFillColor != value)
                {
                    bullFillColor = value;
                    OnPropertyChanged("ActualInterior");
                }
            }
        }

        public double High { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        #endregion
        
        #region constructor

        public CandleSegment()
        {

        }

        /// <summary>
        /// Called when Instance created for CandleSegment
        /// </summary>
        /// <param name="cdpBottomLeft"></param>
        /// <param name="cdpRightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="lopoint"></param>
        /// <param name="isbull"></param>
        /// <param name="series"></param>
        public CandleSegment(Point cdpBottomLeft,Point cdpRightTop,Point hipoint,Point lopoint, 
              bool isbull,CandleSeries series, object item)
        {
            base.Series = series;
            this.containerSeries = series;
            BullFillColor = series.BullFillColor;
            BearFillColor = series.BearFillColor;
            base.Item = item;
            SetData(cdpBottomLeft, cdpRightTop, hipoint, lopoint, isbull);
        }

        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="BottomLeft"></param>
        /// <param name="RightTop"></param>
        /// <param name="hipoint"></param>
        /// <param name="loPoint"></param>
        /// <param name="isBull"></param>
        [ClassReference(IsReviewed = false)]
        public override void SetData(Point BottomLeft, Point RightTop, Point hipoint,Point loPoint,bool isBull)
        {
            this.cdpBottomLeft = BottomLeft;
            this.cdpRightTop = RightTop;
            this.hiPoint = hipoint;
            this.loPoint = loPoint;
            this.isbull = isBull;
            XRange = DoubleRange.Union(new double[] { BottomLeft.X, RightTop.X, hipoint.X, loPoint.X });
            YRange = DoubleRange.Union(new double[] { BottomLeft.Y, RightTop.Y, hipoint.Y, loPoint.Y });        
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
            columnSegment = new Rectangle();
            SetVisualBindings(columnSegment);
            Canvas.SetZIndex(columnSegment, 1);
            segmentCanvas.Children.Add(columnSegment);
            columnSegment.Tag = this;
            hiLoLine = new Line();
            SetVisualBindings(hiLoLine);

            Canvas.SetZIndex(hiLoLine, 0);
            
            segmentCanvas.Children.Add(hiLoLine);
            
            return segmentCanvas;
        }

        /// <summary>
        /// Method Implementation for set  Binding to CgartSegments properties
        /// </summary>
        /// <param name="element"></param>
        protected override void SetVisualBindings(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ActualInterior");
            element.SetBinding(Shape.FillProperty, binding);
            element.SetBinding(Shape.StrokeProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("Stroke");
            element.SetBinding(Shape.StrokeProperty, binding);
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
            ChartTransform.ChartCartesianTransformer cartesianTransformer = transformer as ChartTransform.ChartCartesianTransformer;
            double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
            double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);
            double xBase = cartesianTransformer.XAxis.IsLogarithmic ? (cartesianTransformer.XAxis as LogarithmicAxis).LogarithmicBase : 1;
            bool xIsLogarithmic = cartesianTransformer.XAxis.IsLogarithmic;
            double left = xIsLogarithmic ? Math.Log(cdpBottomLeft.X, xBase) : cdpBottomLeft.X;
            double right = xIsLogarithmic ? Math.Log(cdpRightTop.X, xBase) : cdpRightTop.X;
            if ((left >= xStart && left <= xEnd ||
                  right >= xStart && right <= xEnd)
                 && ((!double.IsNaN(hiPoint.Y) && !double.IsNaN(loPoint.Y) && !double.IsNaN(cdpBottomLeft.Y) && !double.IsNaN(cdpRightTop.Y)) || Series.ShowEmptyPoints)
                    )
            {
                columnSegment.Visibility = Visibility.Visible;
                hiLoLine.Visibility = Visibility.Visible;
                Point blpoint = transformer.TransformToVisible(cdpBottomLeft.X, cdpBottomLeft.Y);
                Point trpoint = transformer.TransformToVisible(cdpRightTop.X, cdpRightTop.Y);
                Rect rect = new Rect(blpoint, trpoint);
                columnSegment.SetValue(Canvas.LeftProperty, rect.X);
                columnSegment.SetValue(Canvas.TopProperty, rect.Y);
                columnSegment.Width = rect.Width;
                columnSegment.Height = rect.Height;

                Point point1 = transformer.TransformToVisible(this.hiPoint.X, this.hiPoint.Y);
                Point point2 = transformer.TransformToVisible(this.loPoint.X, this.loPoint.Y);
                this.hiLoLine.X1 = point1.X;
                this.hiLoLine.X2 = point2.X;
                this.hiLoLine.Y1 = point1.Y;
                this.hiLoLine.Y2 = point2.Y;
            }
            else
            {
                columnSegment.ClearUIValues();
                hiLoLine.ClearUIValues();
                columnSegment.Visibility = Visibility.Collapsed;
                hiLoLine.Visibility = Visibility.Collapsed;
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
        /// Called when Property changed
        /// </summary>
        /// <param name="name"></param>
        protected override void OnPropertyChanged(string name)
        {
            if (name == "Interior")
                name = "ActualInterior";
            base.OnPropertyChanged(name);
        }

        #endregion
    }
}
