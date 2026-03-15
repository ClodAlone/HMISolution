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
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class ErrorBarSegment :ChartSegment
    {
        #region fields

        private Canvas _canvas;

        private Point _verToppoint;
        private Point _verBottompoint;
        private Point _horLeftpoint;
        private Point _horRightpoint;

        
        private readonly ErrorBarSeries _parentSeries;

        internal Line HorLine;

        internal Line VerLine;

        internal Line HorLeftCapLine;

        internal Line HorRightCapLine;

        internal Line VerTopCapLine;

        internal Line VerBottomCapLine;

        
        #endregion

      
        public ErrorBarSegment()
        {
        }
        public ErrorBarSegment(Point hlpoint, Point hrpoint, Point vtpoint, Point vbpoint, ErrorBarSeries series, object item)
        {
            base.Series = series;
            _parentSeries = series;
            base.Item = item;
            SetData(hlpoint, hrpoint, vtpoint, vbpoint);
        }
        public override void SetData(Point hlpoint, Point hrpoint, Point vtpoint, Point vbpoint)
        {
            _horLeftpoint = hlpoint;
            _horRightpoint = hrpoint;
            _verToppoint = vtpoint;
            _verBottompoint = vbpoint;
           
            switch (_parentSeries.Mode)
            {
                case ErrorBarMode.Horizontal:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange =  DoubleRange.Empty;
                    break;
                case ErrorBarMode.Vertical:
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    XRange = DoubleRange.Empty;
                    break;
                default:
                    XRange = new DoubleRange(ChartMath.Min(hlpoint.X, hrpoint.X), ChartMath.Max(hlpoint.X, hrpoint.X));
                    YRange = new DoubleRange(vbpoint.Y, vtpoint.Y);
                    break;
            }
        }

        internal Point DateTimeIntervalCalculation(double errorvalue,DateTimeIntervalType type)
        {
            DateTime date = Convert.ToDouble(_horLeftpoint.X).FromOADate();
            _horLeftpoint.X = DateTimeAxisHelper.IncreaseInterval(date, -errorvalue, type).ToOADate();
            DateTime date1 = Convert.ToDouble(_horRightpoint.X).FromOADate();
            _horRightpoint.X = DateTimeAxisHelper.IncreaseInterval(date1, errorvalue, type).ToOADate();
            return new Point(_horLeftpoint.X, _horRightpoint.X);
        }

        internal void UpdateVisualBinding()
        {
            SetVisualBindings(HorLine, _parentSeries.HorizontalLineStyle);
            SetVisualBindings(HorLeftCapLine, _parentSeries.HorizontalCapLineStyle);
            SetVisualBindings(HorRightCapLine, _parentSeries.HorizontalCapLineStyle);
            SetVisualBindings(VerLine, _parentSeries.VerticalLineStyle);
            SetVisualBindings(VerBottomCapLine, _parentSeries.VerticalCapLineStyle);
            SetVisualBindings(VerTopCapLine, _parentSeries.VerticalCapLineStyle);
        }
        public override UIElement CreateVisual(Size size)
        {
            _canvas = new Canvas();

            HorLine = new Line();
           _canvas.Children.Add(HorLine);
            HorLine.Tag = this;

            HorLeftCapLine = new Line();
           _canvas.Children.Add(HorLeftCapLine);

            HorRightCapLine = new Line();
            _canvas.Children.Add(HorRightCapLine);

            HorRightCapLine.Tag = HorLeftCapLine.Tag = this;

            VerLine = new Line();
            _canvas.Children.Add(VerLine);
            VerLine.Tag = this;

            VerBottomCapLine = new Line();
            _canvas.Children.Add(VerBottomCapLine);

            VerTopCapLine = new Line();
            _canvas.Children.Add(VerTopCapLine);

            VerTopCapLine.Tag = VerBottomCapLine.Tag = this;

            UpdateVisualBinding();
            
            return _canvas;
        }

        
        void SetVisualBindings(Shape element, DependencyObject linestyle)
        {
            Binding binding; 
            var check = element != HorLine && element != VerLine;
            if (check)
            {
                binding = new Binding { Source = linestyle, Path = new PropertyPath("Visibility") };
                element.SetBinding(Shape.VisibilityProperty, binding);
            }
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("Stroke") };
            element.SetBinding(Shape.StrokeProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeThickness") };
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeDashCap") };
            element.SetBinding(Shape.StrokeDashCapProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeEndLineCap") };
            element.SetBinding(Shape.StrokeEndLineCapProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeLineJoin") };
            element.SetBinding(Shape.StrokeLineJoinProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeMiterLimit") };
            element.SetBinding(Shape.StrokeMiterLimitProperty, binding);
            
            binding = new Binding { Source = linestyle, Path = new PropertyPath("StrokeDashOffset") };
            element.SetBinding(Shape.StrokeDashOffsetProperty, binding);

            var lineStyle = linestyle as LineStyle;
            var collection = lineStyle.StrokeDashArray;
            if (collection != null && collection.Count > 0)
            {
              var doubleCollection = new DoubleCollection();
              foreach (double value in collection)
              {
                  doubleCollection.Add(value);
              }
              element.StrokeDashArray = doubleCollection;
            }
        }

        public override UIElement GetRenderedVisual()
        {
            return _canvas;
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
                _canvas.Children.Clear();
                if (HorLine != null && _parentSeries.Mode != ErrorBarMode.Vertical)
                {
                    var hLPoint = transformer.TransformToVisible(_horLeftpoint.X, _horLeftpoint.Y);
                    var hRPoint = transformer.TransformToVisible(_horRightpoint.X, _horRightpoint.Y);

                    HorLine.X1 = hLPoint.X;
                    HorLine.Y1 = hLPoint.Y;
                    HorLine.X2 = hRPoint.X;
                    HorLine.Y2 = hRPoint.Y;
                    _canvas.Children.Add(HorLine);

                    if (_parentSeries.HorizontalCapLineStyle.Visibility == Visibility.Visible)
                    {
                        var horWidth = _parentSeries.HorizontalCapLineStyle.LineWidth / 2;

                        HorLeftCapLine.X1 = HorLine.X1;
                        HorLeftCapLine.Y1 = HorLine.Y1 + horWidth;
                        HorLeftCapLine.X2 = HorLine.X1;
                        HorLeftCapLine.Y2 = HorLine.Y1 - horWidth;
                        _canvas.Children.Add(HorLeftCapLine);

                        HorRightCapLine.X1 = HorLine.X2;
                        HorRightCapLine.Y1 = HorLine.Y2 + horWidth;
                        HorRightCapLine.X2 = HorLine.X2;
                        HorRightCapLine.Y2 = HorLine.Y2 - horWidth;
                        _canvas.Children.Add(HorRightCapLine);
                    }
                }

                if (VerLine != null && _parentSeries.Mode != ErrorBarMode.Horizontal)
                {
                    var vTPoint = transformer.TransformToVisible(_verToppoint.X, _verToppoint.Y);
                    var vBPoint = transformer.TransformToVisible(_verBottompoint.X, _verBottompoint.Y);

                    this.VerLine.X1 = vTPoint.X;
                    this.VerLine.Y1 = vTPoint.Y;
                    this.VerLine.X2 = vBPoint.X;
                    this.VerLine.Y2 = vBPoint.Y;

                    _canvas.Children.Add(VerLine);
                    if (_parentSeries.VerticalCapLineStyle.Visibility == Visibility.Visible)
                    {
                        var halfwidth = _parentSeries.VerticalCapLineStyle.LineWidth / 2;
                        VerBottomCapLine.X1 = VerLine.X1 - halfwidth;
                        VerBottomCapLine.Y1 = VerLine.Y1;
                        VerBottomCapLine.X2 = VerLine.X1 + halfwidth;
                        VerBottomCapLine.Y2 = VerLine.Y1;
                        _canvas.Children.Add(VerBottomCapLine);

                        VerTopCapLine.X1 = VerLine.X1 - halfwidth;
                        VerTopCapLine.Y1 = VerLine.Y2;
                        VerTopCapLine.X2 = VerLine.X1 + halfwidth;
                        VerTopCapLine.Y2 = VerLine.Y2;
                        _canvas.Children.Add(VerTopCapLine);
                    }
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

    }
}
