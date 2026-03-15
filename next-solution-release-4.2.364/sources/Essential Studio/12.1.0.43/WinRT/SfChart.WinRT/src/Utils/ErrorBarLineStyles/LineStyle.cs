#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif
namespace Syncfusion.UI.Xaml.Charts
{
    public class LineStyle : DependencyObject
    {
        internal ChartSeriesBase Series;

        public LineStyle()
        {
            
        }
        
        public LineStyle(ChartSeriesBase series)
        {
            Series = series;
        }
        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(LineStyle), new PropertyMetadata(new SolidColorBrush(Colors.Cyan)));
        
        public Brush Stroke
        {
            get { return (Brush) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(LineStyle), new PropertyMetadata(2d));

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeDashCap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(LineStyle), new PropertyMetadata(PenLineCap.Flat));

        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeEndLineCap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(LineStyle), new PropertyMetadata(PenLineCap.Flat));

        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeLineJoin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(LineStyle), new PropertyMetadata(PenLineJoin.Bevel));

        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }
       
        //The limit on the ratio of the miter length to the StrokeThickness of a Shape element. This value is always a positive number that is greater than or equal to 1.
        // Using a DependencyProperty as the backing store for StrokeMiterLimit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(LineStyle), new PropertyMetadata(1d));

        public double StrokeMiterLimit
        {
            get { return (double)GetValue(StrokeMiterLimitProperty); }
            set { SetValue(StrokeMiterLimitProperty, value); }
        }

        //A Double that represents the distance within the dash pattern where a dash begins.
        // Using a DependencyProperty as the backing store for StrokeDashOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(LineStyle), new PropertyMetadata(null));

        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(LineStyle), new PropertyMetadata(null, OnPropertyChange));

        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }
        private static void OnPropertyChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var chartSeries = (obj as LineStyle).Series;
            if (e.NewValue != null)
            {
                foreach (var segment in chartSeries.Segments)
                {
                    var collection = (DoubleCollection)e.NewValue;
                    if (collection != null && collection.Count > 0)
                    {
                        var doubleCollection = new DoubleCollection();
                        var doubleCollection1 = new DoubleCollection();
                        foreach (var value in collection)
                        {
                            doubleCollection.Add(value);
                            doubleCollection1.Add(value);
                        }
                        if ((obj as LineStyle) == (chartSeries as ErrorBarSeries).HorizontalLineStyle)
                            (segment as ErrorBarSegment).HorLine.StrokeDashArray = doubleCollection;
                        if ((obj as LineStyle) == (chartSeries as ErrorBarSeries).HorizontalCapLineStyle)
                        {
                            (segment as ErrorBarSegment).HorRightCapLine.StrokeDashArray = doubleCollection;
                            (segment as ErrorBarSegment).HorLeftCapLine.StrokeDashArray = doubleCollection1;
                        }
                        if ((obj as LineStyle) == (chartSeries as ErrorBarSeries).VerticalLineStyle)
                            (segment as ErrorBarSegment).VerLine.StrokeDashArray = doubleCollection;
                        if ((obj as LineStyle) == (chartSeries as ErrorBarSeries).VerticalCapLineStyle)
                        {
                            (segment as ErrorBarSegment).VerBottomCapLine.StrokeDashArray = doubleCollection;
                            (segment as ErrorBarSegment).VerTopCapLine.StrokeDashArray = doubleCollection1;
                        }
                    }
                }
            }
        }
    }
}
