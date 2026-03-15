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
using System.ComponentModel;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{

    /// <summary>
    /// Chart enables the user to highlight a specific region of <see cref="ChartAxis"/> by adding strip lines to it.
    /// </summary>
    /// <remarks>
    /// The strip lines length and width can be customized,a text label can be specified and also the look and feel can be customized too.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class ChartStripLine : FrameworkElement, INotifyPropertyChanged
    {
      

        /// <summary>
        /// Gets or Sets start value.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or Sets background of this strip line.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets border brush.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets border thickness.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

       
       /// <summary>
        /// Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(ChartStripLine), new PropertyMetadata(new Thickness(0)));

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for StartX.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN,new PropertyChangedCallback(OnStartPropertChanged)));
        /// <summary>
        /// Called when StartX property changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnStartPropertChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
 	        (d as ChartStripLine).OnPropertyChanged("Start");
        }

        /// <summary>
        /// Gets or Sets SegmentStartValue.
        /// </summary>
        public double SegmentStartValue
        {
            get { return (double)GetValue(SegmentStartValueProperty); }
            set { SetValue(SegmentStartValueProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for StartY.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentStartValueProperty =
            DependencyProperty.Register("SegmentStartValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN,OnSegmentStartValueChanged));

        private static void OnSegmentStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentStartValue"); 
        }

        /// <summary>
        /// Gets or Sets SegmentEndValue
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double SegmentEndValue
        {
            get { return (double)GetValue(SegmentEndValueProperty); }
            set { SetValue(SegmentEndValueProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for EndX.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentEndValueProperty =
            DependencyProperty.Register("SegmentEndValue", typeof(double), typeof(ChartStripLine), new PropertyMetadata(double.NaN,OnSegmentEndValueChanged));

        private static void OnSegmentEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentEndValue"); 
        }

        /// <summary>
        ///  Gets or Sets SegmentAxisName
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string SegmentAxisName
        {
            get { return (string)GetValue(SegmentAxisNameProperty); }
            set { SetValue(SegmentAxisNameProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for AxisName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SegmentAxisNameProperty =
            DependencyProperty.Register("SegmentAxisName", typeof(string), typeof(ChartStripLine), new PropertyMetadata(string.Empty,OnSegmentAxisNameChanged));

        private static void OnSegmentAxisNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("SegmentAxisName");
        }

        /// <summary>
        ///  Gets or Sets IsSegmented
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsSegmented
        {
            get { return (bool)GetValue(IsSegmentedProperty); }
            set { SetValue(IsSegmentedProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSegmented.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSegmentedProperty =
            DependencyProperty.Register("IsSegmented", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, OnIsSegmentedPropertyChanged));

        private static void OnIsSegmentedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("IsSegmented");
        }

        /// <summary>
        /// Gets or Sets value of RepeatEvery
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RepeatEvery
        {
            get { return (double)GetValue(RepeatEveryProperty); }
            set { SetValue(RepeatEveryProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for RepeatEvery.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RepeatEveryProperty =
            DependencyProperty.Register("RepeatEvery", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, OnRepeatEveryPropertyChanged));

        private static void OnRepeatEveryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("RepeatEvery");
        }

        /// <summary>
        /// Gets or Sets a value to RepeatUntil
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double RepeatUntil
        {
            get { return (double)GetValue(RepeatUntilProperty); }
            set { SetValue(RepeatUntilProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for RepeatUntil.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RepeatUntilProperty =
            DependencyProperty.Register("RepeatUntil", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, OnRepeatUntilPropertyChanged));

        private static void OnRepeatUntilPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("RepeatUntil");
        }

        /// <summary>
        /// Gets or Sets label.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public object Label
        {
            get { return (object)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(object), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets label DataTemplate.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(ChartStripLine), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets width of the strip line.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public new double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        /// </summary>
        public new static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d,new PropertyChangedCallback(OnWidthPropertyChanged)));

        private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
 	        (d as ChartStripLine).OnPropertyChanged("Width");
        }

        /// <summary>
        /// Gets or Sets label angle.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double LabelAngle
        {
            get { return (double)GetValue(LabelAngleProperty); }
            set { SetValue(LabelAngleProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelAngle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelAngleProperty =
            DependencyProperty.Register("LabelAngle", typeof(double), typeof(ChartStripLine), new PropertyMetadata(0d, OnLabelAnglePropertyChanged));

        private static void OnLabelAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("LabelAngle");
        }
        /// <summary>
        /// Gets or Sets a value that indicates whether the value specified in Width property should be measured in pixels.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public bool IsPixelWidth
        {
            get { return (bool)GetValue(IsPixelWidthProperty); }
            set { SetValue(IsPixelWidthProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPixelWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPixelWidthProperty =
            DependencyProperty.Register("IsPixelWidth", typeof(bool), typeof(ChartStripLine), new PropertyMetadata(false, OnIsPixelWidthPropertyChanged));

        private static void OnIsPixelWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartStripLine).OnPropertyChanged("IsPixelWidth");
        }
        
        /// <summary>
        /// Gets or Sets horizontal alignment of label
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public HorizontalAlignment LabelHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelHorizontalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register("LabelHorizontalAlignment", typeof(HorizontalAlignment), typeof(ChartStripLine), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or Sets vertical alignment of label.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public VerticalAlignment LabelVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelVerticalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register("LabelVerticalAlignment", typeof(VerticalAlignment), typeof(ChartStripLine), new PropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
