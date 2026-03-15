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
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared;
using System.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Represents the TimelineControl Class
    /// </summary>
    public class TimeLineControl : ChartArea
    {
        #region dependencyProperty


        /// <summary>
        /// Represents VisualStyle for Timelinecontrol
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
       DependencyProperty.Register("VisualStyle", typeof(TimeLineStyles), typeof(TimeLineControl), new PropertyMetadata(TimeLineStyles.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Gets or sets the VisualStyle value.
        /// </summary>
        /// <value>The VisualStyle.</value>
        public TimeLineStyles VisualStyle
        {
            get
            {
                return (TimeLineStyles)GetValue(VisualStyleProperty);
            }

            set
            {
                SetValue(VisualStyleProperty, value);
            }
        }

        /// <summary>
        /// Represents ScrollBarInterior in timelinecontrol.
        /// </summary>
        public static readonly DependencyProperty ScrollBarInteriorProperty =
         DependencyProperty.Register("ScrollBarInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Gets or sets the ScrollBarInterior value.
        /// </summary>
        /// <value>The ScrollBarInterior.</value>
        public Brush ScrollBarInterior
        {
            get
            {
                return (Brush)GetValue(ScrollBarInteriorProperty);
            }

            set
            {
                SetValue(ScrollBarInteriorProperty, value);
            }
        }

        /// <summary>
        /// Represents ScrollThumbTemplate in TimelineControl ScrollBar
        /// </summary>
        public static readonly DependencyProperty ScrollThumbTemplateProperty =
         DependencyProperty.Register("ScrollThumbTemplate", typeof(DataTemplate), typeof(TimeLineControl), new PropertyMetadata(null));



        /// <summary>
        /// Get and Set ScrollBarBorderBrushProeprty
        /// </summary>
        public Brush ScrollBarBorderBrush
        {
            get { return (Brush)GetValue(ScrollBarBorderBrushProperty); }
            set { SetValue(ScrollBarBorderBrushProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for ScrollBarBorderBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollBarBorderBrushProperty =
            DependencyProperty.Register("ScrollBarBorderBrush", typeof(Brush), typeof(TimeLineControl), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0XFF,0XCC,0XCC,0XCC))));




        /// <summary>
        /// Get and Set ScrollBarBorderThickness property
        /// </summary>
        public Thickness ScrollBarBorderThickness
        {
            get { return (Thickness)GetValue(ScrollBarBorderThicknessProperty); }
            set { SetValue(ScrollBarBorderThicknessProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScrollBarBorderThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollBarBorderThicknessProperty =
            DependencyProperty.Register("ScrollBarBorderThickness", typeof(Thickness), typeof(TimeLineControl), new UIPropertyMetadata(new Thickness(1)));



        /// <summary>
        /// Get and Set ScrollBarSmallIncreaseTemplate Property
        /// </summary>
        public ControlTemplate ScrollBarSmallIncreaseTemplate
        {
            get { return (ControlTemplate)GetValue(ScrollBarSmallIncreaseTemplateProperty); }
            set { SetValue(ScrollBarSmallIncreaseTemplateProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScrollBarSmallIncreaseTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollBarSmallIncreaseTemplateProperty =
            DependencyProperty.Register("ScrollBarSmallIncreaseTemplate", typeof(ControlTemplate), typeof(TimeLineControl), new UIPropertyMetadata(null));



        /// <summary>
        /// Get and Set ScrollBarSmallDecreaseTemplate property
        /// </summary>
        public ControlTemplate ScrollBarSmallDecreaseTemplate
        {
            get { return (ControlTemplate)GetValue(ScrollBarSmallDecreaseTemplateProperty); }
            set { SetValue(ScrollBarSmallDecreaseTemplateProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for ScrollBarSmallDecreaseTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollBarSmallDecreaseTemplateProperty =
            DependencyProperty.Register("ScrollBarSmallDecreaseTemplate", typeof(ControlTemplate), typeof(TimeLineControl), new UIPropertyMetadata(null));

        

        

        /// <summary>
        /// Gets or sets the ScrollBarInterior value.
        /// </summary>
        /// <value>The ScrollBarInterior.</value>
        public DataTemplate ScrollThumbTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ScrollThumbTemplateProperty);
            }

            set
            {
                SetValue(ScrollThumbTemplateProperty, value);
            }
        }

              
        /// <summary>
        /// Identifies the DataSource dependency property.
        /// </summary>
        public static readonly DependencyProperty DataSourceProperty =
         DependencyProperty.Register("DataSource", typeof(IEnumerable), typeof(TimeLineControl), new PropertyMetadata(null, new PropertyChangedCallback(OnEnableRangeIndicatorChanged)));

        /// <summary>
        /// Gets or sets the DataSource value.
        /// </summary>
        /// <value>The DataSource.</value>
        public IEnumerable DataSource
        {
            get
            {
                return (IEnumerable)GetValue(DataSourceProperty);
            }

            set
            {
                SetValue(DataSourceProperty, value);
            }
        }

       /// <summary>
        /// Identifies the SelectedData dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDataProperty =
        DependencyProperty.Register("SelectedData", typeof(object), typeof(TimeLineControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the SelectedData value.
        /// </summary>
        /// <value>The SelectedData.</value>
        public object SelectedData
        {
            get
            {
                return (object)GetValue(SelectedDataProperty);
            }

            set
            {
                SetValue(SelectedDataProperty, value);
            }
        }

         /// <summary>
         /// Get and Set the SelectedRangeProeprty 
         /// </summary>
         public DoubleRange SelectedRange
        {
            get { return (DoubleRange)GetValue(SelectedRangeProperty); }
            set { SetValue(SelectedRangeProperty, value); }
        }

        /// <summary>
         /// Using a DependencyProperty as the backing store for SelectedRange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register("SelectedRange", typeof(DoubleRange), typeof(TimeLineControl), new UIPropertyMetadata(DoubleRange.Empty));

        
        /// <summary>
        ///  Identifies the UnSelectedRegionInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty UnSelectedRegionInteriorProperty =
        DependencyProperty.Register("UnSelectedRegionInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets or sets the UnSelectedRegionInterior value.
        /// </summary>
        /// <value>The UnSelectedRegionInterior.</value>
        public Brush UnSelectedRegionInterior
        {
            get
            {
                return (Brush)GetValue(UnSelectedRegionInteriorProperty);
            }

            set
            {
                SetValue(UnSelectedRegionInteriorProperty, value);
            }
        }

        ////[StyleTypedProperty(Property = "BindingPathX", StyleTargetType = typeof(string))]        
        /// <summary>
        /// Identifies the BindingPathX dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathXProperty =
         DependencyProperty.Register("BindingPathX", typeof(string), typeof(TimeLineControl), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Gets or sets the BindingPathX value.
        /// </summary>
        /// <value>The BindingPathX.</value>
        public string BindingPathX
        {
            get
            {
                return (string)GetValue(BindingPathXProperty);
            }

            set
            {
                SetValue(BindingPathXProperty, value);
            }
        }

      
        ////[StyleTypedProperty(Property = "EdgePointSelection", StyleTargetType = typeof(string))]        
        /// <summary>
        ///  Identifies the EdgePointSelection dependency property.
        /// </summary>
        public static readonly DependencyProperty EdgePointSelectionProperty =
         DependencyProperty.Register("EdgePointSelection", typeof(bool), typeof(TimeLineControl), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the EdgePointSelection value.
        /// </summary>
        /// <value>The EdgePointSelection.</value>
        public bool EdgePointSelection
        {
            get
            {
                return (bool)GetValue(EdgePointSelectionProperty);
            }

            set
            {
                SetValue(EdgePointSelectionProperty, value);
            }
        }

       ////[StyleTypedProperty(Property = "BindingPathsY", StyleTargetType = typeof(IEnumerable<string>))]        
        /// <summary>
        /// Identifies the BindingPathsY dependency property.
        /// </summary>
        public static readonly DependencyProperty BindingPathsYProperty =
         DependencyProperty.Register("BindingPathsY", typeof(IEnumerable<string>), typeof(TimeLineControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the BindingPathsY value.
        /// </summary>
        /// <value>The BindingPathsY.</value>
        [TypeConverter(typeof(ChartPathsConverter))]
        public IEnumerable<string> BindingPathsY
        {
            get
            {
                return (IEnumerable<string>)GetValue(BindingPathsYProperty);
            }

            set
            {
                SetValue(BindingPathsYProperty, value);
            }
        }

        /// <summary>
        /// Return double value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  static double GetStartValue(DependencyObject obj)
        {
            return (double)obj.GetValue(StartValueProperty);
        }

        /// <summary>
        /// Sets the value of the StartValue dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public  static void SetStartValue(DependencyObject obj, double value)
        {
            obj.SetValue(StartValueProperty, value);
        }

        /// <summary>
        /// Indicates the StartValue Dependency Property
        /// </summary>.
        public new static readonly DependencyProperty StartValueProperty =
            DependencyProperty.RegisterAttached("StartValue", typeof(double), typeof(TimeLineControl ), new PropertyMetadata(double.NaN,new PropertyChangedCallback(OnStartValueChanged)));

        private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator != null && indicator.PrimaryAxis.ValueType==ChartValueType.Double)
            {
                if (indicator.ValueToPoint(indicator.PrimaryAxis,TimeLineControl.GetStartValue(indicator)) >= indicator.ValueToPoint(indicator.PrimaryAxis, indicator.PrimaryAxis.VisibleRange.Start) && indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetStartValue(indicator)) - 17 < indicator.GridWidth)
                {
                    indicator.LeftOffsetX = indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetStartValue(indicator)) - 17;
                    if (!indicator.leftPanning && !indicator.buttonPanning && !indicator.isEgdelabelCorrectionEnabled && !indicator.isRightUnselectClick && !indicator.isLeftUnSelectClick)
                        indicator.SetProperties();
                }
            }
        }


        /// <summary>
        /// Return double value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public  static double GetEndValue(DependencyObject obj)
        {
            return (double)obj.GetValue(EndValueProperty);
        }

        /// <summary>
        /// Sets the value of the EndValue dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetEndValue(DependencyObject obj, double value)
        {
            obj.SetValue(EndValueProperty, value);
        }

        /// <summary>
        /// Indicates the EndValue Dependency Property
        /// </summary>.
        public new static readonly DependencyProperty EndValueProperty =
            DependencyProperty.RegisterAttached("EndValue", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnEndValueChanged)));



        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator != null && indicator.PrimaryAxis.ValueType==ChartValueType.Double)
            {
                if (indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndValue(indicator) ) - 17 > indicator.ValueToPoint(indicator.PrimaryAxis, indicator.PrimaryAxis.VisibleRange.Start) && indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndValue(indicator)) - 17 < indicator.GridWidth)
                {
                    indicator.RightOffsetX = indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndValue(indicator)) - 17;
                    if (!indicator.rightPanning && !indicator.buttonPanning && !indicator.isEgdelabelCorrectionEnabled && !indicator.isRightUnselectClick && !indicator.isLeftUnSelectClick)
                        indicator.SetProperties();
                }
            }
        }


        /// <summary>
        /// Return DateTime value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetStartDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(StartDateProperty);
        }
        /// <summary>
        /// Sets the value of the StartDate dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetStartDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(StartDateProperty, value);
        }

        /// <summary>
        /// Indicates the StartDate Dependency Property
        /// </summary>
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.RegisterAttached("StartDate", typeof(DateTime), typeof(TimeLineControl ), new PropertyMetadata(new DateTime(), new PropertyChangedCallback(OnStartDateValueChanged)));

        /// <summary>
        /// Return DateTime value from the given DependencyObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime GetEndDate(DependencyObject obj)
        {
            return (DateTime)obj.GetValue(EndDateProperty);
        }
        /// <summary>
        /// Sets the value of the EndDate dependency property.
        /// </summary>
        /// <param name="obj">The DependencyObject obj.</param>
        /// <param name="value">The value.</param>
        public static void SetEndDate(DependencyObject obj, DateTime value)
        {
            obj.SetValue(EndDateProperty, value);
        }

        /// <summary>
        /// Indicates the EndDate Dependency Property
        /// </summary>.
        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.RegisterAttached("EndDate", typeof(DateTime), typeof(TimeLineControl  ), new PropertyMetadata(new DateTime(), new PropertyChangedCallback(OnEndDateValueChanged)));


        ////[StyleTypedProperty(Property = "ViewPortInterior", StyleTargetType = typeof(Brush))]        
        /// <summary>
        ///  Identifies the ViewPortInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewPortInteriorProperty =
         DependencyProperty.Register("ViewPortInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Gets or sets the ViewPortInterior value.
        /// </summary>
        /// <value>The ViewPortInterior.</value>
        public Brush ViewPortInterior
        {
            get
            {
                return (Brush)GetValue(ViewPortInteriorProperty);
            }

            set
            {
                SetValue(ViewPortInteriorProperty, value);
            }
        }

        ////[StyleTypedProperty(Property = "MouseOverInterior", StyleTargetType = typeof(Brush))]        
        /// <summary>
        ///  Identifies the MouseOverInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverInteriorProperty =
         DependencyProperty.Register("MouseOverInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Gets or sets the MouseOverInterior value.
        /// </summary>
        /// <value>The MouseOverInterior.</value>
        public Brush MouseOverInterior
        {
            get
            {
                return (Brush)GetValue(MouseOverInteriorProperty);
            }

            set
            {
                SetValue(MouseOverInteriorProperty, value);
            }
        }

       
        ////[StyleTypedProperty(Property = "ScrollBarVisibility", StyleTargetType = typeof(Brush))]        
        /// <summary>
        ///  Identifies the ScrollBarVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollBarVisibilityProperty =
         DependencyProperty.Register("ScrollBarVisibility", typeof(Visibility), typeof(TimeLineControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the ScrollBarVisibility value.
        /// </summary>
        /// <value>The ScrollBarVisibility.</value>
        public Visibility ScrollBarVisibility
        {
            get
            {
                return (Visibility)GetValue(ScrollBarVisibilityProperty);
            }

            set
            {
                SetValue(ScrollBarVisibilityProperty, value);
            }
        }

        
        ////[StyleTypedProperty(Property = "ViewLineInterior", StyleTargetType = typeof(Brush))]        
        /// <summary>
        /// Identifies the ViewLineInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewLineInteriorProperty =
         DependencyProperty.Register("ViewLineInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.DarkGray));

        /// <summary>
        /// Gets or sets the ViewLineInterior value.
        /// </summary>
        /// <value>The ViewLineInterior.</value>
        public Brush ViewLineInterior
        {
            get
            {
                return (Brush)GetValue(ViewLineInteriorProperty);
            }

            set
            {
                SetValue(ViewLineInteriorProperty, value);
            }
        }


        ////[StyleTypedProperty(Property = "TimeLineInterior", StyleTargetType = typeof(Brush))]        
        /// <summary>
        ///  Identifies the TimeLineInterior dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeLineInteriorProperty =
         DependencyProperty.Register("TimeLineInterior", typeof(Brush), typeof(TimeLineControl), new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnTimeLineChanged)));

        /// <summary>
        /// Gets or sets the TimeLineInterior value.
        /// </summary>
        /// <value>The TimeLineInterior.</value>
        public Brush TimeLineInterior
        {
            get
            {
                return (Brush)GetValue(TimeLineInteriorProperty);
            }

            set
            {
                SetValue(TimeLineInteriorProperty, value);
            }
        }

              
        /// <summary>
        /// Identifies the TimeLineThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeLineThicknessProperty =
         DependencyProperty.Register("TimeLineThickness", typeof(double), typeof(TimeLineControl), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or sets the TimeLineThickness value.
        /// </summary>
        /// <value>The TimeLineThickness.</value>
        public double TimeLineThickness
        {
            get
            {
                return (double)GetValue(TimeLineThicknessProperty);
            }

            set
            {
                SetValue(TimeLineThicknessProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for Define the LeftIndicator in TimelineControl
        /// </summary>
        public static readonly DependencyProperty LeftIndicatorTemplateProperty =
         DependencyProperty.Register("LeftIndicatorTemplate", typeof(DataTemplate), typeof(TimeLineControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the TimeLineThickness value.
        /// </summary>
        /// <value>The TimeLineThickness.</value>
        public DataTemplate LeftIndicatorTemplate
        {
            get
            {
                return (DataTemplate)GetValue(LeftIndicatorTemplateProperty);
            }

            set
            {
                SetValue(LeftIndicatorTemplateProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for define RightIndicator in TimelineControl
        /// </summary>
        public static readonly DependencyProperty RightIndicatorTemplateProperty =
       DependencyProperty.Register("RightIndicatorTemplate", typeof(DataTemplate), typeof(TimeLineControl), new PropertyMetadata(null));



        private double ThumbHeight
        {
            get { return (double)GetValue(ThumbHeightProperty); }
            set { SetValue(ThumbHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftThumbHeight.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ThumbHeightProperty =
            DependencyProperty.Register("ThumbHeight", typeof(double), typeof(TimeLineControl), new UIPropertyMetadata(20d));



        private double ThumbWidth
        {
            get { return (double)GetValue(ThumbWidthProperty); }
            set { SetValue(ThumbWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftThumbWidth.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ThumbWidthProperty =
            DependencyProperty.Register("ThumbWidth", typeof(double), typeof(TimeLineControl), new UIPropertyMetadata(10d));




        /// <summary>
        /// Gets or sets the TimeLineThickness value.
        /// </summary>
        /// <value>The TimeLineThickness.</value>
        public DataTemplate RightIndicatorTemplate
        {
            get
            {
                return (DataTemplate)GetValue(RightIndicatorTemplateProperty);
            }

            set
            {
                SetValue(RightIndicatorTemplateProperty, value);
            }
        }

        ///// <summary>
        ///// Identifies the LeftOffsetX dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "LeftOffsetX", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty LeftOffsetXProperty =
         DependencyProperty.Register("LeftOffsetX", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN));

        internal static readonly DependencyProperty LeftOffsetYProperty =
         DependencyProperty.Register("LeftOffsetY", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN));

        internal static readonly DependencyProperty RightOffsetYProperty =
         DependencyProperty.Register("RightOffsetY", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the LeftOffsetX value.
        /// </summary>
        /// <value>The LeftOffsetX.</value>
        internal double LeftOffsetX
        {
            get
            {
                return (double)GetValue(LeftOffsetXProperty);
            }

            set
            {
                SetValue(LeftOffsetXProperty, value);
            }
        }

        internal double LeftOffsetY
        {
            get
            {
                return (double)GetValue(LeftOffsetYProperty);
            }

            set
            {
                SetValue(LeftOffsetYProperty, value);
            }

        }


        ///// <summary>
        ///// Identifies the RightOffsetX dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "RightOffsetX", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty RightOffsetXProperty =
       DependencyProperty.Register("RightOffsetX", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the RightOffsetX value.
        /// </summary>
        /// <value>The RightOffsetX.</value>
        internal double RightOffsetX
        {
            get
            {
                return (double)GetValue(RightOffsetXProperty);
            }

            set
            {
                SetValue(RightOffsetXProperty, value);
            }
        }

        internal double RightOffsetY
        {
            get
            {
                return (double)GetValue(RightOffsetYProperty);
            }

            set
            {
                SetValue(RightOffsetYProperty, value);
            }
        }

        ///// <summary>
        ///// Identifies the CanvasLeft dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "CanvasLeft", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty CanvasLeftProperty =
       DependencyProperty.Register("CanvasLeft", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the CanvasLeft value.
        /// </summary>
        /// <value>The CanvasLeft.</value>
        internal double CanvasLeft
        {
            get
            {
                return (double)GetValue(CanvasLeftProperty);
            }

            set
            {
                SetValue(CanvasLeftProperty, value);
            }
        }

        internal static readonly DependencyProperty PathLeftProperty =
       DependencyProperty.Register("PathLeft", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the CanvasLeft value.
        /// </summary>
        /// <value>The CanvasLeft.</value>
        internal double PathLeft
        {
            get
            {
                return (double)GetValue(PathLeftProperty);
            }

            set
            {
                SetValue(PathLeftProperty, value);
            }
        }


        ///// <summary>
        ///// Identifies the CanvasRight dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "CanvasRight", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty CanvasRightProperty =
       DependencyProperty.Register("CanvasRight", typeof(double), typeof(TimeLineControl), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the CanvasRight value.
        /// </summary>
        /// <value>The CanvasRight.</value>
        internal double CanvasRight
        {
            get
            {
                return (double)GetValue(CanvasRightProperty);
            }

            set
            {
                SetValue(CanvasRightProperty, value);
            }
        }

        ///// <summary>
        ///// Identifies the SelectedRegionWidth dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "SelectedRegionWidth", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty SelectedRegionWidthProperty =
       DependencyProperty.Register("SelectedRegionWidth", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the SelectedRegionWidth value.
        /// </summary>
        /// <value>The SelectedRegionWidth.</value>
        internal double SelectedRegionWidth
        {
            get
            {
                return (double)GetValue(SelectedRegionWidthProperty);
            }

            set
            {
                SetValue(SelectedRegionWidthProperty, value);
            }
        }


        ///// <summary>
        ///// Identifies the RightUnSelectedWidth dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "RightUnSelectedWidth", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty RightUnSelectedWidthProperty =
       DependencyProperty.Register("RightUnSelectedWidth", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the RightUnSelectedWidth value.
        /// </summary>
        /// <value>The RightUnSelectedWidth.</value>
        internal double RightUnSelectedWidth
        {
            get
            {
                return (double)GetValue(RightUnSelectedWidthProperty);
            }

            set
            {
                SetValue(RightUnSelectedWidthProperty, value);
            }
        }

        ///// <summary>
        ///// Identifies the GridHeight dependency property.
        ///// </summary>
        ////[StyleTypedProperty(Property = "GridHeight", StyleTargetType = typeof(double))]        
        internal static readonly DependencyProperty GridHeightProperty =
       DependencyProperty.Register("GridHeight", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the GridHeight value.
        /// </summary>
        /// <value>The GridHeight.</value>
        internal double GridHeight
        {
            get
            {
                return (double)GetValue(GridHeightProperty);
            }

            set
            {
                SetValue(GridHeightProperty, value);
            }
        }
        
        /// <summary>
        /// Dependency proeprty for MinimumTimelineInteval
        /// </summary>
        public static readonly DependencyProperty MinimumTimeLineIntervalProperty =
     DependencyProperty.Register("MinimumTimeLineInterval", typeof(double), typeof(TimeLineControl), new PropertyMetadata(6d, new PropertyChangedCallback(OnmimiumtimeChanged)));

        /// <summary>
        /// Get ans Set MinimumTimeLineInterval
        /// </summary>
        public double MinimumTimeLineInterval
        {
            get
            {
                return (double)GetValue(MinimumTimeLineIntervalProperty);
            }

            set
            {
                SetValue(MinimumTimeLineIntervalProperty, value);
            }
        }

        internal static readonly DependencyProperty MousehoverWidthProperty =
     DependencyProperty.Register("MousehoverWidth", typeof(double), typeof(TimeLineControl), new PropertyMetadata(6d));

        internal double MousehoverWidth
        {
            get
            {
                return (double)GetValue(MousehoverWidthProperty);
            }

            set
            {
                SetValue(MousehoverWidthProperty, value);
            }
        }

        internal static readonly DependencyProperty MousehoverLeftCanvasProperty =
     DependencyProperty.Register("MousehoverLeftCanvas", typeof(double), typeof(TimeLineControl), new PropertyMetadata(0d));

        internal double MousehoverLeftCanvas
        {
            get
            {
                return (double)GetValue(MousehoverLeftCanvasProperty);
            }

            set
            {
                SetValue(MousehoverLeftCanvasProperty, value);
            }
        }


        internal static readonly DependencyProperty MousehoverVisibilityProperty =
     DependencyProperty.Register("MousehoverVisibility", typeof(Visibility), typeof(TimeLineControl), new PropertyMetadata(Visibility.Collapsed));

        internal Visibility MousehoverVisibility
        {
            get
            {
                return (Visibility)GetValue(MousehoverVisibilityProperty);
            }

            set
            {
                SetValue(MousehoverVisibilityProperty, value);
            }
        }

        #endregion

        #region Unaccessed properties

        /// <summary>
        /// Unaccessed  Allow3DRotateProperty
        /// </summary>
        public new bool Allow3DRotate { get { return base.Allow3DRotate; } }
        /// <summary>
        /// Unaccessed  SecondaryAxisProperty
        /// </summary>
        public new ChartAxis SecondaryAxis { get { return base.SecondaryAxis; } }
        /// <summary>
        /// Unaccessed SeriesProperty
        /// </summary>
        public new ChartSeriesCollection Series { get { return base.Series; } }
        /// <summary>
        /// Unaccessed SplitRatioProperty.
        /// </summary>
        public new double SplitRatio { get { return base.SplitRatio; } }
        /// <summary>
        /// Unaccessed  SplitterBottomSpaceProperty
        /// </summary>
        public new double SplitterBottomSpace { get { return base.SplitterBottomSpace; } }
        /// <summary>
        /// Unaccessed  SplitterColorProperty
        /// </summary>
        public new Brush SplitterColor { get { return base.SplitterColor; } }
        /// <summary>
        /// Unaccessed SplitterDeltaProperty
        /// </summary>
        public new double SplitterDelta { get { return base.SplitterDelta; } }
        /// <summary>
        /// Unaccessed SplitterPositionProperty
        /// </summary>
        public new double SplitterPosition { get { return base.SplitterPosition; } }
        /// <summary>
        /// Unaccessed SplittedStrokeProperty
        /// </summary>
        public new Brush SplitterStroke { get { return base.SplitterStroke; } }
        /// <summary>
        /// Unaccessed SplitterVisibilityProperty
        /// </summary>
        public new SpliterVisibility SplitterVisiblity { get { return base.SplitterVisiblity; } }
        /// <summary>
        /// Unaccessed SplitterWidthProperty
        /// </summary>
        public new double SplitterWidth { get { return base.SplitterWidth; } }

        #endregion

        #region Members

        private Grid leftGrid = null;
        private Grid rightGrid = null;
        private bool leftPanning = false;
        private bool rightPanning = false;
        private RepeatButton leftRepeatButton = null;
        private RepeatButton rightRepeatButton = null;
        private bool buttonPanning = false;
        private Grid button = null;
        private double GridWidth = 0d;
        private ChartSeries TimeLineSeries = null;
        private bool isLoadAtFirstTime = true;
        private double prevScrollWidth = 0d;
        private Canvas leftUnselectCanvas = null;
        private Canvas rightUnselectCanvas = null;
        private bool isEgdelabelCorrectionEnabled = false;
        private bool isRightUnselectClick = false;
        private bool isLeftUnSelectClick = false;
        internal new string stylename = null;
        internal int StyleIndexValue;
        ResourceDictionary baseRD = null;
        #endregion

        #region Constructor
        static TimeLineControl()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(TimeLineControl));
            ////This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            ////This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLineControl), new FrameworkPropertyMetadata(typeof(TimeLineControl)));

        }
        /// <summary>
        /// Constructor for TimelineControl
        /// </summary>
        public TimeLineControl()
        {
            this.DefaultStyleKey = typeof(TimeLineControl);
            this.IsContextMenuEnabled = false;
            this.PrimaryAxis.RangePadding = ChartRangePaddingType.None;
            if (this.RightOffsetX > this.LeftOffsetX)
            {
                this.SelectedRegionWidth = this.RightOffsetX - this.LeftOffsetX;
                this.CanvasLeft = this.LeftOffsetX;
                this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                this.CanvasRight = this.RightOffsetX;
                this.RightUnSelectedWidth = this.GridWidth - this.RightOffsetX;
            }
            else
            {
                this.SelectedRegionWidth = this.LeftOffsetX - this.RightOffsetX;
                this.CanvasLeft = this.RightOffsetX;
                this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                this.CanvasRight = this.LeftOffsetX;
                this.RightUnSelectedWidth = this.GridWidth - this.LeftOffsetX;
            }
            this.PrimaryAxis.AxisVisibility = System.Windows.Visibility.Visible;
            this.PrimaryAxis.LabelPosition = LabelPositions.Inside;
            this.PrimaryAxis.TickSize = 0d;
            this.PrimaryAxis.LabelDateTimeFormat = "MMM d,yyyy";
            this.PrimaryAxis.IntersectAction = ChartLabelIntersectAction.Hide;
            this.SecondaryAxis.AxisVisibility = System.Windows.Visibility.Collapsed;
            TimeLineControl.SetShowGridLines(this.SecondaryAxis, false);
            this.Loaded += new RoutedEventHandler(ChartRangeIndicator_Loaded);
            this.MouseMove += new MouseEventHandler(ChartRangeIndicator_MouseMove);
            this.MouseUp += new MouseButtonEventHandler(ChartRangeIndicator_MouseUp);
            this.MouseLeave += new MouseEventHandler(TimeLineControl_MouseLeave);
            this.MouseDoubleClick += new MouseButtonEventHandler(TimeLineControl_MouseDoubleClick);
            this.SizeChanged += new SizeChangedEventHandler(ChartRangeIndicator_SizeChanged);
        }


        #endregion

        #region Methods


        void ChartRangeIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TimeLineControl indicator = sender as TimeLineControl;
            if (indicator != null)
            {
                ChartAreaPresenter presenter = indicator.m_areaPresenter as ChartAreaPresenter;
                if (presenter != null && !indicator.isLoadAtFirstTime)
                {                   
                    DependencyObject obj = VisualTreeHelper.GetChild(presenter, 0);
                    obj = VisualTreeHelper.GetChild(obj, 1);
                    Grid grid = obj as Grid;
                    if (grid != null)
                    {
                        this.GridHeight = grid.ActualHeight;
                    }
                    this.GridHeight = presenter.GridElement.ActualHeight; 
                    presenter.Width = indicator.ActualWidth - 34;
                    this.GridWidth = presenter.Width;
                    ItemsControl axesContainer = presenter.AxesContainer;
                    FrameworkElement element = axesContainer.ItemContainerGenerator.ContainerFromItem(indicator.PrimaryAxis) as FrameworkElement;
                    indicator.LeftOffsetX = indicator.PrimaryAxis.ValueToCoefficient(TimeLineControl.GetStartDate(indicator).ToOADate()) * presenter.Width;
                    indicator.RightOffsetX = indicator.PrimaryAxis.ValueToCoefficient(TimeLineControl.GetEndDate(indicator).ToOADate()) * presenter.Width;
                    if (this.RightOffsetX > this.LeftOffsetX)
                    {
                        this.SelectedRegionWidth = this.RightOffsetX - (this.LeftOffsetX > 0 ? this.LeftOffsetX : 0);
                        this.CanvasLeft = this.LeftOffsetX > 0 ? this.LeftOffsetX : 0;
                        this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                        this.CanvasRight = this.RightOffsetX;
                        this.RightUnSelectedWidth = this.GridWidth - this.RightOffsetX;
                    }
                    else
                    {
                        this.SelectedRegionWidth = this.LeftOffsetX - (this.RightOffsetX > 0 ? this.RightOffsetX : 0);
                        this.CanvasLeft = this.RightOffsetX > 0 ? this.RightOffsetX : 0;
                        this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                        this.CanvasRight = this.LeftOffsetX;
                        this.RightUnSelectedWidth = this.GridWidth - this.LeftOffsetX;
                    }
                    this.ThumbHeight = this.GridHeight / 4;
                    if (presenter.GridElement.ActualHeight <= this.ThumbHeight)
                        this.ThumbHeight = 0d;
                    if (this.TimeLineSeries != null)
                    {

                        ObservableCollection<IChartDataPoint> dataPoints = new ObservableCollection<IChartDataPoint>();
                        double start = TimeLineControl.GetStartDate(indicator).ToOADate() < TimeLineControl.GetEndDate(indicator).ToOADate() ? TimeLineControl.GetStartDate(indicator).ToOADate() : TimeLineControl.GetEndDate(indicator).ToOADate();
                        double end = TimeLineControl.GetEndDate(indicator).ToOADate() > TimeLineControl.GetStartDate(indicator).ToOADate() ? TimeLineControl.GetEndDate(indicator).ToOADate() : TimeLineControl.GetStartDate(indicator).ToOADate();
                        for (int i = 0; i < indicator.TimeLineSeries.Data.Count; i++)
                        {
                            if (indicator.TimeLineSeries.Data[i].X >= start && indicator.TimeLineSeries.Data[i].X <= end)
                            {
                                dataPoints.Add(indicator.TimeLineSeries.Data[i]);
                            }
                        }
                        Rect rect = new Rect(new Point(this.LeftOffsetX, 0), new Point(this.RightOffsetX, this.ActualHeight));
                        if (this.LeftOffsetX != this.RightOffsetX)
                        {
                            this.SelectedData = this.BoundsToDataSource(rect, this.TimeLineSeries);
                        }
                    }
                    if (this.EdgePointSelection)
                    {
                        this.EdgePointCorrection();
                    }
                }
            }
        }

        /// <summary>
        /// Method represents MinimunTimelineInterval changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnmimiumtimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeLineControl control = d as TimeLineControl;
            if (control != null && control.m_areaPresenter != null)
            {
                control.MousehoverWidth = control.ValueToPoint(control.PrimaryAxis, control.MinimumTimeLineInterval);
            }
        }
        /// <summary>
        /// Method Represents EndValue changes in TimelineControl
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnEndDateValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator != null)
            {
                if (indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndDate(indicator).ToOADate()) - 17 > indicator.ValueToPoint(indicator.PrimaryAxis, indicator.PrimaryAxis.VisibleRange.Start) && indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndDate(indicator).ToOADate()) - 18 <= indicator.GridWidth)
                {
                    indicator.RightOffsetX = indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetEndDate(indicator).ToOADate()) - 17;
                    if (!indicator.rightPanning && !indicator.buttonPanning && !indicator.isEgdelabelCorrectionEnabled && !indicator.isRightUnselectClick && !indicator.isLeftUnSelectClick)
                        indicator.SetProperties();
                    //indicator.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Method represents StartValueChanges
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnStartDateValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator != null)
            {
                if (indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetStartDate(indicator).ToOADate()) >= indicator.ValueToPoint(indicator.PrimaryAxis, indicator.PrimaryAxis.VisibleRange.Start) && indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetStartDate(indicator).ToOADate()) - 17 < indicator.GridWidth)
                {
                    indicator.LeftOffsetX = indicator.ValueToPoint(indicator.PrimaryAxis, TimeLineControl.GetStartDate(indicator).ToOADate()) - 17;
                    if (!indicator.leftPanning && !indicator.buttonPanning && !indicator.isEgdelabelCorrectionEnabled && !indicator.isRightUnselectClick && !indicator.isLeftUnSelectClick)
                        indicator.SetProperties();
                    //indicator.UpdateLayout();
                }
            }
        }
        /// <summary>
        /// Method represents TimelinecontrolInterior Changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnTimeLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator != null && indicator.TimeLineSeries != null)
            {
                indicator.TimeLineSeries.Interior = indicator.TimeLineInterior;
                indicator.TimeLineSeries.Stroke = indicator.TimeLineInterior;
            }

        }
        /// <summary>
        /// Method Represents timelineControl Datasource Changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnEnableRangeIndicatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            TimeLineControl indicator = d as TimeLineControl;
            if (indicator.Series.Contains(indicator.TimeLineSeries))
            {
                indicator.Series.Remove(indicator.TimeLineSeries);
            }
            if (indicator.TimeLineSeries != null)
            {
                indicator.TimeLineSeries.Dispose();
            }
            indicator.TimeLineSeries = new ChartSeries();

            Binding dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.DataSourceProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.DataSourceProperty, dataBinding);

            dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.BindingPathXProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.BindingPathXProperty, dataBinding);

            dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.BindingPathsYProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.BindingPathsYProperty, dataBinding);

            dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.TimeLineInteriorProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.InteriorProperty, dataBinding);
          
            dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.TimeLineInteriorProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.StrokeProperty, dataBinding);

            dataBinding = new Binding();
            dataBinding.Path = new PropertyPath(TimeLineControl.TimeLineThicknessProperty);
            dataBinding.Source = indicator;
            BindingOperations.SetBinding(indicator.TimeLineSeries, ChartSeries.StrokeThicknessProperty, dataBinding);

            indicator.TimeLineSeries.Type = ChartTypes.FastLine;
            indicator.Series.Add(indicator.TimeLineSeries);
            indicator.SetProperties();

        }

        void ChartRangeIndicator_Loaded(object sender, RoutedEventArgs e)
        {
            ChartAreaPresenter presenter = (sender as TimeLineControl).m_areaPresenter;
            if(this.PrimarySeries!=null)
            this.PrimarySeries.IsIndexed = false;
            if (isLoadAtFirstTime)
                presenter.Width = presenter.ActualWidth - 34;
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.OpposedPosition == true)
                {
                }
            }
            this.GridHeight = presenter.GridElement.ActualHeight + this.AxesThickness.Bottom; 
            //this.GridHeight = presenter.ActualHeight;
            presenter.Margin = new Thickness(0, 0, 0, -this.AxesThickness.Bottom);
            foreach (ChartAxis axis in (sender as TimeLineControl).Axes)
            {
                axis.LabelPosition = LabelPositions.Inside;
            }
            (sender as TimeLineControl).isLoadAtFirstTime = false;
            this.GridWidth = presenter.Width;
            if (presenter != null)
            {
                MousehoverWidth = this.ValueToPoint(this.PrimaryAxis, this.MinimumTimeLineInterval);
                DependencyObject obj = VisualTreeHelper.GetChild(presenter, 0);
                //Modified number 1 to 2 in below function call because an watermark element is inserted in that position in Generic.CartesianArea.xaml
                obj = VisualTreeHelper.GetChild(obj, 2);
                Grid grid = obj as Grid;

                int count = grid.Children.Count;

                Grid indicatorgrid = grid.Children[count - 1] as Grid;

                if (indicatorgrid != null)
                {
                    foreach (var item in indicatorgrid.Children)
                    {
                        if ((item is Canvas) && indicatorgrid.Children.IndexOf(item as Canvas) == 1)
                        {
                            leftUnselectCanvas = item as Canvas;
                            leftUnselectCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(leftUnselectCanvas_MouseLeftButtonDown);
                        }
                        if ((item is Canvas) && indicatorgrid.Children.IndexOf(item as Canvas) == 2)
                        {
                            rightUnselectCanvas = item as Canvas;
                            rightUnselectCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(rightUnselectCanvas_MouseLeftButtonDown);
                        }
                        if ((item is Grid) && indicatorgrid.Children.IndexOf(item as Grid) == 4)
                        {
                            leftGrid = item as Grid;
                            leftGrid.MouseEnter += new System.Windows.Input.MouseEventHandler(leftGrid_MouseEnter);
                            leftGrid.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(leftGrid_MouseLeftButtonDown);
                        }
                        else if (item is Canvas && indicatorgrid.Children.IndexOf(item as Canvas) == 3)
                        {
                            Canvas canvas = item as Canvas;
                            canvas.MouseDown += new MouseButtonEventHandler(canvas_MouseDown);
                            canvas.MouseUp += new MouseButtonEventHandler(canvas_MouseUp);
                        }
                        else if ((item is Grid) && indicatorgrid.Children.IndexOf(item as Grid) == 5)
                        {
                            rightGrid = item as Grid;
                            rightGrid.MouseEnter += new MouseEventHandler(rightGrid_MouseEnter);
                            rightGrid.MouseLeftButtonDown += new MouseButtonEventHandler(rightGrid_MouseLeftButtonDown);
                        }
                    }
                }

                DependencyObject obj1 = VisualTreeHelper.GetChild((sender as TimeLineControl), 0);
                obj1 = VisualTreeHelper.GetChild(obj1, 0);
                Grid areagrid = obj1 as Grid;
                Grid scrollBarGrid = areagrid.Children[1] as Grid;
                Border border = scrollBarGrid.Children[0] as Border;
                scrollBarGrid = VisualTreeHelper.GetChild(border, 0) as Grid;
                if (scrollBarGrid != null)
                {
                    foreach (var item in scrollBarGrid.Children)
                    {
                        if ((item is RepeatButton) && scrollBarGrid.Children.IndexOf(item as RepeatButton) == 0)
                        {
                            this.leftRepeatButton = item as RepeatButton;
                            this.leftRepeatButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(leftRepeatButton_PreviewMouseLeftButtonDown);
                        }
                        else if ((item is RepeatButton) && scrollBarGrid.Children.IndexOf(item as RepeatButton) == 2)
                        {
                            this.rightRepeatButton = item as RepeatButton;
                            this.rightRepeatButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(rightRepeatButton_PreviewMouseLeftButtonDown);
                        }
                        else if (item is Grid)
                        {
                            Grid buttongrid = item as Grid;
                            if (buttongrid != null && buttongrid.Children.Count > 0)
                            {
                                Canvas buttonCanvas = buttongrid.Children[0] as Canvas;

                                if (buttonCanvas != null && buttonCanvas.Children.Count > 0)
                                {
                                    button = buttonCanvas.Children[0] as Grid;
                                    if (button != null)
                                    {
                                        button.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(button_PreviewMouseLeftButtonDown);
                                        button.PreviewMouseUp += new MouseButtonEventHandler(button_MouseUp);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.UpdateLayout();
            DateTime startDate = TimeLineControl.GetStartDate(this);
            DateTime endDate = TimeLineControl.GetEndDate(this);
            double startValue = TimeLineControl.GetStartValue(this);
            double endValue = TimeLineControl.GetEndValue(this);
            if (this.PrimaryAxis.ValueType == ChartValueType.Double)
            {
               TimeLineControl.SetStartValue(this,double.IsNaN(startValue) ? this.PrimaryAxis.VisibleRange.Start : startValue);
                TimeLineControl.SetEndValue(this,double.IsNaN(endValue) ? this.PrimaryAxis.VisibleRange.End : endValue);
                this.LeftOffsetX = this.ValueToPoint(this.PrimaryAxis, TimeLineControl.GetStartValue(this)) - 17;
                this.RightOffsetX = this.ValueToPoint(this.PrimaryAxis, TimeLineControl.GetEndValue(this)) - 17;
                this.LeftOffsetY = this.ActualHeight / 2;
                this.RightOffsetY = this.ActualHeight / 2;
                this.ThumbHeight = this.GridHeight / 4;
                (sender as TimeLineControl).SetProperties();
            }
            else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime)
            {
               TimeLineControl.SetStartDate(this,startDate == DateTime.MinValue? DateTime.FromOADate(this.PrimaryAxis.VisibleRange.Start):startDate);
               TimeLineControl.SetEndDate(this, endDate == DateTime.MinValue ? DateTime.FromOADate(this.PrimaryAxis.VisibleRange.End) : endDate);
               this.LeftOffsetX = this.ValueToPoint(this.PrimaryAxis, TimeLineControl.GetStartDate(this).ToOADate()) - 17;
               this.RightOffsetX = this.ValueToPoint(this.PrimaryAxis, TimeLineControl.GetEndDate(this).ToOADate()) - 17;
               this.LeftOffsetY = this.ActualHeight / 2;
               this.RightOffsetY = this.ActualHeight / 2;
               this.ThumbHeight = this.GridHeight / 4;
               (sender as TimeLineControl).SetProperties();
            }

        }


        void rightUnselectCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            Point pt = e.GetPosition(this);
            double width = this.SelectedRegionWidth / 2;
            this.isRightUnselectClick = true;
            if (this.LeftOffsetX < this.RightOffsetX)
            {
                if (pt.X + width < this.GridWidth)
                {
                    this.LeftOffsetX = pt.X - width;
                    this.RightOffsetX = pt.X + width;
                }
                else
                {
                    this.LeftOffsetX = this.GridWidth - this.SelectedRegionWidth;
                    this.RightOffsetX = this.GridWidth;
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                       TimeLineControl.SetStartValue(this,this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetStartDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                        TimeLineControl.SetEndValue(this,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetEndDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
            }
            else if (this.RightOffsetX < this.LeftOffsetX)
            {
                if (pt.X + width < this.GridWidth)
                {
                    this.RightOffsetX = pt.X - width;
                    this.LeftOffsetX = pt.X + width;
                }
                else
                {
                    this.RightOffsetX = this.GridWidth - this.SelectedRegionWidth;
                    this.LeftOffsetX = this.GridWidth;
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                        TimeLineControl.SetStartValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetStartDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                        TimeLineControl.SetEndValue(this,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetEndDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
            }
            this.SetProperties();
        }

        void leftUnselectCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            Point pt = e.GetPosition(this);
            double width = this.SelectedRegionWidth / 2;
            this.isLeftUnSelectClick = true;
            if (this.LeftOffsetX < this.RightOffsetX)
            {
                if (pt.X - width > 0)
                {
                    this.LeftOffsetX = pt.X - width;
                    this.RightOffsetX = pt.X + width;
                }
                else
                {
                    this.LeftOffsetX = 0d;
                    this.RightOffsetX = this.SelectedRegionWidth;
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start && this.ValueToPoint(this.PrimaryAxis, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                       TimeLineControl.SetStartValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetStartDate(this, DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                       TimeLineControl.SetEndValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetEndDate(this, DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
            }
            else if (this.RightOffsetX < this.LeftOffsetX)
            {
                if (pt.X - width > 0)
                {
                    this.RightOffsetX = pt.X - width;
                    this.LeftOffsetX = pt.X + width;
                }
                else
                {
                    this.RightOffsetX = 0d;
                    this.LeftOffsetX = this.SelectedRegionWidth;
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start && this.ValueToPoint(this.PrimaryAxis,(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                        TimeLineControl.SetStartValue(this,this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetStartDate (this, DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) >= this.PrimaryAxis.VisibleRange.Start &&this.ValueToPoint(this.PrimaryAxis,this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))) <= this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double)
                    {
                        TimeLineControl.SetEndValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                    }
                    else if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetEndDate(this, DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
            }
            this.SetProperties();
        }

        void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.buttonPanning = false;
            this.ReleaseMouseCapture();
        }

        void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.buttonPanning = true;
            double value = this.LeftOffsetX > this.RightOffsetX ? this.RightOffsetX : this.LeftOffsetX;
            prevScrollWidth = (e.GetPosition(this).X - this.AxesThickness.Left);
            this.CaptureMouse();
        }

        void leftRepeatButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.LeftOffsetX + 5 > 0 && this.RightOffsetX + 5 > 0)
            {
                this.RightOffsetX = this.RightOffsetX - 1;
                this.LeftOffsetX = this.LeftOffsetX - 1;
            }
            this.SetProperties();
        }

        void rightRepeatButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((this.RightOffsetX < this.GridWidth) && (this.LeftOffsetX < this.GridWidth))
            {
                this.RightOffsetX = this.RightOffsetX + 1;
                this.LeftOffsetX = this.LeftOffsetX + 1;
            }
            this.SetProperties();
        }


        void button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.buttonPanning = false;
            this.ReleaseMouseCapture();
        }

        void button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.buttonPanning = true;
            double value = this.LeftOffsetX > this.RightOffsetX ? this.RightOffsetX : this.LeftOffsetX;
            prevScrollWidth = (e.GetPosition(this).X - this.AxesThickness.Left);
            this.CaptureMouse();
        }

        void TimeLineControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.EdgePointCorrection();
            MousehoverVisibility = System.Windows.Visibility.Collapsed;
            this.leftPanning = false;
            this.rightPanning = false;
            this.buttonPanning = false;
            this.ReleaseMouseCapture();
        }
        void ChartRangeIndicator_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.leftPanning && !this.rightPanning && !this.buttonPanning)
            {
                ChartAxis mousehoveraxis = null;
                foreach (ChartAxis axis in this.Axes)
                {
                    if (axis.VisibleInterval == this.MinimumTimeLineInterval)
                    {
                        mousehoveraxis = axis;
                    }
                }
                if (mousehoveraxis == null)
                {
                    mousehoveraxis = this.PrimaryAxis;
                }
                MousehoverWidth = this.ValueToPoint(mousehoveraxis, this.MinimumTimeLineInterval) - 17;
                MousehoverVisibility = System.Windows.Visibility.Visible;
                double value = this.PointToValue(this.PrimaryAxis, e.GetPosition(this)) % this.MinimumTimeLineInterval;
                this.MousehoverLeftCanvas = (e.GetPosition(this.leftGrid).X - this.ValueToPoint(this.PrimaryAxis, value)) + 17;
                if (this.GridWidth < this.MousehoverLeftCanvas)
                {
                    MousehoverVisibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                MousehoverVisibility = System.Windows.Visibility.Hidden;
            }
            if (this.leftPanning)
            {
                if (e.GetPosition(this.leftGrid).X > 0 && e.GetPosition(this.leftGrid).X < this.GridWidth)
                {
                    this.LeftOffsetX = e.GetPosition(this.leftGrid).X - this.ElementMargin.Left/2;
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double )
                     {
                        //if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) > this.PrimaryAxis.VisibleRange.Start && this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) < this.GridWidth)
                        {
                            TimeLineControl.SetStartValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                        }
                    }
                    else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime)
                    {
                        TimeLineControl.SetStartDate(this,DateTime.FromOADate( this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                this.SetProperties();
            }
            if (this.rightPanning)
            {
                if (e.GetPosition(this.leftGrid).X > 0 && e.GetPosition(this.leftGrid).X < this.GridWidth)
                {
                    this.RightOffsetX = e.GetPosition(this.rightGrid).X + this.ElementMargin.Right/2;
                     if (this.PrimaryAxis.ValueType == ChartValueType.Double )
                     {
                        //if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) > this.PrimaryAxis.VisibleRange.Start && this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) < this.GridWidth)
                        {
                            TimeLineControl.SetEndValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                        }
                    }
                    else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime  )
                    {
                        TimeLineControl.SetEndDate(this,DateTime.FromOADate( this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
                this.SetProperties();
            }
            if (this.buttonPanning)
            {
                double leftValue = this.LeftOffsetX < this.RightOffsetX ? this.LeftOffsetX : this.RightOffsetX;
                double rightValue = this.RightOffsetX > this.LeftOffsetX ? this.RightOffsetX : this.LeftOffsetX;
                if ((leftValue + ((e.GetPosition(this).X - this.AxesThickness.Left) - prevScrollWidth) >= 0) && (rightValue + ((e.GetPosition(this).X - this.AxesThickness.Left) - prevScrollWidth) < this.GridWidth))
                {
                    this.LeftOffsetX = this.LeftOffsetX + ((e.GetPosition(this).X - this.AxesThickness.Left) - prevScrollWidth);
                    this.RightOffsetX = this.RightOffsetX + ((e.GetPosition(this).X - this.AxesThickness.Left) - prevScrollWidth);
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) > this.PrimaryAxis.VisibleRange.Start && this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)) < this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double )
                    {
                        TimeLineControl.SetStartValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                    }
                    else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime  )
                    {
                    TimeLineControl.SetStartDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    }
                }
                if (this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) > this.PrimaryAxis.VisibleRange.Start && this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)) < this.GridWidth)
                {
                    if (this.PrimaryAxis.ValueType == ChartValueType.Double )
                    {
                       TimeLineControl.SetEndValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                    }
                    else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime  )
                    {
                    TimeLineControl.SetEndDate(this,DateTime.FromOADate( this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                    }
                }
                 if (this.PrimaryAxis.ValueType == ChartValueType.DateTime)
                {
                    TimeLineControl.SetStartDate(this,DateTime.FromOADate ( this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0))));
                    TimeLineControl.SetEndDate(this,DateTime.FromOADate(this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0))));
                }
                 else if (this.PrimaryAxis.ValueType == ChartValueType.Double )
                 {
                     TimeLineControl.SetStartValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.LeftOffsetX + 17, 0)));
                   TimeLineControl.SetEndValue(this, this.PointToValue(this.PrimaryAxis, new Point(this.RightOffsetX + 17, 0)));
                 }
                prevScrollWidth = (e.GetPosition(this).X - this.AxesThickness.Left);
                this.SetProperties();
            }

        }

        private void SetProperties()
        {
            this.OnViewPortChanged(new ViewPortChangedEventArgs(TimeLineControl.GetStartDate(this).ToOADate(), TimeLineControl.GetEndDate(this).ToOADate()));
            if (this.RightOffsetX > this.LeftOffsetX)
            {
                this.SelectedRegionWidth = this.RightOffsetX - (this.LeftOffsetX > 0 ? this.LeftOffsetX : 0);
                this.CanvasLeft = this.LeftOffsetX > 0 ? this.LeftOffsetX : 0;
                this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                this.CanvasRight = this.RightOffsetX;
                this.RightUnSelectedWidth = this.GridWidth - this.RightOffsetX;
            }
            else
            {
                this.SelectedRegionWidth = this.LeftOffsetX - (this.RightOffsetX > 0 ? this.RightOffsetX : 0);
                this.CanvasLeft = this.RightOffsetX > 0 ? this.RightOffsetX : 0;
                this.PathLeft = this.CanvasLeft + (this.SelectedRegionWidth / 2);
                this.CanvasRight = this.LeftOffsetX;
                this.RightUnSelectedWidth = this.GridWidth - this.LeftOffsetX;
            }
            foreach (ChartSeries series in this.Series)
            {
                series.IsIndexed = false;
            }
            //if (this.PrimarySeries != null)
            //    this.PrimarySeries.IsIndexed = false;

            if (this.PrimaryAxis != null)
            {
                if (PrimaryAxis.ValueType == ChartValueType.DateTime)
                {
                    this.SelectedRange = new DoubleRange(TimeLineControl.GetStartDate(this).ToOADate(), TimeLineControl.GetEndDate(this).ToOADate());
                }
                else if (PrimaryAxis.ValueType == ChartValueType.Double)
                {
                    this.SelectedRange = new DoubleRange(TimeLineControl.GetStartValue(this), TimeLineControl.GetEndValue(this));
                }
            }

            if (this.TimeLineSeries != null)
            {
                if (this.LeftOffsetX != this.RightOffsetX && this.SelectedRegionWidth > 1)
                {
                    Rect rect = new Rect(new Point(this.LeftOffsetX + 17, 0), new Point(this.RightOffsetX + 17, this.ActualHeight));
                    this.SelectedData = this.BoundsToDataSource(rect, this.TimeLineSeries);
                }
            }
          
            
        }

        void TimeLineControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            double start = this.PointToValue(this.PrimaryAxis, new Point(e.GetPosition(this).X - this.AxesThickness.Left, 0));
            double value = start % this.MinimumTimeLineInterval;
            // this.StartValue = start - value;
            // this.EndValue = this.StartValue + this.MinimumTimeLineInterval;
        }

        void ChartRangeIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.EdgePointCorrection();
            this.leftPanning = false;
            this.rightPanning = false;
            this.buttonPanning = false;
            this.isEgdelabelCorrectionEnabled = false;
            this.isLeftUnSelectClick = false;
            this.isRightUnselectClick = false;
            this.ReleaseMouseCapture();
        }

        void EdgePointCorrection()
        {
            if (this.EdgePointSelection)
            {
                this.isEgdelabelCorrectionEnabled = true;
                if(this.PrimaryAxis.ValueType==ChartValueType.Double )
                {
                  double value =TimeLineControl.GetStartValue(this) % this.MinimumTimeLineInterval;
                double end = TimeLineControl.GetEndValue(this) % this.MinimumTimeLineInterval;
                TimeLineControl.SetStartValue(this, TimeLineControl.GetStartValue(this)- value);
                TimeLineControl.SetEndValue(this,TimeLineControl.GetEndValue(this) - end != TimeLineControl.GetStartValue(this) ?TimeLineControl.GetEndValue(this) - end : TimeLineControl.GetStartValue(this) + this.MinimumTimeLineInterval);
                this.SetProperties();
                }
                else if (this.PrimaryAxis.ValueType == ChartValueType.DateTime )
                {
                    double value = TimeLineControl.GetStartDate(this).ToOADate() % this.MinimumTimeLineInterval;
                    double end = TimeLineControl.GetEndDate(this).ToOADate() % this.MinimumTimeLineInterval;
                    TimeLineControl.SetStartDate(this,DateTime.FromOADate(TimeLineControl.GetStartDate(this).ToOADate() - value));
                    TimeLineControl.SetEndDate(this, TimeLineControl.GetEndDate(this).ToOADate() - end != TimeLineControl.GetStartDate(this).ToOADate() ? DateTime.FromOADate(TimeLineControl.GetEndDate(this).ToOADate() - end) : DateTime.FromOADate(TimeLineControl.GetStartDate(this).ToOADate() + this.MinimumTimeLineInterval));
                    this.SetProperties();
                }
            }
        }

        void leftGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null && grid.Cursor == Cursors.SizeWE)
            {
                this.leftPanning = true;
            }
        }

        void leftGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Cursor = Cursors.SizeWE;
        }

        void rightGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null && grid.Cursor == Cursors.SizeWE)
            {
                this.rightPanning = true;
            }
        }

        void rightGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        /// Method Represents TimelineControl visualStyle Changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="args"></param>
        public static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {

            TimeLineControl timeline = d as TimeLineControl;

            if (timeline != null)
            {
                timeline.StyleIndexValue = (int)Enum.Parse(typeof(TimeLineStyles), args.NewValue.ToString());
                if (timeline.baseRD == null)
                {
                    timeline.baseRD = ChartDictionaries.GenericDictionary;
                    //timeline.baseRD = new SharedResourceDictionary()
                    //{
                    //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
                    //};
                }
                timeline.stylename = args.NewValue.ToString();
                Style style = timeline.baseRD[timeline.stylename] as Style;
                timeline.Style = style;
            }
        }

        #endregion


        /// <summary>
        /// Event for ViewPortChanged
        /// </summary>
        public event ViewPortChangedEventHandler ViewPortChanged;

        internal void OnViewPortChanged(ViewPortChangedEventArgs args)
        {
            if (ViewPortChanged != null)
            {
                ViewPortChanged(this, args);
            }
        }
    }

    /// <summary>
    /// Delegate for ViewPortChangedEventHandler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ViewPortChangedEventHandler(object sender, ViewPortChangedEventArgs e);


    /// <summary>
    /// class represents ViewPortChangedEventArgs
    /// </summary>
    public class ViewPortChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Contructor for ViewPortChangedEventArgs
        /// </summary>
        /// <param name="argsStart"></param>
        /// <param name="argsEnd"></param>
        public ViewPortChangedEventArgs(double argsStart, double argsEnd)
        {
            this.StartValue = argsStart;
            this.EndValue = argsEnd;
        }

        /// <summary>
        /// Get and Set the StartValue
        /// </summary>
        public double StartValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Get and Set EndValue
        /// </summary>
        public double EndValue
        {
            get;
            private set;
        }
    }

}