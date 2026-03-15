#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Tick Placement Enum.
    /// </summary>
    public enum Tickplacement
    {
        /// <summary>
        /// Top Placement.
        /// </summary>
        Top,
        /// <summary>
        /// Down Placement.
        /// </summary>
        Down,
        /// <summary>
        /// Left Placement.
        /// </summary>
        Left,
        /// <summary>
        /// Right Placement.
        /// </summary>
        Right,
        /// <summary>
        /// None Placement.
        /// </summary>
        None,
        /// <summary>
        /// Both Placement.
        /// </summary>
        Both,
    }
    /// <summary>
    /// Represents the Alignment of the Custom Label
    /// </summary>
    public enum CustomLabelAlignment
    {
        /// <summary>
        /// Horizontal Alignment of Label
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical Alignment of Label
        /// </summary>
        Vertical
    }
    /// <summary>
    /// Represents the Range Slider Class.
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
       Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Blend;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2007Black;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Default;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2010Black;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
       Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Windows7;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
      Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.VS2010;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
   Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Metro;component/RangeSlider.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
 Type = typeof(RangeSlider), XamlResource = "/Syncfusion.Theming.Transparent;component/RangeSlider.xaml")]


    public class RangeSlider : Control
    {
        /// <summary>
        /// Identifies the RangeProperty Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RangeProperty = DependencyProperty.Register("Range", typeof(DoubleRange), typeof(RangeSlider), new PropertyMetadata(DoubleRange.Empty, new PropertyChangedCallback(IsRangeChanged)));
        /// <summary>
        /// Gets or sets the range.
        /// </summary>
        /// <value>The range.</value>
        public DoubleRange Range
        {
            get
            {
                return (DoubleRange)GetValue(RangeProperty);
            }
            set
            {
                SetValue(RangeProperty, value);


            }
        }
        /// <summary>
        /// Identifies the RangeFill Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RangeFillProperty = DependencyProperty.Register("RangeFill", typeof(Brush), typeof(RangeSlider), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// Gets or sets the range fill.
        /// </summary>
        /// <value>The range fill.</value>
        public Brush RangeFill
        {
            get
            {
                return (Brush)GetValue(RangeFillProperty);
            }
            set
            {
                SetValue(RangeFillProperty, value);


            }
        }

        /// <summary>
        /// Represents the SetCustomLabel Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SetCustomLabelProperty = DependencyProperty.Register("SetCustomLabel", typeof(bool), typeof(RangeSlider), new PropertyMetadata(false, new PropertyChangedCallback(IsSetCustomLabelChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether [set custom label].
        /// </summary>
        /// <value><c>true</c> if [set custom label]; otherwise, <c>false</c>.</value>
        public bool SetCustomLabel
        {
            get
            {
                return (bool)GetValue(SetCustomLabelProperty);
            }
            set
            {
                SetValue(SetCustomLabelProperty, value);


            }
        }
        /// <summary>
        /// Represents the ItemsCollection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemsCollectionProperty = DependencyProperty.Register("ItemsCollection", typeof(ObservableCollection<Items>), typeof(RangeSlider), new PropertyMetadata(null, new PropertyChangedCallback(IsItemsCollectionChanged)));
        /// <summary>
        /// Gets or sets the items collection.
        /// </summary>
        /// <value>The items collection.</value>
        public ObservableCollection<Items> ItemsCollection
        {
            get
            {
                return (ObservableCollection<Items>)GetValue(ItemsCollectionProperty);
            }
            set
            {
                ObservableCollection<Items> ItemsCollectionValue = Coerse(value);
                SetValue(ItemsCollectionProperty, ItemsCollectionValue);

            }
        }

        /// <summary>
        /// Coerses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private ObservableCollection<Items> Coerse(ObservableCollection<Items> value)
        {

            List<Items> temp = new List<Items>(value);
            //temp.Sort((x, y) => string.Compare(x.Title, y.Title));
            temp.Sort(delegate(Items i1, Items i2) { return i1.value.CompareTo(i2.value); });
            //ObservableCollection<Items> result = new ObservableCollection<CSomeClass>(temp);
            value.Clear();
            foreach (Items i in temp)
            {
                value.Add(i);
            }
            return value;

        }
        /// <summary>
        /// Identifies the LabelVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register("LabelVisibility", typeof(bool), typeof(RangeSlider), new PropertyMetadata(true, new PropertyChangedCallback(IsLabelVisibilityChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether [label visibility].
        /// </summary>
        /// <value><c>true</c> if [label visibility]; otherwise, <c>false</c>.</value>
        public bool LabelVisibility
        {
            get
            {
                return (bool)GetValue(LabelVisibilityProperty);
            }
            set
            {
                SetValue(LabelVisibilityProperty, value);


            }
        }

        /// <summary>
        /// Identifies the LabelOrientation Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelOrientationProperty = DependencyProperty.Register("LabelOrientation", typeof(Itemorientation), typeof(RangeSlider), new PropertyMetadata(Itemorientation.AboveTicks, new PropertyChangedCallback(IsLabelOrientationChanged)));
        /// <summary>
        /// Gets or sets the label orientation.
        /// </summary>
        /// <value>The label orientation.</value>
        public Itemorientation LabelOrientation
        {
            get
            {
                return (Itemorientation)GetValue(LabelOrientationProperty);
            }
            set
            {
                SetValue(LabelOrientationProperty, value);
            }
        }
        /// <summary>
        /// Identifies the TickPlacement Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TickplacementProperty = DependencyProperty.Register("TickPlacement", typeof(Tickplacement), typeof(RangeSlider), new PropertyMetadata(Tickplacement.Both, new PropertyChangedCallback(IsTickPlacementChanged)));
        /// <summary>
        /// Gets or sets the tick placement.
        /// </summary>
        /// <value>The tick placement.</value>
        public Tickplacement TickPlacement
        {
            get
            {
                return (Tickplacement)GetValue(TickplacementProperty);
            }
            set
            {
                SetValue(TickplacementProperty, value);
            }
        }
        /// <summary>
        /// Identifies the MoveToHandle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MoveToHandleProperty = DependencyProperty.Register("MoveToHandle", typeof(bool), typeof(RangeSlider), new PropertyMetadata(false, new PropertyChangedCallback(IsMoveToHandleChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether [move to handle].
        /// </summary>
        /// <value><c>true</c> if [move to handle]; otherwise, <c>false</c>.</value>
        public bool MoveToHandle
        {
            get
            {
                return (bool)GetValue(MoveToHandleProperty);
            }
            set
            {
                SetValue(MoveToHandleProperty, value);
            }
        }
        /// <summary>
        /// Identifies the RangeVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RangeVisibilityProperty = DependencyProperty.Register("RangeVisibility", typeof(bool), typeof(RangeSlider), new PropertyMetadata(true, new PropertyChangedCallback(IsRangeVisibilityChanged)));
        /// <summary>
        /// Gets or sets a value indicating whether [range visibility].
        /// </summary>
        /// <value><c>true</c> if [range visibility]; otherwise, <c>false</c>.</value>
        public bool RangeVisibility
        {
            get
            {
                return (bool)GetValue(RangeVisibilityProperty);
            }
            set
            {
                SetValue(RangeVisibilityProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Orientation Dependency Property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RangeSlider), new PropertyMetadata(Orientation.Horizontal, new PropertyChangedCallback(IsOrientationChanged)));
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);


            }
        }
        /// <summary>
        /// Identifies the ControlWidth Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty ControlWidthProperty = DependencyProperty.Register("ControlWidth", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the width of the control.
        /// </summary>
        /// <value>The width of the control.</value>
        public double ControlWidth
        {
            get
            {
                return (double)GetValue(ControlWidthProperty);
            }
            set
            {
                SetValue(ControlWidthProperty, value);


            }
        }
        /// <summary>
        /// Identifies the ControlHeight Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty ControlHeightProperty = DependencyProperty.Register("ControlHeight", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the height of the control.
        /// </summary>
        /// <value>The height of the control.</value>
        public double ControlHeight
        {
            get
            {
                return (double)GetValue(ControlHeightProperty);
            }
            set
            {
                SetValue(ControlHeightProperty, value);


            }
        }
        /// <summary>
        /// Identifies the ControlGridHeight Dependency Property.
        /// </summary>
        internal static readonly DependencyProperty ControlGridHeightProperty = DependencyProperty.Register("ControlGridHeight", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d));
        /// <summary>
        /// Gets or sets the height of the control grid.
        /// </summary>
        /// <value>The height of the control grid.</value>
        public double ControlGridHeight
        {
            get
            {
                return (double)GetValue(ControlGridHeightProperty);
            }
            set
            {
                SetValue(ControlGridHeightProperty, value);


            }
        }
        /// <summary>
        /// Identifies the TickFrequency Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider), new PropertyMetadata(5d, new PropertyChangedCallback(IsTickFrequencyChanged)));
        /// <summary>
        /// Gets or sets the tick frequency.
        /// </summary>
        /// <value>The tick frequency.</value>
        public double TickFrequency
        {
            get
            {
                return (double)GetValue(TickFrequencyProperty);
            }
            set
            {
                SetValue(TickFrequencyProperty, value);
            }
        }
        /// <summary>
        /// Identifies the TickFrequency Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HandleButtonVisibilityProperty = DependencyProperty.Register("HandleButtonVisibility", typeof(Visibility), typeof(RangeSlider), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(IsHandleVisibiltyChanged)));
        /// <summary>
        /// Gets or sets the handle button visibility.
        /// </summary>
        /// <value>The handle button visibility.</value>
        public Visibility HandleButtonVisibility
        {
            get
            {
                return (Visibility)GetValue(HandleButtonVisibilityProperty);
            }
            set
            {
                SetValue(HandleButtonVisibilityProperty, value);

            }
        }

        /// <summary>
        /// Identifies the UniformChange Dependency Property.
        /// </summary>
        public static readonly DependencyProperty UniformChangeProperty = DependencyProperty.Register("UniformChange", typeof(double), typeof(RangeSlider), new PropertyMetadata(1d, new PropertyChangedCallback(IsUniformChanged)));

        /// <summary>
        /// Gets or sets the uniform change.
        /// </summary>
        /// <value>The uniform change.</value>
        public double UniformChange
        {
            get
            {
                return (double)GetValue(UniformChangeProperty);
            }
            set
            {
                SetValue(UniformChangeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the LargeChange Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeSlider), new PropertyMetadata(1d));

        /// <summary>
        /// Gets or sets the large change.
        /// </summary>
        /// <value>The large change.</value>
        public double LargeChange
        {
            get
            {
                return (double)GetValue(LargeChangeProperty);
            }
            set
            {
                SetValue(LargeChangeProperty, value);
            }
        }
        /// <summary>
        /// Identifies the LargeChange Dependency Property.
        /// </summary>
        public static readonly DependencyProperty RangeValueProperty = DependencyProperty.Register("RangeValue", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d, new PropertyChangedCallback(IsRangeValueChanged)));
        /// <summary>
        /// Gets or sets the range value.
        /// </summary>
        /// <value>The range value.</value>
        public double RangeValue
        {
            get
            {
                return (double)GetValue(RangeValueProperty);
            }
            set
            {
                SetValue(RangeValueProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d, new PropertyChangedCallback(IsValueChanged)));
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }
        /// <summary>
        /// Identifies the Value Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new PropertyMetadata(0d, new PropertyChangedCallback(IsMinimumChanged)));
        /// <summary>
        /// Gets or sets the minimum.
        /// </summary>
        /// <value>The minimum.</value>
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumValueProperty);
            }
            set
            {
                SetValue(MinimumValueProperty, value);
            }

        }

        /// <summary>
        /// Identifies the Maximum Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new PropertyMetadata(100d, new PropertyChangedCallback(IsMaximumChanged)));
        /// <summary>
        /// Gets or sets the maximum.
        /// </summary>
        /// <value>The maximum.</value>
        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumValueProperty);
            }
            set
            {
                SetValue(MaximumValueProperty, value);
            }
        }
        /// <summary>
        /// Identifies the HorizontalLeftRepeatButtonTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLeftRepeatButtonTemplateProperty = DependencyProperty.Register("HorizontalLeftRepeatButtonTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HorizontalRightRepeatButtonTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalRightRepeatButtonTemplateProperty = DependencyProperty.Register("HorizontalRightRepeatButtonTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the VerticalTopRepeatButtonTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VerticalTopRepeatButtonTemplateProperty = DependencyProperty.Register("VerticalTopRepeatButtonTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the VerticalDownRepeatButtonTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VerticalDownRepeatButtonTemplateProperty = DependencyProperty.Register("VerticalDownRepeatButtonTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the HorizontalThumbTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HorizontalThumbTemplateProperty = DependencyProperty.Register("HorizontalThumbTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the VerticalThumbTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty VerticalThumbTemplateProperty = DependencyProperty.Register("VerticalThumbTemplate", typeof(ControlTemplate), typeof(RangeSlider), new PropertyMetadata(null));



        /// <summary>
        /// Gets or sets the horizontal thumb template.
        /// </summary>
        /// <value>The horizontal thumb template.</value>
        public ControlTemplate HorizontalThumbTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(HorizontalThumbTemplateProperty);
            }
            set
            {
                SetValue(HorizontalThumbTemplateProperty, value);


            }
        }
        /// <summary>
        /// Gets or sets the vertical thumb template.
        /// </summary>
        /// <value>The vertical thumb template.</value>
        public ControlTemplate VerticalThumbTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(VerticalThumbTemplateProperty);
            }
            set
            {
                SetValue(VerticalThumbTemplateProperty, value);


            }
        }
        /// <summary>
        /// Gets or sets the horizontal left repeat button template.
        /// </summary>
        /// <value>The horizontal left repeat button template.</value>
        public ControlTemplate HorizontalLeftRepeatButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(HorizontalLeftRepeatButtonTemplateProperty);
            }
            set
            {
                SetValue(HorizontalLeftRepeatButtonTemplateProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the horizontal right repeat button template.
        /// </summary>
        /// <value>The horizontal right repeat button template.</value>
        public ControlTemplate HorizontalRightRepeatButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(HorizontalRightRepeatButtonTemplateProperty);
            }
            set
            {
                SetValue(HorizontalRightRepeatButtonTemplateProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the vertical top repeat button template.
        /// </summary>
        /// <value>The vertical top repeat button template.</value>
        public ControlTemplate VerticalTopRepeatButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(VerticalTopRepeatButtonTemplateProperty);
            }
            set
            {
                SetValue(VerticalTopRepeatButtonTemplateProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the vertical down repeat button template.
        /// </summary>
        /// <value>The vertical down repeat button template.</value>
        public ControlTemplate VerticalDownRepeatButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(VerticalDownRepeatButtonTemplateProperty);
            }
            set
            {
                SetValue(VerticalDownRepeatButtonTemplateProperty, value);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsSnapToTickEnabled
        {
            get { return (bool)GetValue(IsSnapToTickEnabledProperty); }
            set { SetValue(IsSnapToTickEnabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RangeSlider), new PropertyMetadata(false));



        /// <summary>
        /// 
        /// </summary>
        public CustomLabelAlignment LabelAlignment  
        {
            get { return (CustomLabelAlignment)GetValue(LabelAlignmentProperty); }
            set { SetValue(LabelAlignmentProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LabelAlignmentProperty =
            DependencyProperty.Register("LabelAlignment", typeof(CustomLabelAlignment), typeof(RangeSlider), new PropertyMetadata(CustomLabelAlignment.Horizontal));

        


        /// <summary>
        /// Occurs when [range changed].
        /// </summary>
        public event PropertyChangedCallback RangeChanged;
        /// <summary>
        /// Occurs when [value changed].
        /// </summary>
        public event PropertyChangedCallback ValueChanged;
        /// <summary>
        /// 
        /// </summary>
        public RangeSlider()
        {
            DefaultStyleKey = typeof(RangeSlider);
        }

        /// <summary>
        /// Initializes the <see cref="RangeSlider"/> class.
        /// </summary>
        static RangeSlider()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }

        }
        Thumb thumb, thumbsecond, verticalthumb, verticalthumbsecond;
        Rectangle rect, trackrect, verticalrect, verticaltrackrect, trackinnerrect, verticaltrackinnerrect;

        RepeatButton incButton, decButton, backButton, rectButton, verticalrectButton;

        RepeatButton verticalincButton, verticaldecButton, verticalbackButton;

        Grid vertical, horizontal, main;
        Canvas can1, verticalcan1;
        CustomPanelTicks custompanel, custompaneldown, custompanelvertical, custompanelverticaldown;
        CustomPanelLabel custompanellabel, custompanellabeldown, CustomPanelVerticalLabel, CustomPanelVerticalLabelDown;
        //Button b1;
        Point ordinates;
        Border controlBorder;
        //double interval;
        double ControlGridWidth;
        bool FirstThumbIsPressed = false;
        bool SecondThumbIsPressed = false;

        /// <summary>
        /// Local property which gets the width between two ticks.
        /// </summary>
        private double DifferenceWidth
        {
            get
            {
                return ((ControlWidth) / Math.Ceiling((Maximum - Minimum) / TickFrequency) - 1);//custompanel.Children.Count - 2);
            }
        }

        /// <summary>
        /// Local property which gets the height between two ticks.
        /// </summary>
        private double DifferenceHeight
        {
            get
            {
                return ((ControlHeight) / Math.Ceiling((Maximum - Minimum) / TickFrequency) - 1);
            }
        }

        /// <summary>
        /// Local variable represents the Limit Exceeded flag used in Snap To Ticks.
        /// </summary>
        private bool m_isLimitExceeded = false;
        /// <summary>
        /// Local variable represents the last two ticks interval.
        /// </summary>
        private double m_lastTickWidth = 0.0;

        /// <summary>
        /// Local variable represents the last two ticks interval.
        /// </summary>
        private double m_lastTickHeight = 0.0;

        private double m_difference = 0.0;

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            thumb = GetTemplateChild("HorizontalThumb") as Thumb;
            verticalthumb = GetTemplateChild("VerticalThumb") as Thumb;
            thumb.DragDelta += new DragDeltaEventHandler(thumbDragDelta);
            thumb.MouseLeave += new MouseEventHandler(thumb_MouseLeave);
            controlBorder = GetTemplateChild("ControlBorder") as Border;
            thumb.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(thumb_MouseLeftButtonDown), true);
            thumb.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(thumb_MouseLeftButtonUp), true);
            //thumb.MouseLeftButtonUp += new MouseButtonEventHandler(thumb_MouseLeftButtonUp);
            verticalthumb.DragDelta += new DragDeltaEventHandler(verticalthumb_DragDelta);
            verticalthumb.MouseLeave += new MouseEventHandler(thumb_MouseLeave);
            //verticalthumb.MouseLeftButtonUp += new MouseButtonEventHandler(thumb_MouseLeftButtonUp);
            verticalthumb.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(thumb_MouseLeftButtonDown), true);
            verticalthumb.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(thumb_MouseLeftButtonUp), true);
            trackrect = GetTemplateChild("TrackRectangle") as Rectangle;
            verticaltrackrect = GetTemplateChild("VerticalTrackRectangle") as Rectangle;
            trackinnerrect = GetTemplateChild("TrackInnerRect") as Rectangle;
            verticaltrackinnerrect = GetTemplateChild("VerticalTrackInnerRectangle") as Rectangle;
            thumbsecond = GetTemplateChild("HorizontalThumbSecond") as Thumb;
            thumbsecond.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(thumbsecond_MouseLeftButtonDown), true);
            thumbsecond.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(thumbsecond_MouseLeftButtonUp), true);
            thumbsecond.MouseLeave += new MouseEventHandler(thumbsecond_MouseLeave);
            //thumbsecond.MouseLeftButtonUp += new MouseButtonEventHandler(thumbsecond_MouseLeftButtonUp);
            verticalthumbsecond = GetTemplateChild("VerticalThumbSecond") as Thumb;
            rectButton = GetTemplateChild("RectButton") as RepeatButton;
            rectButton.Click += new RoutedEventHandler(rectButton_Click);
            verticalthumbsecond.DragDelta += new DragDeltaEventHandler(verticalthumbsecond_DragDelta);
            verticalthumbsecond.MouseLeave += new MouseEventHandler(thumbsecond_MouseLeave);
            verticalthumbsecond.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(thumbsecond_MouseLeftButtonUp), true);
            verticalthumbsecond.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(thumbsecond_MouseLeftButtonDown), true);
            //verticalthumbsecond.MouseLeftButtonUp += new MouseButtonEventHandler(thumbsecond_MouseLeftButtonUp);
            thumbsecond.DragDelta += new DragDeltaEventHandler(thumbSecondDragDelta);
            rect = GetTemplateChild("Rect1") as Rectangle;
            verticalrect = GetTemplateChild("VerticalRect1") as Rectangle;
            verticalcan1 = GetTemplateChild("VerticalCan1") as Canvas;
            verticalcan1.MouseMove += new MouseEventHandler(verticalcan1_MouseMove);
            incButton = GetTemplateChild("IncreaseButton") as RepeatButton;
            verticalincButton = GetTemplateChild("VerticalIncreaseButton") as RepeatButton;
            decButton = GetTemplateChild("DecreaseButton") as RepeatButton;
            verticaldecButton = GetTemplateChild("VerticalDecreaseButton") as RepeatButton;
            backButton = GetTemplateChild("BackgroundButton") as RepeatButton;
            verticalbackButton = GetTemplateChild("VerticalBackgroundButton") as RepeatButton;
            verticalbackButton.MouseEnter += new MouseEventHandler(verticalbackButton_MouseEnter);
            verticalbackButton.MouseMove += new MouseEventHandler(verticalbackButton_MouseEnter);
            rectButton.MouseEnter += new MouseEventHandler(rectButton_MouseEnter);
            rectButton.MouseMove += new MouseEventHandler(rectButton_MouseEnter);
            backButton.MouseEnter += new MouseEventHandler(backButton_MouseEnter);
            backButton.MouseMove += new MouseEventHandler(backButton_MouseEnter);
            backButton.Click += new RoutedEventHandler(backButton_Click);
            verticalbackButton.Click += new RoutedEventHandler(verticalbackButton_Click);
            decButton.Click += new RoutedEventHandler(decButton_Click);
            verticaldecButton.Click += new RoutedEventHandler(decButton_Click);
            incButton.Click += new RoutedEventHandler(incButton_Click);
            verticalincButton.Click += new RoutedEventHandler(incButton_Click);
            vertical = GetTemplateChild("VerticalTemplate") as Grid;
            double h = vertical.ActualHeight;
            main = GetTemplateChild("MainGrid") as Grid;
            main.SizeChanged += new SizeChangedEventHandler(main_SizeChanged);

            if (this.Parent != null & (this.Parent as FrameworkElement != null))
            {
                (this.Parent as FrameworkElement).SizeChanged += new SizeChangedEventHandler(RangeSliderControl_SizeChanged);
            }

            horizontal = GetTemplateChild("HorizontalTemplate") as Grid;
            custompanel = GetTemplateChild("CustomPanel") as CustomPanelTicks;
            custompanellabel = GetTemplateChild("CustomPanellabel") as CustomPanelLabel;
            custompanellabeldown = GetTemplateChild("CustomPanellabeldown") as CustomPanelLabel;
            custompaneldown = GetTemplateChild("CustomPaneldown") as CustomPanelTicks;
            custompanelvertical = GetTemplateChild("CustomPanelVertical") as CustomPanelTicks;
            custompanelverticaldown = GetTemplateChild("CustomPanelVerticaldown") as CustomPanelTicks;
            CustomPanelVerticalLabel = GetTemplateChild("CustomPanelVerticalLabel") as CustomPanelLabel;
            verticalrectButton = GetTemplateChild("VerticalRectButton") as RepeatButton;
            verticalrectButton.Click += new RoutedEventHandler(verticalrectButton_Click);
            verticalrectButton.MouseEnter += new MouseEventHandler(verticalrectButton_MouseEnter);
            verticalrectButton.MouseMove += new MouseEventHandler(verticalrectButton_MouseEnter);
            CustomPanelVerticalLabelDown = GetTemplateChild("CustomPanelVerticalLabelDown") as CustomPanelLabel;
            can1 = GetTemplateChild("Can1") as Canvas;
            can1.MouseMove += new MouseEventHandler(can1_MouseMove);

        }

        void verticalcan1_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (canvas != null && FirstThumbIsPressed)
            {
                double oldtop = Canvas.GetTop(verticalthumb);
                double secondthumbtop = Canvas.GetTop(verticalthumb);
                double distance = Math.Ceiling(e.GetPosition(this).Y) - oldtop;
                double top = Canvas.GetTop(verticalthumb) + distance;
                double height1 = verticaltrackrect.ActualHeight;
                double left = Canvas.GetLeft(verticalthumb);
                newvalue = top;
                if (IsSnapToTickEnabled)
                {
                    ResetValue();
                }
                if (this.RangeVisibility)
                {
                    if(IsSnapToTickEnabled)
                    {
                        if (distance > 0)
                        {
                            if (Range.Start + TickFrequency > Maximum)
                            {
                                double _top = Canvas.GetTop(verticalthumb);
                                m_lastTickHeight = ControlHeight - _top;
                                if (distance > m_lastTickHeight / 2)
                                {
                                    m_difference = Maximum - Value;
                                    Range.Start = Maximum;
                                    m_isLimitExceeded = true;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (distance > (DifferenceHeight / 2))
                                {
                                    Range.Start = Range.Start + TickFrequency;
                                    if (Range.Start > Maximum)
                                        Range.Start = Maximum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + verticalthumb.Height / 6;
                                }
                            }
                        }
                        else
                        {
                            if (!m_isLimitExceeded)
                            {
                                if (-distance > DifferenceHeight / 2)
                                {
                                    Range.Start = Range.Start - TickFrequency;
                                    if (Range.Start < Minimum)
                                        Range.Start = Minimum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + verticalthumb.Height / 6;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (-distance > (m_lastTickHeight / 2))
                                {
                                    Range.Start = Range.Start - m_difference;
                                    if (Range.Start < Minimum)
                                        Range.Start = Minimum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + verticalthumb.Height / 6;
                                    m_isLimitExceeded = false;
                                    m_difference = 0.0;
                                    distance = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(IsSnapToTickEnabled)
                    {
                        if (distance > 0)
                        {
                            if (Value + TickFrequency > Maximum)
                            {
                                double _top = Canvas.GetTop(verticalthumb);
                                m_lastTickHeight = ControlHeight - _top;
                                if (distance > m_lastTickWidth / 2)
                                {
                                    m_difference = Maximum - Value;
                                    Value = Maximum;
                                    m_isLimitExceeded = true;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (distance > DifferenceHeight / 2)
                                {
                                    Value = Value + TickFrequency;
                                    if (Value > Maximum)
                                        Value = Maximum;
                                    distance = 0;
                                }
                            }
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                        else
                        {
                            if (!m_isLimitExceeded)
                            {
                                if (-distance > DifferenceHeight / 2)
                                {
                                    Value = Value - TickFrequency;
                                    if (Value < Minimum)
                                        Value = Minimum;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (-distance > m_lastTickHeight / 2)
                                {
                                    Value = Value - m_difference;
                                    if (Value < Minimum)
                                        Value = Minimum;
                                    m_isLimitExceeded = false;
                                    m_difference = 0.0;
                                    distance = 0;
                                }
                            }
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                    }
                }
                Canvas.SetLeft(verticalthumb, left);
            }
            else if (canvas != null && SecondThumbIsPressed)
            {
                double firstthumbtop = Canvas.GetTop(verticalthumb) + verticalthumb.Height / 2;

                double height1 = verticaltrackrect.ActualHeight;
                double oldtop = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2;
                double distance = Math.Ceiling(e.GetPosition(this).Y) - Canvas.GetTop(verticalthumbsecond);
                double top = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2 + (distance);
                double left = Canvas.GetLeft(verticalthumbsecond);
                newvalue = top;
                VisualStateManager.GoToState(verticalthumbsecond, "Focus", false);

                VisualStateManager.GoToState(verticalthumb, "MouseOver", false);
                if (IsSnapToTickEnabled)
                {
                    ResetValue();
                }
                if(IsSnapToTickEnabled)
                {
                    if (distance > 0)
                    {
                        if (Range.End + TickFrequency > Maximum)
                        {
                            double _top = Canvas.GetTop(verticalthumbsecond);
                            m_lastTickHeight = ControlHeight - _top;
                            if (distance > m_lastTickHeight / 2)
                            {
                                m_difference = Maximum - Value;
                                Range.End = Maximum;
                                m_isLimitExceeded = true;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (distance > DifferenceHeight / 2)
                            {
                                Range.End = Range.End + TickFrequency;
                                if (Range.End > Maximum)
                                    Range.End = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                    else
                    {
                        if (m_isLimitExceeded)
                        {
                            if (-distance > m_lastTickHeight / 2)
                            {
                                Range.End = Range.End - m_difference;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + verticalthumbsecond.Height / 2;
                                m_difference = 0.0;
                                m_isLimitExceeded = false;
                            }
                        }
                        else
                        {
                            if (-distance > DifferenceHeight / 2)
                            {
                                Range.End = Range.End - TickFrequency;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }
                Canvas.SetLeft(verticalthumbsecond, left);
                thumb1focus = false;
                thumb2focus = true;
                VisualStateManager.GoToState(verticalthumbsecond, "Focus", true);
                VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
            }
        }

        void thumbsecond_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thumb thb = sender as Thumb;
            if(thb !=null)
            {
                SecondThumbIsPressed=true;
            }
        }

        void can1_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (canvas != null && FirstThumbIsPressed)
            {

                double oldleft = Canvas.GetLeft(thumb);
                double distance = Math.Ceiling(e.GetPosition(this).X) - oldleft;
                double secondthumbleft = Canvas.GetLeft(thumbsecond);
                double left = Canvas.GetLeft(thumb) + (e.GetPosition(this).X - oldleft);
                newvalue = left;

                double width1 = trackrect.ActualWidth;
                double top = Canvas.GetTop(thumb);
                if (IsSnapToTickEnabled)
                {
                    ResetValue();
                }
                if (this.RangeVisibility)
                {
                    if(IsSnapToTickEnabled)
                    {
                        if (distance > 0)
                        {
                            if (Range.Start + TickFrequency > Maximum)
                            {
                                double _left = Canvas.GetLeft(thumb);
                                m_lastTickWidth = trackrect.ActualWidth - _left;
                                if (distance > m_lastTickWidth / 2)
                                {
                                    m_difference = Maximum - Value;
                                    Range.Start = Maximum;
                                    m_isLimitExceeded = true;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (distance > (DifferenceWidth / 2))
                                {
                                    Range.Start = Range.Start + TickFrequency;
                                    if (Range.Start > Maximum)
                                        Range.Start = Maximum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                                    distance = 0;
                                }
                            }
                        }
                        else
                        {
                            if (!m_isLimitExceeded)
                            {
                                if (-distance > DifferenceWidth / 2)
                                {
                                    Range.Start = Range.Start - TickFrequency;
                                    if (Range.Start < Minimum)
                                        Range.Start = Minimum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                                    distance = 0;
                                }
                            }
                            else
                            {
                                if (-distance > (m_lastTickWidth / 2))
                                {
                                    Range.Start = Range.Start - m_difference;
                                    if (Range.Start < Minimum)
                                        Range.Start = Minimum;
                                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                                    m_isLimitExceeded = false;
                                    m_difference = 0.0;
                                    distance = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (left < (width1 + thumb.Width / 6) && left > thumb.Width / 6)
                    {
                        if(IsSnapToTickEnabled)
                        {
                            if (distance > 0)
                            {
                                if (Value + TickFrequency > Maximum)
                                {
                                    double _left = Canvas.GetLeft(thumb);
                                    m_lastTickWidth = trackrect.ActualWidth - _left;
                                    if (distance > m_lastTickWidth / 2)
                                    {
                                        m_difference = Maximum - Value;
                                        Value = Maximum;
                                        m_isLimitExceeded = true;
                                        distance = 0;
                                    }
                                }
                                else
                                {
                                    if (distance > DifferenceWidth / 2)
                                    {
                                        Value = Value + TickFrequency;
                                        if (Value > Maximum)
                                            Value = Maximum;
                                        distance = 0;
                                    }
                                }
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                            else
                            {
                                if (!m_isLimitExceeded)
                                {
                                    if (-distance > DifferenceWidth / 2)
                                    {
                                        Value = Value - TickFrequency;
                                        if (Value < Minimum)
                                            Value = Minimum;
                                        distance = 0;
                                    }
                                }
                                else
                                {
                                    if (-distance > m_lastTickWidth / 2)
                                    {
                                        Value = Value - m_difference;
                                        if (Value < Minimum)
                                            Value = Minimum;
                                        m_isLimitExceeded = false;
                                        m_difference = 0.0;
                                        distance = 0;
                                    }
                                }
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }

                    }
                    else if (left <= thumb.Width / 6)
                    {
                        Canvas.SetLeft(thumb, thumb.Width / 6);
                        Value = Minimum;
                        if (!(Calculate(Value) < 0))
                        {
                            RangeValue = Calculate(Value);
                        }
                        else
                        {
                            RangeValue = 0;
                        }
                    }
                    else if (left + thumb.Width / 2 >= width1)
                    {
                        Canvas.SetLeft(thumb, width1 + thumb.Width / 6);
                        Value = Maximum;
                        if (!(Calculate(Value) < 0))
                        {
                            RangeValue = Calculate(Value);
                        }
                        else
                        {
                            RangeValue = 0;
                        }
                    }

                }
                Canvas.SetTop(thumb, top);
            }
            else if (canvas != null && thumbsecond != null && SecondThumbIsPressed)
            {
                double firstthumbleft = Canvas.GetLeft(thumb) + thumb.Width / 2;
                double width1 = trackrect.ActualWidth;
                double distance = Math.Ceiling(e.GetPosition(this).X) - Canvas.GetLeft(thumbsecond);
                double oldleft = Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2;
                double left = Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2 + (e.GetPosition(this).X-Canvas.GetLeft(thumbsecond));
                newvalue = left;
                double top = Canvas.GetTop(thumbsecond);
                
                Debug.WriteLine(Value.ToString());

                if (IsSnapToTickEnabled)
                {
                    ResetValue();
                }

                Debug.WriteLine(Value.ToString());

                if(IsSnapToTickEnabled)
                {
                    if (distance > 0)
                    {
                        if (Range.End + TickFrequency > Maximum)
                        {
                            double _left = Canvas.GetLeft(thumbsecond);
                            m_lastTickWidth = trackrect.ActualWidth - _left;
                            if (distance > m_lastTickWidth / 2)
                            {
                                m_difference = Maximum - Value;
                                Range.End = Maximum;
                                m_isLimitExceeded = true;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                                distance = 0.0;
                            }
                        }
                        else
                        {
                            if (distance > DifferenceWidth / 2)
                            {
                                Range.End = Range.End + TickFrequency;
                                if (Range.End > Maximum)
                                    Range.End = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                                distance = 0.0;
                            }
                        }
                    }
                    else
                    {
                        if (m_isLimitExceeded)
                        {
                            if (-distance > m_lastTickWidth / 2)
                            {
                                Range.End = Range.End - m_difference;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                                m_difference = 0.0;
                                m_isLimitExceeded = false;
                                distance = 0.0;
                            }
                        }
                        else
                        {
                            if (-distance > DifferenceWidth / 2)
                            {
                                Range.End = Range.End - TickFrequency;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                                distance = 0.0;
                            }
                        }
                    }
                }
                Canvas.SetTop(thumbsecond, top);
                thumb1focus = false;
                thumb2focus = true;
                VisualStateManager.GoToState(thumbsecond, "Focus", true);
                VisualStateManager.GoToState(thumb, "MouseEnter", true);
            }
        }

        void RangeSliderControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement elem = sender as FrameworkElement;
            if ((double.IsNaN(this.Height) && double.IsNaN(this.Width)))
            {
                if (elem != null)
                {
                    if (elem.ActualHeight != 0 && elem.ActualWidth != 0)
                    {
                        this.controlBorder.Height = elem.ActualHeight;
                        this.controlBorder.Width = elem.ActualWidth + this.BorderThickness.Left + this.BorderThickness.Right;
                        this.ControlGridHeight = (elem.ActualHeight) - this.BorderThickness.Bottom;
                        ControlGridWidth = elem.ActualWidth;
                        if (HandleButtonVisibility == Visibility.Visible)
                        {
                            ControlWidth = ControlGridWidth - 46;
                            ControlHeight = ControlGridHeight - 46;

                            can1.Margin = new Thickness(0, 0, 0, 0);
                            verticalcan1.Margin = new Thickness(0, 0, 0, 0);
                            trackrect.Margin = new Thickness(5, 0, 5, 0);
                            backButton.Margin = new Thickness(5, 0, 5, 0);
                            verticaltrackrect.Margin = new Thickness(0, 5, 0, 5);
                            verticalbackButton.Margin = new Thickness(0, 5, 0, 5);

                        }
                        else
                        {
                            ControlWidth = ControlGridWidth - 46;
                            ControlHeight = ControlGridHeight - 46;

                            can1.Margin = new Thickness(18, 0, 0, 0);
                            trackrect.Margin = new Thickness(23, 0, 23, 0);
                            backButton.Margin = new Thickness(23, 0, 23, 0);
                            verticalcan1.Margin = new Thickness(0, 18, 0, 0);
                            verticaltrackrect.Margin = new Thickness(0, 23, 0, 23);
                            verticalbackButton.Margin = new Thickness(0, 23, 0, 23);

                        }
                        if (LabelVisibility)
                        {
                            if (SetCustomLabel)
                            {
                                if (Orientation == Orientation.Horizontal)
                                {
                                    AddLabel();
                                    custompanellabel.minimum = Minimum;
                                    custompanellabel.maximum = Maximum;
                                    custompanel.minimum = Minimum;
                                    custompanel.maximum = Maximum;
                                    AddLabelDown();
                                    custompanellabeldown.minimum = Minimum;
                                    custompanellabeldown.maximum = Maximum;
                                    custompaneldown.minimum = Minimum;
                                    custompaneldown.maximum = Maximum;
                                }
                                else
                                {
                                    AddLabelVertical();
                                    CustomPanelVerticalLabel.minimum = Minimum;
                                    CustomPanelVerticalLabel.maximum = Maximum;
                                    custompanelvertical.minimum = Minimum;
                                    custompanelvertical.maximum = Maximum;
                                    AddLabelVerticalDown();
                                    custompanelverticaldown.minimum = Minimum;
                                    custompanelverticaldown.maximum = Maximum;
                                    CustomPanelVerticalLabelDown.minimum = Minimum;
                                    CustomPanelVerticalLabelDown.maximum = Maximum;
                                }
                            }
                            else
                            {
                                if (Orientation == Orientation.Horizontal)
                                {

                                    AddChildren();
                                    AddChildrendown();
                                }
                                else
                                {
                                    AddChildrenVertical();
                                    AddChildrenVerticalDown();
                                }

                            }
                        }

                        if (Range.Start == Range.End && Range.Start == Minimum)
                        {

                            thumb2focus = true;
                            thumb1focus = false;
                        }
                        else
                        {

                            thumb1focus = true;
                            thumb2focus = false;
                        }
                        ArrangeSlider();
                        controlBorder.Height = elem.ActualHeight;
                        controlBorder.Width = elem.ActualWidth;
                        controlBorder.BorderThickness = this.BorderThickness;

                        controlBorder.BorderBrush = this.BorderBrush;
                    }
                }
            }
        }

        private ObservableCollection<double> Ticks;

        private void PopulateItems()
        {
            Ticks = new ObservableCollection<double>();
            double Childcount = ((this.Maximum - this.Minimum)) / ((this.TickFrequency==0) ? 1:this.TickFrequency);
            double count = Minimum;
            for (int i = 0; i <= Childcount + 1; i++)
            {
                double tick = 0.0;
                if (!SetCustomLabel)
                {
                    if (i + 1 > (Childcount + 1))
                    {
                        tick = Maximum;
                    }
                    else
                    {
                        tick = count;
                    }
                }
                else
                {
                    custompanellabel.IsItem = true;
                    custompanellabel.LabelOrientation = LabelOrientation;
                }
                count += this.TickFrequency;
                Ticks.Add(tick);
            }
        }

        private bool ResetValue(double oldleft, double firstleft, double Xvalue)
        {
            PopulateItems();

            if (Ticks != null)
            {
                if (!RangeVisibility)
                {
                    if (Ticks.Contains(Value))
                        return false;
                    try
                    {
                        //Choosing Minimum threshold.
                        double _min = (from tick in Ticks
                                       where Value > tick
                                       select tick).Last();

                        //Choosing Maximum threshold.
                        double _max = (from tick in Ticks
                                       where Value < tick
                                       select tick).First();

                        Value = firstleft > Xvalue ? _min : _max;
                        return true;
                    }
                    catch { }
                }
                else
                {
                    bool ret = false;
                    if (!Ticks.Contains(Range.Start))
                    {
                        try
                        {
                            //Choosing Minimum threshold.
                            double _min = (from tick in Ticks
                                           where Range.Start > tick
                                           select tick).Last();

                            //Choosing Maximum threshold.
                            double _max = (from tick in Ticks
                                           where Range.Start < tick
                                           select tick).First();


                            double min_diff = Range.Start - _min;
                            double max_diff = _max - Range.Start;
                            Range.Start = max_diff > min_diff ? _min : _max;

                            ret = true;
                        }
                        catch { }
                    }
                    else if (!Ticks.Contains(Range.End))
                    {
                        try
                        {
                            //Choosing Minimum threshold.
                            double _min = (from tick in Ticks
                                           where Range.End > tick
                                           select tick).Last();

                            //Choosing Maximum threshold.
                            double _max = (from tick in Ticks
                                           where Range.End < tick
                                           select tick).First();


                            double min_diff = Range.End - _min;
                            double max_diff = _max - Range.End;
                            Range.End = max_diff > min_diff ? _min : _max;
                            ret = true;
                        }
                        catch { }
                    }
                    if (ret)
                        return true;
                }
            }
            return false;
        }

        private void ResetValue()
        {
            PopulateItems();

            if (Ticks != null)
            {
                if (!RangeVisibility)
                {
                    if (Ticks.Contains(Value))
                        return;
                    try
                    {
                        //Choosing Minimum threshold.
                        double _min = (from tick in Ticks
                                       where Value > tick
                                       select tick).Last();

                        //Choosing Maximum threshold.
                        double _max = (from tick in Ticks
                                       where Value < tick
                                       select tick).First();


                        double min_diff = Value - _min;
                        double max_diff = _max - Value;

                        Value = max_diff > min_diff ? _min : _max;
                    }
                    catch { }
                }
                else
                {
                    if (!Ticks.Contains(Range.Start))
                    {
                        try
                        {
                            //Choosing Minimum threshold.
                            double _min = (from tick in Ticks
                                           where Range.Start > tick
                                           select tick).Last();

                            //Choosing Maximum threshold.
                            double _max = (from tick in Ticks
                                           where Range.Start < tick
                                           select tick).First();


                            double min_diff = Range.Start - _min;
                            double max_diff = _max - Range.Start;

                            Range.Start = max_diff > min_diff ? _min : _max;
                        }
                        catch { }
                    }
                    else if (!Ticks.Contains(Range.End))
                    {
                        try
                        {
                            //Choosing Minimum threshold.
                            double _min = (from tick in Ticks
                                           where Range.End > tick
                                           select tick).Last();

                            //Choosing Maximum threshold.
                            double _max = (from tick in Ticks
                                           where Range.End < tick
                                           select tick).First();


                            double min_diff = Range.End - _min;
                            double max_diff = _max - Range.End;

                            Range.End = max_diff > min_diff ? _min : _max;
                        }
                        catch { }
                    }
                }
            }
        }

        private void ResetValueOnDec()
        {
            PopulateItems();
            if (Ticks != null)
            {
                if (Ticks.Contains(Value))
                    return;

                //Choosing Minimum threshold.
                double _min = (from tick in Ticks
                               where Value > tick
                               select tick).Last();

                //Choosing Maximum threshold.
                double _max = (from tick in Ticks
                               where Value < tick
                               select tick).First();


                double min_diff = Value - _min;
                double max_diff = _max - Value;

                Value = _max;
                if (RangeVisibility)
                {
                    Range.Start = _max;
                }
            }
        }

        private void ResetValueOnInc()
        {
            PopulateItems();
            if (Ticks != null)
            {
                if (Ticks.Contains(Value))
                    return;

                //Choosing Minimum threshold.
                double _min = (from tick in Ticks
                               where Value > tick
                               select tick).Last();

                //Choosing Maximum threshold.
                double _max = (from tick in Ticks
                               where Value < tick
                               select tick).First();


                double min_diff = Value - _min;
                double max_diff = _max - Value;

                Value = _min;

                if (RangeVisibility)
                {
                    Range.End = _min;
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the verticalrectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void verticalrectButton_MouseEnter(object sender, MouseEventArgs e)
        {
            rectOrdinates = e.GetPosition(verticalcan1);
        }

        /// <summary>
        /// Handles the Click event of the verticalrectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void verticalrectButton_Click(object sender, RoutedEventArgs e)
        {
            bool toupdate = false;
            if (IsSnapToTickEnabled)
            {
                toupdate = ResetValue(Canvas.GetLeft(thumbsecond), Canvas.GetLeft(thumb), rectOrdinates.X);
            }
            if (thumb1focus)
            {
                double oldleft = Canvas.GetTop(verticalthumb);
                double left = 0d;
                if (!MoveToHandle)
                {
                    left = Canvas.GetTop(verticalthumb) + CalculateUniformRange(this.LargeChange);
                }
                else
                {
                    left = rectOrdinates.Y - verticalthumb.Height / 2;
                }


                if (RangeVisibility)
                {
                    if (!IsSnapToTickEnabled)
                    {
                        if (left < rectOrdinates.Y && !MoveToHandle)
                        {
                            Canvas.SetTop(verticalthumb, left);
                            Range.Start = CalculateRange(left - oldleft) + Range.Start;

                            Canvas.SetTop(verticalrect, left);
                        }
                        else if (!MoveToHandle)
                        {
                            Canvas.SetTop(verticalthumb, rectOrdinates.Y - verticalthumb.Height / 2);
                            Range.Start = CalculateRange(rectOrdinates.Y - oldleft) + Range.Start;

                            Canvas.SetTop(verticalrect, rectOrdinates.Y);


                        }
                        else
                        {
                            Canvas.SetTop(verticalthumb, left);
                            Range.Start = CalculateRange(left - oldleft) + Range.Start;

                            Canvas.SetTop(verticalrect, left);

                        }

                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                    }
                    else
                    {
                        left = Canvas.GetTop(verticalthumb);
                        if (!toupdate)
                        {
                            if (left < rectOrdinates.Y)
                            {
                                Range.Start = Range.Start + TickFrequency;
                                if (Range.Start > Maximum)
                                    Range.Start = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Range.Start = Range.Start - TickFrequency;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }

                }
                else
                {
                    left = Canvas.GetTop(verticalthumb) - CalculateUniformRange(this.LargeChange);

                    if (!IsSnapToTickEnabled)
                    {
                        if (left > rectOrdinates.Y && !MoveToHandle)
                        {

                            Canvas.SetTop(verticalthumb, left);
                            //Canvas.SetLeft(rect, left);
                            Value = Value - CalculateRange(oldleft - left);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                        else if (!MoveToHandle)
                        {
                            Canvas.SetTop(verticalthumb, rectOrdinates.Y - verticalthumb.Height / 2);
                            Value = Value - CalculateRange(oldleft - rectOrdinates.Y);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                        else
                        {
                            Canvas.SetTop(verticalthumb, rectOrdinates.Y - verticalthumb.Height / 2);
                            Value = Value - CalculateRange(oldleft - rectOrdinates.Y);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                    }
                    else
                    {
                        left = Canvas.GetTop(verticalthumb);
                        if (!toupdate)
                        {
                            if (left < rectOrdinates.Y)
                            {
                                Value = Value + TickFrequency;
                                if (Value > Maximum)
                                    Value = Maximum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }

                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (Value < Minimum)
                                    Value = Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                    }


                }
            }
            else if (thumb2focus)
            {
                double oldleft = Canvas.GetTop(verticalthumbsecond);
                double left = 0d;
                if (!IsSnapToTickEnabled)
                {
                    if (!MoveToHandle)
                    {
                        left = Canvas.GetTop(verticalthumbsecond) - CalculateUniformRange(this.LargeChange);
                    }
                    else
                    {
                        left = rectOrdinates.Y - verticalthumbsecond.Height / 2;
                    }

                    //Canvas.SetLeft(thumb, left);
                    //Range.End = Range.End - CalculateRange(oldleft - left);
                    //Range.Start = CalculateRange(left - oldleft) + Range.Start;
                    if (left > rectOrdinates.Y && !MoveToHandle)
                    {
                        Canvas.SetTop(verticalthumbsecond, left);
                        //Range.Start = CalculateRange(left - oldleft) + Range.Start;
                        Range.End = Range.End - CalculateRange(oldleft - left);
                        //Canvas.SetLeft(rect, left);
                    }
                    else if (!MoveToHandle)
                    {
                        Canvas.SetTop(verticalthumbsecond, rectOrdinates.Y - verticalthumbsecond.Height);
                        Range.End = Range.End - CalculateRange(oldleft - rectOrdinates.Y);
                        // Range.Start = CalculateRange(rectOrdinates.X - oldleft) + Range.Start;

                        //Canvas.SetLeft(rect, rectOrdinates.X);


                    }
                    else
                    {
                        Canvas.SetTop(verticalthumbsecond, left);
                        //Range.Start = CalculateRange(left - oldleft) + Range.Start;
                        Range.End = Range.End - CalculateRange(oldleft - left);


                    }
                }
                else
                {
                    if (!toupdate)
                    {
                        if (!RangeVisibility)
                        {
                            left = Canvas.GetTop(thumbsecond);
                            if (rectOrdinates.Y > left)
                            {
                                Value = Value + TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                        else
                        {
                            left = Canvas.GetTop(thumbsecond);
                            if (rectOrdinates.Y > left)
                            {
                                Range.End = Range.End + TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Range.End = Range.End - TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }

                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the rectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void rectButton_MouseEnter(object sender, MouseEventArgs e)
        {
            rectOrdinates = e.GetPosition(can1);
        }

        /// <summary>
        /// Handles the Click event of the rectButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void rectButton_Click(object sender, RoutedEventArgs e)
        {
            bool toupdate = false;
            if (IsSnapToTickEnabled)
            {
                toupdate = ResetValue(Canvas.GetLeft(thumbsecond), Canvas.GetLeft(thumb), rectOrdinates.X);
            }
            if (thumb1focus)
            {
                double oldleft = Canvas.GetLeft(thumb);
                double left = 0d;
                if (!MoveToHandle)
                {
                    left = Canvas.GetLeft(thumb) + CalculateUniformRange(this.LargeChange);
                }
                else
                {
                    left = rectOrdinates.X - thumb.Width / 2;
                }

                if (RangeVisibility)
                {
                    if (!IsSnapToTickEnabled)
                    {
                        if (left < rectOrdinates.X && !MoveToHandle)
                        {
                            Canvas.SetLeft(thumb, left);
                            Range.Start = CalculateRange(left - oldleft) + Range.Start;

                            Canvas.SetLeft(rect, left);
                        }
                        else if (!MoveToHandle)
                        {
                            Canvas.SetLeft(thumb, rectOrdinates.X - thumb.Width / 2);
                            Range.Start = CalculateRange(rectOrdinates.X - oldleft) + Range.Start;

                            Canvas.SetLeft(rect, rectOrdinates.X);


                        }
                        else
                        {
                            Canvas.SetLeft(thumb, left);
                            Range.Start = CalculateRange(left - oldleft) + Range.Start;

                            Canvas.SetLeft(rect, left);

                        }

                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                    }
                    else
                    {
                        left = Canvas.GetLeft(thumb);
                        if (!toupdate)
                        {
                            if (left < rectOrdinates.X)
                            {
                                Range.Start = Range.Start + TickFrequency;
                                if (Range.Start > Maximum)
                                    Range.Start = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Range.Start = Range.Start - TickFrequency;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }

                }
                else
                {
                    left = Canvas.GetLeft(thumb) - CalculateUniformRange(this.LargeChange);

                    if (!IsSnapToTickEnabled)
                    {
                        if (left > rectOrdinates.X && !MoveToHandle)
                        {

                            Canvas.SetLeft(thumb, left);
                            //Canvas.SetLeft(rect, left);
                            Value = Value - CalculateRange(oldleft - left);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                        else if (!MoveToHandle)
                        {
                            Canvas.SetLeft(thumb, rectOrdinates.X - thumb.Width / 2);
                            Value = Value - CalculateRange(oldleft - rectOrdinates.X);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                        else
                        {
                            Canvas.SetLeft(thumb, rectOrdinates.X - thumb.Width / 2);
                            Value = Value - CalculateRange(oldleft - rectOrdinates.X);
                            if (!(Calculate(Value) < 0))
                                RangeValue = Calculate(Value);
                        }
                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                    }
                    else
                    {
                        left = Canvas.GetLeft(thumb);
                        if (!toupdate)
                        {
                            if (left < rectOrdinates.X)
                            {
                                Value = Value + TickFrequency;
                                if (Value > Maximum)
                                    Value = Maximum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }

                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (Value < Minimum)
                                    Value = Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                    }

                }
            }
            else if (thumb2focus)
            {
                double oldleft = Canvas.GetLeft(thumbsecond);
                double left = 0d;
                if (!IsSnapToTickEnabled)
                {
                    if (!MoveToHandle)
                    {
                        left = Canvas.GetLeft(thumbsecond) - CalculateUniformRange(this.LargeChange);
                    }
                    else
                    {
                        left = rectOrdinates.X - thumbsecond.Width / 2;
                    }
                    if (left > rectOrdinates.X && !MoveToHandle)
                    {
                        Canvas.SetLeft(thumbsecond, left);
                        //Range.Start = CalculateRange(left - oldleft) + Range.Start;
                        Range.End = Range.End - CalculateRange(oldleft - left);
                        //Canvas.SetLeft(rect, left);
                    }
                    else if (!MoveToHandle)
                    {
                        Canvas.SetLeft(thumbsecond, rectOrdinates.X - thumbsecond.Width);
                        Range.End = Range.End - CalculateRange(oldleft - rectOrdinates.X);
                        // Range.Start = CalculateRange(rectOrdinates.X - oldleft) + Range.Start;

                        //Canvas.SetLeft(rect, rectOrdinates.X);


                    }
                    else
                    {
                        Canvas.SetLeft(thumbsecond, left);
                        //Range.Start = CalculateRange(left - oldleft) + Range.Start;
                        Range.End = Range.End - CalculateRange(oldleft - left);


                    }
                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                }
                else
                {
                    if (!toupdate)
                    {
                        if (!RangeVisibility)
                        {
                            left = Canvas.GetLeft(thumbsecond);
                            if (rectOrdinates.X > left)
                            {
                                Value = Value + TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                        else
                        {
                            left = Canvas.GetLeft(thumbsecond);
                            if (rectOrdinates.X > left)
                            {
                                Range.End = Range.End + TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Range.End = Range.End - TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }

                //RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

                //Canvas.SetLeft(thumb, left);

                //Range.Start = CalculateRange(left - oldleft) + Range.Start;
            }
        }

        internal Point rectOrdinates;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the thumbsecond control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void thumbsecond_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            thumb2focus = true;
            thumb1focus = false;
            oldvalue = 0;
            newvalue = 0;
            SecondThumbIsPressed = false;
            if (thumb2focus)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(thumbsecond, "Focus", true);

                    VisualStateManager.GoToState(thumb, "MouseEnter", true);
                }
                else
                {
                    VisualStateManager.GoToState(verticalthumbsecond, "Focus", true);

                    VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
                }

            }
        }

        internal double oldvalue = 0d;
        internal double newvalue = 0d;

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void thumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            thumb1focus = true;
            thumb2focus = false;
            oldvalue = 0;
            newvalue = 0;
            FirstThumbIsPressed = false;
            if (thumb1focus)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(thumb, "Focus", true);

                    VisualStateManager.GoToState(thumbsecond, "MouseEnter", true);
                }
                else
                {
                    VisualStateManager.GoToState(verticalthumb, "Focus", true);

                    VisualStateManager.GoToState(verticalthumbsecond, "MouseEnter", true);
                }

            }

        }

        /// <summary>
        /// Handles the MouseLeave event of the thumbsecond control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void thumbsecond_MouseLeave(object sender, MouseEventArgs e)
        {
            if (thumb2focus)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(thumbsecond, "Focus", true);

                    VisualStateManager.GoToState(thumb, "MouseEnter", true);
                }
                else
                {
                    VisualStateManager.GoToState(verticalthumbsecond, "Focus", true);

                    VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
                }
            }
            else
            {
                if (this.Orientation == Orientation.Horizontal)
                {


                    VisualStateManager.GoToState(thumbsecond, "MouseEnter", true);
                }
                else
                {
                    VisualStateManager.GoToState(verticalthumbsecond, "MouseEnter", true);
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void thumb_MouseLeave(object sender, MouseEventArgs e)
        {
            if (thumb1focus)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    VisualStateManager.GoToState(thumb, "Focus", true);

                    VisualStateManager.GoToState(thumbsecond, "MouseEnter", true);
                }
                else
                {
                    VisualStateManager.GoToState(verticalthumb, "Focus", true);

                    VisualStateManager.GoToState(verticalthumbsecond, "MouseEnter", true);
                }

            }
            else
            {
                if (this.Orientation == Orientation.Horizontal)
                {


                    VisualStateManager.GoToState(thumb, "MouseEnter", true);
                }
                else
                {


                    VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
                }
            }

        }

        internal bool thumb1focus = false;
        internal bool thumb2focus = false;
        
        //void ArrangeSlider()
        //{
        //    if (this != null)
        //    {

        //        if (this.Orientation == Orientation.Horizontal)
        //        {

        //            horizontal.Visibility = Visibility.Visible;
        //            vertical.Visibility = Visibility.Collapsed;

        //            Canvas.SetLeft(thumb, Calculate(Range.Start) + thumb.Width / 2);



        //            if (this.TickPlacement == Tickplacement.Both)
        //            {
        //                custompanel.Visibility = Visibility.Visible;
        //                custompaneldown.Visibility = Visibility.Visible;
        //                custompanellabel.Visibility = Visibility.Visible;
        //                custompanellabeldown.Visibility = Visibility.Visible;
        //                if (!LabelVisibility)
        //                {
        //                    //custompanel.Visibility = Visibility.Collapsed;
        //                    //custompaneldown.Visibility = Visibility.Collapsed;
        //                    custompanellabel.Visibility = Visibility.Collapsed;
        //                    custompanellabeldown.Visibility = Visibility.Collapsed;
        //                }

        //            }
        //            else if (this.TickPlacement == Tickplacement.Top )
        //            {
        //                custompanel.Visibility = Visibility.Visible;
        //                custompanellabel.Visibility = Visibility.Visible;

        //                custompaneldown.Visibility = Visibility.Collapsed;
        //                custompanellabeldown.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    //custompanel.Visibility = Visibility.Collapsed;
        //                    //custompaneldown.Visibility = Visibility.Collapsed;
        //                    custompanellabel.Visibility = Visibility.Collapsed;
        //                    custompanellabeldown.Visibility = Visibility.Collapsed;
        //                }
        //            }
        //            else if (this.TickPlacement == Tickplacement.Down )
        //            {
        //                custompaneldown.Visibility = Visibility.Visible;
        //                custompanellabeldown.Visibility = Visibility.Visible;
        //                custompanel.Visibility = Visibility.Collapsed;
        //                custompanellabel.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    //custompanel.Visibility = Visibility.Collapsed;
        //                    //custompaneldown.Visibility = Visibility.Collapsed;
        //                    custompanellabel.Visibility = Visibility.Collapsed;
        //                    custompanellabeldown.Visibility = Visibility.Collapsed;
        //                }

        //            }
        //            else if (this.TickPlacement == Tickplacement.None)
        //            {
        //                custompanel.Visibility = Visibility.Collapsed;
        //                custompaneldown.Visibility = Visibility.Collapsed;
        //                custompanellabel.Visibility = Visibility.Collapsed;
        //                custompanellabeldown.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    //custompanel.Visibility = Visibility.Collapsed;
        //                    //custompaneldown.Visibility = Visibility.Collapsed;
        //                    custompanellabel.Visibility = Visibility.Collapsed;
        //                    custompanellabeldown.Visibility = Visibility.Collapsed;
        //                }
        //            }


        //            if (this.RangeVisibility)
        //            {
        //                this.thumbsecond.Visibility = Visibility.Visible;
        //                this.rect.Visibility = Visibility.Visible;
        //                Canvas.SetLeft(thumbsecond, (Calculate(Range.End) + thumbsecond.Width / 2));
        //                Canvas.SetLeft(rect, Calculate(Range.Start) + thumb.Width );
        //                Canvas.SetLeft(rectButton, Calculate(Range.Start) + thumb.Width );
        //                Canvas.SetTop(rect, Canvas.GetTop(thumb) + thumb.Height / 2 - trackrect.Height / 3);
        //                Canvas.SetTop(rectButton, Canvas.GetTop(thumb) + thumb.Height / 2 - trackrect.Height / 3);                     
        //                Canvas.SetTop(thumbsecond, Canvas.GetTop(thumb));
        //                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

        //            }
        //            else
        //            {
        //                this.thumbsecond.Visibility = Visibility.Collapsed;

        //               // this.rect.Visibility = Visibility.Collapsed;
        //                Canvas.SetLeft(thumb, Calculate(Value)+thumb.Width/2);
        //                Canvas.SetLeft(rect, thumb.Width/2);
        //                Canvas.SetLeft(rectButton, thumb.Width / 2);
        //                if (!(Calculate(Value) < 0))
        //                {
        //                    RangeValue = Calculate(Value);
        //                }
        //                else
        //                {
        //                    RangeValue = 0;
        //                }


        //            }

        //        }
        //        else
        //        {

        //            horizontal.Visibility = Visibility.Collapsed;
        //            vertical.Visibility = Visibility.Visible;
        //            Canvas.SetTop(verticalthumb, Calculate(Range.Start) + verticalthumb.Height / 2);

        //            if (this.TickPlacement == Tickplacement.Both)
        //            {
        //                custompanelvertical.Visibility = Visibility.Visible;
        //                custompanelverticaldown.Visibility = Visibility.Visible;
        //                CustomPanelVerticalLabel.Visibility = Visibility.Visible;
        //                CustomPanelVerticalLabelDown.Visibility = Visibility.Visible;
        //                if (!LabelVisibility)
        //                {
        //                    CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                    CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                }
        //            }
        //            else if ( this.TickPlacement == Tickplacement.Left)
        //            {
        //                custompanelvertical.Visibility = Visibility.Visible;
        //                custompanelverticaldown.Visibility = Visibility.Collapsed;
        //                CustomPanelVerticalLabel.Visibility = Visibility.Visible;
        //                CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                    CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                }

        //            }
        //            else if ( this.TickPlacement == Tickplacement.Right)
        //            {
        //                custompanelverticaldown.Visibility = Visibility.Visible;
        //                CustomPanelVerticalLabelDown.Visibility = Visibility.Visible;
        //                custompanelvertical.Visibility = Visibility.Collapsed;
        //                CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                    CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                }
        //            }
        //            else if (this.TickPlacement == Tickplacement.None)
        //            {
        //                custompanelvertical.Visibility = Visibility.Collapsed;
        //                custompanelverticaldown.Visibility = Visibility.Collapsed;
        //                CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                if (!LabelVisibility)
        //                {
        //                    CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
        //                    CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
        //                }
        //            }
        //            if (this.RangeVisibility)
        //            {
        //                this.verticalthumbsecond.Visibility = Visibility.Visible;
        //                this.verticalrect.Visibility = Visibility.Visible;
        //                Canvas.SetTop(verticalthumbsecond, (Calculate(Range.End) + verticalthumbsecond.Height / 2));
        //                Canvas.SetTop(verticalrect, Calculate(Range.Start) + verticalthumb.Height / 2);
        //                Canvas.SetLeft(verticalrect, Canvas.GetLeft(verticalthumb) + verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
        //                Canvas.SetTop(verticalrectButton, Calculate(Range.Start) + verticalthumb.Height / 2);
        //                Canvas.SetLeft(verticalrectButton, Canvas.GetLeft(verticalthumb) + verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
        //                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))); 
        //            }
        //            else
        //            {
        //                this.verticalthumbsecond.Visibility = Visibility.Collapsed;
        //                //this.verticalrect.Visibility = Visibility.Collapsed;
        //                Canvas.SetTop(verticalthumb, Calculate(Value) + verticalthumb.Height / 2);
        //                Canvas.SetTop(verticalrect, verticalthumb.Height / 2);
        //                Canvas.SetLeft(verticalrect, Canvas.GetLeft(verticalthumb) + verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
        //                Canvas.SetTop(verticalrectButton, verticalthumb.Height / 2);
        //                Canvas.SetLeft(verticalrectButton, Canvas.GetLeft(verticalthumb) + verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
        //                if (!(Calculate(Value) < 0))
        //                {
        //                    RangeValue = Calculate(Value);
        //                }
        //                else
        //                {
        //                    RangeValue = 0;
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Arranges the slider.
        /// </summary>
        void ArrangeSlider()
        {

            if (this != null)
            {
                //PopulateChildren();
                Range.PropertyChanged += new PropertyChangedEventHandler(Range_PropertyChanged);
                if (this.Orientation == Orientation.Horizontal)
                {
                    
                        
                     horizontal.Visibility = Visibility.Visible;
                     vertical.Visibility = Visibility.Collapsed;
                    
                    Canvas.SetLeft(thumb, Calculate(Range.Start) + thumb.Width / 2);


                    if (this.TickPlacement == Tickplacement.Both)
                    {
                        custompanel.Visibility = Visibility.Visible;
                        custompaneldown.Visibility = Visibility.Visible;
                        custompanellabel.Visibility = Visibility.Visible;
                        custompanellabeldown.Visibility = Visibility.Visible;
                        if (!LabelVisibility)
                        {
                            //custompanel.Visibility = Visibility.Collapsed;
                            //custompaneldown.Visibility = Visibility.Collapsed;
                            custompanellabel.Visibility = Visibility.Collapsed;
                            custompanellabeldown.Visibility = Visibility.Collapsed;
                        }

                    }
                    else if (this.TickPlacement == Tickplacement.Top)
                    {
                        custompanel.Visibility = Visibility.Visible;
                        custompanellabel.Visibility = Visibility.Visible;

                        custompaneldown.Visibility = Visibility.Collapsed;
                        custompanellabeldown.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            //custompanel.Visibility = Visibility.Collapsed;
                            //custompaneldown.Visibility = Visibility.Collapsed;
                            custompanellabel.Visibility = Visibility.Collapsed;
                            custompanellabeldown.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (this.TickPlacement == Tickplacement.Down)
                    {
                        custompaneldown.Visibility = Visibility.Visible;
                        custompanellabeldown.Visibility = Visibility.Visible;
                        custompanel.Visibility = Visibility.Collapsed;
                        custompanellabel.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            //custompanel.Visibility = Visibility.Collapsed;
                            //custompaneldown.Visibility = Visibility.Collapsed;
                            custompanellabel.Visibility = Visibility.Collapsed;
                            custompanellabeldown.Visibility = Visibility.Collapsed;
                        }

                    }
                    else if (this.TickPlacement == Tickplacement.None)
                    {
                        custompanel.Visibility = Visibility.Collapsed;
                        custompaneldown.Visibility = Visibility.Collapsed;
                        custompanellabel.Visibility = Visibility.Collapsed;
                        custompanellabeldown.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            //custompanel.Visibility = Visibility.Collapsed;
                            //custompaneldown.Visibility = Visibility.Collapsed;
                            custompanellabel.Visibility = Visibility.Collapsed;
                            custompanellabeldown.Visibility = Visibility.Collapsed;
                        }
                    }


                    if (this.RangeVisibility)
                    {
                        this.thumbsecond.Visibility = Visibility.Visible;
                        this.rect.Visibility = Visibility.Visible;
                        Canvas.SetLeft(thumbsecond, (Calculate(Range.End) + thumbsecond.Width / 2));
                        Canvas.SetLeft(rect, Calculate(Range.Start) + thumb.Width);
                        Canvas.SetLeft(rectButton, Calculate(Range.Start) + thumb.Width);
                        Canvas.SetTop(rect, thumb.Height / 2 - trackrect.Height / 3);
                        Canvas.SetTop(rectButton, thumb.Height / 2 - trackrect.Height / 3);
                        Canvas.SetTop(thumbsecond, Canvas.GetTop(thumb));
                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

                    }
                    else
                    {
                        this.thumbsecond.Visibility = Visibility.Collapsed;
                        if (Value > Maximum)
                            Value = Maximum;
                        // this.rect.Visibility = Visibility.Collapsed;
                        Canvas.SetLeft(thumb, Calculate(Value) + thumb.Width / 2);
                        Canvas.SetLeft(rect, thumb.Width / 2);
                        Canvas.SetLeft(rectButton, thumb.Width / 2);
                        Canvas.SetTop(rect, thumb.Height / 2 - trackrect.Height / 3);
                        Canvas.SetTop(rectButton, thumb.Height / 2 - trackrect.Height / 3);
                        if (!(Calculate(Value) < 0))
                        {
                            RangeValue = Calculate(Value);
                        }
                        else
                        {
                            RangeValue = 0;
                        }


                    }

                }
                else
                {

                    horizontal.Visibility = Visibility.Collapsed;
                    vertical.Visibility = Visibility.Visible;
                    Canvas.SetTop(verticalthumb, Calculate(Range.Start) + verticalthumb.Height / 2);

                    if (this.TickPlacement == Tickplacement.Both)
                    {
                        custompanelvertical.Visibility = Visibility.Visible;
                        custompanelverticaldown.Visibility = Visibility.Visible;
                        CustomPanelVerticalLabel.Visibility = Visibility.Visible;
                        CustomPanelVerticalLabelDown.Visibility = Visibility.Visible;
                        if (!LabelVisibility)
                        {
                            CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                            CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (this.TickPlacement == Tickplacement.Left)
                    {
                        custompanelvertical.Visibility = Visibility.Visible;
                        custompanelverticaldown.Visibility = Visibility.Collapsed;
                        CustomPanelVerticalLabel.Visibility = Visibility.Visible;
                        CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                            CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        }

                    }
                    else if (this.TickPlacement == Tickplacement.Right)
                    {
                        custompanelverticaldown.Visibility = Visibility.Visible;
                        CustomPanelVerticalLabelDown.Visibility = Visibility.Visible;
                        custompanelvertical.Visibility = Visibility.Collapsed;
                        CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                            CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        }
                    }
                    else if (this.TickPlacement == Tickplacement.None)
                    {
                        custompanelvertical.Visibility = Visibility.Collapsed;
                        custompanelverticaldown.Visibility = Visibility.Collapsed;
                        CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                        CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        if (!LabelVisibility)
                        {
                            CustomPanelVerticalLabel.Visibility = Visibility.Collapsed;
                            CustomPanelVerticalLabelDown.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (this.RangeVisibility)
                    {

                        this.verticalthumbsecond.Visibility = Visibility.Visible;
                        this.verticalrect.Visibility = Visibility.Visible;
                        Canvas.SetTop(verticalthumbsecond, (Calculate(Range.End) + verticalthumbsecond.Height / 2));
                        Canvas.SetTop(verticalrect, Calculate(Range.Start) + verticalthumb.Height / 2);
                        Canvas.SetLeft(verticalrect, verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
                        Canvas.SetTop(verticalrectButton, Calculate(Range.Start) + verticalthumb.Height / 2);
                        Canvas.SetLeft(verticalrectButton, verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                    }
                    else
                    {
                        if (Value > Maximum)
                            Value = Maximum;
                        this.verticalthumbsecond.Visibility = Visibility.Collapsed;
                        //this.verticalrect.Visibility = Visibility.Collapsed;
                        Canvas.SetTop(verticalthumb, Calculate(Value) + verticalthumb.Height / 2);
                        Canvas.SetTop(verticalrect, verticalthumb.Height / 2);
                        Canvas.SetLeft(verticalrect, verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
                        Canvas.SetTop(verticalrectButton, verticalthumb.Height / 2);
                        Canvas.SetLeft(verticalrectButton, verticalthumb.Width / 2 - verticaltrackrect.Width / 3 - 1);
                        if (!(Calculate(Value) < 0))
                        {
                            RangeValue = Calculate(Value);
                        }
                        else
                        {
                            RangeValue = 0;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Handles the PropertyChanged event of the Range control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void Range_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Range = new DoubleRange((sender as DoubleRange).Start, (sender as DoubleRange).End);
        }

        /// <summary>
        /// Populates the children.
        /// </summary>
        void PopulateChildren()
        {
            if (LabelVisibility)
            {
                if (SetCustomLabel)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        AddLabel();
                        custompanellabel.minimum = Minimum;
                        custompanellabel.maximum = Maximum;
                        custompanel.minimum = Minimum;
                        custompanel.maximum = Maximum;
                        AddLabelDown();
                        custompanellabeldown.minimum = Minimum;
                        custompanellabeldown.maximum = Maximum;
                        custompaneldown.minimum = Minimum;
                        custompaneldown.maximum = Maximum;
                    }
                    else
                    {
                        //CustomPanelVerticalLabel.Width = this.CustomPanelWidth;
                        // CustomPanelVerticalLabelDown.Width = this.CustomPanelWidth;
                        AddLabelVertical();
                        CustomPanelVerticalLabel.minimum = Minimum;
                        CustomPanelVerticalLabel.maximum = Maximum;
                        custompanelvertical.minimum = Minimum;
                        custompanelvertical.maximum = Maximum;
                        AddLabelVerticalDown();
                        custompanelverticaldown.minimum = Minimum;
                        custompanelverticaldown.maximum = Maximum;
                        CustomPanelVerticalLabelDown.minimum = Minimum;
                        CustomPanelVerticalLabelDown.maximum = Maximum;
                    }
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                    {

                        AddChildren();
                        AddChildrendown();
                    }
                    else
                    {
                        AddChildrenVertical();
                        AddChildrenVerticalDown();
                    }

                }
            }

        }

        /// <summary>
        /// Handles the SizeChanged event of the main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
        void main_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            controlBorder.BorderThickness = this.BorderThickness;
            this.Height = controlBorder.ActualHeight;
            this.Width = (sender as Grid).ActualWidth + this.BorderThickness.Left + this.BorderThickness.Right;
            this.ControlGridHeight = (sender as Grid).ActualHeight - this.BorderThickness.Bottom;
            ControlGridWidth = (sender as Grid).ActualWidth;
            if (HandleButtonVisibility == Visibility.Visible)
            {
                if (ControlGridWidth>=1020)
                    ControlWidth = ControlGridWidth - 56;
                else
                    ControlWidth = ControlGridWidth - 46;
                ControlHeight = ControlGridHeight - 46;

                can1.Margin = new Thickness(0, 0, 0, 0);
                verticalcan1.Margin = new Thickness(0, 0, 0, 0);
                trackrect.Margin = new Thickness(5, 0, 5, 0);
                backButton.Margin = new Thickness(5, 0, 5, 0);
                verticaltrackrect.Margin = new Thickness(0, 5, 0, 5);
                verticalbackButton.Margin = new Thickness(0, 5, 0, 5);
                if (trackinnerrect != null)
                    trackinnerrect.Margin = new Thickness(6, 1, 6, 1);
                if (verticaltrackinnerrect != null)
                    verticaltrackinnerrect.Margin = new Thickness(1, 6, 1, 6);
            }
            else
            {
                ControlWidth = ControlGridWidth - 46;
                ControlHeight = ControlGridHeight - 46;

                can1.Margin = new Thickness(18, 0, 0, 0);
                trackrect.Margin = new Thickness(23, 0, 23, 0);
                backButton.Margin = new Thickness(23, 0, 23, 0);
                verticalcan1.Margin = new Thickness(0, 18, 0, 0);
                verticaltrackrect.Margin = new Thickness(0, 23, 0, 23);
                verticalbackButton.Margin = new Thickness(0, 23, 0, 23);
                if (trackinnerrect != null)
                    trackinnerrect.Margin = new Thickness(24, 1, 24, 1);
                if (verticaltrackinnerrect != null)
                    verticaltrackinnerrect.Margin = new Thickness(1, 6, 1, 6);

            }
            if (LabelVisibility)
            {
                if (SetCustomLabel)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        AddLabel();
                        custompanellabel.minimum = Minimum;
                        custompanellabel.maximum = Maximum;
                        custompanel.minimum = Minimum;
                        custompanel.maximum = Maximum;
                        AddLabelDown();
                        custompanellabeldown.minimum = Minimum;
                        custompanellabeldown.maximum = Maximum;
                        custompaneldown.minimum = Minimum;
                        custompaneldown.maximum = Maximum;
                    }
                    else
                    {
                        AddLabelVertical();
                        CustomPanelVerticalLabel.minimum = Minimum;
                        CustomPanelVerticalLabel.maximum = Maximum;
                        custompanelvertical.minimum = Minimum;
                        custompanelvertical.maximum = Maximum;
                        AddLabelVerticalDown();
                        custompanelverticaldown.minimum = Minimum;
                        custompanelverticaldown.maximum = Maximum;
                        CustomPanelVerticalLabelDown.minimum = Minimum;
                        CustomPanelVerticalLabelDown.maximum = Maximum;
                    }
                }
                else
                {
                    if (Orientation == Orientation.Horizontal)
                    {

                        AddChildren();
                        AddChildrendown();
                    }
                    else
                    {
                        AddChildrenVertical();
                        AddChildrenVerticalDown();
                    }

                }
            }
            thumb1focus = true;
            thumb2focus = false;
            if (Range.Start == Range.End && Range.Start == Minimum)
            {
                VisualStateManager.GoToState(thumbsecond, "Focus", true);
                thumb2focus = true;
                thumb1focus = false;
            }
            else
            {
                VisualStateManager.GoToState(thumb, "Focus", true);
                thumb1focus = true;
                thumb2focus = false;
            }

            //Binding bind = new Binding();
            //bind.Source = rect;

            //rectButton.SetBinding(Canvas.LeftProperty, bind);
            ArrangeSlider();
            controlBorder.Height = controlBorder.ActualHeight;
            controlBorder.Width = controlBorder.ActualWidth;


            controlBorder.BorderBrush = this.BorderBrush;


        }


        /// <summary>
        /// Adds the label.
        /// </summary>
        void AddLabel()
        {
            custompanel.Children.Clear();
            custompanellabel.Children.Clear();
            double count = Minimum;
            //double Childcount = ((this.Maximum - this.Minimum)) / this.TickFrequency;
            if (ItemsCollection != null)
            {
                double Childcount = this.ItemsCollection.Count;
                custompanel.CustomItems = this.ItemsCollection;
                custompanellabel.CustomItems = this.ItemsCollection;

                foreach (Items item in this.ItemsCollection)
                {
                    Path path = new Path();

                    PathGeometry pathgeo = new PathGeometry();

                    PathFigure pathfig = new PathFigure();


                    pathfig.StartPoint = new Point(280, 315);
                    LineSegment lineseg = new LineSegment();

                    lineseg.Point = new Point(280, 330);

                    PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                    LineSegmentCollection.Add(lineseg);

                    pathfig.Segments = LineSegmentCollection;
                    pathgeo.Figures.Add(pathfig);
                    path.Stretch = Stretch.Fill;
                    path.Fill = new SolidColorBrush(Colors.Black);
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    path.Data = pathgeo;
                    path.Height = 7;
                    path.Width = 2;
                    custompanel.PanelWidth = this.ControlWidth;
                    custompanel.XOffset = ((1 / (this.Maximum - this.Minimum)) * this.ControlWidth);
                    custompanellabel.IsItem = custompanel.IsItem = true;

                    custompanel.Children.Add(path);
                    TextBlock text = new TextBlock();
                    text.Text = item.label;

                    if (this.Orientation == Orientation.Horizontal && this.LabelAlignment == CustomLabelAlignment.Vertical)
                    {
                        text.Margin = new Thickness(0, 0, -text.ActualWidth, 0);
                        text.TextWrapping = TextWrapping.NoWrap;
                        TransformGroup customLabelTransform = new TransformGroup();                       
                        RotateTransform rotateTransform = new RotateTransform();
                        rotateTransform.Angle = 270;
                        customLabelTransform.Children.Add(rotateTransform);
                        text.RenderTransform = customLabelTransform;
                    }

                    custompanellabel.XOffset = custompanel.XOffset;
                    custompanellabel.UpOrientation = true;
                   
                    custompanellabel.PanelWidth = this.ControlWidth;
                    custompanellabel.Children.Add(text);
                    text = null;
                    path = null;
                    pathfig = null;
                    pathgeo = null;
                    lineseg = null;
                    LineSegmentCollection = null;
                }
            }
        }
        /// <summary>
        /// Adds the label vertical.
        /// </summary>
        void AddLabelVertical()
        {
            custompanelvertical.Children.Clear();
            CustomPanelVerticalLabel.Children.Clear();
            if (ItemsCollection != null)
            {
                double Childcount = this.ItemsCollection.Count;
                custompanelvertical.CustomItems = this.ItemsCollection;
                CustomPanelVerticalLabel.CustomItems = this.ItemsCollection;

                foreach (Items item in this.ItemsCollection)
                {
                    Path path = new Path();
                    PathGeometry pathgeo = new PathGeometry();

                    PathFigure pathfig = new PathFigure();


                    pathfig.StartPoint = new Point(174, 378);
                    LineSegment lineseg = new LineSegment();

                    lineseg.Point = new Point(189, 378);

                    PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                    LineSegmentCollection.Add(lineseg);

                    pathfig.Segments = LineSegmentCollection;
                    pathgeo.Figures.Add(pathfig);

                    path.Data = pathgeo;


                    path.Height = 2;
                    path.Width = 7;
                    custompanelvertical.PanelHeight = this.ControlHeight;
                    custompanelvertical.PanelWidth = this.ControlWidth;
                    custompanelvertical.YOffsetDown = ((1 / (this.Maximum - this.Minimum)) * this.ControlHeight);
                    // pathdown.Width = path.Width = Math.Round(path.Width, 1);
                    path.Stretch = Stretch.Fill;
                    path.Fill = new SolidColorBrush(Colors.Black);
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    CustomPanelVerticalLabel.UpOrientation = true;
                    CustomPanelVerticalLabel.YOffsetDown = custompaneldown.YOffsetDown;
                   
                    CustomPanelVerticalLabel.IsItem = custompanelvertical.IsItem = true;
                    custompanelvertical.Children.Add(path);
                    TextBlock text = new TextBlock();
                    text.Text = item.label;
                    CustomPanelVerticalLabel.YOffsetDown = custompanelvertical.YOffsetDown;
                    CustomPanelVerticalLabel.PanelHeight = this.ControlHeight;
                    CustomPanelVerticalLabel.PanelWidth = this.ControlWidth;
                    CustomPanelVerticalLabel.Children.Add(text);
                    text = null;
                    path = null;
                    pathfig = null;
                    pathgeo = null;
                    lineseg = null;
                    LineSegmentCollection = null;
                }
            }

        }

        /// <summary>
        /// Adds the label vertical down.
        /// </summary>
        void AddLabelVerticalDown()
        {
            custompanelverticaldown.Children.Clear();
            CustomPanelVerticalLabelDown.Children.Clear();
            if (ItemsCollection != null)
            {
                double Childcount = this.ItemsCollection.Count;
                custompanelverticaldown.CustomItems = this.ItemsCollection;
                CustomPanelVerticalLabelDown.CustomItems = this.ItemsCollection;

                foreach (Items item in this.ItemsCollection)
                {
                    Path path = new Path();
                    PathGeometry pathgeo = new PathGeometry();

                    PathFigure pathfig = new PathFigure();


                    pathfig.StartPoint = new Point(174, 378);
                    LineSegment lineseg = new LineSegment();

                    lineseg.Point = new Point(189, 378);

                    PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                    LineSegmentCollection.Add(lineseg);

                    pathfig.Segments = LineSegmentCollection;
                    pathgeo.Figures.Add(pathfig);

                    path.Data = pathgeo;


                    path.Height = 2;
                    path.Width = 7;
                    custompanelverticaldown.PanelHeight = this.ControlHeight;
                    custompanelverticaldown.PanelWidth = this.ControlWidth;
                    custompanelverticaldown.YOffsetDown = ((1 / (this.Maximum - this.Minimum)) * this.ControlHeight);
                    // pathdown.Width = path.Width = Math.Round(path.Width, 1);
                    path.Stretch = Stretch.Fill;
                    path.Fill = new SolidColorBrush(Colors.Black);
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    CustomPanelVerticalLabelDown.YOffsetDown = custompanelverticaldown.YOffsetDown;
                   
                    CustomPanelVerticalLabelDown.UpOrientation = false;
                    CustomPanelVerticalLabelDown.IsItem = custompanelverticaldown.IsItem = true;
                    custompanelverticaldown.Children.Add(path);
                    TextBlock text = new TextBlock();
                    text.Text = item.label;
                    CustomPanelVerticalLabelDown.YOffsetDown = custompanelverticaldown.YOffsetDown;
                    CustomPanelVerticalLabelDown.PanelHeight = this.ControlHeight;
                    CustomPanelVerticalLabelDown.PanelWidth = this.ControlWidth;
                    CustomPanelVerticalLabelDown.Children.Add(text);
                    text = null;
                    path = null;
                    pathfig = null;
                    pathgeo = null;
                    lineseg = null;
                    LineSegmentCollection = null;
                }
            }
        }
        /// <summary>
        /// Adds the label down.
        /// </summary>
        void AddLabelDown()
        {

            custompaneldown.Children.Clear();
            //custompaneldown.offset = 0;
            custompanellabeldown.Children.Clear();
            double count = Minimum;
            //double Childcount = ((this.Maximum - this.Minimum)) / this.TickFrequency;
            if (ItemsCollection != null)
            {
                double Childcount = this.ItemsCollection.Count;
                custompaneldown.CustomItems = this.ItemsCollection;
                
                custompanellabeldown.CustomItems = this.ItemsCollection;

                foreach (Items item in this.ItemsCollection)
                {
                    Path path = new Path();

                    PathGeometry pathgeo = new PathGeometry();

                    PathFigure pathfig = new PathFigure();


                    pathfig.StartPoint = new Point(280, 315);
                    LineSegment lineseg = new LineSegment();

                    lineseg.Point = new Point(280, 330);

                    PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                    LineSegmentCollection.Add(lineseg);

                    pathfig.Segments = LineSegmentCollection;
                    pathgeo.Figures.Add(pathfig);

                    path.Data = pathgeo;


                    path.Height = 7;



                    path.Width = 2;
                    custompaneldown.PanelWidth = this.ControlWidth;

                    custompaneldown.XOffset = ((1 / (this.Maximum - this.Minimum)) * this.ControlWidth);

                    custompanellabeldown.IsItem = custompaneldown.IsItem = true;
                    path.Stretch = Stretch.Fill;
                    path.Fill = new SolidColorBrush(Colors.Black);
                    path.Stroke = new SolidColorBrush(Colors.Black);
                    custompaneldown.Children.Add(path);
                    TextBlock text = new TextBlock();
                    text.Text = item.label;

                    if (this.Orientation == Orientation.Horizontal && this.LabelAlignment == CustomLabelAlignment.Vertical)
                    {
                        double textWidth = text.ActualWidth + 10;
                        text.Margin = new Thickness(-textWidth, 0, 0, 0);
                        text.TextWrapping = TextWrapping.NoWrap;
                        TransformGroup customLabelTransform = new TransformGroup();
                        TranslateTransform translateTransform = new TranslateTransform();
                        RotateTransform rotateTransform = new RotateTransform();
                        translateTransform.Y = textWidth;
                        translateTransform.X = -textWidth;
                        rotateTransform.Angle = 270;
                        customLabelTransform.Children.Add(translateTransform);
                        customLabelTransform.Children.Add(rotateTransform);
                        text.RenderTransform = customLabelTransform;
                    }

                    custompanellabeldown.XOffset = custompaneldown.XOffset;
                    custompanellabeldown.UpOrientation = false;
                    custompanellabeldown.PanelWidth = this.ControlWidth;
                    custompanellabeldown.Children.Add(text);
                    text = null;
                    path = null;
                    pathfig = null;
                    pathgeo = null;
                    lineseg = null;
                    LineSegmentCollection = null;
                }
            }

        }





        /// <summary>
        /// Adds the children vertical down.
        /// </summary>
        void AddChildrenVerticalDown()
        {
            custompanelverticaldown.Children.Clear();
            CustomPanelVerticalLabelDown.Children.Clear();
            double Childcount = (this.Maximum - this.Minimum) / ((this.TickFrequency==0) ? 1:this.TickFrequency);
            double count = Minimum;
            for (int i = 0; i <= Childcount + 1; i++)
            {

                Path path = new Path();

                PathGeometry pathgeo = new PathGeometry();

                PathFigure pathfig = new PathFigure();


                pathfig.StartPoint = new Point(174, 378);
                LineSegment lineseg = new LineSegment();

                lineseg.Point = new Point(189, 378);

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();
                PathSegmentCollection LineSegmentCollectiondown = new PathSegmentCollection();
                LineSegmentCollection.Add(lineseg);

                pathfig.Segments = LineSegmentCollection;
                pathgeo.Figures.Add(pathfig);

                path.Data = pathgeo;


                path.Height = 2;

                double c = Math.Round(((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlWidth), 1);
                path.Width = 7;
                custompanelverticaldown.PanelHeight = this.ControlHeight;
                custompanelverticaldown.YOffsetDown = ((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlHeight);

                path.Stretch = Stretch.Fill;
                path.Fill = new SolidColorBrush(Colors.Black);
                path.Stroke = new SolidColorBrush(Colors.Black);
                custompanelverticaldown.Children.Add(path);
                if (LabelVisibility)
                {
                    TextBlock text = new TextBlock();
                    if (!SetCustomLabel)
                    {

                        if (i + 1 > (Childcount + 1))
                        {
                            text.Text = Maximum.ToString();
                        }
                        else
                        {
                            text.Text = count.ToString();


                        }
                        CustomPanelVerticalLabelDown.IsItem = false;
                    }
                    else
                    {


                        CustomPanelVerticalLabelDown.IsItem = true;
                        CustomPanelVerticalLabelDown.LabelOrientation = LabelOrientation;
                    }
                    count += this.TickFrequency;
                    CustomPanelVerticalLabelDown.YOffsetDown = custompanelverticaldown.YOffsetDown;
                    CustomPanelVerticalLabelDown.PanelHeight = this.ControlHeight;


                    CustomPanelVerticalLabelDown.Children.Add(text);
                    text = null;
                }
                path = null;
                pathfig = null;
                pathgeo = null;
                lineseg = null;
                LineSegmentCollection = null;
            }
        }
        /// <summary>
        /// Adds the children vertical.
        /// </summary>
        void AddChildrenVertical()
        {
            custompanelvertical.Children.Clear();
            CustomPanelVerticalLabel.Children.Clear();
            double count = Minimum;
            double Childcount = (this.Maximum - this.Minimum) / ((this.TickFrequency==0) ? 1:this.TickFrequency);
            for (int i = 0; i <= Childcount + 1; i++)
            {

                Path path = new Path();

                PathGeometry pathgeo = new PathGeometry();

                PathFigure pathfig = new PathFigure();


                pathfig.StartPoint = new Point(174, 378);
                LineSegment lineseg = new LineSegment();

                lineseg.Point = new Point(189, 378);

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                LineSegmentCollection.Add(lineseg);

                pathfig.Segments = LineSegmentCollection;
                pathgeo.Figures.Add(pathfig);

                path.Data = pathgeo;


                path.Height = 2;

                double c = Math.Round(((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlWidth), 1);
                path.Width = 7;
                custompanelvertical.PanelHeight = this.ControlHeight;
                custompanelvertical.YOffsetDown = ((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlHeight);

                path.Stretch = Stretch.Fill;
                path.Fill = new SolidColorBrush(Colors.Black);
                path.Stroke = new SolidColorBrush(Colors.Black);
                custompanelvertical.Children.Add(path);
                if (LabelVisibility)
                {
                    TextBlock text = new TextBlock();
                    if (!SetCustomLabel)
                    {

                        if (i + 1 > (Childcount + 1))
                        {

                            if (Maximum != count)
                            {
                                text.Text = Maximum.ToString();
                            }

                        }

                        else
                        {
                            text.Text = count.ToString();


                        }
                        CustomPanelVerticalLabel.IsItem = false;
                    }
                    else
                    {


                        CustomPanelVerticalLabel.IsItem = true;
                        CustomPanelVerticalLabel.LabelOrientation = LabelOrientation;
                    }
                    count += this.TickFrequency;
                    CustomPanelVerticalLabel.YOffsetDown = custompanelvertical.YOffsetDown;
                    CustomPanelVerticalLabel.PanelHeight = this.ControlHeight;
                    CustomPanelVerticalLabel.UpOrientation = true;
                    CustomPanelVerticalLabel.Children.Add(text);
                    text = null;
                }
                path = null;
                pathfig = null;
                pathgeo = null;
                lineseg = null;
                LineSegmentCollection = null;

            }
        }
        /// <summary>
        /// Adds the children.
        /// </summary>
        void AddChildren()
        {
            custompanel.Children.Clear();
            custompanellabel.Children.Clear();
            double count = Minimum;
            double Childcount = ((this.Maximum - this.Minimum)) / ((this.TickFrequency==0) ? 1:this.TickFrequency);


            for (int i = 0; i <= Childcount + 1; i++)
            {

                Path path = new Path();

                PathGeometry pathgeo = new PathGeometry();

                PathFigure pathfig = new PathFigure();


                pathfig.StartPoint = new Point(280, 315);
                LineSegment lineseg = new LineSegment();

                lineseg.Point = new Point(280, 330);

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                LineSegmentCollection.Add(lineseg);

                pathfig.Segments = LineSegmentCollection;
                pathgeo.Figures.Add(pathfig);

                path.Data = pathgeo;


                path.Height = 7;



                path.Width = 2;
                custompanel.PanelWidth = this.ControlWidth;
                custompanel.XOffset = ((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlWidth);

                // pathdown.Width = path.Width = Math.Round(path.Width, 1);
                path.Stretch = Stretch.Fill;
                path.Fill = new SolidColorBrush(Colors.Black);
                path.Stroke = new SolidColorBrush(Colors.Black);
                custompanel.Children.Add(path);
                if (LabelVisibility)
                {
                    TextBlock text = new TextBlock();
                    if (!SetCustomLabel)
                    {

                        if (i + 1 > (Childcount + 1))
                        {
                            text.Text = Maximum.ToString();
                        }
                        else
                        {
                            text.Text = count.ToString();


                        }
                        custompanellabel.IsItem = false;
                    }
                    else
                    {


                        custompanellabel.IsItem = true;
                        custompanellabel.LabelOrientation = LabelOrientation;
                    }
                    count += this.TickFrequency;
                    custompanellabel.XOffset = custompanel.XOffset;
                    custompanellabel.UpOrientation = true;
                    custompanellabel.PanelWidth = this.ControlWidth;
                    custompanellabel.Children.Add(text);
                    text = null;
                }
                path = null;
                pathfig = null;
                pathgeo = null;
                lineseg = null;
                LineSegmentCollection = null;


            }

        }
        /// <summary>
        /// Adds the childrendown.
        /// </summary>
        void AddChildrendown()
        {
            custompaneldown.Children.Clear();
            custompanellabeldown.Children.Clear();
            double count = Minimum;
            double Childcount = (this.Maximum - this.Minimum) / ((this.TickFrequency==0) ? 1:this.TickFrequency);
            for (int i = 0; i <= Childcount + 1; i++)
            {

                Path path = new Path();

                PathGeometry pathgeo = new PathGeometry();

                PathFigure pathfig = new PathFigure();


                pathfig.StartPoint = new Point(280, 315);
                LineSegment lineseg = new LineSegment();

                lineseg.Point = new Point(280, 330);

                PathSegmentCollection LineSegmentCollection = new PathSegmentCollection();

                LineSegmentCollection.Add(lineseg);

                pathfig.Segments = LineSegmentCollection;
                pathgeo.Figures.Add(pathfig);

                path.Data = pathgeo;

                path.Height = 7;
                //path.Width = Math.Round((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlWidth);
                path.Width = 2;
                custompaneldown.PanelWidth = this.ControlWidth;

                custompaneldown.XOffset = ((this.TickFrequency / (this.Maximum - this.Minimum)) * this.ControlWidth);
                path.Stretch = Stretch.Fill;
                path.Fill = new SolidColorBrush(Colors.Black);
                path.Stroke = new SolidColorBrush(Colors.Black);
                custompanellabeldown.XOffset = custompanel.XOffset;
                custompanellabeldown.PanelWidth = this.ControlWidth;
                custompaneldown.Children.Add(path);
                if (LabelVisibility)
                {
                    TextBlock text = new TextBlock();
                    if (!SetCustomLabel)
                    {

                        if (i + 1 > (Childcount + 1))
                        {
                            text.Text = Maximum.ToString();
                        }
                        else
                        {
                            text.Text = count.ToString();


                        }
                        custompanellabeldown.IsItem = false;
                    }
                    else
                    {

                        custompanellabeldown.IsItem = true;
                        custompanellabeldown.LabelOrientation = LabelOrientation;
                    }
                    count += this.TickFrequency;


                    custompanellabeldown.Children.Add(text);
                    text = null;
                }
                path = null;
                pathfig = null;
                pathgeo = null;
                lineseg = null;
                LineSegmentCollection = null;


            }
        }

        /// <summary>
        /// Handles the Click event of the decButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void decButton_Click(object sender, RoutedEventArgs e)
        {
            double oldleft, secondthumbleft, left, width1, top;
            if (IsSnapToTickEnabled)
            {
                ResetValueOnDec();
            }
            if (this.Orientation == Orientation.Horizontal)
            {
                oldleft = Canvas.GetLeft(thumb) - thumb.Width / 2;
                secondthumbleft = Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2;
                left = Canvas.GetLeft(thumb) - thumb.Width / 2 - CalculateUniformRange(this.UniformChange);
                width1 = trackrect.ActualWidth;
                top = Canvas.GetTop(thumb);
                if (this.RangeVisibility)
                {
                    if (!thumb2focus)
                    {
                        if (!IsSnapToTickEnabled)
                        {
                            if (left < secondthumbleft && Range.Start > Minimum && left > thumb.Width / 6)
                            {
                                Canvas.SetLeft(thumb, left + thumb.Width / 2);
                                Range.Start = CalculateRange(left - oldleft) + Range.Start;
                                Canvas.SetLeft(rect, left + thumb.Width / 2);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (left < 0)
                            {
                                left = 0;

                                Range.Start = Minimum;

                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left < secondthumbleft && Range.Start > Minimum && left > thumb.Width / 6)
                            {
                                Range.Start = Range.Start - TickFrequency;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        Canvas.SetTop(thumb, top);
                    }
                    else
                    {
                        left = (Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2) - CalculateUniformRange(this.UniformChange);
                        if (!IsSnapToTickEnabled)
                        {
                            if (left > oldleft)
                            {
                                Canvas.SetLeft(thumbsecond, left + thumb.Width / 2);
                                Range.End = Range.End - CalculateRange(secondthumbleft - left);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left > oldleft)
                            {
                                Range.End = Range.End - TickFrequency;
                                if (Range.End < Range.Start)
                                    Range.End = Range.Start;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                            }
                        }

                    }
                }
                else
                {
                    double leftthumb = Canvas.GetLeft(thumb) - thumb.Width / 2 - CalculateUniformRange(this.UniformChange);
                    oldleft = Canvas.GetLeft(thumb) - thumb.Width / 2;
                    double topthumb = Canvas.GetTop(thumb);
                    if (!IsSnapToTickEnabled)
                    {
                        if (Range.End <= Maximum && leftthumb > thumb.Width / 6 && Value > Minimum)
                        {
                            Canvas.SetLeft(thumb, leftthumb + thumb.Width / 2);
                            Value = CalculateRange(left - oldleft) + Value;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                        else if (leftthumb < 0)
                        {
                            // left = 0;
                            Canvas.SetLeft(thumb, thumb.Width / 6);
                            Value = Minimum;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                        else if (leftthumb > width1)
                        {
                            //Canvas.SetLeft(thumbsecond, 0);
                            //Canvas.SetLeft(thumb, width1 + thumb.Width / 2);
                            //Value = CalculateRange(width1);
                        }
                    }
                    else
                    {
                        Value = Value - TickFrequency;
                        if (Value < Minimum)
                            Value = Minimum;
                        if (!(Calculate(Value) < 0))
                        {
                            RangeValue = Calculate(Value);
                        }
                        else
                        {
                            RangeValue = 0;
                        }

                    }
                    Canvas.SetTop(thumb, topthumb);

                }
            }
            else
            {
                oldleft = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2;
                secondthumbleft = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2;
                left = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2 - CalculateUniformRange(this.UniformChange);
                width1 = verticaltrackrect.ActualHeight;
                top = Canvas.GetLeft(verticalthumb);
                if (this.RangeVisibility)
                {
                    if (!thumb2focus)
                    {
                        if (!IsSnapToTickEnabled)
                        {
                            if (left < secondthumbleft && Range.Start > Minimum && left >= verticalthumb.Height / 6)
                            {
                                Canvas.SetTop(verticalthumb, left + verticalthumb.Height / 2);
                                Range.Start = CalculateRange(left - oldleft) + Range.Start;
                                Canvas.SetTop(verticalrect, left + verticalthumb.Height / 2);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (left < 0)
                            {

                                Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left < secondthumbleft && Range.Start > Minimum && left >= verticalthumb.Height / 6)
                            {
                                Range.Start = Range.Start - TickFrequency;
                                if (Range.Start < Minimum)
                                    Range.Start = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        Canvas.SetLeft(verticalthumb, top);
                    }
                    else
                    {
                        left = (Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2) - CalculateUniformRange(this.UniformChange);
                        if (!IsSnapToTickEnabled)
                        {
                            if (left > oldleft)
                            {
                                Canvas.SetTop(verticalthumbsecond, left + verticalthumb.Height / 2);
                                Range.End = Range.End - CalculateRange(secondthumbleft - left);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left > oldleft)
                            {
                                Range.End = Range.End - TickFrequency;
                                if (Range.End < Range.Start)
                                    Range.End = Range.Start;
                                if (Range.End < Minimum)
                                    Range.End = Minimum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }

                    }
                }
                else
                {
                    double leftthumb = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2 - CalculateUniformRange(this.UniformChange);
                    oldleft = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2;
                    double topthumb = Canvas.GetLeft(verticalthumb);

                    if (!IsSnapToTickEnabled)
                    {
                        if (Value > Minimum && leftthumb > verticalthumb.Height / 6)
                        {
                            Canvas.SetTop(verticalthumb, leftthumb + verticalthumb.Height / 2);
                            Value = CalculateRange(left - oldleft) + Value;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                        else if (leftthumb < 0)
                        {
                            left = 0;
                            Canvas.SetTop(verticalthumb, verticalthumb.Height / 6);
                            Value = Minimum;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                    }
                    else
                    {
                        if (Value > Minimum && leftthumb > verticalthumb.Height / 6)
                        {
                            Value = Value - TickFrequency;
                            if (Value < Minimum)
                                Value = Minimum;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                    }

                    Canvas.SetLeft(verticalthumb, topthumb);

                }
            }

        }



        /// <summary>
        /// Handles the Click event of the backButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void backButton_Click(object sender, RoutedEventArgs e)
        {
            double oldleft = Canvas.GetLeft(thumbsecond);
            double firstleft = Canvas.GetLeft(thumb);
            bool toupdate = false;
            if (IsSnapToTickEnabled)
            {
                toupdate = ResetValue(oldleft, firstleft, ordinates.X);
            }
            if (ordinates != null && ordinates.X != 0)
            {
                if (this.RangeVisibility)
                {
                    if (!IsSnapToTickEnabled)
                    {
                        if (ordinates.X > oldleft)
                        {
                            double firstthumbleft = Canvas.GetLeft(thumb) + thumb.Width;
                            double left = 0d;
                            double width1 = trackrect.ActualWidth;
                            if (!this.MoveToHandle)
                            {
                                left = Canvas.GetLeft(thumbsecond) + CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                left = ordinates.X;
                            }
                            double top = Canvas.GetTop(thumbsecond);

                            if (left + thumbsecond.Width / 2 < width1 && left < ordinates.X)
                            {
                                Canvas.SetLeft(thumbsecond, left);
                                Range.End = Range.End - CalculateRange(oldleft - left);

                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Canvas.SetLeft(thumbsecond, ordinates.X - thumbsecond.Width / 2);
                                Range.End = Range.End - CalculateRange(oldleft - ordinates.X);

                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

                            }
                            Canvas.SetTop(thumbsecond, top);
                            thumb2focus = true;
                            thumb1focus = false;
                            VisualStateManager.GoToState(thumbsecond, "Focus", true);

                            VisualStateManager.GoToState(thumb, "MouseEnter", true);



                        }
                        else if (ordinates.X < firstleft)
                        {
                            double secondthumbleft = Canvas.GetLeft(thumbsecond) - thumbsecond.Width;
                            double left = 0d;
                            if (!MoveToHandle)
                            {
                                left = Canvas.GetLeft(thumb) - CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                left = ordinates.X;
                            }

                            double width1 = trackrect.ActualWidth;
                            double top = Canvas.GetTop(thumb);
                            if (left < width1 && left > 0 && left >= ordinates.X)
                            {
                                Canvas.SetLeft(thumb, left);
                                Range.Start = CalculateRange(left - firstleft) + Range.Start;

                                Canvas.SetLeft(rect, left);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Canvas.SetLeft(thumb, ordinates.X + thumb.Width / 2);
                                Range.Start = CalculateRange(ordinates.X - firstleft) + Range.Start;

                                Canvas.SetLeft(rect, ordinates.X + thumb.Width / 2);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

                            }
                            Canvas.SetTop(thumb, top);

                            thumb1focus = true;
                            thumb2focus = false;

                            VisualStateManager.GoToState(thumb, "Focus", true);
                            VisualStateManager.GoToState(thumbsecond, "MouseEnter", true);

                        }
                    }
                    else
                    {
                        if (!toupdate)
                        {
                            if (ordinates.X > oldleft)
                            {
                                Range.End = Range.End + TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (ordinates.X < firstleft)
                            {
                                Range.Start = Range.Start - TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }
                else
                {
                    if (!IsSnapToTickEnabled)
                    {
                        if (ordinates.X < firstleft)
                        {
                            double left = 0d;
                            if (!MoveToHandle)
                            {
                                left = Canvas.GetLeft(thumb) - CalculateUniformRange(this.LargeChange);

                            }
                            else
                            {
                                left = ordinates.X;

                            }
                            double width1 = trackrect.ActualWidth;
                            double top = Canvas.GetTop(thumb);
                            if (left < width1 && left > 0 && left >= ordinates.X)
                            {

                                Value = CalculateRange(left) + Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                                Canvas.SetLeft(thumb, left);



                            }
                            else
                            {

                                Value = CalculateRange(ordinates.X) + Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                                Canvas.SetLeft(thumb, ordinates.X - thumb.Width / 2);

                            }

                        }
                        else
                        {
                            double left = 0d;
                            double width1 = trackrect.ActualWidth;
                            if (!this.MoveToHandle)
                            {
                                left = Canvas.GetLeft(thumb) + CalculateUniformRange(this.LargeChange);

                            }
                            else
                            {
                                left = ordinates.X;

                            }

                            double top = Canvas.GetTop(thumb);
                            if (left < width1 && left > 0 && left <= ordinates.X)
                            {

                                Value = Minimum + CalculateRange(left);
                                if (!((Calculate(Value) < 0)))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                                Canvas.SetLeft(thumb, left);
                            }
                            else
                            {

                                Value = Minimum + CalculateRange(ordinates.X);
                                if (Value > Maximum)
                                    Value = Maximum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                                Canvas.SetLeft(thumb, ordinates.X);
                            }

                        }
                    }
                    else
                    {
                        if (!toupdate)
                        {
                            if (ordinates.X > firstleft)
                            {
                                Value = Value + TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Handles the Click event of the verticalbackButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void verticalbackButton_Click(object sender, RoutedEventArgs e)
        {
            double oldtop = Canvas.GetTop(verticalthumbsecond);
            double firsttop = Canvas.GetTop(verticalthumb);
            bool toupdate = false;
            if (IsSnapToTickEnabled)
            {
                toupdate = ResetValue(oldtop, firsttop, ordinates.Y);
            }
            if (ordinates != null && ordinates.Y != 0)
            {
                if (this.RangeVisibility)
                {
                    if (!IsSnapToTickEnabled)
                    {
                        if (ordinates.Y > oldtop)
                        {
                            double firstthumbtop = Canvas.GetTop(verticalthumb) + verticalthumb.Height;
                            double height1 = verticaltrackrect.ActualHeight;

                            double top = 0d;
                            if (!MoveToHandle)
                            {
                                top = Canvas.GetTop(verticalthumbsecond) + CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                top = ordinates.Y;
                            }

                            double left = Canvas.GetLeft(verticalthumbsecond);

                            if (top + verticalthumbsecond.Height / 2 < height1 && top < ordinates.Y)
                            {
                                Canvas.SetTop(verticalthumbsecond, top);
                                Range.End = Range.End - CalculateRange(oldtop - top);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else
                            {
                                Canvas.SetTop(verticalthumbsecond, ordinates.Y + verticalthumbsecond.Height / 2);
                                Range.End = Range.End - CalculateRange(oldtop - ordinates.Y);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));

                            }

                            Canvas.SetLeft(verticalthumbsecond, left);
                            thumb2focus = true;
                            thumb1focus = false;
                            VisualStateManager.GoToState(verticalthumbsecond, "Focus", true);
                            VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
                        }
                        else if (ordinates.Y < firsttop)
                        {

                            double secondthumbtop = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height;
                            double top = 0d;
                            if (!MoveToHandle)
                            {
                                top = Canvas.GetTop(verticalthumb) - CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                top = ordinates.Y;
                            }
                            double width1 = verticaltrackrect.ActualHeight;
                            double left = Canvas.GetLeft(verticalthumb);
                            if (top < width1 && top > 0 && top >= ordinates.Y)
                            {
                                Canvas.SetTop(verticalthumb, top);
                                Range.Start = CalculateRange(top - firsttop) + Range.Start;


                                Canvas.SetTop(verticalrect, top);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));


                            }
                            else
                            {
                                Canvas.SetTop(verticalthumb, ordinates.Y + verticalthumb.Height / 2);
                                Range.Start = CalculateRange(ordinates.Y - firsttop) + Range.Start;

                                // MessageBox.Show(Range.Start.ToString());
                                Canvas.SetTop(verticalrect, ordinates.Y + verticalthumb.Height / 2);
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            Canvas.SetLeft(verticalthumbsecond, left);
                            thumb1focus = true;
                            thumb2focus = false;
                            VisualStateManager.GoToState(verticalthumb, "Focus", true);
                            VisualStateManager.GoToState(verticalthumbsecond, "MouseEnter", true);
                        }
                    }
                    else
                    {
                        if (!toupdate)
                        {
                            if (ordinates.Y > oldtop)
                            {
                                Range.End = Range.End + TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (ordinates.X < firsttop)
                            {
                                Range.Start = Range.Start - TickFrequency;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }
                else
                {
                    double left = Canvas.GetLeft(verticalthumb);
                    if (!IsSnapToTickEnabled)
                    {
                        if (ordinates.Y < firsttop)
                        {
                            double top = 0d;
                            if (!MoveToHandle)
                            {
                                top = Canvas.GetTop(verticalthumb) - CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                top = ordinates.Y;
                            }
                            double heigh1 = verticaltrackrect.ActualHeight;

                            if (top < heigh1 && top > 0 && top >= ordinates.Y)
                            {
                                Canvas.SetTop(verticalthumb, top);
                                Value = Minimum + CalculateRange(top);
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }

                            }
                            else
                            {
                                Canvas.SetTop(verticalthumb, top - verticalthumb.Height / 2);
                                Value = CalculateRange(ordinates.Y) + Minimum;
                                //if (Value < Minimum)
                                //    Value = Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }

                            }
                        }
                        else
                        {
                            double top = 0d;
                            double heigh1 = verticaltrackrect.ActualHeight;
                            if (!this.MoveToHandle)
                            {
                                top = Canvas.GetTop(verticalthumb) + CalculateUniformRange(this.LargeChange);
                            }
                            else
                            {
                                top = ordinates.Y;

                            }


                            if (top < heigh1 && top > 0 && top <= ordinates.Y)
                            {
                                Canvas.SetTop(verticalthumb, top);
                                Value = CalculateRange(top) + Minimum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }

                            }
                            else
                            {
                                //Value=CalculateRange(top);
                                Value = CalculateRange(ordinates.Y) + Minimum;
                                if (Value > Maximum)
                                    Value = Maximum;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                                Canvas.SetTop(verticalthumb, ordinates.Y);
                            }
                        }
                    }
                    else
                    {
                        if (!toupdate)
                        {
                            if (ordinates.Y > firsttop)
                            {
                                Value = Value + TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                            else
                            {
                                Value = Value - TickFrequency;
                                if (!(Calculate(Value) < 0))
                                {
                                    RangeValue = Calculate(Value);
                                }
                                else
                                {
                                    RangeValue = 0;
                                }
                            }
                        }
                    }
                    Canvas.SetLeft(verticalthumb, left);
                }

            }

        }

        /// <summary>
        /// Handles the MouseEnter event of the verticalbackButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void verticalbackButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ordinates = e.GetPosition(verticalcan1);
            double height = verticaltrackrect.DesiredSize.Height;
        }

        /// <summary>
        /// Handles the MouseEnter event of the backButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void backButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ordinates = e.GetPosition(can1);
            double height = trackrect.DesiredSize.Height;
            double h = incButton.ActualWidth;

        }

        /// <summary>
        /// Thumbs the drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void thumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double oldleft = Canvas.GetLeft(sender as Thumb);
            double secondthumbleft = Canvas.GetLeft(thumbsecond);
            double left = Canvas.GetLeft(sender as Thumb) + e.HorizontalChange;
            newvalue = left;
            double width1 = trackrect.ActualWidth;
            double top = Canvas.GetTop(sender as Thumb);
            if (IsSnapToTickEnabled)
            {
                ResetValue();
            }
            if (this.RangeVisibility)
            {
                if (!IsSnapToTickEnabled)
                {
                    if (left < secondthumbleft && left > thumb.Width / 6)
                    {
                        Canvas.SetLeft(sender as Thumb, left);
                        Range.Start = CalculateRange(left - oldleft) + Range.Start;
                        if ((Range.Start) < this.Minimum)
                            Range.Start = this.Minimum;
                        Canvas.SetLeft(rect, left + thumb.Width / 2);
                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;
                    }
                    else if (left < secondthumbleft && left <= thumb.Width / 6)
                    {
                        Canvas.SetLeft(sender as Thumb, thumb.Width / 6);

                        Range.Start = this.Minimum;
                        Canvas.SetLeft(rect, thumb.Width / 6);
                        RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumb.Width / 6;

                    }
                    thumb2focus = false;
                    thumb1focus = true;
                    VisualStateManager.GoToState(thumb, "Focus", true);
                    VisualStateManager.GoToState(thumbsecond, "MouseEnter", true);
                }
            }
            else
            {
                if (left < (width1 + thumb.Width / 6) && left > thumb.Width / 6)
                {
                    if (!IsSnapToTickEnabled)
                    {
                        Canvas.SetLeft(sender as Thumb, left);
                        if (e.HorizontalChange < 0)
                        {
                            Value = CalculateRange(left - oldleft) + Value;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                        else
                        {
                            Value = CalculateRange(left - oldleft) + Value;
                            if (!(Calculate(Value) < 0))
                            {
                                RangeValue = Calculate(Value);
                            }
                            else
                            {
                                RangeValue = 0;
                            }
                        }
                    }
                }
                else if (left <= thumb.Width / 6)
                {
                    Canvas.SetLeft(sender as Thumb, thumb.Width / 6);
                    Value = Minimum;
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                }
                else if (left + thumb.Width / 2 >= width1)
                {
                    Canvas.SetLeft(thumb, width1 + thumb.Width / 6);
                    Value = Maximum;
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                }

            }
            Canvas.SetTop(sender as Thumb, top);

        }
        /// <summary>
        /// Handles the DragDelta event of the verticalthumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void verticalthumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double oldtop = Canvas.GetTop(sender as Thumb);
            double secondthumbtop = Canvas.GetTop(verticalthumbsecond);
            double top = Canvas.GetTop(sender as Thumb) + e.VerticalChange;
            double height1 = verticaltrackrect.ActualHeight;
            double left = Canvas.GetLeft(sender as Thumb);
            newvalue = top;
            if (IsSnapToTickEnabled)
            {
                ResetValue();
            }
            if (this.RangeVisibility)
            {
                if (!IsSnapToTickEnabled)
                {
                    if (top < secondthumbtop && top >= verticalthumb.Height / 4)
                    {
                        Canvas.SetTop(sender as Thumb, top);
                        Range.Start = VerticalCalculateRange(top - oldtop) + Range.Start;
                        if ((Range.Start) < this.Minimum)
                            Range.Start = this.Minimum;

                        Canvas.SetTop(verticalrect, top + verticalthumb.Height / 2);
                        RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start))) + verticalthumb.Height / 2;


                    }
                    else if (top < secondthumbtop)
                    {
                        Canvas.SetTop(sender as Thumb, verticalthumb.Height / 4);
                        Range.Start = this.Minimum;
                        Canvas.SetTop(verticalrect, verticalthumb.Height / 2);
                        RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start))) + verticalthumb.Height / 2;
                    }
                    thumb2focus = false;
                    thumb1focus = true;
                    VisualStateManager.GoToState(verticalthumb, "Focus", true);
                    VisualStateManager.GoToState(verticalthumbsecond, "MouseEnter", true);
                }
            }
            else
            {
                if (!IsSnapToTickEnabled)
                {
                    if (top < (height1 + verticalthumb.Height / 6) && top > verticalthumb.Height / 6)
                    {
                        Canvas.SetTop(sender as Thumb, top);
                        if (e.VerticalChange < 0)
                        {
                            Value = CalculateRange(top - oldtop) + Value;
                        }
                        else
                        {
                            Value = CalculateRange(top - oldtop) + Value;
                        }
                    }
                    else if (top <= verticalthumb.Height / 6)
                    {
                        Canvas.SetTop(sender as Thumb, verticalthumb.Height / 6);
                        Value = Minimum;
                    }
                    else if (top > height1 + verticalthumb.Height / 6)
                    {
                        Canvas.SetTop(sender as Thumb, height1 + verticalthumb.Height / 6);
                        Value = Maximum;
                    }
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                }
            }
            Canvas.SetLeft(sender as Thumb, left);
        }

        /// <summary>
        /// Handles the MouseMove event of the backButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void backButton_MouseMove(object sender, MouseEventArgs e)
        {
            ordinates = e.GetPosition(can1);

        }

        /// <summary>
        /// Handles the Click event of the incButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void incButton_Click(object sender, RoutedEventArgs e)
        {
            double firstthumbleft, width1, oldleft, left, top; 
            if (IsSnapToTickEnabled)
            {
                ResetValueOnInc();
            }
            if (this.Orientation == Orientation.Horizontal)
            {
                firstthumbleft = Canvas.GetLeft(thumb) + thumb.Width / 2;
                width1 = trackrect.ActualWidth;
                oldleft = Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2;
                left = Canvas.GetLeft(thumbsecond) - thumbsecond.Width / 2 + CalculateUniformRange(this.UniformChange);
                top = Canvas.GetTop(thumbsecond);

                if (this.RangeVisibility)
                {
                    if (!thumb1focus)
                    {
                        if (!IsSnapToTickEnabled)
                        {
                            if (Range.End < this.Maximum && left > firstthumbleft - thumb.Width)
                            {
                                Canvas.SetLeft(thumbsecond, left + thumbsecond.Width / 2);
                                Range.End = Range.End - CalculateRange(oldleft - left);
                                if (Range.End > this.Maximum)
                                    Range.End = this.Maximum;

                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (left + thumbsecond.Width / 2 >= width1)
                            {
                                // Canvas.SetLeft(thumbsecond, width1 + thumbsecond.Width / 2);
                                Range.End = Range.End - CalculateRange(oldleft - (width1));
                                if (Range.End > this.Maximum)
                                    Range.End = this.Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (Range.End < this.Maximum && left > firstthumbleft - thumb.Width)
                            {
                                Range.End = Range.End + TickFrequency;
                                if (Range.End > Maximum)
                                    Range.End = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        Canvas.SetTop(thumbsecond, top);
                    }
                    else
                    {
                        left = Canvas.GetLeft(thumb) + thumb.Width / 2 + CalculateUniformRange(this.UniformChange);
                        if (!IsSnapToTickEnabled)
                        {
                            if (left >= thumb.Width / 2 && left < oldleft + thumb.Width)
                            {
                                Canvas.SetLeft(thumb, left + thumb.Width / 2);
                                Range.Start = CalculateRange(left - firstthumbleft) + Range.Start;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left >= thumb.Width / 2 && left < oldleft + thumb.Width)
                            {
                                Range.Start = Range.Start + TickFrequency;
                                if (Range.Start > Range.End)
                                    Range.Start = Range.End;
                                if (Range.Start > Maximum)
                                    Range.Start = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }
                else
                {
                    double leftthumb = Canvas.GetLeft(thumb) - thumb.Width / 2 + CalculateUniformRange(this.UniformChange);
                    oldleft = Canvas.GetLeft(thumb) - thumb.Width / 2;
                    double topthumb = Canvas.GetTop(thumb);
                    if (!IsSnapToTickEnabled)
                    {
                        if (Value < this.Maximum && (leftthumb + thumb.Width / 2) < width1 + thumb.Width / 6)
                        {
                            Canvas.SetLeft(thumb, leftthumb + thumb.Width / 2);
                            Value = Value + CalculateRange(leftthumb - oldleft);

                        }
                        else if (leftthumb < 0)
                        {
                            left = 0;
                            Canvas.SetLeft(thumb, left + thumb.Width / 2);
                            Value = 0;

                        }
                        else if (leftthumb + thumb.Width / 2 > width1)
                        {
                            Canvas.SetLeft(thumbsecond, 0);
                            Canvas.SetLeft(thumb, width1 + thumb.Width / 6);
                            Value = Value + CalculateRange(width1 - oldleft);
                            if (Value > Maximum)
                                Value = Maximum;

                        }
                    }
                    else
                    {
                        Value = Value + TickFrequency;
                        if (Value > Maximum)
                            Value = Maximum;

                    }
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                    Canvas.SetTop(thumb, topthumb);

                }
            }
            else
            {
                firstthumbleft = Canvas.GetTop(verticalthumb) + verticalthumb.Height / 2;
                width1 = verticaltrackrect.ActualHeight;
                oldleft = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2;
                left = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2 + CalculateUniformRange(this.UniformChange);
                top = Canvas.GetTop(thumbsecond);

                if (this.RangeVisibility)
                {
                    if (!thumb1focus)
                    {
                        if (!IsSnapToTickEnabled)
                        {
                            if (Range.End < this.Maximum && left > firstthumbleft - verticalthumb.Height)
                            {
                                Canvas.SetTop(thumbsecond, left + verticalthumbsecond.Height / 2);
                                Range.End = Range.End - CalculateRange(oldleft - left);
                                if (Range.End > this.Maximum)
                                    Range.End = this.Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                            else if (left + verticalthumbsecond.Height / 2 >= width1)
                            {
                                //Canvas.SetTop(thumbsecond, width1 + verticalthumbsecond.Height / 2);
                                Range.End = Range.End - CalculateRange(oldleft - (width1));
                                if (Range.End > this.Maximum)
                                    Range.End = this.Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (Range.End < this.Maximum && left > firstthumbleft - verticalthumb.Height)
                            {
                                Range.End = Range.End + TickFrequency;
                                if (Range.End > Maximum)
                                    Range.End = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        Canvas.SetLeft(thumbsecond, top);
                    }
                    else
                    {
                        left = Canvas.GetTop(verticalthumb) + verticalthumb.Height / 2 + CalculateUniformRange(this.UniformChange);
                        if (!IsSnapToTickEnabled)
                        {
                            if (left >= verticalthumb.Height / 2 && left < oldleft + verticalthumb.Height)
                            {
                                Canvas.SetTop(verticalthumb, left + verticalthumb.Height / 2);
                                Range.Start = CalculateRange(left - firstthumbleft) + Range.Start;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                        else
                        {
                            if (left >= verticalthumb.Height / 2 && left < oldleft + verticalthumb.Height)
                            {
                                Range.Start = Range.Start + TickFrequency;
                                if (Range.Start > Range.End)
                                    Range.Start = Range.End;
                                if (Range.Start > Maximum)
                                    Range.Start = Maximum;
                                RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                            }
                        }
                    }
                }
                else
                {
                    double leftthumb = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2 + CalculateUniformRange(this.UniformChange);
                    oldleft = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2;
                    double topthumb = Canvas.GetLeft(verticalthumb);

                    if (!IsSnapToTickEnabled)
                    {
                        if (Value < this.Maximum && leftthumb >= verticalthumb.Height / 6 && (leftthumb + verticalthumb.Height / 2) < width1 + verticalthumb.Height / 6)
                        {
                            Canvas.SetTop(verticalthumb, leftthumb + verticalthumb.Height / 2);
                            Value = Value + CalculateRange(leftthumb - oldleft);

                        }
                        else if (leftthumb < 0)
                        {
                            left = 0;
                            Canvas.SetTop(verticalthumb, left + verticalthumb.Height / 2);
                            Value = 0;

                        }
                        else if (leftthumb + verticalthumb.Height / 2 > width1)
                        {
                            Canvas.SetTop(verticalthumbsecond, 0);
                            Canvas.SetTop(verticalthumb, width1 + verticalthumb.Height / 6);
                            Value = Value + CalculateRange(width1 - oldleft);
                            if (Value > Maximum)
                                Value = Maximum;

                        }
                    }
                    else
                    {
                        Value = Value + TickFrequency;
                        if (Value > Maximum)
                            Value = Maximum;

                    }
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                    Canvas.SetLeft(verticalthumb, topthumb);

                }

            }

        }

        /// <summary>
        /// Handles the Click event of the verticalincButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void verticalincButton_Click(object sender, RoutedEventArgs e)
        {
            double firstthumbtop = Canvas.GetTop(verticalthumb) + verticalthumb.Height;

            double height1 = verticaltrackrect.ActualHeight;
            double oldtop = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2;
            double top = Canvas.GetTop(verticalthumbsecond) - verticalthumbsecond.Height / 2 + VerticalCalculateUniformRange(this.UniformChange);
            double left = Canvas.GetLeft(verticalthumbsecond); 
            if (IsSnapToTickEnabled)
            {
                ResetValueOnInc();
            }
            if (this.RangeVisibility)
            {
                if (!IsSnapToTickEnabled)
                {
                    if (top <= (height1) && top > firstthumbtop - verticalthumb.Height)
                    {
                        Canvas.SetTop(verticalthumbsecond, top + verticalthumbsecond.Height / 2);
                        Range.End = Range.End - VerticalCalculateRange(oldtop - top);
                        RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start)));
                    }
                    if (top > height1)
                    {
                        Canvas.SetTop(verticalthumbsecond, height1 + verticalthumbsecond.Height / 2);
                        Range.End = Range.End - VerticalCalculateRange(oldtop - height1);
                        RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start)));
                    }
                }
                else
                {
                    if (!thumb1focus)
                    {
                        if (Range.End < this.Maximum && top > firstthumbtop - thumb.Height)
                        {
                            Range.End = Range.End + TickFrequency;
                            if (Range.End > Maximum)
                                Range.End = Maximum;
                            RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                        }
                    }
                    else
                    {
                        if (left >= thumb.Height / 2 && left < oldtop + thumb.Height)
                        {
                            Range.Start = Range.Start + TickFrequency;
                            if (Range.Start > Range.End)
                                Range.Start = Range.End;
                            if (Range.Start > Maximum)
                                Range.Start = Maximum;
                            RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                        }
                    }
                }
            }
            else
            {
                double topthumb = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2 + VerticalCalculateUniformRange(this.UniformChange);
                oldtop = Canvas.GetTop(verticalthumb) - verticalthumb.Height / 2;
                double leftthumb = Canvas.GetLeft(verticalthumb);
                if (!IsSnapToTickEnabled)
                {
                    if (topthumb < height1 && topthumb >= 0)
                    {
                        Canvas.SetTop(verticalthumb, topthumb + verticalthumb.Height / 2);

                    }
                    else if (leftthumb < 0)
                    {
                        left = 0;
                        Canvas.SetTop(verticalthumb, left + verticalthumb.Height / 2);

                    }
                    else if (leftthumb > height1)
                    {
                        Canvas.SetTop(verticalthumbsecond, 0);
                        Canvas.SetTop(verticalthumb, height1 + verticalthumb.Height / 2);

                    }
                }
                else
                {
                    Value = Value + TickFrequency;
                    if (Value > Maximum)
                        Value = Maximum;
                    if (!(Calculate(Value) < 0))
                    {
                        RangeValue = Calculate(Value);
                    }
                    else
                    {
                        RangeValue = 0;
                    }
                }
            }
            Canvas.SetLeft(verticalthumbsecond, left);
        }



        /// <summary>
        /// Handles the MouseLeftButtonDown event of the thumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void thumb_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Thumb thmb = sender as Thumb;
            if (thmb != null)
            {
                FirstThumbIsPressed = true;
            }
        }
        internal double MousePointX, MousePointY;
        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point p = e.GetPosition(can1);
            MousePointX = p.X;
            p = e.GetPosition(verticalcan1);
            MousePointY = p.Y;
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Thumbs the second drag delta.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void thumbSecondDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thickness m = (sender as Thumb).Margin;
            double firstthumbleft = Canvas.GetLeft(thumb) + thumb.Width / 2;
            double width1 = trackrect.ActualWidth;
            double oldleft = Canvas.GetLeft(sender as Thumb) - thumbsecond.Width / 2;
            double left = Canvas.GetLeft(sender as Thumb) - thumbsecond.Width / 2 + (e.HorizontalChange);
            newvalue = left;
            double top = Canvas.GetTop(sender as Thumb);

            if (IsSnapToTickEnabled)
            {
                ResetValue();
            }
            if (!IsSnapToTickEnabled)
            {
                if (Range.End < Maximum && left > firstthumbleft - thumb.Width)
                {
                    Canvas.SetLeft(thumbsecond, left + thumbsecond.Width / 2);

                    Range.End = Range.End - CalculateRange(oldleft - left);
                    if (Range.End > this.Maximum)
                        Range.End = this.Maximum;

                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start))) + thumbsecond.Width / 6;

                }
                else if (left + thumbsecond.Width / 2 > width1)
                {
                    Canvas.SetLeft(thumbsecond, width1 + thumbsecond.Width / 6);
                    Range.End = Range.End - CalculateRange(oldleft - width1);
                    if (Range.End > this.Maximum)
                        Range.End = this.Maximum;

                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                }
                else if (Range.End == Maximum && e.HorizontalChange < 0)
                {
                    Canvas.SetLeft(thumbsecond, left + thumbsecond.Width / 2);

                    Range.End = Range.End - CalculateRange(oldleft - (left));
                    if (Range.End > this.Maximum)
                        Range.End = this.Maximum;

                    RangeValue = Math.Abs((Calculate(Range.End) - Calculate(Range.Start)));
                }
            }
            Canvas.SetTop(sender as Thumb, top);
            thumb1focus = false;
            thumb2focus = true;
            VisualStateManager.GoToState(thumbsecond, "Focus", true);
            VisualStateManager.GoToState(thumb, "MouseEnter", true);
        }


        /// <summary>
        /// Handles the DragDelta event of the verticalthumbsecond control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void verticalthumbsecond_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double firstthumbtop = Canvas.GetTop(verticalthumb) + verticalthumb.Height / 2;

            double height1 = verticaltrackrect.ActualHeight;
            double oldtop = Canvas.GetTop(sender as Thumb) - verticalthumbsecond.Height / 2;
            double top = Canvas.GetTop(sender as Thumb) - verticalthumbsecond.Height / 2 + (e.VerticalChange);
            double left = Canvas.GetLeft(sender as Thumb);
            newvalue = top;
            VisualStateManager.GoToState(verticalthumbsecond, "Focus", false);

            VisualStateManager.GoToState(verticalthumb, "MouseOver", false);
            if (IsSnapToTickEnabled)
            {
                ResetValue();
            }
            if (!IsSnapToTickEnabled)
            {
                if (Range.End < Maximum && top > firstthumbtop - verticalthumb.Height)
                {
                    Canvas.SetTop(sender as Thumb, top + verticalthumbsecond.Height / 2);
                    Range.End = Range.End - VerticalCalculateRange(oldtop - top);
                    if (Range.End > this.Maximum)
                        Range.End = this.Maximum;

                    RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start))) + verticalthumbsecond.Height / 2;
                }
                else if (Range.End == Maximum && e.VerticalChange < 0)
                {
                    Canvas.SetTop(sender as Thumb, top + verticalthumbsecond.Height / 2);
                    Range.End = Range.End - VerticalCalculateRange(oldtop - top);
                    if (Range.End > this.Maximum)
                        Range.End = this.Maximum;

                    RangeValue = Math.Abs((VerticalCalculate(Range.End) - VerticalCalculate(Range.Start)));

                }
            }
            Canvas.SetLeft(sender as Thumb, left);
            thumb1focus = false;
            thumb2focus = true;
            VisualStateManager.GoToState(verticalthumbsecond, "Focus", true);
            VisualStateManager.GoToState(verticalthumb, "MouseEnter", true);
        }
        /// <summary>
        /// Calculates the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        double Calculate(double value)
        {
            double returnvalue;
            double width;

            double difference;
            if (this.Orientation == Orientation.Horizontal)
            {
                width = this.ControlWidth;
            }
            else
            {
                width = this.ControlHeight;
            }
            difference = ((Maximum - Minimum)) / 2;
            if (value >= difference)
            {
                returnvalue = (width - ((Maximum - value) / (((Maximum - Minimum)) / width))) - 2;
            }
            else
            {
                if (value == Minimum)
                {
                    returnvalue = ((value - Minimum) / (((Maximum - Minimum)) / width)) - 2;
                }
                else
                {
                    returnvalue = ((value - Minimum) / (((Maximum - Minimum)) / width)) - 2;
                }
            }
            // return Math.Round((returnvalue),1);
            return returnvalue;

        }
        /// <summary>
        /// Calculates the range.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <returns></returns>
        double CalculateRange(double value1)
        {

            double returnvalue;
            double width;
            if (this.Orientation == Orientation.Horizontal)
            {
                width = this.ControlWidth;
            }
            else
            {
                width = this.ControlHeight;
            }
            double intervalvalue = ((((Maximum - Minimum)) / width));
            returnvalue = intervalvalue * value1;
            return returnvalue;
        }
        /// <summary>
        /// Calculates the uniform range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        double CalculateUniformRange(double value)
        {
            double width;
            double difference;
            if (this.Orientation == Orientation.Horizontal)
            {
                width = this.ControlWidth;
            }
            else
            {
                width = this.ControlHeight;
            }

            difference = (Maximum - Minimum);
            return ((value * width / difference));
        }

        /// <summary>
        /// Verticals the calculate.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        double VerticalCalculate(double value)
        {
            double returnvalue;

            double difference;
            trackrect.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            difference = (Maximum - Minimum) + 1;
            if (value >= difference)
            {
                returnvalue = this.ControlHeight - ((Maximum - value) / (((Maximum - Minimum) + 1) / this.ControlHeight));
            }
            else
            {
                returnvalue = (value - Minimum) / (((Maximum - Minimum) + 1) / this.ControlHeight);
            }
            return Math.Round(returnvalue);

        }
        /// <summary>
        /// Verticals the calculate range.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <returns></returns>
        double VerticalCalculateRange(double value1)
        {

            double returnvalue;
            double intervalvalue = (((Maximum - Minimum) + 1) / this.ControlHeight);
            returnvalue = intervalvalue * value1;
            return returnvalue;
        }
        /// <summary>
        /// Verticals the calculate uniform range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        double VerticalCalculateUniformRange(double value)
        {
            double difference;
            difference = ((Maximum - Minimum) + 1);
            double l = Math.Ceiling((value * this.ControlHeight / difference));
            return l;
        }

        /// <summary>
        /// Determines whether [is range value changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsRangeValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {
                // rs.ArrangeSlider();
            }

        }
        /// <summary>
        /// Determines whether [is range changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsRangeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {

                if (rs.Range.End >= rs.Maximum)
                {
                    rs.Range.End = rs.Maximum;
                    if (rs.Range.Start >= rs.Range.End)
                    {
                        rs.Range.Start = rs.Range.End;
                    }
                }
                else
                {
                    if (rs.Range.Start >= rs.Range.End)
                    {
                        rs.Range.Start = rs.Range.End;
                    }
                }
                rs.ArrangeSlider();

                if (rs.RangeChanged != null)
                {
                    rs.RangeChanged(rs, e);
                }

            }


        }
        /// <summary>
        /// Determines whether [is orientation changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)o;
            if (rs.horizontal != null)
            {
                if (rs.HandleButtonVisibility == Visibility.Visible)
                {
                    rs.ControlWidth = rs.ControlGridWidth - 46;
                    rs.ControlHeight = rs.ControlGridHeight - 46;

                    rs.can1.Margin = new Thickness(0, 0, 0, 0);
                    rs.verticalcan1.Margin = new Thickness(0, 0, 0, 0);
                    rs.trackrect.Margin = new Thickness(5, 0, 5, 0);
                    rs.verticaltrackrect.Margin = new Thickness(0, 5, 0, 5);
                    if (rs.trackinnerrect != null)
                        rs.trackinnerrect.Margin = new Thickness(6, 1, 6, 1);
                    if (rs.verticaltrackinnerrect != null)
                        rs.verticaltrackinnerrect.Margin = new Thickness(1, 6, 1, 6);
                }
                else
                {
                    rs.ControlWidth = rs.ControlGridWidth - 46;
                    rs.ControlHeight = rs.Height - 46;


                    rs.can1.Margin = new Thickness(18, 0, 0, 0);
                    rs.trackrect.Margin = new Thickness(23, 0, 23, 0);
                    rs.verticalcan1.Margin = new Thickness(0, 18, 0, 0);
                    rs.verticaltrackrect.Margin = new Thickness(0, 23, 0, 23);
                    if (rs.trackinnerrect != null)
                        rs.trackinnerrect.Margin = new Thickness(24, 1, 24, 1);
                    if (rs.verticaltrackinnerrect != null)
                        rs.verticaltrackinnerrect.Margin = new Thickness(1, 24, 1, 24);

                }
                if (rs.LabelVisibility)
                {
                    if (rs.SetCustomLabel)
                    {
                        if (rs.ItemsCollection != null)
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {
                                rs.AddLabel();
                                rs.custompanellabel.minimum = rs.Minimum;
                                rs.custompanellabel.maximum = rs.Maximum;
                                rs.custompanel.minimum = rs.Minimum;
                                rs.custompanel.maximum = rs.Maximum;
                                rs.AddLabelDown();
                                rs.custompanellabeldown.minimum = rs.Minimum;
                                rs.custompanellabeldown.maximum = rs.Maximum;
                                rs.custompaneldown.minimum = rs.Minimum;
                                rs.custompaneldown.maximum = rs.Maximum;
                            }
                            else
                            {
                                rs.AddLabelVertical();
                                rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                rs.custompanelvertical.minimum = rs.Minimum;
                                rs.custompanelvertical.maximum = rs.Maximum;
                                rs.AddLabelVerticalDown();
                                rs.custompanelverticaldown.minimum = rs.Minimum;
                                rs.custompanelverticaldown.maximum = rs.Maximum;
                                rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                            }
                        }
                    }
                    else
                    {
                        if (rs.Orientation == Orientation.Horizontal)
                        {
                            if (rs.thumb1focus)
                            {
                                VisualStateManager.GoToState(rs.thumb, "Focus", true);
                                VisualStateManager.GoToState(rs.thumbsecond, "MouseEnter", true);
                            }
                            else if (rs.thumb2focus)
                            {
                                VisualStateManager.GoToState(rs.thumbsecond, "Focus", true);
                                VisualStateManager.GoToState(rs.thumb, "MouseEnter", true);
                            }
                            rs.AddChildren();
                            rs.AddChildrendown();

                        }
                        else
                        {
                            if (rs.thumb1focus)
                            {
                                VisualStateManager.GoToState(rs.verticalthumb, "Focus", true);
                                VisualStateManager.GoToState(rs.verticalthumbsecond, "MouseEnter", true);
                            }
                            else if (rs.thumb2focus)
                            {
                                VisualStateManager.GoToState(rs.verticalthumbsecond, "Focus", true);
                                VisualStateManager.GoToState(rs.verticalthumb, "MouseEnter", true);
                            }
                            rs.AddChildrenVertical();
                            rs.AddChildrenVerticalDown();
                        }

                    }
                }
                rs.ArrangeSlider();

            }
        }

        /// <summary>
        /// Determines whether [is handle visibilty changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsHandleVisibiltyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;


            if (rs.incButton != null)
            {
                if (rs.HandleButtonVisibility == Visibility.Visible)
                {
                    rs.ControlWidth = rs.ControlGridWidth - 46;
                    rs.ControlHeight = rs.ControlGridHeight - 46;


                    rs.can1.Margin = new Thickness(0, 0, 0, 0);
                    rs.verticalcan1.Margin = new Thickness(0, 0, 0, 0);
                    rs.trackrect.Margin = new Thickness(5, 0, 5, 0);
                    rs.backButton.Margin = new Thickness(5, 0, 5, 0);
                    rs.verticaltrackrect.Margin = new Thickness(0, 5, 0, 5);
                    rs.verticalbackButton.Margin = new Thickness(0, 5, 0, 5);
                    if (rs.trackinnerrect != null)
                        rs.trackinnerrect.Margin = new Thickness(6, 1, 6, 1);
                    if (rs.verticaltrackinnerrect != null)
                        rs.verticaltrackinnerrect.Margin = new Thickness(1, 6, 1, 6);

                }
                else
                {
                    rs.ControlWidth = rs.ControlGridWidth - 46;
                    rs.ControlHeight = rs.ControlGridHeight - 46;


                    rs.can1.Margin = new Thickness(18, 0, 0, 0);
                    rs.trackrect.Margin = new Thickness(23, 0, 23, 0);
                    rs.backButton.Margin = new Thickness(23, 0, 23, 0);
                    rs.verticalcan1.Margin = new Thickness(0, 18, 0, 0);
                    rs.verticaltrackrect.Margin = new Thickness(0, 23, 0, 23);
                    rs.verticalbackButton.Margin = new Thickness(0, 23, 0, 23);
                    if (rs.trackinnerrect != null)
                        rs.trackinnerrect.Margin = new Thickness(24, 1, 24, 1);
                    if (rs.verticaltrackinnerrect != null)
                        rs.verticaltrackinnerrect.Margin = new Thickness(1, 24, 1, 24);

                }
                if (rs.LabelVisibility)
                {
                    if (rs.SetCustomLabel)
                    {
                        if (rs.ItemsCollection != null)
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {
                                rs.AddLabel();
                                rs.custompanellabel.minimum = rs.Minimum;
                                rs.custompanellabel.maximum = rs.Maximum;
                                rs.custompanel.minimum = rs.Minimum;
                                rs.custompanel.maximum = rs.Maximum;
                                rs.AddLabelDown();
                                rs.custompanellabeldown.minimum = rs.Minimum;
                                rs.custompanellabeldown.maximum = rs.Maximum;
                                rs.custompaneldown.minimum = rs.Minimum;
                                rs.custompaneldown.maximum = rs.Maximum;
                            }
                            else
                            {
                                rs.AddLabelVertical();
                                rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                rs.custompanelvertical.minimum = rs.Minimum;
                                rs.custompanelvertical.maximum = rs.Maximum;
                                rs.AddLabelVerticalDown();
                                rs.custompanelverticaldown.minimum = rs.Minimum;
                                rs.custompanelverticaldown.maximum = rs.Maximum;
                                rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                            }
                        }
                    }
                    else
                    {
                        if (rs.Orientation == Orientation.Horizontal)
                        {

                            rs.AddChildren();
                            rs.AddChildrendown();
                        }
                        else
                        {
                            rs.AddChildrenVertical();
                            rs.AddChildrenVerticalDown();
                        }

                    }
                }
                rs.ArrangeSlider();
            }

        }

        /// <summary>
        /// Determines whether [is tick frequency changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsTickFrequencyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {
                if (rs.LabelVisibility)
                {
                    if (rs.SetCustomLabel)
                    {
                        if (rs.ItemsCollection != null)
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {
                                rs.AddLabel();
                                rs.custompanellabel.minimum = rs.Minimum;
                                rs.custompanellabel.maximum = rs.Maximum;
                                rs.custompanel.minimum = rs.Minimum;
                                rs.custompanel.maximum = rs.Maximum;
                                rs.AddLabelDown();
                                rs.custompanellabeldown.minimum = rs.Minimum;
                                rs.custompanellabeldown.maximum = rs.Maximum;
                                rs.custompaneldown.minimum = rs.Minimum;
                                rs.custompaneldown.maximum = rs.Maximum;
                            }
                            else
                            {
                                rs.AddLabelVertical();
                                rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                rs.custompanelvertical.minimum = rs.Minimum;
                                rs.custompanelvertical.maximum = rs.Maximum;
                                rs.AddLabelVerticalDown();
                                rs.custompanelverticaldown.minimum = rs.Minimum;
                                rs.custompanelverticaldown.maximum = rs.Maximum;
                                rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                            }
                        }

                    }
                }
                if (!rs.SetCustomLabel)
                {
                    if (rs.Orientation == Orientation.Horizontal)
                    {

                        rs.AddChildren();
                        rs.AddChildrendown();
                    }
                    else
                    {
                        rs.AddChildrenVertical();
                        rs.AddChildrenVerticalDown();
                    }
                }



                rs.ArrangeSlider();
            }

        }

        /// <summary>
        /// Determines whether [is tick placement changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsTickPlacementChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {
                if (rs.LabelVisibility)
                {
                    if (rs.SetCustomLabel)
                    {
                        if (rs.ItemsCollection != null)
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {
                                rs.AddLabel();
                                rs.custompanellabel.minimum = rs.Minimum;
                                rs.custompanellabel.maximum = rs.Maximum;
                                rs.custompanel.minimum = rs.Minimum;
                                rs.custompanel.maximum = rs.Maximum;
                                rs.AddLabelDown();
                                rs.custompanellabeldown.minimum = rs.Minimum;
                                rs.custompanellabeldown.maximum = rs.Maximum;
                                rs.custompaneldown.minimum = rs.Minimum;
                                rs.custompaneldown.maximum = rs.Maximum;
                            }
                            else
                            {
                                rs.AddLabelVertical();
                                rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                rs.custompanelvertical.minimum = rs.Minimum;
                                rs.custompanelvertical.maximum = rs.Maximum;
                                rs.AddLabelVerticalDown();
                                rs.custompanelverticaldown.minimum = rs.Minimum;
                                rs.custompanelverticaldown.maximum = rs.Maximum;
                                rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                            }
                        }
                    }
                    else
                    {
                        if (rs.Orientation == Orientation.Horizontal)
                        {

                            rs.AddChildren();
                            rs.AddChildrendown();
                        }
                        else
                        {
                            rs.AddChildrenVertical();
                            rs.AddChildrenVerticalDown();
                        }

                    }
                }
                rs.ArrangeSlider();
            }

        }

        /// <summary>
        /// Determines whether [is uniform changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsUniformChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {

                rs.ArrangeSlider();
            }

        }
        /// <summary>
        /// Determines whether [is minimum changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {
                if ((double)e.NewValue < rs.Maximum)
                {
                    if (rs.LabelVisibility)
                    {
                        if (rs.SetCustomLabel)
                        {
                            if (rs.ItemsCollection != null)
                            {
                                if (rs.Orientation == Orientation.Horizontal)
                                {
                                    rs.AddLabel();
                                    rs.custompanellabel.minimum = rs.Minimum;
                                    rs.custompanellabel.maximum = rs.Maximum;
                                    rs.custompanel.minimum = rs.Minimum;
                                    rs.custompanel.maximum = rs.Maximum;
                                    rs.AddLabelDown();
                                    rs.custompanellabeldown.minimum = rs.Minimum;
                                    rs.custompanellabeldown.maximum = rs.Maximum;
                                    rs.custompaneldown.minimum = rs.Minimum;
                                    rs.custompaneldown.maximum = rs.Maximum;
                                }
                                else
                                {
                                    rs.AddLabelVertical();
                                    rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                    rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                    rs.custompanelvertical.minimum = rs.Minimum;
                                    rs.custompanelvertical.maximum = rs.Maximum;
                                    rs.AddLabelVerticalDown();
                                    rs.custompanelverticaldown.minimum = rs.Minimum;
                                    rs.custompanelverticaldown.maximum = rs.Maximum;
                                    rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                    rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                                }
                            }
                        }
                        else
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {

                                rs.AddChildren();
                                rs.AddChildrendown();
                            }
                            else
                            {
                                rs.AddChildrenVertical();
                                rs.AddChildrenVerticalDown();
                            }

                        }
                    }
                    if (rs.Range.Start < rs.Minimum)
                    {
                        rs.Range.Start = rs.Minimum;

                        if (rs.Range.End <= rs.Range.Start)
                        {
                            rs.Range.End = rs.Range.Start;
                        }
                    }
                    if (rs.Value < rs.Minimum)
                    {
                        rs.Value = rs.Minimum;
                    }
                    rs.ArrangeSlider();
                }
                else
                {
                    rs.Minimum = (double)e.OldValue;
                }
            }

        }
        /// <summary>
        /// Determines whether [is maximum changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.incButton != null)
            {
                if ((double)e.NewValue > rs.Minimum)
                {
                    if (rs.LabelVisibility)
                    {
                        if (rs.SetCustomLabel)
                        {
                            if (rs.ItemsCollection != null)
                            {
                                if (rs.Orientation == Orientation.Horizontal)
                                {
                                    rs.AddLabel();
                                    rs.custompanellabel.minimum = rs.Minimum;
                                    rs.custompanellabel.maximum = rs.Maximum;
                                    rs.custompanel.minimum = rs.Minimum;
                                    rs.custompanel.maximum = rs.Maximum;
                                    rs.AddLabelDown();
                                    rs.custompanellabeldown.minimum = rs.Minimum;
                                    rs.custompanellabeldown.maximum = rs.Maximum;
                                    rs.custompaneldown.minimum = rs.Minimum;
                                    rs.custompaneldown.maximum = rs.Maximum;
                                }
                                else
                                {
                                    rs.AddLabelVertical();
                                    rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                    rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                    rs.custompanelvertical.minimum = rs.Minimum;
                                    rs.custompanelvertical.maximum = rs.Maximum;
                                    rs.AddLabelVerticalDown();
                                    rs.custompanelverticaldown.minimum = rs.Minimum;
                                    rs.custompanelverticaldown.maximum = rs.Maximum;
                                    rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                    rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                                }
                            }
                        }
                        else
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {

                                rs.AddChildren();
                                rs.AddChildrendown();
                            }
                            else
                            {
                                rs.AddChildrenVertical();
                                rs.AddChildrenVerticalDown();
                            }

                        }
                    }
                    if (rs.Range.End > rs.Maximum)
                    {
                        rs.Range.End = rs.Maximum;
                    }
                    rs.ArrangeSlider();

                }
                else
                {
                    rs.Maximum = (double)e.OldValue;
                }
            }

        }

        /// <summary>
        /// Determines whether [is move to handle changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsMoveToHandleChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;


        }
        /// <summary>
        /// Determines whether [is value changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                rs.CoerceValue();
                rs.ArrangeSlider();
            }
            if (rs.ValueChanged != null)
            {
                rs.ValueChanged(rs, e);
            }
        }

        private void CoerceValue()
        {
            if (RangeVisibility)
            {
                if (Range.Start < Minimum)
                {
                    Range.Start = Minimum;
                }
                if (Range.End > Maximum)
                {
                    Range.End = Maximum;
                }
            }
            if (Value < Minimum)
            {
                Value = Minimum;
            }
            if (Value > Maximum)
            {
                Value = Maximum;
            }
        }

        /// <summary>
        /// Determines whether [is range visibility changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsRangeVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                // rs.Value = rs.Range.Start;

                rs.ArrangeSlider();
            }


        }

        /// <summary>
        /// Determines whether [is label orientation changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsLabelOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                if (rs.Orientation == Orientation.Horizontal)
                {
                    rs.AddChildren();
                    rs.AddChildrendown();
                }
                else
                {

                    rs.AddChildrenVertical();
                    rs.AddChildrenVerticalDown();
                }
                rs.ArrangeSlider();
            }


        }


        /// <summary>
        /// Determines whether [is label visibility changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsLabelVisibilityChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                if (rs.LabelVisibility)
                {
                    if (rs.SetCustomLabel)
                    {
                        if (rs.ItemsCollection != null)
                        {
                            if (rs.Orientation == Orientation.Horizontal)
                            {
                                rs.AddLabel();
                                rs.custompanellabel.minimum = rs.Minimum;
                                rs.custompanellabel.maximum = rs.Maximum;
                                rs.custompanel.minimum = rs.Minimum;
                                rs.custompanel.maximum = rs.Maximum;
                                rs.AddLabelDown();
                                rs.custompanellabeldown.minimum = rs.Minimum;
                                rs.custompanellabeldown.maximum = rs.Maximum;
                                rs.custompaneldown.minimum = rs.Minimum;
                                rs.custompaneldown.maximum = rs.Maximum;
                            }
                            else
                            {
                                rs.AddLabelVertical();
                                rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                                rs.custompanelvertical.minimum = rs.Minimum;
                                rs.custompanelvertical.maximum = rs.Maximum;
                                rs.AddLabelVerticalDown();
                                rs.custompanelverticaldown.minimum = rs.Minimum;
                                rs.custompanelverticaldown.maximum = rs.Maximum;
                                rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                                rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                            }
                        }
                    }
                    else
                    {
                        if (rs.Orientation == Orientation.Horizontal)
                        {

                            rs.AddChildren();
                            rs.AddChildrendown();
                        }
                        else
                        {
                            rs.AddChildrenVertical();
                            rs.AddChildrenVerticalDown();
                        }

                    }
                }
                rs.ArrangeSlider();
            }


        }
        /// <summary>
        /// Determines whether [is items changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                if (rs.Orientation == Orientation.Horizontal)
                {
                    rs.AddChildren();
                    rs.AddChildrendown();
                }
                else
                {
                    rs.AddChildrenVertical();
                    rs.AddChildrenVerticalDown();
                }
                rs.ArrangeSlider();
            }


        }

        /// <summary>
        /// Determines whether [is items collection changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsItemsCollectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {

            RangeSlider rs = (RangeSlider)o;




            if (rs.thumb != null)
            {
                if (rs.Orientation == Orientation.Horizontal)
                {
                    rs.AddChildren();
                    rs.AddChildrendown();
                }
                else
                {
                    rs.AddChildrenVertical();
                    rs.AddChildrenVerticalDown();
                }
                rs.ArrangeSlider();
            }


        }

        /// <summary>
        /// Determines whether [is set custom label changed] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsSetCustomLabelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RangeSlider rs = (RangeSlider)o;
            if (rs.thumb != null)
            {
                if (rs.SetCustomLabel)
                {
                    if (rs.ItemsCollection != null)
                    {
                        if (rs.Orientation == Orientation.Horizontal)
                        {
                            rs.AddLabel();
                            rs.custompanellabel.minimum = rs.Minimum;
                            rs.custompanellabel.maximum = rs.Maximum;
                            rs.custompanel.minimum = rs.Minimum;
                            rs.custompanel.maximum = rs.Maximum;
                            rs.AddLabelDown();
                            rs.custompanellabeldown.minimum = rs.Minimum;
                            rs.custompanellabeldown.maximum = rs.Maximum;
                            rs.custompaneldown.minimum = rs.Minimum;
                            rs.custompaneldown.maximum = rs.Maximum;
                        }
                        else
                        {
                            rs.AddLabelVertical();
                            rs.CustomPanelVerticalLabel.minimum = rs.Minimum;
                            rs.CustomPanelVerticalLabel.maximum = rs.Maximum;
                            rs.custompanelvertical.minimum = rs.Minimum;
                            rs.custompanelvertical.maximum = rs.Maximum;
                            rs.AddLabelVerticalDown();
                            rs.custompanelverticaldown.minimum = rs.Minimum;
                            rs.custompanelverticaldown.maximum = rs.Maximum;
                            rs.CustomPanelVerticalLabelDown.minimum = rs.Minimum;
                            rs.CustomPanelVerticalLabelDown.maximum = rs.Maximum;
                        }
                    }
                }
                else
                {
                    rs.custompanellabel.IsItem = false;
                    rs.custompanel.IsItem = false;
                    rs.custompaneldown.IsItem = false;
                    rs.custompanelvertical.IsItem = false;
                    rs.custompanelverticaldown.IsItem = false;
                    rs.custompanellabeldown.IsItem = false;
                    rs.CustomPanelVerticalLabel.IsItem = false;
                    rs.CustomPanelVerticalLabelDown.IsItem = false;
                    if (rs.Orientation == Orientation.Horizontal)
                    {

                        rs.AddChildren();
                        rs.AddChildrendown();
                    }
                    else
                    {
                        rs.AddChildrenVertical();
                        rs.AddChildrenVerticalDown();
                    }

                }
                rs.ArrangeSlider();
            }
        }
    }

}
