// <copyright file="RangeSlider.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#if !(WPFSILVERLIGHT||WINDOWS_PHONE_7)
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
#endif
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Syncfusion.Licensing;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Input
#elif WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework.Input;
using System.Collections.ObjectModel;

namespace Syncfusion.WP.Controls.Input
#elif SILVERLIGHT
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
namespace Syncfusion.Tools.Controls.Input
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using System.Collections.ObjectModel;

namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// RangeSlider is a <see
    /// cref="T:Windows.UI.Xaml.Controls.Primitives.RangeBase"/> that lets the user
    /// select from a range of values by moving a <see
    /// cref="T:Windows.UI.Xaml.Controls.Primitives.Thumb"/> control along a track.
    /// </summary>
    /// <remarks>
    /// Range Slider control allows the user to select the range of value within the
    /// specified minimum and maximum limit. The range can be selected by moving the
    /// Thumb control along a track.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfRangeSlider : RangeBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/> class.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public SfRangeSlider()
        {
#if WPF
            if (EnvironmentTestInput.IsSecurityGranted)
            {
                EnvironmentTestInput.StartValidateLicense(typeof(SfRangeSlider));
            }
#endif
            DefaultStyleKey = typeof(SfRangeSlider);
            this.Loaded += RangeSlider_Loaded;
            this.Unloaded += RangeSlider_Unloaded;
            this.IsEnabledChanged += SfRangeSlider_IsEnabledChanged;
            this.ValueChanged += RangeSlider_ValueChanged;
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            this.MouseLeftButtonDown+=RangeSlider_MouseLeftButtonDown;
            AddHandler(MouseLeftButtonDownEvent,(MouseButtonEventHandler)RangeSlider_MouseLeftButtonDown, true);
            MousePoint = new Point();
#elif WINRT
            this.PointerPressed += RangeSlider_PointerPressed;
            AddHandler(PointerPressedEvent, (PointerEventHandler)RangeSlider_PointerPressed, true);
         
#endif
            this.SizeChanged += RangeSlider_SizeChanged;
        }

        void SfRangeSlider_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        void UpdateVisualState()
        {
            if (IsEnabled)
                VisualStateManager.GoToState(this, "Normal", true);
            else
                VisualStateManager.GoToState(this, "Disabled", true);
        }

        void RangeSlider_Unloaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = false;
            this.IsEnabledChanged-=SfRangeSlider_IsEnabledChanged;
        }
      
        #endregion

        #region Variables
#if WPF
        private new bool IsLoaded;
        private bool _canOpen;
#else
        private bool IsLoaded;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private bool _canOpen;
#endif
#endif

        private Point MousePoint;
#if !WPF && !SILVERLIGHT && !WINDOWS_PHONE && !WINDOWS_PHONE_7
        private PointerPoint MousePointerPoint;
#endif
        private bool isMouseEnter;

#if WINRT
        private Border PART_BorderThumb;
#endif

        private Rectangle horizontalDecreaseRect, verticalDecreaseRect;

        private Rectangle horizontalTrackRect, verticalTrackRect;

        private Thumb horizontalRangeEndThumb, verticalRangeEndThumb;

        private Thumb horizontalRangeStartThumb, verticalRangeStartThumb;

        private TranslateTransform horizontalRangeStartTransform, verticalRangeStartTransform;

        private TranslateTransform horizontalRangeEndTransform, verticalRangeEndTransform;

        private ScaleTransform horizontalScale, verticalScale;

        private bool isPointerDown, isDragDelta, isPointerMove, isRangeEnd, showTick;

        private Grid horizontalTemplate, verticalTemplate;

        private double stepValue = 0d;

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        ToolTip horizontalRangeStartThumbToolTip, verticalRangeStartThumbToolTip;

        ToolTip horizontalRangeEndThumbToolTip, verticalRangeEndThumbToolTip;
#endif

#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        Point startPoint;
#elif WINRT
        PointerPoint startPoint;
#endif
        TickBar TopTickBar, HorizontalInlineTickBar, BottomTickBar, LeftTickBar, VerticalInlineTickBar, RightTickBar;

        bool isFirstThumbFocused, isSecondThumbFocused;

        bool isValueChangedInternally, isRangeStartChangedInternally, isRangeEndChangedInternally;

        bool isValueChangedExternally, isRangeStartChangedExternally, isRangeEndChangedExternally;

        internal double ValueFactor
        {
            get
            {
                if (Maximum - Minimum > 0)
                {
                    if(IntermediateValue != 0d)
                        return (IntermediateValue - Minimum) / (Maximum - Minimum);
                    else
                        return (Value - Minimum) / (Maximum - Minimum);
                }
                else
                    return 0d;
            }
        }

        internal double ToolTipDistance
        {
            get
            {
                return 10d;
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value that indicates the dimension for <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// It will accepts the type is <see
        /// cref="N:Windows.UI.Xaml.Controls.Orientation"/>. The default value is <see
        /// cref="N:Windows.UI.Xaml.Controls.Orientation.Horizontal">Orientation.Horizontal</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SfRangeSlider), new PropertyMetadata(Orientation.Horizontal,new PropertyChangedCallback(OnOrientationChanged)));

        /// <summary>
        /// Gets or sets the direction of increasing value with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <remarks>
        /// The default is false.
        /// </remarks>
        /// <value>
        /// <c>true</c> if this instance is direction reversed; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool IsDirectionReversed
        {
            get { return (bool)GetValue(IsDirectionReversedProperty); }
            set { SetValue(IsDirectionReversedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDirectionReversed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDirectionReversedProperty =
            DependencyProperty.Register("IsDirectionReversed", typeof(bool), typeof(SfRangeSlider), new PropertyMetadata(false,OnIsDirectionReversed));

        /// <summary>
        /// Gets or sets a value indicating whether to show the range between the thumb with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        /// <value>
        /// <c>true</c> it show the range; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public bool ShowRange
        {
            get { return (bool)GetValue(ShowRangeProperty); }
            set { SetValue(ShowRangeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowRange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowRangeProperty =
            DependencyProperty.Register("ShowRange", typeof(bool), typeof(SfRangeSlider), new PropertyMetadata(false, OnShowRangeChanged));

        /// <summary>
        /// Gets or sets a value indicating whether to change thumb position <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <remarks>
        /// The default value is None.
        /// </remarks>
        /// <value>
        /// <c>true</c> it show the range; otherwise, <c>false</c>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public MovePoint MoveToPoint
        {
            get { return (MovePoint)GetValue(MoveToPointProperty); }
            set { SetValue(MoveToPointProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowRange.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MoveToPointProperty =
            DependencyProperty.Register("MoveToPoint", typeof(MovePoint), typeof(SfRangeSlider),
                                        new PropertyMetadata(MovePoint.MoveToTapPosition, OnMovePointChanged));

        /// <summary>
        /// Gets the intermediate <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeStart"/> value.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeStart"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeEnd"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeEnd"/>
        [ClassReference(IsReviewed = false)]
        public double IntermediateRangeStart
        {
            get { return (double)GetValue(IntermediateRangeStartProperty); }
            internal set { SetValue(IntermediateRangeStartProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeStart.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntermediateRangeStartProperty =
            DependencyProperty.Register("IntermediateRangeStart", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d, OnIntermediateRangeStartChanged));

        /// <summary>
        /// Gets the intermediate <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeEnd"/> value.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeStart"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeStart"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeEnd"/>
        [ClassReference(IsReviewed = false)]
        public double IntermediateRangeEnd
        {
            get { return (double)GetValue(IntermediateRangeEndProperty); }
            internal set { SetValue(IntermediateRangeEndProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeEnd.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntermediateRangeEndProperty =
            DependencyProperty.Register("IntermediateRangeEnd", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d, OnIntermediateRangeEndChanged));



        /// <summary>
        /// Gets or sets the start value of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeEnd"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeEnd"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeStart"/>
        [ClassReference(IsReviewed = false)]
        public double RangeStart
        {
            get { return (double)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeStart.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register("RangeStart", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d,OnRangeStartChanged));


        /// <summary>
        /// Gets or sets the end value of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeStart"/>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.RangeStart"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeEnd"/>
        [ClassReference(IsReviewed = false)]
        public double RangeEnd
        {
            get { return (double)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RangeEnd.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register("RangeEnd", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d,OnRangeEndChanged));
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.ThumbToolTipPlacement"/> mode to place
        /// the tool tip with <see cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// The default value is  <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Input.ThumbToolTipPlacement.TopLeft">ThumbToolTipPlacement.TopLeft</see>.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.ThumbToolTipPrecision"/>
        [ClassReference(IsReviewed = false)]
        public ThumbToolTipPlacement ThumbToolTipPlacement
        {
            get { return (ThumbToolTipPlacement)GetValue(ThumbTooltipPlacementProperty); }
            set { SetValue(ThumbTooltipPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThumbTooltipPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThumbTooltipPlacementProperty =
            DependencyProperty.Register("ThumbTooltipPlacement", typeof(ThumbToolTipPlacement), typeof(SfRangeSlider), new PropertyMetadata(ThumbToolTipPlacement.TopLeft, OnThumbToolTipPlacementChanged));

        /// <summary>
        /// Gets or set the precision of value to be displayed in the tooltip with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.ThumbToolTipPlacement"/>
        [ClassReference(IsReviewed = false)]
        public int ThumbToolTipPrecision
        {
            get { return (int)GetValue(ThumbToolTipPrecisionProperty); }
            set { SetValue(ThumbToolTipPrecisionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThumbToolTipPrecision.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThumbToolTipPrecisionProperty =
            DependencyProperty.Register("ThumbToolTipPrecision", typeof(int), typeof(SfRangeSlider), new PropertyMetadata(0));
#endif
        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.SliderSnapsTo"/> that determines
        /// whether the <see cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/> snaps
        /// to steps or ticks.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Input.SliderSnapsTo.StepValues">SliderSnapsTo.StepValues</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public SliderSnapsTo SnapsTo
        {
            get { return (SliderSnapsTo)GetValue(SnapsToProperty); }
            set { SetValue(SnapsToProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SnapsTo.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SnapsToProperty =
            DependencyProperty.Register("SnapsTo", typeof(SliderSnapsTo), typeof(SfRangeSlider), new PropertyMetadata(SliderSnapsTo.StepValues));

        /// <summary>
        /// Gets or sets the value used to specify the interval between snap points when the
        /// <see cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.SnapsTo"/> is set to
        /// <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Input.SliderSnapsTo.StepValues">SliderSnapsTo.StepValues</see>.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.TickFrequency"/>
        [ClassReference(IsReviewed = false)]
        public double StepFrequency
        {
            get { return (double)GetValue(StepFrequencyProperty); }
            set { SetValue(StepFrequencyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StepFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StepFrequencyProperty =
            DependencyProperty.Register("StepFrequency", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(1.0d));

        /// <summary>
        /// Gets or sets the value is used to define the number of ticks along the track,
        /// based on Minimum and Maximum values.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        /// <seealso cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.StepFrequency"/>
        [ClassReference(IsReviewed = false)]
        public double TickFrequency
        {
            get { return (double)GetValue(TickFrequencyProperty); }
            set { SetValue(TickFrequencyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d));

        /// <summary>
        /// Gets the intermediate value. This works when <see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.ShowRange"/> is false.
        /// </summary>
        /// <value>
        /// The default value is zero.
        /// </value>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeEnd"/>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Controls.Input.RangeSlider.IntermediateRangeStart"/>
        [ClassReference(IsReviewed = false)]
        public double IntermediateValue
        {
            get { return (double)GetValue(IntermediateValueProperty); }
            internal set { SetValue(IntermediateValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntermediateValue.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntermediateValueProperty =
            DependencyProperty.Register("IntermediateValue", typeof(double), typeof(SfRangeSlider), new PropertyMetadata(0d,OnIntermediateValueChanged));

        /// <summary>
        /// Gets or sets the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.TickPlacement"/> that determine where
        /// to draw tick marks in relation to the track with <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/>.
        /// </summary>
        /// <value>
        /// The default value is <see
        /// cref="F:Syncfusion.UI.Xaml.Controls.Input.TickPlacement.Inline">TickPlacement.Inline</see>.
        /// </value>
        [ClassReference(IsReviewed = false)]
        public TickPlacement TickPlacement
        {
            get { return (TickPlacement)GetValue(TickPlacementProperty); }
            set { SetValue(TickPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(SfRangeSlider), new PropertyMetadata(TickPlacement.Inline,OnTickPlacementChanged));




        /// <summary>
        /// Gets or sets the Label placement
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.LabelPlacement"/>
        /// </summary>
        /// <remarks> 
        /// The default value is <see cref="T:Syncfusion.UI.Xaml.Controls.Input.LabelPlacement.TopLeft"/> 
        /// </remarks>
        public LabelPlacement LabelPlacement
        {
            get { return (LabelPlacement)GetValue(LabelPlacementProperty); }
            set { SetValue(LabelPlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelPlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelPlacementProperty =
            DependencyProperty.Register("LabelPlacement", typeof(LabelPlacement), typeof(SfRangeSlider), new PropertyMetadata(LabelPlacement.TopLeft,OnLabelPlacementChanged));


        /// <summary>
        /// Gets or sets the value placement
        /// <see cref="T:Syncfusion.UI.Xaml.Controls.Input.ValuePlacement"/>
        /// </summary>
        /// <remarks> 
        /// The default value is <see cref="T:Syncfusion.UI.Xaml.Controls.Input.ValuePlacement.BottomRight"/> 
        /// </remarks>
        public ValuePlacement ValuePlacement
        {
            get { return (ValuePlacement)GetValue(ValuePlacementProperty); }
            set { SetValue(ValuePlacementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ValuePlacement.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValuePlacementProperty =
            DependencyProperty.Register("ValuePlacement", typeof(ValuePlacement), typeof(SfRangeSlider), new PropertyMetadata(ValuePlacement.BottomRight,OnValuePlacementChanged));

        

        /// <summary>
        /// Returns a value if set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>The default value is false</remarks>
        public bool ShowValueLabels
        {
            get { return (bool)GetValue(ShowValueLabelsProperty); }
            set { SetValue(ShowValueLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowValueLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowValueLabelsProperty =
            DependencyProperty.Register("ShowValueLabels", typeof(bool), typeof(SfRangeSlider), new PropertyMetadata(false,OnShowValueLabelsChanged));


        /// <summary>
        /// Returns a value if set
        /// </summary>
        /// <value>
        /// <c>true</c> if instance is created ; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>The default value is false</remarks>
        public bool ShowCustomLabels
        {
            get { return (bool)GetValue(ShowCustomLabelsProperty); }
            set { SetValue(ShowCustomLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowCustomLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowCustomLabelsProperty =
            DependencyProperty.Register("ShowCustomLabels", typeof(bool), typeof(SfRangeSlider), new PropertyMetadata(false,OnShowCustomLabelsChanged));


        /// <summary>
        /// Gets or sets the collection of custm labels
        /// </summary>
        public ObservableCollection<Items> CustomLabels
        {
            get { return (ObservableCollection<Items>)GetValue(CustomLabelsProperty); }
            set { SetValue(CustomLabelsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CustomLabels.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomLabelsProperty =
            DependencyProperty.Register("CustomLabels", typeof(ObservableCollection<Items>), typeof(SfRangeSlider), new PropertyMetadata(null));
        
        /// <summary>
        /// Gets or sets the orientation of the label
        /// </summary>
        public Orientation LabelOrientation
        {
            get { return (Orientation)GetValue(LabelOrientationProperty); }
            set { SetValue(LabelOrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LabelOrientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelOrientationProperty =
            DependencyProperty.Register("LabelOrientation", typeof(Orientation), typeof(SfRangeSlider), new PropertyMetadata(Orientation.Horizontal, OnLabelOrientationChanged));  

        

        
        #endregion        

        #region Helper Methods
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        void RangeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#elif WINRT
        void RangeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
#endif
        {
            if (!isValueChangedInternally)
            {
                isValueChangedExternally = true;
                IntermediateValue = Value;
                isValueChangedExternally = false;
            }
            if (!ShowRange)
                UpdateThumbPosition(e.NewValue);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (isDragDelta||isPointerDown)
                UpdateThumbToolTip();
            else
                CloseToolTips();
#endif

        }
#if WINRT
        private static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                object v = (object)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v as DependencyObject);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
#endif
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        void RangeSlider_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
#elif WINRT
        void RangeSlider_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            VisualStateManager.GoToState(this, "Pressed", true);
        }
              
        void RangeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoaded = true;

            ValidateValues();
          
            UpdateOrientation();
            UpdateShowRange();
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            //UpdateThumbToolTip();
#endif
            UpdateMovePoint();
            UpdateTickPlacement();
            UpdateLabelPlacement();
            UpdateValuePlacement();
            if (!ShowRange)
                UpdateThumbPosition(Value);
            else
            {
                if (Orientation == Orientation.Horizontal)
                {
                    UpdateThumbTransform(horizontalRangeEndThumb, IntermediateRangeEnd);
                    UpdateThumbTransform(horizontalRangeStartThumb, IntermediateRangeStart);

                }
                else
                {
                    UpdateThumbTransform(verticalRangeEndThumb, IntermediateRangeEnd);
                    UpdateThumbTransform(verticalRangeStartThumb, IntermediateRangeStart);
                }
            }
            if (!ShowRange)
                IntermediateValue = Value;
            else
            {
                IntermediateRangeEnd = RangeEnd;
                IntermediateRangeStart = RangeStart;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
            UpdateVisualState();
        }

        private void ValidateValues()
        {
            if (RangeEnd > Maximum)
                RangeEnd = Maximum;

            if (RangeEnd < RangeStart)
                RangeEnd = RangeStart;

            if (RangeEnd < Minimum)
                RangeEnd = Minimum;
            if (RangeStart > RangeEnd)
                RangeStart = RangeEnd;

            if (RangeStart < Minimum)
                RangeStart = Minimum;
        }       

        /// <summary>
        /// Occurs when the HorizontalRangeEndThumb is dragged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnHorizontalRangeEndThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (MoveToPoint != MovePoint.None || SnapsTo==SliderSnapsTo.Ticks)
            {
                isDragDelta = true;
                if (ShowRange)
                {
                    if (IsDirectionReversed)
                    {
                        UpdateRangeEnd(IntermediateRangeEnd - (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
                    }
                    else
                        UpdateRangeEnd(IntermediateRangeEnd + (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
                }
                else
                {
                    if (IsDirectionReversed)
                    {
                        UpdateValue(IntermediateValue - (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
                    }
                    else
                        UpdateValue(IntermediateValue + (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
                }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF|| SILVERLIGHT
            CloseToolTips();
#endif
            UpdateThumbToolTip();
            UpdateThumbToolTip(horizontalRangeEndThumb, horizontalRangeEndThumbToolTip, true);
#endif
            isDragDelta = false;
            }
        }

        private void OnVerticalRangeEndThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (MoveToPoint != MovePoint.None || SnapsTo==SliderSnapsTo.Ticks)
            {
                isDragDelta = true;
                if (ShowRange)
                {
                    if (IsDirectionReversed)
                    {
                        UpdateRangeEnd(IntermediateRangeEnd + (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
                    }
                    else
                        UpdateRangeEnd(IntermediateRangeEnd - (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
                }
                else
                {
                    if (IsDirectionReversed)
                        UpdateValue(IntermediateValue + (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
                    else
                        UpdateValue(IntermediateValue - (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
                }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF|| SILVERLIGHT
            CloseToolTips();
#endif
            UpdateThumbToolTip();
            UpdateThumbToolTip(verticalRangeEndThumb, verticalRangeEndThumbToolTip, false);
#endif
            isDragDelta = false;
            }
        }

        void OnVerticalRangeStartThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (MoveToPoint != MovePoint.None || SnapsTo == SliderSnapsTo.Ticks)
            {
                isDragDelta = true;
                if (IsDirectionReversed)
                {
                    UpdateRangeStart(IntermediateRangeStart + (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
                }
                else
                    UpdateRangeStart(IntermediateRangeStart - (e.VerticalChange*((Maximum - Minimum)/ActualHeight)));
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF|| SILVERLIGHT
            CloseToolTips();
#endif
                UpdateThumbToolTip();
                UpdateThumbToolTip(verticalRangeStartThumb, verticalRangeStartThumbToolTip, false);
#endif
                isDragDelta = false;
            }
        }

        void OnHorizontalRangeStartThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (MoveToPoint != MovePoint.None||SnapsTo==SliderSnapsTo.Ticks)
            {
                isDragDelta = true;
                if (IsDirectionReversed)
                {
                    UpdateRangeStart(IntermediateRangeStart - (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
                }
                else
                    UpdateRangeStart(IntermediateRangeStart + (e.HorizontalChange*((Maximum - Minimum)/ActualWidth)));
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
#if WPF|| SILVERLIGHT
            CloseToolTips();
#endif
            UpdateThumbToolTip();
            UpdateThumbToolTip(horizontalRangeStartThumb, horizontalRangeStartThumbToolTip, true);
#endif
            isDragDelta = false;
            }
        }

        private void UpdateShowRange()
        {
            if (ShowRange)
            {
                if (horizontalRangeStartThumb != null)
                    horizontalRangeStartThumb.Visibility = Visibility.Visible;
                if (verticalRangeStartThumb != null)
                    verticalRangeStartThumb.Visibility = Visibility.Visible;
            }
            else
            {
                if (horizontalRangeStartThumb != null)
                    horizontalRangeStartThumb.Visibility = Visibility.Collapsed;
                if (verticalRangeStartThumb != null)
                    verticalRangeStartThumb.Visibility = Visibility.Collapsed;
            }
            if (ShowRange)
            {
                IntermediateRangeEnd = RangeEnd;
                IntermediateRangeStart = RangeStart;
                UpdateAllThumbsTransform();
            }
            else
            {
                IntermediateValue = Value;
                UpdateAllThumbsTransform();
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
        }

        private void UpdateMovePoint()
        {
            switch (MoveToPoint)
            {
                case MovePoint.None:
                    stepValue = 0d;
                    break;
                case MovePoint.MoveToTapPosition:
                    stepValue = 0d;
                    break;
                case MovePoint.IncrementBySmallChange:
                    stepValue = SmallChange;
                    break;
                case MovePoint.IncrementByLargeChange:
                    stepValue = LargeChange;
                    break;
            }
        }

        
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private void UpdateThumbToolTipConstantOffset()
        {
			UpdateThumbToolTip();
            horizontalRangeEndThumbToolTip.VerticalOffset = ToolTipDistance;
            horizontalRangeStartThumbToolTip.VerticalOffset = ToolTipDistance;
            verticalRangeEndThumbToolTip.HorizontalOffset = ToolTipDistance;
            verticalRangeStartThumbToolTip.HorizontalOffset = ToolTipDistance;
        }

        private void UpdateThumbToolTip()
        {
            if (horizontalRangeEndThumb != null)
            {
                if (horizontalRangeEndThumbToolTip == null)
                {
                    horizontalRangeEndThumbToolTip = new ToolTip() { VerticalOffset = ToolTipDistance };                    
                    horizontalRangeEndThumbToolTip.Opened += horizontalRangeEndThumbToolTip_Opened;
                }
                if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    ToolTipService.SetToolTip(horizontalRangeEndThumb, horizontalRangeEndThumbToolTip);
                if(ThumbToolTipPlacement == ThumbToolTipPlacement.BottomRight)
                    ToolTipService.SetPlacement(horizontalRangeEndThumb, PlacementMode.Bottom);  
                else if(ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                    ToolTipService.SetPlacement(horizontalRangeEndThumb, PlacementMode.Top);  
            }
            if (verticalRangeEndThumb != null)
            {
                if (verticalRangeEndThumbToolTip == null)
                {
                    verticalRangeEndThumbToolTip = new ToolTip() { HorizontalOffset = ToolTipDistance };
                    verticalRangeEndThumbToolTip.Opened += verticalRangeEndThumbToolTip_Opened;
                }
                if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    ToolTipService.SetToolTip(verticalRangeEndThumb, verticalRangeEndThumbToolTip);
                if (ThumbToolTipPlacement == ThumbToolTipPlacement.BottomRight)
                    ToolTipService.SetPlacement(verticalRangeEndThumb, PlacementMode.Right);
                else if(ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                    ToolTipService.SetPlacement(verticalRangeEndThumb, PlacementMode.Left);
              
            }
            if (horizontalRangeStartThumb != null)
            {
                if (horizontalRangeStartThumbToolTip == null)
                {
                    horizontalRangeStartThumbToolTip = new ToolTip() { VerticalOffset = ToolTipDistance };
                    horizontalRangeStartThumbToolTip.Opened += horizontalRangeStartThumbToolTip_Opened;
                }
                if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    ToolTipService.SetToolTip(horizontalRangeStartThumb, horizontalRangeStartThumbToolTip);
                if (ThumbToolTipPlacement == ThumbToolTipPlacement.BottomRight)
                    ToolTipService.SetPlacement(horizontalRangeStartThumb, PlacementMode.Bottom);
                else if (ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                    ToolTipService.SetPlacement(horizontalRangeStartThumb, PlacementMode.Top);  
                              
            }
            if (verticalRangeStartThumb != null)
            {
                if (verticalRangeStartThumbToolTip == null)
                {
                    verticalRangeStartThumbToolTip = new ToolTip() { HorizontalOffset = ToolTipDistance };
                    verticalRangeStartThumbToolTip.Opened += verticalRangeStartThumbToolTip_Opened;
                }
                if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    ToolTipService.SetToolTip(verticalRangeStartThumb, verticalRangeStartThumbToolTip);
                if (ThumbToolTipPlacement == ThumbToolTipPlacement.BottomRight)
                    ToolTipService.SetPlacement(verticalRangeStartThumb, PlacementMode.Right);
                else if (ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                    ToolTipService.SetPlacement(verticalRangeStartThumb, PlacementMode.Left);
                
              
            }
        }
#endif
        private void UpdateTickPlacement()
        {
            if ((TickPlacement == TickPlacement.BottomRight && LabelPlacement == LabelPlacement.BottomRight && ValuePlacement == ValuePlacement.BottomRight) ||
                TickPlacement == TickPlacement.TopLeft && LabelPlacement == LabelPlacement.TopLeft && ValuePlacement == ValuePlacement.TopLeft)
            {
                showTick = true;
            }
            else
            {
                showTick = false;
            }
            if (LeftTickBar != null)
                LeftTickBar.Visibility = Visibility.Collapsed;
            if (RightTickBar != null)
                RightTickBar.Visibility = Visibility.Collapsed;
            if (TopTickBar != null)
                TopTickBar.Visibility = Visibility.Collapsed;
            if (BottomTickBar != null)
                BottomTickBar.Visibility = Visibility.Collapsed;
            if (HorizontalInlineTickBar != null)
                HorizontalInlineTickBar.Visibility = Visibility.Collapsed;
            if (VerticalInlineTickBar != null)
                VerticalInlineTickBar.Visibility = Visibility.Collapsed;
            if (TickPlacement == TickPlacement.BottomRight)
            {
                if (BottomTickBar != null && Orientation == Orientation.Horizontal)
                {                   
                    BottomTickBar.Visibility = Visibility.Visible;                  
                }
                if (RightTickBar != null && Orientation == Orientation.Vertical)
                {                  
                    RightTickBar.Visibility = Visibility.Visible;                  
                }
            }
            if (TickPlacement == TickPlacement.TopLeft)
            {
                if (TopTickBar != null && Orientation == Orientation.Horizontal)
                {                  
                    TopTickBar.Visibility = Visibility.Visible;             
                }
                if (LeftTickBar != null && Orientation == Orientation.Vertical)
                {
                    LeftTickBar.Visibility = Visibility.Visible;                   
                }
            }
            if (TickPlacement == TickPlacement.Inline)
            {
                if (VerticalInlineTickBar != null && Orientation == Orientation.Vertical)
                {                
                 VerticalInlineTickBar.Visibility = Visibility.Visible;                
                }
                if (HorizontalInlineTickBar != null && Orientation == Orientation.Horizontal)
                {               
                    HorizontalInlineTickBar.Visibility = Visibility.Visible;                   
                }
            }
            if (TickPlacement == TickPlacement.Outside)
            {
                if (TopTickBar != null && Orientation == Orientation.Horizontal)
                {                 
                  TopTickBar.Visibility = Visibility.Visible;                 
                }
                if (BottomTickBar != null && Orientation == Orientation.Horizontal)
                {                  
                    BottomTickBar.Visibility = Visibility.Visible;                  
                }
                if (LeftTickBar != null && Orientation == Orientation.Vertical)
                {                
                    LeftTickBar.Visibility = Visibility.Visible;                
                }
                if (RightTickBar != null && Orientation == Orientation.Vertical)
                {                 
                    RightTickBar.Visibility = Visibility.Visible;                
                }
            }

        }
        private void UpdateValuePlacement()
        {
            if (ShowValueLabels)
            {
                if (showTick)
                {
                    if (LeftTickBar != null)
                        LeftTickBar.Visibility = Visibility.Collapsed;
                    if (RightTickBar != null)
                        RightTickBar.Visibility = Visibility.Collapsed;
                    if (TopTickBar != null)
                        TopTickBar.Visibility = Visibility.Collapsed;
                    if (BottomTickBar != null)
                        BottomTickBar.Visibility = Visibility.Collapsed;
                }
                if (ValuePlacement == ValuePlacement.BottomRight)
                {
                    if (BottomTickBar != null && Orientation == Orientation.Horizontal)
                    {
                        BottomTickBar.Visibility = Visibility.Visible;
                    }
                    if (RightTickBar != null && Orientation == Orientation.Vertical)
                    {
                        RightTickBar.Visibility = Visibility.Visible;
                    }
                }
                if (ValuePlacement == ValuePlacement.TopLeft)
                {
                    if (TopTickBar != null && Orientation == Orientation.Horizontal)
                    {
                        TopTickBar.Visibility = Visibility.Visible;
                    }
                    if (LeftTickBar != null && Orientation == Orientation.Vertical)
                    {
                        LeftTickBar.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void UpdateLabelPlacement()
        {
            if (ShowCustomLabels)
            {
                if (showTick)
                {
                    if (LeftTickBar != null)
                        LeftTickBar.Visibility = Visibility.Collapsed;
                    if (RightTickBar != null)
                        RightTickBar.Visibility = Visibility.Collapsed;
                    if (TopTickBar != null)
                        TopTickBar.Visibility = Visibility.Collapsed;
                    if (BottomTickBar != null)
                        BottomTickBar.Visibility = Visibility.Collapsed;                  
                }
                if (LabelPlacement == LabelPlacement.BottomRight)
                {
                    if (BottomTickBar != null && Orientation == Orientation.Horizontal)
                    {
                        BottomTickBar.Visibility = Visibility.Visible;
                    }
                    if (RightTickBar != null && Orientation == Orientation.Vertical)
                    {
                        RightTickBar.Visibility = Visibility.Visible;
                    }
                }
                if (LabelPlacement == LabelPlacement.TopLeft)
                {
                    if (TopTickBar != null && Orientation == Orientation.Horizontal)
                    {
                        TopTickBar.Visibility = Visibility.Visible;
                    }
                    if (LeftTickBar != null && Orientation == Orientation.Vertical)
                    {
                        LeftTickBar.Visibility = Visibility.Visible;
                    }
                }
            }
        }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        void verticalRangeEndThumbToolTip_Opened(object sender, RoutedEventArgs e)
        {
			if(SnapsTo==SliderSnapsTo.Ticks)
            {
                if (ShowRange)
                    verticalRangeEndThumbToolTip.Content = RangeEnd.ToString("F" + ThumbToolTipPrecision);
                else
                    verticalRangeEndThumbToolTip.Content = Value.ToString("F" + ThumbToolTipPrecision);
            }
			else
			{
				if (ShowRange)
                	verticalRangeEndThumbToolTip.Content = IntermediateRangeEnd.ToString("F" + ThumbToolTipPrecision);
           		else
                	verticalRangeEndThumbToolTip.Content = IntermediateValue.ToString("F" + ThumbToolTipPrecision);
			}
            verticalRangeEndThumbToolTip.HorizontalOffset = ToolTipDistance;
        }
#endif
        void OnIntermediateValueChanged(DependencyPropertyChangedEventArgs args)
        {
            isValueChangedInternally = true;
            if (!isValueChangedExternally)
            {
                Value = GetRangeValue(IntermediateValue);
            }
			if(SnapsTo==SliderSnapsTo.Ticks)
            UpdateThumbPosition(Value);
			else
			UpdateThumbPosition(IntermediateValue);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (Orientation == Orientation.Horizontal)
            {
                if (horizontalRangeEndThumbToolTip != null)
				{
					if(SnapsTo==SliderSnapsTo.Ticks)
                    horizontalRangeEndThumbToolTip.Content =Value.ToString("F" + ThumbToolTipPrecision);
					else
					horizontalRangeEndThumbToolTip.Content = IntermediateValue.ToString("F" + ThumbToolTipPrecision);
				}
            }
            else
            {
                if (verticalRangeEndThumbToolTip != null)
				{
					if(SnapsTo==SliderSnapsTo.Ticks)
                    verticalRangeEndThumbToolTip.Content = Value.ToString("F" + ThumbToolTipPrecision);
					else
					verticalRangeEndThumbToolTip.Content = IntermediateValue.ToString("F" + ThumbToolTipPrecision);
                }
            }
#endif
            isValueChangedInternally = false;
        }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        void verticalRangeStartThumbToolTip_Opened(object sender, RoutedEventArgs e)
        {
			if(SnapsTo==SliderSnapsTo.Ticks)
            	verticalRangeStartThumbToolTip.Content = RangeStart.ToString("F" + ThumbToolTipPrecision);
			else
				verticalRangeStartThumbToolTip.Content = IntermediateRangeStart.ToString("F" + ThumbToolTipPrecision);
            verticalRangeStartThumbToolTip.HorizontalOffset = ToolTipDistance;
        }

        void horizontalRangeEndThumbToolTip_Opened(object sender, RoutedEventArgs e)
        {
            if (ShowRange)
			{
				if(SnapsTo==SliderSnapsTo.Ticks)
                horizontalRangeEndThumbToolTip.Content = RangeEnd.ToString("F" + ThumbToolTipPrecision);
				else
                horizontalRangeEndThumbToolTip.Content = IntermediateRangeEnd.ToString("F" + ThumbToolTipPrecision);
			}
            else
			{
				if(SnapsTo==SliderSnapsTo.Ticks)
                horizontalRangeEndThumbToolTip.Content = Value.ToString("F" + ThumbToolTipPrecision);
				else
				 horizontalRangeEndThumbToolTip.Content = IntermediateValue.ToString("F" + ThumbToolTipPrecision);
			}
            horizontalRangeEndThumbToolTip.VerticalOffset = ToolTipDistance;
        }

        void horizontalRangeStartThumbToolTip_Opened(object sender, RoutedEventArgs e)
        {
			if(SnapsTo==SliderSnapsTo.Ticks)
            horizontalRangeStartThumbToolTip.Content = RangeStart.ToString("F" + ThumbToolTipPrecision);
			else
            horizontalRangeStartThumbToolTip.Content = IntermediateRangeStart.ToString("F" + ThumbToolTipPrecision);
            horizontalRangeStartThumbToolTip.VerticalOffset = ToolTipDistance;
        }
#endif
		void RangeSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((Orientation == Orientation.Horizontal && e.NewSize.Width != e.PreviousSize.Width) ||
                (Orientation == Orientation.Vertical && e.NewSize.Height != e.PreviousSize.Height))
            UpdateAllThumbsTransform();           
        }
        


        private void UpdateOrientation()
        {
            IntermediateRangeEnd = RangeEnd;
            IntermediateRangeStart = RangeStart;
            IntermediateValue = Value;
            if (Orientation == Orientation.Horizontal)
            {
                if (horizontalTemplate != null)
                    horizontalTemplate.Visibility = Visibility.Visible;
                if (verticalTemplate != null)
                    verticalTemplate.Visibility = Visibility.Collapsed;
                UpdateTickPlacement();
                UpdateLabelPlacement();
                UpdateValuePlacement();
                UpdateAllThumbsTransform();
            }
            else
            {               
                if (horizontalTemplate != null)
                    horizontalTemplate.Visibility = Visibility.Collapsed;
                if (verticalTemplate != null)
                    verticalTemplate.Visibility = Visibility.Visible;
                UpdateTickPlacement();
                UpdateLabelPlacement();
                UpdateValuePlacement();
                UpdateAllThumbsTransform();
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif

        }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private void CloseToolTips()
        {

            if (horizontalRangeStartThumbToolTip != null)
                horizontalRangeStartThumbToolTip.IsOpen = false;
            if (verticalRangeEndThumbToolTip != null)
                verticalRangeEndThumbToolTip.IsOpen = false;
            if (horizontalRangeEndThumbToolTip != null)
                horizontalRangeEndThumbToolTip.IsOpen = false;
            if (verticalRangeStartThumbToolTip != null)
                verticalRangeStartThumbToolTip.IsOpen = false;

        }
#endif
        private void UpdateRangeStart(double newValue)
        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (Orientation == Orientation.Vertical && ((MousePoint.Y > ActualHeight && MousePoint.Y > 0)||newValue<=Minimum))
            {
                isMouseEnter = false;
                if (MousePoint.Y > ActualHeight)
                    IntermediateRangeStart = Minimum;
            }
            else if (Orientation == Orientation.Horizontal && ((MousePoint.X < ActualWidth && MousePoint.X < 0)||newValue<=Minimum))
            {
                isMouseEnter = false;
                if (MousePoint.X < ActualWidth)
                    IntermediateRangeStart = Minimum;
            }
#elif WINRT

            if (MousePointerPoint != null && Orientation == Orientation.Vertical && MousePointerPoint.Position.Y > ActualHeight && MousePointerPoint.Position.Y > 0)
            {
                isMouseEnter = false;
                if (MousePointerPoint.Position.Y > ActualHeight)
                    IntermediateRangeStart = Minimum;
            }
            else if (MousePointerPoint != null && Orientation == Orientation.Horizontal && MousePointerPoint.Position.X < ActualWidth && MousePointerPoint.Position.X < 0)
            {
                isMouseEnter = false;
                if (MousePointerPoint.Position.X < ActualWidth)
                    IntermediateRangeStart = Minimum;
            }
#endif
            if(!isPointerMove && !isDragDelta &&(MoveToPoint == MovePoint.IncrementByLargeChange || MoveToPoint == MovePoint.IncrementBySmallChange)&& (RangeStart>newValue)&& SnapsTo==SliderSnapsTo.StepValues)
            {
                if (newValue > IntermediateRangeStart)
                    newValue = IntermediateRangeStart + stepValue;
                else
                    newValue = IntermediateRangeStart - stepValue;
            }
            
            if (newValue <= IntermediateRangeEnd && newValue >= Minimum && isMouseEnter)
            {
                IntermediateRangeStart = newValue;
            }
        }

        private void UpdateRangeEnd(double newValue)
        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (Orientation == Orientation.Vertical && ((MousePoint.Y < ActualHeight && MousePoint.Y < 0)||newValue>=Maximum))
            {
                isMouseEnter = false;
                if (MousePoint.Y < 0)
                    IntermediateRangeEnd = Maximum;
            }
            else if (Orientation == Orientation.Horizontal && ((MousePoint.X > ActualWidth && MousePoint.X > 0)||newValue>=Maximum))
            {
                isMouseEnter = false;
                if (MousePoint.X > 0)
                    IntermediateRangeEnd = Maximum;
            }
#elif WINRT
            if (MousePointerPoint != null && Orientation == Orientation.Vertical && MousePointerPoint.Position.Y < ActualHeight && MousePointerPoint.Position.Y < 0)
            {
                isMouseEnter = false;
                if (MousePoint.Y < 0)
                    IntermediateRangeEnd = Maximum;
            }
            else if ( MousePointerPoint != null && Orientation == Orientation.Horizontal && MousePointerPoint.Position.X > ActualWidth && MousePointerPoint.Position.X > 0)
            {
                isMouseEnter = false;
                if (MousePoint.X > 0)
                    IntermediateRangeEnd = Maximum;
            }
#endif
            if (!isPointerMove && !isDragDelta &&(MoveToPoint == MovePoint.IncrementByLargeChange || MoveToPoint == MovePoint.IncrementBySmallChange)&& (RangeEnd<newValue) && SnapsTo==SliderSnapsTo.StepValues)
            {
                if (newValue > IntermediateRangeEnd)
                    newValue = IntermediateRangeEnd + stepValue;
                else
                    newValue = IntermediateRangeEnd - stepValue;
            }
            if (newValue >= IntermediateRangeStart && newValue <= Maximum && isMouseEnter)
            {
                IntermediateRangeEnd = newValue;
            }
            if (newValue < RangeStart && IntermediateRangeEnd == RangeStart)
                RangeStart = newValue;
        }

        private double UpdateValue(double newValue)
        {
            double value = newValue;

            if (Orientation == Orientation.Horizontal && !isDragDelta && !isPointerMove)
            {
                double f = ActualWidth / (Maximum - Minimum);
                double val = newValue / f;
                value = val + Minimum;
            }
            else if (Orientation == Orientation.Vertical && !isDragDelta && !isPointerMove)
            {
                double f = ActualHeight / (Maximum - Minimum);
                double val = newValue / f;
                value = val + Minimum;
            }

            if (!isPointerMove && !isDragDelta && !ShowRange &&(MoveToPoint == MovePoint.IncrementByLargeChange || MoveToPoint == MovePoint.IncrementBySmallChange)&&SnapsTo==SliderSnapsTo.StepValues)
            {
                if (value > IntermediateValue)
                    value = IntermediateValue + stepValue;
                else
                    value = IntermediateValue - stepValue;
            }
            if (value > Maximum)
                value = Maximum;
            if (value < Minimum)
                value = Minimum;

            if (!ShowRange)
            {
                IntermediateValue = value;                
            }
            return value;
        }

        private double GetThumbTransform(Thumb thumb, double newValue)
        {
            double factor = (newValue - Minimum) / (Maximum - Minimum);
            double transform = 0d;
            if (factor >= 0d)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (IsDirectionReversed)
                        transform = (1 - factor) * thumb.ActualWidth + (ActualWidth / (Maximum - Minimum)) * (newValue - Minimum);
                    else
                        transform = (ActualWidth / (Maximum - Minimum)) * (newValue - Minimum) - (factor * thumb.ActualWidth);
                }
                else
                {
                    if (IsDirectionReversed)
                        transform = (1 - factor) * thumb.ActualHeight + (ActualHeight / (Maximum - Minimum)) * (newValue - Minimum);
                    else
                        transform = (ActualHeight / (Maximum - Minimum)) * (newValue - Minimum) - (factor * thumb.ActualHeight);
                }
            }
            return transform;
           
        }

        private void UpdateThumbTransform(Thumb thumb, double newValue)
        {           
            if (Orientation == Orientation.Horizontal)
            {
                UpdateHorizontalThumbTransform(thumb, newValue);
            }
            else
            {
                UpdateVerticalThumbTransform(thumb, newValue);
            }
            
        }

        private void UpdateVerticalThumbTransform(Thumb thumb, double newValue)
        {
            if (verticalRangeStartTransform == null)
                verticalRangeStartTransform = new TranslateTransform();
            if (verticalRangeEndTransform == null)
                verticalRangeEndTransform = new TranslateTransform();

            if (thumb != null)
            {
                if (thumb == verticalRangeStartThumb)
                {
                    if (IsDirectionReversed)
                    {
                        verticalRangeStartTransform.Y = (GetThumbTransform(thumb, newValue)) - ActualHeight;
                    }
                    else
                        verticalRangeStartTransform.Y = -(GetThumbTransform(thumb, newValue));
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (verticalRangeStartThumbToolTip != null)
                    {
                        UpdateThumbToolTip(verticalRangeEndThumb, verticalRangeStartThumbToolTip, false);
#if !(WPF|| SILVERLIGHT)
                        verticalRangeStartThumbToolTip.VerticalOffset = verticalRangeStartTransform.Y;
#endif
                    }
#endif
                    thumb.RenderTransform = verticalRangeStartTransform;
                }
                if (thumb == verticalRangeEndThumb)
                {
                    if (IsDirectionReversed)
                    {
                        verticalRangeEndTransform.Y = (GetThumbTransform(thumb, newValue)) - ActualHeight;
                    }
                    else
                        verticalRangeEndTransform.Y = -(GetThumbTransform(thumb, newValue));
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (verticalRangeEndThumbToolTip != null)
                    {
                        UpdateThumbToolTip(verticalRangeEndThumb, verticalRangeEndThumbToolTip, false);
#if !(WPF|| SILVERLIGHT)
                        verticalRangeEndThumbToolTip.VerticalOffset = verticalRangeEndTransform.Y;
#endif
                    }
#endif
                    thumb.RenderTransform = verticalRangeEndTransform;
                }
            }


            if (verticalDecreaseRect != null)
            {
                if (verticalScale == null)
                {
                    verticalScale = new ScaleTransform();
                }

                if (ShowRange)
                {
                    if (Maximum - Minimum != 0d)
                    {
                        double rangeFactor = 0;
                        if (SnapsTo == SliderSnapsTo.Ticks)
                            rangeFactor = (RangeEnd - RangeStart) / (Maximum - Minimum);
                        else
                            rangeFactor = (IntermediateRangeEnd - IntermediateRangeStart) / (Maximum - Minimum);
                        verticalScale.ScaleY = rangeFactor;
                    }
                    else
                        verticalScale.ScaleY = 1;
                }
                else
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
					{
                    if (Maximum - Minimum != 0d)
                    {
                        double valueFactor = (Value - Minimum) / (Maximum - Minimum);
                        verticalScale.ScaleY =  valueFactor;
                    }
                    else
                        verticalScale.ScaleY = 1;
					}
					else
						 verticalScale.ScaleY = ValueFactor;
                }
                verticalScale.ScaleX = 1;
                verticalDecreaseRect.Height = this.ActualHeight;
                TranslateTransform tTransform = new TranslateTransform();
                if (IsDirectionReversed)
                {
                    tTransform.Y = ActualHeight + verticalRangeStartTransform.Y - verticalRangeStartThumb.ActualHeight;
                }
                else
                {
                    tTransform.Y = verticalRangeStartTransform.Y;
                }
                TransformGroup tGroup = new TransformGroup();
                tGroup.Children.Add(verticalScale);
                if (ShowRange)
                    tGroup.Children.Add(tTransform);
                if (IsDirectionReversed)
                    verticalDecreaseRect.RenderTransformOrigin = new Point(0.5, 0);
                else
                    verticalDecreaseRect.RenderTransformOrigin = new Point(0, 1);
                verticalDecreaseRect.RenderTransform = tGroup;
            }
        }

        private void UpdateHorizontalThumbTransform(Thumb thumb, double newValue)
        {
            if (horizontalRangeStartTransform == null)
                horizontalRangeStartTransform = new TranslateTransform();
            if (horizontalRangeEndTransform == null)
                horizontalRangeEndTransform = new TranslateTransform();

            if (thumb != null)
            {
                if (thumb == horizontalRangeStartThumb)
                {
                    if (IsDirectionReversed)
                    {
                        horizontalRangeStartTransform.X = ActualWidth - GetThumbTransform(thumb, newValue);
                    }
                    else
                        horizontalRangeStartTransform.X = GetThumbTransform(thumb, newValue);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (horizontalRangeStartThumbToolTip != null)
                    {
                        UpdateThumbToolTip(horizontalRangeStartThumb, horizontalRangeStartThumbToolTip, true);
#if !(WPF|| SILVERLIGHT)
                        horizontalRangeStartThumbToolTip.HorizontalOffset = horizontalRangeStartTransform.X;
#endif
                    }
#endif
                    thumb.RenderTransform = horizontalRangeStartTransform;
                }
                if (thumb == horizontalRangeEndThumb)
                {
                    if (IsDirectionReversed)
                    {
                        horizontalRangeEndTransform.X = ActualWidth - GetThumbTransform(thumb, newValue);
                    }
                    else
                        horizontalRangeEndTransform.X = GetThumbTransform(thumb, newValue);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    if (horizontalRangeEndThumbToolTip != null)
                    {
                        UpdateThumbToolTip(horizontalRangeEndThumb, horizontalRangeEndThumbToolTip, true);
#if !(WPF|| SILVERLIGHT)
                        horizontalRangeEndThumbToolTip.HorizontalOffset = horizontalRangeEndTransform.X;
#endif
                    }
#endif
                    thumb.RenderTransform = horizontalRangeEndTransform;
                }
            }


            if (horizontalDecreaseRect != null)
            {
                if (horizontalScale == null)
                {
                    horizontalScale = new ScaleTransform();
                }
                TransformGroup tGroup = new TransformGroup();
                TranslateTransform tTransform = new TranslateTransform();
                if (ShowRange)
                {
                    if (Maximum - Minimum != 0d)
                    {
                        double rangeFactor = 0;
						if(SnapsTo==SliderSnapsTo.Ticks)
                            rangeFactor= (RangeEnd - RangeStart)/(Maximum - Minimum);
						else
                            rangeFactor = (IntermediateRangeEnd - IntermediateRangeStart) / (Maximum - Minimum);
                        horizontalScale.ScaleX = rangeFactor;
                    }
                    else
                        horizontalScale.ScaleX = 1;
                }
                else
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
					{
                    if (Maximum - Minimum != 0d)
                    {
                        double valueFactor = (Value - Minimum)/(Maximum - Minimum);
                        horizontalScale.ScaleX = valueFactor;
                    }
                    else
                        horizontalScale.ScaleX = 1;
					}
					else
						horizontalScale.ScaleX = ValueFactor;
                }
				horizontalScale.ScaleY = 1;
                horizontalDecreaseRect.Width = this.ActualWidth;
                if (IsDirectionReversed)
                {
                    horizontalDecreaseRect.RenderTransformOrigin = new Point(1, 0.5);
                    tTransform.X = horizontalRangeStartTransform.X - ActualWidth + horizontalRangeStartThumb.ActualWidth;
                }
                else
                {
                    horizontalDecreaseRect.RenderTransformOrigin = new Point(0, 0.5);
                    tTransform.X = horizontalRangeStartTransform.X;
                }
                tGroup.Children.Add(horizontalScale);
                if (ShowRange)
                    tGroup.Children.Add(tTransform);
                horizontalDecreaseRect.RenderTransform = tGroup;
            }
        }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        private void UpdateThumbToolTip(Thumb thumb, ToolTip toolTip, bool isHorizontal)
        {
            if (thumb != null && toolTip != null & !toolTip.IsOpen && ThumbToolTipPlacement != ThumbToolTipPlacement.None && IsLoaded)
            {
                if (ToolTipService.GetToolTip(thumb) == null)                    
                    ToolTipService.SetToolTip(thumb, toolTip);
                if(_canOpen)
                 toolTip.IsOpen = true;
                if (isHorizontal)
#if WPF
                {
                    if (ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                        toolTip.VerticalOffset = -(ToolTipDistance + horizontalTrackRect.ActualHeight + TopTickBar.ActualHeight + HorizontalInlineTickBar.ActualHeight);
                    else
                        toolTip.VerticalOffset = ToolTipDistance;
                }
#else
                    toolTip.VerticalOffset = ToolTipDistance;
#endif
                else
#if WPF
                {
                    if (ThumbToolTipPlacement == ThumbToolTipPlacement.TopLeft)
                        toolTip.HorizontalOffset = -(ToolTipDistance + verticalTrackRect.ActualWidth + TopTickBar.ActualWidth + BottomTickBar.ActualWidth + VerticalInlineTickBar.ActualWidth);
                    else
                        toolTip.HorizontalOffset = ToolTipDistance + verticalTrackRect.ActualWidth +TopTickBar.ActualWidth + BottomTickBar.ActualWidth+VerticalInlineTickBar.ActualWidth;
                }
#else
                toolTip.HorizontalOffset = ToolTipDistance;
#endif
            }
        }
#endif
        private void UpdateThumbPosition(double newValue)
        {
            if (!ShowRange)
            {
                if (Orientation == Orientation.Horizontal)
                    UpdateThumbTransform(horizontalRangeEndThumb, newValue);
                else
                    UpdateThumbTransform(verticalRangeEndThumb, newValue);
            }
        }

        private void HorizontalRangeStartThumb_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateThumbTransform(horizontalRangeStartThumb, IntermediateRangeStart);
#if WINRT
            PART_BorderThumb = GetVisualChild<Border>(sender as DependencyObject);
            if (PART_BorderThumb != null)
                PART_BorderThumb.PointerPressed += HorizontalRangeStartThumb_PointerPressed;
            //horizontalRangeStartThumbToolTip.IsOpen = false;
#endif
        }

        private void VerticalRangeStartThumb_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateThumbTransform(verticalRangeStartThumb, IntermediateRangeStart);
#if WINRT
            PART_BorderThumb = GetVisualChild<Border>(sender as DependencyObject);
            if (PART_BorderThumb != null)
                PART_BorderThumb.PointerPressed += VerticalRangeStartThumb_PointerPressed;
            //verticalRangeStartThumbToolTip.IsOpen = false;
#endif
        }

        private void HorizontalRangeEndThumb_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowRange)
                UpdateThumbTransform(horizontalRangeEndThumb, IntermediateRangeEnd);           
#if WINRT
            
            PART_BorderThumb = GetVisualChild<Border>(sender as DependencyObject);
            if (PART_BorderThumb != null)
                PART_BorderThumb.PointerPressed += HorizontalRangeEndThumb_PointerPressed;
            //horizontalRangeEndThumbToolTip.IsOpen = false;
#endif
        }

        private void VerticalRangeEndThumb_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowRange)
                UpdateThumbTransform(verticalRangeEndThumb, IntermediateRangeEnd);   
#if WINRT
            PART_BorderThumb = GetVisualChild<Border>(sender as DependencyObject);
            if(PART_BorderThumb!=null)
                PART_BorderThumb.PointerPressed += VerticalRangeEndThumb_PointerPressed;
            //verticalRangeEndThumbToolTip.IsOpen = false;
#endif
        }

        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes all the child elements of <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.RangeSlider"/> control.
        /// </summary>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        public override void OnApplyTemplate()
#elif WINRT
        protected override void OnApplyTemplate()
#endif
        {
            horizontalRangeEndThumb = GetTemplateChild("HorizontalRangeEndThumb") as Thumb;
            verticalRangeEndThumb = GetTemplateChild("VerticalRangeEndThumb") as Thumb;
            horizontalRangeStartThumb = GetTemplateChild("HorizontalRangeStartThumb") as Thumb;
            verticalRangeStartThumb = GetTemplateChild("VerticalRangeStartThumb") as Thumb;
            horizontalTemplate = GetTemplateChild("HorizontalTemplate") as Grid;
            verticalTemplate = GetTemplateChild("VerticalTemplate") as Grid;
            TopTickBar = GetTemplateChild("TopTickBar") as TickBar;
            HorizontalInlineTickBar = GetTemplateChild("HorizontalInlineTickBar") as TickBar;
            BottomTickBar = GetTemplateChild("BottomTickBar") as TickBar;
            LeftTickBar = GetTemplateChild("LeftTickBar") as TickBar;
            VerticalInlineTickBar = GetTemplateChild("VerticalInlineTickBar") as TickBar;
            RightTickBar = GetTemplateChild("RightTickBar") as TickBar;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            //UpdateThumbToolTip();
#endif
            UpdateTickPlacement();
            UpdateLabelPlacement();
            UpdateValuePlacement();
            if (horizontalRangeEndThumb != null)
            {
                horizontalRangeEndThumb.DragDelta += OnHorizontalRangeEndThumbDragDelta;
                horizontalRangeEndThumb.Loaded +=HorizontalRangeEndThumb_Loaded;             
            }
            if (verticalRangeEndThumb != null)
            {
                verticalRangeEndThumb.DragDelta += OnVerticalRangeEndThumbDragDelta;
                verticalRangeEndThumb.Loaded += VerticalRangeEndThumb_Loaded;              
            }

            if (horizontalRangeStartThumb != null)
            {
                horizontalRangeStartThumb.DragDelta += OnHorizontalRangeStartThumbDragDelta;
                horizontalRangeStartThumb.Loaded+= HorizontalRangeStartThumb_Loaded;            
            }

            if (verticalRangeStartThumb != null)
            {
                verticalRangeStartThumb.DragDelta += OnVerticalRangeStartThumbDragDelta;
                verticalRangeStartThumb.Loaded+=VerticalRangeStartThumb_Loaded;            
            }
            horizontalDecreaseRect = GetTemplateChild("HorizontalDecreaseRect") as Rectangle;
            verticalDecreaseRect = GetTemplateChild("VerticalDecreaseRect") as Rectangle;
            horizontalTrackRect = GetTemplateChild("HorizontalTrackRect") as Rectangle;
            verticalTrackRect = GetTemplateChild("VerticalTrackRect") as Rectangle;

            if (IsDirectionReversed)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    horizontalTrackRect.Fill = this.Background;
                    horizontalDecreaseRect.Fill = this.Foreground;
                }
                else
                {
                    verticalTrackRect.Fill = this.Background;
                    verticalDecreaseRect.Fill = this.Foreground;
                }
            }
#if !(WINDOWS_PHONE_7||WINDOWS_PHONE)
            _canOpen = false;
#endif
            UpdateShowRange();
#if !(WINDOWS_PHONE_7||WINDOWS_PHONE)
            _canOpen = true;
#endif

            base.OnApplyTemplate();
        }

#if WINRT
        void VerticalRangeEndThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UpdateThumbToolTip();
            CloseToolTips();
            verticalRangeEndThumbToolTip.IsOpen = true;
        }
        void VerticalRangeStartThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UpdateThumbToolTip();
            CloseToolTips();
            verticalRangeStartThumbToolTip.IsOpen = true;
        }
        void HorizontalRangeEndThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UpdateThumbToolTip();
            CloseToolTips();
            horizontalRangeEndThumbToolTip.IsOpen = true;
        }
        void HorizontalRangeStartThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UpdateThumbToolTip();
            CloseToolTips();
            horizontalRangeStartThumbToolTip.IsOpen = true;
        }
#endif
        /// <summary>
        /// Occurs when the Minimum value has changed.
        /// </summary>
        /// <param name="oldMinimum"></param>
        /// <param name="newMinimum"></param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            if (IsLoaded)
            {
                if (newMinimum > Value || newMinimum > RangeStart || newMinimum > RangeEnd)
                {
                    Value = newMinimum;
                    if (newMinimum > RangeEnd)
                        RangeEnd = newMinimum;
                    RangeStart = newMinimum;
                }
                if (newMinimum > Maximum)
                    Maximum = newMinimum;
                UpdateAllThumbsTransform();
            }
            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        /// <summary>
        /// Occurs when the Maximum value has changed.
        /// </summary>
        /// <param name="oldMaximum"></param>
        /// <param name="newMaximum"></param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            if (IsLoaded)
            {
                if (Value > newMaximum || RangeStart > newMaximum || RangeEnd > newMaximum)
                {
                    Value = newMaximum;
                   
                    if (RangeStart > newMaximum)
                        RangeStart = newMaximum;
                    RangeEnd = newMaximum;
                }
                if (Minimum > newMaximum)
                    Minimum = newMaximum;
                UpdateAllThumbsTransform();
            }
            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        /// <summary>
        /// Occurs when the pointer is pressed
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Focus();
            Point pPoint = e.GetPosition(this);
#elif WINRT
        protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Focus(Windows.UI.Xaml.FocusState.Pointer);
            PointerPoint pPoint = e.GetCurrentPoint(this);
            MousePointerPoint = e.GetCurrentPoint(this);
#endif
            VisualStateManager.GoToState(this, "PointerPressed", true);
            startPoint = pPoint;
            isPointerDown = true;

            if (MoveToPoint != MovePoint.None || SnapsTo==SliderSnapsTo.Ticks)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (ShowRange)
                    {
                        double val = 0d;
                        if (IsDirectionReversed)
                        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        val = UpdateValue(ActualWidth - pPoint.X);
                    }
                    else
                        val = UpdateValue(pPoint.X);
#elif WINRT
                        val = UpdateValue(ActualWidth - pPoint.Position.X);
                    }
                    else
                        val = UpdateValue(pPoint.Position.X);
#endif
                    double rangeStartDiff = Math.Abs(val - IntermediateRangeStart);
                    double rangeEndDiff = Math.Abs(IntermediateRangeEnd - val);
                    if (rangeEndDiff - rangeStartDiff == 0d)
                    {
                        if (val > IntermediateRangeEnd)
                        {
                            isRangeEnd = true;
                            UpdateRangeEnd(val);
                        }

                        if (val < IntermediateRangeStart)
                        {
                            UpdateRangeStart(val);
                        }
                    }
                    else
                    {

                        if (rangeStartDiff < rangeEndDiff)
                        {
                            isRangeEnd = false;
                            UpdateRangeStart(val);
                        }
                        else
                        {
                            isRangeEnd = true;
                            UpdateRangeEnd(val);
                        }
                    }
                }
                else
                {                   
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                    UpdateThumbToolTip();
                    horizontalRangeEndThumbToolTip.VerticalOffset = ToolTipDistance;
#endif
                    if (IsDirectionReversed)
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        UpdateValue(ActualWidth - pPoint.X);
                    else
                        UpdateValue(pPoint.X);
#elif WINRT
                        UpdateValue(ActualWidth - pPoint.Position.X);
                    else
                        UpdateValue(pPoint.Position.X);
#endif
                }
            }
            else
            {
                if (ShowRange)
                {
                    double val = 0d;
                    if (IsDirectionReversed)
                    {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        val = UpdateValue(pPoint.Y);
                    }
                    else
                        val = UpdateValue(ActualHeight - pPoint.Y);
#elif WINRT
                        val = UpdateValue(pPoint.Position.Y);
                    }
                    else
                        val = UpdateValue(ActualHeight - pPoint.Position.Y);
#endif
                    double rangeStartDiff = Math.Abs(val - IntermediateRangeStart);
                    double rangeEndDiff = Math.Abs(IntermediateRangeEnd - val);
                    if (rangeEndDiff - rangeStartDiff == 0d)
                    {
                        if (val > IntermediateRangeEnd)
                        {
                            isRangeEnd = true;
                            UpdateRangeEnd(val);
                        }

                        if (val < IntermediateRangeStart)
                        {
                            UpdateRangeStart(val);
                        }
                    }
                    else
                    {
                        if (rangeStartDiff < rangeEndDiff)
                        {
                            isRangeEnd = false;
                            UpdateRangeStart(val);
                        }
                        else
                        {
                            isRangeEnd = true;
                            UpdateRangeEnd(val);
                        }
                    }
                }
                else
                {                  
                    if (IsDirectionReversed)
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        UpdateValue(pPoint.Y);
                    else
                        UpdateValue(ActualHeight - pPoint.Y);
#elif WINRT
                        UpdateValue(pPoint.Position.Y);
                    else
                        UpdateValue(ActualHeight - pPoint.Position.Y);
#endif
                    }
                }
            }

#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseLeftButtonDown(e);
#elif WINRT
            base.OnPointerPressed(e);   
#endif
        }
#if WPF
        /// <summary>
        /// Gets the position of the pointer when the mouse is moved
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MousePoint = e.GetPosition(this);
            if ((MousePoint.Y <= ActualHeight || MousePoint.X <= ActualWidth) && (MousePoint.Y >= 0 || MousePoint.X >= 0))
                isMouseEnter = true;
            base.OnMouseMove(e);
        }
#endif
        /// <summary>
        /// Occurs when the pointer is moved
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
#if WPF
        protected override void OnMouseDown(MouseButtonEventArgs e)
#else
        protected override void OnMouseMove(MouseEventArgs e)
#endif
#elif WINRT
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            isPointerMove = true;
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            MousePoint = e.GetPosition(this);          
            if ((MousePoint.Y <= ActualHeight || MousePoint.X <=ActualWidth) && (MousePoint.Y >= 0 || MousePoint.X >= 0))
             isMouseEnter = true;
#elif WINRT
            MousePointerPoint = e.GetCurrentPoint(this);
            if ((MousePointerPoint.Position.Y <= ActualHeight || MousePointerPoint.Position.X <= ActualWidth) && (MousePointerPoint.Position.Y >= 0 || MousePointerPoint.Position.X >= 0))
                isMouseEnter = true;
#endif



            if (isPointerDown&& (MoveToPoint!=MovePoint.None || SnapsTo==SliderSnapsTo.Ticks))
            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                Point pPoint = e.GetPosition(this);
#elif WINRT
                PointerPoint pPoint = e.GetCurrentPoint(this);
#endif

                if (Orientation == Orientation.Horizontal)
                {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    double diff = pPoint.X - startPoint.X;
#elif WINRT
                    double diff = pPoint.Position.X - startPoint.Position.X;
#endif

                    if (ShowRange)
                    {
                        double val = 0d;
                        if (IsDirectionReversed)
                        {
                            if (isRangeEnd)
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                horizontalRangeEndThumb.CaptureMouse();
#elif WINRT
                               horizontalRangeEndThumb.CapturePointer(e.Pointer);     
#endif
                                val = UpdateValue(IntermediateRangeEnd - (diff * ((Maximum - Minimum) / ActualWidth)));
                                UpdateRangeEnd(val);
                            }
                            else
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                horizontalRangeStartThumb.CaptureMouse();
#elif WINRT
                                horizontalRangeStartThumb.CapturePointer(e.Pointer); 
#endif
                                val = UpdateValue(IntermediateRangeStart - (diff * ((Maximum - Minimum) / ActualWidth)));
                                UpdateRangeStart(val);
                            }

                        }
                        else
                        {
                            if (isRangeEnd)
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                horizontalRangeEndThumb.CaptureMouse();
                                horizontalRangeEndThumb.Focus();
#elif WINRT
                                horizontalRangeEndThumb.CapturePointer(e.Pointer);
#endif
                                val = UpdateValue(IntermediateRangeEnd + (diff * ((Maximum - Minimum) / ActualWidth)));
                                UpdateRangeEnd(val);
                            }
                            else
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                horizontalRangeStartThumb.CaptureMouse();
                                horizontalRangeStartThumb.Focus();
#elif WINRT
                                horizontalRangeStartThumb.CapturePointer(e.Pointer);
#endif
                                val = UpdateValue(IntermediateRangeStart + (diff * ((Maximum - Minimum) / ActualWidth)));
                                UpdateRangeStart(val);
                            }

                        }

                    }
                    else
                    {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        horizontalRangeEndThumb.CaptureMouse();
#elif WINRT
                        horizontalRangeEndThumb.CapturePointer(e.Pointer);
#endif
                        if (IsDirectionReversed)
                        {
                            UpdateValue(IntermediateValue - (diff * ((Maximum - Minimum) / ActualWidth)));
                        }
                        else
                            UpdateValue(IntermediateValue + (diff * ((Maximum - Minimum) / ActualWidth)));
                    }
                }
                else
                {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                    double diff = pPoint.Y - startPoint.Y;
#elif WINRT
                    double diff = pPoint.Position.Y - startPoint.Position.Y;
#endif
                    if (ShowRange)
                    {
                        double val = 0d;
                        if (IsDirectionReversed)
                        {
                            if (isRangeEnd)
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                verticalRangeEndThumb.CaptureMouse();
#elif WINRT
                                verticalRangeEndThumb.CapturePointer(e.Pointer);
#endif
                                val = UpdateValue(IntermediateRangeEnd + (diff * ((Maximum - Minimum) / ActualHeight)));
                                UpdateRangeEnd(val);
                            }
                            else
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                verticalRangeStartThumb.CaptureMouse();
#elif WINRT
                                verticalRangeStartThumb.CapturePointer(e.Pointer); 
#endif
                                val = UpdateValue(IntermediateRangeStart + (diff * ((Maximum - Minimum) / ActualHeight)));
                                UpdateRangeStart(val);
                            }

                        }
                        else
                        {
                            if (isRangeEnd)
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                verticalRangeEndThumb.CaptureMouse();
#elif WINRT
                                verticalRangeEndThumb.CapturePointer(e.Pointer);
#endif
                                val = UpdateValue(IntermediateRangeEnd - (diff * ((Maximum - Minimum) / ActualHeight)));
                                UpdateRangeEnd(val);
                            }
                            else
                            {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                                verticalRangeStartThumb.CaptureMouse();
#elif WINRT
                                verticalRangeStartThumb.CapturePointer(e.Pointer); 
#endif
                                val = UpdateValue(IntermediateRangeStart - (diff * ((Maximum - Minimum) / ActualHeight)));
                                UpdateRangeStart(val);
                            }

                        }

                    }
                    else
                    {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
                        verticalRangeEndThumb.CaptureMouse();
#elif WINRT
                        verticalRangeEndThumb.CapturePointer(e.Pointer); 
#endif
                        if (IsDirectionReversed)
                        {
                            UpdateValue(IntermediateValue + (diff * ((Maximum - Minimum) / ActualHeight)));
                        }
                        else
                            UpdateValue(IntermediateValue - (diff * ((Maximum - Minimum) / ActualHeight)));
                    }

                }
                startPoint = pPoint;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                UpdateThumbToolTipConstantOffset();
#endif
            }
            isPointerMove = false;

#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
#if WPF
            base.OnMouseDown(e);
#endif
            base.OnMouseMove(e);
#elif WINRT
            base.OnPointerMoved(e);
#endif
        }

        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
#elif WINRT
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            VisualStateManager.GoToState(this, "PointerOver", true);
            if (Maximum > RangeEnd && Maximum > RangeStart && Minimum<RangeStart && Minimum<RangeEnd)
            {
                if (ShowRange)
                {
                    IntermediateRangeStart = RangeStart;
                    IntermediateRangeEnd = RangeEnd;
                }
                else
                    IntermediateValue = Value;
            }
            isPointerDown = false;
            isRangeEnd = false;
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            horizontalRangeStartThumb.ReleaseMouseCapture();
            horizontalRangeEndThumb.ReleaseMouseCapture();
            verticalRangeStartThumb.ReleaseMouseCapture();
            verticalRangeEndThumb.ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
#elif WINRT
            horizontalRangeStartThumb.ReleasePointerCaptures();
            horizontalRangeEndThumb.ReleasePointerCaptures();
            verticalRangeStartThumb.ReleasePointerCaptures();
            verticalRangeEndThumb.ReleasePointerCaptures();
            base.OnPointerReleased(e);
#endif
        }

        /// <summary>
        /// Occurs when the pointer entered
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#elif WINRT
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            isPointerDown = false;
            VisualStateManager.GoToState(this, "PointerOver", true);
            UpdateMovePoint();
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseEnter(e);
#elif WINRT
            base.OnPointerEntered(e);
#endif
        }

        /// <summary>
        /// Occurs when the pointer exited
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#elif WINRT
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            VisualStateManager.GoToState(this, "Normal", true);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            base.OnMouseLeave(e);
#elif WINRT
            base.OnPointerExited(e);
#endif
        }

        /// <summary>
        /// Occurs when the key is pressed
        /// </summary>
        /// <param name="e"></param>
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
#elif WINRT
        protected override void OnKeyDown(KeyRoutedEventArgs e)
#endif
        {
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (ShowRange && e.Key == Key.Tab)
#elif WINRT
            //Focus(Windows.UI.Xaml.FocusState.Keyboard);
            if (ShowRange && e.Key == Windows.System.VirtualKey.Tab)
#endif
            {
                ExecuteTabKey(e);
            }
#if WINRT
            else if(!ShowRange && e.Key == Windows.System.VirtualKey.Home)
#else
            else if (!ShowRange && e.Key == Key.Home)
#endif
            {
                Value = Minimum;
            }
#if WINRT
            else if (!ShowRange && e.Key == Windows.System.VirtualKey.End)
#else
            else if (!ShowRange && e.Key == Key.End)
#endif
            {
                Value =Maximum;
            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            else if(e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down||e.Key == Key.PageUp || e.Key == Key.PageDown)
#elif WINRT
            else if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.PageUp || e.Key == Windows.System.VirtualKey.PageDown)
#endif
            {
                ExecuteDirectionalKeys(e);
            }

            base.OnKeyDown(e);
            
        }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        private void ExecuteDirectionalKeys(System.Windows.Input.KeyEventArgs e)
#elif WINRT
        private void ExecuteDirectionalKeys(KeyRoutedEventArgs e)
#endif
        {
            double newValue = 0d;
            double preValue = 0d;
            double step = 0d;
            if (SnapsTo == SliderSnapsTo.Ticks && TickFrequency != 0d)
                step = TickFrequency;
            else
                step = SmallChange;

            if (ShowRange)
            {
                if (isFirstThumbFocused)
                    preValue = IntermediateRangeStart;
                if (isSecondThumbFocused)
                    preValue = IntermediateRangeEnd;
            }
            else
                preValue = Value;
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (e.Key == Key.Up || e.Key == Key.Right)
#elif WINRT
            if (e.Key == Windows.System.VirtualKey.Up || e.Key == VirtualKey.Right)
#endif
            {
                if (IsDirectionReversed)
                    newValue = preValue - step;
                else
                    newValue = preValue + step;
            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (e.Key ==Key.Down || e.Key == Key.Left)
#elif WINRT
            if (e.Key == Windows.System.VirtualKey.Down || e.Key == VirtualKey.Left)
#endif
            {
                if (IsDirectionReversed)
                    newValue = preValue + step;
                else
                    newValue = preValue - step;
            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (e.Key ==Key.PageUp)
#elif WINRT
            if (e.Key == Windows.System.VirtualKey.PageUp)
#endif
            {
                if (IsDirectionReversed)
                    newValue =preValue - LargeChange;
                else
                    newValue = preValue + LargeChange;
            }
#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
            if (e.Key ==Key.PageDown)
#elif WINRT
            if (e.Key == Windows.System.VirtualKey.PageDown)
#endif
            {
                if (IsDirectionReversed)
                    newValue = preValue + LargeChange;
                else
                    newValue = preValue - LargeChange;
            }
            if (newValue > Maximum)
                newValue = Maximum;
            if (newValue < Minimum)
                newValue = Minimum;
            if (ShowRange)
            {
                if (isFirstThumbFocused)
                {
                    if (newValue <= IntermediateRangeEnd)
                    {
                        isRangeStartChangedInternally = true;
                        IntermediateRangeStart = newValue;
                        RangeStart = IntermediateRangeStart;
                        isRangeStartChangedInternally = false;
                    }
                }
                if (isSecondThumbFocused)
                {
                    if (newValue >= IntermediateRangeStart)
                    {
                        isRangeEndChangedInternally = true;
                        IntermediateRangeEnd = newValue;
                        RangeEnd = IntermediateRangeEnd;
                        isRangeEndChangedInternally = false;
                    }
                }
            }
            else
            {
                isValueChangedInternally = true;
                IntermediateValue = newValue;
                Value = IntermediateValue;
                isValueChangedInternally = false;
            }
            e.Handled = true;
        }

#if WPF || SILVERLIGHT || WINDOWS_PHONE || WINDOWS_PHONE_7
        private void ExecuteTabKey(System.Windows.Input.KeyEventArgs e)
#elif WINRT
        private void ExecuteTabKey(KeyRoutedEventArgs e)
#endif
        {
#if WINDOWS_PHONE_7 || WPF|| SILVERLIGHT
            bool isShiftKeyPressed=(ModifierKeys.Shift==System.Windows.Input.Keyboard.Modifiers);
#else
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            bool isShiftKeyPressed = (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
#endif
            bool isFirst = false, isSecond = false;
            if (ShowRange)
            {
                if (!isShiftKeyPressed)
                {
                    if (Orientation == Orientation.Horizontal && !IsDirectionReversed && isFirstThumbFocused)
                        isSecond = true;
                    if (Orientation == Orientation.Vertical && IsDirectionReversed && isFirstThumbFocused)
                        isSecond = true;
                    if (Orientation == Orientation.Vertical && !IsDirectionReversed && isSecondThumbFocused)
                        isFirst = true;
                    if (Orientation == Orientation.Horizontal && IsDirectionReversed && isSecondThumbFocused)
                        isFirst = true;
                }

                else
                {
                    if (Orientation == Orientation.Horizontal && !IsDirectionReversed && isSecondThumbFocused)
                        isFirst = true;
                    if (Orientation == Orientation.Vertical && !IsDirectionReversed && isFirstThumbFocused)
                        isSecond = true;
                    if (Orientation == Orientation.Horizontal && IsDirectionReversed && isFirstThumbFocused)
                        isSecond = true;
                    if (Orientation == Orientation.Vertical && IsDirectionReversed && isSecondThumbFocused)
                        isFirst = true;
                }
            }

            if (isFirst || isSecond)
            {
                e.Handled = true;
                if (isFirst)
                {
                    isFirstThumbFocused = true;
                    isSecondThumbFocused = false;
                }
                if (isSecond)
                {
                    isSecondThumbFocused = true;
                    isFirstThumbFocused = false;
                }

            }
            else
            {
                isSecondThumbFocused = false;
                isFirstThumbFocused = false;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (e.Handled)
            {
                UpdateThumbToolTip();
                if (isFirstThumbFocused)
                {
                    if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    {
                        if (Orientation == Orientation.Horizontal)
                            horizontalRangeStartThumbToolTip.IsOpen = true;
                        else
                            verticalRangeStartThumbToolTip.IsOpen = true;
                    }
                    verticalRangeEndThumbToolTip.IsOpen = false;
                    horizontalRangeEndThumbToolTip.IsOpen = false;
                }
                if (isSecondThumbFocused)
                {
                    if (ThumbToolTipPlacement != ThumbToolTipPlacement.None)
                    {
                        if (Orientation == Orientation.Horizontal)
                            horizontalRangeEndThumbToolTip.IsOpen = true;
                        else
                            verticalRangeEndThumbToolTip.IsOpen = true;
                    }
                    verticalRangeStartThumbToolTip.IsOpen = false;
                    horizontalRangeStartThumbToolTip.IsOpen = false;
                }
            }
#endif
        }

#if !(WINDOWS_PHONE||WINDOWS_PHONE_7||WINRT)
        /// <summary>
        /// Closes the ToolTips
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            CloseToolTips();
            base.OnLostMouseCapture(e);
        }
#endif
        /// <summary>
        /// Occurs when the focus is obtained
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
#if WPFSILVERLIGHT
#if WPF
            if(this.IsFocused)
#elif !Silverlight4
            if (FocusManager.GetFocusedElement(this)==this)
#else
             if (FocusManager.GetFocusedElement()==this)
#endif
#elif  WINDOWS_PHONE || WINDOWS_PHONE_7
            if (FocusManager.GetFocusedElement() == this)
#elif WINRT
            if (FocusState == FocusState.Keyboard)
#endif
            {
                VisualStateManager.GoToState(this, "Focused", true);
                if (ShowRange)
                {
                    if (!IsDirectionReversed)
                    {
                        if (RangeStart == RangeEnd || Orientation == Orientation.Vertical)
                            isSecondThumbFocused = true;
                        else
                            isFirstThumbFocused = true;
                    }
                    else
                    {
                        if (RangeStart == RangeEnd || Orientation == Orientation.Horizontal)
                            isSecondThumbFocused = true;
                        else
                            isFirstThumbFocused = true;
                    }
                }
                else
                    isSecondThumbFocused = true;
            }
            else
                VisualStateManager.GoToState(this, "Unfocused", true);
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Occurs when the focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Unfocused", true);
            if (ShowRange)
            {
                isFirstThumbFocused = false;
                isSecondThumbFocused = false;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
            base.OnLostFocus(e);
        }
        
        #endregion

        #region Callback Methods

        static void OnIsDirectionReversed(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnIsDirectionReversed(args);
        }

        static void OnIntermediateRangeStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnIntermediateRangeStartChanged(args);
        }

        static void OnIntermediateRangeEndChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnIntermediateRangeEndChanged(args);
        }

        static void OnRangeStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnRangeStartChanged(args);
        }

        static void OnRangeEndChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnRangeEndChanged(args);
        }

        private void OnRangeStartChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Maximum >= (double) args.NewValue && Minimum <= (double) args.NewValue)
            {
                if ((this.RangeChanged != null) && (this.ShowRange))
                {
                    RangeChangedEventArgs rangeArgs = new RangeChangedEventArgs()
                        {
                            NewStartValue = args.NewValue,
                            OldStartValue = args.OldValue
                        };
                    this.RangeChanged(this, rangeArgs);
                }

                if (IsLoaded && RangeStart > RangeEnd)
                    RangeEnd =RangeStart;

                if (!isRangeStartChangedInternally)
                {
                    isRangeStartChangedExternally = true;
                    IntermediateRangeStart = RangeStart;
                    isRangeStartChangedExternally = false;
                }
            }
            else if ((Maximum <= (double)args.NewValue && Minimum >= (double)args.NewValue)&& IsLoaded)
            {
                RangeStart =(double) args.OldValue;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (!isDragDelta && !isPointerDown)
                CloseToolTips();
            else
                UpdateThumbToolTip();
#endif
        }

        private void OnRangeEndChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Maximum >= (double) args.NewValue && Minimum<=(double)args.NewValue)
            {
                if ((this.RangeChanged != null) && (this.ShowRange))
                {
                    RangeChangedEventArgs rangeArgs = new RangeChangedEventArgs()
                        {
                            NewEndValue = args.NewValue,
                            OldEndValue = args.OldValue
                        };
                    this.RangeChanged(this, rangeArgs);
                }

                if (IsLoaded && RangeEnd < RangeStart)
                    RangeEnd = (double) args.OldValue;
                if (!isRangeEndChangedInternally)
                {
                    isRangeEndChangedExternally = true;
                    IntermediateRangeEnd = RangeEnd;
                    isRangeEndChangedExternally = false;
                }
            }
            else if ((Maximum <= (double)args.NewValue || Minimum >= (double)args.NewValue)&& IsLoaded)
            {
                RangeEnd = (double) args.OldValue;
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (!isDragDelta && !isPointerDown)
                CloseToolTips();
            else
                UpdateThumbToolTip();
#endif
        }

        private void UpdateAllThumbsTransform()
        {
            if (ShowRange)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    UpdateThumbTransform(horizontalRangeEndThumb, RangeEnd);
                    UpdateThumbTransform(horizontalRangeStartThumb, RangeStart);                    
                }
                else
                {
                    UpdateThumbTransform(verticalRangeEndThumb, RangeEnd);
                    UpdateThumbTransform(verticalRangeStartThumb, RangeStart);
                }
            }
            else
            {
                if(Orientation == Orientation.Horizontal)
                    UpdateThumbTransform(horizontalRangeEndThumb, Value);
                else
                    UpdateThumbTransform(verticalRangeEndThumb, Value);
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            CloseToolTips();
#endif
        }

        private void OnIsDirectionReversed(DependencyPropertyChangedEventArgs args)
        {
            UpdateAllThumbsTransform();
        }

        private double GetRangeValue(double rangeValue)
        {
            double returnValue = 0d;
            double frequency = 0d;
            if (SnapsTo == SliderSnapsTo.StepValues)
            {
                frequency = StepFrequency;
            }
            else if (SnapsTo == SliderSnapsTo.Ticks)
            {
                frequency = TickFrequency;
            }

            if (frequency != 0d)
            {
                double remainder = Math.Abs(rangeValue % frequency);
                if (remainder < (frequency / 2))
                {
                    if (rangeValue >= 0d)
                        returnValue = rangeValue - remainder;
                    else
                        returnValue = rangeValue + remainder;
                }
                else
                {
                    if (rangeValue >= 0d)
                        returnValue = rangeValue + (frequency - remainder);
                    else
                        returnValue = rangeValue - (frequency - remainder);
                }
            }
            else
                returnValue = rangeValue;

            return returnValue;
        }

        private void OnIntermediateRangeStartChanged(DependencyPropertyChangedEventArgs args)
        {
            isRangeStartChangedInternally = true;
            if (RangeStart < Minimum)
                RangeStart = Minimum;
            if (RangeStart > Maximum)
                RangeStart = RangeEnd;

            if (!isRangeStartChangedExternally)
            {
                RangeStart = GetRangeValue(IntermediateRangeStart);
            }
            if (ShowRange)
            {
                if (Orientation == Orientation.Horizontal)
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                    UpdateThumbTransform(horizontalRangeStartThumb, RangeStart);
					else
                    UpdateThumbTransform(horizontalRangeStartThumb, IntermediateRangeStart);
                }
                else
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                    UpdateThumbTransform(verticalRangeStartThumb, RangeStart);
					else
                    UpdateThumbTransform(verticalRangeStartThumb, IntermediateRangeStart);
                }
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (Orientation == Orientation.Horizontal)
            {
                if (horizontalRangeStartThumbToolTip != null)
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                    horizontalRangeStartThumbToolTip.Content = RangeStart.ToString("F" + ThumbToolTipPrecision);
					else
                    horizontalRangeStartThumbToolTip.Content = IntermediateRangeStart.ToString("F" + ThumbToolTipPrecision);
                }
            }
            else
            {
                if (verticalRangeStartThumbToolTip != null)
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                    verticalRangeStartThumbToolTip.Content = RangeStart.ToString("F"+ThumbToolTipPrecision);
					else
                    verticalRangeStartThumbToolTip.Content = IntermediateRangeStart.ToString("F"+ThumbToolTipPrecision);
                }
            }
#endif
            isRangeStartChangedInternally = false;
        }

        private void OnIntermediateRangeEndChanged(DependencyPropertyChangedEventArgs args)
        {

            isRangeEndChangedInternally = true;

            if (RangeEnd < Minimum)
                RangeEnd = Minimum;
            if (RangeEnd >= Maximum)
                RangeEnd = Maximum;
            if (!isRangeEndChangedExternally)
            {
                RangeEnd = GetRangeValue(IntermediateRangeEnd);
            }
            if (ShowRange)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (SnapsTo == SliderSnapsTo.Ticks)
                        UpdateThumbTransform(horizontalRangeEndThumb, RangeEnd);
                    else
                        UpdateThumbTransform(horizontalRangeEndThumb, IntermediateRangeEnd);
                }
                else
                {
                    if (SnapsTo == SliderSnapsTo.Ticks)
                        UpdateThumbTransform(verticalRangeEndThumb, RangeEnd);
                    else
                        UpdateThumbTransform(verticalRangeEndThumb, IntermediateRangeEnd);
                }
            }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
            if (Orientation == Orientation.Horizontal)
            {
                if (horizontalRangeEndThumbToolTip != null)
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                        horizontalRangeEndThumbToolTip.Content = RangeEnd.ToString("F" + ThumbToolTipPrecision);
					else
				        horizontalRangeEndThumbToolTip.Content = IntermediateRangeEnd.ToString("F" + ThumbToolTipPrecision);
                }
            }
            else
            {
                if (verticalRangeEndThumbToolTip != null)
                {
					if(SnapsTo==SliderSnapsTo.Ticks)
                    verticalRangeEndThumbToolTip.Content = RangeEnd.ToString("F" + ThumbToolTipPrecision);
					else
                    verticalRangeEndThumbToolTip.Content = IntermediateRangeEnd.ToString("F" + ThumbToolTipPrecision);
                }
            }
#endif

            isRangeEndChangedInternally = false;
        }

        static void OnShowRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateShowRange();
        }

        static void OnMovePointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateMovePoint();
        }

        static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if(args.NewValue.ToString() != args.OldValue.ToString())
                (sender as SfRangeSlider).UpdateOrientation();            
        }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
        static void OnThumbToolTipPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateThumbToolTip();
        }
#endif
        static void OnIntermediateValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).OnIntermediateValueChanged(args);
        }

        static void OnTickPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }
        static void OnLabelOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }

        static void OnLabelPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }

        static void OnValuePlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }

        static void OnShowCustomLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }

        static void OnShowValueLabelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as SfRangeSlider).UpdateTickPlacement();
            (sender as SfRangeSlider).UpdateLabelPlacement();
            (sender as SfRangeSlider).UpdateValuePlacement();
        }
        #endregion
      
        #region Events
        /// <summary>
        /// Occurs when RangeStart or RangeEnd gets changed<see
        /// cref="P:Syncfusion.UI.Xaml.Controls.Layout.Input"/> is changed.
        /// </summary>
        //[ClassReference(IsReviewed = false)]
        public event RangeChangedEventHandler RangeChanged;
        #endregion

    }
    
    /// <summary>
    /// specify the mouse move position
    /// </summary>
    public enum MovePoint
    {
        /// <summary>
        /// Thumb doesn't move when clicked using mouse
        /// </summary>
        None,

        /// <summary>
        /// Thumb move to the current tap position
        /// </summary>
        MoveToTapPosition,
        /// <summary>
        /// Increment RangeValue by LargeChange
        /// </summary>
        IncrementByLargeChange,
        /// <summary>
        /// Increment RangeValue by SmallChange
        /// </summary>
        IncrementBySmallChange
    }
}
