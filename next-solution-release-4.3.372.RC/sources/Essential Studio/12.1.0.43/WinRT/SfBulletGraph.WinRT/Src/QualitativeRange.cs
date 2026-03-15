#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


#endif

namespace Syncfusion.UI.Xaml.BulletGraph
{
    #region QualitativeRange

    public class QualitativeRange : DependencyObject
    {
        #region Public Dependency Properties

        #region RangeEnd
        public double RangeEnd
        {
            get { return (double)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register("RangeEnd", typeof(double), typeof(QualitativeRange), new PropertyMetadata(0d, OnRangeChanged));

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is QualitativeRange)
            {
                var range = (d as QualitativeRange);
                if (range.ParentBulletGraph != null)
                {
                    range.ParentBulletGraph.SetRanges();
                    if (range.ParentBulletGraph.BindRangeStrokeToTicks || range.ParentBulletGraph.BindRangeStrokeToLabels)
                        range.ParentBulletGraph.SetTicksAndLabels();
                }
            }
        }
        #endregion

        #region RangeStroke
        public Brush RangeStroke
        {
            get { return (Brush)GetValue(RangeStrokeProperty); }
            set { SetValue(RangeStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeStrokeProperty =
            DependencyProperty.Register("RangeStroke", typeof(Brush), typeof(QualitativeRange), new PropertyMetadata(new SolidColorBrush(Colors.Orange), OnRangeChanged));
        #endregion

        #region RangeOpacity
        public double RangeOpacity
        {
            get { return (double)GetValue(RangeOpacityProperty); }
            set { SetValue(RangeOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeOpacityProperty =
            DependencyProperty.Register("RangeOpacity", typeof(double), typeof(QualitativeRange), new PropertyMetadata(1d));
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region RangeWidth
        internal double RangeWidth
        {
            get { return (double)GetValue(RangeWidthProperty); }
            set { SetValue(RangeWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangeWidthProperty =
            DependencyProperty.Register("RangeWidth", typeof(double), typeof(QualitativeRange), new PropertyMetadata(double.NaN));
        #endregion

        #region ParentBulletGraph
        internal SfBulletGraph ParentBulletGraph
        {
            get { return (SfBulletGraph)GetValue(ParentBulletGraphProperty); }
            set { SetValue(ParentBulletGraphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentBulletGraph.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentBulletGraphProperty =
            DependencyProperty.Register("ParentBulletGraph", typeof(SfBulletGraph), typeof(QualitativeRange), new PropertyMetadata(null));
        #endregion

        #endregion
    }

    #endregion

    #region QualitativeRangeCollection

    public class QualitativeRangeCollection : ObservableCollection<QualitativeRange>
    {
    }

    #endregion
}
