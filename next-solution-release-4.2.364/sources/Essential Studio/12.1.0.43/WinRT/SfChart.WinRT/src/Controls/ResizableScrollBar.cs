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
using System.Net;
using System.Windows;
#if NETFX_CORE
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#else
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif
namespace Syncfusion.UI.Xaml.Charts
{

    [TemplatePartAttribute(Name = "VerticalRoot", Type = typeof(FrameworkElement))]
    [TemplatePartAttribute(Name = "HorizontalRoot", Type = typeof(FrameworkElement))]
    [TemplatePartAttribute(Name = "HorizontalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "HorizontalThumb", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "HorizontalThumbHand1", Type = typeof(Popup))]
    [TemplatePartAttribute(Name = "HorizontalThumbHand2", Type = typeof(Popup))]
    [TemplatePartAttribute(Name = "VerticalLargeIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalLargeDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalSmallIncrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalSmallDecrease", Type = typeof(RepeatButton))]
    [TemplatePartAttribute(Name = "VerticalThumb", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "VerticalThumbHand1", Type = typeof(Thumb))]
    [TemplatePartAttribute(Name = "VerticalThumbHand2", Type = typeof(Thumb))]
    [TemplateVisualStateAttribute(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualStateAttribute(Name = "OnFocus", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnLostFocus", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnExit", GroupName = "TouchMode")]
    [TemplateVisualStateAttribute(Name = "OnView", GroupName = "TouchMode")]
    public class ResizableScrollBar : ContentControl
    {

        #region Constructor

        public ResizableScrollBar()
        {
            this.DefaultStyleKey = typeof(ResizableScrollBar);
        }

        #endregion

        #region Events
        public event EventHandler ValueChanged;
        #endregion

        #region Properties

       // Using a DependencyProperty as the backing store for Orientation.
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ResizableScrollBar), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizableScrollBar).OnOrientationChanged(e);
        }

        protected virtual void OnOrientationChanged(DependencyPropertyChangedEventArgs e)
        {
            ApplyOrientationTemplate();
        }
       
        /// <summary>
        /// Gets or Sets the Orientation for the Scroll Bar
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Maximum.
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(1d));
        
        /// <summary>
        /// Gets or Sets Maximum Value for Scroll Bar
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(0d));
        
        /// <summary>
        /// Gets or Sets Minimum Value for Scroll Bar
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewSizePort.
        public static readonly DependencyProperty ViewSizePortProperty = DependencyProperty.Register("ViewSizePort", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(0d));
        
        /// <summary>
        /// Gets or Sets ViewSizePort Value for Scroll Bar
        /// </summary>
        public double ViewSizePort
        {
            get { return (double)GetValue(ViewSizePortProperty); }
            set { SetValue(ViewSizePortProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallChange.
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(0.01d));

        /// <summary>
        /// Gets or Sets SmallChange Value for Scroll Bar Thumb Change When the Small Increase and Decrease Button is Clicked.
        /// </summary>
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeChange.
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(0.1d));

        /// <summary>
        /// Gets or Sets LargeChange Value for Scroll Bar Thumb Change When the Large Increase and Decrease Button is Clicked.
        /// </summary>
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(1d));
        
        /// <summary>
        /// Gets a value that determines how far the scroll content is scaled. 
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeStart.
        public static readonly DependencyProperty RangeStartProperty = DependencyProperty.Register("RangeStart", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(0d,OnRangeChanged));

        private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizableScrollBar).OnRangeValueChanged();
        }

        protected virtual void OnRangeValueChanged()
        {
            RangeMinMax();
            InvalidateArrange();
            if(IsValueChangedTrigger)
                OnValueChanged();
            IsValueChangedTrigger = (canDrag)? false : true;
         }

        /// <summary>
        /// Gets or Sets RangeStart Value for Scroll Bar.
        /// </summary>
        public double RangeStart
        {
            get { return (double)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeEnd.
        public static readonly DependencyProperty RangeEndProperty = DependencyProperty.Register("RangeEnd", typeof(double), typeof(ResizableScrollBar), new PropertyMetadata(1d, OnRangeChanged));

        /// <summary>
        /// Gets or Sets RangeEnd Value for Scroll Bar.
        /// </summary>
        public double RangeEnd
        {
            get { return (double)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollButtonVisibility.
        public static readonly DependencyProperty ScrollButtonVisibilityProperty = DependencyProperty.Register("ScrollButtonVisibility", typeof(Visibility), typeof(ResizableScrollBar), new PropertyMetadata(Visibility.Visible,OnIncreaseDecreaseVisibilityChanged));

        private static void OnIncreaseDecreaseVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResizableScrollBar).MeasureOverride((d as ResizableScrollBar).AvailabeSize);
        }
        
        /// <summary>
        /// Gets or Sets the visibility of scroll buttons.
        /// </summary>
        public Visibility ScrollButtonVisibility
        {
            get { return (Visibility)GetValue(ScrollButtonVisibilityProperty); }
            set { SetValue(ScrollButtonVisibilityProperty, value); }
        }
        /// <summary>
        /// Enable or Disable the EnableTouchMode.
        /// </summary>
        public bool EnableTouchMode
        {
            get { return (bool)GetValue(EnableTouchModeProperty); }
            set { SetValue(EnableTouchModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTouchMode. This enables the touch mode for the resizable scroll bar.
        public static readonly DependencyProperty EnableTouchModeProperty =
            DependencyProperty.Register("EnableTouchMode", typeof(bool), typeof(ResizableScrollBar), new PropertyMetadata(false));

        internal protected bool IsValueChangedTrigger { get; set; }

        internal Size AvailabeSize { get { return availableSize; }  }

        internal double TrackSize { get { return actualTrackSize;} }

        protected Thumb NearHand { get; set; }

        protected Thumb FarHand { get; set; }

        protected Thumb MiddleThumb { get; set; }

        protected RepeatButton SmallDecrease { get; set; }

        protected RepeatButton LargeDecrease { get; set; }

        protected RepeatButton LargeIncrease { get; set; }

        protected RepeatButton SmallIncrease { get; set; }

        public double ResizableThumbSize { get { return resizeThumbSize; } }

       #endregion

        #region Fields

        Grid horizontalRoot, verticalRoot;
        Size desiredSize, availableSize;
        double resizeThumbSize, smallThumbSize, actualTrackSize, actualSize, previousThumbSize, rangeDiff;
        double middleThumbSize, largeDecreaseThumbSize, largeIncreaseThumbSize, actualLargeThumbSize;
        internal bool canDrag = false;
        const double GapSize = 4, MinimumThumbSize = 0, ResizableBarSize = 15, MinimumDiff = 0,optimizeEnd=1.5;
        public bool isFarDragged = false;
        public bool isNearDragged = false;
        #endregion

        #region Methods
        #if NETFX_CORE
        protected override void OnApplyTemplate()
        #else
        public override void OnApplyTemplate()
        #endif
        {
            ApplyOrientationTemplate();
        }

        private void ApplyOrientationTemplate()
        {
            horizontalRoot = this.GetTemplateChild("HorizontalRoot") as Grid;
            verticalRoot = this.GetTemplateChild("VerticalRoot") as Grid;

            if (LargeIncrease != null)
                LargeIncrease.Click -= OnLargeIncreaseClick;
            if(LargeDecrease != null)
                LargeDecrease.Click -= OnLargeDecreaseClick;
            if (SmallDecrease != null)
                SmallDecrease.Click -= OnSmallDecreaseClick;
            if(SmallIncrease != null)
                SmallIncrease.Click -= OnSmallIncreaseClick;
            if(NearHand != null)
                NearHand.DragDelta -= OnNearHandDragged;

            if(FarHand != null)
                FarHand.DragDelta -= OnFarHandDragged;

            if(MiddleThumb != null)
                MiddleThumb.DragDelta -= OnThumbDragged;

            if (this.Orientation == Orientation.Horizontal && horizontalRoot != null)
            {
                horizontalRoot.Visibility = Visibility.Visible;
                verticalRoot.Visibility = Visibility.Collapsed;
                NearHand = this.GetTemplateChild("HorizontalThumbHand1") as Thumb;
                FarHand = this.GetTemplateChild("HorizontalThumbHand2") as Thumb;
                MiddleThumb = this.GetTemplateChild("HorizontalThumb") as Thumb;
                SmallDecrease = this.GetTemplateChild("HorizontalSmallDecrease") as RepeatButton;
                LargeDecrease = this.GetTemplateChild("HorizontalLargeDecrease") as RepeatButton;
                LargeIncrease = this.GetTemplateChild("HorizontalLargeIncrease") as RepeatButton;
                SmallIncrease = this.GetTemplateChild("HorizontalSmallIncrease") as RepeatButton;
                NearHand.DragDelta += OnNearHandDragged;
                FarHand.DragDelta += OnFarHandDragged;
                MiddleThumb.DragDelta += OnThumbDragged;
                LargeIncrease.Click += OnLargeIncreaseClick;
                LargeDecrease.Click += OnLargeDecreaseClick;
                SmallDecrease.Click += OnSmallDecreaseClick;
                SmallIncrease.Click += OnSmallIncreaseClick;
            }
            else if(verticalRoot != null)
            {
                horizontalRoot.Visibility = Visibility.Collapsed;
                verticalRoot.Visibility = Visibility.Visible;
                NearHand = this.GetTemplateChild("VerticalThumbHand1") as Thumb;
                FarHand = this.GetTemplateChild("VerticalThumbHand2") as Thumb;
                MiddleThumb = this.GetTemplateChild("VerticalThumb") as Thumb;
                SmallDecrease = this.GetTemplateChild("VerticalSmallDecrease") as RepeatButton;
                LargeDecrease = this.GetTemplateChild("VerticalLargeDecrease") as RepeatButton;
                LargeIncrease = this.GetTemplateChild("VerticalLargeIncrease") as RepeatButton;
                SmallIncrease = this.GetTemplateChild("VerticalSmallIncrease") as RepeatButton;
                NearHand.DragDelta += OnNearHandDragged;
                FarHand.DragDelta += OnFarHandDragged;
                MiddleThumb.DragDelta += OnThumbDragged;
                LargeIncrease.Click += OnLargeIncreaseClick;
                LargeDecrease.Click += OnLargeDecreaseClick;
                SmallDecrease.Click += OnSmallDecreaseClick;
                SmallIncrease.Click += OnSmallIncreaseClick;
            }
            IsValueChangedTrigger = true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            this.availableSize = ChartLayoutUtils.CheckSize(availableSize);
            if (this.Orientation == Orientation.Horizontal && NearHand!=null && FarHand != null)
            {
               desiredSize.Width = this.availableSize.Width;
               desiredSize.Height = ResizableBarSize;
               if (NearHand.Visibility != Visibility.Collapsed)
               {
                   NearHand.Measure(availableSize);
                   Size thumbSize = NearHand.DesiredSize;
                   if (thumbSize.Width == 0)
                       resizeThumbSize = NearHand.MinWidth;
                   else
                       resizeThumbSize = thumbSize.Width;
                   NearHand.Width = resizeThumbSize;
                   FarHand.Width = resizeThumbSize;
               }
               else
               {
                   resizeThumbSize = 0d;
               }
               if (this.ScrollButtonVisibility != Visibility.Collapsed)
               {
                   SmallIncrease.Measure(availableSize);
                   Size scrollButtonSize = SmallIncrease.DesiredSize;
                   if (scrollButtonSize.Width == 0)
                       smallThumbSize = SmallIncrease.MinWidth;
                   else
                       smallThumbSize = (smallThumbSize == 0) ? scrollButtonSize.Width : Math.Min(smallThumbSize, scrollButtonSize.Width);
                   SmallDecrease.Width = smallThumbSize;
                   SmallIncrease.Width = smallThumbSize;
               }
               else
               {
                   SmallDecrease.Width = smallThumbSize = 0;
                   SmallIncrease.Width = smallThumbSize = 0;
               }
            }
            else if (this.Orientation == Orientation.Vertical && FarHand!=null && NearHand != null)
            {
               desiredSize.Height =  this.availableSize.Height;
               desiredSize.Width = ResizableBarSize;
               NearHand.Measure(availableSize);
               if (NearHand.Visibility != Visibility.Collapsed)
               {
                   Size thumbSize = NearHand.DesiredSize;
                   if (thumbSize.Height == 0)
                       resizeThumbSize = NearHand.MinHeight;
                   else
                       resizeThumbSize = thumbSize.Height;
                   NearHand.Height = resizeThumbSize;
                   FarHand.Height = resizeThumbSize;
               }
               else
               {
                   resizeThumbSize = 0d;
               }
               if (this.ScrollButtonVisibility != Visibility.Collapsed)
               {
                   SmallIncrease.Measure(availableSize);
                   Size scrollButtonSize = SmallIncrease.DesiredSize;
                   if (scrollButtonSize.Width == 0)
                       smallThumbSize = SmallIncrease.MinHeight;
                   else
                       smallThumbSize = smallThumbSize = (smallThumbSize == 0) ? scrollButtonSize.Height : Math.Min(smallThumbSize, scrollButtonSize.Height);
                   SmallDecrease.Height = smallThumbSize;
                   SmallIncrease.Height = smallThumbSize;
               }
               else
               {
                   SmallDecrease.Height = smallThumbSize = 0;
                   SmallIncrease.Height = smallThumbSize = 0;
               }
            }
            return desiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            actualSize = (this.Orientation == Orientation.Horizontal) ? finalSize.Width : finalSize.Height;
            actualTrackSize = actualSize - (2 * smallThumbSize) - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize);
            CalculateSize();
            actualLargeThumbSize = this.EnableTouchMode ? actualSize : actualTrackSize;
            ThumbMinMax();
            rangeDiff = rangeDiff == 0 ? SmallChange : rangeDiff;
            if (this.Orientation == Orientation.Horizontal)
            {
                MiddleThumb.Width = middleThumbSize;
                LargeDecrease.Width = largeDecreaseThumbSize;
                LargeIncrease.Width = largeIncreaseThumbSize;
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                 MiddleThumb.Height = finalSize.Height;
#endif
#if NETFX_CORE
                LargeDecrease.Height = finalSize.Height;
                LargeIncrease.Height = finalSize.Height;
#endif
            }
            else if (this.Orientation == Orientation.Vertical)
            {
                MiddleThumb.Height = middleThumbSize;
                LargeDecrease.Height = largeDecreaseThumbSize;
                LargeIncrease.Height = largeIncreaseThumbSize;
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                MiddleThumb.Width = finalSize.Width;
#endif
#if NETFX_CORE
                LargeDecrease.Width = finalSize.Width;
                LargeIncrease.Width = finalSize.Width;
#endif
            }
          
            return finalSize;
        }


        private void OnSmallIncreaseClick(object sender, RoutedEventArgs e)
        {
           if (RangeEnd != Maximum)
           {
              IncreaseDecreaseOperation(SmallChange, SmallChange);
           }
        }

        private void OnSmallDecreaseClick(object sender, RoutedEventArgs e)
        {
            if (RangeStart!= Minimum)
            {
               IncreaseDecreaseOperation(-SmallChange, -SmallChange);
            }
        }

        private void OnLargeDecreaseClick(object sender, RoutedEventArgs e)
        {
            if (!(RangeStart == Minimum))
            {
                IncreaseDecreaseOperation(-LargeChange, -LargeChange);
            }
        }

        private void OnLargeIncreaseClick(object sender, RoutedEventArgs e)
        {
            if (!(RangeEnd == Maximum))
            {
                IncreaseDecreaseOperation(LargeChange, LargeChange);
            }
        }

        protected virtual void OnThumbDragged(object sender, DragDeltaEventArgs e)
        {
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;
            if (!(RangeStart == Minimum && RangeEnd == Maximum) && thumbSize != MinimumThumbSize)
            {
                CalculateRangeDifference();
                double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
                if (change < 0 && RangeStart != Minimum)
                    canDrag = true;
                else if (change > 0 && RangeEnd != Maximum)
                    canDrag = true;
                if (canDrag)
                {
                    double newChange = (change * (Maximum - Minimum)) / actualTrackSize;
                    IsValueChangedTrigger = false;
                    CalculateChange(newChange, newChange);
                    RangeStart = (RangeStart > Maximum - rangeDiff) ? Maximum - rangeDiff : RangeStart;
                    RangeEnd = (RangeEnd < rangeDiff) ? rangeDiff : RangeEnd;
                    isFarDragged = false;
                    isNearDragged = false;
                    OnValueChanged();
                    CalculateOperations();
                }
            }
        }

        protected virtual void OnFarHandDragged(object sender, DragDeltaEventArgs e)
        {
            double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
            change = ((actualTrackSize) * change) / (actualTrackSize - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize));
            double newChange = (change * (Maximum - Minimum)) / actualSize;
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;

            bool canChange = false;

            if (RangeEnd + newChange <= Maximum)
            {
                if (((RangeEnd + newChange) - RangeStart) >= (MinimumDiff * SmallChange))
                {
                    canChange = true;
                }
            }
            else if (RangeEnd < Maximum)
            {
                RangeEnd = Maximum;
                canChange = true;
            }

            if (canChange)
            {
                if (thumbSize != MinimumThumbSize || change > 0)
                {
                    IsValueChangedTrigger = false;
                    rangeDiff = (MinimumDiff * SmallChange);
                    CalculateChange(0, newChange);
                    Scale = (Maximum - Minimum) / (RangeEnd - RangeStart);
                    CalculateOperations();
                    isFarDragged = true;
                    isNearDragged = false;
                    OnValueChanged();
                }
            }
        }

        protected virtual void OnNearHandDragged(object sender, DragDeltaEventArgs e)
        {
            double change = (this.Orientation == Orientation.Horizontal) ? e.HorizontalChange : -e.VerticalChange;
            change = ((actualTrackSize) * change) / (actualTrackSize - (this.EnableTouchMode ? 0 : 2 * resizeThumbSize));
            double newChange = (change * (Maximum - Minimum)) / actualSize;
            double thumbSize = (this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height;
            bool canChange= false;

            if (RangeStart + newChange >= Minimum)
            {
                if ((RangeEnd - (RangeStart + newChange)) >= (MinimumDiff * SmallChange))
                {
                    canChange = true;
                }
            }
            else if (RangeStart > Minimum)
            {
                RangeStart = Minimum;
                canChange = true;
            }


            if (canChange)
            {
                if (thumbSize != MinimumThumbSize || change < 0)
                {
                    IsValueChangedTrigger = false;
                    rangeDiff = (MinimumDiff * SmallChange);
                    CalculateChange(newChange, 0);
                    Scale = (Maximum - Minimum) / (RangeEnd - RangeStart);
                    CalculateOperations();
                    isFarDragged = false;
                    isNearDragged = true;
                    OnValueChanged();
                 }
            }
        }

        private void CalculateSize()
        {
            middleThumbSize = ((actualTrackSize) * (((RangeEnd - RangeStart) / (Maximum - Minimum)))) - ((this.ScrollButtonVisibility != Visibility.Collapsed) ? GapSize : 0) ;
            largeDecreaseThumbSize = (actualTrackSize) * ((RangeStart - Minimum) / (Maximum - Minimum));
            largeIncreaseThumbSize = ((actualTrackSize) * ((Maximum - RangeEnd) / (Maximum - Minimum)));
        }

        private void ThumbMinMax()
        {
            middleThumbSize = middleThumbSize < MinimumThumbSize ? MinimumThumbSize : ((middleThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : middleThumbSize);
            largeDecreaseThumbSize = largeDecreaseThumbSize <= 0 ? 0 : ((largeDecreaseThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : largeDecreaseThumbSize);
            largeIncreaseThumbSize = largeIncreaseThumbSize <= 0 ? 0 : ((largeIncreaseThumbSize > actualLargeThumbSize) ? actualLargeThumbSize : largeIncreaseThumbSize);
        }

        private void CalculateChange(double startChange, double endChange)
        {
            RangeStart += startChange;
            RangeEnd += endChange;
            RangeMinMax();
        }

        private void RangeMinMax()
        {
            RangeStart = (RangeStart < Minimum) ? Minimum : (RangeStart > Maximum - (Maximum / 100) && this.ScrollButtonVisibility != Visibility.Collapsed) ? Maximum - (Maximum / 100) : (EnableTouchMode && RangeStart > Maximum - (1.5 * SmallChange)) ? Maximum - (1.5 * SmallChange) : RangeStart;
            RangeEnd = (RangeEnd > Maximum) ? Maximum : ((RangeEnd < Minimum && this.ScrollButtonVisibility == Visibility.Collapsed) ? Minimum : (RangeEnd < (Maximum / 100) && this.ScrollButtonVisibility == Visibility.Visible) ? (Maximum / 100) : RangeEnd);
        }

        private void SetLargeThumbSize()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                MiddleThumb.Width = middleThumbSize;
                LargeDecrease.Width = largeDecreaseThumbSize;
                LargeIncrease.Width = largeIncreaseThumbSize;
            }
            else
            {
                MiddleThumb.Height = middleThumbSize;
                LargeDecrease.Height = largeDecreaseThumbSize;
                LargeIncrease.Height = largeIncreaseThumbSize;
            }
        }

        private void CalculateRangeDifference()
        {
            if (previousThumbSize != ((this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height))
            {
                previousThumbSize = ((this.Orientation == Orientation.Horizontal) ? MiddleThumb.Width : MiddleThumb.Height);
                rangeDiff = RangeEnd - RangeStart;
            }
        }

        private void IncreaseDecreaseOperation(double startChange, double endChange)
        {
                canDrag = true;
                IsValueChangedTrigger = false;
                CalculateRangeDifference();
                CalculateChange(startChange, endChange);
                RangeStart = (RangeStart > Maximum - rangeDiff) ? Maximum - rangeDiff : RangeStart;
                RangeEnd = (RangeEnd < rangeDiff) ? rangeDiff : RangeEnd;
                CalculateOperations();
                OnValueChanged();
            
        }

        private void CalculateOperations()
        {
            CalculateSize();
            ThumbMinMax();
            SetLargeThumbSize();
        }

        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                this.ValueChanged(this, EventArgs.Empty);
                IsValueChangedTrigger = true;
            }
                canDrag = false;
        }
    
        #endregion

    }
}

