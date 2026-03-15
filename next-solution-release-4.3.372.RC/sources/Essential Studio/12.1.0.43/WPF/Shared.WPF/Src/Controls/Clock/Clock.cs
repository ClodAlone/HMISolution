// <copyright file="Clock.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using System.Globalization;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a Clock control.
    /// </summary>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <para/>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public partial class Clock : Control</code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example><code><![CDATA[<local:Clock Name="customControlClock" />]]></code></example>
    /// </list>
    /// <para/>
    /// </example>
    /// </list>
    /// <para/>
    /// <remarks>
    /// Clock class models a ticking clock.
    /// This class is used to tick current or user-defined time. 
    /// <para/>A Clock enables user dragging its hands, doing mouse wheel operations and choosing am/pm time.
    /// Also Clock control contains a text block to display time.
    /// <para/>To create a Clock using C# you can use the Clock method.
    /// <para/>A Clock supports Windows themes (Default, Silver, Metallic, Zune, Royale and Aero) and skins (Office2003, Office2007Blue,
    /// Office2007Black, Office2007Silver and Blend). Also you can define your own user skin by setting properties of the Clock class.
    /// </remarks>
    /// <para/> 
    /// <example>
    /// <para/>This example shows how to create a Clock in XAML.
    /// <code>
    /// <![CDATA[
    /// <Window x:Class="Clock.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Tools.WPF"
    /// Title="Clock" Height="300" Width="300">
    /// <StackPanel HorizontalAlignment="Center">
    ///     <local:Clock Name="customControlClock" />
    /// </StackPanel>
    /// </Window>
    /// ]]>
    /// </code>
    /// <para/>
    /// <para/>This example shows how to create a Clock in C#.
    /// <code>
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// <para/>
    /// namespace Sample1
    /// {
    ///     public partial class Window1 : Window
    ///     {
    ///        public Window1()
    ///        {
    ///             InitializeComponent();
    ///             <para/>
    ///             Clock clock = new Clock();
    ///             stackPanel.Children.Add( clock );
    ///         }
    ///     }
    /// }
    /// </code>   
    /// </example>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
     Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(Clock), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/Clock/Themes/TransparentStyle.xaml")]

    public partial class Clock : Control
    {
        #region Constants
        /// <summary>
        /// Contains radius value of the first clock border.
        /// </summary>
        private const double C_firstBorderFrameRadius = 151;

        /// <summary>
        /// Contains radius value of the second inner border of clock
        /// </summary>
        private const double C_secondInnerBorderFrameRadius = 135;

        /// <summary>
        /// Contains radius of the third border of clock
        /// </summary>
        private const double C_thirdBorderFrameRadius = 122;

        /// <summary>
        /// Contains radius of the centered ellipse 
        /// </summary>
        private const double C_centeredEllipseRadius = 10;

        /// <summary>
        /// Contains width of the inner clock      
        /// </summary>
        private const double C_innerClockGeneralWidth = 125;

        /// <summary>
        /// Contains the height of the inner clock
        /// </summary>
        private const double C_innerClockGeneralHeight = 130;

        /// <summary>
        /// Contains min width of the clock frame;
        /// </summary>
        private const double C_frameMinWidth = 174;

        /// <summary>
        /// Contains angle offset of the clock hand when mouse dragging.
        /// </summary>
        private const int C_angleOffset = 180;

        /// <summary>
        /// Contains rotation offset of the clock hand when mouse dragging.
        /// </summary>
        private const int C_rotationOffset = 90;

        //// <summary>
        // Contains rotation offset of the clock hand when MouseWheel.
        // </summary>
        // private const double c_mouseWheelOffset = 6;
        // <summary>
        // Contains rotation offset of the Hour hand when MouseWheel.
        // </summary>
        //// private const double c_mouseWheelHourOffset = 30;

        /// <summary>
        /// Contains name of the HourHand.
        /// </summary>
        private const string C_hourHandName = "HourHand";

        /// <summary>
        /// Contains name of the MinuteHand.
        /// </summary>
        private const string C_minuteHandName = "MinuteHand";

        /// <summary>
        /// Contains name of the SecondHand.
        /// </summary>
        private const string C_secondHandName = "SecondHand";

        /// <summary>
        /// Contains name of the HourHand RotateTransform.
        /// </summary>
        private const string C_hourHandRotateTransformName = "HourHandRotateTransform";

        /// <summary>
        /// Contains name of the MinuteHand RotateTransform.
        /// </summary>
        private const string C_minuteHandRotateTransformName = "MinuteHandRotateTransform";

        /// <summary>
        /// Contains name of the SecondHand RotateTransform.
        /// </summary>
        private const string C_secondHandRotateTransformHandName = "SecondHandRotateTransform";

        /// <summary>
        /// Contains name of the centered ellipse.
        /// </summary>
        private const string C_centeredEllipseName = "CenteredEllipse";

        /// <summary>
        /// Contains name of the text block up button
        /// </summary>
        private const string C_upRepeatButtonName = "UpRepeatButton";

        /// <summary>
        /// Contains name of the text block down button
        /// </summary>
        private const string C_downRepeatButtonName = "DownRepeatButton";

        /// <summary>
        /// Contains name of the text block up button
        /// </summary>
        private const string C_upInnerRepeatButtonName = "UpInnerRepeatButton";

        /// <summary>
        /// Contains name of the text block down button
        /// </summary>
        private const string C_downInnerRepeatButtonName = "DownInnerRepeatButton";

        /// <summary>
        /// Contains name of the default FirstBorderFrameColor
        /// </summary>
        private const string C_firstBorderFrameColorName = "FirstBorderFrameColor";

        /// <summary>
        /// Contains name of the default ThirdBorderFrameBackgroundColor
        /// </summary>
        private const string C_thirdBorderFrameBackgroundColorName = "ThirdBorderFrameBackgroundColor";

        /// <summary>
        /// Contains name of the default ThirdBorderFrameBrushColor
        /// </summary>
        private const string C_thirdBorderFrameBrushColorName = "ThirdBorderFrameBrushColor";

        /// <summary>
        /// Contains name of the default DialBorderColor
        /// </summary>
        private const string C_dialBorderColorName = "DialBorderColor";

        /// <summary>
        /// Contains name of the default ClockPanelBorderColor
        /// </summary>
        private const string C_clockPanelBorderColorName = "ClockPanelBorderColor";

        /// <summary>
        /// Contains name of the default ClockPanelInnerBorderColor
        /// </summary>
        private const string C_clockPanelInnerBorderColorName = "ClockPanelInnerBorderColor";

        /// <summary>
        /// Contains name of the default ClockPanelBackgroundColor
        /// </summary>
        private const string C_clockPanelBackgroundColorName = "ClockPanelBackgroundColor";

        /// <summary>
        /// Contains name of the default AMPMSelectorBorderBrush
        /// </summary>
        private const string C_aMPMSelectorBorderBrushName = "ArrowBorderColor";

        /// <summary>
        /// Contains name of the default AMPMSelectorBackground
        /// </summary>
        private const string C_aMPMSelectorBackgroundName = "TextBlockBackground";

        /// <summary>
        /// Contains name of the default AMPMSelectorForeground
        /// </summary>
        private const string C_aMPMSelectorForegroundName = "TextBlockForeground";

        /// <summary>
        /// Contains name of the default AMPMSelectorButtonsArrowBrush
        /// </summary>
        private const string C_aMPMSelectorButtonsArrowBrushName = "ArrowFill";

        /// <summary>
        /// Contains name of the default AMPMSelectorButtonsBackground
        /// </summary>
        private const string C_aMPMSelectorButtonsBackgroundName = "RectangleStyle";

        /// <summary>
        /// Contains name of the default AMPMSelectorButtonsBorderBrush
        /// </summary>
        private const string C_aMPMSelectorButtonsBorderBrushName = "ArrowBorderColor";

        /// <summary>
        /// Contains name of the default AMPMMouseOverButtonsBorderBrush
        /// </summary>
        private const string C_aMPMMouseOverButtonsBorderBrushName = "ArrowBorderColorOver";

        /// <summary>
        /// Contains name of the default AMPMMouseOverButtonsArrowBrush
        /// </summary>
        private const string C_aMPMMouseOverButtonsArrowBrushName = "ArrowFill";

        /// <summary>
        /// Contains name of the default AMPMouseOverButtonsBackground
        /// </summary>
        private const string C_aMPMMouseOverButtonsBackgroundName = "RectangleOverStyle";

        /// <summary>
        /// Contains name of the default ClockPointBrush
        /// </summary>
        private const string C_ClockPointBrushName = "ClockPointColor";

        /// <summary>
        /// Contains name of the default CenteredEllipseColor
        /// </summary>
        private const string C_centerCircleBrushName = "CenteredEllipseColor";

        /// <summary>
        /// Contains the name of the default CenterCircleBrushName
        /// </summary>
        private const string C_secondHandBrushName = "SecondHandColor";

        /// <summary>
        /// Contains the name of the default SecondHandMosueOverBrushName
        /// </summary>
        private const string C_secondHandMouseOverBrushName = "SecondHandMouseOverColor";

        /// <summary>
        /// Contains the name of the default MinuteHandBrushName
        /// </summary>
        private const string C_minuteHandBrushName = "MinuteHandColor";

        /// <summary>
        /// Contains the name of the default HandBorderBrushName
        /// </summary>
        private const string C_minuteHandBorderBrushName = "MinuteHandColor";

        /// <summary>
        /// Contains the name of the default HandMouseOverBrushName
        /// </summary>
        private const string C_minuteHandMouseOverBrushName = "MinuteHandMouseOverColor";

        /// <summary>
        /// Contains the name of the default HandMouseOverBorderBrushName
        /// </summary>
        private const string C_minuteHandMouseOverBorderBrushName = "MinuteHandMouseOverColor";

        /// <summary>
        /// Contains the name of the default HourHandBrushName
        /// </summary>
        private const string C_hourHandBrushName = "HourHandColor";

        /// <summary>
        /// Contains the name of the default HourHandBorderBrushName
        /// </summary>
        private const string C_hourHandBorderBrushName = "HourHandColor";

        /// <summary>
        /// Contains the name of the default HourHandMouseOverBorderBrushName
        /// </summary>
        private const string C_hourHandMouseOverBrushName = "HourHandMouseOverColor";

        /// <summary>
        /// Contains the name of the default HourHandMouseOverBorderBrushName
        /// </summary>
        private const string C_hourHandMouseOverBorderBrushName = "HourHandMouseOverColor";

        /// <summary>
        /// Contains the name of the default HourHandPressedBrushName
        /// </summary>
        private const string C_hourHandPressedBrushName = "HourHandPressedColor";

        /// <summary>
        /// Contains the name of the default MinuteHandPrssedBrushName
        /// </summary>
        private const string C_minuteHandPressedBrushName = "MinuteHandPressedColor";

        /// <summary>
        /// Contains the name of the default SecondHandPressedBrushName
        /// </summary>
        private const string C_secondHandPressedBrushName = "SecondHandPressedColor";
        #endregion

        #region Fields
        /// <summary>
        /// Contains time value.
        /// </summary>
        private DispatcherTimer m_timer1;

        /// <summary>
        /// Contains HourHand of the Clock.
        /// </summary>
        private Path m_hourHand = null;

        /// <summary>
        /// Contains MinuteHand of the Clock.
        /// </summary>
        private Path m_minuteHand = null;

        /// <summary>
        /// Contains SecondHand of the Clock.
        /// </summary>
        private Rectangle m_secondHand = null;

        /// <summary>
        /// Contains center of the Clock.
        /// </summary>
        private Ellipse m_centeredEllipse = null;

        /// <summary>
        /// Contains RotateTransform of the HourHand.
        /// </summary>
        private RotateTransform m_hourHandRotateTransform = null;

        /// <summary>
        /// Contains RotateTransform of the MinuteHand.
        /// </summary>
        private RotateTransform m_minuteHandRotateTransform = null;

        /// <summary>
        /// Contains RotateTransform of the SecondHand.
        /// </summary>
        private RotateTransform m_secondHandRotateTransform = null;

        /// <summary>
        /// Contains added/subtracted hours after dragging operations.
        /// </summary>
        private int m_hoursAdded = 0;

        /// <summary>
        /// Contains added/subtracted minutes after dragging operations.
        /// </summary>
        private int m_minutesAdded = 0;

        /// <summary>
        /// Contains added/substracted seconds after dragging operations.
        /// </summary>
        private int m_secondsAdded = 0;

        /// <summary>
        /// Command for down button.
        /// </summary>
        public static RoutedCommand m_AMPMSelect = new RoutedCommand();
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="Clock"/> class.
        /// </summary>
        static Clock()
        {
          //  EnvironmentTest.ValidateLicense(typeof(Clock));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Clock), new FrameworkPropertyMetadata(typeof(Clock)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Clock"/> class.
        /// </summary>
        public Clock()
        {
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(Clock));
            }

            CommandBinding amPMSelectBinding = new CommandBinding(m_AMPMSelect);
            amPMSelectBinding.Executed += new ExecutedRoutedEventHandler(ChangeAMPMSelectValue);
            CommandBindings.Add(amPMSelectBinding);
            m_timer1 = new DispatcherTimer();
            m_timer1.Interval = TimeSpan.FromMilliseconds(1000);
            m_timer1.Tick += new EventHandler(Timer_Tick);
            this.Loaded += new RoutedEventHandler(Clock_Loaded);
            this.Unloaded += new RoutedEventHandler(Clock_Unloaded);
        }
        internal bool timerstatus= true;

        void Clock_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_timer1 != null)
            {
                m_timer1.IsEnabled = true;
                m_timer1.Start();
            }
        }

        void Clock_Unloaded(object sender, RoutedEventArgs e)
        {

            if (m_timer1 != null)
            {
                m_timer1.IsEnabled = false;
                m_timer1.Stop();
                m_timer1 = null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
       public void Stop()
        {
            m_timer1.Stop();
            timerstatus = false;
        }
        /// <summary>
        /// 
        /// </summary>
       public void Start()
        {
            
            m_timer1.Start();
            timerstatus = true;
            
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed hour hand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed hour hand; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPressedHourHand
        {
            get
            {
                return (bool)GetValue(IsPressedHourHandProperty);
            }

            set
            {
                SetValue(IsPressedHourHandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed minute hand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed minute hand; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPressedMinuteHand
        {
            get
            {
                return (bool)GetValue(IsPressedMinuteHandProperty);
            }

            set
            {
                SetValue(IsPressedMinuteHandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed second hand.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pressed second hand; otherwise, <c>false</c>.
        /// </value>
        internal bool IsPressedSecondHand
        {
            get
            {
                return (bool)GetValue(IsPressedSecondHandProperty);
            }

            set
            {
                SetValue(IsPressedSecondHandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FirstBorderFrameRadius of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The first border frame radius.</value>
        internal double FirstBorderFrameRadius
        {
            get
            {
                return (double)GetValue(FirstBorderFrameRadiusProperty);
            }

            set
            {
                SetValue(FirstBorderFrameRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets SecondInnerBorderFrameRadius of the Clock. This is a dependency property.
        /// </summary>
        internal double SecondInnerBorderFrameRadius
        {
            get
            {
                return (double)GetValue(SecondInnerBorderFrameRadiusProperty);
            }

            set
            {
                SetValue(SecondInnerBorderFrameRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ThirdBorderFrameRadius of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The third border frame radius.</value>
        internal double ThirdBorderFrameRadius
        {
            get
            {
                return (double)GetValue(ThirdBorderFrameRadiusProperty);
            }

            set
            {
                SetValue(ThirdBorderFrameRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets CenteredEllipseRadius of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The centered ellipse radius.</value>
        internal double CenteredEllipseRadius
        {
            get
            {
                return (double)GetValue(CenteredEllipseRadiusProperty);
            }

            set
            {
                SetValue(CenteredEllipseRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets InnerClockGeneralWidth of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The width of the inner clock general.</value>
        internal double InnerClockGeneralWidth
        {
            get
            {
                return (double)GetValue(InnerClockGeneralWidthProperty);
            }

            set
            {
                SetValue(InnerClockGeneralWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bool value whether allow user to resize the clock or not. This is a dependency property.
        /// </summary>
        /// <value>CanResize property</value>
        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanResize.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register("CanResize", typeof(bool), typeof(Clock), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or sets InnerClockGeneralHeight of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The height of the inner clock general.</value>
        internal double InnerClockGeneralHeight
        {
            get
            {
                return (double)GetValue(InnerClockGeneralHeightProperty);
            }

            set
            {
                SetValue(InnerClockGeneralHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets second height value of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The height of the second hand.</value>
        internal double SecondHandHeight
        {
            get
            {
                return (double)GetValue(SecondHandHeightProperty);
            }

            set
            {
                SetValue(SecondHandHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets FrameWidth value of the Clock. This is a dependency property.
        /// </summary>
        /// <value>The width of the frame.</value>
        internal double FrameWidth
        {
            get
            {
                return (double)GetValue(FrameWidthProperty);
            }

            set
            {
                SetValue(FrameWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets the long time.
        /// </summary>
        /// <value>The long time.</value>
        internal string LongTime
        {
            get
            {
                return (string)GetValue(LongTimeProperty);
            }

            private set
            {
                SetValue(LongTimeProperty, value);
            }
        }
        #endregion

        #region Enums
        /// <summary>
        /// Positions for the AM/PM selector.
        /// </summary>
        public enum Position
        {
            /// <summary>
            /// Define the Top position
            /// </summary>
            Top,

            /// <summary>
            /// Define the Bottom position
            /// </summary>
            Bottom,

            /// <summary>
            /// Define the Right position
            /// </summary>
            Right,

            /// <summary>
            /// Define the Left position
            /// </summary>
            Left
        }
        #endregion

        #region Events

        //// <summary>
        // Event that is raised when DateTime property is changed.
        // </summary>
        // public static read-only RoutedEvent DateTimeChangedEvent =
        //// EventManager.RegisterRoutedEvent( "DateTimeChanged", RoutingStrategy.Bubble, typeof( RoutedPropertyChangedEventHandler<DateTime> ), typeof( Clock ) );
        #endregion

        #region Implementation
        /// <summary>
        /// Builds the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            FrameWidth = C_frameMinWidth + FrameBorderThickness.Left + FrameBorderThickness.Right + FrameInnerBorderThickness.Left + FrameInnerBorderThickness.Right;

            InitHands();
            InitHandsRotateTransform();
            m_centeredEllipse = Template.FindName(C_centeredEllipseName, this) as Ellipse;
            
          }

        /// <summary>
        /// Raises the Initialized event. 
        /// This method is invoked whenever IsInitialized is set to true internally. 
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            UpdateDateTime();
            
         }

        /// <summary>
        /// Initializes hands of the Clock.
        /// </summary>
        private void InitHands()
        {
            m_hourHand = Template.FindName(C_hourHandName, this) as Path;
            m_hourHand.MouseLeftButtonDown += new MouseButtonEventHandler(HourHand_MouseLeftButtonDown);
            m_minuteHand = Template.FindName(C_minuteHandName, this) as Path;
            m_minuteHand.MouseLeftButtonDown += new MouseButtonEventHandler(MinuteHand_MouseLeftButtonDown);
            m_secondHand = Template.FindName(C_secondHandName, this) as Rectangle;
            m_secondHand.MouseLeftButtonDown += new MouseButtonEventHandler(SecondHand_MouseLeftButtonDown);
        }

        /// <summary>
        /// Initializes RotateTransform of the Clock hands.
        /// </summary>
        private void InitHandsRotateTransform()
        {
            m_hourHandRotateTransform = Template.FindName(C_hourHandRotateTransformName, this) as RotateTransform;
            m_minuteHandRotateTransform = Template.FindName(C_minuteHandRotateTransformName, this) as RotateTransform;
            m_secondHandRotateTransform = Template.FindName(C_secondHandRotateTransformHandName, this) as RotateTransform;
        }

        /// <summary>
        /// Working of timer implementation.
        /// </summary>
        /// <param name="sender">changed object</param>
        /// <param name="e">EventArgs that contains event data.</param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if(timerstatus)
            UpdateDateTime();
        }

               
        /// <summary>
        /// DateTime initialization.
        /// </summary>
        private void UpdateDateTime()
        {
            if (m_secondsAdded != 0)
            {
                DateTime += TimeSpan.FromSeconds(m_secondsAdded);
                m_secondsAdded = 0;
            }

            if (m_minutesAdded != 0)
            {
                DateTime += TimeSpan.FromMinutes(m_minutesAdded);
                m_minutesAdded = 0;
            }

            if (m_hoursAdded != 0)
            {
                DateTime += TimeSpan.FromHours(m_hoursAdded);
                m_hoursAdded = 0;
            }

            if (DateTime != DateTime.MaxValue)
            {
             
                DateTime = DateTime + TimeSpan.FromSeconds(1);
            }
        }

        //// <summary>
        // Raises the DateTimeChanged event. 
        // </summary>
        // <param name="oldValue">old value of the DateTime</param>
        // <param name="newValue">new value of the DateTime</param>
        // protected virtual void OnDateTimeChanged( DateTime oldValue, DateTime newValue )
        // {
        //    RoutedPropertyChangedEventArgs<DateTime> args = new RoutedPropertyChangedEventArgs<DateTime>( oldValue, newValue );
        //    args.RoutedEvent = Clock.DateTimeChangedEvent;
        //    RaiseEvent( args );
        // }
        // <summary>
        // Is called when DateTime property was changed.
        // </summary>
        // <param name="d">changed object</param>
        // <param name="e">DependencyPropertyChangedEventArgs</param>
        // private static void OnDateTimeInvalidated( DependencyObject d, DependencyPropertyChangedEventArgs e )
        // {
        //    Clock clock = ( Clock )d;
        //    DateTime oldValue = ( DateTime )e.OldValue;
        //    DateTime newValue = ( DateTime )e.NewValue;
        //    clock.LongTime = newValue.ToString( "T", new CultureInfo( "en-US" ) );
        //    clock.OnDateTimeChanged( oldValue, newValue );
        //// }

        /// <summary>
        /// Raises DateTimeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDateTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateTimeChanged != null)
            {
                DateTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDateTimeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clock instance = (Clock)d;

            DateTime newValue = (DateTime)e.NewValue;
            instance.LongTime = newValue.ToString("T", new CultureInfo("en-US"));

            instance.OnDateTimeChanged(e);
        }

        /// <summary>
        /// Invoked when MouseLeftButtonDown event of the HourHand is raised.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance that contains the event data.</param>
        private void HourHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_timer1.Stop();
            IsPressedHourHand = true;
        }

        /// <summary>
        /// Invoked when MouseLeftButtonDown event of the MinuteHand event is raised.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance that contains the event data.</param>
        private void MinuteHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_timer1.Stop();
            IsPressedMinuteHand = true;
        }

        /// <summary>
        /// Invoked when MouseLeftButtonDown event of the SecondHand event is raised.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance that contains the event data.</param>
        private void SecondHand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_timer1.Stop();
            IsPressedSecondHand = true;
        }

        /// <summary>
        /// Invoked when MouseMove event is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(m_centeredEllipse);
                double angle = Math.Atan2(p.Y, p.X);
                double transformAngle = (angle * C_angleOffset / Math.PI) + C_rotationOffset;

                if (transformAngle < 0)
                {
                    transformAngle = 360 + transformAngle;
                }

                TimeSpan now = new TimeSpan(DateTime.Hour, DateTime.Minute, DateTime.Second);

                if (IsPressedHourHand && DateTime != DateTime.MaxValue)
                {
                    TimeSpan hourTimeSpan = TimeSpan.FromHours(transformAngle / 30);
                    TimeSpan hourTimeSpanDiff = hourTimeSpan - now;
                    int hourDiff = CorrectHourDifference(hourTimeSpanDiff.Hours);

                    DateTime += TimeSpan.FromHours(hourDiff);
                }
                else if (IsPressedMinuteHand && DateTime != DateTime.MaxValue)
                {
                    TimeSpan minuteTimeSpan = TimeSpan.FromMinutes(transformAngle / 6);
                    int minDiff = CorrectTimeDifference(minuteTimeSpan.Minutes - now.Minutes);

                    DateTime += TimeSpan.FromMinutes(minDiff);
                }
                else if (IsPressedSecondHand && DateTime != DateTime.MaxValue)
                {
                    TimeSpan secTimeSpan = TimeSpan.FromSeconds(transformAngle / 6);
                    int secDiff = CorrectTimeDifference(secTimeSpan.Seconds - now.Seconds);

                    DateTime += TimeSpan.FromSeconds(secDiff);
                }
            }
        }

        /// <summary>
        /// Is used for correct moving hour hand when mouse dragging.
        /// </summary>
        /// <param name="diff">difference between "dragged" time and now time</param>
        /// <returns>corrected value</returns>
        private int CorrectHourDifference(int diff)
        {
            if (diff < -10)
            {
                diff += 12;
            }

            if (diff > 10)
            {
                diff -= 12;
            }

            return diff;
        }

        /// <summary>
        /// Is used for correct moving second/minute hands when mouse dragging.
        /// </summary>
        /// <param name="diff">difference between "dragged" minutes/seconds and now time</param>
        /// <returns>corrected value</returns>
        private int CorrectTimeDifference(int diff)
        {
            if (diff > 50)
            {
                diff = -1;
            }

            if (diff < -50)
            {
                diff = 1;
            }

            return diff;
        }

        /// <summary>
        /// Invoked when OnMouseWheel event is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (IsCtrlAltPressed())
            {
                SecondMouseWheelRotation(e.Delta);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                MinuteMouseWheelRotation(e.Delta);
            }
            else if (Keyboard.Modifiers == ModifierKeys.None)
            {
                HourMouseWheelRotation(e.Delta);
            }
        }

        /// <summary>
        /// Invoked when MouseUp event is raised.
        /// </summary>
        /// <param name="e">The instance that contains the event data.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            ClearDraggingFlags();
            m_timer1.Start();
        }

        /// <summary>
        /// Defines if Ctrl and Alt keys were pressed.
        /// </summary>
        /// <returns>Returns true if Ctrl and Alt keys were pressed.</returns>
        private bool IsCtrlAltPressed()
        {
            return (Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt)) && (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl));
        }

        /// <summary>
        /// Clears dragging flags for HourHand, MinuteHand and SecondHand.
        /// </summary>
        private void ClearDraggingFlags()
        {
            IsPressedHourHand = false;
            IsPressedMinuteHand = false;
            IsPressedSecondHand = false;
        }

        /// <summary>
        /// Rotate hour hand of the clock when mouse wheel.
        /// </summary>
        /// <param name="delta">Value that indicates the amount that the mouse wheel has changed</param>
        private void HourMouseWheelRotation(int delta)
        {
            if (delta < 0)
            {
                TimeSpan hourTimeSpan = TimeSpan.FromHours(m_hourHandRotateTransform.Angle / 30);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, hourTimeSpan.Hours, DateTime.Minute, DateTime.Second);

                m_hoursAdded++;
            }
            else
            {
                TimeSpan hourTimeSpan = TimeSpan.FromHours(m_hourHandRotateTransform.Angle / 30);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, hourTimeSpan.Hours, DateTime.Minute, DateTime.Second);

                m_hoursAdded--;
            }
        }

        /// <summary>
        /// Rotate minute hand of the clock when mouse wheel.
        /// </summary>
        /// <param name="delta">Value that indicates the amount that the mouse wheel has changed</param>
        private void MinuteMouseWheelRotation(int delta)
        {
            if (delta < 0)
            {
                TimeSpan minuteTimeSpan = TimeSpan.FromHours(m_minuteHandRotateTransform.Angle / 360);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, minuteTimeSpan.Minutes, DateTime.Second);

                m_minutesAdded++;
            }
            else
            {
                TimeSpan minuteTimeSpan = TimeSpan.FromHours(m_minuteHandRotateTransform.Angle / 360);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, minuteTimeSpan.Minutes, DateTime.Second);

                m_minutesAdded--;
            }
        }

        /// <summary>
        /// Rotate second hand of the clock when mouse wheel.
        /// </summary>
        /// <param name="delta">Value that indicates the amount that the mouse wheel has changed</param>
        private void SecondMouseWheelRotation(int delta)
        {
            if (delta < 0)
            {
                TimeSpan secondTimeSpan = TimeSpan.FromMinutes(m_secondHandRotateTransform.Angle / 360);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, secondTimeSpan.Seconds);

                m_secondsAdded++;
            }
            else
            {
                TimeSpan secondTimeSpan = TimeSpan.FromMinutes(m_secondHandRotateTransform.Angle / 360);
                DateTime = new DateTime(DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, secondTimeSpan.Seconds);

                m_secondsAdded--;
            }
        }

        /// <summary>
        /// Changes AM/PM selector value.
        /// </summary>
        /// <param name="sender">The clock control</param>
        /// <param name="e">instance containing event data.</param>
        private void ChangeAMPMSelectValue(object sender, ExecutedRoutedEventArgs e)
        {
            FrameworkElement originalSource = e.OriginalSource as FrameworkElement;
            DateTime date = DateTime.MinValue;
            date = date.AddDays(1);

            if (LongTime.Contains("PM") && (originalSource.Name == C_upRepeatButtonName || originalSource.Name == C_upInnerRepeatButtonName) && DateTime != DateTime.MaxValue)
            {
                DateTime += TimeSpan.FromHours(12);
            }
            else if (LongTime.Contains("AM") && (originalSource.Name == C_downRepeatButtonName || originalSource.Name == C_downInnerRepeatButtonName) && date < DateTime)
            {
                DateTime -= TimeSpan.FromHours(12);
            }
        }
        #endregion

        #region Dependency Properties
        /// <summary>
        /// This property defines IsPressedHourHand of the Clock.
        /// </summary>
        internal static readonly DependencyProperty IsPressedHourHandProperty =
            DependencyProperty.Register(
            "IsPressedHourHand",
            typeof(bool),
            typeof(Clock),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// This property defines IsPressedMinuteHand of the Clock.
        /// </summary>
        internal static readonly DependencyProperty IsPressedMinuteHandProperty =
            DependencyProperty.Register(
            "IsPressedMinuteHand",
            typeof(bool),
            typeof(Clock),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// This property defines IsPressedSecondHand of the Clock.
        /// </summary>
        internal static readonly DependencyProperty IsPressedSecondHandProperty =
            DependencyProperty.Register(
            "IsPressedSecondHand",
            typeof(bool),
            typeof(Clock),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// This property defines FirstBorderFrameRadius of the Clock.
        /// </summary>
        internal static readonly DependencyProperty FirstBorderFrameRadiusProperty =
            DependencyProperty.Register(
            "FirstBorderFrameRadius",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_firstBorderFrameRadius));

        /// <summary>
        /// This property defines SecondInnerBorderFrameRadius of the Clock.
        /// </summary>
        internal static readonly DependencyProperty SecondInnerBorderFrameRadiusProperty =
            DependencyProperty.Register(
            "SecondInnerBorderFrameRadius",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_secondInnerBorderFrameRadius));

        /// <summary>
        /// This property defines ThirdBorderFrameRadius of the Clock.
        /// </summary>
        internal static readonly DependencyProperty ThirdBorderFrameRadiusProperty =
            DependencyProperty.Register(
            "ThirdBorderFrameRadius",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_thirdBorderFrameRadius));

        /// <summary>
        /// This property defines CenteredEllipseRadius of the Clock.
        /// </summary>
        internal static readonly DependencyProperty CenteredEllipseRadiusProperty =
            DependencyProperty.Register(
            "CenteredEllipseRadius",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_centeredEllipseRadius));

        /// <summary>
        /// This property defines InnerClockGeneralWidth of the Clock.
        /// </summary>
        internal static readonly DependencyProperty InnerClockGeneralWidthProperty =
            DependencyProperty.Register(
            "InnerClockGeneralWidth",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_innerClockGeneralWidth));

        /// <summary>
        /// This property defines InnerClockGeneralHeight of the Clock.
        /// </summary>
        internal static readonly DependencyProperty InnerClockGeneralHeightProperty =
            DependencyProperty.Register(
            "InnerClockGeneralHeight",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(C_innerClockGeneralHeight));

        /// <summary>
        /// This property defines SecondHandHeight of the Clock second hand.
        /// </summary>
        internal static readonly DependencyProperty SecondHandHeightProperty =
            DependencyProperty.Register(
            "SecondHandHeight",
            typeof(double),
            typeof(Clock),
            new FrameworkPropertyMetadata(52d));

        /// <summary>
        /// This property defines SecondHandHeight of the Clock second hand.
        /// </summary>
        internal static readonly DependencyProperty FrameWidthProperty =
            DependencyProperty.Register("FrameWidth", typeof(double), typeof(Clock), new FrameworkPropertyMetadata(0d));

        /// <summary>
        /// Identifies LongTime dependency property.
        /// </summary>
        internal static DependencyProperty LongTimeProperty = DependencyProperty.Register(
               "LongTime", typeof(string), typeof(Clock));
        #endregion
    }
}
