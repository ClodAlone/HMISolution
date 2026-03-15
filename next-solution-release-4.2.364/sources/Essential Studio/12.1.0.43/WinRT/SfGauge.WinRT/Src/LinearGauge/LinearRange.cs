#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    #region LinearRange

    public class LinearRange : DependencyObject
    {
        #region Public Dependency Properties

        #region StartValue
        public double StartValue
        {
            get { return (double)GetValue(StartValueProperty); }
            set { SetValue(StartValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartValueProperty =
            DependencyProperty.Register("StartValue", typeof(double), typeof(LinearRange), new PropertyMetadata(double.NaN, OnRangeChanged));

        #endregion

        #region EndValue
        public double EndValue
        {
            get { return (double)GetValue(EndValueProperty); }
            set { SetValue(EndValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndValueProperty =
            DependencyProperty.Register("EndValue", typeof(double), typeof(LinearRange), new PropertyMetadata(double.NaN, OnRangeChanged));
        #endregion

        #region StartWidth
        public double StartWidth
        {
            get { return (double)GetValue(StartWidthProperty); }
            set { SetValue(StartWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartWidthProperty =
            DependencyProperty.Register("StartWidth", typeof(double), typeof(LinearRange), new PropertyMetadata(double.NaN, OnRangeChanged));
        #endregion

        #region EndWidth
        public double EndWidth
        {
            get { return (double)GetValue(EndWidthProperty); }
            set { SetValue(EndWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndWidthProperty =
            DependencyProperty.Register("EndWidth", typeof(double), typeof(LinearRange), new PropertyMetadata(double.NaN, OnRangeChanged));
        #endregion

        #region RangeStroke
        public Brush RangeStroke
        {
            get { return (Brush)GetValue(RangeStrokeProperty); }
            set { SetValue(RangeStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeStrokeProperty =
            DependencyProperty.Register("RangeStroke", typeof(Brush), typeof(LinearRange), new PropertyMetadata(new SolidColorBrush(Colors.Magenta), OnRangeStrokeChanged));

        private static void OnRangeStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearRange)
            {
                var linearRange = (d as LinearRange);
                if (linearRange.ParentScale != null)
                {
                    if (linearRange.ParentScale.BindRangeStrokeToLabels || linearRange.ParentScale.BindRangeStrokeToTicks)
                        linearRange.ParentScale.SetTicksAndLabels();
                }
            }
        }
        #endregion

        #region RangeOpacity
        public double RangeOpacity
        {
            get { return (double)GetValue(RangeOpacityProperty); }
            set { SetValue(RangeOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeOpacityProperty =
            DependencyProperty.Register("RangeOpacity", typeof(double), typeof(LinearRange), new PropertyMetadata(1d));
        #endregion

        #region RangeOffset
        public double RangeOffset
        {
            get { return (double)GetValue(RangeOffsetProperty); }
            set { SetValue(RangeOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeOffsetProperty =
            DependencyProperty.Register("RangeOffset", typeof(double), typeof(LinearRange), new PropertyMetadata(0d, OnRangeChanged));

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearRange)
            {
                var linearRange = (d as LinearRange);
                if (linearRange.ParentScale != null)
                {
                    linearRange.ParentScale.SetTicksAndLabels();
                }
                linearRange.SetRangePathGeometry();
            }
        }
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region RangePathGeometry
        internal PathGeometry RangePathGeometry
        {
            get { return (PathGeometry)GetValue(RangePathGeometryProperty); }
            set { SetValue(RangePathGeometryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePathGeometry.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangePathGeometryProperty =
            DependencyProperty.Register("RangePathGeometry", typeof(PathGeometry), typeof(LinearRange), new PropertyMetadata(null));
        #endregion

        #region RangeScaleY
        internal double RangeScaleY
        {
            get { return (double)GetValue(RangeScaleYProperty); }
            set { SetValue(RangeScaleYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeScaleY.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangeScaleYProperty =
            DependencyProperty.Register("RangeScaleY", typeof(double), typeof(LinearRange), new PropertyMetadata(1d));
        #endregion

        #region RangeMargin
        internal Thickness RangeMargin
        {
            get { return (Thickness)GetValue(RangeMarginProperty); }
            set { SetValue(RangeMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangeMarginProperty =
            DependencyProperty.Register("RangeMargin", typeof(Thickness), typeof(LinearRange), new PropertyMetadata(new Thickness()));
        #endregion

        #endregion

        #region CLR Properties

        public LinearScale ParentScale { get; internal set; }

        #endregion

        #region Private Members

        #endregion

        #region Implementation

        internal void SetRangePathGeometry()
        {
            if (ParentScale != null)
            {
                double startWidth = StartWidth;
                double endWidth = EndWidth;

                if (Double.IsNaN(startWidth))
                    startWidth = ParentScale.ScaleBarHeight / 2;
                if (double.IsNaN(endWidth))
                    endWidth = ParentScale.ScaleBarHeight / 2;

                double start = StartValue;
                double end = EndValue;
                if (StartValue < ParentScale.Minimum)
                    start = ParentScale.Minimum;
                if (EndValue < ParentScale.Minimum)
                    end = ParentScale.Minimum;
                if (start > ParentScale.Maximum)
                    start = ParentScale.Maximum;
                if (end > ParentScale.Maximum)
                    end = ParentScale.Maximum;

                double startPt = (start - ParentScale.Minimum) * ParentScale.ScaleBarWidth / (ParentScale.Maximum - ParentScale.Minimum);
                double endPt = (end - ParentScale.Minimum) * ParentScale.ScaleBarWidth / (ParentScale.Maximum - ParentScale.Minimum);

                var rangePath = new PathGeometry();

                var line1 = new LineSegment {Point = new Point(endPt, 0)};
                var line2 = new LineSegment {Point = new Point(endPt, endWidth)};
                var line3 = new LineSegment {Point = new Point(startPt, startWidth)};

                var figure = new PathFigure {StartPoint = new Point(startPt, 0)};
                figure.Segments.Add(line1);
                figure.Segments.Add(line2);
                figure.Segments.Add(line3);
                figure.IsClosed = true;

                rangePath.Figures.Add(figure);
                RangePathGeometry = rangePath;

                RangeMargin = ParentScale.RangePosition == LinearRangesPosition.Above ? new Thickness(0, -RangeOffset, 0, 0) : new Thickness(0, RangeOffset, 0, 0);
                RangeScaleY = ParentScale.RangePosition == LinearRangesPosition.Above ? -1 : 1;
            }
        }

        #endregion
    }

    #endregion

    #region LinearRangeCollection

    public class LinearRangeCollection : ObservableCollection<LinearRange>
    {
        
    }

    #endregion
}
