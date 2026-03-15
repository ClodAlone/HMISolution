#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Windows.Automation.Peers;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents InteractiveCursor class
    /// </summary>
    public class InteractiveCursor : Control, IChartSerializer, INotifyPropertyChanged
    {
        private class InteractiveCursorAutomationPeer : FrameworkElementAutomationPeer
        {
            public InteractiveCursorAutomationPeer(InteractiveCursor control)
                : base(control)
            {
            }

            protected override string GetClassNameCore()
            {
                return "InteractiveCursor";
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }

            public override object GetPattern(PatternInterface patternInterface)
            {               
                return this;
            }

            private InteractiveCursor MyOwner
            {
                get
                {
                    return (InteractiveCursor)base.Owner;
                }
            }
        }

        /// <summary>
        /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new InteractiveCursorAutomationPeer(this);
        }

        #region Label
        /// <summary>
        /// Initializes the <see cref="InteractiveCursor"/> class.
        /// </summary>
        static InteractiveCursor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(InteractiveCursor), new FrameworkPropertyMetadata(typeof(InteractiveCursor)));
        }
        /// <summary>
        /// Called When instance created for InteractiveCursor
        /// </summary>
        public InteractiveCursor()
        {
            this.Loaded += new System.Windows.RoutedEventHandler(InteractiveCursor_Loaded);
        }

        private int index;
        /// <summary>
        /// Gets or sets the index of the collection.
        /// </summary>
        /// <value>The index of the collection.</value>
        internal int CollectionIndex
        {
            set { index = value; }
            get { return index; }
        }

        private double m_y1 = 0.0;
        /// <summary>
        /// Gets or sets the y1.
        /// </summary>
        /// <value>The y1.</value>
        internal double Y1
        {
            set { m_y1 = value; }
            get { return m_y1; }
        }

        internal int areaIndex = 0;

        //internal static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        //internal Visibility LabelVisibility
        //{
        //    get { return (Visibility)GetValue(LabelVisibilityProperty); }
        //    set { SetValue(LabelVisibilityProperty, value); }
        //}




        /// <summary>
        /// Identifies the IsBindWithSegment, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsBindWithSegmentProperty = DependencyProperty.Register("IsBindWithSegment", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(true, new PropertyChangedCallback(OnBindWidthSegmentChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bind with segment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is bind with segment; otherwise, <c>false</c>.
        /// </value>
        public bool IsBindWithSegment
        {
            set { SetValue(IsBindWithSegmentProperty, value); }
            get { return (bool)GetValue(IsBindWithSegmentProperty); }
        }

        /// <summary>
        /// Identifies the IsBindWithMouseMove, It is a Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsBindWithMouseMoveProperty = DependencyProperty.Register("IsBindWithMouseMove", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is bind with segment.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is bind with segment; otherwise, <c>false</c>.
        /// </value>
        public bool IsBindWithMouseMove
        {
            set { SetValue(IsBindWithMouseMoveProperty, value); }
            get { return (bool)GetValue(IsBindWithMouseMoveProperty); }
        }

        /// <summary>
        /// Identifies the Cursor Visibility
        /// </summary>
        public static readonly DependencyProperty CursorVisibilityProperty = DependencyProperty.Register("CursorVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or Sets value fro the Cursor Visibility
        /// </summary>
        public Visibility CursorVisibility
        {
            get { return (Visibility)GetValue(CursorVisibilityProperty); }
            set { SetValue(CursorVisibilityProperty, value); }
        }

        /// <summary>
        /// Sets the thickness for the Cursor Stroke
        /// </summary>
        public static readonly DependencyProperty CursorStrokeThicknessProperty = DependencyProperty.Register("CursorStrokeThickness", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(1.0));
        /// <summary>
        /// Gets or sets the cursor stroke thickness.
        /// </summary>
        /// <value>The cursor stroke thickness.</value>
        public double CursorStrokeThickness
        {
            get { return (double)GetValue(CursorStrokeThicknessProperty); }
            set { SetValue(CursorStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalCursorStroke, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorStrokeProperty = DependencyProperty.Register("HorizontalCursorStroke", typeof(Brush), typeof(InteractiveCursor), new PropertyMetadata(Brushes.BlueViolet));
        /// <summary>
        /// Gets or sets the horizontal cursor stroke.
        /// </summary>
        /// <value>The horizontal cursor stroke.</value>
        public Brush HorizontalCursorStroke
        {
            get { return (Brush)GetValue(HorizontalCursorStrokeProperty); }
            set { SetValue(HorizontalCursorStrokeProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalCursorStroke, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalCursorStrokeProperty = DependencyProperty.Register("VerticalCursorStroke", typeof(Brush), typeof(InteractiveCursor), new PropertyMetadata(Brushes.BlueViolet));
        /// <summary>
        /// Gets or sets the vertical cursor stroke.
        /// </summary>
        /// <value>The vertical cursor stroke.</value>
        public Brush VerticalCursorStroke
        {
            get { return (Brush)GetValue(VerticalCursorStrokeProperty); }
            set { SetValue(VerticalCursorStrokeProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalCursorVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorVisibilityProperty = DependencyProperty.Register("HorizontalCursorVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the horizontal cursor visibility.
        /// </summary>
        /// <value>The horizontal cursor visibility.</value>
        public Visibility HorizontalCursorVisibility
        {
            get { return (Visibility)GetValue(HorizontalCursorVisibilityProperty); }
            set { SetValue(HorizontalCursorVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the InteractiveCursorSymbol Width
        /// </summary>
        public double CursorSymbolWidth
        {
            get { return (double)GetValue(CursorSymbolWidthProperty); }
            set { SetValue(CursorSymbolWidthProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for CurSorWidth.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty CursorSymbolWidthProperty =
            DependencyProperty.Register("CursorSymbolWidth", typeof(double), typeof(InteractiveCursor), new UIPropertyMetadata(10d));
        /// <summary>
        /// Gets or Sets the Interactive Cursor Symbol Height
        /// </summary>
        public double CursorSymbolHeight
        {
            get { return (double)GetValue(CursorSymbolHeightProperty); }
            set { SetValue(CursorSymbolHeightProperty, value); }
        }

        
        
       /// <summary>
        /// Using a DependencyProperty as the backing store for CurSorHeight.  This enables animation, styling, binding, etc... 
       /// </summary>
        public static readonly DependencyProperty CursorSymbolHeightProperty =
            DependencyProperty.Register("CursorSymbolHeight", typeof(double), typeof(InteractiveCursor), new UIPropertyMetadata(10d));



        /// <summary>
        /// Gets or sets the Interactive Cursor Symbol Stroke
        /// </summary>
        public Brush CursorSymbolStroke
        {
            get { return (Brush)GetValue(CursorSymbolStrokeProperty); }
            set { SetValue(CursorSymbolStrokeProperty, value); }
        }

        
      /// <summary>
        /// Using a DependencyProperty as the backing store for CursorSymbolInterior.  This enables animation, styling, binding, etc...
      /// </summary>
        public static readonly DependencyProperty CursorSymbolStrokeProperty =
            DependencyProperty.Register("CursorSymbolStroke", typeof(Brush), typeof(InteractiveCursor), new UIPropertyMetadata(Brushes.Gray));


        /// <summary>
        /// Gets or Sets the InteractiveCursor Vertical Line Template
        /// </summary>
        public DataTemplate VerticalCursorTemplate
        {
            get { return (DataTemplate)GetValue(VerticalCursorTemplateProperty); }
            set { SetValue(VerticalCursorTemplateProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerTicalCursorTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalCursorTemplateProperty =
            DependencyProperty.Register("VerticalCursorTemplate", typeof(DataTemplate), typeof(InteractiveCursor), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or Sets the InteractiveCursor Horizontal Line Template
        /// </summary>
        public DataTemplate HorizontalCursorTemplate
        {
            get { return (DataTemplate)GetValue(HorizontalCursorTemplateProperty); }
            set { SetValue(HorizontalCursorTemplateProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HorizontalCursorTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalCursorTemplateProperty =
            DependencyProperty.Register("HorizontalCursorTemplate", typeof(DataTemplate), typeof(InteractiveCursor), new UIPropertyMetadata(null));

        
        /// <summary>
        /// Identifies the HorizontalCursorVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalCursorVisibilityProperty = DependencyProperty.Register("VerticalCursorVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the horizontal cursor visibility.
        /// </summary>
        /// <value>The horizontal cursor visibility.</value>
        public Visibility VerticalCursorVisibility
        {
            get { return (Visibility)GetValue(VerticalCursorVisibilityProperty); }
            set { SetValue(VerticalCursorVisibilityProperty, value); }
        }

        /// <summary>
        /// Get and Set DataPointProperty
        /// </summary>
        public IChartDataPoint DataPoint
        {
            get { return (IChartDataPoint)GetValue(DataPointProperty); }
            set { SetValue(DataPointProperty, value); }
        }

        /// <summary>
        /// Get and Set PointCollectionProperty
        /// </summary>
        public ObservableCollection<Point> PointCollection
        {
            get { return (ObservableCollection<Point>)GetValue(PointCollectionProperty); }
            set { SetValue(PointCollectionProperty, value); }
        }

        
       /// <summary>
        ///Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc... 
       /// </summary>
        public static readonly DependencyProperty PointCollectionProperty =
            DependencyProperty.Register("PointCollection", typeof(ObservableCollection<Point>), typeof(InteractiveCursor), new UIPropertyMetadata(null));
 
       
        /// <summary>
        /// Using a DependencyProperty as the backing store for DataPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DataPointProperty =
            DependencyProperty.Register("DataPoint", typeof(IChartDataPoint), typeof(InteractiveCursor), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the LabelPosition, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty LabelPositionProperty = DependencyProperty.Register("LabelPosition", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(0.0d));
        /// <summary>
        /// Gets or sets the label position.
        /// </summary>
        /// <value>The label position.</value>
        internal double LabelPosition
        {
            get { return (double)GetValue(LabelPositionProperty); }
            set { SetValue(LabelPositionProperty, value); }
        }


        /// <summary>
        /// Identifies the IsInversedLabel, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsInversedLabelProperty = DependencyProperty.Register("IsInversedLabel", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating whether this instance is inversed label.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is inversed label; otherwise, <c>false</c>.
        /// </value>
        public bool IsInversedLabel
        {
            set { SetValue(IsInversedLabelProperty, value); }
            get { return (bool)GetValue(IsInversedLabelProperty); }
        }

        /// <summary>
        /// Identifies the EnableVerticalMove, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableVerticalMoveProperty = DependencyProperty.Register("EnableVerticalMove", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether [enable vertical move].
        /// </summary>
        /// <value><c>true</c> if [enable vertical move]; otherwise, <c>false</c>.</value>
        public bool EnableVerticalMove
        {
            get { return (bool)GetValue(EnableVerticalMoveProperty); }
            set { SetValue(EnableVerticalMoveProperty, value); }

        }

        /// <summary>
        /// Identifies the EnableHorizontalMove, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableHorizontalMoveProperty = DependencyProperty.Register("EnableHorizontalMove", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether [enable horizontal move].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable horizontal move]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableHorizontalMove
        {
            set { SetValue(EnableHorizontalMoveProperty, value); }
            get { return (bool)GetValue(EnableHorizontalMoveProperty); }
        }

        //internal static readonly DependencyProperty ActualHeightProperty = DependencyProperty.Register("ActualHeight", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        //internal double ActualHeight
        //{
        //    get { return (double)GetValue(ActualHeightProperty); }
        //    set { SetValue(ActualHeightProperty, value); }
        //}

        /// <summary>
        /// Identifies the XValue, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty XValueProperty = DependencyProperty.Register("XValue", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        /// <value>The X value.</value>
        internal double XValue
        {
            get { return (double)GetValue(XValueProperty); }
            set { SetValue(XValueProperty, value); }
        }

        /// <summary>
        /// Identifies the YValue, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        /// <value>The Y value.</value>
        internal double YValue
        {
            get { return (double)GetValue(YValueProperty); }
            set { SetValue(YValueProperty, value); }
        }

        /// <summary>
        /// Identifies the OffsetY, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register("OffsetY", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnOffsetYChanged), OnCoerceOffsetYValue));

        /// <summary>
        /// Gets or sets the offset Y.
        /// </summary>
        /// <value>The offset Y.</value>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as InteractiveCursor;
            if (instance != null) 
                instance.SetValueForInteractiveCursorSymbol();
        }

        /// <summary>
        /// Identifies the OffsetX, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register("OffsetX", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(0.0d, new PropertyChangedCallback(OnOffsetXChanged), OnCoerceOffsetXValue));
        /// <summary>
        /// Gets or sets the offset X.
        /// </summary>
        /// <value>The offset X.</value>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        private static object OnCoerceOffsetYValue(DependencyObject dObj, object value)
        {
            InteractiveCursor cursor = dObj as InteractiveCursor;
            double val = (double)value;
            if (cursor != null)
            {
                if (cursor.chartarea != null)
                {
                    if (cursor.chartarea.SyncChartArea != null || cursor.chartarea is SyncChartAreas)
                    {
                        SyncChartAreas syncArea = cursor.chartarea as SyncChartAreas;
                        if (syncArea != null)
                        {
                            if (syncArea.CursorSeries != null && syncArea.CursorSeries.YAxis != null)
                            {
                                if (syncArea.CursorSeries.Area != null)
                                {
                                    if (syncArea.CursorSeries.Area.PointToValue(syncArea.CursorSeries.YAxis, new Point(0, val)) > syncArea.CursorSeries.YAxis.VisibleRange.End ||
                                        syncArea.CursorSeries.Area.PointToValue(syncArea.CursorSeries.YAxis, new Point(0, val)) < syncArea.CursorSeries.YAxis.VisibleRange.Start)
                                    {
                                        return cursor.OffsetY;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (cursor.Selectedseries != null && cursor.Selectedseries.YAxis != null)
                        {
                            if (cursor.Selectedseries.Area != null)
                            {
                                if ((cursor.Selectedseries.Area.PointToValue(cursor.Selectedseries.YAxis, new Point(0, val)) > cursor.Selectedseries.YAxis.VisibleRange.End ||
                                    cursor.Selectedseries.Area.PointToValue(cursor.Selectedseries.YAxis, new Point(0, val)) < cursor.Selectedseries.YAxis.VisibleRange.Start) && 
                                    (cursor.IsBindWithSegment && (cursor.YValue > cursor.Selectedseries.YAxis.VisibleRange.End || cursor.YValue < cursor.Selectedseries.YAxis.VisibleRange.Start)))
                                {
                                    return cursor.OffsetY;
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as InteractiveCursor;
            if (instance != null)
                instance.SetValueForInteractiveCursorSymbol();
        }

         void InteractiveCursor_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (ChartAxis axis in chartarea.Axes)
            {
                axis.InteractiveCursorLabelVisibility =Visibility.Hidden;
                if (axis.InteractiveCursorTemplate == null)
                {
                    axis.InteractiveCursorTemplate = ChartDictionaries.GenericDictionary["HorizontalLabelTemplate"] as DataTemplate;
                }
                //axis.InteractiveCursorLabelBackground = ChartDictionaries.GenericDictionary["LightBrush"] as LinearGradientBrush;
                //axis.InteractiveCursorTemplate = (axis.Orientation == Orientation.Horizontal) ? ChartDictionaries.GenericDictionary["HorizontalLabelTemplate"] as DataTemplate : ChartDictionaries.GenericDictionary["VerticalLabelTemplate"] as DataTemplate;
            }
            SetValueForInteractiveCursorSymbol();
        }
        private static object OnCoerceOffsetXValue(DependencyObject dObj, object value)
        {
            return value;
            //InteractiveCursor cursor = dObj as InteractiveCursor;
            //double val = (double)value;
            //if (cursor != null)
            //{
            //    if (cursor.chartarea != null)
            //    {
            //        if (cursor.chartarea is SyncChartAreas)
            //        {
            //            SyncChartAreas syncArea = cursor.chartarea as SyncChartAreas;
            //            if (syncArea != null)
            //            {
            //                if (syncArea.CursorSeries != null && syncArea.CursorSeries.XAxis != null)
            //                {
            //                    if (syncArea.CursorSeries.Area != null)
            //                    {
            //                        if (syncArea.CursorSeries.Area.ValueToPoint(syncArea.CursorSeries.XAxis, val) > syncArea.CursorSeries.XAxis.VisibleRange.End)
            //                        {
            //                            return cursor.OffsetX;
            //                        }
            //                    }
            //                }

            //            }
            //        }
            //        else
            //        {
            //            if (cursor.Selectedseries != null && cursor.Selectedseries.XAxis != null)
            //            {
            //                if (cursor.Selectedseries.Area != null)
            //                {
            //                    if (cursor.Selectedseries.Area.ValueToPoint(cursor.Selectedseries.XAxis, val) > cursor.Selectedseries.XAxis.VisibleRange.End)
            //                    {
            //                        return cursor.OffsetX;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //return value;
        }


        /// <summary>
        /// Identifies the BindWithMoseMoveOnSegment, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty BindWithMouseMoveOnSegmentProperty = DependencyProperty.Register("BindWithMouseMoveOnSegment", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(false));
        /// <summary>
        /// Gets or sets a value indicating whether [bind with mouse move on segment].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [bind with mouse move on segment]; otherwise, <c>false</c>.
        /// </value>
        public bool BindWithMouseMoveOnSegment
        {
            get { return (bool)GetValue(BindWithMouseMoveOnSegmentProperty); }
            set { SetValue(BindWithMouseMoveOnSegmentProperty, value); }
        }

        //public static readonly DependencyProperty BindWithMoseMoveOnSeriesProperty = DependencyProperty.Register("BindWithMoseMoveOnSeries", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(false));
        //public bool BindWithMoseMoveOnSeries
        //{
        //    get { return (bool)GetValue(BindWithMoseMoveOnSeriesProperty); }
        //    set { SetValue(BindWithMoseMoveOnSeriesProperty, value); }
        //}

        private static void OnBindWidthSegmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InteractiveCursor ic = d as InteractiveCursor;
            if ((bool)e.NewValue)
                ic.GenerateSegmentPointCollection();
        }

        /// <summary>
        /// Identifies the LabelVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the label visibility.
        /// </summary>
        /// <value>The label visibility.</value>
        public Visibility LabelVisibility
        {
            get { return (Visibility)GetValue(LabelVisibilityProperty); }
            set { SetValue(LabelVisibilityProperty, value); }
        }



        /// <summary>
        /// Identifies the LabelBackground, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelBackgroundProperty = DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(InteractiveCursor), new PropertyMetadata(Brushes.Gray));
        /// <summary>
        /// Gets or sets the label background.
        /// </summary>
        /// <value>The label background.</value>
        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelForeground, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty LabelForegroundProperty = DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(InteractiveCursor), new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the label foreground.
        /// </summary>
        /// <value>The label foreground.</value>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelFont, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty LabelFontProperty = DependencyProperty.Register("LabelFont", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the label font.
        /// </summary>
        /// <value>The label font.</value>
        internal double LabelFont
        {
            get { return (double)GetValue(LabelFontProperty); }
            set { SetValue(LabelFontProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalCursorLabelPosition, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty HorizontalCursorLabelPositionProperty = DependencyProperty.Register("HorizontalCursorLabelPosition", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the horizontal cursor label position.
        /// </summary>
        /// <value>The horizontal cursor label position.</value>
        internal double HorizontalCursorLabelPosition
        {
            get { return (double)GetValue(HorizontalCursorLabelPositionProperty); }
            set { SetValue(HorizontalCursorLabelPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalCursorLabelPosition, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty VerticalCursorLabelPositionProperty = DependencyProperty.Register("VerticalCursorLabelPosition", typeof(double), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the vertical cursor label position.
        /// </summary>
        /// <value>The vertical cursor label position.</value>
        internal double VerticalCursorLabelPosition
        {
            get { return (double)GetValue(VerticalCursorLabelPositionProperty); }
            set { SetValue(VerticalCursorLabelPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalCursorLabelContent, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty HorizontalCursorLabelContentProperty = DependencyProperty.Register("HorizontalCursorLabelContent", typeof(object), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the content of the horizontal cursor label.
        /// </summary>
        /// <value>The content of the horizontal cursor label.</value>
        internal object HorizontalCursorLabelContent
        {
            get { return (object)GetValue(HorizontalCursorLabelContentProperty); }
            set { SetValue(HorizontalCursorLabelContentProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalCursorLabelContent, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalCursorLabelContentProperty = DependencyProperty.Register("VerticalCursorLabelContent", typeof(object), typeof(InteractiveCursor), new PropertyMetadata(null));
        /// <summary>
        /// Gets or sets the content of the vertical cursor label.
        /// </summary>
        /// <value>The content of the vertical cursor label.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal object VerticalCursorLabelContent
        {
            get { return (object)GetValue(VerticalCursorLabelContentProperty); }
            set { SetValue(VerticalCursorLabelContentProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalLabelVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty HorizontalLabelVisibilityProperty = DependencyProperty.Register("HorizontalLabelVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the horizontal label visibility.
        /// </summary>
        /// <value>The horizontal label visibility.</value>
        public Visibility HorizontalLabelVisibility
        {
            get { return (Visibility)GetValue(HorizontalLabelVisibilityProperty); }
            set { SetValue(HorizontalLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Interactive cursor Symbol visibility.
        /// </summary>
        /// <value>The Interactive cursor visibility.</value>
        public Visibility InteractiveCursorSymbolVisibility
        {
            get { return (Visibility)GetValue(InteractiveCursorSymbolVisibilityProperty); }
            set { SetValue(InteractiveCursorSymbolVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the HorizontalCursorVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty InteractiveCursorSymbolVisibilityProperty = DependencyProperty.Register("InteractiveCursorSymbolVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or Sets the InteractiveCusorSymbol Template
        /// </summary>
        public DataTemplate InteractiveCursorSymbolTemplate
        {
            get { return (DataTemplate)GetValue(InteractiveCursorSymbolTemplateProperty); }
            set { SetValue(InteractiveCursorSymbolTemplateProperty, value); }
        }


       
       /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalCursorTemplate.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty InteractiveCursorSymbolTemplateProperty =
            DependencyProperty.Register("InteractiveCursorSymbolTemplate", typeof(DataTemplate), typeof(InteractiveCursor), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifies the VerticalLabelVisibility, It is a Dependency Property
        /// </summary>
        public static readonly DependencyProperty VerticalLabelVisibilityProperty = DependencyProperty.Register("VerticalLabelVisibility", typeof(Visibility), typeof(InteractiveCursor), new PropertyMetadata(Visibility.Visible));
        /// <summary>
        /// Gets or sets the vertical label visibility.
        /// </summary>
        /// <value>The vertical label visibility.</value>
        public Visibility VerticalLabelVisibility
        {
            get { return (Visibility)GetValue(VerticalLabelVisibilityProperty); }
            set { SetValue(VerticalLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Identifies the VerticalLabelVisibility, It is a Dependency Property
        /// </summary>
        internal static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(InteractiveCursor), new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets the vertical label visibility.
        /// </summary>
        /// <value>The vertical label visibility.</value>
        internal bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        //public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.Register("LabelTemplate", typeof(ControlTemplate), typeof(InteractiveCursor), new PropertyMetadata(null));
        //public ControlTemplate LabelTemplate
        //{
        //    get { return (ControlTemplate)GetValue(LabelTemplateProperty); }
        //    set { SetValue(LabelTemplateProperty, value); }
        //}

        #endregion

        private Line m_HLine;
        /// <summary>
        /// Gets or sets the horizontal line.
        /// </summary>
        /// <value>The horizontal line.</value>
        internal Line HorizontalLine
        {
            set { m_HLine = value; }
            get { return m_HLine; }
        }

        private Line m_VLine;
        /// <summary>
        /// Gets or sets the vertical line.
        /// </summary>
        /// <value>The vertical line.</value>
        internal Line VerticalLine
        {
            set { m_VLine = value; }
            get { return m_VLine; }
        }

        private ChartSeries series;
        /// <summary>
        /// Gets or sets the chartseries.
        /// </summary>
        /// <value>The chartseries.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartSeries Selectedseries
        {
            get { return series; }
            set { series = value; }
        }

        internal ChartSeries CurrentSeries
        {
            get;
            set;
        }

        private ChartArea area;
        /// <summary>
        /// Gets or sets the chartarea.
        /// </summary>
        /// <value>The chartarea.</value>
        internal ChartArea chartarea
        {
            get { return area; }
            set { area = value; }
        }


        /// <summary>
        ///  Gets or Sets the Leftposition of the InteractiveCursorSymbol Canvas
        /// </summary>
        internal double LeftPosition
        {
            get { return (double)GetValue(LeftPositionProperty); }
            set { SetValue(LeftPositionProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LeftPostion.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LeftPositionProperty =
            DependencyProperty.Register("LeftPosition", typeof(double), typeof(InteractiveCursor), new UIPropertyMetadata(0d));


        /// <summary>
        /// Gets or Sets the Top position of the InteractiveCursorSymbol Canvas
        /// </summary>
        internal double TopPosition
        {
            get { return (double)GetValue(TopPositionProperty); }
            set { SetValue(TopPositionProperty, value); }
        }

        
       /// <summary>
        /// Using a DependencyProperty as the backing store for TopPosition.  This enables animation, styling, binding, etc...
       /// </summary>
        public static readonly DependencyProperty TopPositionProperty =
            DependencyProperty.Register("TopPosition", typeof(double), typeof(InteractiveCursor), new UIPropertyMetadata(0d));

        
        /// <summary>
        /// Identifies the InteractiveCurosr Collections
        /// </summary>
        internal InteractiveCursorCollection m_interactivecursor = new InteractiveCursorCollection();
        internal InteractiveCursorCollection interactivecursor
        {
            get { return m_interactivecursor; }
        }
        Canvas VerticalCursorLine;
        Canvas HorizontalCursorLine;
        Canvas InteractiveCursorSymbolCanvas;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
        /// </summary>
        /// <seealso cref="InteractiveCursor"/>
        public override void OnApplyTemplate()
        {
            if (chartarea != null)
            {
                base.OnApplyTemplate();
                VerticalCursorLine = this.GetTemplateChild("VerticalCursorCanvas") as Canvas;
                HorizontalCursorLine = this.GetTemplateChild("HorizontalCursorCanvas") as Canvas;
                InteractiveCursorSymbolCanvas = this.GetTemplateChild("InteractiveCursorSymbolCanvas") as Canvas;
                InteractiveCursorSymbolCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(InteractiveCursorSymbolCanvas_MouseLeftButtonDown);
                InteractiveCursorSymbolCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(InteractiveCursorSymbolCanvas_MouseLeftButtonUp);
                VerticalCursorLine.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalLine_MouseLeftButtonDown);
                VerticalCursorLine.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalLine_MouseLeftButtonUp);
                HorizontalCursorLine.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalLine_MouseLeftButtonDown);
                HorizontalCursorLine.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalLine_MouseLeftButtonUp);
                //this.InteractiveCursorSymbolCanvas.MouseMove += new MouseEventHandler(InteractiveCursorSymbolCanvas_MouseMove);
                Canvas canvas = this.GetTemplateChild("IC_canvas") as Canvas;

                //HorizontalCursorLine = this.GetTemplateChild("HLine") as Line;
                //HorizontalCursorLine.MouseLeftButtonDown += new MouseButtonEventHandler(HorizontalLine_MouseLeftButtonDown);
                //HorizontalCursorLine.MouseLeftButtonUp += new MouseButtonEventHandler(HorizontalLine_MouseLeftButtonUp);

                //VerticalCursorLine = this.GetTemplateChild("VLine") as Line;
                //VerticalCursorLine.MouseLeftButtonDown += new MouseButtonEventHandler(VerticalLine_MouseLeftButtonDown);
                //VerticalCursorLine.MouseLeftButtonUp += new MouseButtonEventHandler(VerticalLine_MouseLeftButtonUp);

                chartarea.MouseLeave += new MouseEventHandler(chartarea_MouseLeave);
                chartarea.MouseLeftButtonUp += new MouseButtonEventHandler(chartarea_MouseLeftButtonUp);
                //if (chartarea.SyncChartArea == null)
                {
                    chartarea.MouseMove += new MouseEventHandler(chartarea_MouseMove);
                }
                //else
                {

                }
                chartarea.MouseDoubleClick += new MouseButtonEventHandler(chartarea_MouseDoubleClick);

                //foreach (ChartSeries cs in this.chartarea.Series)
                //{
                //    cs.MouseMove += new ChartMouseEventHandler(cs_MouseMove);
                //}
                if (this.chartarea.SyncChartArea != null) { }
                //VerticalCursorLine.Y1 -= this.Y1;
                if(this.BindWithMouseMoveOnSegment)
                {
                    if (this.area != null && this.Selectedseries == null && this.area.Series != null &&this.area.Series.Count > 0)
                    {
                        this.Selectedseries = this.area.Series[0];
                        //this.XValue = this.Selectedseries.DataModel.ChartPoints[0].X;
                        //this.XValue = this.Selectedseries.DataModel.ChartPoints[0].Y;                        
                    }
                }
            }
        }

        void InteractiveCursorSymbolCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (VerticalCursorLine != null)
                this.VerticalCursorLine.RaiseEvent(e);
            if (HorizontalCursorLine != null)
                this.HorizontalCursorLine.RaiseEvent(e);
        }

        void InteractiveCursorSymbolCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(VerticalCursorLine !=null)
            this.VerticalCursorLine.RaiseEvent(e);
            if (HorizontalCursorLine != null)
                this.HorizontalCursorLine.RaiseEvent(e);
        }


        internal void AddlocationHandler(QtpInteracitveCursorLocationChangedEventHandler handler)
        {
            this.LocationChanged = handler;
        }
        /// <summary>
        /// Event for LocationChanged in InteractiveCursor
        /// </summary>
        public event QtpInteracitveCursorLocationChangedEventHandler LocationChanged;

        internal void OnLocationChanged(CursorLocationChangedeventArgs args)
        {
            if (LocationChanged != null)
            {
                LocationChanged(this, args);
            }
        }
        void chartarea_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.MoveOnMouseMove = false;
            if (area != null)
            {
                foreach (ChartAxis axis in area.Axes)
                {
                    axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                    axis.IsOpen = false;
                }
            }
        }

        private bool HCFlag = true;
        private bool VCFlag = true;
        internal bool SelectedCursor = false;

        internal double SyncOffsetY = 0.0d;

        internal bool MoveOnMouseMove = false;

        void VerticalLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.chartarea.SyncChartArea == null)
            {
                VCFlag = true;
                SelectedCursor = true;
            }
            else
            {
                this.chartarea.SyncChartArea.SyncInteractiveCursorMove = true;
                if (this.chartarea.SyncChartArea != null)
                {
                    if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                    {
                        foreach (InteractiveCursor ic in this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex].interactivecursor)
                            ic.SelectedCursor = true;
                    }
                }

            }
        }

        void HorizontalLine_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.chartarea.SyncChartArea == null)
            {
                HCFlag = true;
                SelectedCursor = true;
            }
            else
            {
                this.chartarea.SyncChartArea.SyncInteractiveCursorMove = true;
                if (this.chartarea.SyncChartArea != null)
                {
                    if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                    {
                        foreach (InteractiveCursor ic in this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex].interactivecursor)
                            ic.SelectedCursor = true;
                    }
                }
            }
        }

        void VerticalLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.chartarea.SyncChartArea == null)
            {
                HCFlag = false;
                VCFlag = false;
                SelectedCursor = false;
            }
            else
            {
                this.chartarea.SyncChartArea.SyncInteractiveCursorMove = false;
                if (this.chartarea.SyncChartArea != null)
                {
                    if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                    {
                        foreach (InteractiveCursor ic in this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex].interactivecursor)
                            ic.SelectedCursor = false;
                    }
                }
            }
        }

        void chartarea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (this.chartarea != null && chartarea.ChartAreaParent != null && (chartarea.ChartAreaParent is SyncChartAreas))
            {
                if (chartarea != null)
                    areaIndex = chartarea.index;
                (chartarea.ChartAreaParent as SyncChartAreas).selectedChartAreaForIC = chartarea;
                (chartarea.ChartAreaParent as SyncChartAreas).selectedChartSeriesForIC = Selectedseries;
            }


            if (this.chartarea.SyncChartArea == null)
            {
                HCFlag = false;
                VCFlag = false;
                SelectedCursor = false;
                if (SelectedCursor == false)
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                        axis.IsOpen = false;
                    }
                }
            }
            else
            {
                this.chartarea.SyncChartArea.SyncInteractiveCursorMove = false;
                SyncChartAreas Sarea = this.chartarea.SyncChartArea as SyncChartAreas;
                if (this.chartarea.SyncChartArea != null)
                {
                    if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                    {
                        foreach (InteractiveCursor ic in this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex].interactivecursor)
                            ic.SelectedCursor = false;
                        if (Sarea != null)
                        {
                            foreach (ChartArea area in Sarea.Areas)
                            {
                                foreach (ChartAxis axis in area.Axes)
                                {
                                    axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                                    axis.IsOpen = false;
                                }
                            }
                        }
                    }
                }

            }
        }

        void HorizontalLine_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.chartarea != null && chartarea.ChartAreaParent != null && (chartarea.ChartAreaParent is SyncChartAreas))
            {
                if (chartarea != null)
                    areaIndex = chartarea.index;
                (chartarea.ChartAreaParent as SyncChartAreas).selectedChartAreaForIC = chartarea;
                (chartarea.ChartAreaParent as SyncChartAreas).selectedChartSeriesForIC = Selectedseries;

            }

            if (this.chartarea.SyncChartArea == null)
            {
                HCFlag = false;
                VCFlag = false;
                SelectedCursor = false;
                //SelectedCursor = true;
            }
            else
            {
                this.chartarea.SyncChartArea.SyncInteractiveCursorMove = false;
                if (this.chartarea.SyncChartArea != null)
                {
                    if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                    {
                        foreach (InteractiveCursor ic in this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex].interactivecursor)
                            ic.SelectedCursor = false;
                    }
                }
            }
        }

        void chartarea_MouseLeave(object sender, MouseEventArgs e)
        {
            ChartMouseLeave();
        }

        private void ChartMouseLeave()
        {
            if (this.chartarea.SyncChartArea == null)
            {
                HCFlag = false;
                VCFlag = false;
                SelectedCursor = false;
                foreach (ChartAxis axis in area.Axes)
                {
                    axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                    axis.IsOpen = false;
                }
            }
            else
            {
                SyncChartAreas Sarea = this.chartarea.SyncChartArea as SyncChartAreas;
                foreach (ChartArea area in Sarea.Areas)
                {
                    foreach (ChartAxis axis in area.Axes)
                    {
                        axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                        axis.IsOpen = false;
                    }
                }
            }
        }

        void BindWithMouseMove(MouseEventArgs e)
        {
            IsOpen = true;
            Point pt = e.GetPosition(this);
            if (pt.X >= 0 && pt.X <= this.ActualWidth && pt.Y >= 0 && pt.Y <= this.ActualHeight)
            {
                InteractiveCursorLabelContent obj = new InteractiveCursorLabelContent();

                if (this.EnableHorizontalMove == true)
                {
                    if (this.chartarea != null)
                    {
                        this.OffsetY = pt.Y;
                        Point pt1 = e.GetPosition(this.chartarea);
                        
                        if (area != null && this.area.SyncChartArea == null)
                        {
                            foreach (ChartAxis axis in area.Axes)
                            {
                                axis.IsOpen = true;
                                if(axis.Orientation == Orientation.Vertical)
                                {
                                    this.VerticalCursorLabelContent = this.chartarea.PointToValue(axis, pt1);
                                    if (axis.ValueType == ChartValueType.DateTime)
                                    {
                                        this.VerticalCursorLabelContent = DateTime.FromOADate((double)this.VerticalCursorLabelContent).ToString(axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);                                       
                                    }
                                    axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                    axis.InteractiveCursorLabelTopPosition = OffsetY;
                                    axis.InteractiveCursorLabelLeftPosition = 0d;
                                    if (axis.InteractiveCursorContentVisibility == true)
                                    {
                                        if (axis.ValueType == ChartValueType.DateTime)
                                        {
                                            DateTime dt = DateTime.ParseExact(this.VerticalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                            obj.Y = dt;
                                        }
                                        else
                                        {
                                            if (IsInversedLabel == true)
                                                obj.X =(double)this.VerticalCursorLabelContent;
                                            else
                                                obj.Y =(double)this.VerticalCursorLabelContent;
                                        }
                                        obj.DataPoint = this.DataPoint;
                                        axis.InteractiveCursorLabelContent = obj;
                                        obj.Series = null;
                                        axis.InteractiveCursorLabelVisibility = this.VerticalLabelVisibility== Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                    }
                                }
                                if (axis.Orientation == Orientation.Horizontal && this.EnableVerticalMove==true)
                                {
                                    this.HorizontalCursorLabelContent = this.chartarea.PointToValue(axis, pt1);

                                    if (axis.ValueType == ChartValueType.DateTime)
                                    {
                                        this.HorizontalCursorLabelContent = DateTime.FromOADate((double)this.HorizontalCursorLabelContent).ToString(axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                    }
                                    if (area.SyncChartArea == null)
                                    {
                                        axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                        axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                        axis.InteractiveCursorLabelTopPosition = 0d;
                                        if (axis.InteractiveCursorContentVisibility)
                                        {
                                            if (axis.ValueType == ChartValueType.DateTime)
                                            {
                                                DateTime dt = DateTime.ParseExact(this.HorizontalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                                obj.X = dt;
                                            }
                                            else
                                            {
                                                if (IsInversedLabel == true)
                                                    obj.Y =(double)this.HorizontalCursorLabelContent;
                                                else
                                                    obj.X = (double)this.HorizontalCursorLabelContent;
                                            }
                                        }
                                        obj.Series = null;
                                        axis.InteractiveCursorLabelVisibility = this.HorizontalLabelVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                                        this.OnLocationChanged(new CursorLocationChangedeventArgs(area, this, axis));
                                    }                                    
                                }                     
                            }
                        }
                    }
                }



                if (this.EnableVerticalMove == true)
                {
                    this.OffsetX = pt.X;
                    Point pt1 = e.GetPosition(this.chartarea);
                    if (area != null)
                    {
                        foreach (ChartAxis axis in this.area.Axes)
                        {
                            axis.IsOpen = true;
                            if (axis.Orientation == Orientation.Horizontal)
                            {
                                this.HorizontalCursorLabelContent = this.chartarea.PointToValue(axis, pt1);

                                if (axis.ValueType == ChartValueType.DateTime)
                                {
                                    this.HorizontalCursorLabelContent = DateTime.FromOADate((double)this.HorizontalCursorLabelContent).ToString(axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);                                   
                                }
                                if (area.SyncChartArea == null)
                                {                                    
                                    axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                    axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                    axis.InteractiveCursorLabelTopPosition = 0d;
                                    if (axis.InteractiveCursorContentVisibility)
                                    {
                                        if (axis.ValueType == ChartValueType.DateTime)
                                        {
                                            DateTime dt = DateTime.ParseExact(this.HorizontalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                            obj.X = dt;
                                        }
                                        else
                                        {
                                            if (IsInversedLabel == true)
                                                obj.Y =(double)this.HorizontalCursorLabelContent;
                                            else
                                                obj.X =(double)this.HorizontalCursorLabelContent;
                                        }
                                    }
                                    obj.Series = null;
                                    axis.InteractiveCursorLabelVisibility = this.HorizontalLabelVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;                                   
                                    this.OnLocationChanged(new CursorLocationChangedeventArgs(area, this, axis));
                                }
                            }                            
                        }
                        foreach (ChartAxis axis in this.area.Axes)
                        {
                            if (area.SyncChartArea == null && axis.InteractiveCursorContentVisibility)
                            {
                                obj.DataPoint = this.DataPoint;
                                axis.InteractiveCursorLabelContent = obj;
                            }
                        }
                    }
                }
                //To display interactive cursor values in IsBindWithSegment as true.
               this.DataPoint = new ChartPoint() ;
                if (this.area.SyncChartArea != null && this.VerticalCursorLabelContent!= null)                    
                {
                    if (this.EnableVerticalMove == true)
                        this.DataPoint.X = obj.X is DateTime ? Convert.ToDateTime(this.VerticalCursorLabelContent.ToString()).ToOADate() : Convert.ToDouble(obj.X);
                    this.DataPoint.Y = obj.Y is DateTime ? Convert.ToDateTime(this.VerticalCursorLabelContent.ToString()).ToOADate() : Convert.ToDouble(obj.Y);
                }
                else
                {
                    if (this.EnableVerticalMove == true)
                        this.DataPoint.X = obj.X is DateTime ? Convert.ToDateTime(obj.X).ToOADate() : Convert.ToDouble(obj.X);
                    this.DataPoint.Y = obj.Y is DateTime ? Convert.ToDateTime(obj.Y).ToOADate() : Convert.ToDouble(obj.Y);
                }
                if (this.chartarea.SyncChartArea != null)
                {
                    SyncChartAreas Sarea = this.area.SyncChartArea as SyncChartAreas;
                    if (Sarea != null)
                    {
                        if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                        {
                            InteractiveCursor syncic = this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex];
                            foreach (InteractiveCursor ic in syncic.interactivecursor)
                                ic.HorizontalCursorVisibility = Visibility.Collapsed;
                            this.HorizontalCursorVisibility = Visibility.Visible;

                            if (syncic.LabelVisibility == Visibility.Visible)
                            {
                                //foreach (InteractiveCursor ic in syncic.interactivecursor)
                                //    ic.HorizontalLabelVisibility = Visibility.Collapsed;
                                //this.HorizontalLabelVisibility = Visibility.Visible;
                            }

                            
                            foreach (ChartArea area in Sarea.Areas)
                            {
                                if (area == this.area)
                                {
                                    Point pt1 = e.GetPosition(area);
                                    foreach (ChartAxis axis in area.Axes)
                                    {
                                        axis.IsOpen = true;
                                        if (axis.Orientation == Orientation.Horizontal) 
                                        {
                                            this.VerticalCursorLabelContent = this.chartarea.PointToValue(axis, pt1);
                                            if (axis.InteractiveCursorContentVisibility == true && VerticalCursorLabelContent!=null)
                                            {                                                
                                                if (axis.ValueType == ChartValueType.DateTime)
                                                {                                                   
                                                    obj.X = DateTime.FromOADate((double)VerticalCursorLabelContent);
                                                    
                                                }
                                                else
                                                {
                                                    if(IsInversedLabel==true)
                                                       obj.Y = (double)this.VerticalCursorLabelContent;
                                                    else
                                                        obj.X = (double)this.VerticalCursorLabelContent;
                                                }

                                                obj.Series = null;                                                
                                                if (area.index == Sarea.Areas.Count - 1)
                                                    axis.InteractiveCursorLabelVisibility = this.HorizontalLabelVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                                                else
                                                    axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;

                                                axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                axis.InteractiveCursorLabelTopPosition = 0d;
                                                this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));

                                            }
                                        }
                                        else if (axis.Orientation == Orientation.Vertical)
                                        {                                            
                                            double value = area.PointToValue(axis, pt1);


                                            obj.Y = value;
                                            obj.Series = null;                                            
                                            axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                            axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                            axis.InteractiveCursorLabelTopPosition = OffsetY;
                                            axis.InteractiveCursorLabelLeftPosition = 0d;
                                            axis.InteractiveCursorLabelContent = obj;
                                        }
                                    }
                                    foreach (ChartAxis axis in area.Axes)
                                    {
                                        if (axis.InteractiveCursorContentVisibility && axis.Orientation==Orientation.Horizontal)
                                            axis.InteractiveCursorLabelContent = obj;
                                    }
                                }
                                else
                                {
                                    foreach (ChartAxis axis in area.Axes)
                                    {
                                        axis.IsOpen = true;
                                        if (axis.InteractiveCursorContentVisibility == true)
                                        {
                                            if (axis.Orientation == Orientation.Vertical)
                                            {
                                                axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                                            }
                                            else if (axis.Orientation == Orientation.Horizontal && VerticalCursorLabelContent!=null)
                                            {
                                                if (axis.ValueType == ChartValueType.DateTime)
                                                {                                                    
                                                    obj.X = DateTime.FromOADate((double)VerticalCursorLabelContent);
                                                }
                                                else
                                                {
                                                    if (IsInversedLabel == true)
                                                        obj.Y = (double)this.VerticalCursorLabelContent;
                                                    else
                                                        obj.X = (double)this.VerticalCursorLabelContent;
                                                }

                                                obj.Series = null;                                                

                                                if (area.index == area.SyncChartArea.Areas.Count - 1)
                                                    axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                                else
                                                    axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;

                                                axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                axis.InteractiveCursorLabelTopPosition = 0d;
                                                this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));

                                            }
                                        }
                                    }
                                    foreach (ChartAxis axis in area.Axes)
                                    {
                                        if (axis.InteractiveCursorLabelVisibility==Visibility.Visible)
                                            axis.InteractiveCursorLabelContent = obj;
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.HorizontalCursorLabelContent != null)
                {
                    if (this.IsInversedLabel == true)
                        this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length - 1) * 8;// this.area.SecondaryAxis.LabelFontSize;
                    else
                        this.LabelPosition = this.ActualWidth;
                }

            }
        }

        internal bool isDifferentArea = false;
        internal double LabelPositionValue = 0d;
        internal ChartArea cursrorArea = null;
        internal bool flag = false;

        void chartarea_MouseMove(object sender, MouseEventArgs e)
        {
            cursrorArea = sender as ChartArea;
            if (cursrorArea != null)
            {
                foreach (ChartAxis axis in cursrorArea.Axes)
                {
                    //Commented for Panning operation to work in SyncChartAreas and this is not affecting normal scenarios.
                    //axis.IsOpen = true;
                    if (axis.Orientation == Orientation.Horizontal)
                    {

                        if (axis.Area.SyncChartArea != null)
                        {
                            if (axis.Area.SyncChartArea.Areas[axis.Area.SyncChartArea.Areas.Count - 1] != null && axis.Area.SyncChartArea.Areas[axis.Area.SyncChartArea.Areas.Count - 1].Axes[0] != null)
                            {
                                if (axis.Area.SyncChartArea.Areas[axis.Area.SyncChartArea.Areas.Count - 1].Axes[0].Orientation == Orientation.Horizontal)
                                {
                                    if (axis.Area.SyncChartArea.Areas[axis.Area.SyncChartArea.Areas.Count - 1].Axes[0].InteractiveCursorTemplate == null)
                                    {
                                        axis.Area.SyncChartArea.Areas[(axis.Area.SyncChartArea.Areas.Count - 1)].Axes[0].InteractiveCursorTemplate = ChartDictionaries.GenericDictionary["HorizontalLabelTemplate"] as DataTemplate;
                                    }
                                    axis.Area.SyncChartArea.Areas[(axis.Area.SyncChartArea.Areas.Count - 1)].Axes[0].InteractiveCursorLabelForeground = LabelForeground;
                                    axis.Area.SyncChartArea.Areas[(axis.Area.SyncChartArea.Areas.Count - 1)].Axes[0].InteractiveCursorLabelBackground = LabelBackground;
                                }
                            }

                        }

                        if (axis.InteractiveCursorTemplate == null)
                        {
                            axis.InteractiveCursorTemplate = ChartDictionaries.GenericDictionary["HorizontalLabelTemplate"] as DataTemplate;
                        }
                        axis.InteractiveCursorLabelForeground = LabelForeground;
                        axis.InteractiveCursorLabelBackground = LabelBackground;
                    }
                    else
                    {
                        if (axis.InteractiveCursorTemplate == null)
                        {
                            axis.InteractiveCursorTemplate = ChartDictionaries.GenericDictionary["VerticalLabelTemplate"] as DataTemplate;
                        }
                        axis.InteractiveCursorLabelForeground = LabelForeground;
                        axis.InteractiveCursorLabelBackground = LabelBackground;
                    }
                }
            }
            this.IsOpen = true;
            if (this.SelectedCursor)
            {
                this.chartarea = sender as ChartArea;
                if (this.IsBindWithSegment == true)
                {
                    if (this.chartarea.SyncChartArea == null)
                    {
                        Point nearPoint = new Point();
                        IChartDataPoint datapt = null;
                        #region initialize
                        foreach (ChartSeries series in this.chartarea.Series)
                        {
                            //Fix for moving interactive cursor from one series to another series in IsBindWithSegment as true.
                            if(this.CurrentSeries != null && series == this.CurrentSeries)
                            if (series.Segments.Count > 0)
                            {
                                if (series.Type == ChartTypes.Bar)
                                {
                                    nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.Y);
                                    nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.X);
                                }
                                else
                                {
                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.X);
                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.Y);
                                }
                                if (series != null)
                                {
                                    this.Selectedseries = series;
                                }

                                break;
                            }
                        }
                        #endregion
                        int serCount1 = 0;
                        if (IsBindWithSegment && this.PointCollection == null)
                        {
                            GenerateSegmentPointCollection();
                        }
                        foreach (ChartSeries series in this.chartarea.Series)
                        {
                            //Fix for moving interactive cursor from one series to another series in IsBindWithSegment as true.
                            if(this.CurrentSeries != null && series == this.CurrentSeries)
                            {
                            #region BarType
                            if (series.Type == ChartTypes.Bar ||
                                    series.Type == ChartTypes.RotatedSpline ||
                                    series.Type == ChartTypes.StackingBar ||
                                    series.Type == ChartTypes.StackingBar100 ||
                                    series.Type == ChartTypes.Tornado ||
                                    series.Type == ChartTypes.Gantt)
                            {
                                if (HCFlag == true)
                                {
                                    Point pt = e.GetPosition(this.chartarea);

                                    int count = 0;
                                    foreach (ChartSegment cs in series.Segments)
                                    {
                                        if (((pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                            (pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))) <=
                                            ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                        {
                                            nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                            nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                            if (series != null)
                                            {
                                                this.Selectedseries = series;
                                            }
                                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                                            datapt = cs.CorrespondingPoints[0].DataPoint;
                                        }
                                        count++;
                                        if (count == series.Segments.Count && series.Type == ChartTypes.RotatedSpline)
                                        {
                                            if (((pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X)) *
                                                (pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X))) <=
                                                ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                            {
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                                nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                                                datapt = cs.CorrespondingPoints[1].DataPoint;
                                            }
                                        }
                                    }
                                }
                                if (VCFlag == true)
                                {
                                    Point pt = e.GetPosition(this.chartarea);
                                    int count = 0;
                                    foreach (ChartSegment cs in series.Segments)
                                    {
                                        if (((pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                                            (pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))) <=
                                            ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                        {
                                            nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                            nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                            if (series != null)
                                            {
                                                this.Selectedseries = series;
                                            }
                                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                                            datapt = cs.CorrespondingPoints[0].DataPoint;
                                        }
                                        count++;
                                        if (count == series.Segments.Count && series.Type == ChartTypes.RotatedSpline)
                                        {
                                            if (((pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y)) *
                                                (pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y))) <=
                                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                            {
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                                nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                                                datapt = cs.CorrespondingPoints[1].DataPoint;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region AreaType
                            else if (series.Type == ChartTypes.Area ||
                                    series.Type == ChartTypes.FastColumn ||
                                    series.Type == ChartTypes.FastLine ||
                                    series.Type == ChartTypes.FastScatter ||
                                    series.Type == ChartTypes.HiLoArea ||
                                    series.Type == ChartTypes.SplineArea ||
                                    series.Type == ChartTypes.StackingArea ||
                                    series.Type == ChartTypes.StackingLine ||
                                    series.Type == ChartTypes.StackingSpline ||
                                    series.Type == ChartTypes.StackingSplineArea ||
                                    series.Type == ChartTypes.StepArea ||
                                    series.Type == ChartTypes.RangeArea)
                            {
                                if (this.PointCollection == null)
                                {
                                    this.GenerateSegmentPointCollection();
                                }
                                else
                                {
                                    if (HCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);
                                        foreach (Point dataPoint in this.PointCollection)
                                        {
                                            if ((pt.Y - dataPoint.Y) * (pt.Y - dataPoint.Y) <= (pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y))
                                            {
                                                nearPoint.X = dataPoint.X;
                                                nearPoint.Y = dataPoint.Y;
                                                if (this.series != null)
                                                    this.Selectedseries = series;
                                                this.XValue = dataPoint.X;
                                                this.YValue = dataPoint.Y;
                                                datapt = new ChartPoint(dataPoint.X, dataPoint.Y);
                                            }
                                        }
                                    }
                                    if (VCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);
                                        foreach (Point dataPoint in this.PointCollection)
                                        {
                                            if ((pt.X - dataPoint.X) * (pt.X - dataPoint.X) <= (pt.X - nearPoint.X) * (pt.X - nearPoint.X))
                                            {
                                                nearPoint.X = dataPoint.X;
                                                nearPoint.Y = dataPoint.Y;
                                                if (this.series != null)
                                                    this.Selectedseries = series;
                                                this.XValue = dataPoint.X;
                                                this.YValue = dataPoint.Y;
                                                datapt = new ChartPoint(dataPoint.X, dataPoint.Y);

                                            }
                                        }
                                    }
                                }
                            }
                    
                            #endregion

                            #region StackingColumn
                            if (series.Type == ChartTypes.StackingColumn || series.Type == ChartTypes.FastStackingColumn)
                            {
                                if (serCount1 == 0)
                                {
                                    if (HCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);

                                        foreach (ChartSegment cs in series.Segments)
                                        {
                                            if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                                                (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))) <=
                                                ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                            {
                                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;

                                                flag = false;
                                                datapt = cs.CorrespondingPoints[0].DataPoint;
                                            }

                                        }

                                    }

                                    if (VCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);

                                        foreach (ChartSegment cs in series.Segments)
                                        {
                                            if (((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                                (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))) <=
                                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                            {
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                                                flag = false;
                                                datapt = cs.CorrespondingPoints[0].DataPoint;
                                            }

                                        }
                                    }
                                }
                                else
                                {

                                    if (HCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);
                                        int count = 0;
                                        if ((this.chartarea.Series[serCount1 - 1].Type == ChartTypes.StackingColumn) || (this.chartarea.Series[serCount1 - 1].Type == ChartTypes.FastStackingColumn))
                                        {
                                            foreach (ChartSegment cs in series.Segments)
                                            {
                                                if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y))) *
                                                    (pt.Y - this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y)))) <=
                                                    ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                                {
                                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y));
                                                    if (series != null)
                                                    {
                                                        this.Selectedseries = series;
                                                    }

                                                    if (serCount1 > 0)
                                                    {
                                                        this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                                        this.YValue = this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y + cs.CorrespondingPoints[0].DataPoint.Y;
                                                        datapt = new ChartPoint(this.XValue, this.YValue);
                                                    }
                                                    flag = true;

                                                }

                                                count++;
                                            }

                                        }
                                    }

                                    if (VCFlag == true)
                                    {
                                        Point pt = e.GetPosition(this.chartarea);
                                        int count = 0;
                                        if ((this.chartarea.Series[serCount1 - 1].Type == ChartTypes.StackingColumn) || (this.chartarea.Series[serCount1 - 1].Type == ChartTypes.FastStackingColumn))
                                        {
                                            foreach (ChartSegment cs in series.Segments)
                                            {
                                                if (((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                                    (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))) <=
                                                    ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                                {
                                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y));
                                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                                    if (series != null)
                                                    {
                                                        this.Selectedseries = series;
                                                    }

                                                    if (serCount1 > 0)
                                                    {
                                                        this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                                        this.YValue = this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y + cs.CorrespondingPoints[0].DataPoint.Y;
                                                        datapt = new ChartPoint(this.XValue, this.YValue);
                                                    }
                                                    flag = true;
                                                }
                                                count++;
                                            }
                                        }
                                    }
                                }
                                serCount1++;
                            }
                            #endregion

                            #region ColumnType
                            else
                            {
                                if (HCFlag == true)
                                {
                                    Point pt = e.GetPosition(this.chartarea);
                                    int count = 0;
                                    foreach (ChartSegment cs in series.Segments)
                                    {
                                        if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                                            (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))) <=
                                            ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                        {
                                            nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                            nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                            if (series != null)
                                            {
                                                this.Selectedseries = series;
                                            }
                                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                                            datapt = cs.CorrespondingPoints[0].DataPoint;
                                        }
                                        count++;
                                        if (count == series.Segments.Count && (series.Type == ChartTypes.Line || series.Type == ChartTypes.Spline ||
                                            series.Type == ChartTypes.StepLine || series.Type == ChartTypes.ThreeLineBreak))
                                        {
                                            if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y)) *
                                                (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y))) <=
                                                ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)))
                                            {
                                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                                                datapt = cs.CorrespondingPoints[1].DataPoint;
                                            }
                                        }
                                    }

                                }

                                if (VCFlag == true)
                                {
                                    Point pt = e.GetPosition(this.chartarea);
                                    int count = 0;
                                    foreach (ChartSegment cs in series.Segments)
                                    {
                                        if (((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                            (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))) <=
                                            ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                        {
                                            nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                            nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                            if (series != null)
                                            {
                                                this.Selectedseries = series;
                                            }
                                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                                            datapt = cs.CorrespondingPoints[0].DataPoint;
                                        }
                                        count++;
                                        if (count == series.Segments.Count && (series.Type == ChartTypes.Line || series.Type == ChartTypes.Spline ||
                                            series.Type == ChartTypes.StepLine || series.Type == ChartTypes.ThreeLineBreak))
                                        {
                                            if (((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X)) *
                                                (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X))) <=
                                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                            {
                                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                                if (series != null)
                                                {
                                                    this.Selectedseries = series;
                                                }
                                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                                                datapt = cs.CorrespondingPoints[1].DataPoint;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            }
                        }
                        if (IsBindWithSegment && IsBindWithMouseMove == false && BindWithMouseMoveOnSegment == false || (IsBindWithSegment && IsBindWithMouseMove))
                       {
                            this.DataPoint = datapt;
                        }
                        SetValueForInteractiveCursor(true);
                        #region ChartArea
                        if (area.SyncChartArea == null)
                        {
                            InteractiveCursorLabelContent obj = new InteractiveCursorLabelContent();

                            foreach (ChartAxis axis in area.Axes)
                            {
                                axis.IsOpen = true;
                                if (axis.Orientation == Orientation.Horizontal)
                                {
                                    if (axis.InteractiveCursorContentVisibility == true)
                                    {

                                        if (axis.InteractiveCursorContentVisibility == true && HorizontalCursorLabelContent!=null)
                                        if (axis.InteractiveCursorContentVisibility == true && this.VerticalCursorLabelContent != null)
                                        {
                                            axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility== Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                            

                                            if (axis.ValueType == ChartValueType.DateTime)
                                            {
                                                obj.X = DateTime.ParseExact(this.HorizontalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);// DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                            }
                                            else
                                            {
                                                obj.X = Convert.ToDouble(this.HorizontalCursorLabelContent);
                                            }

                                            obj.Y = Convert.ToDouble(this.VerticalCursorLabelContent);
                                            obj.Series = series;
                                            //axis.InteractiveCursorLabelContent = obj;
                                            obj.DataPoint = this.DataPoint;
                                            axis.InteractiveCursorLabelContent = obj;
                                            axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                            axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                            axis.InteractiveCursorLabelTopPosition = 0d;
                                            this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));
                                        }
                                    }
                                }
                                if (axis.Orientation == Orientation.Vertical)
                                {
                                    double value = this.area.PointToValue(axis, axiscursorLabelValue);

                                    if (axis.InteractiveCursorContentVisibility == true && VerticalCursorLabelContent!=null)
                                    {
                                        axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Hidden;
                                        if (axis.ValueType == ChartValueType.DateTime)
                                        {
                                            obj.Y = DateTime.ParseExact(this.VerticalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                        }
                                        else
                                        {
                                            obj.Y = Convert.ToDouble(this.VerticalCursorLabelContent);
                                        }
                                        //obj.Y = Math.Round(value, 2);
                                        obj.Series = series;
                                        //axis.InteractiveCursorLabelContent = obj;
                                        obj.DataPoint = this.DataPoint;
                                        axis.InteractiveCursorLabelContent = obj;
                                        axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                        axis.InteractiveCursorLabelTopPosition = OffsetY;
                                        axis.InteractiveCursorLabelLeftPosition = 0d;
                                    }
                                }
                            }
                            foreach (ChartAxis axis in area.Axes)
                            {
                                if (axis.InteractiveCursorContentVisibility == true)
                                {
                                    axis.InteractiveCursorLabelContent = obj;
                                }
                            }
                        }
                        #endregion
                    }
                    #region SyncChartArea
                    if (this.chartarea.SyncChartArea != null)
                    {
                        if (this.chartarea.SyncChartArea.SyncInteractiveCursorMove == true)
                            SyncInteractiveCursor_MouseMove(e);
                        SyncChartAreas Sarea = this.area.SyncChartArea as SyncChartAreas;
                        if (Sarea != null && BindWithMouseMoveOnSegment == false)
                        {
                            if (Sarea.InteractiveCursors.Count > 0)
                            {
                                InteractiveCursor syncic = Sarea.InteractiveCursors[this.CollectionIndex];
                                InteractiveCursorLabelContent obj = new InteractiveCursorLabelContent();

                                foreach (ChartArea area in Sarea.Areas)
                                {
                                    if (area == this.area)
                                    {
                                        foreach (ChartAxis axis in area.Axes)
                                        {
                                            axis.IsOpen = true;
                                            if (axis.InteractiveCursorContentVisibility)
                                            {
                                                if (axis.Orientation == Orientation.Horizontal)
                                                {

                                                    if (area.index == Sarea.Areas.Count - 1)
                                                        axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                                    else
                                                        axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;


                                                    if (axis.ValueType == ChartValueType.DateTime)
                                                    {
                                                        double content = DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                                        obj.X = DateTime.FromOADate((double)content);
                                                    }
                                                    else
                                                    {
                                                        obj.X = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                    }

                                                    obj.Series = series;                                                               
                                                    axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                    axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                    axis.InteractiveCursorLabelTopPosition = 0d;
                                                    this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));

                                                }
                                                if (axis.Orientation == Orientation.Vertical)
                                                {
                                                    double value = this.area.PointToValue(axis, axiscursorLabelValue);

                                                    axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;

                                                    obj.Y = value;
                                                    obj.Series = series;                 
                                                    axis.InteractiveCursorLabelContent = obj;
                                                    axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                                    axis.InteractiveCursorLabelTopPosition = OffsetY;
                                                    axis.InteractiveCursorLabelLeftPosition = 0d;
                                                }
                                            }
                                        }
                                        foreach (ChartAxis axis in area.Axes)
                                        {
                                            if(axis.Orientation==Orientation.Horizontal && axis.InteractiveCursorContentVisibility)
                                                axis.InteractiveCursorLabelContent = obj;
                                        }
                                    }
                                    else
                                    {
                                        foreach (ChartAxis axis in area.Axes)
                                        {
                                            axis.IsOpen = true;
                                            if (axis.InteractiveCursorContentVisibility)
                                            {
                                                if (axis.Orientation == Orientation.Vertical)
                                                {
                                                    axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                                                }
                                                else
                                                {
                                                    if (area.index == area.SyncChartArea.Areas.Count - 1)
                                                        axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                                    else
                                                        axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;



                                                    if (axis.ValueType == ChartValueType.DateTime)
                                                    {
                                                        double content = DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                                        obj.X = DateTime.FromOADate((double)content);
                                                    }
                                                    else
                                                    {
                                                        obj.X = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                    }

                                                    obj.Series = series;                                                    
                                                    axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                    axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                    axis.InteractiveCursorLabelTopPosition = 0d;                                                    
                                                    this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));
                                                }

                                            }
                                        }
                                        foreach (ChartAxis axis in area.Axes)
                                        {
                                            if (axis.InteractiveCursorLabelVisibility == Visibility.Visible)
                                                axis.InteractiveCursorLabelContent = obj;
                                        }
                                    }
                                }

                            }
                        }

                    }
                #endregion
                }
                else
                {
                    BindWithMouseMove(e);
                }

                isDifferentArea = false;

            }
            else
            {
                Point pt2 = e.GetPosition(this);
                //Fix for RangeIndicator break in 10.2 version
                if (!this.IsBindWithSegment && IsBindWithMouseMove && !chartarea.EnableRangeSelection && pt2.X!=0 && pt2.Y!=0)
                {
                    BindWithMouseMove(e);
                }
                if (this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true)
                {
                    if (chartarea != null && chartarea.ChartAreaParent != null && chartarea.ChartAreaParent is SyncChartAreas
                        && (chartarea.ChartAreaParent as SyncChartAreas).selectedChartSeriesForIC != null && (chartarea.ChartAreaParent as SyncChartAreas).selectedChartSeriesForIC != null)
                    {
                        if (chartarea != (chartarea.ChartAreaParent as SyncChartAreas).selectedChartAreaForIC)
                        {
                            chartarea = (chartarea.ChartAreaParent as SyncChartAreas).selectedChartAreaForIC;
                            Selectedseries = (chartarea.ChartAreaParent as SyncChartAreas).selectedChartSeriesForIC;

                            isDifferentArea = true;
                        }
                        this.MoveOnMouseMove = true;
                    }
                    else
                    {
                        isDifferentArea = false;
                    }
                }
                if (this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                {
                    if (this.area.SyncChartArea == null)
                    {
                        #region ChartArea
                        if (this.Selectedseries != null)
                        {
                            if (this.HorizontalCursorLabelContent != null)
                            {
                                if (this.IsInversedLabel == true)
                                {
                                    if (LabelPositionValue != ((double)HorizontalCursorLabelContent))
                                    {
                                        this.LabelPositionValue = (double)HorizontalCursorLabelContent;
                                        this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length) * 8;
                                    }
                                }
                                else
                                    this.LabelPosition = this.ActualWidth;
                            }
                            List<ChartIndexedDataPoint> datasource = new List<ChartIndexedDataPoint>();

                            List<IChartDataPoint> stackSeries = new List<IChartDataPoint>();
                            int count = 0;

                            foreach (ChartSeries ser in this.area.Series)
                            {
                                if (ser != series)
                                {
                                    if (ser.Type == ChartTypes.StackingColumn || ser.Type == ChartTypes.FastStackingColumn)
                                    {
                                        foreach (ChartSegment seg in ser.Segments)
                                        {
                                            stackSeries.Add(seg.CorrespondingPoints[0].DataPoint);
                                        }
                                    }
                                }


                            }

                            if (series.Type == ChartTypes.FastHiLoOpenClose || series.Type == ChartTypes.Area || series.Type == ChartTypes.FastColumn || series.Type == ChartTypes.FastLine || 
                                series.Type == ChartTypes.FastScatter || series.Type == ChartTypes.FastStackingColumn || series.Type == ChartTypes.HiLoArea || series.Type == ChartTypes.SplineArea || series.Type == ChartTypes.StackingArea ||
                                series.Type == ChartTypes.StackingLine || series.Type == ChartTypes.StackingSpline || series.Type == ChartTypes.StackingSplineArea || series.Type == ChartTypes.StepArea || series.Type == ChartTypes.RangeArea)
                            {
                                if (series.Segments.Count > 0)
                                {
                                    foreach (ChartIndexedDataPoint cs in series.Segments[0].CorrespondingPoints)
                                    {
                                        datasource.Add(cs);
                                    }
                                }
                            }
                            else
                            {
                                if (this.Selectedseries.Segments == null)
                                    this.Selectedseries = this.area.Series[0];

                                if (this.Selectedseries != null)
                                {
                                    foreach (ChartSegment cs in this.Selectedseries.Segments)
                                    {
                                        datasource.Add(cs.CorrespondingPoints[0]);
                                        count++;
                                        if (count == series.Segments.Count && (series.Type == ChartTypes.Line || series.Type == ChartTypes.Spline ||
                                                        series.Type == ChartTypes.StepLine || series.Type == ChartTypes.ThreeLineBreak || series.Type == ChartTypes.RotatedSpline))
                                        {
                                            datasource.Add(cs.CorrespondingPoints[1]);
                                        }
                                    }
                                }
                            }

                            Point mousePoint = e.GetPosition(this.chartarea);
                            double valx = this.chartarea.PointToValue(this.chartarea.PrimaryAxis, mousePoint);
                            double valy = this.chartarea.PointToValue(this.chartarea.SecondaryAxis, mousePoint);
                            //Find the nearest data less than the point
                            var startVal = (from sortedItem in
                                                ((from data in datasource
                                                  where data.DataPoint.X <= valx
                                                  select data).ToList<ChartIndexedDataPoint>())
                                            orderby sortedItem.DataPoint.X
                                            select sortedItem).ToList<ChartIndexedDataPoint>();

                            //Find the nearest data greater than the point
                            var endVal = (from sortedItem in
                                              ((from data in datasource
                                                where data.DataPoint.X >= valx
                                                select data).ToList<ChartIndexedDataPoint>())
                                          orderby sortedItem.DataPoint.X
                                          select sortedItem).ToList<ChartIndexedDataPoint>();
                            //Point pointToDisplay = new Point();
                            if (series.Type == ChartTypes.Bar || series.Type == ChartTypes.BoxAndWhisker || series.Type == ChartTypes.Bubble || series.Type == ChartTypes.Candle || series.Type == ChartTypes.Column || series.Type == ChartTypes.FastColumn || series.Type == ChartTypes.FastScatter || series.Type == ChartTypes.FastStackingColumn || series.Type == ChartTypes.FastHiLoOpenClose || series.Type == ChartTypes.Gantt || series.Type == ChartTypes.HiLo || series.Type == ChartTypes.HiLoOpenClose || 
                                series.Type == ChartTypes.Histogram || series.Type == ChartTypes.Kagi || series.Type == ChartTypes.PointAndFigure || series.Type == ChartTypes.RangeColumn || series.Type == ChartTypes.Renko || series.Type == ChartTypes.RotatedSpline || series.Type == ChartTypes.Scatter || series.Type == ChartTypes.Spline || series.Type == ChartTypes.SplineArea || series.Type == ChartTypes.StackingArea ||
                                series.Type == ChartTypes.StackingLine || series.Type == ChartTypes.StackingLine100 || series.Type == ChartTypes.StackingSpline || series.Type == ChartTypes.StackingSpline100 || series.Type == ChartTypes.StackingSplineArea || series.Type == ChartTypes.StackingSplineArea100 || series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.StackingColumn || series.Type == ChartTypes.StackingColumn100 || series.Type == ChartTypes.StepArea || series.Type == ChartTypes.ThreeLineBreak || series.Type == ChartTypes.Tornado)
                            {
                                if (startVal.Count > 0 && endVal.Count > 0)
                                {
                                    if (((startVal[startVal.Count - 1].DataPoint.X - valx) * (startVal[startVal.Count - 1].DataPoint.X - valx)) >
                                        ((endVal[0].DataPoint.X - valx) * (endVal[0].DataPoint.X - valx)))
                                    {
                                        if (flag == false)
                                        {
                                            this.XValue = endVal[0].DataPoint.X;
                                            this.YValue = endVal[0].DataPoint.Y;

                                        }
                                        else
                                        {
                                            foreach (var item in stackSeries)
                                            {
                                                if (item.X == endVal[0].DataPoint.X)
                                                {
                                                    this.XValue = endVal[0].DataPoint.X;
                                                    this.YValue = endVal[0].DataPoint.Y + item.Y;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (flag == false)
                                        {
                                            this.XValue = startVal[startVal.Count - 1].DataPoint.X;
                                            this.YValue = startVal[startVal.Count - 1].DataPoint.Y;

                                        }
                                        else
                                        {
                                            foreach (var item in stackSeries)
                                            {
                                                if (item.X == startVal[startVal.Count - 1].DataPoint.X)
                                                {
                                                    this.XValue = startVal[startVal.Count - 1].DataPoint.X;
                                                    this.YValue = startVal[startVal.Count - 1].DataPoint.Y + item.Y;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                double yVal = 0;
                                if (startVal.Count > 0 && endVal.Count > 0)
                                {
                                    //convert the data as point
                                    Point point1 = new Point(startVal[startVal.Count - 1].DataPoint.X, startVal[startVal.Count - 1].DataPoint.Y);
                                    Point point2 = new Point(endVal[0].DataPoint.X, endVal[0].DataPoint.Y);
                                    //Find the Secondary axis data point
                                    yVal = FindYValue(point1, point2, valx);
                                    //pointToDisplay = new Point(this.chartarea.ValueToPoint(this.chartarea.PrimaryAxis, valx), 
                                    //    this.chartarea.ValueToPoint(this.chartarea.SecondaryAxis, yVal));
                                    this.XValue = valx;
                                    this.YValue = yVal;
                                }
                            }


                            SetValueForInteractiveCursor(true);


                            if (area.SyncChartArea == null)
                            {
                                InteractiveCursorLabelContent obj = new InteractiveCursorLabelContent();
                                foreach (ChartAxis axis in area.Axes)
                                {
                                    axis.IsOpen = true;
                                    if (axis.Orientation == Orientation.Horizontal)
                                    {
                                        axis.IsOpen = true;
                                        #region HorizontalCursorLabelContent
                                        if (axis.InteractiveCursorContentVisibility == true )
                                        {
                                            axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility== Visibility.Visible?Visibility.Visible:Visibility.Collapsed;

                                            if ((series.Type == ChartTypes.StackingBar) || (series.Type == ChartTypes.Bar) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Gantt))
                                                obj.Y = Convert.ToDouble(this.HorizontalCursorLabelContent);
                                            else
                                                obj.Y = Convert.ToDouble(this.VerticalCursorLabelContent);
                                            if (axis.ValueType == ChartValueType.DateTime)
                                            {
                                                if ((series.Type == ChartTypes.StackingBar) || (series.Type == ChartTypes.Bar) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Gantt))
                                                {
                                                    DateTime dt = DateTime.ParseExact(this.VerticalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                                    obj.X = dt;
                                                   
                                                }
                                                else
                                                {
                                                    DateTime dt = DateTime.ParseExact(this.HorizontalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                                    obj.X = dt;
                                                   this.DataPoint = new ChartPoint(DateTime.Parse(this.HorizontalCursorLabelContent.ToString()).ToOADate(), (double)obj.Y);
                                                }
                                            }
                                            else
                                            {
                                                if ((series.Type == ChartTypes.StackingBar) || (series.Type == ChartTypes.Bar) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Gantt))
                                                {
                                                    obj.Y = Convert.ToDouble(this.HorizontalCursorLabelContent);
                                                }
                                                else
                                                {
                                                    obj.X = Convert.ToDouble(this.HorizontalCursorLabelContent);
                                                    this.DataPoint = new ChartPoint(Convert.ToDouble(this.HorizontalCursorLabelContent), (double)obj.Y);
                                                }
                                            }
                                            //obj.Y =Convert.ToDouble(this.HorizontalCursorLabelContent);
                                            obj.Series = series;                                                                                                                          
                                            obj.DataPoint = this.DataPoint;
                                            axis.InteractiveCursorLabelContent = obj;
                                            axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                            axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                            axis.InteractiveCursorLabelTopPosition = 0d;
                                            this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));
                                        }

                                        #endregion
                                    }
                                    if (axis.Orientation == Orientation.Vertical)
                                    {
                                        #region VerticalCursorLabelContent
                                        if (axis.InteractiveCursorContentVisibility == true)
                                        {
                                            double value = this.area.PointToValue(axis, axiscursorLabelValue);
                                            axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;

                                            if (axis.ValueType == ChartValueType.DateTime)
                                            {
                                                if ((series.Type == ChartTypes.StackingBar) || (series.Type == ChartTypes.Bar) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Gantt))
                                                {
                                                    DateTime dt = DateTime.ParseExact(this.VerticalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                                    obj.X = dt;
                                                    this.DataPoint = new ChartPoint(DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate(), (double)obj.Y);
                                                    
                                                }
                                                else
                                                {
                                                    DateTime dt = DateTime.ParseExact(this.VerticalCursorLabelContent.ToString(), axis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                                    obj.Y = dt;// DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                                }
                                            }
                                            else
                                            {
                                                if ((series.Type == ChartTypes.StackingBar) || (series.Type == ChartTypes.Bar) || (series.Type == ChartTypes.StackingBar100) || (series.Type == ChartTypes.Gantt))
                                                {
                                                    obj.X = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                    //this.DataPoint = new ChartPoint(DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate(), (double)obj.Y);
                                                }
                                                else
                                                {
                                                    obj.Y = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                }
                                            }
                                            obj.Series = series;                                            
                                            axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                            axis.InteractiveCursorLabelTopPosition = OffsetY;
                                            axis.InteractiveCursorLabelLeftPosition = 0d;
                                        }
                                        
                                        #endregion
                                    }
                                }
                                foreach (ChartAxis axis in area.Axes)
                                {
                                    if (axis.InteractiveCursorContentVisibility)
                                    {
                                        axis.InteractiveCursorLabelContent = obj;
                                    }
                                }
                            }

                        }
                        #endregion
                    }
                    else
                    {
                        #region SyncChartArea
                        if (this.area.SyncChartArea.CursorSeries != null)
                        {
                            if (this.HorizontalCursorLabelContent != null)
                            {
                                if (this.IsInversedLabel == true)
                                {
                                    if (LabelPositionValue != ((double)HorizontalCursorLabelContent))
                                    {
                                        this.LabelPositionValue = (double)HorizontalCursorLabelContent;
                                        this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length) * 8;
                                    }
                                }
                                else
                                    this.LabelPosition = this.ActualWidth;
                            }
                            List<ChartIndexedDataPoint> datasource = new List<ChartIndexedDataPoint>();
                            int count = 0;

                            List<IChartDataPoint> stackSeries = new List<IChartDataPoint>();
                            foreach (ChartSeries ser in this.area.Series)
                            {
                                if (ser != this.area.SyncChartArea.CursorSeries)
                                {
                                    if (ser.Type == ChartTypes.StackingColumn)
                                    {
                                        foreach (ChartSegment seg in ser.Segments)
                                        {
                                            stackSeries.Add(seg.CorrespondingPoints[0].DataPoint);
                                        }
                                    }
                                    if (ser.Type == ChartTypes.FastStackingColumn)
                                    {
                                        foreach (ChartIndexedDataPoint cs in ser.Segments[0].CorrespondingPoints)
                                        {
                                            stackSeries.Add(cs.DataPoint);
                                        }
                                    }
                                }
                            }
                            int i = 0;
                            foreach (ChartSeries ser in this.area.Series)
                            {
                                if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastStackingColumn || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingColumn)
                                {
                                    if (ser == this.area.SyncChartArea.CursorSeries)
                                    {
                                        if (i > 0)
                                        {
                                            flag = true;
                                            break;
                                        }
                                        else
                                        {
                                            flag = false;
                                            break;
                                        }

                                    }

                                    i++;
                                }
                                else
                                {
                                    flag = false;
                                }

                            }
                            if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastHiLoOpenClose || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Area || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastColumn || 
                                this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastLine || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastScatter || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.HiLoArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.SplineArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingArea ||
                                this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingLine || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSpline || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSplineArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StepArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RangeArea)
                            {
                                foreach (ChartIndexedDataPoint cs in this.area.SyncChartArea.CursorSeries.Segments[0].CorrespondingPoints)
                                {
                                    datasource.Add(cs);
                                }
                            }
                            else
                            {
                                if (this.area.SyncChartArea.CursorSeries.Type != ChartTypes.FastStackingColumn)
                                {
                                    foreach (ChartSegment cs in this.area.SyncChartArea.CursorSeries.Segments)
                                    {
                                        datasource.Add(cs.CorrespondingPoints[0]);
                                        count++;
                                        if (count == this.area.SyncChartArea.CursorSeries.Segments.Count && (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Line || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Spline ||
                                                        this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StepLine || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.ThreeLineBreak || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RotatedSpline))
                                        {
                                            datasource.Add(cs.CorrespondingPoints[1]);
                                        }
                                    }
                                }
                                else if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastStackingColumn)
                                {
                                    foreach (ChartIndexedDataPoint cs in this.area.SyncChartArea.CursorSeries.Segments[0].CorrespondingPoints)
                                    {
                                        datasource.Add(cs);
                                    }
                                }
                            }

                            Point mousePoint = e.GetPosition(this.chartarea);
                            double valx = this.chartarea.PointToValue(this.chartarea.PrimaryAxis, mousePoint);
                            double valy = this.chartarea.PointToValue(this.chartarea.SecondaryAxis, mousePoint);
                            //Find the nearest data less than the point
                            var startVal = (from sortedItem in
                                                ((from data in datasource
                                                  where data.DataPoint.X <= valx
                                                  select data).ToList<ChartIndexedDataPoint>())
                                            orderby sortedItem.DataPoint.X
                                            select sortedItem).ToList<ChartIndexedDataPoint>();

                            //Find the nearest data greater than the point
                            var endVal = (from sortedItem in
                                              ((from data in datasource
                                                where data.DataPoint.X >= valx
                                                select data).ToList<ChartIndexedDataPoint>())
                                          orderby sortedItem.DataPoint.X
                                          select sortedItem).ToList<ChartIndexedDataPoint>();
                            //Point pointToDisplay = new Point();
                            if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Bar || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.BoxAndWhisker || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Bubble || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Candle || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Column || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastColumn || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastScatter || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastStackingColumn || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastHiLoOpenClose || 
                                this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Gantt || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.HiLo || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.HiLoOpenClose || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Histogram || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Kagi || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.PointAndFigure || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RangeColumn || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Renko || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RotatedSpline || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Scatter || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Spline || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.SplineArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingArea ||
                                this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingLine || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingLine100 || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSpline || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSpline100 || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSplineArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSplineArea100 || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingBar || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingBar100 || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingColumn || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingColumn100 || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StepArea || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.ThreeLineBreak || this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Tornado)
                            {
                                if (startVal.Count > 0 && endVal.Count > 0)
                                {
                                    if (((startVal[startVal.Count - 1].DataPoint.X - valx) * (startVal[startVal.Count - 1].DataPoint.X - valx)) >
                                         ((endVal[0].DataPoint.X - valx) * (endVal[0].DataPoint.X - valx)))
                                    {
                                        if (flag == false)
                                        {
                                            if (endVal[0].DataPoint.EmptyPoint != true && endVal[0].DataPoint.Y != 0)
                                            {
                                                this.XValue = endVal[0].DataPoint.X;
                                                this.YValue = endVal[0].DataPoint.Y;
                                            }

                                        }
                                        else
                                        {
                                            foreach (var item in stackSeries)
                                            {
                                                if (item.X == endVal[0].DataPoint.X)
                                                {
                                                    this.XValue = endVal[0].DataPoint.X;
                                                    this.YValue = endVal[0].DataPoint.Y + item.Y;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (flag == false)
                                        {
                                            if (startVal[startVal.Count - 1].DataPoint.EmptyPoint != true && startVal[startVal.Count - 1].DataPoint.Y != 0)
                                            {
                                                this.XValue = startVal[startVal.Count - 1].DataPoint.X;
                                                this.YValue = startVal[startVal.Count - 1].DataPoint.Y;
                                            }

                                        }
                                        else
                                        {
                                            foreach (var item in stackSeries)
                                            {
                                                if (item.X == startVal[startVal.Count - 1].DataPoint.X)
                                                {
                                                    this.XValue = startVal[startVal.Count - 1].DataPoint.X;
                                                    this.YValue = startVal[startVal.Count - 1].DataPoint.Y + item.Y;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                double yVal = 0;
                                if (startVal.Count > 0 && endVal.Count > 0)
                                {
                                    //convert the data as point
                                    Point point1 = new Point(startVal[startVal.Count - 1].DataPoint.X, startVal[startVal.Count - 1].DataPoint.Y);
                                    Point point2 = new Point(endVal[0].DataPoint.X, endVal[0].DataPoint.Y);
                                    //Find the Secondary axis data point
                                    yVal = FindYValue(point1, point2, valx);
                                    //pointToDisplay = new Point(this.chartarea.ValueToPoint(this.chartarea.PrimaryAxis, valx), 
                                    //    this.chartarea.ValueToPoint(this.chartarea.SecondaryAxis, yVal));
                                    this.XValue = valx;
                                    this.YValue = yVal;
                                }
                            }


                            SetValueForInteractiveCursor(true);

                            SyncChartAreas Sarea = this.area.SyncChartArea as SyncChartAreas;
                            if (Sarea != null)
                            {
                                if (Sarea.InteractiveCursors.Count > 0)
                                {
                                    InteractiveCursor syncic = Sarea.InteractiveCursors[this.CollectionIndex];
                                    InteractiveCursorLabelContent obj = new InteractiveCursorLabelContent();

                                    foreach (ChartArea area1 in Sarea.Areas)
                                    {
                                        if (area1 == this.area)
                                        {
                                            foreach (ChartAxis axis in area1.Axes)
                                            {
                                                axis.IsOpen = true;
                                                if (axis.InteractiveCursorContentVisibility)
                                                {
                                                    if (axis.Orientation == Orientation.Horizontal)
                                                    {
                                                        axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;
                                                        if (area1.index == Sarea.Areas.Count - 1)
                                                            axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                                        else
                                                        {
                                                            Sarea.Areas[Sarea.Areas.Count - 1].PrimaryAxis.InteractiveCursorLabelVisibility = System.Windows.Visibility.Visible;
                                                            axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;
                                                        }


                                                        if (axis.ValueType == ChartValueType.DateTime)
                                                        {
                                                            double content = DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                                            obj.X = DateTime.FromOADate((double)content);
                                                        }
                                                        else
                                                        {
                                                            obj.X = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                        }

                                                        // obj.X = Math.Round(Convert.ToDouble(this.VerticalCursorLabelContent));
                                                        obj.Series = this.area.SyncChartArea.CursorSeries;                                                        
                                                        axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                        axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                        axis.InteractiveCursorLabelTopPosition = 0d;
                                                        this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));


                                                    }
                                                    if (axis.Orientation == Orientation.Vertical)
                                                    {

                                                        double value = this.area.PointToValue(axis, axiscursorLabelValue);
                                                        axis.InteractiveCursorLabelVisibility =this.VerticalLabelVisibility==Visibility.Visible?Visibility.Visible:Visibility.Collapsed;

                                                        obj.Y = value;
                                                        obj.Series = this.area.SyncChartArea.CursorSeries;                                                        
                                                        axis.InteractiveCursorMargin = new Thickness(0, OffsetY, 0, 0);
                                                        axis.InteractiveCursorLabelTopPosition = OffsetY;
                                                        axis.InteractiveCursorLabelContent = obj;
                                                        axis.InteractiveCursorLabelLeftPosition = 0d;

                                                    }
                                                }
                                            }
                                            foreach (ChartAxis axis in area.Axes)
                                            {
                                                if (axis.InteractiveCursorContentVisibility && axis.Orientation==Orientation.Horizontal)
                                                    axis.InteractiveCursorLabelContent = obj;
                                            }
                                        }
                                        else
                                        {
                                            foreach (ChartAxis axis in area1.Axes)
                                            {
                                                axis.IsOpen = true;
                                                if (axis.InteractiveCursorContentVisibility)
                                                {
                                                    if (axis.Orientation == Orientation.Vertical)
                                                    {
                                                        axis.InteractiveCursorLabelVisibility = Visibility.Hidden;
                                                    }
                                                    else
                                                    {
                                                        axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;

                                                        if (area1.index == area.SyncChartArea.Areas.Count - 1)
                                                            axis.InteractiveCursorLabelVisibility =this.HorizontalLabelVisibility== Visibility.Visible?Visibility.Visible:Visibility.Collapsed;
                                                        else
                                                        {
                                                            area.SyncChartArea.Areas[Sarea.Areas.Count - 1].PrimaryAxis.InteractiveCursorLabelVisibility = this.HorizontalLabelVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                                                            axis.InteractiveCursorLabelVisibility = Visibility.Collapsed;
                                                        }


                                                        if (axis.ValueType == ChartValueType.DateTime)
                                                        {
                                                            double content = DateTime.Parse(this.VerticalCursorLabelContent.ToString()).ToOADate();
                                                            obj.X = DateTime.FromOADate((double)content);
                                                        }
                                                        else
                                                        {
                                                            obj.X = Convert.ToDouble(this.VerticalCursorLabelContent);
                                                        }
                                                        obj.Series = this.area.SyncChartArea.CursorSeries;                                                       
                                                        axis.InteractiveCursorMargin = new Thickness(OffsetX, 0, 0, 0);
                                                        axis.InteractiveCursorLabelLeftPosition = this.ActualWidth - OffsetX;
                                                        axis.InteractiveCursorLabelTopPosition = 0d;
                                                        this.OnLocationChanged(new CursorLocationChangedeventArgs(axis.Area, this, axis));
                                                    }

                                                }
                                            }
                                            foreach (ChartAxis axis in area1.Axes)
                                            {
                                                if (axis.InteractiveCursorLabelVisibility == Visibility.Visible && axis.Orientation==Orientation.Horizontal)
                                                    axis.InteractiveCursorLabelContent = obj;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        #endregion
                    }
                }
            }
            if (this.area.SyncChartArea != null)
            {
                foreach (ChartArea item in this.area.SyncChartArea.Areas)
                {
                    item.InteractiveCursors[0].LeftPosition = this.OffsetX - (this.CursorSymbolWidth / 2);
                    item.InteractiveCursors[0].TopPosition = this.OffsetY - (this.CursorSymbolHeight / 2);
                }
            }
            else
            {
                SetValueForInteractiveCursorSymbol();
            }
            if (IsBindWithMouseMove && IsBindWithSegment == false && BindWithMouseMoveOnSegment == false || (BindWithMouseMoveOnSegment && IsBindWithMouseMove == false && IsBindWithSegment == false))
            {
                Point point = e.GetPosition(this.chartarea);
                double valx = this.chartarea.PointToValue(this.chartarea.PrimaryAxis, point);
                double valy = this.chartarea.PointToValue(this.chartarea.SecondaryAxis, point);
                this.DataPoint = new ChartPoint(valx,valy);      
            }
            foreach (ChartAxis _axis in this.chartarea.Axes)
            {
                if (chartarea.EnableRangeSelection)
                {
                    _axis.InteractiveCursorLabelContent = new InteractiveCursorLabelContent() { DataPoint = this.DataPoint};
                }
                else if (_axis != null && _axis.InteractiveCursorLabelContent != null && _axis.InteractiveCursorLabelContent.DataPoint == null)
                {
                    _axis.InteractiveCursorLabelContent.DataPoint = this.DataPoint;
                }
            }
        }

        /// <summary>
        /// To find the corresponding Y Value
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="valX"></param>
        /// <returns></returns>
        internal double FindYValue(Point point1, Point point2, double valX)
        {
            double yVal = 0;

            double m = (point2.Y - point1.Y) / (point2.X - point1.X); // slop = y2-y1/x2-x1

            double b = point2.Y - (m * point2.X);  //y - y1 = m (x-x1)  y = mx +b 

            yVal = (m * valX) + b;

            return yVal;

        }
        internal Point axiscursorLabelValue = new Point();
        internal ChartArea HorizontalCursorArea = null;
        internal void SetValueForInteractiveCursor(bool flag)
        {
            if (IsBindWithSegment && BindWithMouseMoveOnSegment == false)
            {

                if (flag == false)
                    if (this.HorizontalCursorVisibility == Visibility.Collapsed)
                        return;

                if (this.Selectedseries != null)
                {
                    #region BarType
                    if (this.Selectedseries.Type == ChartTypes.Bar ||
                        this.Selectedseries.Type == ChartTypes.RotatedSpline ||
                        this.Selectedseries.Type == ChartTypes.StackingBar ||
                        this.Selectedseries.Type == ChartTypes.StackingBar100 ||
                        this.Selectedseries.Type == ChartTypes.Tornado ||
                        this.Selectedseries.Type == ChartTypes.Gantt)
                    {
                        double pend = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);
                        double sstart = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);

                        if (this.Selectedseries.XAxis.IsInversed == true)
                            pend = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);

                        if (this.Selectedseries.YAxis.IsInversed == true)
                            sstart = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                        //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue) - pend;
                        //this.OffsetY = this.HorizontalCursorLabelPosition;
                        if (this.EnableHorizontalMove == true)
                        {
                            this.OffsetY = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.XValue) - pend;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                            if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                this.VerticalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else
                                this.VerticalCursorLabelContent = this.XValue;
                            double value = 0d;
                            double value1 = 0d;
                            if (VerticalCursorLabelContent != null)
                            {
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                    value1 = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value1 = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                }
                            }
                            if (HorizontalCursorLabelContent != null)
                            {                                    
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                    value = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value= this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                }
                            }
                            axiscursorLabelValue = new Point(value1, value);
                            this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                        }

                        //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue);
                        //this.OffsetX = this.VerticalCursorLabelPosition - sstart;
                        if (this.EnableVerticalMove == true)
                        {
                            this.OffsetX = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.YValue) - sstart;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                            if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)                                    
                                this.HorizontalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else
                                this.HorizontalCursorLabelContent = this.YValue;

                        }
                    }
                        #endregion

                        #region AreaType
                    else if (this.Selectedseries.Type == ChartTypes.Area ||
                             this.Selectedseries.Type == ChartTypes.FastColumn ||
                             this.Selectedseries.Type == ChartTypes.FastLine ||
                             this.Selectedseries.Type == ChartTypes.FastScatter ||
                             this.Selectedseries.Type == ChartTypes.FastStackingColumn ||
                             this.Selectedseries.Type == ChartTypes.HiLoArea ||
                             this.Selectedseries.Type == ChartTypes.SplineArea ||
                             this.Selectedseries.Type == ChartTypes.StackingArea ||
                             this.Selectedseries.Type == ChartTypes.StackingLine ||
                             this.Selectedseries.Type == ChartTypes.StackingSpline ||
                             this.Selectedseries.Type == ChartTypes.StackingSplineArea ||
                             this.Selectedseries.Type == ChartTypes.StepArea ||
                             this.Selectedseries.Type == ChartTypes.RangeArea)
                    {
                        double pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);
                        double send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                        if (this.Selectedseries.XAxis.IsInversed == true)
                            pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);

                        if (this.Selectedseries.YAxis.IsInversed == true)
                            send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);
                            
                        if (this.EnableHorizontalMove == true)
                        {
                            if (this.YValue - send > 0)
                                this.OffsetY = this.YValue - send ;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                            if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                this.HorizontalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else
                                this.HorizontalCursorLabelContent = this.XValue;
                            double value = 0d;

                            double value1 = 0d;
                            if (VerticalCursorLabelContent != null)
                            {
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                    value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                }
                            }
                            if (HorizontalCursorLabelContent != null)
                            {
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                    value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                }
                            }
                            axiscursorLabelValue = new Point(value, value1);
                            this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                        }

                        //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue);// -AxesThickness.Left;
                        //this.OffsetX = this.VerticalCursorLabelPosition - pstart;// +AxesThickness.Left;
                        if (this.EnableVerticalMove == true)
                        {
                            if (this.XValue - pstart > 0)
                                this.OffsetX = this.XValue - pstart ;//> 0 ? this.XValue - pstart : pstart;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                            if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                this.VerticalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else
                            {
                                this.VerticalCursorLabelContent = this.YValue;
                            }
                        }

                    }
                        #endregion

                        #region LineType
                    else
                    {
                        double pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);
                        double send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                        if (this.Selectedseries.XAxis.IsInversed == true)
                            pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);

                        if (this.Selectedseries.YAxis.IsInversed == true)
                            send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);

                        //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue) - send;
                        //this.OffsetY = this.HorizontalCursorLabelPosition;
                        if (this.EnableHorizontalMove == true)
                        {
                            this.OffsetY = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.YValue) - send;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }                                
                            if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                this.HorizontalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else
                                this.HorizontalCursorLabelContent = this.XValue;                               
                            double value = 0d;
                            double value1 = 0d;
                            if (VerticalCursorLabelContent != null)
                            {
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                    value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                }
                            }
                            if (HorizontalCursorLabelContent != null)
                            {
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                {
                                    double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                    value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(doublevalue));
                                }
                                else
                                {
                                    value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                }
                            }
                            axiscursorLabelValue = new Point(value, value1);
                            this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                        }

                        //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue);// -AxesThickness.Left;
                        //this.OffsetX = this.VerticalCursorLabelPosition - pstart;// +AxesThickness.Left;
                        if (this.EnableVerticalMove == true)
                        {
                            this.OffsetX = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.XValue) - pstart;
                            if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                            { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                            if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                this.VerticalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                            else                                
                                this.VerticalCursorLabelContent = this.YValue;                                
                        }
                    }
                    #endregion

                    if (this.chartarea.SyncChartArea != null)
                    {
                        if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                        {
                            InteractiveCursor syncic = this.chartarea.SyncChartArea.InteractiveCursors[this.CollectionIndex];
                            foreach (InteractiveCursor ic in syncic.interactivecursor)
                            {
                                if (flag)
                                    ic.MoveOnMouseMove = false;
                                ic.HorizontalCursorVisibility = Visibility.Collapsed;
                            }
                            if (flag)
                                this.MoveOnMouseMove = true;
                            this.HorizontalCursorVisibility = Visibility.Visible;


                            //if (syncic.LabelVisibility == Visibility.Visible)
                            //{
                            //    foreach (InteractiveCursor ic in syncic.interactivecursor)
                            //    {
                            //        ic.HorizontalLabelVisibility = Visibility.Collapsed;

                            //    }
                            //    if (isDifferentArea == false)
                            //    {
                            //        this.HorizontalLabelVisibility = Visibility.Visible;
                            //    }
                            //    else
                            //    {
                            //        this.HorizontalLabelVisibility = Visibility.Collapsed;
                            //    }
                            //}

                            if (isDifferentArea == true)
                            {
                                //  chartarea.InteractiveCursors[this.CollectionIndex].HorizontalCursorVisibility = Visibility.Visible;
                                //  chartarea.InteractiveCursors[this.CollectionIndex].HorizontalLabelVisibility = Visibility.Visible;
                                // (chartarea.ChartAreaParent as SyncChartAreas).Areas[this.areaIndex].InteractiveCursors[this.CollectionIndex].HorizontalCursorVisibility = Visibility.Collapsed;
                                // (chartarea.ChartAreaParent as SyncChartAreas).Areas[this.areaIndex].InteractiveCursors[this.CollectionIndex].HorizontalLabelVisibility = Visibility.Collapsed;
                            }
                        }
                    }
                    else
                    {
                        if (flag)
                            this.MoveOnMouseMove = true;
                    }

                    if (this.HorizontalCursorLabelContent != null)
                    {

                        if (this.IsInversedLabel == true)

                            this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length - 1) * 8;
                        else
                            this.LabelPosition = this.ActualWidth;
                    }
                }

            }
            else if (IsBindWithSegment && BindWithMouseMoveOnSegment == true)
            {
                if (area.SyncChartArea == null)
                {
                    #region ChartArea
                    if (flag == false)
                        if (this.HorizontalCursorVisibility == Visibility.Collapsed)
                            return;

                    if (this.Selectedseries != null)
                    {
                        #region BarType
                        if (this.Selectedseries.Type == ChartTypes.Bar ||
                            this.Selectedseries.Type == ChartTypes.RotatedSpline ||
                            this.Selectedseries.Type == ChartTypes.StackingBar ||
                            this.Selectedseries.Type == ChartTypes.StackingBar100 ||
                            this.Selectedseries.Type == ChartTypes.Tornado ||
                            this.Selectedseries.Type == ChartTypes.Gantt)
                        {
                            double pend = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);
                            double sstart = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);

                            if (this.Selectedseries.XAxis.IsInversed == true)
                                pend = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);

                            if (this.Selectedseries.YAxis.IsInversed == true)
                                sstart = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue) - pend;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.XValue) - pend;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.VerticalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.VerticalCursorLabelContent = this.XValue;
                                double value2 = 0d,value=0;
                                if (HorizontalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                        value2 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                        value = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                        value2 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                    }
                                }

                                    
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        if (VerticalCursorLabelContent != null)
                                        {
                                            double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                            value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                        }
                                        else
                                            value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, 0);
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                axiscursorLabelValue = new Point(value, value1);
                            }

                            //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue);
                            //this.OffsetX = this.VerticalCursorLabelPosition - sstart;
                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.YValue) - sstart;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    this.HorizontalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.HorizontalCursorLabelContent = this.YValue;
                            }

                        }
                            #endregion

                            #region AreaType
                        else if (this.Selectedseries.Type == ChartTypes.Area ||
                                 this.Selectedseries.Type == ChartTypes.FastColumn ||
                                 this.Selectedseries.Type == ChartTypes.FastLine ||
                                 this.Selectedseries.Type == ChartTypes.FastScatter ||
                                 this.Selectedseries.Type == ChartTypes.FastStackingColumn ||
                                 this.Selectedseries.Type == ChartTypes.HiLoArea ||
                                 this.Selectedseries.Type == ChartTypes.SplineArea ||
                                 this.Selectedseries.Type == ChartTypes.StackingArea ||
                                 this.Selectedseries.Type == ChartTypes.StackingLine ||
                                 this.Selectedseries.Type == ChartTypes.StackingSpline ||
                                 this.Selectedseries.Type == ChartTypes.StackingSplineArea ||
                                 this.Selectedseries.Type == ChartTypes.StepArea ||
                                 this.Selectedseries.Type == ChartTypes.RangeArea)
                        {
                            double pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);
                            double send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                            if (this.Selectedseries.XAxis.IsInversed == true)
                                pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);

                            if (this.Selectedseries.YAxis.IsInversed == true)
                                send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue) - send;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.YValue) - send;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }                                    
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.HorizontalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.HorizontalCursorLabelContent = this.XValue;
                                double value = 0d;
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                if (HorizontalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                        value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                    }
                                }
                                this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                axiscursorLabelValue = new Point(value, value1);
                            }

                            //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue);// -AxesThickness.Left;
                            //this.OffsetX = this.VerticalCursorLabelPosition - pstart;// +AxesThickness.Left;
                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.XValue) - pstart;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    this.VerticalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.VerticalCursorLabelContent = this.YValue;   
                            }

                        }
                            #endregion

                            #region LineType
                        else
                        {
                            double pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.Start);
                            double send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.End);

                            if (this.Selectedseries.XAxis.IsInversed == true)
                                pstart = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.Selectedseries.XAxis.VisibleRange.End);

                            if (this.Selectedseries.YAxis.IsInversed == true)
                                send = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.Selectedseries.YAxis.VisibleRange.Start);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue) - send;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.Selectedseries.YAxis, this.YValue) - send;
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.HorizontalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString(Selectedseries.XAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.HorizontalCursorLabelContent = this.XValue;
                                double value = 0d;
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                if (HorizontalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(HorizontalCursorLabelContent.ToString()).ToOADate();
                                        value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value = this.area.ValueToPoint(this.Selectedseries.XAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                    }
                                }
                                //this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                //axiscursorLabelValue = new Point(value1, value);
                            }

                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.Selectedseries.XAxis, this.XValue) - pstart;
                                //if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                //{ this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.Selectedseries.YAxis.ValueType == ChartValueType.DateTime)
                                    this.VerticalCursorLabelContent = DateTime.FromOADate(this.YValue).ToString(Selectedseries.YAxis.LabelDateTimeFormat, CultureInfo.CurrentCulture);
                                else
                                    this.VerticalCursorLabelContent = this.YValue;   
                            }
                        }
                        #endregion

                        if (this.chartarea.SyncChartArea != null)
                        {
                            SyncChartAreas Sarea = this.area.SyncChartArea as SyncChartAreas;
                            if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                            {
                                InteractiveCursor syncic = this.series.Area.SyncChartArea.InteractiveCursors[this.CollectionIndex];
                                foreach (InteractiveCursor ic in syncic.interactivecursor)
                                {
                                    if (flag)
                                        ic.MoveOnMouseMove = false;
                                    ic.HorizontalCursorVisibility = Visibility.Collapsed;
                                }
                                if (flag)
                                    this.MoveOnMouseMove = true;

                                //this.HorizontalCursorVisibility = Visibility.Visible;


                                if (this.area.SyncChartArea.CursorSeries != null)
                                {
                                    if (this.area.SyncChartArea.CursorSeries.Area == cursrorArea)
                                    {
                                        foreach (InteractiveCursor ic in cursrorArea.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Visible;
                                        }
                                    }
                                    if (this.area.SyncChartArea.CursorSeries.Area != cursrorArea)
                                    {
                                        if(cursrorArea != null)
                                        foreach (InteractiveCursor ic in cursrorArea.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Collapsed;
                                        }

                                        foreach (InteractiveCursor ic in this.area.SyncChartArea.CursorSeries.Area.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Visible;
                                        }
                                    }
                                }

                                if (syncic.LabelVisibility == Visibility.Visible)
                                {
                                    foreach (InteractiveCursor ic in syncic.interactivecursor)
                                    {
                                        ic.HorizontalLabelVisibility = Visibility.Collapsed;
                                    }
                                    this.HorizontalLabelVisibility = Visibility.Visible;
                                }
                            }
                        }
                        else
                        {
                            if (flag)
                                this.MoveOnMouseMove = true;
                        }


                        if (this.HorizontalCursorLabelContent != null)
                        {

                            if (this.IsInversedLabel == true)

                                this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length - 1) * 8;
                            else
                                this.LabelPosition = this.ActualWidth;
                        }
                    }
                    #endregion

                }
                else
                {
                    #region SynChartArea
                    if (flag == false)
                        if (this.HorizontalCursorVisibility == Visibility.Collapsed)
                            return;

                    if (this.area.SyncChartArea.CursorSeries != null && this.Selectedseries != null)
                    {
                        #region BarType
                        if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Bar ||
                            this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RotatedSpline ||
                            this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingBar ||
                            this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingBar100 ||
                            this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Tornado ||
                            this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Gantt)
                        {
                            double pend = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.End);
                            double sstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.Start);

                            if (this.area.SyncChartArea.CursorSeries.XAxis.IsInversed == true)
                                pend = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.Start);

                            if (this.area.SyncChartArea.CursorSeries.YAxis.IsInversed == true)
                                sstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.End);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue) - pend;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.XValue) - pend;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                this.HorizontalCursorLabelContent = this.YValue;
                                double value2 = Convert.ToDouble(this.HorizontalCursorLabelContent);

                                double value = this.area.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                // double value1 = this.area.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                axiscursorLabelValue = new Point(value1, value);
                            }

                            //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue);
                            //this.OffsetX = this.VerticalCursorLabelPosition - sstart;
                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.YValue) - sstart;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.HorizontalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString();
                                else
                                    this.HorizontalCursorLabelContent = this.XValue;
                            }

                        }
                            #endregion

                            #region AreaType
                        else if (this.area.SyncChartArea.CursorSeries.Type == ChartTypes.Area ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastColumn ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastLine ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastScatter ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.FastStackingColumn ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.HiLoArea ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.SplineArea ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingArea ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingLine ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSpline ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StackingSplineArea ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.StepArea ||
                                 this.area.SyncChartArea.CursorSeries.Type == ChartTypes.RangeArea)
                        {
                            double pstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.Start);
                            double send = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.End);

                            if (this.Selectedseries.XAxis.IsInversed == true)
                                pstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.End);

                            if (this.Selectedseries.YAxis.IsInversed == true)
                                send = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.Start);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue) - send;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.YValue) - send;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                this.HorizontalCursorLabelContent = this.YValue;

                                double value = this.area.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                //  double value1 = this.area.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                axiscursorLabelValue = new Point(value1, value);
                            }

                            //this.VerticalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.XAxis, this.XValue);// -AxesThickness.Left;
                            //this.OffsetX = this.VerticalCursorLabelPosition - pstart;// +AxesThickness.Left;
                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.XValue) - pstart;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.area.SyncChartArea.CursorSeries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.VerticalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString();
                                else
                                    this.VerticalCursorLabelContent = this.XValue;
                                HorizontalCursorArea = this.area.SyncChartArea.CursorSeries.Area;
                            }

                        }
                            #endregion

                            #region LineType
                        else
                        {
                            double pstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.Start);
                            double send = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.End);

                            if (this.Selectedseries.XAxis.IsInversed == true)
                                pstart = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.area.SyncChartArea.CursorSeries.XAxis.VisibleRange.End);

                            if (this.Selectedseries.YAxis.IsInversed == true)
                                send = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.area.SyncChartArea.CursorSeries.YAxis.VisibleRange.Start);

                            //this.HorizontalCursorLabelPosition = this.chartarea.ValueToPoint(this.chartseries.YAxis, this.YValue) - send;
                            //this.OffsetY = this.HorizontalCursorLabelPosition;
                            if (this.EnableHorizontalMove == true)
                            {
                                this.OffsetY = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.YAxis, this.YValue) - send;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }

                                this.HorizontalCursorLabelContent = this.YValue;
                                double value2 = Convert.ToDouble(this.HorizontalCursorLabelContent);

                                //  this.area.SyncChartArea.CursorSeries.YAxis.InteractiveCursorLabelContent = obj;
                                double value = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.HorizontalCursorLabelContent));
                                //  double value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                double value1 = 0d;
                                if (VerticalCursorLabelContent != null)
                                {
                                    if (this.Selectedseries.XAxis.ValueType == ChartValueType.DateTime)
                                    {
                                        double doublevalue = DateTime.Parse(VerticalCursorLabelContent.ToString()).ToOADate();
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(doublevalue));
                                    }
                                    else
                                    {
                                        value1 = this.area.ValueToPoint(this.Selectedseries.YAxis, Convert.ToDouble(this.VerticalCursorLabelContent));
                                    }
                                }
                                this.OnLocationChanged(new CursorLocationChangedeventArgs(this.Selectedseries.XAxis.Area, this, this.Selectedseries.XAxis));
                                axiscursorLabelValue = new Point(value1, value);
                            }

                            if (this.EnableVerticalMove == true)
                            {
                                this.OffsetX = this.chartarea.ValueToPoint(this.area.SyncChartArea.CursorSeries.XAxis, this.XValue) - pstart;
                                if (this.SelectedCursor == false && this.IsBindWithSegment == true && this.BindWithMouseMoveOnSegment == true && this.MoveOnMouseMove == true)
                                { this.OffsetX -= 0.6; this.OffsetY -= 0.6; }
                                if (this.area.SyncChartArea.CursorSeries.XAxis.ValueType == ChartValueType.DateTime)
                                    this.VerticalCursorLabelContent = DateTime.FromOADate(this.XValue).ToString();
                                else
                                {
                                    this.VerticalCursorLabelContent = this.XValue;

                                }
                            }

                        }
                        #endregion

                        if (this.chartarea.SyncChartArea != null)
                        {
                            SyncChartAreas Sarea = this.area.SyncChartArea as SyncChartAreas;
                            if (this.chartarea.SyncChartArea.InteractiveCursors.Count > 0)
                            {
                                InteractiveCursor syncic = this.series.Area.SyncChartArea.InteractiveCursors[this.CollectionIndex];
                                foreach (InteractiveCursor ic in syncic.interactivecursor)
                                {
                                    if (flag)
                                        ic.MoveOnMouseMove = false;
                                    ic.HorizontalCursorVisibility = Visibility.Collapsed;
                                }
                                if (flag)
                                    this.MoveOnMouseMove = true;

                                //this.HorizontalCursorVisibility = Visibility.Visible;


                                if (this.area.SyncChartArea.CursorSeries != null)
                                {
                                    if (this.area.SyncChartArea.CursorSeries.Area == cursrorArea)
                                    {
                                        foreach (InteractiveCursor ic in cursrorArea.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Visible;
                                        }
                                    }
                                    if (this.area.SyncChartArea.CursorSeries.Area != cursrorArea)
                                    {
                                        if (cursrorArea != null)
                                        foreach (InteractiveCursor ic in cursrorArea.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Collapsed;
                                        }

                                        foreach (InteractiveCursor ic in this.area.SyncChartArea.CursorSeries.Area.InteractiveCursors)
                                        {
                                            ic.HorizontalCursorVisibility = Visibility.Visible;
                                        }
                                    }
                                }

                                //if (syncic.LabelVisibility == Visibility.Visible)
                                //{
                                //    foreach (InteractiveCursor ic in syncic.interactivecursor)
                                //    {
                                //        ic.HorizontalLabelVisibility = Visibility.Collapsed;
                                //    }
                                //    this.HorizontalLabelVisibility = Visibility.Visible;
                                //}
                            }
                        }
                        else
                        {
                            if (flag)
                                this.MoveOnMouseMove = true;
                        }


                        if (this.HorizontalCursorLabelContent != null)
                        {

                            if (this.IsInversedLabel == true)

                                this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length - 1) * 8;
                            else
                                this.LabelPosition = this.ActualWidth;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                if (this.SelectedCursor == true)
                {
                    if (OffsetX >= this.ActualWidth && area.SyncChartArea==null)
                        OffsetX = this.ActualWidth / 2;
                    if (OffsetY >= this.ActualHeight && area.SyncChartArea==null)
                        OffsetY = this.ActualHeight / 2;
                    if (this.HorizontalCursorLabelContent != null)
                    {
                        if (this.IsInversedLabel == true)
                            this.LabelPosition = 0 - (HorizontalCursorLabelContent.ToString().Length - 1) * 8;
                        else
                            this.LabelPosition = ActualWidth;
                    }
                }
            }
            SetValueForInteractiveCursorSymbol();
        }

        private void SetValueForInteractiveCursorSymbol()
        {
            this.LeftPosition = this.OffsetX - (this.CursorSymbolWidth/2);
            this.TopPosition = this.OffsetY - (this.CursorSymbolHeight/2);
        }

        void SyncInteractiveCursor_MouseMove(MouseEventArgs e)
        {
            Point nearPoint = new Point();
            #region initialize

            foreach (ChartSeries series in this.chartarea.Series)
            {
                if (series.Segments.Count > 0)
                {
                    if (series.Type == ChartTypes.Bar)
                    {
                        nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.Y);
                        nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.X);
                    }
                    else
                    {
                        nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.X);
                        nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, series.Segments[0].CorrespondingPoints[0].DataPoint.Y);
                    }
                    if (series != null)
                    {
                        this.Selectedseries = series;
                        this.area.SyncChartArea.CursorSeries = series;
                    }

                    break;
                }
            }
            #endregion
            int serCount1 = 0;
            foreach (ChartSeries series in this.chartarea.Series)
            {
                #region BarType
                if (series.Type == ChartTypes.Bar ||
                        series.Type == ChartTypes.RotatedSpline ||
                        series.Type == ChartTypes.StackingBar ||
                        series.Type == ChartTypes.StackingBar100 ||
                        series.Type == ChartTypes.Tornado ||
                        series.Type == ChartTypes.Gantt)
                {

                    Point pt = e.GetPosition(this.chartarea);
                    int count = 0;
                    foreach (ChartSegment cs in series.Segments)
                    {
                        if ((((pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                            (pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))) +
                            ((pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                            (pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))))
                            <=
                            (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) + ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y))))
                        {
                            nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                            nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                            if (series != null)
                            {
                                this.Selectedseries = series;
                                this.area.SyncChartArea.CursorSeries = series;
                            }
                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                        }
                        count++;
                        if (count == series.Segments.Count && series.Type == ChartTypes.RotatedSpline)
                        {
                            if ((((pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X)) *
                                (pt.Y - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X))) +
                                ((pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y)) *
                                (pt.X - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y))))
                                <=
                                (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X))))
                            {
                                nearPoint.Y = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                nearPoint.X = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                if (series != null)
                                {
                                    this.Selectedseries = series;
                                    this.area.SyncChartArea.CursorSeries = series;
                                }
                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                            }
                        }
                    }

                }
                #endregion

                #region AreaType
                else if (series.Type == ChartTypes.Area ||
                        series.Type == ChartTypes.FastColumn ||
                        series.Type == ChartTypes.FastLine ||
                        series.Type == ChartTypes.FastScatter ||
                        series.Type == ChartTypes.HiLoArea ||
                        series.Type == ChartTypes.SplineArea ||
                        series.Type == ChartTypes.StackingArea ||
                        series.Type == ChartTypes.StackingLine ||
                        series.Type == ChartTypes.StackingSpline ||
                        series.Type == ChartTypes.StackingSplineArea ||
                        series.Type == ChartTypes.StepArea ||
                        series.Type == ChartTypes.RangeArea)
                {
                    Point pt = e.GetPosition(this.chartarea);
                    if (series.Segments != null && series.Segments.Count > 0)
                    {
                        foreach (ChartIndexedDataPoint cs in series.Segments[0].CorrespondingPoints)
                        {
                            if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y)) *
                                (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y))) +
                                ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)) *
                                (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)))
                                <=
                                ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                            {
                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X);
                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y);
                                if (series != null)
                                {
                                    this.Selectedseries = series;
                                    this.area.SyncChartArea.CursorSeries = series;
                                }
                                this.XValue = cs.DataPoint.X;
                                this.YValue = cs.DataPoint.Y;
                            }
                        }
                    }

                }
                #endregion

                #region StackingColumn
                if (series.Type == ChartTypes.StackingColumn)
                {
                    if (serCount1 == 0)
                    {
                        Point pt = e.GetPosition(this.chartarea);

                        foreach (ChartSegment cs in series.Segments)
                        {
                            if ((((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                                (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))) +
                                ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))))
                                <=
                                (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X))))
                            {
                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                                if (series != null)
                                {
                                    this.Selectedseries = series;
                                    this.area.SyncChartArea.CursorSeries = series;
                                }
                                this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                            }

                        }

                    }
                    else
                    {

                        Point pt = e.GetPosition(this.chartarea);
                        if ((this.chartarea.Series[serCount1 - 1].Type == ChartTypes.StackingColumn) || (this.chartarea.Series[serCount1 - 1].Type == ChartTypes.FastStackingColumn))
                        {
                            int count = 0;
                            foreach (ChartSegment cs in series.Segments)
                            {
                                if ((((pt.Y - this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y))) *
                                    (pt.Y - this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y)))) +
                                    ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                                    (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))))
                                    <=
                                    (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                    ((pt.X - nearPoint.X) * (pt.X - nearPoint.X))))
                                {
                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, (cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y));
                                    if (series != null)
                                    {
                                        this.Selectedseries = series;
                                        this.area.SyncChartArea.CursorSeries = series;
                                    }
                                    this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                                    this.YValue = cs.CorrespondingPoints[0].DataPoint.Y + this.chartarea.Series[serCount1 - 1].Segments[count].CorrespondingPoints[0].DataPoint.Y;
                                }
                                count++;
                            }
                        }
                    }
                    serCount1++;
                }
                #endregion

                #region FastStackingColumn
                if (series.Type == ChartTypes.StackingColumn || series.Type == ChartTypes.FastStackingColumn)
                {
                    if (serCount1 == 0)
                    {
                        Point pt = e.GetPosition(this.chartarea);
                        if (series.Segments != null)
                        {
                            foreach (ChartIndexedDataPoint cs in series.Segments[0].CorrespondingPoints)
                            {
                                if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y)) *
                                    (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y))) +
                                    ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)) *
                                    (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)))
                                    <=
                                    ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                    ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                {
                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X);
                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y);
                                    if (series != null)
                                    {
                                        this.Selectedseries = series;
                                        this.area.SyncChartArea.CursorSeries = series;
                                    }
                                    this.XValue = cs.DataPoint.X;
                                    this.YValue = cs.DataPoint.Y;
                                }
                            }
                        }

                    }
                    else
                    {

                        Point pt = e.GetPosition(this.chartarea);
                        List<ChartIndexedDataPoint> datapt = new List<ChartIndexedDataPoint>();
                        if (this.chartarea.Series[serCount1 - 1].Type == ChartTypes.FastStackingColumn)
                        {
                            foreach (ChartIndexedDataPoint cs in this.chartarea.Series[serCount1 - 1].Segments[0].CorrespondingPoints)
                            {
                                datapt.Add(cs);
                            }
                            int count = 0;
                            foreach (ChartIndexedDataPoint cs in series.Segments[0].CorrespondingPoints)
                            {
                                if (((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y + datapt[count].DataPoint.Y)) *
                                    (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y + datapt[count].DataPoint.Y))) +
                                    ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)) *
                                    (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X)))
                                    <=
                                    ((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                    ((pt.X - nearPoint.X) * (pt.X - nearPoint.X)))
                                {
                                    nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X);
                                    nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y + datapt[count].DataPoint.Y);
                                    if (series != null)
                                    {
                                        this.Selectedseries = series;
                                        this.area.SyncChartArea.CursorSeries = series;
                                    }
                                    this.XValue = cs.DataPoint.X;
                                    this.YValue = cs.DataPoint.Y + datapt[count].DataPoint.Y;
                                }
                                count++;
                            }
                        }
                    }
                    serCount1++;
                }
                #endregion

                #region ColumnType
                else
                {
                    Point pt = e.GetPosition(this.chartarea);
                    int count = 0;
                    foreach (ChartSegment cs in series.Segments)
                    {
                        if ((((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)) *
                            (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y))) +
                            ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X)) *
                            (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X))))
                            <=
                            (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                            ((pt.X - nearPoint.X) * (pt.X - nearPoint.X))))
                        {
                            nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X);
                            nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y);
                            if (series != null)
                            {
                                this.Selectedseries = series;
                                this.area.SyncChartArea.CursorSeries = series;
                            }
                            this.XValue = cs.CorrespondingPoints[0].DataPoint.X;
                            this.YValue = cs.CorrespondingPoints[0].DataPoint.Y;
                        }
                        count++;
                        if (count == series.Segments.Count && (series.Type == ChartTypes.Line || series.Type == ChartTypes.Spline ||
                            series.Type == ChartTypes.StepLine || series.Type == ChartTypes.ThreeLineBreak))
                        {
                            if ((((pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y)) *
                                (pt.Y - this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y))) +
                                ((pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X)) *
                                (pt.X - this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X))))
                                <=
                                (((pt.Y - nearPoint.Y) * (pt.Y - nearPoint.Y)) +
                                ((pt.X - nearPoint.X) * (pt.X - nearPoint.X))))
                            {
                                nearPoint.X = this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[1].DataPoint.X);
                                nearPoint.Y = this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[1].DataPoint.Y);
                                if (series != null)
                                {
                                    this.Selectedseries = series;
                                    this.area.SyncChartArea.CursorSeries = series;
                                }
                                this.XValue = cs.CorrespondingPoints[1].DataPoint.X;
                                this.YValue = cs.CorrespondingPoints[1].DataPoint.Y;
                            }
                        }
                    }

                }
                #endregion
            }
            SetValueForInteractiveCursor(true);

        }
        
        void GenerateSegmentPointCollection()
        {
            this.PointCollection = new ObservableCollection<Point>();
            foreach (ChartSeries series in this.chartarea.Series)
            {
                if (series.Type == ChartTypes.Bar ||series.Type == ChartTypes.RotatedSpline ||series.Type == ChartTypes.StackingBar || series.Type == ChartTypes.StackingBar100 || series.Type == ChartTypes.Tornado ||series.Type == ChartTypes.Gantt)
                {
                    foreach (ChartSegment cs in series.Segments)
                    {
                        this.PointCollection.Add(new Point(this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X),this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)));
                    }
                }
                else if (series.Type == ChartTypes.Area || series.Type == ChartTypes.FastColumn || series.Type == ChartTypes.FastLine ||
                                    series.Type == ChartTypes.FastScatter || series.Type == ChartTypes.HiLoArea || series.Type == ChartTypes.SplineArea ||
                                    series.Type == ChartTypes.StackingArea || series.Type == ChartTypes.StackingLine || series.Type == ChartTypes.StackingSpline || series.Type == ChartTypes.StackingSplineArea || series.Type == ChartTypes.StepArea || series.Type == ChartTypes.RangeArea)
                {
                    if (series.Segments.Count > 0)
                    {
                        foreach (ChartIndexedDataPoint cs in series.Segments[0].CorrespondingPoints)
                        {
                            this.PointCollection.Add(new Point(this.chartarea.ValueToPoint(series.XAxis, cs.DataPoint.X),this.chartarea.ValueToPoint(series.YAxis, cs.DataPoint.Y)));
                        }
                    }
                }
                else
                {
                    foreach (ChartSegment cs in series.Segments)
                    {
                        this.PointCollection.Add(new Point(this.chartarea.ValueToPoint(series.XAxis, cs.CorrespondingPoints[0].DataPoint.X),this.chartarea.ValueToPoint(series.YAxis, cs.CorrespondingPoints[0].DataPoint.Y)));
                    }
                }
            }

        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Return XAML string from the object
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            _xamlString = XamlWriter.Save(this);
            return _xamlString;
        }

        /// <summary>
        /// Return the object from the XAML string
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }


    /// <summary>
    /// Represents the class for InteractiveCursorLabelContent
    /// </summary>
    public class InteractiveCursorLabelContent : DependencyObject, INotifyPropertyChanged
    {
        IChartDataPoint datapoint = null;
        /// <summary>
        /// Get and Set the DataPointProperty
        /// </summary>
        public IChartDataPoint DataPoint
        {
            get
            {
                return datapoint;
            }
            set
            {
                datapoint = value;
                NotifyPropertyChanged("DataPoint");
            }
        }
        object x = 0.0;
        /// <summary>
        /// Get and Set the X property
        /// </summary>
        public object X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                NotifyPropertyChanged("X");
            }
        }

        object y = 0.0;
        /// <summary>
        /// Get and Set the Y property
        /// </summary>
        public object Y
        {
            get { return y; }
            set
            {
                y = value;
                NotifyPropertyChanged("Y");
            }
        }

        ChartSeries series = null;
        /// <summary>
        /// Get and Set the Series Property
        /// </summary>
        public ChartSeries Series
        {
            get { return series; }
            set
            {
                series = value;
                NotifyPropertyChanged("Series");
            }
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Event for PropertyChanged in InteractiveCursorLabelContent
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

    }

}
