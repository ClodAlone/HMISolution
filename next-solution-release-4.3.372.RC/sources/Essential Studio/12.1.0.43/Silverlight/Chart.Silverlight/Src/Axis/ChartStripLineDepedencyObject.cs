#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Chart
{
    #region strip line
    /// <summary>
    /// Class implementation for ChartStripLine
    /// </summary>
    public class ChartStripLine : DependencyObject
    {
        /// <summary>
        ///  Identifies the ContentOffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOffsetXProperty = DependencyProperty.Register("ContentOffsetX", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        ///  Identifies the horizontalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartStripLine), new PropertyMetadata(HorizontalAlignment.Center, new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        /// Get or Set HorizontalAlignmentProperty
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            set { SetValue(HorizontalAlignmentProperty, value); }
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
        }
        /// <summary>
        ///  Identifies the VerticalAlignment dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty = DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(ChartStripLine), new PropertyMetadata(VerticalAlignment.Center, new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        /// Get or Set VerticalAlignment property
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            set { SetValue(VerticalAlignmentProperty, value); }
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
        }
        /// <summary>
        ///  Identifies the isPixelWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty isPixelWidthProperty = DependencyProperty.Register("isPixelWidth", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the PixelWidth for StripLine.
        /// </summary>
        /// <value>The isPixelWidth.</value>
        public bool isPixelWidth
        {
            set { SetValue(isPixelWidthProperty, value); }
            get { return (bool)GetValue(isPixelWidthProperty); }
        }

        /// <summary>
        /// Gets or sets the OffsetX for Text.
        /// </summary>
        /// <value>The OffsetX.</value>
        public double ContentOffsetX
        {
            set { SetValue(ContentOffsetXProperty, value); }
            get { return (double)GetValue(ContentOffsetXProperty); }
        }
        /// <summary>
        ///  Identifies the ContentOffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOffsetYProperty = DependencyProperty.Register("ContentOffsetY", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        /// Gets or sets the OffsetX for Text.
        /// </summary>
        /// <value>The OffsetX.</value>
        public double ContentOffsetY
        {
            set { SetValue(ContentOffsetYProperty, value); }
            get { return (double)GetValue(ContentOffsetYProperty); }
        }

        /// <summary>
        ///  Identifies the ContentHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentHeightProperty = DependencyProperty.Register("ContentHeight", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the ContentHeight.
        /// </summary>
        /// <value>The Height.</value>
        public double ContentHeight
        {
            set { SetValue(ContentHeightProperty, value); }
            get { return (double)GetValue(ContentHeightProperty); }
        }
        /// <summary>
        ///  Identifies the ContentWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentWidthProperty = DependencyProperty.Register("ContentWidth", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the ContentWidth.
        /// </summary>
        /// <value>The width.</value>
        public double ContentWidth
        {
            set { SetValue(ContentWidthProperty, value); }
            get { return (double)GetValue(ContentWidthProperty); }
        }

        /// <summary>
        ///  Identifies the ContentVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentVisibilityProperty = DependencyProperty.Register("ContentVisibility", typeof(Visibility), typeof(ChartStripLine), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        /// Gets or sets the ContentVisibility.
        /// </summary>
        /// <value>The Visibility.</value>
        public Visibility ContentVisibility
        {
            set { SetValue(ContentVisibilityProperty, value); }
            get { return (Visibility)GetValue(ContentVisibilityProperty); }
        }
        /// <summary>
        ///  Identifies the ContentOrientation dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentOrientationProperty = DependencyProperty.Register("ContentOrientation", typeof(Orientation), typeof(ChartStripLine), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Get or Set ContentOrientation property
        /// </summary>
        public Orientation ContentOrientation
        {
            set { SetValue(ContentOrientationProperty, value); }
            get { return (Orientation)GetValue(ContentOrientationProperty); }
        }
        /// <summary>
        ///  Identifies the StripLineContent dependency property.
        /// </summary>
        public static readonly DependencyProperty StriplineContentProperty = DependencyProperty.Register("StriplineContent", typeof(object), typeof(ChartStripLine), new PropertyMetadata("", new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the StriplineContent.
        /// </summary>
        /// <value>The Object value.</value>
        public object StriplineContent
        {

            set { SetValue(StriplineContentProperty, value); }
            get { return (object)GetValue(StriplineContentProperty); }
        }

        /// <summary>
        ///  Identifies the Interior dependency property.
        /// </summary>
        public static readonly DependencyProperty InteriorProperty = DependencyProperty.Register("Interior", typeof(Brush), typeof(ChartStripLine), new PropertyMetadata(new SolidColorBrush(), new PropertyChangedCallback(OnPositionPropertyChanged)));
        /// <summary>
        /// Gets or sets the interior.
        /// </summary>
        /// <value>The interior.</value>
        public Brush Interior
        {
            set { SetValue(InteriorProperty, value); }
            get { return (Brush)GetValue(InteriorProperty); }
        }

        /// <summary>
        ///  Identifies the Start dependency property.
        /// </summary>
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public double Start
        {
            set { SetValue(StartProperty, value); }
            get { return (double)GetValue(StartProperty); }
        }
        /// <summary>
        ///  Identifies the RepeatUntilProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatUntilProperty = DependencyProperty.Register("RepeatUntil", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the repeat until.
        /// </summary>
        /// <value>The repeat until.</value>
        public double RepeatUntil
        {
            set { SetValue(RepeatUntilProperty, value); }
            get { return (double)GetValue(RepeatUntilProperty); }
        }
        /// <summary>
        ///  Identifies the Width dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            set { SetValue(WidthProperty, value); }
            get { return (double)GetValue(WidthProperty); }
        }
        /// <summary>
        ///  Identifies the Height dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register("Height", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Get or Set Height property
        /// </summary>
        public double Height
        {
            set { SetValue(HeightProperty, value); }
            get { return (double)GetValue(HeightProperty); }
        }
        /// <summary>
        ///  Identifies the RepeatEvery dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatEveryProperty = DependencyProperty.Register("RepeatEvery", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));


        /// <summary>
        /// Gets or sets the repeat every.
        /// </summary>
        /// <value>The repeat every.</value>
        public double RepeatEvery
        {
            set { SetValue(RepeatEveryProperty, value); }
            get { return (double)GetValue(RepeatEveryProperty); }
        }
        /// <summary>
        ///  Identifies the Offset dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>A double value in axis range metrics.</value>
        public double Offset
        {
            set { SetValue(OffsetProperty, value); }
            get { return (double)GetValue(OffsetProperty); }
        }
        /// <summary>
        ///  Identifies the StartFromAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty StartFromAxisProperty = DependencyProperty.Register("StartFromAxis", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(true, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether [start from axis].
        /// </summary>
        /// <value>True if the stripline should start from the beginning of the axis. False, otherwise.</value>
        public bool StartFromAxis
        {
            set { SetValue(StartFromAxisProperty, value); }
            get { return (bool)GetValue(StartFromAxisProperty); }
        }
        /// <summary>
        ///  Identifies the Axis dependency property.
        /// </summary>
        internal static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(ChartAxis), typeof(ChartStripLine), new PropertyMetadata(null));
       /// <summary>
       /// get or Set Axis Property
       /// </summary>
        internal ChartAxis Axis
        {
            set { SetValue(AxisProperty, value); }
            get { return (ChartAxis)GetValue(AxisProperty); }
        }
        /// <summary>
        ///  Identifies the IsSegmented dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSegmentedProperty = DependencyProperty.Register("IsSegmented", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, new PropertyChangedCallback(OnAppearencePropertyChanged)));
       /// <summary>
       /// Get or Set IsSegmented property
       /// </summary>
        public bool IsSegmented
        {
            set { SetValue(IsSegmentedProperty, value); }
            get { return (bool)GetValue(IsSegmentedProperty); }
        }
        /// <summary>
        ///  Identifies the SegemnetStartValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentStartValueProperty = DependencyProperty.Register("SegmentStartValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));
       /// <summary>
       /// Get or Set SegementStartValue property
       /// </summary>
        public double SegmentStartValue
        {
            set { SetValue(SegmentStartValueProperty, value); }
            get { return (double)GetValue(SegmentStartValueProperty); }
        }
        /// <summary>
        ///  Identifies the SegmentEndValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentEndValueProperty = DependencyProperty.Register("SegmentEndValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, new PropertyChangedCallback(OnAppearencePropertyChanged)));
        /// <summary>
        /// Get or Set SegmentEndValue property
        /// </summary>
        public double SegmentEndValue
        {
            set { SetValue(SegmentEndValueProperty, value); }
            get { return (double)GetValue(SegmentEndValueProperty); }
        }

        private static void OnAppearencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartStripLine stripline = (ChartStripLine)d;
            if (stripline.Axis != null && stripline.Axis.Area != null && stripline.Axis.Area.stripLinePanel != null)
                stripline.Axis.Area.stripLinePanel.InvalidateMeasure();
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ChartStripLine stripline = (ChartStripLine)d;
            if (stripline.Axis != null && stripline.Axis.Area != null && stripline.Axis.Area.stripLinePanel != null)
                stripline.Axis.Area.stripLinePanel.InvalidateArrange();
        }

        #region INotifyPropertyChanged Members
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void OnPropertyChanged(string name)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(name));
        //    }
        //}
        #endregion
    }
    #endregion
}
