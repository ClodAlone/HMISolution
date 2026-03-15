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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Represents chart column segment.
    /// </summary>
    /// <remarks>Class instance is created automatically by WINRT Chart building system.</remarks>
    /// <seealso cref="ColumnSeries"/>
    [ClassReference(IsReviewed = false)]
    public class ColumnSegment : ChartSegment
    {

        #region fields
        /// <summary>
        /// Variables declarations
        /// </summary>
        protected double Left = 0d, Top = 0d, Bottom = 0d, Right = 0d;

        private double rectX, rectY,width,height;
        /// <summary>
        /// RectSegment property declarations
        /// </summary>
        protected Rectangle RectSegment;
        
        private ColumnSeries containerSeries;

        private DataTemplate customTemplate;

    /// <summary>
    /// Get or Set XData property
    /// </summary>
    [ClassReference(IsReviewed = false)]
        public double XData
        {
            get;
            internal set;
        }

    /// <summary>
    /// Get or Set YData property
    /// </summary>
    [ClassReference(IsReviewed = false)]
        public double YData
        {
            get;
            internal set;
        }
        
        /// <summary>
        /// Get or Set Width property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Width
        {

            get
            {

                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged("Width");
            }
        }

        /// <summary>
        /// Get or Set Height property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Height
        {

            get
            {

                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged("Height");
            }
        }

        /// <summary>
        /// Get or Set RectX property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RectX
        {

            get
            {

                return rectX;
            }
            set
            {
                rectX = value;
                OnPropertyChanged("RectX");
            }
        }

        /// <summary>
        /// Get or Set RectY property
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RectY
        {

            get
            {

                return rectY;
            }
            set
            {
                rectY = value;
                OnPropertyChanged("RectY");
            }
        }

        #endregion

        #region ctor


        /// <summary>
        /// Defines the Column Rectangle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="series"></param>
        public ColumnSegment(double x1, double y1, double x2, double y2, ColumnSeries series)
        {
            base.Series = series;
            containerSeries = series;
            customTemplate = series.CustomTemplate;
            SetData(x1, y1, x2, y2);
        }

        /// <summary>
        /// Called when instance created for ColumnSegment
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public ColumnSegment(double x1, double y1, double x2, double y2)
        {
            SetData(x1, y1, x2, y2);
        }


        /// <summary>
        /// Sets the values for this segment. This method is not
        /// intended to be called explicitly outside the Chart but it can be overriden by
        /// any derived class.
        /// </summary>
        /// <param name="Values"></param>
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
        /// method implementation for Set Bindings to properties in ColumnSegement
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
            if (customTemplate == null)
            {
                RectSegment = new Rectangle();
                SetVisualBindings(RectSegment);
                RectSegment.Tag = this;
                return RectSegment;
            }
            else
            {
                ContentControl control = new ContentControl();
                control.Content = this;
                control.ContentTemplate = containerSeries.CustomTemplate;
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
            return RectSegment;
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
                double left = xIsLogarithmic ? Math.Log(Left, xBase) : Left;
                double right = xIsLogarithmic ? Math.Log(Right, xBase) : Right;
                double xStart = Math.Floor(cartesianTransformer.XAxis.VisibleRange.Start);
                double xEnd = Math.Ceiling(cartesianTransformer.XAxis.VisibleRange.End);

                if (RectSegment != null)
                {
                    if (left >= xStart && left <= xEnd || right >= xStart && right <= xEnd && (!double.IsNaN(YData) || Series.ShowEmptyPoints))
                    {
                        Point tlpoint = transformer.TransformToVisible(Left, Top);
                        Point rbpoint = transformer.TransformToVisible(Right, Bottom);
                        Rect rect = new Rect(tlpoint, rbpoint);
                        RectSegment.SetValue(Canvas.LeftProperty, rect.X);
                        RectSegment.SetValue(Canvas.TopProperty, rect.Y);
                        Width = RectSegment.Width = rect.Width;
                        Height = RectSegment.Height = rect.Height;
                    }
                    else
                        RectSegment.ClearUIValues();
                }
                else
                {
                    Point tlpoint = transformer.TransformToVisible(Left, Top);
                    Point rbpoint = transformer.TransformToVisible(Right, Bottom);
                    Rect rect = new Rect(tlpoint, rbpoint);
                    RectX = rect.X;
                    RectY = rect.Y;
                    Width = rect.Width;
                    Height = rect.Height;
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
