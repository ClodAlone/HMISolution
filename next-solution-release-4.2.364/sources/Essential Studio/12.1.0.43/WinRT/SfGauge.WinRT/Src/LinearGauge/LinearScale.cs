#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Specialized;
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
    #region LinearScale

    [TemplatePart(Name = "PART_LinearScaleTicks", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_LinearScaleLabels", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_LinearScalePointers", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_LinearScaleRanges", Type = typeof(ItemsControl))]

    public class LinearScale : Control, IDisposable
    {
        #region Constructor

        public LinearScale()
        {
            DefaultStyleKey = typeof(LinearScale);

            Ticks = new LinearScaleTickCollection();
            Labels = new LinearScaleLabelCollection();
            Pointers = new LinearPointerCollection();
            Ranges = new LinearRangeCollection();

            Pointers.CollectionChanged += Pointers_CollectionChanged;
            Ranges.CollectionChanged += Ranges_CollectionChanged;
        }

        #endregion

        #region Public Dependency Properties

        #region ScaleDirection
        public LinearScaleDirection ScaleDirection
        {
            get { return (LinearScaleDirection)GetValue(ScaleDirectionProperty); }
            set { SetValue(ScaleDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleDirectionProperty =
            DependencyProperty.Register("ScaleDirection", typeof(LinearScaleDirection), typeof(LinearScale), new PropertyMetadata(LinearScaleDirection.Forward, OnScaleDirectionChanged));

        private static void OnScaleDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = d as LinearScale;
                if (linearScale.ParentGauge != null)
                    SfLinearGauge.SetOrientationAndScaleDirection(linearScale.ParentGauge);
            }
        }
        #endregion

        #region ScaleBarSize
        public double ScaleBarSize
        {
            get { return (double)GetValue(ScaleBarSizeProperty); }
            set { SetValue(ScaleBarSizeProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarSizeProperty =
           DependencyProperty.Register("ScaleBarSize", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnScaleBarSizeChanged));

        private static void OnScaleBarSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.ScaleBarHeight = (double)e.NewValue;
                linearScale.SetMargins();
                linearScale.ResetPointers();
                linearScale.ResetRanges();
            }
        }
        #endregion

        #region ScaleBarLength
        public double ScaleBarLength
        {
            get { return (double)GetValue(ScaleBarLengthProperty); }
            set { SetValue(ScaleBarLengthProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarLengthProperty =
           DependencyProperty.Register("ScaleBarLength", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnScaleBarLengthChanged));

        private static void OnScaleBarLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.ScaleBarWidth = (double)e.NewValue;
                linearScale.SetMargins();
                linearScale.ResetPointers();
                linearScale.ResetRanges();
            }
        }
        #endregion

        #region ScaleBarStroke
        public Brush ScaleBarStroke
        {
            get { return (Brush)GetValue(ScaleBarStrokeProperty); }
            set { SetValue(ScaleBarStrokeProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarStrokeProperty =
            DependencyProperty.Register("ScaleBarStroke", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        #endregion

        #region ScaleBarBorderBrush
        public static readonly DependencyProperty ScaleBarBorderBrushProperty =
            DependencyProperty.Register("ScaleBarBorderBrush", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public Brush ScaleBarBorderBrush
        {
            get { return (Brush)GetValue(ScaleBarBorderBrushProperty); }
            set { SetValue(ScaleBarBorderBrushProperty, value); }
        }
        #endregion

        #region ScaleBarBorderThickness
        public Thickness ScaleBarBorderThickness
        {
            get { return (Thickness)GetValue(ScaleBarBorderThicknessProperty); }
            set { SetValue(ScaleBarBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty ScaleBarBorderThicknessProperty =
            DependencyProperty.Register("ScaleBarBorderThickness", typeof(Thickness), typeof(LinearScale), new PropertyMetadata(new Thickness(0d)));

        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(LinearScale), new PropertyMetadata(0d, OnValueChanged));
        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(LinearScale), new PropertyMetadata(100d, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                if (!(double.IsNaN(linearScale.ScaleBarLength)))
                    linearScale.SetScaleBarSize();
                linearScale.SetTicksAndLabels();
                if (linearScale.Pointers != null && linearScale.Pointers.Count > 0)
                    linearScale.ResetPointers();
                if (linearScale.Ranges != null && linearScale.Ranges.Count > 0)
                    linearScale.ResetRanges();
            }
        }
        #endregion

        #region Interval
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnIntervalChanged));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                (d as LinearScale).SetTicksAndLabels();
            }
        }
        #endregion

        #region MinorTicksPerInterval
        public int MinorTicksPerInterval
        {
            get { return (int)GetValue(MinorTicksPerIntervalProperty); }
            set { SetValue(MinorTicksPerIntervalProperty, value); }
        }

        public static readonly DependencyProperty MinorTicksPerIntervalProperty =
            DependencyProperty.Register("MinorTicksPerInterval", typeof(int), typeof(LinearScale), new PropertyMetadata(0, OnTicksChanged));
        #endregion

        #region MajorTickSize
        public double MajorTickSize
        {
            get { return (double)GetValue(MajorTickSizeProperty); }
            set { SetValue(MajorTickSizeProperty, value); }
        }

        public static readonly DependencyProperty MajorTickSizeProperty =
            DependencyProperty.Register("MajorTickSize", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnTickSizeChanged));
        #endregion

        #region MinorTickSize
        public double MinorTickSize
        {
            get { return (double)GetValue(MinorTickSizeProperty); }
            set { SetValue(MinorTickSizeProperty, value); }
        }

        public static readonly DependencyProperty MinorTickSizeProperty =
            DependencyProperty.Register("MinorTickSize", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnTickSizeChanged));

        private static void OnTickSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.SetTicksAndLabels();
                linearScale.SetMargins();
            }
        }
        #endregion

        #region MajorTickStrokeThickness
        public double MajorTickStrokeThickness
        {
            get { return (double)GetValue(MajorTickStrokeThicknessProperty); }
            set { SetValue(MajorTickStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty MajorTickStrokeThicknessProperty =
            DependencyProperty.Register("MajorTickStrokeThickness", typeof(double), typeof(LinearScale), new PropertyMetadata(2d, OnTicksChanged));
        #endregion

        #region MinorTickStrokeThickness
        public double MinorTickStrokeThickness
        {
            get { return (double)GetValue(MinorTickStrokeThicknessProperty); }
            set { SetValue(MinorTickStrokeThicknessProperty, value); }
        }

        public static readonly DependencyProperty MinorTickStrokeThicknessProperty =
            DependencyProperty.Register("MinorTickStrokeThickness", typeof(double), typeof(LinearScale), new PropertyMetadata(1d, OnTicksChanged));
        #endregion

        #region MajorTickStroke
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));
        #endregion

        #region MinorTickStroke
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                (d as LinearScale).SetTicksAndLabels();
            }
        }
        #endregion

        #region TickPosition
        public LinearTicksPosition TickPosition
        {
            get { return (LinearTicksPosition)GetValue(TickPositionProperty); }
            set { SetValue(TickPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickPositionProperty =
            DependencyProperty.Register("TickPosition", typeof(LinearTicksPosition), typeof(LinearScale), new PropertyMetadata(LinearTicksPosition.Below, OnTickPositionChanged));

        private static void OnTickPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.SetTicksAndLabels();
                linearScale.SetMargins();
            }
        }
        #endregion

        #region RangePosition
        public LinearRangesPosition RangePosition
        {
            get { return (LinearRangesPosition)GetValue(RangePositionProperty); }
            set { SetValue(RangePositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangePositionProperty =
            DependencyProperty.Register("RangePosition", typeof(LinearRangesPosition), typeof(LinearScale), new PropertyMetadata(LinearRangesPosition.Above, OnRangePositionChanged));

        private static void OnRangePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.ResetRanges();
                linearScale.SetMargins();
            }
        }
        #endregion

        #region LabelPosition
        public LinearLabelsPosition LabelPosition
        {
            get { return (LinearLabelsPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(LinearLabelsPosition), typeof(LinearScale), new PropertyMetadata(LinearLabelsPosition.Below, OnLabelPositionChanged));

        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.SetMargins();
            }
        }
        #endregion

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(LinearScale), new PropertyMetadata(new SolidColorBrush(Colors.White), OnLabelsChanged));

        private static void OnLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                (d as LinearScale).SetTicksAndLabels();
            }
        }
        #endregion

        #region LabelSize
        public double LabelSize
        {
            get { return (double)GetValue(LabelSizeProperty); }
            set { SetValue(LabelSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelSizeProperty =
            DependencyProperty.Register("LabelSize", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN, OnLabelSizeChanged));

        private static void OnLabelSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                linearScale.SetTicksAndLabels();
                linearScale.SetMargins();
            }
        }
        #endregion

        #region LabelOffset
        public double LabelOffset
        {
            get
            {
                return (double)GetValue(LabelOffsetProperty);
            }
            set { SetValue(LabelOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelOffsetProperty =
            DependencyProperty.Register("LabelOffset", typeof(double), typeof(LinearScale), new PropertyMetadata(0d, OnLabelOffsetChanged));

        private static void OnLabelOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                var linearScale = (d as LinearScale);
                if (linearScale.ParentGauge != null)
                    linearScale.SetMargins();
            }
        }
        #endregion

        #region Pointers
        public LinearPointerCollection Pointers
        {
            get { return (LinearPointerCollection)GetValue(PointersProperty); }
            set { SetValue(PointersProperty, value); }
        }

        public static readonly DependencyProperty PointersProperty =
            DependencyProperty.Register("Pointers", typeof(LinearPointerCollection), typeof(LinearScale), new PropertyMetadata(null));
        #endregion

        #region Ranges
        public LinearRangeCollection Ranges
        {
            get { return (LinearRangeCollection)GetValue(RangesProperty); }
            set { SetValue(RangesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ranges.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangesProperty =
            DependencyProperty.Register("Ranges", typeof(LinearRangeCollection), typeof(LinearScale), new PropertyMetadata(null));
        #endregion

        #region BindRangeStrokeToLabels
        public bool BindRangeStrokeToLabels
        {
            get { return (bool)GetValue(BindRangeStrokeToLabelsProperty); }
            set { SetValue(BindRangeStrokeToLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindRangeStrokeToLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindRangeStrokeToLabelsProperty =
            DependencyProperty.Register("BindRangeStrokeToLabels", typeof(bool), typeof(LinearScale), new PropertyMetadata(false, OnBindRangeStrokeToLabelsChanged));

        private static void OnBindRangeStrokeToLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                (d as LinearScale).SetTicksAndLabels();
            }
        }
        #endregion

        #region BindRangeStrokeToTicks
        public bool BindRangeStrokeToTicks
        {
            get { return (bool)GetValue(BindRangeStrokeToTicksProperty); }
            set { SetValue(BindRangeStrokeToTicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindRangeStrokeToTicks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindRangeStrokeToTicksProperty =
            DependencyProperty.Register("BindRangeStrokeToTicks", typeof(bool), typeof(LinearScale), new PropertyMetadata(false, OnBindRangeStrokeToTicksChanged));

        private static void OnBindRangeStrokeToTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LinearScale)
            {
                (d as LinearScale).SetTicksAndLabels();
            }
        }
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region ScaleBarHeight
        internal double ScaleBarHeight
        {
            get { return (double)GetValue(ScaleBarHeightProperty); }
            set { SetValue(ScaleBarHeightProperty, value); }
        }

        internal static readonly DependencyProperty ScaleBarHeightProperty =
           DependencyProperty.Register("ScaleBarHeight", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN));
        #endregion

        #region ScaleBarWidth
        internal static readonly DependencyProperty ScaleBarWidthProperty =
           DependencyProperty.Register("ScaleBarWidth", typeof(double), typeof(LinearScale), new PropertyMetadata(double.NaN));

        internal double ScaleBarWidth
        {
            get { return (double)GetValue(ScaleBarWidthProperty); }
            set { SetValue(ScaleBarWidthProperty, value); }
        }
        #endregion

        #region Ticks
        internal LinearScaleTickCollection Ticks
        {
            get { return (LinearScaleTickCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        internal static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(LinearScaleTickCollection), typeof(LinearScale), new PropertyMetadata(null));
        #endregion

        #region Labels
        internal LinearScaleLabelCollection Labels
        {
            get { return (LinearScaleLabelCollection)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }

        internal static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(LinearScaleLabelCollection), typeof(LinearScale), new PropertyMetadata(null));
        #endregion

        #region TicksMargin
        internal Thickness TicksMargin
        {
            get { return (Thickness)GetValue(TicksMarginProperty); }
            set { SetValue(TicksMarginProperty, value); }
        }

        internal static readonly DependencyProperty TicksMarginProperty =
           DependencyProperty.Register("TicksMargin", typeof(Thickness), typeof(LinearScale), new PropertyMetadata(new Thickness()));
        #endregion

        #region LabelsMargin
        internal Thickness LabelsMargin
        {
            get { return (Thickness)GetValue(LabelsMarginProperty); }
            set { SetValue(LabelsMarginProperty, value); }
        }

        internal static readonly DependencyProperty LabelsMarginProperty =
           DependencyProperty.Register("LabelsMargin", typeof(Thickness), typeof(LinearScale), new PropertyMetadata(new Thickness()));
        #endregion

        #region RangesMargin
        internal Thickness RangesMargin
        {
            get { return (Thickness)GetValue(RangesMarginProperty); }
            set { SetValue(RangesMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangesMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangesMarginProperty =
            DependencyProperty.Register("RangesMargin", typeof(Thickness), typeof(LinearScale), new PropertyMetadata(new Thickness()));
        #endregion

        #endregion

        #region CLR Properties

        #region ParentGauge

        public SfLinearGauge ParentGauge { get; internal set; }

        #endregion

        #endregion

        #region Private Members

        ItemsControl linearScaleTicksControl;
        internal double interval;

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()        
#else
        public override void OnApplyTemplate()
#endif
        {
            linearScaleTicksControl = GetTemplateChild("PART_LinearScaleTicks") as ItemsControl;
            base.OnApplyTemplate();
        }

        #endregion

        #region Implementation

        internal void ResetScale()
        {
            SetScaleBarSize();
            SetTicksAndLabels();
            //SetTicksAndLabels();
            SetMargins();
            ResetPointers();
            ResetRanges();
        }

        private void SetScaleBarSize()
        {
            if (ParentGauge != null)
            {
                if (Double.IsNaN(ScaleBarLength))
                {
                    if(ParentGauge.Orientation == Orientation.Horizontal)
                        ScaleBarWidth = 4 * ParentGauge.GaugeSize.Width / 5;
                    else
                    {
                        ScaleBarWidth = ParentGauge.GaugeSize.Width < ParentGauge.GaugeSize.Height ?
                            ParentGauge.GaugeSize.Width : 4 * ParentGauge.GaugeSize.Height / 5;
                    }
                }
                if (Double.IsNaN(ScaleBarSize))
                {
                    ScaleBarHeight = ScaleBarWidth / 10;
                }
            }
        }

        private void Pointers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetPointers();
        }

        private void Ranges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ResetRanges();
        }

        private void ResetPointers()
        {
            foreach (LinearPointer pointer in Pointers)
            {
                pointer.ParentScale = this;
                pointer.SetPointerPosition();
            }
        }

        private void ResetRanges()
        {
            foreach (LinearRange range in Ranges)
            {
                range.ParentScale = this;
                range.SetRangePathGeometry();
            }
        }

        private void SetMargins()
        {
            if (ParentGauge != null)
            {
                double majorTickSize = Double.IsNaN(MajorTickSize) ? ScaleBarHeight / 2 : MajorTickSize;
                double minorTickSize = Double.IsNaN(MinorTickSize) ? ScaleBarHeight / 4 : MinorTickSize;
                double labelSize = Double.IsNaN(LabelSize) ? ScaleBarHeight * 2 / 5 : LabelSize;

                #region RangePosition -> Below
                if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Below)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2), 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2 + Math.Max(majorTickSize, minorTickSize) + LabelOffset), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2), 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Cross)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2) + LabelOffset, 0, 0);
                    TicksMargin = new Thickness();
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Above)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2 + LabelOffset, 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
                }

                else if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Below)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2), 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Cross)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness();
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Below && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Above)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - Math.Max(majorTickSize, minorTickSize) - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
                }
                #endregion

                #region RangePosition -> Above
                if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Below)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2 + Math.Max(majorTickSize, minorTickSize) + LabelOffset), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2), 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Cross)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2) + LabelOffset, 0, 0);
                    TicksMargin = new Thickness();
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Below && TickPosition == LinearTicksPosition.Above)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, (ParentGauge.GaugeSize.Height + ScaleBarHeight) / 2 + LabelOffset, 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
                }

                else if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Below)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height / 2 + ScaleBarHeight / 2), 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Cross)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness();
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
                }
                else if (RangePosition == LinearRangesPosition.Above && LabelPosition == LinearLabelsPosition.Above && TickPosition == LinearTicksPosition.Above)
                {
                    RangesMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    LabelsMargin = new Thickness(0, ((ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2 - Math.Max(majorTickSize, minorTickSize) - LabelOffset - labelSize), 0, 0);
                    TicksMargin = new Thickness(0, (ParentGauge.GaugeSize.Height - ScaleBarHeight) / 2, 0, 0);
                    if (linearScaleTicksControl != null)
                        linearScaleTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
                }
                #endregion
            }
        }

        private void SetTransformForLabels(out TransformGroup transform, out Point transformOrigin)
        {
            transform = new TransformGroup();
            transformOrigin = new Point();
            if (ParentGauge != null && ParentGauge.Orientation == Orientation.Horizontal && ScaleDirection == LinearScaleDirection.Backward)
            {
                transform.Children.Add(new ScaleTransform { ScaleX = -1 });
            }
            if (ParentGauge != null && ParentGauge.Orientation == Orientation.Vertical && ScaleDirection == LinearScaleDirection.Forward)
            {
                transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                transform.Children.Add(new RotateTransform { Angle = -90 });
            }
            if (ParentGauge != null && ParentGauge.Orientation == Orientation.Vertical && ScaleDirection == LinearScaleDirection.Backward)
            {
                transform.Children.Add(new RotateTransform { Angle = 90 });
            }
        }

        internal void SetTicksAndLabels()
        {
            if (ParentGauge != null)
            {
                Ticks.Clear();
                Labels.Clear();
                interval = Interval;

                if (interval <= 0 || double.IsNaN(interval) || interval > (Math.Abs(Minimum - Maximum)))
                {
                    interval = (Math.Abs(Minimum - Maximum) / 10);
                }
                MinorTicksPerInterval = MinorTicksPerInterval < 0 ? 1 : MinorTicksPerInterval;
                int majorTicksCount = (int)(Math.Abs(Minimum - Maximum) / interval) + 1;
                var minorTicksCount = (int)((Math.Abs(Minimum - Maximum) % interval) / (interval / (MinorTicksPerInterval + 1)));
                int totalTicksCount = majorTicksCount + MinorTicksPerInterval * (majorTicksCount - 1) + minorTicksCount;

                double MinorInterval = interval / (MinorTicksPerInterval + 1);
                double count = Minimum;
                TransformGroup transform;
                Point transformOrigin;
                SetTransformForLabels(out transform, out transformOrigin);

                for (int i = 0; i < totalTicksCount; i++)
                {
                    LinearRange correspRange = null;
                    if (Ranges.Count > 0 && BindRangeStrokeToTicks || BindRangeStrokeToLabels)
                    {
                        foreach (LinearRange range in Ranges)
                        {
                            if (Math.Round(count, 5) <= range.EndValue && Math.Round(count, 5) >= range.StartValue)
                            {
                                correspRange = range;
                                break;
                            }
                        }
                    }

                    LinearScaleLabel label;
                    LinearScaleTick tick;
                    if ((i) % (MinorTicksPerInterval + 1) == 0)
                    {
                        tick = new LinearScaleTick
                        {
                            TickLength = Double.IsNaN(MajorTickSize) ? ScaleBarHeight / 2 : MajorTickSize,
                            TickStrokeThickness = MajorTickStrokeThickness,
                            TickStroke = BindRangeStrokeToTicks && correspRange != null ? correspRange.RangeStroke : MajorTickStroke,
                            TickVerticalAlignment = TickPosition == LinearTicksPosition.Cross ? VerticalAlignment.Center : VerticalAlignment.Stretch,
                            ParentScale = this
                        };
                        label = new LinearScaleLabel
                        {
#if WINDOWSPHONE_7 || SILVERLIGHT
                                LabelContent = (Math.Round(count, 1 )).ToString(),
#else
                            LabelContent = (Math.Round(count, 1, MidpointRounding.AwayFromZero)).ToString(),
#endif
                            LabelStroke = BindRangeStrokeToLabels && correspRange != null ? correspRange.RangeStroke : LabelStroke,
                            LabelSize = Double.IsNaN(LabelSize) ? ScaleBarHeight * 2 / 5 : LabelSize,
                            Transform = transform,
                            TransformOrigin = transformOrigin,
                            ParentScale = this
                        };
                    }
                    else
                    {
                        tick = new LinearScaleTick
                        {
                            TickLength = Double.IsNaN(MinorTickSize) ? ScaleBarHeight / 4 : MinorTickSize,
                            TickStrokeThickness = MinorTickStrokeThickness,
                            TickStroke = BindRangeStrokeToTicks && correspRange != null ? correspRange.RangeStroke : MinorTickStroke,
                            TickVerticalAlignment = TickPosition == LinearTicksPosition.Cross ? VerticalAlignment.Center : VerticalAlignment.Stretch,
                            ParentScale = this
                        };
                        label = new LinearScaleLabel
                        {
                            LabelContent = "",
                            LabelSize = Double.IsNaN(LabelSize) ? ScaleBarHeight * 2 / 5 : LabelSize,
                            ParentScale = this
                        };
                    }
                    Ticks.Add(tick);
                    Labels.Add(label);

                    count += MinorInterval;
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Pointers != null)
                Pointers.CollectionChanged -= Pointers_CollectionChanged;

            if (Ranges != null)
                Ranges.CollectionChanged -= Ranges_CollectionChanged;

            Ticks.Clear();
            Labels.Clear();
            if (Pointers != null)
                Pointers.Clear();
            if (Ranges != null)
                Ranges.Clear();
        }

        #endregion
    }

    #endregion
}
