#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.BulletGraph
{
    [TemplatePart(Name = "PART_BulletGraphGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_BulletGraphCaption", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_BulletGraphRanges", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_BulletGraphTicks", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_BulletGraphLabels", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_FeaturedMeasure", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_ComparativeMeasure", Type = typeof(ContentPresenter))]

    public class SfBulletGraph : Control
    {
        #region Constructor

        public SfBulletGraph()
        {
            DefaultStyleKey = typeof(SfBulletGraph);

            QualitativeRanges = new QualitativeRangeCollection();
            Ticks = new BulletGraphTickCollection();
            Labels = new BulletGraphLabelCollection();
            SetTicksAndLabels();

            QualitativeRanges.CollectionChanged += QualitativeRanges_CollectionChanged;
            SizeChanged += BulletGraph_SizeChanged;
        }

        #endregion

        #region Public Dependency Properties

        #region Orientation
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SfBulletGraph), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bulletGraph = (d as SfBulletGraph);
            if (bulletGraph != null)
            {
                if (bulletGraph.Orientation == Orientation.Vertical)
                {
                    bulletGraph.TransformOrigin = new Point(0.5, 0.5);
                    var transform = new TransformGroup();
                    if (bulletGraph.FlowDirection == BulletGraphFlowDirection.Forward)
                    {
                        transform.Children.Add(new RotateTransform { Angle = 90 });
                        transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                    }
                    else
                    {
                        transform.Children.Add(new RotateTransform { Angle = -90 });
                    }

                    bulletGraph.Transform = transform;
                }
                else
                {
                    var transform = new TransformGroup();
                    if (bulletGraph.FlowDirection == BulletGraphFlowDirection.Forward)
                    {
                        bulletGraph.TransformOrigin = new Point();
                    }
                    else
                    {
                        bulletGraph.TransformOrigin = new Point(0.5, 0.5);
                        transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                    }
                    bulletGraph.Transform = transform;
                }
            }
            if (bulletGraph != null && bulletGraph.Caption != null)
            {

                bulletGraph.SetTransformForCaption();
            }

            if (bulletGraph != null) bulletGraph.SetTicksAndLabels();
        }
        #endregion

        #region FlowDirection
        public new BulletGraphFlowDirection FlowDirection
        {
            get { return (BulletGraphFlowDirection)GetValue(FlowDirectionProperty); }
            set { SetValue(FlowDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleDirection.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty FlowDirectionProperty =
            DependencyProperty.Register("FlowDirection", typeof(BulletGraphFlowDirection), typeof(SfBulletGraph), new PropertyMetadata(BulletGraphFlowDirection.Forward, OnOrientationChanged));
        #endregion

        #region EnableAnimation
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(SfBulletGraph), new PropertyMetadata(true));
        #endregion

        #region Caption
        public object Caption
        {
            get { return GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(object), typeof(SfBulletGraph), new PropertyMetadata(null, OnCaptionChanged));

        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                if (e.NewValue != null)
                    bulletGraph.SetTransformForCaption();
            }
        }
        #endregion

        #region CaptionPosition
        public BulletGraphCaptionPosition CaptionPosition
        {
            get { return (BulletGraphCaptionPosition)GetValue(CaptionPositionProperty); }
            set { SetValue(CaptionPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaptionPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionPositionProperty =
            DependencyProperty.Register("CaptionPosition", typeof(BulletGraphCaptionPosition), typeof(SfBulletGraph), new PropertyMetadata(BulletGraphCaptionPosition.Near, OnCaptionPositionChanged));

        private static void OnCaptionPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = d as SfBulletGraph;
                bulletGraph.SetTransformForCaption();
            }
        }
        #endregion

        #region Minimum
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetComparativeMeasurePosition();
                    bulletGraph.SetPerfomanceMeasurePosition();
                    bulletGraph.SetRanges();
                    bulletGraph.SetTicksAndLabels();
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #region Maximum
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(100d, OnValueChanged));
        #endregion

        #region Interval
        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(Double.NaN, OnIntervalChanged));

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = d as SfBulletGraph;
                bulletGraph.SetTicksAndLabels();
            }
        }
        #endregion

        #region MinorTicksPerInterval
        public int MinorTicksPerInterval
        {
            get { return (int)GetValue(MinorTicksPerIntervalProperty); }
            set { SetValue(MinorTicksPerIntervalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTicksPerInterval.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTicksPerIntervalProperty =
            DependencyProperty.Register("MinorTicksPerInterval", typeof(int), typeof(SfBulletGraph), new PropertyMetadata(0, OnIntervalChanged));
        #endregion

        #region ComparativeMeasure
        public double ComparativeMeasure
        {
            get { return (double)GetValue(ComparativeMeasureProperty); }
            set { SetValue(ComparativeMeasureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasure.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComparativeMeasureProperty =
            DependencyProperty.Register("ComparativeMeasure", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d, OnComparativeMeasureChanged));

        private static void OnComparativeMeasureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.SetComparativeMeasurePosition();
            }
        }
        #endregion

        #region ComparativeMeasureSymbolStroke
        public Brush ComparativeMeasureSymbolStroke
        {
            get { return (Brush)GetValue(ComparativeMeasureSymbolStrokeProperty); }
            set { SetValue(ComparativeMeasureSymbolStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComparativeMeasureSymbolStrokeProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolStroke", typeof(Brush), typeof(SfBulletGraph), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        #endregion

        #region ComparativeMeasureSymbolStrokeThickness
        public double ComparativeMeasureSymbolStrokeThickness
        {
            get { return (double)GetValue(ComparativeMeasureSymbolStrokeThicknessProperty); }
            set { SetValue(ComparativeMeasureSymbolStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComparativeMeasureSymbolStrokeThicknessProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolStrokeThickness", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnComparativeMeasureSymbolStrokeThicknessChanged));

        private static void OnComparativeMeasureSymbolStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                (d as SfBulletGraph).ComparativeMeasureSymbolWidth = (double)e.NewValue;
            }
        }
        #endregion

        #region FeaturedMeasure
        public double FeaturedMeasure
        {
            get { return (double)GetValue(FeaturedMeasureProperty); }
            set { SetValue(FeaturedMeasureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturedMeasure.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeaturedMeasureProperty =
            DependencyProperty.Register("FeaturedMeasure", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d, OnFeaturedMeasureChanged));

        private static void OnFeaturedMeasureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                (d as SfBulletGraph).SetPerfomanceMeasurePosition();
            }
        }
        #endregion

        #region FeaturedMeasureBarStroke
        public Brush FeaturedMeasureBarStroke
        {
            get { return (Brush)GetValue(FeaturedMeasureBarStrokeProperty); }
            set { SetValue(FeaturedMeasureBarStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturedMeasureBarStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeaturedMeasureBarStrokeProperty =
            DependencyProperty.Register("FeaturedMeasureBarStroke", typeof(Brush), typeof(SfBulletGraph), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region FeaturedMeasureBarStrokeThickness
        public double FeaturedMeasureBarStrokeThickness
        {
            get { return (double)GetValue(FeaturedMeasureBarStrokeThicknessProperty); }
            set { SetValue(FeaturedMeasureBarStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturedMeasureBarStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeaturedMeasureBarStrokeThicknessProperty =
            DependencyProperty.Register("FeaturedMeasureBarStrokeThickness", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnFeaturedMeasureBarStrokeThicknessChanged));

        private static void OnFeaturedMeasureBarStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                (d as SfBulletGraph).FeaturedMeasureBarSize = (double)e.NewValue;
            }
        }
        #endregion

        #region MajorTickSize
        public double MajorTickSize
        {
            get { return (double)GetValue(MajorTickSizeProperty); }
            set { SetValue(MajorTickSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickSizeProperty =
            DependencyProperty.Register("MajorTickSize", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnTicksChanged));

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.SetTicksAndLabels();
            }
        }
        #endregion

        #region MinorTickSize
        public double MinorTickSize
        {
            get { return (double)GetValue(MinorTickSizeProperty); }
            set { SetValue(MinorTickSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickSizeProperty =
            DependencyProperty.Register("MinorTickSize", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnTicksChanged));
        #endregion

        #region MajorTickStrokeThickness
        public double MajorTickStrokeThickness
        {
            get { return (double)GetValue(MajorTickStrokeThicknessProperty); }
            set { SetValue(MajorTickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeThicknessProperty =
            DependencyProperty.Register("MajorTickStrokeThickness", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(2d, OnTicksChanged));
        #endregion

        #region MinorTickStrokeThickness
        public double MinorTickStrokeThickness
        {
            get { return (double)GetValue(MinorTickStrokeThicknessProperty); }
            set { SetValue(MinorTickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeThicknessProperty =
            DependencyProperty.Register("MinorTickStrokeThickness", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(1d, OnTicksChanged));
        #endregion

        #region MajorTickStroke
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(SfBulletGraph), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));
        #endregion

        #region MinorTickStroke
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(SfBulletGraph), new PropertyMetadata(new SolidColorBrush(Colors.White), OnTicksChanged));
        #endregion

        #region LabelStroke
        public Brush LabelStroke
        {
            get { return (Brush)GetValue(LabelStrokeProperty); }
            set { SetValue(LabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelStrokeProperty =
            DependencyProperty.Register("LabelStroke", typeof(Brush), typeof(SfBulletGraph), new PropertyMetadata(new SolidColorBrush(Colors.White), OnLabelsChanged));
        #endregion

        #region LabelSize
        public double LabelSize
        {
            get { return (double)GetValue(LabelSizeProperty); }
            set { SetValue(LabelSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelSizeProperty =
            DependencyProperty.Register("LabelSize", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnLabelsChanged));

        private static void OnLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.SetTicksAndLabels();
            }
        }
        #endregion

        #region LabelOffset
        public double LabelOffset
        {
            get { return (double)GetValue(LabelOffsetProperty); }
            set { SetValue(LabelOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelOffsetProperty =
            DependencyProperty.Register("LabelOffset", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d, OnLabelOffsetChanged));

        private static void OnLabelOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #region LabelFormat
        /// <summary>
        /// Gets or sets a format for labels in bullet graph.
        /// </summary>
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register("LabelFormat", typeof(string), typeof(SfBulletGraph), new PropertyMetadata(string.Empty, OnLabelsChanged));
        #endregion

        #region BindRangeStrokeToLabels
        public bool BindRangeStrokeToLabels
        {
            get { return (bool)GetValue(BindRangeStrokeToLabelsProperty); }
            set { SetValue(BindRangeStrokeToLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindRangeStrokeToLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindRangeStrokeToLabelsProperty =
            DependencyProperty.Register("BindRangeStrokeToLabels", typeof(bool), typeof(SfBulletGraph), new PropertyMetadata(false, OnBindRangeStrokeToLabelsChanged));

        private static void OnBindRangeStrokeToLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                (d as SfBulletGraph).SetTicksAndLabels();
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
            DependencyProperty.Register("BindRangeStrokeToTicks", typeof(bool), typeof(SfBulletGraph), new PropertyMetadata(false, OnBindRangeStrokeToTicksChanged));

        private static void OnBindRangeStrokeToTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                (d as SfBulletGraph).SetTicksAndLabels();
            }
        }
        #endregion

        #region TickPosition
        public BulletGraphTicksPosition TickPosition
        {
            get { return (BulletGraphTicksPosition)GetValue(TickPositionProperty); }
            set { SetValue(TickPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickPositionProperty =
            DependencyProperty.Register("TickPosition", typeof(BulletGraphTicksPosition), typeof(SfBulletGraph), new PropertyMetadata(BulletGraphTicksPosition.Below, OnTickPositionChanged));

        private static void OnTickPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetTicksAndLabels();
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #region LabelPosition
        public BulletGraphLabelsPosition LabelPosition
        {
            get { return (BulletGraphLabelsPosition)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPositionProperty =
            DependencyProperty.Register("LabelPosition", typeof(BulletGraphLabelsPosition), typeof(SfBulletGraph), new PropertyMetadata(BulletGraphLabelsPosition.Below, OnLabelPositionChanged));

        private static void OnLabelPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #region QualitativeRanges
        public QualitativeRangeCollection QualitativeRanges
        {
            get { return (QualitativeRangeCollection)GetValue(QualitativeRangesProperty); }
            set { SetValue(QualitativeRangesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QualitativeRanges.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QualitativeRangesProperty =
            DependencyProperty.Register("QualitativeRanges", typeof(QualitativeRangeCollection), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region QualitativeRangesSize
        public double QualitativeRangesSize
        {
            get { return (double)GetValue(QualitativeRangesSizeProperty); }
            set { SetValue(QualitativeRangesSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QualitativeRangesSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QualitativeRangesSizeProperty =
            DependencyProperty.Register("QualitativeRangesSize", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnQualitativeRangesSizeChanged));

        private static void OnQualitativeRangesSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.RangesHeight = (double)e.NewValue;
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetRanges();
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #region QuantitativeScaleLength
        public double QuantitativeScaleLength
        {
            get { return (double)GetValue(QuantitativeScaleLengthProperty); }
            set { SetValue(QuantitativeScaleLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuantitativeScaleLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuantitativeScaleLengthProperty =
            DependencyProperty.Register("QuantitativeScaleLength", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnQuantitativeScaleLengthChanged));

        private static void OnQuantitativeScaleLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.RangesWidth = (double)e.NewValue;
                if (bulletGraph.isMeasured)
                {
                    bulletGraph.SetComparativeMeasurePosition();
                    bulletGraph.SetPerfomanceMeasurePosition();
                    bulletGraph.SetRanges();
                    bulletGraph.SetMargins();
                }
            }
        }
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region QuantativeScaleWidth
        internal double QuantativeScaleWidth
        {
            get { return (double)GetValue(QuantativeScaleWidthProperty); }
            set { SetValue(QuantativeScaleWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuantativeScaleWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty QuantativeScaleWidthProperty =
            DependencyProperty.Register("QuantativeScaleWidth", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN));
        #endregion

        #region RangesWidth
        internal double RangesWidth
        {
            get { return (double)GetValue(RangesWidthProperty); }
            set { SetValue(RangesWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangesWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangesWidthProperty =
            DependencyProperty.Register("RangesWidth", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN));
        #endregion

        #region RangesHeight
        internal double RangesHeight
        {
            get { return (double)GetValue(RangesHeightProperty); }
            set { SetValue(RangesHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangesHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangesHeightProperty =
            DependencyProperty.Register("RangesHeight", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN, OnRangesHeightChanged));

        private static void OnRangesHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfBulletGraph)
            {
                var bulletGraph = (d as SfBulletGraph);
                bulletGraph.ComparativeMeasureSymbolHeight = bulletGraph.RangesHeight * 3 / 4;
                bulletGraph.ComparativeMeasureSymbolWidth = Double.IsNaN(bulletGraph.ComparativeMeasureSymbolStrokeThickness) ?
                                                            bulletGraph.ComparativeMeasureSymbolHeight / 5 : bulletGraph.ComparativeMeasureSymbolStrokeThickness;
                bulletGraph.FeaturedMeasureBarSize = Double.IsNaN(bulletGraph.FeaturedMeasureBarSize) ? bulletGraph.RangesHeight / 2 : bulletGraph.FeaturedMeasureBarSize;
            }
        }
        #endregion

        #region ComparativeMeasureSymbolTop
        internal double ComparativeMeasureSymbolTop
        {
            get { return (double)GetValue(ComparativeMeasureSymbolTopProperty); }
            set { SetValue(ComparativeMeasureSymbolTopProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolTop.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ComparativeMeasureSymbolTopProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolTop", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d));
        #endregion

        #region ComparativeMeasureSymbolHeight
        internal double ComparativeMeasureSymbolHeight
        {
            get { return (double)GetValue(ComparativeMeasureSymbolHeightProperty); }
            set { SetValue(ComparativeMeasureSymbolHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ComparativeMeasureSymbolHeightProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolHeight", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN));
        #endregion

        #region ComparativeMeasureSymbolWidth
        internal double ComparativeMeasureSymbolWidth
        {
            get { return (double)GetValue(ComparativeMeasureSymbolWidthProperty); }
            set { SetValue(ComparativeMeasureSymbolWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ComparativeMeasureSymbolWidthProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolWidth", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN));
        #endregion

        #region ComparativeMeasureSymbolPosition
        internal double ComparativeMeasureSymbolPosition
        {
            get { return (double)GetValue(ComparativeMeasureSymbolPositionProperty); }
            set { SetValue(ComparativeMeasureSymbolPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComparativeMeasureSymbolPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ComparativeMeasureSymbolPositionProperty =
            DependencyProperty.Register("ComparativeMeasureSymbolPosition", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d));
        #endregion

        #region FeaturedMeasureBarLength
        internal double FeaturedMeasureBarLength
        {
            get { return (double)GetValue(FeaturedMeasureBarLengthProperty); }
            set { SetValue(FeaturedMeasureBarLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturedMeasureBarLength.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FeaturedMeasureBarLengthProperty =
            DependencyProperty.Register("FeaturedMeasureBarLength", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(0d));
        #endregion

        #region FeaturedMeasureBarSize
        internal double FeaturedMeasureBarSize
        {
            get { return (double)GetValue(FeaturedMeasureBarSizeProperty); }
            set { SetValue(FeaturedMeasureBarSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeaturedMeasureBarSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty FeaturedMeasureBarSizeProperty =
            DependencyProperty.Register("FeaturedMeasureBarSize", typeof(double), typeof(SfBulletGraph), new PropertyMetadata(double.NaN));
        #endregion

        #region Ranges
        internal ObservableCollection<QualitativeRange> Ranges
        {
            get { return (ObservableCollection<QualitativeRange>)GetValue(RangesProperty); }
            set { SetValue(RangesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ranges.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RangesProperty =
            DependencyProperty.Register("Ranges", typeof(ObservableCollection<QualitativeRange>), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region Ticks
        internal BulletGraphTickCollection Ticks
        {
            get { return (BulletGraphTickCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        internal static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(BulletGraphTickCollection), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region Labels
        internal BulletGraphLabelCollection Labels
        {
            get { return (BulletGraphLabelCollection)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }

        internal static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(BulletGraphLabelCollection), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region TicksMargin
        internal Thickness TicksMargin
        {
            get { return (Thickness)GetValue(TicksMarginProperty); }
            set { SetValue(TicksMarginProperty, value); }
        }

        internal static readonly DependencyProperty TicksMarginProperty =
           DependencyProperty.Register("TicksMargin", typeof(Thickness), typeof(SfBulletGraph), new PropertyMetadata(new Thickness()));
        #endregion

        #region LabelsMargin
        internal Thickness LabelsMargin
        {
            get { return (Thickness)GetValue(LabelsMarginProperty); }
            set { SetValue(LabelsMarginProperty, value); }
        }

        internal static readonly DependencyProperty LabelsMarginProperty =
           DependencyProperty.Register("LabelsMargin", typeof(Thickness), typeof(SfBulletGraph), new PropertyMetadata(new Thickness()));
        #endregion

        #region RangesMargin
        internal Thickness RangesMargin
        {
            get { return (Thickness)GetValue(RangesMarginProperty); }
            set { SetValue(RangesMarginProperty, value); }
        }

        internal static readonly DependencyProperty RangesMarginProperty =
           DependencyProperty.Register("RangesMargin", typeof(Thickness), typeof(SfBulletGraph), new PropertyMetadata(new Thickness()));
        #endregion

        #region Transform
        internal TransformGroup Transform
        {
            get { return (TransformGroup)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transform.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(TransformGroup), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region TransformOrigin
        internal Point TransformOrigin
        {
            get { return (Point)GetValue(TransformOriginProperty); }
            set { SetValue(TransformOriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TransformOrigin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TransformOriginProperty =
            DependencyProperty.Register("TransformOrigin", typeof(Point), typeof(SfBulletGraph), new PropertyMetadata(new Point()));
        #endregion

        #region CaptionTransform
        internal TransformGroup CaptionTransform
        {
            get { return (TransformGroup)GetValue(CaptionTransformProperty); }
            set { SetValue(CaptionTransformProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CaptionTransform.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CaptionTransformProperty =
            DependencyProperty.Register("CaptionTransform", typeof(TransformGroup), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #region BulletGraphSizes
        internal Size BulletGraphSize
        {
            get { return (Size)GetValue(BulletGraphSizeProperty); }
            set { SetValue(BulletGraphSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BulletGraphSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty BulletGraphSizeProperty =
            DependencyProperty.Register("BulletGraphSize", typeof(Size), typeof(SfBulletGraph), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Private Members

        ItemsControl bulletGraphTicksControl;
        Grid bulletGraphGrid;
        ContentPresenter bulletGraphCaptionPresenter;
        Rectangle comparativeMeasureSymbol, performanceMeasureBar;
        bool isMeasured;
        Storyboard comparativeMeasureStoryBoard, perfomanceMeasureStoryBoard;
        DoubleAnimation comparativeMeasureAnimation, perfomanceMeasureAnimation;
        double BulletGraphHeight, BulletGraphWidth;

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()

#else
        public override void OnApplyTemplate()
#endif
        {
            bulletGraphTicksControl = GetTemplateChild("PART_BulletGraphTicks") as ItemsControl;
            bulletGraphGrid = GetTemplateChild("PART_BulletGraphGrid") as Grid;
            bulletGraphCaptionPresenter = GetTemplateChild("PART_BulletGraphCaption") as ContentPresenter;
            bulletGraphCaptionPresenter.SizeChanged += bulletGraphCaptionPresenter_SizeChanged;
            if (bulletGraphCaptionPresenter != null)
            {
                int col;
                if (FlowDirection == BulletGraphFlowDirection.Forward)
                {
                    col = (CaptionPosition == BulletGraphCaptionPosition.Near) ? 0 : 2;
                }
                else
                {
                    col = (CaptionPosition == BulletGraphCaptionPosition.Near) ? 2 : 0;

                }
                Grid.SetColumn(bulletGraphCaptionPresenter, col);
            }

            comparativeMeasureSymbol = GetTemplateChild("PART_ComparativeMeasureSymbol") as Rectangle;
            performanceMeasureBar = GetTemplateChild("PART_FeaturedMeasureBar") as Rectangle;

            base.OnApplyTemplate();

        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!double.IsNaN(availableSize.Height) && !double.IsInfinity(availableSize.Height) && !double.IsNaN(availableSize.Width) && !double.IsInfinity(availableSize.Width))
            {
                BulletGraphWidth = Double.IsNaN(Width) ? availableSize.Width : Width;
                BulletGraphHeight = Double.IsNaN(Height) ? availableSize.Height : Height;
                if (bulletGraphGrid != null)
                {
                    bulletGraphGrid.Width = BulletGraphWidth;
                    bulletGraphGrid.Height = BulletGraphHeight;
                    bulletGraphGrid.Clip = new RectangleGeometry { Rect = new Rect(0, 0, BulletGraphWidth, BulletGraphHeight) };
                }
                if (Double.IsNaN(QuantitativeScaleLength))
                {
                    double captionSize = 0;
                    if (Orientation == Orientation.Horizontal && (!double.IsNaN(BulletGraphHeight) && !double.IsNaN(BulletGraphWidth) && !double.IsInfinity(BulletGraphHeight) && !double.IsInfinity(BulletGraphWidth)))
                    {
                        if (bulletGraphCaptionPresenter != null)
                            captionSize = bulletGraphCaptionPresenter.ActualWidth;
                        RangesWidth = 3 * (BulletGraphWidth - captionSize) / 4;

                    }
                    else if (!double.IsNaN(BulletGraphHeight) && !double.IsNaN(BulletGraphWidth) && !double.IsInfinity(BulletGraphHeight) && !double.IsInfinity(BulletGraphWidth))
                    {
                        if (bulletGraphCaptionPresenter != null)
                            captionSize = bulletGraphCaptionPresenter.ActualHeight;
                        RangesWidth = 2 * ((BulletGraphHeight < BulletGraphWidth ? BulletGraphHeight : BulletGraphWidth) - captionSize) / 3;
                    }
                }
                if (Double.IsNaN(QualitativeRangesSize))
                    RangesHeight = RangesWidth / 10;

                isMeasured = true;

                if (Caption != null)
                    SetTransformForCaption();

                SetComparativeMeasurePosition();
                SetPerfomanceMeasurePosition();
                SetRanges();
                SetTicksAndLabels();
                SetMargins();
                return availableSize;
            }
            else
                return new Size(100, 100);
        }

        #endregion

        #region Implementation

        private void BulletGraph_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BulletGraphHeight = e.NewSize.Height;
            BulletGraphWidth = e.NewSize.Width;
        }

        void bulletGraphCaptionPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Caption != null && bulletGraphCaptionPresenter != null)
            {
                if (Double.IsNaN(QuantitativeScaleLength))
                {
                    double captionSize = 0;
                    if (Orientation == Orientation.Horizontal && (!double.IsNaN(BulletGraphHeight) && !double.IsNaN(BulletGraphWidth) && !double.IsInfinity(BulletGraphHeight) && !double.IsInfinity(BulletGraphWidth)))
                    {
                        captionSize = bulletGraphCaptionPresenter.ActualWidth;
                        RangesWidth = 3 * (BulletGraphWidth - captionSize) / 4;
                    }
                    else if (!double.IsNaN(BulletGraphHeight) && !double.IsNaN(BulletGraphWidth) && !double.IsInfinity(BulletGraphHeight) && !double.IsInfinity(BulletGraphWidth))
                    {
                        captionSize = bulletGraphCaptionPresenter.ActualHeight;
                        RangesWidth = 2 * ((BulletGraphHeight < BulletGraphWidth ? BulletGraphHeight : BulletGraphWidth) - captionSize) / 3;
                    }
                    if (Double.IsNaN(QualitativeRangesSize))
                        RangesHeight = RangesWidth / 10;
                    SetComparativeMeasurePosition();
                    SetPerfomanceMeasurePosition();
                    SetRanges();
                    SetTicksAndLabels();
                    SetMargins();
                }
            }
        }

        private void QualitativeRanges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SetRanges();
        }

        private void SetComparativeMeasurePosition()
        {
            if (EnableAnimation)
            {
                comparativeMeasureStoryBoard = new Storyboard();
                comparativeMeasureAnimation = new DoubleAnimation
                                                  {
                                                      Duration = TimeSpan.FromMilliseconds(300),
                                                      FillBehavior = FillBehavior.HoldEnd,
                                                      From = ComparativeMeasureSymbolPosition,
#if WINRT
                                                      EnableDependentAnimation = true
#endif
                                                  };
                comparativeMeasureStoryBoard.Children.Add(comparativeMeasureAnimation);
            }

            double measure = ComparativeMeasure;
            if (measure < Minimum)
                measure = Minimum;
            if (measure > Maximum)
                measure = Maximum;
            ComparativeMeasureSymbolPosition = ConvertMeasureToValue(measure) - (Double.IsNaN(ComparativeMeasureSymbolWidth) ? 0 : ComparativeMeasureSymbolWidth / 2);

            if (comparativeMeasureSymbol != null && EnableAnimation)
            {
                Storyboard.SetTarget(comparativeMeasureAnimation, comparativeMeasureSymbol);
#if WINRT
                Storyboard.SetTargetProperty(comparativeMeasureAnimation, "(Canvas.Left)");

#else
                Storyboard.SetTargetProperty(comparativeMeasureAnimation, new PropertyPath("(Canvas.Left)"));

#endif

                comparativeMeasureAnimation.To = ComparativeMeasureSymbolPosition;
                comparativeMeasureStoryBoard.Begin();
            }
        }

        private void SetPerfomanceMeasurePosition()
        {
            if (EnableAnimation)
            {
                perfomanceMeasureStoryBoard = new Storyboard();
                perfomanceMeasureAnimation = new DoubleAnimation
                                                 {
                                                     Duration = TimeSpan.FromMilliseconds(300),
                                                     FillBehavior = FillBehavior.Stop,
                                                     From = FeaturedMeasureBarLength,
#if WINRT
                                                     EnableDependentAnimation = true
#endif
                                                 };

                perfomanceMeasureStoryBoard.Children.Add(perfomanceMeasureAnimation);
            }

            double measure = FeaturedMeasure;
            if (measure < Minimum)
                measure = Minimum;
            if (measure > Maximum)
                measure = Maximum;
            FeaturedMeasureBarLength = ConvertMeasureToValue(measure);

            if (performanceMeasureBar != null && EnableAnimation)
            {
                Storyboard.SetTarget(perfomanceMeasureAnimation, performanceMeasureBar);
#if WINRT
                Storyboard.SetTargetProperty(perfomanceMeasureAnimation, "Width");

#else
                Storyboard.SetTargetProperty(perfomanceMeasureAnimation, new PropertyPath("Width"));
#endif

                perfomanceMeasureAnimation.To = FeaturedMeasureBarLength;
                perfomanceMeasureStoryBoard.Begin();
            }
        }

        internal void SetMargins()
        {
            double majorTickSize = Double.IsNaN(MajorTickSize) ? RangesHeight / 2 : MajorTickSize;
            double minorTickSize = Double.IsNaN(MinorTickSize) ? RangesHeight / 4 : MinorTickSize;
            double labelSize = Double.IsNaN(LabelSize) ? RangesHeight * 2 / 5 : LabelSize;

            ComparativeMeasureSymbolTop = (BulletGraphHeight - ComparativeMeasureSymbolHeight) / 2;

            #region LabelPosition -> Below
            if (LabelPosition == BulletGraphLabelsPosition.Below && TickPosition == BulletGraphTicksPosition.Below)
            {
                LabelsMargin = new Thickness(0, ((BulletGraphHeight + RangesHeight) / 2 + Math.Max(majorTickSize, minorTickSize) + LabelOffset), 0, 0);
                TicksMargin = new Thickness(0, (BulletGraphHeight / 2 + RangesHeight / 2), 0, 0);
                if (bulletGraphTicksControl != null)
                    bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };

            }
            else if (LabelPosition == BulletGraphLabelsPosition.Below && TickPosition == BulletGraphTicksPosition.Cross)
            {
                LabelsMargin = new Thickness(0, (BulletGraphHeight + RangesHeight) / 2 + LabelOffset, 0, 0);
                TicksMargin = new Thickness();
                bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
            }
            else if (LabelPosition == BulletGraphLabelsPosition.Below && TickPosition == BulletGraphTicksPosition.Above)
            {
                LabelsMargin = new Thickness(0, (BulletGraphHeight + RangesHeight) / 2 + LabelOffset, 0, 0);
                TicksMargin = new Thickness(0, (BulletGraphHeight - RangesHeight) / 2, 0, 0);
                bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
            }
            #endregion

            #region LabelPosition -> Above
            else if (LabelPosition == BulletGraphLabelsPosition.Above && TickPosition == BulletGraphTicksPosition.Below)
            {
                LabelsMargin = new Thickness(0, ((BulletGraphHeight - RangesHeight) / 2 - labelSize - LabelOffset), 0, 0);
                TicksMargin = new Thickness(0, (BulletGraphHeight / 2 + RangesHeight / 2), 0, 0);
                bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
            }
            else if (LabelPosition == BulletGraphLabelsPosition.Above && TickPosition == BulletGraphTicksPosition.Cross)
            {
                LabelsMargin = new Thickness(0, ((BulletGraphHeight - RangesHeight) / 2 - labelSize - LabelOffset), 0, 0);
                TicksMargin = new Thickness();
                bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = 1 };
            }
            else if (LabelPosition == BulletGraphLabelsPosition.Above && TickPosition == BulletGraphTicksPosition.Above)
            {
                LabelsMargin = new Thickness(0, ((BulletGraphHeight - RangesHeight) / 2 - Math.Max(majorTickSize, minorTickSize) - labelSize - LabelOffset), 0, 0);
                TicksMargin = new Thickness(0, (BulletGraphHeight - RangesHeight) / 2, 0, 0);
                bulletGraphTicksControl.RenderTransform = new ScaleTransform { ScaleY = -1 };
            }
            #endregion
        }

        private void SetTransformForCaption()
        {
            int col;
            if (FlowDirection == BulletGraphFlowDirection.Forward)
                col = (CaptionPosition == BulletGraphCaptionPosition.Near) ? 0 : 2;
            else
                col = (CaptionPosition == BulletGraphCaptionPosition.Near) ? 2 : 0;

            if (bulletGraphCaptionPresenter != null)
                Grid.SetColumn(bulletGraphCaptionPresenter, col);

            var transform = new TransformGroup();

            if (Orientation == Orientation.Vertical)
            {
                if (FlowDirection == BulletGraphFlowDirection.Forward)
                {
                    transform.Children.Add(new ScaleTransform { ScaleY = -1 });
                    transform.Children.Add(new RotateTransform { Angle = 90 });
                }
                else
                    transform.Children.Add(new RotateTransform { Angle = 90 });
            }
            else
            {
                if (FlowDirection == BulletGraphFlowDirection.Backward)
                    transform.Children.Add(new ScaleTransform { ScaleX = -1 });
            }
            CaptionTransform = transform;

        }

        private TransformGroup SetTransformForLabels()
        {
            var transform = new TransformGroup();
            if (Orientation == Orientation.Horizontal && FlowDirection == BulletGraphFlowDirection.Backward)
            {
                transform.Children.Add(new ScaleTransform { ScaleX = -1 });
            }
            else if (Orientation == Orientation.Vertical && FlowDirection == BulletGraphFlowDirection.Forward)
            {
                transform.Children.Add(new ScaleTransform { ScaleX = -1 });
                transform.Children.Add(new RotateTransform { Angle = -90 });
            }
            else if (Orientation == Orientation.Vertical && FlowDirection == BulletGraphFlowDirection.Backward)
            {
                transform.Children.Add(new RotateTransform { Angle = 90 });
            }
            return transform;
        }

        private double ConvertMeasureToValue(double measure)
        {
            if (!Double.IsNaN(RangesWidth) && measure >= Minimum && measure <= Maximum)
            {
                return (measure - Minimum) * RangesWidth / (Maximum - Minimum);
            }
            return 0;
        }

        internal void SetRanges()
        {
            Ranges = QualitativeRanges.Count == 0 ? new ObservableCollection<QualitativeRange> { new QualitativeRange { RangeEnd = Maximum } } : new ObservableCollection<QualitativeRange>(QualitativeRanges.OrderBy(qualitativeRange => qualitativeRange.RangeEnd));

            for (int i = 0; i < Ranges.Count; i++)
            {
                double currentRangeEnd = CheckRangeEnd(Ranges[i].RangeEnd);
                if (i == 0)
                    Ranges[i].RangeWidth = ConvertMeasureToValue(currentRangeEnd);
                else
                {
                    double prevRangeEnd = CheckRangeEnd(Ranges[i - 1].RangeEnd);
                    if (currentRangeEnd.Equals(prevRangeEnd))
                    {
                        double tempRangeWidth = Ranges[i - 1].RangeWidth;
                        Ranges[i - 1].RangeWidth = ConvertMeasureToValue(currentRangeEnd) - ConvertMeasureToValue(prevRangeEnd);
                        Ranges[i].RangeWidth = tempRangeWidth;
                    }
                    else
                        Ranges[i].RangeWidth = ConvertMeasureToValue(currentRangeEnd) - ConvertMeasureToValue(prevRangeEnd);
                }
                Ranges[i].ParentBulletGraph = this;
            }
        }

        private double CheckRangeEnd(double rangeEnd)
        {
            if (rangeEnd < Minimum)
                rangeEnd = Minimum;
            if (rangeEnd > Maximum)
                rangeEnd = Maximum;
            return rangeEnd;
        }

        internal void SetTicksAndLabels()
        {
            Ticks.Clear();
            Labels.Clear();

            if (Interval <= 0 || double.IsNaN(Interval) || Interval > (Math.Abs(Minimum - Maximum)))
            {
                Interval = (Math.Abs(Minimum - Maximum) / 10);
            }
            else
            {
                MinorTicksPerInterval = MinorTicksPerInterval < 0 ? 1 : MinorTicksPerInterval;
                int majorTicksCount = (int)(Math.Abs(Minimum - Maximum) / Interval) + 1;
                var minorTicksCount = (int)((Math.Abs(Minimum - Maximum) % Interval) / (Interval / (MinorTicksPerInterval + 1)));
                int totalTicksCount = majorTicksCount + MinorTicksPerInterval * (majorTicksCount - 1) + minorTicksCount;

                double MinorInterval = Interval / (MinorTicksPerInterval + 1);
                double count = Minimum;

                for (int i = 0; i < totalTicksCount; i++)
                {
                    QualitativeRange correspRange = null;
                    if (Ranges != null && Ranges.Count > 0 && (BindRangeStrokeToTicks || BindRangeStrokeToLabels))
                    {
                        foreach (QualitativeRange range in Ranges)
                        {
                            if (count <= range.RangeEnd)
                            {
                                correspRange = range;
                                break;
                            }
                        }
                    }

                    BulletGraphTick tick;
                    BulletGraphLabel label;
                    if ((i) % (MinorTicksPerInterval + 1) == 0)
                    {
                        tick = (new BulletGraphTick
                                    {
                                        TickLength = Double.IsNaN(MajorTickSize) ? RangesHeight / 2 : MajorTickSize,
                                        TickStrokeThickness = MajorTickStrokeThickness,
                                        TickStroke = BindRangeStrokeToTicks && correspRange != null ? correspRange.RangeStroke : MajorTickStroke,
                                        TickVerticalAlignment = TickPosition == BulletGraphTicksPosition.Cross ? VerticalAlignment.Center : VerticalAlignment.Stretch,
                                        ParentBulletGraph = this
                                    });

                        label = new BulletGraphLabel
                        {
#if SILVERLIGHT
                            LabelContent = (Math.Round(count, 1)).ToString(LabelFormat),
#else
                            LabelContent = (Math.Round(count, 1, MidpointRounding.AwayFromZero)).ToString(LabelFormat),
#endif
                            LabelStroke = BindRangeStrokeToLabels && correspRange != null ? correspRange.RangeStroke : LabelStroke,
                            LabelSize = Double.IsNaN(LabelSize) ? RangesHeight * 2 / 5 : LabelSize,
                            Transform = SetTransformForLabels(),
                            ParentBulletGraph = this
                        };
                    }
                    else
                    {
                        tick = (new BulletGraphTick
                                    {
                                        TickLength = Double.IsNaN(MinorTickSize) ? RangesHeight / 2 : MinorTickSize,
                                        TickStrokeThickness = MinorTickStrokeThickness,
                                        TickStroke = BindRangeStrokeToTicks && correspRange != null ? correspRange.RangeStroke : MinorTickStroke,
                                        TickVerticalAlignment = TickPosition == BulletGraphTicksPosition.Cross ? VerticalAlignment.Center : VerticalAlignment.Stretch,
                                        ParentBulletGraph = this
                                    });

                        label = new BulletGraphLabel
                        {
                            LabelContent = "",
                            LabelSize = Double.IsNaN(LabelSize) ? RangesHeight * 2 / 5 : LabelSize,
                            ParentBulletGraph = this
                        };
                    }
                    Ticks.Add(tick);
                    Labels.Add(label);

                    count += MinorInterval;
                }
            }
        }

        #endregion

    }
}
